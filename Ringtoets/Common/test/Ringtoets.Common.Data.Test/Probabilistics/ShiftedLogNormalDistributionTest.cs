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
using NUnit.Framework;
using Ringtoets.Common.Data.Probabilistics;

namespace Ringtoets.Common.Data.Test.Probabilistics
{
    [TestFixture]
    public class ShiftedLogNormalDistributionTest
    {
        [Test]
        [TestCase(1)]
        [TestCase(4)]
        [TestCase(15)]
        public void DefaultConstructor_ExpectedValues(int numberOfDecimalPlaces)
        {
            // Call
            var distribution = new ShiftedLogNormalDistribution(numberOfDecimalPlaces);

            // Assert
            Assert.IsInstanceOf<LogNormalDistribution>(distribution);
            Assert.IsInstanceOf<RoundedDouble>(distribution.Shift);
            Assert.AreEqual(numberOfDecimalPlaces, distribution.Shift.NumberOfDecimalPlaces);
            Assert.AreEqual(0.0, distribution.Shift.Value);
        }

        [Test]
        [TestCase(1, 5.6)]
        [TestCase(3, 5.647)]
        [TestCase(4, 5.6473)]
        [TestCase(15, 5.647300000000000)]
        public void Mean_SetNewValue_GetValueRoundedToGivenNumberOfDecimalPlaces(int numberOfDecimalPlaces, double expectedStandardDeviation)
        {
            // Setup
            var distribution = new ShiftedLogNormalDistribution(numberOfDecimalPlaces)
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
            var distribution = new ShiftedLogNormalDistribution(2)
            {
                Mean = new RoundedDouble(2, 10.0),
                StandardDeviation = new RoundedDouble(2, 1.0)
            };

            // Call
            TestDelegate call = () => distribution.Shift = new RoundedDouble(2, 100.0);

            // Assert
            string paramName = Assert.Throws<ArgumentOutOfRangeException>(call).ParamName;
            Assert.AreEqual("De verschuiving mag niet groter zijn dan de verwachtingswaarde.", paramName);
        }
    }
}