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
        public bool ShouldRun(XElement campaignDb, ExportConfig config)
        {
            return config.GhostWriter != GhostWriterConfig.None;
        }

        public void Process(XElement campaignDb, ExportConfig config)
        {
            GhostWriterConfig gwconfig = config.GhostWriter;

            var exportnodes = campaignDb
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
                    var recordname = campaignDb
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
            campaignDb
                .Descendants("refpages")
                .Where(s => s.IsEmpty)
                .Select(s => s.Parent)
                .Remove();

            // After removing all empty subchapters, now we need to remove
            // all empty chapters
            campaignDb
                .Descendants("subchapters")
                .Where(s => s.IsEmpty)
                .Select(s => s.Parent)
                .Remove();
        }
    }
}
