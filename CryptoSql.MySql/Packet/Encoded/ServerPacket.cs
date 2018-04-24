using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CryptoSql.MySql.Packet.Encoded.Interfaces;

namespace CryptoSql.MySql.Packet.Encoded {
    internal abstract class ServerPacket : IServerResponse
    {
        public byte PacketNum;

        public bool IsCommandResponse;

        public byte[] ToByteArray() 
        {
            var list = GetBytes().ToArray();
            var header = list.Length + (PacketNum << (3 * 8));
            return BitConverter.GetBytes(header).Concat(list).ToArray();
        }

        public byte[] ToByteArray(byte num)
        {
            PacketNum = num;
            return ToByteArray();
        }

        protected abstract IEnumerable<byte> GetBytes();

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var b in ToByteArray())
            {
                sb.Append($"{b:X2}");
            }

            return sb.ToString();
        }
    }
}
