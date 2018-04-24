using System.Collections.Generic;
using CryptoSql.MySql.Extensions;

namespace CryptoSql.MySql.Packet.Encoded.ResultsSet
{
    internal class ColumnCount : ServerPacket
    {
        public ulong Count;

        public ColumnCount(ulong count, byte num = 0)
        {
            Count = count;
            PacketNum = num;
        }

        protected override IEnumerable<byte> GetBytes()
        {
            return Count.ToLengthEncoded();
        }
    }
}
