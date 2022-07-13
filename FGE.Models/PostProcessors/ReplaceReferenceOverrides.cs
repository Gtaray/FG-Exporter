using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FGE.Models.PostProcessors
{
    public class ReplaceReferenceOverrides : IPostProcessor
    {
        public void Process(Converter converter)
        {
            // Do all the overrides now
            foreach (var linkOverride in converter.Config.ReferenceLinkConversionOverrides)
            {
                var overrides = converter.Export
                    .Descendants("link")
                    .Where(e => e.Attribute("recordname")?.Value == linkOverride.Before);
                foreach (var reflink in overrides)
                {
                    reflink.Attribute("recordname").Value = linkOverride.After;
                }

                overrides = converter.Export
                    .Descendants("link")
                    .Where(e => e.Element("recordname")?.Value == linkOverride.Before);
                foreach (var reflink in overrides)
                {
                    reflink.Element("recordname").Value = linkOverride.After;
                }
            }
        }

        public bool ShouldRun(Converter converter)
        {
            return converter.Config.ReadOnly && converter.Config.ReferenceLinkConversionOverrides.Count() > 0;
        }
    }
}
