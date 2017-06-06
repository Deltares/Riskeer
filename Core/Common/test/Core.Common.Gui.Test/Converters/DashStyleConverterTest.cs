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
using System.Drawing.Drawing2D;
using System.Globalization;
using Core.Common.Gui.Converters;
using Core.Common.Utils;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test.Converters
{
    [TestFixture]
    public class DashStyleConverterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var converter = new DashStyleConverter(typeof(DashStyle));

            // Assert
            Assert.IsInstanceOf<EnumTypeConverter>(converter);
        }

        [Test]
        public void ConvertTo_ValueNoDashStyle_ThrowNotSupportedException()
        {
            // Setup
            var mocks = new MockRepository();
            var context = mocks.Stub<ITypeDescriptorContext>();
            mocks.ReplayAll();

            var originalValue = new object();
            var converter = new DashStyleConverter(typeof(DashStyle));

            // Call
            TestDelegate test = () => converter.ConvertTo(context, CultureInfo.InvariantCulture, originalValue, typeof(DashStyle));

            // Assert
            Assert.Throws<NotSupportedException>(test);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(DashStyle.Solid, "Doorgetrokken")]
        [TestCase(DashStyle.Dash, "Onderbroken")]
        [TestCase(DashStyle.Dot, "Gestippeld")]
        [TestCase(DashStyle.DashDot, "Streep-stip")]
        [TestCase(DashStyle.DashDotDot, "Streep-stip-stip")]
        public void ConvertTo_ValueValidDashStyle_ReturnStringValue(DashStyle originalValue, string expectedConvertedValue)
        {
            // Setup
            var mocks = new MockRepository();
            var context = mocks.Stub<ITypeDescriptorContext>();
            mocks.ReplayAll();

            var converter = new DashStyleConverter(typeof(DashStyle));

            // Call
            object converterdValue = converter.ConvertTo(context, CultureInfo.InvariantCulture, originalValue, typeof(DashStyle));

            // Assert
            Assert.AreEqual(expectedConvertedValue, converterdValue);
            mocks.VerifyAll();
        }

        [Test]
        public void ConvertTo_InvalidDashStyleValue_ThrowArgumentOutOfRangeException()
        {
            // Setup
            var mocks = new MockRepository();
            var context = mocks.Stub<ITypeDescriptorContext>();
            mocks.ReplayAll();

            const DashStyle originalValue = (DashStyle) 100;
            var converter = new DashStyleConverter(typeof(DashStyle));

            // Call
            TestDelegate test = () => converter.ConvertTo(context, CultureInfo.InvariantCulture, originalValue, typeof(DashStyle));

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(test);
            mocks.VerifyAll();
        }

        [Test]
        public void ConvertFrom_ValueNoString_ThrowNotSupportedException()
        {
            // Setup
            var mocks = new MockRepository();
            var context = mocks.Stub<ITypeDescriptorContext>();
            mocks.ReplayAll();

            var originalValue = new object();
            var converter = new DashStyleConverter(typeof(DashStyle));

            // Call
            TestDelegate test = () => converter.ConvertFrom(context, CultureInfo.InvariantCulture, originalValue);

            // Assert
            Assert.Throws<NotSupportedException>(test);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase("Doorgetrokken", DashStyle.Solid)]
        [TestCase("Onderbroken", DashStyle.Dash)]
        [TestCase("Gestippeld", DashStyle.Dot)]
        [TestCase("Streep-stip", DashStyle.DashDot)]
        [TestCase("Streep-stip-stip", DashStyle.DashDotDot)]
        public void ConvertFrom_ValidString_ReturnDashStyle(string stringValue, DashStyle expectedDashStyle)
        {
            // Setup
            var mocks = new MockRepository();
            var context = mocks.Stub<ITypeDescriptorContext>();
            mocks.ReplayAll();

            var converter = new DashStyleConverter(typeof(DashStyle));

            // Call
            object converterdValue = converter.ConvertFrom(context, CultureInfo.InvariantCulture, stringValue);

            // Assert
            Assert.AreEqual(expectedDashStyle, converterdValue);
            mocks.VerifyAll();
        }

        [Test]
        public void ConvertFrom_InvalidStringValue_ThrowFormatException()
        {
            // Setup
            var mocks = new MockRepository();
            var context = mocks.Stub<ITypeDescriptorContext>();
            mocks.ReplayAll();

            const string originalValue = "test";
            var converter = new DashStyleConverter(typeof(DashStyle));

            // Call
            TestDelegate test = () => converter.ConvertFrom(context, CultureInfo.InvariantCulture, originalValue);

            // Assert
            Assert.Throws<FormatException>(test);
            mocks.VerifyAll();
        }
    }
}