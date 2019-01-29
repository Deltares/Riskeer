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

using System.ComponentModel;
using System.Drawing.Drawing2D;
using Core.Common.TestUtil;
using Core.Components.DotSpatial.Converter;
using Core.Components.Gis.Style;
using DotSpatial.Symbology;
using NUnit.Framework;

namespace Core.Components.DotSpatial.Test.Converter
{
    [TestFixture]
    public class MapDataHelperTest
    {
        [Test]
        [TestCase(PointSymbol.Circle, PointShape.Ellipse)]
        [TestCase(PointSymbol.Square, PointShape.Rectangle)]
        [TestCase(PointSymbol.Triangle, PointShape.Triangle)]
        [TestCase(PointSymbol.Diamond, PointShape.Diamond)]
        [TestCase(PointSymbol.Star, PointShape.Star)]
        [TestCase(PointSymbol.Hexagon, PointShape.Hexagon)]
        [TestCase(PointSymbol.Pentagon, PointShape.Pentagon)]
        public void Convert_ValidPointSymbol_ReturnExpectedShape(PointSymbol pointSymbol,
                                                                 PointShape expectedShape)
        {
            // Call
            PointShape symbol = MapDataHelper.Convert(pointSymbol);

            // Assert
            Assert.AreEqual(expectedShape, symbol);
        }

        [Test]
        public void Convert_InvalidPointSymbol_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const int invalidValue = 100;

            // Call
            TestDelegate call = () => MapDataHelper.Convert((PointSymbol) invalidValue);

            // Assert
            string expectedMessage = $"The value of argument 'symbol' ({invalidValue}) is invalid for Enum type '{nameof(PointSymbol)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, expectedMessage).ParamName;
            Assert.AreEqual("symbol", parameterName);
        }

        [Test]
        [TestCase(LineDashStyle.Solid, DashStyle.Solid)]
        [TestCase(LineDashStyle.Dash, DashStyle.Dash)]
        [TestCase(LineDashStyle.Dot, DashStyle.Dot)]
        [TestCase(LineDashStyle.DashDot, DashStyle.DashDot)]
        [TestCase(LineDashStyle.DashDotDot, DashStyle.DashDotDot)]
        public void Convert_ValidLineDashStyle_ReturnsExpectedDashStyle(LineDashStyle lineDashStyle,
                                                                        DashStyle expectedDashStyle)
        {
            // Call
            DashStyle dashStyle = MapDataHelper.Convert(lineDashStyle);

            // Assert
            Assert.AreEqual(expectedDashStyle, dashStyle);
        }

        [Test]
        public void Convert_InvalidLineDashStyle_ThrowsInvalidEnumArgumentException()
        {
            // Setup 
            const int invalidValue = 100;

            // Call
            TestDelegate call = () => MapDataHelper.Convert((LineDashStyle) invalidValue);

            // Assert
            string expectedMessage = $"The value of argument 'dashStyle' ({invalidValue}) is invalid for Enum type '{nameof(LineDashStyle)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, expectedMessage).ParamName;
            Assert.AreEqual("dashStyle", parameterName);
        }
    }
}