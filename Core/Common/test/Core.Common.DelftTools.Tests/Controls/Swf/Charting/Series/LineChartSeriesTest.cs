using Core.Common.Controls.Swf.Charting.Series;
using NUnit.Framework;

namespace Core.Common.DelftTools.Tests.Controls.Swf.Charting.Series
{
    [TestFixture]
    public class LineChartSeriesTest
    {
        [Test]
        public void TestMaximumValueClipping()
        {
            const int maximum = 999999;
            var series = new LineChartSeries
            {
                Width = maximum,
                PointerSize = maximum,
            };
            Assert.AreEqual(maximum, series.Width);
            Assert.AreEqual(maximum, series.PointerSize);

            series.Width = maximum + 1;
            series.PointerSize = maximum + 1;

            Assert.AreEqual(maximum, series.Width);
            Assert.AreEqual(maximum, series.PointerSize);
        }

        [Test]
        public void TestMinimumValueClipping()
        {
            const int minimum = 0;
            var series = new LineChartSeries
            {
                Width = minimum,
                PointerSize = minimum,
            };
            Assert.AreEqual(minimum, series.Width);
            Assert.AreEqual(minimum, series.PointerSize);

            series.Width = minimum - 1;
            series.PointerSize = minimum - 1;

            Assert.AreEqual(minimum, series.Width);
            Assert.AreEqual(minimum, series.PointerSize);
        }
    }
}