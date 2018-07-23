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
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Data.TestUtil;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input.Structures;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;

namespace Ringtoets.ClosingStructures.Service.Test
{
    [TestFixture]
    public class ClosingStructuresCalculationActivityFactoryTest
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
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                       mocks,
                                                                                                       validFilePath);
            mocks.ReplayAll();

            StructuresCalculation<ClosingStructuresInput> calculation = CreateValidCalculation();

            // Call
            CalculatableActivity activity = ClosingStructuresCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                  failureMechanism,
                                                                                                                  assessmentSection);

            // Assert
            Assert.IsInstanceOf<ClosingStructuresCalculationActivity>(activity);
            AssertClosingStructuresCalculationActivity(activity, calculation);
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
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                       mocks,
                                                                                                       validFilePath);
            mocks.ReplayAll();

            StructuresCalculation<ClosingStructuresInput> calculation1 = CreateValidCalculation();
            StructuresCalculation<ClosingStructuresInput> calculation2 = CreateValidCalculation();

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

            AssertClosingStructuresCalculationActivity(activities.First(), calculation1);
            AssertClosingStructuresCalculationActivity(activities.ElementAt(1), calculation2);
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
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                       mocks,
                                                                                                       validFilePath);
            mocks.ReplayAll();

            StructuresCalculation<ClosingStructuresInput> calculation1 = CreateValidCalculation();
            StructuresCalculation<ClosingStructuresInput> calculation2 = CreateValidCalculation();

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

            AssertClosingStructuresCalculationActivity(activities.First(), calculation1);
            AssertClosingStructuresCalculationActivity(activities.ElementAt(1), calculation2);
            mocks.VerifyAll();
        }

        private static StructuresCalculation<ClosingStructuresInput> CreateValidCalculation()
        {
            return new TestClosingStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "name", 2, 2),
                    FailureProbabilityStructureWithErosion = new Random(39).NextDouble()
                }
            };
        }

        private static void AssertClosingStructuresCalculationActivity(Activity activity,
                                                                       ICalculation<ClosingStructuresInput> calculation)
        {
            var mocks = new MockRepository();
            var testCalculator = new TestStructuresCalculator<StructuresClosureCalculationInput>();
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresClosureCalculationInput>(testDataPath, ""))
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