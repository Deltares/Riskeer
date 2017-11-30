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
using Core.Common.TestUtil;
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

            var calculation = new TestStructuresCalculation();
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

            var calculation = new TestStructuresCalculation();
            var failureMechanism = new TestFailureMechanism();

            // Call
            TestDelegate call = () => new TestStructuresCalculationContext(calculation, null, failureMechanism, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("parent", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [TestFixture]
        private class StructuresCalculationContextEqualsTest : EqualsGuidelinesTestFixture<TestStructuresCalculationContext, DerivedTestStructuresCalculationContext>
        {
            private MockRepository mocks;

            private IAssessmentSection assessmentSection;
            private TestStructuresCalculation calculation;
            private TestFailureMechanism failureMechanism;
            private CalculationGroup parent;

            [SetUp]
            public void SetUp()
            {
                calculation = new TestStructuresCalculation();
                failureMechanism = new TestFailureMechanism();
                parent = new CalculationGroup();

                mocks = new MockRepository();
                assessmentSection = mocks.Stub<IAssessmentSection>();
                mocks.ReplayAll();
            }

            [TearDown]
            public void TearDown()
            {
                mocks.VerifyAll();
            }

            [Test]
            public void Equals_ToOtherWithDifferentWrappedData_ReturnFalse()
            {
                // Setup
                var differentCalculation = new TestStructuresCalculation();
                var context1 = new TestStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);
                var context2 = new TestStructuresCalculationContext(differentCalculation, parent, failureMechanism, assessmentSection);

                // Precondition:
                Assert.IsFalse(calculation.Equals(differentCalculation));

                // Call
                bool isEqual1 = context1.Equals(context2);
                bool isEqual2 = context2.Equals(context1);

                // Assert
                Assert.IsFalse(isEqual1);
                Assert.IsFalse(isEqual2);
            }

            [Test]
            public void Equals_ToOtherWithDifferentParent_ReturnFalse()
            {
                // Setup
                var differentParent = new CalculationGroup();
                var context1 = new TestStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);
                var context2 = new TestStructuresCalculationContext(calculation, differentParent, failureMechanism, assessmentSection);

                // Precondition:
                Assert.IsFalse(parent.Equals(differentParent));

                // Call
                bool isEqual1 = context1.Equals(context2);
                bool isEqual2 = context2.Equals(context1);

                // Assert
                Assert.IsFalse(isEqual1);
                Assert.IsFalse(isEqual2);
            }

            protected override TestStructuresCalculationContext CreateObject()
            {
                return new TestStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);
            }

            protected override DerivedTestStructuresCalculationContext CreateDerivedObject()
            {
                return new DerivedTestStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                yield break;
            }
        }

        private class TestStructuresCalculationContext : StructuresCalculationContext<TestStructuresInput, TestFailureMechanism>
        {
            public TestStructuresCalculationContext(StructuresCalculation<TestStructuresInput> calculation,
                                                    CalculationGroup parent,
                                                    TestFailureMechanism failureMechanism,
                                                    IAssessmentSection assessmentSection)
                : base(calculation, parent, failureMechanism, assessmentSection) {}
        }

        private class DerivedTestStructuresCalculationContext : TestStructuresCalculationContext
        {
            public DerivedTestStructuresCalculationContext(StructuresCalculation<TestStructuresInput> calculation,
                                                           CalculationGroup parent,
                                                           TestFailureMechanism failureMechanism,
                                                           IAssessmentSection assessmentSection)
                : base(calculation, parent, failureMechanism, assessmentSection) {}
        }
    }
}