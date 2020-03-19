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
using Deltares.MacroStability.WaternetCreator;
using Deltares.WTIStability;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.Input;
using Riskeer.MacroStabilityInwards.Primitives;
using PlLineCreationMethod = Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input.PlLineCreationMethod;
using WaternetCreationMode = Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input.WaternetCreationMode;
using WtiStabilityPlLineCreationMethod = Deltares.MacroStability.WaternetCreator.PlLineCreationMethod;
using WtiStabilityWaternetCreationMethod = Deltares.MacroStability.WaternetCreator.WaternetCreationMode;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Test.Creators.Input
{
    [TestFixture]
    public class WaternetStabilityLocationCreatorTest
    {
        [Test]
        public void Create_InputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => WaternetStabilityLocationCreator.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("input", exception.ParamName);
        }

        [Test]
        public void Create_InvalidDikeSoilScenario_ThrowInvalidEnumArgumentException()
        {
            // Setup
            var input = new WaternetCalculatorInput(
                new WaternetCalculatorInput.ConstructionProperties
                {
                    SurfaceLine = new MacroStabilityInwardsSurfaceLine("test"),
                    SoilProfile = new TestSoilProfile(),
                    PhreaticLineOffsets = new PhreaticLineOffsets(),
                    DrainageConstruction = new DrainageConstruction(),
                    DikeSoilScenario = (MacroStabilityInwardsDikeSoilScenario) 99
                });

            // Call
            TestDelegate test = () => WaternetStabilityLocationCreator.Create(input);

            // Assert
            string message = $"The value of argument 'dikeSoilScenario' ({99}) is invalid for Enum type '{typeof(MacroStabilityInwardsDikeSoilScenario).Name}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, message);
        }

        [Test]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay, DikeSoilScenario.ClayDikeOnClay)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.ClayDikeOnSand, DikeSoilScenario.ClayDikeOnSand)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.SandDikeOnClay, DikeSoilScenario.SandDikeOnClay)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.SandDikeOnSand, DikeSoilScenario.SandDikeOnSand)]
        public void Create_ValidDikeSoilScenario_ReturnStabilityLocationWithDikeSoilScenario(MacroStabilityInwardsDikeSoilScenario macroStabilityInwardsDikeSoilScenario,
                                                                                             DikeSoilScenario expectedDikeSoilScenario)
        {
            // Setup
            var input = new WaternetCalculatorInput(
                new WaternetCalculatorInput.ConstructionProperties
                {
                    DrainageConstruction = new DrainageConstruction(),
                    PhreaticLineOffsets = new PhreaticLineOffsets(),
                    SurfaceLine = new MacroStabilityInwardsSurfaceLine("test"),
                    SoilProfile = new TestSoilProfile(),
                    DikeSoilScenario = macroStabilityInwardsDikeSoilScenario
                });

            // Call
            StabilityLocation location = WaternetStabilityLocationCreator.Create(input);

            // Assert
            Assert.AreEqual(expectedDikeSoilScenario, location.DikeSoilScenario);
        }

        [Test]
        public void Create_InvalidWaternetCreationMode_ThrowInvalidEnumArgumentException()
        {
            // Setup
            var input = new WaternetCalculatorInput(
                new WaternetCalculatorInput.ConstructionProperties
                {
                    SurfaceLine = new MacroStabilityInwardsSurfaceLine("test"),
                    SoilProfile = new TestSoilProfile(),
                    PhreaticLineOffsets = new PhreaticLineOffsets(),
                    DrainageConstruction = new DrainageConstruction(),
                    WaternetCreationMode = (WaternetCreationMode) 99
                });

            // Call
            TestDelegate test = () => WaternetStabilityLocationCreator.Create(input);

            // Assert
            string message = $"The value of argument 'waternetCreationMode' ({99}) is invalid for Enum type '{typeof(WaternetCreationMode).Name}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, message);
        }

        [Test]
        [TestCase(WaternetCreationMode.CreateWaternet, WtiStabilityWaternetCreationMethod.CreateWaternet)]
        [TestCase(WaternetCreationMode.FillInWaternetValues, WtiStabilityWaternetCreationMethod.FillInWaternetValues)]
        public void Create_ValidWaternetCreationMode_ReturnStabilityLocationWithWaternetCreationMode(WaternetCreationMode waternetCreationMode,
                                                                                                     WtiStabilityWaternetCreationMethod expectedWaternetCreationMode)
        {
            // Setup
            var input = new WaternetCalculatorInput(
                new WaternetCalculatorInput.ConstructionProperties
                {
                    DrainageConstruction = new DrainageConstruction(),
                    PhreaticLineOffsets = new PhreaticLineOffsets(),
                    SurfaceLine = new MacroStabilityInwardsSurfaceLine("test"),
                    SoilProfile = new TestSoilProfile(),
                    WaternetCreationMode = waternetCreationMode
                });

            // Call
            StabilityLocation location = WaternetStabilityLocationCreator.Create(input);

            // Assert
            Assert.AreEqual(expectedWaternetCreationMode, location.WaternetCreationMode);
        }

        [Test]
        public void Create_InvalidPlLineCreationMethod_ThrowInvalidEnumArgumentException()
        {
            // Setup
            var input = new WaternetCalculatorInput(
                new WaternetCalculatorInput.ConstructionProperties
                {
                    SurfaceLine = new MacroStabilityInwardsSurfaceLine("test"),
                    SoilProfile = new TestSoilProfile(),
                    PhreaticLineOffsets = new PhreaticLineOffsets(),
                    DrainageConstruction = new DrainageConstruction(),
                    PlLineCreationMethod = (PlLineCreationMethod) 99
                });

            // Call
            TestDelegate test = () => WaternetStabilityLocationCreator.Create(input);

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
        public void Create_ValidPlLineCreationMethod_ReturnStabilityLocationWithWaternetCreationMode(PlLineCreationMethod plLineCreationMethod,
                                                                                                     WtiStabilityPlLineCreationMethod expectedPlLineCreationMethod)
        {
            // Setup
            var input = new WaternetCalculatorInput(
                new WaternetCalculatorInput.ConstructionProperties
                {
                    DrainageConstruction = new DrainageConstruction(),
                    PhreaticLineOffsets = new PhreaticLineOffsets(),
                    SurfaceLine = new MacroStabilityInwardsSurfaceLine("test"),
                    SoilProfile = new TestSoilProfile(),
                    PlLineCreationMethod = plLineCreationMethod
                });

            // Call
            StabilityLocation location = WaternetStabilityLocationCreator.Create(input);

            // Assert
            Assert.AreEqual(expectedPlLineCreationMethod, location.PlLineCreationMethod);
        }

        [Test]
        [Combinatorial]
        public void Create_WithInput_ReturnStabilityLocation([Values(true, false)] bool drainageConstructionPresent,
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

            var input = new WaternetCalculatorInput(
                new WaternetCalculatorInput.ConstructionProperties
                {
                    SurfaceLine = new MacroStabilityInwardsSurfaceLine("test"),
                    SoilProfile = new TestSoilProfile(),
                    DikeSoilScenario = MacroStabilityInwardsDikeSoilScenario.SandDikeOnClay,
                    AssessmentLevel = assessmentLevel,
                    WaterLevelRiverAverage = waterLevelRiverAverage,
                    WaterLevelPolder = waterLevelPolderExtreme,
                    DrainageConstruction = drainageConstruction,
                    PhreaticLineOffsets = phreaticLineOffsets,
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
            StabilityLocation location = WaternetStabilityLocationCreator.Create(input);

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