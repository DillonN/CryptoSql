using System;

namespace CryptoSql.Common.Models
{
    public struct ColumnInfo
    {
        public readonly string Name;
        public readonly string Table;
        public readonly Type Type;

        public ColumnInfo(string name, string table, Type type)
        {
            Name = name;
            Table = table;
            Type = type;
        }
    }
}
