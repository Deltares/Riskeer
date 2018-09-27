// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using Core.Common.Util.Attributes;
using Core.Common.Util.Test.Properties;
using NUnit.Framework;

namespace Core.Common.Util.Test
{
    [TestFixture]
    public class EnumTypeConverterTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var converter = new EnumTypeConverter(typeof(object));

            // Assert
            Assert.IsInstanceOf<EnumConverter>(converter);
        }

        [Test]
        public void CanConvertTo_DestinationTypeIsInvalid_ReturnsFalse()
        {
            // Setup
            var converter = new EnumTypeConverter(typeof(object));

            // Call
            bool canConvert = converter.CanConvertTo(typeof(NotSupportedType));

            // Assert
            Assert.IsFalse(canConvert);
        }

        [Test]
        public void CanConvertTo_DestinationTypeIsString_ReturnsTrue()
        {
            // Setup
            var converter = new EnumTypeConverter(typeof(object));

            // Call
            bool canConvert = converter.CanConvertTo(typeof(string));

            // Assert
            Assert.IsTrue(canConvert);
        }

        [Test]
        public void ConvertTo_ValueIsOfInvalidType_ThrowsNotSupportedException()
        {
            // Setup
            var notSupportedValue = new NotSupportedType();
            var converter = new EnumTypeConverter(typeof(SimpleEnum));

            // Call
            TestDelegate test = () => converter.ConvertTo(notSupportedValue, typeof(SimpleEnum));

            // Assert
            Assert.Throws<NotSupportedException>(test);
        }

        [Test]
        public void ConvertTo_ValueIsNull_DoesNotThrowException()
        {
            // Setup
            var converter = new EnumTypeConverter(typeof(SimpleEnum));

            // Call
            var result = new object();
            TestDelegate test = () => result = converter.ConvertTo(null, typeof(string));

            // Assert
            Assert.DoesNotThrow(test);
            Assert.AreEqual(string.Empty, result);
        }

        [Test]
        public void ConvertTo_ValueIsNonExistingEnumValue_ReturnsEmptyString()
        {
            // Setup
            var converter = new EnumTypeConverter(typeof(SimpleEnum));

            // Call
            object result = converter.ConvertTo((SimpleEnum) 5, typeof(string));

            // Assert
            Assert.AreEqual(string.Empty, result);
        }

        [Test]
        public void ConvertTo_ValueWithoutResourceDisplayName_ReturnsEmptyString()
        {
            // Setup
            var converter = new EnumTypeConverter(typeof(SimpleEnum));

            // Call
            object result = converter.ConvertTo(SimpleEnum.ThirdValue, typeof(string));

            // Assert
            string expectedText = SimpleEnum.ThirdValue.ToString();
            Assert.AreEqual(expectedText, result);
        }

        [Test]
        public void ConvertTo_DestinationTypeIsNull_ThrowsArgumentNullException()
        {
            // Setup
            const SimpleEnum enumValue = SimpleEnum.FirstValue;
            var converter = new EnumTypeConverter(typeof(SimpleEnum));

            // Call
            TestDelegate test = () => converter.ConvertTo(enumValue, null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void ConvertTo_DestinationTypeIsInvalid_ThrowsNotSupportedException()
        {
            // Setup
            const SimpleEnum enumValue = SimpleEnum.FirstValue;
            var converter = new EnumTypeConverter(typeof(SimpleEnum));

            // Call
            TestDelegate test = () => converter.ConvertTo(enumValue, typeof(NotSupportedType));

            // Assert
            Assert.Throws<NotSupportedException>(test);
        }

        [Test]
        public void ConvertTo_DestinationTypeIsString_ReturnsExpectedEnumDisplayName()
        {
            // Setup
            const SimpleEnum enumValue = SimpleEnum.FirstValue;
            var converter = new EnumTypeConverter(typeof(SimpleEnum));

            // Call
            object result = converter.ConvertTo(enumValue, typeof(string));

            // Assert
            const string expectedText = "<first>";
            Assert.AreEqual(expectedText, result);
        }

        [Test]
        public void CanConvertFrom_SourceTypeIsInvalid_ReturnsFalse()
        {
            // Setup
            var converter = new EnumTypeConverter(typeof(object));

            // Call
            bool canConvert = converter.CanConvertFrom(typeof(NotSupportedType));

            // Assert
            Assert.IsFalse(canConvert);
        }

        [Test]
        public void CanConvertFrom_SourceTypeIsString_ReturnsTrue()
        {
            // Setup
            var converter = new EnumTypeConverter(typeof(object));

            // Call
            bool canConvert = converter.CanConvertFrom(typeof(string));

            // Assert
            Assert.IsTrue(canConvert);
        }

        [Test]
        public void ConvertFrom_ValueIsOfInvalidType_ThrowsNotSupportedException()
        {
            // Setup
            var converter = new EnumTypeConverter(typeof(SimpleEnum));

            // Call
            TestDelegate test = () => converter.ConvertFrom(typeof(NotSupportedType));

            // Assert
            Assert.Throws<NotSupportedException>(test);
        }

        [Test]
        public void ConvertFrom_ValueIsNull_ThrowsNotSupportedException()
        {
            // Setup
            var converter = new EnumTypeConverter(typeof(SimpleEnum));

            // Call
            TestDelegate test = () => converter.ConvertFrom(null);

            // Assert
            Assert.Throws<NotSupportedException>(test);
        }

        [Test]
        public void ConvertFrom_ValueIsStringOfDisplayName_ReturnsExpectedEnum()
        {
            // Setup
            const string second = "<second>";
            var converter = new EnumTypeConverter(typeof(SimpleEnum));

            // Call
            object result = converter.ConvertFrom(second);

            // Assert
            const SimpleEnum expectedEnumValue = SimpleEnum.SecondValue;
            Assert.AreEqual(expectedEnumValue, result);
        }

        [Test]
        public void ConvertFrom_ValueIsStringOfElementName_ReturnsExpectedEnum()
        {
            // Setup
            string third = SimpleEnum.ThirdValue.ToString();
            var converter = new EnumTypeConverter(typeof(SimpleEnum));

            // Call
            object result = converter.ConvertFrom(third);

            // Assert
            const SimpleEnum expectedEnumValue = SimpleEnum.ThirdValue;
            Assert.AreEqual(expectedEnumValue, result);
        }

        [Test]
        public void ConvertFrom_ValueIsStringOfElementNameWhileHasDisplayName_ThrowsFormatException()
        {
            // Setup
            string second = SimpleEnum.FirstValue.ToString();
            var converter = new EnumTypeConverter(typeof(SimpleEnum));

            // Call
            TestDelegate call = () => converter.ConvertFrom(second);

            // Assert
            string message = Assert.Throws<FormatException>(call).Message;
            Assert.AreEqual("Slechts de volgende waardes worden geaccepteerd: <first>, <second>, ThirdValue.", message);
        }

        private enum SimpleEnum
        {
            [ResourcesDisplayName(typeof(Resources), nameof(Resources.SimpleEnum_FirstValue_DisplayName))]
            FirstValue = 1,

            [ResourcesDisplayName(typeof(Resources), nameof(Resources.SimpleEnum_SecondValue_DisplayName))]
            SecondValue = 2,

            ThirdValue = 3
        }

        private class NotSupportedType {}
    }
}