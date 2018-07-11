﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.Data.Input.WaveConditions;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;

namespace Ringtoets.GrassCoverErosionOutwards.Service.Test
{
    [TestFixture]
    public class GrassCoverErosionOutwardsCalculationActivityFactoryTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");
        private static readonly string validFilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");
        private static readonly string validPreprocessorDirectory = TestHelper.GetScratchPadPath();

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
            TestHydraulicBoundaryLocation[] hydraulicBoundaryLocations =
            {
                hydraulicBoundaryLocation
            };

            assessmentSection.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryLocations);
            failureMechanism.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryLocations);

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
                                                                              () => assessmentSection.FailureMechanismContribution.SignalingNorm / 30);
            double mechanismSpecificSignalingNorm = GetExpectedNorm(failureMechanism, () => assessmentSection.FailureMechanismContribution.SignalingNorm);
            double mechanismSpecificLowerLimitNorm = GetExpectedNorm(failureMechanism, () => assessmentSection.FailureMechanismContribution.LowerLimitNorm);
            double lowerLimitNorm = assessmentSection.FailureMechanismContribution.LowerLimitNorm;
            double factorizedLowerLimitNorm = lowerLimitNorm * 30;

            AssertDesignWaterLevelCalculationActivity(activities.First(),
                                                      hydraulicBoundaryLocation,
                                                      mechanismSpecificFactorizedSignalingNorm,
                                                      "Iv->IIv");
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(1),
                                                      hydraulicBoundaryLocation,
                                                      mechanismSpecificSignalingNorm,
                                                      "IIv->IIIv");

            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(2),
                                                      hydraulicBoundaryLocation,
                                                      mechanismSpecificLowerLimitNorm,
                                                      "IIIv->IVv");
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(3),
                                                      hydraulicBoundaryLocation,
                                                      lowerLimitNorm,
                                                      "IVv->Vv");
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(4),
                                                      hydraulicBoundaryLocation,
                                                      factorizedLowerLimitNorm,
                                                      "Vv->VIv");

            AssertWaveHeightCalculationActivity(activities.ElementAt(5),
                                                hydraulicBoundaryLocation,
                                                mechanismSpecificFactorizedSignalingNorm,
                                                "Iv->IIv");
            AssertWaveHeightCalculationActivity(activities.ElementAt(6),
                                                hydraulicBoundaryLocation,
                                                mechanismSpecificSignalingNorm,
                                                "IIv->IIIv");

            AssertWaveHeightCalculationActivity(activities.ElementAt(7),
                                                hydraulicBoundaryLocation,
                                                mechanismSpecificLowerLimitNorm,
                                                "IIIv->IVv");
            AssertWaveHeightCalculationActivity(activities.ElementAt(8),
                                                hydraulicBoundaryLocation,
                                                lowerLimitNorm,
                                                "IVv->Vv");
            AssertWaveHeightCalculationActivity(activities.ElementAt(9),
                                                hydraulicBoundaryLocation,
                                                factorizedLowerLimitNorm,
                                                "Vv->VIv");

            failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm.Single().Output = new TestHydraulicBoundaryLocationCalculationOutput(2.0);

            AssertGrassCoverErosionOutwardsCalculationActivity(activities.ElementAt(10), calculation1);
            AssertGrassCoverErosionOutwardsCalculationActivity(activities.ElementAt(11), calculation2);
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

        #region Wave Conditions

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

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                       mocks,
                                                                                                       validFilePath);

            mocks.ReplayAll();

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = CreateValidCalculation(hydraulicBoundaryLocation);

            AddHydraulicBoundaryLocationToFailureMechanism(failureMechanism, hydraulicBoundaryLocation);

            // Call
            CalculatableActivity activity = GrassCoverErosionOutwardsCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                          failureMechanism,
                                                                                                                          assessmentSection);

            // Assert
            Assert.IsInstanceOf<GrassCoverErosionOutwardsWaveConditionsCalculationActivity>(activity);
            AssertGrassCoverErosionOutwardsCalculationActivity(activity, calculation);
            mocks.VerifyAll();
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

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                       mocks,
                                                                                                       validFilePath);

            mocks.ReplayAll();

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation1 = CreateValidCalculation(hydraulicBoundaryLocation);
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation2 = CreateValidCalculation(hydraulicBoundaryLocation);

            AddHydraulicBoundaryLocationToFailureMechanism(failureMechanism, hydraulicBoundaryLocation);

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

            AssertGrassCoverErosionOutwardsCalculationActivity(activities.First(), calculation1);
            AssertGrassCoverErosionOutwardsCalculationActivity(activities.ElementAt(1), calculation2);
            mocks.VerifyAll();
        }

        private static void AddHydraulicBoundaryLocationToFailureMechanism(GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                                                           TestHydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            failureMechanism.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });
            failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm.Single().Output = new TestHydraulicBoundaryLocationCalculationOutput(2.0);
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

        private static void AssertGrassCoverErosionOutwardsCalculationActivity(Activity activity,
                                                                               GrassCoverErosionOutwardsWaveConditionsCalculation calculation,
                                                                               bool usePreprocessor = false)
        {
            var mocks = new MockRepository();
            var testCalculator = new TestWaveConditionsCosineCalculator();
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath, usePreprocessor ? validPreprocessorDirectory : ""))
                             .Return(testCalculator).Repeat.Times(3);
            mocks.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                activity.Run();

                Assert.AreEqual(3, testCalculator.ReceivedInputs.Count);
                foreach (WaveConditionsCosineCalculationInput input in testCalculator.ReceivedInputs)
                {
                    Assert.AreEqual(calculation.InputParameters.BreakWater.Height, input.BreakWater.Height);
                }
            }

            mocks.VerifyAll();
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
                                                                              () => assessmentSection.FailureMechanismContribution.SignalingNorm / 30);
            AssertDesignWaterLevelCalculationActivity(activities.First(),
                                                      hydraulicBoundaryLocation1,
                                                      mechanismSpecificFactorizedSignalingNorm,
                                                      "Iv->IIv");
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(1),
                                                      hydraulicBoundaryLocation2,
                                                      mechanismSpecificFactorizedSignalingNorm,
                                                      "Iv->IIv");

            double mechanismSpecificSignalingNorm = GetExpectedNorm(failureMechanism, () => assessmentSection.FailureMechanismContribution.SignalingNorm);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(2),
                                                      hydraulicBoundaryLocation1,
                                                      mechanismSpecificSignalingNorm,
                                                      "IIv->IIIv");
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(3),
                                                      hydraulicBoundaryLocation2,
                                                      mechanismSpecificSignalingNorm,
                                                      "IIv->IIIv");

            double mechanismSpecificLowerLimitNorm = GetExpectedNorm(failureMechanism, () => assessmentSection.FailureMechanismContribution.LowerLimitNorm);
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(4),
                                                      hydraulicBoundaryLocation1,
                                                      mechanismSpecificLowerLimitNorm,
                                                      "IIIv->IVv");
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(5),
                                                      hydraulicBoundaryLocation2,
                                                      mechanismSpecificLowerLimitNorm,
                                                      "IIIv->IVv");

            double lowerLimitNorm = assessmentSection.FailureMechanismContribution.LowerLimitNorm;
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(6),
                                                      hydraulicBoundaryLocation1,
                                                      lowerLimitNorm,
                                                      "IVv->Vv");
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(7),
                                                      hydraulicBoundaryLocation2,
                                                      lowerLimitNorm,
                                                      "IVv->Vv");

            double factorizedLowerLimitNorm = lowerLimitNorm * 30;
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(8),
                                                      hydraulicBoundaryLocation1,
                                                      factorizedLowerLimitNorm,
                                                      "Vv->VIv");
            AssertDesignWaterLevelCalculationActivity(activities.ElementAt(9),
                                                      hydraulicBoundaryLocation2,
                                                      factorizedLowerLimitNorm,
                                                      "Vv->VIv");
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
                                                                              () => assessmentSection.FailureMechanismContribution.SignalingNorm / 30);
            AssertWaveHeightCalculationActivity(activities.First(),
                                                hydraulicBoundaryLocation1,
                                                mechanismSpecificFactorizedSignalingNorm,
                                                "Iv->IIv");
            AssertWaveHeightCalculationActivity(activities.ElementAt(1),
                                                hydraulicBoundaryLocation2,
                                                mechanismSpecificFactorizedSignalingNorm,
                                                "Iv->IIv");

            double mechanismSpecificSignalingNorm = GetExpectedNorm(failureMechanism, () => assessmentSection.FailureMechanismContribution.SignalingNorm);
            AssertWaveHeightCalculationActivity(activities.ElementAt(2),
                                                hydraulicBoundaryLocation1,
                                                mechanismSpecificSignalingNorm,
                                                "IIv->IIIv");
            AssertWaveHeightCalculationActivity(activities.ElementAt(3),
                                                hydraulicBoundaryLocation2,
                                                mechanismSpecificSignalingNorm,
                                                "IIv->IIIv");

            double mechanismSpecificLowerLimitNorm = GetExpectedNorm(failureMechanism, () => assessmentSection.FailureMechanismContribution.LowerLimitNorm);
            AssertWaveHeightCalculationActivity(activities.ElementAt(4),
                                                hydraulicBoundaryLocation1,
                                                mechanismSpecificLowerLimitNorm,
                                                "IIIv->IVv");
            AssertWaveHeightCalculationActivity(activities.ElementAt(5),
                                                hydraulicBoundaryLocation2,
                                                mechanismSpecificLowerLimitNorm,
                                                "IIIv->IVv");

            double lowerLimitNorm = assessmentSection.FailureMechanismContribution.LowerLimitNorm;
            AssertWaveHeightCalculationActivity(activities.ElementAt(6),
                                                hydraulicBoundaryLocation1,
                                                lowerLimitNorm,
                                                "IVv->Vv");
            AssertWaveHeightCalculationActivity(activities.ElementAt(7),
                                                hydraulicBoundaryLocation2,
                                                lowerLimitNorm,
                                                "IVv->Vv");

            double factorizedLowerLimitNorm = lowerLimitNorm * 30;
            AssertWaveHeightCalculationActivity(activities.ElementAt(8),
                                                hydraulicBoundaryLocation1,
                                                factorizedLowerLimitNorm,
                                                "Vv->VIv");
            AssertWaveHeightCalculationActivity(activities.ElementAt(9),
                                                hydraulicBoundaryLocation2,
                                                factorizedLowerLimitNorm,
                                                "Vv->VIv");
        }

        private static void AssertDesignWaterLevelCalculationActivity(Activity activity,
                                                                      HydraulicBoundaryLocation hydraulicBoundaryLocation,
                                                                      double norm,
                                                                      string categoryBoundaryName)
        {
            var mocks = new MockRepository();
            var designWaterLevelCalculator = new TestDesignWaterLevelCalculator();
            var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(testDataPath, "")).Return(designWaterLevelCalculator);
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
                                                                string categoryBoundaryName)
        {
            var mocks = new MockRepository();
            var waveHeightCalculator = new TestWaveHeightCalculator();
            var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(testDataPath, "")).Return(waveHeightCalculator);
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

        private static double GetExpectedNorm(GrassCoverErosionOutwardsFailureMechanism failureMechanism, Func<double> getNormFunc)
        {
            return RingtoetsCommonDataCalculationService.ProfileSpecificRequiredProbability(
                getNormFunc(),
                failureMechanism.Contribution,
                failureMechanism.GeneralInput.N);
        }

        #endregion
    }
}