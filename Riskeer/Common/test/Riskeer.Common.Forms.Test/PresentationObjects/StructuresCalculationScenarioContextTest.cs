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
using System.Collections.Generic;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PresentationObjects;

namespace Riskeer.Common.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class StructuresCalculationScenarioContextTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocksRepository = new MockRepository();
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            mocksRepository.ReplayAll();

            var calculation = new TestStructuresCalculationScenario();
            var failureMechanism = new TestFailureMechanism();
            var parent = new CalculationGroup();

            // Call
            var context = new TestStructuresCalculationScenarioContext(calculation, parent, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<FailureMechanismItemContextBase<StructuresCalculationScenario<TestStructuresInput>, TestFailureMechanism>>(context);
            Assert.IsInstanceOf<ICalculationContext<StructuresCalculationScenario<TestStructuresInput>, TestFailureMechanism>>(context);
            Assert.AreSame(calculation, context.WrappedData);
            Assert.AreSame(parent, context.Parent);
            Assert.AreSame(failureMechanism, context.FailureMechanism);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
            mocksRepository.VerifyAll();
        }

        [Test]
        public void Constructor_ParentNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var calculation = new TestStructuresCalculationScenario();
            var failureMechanism = new TestFailureMechanism();

            // Call
            void Call() => new TestStructuresCalculationScenarioContext(calculation, null, failureMechanism, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("parent", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [TestFixture]
        private class StructuresCalculationContextEqualsTest : EqualsTestFixture<TestStructuresCalculationScenarioContext, DerivedTestStructuresCalculationScenarioContext>
        {
            private static readonly MockRepository mocks = new MockRepository();

            private static readonly IAssessmentSection assessmentSection = mocks.Stub<IAssessmentSection>();
            private static readonly TestStructuresCalculationScenario calculation = new TestStructuresCalculationScenario();
            private static readonly TestFailureMechanism failureMechanism = new TestFailureMechanism();
            private static readonly CalculationGroup parent = new CalculationGroup();

            [SetUp]
            public void SetUp()
            {
                mocks.ReplayAll();
            }

            [TearDown]
            public void TearDown()
            {
                mocks.VerifyAll();
            }

            protected override TestStructuresCalculationScenarioContext CreateObject()
            {
                return new TestStructuresCalculationScenarioContext(calculation, parent, failureMechanism, assessmentSection);
            }

            protected override DerivedTestStructuresCalculationScenarioContext CreateDerivedObject()
            {
                return new DerivedTestStructuresCalculationScenarioContext(calculation, parent, failureMechanism, assessmentSection);
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                yield return new TestCaseData(new TestStructuresCalculationScenarioContext(new TestStructuresCalculationScenario(),
                                                                                           parent,
                                                                                           failureMechanism,
                                                                                           assessmentSection))
                    .SetName("Calculation");
                yield return new TestCaseData(new TestStructuresCalculationScenarioContext(calculation,
                                                                                           new CalculationGroup(),
                                                                                           failureMechanism,
                                                                                           assessmentSection))
                    .SetName("Parent");
            }
        }

        private class TestStructuresCalculationScenarioContext : StructuresCalculationScenarioContext<TestStructuresInput, TestFailureMechanism>
        {
            public TestStructuresCalculationScenarioContext(StructuresCalculationScenario<TestStructuresInput> calculation,
                                                            CalculationGroup parent,
                                                            TestFailureMechanism failureMechanism,
                                                            IAssessmentSection assessmentSection)
                : base(calculation, parent, failureMechanism, assessmentSection) {}
        }

        private class DerivedTestStructuresCalculationScenarioContext : TestStructuresCalculationScenarioContext
        {
            public DerivedTestStructuresCalculationScenarioContext(StructuresCalculationScenario<TestStructuresInput> calculation,
                                                                   CalculationGroup parent,
                                                                   TestFailureMechanism failureMechanism,
                                                                   IAssessmentSection assessmentSection)
                : base(calculation, parent, failureMechanism, assessmentSection) {}
        }
    }
}