using CryptoSql.Blockchain.Ethereum.Currency;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CryptoSql.Blockchain.Ethereum.Tests.Currency
{
    [TestClass]
    public class EthUnitConverterTest
    {
        private const double TestGwei = 20;
        private const double TestWei = 20000000000;

        [TestMethod]
        public void GweiToWei_ShouldMatch()
        {
            var wei = EthUnitConverter.ConvertGWeiToWei(TestGwei);
            var gWei = EthUnitConverter.Convert(TestWei, EthUnit.Wei, EthUnit.GWei);

            Assert.AreEqual(TestGwei, gWei);
            Assert.AreEqual(TestWei, wei);
        }
    }
}
