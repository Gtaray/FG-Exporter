using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FGE.Models.PostProcessors
{
    internal class UpdateRacialTraitReferenceLinks : IPostProcessor
    {
        public void Process(Converter converter)
        {
            var links = converter.Export
                .Descendants("link")
                .Where(e => e.Attribute("class") != null &&
                            e.Attribute("class")?.Value == "reference_racialtrait" &&
                            !string.IsNullOrEmpty(e.Attribute("recordname")?.Value));
            foreach (var link in links)
            {
                string recordname = link.Attribute("recordname")?.Value;
                // Skip listed exceptions
                if (converter.Config.ReferenceLinkConversionExceptions.Any(e => e.Equals(recordname)))
                    continue;

                var config = converter.Config.RecordTypes.FirstOrDefault(r => r.RecordType == "race");
                if (config != null)
                {
                    string replace = "reference." + config.ReferencePath;

                    string newVal = Regex.Replace(link.Attribute("recordname").Value, @"^(race)", replace);
                    link.Attribute("recordname").SetValue(newVal);
                }
            }
        }

        public bool ShouldRun(Converter converter)
        {
            return converter.Config.Ruleset == "5E" && converter.Config.ReadOnly == true;
        }
    }
}
