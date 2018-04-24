using System;

namespace CryptoSql.Common.Extensions 
{
    public static class String
    {
        public static byte[] ToByteArray(this string hex)
        {
            var numberChars = hex.Length;
            if (numberChars % 2 != 0)
                throw new ArgumentException("String must have even length");

            var start = 0;
            if (hex.StartsWith("0x"))
            {
                // Remove prefix
                numberChars -= 2;
                start += 2;
            }
            var bytes = new byte[numberChars / 2];
            for (var i = start; i < numberChars; i += 2)
                bytes[(i - start) / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static string Substring(this string str, string start, string end, bool last = true, bool first = true, bool lowered = false)
        {
            var ind = lowered ? str.ToLower() : str;
            var defStartIndex = first ? ind.IndexOf(start) : ind.LastIndexOf(start) + start.Length - 1;
            var defEndIndex = last ? ind.LastIndexOf(end) : ind.IndexOf(end);
            return str.Substring(defStartIndex + 1, defEndIndex - defStartIndex - (string.IsNullOrEmpty(end) ? 0 : 1));
        }

        public static object ParseToType(this string str, Type type)
        {
            if (type == typeof(string))
            {
                return str.Substring("'", "'");
            }
            if (type == typeof(int) || type == typeof(short) || type == typeof(byte) || type == typeof(long))
            {
                return long.Parse(str);
            }

            if (type == typeof(DateTime))
            {
                return DateTime.Parse(str);
            }

            throw new FormatException($"Cannot parse type {type}");
        } 
    }
}
