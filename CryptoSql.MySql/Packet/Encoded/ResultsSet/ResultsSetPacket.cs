using CryptoSql.Common.Models;
using CryptoSql.MySql.Packet.Encoded.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace CryptoSql.MySql.Packet.Encoded.ResultsSet {
    public class ResultsSetPacket : IServerResponse
    {
        private readonly IEnumerable<ColumnDefinition> _columns;
        private readonly List<ResultsRow> _rows = new List<ResultsRow>();

        public ResultsSetPacket(Dictionary<ColumnInfo, object[]> results)
        {
            _columns = results.Keys.Select(c => new ColumnDefinition(c));

            for (var i = 0; i < results.First().Value.Length; i++)
            {
                var vals = new List<object>();
                foreach (var key in results.Keys)
                {
                    vals.Add(results[key][i]);
                }
                _rows.Add(new ResultsRow(vals));
            }
        }

        public byte[] ToByteArray()
        {
            var x = new List<byte>();
            byte i = 1;

            x.AddRange(new ColumnCount((ulong) _columns.Count()).ToByteArray(i++));
            foreach (var column in _columns)
            {
                x.AddRange(column.ToByteArray(i++));
            }
            x.AddRange(EofPacket.Get(i++));
            foreach (var row in _rows)
            {
                x.AddRange(row.ToByteArray(i++));
            }
            x.AddRange(EofPacket.Get(i));

            return x.ToArray();
        }
    }
}
