using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptoSql.MySql.Packet.Decoded
{
    internal abstract class ClientPacket
    {
        protected byte[] Data;
        public readonly uint Length;
        public readonly byte Number;

        protected ClientPacket(IReadOnlyList<byte> data)
        {
            if (data.Count < 4)
                throw new ArgumentException("Packet too short");
            var len = data.Take(3).Append((byte) 0x00).ToArray();
            Length = BitConverter.ToUInt32(len, 0);
            Number = data[3];
            Data = data.Skip(4).ToArray();
            if (Data.Length != Length)
                throw new ArgumentException("Not a well-formed MySQL packet");
        }
    }
}
