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
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveConditionsCalculationGroupContextTest
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

            var calculationGroup = new CalculationGroup();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            CalculationGroup parent = hasParent ? new CalculationGroup() : null;

            // Call
            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(calculationGroup, parent, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<GrassCoverErosionOutwardsContext<CalculationGroup>>(context);
            Assert.IsInstanceOf<ICalculationContext<CalculationGroup, GrassCoverErosionOutwardsFailureMechanism>>(context);
            Assert.AreSame(calculationGroup, context.WrappedData);
            Assert.AreSame(parent, context.Parent);
            Assert.AreSame(failureMechanism, context.FailureMechanism);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
            mocks.VerifyAll();
        }

        [TestFixture(true)]
        [TestFixture(false)]
        private class GrassCoverErosionOutwardsWaveConditionsCalculationGroupContextEqualsTest
            : EqualsTestFixture<GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext,
                DerivedGrassCoverErosionOutwardsWaveConditionsCalculationsGroupContext>
        {
            private static readonly MockRepository mocks = new MockRepository();

            private static readonly IAssessmentSection assessmentSection = mocks.Stub<IAssessmentSection>();
            private static readonly GrassCoverErosionOutwardsFailureMechanism failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
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

            public GrassCoverErosionOutwardsWaveConditionsCalculationGroupContextEqualsTest(bool hasParent)
            {
                parent = hasParent ? new CalculationGroup() : null;
            }

            protected override GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext CreateObject()
            {
                return new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(calculationGroup,
                                                                                          parent,
                                                                                          failureMechanism,
                                                                                          assessmentSection);
            }

            protected override DerivedGrassCoverErosionOutwardsWaveConditionsCalculationsGroupContext CreateDerivedObject()
            {
                return new DerivedGrassCoverErosionOutwardsWaveConditionsCalculationsGroupContext(calculationGroup,
                                                                                                  parent,
                                                                                                  failureMechanism,
                                                                                                  assessmentSection);
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                yield return new TestCaseData(new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(new CalculationGroup(),
                                                                                                                 parent,
                                                                                                                 failureMechanism,
                                                                                                                 assessmentSection))
                    .SetName("Wrapped Calculation Group");
                yield return new TestCaseData(new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(calculationGroup,
                                                                                                                 new CalculationGroup(),
                                                                                                                 failureMechanism,
                                                                                                                 assessmentSection))
                    .SetName("Parent");
            }
        }

        private class DerivedGrassCoverErosionOutwardsWaveConditionsCalculationsGroupContext : GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext
        {
            public DerivedGrassCoverErosionOutwardsWaveConditionsCalculationsGroupContext(CalculationGroup calculationGroup,
                                                                                          CalculationGroup parent,
                                                                                          GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                                                                          IAssessmentSection assessmentSection)
                : base(calculationGroup, parent, failureMechanism, assessmentSection) {}
        }
    }
}