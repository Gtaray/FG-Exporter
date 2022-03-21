using FGE.Entities.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FGE.Models.PreProcessors
{
    public class GhostWriterPreProcessor : IPreProcessor
    {
        public bool ShouldRun(Converter converter)
        {
            return converter.Config.GhostWriter != GhostWriterConfig.None;
        }

        public void Process(Converter converter)
        {
            GhostWriterConfig gwconfig = converter.Config.GhostWriter;

            var exportnodes = converter.DB
                .Descendants("exportcontrol")
                .Where(e => !GhostWriter.Matches(e.Value, gwconfig));
            List<XElement> toRemove = new List<XElement>();

            foreach (var exportnode in exportnodes)
            {
                toRemove.Add(exportnode.Parent);
            }

            foreach (var node in toRemove)
            {
                // if node is a refmanaul page, we also need to delete it's corresponding index entry
                if (node.Parent.Name == "refmanualdata")
                {
                    var recordname = converter.DB
                        .Descendants("refmanualindex")?
                        .FirstOrDefault()?
                        .Descendants("recordname")?
                        .FirstOrDefault(e => e.Value.Contains("refmanualdata." + node.Name));
                    if (recordname == null)
                        throw new InvalidOperationException("Could not find and remove index entry for reference manual page " + node.Name);

                    // Move up from the recordname entry to the listlink, then to the actual page entry
                    recordname.Parent.Parent.Remove();
                }
                node.Remove();
            }

            // After removing all of the nodes we don't want, we need to go through the
            // reference manual index and remove any empty chapters and subchapter entries
            converter.DB
                .Descendants("refpages")
                .Where(s => s.IsEmpty)
                .Select(s => s.Parent)
                .Remove();

            // After removing all empty subchapters, now we need to remove
            // all empty chapters
            converter.DB
                .Descendants("subchapters")
                .Where(s => s.IsEmpty)
                .Select(s => s.Parent)
                .Remove();
        }
    }
}
