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
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.MacroStabilityInwards.Primitives.MacroStabilityInwardsSoilUnderSurfaceLine;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Test
{
    [TestFixture]
    public class MacroStabilityInwardsCalculatorInputTest
    {
        [Test]
        public void Constructor_WithoutConstructionProperies_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsCalculatorInput(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("properties", paramName);
        }

        [Test]
        public void Constructor_WithConstructionProperties_PropertiesAreSet()
        {
            // Setup
            var random = new Random(11);

            double hRiverValue = random.NextDouble();
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
            var soilProfile = new MacroStabilityInwardsSoilProfileUnderSurfaceLine(new[]
            {
                new MacroStabilityInwardsSoilLayerUnderSurfaceLine(new[]
                {
                    new Point2D(0, 0)
                }, new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine(
                    new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine.ConstructionProperties()))
            }, Enumerable.Empty<MacroStabilityInwardsPreconsolidationStressUnderSurfaceLine>());
            
            double waterLevelRiverAverage = random.Next();
            double waterLevelPolder = random.Next();
            double xCoordinateDrainageConstruction = random.Next();
            double zCoordinateDrainageConstruction = random.Next();
            double minimumLevelPhreaticLineAtDikeTopRiver = random.Next();
            double minimumLevelPhreaticLineAtDikeTopPolder = random.Next();
            double phreaticLineOffsetBelowDikeTopAtRiver = random.Next();
            double phreaticLineOffsetBelowDikeTopAtPolder = random.Next();
            double phreaticLineOffsetBelowShoulderBaseInside = random.Next();
            double phreaticLineOffsetBelowDikeToeAtPolder = random.Next();
            double leakageLengthOutwardsPhreaticLine3 = random.Next();
            double leakageLengthInwardsPhreaticLine3 = random.Next();
            double leakageLengthOutwardsPhreaticLine4 = random.Next();
            double leakageLengthInwardsPhreaticLine4 = random.Next();
            double piezometricHeadPhreaticLine2Outwards = random.Next();
            double piezometricHeadPhreaticLine2Inwards = random.Next();
            double penetrationLength = random.Next();
            bool useDefaultOffsets = random.NextBoolean();
            bool adjustPhreaticLine3And4ForUplift = random.NextBoolean();
            bool drainageConstructionPresent = random.NextBoolean();
            var dikeSoilScenario = random.NextEnumValue<MacroStabilityInwardsDikeSoilScenario>();
            bool moveGrid = random.NextBoolean();
            double maximumSliceWidth = random.Next();
            bool gridAutomaticDetermined = random.NextBoolean();
            var leftGrid = new MacroStabilityInwardsGrid();
            var rightGrid = new MacroStabilityInwardsGrid();
            bool tangentLineAutomaticAtBoundaries = random.NextBoolean();
            double tangentLineZTop = random.Next();
            double tangentLineZBottom = random.Next();
            bool createZones = random.NextBoolean();
            bool automaticForbiddenZones = random.NextBoolean();
            double slipPlaneMinDepth = random.Next();
            double slipPlaneMinLength = random.Next();

            // Call
            var input = new MacroStabilityInwardsCalculatorInput(
                new MacroStabilityInwardsCalculatorInput.ConstructionProperties
                {
                    AssessmentLevel = hRiverValue,
                    SurfaceLine = surfaceLine,
                    SoilProfile = soilProfile,
                    WaterLevelRiverAverage = waterLevelRiverAverage,
                    WaterLevelPolder = waterLevelPolder,
                    XCoordinateDrainageConstruction = xCoordinateDrainageConstruction,
                    ZCoordinateDrainageConstruction = zCoordinateDrainageConstruction,
                    MinimumLevelPhreaticLineAtDikeTopRiver = minimumLevelPhreaticLineAtDikeTopRiver,
                    MinimumLevelPhreaticLineAtDikeTopPolder = minimumLevelPhreaticLineAtDikeTopPolder,
                    PhreaticLineOffsetBelowDikeTopAtRiver = phreaticLineOffsetBelowDikeTopAtRiver,
                    PhreaticLineOffsetBelowDikeTopAtPolder = phreaticLineOffsetBelowDikeTopAtPolder,
                    PhreaticLineOffsetBelowShoulderBaseInside = phreaticLineOffsetBelowShoulderBaseInside,
                    PhreaticLineOffsetBelowDikeToeAtPolder = phreaticLineOffsetBelowDikeToeAtPolder,
                    LeakageLengthOutwardsPhreaticLine3 = leakageLengthOutwardsPhreaticLine3,
                    LeakageLengthInwardsPhreaticLine3 = leakageLengthInwardsPhreaticLine3,
                    LeakageLengthOutwardsPhreaticLine4 = leakageLengthOutwardsPhreaticLine4,
                    LeakageLengthInwardsPhreaticLine4 = leakageLengthInwardsPhreaticLine4,
                    PiezometricHeadPhreaticLine2Outwards = piezometricHeadPhreaticLine2Outwards,
                    PiezometricHeadPhreaticLine2Inwards = piezometricHeadPhreaticLine2Inwards,
                    PenetrationLength = penetrationLength,
                    UseDefaultOffsets = useDefaultOffsets,
                    AdjustPhreaticLine3And4ForUplift = adjustPhreaticLine3And4ForUplift,
                    DrainageConstructionPresent = drainageConstructionPresent,
                    DikeSoilScenario = dikeSoilScenario,
                    MoveGrid = moveGrid,
                    MaximumSliceWidth = maximumSliceWidth,
                    GridAutomaticDetermined = gridAutomaticDetermined,
                    LeftGrid = leftGrid,
                    RightGrid = rightGrid,
                    TangentLineAutomaticAtBoundaries = tangentLineAutomaticAtBoundaries,
                    TangentLineZTop = tangentLineZTop,
                    TangentLineZBottom = tangentLineZBottom,
                    CreateZones = createZones,
                    AutomaticForbiddenZones = automaticForbiddenZones,
                    SlipPlaneMinimumDepth = slipPlaneMinDepth,
                    SlipPlaneMinimumLength = slipPlaneMinLength
                });

            // Assert
            Assert.AreEqual(hRiverValue, input.AssessmentLevel);
            Assert.AreSame(surfaceLine, input.SurfaceLine);
            Assert.AreSame(soilProfile, input.SoilProfile);

            Assert.AreEqual(waterLevelRiverAverage, input.WaterLevelRiverAverage);
            Assert.AreEqual(waterLevelPolder, input.WaterLevelPolder);
            Assert.AreEqual(xCoordinateDrainageConstruction, input.XCoordinateDrainageConstruction);
            Assert.AreEqual(zCoordinateDrainageConstruction, input.ZCoordinateDrainageConstruction);
            Assert.AreEqual(minimumLevelPhreaticLineAtDikeTopRiver, input.MinimumLevelPhreaticLineAtDikeTopRiver);
            Assert.AreEqual(minimumLevelPhreaticLineAtDikeTopPolder, input.MinimumLevelPhreaticLineAtDikeTopPolder);
            Assert.AreEqual(phreaticLineOffsetBelowDikeTopAtRiver, input.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.AreEqual(phreaticLineOffsetBelowDikeTopAtPolder, input.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.AreEqual(phreaticLineOffsetBelowShoulderBaseInside, input.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.AreEqual(phreaticLineOffsetBelowDikeToeAtPolder, input.PhreaticLineOffsetBelowDikeToeAtPolder);
            Assert.AreEqual(leakageLengthOutwardsPhreaticLine3, input.LeakageLengthOutwardsPhreaticLine3);
            Assert.AreEqual(leakageLengthInwardsPhreaticLine3, input.LeakageLengthInwardsPhreaticLine3);
            Assert.AreEqual(leakageLengthOutwardsPhreaticLine4, input.LeakageLengthOutwardsPhreaticLine4);
            Assert.AreEqual(leakageLengthInwardsPhreaticLine4, input.LeakageLengthInwardsPhreaticLine4);
            Assert.AreEqual(piezometricHeadPhreaticLine2Outwards, input.PiezometricHeadPhreaticLine2Outwards);
            Assert.AreEqual(piezometricHeadPhreaticLine2Inwards, input.PiezometricHeadPhreaticLine2Inwards);
            Assert.AreEqual(penetrationLength, input.PenetrationLength);
            Assert.AreEqual(useDefaultOffsets, input.UseDefaultOffsets);
            Assert.AreEqual(adjustPhreaticLine3And4ForUplift, input.AdjustPhreaticLine3And4ForUplift);
            Assert.AreEqual(drainageConstructionPresent, input.DrainageConstructionPresent);

            Assert.AreEqual(dikeSoilScenario, input.DikeSoilScenario);
            Assert.AreEqual(moveGrid, input.MoveGrid);
            Assert.AreEqual(maximumSliceWidth, input.MaximumSliceWidth);
            Assert.AreEqual(gridAutomaticDetermined, input.GridAutomaticDetermined);

            Assert.AreSame(leftGrid, input.LeftGrid);
            Assert.AreSame(rightGrid, input.RightGrid);

            Assert.AreEqual(tangentLineAutomaticAtBoundaries, input.TangentLineAutomaticAtBoundaries);
            Assert.AreEqual(tangentLineZTop, input.TangentLineZTop);
            Assert.AreEqual(tangentLineZBottom, input.TangentLineZBottom);
            Assert.AreEqual(createZones, input.CreateZones);
            Assert.AreEqual(automaticForbiddenZones, input.AutomaticForbiddenZones);
            Assert.AreEqual(slipPlaneMinDepth, input.SlipPlaneMinimumDepth);
            Assert.AreEqual(slipPlaneMinLength, input.SlipPlaneMinimumLength);
        }

        [Test]
        public void Constructor_EmptyConstrutionProperties_ExpectedValues()
        {
            // Call
            var input = new MacroStabilityInwardsCalculatorInput(
                new MacroStabilityInwardsCalculatorInput.ConstructionProperties());

            // Assert
            Assert.IsNull(input.SurfaceLine);
            Assert.IsNull(input.SoilProfile);
            Assert.IsNull(input.LeftGrid);
            Assert.IsNull(input.RightGrid);

            Assert.IsNaN(input.AssessmentLevel);
            Assert.IsNaN(input.WaterLevelRiverAverage);
            Assert.IsNaN(input.WaterLevelPolder);
            Assert.IsNaN(input.XCoordinateDrainageConstruction);
            Assert.IsNaN(input.ZCoordinateDrainageConstruction);
            Assert.IsNaN(input.MinimumLevelPhreaticLineAtDikeTopRiver);
            Assert.IsNaN(input.MinimumLevelPhreaticLineAtDikeTopPolder);
            Assert.IsNaN(input.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.IsNaN(input.PhreaticLineOffsetBelowDikeToeAtPolder);
            Assert.IsNaN(input.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.IsNaN(input.PhreaticLineOffsetBelowDikeToeAtPolder);
            Assert.IsNaN(input.LeakageLengthOutwardsPhreaticLine3);
            Assert.IsNaN(input.LeakageLengthInwardsPhreaticLine3);
            Assert.IsNaN(input.LeakageLengthOutwardsPhreaticLine4);
            Assert.IsNaN(input.LeakageLengthInwardsPhreaticLine4);
            Assert.IsNaN(input.PiezometricHeadPhreaticLine2Outwards);
            Assert.IsNaN(input.PiezometricHeadPhreaticLine2Inwards);
            Assert.IsNaN(input.PenetrationLength);
            Assert.IsNaN(input.MaximumSliceWidth);
            Assert.IsNaN(input.TangentLineZTop);
            Assert.IsNaN(input.TangentLineZBottom);
            Assert.IsNaN(input.SlipPlaneMinimumDepth);
            Assert.IsNaN(input.SlipPlaneMinimumLength);

            Assert.IsFalse(input.DrainageConstructionPresent);
            Assert.IsFalse(input.AdjustPhreaticLine3And4ForUplift);
            Assert.IsFalse(input.UseDefaultOffsets);
            Assert.IsFalse(input.MoveGrid);
            Assert.IsFalse(input.GridAutomaticDetermined);
            Assert.IsFalse(input.TangentLineAutomaticAtBoundaries);
            Assert.IsFalse(input.CreateZones);
            Assert.IsFalse(input.AutomaticForbiddenZones);

            Assert.AreEqual(MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay, input.DikeSoilScenario);
        }
    }
}