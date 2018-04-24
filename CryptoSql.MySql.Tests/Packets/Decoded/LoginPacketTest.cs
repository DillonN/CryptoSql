using CryptoSql.Common.Extensions;
using CryptoSql.MySql.Enums.BitFlags;
using CryptoSql.MySql.Packet.Decoded;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CryptoSql.MySql.Tests.Packets.Decoded
{
    [TestClass]
    public class LoginPacketTest
    {
        [TestMethod]
        public void LoginPacket_ShouldMatch()
        {
            var bytes = "0d0000018da400000064696c6c6f6e0000".ToByteArray();
            var login = new LoginPacket(bytes);
            Assert.AreEqual("", login.Database);
            Assert.AreEqual("dillon", login.User);
            Assert.AreEqual(0u, login.MaxPacket);

            var caps = login.Capabilities;

            Assert.IsTrue(caps.HasFlag(Capabilities.LongPasswords));
            Assert.IsFalse(caps.HasFlag(Capabilities.FoundRows));
            Assert.IsTrue(caps.HasFlag(Capabilities.LongColumnFlags));
            Assert.IsTrue(caps.HasFlag(Capabilities.ConnectWithDB));
            Assert.IsTrue(caps.HasFlag(Capabilities.CanDo41Auth));
        }
    }
}
