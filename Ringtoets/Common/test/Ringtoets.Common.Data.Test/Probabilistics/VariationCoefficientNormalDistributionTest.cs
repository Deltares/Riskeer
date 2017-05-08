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
    public class VariationCoefficientNormalDistributionTest
    {
        private static IEnumerable<TestCaseData> DistributionCombinations
        {
            get
            {
                VariationCoefficientNormalDistribution distribution = CreateFullyDefinedDistribution();

                yield return new TestCaseData(distribution, distribution, true)
                    .SetName("SameDistribution");
                yield return new TestCaseData(distribution, CreateFullyDefinedDistribution(), true)
                    .SetName("EqualDistribution");

                VariationCoefficientNormalDistribution otherMean = CreateFullyDefinedDistribution();
                otherMean.Mean = (RoundedDouble) 987;
                yield return new TestCaseData(distribution, otherMean, false)
                    .SetName(nameof(otherMean));

                VariationCoefficientNormalDistribution otherCoefficientOfVariation = CreateFullyDefinedDistribution();
                otherCoefficientOfVariation.CoefficientOfVariation = (RoundedDouble) 0.987;
                yield return new TestCaseData(distribution, otherCoefficientOfVariation, false)
                    .SetName(nameof(otherCoefficientOfVariation));
            }
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var distribution = new VariationCoefficientNormalDistribution();

            // Assert
            Assert.IsInstanceOf<IVariationCoefficientDistribution>(distribution);

            const int numberOfDecimalPlaces = RoundedDouble.MaximumNumberOfDecimalPlaces;

            Assert.AreEqual(numberOfDecimalPlaces, distribution.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(1.0, distribution.Mean.Value);
            Assert.AreEqual(numberOfDecimalPlaces, distribution.CoefficientOfVariation.NumberOfDecimalPlaces);
            Assert.AreEqual(1.0, distribution.CoefficientOfVariation.Value);
        }

        [Test]
        [TestCase(0)]
        [TestCase(9)]
        [TestCase(15)]
        public void Constructor_WithParameter_ExpectedValues(int numberOfDecimalPlaces)
        {
            // Call
            var distribution = new VariationCoefficientNormalDistribution(numberOfDecimalPlaces);

            // Assert
            Assert.IsInstanceOf<IVariationCoefficientDistribution>(distribution);

            Assert.AreEqual(numberOfDecimalPlaces, distribution.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(1.0, distribution.Mean.Value);
            Assert.AreEqual(numberOfDecimalPlaces, distribution.CoefficientOfVariation.NumberOfDecimalPlaces);
            Assert.AreEqual(1.0, distribution.CoefficientOfVariation.Value);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(16)]
        public void Constructor_InvalidNumberOfDecimalPlaces_ThrowsArgumentOutOfRangeException(int numberOfDecimalPlaces)
        {
            // Call
            TestDelegate call = () => new VariationCoefficientNormalDistribution(numberOfDecimalPlaces);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, "Value must be in range [0, 15].");
        }

        [Test]
        public void Mean_SetNewValue_ReturnNewlySetValue()
        {
            // Setup
            var distribution = new VariationCoefficientNormalDistribution(2);

            // Call
            distribution.Mean = (RoundedDouble) 12.34567;

            // Assert
            Assert.AreEqual(12.35, distribution.Mean.Value);
        }

        [Test]
        [TestCase(-0.004, 0.0)]
        [TestCase(0.0, 0.0)]
        [TestCase(34.56789, 34.57)]
        public void CoefficientOfVariation_SetNewValue_ReturnNewlySetValue(
            double actualSetValue, double expectedRoundedValue)
        {
            // Setup
            var distribution = new VariationCoefficientNormalDistribution(2);

            // Call
            distribution.CoefficientOfVariation = (RoundedDouble) actualSetValue;

            // Assert
            Assert.AreEqual(expectedRoundedValue, distribution.CoefficientOfVariation.Value);
        }

        [Test]
        [TestCase(-0.005)]
        [TestCase(-1.2)]
        public void CoefficientOfVariation_NegativeValue_ThrowArgumentOutOfRangeException(double invalidCoefficient)
        {
            // Setup
            var distribution = new VariationCoefficientNormalDistribution(2);

            // Call
            TestDelegate call = () => distribution.CoefficientOfVariation = (RoundedDouble) invalidCoefficient;

            // Assert
            const string expectedMessage = "Variatiecoëfficiënt (CV) moet groter zijn dan of gelijk zijn aan 0.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        public void Clone_Always_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var random = new Random(21);
            var distribution = new VariationCoefficientNormalDistribution(random.Next(1, 16))
            {
                Mean = random.NextRoundedDouble(),
                CoefficientOfVariation = random.NextRoundedDouble()
            };

            // Call
            object clone = distribution.Clone();

            // Assert
            Assert.IsInstanceOf<VariationCoefficientNormalDistribution>(clone);
            var clonedDistribution = (VariationCoefficientNormalDistribution) clone;
            Assert.AreNotSame(distribution, clonedDistribution);
            Assert.AreNotSame(distribution.Mean, clonedDistribution.Mean);
            Assert.AreNotSame(distribution.CoefficientOfVariation, clonedDistribution.CoefficientOfVariation);
            DistributionAssert.AreEqual(distribution, clonedDistribution);
        }

        [Test]
        [TestCase(null)]
        [TestCase("string")]
        public void Equals_ToDifferentTypeOrNull_ReturnsFalse(object other)
        {
            // Setup
            VariationCoefficientNormalDistribution distribution = CreateFullyDefinedDistribution();

            // Call
            bool isEqual = distribution.Equals(other);

            // Assert
            Assert.IsFalse(isEqual);
        }

        [Test]
        public void Equals_TransitivePropertyAllPropertiesEqual_ReturnsTrue()
        {
            // Setup
            VariationCoefficientNormalDistribution distributionX = CreateFullyDefinedDistribution();
            VariationCoefficientNormalDistribution distributionY = CreateFullyDefinedDistribution();
            VariationCoefficientNormalDistribution distributionZ = CreateFullyDefinedDistribution();

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
        public void Equal_DifferentProperty_RetunsIsEqual(VariationCoefficientNormalDistribution distribution,
                                                        VariationCoefficientNormalDistribution otherDistribution,
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
            VariationCoefficientNormalDistribution distribution = CreateFullyDefinedDistribution();
            VariationCoefficientNormalDistribution otherDistribution = CreateFullyDefinedDistribution();

            // Call
            int hashCodeOne = distribution.GetHashCode();
            int hashCodeTwo = otherDistribution.GetHashCode();

            // Assert
            Assert.AreEqual(hashCodeOne, hashCodeTwo);
        }

        private static VariationCoefficientNormalDistribution CreateFullyDefinedDistribution()
        {
            return new VariationCoefficientNormalDistribution(5)
            {
                Mean = (RoundedDouble) 1,
                CoefficientOfVariation = (RoundedDouble) 0.1
            };
        }
    }
}