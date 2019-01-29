// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.Common.Data.Probabilistics;

namespace Riskeer.Common.Data.Test.Probabilistics
{
    [TestFixture]
    public class NormalDistributionDesignVariableCalculatorTest
    {
        [Test]
        public void CreateWithStandardDeviation_WithParameters_CreateNewInstance()
        {
            // Setup
            var random = new Random(21);
            double mean = random.NextDouble();
            double standardDeviation = random.NextDouble();

            // Call
            PercentileBasedDesignVariableCalculator calculator =
                NormalDistributionDesignVariableCalculator.CreateWithStandardDeviation(mean, standardDeviation);

            // Assert
            Assert.IsInstanceOf<NormalDistributionDesignVariableCalculator>(calculator);
        }

        [Test]
        public void CreateWithCoefficientOfVariation_WithParameters_CreateNewInstance()
        {
            // Setup
            var random = new Random(21);
            double mean = random.NextDouble();
            double coefficientOfVariation = random.NextDouble();

            // Call
            PercentileBasedDesignVariableCalculator calculator =
                NormalDistributionDesignVariableCalculator.CreateWithCoefficientOfVariation(mean, coefficientOfVariation);

            // Assert
            Assert.IsInstanceOf<NormalDistributionDesignVariableCalculator>(calculator);
        }

        [Test]
        [TestCase(8.4, 3.2, 0.95, 13.663531606244712)]
        [TestCase(15.4, 3.2, 0.95, 20.66353160624471)]
        [TestCase(8.4, 6.2, 0.95, 18.59809248709913)]
        [TestCase(8.4, 6.2, 0.45, 7.6208996494985417)]
        public void DetermineDesignValue_DifferentValues_BothTypesOfCalculatorsReturnsExpectedValue(double mean, double standardDeviation, double percentile, double expectedValue)
        {
            // Setup
            PercentileBasedDesignVariableCalculator calculatorStd = NormalDistributionDesignVariableCalculator.CreateWithStandardDeviation(mean, standardDeviation);
            PercentileBasedDesignVariableCalculator calculatorCov = NormalDistributionDesignVariableCalculator.CreateWithCoefficientOfVariation(mean, standardDeviation / mean);

            // Call
            double stdDesignValue = calculatorStd.GetDesignValue(percentile);
            double covDesignValue = calculatorCov.GetDesignValue(percentile);

            // Assert
            Assert.AreEqual(expectedValue, stdDesignValue, 1e-8);
            Assert.AreEqual(expectedValue, covDesignValue, 1e-8);
        }
    }
}