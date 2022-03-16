﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

#nullable enable
namespace FGE
{
    public struct RecordKey
    {
        public readonly string RecordType;
        public readonly string Category;
        public readonly string Id;
        public RecordKey(string type, string category, string id)
        {
            RecordType = type;
            Category = category;
            Id = id;
        }

        public bool Equals(RecordKey other)
        {
            return string.Equals(other.RecordType, RecordType) && string.Equals(other.Category, Category);
        }

        public override string ToString()
        {
            return $"{RecordType}.{Category}.{Id}";
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            RecordKey? other = obj as RecordKey?;
            return other != null &&
                string.Equals(other.Value.RecordType, RecordType) &&
                string.Equals(other.Value.Category, Category) &&
                string.Equals(other.Value.Id, Id);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(RecordType, Category, Id);
        }
    }

    public struct RecordValue
    {
        private readonly RecordTypeEntry _config;
        public readonly XElement Record;
        public string RecordType => _config.RecordType;
        public string LibraryLabel => _config.LibraryName;
        public string DbPath => _config.DbPath;
        public string ModulePath => _config.ModulePath;
        public string ReferencePath => _config.ReferencePath;
        public string LibrarylinkClass => _config.LibrarylinkClass;
        public string LibrarylinkRecordName => _config.LibrarylinkRecordName;
        public bool IncludeLibraryRecordType => _config.IncludeLibraryRecordType;

        public RecordValue(XElement record, RecordTypeEntry config)
        {
            this.Record = record;
            this._config = config;
        }
    }
}
#nullable disable
