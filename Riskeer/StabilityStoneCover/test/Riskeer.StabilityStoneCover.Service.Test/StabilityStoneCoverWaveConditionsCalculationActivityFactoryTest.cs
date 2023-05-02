// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Riskeer.StabilityStoneCover.Data;

namespace Riskeer.StabilityStoneCover.Service.Test
{
    [TestFixture]
    public class StabilityStoneCoverWaveConditionsCalculationActivityFactoryTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Service, "HydraRingCalculation");
        private static readonly string validHrdFilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");
        private static readonly string validHrdFileVersion = "IJssel lake2016-07-04 16:187";
        private static readonly string validHlcdFilePath = Path.Combine(testDataPath, "hlcd.sqlite");

        [Test]
        public void CreateCalculationActivitiesForFailureMechanism_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivities(null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateCalculationActivitiesForFailureMechanism_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivities(new StabilityStoneCoverFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateCalculationActivitiesForFailureMechanism_WithValidData_ExpectedInputSetToActivities()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("locationName 1");

            AssessmentSectionStub assessmentSection = CreateAssessmentSection(hydraulicBoundaryLocation);

            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            SetHydraulicBoundaryLocationToAssessmentSection(assessmentSection, hydraulicBoundaryLocation);

            StabilityStoneCoverWaveConditionsCalculation calculation1 = CreateValidCalculation(hydraulicBoundaryLocation);
            StabilityStoneCoverWaveConditionsCalculation calculation2 = CreateValidCalculation(hydraulicBoundaryLocation);

            failureMechanism.CalculationsGroup.Children.AddRange(new[]
            {
                calculation1,
                calculation2
            });

            // Call
            IEnumerable<CalculatableActivity> activities =
                StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivities(
                    failureMechanism,
                    assessmentSection);

            // Assert
            CollectionAssert.AllItemsAreInstancesOfType(activities, typeof(StabilityStoneCoverWaveConditionsCalculationActivity));
            Assert.AreEqual(2, activities.Count());
            RoundedDouble assessmentLevel = assessmentSection.WaterLevelCalculationsForSignalFloodingProbability.Single().Output.Result;
            HydraulicBoundaryData hydraulicBoundaryData = assessmentSection.HydraulicBoundaryData;
            AssertStabilityStoneCoverWaveConditionsCalculationActivity(activities.ElementAt(0), calculation1, assessmentLevel, hydraulicBoundaryData);
            AssertStabilityStoneCoverWaveConditionsCalculationActivity(activities.ElementAt(1), calculation2, assessmentLevel, hydraulicBoundaryData);
        }

        [Test]
        public void CreateCalculationActivity_CalculationNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(null,
                                                                                                                 new StabilityStoneCoverFailureMechanism(),
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
            void Call() => StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(new StabilityStoneCoverWaveConditionsCalculation(),
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
            var calculation = new StabilityStoneCoverWaveConditionsCalculation();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Call
            void Call() => StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(calculation, failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateCalculationActivity_WithValidCalculation_ReturnsStabilityStoneCoverWaveConditionsCalculationActivityWithParametersSet()
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            AssessmentSectionStub assessmentSection = CreateAssessmentSection(hydraulicBoundaryLocation);

            SetHydraulicBoundaryLocationToAssessmentSection(assessmentSection, hydraulicBoundaryLocation);

            StabilityStoneCoverWaveConditionsCalculation calculation = CreateValidCalculation(hydraulicBoundaryLocation);

            // Call
            CalculatableActivity activity = StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                                  failureMechanism,
                                                                                                                                  assessmentSection);

            // Assert
            Assert.IsInstanceOf<StabilityStoneCoverWaveConditionsCalculationActivity>(activity);
            AssertStabilityStoneCoverWaveConditionsCalculationActivity(activity,
                                                                       calculation,
                                                                       assessmentSection.WaterLevelCalculationsForSignalFloodingProbability.Single().Output.Result,
                                                                       assessmentSection.HydraulicBoundaryData);
        }

        [Test]
        public void CreateCalculationActivitiesForCalculationGroup_CalculationGroupNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivities(null,
                                                                                                                   new StabilityStoneCoverFailureMechanism(),
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
            void Call() => StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivities(new CalculationGroup(),
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
            void Call() => StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivities(new CalculationGroup(),
                                                                                                                   new StabilityStoneCoverFailureMechanism(),
                                                                                                                   null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateCalculationActivitiesForCalculationGroup_WithValidCalculations_ReturnsStabilityStoneCoverWaveConditionsCalculationActivitiesWithParametersSet()
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            AssessmentSectionStub assessmentSection = CreateAssessmentSection(hydraulicBoundaryLocation);

            SetHydraulicBoundaryLocationToAssessmentSection(assessmentSection, hydraulicBoundaryLocation);
            StabilityStoneCoverWaveConditionsCalculation calculation1 = CreateValidCalculation(hydraulicBoundaryLocation);
            StabilityStoneCoverWaveConditionsCalculation calculation2 = CreateValidCalculation(hydraulicBoundaryLocation);

            var calculations = new CalculationGroup
            {
                Children =
                {
                    calculation1,
                    calculation2
                }
            };

            // Call
            IEnumerable<CalculatableActivity> activities = StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivities(
                calculations, failureMechanism, assessmentSection);

            // Assert
            CollectionAssert.AllItemsAreInstancesOfType(activities, typeof(StabilityStoneCoverWaveConditionsCalculationActivity));
            Assert.AreEqual(2, activities.Count());

            RoundedDouble assessmentLevel = assessmentSection.WaterLevelCalculationsForSignalFloodingProbability.Single().Output.Result;
            HydraulicBoundaryData hydraulicBoundaryData = assessmentSection.HydraulicBoundaryData;
            AssertStabilityStoneCoverWaveConditionsCalculationActivity(activities.First(), calculation1, assessmentLevel, hydraulicBoundaryData);
            AssertStabilityStoneCoverWaveConditionsCalculationActivity(activities.ElementAt(1), calculation2, assessmentLevel, hydraulicBoundaryData);
        }

        private static void SetHydraulicBoundaryLocationToAssessmentSection(AssessmentSectionStub assessmentSection, TestHydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            assessmentSection.WaterLevelCalculationsForSignalFloodingProbability.Single().Output = new TestHydraulicBoundaryLocationCalculationOutput(2.0);
        }

        private static AssessmentSectionStub CreateAssessmentSection(HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            return new AssessmentSectionStub
            {
                HydraulicBoundaryData =
                {
                    HydraulicLocationConfigurationDatabase =
                    {
                        FilePath = validHlcdFilePath
                    },
                    HydraulicBoundaryDatabases =
                    {
                        new HydraulicBoundaryDatabase
                        {
                            FilePath = validHrdFilePath,
                            Version = validHrdFileVersion,
                            Locations =
                            {
                                hydraulicBoundaryLocation
                            }
                        }
                    }
                }
            };
        }

        private static StabilityStoneCoverWaveConditionsCalculation CreateValidCalculation(HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            return new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    WaterLevelType = WaveConditionsInputWaterLevelType.SignalFloodingProbability,
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

        private static void AssertStabilityStoneCoverWaveConditionsCalculationActivity(Activity activity,
                                                                                       StabilityStoneCoverWaveConditionsCalculation calculation,
                                                                                       RoundedDouble assessmentLevel,
                                                                                       HydraulicBoundaryData hydraulicBoundaryData)
        {
            var mocks = new MockRepository();
            var testCalculator = new TestWaveConditionsCosineCalculator();
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            int nrOfCalculations = calculation.InputParameters.GetWaterLevels(assessmentLevel).Count() * 2;
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(
                                         hydraulicBoundaryData,
                                         calculation.InputParameters.HydraulicBoundaryLocation),
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