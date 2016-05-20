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
using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Ringtoets.GrassCoverErosionInwards.Service.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsOutputCalculationServiceTest
    {
        [Test]
        public void Calculate_NullCalculation_ThrowsArgumentNullException()
        {
            // Setup & Call
            TestDelegate test = () => GrassCoverErosionInwardsOutputCalculationService.Calculate(null, double.NaN, double.NaN, double.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        [TestCase(30000, 100, 60000)]
        [TestCase(30000, 24, 250000)]
        [TestCase(20000, 100, 40000)]
        [TestCase(20000, 24, 166666.6667)]
        public void RequiredProbability_DifferentInputs_ReturnsExpectedValue(int norm, double contribution, double expectedResult)
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation(new GeneralGrassCoverErosionInwardsInput(),
                                                                      new GeneralNormProbabilityInput());

            // Call
            GrassCoverErosionInwardsOutputCalculationService.Calculate(calculation, norm, contribution, double.NaN);

            // Assert
            RoundedDouble output = calculation.Output.RequiredProbability;
            Assert.AreEqual(expectedResult, output, output.GetAccuracy());
        }

        [Test]
        [TestCase(30000, 100, 4.149409984)]
        [TestCase(30000, 24, 4.465183916)]
        [TestCase(20000, 100, 4.055626981)]
        [TestCase(20000, 24, 4.377587847)]
        public void RequiredReliability_DifferentInputs_ReturnsExpectedValue(int norm, double contribution, double expectedResult)
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation(new GeneralGrassCoverErosionInwardsInput(),
                                                                      new GeneralNormProbabilityInput());

            // Call
            GrassCoverErosionInwardsOutputCalculationService.Calculate(calculation, norm, contribution, double.NaN);

            // Assert
            RoundedDouble output = calculation.Output.RequiredReliability;
            Assert.AreEqual(expectedResult, output, output.GetAccuracy());
        }

        [Test]
        [TestCase(1.23456, 1.23456)]
        [TestCase(789.123, 789.123)]
        public void Reliability_DifferentInputs_ReturnsExpectedValue(double probability, double expectedResult)
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation(new GeneralGrassCoverErosionInwardsInput(),
                                                                      new GeneralNormProbabilityInput());

            // Call
            GrassCoverErosionInwardsOutputCalculationService.Calculate(calculation, double.NaN, double.NaN, probability);

            // Assert
            RoundedDouble output = calculation.Output.Reliability;
            Assert.AreEqual(expectedResult, output, output.GetAccuracy());
        }

        [Test]
        [TestCase(4, 7472.1535758)]
        [TestCase(5, 672621.8295310)]
        public void Probability_DifferentInputs_ReturnsExpectedValue(double probability, double expectedResult)
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation(new GeneralGrassCoverErosionInwardsInput(),
                                                                      new GeneralNormProbabilityInput());

            // Call
            GrassCoverErosionInwardsOutputCalculationService.Calculate(calculation, double.NaN, double.NaN, probability);

            // Assert
            RoundedDouble output = calculation.Output.Probability;
            Assert.AreEqual(expectedResult, output, output.GetAccuracy());
        }

        [Test]
        [TestCase(30000, 100, 4.107479655, 0.989894869)]
        [TestCase(30000, 100, 4.149409984, 1)]
        [TestCase(30000, 24, 4.107479655, 0.919890363)]
        [TestCase(30000, 24, 4.149409984, 0.929280868)]
        [TestCase(20000, 100, 4.107479655, 1.012785366)]
        [TestCase(20000, 100, 4.149409984, 1.023124169)]
        [TestCase(20000, 24, 4.107479655, 0.938297482)]
        [TestCase(20000, 24, 4.149409984, 0.947875892)]
        public void FactorOfSafety_DifferentInputs_ReturnsExpectedValue(int norm, double contribution, double probability, double expectedResult)
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation(new GeneralGrassCoverErosionInwardsInput(),
                                                                      new GeneralNormProbabilityInput());

            // Call
            GrassCoverErosionInwardsOutputCalculationService.Calculate(calculation, norm, contribution, probability);

            // Assert
            RoundedDouble output = calculation.Output.FactorOfSafety;
            Assert.AreEqual(expectedResult, output, output.GetAccuracy());
        }
    }
}