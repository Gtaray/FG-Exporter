﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FGE.Models.PostProcessors
{
    // Only run on 5e conversions
    internal class UpdateClassAbilityReferenceLinks : IPostProcessor
    {
        public void Process(Converter converter)
        {
            var links = converter.Export
                .Descendants("link")
                .Where(e => e.Attribute("class") != null &&
                            (e.Attribute("class")?.Value == "reference_classability" ||
                            e.Attribute("class")?.Value == "reference_classfeature" ) && 
                            !string.IsNullOrEmpty(e.Attribute("recordname")?.Value));
            foreach (var link in links)
            {
                string recordname = link.Attribute("recordname")?.Value;
                // Skip listed exceptions
                if (converter.Config.ReferenceLinkConversionExceptions.Any(e => e.Equals(recordname)))
                    continue;

                var config = converter.Config.RecordTypes.FirstOrDefault(r => r.RecordType == "class");
                string replace = "reference." + config.ReferencePath;

                string newVal = Regex.Replace(link.Attribute("recordname").Value, @"^(class)", replace);
                link.Attribute("recordname").SetValue(newVal);
            }
        }

        public bool ShouldRun(Converter converter)
        {
            return converter.Config.Ruleset == "5E" && converter.Config.ReadOnly == true;
        }
    }
}
