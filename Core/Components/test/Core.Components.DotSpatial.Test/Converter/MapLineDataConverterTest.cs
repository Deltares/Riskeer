﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core.Common.TestUtil;
using Core.Components.DotSpatial.Converter;
using Core.Components.DotSpatial.Data;
using Core.Components.DotSpatial.TestUtil;
using DotSpatial.Data;
using DotSpatial.Topology;
using NUnit.Framework;

namespace Core.Components.DotSpatial.Test.Converter
{
    [TestFixture]
    public class MapLineDataConverterTest
    {
        [Test]
        public void DefaultConstructor_IsChartDataConverter()
        {
            // Call
            var converter = new MapLineDataConverter();

            // Assert
            Assert.IsInstanceOf<MapDataConverter<MapLineData>>(converter);
        }

        [Test]
        public void CanConvertSeries_LineData_ReturnTrue()
        {
            // Setup
            var converter = new MapLineDataConverter();
            var lineData = new MapLineData(new Collection<Tuple<double, double>>());

            // Call
            var canConvert = converter.CanConvertMapData(lineData);

            // Assert
            Assert.IsTrue(canConvert);
        }

        [Test]
        public void CanConvertSeries_ChartData_ReturnsFalse()
        {
            // Setup
            var converter = new MapLineDataConverter();
            var mapData = new TestMapData();

            // Call
            var canConvert = converter.CanConvertMapData(mapData);

            // Assert
            Assert.IsFalse(canConvert);
        }

        [Test]
        public void Convert_DataNull_ThrowsArgumentNullException()
        {
            // Setup
            var testConverter = new MapLineDataConverter();

            // Call
            TestDelegate test = () => testConverter.Convert(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Convert_DataCannotBeConverted_ThrowsArgumentException()
        {
            // Setup
            var testConverter = new MapLineDataConverter();
            var testChartData = new TestMapData();
            var expectedMessage = string.Format("The data of type {0} cannot be converted by this converter.", testChartData.GetType());
            // Precondition
            Assert.IsFalse(testConverter.CanConvertMapData(testChartData));

            // Call
            TestDelegate test = () => testConverter.Convert(testChartData);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        public void Convert_RandomPointData_ReturnsNewFeatureSetList()
        {
            // Setup
            var converter = new MapLineDataConverter();
            var random = new Random(21);
            var randomCount = random.Next(5, 10);
            var points = new Collection<Tuple<double, double>>();

            for (int i = 0; i < randomCount; i++)
            {
                points.Add(new Tuple<double, double>(random.NextDouble(), random.NextDouble()));
            }

            var lineData = new MapLineData(points);

            // Call
            var featureSets = converter.Convert(lineData);

            // Assert
            Assert.IsInstanceOf<IList<FeatureSet>>(featureSets);
            var featureSet = featureSets[0];
            Assert.AreEqual(1, featureSet.Features.Count);
            Assert.IsInstanceOf<FeatureSet>(featureSet);
            Assert.AreEqual(FeatureType.Line, featureSet.FeatureType);
            CollectionAssert.AreNotEqual(lineData.Points, featureSet.Features[0].Coordinates);
        }
    }
}