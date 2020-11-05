﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;
using Riskeer.MacroStabilityInwards.CalculatedInput.TestUtil;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Forms.PropertyClasses;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.Waternet;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class MacroStabilityInwardsWaterStressLinesPropertiesTest
    {
        [Test]
        public void Constructor_DataNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsWaterStressLinesProperties(null,
                                                                                          AssessmentSectionTestHelper.GetTestAssessmentLevel());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("data", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidWaternet_ExpectedValues()
        {
            // Setup
            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties());

            // Call
            var properties = new MacroStabilityInwardsWaterStressLinesProperties(input,
                                                                                 AssessmentSectionTestHelper.GetTestAssessmentLevel());

            // Assert
            Assert.IsInstanceOf<ObjectProperties<MacroStabilityInwardsInput>>(properties);
            TestHelper.AssertTypeConverter<MacroStabilityInwardsWaterStressLinesProperties, ExpandableObjectConverter>(
                nameof(MacroStabilityInwardsWaterStressLinesProperties.WaternetDaily));
            TestHelper.AssertTypeConverter<MacroStabilityInwardsWaterStressLinesProperties, ExpandableObjectConverter>(
                nameof(MacroStabilityInwardsWaterStressLinesProperties.WaternetExtreme));
            Assert.AreSame(input, properties.Data);
        }

        [Test]
        public void WaternetExtreme_ValidWaternet_ExpectedValue()
        {
            // Setup
            RoundedDouble assessmentLevel = new Random(21).NextRoundedDouble();
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());

            var properties = new MacroStabilityInwardsWaterStressLinesProperties(calculation.InputParameters, assessmentLevel);

            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                // Call
                MacroStabilityInwardsWaternetProperties waternetProperties = properties.WaternetExtreme;

                // Assert
                var calculatorFactory = (TestMacroStabilityInwardsCalculatorFactory) MacroStabilityInwardsCalculatorFactory.Instance;

                WaternetCalculatorStub calculator = calculatorFactory.LastCreatedWaternetExtremeCalculator;
                Assert.AreEqual(assessmentLevel, calculator.Input.AssessmentLevel);
                CalculatorOutputAssert.AssertWaternet(calculator.Output, (MacroStabilityInwardsWaternet) waternetProperties.Data);
            }
        }

        [Test]
        public void WaternetDaily_ValidWaternet_ExpectedValue()
        {
            // Setup
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());

            var properties = new MacroStabilityInwardsWaterStressLinesProperties(calculation.InputParameters,
                                                                                 AssessmentSectionTestHelper.GetTestAssessmentLevel());

            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                // Call
                MacroStabilityInwardsWaternetProperties waternetProperties = properties.WaternetDaily;

                // Assert
                var calculatorFactory = (TestMacroStabilityInwardsCalculatorFactory) MacroStabilityInwardsCalculatorFactory.Instance;

                CalculatorOutputAssert.AssertWaternet(calculatorFactory.LastCreatedWaternetDailyCalculator.Output, (MacroStabilityInwardsWaternet) waternetProperties.Data);
            }
        }

        [Test]
        public void ToString_Always_ReturnEmpty()
        {
            // Setup
            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties());
            var properties = new MacroStabilityInwardsWaterStressLinesProperties(input,
                                                                                 AssessmentSectionTestHelper.GetTestAssessmentLevel());

            // Call
            string name = properties.ToString();

            // Assert
            Assert.IsEmpty(name);
        }

        [Test]
        public void Constructor_ValidData_PropertiesHaveExpectedAttributeValues()
        {
            // Setup
            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties());

            // Call
            var properties = new MacroStabilityInwardsWaterStressLinesProperties(input,
                                                                                 AssessmentSectionTestHelper.GetTestAssessmentLevel());

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            Assert.AreEqual(2, dynamicProperties.Count);

            const string waterStressesCategoryName = "Waterspanningen";

            PropertyDescriptor waternetExtremeProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(waternetExtremeProperty,
                                                                            waterStressesCategoryName,
                                                                            "Extreme omstandigheden",
                                                                            "Eigenschappen van de waterspanningslijnen bij extreme omstandigheden.",
                                                                            true);

            PropertyDescriptor waternetDailyProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(waternetDailyProperty,
                                                                            waterStressesCategoryName,
                                                                            "Dagelijkse omstandigheden",
                                                                            "Eigenschappen van de waterspanningslijnen bij dagelijkse omstandigheden.",
                                                                            true);
        }
    }
}