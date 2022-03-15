using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Unicode;

namespace Tests
{
    [TestClass]
    public class EndToEndTests
    {
        [TestMethod]
        public void NoFlagsExportAll()
        {
            string modfile = "files/output/ExportAll.mod";
            string controlFolder = "files/modules/Export All";
            // Delete file from a previous run of this test
            if (File.Exists(modfile))
                File.Delete(modfile);

            // Get the test files
            string configfile = Path.Combine(Directory.GetCurrentDirectory(), "files/config/Export All.json");
            string campaignFolder = Path.Combine(Directory.GetCurrentDirectory(), "files/campaigns/FGE");

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