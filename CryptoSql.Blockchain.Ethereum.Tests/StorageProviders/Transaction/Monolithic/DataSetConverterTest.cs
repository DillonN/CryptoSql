using System.Data;
using System.Linq;
using CryptoSql.Blockchain.Ethereum.StorageProviders.Transaction.Monolithic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CryptoSql.Blockchain.Ethereum.Tests.StorageProviders.Transaction.Monolithic
{
    [TestClass]
    public class DataSetConverterTest
    {
        [TestMethod]
        public void ShouldConvertCompressed()
        {
            var bytes = DataSetConverter.ToBytes(DataSetTestContent.DS);
            var fromBytes = DataSetConverter.FromBytes(bytes);
            
            Assert.IsTrue(IsSetSame(DataSetTestContent.DS, fromBytes));
            Assert.IsFalse(IsSetSame(DataSetTestContent.FalseDS, fromBytes));
        }

        [TestMethod]
        public void ShouldConvertUncompressed()
        {
            var unBytes = DataSetConverter.ToBytes(DataSetTestContent.DS, false);
            var fromUnBytes = DataSetConverter.FromBytes(unBytes, false);
            
            Assert.IsTrue(IsSetSame(DataSetTestContent.DS, fromUnBytes));
        }

        public static bool IsSetSame(DataSet s1, DataSet s2)
        {
            if (s1 == null || s2 == null ||
                s1?.Tables.Count != s2?.Tables.Count ||
                s1.DataSetName != s2.DataSetName)
            {
                return false;
            }

            foreach (DataTable t in s1.Tables)
            {
                var from = s2.Tables[t.TableName];
                if (from == null || !IsTableSame(t, from))
                    return false;
            }

            return true;
        }

        private static bool IsTableSame(DataTable t1, DataTable t2)
        {
            if (t1 == null || t2 == null ||
                t1?.Rows.Count != t2?.Rows.Count ||
                t1.TableName != t2.TableName ||
                t1.Columns.Count != t2.Columns.Count ||
                t1.Columns.Cast<DataColumn>().Any(dc => !t2.Columns.Contains(dc.ColumnName)))
            {
                return false;
            }
            

            for (var i = 0; i <= t1.Rows.Count-1; i++)
            {
                if (t1.Columns.Cast<DataColumn>().Any(dc1 => t1.Rows[i][dc1.ColumnName].ToString() != t2.Rows[i][dc1.ColumnName].ToString()))
                {
                    return false;
                } 
            }

            return true;
        }
    }
}
