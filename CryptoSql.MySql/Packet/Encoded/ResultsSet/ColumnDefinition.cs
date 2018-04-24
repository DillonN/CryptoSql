using CryptoSql.Common.Models;
using CryptoSql.MySql.Enums;
using CryptoSql.MySql.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptoSql.MySql.Packet.Encoded.ResultsSet {
    internal class ColumnDefinition : ServerPacket
    {
        public const uint ColumnLengthL = 3;
        public const uint TypeLengthL = 1;

        public string Table;
        public string Name;

        public uint ColumnLength = 24;
        public byte Type;

        public byte FlagsDecimalLength = 0x02;
        public byte Flags;
        public byte Decimals = 0x1f;

        public ColumnDefinition(string name, string table = "", DataType type = DataType.VarString)
        {
            Name = name;
            Table = table;
            Type = (byte) type;
        }

        public ColumnDefinition(ColumnInfo info)
            : this(info.Name, info.Table, TypeToDataType(info.Type))
        { }

        protected override IEnumerable<byte> GetBytes()
        {
            var x = new List<byte>();

            x.AddRange(Table.ToLengthEncoded());
            x.AddRange(Name.ToLengthEncoded());
            x.AddRange(ColumnLengthL.ToLengthEncoded());
            x.AddRange(BitConverter.GetBytes(ColumnLength).Take(3));
            x.AddRange(TypeLengthL.ToLengthEncoded());
            x.Add(Type);
            x.Add(FlagsDecimalLength);
            x.Add(Flags);
            x.Add(Decimals);

            return x;
        }

        private static DataType TypeToDataType(Type type)
        {
            if (type == typeof(decimal))
                return DataType.Decimal;
            if (type == typeof(byte))
                return DataType.Tiny;
            if (type == typeof(short))
                return DataType.Short;
            if (type == typeof(int))
                return DataType.Long;
            if (type == typeof(long))
                return DataType.LongLong;
            if (type == typeof(DateTime))
                return DataType.DateTime;
            if (type == typeof(string))
                return DataType.VarString;

            throw new ArgumentException("No DataType found");
        }
    }
}
