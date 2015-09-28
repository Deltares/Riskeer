using System;
using DelftTools.Controls.Swf.Charting;
using NUnit.Framework;

namespace DelftTools.Tests.Controls.Swf.Charting
{
    [TestFixture]
    public class YearViewDateTimeFormatProviderTest
    {
        private static readonly YearNavigatableLabelFormatProvider Provider = new YearNavigatableLabelFormatProvider();
        [Test]
        public void GetLabel()
        {
            Assert.AreEqual("1991",Provider.GetLabel(new DateTime(1991,11,11),TimeSpan.Zero));
        }


        [Test]
        public void GetAxisAnnotation()
        {
            Assert.AreEqual("1991 till 2010", Provider.GetRangeLabel(new DateTime(1991, 11, 11), new DateTime(2010,1,1)));
        }
    }
}