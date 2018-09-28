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
using PointShape = DotSpatial.Symbology.PointShape;

namespace Core.Components.DotSpatial.Test.Layer
{
    [TestFixture]
    public class MapPointDataLayerTest
    {
        [Test]
        public void Constructor_WithoutMapPointData_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MapPointDataLayer(null);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("mapPointData", parameter);
        }

        [Test]
        public void Constructor_MapPointDataWithTestProperties_MapPointDataLayerCreatedAccordingly()
        {
            // Setup
            Color color = Color.AliceBlue;
            var mapPointData = new MapPointData("Test name", new PointStyle
            {
                Color = color,
                Size = 4,
                Symbol = PointSymbol.Circle,
                StrokeColor = color,
                StrokeThickness = 1
            });

            SetMapPointDataTestProperties(mapPointData);

            // Call
            var mapPointDataLayer = new MapPointDataLayer(mapPointData);

            // Assert
            Assert.IsInstanceOf<MapPointLayer>(mapPointDataLayer);
            Assert.IsInstanceOf<IFeatureBasedMapDataLayer>(mapPointDataLayer);
            AssertMapPointDataLayerTestProperties(mapPointDataLayer);
            Assert.AreEqual(KnownCoordinateSystems.Projected.NationalGrids.Rijksdriehoekstelsel, mapPointDataLayer.Projection);
        }

        [Test]
        public void Update_MapPointDataWithTestProperties_MapPointDataLayerUpdatedAccordingly()
        {
            // Setup
            Color color = Color.AliceBlue;
            var mapPointData = new MapPointData("Test name", new PointStyle
            {
                Color = color,
                Size = 4,
                Symbol = PointSymbol.Circle,
                StrokeColor = color,
                StrokeThickness = 1
            });
            var mapPointDataLayer = new MapPointDataLayer(mapPointData);

            SetMapPointDataTestProperties(mapPointData);

            // Precondition
            AssertMapPointDataLayerDefaultProperties(mapPointDataLayer);

            // Call
            mapPointDataLayer.Update();

            // Assert
            AssertMapPointDataLayerTestProperties(mapPointDataLayer);
        }

        [Test]
        public void GivenMapPointDataLayer_WhenUpdatedAfterMapPointDataFeaturesChanged_MapPointDataLayerFeaturesChanged()
        {
            // Given
            var mapPointData = new MapPointData("Test name")
            {
                Features = new[]
                {
                    CreateTestMapFeature()
                }
            };

            var mapPointDataLayer = new MapPointDataLayer(mapPointData);
            IFeature[] drawnFeatures = mapPointDataLayer.DataSet.Features.ToArray();

            // When
            mapPointData.Features = new[]
            {
                CreateTestMapFeature()
            };
            mapPointDataLayer.Update();

            // Then
            CollectionAssert.AreNotEqual(drawnFeatures, mapPointDataLayer.DataSet.Features);
        }

        [Test]
        public void GivenMapPointDataLayer_WhenUpdatedAndMapPointDataFeaturesNotChanged_PreviousMapPointDataLayerFeaturesPreserved()
        {
            // Given
            var mapPointData = new MapPointData("Test name")
            {
                Features = new[]
                {
                    CreateTestMapFeature()
                }
            };

            var mapPointDataLayer = new MapPointDataLayer(mapPointData);
            IFeature[] drawnFeatures = mapPointDataLayer.DataSet.Features.ToArray();

            // When
            mapPointDataLayer.Update();

            // Then
            CollectionAssert.AreEqual(drawnFeatures, mapPointDataLayer.DataSet.Features);
        }

        private static void SetMapPointDataTestProperties(MapPointData mapPointData)
        {
            mapPointData.Name = "Another test name";
            mapPointData.IsVisible = false;
            mapPointData.ShowLabels = true;
            mapPointData.SelectedMetaDataAttribute = "Name";
            mapPointData.Features = new[]
            {
                CreateTestMapFeature()
            };
        }

        private static void AssertMapPointDataLayerTestProperties(MapPointDataLayer mapPointDataLayer)
        {
            Assert.AreEqual("Another test name", mapPointDataLayer.Name);
            Assert.IsFalse(mapPointDataLayer.IsVisible);
            Assert.IsTrue(mapPointDataLayer.ShowLabels);

            Assert.IsNotNull(mapPointDataLayer.LabelLayer);
            Assert.AreEqual("[2]", mapPointDataLayer.LabelLayer.Symbology.Categories[0].Expression);

            var firstSymbol = (SimpleSymbol) mapPointDataLayer.Symbolizer.Symbols[0];
            Assert.AreEqual(Color.AliceBlue, firstSymbol.Color);
            Assert.AreEqual(4, firstSymbol.Size.Width);
            Assert.AreEqual(4, firstSymbol.Size.Height);
            Assert.AreEqual(PointShape.Ellipse, firstSymbol.PointShape);

            Assert.AreEqual(1, mapPointDataLayer.DataSet.Features.Count);
        }

        private static void AssertMapPointDataLayerDefaultProperties(MapPointDataLayer mapPointDataLayer)
        {
            Assert.AreEqual("Test name", mapPointDataLayer.Name);
            Assert.IsTrue(mapPointDataLayer.IsVisible);
            Assert.IsFalse(mapPointDataLayer.ShowLabels);

            Assert.IsNotNull(mapPointDataLayer.LabelLayer);
            Assert.IsNull(mapPointDataLayer.LabelLayer.Symbology.Categories[0].Expression);

            var firstSymbol = (SimpleSymbol) mapPointDataLayer.Symbolizer.Symbols[0];
            Assert.AreEqual(4, firstSymbol.Size.Width);
            Assert.AreEqual(4, firstSymbol.Size.Height);
            Assert.AreEqual(PointShape.Ellipse, firstSymbol.PointShape);

            Assert.AreEqual(0, mapPointDataLayer.DataSet.Features.Count);
        }

        private static MapFeature CreateTestMapFeature()
        {
            return new MapFeature(new[]
            {
                new MapGeometry(new[]
                {
                    new[]
                    {
                        new Point2D(1, 2)
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