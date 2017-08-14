// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using Core.Common.Base;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Forms.PresentationObjects;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationScenarioContextTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var surfaceLines = new[]
            {
                new MacroStabilityInwardsSurfaceLine()
            };
            var soilModels = new[]
            {
                new TestStochasticSoilModel()
            };
            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
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
    }
}