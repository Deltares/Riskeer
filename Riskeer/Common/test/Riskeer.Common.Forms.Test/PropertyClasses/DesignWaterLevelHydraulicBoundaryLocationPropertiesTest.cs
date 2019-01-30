// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Linq;
using Core.Common.Gui.Converters;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PropertyClasses;

namespace Riskeer.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class DesignWaterLevelHydraulicBoundaryLocationPropertiesTest
    {
        private const int categoryBoundariesPropertyIndex = 3;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var location = new TestHydraulicBoundaryLocation();

            // Call
            var properties = new DesignWaterLevelHydraulicBoundaryLocationProperties(location,
                                                                                     Enumerable.Empty<Tuple<string, HydraulicBoundaryLocationCalculation>>());

            // Assert
            Assert.IsInstanceOf<HydraulicBoundaryLocationBaseProperties>(properties);
            Assert.AreSame(location, properties.Data);
            TestHelper.AssertTypeConverter<DesignWaterLevelHydraulicBoundaryLocationProperties, ExpandableArrayConverter>(
                nameof(DesignWaterLevelHydraulicBoundaryLocationProperties.CategoryBoundaries));
        }

        [Test]
        public void Constructor_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var location = new TestHydraulicBoundaryLocation();

            // Call
            var properties = new DesignWaterLevelHydraulicBoundaryLocationProperties(location,
                                                                                     Enumerable.Empty<Tuple<string, HydraulicBoundaryLocationCalculation>>());

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(4, dynamicProperties.Count);

            const string generalCategory = "Algemeen";

            PropertyDescriptor categoryBoundariesProperty = dynamicProperties[categoryBoundariesPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(categoryBoundariesProperty,
                                                                            generalCategory,
                                                                            "Categoriegrenzen",
                                                                            "De berekeningen per categoriegrens voor deze locatie.",
                                                                            true);
        }

        [Test]
        public void GetProperties_ValidData_ReturnsExpectedValues()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation)
            {
                Output = new TestHydraulicBoundaryLocationCalculationOutput()
            };

            const string categoryBoundaryName = "A";
            var calculations = new[]
            {
                new Tuple<string, HydraulicBoundaryLocationCalculation>(categoryBoundaryName, hydraulicBoundaryLocationCalculation)
            };

            // Call
            var properties = new DesignWaterLevelHydraulicBoundaryLocationProperties(hydraulicBoundaryLocation,
                                                                                     calculations);

            // Assert
            DesignWaterLevelCalculationCategoryBoundaryProperties categoryBoundaryCalculation = properties.CategoryBoundaries.Single();
            Assert.AreEqual(categoryBoundaryName, categoryBoundaryCalculation.CategoryBoundaryName);
            Assert.AreEqual(hydraulicBoundaryLocationCalculation.Output.Result, categoryBoundaryCalculation.Result);
            Assert.AreEqual(hydraulicBoundaryLocationCalculation.Output.CalculationConvergence, categoryBoundaryCalculation.Convergence);
        }
    }
}