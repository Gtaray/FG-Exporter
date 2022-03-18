using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class UnitTestsGhostWriter
    {
        public string campaignFolderPath = "files/corerpg/campaigns";
        public string output = "files/output";

        [TestMethod]
        public void ExportGmModule()
        {
            string modfile = "files/output/ghostwriter_gm.mod";
            string controlFolder = "files/corerpg/modules/Ghost Writer, GM";
            string workingdir = "files/output/ghostwriter_gm";
            string jsonfile = "files/corerpg/config/Ghost Writer, GM.json";
            // Delete file from a previous run of this test
            if (File.Exists(modfile))
                File.Delete(modfile);
            if (Directory.Exists(workingdir))
                Directory.Delete(workingdir, true);

            // Get the test files
            string configfile = Path.Combine(Directory.GetCurrentDirectory(), jsonfile);
            string campaignFolder = Path.Combine(Directory.GetCurrentDirectory(), campaignFolderPath, "ghostwriter");

            // Run the converter
            FGE.FantasyGroundsExporter.Main(new string[] { "-i", campaignFolder, "-c", configfile, "-o", output });

            // Asserts
            Assert.IsTrue(File.Exists(modfile));

            using (var file = File.OpenRead(modfile))
            using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
            {
                Assert.AreEqual(zip.Entries.Count, 4);
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "db.xml"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "definition.xml"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Maps/BuzzardCliff.jpg"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Maps/Nested Data Folder/HighlandPass.jpg"));

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
        public void ExportPlayerModule()
        {
            string modfile = "files/output/ghostwriter_player.mod";
            string controlFolder = "files/corerpg/modules/Ghost Writer, Player";
            string workingdir = "files/output/ghostwriter_player";
            string jsonfile = "files/corerpg/config/Ghost Writer, Player.json";
            // Delete file from a previous run of this test
            if (File.Exists(modfile))
                File.Delete(modfile);
            if (Directory.Exists(workingdir))
                Directory.Delete(workingdir, true);

            // Get the test files
            string configfile = Path.Combine(Directory.GetCurrentDirectory(), jsonfile);
            string campaignFolder = Path.Combine(Directory.GetCurrentDirectory(), campaignFolderPath, "ghostwriter");

            // Run the converter
            FGE.FantasyGroundsExporter.Main(new string[] { "-i", campaignFolder, "-c", configfile, "-o", output });

            // Asserts
            Assert.IsTrue(File.Exists(modfile));

            using (var file = File.OpenRead(modfile))
            using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
            {
                Assert.AreEqual(zip.Entries.Count, 3);
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "client.xml"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "definition.xml"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Maps/Nested Data Folder/HighlandPass.jpg"));

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
