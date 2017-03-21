﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.ComponentModel;
using NUnit.Framework;
using Ringtoets.Common.IO.Readers;
using Ringtoets.Common.IO.Schema;

namespace Ringtoets.Common.IO.Test
{
    [TestFixture]
    public class BreakWaterTypeConverterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var converter = new ReadBreakWaterTypeConverter();

            // Assert
            Assert.IsInstanceOf<TypeConverter>(converter);
        }

        [Test]
        public void CanConvertTo_String_ReturnTrue()
        {
            // Setup
            var converter = new ReadBreakWaterTypeConverter();

            // Call
            bool canConvertToString = converter.CanConvertTo(typeof(string));

            // Assert
            Assert.IsTrue(canConvertToString);
        }

        [Test]
        public void CanConvertTo_OtherThanString_ReturnFalse()
        {
            // Setup
            var converter = new ReadBreakWaterTypeConverter();

            // Call
            bool canConvertToString = converter.CanConvertTo(typeof(object));

            // Assert
            Assert.IsFalse(canConvertToString);
        }

        [Test]
        [TestCase(ReadBreakWaterType.Caisson, ConfigurationSchemaIdentifiers.BreakWaterCaisson)]
        [TestCase(ReadBreakWaterType.Dam, ConfigurationSchemaIdentifiers.BreakWaterDam)]
        [TestCase(ReadBreakWaterType.Wall, ConfigurationSchemaIdentifiers.BreakWaterWall)]
        public void ConverTo_VariousCases_ReturnExpectedText(ReadBreakWaterType value,
                                                             string expectedResult)
        {
            // Setup
            var converter = new ReadBreakWaterTypeConverter();

            // Call
            object result = converter.ConvertTo(value, typeof(string));

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void ConvertTo_InvalidBreakWaterType_ThrowNotSupportedException()
        {
            // Setup
            var converter = new ReadBreakWaterTypeConverter();
            var invalidValue = (ReadBreakWaterType) 99999999;

            // Call
            TestDelegate call = () => converter.ConvertTo(invalidValue, typeof(string));

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }

        [Test]
        public void CanConvertFrom_String_ReturnTrue()
        {
            // Setup
            var converter = new ReadBreakWaterTypeConverter();

            // Call
            bool canConvertFromString = converter.CanConvertFrom(typeof(string));

            // Assert
            Assert.IsTrue(canConvertFromString);
        }

        [Test]
        public void CanConvertFrom_OtherThanString_ReturnFalse()
        {
            // Setup
            var converter = new ReadBreakWaterTypeConverter();

            // Call
            bool canConvertFromString = converter.CanConvertFrom(typeof(object));

            // Assert
            Assert.IsFalse(canConvertFromString);
        }

        [Test]
        [TestCase(ConfigurationSchemaIdentifiers.BreakWaterCaisson, ReadBreakWaterType.Caisson)]
        [TestCase(ConfigurationSchemaIdentifiers.BreakWaterDam, ReadBreakWaterType.Dam)]
        [TestCase(ConfigurationSchemaIdentifiers.BreakWaterWall, ReadBreakWaterType.Wall)]
        public void ConvertFrom_Text_ReturnExpectedBreakWaterType(string value,
                                                                  ReadBreakWaterType expectedResult)
        {
            // Setup
            var converter = new ReadBreakWaterTypeConverter();

            // Call
            object result = converter.ConvertFrom(value);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void ConvertFrom_InvalidText_ThrowNotSupportedException()
        {
            // Setup
            var converter = new ReadBreakWaterTypeConverter();

            // Call
            TestDelegate call = () => converter.ConvertFrom("A");

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }
    }
}