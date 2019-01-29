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
    public class BackgroundLayerStatusTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            using (var layerStatus = new SimpleBackgroundLayerStatus())
            {
                // Assert
                Assert.IsInstanceOf<IDisposable>(layerStatus);

                Assert.IsNull(layerStatus.BackgroundLayer);
                Assert.IsFalse(layerStatus.PreviousBackgroundLayerCreationFailed);
            }
        }

        [Test]
        public void HasSameConfiguration_MapDataNull_ThrowArgumentNullException()
        {
            // Setup
            using (var layerStatus = new SimpleBackgroundLayerStatus())
            {
                // Call
                TestDelegate call = () => layerStatus.HasSameConfiguration(null);

                // Assert
                string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
                Assert.AreEqual("mapData", paramName);
            }
        }

        [Test]
        public void LayerInitializationSuccessful_LayerNull_ThrowArgumentNullException()
        {
            // Setup
            using (var layerStatus = new SimpleBackgroundLayerStatus())
            {
                var dataSource = new TestImageBasedMapData("Test", false);

                // Call
                TestDelegate call = () => layerStatus.LayerInitializationSuccessful(null, dataSource);

                // Assert
                string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
                Assert.AreEqual("backgroundLayer", paramName);
            }
        }

        [Test]
        public void LayerInitializationSuccessful_MapDataNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var tileFetcher = mocks.Stub<ITileFetcher>();
            IConfiguration configuration = CreateStubConfiguration(mocks, tileFetcher);
            mocks.ReplayAll();

            using (var layer = new BruTileLayer(configuration))
            using (var layerStatus = new SimpleBackgroundLayerStatus())
            {
                // Call
                TestDelegate call = () => layerStatus.LayerInitializationSuccessful(layer, null);

                // Assert
                string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
                Assert.AreEqual("dataSource", paramName);
            }
        }

        [Test]
        public void LayerInitializationFailed_PreviousBackgroundLayerCreationFailedAndConfigurationClearedTrue()
        {
            // Setup
            using (var layerStatus = new SimpleBackgroundLayerStatus())
            {
                // Call
                layerStatus.LayerInitializationFailed();

                // Assert
                Assert.IsTrue(layerStatus.PreviousBackgroundLayerCreationFailed);
                Assert.IsTrue(layerStatus.ConfigurationCleared);
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

        private class SimpleBackgroundLayerStatus : BackgroundLayerStatus
        {
            public bool ConfigurationCleared { get; private set; }

            public override void ClearConfiguration(bool expectRecreationOfSameBackgroundLayer = false)
            {
                ConfigurationCleared = true;
            }

            protected override void OnLayerInitializationSuccessful(BruTileLayer backgroundLayer, ImageBasedMapData dataSource)
            {
                throw new NotImplementedException();
            }

            protected override bool OnHasSameConfiguration(ImageBasedMapData mapData)
            {
                throw new NotImplementedException();
            }
        }
    }
}