﻿using System;
using System.ComponentModel;
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
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call,
                "Value must be in range [0, 15].");
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
        public void Equals_ToNull_ReturnFalse()
        {
            // Setup
            var roundedDouble = new RoundedDouble(2);

            // Call
            bool isEqual = roundedDouble.Equals((object)null);

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
            bool isEqual = roundedDouble.Equals((object)roundedDouble);

            // Assert
            Assert.IsTrue(isEqual);
        }

        [Test]
        public void Equals_ObjectOfSomeOtherType_ReturnFalse()
        {
            // Setup
            var roundedDouble = new RoundedDouble(1);
            object someOtherObject = new object();

            // Call
            var isEqual1 = roundedDouble.Equals(someOtherObject);
            var isEqual2 = someOtherObject.Equals(roundedDouble);

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
            var isEqual1 = baseRoundedDouble.Equals(comparisonRoundedDouble);
            var isEqual2 = comparisonRoundedDouble.Equals(baseRoundedDouble);

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
            var isEqual1 = roundedDouble1 == roundedDouble2;
            var isEqual2 = roundedDouble2 == roundedDouble1;

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
            var isEqual1 = roundedDouble1 == roundedDouble2;
            var isEqual2 = roundedDouble2 == roundedDouble1;

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
            var isNotEqual1 = roundedDouble1 != roundedDouble2;
            var isNotEqual2 = roundedDouble2 != roundedDouble1;

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
            var isNotEqual1 = roundedDouble1 != roundedDouble2;
            var isNotEqual2 = roundedDouble2 != roundedDouble1;

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
            var isEqual1 = roundedDouble.Equals(value);
            var isEqual2 = value.Equals(roundedDouble);

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
            var isEqual1 = roundedDouble.Equals(value);
            var isEqual2 = value.Equals(roundedDouble);

            // Assert
            Assert.IsFalse(isEqual1);
            Assert.IsFalse(isEqual2);
        }

        [Test]
        public void Equals_RoundedDoubleTotallyDifferentFromDouble_ReturnFalse()
        {
            // Setup
            var roundedDouble = new RoundedDouble(2, 1.23);
            double otherValue = 4.56;

            // Call
            var isEqual1 = roundedDouble.Equals(otherValue);
            var isEqual2 = otherValue.Equals(roundedDouble);

            // Assert
            Assert.IsFalse(isEqual1);
            Assert.IsFalse(isEqual2);
        }

        [Test]
        public void GetHashCode_RoundedDoubleEqualToDouble_ReturnSameHashCode()
        {
            // Setup
            double otherValue = 4.56;
            var roundedDouble = new RoundedDouble(2, otherValue);

            // Precondition:
            Assert.IsTrue(otherValue.Equals(roundedDouble));
            
            // Call
            var hash1 = roundedDouble.GetHashCode();
            var hash2 = otherValue.GetHashCode();

            // Assert
            Assert.AreEqual(hash1, hash2);
        }

        [Test]
        public void DoubleEqualityOperator_DoubleIsEqualToRoundedDouble_ReturnTrue()
        {
            // Setup
            double value = 1.234;
            var roundedDouble = new RoundedDouble(4, value);

            // Precondition
            Assert.IsTrue(roundedDouble.Equals(value));

            // Call
            var isEqual1 = value == roundedDouble;
            var isEqual2 = roundedDouble == value;

            // Assert
            Assert.IsTrue(isEqual1);
            Assert.IsTrue(isEqual2);
        }

        [Test]
        public void DoubleEqualityOperator_DoubleIsNotEqualToRoundedDouble_ReturnFalse()
        {
            // Setup
            double value = 1.234;
            var roundedDouble = new RoundedDouble(4, 3.21543);

            // Precondition
            Assert.IsFalse(roundedDouble.Equals(value));

            // Call
            var isEqual1 = value == roundedDouble;
            var isEqual2 = roundedDouble == value;

            // Assert
            Assert.IsFalse(isEqual1);
            Assert.IsFalse(isEqual2);
        }

        [Test]
        public void DoubleInequalityOperator_DoubleIsEqualToRoundedDouble_ReturnFalse()
        {
            // Setup
            double value = 1.234;
            var roundedDouble = new RoundedDouble(4, value);

            // Precondition
            Assert.IsTrue(roundedDouble.Equals(value));

            // Call
            var isEqual1 = value != roundedDouble;
            var isEqual2 = roundedDouble != value;

            // Assert
            Assert.IsFalse(isEqual1);
            Assert.IsFalse(isEqual2);
        }

        [Test]
        public void DoubleInequalityOperator_DoubleIsNotEqualToRoundedDouble_ReturnTrue()
        {
            // Setup
            double value = 1.234;
            var roundedDouble = new RoundedDouble(4, 3.21543);

            // Precondition
            Assert.IsFalse(roundedDouble.Equals(value));

            // Call
            var isEqual1 = value != roundedDouble;
            var isEqual2 = roundedDouble != value;

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
            double doubleValue = 1.23456789;

            // Call
            RoundedDouble roundedDoubleValue = (RoundedDouble)doubleValue;

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
    }
}