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
using Core.Common.Base.Data;
using Core.Common.Base.Service;
using Core.Common.TestUtil;
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
        public void CreateCalculationActivity_CalculationNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => GrassCoverErosionOutwardsCalculationActivityFactory.CreateWaveConditionsCalculationActivity(null,
                                                                                                                       new GrassCoverErosionOutwardsFailureMechanism(),
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
            void Call() => GrassCoverErosionOutwardsCalculationActivityFactory.CreateWaveConditionsCalculationActivity(new GrassCoverErosionOutwardsWaveConditionsCalculation(),
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
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            void Call() => GrassCoverErosionOutwardsCalculationActivityFactory.CreateWaveConditionsCalculationActivity(calculation, failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
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
            CalculatableActivity activity = GrassCoverErosionOutwardsCalculationActivityFactory.CreateWaveConditionsCalculationActivity(calculation,
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
        public void CreateWaveConditionsCalculationActivitiesForCalculationGroup_CalculationGroupNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => GrassCoverErosionOutwardsCalculationActivityFactory.CreateWaveConditionsCalculationActivities(
                null, new GrassCoverErosionOutwardsFailureMechanism(), assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationGroup", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateWaveConditionsCalculationActivitiesForCalculationGroup_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => GrassCoverErosionOutwardsCalculationActivityFactory.CreateWaveConditionsCalculationActivities(
                new CalculationGroup(), null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateWaveConditionsCalculationActivitiesForCalculationGroup_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => GrassCoverErosionOutwardsCalculationActivityFactory.CreateWaveConditionsCalculationActivities(
                new CalculationGroup(), new GrassCoverErosionOutwardsFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateWaveConditionsCalculationActivitiesForCalculationGroup_WithValidCalculations_ReturnsGrassCoverErosionOutwardsWaveConditionsCalculationActivitiesWithParametersSet()
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
            IEnumerable<CalculatableActivity> activities = GrassCoverErosionOutwardsCalculationActivityFactory.CreateWaveConditionsCalculationActivities(
                calculations, failureMechanism, assessmentSection);

            // Assert
            CollectionAssert.AllItemsAreInstancesOfType(activities, typeof(GrassCoverErosionOutwardsWaveConditionsCalculationActivity));
            Assert.AreEqual(2, activities.Count());

            RoundedDouble assessmentLevel = failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm.Single().Output.Result;
            AssertGrassCoverErosionOutwardsWaveConditionsCalculationActivity(activities.First(), calculation1, assessmentLevel, hydraulicBoundaryDatabase);
            AssertGrassCoverErosionOutwardsWaveConditionsCalculationActivity(activities.ElementAt(1), calculation2, assessmentLevel, hydraulicBoundaryDatabase);
        }

        private static GrassCoverErosionOutwardsFailureMechanism CreateFailureMechanism()
        {
            return new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 10
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
    }
}