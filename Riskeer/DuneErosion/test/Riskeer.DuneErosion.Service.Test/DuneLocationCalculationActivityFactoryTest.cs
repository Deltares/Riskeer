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
using System.IO;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Base.Service;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
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
                norm,
                categoryBoundaryName).ToArray();

            // Assert
            CollectionAssert.AllItemsAreInstancesOfType(activities, typeof(DuneLocationCalculationActivity));
            Assert.AreEqual(2, activities.Length);

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;
            AssertDuneLocationCalculationActivity(activities[0], categoryBoundaryName, duneLocation1.Name, duneLocation1.Id, norm, hydraulicBoundaryDatabase);
            AssertDuneLocationCalculationActivity(activities[1], categoryBoundaryName, duneLocation2.Name, duneLocation2.Id, norm, hydraulicBoundaryDatabase);
        }

        [Test]
        public void CreateCalculationActivitiesForFailureMechanism_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
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
        [TestCase(true)]
        [TestCase(false)]
        public void CreateCalculationActivitiesForFailureMechanism_WithValidDataAndUsePreprocessorStates_ReturnsExpectedActivities(bool usePreprocessor)
        {
            // Setup
            AssessmentSectionStub assessmentSection = CreateAssessmentSection(usePreprocessor);

            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = 5
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
            Assert.AreEqual(10, activities.Length);

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;
            double mechanismSpecificFactorizedSignalingNorm = failureMechanism.GetNorm(assessmentSection, FailureMechanismCategoryType.MechanismSpecificFactorizedSignalingNorm);
            AssertDuneLocationCalculationActivity(activities[0],
                                                  "Iv",
                                                  duneLocation1.Name,
                                                  duneLocation1.Id,
                                                  mechanismSpecificFactorizedSignalingNorm,
                                                  hydraulicBoundaryDatabase);
            AssertDuneLocationCalculationActivity(activities[1],
                                                  "Iv",
                                                  duneLocation2.Name,
                                                  duneLocation2.Id,
                                                  mechanismSpecificFactorizedSignalingNorm,
                                                  hydraulicBoundaryDatabase);

            double mechanismSpecificSignalingNorm = failureMechanism.GetNorm(assessmentSection, FailureMechanismCategoryType.MechanismSpecificSignalingNorm);
            AssertDuneLocationCalculationActivity(activities[2],
                                                  "IIv",
                                                  duneLocation1.Name,
                                                  duneLocation1.Id,
                                                  mechanismSpecificSignalingNorm,
                                                  hydraulicBoundaryDatabase);
            AssertDuneLocationCalculationActivity(activities[3],
                                                  "IIv",
                                                  duneLocation2.Name,
                                                  duneLocation2.Id,
                                                  mechanismSpecificSignalingNorm,
                                                  hydraulicBoundaryDatabase);

            double mechanismSpecificLowerLimitNorm = failureMechanism.GetNorm(assessmentSection, FailureMechanismCategoryType.MechanismSpecificLowerLimitNorm);
            AssertDuneLocationCalculationActivity(activities[4],
                                                  "IIIv",
                                                  duneLocation1.Name,
                                                  duneLocation1.Id,
                                                  mechanismSpecificLowerLimitNorm,
                                                  hydraulicBoundaryDatabase);
            AssertDuneLocationCalculationActivity(activities[5],
                                                  "IIIv",
                                                  duneLocation2.Name,
                                                  duneLocation2.Id,
                                                  mechanismSpecificLowerLimitNorm,
                                                  hydraulicBoundaryDatabase);

            double lowerLimitNorm = failureMechanism.GetNorm(assessmentSection, FailureMechanismCategoryType.LowerLimitNorm);
            AssertDuneLocationCalculationActivity(activities[6],
                                                  "IVv",
                                                  duneLocation1.Name,
                                                  duneLocation1.Id,
                                                  lowerLimitNorm,
                                                  hydraulicBoundaryDatabase);
            AssertDuneLocationCalculationActivity(activities[7],
                                                  "IVv",
                                                  duneLocation2.Name,
                                                  duneLocation2.Id,
                                                  lowerLimitNorm,
                                                  hydraulicBoundaryDatabase);

            double factorizedLowerLimitNorm = failureMechanism.GetNorm(assessmentSection, FailureMechanismCategoryType.FactorizedLowerLimitNorm);
            AssertDuneLocationCalculationActivity(activities[8],
                                                  "Vv",
                                                  duneLocation1.Name,
                                                  duneLocation1.Id,
                                                  factorizedLowerLimitNorm,
                                                  hydraulicBoundaryDatabase);
            AssertDuneLocationCalculationActivity(activities[9],
                                                  "Vv",
                                                  duneLocation2.Name,
                                                  duneLocation2.Id,
                                                  factorizedLowerLimitNorm,
                                                  hydraulicBoundaryDatabase);
        }

        private static AssessmentSectionStub CreateAssessmentSection(bool usePreprocessor)
        {
            var assessmentSection = new AssessmentSectionStub();

            assessmentSection.HydraulicBoundaryDatabase.FilePath = validFilePath;
            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.CanUsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.UsePreprocessor = usePreprocessor;
            assessmentSection.HydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.PreprocessorDirectory = validPreprocessorDirectory;
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);

            return assessmentSection;
        }

        private static void AssertDuneLocationCalculationActivity(Activity activity,
                                                                  string categoryBoundaryName,
                                                                  string locationName,
                                                                  long locationId,
                                                                  double norm,
                                                                  HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            var calculator = new TestDunesBoundaryConditionsCalculator();

            var mocks = new MockRepository();
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();

            calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(new AssessmentSectionStub(), new TestHydraulicBoundaryLocation()),
                                     (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(calculator);
            mocks.ReplayAll();

            Action call = () =>
            {
                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                {
                    activity.Run();
                }
            };

            string expectedLogMessage = $"Hydraulische belastingen berekenen voor locatie '{locationName}' (Categoriegrens {categoryBoundaryName}) is gestart.";

            TestHelper.AssertLogMessageIsGenerated(call, expectedLogMessage);
            DunesBoundaryConditionsCalculationInput dunesBoundaryConditionsCalculationInput = calculator.ReceivedInputs.Last();
            Assert.AreEqual(locationId, dunesBoundaryConditionsCalculationInput.HydraulicBoundaryLocationId);
            Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(norm), dunesBoundaryConditionsCalculationInput.Beta);

            mocks.VerifyAll();
        }
    }
}