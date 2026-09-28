// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using Core.Common.Base.Helpers;
using NUnit.Framework;

namespace Core.Common.Base.Test.Helpers
{
    [TestFixture]
    public class DoubleHelperTest
    {
        [Test]
        public void Parse_StringValueNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => DoubleHelper.Parse(null);

            // Assert
            Assert.Throws<ArgumentNullException>(Call);
        }

        [Test]
        [TestCase("")]
        [TestCase("  ")]
        [TestCase("text")]
        public void Parse_StringValueInvalid_ThrowsFormatException(string value)
        {
            // Call
            void Call() => DoubleHelper.Parse(value);

            // Assert
            Assert.Throws<FormatException>(Call);
        }

        [Test]
        public void Parse_StringRepresentingValueLessThanMinValue_ThrowsOverflowException()
        {
            // Call
            void Call() => DoubleHelper.Parse("-1" + double.MaxValue);

            // Assert
            Assert.Throws<OverflowException>(Call);
        }

        [Test]
        public void Parse_StringRepresentingValueGreaterThanMaxValue_ThrowsOverflowException()
        {
            // Call
            void Call() => DoubleHelper.Parse("1" + double.MaxValue);

            // Assert
            Assert.Throws<OverflowException>(Call);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCaseSource(nameof(ValidTestCasesInDutchCulture))]
        public void ParseBasedOnCurrentCultureBeingDutch_ValidStringValueInDutchCulture_ReturnsExpectedOutput(string value, double expectedValue)
        {
            // Call
            double parsedValue = DoubleHelper.Parse(value);

            // Assert
            Assert.AreEqual(expectedValue, parsedValue);
        }

        [Test]
        [TestCaseSource(nameof(ValidTestCasesInDutchCulture))]
        public void ParseBasedOnProvidedCultureBeingDutch_ValidStringValueInDutchCulture_ReturnsExpectedOutput(string value, double expectedValue)
        {
            // Call
            double parsedValue = DoubleHelper.Parse(value, new CultureInfo("nl-NL"));

            // Assert
            Assert.AreEqual(expectedValue, parsedValue);
        }

        [Test]
        [SetCulture("en-US")]
        [TestCaseSource(nameof(ValidTestCasesInEnglishCulture))]
        public void ParseBasedOnCurrentCultureBeingEnglish_ValidStringValueInEnglishCulture_ReturnsExpectedOutput(string value, double expectedValue)
        {
            // Call
            double parsedValue = DoubleHelper.Parse(value);

            // Assert
            Assert.AreEqual(expectedValue, parsedValue);
        }

        [Test]
        [TestCaseSource(nameof(ValidTestCasesInEnglishCulture))]
        public void ParseBasedOnProvidedCultureBeingEnglish_ValidStringValueInEnglishCulture_ReturnsExpectedOutput(string value, double expectedValue)
        {
            // Call
            double parsedValue = DoubleHelper.Parse(value, new CultureInfo("en-US"));

            // Assert
            Assert.AreEqual(expectedValue, parsedValue);
        }

        private static IEnumerable<TestCaseData> ValidTestCasesInDutchCulture()
        {
            var formatProvider = new CultureInfo("nl-NL");

            yield return new TestCaseData("13.137,371446", 13137.371446);
            yield return new TestCaseData("13,3701231", 13.3701231);
            yield return new TestCaseData("1,000000001", 1.000000001);
            yield return new TestCaseData("1e-2", 0.01);
            yield return new TestCaseData("0,003", 0.003);
            yield return new TestCaseData("-0,003", -0.003);
            yield return new TestCaseData("-1e-2", -0.01);
            yield return new TestCaseData("-1,000000001", -1.000000001);
            yield return new TestCaseData("-13,3701231", -13.3701231);
            yield return new TestCaseData("-13.137,37446", -13137.37446);
            yield return new TestCaseData(Convert.ToString(double.MinValue, formatProvider), double.MinValue);
            yield return new TestCaseData(Convert.ToString(double.MaxValue, formatProvider), double.MaxValue);
            yield return new TestCaseData(Convert.ToString(double.PositiveInfinity, formatProvider), double.PositiveInfinity);
            yield return new TestCaseData(Convert.ToString(double.NegativeInfinity, formatProvider), double.NegativeInfinity);
        }

        private static IEnumerable<TestCaseData> ValidTestCasesInEnglishCulture()
        {
            var formatProvider = new CultureInfo("en-US");

            yield return new TestCaseData("13,137.371446", 13137.371446);
            yield return new TestCaseData("13.3701231", 13.3701231);
            yield return new TestCaseData("1.000000001", 1.000000001);
            yield return new TestCaseData("1e-2", 0.01);
            yield return new TestCaseData("0.003", 0.003);
            yield return new TestCaseData("-0.003", -0.003);
            yield return new TestCaseData("-1e-2", -0.01);
            yield return new TestCaseData("-1.000000001", -1.000000001);
            yield return new TestCaseData("-13.3701231", -13.3701231);
            yield return new TestCaseData("-13,137.37446", -13137.37446);
            yield return new TestCaseData(Convert.ToString(double.MinValue, formatProvider), double.MinValue);
            yield return new TestCaseData(Convert.ToString(double.MaxValue, formatProvider), double.MaxValue);
            yield return new TestCaseData(Convert.ToString(double.PositiveInfinity, formatProvider), double.PositiveInfinity);
            yield return new TestCaseData(Convert.ToString(double.NegativeInfinity, formatProvider), double.NegativeInfinity);
        }
    }
}