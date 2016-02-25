﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Components.DotSpatial.Converter;
using Core.Components.DotSpatial.TestUtil;
using Core.Components.Gis.Data;
using DotSpatial.Controls;
using DotSpatial.Topology;
using NUnit.Framework;

namespace Core.Components.DotSpatial.Test.Converter
{
    [TestFixture]
    public class MapDataCollectionConverterTest
    {
        [Test]
        public void DefaultConstructor_IsMapDataConverter()
        {
            // Call
            var converter = new MapDataCollectionConverter();

            // Assert
            Assert.IsInstanceOf<MapDataConverter<MapDataCollection>>(converter);
        }

        [Test]
        public void CanConvertMapData_MapDataCollection_ReturnsTrue()
        {
            // Setup
            var converter = new MapDataCollectionConverter();
            var collectionData = new MapDataCollection(new List<MapData>());

            // Call
            var canConvert = converter.CanConvertMapData(collectionData);

            // Assert
            Assert.IsTrue(canConvert);
        }

        [Test]
        public void CanConvertMapData_MapData_ReturnsFalse()
        {
            // Setup
            var converter = new MapDataCollectionConverter();
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
            var testConverter = new MapDataCollectionConverter();

            // Call
            TestDelegate test = () => testConverter.Convert(null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, "Null data cannot be converted into feature sets.");
        }

        [Test]
        public void Convert_DataCannotBeConverted_ThrowsArgumentException()
        {
            // Setup
            var testConverter = new MapDataCollectionConverter();
            var testMapData = new TestMapData();
            var expectedMessage = string.Format("The data of type {0} cannot be converted by this converter.", testMapData.GetType());

            // Precondition
            Assert.IsFalse(testConverter.CanConvertMapData(testMapData));

            // Call
            TestDelegate test = () => testConverter.Convert(testMapData);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        public void Convert_CollectionOfPointAndLineData_ReturnsTwoNewIMapFeatureLayers()
        {
            // Setup
            var converter = new MapDataCollectionConverter();
            var random = new Random(21);
            var randomCount = random.Next(5, 10);
            var points = new Collection<Point2D>();
            var linePoints = new Collection<Point2D>();

            for (int i = 0; i < randomCount; i++)
            {
                points.Add(new Point2D(random.NextDouble(), random.NextDouble()));
                linePoints.Add(new Point2D(random.NextDouble(), random.NextDouble()));
            }

            var collectionData = new MapDataCollection(new List<MapData>());
            var pointData = new MapPointData(points);
            var lineData = new MapLineData(linePoints);

            collectionData.List.Add(pointData);
            collectionData.List.Add(lineData);

            // Call
            var layers = converter.Convert(collectionData);

            // Assert
            Assert.IsInstanceOf<IList<IMapFeatureLayer>>(layers);
            Assert.AreEqual(2, layers.Count);
            Assert.IsInstanceOf<MapPointLayer>(layers[0]);
            Assert.AreEqual(FeatureType.Point, layers[0].DataSet.FeatureType);            
            Assert.IsInstanceOf<MapLineLayer>(layers[1]);
            Assert.AreEqual(FeatureType.Line, layers[1].DataSet.FeatureType);
        }
    }
}