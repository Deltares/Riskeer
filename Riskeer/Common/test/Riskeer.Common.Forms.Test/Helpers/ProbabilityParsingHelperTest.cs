// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
    public class ProbabilityParsingHelperTest
    {
        [Test]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("    ")]
        [TestCase(null)]
        public void Parse_NullOrEmptyString_ReturnsExpectedOutput(string value)
        {
            // Call
            double parsedValue = ProbabilityParsingHelper.Parse(value);

            // Assert
            Assert.IsNaN(parsedValue);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase("1/25     ", 0.04)]
        [TestCase("1/2,500", 0.4)]
        [TestCase("1/2.500", 0.0004)]
        [TestCase("1e-3", 0.001)]
        [TestCase("1/1.000", 0.001)]
        [TestCase("1/0", double.PositiveInfinity)]
        [TestCase("1/Oneindig", 0.0)]
        [TestCase("1/oneINdig", 0.0)]
        [TestCase("1/-1.000", -0.001)]
        [TestCase("1/-10", -0.1)]
        [TestCase("    1/-2,5", -0.4)]
        [TestCase("    -0,5", -0.5)]
        [TestCase("-1     ", -1.0)]
        [TestCase("-1e-2", -0.01)]
        public void Parse_ValidStringInDutchCulture_ReturnsExpectedOutput(string value, double expectedProbability)
        {
            // Call
            double parsedProbability = ProbabilityParsingHelper.Parse(value);

            // Assert
            Assert.AreEqual(expectedProbability, parsedProbability);
        }

        [Test]
        [SetCulture("en-US")]
        [TestCase("1/25     ", 0.04)]
        [TestCase("1/25", 0.04)]
        [TestCase("1/2.5", 0.4)]
        [TestCase("1e-3", 0.001)]
        [TestCase("1/1,000", 0.001)]
        [TestCase("1/0", double.PositiveInfinity)]
        [TestCase("1/Oneindig", 0.0)]
        [TestCase("1/oneINdig", 0.0)]
        [TestCase("1/-1,000", -0.001)]
        [TestCase("1/-10", -0.1)]
        [TestCase("    1/-2.5", -0.4)]
        [TestCase("    -0.5", -0.5)]
        [TestCase("-1     ", -1.0)]
        [TestCase("-1e-2", -0.01)]
        public void Parse_ValidStringInEnglishCulture_ReturnsExpectedOutput(string value, double expectedProbability)
        {
            // Call
            double parsedProbability = ProbabilityParsingHelper.Parse(value);

            // Assert
            Assert.AreEqual(expectedProbability, parsedProbability);
        }

        [Test]
        public void Parse_ValueDoesNotRepresentProbability_ThrowsProbabilityParsingException()
        {
            // Setup
            const string invalidValue = "I'm not a number!";

            // Call
            void Call() => ProbabilityParsingHelper.Parse(invalidValue);

            // Assert
            var exception = Assert.Throws<ProbabilityParsingException>(Call);
            Assert.IsInstanceOf<FormatException>(exception.InnerException);
            Assert.AreEqual("De waarde kon niet geïnterpreteerd worden als een kans.", exception.Message);
        }

        [Test]
        [TestCase("1")]
        [TestCase("-1")]
        public void Parse_ValueTooLargeToStoreInDouble_ThrowsProbabilityParsingException(string prefix)
        {
            // Setup
            string invalidValue = prefix + double.MaxValue.ToString(CultureInfo.CurrentCulture);

            // Call
            void Call() => ProbabilityParsingHelper.Parse(invalidValue);

            // Assert
            var exception = Assert.Throws<ProbabilityParsingException>(Call);
            Assert.IsInstanceOf<OverflowException>(exception.InnerException);
            Assert.AreEqual("De waarde is te groot of te klein.", exception.Message);
        }
    }
}