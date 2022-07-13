using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace FGE.Entities.Configuration
{
    public class ExportConfig
    {
        public string OutputFolder { get; set; } = "";
        public string FileName { get; set; } = "";
        public string Name { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string Category { get; set; } = "";
        public string Author { get; set; } = "";
        public bool ReadOnly { get; set; } = false;
        public bool PlayerModule { get; set; } = false;
        public bool AnyRuleset { get; set; } = false;
        public string Ruleset { get; set; } = "";
        [JsonConverter(typeof(StringEnumConverter))]
        public GhostWriterConfig GhostWriter { get; set; } = GhostWriterConfig.None;
        public bool CreateSpellLists { get; set; } = false;
        public List<RecordTypeConfig> RecordTypes { get; set; } = new List<RecordTypeConfig>();
        public List<string> Tokens { get; set; } = new List<string>();
    }
}
