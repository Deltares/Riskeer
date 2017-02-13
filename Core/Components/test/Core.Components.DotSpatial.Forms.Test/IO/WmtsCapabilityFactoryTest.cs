﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Linq;
using Core.Components.DotSpatial.Forms.IO;
using Core.Components.DotSpatial.Layer.BruTile.Configurations;
using Core.Components.DotSpatial.TestUtil;
using Core.Components.Gis.Data;
using NUnit.Framework;

namespace Core.Components.DotSpatial.Forms.Test.IO
{
    [TestFixture]
    public class WmtsCapabilityFactoryTest
    {
        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void GetWmtsCapabilities_CapabilitiesUrlInvalid_ThrowsCannotFindTileSourceException(string url)
        {
            // Call
            TestDelegate call = () => WmtsCapabilityFactory.GetWmtsCapabilities(url).ToArray();

            // Assert
            CannotFindTileSourceException exception = Assert.Throws<CannotFindTileSourceException>(call);
            Assert.AreEqual($"Niet in staat om de databronnen op te halen bij de WMTS url '{url}'.", exception.Message);
        }

        [Test]
        public void GetWmtsCapabilities_ValidUrl_ReturnsWmtsCapabilities()
        {
            // Setup
            string url = "validUrl";
            WmtsMapData backgroundMapData = WmtsMapData.CreateDefaultPdokMapData();

            using (new UseCustomTileSourceFactoryConfig(backgroundMapData))
            {
                // Call
                WmtsCapability[] capabilities = WmtsCapabilityFactory.GetWmtsCapabilities(url).ToArray();

                // Assert
                Assert.AreEqual(1, capabilities.Length);
                WmtsCapability capability = capabilities[0];
                Assert.AreEqual("brtachtergrondkaart(EPSG:28992)", capability.Id);
                Assert.AreEqual("Stub schema", capability.Title);
                Assert.AreEqual("image/png", capability.Format);
                Assert.AreEqual("EPSG:28992", capability.CoordinateSystem);
            }
        }
    }
}