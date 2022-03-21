using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FGE.Models.PostProcessors
{
    // Should only run on 5e conversions
    internal class UpdateClassFeatureRefernceLinks : IPostProcessor
    {
        public void Process(Converter converter)
        {
            var links = converter.Export
                .Descendants("link")
                .Where(e => e.Attribute("class") != null &&
                            e.Attribute("class")?.Value == "reference_classfeature" &&
                            !string.IsNullOrEmpty(e.Attribute("recordname")?.Value));
            foreach (var link in links)
            {
                // How far to go up to get to the class element
                int count = 0;
                // get the id-XXXX element that bounds the link element
                var record = link.Ancestors()
                    .FirstOrDefault(e => Regex.IsMatch(e.Name.ToString(), @"id-[0-9]+$"));
                if (record == null)
                {
                    continue;
                }
                count++;

                while (record.Parent.Name != "category" && record.Parent.Name != "classdata")
                {
                    // Bump up to the parent of the id-XXXX element
                    record = record.Parent;
                    count++;

                    // Get the next id-XXXX that's up the chain
                    record = record.Ancestors()
                        .FirstOrDefault(e => Regex.IsMatch(e.Name.ToString(), @"id-[0-9]+$"));
                    if (record == null)
                    {
                        break;
                    }
                    count++;
                }

                // build the string
                string replace = "";
                for (int i = 0; i < count; i++)
                    replace += ".";

                string newVal = Regex.Replace(link.Attribute("recordname").Value, @"(class.id-[0-9]+)", replace);
                link.Attribute("recordname").SetValue(newVal);
            }
        }

        public bool ShouldRun(Converter converter)
        {
            return converter.Config.Ruleset == "5E" && converter.Config.ReadOnly == true;
        }
    }
}
