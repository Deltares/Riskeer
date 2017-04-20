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

using System.Drawing;
using Core.Components.Charting.Styles;
using NUnit.Framework;

namespace Core.Components.Charting.Test.Styles
{
    [TestFixture]
    public class ChartPointStyleTest
    {
        [Test]
        public void Constructor_WithAllParameters_SetsProperties()
        {
            // Setup
            Color color = Color.AliceBlue;
            var size = 3;
            Color strokeColor = Color.AntiqueWhite;
            var strokeThickness = 2;
            var symbol = ChartPointSymbol.Circle;

            // Call
            var pointStyle = new ChartPointStyle(color, size, strokeColor, strokeThickness, symbol);

            // Assert
            Assert.AreEqual(color, pointStyle.Color);
            Assert.AreEqual(size, pointStyle.Size);
            Assert.AreEqual(strokeColor, pointStyle.StrokeColor);
            Assert.AreEqual(strokeThickness, pointStyle.StrokeThickness);
            Assert.AreEqual(symbol, pointStyle.Symbol);
        }
    }
}