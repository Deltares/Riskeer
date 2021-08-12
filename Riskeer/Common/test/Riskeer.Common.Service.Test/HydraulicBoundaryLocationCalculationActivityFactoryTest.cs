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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Service.TestUtil;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Data.Input.Hydraulics;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;

namespace Riskeer.Common.Service.Test
{
    [TestFixture]
    public class HydraulicBoundaryLocationCalculationActivityFactoryTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Service, "HydraRingCalculation");
        private static readonly string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
        private static readonly string validPreprocessorDirectory = TestHelper.GetScratchPadPath();

        [Test]
        public void CreateWaveHeightCalculationActivities_CalculationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => HydraulicBoundaryLocationCalculationActivityFactory.CreateWaveHeightCalculationActivities(
                null,
                assessmentSection,
                new Random(12).NextDouble(),
                "1/10000");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("calculations", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateWaveHeightCalculationActivities_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => HydraulicBoundaryLocationCalculationActivityFactory.CreateWaveHeightCalculationActivities(
                Enumerable.Empty<HydraulicBoundaryLocationCalculation>(),
                null,
                new Random(12).NextDouble(),
                "1/10000");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("assessmentSection", paramName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateWaveHeightCalculationActivities_WithValidDataAndUsePreprocessorStates_ReturnsExpectedActivity(bool usePreprocessor)
        {
            // Setup
            const string calculationIdentifier = "1/30";
            const double targetProbability = 1.0 / 30;

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);
            mocks.ReplayAll();

            ConfigureAssessmentSection(assessmentSection, usePreprocessor);

            var hydraulicBoundaryLocation1 = new TestHydraulicBoundaryLocation("locationName1");
            var hydraulicBoundaryLocation2 = new TestHydraulicBoundaryLocation("locationName2");

            // Call
            IEnumerable<CalculatableActivity> activities = HydraulicBoundaryLocationCalculationActivityFactory.CreateWaveHeightCalculationActivities(
                new[]
                {
                    new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation1),
                    new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation2)
                },
                assessmentSection,
                targetProbability,
                calculationIdentifier);

            // Assert
            Assert.AreEqual(2, activities.Count());
            CollectionAssert.AllItemsAreInstancesOfType(activities, typeof(WaveHeightCalculationActivity));

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;
            AssertWaveHeightCalculationActivity(activities.First(), hydraulicBoundaryLocation1, calculationIdentifier, targetProbability, hydraulicBoundaryDatabase);
            AssertWaveHeightCalculationActivity(activities.ElementAt(1), hydraulicBoundaryLocation2, calculationIdentifier, targetProbability, hydraulicBoundaryDatabase);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateDesignWaterLevelCalculationActivities_CalculationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => HydraulicBoundaryLocationCalculationActivityFactory.CreateDesignWaterLevelCalculationActivities(
                null,
                assessmentSection,
                new Random(12).NextDouble(),
                "1/10000");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("calculations", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateDesignWaterLevelCalculationActivities_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => HydraulicBoundaryLocationCalculationActivityFactory.CreateDesignWaterLevelCalculationActivities(
                Enumerable.Empty<HydraulicBoundaryLocationCalculation>(),
                null,
                new Random(12).NextDouble(),
                "1/10000");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("assessmentSection", paramName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateDesignWaterLevelCalculationActivities_WithValidDataAndUsePreprocessorStates_ReturnsExpectedActivity(bool usePreprocessor)
        {
            // Setup
            const string calculationIdentifier = "1/30";
            const double targetProbability = 1.0 / 30;

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);
            mocks.ReplayAll();

            ConfigureAssessmentSection(assessmentSection, usePreprocessor);

            var hydraulicBoundaryLocation1 = new TestHydraulicBoundaryLocation("locationName1");
            var hydraulicBoundaryLocation2 = new TestHydraulicBoundaryLocation("locationName2");

            // Call
            IEnumerable<CalculatableActivity> activities = HydraulicBoundaryLocationCalculationActivityFactory.CreateDesignWaterLevelCalculationActivities(
                new[]
                {
                    new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation1),
                    new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation2)
                },
                assessmentSection,
                targetProbability,
                calculationIdentifier);

            // Assert
            Assert.AreEqual(2, activities.Count());
            CollectionAssert.AllItemsAreInstancesOfType(activities, typeof(DesignWaterLevelCalculationActivity));

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;
            AssertDesignWaterLevelCalculationActivity(activities.First(), hydraulicBoundaryLocation1, calculationIdentifier, targetProbability, hydraulicBoundaryDatabase);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(1), hydraulicBoundaryLocation2, calculationIdentifier, targetProbability, hydraulicBoundaryDatabase);

            mocks.VerifyAll();
        }

        private static void AssertWaveHeightCalculationActivity(Activity activity,
                                                                HydraulicBoundaryLocation hydraulicBoundaryLocation,
                                                                string calculationIdentifier,
                                                                double targetProbability,
                                                                HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            var mocks = new MockRepository();
            var calculator = new TestWaveHeightCalculator();
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(hydraulicBoundaryDatabase),
                                     (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(calculator);
            mocks.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                Action call = activity.Run;

                string expectedLogMessage = $"Golfhoogte berekenen voor locatie '{hydraulicBoundaryLocation.Name}' ({calculationIdentifier}) is gestart.";

                TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage);
                WaveHeightCalculationInput waveHeightCalculationInput = calculator.ReceivedInputs.Single();
                Assert.AreEqual(hydraulicBoundaryLocation.Id, waveHeightCalculationInput.HydraulicBoundaryLocationId);
                Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(targetProbability), waveHeightCalculationInput.Beta);
            }

            mocks.VerifyAll();
        }

        private static void AssertDesignWaterLevelCalculationActivity(Activity activity,
                                                                      HydraulicBoundaryLocation hydraulicBoundaryLocation,
                                                                      string calculationIdentifier,
                                                                      double targetProbability,
                                                                      HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            var mocks = new MockRepository();
            var calculator = new TestDesignWaterLevelCalculator();
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(hydraulicBoundaryDatabase),
                                     (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(calculator);
            mocks.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                Action call = activity.Run;

                string expectedLogMessage = $"Waterstand berekenen voor locatie '{hydraulicBoundaryLocation.Name}' ({calculationIdentifier}) is gestart.";

                TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage);
                AssessmentLevelCalculationInput designWaterLevelCalculationInput = calculator.ReceivedInputs.Single();
                Assert.AreEqual(hydraulicBoundaryLocation.Id, designWaterLevelCalculationInput.HydraulicBoundaryLocationId);
                Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(targetProbability), designWaterLevelCalculationInput.Beta);
            }

            mocks.VerifyAll();
        }

        private static void ConfigureAssessmentSection(IAssessmentSection assessmentSection, bool usePreprocessor)
        {
            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.CanUsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.UsePreprocessor = usePreprocessor;
            assessmentSection.HydraulicBoundaryDatabase.FilePath = validFilePath;
            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.PreprocessorDirectory = validPreprocessorDirectory;
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);
        }
    }
}