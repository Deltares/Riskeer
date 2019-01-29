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
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.TypeConverters;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Core.Common.Base.Test.Data
{
    [TestFixture]
    public class RoundedDoubleTest
    {
        [Test]
        [TestCase(0)]
        [TestCase(12)]
        [TestCase(15)]
        public void Constructor_WithoutValue_ExpectedValues(int numberOfDecimalPlaces)
        {
            // Call
            var roundedDouble = new RoundedDouble(numberOfDecimalPlaces);

            // Assert
            Assert.IsInstanceOf<IEquatable<RoundedDouble>>(roundedDouble);
            Assert.IsInstanceOf<IEquatable<double>>(roundedDouble);
            Assert.IsInstanceOf<IFormattable>(roundedDouble);
            Assert.IsInstanceOf<IComparable>(roundedDouble);
            Assert.IsInstanceOf<IComparable<RoundedDouble>>(roundedDouble);
            Assert.IsInstanceOf<IComparable<double>>(roundedDouble);
            Assert.AreEqual(numberOfDecimalPlaces, roundedDouble.NumberOfDecimalPlaces);
            Assert.AreEqual(0.0, roundedDouble.Value);

            TypeConverterAttribute[] typeAttributes = roundedDouble.GetType().GetCustomAttributes(typeof(TypeConverterAttribute), false)
                                                                   .Cast<TypeConverterAttribute>()
                                                                   .ToArray();
            Assert.AreEqual(1, typeAttributes.Length);
            Assert.AreEqual(new TypeConverterAttribute(typeof(RoundedDoubleConverter)), typeAttributes[0]);
        }

        [Test]
        [TestCase(1.0, 2, 1.00)]
        [TestCase(123456789.0, 3, 123456789.000)]
        [TestCase(12345678.90, 2, 12345678.90)]
        [TestCase(12345678.90, 3, 12345678.900)]
        [TestCase(1234567.890, 2, 1234567.89)]
        [TestCase(1234567.890, 3, 1234567.890)]
        [TestCase(123456.7890, 2, 123456.79)]
        [TestCase(123456.7890, 3, 123456.789)]
        [TestCase(12345.67890, 2, 12345.68)]
        [TestCase(12345.67890, 3, 12345.679)]
        [TestCase(1234.567890, 2, 1234.57)]
        [TestCase(1234.567890, 3, 1234.568)]
        [TestCase(123.4567890, 2, 123.46)]
        [TestCase(123.4567890, 3, 123.457)]
        [TestCase(12.34567890, 2, 12.35)]
        [TestCase(12.34567890, 3, 12.346)]
        [TestCase(1.234567890, 2, 1.23)]
        [TestCase(1.234567890, 3, 1.235)]
        [TestCase(0.1234567890, 2, 0.12)]
        [TestCase(0.1234567890, 3, 0.123)]
        [TestCase(0.01234567890, 2, 0.01)]
        [TestCase(0.01234567890, 3, 0.012)]
        [TestCase(0.001234567890, 2, 0.00)]
        [TestCase(0.001234567890, 3, 0.001)]
        [TestCase(0.0001234567890, 2, 0.00)]
        [TestCase(0.0001234567890, 3, 0.000)]
        [TestCase(double.NaN, 2, double.NaN)]
        [TestCase(double.PositiveInfinity, 4, double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity, 3, double.NegativeInfinity)]
        public void Constructor_ExpectedValues(double doubleValue, int numberOfDecimalPlaces, double expectedRoundedValue)
        {
            // Call
            var roundedDouble = new RoundedDouble(numberOfDecimalPlaces, doubleValue);

            // Assert
            Assert.IsInstanceOf<IEquatable<RoundedDouble>>(roundedDouble);
            Assert.IsInstanceOf<IEquatable<double>>(roundedDouble);
            Assert.AreEqual(numberOfDecimalPlaces, roundedDouble.NumberOfDecimalPlaces);
            Assert.AreEqual(expectedRoundedValue, roundedDouble.Value);
        }

        [Test]
        [TestCase(-45678)]
        [TestCase(-1)]
        [TestCase(16)]
        [TestCase(34567)]
        public void Constructor_InvalidNumberOfPlaces_ThrowArgumentOutOfRangeException(int invalidNumberOfPlaces)
        {
            // Call
            TestDelegate call = () => new RoundedDouble(invalidNumberOfPlaces);

            // Assert
            const string expectedMessage = "Value must be in range [0, 15].";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        public void NaN_ExpectedValue()
        {
            // Assert
            Assert.IsNaN(RoundedDouble.NaN.Value);
            Assert.AreEqual(RoundedDouble.MaximumNumberOfDecimalPlaces, RoundedDouble.NaN.NumberOfDecimalPlaces);
        }

        [Test]
        [TestCase(1.0, 2, 1.00)]
        [TestCase(123456789.0, 3, 123456789.000)]
        [TestCase(12345678.90, 2, 12345678.90)]
        [TestCase(12345678.90, 3, 12345678.900)]
        [TestCase(1234567.890, 2, 1234567.89)]
        [TestCase(1234567.890, 3, 1234567.890)]
        [TestCase(123456.7890, 2, 123456.79)]
        [TestCase(123456.7890, 3, 123456.789)]
        [TestCase(12345.67890, 2, 12345.68)]
        [TestCase(12345.67890, 3, 12345.679)]
        [TestCase(1234.567890, 2, 1234.57)]
        [TestCase(1234.567890, 3, 1234.568)]
        [TestCase(123.4567890, 2, 123.46)]
        [TestCase(123.4567890, 3, 123.457)]
        [TestCase(12.34567890, 2, 12.35)]
        [TestCase(12.34567890, 3, 12.346)]
        [TestCase(1.234567890, 2, 1.23)]
        [TestCase(1.234567890, 3, 1.235)]
        [TestCase(0.1234567890, 2, 0.12)]
        [TestCase(0.1234567890, 3, 0.123)]
        [TestCase(0.01234567890, 2, 0.01)]
        [TestCase(0.01234567890, 3, 0.012)]
        [TestCase(0.001234567890, 2, 0.00)]
        [TestCase(0.001234567890, 3, 0.001)]
        [TestCase(0.0001234567890, 2, 0.00)]
        [TestCase(0.0001234567890, 3, 0.000)]
        [TestCase(double.NaN, 2, double.NaN)]
        [TestCase(double.PositiveInfinity, 4, double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity, 3, double.NegativeInfinity)]
        public void Value_SetNewValue_RoundValue(
            double value, int numberOfDecimals, double expectedReturnValue)
        {
            // Setup
            var roundedValue = new RoundedDouble(numberOfDecimals, value);

            // Call
            double returnValue = roundedValue.Value;

            // Assert
            Assert.AreEqual(expectedReturnValue, returnValue);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(1.0, 2, "1,00")]
        [TestCase(123456789.0, 3, "123456789,000")]
        [TestCase(12345678.90, 2, "12345678,90")]
        [TestCase(12345678.90, 3, "12345678,900")]
        [TestCase(1234567.890, 2, "1234567,89")]
        [TestCase(1234567.890, 3, "1234567,890")]
        [TestCase(123456.7890, 2, "123456,79")]
        [TestCase(123456.7890, 3, "123456,789")]
        [TestCase(12345.67890, 2, "12345,68")]
        [TestCase(12345.67890, 3, "12345,679")]
        [TestCase(1234.567890, 2, "1234,57")]
        [TestCase(1234.567890, 3, "1234,568")]
        [TestCase(123.4567890, 2, "123,46")]
        [TestCase(123.4567890, 3, "123,457")]
        [TestCase(12.34567890, 2, "12,35")]
        [TestCase(12.34567890, 3, "12,346")]
        [TestCase(1.234567890, 2, "1,23")]
        [TestCase(1.234567890, 3, "1,235")]
        [TestCase(0.1234567890, 2, "0,12")]
        [TestCase(0.1234567890, 3, "0,123")]
        [TestCase(0.01234567890, 2, "0,01")]
        [TestCase(0.01234567890, 3, "0,012")]
        [TestCase(0.001234567890, 2, "0,00")]
        [TestCase(0.001234567890, 3, "0,001")]
        [TestCase(0.0001234567890, 2, "0,00")]
        [TestCase(0.0001234567890, 3, "0,000")]
        [TestCase(double.NaN, 2, "NaN")]
        [TestCase(double.NegativeInfinity, 2, "-Oneindig")]
        [TestCase(double.PositiveInfinity, 2, "Oneindig")]
        public void ToString_VariousScenarios_ExpectedText(
            double value, int numberOfDecimals, string expectedText)
        {
            // Setup
            var roundedValue = new RoundedDouble(numberOfDecimals, value);

            // Call
            string text = roundedValue.ToString();

            // Assert
            Assert.AreEqual(expectedText, text);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase("N", 1.0, 2, "1,00")]
        [TestCase("N0", 123456789.0, 3, "123.456.789")]
        [TestCase("N1", 12345678.90, 2, "12.345.678,9")]
        [TestCase("N1", 12345678.90, 3, "12.345.678,9")]
        [TestCase("G", 1234567.890, 2, "1234567,89")]
        [TestCase("G3", 1234567.890, 3, "1,23E+06")]
        [TestCase("G9", 123456.7890, 2, "123456,79")]
        [TestCase("F1", 0.1234567890, 2, "0,1")]
        [TestCase("F3", 0.1234567890, 3, "0,123")]
        [TestCase("F3", 0.01234567890, 2, "0,010")]
        [TestCase("F5", 0.01234567890, 3, "0,01200")]
        [TestCase("F2", 0.001234567890, 2, "0,00")]
        [TestCase("N0", 0.0001234567890, 2, "0")]
        [TestCase("N1", 0.0001234567890, 3, "0,0")]
        [TestCase("N", double.NaN, 2, "NaN")]
        [TestCase("F7", double.NegativeInfinity, 2, "-Oneindig")]
        [TestCase("G3", double.PositiveInfinity, 2, "Oneindig")]
        public void ToString_WithFormatAndCurrentCultureVariousScenarios_ExpectedText(string format,
                                                                                      double value, int numberOfDecimals, string expectedText)
        {
            // Setup
            var roundedValue = new RoundedDouble(numberOfDecimals, value);

            // Call
            string text = roundedValue.ToString(format, null);

            // Assert
            Assert.AreEqual(expectedText, text);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase("N", 1.0, 2, "1.00")]
        [TestCase("N0", 123456789.0, 3, "123,456,789")]
        [TestCase("N1", 12345678.90, 2, "12,345,678.9")]
        [TestCase("N1", 12345678.90, 3, "12,345,678.9")]
        [TestCase("G", 1234567.890, 2, "1234567.89")]
        [TestCase("G3", 1234567.890, 3, "1.23E+06")]
        [TestCase("G9", 123456.7890, 2, "123456.79")]
        [TestCase("F1", 0.1234567890, 2, "0.1")]
        [TestCase("F3", 0.1234567890, 3, "0.123")]
        [TestCase("F3", 0.01234567890, 2, "0.010")]
        [TestCase("F5", 0.01234567890, 3, "0.01200")]
        [TestCase("F2", 0.001234567890, 2, "0.00")]
        [TestCase("N0", 0.0001234567890, 2, "0")]
        [TestCase("N1", 0.0001234567890, 3, "0.0")]
        [TestCase("N", double.NaN, 2, "NaN")]
        [TestCase("F7", double.NegativeInfinity, 2, "-Oneindig")]
        [TestCase("G3", double.PositiveInfinity, 2, "Oneindig")]
        public void ToString_WithFormatAndDifferentCultureVariousScenarios_ExpectedText(string format,
                                                                                        double value, int numberOfDecimals, string expectedText)
        {
            // Setup
            var roundedValue = new RoundedDouble(numberOfDecimals, value);

            // Call
            string text = roundedValue.ToString(format, CultureInfo.GetCultureInfo("en-GB"));

            // Assert
            Assert.AreEqual(expectedText, text);
        }

        [Test]
        public void Equals_ToNull_ReturnFalse()
        {
            // Setup
            var roundedDouble = new RoundedDouble(2);

            // Call
            bool isEqual = roundedDouble.Equals(null);

            // Assert
            Assert.IsFalse(isEqual);
        }

        [Test]
        public void Equals_ToSameInstance_ReturnTrue()
        {
            // Setup
            var roundedDouble = new RoundedDouble(5);

            // Call
            bool isEqual = roundedDouble.Equals(roundedDouble);

            // Assert
            Assert.IsTrue(isEqual);
        }

        [Test]
        public void Equals_ToSameObjectInstance_ReturnTrue()
        {
            // Setup
            var roundedDouble = new RoundedDouble(5);

            // Call
            bool isEqual = roundedDouble.Equals((object) roundedDouble);

            // Assert
            Assert.IsTrue(isEqual);
        }

        [Test]
        public void Equals_ObjectOfSomeOtherType_ReturnFalse()
        {
            // Setup
            var roundedDouble = new RoundedDouble(1);
            var someOtherObject = new object();

            // Call
            bool isEqual1 = roundedDouble.Equals(someOtherObject);
            bool isEqual2 = someOtherObject.Equals(roundedDouble);

            // Assert
            Assert.IsFalse(isEqual1);
            Assert.IsFalse(isEqual2);
        }

        [Test]
        [TestCase(3, 1.2344444)]
        [TestCase(4, 1.2340444)]
        [TestCase(7, 1.2340000)]
        public void Equals_ToOtherEqualRoundedDouble_ReturnTrue(
            int numberOfPlaces, double value)
        {
            // Setup
            var baseRoundedDouble = new RoundedDouble(3, 1.234);
            object comparisonRoundedDouble = new RoundedDouble(numberOfPlaces, value);

            // Call
            bool isEqual1 = baseRoundedDouble.Equals(comparisonRoundedDouble);
            bool isEqual2 = comparisonRoundedDouble.Equals(baseRoundedDouble);

            // Assert
            Assert.IsTrue(isEqual1);
            Assert.IsTrue(isEqual2);
        }

        [Test]
        [TestCase(3, 1.2344444)]
        [TestCase(4, 1.2340444)]
        [TestCase(7, 1.2340000)]
        public void GetHashCode_TwoEqualInstances_ReturnSameHash(
            int numberOfPlaces, double value)
        {
            // Setup
            var baseRoundedDouble = new RoundedDouble(3, 1.234);
            object comparisonRoundedDouble = new RoundedDouble(numberOfPlaces, value);

            // Call
            int hash1 = baseRoundedDouble.GetHashCode();
            int hash2 = comparisonRoundedDouble.GetHashCode();

            // Assert
            Assert.AreEqual(hash1, hash2);
        }

        [Test]
        public void EqualityOperator_TwoUnequalRoundedValues_ReturnFalse()
        {
            // Setup
            var roundedDouble1 = new RoundedDouble(2, 1.23);
            var roundedDouble2 = new RoundedDouble(1, 1.2);

            // Call
            bool isEqual1 = roundedDouble1 == roundedDouble2;
            bool isEqual2 = roundedDouble2 == roundedDouble1;

            // Assert
            Assert.IsFalse(isEqual1);
            Assert.IsFalse(isEqual2);
        }

        [Test]
        public void EqualityOperator_TwoEqualRoundedValues_ReturnFalse()
        {
            // Setup
            var roundedDouble1 = new RoundedDouble(2, 1.20);
            var roundedDouble2 = new RoundedDouble(1, 1.2);

            // Call
            bool isEqual1 = roundedDouble1 == roundedDouble2;
            bool isEqual2 = roundedDouble2 == roundedDouble1;

            // Assert
            Assert.IsTrue(isEqual1);
            Assert.IsTrue(isEqual2);
        }

        [Test]
        public void InequalityOperator_TwoUnequalRoundedValues_ReturnTrue()
        {
            // Setup
            var roundedDouble1 = new RoundedDouble(2, 1.23);
            var roundedDouble2 = new RoundedDouble(1, 1.2);

            // Precondition:
            Assert.IsFalse(roundedDouble1.Equals(roundedDouble2));

            // Call
            bool isNotEqual1 = roundedDouble1 != roundedDouble2;
            bool isNotEqual2 = roundedDouble2 != roundedDouble1;

            // Assert
            Assert.IsTrue(isNotEqual1);
            Assert.IsTrue(isNotEqual2);
        }

        [Test]
        public void InequalityOperator_TwoEqualRoundedValues_ReturnFalse()
        {
            // Setup
            var roundedDouble1 = new RoundedDouble(2, 1.20);
            var roundedDouble2 = new RoundedDouble(1, 1.2);

            // Precondition:
            Assert.IsTrue(roundedDouble1.Equals(roundedDouble2));

            // Call
            bool isNotEqual1 = roundedDouble1 != roundedDouble2;
            bool isNotEqual2 = roundedDouble2 != roundedDouble1;

            // Assert
            Assert.IsFalse(isNotEqual1);
            Assert.IsFalse(isNotEqual2);
        }

        [Test]
        [TestCase(987654321.0, 5)]
        [TestCase(-9876543.1200, 2)]
        public void Equals_RoundedDoubleEqualToDouble_ReturnTrue(
            double value, int numberOfDecimalPlaces)
        {
            // Setup
            var roundedDouble = new RoundedDouble(numberOfDecimalPlaces, value);

            // Call
            bool isEqual1 = roundedDouble.Equals(value);
            bool isEqual2 = value.Equals(roundedDouble);

            // Assert
            Assert.IsTrue(isEqual1);
            Assert.IsTrue(isEqual2);
        }

        [Test]
        [TestCase(987654321.1, 0)]
        [TestCase(-9876543.1234, 2)]
        public void Equals_RoundedDoubleNotEqualToDouble_ReturnFalse(
            double value, int numberOfDecimalPlaces)
        {
            // Setup
            var roundedDouble = new RoundedDouble(numberOfDecimalPlaces, value);

            // Call
            bool isEqual1 = roundedDouble.Equals(value);
            bool isEqual2 = value.Equals(roundedDouble);

            // Assert
            Assert.IsFalse(isEqual1);
            Assert.IsFalse(isEqual2);
        }

        [Test]
        public void Equals_RoundedDoubleTotallyDifferentFromDouble_ReturnFalse()
        {
            // Setup
            var roundedDouble = new RoundedDouble(2, 1.23);
            const double otherValue = 4.56;

            // Call
            bool isEqual1 = roundedDouble.Equals(otherValue);
            bool isEqual2 = otherValue.Equals(roundedDouble);

            // Assert
            Assert.IsFalse(isEqual1);
            Assert.IsFalse(isEqual2);
        }

        [Test]
        public void GetHashCode_RoundedDoubleEqualToDouble_ReturnSameHashCode()
        {
            // Setup
            const double otherValue = 4.56;
            var roundedDouble = new RoundedDouble(2, otherValue);

            // Precondition:
            Assert.IsTrue(otherValue.Equals(roundedDouble));

            // Call
            int hash1 = roundedDouble.GetHashCode();
            int hash2 = otherValue.GetHashCode();

            // Assert
            Assert.AreEqual(hash1, hash2);
        }

        [Test]
        public void DoubleEqualityOperator_DoubleIsEqualToRoundedDouble_ReturnTrue()
        {
            // Setup
            const double value = 1.234;
            var roundedDouble = new RoundedDouble(4, value);

            // Precondition
            Assert.IsTrue(roundedDouble.Equals(value));

            // Call
            bool isEqual1 = value == roundedDouble;
            bool isEqual2 = roundedDouble == value;

            // Assert
            Assert.IsTrue(isEqual1);
            Assert.IsTrue(isEqual2);
        }

        [Test]
        public void DoubleEqualityOperator_DoubleIsNotEqualToRoundedDouble_ReturnFalse()
        {
            // Setup
            const double value = 1.234;
            var roundedDouble = new RoundedDouble(4, 3.21543);

            // Precondition
            Assert.IsFalse(roundedDouble.Equals(value));

            // Call
            bool isEqual1 = value == roundedDouble;
            bool isEqual2 = roundedDouble == value;

            // Assert
            Assert.IsFalse(isEqual1);
            Assert.IsFalse(isEqual2);
        }

        [Test]
        public void DoubleInequalityOperator_DoubleIsEqualToRoundedDouble_ReturnFalse()
        {
            // Setup
            const double value = 1.234;
            var roundedDouble = new RoundedDouble(4, value);

            // Precondition
            Assert.IsTrue(roundedDouble.Equals(value));

            // Call
            bool isEqual1 = value != roundedDouble;
            bool isEqual2 = roundedDouble != value;

            // Assert
            Assert.IsFalse(isEqual1);
            Assert.IsFalse(isEqual2);
        }

        [Test]
        public void DoubleInequalityOperator_DoubleIsNotEqualToRoundedDouble_ReturnTrue()
        {
            // Setup
            const double value = 1.234;
            var roundedDouble = new RoundedDouble(4, 3.21543);

            // Precondition
            Assert.IsFalse(roundedDouble.Equals(value));

            // Call
            bool isEqual1 = value != roundedDouble;
            bool isEqual2 = roundedDouble != value;

            // Assert
            Assert.IsTrue(isEqual1);
            Assert.IsTrue(isEqual2);
        }

        [Test]
        public void ImplicitConversion_FromRoundedDoubleToDouble_ConvertedValueIsEqual()
        {
            // Setup
            var roundedDouble = new RoundedDouble(4, 3.2154);

            // Call
            double convertedValue = roundedDouble;

            // Assert
            Assert.AreEqual(roundedDouble.Value, convertedValue);
        }

        [Test]
        public void ExplicitConversion_FromDoubleToRoundedDouble_ConvertedValueIsEqual()
        {
            // Setup
            const double doubleValue = 1.23456789;

            // Call
            var roundedDoubleValue = (RoundedDouble) doubleValue;

            // Assert
            Assert.AreEqual(doubleValue, roundedDoubleValue.Value);
            Assert.AreEqual(15, roundedDoubleValue.NumberOfDecimalPlaces);
        }

        [Test]
        [TestCase(0, 1d)]
        [TestCase(2, 1.24)]
        [TestCase(3, 1.236)]
        [TestCase(15, 1.236000000000000)]
        public void ToPrecision_VariousScenarios_ReturnRoundedDouble(int newPrecision, double expectedValue)
        {
            // Setup
            var original = new RoundedDouble(3, 1.236);

            // Call
            RoundedDouble convertedResult = original.ToPrecision(newPrecision);

            // Assert
            Assert.AreEqual(newPrecision, convertedResult.NumberOfDecimalPlaces);
            Assert.AreEqual(expectedValue, convertedResult.Value);
        }

        [Test]
        public void OperatorMinus_LeftHasLowestPrecision_ReturnRoundedDoubleWithDifferenceRoundedToLeastNumberOfDecimalPlaces()
        {
            // Setup
            const int lowestNumberOfDecimalPlaces = 2;

            var value1 = new RoundedDouble(lowestNumberOfDecimalPlaces, 1.12);
            var value2 = new RoundedDouble(3, 3.456);

            // Call
            RoundedDouble diff = value1 - value2;

            // Assert
            Assert.AreEqual(lowestNumberOfDecimalPlaces, diff.NumberOfDecimalPlaces);
            Assert.AreEqual(-2.34, diff.Value);
        }

        [Test]
        public void OperatorMinus_RightHasLowestPrecision_ReturnRoundedDoubleWithDifferenceRoundedToLeastNumberOfDecimalPlaces()
        {
            // Setup
            const int lowestNumberOfDecimalPlaces = 1;

            var value1 = new RoundedDouble(6, 1.123456);
            var value2 = new RoundedDouble(lowestNumberOfDecimalPlaces, -7.8);

            // Call
            RoundedDouble diff = value1 - value2;

            // Assert
            Assert.AreEqual(lowestNumberOfDecimalPlaces, diff.NumberOfDecimalPlaces);
            Assert.AreEqual(8.9, diff.Value);
        }

        [Test]
        public void OperatorPlus_LeftHasLowestPrecision_ReturnRoundedDoubleWithSumRoundedToLeastNumberOfDecimalPlaces()
        {
            // Setup
            const int lowestNumberOfDecimalPlaces = 2;

            var value1 = new RoundedDouble(lowestNumberOfDecimalPlaces, 1.12);
            var value2 = new RoundedDouble(3, 3.456);

            // Call
            RoundedDouble diff = value1 + value2;

            // Assert
            Assert.AreEqual(lowestNumberOfDecimalPlaces, diff.NumberOfDecimalPlaces);
            Assert.AreEqual(4.58, diff.Value);
        }

        [Test]
        public void OperatorPlus_RightHasLowestPrecision_ReturnRoundedDoubleWithSumRoundedToLeastNumberOfDecimalPlaces()
        {
            // Setup
            const int lowestNumberOfDecimalPlaces = 1;

            var value1 = new RoundedDouble(6, 1.123456);
            var value2 = new RoundedDouble(lowestNumberOfDecimalPlaces, -7.8);

            // Call
            RoundedDouble diff = value1 + value2;

            // Assert
            Assert.AreEqual(lowestNumberOfDecimalPlaces, diff.NumberOfDecimalPlaces);
            Assert.AreEqual(-6.7, diff.Value);
        }

        [Test]
        public void OperatorTimes_RoundedDoubleTimesDouble_ReturnResultAsRoundedDoublePreservingNumberOfDecimalPlaces()
        {
            // Setup
            var roundedDouble = new RoundedDouble(3, 1.234);
            const double doubleValue = 5.67891234;

            // Call
            RoundedDouble result1 = roundedDouble * doubleValue;
            RoundedDouble result2 = doubleValue * roundedDouble;

            // Assert
            Assert.AreEqual(roundedDouble.NumberOfDecimalPlaces, result1.NumberOfDecimalPlaces);
            Assert.AreEqual(roundedDouble.NumberOfDecimalPlaces, result2.NumberOfDecimalPlaces);
            const double expectedValue = 7.008;
            Assert.AreEqual(expectedValue, result1.Value);
            Assert.AreEqual(expectedValue, result2.Value);
        }

        [Test]
        public void OperatorTimes_LeftHasLeastPrecision_ResultIsRoundedDoubleWithLeastNumberOfDecimalPlaces()
        {
            // Setup
            var roundedDouble1 = new RoundedDouble(2, 1.23);
            var roundedDouble2 = new RoundedDouble(5, -3.45678);

            // Call
            RoundedDouble result = roundedDouble1 * roundedDouble2;

            // Assert
            Assert.AreEqual(2, result.NumberOfDecimalPlaces);
            Assert.AreEqual(-4.25, result.Value);
        }

        [Test]
        public void OperatorTimes_RightHasLeastPrecision_ResultIsRoundedDoubleWithLeastNumberOfDecimalPlaces()
        {
            // Setup
            var roundedDouble1 = new RoundedDouble(4, -4.5678);
            var roundedDouble2 = new RoundedDouble(3, -9.123);

            // Call
            RoundedDouble result = roundedDouble1 * roundedDouble2;

            // Assert
            Assert.AreEqual(3, result.NumberOfDecimalPlaces);
            Assert.AreEqual(41.672, result.Value);
        }

        [Test]
        public void OperatorTimes_TwoRoundedDoubles_MultiplicationIsCommutative()
        {
            // Setup
            var roundedDouble1 = new RoundedDouble(1, 1.1);
            var roundedDouble2 = new RoundedDouble(2, 2.22);

            // Call
            RoundedDouble result1 = roundedDouble1 * roundedDouble2;
            RoundedDouble result2 = roundedDouble2 * roundedDouble1;

            // Assert
            Assert.AreEqual(result1.NumberOfDecimalPlaces, result2.NumberOfDecimalPlaces);
            Assert.AreEqual(result1.Value, result2.Value);
        }

        [Test]
        public void CompareTo_Null_ReturnsExpectedResult()
        {
            // Setup
            var roundedDouble = new RoundedDouble(1, 10);

            // Call
            int result = roundedDouble.CompareTo(null);

            // Assert
            Assert.AreEqual(1, result);
        }

        [Test]
        public void CompareTo_Object_ThrowsArgumentException()
        {
            // Setup
            var roundedDouble = new RoundedDouble(1, 10);

            // Call
            TestDelegate call = () => roundedDouble.CompareTo(new object());

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, "Arg must be double or RoundedDouble");
        }

        [Test]
        public void CompareTo_Itself_ReturnsZero()
        {
            // Setup
            var roundedDouble = new RoundedDouble(1, 10);

            // Call
            int result = roundedDouble.CompareTo(roundedDouble);

            // Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        [TestCase(10, 10, 0)]
        [TestCase(10.000005, 10, 0)]
        [TestCase(10, -10, 1)]
        [TestCase(10.05, 10, 1)]
        [TestCase(-10, 10, -1)]
        [TestCase(10, 10.05, -1)]
        [TestCase(10, 10.000005, -1)]
        [TestCase(10, double.NaN, 1)]
        [TestCase(double.NaN, 10, -1)]
        [TestCase(double.NaN, double.NaN, 0)]
        public void CompareTo_RoundedDoubleToDouble_ReturnsExpectedResult(double roundedDoubleValue, double value,
                                                                          int expectedRoundedDoubleIndex)
        {
            // Setup
            var roundedDouble = new RoundedDouble(1, roundedDoubleValue);

            // Call
            int roundedDoubleResult = roundedDouble.CompareTo(value);
            int doubleResult = value.CompareTo(roundedDouble);

            // Assert
            Assert.AreEqual(expectedRoundedDoubleIndex, roundedDoubleResult);
            Assert.AreEqual(-1 * expectedRoundedDoubleIndex, doubleResult);
        }

        [Test]
        [TestCase(10, 10, 0)]
        [TestCase(10.000005, 10, 0)]
        [TestCase(10, 10.000005, 0)]
        [TestCase(10.005, 10, 0)]
        [TestCase(10, 10.005, 0)]
        [TestCase(10, -10, 1)]
        [TestCase(10.05, 10, 1)]
        [TestCase(-10, 10, -1)]
        [TestCase(10, 10.05, -1)]
        [TestCase(10, double.NaN, 1)]
        [TestCase(double.NaN, 10, -1)]
        [TestCase(double.NaN, double.NaN, 0)]
        public void CompareTo_RoundedDoubleToRoundedDouble_ReturnsExpectedResult(double roundedDoubleValue, double roundedDoubleValue2,
                                                                                 int expectedRoundedDoubleIndex)
        {
            // Setup
            var roundedDouble1 = new RoundedDouble(1, roundedDoubleValue);
            var roundedDouble2 = new RoundedDouble(1, roundedDoubleValue2);

            // Call
            int roundedDouble1Result = roundedDouble1.CompareTo(roundedDouble2);
            int roundedDouble2Result = roundedDouble2.CompareTo(roundedDouble1);

            // Assert
            Assert.AreEqual(expectedRoundedDoubleIndex, roundedDouble1Result);
            Assert.AreEqual(-1 * expectedRoundedDoubleIndex, roundedDouble2Result);
        }

        [Test]
        [TestCase(10, 10, 10, 0)]
        [TestCase(10.005, 10, 10, 0)]
        [TestCase(10, 10.005, 10, 0)]
        [TestCase(10, 10, 10.005, 0)]
        [TestCase(10, 11, 12, -1)]
        [TestCase(12, 11, 10, 1)]
        public void CompareTo_TransitiveRoundedDouble_ReturnsExpectedResult(double roundedDoubleValue1, double roundedDoubleValue2,
                                                                            double roundedDoubleValue3, int expectedValue)
        {
            // Setup
            var roundedDouble1 = new RoundedDouble(1, roundedDoubleValue1);
            var roundedDouble2 = new RoundedDouble(1, roundedDoubleValue2);
            var roundedDouble3 = new RoundedDouble(1, roundedDoubleValue3);

            // Call
            int roundedDoubleResult12 = roundedDouble1.CompareTo(roundedDouble2);
            int roundedDoubleResult23 = roundedDouble2.CompareTo(roundedDouble3);
            int roundedDoubleResult13 = roundedDouble1.CompareTo(roundedDouble3);

            // Assert
            Assert.AreEqual(expectedValue, roundedDoubleResult12);
            Assert.AreEqual(expectedValue, roundedDoubleResult23);
            Assert.AreEqual(expectedValue, roundedDoubleResult13);
        }

        [Test]
        [TestCase(10, 10, 10, 0)]
        [TestCase(10.005, 10, 10, 0)]
        [TestCase(10, 11, 12, -1)]
        [TestCase(12, 11, 10, 1)]
        public void CompareTo_TransitiveDouble_ReturnsExpectedResult(double roundedDoubleValue, double value2,
                                                                     double value3, int expectedValue)
        {
            // Setup
            var roundedDouble1 = new RoundedDouble(1, roundedDoubleValue);

            // Call
            int roundedDoubleResult12 = roundedDouble1.CompareTo(value2);
            int roundedDoubleResult23 = value2.CompareTo(value3);
            int roundedDoubleResult13 = roundedDouble1.CompareTo(value3);

            // Assert
            Assert.AreEqual(expectedValue, roundedDoubleResult12);
            Assert.AreEqual(expectedValue, roundedDoubleResult23);
            Assert.AreEqual(expectedValue, roundedDoubleResult13);
        }

        [Test]
        [TestCase(11, 10, false)]
        [TestCase(10, 11, true)]
        [TestCase(10.05, 10, false)]
        [TestCase(10, 10.05, true)]
        public void OperatorLess_VaryingDouble_ReturnsExpectedValues(double roundedDoubleValue, double value,
                                                                     bool isRoundedDoubleLess)
        {
            // Setup
            var roundedDouble = new RoundedDouble(1, roundedDoubleValue);

            // Call 
            bool roundedDoubleIsLess = roundedDouble < value;
            bool doubleIsLess = value < roundedDouble;

            // Assert
            Assert.AreEqual(isRoundedDoubleLess, roundedDoubleIsLess);
            Assert.AreEqual(!isRoundedDoubleLess, doubleIsLess);
        }

        [Test]
        [TestCase(10, 10)]
        [TestCase(9.95, 10)]
        [TestCase(10.005, 10)]
        public void OperatorLess_RoundedDoubleEqualDouble_ReturnsFalse(double roundedDoubleValue, double value)
        {
            // Setup
            var roundedDouble = new RoundedDouble(1, roundedDoubleValue);

            // Call 
            bool roundedDoubleIsLess = roundedDouble < value;
            bool doubleIsLess = value < roundedDouble;

            // Assert
            Assert.IsFalse(roundedDoubleIsLess);
            Assert.IsFalse(doubleIsLess);
        }

        [Test]
        [TestCase(11, 10, false)]
        [TestCase(10, 11, true)]
        [TestCase(10.05, 10, false)]
        [TestCase(10, 10.05, true)]
        public void OperatorLess_VaryingRoundedDouble_ReturnsExpectedValues(double roundedDoubleValue1, double roundedDoubleValue2,
                                                                            bool isRoundedDoubleOneLess)
        {
            // Setup
            var roundedDoubleOne = new RoundedDouble(1, roundedDoubleValue1);
            var roundedDoubleTwo = new RoundedDouble(1, roundedDoubleValue2);

            // Call 
            bool roundedDoubleIsLess = roundedDoubleOne < roundedDoubleTwo;
            bool isLessDouble = roundedDoubleTwo < roundedDoubleOne;

            // Assert
            Assert.AreEqual(isRoundedDoubleOneLess, roundedDoubleIsLess);
            Assert.AreEqual(!isRoundedDoubleOneLess, isLessDouble);
        }

        [Test]
        [TestCase(10, 10)]
        [TestCase(9.95, 10)]
        [TestCase(10.04, 10)]
        [TestCase(10, 9.95)]
        [TestCase(10, 10.04)]
        public void OperatorLess_RoundedDoubleEqualRoundedDouble_ReturnsFalse(double roundedDoubleValue1, double roundedDoubleValue2)
        {
            // Setup
            var roundedDouble1 = new RoundedDouble(1, roundedDoubleValue1);
            var roundedDouble2 = new RoundedDouble(1, roundedDoubleValue2);

            // Call 
            bool roundedDoubleOneIsLess = roundedDouble1 < roundedDouble2;
            bool roundedDoubleTwoIsLess = roundedDouble2 < roundedDouble1;

            // Assert
            Assert.IsFalse(roundedDoubleOneIsLess);
            Assert.IsFalse(roundedDoubleTwoIsLess);
        }

        [Test]
        [TestCase(11, 10, false)]
        [TestCase(10, 11, true)]
        [TestCase(10.05, 10, false)]
        [TestCase(10, 10.05, true)]
        public void OperatorLessOrEqual_VaryingDouble_ReturnsExpectedValues(double roundedDoubleValue, double value,
                                                                            bool isRoundedDoubleLess)
        {
            // Setup
            var roundedDouble = new RoundedDouble(1, roundedDoubleValue);

            // Call 
            bool roundedDoubleIsLess = roundedDouble <= value;
            bool doubleIsLess = value <= roundedDouble;

            // Assert
            Assert.AreEqual(isRoundedDoubleLess, roundedDoubleIsLess);
            Assert.AreEqual(!isRoundedDoubleLess, doubleIsLess);
        }

        [Test]
        [TestCase(10, 10)]
        [TestCase(9.95, 10)]
        [TestCase(10.005, 10)]
        public void OperatorLessOrEqual_RoundedDoubleEqualDouble_ReturnsTrue(double roundedDoubleValue, double value)
        {
            // Setup
            var roundedDouble = new RoundedDouble(1, roundedDoubleValue);

            // Call 
            bool roundedDoubleIsLess = roundedDouble <= value;
            bool doubleIsLess = value <= roundedDouble;

            // Assert
            Assert.IsTrue(roundedDoubleIsLess);
            Assert.IsTrue(doubleIsLess);
        }

        [Test]
        [TestCase(11, 10, false)]
        [TestCase(10, 11, true)]
        [TestCase(10.05, 10, false)]
        [TestCase(10, 10.05, true)]
        public void OperatorLessOrEqual_VaryingRoundedDouble_ReturnsExpectedValues(double roundedDoubleValue1, double roundedDoubleValue2,
                                                                                   bool isRoundedDoubleOneLess)
        {
            // Setup
            var roundedDoubleOne = new RoundedDouble(1, roundedDoubleValue1);
            var roundedDoubleTwo = new RoundedDouble(1, roundedDoubleValue2);

            // Call 
            bool roundedDoubleIsLess = roundedDoubleOne <= roundedDoubleTwo;
            bool isLessDouble = roundedDoubleTwo <= roundedDoubleOne;

            // Assert
            Assert.AreEqual(isRoundedDoubleOneLess, roundedDoubleIsLess);
            Assert.AreEqual(!isRoundedDoubleOneLess, isLessDouble);
        }

        [Test]
        [TestCase(10, 10)]
        [TestCase(9.95, 10)]
        [TestCase(10.04, 10)]
        [TestCase(10, 9.95)]
        [TestCase(10, 10.04)]
        public void OperatorLessOrEqual_RoundedDoubleEqualRoundedDouble_ReturnsTrue(double roundedDoubleValue1, double roundedDoubleValue2)
        {
            // Setup
            var roundedDouble1 = new RoundedDouble(1, roundedDoubleValue1);
            var roundedDouble2 = new RoundedDouble(1, roundedDoubleValue2);

            // Call 
            bool roundedDoubleOneIsLess = roundedDouble1 <= roundedDouble2;
            bool roundedDoubleTwoIsLess = roundedDouble2 <= roundedDouble1;

            // Assert
            Assert.IsTrue(roundedDoubleOneIsLess);
            Assert.IsTrue(roundedDoubleTwoIsLess);
        }

        [Test]
        [TestCase(11, 10, true)]
        [TestCase(10, 11, false)]
        [TestCase(10.05, 10, true)]
        [TestCase(10, 10.05, false)]
        public void OperatorGreater_VaryingDouble_ReturnsExpectedValues(double roundedDoubleValue, double value,
                                                                        bool isRoundedDoubleGreater)
        {
            // Setup
            var roundedDouble = new RoundedDouble(1, roundedDoubleValue);

            // Call 
            bool roundedDoubleIsGreater = roundedDouble > value;
            bool doubleIsGreater = value > roundedDouble;

            // Assert
            Assert.AreEqual(isRoundedDoubleGreater, roundedDoubleIsGreater);
            Assert.AreEqual(!isRoundedDoubleGreater, doubleIsGreater);
        }

        [Test]
        [TestCase(10, 10)]
        [TestCase(9.95, 10)]
        [TestCase(10.005, 10)]
        public void OperatorGreater_RoundedDoubleEqualDouble_ReturnsFalse(double roundedDoubleValue, double value)
        {
            // Setup
            var roundedDouble = new RoundedDouble(1, roundedDoubleValue);

            // Call 
            bool roundedDoubleIsLess = roundedDouble > value;
            bool doubleIsLess = value > roundedDouble;

            // Assert
            Assert.IsFalse(roundedDoubleIsLess);
            Assert.IsFalse(doubleIsLess);
        }

        [Test]
        [TestCase(11, 10, true)]
        [TestCase(10, 11, false)]
        [TestCase(10.05, 10, true)]
        [TestCase(10, 10.05, false)]
        public void OperatorGreater_VaryingRoundedDouble_ReturnsExpectedValues(double roundedDoubleValue1, double roundedDoubleValue2,
                                                                               bool isRoundedDoubleOneGreater)
        {
            // Setup
            var roundedDoubleOne = new RoundedDouble(1, roundedDoubleValue1);
            var roundedDoubleTwo = new RoundedDouble(1, roundedDoubleValue2);

            // Call 
            bool roundedDoubleOneIsGreater = roundedDoubleOne > roundedDoubleTwo;
            bool roundedDoubleTwoIsGreater = roundedDoubleTwo > roundedDoubleOne;

            // Assert
            Assert.AreEqual(isRoundedDoubleOneGreater, roundedDoubleOneIsGreater);
            Assert.AreEqual(!isRoundedDoubleOneGreater, roundedDoubleTwoIsGreater);
        }

        [Test]
        [TestCase(10, 10)]
        [TestCase(9.95, 10)]
        [TestCase(10.04, 10)]
        [TestCase(10, 9.95)]
        [TestCase(10, 10.04)]
        public void OperatorGreater_RoundedDoubleEqualRoundedDouble_ReturnsFalse(double roundedDoubleValue1, double roundedDoubleValue2)
        {
            // Setup
            var roundedDouble1 = new RoundedDouble(1, roundedDoubleValue1);
            var roundedDouble2 = new RoundedDouble(1, roundedDoubleValue2);

            // Call 
            bool roundedDoubleOneIsGreater = roundedDouble1 > roundedDouble2;
            bool roundedDoubleTwoIsGreater = roundedDouble2 > roundedDouble1;

            // Assert
            Assert.IsFalse(roundedDoubleOneIsGreater);
            Assert.IsFalse(roundedDoubleTwoIsGreater);
        }

        [Test]
        [TestCase(11, 10, true)]
        [TestCase(10, 11, false)]
        [TestCase(10.05, 10, true)]
        [TestCase(10, 10.05, false)]
        public void OperatorGreaterOrEqual_VaryingDouble_ReturnsExpectedValues(double roundedDoubleValue, double value,
                                                                               bool isRoundedDoubleGreater)
        {
            // Setup
            var roundedDouble = new RoundedDouble(1, roundedDoubleValue);

            // Call 
            bool roundedDoubleIsGreater = roundedDouble >= value;
            bool doubleIsGreater = value >= roundedDouble;

            // Assert
            Assert.AreEqual(isRoundedDoubleGreater, roundedDoubleIsGreater);
            Assert.AreEqual(!isRoundedDoubleGreater, doubleIsGreater);
        }

        [Test]
        [TestCase(10, 10)]
        [TestCase(9.95, 10)]
        [TestCase(10.005, 10)]
        public void OperatorGreaterOrEqual_RoundedDoubleEqualDouble_ReturnsTrue(double roundedDoubleValue, double value)
        {
            // Setup
            var roundedDouble = new RoundedDouble(1, roundedDoubleValue);

            // Call 
            bool roundedDoubleIsLess = roundedDouble >= value;
            bool doubleIsLess = value >= roundedDouble;

            // Assert
            Assert.IsTrue(roundedDoubleIsLess);
            Assert.IsTrue(doubleIsLess);
        }

        [Test]
        [TestCase(11, 10, true)]
        [TestCase(10, 11, false)]
        [TestCase(10.05, 10, true)]
        [TestCase(10, 10.05, false)]
        public void OperatorGreaterOrEqual_VaryingRoundedDouble_ReturnsExpectedValues(double roundedDoubleValue1, double roundedDoubleValue2,
                                                                                      bool isRoundedDoubleOneGreater)
        {
            // Setup
            var roundedDoubleOne = new RoundedDouble(1, roundedDoubleValue1);
            var roundedDoubleTwo = new RoundedDouble(1, roundedDoubleValue2);

            // Call 
            bool roundedDoubleOneIsGreater = roundedDoubleOne >= roundedDoubleTwo;
            bool roundedDoubleTwoIsGreater = roundedDoubleTwo >= roundedDoubleOne;

            // Assert
            Assert.AreEqual(isRoundedDoubleOneGreater, roundedDoubleOneIsGreater);
            Assert.AreEqual(!isRoundedDoubleOneGreater, roundedDoubleTwoIsGreater);
        }

        [Test]
        [TestCase(10, 10)]
        [TestCase(9.95, 10)]
        [TestCase(10.04, 10)]
        [TestCase(10, 9.95)]
        [TestCase(10, 10.04)]
        public void OperatorGreaterOrEqual_RoundedDoubleEqualRoundedDouble_ReturnsFalse(double roundedDoubleValue1, double roundedDoubleValue2)
        {
            // Setup
            var roundedDouble1 = new RoundedDouble(1, roundedDoubleValue1);
            var roundedDouble2 = new RoundedDouble(1, roundedDoubleValue2);

            // Call 
            bool roundedDoubleOneIsGreater = roundedDouble1 >= roundedDouble2;
            bool roundedDoubleTwoIsGreater = roundedDouble2 >= roundedDouble1;

            // Assert
            Assert.IsTrue(roundedDoubleOneIsGreater);
            Assert.IsTrue(roundedDoubleTwoIsGreater);
        }
    }
}