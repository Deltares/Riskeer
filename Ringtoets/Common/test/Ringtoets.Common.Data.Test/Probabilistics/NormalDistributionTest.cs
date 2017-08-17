﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Data.TestUtil;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Common.Data.Test.Probabilistics
{
    [TestFixture]
    public class NormalDistributionTest
    {
        private static IEnumerable<TestCaseData> DistributionCombinations
        {
            get
            {
                NormalDistribution distribution = CreateFullyDefinedDistribution();

                yield return new TestCaseData(distribution, distribution, true)
                    .SetName("SameDistribution");
                yield return new TestCaseData(distribution, CreateFullyDefinedDistribution(), true)
                    .SetName("EqualDistribution");

                NormalDistribution otherMean = CreateFullyDefinedDistribution();
                otherMean.Mean = (RoundedDouble) 987;
                yield return new TestCaseData(distribution, otherMean, false)
                    .SetName(nameof(otherMean));

                NormalDistribution otherStandardDeviation = CreateFullyDefinedDistribution();
                otherStandardDeviation.StandardDeviation = (RoundedDouble) 0.987;
                yield return new TestCaseData(distribution, otherStandardDeviation, false)
                    .SetName(nameof(otherStandardDeviation));
            }
        }

        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var distribution = new NormalDistribution();

            // Assert
            Assert.IsInstanceOf<IDistribution>(distribution);

            const int numberOfDecimalPlaces = RoundedDouble.MaximumNumberOfDecimalPlaces;

            Assert.AreEqual(0.0, distribution.Mean.Value);
            Assert.AreEqual(numberOfDecimalPlaces, distribution.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(1.0, distribution.StandardDeviation.Value);
            Assert.AreEqual(numberOfDecimalPlaces, distribution.StandardDeviation.NumberOfDecimalPlaces);
        }

        [Test]
        [TestCase(0)]
        [TestCase(2)]
        [TestCase(15)]
        public void Constructor_WithParameter_ExpectedValues(int numberOfDecimalPlaces)
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
        public void Mean_SetNewValue_GetValueRoundedToGivenNumberOfDecimalPlaces()
        {
            // Setup
            const double value = 1.23456789;
            const int numberOfDecimalPlaces = 4;
            var distribution = new NormalDistribution(numberOfDecimalPlaces);

            // Call
            distribution.Mean = (RoundedDouble) value;

            // Assert
            Assert.AreEqual(numberOfDecimalPlaces, distribution.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(new RoundedDouble(numberOfDecimalPlaces, value), distribution.Mean);
        }

        [Test]
        [TestCase(1.23456789)]
        [TestCase(0 - 1e-3, Description = "Valid standard deviation due to rounding to 0.0")]
        public void StandardDeviation_SetNewValue_GetValueRoundedToGivenNumberOfDecimalPlaces(double standardDeviation)
        {
            // Setup
            const int numberOfDecimalPlaces = 2;
            var distribution = new NormalDistribution(numberOfDecimalPlaces);

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
            var distribution = new NormalDistribution(2);

            // Call
            TestDelegate call = () => distribution.StandardDeviation = (RoundedDouble) standardDeviation;

            // Assert
            const string expectedMessage = "Standaardafwijking (\u03C3) moet groter zijn dan of gelijk zijn aan 0.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        public void Clone_Always_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var random = new Random(21);
            var original = new NormalDistribution(random.Next(1, 16))
            {
                Mean = random.NextRoundedDouble(),
                StandardDeviation = random.NextRoundedDouble()
            };

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, DistributionAssert.AreEqual);
        }

        [Test]
        [TestCase(null)]
        [TestCase("string")]
        public void Equals_ToDifferentTypeOrNull_ReturnsFalse(object other)
        {
            // Setup
            NormalDistribution distribution = CreateFullyDefinedDistribution();

            // Call
            bool isEqual = distribution.Equals(other);

            // Assert
            Assert.IsFalse(isEqual);
        }

        [Test]
        public void Equals_TransitivePropertyAllPropertiesEqual_ReturnsTrue()
        {
            // Setup
            NormalDistribution distributionX = CreateFullyDefinedDistribution();
            NormalDistribution distributionY = CreateFullyDefinedDistribution();
            NormalDistribution distributionZ = CreateFullyDefinedDistribution();

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
        public void Equals_DifferentProperty_ReturnsIsEqual(NormalDistribution distribution,
                                                            NormalDistribution otherDistribution,
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
            NormalDistribution distribution = CreateFullyDefinedDistribution();
            NormalDistribution otherDistribution = CreateFullyDefinedDistribution();

            // Call
            int hashCodeOne = distribution.GetHashCode();
            int hashCodeTwo = otherDistribution.GetHashCode();

            // Assert
            Assert.AreEqual(hashCodeOne, hashCodeTwo);
        }

        private static NormalDistribution CreateFullyDefinedDistribution()
        {
            return new NormalDistribution(5)
            {
                Mean = (RoundedDouble) 1,
                StandardDeviation = (RoundedDouble) 0.1
            };
        }
    }
}