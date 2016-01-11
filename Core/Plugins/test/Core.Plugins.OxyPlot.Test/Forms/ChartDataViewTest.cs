using System;
using System.Windows.Forms;
using Core.Components.OxyPlot.Data;
using Core.Components.OxyPlot.Forms;
using Core.Plugins.OxyPlot.Forms;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.OxyPlot.Test.Forms
{
    [TestFixture]
    public class ChartDataViewTest
    {
        [Test]
        public void DefaultConstructor_Always_AddsBaseChart()
        {
            // Call
            var chartView = new ChartDataView();

            // Assert
            Assert.AreEqual(1, chartView.Controls.Count);
            object chartObject = chartView.Controls[0];
            Assert.IsInstanceOf<BaseChart>(chartObject);

            var chart = (BaseChart)chartObject;
            Assert.AreEqual(DockStyle.Fill, chart.Dock);
            Assert.NotNull(chartView.Chart);
        }

        [Test]
        public void Data_SetToNull_BaseChartNoSeries()
        {
            // Setup
            var chartView = new ChartDataView();
            var chart = (BaseChart)chartView.Controls[0];

            // Call
            chartView.Data = null;

            // Assert
            Assert.IsEmpty(chart.Model.Series);
        }

        [Test]
        public void Data_SetToIChartData_ChartDataAdded()
        {
            // Setup
            var mocks = new MockRepository();
            var chartView = new ChartDataView();
            var chart = (BaseChart)chartView.Controls[0];
            var chartData = mocks.StrictMock<IChartData>();
            chartData.Expect(d => d.AddTo(chart.Model));
            mocks.ReplayAll();

            // Call
            chartView.Data = chartData;

            // Assert
            Assert.AreSame(chartData, chartView.Data);
            mocks.VerifyAll();
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
    }
}