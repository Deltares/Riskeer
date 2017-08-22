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
using Core.Common.Base;
using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.MacroStabilityInwards.Data.Test
{
    [TestFixture]
    public class MacroStabilityInwardsInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var inputParameters = new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput());

            // Assert
            Assert.IsInstanceOf<Observable>(inputParameters);
            Assert.IsInstanceOf<ICalculationInput>(inputParameters);

            Assert.IsNull(inputParameters.SurfaceLine);
            Assert.IsNull(inputParameters.StochasticSoilModel);
            Assert.IsNull(inputParameters.StochasticSoilProfile);
            Assert.IsNull(inputParameters.HydraulicBoundaryLocation);

            Assert.IsInstanceOf<RoundedDouble>(inputParameters.AssessmentLevel);
            Assert.AreEqual(2, inputParameters.AssessmentLevel.NumberOfDecimalPlaces);
            Assert.IsNaN(inputParameters.AssessmentLevel);

            Assert.IsFalse(inputParameters.UseAssessmentLevelManualInput);

            Assert.AreEqual(10, inputParameters.SlipPlaneMinimumDepth, inputParameters.SlipPlaneMinimumDepth.GetAccuracy());
            Assert.AreEqual(2, inputParameters.SlipPlaneMinimumDepth.NumberOfDecimalPlaces);

            Assert.AreEqual(30, inputParameters.SlipPlaneMinimumLength, inputParameters.SlipPlaneMinimumLength.GetAccuracy());
            Assert.AreEqual(2, inputParameters.SlipPlaneMinimumLength.NumberOfDecimalPlaces);

            Assert.AreEqual(5, inputParameters.MaximumSliceWidth, inputParameters.MaximumSliceWidth.GetAccuracy());
            Assert.AreEqual(2, inputParameters.MaximumSliceWidth.NumberOfDecimalPlaces);

            Assert.IsTrue(inputParameters.MoveGrid);
            Assert.AreEqual(MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay, inputParameters.DikeSoilScenario);

            Assert.AreEqual(0, inputParameters.WaterLevelRiverAverage, inputParameters.WaterLevelRiverAverage.GetAccuracy());
            Assert.AreEqual(2, inputParameters.WaterLevelRiverAverage.NumberOfDecimalPlaces);

            Assert.AreEqual(0, inputParameters.WaterLevelPolder.Value);
            Assert.AreEqual(2, inputParameters.WaterLevelPolder.NumberOfDecimalPlaces);

            Assert.IsFalse(inputParameters.DrainageConstructionPresent);

            Assert.IsNaN(inputParameters.XCoordinateDrainageConstruction);
            Assert.AreEqual(2, inputParameters.XCoordinateDrainageConstruction.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.ZCoordinateDrainageConstruction);
            Assert.AreEqual(2, inputParameters.ZCoordinateDrainageConstruction.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.MinimumLevelPhreaticLineAtDikeTopRiver);
            Assert.AreEqual(2, inputParameters.MinimumLevelPhreaticLineAtDikeTopRiver.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.MinimumLevelPhreaticLineAtDikeTopPolder);
            Assert.AreEqual(2, inputParameters.MinimumLevelPhreaticLineAtDikeTopPolder.NumberOfDecimalPlaces);

            Assert.IsTrue(inputParameters.UseDefaultOffset);

            Assert.IsNaN(inputParameters.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.AreEqual(2, inputParameters.PhreaticLineOffsetBelowDikeTopAtRiver.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.AreEqual(2, inputParameters.PhreaticLineOffsetBelowDikeTopAtPolder.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.AreEqual(2, inputParameters.PhreaticLineOffsetBelowShoulderBaseInside.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.PhreaticLineOffsetBelowDikeToeAtPolder);
            Assert.AreEqual(2, inputParameters.PhreaticLineOffsetBelowDikeToeAtPolder.NumberOfDecimalPlaces);

            Assert.IsTrue(inputParameters.AdjustPhreaticLine3And4ForUplift);

            Assert.AreEqual(0, inputParameters.LeakageLengthOutwardsPhreaticLine3,
                            inputParameters.LeakageLengthOutwardsPhreaticLine3.GetAccuracy());
            Assert.AreEqual(2, inputParameters.LeakageLengthOutwardsPhreaticLine3.NumberOfDecimalPlaces);

            Assert.AreEqual(0, inputParameters.LeakageLengthInwardsPhreaticLine3,
                            inputParameters.LeakageLengthInwardsPhreaticLine3.GetAccuracy());
            Assert.AreEqual(2, inputParameters.LeakageLengthInwardsPhreaticLine3.NumberOfDecimalPlaces);

            Assert.AreEqual(0, inputParameters.LeakageLengthOutwardsPhreaticLine4,
                            inputParameters.LeakageLengthOutwardsPhreaticLine4.GetAccuracy());
            Assert.AreEqual(2, inputParameters.LeakageLengthOutwardsPhreaticLine4.NumberOfDecimalPlaces);

            Assert.AreEqual(0, inputParameters.LeakageLengthInwardsPhreaticLine4,
                            inputParameters.LeakageLengthInwardsPhreaticLine4.GetAccuracy());
            Assert.AreEqual(2, inputParameters.LeakageLengthInwardsPhreaticLine4.NumberOfDecimalPlaces);

            Assert.AreEqual(0, inputParameters.PiezometricHeadPhreaticLine2Outwards,
                            inputParameters.PiezometricHeadPhreaticLine2Outwards.GetAccuracy());
            Assert.AreEqual(2, inputParameters.PiezometricHeadPhreaticLine2Outwards.NumberOfDecimalPlaces);

            Assert.AreEqual(0, inputParameters.PiezometricHeadPhreaticLine2Inwards,
                            inputParameters.PiezometricHeadPhreaticLine2Inwards.GetAccuracy());
            Assert.AreEqual(2, inputParameters.PiezometricHeadPhreaticLine2Inwards.NumberOfDecimalPlaces);

            Assert.AreEqual(0, inputParameters.PenetrationLength,
                            inputParameters.PenetrationLength.GetAccuracy());
            Assert.AreEqual(2, inputParameters.PenetrationLength.NumberOfDecimalPlaces);

            Assert.AreEqual(MacroStabilityInwardsGridDetermination.Automatic, inputParameters.GridDetermination);
            Assert.AreEqual(MacroStabilityInwardsTangentLineDetermination.LayerSeparated, inputParameters.TangentLineDetermination);

            Assert.AreEqual(0, inputParameters.TangentLineZTop,
                            inputParameters.TangentLineZTop.GetAccuracy());
            Assert.AreEqual(2, inputParameters.TangentLineZTop.NumberOfDecimalPlaces);

            Assert.AreEqual(0, inputParameters.TangentLineZBottom,
                            inputParameters.TangentLineZBottom.GetAccuracy());
            Assert.AreEqual(2, inputParameters.TangentLineZBottom.NumberOfDecimalPlaces);

            Assert.IsNotNull(inputParameters.LeftGrid);
            Assert.IsNotNull(inputParameters.RightGrid);
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
            double waterLevelPolder = random.Next();
            double xCoordinateDrainageConstruction = random.Next();
            double zCoordinateDrainageConstruction = random.Next();
            double minimumLevelPhreaticLineAtDikeTopRiver = random.Next();
            double minimumLevelPhreaticLineAtDikeTopPolder = random.Next();
            double phreaticLineOffsetBelowDikeTopAtRiver = random.Next();
            double phreaticLineOffsetBelowDikeTopAtPolder = random.Next();
            double phreaticLineOffsetBelowShoulderBaseInside = random.Next();
            double phreaticLineOffsetBelowDikeToeAtPolder = random.Next();
            double leakageLengthOutwardsPhreaticLine3 = random.Next();
            double leakageLengthInwardsPhreaticLine3 = random.Next();
            double leakageLengthOutwardsPhreaticLine4 = random.Next();
            double leakageLengthInwardsPhreaticLine4 = random.Next();
            double piezometricHeadPhreaticLine2Outwards = random.Next();
            double piezometricHeadPhreaticLine2Inwards = random.Next();
            double penetrationLength = random.Next();
            double tangentLineZTop = random.Next();
            double tangentLineZBottom = random.Next();

            // Call
            var input = new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput())
            {
                SlipPlaneMinimumDepth = (RoundedDouble) slipPlaneMinimumDepth,
                SlipPlaneMinimumLength = (RoundedDouble) slipPlaneMinimumLength,
                MaximumSliceWidth = (RoundedDouble) maximumSliceWidth,
                WaterLevelRiverAverage = (RoundedDouble) waterLevelRiverAverage,
                WaterLevelPolder = (RoundedDouble) waterLevelPolder,
                XCoordinateDrainageConstruction = (RoundedDouble) xCoordinateDrainageConstruction,
                ZCoordinateDrainageConstruction = (RoundedDouble) zCoordinateDrainageConstruction,
                MinimumLevelPhreaticLineAtDikeTopRiver = (RoundedDouble) minimumLevelPhreaticLineAtDikeTopRiver,
                MinimumLevelPhreaticLineAtDikeTopPolder = (RoundedDouble) minimumLevelPhreaticLineAtDikeTopPolder,
                PhreaticLineOffsetBelowDikeTopAtRiver = (RoundedDouble) phreaticLineOffsetBelowDikeTopAtRiver,
                PhreaticLineOffsetBelowDikeTopAtPolder = (RoundedDouble) phreaticLineOffsetBelowDikeTopAtPolder,
                PhreaticLineOffsetBelowShoulderBaseInside = (RoundedDouble) phreaticLineOffsetBelowShoulderBaseInside,
                PhreaticLineOffsetBelowDikeToeAtPolder = (RoundedDouble) phreaticLineOffsetBelowDikeToeAtPolder,
                LeakageLengthOutwardsPhreaticLine3 = (RoundedDouble) leakageLengthOutwardsPhreaticLine3,
                LeakageLengthInwardsPhreaticLine3 = (RoundedDouble) leakageLengthInwardsPhreaticLine3,
                LeakageLengthOutwardsPhreaticLine4 = (RoundedDouble) leakageLengthOutwardsPhreaticLine4,
                LeakageLengthInwardsPhreaticLine4 = (RoundedDouble) leakageLengthInwardsPhreaticLine4,
                PiezometricHeadPhreaticLine2Outwards = (RoundedDouble) piezometricHeadPhreaticLine2Outwards,
                PiezometricHeadPhreaticLine2Inwards = (RoundedDouble) piezometricHeadPhreaticLine2Inwards,
                PenetrationLength = (RoundedDouble) penetrationLength,
                TangentLineZTop = (RoundedDouble) tangentLineZTop,
                TangentLineZBottom = (RoundedDouble) tangentLineZBottom
            };

            // Assert
            Assert.AreEqual(new RoundedDouble(2, slipPlaneMinimumDepth), input.SlipPlaneMinimumDepth);
            Assert.AreEqual(new RoundedDouble(2, slipPlaneMinimumLength), input.SlipPlaneMinimumLength);
            Assert.AreEqual(new RoundedDouble(2, maximumSliceWidth), input.MaximumSliceWidth);
            Assert.AreEqual(new RoundedDouble(2, waterLevelRiverAverage), input.WaterLevelRiverAverage);
            Assert.AreEqual(new RoundedDouble(2, waterLevelPolder), input.WaterLevelPolder);
            Assert.AreEqual(new RoundedDouble(2, xCoordinateDrainageConstruction), input.XCoordinateDrainageConstruction);
            Assert.AreEqual(new RoundedDouble(2, zCoordinateDrainageConstruction), input.ZCoordinateDrainageConstruction);
            Assert.AreEqual(new RoundedDouble(2, minimumLevelPhreaticLineAtDikeTopRiver), input.MinimumLevelPhreaticLineAtDikeTopRiver);
            Assert.AreEqual(new RoundedDouble(2, minimumLevelPhreaticLineAtDikeTopPolder), input.MinimumLevelPhreaticLineAtDikeTopPolder);
            Assert.AreEqual(new RoundedDouble(2, phreaticLineOffsetBelowDikeTopAtRiver), input.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.AreEqual(new RoundedDouble(2, phreaticLineOffsetBelowDikeTopAtPolder), input.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.AreEqual(new RoundedDouble(2, phreaticLineOffsetBelowShoulderBaseInside), input.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.AreEqual(new RoundedDouble(2, phreaticLineOffsetBelowDikeToeAtPolder), input.PhreaticLineOffsetBelowDikeToeAtPolder);
            Assert.AreEqual(new RoundedDouble(2, leakageLengthOutwardsPhreaticLine3), input.LeakageLengthOutwardsPhreaticLine3);
            Assert.AreEqual(new RoundedDouble(2, leakageLengthInwardsPhreaticLine3), input.LeakageLengthInwardsPhreaticLine3);
            Assert.AreEqual(new RoundedDouble(2, leakageLengthOutwardsPhreaticLine4), input.LeakageLengthOutwardsPhreaticLine4);
            Assert.AreEqual(new RoundedDouble(2, leakageLengthInwardsPhreaticLine4), input.LeakageLengthInwardsPhreaticLine4);
            Assert.AreEqual(new RoundedDouble(2, piezometricHeadPhreaticLine2Outwards), input.PiezometricHeadPhreaticLine2Outwards);
            Assert.AreEqual(new RoundedDouble(2, piezometricHeadPhreaticLine2Inwards), input.PiezometricHeadPhreaticLine2Inwards);
            Assert.AreEqual(new RoundedDouble(2, penetrationLength), input.PenetrationLength);
            Assert.AreEqual(new RoundedDouble(2, tangentLineZTop), input.TangentLineZTop);
            Assert.AreEqual(new RoundedDouble(2, tangentLineZBottom), input.TangentLineZBottom);
        }

        [Test]
        public void Constructor_GeneralInputIsNull_ArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MacroStabilityInwardsInput(null);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void AssessmentLevel_UseAssessmentLevelManualInputIsFalse_ReturnsNaN()
        {
            // Setup
            var input = new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput())
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

            var input = new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput())
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
            var input = new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput())
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
            var input = new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput())
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
            var input = new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput())
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
            var input = new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput())
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
    }
}