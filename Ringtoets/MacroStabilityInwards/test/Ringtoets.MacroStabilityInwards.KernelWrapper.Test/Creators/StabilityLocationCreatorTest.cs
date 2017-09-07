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
using Ringtoets.MacroStabilityInwards.KernelWrapper.Creators;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Test.Creators
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
            var input = new MacroStabilityInwardsCalculatorInput(
                new MacroStabilityInwardsCalculatorInput.ConstructionProperties
                {
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
            var input = new MacroStabilityInwardsCalculatorInput(
                new MacroStabilityInwardsCalculatorInput.ConstructionProperties
                {
                    DikeSoilScenario = macroStabilityInwardsDikeSoilScenario
                });

            // Call
            StabilityLocation location = StabilityLocationCreator.Create(input);

            // Assert
            Assert.AreEqual(dikeSoilScenario, location.DikeSoilScenario);
        }

        [Test]
        public void Create_WithInput_ReturnStabilityLocation()
        {
            // Setup
            var random = new Random(21);
            double assessmentLevel = random.Next();
            double waterLevelRiverAverage = random.Next();
            double waterLevelPolder = random.Next();
            bool drainageConstructionPresent = random.NextBoolean();
            double xCoordinateDrainageConstruction = random.Next();
            double zCoordinateDrainageConstruction = random.Next();
            double minimumLevelPhreaticLineAtDikeTopRiver = random.Next();
            double minimumLevelPhreaticLineAtDikeTopPolder = random.Next();
            bool useDefaultOffsets = random.NextBoolean();
            double phreaticLineOffsetBelowDikeTopAtRiver = random.Next();
            double phreaticLineOffsetBelowDikeTopAtPolder = random.Next();
            double phreaticLineOffsetBelowShoulderBaseInside = random.Next();
            double phreaticLineOffsetBelowDikeToeAtPolder = random.Next();
            double piezometricHeadPhreaticLine2Outwards = random.Next();
            double piezometricHeadPhreaticLine2Inwards = random.Next();
            bool adjustPhreaticLine3And4ForUplift = random.NextBoolean();
            double penetrationLength = random.Next();
            double leakageLengthOutwardsPhreaticLine3 = random.Next();
            double leakageLengthInwardsPhreaticLine3 = random.Next();
            double leakageLengthOutwardsPhreaticLine4 = random.Next();
            double leakageLengthInwardsPhreaticLine4 = random.Next();

            var input = new MacroStabilityInwardsCalculatorInput(
                new MacroStabilityInwardsCalculatorInput.ConstructionProperties
                {
                    DikeSoilScenario = MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay,
                    AssessmentLevel = assessmentLevel,
                    WaterLevelRiverAverage = waterLevelRiverAverage,
                    WaterLevelPolder = waterLevelPolder,
                    DrainageConstructionPresent = drainageConstructionPresent,
                    XCoordinateDrainageConstruction = xCoordinateDrainageConstruction,
                    ZCoordinateDrainageConstruction = zCoordinateDrainageConstruction,
                    MinimumLevelPhreaticLineAtDikeTopRiver = minimumLevelPhreaticLineAtDikeTopRiver,
                    MinimumLevelPhreaticLineAtDikeTopPolder = minimumLevelPhreaticLineAtDikeTopPolder,
                    UseDefaultOffsets = useDefaultOffsets,
                    PhreaticLineOffsetBelowDikeTopAtRiver = phreaticLineOffsetBelowDikeTopAtRiver,
                    PhreaticLineOffsetBelowDikeTopAtPolder = phreaticLineOffsetBelowDikeTopAtPolder,
                    PhreaticLineOffsetBelowShoulderBaseInside = phreaticLineOffsetBelowShoulderBaseInside,
                    PhreaticLineOffsetBelowDikeToeAtPolder = phreaticLineOffsetBelowDikeToeAtPolder,
                    PiezometricHeadPhreaticLine2Outwards = piezometricHeadPhreaticLine2Outwards,
                    PiezometricHeadPhreaticLine2Inwards = piezometricHeadPhreaticLine2Inwards,
                    AdjustPhreaticLine3And4ForUplift = adjustPhreaticLine3And4ForUplift,
                    PenetrationLength = penetrationLength,
                    LeakageLengthOutwardsPhreaticLine3 = leakageLengthOutwardsPhreaticLine3,
                    LeakageLengthInwardsPhreaticLine3 = leakageLengthInwardsPhreaticLine3,
                    LeakageLengthOutwardsPhreaticLine4 = leakageLengthOutwardsPhreaticLine4,
                    LeakageLengthInwardsPhreaticLine4 = leakageLengthInwardsPhreaticLine4,
                });

            // Call
            StabilityLocation location = StabilityLocationCreator.Create(input);

            // Assert
            Assert.AreEqual(DikeSoilScenario.ClayDikeOnClay, location.DikeSoilScenario);
            Assert.AreEqual(assessmentLevel, location.WaterLevelRiver);
            Assert.AreEqual(assessmentLevel, location.HeadInPlLine3);
            Assert.AreEqual(assessmentLevel, location.HeadInPlLine4);
            Assert.AreEqual(waterLevelRiverAverage, location.WaterLevelRiverAverage);
            Assert.AreEqual(waterLevelPolder, location.WaterLevelPolder);
            Assert.IsNaN(location.WaterLevelRiverLow);
            Assert.AreEqual(drainageConstructionPresent, location.DrainageConstructionPresent);
            Assert.AreEqual(xCoordinateDrainageConstruction, location.XCoordMiddleDrainageConstruction);
            Assert.AreEqual(zCoordinateDrainageConstruction, location.ZCoordMiddleDrainageConstruction);
            Assert.AreEqual(minimumLevelPhreaticLineAtDikeTopRiver, location.MinimumLevelPhreaticLineAtDikeTopRiver);
            Assert.AreEqual(minimumLevelPhreaticLineAtDikeTopPolder, location.MinimumLevelPhreaticLineAtDikeTopPolder);
            Assert.AreEqual(useDefaultOffsets, location.UseDefaultOffsets);
            Assert.AreEqual(phreaticLineOffsetBelowDikeTopAtRiver, location.PlLineOffsetBelowPointBRingtoetsWti2017);
            Assert.AreEqual(phreaticLineOffsetBelowDikeTopAtPolder, location.PlLineOffsetBelowDikeTopAtPolder);
            Assert.AreEqual(phreaticLineOffsetBelowShoulderBaseInside, location.PlLineOffsetBelowShoulderBaseInside);
            Assert.AreEqual(phreaticLineOffsetBelowDikeToeAtPolder, location.PlLineOffsetBelowDikeToeAtPolder);
            Assert.AreEqual(piezometricHeadPhreaticLine2Outwards, location.HeadInPlLine2Outwards);
            Assert.AreEqual(piezometricHeadPhreaticLine2Inwards, location.HeadInPlLine2Inwards);
            Assert.AreEqual(adjustPhreaticLine3And4ForUplift, location.AdjustPl3And4ForUplift);
            Assert.AreEqual(penetrationLength, location.PenetrationLength);
            Assert.AreEqual(leakageLengthOutwardsPhreaticLine3, location.LeakageLengthOutwardsPl3);
            Assert.AreEqual(leakageLengthInwardsPhreaticLine3, location.LeakageLengthInwardsPl3);
            Assert.AreEqual(leakageLengthOutwardsPhreaticLine4, location.LeakageLengthOutwardsPl4);
            Assert.AreEqual(leakageLengthInwardsPhreaticLine4, location.LeakageLengthInwardsPl4);
        }
    }
}