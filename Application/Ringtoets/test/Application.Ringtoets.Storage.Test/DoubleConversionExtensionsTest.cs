// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test
{
    [TestFixture]
    public class DoubleConversionExtensionsTest
    {
        [Test]
        public void ToNullableDecimal_NaN_ReturnNull()
        {
            // Call
            decimal? result = double.NaN.ToNullableDecimal();

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        [TestCase(-154.516)]
        [TestCase(9684.51)]
        [TestCase(0.0)]
        public void ToNullableDecimal_Number_ReturnThatNumberAsDecimal(double value)
        {
            // Call
            decimal? result = value.ToNullableDecimal();

            // Assert
            Assert.AreEqual(value, Convert.ToDouble(result), 1e-6);
        }

        [Test]
        public void ToNullableDecimal_Null_ReturnNull()
        {
            // Call
            decimal? result = ((double?)null).ToNullableDecimal();

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.MaxValue)]
        [TestCase(double.MinValue)]
        public void ToNullableDecimal_NullableSpecialDoubleValues_ThrowsOverflowException(double specialDoubleValue)
        {
            // Call
            TestDelegate test = () => ((double?)specialDoubleValue).ToNullableDecimal();

            // Assert
            Assert.Throws<OverflowException>(test);
        }

        [Test]
        public void ToNullableDecimal_NullableEpsilon_ReturnsZeroDecimal()
        {
            // Call
            decimal? value = ((double?)double.Epsilon).ToNullableDecimal();

            // Assert
            Assert.AreEqual(decimal.Zero, value);
        }

        [Test]
        [TestCase(-12312.352)]
        [TestCase(51516.351)]
        [TestCase(0.0)]
        public void ToNullableDecimal_NullableNumber_ReturnThatNumberAsDecimal(double value)
        {
            // Call
            decimal? result = ((double?)value).ToNullableDecimal();

            // Assert
            Assert.AreEqual(value, Convert.ToDouble(result), 1e-6);
        }
    }
}