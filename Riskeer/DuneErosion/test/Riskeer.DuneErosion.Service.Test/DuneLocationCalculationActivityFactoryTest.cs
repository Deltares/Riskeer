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
using System.IO;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Base.Service;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Service;
using Riskeer.Common.Service.TestUtil;
using Riskeer.DuneErosion.Data;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Data.Input.Hydraulics;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;

namespace Riskeer.DuneErosion.Service.Test
{
    [TestFixture]
    public class DuneLocationCalculationActivityFactoryTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Service, "HydraRingCalculation");
        private static readonly string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
        private static readonly string validPreprocessorDirectory = TestHelper.GetScratchPadPath();

        [Test]
        public void CreateCalculationActivitiesForCalculations_CalculationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            // Call
            void Call() => DuneLocationCalculationActivityFactory.CreateCalculationActivities(null,
                                                                                              assessmentSection,
                                                                                              0.01,
                                                                                              "1/100");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculations", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void CreateCalculationActivitiesForCalculations_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => DuneLocationCalculationActivityFactory.CreateCalculationActivities(Enumerable.Empty<DuneLocationCalculation>(),
                                                                                              null,
                                                                                              0.01,
                                                                                              "1/100");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateCalculationActivitiesForCalculations_WithValidDataAndUsePreprocessorStates_ReturnsExpectedActivities(bool usePreprocessor)
        {
            // Setup
            const double targetProbability = 0.01;
            const string calculationIdentifier = "1/100";

            AssessmentSectionStub assessmentSection = CreateAssessmentSection(usePreprocessor);

            var duneLocation1 = new DuneLocation(1, "locationName1", new Point2D(1, 1), new DuneLocation.ConstructionProperties());
            var duneLocation2 = new DuneLocation(2, "locationName2", new Point2D(2, 2), new DuneLocation.ConstructionProperties());

            // Call
            CalculatableActivity[] activities = DuneLocationCalculationActivityFactory.CreateCalculationActivities(
                new[]
                {
                    new DuneLocationCalculation(duneLocation1),
                    new DuneLocationCalculation(duneLocation2)
                },
                assessmentSection,
                targetProbability,
                calculationIdentifier).ToArray();

            // Assert
            CollectionAssert.AllItemsAreInstancesOfType(activities, typeof(DuneLocationCalculationActivity));
            Assert.AreEqual(2, activities.Length);

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;
            AssertDuneLocationCalculationActivity(activities[0], calculationIdentifier, duneLocation1.Name, duneLocation1.Id, targetProbability, hydraulicBoundaryDatabase);
            AssertDuneLocationCalculationActivity(activities[1], calculationIdentifier, duneLocation2.Name, duneLocation2.Id, targetProbability, hydraulicBoundaryDatabase);
        }

        [Test]
        public void CreateCalculationActivitiesForFailureMechanism_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            // Call
            void Call() => DuneLocationCalculationActivityFactory.CreateCalculationActivities(null,
                                                                                              assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void CreateCalculationActivitiesForFailureMechanism_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => DuneLocationCalculationActivityFactory.CreateCalculationActivities(new DuneErosionFailureMechanism(),
                                                                                              null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateCalculationActivitiesForFailureMechanism_WithValidDataAndUsePreprocessorStates_ReturnsExpectedActivities(bool usePreprocessor)
        {
            // Setup
            AssessmentSectionStub assessmentSection = CreateAssessmentSection(usePreprocessor);

            var duneLocationCalculationsForTargetProbability1 = new DuneLocationCalculationsForTargetProbability(0.1);
            var duneLocationCalculationsForTargetProbability2 = new DuneLocationCalculationsForTargetProbability(0.01);

            var failureMechanism = new DuneErosionFailureMechanism
            {
                DuneLocationCalculationsForUserDefinedTargetProbabilities =
                {
                    duneLocationCalculationsForTargetProbability1,
                    duneLocationCalculationsForTargetProbability2
                }
            };

            var duneLocation1 = new DuneLocation(1, "locationName1", new Point2D(1, 1), new DuneLocation.ConstructionProperties());
            var duneLocation2 = new DuneLocation(2, "locationName2", new Point2D(2, 2), new DuneLocation.ConstructionProperties());

            failureMechanism.SetDuneLocations(new[]
            {
                duneLocation1,
                duneLocation2
            });

            // Call
            CalculatableActivity[] activities = DuneLocationCalculationActivityFactory.CreateCalculationActivities(failureMechanism, assessmentSection).ToArray();

            // Assert
            Assert.AreEqual(4, activities.Length);

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;

            AssertDuneLocationCalculationActivity(activities[0],
                                                  "1/10",
                                                  duneLocation1.Name,
                                                  duneLocation1.Id,
                                                  duneLocationCalculationsForTargetProbability1.TargetProbability,
                                                  hydraulicBoundaryDatabase);
            AssertDuneLocationCalculationActivity(activities[1],
                                                  "1/10",
                                                  duneLocation2.Name,
                                                  duneLocation2.Id,
                                                  duneLocationCalculationsForTargetProbability1.TargetProbability,
                                                  hydraulicBoundaryDatabase);

            AssertDuneLocationCalculationActivity(activities[2],
                                                  "1/100",
                                                  duneLocation1.Name,
                                                  duneLocation1.Id,
                                                  duneLocationCalculationsForTargetProbability2.TargetProbability,
                                                  hydraulicBoundaryDatabase);
            AssertDuneLocationCalculationActivity(activities[3],
                                                  "1/100",
                                                  duneLocation2.Name,
                                                  duneLocation2.Id,
                                                  duneLocationCalculationsForTargetProbability2.TargetProbability,
                                                  hydraulicBoundaryDatabase);
        }

        private static AssessmentSectionStub CreateAssessmentSection(bool usePreprocessor)
        {
            var assessmentSection = new AssessmentSectionStub
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = validFilePath,
                    HydraulicLocationConfigurationSettings =
                    {
                        CanUsePreprocessor = true,
                        UsePreprocessor = usePreprocessor,
                        PreprocessorDirectory = validPreprocessorDirectory
                    }
                }
            };

            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);

            return assessmentSection;
        }

        private static void AssertDuneLocationCalculationActivity(Activity activity,
                                                                  string calculationIdentifier,
                                                                  string locationName,
                                                                  long locationId,
                                                                  double targetProbability,
                                                                  HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            var calculator = new TestDunesBoundaryConditionsCalculator();

            var mocks = new MockRepository();
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();

            calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(hydraulicBoundaryDatabase),
                                     (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(calculator);
            mocks.ReplayAll();

            void Call()
            {
                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                {
                    activity.Run();
                }
            }

            var expectedLogMessage = $"Hydraulische belastingen berekenen voor locatie '{locationName}' ({calculationIdentifier}) is gestart.";

            TestHelper.AssertLogMessageIsGenerated(Call, expectedLogMessage);
            DunesBoundaryConditionsCalculationInput dunesBoundaryConditionsCalculationInput = calculator.ReceivedInputs.Last();
            Assert.AreEqual(locationId, dunesBoundaryConditionsCalculationInput.HydraulicBoundaryLocationId);
            Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(targetProbability), dunesBoundaryConditionsCalculationInput.Beta);

            mocks.VerifyAll();
        }
    }
}