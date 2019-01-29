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
using System.Drawing;
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

namespace Core.Components.DotSpatial.Test.Layer
{
    [TestFixture]
    public class MapPolygonDataLayerTest
    {
        [Test]
        public void Constructor_WithoutMapPolygonData_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MapPolygonDataLayer(null);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("mapPolygonData", parameter);
        }

        [Test]
        public void Constructor_MapPolygonDataWithTestProperties_MapPolygonDataLayerCreatedAccordingly()
        {
            // Setup
            var mapPolygonData = new MapPolygonData("Test name", new PolygonStyle
            {
                FillColor = Color.AliceBlue,
                StrokeColor = Color.Azure,
                StrokeThickness = 2
            });

            SetMapPolygonDataTestProperties(mapPolygonData);

            // Call
            var mapPolygonDataLayer = new MapPolygonDataLayer(mapPolygonData);

            // Assert
            Assert.IsInstanceOf<MapPolygonLayer>(mapPolygonDataLayer);
            Assert.IsInstanceOf<IFeatureBasedMapDataLayer>(mapPolygonDataLayer);
            AssertMapPolygonDataLayerTestProperties(mapPolygonDataLayer);
            Assert.AreEqual(KnownCoordinateSystems.Projected.NationalGrids.Rijksdriehoekstelsel, mapPolygonDataLayer.Projection);
        }

        [Test]
        public void Update_MapPolygonDataWithTestProperties_MapPolygonDataLayerUpdatedAccordingly()
        {
            // Setup
            var mapPolygonData = new MapPolygonData("Test name", new PolygonStyle
            {
                FillColor = Color.AliceBlue,
                StrokeColor = Color.Azure,
                StrokeThickness = 2
            });
            var mapPolygonDataLayer = new MapPolygonDataLayer(mapPolygonData);

            SetMapPolygonDataTestProperties(mapPolygonData);

            // Precondition
            AssertMapPolygonDataLayerDefaultProperties(mapPolygonDataLayer);

            // Call
            mapPolygonDataLayer.Update();

            // Assert
            AssertMapPolygonDataLayerTestProperties(mapPolygonDataLayer);
        }

        [Test]
        public void GivenMapPolygonDataLayer_WhenUpdatedAfterMapPolygonDataFeaturesChanged_MapPolygonDataLayerFeaturesChanged()
        {
            // Given
            var mapPolygonData = new MapPolygonData("Test name")
            {
                Features = new[]
                {
                    CreateTestMapFeature()
                }
            };

            var mapPolygonDataLayer = new MapPolygonDataLayer(mapPolygonData);
            IFeature[] drawnFeatures = mapPolygonDataLayer.DataSet.Features.ToArray();

            // When
            mapPolygonData.Features = new[]
            {
                CreateTestMapFeature()
            };
            mapPolygonDataLayer.Update();

            // Then
            CollectionAssert.AreNotEqual(drawnFeatures, mapPolygonDataLayer.DataSet.Features);
        }

        [Test]
        public void GivenMapPolygonDataLayer_WhenUpdatedAndMapPolygonDataFeaturesNotChanged_PreviousMapPolygonDataLayerFeaturesPreserved()
        {
            // Given
            var mapPolygonData = new MapPolygonData("Test name")
            {
                Features = new[]
                {
                    CreateTestMapFeature()
                }
            };

            var mapPolygonDataLayer = new MapPolygonDataLayer(mapPolygonData);
            IFeature[] drawnFeatures = mapPolygonDataLayer.DataSet.Features.ToArray();

            // When
            mapPolygonDataLayer.Update();

            // Then
            CollectionAssert.AreEqual(drawnFeatures, mapPolygonDataLayer.DataSet.Features);
        }

        private static void SetMapPolygonDataTestProperties(MapPolygonData mapPolygonData)
        {
            mapPolygonData.Name = "Another test name";
            mapPolygonData.IsVisible = false;
            mapPolygonData.ShowLabels = true;
            mapPolygonData.SelectedMetaDataAttribute = "Name";
            mapPolygonData.Features = new[]
            {
                CreateTestMapFeature()
            };
        }

        private static void AssertMapPolygonDataLayerTestProperties(MapPolygonDataLayer mapPolygonDataLayer)
        {
            Assert.AreEqual("Another test name", mapPolygonDataLayer.Name);
            Assert.IsFalse(mapPolygonDataLayer.IsVisible);
            Assert.IsTrue(mapPolygonDataLayer.ShowLabels);

            Assert.IsNotNull(mapPolygonDataLayer.LabelLayer);
            Assert.AreEqual("[2]", mapPolygonDataLayer.LabelLayer.Symbology.Categories[0].Expression);

            var firstPattern = (SimplePattern) mapPolygonDataLayer.Symbolizer.Patterns[0];
            Assert.AreEqual(Color.AliceBlue, firstPattern.FillColor);
            Assert.AreEqual(Color.Azure, firstPattern.Outline.GetFillColor());
            Assert.AreEqual(2, firstPattern.Outline.GetWidth());

            Assert.AreEqual(1, mapPolygonDataLayer.DataSet.Features.Count);
        }

        private static void AssertMapPolygonDataLayerDefaultProperties(MapPolygonDataLayer mapPolygonDataLayer)
        {
            Assert.AreEqual("Test name", mapPolygonDataLayer.Name);
            Assert.IsTrue(mapPolygonDataLayer.IsVisible);
            Assert.IsFalse(mapPolygonDataLayer.ShowLabels);

            Assert.IsNotNull(mapPolygonDataLayer.LabelLayer);
            Assert.IsNull(mapPolygonDataLayer.LabelLayer.Symbology.Categories[0].Expression);

            var firstPattern = (SimplePattern) mapPolygonDataLayer.Symbolizer.Patterns[0];
            Assert.AreEqual(2, firstPattern.Outline.GetWidth());

            Assert.AreEqual(0, mapPolygonDataLayer.DataSet.Features.Count);
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