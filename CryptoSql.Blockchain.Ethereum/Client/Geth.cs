using System;
using System.Diagnostics;

namespace CryptoSql.Blockchain.Ethereum.Client
{
    public class Geth : IDisposable
    {
        private const string PathToGeth =
            @"C:\Users\dillo\Documents\code\CryptoSQL\CryptoSQL\bin\Debug\netcoreapp2.0\bin\geth\geth.exe";

        private Process _geth;

        public Uri RunRinkebyTestNet()
        {
            _geth = new Process
            {
                StartInfo =
                {
                    Arguments = "--rinkeby",
                    FileName = PathToGeth
                }
            };

            _geth.Start();

            return new Uri("http://localhost:8545");
        }

        public void Stop()
        {
            Dispose();
        }

        public void Dispose()
        {
            _geth?.Dispose();
        }
    }
}
