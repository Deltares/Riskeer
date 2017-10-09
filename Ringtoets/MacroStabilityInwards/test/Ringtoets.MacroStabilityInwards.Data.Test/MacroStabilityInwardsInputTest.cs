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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Data.TestUtil.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Data.Test
{
    [TestFixture]
    public class MacroStabilityInwardsInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var inputParameters = new MacroStabilityInwardsInput();

            // Assert
            Assert.IsInstanceOf<Observable>(inputParameters);
            Assert.IsInstanceOf<ICalculationInput>(inputParameters);

            Assert.IsNull(inputParameters.SurfaceLine);
            Assert.IsNull(inputParameters.StochasticSoilModel);
            Assert.IsNull(inputParameters.StochasticSoilProfile);
            Assert.IsNull(inputParameters.HydraulicBoundaryLocation);
            Assert.IsNull(inputParameters.SoilProfileUnderSurfaceLine);

            Assert.IsInstanceOf<RoundedDouble>(inputParameters.AssessmentLevel);
            Assert.IsNaN(inputParameters.AssessmentLevel);

            Assert.IsFalse(inputParameters.UseAssessmentLevelManualInput);

            Assert.AreEqual(0, inputParameters.SlipPlaneMinimumDepth, inputParameters.SlipPlaneMinimumDepth.GetAccuracy());
            Assert.AreEqual(2, inputParameters.SlipPlaneMinimumDepth.NumberOfDecimalPlaces);

            Assert.AreEqual(0, inputParameters.SlipPlaneMinimumLength, inputParameters.SlipPlaneMinimumLength.GetAccuracy());
            Assert.AreEqual(2, inputParameters.SlipPlaneMinimumLength.NumberOfDecimalPlaces);

            Assert.AreEqual(1, inputParameters.MaximumSliceWidth, inputParameters.MaximumSliceWidth.GetAccuracy());
            Assert.AreEqual(2, inputParameters.MaximumSliceWidth.NumberOfDecimalPlaces);

            Assert.IsTrue(inputParameters.MoveGrid);
            Assert.AreEqual(MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay, inputParameters.DikeSoilScenario);

            Assert.IsNaN(inputParameters.WaterLevelRiverAverage);
            Assert.AreEqual(2, inputParameters.WaterLevelRiverAverage.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.LocationInputExtreme.WaterLevelPolder);
            Assert.AreEqual(2, inputParameters.LocationInputExtreme.WaterLevelPolder.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.LocationInputExtreme.PenetrationLength);
            Assert.AreEqual(2, inputParameters.LocationInputExtreme.PenetrationLength.NumberOfDecimalPlaces);

            Assert.IsTrue(inputParameters.LocationInputExtreme.UseDefaultOffsets);

            Assert.IsNaN(inputParameters.LocationInputExtreme.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.AreEqual(2, inputParameters.LocationInputExtreme.PhreaticLineOffsetBelowDikeTopAtRiver.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.LocationInputExtreme.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.AreEqual(2, inputParameters.LocationInputExtreme.PhreaticLineOffsetBelowDikeTopAtPolder.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.LocationInputExtreme.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.AreEqual(2, inputParameters.LocationInputExtreme.PhreaticLineOffsetBelowShoulderBaseInside.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.LocationInputExtreme.PhreaticLineOffsetBelowDikeToeAtPolder);
            Assert.AreEqual(2, inputParameters.LocationInputExtreme.PhreaticLineOffsetBelowDikeToeAtPolder.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.LocationInputDaily.WaterLevelPolder);
            Assert.AreEqual(2, inputParameters.LocationInputDaily.WaterLevelPolder.NumberOfDecimalPlaces);

            Assert.AreEqual(0.0, inputParameters.LocationInputDaily.PenetrationLength);
            Assert.AreEqual(2, inputParameters.LocationInputDaily.PenetrationLength.NumberOfDecimalPlaces);

            Assert.IsTrue(inputParameters.LocationInputDaily.UseDefaultOffsets);

            Assert.IsNaN(inputParameters.LocationInputDaily.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.AreEqual(2, inputParameters.LocationInputDaily.PhreaticLineOffsetBelowDikeTopAtRiver.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.LocationInputDaily.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.AreEqual(2, inputParameters.LocationInputDaily.PhreaticLineOffsetBelowDikeTopAtPolder.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.LocationInputDaily.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.AreEqual(2, inputParameters.LocationInputDaily.PhreaticLineOffsetBelowShoulderBaseInside.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.LocationInputDaily.PhreaticLineOffsetBelowDikeToeAtPolder);
            Assert.AreEqual(2, inputParameters.LocationInputDaily.PhreaticLineOffsetBelowDikeToeAtPolder.NumberOfDecimalPlaces);

            Assert.IsFalse(inputParameters.DrainageConstructionPresent);

            Assert.IsNaN(inputParameters.XCoordinateDrainageConstruction);
            Assert.AreEqual(2, inputParameters.XCoordinateDrainageConstruction.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.ZCoordinateDrainageConstruction);
            Assert.AreEqual(2, inputParameters.ZCoordinateDrainageConstruction.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.MinimumLevelPhreaticLineAtDikeTopRiver);
            Assert.AreEqual(2, inputParameters.MinimumLevelPhreaticLineAtDikeTopRiver.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.MinimumLevelPhreaticLineAtDikeTopPolder);
            Assert.AreEqual(2, inputParameters.MinimumLevelPhreaticLineAtDikeTopPolder.NumberOfDecimalPlaces);

            Assert.IsTrue(inputParameters.AdjustPhreaticLine3And4ForUplift);

            Assert.IsNaN(inputParameters.LeakageLengthOutwardsPhreaticLine3);
            Assert.AreEqual(2, inputParameters.LeakageLengthOutwardsPhreaticLine3.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.LeakageLengthInwardsPhreaticLine3);
            Assert.AreEqual(2, inputParameters.LeakageLengthInwardsPhreaticLine3.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.LeakageLengthOutwardsPhreaticLine4);
            Assert.AreEqual(2, inputParameters.LeakageLengthOutwardsPhreaticLine4.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.LeakageLengthInwardsPhreaticLine4);
            Assert.AreEqual(2, inputParameters.LeakageLengthInwardsPhreaticLine4.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.PiezometricHeadPhreaticLine2Outwards);
            Assert.AreEqual(2, inputParameters.PiezometricHeadPhreaticLine2Outwards.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.PiezometricHeadPhreaticLine2Inwards);
            Assert.AreEqual(2, inputParameters.PiezometricHeadPhreaticLine2Inwards.NumberOfDecimalPlaces);

            Assert.AreEqual(MacroStabilityInwardsGridDeterminationType.Automatic, inputParameters.GridDeterminationType);
            Assert.AreEqual(MacroStabilityInwardsTangentLineDeterminationType.LayerSeparated, inputParameters.TangentLineDeterminationType);

            Assert.IsNaN(inputParameters.TangentLineZTop);
            Assert.AreEqual(2, inputParameters.TangentLineZTop.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.TangentLineZBottom);
            Assert.AreEqual(2, inputParameters.TangentLineZBottom.NumberOfDecimalPlaces);

            Assert.IsNotNull(inputParameters.LeftGrid);
            Assert.IsNotNull(inputParameters.RightGrid);

            Assert.IsTrue(inputParameters.CreateZones);
            Assert.AreEqual(MacroStabilityInwardsZoningBoundariesDeterminationType.Automatic, inputParameters.ZoningBoundariesDeterminationType);
        }

        [Test]
        public void Constructor_SetProperties_ExpectedValues()
        {
            // Setup
            var random = new Random();
            double slipPlaneMinimumDepth = random.Next();
            double slipPlaneMinimumLength = random.Next();
            double maximumSliceWidth = random.Next();
            double waterLevelRiverAverage = random.Next();
            double xCoordinateDrainageConstruction = random.Next();
            double zCoordinateDrainageConstruction = random.Next();
            double minimumLevelPhreaticLineAtDikeTopRiver = random.Next();
            double minimumLevelPhreaticLineAtDikeTopPolder = random.Next();
            double leakageLengthOutwardsPhreaticLine3 = random.Next();
            double leakageLengthInwardsPhreaticLine3 = random.Next();
            double leakageLengthOutwardsPhreaticLine4 = random.Next();
            double leakageLengthInwardsPhreaticLine4 = random.Next();
            double piezometricHeadPhreaticLine2Outwards = random.Next();
            double piezometricHeadPhreaticLine2Inwards = random.Next();
            double tangentLineZTop = random.Next();
            double tangentLineZBottom = random.Next();

            // Call
            var input = new MacroStabilityInwardsInput
            {
                SlipPlaneMinimumDepth = (RoundedDouble) slipPlaneMinimumDepth,
                SlipPlaneMinimumLength = (RoundedDouble) slipPlaneMinimumLength,
                MaximumSliceWidth = (RoundedDouble) maximumSliceWidth,
                WaterLevelRiverAverage = (RoundedDouble) waterLevelRiverAverage,
                XCoordinateDrainageConstruction = (RoundedDouble) xCoordinateDrainageConstruction,
                ZCoordinateDrainageConstruction = (RoundedDouble) zCoordinateDrainageConstruction,
                MinimumLevelPhreaticLineAtDikeTopRiver = (RoundedDouble) minimumLevelPhreaticLineAtDikeTopRiver,
                MinimumLevelPhreaticLineAtDikeTopPolder = (RoundedDouble) minimumLevelPhreaticLineAtDikeTopPolder,
                LeakageLengthOutwardsPhreaticLine3 = (RoundedDouble) leakageLengthOutwardsPhreaticLine3,
                LeakageLengthInwardsPhreaticLine3 = (RoundedDouble) leakageLengthInwardsPhreaticLine3,
                LeakageLengthOutwardsPhreaticLine4 = (RoundedDouble) leakageLengthOutwardsPhreaticLine4,
                LeakageLengthInwardsPhreaticLine4 = (RoundedDouble) leakageLengthInwardsPhreaticLine4,
                PiezometricHeadPhreaticLine2Outwards = (RoundedDouble) piezometricHeadPhreaticLine2Outwards,
                PiezometricHeadPhreaticLine2Inwards = (RoundedDouble) piezometricHeadPhreaticLine2Inwards,
                TangentLineZTop = (RoundedDouble) tangentLineZTop,
                TangentLineZBottom = (RoundedDouble) tangentLineZBottom
            };

            // Assert
            Assert.AreEqual(new RoundedDouble(2, slipPlaneMinimumDepth), input.SlipPlaneMinimumDepth);
            Assert.AreEqual(new RoundedDouble(2, slipPlaneMinimumLength), input.SlipPlaneMinimumLength);
            Assert.AreEqual(new RoundedDouble(2, maximumSliceWidth), input.MaximumSliceWidth);
            Assert.AreEqual(new RoundedDouble(2, waterLevelRiverAverage), input.WaterLevelRiverAverage);
            Assert.AreEqual(new RoundedDouble(2, xCoordinateDrainageConstruction), input.XCoordinateDrainageConstruction);
            Assert.AreEqual(new RoundedDouble(2, zCoordinateDrainageConstruction), input.ZCoordinateDrainageConstruction);
            Assert.AreEqual(new RoundedDouble(2, minimumLevelPhreaticLineAtDikeTopRiver), input.MinimumLevelPhreaticLineAtDikeTopRiver);
            Assert.AreEqual(new RoundedDouble(2, minimumLevelPhreaticLineAtDikeTopPolder), input.MinimumLevelPhreaticLineAtDikeTopPolder);
            Assert.AreEqual(new RoundedDouble(2, leakageLengthOutwardsPhreaticLine3), input.LeakageLengthOutwardsPhreaticLine3);
            Assert.AreEqual(new RoundedDouble(2, leakageLengthInwardsPhreaticLine3), input.LeakageLengthInwardsPhreaticLine3);
            Assert.AreEqual(new RoundedDouble(2, leakageLengthOutwardsPhreaticLine4), input.LeakageLengthOutwardsPhreaticLine4);
            Assert.AreEqual(new RoundedDouble(2, leakageLengthInwardsPhreaticLine4), input.LeakageLengthInwardsPhreaticLine4);
            Assert.AreEqual(new RoundedDouble(2, piezometricHeadPhreaticLine2Outwards), input.PiezometricHeadPhreaticLine2Outwards);
            Assert.AreEqual(new RoundedDouble(2, piezometricHeadPhreaticLine2Inwards), input.PiezometricHeadPhreaticLine2Inwards);
            Assert.AreEqual(new RoundedDouble(2, tangentLineZTop), input.TangentLineZTop);
            Assert.AreEqual(new RoundedDouble(2, tangentLineZBottom), input.TangentLineZBottom);
        }

        [Test]
        public void AssessmentLevel_UseAssessmentLevelManualInputIsFalse_ReturnsNaN()
        {
            // Setup
            var input = new MacroStabilityInwardsInput
            {
                UseAssessmentLevelManualInput = false,
                HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
            };

            // Call
            RoundedDouble calculatedAssessmentLevel = input.AssessmentLevel;

            // Assert
            Assert.IsNaN(calculatedAssessmentLevel);
        }

        [Test]
        public void AssessmentLevel_UseAssessmentLevelManualInputIsFalseWithHydraulicLocationSetAndDesignWaterLevelOutputSet_ReturnCalculatedAssessmentLevel()
        {
            // Setup
            double calculatedAssessmentLevel = new Random(21).NextDouble();
            HydraulicBoundaryLocation testHydraulicBoundaryLocation = new TestHydraulicBoundaryLocation
            {
                DesignWaterLevelCalculation =
                {
                    Output = new TestHydraulicBoundaryLocationOutput(calculatedAssessmentLevel)
                }
            };

            var input = new MacroStabilityInwardsInput
            {
                HydraulicBoundaryLocation = testHydraulicBoundaryLocation
            };

            // Call
            RoundedDouble newAssessmentLevel = input.AssessmentLevel;

            // Assert
            Assert.AreEqual(calculatedAssessmentLevel, newAssessmentLevel, input.AssessmentLevel.GetAccuracy());
        }

        [Test]
        public void AssessmentLevel_UseAssessmentLevelManualInputFalseAndSettingValue_ThrowsInvalidOperationException()
        {
            // Setup
            var input = new MacroStabilityInwardsInput
            {
                UseAssessmentLevelManualInput = false
            };

            var testLevel = (RoundedDouble) new Random(21).NextDouble();

            // Call 
            TestDelegate call = () => input.AssessmentLevel = testLevel;

            // Assert
            string message = Assert.Throws<InvalidOperationException>(call).Message;
            Assert.AreEqual("UseAssessmentLevelManualInput is false", message);
        }

        [Test]
        public void AssessmentLevel_UseAssessmentLevelManualInputTrueAndSettingValue_ReturnSetValue()
        {
            // Setup
            var input = new MacroStabilityInwardsInput
            {
                UseAssessmentLevelManualInput = true
            };

            var testLevel = (RoundedDouble) new Random(21).NextDouble();

            // Call
            input.AssessmentLevel = testLevel;

            // Assert
            Assert.AreEqual(2, input.AssessmentLevel.NumberOfDecimalPlaces);
            Assert.AreEqual(testLevel, input.AssessmentLevel, input.AssessmentLevel.GetAccuracy());
        }

        [Test]
        public void GivenAssessmentLevelSetByHydraulicBoundaryLocation_WhenManualAssessmentLevelTrueAndNewLevelSet_ThenLevelUpdatedAndLocationRemoved()
        {
            // Given
            var random = new Random(21);
            var testLevel = (RoundedDouble) random.NextDouble();
            var input = new MacroStabilityInwardsInput
            {
                HydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(testLevel)
            };

            var newLevel = (RoundedDouble) random.NextDouble();

            // When
            input.UseAssessmentLevelManualInput = true;
            input.AssessmentLevel = newLevel;

            // Then
            Assert.AreEqual(2, input.AssessmentLevel.NumberOfDecimalPlaces);
            Assert.AreEqual(newLevel, input.AssessmentLevel, input.AssessmentLevel.GetAccuracy());
            Assert.IsNull(input.HydraulicBoundaryLocation);
        }

        [Test]
        public void GivenAssessmentLevelSetByManualInput_WhenManualAssessmentLevelFalseAndHydraulicBoundaryLocationSet_ThenAssessmentLevelUpdatedAndLocationSet()
        {
            // Given
            var random = new Random(21);
            var testLevel = (RoundedDouble) random.NextDouble();
            var input = new MacroStabilityInwardsInput
            {
                UseAssessmentLevelManualInput = true,
                AssessmentLevel = testLevel
            };

            var newLevel = (RoundedDouble) random.NextDouble();
            TestHydraulicBoundaryLocation hydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(newLevel);

            // When
            input.UseAssessmentLevelManualInput = false;
            input.HydraulicBoundaryLocation = hydraulicBoundaryLocation;

            // Then
            Assert.AreEqual(2, input.AssessmentLevel.NumberOfDecimalPlaces);
            Assert.AreSame(hydraulicBoundaryLocation, input.HydraulicBoundaryLocation);
            Assert.AreEqual(newLevel, input.AssessmentLevel, input.AssessmentLevel.GetAccuracy());
        }

        [Test]
        public void GivenInput_WhenSurfaceLineSetAndStochasticSoilProfileNull_ThenSoilProfileUnderSurfaceLineNull()
        {
            // Given
            var inputParameters = new MacroStabilityInwardsInput();

            // When
            inputParameters.SurfaceLine = new MacroStabilityInwardsSurfaceLine("test");

            // Then
            Assert.IsNull(inputParameters.SoilProfileUnderSurfaceLine);
        }

        [Test]
        public void GivenInput_WhenStochasticSoilProfileSetAndSurfaceLineNull_ThenSoilProfileUnderSurfaceLineNull()
        {
            // Given
            var inputParameters = new MacroStabilityInwardsInput();

            // When
            MacroStabilityInwardsSoilProfile1D soilProfile = MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D();
            inputParameters.StochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0, soilProfile);

            // Then
            Assert.IsNull(inputParameters.SoilProfileUnderSurfaceLine);
        }

        [Test]
        public void GivenInput_WhenSurfaceLineAndStochasticSoilProfileSet_ThenSoilProfileUnderSurfaceLineSet()
        {
            // Given
            var inputParameters = new MacroStabilityInwardsInput();

            // When
            var surfaceLine = new MacroStabilityInwardsSurfaceLine("test");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(1, 1, 1)
            });
            inputParameters.SurfaceLine = surfaceLine;
            MacroStabilityInwardsSoilProfile1D soilProfile = MacroStabilityInwardsSoilProfile1DTestFactory.CreateMacroStabilityInwardsSoilProfile1D();
            inputParameters.StochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0, soilProfile);

            // Then
            Assert.IsNotNull(inputParameters.SoilProfileUnderSurfaceLine);
        }
    }
}