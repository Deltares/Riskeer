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
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.Configurations.Helpers;
using Ringtoets.Common.IO.Schema;

namespace Ringtoets.Common.IO.Test.Configurations
{
    [TestFixture]
    public class SchemaBreakWaterTypeConverterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var converter = new SchemaBreakWaterTypeConverter();

            // Assert
            Assert.IsInstanceOf<TypeConverter>(converter);
        }

        [Test]
        public void CanConvertTo_String_ReturnTrue()
        {
            // Setup
            var converter = new SchemaBreakWaterTypeConverter();

            // Call
            bool canConvertToString = converter.CanConvertTo(typeof(string));

            // Assert
            Assert.IsTrue(canConvertToString);
        }

        [Test]
        public void CanConvertTo_BreakWaterType_ReturnTrue()
        {
            // Setup
            var converter = new SchemaBreakWaterTypeConverter();

            // Call
            bool canConvertToString = converter.CanConvertTo(typeof(BreakWaterType));

            // Assert
            Assert.IsTrue(canConvertToString);
        }

        [Test]
        public void CanConvertTo_OtherThanStringOrBreakWaterType_ReturnFalse()
        {
            // Setup
            var converter = new SchemaBreakWaterTypeConverter();

            // Call
            bool canConvertToString = converter.CanConvertTo(typeof(object));

            // Assert
            Assert.IsFalse(canConvertToString);
        }

        [Test]
        [TestCase(SchemaBreakWaterType.Caisson, ConfigurationSchemaIdentifiers.BreakWaterCaisson)]
        [TestCase(SchemaBreakWaterType.Dam, ConfigurationSchemaIdentifiers.BreakWaterDam)]
        [TestCase(SchemaBreakWaterType.Wall, ConfigurationSchemaIdentifiers.BreakWaterWall)]
        public void ConvertTo_VariousCases_ReturnExpectedText(SchemaBreakWaterType value,
                                                              string expectedResult)
        {
            // Setup
            var converter = new SchemaBreakWaterTypeConverter();

            // Call
            object result = converter.ConvertTo(value, typeof(string));

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void ConvertTo_InvalidBreakWaterType_ThrowNotSupportedException()
        {
            // Setup
            var converter = new SchemaBreakWaterTypeConverter();
            var invalidValue = (SchemaBreakWaterType) 99999999;

            // Call
            TestDelegate call = () => converter.ConvertTo(invalidValue, typeof(string));

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }

        [Test]
        [TestCase(SchemaBreakWaterType.Caisson, BreakWaterType.Caisson)]
        [TestCase(SchemaBreakWaterType.Dam, BreakWaterType.Dam)]
        [TestCase(SchemaBreakWaterType.Wall, BreakWaterType.Wall)]
        public void ConvertTo_VariousCases_ReturnExpectedText(SchemaBreakWaterType value,
                                                              BreakWaterType expectedResult)
        {
            // Setup
            var converter = new SchemaBreakWaterTypeConverter();

            // Call
            object result = converter.ConvertTo(value, typeof(BreakWaterType));

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void CanConvertFrom_String_ReturnTrue()
        {
            // Setup
            var converter = new SchemaBreakWaterTypeConverter();

            // Call
            bool canConvertFromString = converter.CanConvertFrom(typeof(string));

            // Assert
            Assert.IsTrue(canConvertFromString);
        }

        [Test]
        public void CanConvertFrom_BreakWaterType_ReturnTrue()
        {
            // Setup
            var converter = new SchemaBreakWaterTypeConverter();

            // Call
            bool canConvertFromString = converter.CanConvertFrom(typeof(BreakWaterType));

            // Assert
            Assert.IsTrue(canConvertFromString);
        }

        [Test]
        public void CanConvertFrom_OtherThanStringOrBreakWaterType_ReturnFalse()
        {
            // Setup
            var converter = new SchemaBreakWaterTypeConverter();

            // Call
            bool canConvertFromString = converter.CanConvertFrom(typeof(object));

            // Assert
            Assert.IsFalse(canConvertFromString);
        }

        [Test]
        [TestCase(ConfigurationSchemaIdentifiers.BreakWaterCaisson, SchemaBreakWaterType.Caisson)]
        [TestCase(ConfigurationSchemaIdentifiers.BreakWaterDam, SchemaBreakWaterType.Dam)]
        [TestCase(ConfigurationSchemaIdentifiers.BreakWaterWall, SchemaBreakWaterType.Wall)]
        public void ConvertFrom_Text_ReturnExpectedBreakWaterType(string value,
                                                                  SchemaBreakWaterType expectedResult)
        {
            // Setup
            var converter = new SchemaBreakWaterTypeConverter();

            // Call
            object result = converter.ConvertFrom(value);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void ConvertFrom_InvalidText_ThrowNotSupportedException()
        {
            // Setup
            var converter = new SchemaBreakWaterTypeConverter();

            // Call
            TestDelegate call = () => converter.ConvertFrom("A");

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }

        [Test]
        [TestCase(BreakWaterType.Caisson, SchemaBreakWaterType.Caisson)]
        [TestCase(BreakWaterType.Dam, SchemaBreakWaterType.Dam)]
        [TestCase(BreakWaterType.Wall, SchemaBreakWaterType.Wall)]
        public void ConvertFrom_BreakWaterType_ReturnExpectedBreakWaterType(BreakWaterType value,
                                                                            SchemaBreakWaterType expectedResult)
        {
            // Setup
            var converter = new SchemaBreakWaterTypeConverter();

            // Call
            object result = converter.ConvertFrom(value);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}