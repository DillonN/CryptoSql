using System.IO;
using Newtonsoft.Json;

namespace CryptoSql.Config
{
    public static class ConfigManager
    {
        private const string ConfigPath = @"C:\Users\Dillon\source\repos\CryptoSql\CryptoSQL\bin\Debug\netcoreapp2.0";
        private const string ConfigFile = "config.json";
        private const string KeyStoreFile = "keystore.json";

        public static readonly ConfigJson Config;
        public static readonly string KeyStore;

        static ConfigManager()
        {
            try
            {
                var json = File.ReadAllText(Path.Combine(ConfigPath, ConfigFile));
                Config = JsonConvert.DeserializeObject<ConfigJson>(json);
            }
            catch (FileNotFoundException)
            {
                Config = new ConfigJson();
            }

            try
            {
                KeyStore = File.ReadAllText(Path.Combine(ConfigPath, KeyStoreFile));
            }
            catch (FileNotFoundException) { }
        }

        public static void SaveConfig()
        {
            var json = JsonConvert.SerializeObject(Config);
            File.WriteAllText(Path.Combine(ConfigPath, ConfigFile), json);
        }
    }
}
