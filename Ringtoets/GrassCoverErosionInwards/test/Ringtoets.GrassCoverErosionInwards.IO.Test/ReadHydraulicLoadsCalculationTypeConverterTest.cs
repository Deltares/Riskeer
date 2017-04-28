﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

namespace Ringtoets.GrassCoverErosionInwards.IO.Test
{
    [TestFixture]
    public class ReadHydraulicLoadsCalculationTypeConverterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var converter = new ReadHydraulicLoadsCalculationTypeConverter();

            // Assert
            Assert.IsInstanceOf<TypeConverter>(converter);
        }

        [Test]
        public void CanConvertTo_String_ReturnTrue()
        {
            // Setup
            var converter = new ReadHydraulicLoadsCalculationTypeConverter();

            // Call
            bool canConvertToString = converter.CanConvertTo(typeof(string));

            // Assert
            Assert.IsTrue(canConvertToString);
        }

        [Test]
        public void CanConvertTo_NotString_ReturnFalse()
        {
            // Setup
            var converter = new ReadHydraulicLoadsCalculationTypeConverter();

            // Call
            bool canConvert = converter.CanConvertTo(typeof(object));

            // Assert
            Assert.IsFalse(canConvert);
        }

        [Test]
        [TestCase(ReadHydraulicLoadsCalculationType.NoCalculation, "niet")]
        [TestCase(ReadHydraulicLoadsCalculationType.CalculateByAssessmentSectionNorm, "norm")]
        [TestCase(ReadHydraulicLoadsCalculationType.CalculateByProfileSpecificRequiredProbability, "doorsnede")]
        public void ConvertTo_VariousCases_ReturnExpectedValues(ReadHydraulicLoadsCalculationType value, string expectedResult)
        {
            // Setup
            var converter = new ReadHydraulicLoadsCalculationTypeConverter();

            // Call
            object result = converter.ConvertTo(value, typeof(string));

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void ConvertTo_InvalidReadHydraulicLoadsCalculationTypeValue_ThrowNotSupportedException()
        {
            // Setup
            var converter = new ReadHydraulicLoadsCalculationTypeConverter();

            const ReadHydraulicLoadsCalculationType invalidValue = (ReadHydraulicLoadsCalculationType) 9999999;

            // Call
            TestDelegate call = () => converter.ConvertTo(invalidValue, typeof(string));

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }

        [Test]
        public void ConvertTo_Object_ThrowNotSupportedException()
        {
            // Setup
            var converter = new ReadHydraulicLoadsCalculationTypeConverter();

            // Call
            TestDelegate call = () => converter.ConvertTo(ReadHydraulicLoadsCalculationType.NoCalculation, typeof(object));

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }

        [Test]
        public void CanConvertFrom_String_ReturnTrue()
        {
            // Setup
            var converter = new ReadHydraulicLoadsCalculationTypeConverter();

            // Call
            bool canConvertFromString = converter.CanConvertFrom(typeof(string));

            // Assert
            Assert.IsTrue(canConvertFromString);
        }

        [Test]
        public void CanConvertFrom_NonString_ReturnFalse()
        {
            // Setup
            var converter = new ReadHydraulicLoadsCalculationTypeConverter();

            // Call
            bool canConvert = converter.CanConvertFrom(typeof(object));

            // Assert
            Assert.IsFalse(canConvert);
        }

        [Test]
        [TestCase("niet", ReadHydraulicLoadsCalculationType.NoCalculation)]
        [TestCase("norm", ReadHydraulicLoadsCalculationType.CalculateByAssessmentSectionNorm)]
        [TestCase("doorsnede", ReadHydraulicLoadsCalculationType.CalculateByProfileSpecificRequiredProbability)]
        public void ConvertFrom_VariousCases_ReturnExpectedValue(string value, ReadHydraulicLoadsCalculationType expectedResult)
        {
            // Setup
            var converter = new ReadHydraulicLoadsCalculationTypeConverter();

            // Call
            object result = converter.ConvertFrom(value);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void ConvertFrom_UnsupportedString_ThrowNotSupportedException()
        {
            // Setup
            var converter = new ReadHydraulicLoadsCalculationTypeConverter();

            // Call
            TestDelegate call = () => converter.ConvertFrom("<unsupported string value>");

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }
    }
}