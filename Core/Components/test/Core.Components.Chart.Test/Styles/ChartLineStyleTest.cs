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
    public class ChartLineStyleTest
    {
        [Test]
        public void Constructor_WithStyle_SetsProperties()
        {
            // Setup
            Color color = Color.AliceBlue;
            const int width = 3;
            const ChartLineDashStyle style = ChartLineDashStyle.Solid;

            // Call
            var lineStyle = new ChartLineStyle
            {
                Color = color,
                Width = width,
                DashStyle = style,
                IsEditable = true
            };

            // Assert
            Assert.AreEqual(color, lineStyle.Color);
            Assert.AreEqual(width, lineStyle.Width);
            Assert.AreEqual(style, lineStyle.DashStyle);
            Assert.IsTrue(lineStyle.IsEditable);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(-10)]
        [TestCase(49)]
        [TestCase(501)]
        [TestCase(int.MaxValue)]
        [TestCase(int.MinValue)]
        public void Width_SetInvalidValue_ThrowsArgumentOutOfRangeException(int invalidValue)
        {
            // Setup
            var lineStyle = new ChartLineStyle();

            // Call
            TestDelegate test = () => lineStyle.Width = invalidValue;

            // Assert
            const string message = "De waarde voor lijndikte moet in het bereik [0, 48] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, message);
        }

        [Test]
        [TestCase(0)]
        [TestCase(10)]
        [TestCase(48)]
        public void Width_SetValidValue_ValueSet(int validValue)
        {
            // Setup
            var lineStyle = new ChartLineStyle();

            // Call
            lineStyle.Width = validValue;

            // Assert
            Assert.AreEqual(validValue, lineStyle.Width);
        }
    }
}