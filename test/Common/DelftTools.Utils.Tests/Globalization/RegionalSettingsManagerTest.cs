using System;
using System.Globalization;
using System.Threading;
using DelftTools.Utils.Globalization;
using NUnit.Framework;
using SharpTestsEx;

namespace DelftTools.Utils.Tests.Globalization
{
    [TestFixture]
    public class RegionalSettingsManagerTest
    {
        [Test]
        public void ChangeLanguage()
        {
            var languageChangedCallCount = 0;

            RegionalSettingsManager.LanguageChanged += delegate { languageChangedCallCount++; };

            RegionalSettingsManager.Language = "ru-RU";

            languageChangedCallCount
                .Should("event is fired when language is changed").Be.EqualTo(1);

            RegionalSettingsManager.Language
                                   .Should().Be.EqualTo("ru-RU");
        }

        [Test]
        public void SetRealNumberFormat()
        {
            var formatChangedCallCount = 0;
            RegionalSettingsManager.FormatChanged += delegate { formatChangedCallCount++; };

            RegionalSettingsManager.RealNumberFormat = "0:0.0";

            var value = 1000000.0001;

            var formatProvider = RegionalSettingsManager.GetCustomFormatProvider();

            var str = string.Format(formatProvider, "{0}", value);

            var currentNumberFormat = Thread.CurrentThread.CurrentCulture.NumberFormat;

            str
                .Should().Be.EqualTo("1000000" + currentNumberFormat.NumberDecimalSeparator + "0");

            formatChangedCallCount
                .Should().Be.EqualTo(1);
        }

        [Test]
        public void TestForLowerAndUpperCaseInTurkishLanguage()
        {
            // This test is just to identify the problem and a possible solution
            Assert.AreEqual("i", "I".ToLower());
            Assert.AreEqual("I", "i".ToUpper());
            Assert.IsTrue(String.Compare("file", "FILE", StringComparison.CurrentCultureIgnoreCase) == 0);
            var currentCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
                // ToLower and ToUpper do not work as expected in Turkish and Azeri language
                Assert.AreNotEqual("i", "I".ToLower());
                Assert.AreEqual("\u0131", "I".ToLower());
                Assert.AreNotEqual("i".ToLower(), "I".ToLower());
                Assert.AreEqual("i", "I".ToLower(CultureInfo.InvariantCulture));

                Assert.AreNotEqual("I", "i".ToUpper());
                Assert.AreEqual("\u0130", "i".ToUpper());
                Assert.AreNotEqual("i".ToUpper(), "I".ToUpper());
                Assert.AreEqual("I", "i".ToUpper(CultureInfo.InvariantCulture));

                Assert.IsFalse(String.Compare("file", "FILE", StringComparison.CurrentCultureIgnoreCase) == 0);
                // InvariantCultureIgnoreCase works ok to solve this problem
                Assert.IsTrue(String.Compare("file", "FILE", StringComparison.InvariantCultureIgnoreCase) == 0);
                // But adviced is to use OrdinalIgnoreCase for handling combinations of characters,
                // see http://msdn.microsoft.com/en-us/library/ms973919.aspx#stringsinnet20_topic5
                Assert.IsTrue(String.Compare("file", "FILE", StringComparison.OrdinalIgnoreCase) == 0);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }
    }
}