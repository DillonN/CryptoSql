using System;

namespace CryptoSql.MySql.Enums.BitFlags
{
    [Flags]
    public enum ServerCapabilitiesExt : ushort
    {
        None,
        MultStatements = 0b00000000_00000001,
        MultResults = 0b00000000_00000010,
        PSMultResults = 0b00000000_00000100,
        PluginAuth = 0b00000000_00001000,
        ConnectAttrs = 0b00000000_00010000,
        AuthClientData = 0b00000000_00100000,
        ClientHandleExpiredPass = 0b00000000_01000000,
        SessionVarTracking = 0b00000000_10000000,
        DeprecateEof = 0b00000001_00000000
    }
}
