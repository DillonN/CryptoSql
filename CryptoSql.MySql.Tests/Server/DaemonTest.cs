using CryptoSql.Common.Extensions;
using CryptoSql.Common.Models;
using CryptoSql.MySql.Daemon;
using CryptoSql.MySql.Packet.Encoded;
using CryptoSql.MySql.Tests.Processing.Incoming;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace CryptoSql.MySql.Tests.Server
{
    [TestClass]
    public class DaemonTest
    {
        private const int Port = 6690;
        private const string Version = "Version1";

        private static TcpClient _client;
        private static NetworkStream _stream;
        
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            var info = new ServerInfo
            {
                Address = IPAddress.Any,
                Port = Port,
                MaxConnections = 2,
                VersionString = Version
            };
            var daemon = new ServerDaemon();
            Assert.IsTrue(daemon.Start(info));
            Assert.IsTrue(daemon.IsRunning);

            _client = new TcpClient("localhost", Port);

            Assert.IsTrue(_client.Connected);

            _stream = _client.GetStream();
            var rec = new byte[5000];
            var i = _stream.Read(rec, 0, rec.Length);
            var hello =
                "2a0000000A43727970746f53716c0000000000111111111111111100000008020000000000000000000000000000"
                    .ToByteArray();
            CollectionAssert.AreEqual(hello, rec.Take(i).ToArray());
            var login = "0d00000185a400000064696c6c6f6e0000".ToByteArray();
            _stream.Write(login, 0, login.Length);
            i = _stream.Read(rec, 0, rec.Length);
            var ok = new OKPacket(number: 2).ToByteArray();
            CollectionAssert.AreEqual(ok, rec.Take(i).ToArray());
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            _client?.Dispose();
        }

        //[TestMethod]
        //public void VersionQuery_ShouldReturn()
        //{
        //    var query = "210000000373656c65637420404076657273696f6e5f636f6d6d656e74206c696d69742031".ToByteArray();
        //    _stream.Write(query, 0, query.Length);
        //    var rec = new byte[5000];
        //    var i = _stream.Read(rec, 0, rec.Length);

        //    CommandProcessorTest.VerifyVerReponse(rec.Take(i).ToArray(), Version);
        //}
    }
}
