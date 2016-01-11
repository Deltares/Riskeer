using System;
using System.Collections.ObjectModel;
using System.Linq;
using Core.Components.OxyPlot.Data;
using NUnit.Framework;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
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
            Assert.IsInstanceOf<PlotView>(chart);
            Assert.IsInstanceOf<PlotModel>(chart.Model);
            Assert.IsNull(chart.Controller);
            Assert.AreEqual(2, chart.Model.Axes.Count);

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

        [Test]
        public void GivenBaseChart_WhenPointDataAdded_ThenSeriesHasPointStyle()
        {
            // Given
            var chart = new BaseChart();
            var pointData = new PointData(new Collection<Tuple<double, double>>());

            // When
            chart.AddData(pointData);

            // Then
            var pointSeries = (LineSeries)chart.Model.Series.First();
            Assert.AreEqual(LineStyle.None, pointSeries.LineStyle);
            Assert.AreEqual(MarkerType.Circle, pointSeries.MarkerType);
        }

        [Test]
        public void GivenBaseChart_WhenLineDataAdded_ThenSeriesIsLineSeries()
        {
            // Given
            var chart = new BaseChart();
            var pointData = new LineData(new Collection<Tuple<double, double>>());

            // When
            chart.AddData(pointData);

            // Then
            Assert.IsInstanceOf<LineSeries>(chart.Model.Series.First());
        }

        [Test]
        public void GivenBaseChart_WhenAreaDataAdded_ThenSeriesIsAreaSeries()
        {
            // Given
            var chart = new BaseChart();
            var pointData = new AreaData(new Collection<Tuple<double, double>>());

            // When
            chart.AddData(pointData);

            // Then
            Assert.IsInstanceOf<AreaSeries>(chart.Model.Series.First());
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(11)]
        public void GivenANumberOfChartData_WhenAddedToChart_AddsSameNumberOfSeriesToModel(int numberOfSeries)
        {
            // Given
            var chart = new BaseChart();
            var data = new CollectionData();
            for (int i = 0; i < numberOfSeries; i++)
            {
                data.Add(new LineData(new Collection<Tuple<double, double>>()));
            }

            // When
            chart.AddData(data);

            // Assert
            Assert.AreEqual(numberOfSeries, chart.Model.Series.Count);
        }
    }
}