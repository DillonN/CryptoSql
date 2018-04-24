using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CryptoSql.MySql.Extensions {
    public static class String
    {
        /// <summary>
        /// Convert to length encoded string MySQL data type
        /// </summary>
        /// <remarks>
        /// MySQL defines a length encoded string for use in TCP packets.
        /// </remarks>
        public static IEnumerable<byte> ToLengthEncoded(this string s)
        {
            var len = s.Length.ToLengthEncoded();
            return len.Concat(Encoding.ASCII.GetBytes(s));
        }
    }
}
