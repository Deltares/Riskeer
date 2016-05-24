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
using Ringtoets.Common.Data.Probability;
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
            //Call
            TestDelegate test = () => GrassCoverErosionInwardsOutputCalculationService.Calculate(null, double.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        [TestCase(30000, 100, 2, 60000)]
        [TestCase(30000, 100, 1, 30000)]
        [TestCase(30000, 24, 2, 250000)]
        [TestCase(30000, 24, 1, 125000)]
        [TestCase(20000, 100, 2, 40000)]
        [TestCase(20000, 100, 1, 20000)]
        [TestCase(20000, 24, 2, 166666.6667)]
        [TestCase(20000, 24, 1, 83333.33)]
        public void RequiredProbability_DifferentInputs_ReturnsExpectedValue(int norm, double contribution, int lengthEffectN, double expectedResult)
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation(new GeneralGrassCoverErosionInwardsInput(),
                                                                      new NormProbabilityInput())
            {
                NormProbabilityInput =
                {
                    Norm = norm,
                    Contribution = contribution,
                    N = lengthEffectN
                }
            };

            // Call
            GrassCoverErosionInwardsOutputCalculationService.Calculate(calculation, double.NaN);

            // Assert
            RoundedDouble output = calculation.Output.RequiredProbability;
            Assert.AreEqual(expectedResult, output, output.GetAccuracy());
        }

        [Test]
        [TestCase(30000, 100, 2, 4.149409984)]
        [TestCase(30000, 100, 1, 3.987878937)]
        [TestCase(30000, 24, 2, 4.465183916)]
        [TestCase(30000, 24, 1, 4.314451022)]
        [TestCase(20000, 100, 2, 4.055626981)]
        [TestCase(20000, 100, 1, 3.890591886)]
        [TestCase(20000, 24, 2, 4.377587847)]
        [TestCase(20000, 24, 1, 4.2240038)]
        public void RequiredReliability_DifferentInputs_ReturnsExpectedValue(int norm, double contribution, int lengthEffectN, double expectedResult)
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation(new GeneralGrassCoverErosionInwardsInput(),
                                                                      new NormProbabilityInput())
            {
                NormProbabilityInput =
                {
                    Norm = norm,
                    Contribution = contribution,
                    N = lengthEffectN
                }
            };

            // Call
            GrassCoverErosionInwardsOutputCalculationService.Calculate(calculation, double.NaN);

            // Assert
            RoundedDouble output = calculation.Output.RequiredReliability;
            Assert.AreEqual(expectedResult, output, output.GetAccuracy());
        }

        [Test]
        [TestCase(1.23456, 1.23456)]
        [TestCase(789.123, 789.123)]
        public void Reliability_DifferentInputs_ReturnsExpectedValue(double reliability, double expectedResult)
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation(new GeneralGrassCoverErosionInwardsInput(),
                                                                      new NormProbabilityInput());

            // Call
            GrassCoverErosionInwardsOutputCalculationService.Calculate(calculation, reliability);

            // Assert
            RoundedDouble output = calculation.Output.Reliability;
            Assert.AreEqual(expectedResult, output, output.GetAccuracy());
        }

        [Test]
        [TestCase(4, 31574.3855346)]
        [TestCase(5, 3488555.78723)]
        public void Probability_DifferentInputs_ReturnsExpectedValue(double reliability, double expectedResult)
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation(new GeneralGrassCoverErosionInwardsInput(),
                                                                      new NormProbabilityInput());

            // Call
            GrassCoverErosionInwardsOutputCalculationService.Calculate(calculation, reliability);

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
        public void FactorOfSafety_DifferentInputs_ReturnsExpectedValue(int norm, double contribution, double reliability, double expectedResult)
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation(new GeneralGrassCoverErosionInwardsInput(),
                                                                      new NormProbabilityInput())
            {
                NormProbabilityInput =
                {
                    Norm = norm,
                    Contribution = contribution
                }
            };

            // Call
            GrassCoverErosionInwardsOutputCalculationService.Calculate(calculation, reliability);

            // Assert
            RoundedDouble output = calculation.Output.FactorOfSafety;
            Assert.AreEqual(expectedResult, output, output.GetAccuracy());
        }
    }
}