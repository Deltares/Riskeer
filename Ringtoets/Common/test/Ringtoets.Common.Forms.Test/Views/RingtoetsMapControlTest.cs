// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Collections.Generic;
using System.Linq;
using BruTile;
using BruTile.Predefined;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.TestUtil.Settings;
using Core.Common.TestUtil;
using Core.Components.BruTile.Configurations;
using Core.Components.BruTile.TestUtil;
using Core.Components.DotSpatial.Forms;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.Test.Views
{
    [TestFixture]
    public class RingtoetsMapControlTest
    {
        private DirectoryDisposeHelper directoryDisposeHelper;
        private TestSettingsHelper testSettingsHelper;

        private static IEnumerable<TestCaseData> BackgroundTypes
        {
            get
            {
                yield return new TestCaseData(BackgroundDataTestDataGenerator.GetWellKnownBackgroundMapData(RingtoetsWellKnownTileSource.BingAerial),
                                              BackgroundDataTestDataGenerator.GetWmtsBackgroundMapData(WmtsMapData.CreateDefaultPdokMapData()))
                    .SetName("WellKnownToWmts");

                yield return new TestCaseData(BackgroundDataTestDataGenerator.GetWmtsBackgroundMapData(WmtsMapData.CreateDefaultPdokMapData()),
                                              BackgroundDataTestDataGenerator.GetWellKnownBackgroundMapData(RingtoetsWellKnownTileSource.BingAerial))
                    .SetName("WmtsToWellKnown");
            }
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var control = new RingtoetsMapControl();

            // Assert
            Assert.IsInstanceOf<MapControl>(control);
            Assert.IsNull(control.BackgroundData);
        }

        [Test]
        public void RemoveAllData_Always_SetDataAndBackgroundMapDataNull()
        {
            // Setup
            WmtsMapData backgroundMapData = WmtsMapData.CreateDefaultPdokMapData();

            using (new UseCustomSettingsHelper(testSettingsHelper))
            using (new UseCustomTileSourceFactoryConfig(backgroundMapData))
            using (var map = new RingtoetsMapControl())
            {
                map.BackgroundMapData = backgroundMapData;
                var mapDataCollection = new MapDataCollection("A");
                mapDataCollection.Add(new MapPointData("points")
                {
                    Features = new[]
                    {
                        new MapFeature(new[]
                        {
                            new MapGeometry(new[]
                            {
                                new[]
                                {
                                    new Point2D(1.1, 2.2)
                                }
                            })
                        })
                    }
                });
                map.Data = mapDataCollection;

                // Precondition
                Assert.IsNotNull(map.Data);
                Assert.IsNotNull(map.BackgroundMapData);

                // Call
                map.RemoveAllData();

                // Assert
                Assert.IsNull(map.Data);
                Assert.IsNull(map.BackgroundData);
                Assert.IsNull(map.BackgroundMapData);
            }
        }

        [Test]
        public void BackgroundData_NotNull_BackgroundMapDataSet()
        {
            // Setup
            WmtsMapData mapData = WmtsMapData.CreateDefaultPdokMapData();
            BackgroundData backgroundData = BackgroundDataTestDataGenerator.GetWmtsBackgroundMapData(mapData);

            var control = new RingtoetsMapControl();

            using (new UseCustomSettingsHelper(testSettingsHelper))
            using (new UseCustomTileSourceFactoryConfig(mapData))
            {
                // Call
                control.BackgroundData = backgroundData;

                // Assert
                Assert.AreSame(backgroundData, control.BackgroundData);
                Assert.IsNotNull(control.BackgroundMapData);
                MapDataTestHelper.AssertImageBasedMapData(backgroundData, control.BackgroundMapData);
            }
        }

        [Test]
        public void GivenBackgroundData_WhenSetToNull_ThenBackgroundMapDataSetToNull()
        {
            // Given
            WmtsMapData mapData = WmtsMapData.CreateDefaultPdokMapData();
            BackgroundData backgroundData = BackgroundDataTestDataGenerator.GetWmtsBackgroundMapData(mapData);

            using (new UseCustomSettingsHelper(testSettingsHelper))
            using (new UseCustomTileSourceFactoryConfig(mapData))
            {
                var control = new RingtoetsMapControl
                {
                    BackgroundData = backgroundData
                };

                // Precondition
                Assert.IsNotNull(control.BackgroundMapData);

                // When
                control.BackgroundData = null;

                // Then
                Assert.IsNull(control.BackgroundMapData);
            }
        }

        [Test]
        [TestCaseSource(nameof(BackgroundTypes))]
        public void GivenBackgroundData_WhenBackgroundDataChangedToOtherTypeAndNotified_ThenNewInstanceSetOnBackgroundMapData(
            BackgroundData originalBackgroundData,
            BackgroundData newBackgroundData)
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            var tileSourceFactory = mocks.StrictMock<ITileSourceFactory>();

            var testWellKnownTileSource = new TestWellKnownTileSource(new WellKnownTileSourceMapData(WellKnownTileSource.BingAerial));

            string sourceCapabilitiesUrl = originalBackgroundData.Configuration is WmtsBackgroundDataConfiguration
                                               ? ((WmtsBackgroundDataConfiguration)originalBackgroundData.Configuration).SourceCapabilitiesUrl
                                               : ((WmtsBackgroundDataConfiguration)newBackgroundData.Configuration).SourceCapabilitiesUrl;

            tileSourceFactory.Expect(tsf => tsf.GetWmtsTileSources(sourceCapabilitiesUrl))
                             .Return(Enumerable.Empty<ITileSource>());
            tileSourceFactory.Expect(tsf => tsf.GetKnownTileSource(KnownTileSource.BingAerial)).Return(testWellKnownTileSource);
            mocks.ReplayAll();

            using (new UseCustomSettingsHelper(testSettingsHelper))
            using (new UseCustomTileSourceFactoryConfig(tileSourceFactory))
            {
                var control = new RingtoetsMapControl
                {
                    BackgroundData = originalBackgroundData
                };

                control.BackgroundMapData.Attach(observer);

                ImageBasedMapData oldMapData = control.BackgroundMapData;

                // When
                originalBackgroundData.Name = newBackgroundData.Name;
                originalBackgroundData.IsVisible = newBackgroundData.IsVisible;
                originalBackgroundData.Transparency = newBackgroundData.Transparency;
                originalBackgroundData.Configuration = newBackgroundData.Configuration;
                originalBackgroundData.NotifyObservers();

                // Then
                Assert.IsNotNull(control.BackgroundMapData);
                Assert.AreNotEqual(oldMapData, control.BackgroundMapData);
                Assert.AreNotEqual(oldMapData.GetType(), control.BackgroundMapData.GetType());
                mocks.VerifyAll(); // Expect no observers notified
            }
        }

        [Test]
        public void GivenBackgroundData_WhenBackgroundDataChangedButSameTypeAndNotified_ThenBackgroundMapDataUpdatedAndNotified()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            WmtsMapData mapData = WmtsMapData.CreateDefaultPdokMapData();
            BackgroundData backgroundData = BackgroundDataTestDataGenerator.GetWmtsBackgroundMapData(mapData);

            using (new UseCustomSettingsHelper(testSettingsHelper))
            using (new UseCustomTileSourceFactoryConfig(mapData))
            {
                var control = new RingtoetsMapControl
                {
                    BackgroundData = backgroundData
                };

                control.BackgroundMapData.Attach(observer);

                ImageBasedMapData oldBackgroundMapData = control.BackgroundMapData;

                // When
                backgroundData.Transparency = (RoundedDouble) 0.3;
                backgroundData.NotifyObservers();

                // Then
                Assert.AreSame(oldBackgroundMapData, control.BackgroundMapData);
                Assert.AreEqual(0.3, control.BackgroundMapData.Transparency.Value);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenConfiguredWmtsBackgroundData_WhenWmtsConfigurationSetToFalseAndNotified_ThenBackgroundMapDataConfigurationRemovedAndNotified()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            WmtsMapData mapData = WmtsMapData.CreateDefaultPdokMapData();
            BackgroundData backgroundData = BackgroundDataTestDataGenerator.GetWmtsBackgroundMapData(mapData);

            using (new UseCustomSettingsHelper(testSettingsHelper))
            using (new UseCustomTileSourceFactoryConfig(mapData))
            {
                var control = new RingtoetsMapControl
                {
                    BackgroundData = backgroundData
                };

                control.BackgroundMapData.Attach(observer);

                ImageBasedMapData oldBackgroundMapData = control.BackgroundMapData;

                // When
                backgroundData.Configuration = new WmtsBackgroundDataConfiguration();
                backgroundData.NotifyObservers();

                // Then
                Assert.AreSame(oldBackgroundMapData, control.BackgroundMapData);

                var newWmtsMapData = (WmtsMapData) control.BackgroundMapData;
                Assert.IsNull(newWmtsMapData.SourceCapabilitiesUrl);
                Assert.IsNull(newWmtsMapData.SelectedCapabilityIdentifier);
                Assert.IsNull(newWmtsMapData.PreferredFormat);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenConfiguredWellKnownBackgroundData_WhenTileSourceChangedAndNotified_ThenBackgroundMapDataUpdatedAndNotified()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            const RingtoetsWellKnownTileSource wellKnownTileSource = RingtoetsWellKnownTileSource.BingAerial;
            var mapData = new WellKnownTileSourceMapData(WellKnownTileSource.BingAerial);
            BackgroundData backgroundData = BackgroundDataTestDataGenerator.GetWellKnownBackgroundMapData(wellKnownTileSource);

            using (new UseCustomSettingsHelper(testSettingsHelper))
            using (new UseCustomTileSourceFactoryConfig(mapData))
            {
                var control = new RingtoetsMapControl
                {
                    BackgroundData = backgroundData
                };

                control.BackgroundMapData.Attach(observer);

                ImageBasedMapData oldBackgroundMapData = control.BackgroundMapData;

                // When
                backgroundData.Configuration = new WellKnownBackgroundDataConfiguration(RingtoetsWellKnownTileSource.BingRoads);
                backgroundData.NotifyObservers();

                // Then
                Assert.AreSame(oldBackgroundMapData, control.BackgroundMapData);

                var newWellKnownMapData = (WellKnownTileSourceMapData) control.BackgroundMapData;
                Assert.AreEqual(WellKnownTileSource.BingRoads, newWellKnownMapData.TileSource);
                mocks.VerifyAll();
            }
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            testSettingsHelper = new TestSettingsHelper
            {
                ApplicationLocalUserSettingsDirectory = TestHelper.GetScratchPadPath(nameof(RingtoetsMapControlTest))
            };

            directoryDisposeHelper = new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), nameof(RingtoetsMapControlTest));
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            directoryDisposeHelper.Dispose();
        }
    }
}