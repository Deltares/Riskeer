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
    public class AreaDataConverterTest
    {

        [Test]
        public void DefaultConstructor_IsChartDataConverter()
        {
            // Call
            var converter = new AreaDataConverter();

            // Assert
            Assert.IsInstanceOf<ChartDataConverter>(converter);
        }

        [Test]
        public void CanConvertSeries_PointData_ReturnTrue()
        {
            // Setup
            var converter = new AreaDataConverter();
            var areaData = new AreaData(new Collection<Tuple<double, double>>());

            // Call & Assert
            Assert.IsTrue(converter.CanConvertSeries(areaData));
        }

        [Test]
        public void CanConvertSeries_ChartData_ReturnsFalse()
        {
            // Setup
            var converter = new AreaDataConverter();
            var chartData = new TestChartData();

            // Call & Assert
            Assert.IsFalse(converter.CanConvertSeries(chartData));
        }

        [Test]
        public void Convert_RandomPointData_ReturnsNewSeries()
        {
            // Setup
            var converter = new AreaDataConverter();
            var random = new Random(21);
            var randomCount = random.Next(5, 10);
            var points = new Collection<Tuple<double, double>>();

            for (int i = 0; i < randomCount; i++)
            {
                points.Add(new Tuple<double, double>(random.NextDouble(), random.NextDouble()));
            }

            var areaData = new AreaData(points);

            // Call
            var series = converter.Convert(areaData);

            // Assert
            Assert.IsInstanceOf<IList<Series>>(series);
            var areaSeries = ((AreaSeries)series[0]);
            var expectedData = points.Select(t => new DataPoint(t.Item1, t.Item2)).ToArray();
            CollectionAssert.AreEqual(expectedData, areaSeries.Points);
            CollectionAssert.AreEqual(new Collection<DataPoint> { expectedData.First() }, areaSeries.Points2);
        }
    }
}