using System;

namespace CryptoSql.MySql.Enums.BitFlags
{
    [Flags]
    public enum Capabilities : ushort
    {
        None,
        LongPasswords = 0b00000000_00000001,
        FoundRows = 0b00000000_000000010,
        LongColumnFlags = 0b00000000_00000100,
        ConnectWithDB = 0b00000000_00001000,
        DontAllowDBTC = 0b00000000_00010000,
        CanUseCompr = 0b00000000_00100000,
        OdbcClient = 0b00000000_01000000,
        CanUseLdl = 0b00000000_10000000,
        IgnoreSpacesBeforeBrack = 0b00000001_00000000,
        Speaks41 = 0b00000010_00000000,
        InteractiveClient = 0b00000100_00000000,
        SwitchToSsl = 0b00001000_00000000,
        IgnoreSigpipes = 0b00010000_00000000,
        KnowsAboutTrans = 0b00100000_00000001,
        Speaks41Old = 0b01000000_00000000,
        CanDo41Auth = 0b10000000_00000000
    }
}
