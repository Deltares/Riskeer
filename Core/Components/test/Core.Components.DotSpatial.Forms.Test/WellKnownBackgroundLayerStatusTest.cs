// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
using BruTile;
using Core.Components.BruTile.Configurations;
using Core.Components.BruTile.IO;
using Core.Components.DotSpatial.Layer.BruTile;
using Core.Components.Gis.Data;
using Core.Components.Gis.TestUtil;
using DotSpatial.Symbology;
using NSubstitute;
using NUnit.Framework;

namespace Core.Components.DotSpatial.Forms.Test
{
    [TestFixture]
    public class WellKnownBackgroundLayerStatusTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            using (var layerStatus = new WellKnownBackgroundLayerStatus())
            {
                // Assert
                Assert.IsInstanceOf<BackgroundLayerStatus>(layerStatus);

                Assert.IsNull(layerStatus.BackgroundLayer);
                Assert.IsFalse(layerStatus.PreviousBackgroundLayerCreationFailed);
            }
        }

        [Test]
        public void HasSameConfiguration_MapDataNotWellKnownMapData_ReturnFalse()
        {
            // Setup
            using (var layerStatus = new WellKnownBackgroundLayerStatus())
            {
                var mapData = new TestImageBasedMapData("test", true);

                // Call
                bool isSame = layerStatus.HasSameConfiguration(mapData);

                // Assert
                Assert.IsFalse(isSame);
            }
        }

        [Test]
        public void HasSameConfiguration_ForInitializedLayer_ReturnTrue()
        {
            // Setup
            var tileFetcher = Substitute.For<ITileFetcher>();
            using (var layerStatus = new WellKnownBackgroundLayerStatus())
            {
                IConfiguration configuration = CreateStubConfiguration(tileFetcher);

                using (var layer = new BruTileLayer(configuration))
                {
                    var source = new Random(789).NextEnum<WellKnownTileSource>();
                    var mapData1 = new WellKnownTileSourceMapData(source);
                    var mapData2 = new WellKnownTileSourceMapData(source);

                    layerStatus.LayerInitializationSuccessful(layer, mapData1);

                    // Call
                    bool isSame = layerStatus.HasSameConfiguration(mapData2);

                    // Assert
                    Assert.IsTrue(isSame, "Should recognize same configuration even if instance is not the same.");
                }
            }
        }

        [Test]
        public void LayerInitializationSuccessful_MapDataNotWellKnownMapData_SetCreationFailedTrue()
        {
            // Setup
            var tileFetcher = Substitute.For<ITileFetcher>();
            IConfiguration configuration = CreateStubConfiguration(tileFetcher);

            using (var layer = new BruTileLayer(configuration))
            using (var layerStatus = new WellKnownBackgroundLayerStatus())
            {
                var mapData = new TestImageBasedMapData("test", true);

                // Call
                layerStatus.LayerInitializationSuccessful(layer, mapData);

                // Assert
                Assert.IsTrue(layerStatus.PreviousBackgroundLayerCreationFailed);
            }
        }

        [Test]
        public void LayerInitializationSuccessful_InitializationPreviouslyFailed_PreviousBackgroundLayerCreationFailedFalse()
        {
            // Setup
            var tileFetcher = Substitute.For<ITileFetcher>();
            IConfiguration configuration = CreateStubConfiguration(tileFetcher);

            using (var layer = new BruTileLayer(configuration))
            using (var layerStatus = new WellKnownBackgroundLayerStatus())
            {
                layerStatus.LayerInitializationFailed();

                var mapData = new WellKnownTileSourceMapData(new Random(789).NextEnum<WellKnownTileSource>());

                // Call
                layerStatus.LayerInitializationSuccessful(layer, mapData);

                // Assert
                Assert.IsFalse(layerStatus.PreviousBackgroundLayerCreationFailed);
            }
        }

        [Test]
        public void LayerInitializationFailed_PreviousBackgroundLayerCreationFailedTrue()
        {
            // Setup
            using (var layerStatus = new WellKnownBackgroundLayerStatus())
            {
                // Call
                layerStatus.LayerInitializationFailed();

                // Assert
                Assert.IsTrue(layerStatus.PreviousBackgroundLayerCreationFailed);
            }
        }

        [Test]
        public void LayerInitializationFailed_HasLayer_ConfigurationCleared()
        {
            // Setup
            var tileFetcher = Substitute.For<ITileFetcher>();
            IConfiguration configuration = CreateStubConfiguration(tileFetcher);

            using (var layer = new BruTileLayer(configuration))
            using (var layerStatus = new WellKnownBackgroundLayerStatus())
            {
                var mapData = new WellKnownTileSourceMapData(new Random(789).NextEnum<WellKnownTileSource>());
                layerStatus.LayerInitializationSuccessful(layer, mapData);

                // Call
                layerStatus.LayerInitializationFailed();

                // Assert
                Assert.IsTrue(layerStatus.PreviousBackgroundLayerCreationFailed);
                Assert.IsFalse(layerStatus.HasSameConfiguration(mapData));
            }
        }

        [Test]
        public void ClearConfiguration_HasLayer_ConfigurationCleared()
        {
            // Setup
            var tileFetcher = Substitute.For<ITileFetcher>();
            IConfiguration configuration = CreateStubConfiguration(tileFetcher);

            using (var layer = new BruTileLayer(configuration))
            using (var layerStatus = new WellKnownBackgroundLayerStatus())
            {
                var mapData = new WellKnownTileSourceMapData(new Random(789).NextEnum<WellKnownTileSource>());
                layerStatus.LayerInitializationSuccessful(layer, mapData);

                // Call
                layerStatus.ClearConfiguration();

                // Assert
                Assert.IsFalse(layerStatus.PreviousBackgroundLayerCreationFailed);
                Assert.IsFalse(layerStatus.HasSameConfiguration(mapData));
            }
        }

        [Test]
        public void ClearConfiguration_WithFailedLayerInitializationAndExpectingRecreation_ConfigurationClearedButKeepFailedFlagSet()
        {
            // Setup
            using (var layerStatus = new WellKnownBackgroundLayerStatus())
            {
                var mapData = new WellKnownTileSourceMapData(new Random(465).NextEnum<WellKnownTileSource>());
                layerStatus.LayerInitializationFailed();

                // Call
                layerStatus.ClearConfiguration(true);

                // Assert
                Assert.IsTrue(layerStatus.PreviousBackgroundLayerCreationFailed);
                Assert.IsFalse(layerStatus.HasSameConfiguration(mapData));
            }
        }

        private static IConfiguration CreateStubConfiguration(ITileFetcher tileFetcher)
        {
            var schema = Substitute.For<ITileSchema>();
            schema.Srs.Returns("EPSG:28992");
            schema.Extent.Returns(new Extent());

            var configuration = Substitute.For<IConfiguration>();
            configuration.Initialized.Returns(true);
            configuration.TileSchema.Returns(schema);
            configuration.TileFetcher.Returns(tileFetcher);
            return configuration;
        }
    }
}