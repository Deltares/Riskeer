﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(FailureMechanismContributionContext), info.DataType);
            Assert.AreEqual(typeof(FailureMechanismContribution), info.ViewDataType);
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
        public void GetViewData_Always_ReturnsFailureMechanismContribution()
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
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSectionStub();

            using (var view = new FailureMechanismContributionView(assessmentSection, viewCommands))
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
            mocks.ReplayAll();

            var assessmentSection1 = new AssessmentSectionStub();
            var assessmentSection2 = new AssessmentSectionStub();

            using (var view = new FailureMechanismContributionView(assessmentSection1, viewCommands))
            {
                // Call
                bool closeForData = info.CloseForData(view, assessmentSection2);

                // Assert
                Assert.IsFalse(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CreateInstance_WithContext_SetsExpectedViewProperties()
        {
            // Setup
            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
            gui.Stub(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSectionStub();
            plugin.Gui = gui;

            var context = new FailureMechanismContributionContext(assessmentSection.FailureMechanismContribution, assessmentSection);

            // Call
            var view = (FailureMechanismContributionView) info.CreateInstance(context);

            // Assert
            Assert.AreSame(assessmentSection, view.AssessmentSection);
        }
    }
}