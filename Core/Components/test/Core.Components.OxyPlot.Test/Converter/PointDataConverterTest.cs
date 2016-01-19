using System;
using System.Collections.ObjectModel;
using Core.Components.Charting.Data;
using Core.Components.Charting.TestUtil;
using Core.Components.OxyPlot.Converter;
using NUnit.Framework;
using OxyPlot;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Test.Converter
{
    [TestFixture]
    public class PointDataConverterTest
    {
        [Test]
        public void DefaultConstructor_IsChartDataConverter()
        {
            // Call
            var converter = new PointDataConverter();

            // Assert
            Assert.IsInstanceOf<ChartDataConverter>(converter);
        }

        [Test]
        public void CanConvertSeries_PointData_ReturnTrue()
        {
            // Setup
            var converter = new PointDataConverter();
            var pointData = new PointData(new Collection<Tuple<double, double>>());

            // Call & Assert
            Assert.IsTrue(converter.CanConvertSeries(pointData));
        }

        [Test]
        public void CanConvertSeries_ChartData_ReturnsFalse()
        {
            // Setup
            var converter = new PointDataConverter();
            var chartData = new TestChartData();

            // Call & Assert
            Assert.IsFalse(converter.CanConvertSeries(chartData));
        }

        [Test]
        public void Convert_RandomPointData_ReturnsNewSeries()
        {
            // Setup
            var converter = new PointDataConverter();
            var random = new Random(21);
            var randomCount = random.Next(5, 10);
            var points = new Collection<Tuple<double, double>>();

            for (int i = 0; i < randomCount; i++)
            {
                points.Add(new Tuple<double, double>(random.NextDouble(), random.NextDouble()));
            }

            var pointData = new PointData(points);

            // Call
            var series = converter.Convert(pointData);

            // Assert
            Assert.IsInstanceOf<LineSeries>(series);
            var lineSeries = ((LineSeries)series);
            CollectionAssert.AreEqual(points, lineSeries.ItemsSource);
            Assert.AreNotSame(points, lineSeries.ItemsSource);
            Assert.AreEqual(LineStyle.None, lineSeries.LineStyle);
            Assert.AreEqual(MarkerType.Circle, lineSeries.MarkerType);
        }
    }
}