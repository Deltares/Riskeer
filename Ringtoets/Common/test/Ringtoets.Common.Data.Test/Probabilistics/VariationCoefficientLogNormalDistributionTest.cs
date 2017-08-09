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
    public class VariationCoefficientLogNormalDistributionTest
    {
        private static IEnumerable<TestCaseData> DistributionCombinations
        {
            get
            {
                VariationCoefficientLogNormalDistribution distribution = CreateFullyDefinedDistribution();

                yield return new TestCaseData(distribution, distribution, true)
                    .SetName("SameDistribution");
                yield return new TestCaseData(distribution, CreateFullyDefinedDistribution(), true)
                    .SetName("EqualDistribution");

                VariationCoefficientLogNormalDistribution otherMean = CreateFullyDefinedDistribution();
                otherMean.Mean = (RoundedDouble) 987;
                yield return new TestCaseData(distribution, otherMean, false)
                    .SetName(nameof(otherMean));

                VariationCoefficientLogNormalDistribution otherCoefficientOfVariation = CreateFullyDefinedDistribution();
                otherCoefficientOfVariation.CoefficientOfVariation = (RoundedDouble) 0.987;
                yield return new TestCaseData(distribution, otherCoefficientOfVariation, false)
                    .SetName(nameof(otherCoefficientOfVariation));
            }
        }

        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var distribution = new VariationCoefficientLogNormalDistribution();

            // Assert
            Assert.IsInstanceOf<IVariationCoefficientDistribution>(distribution);

            const int numberOfDecimals = RoundedDouble.MaximumNumberOfDecimalPlaces;

            Assert.AreEqual(numberOfDecimals, distribution.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(1.0, distribution.Mean.Value);
            Assert.AreEqual(numberOfDecimals, distribution.CoefficientOfVariation.NumberOfDecimalPlaces);
            Assert.AreEqual(1.0, distribution.CoefficientOfVariation.Value);
        }

        [Test]
        [TestCase(0)]
        [TestCase(9)]
        [TestCase(15)]
        public void Constructor_WithParameter_ExpectedValues(int numberOfDecimals)
        {
            // Call
            var distribution = new VariationCoefficientLogNormalDistribution(numberOfDecimals);

            // Assert
            Assert.IsInstanceOf<IVariationCoefficientDistribution>(distribution);

            Assert.AreEqual(numberOfDecimals, distribution.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(1.0, distribution.Mean.Value);
            Assert.AreEqual(numberOfDecimals, distribution.CoefficientOfVariation.NumberOfDecimalPlaces);
            Assert.AreEqual(1.0, distribution.CoefficientOfVariation.Value);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(16)]
        public void Constructor_InvalidNumberOfDecimalPlaces_ThrowArgumentOutOfRangeException(int numberOfDecimals)
        {
            // Call
            TestDelegate call = () => new VariationCoefficientLogNormalDistribution(numberOfDecimals);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, "Value must be in range [0, 15].");
        }

        [Test]
        [TestCase(0.005, 0.01)]
        [TestCase(34.56789, 34.57)]
        public void Mean_SetNewValue_ReturnNewlySetValue(double actualSetValue, double expectedRoundedValue)
        {
            // Setup
            var distribution = new VariationCoefficientLogNormalDistribution(2);

            // Call
            distribution.Mean = (RoundedDouble) actualSetValue;

            // Assert
            Assert.AreEqual(expectedRoundedValue, distribution.Mean.Value);
        }

        [Test]
        [TestCase(0.004)]
        [TestCase(-1.2)]
        public void Mean_NegativeOrZeroValue_ThrowArgumentOutOfRangeException(double invalidCoefficient)
        {
            // Setup
            var distribution = new VariationCoefficientLogNormalDistribution(2);

            // Call
            TestDelegate call = () => distribution.Mean = (RoundedDouble) invalidCoefficient;

            // Assert
            const string expectedMessage = "Gemiddelde moet groter zijn dan 0.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        [TestCase(-0.004, 0.0)]
        [TestCase(0.0, 0.0)]
        [TestCase(34.56789, 34.57)]
        public void CoefficientOfVariation_SetNewValue_ReturnNewlySetValue(
            double actualSetValue, double expectedRoundedValue)
        {
            // Setup
            var distribution = new VariationCoefficientLogNormalDistribution(2);

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
            var distribution = new VariationCoefficientLogNormalDistribution(2);

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
            var distribution = new VariationCoefficientLogNormalDistribution(random.Next(1, 16))
            {
                Mean = random.NextRoundedDouble(),
                CoefficientOfVariation = random.NextRoundedDouble()
            };

            // Call
            object clone = distribution.Clone();

            // Assert
            Assert.AreNotSame(distribution, clone);
            Assert.IsInstanceOf<VariationCoefficientLogNormalDistribution>(clone);
            DistributionAssert.AreEqual(distribution, (VariationCoefficientLogNormalDistribution) clone);
        }

        [Test]
        [TestCase(null)]
        [TestCase("string")]
        public void Equals_ToDifferentTypeOrNull_ReturnsFalse(object other)
        {
            // Setup
            VariationCoefficientLogNormalDistribution distribution = CreateFullyDefinedDistribution();

            // Call
            bool isEqual = distribution.Equals(other);

            // Assert
            Assert.IsFalse(isEqual);
        }

        [Test]
        public void Equals_TransitivePropertyAllPropertiesEqual_ReturnsTrue()
        {
            // Setup
            VariationCoefficientLogNormalDistribution distributionX = CreateFullyDefinedDistribution();
            VariationCoefficientLogNormalDistribution distributionY = CreateFullyDefinedDistribution();
            VariationCoefficientLogNormalDistribution distributionZ = CreateFullyDefinedDistribution();

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
        public void Equals_DifferentProperty_ReturnsIsEqual(VariationCoefficientLogNormalDistribution distribution,
                                                            VariationCoefficientLogNormalDistribution otherDistribution,
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
            VariationCoefficientLogNormalDistribution distribution = CreateFullyDefinedDistribution();
            VariationCoefficientLogNormalDistribution otherDistribution = CreateFullyDefinedDistribution();

            // Call
            int hashCodeOne = distribution.GetHashCode();
            int hashCodeTwo = otherDistribution.GetHashCode();

            // Assert
            Assert.AreEqual(hashCodeOne, hashCodeTwo);
        }

        private static VariationCoefficientLogNormalDistribution CreateFullyDefinedDistribution()
        {
            return new VariationCoefficientLogNormalDistribution(5)
            {
                Mean = (RoundedDouble) 1,
                CoefficientOfVariation = (RoundedDouble) 0.1
            };
        }
    }
}