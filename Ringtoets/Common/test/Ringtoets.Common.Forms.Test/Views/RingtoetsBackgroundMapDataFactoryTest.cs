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
using Core.Common.Base.Data;
using Core.Components.Gis.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.Test.Views
{
    [TestFixture]
    public class RingtoetsBackgroundMapDataFactoryTest
    {
        [Test]
        public void CreateBackgroundMapData_BackgroundMapDataNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => RingtoetsBackgroundMapDataFactory.CreateBackgroundMapData(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("backgroundMapData", exception.ParamName);
        }

        [Test]
        public void CreateBackgroundMapData_BackgroundMapDataTypeWellKnown_ReturnNull()
        {
            // Setup
            BackgroundData backgroundData = BackgroundMapDataTestDataGenerator.GetWellKnownBackgroundMapData();

            // Call
            WmtsMapData mapData = RingtoetsBackgroundMapDataFactory.CreateBackgroundMapData(backgroundData);

            // Assert
            Assert.IsNull(mapData);
        }

        [Test]
        public void CreateBackgroundMapData_BackgroundMapDataTypeWmtsAndConfigured_ReturnConfiguredMapData()
        {
            // Setup
            WmtsMapData wmtsMapData = WmtsMapData.CreateDefaultPdokMapData();
            BackgroundData backgroundData = BackgroundMapDataTestDataGenerator.GetWmtsBackgroundMapData(wmtsMapData);

            // Call
            WmtsMapData mapData = RingtoetsBackgroundMapDataFactory.CreateBackgroundMapData(backgroundData);

            // Assert
            Assert.AreEqual(wmtsMapData.Name, mapData.Name);
            Assert.AreEqual(wmtsMapData.IsVisible, mapData.IsVisible);
            Assert.AreEqual(wmtsMapData.IsConfigured, mapData.IsConfigured);
            Assert.AreEqual(wmtsMapData.Transparency, mapData.Transparency);
            Assert.AreEqual(wmtsMapData.SourceCapabilitiesUrl, mapData.SourceCapabilitiesUrl);
            Assert.AreEqual(wmtsMapData.SelectedCapabilityIdentifier, mapData.SelectedCapabilityIdentifier);
            Assert.AreEqual(wmtsMapData.PreferredFormat, mapData.PreferredFormat);
        }

        [Test]
        public void CreateBackgroundMapData_BackgroundMapDataTypeWmtsAndNotConfigured_ReturnUnconfiguredMapData()
        {
            // Setup
            WmtsMapData wmtsMapData = WmtsMapData.CreateUnconnectedMapData();
            BackgroundData backgroundData = BackgroundMapDataTestDataGenerator.GetWmtsBackgroundMapData(wmtsMapData);

            // Call
            WmtsMapData mapData = RingtoetsBackgroundMapDataFactory.CreateBackgroundMapData(backgroundData);

            // Assert
            Assert.AreEqual(wmtsMapData.Name, mapData.Name);
            Assert.AreEqual(wmtsMapData.IsVisible, mapData.IsVisible);
            Assert.AreEqual(wmtsMapData.IsConfigured, mapData.IsConfigured);
            Assert.AreEqual(wmtsMapData.Transparency, mapData.Transparency);
            Assert.IsNull(mapData.SourceCapabilitiesUrl);
            Assert.IsNull(mapData.SelectedCapabilityIdentifier);
            Assert.IsNull(mapData.PreferredFormat);
        }

        [Test]
        public void UpdateBackgroundMapData_WmtsMapDataNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => RingtoetsBackgroundMapDataFactory.UpdateBackgroundMapData(null, new BackgroundData());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("mapData", exception.ParamName);
        }

        [Test]
        public void UpdateBackgroundMapData_BackgroundMapDataNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => RingtoetsBackgroundMapDataFactory.UpdateBackgroundMapData(WmtsMapData.CreateUnconnectedMapData(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("backgroundMapData", exception.ParamName);
        }

        [Test]
        public void UpdateBackgroundMapData_BackgroundMapDataTypeWellKnown_ReturnUnchangedWmtsMapData()
        {
            // Setup
            WmtsMapData wmtsMapData = WmtsMapData.CreateDefaultPdokMapData();
            BackgroundData backgroundData = BackgroundMapDataTestDataGenerator.GetWellKnownBackgroundMapData();

            string originalName = wmtsMapData.Name;
            bool originalVisibility = wmtsMapData.IsVisible;
            RoundedDouble originalTransparancy = wmtsMapData.Transparency;

            backgroundData.Name = "new name";
            backgroundData.IsVisible = true;
            backgroundData.Transparency = (RoundedDouble) 0.4;

            // Call
            RingtoetsBackgroundMapDataFactory.UpdateBackgroundMapData(wmtsMapData, backgroundData);

            // Assert
            Assert.AreEqual(originalName, wmtsMapData.Name);
            Assert.AreEqual(originalVisibility, wmtsMapData.IsVisible);
            Assert.AreEqual(originalTransparancy, wmtsMapData.Transparency);
        }

        [Test]
        public void UpdateBackgroundMapData_BackgroundMapDataTypeWmtsAndConfigured_ReturnChangedWmtsMapData()
        {
            // Setup
            WmtsMapData wmtsMapData = WmtsMapData.CreateDefaultPdokMapData();
            BackgroundData backgroundData = BackgroundMapDataTestDataGenerator.GetWmtsBackgroundMapData(wmtsMapData);

            string originalSourceCapabilitiesUrl = backgroundData.Parameters["SourceCapabilitiesUrl"];
            string originalSelectedCapabilityIdentifier = backgroundData.Parameters["SelectedCapabilityIdentifier"];
            string originalPreferredFormat = backgroundData.Parameters["PreferredFormat"];

            backgroundData.Parameters["SourceCapabilitiesUrl"] = "some/url";
            backgroundData.Parameters["SelectedCapabilityIdentifier"] = "identifier";
            backgroundData.Parameters["PreferredFormat"] = "image/format";

            // Call
            RingtoetsBackgroundMapDataFactory.UpdateBackgroundMapData(wmtsMapData, backgroundData);

            // Assert
            Assert.AreNotEqual(originalSourceCapabilitiesUrl, wmtsMapData.SourceCapabilitiesUrl);
            Assert.AreNotEqual(originalSelectedCapabilityIdentifier, wmtsMapData.SelectedCapabilityIdentifier);
            Assert.AreNotEqual(originalPreferredFormat, wmtsMapData.PreferredFormat);
        }

        [Test]
        public void GivenConfiguredMapData_WhenBackgroundMapDataConfiguredSetToFalse_ConfigurationRemovedFromWmtsMapData()
        {
            // Given
            WmtsMapData wmtsMapData = WmtsMapData.CreateDefaultPdokMapData();
            BackgroundData backgroundData = BackgroundMapDataTestDataGenerator.GetWmtsBackgroundMapData(wmtsMapData);

            // Precondition
            Assert.IsNotNull(wmtsMapData.SourceCapabilitiesUrl);
            Assert.IsNotNull(wmtsMapData.SelectedCapabilityIdentifier);
            Assert.IsNotNull(wmtsMapData.PreferredFormat);

            // When
            backgroundData.IsConfigured = false;
            RingtoetsBackgroundMapDataFactory.UpdateBackgroundMapData(wmtsMapData, backgroundData);

            // Then
            Assert.IsNull(wmtsMapData.SourceCapabilitiesUrl);
            Assert.IsNull(wmtsMapData.SelectedCapabilityIdentifier);
            Assert.IsNull(wmtsMapData.PreferredFormat);
        }
    }
}