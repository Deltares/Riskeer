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
using System.Drawing;
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
            // Call
            string viewName = info.GetViewName(null, null);

            // Assert
            Assert.AreEqual("Faalkansbegroting", viewName);
        }

        [Test]
        public void ViewDataType_Always_ReturnsViewDataType()
        {
            // Call
            Type viewDataType = info.ViewDataType;

            // Assert
            Assert.AreEqual(typeof(FailureMechanismContribution), viewDataType);
        }

        [Test]
        public void ViewType_Always_ReturnsViewType()
        {
            // Call
            Type viewType = info.ViewType;

            // Assert
            Assert.AreEqual(typeof(FailureMechanismContributionView), viewType);
        }

        [Test]
        public void DataType_Always_ReturnsDataType()
        {
            // Call
            Type dataType = info.DataType;

            // Assert
            Assert.AreEqual(typeof(FailureMechanismContributionContext), dataType);
        }

        [Test]
        public void Image_Always_ReturnsGenericInputOutputIcon()
        {
            // Call
            Image image = info.Image;

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.FailureMechanismContributionIcon, image);
        }

        [Test]
        public void GetViewData_Always_Returns_FailureMechanismContribution()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(null, mocks);
            mocks.ReplayAll();

            var context = new FailureMechanismContributionContext(assessmentSection.FailureMechanismContribution, assessmentSection);

            // Call
            object viewData = info.GetViewData(context);

            // Assert
            Assert.AreSame(viewData, assessmentSection.FailureMechanismContribution);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSection_ReturnsTrue()
        {
            // Setup
            var viewCommands = mocks.Stub<IViewCommands>();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(null, mocks);
            assessmentSection.Stub(section => section.Composition)
                             .Return(AssessmentSectionComposition.Dike);

            mocks.ReplayAll();

            using (var view = new FailureMechanismContributionView(assessmentSection, viewCommands)
            {
                Data = assessmentSection.FailureMechanismContribution,
                AssessmentSection = assessmentSection
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsTrue(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var viewCommands = mocks.Stub<IViewCommands>();

            IAssessmentSection assessmentSection1 = AssessmentSectionHelper.CreateAssessmentSectionStub(null, mocks);
            assessmentSection1.Stub(section => section.Composition)
                              .Return(AssessmentSectionComposition.DikeAndDune);

            IAssessmentSection assessmentSection2 = AssessmentSectionHelper.CreateAssessmentSectionStub(null, mocks);
            assessmentSection2.Stub(section => section.Composition)
                              .Return(AssessmentSectionComposition.DikeAndDune);
            mocks.ReplayAll();

            using (var view = new FailureMechanismContributionView(assessmentSection1, viewCommands)
            {
                Data = assessmentSection1.FailureMechanismContribution,
                AssessmentSection = assessmentSection1
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, assessmentSection2);

                // Assert
                Assert.IsFalse(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewWithoutData_ReturnsFalse()
        {
            // Setup
            var viewCommands = mocks.Stub<IViewCommands>();

            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(null, mocks);
            mocks.ReplayAll();

            using (var view = new FailureMechanismContributionView(assessmentSection, viewCommands))
            {
                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsFalse(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void AfterCreate_WithGuiSet_SetsAssessmentSection()
        {
            // Setup
            var viewCommands = mocks.Stub<IViewCommands>();

            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(null, mocks);
            assessmentSection.Stub(section => section.Composition).Return(AssessmentSectionComposition.Dike);

            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();

            mocks.ReplayAll();

            var context = new FailureMechanismContributionContext(assessmentSection.FailureMechanismContribution, assessmentSection);

            using (var view = new FailureMechanismContributionView(assessmentSection, viewCommands))
            using (var ringtoetsPlugin = new RingtoetsPlugin())
            {
                info = ringtoetsPlugin.GetViewInfos().First(tni => tni.ViewType == typeof(FailureMechanismContributionView));
                ringtoetsPlugin.Gui = gui;

                // Call
                info.AfterCreate(view, context);

                // Assert
                Assert.AreSame(view.AssessmentSection, assessmentSection);
            }

            mocks.VerifyAll();
        }
    }
}