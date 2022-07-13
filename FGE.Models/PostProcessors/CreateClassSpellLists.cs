using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FGE.Models.PostProcessors
{
    internal class CreateClassSpellLists : IPostProcessor
    {
        public void Process(Converter converter)
        {
            var sources = converter.DB
                .Element("spell")
                .Descendants("source")
                .SelectMany(e => e.Value.Split(','))
                .Select(c => c.Trim())
                .Distinct()
                .OrderBy(s => s);

            XElement index = new XElement("index");
            foreach (string source in sources) 
            {
                string lower = source.ToLower();
                var s = new XElement(
                    lower, 
                    new XElement(
                        "listlink", 
                        new XAttribute("type", "windowreference"),
                        new XElement("class", "class_spell_view"),
                        new XElement("recordname", string.Format("class_spell_view.{0}@", lower))
                    ),
                    new XElement(
                        "name", 
                        new XAttribute("type", "string"),
                        source
                    )
                );
                index.Add(s);
            }

            XElement list = new XElement(
                "list",
                new XAttribute("static", true),
                new XElement("class_spell_views",
                    new XElement(
                        "name",
                        new XAttribute("type", "string"),
                        "Class Spell Lists"
                    ),
                    index
                )
            );

            converter.Export.Add(list);

            //  Now add the entry to the library node
            var entries = converter.Export.Element("library")?.Descendants("entries").FirstOrDefault();
            if (entries != null)
            {
                entries.Add(
                    new XElement(
                        "class_spell_views",
                        new XElement(
                            "librarylink",
                            new XAttribute("type", "windowreference"),
                            new XElement("class", "referenceindex"),
                            new XElement("recordname", "list.class_spell_views")
                        ),
                        new XElement(
                            "name",
                            new XAttribute("type", "string"),
                            "Class Spell Lists"
                        )
                    )
                );
            }
        }

        public bool ShouldRun(Converter converter)
        {
            return converter.Config.Ruleset == "5E"
                && converter.Config.CreateSpellLists == true
                && converter.DB.Element("spell") != null;
        }
    }
}
