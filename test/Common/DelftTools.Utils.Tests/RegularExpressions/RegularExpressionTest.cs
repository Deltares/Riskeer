using DelftTools.Utils.RegularExpressions;
using NUnit.Framework;

namespace DelftTools.Utils.Tests.RegularExpressions
{
    [TestFixture]
    public class RegularExpressionTest
    {
        [Test]
        public void GetFloat()
        {
            string source = "a 1.8 b 2.2 c 4.5";
            string pattern = RegularExpression.GetFloat("a")
                             + RegularExpression.GetFloat("b")
                             + RegularExpression.GetFloat("c");
            var match = RegularExpression.GetFirstMatch(pattern, source);
            Assert.AreEqual("1.8",match.Groups["a"].Value);
            Assert.AreEqual("2.2", match.Groups["b"].Value);
            Assert.AreEqual("4.5", match.Groups["c"].Value);
        }
        
        [Test]
        public void GetFloatOptionalIsReadWhenFound()
        {
            string pattern = RegularExpression.GetFloat("a")
                             + RegularExpression.GetFloatOptional("b")
                             + RegularExpression.GetFloat("c");
            
            string source = "a 1.8 b 2.2 c 4.5";
            
            var match = RegularExpression.GetFirstMatch(pattern, source);
            Assert.AreEqual("1.8", match.Groups["a"].Value);
            Assert.AreEqual("2.2", match.Groups["b"].Value);
            Assert.AreEqual("4.5", match.Groups["c"].Value);

        }
        [Test]
        public void GetFloatOptionalIsOptional()
        {
            string source = "a 1.8 c 4.5";
            string pattern = RegularExpression.GetFloat("a")
                             + RegularExpression.GetFloatOptional("b")
                             + RegularExpression.GetFloat("c");
            var match = RegularExpression.GetFirstMatch(pattern, source);
            Assert.AreEqual("1.8", match.Groups["a"].Value);
            Assert.AreEqual("4.5", match.Groups["c"].Value);
        }

        [Test]
        public void GetScientific()
        {
            var p = RegularExpression.GetScientific("a"); 

            Assert.AreEqual("1.8", RegularExpression.GetFirstMatch(p, "a 1.8").Groups["a"].Value.Trim());
            Assert.AreEqual("2.9e02", RegularExpression.GetFirstMatch(p, "a 2.9e02").Groups["a"].Value.Trim());
            Assert.AreEqual("-1e-1", RegularExpression.GetFirstMatch(p, "a -1e-1").Groups["a"].Value.Trim());
            Assert.AreEqual("+0.2e+0001", RegularExpression.GetFirstMatch(p, "a +0.2e+0001").Groups["a"].Value.Trim());
            Assert.AreEqual(".333", RegularExpression.GetFirstMatch(p, "a .333").Groups["a"].Value.Trim());
            Assert.AreEqual("+12345", RegularExpression.GetFirstMatch(p, "a +12345").Groups["a"].Value.Trim());
            Assert.AreEqual("12.", RegularExpression.GetFirstMatch(p, "a 12.").Groups["a"].Value.Trim());
        }

        /// <summary>
        /// added extra + to pattern to support extra spaces
        /// </summary>
        [Test]
        public void GetFloatPattern()
        {
            string pattern = RegularExpression.GetFloat("a");
            Assert.AreEqual(@"a\s+(?<a>[0-9\.-]+)+\s?",pattern);
        }
    }
}