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
using System.Drawing;
using Core.Common.TestUtil;
using Core.Components.Chart.Styles;
using Core.Components.OxyPlot.Converter;
using NUnit.Framework;
using OxyPlot;

namespace Core.Components.OxyPlot.Test.Converter
{
    [TestFixture]
    public class ChartDataHelperTest
    {
        [Test]
        [TestCase(KnownColor.Blue)]
        [TestCase(KnownColor.Red)]
        [TestCase(KnownColor.Green)]
        public void Convert_Color_ReturnsOxyColor(KnownColor knownColor)
        {
            // Setup
            Color color = Color.FromKnownColor(knownColor);

            // Call
            OxyColor oxyColor = ChartDataHelper.Convert(color);

            // Assert
            OxyColor originalColor = OxyColor.FromArgb(color.A, color.R, color.G, color.B);
            Assert.AreEqual(originalColor, oxyColor);
        }

        [Test]
        [TestCase(ChartLineDashStyle.Solid, LineStyle.Solid)]
        [TestCase(ChartLineDashStyle.Dash, LineStyle.Dash)]
        [TestCase(ChartLineDashStyle.Dot, LineStyle.Dot)]
        [TestCase(ChartLineDashStyle.DashDot, LineStyle.DashDot)]
        [TestCase(ChartLineDashStyle.DashDotDot, LineStyle.DashDotDot)]
        public void Convert_ValidChartLineDashStyle_ReturnsExpectedLineStyle(ChartLineDashStyle chartLineDashStyle,
                                                                             LineStyle expectedLineStyle)
        {
            // Call
            LineStyle lineStyle = ChartDataHelper.Convert(chartLineDashStyle);

            // Assert
            Assert.AreEqual(expectedLineStyle, lineStyle);
        }

        [Test]
        public void Convert_InvalidChartLineDashStyle_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const int invalidValue = 100;

            // Call
            TestDelegate call = () => ChartDataHelper.Convert((ChartLineDashStyle) invalidValue);

            // Assert
            string expectedMessage = $"The value of argument 'dashStyle' ({invalidValue}) is invalid for Enum type '{nameof(ChartLineDashStyle)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, expectedMessage).ParamName;
            Assert.AreEqual("dashStyle", parameterName);
        }

        [Test]
        [TestCase(ChartPointSymbol.Circle, MarkerType.Circle)]
        [TestCase(ChartPointSymbol.Square, MarkerType.Square)]
        [TestCase(ChartPointSymbol.Diamond, MarkerType.Diamond)]
        [TestCase(ChartPointSymbol.Triangle, MarkerType.Triangle)]
        [TestCase(ChartPointSymbol.Star, MarkerType.Star)]
        [TestCase(ChartPointSymbol.Cross, MarkerType.Cross)]
        [TestCase(ChartPointSymbol.Plus, MarkerType.Plus)]
        public void Convert_ValidChartPointSymbol_ReturnsExpectedMarkerType(ChartPointSymbol chartPointSymbol,
                                                                            MarkerType expectedMarkerType)
        {
            // Call
            MarkerType markerType = ChartDataHelper.Convert(chartPointSymbol);

            // Assert
            Assert.AreEqual(expectedMarkerType, markerType);
        }

        [Test]
        public void Convert_InvalidChartPointSymbol_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const int invalidValue = 10;

            // Call
            TestDelegate call = () => ChartDataHelper.Convert((ChartPointSymbol) invalidValue);

            // Assert
            string expectedMessage = $"The value of argument 'symbol' ({invalidValue}) is invalid for Enum type '{nameof(ChartPointSymbol)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, expectedMessage).ParamName;
            Assert.AreEqual("symbol", parameterName);
        }
    }
}