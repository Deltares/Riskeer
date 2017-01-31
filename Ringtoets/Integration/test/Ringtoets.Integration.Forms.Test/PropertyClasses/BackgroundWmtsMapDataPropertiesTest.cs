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
using System.ComponentModel;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Integration.Forms.PropertyClasses;

namespace Ringtoets.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class BackgroundWmtsMapDataPropertiesTest
    {
        private const int requiredNamePropertyIndex = 0;
        private const int requiredUrlPropertyIndex = 1;
        private const int requiredTransparencyPropertyIndex = 2;
        private const int requiredVisibilityPropertyIndex = 3;

        [Test]
        public void Constructor_DataNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new BackgroundWmtsMapDataProperties(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("data", paramName);
        }

        [Test]
        public void GetProperties_ConfiguredMapData_ReturnsExpectedValues()
        {
            // Setup
            WmtsMapData mapData = WmtsMapData.CreateDefaultPdokMapData();

            // Call
            var properties = new BackgroundWmtsMapDataProperties(mapData);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<WmtsMapData>>(properties);

            Assert.AreSame(mapData, properties.Data);

            Assert.AreEqual(mapData.Name, properties.Name);
            Assert.AreEqual(mapData.SourceCapabilitiesUrl, properties.Url);

            Assert.AreEqual(2, properties.Transparency.NumberOfDecimalPlaces);
            Assert.AreEqual(mapData.Transparency, properties.Transparency);
            Assert.AreEqual(mapData.IsVisible, properties.IsVisible);
        }

        [Test]
        public void GetProperties_UnconfiguredMapData_ReturnsExpectedValues()
        {
            // Setup
            WmtsMapData mapData = WmtsMapData.CreateUnconnectedMapData();

            // Call
            var properties = new BackgroundWmtsMapDataProperties(mapData);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<WmtsMapData>>(properties);

            Assert.AreSame(mapData, properties.Data);

            Assert.AreEqual(string.Empty, properties.Name);
            Assert.AreEqual(mapData.SourceCapabilitiesUrl, properties.Url);

            Assert.AreEqual(2, properties.Transparency.NumberOfDecimalPlaces);
            Assert.AreEqual(mapData.Transparency, properties.Transparency);
            Assert.AreEqual(mapData.IsVisible, properties.IsVisible);
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            var mockRepository = new MockRepository();
            var observer = mockRepository.StrictMock<IObserver>();
            const int numberOfChangedProperties = 2;
            observer.Expect(o => o.UpdateObserver()).Repeat.Times(numberOfChangedProperties);
            mockRepository.ReplayAll();

            var random = new Random(21);
            var newTransparency = (RoundedDouble) random.NextDouble();
            var newVisibility = random.NextBoolean();

            WmtsMapData mapData = WmtsMapData.CreateDefaultPdokMapData();
            mapData.Attach(observer);

            var properties = new BackgroundWmtsMapDataProperties(mapData);

            // Call
            properties.IsVisible = newVisibility;
            properties.Transparency = newTransparency;

            // Assert
            Assert.AreEqual(newTransparency, properties.Transparency, properties.Transparency.GetAccuracy());
            Assert.AreEqual(newVisibility, properties.IsVisible);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            WmtsMapData mapData = WmtsMapData.CreateUnconnectedMapData();

            // Call
            var properties = new BackgroundWmtsMapDataProperties(mapData);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(4, dynamicProperties.Count);

            PropertyDescriptor nameProperty = dynamicProperties[requiredNamePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            "Algemeen",
                                                                            "Omschrijving",
                                                                            "Omschrijving van de achtergrond kaartlaag.",
                                                                            true);

            PropertyDescriptor urlProperty = dynamicProperties[requiredUrlPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(urlProperty,
                                                                            "Algemeen",
                                                                            "URL",
                                                                            "Volledige URL naar de Web Map Tile Service (WMTS) die als achtergrond kaartlaag gebruikt wordt.",
                                                                            true);

            PropertyDescriptor transparencyPropertyIndex = dynamicProperties[requiredTransparencyPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(transparencyPropertyIndex,
                                                                            "Algemeen",
                                                                            "Transparantie",
                                                                            "Transparantie waarmee de achtergrond kaartlaag wordt weergegeven.");

            PropertyDescriptor visibilityProperty = dynamicProperties[requiredVisibilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(visibilityProperty,
                                                                            "Algemeen",
                                                                            "Weergeven",
                                                                            "Geeft aan of de geselecteerde achtergrond kaartlaag in alle kaarten van dit traject wordt weergegeven.");
        }
    }
}