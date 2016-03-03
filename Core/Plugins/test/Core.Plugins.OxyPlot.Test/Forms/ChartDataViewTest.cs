using System;
using System.Linq;
using System.Windows.Forms;
using Core.Components.Charting.Data;
using Core.Components.OxyPlot.Forms;
using Core.Plugins.OxyPlot.Forms;
using NUnit.Framework;

namespace Core.Plugins.OxyPlot.Test.Forms
{
    [TestFixture]
    public class ChartDataViewTest
    {
        [Test]
        public void DefaultConstructor_Always_AddsChartControl()
        {
            // Call
            var chartView = new ChartDataView();

            // Assert
            Assert.AreEqual(1, chartView.Controls.Count);
            object chartObject = chartView.Controls[0];
            Assert.IsInstanceOf<ChartControl>(chartObject);

            var chart = (ChartControl)chartObject;
            Assert.AreEqual(DockStyle.Fill, chart.Dock);
            Assert.NotNull(chartView.Chart);
        }

        [Test]
        public void Data_SetToNull_ChartControlNoSeries()
        {
            // Setup
            var chartView = new ChartDataView();
            var chart = (ChartControl)chartView.Controls[0];

            // Call
            chartView.Data = null;

            // Assert
            Assert.IsNull(chart.Data);
        }

        [Test]
        public void Data_SetToObject_InvalidCastException()
        {
            // Setup
            var chartView = new ChartDataView();

            // Call
            TestDelegate test = () => chartView.Data = new object();

            // Assert
            Assert.Throws<InvalidCastException>(test);
        }

        [Test]
        public void Data_SetToLineData_ChartDataSet()
        {
            // Setup
            var chartView = new ChartDataView();
            var chart = (ChartControl)chartView.Controls[0];
            var lineData = new LineData(Enumerable.Empty<Tuple<double, double>>());

            // Call
            chartView.Data = lineData;

            // Assert
            Assert.AreSame(lineData, chart.Data);
            Assert.AreSame(lineData, chartView.Data);
        }

        [Test]
        public void Data_SetToPointData_ChartDataSet()
        {
            // Setup
            var chartView = new ChartDataView();
            var chart = (ChartControl)chartView.Controls[0];
            var pointData = new PointData(Enumerable.Empty<Tuple<double, double>>());

            // Call
            chartView.Data = pointData;

            // Assert
            Assert.AreSame(pointData, chart.Data);
            Assert.AreSame(pointData, chartView.Data);
        }

        [Test]
        public void Data_SetToAreaData_ChartDataSet()
        {
            // Setup
            var chartView = new ChartDataView();
            var chart = (ChartControl)chartView.Controls[0];
            var areaData = new AreaData(Enumerable.Empty<Tuple<double, double>>());

            // Call
            chartView.Data = areaData;

            // Assert
            Assert.AreSame(areaData, chart.Data);
            Assert.AreSame(areaData, chartView.Data);
        }

        [Test]
        public void Data_SetToCollectionChartData_ChartDataSet()
        {
            // Setup
            var chartView = new ChartDataView();
            var chart = (ChartControl)chartView.Controls[0];
            var chartDataCollection = new ChartDataCollection(new ChartData[0]);

            // Call
            chartView.Data = chartDataCollection;

            // Assert
            Assert.AreSame(chartDataCollection, chart.Data);
            Assert.AreSame(chartDataCollection, chartView.Data);
        }
    }
}