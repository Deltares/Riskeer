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

using NUnit.Framework;
using Riskeer.Storage.Core;

namespace Ringtoets.Storage.Core.Test
{
    [TestFixture]
    public class DoubleConversionExtensionsTest
    {
        [Test]
        public void ToNaNAsNull_ValueIsNaN_ReturnNull()
        {
            // Call
            double? value = double.NaN.ToNaNAsNull();

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
            double? value = original.ToNaNAsNull();

            // Assert
            Assert.IsTrue(value.HasValue);
            Assert.AreEqual(original, value.Value);
        }

        [Test]
        public void ToNullAsNaN_NullValue_ReturnNaN()
        {
            // Call
            double value = ((double?) null).ToNullAsNaN();

            // Assert
            Assert.IsNaN(value);
        }

        [Test]
        [TestCase(956.654)]
        [TestCase(-456.789)]
        [TestCase(double.MinValue)]
        [TestCase(double.MaxValue)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.NaN)]
        public void ToNullAsNaN_NotNullValue_ReturnThatValue(double value)
        {
            // Setup
            double? original = value;

            // Call
            double convertedValue = original.ToNullAsNaN();

            // Assert
            Assert.AreEqual(value, convertedValue);
        }
    }
}