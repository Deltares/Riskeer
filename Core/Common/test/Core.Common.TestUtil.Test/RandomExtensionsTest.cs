// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Linq;
using Core.Common.Base.Data;
using NUnit.Framework;

namespace Core.Common.TestUtil.Test
{
    [TestFixture]
    public class RandomExtensionsTest
    {
        [Test]
        public void NextDouble_RandomNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ((Random) null).NextDouble(0, 0);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("random", paramName);
        }

        [Test]
        public void NextDouble_LowerLimitLargerThanUpperLimit_ThrowsArgumentException()
        {
            // Setup
            var random = new Random();

            // Call
            TestDelegate test = () => random.NextDouble(1, 0);

            // Assert
            string message = Assert.Throws<ArgumentException>(test).Message;
            Assert.AreEqual("lowerLimit is larger than upperLimit", message);
        }

        [Test]
        [TestCase(double.MinValue, double.MaxValue)]
        [TestCase(double.NegativeInfinity, 0)]
        [TestCase(0, double.PositiveInfinity)]
        [TestCase(double.NaN, 0)]
        [TestCase(0, double.NaN)]
        public void NextDouble_LimitsTooLarge_ThrowsNotFiniteNumberException(double lowerLimit, double upperLimit)
        {
            // Setup
            var random = new Random();

            // Call
            TestDelegate test = () => random.NextDouble(lowerLimit, upperLimit);

            // Assert
            string message = Assert.Throws<NotFiniteNumberException>(test).Message;
            string expectedMessage = $"Creating a new random value with lower limit {lowerLimit} " +
                                     $"and upper limit {upperLimit} did not result in a finite value.";
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase(double.MinValue, 0)]
        [TestCase(0, double.MaxValue)]
        [TestCase(-10, 0)]
        [TestCase(0, 0)]
        [TestCase(0, 10)]
        public void NextDouble_VariousLimits_RandomNumberBetweenLimits(double lowerLimit, double upperLimit)
        {
            // Setup
            var random = new Random();

            // Call
            double randomValue = random.NextDouble(lowerLimit, upperLimit);

            // Assert
            Assert.LessOrEqual(randomValue, upperLimit);
            Assert.GreaterOrEqual(randomValue, lowerLimit);
        }

        [Test]
        public void NextBoolean_RandomNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => RandomExtensions.NextBoolean(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("random", paramName);
        }

        [Test]
        [TestCase(0, true)]
        [TestCase(1, false)]
        public void NextBoolean_ReturnRandomTrueOrFalse(int seed, bool expectedFirstCallResult)
        {
            // Setup
            var random = new Random(seed);

            // Call
            bool result = random.NextBoolean();

            // Assert
            Assert.AreEqual(expectedFirstCallResult, result);
        }

        [Test]
        public void NextEnumValue_RandomNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => RandomExtensions.NextEnumValue<TestEnum>(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("random", paramName);
        }

        [Test]
        public void NextEnumValue_TypeIsNoEnum_ThrowsArgumentException()
        {
            // Setup
            var random = new Random();

            // Call
            TestDelegate test = () => random.NextEnumValue<string>();

            // Assert
            Assert.Throws<ArgumentException>(test);
        }

        [Test]
        [TestCase(0, TestEnum.ValueThree)]
        [TestCase(1, TestEnum.ValueOne)]
        public void NextEnumValue_TypeIsEnum_ReturnsEnum(int seed, TestEnum expectedResult)
        {
            // Setup
            var random = new Random(seed);

            // Call
            var result = random.NextEnumValue<TestEnum>();

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void NextRoundedDouble_RandomNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => RandomExtensions.NextRoundedDouble(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("random", paramName);
        }

        [Test]
        public void NextRoundedDouble_Always_ReturnsNewRoundedDouble()
        {
            // Setup
            const int seed = 21;
            var seededRandomA = new Random(seed);
            var seededRandomB = new Random(seed);

            // Call
            RoundedDouble result = seededRandomA.NextRoundedDouble();

            // Assert
            Assert.AreEqual(seededRandomB.NextDouble(), result.Value, 1e-15);
        }

        [Test]
        public void NextEnumValueWithEnumValues_RandomNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => RandomExtensions.NextEnumValue(null, Enumerable.Empty<TestEnum>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("random", exception.ParamName);
        }

        [Test]
        public void NextEnumValueWithEnumValues_ValidEnumValuesNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            TestDelegate call = () => random.NextEnumValue<TestEnum>(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("enumValues", exception.ParamName);
        }

        [Test]
        public void NextEnumValueWithEnumValues_EnumValuesEmpty_ThrowsArgumentException()
        {
            // Setup
            var random = new Random(21);

            // Call
            TestDelegate call = () => random.NextEnumValue(Enumerable.Empty<TestEnum>());

            // Assert
            const string expectedMessage = "'enumValues' cannot be an empty collection.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void NextEnumValueWithEnumValues_TypeIsNoEnum_ThrowsArgumentException()
        {
            // Setup
            var random = new Random(21);

            // Call
            TestDelegate call = () => random.NextEnumValue(Enumerable.Empty<object>());

            // Assert
            const string expectedMessage = "'Object' is not an enum.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        [TestCase(0, TestEnum.ValueThree)]
        [TestCase(1, TestEnum.ValueTwo)]
        public void NextEnumValueWithEnumValues_WithValidArguments_ReturnsRandomEnum(int seed, TestEnum expectedResult)
        {
            // Setup
            var random = new Random(seed);
            var enumValues = new[]
            {
                TestEnum.ValueTwo,
                TestEnum.ValueThree
            };

            // Call
            object randomItem = random.NextEnumValue(enumValues);

            // Assert
            Assert.AreEqual(expectedResult, randomItem);
        }

        public enum TestEnum
        {
            ValueOne,
            ValueTwo,
            ValueThree
        }
    }
}