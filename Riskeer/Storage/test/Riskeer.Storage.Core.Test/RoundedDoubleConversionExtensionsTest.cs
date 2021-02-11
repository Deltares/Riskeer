﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using Core.Common.Base.Data;
using NUnit.Framework;

namespace Riskeer.Storage.Core.Test
{
    [TestFixture]
    public class RoundedDoubleConversionExtensionsTest
    {
        [Test]
        public void ToNaNAsNull_ValueIsNaN_ReturnNull()
        {
            // Call
            RoundedDouble? value = RoundedDouble.NaN.ToNaNAsNull();

            // Assert
            Assert.IsNull(value);
        }

        [Test]
        [TestCase(1.1)]
        [TestCase(-2.2)]
        [TestCase(double.MinValue)]
        [TestCase(double.MaxValue)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        public void ToNaNAsNull_NotNaNValue_ReturnThatValue(double original)
        {
            // Call
            var originalRoundedDouble = (RoundedDouble) original;
            RoundedDouble? value = originalRoundedDouble.ToNaNAsNull();

            // Assert
            Assert.IsTrue(value.HasValue);
            Assert.AreEqual(originalRoundedDouble, value.Value);
        }
    }
}