using System;
using NUnit.Framework;

namespace DelftTools.Utils.Tests
{
    [TestFixture]
    public class DeltaShellTimeSpanConverterTest
    {
        [Test]
        public void ConvertToAndFrom()
        {
            var timeSpan = new TimeSpan(40, 1, 1, 1);
            var theString = new DeltaShellTimeSpanConverter().ConvertTo(timeSpan, typeof(string));
            Assert.AreEqual("40d 01:01:01", theString);
            var timeSpanRetrieved = (TimeSpan) new DeltaShellTimeSpanConverter().ConvertFrom(theString);
            Assert.AreEqual(timeSpan, timeSpanRetrieved);
        }
    }
}