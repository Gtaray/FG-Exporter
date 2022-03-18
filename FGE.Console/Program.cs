using CommandLine;
using FGE.Entities.Configuration;
using FGE.Models;
using Newtonsoft.Json;

namespace FGE {
    public class FantasyGroundsExporter
    {
        public class Options
        {
            [Option('i', "input", Required = true, HelpText = "Path to the campaign folder that includes the db.xml to export.")]
            public string CampaignFolder { get; set; } = "";
            [Option('o', "output", Required = false, HelpText = "Path to where the converted module is placed. Defaults to location of fge.exe")]
            public string OutputFolder { get; set; } = Directory.GetCurrentDirectory();
            [Option('t', "thumbnail", Required = false, HelpText = "Path to the thumbnail to use. Default is no thumbnail used")]
            public string Thumbnail { get; set; } = "";
            [Option('d', "data", Required = false, HelpText = "Path to the Fantasy Ground data folder.")]
            public string DataFolder { get; set; } = "";
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
                          throw new ArgumentException($"Campaign folder not found. {o.CampaignFolder}");
                      }
                      string dbfile = Path.Combine(o.CampaignFolder, "db.xml");
                      if (!File.Exists(dbfile))
                      {
                          throw new ArgumentException("Campaign db.xml file not found.");
                      }

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

                      var Converter = new Converter(config, o.CampaignFolder, o.OutputFolder, o.DataFolder, o.Thumbnail);

                      Converter.Export();
                  });
        }       
    }
}