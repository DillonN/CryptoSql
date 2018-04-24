using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CryptoSql.Common.Models;
using CryptoSql.Common.Models.Commands;

namespace CryptoSql.Common.Interfaces {
    public interface IServerDaemon
    {
        bool Start();
        bool Start(ServerInfo info);
        void Stop();

        Func<Command, Stopwatch, Task<CommandResponse>> CommandReceivedAsync { set; }
    }
}
