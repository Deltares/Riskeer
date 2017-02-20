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
using Core.Components.Gis;
using Core.Components.Gis.Data;
using Core.Components.Gis.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Integration.Forms.PropertyClasses;

namespace Ringtoets.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class BackgroundMapDataContainerPropertiesTest
    {
        private const int requiredNamePropertyIndex = 0;
        private const int requiredTransparencyPropertyIndex = 1;
        private const int requiredVisibilityPropertyIndex = 2;

        [Test]
        public void Constructor_ContainerNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new BackgroundMapDataContainerProperties(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("container", paramName);
        }

        [Test]
        public void Constructor_ValidContainer_ExpectedValues()
        {
            // Setup
            var container = new BackgroundMapDataContainer();

            // Call
            var properties = new BackgroundMapDataContainerProperties(container);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<BackgroundMapDataContainer>>(properties);
            Assert.AreSame(container, properties.Data);
        }

        [Test]
        public void GetProperties_ContainerWithoutMapData_ReturnExpectedValues()
        {
            // Setup
            var container = new BackgroundMapDataContainer
            {
                MapData = null
            };

            // Call
            var properties = new BackgroundMapDataContainerProperties(container);

            // Assert
            Assert.AreEqual(container.IsVisible, properties.IsVisible);
            Assert.AreEqual(container.Transparency, properties.Transparency);
            Assert.AreEqual(string.Empty, properties.Name);
        }

        [Test]
        public void GetProperties_ContainerWithConfiguredMapData_ReturnExpectedValues()
        {
            // Setup
            const string name = "A";

            var mapData = new TestImageBasedMapData(name, true);

            var container = new BackgroundMapDataContainer
            {
                MapData = mapData,
                IsVisible = false,
                Transparency = (RoundedDouble)0.5
            };

            // Call
            var properties = new BackgroundMapDataContainerProperties(container);

            // Assert
            Assert.AreEqual(container.IsVisible, properties.IsVisible);
            Assert.AreEqual(container.Transparency, properties.Transparency);
            Assert.AreEqual(name, properties.Name);
        }

        [Test]
        public void GetProperties_ContainerWithUnconfiguredMapData_ReturnExpectedValues()
        {
            // Setup
            const string name = "A";

            var mapData = new TestImageBasedMapData(name, false);

            var container = new BackgroundMapDataContainer
            {
                MapData = mapData
            };

            // Call
            var properties = new BackgroundMapDataContainerProperties(container);

            // Assert
            Assert.AreEqual(container.IsVisible, properties.IsVisible);
            Assert.AreEqual(container.Transparency, properties.Transparency);
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

            var container = new BackgroundMapDataContainer();
            container.Attach(observer);

            var properties = new BackgroundMapDataContainerProperties(container);

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
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var container = new BackgroundMapDataContainer();

            // Call
            var properties = new BackgroundMapDataContainerProperties(container);

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
    }
}