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
        public void Convert_SimpleMapPointData_ReturnMapPointLayer()
        {
            // Setup
            var converter = new MapPointDataConverter();
            var pointData = new MapPointData("test data")
            {
                Features = new MapFeature[0]
            };

            // Call
            IMapFeatureLayer layer = converter.Convert(pointData);

            // Assert
            Assert.IsInstanceOf<MapPointLayer>(layer);
            Assert.AreEqual(FeatureType.Point, layer.DataSet.FeatureType);
        }

        [Test]
        public void Convert_MapPointDataWithMultipleFeatures_ConvertsEachGeometryAsSingleFeature()
        {
            // Setup
            var converter = new MapPointDataConverter();

            MapFeature[] features =
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
            };

            var pointData = new MapPointData("test")
            {
                Features = features
            };

            // Call
            IMapFeatureLayer layer = converter.Convert(pointData);

            // Assert
            Assert.AreEqual(4, layer.DataSet.Features.Count);
        }

        [Test]
        public void Convert_MapPointDataWithFeature_ReturnMapPointLayerWithPointData()
        {
            // Setup
            var converter = new MapPointDataConverter();
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

            MapFeature[] features =
            {
                mapFeature
            };

            var pointData = new MapPointData("test")
            {
                Features = features
            };

            // Call
            IMapFeatureLayer layer = converter.Convert(pointData);

            // Assert
            IFeature feature = layer.DataSet.Features[0];
            Assert.AreEqual(features.Length, layer.DataSet.Features.Count);
            Assert.IsInstanceOf<Point>(feature.BasicGeometry);

            var expectedCoordinates = mapFeature.MapGeometries.ElementAt(0).PointCollections.ElementAt(0).Select(p => new Coordinate(p.X, p.Y));
            CollectionAssert.AreEqual(expectedCoordinates, feature.Coordinates);
        }

        [Test]
        [TestCase(KnownColor.AliceBlue)]
        [TestCase(KnownColor.Azure)]
        [TestCase(KnownColor.Beige)]
        public void Convert_PointStyleSetWithDifferentColors_AppliesStyleToLayer(KnownColor color)
        {
            // Setup
            var converter = new MapPointDataConverter();
            var expectedColor = Color.FromKnownColor(color);
            var data = new MapPointData("test")
            {
                Style = new PointStyle(expectedColor, 3, PointSymbol.Circle)
            };

            // Call
            MapPointLayer layer = (MapPointLayer) converter.Convert(data);

            // Assert
            AssertAreEqual(new PointSymbolizer(expectedColor, PointShape.Ellipse, 3), layer.Symbolizer);
        }

        [Test]
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(7)]
        public void Convert_PointStyleSetWithDifferentWidths_AppliesStyleToLayer(int width)
        {
            // Setup
            var converter = new MapPointDataConverter();
            var data = new MapPointData("test")
            {
                Style = new PointStyle(Color.AliceBlue, width, PointSymbol.Circle)
            };

            // Call
            MapPointLayer layer = (MapPointLayer) converter.Convert(data);

            // Assert
            AssertAreEqual(new PointSymbolizer(Color.AliceBlue, PointShape.Ellipse, width), layer.Symbolizer);
        }

        [Test]
        [TestCase(PointSymbol.Circle)]
        [TestCase(PointSymbol.Square)]
        [TestCase(PointSymbol.Triangle)]
        public void Convert_PointStyleSetWithDifferentPointStyles_AppliesStyleToLayer(PointSymbol pointStyle)
        {
            // Setup
            var converter = new MapPointDataConverter();
            var data = new MapPointData("test")
            {
                Style = new PointStyle(Color.AliceBlue, 3, pointStyle)
            };

            // Call
            MapPointLayer layer = (MapPointLayer) converter.Convert(data);

            // Assert
            PointShape expectedPointShape = pointStyle == PointSymbol.Circle
                                                ? PointShape.Ellipse
                                                : pointStyle == PointSymbol.Square
                                                      ? PointShape.Rectangle
                                                      : PointShape.Triangle;
            AssertAreEqual(new PointSymbolizer(Color.AliceBlue, expectedPointShape, 3), layer.Symbolizer);
        }

        private static void AssertAreEqual(IPointSymbolizer firstSymbolizer, IPointSymbolizer secondSymbolizer)
        {
            IList<ISymbol> firstSymbols = firstSymbolizer.Symbols;
            IList<ISymbol> secondSymbols = secondSymbolizer.Symbols;
            Assert.AreEqual(firstSymbols.Count, secondSymbols.Count, "Unequal amount of strokes defined.");
            for (var i = 0; i < firstSymbols.Count; i++)
            {
                SimpleSymbol firstStroke = (SimpleSymbol) firstSymbols[i];
                SimpleSymbol secondStroke = (SimpleSymbol) secondSymbols[i];

                Assert.AreEqual(firstStroke.Color, secondStroke.Color);
                Assert.AreEqual(firstStroke.PointShape, secondStroke.PointShape);
                Assert.AreEqual(firstStroke.Size, secondStroke.Size);
            }
        }
    }
}