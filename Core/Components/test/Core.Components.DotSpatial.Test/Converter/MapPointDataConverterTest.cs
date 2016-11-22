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
using System.Data;
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
            var feature = new[]
            {
                new MapFeature(new[]
                {
                    new MapGeometry(new[]
                    {
                        Enumerable.Empty<Point2D>()
                    })
                })
            };

            var converter = new MapPointDataConverter();
            var pointData = new MapPointData("test data")
            {
                Features = feature
            };

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
        [TestCase(true)]
        [TestCase(false)]
        public void Convert_RandomPointDataWithoutAttributes_ReturnsNewMapPointLayerListWithDefaultLabelLayer(bool showLabels)
        {
            // Setup
            var converter = new MapPointDataConverter();
            var random = new Random(21);
            var randomCount = random.Next(5, 10);
            var features = new List<MapFeature>();

            for (var i = 0; i < randomCount; i++)
            {
                features.Add(new MapFeature(new[]
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

            var pointData = new MapPointData("test data")
            {
                Features = features.ToArray(),
                ShowLabels = showLabels
            };

            // Call
            var mapLayers = converter.Convert(pointData);

            // Assert
            Assert.IsInstanceOf<IList<IMapFeatureLayer>>(mapLayers);
            var layer = mapLayers[0];

            Assert.AreEqual(pointData.Features.ToArray().Length, layer.DataSet.Features.Count);
            Assert.IsInstanceOf<MapPointLayer>(layer);
            Assert.AreEqual(FeatureType.Point, layer.DataSet.FeatureType);
            CollectionAssert.AreNotEqual(pointData.Features.First().MapGeometries.First().PointCollections, layer.DataSet.Features[0].Coordinates);
            Assert.AreEqual(showLabels, layer.ShowLabels);
            CollectionAssert.IsEmpty(layer.DataSet.GetColumns());

            Assert.IsNotNull(layer.LabelLayer);
            Assert.AreEqual("FID", layer.LabelLayer.Symbology.Categories[0].Symbolizer.PriorityField);
            Assert.IsNull(layer.LabelLayer.Symbology.Categories[0].Expression);
        }
        
        [Test]
        public void Convert_RandomPointDataWithAttributesShowLabelsFalse_ReturnsNewMapPointLayerListWithDefaultLabelLayer()
        {
            // Setup
            var converter = new MapPointDataConverter();
            var random = new Random(21);
            var randomCount = random.Next(5, 10);
            var features = new List<MapFeature>();

            for (var i = 0; i < randomCount; i++)
            {
                var mapFeature = new MapFeature(new[]
                {
                    new MapGeometry(new[]
                    {
                        new[]
                        {
                            new Point2D(random.NextDouble(), random.NextDouble())
                        }
                    })
                });
                mapFeature.MetaData["ID"] = random.NextDouble();
                mapFeature.MetaData["Name"] = string.Format("feature [{0}]", i);

                features.Add(mapFeature);
            }

            var pointData = new MapPointData("test data")
            {
                Features = features.ToArray(),
                ShowLabels = false
            };

            // Call
            IList<IMapFeatureLayer> mapLayers = converter.Convert(pointData);

            // Assert
            Assert.IsInstanceOf<IList<IMapFeatureLayer>>(mapLayers);
            IMapFeatureLayer layer = mapLayers[0];

            Assert.AreEqual(pointData.Features.ToArray().Length, layer.DataSet.Features.Count);
            Assert.IsInstanceOf<MapPointLayer>(layer);
            Assert.AreEqual(FeatureType.Point, layer.DataSet.FeatureType);
            CollectionAssert.AreNotEqual(pointData.Features.First().MapGeometries.First().PointCollections, layer.DataSet.Features[0].Coordinates);
            Assert.IsFalse(layer.ShowLabels);
            CollectionAssert.IsEmpty(layer.DataSet.GetColumns());

            Assert.IsNotNull(layer.LabelLayer);
            Assert.AreEqual("FID", layer.LabelLayer.Symbology.Categories[0].Symbolizer.PriorityField);
            Assert.IsNull(layer.LabelLayer.Symbology.Categories[0].Expression);
        }
        
        [Test]
        [TestCase("id")]
        [TestCase("name")]
        public void Convert_RandomPointDataWithAttributesShowLabelsTrue_ReturnsNewMapPointLayerListWithCustomLabelLayer(string selectedAttribute)
        {
            // Setup
            var converter = new MapPointDataConverter();
            var random = new Random(21);
            var randomCount = random.Next(5, 10);
            var features = new List<MapFeature>();

            for (var i = 0; i < randomCount; i++)
            {
                var mapFeature = new MapFeature(new[]
                {
                    new MapGeometry(new[]
                    {
                        new[]
                        {
                            new Point2D(random.NextDouble(), random.NextDouble())
                        }
                    })
                });
                mapFeature.MetaData["ID"] = random.NextDouble();
                mapFeature.MetaData["Name"] = string.Format("feature [{0}]", i);

                features.Add(mapFeature);
            }

            var pointData = new MapPointData("test data")
            {
                Features = features.ToArray(),
                ShowLabels = true,
                SelectedMetaDataAttribute = selectedAttribute
            };

            // Call
            IList<IMapFeatureLayer> mapLayers = converter.Convert(pointData);

            // Assert
            Assert.IsInstanceOf<IList<IMapFeatureLayer>>(mapLayers);
            IMapFeatureLayer layer = mapLayers[0];

            Assert.AreEqual(pointData.Features.ToArray().Length, layer.DataSet.Features.Count);
            Assert.IsInstanceOf<MapPointLayer>(layer);
            Assert.AreEqual(FeatureType.Point, layer.DataSet.FeatureType);
            CollectionAssert.AreNotEqual(pointData.Features.First().MapGeometries.First().PointCollections, layer.DataSet.Features[0].Coordinates);
            Assert.IsTrue(layer.ShowLabels);

            DataColumn[] dataColumns = layer.DataSet.GetColumns();
            Assert.AreEqual(2, dataColumns.Length);
            Assert.AreEqual("ID", dataColumns[0].ColumnName);
            Assert.AreEqual("Name", dataColumns[1].ColumnName);

            Assert.IsNotNull(layer.LabelLayer);
            var labelCategory = layer.LabelLayer.Symbology.Categories[0];
            Assert.AreEqual(selectedAttribute, labelCategory.Symbolizer.PriorityField);
            Assert.AreEqual(ContentAlignment.MiddleRight, labelCategory.Symbolizer.Orientation);
            Assert.AreEqual(5, labelCategory.Symbolizer.OffsetX);
            Assert.AreEqual(string.Format("[{0}]", pointData.SelectedMetaDataAttribute), labelCategory.Expression);
        }

        [Test]
        public void Convert_MultipleFeatures_ReturnsAllFeaturesWithOneGeometry()
        {
            // Setup
            var converter = new MapPointDataConverter();
            var features = new[]
            {
                new MapFeature(new[]
                {
                    new MapGeometry(new[]
                    {
                        new[]
                        {
                            new Point2D(1, 2)
                        }
                    })
                }),
                new MapFeature(new[]
                {
                    new MapGeometry(new[]
                    {
                        new[]
                        {
                            new Point2D(2, 3)
                        }
                    })
                }),
                new MapFeature(new[]
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

            var pointData = new MapPointData("test")
            {
                Features = features
            };

            // Call
            var layers = converter.Convert(pointData);

            // Assert
            var layer = layers.First();
            Assert.AreEqual(features.Length, layer.DataSet.Features.Count);
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
            const string expectedMessage = "Null data cannot be converted into feature sets.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void Convert_DataCannotBeConverted_ThrowsArgumentException()
        {
            // Setup
            var testConverter = new MapPointDataConverter();
            var testMapData = new TestMapData("test data");

            // Precondition
            Assert.IsFalse(testConverter.CanConvertMapData(testMapData));

            // Call
            TestDelegate test = () => testConverter.Convert(testMapData);

            // Assert
            var expectedMessage = string.Format("The data of type {0} cannot be converted by this converter.", testMapData.GetType());
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Convert_DataIsVisible_LayerIsVisibleSameAsData(bool isVisible)
        {
            // Setup
            var converter = new MapPointDataConverter();
            var data = new MapPointData("test")
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
            var data = new MapPointData(name);

            // Call
            var layers = converter.Convert(data);

            // Assert
            var layer = (MapPointLayer) layers.First();
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
            var data = new MapPointData("test")
            {
                Style = style
            };

            // Call
            var layers = converter.Convert(data);

            // Assert
            var layer = (MapPointLayer) layers.First();
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
            var data = new MapPointData("test")
            {
                Style = style
            };

            // Call
            var layers = converter.Convert(data);

            // Assert
            var layer = (MapPointLayer) layers.First();
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
            var data = new MapPointData("test")
            {
                Style = style
            };

            // Call
            var layers = converter.Convert(data);

            // Assert
            var layer = (MapPointLayer) layers.First();
            PointShape expectedPointShape = pointStyle == PointSymbol.Circle
                                                ? PointShape.Ellipse
                                                : pointStyle == PointSymbol.Square
                                                      ? PointShape.Rectangle
                                                      : PointShape.Triangle;
            AssertAreEqual(new PointSymbolizer(Color.AliceBlue, expectedPointShape, 3), layer.Symbolizer);
        }

        private void AssertAreEqual(IPointSymbolizer firstSymbolizer, IPointSymbolizer secondSymbolizer)
        {
            var firstSymbols = firstSymbolizer.Symbols;
            var secondSymbols = secondSymbolizer.Symbols;
            Assert.AreEqual(firstSymbols.Count, secondSymbols.Count, "Unequal amount of strokes defined.");
            for (var i = 0; i < firstSymbols.Count; i++)
            {
                var firstStroke = (SimpleSymbol) firstSymbols[i];
                var secondStroke = (SimpleSymbol) secondSymbols[i];

                Assert.AreEqual(firstStroke.Color, secondStroke.Color);
                Assert.AreEqual(firstStroke.PointShape, secondStroke.PointShape);
                Assert.AreEqual(firstStroke.Size, secondStroke.Size);
            }
        }
    }
}