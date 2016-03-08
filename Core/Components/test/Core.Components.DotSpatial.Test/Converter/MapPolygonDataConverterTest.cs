using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Components.DotSpatial.Converter;
using Core.Components.DotSpatial.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using DotSpatial.Controls;
using DotSpatial.Topology;
using NUnit.Framework;

namespace Core.Components.DotSpatial.Test.Converter
{
    [TestFixture]
    public class MapPolygonDataConverterTest
    {
        [Test]
        public void DefaultConstructor_IsMapPolygonDataConverter()
        {
            // Call
            var converter = new MapPolygonDataConverter();

            // Assert
            Assert.IsInstanceOf<MapDataConverter<MapPolygonData>>(converter);
        }

        [Test]
        public void CanConvertMapData_MapPolygonData_ReturnsTrue()
        {
            // Setup
            var feature = new List<MapFeature>
            {
                new MapFeature(new List<MapGeometry>
                {
                    new MapGeometry(Enumerable.Empty<Point2D>())
                })
            };

            var converter = new MapPolygonDataConverter();
            var polygonData = new MapPolygonData(feature, "test data");

            // Call
            var canConvert = converter.CanConvertMapData(polygonData);

            // Assert
            Assert.IsTrue(canConvert);
        }

        [Test]
        public void CanConvertMapData_TestMapData_ReturnsFalse()
        {
            // Setup
            var converter = new MapPolygonDataConverter();
            var mapData = new TestMapData("test data");

            // Call
            var canConvert = converter.CanConvertMapData(mapData);

            // Assert
            Assert.IsFalse(canConvert);
        }

        [Test]
        public void Convert_RandomPointData_ReturnsNewFeatureSetList()
        {
            // Setup
            var converter = new MapPolygonDataConverter();
            var random = new Random(21);
            var randomCount = random.Next(5, 10);
            var polygonPoints = new Collection<Point2D>();

            for (int i = 0; i < randomCount; i++)
            {
                polygonPoints.Add(new Point2D(random.NextDouble(), random.NextDouble()));
            }

            var feature = new List<MapFeature>
            {
                new MapFeature(new List<MapGeometry>
                {
                    new MapGeometry(polygonPoints)
                })
            };

            var polygonData = new MapPolygonData(feature, "test data");

            // Call
            var mapLayers = converter.Convert(polygonData);

            // Assert
            Assert.IsInstanceOf<IList<IMapFeatureLayer>>(mapLayers);
            var layer = mapLayers[0];
            Assert.AreEqual(1, layer.DataSet.Features.Count);
            Assert.IsInstanceOf<MapPolygonLayer>(layer);
            Assert.AreEqual(FeatureType.Polygon, layer.DataSet.FeatureType);
            CollectionAssert.AreNotEqual(polygonData.Features.First().MapGeometries.First().Points, layer.DataSet.Features[0].Coordinates);
        }

        [Test]
        public void Convert_DataNull_ThrowsArgumentNullException()
        {
            // Setup
            var testConverter = new MapPolygonDataConverter();

            // Call
            TestDelegate test = () => testConverter.Convert(null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, "Null data cannot be converted into feature sets.");
        }

        [Test]
        public void Convert_DataCannotBeConverted_ThrowsArgumentException()
        {
            // Setup
            var testConverter = new MapPolygonDataConverter();
            var testMapData = new TestMapData("test data");
            var expectedMessage = string.Format("The data of type {0} cannot be converted by this converter.", testMapData.GetType());

            // Precondition
            Assert.IsFalse(testConverter.CanConvertMapData(testMapData));

            // Call
            TestDelegate test = () => testConverter.Convert(testMapData);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }
    }
}