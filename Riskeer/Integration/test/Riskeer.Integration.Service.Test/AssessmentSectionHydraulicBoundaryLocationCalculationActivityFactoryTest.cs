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
using System.IO;
using System.Linq;
using Core.Common.Base.Service;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.TypeConverters;
using Riskeer.Common.Service;
using Riskeer.Common.Service.TestUtil;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Data.Input.Hydraulics;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;

namespace Riskeer.Integration.Service.Test
{
    [TestFixture]
    public class AssessmentSectionHydraulicBoundaryLocationCalculationActivityFactoryTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Service, "HydraRingCalculation");
        private static readonly string validFilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");
        private static readonly string validPreprocessorDirectory = TestHelper.GetScratchPadPath();
        private static readonly NoProbabilityValueDoubleConverter noProbabilityValueDoubleConverter = new NoProbabilityValueDoubleConverter();

        [Test]
        public void CreateHydraulicBoundaryLocationCalculationActivities_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => AssessmentSectionHydraulicBoundaryLocationCalculationActivityFactory.CreateHydraulicBoundaryLocationCalculationActivities(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
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
            Assert.AreEqual(12, activities.Count());

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;

            double signalingNorm = assessmentSection.FailureMechanismContribution.SignalingNorm;
            string signalingNormText = noProbabilityValueDoubleConverter.ConvertToString(signalingNorm);
            double lowerLimitNorm = assessmentSection.FailureMechanismContribution.LowerLimitNorm;
            string lowerLimitNormText = noProbabilityValueDoubleConverter.ConvertToString(lowerLimitNorm);

            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(0),
                                                      hydraulicBoundaryLocation1,
                                                      signalingNorm,
                                                      signalingNormText,
                                                      hydraulicBoundaryDatabase);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(1),
                                                      hydraulicBoundaryLocation2,
                                                      signalingNorm,
                                                      signalingNormText,
                                                      hydraulicBoundaryDatabase);

            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(2),
                                                      hydraulicBoundaryLocation1,
                                                      lowerLimitNorm,
                                                      lowerLimitNormText,
                                                      hydraulicBoundaryDatabase);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(3),
                                                      hydraulicBoundaryLocation2,
                                                      lowerLimitNorm,
                                                      lowerLimitNormText,
                                                      hydraulicBoundaryDatabase);

            double firstWaterLevelTargetProbability = assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities[0].TargetProbability;
            string firstWaterLevelTargetProbabilityText = noProbabilityValueDoubleConverter.ConvertToString(firstWaterLevelTargetProbability);
            double secondWaterLevelTargetProbability = assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities[1].TargetProbability;
            string secondWaterLevelTargetProbabilityText = noProbabilityValueDoubleConverter.ConvertToString(secondWaterLevelTargetProbability);

            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(4),
                                                      hydraulicBoundaryLocation1,
                                                      firstWaterLevelTargetProbability,
                                                      firstWaterLevelTargetProbabilityText,
                                                      hydraulicBoundaryDatabase);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(5),
                                                      hydraulicBoundaryLocation2,
                                                      firstWaterLevelTargetProbability,
                                                      firstWaterLevelTargetProbabilityText,
                                                      hydraulicBoundaryDatabase);

            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(6),
                                                      hydraulicBoundaryLocation1,
                                                      secondWaterLevelTargetProbability,
                                                      secondWaterLevelTargetProbabilityText,
                                                      hydraulicBoundaryDatabase);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(7),
                                                      hydraulicBoundaryLocation2,
                                                      secondWaterLevelTargetProbability,
                                                      secondWaterLevelTargetProbabilityText,
                                                      hydraulicBoundaryDatabase);

            double firstWaveHeightTargetProbability = assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities[0].TargetProbability;
            string firstWaveHeightTargetProbabilityText = noProbabilityValueDoubleConverter.ConvertToString(firstWaveHeightTargetProbability);
            double secondWaveHeightTargetProbability = assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities[1].TargetProbability;
            string secondWaveHeightTargetProbabilityText = noProbabilityValueDoubleConverter.ConvertToString(secondWaveHeightTargetProbability);

            AssertWaveHeightCalculationActivity(activities.ElementAt(8),
                                                hydraulicBoundaryLocation1,
                                                firstWaveHeightTargetProbability,
                                                firstWaveHeightTargetProbabilityText,
                                                hydraulicBoundaryDatabase);
            AssertWaveHeightCalculationActivity(activities.ElementAt(9),
                                                hydraulicBoundaryLocation2,
                                                firstWaveHeightTargetProbability,
                                                firstWaveHeightTargetProbabilityText,
                                                hydraulicBoundaryDatabase);

            AssertWaveHeightCalculationActivity(activities.ElementAt(10),
                                                hydraulicBoundaryLocation1,
                                                secondWaveHeightTargetProbability,
                                                secondWaveHeightTargetProbabilityText,
                                                hydraulicBoundaryDatabase);
            AssertWaveHeightCalculationActivity(activities.ElementAt(11),
                                                hydraulicBoundaryLocation2,
                                                secondWaveHeightTargetProbability,
                                                secondWaveHeightTargetProbabilityText,
                                                hydraulicBoundaryDatabase);
        }

        [Test]
        public void CreateWaterLevelCalculationActivitiesForNormTargetProbabilities_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => AssessmentSectionHydraulicBoundaryLocationCalculationActivityFactory.CreateWaterLevelCalculationActivitiesForNormTargetProbabilities(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateWaterLevelCalculationActivitiesForNormTargetProbabilities_WithValidDataAndUsePreprocessorStates_ExpectedInputSetToActivities(bool usePreprocessor)
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
                AssessmentSectionHydraulicBoundaryLocationCalculationActivityFactory.CreateWaterLevelCalculationActivitiesForNormTargetProbabilities(assessmentSection);

            // Assert
            Assert.AreEqual(4, activities.Count());

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;

            double signalingNorm = assessmentSection.FailureMechanismContribution.SignalingNorm;
            string signalingNormText = noProbabilityValueDoubleConverter.ConvertToString(signalingNorm);
            double lowerLimitNorm = assessmentSection.FailureMechanismContribution.LowerLimitNorm;
            string lowerLimitNormText = noProbabilityValueDoubleConverter.ConvertToString(lowerLimitNorm);

            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(0),
                                                      hydraulicBoundaryLocation1,
                                                      signalingNorm,
                                                      signalingNormText,
                                                      hydraulicBoundaryDatabase);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(1),
                                                      hydraulicBoundaryLocation2,
                                                      signalingNorm,
                                                      signalingNormText,
                                                      hydraulicBoundaryDatabase);

            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(2),
                                                      hydraulicBoundaryLocation1,
                                                      lowerLimitNorm,
                                                      lowerLimitNormText,
                                                      hydraulicBoundaryDatabase);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(3),
                                                      hydraulicBoundaryLocation2,
                                                      lowerLimitNorm,
                                                      lowerLimitNormText,
                                                      hydraulicBoundaryDatabase);
        }

        [Test]
        public void CreateWaterLevelCalculationActivitiesForUserDefinedTargetProbabilities_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => AssessmentSectionHydraulicBoundaryLocationCalculationActivityFactory.CreateWaterLevelCalculationActivitiesForUserDefinedTargetProbabilities(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateWaterLevelCalculationActivitiesForUserDefinedTargetProbabilities_WithValidDataAndUsePreprocessorStates_ExpectedInputSetToActivities(bool usePreprocessor)
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
                AssessmentSectionHydraulicBoundaryLocationCalculationActivityFactory.CreateWaterLevelCalculationActivitiesForUserDefinedTargetProbabilities(assessmentSection);

            // Assert
            Assert.AreEqual(4, activities.Count());

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;

            double firstTargetProbability = assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities[0].TargetProbability;
            string firstTargetProbabilityText = noProbabilityValueDoubleConverter.ConvertToString(firstTargetProbability);
            double secondTargetProbability = assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities[1].TargetProbability;
            string secondTargetProbabilityText = noProbabilityValueDoubleConverter.ConvertToString(secondTargetProbability);

            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(0),
                                                      hydraulicBoundaryLocation1,
                                                      firstTargetProbability,
                                                      firstTargetProbabilityText,
                                                      hydraulicBoundaryDatabase);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(1),
                                                      hydraulicBoundaryLocation2,
                                                      firstTargetProbability,
                                                      firstTargetProbabilityText,
                                                      hydraulicBoundaryDatabase);

            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(2),
                                                      hydraulicBoundaryLocation1,
                                                      secondTargetProbability,
                                                      secondTargetProbabilityText,
                                                      hydraulicBoundaryDatabase);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(3),
                                                      hydraulicBoundaryLocation2,
                                                      secondTargetProbability,
                                                      secondTargetProbabilityText,
                                                      hydraulicBoundaryDatabase);
        }

        [Test]
        public void CreateWaveHeightCalculationActivitiesForUserDefinedTargetProbabilities_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => AssessmentSectionHydraulicBoundaryLocationCalculationActivityFactory.CreateWaveHeightCalculationActivitiesForUserDefinedTargetProbabilities(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateWaveHeightCalculationActivitiesForUserDefinedTargetProbabilities_WithValidDataAndUsePreprocessorStates_ExpectedInputSetToActivities(bool usePreprocessor)
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
                AssessmentSectionHydraulicBoundaryLocationCalculationActivityFactory.CreateWaveHeightCalculationActivitiesForUserDefinedTargetProbabilities(assessmentSection);

            // Assert
            Assert.AreEqual(4, activities.Count());

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;

            double firstTargetProbability = assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities[0].TargetProbability;
            string firstTargetProbabilityText = noProbabilityValueDoubleConverter.ConvertToString(firstTargetProbability);
            double secondTargetProbability = assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities[1].TargetProbability;
            string secondTargetProbabilityText = noProbabilityValueDoubleConverter.ConvertToString(secondTargetProbability);

            AssertWaveHeightCalculationActivity(activities.ElementAt(0),
                                                hydraulicBoundaryLocation1,
                                                firstTargetProbability,
                                                firstTargetProbabilityText,
                                                hydraulicBoundaryDatabase);
            AssertWaveHeightCalculationActivity(activities.ElementAt(1),
                                                hydraulicBoundaryLocation2,
                                                firstTargetProbability,
                                                firstTargetProbabilityText,
                                                hydraulicBoundaryDatabase);

            AssertWaveHeightCalculationActivity(activities.ElementAt(2),
                                                hydraulicBoundaryLocation1,
                                                secondTargetProbability,
                                                secondTargetProbabilityText,
                                                hydraulicBoundaryDatabase);
            AssertWaveHeightCalculationActivity(activities.ElementAt(3),
                                                hydraulicBoundaryLocation2,
                                                secondTargetProbability,
                                                secondTargetProbabilityText,
                                                hydraulicBoundaryDatabase);
        }

        private static AssessmentSectionStub CreateAssessmentSection(bool usePreprocessor)
        {
            var assessmentSection = new AssessmentSectionStub();

            assessmentSection.HydraulicBoundaryDatabase.FilePath = validFilePath;
            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.CanUsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.UsePreprocessor = usePreprocessor;
            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.PreprocessorDirectory = validPreprocessorDirectory;
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);

            return assessmentSection;
        }

        private static void AssertDesignWaterLevelCalculationActivity(Activity activity,
                                                                      HydraulicBoundaryLocation hydraulicBoundaryLocation,
                                                                      double norm,
                                                                      string calculationIdentifier,
                                                                      HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            var mocks = new MockRepository();
            var designWaterLevelCalculator = new TestDesignWaterLevelCalculator();
            var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(hydraulicBoundaryDatabase),
                                     (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(designWaterLevelCalculator);
            mocks.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                Action call = activity.Run;

                string expectedLogMessage = $"Waterstand berekenen voor locatie '{hydraulicBoundaryLocation.Name}' ({calculationIdentifier}) is gestart.";

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
                                                                string calculationIdentifier,
                                                                HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            var mocks = new MockRepository();
            var waveHeightCalculator = new TestWaveHeightCalculator();
            var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(hydraulicBoundaryDatabase),
                                     (HydraRingCalculationSettings) invocation.Arguments[0]);
                             }).Return(waveHeightCalculator);
            mocks.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                Action call = activity.Run;

                string expectedLogMessage = $"Golfhoogte berekenen voor locatie '{hydraulicBoundaryLocation.Name}' ({calculationIdentifier}) is gestart.";

                TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage);
                WaveHeightCalculationInput actualCalculationInput = waveHeightCalculator.ReceivedInputs.Single();
                Assert.AreEqual(hydraulicBoundaryLocation.Id, actualCalculationInput.HydraulicBoundaryLocationId);
                Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(norm), actualCalculationInput.Beta);
            }

            mocks.VerifyAll();
        }
    }
}