using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CryptoSql.MySql.Enums.BitFlags;

namespace CryptoSql.MySql.Packet.Decoded
{
    internal class LoginPacket : ClientPacket
    {
        public uint MaxPacket;
        public string User;
        public string Database;
        public Capabilities Capabilities;

        public LoginPacket(IReadOnlyList<byte> data) : base(data)
        {
            Capabilities = Capabilities.None;
            Capabilities |= (Capabilities) (Data[0] | (Data[1] << 8));
            var max = Data.Take(5).Append((byte) 0x00).ToArray();
            MaxPacket = BitConverter.ToUInt32(max, 2);
            var user = Data.Skip(5).TakeWhile(b => b != 0x00).ToArray();
            var db = Data.Skip(5 + user.Length).TakeWhile(b => b != 0x00).ToArray();
            User = Encoding.ASCII.GetString(user);
            Database = Encoding.ASCII.GetString(db);
        }
    }
}
