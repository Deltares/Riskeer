// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.WaveImpactAsphaltCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Forms.PresentationObjects;

namespace Ringtoets.WaveImpactAsphaltCover.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class WaveImpactAsphaltCoverWaveConditionsCalculationContextTest
    {
        [Test]
        public void ConstructorWithData_Always_ExpectedPropertiesSet()
        {
            // Setup
            var mocksRepository = new MockRepository();
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            mocksRepository.ReplayAll();

            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            var parent = new CalculationGroup();

            // Call
            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationContext(calculation, parent, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<WaveImpactAsphaltCoverContext<WaveImpactAsphaltCoverWaveConditionsCalculation>>(context);
            Assert.IsInstanceOf<ICalculationContext<WaveImpactAsphaltCoverWaveConditionsCalculation, WaveImpactAsphaltCoverFailureMechanism>>(context);
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

            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            // Call
            TestDelegate call = () => new WaveImpactAsphaltCoverWaveConditionsCalculationContext(calculation, null, failureMechanism, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("parent", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [TestFixture]
        private class WaveImpactAsphaltCoverWaveConditionsCalculationContextEqualsTest
            : EqualsTestFixture<WaveImpactAsphaltCoverWaveConditionsCalculationContext,
                DerivedWaveImpactAsphaltCoverWaveConditionsCalculationContext>
        {
            private static readonly MockRepository mocks = new MockRepository();

            private static readonly IAssessmentSection assessmentSection = mocks.Stub<IAssessmentSection>();
            private static readonly WaveImpactAsphaltCoverWaveConditionsCalculation calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            private static readonly WaveImpactAsphaltCoverFailureMechanism failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
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

            protected override WaveImpactAsphaltCoverWaveConditionsCalculationContext CreateObject()
            {
                return new WaveImpactAsphaltCoverWaveConditionsCalculationContext(calculation, parent, failureMechanism, assessmentSection);
            }

            protected override DerivedWaveImpactAsphaltCoverWaveConditionsCalculationContext CreateDerivedObject()
            {
                return new DerivedWaveImpactAsphaltCoverWaveConditionsCalculationContext(calculation, parent, failureMechanism, assessmentSection);
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                yield return new TestCaseData(new WaveImpactAsphaltCoverWaveConditionsCalculationContext(new WaveImpactAsphaltCoverWaveConditionsCalculation(),
                                                                                                         parent,
                                                                                                         failureMechanism,
                                                                                                         assessmentSection))
                    .SetName("Calculation");
                yield return new TestCaseData(new WaveImpactAsphaltCoverWaveConditionsCalculationContext(calculation,
                                                                                                         new CalculationGroup(),
                                                                                                         failureMechanism,
                                                                                                         assessmentSection))
                    .SetName("Parent");
            }
        }

        private class DerivedWaveImpactAsphaltCoverWaveConditionsCalculationContext : WaveImpactAsphaltCoverWaveConditionsCalculationContext
        {
            public DerivedWaveImpactAsphaltCoverWaveConditionsCalculationContext(WaveImpactAsphaltCoverWaveConditionsCalculation calculation,
                                                                                 CalculationGroup parent,
                                                                                 WaveImpactAsphaltCoverFailureMechanism failureMechanism,
                                                                                 IAssessmentSection assessmentSection)
                : base(calculation, parent, failureMechanism, assessmentSection) {}
        }
    }
}