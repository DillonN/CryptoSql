using CryptoSql.Common.Models;

namespace CryptoSql.Blockchain.Ethereum.Models
{
    public class EthCostInfo : CostInfo
    {
        public double GasPrice { get; }

        public double EthCost => BaseCost * GasPrice;

        public EthCostInfo(double gas, double gasPrice)
            : base(gas, "GAS")
        {
            GasPrice = gasPrice;
        }
    }
}
