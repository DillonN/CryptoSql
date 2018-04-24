using System.Data;

namespace CryptoSql.Blockchain.Ethereum.Tests
{
    internal static class DataSetTestContent
    {
        public const ulong Gas = 21000 + 4 * 11 + 68 * 598;
        public const ulong UncompressedGas = 21000 + 4 * 49 + 68 * 1274;

        public static readonly DataSet DS;
        public static readonly DataSet FalseDS = new DataSet();

        static DataSetTestContent()
        {
            DS = new DataSet();
            var table = new DataTable("blarg");
            table.Columns.Add(new DataColumn("id"));
            table.Columns.Add(new DataColumn("num"));
            table.LoadDataRow(new object[] {"honk", 1}, false);
            DS.Tables.Add(table);
            DS.AcceptChanges();
        }
    }
}
