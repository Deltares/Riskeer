// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using Core.Common.Base.Data;

using NUnit.Framework;

using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.TypeConverters;
using CommonBaseResources = Core.Common.Base.Properties.Resources;

namespace Ringtoets.Common.Forms.Test.TypeConverters
{
    [TestFixture]
    public class FailureMechanismSectionResultNoProbabilityValueRoundedDoubleConverterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var converter = new FailureMechanismSectionResultNoProbabilityValueRoundedDoubleConverter();

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionResultNoValueRoundedDoubleConverter>(converter);
        }

        [Test]
        [TestCase("")]
        [TestCase("     ")]
        [TestCase("-")]
        [TestCase("NaN")]
        public void ConvertFrom_NoValueText_ReturnNaN(string text)
        {
            // Setup
            var converter = new FailureMechanismSectionResultNoProbabilityValueRoundedDoubleConverter();

            // Call
            var result = (RoundedDouble)converter.ConvertFrom(text);

            // Assert
            Assert.IsNaN(result.Value);
            Assert.AreEqual(RoundedDouble.MaximumNumberOfDecimalPlaces, result.NumberOfDecimalPlaces);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(123.4567)]
        [TestCase(-9.87654321)]
        public void ConvertFrom_SomeNumericalTextInDutchCulture_ReturnConvertedRoundedDouble(double input)
        {
            DoConvertFrom_SomeNumericalTextInCurrentCulture_ReturnConvertedRoundedDouble(input);
        }

        [Test]
        [SetCulture("en-US")]
        [TestCase(12.34)]
        [TestCase(-0.96834715)]
        public void ConvertFrom_SomeNumericalTextInEnglishCulture_ReturnConvertedRoundedDouble(double input)
        {
            DoConvertFrom_SomeNumericalTextInCurrentCulture_ReturnConvertedRoundedDouble(input);
        }

        [Test]
        public void ConvertFrom_TextDoesNotRepresentNumber_ThrowNotSupportedException()
        {
            // Setup
            string text = "I'm not a number!";

            var converter = new FailureMechanismSectionResultNoProbabilityValueRoundedDoubleConverter();

            // Call
            TestDelegate call = () => converter.ConvertFrom(null, CultureInfo.CurrentCulture, text);

            // Assert
            string message = Assert.Throws<NotSupportedException>(call).Message;
            Assert.AreEqual("De tekst moet een getal zijn.", message);
        }

        [Test]
        public void ConvertFrom_TextTooLongToStoreInDouble_ThrowNotSupportedException()
        {
            // Setup
            string text = "1" + double.MaxValue.ToString(CultureInfo.CurrentCulture);

            var converter = new FailureMechanismSectionResultNoProbabilityValueRoundedDoubleConverter();

            // Call
            TestDelegate call = () => converter.ConvertFrom(null, CultureInfo.CurrentCulture, text);

            // Assert
            string message = Assert.Throws<NotSupportedException>(call).Message;
            Assert.AreEqual("De tekst is een getal dat te groot of te klein is om gerepresenteerd te worden.", message);
        }

        [Test]
        public void CanConvertTo_ToString_ReturnTrue()
        {
            // Setup
            var converter = new FailureMechanismSectionResultNoProbabilityValueRoundedDoubleConverter();

            // Call
            bool canConvertToString = converter.CanConvertTo(typeof(string));

            // Assert
            Assert.IsTrue(canConvertToString);
        }

        [Test]
        public void ConvertTo_NaNToString_ReturnHyphen()
        {
            // Setup
            var converter = new FailureMechanismSectionResultNoProbabilityValueRoundedDoubleConverter();

            // Call
            var text = (string)converter.ConvertTo((RoundedDouble)double.NaN, typeof(string));

            // Assert
            Assert.AreEqual("-", text);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(-0.000235)]
        [TestCase(0.0069)]
        public void ConvertTo_NumberToString_ReturnStringInLocalDutchCulture(double value)
        {
            // Setup
            var roundedDouble = new RoundedDouble(8, value);
            var converter = new FailureMechanismSectionResultNoProbabilityValueRoundedDoubleConverter();

            // Call
            var text = (string)converter.ConvertTo(roundedDouble, typeof(string));

            // Assert
            string expectedText = ProbabilityFormattingHelper.Format(roundedDouble);
            Assert.AreEqual(expectedText, text);
        }

        [Test]
        [SetCulture("en-US")]
        [TestCase(-0.0000658)]
        [TestCase(0.000006788)]
        public void ConvertTo_NumberToString_ReturnStringInLocalEnglishCulture(double value)
        {
            // Setup
            var roundedDouble = new RoundedDouble(8, value);
            var converter = new FailureMechanismSectionResultNoProbabilityValueRoundedDoubleConverter();

            // Call
            var text = (string)converter.ConvertTo(roundedDouble, typeof(string));

            // Assert
            string expectedText = ProbabilityFormattingHelper.Format(roundedDouble);
            Assert.AreEqual(expectedText, text);
        }

        private static void DoConvertFrom_SomeNumericalTextInCurrentCulture_ReturnConvertedRoundedDouble(double input)
        {
            // Setup
            string text = input.ToString(CultureInfo.CurrentCulture);

            var converter = new FailureMechanismSectionResultNoProbabilityValueRoundedDoubleConverter();

            // Call
            RoundedDouble conversionResult = (RoundedDouble)converter.ConvertFrom(null, CultureInfo.CurrentCulture, text);

            // Assert
            Assert.IsNotNull(conversionResult);
            Assert.AreEqual(RoundedDouble.MaximumNumberOfDecimalPlaces, conversionResult.NumberOfDecimalPlaces);
            Assert.AreEqual(input, conversionResult.Value);
        }
    }
}