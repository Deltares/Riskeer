// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Core.Common.Base.Data;
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Exceptions;
using Riskeer.Common.Forms.Helpers;

namespace Riskeer.Common.Forms.Test.Helpers
{
    [TestFixture]
    public class RoundedDoubleParsingHelperTest
    {
        [Test]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("    ")]
        [TestCase(null)]
        public void Parse_NullOrEmptyString_ReturnsExpectedOutput(string value)
        {
            // Setup
            var random = new Random(21);

            // Call
            double parsedValue = RoundedDoubleParsingHelper.Parse(value, random.Next());

            // Assert
            Assert.IsNaN(parsedValue);
        }

        [Test]
        [TestCase("13.137,371446", 13137.371)]
        [TestCase("13,3701231", 13.370)]
        [TestCase("1,000000001", 1.0)]
        [TestCase("1e-2", 0.01)]
        [TestCase("0,003", 0.003)]
        [TestCase("-0,003", -0.003)]
        [TestCase("-1e-2", -0.01)]
        [TestCase("-1,000000001", -1.0)]
        [TestCase("-13,3701231", -13.370)]
        [TestCase("-13.137,37446", -13137.374)]
        public void Parse_ValidStringInDutchCulture_ReturnsExpectedOutput(string value, double expectedValue)
        {
            // Setup
            const int nrOfDecimals = 3;

            // Call
            RoundedDouble parsedValue = RoundedDoubleParsingHelper.Parse(value, nrOfDecimals);

            // Assert
            Assert.AreEqual(nrOfDecimals, parsedValue.NumberOfDecimalPlaces);
            Assert.AreEqual(expectedValue, parsedValue, parsedValue.GetAccuracy());
        }

        [Test]
        [SetCulture("en-US")]
        [TestCase("13,137.371446", 13137.371)]
        [TestCase("13.3701231", 13.370)]
        [TestCase("1.000000001", 1.0)]
        [TestCase("1e-2", 0.01)]
        [TestCase("0.003", 0.003)]
        [TestCase("-0.003", -0.003)]
        [TestCase("-1e-2", -0.01)]
        [TestCase("-1.000000001", -1.0)]
        [TestCase("-13.3701231", -13.370)]
        [TestCase("-13,137.37446", -13137.374)]
        public void Parse_ValidStringInEnglishCulture_ReturnsExpectedOutput(string value, double expectedValue)
        {
            // Setup
            const int nrOfDecimals = 3;

            // Call
            RoundedDouble parsedValue = RoundedDoubleParsingHelper.Parse(value, nrOfDecimals);

            // Assert
            Assert.AreEqual(nrOfDecimals, parsedValue.NumberOfDecimalPlaces);
            Assert.AreEqual(expectedValue, parsedValue, parsedValue.GetAccuracy());
        }

        [Test]
        public void Parse_ValueDoesNotRepresentRoundedDouble_ThrowsProbabilityParsingException()
        {
            // Setup
            var random = new Random(21);
            const string invalidValue = "I'm not a number!";

            // Call
            void Call() => RoundedDoubleParsingHelper.Parse(invalidValue, random.Next());

            // Assert
            var exception = Assert.Throws<RoundedDoubleParsingException>(Call);
            Assert.IsInstanceOf<FormatException>(exception.InnerException);
            Assert.AreEqual("De waarde kon niet geïnterpreteerd worden als een kommagetal.", exception.Message);
        }

        [Test]
        public void Parse_ValueTooLargeToStoreInDouble_ThrowsProbabilityParsingException()
        {
            // Setup
            var random = new Random(21);
            string invalidValue = "1" + double.MaxValue.ToString(CultureInfo.CurrentCulture);

            // Call
            void Call() => RoundedDoubleParsingHelper.Parse(invalidValue, random.Next());

            // Assert
            var exception = Assert.Throws<RoundedDoubleParsingException>(Call);
            Assert.IsInstanceOf<OverflowException>(exception.InnerException);
            Assert.AreEqual("De waarde is te groot of te klein.", exception.Message);
        }
    }
}