namespace FGE.Entities.Configuration
{
    public class RecordTypeConfig
    {
        /// <summary>
        /// Name of the record type
        /// </summary>
        public string RecordType { get; set; } = "";

        /// <summary>
        /// The value in the Name element of the library entry.
        /// This is displayed in the module's index in Fantasy Grounds
        /// </summary>
        public string LibraryName { get; set; } = "";

        /// <summary>
        /// Element name where the module's library entry is stored (within library.entries)
        /// Defaults to RecordType
        /// </summary>
        public string LibraryPath { get; set; } = "";

        /// <summary>
        /// Element name where records are found in the campaign DB.xml
        /// Defaults to RecordType
        /// </summary>
        public string DbPath { get; set; } = "";

        /// <summary>
        /// Element name where the records should go in the module DB.xml
        /// Defaults to RecordType
        /// </summary>
        public string ModulePath { get; set; } = "";

        /// <summary>
        /// Element name where the records should go in the module DB.xml for read only modules
        /// Defaults to RecordType
        /// </summary>
        public string ReferencePath { get; set; } = "";

        /// <summary>
        /// Value for the librarylink.class element in the module's library
        /// </summary>
        public string LibrarylinkClass { get; set; } = "reference_list";

        /// <summary>
        /// Value for the librarylink.recordname element in the module's library
        /// </summary>
        public string LibrarylinkRecordName { get; set; } = "..";

        /// <summary>
        /// Boolean flat to include the recordtype element in the library entry for a record
        /// Defaults: true
        /// </summary>
        public bool IncludeLibraryRecordType { get; set; } = true;

        /// <summary>
        /// The data type records of this type use in Fantasy Grounds.
        /// Used in transforming links of this data type
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// List of individual record ids that are exported.
        /// If empty, exports all records
        /// </summary>
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