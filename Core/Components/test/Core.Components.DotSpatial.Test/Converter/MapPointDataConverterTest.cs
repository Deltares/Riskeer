// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Point = DotSpatial.Topology.Point;
using PointShape = DotSpatial.Symbology.PointShape;

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
            Assert.IsInstanceOf<FeatureBasedMapDataConverter<MapPointData, MapPointLayer, PointCategoryTheme>>(converter);
        }

        [Test]
        public void ConvertLayerFeatures_MapPointDataWithMultipleFeatures_ConvertsEachGeometryToMapPointLayerAsSingleFeature()
        {
            // Setup
            var converter = new MapPointDataConverter();
            var mapPointLayer = new MapPointLayer();
            var mapPointData = new MapPointData("test")
            {
                Features = new[]
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
                    new MapFeature(Enumerable.Empty<MapGeometry>()),
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
                    new MapFeature(Enumerable.Empty<MapGeometry>()),
                    new MapFeature(new[]
                    {
                        new MapGeometry(new[]
                        {
                            new[]
                            {
                                new Point2D(3, 4)
                            }
                        }),
                        new MapGeometry(new[]
                        {
                            new[]
                            {
                                new Point2D(5, 6)
                            }
                        })
                    })
                }
            };

            // Call
            converter.ConvertLayerFeatures(mapPointData, mapPointLayer);

            // Assert
            Assert.AreEqual(4, mapPointLayer.DataSet.Features.Count);
        }

        [Test]
        public void ConvertLayerFeatures_MapPointDataWithFeature_ConvertsFeatureToMapPointLayerAsPointData()
        {
            // Setup
            var converter = new MapPointDataConverter();
            var mapPointLayer = new MapPointLayer();
            var mapFeature = new MapFeature(new[]
            {
                new MapGeometry(new[]
                {
                    new[]
                    {
                        new Point2D(1, 2)
                    }
                })
            });

            var mapPointData = new MapPointData("test")
            {
                Features = new[]
                {
                    mapFeature
                }
            };

            // Call
            converter.ConvertLayerFeatures(mapPointData, mapPointLayer);

            // Assert
            IFeature feature = mapPointLayer.DataSet.Features[0];
            Assert.AreEqual(mapPointData.Features.Count(), mapPointLayer.DataSet.Features.Count);
            Assert.IsInstanceOf<Point>(feature.BasicGeometry);

            IEnumerable<Coordinate> expectedCoordinates = mapFeature.MapGeometries.ElementAt(0).PointCollections.ElementAt(0).Select(p => new Coordinate(p.X, p.Y));
            CollectionAssert.AreEqual(expectedCoordinates, feature.Coordinates);
        }

        [Test]
        public void GivenMapLayerWithScheme_WhenConvertingLayerFeatures_ThenClearsAppliedSchemeAndAppliesDefaultCategory()
        {
            // Given
            const string metadataAttribute = "Meta";

            var mocks = new MockRepository();
            var categoryOne = mocks.Stub<IPointCategory>();
            var categoryTwo = mocks.Stub<IPointCategory>();
            mocks.ReplayAll();

            var mapPointLayer = new MapPointLayer
            {
                Symbology = new PointScheme
                {
                    Categories =
                    {
                        categoryOne,
                        categoryTwo
                    }
                }
            };

            var converter = new MapPointDataConverter();

            var random = new Random(21);
            var pointStyle = new PointStyle
            {
                Color = Color.FromKnownColor(random.NextEnum<KnownColor>()),
                Size = random.Next(1, 48),
                Symbol = random.NextEnum<PointSymbol>(),
                StrokeColor = Color.FromKnownColor(random.NextEnum<KnownColor>()),
                StrokeThickness = random.Next(1, 48)
            };
            var theme = new MapTheme<PointCategoryTheme>(metadataAttribute, new[]
            {
                new PointCategoryTheme(ValueCriterionTestFactory.CreateValueCriterion(),
                                       new PointStyle
                                       {
                                           Color = Color.FromKnownColor(random.NextEnum<KnownColor>()),
                                           Size = random.Next(1, 48),
                                           Symbol = random.NextEnum<PointSymbol>(),
                                           StrokeColor = Color.FromKnownColor(random.NextEnum<KnownColor>()),
                                           StrokeThickness = random.Next(1, 48)
                                       })
            });
            var mapPointData = new MapPointData("test", pointStyle, theme)
            {
                Features = new[]
                {
                    CreateMapFeatureWithMetaData(metadataAttribute)
                }
            };

            // When
            converter.ConvertLayerFeatures(mapPointData, mapPointLayer);

            // Then
            PointCategoryCollection categoryCollection = mapPointLayer.Symbology.Categories;
            Assert.AreEqual(1, categoryCollection.Count);

            IPointSymbolizer expectedSymbolizer = CreateExpectedSymbolizer(pointStyle);
            AssertAreEqual(expectedSymbolizer, categoryCollection.Single().Symbolizer);

            mocks.VerifyAll();
        }

        [Test]
        [Combinatorial]
        public void ConvertLayerProperties_MapPointDataWithStyle_ConvertsStyleToMapPointLayer(
            [Values(KnownColor.AliceBlue, KnownColor.Azure)]
            KnownColor color,
            [Values(1, 5)] int width,
            [Values(PointSymbol.Circle, PointSymbol.Square)]
            PointSymbol pointStyle)
        {
            // Setup
            var converter = new MapPointDataConverter();
            var mapPointLayer = new MapPointLayer();
            Color expectedColor = Color.FromKnownColor(color);
            var mapPointData = new MapPointData("test", new PointStyle
            {
                Color = expectedColor,
                Size = width,
                Symbol = pointStyle,
                StrokeColor = expectedColor,
                StrokeThickness = 1
            });

            // Call
            converter.ConvertLayerProperties(mapPointData, mapPointLayer);

            // Assert
            PointShape expectedPointShape = pointStyle == PointSymbol.Circle
                                                ? PointShape.Ellipse
                                                : PointShape.Rectangle;

            var expectedSymbolizer = new PointSymbolizer(expectedColor, expectedPointShape, width);
            expectedSymbolizer.SetOutline(expectedColor, 1);

            AssertAreEqual(expectedSymbolizer, mapPointLayer.Symbolizer);
        }

        [Test]
        public void ConvertLayerProperties_MapPointDataWithThemeAndMetaDataNameNotInFeatures_OnlyAddsDefaultCategory()
        {
            // Setup
            const string metadataAttribute = "Meta";
            var random = new Random(21);
            var theme = new MapTheme<PointCategoryTheme>("Other Meta", new[]
            {
                new PointCategoryTheme(ValueCriterionTestFactory.CreateValueCriterion(),
                                       new PointStyle
                                       {
                                           Color = Color.FromKnownColor(random.NextEnum<KnownColor>()),
                                           Size = random.Next(1, 48),
                                           Symbol = random.NextEnum<PointSymbol>(),
                                           StrokeColor = Color.FromKnownColor(random.NextEnum<KnownColor>()),
                                           StrokeThickness = random.Next(1, 48)
                                       })
            });

            var pointStyle = new PointStyle
            {
                Color = Color.FromKnownColor(random.NextEnum<KnownColor>()),
                Size = random.Next(1, 48),
                Symbol = random.NextEnum<PointSymbol>(),
                StrokeColor = Color.FromKnownColor(random.NextEnum<KnownColor>()),
                StrokeThickness = random.Next(1, 48)
            };
            var mapPointData = new MapPointData("test", pointStyle, theme)
            {
                Features = new[]
                {
                    CreateMapFeatureWithMetaData(metadataAttribute)
                }
            };

            var mapPointLayer = new MapPointLayer();

            var converter = new MapPointDataConverter();

            // Call
            converter.ConvertLayerProperties(mapPointData, mapPointLayer);

            // Assert
            PointSymbolizer expectedSymbolizer = CreateExpectedSymbolizer(pointStyle);

            IPointScheme appliedScheme = mapPointLayer.Symbology;
            Assert.AreEqual(1, appliedScheme.Categories.Count);

            IPointCategory baseCategory = appliedScheme.Categories[0];
            AssertAreEqual(expectedSymbolizer, baseCategory.Symbolizer);
            Assert.IsNull(baseCategory.FilterExpression);
        }

        [Test]
        public void ConvertLayerProperties_MapPointDataWithThemeAndMetaDataNameInFeatures_ConvertDataToMapPointLayer()
        {
            // Setup
            const string metadataAttribute = "Meta";
            var random = new Random(21);

            var unequalCriterion = new ValueCriterion(ValueCriterionOperator.UnequalValue,
                                                      "unequal value");
            var equalCriterion = new ValueCriterion(ValueCriterionOperator.EqualValue,
                                                    "equal value");
            var theme = new MapTheme<PointCategoryTheme>(metadataAttribute, new[]
            {
                new PointCategoryTheme(equalCriterion, new PointStyle
                {
                    Color = Color.FromKnownColor(random.NextEnum<KnownColor>()),
                    Size = random.Next(1, 48),
                    Symbol = random.NextEnum<PointSymbol>(),
                    StrokeColor = Color.FromKnownColor(random.NextEnum<KnownColor>()),
                    StrokeThickness = random.Next(1, 48)
                }),
                new PointCategoryTheme(unequalCriterion, new PointStyle
                {
                    Color = Color.FromKnownColor(random.NextEnum<KnownColor>()),
                    Size = random.Next(1, 48),
                    Symbol = random.NextEnum<PointSymbol>(),
                    StrokeColor = Color.FromKnownColor(random.NextEnum<KnownColor>()),
                    StrokeThickness = random.Next(1, 48)
                })
            });

            var pointStyle = new PointStyle
            {
                Color = Color.FromKnownColor(random.NextEnum<KnownColor>()),
                Size = random.Next(1, 48),
                Symbol = random.NextEnum<PointSymbol>(),
                StrokeColor = Color.FromKnownColor(random.NextEnum<KnownColor>()),
                StrokeThickness = random.Next(1, 48)
            };
            var mapPointData = new MapPointData("test", pointStyle, theme)
            {
                Features = new[]
                {
                    CreateMapFeatureWithMetaData(metadataAttribute)
                }
            };

            var mapPointLayer = new MapPointLayer();

            var converter = new MapPointDataConverter();

            // Call
            converter.ConvertLayerProperties(mapPointData, mapPointLayer);

            // Assert
            PointSymbolizer expectedSymbolizer = CreateExpectedSymbolizer(pointStyle);

            IPointScheme appliedScheme = mapPointLayer.Symbology;
            Assert.AreEqual(3, appliedScheme.Categories.Count);

            IPointCategory baseCategory = appliedScheme.Categories[0];
            AssertAreEqual(expectedSymbolizer, baseCategory.Symbolizer);
            Assert.IsNull(baseCategory.FilterExpression);

            IPointCategory equalSchemeCategory = appliedScheme.Categories[1];
            string expectedFilter = $"[1] = '{equalCriterion.Value}'";
            Assert.AreEqual(expectedFilter, equalSchemeCategory.FilterExpression);
            PointStyle expectedCategoryStyle = theme.CategoryThemes.ElementAt(0).Style;
            expectedSymbolizer = CreateExpectedSymbolizer(expectedCategoryStyle);
            AssertAreEqual(expectedSymbolizer, equalSchemeCategory.Symbolizer);

            IPointCategory unEqualSchemeCategory = appliedScheme.Categories[2];
            expectedFilter = $"NOT [1] = '{unequalCriterion.Value}'";
            Assert.AreEqual(expectedFilter, unEqualSchemeCategory.FilterExpression);
            expectedCategoryStyle = theme.CategoryThemes.ElementAt(1).Style;
            expectedSymbolizer = CreateExpectedSymbolizer(expectedCategoryStyle);
            AssertAreEqual(expectedSymbolizer, unEqualSchemeCategory.Symbolizer);
        }

        private static PointSymbolizer CreateExpectedSymbolizer(PointStyle expectedPointStyle)
        {
            PointShape expectedPointShape = MapDataHelper.Convert(expectedPointStyle.Symbol);

            var expectedSymbolizer = new PointSymbolizer(expectedPointStyle.Color, expectedPointShape, expectedPointStyle.Size);
            expectedSymbolizer.SetOutline(expectedPointStyle.StrokeColor, expectedPointStyle.StrokeThickness);

            return expectedSymbolizer;
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
                        new Point2D(random.NextDouble(), random.NextDouble())
                    }
                })
            });
            mapFeature.MetaData[metadataAttributeName] = new object();

            return mapFeature;
        }

        private static void AssertAreEqual(IPointSymbolizer expectedSymbolizer, IPointSymbolizer actualSymbolizer)
        {
            IList<ISymbol> firstSymbols = expectedSymbolizer.Symbols;
            IList<ISymbol> secondSymbols = actualSymbolizer.Symbols;
            Assert.AreEqual(firstSymbols.Count, secondSymbols.Count, "Unequal amount of symbols defined.");
            for (var i = 0; i < firstSymbols.Count; i++)
            {
                var firstSymbol = (SimpleSymbol) firstSymbols[i];
                var secondSymbol = (SimpleSymbol) secondSymbols[i];

                Assert.AreEqual(firstSymbol.Color, secondSymbol.Color);
                Assert.AreEqual(firstSymbol.PointShape, secondSymbol.PointShape);
                Assert.AreEqual(firstSymbol.Size, secondSymbol.Size);
                Assert.AreEqual(firstSymbol.OutlineColor, secondSymbol.OutlineColor);
                Assert.AreEqual(firstSymbol.OutlineWidth, secondSymbol.OutlineWidth);
            }
        }
    }
}