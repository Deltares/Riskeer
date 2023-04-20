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
using Core.Common.TestUtil;
using Core.Gui.PropertyBag;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Integration.Forms.PropertyClasses;

namespace Riskeer.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class HydraulicBoundaryDatabasePropertiesTest
    {
        private const int usePreprocessorClosurePropertyIndex = 0;
        private const int locationsPropertyIndex = 1;
        private const int versionPropertyIndex = 2;

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
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            // Call
            var properties = new HydraulicBoundaryDatabaseProperties(hydraulicBoundaryDatabase);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<HydraulicBoundaryDatabase>>(properties);
            Assert.AreSame(hydraulicBoundaryDatabase, properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnsExpectedValues()
        {
            // Setup
            var random = new Random();
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                UsePreprocessorClosure = random.NextBoolean()
            };

            // Call
            var properties = new HydraulicBoundaryDatabaseProperties(hydraulicBoundaryDatabase);

            // Assert
            Assert.AreEqual(hydraulicBoundaryDatabase.UsePreprocessorClosure, properties.UsePreprocessorClosure);
        }

        [Test]
        public void Constructor_WithData_PropertiesHaveExpectedAttributesValues()
        {
            // Call
            var properties = new HydraulicBoundaryDatabaseProperties(new HydraulicBoundaryDatabase());

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(3, dynamicProperties.Count);

            const string expectedCategory = "Algemeen";

            PropertyDescriptor usePreprocessorClosureProperty = dynamicProperties[usePreprocessorClosurePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(usePreprocessorClosureProperty,
                                                                            expectedCategory,
                                                                            "Gebruik preprocessor sluitregime database",
                                                                            "Gebruik de preprocessor sluitregime database bij het uitvoeren van een berekening.",
                                                                            true);

            PropertyDescriptor locationsProperty = dynamicProperties[locationsPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(locationsProperty,
                                                                            expectedCategory,
                                                                            "Locaties",
                                                                            "Locaties uit de hydraulische belastingendatabase.",
                                                                            true);

            PropertyDescriptor versionProperty = dynamicProperties[versionPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(versionProperty,
                                                                            expectedCategory,
                                                                            "Versie",
                                                                            "Versie van HRD bestand",
                                                                            true);
        }
    }
}