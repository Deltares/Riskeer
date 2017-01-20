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
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.PropertyClasses;

namespace Ringtoets.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class BackgroundMapDataContextPropertiesTest
    {
        private const int requiredNamePropertyIndex = 0;
        private const int requiredUrlPropertyIndex = 1;
        private const int requiredTransparencyPropertyIndex = 2;
        private const int requiredVisibilityPropertyIndex = 3;

        [Test]
        public void GetProperties_ValidData_ReturnsExpectedValues()
        {
            // Setup
            WmtsMapData mapData = WmtsMapData.CreateDefaultPdokMapData();
            var mapDataContext = new BackgroundMapDataContext(mapData);

            // Call
            var properties = new BackgroundMapDataContextProperties
            {
                Data = mapDataContext
            };

            // Assert
            Assert.IsInstanceOf<ObjectProperties<BackgroundMapDataContext>>(properties);

            Assert.AreSame(mapDataContext, properties.Data);

            Assert.AreEqual(mapData.Name, properties.Name);
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

            var properties = new BackgroundMapDataContextProperties
            {
                Data = new BackgroundMapDataContext(mapData)
            };

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
            var properties = new BackgroundMapDataContextProperties
            {
                Data = new BackgroundMapDataContext(mapData)
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(4, dynamicProperties.Count);

            PropertyDescriptor nameProperty = dynamicProperties[requiredNamePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            "Algemeen",
                                                                            "Naam van de kaartlaag",
                                                                            "Naam van de achtergrond kaartlaag.",
                                                                            true);

            PropertyDescriptor urlProperty = dynamicProperties[requiredUrlPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(urlProperty,
                                                                            "Algemeen",
                                                                            "URL naar de wms / wmts service",
                                                                            "Volledige URL naar de wms of wmts service die als achtergrond kaartlaag gebruikt wordt.",
                                                                            true);

            PropertyDescriptor transparencyPropertyIndex = dynamicProperties[requiredTransparencyPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(transparencyPropertyIndex,
                                                                            "Algemeen",
                                                                            "Transparantie",
                                                                            "Transparantie waarmee de achtergrondlaag wordt weergegeven.",
                                                                            true);

            PropertyDescriptor visibilityProperty = dynamicProperties[requiredVisibilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(visibilityProperty,
                                                                            "Algemeen",
                                                                            "Achtergrondlaag tonen",
                                                                            "Geeft aan of de geselecteerde achtergrondlaag in alle kaarten wordt weergegeven.",
                                                                            true);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GetProperties_UseConfiguredMapData_ReturnsExpectedAttributeValues(bool isMapConfigured)
        {
            // Setup
            WmtsMapData mapData = WmtsMapData.CreateUnconnectedMapData();

            if (isMapConfigured)
            {
                mapData.Configure("https://geodata.nationaalgeoregister.nl/wmts/top10nlv2?VERSION=1.0.0&request=GetCapabilities",
                                  "brtachtergrondkaart",
                                  "image/png");
            }

            // Call
            var properties = new BackgroundMapDataContextProperties
            {
                Data = new BackgroundMapDataContext(mapData)
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(4, dynamicProperties.Count);

            PropertyDescriptor transparencyPropertyIndex = dynamicProperties[requiredTransparencyPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(transparencyPropertyIndex,
                                                                            "Algemeen",
                                                                            "Transparantie",
                                                                            "Transparantie waarmee de achtergrondlaag wordt weergegeven.",
                                                                            !isMapConfigured);

            PropertyDescriptor visibilityProperty = dynamicProperties[requiredVisibilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(visibilityProperty,
                                                                            "Algemeen",
                                                                            "Achtergrondlaag tonen",
                                                                            "Geeft aan of de geselecteerde achtergrondlaag in alle kaarten wordt weergegeven.",
                                                                            !isMapConfigured);
        }
    }
}