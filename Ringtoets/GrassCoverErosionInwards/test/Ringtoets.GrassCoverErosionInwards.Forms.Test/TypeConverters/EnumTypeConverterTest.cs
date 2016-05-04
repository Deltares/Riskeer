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

using System.ComponentModel;
using Core.Common.Utils.Attributes;
using NUnit.Framework;
using Ringtoets.GrassCoverErosionInwards.Forms.Test.Properties;
using Ringtoets.GrassCoverErosionInwards.Forms.TypeConverters;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.TypeConverters
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
            Assert.IsInstanceOf<TypeConverter>(converter);
        }

        [Test]
        public void CanConvertTo_DestinationTypeIsString_ReturnTrue()
        {
            // Setup
            var converter = new EnumTypeConverter(typeof(object));

            // Call
            var canConvert = converter.CanConvertTo(typeof(string));

            // Assert
            Assert.IsTrue(canConvert);
        }

        [Test]
        public void ConvertTo_DestinationTypeIsString_ReturnsExpectedEnumDisplayName()
        {
            // Setup
            const SimpleEnum enumValue = SimpleEnum.FirstValue;
            var converter = new EnumTypeConverter(typeof(SimpleEnum));

            // Call
            var result = converter.ConvertTo(enumValue, typeof(string));

            // Assert
            var expectedText = "<first>";
            Assert.AreEqual(expectedText, result);
        }

        [Test]
        public void ConvertTo_NullValue_DoesNotThrowException()
        {
            // Setup
            var converter = new EnumTypeConverter(typeof(SimpleEnum));

            // Call
            object result = new object();
            TestDelegate test = () => result = converter.ConvertTo(null, typeof(string));

            // Assert
            Assert.DoesNotThrow(test);
            Assert.AreEqual(string.Empty, result);
        }

        [Test]
        public void CanConvertFrom_SourceTypeIsString_ReturnTrue()
        {
            // Setup
            var converter = new EnumTypeConverter(typeof(object));

            // Call
            var canConvert = converter.CanConvertFrom(typeof(string));

            // Assert
            Assert.IsTrue(canConvert);
        }

        [Test]
        public void ConvertFrom_NullValue_DoesNotThrowException()
        {
            // Setup
            var converter = new EnumTypeConverter(typeof(SimpleEnum));

            // Call
            object result = new object();
            TestDelegate test = () => result = converter.ConvertFrom(null);

            // Assert
            Assert.DoesNotThrow(test);
            Assert.IsNull(result);
        }

        [Test]
        public void ConvertFrom_SourceTypeIsString_ReturnsExpectedEnum()
        {
            // Setup
            const string second = "<second>";
            var converter = new EnumTypeConverter(typeof(SimpleEnum));

            // Call
            var result = converter.ConvertFrom(second);

            // Assert
            var expectedEnumValue = SimpleEnum.SecondValue;
            Assert.AreEqual(expectedEnumValue, result);
        }

        private enum SimpleEnum
        {
            [ResourcesEnumDisplayName(typeof(Resources), "SimpleEnum_FirstValue_DisplayName")]
            FirstValue,

            [ResourcesEnumDisplayName(typeof(Resources), "SimpleEnum_SecondValue_DisplayName")]
            SecondValue
        }
    }
}