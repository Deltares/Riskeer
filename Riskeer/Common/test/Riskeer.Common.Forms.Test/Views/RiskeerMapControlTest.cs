// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.Views;
using Riskeer.Common.Util.TypeConverters;

namespace Riskeer.Common.Forms.Test.Views
{
    [TestFixture]
    public class RiskeerMapControlTest
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
            var riskeerMapControl = new RiskeerMapControl();

            // Assert
            Assert.IsInstanceOf<UserControl>(riskeerMapControl);
            Assert.IsNotNull(riskeerMapControl.MapControl);
            Assert.IsNull(riskeerMapControl.MapControl.Data);
            Assert.IsNull(riskeerMapControl.MapControl.BackgroundMapData);
        }

        [Test]
        public void SetAllData_MapDataCollectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var riskeerMapControl = new RiskeerMapControl();
            var backgroundData = new BackgroundData(new TestBackgroundDataConfiguration());

            // Call
            TestDelegate test = () => riskeerMapControl.SetAllData(null, backgroundData);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("data", paramName);
        }

        [Test]
        public void SetAllData_BackgroundDataNull_ThrowsArgumentNullException()
        {
            // Setup
            var riskeerMapControl = new RiskeerMapControl();
            var mapDataCollection = new MapDataCollection("Collection");

            // Call
            TestDelegate test = () => riskeerMapControl.SetAllData(mapDataCollection, null);

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
            using (var riskeerMapControl = new RiskeerMapControl())
            {
                // Call
                riskeerMapControl.SetAllData(mapDataCollection, backgroundData);

                // Assert
                Assert.AreSame(mapDataCollection, riskeerMapControl.MapControl.Data);
                Assert.IsNotNull(riskeerMapControl.MapControl.BackgroundMapData);
                MapDataTestHelper.AssertImageBasedMapData(backgroundData, riskeerMapControl.MapControl.BackgroundMapData);
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
            using (var riskeerMapControl = new RiskeerMapControl())
            {
                riskeerMapControl.SetAllData(mapDataCollection, backgroundData);

                // Precondition
                Assert.IsNotNull(riskeerMapControl.MapControl.Data);
                Assert.IsNotNull(riskeerMapControl.MapControl.BackgroundMapData);

                // Call
                riskeerMapControl.RemoveAllData();

                // Assert
                Assert.IsNull(riskeerMapControl.MapControl.Data);
                Assert.IsNull(riskeerMapControl.MapControl.BackgroundMapData);
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
                var riskeerMapControl = new RiskeerMapControl();
                var mapDataCollection = new MapDataCollection("Collection");

                riskeerMapControl.SetAllData(mapDataCollection, originalBackgroundData);
                riskeerMapControl.MapControl.BackgroundMapData.Attach(observer);

                ImageBasedMapData oldMapData = riskeerMapControl.MapControl.BackgroundMapData;

                // When
                originalBackgroundData.Name = newBackgroundData.Name;
                originalBackgroundData.IsVisible = newBackgroundData.IsVisible;
                originalBackgroundData.Transparency = newBackgroundData.Transparency;
                originalBackgroundData.Configuration = newBackgroundData.Configuration;
                originalBackgroundData.NotifyObservers();

                // Then
                Assert.IsNotNull(riskeerMapControl.MapControl.BackgroundMapData);
                Assert.AreNotSame(oldMapData, riskeerMapControl.MapControl.BackgroundMapData);
                Assert.AreNotEqual(oldMapData.GetType(), riskeerMapControl.MapControl.BackgroundMapData.GetType());
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
                var riskeerMapControl = new RiskeerMapControl();
                riskeerMapControl.SetAllData(mapDataCollection, backgroundData);
                riskeerMapControl.MapControl.BackgroundMapData.Attach(observer);

                ImageBasedMapData oldBackgroundMapData = riskeerMapControl.MapControl.BackgroundMapData;

                // When
                backgroundData.Transparency = (RoundedDouble) 0.3;
                backgroundData.NotifyObservers();

                // Then
                Assert.AreSame(oldBackgroundMapData, riskeerMapControl.MapControl.BackgroundMapData);
                Assert.AreEqual(0.3, riskeerMapControl.MapControl.BackgroundMapData.Transparency.Value);
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
                var riskeerMapControl = new RiskeerMapControl();
                riskeerMapControl.SetAllData(mapDataCollection, backgroundData);
                riskeerMapControl.MapControl.BackgroundMapData.Attach(observer);

                ImageBasedMapData oldBackgroundMapData = riskeerMapControl.MapControl.BackgroundMapData;

                // When
                backgroundData.Configuration = new WmtsBackgroundDataConfiguration();
                backgroundData.NotifyObservers();

                // Then
                Assert.AreSame(oldBackgroundMapData, riskeerMapControl.MapControl.BackgroundMapData);

                var newWmtsMapData = (WmtsMapData) riskeerMapControl.MapControl.BackgroundMapData;
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
                var riskeerMapControl = new RiskeerMapControl();
                riskeerMapControl.SetAllData(mapDataCollection, backgroundData);
                riskeerMapControl.MapControl.BackgroundMapData.Attach(observer);

                ImageBasedMapData oldBackgroundMapData = riskeerMapControl.MapControl.BackgroundMapData;

                // When
                backgroundData.Configuration = new WellKnownBackgroundDataConfiguration(RiskeerWellKnownTileSource.BingRoads);
                backgroundData.NotifyObservers();

                // Then
                Assert.AreSame(oldBackgroundMapData, riskeerMapControl.MapControl.BackgroundMapData);

                var newWellKnownMapData = (WellKnownTileSourceMapData) riskeerMapControl.MapControl.BackgroundMapData;
                Assert.AreEqual(WellKnownTileSource.BingRoads, newWellKnownMapData.TileSource);
                mocks.VerifyAll();
            }
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            testSettingsHelper = new TestSettingsHelper
            {
                ApplicationLocalUserSettingsDirectory = TestHelper.GetScratchPadPath(nameof(RiskeerMapControlTest))
            };

            directoryDisposeHelper = new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), nameof(RiskeerMapControlTest));
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            directoryDisposeHelper.Dispose();
        }
    }
}