using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using Core.Components.OxyPlot.Data;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Components.OxyPlot.Forms.Test
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
            Assert.IsInstanceOf<Control>(chart);
            Assert.AreEqual(75, chart.MinimumSize.Height);
            Assert.AreEqual(50, chart.MinimumSize.Width);
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
        public void ClearData_Always_RemovesSeriesFromModel()
        {
            // Setup
            var mocks = new MockRepository();
            var chart = new BaseChart();
            var dataMock = new PointData(new Collection<Tuple<double, double>>());
            mocks.ReplayAll();

            chart.AddData(dataMock);

            // Call
            chart.ClearData();
            
            // Assert
            Assert.IsEmpty(chart.Series);
            mocks.VerifyAll();
        }

        [Test]
        public void AddData_Always_AddsToSeries()
        {
            // Setup
            var chart = new BaseChart();
            var pointData = new PointData(new Collection<Tuple<double,double>>());
            var lineData = new LineData(new Collection<Tuple<double,double>>());
            var areaData = new AreaData(new Collection<Tuple<double,double>>());

            // Call
            chart.AddData(pointData);
            chart.AddData(lineData);
            chart.AddData(areaData);

            // Assert
            CollectionAssert.AreEqual(new IChartData[] {pointData, lineData, areaData}, chart.Series);
        }
    }
}