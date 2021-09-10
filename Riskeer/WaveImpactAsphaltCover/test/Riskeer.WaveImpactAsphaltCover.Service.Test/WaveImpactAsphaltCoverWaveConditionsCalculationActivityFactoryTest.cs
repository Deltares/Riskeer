﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Service;
using Riskeer.Common.Service.TestUtil;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Data.Input.WaveConditions;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;
using Riskeer.Revetment.Data;
using Riskeer.WaveImpactAsphaltCover.Data;

namespace Riskeer.WaveImpactAsphaltCover.Service.Test
{
    [TestFixture]
    public class WaveImpactAsphaltCoverWaveConditionsCalculationActivityFactoryTest
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
            void Call() => WaveImpactAsphaltCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivities(null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateCalculationActivitiesForFailureMechanism_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => WaveImpactAsphaltCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivities(new WaveImpactAsphaltCoverFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateCalculationActivitiesForFailureMechanism_WithValidData_ExpectedInputSetToActivities()
        {
            // Setup
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();
            WaveImpactAsphaltCoverFailureMechanism failureMechanism = CreateFailureMechanism();

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("locationName 1");
            SetHydraulicBoundaryLocationToAssessmentSection(assessmentSection, hydraulicBoundaryLocation);

            WaveImpactAsphaltCoverWaveConditionsCalculation calculation1 = CreateValidCalculation(hydraulicBoundaryLocation);
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation2 = CreateValidCalculation(hydraulicBoundaryLocation);

            failureMechanism.WaveConditionsCalculationGroup.Children.AddRange(new[]
            {
                calculation1,
                calculation2
            });

            // Call
            IEnumerable<CalculatableActivity> activities =
                WaveImpactAsphaltCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivities(
                    failureMechanism,
                    assessmentSection);

            // Assert
            CollectionAssert.AllItemsAreInstancesOfType(activities, typeof(WaveImpactAsphaltCoverWaveConditionsCalculationActivity));
            Assert.AreEqual(2, activities.Count());
            RoundedDouble assessmentLevel = assessmentSection.WaterLevelCalculationsForSignalingNorm.Single().Output.Result;
            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;
            AssertWaveImpactAsphaltCoverWaveConditionsCalculationActivity(activities.ElementAt(0), calculation1, assessmentLevel, hydraulicBoundaryDatabase);
            AssertWaveImpactAsphaltCoverWaveConditionsCalculationActivity(activities.ElementAt(1), calculation2, assessmentLevel, hydraulicBoundaryDatabase);
        }

        [Test]
        public void CreateCalculationActivity_CalculationNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => WaveImpactAsphaltCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(null,
                                                                                                                    new WaveImpactAsphaltCoverFailureMechanism(),
                                                                                                                    assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
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
            void Call() => WaveImpactAsphaltCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(new WaveImpactAsphaltCoverWaveConditionsCalculation(),
                                                                                                                    null,
                                                                                                                    assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateCalculationActivity_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            // Call
            void Call() => WaveImpactAsphaltCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(calculation, failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateCalculationActivity_WithValidCalculation_ReturnsWaveImpactAsphaltCoverWaveConditionsCalculationActivityWithParametersSet()
        {
            // Setup
            WaveImpactAsphaltCoverFailureMechanism failureMechanism = CreateFailureMechanism();
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            SetHydraulicBoundaryLocationToAssessmentSection(assessmentSection, hydraulicBoundaryLocation);

            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = CreateValidCalculation(hydraulicBoundaryLocation);

            // Call
            CalculatableActivity activity = WaveImpactAsphaltCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                                     failureMechanism,
                                                                                                                                     assessmentSection);

            // Assert
            Assert.IsInstanceOf<WaveImpactAsphaltCoverWaveConditionsCalculationActivity>(activity);
            AssertWaveImpactAsphaltCoverWaveConditionsCalculationActivity(activity,
                                                                          calculation,
                                                                          assessmentSection.WaterLevelCalculationsForSignalingNorm.Single().Output.Result,
                                                                          assessmentSection.HydraulicBoundaryDatabase);
        }

        [Test]
        public void CreateCalculationActivitiesForCalculationGroup_CalculationGroupNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => WaveImpactAsphaltCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivities(null,
                                                                                                                      new WaveImpactAsphaltCoverFailureMechanism(),
                                                                                                                      assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
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
            void Call() => WaveImpactAsphaltCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivities(new CalculationGroup(),
                                                                                                                      null,
                                                                                                                      assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateCalculationActivitiesForCalculationGroup_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => WaveImpactAsphaltCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivities(new CalculationGroup(),
                                                                                                                      new WaveImpactAsphaltCoverFailureMechanism(),
                                                                                                                      null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateCalculationActivitiesForCalculationGroup_WithValidCalculations_ReturnsWaveImpactAsphaltCoverWaveConditionsCalculationActivitiesWithParametersSet()
        {
            // Setup
            WaveImpactAsphaltCoverFailureMechanism failureMechanism = CreateFailureMechanism();
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            SetHydraulicBoundaryLocationToAssessmentSection(assessmentSection, hydraulicBoundaryLocation);
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation1 = CreateValidCalculation(hydraulicBoundaryLocation);
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation2 = CreateValidCalculation(hydraulicBoundaryLocation);

            var calculations = new CalculationGroup
            {
                Children =
                {
                    calculation1,
                    calculation2
                }
            };

            // Call
            IEnumerable<CalculatableActivity> activities = WaveImpactAsphaltCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivities(
                calculations, failureMechanism, assessmentSection);

            // Assert
            CollectionAssert.AllItemsAreInstancesOfType(activities, typeof(WaveImpactAsphaltCoverWaveConditionsCalculationActivity));
            Assert.AreEqual(2, activities.Count());

            RoundedDouble assessmentLevel = assessmentSection.WaterLevelCalculationsForSignalingNorm.Single().Output.Result;
            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;
            AssertWaveImpactAsphaltCoverWaveConditionsCalculationActivity(activities.First(), calculation1, assessmentLevel, hydraulicBoundaryDatabase);
            AssertWaveImpactAsphaltCoverWaveConditionsCalculationActivity(activities.ElementAt(1), calculation2, assessmentLevel, hydraulicBoundaryDatabase);
        }

        private static void SetHydraulicBoundaryLocationToAssessmentSection(AssessmentSectionStub assessmentSection, TestHydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            assessmentSection.WaterLevelCalculationsForSignalingNorm.Single().Output = new TestHydraulicBoundaryLocationCalculationOutput(2.0);
        }

        private static WaveImpactAsphaltCoverFailureMechanism CreateFailureMechanism()
        {
            return new WaveImpactAsphaltCoverFailureMechanism
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

        private static WaveImpactAsphaltCoverWaveConditionsCalculation CreateValidCalculation(HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            return new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    WaterLevelType = WaveConditionsInputWaterLevelType.Signaling,
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

        private static void AssertWaveImpactAsphaltCoverWaveConditionsCalculationActivity(Activity activity,
                                                                                          WaveImpactAsphaltCoverWaveConditionsCalculation calculation,
                                                                                          RoundedDouble assessmentLevel,
                                                                                          HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            var mocks = new MockRepository();
            var testCalculator = new TestWaveConditionsCosineCalculator();
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            int nrOfCalculations = calculation.InputParameters.GetWaterLevels(assessmentLevel).Count();

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
    }
}