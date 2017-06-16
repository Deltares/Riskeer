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