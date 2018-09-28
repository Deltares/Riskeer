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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Components.DotSpatial.Layer;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using Core.Components.Gis.Style;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.Symbology;
using NUnit.Framework;
using LineStyle = Core.Components.Gis.Style.LineStyle;

namespace Core.Components.DotSpatial.Test.Layer
{
    [TestFixture]
    public class MapLineDataLayerTest
    {
        [Test]
        public void Constructor_WithoutMapLineData_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MapLineDataLayer(null);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("mapLineData", parameter);
        }

        [Test]
        public void Constructor_MapLineDataWithTestProperties_MapLineDataLayerCreatedAccordingly()
        {
            // Setup
            var mapLineData = new MapLineData("Test name", new LineStyle
            {
                Color = Color.AliceBlue,
                Width = 2,
                DashStyle = LineDashStyle.DashDot
            });

            SetMapLineDataTestProperties(mapLineData);

            // Call
            var mapLineDataLayer = new MapLineDataLayer(mapLineData);

            // Assert
            Assert.IsInstanceOf<MapLineLayer>(mapLineDataLayer);
            Assert.IsInstanceOf<IFeatureBasedMapDataLayer>(mapLineDataLayer);
            AssertMapLineDataLayerTestProperties(mapLineDataLayer);
            Assert.AreEqual(KnownCoordinateSystems.Projected.NationalGrids.Rijksdriehoekstelsel, mapLineDataLayer.Projection);
        }

        [Test]
        public void Update_MapLineDataWithTestProperties_MapLineDataLayerUpdatedAccordingly()
        {
            // Setup
            var mapLineData = new MapLineData("Test name", new LineStyle
            {
                Color = Color.AliceBlue,
                Width = 2,
                DashStyle = LineDashStyle.DashDot
            });
            var mapLineDataLayer = new MapLineDataLayer(mapLineData);

            SetMapLineDataTestProperties(mapLineData);

            // Precondition
            AssertMapLineDataLayerDefaultProperties(mapLineDataLayer);

            // Call
            mapLineDataLayer.Update();

            // Assert
            AssertMapLineDataLayerTestProperties(mapLineDataLayer);
        }

        [Test]
        public void GivenMapLineDataLayer_WhenUpdatedAfterMapLineDataFeaturesChanged_MapLineDataLayerFeaturesChanged()
        {
            // Given
            var mapLineData = new MapLineData("Test name")
            {
                Features = new[]
                {
                    CreateTestMapFeature()
                }
            };

            var mapLineDataLayer = new MapLineDataLayer(mapLineData);
            IFeature[] drawnFeatures = mapLineDataLayer.DataSet.Features.ToArray();

            // When
            mapLineData.Features = new[]
            {
                CreateTestMapFeature()
            };
            mapLineDataLayer.Update();

            // Then
            CollectionAssert.AreNotEqual(drawnFeatures, mapLineDataLayer.DataSet.Features);
        }

        [Test]
        public void GivenMapLineDataLayer_WhenUpdatedAndMapLineDataFeaturesNotChanged_PreviousMapLineDataLayerFeaturesPreserved()
        {
            // Given
            var mapLineData = new MapLineData("Test name")
            {
                Features = new[]
                {
                    CreateTestMapFeature()
                }
            };

            var mapLineDataLayer = new MapLineDataLayer(mapLineData);
            IFeature[] drawnFeatures = mapLineDataLayer.DataSet.Features.ToArray();

            // When
            mapLineDataLayer.Update();

            // Then
            CollectionAssert.AreEqual(drawnFeatures, mapLineDataLayer.DataSet.Features);
        }

        private static void SetMapLineDataTestProperties(MapLineData mapLineData)
        {
            mapLineData.Name = "Another test name";
            mapLineData.IsVisible = false;
            mapLineData.ShowLabels = true;
            mapLineData.SelectedMetaDataAttribute = "Name";
            mapLineData.Features = new[]
            {
                CreateTestMapFeature()
            };
        }

        private static void AssertMapLineDataLayerTestProperties(MapLineDataLayer mapLineDataLayer)
        {
            Assert.AreEqual("Another test name", mapLineDataLayer.Name);
            Assert.IsFalse(mapLineDataLayer.IsVisible);
            Assert.IsTrue(mapLineDataLayer.ShowLabels);

            Assert.IsNotNull(mapLineDataLayer.LabelLayer);
            Assert.AreEqual("[2]", mapLineDataLayer.LabelLayer.Symbology.Categories[0].Expression);

            var firstStroke = (CartographicStroke) mapLineDataLayer.Symbolizer.Strokes[0];
            Assert.AreEqual(Color.AliceBlue, firstStroke.Color);
            Assert.AreEqual(2, firstStroke.Width);
            Assert.AreEqual(DashStyle.DashDot, firstStroke.DashStyle);

            Assert.AreEqual(1, mapLineDataLayer.DataSet.Features.Count);
        }

        private static void AssertMapLineDataLayerDefaultProperties(MapLineDataLayer mapLineDataLayer)
        {
            Assert.AreEqual("Test name", mapLineDataLayer.Name);
            Assert.IsTrue(mapLineDataLayer.IsVisible);
            Assert.IsFalse(mapLineDataLayer.ShowLabels);

            Assert.IsNotNull(mapLineDataLayer.LabelLayer);
            Assert.IsNull(mapLineDataLayer.LabelLayer.Symbology.Categories[0].Expression);

            var firstStroke = (SimpleStroke) mapLineDataLayer.Symbolizer.Strokes[0];
            Assert.AreEqual(2.0, firstStroke.Width);
            Assert.AreEqual(DashStyle.DashDot, firstStroke.DashStyle);

            Assert.AreEqual(0, mapLineDataLayer.DataSet.Features.Count);
        }

        private static MapFeature CreateTestMapFeature()
        {
            return new MapFeature(new[]
            {
                new MapGeometry(new[]
                {
                    new[]
                    {
                        new Point2D(1, 2),
                        new Point2D(2, 3)
                    }
                })
            })
            {
                MetaData =
                {
                    {
                        "Id", 1.1
                    },
                    {
                        "Name", "Feature"
                    }
                }
            };
        }
    }
}