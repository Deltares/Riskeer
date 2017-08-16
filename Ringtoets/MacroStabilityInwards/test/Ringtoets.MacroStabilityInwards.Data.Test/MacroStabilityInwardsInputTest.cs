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