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
        public void Calculate_CalculationNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            TestDelegate call = () => MacroStabilityInwardsSemiProbabilisticCalculationService.Calculate(null,
                                                                                                         new MacroStabilityInwardsProbabilityAssessmentInput(),
                                                                                                         random.NextDouble(),
                                                                                                         random.NextDouble());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void Calculate_ProbabilityAssessmentInputNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            TestDelegate call = () => MacroStabilityInwardsSemiProbabilisticCalculationService.Calculate(new MacroStabilityInwardsCalculation(),
                                                                                                         null,
                                                                                                         random.NextDouble(),
                                                                                                         random.NextDouble());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("probabilityAssessmentInput", exception.ParamName);
        }

        [Test]
        [TestCase(30000, 6000, 24, 4.655890000)]
        [TestCase(20000, 6000, 12, 4.714809999)]
        [TestCase(20000, 8000, 24, 4.620849999)]
        public void RequiredReliability_DifferentInputs_ReturnsExpectedValue(int returnPeriod,
                                                                             double assessmentSectionLength,
                                                                             double contribution,
                                                                             double expectedResult)
        {
            // Setup
            var calculatorResult = new MacroStabilityInwardsOutput(new MacroStabilityInwardsOutput.ConstructionProperties());
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
        [TestCase(30000, 6000, 24)]
        [TestCase(20000, 6000, 12)]
        [TestCase(20000, 8000, 24)]
        public void RequiredProbability_DifferentInputs_ReturnExpectedValues(int returnPeriod,
                                                                             double assessmentSectionLength,
                                                                             double contribution)
        {
            // Setup
            var calculatorResult = new MacroStabilityInwardsOutput(new MacroStabilityInwardsOutput.ConstructionProperties());
            var probabilityAssessmentInput = new MacroStabilityInwardsProbabilityAssessmentInput
            {
                SectionLength = assessmentSectionLength
            };
            MacroStabilityInwardsCalculation calculation = AsMacroStabilityInwardsCalculation(calculatorResult);
            double norm = 1.0 / returnPeriod;

            MacroStabilityInwardsSemiProbabilisticCalculationService.Calculate(calculation, probabilityAssessmentInput, norm, contribution);

            // Call
            double result = calculation.SemiProbabilisticOutput.RequiredProbability;

            // Assert
            double expectedProbability = GetRequiredProbability(norm,
                                                                contribution,
                                                                probabilityAssessmentInput.A,
                                                                probabilityAssessmentInput.B,
                                                                probabilityAssessmentInput.SectionLength);

            Assert.AreEqual(expectedProbability, result);
        }

        [Test]
        [Combinatorial]
        public void MacroStabilityInwardsFactorOfSafety_DifferentInputs_ReturnsExpectedValue(
            [Values(20000, 30000)] int returnPeriod,
            [Values(6000, 8000)] double assessmentSectionLength,
            [Values(12, 24)] double contribution)
        {
            // Setup
            var calculatorResult = new MacroStabilityInwardsOutput(new MacroStabilityInwardsOutput.ConstructionProperties());
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
            MacroStabilityInwardsSemiProbabilisticOutput semiProbabilisticOutput = calculation.SemiProbabilisticOutput;
            double expectedFactorOfSafety = semiProbabilisticOutput.MacroStabilityInwardsReliability / semiProbabilisticOutput.RequiredReliability;
            Assert.AreEqual(expectedFactorOfSafety, result, result.GetAccuracy());
        }

        [Test]
        [TestCase(0.75)]
        [TestCase(0.55)]
        [TestCase(0.30)]
        public void MacroStabilityInwardsReliability_DifferentInputs_ReturnsExpectedValues(double factorOfStability)
        {
            // Setup
            var random = new Random(21);
            var calculatorResult = new MacroStabilityInwardsOutput(new MacroStabilityInwardsOutput.ConstructionProperties
            {
                FactorOfStability = factorOfStability
            });
            var probabilityAssessmentInput = new MacroStabilityInwardsProbabilityAssessmentInput();

            MacroStabilityInwardsCalculation calculation = AsMacroStabilityInwardsCalculation(calculatorResult);

            MacroStabilityInwardsSemiProbabilisticCalculationService.Calculate(calculation,
                                                                               probabilityAssessmentInput,
                                                                               random.NextDouble(),
                                                                               random.NextDouble());

            // Call
            RoundedDouble result = calculation.SemiProbabilisticOutput.MacroStabilityInwardsReliability;

            // Assert
            Assert.AreEqual(GetDerivedEstimatedReliability(factorOfStability), result, result.GetAccuracy());
        }

        [Test]
        [TestCase(0.75, 0.037743005)]
        [TestCase(0.55, 0.2961520790)]
        [TestCase(0.30, 0.845423280)]
        public void MacroStabilityInwardsProbability_DifferentInputs_ReturnsExpectedValues(double factorOfStability,
                                                                                           double expectedResult)
        {
            // Setup
            var random = new Random(21);
            var calculatorResult = new MacroStabilityInwardsOutput(new MacroStabilityInwardsOutput.ConstructionProperties
            {
                FactorOfStability = factorOfStability
            });
            var probabilityAssessmentInput = new MacroStabilityInwardsProbabilityAssessmentInput();

            MacroStabilityInwardsCalculation calculation = AsMacroStabilityInwardsCalculation(calculatorResult);

            MacroStabilityInwardsSemiProbabilisticCalculationService.Calculate(calculation,
                                                                               probabilityAssessmentInput,
                                                                               random.NextDouble(),
                                                                               random.NextDouble());
            // Call
            double result = calculation.SemiProbabilisticOutput.MacroStabilityInwardsProbability;

            // Assert
            int nrOfDecimals = calculation.SemiProbabilisticOutput.MacroStabilityInwardsReliability.NumberOfDecimalPlaces;
            double accuracy = Math.Pow(10, -nrOfDecimals);
            Assert.AreEqual(expectedResult, result, accuracy); // Probability is a derived output from reliability, hence accuracy is determined by the accuracy of the reliability
        }

        private static double GetDerivedEstimatedReliability(double stabilityFactor)
        {
            return (6.21 * stabilityFactor) - 2.88;
        }

        private static double GetRequiredProbability(double norm,
                                                     double contribution,
                                                     double constantA,
                                                     double constantB,
                                                     double assessmentSectionLength)
        {
            double contributionPercentage = contribution / 100;
            return (norm * contributionPercentage) / (1 + (constantA * assessmentSectionLength) / constantB);
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