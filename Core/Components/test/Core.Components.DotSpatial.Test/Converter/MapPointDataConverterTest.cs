using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Components.DotSpatial.Converter;
using Core.Components.DotSpatial.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using Core.Components.Gis.Style;
using DotSpatial.Controls;
using DotSpatial.Symbology;
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
                    new MapGeometry(new[]
                    {
                        Enumerable.Empty<Point2D>()
                    })
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
                    new MapGeometry(new[]
                    {
                        new[]
                        {
                            new Point2D(random.NextDouble(), random.NextDouble())
                        }
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
            CollectionAssert.AreNotEqual(pointData.Features.First().MapGeometries.First().PointCollections, layer.DataSet.Features[0].Coordinates);
        }

        [Test]
        public void Convert_MultipleFeatures_ReturnsAllFeaturesWithOneGeometry()
        {
            // Setup
            var converter = new MapPointDataConverter();
            var features = new List<MapFeature>
            {
                new MapFeature(new List<MapGeometry>
                {
                    new MapGeometry(new[]
                    {
                        new[]
                        {
                            new Point2D(1, 2)
                        }
                    })
                }),
                new MapFeature(new List<MapGeometry>
                {
                    new MapGeometry(new[]
                    {
                        new[]
                        {
                            new Point2D(2, 3)
                        }
                    })
                }),
                new MapFeature(new List<MapGeometry>
                {
                    new MapGeometry(new[]
                    {
                        new[]
                        {
                            new Point2D(4, 6)
                        }
                    })
                })
            };

            var pointData = new MapPointData(features, "test");

            // Call
            var layers = converter.Convert(pointData);

            // Assert
            var layer = layers.First();
            Assert.AreEqual(features.Count, layer.DataSet.Features.Count);
            layer.DataSet.InitializeVertices();
            Assert.AreEqual(3, layer.DataSet.ShapeIndices.Count);

            foreach (var shapeIndex in layer.DataSet.ShapeIndices)
            {
                Assert.AreEqual(1, shapeIndex.NumParts);
            }
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

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Convert_DataIsVisible_LayerIsVisibleSameAsData(bool isVisible)
        {
            // Setup
            var converter = new MapPointDataConverter();
            var data = new MapPointData(Enumerable.Empty<MapFeature>(), "test")
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
            var converter = new MapPointDataConverter();
            var data = new MapPointData(Enumerable.Empty<MapFeature>(), name);

            // Call
            var layers = converter.Convert(data);

            // Assert
            var layer = layers.First() as MapPointLayer;
            Assert.AreEqual(name, layer.Name);
        }

        [Test]
        [TestCase(KnownColor.AliceBlue)]
        [TestCase(KnownColor.Azure)]
        [TestCase(KnownColor.Beige)]
        public void Convert_WithDifferentColors_AppliesStyleToLayer(KnownColor color)
        {
            // Setup
            var converter = new MapPointDataConverter();
            var expectedColor = Color.FromKnownColor(color);
            var style = new PointStyle(expectedColor, 3, PointSymbol.Circle);
            var data = new MapPointData(Enumerable.Empty<MapFeature>(), "test")
            {
                Style = style
            };

            // Call
            var layers = converter.Convert(data);

            // Assert
            var layer = (MapPointLayer)layers.First();
            AssertAreEqual(new PointSymbolizer(expectedColor, PointShape.Ellipse, 3), layer.Symbolizer);
        }

        [Test]
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(7)]
        public void Convert_WithDifferentWidths_AppliesStyleToLayer(int width)
        {
            // Setup
            var converter = new MapPointDataConverter();
            var style = new PointStyle(Color.AliceBlue, width, PointSymbol.Circle);
            var data = new MapPointData(Enumerable.Empty<MapFeature>(), "test")
            {
                Style = style
            };

            // Call
            var layers = converter.Convert(data);

            // Assert
            var layer = (MapPointLayer)layers.First();
            AssertAreEqual(new PointSymbolizer(Color.AliceBlue, PointShape.Ellipse, width), layer.Symbolizer);
        }

        [Test]
        [TestCase(PointSymbol.Circle)]
        [TestCase(PointSymbol.Square)]
        [TestCase(PointSymbol.Triangle)]
        public void Convert_WithDifferentPointStyles_AppliesStyleToLayer(PointSymbol pointStyle)
        {
            // Setup
            var converter = new MapPointDataConverter();
            var style = new PointStyle(Color.AliceBlue, 3, pointStyle);
            var data = new MapPointData(Enumerable.Empty<MapFeature>(), "test")
            {
                Style = style
            };

            // Call
            var layers = converter.Convert(data);

            // Assert
            var layer = (MapPointLayer)layers.First();
            PointShape expectedPointShape = pointStyle == PointSymbol.Circle ? PointShape.Ellipse : pointStyle == PointSymbol.Square ? PointShape.Rectangle : PointShape.Triangle;
            AssertAreEqual(new PointSymbolizer(Color.AliceBlue, expectedPointShape, 3), layer.Symbolizer);
        }

        private void AssertAreEqual(IPointSymbolizer firstSymbolizer, IPointSymbolizer secondSymbolizer)
        {
            var firstSymbols = firstSymbolizer.Symbols;
            var secondSymbols = secondSymbolizer.Symbols;
            Assert.AreEqual(firstSymbols.Count, secondSymbols.Count, "Unequal amount of strokes defined.");
            for (var i = 0; i < firstSymbols.Count; i++)
            {
                var firstStroke = (SimpleSymbol)firstSymbols[i];
                var secondStroke = (SimpleSymbol)secondSymbols[i];

                Assert.AreEqual(firstStroke.Color, secondStroke.Color);
                Assert.AreEqual(firstStroke.PointShape, secondStroke.PointShape);
                Assert.AreEqual(firstStroke.Size, secondStroke.Size);
            }
        }
    }
}