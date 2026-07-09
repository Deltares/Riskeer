// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
using Core.Common.Base.Service;
using Core.Common.TestUtil;
using NUnit.Framework;
using NSubstitute;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Service;
using Riskeer.Common.Service.TestUtil;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Data.Input.Structures;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityPointStructures.Data.TestUtil;

namespace Riskeer.StabilityPointStructures.Service.Test
{
    [TestFixture]
    public class StabilityPointStructuresCalculationActivityFactoryTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Service, "HydraRingCalculation");
        private static readonly string validHrdFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

        [Test]
        public void CreateCalculationActivity_CalculationNull_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();

            // Call
            TestDelegate test = () => StabilityPointStructuresCalculationActivityFactory.CreateCalculationActivity(null,
                                                                                                                   new StabilityPointStructuresFailureMechanism(),
                                                                                                                   assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void CreateCalculationActivity_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();

            // Call
            TestDelegate test = () => StabilityPointStructuresCalculationActivityFactory.CreateCalculationActivity(new StructuresCalculation<StabilityPointStructuresInput>(),
                                                                                                                   null,
                                                                                                                   assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void CreateCalculationActivity_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            // Call
            TestDelegate test = () => StabilityPointStructuresCalculationActivityFactory.CreateCalculationActivity(calculation, failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateCalculationActivity_WithValidCalculation_ReturnsStabilityPointStructuresCalculationActivityWithParametersSet()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, validHrdFilePath);

            StructuresCalculation<StabilityPointStructuresInput> calculation = CreateValidCalculation(assessmentSection.HydraulicBoundaryData.GetLocations().First());

            // Call
            CalculatableActivity activity = StabilityPointStructuresCalculationActivityFactory.CreateCalculationActivity(calculation, failureMechanism,
                                                                                                                         assessmentSection);

            // Assert
            Assert.IsInstanceOf<StabilityPointStructuresCalculationActivity>(activity);
            AssertStabilityPointStructuresCalculationActivity(activity, calculation, assessmentSection.HydraulicBoundaryData);
        }

        [Test]
        public void CreateCalculationActivitiesForCalculationGroup_CalculationGroupNull_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();

            // Call
            TestDelegate test = () => StabilityPointStructuresCalculationActivityFactory.CreateCalculationActivities(null,
                                                                                                                     new StabilityPointStructuresFailureMechanism(),
                                                                                                                     assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculationGroup", exception.ParamName);
        }

        [Test]
        public void CreateCalculationActivitiesForCalculationGroup_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();

            // Call
            TestDelegate test = () => StabilityPointStructuresCalculationActivityFactory.CreateCalculationActivities(new CalculationGroup(),
                                                                                                                     null,
                                                                                                                     assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void CreateCalculationActivitiesForCalculationGroup_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => StabilityPointStructuresCalculationActivityFactory.CreateCalculationActivities(new CalculationGroup(),
                                                                                                                     new StabilityPointStructuresFailureMechanism(),
                                                                                                                     null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateCalculationActivitiesForCalculationGroup_WithValidCalculations_ReturnsStabilityPointStructuresCalculationActivitiesWithParametersSet()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, validHrdFilePath);

            HydraulicBoundaryLocation hydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.GetLocations().First();

            StructuresCalculation<StabilityPointStructuresInput> calculation1 = CreateValidCalculation(hydraulicBoundaryLocation);
            StructuresCalculation<StabilityPointStructuresInput> calculation2 = CreateValidCalculation(hydraulicBoundaryLocation);

            var calculations = new CalculationGroup
            {
                Children =
                {
                    calculation1,
                    calculation2
                }
            };

            // Call
            IEnumerable<CalculatableActivity> activities = StabilityPointStructuresCalculationActivityFactory.CreateCalculationActivities(
                calculations, failureMechanism, assessmentSection);

            // Assert
            CollectionAssert.AllItemsAreInstancesOfType(activities, typeof(StabilityPointStructuresCalculationActivity));
            Assert.AreEqual(2, activities.Count());

            HydraulicBoundaryData hydraulicBoundaryData = assessmentSection.HydraulicBoundaryData;
            AssertStabilityPointStructuresCalculationActivity(activities.First(), calculation1, hydraulicBoundaryData);
            AssertStabilityPointStructuresCalculationActivity(activities.ElementAt(1), calculation2, hydraulicBoundaryData);
        }

        [Test]
        public void CreateCalculationActivitiesForFailureMechanism_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();

            // Call
            TestDelegate test = () => StabilityPointStructuresCalculationActivityFactory.CreateCalculationActivities(null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void CreateCalculationActivitiesForFailureMechanism_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => StabilityPointStructuresCalculationActivityFactory.CreateCalculationActivities(new StabilityPointStructuresFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateCalculationActivitiesForFailureMechanism_WithValidCalculations_ReturnsStabilityPointStructuresCalculationActivitiesWithParametersSet()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, validHrdFilePath);

            HydraulicBoundaryLocation hydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.GetLocations().First();

            StructuresCalculation<StabilityPointStructuresInput> calculation1 = CreateValidCalculation(hydraulicBoundaryLocation);
            StructuresCalculation<StabilityPointStructuresInput> calculation2 = CreateValidCalculation(hydraulicBoundaryLocation);

            failureMechanism.CalculationsGroup.Children.AddRange(new[]
            {
                calculation1,
                calculation2
            });

            // Call
            IEnumerable<CalculatableActivity> activities = StabilityPointStructuresCalculationActivityFactory.CreateCalculationActivities(
                failureMechanism, assessmentSection);

            // Assert
            CollectionAssert.AllItemsAreInstancesOfType(activities, typeof(StabilityPointStructuresCalculationActivity));
            Assert.AreEqual(2, activities.Count());

            HydraulicBoundaryData hydraulicBoundaryData = assessmentSection.HydraulicBoundaryData;
            AssertStabilityPointStructuresCalculationActivity(activities.First(), calculation1, hydraulicBoundaryData);
            AssertStabilityPointStructuresCalculationActivity(activities.ElementAt(1), calculation2, hydraulicBoundaryData);
        }

        private static StructuresCalculation<StabilityPointStructuresInput> CreateValidCalculation(HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            return new TestStabilityPointStructuresCalculationScenario
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    FailureProbabilityStructureWithErosion = new Random(39).NextDouble()
                }
            };
        }

        private static void AssertStabilityPointStructuresCalculationActivity(Activity activity,
                                                                              ICalculation<StabilityPointStructuresInput> calculation,
                                                                              HydraulicBoundaryData hydraulicBoundaryData)
        {
            var testCalculator = new TestStructuresCalculator<StructuresStabilityPointCalculationInput>();
            var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();
            calculatorFactory.CreateStructuresCalculator<StructuresStabilityPointCalculationInput>(
                Arg.Any<HydraRingCalculationSettings>()).Returns(testCalculator);
            calculatorFactory.When(x => x.CreateStructuresCalculator<StructuresStabilityPointCalculationInput>(
                                       Arg.Any<HydraRingCalculationSettings>()))
                             .Do(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(
                                         hydraulicBoundaryData,
                                         calculation.InputParameters.HydraulicBoundaryLocation),
                                     (HydraRingCalculationSettings) invocation[0]);
                             });

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                activity.Run();

                StructuresStabilityPointCalculationInput actualInput = testCalculator.ReceivedInputs.Single();
                Assert.AreEqual(calculation.InputParameters.FailureProbabilityStructureWithErosion,
                                actualInput.Variables.Single(v => v.VariableId == 105).Value);
            }
        }
    }
}