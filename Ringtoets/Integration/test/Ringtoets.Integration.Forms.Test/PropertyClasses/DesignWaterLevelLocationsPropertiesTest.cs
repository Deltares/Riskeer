// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Linq;
using Core.Common.Base;
using Core.Common.Gui.Converters;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Integration.Forms.PropertyClasses;

namespace Ringtoets.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class DesignWaterLevelLocationsPropertiesTest
    {
        private const int requiredLocationsPropertyIndex = 0;

        [Test]
        public void Constructor_GetCalculationFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var hydraulicBoundaryLocations = new ObservableList<HydraulicBoundaryLocation>();

            // Call
            TestDelegate call = () => new DesignWaterLevelLocationsProperties(hydraulicBoundaryLocations, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("getCalculationFunc", exception.ParamName);
        }

        [Test]
        public void Constructor_WithHydraulicBoundaryLocations_ExpectedValues()
        {
            // Setup
            var hydraulicBoundaryLocations = new ObservableList<HydraulicBoundaryLocation>();

            // Call
            var properties = new DesignWaterLevelLocationsProperties(hydraulicBoundaryLocations,
                                                                     hbl => new HydraulicBoundaryLocationCalculation());

            // Assert
            Assert.IsInstanceOf<HydraulicBoundaryLocationsProperties>(properties);
            Assert.AreSame(hydraulicBoundaryLocations, properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            HydraulicBoundaryLocation location = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(1.5);
            var hydraulicBoundaryLocations = new ObservableList<HydraulicBoundaryLocation>
            {
                location
            };

            // Call
            var properties = new DesignWaterLevelLocationsProperties(hydraulicBoundaryLocations,
                                                                     hbl => hbl.DesignWaterLevelCalculation);

            // Assert
            CollectionAssert.AllItemsAreInstancesOfType(properties.Locations, typeof(DesignWaterLevelLocationProperties));
            Assert.AreEqual(1, properties.Locations.Length);
            TestHelper.AssertTypeConverter<DesignWaterLevelLocationsProperties,
                ExpandableArrayConverter>(nameof(DesignWaterLevelLocationsProperties.Locations));

            DesignWaterLevelLocationProperties designWaterLevelLocationProperties = properties.Locations.First();
            Assert.AreEqual(location.Name, designWaterLevelLocationProperties.Name);
            Assert.AreEqual(location.Id, designWaterLevelLocationProperties.Id);
            Assert.AreEqual(location.Location, designWaterLevelLocationProperties.Location);
            Assert.AreEqual(location.DesignWaterLevel, designWaterLevelLocationProperties.DesignWaterLevel, location.DesignWaterLevel.GetAccuracy());
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Call
            var properties = new DesignWaterLevelLocationsProperties(new ObservableList<HydraulicBoundaryLocation>(),
                                                                     hbl => new HydraulicBoundaryLocationCalculation());

            // Assert
            TypeConverter classTypeConverter = TypeDescriptor.GetConverter(properties, true);
            Assert.IsInstanceOf<TypeConverter>(classTypeConverter);

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(1, dynamicProperties.Count);

            PropertyDescriptor locationsProperty = dynamicProperties[requiredLocationsPropertyIndex];
            Assert.IsInstanceOf<ExpandableArrayConverter>(locationsProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(locationsProperty,
                                                                            "Algemeen",
                                                                            "Locaties",
                                                                            "Locaties uit de hydraulische randvoorwaardendatabase.",
                                                                            true);
        }
    }
}