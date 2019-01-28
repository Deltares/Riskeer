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
using Core.Common.Base.Data;
using Core.Common.Base.Service;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.Revetment.Data;
using Ringtoets.StabilityStoneCover.Data;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Data.Input.WaveConditions;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;

namespace Ringtoets.StabilityStoneCover.Service.Test
{
    [TestFixture]
    public class StabilityStoneCoverWaveConditionsCalculationActivityFactoryTest
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
            TestDelegate test = () => StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivities(null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateCalculationActivitiesForFailureMechanism_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivities(new StabilityStoneCoverFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateCalculationActivitiesForFailureMechanism_WithValidData_ExpectedInputSetToActivities()
        {
            // Setup
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();
            StabilityStoneCoverFailureMechanism failureMechanism = CreateFailureMechanism();

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("locationName 1");
            SetHydraulicBoundaryLocationToAssessmentSection(assessmentSection, hydraulicBoundaryLocation);

            StabilityStoneCoverWaveConditionsCalculation calculation1 = CreateValidCalculation(hydraulicBoundaryLocation);
            StabilityStoneCoverWaveConditionsCalculation calculation2 = CreateValidCalculation(hydraulicBoundaryLocation);

            failureMechanism.WaveConditionsCalculationGroup.Children.AddRange(new[]
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
            RoundedDouble assessmentLevel = assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.Single().Output.Result;
            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;
            AssertStabilityStoneCoverWaveConditionsCalculationActivity(activities.ElementAt(0), calculation1, assessmentLevel, hydraulicBoundaryDatabase);
            AssertStabilityStoneCoverWaveConditionsCalculationActivity(activities.ElementAt(1), calculation2, assessmentLevel, hydraulicBoundaryDatabase);
        }

        [Test]
        public void CreateCalculationActivity_CalculationNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(null,
                                                                                                                            new StabilityStoneCoverFailureMechanism(),
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
            TestDelegate test = () => StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(new StabilityStoneCoverWaveConditionsCalculation(),
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
            var calculation = new StabilityStoneCoverWaveConditionsCalculation();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Call
            TestDelegate test = () => StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(calculation, failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateCalculationActivity_WithValidCalculation_ReturnsStabilityStoneCoverWaveConditionsCalculationActivityWithParametersSet()
        {
            // Setup
            StabilityStoneCoverFailureMechanism failureMechanism = CreateFailureMechanism();
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
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
                                                                       assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.Single().Output.Result,
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
            TestDelegate test = () => StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivities(null,
                                                                                                                              new StabilityStoneCoverFailureMechanism(),
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
            TestDelegate test = () => StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivities(new CalculationGroup(),
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
            TestDelegate test = () => StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivities(new CalculationGroup(),
                                                                                                                              new StabilityStoneCoverFailureMechanism(),
                                                                                                                              null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateCalculationActivitiesForCalculationGroup_WithValidCalculations_ReturnsStabilityStoneCoverWaveConditionsCalculationActivitiesWithParametersSet()
        {
            // Setup
            StabilityStoneCoverFailureMechanism failureMechanism = CreateFailureMechanism();
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
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

            RoundedDouble assessmentLevel = assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.Single().Output.Result;
            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;
            AssertStabilityStoneCoverWaveConditionsCalculationActivity(activities.First(), calculation1, assessmentLevel, hydraulicBoundaryDatabase);
            AssertStabilityStoneCoverWaveConditionsCalculationActivity(activities.ElementAt(1), calculation2, assessmentLevel, hydraulicBoundaryDatabase);
        }

        private static void SetHydraulicBoundaryLocationToAssessmentSection(AssessmentSectionStub assessmentSection, TestHydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });
            assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.Single().Output = new TestHydraulicBoundaryLocationCalculationOutput(2.0);
        }

        private static StabilityStoneCoverFailureMechanism CreateFailureMechanism()
        {
            return new StabilityStoneCoverFailureMechanism
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

        private static StabilityStoneCoverWaveConditionsCalculation CreateValidCalculation(HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            return new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    CategoryType = AssessmentSectionCategoryType.FactorizedSignalingNorm,
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
                                     (HydraRingCalculationSettings)invocation.Arguments[0]);
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