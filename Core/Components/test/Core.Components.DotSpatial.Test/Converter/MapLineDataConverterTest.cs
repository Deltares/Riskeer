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
using Core.Components.DotSpatial.Converter;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using DotSpatial.Controls;
using DotSpatial.Data;
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
            Assert.IsInstanceOf<FeatureBasedMapDataConverter<MapLineData, MapLineLayer>>(converter);
        }

        [Test]
        public void Convert_MapDataWithSingleGeometry_ReturnMapLineLayerWithLineStringData()
        {
            // Setup
            var converter = new MapLineDataConverter();
            var random = new Random(21);
            MapFeature[] features =
            {
                CreateSingleGeometryMapFeature(random),
                CreateSingleGeometryMapFeature(random)
            };

            var lineData = new MapLineData("test data")
            {
                Features = features
            };

            // Call
            IMapFeatureLayer layer = converter.Convert(lineData);

            // Assert
            Assert.AreEqual(lineData.Features.Length, layer.DataSet.Features.Count);
            Assert.IsInstanceOf<MapLineLayer>(layer);
            Assert.AreEqual(FeatureType.Line, layer.DataSet.FeatureType);
            AssertLineStringData(lineData.Features[0], layer.DataSet.Features[0]);
            AssertLineStringData(lineData.Features[1], layer.DataSet.Features[1]);
        }

        [Test]
        public void Convert_MapDataWithMultipleGeometry_ReturnMapLineLayerWithMultiLineStringData()
        {
            // Setup
            var converter = new MapLineDataConverter();
            var random = new Random(21);
            MapFeature[] features =
            {
                CreateMultipleGeometryMapFeature(random),
                CreateMultipleGeometryMapFeature(random)
            };

            var lineData = new MapLineData("test data")
            {
                Features = features
            };

            // Call
            IMapFeatureLayer layer = converter.Convert(lineData);

            // Assert
            Assert.AreEqual(lineData.Features.Length, layer.DataSet.Features.Count);
            Assert.IsInstanceOf<MapLineLayer>(layer);
            Assert.AreEqual(FeatureType.Line, layer.DataSet.FeatureType);
            Assert.IsInstanceOf<MultiLineString>(layer.DataSet.Features[0].BasicGeometry);
            Assert.IsInstanceOf<MultiLineString>(layer.DataSet.Features[1].BasicGeometry);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Convert_RandomLineDataWithoutAttributes_ReturnsNewMapLineLayerWithDefaultLabelLayer(bool showLabels)
        {
            // Setup
            var converter = new MapLineDataConverter();
            var random = new Random(21);
            MapFeature[] features =
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
                Features = features,
                ShowLabels = showLabels
            };

            // Call
            IMapFeatureLayer layer = converter.Convert(lineData);

            // Assert
            Assert.AreEqual(lineData.Features.Length, layer.DataSet.Features.Count);
            Assert.IsInstanceOf<MapLineLayer>(layer);
            Assert.AreEqual(FeatureType.Line, layer.DataSet.FeatureType);

            IEnumerable<Point2D> points = lineData.Features.First().MapGeometries.First().PointCollections.First();
            CollectionAssert.AreEqual(points.Select(p => new Coordinate(p.X, p.Y)), layer.DataSet.Features[0].Coordinates);
            Assert.AreEqual(showLabels, layer.ShowLabels);
            CollectionAssert.IsEmpty(layer.DataSet.GetColumns());

            Assert.IsNotNull(layer.LabelLayer);
            Assert.AreEqual("FID", layer.LabelLayer.Symbology.Categories[0].Symbolizer.PriorityField);
            Assert.IsNull(layer.LabelLayer.Symbology.Categories[0].Expression);
        }

        [Test]
        public void Convert_RandomLineDataWithAttributesShowLabelsFalse_ReturnsNewMapPointLayerWithDefaultLabelLayer()
        {
            // Setup
            var converter = new MapLineDataConverter();
            var random = new Random(21);
            var features = new MapFeature[1];

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

            features[0] = mapFeature;

            var lineData = new MapLineData("test data")
            {
                Features = features,
                ShowLabels = false
            };

            // Call
            IMapFeatureLayer layer = converter.Convert(lineData);

            // Assert
            Assert.AreEqual(lineData.Features.Length, layer.DataSet.Features.Count);
            Assert.IsInstanceOf<MapLineLayer>(layer);
            Assert.AreEqual(FeatureType.Line, layer.DataSet.FeatureType);
            IEnumerable<Point2D> points = lineData.Features.First().MapGeometries.First().PointCollections.First();
            CollectionAssert.AreEqual(points.Select(p => new Coordinate(p.X, p.Y)), layer.DataSet.Features[0].Coordinates);
            Assert.IsFalse(layer.ShowLabels);

            DataColumn[] dataColumns = layer.DataSet.GetColumns();
            Assert.AreEqual(2, dataColumns.Length);
            Assert.AreEqual("1", dataColumns[0].ColumnName);
            Assert.AreEqual("2", dataColumns[1].ColumnName);

            Assert.IsNotNull(layer.LabelLayer);
            Assert.AreEqual("FID", layer.LabelLayer.Symbology.Categories[0].Symbolizer.PriorityField);
            Assert.IsNull(layer.LabelLayer.Symbology.Categories[0].Expression);
        }

        [Test]
        [TestCase("ID", 1)]
        [TestCase("Name", 2)]
        public void Convert_RandomLineDataWithAttributesShowLabelsTrue_ReturnsNewMapLineLayerWithCustomLabelLayer(string selectedAttribute, int selectedAttributeId)
        {
            // Setup
            var converter = new MapLineDataConverter();
            var random = new Random(21);
            var features = new MapFeature[1];

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

            features[0] = mapFeature;

            var lineData = new MapLineData("test data")
            {
                Features = features,
                ShowLabels = true,
                SelectedMetaDataAttribute = selectedAttribute
            };

            // Call
            IMapFeatureLayer layer = converter.Convert(lineData);

            // Assert
            Assert.AreEqual(lineData.Features.Length, layer.DataSet.Features.Count);
            Assert.IsInstanceOf<MapLineLayer>(layer);
            Assert.AreEqual(FeatureType.Line, layer.DataSet.FeatureType);
            IEnumerable<Point2D> points = lineData.Features.First().MapGeometries.First().PointCollections.First();
            CollectionAssert.AreEqual(points.Select(p => new Coordinate(p.X, p.Y)), layer.DataSet.Features[0].Coordinates);
            Assert.IsTrue(layer.ShowLabels);

            DataColumn[] dataColumns = layer.DataSet.GetColumns();
            Assert.AreEqual(2, dataColumns.Length);
            Assert.AreEqual("1", dataColumns[0].ColumnName);
            Assert.AreEqual("2", dataColumns[1].ColumnName);

            Assert.IsNotNull(layer.LabelLayer);
            ILabelCategory labelCategory = layer.LabelLayer.Symbology.Categories[0];
            Assert.AreEqual("FID", labelCategory.Symbolizer.PriorityField);
            Assert.AreEqual(ContentAlignment.MiddleRight, labelCategory.Symbolizer.Orientation);
            Assert.AreEqual(5, labelCategory.Symbolizer.OffsetX);
            Assert.AreEqual(string.Format("[{0}]", selectedAttributeId), labelCategory.Expression);
        }

        [Test]
        public void Convert_RandomPointData_ReturnsNewMapLineLayer()
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

            MapFeature[] mapFeatures =
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
            IMapFeatureLayer layer = converter.Convert(lineData);

            // Assert
            Assert.IsInstanceOf<MapLineLayer>(layer);
        }

        [Test]
        public void Convert_MultipleFeatures_ConvertsAllFeatures()
        {
            // Setup
            var converter = new MapLineDataConverter();
            MapFeature[] features =
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
            IMapFeatureLayer layer = converter.Convert(lineData);

            // Assert
            Assert.AreEqual(features.Length, layer.DataSet.Features.Count);
        }

        [Test]
        public void Convert_MultipleGeometriesInFeature_ReturnsOneFeatureWithAllGeometries()
        {
            // Setup
            var converter = new MapLineDataConverter();
            MapFeature[] features =
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

            MapGeometry[] geometries = features.First().MapGeometries.ToArray();

            var lineData = new MapLineData("test")
            {
                Features = features
            };

            // Call
            IMapFeatureLayer layer = converter.Convert(lineData);

            // Assert
            Assert.AreEqual(features.Length, layer.DataSet.Features.Count);
            layer.DataSet.InitializeVertices();
            List<PartRange> layerGeometries = layer.DataSet.ShapeIndices.First().Parts;
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
            IMapFeatureLayer layer = converter.Convert(data);

            // Assert
            Assert.AreEqual(isVisible, layer.IsVisible);
        }

        [Test]
        public void Convert_DataName_LayerNameSameAsData()
        {
            // Setup
            var name = "<Some name>";
            var converter = new MapLineDataConverter();
            var data = new MapLineData(name);

            // Call
            MapLineLayer layer = (MapLineLayer) converter.Convert(data);

            // Assert
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
            MapLineLayer layer = (MapLineLayer) converter.Convert(data);

            // Assert
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
            MapLineLayer layer = (MapLineLayer) converter.Convert(data);

            // Assert
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
            MapLineLayer layer = (MapLineLayer) converter.Convert(data);

            // Assert
            AssertAreEqual(new LineSymbolizer(Color.AliceBlue, Color.AliceBlue, 3, lineStyle, LineCap.Round), layer.Symbolizer);
        }

        private static void AssertLineStringData(MapFeature mapFeature, IFeature feature)
        {
            Assert.IsInstanceOf<LineString>(feature.BasicGeometry);

            var expectedCoordinates = mapFeature.MapGeometries.First().PointCollections.First().Select(p => new Coordinate(p.X, p.Y));

            CollectionAssert.AreEqual(expectedCoordinates, feature.Coordinates);
        }

        private static MapFeature CreateMultipleGeometryMapFeature(Random random)
        {
            return new MapFeature(new[]
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
            });
        }

        private static MapFeature CreateSingleGeometryMapFeature(Random random)
        {
            return new MapFeature(new[]
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
        }

        private static void AssertAreEqual(ILineSymbolizer firstSymbolizer, ILineSymbolizer secondSymbolizer)
        {
            IList<IStroke> firstStrokes = firstSymbolizer.Strokes;
            IList<IStroke> secondStrokes = secondSymbolizer.Strokes;
            Assert.AreEqual(firstStrokes.Count, secondStrokes.Count, "Unequal amount of strokes defined.");
            for (var i = 0; i < firstStrokes.Count; i++)
            {
                CartographicStroke firstStroke = (CartographicStroke) firstStrokes[i];
                CartographicStroke secondStroke = (CartographicStroke) secondStrokes[i];

                Assert.AreEqual(firstStroke.Color, secondStroke.Color);
                Assert.AreEqual(firstStroke.EndCap, secondStroke.EndCap);
                Assert.AreEqual(firstStroke.DashStyle, secondStroke.DashStyle);
                Assert.AreEqual(firstStroke.Width, secondStroke.Width);
            }
        }
    }
}