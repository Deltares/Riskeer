using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core.Components.Charting.Data;
using Core.Components.Charting.TestUtil;
using Core.Components.OxyPlot.Converter;
using NUnit.Framework;
using OxyPlot;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Test.Converter
{
    [TestFixture]
    public class ChartSeriesFactoryTest
    {
        [Test]
        public void Create_AreaData_ReturnsAreaSeries()
        {
            // Setup
            var factory = new ChartSeriesFactory();
            var testData = CreateTestData();
            var expectedData = CreateExpectedData(testData);

            // Call
            IList<Series> series = factory.Create(new ChartAreaData(testData, "test data"));

            // Assert
            Assert.AreEqual(1, series.Count);
            Assert.IsInstanceOf<IList<Series>>(series);
            var areaSeries = ((AreaSeries)series[0]);
            CollectionAssert.AreEqual(expectedData, areaSeries.Points);
            CollectionAssert.AreEqual(new Collection<DataPoint>{expectedData.First()}, areaSeries.Points2);
            Assert.AreNotSame(expectedData, areaSeries.ItemsSource);
        }

        [Test]
        public void Create_LineData_ReturnsLineSeries()
        {
            // Setup
            var factory = new ChartSeriesFactory();
            var testData = CreateTestData();

            // Call
            IList<Series> series = factory.Create(new ChartLineData(testData, "test data"));

            // Assert
            Assert.AreEqual(1, series.Count);
            Assert.IsInstanceOf<IList<Series>>(series);
            var lineSeries = ((LineSeries)series[0]);
            CollectionAssert.AreEqual(testData, lineSeries.ItemsSource);
            Assert.AreNotSame(testData, lineSeries.ItemsSource);
        }

        [Test]
        public void Create_PointData_ReturnsLinesSeriesWithPointStyle()
        {
            // Setup
            var factory = new ChartSeriesFactory();
            var testData = CreateTestData();

            // Call
            IList<Series> series = factory.Create(new ChartPointData(testData, "test data"));

            // Assert
            Assert.AreEqual(1, series.Count);
            Assert.IsInstanceOf<IList<Series>>(series);
            var lineSeries = ((LineSeries)series[0]);
            CollectionAssert.AreEqual(testData, lineSeries.ItemsSource);
            Assert.AreNotSame(testData, lineSeries.ItemsSource);
            Assert.AreEqual(LineStyle.None, lineSeries.LineStyle);
            Assert.AreEqual(MarkerType.Circle, lineSeries.MarkerType);
        }

        [Test]
        public void Create_OtherData_ThrowsNotSupportedException()
        {
            // Setup
            var factory = new ChartSeriesFactory();
            var testData = new TestChartData("test data");

            // Call
            TestDelegate test = () => factory.Create(testData);

            // Assert
            Assert.Throws<NotSupportedException>(test);
        }

        private static ICollection<DataPoint> CreateExpectedData(IEnumerable<Tuple<double, double>> testData)
        {
            return testData.Select(p => new DataPoint(p.Item1, p.Item2)).ToArray();
        }

        private static Collection<Tuple<double, double>> CreateTestData()
        {
            return new Collection<Tuple<double, double>>
            {
                Tuple.Create(1.2, 3.4),
                Tuple.Create(3.2, 3.4),
                Tuple.Create(0.2, 2.4)
            };
        }
    }
}