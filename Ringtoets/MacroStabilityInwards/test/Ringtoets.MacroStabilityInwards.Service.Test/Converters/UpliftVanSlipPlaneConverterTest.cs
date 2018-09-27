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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Ringtoets.MacroStabilityInwards.Service.Converters;

namespace Ringtoets.MacroStabilityInwards.Service.Test.Converters
{
    [TestFixture]
    public class UpliftVanSlipPlaneConverterTest
    {
        [Test]
        public void Convert_InputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => UpliftVanSlipPlaneConverter.Convert(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("input", exception.ParamName);
        }

        [Test]
        public void Convert_MacroStabilityInwardsGridDeterminationTypeAutomatic_ReturnUpliftVanSlipPlane()
        {
            // Setup
            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties
            {
                TangentLineZTop = 1,
                TangentLineZBottom = 0
            })
            {
                GridDeterminationType = MacroStabilityInwardsGridDeterminationType.Automatic,
                TangentLineDeterminationType = MacroStabilityInwardsTangentLineDeterminationType.Specified,
                TangentLineNumber = 10
            };

            // Precondition
            Assert.IsNotNull(input.LeftGrid);
            Assert.IsNotNull(input.RightGrid);

            // Call
            UpliftVanSlipPlane slipPlane = UpliftVanSlipPlaneConverter.Convert(input);

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
        public void Convert_MacroStabilityInwardsGridDeterminationTypeManualAndTangentLineDeterminationTypeLayerSeparated_ReturnUpliftVanSlipPlane()
        {
            // Setup
            var random = new Random(11);
            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties
            {
                LeftGridXLeft = random.NextRoundedDouble(0.0, 1.0),
                LeftGridXRight = random.NextRoundedDouble(2.0, 3.0),
                LeftGridZTop = random.NextRoundedDouble(2.0, 3.0),
                LeftGridZBottom = random.NextRoundedDouble(0.0, 1.0),

                RightGridXLeft = random.NextRoundedDouble(0.0, 1.0),
                RightGridXRight = random.NextRoundedDouble(2.0, 3.0),
                RightGridZTop = random.NextRoundedDouble(2.0, 3.0),
                RightGridZBottom = random.NextRoundedDouble(0.0, 1.0),

                TangentLineZTop = 1,
                TangentLineZBottom = 0
            })
            {
                GridDeterminationType = MacroStabilityInwardsGridDeterminationType.Manual,
                LeftGrid =
                {
                    NumberOfVerticalPoints = random.Next(1, 100),
                    NumberOfHorizontalPoints = random.Next(1, 100)
                },
                RightGrid =
                {
                    NumberOfVerticalPoints = random.Next(1, 100),
                    NumberOfHorizontalPoints = random.Next(1, 100)
                },
                TangentLineDeterminationType = MacroStabilityInwardsTangentLineDeterminationType.LayerSeparated,
                TangentLineNumber = 10
            };

            // Call
            UpliftVanSlipPlane slipPlane = UpliftVanSlipPlaneConverter.Convert(input);

            // Assert
            Assert.IsFalse(slipPlane.GridAutomaticDetermined);
            AssertGrid(input.LeftGrid, slipPlane.LeftGrid);
            AssertGrid(input.RightGrid, slipPlane.RightGrid);
            Assert.IsTrue(slipPlane.TangentLinesAutomaticAtBoundaries);
            Assert.IsNaN(slipPlane.TangentZTop);
            Assert.IsNaN(slipPlane.TangentZBottom);
            Assert.AreEqual(0, slipPlane.TangentLineNumber);
        }

        [Test]
        public void Convert_MacroStabilityInwardsGridDeterminationTypeManualAndTangentLineDeterminationTypeSpecified_ReturnUpliftVanSlipPlane()
        {
            // Setup
            var random = new Random(11);
            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties
            {
                LeftGridXLeft = random.NextRoundedDouble(0.0, 1.0),
                LeftGridXRight = random.NextRoundedDouble(2.0, 3.0),
                LeftGridZTop = random.NextRoundedDouble(2.0, 3.0),
                LeftGridZBottom = random.NextRoundedDouble(0.0, 1.0),

                RightGridXLeft = random.NextRoundedDouble(0.0, 1.0),
                RightGridXRight = random.NextRoundedDouble(2.0, 3.0),
                RightGridZTop = random.NextRoundedDouble(2.0, 3.0),
                RightGridZBottom = random.NextRoundedDouble(0.0, 1.0),

                TangentLineZTop = random.NextRoundedDouble(2.0, 3.0),
                TangentLineZBottom = random.NextRoundedDouble(0.0, 1.0)
            })
            {
                GridDeterminationType = MacroStabilityInwardsGridDeterminationType.Manual,
                LeftGrid =
                {
                    NumberOfVerticalPoints = random.Next(1, 100),
                    NumberOfHorizontalPoints = random.Next(1, 100)
                },
                RightGrid =
                {
                    NumberOfVerticalPoints = random.Next(1, 100),
                    NumberOfHorizontalPoints = random.Next(1, 100)
                },
                TangentLineDeterminationType = MacroStabilityInwardsTangentLineDeterminationType.Specified,
                TangentLineNumber = random.Next(1, 51)
            };

            // Call
            UpliftVanSlipPlane slipPlane = UpliftVanSlipPlaneConverter.Convert(input);

            // Assert
            Assert.IsFalse(slipPlane.GridAutomaticDetermined);
            AssertGrid(input.LeftGrid, slipPlane.LeftGrid);
            AssertGrid(input.RightGrid, slipPlane.RightGrid);
            Assert.IsFalse(slipPlane.TangentLinesAutomaticAtBoundaries);
            Assert.AreEqual(input.TangentLineZTop, slipPlane.TangentZTop);
            Assert.AreEqual(input.TangentLineZBottom, slipPlane.TangentZBottom);
            Assert.AreEqual(input.TangentLineNumber, slipPlane.TangentLineNumber);
        }

        private static void AssertGrid(MacroStabilityInwardsGrid expectedGrid, UpliftVanGrid actualGrid)
        {
            Assert.AreEqual(expectedGrid.XLeft, actualGrid.XLeft);
            Assert.AreEqual(expectedGrid.XRight, actualGrid.XRight);
            Assert.AreEqual(expectedGrid.ZTop, actualGrid.ZTop);
            Assert.AreEqual(expectedGrid.ZBottom, actualGrid.ZBottom);
            Assert.AreEqual(expectedGrid.NumberOfHorizontalPoints, actualGrid.NumberOfHorizontalPoints);
            Assert.AreEqual(expectedGrid.NumberOfVerticalPoints, actualGrid.NumberOfVerticalPoints);
        }
    }
}