using CryptoSql.Blockchain.Ethereum.StorageProviders.Transaction.Monolithic;
using CryptoSql.Common.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using CryptoSql.Blockchain.Ethereum.Tests.StorageProviders.Transaction.Monolithic;
using CryptoSql.Config;

namespace CryptoSql.Blockchain.Ethereum.Tests
{
    [TestClass]
    public class EthereumConnectionTest
    {
        private const string TransHash = "0x698487d4ab54de9d113f09c1d3d0c9dfc1127c1bb2b4969e4f2e396b209e36f8";
        private const string TransData = "0x59381d2e000000000000000000000000000000000000000000000000000000005c2b1e0000000000000000000000000000000000000000000000000000000000001234560000000000000000000000004cb6a9ea5c5ac6583285b80c04c3410598ac6670000000000000000000000000113b462d14c542d208f5262d82e2eafd7cffd88a63727970746f000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000e00000000000000000000000000000000000000000000000000000000000000160000000000000000000000000000000000000000000000000000000000000005657696c6c2052455020746f6b656e7320626520776f727468206d6f7265207468616e2035303020555344206561636820616e792074696d6520647572696e672032303138206f6e20436f696e4d61726b65744361703f0000000000000000000000000000000000000000000000000000000000000000000000000000000000a57b227265736f6c7574696f6e536f75726365223a2268747470733a2f2f6170692e636f696e6d61726b65746361702e636f6d2f76312f7469636b65722f6175677572222c2274616773223a5b2274726164696e67222c2252657075746174696f6e225d2c226c6f6e674465736372697074696f6e223a22687474703a2f2f636f696e6d61726b65746361702e636f6d2f63757272656e636965732f657468657265756d227d000000000000000000000000000000000000000000000000000000";

        private const string NewData = "0x0005657696c6c2052455020746f6b656e7320626520776f727468206d6f7265207468616e2035303020555344206561636820616e792074696d6520647572696e672032303138206f6e20436f696e4d61726b65744361703f000000000000000000000000000000000000000000000000000000000000000000000000000000000";

        public const string Address = "http://localhost:8545";

        private const int TransHashLength = 66;

        private static EthereumConnection _ethCon;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            var uri = new Uri(Address);
            _ethCon = new EthereumConnection(uri, ConfigManager.KeyStore, ConfigManager.Config.Password);
        }

        [TestMethod]
        public async Task TransactionData_ShouldMatch()
        {
            var data = await _ethCon.GetTransactionData(TransHash);
            CollectionAssert.AreEqual(TransData.ToByteArray(), data);
        }

        [TestMethod]
        [TestCategory("SkipWhenLiveUnitTesting")]
        public async Task SentTransaction_ShouldMatch()
        {
            var data = NewData.ToByteArray();
            var receipt = await _ethCon.PostNewTransaction(data, EthmonolithicProviderTest.GasPriceWei);
            var hash = receipt.Hash;
            Assert.IsFalse(hash == null);
            Assert.AreEqual(66, hash.Length);
            Assert.AreEqual(27148, receipt.Cost.BaseCost);
            Assert.IsTrue(receipt.Success);

            var transData = await _ethCon.GetTransactionData(hash);
            CollectionAssert.AreEqual(data, transData);
        }

        [TestMethod]
        public void GasCostShouldMatch()
        {
            var bytes = DataSetConverter.ToBytes(DataSetTestContent.DS);
            Assert.AreEqual(DataSetTestContent.Gas, EthereumConnection.EstimateGasCost(bytes));
        }

        [TestMethod]
        public void GasCostUncompressedShouldMatch()
        {
            var bytes = DataSetConverter.ToBytes(DataSetTestContent.DS, false);
            Assert.AreEqual(DataSetTestContent.UncompressedGas, EthereumConnection.EstimateGasCost(bytes));
        }
    }
}
