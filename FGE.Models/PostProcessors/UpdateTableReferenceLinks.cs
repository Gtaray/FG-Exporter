using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FGE.Models.PostProcessors
{
    // Only run on 5e conversions
    internal class UpdateTableReferenceLinks : IPostProcessor
    {
        public void Process(Converter converter)
        {
            var links = converter.Export
                .Descendants("resultlink")
                .Where(e => e.Element("class") != null && 
                            !string.IsNullOrEmpty(e.Element("class")?.Value) &&
                            e.Element("recordname") != null &&
                            !string.IsNullOrEmpty(e.Element("recordname")?.Value));
            foreach (var link in links)
            {
                string recordname = link.Element("recordname")?.Value;
                // Skip listed exceptions
                if (converter.Config.ReferenceLinkConversionExceptions.Any(e => e.Equals(recordname)))
                    continue;

                var recordclass = link.Element("class").Value;
                var config = converter.Config.RecordTypes.FirstOrDefault(r => r.RecordType == recordclass);

                string replace = "reference." + config.ReferencePath;
                string regex = $"^({recordclass})";
                string newVal = Regex.Replace(link.Element("recordname").Value, regex, replace);
                link.Element("recordname").SetValue(newVal);
            }
        }

        public bool ShouldRun(Converter converter)
        {
            return converter.Config.Ruleset == "5E" && converter.Config.ReadOnly == true;
        }
    }
}
