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
using Rhino.Mocks;
using Ringtoets.Common.Data.Probabilistics;

namespace Ringtoets.Common.Data.Test.Probabilistics
{
    [TestFixture]
    public class NormalDistributionDesignVariableTest
    {
        [Test]
        public void ParameterdConstructor_ValidLogNormalDistribution_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var normalDistribution = mocks.Stub<IDistributionBase>();
            mocks.ReplayAll();

            // Call
            var designValue = new NormalDistributionDesignVariable<IDistributionBase>(normalDistribution);

            // Assert
            Assert.AreSame(normalDistribution, designValue.Distribution);
            Assert.AreEqual(0.5, designValue.Percentile);
            mocks.VerifyAll();
        }

        /// <summary>
        /// Tests the <see cref="NormalDistributionDesignVariable{TDistribution}.GetDesignValue"/>
        /// against the values calculated with the excel sheet in WTI-33 (timestamp: 27-11-2015 10:27).
        /// </summary>
        /// <param name="expectedValue">MEAN.</param>
        /// <param name="variance">VARIANCE.</param>
        /// <param name="percentile">Percentile.</param>
        /// <param name="expectedResult">Rekenwaarde.</param>
        [Test]
        [TestCase(75, 70, 0.95, 88.76183279)]
        [TestCase(75, 70, 0.5, 75)]
        [TestCase(75, 70, 0.05, 61.23816721)]
        [TestCase(75, 123.45, 0.95, 93.27564881)]
        [TestCase(75, 1.2345, 0.95, 76.82756488)]
        [TestCase(123.45, 70, 0.95, 137.2118328)]
        [TestCase(1.2345, 70, 0.95, 14.99633279)]
        public void GetDesignVariable_ValidNormalDistribution_ReturnExpectedValue(
            double expectedValue, double variance, double percentile,
            double expectedResult)
        {
            // Setup
            const int numberOfDecimalPlaces = 4;

            var mocks = new MockRepository();
            var normalDistribution = mocks.Stub<IDistributionBase>();
            normalDistribution.Stub(d => d.Mean).Return(new RoundedDouble(numberOfDecimalPlaces, expectedValue));
            normalDistribution.Stub(d => d.StandardDeviation).Return(new RoundedDouble(numberOfDecimalPlaces, Math.Sqrt(variance)));
            mocks.ReplayAll();

            var designVariable = new NormalDistributionDesignVariable<IDistributionBase>(normalDistribution)
            {
                Percentile = percentile
            };

            // Call
            double result = designVariable.GetDesignValue();

            // Assert
            Assert.AreEqual(expectedResult, result, 1e-4);
        }
    }
}