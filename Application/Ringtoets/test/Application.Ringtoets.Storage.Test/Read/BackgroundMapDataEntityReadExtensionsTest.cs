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
using Core.Components.Gis;
using Core.Components.Gis.Data;
using NUnit.Framework;
using Ringtoets.Common.Data;

namespace Application.Ringtoets.Storage.Test.Read
{
    [TestFixture]
    public class BackgroundMapDataEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowArgumentNullException()
        {
            // Setup
            BackgroundMapDataEntity entity = null;

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

            var entity = new BackgroundMapDataEntity
            {
                Name = name,
                SourceCapabilitiesUrl = url,
                SelectedCapabilityName = capabilityName,
                PreferredFormat = preferredFormat,
                IsVisible = Convert.ToByte(isVisible),
                Transparency = transparancy
            };

            // Call
            BackgroundMapDataContainer container = entity.Read();

            // Assert
            Assert.AreEqual(isVisible, container.IsVisible);
            Assert.AreEqual(transparancy, container.Transparency.Value);

            var wmtsMapData = (WmtsMapData) container.MapData;
            Assert.AreEqual(name, wmtsMapData.Name);
            Assert.AreEqual(url, wmtsMapData.SourceCapabilitiesUrl);
            Assert.AreEqual(capabilityName, wmtsMapData.SelectedCapabilityIdentifier);
            Assert.AreEqual(preferredFormat, wmtsMapData.PreferredFormat);
            Assert.IsTrue(wmtsMapData.IsConfigured);
        }

        [Test]
        public void Read_ConfigurableColumnsNull_ReturnUnConfiguredMapData()
        {
            // Setup
            const string name = "map data";
            const bool isVisible = false;
            const double transparancy = 0.4;

            var entity = new BackgroundMapDataEntity
            {
                Name = name,
                IsVisible = Convert.ToByte(isVisible),
                Transparency = transparancy
            };

            // Call
            BackgroundMapDataContainer container = entity.Read();

            // Assert
            Assert.AreEqual(isVisible, container.IsVisible);
            Assert.AreEqual(transparancy, container.Transparency.Value);

            var wmtsMapData = (WmtsMapData)container.MapData;
            Assert.AreEqual(name, wmtsMapData.Name);
            Assert.IsNull(wmtsMapData.SourceCapabilitiesUrl);
            Assert.IsNull(wmtsMapData.SelectedCapabilityIdentifier);
            Assert.IsNull(wmtsMapData.PreferredFormat);
            Assert.IsFalse(wmtsMapData.IsConfigured);
        }
    }
}