﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Common.Data.Test.AssessmentSection
{
    [TestFixture]
    public class AssessmentSectionExtensionsTest
    {
        [Test]
        public void GetNormativeAssessmentLevel_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => AssessmentSectionExtensions.GetNormativeAssessmentLevel(null, new TestHydraulicBoundaryLocation());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("assessmentSection", paramName);
        }

        [Test]
        public void GetNormativeAssessmentLevel_AssessmentSectionWithInvalidNormativeProbabilityType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const int invalidValue = 9999;

            var assessmentSection = new AssessmentSectionStub
            {
                FailureMechanismContribution =
                {
                    NormativeProbabilityType = (NormativeProbabilityType) invalidValue
                }
            };

            // Call
            void Call() => assessmentSection.GetNormativeAssessmentLevel(new TestHydraulicBoundaryLocation());

            // Assert
            var expectedMessage = $"The value of argument 'normativeProbabilityType' ({invalidValue}) is invalid for Enum type '{nameof(NormativeProbabilityType)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage).ParamName;
            Assert.AreEqual("normativeProbabilityType", parameterName);
        }

        [Test]
        public void GetNormativeAssessmentLevel_HydraulicBoundaryLocationNull_ReturnsNaN()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub
            {
                FailureMechanismContribution =
                {
                    NormativeProbabilityType = new Random(32).NextEnumValue<NormativeProbabilityType>()
                }
            };

            // Call
            RoundedDouble normativeAssessmentLevel = assessmentSection.GetNormativeAssessmentLevel(null);

            // Assert
            Assert.IsNaN(normativeAssessmentLevel);
        }

        [Test]
        public void GetNormativeAssessmentLevel_NoCorrespondingCalculation_ReturnsNaN()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub
            {
                FailureMechanismContribution =
                {
                    NormativeProbabilityType = new Random(32).NextEnumValue<NormativeProbabilityType>()
                }
            };

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

            assessmentSection.FailureMechanismContribution.NormativeProbabilityType = new Random(32).NextEnumValue<NormativeProbabilityType>();
            assessmentSection.AddHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            // Call
            RoundedDouble normativeAssessmentLevel = assessmentSection.GetNormativeAssessmentLevel(hydraulicBoundaryLocation);

            // Assert
            Assert.IsNaN(normativeAssessmentLevel);
        }

        [Test]
        [TestCaseSource(nameof(GetNormativeHydraulicBoundaryLocationCalculationPerNormativeProbabilityType))]
        public void GetNormativeAssessmentLevel_HydraulicBoundaryLocationWithOutput_ReturnsCorrespondingAssessmentLevel(
            IAssessmentSection assessmentSection,
            HydraulicBoundaryLocation hydraulicBoundaryLocation,
            NormativeProbabilityType normativeProbabilityType,
            HydraulicBoundaryLocationCalculation calculation)
        {
            // Setup
            assessmentSection.FailureMechanismContribution.NormativeProbabilityType = normativeProbabilityType;

            // Call
            RoundedDouble normativeAssessmentLevel = assessmentSection.GetNormativeAssessmentLevel(hydraulicBoundaryLocation);

            // Assert
            RoundedDouble expectedNormativeAssessmentLevel = calculation.Output.Result;
            Assert.AreEqual(expectedNormativeAssessmentLevel, normativeAssessmentLevel);
        }

        [Test]
        public void GetNormativeHydraulicBoundaryLocationCalculation_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => AssessmentSectionExtensions.GetNormativeHydraulicBoundaryLocationCalculation(null, new TestHydraulicBoundaryLocation());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void GetNormativeHydraulicBoundaryLocationCalculation_AssessmentSectionWithInvalidNormativeProbabilityType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const int invalidValue = 9999;

            var assessmentSection = new AssessmentSectionStub
            {
                FailureMechanismContribution =
                {
                    NormativeProbabilityType = (NormativeProbabilityType) invalidValue
                }
            };

            // Call
            void Call() => assessmentSection.GetNormativeHydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation());

            // Assert
            var expectedMessage = $"The value of argument 'normativeProbabilityType' ({invalidValue}) is invalid for Enum type '{nameof(NormativeProbabilityType)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage).ParamName;
            Assert.AreEqual("normativeProbabilityType", parameterName);
        }

        [Test]
        public void GetNormativeHydraulicBoundaryLocationCalculation_HydraulicBoundaryLocationNull_ReturnsNull()
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSectionStub
            {
                FailureMechanismContribution =
                {
                    NormativeProbabilityType = random.NextEnumValue<NormativeProbabilityType>()
                }
            };

            // Call
            HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation = assessmentSection.GetNormativeHydraulicBoundaryLocationCalculation(null);

            // Assert
            Assert.IsNull(hydraulicBoundaryLocationCalculation);
        }

        [Test]
        public void GetNormativeHydraulicBoundaryLocationCalculation_NoCorrespondingCalculation_ReturnsNull()
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSectionStub
            {
                FailureMechanismContribution =
                {
                    NormativeProbabilityType = random.NextEnumValue<NormativeProbabilityType>()
                }
            };

            // Call
            HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation =
                assessmentSection.GetNormativeHydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation());

            // Assert
            Assert.IsNull(hydraulicBoundaryLocationCalculation);
        }

        [Test]
        [TestCaseSource(nameof(GetNormativeHydraulicBoundaryLocationCalculationPerNormativeProbabilityType))]
        public void GetNormativeHydraulicBoundaryLocationCalculation_HydraulicBoundaryLocation_ReturnsCorrespondingCalculation(
            IAssessmentSection assessmentSection,
            HydraulicBoundaryLocation hydraulicBoundaryLocation,
            NormativeProbabilityType normativeProbabilityType,
            HydraulicBoundaryLocationCalculation calculation)
        {
            // Setup
            assessmentSection.FailureMechanismContribution.NormativeProbabilityType = normativeProbabilityType;

            // Call
            HydraulicBoundaryLocationCalculation normativeHydraulicBoundaryLocationCalculation =
                assessmentSection.GetNormativeHydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation);

            // Assert
            Assert.AreSame(calculation, normativeHydraulicBoundaryLocationCalculation);
        }

        private static IEnumerable<TestCaseData> GetNormativeHydraulicBoundaryLocationCalculationPerNormativeProbabilityType()
        {
            var assessmentSection = new AssessmentSectionStub();
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            assessmentSection.AddHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            }, true);

            yield return new TestCaseData(
                assessmentSection,
                hydraulicBoundaryLocation,
                NormativeProbabilityType.SignalFloodingProbability,
                assessmentSection.WaterLevelCalculationsForSignalFloodingProbability.ElementAt(0));

            yield return new TestCaseData(
                assessmentSection,
                hydraulicBoundaryLocation,
                NormativeProbabilityType.MaximumAllowableFloodingProbability,
                assessmentSection.WaterLevelCalculationsForMaximumAllowableFloodingProbability.ElementAt(0));
        }
    }
}