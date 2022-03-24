﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System.Collections.Generic;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.StabilityStoneCover.Forms.PresentationObjects;

namespace Riskeer.StabilityStoneCover.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class StabilityStoneCoverCalculationGroupContextTest
    {
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ParameteredConstructor_ExpectedValues(bool hasParent)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var calculationGroup = new CalculationGroup();

            CalculationGroup parent = hasParent ? new CalculationGroup() : null;

            // Call
            var context = new StabilityStoneCoverCalculationGroupContext(calculationGroup, parent, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<StabilityStoneCoverContext<CalculationGroup>>(context);
            Assert.IsInstanceOf<ICalculationContext<CalculationGroup, StabilityStoneCoverFailureMechanism>>(context);
            Assert.AreSame(calculationGroup, context.WrappedData);
            Assert.AreSame(parent, context.Parent);
            Assert.AreSame(failureMechanism, context.FailureMechanism);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
            mocks.VerifyAll();
        }

        [TestFixture(true)]
        [TestFixture(false)]
        private class StabilityStoneCoverCalculationGroupContextEqualsTest
            : EqualsTestFixture<StabilityStoneCoverCalculationGroupContext,
                DerivedStabilityStoneCoverCalculationGroupContext>
        {
            private static readonly MockRepository mocks = new MockRepository();

            private static readonly IAssessmentSection assessmentSection = mocks.Stub<IAssessmentSection>();
            private static readonly StabilityStoneCoverFailureMechanism failureMechanism = new StabilityStoneCoverFailureMechanism();
            private static readonly CalculationGroup calculationGroup = new CalculationGroup();

            private static CalculationGroup parent;

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

            public StabilityStoneCoverCalculationGroupContextEqualsTest(bool hasParent)
            {
                parent = hasParent ? new CalculationGroup() : null;
            }

            protected override StabilityStoneCoverCalculationGroupContext CreateObject()
            {
                return new StabilityStoneCoverCalculationGroupContext(calculationGroup,
                                                                      parent,
                                                                      failureMechanism,
                                                                      assessmentSection);
            }

            protected override DerivedStabilityStoneCoverCalculationGroupContext CreateDerivedObject()
            {
                return new DerivedStabilityStoneCoverCalculationGroupContext(calculationGroup,
                                                                             parent,
                                                                             failureMechanism,
                                                                             assessmentSection);
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                yield return new TestCaseData(new StabilityStoneCoverCalculationGroupContext(new CalculationGroup(),
                                                                                             parent,
                                                                                             failureMechanism,
                                                                                             assessmentSection))
                    .SetName("Wrapped Calculation Group");
                yield return new TestCaseData(new StabilityStoneCoverCalculationGroupContext(calculationGroup,
                                                                                             new CalculationGroup(),
                                                                                             failureMechanism,
                                                                                             assessmentSection))
                    .SetName("Parent");
            }
        }

        private class DerivedStabilityStoneCoverCalculationGroupContext : StabilityStoneCoverCalculationGroupContext
        {
            public DerivedStabilityStoneCoverCalculationGroupContext(CalculationGroup calculationGroup,
                                                                     CalculationGroup parent,
                                                                     StabilityStoneCoverFailureMechanism failureMechanism,
                                                                     IAssessmentSection assessmentSection)
                : base(calculationGroup, parent, failureMechanism, assessmentSection) {}
        }
    }
}