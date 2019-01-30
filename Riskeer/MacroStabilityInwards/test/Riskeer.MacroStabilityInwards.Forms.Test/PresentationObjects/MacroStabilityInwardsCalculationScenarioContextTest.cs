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
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Forms.PresentationObjects;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationScenarioContextTest
    {
        [Test]
        public void ConstructorWithData_Always_ExpectedPropertiesSet()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var surfaceLines = new[]
            {
                new MacroStabilityInwardsSurfaceLine(string.Empty)
            };
            MacroStabilityInwardsStochasticSoilModel[] soilModels =
            {
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel()
            };
            var calculation = new MacroStabilityInwardsCalculationScenario();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var parent = new CalculationGroup();

            // Call
            var presentationObject = new MacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                                         parent,
                                                                                         surfaceLines,
                                                                                         soilModels,
                                                                                         failureMechanism,
                                                                                         assessmentSection);

            // Assert
            Assert.IsInstanceOf<MacroStabilityInwardsContext<MacroStabilityInwardsCalculationScenario>>(presentationObject);
            Assert.IsInstanceOf<ICalculationContext<MacroStabilityInwardsCalculationScenario, MacroStabilityInwardsFailureMechanism>>(presentationObject);
            Assert.AreSame(calculation, presentationObject.WrappedData);
            Assert.AreSame(parent, presentationObject.Parent);
            Assert.AreSame(surfaceLines, presentationObject.AvailableMacroStabilityInwardsSurfaceLines);
            Assert.AreSame(soilModels, presentationObject.AvailableStochasticSoilModels);
            Assert.AreSame(failureMechanism, presentationObject.FailureMechanism);
            Assert.AreSame(assessmentSection, presentationObject.AssessmentSection);
            mocks.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_ParentNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var surfaceLines = new[]
            {
                new MacroStabilityInwardsSurfaceLine(string.Empty)
            };
            MacroStabilityInwardsStochasticSoilModel[] soilModels =
            {
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel()
            };
            var calculation = new MacroStabilityInwardsCalculationScenario();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            // Call
            TestDelegate call = () => new MacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                                          null,
                                                                                          surfaceLines,
                                                                                          soilModels,
                                                                                          failureMechanism,
                                                                                          assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("parent", exception.ParamName);
            mocks.VerifyAll();
        }

        [TestFixture]
        private class MacroStabilityInwardsCalculationScenarioContextEqualsTest
            : EqualsTestFixture<MacroStabilityInwardsCalculationScenarioContext,
                DerivedMacroStabilityInwardsCalculationScenarioContext>
        {
            private static readonly MockRepository mocks = new MockRepository();

            private static readonly IAssessmentSection assessmentSection = mocks.Stub<IAssessmentSection>();
            private static readonly MacroStabilityInwardsCalculationScenario calculation = new MacroStabilityInwardsCalculationScenario();
            private static readonly IEnumerable<MacroStabilityInwardsSurfaceLine> surfaceLines = new MacroStabilityInwardsSurfaceLine[0];
            private static readonly IEnumerable<MacroStabilityInwardsStochasticSoilModel> stochasticSoilModels = new MacroStabilityInwardsStochasticSoilModel[0];
            private static readonly MacroStabilityInwardsFailureMechanism failureMechanism = new MacroStabilityInwardsFailureMechanism();
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

            protected override MacroStabilityInwardsCalculationScenarioContext CreateObject()
            {
                return new MacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                           parent,
                                                                           surfaceLines,
                                                                           stochasticSoilModels,
                                                                           failureMechanism,
                                                                           assessmentSection);
            }

            protected override DerivedMacroStabilityInwardsCalculationScenarioContext CreateDerivedObject()
            {
                return new DerivedMacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                                  parent,
                                                                                  surfaceLines,
                                                                                  stochasticSoilModels,
                                                                                  failureMechanism,
                                                                                  assessmentSection);
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                yield return new TestCaseData(new MacroStabilityInwardsCalculationScenarioContext(new MacroStabilityInwardsCalculationScenario(),
                                                                                                  parent,
                                                                                                  surfaceLines,
                                                                                                  stochasticSoilModels,
                                                                                                  failureMechanism,
                                                                                                  assessmentSection))
                    .SetName("Calculation");
                yield return new TestCaseData(new MacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                                                  new CalculationGroup(),
                                                                                                  surfaceLines,
                                                                                                  stochasticSoilModels,
                                                                                                  failureMechanism,
                                                                                                  assessmentSection))
                    .SetName("Parent");
            }
        }

        private class DerivedMacroStabilityInwardsCalculationScenarioContext : MacroStabilityInwardsCalculationScenarioContext
        {
            public DerivedMacroStabilityInwardsCalculationScenarioContext(MacroStabilityInwardsCalculationScenario calculation,
                                                                          CalculationGroup parent,
                                                                          IEnumerable<MacroStabilityInwardsSurfaceLine> surfaceLines,
                                                                          IEnumerable<MacroStabilityInwardsStochasticSoilModel> stochasticSoilModels,
                                                                          MacroStabilityInwardsFailureMechanism macroStabilityInwardsFailureMechanism,
                                                                          IAssessmentSection assessmentSection)
                : base(calculation, parent, surfaceLines, stochasticSoilModels, macroStabilityInwardsFailureMechanism, assessmentSection) {}
        }
    }
}