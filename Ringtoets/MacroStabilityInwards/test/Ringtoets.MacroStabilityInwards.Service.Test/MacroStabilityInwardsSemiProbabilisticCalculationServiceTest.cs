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
using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data;

namespace Ringtoets.MacroStabilityInwards.Service.Test
{
    [TestFixture]
    public class MacroStabilityInwardsSemiProbabilisticCalculationServiceTest
    {
        [Test]
        [TestCase(30000, 4.796710, 6000, 12)]
        [TestCase(30000, 4.655890, 6000, 24)]
        [TestCase(30000, 4.843790, 8000, 12)]
        [TestCase(20000, 4.620849, 8000, 24)]
        [TestCase(20000, 4.651410, 4000, 12)]
        [TestCase(20000, 4.506409, 4000, 24)]
        public void MacroStabilityInwardsReliability_DifferentInputs_ReturnsExpectedValue(int returnPeriod, double expectedResult, double assessmentSectionLength, double contribution)
        {
            // Setup
            var calculatorResult = new MacroStabilityInwardsOutput();
            var probabilityAssessmentInput = new MacroStabilityInwardsProbabilityAssessmentInput
            {
                SectionLength = assessmentSectionLength
            };

            MacroStabilityInwardsCalculation calculation = AsMacroStabilityInwardsCalculation(calculatorResult);
            double norm = 1.0 / returnPeriod;

            MacroStabilityInwardsSemiProbabilisticCalculationService.Calculate(calculation, probabilityAssessmentInput, norm, contribution);

            // Call
            RoundedDouble result = calculation.SemiProbabilisticOutput.MacroStabilityInwardsReliability;

            // Assert
            Assert.AreEqual(expectedResult, result, result.GetAccuracy());
        }

        [Test]
        [TestCase(30000, 6000, 24, 4.655890000)]
        [TestCase(20000, 6000, 12, 4.714809999)]
        [TestCase(20000, 8000, 24, 4.620849999)]
        public void RequiredReliability_DifferentInputs_ReturnsExpectedValue(int returnPeriod, double assessmentSectionLength, double contribution, double expectedResult)
        {
            // Setup
            var calculatorResult = new MacroStabilityInwardsOutput();
            var probabilityAssessmentInput = new MacroStabilityInwardsProbabilityAssessmentInput
            {
                SectionLength = assessmentSectionLength
            };
            MacroStabilityInwardsCalculation calculation = AsMacroStabilityInwardsCalculation(calculatorResult);
            double norm = 1.0 / returnPeriod;

            MacroStabilityInwardsSemiProbabilisticCalculationService.Calculate(calculation, probabilityAssessmentInput, norm, contribution);

            // Call
            RoundedDouble result = calculation.SemiProbabilisticOutput.RequiredReliability;

            // Assert
            Assert.AreEqual(expectedResult, result, result.GetAccuracy());
        }

        [Test]
        public void MacroStabilityInwardsFactorOfSafety_SampleInput_ReturnsExpectedValue()
        {
            // Setup
            const int returnPeriod = 30000;
            const double assessmentSectionLength = 6000;
            const double contribution = 24;
            const double expectedResult = 1;

            var calculatorResult = new MacroStabilityInwardsOutput();
            var probabilityAssessmentInput = new MacroStabilityInwardsProbabilityAssessmentInput
            {
                SectionLength = assessmentSectionLength
            };
            MacroStabilityInwardsCalculation calculation = AsMacroStabilityInwardsCalculation(calculatorResult);
            const double norm = 1.0 / returnPeriod;

            MacroStabilityInwardsSemiProbabilisticCalculationService.Calculate(calculation, probabilityAssessmentInput, norm, contribution);

            // Call
            RoundedDouble result = calculation.SemiProbabilisticOutput.MacroStabilityInwardsFactorOfSafety;

            // Assert
            Assert.AreEqual(expectedResult, result, result.GetAccuracy());
        }

        [Test]
        [Combinatorial]
        public void MacroStabilityInwardsFactorOfSafety_DifferentInputs_ReturnsExpectedValue(
            [Values(20000, 30000)] int returnPeriod,
            [Values(6000, 8000)] double assessmentSectionLength,
            [Values(12, 24)] double contribution)
        {
            // Setup
            var calculatorResult = new MacroStabilityInwardsOutput();
            var probabilityAssessmentInput = new MacroStabilityInwardsProbabilityAssessmentInput
            {
                SectionLength = assessmentSectionLength
            };
            MacroStabilityInwardsCalculation calculation = AsMacroStabilityInwardsCalculation(calculatorResult);
            double norm = 1.0 / returnPeriod;

            MacroStabilityInwardsSemiProbabilisticCalculationService.Calculate(calculation, probabilityAssessmentInput, norm, contribution);

            // Call
            RoundedDouble result = calculation.SemiProbabilisticOutput.MacroStabilityInwardsFactorOfSafety;

            // Assert
            double accuracy = Math.Pow(10.0, -result.NumberOfDecimalPlaces); // Less strict accuracy because of calculation using rounded doubles
            Assert.AreEqual(calculation.SemiProbabilisticOutput.MacroStabilityInwardsReliability / calculation.SemiProbabilisticOutput.RequiredReliability, result, accuracy);
        }

        private static MacroStabilityInwardsCalculation AsMacroStabilityInwardsCalculation(MacroStabilityInwardsOutput macroStabilityInwardsOutput)
        {
            return new MacroStabilityInwardsCalculation
            {
                Output = macroStabilityInwardsOutput
            };
        }
    }
}