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

using System.Linq;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Forms.PresentationObjects;
using Riskeer.GrassCoverErosionInwards.Forms.Views;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.GrassCoverErosionInwards.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationsViewInfoTest
    {
        private MockRepository mocks;
        private GrassCoverErosionInwardsPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new GrassCoverErosionInwardsPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(GrassCoverErosionInwardsCalculationsView));
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
            Assert.AreEqual(typeof(GrassCoverErosionInwardsCalculationGroupContext), info.DataType);
            Assert.AreEqual(typeof(CalculationGroup), info.ViewDataType);
            TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.GeneralFolderIcon, info.Image);
        }

        [Test]
        public void GetViewData_Always_ReturnsWrappedCalculationGroup()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var calculationGroup = new CalculationGroup();
            var calculationGroupContext = new GrassCoverErosionInwardsCalculationGroupContext(calculationGroup,
                                                                            null,
                                                                            failureMechanism,
                                                                            assessmentSection);

            // Call & Assert
            Assert.AreSame(calculationGroup, info.GetViewData(calculationGroupContext));
            mocks.VerifyAll();
        }

        [Test]
        public void GetViewName_WithGrassCoverErosionInwardsCalculationGroupContext_ReturnsCalculationGroupName()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationsView = new GrassCoverErosionInwardsCalculationsView();

            const string calculationGroupName = "Test";

            var calculationGroup = new CalculationGroup
            {
                Name = calculationGroupName
            };

            var grassCoverErosionInwardsCalculationGroupContext = new GrassCoverErosionInwardsCalculationGroupContext(calculationGroup,
                                                                                  null,
                                                                                  new GrassCoverErosionInwardsFailureMechanism(),
                                                                                  assessmentSection);

            // Call
            string name = info.GetViewName(calculationsView, grassCoverErosionInwardsCalculationGroupContext);

            // Assert
            Assert.AreEqual(calculationGroupName, name);
            mocks.VerifyAll();
        }

        [Test]
        public void AdditionalDataCheck_GrassCoverErosionInwardsCalculationGroupContextWithGrassCoverErosionInwardsFailureMechanismParent_ReturnsTrue()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var grassCoverErosionInwardsCalculationGroupContext = new GrassCoverErosionInwardsCalculationGroupContext(grassCoverErosionInwardsFailureMechanism.CalculationsGroup,
                                                                                  null,
                                                                                  grassCoverErosionInwardsFailureMechanism,
                                                                                  assessmentSection);

            // Call
            bool additionalDataCheck = info.AdditionalDataCheck(grassCoverErosionInwardsCalculationGroupContext);

            // Assert
            Assert.IsTrue(additionalDataCheck);
            mocks.VerifyAll();
        }

        [Test]
        public void AdditionalDataCheck_GrassCoverErosionInwardsCalculationGroupContextWithoutGrassCoverErosionInwardsFailureMechanismParent_ReturnsFalse()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var calculationGroup = new CalculationGroup();
            var grassCoverErosionInwardsCalculationGroupContext = new GrassCoverErosionInwardsCalculationGroupContext(calculationGroup,
                                                                                  null,
                                                                                  failureMechanism,
                                                                                  assessmentSection);

            // Call
            bool additionalDataCheck = info.AdditionalDataCheck(grassCoverErosionInwardsCalculationGroupContext);

            // Assert
            Assert.IsFalse(additionalDataCheck);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_AssessmentSectionRemovedWithoutGrassCoverErosionInwardsFailureMechanism_ReturnsFalse()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new IFailureMechanism[0]);
            mocks.ReplayAll();

            var view = new GrassCoverErosionInwardsCalculationsView
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
                new GrassCoverErosionInwardsFailureMechanism()
            });
            mocks.ReplayAll();

            var view = new GrassCoverErosionInwardsCalculationsView
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
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });
            mocks.ReplayAll();

            var view = new GrassCoverErosionInwardsCalculationsView
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

            var view = new GrassCoverErosionInwardsCalculationsView();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

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

            var view = new GrassCoverErosionInwardsCalculationsView();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

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

            var view = new GrassCoverErosionInwardsCalculationsView();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(new GrassCoverErosionInwardsFailureMechanism(), assessmentSection);

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

            var view = new GrassCoverErosionInwardsCalculationsView();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, assessmentSection);

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
            var view = mocks.StrictMock<GrassCoverErosionInwardsCalculationsView>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var grassCoverErosionInwardsFailureMechanism = mocks.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var grassCoverErosionInwardsCalculationsGroup = mocks.StrictMock<CalculationGroup>();
            var grassCoverErosionInwardsCalculationGroupContext = new GrassCoverErosionInwardsCalculationGroupContext(grassCoverErosionInwardsCalculationsGroup,
                                                                                  null,
                                                                                  grassCoverErosionInwardsFailureMechanism, assessmentSection);

            view.Expect(v => v.AssessmentSection = assessmentSection);
            view.Expect(v => v.GrassCoverErosionInwardsFailureMechanism = grassCoverErosionInwardsFailureMechanism);

            mocks.ReplayAll();

            // Call
            info.AfterCreate(view, grassCoverErosionInwardsCalculationGroupContext);

            // Assert
            mocks.VerifyAll();
        }
    }
}
