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
using System.Collections.Generic;
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

            Assert.AreEqual(10, inputParameters.SlipPlaneMinimumDepth.Value);
            Assert.AreEqual(2, inputParameters.SlipPlaneMinimumDepth.NumberOfDecimalPlaces);

            Assert.AreEqual(30, inputParameters.SlipPlaneMinimumLength.Value);
            Assert.AreEqual(2, inputParameters.SlipPlaneMinimumLength.NumberOfDecimalPlaces);

            Assert.AreEqual(5, inputParameters.MaximumSliceWidth.Value);
            Assert.AreEqual(2, inputParameters.MaximumSliceWidth.NumberOfDecimalPlaces);

            Assert.IsTrue(inputParameters.MoveGrid);
            Assert.AreEqual(MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay, inputParameters.DikeSoilScenario);

            Assert.AreEqual(0, inputParameters.WaterLevelRiverAverage.Value);
            Assert.AreEqual(2, inputParameters.WaterLevelRiverAverage.NumberOfDecimalPlaces);

            Assert.AreEqual(0, inputParameters.WaterLevelPolder.Value);
            Assert.AreEqual(2, inputParameters.WaterLevelPolder.NumberOfDecimalPlaces);

            Assert.IsFalse(inputParameters.DrainageConstructionPresent);

            Assert.IsNaN(inputParameters.XCoordinateDrainageConstruction.Value);
            Assert.AreEqual(2, inputParameters.XCoordinateDrainageConstruction.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.ZCoordinateDrainageConstruction.Value);
            Assert.AreEqual(2, inputParameters.ZCoordinateDrainageConstruction.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.MinimumLevelPhreaticLineAtDikeTopRiver.Value);
            Assert.AreEqual(2, inputParameters.MinimumLevelPhreaticLineAtDikeTopRiver.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.MinimumLevelPhreaticLineAtDikeTopPolder.Value);
            Assert.AreEqual(2, inputParameters.MinimumLevelPhreaticLineAtDikeTopPolder.NumberOfDecimalPlaces);

            Assert.IsTrue(inputParameters.UseDefaultOffset);

            Assert.IsNaN(inputParameters.PhreaticLineOffsetBelowDikeTopAtRiver.Value);
            Assert.AreEqual(2, inputParameters.PhreaticLineOffsetBelowDikeTopAtRiver.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.PhreaticLineOffsetBelowDikeTopAtPolder.Value);
            Assert.AreEqual(2, inputParameters.PhreaticLineOffsetBelowDikeTopAtPolder.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.PhreaticLineOffsetBelowShoulderBaseInside.Value);
            Assert.AreEqual(2, inputParameters.PhreaticLineOffsetBelowShoulderBaseInside.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.PhreaticLineOffsetBelowDikeToeAtPolder.Value);
            Assert.AreEqual(2, inputParameters.PhreaticLineOffsetBelowDikeToeAtPolder.NumberOfDecimalPlaces);

            Assert.IsFalse(inputParameters.AdjustPhreaticLine3And4ForUplift);

            Assert.AreEqual(0, inputParameters.LeakageLengthOutwardsPhreaticLine3.Value);
            Assert.AreEqual(2, inputParameters.LeakageLengthOutwardsPhreaticLine3.NumberOfDecimalPlaces);

            Assert.AreEqual(0, inputParameters.LeakageLengthInwardsPhreaticLine3.Value);
            Assert.AreEqual(2, inputParameters.LeakageLengthInwardsPhreaticLine3.NumberOfDecimalPlaces);

            Assert.AreEqual(0, inputParameters.LeakageLengthOutwardsPhreaticLine4.Value);
            Assert.AreEqual(2, inputParameters.LeakageLengthOutwardsPhreaticLine4.NumberOfDecimalPlaces);

            Assert.AreEqual(0, inputParameters.LeakageLengthInwardsPhreaticLine4.Value);
            Assert.AreEqual(2, inputParameters.LeakageLengthInwardsPhreaticLine4.NumberOfDecimalPlaces);

            Assert.AreEqual(0, inputParameters.PiezometricHeadPhreaticLine2Outwards.Value);
            Assert.AreEqual(2, inputParameters.PiezometricHeadPhreaticLine2Outwards.NumberOfDecimalPlaces);

            Assert.AreEqual(0, inputParameters.PiezometricHeadPhreaticLine2Inwards.Value);
            Assert.AreEqual(2, inputParameters.PiezometricHeadPhreaticLine2Inwards.NumberOfDecimalPlaces);

            Assert.AreEqual(0, inputParameters.PenetrationLength.Value);
            Assert.AreEqual(2, inputParameters.PenetrationLength.NumberOfDecimalPlaces);
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

        [Test]
        [TestCaseSource(nameof(GetUnroundedValues), new object[]
        {
            "SlipPlaneMinimumDepth"
        })]
        public void SlipPlaneMinimumDepth_SetToNew_ValueIsRounded(double newValue)
        {
            TestValueIsRounded(input => input.SlipPlaneMinimumDepth = (RoundedDouble) newValue, newValue, input => input.SlipPlaneMinimumDepth);
        }

        [Test]
        [TestCaseSource(nameof(GetUnroundedValues), new object[]
        {
            "SlipPlaneMinimumLength"
        })]
        public void SlipPlaneMinimumLength_SetToNew_ValueIsRounded(double newValue)
        {
            TestValueIsRounded(input => input.SlipPlaneMinimumLength = (RoundedDouble) newValue, newValue, input => input.SlipPlaneMinimumLength);
        }

        [Test]
        [TestCaseSource(nameof(GetUnroundedValues), new object[]
        {
            "MaximumSliceWidth"
        })]
        public void MaximumSliceWidth_SetToNew_ValueIsRounded(double newValue)
        {
            TestValueIsRounded(input => input.MaximumSliceWidth = (RoundedDouble) newValue, newValue, input => input.MaximumSliceWidth);
        }

        [Test]
        [TestCaseSource(nameof(GetUnroundedValues), new object[]
        {
            "WaterLevelRiverAverage"
        })]
        public void WaterLevelRiverAverage_SetToNew_ValueIsRounded(double newValue)
        {
            TestValueIsRounded(input => input.WaterLevelRiverAverage = (RoundedDouble) newValue, newValue, input => input.WaterLevelRiverAverage);
        }

        [Test]
        [TestCaseSource(nameof(GetUnroundedValues), new object[]
        {
            "WaterLevelPolder"
        })]
        public void WaterLevelPolder_SetToNew_ValueIsRounded(double newValue)
        {
            TestValueIsRounded(input => input.WaterLevelPolder = (RoundedDouble) newValue, newValue, input => input.WaterLevelPolder);
        }

        [Test]
        [TestCaseSource(nameof(GetUnroundedValues), new object[]
        {
            "XCoordinateDrainageConstruction"
        })]
        public void XCoordinateDrainageConstruction_SetToNew_ValueIsRounded(double newValue)
        {
            TestValueIsRounded(input => input.XCoordinateDrainageConstruction = (RoundedDouble) newValue, newValue, input => input.XCoordinateDrainageConstruction);
        }

        [Test]
        [TestCaseSource(nameof(GetUnroundedValues), new object[]
        {
            "ZCoordinateDrainageConstruction"
        })]
        public void ZCoordinateDrainageConstruction_SetToNew_ValueIsRounded(double newValue)
        {
            TestValueIsRounded(input => input.ZCoordinateDrainageConstruction = (RoundedDouble) newValue, newValue, input => input.ZCoordinateDrainageConstruction);
        }

        [Test]
        [TestCaseSource(nameof(GetUnroundedValues), new object[]
        {
            "MinimumLevelPhreaticLineAtDikeTopRiver"
        })]
        public void MinimumLevelPhreaticLineAtDikeTopRiver_SetToNew_ValueIsRounded(double newValue)
        {
            TestValueIsRounded(input => input.MinimumLevelPhreaticLineAtDikeTopRiver = (RoundedDouble) newValue, newValue, input => input.MinimumLevelPhreaticLineAtDikeTopRiver);
        }

        [Test]
        [TestCaseSource(nameof(GetUnroundedValues), new object[]
        {
            "MinimumLevelPhreaticLineAtDikeTopPolder"
        })]
        public void MinimumLevelPhreaticLineAtDikeTopPolder_SetToNew_ValueIsRounded(double newValue)
        {
            TestValueIsRounded(input => input.MinimumLevelPhreaticLineAtDikeTopPolder = (RoundedDouble) newValue, newValue, input => input.MinimumLevelPhreaticLineAtDikeTopPolder);
        }

        [Test]
        [TestCaseSource(nameof(GetUnroundedValues), new object[]
        {
            "PhreaticLineOffsetBelowDikeTopAtRiver"
        })]
        public void PhreaticLineOffsetBelowDikeTopAtRiver_SetToNew_ValueIsRounded(double newValue)
        {
            TestValueIsRounded(input => input.PhreaticLineOffsetBelowDikeTopAtRiver = (RoundedDouble) newValue, newValue, input => input.PhreaticLineOffsetBelowDikeTopAtRiver);
        }

        [Test]
        [TestCaseSource(nameof(GetUnroundedValues), new object[]
        {
            "PhreaticLineOffsetBelowDikeTopAtPolder"
        })]
        public void PhreaticLineOffsetBelowDikeTopAtPolder_SetToNew_ValueIsRounded(double newValue)
        {
            TestValueIsRounded(input => input.PhreaticLineOffsetBelowDikeTopAtPolder = (RoundedDouble) newValue, newValue, input => input.PhreaticLineOffsetBelowDikeTopAtPolder);
        }

        [Test]
        [TestCaseSource(nameof(GetUnroundedValues), new object[]
        {
            "PhreaticLineOffsetBelowShoulderBaseInside"
        })]
        public void PhreaticLineOffsetBelowShoulderBaseInside_SetToNew_ValueIsRounded(double newValue)
        {
            TestValueIsRounded(input => input.PhreaticLineOffsetBelowShoulderBaseInside = (RoundedDouble) newValue, newValue, input => input.PhreaticLineOffsetBelowShoulderBaseInside);
        }

        [Test]
        [TestCaseSource(nameof(GetUnroundedValues), new object[]
        {
            "PhreaticLineOffsetBelowDikeToeAtPolder"
        })]
        public void PhreaticLineOffsetBelowDikeToeAtPolder_SetToNew_ValueIsRounded(double newValue)
        {
            TestValueIsRounded(input => input.PhreaticLineOffsetBelowDikeToeAtPolder = (RoundedDouble) newValue, newValue, input => input.PhreaticLineOffsetBelowDikeToeAtPolder);
        }

        [Test]
        [TestCaseSource(nameof(GetUnroundedValues), new object[]
        {
            "LeakageLengthOutwardsPhreaticLine3"
        })]
        public void LeakageLengthOutwardsPhreaticLine3_SetToNew_ValueIsRounded(double newValue)
        {
            TestValueIsRounded(input => input.LeakageLengthOutwardsPhreaticLine3 = (RoundedDouble) newValue, newValue, input => input.LeakageLengthOutwardsPhreaticLine3);
        }

        [Test]
        [TestCaseSource(nameof(GetUnroundedValues), new object[]
        {
            "LeakageLengthInwardsPhreaticLine3"
        })]
        public void LeakageLengthInwardsPhreaticLine3_SetToNew_ValueIsRounded(double newValue)
        {
            TestValueIsRounded(input => input.LeakageLengthInwardsPhreaticLine3 = (RoundedDouble) newValue, newValue, input => input.LeakageLengthInwardsPhreaticLine3);
        }

        [Test]
        [TestCaseSource(nameof(GetUnroundedValues), new object[]
        {
            "LeakageLengthOutwardsPhreaticLine4"
        })]
        public void LeakageLengthOutwardsPhreaticLine4_SetToNew_ValueIsRounded(double newValue)
        {
            TestValueIsRounded(input => input.LeakageLengthOutwardsPhreaticLine4 = (RoundedDouble) newValue, newValue, input => input.LeakageLengthOutwardsPhreaticLine4);
        }

        [Test]
        [TestCaseSource(nameof(GetUnroundedValues), new object[]
        {
            "LeakageLengthInwardsPhreaticLine4"
        })]
        public void LeakageLengthInwardsPhreaticLine4_SetToNew_ValueIsRounded(double newValue)
        {
            TestValueIsRounded(input => input.LeakageLengthInwardsPhreaticLine4 = (RoundedDouble) newValue, newValue, input => input.LeakageLengthInwardsPhreaticLine4);
        }

        [Test]
        [TestCaseSource(nameof(GetUnroundedValues), new object[]
        {
            "PiezometricHeadPhreaticLine2Outwards"
        })]
        public void PiezometricHeadPhreaticLine2Outwards_SetToNew_ValueIsRounded(double newValue)
        {
            TestValueIsRounded(input => input.PiezometricHeadPhreaticLine2Outwards = (RoundedDouble) newValue, newValue, input => input.PiezometricHeadPhreaticLine2Outwards);
        }

        [Test]
        [TestCaseSource(nameof(GetUnroundedValues), new object[]
        {
            "PiezometricHeadPhreaticLine2Inwards"
        })]
        public void PiezometricHeadPhreaticLine2Inwards_SetToNew_ValueIsRounded(double newValue)
        {
            TestValueIsRounded(input => input.PiezometricHeadPhreaticLine2Inwards = (RoundedDouble) newValue, newValue, input => input.PiezometricHeadPhreaticLine2Inwards);
        }


        [Test]
        [TestCaseSource(nameof(GetUnroundedValues), new object[]
        {
            "PenetrationLength"
        })]
        public void PenetrationLength_SetToNew_ValueIsRounded(double newValue)
        {
            TestValueIsRounded(input => input.PenetrationLength = (RoundedDouble) newValue, newValue, input => input.PenetrationLength);
        }

        private static void TestValueIsRounded(Action<MacroStabilityInwardsInput> setValue, double newValue, Func<MacroStabilityInwardsInput, RoundedDouble> getValue)
        {
            // Setup
            var input = new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput());

            int originalNumberOfDecimalPlaces = getValue(input).NumberOfDecimalPlaces;

            // Call
            setValue(input);

            // Assert
            Assert.AreEqual(originalNumberOfDecimalPlaces, getValue(input).NumberOfDecimalPlaces);
            Assert.AreEqual(new RoundedDouble(originalNumberOfDecimalPlaces, newValue), getValue(input));
        }

        private static IEnumerable<TestCaseData> GetUnroundedValues(string name)
        {
            yield return new TestCaseData(double.NaN).SetName($"{name} double.NaN");
            yield return new TestCaseData(-1e-3).SetName($"{name} -1e-3");
            yield return new TestCaseData(0.005).SetName($"{name} 0.005");
            yield return new TestCaseData(0.1004).SetName($"{name} 0.1004");
            yield return new TestCaseData(0.5).SetName($"{name} 0.5");
        }
    }
}