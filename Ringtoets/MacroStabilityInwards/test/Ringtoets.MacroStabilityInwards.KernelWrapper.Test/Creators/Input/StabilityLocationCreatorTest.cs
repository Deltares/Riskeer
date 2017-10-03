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
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Creators.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.UpliftVan.Input;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Test.Creators.Input
{
    [TestFixture]
    public class StabilityLocationCreatorTest
    {
        [Test]
        public void Create_InputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => StabilityLocationCreator.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("input", exception.ParamName);
        }

        [Test]
        public void Create_InvalidDikeSoilScenario_ThrowInvalidEnumArgumentException()
        {
            // Setup
            var input = new UpliftVanCalculatorInput(
                new UpliftVanCalculatorInput.ConstructionProperties
                {
                    SurfaceLine = new MacroStabilityInwardsSurfaceLine("test"),
                    SoilProfile = new TestUpliftVanSoilProfile(),
                    PhreaticLineOffsets = new UpliftVanPhreaticLineOffsets(),
                    DrainageConstruction = new UpliftVanDrainageConstruction(),
                    DikeSoilScenario = (MacroStabilityInwardsDikeSoilScenario) 99
                });

            // Call
            TestDelegate test = () => StabilityLocationCreator.Create(input);

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
                                                                                             DikeSoilScenario dikeSoilScenario)
        {
            // Setup
            var input = new UpliftVanCalculatorInput(
                new UpliftVanCalculatorInput.ConstructionProperties
                {
                    DrainageConstruction = new UpliftVanDrainageConstruction(),
                    PhreaticLineOffsets = new UpliftVanPhreaticLineOffsets(),
                    SurfaceLine = new MacroStabilityInwardsSurfaceLine("test"),
                    SoilProfile = new TestUpliftVanSoilProfile(),
                    DikeSoilScenario = macroStabilityInwardsDikeSoilScenario
                });

            // Call
            StabilityLocation location = StabilityLocationCreator.Create(input);

            // Assert
            Assert.AreEqual(dikeSoilScenario, location.DikeSoilScenario);
        }

        [Test]
        public void Create_InvalidWaternetCreationMode_ThrowInvalidEnumArgumentException()
        {
            // Setup
            var input = new UpliftVanCalculatorInput(
                new UpliftVanCalculatorInput.ConstructionProperties
                {
                    SurfaceLine = new MacroStabilityInwardsSurfaceLine("test"),
                    SoilProfile = new TestUpliftVanSoilProfile(),
                    PhreaticLineOffsets = new UpliftVanPhreaticLineOffsets(),
                    DrainageConstruction = new UpliftVanDrainageConstruction(),
                    WaternetCreationMode = (UpliftVanWaternetCreationMode) 99
                });

            // Call
            TestDelegate test = () => StabilityLocationCreator.Create(input);

            // Assert
            string message = $"The value of argument 'waternetCreationMode' ({99}) is invalid for Enum type '{typeof(UpliftVanWaternetCreationMode).Name}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, message);
        }

        [Test]
        [TestCase(UpliftVanWaternetCreationMode.CreateWaternet, WaternetCreationMode.CreateWaternet)]
        [TestCase(UpliftVanWaternetCreationMode.FillInWaternetValues, WaternetCreationMode.FillInWaternetValues)]
        public void Create_ValidWaternetCreationMode_ReturnStabilityLocationWithWaternetCreationMode(UpliftVanWaternetCreationMode upliftVanWaternetCreationMode,
                                                                                                     WaternetCreationMode waternetCreationMode)
        {
            // Setup
            var input = new UpliftVanCalculatorInput(
                new UpliftVanCalculatorInput.ConstructionProperties
                {
                    DrainageConstruction = new UpliftVanDrainageConstruction(),
                    PhreaticLineOffsets = new UpliftVanPhreaticLineOffsets(),
                    SurfaceLine = new MacroStabilityInwardsSurfaceLine("test"),
                    SoilProfile = new TestUpliftVanSoilProfile(),
                    WaternetCreationMode = upliftVanWaternetCreationMode
                });

            // Call
            StabilityLocation location = StabilityLocationCreator.Create(input);

            // Assert
            Assert.AreEqual(waternetCreationMode, location.WaternetCreationMode);
        }

        [Test]
        public void Create_InvalidPlLineCreationMethod_ThrowInvalidEnumArgumentException()
        {
            // Setup
            var input = new UpliftVanCalculatorInput(
                new UpliftVanCalculatorInput.ConstructionProperties
                {
                    SurfaceLine = new MacroStabilityInwardsSurfaceLine("test"),
                    SoilProfile = new TestUpliftVanSoilProfile(),
                    PhreaticLineOffsets = new UpliftVanPhreaticLineOffsets(),
                    DrainageConstruction = new UpliftVanDrainageConstruction(),
                    PlLineCreationMethod = (UpliftVanPlLineCreationMethod) 99
                });

            // Call
            TestDelegate test = () => StabilityLocationCreator.Create(input);

            // Assert
            string message = $"The value of argument 'plLineCreationMethod' ({99}) is invalid for Enum type '{typeof(UpliftVanPlLineCreationMethod).Name}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, message);
        }

        [Test]
        [TestCase(UpliftVanPlLineCreationMethod.ExpertKnowledgeRrd, PlLineCreationMethod.ExpertKnowledgeRrd)]
        [TestCase(UpliftVanPlLineCreationMethod.ExpertKnowledgeLinearInDike, PlLineCreationMethod.ExpertKnowledgeLinearInDike)]
        [TestCase(UpliftVanPlLineCreationMethod.RingtoetsWti2017, PlLineCreationMethod.RingtoetsWti2017)]
        [TestCase(UpliftVanPlLineCreationMethod.DupuitStatic, PlLineCreationMethod.DupuitStatic)]
        [TestCase(UpliftVanPlLineCreationMethod.DupuitDynamic, PlLineCreationMethod.DupuitDynamic)]
        [TestCase(UpliftVanPlLineCreationMethod.Sensors, PlLineCreationMethod.Sensors)]
        [TestCase(UpliftVanPlLineCreationMethod.None, PlLineCreationMethod.None)]
        public void Create_ValidPlLineCreationMethod_ReturnStabilityLocationWithWaternetCreationMode(UpliftVanPlLineCreationMethod upliftVanPlLineCreationMethod,
                                                                                                     PlLineCreationMethod plLineCreationMethod)
        {
            // Setup
            var input = new UpliftVanCalculatorInput(
                new UpliftVanCalculatorInput.ConstructionProperties
                {
                    DrainageConstruction = new UpliftVanDrainageConstruction(),
                    PhreaticLineOffsets = new UpliftVanPhreaticLineOffsets(),
                    SurfaceLine = new MacroStabilityInwardsSurfaceLine("test"),
                    SoilProfile = new TestUpliftVanSoilProfile(),
                    PlLineCreationMethod = upliftVanPlLineCreationMethod
                });

            // Call
            StabilityLocation location = StabilityLocationCreator.Create(input);

            // Assert
            Assert.AreEqual(plLineCreationMethod, location.PlLineCreationMethod);
        }

        [Test]
        public void Create_WithInput_ReturnStabilityLocation()
        {
            // Setup
            var random = new Random(21);
            double assessmentLevel = random.Next();
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
            bool adjustPhreaticLine3And4ForUplift = random.NextBoolean();
            double leakageLengthOutwardsPhreaticLine3 = random.Next();
            double leakageLengthInwardsPhreaticLine3 = random.Next();
            double leakageLengthOutwardsPhreaticLine4 = random.Next();
            double leakageLengthInwardsPhreaticLine4 = random.Next();
            double piezometricHeadPhreaticLine2Outwards = random.Next();
            double piezometricHeadPhreaticLine2Inwards = random.Next();
            double penetrationLength = random.Next();

            var drainageConstruction = new UpliftVanDrainageConstruction(xCoordinateDrainageConstruction, zCoordinateDrainageConstruction);
            var phreaticLineOffsets = new UpliftVanPhreaticLineOffsets(phreaticLineOffsetBelowDikeTopAtRiver, phreaticLineOffsetBelowDikeTopAtPolder,
                                                                       phreaticLineOffsetBelowDikeToeAtPolder, phreaticLineOffsetBelowShoulderBaseInside);

            var input = new UpliftVanCalculatorInput(
                new UpliftVanCalculatorInput.ConstructionProperties
                {
                    SurfaceLine = new MacroStabilityInwardsSurfaceLine("test"),
                    SoilProfile = new TestUpliftVanSoilProfile(),
                    DikeSoilScenario = MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay,
                    AssessmentLevel = assessmentLevel,
                    WaterLevelRiverAverage = waterLevelRiverAverage,
                    WaterLevelPolder = waterLevelPolder,
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
            StabilityLocation location = StabilityLocationCreator.Create(input);

            // Assert
            Assert.AreEqual(DikeSoilScenario.ClayDikeOnClay, location.DikeSoilScenario);
            Assert.AreEqual(WaternetCreationMode.CreateWaternet, location.WaternetCreationMode);
            Assert.AreEqual(PlLineCreationMethod.RingtoetsWti2017, location.PlLineCreationMethod);
            Assert.AreEqual(assessmentLevel, location.WaterLevelRiver);
            Assert.AreEqual(assessmentLevel, location.HeadInPlLine3);
            Assert.AreEqual(assessmentLevel, location.HeadInPlLine4);
            Assert.AreEqual(waterLevelRiverAverage, location.WaterLevelRiverAverage);
            Assert.AreEqual(waterLevelPolder, location.WaterLevelPolder);
            Assert.AreEqual(xCoordinateDrainageConstruction, location.XCoordMiddleDrainageConstruction);
            Assert.AreEqual(zCoordinateDrainageConstruction, location.ZCoordMiddleDrainageConstruction);
            Assert.AreEqual(minimumLevelPhreaticLineAtDikeTopRiver, location.MinimumLevelPhreaticLineAtDikeTopRiver);
            Assert.AreEqual(minimumLevelPhreaticLineAtDikeTopPolder, location.MinimumLevelPhreaticLineAtDikeTopPolder);
            Assert.IsFalse(location.UseDefaultOffsets);
            Assert.AreEqual(phreaticLineOffsetBelowDikeTopAtRiver, location.PlLineOffsetBelowPointBRingtoetsWti2017);
            Assert.AreEqual(phreaticLineOffsetBelowDikeTopAtPolder, location.PlLineOffsetBelowDikeTopAtPolder);
            Assert.AreEqual(phreaticLineOffsetBelowShoulderBaseInside, location.PlLineOffsetBelowShoulderBaseInside);
            Assert.AreEqual(phreaticLineOffsetBelowDikeToeAtPolder, location.PlLineOffsetBelowDikeToeAtPolder);
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