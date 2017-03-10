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
using Core.Common.Gui.TestUtil.Settings;
using Core.Components.BruTile.Configurations;
using Core.Components.BruTile.TestUtil;
using Core.Components.DotSpatial.Forms;
using Core.Components.Gis.Data;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.Test.Views
{
    [TestFixture]
    public class RingtoetsMapControlTest
    {
        private static IEnumerable<TestCaseData> BackgroundTypes
        {
            get
            {
                yield return new TestCaseData(BackgroundDataTestDataGenerator.GetWellKnownBackgroundMapData(),
                                              BackgroundDataTestDataGenerator.GetWmtsBackgroundMapData(WmtsMapData.CreateDefaultPdokMapData()))
                    .SetName("WellKnownToWmts");

                yield return new TestCaseData(BackgroundDataTestDataGenerator.GetWmtsBackgroundMapData(WmtsMapData.CreateDefaultPdokMapData()),
                                              BackgroundDataTestDataGenerator.GetWellKnownBackgroundMapData())
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
        public void BackgroundData_NotNull_BackgroundMapDataSet()
        {
            // Setup
            var mapData = WmtsMapData.CreateDefaultPdokMapData();
            BackgroundData backgroundData = BackgroundDataTestDataGenerator.GetWmtsBackgroundMapData(mapData);

            var control = new RingtoetsMapControl();

            using (new UseCustomSettingsHelper(new TestSettingsHelper()))
            using (new UseCustomTileSourceFactoryConfig(mapData))
            {
                // Call
                control.BackgroundData = backgroundData;

                // Assert
                Assert.AreSame(backgroundData, control.BackgroundData);
                Assert.IsNotNull(control.BackgroundMapData);
                Assert.IsTrue(control.BackgroundMapData is WmtsMapData);
            }
        }

        [Test]
        public void GivenBackgroundData_WhenSetToNull_ThenBackgroundMapDataSetToNull()
        {
            // Given
            var mapData = WmtsMapData.CreateDefaultPdokMapData();
            BackgroundData backgroundData = BackgroundDataTestDataGenerator.GetWmtsBackgroundMapData(mapData);

            using (new UseCustomSettingsHelper(new TestSettingsHelper()))
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
            if (originalBackgroundData.BackgroundMapDataType == BackgroundMapDataType.WellKnown && newBackgroundData.BackgroundMapDataType == BackgroundMapDataType.Wmts)
            {
                tileSourceFactory.Expect(tsf => tsf.GetKnownTileSources(KnownTileSource.BingAerial)).Return(testWellKnownTileSource);
                tileSourceFactory.Expect(tsf => tsf.GetWmtsTileSources(newBackgroundData.Parameters[BackgroundDataIdentifiers.SourceCapabilitiesUrl]))
                                 .Return(Enumerable.Empty<ITileSource>());
            }
            else if (originalBackgroundData.BackgroundMapDataType == BackgroundMapDataType.Wmts && newBackgroundData.BackgroundMapDataType == BackgroundMapDataType.WellKnown)
            {
                tileSourceFactory.Expect(tsf => tsf.GetWmtsTileSources(originalBackgroundData.Parameters[BackgroundDataIdentifiers.SourceCapabilitiesUrl]))
                                 .Return(Enumerable.Empty<ITileSource>());
                tileSourceFactory.Expect(tsf => tsf.GetKnownTileSources(KnownTileSource.BingAerial)).Return(testWellKnownTileSource);
            }
            mocks.ReplayAll();

            using (new UseCustomSettingsHelper(new TestSettingsHelper()))
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
                originalBackgroundData.IsConfigured = newBackgroundData.IsConfigured;
                originalBackgroundData.Transparency = newBackgroundData.Transparency;
                originalBackgroundData.BackgroundMapDataType = newBackgroundData.BackgroundMapDataType;
                originalBackgroundData.Parameters.Clear();

                foreach (KeyValuePair<string, string> parameter in newBackgroundData.Parameters)
                {
                    originalBackgroundData.Parameters.Add(parameter);
                }
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

            using (new UseCustomSettingsHelper(new TestSettingsHelper()))
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
                mocks.VerifyAll();
            }
        }
    }
}