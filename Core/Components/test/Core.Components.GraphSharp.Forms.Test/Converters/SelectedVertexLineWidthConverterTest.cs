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
using System.Globalization;
using System.Windows.Data;
using Core.Components.GraphSharp.Forms.Converters;
using NUnit.Framework;

namespace Core.Components.GraphSharp.Forms.Test.Converters
{
    [TestFixture]
    public class SelectedVertexLineWidthConverterTest
    {
        [Test]
        public void DefaultConcstructor_ExpectedValues()
        {
            // Call
            var converter = new SelectedVertexLineWidthConverter();

            // Assert
            Assert.IsInstanceOf<IValueConverter>(converter);
        }

        [Test]
        public void Convert_ValueNull_ReturnNull()
        {
            // Setup
            var converter = new SelectedVertexLineWidthConverter();

            // Call
            object convertedValue = converter.Convert(null, typeof(int), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.IsNull(convertedValue);
        }

        [Test]
        public void Convert_IntValue_ReturnIntValue()
        {
            // Setup
            var converter = new SelectedVertexLineWidthConverter();

            // Call
            var convertedValue = (int) converter.Convert(2, typeof(int), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.AreEqual(3, convertedValue);
        }

        [Test]
        public void ConvertBack_ThrowNotSupportedException()
        {
            // Setup
            var converter = new SelectedVertexLineWidthConverter();

            // Call
            TestDelegate call = () => converter.ConvertBack(string.Empty, typeof(int), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }
    }
}