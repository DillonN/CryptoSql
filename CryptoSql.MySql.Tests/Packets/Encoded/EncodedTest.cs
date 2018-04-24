using System.Collections.Generic;
using System.Linq;
using CryptoSql.MySql.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CryptoSql.MySql.Tests.Packets.Encoded {
    [TestClass]
    public class EncodedTest
    {
        [TestMethod]
        public void LengthEncodedInt_ShouldMatch()
        {
            var tests = new Dictionary<uint, byte[]>
            {
                {250, new[] {(byte) 0xfa}},
                {251, new[] {(byte) 0xfc, (byte) 0xfb, (byte) 0x00}}
            };

            foreach (var test in tests.Keys)
            {
                var lei = test.ToLengthEncoded();
                CollectionAssert.AreEqual(tests[test], lei.ToArray());
            }
        }
    }
}
