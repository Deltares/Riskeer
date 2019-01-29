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

using System.Linq;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Forms.PresentationObjects;
using Ringtoets.MacroStabilityInwards.Forms.Views;
using Ringtoets.MacroStabilityInwards.Primitives;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Riskeer.MacroStabilityInwards.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationsViewInfoTest
    {
        private MockRepository mocks;
        private MacroStabilityInwardsPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new MacroStabilityInwardsPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(MacroStabilityInwardsCalculationsView));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(MacroStabilityInwardsCalculationGroupContext), info.DataType);
            Assert.AreEqual(typeof(CalculationGroup), info.ViewDataType);
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GeneralFolderIcon, info.Image);
        }

        [Test]
        public void GetViewData_Always_ReturnsWrappedCalculationGroup()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            mocks.ReplayAll();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var calculationGroup = new CalculationGroup();
            var calculationGroupContext = new MacroStabilityInwardsCalculationGroupContext(calculationGroup,
                                                                                           null,
                                                                                           Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                           Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                                           failureMechanism,
                                                                                           assessmentSection);

            // Call & Assert
            Assert.AreSame(calculationGroup, info.GetViewData(calculationGroupContext));
            mocks.VerifyAll();
        }

        [Test]
        public void GetViewName_WithMacroStabilityInwardsCalculationGroupContext_ReturnsCalculationGroupName()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationsView = new MacroStabilityInwardsCalculationsView();

            const string calculationGroupName = "Test";

            var calculationGroup = new CalculationGroup
            {
                Name = calculationGroupName
            };

            var calculationGroupContext = new MacroStabilityInwardsCalculationGroupContext(calculationGroup,
                                                                                           null,
                                                                                           Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                           Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                                           new MacroStabilityInwardsFailureMechanism(),
                                                                                           assessmentSection);
            // Call
            string name = info.GetViewName(calculationsView, calculationGroupContext);

            // Assert
            Assert.AreEqual(calculationGroupName, name);
            mocks.VerifyAll();
        }

        [Test]
        public void AdditionalDataCheck_CalculationGroupContextWithFailureMechanismParent_ReturnsTrue()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var calculationGroupContext = new MacroStabilityInwardsCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                                           null,
                                                                                           Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                           Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                                           failureMechanism,
                                                                                           assessmentSection);

            // Call
            bool additionalDataCheck = info.AdditionalDataCheck(calculationGroupContext);

            // Assert
            Assert.IsTrue(additionalDataCheck);
            mocks.VerifyAll();
        }

        [Test]
        public void AdditionalDataCheck_CalculationGroupContextWithoutFailureMechanismParent_ReturnsFalse()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var calculationGroup = new CalculationGroup();
            var calculationGroupContext = new MacroStabilityInwardsCalculationGroupContext(calculationGroup,
                                                                                           null,
                                                                                           Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                           Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                                           failureMechanism,
                                                                                           assessmentSection);

            // Call
            bool additionalDataCheck = info.AdditionalDataCheck(calculationGroupContext);

            // Assert
            Assert.IsFalse(additionalDataCheck);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_AssessmentSectionRemovedWithoutFailureMechanism_ReturnsFalse()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new IFailureMechanism[0]);
            mocks.ReplayAll();

            var view = new MacroStabilityInwardsCalculationsView
            {
                Data = new CalculationGroup()
            };

            // Call
            bool closeForData = info.CloseForData(view, assessmentSection);

            // Assert
            Assert.IsFalse(closeForData);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new[]
            {
                new MacroStabilityInwardsFailureMechanism()
            });
            mocks.ReplayAll();

            var view = new MacroStabilityInwardsCalculationsView
            {
                Data = new CalculationGroup()
            };

            // Call
            bool closeForData = info.CloseForData(view, assessmentSection);

            // Assert
            Assert.IsFalse(closeForData);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSection_ReturnsTrue()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });
            mocks.ReplayAll();

            var view = new MacroStabilityInwardsCalculationsView
            {
                Data = failureMechanism.CalculationsGroup
            };

            // Call
            bool closeForData = info.CloseForData(view, assessmentSection);

            // Assert
            Assert.IsTrue(closeForData);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanism_ReturnsFalse()
        {
            // Setup
            mocks.ReplayAll();

            var view = new MacroStabilityInwardsCalculationsView();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            view.Data = new CalculationGroup();

            // Call
            bool closeForData = info.CloseForData(view, failureMechanism);

            // Assert
            Assert.IsFalse(closeForData);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanism_ReturnsTrue()
        {
            // Setup
            mocks.ReplayAll();

            var view = new MacroStabilityInwardsCalculationsView();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            view.Data = failureMechanism.CalculationsGroup;

            // Call
            bool closeForData = info.CloseForData(view, failureMechanism);

            // Assert
            Assert.IsTrue(closeForData);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanismContext_ReturnsFalse()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var view = new MacroStabilityInwardsCalculationsView();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var failureMechanismContext = new MacroStabilityInwardsFailureMechanismContext(new MacroStabilityInwardsFailureMechanism(), assessmentSection);

            view.Data = failureMechanism.CalculationsGroup;

            // Call
            bool closeForData = info.CloseForData(view, failureMechanismContext);

            // Assert
            Assert.IsFalse(closeForData);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanismContext_ReturnsTrue()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var view = new MacroStabilityInwardsCalculationsView();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var failureMechanismContext = new MacroStabilityInwardsFailureMechanismContext(failureMechanism, assessmentSection);

            view.Data = failureMechanism.CalculationsGroup;

            // Call
            bool closeForData = info.CloseForData(view, failureMechanismContext);

            // Assert
            Assert.IsTrue(closeForData);
            mocks.VerifyAll();
        }

        [Test]
        public void AfterCreate_Always_SetsSpecificPropertiesToView()
        {
            // Setup
            var view = mocks.StrictMock<MacroStabilityInwardsCalculationsView>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanism = mocks.StrictMock<MacroStabilityInwardsFailureMechanism>();
            var calculationsGroup = mocks.StrictMock<CalculationGroup>();
            var calculationGroupContext = new MacroStabilityInwardsCalculationGroupContext(calculationsGroup, null, Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(), Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(), failureMechanism, assessmentSection);

            view.Expect(v => v.AssessmentSection = assessmentSection);
            view.Expect(v => v.MacroStabilityInwardsFailureMechanism = failureMechanism);

            mocks.ReplayAll();

            // Call
            info.AfterCreate(view, calculationGroupContext);

            // Assert
            mocks.VerifyAll();
        }
    }
}