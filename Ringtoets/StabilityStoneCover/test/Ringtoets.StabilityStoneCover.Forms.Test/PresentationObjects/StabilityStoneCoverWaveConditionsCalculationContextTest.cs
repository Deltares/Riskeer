﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Forms.PresentationObjects;

namespace Ringtoets.StabilityStoneCover.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class StabilityStoneCoverWaveConditionsCalculationContextTest
    {
        [Test]
        public void ConstructorWithData_Always_ExpectedPropertiesSet()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new StabilityStoneCoverWaveConditionsCalculation();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var parent = new CalculationGroup();

            // Call
            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation, parent, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<StabilityStoneCoverContext<StabilityStoneCoverWaveConditionsCalculation>>(context);
            Assert.IsInstanceOf<ICalculationContext<StabilityStoneCoverWaveConditionsCalculation, StabilityStoneCoverFailureMechanism>>(context);
            Assert.AreSame(calculation, context.WrappedData);
            Assert.AreSame(parent, context.Parent);
            Assert.AreSame(failureMechanism, context.FailureMechanism);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
            mocks.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_ParentNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var calculation = new StabilityStoneCoverWaveConditionsCalculation();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Call
            TestDelegate call = () => new StabilityStoneCoverWaveConditionsCalculationContext(calculation, null, failureMechanism, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("parent", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [TestFixture]
        private class StabilityStoneCoverWaveConditionsCalculationContextEqualsTest
            : EqualsTestFixture<StabilityStoneCoverWaveConditionsCalculationContext,
                DerivedStabilityStoneCoverWaveConditionsCalculationContext>
        {
            private static readonly MockRepository mocks = new MockRepository();

            private static readonly IAssessmentSection assessmentSection = mocks.Stub<IAssessmentSection>();
            private static readonly StabilityStoneCoverWaveConditionsCalculation calculation = new StabilityStoneCoverWaveConditionsCalculation();
            private static readonly StabilityStoneCoverFailureMechanism failureMechanism = new StabilityStoneCoverFailureMechanism();
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

            protected override StabilityStoneCoverWaveConditionsCalculationContext CreateObject()
            {
                return new StabilityStoneCoverWaveConditionsCalculationContext(calculation, parent, failureMechanism, assessmentSection);
            }

            protected override DerivedStabilityStoneCoverWaveConditionsCalculationContext CreateDerivedObject()
            {
                return new DerivedStabilityStoneCoverWaveConditionsCalculationContext(calculation, parent, failureMechanism, assessmentSection);
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                yield return new TestCaseData(new StabilityStoneCoverWaveConditionsCalculationContext(new StabilityStoneCoverWaveConditionsCalculation(),
                                                                                                      parent,
                                                                                                      failureMechanism,
                                                                                                      assessmentSection))
                    .SetName("Calculation");
                yield return new TestCaseData(new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
                                                                                                      new CalculationGroup(),
                                                                                                      failureMechanism,
                                                                                                      assessmentSection))
                    .SetName("Parent");
            }
        }

        private class DerivedStabilityStoneCoverWaveConditionsCalculationContext : StabilityStoneCoverWaveConditionsCalculationContext
        {
            public DerivedStabilityStoneCoverWaveConditionsCalculationContext(StabilityStoneCoverWaveConditionsCalculation wrappedData,
                                                                              CalculationGroup parent,
                                                                              StabilityStoneCoverFailureMechanism failureMechanism,
                                                                              IAssessmentSection assessmentSection)
                : base(wrappedData, parent, failureMechanism, assessmentSection) {}
        }
    }
}