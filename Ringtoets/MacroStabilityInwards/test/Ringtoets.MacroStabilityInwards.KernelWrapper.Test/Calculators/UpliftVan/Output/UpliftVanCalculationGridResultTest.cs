﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Linq;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Output;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Result;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Test.Calculators.UpliftVan.Output
{
    [TestFixture]
    public class UpliftVanCalculationGridResultTest
    {
        [Test]
        public void Constructor_LeftGridNull_ThrowsArgumentNullException()
        {
            // Setup
            UpliftVanGridResult grid = UpliftVanGridResultTestFactory.Create();

            // Call
            TestDelegate call = () => new UpliftVanCalculationGridResult(null, grid, Enumerable.Empty<double>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("leftGrid", exception.ParamName);
        }

        [Test]
        public void Constructor_RightGridNull_ThrowsArgumentNullException()
        {
            // Setup
            UpliftVanGridResult grid = UpliftVanGridResultTestFactory.Create();

            // Call
            TestDelegate call = () => new UpliftVanCalculationGridResult(grid, null, Enumerable.Empty<double>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("rightGrid", exception.ParamName);
        }

        [Test]
        public void Constructor_TangentLinesNull_ThrowsArgumentNullException()
        {
            // Setup
            UpliftVanGridResult leftGrid = UpliftVanGridResultTestFactory.Create();
            UpliftVanGridResult rightGrid = UpliftVanGridResultTestFactory.Create();

            // Call
            TestDelegate call = () => new UpliftVanCalculationGridResult(leftGrid, rightGrid, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("tangentLines", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            UpliftVanGridResult leftGrid = UpliftVanGridResultTestFactory.Create();
            UpliftVanGridResult rightGrid = UpliftVanGridResultTestFactory.Create();
            var tangentLines = new[]
            {
                0,
                1,
                1.5,
                2
            };

            // Call
            var result = new UpliftVanCalculationGridResult(leftGrid, rightGrid, tangentLines);

            // Assert
            Assert.AreSame(leftGrid, result.LeftGrid);
            Assert.AreSame(rightGrid, result.RightGrid);
            CollectionAssert.AreEqual(tangentLines, result.TangentLines);
        }
    }
}