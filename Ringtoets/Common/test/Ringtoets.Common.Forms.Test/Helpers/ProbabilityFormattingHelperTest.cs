// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Globalization;
using NUnit.Framework;
using Ringtoets.Common.Forms.Helpers;

namespace Ringtoets.Common.Forms.Test.Helpers
{
    [TestFixture]
    public class ProbabilityFormattingHelperTest
    {
        [Test]
        public void Format_ProbabilityIsZero_ReturnOneOverInfinity()
        {
            // Call
            string text = ProbabilityFormattingHelper.Format(0);

            // Assert
            Assert.AreEqual("1/Oneindig", text);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(1.0, "1/1")]
        [TestCase(0.5, "1/2")]
        [TestCase(0.6, "1/2")]
        [TestCase(0.0001, "1/10.000")]
        [TestCase(0.000000123456789, "1/8.100.000")]
        public void Format_ProbabilityNotZero_ReturnOneOverReturnPeriod(double probability, string expectedText)
        {
            // Call
            string text = ProbabilityFormattingHelper.Format(probability);

            // Assert
            Assert.AreEqual(expectedText, text);
        }

        [Test]
        public void FormatFromReturnPeriod_ReturnPeriodIsZero_ReturnOneOverInfinity()
        {
            // Call
            string text = ProbabilityFormattingHelper.FormatFromReturnPeriod(0);

            // Assert
            Assert.AreEqual("1/Oneindig", text);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(1, "1/1")]
        [TestCase(2, "1/2")]
        [TestCase(10000, "1/10.000")]
        [TestCase(8100000, "1/8.100.000")]
        public void FormatFromReturnPeriod_ReturnPeriodNotZero_ReturnOneOverReturnPeriod(int returnPeriod, string expectedText)
        {
            // Call
            string text = ProbabilityFormattingHelper.FormatFromReturnPeriod(returnPeriod);

            // Assert
            Assert.AreEqual(expectedText, text);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase("1/10", 0.1)]
        [TestCase("1/2,5", 0.4)]
        [TestCase("0,5", 0.5)]
        [TestCase("1", 1.0)]
        [TestCase("1e-2", 0.01)]
        public void Parse_WithValidInput_ReturnsCorrectProbability(string input, double expectedProbability)
        {
            // Call
            double probability = ProbabilityFormattingHelper.Parse(input);

            // Assert
            Assert.AreEqual(expectedProbability, probability);
        }

        [Test]
        [TestCase("not a double")]
        [TestCase("")]
        [TestCase("1/aaa")]
        [TestCase("1/")]
        [TestCase(".")]
        public void Parse_WithInvalidInput_ThrowsFormatException(string input)
        {
            // Call
            TestDelegate call = () => ProbabilityFormattingHelper.Parse(input);

            // Assert
            Assert.Throws<FormatException>(call);
        }

        [Test]
        [TestCase(double.MinValue)]
        [TestCase(double.MaxValue)]
        public void Parse_WithOutOfRangeInput_ThrowsOverflowException(double input)
        {
            // Call
            TestDelegate call = () => ProbabilityFormattingHelper.Parse(input.ToString(CultureInfo.CurrentCulture) + "1");

            // Assert
            Assert.Throws<OverflowException>(call);
        }
    }
}