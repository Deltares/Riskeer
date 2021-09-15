// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Revetment.Data.TestUtil;

namespace Riskeer.Revetment.Data.Test
{
    [TestFixture]
    public class WaveConditionsInputHelperTest
    {
        [Test]
        [TestCase(-0.5, -0.51)]
        [TestCase(0.0, -0.01)]
        [TestCase(0.5, 0.49)]
        [TestCase(double.NaN, double.NaN)]
        public void GetUpperBoundaryAssessmentLevel_DifferentAssessmentLevels_ReturnsExpectedUpperBoundary(double assessmentLevel,
                                                                                                           double expectedUpperBoundary)
        {
            // Call
            RoundedDouble upperBoundary = WaveConditionsInputHelper.GetUpperBoundaryAssessmentLevel((RoundedDouble) assessmentLevel);

            // Assert
            Assert.AreEqual(2, upperBoundary.NumberOfDecimalPlaces);
            Assert.AreEqual(expectedUpperBoundary, upperBoundary);
        }

        [Test]
        public void SetWaterLevelType_AssessmentSectionCategoryWaveConditionsInputNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            void Call() => WaveConditionsInputHelper.SetWaterLevelType(null,
                                                                       random.NextEnumValue<NormType>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("waveConditionsInput", exception.ParamName);
        }

        [Test]
        public void SetWaterLevelType_WithAssessmentSectionCategoryWaveConditionsInputAndInvalidNormType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const NormType normType = (NormType) 99;

            // Call
            void Call() => WaveConditionsInputHelper.SetWaterLevelType(new AssessmentSectionCategoryWaveConditionsInput(), normType);

