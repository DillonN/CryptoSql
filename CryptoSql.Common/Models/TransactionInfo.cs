namespace CryptoSql.Common.Models
{
    public struct TransactionInfo
    {
        public readonly string Hash;
        public readonly CostInfo Cost;
        public readonly bool Success;

        public TransactionInfo(string hash, CostInfo cost, bool success)
        {
            Hash = hash;
            Cost = cost;
            Success = success;
        }
    }
}
