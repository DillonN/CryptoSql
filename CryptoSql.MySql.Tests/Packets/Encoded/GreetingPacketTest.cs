using CryptoSql.MySql.Packet.Encoded;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CryptoSql.MySql.Tests.Packets.Encoded
{
    [TestClass]
    public class GreetingPacketTest
    {
        [TestMethod]
        public void GreetingPacket10_ShouldReturnCorrectValue()
        {
            var packet = new GreetingPacket10(66738, "5.7.18-0ubuntu0.16.10.1-log", 8, 0x0002);

            var output = "3C0000000A352E372E31382D307562756E7475302E31362E31302E312D6C6F6700B2040100111111111111111100000008020000000000000000000000000000".ToUpper();
            
            Assert.AreEqual(output, packet.ToString());
        }

        [TestMethod]
        public void GreetingPacket9_ShouldReturnCorrectValue()
        {
            var packet = new GreetingPacket9(66738, "")
            {
                VersionString = "5.7.18-0ubuntu0.16.10.1-log"
            };

            var output = "2200000009352e372e31382d307562756e7475302e31362e31302e312d6c6f6700b204010000".ToUpper();

            Assert.AreEqual(output, packet.ToString());
        }
    }
}
