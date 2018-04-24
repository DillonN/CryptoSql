using System;

namespace CryptoSql.Blockchain.Ethereum.Currency
{
    public static class EthUnitConverter
    {
        public static double Convert(double amount, EthUnit fromUnit = EthUnit.Wei, EthUnit toUnit = EthUnit.Eth)
        {
            var exp = fromUnit - toUnit;

            return amount * Math.Pow(10, exp * 3);
        }

        public static double ConvertGWeiToWei(double amount)
        {
            return Convert(amount, EthUnit.GWei, EthUnit.Wei);
        }
    }
}
