using NUnit.Framework;
using OxyPlot;
using OxyPlot.WindowsForms;

namespace Core.Components.OxyPlot.Test
{
    [TestFixture]
    public class BaseChartTest
    {
        [Test]
        public void DefaultConstructor_PropertiesSet()
        {
            // Call
            var chart = new BaseChart();

            // Assert
            Assert.IsInstanceOf<PlotView>(chart);
            Assert.IsInstanceOf<PlotModel>(chart.Model);
            Assert.IsNull(chart.Controller);
            Assert.AreEqual(2, chart.Model.Axes.Count);
        }
    }
}