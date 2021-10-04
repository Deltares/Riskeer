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
using System.Linq;
using Core.Common.Base;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Helpers;

namespace Riskeer.Common.Forms.Test.Helpers
{
    [TestFixture]
    public class TargetProbabilityCalculationsDisplayNameHelperTest
    {
        [Test]
        public void GetUniqueDisplayNameForWaterLevelCalculations_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => TargetProbabilityCalculationsDisplayNameHelper.GetUniqueDisplayNameForWaterLevelCalculations(
                null, Enumerable.Empty<HydraulicBoundaryLocationCalculation>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void GetUniqueDisplayNameForWaterLevelCalculations_CalculationsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => TargetProbabilityCalculationsDisplayNameHelper.GetUniqueDisplayNameForWaterLevelCalculations(
                new AssessmentSectionStub(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculations", exception.ParamName);
        }

        [Test]
        [TestCase(0.0025, 0.00025, 0.0025, 0.0025, "1/400")]
        [TestCase(0.0025, 0.0025, 0.0025, 0.0025, "1/400")]
        public void GetUniqueDisplayNameForWaterLevelCalculations_ValidParameters_ReturnsExpectedDisplayNameForLowerLimitNorm(
            double lowerLimitNorm, double signalingNorm, double userDefinedTargetProbability1, double userDefinedTargetProbability2, string expectedName)
        {
            // Setup
            var hydraulicBoundaryLocationCalculations = new ObservableList<HydraulicBoundaryLocationCalculation>();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mockRepository);

            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(new FailureMechanismContribution(lowerLimitNorm, signalingNorm));
            assessmentSection.Stub(a => a.WaterLevelCalculationsForLowerLimitNorm).Return(hydraulicBoundaryLocationCalculations);
            assessmentSection.Stub(a => a.WaterLevelCalculationsForSignalingNorm).Return(new ObservableList<HydraulicBoundaryLocationCalculation>());
            assessmentSection.Stub(a => a.WaterLevelCalculationsForUserDefinedTargetProbabilities).Return(
                new ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>
                {
                    new HydraulicBoundaryLocationCalculationsForTargetProbability(userDefinedTargetProbability1),
                    new HydraulicBoundaryLocationCalculationsForTargetProbability(userDefinedTargetProbability2)
                });

            mockRepository.ReplayAll();

            // Call
            string name = TargetProbabilityCalculationsDisplayNameHelper.GetUniqueDisplayNameForWaterLevelCalculations(assessmentSection, hydraulicBoundaryLocationCalculations);

            // Assert
            Assert.AreEqual(expectedName, name);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(0.025, 0.0025, 0.0025, 0.0025, "1/400")]
        [TestCase(0.0025, 0.0025, 0.0025, 0.0025, "1/400 (1)")]
        public void GetUniqueDisplayNameForWaterLevelCalculations_ValidParameters_ReturnsExpectedDisplayNameForSignalingNorm(
            double lowerLimitNorm, double signalingNorm, double userDefinedTargetProbability1, double userDefinedTargetProbability2, string expectedName)
        {
            // Setup
            var hydraulicBoundaryLocationCalculations = new ObservableList<HydraulicBoundaryLocationCalculation>();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mockRepository);

            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(new FailureMechanismContribution(lowerLimitNorm, signalingNorm));
            assessmentSection.Stub(a => a.WaterLevelCalculationsForLowerLimitNorm).Return(new ObservableList<HydraulicBoundaryLocationCalculation>());
            assessmentSection.Stub(a => a.WaterLevelCalculationsForSignalingNorm).Return(hydraulicBoundaryLocationCalculations);
            assessmentSection.Stub(a => a.WaterLevelCalculationsForUserDefinedTargetProbabilities).Return(
                new ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>
                {
                    new HydraulicBoundaryLocationCalculationsForTargetProbability(userDefinedTargetProbability1),
                    new HydraulicBoundaryLocationCalculationsForTargetProbability(userDefinedTargetProbability2)
                });

            mockRepository.ReplayAll();

            // Call
            string name = TargetProbabilityCalculationsDisplayNameHelper.GetUniqueDisplayNameForWaterLevelCalculations(assessmentSection, hydraulicBoundaryLocationCalculations);

            // Assert
            Assert.AreEqual(expectedName, name);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(0.1, 0.01, 0.025, 0.0025, "1/400")]
        [TestCase(0.1, 0.01, 0.0025, 0.0025, "1/400 (1)")]
        [TestCase(0.1, 0.0025, 0.025, 0.0025, "1/400 (1)")]
        [TestCase(0.0025, 0.00025, 0.025, 0.0025, "1/400 (1)")]
        [TestCase(0.0025, 0.0025, 0.025, 0.0025, "1/400 (2)")]
        [TestCase(0.0025, 0.0025, 0.0025, 0.0025, "1/400 (3)")]
        public void GetUniqueDisplayNameForWaterLevelCalculations_ValidParameters_ReturnsExpectedDisplayNameForUserDefinedTargetProbability(
            double lowerLimitNorm, double signalingNorm, double userDefinedTargetProbability1, double userDefinedTargetProbability2, string expectedName)
        {
            // Setup
            var calculationsForTargetProbability = new HydraulicBoundaryLocationCalculationsForTargetProbability(userDefinedTargetProbability2);

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mockRepository);

            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(new FailureMechanismContribution(lowerLimitNorm, signalingNorm));
            assessmentSection.Stub(a => a.WaterLevelCalculationsForLowerLimitNorm).Return(new ObservableList<HydraulicBoundaryLocationCalculation>());
            assessmentSection.Stub(a => a.WaterLevelCalculationsForSignalingNorm).Return(new ObservableList<HydraulicBoundaryLocationCalculation>());
            assessmentSection.Stub(a => a.WaterLevelCalculationsForUserDefinedTargetProbabilities).Return(
                new ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>
                {
                    new HydraulicBoundaryLocationCalculationsForTargetProbability(userDefinedTargetProbability1),
                    calculationsForTargetProbability
                });

            mockRepository.ReplayAll();

            // Call
            string name = TargetProbabilityCalculationsDisplayNameHelper.GetUniqueDisplayNameForWaterLevelCalculations(assessmentSection, calculationsForTargetProbability.HydraulicBoundaryLocationCalculations);

            // Assert
            Assert.AreEqual(expectedName, name);

            mockRepository.VerifyAll();
        }
    }
}