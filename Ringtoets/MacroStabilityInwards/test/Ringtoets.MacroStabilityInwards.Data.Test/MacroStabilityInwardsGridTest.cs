// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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

using System;
using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.MacroStabilityInwards.Data.Test
{
    [TestFixture]
    public class MacroStabilityInwardsGridTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var grid = new MacroStabilityInwardsGrid();

            // Assert
            Assert.AreEqual(0, grid.XLeft, grid.XLeft.GetAccuracy());
            Assert.AreEqual(2, grid.XLeft.NumberOfDecimalPlaces);

            Assert.AreEqual(0, grid.XRight, grid.XRight.GetAccuracy());
            Assert.AreEqual(2, grid.XRight.NumberOfDecimalPlaces);

            Assert.AreEqual(0, grid.ZTop, grid.ZTop.GetAccuracy());
            Assert.AreEqual(2, grid.ZTop.NumberOfDecimalPlaces);

            Assert.AreEqual(0, grid.ZBottom, grid.ZBottom.GetAccuracy());
            Assert.AreEqual(2, grid.ZBottom.NumberOfDecimalPlaces);

            Assert.AreEqual(0, grid.NumberOfHorizontalPoints);
            Assert.AreEqual(0, grid.NumberOfVerticalPoints);
        }

        [Test]
        public void Constructor_SetProperties_ExpectedValues()
        {
            // Setup
            var random = new Random();
            double xLeft = random.Next();
            double xRight = random.Next();
            double zTop = random.Next();
            double zBottom = random.Next();

            // call
            var grid = new MacroStabilityInwardsGrid
            {
                XLeft = (RoundedDouble) xLeft,
                XRight = (RoundedDouble) xRight,
                ZTop = (RoundedDouble) zTop,
                ZBottom = (RoundedDouble) zBottom
            };

            // Assert
            Assert.AreEqual(new RoundedDouble(2, xLeft), grid.XLeft);
            Assert.AreEqual(new RoundedDouble(2, xRight), grid.XRight);
            Assert.AreEqual(new RoundedDouble(2, zTop), grid.ZTop);
            Assert.AreEqual(new RoundedDouble(2, zBottom), grid.ZBottom);
        }
    }
}