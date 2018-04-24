namespace CryptoSql.Common.Models
{
    public class CostInfo
    {
        public double BaseCost { get; }
        public string BaseCurrency { get; }

        public double CadCost { get; }

        public CostInfo(double cost, string currency)
        {
            BaseCost = cost;
            BaseCurrency = currency;
        }
    }
}
