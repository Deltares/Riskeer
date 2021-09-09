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
using System.ComponentModel;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.GrassCoverErosionOutwards.Data.TestUtil;

namespace Riskeer.GrassCoverErosionOutwards.Data.Test
{
    [TestFixture]
    public class GrassCoverErosionOutwardsFailureMechanismExtensionsTest
    {
        [Test]
        public void GetAssessmentLevel_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => GrassCoverErosionOutwardsFailureMechanismExtensions.GetAssessmentLevel(null,
                                                                                                  new AssessmentSectionStub(),
                                                                                                  new TestHydraulicBoundaryLocation(),
                                                                                                  FailureMechanismCategoryType.FactorizedLowerLimitNorm);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void GetAssessmentLevel_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            void Call() => failureMechanism.GetAssessmentLevel(null,
                                                               new TestHydraulicBoundaryLocation(),
                                                               FailureMechanismCategoryType.FactorizedLowerLimitNorm);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("assessmentSection", paramName);
        }

        [Test]
        public void GetAssessmentLevel_InvalidFailureMechanismCategoryType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const int invalidValue = 9999;

            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            void Call() => failureMechanism.GetAssessmentLevel(assessmentSection,
                                                               new TestHydraulicBoundaryLocation(),
                                                               (FailureMechanismCategoryType) invalidValue);

            // Assert
            var expectedMessage = $"The value of argument 'categoryType' ({invalidValue}) is invalid for Enum type '{nameof(FailureMechanismCategoryType)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage).ParamName;
            Assert.AreEqual("categoryType", parameterName);
        }

        [Test]
        public void GetAssessmentLevel_HydraulicBoundaryLocationNull_ReturnsNaN()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            RoundedDouble assessmentLevel = failureMechanism.GetAssessmentLevel(assessmentSection,
                                                                                null,
                                                                                new Random(32).NextEnumValue<FailureMechanismCategoryType>());

            // Assert
            Assert.IsNaN(assessmentLevel);
        }

        [Test]
        public void GetAssessmentLevel_NoCorrespondingCalculation_ReturnsNaN()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            RoundedDouble assessmentLevel = failureMechanism.GetAssessmentLevel(assessmentSection,
                                                                                new TestHydraulicBoundaryLocation(),
                                                                                new Random(32).NextEnumValue<FailureMechanismCategoryType>());

            // Assert
            Assert.IsNaN(assessmentLevel);
        }

        [Test]
        public void GetAssessmentLevel_NoCorrespondingAssessmentLevelOutput_ReturnsNaN()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            RoundedDouble assessmentLevel = failureMechanism.GetAssessmentLevel(assessmentSection,
                                                                                hydraulicBoundaryLocation,
                                                                                new Random(32).NextEnumValue<FailureMechanismCategoryType>());

            // Assert
            Assert.IsNaN(assessmentLevel);
        }

        [Test]
        [TestCaseSource(
            typeof(GrassCoverErosionOutwardsAssessmentSectionTestHelper),
            nameof(GrassCoverErosionOutwardsAssessmentSectionTestHelper.GetHydraulicBoundaryLocationCalculationConfigurationPerFailureMechanismCategoryType))]
        public void GetAssessmentLevel_HydraulicBoundaryLocationWithOutput_ReturnsCorrespondingAssessmentLevel(
            IAssessmentSection assessmentSection,
            GrassCoverErosionOutwardsFailureMechanism failureMechanism,
            HydraulicBoundaryLocation hydraulicBoundaryLocation,
            FailureMechanismCategoryType categoryType,
            HydraulicBoundaryLocationCalculation expectedHydraulicBoundaryLocationCalculation)
        {
            // Call
            RoundedDouble assessmentLevel = failureMechanism.GetAssessmentLevel(assessmentSection,
                                                                                hydraulicBoundaryLocation,
                                                                                categoryType);

            // Assert
            Assert.AreEqual(expectedHydraulicBoundaryLocationCalculation.Output.Result, assessmentLevel);
        }

        [Test]
        public void GetHydraulicBoundaryLocationCalculation_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => GrassCoverErosionOutwardsFailureMechanismExtensions.GetHydraulicBoundaryLocationCalculation(
                null,
                new AssessmentSectionStub(),
                new TestHydraulicBoundaryLocation(),
                FailureMechanismCategoryType.FactorizedLowerLimitNorm);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void GetHydraulicBoundaryLocationCalculation_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            void Call() => failureMechanism.GetHydraulicBoundaryLocationCalculation(
                null,
                new TestHydraulicBoundaryLocation(),
                FailureMechanismCategoryType.FactorizedLowerLimitNorm);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("assessmentSection", paramName);
        }

        [Test]
        public void GetHydraulicBoundaryLocationCalculation_InvalidFailureMechanismCategoryType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const int invalidValue = 9999;

            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            void Call() => failureMechanism.GetHydraulicBoundaryLocationCalculation(assessmentSection,
                                                                                    new TestHydraulicBoundaryLocation(),
                                                                                    (FailureMechanismCategoryType) invalidValue);

            // Assert
            var expectedMessage = $"The value of argument 'categoryType' ({invalidValue}) is invalid for Enum type '{nameof(FailureMechanismCategoryType)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage).ParamName;
            Assert.AreEqual("categoryType", parameterName);
        }

        [Test]
        public void GetHydraulicBoundaryLocationCalculation_HydraulicBoundaryLocationNull_ReturnsNull()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation = failureMechanism.GetHydraulicBoundaryLocationCalculation(
                assessmentSection,
                null,
                new Random(32).NextEnumValue<FailureMechanismCategoryType>());

            // Assert
            Assert.IsNull(hydraulicBoundaryLocationCalculation);
        }

        [Test]
        public void GetHydraulicBoundaryLocationCalculation_NoCorrespondingCalculation_ReturnsNull()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation = failureMechanism.GetHydraulicBoundaryLocationCalculation(
                assessmentSection,
                new TestHydraulicBoundaryLocation(),
                new Random(32).NextEnumValue<FailureMechanismCategoryType>());

            // Assert
            Assert.IsNull(hydraulicBoundaryLocationCalculation);
        }

        [Test]
        [TestCaseSource(
            typeof(GrassCoverErosionOutwardsAssessmentSectionTestHelper),
            nameof(GrassCoverErosionOutwardsAssessmentSectionTestHelper.GetHydraulicBoundaryLocationCalculationConfigurationPerFailureMechanismCategoryType))]
        public void GetHydraulicBoundaryLocationCalculation_HydraulicBoundaryLocationWithOutput_ReturnsCorrespondingHydraulicBoundaryLocationCalculation(
            IAssessmentSection assessmentSection,
            GrassCoverErosionOutwardsFailureMechanism failureMechanism,
            HydraulicBoundaryLocation hydraulicBoundaryLocation,
            FailureMechanismCategoryType categoryType,
            HydraulicBoundaryLocationCalculation expectedHydraulicBoundaryLocationCalculation)
        {
            // Call
            HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation = failureMechanism.GetHydraulicBoundaryLocationCalculation(
                assessmentSection,
                hydraulicBoundaryLocation,
                categoryType);

            // Assert
            Assert.AreSame(expectedHydraulicBoundaryLocationCalculation, hydraulicBoundaryLocationCalculation);
        }
    }
}