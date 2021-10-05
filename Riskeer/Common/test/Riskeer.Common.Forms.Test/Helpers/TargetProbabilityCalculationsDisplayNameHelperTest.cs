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
        public void GetUniqueDisplayNameForWaterLevelCalculations_CalculationsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => TargetProbabilityCalculationsDisplayNameHelper.GetUniqueDisplayNameForWaterLevelCalculations(
                null, new AssessmentSectionStub());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculations", exception.ParamName);
        }

        [Test]
        public void GetUniqueDisplayNameForWaterLevelCalculations_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => TargetProbabilityCalculationsDisplayNameHelper.GetUniqueDisplayNameForWaterLevelCalculations(
                Enumerable.Empty<HydraulicBoundaryLocationCalculation>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void GetUniqueDisplayNameForWaterLevelCalculations_CalculationsNotInWaterLevelCalculationsOfAssessmentSection_ThrowsInvalidOperationException()
        {
            // Call
            void Call() => TargetProbabilityCalculationsDisplayNameHelper.GetUniqueDisplayNameForWaterLevelCalculations(
                Enumerable.Empty<HydraulicBoundaryLocationCalculation>(), new AssessmentSectionStub());

            // Assert
            var exception = Assert.Throws<InvalidOperationException>(Call);
            Assert.AreEqual("The provided calculations object is not part of the water level calculations within the assessment section.", exception.Message);
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
            string name = TargetProbabilityCalculationsDisplayNameHelper.GetUniqueDisplayNameForWaterLevelCalculations(hydraulicBoundaryLocationCalculations,
                                                                                                                       assessmentSection);

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
            string name = TargetProbabilityCalculationsDisplayNameHelper.GetUniqueDisplayNameForWaterLevelCalculations(hydraulicBoundaryLocationCalculations,
                                                                                                                       assessmentSection);

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
            string name = TargetProbabilityCalculationsDisplayNameHelper.GetUniqueDisplayNameForWaterLevelCalculations(calculationsForTargetProbability.HydraulicBoundaryLocationCalculations,
                                                                                                                       assessmentSection);

            // Assert
            Assert.AreEqual(expectedName, name);

            mockRepository.VerifyAll();
        }

        [Test]
        public void GetUniqueDisplayNameForCalculations_CalculationsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => TargetProbabilityCalculationsDisplayNameHelper.GetUniqueDisplayNameForCalculations(
                null, Enumerable.Empty<object>(), calculation => double.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculations", exception.ParamName);
        }

        [Test]
        public void GetUniqueDisplayNameForCalculations_AllCalculationsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => TargetProbabilityCalculationsDisplayNameHelper.GetUniqueDisplayNameForCalculations(
                new object(), null, calculation => double.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("allCalculations", exception.ParamName);
        }

        [Test]
        public void GetUniqueDisplayNameForCalculations_GetTargetProbabilityFuncNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => TargetProbabilityCalculationsDisplayNameHelper.GetUniqueDisplayNameForCalculations(
                new object(), Enumerable.Empty<object>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("getTargetProbabilityFunc", exception.ParamName);
        }

        [Test]
        public void GetUniqueDisplayNameForCalculations_CalculationsNotInEnumerationOfAllCalculations_ThrowsInvalidOperationException()
        {
            // Call
            void Call() => TargetProbabilityCalculationsDisplayNameHelper.GetUniqueDisplayNameForCalculations(
                new object(), Enumerable.Empty<object>(), o => double.NaN);

            // Assert
            var exception = Assert.Throws<InvalidOperationException>(Call);
            Assert.AreEqual("The provided calculations object is not part of the enumeration that contains all calculations.", exception.Message);
        }

        [Test]
        public void GetUniqueDisplayNameForCalculations_ValidParameters_ReturnsExpectedDisplayName()
        {
            // Setup
            var calculation = new object();
            var allCalculations = new ObservableList<object>();
            int randomNumberOfCalculationElements = new Random().Next() % 10;

            for (var i = 0; i < randomNumberOfCalculationElements; i++)
            {
                allCalculations.Add(new object());
            }

            allCalculations.Add(calculation);

            // Call
            string name = TargetProbabilityCalculationsDisplayNameHelper.GetUniqueDisplayNameForCalculations(calculation, allCalculations, o => 0.01);

            // Assert
            var expectedName = "1/100";

            if (randomNumberOfCalculationElements != 0)
            {
                expectedName += $" ({randomNumberOfCalculationElements})";
            }

            Assert.AreEqual(expectedName, name);
        }
    }
}