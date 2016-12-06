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

namespace Ringtoets.Common.Data.Test.Probabilistics
{
    [TestFixture]
    public class TruncatedNormalDistributionTest
    {
        [Test]
        [TestCase(0)]
        [TestCase(2)]
        [TestCase(15)]
        public void DefaultConstructor_ExpectedValues(int numberOfDecimalPlaces)
        {
            // Call
            var distribution = new TruncatedNormalDistribution(numberOfDecimalPlaces);

            // Assert
            Assert.IsInstanceOf<NormalDistribution>(distribution);
            Assert.AreEqual(0.0, distribution.Mean.Value);
            Assert.AreEqual(numberOfDecimalPlaces, distribution.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(1.0, distribution.StandardDeviation.Value);
            Assert.AreEqual(numberOfDecimalPlaces, distribution.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.0, distribution.LowerBoundary.Value);
            Assert.AreEqual(numberOfDecimalPlaces, distribution.LowerBoundary.NumberOfDecimalPlaces);
            Assert.AreEqual(0.0, distribution.UpperBoundary.Value);
            Assert.AreEqual(numberOfDecimalPlaces, distribution.UpperBoundary.NumberOfDecimalPlaces);
        }

        [Test]
        public void Mean_SetNewValue_GetValueRoundedToGivenNumberOfDecimalPlaces()
        {
            // Setup
            var value = 1.23456789;
            var numberOfDecimalPlaces = 4;
            var distribution = new TruncatedNormalDistribution(numberOfDecimalPlaces);

            // Call
            distribution.Mean = (RoundedDouble)value;

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
            var numberOfDecimalPlaces = 2;
            var distribution = new TruncatedNormalDistribution(numberOfDecimalPlaces);

            // Call
            distribution.StandardDeviation = (RoundedDouble)standardDeviation;

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
            var distribution = new TruncatedNormalDistribution(2);

            // Call
            TestDelegate call = () => distribution.StandardDeviation = (RoundedDouble)standardDeviation;

            // Assert
            const string expectedMessage = "Standaardafwijking (\u03C3) moet groter zijn dan of gelijk zijn aan 0.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        public void LowerBoundary_SetNewValue_GetValueRoundedToGivenNumberOfDecimalPlaces()
        {
            var value = 1.23456789;
            var numberOfDecimalPlaces = 4;
            var distribution = new TruncatedNormalDistribution(numberOfDecimalPlaces);

            // Call
            distribution.LowerBoundary = (RoundedDouble)value;

            // Assert
            Assert.AreEqual(numberOfDecimalPlaces, distribution.LowerBoundary.NumberOfDecimalPlaces);
            Assert.AreEqual(new RoundedDouble(numberOfDecimalPlaces, value), distribution.LowerBoundary);
        }

        [Test]
        public void UpperBoundary_SetNewValue_GetValueRoundedToGivenNumberOfDecimalPlaces()
        {
            var value = 1.23456789;
            var numberOfDecimalPlaces = 4;
            var distribution = new TruncatedNormalDistribution(numberOfDecimalPlaces);

            // Call
            distribution.UpperBoundary = (RoundedDouble)value;

            // Assert
            Assert.AreEqual(numberOfDecimalPlaces, distribution.UpperBoundary.NumberOfDecimalPlaces);
            Assert.AreEqual(new RoundedDouble(numberOfDecimalPlaces, value), distribution.UpperBoundary);
        }
    }
}