using CryptoSql.MySql.EventArgs;
using CryptoSql.MySql.Packet.Decoded;
using CryptoSql.MySql.Packet.Decoded.CommandPackets;
using CryptoSql.MySql.Packet.Encoded;
using CryptoSql.MySql.Packet.Encoded.Interfaces;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using CryptoSql.Common.Models;

namespace CryptoSql.MySql.Daemon
{
    internal class SqlConnection
    {
        private TcpClient _client;
        private readonly Func<CommandPacket, Stopwatch, Task<IServerResponse>> _commandFunc;

        public EventHandler<SqlConnectionClosedEventArgs> OnClose;

        public readonly uint ThreadID;

        private string _database = "";

        public string Database
        {
            get
            {
                lock (_database)
                {
                    return _database;
                }
            }
            set
            {
                lock (_database)
                {
                    _database = value;
                }
            }
        }

        public SqlConnection(TcpClient client, uint id, Func<CommandPacket, Stopwatch, Task<IServerResponse>> command)
        {
            _client = client;
            ThreadID = id;
            _commandFunc = command;
        }

        public async Task HandleClient(ServerInfo info)
        {
            try
            {
                using (var stream = _client.GetStream())
                {
                    // Send hello packet
                    var inHandshake = true;
                    var hello = new GreetingPacket10(ThreadID, info.VersionString).ToByteArray();
                    await stream.WriteAsync(hello, 0, hello.Length).ConfigureAwait(false);
                    
                    while (true)
                    {
                        var rec = new byte[ushort.MaxValue << 2];
                        var i = await stream.ReadAsync(rec, 0, rec.Length).ConfigureAwait(false);
                        if (i <= 0) break;

                        try
                        {
                            var data = rec.Take(i).ToArray();
                            byte[] ret;
                            if (inHandshake)
                            {
                                var login = new LoginPacket(data);
                                ret = HandshakeResponse(login);
                                inHandshake = false;
                            }
                            else
                            {
                                var timer = info.ReportTimings ? new Stopwatch() : null;
                                timer?.Start();
                                var command = new CommandPacket(data, Database);
                                ret = (await _commandFunc(command, timer)).ToByteArray();
                            }

                            if (ret.Length > 0)
                                await stream.WriteAsync(ret, 0, ret.Length).ConfigureAwait(false);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Exception occurred on thread {ThreadID}: {e}");
                        }
                    }
                }
            }
            finally
            {
                if (_client != null)
                {
                    (_client as IDisposable).Dispose();
                    _client = null;
                }
                OnClose?.Invoke(this, new SqlConnectionClosedEventArgs(ThreadID));
            }
        }

        private byte[] HandshakeResponse(LoginPacket login)
        {
            // TODO authentication, etc
            return new OKPacket(number: 2).ToByteArray();
        }
    }
}
