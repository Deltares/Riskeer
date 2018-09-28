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
    public class ChartAreaStyleTest
    {
        [Test]
        public void Constructor_WithAllParameters_SetsProperties()
        {
            // Setup
            Color fillColor = Color.AliceBlue;
            Color strokeColor = Color.Blue;
            const int width = 3;

            // Call
            var areaStyle = new ChartAreaStyle
            {
                FillColor = fillColor,
                StrokeColor = strokeColor,
                StrokeThickness = width,
                IsEditable = true
            };

            // Assert
            Assert.AreEqual(fillColor, areaStyle.FillColor);
            Assert.AreEqual(strokeColor, areaStyle.StrokeColor);
            Assert.AreEqual(width, areaStyle.StrokeThickness);
            Assert.IsTrue(areaStyle.IsEditable);
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
            var polygonStyle = new ChartAreaStyle();

            // Call
            TestDelegate test = () => polygonStyle.StrokeThickness = invalidValue;

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
            var polygonStyle = new ChartAreaStyle();

            // Call
            polygonStyle.StrokeThickness = validValue;

            // Assert
            Assert.AreEqual(validValue, polygonStyle.StrokeThickness);
        }
    }
}