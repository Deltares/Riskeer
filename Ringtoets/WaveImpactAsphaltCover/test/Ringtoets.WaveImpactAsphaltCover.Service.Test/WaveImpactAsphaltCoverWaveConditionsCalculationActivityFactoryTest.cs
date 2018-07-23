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
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input.WaveConditions;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.Revetment.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Ringtoets.WaveImpactAsphaltCover.Service.Test
{
    [TestFixture]
    public class WaveImpactAsphaltCoverWaveConditionsCalculationActivityFactoryTest
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
            TestDelegate test = () => WaveImpactAsphaltCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivities(null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateCalculationActivitiesForFailureMechanism_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => WaveImpactAsphaltCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivities(new WaveImpactAsphaltCoverFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
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
            RoundedDouble assessmentLevel = assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.Single().Output.Result;
            AssertWaveImpactAsphaltCoverWaveConditionsCalculationActivity(activities.ElementAt(0), calculation1, assessmentLevel);
            AssertWaveImpactAsphaltCoverWaveConditionsCalculationActivity(activities.ElementAt(1), calculation2, assessmentLevel);
        }

        [Test]
        public void CreateCalculationActivity_CalculationNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => WaveImpactAsphaltCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(null,
                                                                                                                               new WaveImpactAsphaltCoverFailureMechanism(),
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
            TestDelegate test = () => WaveImpactAsphaltCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(new WaveImpactAsphaltCoverWaveConditionsCalculation(),
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
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            // Call
            TestDelegate test = () => WaveImpactAsphaltCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(calculation, failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
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
                                                                          assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.Single().Output.Result);
        }

        [Test]
        public void CreateCalculationActivitiesForCalculationGroup_CalculationGroupNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => WaveImpactAsphaltCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivities(null,
                                                                                                                                 new WaveImpactAsphaltCoverFailureMechanism(),
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
            TestDelegate test = () => WaveImpactAsphaltCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivities(new CalculationGroup(),
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
            TestDelegate test = () => WaveImpactAsphaltCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivities(new CalculationGroup(),
                                                                                                                                 new WaveImpactAsphaltCoverFailureMechanism(),
                                                                                                                                 null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
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

            RoundedDouble assessmentLevel = assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.Single().Output.Result;
            AssertWaveImpactAsphaltCoverWaveConditionsCalculationActivity(activities.First(), calculation1, assessmentLevel);
            AssertWaveImpactAsphaltCoverWaveConditionsCalculationActivity(activities.ElementAt(1), calculation2, assessmentLevel);
        }

        private static void SetHydraulicBoundaryLocationToAssessmentSection(AssessmentSectionStub assessmentSection, TestHydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });
            assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.Single().Output = new TestHydraulicBoundaryLocationCalculationOutput(2.0);
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
            return new AssessmentSectionStub
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = validFilePath
                }
            };
        }

        private static WaveImpactAsphaltCoverWaveConditionsCalculation CreateValidCalculation(HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            return new WaveImpactAsphaltCoverWaveConditionsCalculation
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

        private static void AssertWaveImpactAsphaltCoverWaveConditionsCalculationActivity(Activity activity,
                                                                                          WaveImpactAsphaltCoverWaveConditionsCalculation calculation,
                                                                                          RoundedDouble assessmentLevel)
        {
            var mocks = new MockRepository();
            var testCalculator = new TestWaveConditionsCosineCalculator();
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            int nrOfCalculations = calculation.InputParameters.GetWaterLevels(assessmentLevel).Count();
            calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath, ""))
                             .Return(testCalculator).Repeat.Times(nrOfCalculations);
            mocks.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                activity.Run();

                Assert.AreEqual(nrOfCalculations, testCalculator.ReceivedInputs.Count);
                foreach (WaveConditionsCosineCalculationInput input in testCalculator.ReceivedInputs)
                {
                    Assert.AreEqual(calculation.InputParameters.BreakWater.Height, input.BreakWater.Height);
                }
            }

            mocks.VerifyAll();
        }
    }
}