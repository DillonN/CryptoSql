using System.Collections.Generic;
using System.Text;
using CryptoSql.MySql.Extensions;

namespace CryptoSql.MySql.Packet.Encoded {
    internal class OKPacket : ServerPacket
    {
        private ulong AffectedRows;
        private ulong LastInsertID;

        public string Status;

        public OKPacket(string status = "", ulong rows = 0, ulong insert = 0, byte number = 1)
        {
            AffectedRows = rows;
            LastInsertID = insert;
            Status = status;
            PacketNum = number;
            IsCommandResponse = true;
        }

        protected override IEnumerable<byte> GetBytes()
        {
            var x = new List<byte> {0x00};
            x.AddRange(AffectedRows.ToLengthEncoded());
            x.AddRange(LastInsertID.ToLengthEncoded());
            x.AddRange(Encoding.ASCII.GetBytes(Status));

            return x;
        }
    }
}
