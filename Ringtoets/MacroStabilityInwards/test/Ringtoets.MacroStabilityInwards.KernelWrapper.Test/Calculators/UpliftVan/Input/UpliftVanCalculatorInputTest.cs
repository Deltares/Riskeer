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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.UpliftVan.Input;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Test.Calculators.UpliftVan.Input
{
    [TestFixture]
    public class UpliftVanCalculatorInputTest
    {
        [Test]
        public void Constructor_ConstructionPropertiesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new UpliftVanCalculatorInput(null);

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
            var soilProfile = new TestUpliftVanSoilProfile();
            var drainageConstruction = new UpliftVanDrainageConstruction();
            var phreaticLineOffsets = new UpliftVanPhreaticLineOffsets();

            var waternetCreationMode = random.NextEnumValue<UpliftVanWaternetCreationMode>();
            var plLineCreationMethod = random.NextEnumValue<UpliftVanPlLineCreationMethod>();
            var landwardDirection = random.NextEnumValue<UpliftVanLandwardDirection>();
            double waterLevelRiverAverage = random.Next();
            double waterLevelPolder = random.Next();
            double minimumLevelPhreaticLineAtDikeTopRiver = random.Next();
            double minimumLevelPhreaticLineAtDikeTopPolder = random.Next();
            double leakageLengthOutwardsPhreaticLine3 = random.Next();
            double leakageLengthInwardsPhreaticLine3 = random.Next();
            double leakageLengthOutwardsPhreaticLine4 = random.Next();
            double leakageLengthInwardsPhreaticLine4 = random.Next();
            double piezometricHeadPhreaticLine2Outwards = random.Next();
            double piezometricHeadPhreaticLine2Inwards = random.Next();
            double penetrationLength = random.Next();
            bool adjustPhreaticLine3And4ForUplift = random.NextBoolean();
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
            var input = new UpliftVanCalculatorInput(
                new UpliftVanCalculatorInput.ConstructionProperties
                {
                    WaternetCreationMode = waternetCreationMode,
                    PlLineCreationMethod = plLineCreationMethod,
                    AssessmentLevel = hRiverValue,
                    LandwardDirection = landwardDirection,
                    SurfaceLine = surfaceLine,
                    SoilProfile = soilProfile,
                    DrainageConstruction = drainageConstruction,
                    PhreaticLineOffsets = phreaticLineOffsets,
                    WaterLevelRiverAverage = waterLevelRiverAverage,
                    WaterLevelPolder = waterLevelPolder,
                    MinimumLevelPhreaticLineAtDikeTopRiver = minimumLevelPhreaticLineAtDikeTopRiver,
                    MinimumLevelPhreaticLineAtDikeTopPolder = minimumLevelPhreaticLineAtDikeTopPolder,
                    LeakageLengthOutwardsPhreaticLine3 = leakageLengthOutwardsPhreaticLine3,
                    LeakageLengthInwardsPhreaticLine3 = leakageLengthInwardsPhreaticLine3,
                    LeakageLengthOutwardsPhreaticLine4 = leakageLengthOutwardsPhreaticLine4,
                    LeakageLengthInwardsPhreaticLine4 = leakageLengthInwardsPhreaticLine4,
                    PiezometricHeadPhreaticLine2Outwards = piezometricHeadPhreaticLine2Outwards,
                    PiezometricHeadPhreaticLine2Inwards = piezometricHeadPhreaticLine2Inwards,
                    PenetrationLength = penetrationLength,
                    AdjustPhreaticLine3And4ForUplift = adjustPhreaticLine3And4ForUplift,
                    DikeSoilScenario = dikeSoilScenario,
                    MoveGrid = moveGrid,
                    MaximumSliceWidth = maximumSliceWidth,
                    GridAutomaticDetermined = gridAutomaticDetermined,
                    LeftGrid = leftGrid,
                    RightGrid = rightGrid,
                    TangentLinesAutomaticAtBoundaries = tangentLineAutomaticAtBoundaries,
                    TangentLineZTop = tangentLineZTop,
                    TangentLineZBottom = tangentLineZBottom,
                    CreateZones = createZones,
                    AutomaticForbiddenZones = automaticForbiddenZones,
                    SlipPlaneMinimumDepth = slipPlaneMinDepth,
                    SlipPlaneMinimumLength = slipPlaneMinLength
                });

            // Assert
            Assert.AreEqual(waternetCreationMode, input.WaternetCreationMode);
            Assert.AreEqual(plLineCreationMethod, input.PlLineCreationMethod);
            Assert.AreEqual(hRiverValue, input.AssessmentLevel);
            Assert.AreSame(surfaceLine, input.SurfaceLine);
            Assert.AreEqual(landwardDirection, input.LandwardDirection);
            Assert.AreSame(soilProfile, input.SoilProfile);
            Assert.AreSame(drainageConstruction, input.DrainageConstruction);
            Assert.AreSame(phreaticLineOffsets, input.PhreaticLineOffsets);

            Assert.AreEqual(waterLevelRiverAverage, input.WaterLevelRiverAverage);
            Assert.AreEqual(waterLevelPolder, input.WaterLevelPolder);
            Assert.AreEqual(minimumLevelPhreaticLineAtDikeTopRiver, input.MinimumLevelPhreaticLineAtDikeTopRiver);
            Assert.AreEqual(minimumLevelPhreaticLineAtDikeTopPolder, input.MinimumLevelPhreaticLineAtDikeTopPolder);
            Assert.AreEqual(leakageLengthOutwardsPhreaticLine3, input.LeakageLengthOutwardsPhreaticLine3);
            Assert.AreEqual(leakageLengthInwardsPhreaticLine3, input.LeakageLengthInwardsPhreaticLine3);
            Assert.AreEqual(leakageLengthOutwardsPhreaticLine4, input.LeakageLengthOutwardsPhreaticLine4);
            Assert.AreEqual(leakageLengthInwardsPhreaticLine4, input.LeakageLengthInwardsPhreaticLine4);
            Assert.AreEqual(piezometricHeadPhreaticLine2Outwards, input.PiezometricHeadPhreaticLine2Outwards);
            Assert.AreEqual(piezometricHeadPhreaticLine2Inwards, input.PiezometricHeadPhreaticLine2Inwards);
            Assert.AreEqual(penetrationLength, input.PenetrationLength);
            Assert.AreEqual(adjustPhreaticLine3And4ForUplift, input.AdjustPhreaticLine3And4ForUplift);

            Assert.AreEqual(dikeSoilScenario, input.DikeSoilScenario);
            Assert.AreEqual(moveGrid, input.MoveGrid);
            Assert.AreEqual(maximumSliceWidth, input.MaximumSliceWidth);
            Assert.AreEqual(gridAutomaticDetermined, input.GridAutomaticDetermined);

            Assert.AreSame(leftGrid, input.LeftGrid);
            Assert.AreSame(rightGrid, input.RightGrid);

            Assert.AreEqual(tangentLineAutomaticAtBoundaries, input.TangentLinesAutomaticAtBoundaries);
            Assert.AreEqual(tangentLineZTop, input.TangentLineZTop);
            Assert.AreEqual(tangentLineZBottom, input.TangentLineZBottom);
            Assert.AreEqual(createZones, input.CreateZones);
            Assert.AreEqual(automaticForbiddenZones, input.AutomaticForbiddenZones);
            Assert.AreEqual(slipPlaneMinDepth, input.SlipPlaneMinimumDepth);
            Assert.AreEqual(slipPlaneMinLength, input.SlipPlaneMinimumLength);
        }

        [Test]
        public void Constructor_EmptyConstructionProperties_ExpectedValues()
        {
            // Setup
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
            var soilProfile = new TestUpliftVanSoilProfile();
            var drainageConstruction = new UpliftVanDrainageConstruction();
            var phreaticLineOffsets = new UpliftVanPhreaticLineOffsets();

            // Call
            var input = new UpliftVanCalculatorInput(
                new UpliftVanCalculatorInput.ConstructionProperties
                {
                    SurfaceLine = surfaceLine,
                    SoilProfile = soilProfile,
                    DrainageConstruction = drainageConstruction,
                    PhreaticLineOffsets = phreaticLineOffsets
                });

            // Assert
            Assert.IsNull(input.LeftGrid);
            Assert.IsNull(input.RightGrid);

            Assert.IsNaN(input.AssessmentLevel);
            Assert.IsNaN(input.WaterLevelRiverAverage);
            Assert.IsNaN(input.WaterLevelPolder);
            Assert.IsNaN(input.MinimumLevelPhreaticLineAtDikeTopRiver);
            Assert.IsNaN(input.MinimumLevelPhreaticLineAtDikeTopPolder);
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

            Assert.IsFalse(input.AdjustPhreaticLine3And4ForUplift);
            Assert.IsFalse(input.MoveGrid);
            Assert.IsFalse(input.GridAutomaticDetermined);
            Assert.IsFalse(input.TangentLinesAutomaticAtBoundaries);
            Assert.IsFalse(input.CreateZones);
            Assert.IsFalse(input.AutomaticForbiddenZones);

            Assert.AreEqual(UpliftVanWaternetCreationMode.CreateWaternet, input.WaternetCreationMode);
            Assert.AreEqual(UpliftVanPlLineCreationMethod.RingtoetsWti2017, input.PlLineCreationMethod);
            Assert.AreEqual(UpliftVanLandwardDirection.PositiveX, input.LandwardDirection);
            Assert.AreEqual(MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay, input.DikeSoilScenario);
        }

        [Test]
        public void Constructor_SurfaceLineNull_ThrowsArgumentException()
        {
            // Call
            TestDelegate test = () => new UpliftVanCalculatorInput(
                new UpliftVanCalculatorInput.ConstructionProperties
                {
                    SoilProfile = new TestUpliftVanSoilProfile(),
                    PhreaticLineOffsets = new UpliftVanPhreaticLineOffsets(),
                    DrainageConstruction = new UpliftVanDrainageConstruction()
                });

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, "SurfaceLine must be set.");
        }

        [Test]
        public void Constructor_SoilProfileNull_ThrowsArgumentException()
        {
            // Call
            TestDelegate test = () => new UpliftVanCalculatorInput(
                new UpliftVanCalculatorInput.ConstructionProperties
                {
                    SurfaceLine = new MacroStabilityInwardsSurfaceLine("test"),
                    PhreaticLineOffsets = new UpliftVanPhreaticLineOffsets(),
                    DrainageConstruction = new UpliftVanDrainageConstruction()
                });

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, "SoilProfile must be set.");
        }

        [Test]
        public void Constructor_DrainageConstructionNull_ThrowsArgumentException()
        {
            // Call
            TestDelegate test = () => new UpliftVanCalculatorInput(
                new UpliftVanCalculatorInput.ConstructionProperties
                {
                    SoilProfile = new TestUpliftVanSoilProfile(),
                    SurfaceLine = new MacroStabilityInwardsSurfaceLine("test"),
                    PhreaticLineOffsets = new UpliftVanPhreaticLineOffsets()
                });

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, "DrainageConstruction must be set.");
        }

        [Test]
        public void Constructor_PhreaticLineOffsetsNull_ThrowsArgumentException()
        {
            // Call
            TestDelegate test = () => new UpliftVanCalculatorInput(
                new UpliftVanCalculatorInput.ConstructionProperties
                {
                    SoilProfile = new TestUpliftVanSoilProfile(),
                    SurfaceLine = new MacroStabilityInwardsSurfaceLine("test"),
                    DrainageConstruction = new UpliftVanDrainageConstruction()
                });

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, "PhreaticLineOffsets must be set.");
        }
    }
}