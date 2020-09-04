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

using System;
using Deltares.MacroStability.CSharpWrapper;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Input;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Test.Creators.Input
{
    [TestFixture]
    public class UpliftVanCalculationGridCreatorTest
    {
        [Test]
        public void Create_SlipPlaneNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => UpliftVanCalculationGridCreator.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("slipPlane", exception.ParamName);
        }

        [Test]
        public void Create_WithInput_ReturnUpliftVanCalculationGrid()
        {
            // Setup
            var random = new Random(21);
            double leftGridXLeft = random.NextDouble();
            double leftGridXRight = random.NextDouble();
            double leftGridZTop = random.NextDouble();
            double leftGridZBottom = random.NextDouble();
            int leftGridXNumber = random.Next();
            int leftGridZNumber = random.Next();
            double rightGridXLeft = random.NextDouble();
            double rightGridXRight = random.NextDouble();
            double rightGridZTop = random.NextDouble();
            double rightGridZBottom = random.NextDouble();
            int rightGridXNumber = random.Next();
            int rightGridZNumber = random.Next();

            var leftGrid = new UpliftVanGrid(leftGridXLeft, leftGridXRight, leftGridZTop, leftGridZBottom, leftGridXNumber, leftGridZNumber);
            var rightGrid = new UpliftVanGrid(rightGridXLeft, rightGridXRight, rightGridZTop, rightGridZBottom, rightGridXNumber, rightGridZNumber);
            var slipPlane = new UpliftVanSlipPlane(leftGrid, rightGrid);

            // Call
            UpliftVanCalculationGrid upliftVanCalculationGrid = UpliftVanCalculationGridCreator.Create(slipPlane);

            // Assert
            Assert.AreEqual(leftGridXLeft, upliftVanCalculationGrid.LeftGrid.GridXLeft);
            Assert.AreEqual(leftGridXRight, upliftVanCalculationGrid.LeftGrid.GridXRight);
            Assert.AreEqual(leftGridZTop, upliftVanCalculationGrid.LeftGrid.GridZTop);
            Assert.AreEqual(leftGridZBottom, upliftVanCalculationGrid.LeftGrid.GridZBottom);
            Assert.AreEqual(leftGridXNumber, upliftVanCalculationGrid.LeftGrid.GridXNumber);
            Assert.AreEqual(leftGridZNumber, upliftVanCalculationGrid.LeftGrid.GridZNumber);
            Assert.AreEqual(rightGridXLeft, upliftVanCalculationGrid.RightGrid.GridXLeft);
            Assert.AreEqual(rightGridXRight, upliftVanCalculationGrid.RightGrid.GridXRight);
            Assert.AreEqual(rightGridZTop, upliftVanCalculationGrid.RightGrid.GridZTop);
            Assert.AreEqual(rightGridZBottom, upliftVanCalculationGrid.RightGrid.GridZBottom);
            Assert.AreEqual(rightGridXNumber, upliftVanCalculationGrid.RightGrid.GridXNumber);
            Assert.AreEqual(rightGridZNumber, upliftVanCalculationGrid.RightGrid.GridZNumber);
            CollectionAssert.IsEmpty(upliftVanCalculationGrid.TangentLines);
        }

        [Test]
        public void Create_SlipPlaneGridsAutomatic_ReturnSlipPlaneUpliftVan()
        {
            // Setup
            var slipPlane = new UpliftVanSlipPlane();

            // Call
            UpliftVanCalculationGrid upliftVanCalculationGrid = UpliftVanCalculationGridCreator.Create(slipPlane);

            // Assert
            Assert.IsNotNull(upliftVanCalculationGrid.LeftGrid);
            Assert.AreEqual(0, upliftVanCalculationGrid.LeftGrid.GridXLeft);
            Assert.AreEqual(0, upliftVanCalculationGrid.LeftGrid.GridXRight);
            Assert.AreEqual(0, upliftVanCalculationGrid.LeftGrid.GridZTop);
            Assert.AreEqual(0, upliftVanCalculationGrid.LeftGrid.GridZBottom);
            Assert.AreEqual(0, upliftVanCalculationGrid.LeftGrid.GridXNumber);
            Assert.AreEqual(0, upliftVanCalculationGrid.LeftGrid.GridZNumber);
            Assert.IsNotNull(upliftVanCalculationGrid.RightGrid);
            Assert.AreEqual(0, upliftVanCalculationGrid.RightGrid.GridXLeft);
            Assert.AreEqual(0, upliftVanCalculationGrid.RightGrid.GridXRight);
            Assert.AreEqual(0, upliftVanCalculationGrid.RightGrid.GridZTop);
            Assert.AreEqual(0, upliftVanCalculationGrid.RightGrid.GridZBottom);
            Assert.AreEqual(0, upliftVanCalculationGrid.RightGrid.GridXNumber);
            Assert.AreEqual(0, upliftVanCalculationGrid.RightGrid.GridZNumber);
            CollectionAssert.IsEmpty(upliftVanCalculationGrid.TangentLines);
        }
    }
}