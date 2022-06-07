using FGE.Entities;
using FGE.Entities.Configuration;
using FGE.Models.Extensions;
using FGE.Models.PostProcessors;
using FGE.Models.PreProcessors;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace FGE.Models
{
    public class Converter
    {
        public ExportConfig Config { get; set; }
        public string CampaignFolder { get; set; }
        public string OutputFolder { get; set; }
        public string DataFolder { get; set; }
        public string Thumbnail { get; set; }
        public XElement DB { get; set; }
        public List<ImageRecord> Images { get; set; } = new List<ImageRecord>();
        public Dictionary<RecordKey, RecordValue> Library = new Dictionary<RecordKey, RecordValue>();

        public XElement Export { get; set; }

        List<IPreProcessor> PreProcessors { get; set; } = new List<IPreProcessor>();
        List<IPostProcessor> PostProcessors { get; set; } = new List<IPostProcessor>();

        public Converter(ExportConfig config, string campaignFolder, string outputFolder, string dataFolder, string thumbnail)
        {
            this.Config = config;
            this.CampaignFolder = campaignFolder;
            this.OutputFolder = outputFolder;
            this.DataFolder = dataFolder;
            this.Thumbnail = thumbnail;

            this.PreProcessors.Add(new GhostWriterPreProcessor());

            this.PostProcessors.Add(new UpdateReferenceLinks());
            this.PostProcessors.Add(new AddKeywordsToRefPages());
            this.PostProcessors.Add(new UpdateClassAbilityReferenceLinks());
            this.PostProcessors.Add(new UpdateTableReferenceLinks());
            this.PostProcessors.Add(new UpdateRacialTraitReferenceLinks());

            DB = XElement.Load(Path.Combine(CampaignFolder, "db.xml"));
        }

        public void ExportModule()
        {
            // Run pre-processors
            foreach (var processor in PreProcessors)
            {
                if (processor.ShouldRun(this))
                    processor.Process(this);
            }

            // Read in the campaign db.xml
            Library = BuildLibraryDictionary();

            // Build db.xml / client.xml
            XDocument db = BuildDbXml();

            // Run post processors
            foreach (var processor in PostProcessors)
            {
                if (processor.ShouldRun(this))
                    processor.Process(this);
            }

            // Build definition.xml
            XDocument definition = BuildDefinitionXml();

            // ---------------------------------------------
            // Write to output
            // ---------------------------------------------
            // If it doesn't exist, create output directory
            Directory.CreateDirectory(OutputFolder);

            string outputDirString = Path.Combine(OutputFolder, $"working_{Config.FileName}");

            // If the working directly already exists due to a failed previous attempt, delete it
            if (Directory.Exists(outputDirString))
                Directory.Delete(outputDirString, true);

            // Create temporary working directory
            var outputDir = Directory.CreateDirectory(outputDirString);

            SaveXDocToFolder(db, outputDir.FullName, Config.PlayerModule ? "client.xml" : "db.xml");
            SaveXDocToFolder(definition, outputDir.FullName, "definition.xml");

            // Add the thumbnail
            if (!string.IsNullOrEmpty(Thumbnail))
            {
                string thumbnailPath = Path.Combine(Directory.GetCurrentDirectory(), Thumbnail);
                if (!File.Exists(thumbnailPath))
                {
                    throw new ArgumentException($"Could not find thumbnail {thumbnailPath}");
                }
                string thumbnailName = Path.GetFileName(thumbnailPath);
                File.Copy(thumbnailPath, Path.Combine(outputDir.FullName, thumbnailName));
            }

            // Gather tokens that aren't on npcs
            GetExportedTokens();

            // Get all the images into the module folder
            CopyImagesToWorkingDirectory(outputDir.FullName);

            // if the mod file already exists, delete it
            string modFileName = Path.Combine(OutputFolder, Config.FileName + ".mod");
            if (File.Exists(modFileName))
                File.Delete(modFileName);

            ZipFile.CreateFromDirectory(outputDir.FullName, modFileName);

            // Delete temporary working directory
            outputDir.Delete(true);
        }

        Dictionary<RecordKey, RecordValue> BuildLibraryDictionary()
        {
            Dictionary<RecordKey, RecordValue> dict = new Dictionary<RecordKey, RecordValue>();

            foreach (RecordTypeConfig typeconfig in Config.RecordTypes)
            {
                string dbpath = string.IsNullOrEmpty(typeconfig.DbPath) 
                    ? typeconfig.RecordType 
                    : typeconfig.DbPath;


                var recordTypeElement = DB.Element(dbpath);
                if (recordTypeElement == null) continue;

                // Get records that aren't in a category
                dict.AddMany(GetDbRecords(recordTypeElement, typeconfig, true));

                // Get all cateogry elements within the record type
                foreach (var category in recordTypeElement.Elements("category"))
                {
                    dict.AddMany(GetDbRecords(category, typeconfig));
                }
            }

            return dict;
        }

        Dictionary<RecordKey, RecordValue> GetDbRecords(XElement parent, RecordTypeConfig typeconfig, bool noCategory = false)
        {
            Dictionary<RecordKey, RecordValue> dict = new Dictionary<RecordKey, RecordValue>();

            // Start by getting all records in the parent element
            IEnumerable<XElement> elementRecords = parent
                .Elements()
                .Where(
                    e => e.Name.ToString().StartsWith("id-") ||
                    e.Name.ToString().StartsWith("refmanual"));

            // If the config specifies only specific records, then trim elementRecords to only ones that are listed in the config
            if (typeconfig.Records.Count > 0)
            {
                elementRecords = elementRecords
                    .Where(e => typeconfig.Records.Contains(e.Name.ToString()));
            }

            // Iterate through those elements and create the dictionary
            foreach (var record in elementRecords)
            {
                // If noCategory is true, then don't add the category to the key
                RecordKey key = new RecordKey(
                    typeconfig.RecordType,
                    noCategory ? null : parent.Attribute("name").Value,
                    record.Name.ToString());

                if (!dict.ContainsKey(key))
                    dict[key] = new RecordValue(
                        record,
                        typeconfig);
            }

            return dict;
        }
        
        // Fetch all image bitmaps from the export data
        void GetImageBitmaps(XElement export)
        {
            // Get image elements whose parent (ex. id-00001) don't have an imagelink element
            // since the imagelink element seems to only be part of the reference manual
            var imageElements = export
               .Descendants("image")
               .Where(e => e.Attribute("type")?.Value == "image")
               .Where(e =>
               {
                   var link = e.Parent.Element("imagelink");
                   var recordname = link?.Element("recordname");
                   // either the link class needs to be null or 
                   // the value in recordname needs to be null
                   bool hasRecordName = !string.IsNullOrEmpty(recordname?.Value);
                   return link == null || hasRecordName;
               });
            var bitmapElements = imageElements
                .Descendants("bitmap")
                .Where(e => !string.IsNullOrEmpty(e.Value))
                .Where(e => !e.Value.Contains("@"));   

            foreach (var bitmap in bitmapElements)
            {
                ImageRecord record = new ImageRecord(bitmap.Value, ImageType.Image);
                // Normalize bitmap path
                bitmap.SetValue(record.ModuleGraphic);

                Images.Add(record);
            }
        }

        // Fetch all referenceimage bitmaps from the export data
        void GetReferenceImageBitmaps(XElement export)
        {
            // We are specifically looking for the recordname element that
            // has no recordname value in there
            // That indicates that it's a reference manual image.
            var imageElements = export
               .Descendants("image")
               .Where(e => e.Attribute("type")?.Value == "image")
               .Where(e =>
               {
                   var link = e.Parent.Element("imagelink");
                   var recordname = link?.Element("recordname");
                   return link != null
                    && recordname != null
                    && string.IsNullOrEmpty(recordname.Value);
               });
            var bitmapElements = imageElements
                .Descendants("bitmap")
                .Where(e => !string.IsNullOrEmpty(e.Value))
                .Where(e => !e.Value.Contains("@"));

            foreach (var bitmap in bitmapElements)
            {
                ImageRecord record = new ImageRecord(bitmap.Value, ImageType.RefImage);
                // Normalize bitmap path
                bitmap.SetValue(record.ModuleGraphic);

                Images.Add(record);
            }
        }

        // Fetch all token png files from the export data
        void GetTokens(XElement export)
        {
            var tokenElements = export
                .Descendants("token")
                .Where(e => !string.IsNullOrEmpty(e.Value))
                .Where(e => !e.Value.Contains("@"));

            foreach (var token in tokenElements)
            {
                ImageRecord record = new ImageRecord(token.Value, ImageType.Token);
                token.SetValue(record.ModuleGraphic);
                Images.Add(record);
            }
        }

        // Get the tokens that are exported separate from NPCs
        void GetExportedTokens()
        {
            foreach (string token in Config.Tokens)
            {
                Images.Add(new ImageRecord(token, ImageType.Token));
            }
        }

        XDocument BuildDbXml()
        {
            Export = new XElement("root");

            foreach (KeyValuePair<RecordKey, RecordValue> item in Library)
            {
                AddExportNode(Export, item.Key, item.Value);
            }

            // Get the bitmaps and tokens
            GetImageBitmaps(Export);
            GetTokens(Export);
            GetReferenceImageBitmaps(Export);
            //GetImageFileFromDb(export, "bitmap", ImageType.Image);
            //GetImageFileFromDb(export, "token", ImageType.Token);

            // Remove all locked, allowplayerdrawing, and public nodes from every element
            Export.Descendants("locked").Remove();
            Export.Descendants("allowplayerdrawing").Remove();
            Export.Descendants("public").Remove();
            Export.Descendants("tokenlock").Remove();

            // Sort the elements directly under the root alphabetically
            Export.SortElements();
            var entries = Export.Descendants("entries").FirstOrDefault();
            if (entries != null)
            {
                entries.SortElements();
            }

            // Sort elements in the reference element
            if (Export.Element("reference") != null)
                Export.Element("reference").SortElements();

            // Add the attributes to the root element
            Export.Add(new XAttribute("version", DB.Attributes().First(a => a.Name == "version").Value));
            Export.Add(new XAttribute("dataversion", DB.Attributes().First(a => a.Name == "dataversion").Value));
            Export.Add(new XAttribute("release", DB.Attributes().First(a => a.Name == "release").Value));

            XDocument db = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                Export
            );

            return db;
        }

        // Adds XElement to the export xml node. Also creates any record type and category nodes if they don't exist
        void AddExportNode(XElement parent, RecordKey key, RecordValue record)
        {
            // Get or create the element for the record type (spell, item, npc, etc).
            // If module is read only, then create the reference element as well

            XElement tElement;
            if (Config.ReadOnly && !string.IsNullOrEmpty(record.ReferencePath))
            {
                XElement refElement = parent.Element("reference");
                if (refElement == null)
                {
                    refElement = new XElement(
                        "reference");
                    parent.Add(refElement);
                }
                tElement = refElement.Element(record.ReferencePath);
                if (tElement == null)
                {
                    tElement = new XElement(record.ReferencePath);
                    refElement.Add(tElement);
                }
            }
            else
            {
                string modulepath
                    = string.IsNullOrEmpty(record.ModulePath)
                    ? key.RecordType 
                    : record.ModulePath;
                tElement = parent.Element(modulepath);
                if (tElement == null)
                {
                    tElement = new XElement(modulepath);
                    parent.Add(tElement);
                }
            }

            // We add this afterwards because a read only reference manual 
            // Is a strange case where the root node is 'reference'.
            // so we need to check after it's added.
            if (Config.ReadOnly)
            {
                var refElement = parent.Element("reference");
                if (refElement != null && refElement.Attribute("static") == null)
                    refElement.Add(new XAttribute("static", "true"));
            }

            // Depending on if the record is in a category or not, we get or create the category. cElement is the parent of the record xml
            XElement cElement;
            if (key.Category != null)
            {
                // Get or create the element for the record category
                cElement = tElement.Elements("category").FirstOrDefault(e => e.Attribute("name")?.Value == key.Category);
                if (cElement == null)
                {
                    cElement = new XElement("category", new XAttribute("name", key.Category));
                    tElement.Add(cElement);
                }
            }
            else
            {
                // If there's no category, then simply set cElement to tElement
                cElement = tElement;
            }

            XElement rElement = cElement.Element(record.Record.Name);
            if (rElement != null)
            {
                Console.Error.WriteLine($"Attempted to add record that already exists. Record Type: {key.RecordType}, Category: {key.Category}, Record Name: {key.Id}");
                return;
            }

            cElement.Add(record.Record);

            // Add the record to the library
            if (string.IsNullOrEmpty(record.LibraryLabel))
            {
                return;
            }

            AddExportLibraryNode(parent, key, record);
        }

        // Adds records to the export's library node.
        void AddExportLibraryNode(XElement parent, RecordKey key, RecordValue value)
        {
            // Get the configuration for this particular type
            RecordTypeConfig typeconfig = Config.RecordTypes
                .FirstOrDefault(t => t.RecordType == key.RecordType);
            if (typeconfig == null)
                throw new InvalidOperationException($"Could not find configuration for record type {key.RecordType}");

            string sName = Regex.Replace(Config.Name, @"[\s_]+", "").ToLower();

            // Get or create the library element
            XElement library = parent.Element("library");
            if (library == null)
            {
                library = new XElement("library");
                parent.Add(library);
            }
            // Get or create the module element within the library element
            XElement module = library.Element(sName);
            if (module == null)
            {
                module = new XElement(sName, new XAttribute("static", "true"));
                library.Add(module);
            }

            // Add these elements if they're not already there
            if (module.Element("categoryname") == null)
            {
                module.Add(new XElement("categoryname", Config.Category, new XAttribute("type", "string")));
            }
            if (module.Element("name") == null)
            {
                module.Add(new XElement("name", sName, new XAttribute("type", "string")));
            }

            // Get or create the entries element. This is the root of the actual library entries
            XElement entries = module.Element("entries");
            if (entries == null)
            {
                entries = new XElement("entries");
                module.Add(entries);
            }

            // Get or add the record type entry to the library
            // We use typeconfig.RecordType here specifically because of the tables data type
            // table objects are located in the 'tables' (plural) element of the db,
            // but the library lists them under 'table' (singular) element with record type 'table'. 
            // In that case, the objects go to the DB under 'tables', but are listed in the library as 'table'
            string libraryPath = string.IsNullOrEmpty(typeconfig.LibraryPath)
                ? typeconfig.RecordType
                : typeconfig.LibraryPath;
            XElement recordtype = entries.Element(libraryPath);
            if (recordtype == null)
            {
                recordtype = new XElement(libraryPath);
                entries.Add(recordtype);
            }

            // If the recordtype doesn't contain all the relevant data, add it
            if (recordtype.Element("librarylink") == null)
            {
                XElement librarylink = new XElement("librarylink", new XAttribute("type", "windowreference"));
                librarylink.Add(new XElement("class", value.LibrarylinkClass));
                librarylink.Add(new XElement("recordname", value.LibrarylinkRecordName));
                recordtype.Add(librarylink);
            }
            if (recordtype.Element("name") == null)
            {
                recordtype.Add(new XElement("name", value.LibraryLabel, new XAttribute("type", "string")));
            }
            if (recordtype.Element("recordtype") == null && value.IncludeLibraryRecordType)
            {
                recordtype.Add(new XElement("recordtype", typeconfig.RecordType, new XAttribute("type", "string")));
            }
        }

        XDocument BuildDefinitionXml()
        {
            XDocument definition = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("root",
                    new XAttribute("version", DB.Attributes().First(a => a.Name == "version").Value),
                    new XAttribute("dataversion", DB.Attributes().First(a => a.Name == "dataversion").Value),
                    new XAttribute("release", DB.Attributes().First(a => a.Name == "release").Value),
                    new XElement("name", Config.Name),
                    new XElement("displayname", Config.DisplayName),
                    new XElement("category", Config.Category),
                    new XElement("author", Config.Author)
                )
            );
            if (!Config.AnyRuleset)
            {
                definition
                    .Element("root")
                    .Add(new XElement("ruleset", Config.Ruleset));
            }

            return definition;
        }

        // Gathers all of the images and makes sure they exist
        void CopyImagesToWorkingDirectory(string destination)
        {
            foreach (var imageRecord in Images)
            {
                string root = imageRecord.Source == ImageRecordSource.Data
                    ? DataFolder
                    : CampaignFolder;
                string path = Path.Combine(root, imageRecord.SourceGraphic);

                if (!File.Exists(path))
                {
                    throw new ArgumentException($"Bitmap file not found. File: {path} ");
                }

                string output = Path.Combine(destination, imageRecord.ModuleGraphic);
                Directory.CreateDirectory(Path.GetDirectoryName(output));

                // If the file doesn't already exist, then copy it.
                // It could already exist if the image is used multiple times in the campaign
                if (!File.Exists(output))
                    File.Copy(path, output);
            }
        }

        void SaveXDocToFolder(XDocument doc, string dir, string filename)
        {
            // Save the xml file to the working directory
            // It will save either db.xml (gm modules) or client.xml (palyer moduels)
            string db = Path.Combine(dir, filename);
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            xws.IndentChars = "\t";
            using (FileStream fs = new FileStream(db, FileMode.Create))
            using (XmlWriter xw = XmlWriter.Create(fs, xws))
            {
                doc.Save(xw);
            }
        }
    }
}
