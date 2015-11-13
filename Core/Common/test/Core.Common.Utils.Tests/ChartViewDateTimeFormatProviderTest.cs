using System;
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using SharpTestsEx;

namespace Core.Common.Utils.Tests
{
    [TestFixture]
    public class ChartViewDateTimeFormatProviderTest
    {
        [Test]
        public void ReturnsSingleDayOutputForSingleDayRange()
        {
            var provider = new TimeNavigatableLabelFormatProvider();

            var oldCulture = Thread.CurrentThread.CurrentCulture;
            var oldCultureUI = Thread.CurrentThread.CurrentUICulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            provider.CustomDateTimeFormatInfo = CultureInfo.InvariantCulture.DateTimeFormat;
            var outputString = provider.GetRangeLabel(new DateTime(2010, 1, 1), new DateTime(2010, 1, 1));

            outputString.Should("Datetime formatted output not as expected").Be.EqualTo("(Friday, 01 January 2010)");

            Thread.CurrentThread.CurrentCulture = oldCulture;
            Thread.CurrentThread.CurrentUICulture = oldCultureUI;
        }

        [Test]
        [SetCulture("nl-NL")]
        public void GetCorrectFormatForRange_forNL()
        {
            GetCorrectFormatForRange();
        }

        [Test]
        [SetCulture("en-US")]
        public void GetCorrectFormatForRange_forEN()
        {
            GetCorrectFormatForRange();
        }

        private static void GetCorrectFormatForRange()
        {
            // Setup
            var provider = new TimeNavigatableLabelFormatProvider();

            var timeA = new DateTime(2010, 1, 1);
            var timeB = new DateTime(2010, 10, 1);

            DateTimeFormatInfo dateTimeFormatInfo = CultureInfo.CurrentCulture.DateTimeFormat;

            string longDatePattern = dateTimeFormatInfo.LongDatePattern;
            string shortDateTimePattern = dateTimeFormatInfo.ShortDatePattern + " " + dateTimeFormatInfo.ShortTimePattern;
            string yearMonthPattern = dateTimeFormatInfo.YearMonthPattern;

            var a = timeA.ToString(longDatePattern);
            var b = timeB.ToString(longDatePattern);
            var expectedAnnotation = String.Format("({0} tot {1})", a, b);

            // Call
            var annotation = provider.GetRangeLabel(timeA, timeB);

            // Assert
            Assert.AreEqual(expectedAnnotation, annotation);

            var label1 = provider.GetLabel(timeA, new TimeSpan(2, 1, 1, 1));
            var label2 = provider.GetLabel(timeA, new TimeSpan(600, 1, 1, 1));

            Assert.AreEqual(label1, timeA.ToString(shortDateTimePattern));
            Assert.AreEqual(label2, timeA.ToString(yearMonthPattern));
        }
    }
}