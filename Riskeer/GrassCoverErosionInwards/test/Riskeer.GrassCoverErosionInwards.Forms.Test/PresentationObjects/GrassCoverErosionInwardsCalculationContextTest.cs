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
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Forms.PresentationObjects;

namespace Riskeer.GrassCoverErosionInwards.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationContextTest
    {
        [Test]
        public void ConstructorWithData_Always_ExpectedPropertiesSet()
        {
            // Setup
            var mocksRepository = new MockRepository();
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            mocksRepository.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var parent = new CalculationGroup();

            // Call
            var context = new GrassCoverErosionInwardsCalculationContext(calculation, parent, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<GrassCoverErosionInwardsContext<GrassCoverErosionInwardsCalculation>>(context);
            Assert.IsInstanceOf<ICalculationContext<GrassCoverErosionInwardsCalculation, GrassCoverErosionInwardsFailureMechanism>>(context);
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

            var calculation = new GrassCoverErosionInwardsCalculation();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Call
            TestDelegate call = () => new GrassCoverErosionInwardsCalculationContext(calculation, null, failureMechanism, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("parent", exception.ParamName);

            mockRepository.VerifyAll();
        }

        [TestFixture]
        private class GrassCoverErosionInwardsCalculationContextEqualsTest : EqualsTestFixture<GrassCoverErosionInwardsCalculationContext, DerivedGrassCoverErosionInwardsCalculationContext>
        {
            private static readonly MockRepository mocks = new MockRepository();

            private static readonly IAssessmentSection assessmentSection = mocks.Stub<IAssessmentSection>();
            private static readonly GrassCoverErosionInwardsCalculation calculation = new GrassCoverErosionInwardsCalculation();
            private static readonly GrassCoverErosionInwardsFailureMechanism failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
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

            protected override GrassCoverErosionInwardsCalculationContext CreateObject()
            {
                return new GrassCoverErosionInwardsCalculationContext(calculation, parent, failureMechanism, assessmentSection);
            }

            protected override DerivedGrassCoverErosionInwardsCalculationContext CreateDerivedObject()
            {
                return new DerivedGrassCoverErosionInwardsCalculationContext(calculation, parent, failureMechanism, assessmentSection);
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                yield return new TestCaseData(new GrassCoverErosionInwardsCalculationContext(new GrassCoverErosionInwardsCalculation(),
                                                                                             parent,
                                                                                             failureMechanism,
                                                                                             assessmentSection))
                    .SetName("Calculation");
                yield return new TestCaseData(new GrassCoverErosionInwardsCalculationContext(calculation,
                                                                                             new CalculationGroup(),
                                                                                             failureMechanism,
                                                                                             assessmentSection))
                    .SetName("Parent");
            }
        }

        private class DerivedGrassCoverErosionInwardsCalculationContext : GrassCoverErosionInwardsCalculationContext
        {
            public DerivedGrassCoverErosionInwardsCalculationContext(GrassCoverErosionInwardsCalculation calculation,
                                                                     CalculationGroup parent,
                                                                     GrassCoverErosionInwardsFailureMechanism failureMechanism,
                                                                     IAssessmentSection assessmentSection)
                : base(calculation, parent, failureMechanism, assessmentSection) {}
        }
    }
}