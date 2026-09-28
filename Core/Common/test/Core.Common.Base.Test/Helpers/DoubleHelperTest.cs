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
using System.Xml;
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

        [Test]
        public void ConvertToDouble_ObjectValueNull_ReturnsExpectedOutput()
        {
            // Call
            double convertedValue = DoubleHelper.ConvertToDouble(null);

            // Assert
            Assert.AreEqual(0, convertedValue);
        }

        [Test]
        [TestCase("")]
        [TestCase("  ")]
        [TestCase("text")]
        public void ConvertToDouble_ObjectValueInvalid_ThrowsFormatException(string value)
        {
            // Call
            void Call() => DoubleHelper.ConvertToDouble(value);

            // Assert
            Assert.Throws<FormatException>(Call);
        }

        [Test]
        public void ConvertToDouble_ObjectValueOfIncorrectType_ThrowsInvalidCastException()
        {
            // Call
            void Call() => DoubleHelper.ConvertToDouble(new object());

            // Assert
            Assert.Throws<InvalidCastException>(Call);
        }

        [Test]
        public void ConvertToDouble_ObjectRepresentingValueLessThanMinValue_ThrowsOverflowException()
        {
            // Call
            void Call() => DoubleHelper.ConvertToDouble("-1" + double.MaxValue);

            // Assert
            Assert.Throws<OverflowException>(Call);
        }

        [Test]
        public void ConvertToDouble_ObjectRepresentingValueGreaterThanMaxValue_ThrowsOverflowException()
        {
            // Call
            void Call() => DoubleHelper.ConvertToDouble("1" + double.MaxValue);

            // Assert
            Assert.Throws<OverflowException>(Call);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCaseSource(nameof(ValidTestCasesInDutchCulture))]
        public void ConvertToDoubleBasedOnCurrentCultureBeingDutch_ValidObjectValueInDutchCulture_ReturnsExpectedOutput(string value, double expectedValue)
        {
            // Call
            double convertedValue = DoubleHelper.ConvertToDouble(value);

            // Assert
            Assert.AreEqual(expectedValue, convertedValue);
        }

        [Test]
        [TestCaseSource(nameof(ValidTestCasesInDutchCulture))]
        public void ConvertToDoubleBasedOnProvidedCultureBeingDutch_ValidObjectValueInDutchCulture_ReturnsExpectedOutput(string value, double expectedValue)
        {
            // Call
            double convertedValue = DoubleHelper.ConvertToDouble(value, new CultureInfo("nl-NL"));

            // Assert
            Assert.AreEqual(expectedValue, convertedValue);
        }

        [Test]
        [SetCulture("en-US")]
        [TestCaseSource(nameof(ValidTestCasesInEnglishCulture))]
        public void ConvertToDoubleBasedOnCurrentCultureBeingEnglish_ValidObjectValueInEnglishCulture_ReturnsExpectedOutput(string value, double expectedValue)
        {
            // Call
            double convertedValue = DoubleHelper.ConvertToDouble(value);

            // Assert
            Assert.AreEqual(expectedValue, convertedValue);
        }

        [Test]
        [TestCaseSource(nameof(ValidTestCasesInEnglishCulture))]
        public void ConvertToDoubleBasedOnProvidedCultureBeingEnglish_ValidObjectValueInEnglishCulture_ReturnsExpectedOutput(string value, double expectedValue)
        {
            // Call
            double convertedValue = DoubleHelper.ConvertToDouble(value, new CultureInfo("en-US"));

            // Assert
            Assert.AreEqual(expectedValue, convertedValue);
        }

        [Test]
        public void XmlConvertToDouble_StringValueNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => DoubleHelper.XmlConvertToDouble(null);

            // Assert
            Assert.Throws<ArgumentNullException>(Call);
        }

        [Test]
        [TestCase("")]
        [TestCase("  ")]
        [TestCase("text")]
        public void XmlConvertToDouble_StringValueInvalid_ThrowsFormatException(string value)
        {
            // Call
            void Call() => DoubleHelper.XmlConvertToDouble(value);

            // Assert
            Assert.Throws<FormatException>(Call);
        }

        [Test]
        public void XmlConvertToDouble_StringRepresentingValueLessThanMinValue_ThrowsOverflowException()
        {
            // Call
            void Call() => DoubleHelper.XmlConvertToDouble("-1" + XmlConvert.ToString(double.MaxValue));

            // Assert
            Assert.Throws<OverflowException>(Call);
        }

        [Test]
        public void XmlConvertToDouble_StringRepresentingValueGreaterThanMaxValue_ThrowsOverflowException()
        {
            // Call
            void Call() => DoubleHelper.XmlConvertToDouble("1" + XmlConvert.ToString(double.MaxValue));

            // Assert
            Assert.Throws<OverflowException>(Call);
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