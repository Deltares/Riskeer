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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Probabilistics;

namespace Ringtoets.Common.Data.Test.Probabilistics
{
    [TestFixture]
    public class NomalDistributionTest
    {
        [Test]
        [TestCase(0)]
        [TestCase(2)]
        [TestCase(15)]
        public void DefaultConstructor_ExpectedValues(int numberOfDecimalPlaces)
        {
            // Call
            var distribution = new NormalDistribution(numberOfDecimalPlaces);

            // Assert
            Assert.IsInstanceOf<IDistribution>(distribution);
            Assert.AreEqual(0.0, distribution.Mean.Value);
            Assert.AreEqual(numberOfDecimalPlaces, distribution.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(1.0, distribution.StandardDeviation.Value);
            Assert.AreEqual(numberOfDecimalPlaces, distribution.StandardDeviation.NumberOfDecimalPlaces);
        }

        [Test]
        [TestCase(0, 1.0)]
        [TestCase(3, 1.235)]
        [TestCase(4, 1.2345)]
        [TestCase(15, 1.234500000000000)]
        public void Mean_SetNewValue_GetValueRoundedToGivenNumberOfDecimalPlaces(int numberOfDecimalPlaces, double expectedStandardDeviation)
        {
            // Setup
            var distribution = new NormalDistribution(numberOfDecimalPlaces);

            // Call
            distribution.Mean = new RoundedDouble(4, 1.2345);

            // Assert
            Assert.AreEqual(numberOfDecimalPlaces, distribution.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(expectedStandardDeviation, distribution.Mean.Value);
        }

        [Test]
        [TestCase(0 - 1e-2)]
        [TestCase(-4)]
        public void StandardDeviation_SettingToLessThan0_ThrowArgumentOutOfRangeException(double newStd)
        {
            // Setup
            var distribution = new NormalDistribution(2);

            // Call
            TestDelegate call = () => distribution.StandardDeviation = (RoundedDouble) newStd;

            // Assert
            const string expectedMessage = "Standaard afwijking (\u03C3) moet groter zijn dan of gelijk zijn aan 0.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        [TestCase(0, 6.0)]
        [TestCase(2, 5.68)]
        [TestCase(3, 5.678)]
        [TestCase(15, 5.678000000000000)]
        public void StandardDeviation_SetNewValue_GetValueRoundedToGivenNumberOfDecimalPlaces(int numberOfDecimalPlaces, double expectedStandardDeviation)
        {
            // Setup
            var distribution = new NormalDistribution(numberOfDecimalPlaces);

            // Call
            distribution.StandardDeviation = new RoundedDouble(3, 5.678);

            // Assert
            Assert.AreEqual(numberOfDecimalPlaces, distribution.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(expectedStandardDeviation, distribution.StandardDeviation.Value);
        }
    }
}