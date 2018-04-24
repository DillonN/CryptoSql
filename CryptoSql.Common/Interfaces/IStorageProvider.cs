using System.Collections.Generic;
using System.Threading.Tasks;
using CryptoSql.Common.Models;

namespace CryptoSql.Common.Interfaces
{
    public interface IStorageProvider
    {
        Task<bool> UseAddress(string address);

        Task Reset();
        /// <summary>
        /// Commit changes to the database
        /// </summary>
        /// <returns>true iff commit was successful</returns>
        Task<TransactionInfo> CommitAsync();
        /// <summary>
        /// Get the estimated cost associated with committing the database
        /// </summary>
        /// <returns><see cref="CostInfo"/> with associated costs</returns>
        CostInfo GetEstimatedCommitCostAsync();

        /// <summary>
        /// Remove table from database
        /// </summary>
        /// <param name="tableName">Name of table to remove</param>
        void DropTableAsync(string tableName);

        /// <summary>
        /// Add a new table to the database
        /// </summary>
        /// <param name="info">ColumnInfos for new table</param>
        /// <returns>true iff created</returns>
        bool NewTableAsync(string name, IEnumerable<ColumnInfo> info);

        bool InsertRow(string tableName, IReadOnlyDictionary<string, string> values);

        /// <summary>
        /// Update a row in the table
        /// </summary>
        /// <param name="tableName">Name of table to update</param>
        /// <param name="values"></param>
        /// <returns></returns>
        bool UpdateRowAsync(string tableName, IReadOnlyDictionary<string, string> values);

        bool UpdateRowsAsync(string tableName, IEnumerable<IReadOnlyDictionary<string, string>> values);

        void DropRowsAsync(string tableName, IEnumerable<uint> indices);

        Dictionary<ColumnInfo, object[]> SelectRowsAsync(string tableName, IReadOnlyDictionary<string, string> columns);
    }
}
