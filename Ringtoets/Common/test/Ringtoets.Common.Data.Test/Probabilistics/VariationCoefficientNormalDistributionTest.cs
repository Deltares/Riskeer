﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Common.Data.Test.Probabilistics
{
    [TestFixture]
    public class VariationCoefficientNormalDistributionTest
    {
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
    }
}