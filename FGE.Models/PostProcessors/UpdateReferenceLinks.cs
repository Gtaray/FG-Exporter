using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FGE.Models.PostProcessors
{
    public class UpdateReferenceLinks : IPostProcessor
    {
        public void Process(Converter converter)
        {
            // First, replace link elements that have class and recordname children
            var links = converter.Export
                .Descendants("link")
                .Where(e => e.Element("class") != null && e.Element("recordname") != null);
            foreach (var link in links)
            {
                string recordclass = link.Element("class").Value;
                string recordname = link.Element("recordname").Value;
                if (string.IsNullOrEmpty(recordclass) || string.IsNullOrEmpty(recordname))
                    continue;
                // Skip any references to other modules
                if (recordname.Contains("@"))
                    continue;

                // throw exception if there's no matching recordtype
                var config = converter.Config.RecordTypes
                    .First(r => {
                        string path = string.IsNullOrEmpty(r.DbPath) 
                            ? r.RecordType
                            : r.DbPath;
                        return recordclass == path;
                });
                string id = Regex.Match(recordname, @"(id-[0-9]+)$").Value;

                string newPath = $"reference.{config.ReferencePath}.{id}";
                link.Element("recordname").SetValue(newPath);
            }

            // Second, replace link elements that have class and recordname attributes
            // This is used in linklists (and probably other places
            links = converter.Export
                .Descendants("link")
                .Where(e => e.Attribute("class") != null && e.Attribute("recordname") != null);
            foreach (var link in links)
            {
                string recordclass = link.Attribute("class").Value;
                string recordname = link.Attribute("recordname").Value;
                if (string.IsNullOrEmpty(recordclass) || string.IsNullOrEmpty(recordname))
                    continue;
                // Skip any references to other modules
                if (recordname.Contains("@"))
                    continue;

                // throw exception if there's no matching recordtype
                var config = converter.Config.RecordTypes
                    .First(r => {
                        string path = string.IsNullOrEmpty(r.DbPath)
                            ? r.RecordType
                            : r.DbPath;
                        return recordclass == path;
                    });
                string id = Regex.Match(recordname, @"(id-[0-9]+)$").Value;

                string newPath = $"reference.{config.ReferencePath}.{id}";
                link.Attribute("recordname").SetValue(newPath);
            }

            // Third, replace imagelinks
            // Don't need to replace image links, image links don't go in the
            // reference section
            //links = converter.Export
            //    .Descendants("imagelink")
            //    .Where(e => e.Element("class") != null && e.Element("recordname") != null);
            //foreach (var link in links)
            //{
            //    string recordclass = link.Element("class").Value;
            //    string recordname = link.Element("recordname").Value;
            //    if (string.IsNullOrEmpty(recordclass) || string.IsNullOrEmpty(recordname))
            //        continue;

            //    // throw exception if there's no matching recordtype
            //    var config = converter.Config.RecordTypes
            //        .First(r => r.RecordType == "image" );
            //    string id = Regex.Match(recordname, @"(id-[0-9]+)$").Value;

            //    string newPath = $"reference.{config.ReferencePath}.{id}";
            //    link.Element("recordname").SetValue(newPath);
            //}
        }

        // This processor should only run if exporting a 
        // read only module
        public bool ShouldRun(Converter converter)
        {
            return converter.Config.ReadOnly;
        }
    }
}
