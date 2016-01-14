using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using Core.Components.Charting.Data;
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
        public void Data_SetToNull_RemovesSeriesFromModel()
        {
            // Setup
            var mocks = new MockRepository();
            var chart = new BaseChart();
            var dataMock = new PointData(new Collection<Tuple<double, double>>());
            mocks.ReplayAll();

            chart.Data = new [] { dataMock };

            // Call
            chart.Data = null;
            
            // Assert
            Assert.IsEmpty(chart.Data);
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
            chart.Data = new IChartData[] { pointData, lineData, areaData };
            // Assert
            CollectionAssert.AreEqual(new IChartData[] {pointData, lineData, areaData}, chart.Data);
        }
    }
}