using CommandLine;
using FGE.Extensions;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace FGE {
    public class FantasyGroundsExporter
    {
        public static string CampaignFolder { get; set; }
        public static ExportConfig Config { get; set; }
        public static XElement DB { get; set; }
        public static List<ImageRecord> Images { get; set; } = new List<ImageRecord>();

        public static Dictionary<RecordKey, RecordValue> Library { get; set; } = new Dictionary<RecordKey, RecordValue>();
        public class Options
        {
            [Option('i', "input", Required = true, HelpText = "Path to the campaign folder that includes the db.xml to export.")]
            public string CampaignFolder { get; set; } = "";

            [Option('c', "config", Required = true, HelpText = "Path to json configuration file that defines the export.")]
            public string ConfigFile { get; set; } = "";
        }

        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                  .WithParsed<Options>(o =>
                  {
                      // Check that the campaign folder and db.xml file exist.
                      if (!Directory.Exists(o.CampaignFolder))
                      {
                          throw new ArgumentException("Campaign folder not found.");
                      }
                      string dbfile = Path.Combine(o.CampaignFolder, "db.xml");
                      if (!File.Exists(dbfile))
                      {
                          throw new ArgumentException("Campaign db.xml file not found.");
                      }
                      CampaignFolder = o.CampaignFolder;

                      // Check that the config file exists and has necessary data
                      if (!File.Exists(o.ConfigFile))
                      {
                          throw new ArgumentException("Configuration file not found");
                      }

                      var config = JsonConvert.DeserializeObject<ExportConfig>(File.ReadAllText(o.ConfigFile));

                      if (config == null)
                      {
                          throw new InvalidOperationException("Error deserializing configuration file.");
                      }

                      if (string.IsNullOrEmpty(config.Name))
                      {
                          throw new ArgumentException("Module name not specified");
                      }

                      if (string.IsNullOrEmpty(config.FileName))
                      {
                          throw new ArgumentException("Module file name not specified");
                      }

                      if (config.FileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                      {
                          throw new InvalidOperationException("Module file name contains invalid characters");
                      }

                      Config = config;
                      DB = XElement.Load(dbfile);

                      Library = BuildLibraryDictionary();
                      Export();
                  });
        }

        internal static Dictionary<RecordKey, RecordValue> BuildLibraryDictionary()
        {
            Dictionary<RecordKey, RecordValue> dict = new Dictionary<RecordKey, RecordValue>();

            foreach (RecordTypeEntry typeconfig in Config.RecordTypes)
            {
                string dbpath = string.IsNullOrEmpty(typeconfig.DbPath) ? typeconfig.RecordType : typeconfig.DbPath;
                

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

        internal static Dictionary<RecordKey, RecordValue> GetDbRecords(XElement parent, RecordTypeEntry typeconfig, bool noCategory = false)
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
                    dict[key] = new RecordValue(record, typeconfig.LibraryName, typeconfig.DbPath);
            }

            return dict;
        }

        internal static void Export()
        {
            XElement export = new XElement("root");

            // ---------------------------------------------
            // Build db.xml / client.xml
            // ---------------------------------------------
            foreach (KeyValuePair<RecordKey, RecordValue> item in Library)
            {
                AddExportNode(export, item.Key, item.Value);
            }

            // Remove all locked and allowplayerdrawing nodes from every element
            export.Descendants("locked").Remove();
            export.Descendants("allowplayerdrawing").Remove();

            // Sort the elements directly under the root alphabetically
            var sorted = export.Elements().OrderBy(e => e.Name.ToString()).ToList();
            export.RemoveAll();
            export.Add(sorted);

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
                    new XElement("author", Config.Author),
                    new XElement("ruleset", Config.Ruleset)
                )
            );


            // ---------------------------------------------
            // Write to output
            // ---------------------------------------------
            // If it doesn't exist, create output directory
            Directory.CreateDirectory(Config.OutputFolder);

            // Create temporary working directory
            var workingdir = Directory.CreateDirectory(Path.Combine(Config.OutputFolder, "working"));

            ExportXDoc(db, workingdir.FullName, Config.PlayerModule ? "client.xml" : "db.xml");
            ExportXDoc(definition, workingdir.FullName, "definition.xml");

            // Add the thumbnail
            if (!string.IsNullOrEmpty(Config.Thumbnail))
            {
                if (!File.Exists(Config.Thumbnail))
                {
                    throw new ArgumentException($"Could not find thumbnail {Config.Thumbnail}");
                }
                //File.Copy(Config.Thumbnail)
            }

            // Get all the images into the module folder
            GatherImages(workingdir.FullName);

            // zip the working dir into a module zip archive
            string modFileName = Path.Combine(Config.OutputFolder, Config.FileName + ".mod");
            ZipFile.CreateFromDirectory(workingdir.FullName, modFileName);

            // Delete temporary working directory
            workingdir.Delete(true);
        }

        // Gathers all of the images and makes sure they exist
        internal static void GatherImages(string destination)
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

        // Adds XElement to the export xml node. Also creates any record type and category nodes if they don't exist
        internal static void AddExportNode(XElement parent, RecordKey key, RecordValue record)
        {

            // Get or create the element for the record type (spell, item, npc, etc).
            string dbpath = string.IsNullOrEmpty(record.DbPath) ? key.RecordType : record.DbPath;
            XElement tElement = parent.Element(dbpath);
            if (tElement == null)
            {
                tElement = new XElement(dbpath);
                parent.Add(tElement);
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
        internal static void AddExportLibraryNode(XElement parent, RecordKey key, RecordValue value)
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
                librarylink.Add(new XElement("class", "reference_list"));
                librarylink.Add(new XElement("recordname", ".."));
                recordtype.Add(librarylink);
            }
            if (recordtype.Element("name") == null)
            {
                recordtype.Add(new XElement("name", value.LibraryLabel, new XAttribute("type", "string")));
            }
            if (recordtype.Element("recordtype") == null)
            {
                recordtype.Add(new XElement("recordtype", typeconfig.RecordType, new XAttribute("type", "string")));
            }
        }

        internal static void ExportXDoc(XDocument doc, string dir, string filename)
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

