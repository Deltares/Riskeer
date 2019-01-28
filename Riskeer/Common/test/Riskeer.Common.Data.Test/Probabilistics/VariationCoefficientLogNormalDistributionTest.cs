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
    public class VariationCoefficientLogNormalDistributionTest
    {
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
            Assert.AreEqual(numberOfDecimals, distribution.Shift.NumberOfDecimalPlaces);
            Assert.AreEqual(0, distribution.Shift.Value);
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
            Assert.AreEqual(numberOfDecimals, distribution.Shift.NumberOfDecimalPlaces);
            Assert.AreEqual(0, distribution.Shift.Value);
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
        public void Mean_SettingToLessThanShift_ThrowArgumentOutOfRangeException()
        {
            // Setup
            var distribution = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = new RoundedDouble(2, 20),
                Shift = new RoundedDouble(2, 10)
            };

            // Call
            TestDelegate test = () => distribution.Mean = (RoundedDouble) 5;

            // Assert
            const string expectedMessage = "De verschuiving mag niet groter zijn dan de verwachtingswaarde.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, expectedMessage);
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
        [TestCase(1, 5.6)]
        [TestCase(3, 5.647)]
        [TestCase(4, 5.6473)]
        [TestCase(15, 5.647300000000000)]
        public void Shift_SetNewValue_GetValueRoundedToGivenNumberOfDecimalPlaces(int numberOfDecimalPlaces, double expectedShift)
        {
            // Setup
            var distribution = new VariationCoefficientLogNormalDistribution(numberOfDecimalPlaces)
            {
                Mean = new RoundedDouble(2, 10.0)
            };

            // Call
            distribution.Shift = new RoundedDouble(4, 5.6473);

            // Assert
            Assert.AreEqual(numberOfDecimalPlaces, distribution.Shift.NumberOfDecimalPlaces);
            Assert.AreEqual(expectedShift, distribution.Shift.Value);
        }

        [Test]
        public void Shift_SetIllegalValue_ThrowArgumentOutOfRangeException()
        {
            // Setup
            var distribution = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = new RoundedDouble(2, 10.0),
                CoefficientOfVariation = new RoundedDouble(2, 1.0)
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
            var original = new VariationCoefficientLogNormalDistribution(random.Next(1, 16))
            {
                Mean = random.NextRoundedDouble(),
                CoefficientOfVariation = random.NextRoundedDouble(),
                Shift = random.NextRoundedDouble()
            };

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, DistributionAssert.AreEqual);
        }

        [TestFixture]
        private class VariationCoefficientLogNormalDistributionEqualsTest : EqualsTestFixture<VariationCoefficientLogNormalDistribution,
            DerivedVariationCoefficientLogNormalDistribution>
        {
            protected override VariationCoefficientLogNormalDistribution CreateObject()
            {
                return CreateFullyDefinedDistribution();
            }

            protected override DerivedVariationCoefficientLogNormalDistribution CreateDerivedObject()
            {
                VariationCoefficientLogNormalDistribution baseDistribution = CreateFullyDefinedDistribution();
                return new DerivedVariationCoefficientLogNormalDistribution(baseDistribution);
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                VariationCoefficientLogNormalDistribution otherMean = CreateFullyDefinedDistribution();
                otherMean.Mean = (RoundedDouble) 987;
                yield return new TestCaseData(otherMean)
                    .SetName("Mean");

                VariationCoefficientLogNormalDistribution otherCoefficientOfVariation = CreateFullyDefinedDistribution();
                otherCoefficientOfVariation.CoefficientOfVariation = (RoundedDouble) 0.987;
                yield return new TestCaseData(otherCoefficientOfVariation)
                    .SetName("CoefficientOfVariation");

                VariationCoefficientLogNormalDistribution otherShift = CreateFullyDefinedDistribution();
                otherShift.Shift = (RoundedDouble) 0.987;
                yield return new TestCaseData(otherShift)
                    .SetName("Shift");
            }

            private static VariationCoefficientLogNormalDistribution CreateFullyDefinedDistribution()
            {
                return new VariationCoefficientLogNormalDistribution(5)
                {
                    Mean = (RoundedDouble) 1,
                    CoefficientOfVariation = (RoundedDouble) 0.1,
                    Shift = (RoundedDouble) 0.2
                };
            }
        }

        private class DerivedVariationCoefficientLogNormalDistribution : VariationCoefficientLogNormalDistribution
        {
            public DerivedVariationCoefficientLogNormalDistribution(VariationCoefficientLogNormalDistribution distribution)
            {
                Mean = distribution.Mean;
                CoefficientOfVariation = distribution.CoefficientOfVariation;
                Shift = distribution.Shift;
            }
        }
    }
}