﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Windows.Forms;
using Core.Common.Base;
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
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Plugin;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class GrassCoverErosionOutwardsFailureMechanismContextTreeNodeInfoTest
    {
        private MockRepository mocks;
        private GrassCoverErosionOutwardsPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new GrassCoverErosionOutwardsPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(GrassCoverErosionOutwardsFailureMechanismContext));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
            mocks.VerifyAll();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            mocks.ReplayAll();

            // Assert
            Assert.AreEqual(typeof(GrassCoverErosionOutwardsFailureMechanismContext), info.TagType);

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
        public void Text_Always_ReturnName()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var failureMechanismContext = new GrassCoverErosionOutwardsFailureMechanismContext(failureMechanism,
                                                                                               assessmentSection);

            // Call
            string nodeText = info.Text(failureMechanismContext);

            // Assert
            Assert.AreEqual(failureMechanism.Name, nodeText);
        }

        [Test]
        public void ForeColor_FailureMechanismIsRelevant_ReturnControlText()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                IsRelevant = true
            };
            var failureMechanismContext = new GrassCoverErosionOutwardsFailureMechanismContext(failureMechanism,
                                                                                               assessmentSection);

            // Call
            Color foreColor = info.ForeColor(failureMechanismContext);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), foreColor);
        }

        [Test]
        public void ForeColor_FailureMechanismIsNotRelevant_ReturnGrayText()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                IsRelevant = false
            };
            var failureMechanismContext = new GrassCoverErosionOutwardsFailureMechanismContext(failureMechanism,
                                                                                               assessmentSection);

            // Call
            Color foreColor = info.ForeColor(failureMechanismContext);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), foreColor);
        }

        [Test]
        public void Image_Always_ReturnFailureMechanismIcon()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var failureMechanismContext = new GrassCoverErosionOutwardsFailureMechanismContext(failureMechanism,
                                                                                               assessmentSection);

            // Call
            Image icon = info.Image(failureMechanismContext);

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.FailureMechanismIcon, icon);
        }

        [Test]
        public void ChildNodeObjects_FailureMechanismIsRelevant_ReturnChildDataNodes()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                IsRelevant = true
            };
            var failureMechanismContext = new GrassCoverErosionOutwardsFailureMechanismContext(failureMechanism,
                                                                                               assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(failureMechanismContext).ToArray();

            // Assert
            Assert.AreEqual(3, children.Length);
            var inputsFolder = (CategoryTreeFolder) children[0];
            Assert.AreEqual("Invoer", inputsFolder.Name);
            Assert.AreEqual(TreeFolderCategory.Input, inputsFolder.Category);

            Assert.AreEqual(2, inputsFolder.Contents.Count);
            var failureMechanismSectionsContext = (FailureMechanismSectionsContext) inputsFolder.Contents[0];
            Assert.AreSame(failureMechanism, failureMechanismSectionsContext.WrappedData);
            Assert.AreSame(assessmentSection, failureMechanismSectionsContext.ParentAssessmentSection);

            var commentContext = (CommentContext<ICommentable>) inputsFolder.Contents[1];
            Assert.AreSame(failureMechanism, commentContext.WrappedData);

            var hydraulicBoundariesGroupContext = (HydraulicBoundariesGroupContext) children[1];
            Assert.AreSame(failureMechanism, hydraulicBoundariesGroupContext.WrappedData);

            var outputsFolder = (CategoryTreeFolder) children[2];
            Assert.AreEqual("Oordeel", outputsFolder.Name);
            Assert.AreEqual(TreeFolderCategory.Output, outputsFolder.Category);
            Assert.AreEqual(1, outputsFolder.Contents.Count);

            var failureMechanismResultsContext = (FailureMechanismSectionResultContext<GrassCoverErosionOutwardsFailureMechanismSectionResult>)
                                                 outputsFolder.Contents[0];
            Assert.AreSame(failureMechanism, failureMechanismResultsContext.FailureMechanism);
            Assert.AreSame(failureMechanism.SectionResults, failureMechanismResultsContext.WrappedData);
        }

        [Test]
        public void ChildNodeObjects_FailureMechanismIsNotRelevant_ReturnChildDataNodes()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                IsRelevant = false
            };
            var failureMechanismContext = new GrassCoverErosionOutwardsFailureMechanismContext(failureMechanism,
                                                                                               assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(failureMechanismContext).ToArray();

            // Assert
            Assert.AreEqual(1, children.Length);
            var commentContext = (CommentContext<ICommentable>) children[0];
            Assert.AreSame(failureMechanism, commentContext.WrappedData);
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsRelevant_CallsContextMenuBuilderMethods()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
                {
                    IsRelevant = true
                };
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var failureMechanismContext = new GrassCoverErosionOutwardsFailureMechanismContext(failureMechanism,
                                                                                                   assessmentSection);

                var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.Build()).Return(null);

                var gui = mocks.StrictMock<IGui>();
                gui.Expect(cmp => cmp.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);

                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(failureMechanismContext, null, treeViewControl);
            }

            // Assert
            // Assert expectancies are called in TearDown()
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsNotRelevant_CallsContextMenuBuilderMethods()
        {
            // Setup
            var treeViewControl = new TreeViewControl();
            {
                var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
                {
                    IsRelevant = false
                };
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var failureMechanismContext = new GrassCoverErosionOutwardsFailureMechanismContext(failureMechanism, assessmentSection);

                var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.Build()).Return(null);

                var gui = mocks.StrictMock<IGui>();
                gui.Expect(cmp => cmp.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);

                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(failureMechanismContext, null, treeViewControl);
            }

            // Assert
            // Assert expectancies are called in TearDown()
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsRelevantAndClickOnIsRelevantItem_MakeFailureMechanismNotRelevant()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var failureMechanismObserver = mocks.Stub<IObserver>();
                failureMechanismObserver.Expect(o => o.UpdateObserver());

                var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
                {
                    IsRelevant = true
                };
                failureMechanism.Attach(failureMechanismObserver);

                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var failureMechanismContext = new GrassCoverErosionOutwardsFailureMechanismContext(failureMechanism,
                                                                                                   assessmentSection);

                var viewCommands = mocks.StrictMock<IViewCommands>();
                viewCommands.Expect(vs => vs.RemoveAllViewsForItem(failureMechanismContext));

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.StrictMock<IGui>();
                gui.Stub(g => g.ViewCommands).Return(viewCommands);
                gui.Expect(g => g.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);

                mocks.ReplayAll();

                plugin.Gui = gui;

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
        public void ContextMenuStrip_FailureMechanismIsNotRelevantAndClickOnIsRelevantItem_MakeFailureMechanismRelevant()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var failureMechanismObserver = mocks.Stub<IObserver>();
                failureMechanismObserver.Expect(o => o.UpdateObserver());

                var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
                {
                    IsRelevant = false
                };
                failureMechanism.Attach(failureMechanismObserver);

                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var failureMechanismContext = new GrassCoverErosionOutwardsFailureMechanismContext(failureMechanism,
                                                                                                   assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.StrictMock<IGui>();
                gui.Expect(g => g.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);

                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl))
                {
                    // Call
                    contextMenu.Items[contextMenuRelevancyIndexWhenNotRelevant].PerformClick();

                    // Assert
                    Assert.IsTrue(failureMechanism.IsRelevant);
                }
            }
        }

        private const int contextMenuRelevancyIndexWhenRelevant = 0;
        private const int contextMenuRelevancyIndexWhenNotRelevant = 0;
    }
}