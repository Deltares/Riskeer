﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Rhino.Mocks;
using Riskeer.ClosingStructures.Data;
using Riskeer.ClosingStructures.Data.TestUtil;
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

namespace Riskeer.ClosingStructures.Service.Test
{
    [TestFixture]
    public class ClosingStructuresCalculationActivityFactoryTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Service, "HydraRingCalculation");
        private static readonly string validHrdFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

        [Test]
        public void CreateCalculationActivity_CalculationNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => ClosingStructuresCalculationActivityFactory.CreateCalculationActivity(null,
                                                                                                            new ClosingStructuresFailureMechanism(),
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
            TestDelegate test = () => ClosingStructuresCalculationActivityFactory.CreateCalculationActivity(new StructuresCalculation<ClosingStructuresInput>(),
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
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var failureMechanism = new ClosingStructuresFailureMechanism();

            // Call
            TestDelegate test = () => ClosingStructuresCalculationActivityFactory.CreateCalculationActivity(calculation, failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateCalculationActivity_WithValidCalculation_ReturnsClosingStructuresCalculationActivityWithParametersSet()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mocks,
                                                                                                           validHrdFilePath);
            mocks.ReplayAll();

            StructuresCalculation<ClosingStructuresInput> calculation = CreateValidCalculation(assessmentSection.HydraulicBoundaryData.GetLocations().First());

            // Call
            CalculatableActivity activity = ClosingStructuresCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                  failureMechanism,
                                                                                                                  assessmentSection);

            // Assert
            Assert.IsInstanceOf<ClosingStructuresCalculationActivity>(activity);
            AssertClosingStructuresCalculationActivity(activity, calculation, assessmentSection.HydraulicBoundaryData);
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
            TestDelegate test = () => ClosingStructuresCalculationActivityFactory.CreateCalculationActivities(null,
                                                                                                              new ClosingStructuresFailureMechanism(),
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
            TestDelegate test = () => ClosingStructuresCalculationActivityFactory.CreateCalculationActivities(new CalculationGroup(),
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
            TestDelegate test = () => ClosingStructuresCalculationActivityFactory.CreateCalculationActivities(new CalculationGroup(),
                                                                                                              new ClosingStructuresFailureMechanism(),
                                                                                                              null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateCalculationActivitiesForCalculationGroup_WithValidCalculations_ReturnsClosingStructuresCalculationActivitiesWithParametersSet()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mocks,
                                                                                                           validHrdFilePath);
            mocks.ReplayAll();

            HydraulicBoundaryLocation hydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.GetLocations().First();
            
            StructuresCalculation<ClosingStructuresInput> calculation1 = CreateValidCalculation(hydraulicBoundaryLocation);
            StructuresCalculation<ClosingStructuresInput> calculation2 = CreateValidCalculation(hydraulicBoundaryLocation);

            var calculations = new CalculationGroup
            {
                Children =
                {
                    calculation1,
                    calculation2
                }
            };

            // Call
            IEnumerable<CalculatableActivity> activities = ClosingStructuresCalculationActivityFactory.CreateCalculationActivities(
                calculations, failureMechanism, assessmentSection);

            // Assert
            CollectionAssert.AllItemsAreInstancesOfType(activities, typeof(ClosingStructuresCalculationActivity));
            Assert.AreEqual(2, activities.Count());

            HydraulicBoundaryData hydraulicBoundaryData = assessmentSection.HydraulicBoundaryData;
            AssertClosingStructuresCalculationActivity(activities.First(), calculation1, hydraulicBoundaryData);
            AssertClosingStructuresCalculationActivity(activities.ElementAt(1), calculation2, hydraulicBoundaryData);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateCalculationActivitiesForFailureMechanism_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => ClosingStructuresCalculationActivityFactory.CreateCalculationActivities(null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateCalculationActivitiesForFailureMechanism_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ClosingStructuresCalculationActivityFactory.CreateCalculationActivities(new ClosingStructuresFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateCalculationActivitiesForFailureMechanism_WithValidCalculations_ReturnsClosingStructuresCalculationActivitiesWithParametersSet()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mocks,
                                                                                                           validHrdFilePath);
            mocks.ReplayAll();

            HydraulicBoundaryLocation hydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.GetLocations().First();
            
            StructuresCalculation<ClosingStructuresInput> calculation1 = CreateValidCalculation(hydraulicBoundaryLocation);
            StructuresCalculation<ClosingStructuresInput> calculation2 = CreateValidCalculation(hydraulicBoundaryLocation);

            failureMechanism.CalculationsGroup.Children.AddRange(new[]
            {
                calculation1,
                calculation2
            });

            // Call
            IEnumerable<CalculatableActivity> activities = ClosingStructuresCalculationActivityFactory.CreateCalculationActivities(
                failureMechanism, assessmentSection);

            // Assert
            CollectionAssert.AllItemsAreInstancesOfType(activities, typeof(ClosingStructuresCalculationActivity));
            Assert.AreEqual(2, activities.Count());

            HydraulicBoundaryData hydraulicBoundaryData = assessmentSection.HydraulicBoundaryData;
            AssertClosingStructuresCalculationActivity(activities.First(), calculation1, hydraulicBoundaryData);
            AssertClosingStructuresCalculationActivity(activities.ElementAt(1), calculation2, hydraulicBoundaryData);
            mocks.VerifyAll();
        }

        private static StructuresCalculation<ClosingStructuresInput> CreateValidCalculation(HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            return new TestClosingStructuresCalculationScenario
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    FailureProbabilityStructureWithErosion = new Random(39).NextDouble()
                }
            };
        }

        private static void AssertClosingStructuresCalculationActivity(Activity activity,
                                                                       ICalculation<ClosingStructuresInput> calculation,
                                                                       HydraulicBoundaryData hydraulicBoundaryData)
        {
            var mocks = new MockRepository();
            var testCalculator = new TestStructuresCalculator<StructuresClosureCalculationInput>();
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresClosureCalculationInput>(
                                         Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(
                                         hydraulicBoundaryData,
                                         calculation.InputParameters.HydraulicBoundaryLocation),
                                     (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(testCalculator);
            mocks.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                activity.Run();

                StructuresClosureCalculationInput actualInput = testCalculator.ReceivedInputs.Single();
                Assert.AreEqual(calculation.InputParameters.FailureProbabilityStructureWithErosion,
                                actualInput.Variables.Single(v => v.VariableId == 105).Value);
            }

            mocks.VerifyAll();
        }
    }
}