using CryptoSql.MySql.Enums.BitFlags;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoSql.MySql.Packet.Encoded {
    internal class GreetingPacket10 : ServerPacket
    {
        public byte Protocol = 0x0a;
        public uint ThreadID = 1600;  // Placeholder
        public ulong AuthData = 0x1111111111111111;
        public Capabilities CapabilityFlags;
        public byte CharacterSet = 0x00;
        public ushort StatusFlags = 0x0000;
        public ServerCapabilitiesExt CapabilityFlagsExt;
        public byte Auth = 0x00;

        public string VersionString = "";

        public GreetingPacket10()
        { }

        public GreetingPacket10(uint threadID, string version = "CryptoSql", byte charSet = 0x08, ushort statusFlags = 0x02)
        {
            ThreadID = threadID ;
            CharacterSet = charSet;
            StatusFlags = statusFlags;
            VersionString = version;
        }

        protected override IEnumerable<byte> GetBytes()
        {
            var x = new List<byte> { Protocol };
            x.AddRange(Encoding.ASCII.GetBytes(VersionString));
            x.Add(0x00);  // Null term
            x.AddRange(BitConverter.GetBytes(ThreadID));
            x.AddRange(BitConverter.GetBytes(AuthData));
            x.Add(0x00);  // Filler
            x.AddRange(BitConverter.GetBytes((ushort) CapabilityFlags));
            x.Add(CharacterSet);
            x.AddRange(BitConverter.GetBytes(StatusFlags));
            x.AddRange(BitConverter.GetBytes((ushort) CapabilityFlagsExt));
            x.Add(Auth);
            // 10 reserved bits
            for (var i = 0; i < 10; i++) {
                x.Add(0x00);
            }

            return x;
        }
    }
}
