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
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Plugin;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using GrassCoverErosionInwardsFormsResources = Ringtoets.GrassCoverErosionInwards.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class GrassCoverErosionInwardsFailureMechanismContextTreeNodeInfoTest : NUnitFormTest
    {
        private MockRepository mocksRepository;
        private GrassCoverErosionInwardsGuiPlugin plugin;
        private TreeNodeInfo info;

        private const int contextMenuRelevancyIndex = 1;

        [SetUp]
        public void SetUp()
        {
            mocksRepository = new MockRepository();
            plugin = new GrassCoverErosionInwardsGuiPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(GrassCoverErosionInwardsFailureMechanismContext));
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(GrassCoverErosionInwardsFailureMechanismContext), info.TagType);
            Assert.IsNotNull(info.ForeColor);
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
        public void Text_Always_ReturnsName()
        {
            // Setup
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            mocksRepository.ReplayAll();

            var mechanism = new GrassCoverErosionInwardsFailureMechanism();
            var mechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(mechanism, assessmentSection);

            // Call
            var text = info.Text(mechanismContext);

            // Assert
            const string expectedName = "Dijken - Grasbekleding erosie kruin en binnentalud";
            Assert.AreEqual(expectedName, text);
            mocksRepository.VerifyAll();
        }

        [Test]
        public void Image_Always_ReturnsFailureMechanismIcon()
        {
            // Call
            var image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.FailureMechanismIcon, image);
        }

        [Test]
        public void ChildNodeObjects_FailureMechanismIsRelevant_ReturnChildDataNodes()
        {
            // Setup
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            mocksRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            var children = info.ChildNodeObjects(failureMechanismContext).ToArray();

            // Assert
            Assert.AreEqual(3, children.Length);
            var inputsFolder = (CategoryTreeFolder) children[0];
            Assert.AreEqual("Invoer", inputsFolder.Name);
            Assert.AreEqual(TreeFolderCategory.Input, inputsFolder.Category);

            Assert.AreEqual(2, inputsFolder.Contents.Count);
            var failureMechanismSectionsContext = (FailureMechanismSectionsContext) inputsFolder.Contents[0];
            CollectionAssert.AreEqual(failureMechanism.Sections, failureMechanismSectionsContext.WrappedData);
            Assert.AreSame(failureMechanism, failureMechanismSectionsContext.ParentFailureMechanism);
            Assert.AreSame(assessmentSection, failureMechanismSectionsContext.ParentAssessmentSection);

            var commentContext = (CommentContext<ICommentable>) inputsFolder.Contents[1];
            Assert.AreSame(failureMechanism, commentContext.CommentContainer);

            var calculationsFolder = (GrassCoverErosionInwardsCalculationGroupContext) children[1];
            Assert.AreEqual("Berekeningen", calculationsFolder.WrappedData.Name);
            CollectionAssert.AreEqual(failureMechanism.CalculationsGroup.Children, calculationsFolder.WrappedData.Children);
            Assert.AreSame(failureMechanism, calculationsFolder.FailureMechanism);

            var outputsFolder = (CategoryTreeFolder) children[2];
            Assert.AreEqual("Uitvoer", outputsFolder.Name);
            Assert.AreEqual(TreeFolderCategory.Output, outputsFolder.Category);

            var failureMechanismResultsContext = (FailureMechanismSectionResultContext) outputsFolder.Contents[0];
            Assert.AreSame(failureMechanism, failureMechanismResultsContext.FailureMechanism);
            Assert.AreSame(failureMechanism.SectionResults, failureMechanismResultsContext.SectionResults);
            mocksRepository.VerifyAll();
        }

        [Test]
        public void ChildNodeObjects_FailureMechanismIsNotRelevant_ReturnOnlyFailureMechanismComments()
        {
            // Setup
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            mocksRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                IsRelevant = false
            };
            var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            var children = info.ChildNodeObjects(failureMechanismContext).ToArray();

            // Assert
            Assert.AreEqual(1, children.Length);
            var commentContext = (CommentContext<ICommentable>)children[0];
            Assert.AreSame(failureMechanism, commentContext.CommentContainer);

            mocksRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsRelevant_CallsContextMenuBuilderMethods()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, assessmentSection);

            var guiMock = mocksRepository.StrictMock<IGui>();
            var treeViewControlMock = mocksRepository.StrictMock<TreeViewControl>();
            var menuBuilderMock = mocksRepository.StrictMock<IContextMenuBuilder>();

            menuBuilderMock.Expect(mb => mb.AddOpenItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddImportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            guiMock.Expect(cmp => cmp.Get(failureMechanismContext, treeViewControlMock)).Return(menuBuilderMock);
            mocksRepository.ReplayAll();

            plugin.Gui = guiMock;

            // Call
            info.ContextMenuStrip(failureMechanismContext, null, treeViewControlMock);

            // Assert
            mocksRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsNotRelevant_CallsContextMenuBuilderMethods()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                IsRelevant = false
            };
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, assessmentSection);

            var guiMock = mocksRepository.StrictMock<IGui>();
            var treeViewControlMock = mocksRepository.StrictMock<TreeViewControl>();
            var menuBuilderMock = mocksRepository.StrictMock<IContextMenuBuilder>();

            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            guiMock.Expect(cmp => cmp.Get(failureMechanismContext, treeViewControlMock)).Return(menuBuilderMock);
            mocksRepository.ReplayAll();

            plugin.Gui = guiMock;

            // Call
            info.ContextMenuStrip(failureMechanismContext, null, treeViewControlMock);

            // Assert
            mocksRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsRelevant_IsRelevantEnabledAddCalculationGroupAddCalculationItemDisabled()
        {
            // Setup
            using (var treeView = new TreeViewControl())
            {
                var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
                var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
                var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, assessmentSection);

                var menuBuilderMock = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocksRepository.StrictMock<IGui>();
                gui.Expect(cmp => cmp.Get(failureMechanismContext, treeView)).Return(menuBuilderMock);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();

                mocksRepository.ReplayAll();

                plugin.Gui = gui;

                // Call
                var menu = info.ContextMenuStrip(failureMechanismContext, assessmentSection, treeView);

                // Assert
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuRelevancyIndex,
                                                              RingtoetsCommonFormsResources.FailureMechanismContextMenuStrip_Is_relevant,
                                                              RingtoetsCommonFormsResources.FailureMechanismContextMenuStrip_Is_relevant_Tooltip,
                                                              RingtoetsCommonFormsResources.Checkbox_ticked);

                mocksRepository.VerifyAll();
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsRelevantAndClickOnIsRelevantItem_MakeFailureMechanismNotRelevant()
        {
            // Setup
            var failureMechanismObserver = mocksRepository.Stub<IObserver>();
            failureMechanismObserver.Expect(o => o.UpdateObserver());

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                IsRelevant = true
            };
            failureMechanism.Attach(failureMechanismObserver);

            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, assessmentSection);

            var viewCommands = mocksRepository.StrictMock<IViewCommands>();
            viewCommands.Expect(vs => vs.RemoveAllViewsForItem(failureMechanismContext));

            var treeViewControl = mocksRepository.StrictMock<TreeViewControl>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var gui = mocksRepository.StrictMock<IGui>();
            gui.Stub(g => g.ViewCommands).Return(viewCommands);
            gui.Expect(g => g.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);

            mocksRepository.ReplayAll();

            plugin.Gui = gui;

            var contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl);

            // Call
            contextMenu.Items[contextMenuRelevancyIndex].PerformClick();

            // Assert
            Assert.IsFalse(failureMechanism.IsRelevant);
            mocksRepository.VerifyAll();
        }
    }
}