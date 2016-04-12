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

using Ringtoets.Piping.Data.Probabilistics;

namespace Ringtoets.Piping.Data.Test.Probabilistics
{
    [TestFixture]
    public class LognormalDistributionTest
    {
        [Test]
        [TestCase(1)]
        [TestCase(9)]
        [TestCase(15)]
        public void DefaultConstructor_ExpectedValues(int numberOfDecimalPlaces)
        {
            // Call
            var distribution = new LognormalDistribution(numberOfDecimalPlaces);

            // Assert
            Assert.IsInstanceOf<IDistribution>(distribution);
            double expectedAccuracy = Math.Pow(10.0, -numberOfDecimalPlaces);
            Assert.AreEqual(Math.Exp(-0.5), distribution.Mean, expectedAccuracy);
            Assert.AreEqual(numberOfDecimalPlaces, distribution.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(Math.Sqrt((Math.Exp(1) - 1) * Math.Exp(1)), distribution.StandardDeviation, expectedAccuracy);
            Assert.AreEqual(numberOfDecimalPlaces, distribution.StandardDeviation.NumberOfDecimalPlaces);
        }

        [Test]
        public void Constructor_InvalidNumberOfDecimalPlaces_ThrowArgumentOutOfRangeException()
        {
            // Setup

            // Call
            TestDelegate call = () => new LognormalDistribution(0);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, "Value must be in range [1, 15].");
        }

        [Test]
        [TestCase(0)]
        [TestCase(-123.45)]
        public void Mean_SettingToLessThanOrEqualTo0_ThrowArgumentOutOfRangeException(double newMean)
        {
            // Setup
            var distribution = new LognormalDistribution(2);

            // Call
            TestDelegate call = () => distribution.Mean = (RoundedDouble)newMean;

            // Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(call);
            var customMessagePart = exception.Message.Split(new[]{Environment.NewLine}, StringSplitOptions.None)[0];
            Assert.AreEqual("Gemiddelde moet groter zijn dan 0.", customMessagePart);
        }

        [Test]
        [TestCase(0 + 1e-4)]
        [TestCase(156.23)]
        public void Mean_SettingValidValue_ValueIsSet(double newMean)
        {
            // Setup
            const int numberOfDecimalPlaces = 4;
            var distribution = new LognormalDistribution(numberOfDecimalPlaces);

            // Call
            distribution.Mean = (RoundedDouble)newMean;

            // Assert
            Assert.AreEqual(newMean, distribution.Mean, 1e-4);
            Assert.AreEqual(numberOfDecimalPlaces, distribution.Mean.NumberOfDecimalPlaces);
        }

        [Test]
        [TestCase(1, 1.2)]
        [TestCase(3, 1.235)]
        [TestCase(4, 1.2345)]
        [TestCase(15, 1.234500000000000)]
        public void Mean_SetNewValue_GetValueRoundedToGivenNumberOfDecimalPlaces(int numberOfDecimalPlaces, double expectedStandardDeviation)
        {
            // Setup
            var distribution = new LognormalDistribution(numberOfDecimalPlaces);

            // Call
            distribution.Mean = new RoundedDouble(4, 1.2345);

            // Assert
            Assert.AreEqual(numberOfDecimalPlaces, distribution.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(expectedStandardDeviation, distribution.Mean.Value);
        }

        [Test]
        [TestCase(0 - 1e-4)]
        [TestCase(-4)]
        public void StandardDeviation_SettingToLessThan0_ThrowArgumentOutOfRangeException(double newStd)
        {
            // Setup
            var distribution = new LognormalDistribution(4);

            // Call
            TestDelegate call = () => distribution.StandardDeviation = (RoundedDouble)newStd;

            // Assert
            ArgumentException exception = Assert.Throws<ArgumentOutOfRangeException>(call);
            string customMessagePart = exception.Message.Split(new []{Environment.NewLine}, StringSplitOptions.None)[0];
            Assert.AreEqual("Standaard afwijking (\u03C3) moet groter zijn dan of gelijk zijn aan 0.", customMessagePart);
        }

        [Test]
        [TestCase(1, 5.7)]
        [TestCase(2, 5.68)]
        [TestCase(3, 5.678)]
        [TestCase(15, 5.678000000000000)]
        public void StandardDeviation_SetNewValue_GetValueRoundedToGivenNumberOfDecimalPlaces(int numberOfDecimalPlaces, double expectedStandardDeviation)
        {
            // Setup
            var distribution = new LognormalDistribution(numberOfDecimalPlaces);

            // Call
            distribution.StandardDeviation = new RoundedDouble(3, 5.678);

            // Assert
            Assert.AreEqual(numberOfDecimalPlaces, distribution.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(expectedStandardDeviation, distribution.StandardDeviation.Value);
        }
    }
}