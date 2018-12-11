// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;

namespace Ringtoets.Integration.Service.Test
{
    [TestFixture]
    public class AssessmentSectionHydraulicBoundaryLocationCalculationActivityFactoryTest
    {
        private const string expectedCategoryBoundaryName1 = "A+";
        private const string expectedCategoryBoundaryName2 = "A";
        private const string expectedCategoryBoundaryName3 = "B";
        private const string expectedCategoryBoundaryName4 = "C";

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

            var hydraulicBoundaryLocation1 = new TestHydraulicBoundaryLocation("locationName 1");
            var hydraulicBoundaryLocation2 = new TestHydraulicBoundaryLocation("locationName 2");
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation1,
                hydraulicBoundaryLocation2
            });

            // Call
            IEnumerable<CalculatableActivity> activities =
                AssessmentSectionHydraulicBoundaryLocationCalculationActivityFactory.CreateHydraulicBoundaryLocationCalculationActivities(assessmentSection);

            // Assert
            Assert.AreEqual(16, activities.Count());

            double signalingNorm = assessmentSection.FailureMechanismContribution.SignalingNorm;
            double factorizedSignalingNorm = signalingNorm / 30;
            double lowerLimitNorm = assessmentSection.FailureMechanismContribution.LowerLimitNorm;
            double factorizedLowerLimitNorm = lowerLimitNorm * 30;

            var hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(0),
                                                      hydraulicBoundaryLocation1,
                                                      factorizedSignalingNorm,
                                                      expectedCategoryBoundaryName1,
                                                      hydraulicBoundaryDatabase);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(1),
                                                      hydraulicBoundaryLocation2,
                                                      factorizedSignalingNorm,
                                                      expectedCategoryBoundaryName1,
                                                      hydraulicBoundaryDatabase);

            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(2),
                                                      hydraulicBoundaryLocation1,
                                                      signalingNorm,
                                                      expectedCategoryBoundaryName2,
                                                      hydraulicBoundaryDatabase);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(3),
                                                      hydraulicBoundaryLocation2,
                                                      signalingNorm,
                                                      expectedCategoryBoundaryName2,
                                                      hydraulicBoundaryDatabase);

            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(4),
                                                      hydraulicBoundaryLocation1,
                                                      lowerLimitNorm,
                                                      expectedCategoryBoundaryName3,
                                                      hydraulicBoundaryDatabase);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(5),
                                                      hydraulicBoundaryLocation2,
                                                      lowerLimitNorm,
                                                      expectedCategoryBoundaryName3,
                                                      hydraulicBoundaryDatabase);

            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(6),
                                                      hydraulicBoundaryLocation1,
                                                      factorizedLowerLimitNorm,
                                                      expectedCategoryBoundaryName4,
                                                      hydraulicBoundaryDatabase);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(7),
                                                      hydraulicBoundaryLocation2,
                                                      factorizedLowerLimitNorm,
                                                      expectedCategoryBoundaryName4,
                                                      hydraulicBoundaryDatabase);

            AssertWaveHeightCalculationActivity(activities.ElementAt(8),
                                                hydraulicBoundaryLocation1,
                                                factorizedSignalingNorm,
                                                expectedCategoryBoundaryName1,
                                                usePreprocessor);
            AssertWaveHeightCalculationActivity(activities.ElementAt(9),
                                                hydraulicBoundaryLocation2,
                                                factorizedSignalingNorm,
                                                expectedCategoryBoundaryName1,
                                                usePreprocessor);

            AssertWaveHeightCalculationActivity(activities.ElementAt(10),
                                                hydraulicBoundaryLocation1,
                                                signalingNorm,
                                                expectedCategoryBoundaryName2,
                                                usePreprocessor);
            AssertWaveHeightCalculationActivity(activities.ElementAt(11),
                                                hydraulicBoundaryLocation2,
                                                signalingNorm,
                                                expectedCategoryBoundaryName2,
                                                usePreprocessor);

            AssertWaveHeightCalculationActivity(activities.ElementAt(12),
                                                hydraulicBoundaryLocation1,
                                                lowerLimitNorm,
                                                expectedCategoryBoundaryName3,
                                                usePreprocessor);
            AssertWaveHeightCalculationActivity(activities.ElementAt(13),
                                                hydraulicBoundaryLocation2,
                                                lowerLimitNorm,
                                                expectedCategoryBoundaryName3,
                                                usePreprocessor);

            AssertWaveHeightCalculationActivity(activities.ElementAt(14),
                                                hydraulicBoundaryLocation1,
                                                factorizedLowerLimitNorm,
                                                expectedCategoryBoundaryName4,
                                                usePreprocessor);
            AssertWaveHeightCalculationActivity(activities.ElementAt(15),
                                                hydraulicBoundaryLocation2,
                                                factorizedLowerLimitNorm,
                                                expectedCategoryBoundaryName4,
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
            double lowerLimitNorm = assessmentSection.FailureMechanismContribution.LowerLimitNorm;
            double factorizedLowerLimitNorm = lowerLimitNorm * 30;

            var hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(0),
                                                      hydraulicBoundaryLocation1,
                                                      factorizedSignalingNorm,
                                                      expectedCategoryBoundaryName1,
                                                      hydraulicBoundaryDatabase);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(1),
                                                      hydraulicBoundaryLocation2,
                                                      factorizedSignalingNorm,
                                                      expectedCategoryBoundaryName1,
                                                      hydraulicBoundaryDatabase);

            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(2),
                                                      hydraulicBoundaryLocation1,
                                                      signalingNorm,
                                                      expectedCategoryBoundaryName2,
                                                      hydraulicBoundaryDatabase);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(3),
                                                      hydraulicBoundaryLocation2,
                                                      signalingNorm,
                                                      expectedCategoryBoundaryName2,
                                                      hydraulicBoundaryDatabase);

            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(4),
                                                      hydraulicBoundaryLocation1,
                                                      lowerLimitNorm,
                                                      expectedCategoryBoundaryName3,
                                                      hydraulicBoundaryDatabase);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(5),
                                                      hydraulicBoundaryLocation2,
                                                      lowerLimitNorm,
                                                      expectedCategoryBoundaryName3,
                                                      hydraulicBoundaryDatabase);

            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(6),
                                                      hydraulicBoundaryLocation1,
                                                      factorizedLowerLimitNorm,
                                                      expectedCategoryBoundaryName4,
                                                      hydraulicBoundaryDatabase);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(7),
                                                      hydraulicBoundaryLocation2,
                                                      factorizedLowerLimitNorm,
                                                      expectedCategoryBoundaryName4,
                                                      hydraulicBoundaryDatabase);
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
            double lowerLimitNorm = assessmentSection.FailureMechanismContribution.LowerLimitNorm;
            double factorizedLowerLimitNorm = lowerLimitNorm * 30;

            AssertWaveHeightCalculationActivity(activities.ElementAt(0),
                                                hydraulicBoundaryLocation1,
                                                factorizedSignalingNorm,
                                                expectedCategoryBoundaryName1,
                                                usePreprocessor);
            AssertWaveHeightCalculationActivity(activities.ElementAt(1),
                                                hydraulicBoundaryLocation2,
                                                factorizedSignalingNorm,
                                                expectedCategoryBoundaryName1,
                                                usePreprocessor);

            AssertWaveHeightCalculationActivity(activities.ElementAt(2),
                                                hydraulicBoundaryLocation1,
                                                signalingNorm,
                                                expectedCategoryBoundaryName2,
                                                usePreprocessor);
            AssertWaveHeightCalculationActivity(activities.ElementAt(3),
                                                hydraulicBoundaryLocation2,
                                                signalingNorm,
                                                expectedCategoryBoundaryName2,
                                                usePreprocessor);

            AssertWaveHeightCalculationActivity(activities.ElementAt(4),
                                                hydraulicBoundaryLocation1,
                                                lowerLimitNorm,
                                                expectedCategoryBoundaryName3,
                                                usePreprocessor);
            AssertWaveHeightCalculationActivity(activities.ElementAt(5),
                                                hydraulicBoundaryLocation2,
                                                lowerLimitNorm,
                                                expectedCategoryBoundaryName3,
                                                usePreprocessor);

            AssertWaveHeightCalculationActivity(activities.ElementAt(6),
                                                hydraulicBoundaryLocation1,
                                                factorizedLowerLimitNorm,
                                                expectedCategoryBoundaryName4,
                                                usePreprocessor);
            AssertWaveHeightCalculationActivity(activities.ElementAt(7),
                                                hydraulicBoundaryLocation2,
                                                factorizedLowerLimitNorm,
                                                expectedCategoryBoundaryName4,
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
                                                                      HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            var mocks = new MockRepository();
            var designWaterLevelCalculator = new TestDesignWaterLevelCalculator();
            var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var hydraRingCalculationSettings = (HydraRingCalculationSettings) invocation.Arguments[0];
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(hydraulicBoundaryDatabase),
                                     hydraRingCalculationSettings);
                             })
                             .Return(designWaterLevelCalculator);
            mocks.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                Action call = activity.Run;

                string expectedLogMessage = $"Waterstand berekenen voor locatie '{hydraulicBoundaryLocation.Name}' (Categoriegrens {categoryBoundaryName}) is gestart.";

                TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage);
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
            string preprocessorDirectory = usePreprocessor ? validPreprocessorDirectory : string.Empty;
            calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings) invocation.Arguments[0];
                                 Assert.AreEqual(validFilePath, settings.HlcdFilePath);
                                 Assert.AreEqual(preprocessorDirectory, settings.PreprocessorDirectory);
                             }).Return(waveHeightCalculator);
            mocks.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                Action call = activity.Run;

                string expectedLogMessage = $"Golfhoogte berekenen voor locatie '{hydraulicBoundaryLocation.Name}' (Categoriegrens {categoryBoundaryName}) is gestart.";

                TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage);
                WaveHeightCalculationInput actualCalculationInput = waveHeightCalculator.ReceivedInputs.Single();
                Assert.AreEqual(hydraulicBoundaryLocation.Id, actualCalculationInput.HydraulicBoundaryLocationId);
                Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(norm), actualCalculationInput.Beta);
            }

            mocks.VerifyAll();
        }
    }
}