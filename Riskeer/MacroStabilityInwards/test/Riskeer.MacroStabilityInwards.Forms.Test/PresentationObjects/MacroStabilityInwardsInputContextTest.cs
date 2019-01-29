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
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Forms.PresentationObjects;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class MacroStabilityInwardsInputContextTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new MacroStabilityInwardsCalculationScenario();
            var surfaceLines = new[]
            {
                new MacroStabilityInwardsSurfaceLine(string.Empty)
            };
            MacroStabilityInwardsStochasticSoilModel[] stochasticSoilModels =
            {
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel()
            };

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            // Call
            var context = new MacroStabilityInwardsInputContext(calculation.InputParameters, calculation, surfaceLines, stochasticSoilModels, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<MacroStabilityInwardsContext<MacroStabilityInwardsInput>>(context);
            Assert.AreSame(calculation.InputParameters, context.WrappedData);
            Assert.AreSame(calculation, context.MacroStabilityInwardsCalculation);
            Assert.AreSame(failureMechanism, context.FailureMechanism);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
            CollectionAssert.AreEqual(surfaceLines, context.AvailableMacroStabilityInwardsSurfaceLines);
            CollectionAssert.AreEqual(stochasticSoilModels, context.AvailableStochasticSoilModels);
            mocks.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_CalculationItemNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationInput = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties());
            var surfaceLines = new[]
            {
                new MacroStabilityInwardsSurfaceLine(string.Empty)
            };
            MacroStabilityInwardsStochasticSoilModel[] stochasticSoilModels =
            {
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel()
            };
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            // Call
            TestDelegate call = () => new MacroStabilityInwardsInputContext(calculationInput, null, surfaceLines, stochasticSoilModels, failureMechanism, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculation", exception.ParamName);
            mocks.VerifyAll();
        }
    }
}