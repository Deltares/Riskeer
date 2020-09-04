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
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.Input;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Test.Creators.Input
{
    [TestFixture]
    public class WaternetCreatorInputCreatorTest
    {
        [Test]
        public void Create_InputNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => WaternetCreatorInputCreator.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
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
            void Call() => WaternetCreatorInputCreator.Create(input);

            // Assert
            string message = $"The value of argument 'dikeSoilScenario' ({99}) is invalid for Enum type '{nameof(MacroStabilityInwardsDikeSoilScenario)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, message);
        }

        [Test]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay, DikeSoilScenario.ClayDikeOnClay)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.ClayDikeOnSand, DikeSoilScenario.ClayDikeOnSand)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.SandDikeOnClay, DikeSoilScenario.SandDikeOnClay)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.SandDikeOnSand, DikeSoilScenario.SandDikeOnSand)]
        public void Create_ValidDikeSoilScenario_ReturnLocationWithDikeSoilScenario(MacroStabilityInwardsDikeSoilScenario macroStabilityInwardsDikeSoilScenario,
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
            WaternetCreatorInput waternetCreatorInput = WaternetCreatorInputCreator.Create(input);

            // Assert
            Assert.AreEqual(expectedDikeSoilScenario, waternetCreatorInput.DikeSoilScenario);
        }

        [Test]
        [Combinatorial]
        public void Create_WithInput_ReturnLocation([Values(true, false)] bool drainageConstructionPresent,
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
                    PenetrationLength = penetrationLength,
                });

            // Call
            WaternetCreatorInput waternetCreatorInput = WaternetCreatorInputCreator.Create(input);

            // Assert
            Assert.AreEqual(DikeSoilScenario.SandDikeOnClay, waternetCreatorInput.DikeSoilScenario);
            Assert.AreEqual(assessmentLevel, waternetCreatorInput.WaterLevelRiver);
            Assert.AreEqual(assessmentLevel, waternetCreatorInput.HeadInPlLine3);
            Assert.AreEqual(assessmentLevel, waternetCreatorInput.HeadInPlLine4);
            Assert.AreEqual(waterLevelRiverAverage, waternetCreatorInput.WaterLevelRiverAverage);
            Assert.AreEqual(waterLevelPolderExtreme, waternetCreatorInput.WaterLevelPolder);
            Assert.AreEqual(drainageConstruction.IsPresent, waternetCreatorInput.DrainageConstructionPresent);
            Assert.AreEqual(drainageConstruction.XCoordinate, waternetCreatorInput.DrainageConstruction.X);
            Assert.AreEqual(drainageConstruction.ZCoordinate, waternetCreatorInput.DrainageConstruction.Z);
            Assert.AreEqual(minimumLevelPhreaticLineAtDikeTopRiver, waternetCreatorInput.MinimumLevelPhreaticLineAtDikeTopRiver);
            Assert.AreEqual(minimumLevelPhreaticLineAtDikeTopPolder, waternetCreatorInput.MinimumLevelPhreaticLineAtDikeTopPolder);
            Assert.AreEqual(phreaticLineOffsets.UseDefaults, waternetCreatorInput.UseDefaultOffsets);
            Assert.AreEqual(phreaticLineOffsets.BelowDikeTopAtRiver, waternetCreatorInput.PlLineOffsetBelowPointBRingtoetsWti2017);
            Assert.AreEqual(phreaticLineOffsets.BelowDikeTopAtPolder, waternetCreatorInput.PlLineOffsetBelowDikeTopAtPolder);
            Assert.AreEqual(phreaticLineOffsets.BelowShoulderBaseInside, waternetCreatorInput.PlLineOffsetBelowShoulderBaseInside);
            Assert.AreEqual(phreaticLineOffsets.BelowDikeToeAtPolder, waternetCreatorInput.PlLineOffsetBelowDikeToeAtPolder);
            Assert.AreEqual(adjustPhreaticLine3And4ForUplift, waternetCreatorInput.AdjustPl3And4ForUplift);
            Assert.AreEqual(leakageLengthOutwardsPhreaticLine3, waternetCreatorInput.LeakageLengthOutwardsPl3);
            Assert.AreEqual(leakageLengthInwardsPhreaticLine3, waternetCreatorInput.LeakageLengthInwardsPl3);
            Assert.AreEqual(leakageLengthOutwardsPhreaticLine4, waternetCreatorInput.LeakageLengthOutwardsPl4);
            Assert.AreEqual(leakageLengthInwardsPhreaticLine4, waternetCreatorInput.LeakageLengthInwardsPl4);
            Assert.AreEqual(piezometricHeadPhreaticLine2Outwards, waternetCreatorInput.HeadInPlLine2Outwards);
            Assert.AreEqual(piezometricHeadPhreaticLine2Inwards, waternetCreatorInput.HeadInPlLine2Inwards);
            Assert.AreEqual(penetrationLength, waternetCreatorInput.PenetrationLength);
            Assert.AreEqual(9.81, waternetCreatorInput.UnitWeightWater);

            AssertIrrelevantValues(waternetCreatorInput);
        }

        private static void AssertIrrelevantValues(WaternetCreatorInput waternetCreatorInput)
        {
            Assert.IsNaN(waternetCreatorInput.WaterLevelRiverLow); // Only for macro stability outwards
        }
    }
}