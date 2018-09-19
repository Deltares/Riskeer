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
using Core.Common.Base.Service;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Data.TestUtil;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input.Structures;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;

namespace Ringtoets.HeightStructures.Service.Test
{
    [TestFixture]
    public class HeightStructuresCalculationActivityFactoryTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");
        private static readonly string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

        [Test]
        public void CreateCalculationActivity_CalculationNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => HeightStructuresCalculationActivityFactory.CreateCalculationActivity(null,
                                                                                                           new HeightStructuresFailureMechanism(),
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
            TestDelegate test = () => HeightStructuresCalculationActivityFactory.CreateCalculationActivity(new StructuresCalculation<HeightStructuresInput>(),
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
            var calculation = new StructuresCalculation<HeightStructuresInput>();
            var failureMechanism = new HeightStructuresFailureMechanism();

            // Call
            TestDelegate test = () => HeightStructuresCalculationActivityFactory.CreateCalculationActivity(calculation, failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateCalculationActivity_WithValidCalculation_ReturnsHeightStructuresCalculationActivityWithParametersSet()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mocks,
                                                                                                           validFilePath);
            mocks.ReplayAll();

            StructuresCalculation<HeightStructuresInput> calculation = CreateValidCalculation();

            // Call
            CalculatableActivity activity = HeightStructuresCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                 failureMechanism,
                                                                                                                 assessmentSection);

            // Assert
            Assert.IsInstanceOf<HeightStructuresCalculationActivity>(activity);
            AssertHeightStructuresCalculationActivity(activity, calculation);
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
            TestDelegate test = () => HeightStructuresCalculationActivityFactory.CreateCalculationActivities(null,
                                                                                                             new HeightStructuresFailureMechanism(),
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
            TestDelegate test = () => HeightStructuresCalculationActivityFactory.CreateCalculationActivities(new CalculationGroup(),
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
            TestDelegate test = () => HeightStructuresCalculationActivityFactory.CreateCalculationActivities(new CalculationGroup(),
                                                                                                             new HeightStructuresFailureMechanism(),
                                                                                                             null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateCalculationActivitiesForCalculationGroup_WithValidCalculations_ReturnsHeightStructuresCalculationActivitiesWithParametersSet()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mocks,
                                                                                                           validFilePath);
            mocks.ReplayAll();

            StructuresCalculation<HeightStructuresInput> calculation1 = CreateValidCalculation();
            StructuresCalculation<HeightStructuresInput> calculation2 = CreateValidCalculation();

            var calculations = new CalculationGroup
            {
                Children =
                {
                    calculation1,
                    calculation2
                }
            };

            // Call
            IEnumerable<CalculatableActivity> activities = HeightStructuresCalculationActivityFactory.CreateCalculationActivities(
                calculations, failureMechanism, assessmentSection);

            // Assert
            CollectionAssert.AllItemsAreInstancesOfType(activities, typeof(HeightStructuresCalculationActivity));
            Assert.AreEqual(2, activities.Count());

            AssertHeightStructuresCalculationActivity(activities.First(), calculation1);
            AssertHeightStructuresCalculationActivity(activities.ElementAt(1), calculation2);
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
            TestDelegate test = () => HeightStructuresCalculationActivityFactory.CreateCalculationActivities(null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateCalculationActivitiesForFailureMechanism_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => HeightStructuresCalculationActivityFactory.CreateCalculationActivities(new HeightStructuresFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateCalculationActivitiesForFailureMechanism_WithValidCalculations_ReturnsHeightStructuresCalculationActivitiesWithParametersSet()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mocks,
                                                                                                           validFilePath);
            mocks.ReplayAll();

            StructuresCalculation<HeightStructuresInput> calculation1 = CreateValidCalculation();
            StructuresCalculation<HeightStructuresInput> calculation2 = CreateValidCalculation();

            failureMechanism.CalculationsGroup.Children.AddRange(new[]
            {
                calculation1,
                calculation2
            });

            // Call
            IEnumerable<CalculatableActivity> activities = HeightStructuresCalculationActivityFactory.CreateCalculationActivities(
                failureMechanism, assessmentSection);

            // Assert
            CollectionAssert.AllItemsAreInstancesOfType(activities, typeof(HeightStructuresCalculationActivity));
            Assert.AreEqual(2, activities.Count());

            AssertHeightStructuresCalculationActivity(activities.First(), calculation1);
            AssertHeightStructuresCalculationActivity(activities.ElementAt(1), calculation2);
            mocks.VerifyAll();
        }

        private static StructuresCalculation<HeightStructuresInput> CreateValidCalculation()
        {
            return new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    FailureProbabilityStructureWithErosion = new Random(39).NextDouble()
                }
            };
        }

        private static void AssertHeightStructuresCalculationActivity(Activity activity,
                                                                      ICalculation<HeightStructuresInput> calculation)
        {
            var mocks = new MockRepository();
            var testCalculator = new TestStructuresCalculator<StructuresOvertoppingCalculationInput>();
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresOvertoppingCalculationInput>(testDataPath, ""))
                             .Return(testCalculator);
            mocks.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                activity.Run();

                StructuresOvertoppingCalculationInput actualInput = testCalculator.ReceivedInputs.Single();
                Assert.AreEqual(calculation.InputParameters.FailureProbabilityStructureWithErosion,
                                actualInput.Variables.Single(v => v.VariableId == 105).Value);
            }

            mocks.VerifyAll();
        }
    }
}