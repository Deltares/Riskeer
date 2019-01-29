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
using System.ComponentModel;
using Core.Common.TestUtil;
using Core.Common.Util.TestUtil.Settings;
using Core.Components.BruTile.Configurations;
using Core.Components.BruTile.IO;
using Core.Components.BruTile.TestUtil;
using Core.Components.Gis.Data;
using NUnit.Framework;

namespace Core.Components.BruTile.Test.Configurations
{
    [TestFixture]
    public class WellKnownTileSourceLayerConfigurationTest
    {
        private DirectoryDisposeHelper directoryDisposeHelper;
        private TestSettingsHelper testSettingsHelper;

        [Test]
        public void CreateInitializedConfiguration_InvalidWellKnownTileSource_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const int invalidTileSource = 9999;

            // Call
            TestDelegate call = () => WellKnownTileSourceLayerConfiguration.CreateInitializedConfiguration((WellKnownTileSource) invalidTileSource);

            // Assert
            string exoectedMessage = $"The value of argument 'wellKnownTileSource' ({invalidTileSource}) is invalid for Enum type '{nameof(WellKnownTileSource)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, exoectedMessage).ParamName;
            Assert.AreEqual("wellKnownTileSource", parameterName);
        }

        [Test]
        public void GivenAllAvailableKnownTileSources_WhenCreatingInitializedConfiguration_ThenConfigurationHasExpectedValues()
        {
            // Given
            using (new UseCustomSettingsHelper(testSettingsHelper))
            {
                foreach (WellKnownTileSource wellKnownTileSource in Enum.GetValues(typeof(WellKnownTileSource)))
                {
                    using (new UseCustomTileSourceFactoryConfig(new WellKnownTileSourceMapData(wellKnownTileSource)))
                    {
                        // When
                        using (WellKnownTileSourceLayerConfiguration configuration = WellKnownTileSourceLayerConfiguration.CreateInitializedConfiguration(wellKnownTileSource))
                        {
                            // Then
                            Assert.IsTrue(configuration.Initialized);

                            Assert.IsNotNull(configuration.TileSchema);
                            Assert.IsInstanceOf<AsyncTileFetcher>(configuration.TileFetcher);
                        }
                    }
                }
            }
        }

        [Test]
        public void Clone_FromFullyInitializedConfiguration_ReturnInitializedConfiguration()
        {
            // Setup
            const WellKnownTileSource knownTileSource = WellKnownTileSource.BingAerial;
            using (new UseCustomSettingsHelper(testSettingsHelper))
            using (new UseCustomTileSourceFactoryConfig(new WellKnownTileSourceMapData(knownTileSource)))
            using (WellKnownTileSourceLayerConfiguration configuration = WellKnownTileSourceLayerConfiguration.CreateInitializedConfiguration(knownTileSource))
            {
                // Call
                using (var clone = (WellKnownTileSourceLayerConfiguration) configuration.Clone())
                {
                    // Assert
                    Assert.IsTrue(clone.Initialized);

                    Assert.AreEqual(configuration.TileSchema, clone.TileSchema);
                    Assert.AreEqual(configuration.TileSchema.Name, clone.TileSchema.Name);
                    Assert.AreEqual(configuration.TileSchema.Format, clone.TileSchema.Format);

                    Assert.IsInstanceOf<AsyncTileFetcher>(clone.TileFetcher);
                }
            }
        }

        [Test]
        public void Clone_ConfigurationDisposed_ThrowObjectDisposedException()
        {
            // Setup
            using (new UseCustomSettingsHelper(testSettingsHelper))
            using (new UseCustomTileSourceFactoryConfig(new WellKnownTileSourceMapData(WellKnownTileSource.BingAerial)))
            {
                WellKnownTileSourceLayerConfiguration configuration = WellKnownTileSourceLayerConfiguration.CreateInitializedConfiguration(WellKnownTileSource.BingAerial);
                configuration.Dispose();

                // Call
                TestDelegate call = () => configuration.Clone();

                // Assert
                string objectName = Assert.Throws<ObjectDisposedException>(call).ObjectName;
                Assert.AreEqual("WellKnownTileSourceLayerConfiguration", objectName);
            }
        }

        [Test]
        public void Initialize_InitializedTrue()
        {
            // Setup
            using (new UseCustomSettingsHelper(testSettingsHelper))
            using (new UseCustomTileSourceFactoryConfig(new WellKnownTileSourceMapData(WellKnownTileSource.BingAerial)))
            using (WellKnownTileSourceLayerConfiguration configuration = WellKnownTileSourceLayerConfiguration.CreateInitializedConfiguration(WellKnownTileSource.BingAerial))
            {
                configuration.GetType()
                             .GetProperty(nameof(configuration.Initialized))
                             .SetValue(configuration, false, null);

                // Call
                configuration.Initialize();

                // Assert
                Assert.IsTrue(configuration.Initialized);
            }
        }

        [Test]
        public void Initialize_ConfigurationDisposed_ThrowObjectDisposedException()
        {
            // Setup
            using (new UseCustomSettingsHelper(testSettingsHelper))
            using (new UseCustomTileSourceFactoryConfig(new WellKnownTileSourceMapData(WellKnownTileSource.BingAerial)))
            {
                WellKnownTileSourceLayerConfiguration configuration = WellKnownTileSourceLayerConfiguration.CreateInitializedConfiguration(WellKnownTileSource.BingAerial);
                configuration.Dispose();

                // Call
                TestDelegate call = () => configuration.Initialize();

                // Assert
                string objectName = Assert.Throws<ObjectDisposedException>(call).ObjectName;
                Assert.AreEqual("WellKnownTileSourceLayerConfiguration", objectName);
            }
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