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
            using (var chartView = new ChartDataView())
            {
                // Assert
                Assert.AreEqual(1, chartView.Controls.Count);
                object chartObject = chartView.Controls[0];
                Assert.IsInstanceOf<ChartControl>(chartObject);

                var chart = (ChartControl) chartObject;
                Assert.AreEqual(DockStyle.Fill, chart.Dock);
                Assert.NotNull(chartView.Chart);
            }
        }

        [Test]
        public void Data_SetToNull_ChartControlNotUpdated()
        {
            // Setup
            using (var chartView = new ChartDataView())
            {
                ChartControl chart = (ChartControl) chartView.Controls[0];
                ChartDataCollection chartData = chart.Data;

                // Call
                TestDelegate testDelegate = () => chartView.Data = null;

                // Assert
                Assert.DoesNotThrow(testDelegate);
                Assert.AreSame(chartData, chart.Data);
            }
        }

        [Test]
        public void Data_SetToObject_InvalidCastException()
        {
            // Setup
            using (var chartView = new ChartDataView())
            {
                // Call
                TestDelegate test = () => chartView.Data = new object();

                // Assert
                Assert.Throws<InvalidCastException>(test);
            }
        }

        [Test]
        public void Data_SetToCollectionChartData_ChartDataSet()
        {
            // Setup
            using (var chartView = new ChartDataView())
            {
                var chart = (ChartControl) chartView.Controls[0];
                var pointData = new ChartPointData(Enumerable.Empty<Tuple<double, double>>(), "test");
                var chartDataCollection = new ChartDataCollection(new ChartData[]
                {
                    pointData
                }, "test data");

                // Call
                chartView.Data = chartDataCollection;

                // Assert
                Assert.AreSame(pointData, chart.Data.List.First());
                Assert.AreSame(chartDataCollection, chartView.Data);
            }
        }
    }
}