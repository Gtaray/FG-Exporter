using FGE.Entities.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FGE.Models.PostProcessors
{
    public class AddKeywordsToRefPages : IPostProcessor
    {
        public bool ShouldRun(XDocument doc, ExportConfig config)
        {
            // Look for any ref pages that don't have a keyword element
            return (bool)doc
                .Descendants("refmanualindex")?
                .Descendants("refpages")?
                .Elements()
                .Where(e => e.Element("keywords") == null)
                .Any();
        }

        public void Process(XDocument doc, ExportConfig config)
        {
            var refpages = doc
                .Descendants("refmanualindex")?
                .Descendants("refpages")?
                .Elements()
                .Where(e => e.Element("keywords") == null);

            foreach (var refpage in refpages)
            {
                refpage.Add(
                    new XElement(
                        "keywords", 
                        new XAttribute("type", "string")
                    )
                );
            }
        }
    }
}
