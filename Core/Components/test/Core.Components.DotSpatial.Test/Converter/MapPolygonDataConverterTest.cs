// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Drawing;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Components.DotSpatial.Converter;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using Core.Components.Gis.Style;
using Core.Components.Gis.TestUtil;
using Core.Components.Gis.Theme;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;
using DotSpatial.Topology;
using NUnit.Framework;
using Rhino.Mocks;

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
            Assert.IsInstanceOf<FeatureBasedMapDataConverter<MapPolygonData, MapPolygonLayer, PolygonCategoryTheme>>(converter);
        }

        [Test]
        public void ConvertLayerFeatures_MapPolygonDataWithMultipleFeatures_ConvertsAllFeaturesToMapPolygonLayer()
        {
            // Setup
            var converter = new MapPolygonDataConverter();
            var mapPolygonLayer = new MapPolygonLayer();
            var mapPolygonData = new MapPolygonData("test")
            {
                Features = new[]
                {
                    new MapFeature(Enumerable.Empty<MapGeometry>()),
                    new MapFeature(Enumerable.Empty<MapGeometry>()),
                    new MapFeature(Enumerable.Empty<MapGeometry>())
                }
            };

            // Call
            converter.ConvertLayerFeatures(mapPolygonData, mapPolygonLayer);

            // Assert
            Assert.AreEqual(mapPolygonData.Features.Count(), mapPolygonLayer.DataSet.Features.Count);
        }

        [Test]
        public void ConvertLayerFeatures_MapPolygonDataWithSingleGeometryFeature_ConvertsFeatureToMapPolygonLayerAsPolygonData()
        {
            // Setup
            var converter = new MapPolygonDataConverter();
            var mapPolygonLayer = new MapPolygonLayer();
            var mapPolygonData = new MapPolygonData("test data")
            {
                Features = new[]
                {
                    new MapFeature(new[]
                    {
                        new MapGeometry(new[]
                        {
                            CreateRectangularRing(0.0, 10.0)
                        })
                    })
                }
            };

            // Call
            converter.ConvertLayerFeatures(mapPolygonData, mapPolygonLayer);

            // Assert
            IFeature feature = mapPolygonLayer.DataSet.Features[0];
            Assert.AreEqual(mapPolygonData.Features.Count(), mapPolygonLayer.DataSet.Features.Count);
            Assert.IsInstanceOf<Polygon>(feature.BasicGeometry);

            IEnumerable<Coordinate> expectedCoordinates = mapPolygonData.Features.First().MapGeometries.First().PointCollections.First().Select(p => new Coordinate(p.X, p.Y));
            CollectionAssert.AreEqual(expectedCoordinates, mapPolygonLayer.DataSet.Features[0].Coordinates);
        }

        [Test]
        public void ConvertLayerFeatures_MapPolygonDataWithMultipleGeometryFeature_ConvertsFeatureToMapPolygonLayerAsMultiPolygonData()
        {
            // Setup
            var converter = new MapPolygonDataConverter();
            var mapPolygonLayer = new MapPolygonLayer();
            var mapFeature = new MapFeature(new[]
            {
                new MapGeometry(new[]
                {
                    CreateRectangularRing(2.0, 4.0)
                }),
                new MapGeometry(new[]
                {
                    CreateRectangularRing(6.0, 8.0)
                })
            });

            var mapPolygonData = new MapPolygonData("test data")
            {
                Features = new[]
                {
                    mapFeature
                }
            };

            // Call
            converter.ConvertLayerFeatures(mapPolygonData, mapPolygonLayer);

            // Assert
            IFeature feature = mapPolygonLayer.DataSet.Features[0];
            Assert.AreEqual(mapPolygonData.Features.Count(), mapPolygonLayer.DataSet.Features.Count);
            Assert.IsInstanceOf<MultiPolygon>(feature.BasicGeometry);

            IEnumerable<Coordinate> expectedCoordinates = mapFeature.MapGeometries.SelectMany(mg => mg.PointCollections.ElementAt(0).Select(p => new Coordinate(p.X, p.Y)));
            CollectionAssert.AreEqual(expectedCoordinates, feature.Coordinates);
        }

        [Test]
        public void ConvertLayerFeatures_MapPolygonDataWithRingFeature_ConvertsFeatureToMapPolygonLayerAsPolygonData()
        {
            // Setup
            var converter = new MapPolygonDataConverter();
            var mapPolygonLayer = new MapPolygonLayer();
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
            var mapPolygonData = new MapPolygonData("test data")
            {
                Features = mapFeatures
            };

            // Call
            converter.ConvertLayerFeatures(mapPolygonData, mapPolygonLayer);

            // Assert
            IFeature feature = mapPolygonLayer.DataSet.Features[0];
            Assert.AreEqual(mapPolygonData.Features.Count(), mapPolygonLayer.DataSet.Features.Count);
            Assert.IsInstanceOf<Polygon>(feature.BasicGeometry);

            var polygonGeometry = (Polygon) mapPolygonLayer.FeatureSet.Features[0].BasicGeometry;
            Assert.AreEqual(1, polygonGeometry.NumGeometries);
            CollectionAssert.AreEqual(outerRingPoints, polygonGeometry.Shell.Coordinates.Select(c => new Point2D(c.X, c.Y)));
            CollectionAssert.IsEmpty(polygonGeometry.Holes);
        }

        [Test]
        public void ConvertLayerFeatures_MapPolygonDataWithRingFeatureAndHoles_ConvertsFeatureToMapPolygonLayerAsPolygonData()
        {
            // Setup
            var converter = new MapPolygonDataConverter();
            var mapPolygonLayer = new MapPolygonLayer();
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
            var mapPolygonData = new MapPolygonData("test data")
            {
                Features = mapFeatures
            };

            // Call
            converter.ConvertLayerFeatures(mapPolygonData, mapPolygonLayer);

            // Assert
            IFeature feature = mapPolygonLayer.DataSet.Features[0];
            Assert.AreEqual(mapPolygonData.Features.Count(), mapPolygonLayer.DataSet.Features.Count);
            Assert.IsInstanceOf<Polygon>(feature.BasicGeometry);

            var polygonGeometry = (Polygon) mapPolygonLayer.FeatureSet.Features[0].BasicGeometry;
            Assert.AreEqual(1, polygonGeometry.NumGeometries);
            CollectionAssert.AreEqual(outerRingPoints, polygonGeometry.Shell.Coordinates.Select(c => new Point2D(c.X, c.Y)));
            Assert.AreEqual(2, polygonGeometry.Holes.Length);
            CollectionAssert.AreEqual(innerRing1Points, polygonGeometry.Holes.ElementAt(0).Coordinates.Select(c => new Point2D(c.X, c.Y)));
            CollectionAssert.AreEqual(innerRing2Points, polygonGeometry.Holes.ElementAt(1).Coordinates.Select(c => new Point2D(c.X, c.Y)));
        }

        [Test]
        public void GivenMapLayerWithScheme_WhenConvertingLayerFeatures_ThenClearsAppliedSchemeAndAppliesDefaultCategory()
        {
            // Given
            const string metadataAttribute = "Meta";

            var mocks = new MockRepository();
            var categoryOne = mocks.Stub<IPolygonCategory>();
            var categoryTwo = mocks.Stub<IPolygonCategory>();
            mocks.ReplayAll();

            var mapPolygonLayer = new MapPolygonLayer
            {
                Symbology = new PolygonScheme
                {
                    Categories =
                    {
                        categoryOne,
                        categoryTwo
                    }
                }
            };

            var converter = new MapPolygonDataConverter();

            var random = new Random(21);
            var polygonStyle = new PolygonStyle
            {
                FillColor = Color.FromKnownColor(random.NextEnum<KnownColor>()),
                StrokeColor = Color.FromKnownColor(random.NextEnum<KnownColor>()),
                StrokeThickness = random.Next(1, 48)
            };
            var theme = new MapTheme<PolygonCategoryTheme>(metadataAttribute, new[]
            {
                new PolygonCategoryTheme(ValueCriterionTestFactory.CreateValueCriterion(),
                                         new PolygonStyle
                                         {
                                             FillColor = Color.FromKnownColor(random.NextEnum<KnownColor>()),
                                             StrokeColor = Color.FromKnownColor(random.NextEnum<KnownColor>()),
                                             StrokeThickness = random.Next(1, 48)
                                         })
            });
            var mapPolygonData = new MapPolygonData("test", polygonStyle, theme)
            {
                Features = new[]
                {
                    CreateMapFeatureWithMetaData(metadataAttribute)
                }
            };

            // When
            converter.ConvertLayerFeatures(mapPolygonData, mapPolygonLayer);

            // Then
            PolygonCategoryCollection categoryCollection = mapPolygonLayer.Symbology.Categories;
            Assert.AreEqual(1, categoryCollection.Count);

            PolygonSymbolizer expectedSymbolizer = CreateExpectedSymbolizer(polygonStyle);
            AssertAreEqual(expectedSymbolizer, categoryCollection.Single().Symbolizer);

            mocks.VerifyAll();
        }

        [Test]
        [Combinatorial]
        public void ConvertLayerProperties_MapPolygonDataWithStyle_ConvertsStyleToMapPolygonLayer(
            [Values(KnownColor.AliceBlue, KnownColor.Azure)]
            KnownColor fillColor,
            [Values(KnownColor.AppWorkspace, KnownColor.BlueViolet)]
            KnownColor outlineFillColor,
            [Values(1, 5)] int width)
        {
            // Setup
            var converter = new MapPolygonDataConverter();
            var mapPolygonLayer = new MapPolygonLayer();
            Color expectedFillColor = Color.FromKnownColor(fillColor);
            Color expectedOutlineFillColor = Color.FromKnownColor(outlineFillColor);
            var mapPolygonData = new MapPolygonData("test", new PolygonStyle
            {
                FillColor = expectedFillColor,
                StrokeColor = expectedOutlineFillColor,
                StrokeThickness = width
            });

            // Call
            converter.ConvertLayerProperties(mapPolygonData, mapPolygonLayer);

            // Assert
            AssertAreEqual(new PolygonSymbolizer(expectedFillColor, expectedOutlineFillColor, width), mapPolygonLayer.Symbolizer);
        }

        [Test]
        [Combinatorial]
        public void ConvertLayerProperties_MapPolygonDataWithStrokeThicknessZero_MapPolygonLayerOutlineColorTransparent()
        {
            // Setup
            var converter = new MapPolygonDataConverter();
            var mapPolygonLayer = new MapPolygonLayer();
            Color expectedFillColor = Color.FromKnownColor(new Random(21).NextEnum<KnownColor>());
            var mapPolygonData = new MapPolygonData("test", new PolygonStyle
            {
                FillColor = expectedFillColor,
                StrokeColor = Color.ForestGreen,
                StrokeThickness = 0
            });

            // Call
            converter.ConvertLayerProperties(mapPolygonData, mapPolygonLayer);

            // Assert
            AssertAreEqual(new PolygonSymbolizer(expectedFillColor, Color.Transparent, 0), mapPolygonLayer.Symbolizer);
        }

        [Test]
        public void ConvertLayerProperties_MapPolygonDataWithThemeAndMetaDataNameNotInFeatures_OnlyAddsDefaultCategory()
        {
            // Setup
            const string metadataAttribute = "Meta";
            var random = new Random(21);
            var theme = new MapTheme<PolygonCategoryTheme>("Other Meta", new[]
            {
                new PolygonCategoryTheme(ValueCriterionTestFactory.CreateValueCriterion(),
                                         new PolygonStyle
                                         {
                                             FillColor = Color.FromKnownColor(random.NextEnum<KnownColor>()),
                                             StrokeColor = Color.FromKnownColor(random.NextEnum<KnownColor>()),
                                             StrokeThickness = random.Next(1, 48)
                                         })
            });

            var polygonStyle = new PolygonStyle
            {
                FillColor = Color.FromKnownColor(random.NextEnum<KnownColor>()),
                StrokeColor = Color.FromKnownColor(random.NextEnum<KnownColor>()),
                StrokeThickness = random.Next(1, 48)
            };
            var mapPolygonData = new MapPolygonData("test", polygonStyle, theme)
            {
                Features = new[]
                {
                    CreateMapFeatureWithMetaData(metadataAttribute)
                }
            };

            var mapPolygonLayer = new MapPolygonLayer();

            var converter = new MapPolygonDataConverter();

            // Call
            converter.ConvertLayerProperties(mapPolygonData, mapPolygonLayer);

            // Assert
            IPolygonSymbolizer expectedSymbolizer = CreateExpectedSymbolizer(polygonStyle);

            IPolygonScheme appliedScheme = mapPolygonLayer.Symbology;
            Assert.AreEqual(1, appliedScheme.Categories.Count);

            IPolygonCategory baseCategory = appliedScheme.Categories[0];
            AssertAreEqual(expectedSymbolizer, baseCategory.Symbolizer);
            Assert.IsNull(baseCategory.FilterExpression);
        }

        [Test]
        public void ConvertLayerProperties_MapPolygonDataWithThemeAndMetaDataNameInFeatures_ConvertDataToMapPolygonLayer()
        {
            // Setup
            const string metadataAttribute = "Meta";
            var random = new Random(21);

            var unequalCriterion = new ValueCriterion(ValueCriterionOperator.UnequalValue,
                                                      "unequal value");
            var equalCriterion = new ValueCriterion(ValueCriterionOperator.EqualValue,
                                                    "equal value");
            var theme = new MapTheme<PolygonCategoryTheme>(metadataAttribute, new[]
            {
                new PolygonCategoryTheme(equalCriterion, new PolygonStyle
                {
                    FillColor = Color.FromKnownColor(random.NextEnum<KnownColor>()),
                    StrokeColor = Color.FromKnownColor(random.NextEnum<KnownColor>()),
                    StrokeThickness = random.Next(1, 48)
                }),
                new PolygonCategoryTheme(unequalCriterion, new PolygonStyle
                {
                    FillColor = Color.FromKnownColor(random.NextEnum<KnownColor>()),
                    StrokeColor = Color.FromKnownColor(random.NextEnum<KnownColor>()),
                    StrokeThickness = random.Next(1, 48)
                })
            });

            var polygonStyle = new PolygonStyle
            {
                FillColor = Color.FromKnownColor(random.NextEnum<KnownColor>()),
                StrokeColor = Color.FromKnownColor(random.NextEnum<KnownColor>()),
                StrokeThickness = random.Next(1, 48)
            };
            var mapPolygonData = new MapPolygonData("test", polygonStyle, theme)
            {
                Features = new[]
                {
                    CreateMapFeatureWithMetaData(metadataAttribute)
                }
            };

            var mapPolygonLayer = new MapPolygonLayer();

            var converter = new MapPolygonDataConverter();

            // Call
            converter.ConvertLayerProperties(mapPolygonData, mapPolygonLayer);

            // Assert
            PolygonSymbolizer expectedSymbolizer = CreateExpectedSymbolizer(polygonStyle);

            IPolygonScheme appliedScheme = mapPolygonLayer.Symbology;
            Assert.AreEqual(3, appliedScheme.Categories.Count);

            IPolygonCategory baseCategory = appliedScheme.Categories[0];
            AssertAreEqual(expectedSymbolizer, baseCategory.Symbolizer);
            Assert.IsNull(baseCategory.FilterExpression);

            IPolygonCategory equalSchemeCategory = appliedScheme.Categories[1];
            string expectedFilter = $"[1] = '{equalCriterion.Value}'";
            Assert.AreEqual(expectedFilter, equalSchemeCategory.FilterExpression);
            PolygonStyle expectedCategoryStyle = theme.CategoryThemes.ElementAt(0).Style;
            expectedSymbolizer = CreateExpectedSymbolizer(expectedCategoryStyle);
            AssertAreEqual(expectedSymbolizer, equalSchemeCategory.Symbolizer);

            IPolygonCategory unEqualSchemeCategory = appliedScheme.Categories[2];
            expectedFilter = $"NOT [1] = '{unequalCriterion.Value}'";
            Assert.AreEqual(expectedFilter, unEqualSchemeCategory.FilterExpression);
            expectedCategoryStyle = theme.CategoryThemes.ElementAt(1).Style;
            expectedSymbolizer = CreateExpectedSymbolizer(expectedCategoryStyle);
            AssertAreEqual(expectedSymbolizer, unEqualSchemeCategory.Symbolizer);
        }

        private static MapFeature CreateMapFeatureWithMetaData(string metadataAttributeName)
        {
            var random = new Random(21);
            var mapFeature = new MapFeature(new[]
            {
                new MapGeometry(new[]
                {
                    new[]
                    {
                        new Point2D(random.NextDouble(), random.NextDouble()),
                        new Point2D(random.NextDouble(), random.NextDouble())
                    }
                })
            });
            mapFeature.MetaData[metadataAttributeName] = new object();

            return mapFeature;
        }

        private static PolygonSymbolizer CreateExpectedSymbolizer(PolygonStyle expectedPolygonStyle)
        {
            return new PolygonSymbolizer(expectedPolygonStyle.FillColor,
                                         expectedPolygonStyle.StrokeColor,
                                         expectedPolygonStyle.StrokeThickness);
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
            IList<IPattern> firstPatterns = firstSymbolizer.Patterns;
            IList<IPattern> secondPatterns = secondSymbolizer.Patterns;
            Assert.AreEqual(firstPatterns.Count, secondPatterns.Count, "Unequal amount of patterns defined.");
            for (var i = 0; i < firstPatterns.Count; i++)
            {
                var firstPattern = (SimplePattern) firstPatterns[i];
                var secondPattern = (SimplePattern) secondPatterns[i];

                Assert.AreEqual(firstPattern.FillColor, secondPattern.FillColor);
                Assert.AreEqual(firstPattern.Outline.GetFillColor(), secondPattern.Outline.GetFillColor());
                Assert.AreEqual(firstPattern.Outline.GetWidth(), secondPattern.Outline.GetWidth());
            }
        }
    }
}