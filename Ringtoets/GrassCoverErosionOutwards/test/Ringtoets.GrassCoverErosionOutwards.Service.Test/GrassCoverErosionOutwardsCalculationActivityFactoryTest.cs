﻿// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Base.Service;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data.TestUtil;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.Data.Input.WaveConditions;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.Revetment.Data;

namespace Ringtoets.GrassCoverErosionOutwards.Service.Test
{
    [TestFixture]
    public class GrassCoverErosionOutwardsCalculationActivityFactoryTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");
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

            AssertDesignWaterLevelCalculationActivity(activities.First(),
                                                      hydraulicBoundaryLocation,
                                                      mechanismSpecificFactorizedSignalingNorm,
                                                      "Iv");
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(1),
                                                      hydraulicBoundaryLocation,
                                                      mechanismSpecificSignalingNorm,
                                                      "IIv");

            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(2),
                                                      hydraulicBoundaryLocation,
                                                      mechanismSpecificLowerLimitNorm,
                                                      "IIIv");
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(3),
                                                      hydraulicBoundaryLocation,
                                                      lowerLimitNorm,
                                                      "IVv");
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(4),
                                                      hydraulicBoundaryLocation,
                                                      factorizedLowerLimitNorm,
                                                      "Vv");

            AssertWaveHeightCalculationActivity(activities.ElementAt(5),
                                                hydraulicBoundaryLocation,
                                                mechanismSpecificFactorizedSignalingNorm,
                                                "Iv");
            AssertWaveHeightCalculationActivity(activities.ElementAt(6),
                                                hydraulicBoundaryLocation,
                                                mechanismSpecificSignalingNorm,
                                                "IIv");

            AssertWaveHeightCalculationActivity(activities.ElementAt(7),
                                                hydraulicBoundaryLocation,
                                                mechanismSpecificLowerLimitNorm,
                                                "IIIv");
            AssertWaveHeightCalculationActivity(activities.ElementAt(8),
                                                hydraulicBoundaryLocation,
                                                lowerLimitNorm,
                                                "IVv");
            AssertWaveHeightCalculationActivity(activities.ElementAt(9),
                                                hydraulicBoundaryLocation,
                                                factorizedLowerLimitNorm,
                                                "Vv");

            var hydraulicBoundaryLocationCalculationOutput = new TestHydraulicBoundaryLocationCalculationOutput(2.0);
            failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm.Single().Output = hydraulicBoundaryLocationCalculationOutput;

            AssertGrassCoverErosionOutwardsWaveConditionsCalculationActivity(activities.ElementAt(10), calculation1, hydraulicBoundaryLocationCalculationOutput.Result);
            AssertGrassCoverErosionOutwardsWaveConditionsCalculationActivity(activities.ElementAt(11), calculation2, hydraulicBoundaryLocationCalculationOutput.Result);
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

            AssertDesignWaterLevelCalculationActivity(activities.First(),
                                                      hydraulicBoundaryLocation,
                                                      mechanismSpecificFactorizedSignalingNorm,
                                                      "Iv");
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(1),
                                                      hydraulicBoundaryLocation,
                                                      mechanismSpecificSignalingNorm,
                                                      "IIv");

            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(2),
                                                      hydraulicBoundaryLocation,
                                                      mechanismSpecificLowerLimitNorm,
                                                      "IIIv");

            AssertWaveHeightCalculationActivity(activities.ElementAt(3),
                                                hydraulicBoundaryLocation,
                                                mechanismSpecificFactorizedSignalingNorm,
                                                "Iv");
            AssertWaveHeightCalculationActivity(activities.ElementAt(4),
                                                hydraulicBoundaryLocation,
                                                mechanismSpecificSignalingNorm,
                                                "IIv");

            AssertWaveHeightCalculationActivity(activities.ElementAt(5),
                                                hydraulicBoundaryLocation,
                                                mechanismSpecificLowerLimitNorm,
                                                "IIIv");

            var hydraulicBoundaryLocationCalculationOutput = new TestHydraulicBoundaryLocationCalculationOutput(2.0);
            failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm.Single().Output = hydraulicBoundaryLocationCalculationOutput;

            AssertGrassCoverErosionOutwardsWaveConditionsCalculationActivity(activities.ElementAt(6), calculation1, hydraulicBoundaryLocationCalculationOutput.Result);
            AssertGrassCoverErosionOutwardsWaveConditionsCalculationActivity(activities.ElementAt(7), calculation2, hydraulicBoundaryLocationCalculationOutput.Result);
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
            return new AssessmentSectionStub
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = validFilePath
                }
            };
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
                                                                                             RoundedDouble assessmentLevel)
        {
            var mocks = new MockRepository();
            var testCalculator = new TestWaveConditionsCosineCalculator();
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            int nrOfCalculations = calculation.InputParameters.GetWaterLevels(assessmentLevel).Count();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings) invocation.Arguments[0];
                                 Assert.AreEqual(validFilePath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
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
            assessmentSection.HydraulicBoundaryDatabase.FilePath = validFilePath;

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
                failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm.Single().Output.Result);
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
            assessmentSection.HydraulicBoundaryDatabase.FilePath = validFilePath;

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
            AssertGrassCoverErosionOutwardsWaveConditionsCalculationActivity(activities.First(), calculation1, assessmentLevel);
            AssertGrassCoverErosionOutwardsWaveConditionsCalculationActivity(activities.ElementAt(1), calculation2, assessmentLevel);
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
            AssertDesignWaterLevelCalculationActivity(activities.First(),
                                                      hydraulicBoundaryLocation1,
                                                      mechanismSpecificFactorizedSignalingNorm,
                                                      "Iv");
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(1),
                                                      hydraulicBoundaryLocation2,
                                                      mechanismSpecificFactorizedSignalingNorm,
                                                      "Iv");

            double mechanismSpecificSignalingNorm = GetExpectedNorm(failureMechanism, assessmentSection.FailureMechanismContribution.SignalingNorm);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(2),
                                                      hydraulicBoundaryLocation1,
                                                      mechanismSpecificSignalingNorm,
                                                      "IIv");
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(3),
                                                      hydraulicBoundaryLocation2,
                                                      mechanismSpecificSignalingNorm,
                                                      "IIv");

            double mechanismSpecificLowerLimitNorm = GetExpectedNorm(failureMechanism, assessmentSection.FailureMechanismContribution.LowerLimitNorm);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(4),
                                                      hydraulicBoundaryLocation1,
                                                      mechanismSpecificLowerLimitNorm,
                                                      "IIIv");
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(5),
                                                      hydraulicBoundaryLocation2,
                                                      mechanismSpecificLowerLimitNorm,
                                                      "IIIv");

            double lowerLimitNorm = assessmentSection.FailureMechanismContribution.LowerLimitNorm;
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(6),
                                                      hydraulicBoundaryLocation1,
                                                      lowerLimitNorm,
                                                      "IVv");
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(7),
                                                      hydraulicBoundaryLocation2,
                                                      lowerLimitNorm,
                                                      "IVv");

            double factorizedLowerLimitNorm = lowerLimitNorm * 30;
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(8),
                                                      hydraulicBoundaryLocation1,
                                                      factorizedLowerLimitNorm,
                                                      "Vv");
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(9),
                                                      hydraulicBoundaryLocation2,
                                                      factorizedLowerLimitNorm,
                                                      "Vv");
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
            AssertWaveHeightCalculationActivity(activities.First(),
                                                hydraulicBoundaryLocation1,
                                                mechanismSpecificFactorizedSignalingNorm,
                                                "Iv");
            AssertWaveHeightCalculationActivity(activities.ElementAt(1),
                                                hydraulicBoundaryLocation2,
                                                mechanismSpecificFactorizedSignalingNorm,
                                                "Iv");

            double mechanismSpecificSignalingNorm = GetExpectedNorm(failureMechanism, assessmentSection.FailureMechanismContribution.SignalingNorm);
            AssertWaveHeightCalculationActivity(activities.ElementAt(2),
                                                hydraulicBoundaryLocation1,
                                                mechanismSpecificSignalingNorm,
                                                "IIv");
            AssertWaveHeightCalculationActivity(activities.ElementAt(3),
                                                hydraulicBoundaryLocation2,
                                                mechanismSpecificSignalingNorm,
                                                "IIv");

            double mechanismSpecificLowerLimitNorm = GetExpectedNorm(failureMechanism, assessmentSection.FailureMechanismContribution.LowerLimitNorm);
            AssertWaveHeightCalculationActivity(activities.ElementAt(4),
                                                hydraulicBoundaryLocation1,
                                                mechanismSpecificLowerLimitNorm,
                                                "IIIv");
            AssertWaveHeightCalculationActivity(activities.ElementAt(5),
                                                hydraulicBoundaryLocation2,
                                                mechanismSpecificLowerLimitNorm,
                                                "IIIv");

            double lowerLimitNorm = assessmentSection.FailureMechanismContribution.LowerLimitNorm;
            AssertWaveHeightCalculationActivity(activities.ElementAt(6),
                                                hydraulicBoundaryLocation1,
                                                lowerLimitNorm,
                                                "IVv");
            AssertWaveHeightCalculationActivity(activities.ElementAt(7),
                                                hydraulicBoundaryLocation2,
                                                lowerLimitNorm,
                                                "IVv");

            double factorizedLowerLimitNorm = lowerLimitNorm * 30;
            AssertWaveHeightCalculationActivity(activities.ElementAt(8),
                                                hydraulicBoundaryLocation1,
                                                factorizedLowerLimitNorm,
                                                "Vv");
            AssertWaveHeightCalculationActivity(activities.ElementAt(9),
                                                hydraulicBoundaryLocation2,
                                                factorizedLowerLimitNorm,
                                                "Vv");
        }

        private static void AssertDesignWaterLevelCalculationActivity(Activity activity,
                                                                      HydraulicBoundaryLocation hydraulicBoundaryLocation,
                                                                      double norm,
                                                                      string categoryBoundaryName)
        {
            var mocks = new MockRepository();
            var designWaterLevelCalculator = new TestDesignWaterLevelCalculator();
            var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings) invocation.Arguments[0];
                                 Assert.AreEqual(validFilePath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
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
                                                                string categoryBoundaryName)
        {
            var mocks = new MockRepository();
            var waveHeightCalculator = new TestWaveHeightCalculator();
            var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 var settings = (HydraRingCalculationSettings)invocation.Arguments[0];
                                 Assert.AreEqual(validFilePath, settings.HlcdFilePath);
                                 Assert.IsEmpty(settings.PreprocessorDirectory);
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
            return RingtoetsCommonDataCalculationService.ProfileSpecificRequiredProbability(
                norm,
                failureMechanism.Contribution,
                failureMechanism.GeneralInput.N);
        }

        #endregion
    }
}