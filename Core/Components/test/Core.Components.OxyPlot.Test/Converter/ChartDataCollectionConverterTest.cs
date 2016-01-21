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
    public class ChartDataCollectionConverterTest
    {

        [Test]
        public void DefaultConstructor_IsChartDataConverter()
        {
            // Call
            var converter = new ChartDataCollectionConverter();

            // Assert
            Assert.IsInstanceOf<ChartDataConverter>(converter);
        }

        [Test]
        public void CanConvertSeries_ChartDataCollection_ReturnTrue()
        {
            // Setup
            var converter = new ChartDataCollectionConverter();
            var collectionData = new ChartDataCollection(new List<ChartData>());

            // Call & Assert
            Assert.IsTrue(converter.CanConvertSeries(collectionData));
        }

        [Test]
        public void CanConvertSeries_ChartData_ReturnsFalse()
        {
            // Setup
            var converter = new ChartDataCollectionConverter();
            var chartData = new TestChartData();

            // Call & Assert
            Assert.IsFalse(converter.CanConvertSeries(chartData));
        }

        [Test]
        public void Convert_CollectionOfAreaDataAndLineData_ReturnsTwoNewSeries()
        {
            // Setup
            var converter = new ChartDataCollectionConverter();
            var random = new Random(21);
            var randomCount = random.Next(5, 10);
            var pointsArea = new Collection<Tuple<double, double>>();
            var pointsLine = new Collection<Tuple<double, double>>();

            for (int i = 0; i < randomCount; i++)
            {
                pointsArea.Add(new Tuple<double, double>(random.NextDouble(), random.NextDouble()));
                pointsLine.Add(new Tuple<double, double>(random.NextDouble(), random.NextDouble()));
            }

            var collectionData = new ChartDataCollection(new List<ChartData>());
            var areaData = new AreaData(pointsArea);
            var lineData = new LineData(pointsLine);

            collectionData.List.Add(areaData);
            collectionData.List.Add(lineData);

            // Call
            var series = converter.Convert(collectionData);

            // Assert
            Assert.IsInstanceOf<IList<Series>>(series);
            Assert.AreEqual(2, series.Count);
            Assert.IsInstanceOf<AreaSeries>(series[0]);
            Assert.IsInstanceOf<LineSeries>(series[1]);
        }
         
    }
}