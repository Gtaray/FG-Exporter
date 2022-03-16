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
        [TestMethod]
        public void NoFlagsExportAll()
        {
            string modfile = "files/output/5e_ExportAll.mod";
            string controlFolder = "files/5e/modules/Export All";
            // Delete file from a previous run of this test
            if (File.Exists(modfile))
                File.Delete(modfile);

            // Get the test files
            string configfile = Path.Combine(Directory.GetCurrentDirectory(), "files/5e/config/Export All.json");
            string campaignFolder = Path.Combine(Directory.GetCurrentDirectory(), "files/5e/campaigns/FGE");

            // Run the converter
            FGE.FantasyGroundsExporter.Main(new string[] { "-i", campaignFolder, "-c", configfile });

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
            // Delete file from a previous run of this test
            if (File.Exists(modfile))
                File.Delete(modfile);

            // Get the test files
            string configfile = Path.Combine(Directory.GetCurrentDirectory(), "files/5e/config/Thumbnail.json");
            string campaignFolder = Path.Combine(Directory.GetCurrentDirectory(), "files/5e/campaigns/FGE");

            // Run the converter
            FGE.FantasyGroundsExporter.Main(new string[] { "-i", campaignFolder, "-c", configfile });

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
            // Delete file from a previous run of this test
            if (File.Exists(modfile))
                File.Delete(modfile);

            // Get the test files
            string configfile = Path.Combine(Directory.GetCurrentDirectory(), "files/5e/config/Read Only.json");
            string campaignFolder = Path.Combine(Directory.GetCurrentDirectory(), "files/5e/campaigns/FGE");

            // Run the converter
            FGE.FantasyGroundsExporter.Main(new string[] { "-i", campaignFolder, "-c", configfile });

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
            // Delete file from a previous run of this test
            if (File.Exists(modfile))
                File.Delete(modfile);

            // Get the test files
            string configfile = Path.Combine(Directory.GetCurrentDirectory(), "files/5e/config/Player Module.json");
            string campaignFolder = Path.Combine(Directory.GetCurrentDirectory(), "files/5e/campaigns/FGE");

            // Run the converter
            FGE.FantasyGroundsExporter.Main(new string[] { "-i", campaignFolder, "-c", configfile });

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
            // Delete file from a previous run of this test
            if (File.Exists(modfile))
                File.Delete(modfile);

            // Get the test files
            string configfile = Path.Combine(Directory.GetCurrentDirectory(), "files/5e/config/Any Ruleset.json");
            string campaignFolder = Path.Combine(Directory.GetCurrentDirectory(), "files/5e/campaigns/FGE");

            // Run the converter
            FGE.FantasyGroundsExporter.Main(new string[] { "-i", campaignFolder, "-c", configfile });

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
    }
}