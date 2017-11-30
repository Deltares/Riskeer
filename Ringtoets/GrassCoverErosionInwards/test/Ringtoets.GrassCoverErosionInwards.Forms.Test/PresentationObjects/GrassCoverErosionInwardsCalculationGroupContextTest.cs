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

using System.Collections.Generic;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationGroupContextTest
    {
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ParameteredConstructor_ExpectedValues(bool withParent)
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var calculationGroup = new CalculationGroup();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            CalculationGroup parent = withParent ? new CalculationGroup() : null;

            // Call
            var groupContext = new GrassCoverErosionInwardsCalculationGroupContext(calculationGroup, parent, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<GrassCoverErosionInwardsContext<CalculationGroup>>(groupContext);
            Assert.IsInstanceOf<ICalculationContext<CalculationGroup, GrassCoverErosionInwardsFailureMechanism>>(groupContext);
            Assert.AreSame(calculationGroup, groupContext.WrappedData);
            Assert.AreSame(parent, groupContext.Parent);
            Assert.AreSame(failureMechanism, groupContext.FailureMechanism);
            Assert.AreSame(assessmentSection, groupContext.AssessmentSection);
            Assert.AreSame(failureMechanism.DikeProfiles, groupContext.AvailableDikeProfiles);
            mockRepository.VerifyAll();
        }

        [TestFixture]
        private class GrassCoverErosionInwardsCalculationGroupContextEqualsTest
            : EqualsGuidelinesTestFixture<GrassCoverErosionInwardsCalculationGroupContext,
                DerivedGrassCoverErosionInwardsCalculationGroupContext>
        {
            private static readonly MockRepository mocks = new MockRepository();

            private static readonly IAssessmentSection assessmentSection = mocks.Stub<IAssessmentSection>();
            private static readonly GrassCoverErosionInwardsFailureMechanism failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            private static readonly CalculationGroup parent = new CalculationGroup();
            private static readonly CalculationGroup calculationGroup = new CalculationGroup();

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

            protected override GrassCoverErosionInwardsCalculationGroupContext CreateObject()
            {
                return new GrassCoverErosionInwardsCalculationGroupContext(calculationGroup,
                                                                           parent,
                                                                           failureMechanism,
                                                                           assessmentSection);
            }

            protected override DerivedGrassCoverErosionInwardsCalculationGroupContext CreateDerivedObject()
            {
                return new DerivedGrassCoverErosionInwardsCalculationGroupContext(calculationGroup,
                                                                                  parent,
                                                                                  failureMechanism,
                                                                                  assessmentSection);
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                yield return new TestCaseData(new GrassCoverErosionInwardsCalculationGroupContext(new CalculationGroup(),
                                                                                                  parent,
                                                                                                  failureMechanism,
                                                                                                  assessmentSection))
                    .SetName("Wrapped Calculation Group");
                yield return new TestCaseData(new GrassCoverErosionInwardsCalculationGroupContext(calculationGroup,
                                                                                                  new CalculationGroup(),
                                                                                                  failureMechanism,
                                                                                                  assessmentSection))
                    .SetName("Parent");
            }
        }

        private class DerivedGrassCoverErosionInwardsCalculationGroupContext : GrassCoverErosionInwardsCalculationGroupContext
        {
            public DerivedGrassCoverErosionInwardsCalculationGroupContext(CalculationGroup calculationGroup,
                                                                          CalculationGroup parent,
                                                                          GrassCoverErosionInwardsFailureMechanism failureMechanism,
                                                                          IAssessmentSection assessmentSection)
                : base(calculationGroup, parent, failureMechanism, assessmentSection) {}
        }
    }
}