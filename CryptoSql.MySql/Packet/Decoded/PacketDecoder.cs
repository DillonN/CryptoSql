using System;
using CryptoSql.MySql.Packet.Decoded.CommandPackets;

namespace CryptoSql.MySql.Packet.Decoded
{
    internal static class PacketDecoder
    {
        public static ClientPacket DecodePacket(byte[] data, bool handshake = false)
        {
            if (data.Length < 5)
                throw new ArgumentException("Packet is too short!");

            if (handshake)
            {
                return new LoginPacket(data);
            }

            return new CommandPacket(data);
        }
    }
}
