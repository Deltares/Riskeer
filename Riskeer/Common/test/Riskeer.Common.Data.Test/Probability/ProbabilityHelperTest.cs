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

using System;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Probability;

namespace Ringtoets.Common.Data.Test.Probability
{
    [TestFixture]
    public class ProbabilityHelperTest
    {
        [Test]
        [TestCase(0)]
        [TestCase(0.3)]
        [TestCase(1.0)]
        public void IsValidProbability_ValidProbability_ReturnTrue(double value)
        {
            // Call
            bool isValid = ProbabilityHelper.IsValidProbability(value);

            // Assert
            Assert.IsTrue(isValid);
        }

        [Test]
        public void IsValidProbability_NanIsValid_ReturnTrue()
        {
            // Call
            bool isValid = ProbabilityHelper.IsValidProbability(double.NaN, true);

            // Assert
            Assert.IsTrue(isValid);
        }

        [Test]
        [TestCase(-34.56)]
        [TestCase(-1e-6)]
        [TestCase(1.0 + 1e-6)]
        [TestCase(13.76)]
        [TestCase(double.NaN)]
        public void IsValidProbability_InvalidProbability_ReturnFalse(double value)
        {
            // Call
            bool isValid = ProbabilityHelper.IsValidProbability(value);

            // Assert
            Assert.IsFalse(isValid);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(-123.456, "A")]
        [TestCase(-1e-6, "b")]
        [TestCase(1 + 1e-6, "C")]
        [TestCase(456.789, "d")]
        [TestCase(double.NaN, "e")]
        public void ValidateProbability_InvalidProbability_ThrowsArgumentOutOfRangeException(double invalidProbabilityValue, string expectedParamName)
        {
            // Call
            TestDelegate call = () => ProbabilityHelper.ValidateProbability(invalidProbabilityValue, expectedParamName);

            // Assert
            const string message = "Kans moet in het bereik [0,0, 1,0] liggen.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, message).ParamName;
            Assert.AreEqual(expectedParamName, paramName);
        }

        [Test]
        [TestCase(0)]
        [TestCase(0.7)]
        [TestCase(1)]
        public void ValidateProbability_ValidProbability_DoesNotThrow(double probability)
        {
            // Call
            TestDelegate call = () => ProbabilityHelper.ValidateProbability(probability, "A");

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        public void ValidateProbability_AllowNaN_DoesNotThrow()
        {
            // Call
            TestDelegate call = () => ProbabilityHelper.ValidateProbability(double.NaN, "A", true);

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        public void ValidateProbability_WithInvalidCustomMessage_ThrowsArgumentException()
        {
            // Setup
            const string customMessage = "Test";

            // Call
            TestDelegate call = () => ProbabilityHelper.ValidateProbability(1.0, "value", customMessage);

            // Assert
            const string expectedMessage = "The custom message should have a insert location (\"{0}\") where the validity range is to be inserted.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage).ParamName;
            Assert.AreEqual("customMessage", paramName);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(-123.456, "A")]
        [TestCase(-1e-6, "b")]
        [TestCase(1 + 1e-6, "C")]
        [TestCase(456.789, "d")]
        [TestCase(double.NaN, "e")]
        public void ValidateProbability_WithCustomMessageAndInvalidProbability_ThrowsArgumentOutOfRangeException(double invalidProbabilityValue, string expectedParamName)
        {
            // Setup
            const string customMessage = "Test {0}";

            // Call
            TestDelegate call = () => ProbabilityHelper.ValidateProbability(invalidProbabilityValue, expectedParamName, customMessage);

            // Assert
            const string message = "Test [0,0, 1,0]";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, message).ParamName;
            Assert.AreEqual(expectedParamName, paramName);
        }

        [Test]
        [TestCase(0)]
        [TestCase(0.7)]
        [TestCase(1)]
        public void ValidateProbability_WithCustomMessageAndValidProbability_DoesNotThrow(double probability)
        {
            // Setup
            const string customMessage = "Test {0}";

            // Call
            TestDelegate call = () => ProbabilityHelper.ValidateProbability(probability, "A", customMessage);

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        public void ValidateProbability_WithCustomMessageAndAllowNaN_DoesNotThrow()
        {
            // Setup
            const string customMessage = "Test {0}";

            // Call
            TestDelegate call = () => ProbabilityHelper.ValidateProbability(double.NaN, "A", customMessage, true);

            // Assert
            Assert.DoesNotThrow(call);
        }
    }
}