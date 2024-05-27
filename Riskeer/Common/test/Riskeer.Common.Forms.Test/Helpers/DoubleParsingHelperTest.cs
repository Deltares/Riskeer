﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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

using System;
using System.Globalization;
using NUnit.Framework;
using Riskeer.Common.Forms.Exceptions;
using Riskeer.Common.Forms.Helpers;

namespace Riskeer.Common.Forms.Test.Helpers
{
    [TestFixture]
    public class DoubleParsingHelperTest
    {
        [Test]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("    ")]
        [TestCase(null)]
        public void Parse_NullOrEmptyString_ReturnsExpectedOutput(string value)
        {
            // Call
            double parsedValue = DoubleParsingHelper.Parse(value);

            // Assert
            Assert.IsNaN(parsedValue);
        }

        [Test]
        [SetCulture("nl-NL")]
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
        public void Parse_ValueDoesNotRepresentRoundedDouble_ThrowsProbabilityParsingException()
        {
            // Setup
            const string invalidValue = "I'm not a number!";

            // Call
            void Call() => DoubleParsingHelper.Parse(invalidValue);

            // Assert
            var exception = Assert.Throws<DoubleParsingException>(Call);
            Assert.IsInstanceOf<FormatException>(exception.InnerException);
            Assert.AreEqual("De waarde kon niet geïnterpreteerd worden als een kommagetal.", exception.Message);
        }

        [Test]
        public void Parse_ValueTooLargeToStoreInDouble_ThrowsProbabilityParsingException()
        {
            // Setup
            string invalidValue = "1" + double.MaxValue.ToString(CultureInfo.CurrentCulture);

            // Call
            void Call() => DoubleParsingHelper.Parse(invalidValue);

            // Assert
            var exception = Assert.Throws<DoubleParsingException>(Call);
            Assert.IsInstanceOf<OverflowException>(exception.InnerException);
            Assert.AreEqual("De waarde is te groot of te klein.", exception.Message);
        }
    }
}