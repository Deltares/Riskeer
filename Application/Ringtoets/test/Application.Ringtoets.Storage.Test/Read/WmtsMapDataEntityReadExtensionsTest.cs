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
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read;
using Core.Components.Gis.Data;
using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test.Read
{
    [TestFixture]
    public class WmtsMapDataEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowArgumentNullException()
        {
            // Setup
            WmtsMapDataEntity entity = null;

            // Call
            TestDelegate test = () => entity.Read();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_WithEntity_ReturnMapData()
        {
            // Setup
            const string name = "map data";
            const string url = "//url";
            const string capabilityName = "capability name";
            const string preferredFormat = "image/jpeg";
            const bool isVisible = false;
            const double transparancy = 0.4;

            var entity = new WmtsMapDataEntity
            {
                Name = name,
                SourceCapabilitiesUrl = url,
                SelectedCapabilityName = capabilityName,
                PreferredFormat = preferredFormat,
                IsVisible = Convert.ToByte(isVisible),
                Transparency = transparancy
            };

            // Call
            WmtsMapData mapData = entity.Read();

            // Assert
            Assert.AreEqual(name, mapData.Name);
            Assert.AreEqual(url, mapData.SourceCapabilitiesUrl);
            Assert.AreEqual(capabilityName, mapData.SelectedCapabilityIdentifier);
            Assert.AreEqual(isVisible, mapData.IsVisible);
            Assert.AreEqual(transparancy, mapData.Transparency.Value);
            Assert.AreEqual(preferredFormat, mapData.PreferredFormat);
        }
    }
}