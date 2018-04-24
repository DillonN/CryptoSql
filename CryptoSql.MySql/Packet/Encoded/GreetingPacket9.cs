using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoSql.MySql.Packet.Encoded
{
    internal class GreetingPacket9 : ServerPacket
    {        
        public byte Protocol = 0x09;
        public uint ThreadID = 1600;  // Placeholder
        public string AuthData = "";

        public string VersionString = "";

        public GreetingPacket9()
        { }

        public GreetingPacket9(uint threadID, string authData)
        {
            ThreadID = threadID;
            AuthData = authData;
        }

        protected override IEnumerable<byte> GetBytes()
        {
            var x = new List<byte> { Protocol };
            x.AddRange(Encoding.ASCII.GetBytes(VersionString));
            x.Add(0x00);  // Null term
            x.AddRange(BitConverter.GetBytes(ThreadID));
            x.AddRange(Encoding.ASCII.GetBytes(AuthData));
            x.Add(0x00);  // Null term

            return x;
        }
    }
}
