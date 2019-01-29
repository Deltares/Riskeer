// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Core.Common.Base.Test.Data
{
    [TestFixture]
    public class RangeTest
    {
        [Test]
        [TestCase(0, 1)]
        [TestCase(25, 25)]
        [TestCase(int.MinValue, int.MaxValue)]
        public void Constructor_ForInt_ExpectedValues(int min, int max)
        {
            // Call
            var constructor = new Range<int>(min, max);

            // Assert
            Assert.IsInstanceOf<IFormattable>(constructor);

            Assert.AreEqual(min, constructor.Minimum);
            Assert.AreEqual(max, constructor.Maximum);
        }

        [Test]
        [TestCase(0.0, 1.0)]
        [TestCase(2.5, 2.5)]
        [TestCase(double.MinValue, double.MaxValue)]
        public void Constructor_ForDouble_ExpectedValues(double min, double max)
        {
            // Call
            var constructor = new Range<double>(min, max);

            // Assert
            Assert.IsInstanceOf<IFormattable>(constructor);

            Assert.AreEqual(min, constructor.Minimum);
            Assert.AreEqual(max, constructor.Maximum);
        }

        [Test]
        public void Constructor_MinimumGreaterThenMaximum_ThrowArgumentException()
        {
            // Call
            TestDelegate call = () => new Range<int>(1, 0);

            // Assert
            const string message = "Minimum must be smaller or equal to Maximum.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, message).ParamName;
            Assert.AreEqual("min", paramName);
        }

        [Test]
        [TestCase(0, 20, -1233, false)]
        [TestCase(0, 20, -1, false)]
        [TestCase(0, 20, 0, true)]
        [TestCase(0, 20, 8, true)]
        [TestCase(0, 20, 20, true)]
        [TestCase(0, 20, 21, false)]
        [TestCase(0, 20, 4546, false)]
        public void InRange_VariousIntScenarios_ReturnExpectedResult(int min, int max, int actual, bool expetedResult)
        {
            // Setup
            var range = new Range<int>(min, max);

            // Call
            bool result = range.InRange(actual);

            // Assert
            Assert.AreEqual(expetedResult, result);
        }

        [Test]
        [TestCase(0.0, 20.0, -123.3, false)]
        [TestCase(0.0, 20.0, -1e-6, false)]
        [TestCase(0.0, 20.0, 0.0, true)]
        [TestCase(0.0, 20.0, 8, true)]
        [TestCase(0.0, 20.0, 20.0, true)]
        [TestCase(0.0, 20.0, 20.0 + 1e-6, false)]
        [TestCase(0.0, 20.0, 454.6, false)]
        [TestCase(0.0, 20.0, double.NaN, false)]
        public void InRange_VariousDoubleScenarios_ReturnExpectedResult(double min, double max, double actual, bool expetedResult)
        {
            // Setup
            var range = new Range<double>(min, max);

            // Call
            bool result = range.InRange(actual);

            // Assert
            Assert.AreEqual(expetedResult, result);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(int.MinValue, int.MaxValue)]
        [TestCase(0, 20)]
        public void ToString_ReturnIntRangeInCurrentCulture(int min, int max)
        {
            // Setup
            var range = new Range<int>(min, max);

            // Call
            string text = range.ToString();

            // Assert
            string expectedText = $"[{min}, {max}]";
            Assert.AreEqual(expectedText, text);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(double.MinValue, double.MaxValue)]
        [TestCase(0.123, 456.789)]
        public void ToString_ReturnDoubleRangeInCurrentCulture(double min, double max)
        {
            // Setup
            var range = new Range<double>(min, max);

            // Call
            string text = range.ToString();

            // Assert
            string expectedText = $"[{min}, {max}]";
            Assert.AreEqual(expectedText, text);
        }

        [Test]
        [TestCase("nl-NL", int.MinValue, int.MaxValue)]
        [TestCase("en-US", 0, 20)]
        public void ToString_WithFormatStringAndCulture_ReturnIntRangeInGivenCulture(string cultureName, int min, int max)
        {
            // Setup
            CultureInfo cultureToUse = CultureInfo.GetCultureInfo(cultureName);
            const string formatString = "x";

            var range = new Range<int>(min, max);

            // Call
            string text = range.ToString(formatString, cultureToUse);

            // Assert
            string expectedText = $"[{min.ToString(formatString, cultureToUse)}, {max.ToString(formatString, cultureToUse)}]";
            Assert.AreEqual(expectedText, text);
        }

        [Test]
        [TestCase("nl-NL", double.MinValue, double.MaxValue)]
        [TestCase("nl-NL", 123456.789, 987789.654321)]
        [TestCase("en-US", 0.123, 456.789)]
        public void ToString_WithFormatStringCulture_ReturnDoubleRangeInGivenCulture(string cultureName, double min, double max)
        {
            // Setup
            CultureInfo cultureToUse = CultureInfo.GetCultureInfo(cultureName);
            const string formatString = "c";

            var range = new Range<double>(min, max);

            // Call
            string text = range.ToString(formatString, cultureToUse);

            // Assert
            string expectedText = $"[{min.ToString(formatString, cultureToUse)}, {max.ToString(formatString, cultureToUse)}]";
            Assert.AreEqual(expectedText, text);
        }
    }
}