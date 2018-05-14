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
using System.Collections;
using System.ComponentModel;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Common.Data.Test.AssessmentSection
{
    [TestFixture]
    public class AssessmentSectionExtensionsTest
    {
        [Test]
        public void GetNormativeAssessmentLevel_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => AssessmentSectionExtensions.GetNormativeAssessmentLevel(null, new TestHydraulicBoundaryLocation());

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

            // Call
            TestDelegate test = () => assessmentSection.GetNormativeAssessmentLevel(new TestHydraulicBoundaryLocation());

            // Assert
            string expectedMessage = $"The value of argument 'normType' ({invalidValue}) is invalid for Enum type '{nameof(NormType)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage).ParamName;
            Assert.AreEqual("normType", parameterName);
        }

        [Test]
        public void GetNormativeAssessmentLevel_HydraulicBoundaryLocationNull_ReturnsNaN()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();

            assessmentSection.FailureMechanismContribution.NormativeNorm = new Random(32).NextEnumValue<NormType>();

            // Call
            RoundedDouble normativeAssessmentLevel = assessmentSection.GetNormativeAssessmentLevel(null);

            // Assert
            Assert.IsNaN(normativeAssessmentLevel);
        }

        [Test]
        public void GetNormativeAssessmentLevel_NoCorrespondingCalculation_ReturnsNaN()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();

            assessmentSection.FailureMechanismContribution.NormativeNorm = new Random(32).NextEnumValue<NormType>();

            // Call
            RoundedDouble normativeAssessmentLevel = assessmentSection.GetNormativeAssessmentLevel(new TestHydraulicBoundaryLocation());

            // Assert
            Assert.IsNaN(normativeAssessmentLevel);
        }

        [Test]
        public void GetNormativeAssessmentLevel_NoCorrespondingAssessmentLevelOutput_ReturnsNaN()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            assessmentSection.FailureMechanismContribution.NormativeNorm = new Random(32).NextEnumValue<NormType>();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            // Call
            RoundedDouble normativeAssessmentLevel = assessmentSection.GetNormativeAssessmentLevel(hydraulicBoundaryLocation);

            // Assert
            Assert.IsNaN(normativeAssessmentLevel);
        }

        [Test]
        [TestCaseSource(nameof(DifferentNormTypes))]
        public void GetNormativeAssessmentLevel_HydraulicBoundaryLocationWithOutput_ReturnsCorrespondingAssessmentLevel(
            IAssessmentSection assessmentSection,
            HydraulicBoundaryLocation hydraulicBoundaryLocation,
            NormType normType,
            RoundedDouble expectedNormativeAssessmentLevel)
        {
            // Setup
            assessmentSection.FailureMechanismContribution.NormativeNorm = normType;

            // Call
            RoundedDouble normativeAssessmentLevel = assessmentSection.GetNormativeAssessmentLevel(hydraulicBoundaryLocation);

            // Assert
            Assert.AreEqual(expectedNormativeAssessmentLevel, normativeAssessmentLevel);
        }

        [Test]
        public void GetAssessmentLevel_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => AssessmentSectionExtensions.GetAssessmentLevel(null,
                                                                                     new TestHydraulicBoundaryLocation(),
                                                                                     AssessmentSectionCategoryType.FactorizedLowerLimitNorm);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("assessmentSection", paramName);
        }

        [Test]
        public void GetAssessmentLevel_InvalidAssessmentSectionCategoryType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const int invalidValue = 9999;

            var assessmentSection = new AssessmentSectionStub();

            // Call
            TestDelegate test = () => assessmentSection.GetAssessmentLevel(new TestHydraulicBoundaryLocation(),
                                                                           (AssessmentSectionCategoryType) invalidValue);

            // Assert
            string expectedMessage = $"The value of argument 'categoryType' ({invalidValue}) is invalid for Enum type '{nameof(AssessmentSectionCategoryType)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage).ParamName;
            Assert.AreEqual("categoryType", parameterName);
        }

        [Test]
        public void GetAssessmentLevel_HydraulicBoundaryLocationNull_ReturnsNaN()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();

            // Call
            RoundedDouble assessmentLevel = assessmentSection.GetAssessmentLevel(null,
                                                                                 new Random(32).NextEnumValue<AssessmentSectionCategoryType>());

            // Assert
            Assert.IsNaN(assessmentLevel);
        }

        [Test]
        public void GetAssessmentLevel_NoCorrespondingCalculation_ReturnsNaN()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();

            // Call
            RoundedDouble assessmentLevel = assessmentSection.GetAssessmentLevel(new TestHydraulicBoundaryLocation(),
                                                                                 new Random(32).NextEnumValue<AssessmentSectionCategoryType>());

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

            // Call
            RoundedDouble assessmentLevel = assessmentSection.GetAssessmentLevel(hydraulicBoundaryLocation,
                                                                                 new Random(32).NextEnumValue<AssessmentSectionCategoryType>());

            // Assert
            Assert.IsNaN(assessmentLevel);
        }

        [Test]
        [TestCaseSource(
            typeof(AssessmentSectionHelper),
            nameof(AssessmentSectionHelper.GetAssessmentLevelConfigurationPerAssessmentSectionCategoryType))]
        public void GetAssessmentLevel_HydraulicBoundaryLocationWithOutput_ReturnsCorrespondingAssessmentLevel(
            IAssessmentSection assessmentSection,
            HydraulicBoundaryLocation hydraulicBoundaryLocation,
            AssessmentSectionCategoryType categoryType,
            RoundedDouble expectedAssessmentLevel)
        {
            // Call
            RoundedDouble assessmentLevel = assessmentSection.GetAssessmentLevel(hydraulicBoundaryLocation, categoryType);

            // Assert
            Assert.AreEqual(expectedAssessmentLevel, assessmentLevel);
        }

        private static IEnumerable DifferentNormTypes()
        {
            var assessmentSection = new AssessmentSectionStub();
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            }, true);

            yield return new TestCaseData(
                assessmentSection,
                hydraulicBoundaryLocation,
                NormType.Signaling,
                assessmentSection.WaterLevelCalculationsForSignalingNorm.ElementAt(0).Output.Result
            ).SetName("SignalingNorm");

            yield return new TestCaseData(
                assessmentSection,
                hydraulicBoundaryLocation,
                NormType.LowerLimit,
                assessmentSection.WaterLevelCalculationsForLowerLimitNorm.ElementAt(0).Output.Result
            ).SetName("LowerLimitNorm");
        }
    }
}