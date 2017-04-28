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
using System.Linq;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Integration.Forms.PropertyClasses;

namespace Ringtoets.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class WaveHeightLocationsContextPropertiesTest
    {
        private const int requiredLocationsPropertyIndex = 0;

        [Test]
        public void Constructor_WithoutDatabase_ExpectedValues()
        {
            // Call
            TestDelegate test = () => new WaveHeightLocationsContextProperties(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("database", paramName);
        }

        [Test]
        public void Constructor_WithDatabase_ExpectedValues()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            // Call
            var properties = new WaveHeightLocationsContextProperties(hydraulicBoundaryDatabase);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<HydraulicBoundaryDatabase>>(properties);
            Assert.AreSame(hydraulicBoundaryDatabase, properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            HydraulicBoundaryLocation location = TestHydraulicBoundaryLocation.CreateWaveHeightCalculated(1.5);
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    location
                }
            };

            // Call
            var properties = new WaveHeightLocationsContextProperties(hydraulicBoundaryDatabase);

            // Assert
            CollectionAssert.AllItemsAreInstancesOfType(properties.Locations, typeof(WaveHeightLocationContextProperties));
            Assert.AreEqual(1, properties.Locations.Length);
            TestHelper.AssertTypeConverter<WaveHeightLocationsContextProperties,
                ExpandableArrayConverter>(nameof(WaveHeightLocationsContextProperties.Locations));

            WaveHeightLocationContextProperties waveHeightLocationProperties = properties.Locations.First();
            Assert.AreEqual(location.Name, waveHeightLocationProperties.Name);
            Assert.AreEqual(location.Id, waveHeightLocationProperties.Id);
            Assert.AreEqual(location.Location, waveHeightLocationProperties.Location);
            Assert.AreEqual(location.WaveHeight, waveHeightLocationProperties.WaveHeight, location.WaveHeight.GetAccuracy());
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Call
            var properties = new WaveHeightLocationsContextProperties(new HydraulicBoundaryDatabase());

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