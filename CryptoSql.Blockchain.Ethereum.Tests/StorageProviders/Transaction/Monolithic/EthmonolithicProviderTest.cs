using System.Data;
using System.Threading.Tasks;
using CryptoSql.Blockchain.Ethereum.Currency;
using CryptoSql.Blockchain.Ethereum.StorageProviders.Transaction.Monolithic;
using CryptoSql.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CryptoSql.Blockchain.Ethereum.Tests.StorageProviders.Transaction.Monolithic
{
    [TestClass]
    public class EthmonolithicProviderTest
    {
        private const double GasPriceGWei = 1;

        public static double GasPriceWei => EthUnitConverter.ConvertGWeiToWei(GasPriceGWei);

        private const string DBHash = "0x9831774f8b03dc384dfbbda78a7e0863cb9f032f0a49de119ce2b768f6e7793b";

        [TestMethod]
        [TestCategory("SkipWhenLiveUnitTesting")]
        public async Task PublishedDataSetShouldMatch()
        {
            var provider = new EthMonolithicProvider(EthereumConnectionTest.Address, ConfigManager.KeyStore,
                ConfigManager.Config.Password, DataSetTestContent.DS)
            {
                GasPrice = GasPriceWei
            };
            var info = await provider.CommitAsync();

            Assert.IsTrue(info.Success);
            Assert.AreEqual(61708, info.Cost.BaseCost);
            Assert.IsTrue(await IsTransactionDBMatch(info.Hash, DataSetTestContent.DS));
        }

        [TestMethod]
        public async Task VerifyPostedTransaction()
        {
            // This is handled in the above test, but we'll have it here for live testing
            Assert.IsTrue(await IsTransactionDBMatch(DBHash, DataSetTestContent.DS));
        }

        private static async Task<bool> IsTransactionDBMatch(string hash, DataSet ds)
        {
            var provider = new EthMonolithicProvider(EthereumConnectionTest.Address, ConfigManager.KeyStore, ConfigManager.Config.Password);

            if (!await provider.UseAddress(hash))
                return false;

            return DataSetConverterTest.IsSetSame(ds, provider.DataSet);
        }
    }
}
