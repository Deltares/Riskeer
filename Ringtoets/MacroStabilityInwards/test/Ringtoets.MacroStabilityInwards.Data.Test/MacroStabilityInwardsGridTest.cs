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
using Core.Common.Data.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;

namespace Ringtoets.MacroStabilityInwards.Data.Test
{
    [TestFixture]
    public class MacroStabilityInwardsGridTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var grid = new MacroStabilityInwardsGrid(double.NaN, double.NaN, double.NaN, double.NaN);

            // Assert
            Assert.IsInstanceOf<ICloneable>(grid);

            Assert.IsNaN(grid.XLeft);
            Assert.AreEqual(2, grid.XLeft.NumberOfDecimalPlaces);

            Assert.IsNaN(grid.XRight);
            Assert.AreEqual(2, grid.XRight.NumberOfDecimalPlaces);

            Assert.IsNaN(grid.ZTop);
            Assert.AreEqual(2, grid.ZTop.NumberOfDecimalPlaces);

            Assert.IsNaN(grid.ZBottom);
            Assert.AreEqual(2, grid.ZBottom.NumberOfDecimalPlaces);

            Assert.AreEqual(0, grid.NumberOfHorizontalPoints);
            Assert.AreEqual(0, grid.NumberOfVerticalPoints);
        }

        [Test]
        public void Constructor_SetProperties_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            double xLeft = random.NextDouble();
            double xRight = 1 + random.NextDouble();
            double zTop = 1 + random.NextDouble();
            double zBottom = random.NextDouble();
            int numberOfHorizontalPoints = random.Next(1, 100);
            int numberOfVerticalPoints = random.Next(1, 100);

            // call
            var grid = new MacroStabilityInwardsGrid(double.NaN, double.NaN, double.NaN, double.NaN)
            {
                XLeft = (RoundedDouble) xLeft,
                XRight = (RoundedDouble) xRight,
                ZTop = (RoundedDouble) zTop,
                ZBottom = (RoundedDouble) zBottom,
                NumberOfHorizontalPoints = numberOfHorizontalPoints,
                NumberOfVerticalPoints = numberOfVerticalPoints
            };

            // Assert
            Assert.AreEqual(2, grid.XLeft.NumberOfDecimalPlaces);
            Assert.AreEqual(xLeft, grid.XLeft,
                            grid.XLeft.GetAccuracy());

            Assert.AreEqual(2, grid.XRight.NumberOfDecimalPlaces);
            Assert.AreEqual(xRight, grid.XRight,
                            grid.XRight.GetAccuracy());

            Assert.AreEqual(2, grid.ZTop.NumberOfDecimalPlaces);
            Assert.AreEqual(zTop, grid.ZTop,
                            grid.ZTop.GetAccuracy());

            Assert.AreEqual(2, grid.ZBottom.NumberOfDecimalPlaces);
            Assert.AreEqual(zBottom, grid.ZBottom,
                            grid.ZBottom.GetAccuracy());

            Assert.AreEqual(numberOfHorizontalPoints, grid.NumberOfHorizontalPoints);
            Assert.AreEqual(numberOfVerticalPoints, grid.NumberOfVerticalPoints);
        }

        [Test]
        public void Clone_Always_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var random = new Random(21);
            var original = new MacroStabilityInwardsGrid(random.NextDouble(),
                                                         1 + random.NextDouble(),
                                                         1 + random.NextDouble(),
                                                         random.NextDouble())
            {
                NumberOfHorizontalPoints = random.Next(1, 100),
                NumberOfVerticalPoints = random.Next(1, 100)
            };

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, MacroStabilityInwardsCloneAssert.AreClones);
        }
    }
}