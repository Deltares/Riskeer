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
using Ringtoets.Common.Data.Probabilistics;

namespace Ringtoets.Common.Data.Test.Probabilistics
{
    [TestFixture]
    public class LogNormalDistributionDesignVariableTest
    {
        [Test]
        public void ParameterdConstructor_ValidLogNormalDistribution_ExpectedValues()
        {
            // Setup
            var logNormalDistribution = new LogNormalDistribution(2);

            // Call
            var designValue = new LogNormalDistributionDesignVariable(logNormalDistribution);

            // Assert
            Assert.AreSame(logNormalDistribution, designValue.Distribution);
            Assert.AreEqual(0.5, designValue.Percentile);
        }

        /// <summary>
        /// Tests the <see cref="LogNormalDistributionDesignVariable.GetDesignValue"/>
        /// against the values calculated with the excel sheet in WTI-33 (timestamp: 27-11-2015 10:27).
        /// </summary>
        /// <param name="expectedValue">MEAN.</param>
        /// <param name="variance">VARIANCE.</param>
        /// <param name="percentile">Percentile.</param>
        /// <param name="expectedResult">Rekenwaarde.</param>
        [Test]
        [TestCase(75, 70, 0.95, 89.4965)]
        [TestCase(75, 70, 0.5, 74.5373)]
        [TestCase(75, 70, 0.05, 62.0785)]
        [TestCase(75, 123.45, 0.95, 94.5284)]
        [TestCase(75, 1.2345, 0.95, 76.8381)]
        [TestCase(123.45, 70, 0.95, 137.6756)]
        [TestCase(1.2345, 70, 0.95, 4.5413)]
        public void GetDesignVariable_ValidLogNormalDistribution_ReturnExpectedValue(
            double expectedValue, double variance, double percentile,
            double expectedResult)
        {
            // Setup
            const int numberOfDecimalPlaces = 4;
            var logNormalDistribution = new LogNormalDistribution(numberOfDecimalPlaces)
            {
                Mean = (RoundedDouble)expectedValue,
                StandardDeviation = (RoundedDouble)Math.Sqrt(variance)
            };

            var designVariable = new LogNormalDistributionDesignVariable(logNormalDistribution)
            {
                Percentile = percentile
            };

            // Call
            RoundedDouble result = designVariable.GetDesignValue();

            // Assert
            Assert.AreEqual(numberOfDecimalPlaces, result.NumberOfDecimalPlaces);
            Assert.AreEqual(expectedResult, result, 1e-4);
        }
    }
}