using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using Core.Components.OxyPlot.Data;
using NUnit.Framework;
using OxyPlot.Series;
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
            var dataMock = new TestChartData(mocks.Stub<Series>());
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
            // Given
            var mocks = new MockRepository();
            var chart = new BaseChart();
            var dataMock = new TestChartData(mocks.Stub<Series>());

            // When
            chart.AddData(dataMock);

            // Then
            Assert.IsInstanceOf<TestChartData>(chart.Series.First());
        }
    }

    public class TestChartData : ISeries {
        private Series series;

        public TestChartData(Series series)
        {
            this.series = series;
        }

        public bool IsVisible { get; set; }

        public Series Series
        {
            get
            {
                return series;
            }
        }
    }
}