// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using Core.Common.Util.TestUtil.Settings;
using Core.Components.BruTile.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Util.TypeConverters;

namespace Ringtoets.Common.Forms.Test.Views
{
    [TestFixture]
    public class RingtoetsMapControlTest
    {
        private DirectoryDisposeHelper directoryDisposeHelper;
        private TestSettingsHelper testSettingsHelper;

        private static IEnumerable<TestCaseData> BackgroundDataTypeTransitions
        {
            get
            {
                yield return new TestCaseData(BackgroundDataConverter.ConvertTo(new WellKnownTileSourceMapData(WellKnownTileSource.BingAerial)),
                                              BackgroundDataConverter.ConvertTo(WmtsMapDataTestHelper.CreateDefaultPdokMapData()))
                    .SetName("WellKnownToWmts");

                yield return new TestCaseData(BackgroundDataConverter.ConvertTo(WmtsMapDataTestHelper.CreateDefaultPdokMapData()),
                                              BackgroundDataConverter.ConvertTo(new WellKnownTileSourceMapData(WellKnownTileSource.BingAerial)))
                    .SetName("WmtsToWellKnown");
            }
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var ringtoetsMapControl = new RingtoetsMapControl();

            // Assert
            Assert.IsInstanceOf<UserControl>(ringtoetsMapControl);
            Assert.IsNotNull(ringtoetsMapControl.MapControl);
            Assert.IsNull(ringtoetsMapControl.MapControl.Data);
            Assert.IsNull(ringtoetsMapControl.MapControl.BackgroundMapData);
        }

        [Test]
        public void SetAllData_MapDataCollectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var ringtoetsMapControl = new RingtoetsMapControl();
            var backgroundData = new BackgroundData(new TestBackgroundDataConfiguration());

            // Call
            TestDelegate test = () => ringtoetsMapControl.SetAllData(null, backgroundData);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("data", paramName);
        }

