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
using System.Drawing.Drawing2D;
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
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using NUnit.Framework;
using Rhino.Mocks;
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
            Assert.IsInstanceOf<FeatureBasedMapDataConverter<MapLineData, MapLineLayer, LineCategoryTheme>>(converter);
        }

        [Test]
        public void ConvertLayerFeatures_MapLineDataWithMultipleFeatures_ConvertsAllFeaturesToMapLineLayer()
        {
            // Setup
            var converter = new MapLineDataConverter();
            var mapLineLayer = new MapLineLayer();
            var mapLineData = new MapLineData("test")
            {
                Features = new[]
                {
                    new MapFeature(Enumerable.Empty<MapGeometry>()),
                    new MapFeature(Enumerable.Empty<MapGeometry>()),
                    new MapFeature(Enumerable.Empty<MapGeometry>())
                }
            };

            // Call
            converter.ConvertLayerFeatures(mapLineData, mapLineLayer);

            // Assert
            Assert.AreEqual(mapLineData.Features.Count(), mapLineLayer.DataSet.Features.Count);
        }

        [Test]
        public void ConvertLayerFeatures_MapLineDataWithSingleGeometryFeature_ConvertsFeaturesToMapLineLayerAsLineStringData()
        {
            // Setup
            var converter = new MapLineDataConverter();
            var mapLineLayer = new MapLineLayer();
            var random = new Random(21);
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

            var mapLineData = new MapLineData("test data")
            {
                Features = new[]
                {
                    mapFeature
                }
            };

            // Call
            converter.ConvertLayerFeatures(mapLineData, mapLineLayer);

            // Assert
            IFeature feature = mapLineLayer.DataSet.Features[0];
            Assert.AreEqual(mapLineData.Features.Count(), mapLineLayer.DataSet.Features.Count);
            Assert.IsInstanceOf<LineString>(feature.Geometry);

            IEnumerable<Coordinate> expectedCoordinates = mapFeature.MapGeometries.ElementAt(0).PointCollections.ElementAt(0)
                                                                    .Select(p => new Coordinate(p.X, p.Y));
            CollectionAssert.AreEqual(expectedCoordinates, feature.Geometry.Coordinates);
        }

        [Test]
        public void ConvertLayerFeatures_MapLineDataWithMultipleGeometryFeature_ConvertsFeaturesToMapLineLayerAsMultiLineStringData()
        {
            // Setup
            var converter = new MapLineDataConverter();
            var mapLineLayer = new MapLineLayer();
            var random = new Random(21);
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

            var mapLineData = new MapLineData("test data")
            {
                Features = new[]
                {
                    mapFeature
                }
            };

            // Call
            converter.ConvertLayerFeatures(mapLineData, mapLineLayer);

            // Assert
            IFeature feature = mapLineLayer.DataSet.Features[0];
            Assert.AreEqual(mapLineData.Features.Count(), mapLineLayer.DataSet.Features.Count);
            Assert.IsInstanceOf<MultiLineString>(feature.Geometry);

            IEnumerable<Coordinate> expectedCoordinates = mapFeature.MapGeometries.SelectMany(mg => mg.PointCollections.ElementAt(0).Select(p => new Coordinate(p.X, p.Y)));
            CollectionAssert.AreEqual(expectedCoordinates, feature.Geometry.Coordinates);
        }

        [Test]
        public void GivenMapLayerWithScheme_WhenConvertingLayerFeatures_ThenClearsAppliedSchemeAndAppliesDefaultCategory()
        {
            // Given
            var mocks = new MockRepository();
            var categoryOne = mocks.Stub<ILineCategory>();
            var categoryTwo = mocks.Stub<ILineCategory>();
            mocks.ReplayAll();

            var mapLineLayer = new MapLineLayer
            {
                Symbology = new LineScheme
                {
                    Categories =
                    {
                        categoryOne,
                        categoryTwo
                    }
                }
            };

            var converter = new MapLineDataConverter();

            var random = new Random(21);
            var lineStyle = new LineStyle
            {
                Color = Color.FromKnownColor(random.NextEnum<KnownColor>()),
                Width = random.Next(1, 48),
                DashStyle = random.NextEnum<LineDashStyle>()
            };
            var theme = new MapTheme<LineCategoryTheme>("Meta", new[]
            {
                new LineCategoryTheme(ValueCriterionTestFactory.CreateValueCriterion(),
                                      new LineStyle
                                      {
                                          Color = Color.FromKnownColor(random.NextEnum<KnownColor>()),
                                          Width = random.Next(1, 48),
                                          DashStyle = random.NextEnum<LineDashStyle>()
                                      })
            });

            var mapLineData = new MapLineData("test", lineStyle, theme)
            {
                Features = new[]
                {
                    CreateMapFeatureWithMetaData("Meta")
                }
            };

            // When
            converter.ConvertLayerFeatures(mapLineData, mapLineLayer);

            // Then
            LineCategoryCollection categoryCollection = mapLineLayer.Symbology.Categories;
            Assert.AreEqual(1, categoryCollection.Count);

            ILineSymbolizer expectedSymbolizer = CreateExpectedSymbolizer(lineStyle);
            AssertAreEqual(expectedSymbolizer, categoryCollection.Single().Symbolizer);

            mocks.VerifyAll();
        }

        [Test]
        [Combinatorial]
        public void ConvertLayerProperties_MapLineDataWithStyle_ConvertsStyleToMapLineLayer(
            [Values(KnownColor.AliceBlue, KnownColor.Azure)]
            KnownColor color,
            [Values(1, 5)] int width,
            [Values(LineDashStyle.Solid, LineDashStyle.Dash)]
            LineDashStyle lineStyle)
        {
            // Setup
            Color expectedColor = Color.FromKnownColor(color);
            var converter = new MapLineDataConverter();
            var mapLineLayer = new MapLineLayer();
            var mapLineData = new MapLineData("test", new LineStyle
            {
                Color = expectedColor,
                Width = width,
                DashStyle = lineStyle
            });

            // Call
            converter.ConvertLayerProperties(mapLineData, mapLineLayer);

            // Assert
            AssertAreEqual(new LineSymbolizer(expectedColor, expectedColor, width, MapDataHelper.Convert(lineStyle), LineCap.Round), mapLineLayer.Symbolizer);
        }

        [Test]
        public void ConvertLayerProperties_MapLineDataWithThemeAndMetaDataNameNotInFeatures_OnlyAddsDefaultCategory()
        {
            // Setup
            const string metadataAttribute = "Meta";
            var random = new Random(21);
            var theme = new MapTheme<LineCategoryTheme>("Other Meta", new[]
            {
                new LineCategoryTheme(ValueCriterionTestFactory.CreateValueCriterion(),
                                      new LineStyle
                                      {
                                          Color = Color.FromKnownColor(random.NextEnum<KnownColor>()),
                                          Width = random.Next(1, 48),
                                          DashStyle = random.NextEnum<LineDashStyle>()
                                      })
            });

            var lineStyle = new LineStyle
            {
                Color = Color.FromKnownColor(random.NextEnum<KnownColor>()),
                Width = random.Next(1, 48),
                DashStyle = random.NextEnum<LineDashStyle>()
            };
            var mapLineData = new MapLineData("test", lineStyle, theme)
            {
                Features = new[]
                {
                    CreateMapFeatureWithMetaData(metadataAttribute)
                }
            };

            var mapLineLayer = new MapLineLayer();

            var converter = new MapLineDataConverter();

            // Call
            converter.ConvertLayerProperties(mapLineData, mapLineLayer);

            // Assert
            ILineSymbolizer expectedSymbolizer = CreateExpectedSymbolizer(lineStyle);

            ILineScheme appliedScheme = mapLineLayer.Symbology;
            Assert.AreEqual(1, appliedScheme.Categories.Count);

            ILineCategory baseCategory = appliedScheme.Categories[0];
            AssertAreEqual(expectedSymbolizer, baseCategory.Symbolizer);
            Assert.IsNull(baseCategory.FilterExpression);
        }

        [Test]
        public void ConvertLayerProperties_MapLineDataWithThemeAndMetaDataNameInFeatures_ConvertDataToMapLineLayer()
        {
            // Setup
            const string metadataAttribute = "Meta";
            var random = new Random(21);

            var unequalCriterion = new ValueCriterion(ValueCriterionOperator.UnequalValue,
                                                      "unequal value");
            var equalCriterion = new ValueCriterion(ValueCriterionOperator.EqualValue,
                                                    "equal value");
            var theme = new MapTheme<LineCategoryTheme>(metadataAttribute, new[]
            {
                new LineCategoryTheme(equalCriterion, new LineStyle
                {
                    Color = Color.FromKnownColor(random.NextEnum<KnownColor>()),
                    Width = random.Next(1, 48),
                    DashStyle = random.NextEnum<LineDashStyle>()
                }),
                new LineCategoryTheme(unequalCriterion, new LineStyle
                {
                    Color = Color.FromKnownColor(random.NextEnum<KnownColor>()),
                    Width = random.Next(1, 48),
                    DashStyle = random.NextEnum<LineDashStyle>()
                })
            });

            var lineStyle = new LineStyle
            {
                Color = Color.FromKnownColor(random.NextEnum<KnownColor>()),
                Width = random.Next(1, 48),
                DashStyle = random.NextEnum<LineDashStyle>()
            };
            var mapLineData = new MapLineData("test", lineStyle, theme)
            {
                Features = new[]
                {
                    CreateMapFeatureWithMetaData(metadataAttribute)
                }
            };

            var mapLineLayer = new MapLineLayer();

            var converter = new MapLineDataConverter();

            // Call
            converter.ConvertLayerProperties(mapLineData, mapLineLayer);

            // Assert
            ILineSymbolizer expectedSymbolizer = CreateExpectedSymbolizer(lineStyle);

            ILineScheme appliedScheme = mapLineLayer.Symbology;
            Assert.AreEqual(3, appliedScheme.Categories.Count);

            ILineCategory baseCategory = appliedScheme.Categories[0];
            AssertAreEqual(expectedSymbolizer, baseCategory.Symbolizer);
            Assert.IsNull(baseCategory.FilterExpression);

            ILineCategory equalSchemeCategory = appliedScheme.Categories[1];
            string expectedFilter = $"[1] = '{equalCriterion.Value}'";
            Assert.AreEqual(expectedFilter, equalSchemeCategory.FilterExpression);
            LineStyle expectedCategoryStyle = theme.CategoryThemes.ElementAt(0).Style;
            expectedSymbolizer = CreateExpectedSymbolizer(expectedCategoryStyle);
            AssertAreEqual(expectedSymbolizer, equalSchemeCategory.Symbolizer);

            ILineCategory unEqualSchemeCategory = appliedScheme.Categories[2];
            expectedFilter = $"NOT [1] = '{unequalCriterion.Value}'";
            Assert.AreEqual(expectedFilter, unEqualSchemeCategory.FilterExpression);
            expectedCategoryStyle = theme.CategoryThemes.ElementAt(1).Style;
            expectedSymbolizer = CreateExpectedSymbolizer(expectedCategoryStyle);
            AssertAreEqual(expectedSymbolizer, unEqualSchemeCategory.Symbolizer);
        }

        private static ILineSymbolizer CreateExpectedSymbolizer(LineStyle expectedLineStyle)
        {
            DashStyle expectedDashStyle = MapDataHelper.Convert(expectedLineStyle.DashStyle);
            Color expectedColor = expectedLineStyle.Color;
            return new LineSymbolizer(expectedColor, expectedColor, expectedLineStyle.Width, expectedDashStyle, LineCap.Round);
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

        private static void AssertAreEqual(ILineSymbolizer firstSymbolizer, ILineSymbolizer secondSymbolizer)
        {
            IList<IStroke> firstStrokes = firstSymbolizer.Strokes;
            IList<IStroke> secondStrokes = secondSymbolizer.Strokes;
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