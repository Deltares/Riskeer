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
using Core.Components.Gis.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Integration.Forms.PropertyClasses;

namespace Ringtoets.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class BackgroundDataPropertiesTest
    {
        private const int requiredNamePropertyIndex = 0;
        private const int requiredTransparencyPropertyIndex = 1;
        private const int requiredVisibilityPropertyIndex = 2;

        private const int wmtsUrlPropertyIndex = 3;
        private const int wmtsSelectedCapabilityPropertyIndex = 4;
        private const int wmtsPreferredFormatPropertyIndex = 5;

        [Test]
        public void Constructor_BackgroundDataNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new BackgroundDataProperties(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("backgroundData", paramName);
        }

        [Test]
        public void Constructor_ValidBackgroundData_ExpectedValues()
        {
            // Setup
            var backgroundMapData = new BackgroundData();

            // Call
            var properties = new BackgroundDataProperties(backgroundMapData);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<BackgroundData>>(properties);
            Assert.AreSame(backgroundMapData, properties.Data);
        }

        [Test]
        public void GetProperties_EmptyBackgroundData_ReturnExpectedValues()
        {
            // Setup
            var backgroundMapData = new BackgroundData();

            // Call
            var properties = new BackgroundDataProperties(backgroundMapData);

            // Assert
            Assert.AreEqual(backgroundMapData.IsVisible, properties.IsVisible);
            Assert.AreEqual(backgroundMapData.Transparency, properties.Transparency);
            Assert.AreEqual(string.Empty, properties.Name);
            Assert.AreEqual(string.Empty, properties.SourceCapabilitiesUrl);
            Assert.AreEqual(string.Empty, properties.SelectedCapabilityIdentifier);
            Assert.AreEqual(string.Empty, properties.PreferredFormat);
        }

        [Test]
        public void GetProperties_ConfiguredBackgroundData_ReturnExpectedValues()
        {
            // Setup
            const string name = "A";

            var mapData = new TestImageBasedMapData(name, true);

            var backGroundMapData = new BackgroundData
            {
                Name = mapData.Name,
                IsVisible = false,
                Transparency = (RoundedDouble)0.5,
                IsConfigured = mapData.IsConfigured
            };

            // Call
            var properties = new BackgroundDataProperties(backGroundMapData);

            // Assert
            Assert.AreEqual(backGroundMapData.IsVisible, properties.IsVisible);
            Assert.AreEqual(backGroundMapData.Transparency, properties.Transparency);
            Assert.AreEqual(name, properties.Name);
            Assert.AreEqual(string.Empty, properties.SourceCapabilitiesUrl);
            Assert.AreEqual(string.Empty, properties.SelectedCapabilityIdentifier);
            Assert.AreEqual(string.Empty, properties.PreferredFormat);
        }

        [Test]
        public void GetProperties_BackgroundDataWmts_ReturnExpectedValues()
        {
            // Setup
            var mapData = WmtsMapData.CreateDefaultPdokMapData();

            var backgroundMapData = new BackgroundData
            {
                IsVisible = false,
                Transparency = (RoundedDouble)0.5,
                Name = mapData.Name,
                IsConfigured = mapData.IsConfigured,
                BackgroundMapDataType = BackgroundMapDataType.Wmts,
                Parameters =
                {
                    { "SourceCapabilitiesUrl", mapData.SourceCapabilitiesUrl },
                    { "SelectedCapabilityIdentifier", mapData.SelectedCapabilityIdentifier },
                    { "PreferredFormat", mapData.PreferredFormat }
                }
            };

            // Call
            var properties = new BackgroundDataProperties(backgroundMapData);

            // Assert
            Assert.AreEqual(backgroundMapData.IsVisible, properties.IsVisible);
            Assert.AreEqual(backgroundMapData.Transparency, properties.Transparency);
            Assert.AreEqual(mapData.Name, properties.Name);
            Assert.AreEqual(mapData.SourceCapabilitiesUrl, properties.SourceCapabilitiesUrl);
            Assert.AreEqual(mapData.SelectedCapabilityIdentifier, properties.SelectedCapabilityIdentifier);
            Assert.AreEqual(mapData.PreferredFormat, properties.PreferredFormat);
        }

        [Test]
        public void GetProperties_BackgroundDataWithoutParameters_ReturnExpectedValues()
        {
            // Setup
            const string name = "A";

            var mapData = new TestImageBasedMapData(name, false);

            var backgroundMapData = new BackgroundData
            {
                Name = mapData.Name,
                Transparency = mapData.Transparency,
                IsVisible = mapData.IsVisible,
                IsConfigured = mapData.IsConfigured
            };

            // Call
            var properties = new BackgroundDataProperties(backgroundMapData);

            // Assert
            Assert.AreEqual(backgroundMapData.IsVisible, properties.IsVisible);
            Assert.AreEqual(backgroundMapData.Transparency, properties.Transparency);
            Assert.AreEqual(string.Empty, properties.Name);
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            const int numberOfChangedProperties = 2;

            var mockRepository = new MockRepository();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Times(numberOfChangedProperties);
            mockRepository.ReplayAll();

            var backgroundMapData = new BackgroundData();
            backgroundMapData.Attach(observer);

            var properties = new BackgroundDataProperties(backgroundMapData);

            var random = new Random(123);
            bool newVisibility = random.NextBoolean();
            RoundedDouble newTransparency = random.NextRoundedDouble();

            // Call
            properties.IsVisible = newVisibility;
            properties.Transparency = newTransparency;

            // Assert
            Assert.AreEqual(newTransparency, properties.Transparency, properties.Transparency.GetAccuracy());
            Assert.AreEqual(newVisibility, properties.IsVisible);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_WithoutMapData_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var backgroundMapData = new BackgroundData();

            // Call
            var properties = new BackgroundDataProperties(backgroundMapData);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(3, dynamicProperties.Count);

            PropertyDescriptor nameProperty = dynamicProperties[requiredNamePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            "Algemeen",
                                                                            "Omschrijving",
                                                                            "Omschrijving van de achtergrond kaartlaag.",
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

        [Test]
        public void Constructor_WithUnconfiguredMapData_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var testImageBasedMapData = new TestImageBasedMapData("name", false);
            var backgroundMapData = new BackgroundData
            {
                Name = testImageBasedMapData.Name,
                IsVisible = testImageBasedMapData.IsVisible,
                IsConfigured = testImageBasedMapData.IsConfigured,
                Transparency = testImageBasedMapData.Transparency
            };

            // Call
            var properties = new BackgroundDataProperties(backgroundMapData);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(3, dynamicProperties.Count);

            PropertyDescriptor nameProperty = dynamicProperties[requiredNamePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            "Algemeen",
                                                                            "Omschrijving",
                                                                            "Omschrijving van de achtergrond kaartlaag.",
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

        [Test]
        public void Constructor_WithWmtsMapData_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            WmtsMapData defaultPdokMapData = WmtsMapData.CreateDefaultPdokMapData();
            var backgroundMapData = new BackgroundData
            {
                Name = defaultPdokMapData.Name,
                Transparency = defaultPdokMapData.Transparency,
                IsVisible = defaultPdokMapData.IsVisible,
                IsConfigured = defaultPdokMapData.IsConfigured,
                BackgroundMapDataType = BackgroundMapDataType.Wmts,
                Parameters =
                {
                    { "SourceCapabilitiesUrl", defaultPdokMapData.SourceCapabilitiesUrl },
                    { "SelectedCapabilityIdentifier", defaultPdokMapData.SelectedCapabilityIdentifier },
                    { "PreferredFormat", defaultPdokMapData.PreferredFormat }
                }
            };

            // Call
            var properties = new BackgroundDataProperties(backgroundMapData);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(6, dynamicProperties.Count);

            PropertyDescriptor nameProperty = dynamicProperties[requiredNamePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            "Algemeen",
                                                                            "Omschrijving",
                                                                            "Omschrijving van de achtergrond kaartlaag.",
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

            const string wmtsCategory = "WMTS";
            PropertyDescriptor urlProperty = dynamicProperties[wmtsUrlPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(urlProperty,
                                                                            wmtsCategory,
                                                                            "URL",
                                                                            "Volledige URL naar de Web Map Tile Service (WMTS) die als achtergrond kaartlaag gebruikt wordt.",
                                                                            true);

            PropertyDescriptor selectedCapabilityProperty = dynamicProperties[wmtsSelectedCapabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(selectedCapabilityProperty,
                                                                            wmtsCategory,
                                                                            "Kaartlaag",
                                                                            "De naam van de geselecteerde kaartlaag.",
                                                                            true);

            PropertyDescriptor preferredFormatProperty = dynamicProperties[wmtsPreferredFormatPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(preferredFormatProperty,
                                                                            wmtsCategory,
                                                                            "Formaat",
                                                                            "Het type afbeelding die door de geselecteerde kaartlaag aangeleverd wordt.",
                                                                            true);
        }
    }
}