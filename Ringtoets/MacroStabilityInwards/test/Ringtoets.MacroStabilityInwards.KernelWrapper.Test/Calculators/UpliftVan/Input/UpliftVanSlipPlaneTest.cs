// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.UpliftVan;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Test.Calculators.UpliftVan.Input
{
    [TestFixture]
    public class UpliftVanSlipPlaneTest
    {
        [Test]
        public void ParameterlessConstructor_ExpectedValues()
        {
            // Call
            var slipPlane = new UpliftVanSlipPlane();

            // Assert
            Assert.IsTrue(slipPlane.GridAutomaticDetermined);
            Assert.IsNull(slipPlane.LeftGrid);
            Assert.IsNull(slipPlane.RightGrid);
            Assert.IsTrue(slipPlane.TangentLinesAutomaticAtBoundaries);
            Assert.IsNaN(slipPlane.TangentZTop);
            Assert.IsNaN(slipPlane.TangentZBottom);
            Assert.AreEqual(0, slipPlane.TangentLineNumber);
        }

        [Test]
        public void ConstructorWithGrids_LeftGridNull_ThrowsArgumentNullException()
        {
            // Setup
            UpliftVanGrid grid = UpliftVanGridTestFactory.Create();

            // Call
            TestDelegate test = () => new UpliftVanSlipPlane(null, grid);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("leftGrid", exception.ParamName);
        }

        [Test]
        public void ConstructorWithGrids_RightGridNull_ThrowsArgumentNullException()
        {
            // Setup
            UpliftVanGrid grid = UpliftVanGridTestFactory.Create();

            // Call
            TestDelegate test = () => new UpliftVanSlipPlane(grid, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("rightGrid", exception.ParamName);
        }

        [Test]
        public void ConstructorWithGrids_ExpectedValues()
        {
            // Setup
            UpliftVanGrid leftGrid = UpliftVanGridTestFactory.Create();
            UpliftVanGrid rightGrid = UpliftVanGridTestFactory.Create();

            // Call
            var slipPlane = new UpliftVanSlipPlane(leftGrid, rightGrid);

            // Assert
            Assert.IsFalse(slipPlane.GridAutomaticDetermined);
            Assert.AreSame(leftGrid, slipPlane.LeftGrid);
            Assert.AreSame(rightGrid, slipPlane.RightGrid);
            Assert.IsTrue(slipPlane.TangentLinesAutomaticAtBoundaries);
            Assert.IsNaN(slipPlane.TangentZTop);
            Assert.IsNaN(slipPlane.TangentZBottom);
            Assert.AreEqual(0, slipPlane.TangentLineNumber);
        }

        [Test]
        public void ConstructorWithGridsAndTangent_LeftGridNull_ThrowsArgumentNullException()
        {
            // Setup
            UpliftVanGrid grid = UpliftVanGridTestFactory.Create();

            // Call
            TestDelegate test = () => new UpliftVanSlipPlane(null, grid, 0, 0, 0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("leftGrid", exception.ParamName);
        }

        [Test]
        public void ConstructorWithGridsAndTangent_RightGridNull_ThrowsArgumentNullException()
        {
            // Setup
            UpliftVanGrid grid = UpliftVanGridTestFactory.Create();

            // Call
            TestDelegate test = () => new UpliftVanSlipPlane(grid, null, 0, 0, 0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("rightGrid", exception.ParamName);
        }

        [Test]
        public void ConstructorWithGridsAndTangent_ExpectedValues()
        {
            // Setup
            var random = new Random(11);
            double tangentZTop = random.NextDouble();
            double tangentZBottom = random.NextDouble();
            int tangentLineNumber = random.Next();

            UpliftVanGrid leftGrid = UpliftVanGridTestFactory.Create();
            UpliftVanGrid rightGrid = UpliftVanGridTestFactory.Create();

            // Call
            var slipPlane = new UpliftVanSlipPlane(leftGrid, rightGrid, tangentZTop, tangentZBottom, tangentLineNumber);

            // Assert
            Assert.IsFalse(slipPlane.GridAutomaticDetermined);
            Assert.AreSame(leftGrid, slipPlane.LeftGrid);
            Assert.AreSame(rightGrid, slipPlane.RightGrid);
            Assert.IsFalse(slipPlane.TangentLinesAutomaticAtBoundaries);
            Assert.AreEqual(tangentZTop, slipPlane.TangentZTop);
            Assert.AreEqual(tangentZBottom, slipPlane.TangentZBottom);
            Assert.AreEqual(tangentLineNumber, slipPlane.TangentLineNumber);
        }
    }
}