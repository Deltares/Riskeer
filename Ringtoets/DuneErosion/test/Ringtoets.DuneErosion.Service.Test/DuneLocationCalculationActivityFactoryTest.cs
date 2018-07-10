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
using System.IO;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Base.Service;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Data.TestUtil;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;

namespace Ringtoets.DuneErosion.Service.Test
{
    [TestFixture]
    public class DuneLocationCalculationActivityFactoryTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");
        private static readonly string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
        private static readonly string validPreprocessorDirectory = TestHelper.GetScratchPadPath();

        [Test]
        public void CreateCalculationActivitiesForCalculations_CalculationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => DuneLocationCalculationActivityFactory.CreateCalculationActivities(null,
                                                                                                         assessmentSection,
                                                                                                         double.NaN,
                                                                                                         "A");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculations", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void CreateCalculationActivitiesForCalculations_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => DuneLocationCalculationActivityFactory.CreateCalculationActivities(Enumerable.Empty<DuneLocationCalculation>(),
                                                                                                         null,
                                                                                                         double.NaN,
                                                                                                         "A");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateCalculationActivitiesForCalculations_WithValidDataAndUsePreprocessorStates_ReturnsExpectedActivities(bool usePreprocessor)
        {
            // Setup
            const double norm = 1.0 / 30;
            const string categoryBoundaryName = "A";

            var calculator = new TestDunesBoundaryConditionsCalculator
            {
                Converged = true
            };

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mocks);
            mocks.ReplayAll();

            ConfigureAssessmentSection(assessmentSection, usePreprocessor);

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
                norm,
                categoryBoundaryName).ToArray();

            // Assert
            CollectionAssert.AllItemsAreInstancesOfType(activities, typeof(DuneLocationCalculationActivity));
            Assert.AreEqual(2, activities.Length);

            AssertDuneLocationCalculationActivity(activities[0], categoryBoundaryName, duneLocation1.Name, duneLocation1.Id, norm, usePreprocessor, calculator);
            AssertDuneLocationCalculationActivity(activities[1], categoryBoundaryName, duneLocation2.Name, duneLocation2.Id, norm, usePreprocessor, calculator);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateCalculationActivitiesForFailureMechanism_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => DuneLocationCalculationActivityFactory.CreateCalculationActivities(null,
                                                                                                         assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void CreateCalculationActivitiesForFailureMechanism_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => DuneLocationCalculationActivityFactory.CreateCalculationActivities(new DuneErosionFailureMechanism(),
                                                                                                         null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateCalculationActivitiesForFailureMechanism_WithValidData_ReturnsExpectedActivities()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = validFilePath
                }
            };

            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = 5
            };

            var duneLocation1 = new TestDuneLocation("locationName 1");
            var duneLocation2 = new TestDuneLocation("locationName 2");

            failureMechanism.SetDuneLocations(new[]
            {
                duneLocation1,
                duneLocation2
            });

            // Call
            CalculatableActivity[] activities = DuneLocationCalculationActivityFactory.CreateCalculationActivities(failureMechanism, assessmentSection).ToArray();

            // Assert
            Assert.AreEqual(10, activities.Length);

            double mechanismSpecificFactorizedSignalingNorm = failureMechanism.GetNorm(assessmentSection, FailureMechanismCategoryType.MechanismSpecificFactorizedSignalingNorm);
            AssertDuneLocationCalculationActivity(activities.First(),
                                                  duneLocation1,
                                                  mechanismSpecificFactorizedSignalingNorm);
            AssertDuneLocationCalculationActivity(activities.ElementAt(1),
                                                  duneLocation2,
                                                  mechanismSpecificFactorizedSignalingNorm);

            double mechanismSpecificSignalingNorm = failureMechanism.GetNorm(assessmentSection, FailureMechanismCategoryType.MechanismSpecificSignalingNorm);
            AssertDuneLocationCalculationActivity(activities.ElementAt(2),
                                                  duneLocation1,
                                                  mechanismSpecificSignalingNorm);
            AssertDuneLocationCalculationActivity(activities.ElementAt(3),
                                                  duneLocation2,
                                                  mechanismSpecificSignalingNorm);

            double mechanismSpecificLowerLimitNorm = failureMechanism.GetNorm(assessmentSection, FailureMechanismCategoryType.MechanismSpecificLowerLimitNorm);
            AssertDuneLocationCalculationActivity(activities.ElementAt(4),
                                                  duneLocation1,
                                                  mechanismSpecificLowerLimitNorm);
            AssertDuneLocationCalculationActivity(activities.ElementAt(5),
                                                  duneLocation2,
                                                  mechanismSpecificLowerLimitNorm);

            double lowerLimitNorm = failureMechanism.GetNorm(assessmentSection, FailureMechanismCategoryType.LowerLimitNorm);
            AssertDuneLocationCalculationActivity(activities.ElementAt(6),
                                                  duneLocation1,
                                                  lowerLimitNorm);
            AssertDuneLocationCalculationActivity(activities.ElementAt(7),
                                                  duneLocation2,
                                                  lowerLimitNorm);

            double factorizedLowerLimitNorm = failureMechanism.GetNorm(assessmentSection, FailureMechanismCategoryType.FactorizedLowerLimitNorm);
            AssertDuneLocationCalculationActivity(activities.ElementAt(8),
                                                  duneLocation1,
                                                  factorizedLowerLimitNorm);
            AssertDuneLocationCalculationActivity(activities.ElementAt(9),
                                                  duneLocation2,
                                                  factorizedLowerLimitNorm);
        }

        private static void AssertDuneLocationCalculationActivity(Activity activity,
                                                                  string categoryBoundaryName,
                                                                  string locationName,
                                                                  long locationId,
                                                                  double norm,
                                                                  bool usePreprocessor,
                                                                  TestDunesBoundaryConditionsCalculator calculator)
        {
            var mocks = new MockRepository();
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(testDataPath, usePreprocessor ? validPreprocessorDirectory : ""))
                             .Return(calculator);
            mocks.ReplayAll();

            Action call = () =>
            {
                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                {
                    activity.Run();
                }
            };

            TestHelper.AssertLogMessages(call, m =>
            {
                string[] messages = m.ToArray();
                Assert.AreEqual(6, messages.Length);
                Assert.AreEqual($"Hydraulische randvoorwaarden berekenen voor locatie '{locationName}' (Categorie {categoryBoundaryName}) is gestart.", messages[0]);
                CalculationServiceTestHelper.AssertValidationStartMessage(messages[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(messages[2]);
                CalculationServiceTestHelper.AssertCalculationStartMessage(messages[3]);
                StringAssert.StartsWith("Hydraulische randvoorwaarden berekening is uitgevoerd op de tijdelijke locatie", messages[4]);
                CalculationServiceTestHelper.AssertCalculationEndMessage(messages[5]);
            });

            DunesBoundaryConditionsCalculationInput dunesBoundaryConditionsCalculationInput = calculator.ReceivedInputs.Last();
            Assert.AreEqual(locationId, dunesBoundaryConditionsCalculationInput.HydraulicBoundaryLocationId);
            Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(norm), dunesBoundaryConditionsCalculationInput.Beta);
            Assert.AreEqual(ActivityState.Executed, activity.State);

            mocks.VerifyAll();
        }

        private static void AssertDuneLocationCalculationActivity(Activity activity,
                                                                  DuneLocation duneLocation,
                                                                  double norm)
        {
            var mocks = new MockRepository();
            var dunesBoundaryConditionsCalculator = new TestDunesBoundaryConditionsCalculator();
            var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(testDataPath, string.Empty)).Return(dunesBoundaryConditionsCalculator);
            mocks.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                activity.Run();

                DunesBoundaryConditionsCalculationInput actualCalculationInput = dunesBoundaryConditionsCalculator.ReceivedInputs.Single();
                Assert.AreEqual(duneLocation.Id, actualCalculationInput.HydraulicBoundaryLocationId);
                Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(norm), actualCalculationInput.Beta);
            }

            mocks.VerifyAll();
        }

        private static void ConfigureAssessmentSection(IAssessmentSection assessmentSection, bool usePreprocessor)
        {
            assessmentSection.HydraulicBoundaryDatabase.CanUsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.UsePreprocessor = usePreprocessor;
            assessmentSection.HydraulicBoundaryDatabase.FilePath = validFilePath;
            assessmentSection.HydraulicBoundaryDatabase.PreprocessorDirectory = validPreprocessorDirectory;
        }
    }
}