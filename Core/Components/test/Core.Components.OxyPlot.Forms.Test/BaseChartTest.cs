using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using Core.Components.Charting.Data;
using Core.Components.Charting.TestUtil;
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
        public void Data_SetToNull_EmptyData()
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
        public void Data_NotNull_DataSet()
        {
            // Setup
            var chart = new BaseChart();
            var pointData = new PointData(new Collection<Tuple<double,double>>());
            var lineData = new LineData(new Collection<Tuple<double,double>>());
            var areaData = new AreaData(new Collection<Tuple<double,double>>());

            // Call
            chart.Data = new ChartData[] { pointData, lineData, areaData };

            // Assert
            CollectionAssert.AreEqual(new ChartData[] {pointData, lineData, areaData}, chart.Data);
        }

        [Test]
        public void Data_NotKnownChartData_ThrowsNotSupportedException()
        {
            // Setup
            var chart = new BaseChart();
            var testData = new TestChartData();

            // Call
            TestDelegate test = () => chart.Data = new ChartData[] { testData };

            // Assert
            Assert.Throws<NotSupportedException>(test);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void SetVisibility_ForContainingData_SetsDataVisibility(bool visibility)
        {
            // Setup
            var chart = new BaseChart();
            var pointData = new PointData(new Collection<Tuple<double, double>>());
            chart.Data = new ChartData[] { pointData };

            // Call
            chart.SetVisibility(pointData, visibility);

            // Assert
            Assert.AreEqual(visibility, pointData.IsVisible);
        }

        [Test]
        public void SetVisibility_ForNonContainingData_ThrowsKeyNotFoundException()
        {
            // Setup
            var chart = new BaseChart();
            var pointData = new PointData(new Collection<Tuple<double,double>>());

            // Call
            TestDelegate test = () => chart.SetVisibility(pointData, true);
            
            // Assert
            Assert.Throws<KeyNotFoundException>(test);
        }
    }
}