// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using Core.Common.TestUtil;
using NSubstitute;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.GrassCoverErosionOutwards.Forms.PresentationObjects;

namespace Riskeer.GrassCoverErosionOutwards.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveConditionsCalculationContextTest
    {
        [Test]
        public void ConstructorWithData_Always_ExpectedPropertiesSet()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var parent = new CalculationGroup();

            // Call
            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation, parent, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<GrassCoverErosionOutwardsContext<GrassCoverErosionOutwardsWaveConditionsCalculation>>(context);
            Assert.IsInstanceOf<ICalculationContext<GrassCoverErosionOutwardsWaveConditionsCalculation, GrassCoverErosionOutwardsFailureMechanism>>(context);
            Assert.AreSame(calculation, context.WrappedData);
            Assert.AreSame(parent, context.Parent);
            Assert.AreSame(failureMechanism, context.FailureMechanism);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
        }

        [Test]
        public void ParameteredConstructor_ParentNull_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            TestDelegate call = () => new GrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation, null, failureMechanism, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("parent", exception.ParamName);
        }

        [TestFixture]
        private class GrassCoverErosionOutwardsCalculationContextEqualsTest
            : EqualsTestFixture<GrassCoverErosionOutwardsWaveConditionsCalculationContext,
                DerivedGrassCoverErosionOutwardsWaveConditionsCalculationContext>
        {
            private static readonly IAssessmentSection assessmentSection = Substitute.For<IAssessmentSection>();
            private static readonly GrassCoverErosionOutwardsWaveConditionsCalculation calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            private static readonly GrassCoverErosionOutwardsFailureMechanism failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            private static readonly CalculationGroup parent = new CalculationGroup();

            protected override GrassCoverErosionOutwardsWaveConditionsCalculationContext CreateObject()
            {
                return new GrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation, parent, failureMechanism, assessmentSection);
            }

            protected override DerivedGrassCoverErosionOutwardsWaveConditionsCalculationContext CreateDerivedObject()
            {
                return new DerivedGrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation, parent, failureMechanism, assessmentSection);
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                yield return new TestCaseData(new GrassCoverErosionOutwardsWaveConditionsCalculationContext(new GrassCoverErosionOutwardsWaveConditionsCalculation(),
                                                                                                            parent,
                                                                                                            failureMechanism,
                                                                                                            assessmentSection))
                    .SetName("Calculation");
                yield return new TestCaseData(new GrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation,
                                                                                                            new CalculationGroup(),
                                                                                                            failureMechanism,
                                                                                                            assessmentSection))
                    .SetName("Parent");
            }
        }

        private class DerivedGrassCoverErosionOutwardsWaveConditionsCalculationContext : GrassCoverErosionOutwardsWaveConditionsCalculationContext
        {
            public DerivedGrassCoverErosionOutwardsWaveConditionsCalculationContext(GrassCoverErosionOutwardsWaveConditionsCalculation calculation,
                                                                                    CalculationGroup parent,
                                                                                    GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                                                                    IAssessmentSection assessmentSection)
                : base(calculation, parent, failureMechanism, assessmentSection) {}
        }
    }
}