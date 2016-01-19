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
    public class SeriesFactoryTest
    {
        [Test]
        public void Create_AreaData_ReturnsAreaSeries()
        {
            // Setup
            var factory = new SeriesFactory();
            var testData = CreateTestData();
            var expectedData = CreateExpectedData(testData);

            // Call
            Series series = factory.Create(new AreaData(testData));

            // Assert
            Assert.IsInstanceOf<AreaSeries>(series);
            var areaSeries = ((AreaSeries)series);
            CollectionAssert.AreEqual(expectedData, areaSeries.Points);
            CollectionAssert.AreEqual(new Collection<DataPoint>{expectedData.First()}, areaSeries.Points2);
            Assert.AreNotSame(expectedData, areaSeries.ItemsSource);
        }

        [Test]
        public void Create_LineData_ReturnsLineSeries()
        {
            // Setup
            var factory = new SeriesFactory();
            var testData = CreateTestData();

            // Call
            Series series = factory.Create(new LineData(testData));

            // Assert
            Assert.IsInstanceOf<LineSeries>(series);
            var lineSeries = ((LineSeries)series);
            CollectionAssert.AreEqual(testData, lineSeries.ItemsSource);
            Assert.AreNotSame(testData, lineSeries.ItemsSource);
        }

        [Test]
        public void Create_PointData_ReturnsLinesSeriesWithPointStyle()
        {
            // Setup
            var factory = new SeriesFactory();
            var testData = CreateTestData();

            // Call
            Series series = factory.Create(new PointData(testData));

            // Assert
            Assert.IsInstanceOf<LineSeries>(series);
            var lineSeries = ((LineSeries)series);
            CollectionAssert.AreEqual(testData, lineSeries.ItemsSource);
            Assert.AreNotSame(testData, lineSeries.ItemsSource);
            Assert.AreEqual(LineStyle.None, lineSeries.LineStyle);
            Assert.AreEqual(MarkerType.Circle, lineSeries.MarkerType);
        }

        [Test]
        public void Create_OtherData_ThrowsNotSupportedException()
        {
            // Setup
            var factory = new SeriesFactory();
            var testData = new TestChartData();

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
                new Tuple<double, double>(1.2, 3.4),
                new Tuple<double, double>(3.2, 3.4),
                new Tuple<double, double>(0.2, 2.4)
            };
        }
    }
}