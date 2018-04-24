using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace CryptoSql.MySql.Extensions.Tests {
    [TestClass]
    public class NumbersTest
    {
        private readonly Dictionary<ulong, byte[]>  _tests = new Dictionary<ulong, byte[]>
        {
            {
                250, 
                new[] {(byte) 0xfa}
            },
            {
                251, 
                new[] {(byte) 0xfc, (byte) 0xfb, (byte) 0x00}
            },
            {
                65580, 
                new [] {(byte) 0xfd, (byte) 0x2c, (byte) 0x00, (byte) 0x01}
            },
            {
                268435456, 
                new [] {(byte) 0xfe, (byte) 0x00, (byte) 0x00, (byte) 0x00, (byte) 0x10, (byte) 0x00, (byte) 0x00, (byte) 0x00, (byte) 0x00}
            },
            {
                1152921506911829280, 
                new [] {(byte) 0xfe, (byte) 0x20, (byte) 0x3d, (byte) 0x63, (byte) 0x89, (byte) 0x00, (byte) 0x00, (byte) 0x00, (byte) 0x10}
            }
        };

        [TestMethod]
        public void LengthEncodedInt_ShouldMatch()
        {

            foreach (var test in _tests.Keys)
            {
                var lei = test.ToLengthEncoded();
                CollectionAssert.AreEqual(_tests[test], lei.ToArray());
            }
        }
    }
}
