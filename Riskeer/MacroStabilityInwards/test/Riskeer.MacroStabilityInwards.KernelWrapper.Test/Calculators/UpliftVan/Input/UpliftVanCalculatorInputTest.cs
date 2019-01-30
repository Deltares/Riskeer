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
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.Input;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Test.Calculators.UpliftVan.Input
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
            var soilProfile = new TestSoilProfile();
            var drainageConstruction = new DrainageConstruction();
            var phreaticLineOffsets = new PhreaticLineOffsets();
            var slipPlane = new UpliftVanSlipPlane();
            var slipPlaneConstraints = new UpliftVanSlipPlaneConstraints(random.NextDouble(), random.NextDouble(), random.NextBoolean());
            var waternetCreationMode = random.NextEnumValue<WaternetCreationMode>();
            var plLineCreationMethod = random.NextEnumValue<PlLineCreationMethod>();
            var landwardDirection = random.NextEnumValue<LandwardDirection>();
            double waterLevelRiverAverage = random.NextDouble();
            double waterLevelPolderExtreme = random.NextDouble();
            double waterLevelPolderDaily = random.NextDouble();
            double minimumLevelPhreaticLineAtDikeTopRiver = random.NextDouble();
            double minimumLevelPhreaticLineAtDikeTopPolder = random.NextDouble();
            double leakageLengthOutwardsPhreaticLine3 = random.NextDouble();
            double leakageLengthInwardsPhreaticLine3 = random.NextDouble();
            double leakageLengthOutwardsPhreaticLine4 = random.NextDouble();
            double leakageLengthInwardsPhreaticLine4 = random.NextDouble();
            double piezometricHeadPhreaticLine2Outwards = random.NextDouble();
            double piezometricHeadPhreaticLine2Inwards = random.NextDouble();
            double penetrationLengthExtreme = random.NextDouble();
            double penetrationLengthDaily = random.NextDouble();
            bool adjustPhreaticLine3And4ForUplift = random.NextBoolean();
            var dikeSoilScenario = random.NextEnumValue<MacroStabilityInwardsDikeSoilScenario>();
            bool moveGrid = random.NextBoolean();
            double maximumSliceWidth = random.NextDouble();

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
                    PhreaticLineOffsetsExtreme = phreaticLineOffsets,
                    PhreaticLineOffsetsDaily = phreaticLineOffsets,
                    SlipPlane = slipPlane,
                    SlipPlaneConstraints = slipPlaneConstraints,
                    WaterLevelRiverAverage = waterLevelRiverAverage,
                    WaterLevelPolderExtreme = waterLevelPolderExtreme,
                    WaterLevelPolderDaily = waterLevelPolderDaily,
                    MinimumLevelPhreaticLineAtDikeTopRiver = minimumLevelPhreaticLineAtDikeTopRiver,
                    MinimumLevelPhreaticLineAtDikeTopPolder = minimumLevelPhreaticLineAtDikeTopPolder,
                    LeakageLengthOutwardsPhreaticLine3 = leakageLengthOutwardsPhreaticLine3,
                    LeakageLengthInwardsPhreaticLine3 = leakageLengthInwardsPhreaticLine3,
                    LeakageLengthOutwardsPhreaticLine4 = leakageLengthOutwardsPhreaticLine4,
                    LeakageLengthInwardsPhreaticLine4 = leakageLengthInwardsPhreaticLine4,
                    PiezometricHeadPhreaticLine2Outwards = piezometricHeadPhreaticLine2Outwards,
                    PiezometricHeadPhreaticLine2Inwards = piezometricHeadPhreaticLine2Inwards,
                    PenetrationLengthExtreme = penetrationLengthExtreme,
                    PenetrationLengthDaily = penetrationLengthDaily,
                    AdjustPhreaticLine3And4ForUplift = adjustPhreaticLine3And4ForUplift,
                    DikeSoilScenario = dikeSoilScenario,
                    MoveGrid = moveGrid,
                    MaximumSliceWidth = maximumSliceWidth
                });

            // Assert
            Assert.AreEqual(waternetCreationMode, input.WaternetCreationMode);
            Assert.AreEqual(plLineCreationMethod, input.PlLineCreationMethod);
            Assert.AreEqual(hRiverValue, input.AssessmentLevel);
            Assert.AreSame(surfaceLine, input.SurfaceLine);
            Assert.AreEqual(landwardDirection, input.LandwardDirection);
            Assert.AreSame(soilProfile, input.SoilProfile);
            Assert.AreSame(drainageConstruction, input.DrainageConstruction);
            Assert.AreSame(phreaticLineOffsets, input.PhreaticLineOffsetsDaily);
            Assert.AreSame(phreaticLineOffsets, input.PhreaticLineOffsetsExtreme);
            Assert.AreSame(slipPlane, input.SlipPlane);
            Assert.AreSame(slipPlaneConstraints, input.SlipPlaneConstraints);

            Assert.AreEqual(waterLevelRiverAverage, input.WaterLevelRiverAverage);
            Assert.AreEqual(waterLevelPolderExtreme, input.WaterLevelPolderExtreme);
            Assert.AreEqual(waterLevelPolderDaily, input.WaterLevelPolderDaily);
            Assert.AreEqual(minimumLevelPhreaticLineAtDikeTopRiver, input.MinimumLevelPhreaticLineAtDikeTopRiver);
            Assert.AreEqual(minimumLevelPhreaticLineAtDikeTopPolder, input.MinimumLevelPhreaticLineAtDikeTopPolder);
            Assert.AreEqual(leakageLengthOutwardsPhreaticLine3, input.LeakageLengthOutwardsPhreaticLine3);
            Assert.AreEqual(leakageLengthInwardsPhreaticLine3, input.LeakageLengthInwardsPhreaticLine3);
            Assert.AreEqual(leakageLengthOutwardsPhreaticLine4, input.LeakageLengthOutwardsPhreaticLine4);
            Assert.AreEqual(leakageLengthInwardsPhreaticLine4, input.LeakageLengthInwardsPhreaticLine4);
            Assert.AreEqual(piezometricHeadPhreaticLine2Outwards, input.PiezometricHeadPhreaticLine2Outwards);
            Assert.AreEqual(piezometricHeadPhreaticLine2Inwards, input.PiezometricHeadPhreaticLine2Inwards);
            Assert.AreEqual(penetrationLengthExtreme, input.PenetrationLengthExtreme);
            Assert.AreEqual(penetrationLengthDaily, input.PenetrationLengthDaily);
            Assert.AreEqual(adjustPhreaticLine3And4ForUplift, input.AdjustPhreaticLine3And4ForUplift);

            Assert.AreEqual(dikeSoilScenario, input.DikeSoilScenario);
            Assert.AreEqual(moveGrid, input.MoveGrid);
            Assert.AreEqual(maximumSliceWidth, input.MaximumSliceWidth);
        }

        [Test]
        public void Constructor_EmptyConstructionProperties_ExpectedValues()
        {
            // Setup
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
            var soilProfile = new TestSoilProfile();
            var drainageConstruction = new DrainageConstruction();
            var phreaticLineOffsets = new PhreaticLineOffsets();
            var slipPlane = new UpliftVanSlipPlane();
            var slipPlaneConstraints = new UpliftVanSlipPlaneConstraints(double.NaN, double.NaN, true);

            // Call
            var input = new UpliftVanCalculatorInput(
                new UpliftVanCalculatorInput.ConstructionProperties
                {
                    SurfaceLine = surfaceLine,
                    SoilProfile = soilProfile,
                    DrainageConstruction = drainageConstruction,
                    PhreaticLineOffsetsExtreme = phreaticLineOffsets,
                    PhreaticLineOffsetsDaily = phreaticLineOffsets,
                    SlipPlane = slipPlane,
                    SlipPlaneConstraints = slipPlaneConstraints
                });

            // Assert
            Assert.IsNaN(input.AssessmentLevel);
            Assert.IsNaN(input.WaterLevelRiverAverage);
            Assert.IsNaN(input.WaterLevelPolderExtreme);
            Assert.IsNaN(input.WaterLevelPolderDaily);
            Assert.IsNaN(input.MinimumLevelPhreaticLineAtDikeTopRiver);
            Assert.IsNaN(input.MinimumLevelPhreaticLineAtDikeTopPolder);
            Assert.IsNaN(input.LeakageLengthOutwardsPhreaticLine3);
            Assert.IsNaN(input.LeakageLengthInwardsPhreaticLine3);
            Assert.IsNaN(input.LeakageLengthOutwardsPhreaticLine4);
            Assert.IsNaN(input.LeakageLengthInwardsPhreaticLine4);
            Assert.IsNaN(input.PiezometricHeadPhreaticLine2Outwards);
            Assert.IsNaN(input.PiezometricHeadPhreaticLine2Inwards);
            Assert.IsNaN(input.PenetrationLengthDaily);
            Assert.IsNaN(input.PenetrationLengthExtreme);
            Assert.IsNaN(input.MaximumSliceWidth);

            Assert.IsFalse(input.AdjustPhreaticLine3And4ForUplift);
            Assert.IsFalse(input.MoveGrid);

            Assert.AreEqual(WaternetCreationMode.CreateWaternet, input.WaternetCreationMode);
            Assert.AreEqual(PlLineCreationMethod.RingtoetsWti2017, input.PlLineCreationMethod);
            Assert.AreEqual(LandwardDirection.PositiveX, input.LandwardDirection);
            Assert.AreEqual(MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay, input.DikeSoilScenario);
        }

        [Test]
        public void Constructor_SurfaceLineNull_ThrowsArgumentException()
        {
            // Call
            TestDelegate test = () => new UpliftVanCalculatorInput(
                new UpliftVanCalculatorInput.ConstructionProperties
                {
                    SoilProfile = new TestSoilProfile(),
                    PhreaticLineOffsetsExtreme = new PhreaticLineOffsets(),
                    PhreaticLineOffsetsDaily = new PhreaticLineOffsets(),
                    DrainageConstruction = new DrainageConstruction(),
                    SlipPlane = new UpliftVanSlipPlane()
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
                    PhreaticLineOffsetsExtreme = new PhreaticLineOffsets(),
                    PhreaticLineOffsetsDaily = new PhreaticLineOffsets(),
                    DrainageConstruction = new DrainageConstruction(),
                    SlipPlane = new UpliftVanSlipPlane()
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
                    SoilProfile = new TestSoilProfile(),
                    SurfaceLine = new MacroStabilityInwardsSurfaceLine("test"),
                    PhreaticLineOffsetsDaily = new PhreaticLineOffsets(),
                    PhreaticLineOffsetsExtreme = new PhreaticLineOffsets(),
                    SlipPlane = new UpliftVanSlipPlane()
                });

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, "DrainageConstruction must be set.");
        }

        [Test]
        public void Constructor_PhreaticLineOffsetsExtremeNull_ThrowsArgumentException()
        {
            // Call
            TestDelegate test = () => new UpliftVanCalculatorInput(
                new UpliftVanCalculatorInput.ConstructionProperties
                {
                    SoilProfile = new TestSoilProfile(),
                    SurfaceLine = new MacroStabilityInwardsSurfaceLine("test"),
                    DrainageConstruction = new DrainageConstruction(),
                    SlipPlane = new UpliftVanSlipPlane(),
                    PhreaticLineOffsetsDaily = new PhreaticLineOffsets()
                });

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, "PhreaticLineOffsetsExtreme must be set.");
        }

        [Test]
        public void Constructor_PhreaticLineOffsetsDailyNull_ThrowsArgumentException()
        {
            // Call
            TestDelegate test = () => new UpliftVanCalculatorInput(
                new UpliftVanCalculatorInput.ConstructionProperties
                {
                    SoilProfile = new TestSoilProfile(),
                    SurfaceLine = new MacroStabilityInwardsSurfaceLine("test"),
                    DrainageConstruction = new DrainageConstruction(),
                    SlipPlane = new UpliftVanSlipPlane(),
                    PhreaticLineOffsetsExtreme = new PhreaticLineOffsets()
                });

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, "PhreaticLineOffsetsDaily must be set.");
        }

        [Test]
        public void Constructor_SlipPlaneNull_ThrowsArgumentException()
        {
            // Call
            TestDelegate test = () => new UpliftVanCalculatorInput(
                new UpliftVanCalculatorInput.ConstructionProperties
                {
                    SoilProfile = new TestSoilProfile(),
                    SurfaceLine = new MacroStabilityInwardsSurfaceLine("test"),
                    DrainageConstruction = new DrainageConstruction(),
                    PhreaticLineOffsetsExtreme = new PhreaticLineOffsets(),
                    PhreaticLineOffsetsDaily = new PhreaticLineOffsets()
                });

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, "SlipPlane must be set.");
        }
    }
}