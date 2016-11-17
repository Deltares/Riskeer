// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.ComponentModel;
using Core.Common.Base;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using Core.Plugins.Map.PropertyClasses;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.Map.Test.PropertyClasses
{
    [TestFixture]
    public class MapPolygonDataPropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int typePropertyIndex = 1;
        private const int showLabelsPropertyIndex = 2;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new MapPolygonDataProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<MapPolygonData>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void Data_SetNewMapPolygonDataInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            MapPolygonData mapPolygonData = new MapPolygonData("Test");
            var properties = new MapPolygonDataProperties();

            // Call
            properties.Data = mapPolygonData;

            // Assert
            Assert.AreEqual(mapPolygonData.Name, properties.Name);
            Assert.AreEqual("Polygonen", properties.Type);
            Assert.AreEqual(mapPolygonData.ShowLabels, properties.ShowLabels);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            MapPolygonData mapPolygonData = new MapPolygonData("Test");

            // Call
            var properties = new MapPolygonDataProperties
            {
                Data = mapPolygonData
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(3, dynamicProperties.Count);

            const string generalCategory = "Algemeen";
            const string labelCategory = "Label";

            PropertyDescriptor nameProperty = dynamicProperties[namePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            generalCategory,
                                                                            "Naam",
                                                                            "De naam van de kaartlaag.",
                                                                            true);

            PropertyDescriptor typeProperty = dynamicProperties[typePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(typeProperty,
                                                                            generalCategory,
                                                                            "Type",
                                                                            "Type van de data dat wordt getoond op de kaartlaag.",
                                                                            true);

            PropertyDescriptor showlabelsProperty = dynamicProperties[showLabelsPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(showlabelsProperty,
                                                                            labelCategory,
                                                                            "Weergeven",
                                                                            "Geeft aan of op deze kaartlaag labels moeten worden weergegeven.");
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            MapPolygonData mapPolygonData = new MapPolygonData("Test")
            {
                ShowLabels = true
            };

            mapPolygonData.Attach(observerMock);

            var properties = new MapPolygonDataProperties
            {
                Data = mapPolygonData
            };

            // Call
            properties.ShowLabels = false;

            // Assert
            Assert.AreEqual(properties.ShowLabels, mapPolygonData.ShowLabels);
            mocks.VerifyAll();
        }
    }
}