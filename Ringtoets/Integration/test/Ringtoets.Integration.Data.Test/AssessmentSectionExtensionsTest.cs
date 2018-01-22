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
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Integration.Data.Test
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
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                FailureMechanismContribution =
                {
                    NormativeNorm = (NormType) invalidValue
                }
            };

            // Call
            TestDelegate test = () => assessmentSection.GetNormativeAssessmentLevel(new TestHydraulicBoundaryLocation());

            // Assert
            string expectedMessage = $"The value of argument 'normType' ({invalidValue}) is invalid for Enum type '{nameof(NormType)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage).ParamName;
            Assert.AreEqual("normType", parameterName);
        }

        [Test]
        public void GetNormativeAssessmentLevel_HydraulicBoundaryLocationWithOutputAndNormTypeSignaling_ReturnsCorrespondingAssessmentLevel()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                FailureMechanismContribution =
                {
                    NormativeNorm = NormType.Signaling
                }
            };

            HydraulicBoundaryLocation hydraulicBoundaryLocationWithOutput = CreateHydraulicBoundaryLocationWithOutput();

            // Call
            RoundedDouble normativeAssessmentLevel = assessmentSection.GetNormativeAssessmentLevel(hydraulicBoundaryLocationWithOutput);

            // Assert
            Assert.AreEqual(hydraulicBoundaryLocationWithOutput.DesignWaterLevelCalculation2.Output.Result, normativeAssessmentLevel);
        }

        [Test]
        public void GetNormativeAssessmentLevel_HydraulicBoundaryLocationWithOutputAndNormTypeLowerLimit_ReturnsCorrespondingAssessmentLevel()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                FailureMechanismContribution =
                {
                    NormativeNorm = NormType.LowerLimit
                }
            };

            HydraulicBoundaryLocation hydraulicBoundaryLocationWithOutput = CreateHydraulicBoundaryLocationWithOutput();

            // Call
            RoundedDouble normativeAssessmentLevel = assessmentSection.GetNormativeAssessmentLevel(hydraulicBoundaryLocationWithOutput);

            // Assert
            Assert.AreEqual(hydraulicBoundaryLocationWithOutput.DesignWaterLevelCalculation3.Output.Result, normativeAssessmentLevel);
        }

        [TestCase(NormType.Signaling)]
        [TestCase(NormType.LowerLimit)]
        public void GetNormativeAssessmentLevel_HydraulicBoundaryLocationNull_ReturnsNaN(NormType normType)
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                FailureMechanismContribution =
                {
                    NormativeNorm = normType
                }
            };

            // Call
            RoundedDouble normativeAssessmentLevel = assessmentSection.GetNormativeAssessmentLevel(null);

            // Assert
            Assert.AreEqual(RoundedDouble.NaN, normativeAssessmentLevel);
        }

        [TestCase(NormType.Signaling)]
        [TestCase(NormType.LowerLimit)]
        public void GetNormativeAssessmentLevel_NoCorrespondingAssessmentLevelOutput_ReturnsNaN(NormType normType)
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                FailureMechanismContribution =
                {
                    NormativeNorm = normType
                }
            };

            // Call
            RoundedDouble normativeAssessmentLevel = assessmentSection.GetNormativeAssessmentLevel(new TestHydraulicBoundaryLocation());

            // Assert
            Assert.AreEqual(RoundedDouble.NaN, normativeAssessmentLevel);
        }

        private static HydraulicBoundaryLocation CreateHydraulicBoundaryLocationWithOutput()
        {
            var random = new Random(32);

            return new TestHydraulicBoundaryLocation
            {
                DesignWaterLevelCalculation2 =
                {
                    Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble())
                },
                DesignWaterLevelCalculation3 =
                {
                    Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble())
                }
            };
        }
    }
}