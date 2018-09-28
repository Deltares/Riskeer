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
using System.Drawing;
using Core.Common.TestUtil;
using Core.Components.Chart.Styles;
using NUnit.Framework;

namespace Core.Components.Chart.Test.Styles
{
    [TestFixture]
    public class ChartPointStyleTest
    {
        [Test]
        public void Constructor_WithAllParameters_SetsProperties()
        {
            // Setup
            Color color = Color.AliceBlue;
            const int size = 3;
            Color strokeColor = Color.AntiqueWhite;
            const int strokeThickness = 2;
            const ChartPointSymbol symbol = ChartPointSymbol.Circle;

            // Call
            var pointStyle = new ChartPointStyle
            {
                Color = color,
                StrokeColor = strokeColor,
                Size = size,
                StrokeThickness = strokeThickness,
                Symbol = symbol,
                IsEditable = true
            };

            // Assert
            Assert.AreEqual(color, pointStyle.Color);
            Assert.AreEqual(size, pointStyle.Size);
            Assert.AreEqual(strokeColor, pointStyle.StrokeColor);
            Assert.AreEqual(strokeThickness, pointStyle.StrokeThickness);
            Assert.AreEqual(symbol, pointStyle.Symbol);
            Assert.IsTrue(pointStyle.IsEditable);
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
            var pointStyle = new ChartPointStyle();

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
            var pointStyle = new ChartPointStyle();

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
            var pointStyle = new ChartPointStyle();

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
            var pointStyle = new ChartPointStyle();

            // Call
            pointStyle.StrokeThickness = validValue;

            // Assert
            Assert.AreEqual(validValue, pointStyle.StrokeThickness);
        }
    }
}