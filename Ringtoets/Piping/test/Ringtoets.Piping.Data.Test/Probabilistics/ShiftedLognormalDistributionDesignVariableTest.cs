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
using System.Security.Cryptography;

using Core.Common.Base.Data;

using NUnit.Framework;

using Ringtoets.Piping.Data.Probabilistics;

namespace Ringtoets.Piping.Data.Test.Probabilistics
{
    [TestFixture]
    public class ShiftedLognormalDistributionDesignVariableTest
    {
        [Test]
        public void ParameterdConstructor_ValidLognormalDistribution_ExpectedValues()
        {
            // Setup
            var shiftedLognormalDistribution = new ShiftedLognormalDistribution(1);

            // Call
            var designValue = new ShiftedLognormalDistributionDesignVariable(shiftedLognormalDistribution);

            // Assert
            Assert.AreSame(shiftedLognormalDistribution, designValue.Distribution);
            Assert.AreEqual(0.5, designValue.Percentile);
        }

        /// <summary>
        /// Tests the <see cref="ShiftedLognormalDistributionDesignVariable.GetDesignValue"/>
        /// against the values calculated with the excel sheet in WTI-33 (timestamp: 27-11-2015 10:27).
        /// </summary>
        /// <param name="expectedValue">MEAN.</param>
        /// <param name="variance">VARIANCE.</param>
        /// <param name="shift">SHIFT</param>
        /// <param name="percentile">Percentile.</param>
        /// <param name="expectedResult">Rekenwaarde.</param>
        [Test]
        [TestCase(75, 70, 10, 0.5, 84.5373)]
        [TestCase(75, 70, 10, 0.95, 99.4965)]
        [TestCase(75, 70, 10, 0.05, 72.0785)]
        [TestCase(75, 70, -30, 0.95, 59.4965)]
        [TestCase(75, 70, 123.45, 0.95, 212.9465)]
        [TestCase(75, 123.45, 10, 0.95, 104.5284)]
        [TestCase(75, 1.2345, 10, 0.95, 86.8381)]
        [TestCase(123.45, 70, 10, 0.95, 147.6756)]
        [TestCase(1.2345, 70, 10, 0.95, 14.54127084)]
        public void GetDesignVariable_ValidShiftedLognormalDistribution_ReturnExpectedValue(
            double expectedValue, double variance, double shift, double percentile,
            double expectedResult)
        {
            // Setup
            const int numberOfDecimalPlaces = 4;
            var shiftedLognormalDistribution = new ShiftedLognormalDistribution(numberOfDecimalPlaces)
            {
                Mean = (RoundedDouble)expectedValue,
                StandardDeviation = (RoundedDouble)Math.Sqrt(variance),
                Shift = (RoundedDouble)shift
            };

            var designVariable = new ShiftedLognormalDistributionDesignVariable(shiftedLognormalDistribution)
            {
                Percentile = percentile
            };

            // Call
            RoundedDouble result = designVariable.GetDesignValue();

            // Assert
            Assert.AreEqual(numberOfDecimalPlaces, result.NumberOfDecimalPlaces);
            Assert.AreEqual(expectedResult, result, 1e-4);
        }

        [Test]
        [TestCase(75, 70, 0.5)]
        [TestCase(75, 70, 0.95)]
        [TestCase(75, 70, 0.05)]
        [TestCase(75, 123.45, 0.95)]
        [TestCase(75, 1.2345, 0.95)]
        [TestCase(123.45, 70, 0.95)]
        [TestCase(1.2345, 70, 0.95)]
        public void GetDesignVariable_ShiftIsZero_ReturnIdenticalValueAsLognormalDistributionDesignVariable(
            double expectedValue, double variance, double percentile)
        {
            // Setup
            const int numberOfDecimalPlaces = 6;
            var shiftedLognormalDistribution = new ShiftedLognormalDistribution(numberOfDecimalPlaces)
            {
                Mean = (RoundedDouble)expectedValue,
                StandardDeviation = (RoundedDouble)Math.Sqrt(variance),
                Shift = (RoundedDouble)0.0
            };

            var designVariable = new ShiftedLognormalDistributionDesignVariable(shiftedLognormalDistribution)
            {
                Percentile = percentile
            };

            // Call
            RoundedDouble result = designVariable.GetDesignValue();

            // Assert
            RoundedDouble expectedResult = new LognormalDistributionDesignVariable(shiftedLognormalDistribution)
            {
                Percentile = percentile
            }.GetDesignValue();

            Assert.AreEqual(numberOfDecimalPlaces, result.NumberOfDecimalPlaces);
            Assert.AreEqual(expectedResult, result, 1e-6);
        }
    }
}