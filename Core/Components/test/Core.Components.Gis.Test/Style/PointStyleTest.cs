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
using System.Drawing;
using Core.Common.TestUtil;
using Core.Components.Gis.Style;
using NUnit.Framework;

namespace Core.Components.Gis.Test.Style
{
    [TestFixture]
    public class PointStyleTest
    {
        [Test]
        public void Properties_SetValues_PropertyValuesAsExpected()
        {
            // Setup
            Color color = Color.AliceBlue;
            const int width = 3;
            const PointSymbol style = PointSymbol.Square;

            // Call
            var pointStyle = new PointStyle
            {
                Color = color,
                Size = width,
                Symbol = style,
                StrokeColor = color,
                StrokeThickness = 1
            };

            // Assert
            Assert.AreEqual(color, pointStyle.Color);
            Assert.AreEqual(width, pointStyle.Size);
            Assert.AreEqual(style, pointStyle.Symbol);
            Assert.AreEqual(color, pointStyle.StrokeColor);
            Assert.AreEqual(1, pointStyle.StrokeThickness);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(-10)]
        [TestCase(49)]
        [TestCase(501)]
        [TestCase(int.MaxValue)]
        [TestCase(int.MinValue)]
        public void Size_SetInvalidValue_ThrowsArgumentOutOfRangeException(int invalidValue)
        {
            // Setup
            var pointStyle = new PointStyle();

            // Call
            TestDelegate test = () => pointStyle.Size = invalidValue;

            // Assert
            const string message = "De waarde voor grootte moet in het bereik [0, 48] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, message);
        }

        [Test]
        [TestCase(0)]
        [TestCase(10)]
        [TestCase(48)]
        public void Size_SetValidValue_ValueSet(int validValue)
        {
            // Setup
            var pointStyle = new PointStyle();

            // Call
            pointStyle.Size = validValue;

            // Assert
            Assert.AreEqual(validValue, pointStyle.Size);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(-10)]
        [TestCase(49)]
        [TestCase(501)]
        [TestCase(int.MaxValue)]
        [TestCase(int.MinValue)]
        public void StrokeThickness_SetInvalidValue_ThrowsArgumentOutOfRangeException(int invalidValue)
        {
            // Setup
            var pointStyle = new PointStyle();

            // Call
            TestDelegate test = () => pointStyle.StrokeThickness = invalidValue;

            // Assert
            const string message = "De waarde voor lijndikte moet in het bereik [0, 48] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, message);
        }

        [Test]
        [TestCase(0)]
        [TestCase(10)]
        [TestCase(48)]
        public void StrokeThickness_SetValidValue_ValueSet(int validValue)
        {
            // Setup
            var pointStyle = new PointStyle();

            // Call
            pointStyle.StrokeThickness = validValue;

            // Assert
            Assert.AreEqual(validValue, pointStyle.StrokeThickness);
        }
    }
}