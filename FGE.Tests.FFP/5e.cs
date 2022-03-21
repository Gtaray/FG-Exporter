using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Unicode;

namespace Tests
{
    [TestClass]
    public class UnitTests5e
    {
        public string campaignFolderPath = "files/5e/campaigns";
        public string output = "files/output";
        public string data = "files/5e";
        public string thumbnail = "files/5e/config/Thumbnail.png";

        // Tests all combinations of flags when exporting all content
        [TestMethod]
        public void NoFlagsExportAll()
        {
            string modfile = "files/output/5e_ExportAll.mod";
            string controlFolder = "files/5e/modules/Export All";
            string workingdir = "files/output/5e_ExportAll";
            string jsonfile = "files/5e/config/Export All.json";
            // Delete file from a previous run of this test
            if (File.Exists(modfile))
                File.Delete(modfile);
            if (Directory.Exists(workingdir))
                Directory.Delete(workingdir, true);

            // Get the test files
            string configfile = Path.Combine(Directory.GetCurrentDirectory(), jsonfile);
            string campaignFolder = Path.Combine(Directory.GetCurrentDirectory(), campaignFolderPath, "5e");

            // Run the converter
            FGE.FantasyGroundsExporter.Main(new string[] { "-i", campaignFolder, "-c", configfile, "-o", output, "-d", data });

            // Asserts
            Assert.IsTrue(File.Exists(modfile));

            using (var file = File.OpenRead(modfile))
            using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
            {
                Assert.AreEqual(zip.Entries.Count, 6);
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "db.xml"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "definition.xml"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Battlemap.jpg"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Test Map.jpg"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Sample Image/Test Map 2.jpg"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Sample Images/Test Map 3.jpg"));

                using (var stream = zip.Entries.First(e => e.FullName == "db.xml").Open())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string control = File.ReadAllText(Path.Combine(controlFolder, "db.xml"));
                    control = TestHelpers.ReplaceEscapedCharacters(control);
                    string xml = reader.ReadToEnd();
                    Assert.AreEqual(control, xml);
                }

