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

using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Ringtoets.MacroStabilityInwards.Forms.Test.PresentationObjects
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
            var soilModels = new[]
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
            var soilModels = new[]
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

        [Test]
        public void Equals_ToNull_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new MacroStabilityInwardsCalculationScenario();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var context = new MacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                              new CalculationGroup(),
                                                                              Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                              Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                              failureMechanism,
                                                                              assessmentSection);

            // Call
            bool isContextEqualToNull = context.Equals(null);

            // Assert
            Assert.IsFalse(isContextEqualToNull);
            mocks.VerifyAll();
        }

        [Test]
        public void Equals_ToDifferentObject_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new MacroStabilityInwardsCalculationScenario();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var context = new MacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                              new CalculationGroup(),
                                                                              Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                              Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                              failureMechanism,
                                                                              assessmentSection);

            // Call
            bool isContextEqualToDifferentObject = context.Equals(new object());

            // Assert
            Assert.IsFalse(isContextEqualToDifferentObject);
            mocks.VerifyAll();
        }

        [Test]
        public void Equals_ToDerivedObject_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new MacroStabilityInwardsCalculationScenario();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var calculationGroup = new CalculationGroup();
            var surfaceLines = new MacroStabilityInwardsSurfaceLine[0];
            var stochasticSoilModels = new MacroStabilityInwardsStochasticSoilModel[0];

            var context = new MacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                              calculationGroup,
                                                                              surfaceLines,
                                                                              stochasticSoilModels,
                                                                              failureMechanism,
                                                                              assessmentSection);

            var derivedContext = new DerivedMacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                                            calculationGroup,
                                                                                            surfaceLines,
                                                                                            stochasticSoilModels,
                                                                                            failureMechanism,
                                                                                            assessmentSection);

            // Call
            bool isContextEqualToDerivedContext = context.Equals(derivedContext);

            // Assert
            Assert.IsFalse(isContextEqualToDerivedContext);
            mocks.VerifyAll();
        }

        [Test]
        public void Equals_ToItself_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new MacroStabilityInwardsCalculationScenario();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var calculationGroup = new CalculationGroup();
            var surfaceLines = new MacroStabilityInwardsSurfaceLine[0];
            var stochasticSoilModels = new MacroStabilityInwardsStochasticSoilModel[0];

            var context1 = new MacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                               calculationGroup,
                                                                               surfaceLines,
                                                                               stochasticSoilModels,
                                                                               failureMechanism,
                                                                               assessmentSection);
            MacroStabilityInwardsCalculationScenarioContext context2 = context1;

            // Call
            bool isContext1EqualTo2 = context1.Equals(context2);
            bool isContext2EqualTo1 = context2.Equals(context1);

            // Assert
            Assert.IsTrue(isContext1EqualTo2);
            Assert.IsTrue(isContext2EqualTo1);
            mocks.VerifyAll();
        }

        [Test]
        public void Equals_ToOtherWithSameWrappedDataAndParent_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new MacroStabilityInwardsCalculationScenario();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var parent = new CalculationGroup();
            var surfaceLines = new MacroStabilityInwardsSurfaceLine[0];
            var stochasticSoilModels = new MacroStabilityInwardsStochasticSoilModel[0];

            var context1 = new MacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                               parent,
                                                                               surfaceLines,
                                                                               stochasticSoilModels,
                                                                               failureMechanism,
                                                                               assessmentSection);

            var context2 = new MacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                               parent,
                                                                               surfaceLines,
                                                                               stochasticSoilModels,
                                                                               failureMechanism,
                                                                               assessmentSection);

            // Call
            bool isContext1EqualTo2 = context1.Equals(context2);
            bool isContext2EqualTo1 = context2.Equals(context1);

            // Assert
            Assert.IsTrue(isContext1EqualTo2);
            Assert.IsTrue(isContext2EqualTo1);
            mocks.VerifyAll();
        }

        [Test]
        public void Equals_ToOtherWithDifferentWrappedData_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var calculationGroup = new CalculationGroup();
            var surfaceLines = new MacroStabilityInwardsSurfaceLine[0];
            var stochasticSoilModels = new MacroStabilityInwardsStochasticSoilModel[0];

            var context1 = new MacroStabilityInwardsCalculationScenarioContext(new MacroStabilityInwardsCalculationScenario(),
                                                                               calculationGroup,
                                                                               surfaceLines,
                                                                               stochasticSoilModels,
                                                                               failureMechanism,
                                                                               assessmentSection);

            var context2 = new MacroStabilityInwardsCalculationScenarioContext(new MacroStabilityInwardsCalculationScenario(),
                                                                               calculationGroup,
                                                                               surfaceLines,
                                                                               stochasticSoilModels,
                                                                               failureMechanism,
                                                                               assessmentSection);

            // Call
            bool isContext1EqualTo2 = context1.Equals(context2);
            bool isContext2EqualTo1 = context2.Equals(context1);

            // Assert
            Assert.IsFalse(isContext1EqualTo2);
            Assert.IsFalse(isContext2EqualTo1);
            mocks.VerifyAll();
        }

        [Test]
        public void Equals_ToOtherWithDifferentParent_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new MacroStabilityInwardsCalculationScenario();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var surfaceLines = new MacroStabilityInwardsSurfaceLine[0];
            var stochasticSoilModels = new MacroStabilityInwardsStochasticSoilModel[0];

            var context1 = new MacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                               new CalculationGroup(),
                                                                               surfaceLines,
                                                                               stochasticSoilModels,
                                                                               failureMechanism,
                                                                               assessmentSection);

            var context2 = new MacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                               new CalculationGroup(),
                                                                               surfaceLines,
                                                                               stochasticSoilModels,
                                                                               failureMechanism,
                                                                               assessmentSection);

            // Call
            bool isContext1EqualTo2 = context1.Equals(context2);
            bool isContext2EqualTo1 = context2.Equals(context1);

            // Assert
            Assert.IsFalse(isContext1EqualTo2);
            Assert.IsFalse(isContext2EqualTo1);
            mocks.VerifyAll();
        }

        [Test]
        public void GetHashCode_EqualObjects_ReturnSameHashCode()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new MacroStabilityInwardsCalculationScenario();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var parent = new CalculationGroup();
            var surfaceLines = new MacroStabilityInwardsSurfaceLine[0];
            var stochasticSoilModels = new MacroStabilityInwardsStochasticSoilModel[0];

            var context1 = new MacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                               parent,
                                                                               surfaceLines,
                                                                               stochasticSoilModels,
                                                                               failureMechanism,
                                                                               assessmentSection);

            var context2 = new MacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                               parent,
                                                                               surfaceLines,
                                                                               stochasticSoilModels,
                                                                               failureMechanism,
                                                                               assessmentSection);

            // Call
            int hashCode1 = context1.GetHashCode();
            int hashCode2 = context2.GetHashCode();

            // Assert
            Assert.AreEqual(hashCode1, hashCode2);
            mocks.VerifyAll();
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