﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.PropertyClasses;

namespace Ringtoets.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class DesignWaterLevelContextPropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new DesignWaterLevelContextProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<DesignWaterLevelContext>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();
            
            DesignWaterLevelContext designWaterLevelContext = new DesignWaterLevelContext(assessmentSectionMock)
            {
                WrappedData =
                {
                    HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
                }
            };

            const double designWaterLevel = 12.34;
            HydraulicBoundaryLocation hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 1.0, 2.0)
            {
                DesignWaterLevel = designWaterLevel
            };
            designWaterLevelContext.WrappedData.HydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation);

            // Call
            DesignWaterLevelContextProperties properties = new DesignWaterLevelContextProperties
            {
                Data = designWaterLevelContext
            };

            // Assert
            CollectionAssert.AllItemsAreInstancesOfType(properties.Locations, typeof(HydraulicBoundaryLocationDesignWaterLevelProperties));
            Assert.AreEqual(1, properties.Locations.Length);

            HydraulicBoundaryLocationDesignWaterLevelProperties designWaterLevelLocationProperties = properties.Locations.First();
            Assert.AreEqual(hydraulicBoundaryLocation.Name, designWaterLevelLocationProperties.Name);
            Assert.AreEqual(hydraulicBoundaryLocation.Id, designWaterLevelLocationProperties.Id);
            Assert.AreEqual(hydraulicBoundaryLocation.Location, designWaterLevelLocationProperties.Location);
            var expectedDesignWaterLevelValue = designWaterLevel.ToString("F2", CultureInfo.InvariantCulture);
            Assert.AreEqual(expectedDesignWaterLevelValue, designWaterLevelLocationProperties.DesignWaterLevel);
            mockRepository.VerifyAll();
        }

        [Test]
        public void PropertyAttributes_ReturnExpectedValues()
        {
            // Setup
            var properties = new DesignWaterLevelContextProperties();

            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            const string expectedLocationsDisplayName = "Locaties";
            const string expectedLocationsDescription = "Locaties uit de hydraulische randvoorwaardendatabase.";
            const string expectedLocationsCategory = "Algemeen";

            // Call
            TypeConverter classTypeConverter = TypeDescriptor.GetConverter(properties, true);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties();
            PropertyDescriptor locationsProperty = dynamicProperties.Find("Locations", false);

            // Assert
            Assert.IsInstanceOf<TypeConverter>(classTypeConverter);

            Assert.IsNotNull(locationsProperty);
            Assert.IsInstanceOf<ExpandableArrayConverter>(locationsProperty.Converter);
            Assert.IsTrue(locationsProperty.IsReadOnly);
            Assert.IsTrue(locationsProperty.IsBrowsable);
            Assert.AreEqual(expectedLocationsDisplayName, locationsProperty.DisplayName);
            Assert.AreEqual(expectedLocationsDescription, locationsProperty.Description);
            Assert.AreEqual(expectedLocationsCategory, locationsProperty.Category);
        }
    }
}