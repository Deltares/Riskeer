using System;
using System.Globalization;

using Core.Common.Utils.Extensions;

using NUnit.Framework;

namespace Core.Common.Utils.Test.Extensions
{
    [TestFixture]
    public class StringExtensionsTests
    {
        [Test]
        public void Parse()
        {
            Assert.AreEqual(12.34, "12.34".Parse<double>(CultureInfo.GetCultureInfo("nl-NL")));

            Assert.AreEqual(1234, "1234".Parse<int>());

            Assert.AreEqual(0, "asdf".Parse<int>()); // i.e. default(int)

            Assert.AreEqual(1234, "1234".Parse<int?>());

            Assert.IsNull("asdf".Parse<int?>());

            Assert.AreEqual(new DateTime(2001, 2, 3), "2001-02-03".Parse<DateTime?>());
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