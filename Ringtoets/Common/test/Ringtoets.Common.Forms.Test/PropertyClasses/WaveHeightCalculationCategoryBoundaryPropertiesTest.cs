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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class WaveHeightCalculationCategoryBoundaryPropertiesTest
    {
        private const int waveHeightPropertyIndex = 1;
        private const int convergencePropertyIndex = 6;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation());

            // Call
            var properties = new WaveHeightCalculationCategoryBoundaryProperties(hydraulicBoundaryLocationCalculation, "A");

            // Assert
            Assert.IsInstanceOf<HydraulicBoundaryLocationCalculationCategoryBoundaryProperties>(properties);
            Assert.AreSame(hydraulicBoundaryLocationCalculation, properties.Data);
        }

        [Test]
        public void Constructor_WithValidData_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation());

            // Call
            var properties = new WaveHeightCalculationCategoryBoundaryProperties(hydraulicBoundaryLocationCalculation, "A");

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(8, dynamicProperties.Count);

            const string resultCategory = "Resultaat";

            PropertyDescriptor waveHeightProperty = dynamicProperties[waveHeightPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(waveHeightProperty,
                                                                            resultCategory,
                                                                            "Golfhoogte [m]",
                                                                            "Berekende golfhoogte.",
                                                                            true);

            PropertyDescriptor convergenceProperty = dynamicProperties[convergencePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(convergenceProperty,
                                                                            resultCategory,
                                                                            "Convergentie",
                                                                            "Is convergentie bereikt in de golfhoogte berekening?",
                                                                            true);
        }

        [Test]
        public void GetProperties_Always_ReturnsExpectedValues()
        {
            // Setup
            var random = new Random();
            double waveHeight = random.NextDouble();
            var convergence = random.NextEnumValue<CalculationConvergence>();
            const string categoryBoundaryName = "A";
            var hydraulicBoundaryLocationCalculationOutput = new HydraulicBoundaryLocationCalculationOutput(waveHeight,
                                                                                                            random.NextDouble(),
                                                                                                            random.NextDouble(),
                                                                                                            random.NextDouble(),
                                                                                                            random.NextDouble(),
                                                                                                            convergence,
                                                                                                            new TestGeneralResultSubMechanismIllustrationPoint());

            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
            {
                Output = hydraulicBoundaryLocationCalculationOutput
            };

            // Call
            var properties = new WaveHeightCalculationCategoryBoundaryProperties(hydraulicBoundaryLocationCalculation, categoryBoundaryName);

            // Assert
            Assert.AreEqual(hydraulicBoundaryLocationCalculation.Output.Result, properties.Result);
            Assert.AreEqual(convergence, properties.Convergence);
            Assert.AreEqual(categoryBoundaryName, properties.CategoryBoundaryName);
        }
    }
}