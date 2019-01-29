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
    public class LogNormalDistributionDesignVariableCalculatorTest
    {
        [Test]
        public void CreateWithStandardDeviation_WithParameters_CreateNewInstance()
        {
            // Setup
            var random = new Random(21);
            double mean = random.NextDouble();
            double standardDeviation = random.NextDouble();
            double shift = random.NextDouble();

            // Call
            PercentileBasedDesignVariableCalculator calculator =
                LogNormalDistributionDesignVariableCalculator.CreateWithStandardDeviation(mean, standardDeviation, shift);

            // Assert
            Assert.IsInstanceOf<LogNormalDistributionDesignVariableCalculator>(calculator);
        }

        [Test]
        public void CreateWithCoefficientOfVariation_WithParameters_CreateNewInstance()
        {
            // Setup
            var random = new Random(21);
            double mean = random.NextDouble();
            double coefficientOfVariation = random.NextDouble();
            double shift = random.NextDouble();

            // Call
            PercentileBasedDesignVariableCalculator calculator =
                LogNormalDistributionDesignVariableCalculator.CreateWithCoefficientOfVariation(mean, coefficientOfVariation, shift);

            // Assert
            Assert.IsInstanceOf<LogNormalDistributionDesignVariableCalculator>(calculator);
        }

        [Test]
        [TestCase(8.4, 3.2, 2.5, 0.95, 14.456641939677828)]
        [TestCase(15.4, 3.2, 2.5, 0.95, 21.214883819366989)]
        [TestCase(8.4, 6.2, 2.5, 0.95, 19.305664447180419)]
        [TestCase(8.4, 6.2, 2.5, 0.45, 6.1494547175983385)]
        [TestCase(8.4, 6.2, 1.5, 0.45, 6.1594719459843708)]
        public void DetermineDesignValue_DifferentValues_BothTypesOfCalculatorsReturnsExpectedValue(
            double mean,
            double standardDeviation,
            double shift,
            double percentile,
            double expectedValue)
        {
            // Setup
            PercentileBasedDesignVariableCalculator calculatorStd = LogNormalDistributionDesignVariableCalculator.CreateWithStandardDeviation(mean, standardDeviation, shift);
            PercentileBasedDesignVariableCalculator calculatorCov = LogNormalDistributionDesignVariableCalculator.CreateWithCoefficientOfVariation(mean, standardDeviation / (mean - shift), shift);

            // Call
            double stdDesignValue = calculatorStd.GetDesignValue(percentile);
            double covDesignValue = calculatorCov.GetDesignValue(percentile);

            // Assert
            Assert.AreEqual(expectedValue, stdDesignValue, 1e-8);
            Assert.AreEqual(expectedValue, covDesignValue, 1e-8);
        }
    }
}