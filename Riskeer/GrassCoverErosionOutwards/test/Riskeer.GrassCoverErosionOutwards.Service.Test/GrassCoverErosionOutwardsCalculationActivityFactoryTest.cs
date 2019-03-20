// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Base.Service;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Service;
using Riskeer.Common.Service.TestUtil;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.GrassCoverErosionOutwards.Data.TestUtil;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Data.Input.Hydraulics;
using Riskeer.HydraRing.Calculation.Data.Input.WaveConditions;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;
using Riskeer.Revetment.Data;

namespace Riskeer.GrassCoverErosionOutwards.Service.Test
{
    [TestFixture]
    public class GrassCoverErosionOutwardsCalculationActivityFactoryTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Service, "HydraRingCalculation");
        private static readonly string validFilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");

        [Test]
        public void CreateCalculationActivitiesForFailureMechanism_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => GrassCoverErosionOutwardsCalculationActivityFactory.CreateCalculationActivities(null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateCalculationActivitiesForFailureMechanism_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => GrassCoverErosionOutwardsCalculationActivityFactory.CreateCalculationActivities(new GrassCoverErosionOutwardsFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateCalculationActivitiesForFailureMechanism_WithValidData_ExpectedInputSetToActivities()
        {
            // Setup
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();
            GrassCoverErosionOutwardsFailureMechanism failureMechanism = CreateFailureMechanism();

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("locationName 1");
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(failureMechanism, assessmentSection, new[]
            {
                hydraulicBoundaryLocation
            });

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation1 = CreateValidCalculation(hydraulicBoundaryLocation);
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation2 = CreateValidCalculation(hydraulicBoundaryLocation);

            failureMechanism.WaveConditionsCalculationGroup.Children.AddRange(new[]
            {
                calculation1,
                calculation2
            });

            // Call
            IEnumerable<CalculatableActivity> activities =
                GrassCoverErosionOutwardsCalculationActivityFactory.CreateCalculationActivities(
                    failureMechanism,
                    assessmentSection);

            // Assert
            Assert.AreEqual(12, activities.Count());

            double mechanismSpecificFactorizedSignalingNorm = GetExpectedNorm(failureMechanism,
                                                                              assessmentSection.FailureMechanismContribution.SignalingNorm / 30);
            double mechanismSpecificSignalingNorm = GetExpectedNorm(failureMechanism, assessmentSection.FailureMechanismContribution.SignalingNorm);
            double mechanismSpecificLowerLimitNorm = GetExpectedNorm(failureMechanism, assessmentSection.FailureMechanismContribution.LowerLimitNorm);
            double lowerLimitNorm = assessmentSection.FailureMechanismContribution.LowerLimitNorm;
            double factorizedLowerLimitNorm = lowerLimitNorm * 30;

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;
            AssertDesignWaterLevelCalculationActivity(activities.First(),
                                                      hydraulicBoundaryLocation,
                                                      mechanismSpecificFactorizedSignalingNorm,
                                                      "Iv",
                                                      hydraulicBoundaryDatabase);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(1),
                                                      hydraulicBoundaryLocation,
                                                      mechanismSpecificSignalingNorm,
                                                      "IIv",
                                                      hydraulicBoundaryDatabase);

            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(2),
                                                      hydraulicBoundaryLocation,
                                                      mechanismSpecificLowerLimitNorm,
                                                      "IIIv",
                                                      hydraulicBoundaryDatabase);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(3),
                                                      hydraulicBoundaryLocation,
                                                      lowerLimitNorm,
                                                      "IVv",
                                                      hydraulicBoundaryDatabase);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(4),
                                                      hydraulicBoundaryLocation,
                                                      factorizedLowerLimitNorm,
                                                      "Vv",
                                                      hydraulicBoundaryDatabase);

            AssertWaveHeightCalculationActivity(activities.ElementAt(5),
                                                hydraulicBoundaryLocation,
                                                mechanismSpecificFactorizedSignalingNorm,
                                                "Iv",
                                                hydraulicBoundaryDatabase);
            AssertWaveHeightCalculationActivity(activities.ElementAt(6),
                                                hydraulicBoundaryLocation,
                                                mechanismSpecificSignalingNorm,
                                                "IIv",
                                                hydraulicBoundaryDatabase);

            AssertWaveHeightCalculationActivity(activities.ElementAt(7),
                                                hydraulicBoundaryLocation,
                                                mechanismSpecificLowerLimitNorm,
                                                "IIIv",
                                                hydraulicBoundaryDatabase);
            AssertWaveHeightCalculationActivity(activities.ElementAt(8),
                                                hydraulicBoundaryLocation,
                                                lowerLimitNorm,
                                                "IVv",
                                                hydraulicBoundaryDatabase);
            AssertWaveHeightCalculationActivity(activities.ElementAt(9),
                                                hydraulicBoundaryLocation,
                                                factorizedLowerLimitNorm,
                                                "Vv",
                                                hydraulicBoundaryDatabase);

            var hydraulicBoundaryLocationCalculationOutput = new TestHydraulicBoundaryLocationCalculationOutput(2.0);
            failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm.Single().Output = hydraulicBoundaryLocationCalculationOutput;

            AssertGrassCoverErosionOutwardsWaveConditionsCalculationActivity(activities.ElementAt(10),
                                                                             calculation1,
                                                                             hydraulicBoundaryLocationCalculationOutput.Result,
                                                                             hydraulicBoundaryDatabase);
            AssertGrassCoverErosionOutwardsWaveConditionsCalculationActivity(activities.ElementAt(11),
                                                                             calculation2,
                                                                             hydraulicBoundaryLocationCalculationOutput.Result,
                                                                             hydraulicBoundaryDatabase);
        }

        [Test]
        public void CreateCalculationActivitiesWithoutAssessmentSectionCalculations_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () =>
                GrassCoverErosionOutwardsCalculationActivityFactory.CreateCalculationActivitiesWithoutAssessmentSectionCalculations(null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateCalculationActivitiesWithoutAssessmentSectionCalculations_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () =>
                GrassCoverErosionOutwardsCalculationActivityFactory.CreateCalculationActivitiesWithoutAssessmentSectionCalculations(new GrassCoverErosionOutwardsFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateCalculationActivitiesWithoutAssessmentSectionCalculations_WithValidData_ExpectedInputSetToActivities()
        {
            // Setup
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();
            GrassCoverErosionOutwardsFailureMechanism failureMechanism = CreateFailureMechanism();

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("locationName 1");
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(failureMechanism, assessmentSection, new[]
            {
                hydraulicBoundaryLocation
            });

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation1 = CreateValidCalculation(hydraulicBoundaryLocation);
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation2 = CreateValidCalculation(hydraulicBoundaryLocation);

            failureMechanism.WaveConditionsCalculationGroup.Children.AddRange(new[]
            {
                calculation1,
                calculation2
            });

            // Call
            IEnumerable<CalculatableActivity> activities =
                GrassCoverErosionOutwardsCalculationActivityFactory.CreateCalculationActivitiesWithoutAssessmentSectionCalculations(
                    failureMechanism,
                    assessmentSection);

            // Assert
            Assert.AreEqual(8, activities.Count());

            double mechanismSpecificFactorizedSignalingNorm = GetExpectedNorm(failureMechanism,
                                                                              assessmentSection.FailureMechanismContribution.SignalingNorm / 30);
            double mechanismSpecificSignalingNorm = GetExpectedNorm(failureMechanism, assessmentSection.FailureMechanismContribution.SignalingNorm);
            double mechanismSpecificLowerLimitNorm = GetExpectedNorm(failureMechanism, assessmentSection.FailureMechanismContribution.LowerLimitNorm);

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;
            AssertDesignWaterLevelCalculationActivity(activities.First(),
                                                      hydraulicBoundaryLocation,
                                                      mechanismSpecificFactorizedSignalingNorm,
                                                      "Iv",
                                                      hydraulicBoundaryDatabase);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(1),
                                                      hydraulicBoundaryLocation,
                                                      mechanismSpecificSignalingNorm,
                                                      "IIv",
                                                      hydraulicBoundaryDatabase);

            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(2),
                                                      hydraulicBoundaryLocation,
                                                      mechanismSpecificLowerLimitNorm,
                                                      "IIIv",
                                                      hydraulicBoundaryDatabase);

            AssertWaveHeightCalculationActivity(activities.ElementAt(3),
                                                hydraulicBoundaryLocation,
                                                mechanismSpecificFactorizedSignalingNorm,
                                                "Iv", hydraulicBoundaryDatabase);
            AssertWaveHeightCalculationActivity(activities.ElementAt(4),
                                                hydraulicBoundaryLocation,
                                                mechanismSpecificSignalingNorm,
                                                "IIv", hydraulicBoundaryDatabase);

            AssertWaveHeightCalculationActivity(activities.ElementAt(5),
                                                hydraulicBoundaryLocation,
                                                mechanismSpecificLowerLimitNorm,
                                                "IIIv", hydraulicBoundaryDatabase);

            var hydraulicBoundaryLocationCalculationOutput = new TestHydraulicBoundaryLocationCalculationOutput(2.0);
            failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm.Single().Output = hydraulicBoundaryLocationCalculationOutput;

            AssertGrassCoverErosionOutwardsWaveConditionsCalculationActivity(activities.ElementAt(6),
                                                                             calculation1,
                                                                             hydraulicBoundaryLocationCalculationOutput.Result,
                                                                             hydraulicBoundaryDatabase);
            AssertGrassCoverErosionOutwardsWaveConditionsCalculationActivity(activities.ElementAt(7),
                                                                             calculation2,
                                                                             hydraulicBoundaryLocationCalculationOutput.Result,
                                                                             hydraulicBoundaryDatabase);
        }

        private static GrassCoverErosionOutwardsFailureMechanism CreateFailureMechanism()
        {
            return new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 10
            };
        }

        private static AssessmentSectionStub CreateAssessmentSection()
        {
            var assessmentSection = new AssessmentSectionStub
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = validFilePath
                }
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);

            return assessmentSection;
        }

        private static GrassCoverErosionOutwardsWaveConditionsCalculation CreateValidCalculation(HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            return new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    CategoryType = FailureMechanismCategoryType.MechanismSpecificFactorizedSignalingNorm,
                    ForeshoreProfile = new TestForeshoreProfile(true)
                    {
                        BreakWater =
                        {
                            Height = new Random(39).NextRoundedDouble()
                        }
                    },
                    UseForeshore = true,
                    UseBreakWater = true,
                    LowerBoundaryRevetment = (RoundedDouble) 1,
                    UpperBoundaryRevetment = (RoundedDouble) 3,
                    LowerBoundaryWaterLevels = (RoundedDouble) 1,
                    UpperBoundaryWaterLevels = (RoundedDouble) 3
                }
            };
        }

        private static void AssertGrassCoverErosionOutwardsWaveConditionsCalculationActivity(Activity activity,
                                                                                             GrassCoverErosionOutwardsWaveConditionsCalculation calculation,
                                                                                             RoundedDouble assessmentLevel,
                                                                                             HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            var mocks = new MockRepository();
            var testCalculator = new TestWaveConditionsCosineCalculator();
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            int nrOfCalculations = calculation.InputParameters.GetWaterLevels(assessmentLevel).Count() * 2;
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(hydraulicBoundaryDatabase),
                                     (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(testCalculator)
                             .Repeat
                             .Times(nrOfCalculations);
            mocks.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                activity.Run();

                Assert.AreEqual(nrOfCalculations, testCalculator.ReceivedInputs.Count());
                foreach (WaveConditionsCosineCalculationInput input in testCalculator.ReceivedInputs)
                {
                    Assert.AreEqual(calculation.InputParameters.BreakWater.Height, input.BreakWater.Height);
                }
            }

            mocks.VerifyAll();
        }

        #region Wave Conditions Calculations

        [Test]
        public void CreateCalculationActivity_CalculationNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => GrassCoverErosionOutwardsCalculationActivityFactory.CreateCalculationActivity(null,
                                                                                                                    new GrassCoverErosionOutwardsFailureMechanism(),
                                                                                                                    assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateCalculationActivity_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => GrassCoverErosionOutwardsCalculationActivityFactory.CreateCalculationActivity(new GrassCoverErosionOutwardsWaveConditionsCalculation(),
                                                                                                                    null,
                                                                                                                    assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateCalculationActivity_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            TestDelegate test = () => GrassCoverErosionOutwardsCalculationActivityFactory.CreateCalculationActivity(calculation, failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateCalculationActivity_WithValidCalculation_ReturnsGrassCoverErosionOutwardsWaveConditionsCalculationActivityWithParametersSet()
        {
            // Setup
            GrassCoverErosionOutwardsFailureMechanism failureMechanism = CreateFailureMechanism();
            var assessmentSection = new AssessmentSectionStub();
            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;
            hydraulicBoundaryDatabase.FilePath = validFilePath;

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("locationName 1");
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(failureMechanism, assessmentSection, new[]
            {
                hydraulicBoundaryLocation
            }, true);
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = CreateValidCalculation(hydraulicBoundaryLocation);

            // Call
            CalculatableActivity activity = GrassCoverErosionOutwardsCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                          failureMechanism,
                                                                                                                          assessmentSection);

            // Assert
            Assert.IsInstanceOf<GrassCoverErosionOutwardsWaveConditionsCalculationActivity>(activity);
            AssertGrassCoverErosionOutwardsWaveConditionsCalculationActivity(
                activity,
                calculation,
                failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm.Single().Output.Result,
                hydraulicBoundaryDatabase);
        }

        [Test]
        public void CreateCalculationActivitiesForCalculationGroup_CalculationGroupNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => GrassCoverErosionOutwardsCalculationActivityFactory.CreateCalculationActivities(null,
                                                                                                                      new GrassCoverErosionOutwardsFailureMechanism(),
                                                                                                                      assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculationGroup", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateCalculationActivitiesForCalculationGroup_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => GrassCoverErosionOutwardsCalculationActivityFactory.CreateCalculationActivities(new CalculationGroup(),
                                                                                                                      null,
                                                                                                                      assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateCalculationActivitiesForCalculationGroup_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => GrassCoverErosionOutwardsCalculationActivityFactory.CreateCalculationActivities(new CalculationGroup(),
                                                                                                                      new GrassCoverErosionOutwardsFailureMechanism(),
                                                                                                                      null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateCalculationActivitiesForCalculationGroup_WithValidCalculations_ReturnsGrassCoverErosionOutwardsWaveConditionsCalculationActivitiesWithParametersSet()
        {
            // Setup
            GrassCoverErosionOutwardsFailureMechanism failureMechanism = CreateFailureMechanism();
            var assessmentSection = new AssessmentSectionStub();
            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;
            hydraulicBoundaryDatabase.FilePath = validFilePath;

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("locationName 1");
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(failureMechanism, assessmentSection, new[]
            {
                hydraulicBoundaryLocation
            }, true);
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation1 = CreateValidCalculation(hydraulicBoundaryLocation);
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation2 = CreateValidCalculation(hydraulicBoundaryLocation);

            var calculations = new CalculationGroup
            {
                Children =
                {
                    calculation1,
                    calculation2
                }
            };

            // Call
            IEnumerable<CalculatableActivity> activities = GrassCoverErosionOutwardsCalculationActivityFactory.CreateCalculationActivities(
                calculations, failureMechanism, assessmentSection);

            // Assert
            CollectionAssert.AllItemsAreInstancesOfType(activities, typeof(GrassCoverErosionOutwardsWaveConditionsCalculationActivity));
            Assert.AreEqual(2, activities.Count());

            RoundedDouble assessmentLevel = failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm.Single().Output.Result;
            AssertGrassCoverErosionOutwardsWaveConditionsCalculationActivity(activities.First(), calculation1, assessmentLevel, hydraulicBoundaryDatabase);
            AssertGrassCoverErosionOutwardsWaveConditionsCalculationActivity(activities.ElementAt(1), calculation2, assessmentLevel, hydraulicBoundaryDatabase);
        }

        #endregion

        #region Hydraulic Boundary Location Calculations

        [Test]
        public void CreateDesignWaterLevelCalculationActivities_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () =>
                GrassCoverErosionOutwardsCalculationActivityFactory.CreateDesignWaterLevelCalculationActivities(
                    null,
                    assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateDesignWaterLevelCalculationActivities_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () =>
                GrassCoverErosionOutwardsCalculationActivityFactory.CreateDesignWaterLevelCalculationActivities(
                    new GrassCoverErosionOutwardsFailureMechanism(),
                    null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateDesignWaterLevelCalculationActivities_WithValidDataAndUsePreprocessorStates_ExpectedInputSetToActivities()
        {
            // Setup
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();
            GrassCoverErosionOutwardsFailureMechanism failureMechanism = CreateFailureMechanism();

            var hydraulicBoundaryLocation1 = new TestHydraulicBoundaryLocation("locationName 1");
            var hydraulicBoundaryLocation2 = new TestHydraulicBoundaryLocation("locationName 2");
            TestHydraulicBoundaryLocation[] hydraulicBoundaryLocations =
            {
                hydraulicBoundaryLocation1,
                hydraulicBoundaryLocation2
            };

            assessmentSection.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryLocations);
            failureMechanism.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryLocations);

            // Call
            IEnumerable<CalculatableActivity> activities =
                GrassCoverErosionOutwardsCalculationActivityFactory.CreateDesignWaterLevelCalculationActivities(
                    failureMechanism,
                    assessmentSection);

            // Assert
            Assert.AreEqual(10, activities.Count());

            double mechanismSpecificFactorizedSignalingNorm = GetExpectedNorm(failureMechanism,
                                                                              assessmentSection.FailureMechanismContribution.SignalingNorm / 30);
            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;
            AssertDesignWaterLevelCalculationActivity(activities.First(),
                                                      hydraulicBoundaryLocation1,
                                                      mechanismSpecificFactorizedSignalingNorm,
                                                      "Iv",
                                                      hydraulicBoundaryDatabase);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(1),
                                                      hydraulicBoundaryLocation2,
                                                      mechanismSpecificFactorizedSignalingNorm,
                                                      "Iv",
                                                      hydraulicBoundaryDatabase);

            double mechanismSpecificSignalingNorm = GetExpectedNorm(failureMechanism, assessmentSection.FailureMechanismContribution.SignalingNorm);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(2),
                                                      hydraulicBoundaryLocation1,
                                                      mechanismSpecificSignalingNorm,
                                                      "IIv",
                                                      hydraulicBoundaryDatabase);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(3),
                                                      hydraulicBoundaryLocation2,
                                                      mechanismSpecificSignalingNorm,
                                                      "IIv",
                                                      hydraulicBoundaryDatabase);

            double mechanismSpecificLowerLimitNorm = GetExpectedNorm(failureMechanism, assessmentSection.FailureMechanismContribution.LowerLimitNorm);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(4),
                                                      hydraulicBoundaryLocation1,
                                                      mechanismSpecificLowerLimitNorm,
                                                      "IIIv",
                                                      hydraulicBoundaryDatabase);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(5),
                                                      hydraulicBoundaryLocation2,
                                                      mechanismSpecificLowerLimitNorm,
                                                      "IIIv",
                                                      hydraulicBoundaryDatabase);

            double lowerLimitNorm = assessmentSection.FailureMechanismContribution.LowerLimitNorm;
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(6),
                                                      hydraulicBoundaryLocation1,
                                                      lowerLimitNorm,
                                                      "IVv",
                                                      hydraulicBoundaryDatabase);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(7),
                                                      hydraulicBoundaryLocation2,
                                                      lowerLimitNorm,
                                                      "IVv",
                                                      hydraulicBoundaryDatabase);

            double factorizedLowerLimitNorm = lowerLimitNorm * 30;
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(8),
                                                      hydraulicBoundaryLocation1,
                                                      factorizedLowerLimitNorm,
                                                      "Vv", hydraulicBoundaryDatabase);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(9),
                                                      hydraulicBoundaryLocation2,
                                                      factorizedLowerLimitNorm,
                                                      "Vv",
                                                      hydraulicBoundaryDatabase);
        }

        [Test]
        public void CreateWaveHeightCalculationActivities_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () =>
                GrassCoverErosionOutwardsCalculationActivityFactory.CreateWaveHeightCalculationActivities(
                    null,
                    assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateWaveHeightCalculationActivities_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () =>
                GrassCoverErosionOutwardsCalculationActivityFactory.CreateWaveHeightCalculationActivities(
                    new GrassCoverErosionOutwardsFailureMechanism(),
                    null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateWaveHeightCalculationActivities_WithValidData_ExpectedInputSetToActivities()
        {
            // Setup
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();
            GrassCoverErosionOutwardsFailureMechanism failureMechanism = CreateFailureMechanism();

            var hydraulicBoundaryLocation1 = new TestHydraulicBoundaryLocation("locationName 1");
            var hydraulicBoundaryLocation2 = new TestHydraulicBoundaryLocation("locationName 2");
            TestHydraulicBoundaryLocation[] hydraulicBoundaryLocations =
            {
                hydraulicBoundaryLocation1,
                hydraulicBoundaryLocation2
            };

            assessmentSection.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryLocations);
            failureMechanism.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryLocations);

            // Call
            IEnumerable<CalculatableActivity> activities =
                GrassCoverErosionOutwardsCalculationActivityFactory.CreateWaveHeightCalculationActivities(
                    failureMechanism,
                    assessmentSection);

            // Assert
            Assert.AreEqual(10, activities.Count());

            double mechanismSpecificFactorizedSignalingNorm = GetExpectedNorm(failureMechanism,
                                                                              assessmentSection.FailureMechanismContribution.SignalingNorm / 30);

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;
            AssertWaveHeightCalculationActivity(activities.First(),
                                                hydraulicBoundaryLocation1,
                                                mechanismSpecificFactorizedSignalingNorm,
                                                "Iv",
                                                hydraulicBoundaryDatabase);
            AssertWaveHeightCalculationActivity(activities.ElementAt(1),
                                                hydraulicBoundaryLocation2,
                                                mechanismSpecificFactorizedSignalingNorm,
                                                "Iv",
                                                hydraulicBoundaryDatabase);

            double mechanismSpecificSignalingNorm = GetExpectedNorm(failureMechanism, assessmentSection.FailureMechanismContribution.SignalingNorm);
            AssertWaveHeightCalculationActivity(activities.ElementAt(2),
                                                hydraulicBoundaryLocation1,
                                                mechanismSpecificSignalingNorm,
                                                "IIv",
                                                hydraulicBoundaryDatabase);
            AssertWaveHeightCalculationActivity(activities.ElementAt(3),
                                                hydraulicBoundaryLocation2,
                                                mechanismSpecificSignalingNorm,
                                                "IIv",
                                                hydraulicBoundaryDatabase);

            double mechanismSpecificLowerLimitNorm = GetExpectedNorm(failureMechanism, assessmentSection.FailureMechanismContribution.LowerLimitNorm);
            AssertWaveHeightCalculationActivity(activities.ElementAt(4),
                                                hydraulicBoundaryLocation1,
                                                mechanismSpecificLowerLimitNorm,
                                                "IIIv",
                                                hydraulicBoundaryDatabase);
            AssertWaveHeightCalculationActivity(activities.ElementAt(5),
                                                hydraulicBoundaryLocation2,
                                                mechanismSpecificLowerLimitNorm,
                                                "IIIv",
                                                hydraulicBoundaryDatabase);

            double lowerLimitNorm = assessmentSection.FailureMechanismContribution.LowerLimitNorm;
            AssertWaveHeightCalculationActivity(activities.ElementAt(6),
                                                hydraulicBoundaryLocation1,
                                                lowerLimitNorm,
                                                "IVv",
                                                hydraulicBoundaryDatabase);
            AssertWaveHeightCalculationActivity(activities.ElementAt(7),
                                                hydraulicBoundaryLocation2,
                                                lowerLimitNorm,
                                                "IVv",
                                                hydraulicBoundaryDatabase);

            double factorizedLowerLimitNorm = lowerLimitNorm * 30;
            AssertWaveHeightCalculationActivity(activities.ElementAt(8),
                                                hydraulicBoundaryLocation1,
                                                factorizedLowerLimitNorm,
                                                "Vv",
                                                hydraulicBoundaryDatabase);
            AssertWaveHeightCalculationActivity(activities.ElementAt(9),
                                                hydraulicBoundaryLocation2,
                                                factorizedLowerLimitNorm,
                                                "Vv",
                                                hydraulicBoundaryDatabase);
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
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(hydraulicBoundaryDatabase),
                                     (HydraRingCalculationSettings) invocation.Arguments[0]);
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
                             })
                             .Return(waveHeightCalculator);
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

        private static double GetExpectedNorm(GrassCoverErosionOutwardsFailureMechanism failureMechanism, double norm)
        {
            return RiskeerCommonDataCalculationService.ProfileSpecificRequiredProbability(
                norm,
                failureMechanism.Contribution,
                failureMechanism.GeneralInput.N);
        }

        #endregion
    }
}