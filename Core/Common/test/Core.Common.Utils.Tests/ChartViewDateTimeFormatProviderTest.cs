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
        public void GetCorrectFormatForRange()
        {
            var provider = new TimeNavigatableLabelFormatProvider();

            var oldCulture = Thread.CurrentThread.CurrentCulture;
            var oldCultureUI = Thread.CurrentThread.CurrentUICulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            provider.CustomDateTimeFormatInfo = CultureInfo.InvariantCulture.DateTimeFormat;

            var annotation = provider.GetRangeLabel(new DateTime(2010, 1, 1), new DateTime(2010, 10, 1));

            annotation.Should("Annotation not as expected.").Be.EqualTo("(Friday, 01 January 2010 till Friday, 01 October 2010)");

            var label1 = provider.GetLabel(new DateTime(2010, 1, 1), new TimeSpan(2, 1, 1, 1));
            var label2 = provider.GetLabel(new DateTime(2010, 1, 1), new TimeSpan(600, 1, 1, 1));

            label1.Should("Label 1 not as expected").Be.EqualTo("01/01/2010 00:00");
            label2.Should("Label 2 not as expected").Be.EqualTo("2010 January");

            Thread.CurrentThread.CurrentCulture = oldCulture;
            Thread.CurrentThread.CurrentUICulture = oldCultureUI;
        }
    }
}