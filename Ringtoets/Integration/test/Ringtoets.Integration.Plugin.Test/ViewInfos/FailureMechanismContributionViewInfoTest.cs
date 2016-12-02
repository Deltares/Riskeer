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
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.Views;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class FailureMechanismContributionViewInfoTest
    {
        private MockRepository mocks;
        private RingtoetsPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new RingtoetsPlugin();
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
            var handler1 = mocks.Stub<IFailureMechanismContributionNormChangeHandler>();
            var handler2 = mocks.Stub<IAssessmentSectionCompositionChangeHandler>();
            var viewCommands = mocks.Stub<IViewCommands>();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabaseOrFailureMechanisms(mocks);
            mocks.ReplayAll();

            var view = new FailureMechanismContributionView(handler1, handler2, viewCommands);

            // Call
            var viewName = info.GetViewName(view, assessmentSectionStub.FailureMechanismContribution);

            // Assert
            Assert.AreEqual("Faalkansbegroting", viewName);
            mocks.VerifyAll();
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
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabaseOrFailureMechanisms(mocks);
            mocks.ReplayAll();

            var context = new FailureMechanismContributionContext(assessmentSectionStub.FailureMechanismContribution, assessmentSectionStub);

            // Call
            var viewData = info.GetViewData(context);

            // Assert
            Assert.AreSame(viewData, assessmentSectionStub.FailureMechanismContribution);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSection_ReturnsTrue()
        {
            // Setup
            var handler1 = mocks.Stub<IFailureMechanismContributionNormChangeHandler>();
            var handler2 = mocks.Stub<IAssessmentSectionCompositionChangeHandler>();
            var viewCommands = mocks.Stub<IViewCommands>();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabaseOrFailureMechanisms(mocks);
            assessmentSectionStub.Stub(section => section.Composition)
                                 .Return(AssessmentSectionComposition.Dike);

            mocks.ReplayAll();

            using (var view = new FailureMechanismContributionView(handler1, handler2, viewCommands)
            {
                Data = assessmentSectionStub.FailureMechanismContribution,
                AssessmentSection = assessmentSectionStub
            })
            {
                // Call
                var closeForData = info.CloseForData(view, assessmentSectionStub);

                // Assert
                Assert.IsTrue(closeForData);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var handler1 = mocks.Stub<IFailureMechanismContributionNormChangeHandler>();
            var handler2 = mocks.Stub<IAssessmentSectionCompositionChangeHandler>();
            var viewCommands = mocks.Stub<IViewCommands>();

            IAssessmentSection assessmentSection1 = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabaseOrFailureMechanisms(mocks);
            assessmentSection1.Stub(section => section.Composition)
                              .Return(AssessmentSectionComposition.DikeAndDune);

            IAssessmentSection assessmentSection2 = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabaseOrFailureMechanisms(mocks);
            assessmentSection2.Stub(section => section.Composition)
                              .Return(AssessmentSectionComposition.DikeAndDune);
            mocks.ReplayAll();

            using (var view = new FailureMechanismContributionView(handler1, handler2, viewCommands)
            {
                Data = assessmentSection1.FailureMechanismContribution,
                AssessmentSection = assessmentSection1
            })
            {
                // Call
                var closeForData = info.CloseForData(view, assessmentSection2);

                // Assert
                Assert.IsFalse(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewWithoutData_ReturnsFalse()
        {
            // Setup
            var handler1 = mocks.Stub<IFailureMechanismContributionNormChangeHandler>();
            var handler2 = mocks.Stub<IAssessmentSectionCompositionChangeHandler>();
            var viewCommands = mocks.Stub<IViewCommands>();

            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabaseOrFailureMechanisms(mocks);
            mocks.ReplayAll();

            using (var view = new FailureMechanismContributionView(handler1, handler2, viewCommands))
            {
                // Call
                var closeForData = info.CloseForData(view, assessmentSectionStub);

                // Assert
                Assert.IsFalse(closeForData);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AfterCreate_WithGuiSet_SetsAssessmentSection()
        {
            // Setup
            var handler1 = mocks.Stub<IFailureMechanismContributionNormChangeHandler>();
            var handler2 = mocks.Stub<IAssessmentSectionCompositionChangeHandler>();
            var viewCommands = mocks.Stub<IViewCommands>();

            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabaseOrFailureMechanisms(mocks);
            assessmentSectionStub.Stub(section => section.Composition).Return(AssessmentSectionComposition.Dike);

            IGui guiStub = mocks.Stub<IGui>();
            guiStub.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            guiStub.Stub(g => g.ProjectOpened -= null).IgnoreArguments();

            mocks.ReplayAll();

            var context = new FailureMechanismContributionContext(assessmentSectionStub.FailureMechanismContribution, assessmentSectionStub);

            using (var view = new FailureMechanismContributionView(handler1, handler2, viewCommands))
            using (var ringtoetsPlugin = new RingtoetsPlugin())
            {
                info = ringtoetsPlugin.GetViewInfos().First(tni => tni.ViewType == typeof(FailureMechanismContributionView));
                ringtoetsPlugin.Gui = guiStub;

                // Call
                info.AfterCreate(view, context);

                // Assert
                Assert.AreSame(view.AssessmentSection, assessmentSectionStub);
            }
            mocks.VerifyAll();
        }
    }
}