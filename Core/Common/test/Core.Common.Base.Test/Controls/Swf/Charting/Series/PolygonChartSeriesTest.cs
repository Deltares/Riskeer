using Core.Common.Controls.Swf.Charting.Series;
using NUnit.Framework;

namespace Core.Common.Base.Test.Controls.Swf.Charting.Series
{
    [TestFixture]
    public class PolygonChartSeriesTest
    {
        [Test]
        public void TestMaximumValueClipping()
        {
            const int maximum = 999999;
            var series = new PolygonChartSeries
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
            var series = new PolygonChartSeries
            {
                LineWidth = minimum
            };
            Assert.AreEqual(minimum, series.LineWidth);

            series.LineWidth = minimum - 1;

            Assert.AreEqual(minimum, series.LineWidth);
        }
    }
}