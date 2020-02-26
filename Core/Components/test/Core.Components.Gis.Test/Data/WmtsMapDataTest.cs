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
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.TestUtil;
using NUnit.Framework;

namespace Core.Components.Gis.Test.Data
{
    [TestFixture]
    public class WmtsMapDataTest
    {
        private const string url = "https://geodata.nationaalgeoregister.nl/wmts/top10nlv2?VERSION=1.0.0&request=GetCapabilities";
        private const string capabilityIdentifier = "brtachtergrondkaart";
        private const string imageFormat = "image/png";

        [Test]
        public void Constructor_ValidArguments_ExpectedValues()
        {
            // Setup
            const string name = "A";

            // Call
            var mapData = new WmtsMapData(name, url, capabilityIdentifier, imageFormat);

            // Assert
            Assert.IsInstanceOf<ImageBasedMapData>(mapData);

            Assert.AreEqual(name, mapData.Name);
            Assert.IsTrue(mapData.IsVisible);
            Assert.IsTrue(mapData.IsConfigured);

            Assert.AreEqual(url, mapData.SourceCapabilitiesUrl);
            Assert.AreEqual(capabilityIdentifier, mapData.SelectedCapabilityIdentifier);
            Assert.AreEqual(imageFormat, mapData.PreferredFormat);

            Assert.AreEqual(2, mapData.Transparency.NumberOfDecimalPlaces);
            Assert.AreEqual(0.0, mapData.Transparency.Value);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        public void Constructor_NameInvalid_ThrowArgumentException(string invalidName)
        {
            // Call
            TestDelegate call = () => new WmtsMapData(invalidName, url, capabilityIdentifier, imageFormat);

            // Assert
            string paramName = Assert.Throws<ArgumentException>(call).ParamName;
            Assert.AreEqual("Name", paramName);
        }

        [Test]
        public void Constructor_UrlNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new WmtsMapData("A", null, capabilityIdentifier, imageFormat);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("sourceCapabilitiesUrl", paramName);
        }

        [Test]
        public void Constructor_CapabilityNameNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new WmtsMapData("A", url, null, imageFormat);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("selectedCapabilityName", paramName);
        }

        [Test]
        public void Constructor_PreferredImageFormatNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new WmtsMapData("A", url, capabilityIdentifier, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("preferredFormat", paramName);
        }

        [Test]
        public void Constructor_PreferredImageFormatNotInMime_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new WmtsMapData("A", url, capabilityIdentifier, "png");

            // Assert
            const string message = "Specified image format is not a MIME type.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, message).ParamName;
            Assert.AreEqual("preferredFormat", paramName);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        public void SingleParameteredConstructor_NameInvalid_ThrowArgumentException(string invalidName)
        {
            // Call
            TestDelegate call = () => new WmtsMapData(invalidName);

            // Assert
            string paramName = Assert.Throws<ArgumentException>(call).ParamName;
            Assert.AreEqual("Name", paramName);
        }

        [Test]
        public void SingleParameteredConstructor_NameValid_ExpectedValues()
        {
            // Setup
            const string name = "A";

            // Call
            var mapData = new WmtsMapData(name);

            // Assert
            Assert.IsInstanceOf<ImageBasedMapData>(mapData);

            Assert.AreEqual(name, mapData.Name);
            Assert.IsFalse(mapData.IsVisible);
            Assert.IsFalse(mapData.IsConfigured);

            Assert.IsNull(mapData.SourceCapabilitiesUrl);
            Assert.IsNull(mapData.SelectedCapabilityIdentifier);
            Assert.IsNull(mapData.PreferredFormat);

            Assert.AreEqual(2, mapData.Transparency.NumberOfDecimalPlaces);
            Assert.AreEqual(0.0, mapData.Transparency.Value);
        }

        [Test]
        public void Configure_CapabilitiesUrlNull_ThrowArgumentNullException()
        {
            // Setup
            WmtsMapData mapData = WmtsMapDataTestHelper.CreateUnconnectedMapData();

            // Call
            TestDelegate call = () => mapData.Configure(null, capabilityIdentifier, imageFormat);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("sourceCapabilitiesUrl", paramName);
        }

        [Test]
        public void Configure_SelectedCapabilityNameNull_ThrowArgumentNullException()
        {
            // Setup
            WmtsMapData mapData = WmtsMapDataTestHelper.CreateUnconnectedMapData();

            // Call
            TestDelegate call = () => mapData.Configure(url, null, imageFormat);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("selectedCapabilityName", paramName);
        }

        [Test]
        public void Configure_PreferredImageFormatNull_ThrowArgumentNullException()
        {
            // Setup
            WmtsMapData mapData = WmtsMapDataTestHelper.CreateUnconnectedMapData();

            // Call
            TestDelegate call = () => mapData.Configure(url, capabilityIdentifier, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("preferredFormat", paramName);
        }

        [Test]
        public void Configure_PreferredImageFormatNotMime_ThrowArgumentException()
        {
            // Setup
            WmtsMapData mapData = WmtsMapDataTestHelper.CreateUnconnectedMapData();

            // Call
            TestDelegate call = () => mapData.Configure(url, capabilityIdentifier, "png");

            // Assert
            const string message = "Specified image format is not a MIME type.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, message).ParamName;
            Assert.AreEqual("preferredFormat", paramName);
        }

        [Test]
        public void Configure_ValidArguments_SetConnectionProperties()
        {
            // Setup
            WmtsMapData mapData = WmtsMapDataTestHelper.CreateUnconnectedMapData();

            // Call
            mapData.Configure(url, capabilityIdentifier, imageFormat);

            // Assert
            Assert.AreEqual(url, mapData.SourceCapabilitiesUrl);
            Assert.AreEqual(capabilityIdentifier, mapData.SelectedCapabilityIdentifier);
            Assert.IsTrue(mapData.IsConfigured);
            Assert.IsTrue(mapData.IsVisible);
        }

        [Test]
        public void RemoveConfiguration_MapDataConfigured_ConfigurationRemoved()
        {
            // Setup
            WmtsMapData mapData = WmtsMapDataTestHelper.CreateDefaultPdokMapData();

            // Call
            mapData.RemoveConfiguration();

            // Assert
            Assert.IsNull(mapData.SourceCapabilitiesUrl);
            Assert.IsNull(mapData.SelectedCapabilityIdentifier);
            Assert.IsFalse(mapData.IsConfigured);
            Assert.IsFalse(mapData.IsVisible);
            Assert.AreEqual("<niet bepaald>", mapData.Name);
        }
    }
}