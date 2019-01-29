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
using System.Linq;
using Deltares.WTIStability;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Output;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Creators.Output;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Test.Creators.Output
{
    [TestFixture]
    public class UpliftVanCalculationGridResultCreatorTest
    {
        [Test]
        public void Create_SlipPlaneUpliftVanNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => UpliftVanCalculationGridResultCreator.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("slipPlaneUpliftVan", exception.ParamName);
        }

        [Test]
        public void Create_WithSlipPlaneUpliftVan_ReturnUpliftVanCalculationGridResult()
        {
            // Setup
            var random = new Random(21);
            double leftGridXLeft = random.Next();
            double leftGridXRight = random.Next();
            double leftGridZTop = random.Next();
            double leftGridZBottom = random.Next();
            int leftGridHorizontalPoints = random.Next();
            int leftGridVerticalPoints = random.Next();

            double rightGridXLeft = random.Next();
            double rightGridXRight = random.Next();
            double rightGridZTop = random.Next();
            double rightGridZBottom = random.Next();
            int rightGridHorizontalPoints = random.Next();
            int rightGridVerticalPoints = random.Next();

            double tangentLine1 = random.Next();
            double tangentLine2 = random.Next();

            var slipPlaneUpliftVan = new SlipPlaneUpliftVan
            {
                SlipPlaneLeftGrid = new SlipCircleGrid
                {
                    GridXLeft = leftGridXLeft,
                    GridXRight = leftGridXRight,
                    GridZTop = leftGridZTop,
                    GridZBottom = leftGridZBottom,
                    GridXNumber = leftGridHorizontalPoints,
                    GridZNumber = leftGridVerticalPoints
                },
                SlipPlaneRightGrid = new SlipCircleGrid
                {
                    GridXLeft = rightGridXLeft,
                    GridXRight = rightGridXRight,
                    GridZTop = rightGridZTop,
                    GridZBottom = rightGridZBottom,
                    GridXNumber = rightGridHorizontalPoints,
                    GridZNumber = rightGridVerticalPoints
                },
                SlipPlaneTangentLine = new SlipCircleTangentLine
                {
                    BoundaryHeights =
                    {
                        new TangentLine(tangentLine1),
                        new TangentLine(tangentLine2)
                    }
                }
            };

            // Call
            UpliftVanCalculationGridResult result = UpliftVanCalculationGridResultCreator.Create(slipPlaneUpliftVan);

            // Assert
            AssertGrid(slipPlaneUpliftVan.SlipPlaneLeftGrid, result.LeftGrid);
            AssertGrid(slipPlaneUpliftVan.SlipPlaneRightGrid, result.RightGrid);
            CollectionAssert.AreEqual(slipPlaneUpliftVan.SlipPlaneTangentLine.BoundaryHeights.Select(sl => sl.Height), result.TangentLines);
        }

        private static void AssertGrid(SlipCircleGrid originalGrid, UpliftVanGrid actualGrid)
        {
            Assert.AreEqual(originalGrid.GridXLeft, actualGrid.XLeft);
            Assert.AreEqual(originalGrid.GridXRight, actualGrid.XRight);
            Assert.AreEqual(originalGrid.GridZTop, actualGrid.ZTop);
            Assert.AreEqual(originalGrid.GridZBottom, actualGrid.ZBottom);
            Assert.AreEqual(originalGrid.GridXNumber, actualGrid.NumberOfHorizontalPoints);
            Assert.AreEqual(originalGrid.GridZNumber, actualGrid.NumberOfVerticalPoints);
        }
    }
}