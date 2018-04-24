using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CryptoSql.Common.Enums;

namespace CryptoSql.MySql.Packet.Decoded.CommandPackets {
    internal class CommandPacket : ClientPacket
    {
        public readonly CommandType CommandType;

        public readonly string Payload;

        public readonly string Database;

        public CommandPacket(IReadOnlyList<byte> data, string database = null) : base(data)
        {
            if (Data.Length < 2)
                throw new ArgumentException("Packet too short");

            CommandType = (CommandType) Data[0];

            Payload = Encoding.ASCII.GetString(Data.Skip(1).ToArray());

            Database = database;

            if (Payload.Length != Length - 1)
                throw new ArgumentException("Not a well-formed command packet");
        }
    }
}
