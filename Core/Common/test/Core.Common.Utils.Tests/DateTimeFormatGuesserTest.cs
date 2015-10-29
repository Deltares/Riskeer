using System;
using System.Globalization;
using NUnit.Framework;

namespace Core.Common.Utils.Tests
{
    [TestFixture]
    public class DateTimeFormatGuesserTest
    {
        [Test]
        public void Guessing()
        {
            string formatDate, formatTimeFirst, formatFull, formatFull2;

            var dateTime = new DateTime(1999, 6, 15, 5, 13, 2);

            var shortDate = dateTime.ToString("d", CultureInfo.InvariantCulture.DateTimeFormat);
            var fullDate = dateTime.ToString("g", CultureInfo.InvariantCulture.DateTimeFormat);
            var fullDate2 = dateTime.ToString("G", CultureInfo.InvariantCulture.DateTimeFormat);
            var timeFirst = "14:21:01 01/13/09";

            DateTimeFormatGuesser.TryGuessDateTimeFormat(shortDate, out formatDate);
            DateTimeFormatGuesser.TryGuessDateTimeFormat(fullDate, out formatFull);
            DateTimeFormatGuesser.TryGuessDateTimeFormat(fullDate2, out formatFull2);
            DateTimeFormatGuesser.TryGuessDateTimeFormat(timeFirst, out formatTimeFirst);

            Assert.AreEqual("MM/dd/yyyy", formatDate);
            Assert.AreEqual("MM/dd/yyyy HH:mm", formatFull);
            Assert.AreEqual("MM/dd/yyyy HH:mm:ss", formatFull2);
            Assert.AreEqual("HH:mm:ss MM/dd/yy", formatTimeFirst);
        }

        [Test]
        public void GuessFormatWithAmPmDesignatorJira9054()
        {
            string format;
            Assert.IsTrue(DateTimeFormatGuesser.TryGuessDateTimeFormat("7/15/2013 11:47:00 AM", out format));
            Assert.AreEqual("M/d/yyyy hh:mm:ss tt", format);
        }

        [Test]
        public void GuessFormatFromAmbiguousListOfDateTimes()
        {
            string format;
            var dates = new[]
            {
                "7/5/2013 11:47:00 AM",
                "7/5/2013 11:47:00 PM",
                "13/5/2013 11:47:00 AM"
            };
            Assert.IsTrue(DateTimeFormatGuesser.TryGuessDateTimeFormat(dates, out format));
            Assert.AreEqual("d/M/yyyy hh:mm:ss tt", format);

            var moreDates = new[]
            {
                "1/5/2013 23:47:00",
                "1/5/2013 11:47:00",
                "1/13/2013 1:47:00"
            };
            Assert.IsTrue(DateTimeFormatGuesser.TryGuessDateTimeFormat(moreDates, out format));
            Assert.AreEqual("M/d/yyyy H:mm:ss", format);
        }
    }
}