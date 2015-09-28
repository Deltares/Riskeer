using System;
using System.Linq;
using NUnit.Framework;

namespace DelftTools.Utils.Tests
{
    [TestFixture]
    public class SnappingCalculatorTest
    {
        [Test]
        public void GetLastTimeInRangeTakesRangeIntoAccount()
        {
            //year 2000,2001,2002,2003,2004
            var times = Enumerable.Range(2000, 5).Select(i => new DateTime(i, 1, 1)).ToArray();

            //selected nearest value for 31/12/2003...should be 1/1/2003 iso 1/1/2004 because 1/1/2004 is not in range
            Assert.AreEqual(new DateTime(2003,1,1),SnappingCalculator.GetLastTimeInRange(times,new DateTime(2000,1,1), new DateTime(2003,12,31)));
        }

        [Test]
        public void GetLastTimeInRangeReturnsNullIfNothingIsInRange()
        {    //year 2000,2001,2002,2003,2004
            var times = Enumerable.Range(2000, 5).Select(i => new DateTime(i, 1, 1)).ToArray();

            //selected nearest value for 31/12/2003...should be 1/1/2003 iso 1/1/2004 because 1/1/2004 is not in range
            Assert.IsNull(SnappingCalculator.GetLastTimeInRange(times, new DateTime(2010, 1, 1), new DateTime(2013, 12, 31)));
        }

        [Test]
        public void GetNearestDefinedTime()
        {
            //year 2000,2001,2002,2003,2004
            var times = Enumerable.Range(2000, 5).Select(i => new DateTime(i, 1, 1)).ToArray();
            Assert.AreEqual(new DateTime(2002,1,1),SnappingCalculator.GetNearestDefinedTime(times,new DateTime(2001,10,22)));
        }

        [Test]
        public void GetIntervalTime()
        {
            //year 2000,2001,2002,2003,2004
            var times = Enumerable.Range(2000, 5).Select(i => new DateTime(i, 1, 1)).ToArray();
            Assert.AreEqual(new DateTime(2001, 1, 1), SnappingCalculator.GetFirstTimeLeftOfValue(times, new DateTime(2001, 11, 22)));
        }

        [Test]
        public void GetIntervalTimeReturnsNullIfNothingIsInInterval()
        {
            //year 2000,2001,2002,2003,2004
            var times = Enumerable.Range(2000, 5).Select(i => new DateTime(i, 1, 1)).ToArray();
            Assert.AreEqual(null, SnappingCalculator.GetFirstTimeLeftOfValue(times, new DateTime(1999, 11, 22)));
        }
    }
}