using System.Collections.Generic;
using CryptoSql.MySql.Extensions;

namespace CryptoSql.MySql.Packet.Encoded.ResultsSet
{
    internal class ResultsRow : ServerPacket
    {
        public IEnumerable<object> Catalog;

        public ResultsRow(IEnumerable<object> catalog)
        {
            Catalog = catalog;
        }

        protected override IEnumerable<byte> GetBytes()
        {
            var x = new List<byte>();
            foreach (var item in Catalog)
            {
                if (item == null)
                {
                    x.Add(0xfb);
                }
                else
                {
                    x.AddRange(item?.ToString().ToLengthEncoded());
                }
            }

            return x;
        }
    }
}
