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
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Base.Data;
using Core.Components.Gis.Data;
using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test.Create
{
    [TestFixture]
    public class BackgroundMapDataCreateExtensionsTest
    {
        [Test]
        public void Create_WmtsMapDataNull_ThrowArgumentNullException()
        {
            // Setup
            WmtsMapData mapData = null;

            // Call
            TestDelegate test = () => mapData.Create();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("mapData", exception.ParamName);
        }

        [Test]
        public void Create_ValidData_ReturnWmtsMapDataEntity()
        {
            // Setup
            const string name = "background";
            const string sourceCapabilitiesUrl = "//url";
            const string selectedCapabilityName = "selectedName";
            const string preferredFormat = "image/png";
            const bool isVisible = true;
            RoundedDouble transparancy = (RoundedDouble) 0.3;

            var mapData = new WmtsMapData(name, sourceCapabilitiesUrl, selectedCapabilityName, preferredFormat)
            {
                IsVisible = isVisible,
                Transparency = transparancy
            };

            // Call
            var entity = mapData.Create();

            // Assert
            Assert.IsInstanceOf<BackgroundMapDataEntity>(entity);
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(sourceCapabilitiesUrl, entity.SourceCapabilitiesUrl);
            Assert.AreEqual(selectedCapabilityName, entity.SelectedCapabilityName);
            Assert.AreEqual(preferredFormat, entity.PreferredFormat);
            Assert.AreEqual(Convert.ToByte(isVisible), entity.IsVisible);
            Assert.AreEqual(transparancy, entity.Transparency);
        }
    }
}