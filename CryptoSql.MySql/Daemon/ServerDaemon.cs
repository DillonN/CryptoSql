using CryptoSql.Common.Models;
using CryptoSql.MySql.EventArgs;
using CryptoSql.MySql.Packet.Decoded.CommandPackets;
using CryptoSql.MySql.Packet.Encoded.Interfaces;
using CryptoSql.MySql.Processing.Incoming;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CryptoSql.Common.Interfaces;
using CryptoSql.Common.Models.Commands;

// Arbitrarily specify this here
[assembly: InternalsVisibleTo("CryptoSql.MySql.Tests")]

namespace CryptoSql.MySql.Daemon 
{
    public class ServerDaemon : IServerDaemon
    {
        private const string VersionValue = "(CryptoSql)";
        private const int MaxTries = 100;

        private readonly CommandProcessor _processor = new CommandProcessor();

        public Func<Command, Stopwatch, Task<CommandResponse>> CommandReceivedAsync
        {
            set => _processor.CommandReceivedAsync = value;
        }

        public ServerInfo Info;
        

        public bool IsRunning { get; private set; }
        
        private readonly ConcurrentDictionary<uint, SqlConnection> _connections;

        public ServerDaemon()
        {
            _connections = new ConcurrentDictionary<uint, SqlConnection>();
        }

        public bool Start(ServerInfo info)
        {
            Info = info;
            info.VersionString = info.VersionString ?? VersionValue;

            return Start();
        }

        public bool Start()
        {
            if (Info == null)
                throw new InvalidOperationException($"Cannot start before setting {nameof(Info)}");

            var listener = new TcpListener(Info.Address, Info.Port);
            for (var i = 0; i < MaxTries; i++)
            {
                Console.WriteLine($"Starting listening on port {Info.Port}");
                try
                {
                    listener.Start();
                    IsRunning = true;
                    break;
                }
                catch (SocketException e)
                {
                    Console.WriteLine(e.Message);
                    Info.Port++;
                    listener = new TcpListener(Info.Address, Info.Port);
                    Thread.Sleep(1000);
                }
            }

            if (!IsRunning) return false;

            var t = new Thread(StartThreadRoutine);
            t.Start(listener);

            return true;
        }

        public void Stop()
        {
            // TODO
            throw new NotImplementedException();
        }

        private void StartThreadRoutine(object o)
        {
            if (o is TcpListener listener)
                ListenAsync(listener).Wait();
        }

        private async Task ListenAsync(TcpListener listener)
        {
            while (IsRunning)
            {
                var client = await listener.AcceptTcpClientAsync().ConfigureAwait(false);

                var didAdd = false;

                for (uint i = 0; i < Info.MaxConnections; i++)
                {
                    var connection = new SqlConnection(client, i, CommandReceived);
                    if (_connections.TryAdd(i, connection))
                    {
                        Console.WriteLine($"Connection creating with id {i}");
                        
                        connection.OnClose += ConnectionOnClose;
#pragma warning disable 4014
                        connection.HandleClient(Info);
#pragma warning restore 4014

                        didAdd = true;
                        break;
                    }
                }
                if (!didAdd)
                {
                    Console.WriteLine("Connection attempted but already at max");
                    client.Close();
                }
            }
        }

        private async Task<IServerResponse> CommandReceived(CommandPacket commandPacket, Stopwatch timer)
        {
            return await _processor.GetResponse(commandPacket, Info, timer);
        }

        private void ConnectionOnClose(object sender, SqlConnectionClosedEventArgs e)
        {
            _connections.TryRemove(e.ThreadID, out _);

            Console.WriteLine($"Connection {e.ThreadID} closed");
        }
    }
}
