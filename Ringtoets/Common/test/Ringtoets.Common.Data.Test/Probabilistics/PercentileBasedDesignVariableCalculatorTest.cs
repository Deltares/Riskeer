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

namespace Ringtoets.Common.Data.Test.Probabilistics
{
    [TestFixture]
    public class PercentileBasedDesignVariableCalculatorTest
    {
        [Test]
        public void Constructor_WithParameters_SetParameterValues()
        {
            // Setup
            var random = new Random(21);
            double mean = random.NextDouble();
            double standardDeviation = random.NextDouble();
            double coefficientOfVariation = random.NextDouble();

            // Call
            var calculator = new SimplePercentileBasedDesignVariableCalculator(mean, standardDeviation, coefficientOfVariation);

            // Assert
            Assert.AreEqual(mean, calculator.PublicMean);
            Assert.AreEqual(standardDeviation, calculator.PublicStandardDeviation);
            Assert.AreEqual(coefficientOfVariation, calculator.PublicCoefficientOfVariation);
        }

        [Test]
        [TestCase(8.4, 3.2, 0.95, 13.663531606244712)]
        [TestCase(15.4, 3.2, 0.95, 20.66353160624471)]
        [TestCase(8.4, 6.2, 0.95, 18.59809248709913)]
        [TestCase(8.4, 6.2, 0.45, 7.6208996494985417)]
        public void DetermineDesignValue_DifferentValues_ReturnsExpectedValue(double mean, double standardDeviation, double percentile, double expectedValue)
        {
            // Setup
            var calculator = new SimplePercentileBasedDesignVariableCalculator(mean, standardDeviation, double.NaN);

            // Call
            double actualDesignVariable = calculator.PublicDetermineDesignVariable(mean, standardDeviation, percentile);

            // Assert
            Assert.AreEqual(expectedValue, actualDesignVariable, 1e-8);
        }

        private class SimplePercentileBasedDesignVariableCalculator : PercentileBasedDesignVariableCalculator
        {
            public SimplePercentileBasedDesignVariableCalculator(
                double mean,
                double standardDeviation,
                double coefficientOfVariation)
                : base(mean, standardDeviation, coefficientOfVariation) {}

            public double PublicMean
            {
                get
                {
                    return Mean;
                }
            }

            public double PublicStandardDeviation
            {
                get
                {
                    return StandardDeviation;
                }
            }

            public double PublicCoefficientOfVariation
            {
                get
                {
                    return CoefficientOfVariation;
                }
            }

            public double PublicDetermineDesignVariable(double expectedValue, double standardDeviation, double percentile)
            {
                return DetermineDesignValue(expectedValue, standardDeviation, percentile);
            }

            internal override double GetDesignValue(double percentile)
            {
                throw new NotImplementedException();
            }
        }
    }
}