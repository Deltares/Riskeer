using DelftTools.Controls.Swf.Charting.Series;
using NUnit.Framework;

namespace DelftTools.Tests.Controls.Swf.Charting.Series
{
    [TestFixture]
    public class AreaChartSeriesTest
    {
        [Test]
        public void TestMaximumValueClipping()
        {
            const int maximum = 999999;
            var series = new AreaChartSeries
            {
                LineWidth = maximum,
                PointerSize = maximum,
            };
            Assert.AreEqual(maximum, series.LineWidth);
            Assert.AreEqual(maximum, series.PointerSize);

            series.LineWidth = maximum + 1;
            series.PointerSize = maximum + 1;

            Assert.AreEqual(maximum, series.LineWidth);
            Assert.AreEqual(maximum, series.PointerSize);
        }

        [Test]
        public void TestMinimumValueClipping()
        {
            const int minimum = 0;
            var series = new AreaChartSeries
            {
                LineWidth = minimum,
                PointerSize = minimum,
            };
            Assert.AreEqual(minimum, series.LineWidth);
            Assert.AreEqual(minimum, series.PointerSize);

            series.LineWidth = minimum - 1;
            series.PointerSize = minimum - 1;

            Assert.AreEqual(minimum, series.LineWidth);
            Assert.AreEqual(minimum, series.PointerSize);
        }
    }
}