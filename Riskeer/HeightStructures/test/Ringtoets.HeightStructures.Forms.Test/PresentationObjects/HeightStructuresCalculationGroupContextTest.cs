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

using System.Collections.Generic;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Forms.PresentationObjects;

namespace Ringtoets.HeightStructures.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class HeightStructuresCalculationGroupContextTest
    {
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ParameteredConstructor_ExpectedValues(bool hasParent)
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var calculationGroup = new CalculationGroup();
            var failureMechanism = new HeightStructuresFailureMechanism();

            CalculationGroup parent = hasParent ? new CalculationGroup() : null;

            // Call
            var groupContext = new HeightStructuresCalculationGroupContext(calculationGroup, parent, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<FailureMechanismItemContextBase<CalculationGroup, HeightStructuresFailureMechanism>>(groupContext);
            Assert.IsInstanceOf<ICalculationContext<CalculationGroup, HeightStructuresFailureMechanism>>(groupContext);
            Assert.AreSame(calculationGroup, groupContext.WrappedData);
            Assert.AreSame(parent, groupContext.Parent);
            Assert.AreSame(failureMechanism, groupContext.FailureMechanism);
            Assert.AreSame(assessmentSection, groupContext.AssessmentSection);
            Assert.AreSame(failureMechanism.ForeshoreProfiles, groupContext.AvailableForeshoreProfiles);
            Assert.AreSame(failureMechanism.HeightStructures, groupContext.AvailableStructures);
            mockRepository.VerifyAll();
        }

        [TestFixture(true)]
        [TestFixture(false)]
        private class HeightStructuresCalculationGroupContextEqualsTest
            : EqualsTestFixture<HeightStructuresCalculationGroupContext,
                DerivedHeightStructuresCalculationGroupContext>
        {
            private static readonly MockRepository mocks = new MockRepository();

            private static readonly IAssessmentSection assessmentSection = mocks.Stub<IAssessmentSection>();
            private static readonly HeightStructuresFailureMechanism failureMechanism = new HeightStructuresFailureMechanism();
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

            public HeightStructuresCalculationGroupContextEqualsTest(bool hasParent)
            {
                parent = hasParent ? new CalculationGroup() : null;
            }

            protected override HeightStructuresCalculationGroupContext CreateObject()
            {
                return new HeightStructuresCalculationGroupContext(calculationGroup,
                                                                   parent,
                                                                   failureMechanism,
                                                                   assessmentSection);
            }

            protected override DerivedHeightStructuresCalculationGroupContext CreateDerivedObject()
            {
                return new DerivedHeightStructuresCalculationGroupContext(calculationGroup,
                                                                          parent,
                                                                          failureMechanism,
                                                                          assessmentSection);
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                yield return new TestCaseData(new HeightStructuresCalculationGroupContext(new CalculationGroup(),
                                                                                          parent,
                                                                                          failureMechanism,
                                                                                          assessmentSection))
                    .SetName("Wrapped Calculation Group");
                yield return new TestCaseData(new HeightStructuresCalculationGroupContext(calculationGroup,
                                                                                          new CalculationGroup(),
                                                                                          failureMechanism,
                                                                                          assessmentSection))
                    .SetName("Parent");
            }
        }

        private class DerivedHeightStructuresCalculationGroupContext : HeightStructuresCalculationGroupContext
        {
            public DerivedHeightStructuresCalculationGroupContext(CalculationGroup calculationsGroup,
                                                                  CalculationGroup parent,
                                                                  HeightStructuresFailureMechanism failureMechanism,
                                                                  IAssessmentSection assessmentSection)
                : base(calculationsGroup, parent, failureMechanism, assessmentSection) {}
        }
    }
}