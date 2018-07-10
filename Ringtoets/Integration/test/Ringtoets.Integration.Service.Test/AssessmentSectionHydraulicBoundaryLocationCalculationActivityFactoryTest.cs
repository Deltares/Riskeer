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
using System.IO;
using System.Linq;
using Core.Common.Base.Service;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;

namespace Ringtoets.Integration.Service.Test
{
    [TestFixture]
    public class AssessmentSectionHydraulicBoundaryLocationCalculationActivityFactoryTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");
        private static readonly string validFilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");
        private static readonly string validPreprocessorDirectory = TestHelper.GetScratchPadPath();

        [Test]
        public void CreateHydraulicBoundaryLocationCalculationActivities_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => AssessmentSectionHydraulicBoundaryLocationCalculationActivityFactory.CreateHydraulicBoundaryLocationCalculationActivities(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateHydraulicBoundaryLocationCalculationActivities_WithValidDataAndUsePreprocessorStates_ExpectedInputSetToActivities(bool usePreprocessor)
        {
            // Setup
            AssessmentSectionStub assessmentSection = CreateAssessmentSection(usePreprocessor);

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("locationName 1");
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            // Call
            IEnumerable<CalculatableActivity> activities =
                AssessmentSectionHydraulicBoundaryLocationCalculationActivityFactory.CreateHydraulicBoundaryLocationCalculationActivities(assessmentSection);

            // Assert
            Assert.AreEqual(8, activities.Count());

            double signalingNorm = assessmentSection.FailureMechanismContribution.SignalingNorm;
            double factorizedSignalingNorm = signalingNorm / 30;
            double lowerLimitNorm = assessmentSection.FailureMechanismContribution.LowerLimitNorm;
            double factorizedLowerLimitNorm = lowerLimitNorm * 30;

            AssertDesignWaterLevelCalculationActivity(activities.First(),
                                                      hydraulicBoundaryLocation,
                                                      factorizedSignalingNorm,
                                                      "A+->A",
                                                      usePreprocessor);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(1),
                                                      hydraulicBoundaryLocation,
                                                      signalingNorm,
                                                      "A->B",
                                                      usePreprocessor);

            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(2),
                                                      hydraulicBoundaryLocation,
                                                      lowerLimitNorm,
                                                      "B->C",
                                                      usePreprocessor);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(3),
                                                      hydraulicBoundaryLocation,
                                                      factorizedLowerLimitNorm,
                                                      "C->D",
                                                      usePreprocessor);

            AssertWaveHeightCalculationActivity(activities.ElementAt(4),
                                                hydraulicBoundaryLocation,
                                                factorizedSignalingNorm,
                                                "A+->A",
                                                usePreprocessor);
            AssertWaveHeightCalculationActivity(activities.ElementAt(5),
                                                hydraulicBoundaryLocation,
                                                signalingNorm,
                                                "A->B",
                                                usePreprocessor);

            AssertWaveHeightCalculationActivity(activities.ElementAt(6),
                                                hydraulicBoundaryLocation,
                                                lowerLimitNorm,
                                                "B->C",
                                                usePreprocessor);
            AssertWaveHeightCalculationActivity(activities.ElementAt(7),
                                                hydraulicBoundaryLocation,
                                                factorizedLowerLimitNorm,
                                                "C->D",
                                                usePreprocessor);
        }

        [Test]
        public void CreateDesignWaterLevelCalculationActivities_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => AssessmentSectionHydraulicBoundaryLocationCalculationActivityFactory.CreateDesignWaterLevelCalculationActivities(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateDesignWaterLevelCalculationActivities_WithValidDataAndUsePreprocessorStates_ExpectedInputSetToActivities(bool usePreprocessor)
        {
            // Setup
            AssessmentSectionStub assessmentSection = CreateAssessmentSection(usePreprocessor);

            var hydraulicBoundaryLocation1 = new TestHydraulicBoundaryLocation("locationName 1");
            var hydraulicBoundaryLocation2 = new TestHydraulicBoundaryLocation("locationName 2");
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation1,
                hydraulicBoundaryLocation2
            });

            // Call
            IEnumerable<CalculatableActivity> activities =
                AssessmentSectionHydraulicBoundaryLocationCalculationActivityFactory.CreateDesignWaterLevelCalculationActivities(assessmentSection);

            // Assert
            Assert.AreEqual(8, activities.Count());

            double signalingNorm = assessmentSection.FailureMechanismContribution.SignalingNorm;
            double factorizedSignalingNorm = signalingNorm / 30;
            AssertDesignWaterLevelCalculationActivity(activities.First(),
                                                      hydraulicBoundaryLocation1,
                                                      factorizedSignalingNorm,
                                                      "A+->A",
                                                      usePreprocessor);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(1),
                                                      hydraulicBoundaryLocation2,
                                                      factorizedSignalingNorm,
                                                      "A+->A",
                                                      usePreprocessor);

            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(2),
                                                      hydraulicBoundaryLocation1,
                                                      signalingNorm,
                                                      "A->B",
                                                      usePreprocessor);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(3),
                                                      hydraulicBoundaryLocation2,
                                                      signalingNorm,
                                                      "A->B",
                                                      usePreprocessor);

            double lowerLimitNorm = assessmentSection.FailureMechanismContribution.LowerLimitNorm;
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(4),
                                                      hydraulicBoundaryLocation1,
                                                      lowerLimitNorm,
                                                      "B->C",
                                                      usePreprocessor);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(5),
                                                      hydraulicBoundaryLocation2,
                                                      lowerLimitNorm,
                                                      "B->C",
                                                      usePreprocessor);

            double factorizedLowerLimitNorm = lowerLimitNorm * 30;
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(6),
                                                      hydraulicBoundaryLocation1,
                                                      factorizedLowerLimitNorm,
                                                      "C->D",
                                                      usePreprocessor);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(7),
                                                      hydraulicBoundaryLocation2,
                                                      factorizedLowerLimitNorm,
                                                      "C->D",
                                                      usePreprocessor);
        }

        [Test]
        public void CreateWaveHeightCalculationActivities_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => AssessmentSectionHydraulicBoundaryLocationCalculationActivityFactory.CreateWaveHeightCalculationActivities(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateWaveHeightCalculationActivities_WithValidDataAndUsePreprocessorStates_ExpectedInputSetToActivities(bool usePreprocessor)
        {
            // Setup
            AssessmentSectionStub assessmentSection = CreateAssessmentSection(usePreprocessor);

            var hydraulicBoundaryLocation1 = new TestHydraulicBoundaryLocation("locationName 1");
            var hydraulicBoundaryLocation2 = new TestHydraulicBoundaryLocation("locationName 2");
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation1,
                hydraulicBoundaryLocation2
            });

            // Call
            IEnumerable<CalculatableActivity> activities =
                AssessmentSectionHydraulicBoundaryLocationCalculationActivityFactory.CreateWaveHeightCalculationActivities(assessmentSection);

            // Assert
            Assert.AreEqual(8, activities.Count());

            double signalingNorm = assessmentSection.FailureMechanismContribution.SignalingNorm;
            double factorizedSignalingNorm = signalingNorm / 30;
            AssertWaveHeightCalculationActivity(activities.First(),
                                                hydraulicBoundaryLocation1,
                                                factorizedSignalingNorm,
                                                "A+->A",
                                                usePreprocessor);
            AssertWaveHeightCalculationActivity(activities.ElementAt(1),
                                                hydraulicBoundaryLocation2,
                                                factorizedSignalingNorm,
                                                "A+->A",
                                                usePreprocessor);

            AssertWaveHeightCalculationActivity(activities.ElementAt(2),
                                                hydraulicBoundaryLocation1,
                                                signalingNorm,
                                                "A->B",
                                                usePreprocessor);
            AssertWaveHeightCalculationActivity(activities.ElementAt(3),
                                                hydraulicBoundaryLocation2,
                                                signalingNorm,
                                                "A->B",
                                                usePreprocessor);

            double lowerLimitNorm = assessmentSection.FailureMechanismContribution.LowerLimitNorm;
            AssertWaveHeightCalculationActivity(activities.ElementAt(4),
                                                hydraulicBoundaryLocation1,
                                                lowerLimitNorm,
                                                "B->C",
                                                usePreprocessor);
            AssertWaveHeightCalculationActivity(activities.ElementAt(5),
                                                hydraulicBoundaryLocation2,
                                                lowerLimitNorm,
                                                "B->C",
                                                usePreprocessor);

            double factorizedLowerLimitNorm = lowerLimitNorm * 30;
            AssertWaveHeightCalculationActivity(activities.ElementAt(6),
                                                hydraulicBoundaryLocation1,
                                                factorizedLowerLimitNorm,
                                                "C->D",
                                                usePreprocessor);
            AssertWaveHeightCalculationActivity(activities.ElementAt(7),
                                                hydraulicBoundaryLocation2,
                                                factorizedLowerLimitNorm,
                                                "C->D",
                                                usePreprocessor);
        }

        private static AssessmentSectionStub CreateAssessmentSection(bool usePreprocessor)
        {
            var assessmentSection = new AssessmentSectionStub();

            assessmentSection.HydraulicBoundaryDatabase.CanUsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.UsePreprocessor = usePreprocessor;
            assessmentSection.HydraulicBoundaryDatabase.FilePath = validFilePath;
            assessmentSection.HydraulicBoundaryDatabase.PreprocessorDirectory = validPreprocessorDirectory;

            return assessmentSection;
        }

        private static void AssertDesignWaterLevelCalculationActivity(Activity activity,
                                                                      HydraulicBoundaryLocation hydraulicBoundaryLocation,
                                                                      double norm,
                                                                      string categoryBoundaryName,
                                                                      bool usePreprocessor)
        {
            var mocks = new MockRepository();
            var designWaterLevelCalculator = new TestDesignWaterLevelCalculator();
            var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(testDataPath, usePreprocessor ? validPreprocessorDirectory : "")).Return(designWaterLevelCalculator);
            mocks.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                Action call = activity.Run;

                TestHelper.AssertLogMessages(call, m =>
                {
                    string[] messages = m.ToArray();
                    AssertHydraulicBoundaryLocationCalculationMessages(hydraulicBoundaryLocation,
                                                                       messages,
                                                                       "Waterstand",
                                                                       categoryBoundaryName);
                });

                AssessmentLevelCalculationInput actualCalculationInput = designWaterLevelCalculator.ReceivedInputs.Single();
                Assert.AreEqual(hydraulicBoundaryLocation.Id, actualCalculationInput.HydraulicBoundaryLocationId);
                Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(norm), actualCalculationInput.Beta);
            }

            mocks.VerifyAll();
        }

        private static void AssertWaveHeightCalculationActivity(Activity activity,
                                                                HydraulicBoundaryLocation hydraulicBoundaryLocation,
                                                                double norm,
                                                                string categoryBoundaryName,
                                                                bool usePreprocessor)
        {
            var mocks = new MockRepository();
            var waveHeightCalculator = new TestWaveHeightCalculator();
            var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(testDataPath, usePreprocessor ? validPreprocessorDirectory : "")).Return(waveHeightCalculator);
            mocks.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                Action call = activity.Run;

                TestHelper.AssertLogMessages(call, m =>
                {
                    string[] messages = m.ToArray();
                    AssertHydraulicBoundaryLocationCalculationMessages(hydraulicBoundaryLocation,
                                                                       messages,
                                                                       "Golfhoogte",
                                                                       categoryBoundaryName);
                });

                WaveHeightCalculationInput actualCalculationInput = waveHeightCalculator.ReceivedInputs.Single();
                Assert.AreEqual(hydraulicBoundaryLocation.Id, actualCalculationInput.HydraulicBoundaryLocationId);
                Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(norm), actualCalculationInput.Beta);
            }

            mocks.VerifyAll();
        }

        private static void AssertHydraulicBoundaryLocationCalculationMessages(HydraulicBoundaryLocation hydraulicBoundaryLocation,
                                                                               IEnumerable<string> messages,
                                                                               string calculationTypeName,
                                                                               string categoryBoundaryName)
        {
            Assert.AreEqual(7, messages.Count());
            Assert.AreEqual($"{calculationTypeName} berekenen voor locatie '{hydraulicBoundaryLocation.Name}' (Categorie {categoryBoundaryName}) is gestart.", messages.First());
            CalculationServiceTestHelper.AssertValidationStartMessage(messages.ElementAt(1));
            CalculationServiceTestHelper.AssertValidationEndMessage(messages.ElementAt(2));
            CalculationServiceTestHelper.AssertCalculationStartMessage(messages.ElementAt(3));
            Assert.AreEqual($"{calculationTypeName} berekening voor locatie '{hydraulicBoundaryLocation.Name}' (Categorie {categoryBoundaryName}) is niet geconvergeerd.", messages.ElementAt(4));
            StringAssert.StartsWith($"{calculationTypeName} berekening is uitgevoerd op de tijdelijke locatie", messages.ElementAt(5));
            CalculationServiceTestHelper.AssertCalculationEndMessage(messages.ElementAt(6));
        }
    }
}