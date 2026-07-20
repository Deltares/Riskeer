// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
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
using NSubstitute;
using NUnit.Framework;
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
        private static readonly string validHlcdFilePath = Path.Combine(testDataPath, "hlcd.sqlite");
        private static readonly string validHrdFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
        private static readonly string validHrdFileVersion = "Dutch coast South19-11-2015 12:0013";

        [Test]
        public void CreateWaveHeightCalculationActivities_CalculationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();

            // Call
            void Call() => HydraulicBoundaryLocationCalculationActivityFactory.CreateWaveHeightCalculationActivities(
                null,
                assessmentSection,
                new Random(12).NextDouble(),
                "1/10000");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("calculations", paramName);
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
        public void CreateWaveHeightCalculationActivities_WithValidData_ReturnsExpectedActivity(bool usePreprocessorClosure)
        {
            // Setup
            const string calculationIdentifier = "1/30";
            const double targetProbability = 1.0 / 30;
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();
            var hydraulicBoundaryLocation1 = new TestHydraulicBoundaryLocation("locationName1");
            var hydraulicBoundaryLocation2 = new TestHydraulicBoundaryLocation("locationName2");

            ConfigureAssessmentSection(assessmentSection, usePreprocessorClosure, new[]
            {
                hydraulicBoundaryLocation1,
                hydraulicBoundaryLocation2
            });

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

            HydraulicBoundaryData hydraulicBoundaryData = assessmentSection.HydraulicBoundaryData;
            AssertWaveHeightCalculationActivity(activities.First(), hydraulicBoundaryLocation1, calculationIdentifier, targetProbability, hydraulicBoundaryData);
            AssertWaveHeightCalculationActivity(activities.ElementAt(1), hydraulicBoundaryLocation2, calculationIdentifier, targetProbability, hydraulicBoundaryData);
        }

        [Test]
        public void CreateDesignWaterLevelCalculationActivities_CalculationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();

            // Call
            void Call() => HydraulicBoundaryLocationCalculationActivityFactory.CreateDesignWaterLevelCalculationActivities(
                null,
                assessmentSection,
                new Random(12).NextDouble(),
                "1/10000");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("calculations", paramName);
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
        public void CreateDesignWaterLevelCalculationActivities_WithValidData_ReturnsExpectedActivity(bool usePreprocessorClosure)
        {
            // Setup
            const string calculationIdentifier = "1/30";
            const double targetProbability = 1.0 / 30;
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();
            var hydraulicBoundaryLocation1 = new TestHydraulicBoundaryLocation("locationName1");
            var hydraulicBoundaryLocation2 = new TestHydraulicBoundaryLocation("locationName2");

            ConfigureAssessmentSection(assessmentSection, usePreprocessorClosure, new[]
            {
                hydraulicBoundaryLocation1,
                hydraulicBoundaryLocation2
            });

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

            HydraulicBoundaryData hydraulicBoundaryData = assessmentSection.HydraulicBoundaryData;
            AssertDesignWaterLevelCalculationActivity(activities.First(), hydraulicBoundaryLocation1, calculationIdentifier, targetProbability, hydraulicBoundaryData);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(1), hydraulicBoundaryLocation2, calculationIdentifier, targetProbability, hydraulicBoundaryData);
        }

        private static void AssertWaveHeightCalculationActivity(Activity activity,
                                                                HydraulicBoundaryLocation hydraulicBoundaryLocation,
                                                                string calculationIdentifier,
                                                                double targetProbability,
                                                                HydraulicBoundaryData hydraulicBoundaryData)
        {
            var calculator = new TestWaveHeightCalculator();
            var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();
            calculatorFactory
                .CreateWaveHeightCalculator(Arg.Is<HydraRingCalculationSettings>(x => x != null))
                .Returns(callInfo =>
                {
                    HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                        HydraulicBoundaryCalculationSettingsFactory.CreateSettings(hydraulicBoundaryData,
                                                                                   hydraulicBoundaryLocation),
                        callInfo.Arg<HydraRingCalculationSettings>());
                    return calculator;
                });

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                Action call = activity.Run;

                string expectedLogMessage = $"Golfhoogte berekenen voor locatie '{hydraulicBoundaryLocation.Name}' ({calculationIdentifier}) is gestart.";

                TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage);
                WaveHeightCalculationInput waveHeightCalculationInput = calculator.ReceivedInputs.Single();
                Assert.AreEqual(hydraulicBoundaryLocation.Id, waveHeightCalculationInput.HydraulicBoundaryLocationId);
                Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(targetProbability), waveHeightCalculationInput.Beta);
            }

            calculatorFactory.Received()
                             .CreateWaveHeightCalculator(Arg.Is<HydraRingCalculationSettings>(x => x != null));
        }

        private static void AssertDesignWaterLevelCalculationActivity(Activity activity,
                                                                      HydraulicBoundaryLocation hydraulicBoundaryLocation,
                                                                      string calculationIdentifier,
                                                                      double targetProbability,
                                                                      HydraulicBoundaryData hydraulicBoundaryData)
        {
            var calculator = new TestDesignWaterLevelCalculator();
            var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();
            calculatorFactory
                .CreateDesignWaterLevelCalculator(Arg.Is<HydraRingCalculationSettings>(x => x != null))
                .Returns(callInfo =>
                {
                    HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                        HydraulicBoundaryCalculationSettingsFactory.CreateSettings(hydraulicBoundaryData,
                                                                                   hydraulicBoundaryLocation),
                        callInfo.Arg<HydraRingCalculationSettings>());
                    return calculator;
                });
            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                Action call = activity.Run;

                string expectedLogMessage = $"Waterstand berekenen voor locatie '{hydraulicBoundaryLocation.Name}' ({calculationIdentifier}) is gestart.";

                TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage);
                AssessmentLevelCalculationInput designWaterLevelCalculationInput = calculator.ReceivedInputs.Single();
                Assert.AreEqual(hydraulicBoundaryLocation.Id, designWaterLevelCalculationInput.HydraulicBoundaryLocationId);
                Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(targetProbability), designWaterLevelCalculationInput.Beta);
            }

            calculatorFactory.Received()
                             .CreateDesignWaterLevelCalculator(Arg.Is<HydraRingCalculationSettings>(x => x != null));
        }

        private static void ConfigureAssessmentSection(IAssessmentSection assessmentSection, bool usePreprocessorClosure, IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations)
        {
            HydraulicBoundaryData hydraulicBoundaryData = assessmentSection.HydraulicBoundaryData;

            HydraulicLocationConfigurationDatabase hydraulicLocationConfigurationDatabase = hydraulicBoundaryData.HydraulicLocationConfigurationDatabase;
            hydraulicLocationConfigurationDatabase.FilePath = validHlcdFilePath;

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validHrdFilePath,
                Version = validHrdFileVersion,
                UsePreprocessorClosure = usePreprocessorClosure
            };

            hydraulicBoundaryDatabase.Locations.AddRange(hydraulicBoundaryLocations);

            hydraulicBoundaryData.HydraulicBoundaryDatabases.Add(hydraulicBoundaryDatabase);
        }
    }
}