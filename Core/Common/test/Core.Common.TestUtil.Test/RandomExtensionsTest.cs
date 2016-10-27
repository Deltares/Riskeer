// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using NUnit.Framework;

namespace Core.Common.TestUtil.Test
{
    [TestFixture]
    public class RandomExtensionsTest
    {
        [Test]
        public void GetFromRange_RandomIsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ((Random) null).GetFromRange(0, 0);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("random", paramName);
        }

        [Test]
        public void GetFromRange_LowerLimitLargerThanUpperLimit_ThrowsArgumentException()
        {
            // Setup
            var random = new Random();

            // Call
            TestDelegate test = () => random.GetFromRange(1, 0);

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
        public void GetFromRange_LimitsTooLarge_ThrowsNotFiniteNumberException(double lowerLimit, double upperLimit)
        {
            // Setup
            var random = new Random();

            // Call
            TestDelegate test = () => random.GetFromRange(lowerLimit, upperLimit);

            // Assert
            string message = Assert.Throws<NotFiniteNumberException>(test).Message;
            string expectedMessage = string.Format("Creating a new random value with lower limit {0} " +
                                                   "and upper limit {1} did not result in a finite value.",
                                                   lowerLimit, upperLimit);
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase(double.MinValue, 0)]
        [TestCase(0, double.MaxValue)]
        [TestCase(-10, 0)]
        [TestCase(0, 0)]
        [TestCase(0, 10)]
        [TestCase(0, 10)]
        public void GetFromRange_VariousLimits_RandomNumberBetweenLimits(double lowerLimit, double upperLimit)
        {
            // Setup
            var random = new Random();

            // Call
            var randomValue = random.GetFromRange(lowerLimit, upperLimit);

            // Assert
            Assert.LessOrEqual(randomValue, upperLimit);
            Assert.GreaterOrEqual(randomValue, lowerLimit);
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
    }
}