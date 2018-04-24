using CryptoSql.Common.Models;
using System;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using CryptoSql.Blockchain.Ethereum.Currency;
using CryptoSql.Blockchain.Ethereum.StorageProviders.Transaction.Monolithic;
using CryptoSql.Common.Enums;
using CryptoSql.Config;
using CryptoSql.MySql.Daemon;

namespace CryptoSql
{
    internal static class Program
    {
        private static readonly string Version = typeof(RuntimeEnvironment).GetTypeInfo().Assembly
            .GetCustomAttribute<AssemblyFileVersionAttribute>().Version;

        private static readonly string ProjectName = typeof(RuntimeEnvironment).GetTypeInfo()
            .Assembly.GetCustomAttribute<AssemblyProductAttribute>().Product;

        private static readonly string VersionString = $"{ProjectName}-{Version}";

        internal static void Main(string[] args)
        {
            Console.WriteLine("Starting up CryptoSql...");
            var info = new ServerInfo
            {
                Address = IPAddress.Any,
                Port = 3307,
                MaxConnections = 1,
                // MySQL ver
                VersionString = "5.0.0",
                ReportTimings = true
            };

            for (var i = 0; i < args.Length; i++)
            {
                if (i + 1 < args.Length)
                {
                    switch (args[i])
                    {
                        case "-p":
                            if (int.TryParse(args[i++], out var port))
                            {
                                info.Port = port;
                            }

                            break;

                        case "-m":
                            if (int.TryParse(args[i++], out var max))
                            {
                                info.MaxConnections = max;
                            }

                            break;
                    }
                }
            }

            var bridge = new Bridge(info);
            bridge.Providers[ProviderType.EthMonolithic] = new EthMonolithicProvider(ConfigManager.Config.GethHost,
                ConfigManager.KeyStore, ConfigManager.Config.Password)
            {
                GasPrice = EthUnitConverter.ConvertGWeiToWei(5)
            };
            bridge.AddDaemon(new ServerDaemon());
            bridge.StartAllDaemons();

            Console.ReadKey();
        }
    }
}
