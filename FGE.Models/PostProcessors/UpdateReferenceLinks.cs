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
                // Skip listed exceptions
                if (converter.Config.ReferenceLinkConversionOverrides.Any(e => e.Before.Equals(recordname)))
                    continue;

                // Check dbpath first, then if that's null, check recordtype for a configuration
                var config = converter.Config.RecordTypes
                    .FirstOrDefault(r => recordclass == r.DbPath);
                if (config == null)
                    config = converter.Config.RecordTypes
                        .FirstOrDefault(r => recordclass == r.RecordType);
                // No configuration here isn't necessarily a problem
                // since there's things like imagewindows and urls that don't need transforming
                if (config == null)
                    continue;
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
                // Skip listed exceptions
                if (converter.Config.ReferenceLinkConversionOverrides.Any(e => e.Before.Equals(recordname)))
                    continue;

                // Check data type first, then dbpath, then if that's null, check recordtype for a configuration
                var config = converter.Config.RecordTypes
                    .FirstOrDefault(r => recordclass == r.DbPath);
                if (config == null)
                    config = converter.Config.RecordTypes
                        .FirstOrDefault(r => recordclass == r.DataType);
                if (config == null)
                    config = converter.Config.RecordTypes
                        .FirstOrDefault(r => recordclass == r.RecordType);

                // No configuration here isn't necessarily a problem
                // since there's things like imagewindows and urls that don't need transforming
                if (config == null)
                    continue;
                string id = Regex.Match(recordname, @"(id-[0-9]+)$").Value;

                string newPath = $"reference.{config.ReferencePath}.{id}";
                link.Attribute("recordname").SetValue(newPath);
            }
        }

        // This processor should only run if exporting a 
        // read only module
        public bool ShouldRun(Converter converter)
        {
            return converter.Config.ReadOnly;
        }
    }
}
