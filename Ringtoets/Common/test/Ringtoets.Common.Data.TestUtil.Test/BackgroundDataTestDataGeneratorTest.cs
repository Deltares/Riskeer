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

using System;
using System.ComponentModel;
using Core.Common.TestUtil;
using Core.Common.Utils.Reflection;
using Core.Components.Gis.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;

namespace Ringtoets.Common.Data.TestUtil.Test
{
    [TestFixture]
    public class BackgroundDataTestDataGeneratorTest
    {
        [Test]
        public void GetWmtsBackgroundMapData_ConfiguredWmtsMapData_ReturnBackgroundDataWithParametersSet()
        {
            // Setup
            WmtsMapData mapData = WmtsMapData.CreateDefaultPdokMapData();

            // Call
            BackgroundData backgroundData = BackgroundDataTestDataGenerator.GetWmtsBackgroundMapData(mapData);

            // Assert
            Assert.AreEqual(mapData.Name, backgroundData.Name);
            Assert.AreEqual(mapData.IsVisible, backgroundData.IsVisible);
            Assert.AreEqual(mapData.Transparency, backgroundData.Transparency);
            var configuration = (WmtsBackgroundDataConfiguration) backgroundData.Configuration;
            Assert.AreEqual(mapData.IsConfigured, configuration.IsConfigured);

            Assert.AreEqual(mapData.SourceCapabilitiesUrl, configuration.SourceCapabilitiesUrl);
            Assert.AreEqual(mapData.SelectedCapabilityIdentifier, configuration.SelectedCapabilityIdentifier);
            Assert.AreEqual(mapData.PreferredFormat, configuration.PreferredFormat);
        }

        [Test]
        public void GetWmtsBackgroundMapData_NotConfiguredWmtsMapData_ReturnBackgroundDataWithoutParametersSet()
        {
            // Setup
            WmtsMapData mapData = WmtsMapData.CreateUnconnectedMapData();

            // Call
            BackgroundData backgroundData = BackgroundDataTestDataGenerator.GetWmtsBackgroundMapData(mapData);

            // Assert
            Assert.AreEqual(mapData.Name, backgroundData.Name);
            Assert.AreEqual(mapData.IsVisible, backgroundData.IsVisible);
            Assert.AreEqual(mapData.Transparency, backgroundData.Transparency);

            var configuration = (WmtsBackgroundDataConfiguration) backgroundData.Configuration;
            Assert.AreEqual(mapData.IsConfigured, configuration.IsConfigured);

            Assert.IsNull(configuration.SourceCapabilitiesUrl);
            Assert.IsNull(configuration.SelectedCapabilityIdentifier);
            Assert.IsNull(configuration.PreferredFormat);
        }

        [Test]
        public void GetWellKnownBackgroundMapData_WithValidWellKnownTileSourceValue_ReturnBackgroundDataWithTypeSetToWellKnown()
        {
            // Setup
            var random = new Random(21);
            RingtoetsWellKnownTileSource wellKnownTileSource = random.NextEnumValue<RingtoetsWellKnownTileSource>();

            // Call
            BackgroundData backgroundData = BackgroundDataTestDataGenerator.GetWellKnownBackgroundMapData(wellKnownTileSource);

            // Assert
            Assert.IsTrue(backgroundData.IsVisible);
            Assert.AreEqual(0, backgroundData.Transparency.Value);

            var configuration = (WellKnownBackgroundDataConfiguration) backgroundData.Configuration;

            string backgroundDataName = TypeUtils.GetDisplayName(wellKnownTileSource);
            Assert.AreEqual(backgroundDataName, backgroundData.Name);
            Assert.AreEqual(wellKnownTileSource, configuration.WellKnownTileSource);
        }

        [Test]
        public void GetWellKnownBackgroundMapData_WithInvalidWellKnownTileSourceValue_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const RingtoetsWellKnownTileSource invalidWellKnownTileSource = (RingtoetsWellKnownTileSource) 1337;

            // Call
            TestDelegate call = () => BackgroundDataTestDataGenerator.GetWellKnownBackgroundMapData(invalidWellKnownTileSource);

            // Assert
            Assert.Throws<InvalidEnumArgumentException>(call);
        }
    }
}