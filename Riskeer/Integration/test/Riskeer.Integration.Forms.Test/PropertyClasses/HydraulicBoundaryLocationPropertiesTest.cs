// Copyright (C) Stichting Deltares 2022. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Gui.PropertyBag;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Integration.Forms.PropertyClasses;

namespace Riskeer.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class HydraulicBoundaryLocationPropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int idPropertyIndex = 1;
        private const int locationPropertyIndex = 2;

        [Test]
        public void Constructor_HydraulicBoundaryDatabaseNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new HydraulicBoundaryDatabaseProperties(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryDatabase", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var hydraulicBoundaryDatabaseLocation = new HydraulicBoundaryLocation(0, "", 0, 0);

            // Call
            var properties = new HydraulicBoundaryLocationProperties(hydraulicBoundaryDatabaseLocation);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<HydraulicBoundaryLocation>>(properties);
            Assert.AreSame(hydraulicBoundaryDatabaseLocation, properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnsExpectedValues()
        {
            // Setup
            var random = new Random();
            var hydraulicBoundaryDatabaseLocation = new HydraulicBoundaryLocation(0, "", 0, 0);

            // Call
            var properties = new HydraulicBoundaryLocationProperties(hydraulicBoundaryDatabaseLocation);

            // Assert
            Assert.AreEqual(hydraulicBoundaryDatabaseLocation.Id, properties.Id);
            Assert.AreEqual(hydraulicBoundaryDatabaseLocation.Name, properties.Name);
            Assert.AreEqual(hydraulicBoundaryDatabaseLocation.Location, properties.Location);
        }

        [Test]
        public void Constructor_WithData_PropertiesHaveExpectedAttributesValues()
        {
            // Call
            var properties = new HydraulicBoundaryLocationProperties(new HydraulicBoundaryLocation(0, "", 0, 0));

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(3, dynamicProperties.Count);

            const string expectedCategory = "Algemeen";

            PropertyDescriptor nameProperty = dynamicProperties[namePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            expectedCategory,
                                                                            "Naam",
                                                                            "Naam van de hydraulische belastingenlocatie.",
                                                                            true);

            PropertyDescriptor idProperty = dynamicProperties[idPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(idProperty,
                                                                            expectedCategory,
                                                                            "ID",
                                                                            "ID van de hydraulische belastingenlocatie in de database.",
                                                                            true);

            PropertyDescriptor locationProperty = dynamicProperties[locationPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(locationProperty,
                                                                            expectedCategory,
                                                                            "Coördinaten [m]",
                                                                            "Coördinaten van de hydraulische belastingenlocatie.",
                                                                            true);
        }
    }
}