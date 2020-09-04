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
using System.ComponentModel;
using Core.Common.TestUtil;
using Deltares.MacroStability.CSharpWrapper.Input;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.Input;
using Riskeer.MacroStabilityInwards.Primitives;
using WaternetCreationMode = Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input.WaternetCreationMode;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Test.Creators.Input
{
    [TestFixture]
    public class UpliftVanWaternetCreatorInputCreatorTest
    {
        [Test]
        public void CreateExtreme_InputNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => UpliftVanWaternetCreatorInputCreator.CreateExtreme(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("input", exception.ParamName);
        }

        [Test]
        public void CreateExtreme_InvalidDikeSoilScenario_ThrowInvalidEnumArgumentException()
        {
            // Setup
            var input = new UpliftVanCalculatorInput(
                new UpliftVanCalculatorInput.ConstructionProperties
                {
                    SurfaceLine = new MacroStabilityInwardsSurfaceLine("test"),
                    SoilProfile = new TestSoilProfile(),
                    PhreaticLineOffsetsExtreme = new PhreaticLineOffsets(),
                    PhreaticLineOffsetsDaily = new PhreaticLineOffsets(),
                    DrainageConstruction = new DrainageConstruction(),
                    SlipPlane = new UpliftVanSlipPlane(),
                    DikeSoilScenario = (MacroStabilityInwardsDikeSoilScenario) 99
                });

            // Call
            void Call() => UpliftVanWaternetCreatorInputCreator.CreateExtreme(input);

            // Assert
            string message = $"The value of argument 'dikeSoilScenario' ({99}) is invalid for Enum type '{nameof(MacroStabilityInwardsDikeSoilScenario)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, message);
        }

        [Test]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay, DikeSoilScenario.ClayDikeOnClay)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.ClayDikeOnSand, DikeSoilScenario.ClayDikeOnSand)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.SandDikeOnClay, DikeSoilScenario.SandDikeOnClay)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.SandDikeOnSand, DikeSoilScenario.SandDikeOnSand)]
        public void CreateExtreme_ValidDikeSoilScenario_ReturnLocationWithDikeSoilScenario(MacroStabilityInwardsDikeSoilScenario macroStabilityInwardsDikeSoilScenario,
                                                                                           DikeSoilScenario expectedDikeSoilScenario)
        {
            // Setup
            var input = new UpliftVanCalculatorInput(
                new UpliftVanCalculatorInput.ConstructionProperties
                {
                    DrainageConstruction = new DrainageConstruction(),
                    PhreaticLineOffsetsExtreme = new PhreaticLineOffsets(),
                    PhreaticLineOffsetsDaily = new PhreaticLineOffsets(),
                    SurfaceLine = new MacroStabilityInwardsSurfaceLine("test"),
                    SoilProfile = new TestSoilProfile(),
                    SlipPlane = new UpliftVanSlipPlane(),
                    DikeSoilScenario = macroStabilityInwardsDikeSoilScenario
                });

            // Call
            WaternetCreatorInput waternetCreatorInput = UpliftVanWaternetCreatorInputCreator.CreateExtreme(input);

            // Assert
            Assert.AreEqual(expectedDikeSoilScenario, waternetCreatorInput.DikeSoilScenario);
        }

        [Test]
        public void CreateExtreme_InvalidWaternetCreationMode_ThrowInvalidEnumArgumentException()
        {
            // Setup
            var input = new UpliftVanCalculatorInput(
                new UpliftVanCalculatorInput.ConstructionProperties
                {
                    SurfaceLine = new MacroStabilityInwardsSurfaceLine("test"),
                    SoilProfile = new TestSoilProfile(),
                    PhreaticLineOffsetsExtreme = new PhreaticLineOffsets(),
                    PhreaticLineOffsetsDaily = new PhreaticLineOffsets(),
                    DrainageConstruction = new DrainageConstruction(),
                    SlipPlane = new UpliftVanSlipPlane(),
                    WaternetCreationMode = (WaternetCreationMode) 99
                });

            // Call
            void Call() => UpliftVanWaternetCreatorInputCreator.CreateExtreme(input);

            // Assert
            string message = $"The value of argument 'waternetCreationMode' ({99}) is invalid for Enum type '{nameof(WaternetCreationMode)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, message);
        }

        [Test]
        public void CreateExtreme_InvalidPlLineCreationMethod_ThrowInvalidEnumArgumentException()
        {
            // Setup
            var input = new UpliftVanCalculatorInput(
                new UpliftVanCalculatorInput.ConstructionProperties
                {
                    SurfaceLine = new MacroStabilityInwardsSurfaceLine("test"),
                    SoilProfile = new TestSoilProfile(),
                    PhreaticLineOffsetsExtreme = new PhreaticLineOffsets(),
                    PhreaticLineOffsetsDaily = new PhreaticLineOffsets(),
                    DrainageConstruction = new DrainageConstruction(),
                    SlipPlane = new UpliftVanSlipPlane(),
                    PlLineCreationMethod = (PlLineCreationMethod) 99
                });

            // Call
            void Call() => UpliftVanWaternetCreatorInputCreator.CreateExtreme(input);

            // Assert
            string message = $"The value of argument 'plLineCreationMethod' ({99}) is invalid for Enum type '{nameof(PlLineCreationMethod)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, message);
        }

        [Test]
        [Combinatorial]
        public void CreateExtreme_WithInput_ReturnLocation([Values(true, false)] bool drainageConstructionPresent,
                                                           [Values(true, false)] bool useDefaultOffsets)
        {
            // Setup
            var random = new Random(21);
            DrainageConstruction drainageConstruction = drainageConstructionPresent
                                                            ? new DrainageConstruction(random.Next(), random.Next())
                                                            : new DrainageConstruction();
            PhreaticLineOffsets phreaticLineOffsets = useDefaultOffsets
                                                          ? new PhreaticLineOffsets()
                                                          : new PhreaticLineOffsets(random.Next(), random.Next(),
                                                                                    random.Next(), random.Next());

            var input = new UpliftVanCalculatorInput(
                new UpliftVanCalculatorInput.ConstructionProperties
                {
                    SurfaceLine = new MacroStabilityInwardsSurfaceLine("test"),
                    SoilProfile = new TestSoilProfile(),
                    SlipPlane = new UpliftVanSlipPlane(),
                    DikeSoilScenario = MacroStabilityInwardsDikeSoilScenario.SandDikeOnClay,
                    AssessmentLevel = random.NextDouble(),
                    WaterLevelRiverAverage = random.NextDouble(),
                    WaterLevelPolderExtreme = random.NextDouble(),
                    DrainageConstruction = drainageConstruction,
                    PhreaticLineOffsetsExtreme = phreaticLineOffsets,
                    PhreaticLineOffsetsDaily = new PhreaticLineOffsets(),
                    MinimumLevelPhreaticLineAtDikeTopRiver = random.NextDouble(),
                    MinimumLevelPhreaticLineAtDikeTopPolder = random.NextDouble(),
                    AdjustPhreaticLine3And4ForUplift = random.NextBoolean(),
                    LeakageLengthOutwardsPhreaticLine3 = random.NextDouble(),
                    LeakageLengthInwardsPhreaticLine3 = random.NextDouble(),
                    LeakageLengthOutwardsPhreaticLine4 = random.NextDouble(),
                    LeakageLengthInwardsPhreaticLine4 = random.NextDouble(),
                    PiezometricHeadPhreaticLine2Outwards = random.NextDouble(),
                    PiezometricHeadPhreaticLine2Inwards = random.NextDouble(),
                    PenetrationLengthExtreme = random.NextDouble()
                });

            // Call
            WaternetCreatorInput waternetCreatorInput = UpliftVanWaternetCreatorInputCreator.CreateExtreme(input);

            // Assert
            Assert.AreEqual(input.AssessmentLevel, waternetCreatorInput.HeadInPlLine3);
            Assert.AreEqual(input.AssessmentLevel, waternetCreatorInput.HeadInPlLine4);
            Assert.AreEqual(input.AssessmentLevel, waternetCreatorInput.WaterLevelRiver);
            Assert.AreEqual(input.WaterLevelPolderExtreme, waternetCreatorInput.WaterLevelPolder);
            Assert.AreEqual(input.PhreaticLineOffsetsExtreme.UseDefaults, waternetCreatorInput.UseDefaultOffsets);
            Assert.AreEqual(input.PhreaticLineOffsetsExtreme.BelowDikeTopAtRiver, waternetCreatorInput.PlLineOffsetBelowPointBRingtoetsWti2017);
            Assert.AreEqual(input.PhreaticLineOffsetsExtreme.BelowDikeTopAtPolder, waternetCreatorInput.PlLineOffsetBelowDikeTopAtPolder);
            Assert.AreEqual(input.PhreaticLineOffsetsExtreme.BelowShoulderBaseInside, waternetCreatorInput.PlLineOffsetBelowShoulderBaseInside);
            Assert.AreEqual(input.PhreaticLineOffsetsExtreme.BelowDikeToeAtPolder, waternetCreatorInput.PlLineOffsetBelowDikeToeAtPolder);
            Assert.AreEqual(input.PenetrationLengthExtreme, waternetCreatorInput.PenetrationLength);

            AssertGeneralLocationValues(input, waternetCreatorInput);
            AssertIrrelevantValues(waternetCreatorInput);
        }

        [Test]
        public void CreateDaily_InputNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => UpliftVanWaternetCreatorInputCreator.CreateDaily(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("input", exception.ParamName);
        }

        [Test]
        public void CreateDaily_InvalidDikeSoilScenario_ThrowInvalidEnumArgumentException()
        {
            // Setup
            var input = new UpliftVanCalculatorInput(
                new UpliftVanCalculatorInput.ConstructionProperties
                {
                    SurfaceLine = new MacroStabilityInwardsSurfaceLine("test"),
                    SoilProfile = new TestSoilProfile(),
                    PhreaticLineOffsetsExtreme = new PhreaticLineOffsets(),
                    PhreaticLineOffsetsDaily = new PhreaticLineOffsets(),
                    DrainageConstruction = new DrainageConstruction(),
                    SlipPlane = new UpliftVanSlipPlane(),
                    DikeSoilScenario = (MacroStabilityInwardsDikeSoilScenario) 99
                });

            // Call
            void Call() => UpliftVanWaternetCreatorInputCreator.CreateDaily(input);

            // Assert
            string message = $"The value of argument 'dikeSoilScenario' ({99}) is invalid for Enum type '{nameof(MacroStabilityInwardsDikeSoilScenario)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, message);
        }

        [Test]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay, DikeSoilScenario.ClayDikeOnClay)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.ClayDikeOnSand, DikeSoilScenario.ClayDikeOnSand)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.SandDikeOnClay, DikeSoilScenario.SandDikeOnClay)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.SandDikeOnSand, DikeSoilScenario.SandDikeOnSand)]
        public void CreateDaily_ValidDikeSoilScenario_ReturnLocationWithDikeSoilScenario(MacroStabilityInwardsDikeSoilScenario macroStabilityInwardsDikeSoilScenario,
                                                                                         DikeSoilScenario expectedDikeSoilScenario)
        {
            // Setup
            var input = new UpliftVanCalculatorInput(
                new UpliftVanCalculatorInput.ConstructionProperties
                {
                    DrainageConstruction = new DrainageConstruction(),
                    PhreaticLineOffsetsExtreme = new PhreaticLineOffsets(),
                    PhreaticLineOffsetsDaily = new PhreaticLineOffsets(),
                    SurfaceLine = new MacroStabilityInwardsSurfaceLine("test"),
                    SoilProfile = new TestSoilProfile(),
                    SlipPlane = new UpliftVanSlipPlane(),
                    DikeSoilScenario = macroStabilityInwardsDikeSoilScenario
                });

            // Call
            WaternetCreatorInput waternetCreatorInput = UpliftVanWaternetCreatorInputCreator.CreateDaily(input);

            // Assert
            Assert.AreEqual(expectedDikeSoilScenario, waternetCreatorInput.DikeSoilScenario);
        }

        [Test]
        public void CreateDaily_InvalidWaternetCreationMode_ThrowInvalidEnumArgumentException()
        {
            // Setup
            var input = new UpliftVanCalculatorInput(
                new UpliftVanCalculatorInput.ConstructionProperties
                {
                    SurfaceLine = new MacroStabilityInwardsSurfaceLine("test"),
                    SoilProfile = new TestSoilProfile(),
                    PhreaticLineOffsetsExtreme = new PhreaticLineOffsets(),
                    PhreaticLineOffsetsDaily = new PhreaticLineOffsets(),
                    DrainageConstruction = new DrainageConstruction(),
                    SlipPlane = new UpliftVanSlipPlane(),
                    WaternetCreationMode = (WaternetCreationMode) 99
                });

            // Call
            void Call() => UpliftVanWaternetCreatorInputCreator.CreateDaily(input);

            // Assert
            string message = $"The value of argument 'waternetCreationMode' ({99}) is invalid for Enum type '{nameof(WaternetCreationMode)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, message);
        }

        [Test]
        public void CreateDaily_InvalidPlLineCreationMethod_ThrowInvalidEnumArgumentException()
        {
            // Setup
            var input = new UpliftVanCalculatorInput(
                new UpliftVanCalculatorInput.ConstructionProperties
                {
                    SurfaceLine = new MacroStabilityInwardsSurfaceLine("test"),
                    SoilProfile = new TestSoilProfile(),
                    PhreaticLineOffsetsExtreme = new PhreaticLineOffsets(),
                    PhreaticLineOffsetsDaily = new PhreaticLineOffsets(),
                    DrainageConstruction = new DrainageConstruction(),
                    SlipPlane = new UpliftVanSlipPlane(),
                    PlLineCreationMethod = (PlLineCreationMethod) 99
                });

            // Call
            void Call() => UpliftVanWaternetCreatorInputCreator.CreateDaily(input);

            // Assert
            string message = $"The value of argument 'plLineCreationMethod' ({99}) is invalid for Enum type '{nameof(PlLineCreationMethod)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, message);
        }

        [Test]
        [Combinatorial]
        public void CreateDaily_WithInput_ReturnLocation([Values(true, false)] bool drainageConstructionPresent,
                                                         [Values(true, false)] bool useDefaultOffsets)
        {
            // Setup
            var random = new Random(21);
            DrainageConstruction drainageConstruction = drainageConstructionPresent
                                                            ? new DrainageConstruction(random.Next(), random.Next())
                                                            : new DrainageConstruction();
            PhreaticLineOffsets phreaticLineOffsets = useDefaultOffsets
                                                          ? new PhreaticLineOffsets()
                                                          : new PhreaticLineOffsets(random.Next(), random.Next(),
                                                                                    random.Next(), random.Next());

            var input = new UpliftVanCalculatorInput(
                new UpliftVanCalculatorInput.ConstructionProperties
                {
                    SurfaceLine = new MacroStabilityInwardsSurfaceLine("test"),
                    SoilProfile = new TestSoilProfile(),
                    SlipPlane = new UpliftVanSlipPlane(),
                    DikeSoilScenario = MacroStabilityInwardsDikeSoilScenario.SandDikeOnClay,
                    AssessmentLevel = random.NextDouble(),
                    WaterLevelRiverAverage = random.NextDouble(),
                    WaterLevelPolderDaily = random.NextDouble(),
                    DrainageConstruction = drainageConstruction,
                    PhreaticLineOffsetsExtreme = new PhreaticLineOffsets(),
                    PhreaticLineOffsetsDaily = phreaticLineOffsets,
                    MinimumLevelPhreaticLineAtDikeTopRiver = random.NextDouble(),
                    MinimumLevelPhreaticLineAtDikeTopPolder = random.NextDouble(),
                    AdjustPhreaticLine3And4ForUplift = random.NextBoolean(),
                    LeakageLengthOutwardsPhreaticLine3 = random.NextDouble(),
                    LeakageLengthInwardsPhreaticLine3 = random.NextDouble(),
                    LeakageLengthOutwardsPhreaticLine4 = random.NextDouble(),
                    LeakageLengthInwardsPhreaticLine4 = random.NextDouble(),
                    PiezometricHeadPhreaticLine2Outwards = random.NextDouble(),
                    PiezometricHeadPhreaticLine2Inwards = random.NextDouble(),
                    PenetrationLengthDaily = random.NextDouble()
                });

            // Call
            WaternetCreatorInput waternetCreatorInput = UpliftVanWaternetCreatorInputCreator.CreateDaily(input);

            // Assert
            Assert.AreEqual(input.WaterLevelRiverAverage, waternetCreatorInput.HeadInPlLine3);
            Assert.AreEqual(input.WaterLevelRiverAverage, waternetCreatorInput.HeadInPlLine4);
            Assert.AreEqual(input.WaterLevelRiverAverage, waternetCreatorInput.WaterLevelRiver);
            Assert.AreEqual(input.WaterLevelPolderDaily, waternetCreatorInput.WaterLevelPolder);
            Assert.AreEqual(input.PhreaticLineOffsetsDaily.UseDefaults, waternetCreatorInput.UseDefaultOffsets);
            Assert.AreEqual(input.PhreaticLineOffsetsDaily.BelowDikeTopAtRiver, waternetCreatorInput.PlLineOffsetBelowPointBRingtoetsWti2017);
            Assert.AreEqual(input.PhreaticLineOffsetsDaily.BelowDikeTopAtPolder, waternetCreatorInput.PlLineOffsetBelowDikeTopAtPolder);
            Assert.AreEqual(input.PhreaticLineOffsetsDaily.BelowShoulderBaseInside, waternetCreatorInput.PlLineOffsetBelowShoulderBaseInside);
            Assert.AreEqual(input.PhreaticLineOffsetsDaily.BelowDikeToeAtPolder, waternetCreatorInput.PlLineOffsetBelowDikeToeAtPolder);
            Assert.AreEqual(input.PenetrationLengthDaily, waternetCreatorInput.PenetrationLength);

            AssertGeneralLocationValues(input, waternetCreatorInput);
            AssertIrrelevantValues(waternetCreatorInput);
        }

        private static void AssertGeneralLocationValues(UpliftVanCalculatorInput input, WaternetCreatorInput waternetCreatorInput)
        {
            Assert.AreEqual(input.WaterLevelRiverAverage, waternetCreatorInput.WaterLevelRiverAverage);
            Assert.AreEqual(input.DrainageConstruction.IsPresent, waternetCreatorInput.DrainageConstructionPresent);
            Assert.AreEqual(input.DrainageConstruction.XCoordinate, waternetCreatorInput.DrainageConstruction.X);
            Assert.AreEqual(input.DrainageConstruction.ZCoordinate, waternetCreatorInput.DrainageConstruction.Z);
            Assert.AreEqual(input.MinimumLevelPhreaticLineAtDikeTopRiver, waternetCreatorInput.MinimumLevelPhreaticLineAtDikeTopRiver);
            Assert.AreEqual(input.MinimumLevelPhreaticLineAtDikeTopPolder, waternetCreatorInput.MinimumLevelPhreaticLineAtDikeTopPolder);
            Assert.AreEqual(DikeSoilScenario.SandDikeOnClay, waternetCreatorInput.DikeSoilScenario);
            Assert.AreEqual(input.AdjustPhreaticLine3And4ForUplift, waternetCreatorInput.AdjustPl3And4ForUplift);
            Assert.AreEqual(input.LeakageLengthOutwardsPhreaticLine3, waternetCreatorInput.LeakageLengthOutwardsPl3);
            Assert.AreEqual(input.LeakageLengthInwardsPhreaticLine3, waternetCreatorInput.LeakageLengthInwardsPl3);
            Assert.AreEqual(input.LeakageLengthOutwardsPhreaticLine4, waternetCreatorInput.LeakageLengthOutwardsPl4);
            Assert.AreEqual(input.LeakageLengthInwardsPhreaticLine4, waternetCreatorInput.LeakageLengthInwardsPl4);
            Assert.AreEqual(input.PiezometricHeadPhreaticLine2Outwards, waternetCreatorInput.HeadInPlLine2Outwards);
            Assert.AreEqual(input.PiezometricHeadPhreaticLine2Inwards, waternetCreatorInput.HeadInPlLine2Inwards);
            Assert.AreEqual(9.81, waternetCreatorInput.UnitWeightWater);
        }

        private static void AssertIrrelevantValues(WaternetCreatorInput location)
        {
            Assert.IsNaN(location.WaterLevelRiverLow); // Only for macro stability outwards
        }
    }
}