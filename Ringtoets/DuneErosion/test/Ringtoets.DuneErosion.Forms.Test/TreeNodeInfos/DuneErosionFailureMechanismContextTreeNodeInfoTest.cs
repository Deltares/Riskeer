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
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Forms.PresentationObjects;
using Ringtoets.DuneErosion.Plugin;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.DuneErosion.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class DuneErosionFailureMechanismContextTreeNodeInfoTest
    {
        private const int contextMenuRelevancyIndexWhenRelevant = 2;
        private const int contextMenuRelevancyIndexWhenNotRelevant = 0;

        private MockRepository mocksRepository;
        private DuneErosionPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocksRepository = new MockRepository();
            plugin = new DuneErosionPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(DuneErosionFailureMechanismContext));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();

            mocksRepository.VerifyAll();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            mocksRepository.ReplayAll();

            // Assert
            Assert.AreEqual(typeof(DuneErosionFailureMechanismContext), info.TagType);
            Assert.IsNotNull(info.Text);
            Assert.IsNotNull(info.ForeColor);
            Assert.IsNotNull(info.Image);
            Assert.IsNotNull(info.ChildNodeObjects);
            Assert.IsNotNull(info.ContextMenuStrip);
            Assert.IsNull(info.EnsureVisibleOnCreate);
            Assert.IsNull(info.CanRename);
            Assert.IsNull(info.OnNodeRenamed);
            Assert.IsNull(info.CanRemove);
            Assert.IsNull(info.OnNodeRemoved);
            Assert.IsNull(info.CanCheck);
            Assert.IsNull(info.IsChecked);
            Assert.IsNull(info.OnNodeChecked);
            Assert.IsNull(info.CanDrag);
            Assert.IsNull(info.CanDrop);
            Assert.IsNull(info.CanInsert);
            Assert.IsNull(info.OnDrop);
        }

        [Test]
        public void ChildNodeObjects_FailureMechanismIsNotRelevant_ReturnOnlyFailureMechanismNotRelevantComments()
        {
            // Setup
            var assessmentSectionStub = mocksRepository.Stub<IAssessmentSection>();
            mocksRepository.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism
            {
                IsRelevant = false
            };
            var failureMechanismContext = new DuneErosionFailureMechanismContext(failureMechanism, assessmentSectionStub);

            // Call
            var children = info.ChildNodeObjects(failureMechanismContext).ToArray();

            // Assert
            Assert.AreEqual(1, children.Length);
            var comment = (Comment)children[0];
            Assert.AreSame(failureMechanism.NotRelevantComments, comment);
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsRelevant_CallsContextMenuBuilderMethods()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();
            var assessmentSectionMock = mocksRepository.Stub<IAssessmentSection>();
            var failureMechanismContext = new DuneErosionFailureMechanismContext(failureMechanism, assessmentSectionMock);
            var guiMock = mocksRepository.StrictMock<IGui>();
            var menuBuilderMock = mocksRepository.StrictMock<IContextMenuBuilder>();

            menuBuilderMock.Expect(mb => mb.AddOpenItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(cmp => cmp.Get(failureMechanismContext, treeViewControl)).Return(menuBuilderMock);
                mocksRepository.ReplayAll();

                plugin.Gui = guiMock;

                // Call
                info.ContextMenuStrip(failureMechanismContext, null, treeViewControl);
            }

            // Assert
            // Assert is done in TearDown
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsNotRelevant_CallsContextMenuBuilderMethods()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism
            {
                IsRelevant = false
            };
            var assessmentSectionMock = mocksRepository.Stub<IAssessmentSection>();
            var failureMechanismContext = new DuneErosionFailureMechanismContext(failureMechanism, assessmentSectionMock);
            var guiMock = mocksRepository.StrictMock<IGui>();
            var menuBuilderMock = mocksRepository.StrictMock<IContextMenuBuilder>();

            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(cmp => cmp.Get(failureMechanismContext, treeViewControl)).Return(menuBuilderMock);
                mocksRepository.ReplayAll();

                plugin.Gui = guiMock;

                // Call
                info.ContextMenuStrip(failureMechanismContext, null, treeViewControl);
            }

            // Assert
            // Assert is done in TearDown
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsRelevant_AddCustomItems()
        {
            // Setup
            using (var treeView = new TreeViewControl())
            {
                var assessmentSectionMock = mocksRepository.Stub<IAssessmentSection>();
                var failureMechanism = new DuneErosionFailureMechanism();
                var failureMechanismContext = new DuneErosionFailureMechanismContext(failureMechanism, assessmentSectionMock);
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var guiMock = mocksRepository.StrictMock<IGui>();

                guiMock.Expect(cmp => cmp.Get(failureMechanismContext, treeView)).Return(menuBuilder);
                guiMock.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                guiMock.Stub(g => g.ProjectOpened -= null).IgnoreArguments();

                mocksRepository.ReplayAll();
                plugin.Gui = guiMock;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(failureMechanismContext, assessmentSectionMock, treeView))
                {
                    // Assert
                    Assert.AreEqual(8, menu.Items.Count);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuRelevancyIndexWhenRelevant,
                                                                  "I&s relevant",
                                                                  "Geeft aan of dit toetsspoor relevant is of niet.",
                                                                  RingtoetsCommonFormsResources.Checkbox_ticked);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsNotRelevant_AddCustomItems()
        {
            // Setup
            using (var treeView = new TreeViewControl())
            {
                var assessmentSectionMock = mocksRepository.Stub<IAssessmentSection>();
                var failureMechanism = new DuneErosionFailureMechanism
                {
                    IsRelevant = false
                };
                var failureMechanismContext = new DuneErosionFailureMechanismContext(failureMechanism, assessmentSectionMock);
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var guiMock = mocksRepository.StrictMock<IGui>();

                guiMock.Expect(cmp => cmp.Get(failureMechanismContext, treeView)).Return(menuBuilder);
                guiMock.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                guiMock.Stub(g => g.ProjectOpened -= null).IgnoreArguments();

                mocksRepository.ReplayAll();
                plugin.Gui = guiMock;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(failureMechanismContext, assessmentSectionMock, treeView))
                {
                    // Assert
                    Assert.AreEqual(4, menu.Items.Count);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuRelevancyIndexWhenNotRelevant,
                                                                  "I&s relevant",
                                                                  "Geeft aan of dit toetsspoor relevant is of niet.",
                                                                  RingtoetsCommonFormsResources.Checkbox_empty);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsRelevantAndClickOnIsRelevantItem_MakeFailureMechanismNotRelevantAndRemovesAllViewsForItem()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();
            var assessmentSectionStub = mocksRepository.Stub<IAssessmentSection>();
            var failureMechanismContext = new DuneErosionFailureMechanismContext(failureMechanism, assessmentSectionStub);
            var viewCommandsMock = mocksRepository.StrictMock<IViewCommands>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            var guiMock = mocksRepository.StrictMock<IGui>();

            viewCommandsMock.Expect(vs => vs.RemoveAllViewsForItem(failureMechanismContext));
            guiMock.Stub(g => g.ViewCommands).Return(viewCommandsMock);

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(g => g.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);

                mocksRepository.ReplayAll();
                plugin.Gui = guiMock;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl))
                {
                    // Call
                    contextMenu.Items[contextMenuRelevancyIndexWhenRelevant].PerformClick();

                    // Assert
                    Assert.IsFalse(failureMechanism.IsRelevant);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsNotRelevantAndClickOnIsRelevantItem_MakeFailureMechanismRelevantAndRemovesAllViewsForItem()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism
            {
                IsRelevant = false
            };
            var assessmentSectionStub = mocksRepository.Stub<IAssessmentSection>();
            var failureMechanismContext = new DuneErosionFailureMechanismContext(failureMechanism, assessmentSectionStub);
            var viewCommandsMock = mocksRepository.StrictMock<IViewCommands>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            var guiMock = mocksRepository.StrictMock<IGui>();

            viewCommandsMock.Expect(vs => vs.RemoveAllViewsForItem(failureMechanismContext));
            guiMock.Stub(g => g.ViewCommands).Return(viewCommandsMock);

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(g => g.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);

                mocksRepository.ReplayAll();
                plugin.Gui = guiMock;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl))
                {
                    // Call
                    contextMenu.Items[contextMenuRelevancyIndexWhenNotRelevant].PerformClick();

                    // Assert
                    Assert.IsTrue(failureMechanism.IsRelevant);
                }
            }
        }
    }
}