// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using Core.Common.Util.Extensions;
using NUnit.Framework;

namespace Core.Common.Util.Test.Extensions
{
    [TestFixture]
    public class ComparableExtensionsTest
    {
        [Test]
        [TestCase(null, 1, false)] // Null is smaller then any "not-null"
        [TestCase(null, null, false)] // Null can be considered equal to Null
        [TestCase(1, null, true)] // Any "not-null" is greater then Null
        [TestCase(2, 1, true)]
        [TestCase(1, 1, false)]
        [TestCase(1, 2, false)]
        public void IsBigger_VariousUseCases_ReturnExpectedResult(
            IComparable first, IComparable second, bool expectedResult)
        {
            // Call
            bool isFirstBiggerThenSecond = first.IsBigger(second);

            // Assert
            Assert.AreEqual(expectedResult, isFirstBiggerThenSecond);
        }

        [Test]
        public void IsBigger_FirstObjectNotSameTypeAsSecond_ThrowArgumentException()
        {
            // Setup
            const int first = 1;
            const string second = "one";

            // Call
            TestDelegate call = () => first.IsBigger(second);

            // Assert
            Assert.Throws<ArgumentException>(call);
        }

        [Test]
        [TestCase(null, 1, true)] // Null is smaller then any "not-null"
        [TestCase(null, null, false)] // Null can be considered equal to Null
        [TestCase(1, null, false)] // Any "not-null" is greater then Null
        [TestCase(2, 1, false)]
        [TestCase(1, 1, false)]
        [TestCase(1, 2, true)]
        public void IsSmaller_VariousUseCases_ReturnExpectedResult(
            IComparable first, IComparable second, bool expectedResult)
        {
            // Call
            bool isFirstBiggerThenSecond = first.IsSmaller(second);

            // Assert
            Assert.AreEqual(expectedResult, isFirstBiggerThenSecond);
        }

        [Test]
        public void IsSmaller_FirstObjectNotSameTypeAsSecond_ThrowArgumentException()
        {
            // Setup
            const int first = 1;
            const string second = "one";

            // Call
            TestDelegate call = () => first.IsSmaller(second);

            // Assert
            Assert.Throws<ArgumentException>(call);
        }

        [Test]
        [TestCase(-5, 1, 3, false)]
        [TestCase(-5, 3, 1, false)]
        [TestCase(1 - 1e-6, 1.0, 3.0, false)]
        [TestCase(1 - 1e-6, 3.0, 1.0, false)]
        [TestCase(1, 1, 3, true)]
        [TestCase(1, 3, 1, true)]
        [TestCase(2.01, 1.0, 3.0, true)]
        [TestCase(2.01, 3.0, 1.0, true)]
        [TestCase(3, 1, 3, true)]
        [TestCase(3, 3, 1, true)]
        [TestCase(3 + 1e-6, 1.0, 3.0, false)]
        [TestCase(3 + 1e-6, 3.0, 1.0, false)]
        [TestCase(5, 1, 3, false)]
        [TestCase(5, 3, 1, false)]
        public void IsInRange_VariousUseCases_ReturnExpectedResult(
            IComparable sample, IComparable firstLimit, IComparable secondLimit, bool expectedResult)
        {
            // Call
            bool isSampleInRange = sample.IsInRange(firstLimit, secondLimit);

            // Assert
            Assert.AreEqual(expectedResult, isSampleInRange);
        }

        [Test]
        public void IsInRange_SampleObjectTypeNotSameAsFirstLimit_ThrowArgumentException()
        {
            // Setup
            const int sample = 1;
            const string firstLimit = "one";
            const int secondLimit = 2;

            // Call
            TestDelegate call = () => sample.IsInRange(firstLimit, secondLimit);

            // Assert
            Assert.Throws<ArgumentException>(call);
        }

        [Test]
        public void IsInRange_SampleObjectTypeNotSameAsSecondLimit_ThrowArgumentException()
        {
            // Setup
            const int sample = 1;
            const int firstLimit = 2;
            const string secondLimit = "one";

            // Call
            TestDelegate call = () => sample.IsInRange(firstLimit, secondLimit);

            // Assert
            Assert.Throws<ArgumentException>(call);
        }

        [Test]
        [TestCase(-4.4, -2.2, 3.3, -2.2)]
        [TestCase(-2.2, -2.2, 3.3, -2.2)]
        [TestCase(1.1, -2.2, 3.3, 1.1)]
        [TestCase(3.3, -2.2, 3.3, 3.3)]
        [TestCase(5.5, -2.2, 3.3, 3.3)]
        [TestCase(-4.4, 3.3, -2.2, -2.2)]
        [TestCase(-2.2, 3.3, -2.2, -2.2)]
        [TestCase(1.1, 3.3, -2.2, 1.1)]
        [TestCase(3.3, 3.3, -2.2, 3.3)]
        [TestCase(5.5, 3.3, -2.2, 3.3)]
        public void ClipValue_VariousTestCases_ReturnExpectedValue(double input, double limit1, double limit2, double expectedValue)
        {
            // Call
            double result = input.ClipValue(limit1, limit2);

            // Assert
            Assert.AreEqual(expectedValue, result);
        }
    }
}