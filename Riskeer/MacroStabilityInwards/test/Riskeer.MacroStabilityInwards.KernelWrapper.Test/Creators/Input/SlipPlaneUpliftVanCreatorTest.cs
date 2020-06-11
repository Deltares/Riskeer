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
using Core.Common.TestUtil;
using Deltares.MacroStability.Data;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Input;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Test.Creators.Input
{
    [TestFixture]
    public class SlipPlaneUpliftVanCreatorTest
    {
        [Test]
        public void Create_SlipPlaneNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => SlipPlaneUpliftVanCreator.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("slipPlane", exception.ParamName);
        }

        [Test]
        public void Create_WithInput_ReturnSlipPlaneUpliftVan()
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
            double tangentLineZTop = random.NextDouble(2.0, 3.0);
            double tangentLineZBottom = random.NextDouble(0.0, 1.0);
            int tangentLineNumber = random.Next();

            var leftGrid = new UpliftVanGrid(leftGridXLeft, leftGridXRight, leftGridZTop, leftGridZBottom, leftGridXNumber, leftGridZNumber);
            var rightGrid = new UpliftVanGrid(rightGridXLeft, rightGridXRight, rightGridZTop, rightGridZBottom, rightGridXNumber, rightGridZNumber);
            var slipPlane = new UpliftVanSlipPlane(leftGrid, rightGrid, tangentLineZTop, tangentLineZBottom, tangentLineNumber);

            // Call
            SlipPlaneUpliftVan slipPlaneUpliftVan = SlipPlaneUpliftVanCreator.Create(slipPlane);

            // Assert
            Assert.AreEqual(leftGridXLeft, slipPlaneUpliftVan.SlipPlaneLeftGrid.GridXLeft);
            Assert.AreEqual(leftGridXRight, slipPlaneUpliftVan.SlipPlaneLeftGrid.GridXRight);
            Assert.AreEqual(leftGridZTop, slipPlaneUpliftVan.SlipPlaneLeftGrid.GridZTop);
            Assert.AreEqual(leftGridZBottom, slipPlaneUpliftVan.SlipPlaneLeftGrid.GridZBottom);
            Assert.AreEqual(leftGridXNumber, slipPlaneUpliftVan.SlipPlaneLeftGrid.GridXNumber);
            Assert.AreEqual(leftGridZNumber, slipPlaneUpliftVan.SlipPlaneLeftGrid.GridZNumber);
            Assert.AreEqual(rightGridXLeft, slipPlaneUpliftVan.SlipPlaneRightGrid.GridXLeft);
            Assert.AreEqual(rightGridXRight, slipPlaneUpliftVan.SlipPlaneRightGrid.GridXRight);
            Assert.AreEqual(rightGridZTop, slipPlaneUpliftVan.SlipPlaneRightGrid.GridZTop);
            Assert.AreEqual(rightGridZBottom, slipPlaneUpliftVan.SlipPlaneRightGrid.GridZBottom);
            Assert.AreEqual(rightGridXNumber, slipPlaneUpliftVan.SlipPlaneRightGrid.GridXNumber);
            Assert.AreEqual(rightGridZNumber, slipPlaneUpliftVan.SlipPlaneRightGrid.GridZNumber);
            Assert.AreEqual(tangentLineZTop, slipPlaneUpliftVan.SlipPlaneTangentLine.TangentLineZTop);
            Assert.AreEqual(tangentLineZBottom, slipPlaneUpliftVan.SlipPlaneTangentLine.TangentLineZBottom);
            Assert.AreEqual(tangentLineNumber, slipPlaneUpliftVan.SlipPlaneTangentLine.TangentLineNumber);
            Assert.AreSame(slipPlaneUpliftVan, slipPlaneUpliftVan.SlipCircleTangentLine.TangentLinesBoundaries); // Automatically synced
            Assert.AreSame(slipPlaneUpliftVan, slipPlaneUpliftVan.SlipPlaneTangentLine.TangentLinesBoundaries); // Automatically synced
        }

        [Test]
        public void Create_SlipPlaneGridsAutomatic_ReturnSlipPlaneUpliftVan()
        {
            // Setup
            var slipPlane = new UpliftVanSlipPlane();

            // Call
            SlipPlaneUpliftVan slipPlaneUpliftVan = SlipPlaneUpliftVanCreator.Create(slipPlane);

            // Assert
            Assert.IsNotNull(slipPlaneUpliftVan.SlipPlaneLeftGrid);
            Assert.AreEqual(0, slipPlaneUpliftVan.SlipPlaneLeftGrid.GridXLeft);
            Assert.AreEqual(0, slipPlaneUpliftVan.SlipPlaneLeftGrid.GridXRight);
            Assert.AreEqual(0, slipPlaneUpliftVan.SlipPlaneLeftGrid.GridZTop);
            Assert.AreEqual(0, slipPlaneUpliftVan.SlipPlaneLeftGrid.GridZBottom);
            Assert.AreEqual(0, slipPlaneUpliftVan.SlipPlaneLeftGrid.GridXNumber);
            Assert.AreEqual(0, slipPlaneUpliftVan.SlipPlaneLeftGrid.GridZNumber);
            Assert.IsNotNull(slipPlaneUpliftVan.SlipPlaneRightGrid);
            Assert.AreEqual(0, slipPlaneUpliftVan.SlipPlaneRightGrid.GridXLeft);
            Assert.AreEqual(0, slipPlaneUpliftVan.SlipPlaneRightGrid.GridXRight);
            Assert.AreEqual(0, slipPlaneUpliftVan.SlipPlaneRightGrid.GridZTop);
            Assert.AreEqual(0, slipPlaneUpliftVan.SlipPlaneRightGrid.GridZBottom);
            Assert.AreEqual(0, slipPlaneUpliftVan.SlipPlaneRightGrid.GridXNumber);
            Assert.AreEqual(0, slipPlaneUpliftVan.SlipPlaneRightGrid.GridZNumber);
            Assert.IsNotNull(slipPlaneUpliftVan.SlipPlaneTangentLine);
            Assert.AreEqual(0, slipPlaneUpliftVan.SlipPlaneTangentLine.TangentLineZTop);
            Assert.AreEqual(0, slipPlaneUpliftVan.SlipPlaneTangentLine.TangentLineZBottom);
            Assert.AreEqual(1, slipPlaneUpliftVan.SlipPlaneTangentLine.TangentLineNumber);
        }

        [Test]
        public void Create_SlipPlaneTangentLinesAutomaticAtBoundaries_ReturnSlipPlaneUpliftVan()
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
            SlipPlaneUpliftVan slipPlaneUpliftVan = SlipPlaneUpliftVanCreator.Create(slipPlane);

            // Assert
            Assert.AreEqual(leftGridXLeft, slipPlaneUpliftVan.SlipPlaneLeftGrid.GridXLeft);
            Assert.AreEqual(leftGridXRight, slipPlaneUpliftVan.SlipPlaneLeftGrid.GridXRight);
            Assert.AreEqual(leftGridZTop, slipPlaneUpliftVan.SlipPlaneLeftGrid.GridZTop);
            Assert.AreEqual(leftGridZBottom, slipPlaneUpliftVan.SlipPlaneLeftGrid.GridZBottom);
            Assert.AreEqual(leftGridXNumber, slipPlaneUpliftVan.SlipPlaneLeftGrid.GridXNumber);
            Assert.AreEqual(leftGridZNumber, slipPlaneUpliftVan.SlipPlaneLeftGrid.GridZNumber);
            Assert.AreEqual(rightGridXLeft, slipPlaneUpliftVan.SlipPlaneRightGrid.GridXLeft);
            Assert.AreEqual(rightGridXRight, slipPlaneUpliftVan.SlipPlaneRightGrid.GridXRight);
            Assert.AreEqual(rightGridZTop, slipPlaneUpliftVan.SlipPlaneRightGrid.GridZTop);
            Assert.AreEqual(rightGridZBottom, slipPlaneUpliftVan.SlipPlaneRightGrid.GridZBottom);
            Assert.AreEqual(rightGridXNumber, slipPlaneUpliftVan.SlipPlaneRightGrid.GridXNumber);
            Assert.AreEqual(rightGridZNumber, slipPlaneUpliftVan.SlipPlaneRightGrid.GridZNumber);
            Assert.AreEqual(0, slipPlaneUpliftVan.SlipPlaneTangentLine.TangentLineZTop);
            Assert.AreEqual(0, slipPlaneUpliftVan.SlipPlaneTangentLine.TangentLineZBottom);
            Assert.AreEqual(1, slipPlaneUpliftVan.SlipPlaneTangentLine.TangentLineNumber);
            Assert.AreSame(slipPlaneUpliftVan, slipPlaneUpliftVan.SlipCircleTangentLine.TangentLinesBoundaries); // Automatically synced
            Assert.AreSame(slipPlaneUpliftVan, slipPlaneUpliftVan.SlipPlaneTangentLine.TangentLinesBoundaries); // Automatically synced
        }
    }
}