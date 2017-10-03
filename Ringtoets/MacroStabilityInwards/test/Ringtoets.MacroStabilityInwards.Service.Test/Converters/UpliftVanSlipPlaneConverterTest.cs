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
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Ringtoets.MacroStabilityInwards.Primitives;
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
            var input = new MacroStabilityInwardsInput
            {
                GridDeterminationType = MacroStabilityInwardsGridDeterminationType.Automatic,
                TangentLineDeterminationType = MacroStabilityInwardsTangentLineDeterminationType.Specified,
                TangentLineZTop = (RoundedDouble) 1,
                TangentLineZBottom = (RoundedDouble) 0
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
            Assert.IsNaN(slipPlane.MaxSpacingBetweenBoundaries);
        }

        [Test]
        public void Convert_MacroStabilityInwardsGridDeterminationTypeManualAdnTangentLineDeterminationTypeLayerSeparated_ReturnUpliftVanSlipPlane()
        {
            // Setup
            var random = new Random(11);
            var input = new MacroStabilityInwardsInput
            {
                GridDeterminationType = MacroStabilityInwardsGridDeterminationType.Manual,
                LeftGrid =
                {
                    XLeft = random.NextRoundedDouble(),
                    XRight = random.NextRoundedDouble(),
                    ZTop = random.NextRoundedDouble(),
                    ZBottom = random.NextRoundedDouble(),
                    NumberOfVerticalPoints = random.Next(),
                    NumberOfHorizontalPoints = random.Next()
                },
                RightGrid =
                {
                    XLeft = random.NextRoundedDouble(),
                    XRight = random.NextRoundedDouble(),
                    ZTop = random.NextRoundedDouble(),
                    ZBottom = random.NextRoundedDouble(),
                    NumberOfVerticalPoints = random.Next(),
                    NumberOfHorizontalPoints = random.Next()
                },
                TangentLineDeterminationType = MacroStabilityInwardsTangentLineDeterminationType.LayerSeparated,
                TangentLineZTop = (RoundedDouble) 1,
                TangentLineZBottom = (RoundedDouble) 0
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
            Assert.IsNaN(slipPlane.MaxSpacingBetweenBoundaries);
        }

        [Test]
        public void Convert_MacroStabilityInwardsGridDeterminationTypeManualAdnTangentLineDeterminationTypeSpecified_ReturnUpliftVanSlipPlane()
        {
            // Setup
            var random = new Random(11);
            var input = new MacroStabilityInwardsInput
            {
                GridDeterminationType = MacroStabilityInwardsGridDeterminationType.Manual,
                LeftGrid =
                {
                    XLeft = random.NextRoundedDouble(),
                    XRight = random.NextRoundedDouble(),
                    ZTop = random.NextRoundedDouble(),
                    ZBottom = random.NextRoundedDouble(),
                    NumberOfVerticalPoints = random.Next(),
                    NumberOfHorizontalPoints = random.Next()
                },
                RightGrid =
                {
                    XLeft = random.NextRoundedDouble(),
                    XRight = random.NextRoundedDouble(),
                    ZTop = random.NextRoundedDouble(),
                    ZBottom = random.NextRoundedDouble(),
                    NumberOfVerticalPoints = random.Next(),
                    NumberOfHorizontalPoints = random.Next()
                },
                TangentLineDeterminationType = MacroStabilityInwardsTangentLineDeterminationType.Specified,
                TangentLineZTop = random.NextRoundedDouble(),
                TangentLineZBottom = random.NextRoundedDouble()
            };

            // Call
            UpliftVanSlipPlane slipPlane = UpliftVanSlipPlaneConverter.Convert(input);

            // Assert
            Assert.IsFalse(slipPlane.GridAutomaticDetermined);
            AssertGrid(input.LeftGrid, slipPlane.LeftGrid);
            AssertGrid(input.RightGrid, slipPlane.RightGrid);            
            Assert.IsFalse(slipPlane.TangentLinesAutomaticAtBoundaries);
            Assert.AreEqual(input.TangentLineZTop ,slipPlane.TangentZTop);
            Assert.AreEqual(input.TangentLineZBottom ,slipPlane.TangentZBottom);
            Assert.AreEqual(1 ,slipPlane.TangentLineNumber);
            Assert.AreEqual(10 ,slipPlane.MaxSpacingBetweenBoundaries);
        }

        private void AssertGrid(MacroStabilityInwardsGrid expectedGrid, UpliftVanGrid actualGrid)
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