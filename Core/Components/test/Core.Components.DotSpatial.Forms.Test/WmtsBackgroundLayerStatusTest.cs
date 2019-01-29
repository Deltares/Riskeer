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

using System.Collections.Generic;
using BruTile;
using Core.Components.BruTile.Configurations;
using Core.Components.BruTile.IO;
using Core.Components.DotSpatial.Layer.BruTile;
using Core.Components.Gis.Data;
using Core.Components.Gis.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Components.DotSpatial.Forms.Test
{
    [TestFixture]
    public class WmtsBackgroundLayerStatusTest
    {
        private static IEnumerable<TestCaseData> SlightlyDifferentMapDataFromDefault
        {
            get
            {
                yield return new TestCaseData(new WmtsMapData("case 1",
                                                              "https://geodata.nationaalgeoregister.nl/wmts/top10nlv1?VERSION=1.1.0&request=GetCapabilities",
                                                              "brtachtergrondkaart(EPSG:28992)",
                                                              "image/png"))
                    .SetName("HasSameConfiguration_ForCase1_ReturnFalse");
                yield return new TestCaseData(new WmtsMapData("case 2",
                                                              "https://geodata.nationaalgeoregister.nl/wmts/top10nlv1?VERSION=1.1.0&request=GetCapabilities",
                                                              "brtachtergrondkaart(EPSG:12345)",
                                                              "image/png"))
                    .SetName("HasSameConfiguration_ForCase2_ReturnFalse");
                yield return new TestCaseData(new WmtsMapData("case 3",
                                                              "https://geodata.nationaalgeoregister.nl/wmts/top10nlv1?VERSION=1.1.0&request=GetCapabilities",
                                                              "brtachtergrondkaart(EPSG:28992)",
                                                              "image/jpeg"))
                    .SetName("HasSameConfiguration_ForCase3_ReturnFalse");
            }
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            using (var layerStatus = new WmtsBackgroundLayerStatus())
            {
                // Assert
                Assert.IsInstanceOf<BackgroundLayerStatus>(layerStatus);

                Assert.IsNull(layerStatus.BackgroundLayer);
                Assert.IsFalse(layerStatus.PreviousBackgroundLayerCreationFailed);
            }
        }

        [Test]
        public void HasSameConfiguration_MapDataNotWmtsMapData_ReturnFalse()
        {
            // Setup
            using (var layerStatus = new WmtsBackgroundLayerStatus())
            {
                var mapData = new SimpleImageBasedMapData();

                // Call
                bool isSame = layerStatus.HasSameConfiguration(mapData);

                // Assert
                Assert.IsFalse(isSame);
            }
        }

        [Test]
        public void HasSameConfiguration_NoInitializedLayer_ReturnFalse()
        {
            // Setup
            using (var layerStatus = new WmtsBackgroundLayerStatus())
            {
                WmtsMapData mapData = WmtsMapDataTestHelper.CreateDefaultPdokMapData();

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
            var mocks = new MockRepository();
            var tileFetcher = mocks.Stub<ITileFetcher>();
            using (var layerStatus = new WmtsBackgroundLayerStatus())
            {
                IConfiguration configuration = CreateStubConfiguration(mocks, tileFetcher);
                mocks.ReplayAll();

                using (var layer = new BruTileLayer(configuration))
                {
                    WmtsMapData mapData1 = WmtsMapDataTestHelper.CreateDefaultPdokMapData();
                    WmtsMapData mapData2 = WmtsMapDataTestHelper.CreateDefaultPdokMapData();

                    layerStatus.LayerInitializationSuccessful(layer, mapData1);

                    // Call
                    bool isSame = layerStatus.HasSameConfiguration(mapData2);

                    // Assert
                    Assert.IsTrue(isSame,
                                  "Should recognize same configuration even if instance is not the same.");
                }
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(SlightlyDifferentMapDataFromDefault))]
        public void HasSameConfiguration_ForDifferentInitializedLayer_ReturnFalse(WmtsMapData otherData)
        {
            // Setup
            var mocks = new MockRepository();
            var tileFetcher = mocks.Stub<ITileFetcher>();
            using (var layerStatus = new WmtsBackgroundLayerStatus())
            {
                IConfiguration configuration = CreateStubConfiguration(mocks, tileFetcher);
                mocks.ReplayAll();

                using (var layer = new BruTileLayer(configuration))
                {
                    WmtsMapData mapData = WmtsMapDataTestHelper.CreateDefaultPdokMapData();

                    layerStatus.LayerInitializationSuccessful(layer, mapData);

                    // Call
                    bool isSame = layerStatus.HasSameConfiguration(otherData);

                    // Assert
                    Assert.IsFalse(isSame);
                }
            }

            mocks.VerifyAll();
        }

        [Test]
        public void LayerInitializationSuccessful_MapDataNotWmtsMapData_SetCreationFailedTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var tileFetcher = mocks.Stub<ITileFetcher>();
            IConfiguration configuration = CreateStubConfiguration(mocks, tileFetcher);
            mocks.ReplayAll();

            using (var layer = new BruTileLayer(configuration))
            using (var layerStatus = new WmtsBackgroundLayerStatus())
            {
                var mapData = new SimpleImageBasedMapData();

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
            var mocks = new MockRepository();
            var tileFetcher = mocks.Stub<ITileFetcher>();
            IConfiguration configuration = CreateStubConfiguration(mocks, tileFetcher);
            mocks.ReplayAll();

            using (var layer = new BruTileLayer(configuration))
            using (var layerStatus = new WmtsBackgroundLayerStatus())
            {
                layerStatus.LayerInitializationFailed();

                // Precondition
                Assert.IsTrue(layerStatus.PreviousBackgroundLayerCreationFailed);

                WmtsMapData mapData = WmtsMapDataTestHelper.CreateDefaultPdokMapData();

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
            using (var layerStatus = new WmtsBackgroundLayerStatus())
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
            var mocks = new MockRepository();
            var tileFetcher = mocks.Stub<ITileFetcher>();
            IConfiguration configuration = CreateStubConfiguration(mocks, tileFetcher);
            mocks.ReplayAll();

            using (var layer = new BruTileLayer(configuration))
            using (var layerStatus = new WmtsBackgroundLayerStatus())
            {
                WmtsMapData mapData = WmtsMapDataTestHelper.CreateDefaultPdokMapData();
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
            var mocks = new MockRepository();
            var tileFetcher = mocks.Stub<ITileFetcher>();
            IConfiguration configuration = CreateStubConfiguration(mocks, tileFetcher);
            mocks.ReplayAll();

            using (var layer = new BruTileLayer(configuration))
            using (var layerStatus = new WmtsBackgroundLayerStatus())
            {
                WmtsMapData mapData = WmtsMapDataTestHelper.CreateDefaultPdokMapData();
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
            using (var layerStatus = new WmtsBackgroundLayerStatus())
            {
                WmtsMapData mapData = WmtsMapDataTestHelper.CreateDefaultPdokMapData();
                layerStatus.LayerInitializationFailed();

                // Call
                layerStatus.ClearConfiguration(true);

                // Assert
                Assert.IsTrue(layerStatus.PreviousBackgroundLayerCreationFailed);
                Assert.IsFalse(layerStatus.HasSameConfiguration(mapData));
            }
        }

        private static IConfiguration CreateStubConfiguration(MockRepository mocks, ITileFetcher tileFetcher)
        {
            var schema = mocks.Stub<ITileSchema>();
            schema.Stub(s => s.Srs).Return("EPSG:28992");
            schema.Stub(s => s.Extent).Return(new Extent());

            var configuration = mocks.Stub<IConfiguration>();
            configuration.Stub(c => c.Initialized).Return(true);
            configuration.Stub(c => c.TileSchema).Return(schema);
            configuration.Stub(c => c.TileFetcher).Return(tileFetcher);
            configuration.Stub(c => c.Dispose());
            return configuration;
        }

        private class SimpleImageBasedMapData : ImageBasedMapData
        {
            public SimpleImageBasedMapData() : base("SimpleImageBasedMapData") {}
        }
    }
}