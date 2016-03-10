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
using NUnit.Framework;

namespace Core.Components.DotSpatial.Test.Converter
{
    [TestFixture]
    public class MapLineDataConverterTest
    {
        [Test]
        public void DefaultConstructor_IsMapLineDataConverter()
        {
            // Call
            var converter = new MapLineDataConverter();

            // Assert
            Assert.IsInstanceOf<MapDataConverter<MapLineData>>(converter);
        }

        [Test]
        public void CanConvertMapData_MapLineData_ReturnTrue()
        {
            // Setup
            var feature = new List<MapFeature>
            {
                new MapFeature(new List<MapGeometry>
                {
                    new MapGeometry(Enumerable.Empty<Point2D>())
                })
            };
            var converter = new MapLineDataConverter();
            var lineData = new MapLineData(feature, "test data");

            // Call
            var canConvert = converter.CanConvertMapData(lineData);

            // Assert
            Assert.IsTrue(canConvert);
        }

        [Test]
        public void CanConvertMapData_TestMapData_ReturnsFalse()
        {
            // Setup
            var converter = new MapLineDataConverter();
            var mapData = new TestMapData("test data");

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
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, "Null data cannot be converted into feature sets.");
        }

        [Test]
        public void Convert_DataCannotBeConverted_ThrowsArgumentException()
        {
            // Setup
            var testConverter = new MapLineDataConverter();
            var testMapData = new TestMapData("test data");
            var expectedMessage = string.Format("The data of type {0} cannot be converted by this converter.", testMapData.GetType());

            // Precondition
            Assert.IsFalse(testConverter.CanConvertMapData(testMapData));

            // Call
            TestDelegate test = () => testConverter.Convert(testMapData);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        public void Convert_RandomPointData_ReturnsNewMapLineLayerList()
        {
            // Setup
            var converter = new MapLineDataConverter();
            var random = new Random(21);
            var randomCount = random.Next(5, 10);           
            var points = new Collection<Point2D>();

            for (int i = 0; i < randomCount; i++)
            {
                points.Add(new Point2D(random.NextDouble(), random.NextDouble()));
            }

            var feature = new List<MapFeature>
            {
                new MapFeature(new List<MapGeometry>
                {
                    new MapGeometry(points)
                })
            };

            var lineData = new MapLineData(feature, "test data");

            // Call
            var mapLayers = converter.Convert(lineData);

            // Assert
            Assert.IsInstanceOf<IList<IMapFeatureLayer>>(mapLayers);
            var layer = mapLayers[0];
            Assert.AreEqual(1, mapLayers.Count);
            Assert.IsInstanceOf<MapLineLayer>(layer);
        }

        [Test]
        public void Convert_MultipleFeatures_ConvertsAllFeatures()
        {
            // Setup
            var converter = new MapLineDataConverter();
            var features = new List<MapFeature>
            {
                new MapFeature(Enumerable.Empty<MapGeometry>()),
                new MapFeature(Enumerable.Empty<MapGeometry>()),
                new MapFeature(Enumerable.Empty<MapGeometry>())
            };

            var lineData = new MapLineData(features, "test");

            // Call
            var mapLayers = converter.Convert(lineData);

            // Assert
            Assert.AreEqual(1, mapLayers.Count);
            var layer = mapLayers[0];
            Assert.AreEqual(features.Count, layer.DataSet.Features.Count);
        }

        [Test]
        public void Convert_MultipleGeometriesInFeature_ReturnsOneFeatureWithAllGeometries()
        {
            // Setup
            var converter = new MapLineDataConverter();
            var features = new List<MapFeature>
            {
                new MapFeature(new List<MapGeometry>
                {
                    new MapGeometry(new List<Point2D>
                    {
                        new Point2D(1.0, 2.0),
                        new Point2D(2.0, 1.0),
                    }),
                    new MapGeometry(new List<Point2D>
                    {
                        new Point2D(2.0, 2.0),
                        new Point2D(3.0, 2.0),
                    }),
                    new MapGeometry(new List<Point2D>
                    {
                        new Point2D(1.0, 3.0),
                        new Point2D(1.0, 4.0),
                    }),
                    new MapGeometry(new List<Point2D>
                    {
                        new Point2D(3.0, 2.0),
                        new Point2D(4.0, 1.0),
                    })
                })
            };

            var geometries = features.First().MapGeometries.ToArray();

            var lineData = new MapLineData(features, "test");

            // Call
            var mapLayers = converter.Convert(lineData);

            // Assert
            Assert.AreEqual(1, mapLayers.Count);
            var layer = mapLayers[0];
            Assert.AreEqual(features.Count, layer.DataSet.Features.Count);
            var layerGeometries = layer.DataSet.ShapeIndices.First().Parts;
            Assert.AreEqual(geometries.Length, layerGeometries.Count);
        }
			

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Convert_DataIsVisible_LayerIsVisibleSameAsData(bool isVisible)
        {
            // Setup
            var converter = new MapLineDataConverter();
            var data = new MapLineData(Enumerable.Empty<MapFeature>(), "test")
            {
                IsVisible = isVisible
            };

            // Call
            var layers = converter.Convert(data);

            // Assert
            Assert.AreEqual(isVisible, layers.First().IsVisible);
        }

        [Test]
        public void Convert_DataName_LayerNameSameAsData()
        {
            // Setup
            var name = "<Some name>";
            var converter = new MapLineDataConverter();
            var data = new MapLineData(Enumerable.Empty<MapFeature>(), name);

            // Call
            var layers = converter.Convert(data);

            // Assert
            var layer = layers.First() as MapLineLayer;
            Assert.AreEqual(name, layer.Name);
        }
    }
}