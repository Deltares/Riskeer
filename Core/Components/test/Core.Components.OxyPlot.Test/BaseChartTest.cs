using System;
using Core.Components.OxyPlot.Data;
using NUnit.Framework;
using OxyPlot;
using OxyPlot.WindowsForms;
using Rhino.Mocks;

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

        [Test]
        public void AddData_WithoutData_ThrowsArgumentNullException()
        {
            // Setup
            var chart = new BaseChart();
            
            // Call
            TestDelegate test = () => chart.AddData(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void AddData_WithIChartData_Added()
        {
            // Setup
            var mocks = new MockRepository();
            var chart = new BaseChart();
            var dataMock = mocks.StrictMock<IChartData>();
            dataMock.Expect(d => d.AddTo(chart.Model));

            mocks.ReplayAll();

            // Call
            chart.AddData(dataMock);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void ClearData_Always_RemovesSeriesFromModel()
        {
            // Setup
            var mocks = new MockRepository();
            var chart = new BaseChart();
            var dataMock = mocks.StrictMock<IChartData>();
            dataMock.Expect(d => d.AddTo(chart.Model));
            mocks.ReplayAll();

            chart.AddData(dataMock);

            // Call
            chart.ClearData();
            
            // Assert
            Assert.IsEmpty(chart.Model.Series);
            mocks.VerifyAll();
        }
    }
}