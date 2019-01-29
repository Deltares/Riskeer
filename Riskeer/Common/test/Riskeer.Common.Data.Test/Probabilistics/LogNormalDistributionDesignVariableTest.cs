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
using Core.Common.Base.Data;
using NUnit.Framework;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Common.Data.Test.Probabilistics
{
    [TestFixture]
    public class LogNormalDistributionDesignVariableTest
    {
        [Test]
        public void ParameteredConstructor_ValidLogNormalDistribution_ExpectedValues()
        {
            // Setup
            var logNormalDistribution = new LogNormalDistribution(2);

            // Call
            var designVariable = new LogNormalDistributionDesignVariable(logNormalDistribution);

            // Assert
            Assert.IsInstanceOf<PercentileBasedDesignVariable<LogNormalDistribution>>(designVariable);
            Assert.AreSame(logNormalDistribution, designVariable.Distribution);
            Assert.AreEqual(0.5, designVariable.Percentile);
        }

        [Test]
        [TestCase(75, 70, 0.95, 89.49908018)]
        [TestCase(75, 70, 0.5, 74.53764421)]
        [TestCase(75, 70, 0.05, 62.07729055)]
        [TestCase(75, 123.45, 0.95, 94.5366392)]
        [TestCase(75, 1.2345, 0.95, 76.84147913)]
        [TestCase(123.45, 70, 0.95, 137.6747689)]
        [TestCase(1.2345, 70, 0.95, 4.541270837)]
        public void GetDesignValue_ValidLogNormalDistribution_ReturnExpectedValue(
            double expectedValue, double variance, double percentile,
            double expectedResult)
        {
            // Setup
            const int numberOfDecimalPlaces = 4;
            var logNormalDistribution = new LogNormalDistribution(numberOfDecimalPlaces)
            {
                Mean = (RoundedDouble) expectedValue,
                StandardDeviation = (RoundedDouble) Math.Sqrt(variance)
            };

            var designVariable = new LogNormalDistributionDesignVariable(logNormalDistribution)
            {
                Percentile = percentile
            };

            // Call
            RoundedDouble result = designVariable.GetDesignValue();

            // Assert
            Assert.AreEqual(numberOfDecimalPlaces, result.NumberOfDecimalPlaces);
            Assert.AreEqual(expectedResult, result, result.GetAccuracy());
        }

        [Test]
        [TestCase(75, 20, 10, 0.95, 82.60703184)]
        [TestCase(75, 70, 10, 0.5, 74.46813834)]
        [TestCase(75, 70, 10, 0.95, 89.60066615)]
        [TestCase(75, 70, 10, 0.05, 62.21238794)]
        [TestCase(75, 70, -30, 0.95, 89.30145551)]
        [TestCase(75, 123.45, 10, 0.95, 94.70113921)]
        [TestCase(75, 1.2345, 10, 0.95, 76.84359757)]
        [TestCase(123.45, 70, 10, 0.95, 137.713425)]
        public void GetDesignValue_ValidLogNormalDistributionWithNonZeroShift_ReturnExpectedValue(
            double expectedValue, double variance, double shift, double percentile,
            double expectedResult)
        {
            // Setup
            const int numberOfDecimalPlaces = 4;
            var logNormalDistribution = new LogNormalDistribution(numberOfDecimalPlaces)
            {
                Mean = (RoundedDouble) expectedValue,
                StandardDeviation = (RoundedDouble) Math.Sqrt(variance),
                Shift = (RoundedDouble) shift
            };

            var designVariable = new LogNormalDistributionDesignVariable(logNormalDistribution)
            {
                Percentile = percentile
            };

            // Call
            RoundedDouble result = designVariable.GetDesignValue();

            // Assert
            Assert.AreEqual(numberOfDecimalPlaces, result.NumberOfDecimalPlaces);
            Assert.AreEqual(expectedResult, result, result.GetAccuracy());
        }
    }
}