        [Test]
        public void SetAllData_BackgroundDataNull_ThrowsArgumentNullException()
        {
            // Setup
            var ringtoetsMapControl = new RingtoetsMapControl();
            var mapDataCollection = new MapDataCollection("Collection");

            // Call
            TestDelegate test = () => ringtoetsMapControl.SetAllData(mapDataCollection, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("background", paramName);
        }

        [Test]
        public void SetAllData_Always_SetsAllDataToMapControl()
        {
            // Setup
            var mapDataCollection = new MapDataCollection("Collection");
            WmtsMapData backgroundMapData = WmtsMapDataTestHelper.CreateDefaultPdokMapData();
            BackgroundData backgroundData = BackgroundDataConverter.ConvertTo(backgroundMapData);

            using (new UseCustomSettingsHelper(testSettingsHelper))
            using (new UseCustomTileSourceFactoryConfig(backgroundMapData))
            using (var ringtoetsMapControl = new RingtoetsMapControl())
            {
                // Call
                ringtoetsMapControl.SetAllData(mapDataCollection, backgroundData);

                // Assert
                Assert.AreSame(mapDataCollection, ringtoetsMapControl.MapControl.Data);
                Assert.IsNotNull(ringtoetsMapControl.MapControl.BackgroundMapData);
                MapDataTestHelper.AssertImageBasedMapData(backgroundData, ringtoetsMapControl.MapControl.BackgroundMapData);
            }
        }

        [Test]
        public void RemoveAllData_Always_RemovesAllDataFromMapControl()
        {
            // Setup
            var mapDataCollection = new MapDataCollection("Collection");
            WmtsMapData backgroundMapData = WmtsMapDataTestHelper.CreateDefaultPdokMapData();
            BackgroundData backgroundData = BackgroundDataConverter.ConvertTo(backgroundMapData);

            using (new UseCustomSettingsHelper(testSettingsHelper))
            using (new UseCustomTileSourceFactoryConfig(backgroundMapData))
            using (var ringtoetsMapControl = new RingtoetsMapControl())
            {
                ringtoetsMapControl.SetAllData(mapDataCollection, backgroundData);

                // Precondition
                Assert.IsNotNull(ringtoetsMapControl.MapControl.Data);
                Assert.IsNotNull(ringtoetsMapControl.MapControl.BackgroundMapData);

                // Call
                ringtoetsMapControl.RemoveAllData();

                // Assert
                Assert.IsNull(ringtoetsMapControl.MapControl.Data);
                Assert.IsNull(ringtoetsMapControl.MapControl.BackgroundMapData);
            }
        }

        [Test]
        [TestCaseSource(nameof(BackgroundDataTypeTransitions))]
        public void GivenBackgroundData_WhenBackgroundDataChangedToOtherTypeAndNotified_ThenNewInstanceSetOnBackgroundMapData(
            BackgroundData originalBackgroundData,
            BackgroundData newBackgroundData)
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            using (new UseCustomSettingsHelper(testSettingsHelper))
            using (new UseCustomTileSourceFactoryConfig(new TestTileSourceFactory(BackgroundDataConverter.ConvertFrom(originalBackgroundData))))
            {
                var ringtoetsMapControl = new RingtoetsMapControl();
                var mapDataCollection = new MapDataCollection("Collection");

                ringtoetsMapControl.SetAllData(mapDataCollection, originalBackgroundData);
                ringtoetsMapControl.MapControl.BackgroundMapData.Attach(observer);

                ImageBasedMapData oldMapData = ringtoetsMapControl.MapControl.BackgroundMapData;

                // When
                originalBackgroundData.Name = newBackgroundData.Name;
                originalBackgroundData.IsVisible = newBackgroundData.IsVisible;
                originalBackgroundData.Transparency = newBackgroundData.Transparency;
                originalBackgroundData.Configuration = newBackgroundData.Configuration;
                originalBackgroundData.NotifyObservers();

                // Then
                Assert.IsNotNull(ringtoetsMapControl.MapControl.BackgroundMapData);
                Assert.AreNotSame(oldMapData, ringtoetsMapControl.MapControl.BackgroundMapData);
                Assert.AreNotEqual(oldMapData.GetType(), ringtoetsMapControl.MapControl.BackgroundMapData.GetType());
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

            WmtsMapData mapData = WmtsMapDataTestHelper.CreateDefaultPdokMapData();
            BackgroundData backgroundData = BackgroundDataConverter.ConvertTo(mapData);
            var mapDataCollection = new MapDataCollection("Collection");

            using (new UseCustomSettingsHelper(testSettingsHelper))
            using (new UseCustomTileSourceFactoryConfig(mapData))
            {
                var ringtoetsMapControl = new RingtoetsMapControl();
                ringtoetsMapControl.SetAllData(mapDataCollection, backgroundData);
                ringtoetsMapControl.MapControl.BackgroundMapData.Attach(observer);

                ImageBasedMapData oldBackgroundMapData = ringtoetsMapControl.MapControl.BackgroundMapData;

                // When
                backgroundData.Transparency = (RoundedDouble) 0.3;
                backgroundData.NotifyObservers();

                // Then
                Assert.AreSame(oldBackgroundMapData, ringtoetsMapControl.MapControl.BackgroundMapData);
                Assert.AreEqual(0.3, ringtoetsMapControl.MapControl.BackgroundMapData.Transparency.Value);
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

            WmtsMapData mapData = WmtsMapDataTestHelper.CreateDefaultPdokMapData();
            BackgroundData backgroundData = BackgroundDataConverter.ConvertTo(mapData);
            var mapDataCollection = new MapDataCollection("Collection");

            using (new UseCustomSettingsHelper(testSettingsHelper))
            using (new UseCustomTileSourceFactoryConfig(mapData))
            {
                var ringtoetsMapControl = new RingtoetsMapControl();
                ringtoetsMapControl.SetAllData(mapDataCollection, backgroundData);
                ringtoetsMapControl.MapControl.BackgroundMapData.Attach(observer);

                ImageBasedMapData oldBackgroundMapData = ringtoetsMapControl.MapControl.BackgroundMapData;

                // When
                backgroundData.Configuration = new WmtsBackgroundDataConfiguration();
                backgroundData.NotifyObservers();

                // Then
                Assert.AreSame(oldBackgroundMapData, ringtoetsMapControl.MapControl.BackgroundMapData);

                var newWmtsMapData = (WmtsMapData) ringtoetsMapControl.MapControl.BackgroundMapData;
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

            var mapData = new WellKnownTileSourceMapData(WellKnownTileSource.BingAerial);
            BackgroundData backgroundData = BackgroundDataConverter.ConvertTo(mapData);
            var mapDataCollection = new MapDataCollection("Collection");

            using (new UseCustomSettingsHelper(testSettingsHelper))
            using (new UseCustomTileSourceFactoryConfig(mapData))
            {
                var ringtoetsMapControl = new RingtoetsMapControl();
                ringtoetsMapControl.SetAllData(mapDataCollection, backgroundData);
                ringtoetsMapControl.MapControl.BackgroundMapData.Attach(observer);

                ImageBasedMapData oldBackgroundMapData = ringtoetsMapControl.MapControl.BackgroundMapData;

                // When
                backgroundData.Configuration = new WellKnownBackgroundDataConfiguration(RingtoetsWellKnownTileSource.BingRoads);
                backgroundData.NotifyObservers();

                // Then
                Assert.AreSame(oldBackgroundMapData, ringtoetsMapControl.MapControl.BackgroundMapData);

                var newWellKnownMapData = (WellKnownTileSourceMapData) ringtoetsMapControl.MapControl.BackgroundMapData;
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