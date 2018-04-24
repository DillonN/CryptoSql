using System;
using CryptoSql.Common.Interfaces;
using CryptoSql.Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CryptoSql.Blockchain.Ethereum.StorageProviders {
    public abstract class StorageProvider : IStorageProvider
    {
        protected readonly EthereumConnection EthCon;
        
        private double _gasPrice;
        public double GasPrice
        {
            get => _gasPrice > 0 ? _gasPrice : EthereumConnection.LastGasPrice;
            set => _gasPrice = Math.Max(0, value);
        }

        public string RootHash { get; private set; }
        
        protected StorageProvider(string serverAddress, string keyStore, string password)
        {
            EthCon = new EthereumConnection(new Uri(serverAddress), keyStore, password, 7000000);
        }

        #region Abstract IStorageProvider methods

        public abstract Task<bool> UseAddress(string address);

        public abstract Task Reset();
        
        public abstract Task<TransactionInfo> CommitAsync();

        public abstract CostInfo GetEstimatedCommitCostAsync();

        public abstract void DropTableAsync(string tableName);

        public abstract bool NewTableAsync(string name, IEnumerable<ColumnInfo> info);

        public abstract bool InsertRow(string tableName, IReadOnlyDictionary<string, string> values);

        public abstract bool UpdateRowAsync(string tableName, IReadOnlyDictionary<string, string> values);

        public abstract bool UpdateRowsAsync(string tableName, IEnumerable<IReadOnlyDictionary<string, string>> values);

        public abstract void DropRowsAsync(string tableName, IEnumerable<uint> indices);

        public abstract Dictionary<ColumnInfo, object[]> SelectRowsAsync(string tableName, IReadOnlyDictionary<string, string> columns);

        #endregion
    }
}
