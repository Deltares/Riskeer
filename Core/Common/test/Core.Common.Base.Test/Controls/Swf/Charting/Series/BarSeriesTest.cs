using Core.Common.Controls.Swf.Charting.Series;
using NUnit.Framework;

namespace Core.Common.Base.Tests.Controls.Swf.Charting.Series
{
    [TestFixture]
    public class BarSeriesTest
    {
        [Test]
        public void TestMaximumValueClipping()
        {
            const int maximum = 999999;
            var series = new BarSeries
            {
                LineWidth = maximum
            };
            Assert.AreEqual(maximum, series.LineWidth);

            series.LineWidth = maximum + 1;

            Assert.AreEqual(maximum, series.LineWidth);
        }

        [Test]
        public void TestMinimumValueClipping()
        {
            const int minimum = 0;
            var series = new BarSeries
            {
                LineWidth = minimum,
            };
            Assert.AreEqual(minimum, series.LineWidth);

            series.LineWidth = minimum - 1;

            Assert.AreEqual(minimum, series.LineWidth);
        }
    }
}