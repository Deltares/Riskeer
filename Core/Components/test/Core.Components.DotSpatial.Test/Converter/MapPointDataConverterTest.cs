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
    public class MapPointDataConverterTest
    {
        [Test]
        public void DefaultConstructor_IsMapPointDataConverter()
        {
            // Call
            var converter = new MapPointDataConverter();

            // Assert
            Assert.IsInstanceOf<MapDataConverter<MapPointData>>(converter);
        }

        [Test]
        public void CanConvertMapData_MapPointData_ReturnsTrue()
        {
            // Setup
            var feature = new List<MapFeature>
            {
                new MapFeature(new List<MapGeometry>
                {
                    new MapGeometry(Enumerable.Empty<Point2D>())
                })
            };

            var converter = new MapPointDataConverter();
            var pointData = new MapPointData(feature, "test data");

            // Call
            var canConvert = converter.CanConvertMapData(pointData);

            // Assert
            Assert.IsTrue(canConvert);
        }

        [Test]
        public void CanConvertMapData_TestMapData_ReturnsFalse()
        {
            // Setup
            var converter = new MapPointDataConverter();
            var mapData = new TestMapData("test data");

            // Call
            var canConvert = converter.CanConvertMapData(mapData);

            // Assert
            Assert.IsFalse(canConvert);
        }

        [Test]
        public void Convert_RandomPointData_ReturnsNewMapPointLayerList()
        {
            // Setup
            var converter = new MapPointDataConverter();
            var random = new Random(21);
            var randomCount = random.Next(5, 10);
            var features = new List<MapFeature>();

            for (int i = 0; i < randomCount; i++)
            {
                features.Add(new MapFeature(new List<MapGeometry>
                {
                    new MapGeometry(new List<Point2D>
                    {
                        new Point2D(random.NextDouble(), random.NextDouble())
                    })
                }));
            }            
            

            var pointData = new MapPointData(features, "test data");

            // Call
            var mapLayers = converter.Convert(pointData);

            // Assert
            Assert.IsInstanceOf<IList<IMapFeatureLayer>>(mapLayers);
            var layer = mapLayers[0];

            Assert.AreEqual(pointData.Features.ToArray().Length, layer.DataSet.Features.Count);
            Assert.IsInstanceOf<MapPointLayer>(layer);
            Assert.AreEqual(FeatureType.Point, layer.DataSet.FeatureType);
            CollectionAssert.AreNotEqual(pointData.Features.First().MapGeometries.First().Points, layer.DataSet.Features[0].Coordinates);
        }

        [Test]
        public void Convert_DataNull_ThrowsArgumentNullException()
        {
            // Setup
            var testConverter = new MapPointDataConverter();

            // Call
            TestDelegate test = () => testConverter.Convert(null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, "Null data cannot be converted into feature sets.");
        }

        [Test]
        public void Convert_DataCannotBeConverted_ThrowsArgumentException()
        {
            // Setup
            var testConverter = new MapPointDataConverter();
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