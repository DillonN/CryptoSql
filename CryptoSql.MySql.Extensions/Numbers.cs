using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptoSql.MySql.Extensions
{
    public static class Numbers
    {
        /// <summary>
        /// Convert to a length encoded integer MySQL data type
        /// </summary>
        public static IEnumerable<byte> ToLengthEncoded(this ulong x)
        {
            if (x >= (uint) 1 << 24)
            {
                return BitConverter.GetBytes(x).Prepend((byte) 0xfe);
            }

            if (x >= (uint) 1 << 16)
            {
                return BitConverter.GetBytes(x).Take(3).Prepend((byte) 0xfd);
            }

            if (x >= 251)
            {
                return BitConverter.GetBytes(x).Take(2).Prepend((byte) 0xfc);
            }

            return new[] {(byte) x};
        }

        /// <summary>
        /// Convert to a length encoded integer MySQL data type
        /// </summary>
        public static IEnumerable<byte> ToLengthEncoded(this int x)
        {
            if (x < 0) throw new ArgumentException();
            return ((ulong) x).ToLengthEncoded();
        }

        /// <summary>
        /// Convert to a length encoded integer MySQL data type
        /// </summary>
        public static IEnumerable<byte> ToLengthEncoded(this uint x)
        {
            return ((ulong) x).ToLengthEncoded();
        }
    }
}
