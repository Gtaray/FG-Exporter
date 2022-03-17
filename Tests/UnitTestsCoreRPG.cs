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
    public class UnitTestsCoreRPG
    {
        public string campaignFolderPath = "files/corerpg/campaigns";
        public string output = "files/output";

        [TestMethod]
        public void ExportImageLayers()
        {
            string modfile = "files/output/corerpg_Images.mod";
            string controlFolder = "files/corerpg/modules/Images";
            string workingdir = "files/output/corerpg_Images";
            string jsonfile = "files/corerpg/config/Images.json";
            // Delete file from a previous run of this test
            if (File.Exists(modfile))
                File.Delete(modfile);
            if (Directory.Exists(workingdir))
                Directory.Delete(workingdir, true);

            // Get the test files
            string configfile = Path.Combine(Directory.GetCurrentDirectory(), jsonfile);
            string campaignFolder = Path.Combine(Directory.GetCurrentDirectory(), campaignFolderPath, "corerpg");

            // Run the converter
            FGE.FantasyGroundsExporter.Main(new string[] { "-i", campaignFolder, "-c", configfile });

            // Asserts
            Assert.IsTrue(File.Exists(modfile));

            using (var file = File.OpenRead(modfile))
            using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
            {
                Assert.AreEqual(zip.Entries.Count, 20);
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "db.xml"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "definition.xml"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Maps/BuzzardCliff.jpg"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Maps/Forest Clearing (23x16) Day.jpg"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Maps/HalfwayCamp.jpg"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Maps/HighlandPass.jpg"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Assets/3.png"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Assets/4.png"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Assets/9.png"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Assets/10.png"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Assets/17.png"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Assets/18.png"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Assets/Bush 1 - Red.png"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Assets/Bush 2 - Green 1.png"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Assets/Bush 3 - Green 3.png"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Assets/Bush 4 - Green 2.png"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Assets/Bush 5 - Green 1.png"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Assets/Bush 6 - Green 3.png"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Assets/Bush 7 - Green 1.png"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "images/Assets/Bush 8 - Green 1.png"));

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
        public void ExportTokenImages()
        {
            string modfile = "files/output/corerpg_Tokens.mod";
            string controlFolder = "files/corerpg/modules/Tokens";
            string workingdir = "files/output/corerpg_Tokens";
            string jsonfile = "files/corerpg/config/Tokens.json";
            // Delete file from a previous run of this test
            if (File.Exists(modfile))
                File.Delete(modfile);
            if (Directory.Exists(workingdir))
                Directory.Delete(workingdir, true);

            // Get the test files
            string configfile = Path.Combine(Directory.GetCurrentDirectory(), jsonfile);
            string campaignFolder = Path.Combine(Directory.GetCurrentDirectory(), campaignFolderPath, "corerpg");

            // Run the converter
            FGE.FantasyGroundsExporter.Main(new string[] { "-i", campaignFolder, "-c", configfile });

            // Asserts
            Assert.IsTrue(File.Exists(modfile));

            using (var file = File.OpenRead(modfile))
            using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
            {
                Assert.AreEqual(zip.Entries.Count, 4);
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "db.xml"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "definition.xml"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "tokens/Griffin.png"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "tokens/Giant Crocodile.png"));

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
        public void ExportRefMan()
        {
            string modfile = "files/output/corerpg_refman.mod";
            string controlFolder = "files/corerpg/modules/Ref Man";
            string workingdir = "files/output/corerpg_refman";
            string jsonfile = "files/corerpg/config/Ref Man.json";
            // Delete file from a previous run of this test
            if (File.Exists(modfile))
                File.Delete(modfile);
            if (Directory.Exists(workingdir))
                Directory.Delete(workingdir, true);

            // Get the test files
            string configfile = Path.Combine(Directory.GetCurrentDirectory(), jsonfile);
            string campaignFolder = Path.Combine(Directory.GetCurrentDirectory(), campaignFolderPath, "refman");

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
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "referenceimages/Maps/BuzzardCliff.jpg"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "referenceimages/Maps/Forest Clearing (23x16) Day.jpg"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "referenceimages/Maps/Nested Campaign Folder/HalfwayCamp.jpg"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "referenceimages/Maps/Nested Data Folder/HighlandPass.jpg"));

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
        public void ExportRefManReadOnly()
        {
            string modfile = "files/output/corerpg_refman_readonly.mod";
            string controlFolder = "files/corerpg/modules/Ref Man, Read Only";
            string workingdir = "files/output/corerpg_refman_readonly";
            string jsonfile = "files/corerpg/config/Ref Man, Read Only.json";
            // Delete file from a previous run of this test
            if (File.Exists(modfile))
                File.Delete(modfile);
            if (Directory.Exists(workingdir))
                Directory.Delete(workingdir, true);

            // Get the test files
            string configfile = Path.Combine(Directory.GetCurrentDirectory(), jsonfile);
            string campaignFolder = Path.Combine(Directory.GetCurrentDirectory(), campaignFolderPath, "refman");

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
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "referenceimages/Maps/BuzzardCliff.jpg"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "referenceimages/Maps/Forest Clearing (23x16) Day.jpg"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "referenceimages/Maps/Nested Campaign Folder/HalfwayCamp.jpg"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "referenceimages/Maps/Nested Data Folder/HighlandPass.jpg"));

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
        public void ExportRefManPlayerModule()
        {
            string modfile = "files/output/corerpg_refman_playermodule.mod";
            string controlFolder = "files/corerpg/modules/Ref Man, Player Module";
            string workingdir = "files/output/corerpg_refman_playermodule";
            string jsonfile = "files/corerpg/config/Ref Man, Player Module.json";
            // Delete file from a previous run of this test
            if (File.Exists(modfile))
                File.Delete(modfile);
            if (Directory.Exists(workingdir))
                Directory.Delete(workingdir, true);

            // Get the test files
            string configfile = Path.Combine(Directory.GetCurrentDirectory(), jsonfile);
            string campaignFolder = Path.Combine(Directory.GetCurrentDirectory(), campaignFolderPath, "refman");

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
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "referenceimages/Maps/BuzzardCliff.jpg"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "referenceimages/Maps/Forest Clearing (23x16) Day.jpg"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "referenceimages/Maps/Nested Campaign Folder/HalfwayCamp.jpg"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "referenceimages/Maps/Nested Data Folder/HighlandPass.jpg"));

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
        public void ExportRefManReadOnlyPlayerModule()
        {
            string modfile = "files/output/corerpg_refman_readonly_playermodule.mod";
            string controlFolder = "files/corerpg/modules/Ref Man, Read Only, Player Module";
            string workingdir = "files/output/corerpg_refman_readonly_playermodule";
            string jsonfile = "files/corerpg/config/Ref Man, Read Only, Player Module.json";
            // Delete file from a previous run of this test
            if (File.Exists(modfile))
                File.Delete(modfile);
            if (Directory.Exists(workingdir))
                Directory.Delete(workingdir, true);

            // Get the test files
            string configfile = Path.Combine(Directory.GetCurrentDirectory(), jsonfile);
            string campaignFolder = Path.Combine(Directory.GetCurrentDirectory(), campaignFolderPath, "refman");

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
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "referenceimages/Maps/BuzzardCliff.jpg"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "referenceimages/Maps/Forest Clearing (23x16) Day.jpg"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "referenceimages/Maps/Nested Campaign Folder/HalfwayCamp.jpg"));
                Assert.IsTrue(zip.Entries.Any(e => e.FullName == "referenceimages/Maps/Nested Data Folder/HighlandPass.jpg"));

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
