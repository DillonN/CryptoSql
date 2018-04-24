namespace CryptoSql.MySql.Enums
{
    public enum DataType : byte
    {
        Decimal,
        Tiny,
        Short,
        Long,
        Float,
        Double,
        Null,
        LongLong = 0x08,
        Date = 0x0a,
        Time,
        DateTime,
        Year,
        VarChar = 0x0f,
        Bit,
        Enum = 0xf7,
        VarString = 0xfd,
        String
    }
}
