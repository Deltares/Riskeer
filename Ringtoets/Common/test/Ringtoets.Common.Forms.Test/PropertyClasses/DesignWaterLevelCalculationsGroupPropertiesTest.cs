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
using Core.Common.Base;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class DesignWaterLevelCalculationsGroupPropertiesTest
    {
        [Test]
        public void Constructor_LocationsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new DesignWaterLevelCalculationsGroupProperties(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("locations", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var locations = new ObservableList<HydraulicBoundaryLocation>();

            // Call
            var properties = new DesignWaterLevelCalculationsGroupProperties(locations);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<ObservableList<HydraulicBoundaryLocation>>>(properties);
            Assert.AreSame(locations, properties.Data);

            TestHelper.AssertTypeConverter<DesignWaterLevelCalculationsGroupProperties, ExpandableArrayConverter>(
                nameof(DesignWaterLevelCalculationsGroupProperties.Locations));
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Call
            var properties = new DesignWaterLevelCalculationsGroupProperties(new ObservableList<HydraulicBoundaryLocation>());

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(1, dynamicProperties.Count);

            PropertyDescriptor locationsProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(locationsProperty,
                                                                            "Algemeen",
                                                                            "Locaties",
                                                                            "Locaties uit de hydraulische randvoorwaardendatabase.",
                                                                            true);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var location = new TestHydraulicBoundaryLocation();

            // Call
            var properties = new DesignWaterLevelCalculationsGroupProperties(new ObservableList<HydraulicBoundaryLocation>
            {
                location
            });

            // Assert
            Assert.AreEqual(1, properties.Locations.Length);
            Assert.AreSame(location, properties.Locations[0].Data);
        }
    }
}