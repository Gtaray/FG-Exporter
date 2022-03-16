using FGE.Extensions;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace FGE
{
    internal class Converter
    {
        ExportConfig Config { get; set; }
        string CampaignFolder { get; set; }

        XElement DB;
        List<ImageRecord> Images = new List<ImageRecord>();
        public static Dictionary<RecordKey, RecordValue> Library = new Dictionary<RecordKey, RecordValue>();

        public Converter(ExportConfig config, string campaignFolder)
        {
            this.Config = config;
            this.CampaignFolder = campaignFolder;

            DB = XElement.Load(Path.Combine(CampaignFolder, "db.xml"));
        }

        public void Export()
        {
            Library = BuildLibraryDictionary();
            XElement export = new XElement("root");

            // ---------------------------------------------
            // Build db.xml / client.xml
            // ---------------------------------------------
            foreach (KeyValuePair<RecordKey, RecordValue> item in Library)
            {
                AddExportNode(export, item.Key, item.Value);
            }

            // Remove all locked, allowplayerdrawing, and public nodes from every element
            export.Descendants("locked").Remove();
            export.Descendants("allowplayerdrawing").Remove();
            export.Descendants("public").Remove();

            // Sort the elements directly under the root alphabetically
            //var sorted = export.Elements().OrderBy(e => e.Name.ToString()).ToList();
            //export.RemoveAll();
            //export.Add(sorted);
            export.SortElements();
            var entries = export.Descendants("entries").FirstOrDefault();
            if (entries != null)
            {
                entries.SortElements();
            }
            

            // Sort elements in the reference element
            if (export.Element("reference") != null)
                export.Element("reference").SortElements();

            // Add the attributes to the root element
            export.Add(new XAttribute("version", DB.Attributes().First(a => a.Name == "version").Value));
            export.Add(new XAttribute("dataversion", DB.Attributes().First(a => a.Name == "dataversion").Value));
            export.Add(new XAttribute("release", DB.Attributes().First(a => a.Name == "release").Value));

            XDocument db = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                export
            );

            // ---------------------------------------------
            // Build definition.xml
            // ---------------------------------------------
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


            // ---------------------------------------------
            // Write to output
            // ---------------------------------------------
            // If it doesn't exist, create output directory
            Directory.CreateDirectory(Config.OutputFolder);

            // Create temporary working directory
            var workingdir = Directory.CreateDirectory(Path.Combine(Config.OutputFolder, $"working_{Config.FileName}"));

            ExportXDoc(db, workingdir.FullName, Config.PlayerModule ? "client.xml" : "db.xml");
            ExportXDoc(definition, workingdir.FullName, "definition.xml");

            // Add the thumbnail
            if (!string.IsNullOrEmpty(Config.Thumbnail))
            {
                string thumbnailPath = Path.Combine(Directory.GetCurrentDirectory(), Config.Thumbnail);
                if (!File.Exists(thumbnailPath))
                {
                    throw new ArgumentException($"Could not find thumbnail {thumbnailPath}");
                }
                string thumbnailName = Path.GetFileName(thumbnailPath);
                File.Copy(thumbnailPath, Path.Combine(workingdir.FullName, thumbnailName));
            }

            // Get all the images into the module folder
            GatherImages(workingdir.FullName);

            // zip the working dir into a module zip archive
            string modFileName = Path.Combine(Config.OutputFolder, Config.FileName + ".mod");
            ZipFile.CreateFromDirectory(workingdir.FullName, modFileName);

            // Delete temporary working directory
            workingdir.Delete(true);
        }

        Dictionary<RecordKey, RecordValue> BuildLibraryDictionary()
        {
            Dictionary<RecordKey, RecordValue> dict = new Dictionary<RecordKey, RecordValue>();

            foreach (RecordTypeEntry typeconfig in Config.RecordTypes)
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

        Dictionary<RecordKey, RecordValue> GetDbRecords(XElement parent, RecordTypeEntry typeconfig, bool noCategory = false)
        {
            Dictionary<RecordKey, RecordValue> dict = new Dictionary<RecordKey, RecordValue>();

            // Start by getting all records in the parent element
            IEnumerable<XElement> elementRecords = parent.Elements().Where(e => e.Name.ToString().StartsWith("id-"));

            // If the config specifies only specific records, then trim elementRecords to only ones that are listed in the config
            if (typeconfig.Records.Count > 0)
            {
                elementRecords = elementRecords.Where(e => typeconfig.Records.Contains(e.Name.ToString()));
            }

            // Iterate through those elements and create the dictionary
            foreach (var record in elementRecords)
            {
                // If noCategory is true, then don't add the category to the key
                RecordKey key = new RecordKey(
                    typeconfig.RecordType,
                    noCategory ? null : parent.Attribute("name").Value,
                    record.Name.ToString());

                // When processing images we need to do some extra handling to make sure the bitmaps are handled
                if (string.Equals(typeconfig.RecordType, "image", StringComparison.OrdinalIgnoreCase))
                {
                    XElement bElement = record.Element("image").Element("layers").Element("layer").Element("bitmap");
                    string bitmap = bElement?.Value ?? "";
                    var imageRecord = new ImageRecord(record.Name.ToString(), bitmap);

                    // Normalize bitmap path
                    bElement.SetValue(imageRecord.ModuleBitmap);

                    // Add for later handling
                    Images.Add(imageRecord);
                }

                if (!dict.ContainsKey(key))
                    dict[key] = new RecordValue(
                        record,
                        typeconfig);
            }

            return dict;
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
                        "reference", 
                        new XAttribute("static", "true"));
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
            RecordTypeEntry typeconfig = Config.RecordTypes
                .FirstOrDefault(t => t.RecordType == key.RecordType);
            if (typeconfig == null)
                throw new InvalidOperationException($"Could not find configuration for record type {key.RecordType}");

            string sName = Regex.Replace(Config.Name, @"\s+", "").ToLower();

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
            XElement recordtype = entries.Element(typeconfig.RecordType);
            if (recordtype == null)
            {
                recordtype = new XElement(typeconfig.RecordType);
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

        // Gathers all of the images and makes sure they exist
        void GatherImages(string destination)
        {
            foreach (var imageRecord in Images)
            {
                string root = imageRecord.Source == ImageRecordSource.Data
                    ? Config.DataImagesFolder
                    : CampaignFolder;
                string path = Path.Combine(root, imageRecord.ModuleBitmap);

                if (!File.Exists(path))
                {
                    throw new ArgumentException($"Bitmap file not found for image record {imageRecord.Id}. File: {path} ");
                }

                string output = Path.Combine(destination, imageRecord.ModuleBitmap);
                Directory.CreateDirectory(Path.GetDirectoryName(output));
                File.Copy(path, output);
            }
        }

        void ExportXDoc(XDocument doc, string dir, string filename)
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
