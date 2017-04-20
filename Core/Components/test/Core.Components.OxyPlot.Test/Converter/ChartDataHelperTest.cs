// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using Core.Components.Charting.Styles;
using Core.Components.OxyPlot.Converter;
using NUnit.Framework;
using OxyPlot;

namespace Core.Components.OxyPlot.Test.Converter
{
    [TestFixture]
    public class ChartDataHelperTest
    {
        #region Convert Color

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

        #endregion

        #region Convert DashStyle

        [Test]
        public void Convert_Solid_ReturnsSolid()
        {
            // Call
            LineStyle lineStyle = ChartDataHelper.Convert(DashStyle.Solid);

            // Assert
            Assert.AreEqual(LineStyle.Solid, lineStyle);
        }

        [Test]
        public void Convert_Dash_ReturnsDash()
        {
            // Call
            LineStyle lineStyle = ChartDataHelper.Convert(DashStyle.Dash);

            // Assert
            Assert.AreEqual(LineStyle.Dash, lineStyle);
        }

        [Test]
        public void Convert_Dot_ReturnsDot()
        {
            // Call
            LineStyle lineStyle = ChartDataHelper.Convert(DashStyle.Dot);

            // Assert
            Assert.AreEqual(LineStyle.Dot, lineStyle);
        }

        [Test]
        public void Convert_DashDot_ReturnsDashDot()
        {
            // Call
            LineStyle lineStyle = ChartDataHelper.Convert(DashStyle.DashDot);

            // Assert
            Assert.AreEqual(LineStyle.DashDot, lineStyle);
        }

        [Test]
        public void Convert_DashDotDot_ReturnsDashDotDot()
        {
            // Call
            LineStyle lineStyle = ChartDataHelper.Convert(DashStyle.DashDotDot);

            // Assert
            Assert.AreEqual(LineStyle.DashDotDot, lineStyle);
        }

        [Test]
        public void Convert_Custom_ThrowsInvalidEnumArgumentException()
        {
            // Call
            TestDelegate call = () => ChartDataHelper.Convert(DashStyle.Custom);

            // Assert
            Assert.Throws<InvalidEnumArgumentException>(call);
        }

        #endregion

        #region Convert ChartPointSymbol

        [Test]
        public void Convert_None_ReturnsNone()
        {
            // Call
            MarkerType markerType = ChartDataHelper.Convert(ChartPointSymbol.None);

            // Assert
            Assert.AreEqual(MarkerType.None, markerType);
        }

        [Test]
        public void Convert_Circle_ReturnsCircle()
        {
            // Call
            MarkerType markerType = ChartDataHelper.Convert(ChartPointSymbol.Circle);

            // Assert
            Assert.AreEqual(MarkerType.Circle, markerType);
        }

        [Test]
        public void Convert_Square_ReturnsSquare()
        {
            // Call
            MarkerType markerType = ChartDataHelper.Convert(ChartPointSymbol.Square);

            // Assert
            Assert.AreEqual(MarkerType.Square, markerType);
        }

        [Test]
        public void Convert_Diamond_ReturnsDiamond()
        {
            // Call
            MarkerType markerType = ChartDataHelper.Convert(ChartPointSymbol.Diamond);

            // Assert
            Assert.AreEqual(MarkerType.Diamond, markerType);
        }

        [Test]
        public void Convert_Triangle_ReturnsTriangle()
        {
            // Call
            MarkerType markerType = ChartDataHelper.Convert(ChartPointSymbol.Triangle);

            // Assert
            Assert.AreEqual(MarkerType.Triangle, markerType);
        }

        [Test]
        public void Convert_UnknownChartPointSymbol_ThrowsInvalidEnumArgumentException()
        {
            // Call
            TestDelegate call = () => ChartDataHelper.Convert((ChartPointSymbol) 10);

            // Assert
            Assert.Throws<InvalidEnumArgumentException>(call);
        }

        #endregion
    }
}