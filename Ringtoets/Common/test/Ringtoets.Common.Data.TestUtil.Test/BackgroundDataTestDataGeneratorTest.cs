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
            Assert.AreEqual(BackgroundMapDataType.Wmts, backgroundData.BackgroundMapDataType);
            Assert.AreEqual(mapData.Name, backgroundData.Name);
            Assert.AreEqual(mapData.IsConfigured, backgroundData.IsConfigured);
            Assert.AreEqual(mapData.IsVisible, backgroundData.IsVisible);
            Assert.AreEqual(mapData.Transparency, backgroundData.Transparency);
            Assert.AreEqual(3, backgroundData.Parameters.Count);
            Assert.AreEqual(mapData.SourceCapabilitiesUrl, backgroundData.Parameters[BackgroundDataIdentifiers.SourceCapabilitiesUrl]);
            Assert.AreEqual(mapData.SelectedCapabilityIdentifier, backgroundData.Parameters[BackgroundDataIdentifiers.SelectedCapabilityIdentifier]);
            Assert.AreEqual(mapData.PreferredFormat, backgroundData.Parameters[BackgroundDataIdentifiers.PreferredFormat]);
        }

        [Test]
        public void GetWmtsBackgroundMapData_NotConfiguredWmtsMapData_ReturnBackgroundDataWithoutParametersSet()
        {
            // Setup
            WmtsMapData mapData = WmtsMapData.CreateUnconnectedMapData();

            // Call
            BackgroundData backgroundData = BackgroundDataTestDataGenerator.GetWmtsBackgroundMapData(mapData);

            // Assert
            Assert.AreEqual(BackgroundMapDataType.Wmts, backgroundData.BackgroundMapDataType);
            Assert.AreEqual(mapData.Name, backgroundData.Name);
            Assert.AreEqual(mapData.IsConfigured, backgroundData.IsConfigured);
            Assert.AreEqual(mapData.IsVisible, backgroundData.IsVisible);
            Assert.AreEqual(mapData.Transparency, backgroundData.Transparency);
            CollectionAssert.IsEmpty(backgroundData.Parameters);
        }

        [Test]
        public void GetWellKnownBackgroundMapData_Always_ReturnBackgroundDataWithTypeSetToWellKnown()
        {
            // Call
            BackgroundData backgroundData = BackgroundDataTestDataGenerator.GetWellKnownBackgroundMapData();

            // Assert
            Assert.AreEqual(BackgroundMapDataType.WellKnown, backgroundData.BackgroundMapDataType);
            Assert.AreEqual("BingAerial", backgroundData.Name);
            Assert.IsTrue(backgroundData.IsConfigured);
            Assert.IsTrue(backgroundData.IsVisible);
            Assert.AreEqual(0, backgroundData.Transparency.Value);
            Assert.AreEqual(1, backgroundData.Parameters.Count);
            Assert.AreEqual("1", backgroundData.Parameters[BackgroundDataIdentifiers.WellKnownTileSource]);
        }
    }
}