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
using System.ComponentModel;
using Core.Common.TestUtil;
using Deltares.WaternetCreator;
using Deltares.WTIStability;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Creators.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.Input;
using Ringtoets.MacroStabilityInwards.Primitives;
using PlLineCreationMethod = Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input.PlLineCreationMethod;
using WaternetCreationMode = Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input.WaternetCreationMode;
using WtiStabilityPlLineCreationMethod = Deltares.WaternetCreator.PlLineCreationMethod;
using WtiStabilityWaternetCreationMethod = Deltares.WaternetCreator.WaternetCreationMode;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Test.Creators.Input
{
    [TestFixture]
    public class UpliftVanStabilityLocationCreatorTest
    {
        [Test]
        public void CreateExtreme_InputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => UpliftVanStabilityLocationCreator.CreateExtreme(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
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
            TestDelegate test = () => UpliftVanStabilityLocationCreator.CreateExtreme(input);

            // Assert
            string message = $"The value of argument 'dikeSoilScenario' ({99}) is invalid for Enum type '{typeof(MacroStabilityInwardsDikeSoilScenario).Name}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, message);
        }

        [Test]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay, DikeSoilScenario.ClayDikeOnClay)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.ClayDikeOnSand, DikeSoilScenario.ClayDikeOnSand)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.SandDikeOnClay, DikeSoilScenario.SandDikeOnClay)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.SandDikeOnSand, DikeSoilScenario.SandDikeOnSand)]
        public void CreateExtreme_ValidDikeSoilScenario_ReturnStabilityLocationWithDikeSoilScenario(MacroStabilityInwardsDikeSoilScenario macroStabilityInwardsDikeSoilScenario,
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
            StabilityLocation location = UpliftVanStabilityLocationCreator.CreateExtreme(input);

            // Assert
            Assert.AreEqual(expectedDikeSoilScenario, location.DikeSoilScenario);
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
            TestDelegate test = () => UpliftVanStabilityLocationCreator.CreateExtreme(input);

            // Assert
            string message = $"The value of argument 'waternetCreationMode' ({99}) is invalid for Enum type '{typeof(WaternetCreationMode).Name}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, message);
        }

        [Test]
        [TestCase(WaternetCreationMode.CreateWaternet, WtiStabilityWaternetCreationMethod.CreateWaternet)]
        [TestCase(WaternetCreationMode.FillInWaternetValues, WtiStabilityWaternetCreationMethod.FillInWaternetValues)]
        public void CreateExtreme_ValidWaternetCreationMode_ReturnStabilityLocationWithWaternetCreationMode(WaternetCreationMode waternetCreationMode,
                                                                                                            WtiStabilityWaternetCreationMethod expectedWaternetCreationMode)
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
                    WaternetCreationMode = waternetCreationMode
                });

            // Call
            StabilityLocation location = UpliftVanStabilityLocationCreator.CreateExtreme(input);

            // Assert
            Assert.AreEqual(expectedWaternetCreationMode, location.WaternetCreationMode);
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
            TestDelegate test = () => UpliftVanStabilityLocationCreator.CreateExtreme(input);

            // Assert
            string message = $"The value of argument 'plLineCreationMethod' ({99}) is invalid for Enum type '{typeof(PlLineCreationMethod).Name}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, message);
        }

        [Test]
        [TestCase(PlLineCreationMethod.ExpertKnowledgeRrd, WtiStabilityPlLineCreationMethod.ExpertKnowledgeRrd)]
        [TestCase(PlLineCreationMethod.ExpertKnowledgeLinearInDike, WtiStabilityPlLineCreationMethod.ExpertKnowledgeLinearInDike)]
        [TestCase(PlLineCreationMethod.RingtoetsWti2017, WtiStabilityPlLineCreationMethod.RingtoetsWti2017)]
        [TestCase(PlLineCreationMethod.DupuitStatic, WtiStabilityPlLineCreationMethod.DupuitStatic)]
        [TestCase(PlLineCreationMethod.DupuitDynamic, WtiStabilityPlLineCreationMethod.DupuitDynamic)]
        [TestCase(PlLineCreationMethod.Sensors, WtiStabilityPlLineCreationMethod.Sensors)]
        [TestCase(PlLineCreationMethod.None, WtiStabilityPlLineCreationMethod.None)]
        public void CreateExtreme_ValidPlLineCreationMethod_ReturnStabilityLocationWithWaternetCreationMode(PlLineCreationMethod plLineCreationMethod,
                                                                                                            WtiStabilityPlLineCreationMethod expectedPlLineCreationMethod)
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
                    PlLineCreationMethod = plLineCreationMethod
                });

            // Call
            StabilityLocation location = UpliftVanStabilityLocationCreator.CreateExtreme(input);

            // Assert
            Assert.AreEqual(expectedPlLineCreationMethod, location.PlLineCreationMethod);
        }

        [Test]
        [Combinatorial]
        public void CreateExtreme_WithInput_ReturnStabilityLocation([Values(true, false)] bool drainageConstructionPresent,
                                                                    [Values(true, false)] bool useDefaultOffsets)
        {
            // Setup
            var random = new Random(21);
            double assessmentLevel = random.Next();
            double waterLevelRiverAverage = random.Next();
            double waterLevelPolderExtreme = random.Next();
            double minimumLevelPhreaticLineAtDikeTopRiver = random.Next();
            double minimumLevelPhreaticLineAtDikeTopPolder = random.Next();
            bool adjustPhreaticLine3And4ForUplift = random.NextBoolean();
            double leakageLengthOutwardsPhreaticLine3 = random.Next();
            double leakageLengthInwardsPhreaticLine3 = random.Next();
            double leakageLengthOutwardsPhreaticLine4 = random.Next();
            double leakageLengthInwardsPhreaticLine4 = random.Next();
            double piezometricHeadPhreaticLine2Outwards = random.Next();
            double piezometricHeadPhreaticLine2Inwards = random.Next();
            double penetrationLength = random.Next();

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
                    AssessmentLevel = assessmentLevel,
                    WaterLevelRiverAverage = waterLevelRiverAverage,
                    WaterLevelPolderExtreme = waterLevelPolderExtreme,
                    DrainageConstruction = drainageConstruction,
                    PhreaticLineOffsetsExtreme = phreaticLineOffsets,
                    PhreaticLineOffsetsDaily = new PhreaticLineOffsets(),
                    MinimumLevelPhreaticLineAtDikeTopRiver = minimumLevelPhreaticLineAtDikeTopRiver,
                    MinimumLevelPhreaticLineAtDikeTopPolder = minimumLevelPhreaticLineAtDikeTopPolder,
                    AdjustPhreaticLine3And4ForUplift = adjustPhreaticLine3And4ForUplift,
                    LeakageLengthOutwardsPhreaticLine3 = leakageLengthOutwardsPhreaticLine3,
                    LeakageLengthInwardsPhreaticLine3 = leakageLengthInwardsPhreaticLine3,
                    LeakageLengthOutwardsPhreaticLine4 = leakageLengthOutwardsPhreaticLine4,
                    LeakageLengthInwardsPhreaticLine4 = leakageLengthInwardsPhreaticLine4,
                    PiezometricHeadPhreaticLine2Outwards = piezometricHeadPhreaticLine2Outwards,
                    PiezometricHeadPhreaticLine2Inwards = piezometricHeadPhreaticLine2Inwards,
                    PenetrationLength = penetrationLength
                });

            // Call
            StabilityLocation location = UpliftVanStabilityLocationCreator.CreateExtreme(input);

            // Assert
            Assert.AreEqual(DikeSoilScenario.SandDikeOnClay, location.DikeSoilScenario);
            Assert.AreEqual(WtiStabilityWaternetCreationMethod.CreateWaternet, location.WaternetCreationMode);
            Assert.AreEqual(WtiStabilityPlLineCreationMethod.RingtoetsWti2017, location.PlLineCreationMethod);
            Assert.AreEqual(assessmentLevel, location.WaterLevelRiver);
            Assert.AreEqual(assessmentLevel, location.HeadInPlLine3);
            Assert.AreEqual(assessmentLevel, location.HeadInPlLine4);
            Assert.AreEqual(waterLevelRiverAverage, location.WaterLevelRiverAverage);
            Assert.AreEqual(waterLevelPolderExtreme, location.WaterLevelPolder);
            Assert.AreEqual(drainageConstruction.IsPresent, location.DrainageConstructionPresent);
            Assert.AreEqual(drainageConstruction.XCoordinate, location.XCoordMiddleDrainageConstruction);
            Assert.AreEqual(drainageConstruction.ZCoordinate, location.ZCoordMiddleDrainageConstruction);
            Assert.AreEqual(minimumLevelPhreaticLineAtDikeTopRiver, location.MinimumLevelPhreaticLineAtDikeTopRiver);
            Assert.AreEqual(minimumLevelPhreaticLineAtDikeTopPolder, location.MinimumLevelPhreaticLineAtDikeTopPolder);
            Assert.AreEqual(phreaticLineOffsets.UseDefaults, location.UseDefaultOffsets);
            Assert.AreEqual(phreaticLineOffsets.BelowDikeTopAtRiver, location.PlLineOffsetBelowPointBRingtoetsWti2017);
            Assert.AreEqual(phreaticLineOffsets.BelowDikeTopAtPolder, location.PlLineOffsetBelowDikeTopAtPolder);
            Assert.AreEqual(phreaticLineOffsets.BelowShoulderBaseInside, location.PlLineOffsetBelowShoulderBaseInside);
            Assert.AreEqual(phreaticLineOffsets.BelowDikeToeAtPolder, location.PlLineOffsetBelowDikeToeAtPolder);
            Assert.AreEqual(adjustPhreaticLine3And4ForUplift, location.AdjustPl3And4ForUplift);
            Assert.AreEqual(leakageLengthOutwardsPhreaticLine3, location.LeakageLengthOutwardsPl3);
            Assert.AreEqual(leakageLengthInwardsPhreaticLine3, location.LeakageLengthInwardsPl3);
            Assert.AreEqual(leakageLengthOutwardsPhreaticLine4, location.LeakageLengthOutwardsPl4);
            Assert.AreEqual(leakageLengthInwardsPhreaticLine4, location.LeakageLengthInwardsPl4);
            Assert.AreEqual(piezometricHeadPhreaticLine2Outwards, location.HeadInPlLine2Outwards);
            Assert.AreEqual(piezometricHeadPhreaticLine2Inwards, location.HeadInPlLine2Inwards);
            Assert.AreEqual(penetrationLength, location.PenetrationLength);

            AssertIrrelevantValues(location);
        }

        [Test]
        public void CreateDaily_InputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => UpliftVanStabilityLocationCreator.CreateDaily(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
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
            TestDelegate test = () => UpliftVanStabilityLocationCreator.CreateDaily(input);

            // Assert
            string message = $"The value of argument 'dikeSoilScenario' ({99}) is invalid for Enum type '{typeof(MacroStabilityInwardsDikeSoilScenario).Name}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, message);
        }

        [Test]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay, DikeSoilScenario.ClayDikeOnClay)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.ClayDikeOnSand, DikeSoilScenario.ClayDikeOnSand)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.SandDikeOnClay, DikeSoilScenario.SandDikeOnClay)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.SandDikeOnSand, DikeSoilScenario.SandDikeOnSand)]
        public void CreateDaily_ValidDikeSoilScenario_ReturnStabilityLocationWithDikeSoilScenario(MacroStabilityInwardsDikeSoilScenario macroStabilityInwardsDikeSoilScenario,
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
            StabilityLocation location = UpliftVanStabilityLocationCreator.CreateDaily(input);

            // Assert
            Assert.AreEqual(expectedDikeSoilScenario, location.DikeSoilScenario);
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
            TestDelegate test = () => UpliftVanStabilityLocationCreator.CreateDaily(input);

            // Assert
            string message = $"The value of argument 'waternetCreationMode' ({99}) is invalid for Enum type '{typeof(WaternetCreationMode).Name}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, message);
        }

        [Test]
        [TestCase(WaternetCreationMode.CreateWaternet, WtiStabilityWaternetCreationMethod.CreateWaternet)]
        [TestCase(WaternetCreationMode.FillInWaternetValues, WtiStabilityWaternetCreationMethod.FillInWaternetValues)]
        public void CreateDaily_ValidWaternetCreationMode_ReturnStabilityLocationWithWaternetCreationMode(WaternetCreationMode waternetCreationMode,
                                                                                                          WtiStabilityWaternetCreationMethod expectedWaternetCreationMode)
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
                    WaternetCreationMode = waternetCreationMode
                });

            // Call
            StabilityLocation location = UpliftVanStabilityLocationCreator.CreateDaily(input);

            // Assert
            Assert.AreEqual(expectedWaternetCreationMode, location.WaternetCreationMode);
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
            TestDelegate test = () => UpliftVanStabilityLocationCreator.CreateDaily(input);

            // Assert
            string message = $"The value of argument 'plLineCreationMethod' ({99}) is invalid for Enum type '{typeof(PlLineCreationMethod).Name}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, message);
        }

        [Test]
        [TestCase(PlLineCreationMethod.ExpertKnowledgeRrd, WtiStabilityPlLineCreationMethod.ExpertKnowledgeRrd)]
        [TestCase(PlLineCreationMethod.ExpertKnowledgeLinearInDike, WtiStabilityPlLineCreationMethod.ExpertKnowledgeLinearInDike)]
        [TestCase(PlLineCreationMethod.RingtoetsWti2017, WtiStabilityPlLineCreationMethod.RingtoetsWti2017)]
        [TestCase(PlLineCreationMethod.DupuitStatic, WtiStabilityPlLineCreationMethod.DupuitStatic)]
        [TestCase(PlLineCreationMethod.DupuitDynamic, WtiStabilityPlLineCreationMethod.DupuitDynamic)]
        [TestCase(PlLineCreationMethod.Sensors, WtiStabilityPlLineCreationMethod.Sensors)]
        [TestCase(PlLineCreationMethod.None, WtiStabilityPlLineCreationMethod.None)]
        public void CreateDaily_ValidPlLineCreationMethod_ReturnStabilityLocationWithWaternetCreationMode(PlLineCreationMethod plLineCreationMethod,
                                                                                                          WtiStabilityPlLineCreationMethod expectedPlLineCreationMethod)
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
                    PlLineCreationMethod = plLineCreationMethod
                });

            // Call
            StabilityLocation location = UpliftVanStabilityLocationCreator.CreateDaily(input);

            // Assert
            Assert.AreEqual(expectedPlLineCreationMethod, location.PlLineCreationMethod);
        }

        [Test]
        [Combinatorial]
        public void CreateDaily_WithInput_ReturnStabilityLocation([Values(true, false)] bool drainageConstructionPresent,
                                                                  [Values(true, false)] bool useDefaultOffsets)
        {
            // Setup
            var random = new Random(21);
            double assessmentLevel = random.Next();
            double waterLevelRiverAverage = random.Next();
            double waterLevelPolderDaily = random.Next();
            double minimumLevelPhreaticLineAtDikeTopRiver = random.Next();
            double minimumLevelPhreaticLineAtDikeTopPolder = random.Next();
            bool adjustPhreaticLine3And4ForUplift = random.NextBoolean();
            double leakageLengthOutwardsPhreaticLine3 = random.Next();
            double leakageLengthInwardsPhreaticLine3 = random.Next();
            double leakageLengthOutwardsPhreaticLine4 = random.Next();
            double leakageLengthInwardsPhreaticLine4 = random.Next();
            double piezometricHeadPhreaticLine2Outwards = random.Next();
            double piezometricHeadPhreaticLine2Inwards = random.Next();

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
                    AssessmentLevel = assessmentLevel,
                    WaterLevelRiverAverage = waterLevelRiverAverage,
                    WaterLevelPolderDaily = waterLevelPolderDaily,
                    DrainageConstruction = drainageConstruction,
                    PhreaticLineOffsetsExtreme = new PhreaticLineOffsets(),
                    PhreaticLineOffsetsDaily = phreaticLineOffsets,
                    MinimumLevelPhreaticLineAtDikeTopRiver = minimumLevelPhreaticLineAtDikeTopRiver,
                    MinimumLevelPhreaticLineAtDikeTopPolder = minimumLevelPhreaticLineAtDikeTopPolder,
                    AdjustPhreaticLine3And4ForUplift = adjustPhreaticLine3And4ForUplift,
                    LeakageLengthOutwardsPhreaticLine3 = leakageLengthOutwardsPhreaticLine3,
                    LeakageLengthInwardsPhreaticLine3 = leakageLengthInwardsPhreaticLine3,
                    LeakageLengthOutwardsPhreaticLine4 = leakageLengthOutwardsPhreaticLine4,
                    LeakageLengthInwardsPhreaticLine4 = leakageLengthInwardsPhreaticLine4,
                    PiezometricHeadPhreaticLine2Outwards = piezometricHeadPhreaticLine2Outwards,
                    PiezometricHeadPhreaticLine2Inwards = piezometricHeadPhreaticLine2Inwards,
                    PenetrationLength = random.Next()
                });

            // Call
            StabilityLocation location = UpliftVanStabilityLocationCreator.CreateDaily(input);

            // Assert
            Assert.AreEqual(DikeSoilScenario.SandDikeOnClay, location.DikeSoilScenario);
            Assert.AreEqual(WtiStabilityWaternetCreationMethod.CreateWaternet, location.WaternetCreationMode);
            Assert.AreEqual(WtiStabilityPlLineCreationMethod.RingtoetsWti2017, location.PlLineCreationMethod);
            Assert.AreEqual(assessmentLevel, location.WaterLevelRiver);
            Assert.AreEqual(assessmentLevel, location.HeadInPlLine3);
            Assert.AreEqual(assessmentLevel, location.HeadInPlLine4);
            Assert.AreEqual(waterLevelRiverAverage, location.WaterLevelRiverAverage);
            Assert.AreEqual(waterLevelPolderDaily, location.WaterLevelPolder);
            Assert.AreEqual(drainageConstruction.IsPresent, location.DrainageConstructionPresent);
            Assert.AreEqual(drainageConstruction.XCoordinate, location.XCoordMiddleDrainageConstruction);
            Assert.AreEqual(drainageConstruction.ZCoordinate, location.ZCoordMiddleDrainageConstruction);
            Assert.AreEqual(minimumLevelPhreaticLineAtDikeTopRiver, location.MinimumLevelPhreaticLineAtDikeTopRiver);
            Assert.AreEqual(minimumLevelPhreaticLineAtDikeTopPolder, location.MinimumLevelPhreaticLineAtDikeTopPolder);
            Assert.AreEqual(phreaticLineOffsets.UseDefaults, location.UseDefaultOffsets);
            Assert.AreEqual(phreaticLineOffsets.BelowDikeTopAtRiver, location.PlLineOffsetBelowPointBRingtoetsWti2017);
            Assert.AreEqual(phreaticLineOffsets.BelowDikeTopAtPolder, location.PlLineOffsetBelowDikeTopAtPolder);
            Assert.AreEqual(phreaticLineOffsets.BelowShoulderBaseInside, location.PlLineOffsetBelowShoulderBaseInside);
            Assert.AreEqual(phreaticLineOffsets.BelowDikeToeAtPolder, location.PlLineOffsetBelowDikeToeAtPolder);
            Assert.AreEqual(adjustPhreaticLine3And4ForUplift, location.AdjustPl3And4ForUplift);
            Assert.AreEqual(leakageLengthOutwardsPhreaticLine3, location.LeakageLengthOutwardsPl3);
            Assert.AreEqual(leakageLengthInwardsPhreaticLine3, location.LeakageLengthInwardsPl3);
            Assert.AreEqual(leakageLengthOutwardsPhreaticLine4, location.LeakageLengthOutwardsPl4);
            Assert.AreEqual(leakageLengthInwardsPhreaticLine4, location.LeakageLengthInwardsPl4);
            Assert.AreEqual(piezometricHeadPhreaticLine2Outwards, location.HeadInPlLine2Outwards);
            Assert.AreEqual(piezometricHeadPhreaticLine2Inwards, location.HeadInPlLine2Inwards);
            Assert.AreEqual(0.0, location.PenetrationLength);

            AssertIrrelevantValues(location);
        }

        private static void AssertIrrelevantValues(StabilityLocation location)
        {
            Assert.IsNaN(location.WaterLevelRiverLow); // Only for macro stability outwards
            Assert.AreEqual(0.0, location.X); // Unused property
            Assert.AreEqual(0.0, location.Y); // Unused property
            Assert.IsTrue(string.IsNullOrEmpty(location.PiezometricHeads.Name)); // Unused property
            Assert.IsNaN(location.PiezometricHeads.HeadPl3); // Unused property
            Assert.AreEqual(0.30, location.PiezometricHeads.DampingFactorPl3); // Unused property
            Assert.IsNaN(location.PiezometricHeads.HeadPl4); // Unused property
            Assert.AreEqual(0.30, location.PiezometricHeads.DampingFactorPl4); // Unused property
        }
    }
}