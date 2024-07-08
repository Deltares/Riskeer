// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using System.Globalization;
using Core.Common.Base.Exceptions;
using Core.Common.Base.Helpers;
using NUnit.Framework;

namespace Core.Common.Base.Test.Helpers
{
    [TestFixture]
    public class DoubleParsingHelperTest
    {
        [Test]
        [SetCulture("nl-NL")]
        [TestCase(null, 0)]
        [TestCase("13.137,371446", 13137.371446)]
        [TestCase("13,3701231", 13.3701231)]
        [TestCase("1,000000001", 1.000000001)]
        [TestCase("1e-2", 0.01)]
        [TestCase("0,003", 0.003)]
        [TestCase("-0,003", -0.003)]
        [TestCase("-1e-2", -0.01)]
        [TestCase("-1,000000001", -1.000000001)]
        [TestCase("-13,3701231", -13.3701231)]
        [TestCase("-13.137,37446", -13137.37446)]
        public void Parse_ValidStringInDutchCulture_ReturnsExpectedOutput(string value, double expectedValue)
        {
            // Call
            double parsedValue = DoubleParsingHelper.Parse(value);

            // Assert
            Assert.AreEqual(expectedValue, parsedValue);
        }

        [Test]
        [SetCulture("en-US")]
        [TestCase(null, 0)]
        [TestCase("13,137.371446", 13137.371446)]
        [TestCase("13.3701231", 13.3701231)]
        [TestCase("1.000000001", 1.000000001)]
        [TestCase("1e-2", 0.01)]
        [TestCase("0.003", 0.003)]
        [TestCase("-0.003", -0.003)]
        [TestCase("-1e-2", -0.01)]
        [TestCase("-1.000000001", -1.000000001)]
        [TestCase("-13.3701231", -13.3701231)]
        [TestCase("-13,137.37446", -13137.37446)]
        public void Parse_ValidStringInEnglishCulture_ReturnsExpectedOutput(string value, double expectedValue)
        {
            // Call
            double parsedValue = DoubleParsingHelper.Parse(value);

            // Assert
            Assert.AreEqual(expectedValue, parsedValue);
        }

        [Test]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("    ")]
        public void Parse_WhitespacesOrEmptyString_ThrowsDoubleParsingException(string value)
        {
            // Call
            void Call() => DoubleParsingHelper.Parse(value);

            // Assert
            var exception = Assert.Throws<DoubleParsingException>(Call);
            Assert.IsInstanceOf<FormatException>(exception.InnerException);
            Assert.AreEqual("De tekst mag niet leeg zijn.", exception.Message);
        }

        [Test]
        public void Parse_ValueDoesNotRepresentRoundedDouble_ThrowsDoubleParsingException()
        {
            // Setup
            const string invalidValue = "I'm not a number!";

            // Call
            void Call() => DoubleParsingHelper.Parse(invalidValue);

            // Assert
            var exception = Assert.Throws<DoubleParsingException>(Call);
            Assert.IsInstanceOf<FormatException>(exception.InnerException);
            Assert.AreEqual("De tekst moet een getal zijn.", exception.Message);
        }

        [Test]
        [TestCase("1")]
        [TestCase("-1")]
        public void Parse_ValueTooLargeToStoreInDouble_ThrowsDoubleParsingException(string prefix)
        {
            // Setup
            string invalidValue = prefix + double.MaxValue.ToString(CultureInfo.CurrentCulture);

            // Call
            void Call() => DoubleParsingHelper.Parse(invalidValue);

            // Assert
            var exception = Assert.Throws<DoubleParsingException>(Call);
            Assert.IsInstanceOf<OverflowException>(exception.InnerException);
            Assert.AreEqual("De tekst is een getal dat te groot of te klein is om gerepresenteerd te worden.", exception.Message);
        }
    }
}