using System.Collections.Generic;

namespace CryptoSql.MySql.Packet.Encoded
{
    internal class EofPacket : ServerPacket
    {
        protected override IEnumerable<byte> GetBytes()
        {
            yield return 0xfe;
        }

        public static byte[] Get(byte num = 0)
        {
            return new EofPacket().ToByteArray(num);
        }
    }
}