            // Assert
            var expectedMessage = $"The value of argument 'normType' ({(int) normType}) is invalid for Enum type '{nameof(NormType)}'.";
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage);
            Assert.AreEqual("normType", exception.ParamName);
        }

        [Test]
        [TestCase(NormType.LowerLimit, WaveConditionsInputWaterLevelType.LowerLimit)]
        [TestCase(NormType.Signaling, WaveConditionsInputWaterLevelType.Signaling)]
        public void SetWaterLevelType_WithAssessmentSectionCategoryWaveConditionsInputAndVariousNormTypes_SetsWaterLevelType(
            NormType normType,
            WaveConditionsInputWaterLevelType expectedWaveConditionsInputWaterLevelType)
        {
            // Setup
            var waveConditionsInput = new AssessmentSectionCategoryWaveConditionsInput();

            // Call
            WaveConditionsInputHelper.SetWaterLevelType(waveConditionsInput, normType);

            // Assert
            Assert.AreEqual(expectedWaveConditionsInputWaterLevelType, waveConditionsInput.WaterLevelType);
        }

        [Test]
        public void SetWaterLevelType_FailureMechanismCategoryWaveConditionsInputNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            void Call() => WaveConditionsInputHelper.SetCategoryType(null,
                                                                     random.NextEnumValue<NormType>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("waveConditionsInput", exception.ParamName);
        }

        [Test]
        public void SetCategoryType_WithFailureMechanismCategoryWaveConditionsInputAndInvalidNormType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const NormType normType = (NormType) 99;

            // Call
            void Call() => WaveConditionsInputHelper.SetCategoryType(new FailureMechanismCategoryWaveConditionsInput(), normType);

            // Assert
            var expectedMessage = $"The value of argument 'normType' ({(int) normType}) is invalid for Enum type '{nameof(NormType)}'.";
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage);
            Assert.AreEqual("normType", exception.ParamName);
        }

        [Test]
        [TestCase(NormType.Signaling, FailureMechanismCategoryType.MechanismSpecificSignalingNorm)]
        [TestCase(NormType.LowerLimit, FailureMechanismCategoryType.MechanismSpecificLowerLimitNorm)]
        public void SetCategoryType_WithFailureMechanismCategoryWaveConditionsInputAndVariousNormTypes_SetsCategoryType(
            NormType normType,
            FailureMechanismCategoryType expectedFailureMechanismCategoryType)
        {
            // Setup
            var waveConditionsInput = new FailureMechanismCategoryWaveConditionsInput();

            // Call
            WaveConditionsInputHelper.SetCategoryType(waveConditionsInput, normType);

            // Assert
            Assert.AreEqual(expectedFailureMechanismCategoryType, waveConditionsInput.CategoryType);
        }

        [Test]
        public void GetHydraulicBoundaryLocationCalculation_InputNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => WaveConditionsInputHelper.GetHydraulicBoundaryLocationCalculation(null, new AssessmentSectionStub());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("input", exception.ParamName);
        }

        [Test]
        public void GetHydraulicBoundaryLocationCalculation_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => WaveConditionsInputHelper.GetHydraulicBoundaryLocationCalculation(new TestWaveConditionsInput(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void GetHydraulicBoundaryLocationCalculation_InputWithInvalidWaterLevelType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const WaveConditionsInputWaterLevelType waterLevelType = (WaveConditionsInputWaterLevelType) 99;
            var waveConditionsInput = new TestWaveConditionsInput
            {
                WaterLevelType = waterLevelType
            };

            // Call
            void Call() => WaveConditionsInputHelper.GetHydraulicBoundaryLocationCalculation(waveConditionsInput, new AssessmentSectionStub());

            // Assert
            var expectedMessage = $"The value of argument 'WaterLevelType' ({(int) waterLevelType}) is invalid for Enum type '{nameof(WaveConditionsInputWaterLevelType)}'.";
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage);
            Assert.AreEqual("WaterLevelType", exception.ParamName);
        }

        [Test]
        [TestCaseSource(nameof(GetHydraulicBoundaryLocationCalculationConfigurations))]
        public void GetHydraulicBoundaryLocationCalculation_ValidInput_ReturnsExpectedValue(
            WaveConditionsInputWaterLevelType waterLevelType,
            Func<WaveConditionsInput, IAssessmentSection, HydraulicBoundaryLocationCalculation> getExpectedTargetProbability)
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            }, true);

            var input = new TestWaveConditionsInput
            {
                HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                WaterLevelType = waterLevelType,
                CalculationsTargetProbability = assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities.First()
            };

            // Call
            HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation = WaveConditionsInputHelper.GetHydraulicBoundaryLocationCalculation(input, assessmentSection);

            // Assert
            Assert.AreEqual(getExpectedTargetProbability(input, assessmentSection), hydraulicBoundaryLocationCalculation);
        }

        [Test]
        public void GetTargetProbability_InputNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => WaveConditionsInputHelper.GetTargetProbability(null, new AssessmentSectionStub());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("input", exception.ParamName);
        }

        [Test]
        public void GetTargetProbability_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => WaveConditionsInputHelper.GetTargetProbability(new TestWaveConditionsInput(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void GetTargetProbability_InputWithInvalidWaterLevelType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const WaveConditionsInputWaterLevelType waterLevelType = (WaveConditionsInputWaterLevelType) 99;
            var waveConditionsInput = new TestWaveConditionsInput
            {
                WaterLevelType = waterLevelType
            };

            // Call
            void Call() => WaveConditionsInputHelper.GetTargetProbability(waveConditionsInput, new AssessmentSectionStub());

            // Assert
            var expectedMessage = $"The value of argument 'WaterLevelType' ({(int) waterLevelType}) is invalid for Enum type '{nameof(WaveConditionsInputWaterLevelType)}'.";
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage);
            Assert.AreEqual("WaterLevelType", exception.ParamName);
        }

        [Test]
        [TestCaseSource(nameof(GetTargetProbabilityConfigurations))]
        public void GetTargetProbability_ValidInput_ReturnsExpectedValue(
            WaveConditionsInputWaterLevelType waterLevelType,
            Func<WaveConditionsInput, IAssessmentSection, double> getExpectedTargetProbability)
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();

            var input = new TestWaveConditionsInput
            {
                WaterLevelType = waterLevelType,
                CalculationsTargetProbability = new HydraulicBoundaryLocationCalculationsForTargetProbability(0.01)
            };

            // Call
            double targetProbability = WaveConditionsInputHelper.GetTargetProbability(input, assessmentSection);

            // Assert
            Assert.AreEqual(getExpectedTargetProbability(input, assessmentSection), targetProbability);
        }

        [Test]
        public void GetAssessmentLevel_InputNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => WaveConditionsInputHelper.GetAssessmentLevel(null, new AssessmentSectionStub());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("input", exception.ParamName);
        }

        [Test]
        public void GetAssessmentLevel_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => WaveConditionsInputHelper.GetAssessmentLevel(new TestWaveConditionsInput(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void GetAssessmentLevel_InputWithInvalidWaterLevelType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const WaveConditionsInputWaterLevelType waterLevelType = (WaveConditionsInputWaterLevelType) 99;
            var waveConditionsInput = new TestWaveConditionsInput
            {
                WaterLevelType = waterLevelType
            };

            // Call
            void Call() => WaveConditionsInputHelper.GetAssessmentLevel(waveConditionsInput, new AssessmentSectionStub());

            // Assert
            var expectedMessage = $"The value of argument 'WaterLevelType' ({(int) waterLevelType}) is invalid for Enum type '{nameof(WaveConditionsInputWaterLevelType)}'.";
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage);
            Assert.AreEqual("WaterLevelType", exception.ParamName);
        }

        [Test]
        [TestCase(WaveConditionsInputWaterLevelType.None)]
        [TestCase(WaveConditionsInputWaterLevelType.LowerLimit)]
        [TestCase(WaveConditionsInputWaterLevelType.Signaling)]
        [TestCase(WaveConditionsInputWaterLevelType.UserDefinedTargetProbability)]
        public void GetAssessmentLevel_ValidInputWithoutHydraulicBoundaryLocation_ReturnsNaN(WaveConditionsInputWaterLevelType waterLevelType)
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();

            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                new TestHydraulicBoundaryLocation()
            }, true);

            var input = new TestWaveConditionsInput
            {
                WaterLevelType = waterLevelType,
                CalculationsTargetProbability = assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities.First()
            };

            // Call
            double assessmentLevel = WaveConditionsInputHelper.GetAssessmentLevel(input, assessmentSection);

            // Assert
            Assert.IsNaN(assessmentLevel);
        }

        [Test]
        [TestCaseSource(nameof(GetAssessmentLevelConfigurations))]
        public void GetAssessmentLevel_ValidInputWithHydraulicBoundaryLocation_ReturnsExpectedValue(
            WaveConditionsInputWaterLevelType waterLevelType,
            Func<WaveConditionsInput, IAssessmentSection, double> getExpectedAssessmentLevel)
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            }, true);

            var input = new TestWaveConditionsInput
            {
                HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                WaterLevelType = waterLevelType,
                CalculationsTargetProbability = assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities.First()
            };

            // Call
            double assessmentLevel = WaveConditionsInputHelper.GetAssessmentLevel(input, assessmentSection);

            // Assert
            Assert.AreEqual(getExpectedAssessmentLevel(input, assessmentSection), assessmentLevel);
        }

        private static IEnumerable<TestCaseData> GetHydraulicBoundaryLocationCalculationConfigurations()
        {
            yield return new TestCaseData(
                WaveConditionsInputWaterLevelType.None,
                (Func<WaveConditionsInput, IAssessmentSection, HydraulicBoundaryLocationCalculation>) ((input, assessmentSection) => null));
            yield return new TestCaseData(
                WaveConditionsInputWaterLevelType.LowerLimit,
                (Func<WaveConditionsInput, IAssessmentSection, HydraulicBoundaryLocationCalculation>) ((input, assessmentSection) => assessmentSection.WaterLevelCalculationsForLowerLimitNorm.First()));
            yield return new TestCaseData(
                WaveConditionsInputWaterLevelType.Signaling,
                (Func<WaveConditionsInput, IAssessmentSection, HydraulicBoundaryLocationCalculation>) ((input, assessmentSection) => assessmentSection.WaterLevelCalculationsForSignalingNorm.First()));
            yield return new TestCaseData(
                WaveConditionsInputWaterLevelType.UserDefinedTargetProbability,
                (Func<WaveConditionsInput, IAssessmentSection, HydraulicBoundaryLocationCalculation>) ((input, assessmentSection) => input.CalculationsTargetProbability.HydraulicBoundaryLocationCalculations.First()));
        }

        private static IEnumerable<TestCaseData> GetTargetProbabilityConfigurations()
        {
            yield return new TestCaseData(
                WaveConditionsInputWaterLevelType.None,
                (Func<WaveConditionsInput, IAssessmentSection, double>) ((input, assessmentSection) => double.NaN));
            yield return new TestCaseData(
                WaveConditionsInputWaterLevelType.LowerLimit,
                (Func<WaveConditionsInput, IAssessmentSection, double>) ((input, assessmentSection) => assessmentSection.FailureMechanismContribution.LowerLimitNorm));
            yield return new TestCaseData(
                WaveConditionsInputWaterLevelType.Signaling,
                (Func<WaveConditionsInput, IAssessmentSection, double>) ((input, assessmentSection) => assessmentSection.FailureMechanismContribution.SignalingNorm));
            yield return new TestCaseData(
                WaveConditionsInputWaterLevelType.UserDefinedTargetProbability,
                (Func<WaveConditionsInput, IAssessmentSection, double>) ((input, assessmentSection) => input.CalculationsTargetProbability.TargetProbability));
        }

        private static IEnumerable<TestCaseData> GetAssessmentLevelConfigurations()
        {
            yield return new TestCaseData(
                WaveConditionsInputWaterLevelType.None,
                (Func<WaveConditionsInput, IAssessmentSection, double>) ((input, assessmentSection) => double.NaN));
            yield return new TestCaseData(
                WaveConditionsInputWaterLevelType.LowerLimit,
                (Func<WaveConditionsInput, IAssessmentSection, double>) ((input, assessmentSection) => assessmentSection.WaterLevelCalculationsForLowerLimitNorm.First().Output.Result));
            yield return new TestCaseData(
                WaveConditionsInputWaterLevelType.Signaling,
                (Func<WaveConditionsInput, IAssessmentSection, double>) ((input, assessmentSection) => assessmentSection.WaterLevelCalculationsForSignalingNorm.First().Output.Result));
            yield return new TestCaseData(
                WaveConditionsInputWaterLevelType.UserDefinedTargetProbability,
                (Func<WaveConditionsInput, IAssessmentSection, double>) ((input, assessmentSection) => input.CalculationsTargetProbability.HydraulicBoundaryLocationCalculations.First().Output.Result));
        }
    }
}