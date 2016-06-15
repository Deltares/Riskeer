using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core.Common.TestUtil;
using Core.Components.Charting.Data;
using Core.Components.Charting.TestUtil;
using Core.Components.OxyPlot.Converter;
using NUnit.Framework;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Test.Converter
{
    [TestFixture]
    public class ChartLineDataConverterTest
    {
        [Test]
        public void DefaultConstructor_IsChartDataConverter()
        {
            // Call
            var converter = new ChartLineDataConverter();

            // Assert
            Assert.IsInstanceOf<ChartDataConverter<ChartLineData>>(converter);
        }

        [Test]
        public void CanConvertSeries_LineData_ReturnTrue()
        {
            // Setup
            var converter = new ChartLineDataConverter();
            var lineData = new ChartLineData(new Collection<Tuple<double, double>>(), "test data");

            // Call
            var canConvert = converter.CanConvertSeries(lineData);

            // Assert
            Assert.IsTrue(canConvert);
        }

        [Test]
        public void CanConvertSeries_ChartData_ReturnsFalse()
        {
            // Setup
            var converter = new ChartLineDataConverter();
            var chartData = new TestChartData("test data");

            // Call
            var canConvert = converter.CanConvertSeries(chartData);

            // Assert
            Assert.IsFalse(canConvert);
        }

        [Test]
        public void Convert_RandomPointData_ReturnsNewSeries()
        {
            // Setup
            var converter = new ChartLineDataConverter();
            var random = new Random(21);
            var randomCount = random.Next(5, 10);
            var points = new Collection<Tuple<double, double>>();

            for (int i = 0; i < randomCount; i++)
            {
                points.Add(Tuple.Create(random.NextDouble(), random.NextDouble()));
            }

            var lineData = new ChartLineData(points, "test data");

            // Call
            var series = converter.Convert(lineData);

            // Assert
            Assert.IsInstanceOf<IList<Series>>(series);
            var lineSeries = ((LineSeries)series[0]);
            CollectionAssert.AreEqual(points, lineSeries.ItemsSource);
            Assert.AreNotSame(points, lineSeries.ItemsSource);
        }

        [Test]
        public void Convert_DataNull_ThrowsArgumentNullException()
        {
            // Setup
            var testConverter = new ChartLineDataConverter();

            // Call
            TestDelegate test = () => testConverter.Convert(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Convert_DataCannotBeConverted_ThrowsArgumentException()
        {
            // Setup
            var testConverter = new ChartLineDataConverter();
            var testChartData = new TestChartData("test data");
            var expectedMessage = string.Format("The data of type {0} cannot be converted by this converter.", testChartData.GetType());
            // Precondition
            Assert.IsFalse(testConverter.CanConvertSeries(testChartData));

            // Call
            TestDelegate test = () => testConverter.Convert(testChartData);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }
    }
}