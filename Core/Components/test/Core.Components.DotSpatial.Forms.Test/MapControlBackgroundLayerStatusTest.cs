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
using BruTile;
using Core.Components.BruTile.Configurations;
using Core.Components.BruTile.IO;
using Core.Components.DotSpatial.Layer.BruTile;
using Core.Components.Gis.Data;
using Core.Components.Gis.TestUtil;
using DotSpatial.Symbology;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Components.DotSpatial.Forms.Test
{
    [TestFixture]
    public class MapControlBackgroundLayerStatusTest
    {
        private static IEnumerable<TestCaseData> SameMapDataTypes
        {
            get
            {
                var source = new Random().NextEnum<WellKnownTileSource>();
                yield return new TestCaseData(new WellKnownTileSourceMapData(source),
                                              new WellKnownTileSourceMapData(source))
                    .SetName("WellKnownMapDataType");
                yield return new TestCaseData(WmtsMapDataTestHelper.CreateDefaultPdokMapData(),
                                              WmtsMapDataTestHelper.CreateDefaultPdokMapData())
                    .SetName("WmtsMapDataType");
            }
        }

        private static IEnumerable<TestCaseData> OtherConfigurations
        {
            get
            {
                var source = new Random().NextEnum<WellKnownTileSource>();
                yield return new TestCaseData(new WellKnownTileSourceMapData(source),
                                              WmtsMapDataTestHelper.CreateDefaultPdokMapData())
                    .SetName("WellKnownToWmts");
                yield return new TestCaseData(WmtsMapDataTestHelper.CreateDefaultPdokMapData(),
                                              new WellKnownTileSourceMapData(source))
                    .SetName("WmtsToWellKnown");
                yield return new TestCaseData(new WellKnownTileSourceMapData(WellKnownTileSource.BingAerial),
                                              new WellKnownTileSourceMapData(WellKnownTileSource.BingHybrid))
                    .SetName("OtherWellKnownConfiguration");
                yield return new TestCaseData(WmtsMapDataTestHelper.CreateDefaultPdokMapData(),
                                              WmtsMapDataTestHelper.CreateAlternativePdokMapData())
                    .SetName("OtherWmtsConfiguration");
            }
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            using (var layerStatus = new MapControlBackgroundLayerStatus())
            {
                // Assert
                Assert.IsInstanceOf<BackgroundLayerStatus>(layerStatus);

                Assert.IsNull(layerStatus.BackgroundLayer);
                Assert.IsFalse(layerStatus.PreviousBackgroundLayerCreationFailed);
            }
        }

        [Test]
        public void HasSameConfiguration_LayerNotInitialized_ReturnFalse()
        {
            // Setup
            using (var layerStatus = new MapControlBackgroundLayerStatus())
            {
                var mapData = new TestImageBasedMapData("test", true);

                // Call
                bool isSame = layerStatus.HasSameConfiguration(mapData);

                // Assert
                Assert.IsFalse(isSame);
            }
        }

        [Test]
        [TestCaseSource(nameof(OtherConfigurations))]
        public void HasSameConfiguration_LayerInitializedOtherConfigurations_ReturnFalse(ImageBasedMapData mapData1, ImageBasedMapData mapData2)
        {
            // Setup
            var mocks = new MockRepository();
            var tileFetcher = mocks.Stub<ITileFetcher>();
            IConfiguration configuration = CreateStubConfiguration(mocks, tileFetcher);
            mocks.ReplayAll();

            using (var layerStatus = new MapControlBackgroundLayerStatus())
            {
                using (var layer = new BruTileLayer(configuration))
                {
                    layerStatus.LayerInitializationSuccessful(layer, mapData1);

                    // Call
                    bool isSame = layerStatus.HasSameConfiguration(mapData2);

                    // Assert
                    Assert.IsFalse(isSame);
                }
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(SameMapDataTypes))]
        public void HasSameConfiguration_LayerInitializedSameMapDataType_ReturnFalse(ImageBasedMapData mapData1, ImageBasedMapData mapData2)
        {
            // Setup
            var mocks = new MockRepository();
            var tileFetcher = mocks.Stub<ITileFetcher>();
            IConfiguration configuration = CreateStubConfiguration(mocks, tileFetcher);
            mocks.ReplayAll();

            using (var layerStatus = new MapControlBackgroundLayerStatus())
            {
                using (var layer = new BruTileLayer(configuration))
                {
                    layerStatus.LayerInitializationSuccessful(layer, mapData1);

                    // Call
                    bool isSame = layerStatus.HasSameConfiguration(mapData2);

                    // Assert
                    Assert.IsTrue(isSame, "Should recognize same configuration even if instance is not the same.");
                }
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(Configurations), new object[]
        {
            "ClearConfiguration"
        })]
        public void ClearConfiguration_HasLayer_ConfigurationCleared(ImageBasedMapData mapData)
        {
            // Setup
            var mocks = new MockRepository();
            var tileFetcher = mocks.Stub<ITileFetcher>();
            IConfiguration configuration = CreateStubConfiguration(mocks, tileFetcher);
            mocks.ReplayAll();

            using (var layer = new BruTileLayer(configuration))
            using (var layerStatus = new MapControlBackgroundLayerStatus())
            {
                layerStatus.LayerInitializationSuccessful(layer, mapData);

                // Call
                layerStatus.ClearConfiguration();

                // Assert
                Assert.IsFalse(layerStatus.PreviousBackgroundLayerCreationFailed);
                Assert.IsFalse(layerStatus.HasSameConfiguration(mapData));
            }
        }

        [Test]
        [TestCaseSource(nameof(Configurations), new object[]
        {
            "ClearConfigurationWithFailedInitialization"
        })]
        public void ClearConfiguration_WithFailedLayerInitializationAndExpectingRecreation_ConfigurationClearedButKeepFailedFlagSet(ImageBasedMapData mapData)
        {
            // Setup
            using (var layerStatus = new MapControlBackgroundLayerStatus())
            {
                layerStatus.LayerInitializationFailed();

                // Call
                layerStatus.ClearConfiguration(true);

                // Assert
                Assert.IsTrue(layerStatus.PreviousBackgroundLayerCreationFailed);
                Assert.IsFalse(layerStatus.HasSameConfiguration(mapData));
            }
        }

        private static IEnumerable<TestCaseData> Configurations(string prefix)
        {
            yield return new TestCaseData(WmtsMapDataTestHelper.CreateDefaultPdokMapData())
                .SetName($"{prefix}_Wmts");
            yield return new TestCaseData(new WellKnownTileSourceMapData(new Random(21).NextEnum<WellKnownTileSource>()))
                .SetName($"{prefix}_WellKnown");
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
    }
}