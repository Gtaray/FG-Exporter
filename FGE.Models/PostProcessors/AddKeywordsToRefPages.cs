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
        public bool ShouldRun(Converter converter)
        {
            // Look for any ref pages that don't have a keyword element
            return (bool)converter.Export
                .Descendants("refmanualindex")?
                .Descendants("refpages")?
                .Elements()
                .Where(e => e.Element("keywords") == null)
                .Any();
        }

        public void Process(Converter converter)
        {
            var refpages = converter.Export
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
