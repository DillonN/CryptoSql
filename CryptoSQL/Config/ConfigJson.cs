using CryptoSql.Common.Enums;
using System;
using System.Collections.Generic;

namespace CryptoSql.Config
{
    [Serializable]
    public class ConfigJson
    {
        public string GethHost { get; set; } = "http://localhost:8545";
        public string Password { get; set; }

        public Dictionary<ProviderType, Dictionary<string, string>> DatabaseAddressed { get; set; } = new Dictionary<ProviderType, Dictionary<string, string>>();
    }
}
