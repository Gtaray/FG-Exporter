namespace FGE
{
    public class RecordTypeEntry
    {
        public string RecordType { get; set; } = "";
        public string LibraryName { get; set; } = "";
        public string DbPath { get; set; } = "";
        public List<string> Records { get; set; } = new List<string>();

        public override string ToString()
        {
            if (Records.Count() == 0)
                return $"{RecordType} (all)";
            else
                return $"{RecordType} ({Records.Count()})";

        }
    }
}