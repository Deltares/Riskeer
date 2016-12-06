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
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Components.DotSpatial.Converter;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using Core.Components.Gis.Style;
using DotSpatial.Controls;
using DotSpatial.Data;
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
            Assert.IsInstanceOf<FeatureBasedMapDataConverter<MapPolygonData, MapPolygonLayer>>(converter);
        }

        [Test]
        public void Convert_RandomPolygonData_ReturnsNewMapPolygonLayer()
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

            MapFeature[] mapFeatures = 
            {
                new MapFeature(new[]
                {
                    new MapGeometry(new[]
                    {
                        polygonPoints
                    })
                })
            };

            var polygonData = new MapPolygonData("test data")
            {
                Features = mapFeatures
            };

            // Call
            IMapFeatureLayer layer = converter.Convert(polygonData);

            // Assert
            Assert.AreEqual(1, layer.DataSet.Features.Count);
            Assert.IsInstanceOf<MapPolygonLayer>(layer);
            Assert.AreEqual(FeatureType.Polygon, layer.DataSet.FeatureType);
            
            IEnumerable<Point2D> points = polygonData.Features.First().MapGeometries.First().PointCollections.First();
            List<Point2D> expectedPoints = points.ToList();
            expectedPoints.Add(expectedPoints.First()); // Needed because DotSpatial adds the first point at the end of the collection.
            CollectionAssert.AreEqual(expectedPoints.Select(p => new Coordinate(p.X, p.Y)), layer.DataSet.Features[0].Coordinates);
        }

        [Test]
        public void Convert_MapFeatureWithSimplePolygon_ReturnPolygonLayerWithOnePolygonFeature()
        {
            // Setup
            Point2D[] outerRingPoints = CreateRectangularRing(0.0, 10.0);
            MapFeature[] mapFeatures = 
            {
                new MapFeature(new[]
                {
                    new MapGeometry(new[]
                    {
                        outerRingPoints
                    })
                })
            };
            const string layerName = "test data";
            var polygonData = new MapPolygonData(layerName)
            {
                Features = mapFeatures
            };

            // Call
            PolygonLayer layer = (PolygonLayer) new MapPolygonDataConverter().Convert(polygonData);

            // Assert
            Assert.AreEqual(polygonData.IsVisible, layer.IsVisible);
            Assert.AreEqual(layerName, layer.Name);
            Assert.AreEqual(FeatureType.Polygon, layer.FeatureSet.FeatureType);
            Assert.AreEqual(1, layer.FeatureSet.Features.Count);

            Polygon polygonGeometry = (Polygon) layer.FeatureSet.Features[0].BasicGeometry;
            Assert.AreEqual(1, polygonGeometry.NumGeometries);

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
            MapFeature[] mapFeatures = 
            {
                new MapFeature(new[]
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
            var polygonData = new MapPolygonData(layerName)
            {
                Features = mapFeatures
            };

            // Call
            PolygonLayer layer = (PolygonLayer) new MapPolygonDataConverter().Convert(polygonData);

            // Assert
            Assert.AreEqual(polygonData.IsVisible, layer.IsVisible);
            Assert.AreEqual(layerName, layer.Name);
            Assert.AreEqual(FeatureType.Polygon, layer.FeatureSet.FeatureType);
            Assert.AreEqual(1, layer.FeatureSet.Features.Count);

            Polygon polygonGeometry = (Polygon) layer.FeatureSet.Features[0].BasicGeometry;
            Assert.AreEqual(1, polygonGeometry.NumGeometries);

            CollectionAssert.AreEqual(outerRingPoints, polygonGeometry.Shell.Coordinates.Select(c => new Point2D(c.X, c.Y)));
            Assert.AreEqual(2, polygonGeometry.Holes.Length);
            CollectionAssert.AreEqual(innerRing1Points, polygonGeometry.Holes.ElementAt(0).Coordinates.Select(c => new Point2D(c.X, c.Y)));
            CollectionAssert.AreEqual(innerRing2Points, polygonGeometry.Holes.ElementAt(1).Coordinates.Select(c => new Point2D(c.X, c.Y)));
        }

        [Test]
        public void Convert_MultipleFeatures_ConvertsAllFeatures()
        {
            // Setup
            var converter = new MapPolygonDataConverter();
            MapFeature[] features = 
            {
                new MapFeature(Enumerable.Empty<MapGeometry>()),
                new MapFeature(Enumerable.Empty<MapGeometry>()),
                new MapFeature(Enumerable.Empty<MapGeometry>())
            };

            var polygonData = new MapPolygonData("test")
            {
                Features = features
            };

            // Call
            IMapFeatureLayer layer = converter.Convert(polygonData);

            // Assert
            Assert.AreEqual(features.Length, layer.DataSet.Features.Count);
        }

        [Test]
        public void Convert_MultipleGeometriesInFeature_ReturnsOneFeatureWithAllGeometries()
        {
            // Setup
            var converter = new MapPolygonDataConverter();
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

            var polygonData = new MapPolygonData("test")
            {
                Features = features
            };

            // Call
            IMapFeatureLayer layer = converter.Convert(polygonData);

            // Assert
            Assert.AreEqual(features.Length, layer.DataSet.Features.Count);
            layer.DataSet.InitializeVertices();
            List<PartRange> layerGeometries = layer.DataSet.ShapeIndices.First().Parts;
            Assert.AreEqual(geometries.Length, layerGeometries.Count);
        }

        [Test]
        public void Convert_MapDataWithSingleGeometry_ReturnMapPolygonLayerWithPolygon()
        {
            // Setup
            var converter = new MapPolygonDataConverter();
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
                        },
                        new[]
                        {
                            new Point2D(random.NextDouble(), random.NextDouble()),
                            new Point2D(random.NextDouble(), random.NextDouble()),
                            new Point2D(random.NextDouble(), random.NextDouble())
                        }
                    })
                })
            };

            var polygonData = new MapPolygonData("test data")
            {
                Features = features
            };

            // Call
            IMapFeatureLayer layer = converter.Convert(polygonData);

            // Assert
            Assert.IsInstanceOf<Polygon>(layer.DataSet.Features[0].BasicGeometry);
        }

        [Test]
        public void Convert_MapDataWithMultipleGeometry_ReturnMapPolygonLayerWithMultiPolygon()
        {
            // Setup
            var converter = new MapPolygonDataConverter();
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
                        },
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
                        },
                        new[]
                        {
                            new Point2D(random.NextDouble(), random.NextDouble()),
                            new Point2D(random.NextDouble(), random.NextDouble()),
                            new Point2D(random.NextDouble(), random.NextDouble())
                        }
                    })
                })
            };

            var lineData = new MapPolygonData("test data")
            {
                Features = features
            };

            // Call
            IMapFeatureLayer layer = converter.Convert(lineData);

            // Assert
            Assert.IsInstanceOf<MultiPolygon>(layer.DataSet.Features[0].BasicGeometry);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Convert_RandomPolygonDataWithoutAttributes_ReturnsNewMapPolygonLayerWithDefaultLabelLayer(bool showLabels)
        {
            // Setup
            var converter = new MapPolygonDataConverter();
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
                            new Point2D(random.NextDouble(), random.NextDouble()),
                            new Point2D(random.NextDouble(), random.NextDouble()),
                            new Point2D(random.NextDouble(), random.NextDouble())
                        }
                    })
                }));
            }

            var polygonData = new MapPolygonData("test data")
            {
                Features = features.ToArray(),
                ShowLabels = showLabels
            };

            // Call
            IMapFeatureLayer layer = converter.Convert(polygonData);

            // Assert
            Assert.AreEqual(polygonData.Features.Length, layer.DataSet.Features.Count);
            Assert.IsInstanceOf<MapPolygonLayer>(layer);
            Assert.AreEqual(FeatureType.Polygon, layer.DataSet.FeatureType);
            CollectionAssert.AreNotEqual(polygonData.Features.First().MapGeometries.First().PointCollections, layer.DataSet.Features[0].Coordinates);
            Assert.AreEqual(showLabels, layer.ShowLabels);
            CollectionAssert.IsEmpty(layer.DataSet.GetColumns());

            Assert.IsNotNull(layer.LabelLayer);
            Assert.AreEqual("FID", layer.LabelLayer.Symbology.Categories[0].Symbolizer.PriorityField);
            Assert.IsNull(layer.LabelLayer.Symbology.Categories[0].Expression);
        }

        [Test]
        public void Convert_RandomPolygonDataWithAttributesShowLabelsFalse_ReturnsNewMapPolygonLayerWithDefaultLabelLayer()
        {
            // Setup
            var converter = new MapPolygonDataConverter();
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
                            new Point2D(random.NextDouble(), random.NextDouble()),
                            new Point2D(random.NextDouble(), random.NextDouble()),
                            new Point2D(random.NextDouble(), random.NextDouble())
                        }
                    })
                });
                mapFeature.MetaData["ID"] = random.NextDouble();
                mapFeature.MetaData["Name"] = string.Format("feature [{0}]", i);

                features.Add(mapFeature);
            }

            var polygonData = new MapPolygonData("test data")
            {
                Features = features.ToArray(),
                ShowLabels = false
            };

            // Call
            IMapFeatureLayer layer = converter.Convert(polygonData);

            // Assert
            Assert.AreEqual(polygonData.Features.Length, layer.DataSet.Features.Count);
            Assert.IsInstanceOf<MapPolygonLayer>(layer);
            Assert.AreEqual(FeatureType.Polygon, layer.DataSet.FeatureType);
            CollectionAssert.AreNotEqual(polygonData.Features.First().MapGeometries.First().PointCollections, layer.DataSet.Features[0].Coordinates);
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
        public void Convert_RandomPolygonDataWithAttributesShowLabelsTrue_ReturnsNewMapPolygonLayerWithCustomLabelLayer(string selectedAttribute, int selectedAttributeId)
        {
            // Setup
            var converter = new MapPolygonDataConverter();
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
                            new Point2D(random.NextDouble(), random.NextDouble()),
                            new Point2D(random.NextDouble(), random.NextDouble()),
                            new Point2D(random.NextDouble(), random.NextDouble())
                        }
                    })
                });
                mapFeature.MetaData["ID"] = random.NextDouble();
                mapFeature.MetaData["Name"] = string.Format("feature [{0}]", i);

                features.Add(mapFeature);
            }

            var polygonData = new MapPolygonData("test data")
            {
                Features = features.ToArray(),
                ShowLabels = true,
                SelectedMetaDataAttribute = selectedAttribute
            };

            // Call
            IMapFeatureLayer layer = converter.Convert(polygonData);

            // Assert
            Assert.AreEqual(polygonData.Features.Length, layer.DataSet.Features.Count);
            Assert.IsInstanceOf<MapPolygonLayer>(layer);
            Assert.AreEqual(FeatureType.Polygon, layer.DataSet.FeatureType);
            CollectionAssert.AreNotEqual(polygonData.Features.First().MapGeometries.First().PointCollections, layer.DataSet.Features[0].Coordinates);
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
        [TestCase(KnownColor.AliceBlue)]
        [TestCase(KnownColor.Azure)]
        [TestCase(KnownColor.Beige)]
        public void Convert_WithDifferentFillColors_AppliesStyleToLayer(KnownColor color)
        {
            // Setup
            var converter = new MapPolygonDataConverter();
            var expectedColor = Color.FromKnownColor(color);
            var style = new PolygonStyle(expectedColor, Color.AliceBlue, 3);
            var data = new MapPolygonData("test")
            {
                Style = style
            };

            // Call
            MapPolygonLayer layer = (MapPolygonLayer) converter.Convert(data);

            // Assert
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
            var data = new MapPolygonData("test")
            {
                Style = style
            };

            // Call
            MapPolygonLayer layer = (MapPolygonLayer) converter.Convert(data);

            // Assert
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
            var data = new MapPolygonData("test")
            {
                Style = style
            };

            // Call
            MapPolygonLayer layer = (MapPolygonLayer) converter.Convert(data);

            // Assert
            AssertAreEqual(new PolygonSymbolizer(Color.AliceBlue, Color.AliceBlue, width), layer.Symbolizer);
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

        private static void AssertAreEqual(IPolygonSymbolizer firstSymbolizer, IPolygonSymbolizer secondSymbolizer)
        {
            IList<IPattern> firstSymbols = firstSymbolizer.Patterns;
            IList<IPattern> secondSymbols = secondSymbolizer.Patterns;
            Assert.AreEqual(firstSymbols.Count, secondSymbols.Count, "Unequal amount of strokes defined.");
            for (var i = 0; i < firstSymbols.Count; i++)
            {
                SimplePattern firstStroke = (SimplePattern) firstSymbols[i];
                SimplePattern secondStroke = (SimplePattern) secondSymbols[i];

                Assert.AreEqual(firstStroke.FillColor, secondStroke.FillColor);
                Assert.AreEqual(firstStroke.Outline.GetFillColor(), secondStroke.Outline.GetFillColor());
                Assert.AreEqual(firstStroke.Outline.GetWidth(), secondStroke.Outline.GetWidth());
            }
        }
    }
}