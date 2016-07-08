﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                    new MapGeometry(new[]
                    {
                        Enumerable.Empty<Point2D>()
                    })
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
        public void Convert_RandomPolygonData_ReturnsNewFeatureSetList()
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
                    new MapGeometry(new[]
                    {
                        polygonPoints
                    })
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
            CollectionAssert.AreNotEqual(polygonData.Features.First().MapGeometries.First().PointCollections, layer.DataSet.Features[0].Coordinates);
        }

        [Test]
        public void Convert_MapFeatureWithSimplePolygon_ReturnPolygonLayerWithOnePolygonFeature()
        {
            // Setup
            Point2D[] outerRingPoints = CreateRectangularRing(0.0, 10.0);
            var feature = new List<MapFeature>
            {
                new MapFeature(new List<MapGeometry>
                {
                    new MapGeometry(new[]
                    {
                        outerRingPoints
                    })
                })
            };
            const string layerName = "test data";
            var polygonData = new MapPolygonData(feature, layerName);

            // Call
            IList<IMapFeatureLayer> layers = new MapPolygonDataConverter().Convert(polygonData);

            // Assert
            Assert.AreEqual(1, layers.Count);
            var polygonLayer = (MapPolygonLayer)layers[0];
            Assert.AreEqual(polygonData.IsVisible, polygonLayer.IsVisible);
            Assert.AreEqual(layerName, polygonLayer.Name);
            Assert.AreEqual(FeatureType.Polygon, polygonLayer.FeatureSet.FeatureType);
            Assert.AreEqual(1, polygonLayer.FeatureSet.Features.Count);

            var featureGeometry = (IMultiPolygon)polygonLayer.FeatureSet.Features[0].BasicGeometry;
            Assert.AreEqual(1, featureGeometry.NumGeometries);

            var polygonGeometry = (IBasicPolygon)featureGeometry.Geometries[0];
            CollectionAssert.AreEqual(outerRingPoints, polygonGeometry.Shell.Coordinates.Select(c => new Point2D(c.X, c.Y)));
            CollectionAssert.IsEmpty(polygonGeometry.Holes);
        }

        [Test]
        public void Convert_MapFeatureWithPolygonWithHoles_ReturnPolygonLayerWithOnePolygonFeature()
        {
            // Setup
            Point2D[] outerRingPoints = CreateRectangularRing(0.0, 10.0);
            Point2D[] innerRing1Points = CreateRectangularRing(2.0, 3.0);
            Point2D[] innerRing2Points = CreateRectangularRing(8.0, 5.0);
            var feature = new List<MapFeature>
            {
                new MapFeature(new List<MapGeometry>
                {
                    new MapGeometry(new[]
                    {
                        outerRingPoints,
                        innerRing1Points,
                        innerRing2Points
                    })
                })
            };
            const string layerName = "test data";
            var polygonData = new MapPolygonData(feature, layerName);

            // Call
            IList<IMapFeatureLayer> layers = new MapPolygonDataConverter().Convert(polygonData);

            // Assert
            Assert.AreEqual(1, layers.Count);
            var polygonLayer = (MapPolygonLayer)layers[0];
            Assert.AreEqual(polygonData.IsVisible, polygonLayer.IsVisible);
            Assert.AreEqual(layerName, polygonLayer.Name);
            Assert.AreEqual(FeatureType.Polygon, polygonLayer.FeatureSet.FeatureType);
            Assert.AreEqual(1, polygonLayer.FeatureSet.Features.Count);

            var featureGeometry = (IMultiPolygon)polygonLayer.FeatureSet.Features[0].BasicGeometry;
            Assert.AreEqual(1, featureGeometry.NumGeometries);

            var polygonGeometry = (IBasicPolygon)featureGeometry.Geometries[0];
            CollectionAssert.AreEqual(outerRingPoints, polygonGeometry.Shell.Coordinates.Select(c => new Point2D(c.X, c.Y)));
            Assert.AreEqual(2, polygonGeometry.Holes.Count);
            CollectionAssert.AreEqual(innerRing1Points, polygonGeometry.Holes.ElementAt(0).Coordinates.Select(c => new Point2D(c.X, c.Y)));
            CollectionAssert.AreEqual(innerRing2Points, polygonGeometry.Holes.ElementAt(1).Coordinates.Select(c => new Point2D(c.X, c.Y)));
        }

        private static Point2D[] CreateRectangularRing(double xy1, double xy2)
        {
            return new[]
            {
                new Point2D(xy1, xy1),
                new Point2D(xy2, xy1),
                new Point2D(xy2, xy2),
                new Point2D(xy1, xy2),
                new Point2D(xy1, xy1)
            };
        }

        [Test]
        public void Convert_MultipleFeatures_ConvertsAllFeatures()
        {
            // Setup
            var converter = new MapPolygonDataConverter();
            var features = new List<MapFeature>
            {
                new MapFeature(Enumerable.Empty<MapGeometry>()),
                new MapFeature(Enumerable.Empty<MapGeometry>()),
                new MapFeature(Enumerable.Empty<MapGeometry>())
            };

            var polygonData = new MapPolygonData(features, "test");

            // Call
            var mapLayers = converter.Convert(polygonData);

            // Assert
            Assert.AreEqual(1, mapLayers.Count);
            var layer = mapLayers[0];
            Assert.AreEqual(features.Count, layer.DataSet.Features.Count);
        }

        [Test]
        public void Convert_MultipleGeometriesInFeature_ReturnsOneFeatureWithAllGeometries()
        {
            // Setup
            var converter = new MapPolygonDataConverter();
            var features = new List<MapFeature>
            {
                new MapFeature(new List<MapGeometry>
                {
                    new MapGeometry(new[]
                    {
                        new[]
                        {
                            new Point2D(1.0, 2.0),
                            new Point2D(2.0, 1.0),
                        }
                    }),
                    new MapGeometry(new[]
                    {
                        new[]
                        {
                            new Point2D(2.0, 2.0),
                            new Point2D(3.0, 2.0),
                        }
                    }),
                    new MapGeometry(new[]
                    {
                        new[]
                        {
                            new Point2D(1.0, 3.0),
                            new Point2D(1.0, 4.0),
                        }
                    }),
                    new MapGeometry(new[]
                    {
                        new[]
                        {
                            new Point2D(3.0, 2.0),
                            new Point2D(4.0, 1.0),
                        }
                    })
                })
            };
            var geometries = features.First().MapGeometries.ToArray();

            var polygonData = new MapPolygonData(features, "test");

            // Call
            var mapLayers = converter.Convert(polygonData);

            // Assert
            Assert.AreEqual(1, mapLayers.Count);
            var layer = mapLayers[0];
            Assert.AreEqual(features.Count, layer.DataSet.Features.Count);
            layer.DataSet.InitializeVertices();
            var layerGeometries = layer.DataSet.ShapeIndices.First().Parts;
            Assert.AreEqual(geometries.Length, layerGeometries.Count);
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

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Convert_DataIsVisible_LayerIsVisibleSameAsData(bool isVisible)
        {
            // Setup
            var converter = new MapPolygonDataConverter();
            var data = new MapPolygonData(Enumerable.Empty<MapFeature>(), "test")
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
            var converter = new MapPolygonDataConverter();
            var data = new MapPolygonData(Enumerable.Empty<MapFeature>(), name);

            // Call
            var layers = converter.Convert(data);

            // Assert
            var layer = layers.First() as MapPolygonLayer;
            Assert.AreEqual(name, layer.Name);
        }

        [Test]
        [TestCase(KnownColor.AliceBlue)]
        [TestCase(KnownColor.Azure)]
        [TestCase(KnownColor.Beige)]
        public void Convert_WithDifferentFillColors_AppliesStyleToLayer(KnownColor color)
        {
            // Setup
            var converter = new MapPolygonDataConverter();
            var expectedColor = Color.FromKnownColor(color);
            var style = new PolygonStyle(expectedColor, Color.AliceBlue, 3);
            var data = new MapPolygonData(Enumerable.Empty<MapFeature>(), "test")
            {
                Style = style
            };

            // Call
            var layers = converter.Convert(data);

            // Assert
            var layer = (MapPolygonLayer)layers.First();
            AssertAreEqual(new PolygonSymbolizer(expectedColor, Color.AliceBlue, 3), layer.Symbolizer);
        }

        [Test]
        [TestCase(KnownColor.AliceBlue)]
        [TestCase(KnownColor.Azure)]
        [TestCase(KnownColor.Beige)]
        public void Convert_WithDifferentStrokeColors_AppliesStyleToLayer(KnownColor color)
        {
            // Setup
            var converter = new MapPolygonDataConverter();
            var expectedColor = Color.FromKnownColor(color);
            var style = new PolygonStyle(Color.AliceBlue, expectedColor, 3);
            var data = new MapPolygonData(Enumerable.Empty<MapFeature>(), "test")
            {
                Style = style
            };

            // Call
            var layers = converter.Convert(data);

            // Assert
            var layer = (MapPolygonLayer)layers.First();
            AssertAreEqual(new PolygonSymbolizer(Color.AliceBlue, expectedColor, 3), layer.Symbolizer);
        }

        [Test]
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(7)]
        public void Convert_WithDifferentWidths_AppliesStyleToLayer(int width)
        {
            // Setup
            var converter = new MapPolygonDataConverter();
            var style = new PolygonStyle(Color.AliceBlue, Color.AliceBlue, width);
            var data = new MapPolygonData(Enumerable.Empty<MapFeature>(), "test")
            {
                Style = style
            };

            // Call
            var layers = converter.Convert(data);

            // Assert
            var layer = (MapPolygonLayer)layers.First();
            AssertAreEqual(new PolygonSymbolizer(Color.AliceBlue, Color.AliceBlue, width), layer.Symbolizer);
        }

        private void AssertAreEqual(IPolygonSymbolizer firstSymbolizer, IPolygonSymbolizer secondSymbolizer)
        {
            var firstSymbols = firstSymbolizer.Patterns;
            var secondSymbols = secondSymbolizer.Patterns;
            Assert.AreEqual(firstSymbols.Count, secondSymbols.Count, "Unequal amount of strokes defined.");
            for (var i = 0; i < firstSymbols.Count; i++)
            {
                var firstStroke = (SimplePattern)firstSymbols[i];
                var secondStroke = (SimplePattern)secondSymbols[i];

                Assert.AreEqual(firstStroke.FillColor, secondStroke.FillColor);
                Assert.AreEqual(firstStroke.Outline.GetFillColor(), secondStroke.Outline.GetFillColor());
                Assert.AreEqual(firstStroke.Outline.GetWidth(), secondStroke.Outline.GetWidth());
            }
        }
    }
}