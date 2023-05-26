﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.Input;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Test.Calculators.Waternet.Input
{
    [TestFixture]
    public class WaternetCalculatorInputTest
    {
        [Test]
        public void Constructor_ConstructionPropertiesNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new WaternetCalculatorInput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("properties", exception.ParamName);
        }

        [Test]
        public void Constructor_WithConstructionProperties_PropertiesAreSet()
        {
            // Setup
            var random = new Random(11);

            double assessmentLevel = random.NextDouble();
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
            var soilProfile = new TestSoilProfile();
            var drainageConstruction = new DrainageConstruction();
            var phreaticLineOffsets = new PhreaticLineOffsets();

            double waterLevelRiverAverage = random.NextDouble();
            double waterLevelPolder = random.NextDouble();
            double minimumLevelPhreaticLineAtDikeTopRiver = random.NextDouble();
            double minimumLevelPhreaticLineAtDikeTopPolder = random.NextDouble();
            double leakageLengthOutwardsPhreaticLine3 = random.NextDouble();
            double leakageLengthInwardsPhreaticLine3 = random.NextDouble();
            double leakageLengthOutwardsPhreaticLine4 = random.NextDouble();
            double leakageLengthInwardsPhreaticLine4 = random.NextDouble();
            double piezometricHeadPhreaticLine2Outwards = random.NextDouble();
            double piezometricHeadPhreaticLine2Inwards = random.NextDouble();
            double penetrationLength = random.NextDouble();
            bool adjustPhreaticLine3And4ForUplift = random.NextBoolean();
            var dikeSoilScenario = random.NextEnumValue<MacroStabilityInwardsDikeSoilScenario>();
            double waterVolumetricWeight = random.NextDouble();

            // Call
            var input = new WaternetCalculatorInput(
                new WaternetCalculatorInput.ConstructionProperties
                {
                    AssessmentLevel = assessmentLevel,
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
                    WaterVolumetricWeight = waterVolumetricWeight
                });

            // Assert
            Assert.AreEqual(assessmentLevel, input.AssessmentLevel);
            Assert.AreSame(surfaceLine, input.SurfaceLine);
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
            Assert.AreEqual(waterVolumetricWeight, input.WaterVolumetricWeight);
        }

        [Test]
        public void Constructor_EmptyConstructionProperties_ExpectedValues()
        {
            // Setup
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
            var soilProfile = new TestSoilProfile();
            var drainageConstruction = new DrainageConstruction();
            var phreaticLineOffsets = new PhreaticLineOffsets();

            // Call
            var input = new WaternetCalculatorInput(
                new WaternetCalculatorInput.ConstructionProperties
                {
                    SurfaceLine = surfaceLine,
                    SoilProfile = soilProfile,
                    DrainageConstruction = drainageConstruction,
                    PhreaticLineOffsets = phreaticLineOffsets
                });

            // Assert
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
            Assert.IsFalse(input.AdjustPhreaticLine3And4ForUplift);
            Assert.IsNaN(input.WaterVolumetricWeight);

            Assert.AreEqual(MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay, input.DikeSoilScenario);
        }

        [Test]
        public void Constructor_SurfaceLineNull_ThrowsArgumentException()
        {
            // Call
            void Call() =>
                new WaternetCalculatorInput(new WaternetCalculatorInput.ConstructionProperties
                {
                    SoilProfile = new TestSoilProfile(),
                    PhreaticLineOffsets = new PhreaticLineOffsets(),
                    DrainageConstruction = new DrainageConstruction()
                });

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(Call, "SurfaceLine must be set.");
        }

        [Test]
        public void Constructor_SoilProfileNull_ThrowsArgumentException()
        {
            // Call
            void Call() =>
                new WaternetCalculatorInput(new WaternetCalculatorInput.ConstructionProperties
                {
                    SurfaceLine = new MacroStabilityInwardsSurfaceLine("test"),
                    PhreaticLineOffsets = new PhreaticLineOffsets(),
                    DrainageConstruction = new DrainageConstruction()
                });

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(Call, "SoilProfile must be set.");
        }

        [Test]
        public void Constructor_DrainageConstructionNull_ThrowsArgumentException()
        {
            // Call
            void Call() =>
                new WaternetCalculatorInput(new WaternetCalculatorInput.ConstructionProperties
                {
                    SoilProfile = new TestSoilProfile(),
                    SurfaceLine = new MacroStabilityInwardsSurfaceLine("test"),
                    PhreaticLineOffsets = new PhreaticLineOffsets()
                });

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(Call, "DrainageConstruction must be set.");
        }

        [Test]
        public void Constructor_PhreaticLineOffsetsNull_ThrowsArgumentException()
        {
            // Call
            void Call() =>
                new WaternetCalculatorInput(new WaternetCalculatorInput.ConstructionProperties
                {
                    SoilProfile = new TestSoilProfile(),
                    SurfaceLine = new MacroStabilityInwardsSurfaceLine("test"),
                    DrainageConstruction = new DrainageConstruction()
                });

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(Call, "PhreaticLineOffsets must be set.");
        }
    }
}