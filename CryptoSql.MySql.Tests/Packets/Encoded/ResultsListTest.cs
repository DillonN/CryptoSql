using System.Collections.Generic;
using CryptoSql.Common.Extensions;
using CryptoSql.MySql.Packet.Encoded.ResultsSet;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CryptoSql.MySql.Tests.Packets.Encoded
{
    [TestClass]
    public class ResultsListTest
    {
        [TestMethod]
        public void ColumnDefinition_Matches()
        {
            var def = new ColumnDefinition("@@version_comment");
            var expect = "1c0000000011404076657273696f6e5f636f6d6d656e740318000001fd02001f".ToByteArray();
            CollectionAssert.AreEqual(expect, def.ToByteArray());
        }

        [TestMethod]
        public void ResultsRow_Matches()
        {
            var tests = new Dictionary<string[], byte[]>
            {
                {new[] {"(Ubuntu)"}, "0900000008285562756e747529".ToByteArray()}
            };

            foreach (var key in tests.Keys)
            {
                var row = new ResultsRow(key);
                CollectionAssert.AreEqual(tests[key], row.ToByteArray());
            }
        }
    }
}
