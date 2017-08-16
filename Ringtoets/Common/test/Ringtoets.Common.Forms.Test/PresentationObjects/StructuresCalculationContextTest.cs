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
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;

namespace Ringtoets.Common.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class StructuresCalculationContextTest
    {
        [Test]
        public void ConstructorWithData_Always_ExpectedPropertiesSet()
        {
            // Setup
            var mocksRepository = new MockRepository();
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            mocksRepository.ReplayAll();

            var calculation = new StructuresCalculation<TestStructuresInput>();
            var failureMechanism = new TestFailureMechanism();
            var parent = new CalculationGroup();

            // Call
            var context = new TestStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<FailureMechanismItemContextBase<StructuresCalculation<TestStructuresInput>, TestFailureMechanism>>(context);
            Assert.IsInstanceOf<ICalculationContext<StructuresCalculation<TestStructuresInput>, TestFailureMechanism>>(context);
            Assert.AreSame(calculation, context.WrappedData);
            Assert.AreSame(parent, context.Parent);
            Assert.AreSame(failureMechanism, context.FailureMechanism);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
            mocksRepository.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_ParentNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var calculation = new StructuresCalculation<TestStructuresInput>();
            var failureMechanism = new TestFailureMechanism();

            // Call
            TestDelegate call = () => new TestStructuresCalculationContext(calculation, null, failureMechanism, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("parent", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Equals_ToNull_ReturnFalse()
        {
            // Setup
            var mocksRepository = new MockRepository();
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            mocksRepository.ReplayAll();

            var calculation = new TestStructuresCalculation();
            var failureMechanism = new TestFailureMechanism();
            var parent = new CalculationGroup();
            var context = new TestStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);

            // Call
            bool isEqual = context.Equals(null);

            // Assert
            Assert.IsFalse(isEqual);

            mocksRepository.VerifyAll();
        }

        [Test]
        public void Equals_ToItself_ReturnTrue()
        {
            // Setup
            var mocksRepository = new MockRepository();
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            mocksRepository.ReplayAll();

            var calculation = new TestStructuresCalculation();
            var failureMechanism = new TestFailureMechanism();
            var parent = new CalculationGroup();
            var context = new TestStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);

            // Call
            bool isEqual = context.Equals(context);

            // Assert
            Assert.IsTrue(isEqual);

            mocksRepository.VerifyAll();
        }

        [Test]
        public void Equals_ToOtherWithDifferentType_ReturnFalse()
        {
            // Setup
            var mocksRepository = new MockRepository();
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            mocksRepository.ReplayAll();

            var calculation = new TestStructuresCalculation();
            var failureMechanism = new TestFailureMechanism();
            var parent = new CalculationGroup();
            var context = new TestStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);

            // Call
            bool isEqual = context.Equals(new object());

            // Assert
            Assert.IsFalse(isEqual);

            mocksRepository.VerifyAll();
        }

        [Test]
        public void Equals_ToOtherWithDifferentWrappedData_ReturnFalse()
        {
            // Setup
            var mocksRepository = new MockRepository();
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            mocksRepository.ReplayAll();

            var calculation1 = new TestStructuresCalculation();
            var calculation2 = new TestStructuresCalculation();
            var failureMechanism = new TestFailureMechanism();
            var parent = new CalculationGroup();
            var context1 = new TestStructuresCalculationContext(calculation1, parent, failureMechanism, assessmentSection);
            var context2 = new TestStructuresCalculationContext(calculation2, parent, failureMechanism, assessmentSection);

            // Precondition:
            Assert.IsFalse(calculation1.Equals(calculation2));

            // Call
            bool isEqual1 = context1.Equals(context2);
            bool isEqual2 = context2.Equals(context1);

            // Assert
            Assert.IsFalse(isEqual1);
            Assert.IsFalse(isEqual2);

            mocksRepository.VerifyAll();
        }

        [Test]
        public void Equals_ToOtherWithDifferentParent_ReturnFalse()
        {
            // Setup
            var mocksRepository = new MockRepository();
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            mocksRepository.ReplayAll();

            var calculation = new TestStructuresCalculation();
            var failureMechanism = new TestFailureMechanism();
            var parent1 = new CalculationGroup();
            var parent2 = new CalculationGroup();
            var context1 = new TestStructuresCalculationContext(calculation, parent1, failureMechanism, assessmentSection);
            var context2 = new TestStructuresCalculationContext(calculation, parent2, failureMechanism, assessmentSection);

            // Precondition:
            Assert.IsFalse(parent1.Equals(parent2));

            // Call
            bool isEqual1 = context1.Equals(context2);
            bool isEqual2 = context2.Equals(context1);

            // Assert
            Assert.IsFalse(isEqual1);
            Assert.IsFalse(isEqual2);

            mocksRepository.VerifyAll();
        }

        [Test]
        public void Equals_ToOtherWithSameWrappedDataAndParent_ReturnTrue()
        {
            // Setup
            var mocksRepository = new MockRepository();
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            mocksRepository.ReplayAll();

            var calculation = new TestStructuresCalculation();
            var failureMechanism = new TestFailureMechanism();
            var parent = new CalculationGroup();
            var context1 = new TestStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);
            var context2 = new TestStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);

            // Call
            bool isEqual1 = context1.Equals(context2);
            bool isEqual2 = context2.Equals(context1);

            // Assert
            Assert.IsTrue(isEqual1);
            Assert.IsTrue(isEqual2);

            mocksRepository.VerifyAll();
        }

        [Test]
        public void GetHashCode_EqualObjects_ReturnSameHashCode()
        {
            // Setup
            var mocksRepository = new MockRepository();
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            mocksRepository.ReplayAll();

            var calculation = new TestStructuresCalculation();
            var failureMechanism = new TestFailureMechanism();
            var parent = new CalculationGroup();
            var context1 = new TestStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);
            var context2 = new TestStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);

            // Precondition:
            Assert.AreEqual(context1, context2);

            // Call
            int hashCode1 = context1.GetHashCode();
            int hashCode2 = context2.GetHashCode();

            // Assert
            Assert.AreEqual(hashCode1, hashCode2);

            mocksRepository.VerifyAll();
        }

        private class TestStructuresCalculationContext : StructuresCalculationContext<TestStructuresInput, TestFailureMechanism>
        {
            public TestStructuresCalculationContext(StructuresCalculation<TestStructuresInput> calculation,
                                                    CalculationGroup parent,
                                                    TestFailureMechanism failureMechanism,
                                                    IAssessmentSection assessmentSection)
                : base(calculation, parent, failureMechanism, assessmentSection) {}
        }
    }
}