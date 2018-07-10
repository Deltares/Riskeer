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
using Core.Common.Util.Extensions;
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
        public void CreateHydraulicBoundaryLocationCalculationActivities_WithValidData_ExpectedInputSetToActivities()
        {
            // Setup
            AssessmentSectionStub assessmentSection = CreateAssessmentSectionStub();

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
                                                      factorizedSignalingNorm);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(1),
                                                      hydraulicBoundaryLocation,
                                                      signalingNorm);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(2),
                                                      hydraulicBoundaryLocation,
                                                      lowerLimitNorm);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(3),
                                                      hydraulicBoundaryLocation,
                                                      factorizedLowerLimitNorm);

            AssertWaveHeightCalculationActivity(activities.ElementAt(4),
                                                hydraulicBoundaryLocation,
                                                factorizedSignalingNorm);
            AssertWaveHeightCalculationActivity(activities.ElementAt(5),
                                                hydraulicBoundaryLocation,
                                                signalingNorm);
            AssertWaveHeightCalculationActivity(activities.ElementAt(6),
                                                hydraulicBoundaryLocation,
                                                lowerLimitNorm);
            AssertWaveHeightCalculationActivity(activities.ElementAt(7),
                                                hydraulicBoundaryLocation,
                                                factorizedLowerLimitNorm);
        }

        [Test]
        public void GivenCreatedHydraulicBoundaryLocationCalculationActivities_WhenActivitiesRan_ThenLogMessagesShown()
        {
            // Given
            var mocks = new MockRepository();
            var designWaterLevelCalculator = new TestDesignWaterLevelCalculator
            {
                Converged = false
            };
            var waveHeightCalculator = new TestWaveHeightCalculator
            {
                Converged = false
            };
            var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(testDataPath, validPreprocessorDirectory)).Return(designWaterLevelCalculator).Repeat.Times(4);
            calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(testDataPath, validPreprocessorDirectory)).Return(waveHeightCalculator).Repeat.Times(4);
            mocks.ReplayAll();

            AssessmentSectionStub assessmentSection = CreateAssessmentSectionStub();

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("locationName 1");
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            IEnumerable<CalculatableActivity> activities =
                AssessmentSectionHydraulicBoundaryLocationCalculationActivityFactory.CreateHydraulicBoundaryLocationCalculationActivities(assessmentSection);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // When
                Action call = () => activities.ForEachElementDo(activity => activity.Run());

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(56, msgs.Length);
                    const string designWaterLevelDisplayName = "Waterstand";
                    AssertHydraulicBoundaryLocationCalculationMessages(hydraulicBoundaryLocation, msgs, 0, designWaterLevelDisplayName, "A+->A");
                    AssertHydraulicBoundaryLocationCalculationMessages(hydraulicBoundaryLocation, msgs, 7, designWaterLevelDisplayName, "A->B");
                    AssertHydraulicBoundaryLocationCalculationMessages(hydraulicBoundaryLocation, msgs, 14, designWaterLevelDisplayName, "B->C");
                    AssertHydraulicBoundaryLocationCalculationMessages(hydraulicBoundaryLocation, msgs, 21, designWaterLevelDisplayName, "C->D");

                    const string waveHeightDisplayName = "Golfhoogte";
                    AssertHydraulicBoundaryLocationCalculationMessages(hydraulicBoundaryLocation, msgs, 28, waveHeightDisplayName, "A+->A");
                    AssertHydraulicBoundaryLocationCalculationMessages(hydraulicBoundaryLocation, msgs, 35, waveHeightDisplayName, "A->B");
                    AssertHydraulicBoundaryLocationCalculationMessages(hydraulicBoundaryLocation, msgs, 42, waveHeightDisplayName, "B->C");
                    AssertHydraulicBoundaryLocationCalculationMessages(hydraulicBoundaryLocation, msgs, 49, waveHeightDisplayName, "C->D");
                });
            }

            mocks.VerifyAll();
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
        public void CreateDesignWaterLevelCalculationActivities_WithValidData_ExpectedInputSetToActivities()
        {
            // Setup
            AssessmentSectionStub assessmentSection = CreateAssessmentSectionStub();

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
                                                      factorizedSignalingNorm);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(1),
                                                      hydraulicBoundaryLocation2,
                                                      factorizedSignalingNorm);

            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(2),
                                                      hydraulicBoundaryLocation1,
                                                      signalingNorm);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(3),
                                                      hydraulicBoundaryLocation2,
                                                      signalingNorm);

            double lowerLimitNorm = assessmentSection.FailureMechanismContribution.LowerLimitNorm;
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(4),
                                                      hydraulicBoundaryLocation1,
                                                      lowerLimitNorm);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(5),
                                                      hydraulicBoundaryLocation2,
                                                      lowerLimitNorm);

            double factorizedLowerLimitNorm = lowerLimitNorm * 30;
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(6),
                                                      hydraulicBoundaryLocation1,
                                                      factorizedLowerLimitNorm);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(7),
                                                      hydraulicBoundaryLocation2,
                                                      factorizedLowerLimitNorm);
        }

        [Test]
        public void GivenCreatedDesignWaterLevelCalculationActivities_WhenActivitiesRan_ThenLogMessagesShown()
        {
            // Given
            var mocks = new MockRepository();
            var designWaterLevelCalculator = new TestDesignWaterLevelCalculator
            {
                Converged = false
            };
            var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(testDataPath, validPreprocessorDirectory)).Return(designWaterLevelCalculator).Repeat.Times(4);
            mocks.ReplayAll();

            AssessmentSectionStub assessmentSection = CreateAssessmentSectionStub();

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("locationName 1");
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            IEnumerable<CalculatableActivity> activities =
                AssessmentSectionHydraulicBoundaryLocationCalculationActivityFactory.CreateDesignWaterLevelCalculationActivities(assessmentSection);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // When
                Action call = () => activities.ForEachElementDo(activity => activity.Run());

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(28, msgs.Length);
                    const string designWaterLevelDisplayName = "Waterstand";
                    AssertHydraulicBoundaryLocationCalculationMessages(hydraulicBoundaryLocation, msgs, 0, designWaterLevelDisplayName, "A+->A");
                    AssertHydraulicBoundaryLocationCalculationMessages(hydraulicBoundaryLocation, msgs, 7, designWaterLevelDisplayName, "A->B");
                    AssertHydraulicBoundaryLocationCalculationMessages(hydraulicBoundaryLocation, msgs, 14, designWaterLevelDisplayName, "B->C");
                    AssertHydraulicBoundaryLocationCalculationMessages(hydraulicBoundaryLocation, msgs, 21, designWaterLevelDisplayName, "C->D");
                });
            }

            mocks.VerifyAll();
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
        public void CreateWaveHeightCalculationActivities_WithValidData_ExpectedInputSetToActivities()
        {
            // Setup
            AssessmentSectionStub assessmentSection = CreateAssessmentSectionStub();

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
                                                factorizedSignalingNorm);
            AssertWaveHeightCalculationActivity(activities.ElementAt(1),
                                                hydraulicBoundaryLocation2,
                                                factorizedSignalingNorm);

            AssertWaveHeightCalculationActivity(activities.ElementAt(2),
                                                hydraulicBoundaryLocation1,
                                                signalingNorm);
            AssertWaveHeightCalculationActivity(activities.ElementAt(3),
                                                hydraulicBoundaryLocation2,
                                                signalingNorm);

            double lowerLimitNorm = assessmentSection.FailureMechanismContribution.LowerLimitNorm;
            AssertWaveHeightCalculationActivity(activities.ElementAt(4),
                                                hydraulicBoundaryLocation1,
                                                lowerLimitNorm);
            AssertWaveHeightCalculationActivity(activities.ElementAt(5),
                                                hydraulicBoundaryLocation2,
                                                lowerLimitNorm);

            double factorizedLowerLimitNorm = lowerLimitNorm * 30;
            AssertWaveHeightCalculationActivity(activities.ElementAt(6),
                                                hydraulicBoundaryLocation1,
                                                factorizedLowerLimitNorm);
            AssertWaveHeightCalculationActivity(activities.ElementAt(7),
                                                hydraulicBoundaryLocation2,
                                                factorizedLowerLimitNorm);
        }

        [Test]
        public void GivenCreatedWaveHeightCalculationActivities_WhenActivitiesRan_ThenLogMessagesShown()
        {
            // Given
            var mocks = new MockRepository();
            var waveHeightCalculator = new TestWaveHeightCalculator
            {
                Converged = false
            };
            var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(testDataPath, validPreprocessorDirectory)).Return(waveHeightCalculator).Repeat.Times(4);
            mocks.ReplayAll();

            AssessmentSectionStub assessmentSection = CreateAssessmentSectionStub();

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("locationName 1");
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            IEnumerable<CalculatableActivity> activities =
                AssessmentSectionHydraulicBoundaryLocationCalculationActivityFactory.CreateWaveHeightCalculationActivities(assessmentSection);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // When
                Action call = () => activities.ForEachElementDo(activity => activity.Run());

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(28, msgs.Length);
                    const string waveHeightDisplayName = "Golfhoogte";
                    AssertHydraulicBoundaryLocationCalculationMessages(hydraulicBoundaryLocation, msgs, 0, waveHeightDisplayName, "A+->A");
                    AssertHydraulicBoundaryLocationCalculationMessages(hydraulicBoundaryLocation, msgs, 7, waveHeightDisplayName, "A->B");
                    AssertHydraulicBoundaryLocationCalculationMessages(hydraulicBoundaryLocation, msgs, 14, waveHeightDisplayName, "B->C");
                    AssertHydraulicBoundaryLocationCalculationMessages(hydraulicBoundaryLocation, msgs, 21, waveHeightDisplayName, "C->D");
                });
            }

            mocks.VerifyAll();
        }

        private static void AssertHydraulicBoundaryLocationCalculationMessages(HydraulicBoundaryLocation hydraulicBoundaryLocation,
                                                                               IEnumerable<string> messages,
                                                                               int startIndex,
                                                                               string calculationTypeName,
                                                                               string categoryName)
        {
            Assert.AreEqual($"{calculationTypeName} berekenen voor locatie '{hydraulicBoundaryLocation.Name}' (Categorie {categoryName}) is gestart.", messages.ElementAt(startIndex));
            CalculationServiceTestHelper.AssertValidationStartMessage(messages.ElementAt(startIndex + 1));
            CalculationServiceTestHelper.AssertValidationEndMessage(messages.ElementAt(startIndex + 2));
            CalculationServiceTestHelper.AssertCalculationStartMessage(messages.ElementAt(startIndex + 3));
            Assert.AreEqual($"{calculationTypeName} berekening voor locatie '{hydraulicBoundaryLocation.Name}' (Categorie {categoryName}) is niet geconvergeerd.", messages.ElementAt(startIndex + 4));
            StringAssert.StartsWith($"{calculationTypeName} berekening is uitgevoerd op de tijdelijke locatie", messages.ElementAt(startIndex + 5));
            CalculationServiceTestHelper.AssertCalculationEndMessage(messages.ElementAt(startIndex + 6));
        }

        private static AssessmentSectionStub CreateAssessmentSectionStub()
        {
            var assessmentSection = new AssessmentSectionStub
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = validFilePath,
                    CanUsePreprocessor = true,
                    PreprocessorDirectory = validPreprocessorDirectory,
                    UsePreprocessor = true
                }
            };
            return assessmentSection;
        }

        private static void AssertDesignWaterLevelCalculationActivity(Activity activity,
                                                                      HydraulicBoundaryLocation hydraulicBoundaryLocation,
                                                                      double norm)
        {
            var mocks = new MockRepository();
            var designWaterLevelCalculator = new TestDesignWaterLevelCalculator();
            var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(testDataPath, validPreprocessorDirectory)).Return(designWaterLevelCalculator);
            mocks.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                activity.Run();

                AssessmentLevelCalculationInput actualCalculationInput = designWaterLevelCalculator.ReceivedInputs.Single();
                Assert.AreEqual(hydraulicBoundaryLocation.Id, actualCalculationInput.HydraulicBoundaryLocationId);
                Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(norm), actualCalculationInput.Beta);
            }

            mocks.VerifyAll();
        }

        private static void AssertWaveHeightCalculationActivity(Activity activity,
                                                                HydraulicBoundaryLocation hydraulicBoundaryLocation,
                                                                double norm)
        {
            var mocks = new MockRepository();
            var waveHeightCalculator = new TestWaveHeightCalculator();
            var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(testDataPath, validPreprocessorDirectory)).Return(waveHeightCalculator);
            mocks.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                activity.Run();

                WaveHeightCalculationInput actualCalculationInput = waveHeightCalculator.ReceivedInputs.Single();
                Assert.AreEqual(hydraulicBoundaryLocation.Id, actualCalculationInput.HydraulicBoundaryLocationId);
                Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(norm), actualCalculationInput.Beta);
            }

            mocks.VerifyAll();
        }
    }
}