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
using System.Linq;
using System.Security.AccessControl;
using BruTile;
using BruTile.Web;
using Core.Common.TestUtil;
using Core.Common.Util.TestUtil.Settings;
using Core.Components.BruTile.Configurations;
using Core.Components.BruTile.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.Exceptions;
using Core.Components.Gis.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Components.BruTile.Test.Configurations
{
    [TestFixture]
    public class WmtsLayerConfigurationTest
    {
        private const string validPreferredFormat = "image/png";
        private DirectoryDisposeHelper directoryDisposeHelper;
        private TestSettingsHelper testSettingsHelper;

        [Test]
        public void CreateInitializedConfiguration_CapabilitiesUrlNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => WmtsLayerConfiguration.CreateInitializedConfiguration(null, "A", validPreferredFormat);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("wmtsCapabilitiesUrl", paramName);
        }

        [Test]
        public void CreateInitializedConfiguration_CapabilityIdNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => WmtsLayerConfiguration.CreateInitializedConfiguration("A", null, validPreferredFormat);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("capabilityIdentifier", paramName);
        }

        [Test]
        public void CreateInitializedConfiguration_PreferredFormatNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => WmtsLayerConfiguration.CreateInitializedConfiguration("A", "B", null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("preferredFormat", paramName);
        }

        [Test]
        public void CreateInitializedConfiguration_PreferredFormatNotMime_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => WmtsLayerConfiguration.CreateInitializedConfiguration("A", "B", "png");

            // Assert
            const string message = "Afbeelding formaat moet opgegeven worden als MIME-type.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, message).ParamName;
            Assert.AreEqual("preferredFormat", paramName);
        }

        [Test]
        public void CreateInitializedConfiguration_LayerIdNotInWmts_ThrowCannotFindTileSourceException()
        {
            // Setup
            const string url = "url";
            const string id = "id";
            var mocks = new MockRepository();
            var factory = mocks.Stub<ITileSourceFactory>();
            factory.Stub(f => f.GetWmtsTileSources(url)).Return(Enumerable.Empty<ITileSource>());
            mocks.ReplayAll();

            using (new UseCustomSettingsHelper(testSettingsHelper))
            using (new UseCustomTileSourceFactoryConfig(factory))
            {
                // Call
                TestDelegate call = () => WmtsLayerConfiguration.CreateInitializedConfiguration(url, id, validPreferredFormat);

                // Assert
                string message = Assert.Throws<CannotFindTileSourceException>(call).Message;
                string expectedMessage = $"Niet in staat om de databron met naam '{id}' te kunnen vinden bij de WMTS URL '{url}'.";
                Assert.AreEqual(expectedMessage, message);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CreateInitializedConfiguration_CannotCreateCache_ThrowCannotCreateCacheException()
        {
            // Setup
            WmtsMapData targetMapData = WmtsMapDataTestHelper.CreateDefaultPdokMapData();

            var tileSource = new HttpTileSource(TileSchemaFactory.CreateWmtsTileSchema(targetMapData),
                                                (IRequest) null);

            var mocks = new MockRepository();
            var factory = mocks.Stub<ITileSourceFactory>();
            factory.Stub(f => f.GetWmtsTileSources(targetMapData.SourceCapabilitiesUrl))
                   .Return(new[]
                   {
                       tileSource
                   });
            mocks.ReplayAll();

            using (new UseCustomSettingsHelper(testSettingsHelper))
            using (new UseCustomTileSourceFactoryConfig(factory))
            {
                directoryDisposeHelper.LockDirectory(FileSystemRights.Write);

                // Call
                TestDelegate call = () => WmtsLayerConfiguration.CreateInitializedConfiguration(targetMapData.SourceCapabilitiesUrl,
                                                                                                targetMapData.SelectedCapabilityIdentifier,
                                                                                                targetMapData.PreferredFormat);

                try
                {
                    // Assert
                    string message = Assert.Throws<CannotCreateTileCacheException>(call).Message;
                    const string expectedMessage = "Een kritieke fout is opgetreden bij het aanmaken van de cache.";
                    Assert.AreEqual(expectedMessage, message);
                }
                finally
                {
                    directoryDisposeHelper.UnlockDirectory();
                }
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CreateInitializedConfiguration_MatchingLayerAvailable_ReturnConfiguration()
        {
            // Setup
            WmtsMapData targetMapData = WmtsMapDataTestHelper.CreateAlternativePdokMapData();

            var tileSource1 = new HttpTileSource(TileSchemaFactory.CreateWmtsTileSchema(WmtsMapDataTestHelper.CreateDefaultPdokMapData()),
                                                 (IRequest) null);
            var tileSource2 = new HttpTileSource(TileSchemaFactory.CreateWmtsTileSchema(targetMapData),
                                                 (IRequest) null);
            var tileSources = new ITileSource[]
            {
                tileSource1,
                tileSource2
            };

            var mocks = new MockRepository();
            var factory = mocks.Stub<ITileSourceFactory>();
            factory.Stub(f => f.GetWmtsTileSources(targetMapData.SourceCapabilitiesUrl)).Return(tileSources);
            mocks.ReplayAll();

            using (new UseCustomSettingsHelper(testSettingsHelper))
            using (new UseCustomTileSourceFactoryConfig(factory))
            {
                // Call
                using (WmtsLayerConfiguration configuration = WmtsLayerConfiguration.CreateInitializedConfiguration(targetMapData.SourceCapabilitiesUrl,
                                                                                                                    targetMapData.SelectedCapabilityIdentifier,
                                                                                                                    targetMapData.PreferredFormat))
                {
                    // Assert
                    Assert.IsTrue(configuration.Initialized);
                    Assert.IsTrue(configuration.TileFetcher.IsReady());
                    Assert.AreSame(tileSource2.Schema, configuration.TileSchema);
                }
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Clone_FromFullyInitializedConfiguration_CreateNewUninitializedInstance()
        {
            // Setup
            WmtsMapData targetMapData = WmtsMapDataTestHelper.CreateAlternativePdokMapData();

            var tileSource = new HttpTileSource(TileSchemaFactory.CreateWmtsTileSchema(targetMapData),
                                                (IRequest) null);
            var tileSources = new ITileSource[]
            {
                tileSource
            };

            var mocks = new MockRepository();
            var factory = mocks.Stub<ITileSourceFactory>();
            factory.Stub(f => f.GetWmtsTileSources(targetMapData.SourceCapabilitiesUrl)).Return(tileSources);
            mocks.ReplayAll();

            using (new UseCustomSettingsHelper(testSettingsHelper))
            using (new UseCustomTileSourceFactoryConfig(factory))
            using (WmtsLayerConfiguration configuration = WmtsLayerConfiguration.CreateInitializedConfiguration(targetMapData.SourceCapabilitiesUrl,
                                                                                                                targetMapData.SelectedCapabilityIdentifier,
                                                                                                                targetMapData.PreferredFormat))
            {
                // Call
                IConfiguration clone = configuration.Clone();

                // Assert
                Assert.IsInstanceOf<WmtsLayerConfiguration>(clone);
                Assert.AreNotSame(configuration, clone);

                Assert.IsFalse(clone.Initialized);
                Assert.IsNull(clone.TileFetcher, "TileFetcher should be null because the clone hasn't been initialized yet.");
                Assert.IsNull(clone.TileSchema, "TileSchema should be null because the clone hasn't been initialized yet.");
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Clone_ConfigurationDisposed_ThrowObjectDisposedException()
        {
            // Setup
            WmtsMapData targetMapData = WmtsMapDataTestHelper.CreateAlternativePdokMapData();

            var tileSource = new HttpTileSource(TileSchemaFactory.CreateWmtsTileSchema(targetMapData),
                                                (IRequest) null);
            var tileSources = new ITileSource[]
            {
                tileSource
            };

            var mocks = new MockRepository();
            var factory = mocks.Stub<ITileSourceFactory>();
            factory.Stub(f => f.GetWmtsTileSources(targetMapData.SourceCapabilitiesUrl)).Return(tileSources);
            mocks.ReplayAll();

            using (new UseCustomSettingsHelper(testSettingsHelper))
            using (new UseCustomTileSourceFactoryConfig(factory))
            {
                WmtsLayerConfiguration configuration = WmtsLayerConfiguration.CreateInitializedConfiguration(targetMapData.SourceCapabilitiesUrl,
                                                                                                             targetMapData.SelectedCapabilityIdentifier,
                                                                                                             targetMapData.PreferredFormat);
                configuration.Dispose();

                // Call
                TestDelegate call = () => configuration.Clone();

                // Assert
                string objectName = Assert.Throws<ObjectDisposedException>(call).ObjectName;
                Assert.AreEqual("WmtsLayerConfiguration", objectName);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenFullyInitializedConfiguration_WhenClonedAndInitialized_ConfigurationAreEqual()
        {
            // Given
            WmtsMapData targetMapData = WmtsMapDataTestHelper.CreateAlternativePdokMapData();

            var tileSource = new HttpTileSource(TileSchemaFactory.CreateWmtsTileSchema(targetMapData),
                                                (IRequest) null);
            var tileSources = new ITileSource[]
            {
                tileSource
            };

            var mocks = new MockRepository();
            var factory = mocks.Stub<ITileSourceFactory>();
            factory.Stub(f => f.GetWmtsTileSources(targetMapData.SourceCapabilitiesUrl)).Return(tileSources);
            mocks.ReplayAll();

            using (new UseCustomSettingsHelper(testSettingsHelper))
            using (new UseCustomTileSourceFactoryConfig(factory))
            using (WmtsLayerConfiguration configuration = WmtsLayerConfiguration.CreateInitializedConfiguration(targetMapData.SourceCapabilitiesUrl,
                                                                                                                targetMapData.SelectedCapabilityIdentifier,
                                                                                                                targetMapData.PreferredFormat))
            {
                // When
                IConfiguration clone = configuration.Clone();
                clone.Initialize();

                // Assert
                Assert.IsTrue(clone.Initialized);
                Assert.IsTrue(clone.TileFetcher.IsReady());
                Assert.AreSame(configuration.TileSchema, clone.TileSchema);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Initialize_ConfigurationDisposed_ThrowsObjectDisposedException()
        {
            // Setup
            WmtsMapData targetMapData = WmtsMapDataTestHelper.CreateAlternativePdokMapData();

            var tileSource = new HttpTileSource(TileSchemaFactory.CreateWmtsTileSchema(targetMapData),
                                                (IRequest) null);
            var tileSources = new ITileSource[]
            {
                tileSource
            };

            var mocks = new MockRepository();
            var factory = mocks.Stub<ITileSourceFactory>();
            factory.Stub(f => f.GetWmtsTileSources(targetMapData.SourceCapabilitiesUrl)).Return(tileSources);
            mocks.ReplayAll();

            using (new UseCustomSettingsHelper(testSettingsHelper))
            using (new UseCustomTileSourceFactoryConfig(factory))
            {
                WmtsLayerConfiguration configuration = WmtsLayerConfiguration.CreateInitializedConfiguration(targetMapData.SourceCapabilitiesUrl,
                                                                                                             targetMapData.SelectedCapabilityIdentifier,
                                                                                                             targetMapData.PreferredFormat);
                configuration.Dispose();

                // Call
                TestDelegate call = () => configuration.Initialize();

                // Assert
                string objectName = Assert.Throws<ObjectDisposedException>(call).ObjectName;
                Assert.AreEqual("WmtsLayerConfiguration", objectName);
            }

            mocks.VerifyAll();
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            testSettingsHelper = new TestSettingsHelper
            {
                ApplicationLocalUserSettingsDirectory = TestHelper.GetScratchPadPath(nameof(WellKnownTileSourceLayerConfigurationTest))
            };

            directoryDisposeHelper = new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), nameof(WellKnownTileSourceLayerConfigurationTest));
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            directoryDisposeHelper.Dispose();
        }
    }
}