                using (var stream = zip.Entries.First(e => e.FullName == "definition.xml").Open())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string control = File.ReadAllText(Path.Combine(controlFolder, "definition.xml"));
                    control = TestHelpers.ReplaceEscapedCharacters(control);
                    string xml = reader.ReadToEnd();
                    Assert.AreEqual(control, xml);
                }
            }
        }

        [TestMethod]
        public void NoFlagsExportWithThumbnail()
        {
            string modfile = "files/output/5e_Thumbnail.mod";
            string controlFolder = "files/5e/modules/Thumbnail";
            string workingdir = "files/output/5e_Thumbnail";
            string jsonfile = "files/5e/config/Thumbnail.json";

            // Delete file from a previous run of this test
            if (File.Exists(modfile))
                File.Delete(modfile);
            if (Directory.Exists(workingdir))
                Directory.Delete(workingdir, true);

            // Get the test files
            string configfile = Path.Combine(Directory.GetCurrentDirectory(), jsonfile);
            string campaignFolder = Path.Combine(Directory.GetCurrentDirectory(), campaignFolderPath, "5e");

            // Run the converter
            FGE.FantasyGroundsExporter.Main(new string[] { "-i", campaignFolder, "-c", configfile, "-o", output, "-d", data, "-t", thumbnail });

            // Asserts
            Assert.IsTrue(File.Exists(modfile));

            using (var file = File.OpenRead(modfile))
            using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
            {
                Assert.AreEqual(zip.Entries.Count, 3);
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "db.xml"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "definition.xml"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "Thumbnail.png"));

                using (var stream = zip.Entries.First(e => e.FullName == "db.xml").Open())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string control = File.ReadAllText(Path.Combine(controlFolder, "db.xml"));
                    control = TestHelpers.ReplaceEscapedCharacters(control);
                    string xml = reader.ReadToEnd();
                    Assert.AreEqual(control, xml);
                }

                using (var stream = zip.Entries.First(e => e.FullName == "definition.xml").Open())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string control = File.ReadAllText(Path.Combine(controlFolder, "definition.xml"));
                    control = TestHelpers.ReplaceEscapedCharacters(control);
                    string xml = reader.ReadToEnd();
                    Assert.AreEqual(control, xml);
                }
            }
        }

        [TestMethod]
        public void ReadOnlyExportAll()
        {
            string modfile = "files/output/5e_ReadOnly.mod";
            string controlFolder = "files/5e/modules/Read Only";
            string workingdir = "files/output/5e_ReadOnly";
            string jsonfile = "files/5e/config/Read Only.json";

            // Delete file from a previous run of this test
            if (File.Exists(modfile))
                File.Delete(modfile);
            if (Directory.Exists(workingdir))
                Directory.Delete(workingdir, true);

            // Get the test files
            string configfile = Path.Combine(Directory.GetCurrentDirectory(), jsonfile);
            string campaignFolder = Path.Combine(Directory.GetCurrentDirectory(), campaignFolderPath, "5e");

            // Run the converter
            FGE.FantasyGroundsExporter.Main(new string[] { "-i", campaignFolder, "-c", configfile, "-o", output, "-d", data });

            // Asserts
            Assert.IsTrue(File.Exists(modfile));

            using (var file = File.OpenRead(modfile))
            using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
            {
                Assert.AreEqual(zip.Entries.Count, 6);
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "db.xml"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "definition.xml"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Battlemap.jpg"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Test Map.jpg"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Sample Image/Test Map 2.jpg"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Sample Images/Test Map 3.jpg"));

                using (var stream = zip.Entries.First(e => e.FullName == "db.xml").Open())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string control = File.ReadAllText(Path.Combine(controlFolder, "db.xml"));
                    control = TestHelpers.ReplaceEscapedCharacters(control);
                    string xml = reader.ReadToEnd();
                    Assert.AreEqual(control, xml);
                }

                using (var stream = zip.Entries.First(e => e.FullName == "definition.xml").Open())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string control = File.ReadAllText(Path.Combine(controlFolder, "definition.xml"));
                    control = TestHelpers.ReplaceEscapedCharacters(control);
                    string xml = reader.ReadToEnd();
                    Assert.AreEqual(control, xml);
                }
            }
        }

        [TestMethod]
        public void PlayerModuleExportAll()
        {
            string modfile = "files/output/5e_PlayerModule.mod";
            string controlFolder = "files/5e/modules/Player Module";
            string workingdir = "files/output/5e_PlayerModule";
            string jsonfile = "files/5e/config/Player Module.json";

            // Delete file from a previous run of this test
            if (File.Exists(modfile))
                File.Delete(modfile);
            if (Directory.Exists(workingdir))
                Directory.Delete(workingdir, true);

            // Get the test files
            string configfile = Path.Combine(Directory.GetCurrentDirectory(), jsonfile);
            string campaignFolder = Path.Combine(Directory.GetCurrentDirectory(), campaignFolderPath, "5e");

            // Run the converter
            FGE.FantasyGroundsExporter.Main(new string[] { "-i", campaignFolder, "-c", configfile, "-o", output, "-d", data });

            // Asserts
            Assert.IsTrue(File.Exists(modfile));

            using (var file = File.OpenRead(modfile))
            using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
            {
                Assert.AreEqual(zip.Entries.Count, 6);
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "client.xml"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "definition.xml"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Battlemap.jpg"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Test Map.jpg"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Sample Image/Test Map 2.jpg"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Sample Images/Test Map 3.jpg"));

                using (var stream = zip.Entries.First(e => e.FullName == "client.xml").Open())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string control = File.ReadAllText(Path.Combine(controlFolder, "client.xml"));
                    control = TestHelpers.ReplaceEscapedCharacters(control);
                    string xml = reader.ReadToEnd();
                    Assert.AreEqual(control, xml);
                }

                using (var stream = zip.Entries.First(e => e.FullName == "definition.xml").Open())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string control = File.ReadAllText(Path.Combine(controlFolder, "definition.xml"));
                    control = TestHelpers.ReplaceEscapedCharacters(control);
                    string xml = reader.ReadToEnd();
                    Assert.AreEqual(control, xml);
                }
            }
        }

        [TestMethod]
        public void AnyRulesetExportAll()
        {
            string modfile = "files/output/5e_AnyRuleset.mod";
            string controlFolder = "files/5e/modules/Any Ruleset";
            string workingdir = "files/output/5e_AnyRuleset";
            string jsonfile = "files/5e/config/Any Ruleset.json";

            // Delete file from a previous run of this test
            if (File.Exists(modfile))
                File.Delete(modfile);
            if (Directory.Exists(workingdir))
                Directory.Delete(workingdir, true);

            // Get the test files
            string configfile = Path.Combine(Directory.GetCurrentDirectory(), jsonfile);
            string campaignFolder = Path.Combine(Directory.GetCurrentDirectory(), campaignFolderPath, "5e");

            // Run the converter
            FGE.FantasyGroundsExporter.Main(new string[] { "-i", campaignFolder, "-c", configfile, "-o", output, "-d", data });

            // Asserts
            Assert.IsTrue(File.Exists(modfile));

            using (var file = File.OpenRead(modfile))
            using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
            {
                Assert.AreEqual(zip.Entries.Count, 6);
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "db.xml"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "definition.xml"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Battlemap.jpg"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Test Map.jpg"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Sample Image/Test Map 2.jpg"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Sample Images/Test Map 3.jpg"));

                using (var stream = zip.Entries.First(e => e.FullName == "db.xml").Open())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string control = File.ReadAllText(Path.Combine(controlFolder, "db.xml"));
                    control = TestHelpers.ReplaceEscapedCharacters(control);
                    string xml = reader.ReadToEnd();
                    Assert.AreEqual(control, xml);
                }

                using (var stream = zip.Entries.First(e => e.FullName == "definition.xml").Open())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string control = File.ReadAllText(Path.Combine(controlFolder, "definition.xml"));
                    control = TestHelpers.ReplaceEscapedCharacters(control);
                    string xml = reader.ReadToEnd();
                    Assert.AreEqual(control, xml);
                }
            }
        }

        [TestMethod]
        public void ReadOnlyPlayerModuleExportAll()
        {
            string modfile = "files/output/5e_ReadOnlyPlayerModule.mod";
            string controlFolder = "files/5e/modules/Read Only, Player Module";
            string workingdir = "files/output/5e_ReadOnlyPlayerModule";
            string jsonfile = "files/5e/config/Read Only, Player Module.json";

            // Delete file from a previous run of this test
            if (File.Exists(modfile))
                File.Delete(modfile);
            if (Directory.Exists(workingdir))
                Directory.Delete(workingdir, true);

            // Get the test files
            string configfile = Path.Combine(Directory.GetCurrentDirectory(), jsonfile);
            string campaignFolder = Path.Combine(Directory.GetCurrentDirectory(), campaignFolderPath, "5e");

            // Run the converter
            FGE.FantasyGroundsExporter.Main(new string[] { "-i", campaignFolder, "-c", configfile, "-o", output, "-d", data });

            // Asserts
            Assert.IsTrue(File.Exists(modfile));

            using (var file = File.OpenRead(modfile))
            using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
            {
                Assert.AreEqual(zip.Entries.Count, 6);
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "client.xml"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "definition.xml"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Battlemap.jpg"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Test Map.jpg"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Sample Image/Test Map 2.jpg"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Sample Images/Test Map 3.jpg"));

                using (var stream = zip.Entries.First(e => e.FullName == "client.xml").Open())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string control = File.ReadAllText(Path.Combine(controlFolder, "client.xml"));
                    control = TestHelpers.ReplaceEscapedCharacters(control);
                    string xml = reader.ReadToEnd();
                    Assert.AreEqual(control, xml);
                }

                using (var stream = zip.Entries.First(e => e.FullName == "definition.xml").Open())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string control = File.ReadAllText(Path.Combine(controlFolder, "definition.xml"));
                    control = TestHelpers.ReplaceEscapedCharacters(control);
                    string xml = reader.ReadToEnd();
                    Assert.AreEqual(control, xml);
                }
            }
        }

        [TestMethod]
        public void AnyRulesetReadOnlyExportAll()
        {
            string modfile = "files/output/5e_AnyRulesetReadOnly.mod";
            string controlFolder = "files/5e/modules/Any Ruleset, Read Only";
            string workingdir = "files/output/5e_AnyRulesetReadOnly";
            string jsonfile = "files/5e/config/Any Ruleset, Read Only.json";

            // Delete file from a previous run of this test
            if (File.Exists(modfile))
                File.Delete(modfile);
            if (Directory.Exists(workingdir))
                Directory.Delete(workingdir, true);

            // Get the test files
            string configfile = Path.Combine(Directory.GetCurrentDirectory(), jsonfile);
            string campaignFolder = Path.Combine(Directory.GetCurrentDirectory(), campaignFolderPath, "5e");

            // Run the converter
            FGE.FantasyGroundsExporter.Main(new string[] { "-i", campaignFolder, "-c", configfile, "-o", output, "-d", data });

            // Asserts
            Assert.IsTrue(File.Exists(modfile));

            using (var file = File.OpenRead(modfile))
            using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
            {
                Assert.AreEqual(zip.Entries.Count, 6);
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "db.xml"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "definition.xml"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Battlemap.jpg"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Test Map.jpg"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Sample Image/Test Map 2.jpg"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Sample Images/Test Map 3.jpg"));

                using (var stream = zip.Entries.First(e => e.FullName == "db.xml").Open())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string control = File.ReadAllText(Path.Combine(controlFolder, "db.xml"));
                    control = TestHelpers.ReplaceEscapedCharacters(control);
                    string xml = reader.ReadToEnd();
                    Assert.AreEqual(control, xml);
                }

                using (var stream = zip.Entries.First(e => e.FullName == "definition.xml").Open())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string control = File.ReadAllText(Path.Combine(controlFolder, "definition.xml"));
                    control = TestHelpers.ReplaceEscapedCharacters(control);
                    string xml = reader.ReadToEnd();
                    Assert.AreEqual(control, xml);
                }
            }
        }

        [TestMethod]
        public void AnyRulesetPlayerModuleExportAll()
        {
            string modfile = "files/output/5e_AnyRulesetPlayerModule.mod";
            string controlFolder = "files/5e/modules/Any Ruleset, Player Module";
            string workingdir = "files/output/5e_AnyRulesetPlayerModule";
            string jsonfile = "files/5e/config/Any Ruleset, Player Module.json";

            // Delete file from a previous run of this test
            if (File.Exists(modfile))
                File.Delete(modfile);
            if (Directory.Exists(workingdir))
                Directory.Delete(workingdir, true);

            // Get the test files
            string configfile = Path.Combine(Directory.GetCurrentDirectory(), jsonfile);
            string campaignFolder = Path.Combine(Directory.GetCurrentDirectory(), campaignFolderPath, "5e");

            // Run the converter
            FGE.FantasyGroundsExporter.Main(new string[] { "-i", campaignFolder, "-c", configfile, "-o", output, "-d", data });

            // Asserts
            Assert.IsTrue(File.Exists(modfile));

            using (var file = File.OpenRead(modfile))
            using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
            {
                Assert.AreEqual(zip.Entries.Count, 6);
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "client.xml"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "definition.xml"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Battlemap.jpg"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Test Map.jpg"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Sample Image/Test Map 2.jpg"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Sample Images/Test Map 3.jpg"));

                using (var stream = zip.Entries.First(e => e.FullName == "client.xml").Open())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string control = File.ReadAllText(Path.Combine(controlFolder, "client.xml"));
                    control = TestHelpers.ReplaceEscapedCharacters(control);
                    string xml = reader.ReadToEnd();
                    Assert.AreEqual(control, xml);
                }

                using (var stream = zip.Entries.First(e => e.FullName == "definition.xml").Open())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string control = File.ReadAllText(Path.Combine(controlFolder, "definition.xml"));
                    control = TestHelpers.ReplaceEscapedCharacters(control);
                    string xml = reader.ReadToEnd();
                    Assert.AreEqual(control, xml);
                }
            }
        }

        [TestMethod]
        public void AnyRulesetReadOnlyPlayerModuleExportAll()
        {
            string modfile = "files/output/5e_AnyRulesetReadOnlyPlayerModule.mod";
            string controlFolder = "files/5e/modules/Any Ruleset, Read Only, Player Module";
            string workingdir = "files/output/5e_AnyRulesetReadOnlyPlayerModule";
            string jsonfile = "files/5e/config/Any Ruleset, Read Only, Player Module.json";

            // Delete file from a previous run of this test
            if (File.Exists(modfile))
                File.Delete(modfile);
            if (Directory.Exists(workingdir))
                Directory.Delete(workingdir, true);

            // Get the test files
            string configfile = Path.Combine(Directory.GetCurrentDirectory(), jsonfile);
            string campaignFolder = Path.Combine(Directory.GetCurrentDirectory(), campaignFolderPath, "5e");

            // Run the converter
            FGE.FantasyGroundsExporter.Main(new string[] { "-i", campaignFolder, "-c", configfile, "-o", output, "-d", data });

            // Asserts
            Assert.IsTrue(File.Exists(modfile));

            using (var file = File.OpenRead(modfile))
            using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
            {
                Assert.AreEqual(zip.Entries.Count, 6);
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "client.xml"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "definition.xml"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Battlemap.jpg"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Test Map.jpg"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Sample Image/Test Map 2.jpg"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Sample Images/Test Map 3.jpg"));

                using (var stream = zip.Entries.First(e => e.FullName == "client.xml").Open())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string control = File.ReadAllText(Path.Combine(controlFolder, "client.xml"));
                    control = TestHelpers.ReplaceEscapedCharacters(control);
                    string xml = reader.ReadToEnd();
                    Assert.AreEqual(control, xml);
                }

                using (var stream = zip.Entries.First(e => e.FullName == "definition.xml").Open())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string control = File.ReadAllText(Path.Combine(controlFolder, "definition.xml"));
                    control = TestHelpers.ReplaceEscapedCharacters(control);
                    string xml = reader.ReadToEnd();
                    Assert.AreEqual(control, xml);
                }
            }
        }

        // Tests various flags when exporting specific content
        [TestMethod]
        public void NoFlagsExportSpecific()
        {
            string modfile = "files/output/5e_ExportSpecific.mod";
            string controlFolder = "files/5e/modules/Export Specific";
            string workingdir = "files/output/5e_ExportSpecific";
            string jsonfile = "files/5e/config/Export Specific.json";
            // Delete file from a previous run of this test
            if (File.Exists(modfile))
                File.Delete(modfile);
            if (Directory.Exists(workingdir))
                Directory.Delete(workingdir, true);

            // Get the test files
            string configfile = Path.Combine(Directory.GetCurrentDirectory(), jsonfile);
            string campaignFolder = Path.Combine(Directory.GetCurrentDirectory(), campaignFolderPath, "5e");

            // Run the converter
            FGE.FantasyGroundsExporter.Main(new string[] { "-i", campaignFolder, "-c", configfile, "-o", output, "-d", data });

            // Asserts
            Assert.IsTrue(File.Exists(modfile));

            using (var file = File.OpenRead(modfile))
            using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
            {
                Assert.AreEqual(zip.Entries.Count, 3);
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "db.xml"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "definition.xml"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Battlemap.jpg"));

                using (var stream = zip.Entries.First(e => e.FullName == "db.xml").Open())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string control = File.ReadAllText(Path.Combine(controlFolder, "db.xml"));
                    control = TestHelpers.ReplaceEscapedCharacters(control);
                    string xml = reader.ReadToEnd();
                    Assert.AreEqual(control, xml);
                }

                using (var stream = zip.Entries.First(e => e.FullName == "definition.xml").Open())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string control = File.ReadAllText(Path.Combine(controlFolder, "definition.xml"));
                    control = TestHelpers.ReplaceEscapedCharacters(control);
                    string xml = reader.ReadToEnd();
                    Assert.AreEqual(control, xml);
                }
            }
        }

        [TestMethod]
        public void ReadOnlyExportSpecific()
        {
            string modfile = "files/output/5e_ReadOnlyExportSpecific.mod";
            string controlFolder = "files/5e/modules/Export Specific, Read Only";
            string workingdir = "files/output/5e_ReadOnlyExportSpecific";
            string jsonfile = "files/5e/config/Export Specific, Read Only.json";
            // Delete file from a previous run of this test
            if (File.Exists(modfile))
                File.Delete(modfile);
            if (Directory.Exists(workingdir))
                Directory.Delete(workingdir, true);

            // Get the test files
            string configfile = Path.Combine(Directory.GetCurrentDirectory(), jsonfile);
            string campaignFolder = Path.Combine(Directory.GetCurrentDirectory(), campaignFolderPath, "5e");

            // Run the converter
            FGE.FantasyGroundsExporter.Main(new string[] { "-i", campaignFolder, "-c", configfile, "-o", output, "-d", data });

            // Asserts
            Assert.IsTrue(File.Exists(modfile));

            using (var file = File.OpenRead(modfile))
            using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
            {
                Assert.AreEqual(zip.Entries.Count, 3);
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "db.xml"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "definition.xml"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Battlemap.jpg"));

                using (var stream = zip.Entries.First(e => e.FullName == "db.xml").Open())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string control = File.ReadAllText(Path.Combine(controlFolder, "db.xml"));
                    control = TestHelpers.ReplaceEscapedCharacters(control);
                    string xml = reader.ReadToEnd();
                    Assert.AreEqual(control, xml);
                }

                using (var stream = zip.Entries.First(e => e.FullName == "definition.xml").Open())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string control = File.ReadAllText(Path.Combine(controlFolder, "definition.xml"));
                    control = TestHelpers.ReplaceEscapedCharacters(control);
                    string xml = reader.ReadToEnd();
                    Assert.AreEqual(control, xml);
                }
            }
        }

        [TestMethod]
        public void AnyRulesetExportSpecific()
        {
            string modfile = "files/output/5e_AnyRulesetExportSpecific.mod";
            string controlFolder = "files/5e/modules/Export Specific, Any Ruleset";
            string workingdir = "files/output/5e_AnyRulesetExportSpecific";
            string jsonfile = "files/5e/config/Export Specific, Any Ruleset.json";
            // Delete file from a previous run of this test
            if (File.Exists(modfile))
                File.Delete(modfile);
            if (Directory.Exists(workingdir))
                Directory.Delete(workingdir, true);

            // Get the test files
            string configfile = Path.Combine(Directory.GetCurrentDirectory(), jsonfile);
            string campaignFolder = Path.Combine(Directory.GetCurrentDirectory(), campaignFolderPath, "5e");

            // Run the converter
            FGE.FantasyGroundsExporter.Main(new string[] { "-i", campaignFolder, "-c", configfile, "-o", output, "-d", data });

            // Asserts
            Assert.IsTrue(File.Exists(modfile));

            using (var file = File.OpenRead(modfile))
            using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
            {
                Assert.AreEqual(zip.Entries.Count, 3);
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "db.xml"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "definition.xml"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Battlemap.jpg"));

                using (var stream = zip.Entries.First(e => e.FullName == "db.xml").Open())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string control = File.ReadAllText(Path.Combine(controlFolder, "db.xml"));
                    control = TestHelpers.ReplaceEscapedCharacters(control);
                    string xml = reader.ReadToEnd();
                    Assert.AreEqual(control, xml);
                }

                using (var stream = zip.Entries.First(e => e.FullName == "definition.xml").Open())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string control = File.ReadAllText(Path.Combine(controlFolder, "definition.xml"));
                    control = TestHelpers.ReplaceEscapedCharacters(control);
                    string xml = reader.ReadToEnd();
                    Assert.AreEqual(control, xml);
                }
            }
        }

        [TestMethod]
        public void PlayerModuleExportSpecific()
        {
            string modfile = "files/output/5e_PlayerModuleExportSpecific.mod";
            string controlFolder = "files/5e/modules/Export Specific, Player Module";
            string workingdir = "files/output/5e_PlayerModuleExportSpecific";
            string jsonfile = "files/5e/config/Export Specific, Player Module.json";

            // Delete file from a previous run of this test
            if (File.Exists(modfile))
                File.Delete(modfile);
            if (Directory.Exists(workingdir))
                Directory.Delete(workingdir, true);

            // Get the test files
            string configfile = Path.Combine(Directory.GetCurrentDirectory(), jsonfile);
            string campaignFolder = Path.Combine(Directory.GetCurrentDirectory(), campaignFolderPath, "5e");

            // Run the converter
            FGE.FantasyGroundsExporter.Main(new string[] { "-i", campaignFolder, "-c", configfile, "-o", output, "-d", data });

            // Asserts
            Assert.IsTrue(File.Exists(modfile));

            using (var file = File.OpenRead(modfile))
            using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
            {
                Assert.AreEqual(zip.Entries.Count, 3);
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "client.xml"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "definition.xml"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Battlemap.jpg"));

                using (var stream = zip.Entries.First(e => e.FullName == "client.xml").Open())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string control = File.ReadAllText(Path.Combine(controlFolder, "client.xml"));
                    control = TestHelpers.ReplaceEscapedCharacters(control);
                    string xml = reader.ReadToEnd();
                    Assert.AreEqual(control, xml);
                }

                using (var stream = zip.Entries.First(e => e.FullName == "definition.xml").Open())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string control = File.ReadAllText(Path.Combine(controlFolder, "definition.xml"));
                    control = TestHelpers.ReplaceEscapedCharacters(control);
                    string xml = reader.ReadToEnd();
                    Assert.AreEqual(control, xml);
                }
            }
        }
    }
}