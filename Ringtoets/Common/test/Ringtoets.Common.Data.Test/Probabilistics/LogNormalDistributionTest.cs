// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Collections.Generic;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Common.Data.Test.Probabilistics
{
    [TestFixture]
    public class LogNormalDistributionTest
    {
        private static IEnumerable<TestCaseData> DistributionCombinations
        {
            get
            {
                LogNormalDistribution distribution = CreateFullyDefinedDistribution();

                yield return new TestCaseData(distribution, distribution, true)
                    .SetName("SameDistribution");
                yield return new TestCaseData(distribution, CreateFullyDefinedDistribution(), true)
                    .SetName("EqualDistribution");

                LogNormalDistribution otherMean = CreateFullyDefinedDistribution();
                otherMean.Mean = (RoundedDouble) 987;
                yield return new TestCaseData(distribution, otherMean, false)
                    .SetName(nameof(otherMean));

                LogNormalDistribution otherStandardDeviation = CreateFullyDefinedDistribution();
                otherStandardDeviation.StandardDeviation = (RoundedDouble) 0.987;
                yield return new TestCaseData(distribution, otherStandardDeviation, false)
                    .SetName(nameof(otherStandardDeviation));

                LogNormalDistribution otherShift = CreateFullyDefinedDistribution();
                otherShift.Shift = (RoundedDouble) 0.987;
                yield return new TestCaseData(distribution, otherShift, false)
                    .SetName(nameof(otherShift));
            }
        }

        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var distribution = new LogNormalDistribution();

            // Assert
            Assert.IsInstanceOf<IDistribution>(distribution);

            const int numberOfDecimalPlaces = RoundedDouble.MaximumNumberOfDecimalPlaces;
            double expectedAccuracy = Math.Pow(10.0, -numberOfDecimalPlaces);

            Assert.AreEqual(Math.Exp(-0.5), distribution.Mean, expectedAccuracy);
            Assert.AreEqual(numberOfDecimalPlaces, distribution.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(Math.Sqrt((Math.Exp(1) - 1) * Math.Exp(1)), distribution.StandardDeviation, expectedAccuracy);
            Assert.AreEqual(numberOfDecimalPlaces, distribution.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.0, distribution.Shift, expectedAccuracy);
            Assert.AreEqual(numberOfDecimalPlaces, distribution.Shift.NumberOfDecimalPlaces);
        }

        [Test]
        [TestCase(1)]
        [TestCase(9)]
        [TestCase(15)]
        public void Constructor_WithParameter_ExpectedValues(int numberOfDecimalPlaces)
        {
            // Call
            var distribution = new LogNormalDistribution(numberOfDecimalPlaces);

            // Assert
            Assert.IsInstanceOf<IDistribution>(distribution);

            double expectedAccuracy = Math.Pow(10.0, -numberOfDecimalPlaces);

            Assert.AreEqual(Math.Exp(-0.5), distribution.Mean, expectedAccuracy);
            Assert.AreEqual(numberOfDecimalPlaces, distribution.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(Math.Sqrt((Math.Exp(1) - 1) * Math.Exp(1)), distribution.StandardDeviation, expectedAccuracy);
            Assert.AreEqual(numberOfDecimalPlaces, distribution.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.0, distribution.Shift, expectedAccuracy);
            Assert.AreEqual(numberOfDecimalPlaces, distribution.Shift.NumberOfDecimalPlaces);
        }

        [Test]
        public void Constructor_InvalidNumberOfDecimalPlaces_ThrowArgumentOutOfRangeException()
        {
            // Call
            TestDelegate call = () => new LogNormalDistribution(0);

            // Assert
            const string expectedMessage = "Value must be in range [1, 15].";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        public void Mean_SetNewValue_GetValueRoundedToGivenNumberOfDecimalPlaces()
        {
            // Setup
            const double value = 1.23456789;
            const int numberOfDecimalPlaces = 2;
            var distribution = new LogNormalDistribution(numberOfDecimalPlaces);

            // Call
            distribution.Mean = (RoundedDouble) value;

            // Assert
            Assert.AreEqual(numberOfDecimalPlaces, distribution.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(new RoundedDouble(numberOfDecimalPlaces, value), distribution.Mean);
        }

        [Test]
        [TestCase(0)]
        [TestCase(0 + 1e-3, Description = "Invalid mean due to rounding to 0.0")]
        [TestCase(-123.45)]
        public void Mean_SettingToLessThanOrEqualTo0_ThrowArgumentOutOfRangeException(double mean)
        {
            // Setup
            var distribution = new LogNormalDistribution(2);

            // Call
            TestDelegate call = () => distribution.Mean = (RoundedDouble) mean;

            // Assert
            const string expectedMessage = "Gemiddelde moet groter zijn dan 0.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        [TestCase(1.23456789)]
        [TestCase(0 - 1e-3, Description = "Valid standard deviation due to rounding to 0.0")]
        public void StandardDeviation_SetNewValue_GetValueRoundedToGivenNumberOfDecimalPlaces(double standardDeviation)
        {
            // Setup
            const int numberOfDecimalPlaces = 2;
            var distribution = new LogNormalDistribution(numberOfDecimalPlaces);

            // Call
            distribution.StandardDeviation = (RoundedDouble) standardDeviation;

            // Assert
            Assert.AreEqual(numberOfDecimalPlaces, distribution.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(new RoundedDouble(numberOfDecimalPlaces, standardDeviation), distribution.StandardDeviation);
        }

        [Test]
        [TestCase(-4)]
        [TestCase(0 - 1e-2)]
        public void StandardDeviation_SettingToLessThan0_ThrowArgumentOutOfRangeException(double standardDeviation)
        {
            // Setup
            var distribution = new LogNormalDistribution(2);

            // Call
            TestDelegate call = () => distribution.StandardDeviation = (RoundedDouble) standardDeviation;

            // Assert
            const string expectedMessage = "Standaardafwijking (\u03C3) moet groter zijn dan of gelijk zijn aan 0.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        [TestCase(1, 5.6)]
        [TestCase(3, 5.647)]
        [TestCase(4, 5.6473)]
        [TestCase(15, 5.647300000000000)]
        public void Shift_SetNewValue_GetValueRoundedToGivenNumberOfDecimalPlaces(int numberOfDecimalPlaces, double expectedStandardDeviation)
        {
            // Setup
            var distribution = new LogNormalDistribution(numberOfDecimalPlaces)
            {
                Mean = new RoundedDouble(2, 10.0)
            };

            // Call
            distribution.Shift = new RoundedDouble(4, 5.6473);

            // Assert
            Assert.AreEqual(numberOfDecimalPlaces, distribution.Shift.NumberOfDecimalPlaces);
            Assert.AreEqual(expectedStandardDeviation, distribution.Shift.Value);
        }

        [Test]
        public void Shift_SetIllegalValue_ThrowArgumentOutOfRangeException()
        {
            // Setup
            var distribution = new LogNormalDistribution(2)
            {
                Mean = new RoundedDouble(2, 10.0),
                StandardDeviation = new RoundedDouble(2, 1.0)
            };

            // Call
            TestDelegate call = () => distribution.Shift = new RoundedDouble(2, 100.0);

            // Assert
            const string expectedMessage = "De verschuiving mag niet groter zijn dan de verwachtingswaarde.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        public void Clone_Always_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var random = new Random(21);
            var distribution = new LogNormalDistribution(random.Next(1, 16))
            {
                Mean = random.NextRoundedDouble(),
                StandardDeviation = random.NextRoundedDouble()
            };

            // Call
            object clone = distribution.Clone();

            // Assert
            Assert.IsInstanceOf<LogNormalDistribution>(clone);
            var clonedDistribution = (LogNormalDistribution) clone;
            Assert.AreNotSame(distribution, clonedDistribution);
            Assert.AreNotSame(distribution.Mean, clonedDistribution.Mean);
            Assert.AreNotSame(distribution.StandardDeviation, clonedDistribution.StandardDeviation);
            DistributionAssert.AreEqual(distribution, clonedDistribution);
        }

        [Test]
        [TestCase(null)]
        [TestCase("string")]
        public void Equals_ToDifferentTypeOrNull_ReturnsFalse(object other)
        {
            // Setup
            LogNormalDistribution distribution = CreateFullyDefinedDistribution();

            // Call
            bool isEqual = distribution.Equals(other);

            // Assert
            Assert.IsFalse(isEqual);
        }

        [Test]
        public void Equals_TransitivePropertyAllPropertiesEqual_ReturnsTrue()
        {
            // Setup
            LogNormalDistribution distributionX = CreateFullyDefinedDistribution();
            LogNormalDistribution distributionY = CreateFullyDefinedDistribution();
            LogNormalDistribution distributionZ = CreateFullyDefinedDistribution();

            // Call
            bool isXEqualToY = distributionX.Equals(distributionY);
            bool isYEqualToZ = distributionY.Equals(distributionZ);
            bool isXEqualToZ = distributionX.Equals(distributionZ);

            // Assert
            Assert.IsTrue(isXEqualToY);
            Assert.IsTrue(isYEqualToZ);
            Assert.IsTrue(isXEqualToZ);
        }

        [Test]
        [TestCaseSource(nameof(DistributionCombinations))]
        public void Equal_DifferentProperty_RetunsIsEqual(LogNormalDistribution distribution,
                                                          LogNormalDistribution otherDistribution,
                                                          bool expectedToBeEqual)
        {
            // Call
            bool isDistributionEqualToOther = distribution.Equals(otherDistribution);
            bool isOtherEqualToDistribution = otherDistribution.Equals(distribution);

            // Assert
            Assert.AreEqual(expectedToBeEqual, isDistributionEqualToOther);
            Assert.AreEqual(expectedToBeEqual, isOtherEqualToDistribution);
        }

        [Test]
        public void GetHashCode_EqualDistributions_ReturnsSameHashCode()
        {
            // Setup
            LogNormalDistribution distribution = CreateFullyDefinedDistribution();
            LogNormalDistribution otherDistribution = CreateFullyDefinedDistribution();

            // Call
            int hashCodeOne = distribution.GetHashCode();
            int hashCodeTwo = otherDistribution.GetHashCode();

            // Assert
            Assert.AreEqual(hashCodeOne, hashCodeTwo);
        }

        private static LogNormalDistribution CreateFullyDefinedDistribution()
        {
            return new LogNormalDistribution(5)
            {
                Mean = (RoundedDouble) 1,
                StandardDeviation = (RoundedDouble) 0.1,
                Shift = (RoundedDouble) 0.2
            };
        }
    }
}