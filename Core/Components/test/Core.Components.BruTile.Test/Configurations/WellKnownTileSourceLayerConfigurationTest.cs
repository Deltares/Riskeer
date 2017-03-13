// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using BruTile;
using BruTile.Predefined;
using Core.Common.Gui.TestUtil.Settings;
using Core.Common.TestUtil;
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
        public void CreateInitializedConfiguration_InvalidKnownTileSource_ThrowNotSupportedException()
        {
            // Setup
            const KnownTileSource invalidValue = (KnownTileSource) 9999;

            // Call
            TestDelegate call = () => WellKnownTileSourceLayerConfiguration.CreateInitializedConfiguration(invalidValue);

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }

        [Test]
        public void WellKnownTileSourceToKnownTileSource_InvalidWellKnownTileSource_ThrowsNotSupportedException()
        {
            // Setup
            const WellKnownTileSource invalidValue = (WellKnownTileSource) 9999;

            // Call
            TestDelegate call = () => WellKnownTileSourceLayerConfiguration.WellKnownTileSourceToKnownTileSource(invalidValue);

            // Assert
            string message = Assert.Throws<NotSupportedException>(call).Message;
            Assert.AreEqual($"Unknown value '{invalidValue}' for 'wellKnownTileSource'", message);
        }

        [Test]
        [TestCaseSource(nameof(ValidWellKnownTileSourceToCorrespondingKnownTileSource))]
        public void WellKnownTileSourceToKnownTileSource_ValidWellKnownTileSource_ReturnsCorrespondingKnownTileSource(
            WellKnownTileSource wellKnownTileSource, KnownTileSource expectedKnownTileSource)
        {
            // Call
            KnownTileSource knownTileSource = WellKnownTileSourceLayerConfiguration.WellKnownTileSourceToKnownTileSource(wellKnownTileSource);

            // Assert
            Assert.AreEqual(expectedKnownTileSource, knownTileSource);
        }

        [Test]
        public void GivenAllAvailableKnownTileSources_WhenCreatingInitializedConfiguration_ThenConfigurationHasExpectedValues()
        {
            // Given
            using (new UseCustomSettingsHelper(testSettingsHelper))
            using (new UseCustomTileSourceFactoryConfig(new WellKnownTileSourceMapData(WellKnownTileSource.BingAerial)))
            {
                foreach (KnownTileSource knownTileSource in Enum.GetValues(typeof(KnownTileSource)))
                {
                    // When
                    using (WellKnownTileSourceLayerConfiguration configuration = WellKnownTileSourceLayerConfiguration.CreateInitializedConfiguration(knownTileSource))
                    {
                        // Then
                        Assert.IsTrue(configuration.Initialized);

                        Assert.IsNotNull(configuration.TileSource);
                        Assert.IsInstanceOf<AsyncTileFetcher>(configuration.TileFetcher);
                    }
                }
            }
        }

        [Test]
        public void Clone_FromFullyInitializedConfiguration_ReturnInitializedConfiguration()
        {
            // Setup
            const KnownTileSource knownTileSource = KnownTileSource.BingAerial;
            using (new UseCustomSettingsHelper(testSettingsHelper))
            using (new UseCustomTileSourceFactoryConfig(new WellKnownTileSourceMapData(WellKnownTileSource.BingAerial)))
            using (WellKnownTileSourceLayerConfiguration configuration = WellKnownTileSourceLayerConfiguration.CreateInitializedConfiguration(knownTileSource))
            {
                // Call
                using (var clone = (WellKnownTileSourceLayerConfiguration) configuration.Clone())
                {
                    // Assert
                    Assert.IsTrue(clone.Initialized);

                    Assert.AreEqual(configuration.TileSource, clone.TileSource);
                    Assert.AreEqual(configuration.TileSource.Name, clone.TileSource.Name);
                    AssertAttribution(configuration.TileSource.Attribution, clone.TileSource.Attribution);
                    Assert.AreEqual(configuration.TileSource.Schema.Format, clone.TileSource.Schema.Format);

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
                WellKnownTileSourceLayerConfiguration configuration = WellKnownTileSourceLayerConfiguration.CreateInitializedConfiguration(KnownTileSource.BingAerial);
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
            using (WellKnownTileSourceLayerConfiguration configuration = WellKnownTileSourceLayerConfiguration.CreateInitializedConfiguration(KnownTileSource.BingAerial))
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
                WellKnownTileSourceLayerConfiguration configuration = WellKnownTileSourceLayerConfiguration.CreateInitializedConfiguration(KnownTileSource.BingAerial);
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

        private static TestCaseData[] ValidWellKnownTileSourceToCorrespondingKnownTileSource()
        {
            return new[]
            {
                new TestCaseData(WellKnownTileSource.EsriWorldTopo, KnownTileSource.EsriWorldTopo),
                new TestCaseData(WellKnownTileSource.EsriWorldShadedRelief, KnownTileSource.EsriWorldShadedRelief),
                new TestCaseData(WellKnownTileSource.BingAerial, KnownTileSource.BingAerial),
                new TestCaseData(WellKnownTileSource.BingHybrid, KnownTileSource.BingHybrid),
                new TestCaseData(WellKnownTileSource.BingRoads, KnownTileSource.BingRoads),
                new TestCaseData(WellKnownTileSource.OpenStreetMap, KnownTileSource.OpenStreetMap)
            };
        }

        private static void AssertAttribution(Attribution expectedAttribution, Attribution actualAttribution)
        {
            if (expectedAttribution == null)
            {
                Assert.IsNull(actualAttribution);
            }
            else
            {
                Assert.AreEqual(expectedAttribution.Text, actualAttribution.Text);
                Assert.AreEqual(expectedAttribution.Url, actualAttribution.Url);
            }
        }
    }
}