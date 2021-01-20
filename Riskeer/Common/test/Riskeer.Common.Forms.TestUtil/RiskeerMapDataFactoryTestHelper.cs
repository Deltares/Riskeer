// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Drawing;
using Core.Components.Gis.Style;
using NUnit.Framework;

namespace Riskeer.Common.Forms.TestUtil
{
    /// <summary>
    /// Class to help asserting drawing styles.
    /// </summary>
    public static class RiskeerMapDataFactoryTestHelper
    {
        /// <summary>
        /// Asserts the line style.
        /// </summary>
        /// <param name="lineStyle">The <see cref="LineStyle"/>to assert.</param>
        /// <param name="color">The expected <see cref="Color"/>.</param>
        /// <param name="width">The expected width of the line.</param>
        /// <param name="style">The expected <see cref="LineDashStyle"/>.</param>
        /// <exception cref="AssertionException">Thrown when one of the parameters
        /// doesn't have the expected corresponding value.</exception>
        public static void AssertEqualStyle(LineStyle lineStyle, Color color, int width, LineDashStyle style)
        {
            Assert.AreEqual(color, lineStyle.Color);
            Assert.AreEqual(width, lineStyle.Width);
            Assert.AreEqual(style, lineStyle.DashStyle);
        }

        /// <summary>
        /// Asserts the point style.
        /// </summary>
        /// <param name="pointStyle">The <see cref="PointStyle"/>to assert.</param>
        /// <param name="color">The expected <see cref="Color"/>.</param>
        /// <param name="width">The expected width of the point.</param>
        /// <param name="symbol">The expected <see cref="PointSymbol"/>.</param>
        /// <exception cref="AssertionException">Thrown when one of the parameters
        /// doesn't have the expected corresponding value.</exception>
        public static void AssertEqualStyle(PointStyle pointStyle, Color color, int width, PointSymbol symbol)
        {
            Assert.AreEqual(color, pointStyle.Color);
            Assert.AreEqual(width, pointStyle.Size);
            Assert.AreEqual(symbol, pointStyle.Symbol);
        }
    }
}