using System;
using System.Globalization;
using System.Threading;
using NUnit.Framework;

namespace DelftTools.Utils.Tests
{
    [TestFixture]
    public class ConversionHelperTest
    {
        private CultureInfo previousCulture;

        [SetUp]
        public void TestFixtureSetup()
        {
            previousCulture = Thread.CurrentThread.CurrentCulture;
        }

        [TearDown]
        public void TestFixtureTearDown()
        {
            Thread.CurrentThread.CurrentCulture = previousCulture;
        }

        [Test]
        public void ConvertCommasAsThousandsSeparator()
        {
            string s = "1,121";
            Assert.AreEqual(1121f, ConversionHelper.ToSingle(s)); 
        }

        [Test]
        public void ConvertPeriodAsDecimalSeparator()
        {
            string s = "1.12";
            Assert.AreEqual(1.12f, ConversionHelper.ToSingle(s));
        }

        [Test]
        public void ConvertPeriodInAWeirdCulture()
        {
            var s = "1.123";

            // This is why conversionhelper exists.
            Assert.AreEqual(1123f, Convert.ToSingle(s, new CultureInfo("nl-NL", false)));
            Assert.AreEqual(1.123f, ConversionHelper.ToSingle(s));
        }

    }
}