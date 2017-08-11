// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Globalization;
using System.Windows.Data;
using Core.Common.TestUtil;
using Core.Components.GraphSharp.Data;
using Core.Components.GraphSharp.Forms.Converters;
using NUnit.Framework;

namespace Core.Components.GraphSharp.Forms.Test.Converters
{
    [TestFixture]
    public class PointedTreeVertexTypeToStringConverterTest
    {
        [Test]
        public void DefaultConcstructor_ExpectedValues()
        {
            // Call
            var converter = new PointedTreeVertexTypeToStringConverter();

            // Assert
            Assert.IsInstanceOf<IValueConverter>(converter);
        }

        [Test]
        public void Convert_ValueNull_ReturnNull()
        {
            // Setup
            var converter = new PointedTreeVertexTypeToStringConverter();

            // Call
            object convertedValue = converter.Convert(null, typeof(string), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.IsNull(convertedValue);
        }

        [Test]
        public void Convert_InvalidPointedTreeVertexType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var converter = new PointedTreeVertexTypeToStringConverter();

            // Call
            TestDelegate test = () => converter.Convert((PointedTreeVertexType) 99, typeof(string), null, CultureInfo.InvariantCulture);

            // Assert
            const string message = "The value of argument 'value' (99) is invalid for Enum type 'PointedTreeVertexType'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, message);
        }

        [Test]
        [TestCase(PointedTreeVertexType.Rectangle, "Rectangle")]
        [TestCase(PointedTreeVertexType.Diamond, "Diamond")]
        public void Convert_WithPointedTreeVertexType_ConvertToString(PointedTreeVertexType vertexType, string expectedString)
        {
            // Setup
            var converter = new PointedTreeVertexTypeToStringConverter();

            // Call
            var actualString = converter.Convert(vertexType, typeof(string), null, CultureInfo.InvariantCulture) as string;

            // Assert
            Assert.NotNull(actualString);
            Assert.AreEqual(expectedString, actualString);
        }

        [Test]
        public void ConvertBack_ThrowNotImplementedException()
        {
            // Setup
            var converter = new PointedTreeVertexTypeToStringConverter();

            // Call
            TestDelegate call = () => converter.ConvertBack(string.Empty, typeof(PointedTreeVertexType), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.Throws<NotImplementedException>(call);
        }
    }
}