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

using Core.Components.Gis.Data;
using NUnit.Framework;

namespace Core.Components.Gis.TestUtil.Test
{
    [TestFixture]
    public class WmtsMapDataTestHelperTest
    {
        [Test]
        public void CreateDefaultPdokMapData_ReturnsInitializedWmtsMapData()
        {
            // Call
            WmtsMapData mapData = WmtsMapDataTestHelper.CreateDefaultPdokMapData();

            // Assert
            Assert.AreEqual("PDOK achtergrondkaart", mapData.Name);
            Assert.AreEqual("https://geodata.nationaalgeoregister.nl/wmts/top10nlv2?VERSION=1.0.0&request=GetCapabilities", mapData.SourceCapabilitiesUrl);
            Assert.AreEqual("brtachtergrondkaart(EPSG:28992)", mapData.SelectedCapabilityIdentifier);
            Assert.AreEqual("image/png", mapData.PreferredFormat);
            Assert.AreEqual(0.0, mapData.Transparency.Value);
            Assert.IsTrue(mapData.IsConfigured);
            Assert.IsTrue(mapData.IsVisible);
        }

        [Test]
        public void CreateAlternativePdokMapData_ReturnsInitializedWmtsMapData()
        {
            // Call
            WmtsMapData mapData = WmtsMapDataTestHelper.CreateAlternativePdokMapData();

            // Assert
            Assert.AreEqual("PDOK achtergrondkaart", mapData.Name);
            Assert.AreEqual("https://geodata.nationaalgeoregister.nl/wmts/top10nlv2?VERSION=1.0.0&request=GetCapabilities", mapData.SourceCapabilitiesUrl);
            Assert.AreEqual("brtachtergrondkaart(EPSG:25831:RWS)", mapData.SelectedCapabilityIdentifier);
            Assert.AreEqual("image/png", mapData.PreferredFormat);
            Assert.AreEqual(0.0, mapData.Transparency.Value);
            Assert.IsTrue(mapData.IsConfigured);
            Assert.IsTrue(mapData.IsVisible);
        }

        [Test]
        public void CreateUnconnectedMapData_ReturnsUnconfiguredWmtsMapData()
        {
            // Call
            WmtsMapData mapData = WmtsMapDataTestHelper.CreateUnconnectedMapData();

            // Assert
            Assert.AreEqual("<niet bepaald>", mapData.Name);
            Assert.IsNull(mapData.SourceCapabilitiesUrl);
            Assert.IsNull(mapData.SelectedCapabilityIdentifier);
            Assert.IsNull(mapData.PreferredFormat);
            Assert.AreEqual(0.0, mapData.Transparency.Value);
            Assert.IsFalse(mapData.IsConfigured);
            Assert.IsFalse(mapData.IsVisible);
        }
    }
}