using Core.Common.Controls.Swf.Charting.Series;
using NUnit.Framework;

namespace Core.Common.Base.Tests.Controls.Swf.Charting.Series
{
    [TestFixture]
    public class PointChartSeriesTest
    {
        [Test]
        public void TestMaximumValueClipping()
        {
            const int maximum = 999999;
            var series = new PointChartSeries
            {
                Size = maximum
            };
            Assert.AreEqual(maximum, series.Size);

            series.Size = maximum + 1;

            Assert.AreEqual(maximum, series.Size);
        }

        [Test]
        public void TestMinimumValueClipping()
        {
            const int minimum = 0;
            var series = new PointChartSeries
            {
                Size = minimum
            };
            Assert.AreEqual(minimum, series.Size);

            series.Size = minimum - 1;

            Assert.AreEqual(minimum, series.Size);
        }
    }
}