﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Service.Test
{
    [TestFixture]
    public class PipingSemiProbabilisticCalculationServiceTest
    {
        [Test]
        [TestCase(30000, 1.2, 1.0/7.36633055700265E-06)]
        [TestCase(30000, 1.0, 1.0/4.13743266617776E-05)]
        [TestCase(20000, 1.2, 1.0/9.53352884976163E-06)]
        [TestCase(20000, 1.0, 1.0/5.24016937211752E-05)]
        public void UpliftProbability_DifferentInputs_ReturnsExpectedValue(int norm, double factorOfSafety, double expectedResult)
        {
            // Setup
            var calculatorResult = new PipingOutput(double.NaN, factorOfSafety, double.NaN, double.NaN, double.NaN, double.NaN);
            var calculation = AsPipingCalculation(calculatorResult);

            PipingSemiProbabilisticCalculationService.Calculate(calculation, new PipingProbabilityAssessmentInput(), norm, double.NaN);

            // Call
            RoundedDouble result = calculation.SemiProbabilisticOutput.UpliftProbability;

            // Assert
            Assert.AreEqual(expectedResult, result, result.GetAccuracy());
        }

        [Test]
        [TestCase(30000, 0.6, 1/0.000233011)]
        [TestCase(30000, 0.4, 1/0.003967252)]
        [TestCase(20000, 0.6, 1/0.000292194)]
        [TestCase(20000, 0.4, 1/0.004742775)]
        public void HeaveProbability_DifferentInputs_ReturnsExpectedValue(int norm, double factorOfSafety, double expectedResult)
        {
            // Setup
            var calculatorResult = new PipingOutput(double.NaN, double.NaN, double.NaN, factorOfSafety, double.NaN, double.NaN);
            var calculation = AsPipingCalculation(calculatorResult);

            PipingSemiProbabilisticCalculationService.Calculate(calculation, new PipingProbabilityAssessmentInput(), norm, double.NaN);

            // Call
            RoundedDouble result = calculation.SemiProbabilisticOutput.HeaveProbability;

            // Assert
            Assert.AreEqual(expectedResult, result, result.GetAccuracy());
        }

        [Test]
        [TestCase(30000, 0.9, 1/1.0988217217028E-05)]
        [TestCase(30000, 0.6, 1/8.22098269097995E-04)]
        [TestCase(20000, 0.9, 1/1.80799783465546E-05)]
        [TestCase(20000, 0.6, 1/1.20312928722076E-03)]
        public void SellmeijerProbability_DifferentInputs_ReturnsExpectedValue(int norm, double factorOfSafety, double expectedResult)
        {
            // Setup
            var calculatorResult = new PipingOutput(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, factorOfSafety);
            var calculation = AsPipingCalculation(calculatorResult);

            PipingSemiProbabilisticCalculationService.Calculate(calculation, new PipingProbabilityAssessmentInput(), norm, double.NaN);

            // Call
            RoundedDouble result = calculation.SemiProbabilisticOutput.SellmeijerProbability;

            // Assert
            Assert.AreEqual(expectedResult, result, result.GetAccuracy());
        }

        [Test]
        [TestCase(30000, 1.2, 0.6, 0.9, 4.332647923)]
        [TestCase(30000, 1.2, 1.4, 0.9, 5.264767065)]
        [TestCase(30000, 1.2, 0.6, 1.1, 4.786155161)]
        [TestCase(20000, 1.2, 0.6, 0.9, 4.275544655)]
        [TestCase(20000, 1.2, 1.4, 0.9, 5.203962658)]
        [TestCase(20000, 1.2, 0.6, 1.1, 4.673091832)]
        public void PipingReliability_DifferentInputs_ReturnsExpectedValue(int norm, double fosUplift, double fosHeave, double fosSellmeijer, double expectedResult)
        {
            // Setup
            var calculatorResult = new PipingOutput(double.NaN, fosUplift, double.NaN, fosHeave, double.NaN, fosSellmeijer);
            var calculation = AsPipingCalculation(calculatorResult);

            PipingSemiProbabilisticCalculationService.Calculate(calculation, new PipingProbabilityAssessmentInput(), norm, double.NaN);

            // Call
            RoundedDouble result = calculation.SemiProbabilisticOutput.PipingReliability;

            // Assert
            Assert.AreEqual(expectedResult, result, result.GetAccuracy());
        }

        [Test]
        [TestCase(30000, 6000, 24, 4.916313847)]
        [TestCase(20000, 6000, 12, 4.972362935)]
        [TestCase(20000, 8000, 24, 4.890463519)]
        public void RequiredReliability_DifferentInputs_ReturnsExpectedValue(int norm, double assessmentSectionLength, double contribution, double expectedResult)
        {
            // Setup
            var calculatorResult = new PipingOutput(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN);
            var pipingProbabilityAssessmentInput = new PipingProbabilityAssessmentInput
            {
                SectionLength = assessmentSectionLength
            };
            var calculation = AsPipingCalculation(calculatorResult);

            PipingSemiProbabilisticCalculationService.Calculate(calculation, pipingProbabilityAssessmentInput, norm, contribution);

            // Call
            RoundedDouble result = calculation.SemiProbabilisticOutput.RequiredReliability;

            // Assert
            Assert.AreEqual(expectedResult, result, result.GetAccuracy());
        }

        [Test]
        public void PipingFactorOfSafety_SampleInput_ReturnsExpectedValue()
        {
            // Setup
            int norm = 30000;
            double assessmentSectionLength = 6000;
            double contribution = 24;
            double fosUplift = 1.2;
            double fosHeave = 0.6;
            double fosSellmeijer = 0.9;
            double expectedResult = 0.881;

            var calculatorResult = new PipingOutput(double.NaN, fosUplift, double.NaN, fosHeave, double.NaN, fosSellmeijer);
            var pipingProbabilityAssessmentInput = new PipingProbabilityAssessmentInput
            {
                SectionLength = assessmentSectionLength
            };
            var calculation = AsPipingCalculation(calculatorResult);

            PipingSemiProbabilisticCalculationService.Calculate(calculation, pipingProbabilityAssessmentInput, norm, contribution);

            // Call
            RoundedDouble result = calculation.SemiProbabilisticOutput.PipingFactorOfSafety;

            // Assert
            Assert.AreEqual(expectedResult, result, result.GetAccuracy());
        }

        [Test]
        [Combinatorial]
        public void PipingFactorOfSafety_DifferentInputs_ReturnsExpectedValue(
            [Values(20000, 30000)] int norm,
            [Values(6000, 8000)] double assessmentSectionLength,
            [Values(12, 24)] double contribution,
            [Values(1.2, 1.0)] double fosUplift,
            [Values(1.4, 0.6)] double fosHeave,
            [Values(0.9, 1.1)] double fosSellmeijer)
        {
            // Setup
            var calculatorResult = new PipingOutput(double.NaN, fosUplift, double.NaN, fosHeave, double.NaN, fosSellmeijer);
            var pipingProbabilityAssessmentInput = new PipingProbabilityAssessmentInput
            {
                SectionLength = assessmentSectionLength
            };
            var calculation = AsPipingCalculation(calculatorResult);

            PipingSemiProbabilisticCalculationService.Calculate(calculation, pipingProbabilityAssessmentInput, norm, contribution);

            // Call
            RoundedDouble result = calculation.SemiProbabilisticOutput.PipingFactorOfSafety;

            // Assert
            Assert.AreEqual(calculation.SemiProbabilisticOutput.PipingReliability/calculation.SemiProbabilisticOutput.RequiredReliability, result, result.GetAccuracy());
        }

        [Test]
        public void Calculate_CompleteInput_ReturnsPipingSemiProbabilisticOutputWithValues()
        {
            // Setup
            var pipingProbabilityAssessmentInput = new PipingProbabilityAssessmentInput
            {
                SectionLength = 6000
            };
            var pipingOutput = new PipingOutput(double.NaN, 1.2, double.NaN, 0.6, double.NaN, 0.9);
            var pipingCalculation = new PipingCalculation(new GeneralPipingInput())
            {
                Output = pipingOutput
            };

            // Call
            PipingSemiProbabilisticCalculationService.Calculate(pipingCalculation, pipingProbabilityAssessmentInput, 30000, 24);

            // Assert
            RoundedDouble result = pipingCalculation.SemiProbabilisticOutput.PipingFactorOfSafety;
            Assert.AreEqual(0.881, result, result.GetAccuracy());
        }

        [Test]
        public void Calculate_MissingOutput_ThrowsArgumentException()
        {
            // Setup
            var generalInput = new GeneralPipingInput();
            var pipingCalculation = new PipingCalculation(generalInput);

            // Call
            TestDelegate test = () => PipingSemiProbabilisticCalculationService.Calculate(pipingCalculation, new PipingProbabilityAssessmentInput(), int.MinValue, double.NaN);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, "Cannot perform a semi-probabilistic calculation without output from the piping kernel.");
        }

        private PipingCalculation AsPipingCalculation(PipingOutput pipingOutput)
        {
            return new PipingCalculation(new GeneralPipingInput())
            {
                Output = pipingOutput
            };
        }
    }
}