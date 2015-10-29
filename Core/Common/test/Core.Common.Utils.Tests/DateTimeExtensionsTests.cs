using System;
using NUnit.Framework;

namespace Core.Common.Utils.Tests
{
    [TestFixture]
    public class DateTimeExtensionsTests
    {
        [Test]
        public void ToJulianDate()
        {
            var date = new DateTime(2000, 1, 1);
            Assert.AreEqual(2451544.5d, date.ToJulianDate());
        }

        [Test]
        public void ToModifiedJulianDay()
        {
            var date = new DateTime(2000, 1, 1);
            Assert.AreEqual(51544, date.ToModifiedJulianDay());
        }

        [Test]
        public void ToDecimal()
        {
            var date = new DateTime(1980, 1, 1);
            Assert.AreEqual(1980.0, date.ToDecimalYear());

            var date2 = new DateTime(1980, 12, 31);
            Assert.AreEqual(1980.99, date2.ToDecimalYear(), 0.01);
        }
    }
}