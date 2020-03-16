// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace Core.Common.TestUtil.Test
{
    [TestFixture]
    public class DoubleWithToleranceComparerTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup

            // Call
            var comparer = new DoubleWithToleranceComparer(1.1);

            // Assert
            Assert.IsInstanceOf<IComparer>(comparer);
            Assert.IsInstanceOf<IComparer<double>>(comparer);
        }

        [Test]
        public void Compare_FirstObjectOfIncorrectType_ThrowArgumentException()
        {
            // Setup
            var firstObject = new object();
            object secondObject = 1.1;

            var comparer = new DoubleWithToleranceComparer(2.2);

            // Call
            TestDelegate call = () => comparer.Compare(firstObject, secondObject);

            // Assert
            string message = Assert.Throws<ArgumentException>(call).Message;
            Assert.AreEqual($"Cannot compare objects other than {typeof(double)} with this comparer.", message);
        }

        [Test]
        public void Compare_SecondObjectOfIncorrectType_ThrowArgumentException()
        {
            // Setup
            object firstObject = 2.2;
            var secondObject = new object();

            var comparer = new DoubleWithToleranceComparer(2.2);

            // Call
            TestDelegate call = () => comparer.Compare(firstObject, secondObject);

            // Assert
            string message = Assert.Throws<ArgumentException>(call).Message;
            Assert.AreEqual($"Cannot compare objects other than {typeof(double)} with this comparer.", message);
        }

        [Test]
        [TestCase(1.1, 2.2, 1.1)]
        [TestCase(1.1, 1.1, 0.0)]
        [TestCase(-2.2, 0.0, 2.2)]
        [TestCase(0.0, -1.6, 2)]
        public void Compare_ValuesWithinTolerance_ReturnZero(double first, double second, double tolerance)
        {
            // Setup
            var comparer = new DoubleWithToleranceComparer(tolerance);

            // Call
            int result = comparer.Compare(first, second);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        [Combinatorial]
        public void Compare_FirstLessThanSecond_ReturnLessThanZero(
            [Values(1.1)] double first,
            [Values(2.2 + 1e-6, 6.8)] double second,
            [Values(true, false)] bool castToObject)
        {
            // Setup
            var comparer = new DoubleWithToleranceComparer(1.1);

            // Call
            int result = castToObject ? comparer.Compare((object) first, second) : comparer.Compare(first, second);

            // Assert
            Assert.Less(result, 0);
        }

        [Test]
        [Combinatorial]
        public void Compare_FirstGreaterThanSecond_ReturnGreaterThanZero(
            [Values(1.1)] double first,
            [Values(0.6 - 1e-6, -9.65)] double second,
            [Values(true, false)] bool castToObject)
        {
            // Setup
            var comparer = new DoubleWithToleranceComparer(0.5);

            // Call
            int result = castToObject ? comparer.Compare((object) first, second) : comparer.Compare(first, second);

            // Assert
            Assert.Greater(result, 0);
        }
    }
}