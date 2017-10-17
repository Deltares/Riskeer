﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.CalculatedInput.TestUtil;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.CalculatedInput.Test
{
    [TestFixture]
    public class WaternetCalculationServiceTest
    {
        [Test]
        public void CalculateExtreme_InputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => WaternetCalculationService.CalculateExtreme(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("input", exception.ParamName);
        }

        [Test]
        public void CalculateExtreme_WithInput_SetsInputOnCalculator()
        {
            // Setup
            MacroStabilityInwardsCalculationScenario testCalculation = MacroStabilityInwardsCalculationScenarioFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput();
            MacroStabilityInwardsInput input = testCalculation.InputParameters;

            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                // Call
                WaternetCalculationService.CalculateExtreme(input);

                // Assert
                AssertInput(input, (TestMacroStabilityInwardsCalculatorFactory) MacroStabilityInwardsCalculatorFactory.Instance);
            }
        }

        [Test]
        public void CalculateExtreme_CalculationRan_ReturnMacroStabilityInwardsWaternet()
        {
            // Setup
            MacroStabilityInwardsCalculationScenario testCalculation = MacroStabilityInwardsCalculationScenarioFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput();

            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestMacroStabilityInwardsCalculatorFactory)MacroStabilityInwardsCalculatorFactory.Instance;

                // Call
                MacroStabilityInwardsWaternet output = WaternetCalculationService.CalculateExtreme(testCalculation.InputParameters);

                // Assert
                CalculatorOutputAssert.AssertWaternet(calculatorFactory.LastCreatedWaternetCalculator.Output, output);
            }
        }

        [Test]
        public void CalculateExtreme_ErrorInCalculation_ReturnNull()
        {
            // Setup
            MacroStabilityInwardsCalculationScenario testCalculation = MacroStabilityInwardsCalculationScenarioFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput();

            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestMacroStabilityInwardsCalculatorFactory)MacroStabilityInwardsCalculatorFactory.Instance;
                calculatorFactory.LastCreatedWaternetCalculator.ThrowExceptionOnCalculate = true;

                // Call
                MacroStabilityInwardsWaternet output = WaternetCalculationService.CalculateExtreme(testCalculation.InputParameters);

                // Assert
                Assert.IsNull(output);
            }
        }

        private static void AssertInput(IMacroStabilityInwardsWaternetInput originalInput, TestMacroStabilityInwardsCalculatorFactory factory)
        {
            WaternetCalculatorInput actualInput = factory.LastCreatedWaternetCalculator.Input;
            CalculatorInputAssert.AssertSoilProfile(originalInput.SoilProfileUnderSurfaceLine, actualInput.SoilProfile);
            CalculatorInputAssert.AssertDrainageConstruction(originalInput, actualInput.DrainageConstruction);
            CalculatorInputAssert.AssertPhreaticLineOffsets(originalInput.LocationInputExtreme, actualInput.PhreaticLineOffsets);
            Assert.AreEqual(WaternetCreationMode.CreateWaternet, actualInput.WaternetCreationMode);
            Assert.AreEqual(PlLineCreationMethod.RingtoetsWti2017, actualInput.PlLineCreationMethod);
            Assert.AreEqual(LandwardDirection.PositiveX, actualInput.LandwardDirection);
            Assert.AreSame(originalInput.SurfaceLine, actualInput.SurfaceLine);
            Assert.AreEqual(originalInput.AssessmentLevel, actualInput.AssessmentLevel);
            Assert.AreEqual(originalInput.DikeSoilScenario, actualInput.DikeSoilScenario);
            Assert.AreEqual(originalInput.WaterLevelRiverAverage, actualInput.WaterLevelRiverAverage);
            Assert.AreEqual(originalInput.LocationInputExtreme.WaterLevelPolder, actualInput.WaterLevelPolder);
            Assert.AreEqual(originalInput.DrainageConstructionPresent, actualInput.DrainageConstruction.IsPresent);
            Assert.AreEqual(originalInput.XCoordinateDrainageConstruction, actualInput.DrainageConstruction.XCoordinate);
            Assert.AreEqual(originalInput.ZCoordinateDrainageConstruction, actualInput.DrainageConstruction.ZCoordinate);
            Assert.AreEqual(originalInput.MinimumLevelPhreaticLineAtDikeTopRiver, actualInput.MinimumLevelPhreaticLineAtDikeTopRiver);
            Assert.AreEqual(originalInput.MinimumLevelPhreaticLineAtDikeTopPolder, actualInput.MinimumLevelPhreaticLineAtDikeTopPolder);
            Assert.AreEqual(originalInput.LeakageLengthOutwardsPhreaticLine3, actualInput.LeakageLengthOutwardsPhreaticLine3);
            Assert.AreEqual(originalInput.LeakageLengthInwardsPhreaticLine3, actualInput.LeakageLengthInwardsPhreaticLine3);
            Assert.AreEqual(originalInput.LeakageLengthOutwardsPhreaticLine4, actualInput.LeakageLengthOutwardsPhreaticLine4);
            Assert.AreEqual(originalInput.LeakageLengthInwardsPhreaticLine4, actualInput.LeakageLengthInwardsPhreaticLine4);
            Assert.AreEqual(originalInput.PiezometricHeadPhreaticLine2Outwards, actualInput.PiezometricHeadPhreaticLine2Outwards);
            Assert.AreEqual(originalInput.PiezometricHeadPhreaticLine2Inwards, actualInput.PiezometricHeadPhreaticLine2Inwards);
            Assert.AreEqual(originalInput.LocationInputExtreme.PenetrationLength, actualInput.PenetrationLength);
            Assert.AreEqual(originalInput.AdjustPhreaticLine3And4ForUplift, actualInput.AdjustPhreaticLine3And4ForUplift);
        }
    }
}