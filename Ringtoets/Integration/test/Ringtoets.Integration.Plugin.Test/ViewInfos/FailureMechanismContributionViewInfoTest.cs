// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.Views;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class FailureMechanismContributionViewInfoTest
    {
        private MockRepository mocks;
        private RingtoetsGuiPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new RingtoetsGuiPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(FailureMechanismContributionView));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
        }

        [Test]
        public void GetViewName_Always_ReturnsViewName()
        {
            // Setup
            var view = new FailureMechanismContributionView();

            var failureMechanismContribution = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 30, 1000);

            // Call
            var viewName = info.GetViewName(view, failureMechanismContribution);

            // Assert
            Assert.AreEqual("Faalkansbegroting", viewName);
        }

        [Test]
        public void ViewDataType_Always_ReturnsViewDataType()
        {
            // Call
            var viewDataType = info.ViewDataType;

            // Assert
            Assert.AreEqual(typeof(FailureMechanismContribution), viewDataType);
        }

        [Test]
        public void ViewType_Always_ReturnsViewType()
        {
            // Call
            var viewType = info.ViewType;

            // Assert
            Assert.AreEqual(typeof(FailureMechanismContributionView), viewType);
        }

        [Test]
        public void DataType_Always_ReturnsDataType()
        {
            // Call
            var dataType = info.DataType;

            // Assert
            Assert.AreEqual(typeof(FailureMechanismContributionContext), dataType);
        }

        [Test]
        public void Image_Always_ReturnsGenericInputOutputIcon()
        {
            // Call
            var image = info.Image;

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.FailureMechanismContributionIcon, image);
        }

        [Test]
        public void GetViewData_Always_Returns_FailureMechanismContribution()
        {
            // Setup
            var contribution = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 100.0, 123456);
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            var context = new FailureMechanismContributionContext(contribution, assessmentSection);

            mocks.ReplayAll();

            // Call
            var viewData = info.GetViewData(context);

            // Assert
            Assert.AreSame(viewData, contribution);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSection_ReturnsTrue()
        {
            // Setup
            var contribution = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 100.0, 123456);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.GetFailureMechanisms())
                             .Return(Enumerable.Empty<IFailureMechanism>());
            assessmentSection.Stub(section => section.Composition)
                             .Return(AssessmentSectionComposition.Dike);
            assessmentSection.Stub(section => section.FailureMechanismContribution)
                             .Return(contribution);

            mocks.ReplayAll();

            using (var view = new FailureMechanismContributionView
            {
                Data = contribution,
                AssessmentSection = assessmentSection
            })
            {
                // Call
                var closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsTrue(closeForData);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var contribution1 = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 100.0, 123456);
            var contribution2 = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 100.0, 789123);

            var assessmentSection1 = mocks.Stub<IAssessmentSection>();
            assessmentSection1.Stub(section => section.FailureMechanismContribution)
                              .Return(contribution1);
            assessmentSection1.Stub(section => section.Composition)
                              .Return(AssessmentSectionComposition.DikeAndDune);
            assessmentSection1.Stub(section => section.GetFailureMechanisms())
                              .Return(Enumerable.Empty<IFailureMechanism>());

            var assessmentSection2 = mocks.Stub<IAssessmentSection>();
            assessmentSection2.Stub(section => section.FailureMechanismContribution)
                              .Return(contribution2);
            assessmentSection2.Stub(section => section.Composition)
                              .Return(AssessmentSectionComposition.DikeAndDune);
            assessmentSection2.Stub(section => section.GetFailureMechanisms())
                              .Return(Enumerable.Empty<IFailureMechanism>());
            mocks.ReplayAll();

            var view = new FailureMechanismContributionView
            {
                Data = contribution1,
                AssessmentSection = assessmentSection1
            };

            // Call
            var closeForData = info.CloseForData(view, assessmentSection2);

            // Assert
            Assert.IsFalse(closeForData);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewWithoutData_ReturnsFalse()
        {
            // Setup
            var contribution = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 100.0, 789123);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.FailureMechanismContribution)
                             .Return(contribution);
            mocks.ReplayAll();

            var view = new FailureMechanismContributionView();

            // Call
            var closeForData = info.CloseForData(view, assessmentSection);

            // Assert
            Assert.IsFalse(closeForData);
            mocks.VerifyAll();
        }

        [Test]
        public void AfterCreate_WithGuiSet_SetsAssessmentSection()
        {
            // Setup
            var contribution = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 100.0, 789123);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.GetFailureMechanisms())
                             .Return(Enumerable.Empty<IFailureMechanism>());
            assessmentSection.Stub(section => section.Composition)
                             .Return(AssessmentSectionComposition.Dike);
            assessmentSection.Stub(section => section.FailureMechanismContribution)
                             .Return(contribution);

            IGui guiStub = mocks.Stub<IGui>();
            guiStub.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            guiStub.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
            guiStub.Stub(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());

            mocks.ReplayAll();

            var context = new FailureMechanismContributionContext(contribution, assessmentSection);
            var view = new FailureMechanismContributionView();

            using (var guiPlugin = new RingtoetsGuiPlugin())
            {
                info = guiPlugin.GetViewInfos().First(tni => tni.ViewType == typeof(FailureMechanismContributionView));
                guiPlugin.Gui = guiStub;

                // Call
                info.AfterCreate(view, context);

                // Assert
                Assert.AreSame(view.AssessmentSection, assessmentSection);
            }
            mocks.VerifyAll();
        }
    }
}