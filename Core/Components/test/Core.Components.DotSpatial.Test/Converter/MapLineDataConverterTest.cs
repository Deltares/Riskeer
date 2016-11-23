// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Components.DotSpatial.Converter;
using Core.Components.DotSpatial.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using DotSpatial.Controls;
using DotSpatial.Symbology;
using DotSpatial.Topology;
using NUnit.Framework;
using LineStyle = Core.Components.Gis.Style.LineStyle;

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
            var mapFeatures = new[]
            {
                new MapFeature(new[]
                {
                    new MapGeometry(new[]
                    {
                        Enumerable.Empty<Point2D>()
                    })
                })
            };
            var converter = new MapLineDataConverter();
            var lineData = new MapLineData("test data")
            {
                Features = mapFeatures
            };

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
            const string expectedMessage = "Null data cannot be converted into feature sets.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void Convert_DataCannotBeConverted_ThrowsArgumentException()
        {
            // Setup
            var testConverter = new MapLineDataConverter();
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
        public void Convert_MapDataWithSingleGeometry_ReturnMapLineLayerWithLineString()
        {
            // Setup
            var converter = new MapLineDataConverter();
            var random = new Random(21);
            var features = new List<MapFeature>
            {
                new MapFeature(new[]
                {
                    new MapGeometry(new[]
                    {
                        new[]
                        {
                            new Point2D(random.NextDouble(), random.NextDouble()),
                            new Point2D(random.NextDouble(), random.NextDouble()),
                            new Point2D(random.NextDouble(), random.NextDouble())
                        }
                    })
                })
            };

            var lineData = new MapLineData("test data")
            {
                Features = features.ToArray()
            };

            // Call
            var mapLayers = converter.Convert(lineData);

            // Assert
            var layer = mapLayers[0];
            Assert.IsInstanceOf<LineString>(layer.DataSet.Features[0].BasicGeometry);
        }

        [Test]
        public void Convert_MapDataWithMultipleGeometry_ReturnMapLineLayerWithMultiLineString()
        {
            // Setup
            var converter = new MapLineDataConverter();
            var random = new Random(21);
            var features = new List<MapFeature>
            {
                new MapFeature(new[]
                {
                    new MapGeometry(new[]
                    {
                        new[]
                        {
                            new Point2D(random.NextDouble(), random.NextDouble()),
                            new Point2D(random.NextDouble(), random.NextDouble()),
                            new Point2D(random.NextDouble(), random.NextDouble())
                        }
                    }),
                    new MapGeometry(new[]
                    {
                        new[]
                        {
                            new Point2D(random.NextDouble(), random.NextDouble()),
                            new Point2D(random.NextDouble(), random.NextDouble()),
                            new Point2D(random.NextDouble(), random.NextDouble())
                        }
                    })
                })
            };

            var lineData = new MapLineData("test data")
            {
                Features = features.ToArray()
            };

            // Call
            var mapLayers = converter.Convert(lineData);

            // Assert
            var layer = mapLayers[0];
            Assert.IsInstanceOf<MultiLineString>(layer.DataSet.Features[0].BasicGeometry);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Convert_RandomLineDataWithoutAttributes_ReturnsNewMapLineLayerListWithDefaultLabelLayer(bool showLabels)
        {
            // Setup
            var converter = new MapLineDataConverter();
            var random = new Random(21);
            var features = new List<MapFeature>
            {
                new MapFeature(new[]
                {
                    new MapGeometry(new[]
                    {
                        new[]
                        {
                            new Point2D(random.NextDouble(), random.NextDouble()),
                            new Point2D(random.NextDouble(), random.NextDouble()),
                            new Point2D(random.NextDouble(), random.NextDouble())
                        }
                    })
                })
            };

            var lineData = new MapLineData("test data")
            {
                Features = features.ToArray(),
                ShowLabels = showLabels
            };

            // Call
            var mapLayers = converter.Convert(lineData);

            // Assert
            Assert.IsInstanceOf<IList<IMapFeatureLayer>>(mapLayers);
            var layer = mapLayers[0];

            Assert.AreEqual(lineData.Features.ToArray().Length, layer.DataSet.Features.Count);
            Assert.IsInstanceOf<MapLineLayer>(layer);
            Assert.AreEqual(FeatureType.Line, layer.DataSet.FeatureType);
            CollectionAssert.AreNotEqual(lineData.Features.First().MapGeometries.First().PointCollections, layer.DataSet.Features[0].Coordinates);
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
            var converter = new MapLineDataConverter();
            var random = new Random(21);
            var features = new List<MapFeature>();

            var mapFeature = new MapFeature(new[]
            {
                new MapGeometry(new[]
                {
                    new[]
                    {
                        new Point2D(random.NextDouble(), random.NextDouble()),
                        new Point2D(random.NextDouble(), random.NextDouble()),
                        new Point2D(random.NextDouble(), random.NextDouble())
                    }
                })
            });
            mapFeature.MetaData["ID"] = random.NextDouble();
            mapFeature.MetaData["Name"] = "feature";

            features.Add(mapFeature);

            var lineData = new MapLineData("test data")
            {
                Features = features.ToArray(),
                ShowLabels = false
            };

            // Call
            IList<IMapFeatureLayer> mapLayers = converter.Convert(lineData);

            // Assert
            Assert.IsInstanceOf<IList<IMapFeatureLayer>>(mapLayers);
            IMapFeatureLayer layer = mapLayers[0];

            Assert.AreEqual(lineData.Features.ToArray().Length, layer.DataSet.Features.Count);
            Assert.IsInstanceOf<MapLineLayer>(layer);
            Assert.AreEqual(FeatureType.Line, layer.DataSet.FeatureType);
            CollectionAssert.AreNotEqual(lineData.Features.First().MapGeometries.First().PointCollections, layer.DataSet.Features[0].Coordinates);
            Assert.IsFalse(layer.ShowLabels);
            CollectionAssert.IsEmpty(layer.DataSet.GetColumns());

            Assert.IsNotNull(layer.LabelLayer);
            Assert.AreEqual("FID", layer.LabelLayer.Symbology.Categories[0].Symbolizer.PriorityField);
            Assert.IsNull(layer.LabelLayer.Symbology.Categories[0].Expression);
        }

        [Test]
        [TestCase("id")]
        [TestCase("name")]
        public void Convert_RandomLineDataWithAttributesShowLabelsTrue_ReturnsNewMapLineLayerListWithCustomLabelLayer(string selectedAttribute)
        {
            // Setup
            var converter = new MapLineDataConverter();
            var random = new Random(21);
            var features = new List<MapFeature>();

            var mapFeature = new MapFeature(new[]
            {
                new MapGeometry(new[]
                {
                    new[]
                    {
                        new Point2D(random.NextDouble(), random.NextDouble()),
                        new Point2D(random.NextDouble(), random.NextDouble()),
                        new Point2D(random.NextDouble(), random.NextDouble())
                    }
                })
            });
            mapFeature.MetaData["ID"] = random.NextDouble();
            mapFeature.MetaData["Name"] = "feature";

            features.Add(mapFeature);

            var lineData = new MapLineData("test data")
            {
                Features = features.ToArray(),
                ShowLabels = true,
                SelectedMetaDataAttribute = selectedAttribute
            };

            // Call
            IList<IMapFeatureLayer> mapLayers = converter.Convert(lineData);

            // Assert
            Assert.IsInstanceOf<IList<IMapFeatureLayer>>(mapLayers);
            IMapFeatureLayer layer = mapLayers[0];

            Assert.AreEqual(lineData.Features.ToArray().Length, layer.DataSet.Features.Count);
            Assert.IsInstanceOf<MapLineLayer>(layer);
            Assert.AreEqual(FeatureType.Line, layer.DataSet.FeatureType);
            CollectionAssert.AreNotEqual(lineData.Features.First().MapGeometries.First().PointCollections, layer.DataSet.Features[0].Coordinates);
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
            Assert.AreEqual(string.Format("[{0}]", lineData.SelectedMetaDataAttribute), labelCategory.Expression);
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

            var mapFeatures = new[]
            {
                new MapFeature(new[]
                {
                    new MapGeometry(new[]
                    {
                        points
                    })
                })
            };

            var lineData = new MapLineData("test data")
            {
                Features = mapFeatures
            };

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
            var features = new[]
            {
                new MapFeature(Enumerable.Empty<MapGeometry>()),
                new MapFeature(Enumerable.Empty<MapGeometry>()),
                new MapFeature(Enumerable.Empty<MapGeometry>())
            };

            var lineData = new MapLineData("test")
            {
                Features = features
            };

            // Call
            var mapLayers = converter.Convert(lineData);

            // Assert
            Assert.AreEqual(1, mapLayers.Count);
            var layer = mapLayers[0];
            Assert.AreEqual(features.Length, layer.DataSet.Features.Count);
        }

        [Test]
        public void Convert_MultipleGeometriesInFeature_ReturnsOneFeatureWithAllGeometries()
        {
            // Setup
            var converter = new MapLineDataConverter();
            var features = new[]
            {
                new MapFeature(new[]
                {
                    new MapGeometry(new[]
                    {
                        new[]
                        {
                            new Point2D(1.0, 2.0),
                            new Point2D(2.0, 1.0)
                        }
                    }),
                    new MapGeometry(new[]
                    {
                        new[]
                        {
                            new Point2D(2.0, 2.0),
                            new Point2D(3.0, 2.0)
                        }
                    }),
                    new MapGeometry(new[]
                    {
                        new[]
                        {
                            new Point2D(1.0, 3.0),
                            new Point2D(1.0, 4.0)
                        }
                    }),
                    new MapGeometry(new[]
                    {
                        new[]
                        {
                            new Point2D(3.0, 2.0),
                            new Point2D(4.0, 1.0)
                        }
                    })
                })
            };

            var geometries = features.First().MapGeometries.ToArray();

            var lineData = new MapLineData("test")
            {
                Features = features
            };

            // Call
            var mapLayers = converter.Convert(lineData);

            // Assert
            Assert.AreEqual(1, mapLayers.Count);
            var layer = mapLayers[0];
            Assert.AreEqual(features.Length, layer.DataSet.Features.Count);
            layer.DataSet.InitializeVertices();
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
            var data = new MapLineData("test")
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
            var data = new MapLineData(name);

            // Call
            var layers = converter.Convert(data);

            // Assert
            var layer = (MapLineLayer) layers.First();
            Assert.AreEqual(name, layer.Name);
        }

        [Test]
        [TestCase(KnownColor.AliceBlue)]
        [TestCase(KnownColor.Azure)]
        [TestCase(KnownColor.Beige)]
        public void Convert_WithDifferentColors_AppliesStyleToLayer(KnownColor color)
        {
            // Setup
            var converter = new MapLineDataConverter();
            var expectedColor = Color.FromKnownColor(color);
            var style = new LineStyle(expectedColor, 3, DashStyle.Solid);
            var data = new MapLineData("test")
            {
                Style = style
            };

            // Call
            var layers = converter.Convert(data);

            // Assert
            var layer = (MapLineLayer) layers.First();
            AssertAreEqual(new LineSymbolizer(expectedColor, expectedColor, 3, DashStyle.Solid, LineCap.Round), layer.Symbolizer);
        }

        [Test]
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(7)]
        public void Convert_WithDifferentWidths_AppliesStyleToLayer(int width)
        {
            // Setup
            var converter = new MapLineDataConverter();
            var style = new LineStyle(Color.AliceBlue, width, DashStyle.Solid);
            var data = new MapLineData("test")
            {
                Style = style
            };

            // Call
            var layers = converter.Convert(data);

            // Assert
            var layer = (MapLineLayer) layers.First();
            AssertAreEqual(new LineSymbolizer(Color.AliceBlue, Color.AliceBlue, width, DashStyle.Solid, LineCap.Round), layer.Symbolizer);
        }

        [Test]
        [TestCase(DashStyle.Solid)]
        [TestCase(DashStyle.Dash)]
        [TestCase(DashStyle.Dot)]
        public void Convert_WithDifferentLineStyles_AppliesStyleToLayer(DashStyle lineStyle)
        {
            // Setup
            var converter = new MapLineDataConverter();
            var style = new LineStyle(Color.AliceBlue, 3, lineStyle);
            var data = new MapLineData("test")
            {
                Style = style
            };

            // Call
            var layers = converter.Convert(data);

            // Assert
            var layer = (MapLineLayer) layers.First();
            AssertAreEqual(new LineSymbolizer(Color.AliceBlue, Color.AliceBlue, 3, lineStyle, LineCap.Round), layer.Symbolizer);
        }

        private void AssertAreEqual(ILineSymbolizer firstSymbolizer, ILineSymbolizer secondSymbolizer)
        {
            var firstStrokes = firstSymbolizer.Strokes;
            var secondStrokes = secondSymbolizer.Strokes;
            Assert.AreEqual(firstStrokes.Count, secondStrokes.Count, "Unequal amount of strokes defined.");
            for (var i = 0; i < firstStrokes.Count; i++)
            {
                var firstStroke = (CartographicStroke) firstStrokes[i];
                var secondStroke = (CartographicStroke) secondStrokes[i];

                Assert.AreEqual(firstStroke.Color, secondStroke.Color);
                Assert.AreEqual(firstStroke.EndCap, secondStroke.EndCap);
                Assert.AreEqual(firstStroke.DashStyle, secondStroke.DashStyle);
                Assert.AreEqual(firstStroke.Width, secondStroke.Width);
            }
        }
    }
}