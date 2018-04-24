using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Text;

namespace CryptoSql.MySql.Extensions.Tests {
    [TestClass]
    public class StringTest
    {
        private readonly string[] _tests = 
        {
            "blarghonk",
            "asdfkjasdfhjopqwiepoiquwe98efinoaisdcnlkjn",
            "asdflkjhwqioeuriouqweuioasdcinqwipe9p2q3989p21349p823opiuasudfnaiudfiounqw9per92qp3r9p81239rp8q2wn3r89udfnjxzndvlkjasdnopqwuefnr9poqweunfuioadcn"
        };

        [TestMethod]
        public void EncodedLength_ShouldMatch()
        {
            foreach (var tag in _tests)
            {
                var len = tag.Length.ToLengthEncoded();
                var encoded = Encoding.ASCII.GetBytes(tag);

                CollectionAssert.AreEqual(len.Concat(encoded).ToArray(), tag.ToLengthEncoded().ToArray());
            }
        }
    }
}
