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
using NUnit.Framework;

namespace Core.Components.Gis.Forms.Test
{
    public class WmtsCapabilityTest
    {
        [Test]
        public void Constructor_IdNull_ThrowsArgumentNullException()
        {
            // Setup
            const string format = "image/png";
            const string title = "Eerste kaartlaag";
            const string coordinateSystem = "Coördinatenstelsel";

            // Call
            TestDelegate call = () => new WmtsCapability(null, format, title, coordinateSystem);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("id", paramName);
        }

        [Test]
        public void Constructor_FormatNull_ThrowsArgumentNullException()
        {
            // Setup
            const string id = "laag1(abc)";
            const string title = "Eerste kaartlaag";
            const string coordinateSystem = "Coördinatenstelsel";

            // Call
            TestDelegate call = () => new WmtsCapability(id, null, title, coordinateSystem);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("format", paramName);
        }

        [Test]
        public void Constructor_TitleNull_ThrowsArgumentNullException()
        {
            // Setup
            const string id = "laag1(abc)";
            const string format = "image/png";
            const string coordinateSystem = "Coördinatenstelsel";

            // Call
            TestDelegate call = () => new WmtsCapability(id, format, null, coordinateSystem);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("title", paramName);
        }

        [Test]
        public void Constructor_CoordinateSystemNull_ThrowsArgumentNullException()
        {
            // Setup
            const string id = "laag1(abc)";
            const string format = "image/png";
            const string title = "Eerste kaartlaag";

            // Call
            TestDelegate call = () => new WmtsCapability(id, format, title, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("coordinateSystem", paramName);
        }

        [Test]
        public void Constructor_FormatNotMIMEType_ThrowsArgumentException()
        {
            // Setup
            const string id = "laag1(abc)";
            const string format = "some string";
            const string title = "Eerste kaartlaag";
            const string coordinateSystem = "Coördinatenstelsel";

            // Call
            TestDelegate call = () => new WmtsCapability(id, format, title, coordinateSystem);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, "Specified image format is not a MIME type.");
        }

        [Test]
        public void Constructor_ValidProperties_ExpectedProperties()
        {
            // Setup
            const string id = "laag1(abc)";
            const string format = "image/png";
            const string title = "Eerste kaartlaag";
            const string coordinateSystem = "Coördinatenstelsel";

            // Call
            var wmtsCapability = new WmtsCapability(id, format, title, coordinateSystem);

            // Assert
            Assert.AreEqual(id, wmtsCapability.Id);
            Assert.AreEqual(format, wmtsCapability.Format);
            Assert.AreEqual(title, wmtsCapability.Title);
            Assert.AreEqual(coordinateSystem, wmtsCapability.CoordinateSystem);
        }

        [Test]
        public void ToWmtsMapData_DisplayNameNull_ThrowsArgumentNullException()
        {
            // Setup
            var wmtsCapability = new WmtsCapability("laag1(abc)", "image/png", "Eerste kaartlaag", "Coördinatenstelsel");
            const string sourceCapabilitiesUrl = "sourceCapabilitiesUrl";

            // Call
            TestDelegate call = () => wmtsCapability.ToWmtsMapdata(null, sourceCapabilitiesUrl);

            // Assert
            Assert.Throws<ArgumentException>(call);
        }

        [Test]
        public void ToWmtsMapData_CapabilitiesUrlNull_ThrowsArgumentNullException()
        {
            // Setup
            var wmtsCapability = new WmtsCapability("laag1(abc)", "image/png", "Eerste kaartlaag", "Coördinatenstelsel");
            const string displayName = "displayName";

            // Call
            TestDelegate call = () => wmtsCapability.ToWmtsMapdata(displayName, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("sourceCapabilitiesUrl", paramName);
        }

        [Test]
        public void ToWmtsMapData_ValidParameters_ReturnsNewWmtsMapData()
        {
            // Setup
            const string id = "laag1(abc)";
            const string format = "image/png";
            const string title = "Eerste kaartlaag";
            const string coordinateSystem = "Coördinatenstelsel";
            const string displayName = "displayName";
            const string sourceCapabilitiesUrl = "sourceCapabilitiesUrl";

            var wmtsCapability = new WmtsCapability(id, format, title, coordinateSystem);

            // Call
            WmtsMapData mapData = wmtsCapability.ToWmtsMapdata(displayName, sourceCapabilitiesUrl);

            // Assert
            Assert.IsInstanceOf<WmtsMapData>(mapData);
            Assert.AreEqual(displayName, mapData.Name);
            Assert.AreEqual(sourceCapabilitiesUrl, mapData.SourceCapabilitiesUrl);
        }
    }
}