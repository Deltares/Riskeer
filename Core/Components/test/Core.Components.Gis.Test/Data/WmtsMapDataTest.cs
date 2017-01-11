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
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using NUnit.Framework;

namespace Core.Components.Gis.Test.Data
{
    [TestFixture]
    public class WmtsMapDataTest
    {
        private const string url = "https://geodata.nationaalgeoregister.nl/wmts/top10nlv2?VERSION=1.0.0&request=GetCapabilities";
        private const string selectedCapabilityName = "brtachtergrondkaart";

        [Test]
        public void Constructor_ValidArguments_ExpectedValues()
        {
            // Setup
            const string name = "A";

            // Call
            var mapData = new WmtsMapData(name, url, selectedCapabilityName);

            // Assert
            Assert.IsInstanceOf<MapData>(mapData);

            Assert.AreEqual(name, mapData.Name);
            Assert.IsTrue(mapData.IsVisible);

            Assert.AreEqual(url, mapData.SourceCapabilitiesUrl);
            Assert.AreEqual(selectedCapabilityName, mapData.SelectedCapabilityName);

            Assert.AreEqual(2, mapData.Transparency.NumberOfDecimalPlaces);
            Assert.AreEqual(0.0, mapData.Transparency.Value);

            Assert.IsTrue(mapData.IsConfigured);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        public void Constructor_NameInvalid_ThrowArgumentException(string invalidName)
        {
            // Call
            TestDelegate call = () => new WmtsMapData(invalidName, url, selectedCapabilityName);

            // Assert
            string paramName = Assert.Throws<ArgumentException>(call).ParamName;
            Assert.AreEqual("Name", paramName);
        }

        [Test]
        public void Constructor_UrlNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new WmtsMapData("A", null, selectedCapabilityName);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("sourceCapabilitiesUrl", paramName);
        }

        [Test]
        public void Constructor_CapabilityNameNull_ThrowArgumentNullException()
        {
            // Setup

            // Call
            TestDelegate call = () => new WmtsMapData("A", url, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("selectedCapabilityName", paramName);
        }

        [Test]
        [TestCase(-123.56)]
        [TestCase(0.0-1e-2)]
        [TestCase(1.0+1e-2)]
        [TestCase(456.876)]
        [TestCase(double.NaN)]
        public void Transparency_SetInvalidValue_ThrowArgumentOutOfRangeException(double invalidTransparency)
        {
            // Setup
            var mapData= new WmtsMapData("A", url, selectedCapabilityName);

            // Call
            TestDelegate call = () => mapData.Transparency = (RoundedDouble)invalidTransparency;

            // Assert
            var message = "De transparantie moet in het bereik [0.0, 1.0] liggen.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, message).ParamName;
            Assert.AreEqual("value", paramName);
        }

        [Test]
        public void Transparency_SetNewValue_GetNewlySetValueRounded()
        {
            // Setup
            var mapData = new WmtsMapData("A", url, selectedCapabilityName);

            // Call
            mapData.Transparency = (RoundedDouble) 0.5678;

            // Assert
            Assert.AreEqual(2, mapData.Transparency.NumberOfDecimalPlaces);
            Assert.AreEqual(0.57, mapData.Transparency.Value);
        }
        
        [Test]
        public void CreateDefaultPdokMapData_ReturnsInitializedWmtsMapData()
        {
            // Call
            WmtsMapData mapData = WmtsMapData.CreateDefaultPdokMapData();

            // Assert
            Assert.AreEqual("PDOK achtergrondkaart", mapData.Name);
            Assert.AreEqual("https://geodata.nationaalgeoregister.nl/tiles/service/wmts/bgtachtergrond?request=GetCapabilities", mapData.SourceCapabilitiesUrl);
            Assert.AreEqual("brtachtergrondkaart", mapData.SelectedCapabilityName);
            Assert.AreEqual(0.0, mapData.Transparency.Value);
            Assert.IsTrue(mapData.IsConfigured);
        }

        [Test]
        public void CreateUnconnectedMapData_ReturnsUnconfiguredWmtsMapData()
        {
            // Call
            WmtsMapData mapData = WmtsMapData.CreateUnconnectedMapData();

            // Assert
            Assert.AreEqual("<niet bepaald>", mapData.Name);
            Assert.IsNull(mapData.SourceCapabilitiesUrl);
            Assert.IsNull(mapData.SelectedCapabilityName);
            Assert.AreEqual(0.0, mapData.Transparency.Value);
            Assert.IsFalse(mapData.IsConfigured);
        }

        [Test]
        public void Configure_CapabilitiesUrlNull_ThrowArgumentNullException()
        {
            // Setup
            WmtsMapData mapData = WmtsMapData.CreateUnconnectedMapData();

            // Call
            TestDelegate call = () => mapData.Configure(null, selectedCapabilityName);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("sourceCapabilitiesUrl", paramName);
        }

        [Test]
        public void Configure_SelectedCapabilityNameNull_ThrowArgumentNullException()
        {
            // Setup
            WmtsMapData mapData = WmtsMapData.CreateUnconnectedMapData();

            // Call
            TestDelegate call = () => mapData.Configure(url, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("selectedCapabilityName", paramName);
        }

        [Test]
        public void Configure_ValidArguments_SetConnectionProperties()
        {
            // Setup
            WmtsMapData mapData = WmtsMapData.CreateUnconnectedMapData();

            // Call
            mapData.Configure(url, selectedCapabilityName);

            // Assert
            Assert.AreEqual(url, mapData.SourceCapabilitiesUrl);
            Assert.AreEqual(selectedCapabilityName, mapData.SelectedCapabilityName);
            Assert.IsTrue(mapData.IsConfigured);
        }

        [Test]
        public void RemoveConfiguration_MapDataConfigured_ConfigurationRemoved()
        {
            // Setup
            WmtsMapData mapData = WmtsMapData.CreateDefaultPdokMapData();

            // Call
            mapData.RemoveConfiguration();

            // Assert
            Assert.IsNull(mapData.SourceCapabilitiesUrl);
            Assert.IsNull(mapData.SelectedCapabilityName);
            Assert.IsFalse(mapData.IsConfigured);
        }
    }
}