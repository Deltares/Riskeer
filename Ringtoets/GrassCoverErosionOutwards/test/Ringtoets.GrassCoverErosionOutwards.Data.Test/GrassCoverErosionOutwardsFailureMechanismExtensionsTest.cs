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
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Util.TestUtil;

namespace Ringtoets.GrassCoverErosionOutwards.Data.Test
{
    [TestFixture]
    public class GrassCoverErosionOutwardsFailureMechanismExtensionsTest
    {
        [Test]
        public void GetNormativeAssessmentLevel_FailureMechanismnNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => GrassCoverErosionOutwardsFailureMechanismExtensions.GetNormativeAssessmentLevel(null, assessmentSection,
                                                                                                                      new TestHydraulicBoundaryLocation());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void GetNormativeAssessmentLevel_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            TestDelegate test = () => failureMechanism.GetNormativeAssessmentLevel(null, new TestHydraulicBoundaryLocation());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("assessmentSection", paramName);
        }

        [Test]
        public void GetNormativeAssessmentLevel_AssessmentSectionWithInvalidNormType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const int invalidValue = 9999;

            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.FailureMechanismContribution.NormativeNorm = (NormType) invalidValue;

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            TestDelegate test = () => failureMechanism.GetNormativeAssessmentLevel(assessmentSection, new TestHydraulicBoundaryLocation());

            // Assert
            string expectedMessage = $"The value of argument 'normType' ({invalidValue}) is invalid for Enum type '{nameof(NormType)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage).ParamName;
            Assert.AreEqual("normType", parameterName);
        }

        [Test]
        public void GetNormativeAssessmentLevel_HydraulicBoundaryLocationWithOutputAndNormTypeSignaling_ReturnsCorrespondingAssessmentLevel()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.FailureMechanismContribution.NormativeNorm = NormType.Signaling;

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism, assessmentSection,
                new[]
                {
                    hydraulicBoundaryLocation
                }, true);

            // Call
            RoundedDouble normativeAssessmentLevel = failureMechanism.GetNormativeAssessmentLevel(assessmentSection, hydraulicBoundaryLocation);

            // Assert
            Assert.AreEqual(failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm.ElementAt(0).Output.Result, normativeAssessmentLevel);
        }

        [Test]
        public void GetNormativeAssessmentLevel_HydraulicBoundaryLocationWithOutputAndNormTypeLowerLimit_ReturnsCorrespondingAssessmentLevel()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.FailureMechanismContribution.NormativeNorm = NormType.LowerLimit;

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism, assessmentSection,
                new[]
                {
                    hydraulicBoundaryLocation
                }, true);

            // Call
            RoundedDouble normativeAssessmentLevel = failureMechanism.GetNormativeAssessmentLevel(assessmentSection, hydraulicBoundaryLocation);

            // Assert
            Assert.AreEqual(failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm.ElementAt(0).Output.Result, normativeAssessmentLevel);
        }

        [TestCase(NormType.Signaling)]
        [TestCase(NormType.LowerLimit)]
        public void GetNormativeAssessmentLevel_HydraulicBoundaryLocationNull_ReturnsNaN(NormType normType)
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            RoundedDouble normativeAssessmentLevel = failureMechanism.GetNormativeAssessmentLevel(assessmentSection, null);

            // Assert
            Assert.AreEqual(RoundedDouble.NaN, normativeAssessmentLevel);
        }

        [TestCase(NormType.Signaling)]
        [TestCase(NormType.LowerLimit)]
        public void GetNormativeAssessmentLevel_NoCorrespondingCalculation_ReturnsNaN(NormType normType)
        {
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.FailureMechanismContribution.NormativeNorm = normType;

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            RoundedDouble normativeAssessmentLevel = failureMechanism.GetNormativeAssessmentLevel(assessmentSection, new TestHydraulicBoundaryLocation());

            // Assert
            Assert.AreEqual(RoundedDouble.NaN, normativeAssessmentLevel);
        }

        [TestCase(NormType.Signaling)]
        [TestCase(NormType.LowerLimit)]
        public void GetNormativeAssessmentLevel_NoCorrespondingAssessmentLevelOutput_ReturnsNaN(NormType normType)
        {
            var assessmentSection = new AssessmentSectionStub();
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            assessmentSection.FailureMechanismContribution.NormativeNorm = normType;
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            RoundedDouble normativeAssessmentLevel = failureMechanism.GetNormativeAssessmentLevel(assessmentSection, hydraulicBoundaryLocation);

            // Assert
            Assert.AreEqual(RoundedDouble.NaN, normativeAssessmentLevel);
        }
    }
}