using CryptoSql.Common.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CryptoSql.Blockchain.Ethereum.Models;
using CryptoSql.Common.Extensions;

namespace CryptoSql.Blockchain.Ethereum.StorageProviders.Transaction.Monolithic
{
    public class EthMonolithicProvider : StorageProvider
    {
        public DataSet DataSet { get; private set; }

        public EthMonolithicProvider(string serverAddress, string keyStore, string password, DataSet dataSet = null) 
            : base(serverAddress, keyStore, password)
        {
            DataSet = dataSet ?? new DataSet();
        }

        public override async Task<bool> UseAddress(string address)
        {
            if (address == null)
            {
                // No table has been published yet
                DataSet = new DataSet();
                return true;
            }
            var data = await EthCon.GetTransactionData(address);
            if (data == null)
                return false;

            DataSet = DataSetConverter.FromBytes(data);

            return true;
        }

        public override async Task Reset()
        {
            DataSet = new DataSet();
        }

        public override Task<TransactionInfo> CommitAsync()
        {
            DataSet.AcceptChanges();
            var bytes = DataSetConverter.ToBytes(DataSet);
            Console.WriteLine($"Commiting {bytes.Length} bytes to the network");
            var gasToUse = EthereumConnection.EstimateGasCost(bytes) + 5000;
            return EthCon.PostNewTransaction(bytes, GasPrice);
        }

        public override CostInfo GetEstimatedCommitCostAsync()
        {
            DataSet.AcceptChanges();
            var bytes = DataSetConverter.ToBytes(DataSet);
            var gas = EthereumConnection.EstimateGasCost(bytes);

            var cost = new EthCostInfo(gas, GasPrice);

            return cost;
        }

        public override void DropTableAsync(string tableName)
        {
            DataSet.Tables.Remove(tableName);
        }

        public override bool NewTableAsync(string name, IEnumerable<ColumnInfo> info)
        {
            if (DataSet.Tables.Contains(name))
                return false;

            var table = new DataTable(name);

            foreach (var col in info)
            {
                var column = new DataColumn(col.Name)
                {
                    DataType = col.Type
                };
                table.Columns.Add(column);
            }

            DataSet.Tables.Add(table);

            return true;
        }

        public override bool InsertRow(string tableName, IReadOnlyDictionary<string, string> values)
        {
            return UpdateRowAsync(tableName, values);
        }

        public override bool UpdateRowAsync(string tableName, IReadOnlyDictionary<string, string> values)
        {
            CheckNameAndCount(tableName, values.Count);
            DataSet.Tables[tableName].LoadDataRow(values.Values.ToArray(), false);

            return true;
        }

        public override bool UpdateRowsAsync(string tableName, IEnumerable<IReadOnlyDictionary<string, string>> values)
        {
            foreach (var row in values)
            {
                DataSet.Tables[tableName].LoadDataRow(row.Values.ToArray(), false);
            }

            return false;
        }

        public override void DropRowsAsync(string tableName, IEnumerable<uint> indices)
        {
            CheckName(tableName);

            if (indices == null)
            {
                DataSet.Tables[tableName].Rows.Clear();
                return;
            }

            foreach (var index in indices)
            {
                DataSet.Tables[tableName].Rows.RemoveAt((int) index);
            }
        }

        public override Dictionary<ColumnInfo, object[]> SelectRowsAsync(
            string tableName,
            IReadOnlyDictionary<string, string> columns)
        {
            CheckName(tableName);

            var results = new Dictionary<ColumnInfo, object[]>();

            foreach (var col in columns.Keys)
            {
                var list = new List<object>();
                var type = DataSet.Tables[tableName].Columns[col].DataType;
                var colInfo = new ColumnInfo(col, tableName, type);

                foreach (DataRow row in DataSet.Tables[tableName].Rows)
                {
                    if (columns[col] == null || row[col] == columns[col].ParseToType(type))
                    {
                        list.Add(row[col]);
                    }
                }

                results[colInfo] = list.ToArray();
            }

            //foreach (DataRow row in DataSet.Tables[tableName].Rows)
            //{
            //    var dict = new Dictionary<string, object>();
            //    foreach (var col in columns)
            //    {
            //        if (filters == null || row[col] == filters[col])
            //        {
            //            dict[col] = row[col];
            //        }
            //    }

            //    results.Add(dict);
            //}

            return results;
        }

        private void CheckName(string tableName)
        {
            if (!DataSet.Tables.Contains(tableName))
                throw new InvalidOperationException($"Table {tableName} not found!");
        }

        private void CheckNameAndCount(string tableName, int colCount)
        {
            CheckName(tableName);
            if (DataSet.Tables[tableName].Columns.Count != colCount)
                throw new ArgumentException($"Amount of values provided ({colCount}) does not match column count");
        }
    }
}
