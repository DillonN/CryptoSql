using CryptoSql.Common.Models;
using CryptoSql.MySql.Extensions;
using CryptoSql.MySql.Packet.Decoded.CommandPackets;
using CryptoSql.MySql.Packet.Encoded;
using CryptoSql.MySql.Processing.Incoming;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoSql.Common.Extensions;

namespace CryptoSql.MySql.Tests.Processing.Incoming {
    [TestClass]
    public class CommandProcessorTest
    {
        private readonly ServerInfo _info = new ServerInfo
        {
            VersionString = "Blarg"
        };

        private readonly CommandProcessor _processor = new CommandProcessor();

        private readonly byte[] _invalidCommand = "02000000ff00".ToByteArray();

        private readonly byte[] _versionQuery = "210000000373656c65637420404076657273696f6e5f636f6d6d656e74206c696d69742031".ToByteArray();

        [TestMethod]
        public async Task GetResponse_ShouldThrowArgEx_WhenInvalidType()
        {
            var packet = new CommandPacket(_invalidCommand);
            try
            {
                await _processor.GetResponse(packet, _info, null);
                Assert.Fail("No exception thrown");
            }
            catch (ArgumentException)
            {
                // Good
            }
        }

        [TestMethod]
        public async Task GetResponse_ShouldReturnCorrectVersion()
        {
            var packet = new CommandPacket(_versionQuery);
            var response = await _processor.GetResponse(packet, _info, null);
            VerifyVerReponse(response.ToByteArray(), _info.VersionString);
        }

        public static void VerifyVerReponse(byte[] rec, string ver)
        {
            var x = new List<byte>();

            var count = "0100000101".ToByteArray();
            CollectionAssert.AreEqual(count, rec.Take(count.Length).ToArray());
            x.AddRange(count);

            var column = "00".ToByteArray().Concat(CommandProcessor.VersionField.ToLengthEncoded())
                .Concat("0318000001fd02001f".ToByteArray()).ToArray();
            column = BitConverter.GetBytes((uint) column.Length).Take(3).Append((byte) 0x02).Concat(column).ToArray();
            CollectionAssert.AreEqual(column, rec.Skip(x.Count).Take(column.Length).ToArray());
            x.AddRange(column);

            x.AddRange(EofPacket.Get(3));

            var row = BitConverter.GetBytes((uint) ver.Length + 1).Take(3).Append((byte) 0x04)
                .Concat(ver.ToLengthEncoded()).ToArray();
            CollectionAssert.AreEqual(row, rec.Skip(x.Count).Take(row.Length).ToArray());
            x.AddRange(row);

            x.AddRange(EofPacket.Get(5));

            CollectionAssert.AreEqual(x, rec);
        }
    }
}
