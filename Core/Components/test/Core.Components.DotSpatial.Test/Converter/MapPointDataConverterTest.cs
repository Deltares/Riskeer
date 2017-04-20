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

using System.Collections.Generic;
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
            Assert.IsInstanceOf<FeatureBasedMapDataConverter<MapPointData, MapPointLayer>>(converter);
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
            Assert.AreEqual(mapPointData.Features.Length, mapPointLayer.DataSet.Features.Count);
            Assert.IsInstanceOf<Point>(feature.BasicGeometry);

            IEnumerable<Coordinate> expectedCoordinates = mapFeature.MapGeometries.ElementAt(0).PointCollections.ElementAt(0).Select(p => new Coordinate(p.X, p.Y));
            CollectionAssert.AreEqual(expectedCoordinates, feature.Coordinates);
        }

        [Test]
        [Combinatorial]
        public void ConvertLayerProperties_MapPointDataWithStyle_ConvertsStyleToMapPointLayer(
            [Values(KnownColor.AliceBlue, KnownColor.Azure)] KnownColor color,
            [Values(1, 5)] int width,
            [Values(PointSymbol.Circle, PointSymbol.Square)] PointSymbol pointStyle)
        {
            // Setup
            var converter = new MapPointDataConverter();
            var mapPointLayer = new MapPointLayer();
            Color expectedColor = Color.FromKnownColor(color);
            var mapPointData = new MapPointData("test")
            {
                Style = new PointStyle(expectedColor, width, pointStyle)
            };

            // Call
            converter.ConvertLayerProperties(mapPointData, mapPointLayer);

            // Assert
            PointShape expectedPointShape = pointStyle == PointSymbol.Circle
                                                ? PointShape.Ellipse
                                                : PointShape.Rectangle;

            AssertAreEqual(new PointSymbolizer(expectedColor, expectedPointShape, width), mapPointLayer.Symbolizer);
        }

        private static void AssertAreEqual(IPointSymbolizer firstSymbolizer, IPointSymbolizer secondSymbolizer)
        {
            IList<ISymbol> firstSymbols = firstSymbolizer.Symbols;
            IList<ISymbol> secondSymbols = secondSymbolizer.Symbols;
            Assert.AreEqual(firstSymbols.Count, secondSymbols.Count, "Unequal amount of symbols defined.");
            for (var i = 0; i < firstSymbols.Count; i++)
            {
                var firstSymbol = (SimpleSymbol) firstSymbols[i];
                var secondSymbol = (SimpleSymbol) secondSymbols[i];

                Assert.AreEqual(firstSymbol.Color, secondSymbol.Color);
                Assert.AreEqual(firstSymbol.PointShape, secondSymbol.PointShape);
                Assert.AreEqual(firstSymbol.Size, secondSymbol.Size);
            }
        }
    }
}