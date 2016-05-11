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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Integration.Data.Placeholders;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Plugin;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using RingtoetsIntegrationFormsResources = Ringtoets.Integration.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class AssessmentSectionTreeNodeInfoTest
    {
        private MockRepository mocks;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new RingtoetsGuiPlugin())
            {
                var info = GetInfo(plugin);

                // Assert
                Assert.AreEqual(typeof(IAssessmentSection), info.TagType);
                Assert.IsNull(info.ForeColor);
                Assert.IsNull(info.CanCheck);
                Assert.IsNull(info.IsChecked);
                Assert.IsNull(info.OnNodeChecked);
                Assert.IsNull(info.CanDrag);
                Assert.IsNull(info.CanDrop);
                Assert.IsNull(info.CanInsert);
                Assert.IsNull(info.OnDrop);
            }
        }

        [Test]
        public void Text_Always_ReturnsName()
        {
            // Setup
            var testName = "ttt";

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Name = testName;

            mocks.ReplayAll();

            using (var plugin = new RingtoetsGuiPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var text = info.Text(assessmentSection);

                // Assert
                Assert.AreEqual(testName, text);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();

            mocks.ReplayAll();

            using (var plugin = new RingtoetsGuiPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var image = info.Image(assessmentSection);

                // Assert
                TestHelper.AssertImagesAreEqual(RingtoetsIntegrationFormsResources.AssessmentSectionFolderIcon, image);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void EnsureVisibleOnCreate_Always_ReturnsTrue()
        {
            // Setup
            using (var plugin = new RingtoetsGuiPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var result = info.EnsureVisibleOnCreate(null, null);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnsChildsOnData()
        {
            // Setup
            var failureMechanisms = new IFailureMechanism[]
            {
                new PipingFailureMechanism(),
                new StandAloneFailureMechanism("A", "C")
            };
            var contribution = new FailureMechanismContribution(failureMechanisms, 10.0, 2);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.FailureMechanismContribution).Return(contribution);
            assessmentSection.Stub(section => section.GetFailureMechanisms()).Return(failureMechanisms);
            mocks.ReplayAll();

            using (var plugin = new RingtoetsGuiPlugin())
            {
                var info = GetInfo(plugin);
                // Call
                var objects = info.ChildNodeObjects(assessmentSection).ToArray();

                // Assert
                Assert.AreEqual(6, objects.Length);
                var referenceLineContext = (ReferenceLineContext) objects[0];
                Assert.AreSame(assessmentSection.ReferenceLine, referenceLineContext.WrappedData);
                Assert.AreSame(assessmentSection, referenceLineContext.Parent);

                var contributionContext = (FailureMechanismContributionContext) objects[1];
                Assert.AreSame(contribution, contributionContext.WrappedData);
                Assert.AreSame(assessmentSection, contributionContext.Parent);

                var context = (HydraulicBoundaryDatabaseContext) objects[2];
                Assert.AreSame(assessmentSection.HydraulicBoundaryDatabase, context.Parent.HydraulicBoundaryDatabase);
                Assert.AreSame(assessmentSection, context.Parent);

                var commentContext = (CommentContext<ICommentable>) objects[3];
                Assert.AreSame(assessmentSection, commentContext.CommentContainer);

                var pipingFailureMechanismContext = (PipingFailureMechanismContext) objects[4];
                Assert.AreSame(failureMechanisms[0], pipingFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, pipingFailureMechanismContext.Parent);

                var placeholderFailureMechanismContext = (StandAloneFailureMechanismContext) objects[5];
                Assert.AreSame(failureMechanisms[1], placeholderFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, placeholderFailureMechanismContext.Parent);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            var gui = mocks.StrictMultiMock<IGui>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();
            gui.Expect(g => g.Get(null, treeViewControlMock)).Return(menuBuilderMock);
            gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();

            menuBuilderMock.Expect(mb => mb.AddOpenItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddRenameItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddDeleteItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddImportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            mocks.ReplayAll();

            using (var plugin = new RingtoetsGuiPlugin())
            {
                var info = GetInfo(plugin);
                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(null, null, treeViewControlMock);
            }
            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void CanRename_Always_ReturnsTrue()
        {
            // Setup
            using (var plugin = new RingtoetsGuiPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var canRename = info.CanRename(null, null);

                // Assert
                Assert.IsTrue(canRename);
            }
        }

        [Test]
        public void OnNodeRenamed_WithData_SetProjectNameWithNotification()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Expect(section => section.NotifyObservers());

            mocks.ReplayAll();

            using (var plugin = new RingtoetsGuiPlugin())
            {
                var info = GetInfo(plugin);
                // Call
                const string newName = "New Name";
                info.OnNodeRenamed(assessmentSection, newName);

                // Assert
                Assert.AreEqual(newName, assessmentSection.Name);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void CanRemove_Always_ReturnsTrue()
        {
            // Setup
            using (var plugin = new RingtoetsGuiPlugin())
            {
                var info = GetInfo(plugin);
                // Call
                var canRemove = info.CanRemove(null, null);

                // Assert
                Assert.IsTrue(canRemove);
            }
        }

        [Test]
        public void RemoveNodeData_ProjectWithAssessmentSection_ReturnTrueAndRemoveAssessmentSection()
        {
            // Setup
            var observerMock = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            observerMock.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            var project = new Project();
            project.Items.Add(assessmentSection);
            project.Attach(observerMock);

            using (var plugin = new RingtoetsGuiPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                info.OnNodeRemoved(assessmentSection, project);

                // Assert
                CollectionAssert.DoesNotContain(project.Items, assessmentSection);
            }
            mocks.VerifyAll();
        }

        private TreeNodeInfo GetInfo(RingtoetsGuiPlugin guiPlugin)
        {
            return guiPlugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(IAssessmentSection));
        }
    }
}