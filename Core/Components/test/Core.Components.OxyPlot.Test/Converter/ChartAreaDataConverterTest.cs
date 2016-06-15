using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core.Common.TestUtil;
using Core.Components.Charting.Data;
using Core.Components.Charting.TestUtil;
using Core.Components.OxyPlot.Converter;
using NUnit.Framework;
using OxyPlot;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Test.Converter
{
    [TestFixture]
    public class ChartAreaDataConverterTest
    {
        [Test]
        public void DefaultConstructor_IsChartDataConverter()
        {
            // Call
            var converter = new ChartAreaDataConverter();

            // Assert
            Assert.IsInstanceOf<ChartDataConverter<ChartAreaData>>(converter);
        }

        [Test]
        public void CanConvertSeries_AreaData_ReturnTrue()
        {
            // Setup
            var converter = new ChartAreaDataConverter();
            var areaData = new ChartAreaData(new Collection<Tuple<double, double>>(), "test data");

            // Call
            var canConvert = converter.CanConvertSeries(areaData);

            // Assert
            Assert.IsTrue(canConvert);
        }

        [Test]
        public void CanConvertSeries_ChartData_ReturnsFalse()
        {
            // Setup
            var converter = new ChartAreaDataConverter();
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
            var converter = new ChartAreaDataConverter();
            var random = new Random(21);
            var randomCount = random.Next(5, 10);
            var points = new Collection<Tuple<double, double>>();

            for (int i = 0; i < randomCount; i++)
            {
                points.Add(Tuple.Create(random.NextDouble(), random.NextDouble()));
            }

            var areaData = new ChartAreaData(points, "test data");

            // Call
            var series = converter.Convert(areaData);

            // Assert
            Assert.IsInstanceOf<IList<Series>>(series);
            var areaSeries = ((AreaSeries)series[0]);
            var expectedData = points.Select(t => new DataPoint(t.Item1, t.Item2)).ToArray();
            CollectionAssert.AreEqual(expectedData, areaSeries.Points);
            CollectionAssert.AreEqual(new Collection<DataPoint> { expectedData.First() }, areaSeries.Points2);
        }

        [Test]
        public void Convert_DataNull_ThrowsArgumentNullException()
        {
            // Setup
            var testConverter = new ChartAreaDataConverter();

            // Call
            TestDelegate test = () => testConverter.Convert(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Convert_DataCannotBeConverted_ThrowsArgumentException()
        {
            // Setup
            var testConverter = new ChartAreaDataConverter();
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