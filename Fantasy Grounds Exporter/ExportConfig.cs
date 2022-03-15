using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FGE
{
    public class ExportConfig
    {
        public string OutputFolder { get; set; } = "";
        public string FileName { get; set; } = "";
        public string Thumbnail { get; set; } = "";
        public string Name { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string Category { get; set; } = "";
        public string Author { get; set; } = "";
        public bool ReadOnly { get; set; } = false;
        public bool PlayerModule { get; set; } = false;
        public bool AnyRuleset { get; set; } = false;
        public string Ruleset { get; set; } = "";
        public string DataImagesFolder { get; set; } = "";
        public List<RecordTypeEntry> RecordTypes { get; set; } = new List<RecordTypeEntry>();
    }
}
