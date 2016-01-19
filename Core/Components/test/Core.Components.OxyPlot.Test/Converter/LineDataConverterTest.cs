using System;
using System.Collections.ObjectModel;
using Core.Components.Charting.Data;
using Core.Components.Charting.TestUtil;
using Core.Components.OxyPlot.Converter;
using NUnit.Framework;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Test.Converter
{
    [TestFixture]
    public class LineDataConverterTest
    {
        [Test]
        public void DefaultConstructor_IsChartDataConverter()
        {
            // Call
            var converter = new LineDataConverter();

            // Assert
            Assert.IsInstanceOf<ChartDataConverter>(converter);
        }

        [Test]
        public void CanConvertSeries_PointData_ReturnTrue()
        {
            // Setup
            var converter = new LineDataConverter();
            var lineData = new LineData(new Collection<Tuple<double, double>>());

            // Call & Assert
            Assert.IsTrue(converter.CanConvertSeries(lineData));
        }

        [Test]
        public void CanConvertSeries_ChartData_ReturnsFalse()
        {
            // Setup
            var converter = new LineDataConverter();
            var chartData = new TestChartData();

            // Call & Assert
            Assert.IsFalse(converter.CanConvertSeries(chartData));
        }

        [Test]
        public void Convert_RandomPointData_ReturnsNewSeries()
        {
            // Setup
            var converter = new LineDataConverter();
            var random = new Random(21);
            var randomCount = random.Next(5, 10);
            var points = new Collection<Tuple<double, double>>();

            for (int i = 0; i < randomCount; i++)
            {
                points.Add(new Tuple<double, double>(random.NextDouble(), random.NextDouble()));
            }

            var lineData = new LineData(points);

            // Call
            var series = converter.Convert(lineData);

            // Assert
            Assert.IsInstanceOf<LineSeries>(series);
            var lineSeries = ((LineSeries)series);
            CollectionAssert.AreEqual(points, lineSeries.ItemsSource);
            Assert.AreNotSame(points, lineSeries.ItemsSource);
        }
    }
}