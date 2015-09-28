using System;
using System.Globalization;
using NUnit.Framework;
using SharpTestsEx;

namespace DelftTools.Utils.Tests
{
    [TestFixture]
    public class StringExtensionsTests
    {
        [Test]
        public void Parse()
        {
            
            "12.34".Parse<double>(CultureInfo.InvariantCulture).Should().Be.EqualTo(12.34);

            "1234".Parse<int>().Should().Be.EqualTo(1234);
            
            "asdf".Parse<int>().Should().Be.EqualTo(0); // i.e. default(int)
            
            "1234".Parse<int?>().Should().Be.EqualTo(1234);
            
            "asdf".Parse<int?>().Should().Be.EqualTo(null);
            
            "2001-02-03".Parse<DateTime?>().Should().Be.EqualTo(new DateTime(2001, 2, 3));
        }

        [Test]
        public void ReplaceFirst()
        {
            const string text = "Lorem ipsum lorem ipsum";

            Assert.AreEqual("Lorem test lorem ipsum", text.ReplaceFirst("ipsum", "test"));
            Assert.AreEqual("Lorem test lorem test", text.ReplaceFirst("ipsum", "test").ReplaceFirst("ipsum", "test"));
            Assert.AreEqual("Lorem test lorem test", text.ReplaceFirst("ipsum", "test").ReplaceFirst("ipsum", "test").ReplaceFirst("ipsum", "test"));
        }
    }
}