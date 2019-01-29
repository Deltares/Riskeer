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
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Forms.PresentationObjects;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationGroupContextTest
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
            var surfaceLines = new[]
            {
                new MacroStabilityInwardsSurfaceLine(string.Empty)
            };
            MacroStabilityInwardsStochasticSoilModel[] soilModels =
            {
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel()
            };

            CalculationGroup parent = hasParent ? new CalculationGroup() : null;

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            // Call
            var groupContext = new MacroStabilityInwardsCalculationGroupContext(calculationGroup,
                                                                                parent,
                                                                                surfaceLines,
                                                                                soilModels,
                                                                                failureMechanism,
                                                                                assessmentSection);

            // Assert
            Assert.IsInstanceOf<MacroStabilityInwardsContext<CalculationGroup>>(groupContext);
            Assert.IsInstanceOf<ICalculationContext<CalculationGroup, MacroStabilityInwardsFailureMechanism>>(groupContext);
            Assert.AreSame(calculationGroup, groupContext.WrappedData);
            Assert.AreSame(parent, groupContext.Parent);
            Assert.AreSame(surfaceLines, groupContext.AvailableMacroStabilityInwardsSurfaceLines);
            Assert.AreSame(soilModels, groupContext.AvailableStochasticSoilModels);
            Assert.AreSame(failureMechanism, groupContext.FailureMechanism);
            Assert.AreSame(assessmentSection, groupContext.AssessmentSection);
            mocks.VerifyAll();
        }

        [TestFixture(true)]
        [TestFixture(false)]
        private class MacroStabilityInwardsCalculationGroupContextEqualsTest
            : EqualsTestFixture<MacroStabilityInwardsCalculationGroupContext, DerivedMacroStabilityInwardsCalculationGroupContext>
        {
            private static readonly MockRepository mocks = new MockRepository();

            private static readonly IAssessmentSection assessmentSection = mocks.Stub<IAssessmentSection>();
            private static readonly IEnumerable<MacroStabilityInwardsSurfaceLine> surfaceLines = new MacroStabilityInwardsSurfaceLine[0];
            private static readonly IEnumerable<MacroStabilityInwardsStochasticSoilModel> stochasticSoilModels = new MacroStabilityInwardsStochasticSoilModel[0];
            private static readonly MacroStabilityInwardsFailureMechanism failureMechanism = new MacroStabilityInwardsFailureMechanism();
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

            public MacroStabilityInwardsCalculationGroupContextEqualsTest(bool hasParent)
            {
                parent = hasParent ? new CalculationGroup() : null;
            }

            protected override MacroStabilityInwardsCalculationGroupContext CreateObject()
            {
                return new MacroStabilityInwardsCalculationGroupContext(calculationGroup,
                                                                        parent,
                                                                        surfaceLines,
                                                                        stochasticSoilModels,
                                                                        failureMechanism,
                                                                        assessmentSection);
            }

            protected override DerivedMacroStabilityInwardsCalculationGroupContext CreateDerivedObject()
            {
                return new DerivedMacroStabilityInwardsCalculationGroupContext(calculationGroup,
                                                                               parent,
                                                                               surfaceLines,
                                                                               stochasticSoilModels,
                                                                               failureMechanism,
                                                                               assessmentSection);
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                yield return new TestCaseData(new MacroStabilityInwardsCalculationGroupContext(new CalculationGroup(),
                                                                                               parent,
                                                                                               surfaceLines,
                                                                                               stochasticSoilModels,
                                                                                               failureMechanism,
                                                                                               assessmentSection))
                    .SetName("Wrapped Calculation Group");
                yield return new TestCaseData(new MacroStabilityInwardsCalculationGroupContext(calculationGroup,
                                                                                               new CalculationGroup(),
                                                                                               surfaceLines,
                                                                                               stochasticSoilModels,
                                                                                               failureMechanism,
                                                                                               assessmentSection))
                    .SetName("Parent");
            }
        }

        private class DerivedMacroStabilityInwardsCalculationGroupContext : MacroStabilityInwardsCalculationGroupContext
        {
            public DerivedMacroStabilityInwardsCalculationGroupContext(CalculationGroup calculationGroup,
                                                                       CalculationGroup parent,
                                                                       IEnumerable<MacroStabilityInwardsSurfaceLine> surfaceLines,
                                                                       IEnumerable<MacroStabilityInwardsStochasticSoilModel> stochasticSoilModels,
                                                                       MacroStabilityInwardsFailureMechanism macroStabilityInwardsFailureMechanism,
                                                                       IAssessmentSection assessmentSection)
                : base(calculationGroup, parent, surfaceLines, stochasticSoilModels, macroStabilityInwardsFailureMechanism, assessmentSection) {}
        }
    }
}