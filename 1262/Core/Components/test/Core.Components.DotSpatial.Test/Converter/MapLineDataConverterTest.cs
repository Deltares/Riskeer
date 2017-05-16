// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
            Assert.AreEqual(mapLineData.Features.Length, mapLineLayer.DataSet.Features.Count);
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
            Assert.AreEqual(mapLineData.Features.Length, mapLineLayer.DataSet.Features.Count);
            Assert.IsInstanceOf<LineString>(feature.BasicGeometry);

            IEnumerable<Coordinate> expectedCoordinates = mapFeature.MapGeometries.ElementAt(0).PointCollections.ElementAt(0)
                                                                    .Select(p => new Coordinate(p.X, p.Y));
            CollectionAssert.AreEqual(expectedCoordinates, feature.Coordinates);
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
            Assert.AreEqual(mapLineData.Features.Length, mapLineLayer.DataSet.Features.Count);
            Assert.IsInstanceOf<MultiLineString>(feature.BasicGeometry);

            IEnumerable<Coordinate> expectedCoordinates = mapFeature.MapGeometries.SelectMany(mg => mg.PointCollections.ElementAt(0).Select(p => new Coordinate(p.X, p.Y)));
            CollectionAssert.AreEqual(expectedCoordinates, feature.Coordinates);
        }

        [Test]
        [Combinatorial]
        public void ConvertLayerProperties_MapLineDataWithStyle_ConvertsStyleToMapLineLayer(
            [Values(KnownColor.AliceBlue, KnownColor.Azure)] KnownColor color,
            [Values(1, 5)] int width,
            [Values(DashStyle.Solid, DashStyle.Dash)] DashStyle lineStyle)
        {
            // Setup
            Color expectedColor = Color.FromKnownColor(color);
            var converter = new MapLineDataConverter();
            var mapLineLayer = new MapLineLayer();
            var mapLineData = new MapLineData("test")
            {
                Style = new LineStyle(expectedColor, width, lineStyle)
            };

            // Call
            converter.ConvertLayerProperties(mapLineData, mapLineLayer);

            // Assert
            AssertAreEqual(new LineSymbolizer(expectedColor, expectedColor, width, lineStyle, LineCap.Round), mapLineLayer.Symbolizer);
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