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

using System.Collections.Generic;
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
    public class MacroStabilityInwardsCalculationGroupContextTest
    {
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ParameteredConstructor_ExpectedValues(bool withParent)
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
            var soilModels = new[]
            {
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel()
            };

            CalculationGroup parent = withParent ? new CalculationGroup() : null;

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

        [Test]
        public void Equals_ToNull_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup();
            var parent = new CalculationGroup();
            var surfaceLines = new MacroStabilityInwardsSurfaceLine[0];
            var soilModels = new MacroStabilityInwardsStochasticSoilModel[0];
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var groupContext = new MacroStabilityInwardsCalculationGroupContext(calculationGroup,
                                                                                parent,
                                                                                surfaceLines,
                                                                                soilModels,
                                                                                failureMechanism,
                                                                                assessmentSection);

            // Call
            bool equalToNull = groupContext.Equals(null);

            // Assert
            Assert.IsFalse(equalToNull);
            mocks.VerifyAll();
        }

        [Test]
        public void Equals_ToDifferentObject_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup();
            var parent = new CalculationGroup();
            var surfaceLines = new MacroStabilityInwardsSurfaceLine[0];
            var soilModels = new MacroStabilityInwardsStochasticSoilModel[0];
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var groupContext1 = new MacroStabilityInwardsCalculationGroupContext(calculationGroup,
                                                                                 parent,
                                                                                 surfaceLines,
                                                                                 soilModels,
                                                                                 failureMechanism,
                                                                                 assessmentSection);

            // Call
            bool isContextEqualToDifferentObject = groupContext1.Equals(new object());

            // Assert
            Assert.IsFalse(isContextEqualToDifferentObject);
            mocks.VerifyAll();
        }

        [Test]
        public void Equals_ToDerivedClass_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup();
            var parent = new CalculationGroup();
            var surfaceLines = new MacroStabilityInwardsSurfaceLine[0];
            var soilModels = new MacroStabilityInwardsStochasticSoilModel[0];
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var groupContext = new MacroStabilityInwardsCalculationGroupContext(calculationGroup,
                                                                                parent,
                                                                                surfaceLines,
                                                                                soilModels,
                                                                                failureMechanism,
                                                                                assessmentSection);

            var derivedContext = new DerivedMacroStabilityInwardsCalculationGroupContext(calculationGroup,
                                                                                         parent,
                                                                                         surfaceLines,
                                                                                         soilModels,
                                                                                         failureMechanism,
                                                                                         assessmentSection);
            // Call
            bool isContextEqualToDerivedContext = groupContext.Equals(derivedContext);

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

            var calculationGroup = new CalculationGroup();
            var parent = new CalculationGroup();
            var surfaceLines = new MacroStabilityInwardsSurfaceLine[0];
            var soilModels = new MacroStabilityInwardsStochasticSoilModel[0];
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var groupContext1 = new MacroStabilityInwardsCalculationGroupContext(calculationGroup,
                                                                                 parent,
                                                                                 surfaceLines,
                                                                                 soilModels,
                                                                                 failureMechanism,
                                                                                 assessmentSection);
            MacroStabilityInwardsCalculationGroupContext groupContext2 = groupContext1;

            // Call
            bool isContext1EqualTo2 = groupContext1.Equals(groupContext2);
            bool isContext2EqualTo1 = groupContext2.Equals(groupContext1);

            // Assert
            Assert.IsTrue(isContext1EqualTo2);
            Assert.IsTrue(isContext2EqualTo1);
            mocks.VerifyAll();
        }

        [Test]
        public void Equal_ToOtherWithSameWrappedDataAndParent_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup();
            var parent = new CalculationGroup();
            var surfaceLines = new MacroStabilityInwardsSurfaceLine[0];
            var soilModels = new MacroStabilityInwardsStochasticSoilModel[0];
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var groupContext1 = new MacroStabilityInwardsCalculationGroupContext(calculationGroup,
                                                                                 parent,
                                                                                 surfaceLines,
                                                                                 soilModels,
                                                                                 failureMechanism,
                                                                                 assessmentSection);
            var groupContext2 = new MacroStabilityInwardsCalculationGroupContext(calculationGroup,
                                                                                 parent,
                                                                                 surfaceLines,
                                                                                 soilModels,
                                                                                 failureMechanism,
                                                                                 assessmentSection);

            // Call
            bool isContext1EqualTo2 = groupContext1.Equals(groupContext2);
            bool isContext2EqualTo1 = groupContext2.Equals(groupContext1);

            // Assert
            Assert.IsTrue(isContext1EqualTo2);
            Assert.IsTrue(isContext2EqualTo1);
            mocks.VerifyAll();
        }

        [Test]
        public void Equal_ToOtherWithDifferentWrappedData_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var parent = new CalculationGroup();
            var surfaceLines = new MacroStabilityInwardsSurfaceLine[0];
            var soilModels = new MacroStabilityInwardsStochasticSoilModel[0];
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var groupContext1 = new MacroStabilityInwardsCalculationGroupContext(new CalculationGroup(),
                                                                                 parent,
                                                                                 surfaceLines,
                                                                                 soilModels,
                                                                                 failureMechanism,
                                                                                 assessmentSection);
            var groupContext2 = new MacroStabilityInwardsCalculationGroupContext(new CalculationGroup(),
                                                                                 parent,
                                                                                 surfaceLines,
                                                                                 soilModels,
                                                                                 failureMechanism,
                                                                                 assessmentSection);

            // Call
            bool isContext1EqualTo2 = groupContext1.Equals(groupContext2);
            bool isContext2EqualTo1 = groupContext2.Equals(groupContext1);

            // Assert
            Assert.IsFalse(isContext1EqualTo2);
            Assert.IsFalse(isContext2EqualTo1);
            mocks.VerifyAll();
        }

        [Test]
        public void Equal_ToOtherWithDifferentParent_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup();
            var surfaceLines = new MacroStabilityInwardsSurfaceLine[0];
            var soilModels = new MacroStabilityInwardsStochasticSoilModel[0];
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var groupContext1 = new MacroStabilityInwardsCalculationGroupContext(calculationGroup,
                                                                                 new CalculationGroup(),
                                                                                 surfaceLines,
                                                                                 soilModels,
                                                                                 failureMechanism,
                                                                                 assessmentSection);
            var groupContext2 = new MacroStabilityInwardsCalculationGroupContext(calculationGroup,
                                                                                 new CalculationGroup(),
                                                                                 surfaceLines,
                                                                                 soilModels,
                                                                                 failureMechanism,
                                                                                 assessmentSection);

            // Call
            bool isContext1EqualTo2 = groupContext1.Equals(groupContext2);
            bool isContext2EqualTo1 = groupContext2.Equals(groupContext1);

            // Assert
            Assert.IsFalse(isContext1EqualTo2);
            Assert.IsFalse(isContext2EqualTo1);
            mocks.VerifyAll();
        }

        [Test]
        public void GetHashCode_EqualObject_ReturnsSameHashCode()
        {
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup();
            var parent = new CalculationGroup();
            var surfaceLines = new MacroStabilityInwardsSurfaceLine[0];
            var soilModels = new MacroStabilityInwardsStochasticSoilModel[0];
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var groupContext1 = new MacroStabilityInwardsCalculationGroupContext(calculationGroup,
                                                                                 parent,
                                                                                 surfaceLines,
                                                                                 soilModels,
                                                                                 failureMechanism,
                                                                                 assessmentSection);
            var groupContext2 = new MacroStabilityInwardsCalculationGroupContext(calculationGroup,
                                                                                 parent,
                                                                                 surfaceLines,
                                                                                 soilModels,
                                                                                 failureMechanism,
                                                                                 assessmentSection);

            // Call
            int hashCode1 = groupContext1.GetHashCode();
            int hashCode2 = groupContext2.GetHashCode();

            // Assert
            Assert.AreEqual(hashCode1, hashCode2);
            mocks.VerifyAll();
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