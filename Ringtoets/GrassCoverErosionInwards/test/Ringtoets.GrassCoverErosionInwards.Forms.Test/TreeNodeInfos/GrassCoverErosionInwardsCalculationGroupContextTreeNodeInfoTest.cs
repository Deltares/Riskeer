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

using System;
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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Plugin;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;
using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using GrassCoverErosionInwardsFormResources = Ringtoets.GrassCoverErosionInwards.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationGroupContextTreeNodeInfoTest
    {
        private MockRepository mocks;
        private GrassCoverErosionInwardsGuiPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new GrassCoverErosionInwardsGuiPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(GrassCoverErosionInwardsCalculationGroupContext));
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(GrassCoverErosionInwardsCalculationGroupContext), info.TagType);
            Assert.IsNull(info.ForeColor);
            Assert.IsNull(info.CanCheck);
            Assert.IsNull(info.IsChecked);
            Assert.IsNull(info.OnNodeChecked);
        }

        [Test]
        public void Text_Always_ReturnsWrappedDataName()
        {
            // Setup
            var testname = "testName";
            var group = new CalculationGroup
            {
                Name = testname
            };
            var failureMechanismMock = mocks.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var groupContext = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                                   failureMechanismMock,
                                                                                   assessmentSectionMock);

            // Call
            var text = info.Text(groupContext);

            // Assert
            Assert.AreEqual(testname, text);
            mocks.VerifyAll();
        }

        [Test]
        public void Image_Always_FolderIcon()
        {
            // Call
            var image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(Resources.GeneralFolderIcon, image);
        }

        [Test]
        public void EnsureVisibleOnCreate_Always_ReturnsTrue()
        {
            // Setup
            var group = new CalculationGroup();
            var failureMechanismMock = mocks.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var groupContext = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                                   failureMechanismMock,
                                                                                   assessmentSectionMock);

            // Call
            var result = info.EnsureVisibleOnCreate(groupContext);

            // Assert
            Assert.IsTrue(result);
            mocks.VerifyAll();
        }

        [Test]
        public void ChildNodeObjects_EmptyGroup_ReturnEmpty()
        {
            // Setup
            var group = new CalculationGroup();
            var failureMechanismMock = mocks.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var groupContext = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                 failureMechanismMock,
                                                                 assessmentSectionMock);

            // Call
            var children = info.ChildNodeObjects(groupContext);

            // Assert
            CollectionAssert.IsEmpty(children);
            mocks.VerifyAll();
        }

        [Test]
        public void ChildNodeObjects_GroupWithMixedContents_ReturnChildren()
        {
            // Setup
            var group = new CalculationGroup();
            var childGroup = new CalculationGroup();
            var calculationItem = mocks.StrictMock<ICalculationBase>();
            var childCalculation = new GrassCoverErosionInwardsCalculation();

            group.Children.Add(childGroup);
            group.Children.Add(calculationItem);
            group.Children.Add(childCalculation);

            var failureMechanismMock = mocks.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var groupContext = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                 failureMechanismMock,
                                                                 assessmentSectionMock);

            // Call
            var children = info.ChildNodeObjects(groupContext).ToArray();

            // Assert
            Assert.AreEqual(group.Children.Count, children.Length);            
            var calculationGroupContext = (GrassCoverErosionInwardsCalculationGroupContext) children[0];
            Assert.AreSame(childGroup, calculationGroupContext.WrappedData);
            Assert.AreSame(failureMechanismMock, calculationGroupContext.GrassCoverErosionInwardsFailureMechanism);
            Assert.AreSame(assessmentSectionMock, calculationGroupContext.AssessmentSection);
            Assert.AreSame(calculationItem, children[1]);
            var calculationContext = (GrassCoverErosionInwardsCalculationContext) children[2];
            Assert.AreSame(childCalculation, calculationContext.WrappedData);
            Assert.AreSame(assessmentSectionMock, calculationContext.AssessmentSection);
        }

        [Test]
        public void ContextmenuStrip_FailureMechanismContextParent_ReturnContextMenuWithoutRenameRemove()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var group = new CalculationGroup();

            var failureMechanismMock = mocks.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var parentData = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanismMock, assessmentSectionMock);
            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               failureMechanismMock, 
                                                                               assessmentSectionMock);

            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var exportImportHandler = mocks.Stub<IExportImportCommandHandler>();
            var viewCommandsHandler = mocks.StrictMock<IViewCommands>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();

            var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler, exportImportHandler, viewCommandsHandler, nodeData, treeViewControl);
            gui.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
            
            treeViewControl.Expect(tvc => tvc.CanExpandOrCollapseForData(nodeData)).Repeat.Twice().Return(false);
            viewCommandsHandler.Expect(vc => vc.CanOpenViewFor(nodeData)).Return(false);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            ContextMenuStrip menu = info.ContextMenuStrip(nodeData, parentData, treeViewControl);

            // Assert
            var mainCalculationGroupContextItemOffset = 2;
            Assert.AreEqual(12, menu.Items.Count);
            TestHelper.AssertContextMenuStripContainsItem(menu, 0,
                                                          CoreCommonGuiResources.Open,
                                                          CoreCommonGuiResources.Open_ToolTip,
                                                          CoreCommonGuiResources.OpenIcon,
                                                          false);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationGroupIndex + mainCalculationGroupContextItemOffset,
                                                          RingtoetsFormsResources.CalculationGroup_Add_CalculationGroup,
                                                          "Voeg een nieuwe berekeningsmap toe aan dit faalmechanisme.",
                                                          RingtoetsFormsResources.AddFolderIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationItemIndex + mainCalculationGroupContextItemOffset,
                                                          RingtoetsFormsResources.CalculationGroup_Add_Calculation,
                                                          "Voeg een nieuwe grasbekleding erosie kruin en binnentalud berekening toe aan dit faalmechanisme.",
                                                          GrassCoverErosionInwardsFormResources.CalculationIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, 5,
                                                          CoreCommonGuiResources.Import,
                                                          CoreCommonGuiResources.Import_ToolTip,
                                                          CoreCommonGuiResources.ImportIcon,
                                                          false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 6,
                                                          CoreCommonGuiResources.Export,
                                                          CoreCommonGuiResources.Export_ToolTip,
                                                          CoreCommonGuiResources.ExportIcon,
                                                          false);

            TestHelper.AssertContextMenuStripContainsItem(menu, 8,
                                                          CoreCommonGuiResources.Expand_all,
                                                          CoreCommonGuiResources.Expand_all_ToolTip,
                                                          CoreCommonGuiResources.ExpandAllIcon,
                                                          false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 9,
                                                          CoreCommonGuiResources.Collapse_all,
                                                          CoreCommonGuiResources.Collapse_all_ToolTip,
                                                          CoreCommonGuiResources.CollapseAllIcon,
                                                          false);

            TestHelper.AssertContextMenuStripContainsItem(menu, 11,
                                                          CoreCommonGuiResources.Properties,
                                                          CoreCommonGuiResources.Properties_ToolTip,
                                                          CoreCommonGuiResources.PropertiesHS,
                                                          false);
            CollectionAssert.AllItemsAreInstancesOfType(new[]
            {
                menu.Items[1],
                menu.Items[4],
                menu.Items[7],
                menu.Items[10]
            }, typeof(ToolStripSeparator));

            mocks.VerifyAll();
        }

        [Test]
        public void ContextmenuStrip_ChildOfGroup_ReturnContextMenuWithAllItems()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var parentGroup = new CalculationGroup();
            var group = new CalculationGroup();

            parentGroup.Children.Add(group);

            var failureMechanismMock = mocks.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var parentData = new GrassCoverErosionInwardsCalculationGroupContext(parentGroup,
                                                                               failureMechanismMock,
                                                                               assessmentSectionMock);

            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               failureMechanismMock, 
                                                                               assessmentSectionMock);

            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var exportImportHandler = mocks.Stub<IExportImportCommandHandler>();
            var viewCommandsHandler = mocks.StrictMock<IViewCommands>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();

            var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler, exportImportHandler, viewCommandsHandler, nodeData, treeViewControl);
            gui.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);

            treeViewControl.Expect(tvc => tvc.CanRenameNodeForData(nodeData)).Return(true);
            treeViewControl.Expect(tvc => tvc.CanRemoveNodeForData(nodeData)).Return(true);
            treeViewControl.Expect(tvc => tvc.CanExpandOrCollapseForData(nodeData)).Repeat.Twice().Return(false);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            ContextMenuStrip menu = info.ContextMenuStrip(nodeData, parentData, treeViewControl);

            // Assert
            Assert.AreEqual(13, menu.Items.Count);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationGroupIndex,
                                                          RingtoetsFormsResources.CalculationGroup_Add_CalculationGroup,
                                                          "Voeg een nieuwe berekeningsmap toe aan dit faalmechanisme.",
                                                          RingtoetsFormsResources.AddFolderIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationItemIndex,
                                                          RingtoetsFormsResources.CalculationGroup_Add_Calculation,
                                                          "Voeg een nieuwe grasbekleding erosie kruin en binnentalud berekening toe aan dit faalmechanisme.",
                                                          GrassCoverErosionInwardsFormResources.CalculationIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, 3,
                                                          CoreCommonGuiResources.Rename,
                                                          CoreCommonGuiResources.Rename_ToolTip,
                                                          CoreCommonGuiResources.RenameIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, 4,
                                                          CoreCommonGuiResources.Delete,
                                                          CoreCommonGuiResources.Delete_ToolTip,
                                                          CoreCommonGuiResources.DeleteIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, 6,
                                                          CoreCommonGuiResources.Import,
                                                          CoreCommonGuiResources.Import_ToolTip,
                                                          CoreCommonGuiResources.ImportIcon,
                                                          false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 7,
                                                          CoreCommonGuiResources.Export,
                                                          CoreCommonGuiResources.Export_ToolTip,
                                                          CoreCommonGuiResources.ExportIcon,
                                                          false);

            TestHelper.AssertContextMenuStripContainsItem(menu, 9,
                                                          CoreCommonGuiResources.Expand_all,
                                                          CoreCommonGuiResources.Expand_all_ToolTip,
                                                          CoreCommonGuiResources.ExpandAllIcon,
                                                          false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 10,
                                                          CoreCommonGuiResources.Collapse_all,
                                                          CoreCommonGuiResources.Collapse_all_ToolTip,
                                                          CoreCommonGuiResources.CollapseAllIcon,
                                                          false);

            TestHelper.AssertContextMenuStripContainsItem(menu, 12,
                                                          CoreCommonGuiResources.Properties,
                                                          CoreCommonGuiResources.Properties_ToolTip,
                                                          CoreCommonGuiResources.PropertiesHS,
                                                          false);
            CollectionAssert.AllItemsAreInstancesOfType(new[]
            {
                menu.Items[2],
                menu.Items[5],
                menu.Items[8],
                menu.Items[11]
            }, typeof(ToolStripSeparator));

            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_ClickOnAddGroupItem_AddGroupToCalculationGroupAndNotifyObservers()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            var group = new CalculationGroup();
            var failureMechanismMock = mocks.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                             failureMechanismMock,
                                                             assessmentSectionMock);

            var calculationItem = mocks.Stub<ICalculationBase>();
            calculationItem.Name = "Nieuwe map";

            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var treeViewControl = mocks.StrictMock<TreeViewControl>();

            gui.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);

            mocks.ReplayAll();

            plugin.Gui = gui;

            group.Children.Add(calculationItem);

            nodeData.Attach(observer);

            ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl);

            // Precondition
            Assert.AreEqual(1, group.Children.Count);

            // Call
            contextMenu.Items[contextMenuAddCalculationGroupIndex].PerformClick();

            // Assert
            Assert.AreEqual(2, group.Children.Count);
            var newlyAddedItem = group.Children.Last();
            Assert.IsInstanceOf<CalculationGroup>(newlyAddedItem);
            Assert.AreEqual("Nieuwe map (1)", newlyAddedItem.Name,
                            "An item with the same name default name already exists, therefore '(1)' needs to be appended.");

            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_ClickOnAddCalculationItem_AddCalculationToCalculationGroupAndNotifyObservers()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            var group = new CalculationGroup();
            var failureMechanismMock = mocks.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                             failureMechanismMock,
                                                             assessmentSectionMock);

            var calculationItem = mocks.Stub<ICalculationBase>();
            calculationItem.Name = "Nieuwe berekening";

            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var treeViewControl = mocks.StrictMock<TreeViewControl>();

            gui.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);

            mocks.ReplayAll();

            plugin.Gui = gui;

            group.Children.Add(calculationItem);

            nodeData.Attach(observer);

            var contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl);

            // Precondition
            Assert.AreEqual(1, group.Children.Count);

            // Call
            contextMenu.Items[contextMenuAddCalculationItemIndex].PerformClick();

            // Assert
            Assert.AreEqual(2, group.Children.Count);
            var newlyAddedItem = group.Children.Last();
            Assert.IsInstanceOf<GrassCoverErosionInwardsCalculation>(newlyAddedItem);
            Assert.AreEqual("Nieuwe berekening (1)", newlyAddedItem.Name,
                            "An item with the same name default name already exists, therefore '(1)' needs to be appended.");

            mocks.VerifyAll();
        }

        [Test]
        public void CanRenameNode_ParentIsGrassCoverErosionInwardsFailureMechanismContext_ReturnFalse()
        {
            // Setup
            var failureMechanismMock = mocks.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var failureMechanismContextMock = mocks.StrictMock<GrassCoverErosionInwardsFailureMechanismContext>(failureMechanismMock, assessmentSectionMock);

            mocks.ReplayAll();

            // Call
            bool isRenamingAllowed = info.CanRename(null, failureMechanismContextMock);

            // Assert
            Assert.IsFalse(isRenamingAllowed);
            mocks.VerifyAll();
        }

        [Test]
        public void CanRenameNode_EverythingElse_ReturnTrue()
        {
            // Call
            bool isRenamingAllowed = info.CanRename(null, null);

            // Assert
            Assert.IsTrue(isRenamingAllowed);
            mocks.VerifyAll();
        }

        [Test]
        public void OnNodeRenamed_WithData_RenameGroupAndNotifyObservers()
        {
            // Setup
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            var group = new CalculationGroup();
            var failureMechanismMock = mocks.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                             failureMechanismMock,
                                                             assessmentSectionMock);
            nodeData.Attach(observer);

            // Call
            const string newName = "new name";
            info.OnNodeRenamed(nodeData, newName);

            // Assert
            Assert.AreEqual(newName, group.Name);
            mocks.VerifyAll();
        }

        [Test]
        public void CanRemove_ParentIsFailureMechanism_ReturnFalse()
        {
            // Setup
            var group = new CalculationGroup();
            var failureMechanismMock = mocks.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                             failureMechanismMock,
                                                             assessmentSectionMock);

            var parentNodeData = new GrassCoverErosionInwardsFailureMechanism();
            parentNodeData.CalculationsGroup.Children.Add(group);

            mocks.ReplayAll();

            // Call
            bool isRemovalAllowed = info.CanRemove(nodeData, parentNodeData);

            // Assert
            Assert.IsFalse(isRemovalAllowed);
            mocks.VerifyAll();
        }

        [Test]
        public void CanRemove_ParentIsGrasCoverErosionInwardsCalculationGroupContainingGroup_ReturnTrue()
        {
            // Setup
            var group = new CalculationGroup();
            var failureMechanismMock = mocks.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                             failureMechanismMock,
                                                             assessmentSectionMock);

            var parentGroup = new CalculationGroup();
            parentGroup.Children.Add(group);
            var parentNodeData = new GrassCoverErosionInwardsCalculationGroupContext(parentGroup,
                                                                   failureMechanismMock,
                                                                   assessmentSectionMock);

            mocks.ReplayAll();

            // Call
            bool isRemovalAllowed = info.CanRemove(nodeData, parentNodeData);

            // Assert
            Assert.IsTrue(isRemovalAllowed);
            mocks.VerifyAll();
        }

        [Test]
        public void CanRemove_ParentIsGrasCoverErosionInwardsCalculationGroupNotContainingGroup_ReturnFalse()
        {
            // Setup
            var group = new CalculationGroup();
            var failureMechanismMock = mocks.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                             failureMechanismMock,
                                                             assessmentSectionMock);

            var parentGroup = new CalculationGroup();
            var parentNodeData = new GrassCoverErosionInwardsCalculationGroupContext(parentGroup,
                                                                   failureMechanismMock,
                                                                   assessmentSectionMock);

            // Precondition
            CollectionAssert.DoesNotContain(parentGroup.Children, group);

            // Call
            bool isRemovalAllowed = info.CanRemove(nodeData, parentNodeData);

            // Assert
            Assert.IsFalse(isRemovalAllowed);
        }

        [Test]
        public void OnNodeRemoved_ParentIsGrassCoverErosionInwardsCalculationGroupContainingGroup_RemoveGroupAndNotifyObservers()
        {
            // Setup
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            var group = new CalculationGroup();
            var failureMechanismMock = mocks.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                             failureMechanismMock,
                                                             assessmentSectionMock);

            var parentGroup = new CalculationGroup();
            parentGroup.Children.Add(group);
            var parentNodeData = new GrassCoverErosionInwardsCalculationGroupContext(parentGroup,
                                                                   failureMechanismMock,
                                                                   assessmentSectionMock);
            parentNodeData.Attach(observer);

            // Precondition
            Assert.IsTrue(info.CanRemove(nodeData, parentNodeData));

            // Call
            info.OnNodeRemoved(nodeData, parentNodeData);

            // Assert
            CollectionAssert.DoesNotContain(parentGroup.Children, group);
            mocks.VerifyAll();
        }

        [Test]
        public void OnNodeRemoved_ParentIsGrassCoverErosionInwardsCalculationGroupContainingGroupContainingCalculations_RemoveGroupAndCalculationsAndNotifyObservers()
        {
            // Setup
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            var group = new CalculationGroup();
            var failureMechanismMock = mocks.StrictMock<GrassCoverErosionInwardsFailureMechanism>();

            var calculation = new GrassCoverErosionInwardsCalculation();

            group.Children.Add(calculation);

            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                             failureMechanismMock,
                                                             assessmentSectionMock);

            var parentGroup = new CalculationGroup();
            parentGroup.Children.Add(group);
            var parentNodeData = new GrassCoverErosionInwardsCalculationGroupContext(parentGroup,
                                                                   failureMechanismMock,
                                                                   assessmentSectionMock);
            parentNodeData.Attach(observer);

            // Precondition
            Assert.IsTrue(info.CanRemove(nodeData, parentNodeData));

            // Call
            info.OnNodeRemoved(nodeData, parentNodeData);

            // Assert
            CollectionAssert.DoesNotContain(parentGroup.Children, group);
            mocks.VerifyAll();
        }

        [Test]
        public void CanDrag_WithParentNodeDefaultBehavior_ReturnTrue()
        {
            // Setup
            var group = new CalculationGroup();
            var failureMechanismMock = mocks.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var groupContext = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                 failureMechanismMock,
                                                                 assessmentSectionMock);

            // Call
            var canDrag = info.CanDrag(groupContext, null);

            // Assert
            Assert.IsTrue(canDrag);
        }

        [Test]
        public void CanDrag_ParentIsGrassCoverErosionInwardsFailureMechanismContext_ReturnFalse()
        {
            // Setup
            var group = new CalculationGroup();
            var failureMechanismMock = mocks.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var failureMechanismContextMock = mocks.StrictMock<GrassCoverErosionInwardsFailureMechanismContext>(failureMechanismMock, assessmentSectionMock);

            mocks.ReplayAll();

            var groupContext = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                 failureMechanismMock,
                                                                 assessmentSectionMock);

            // Call
            var canDrag = info.CanDrag(groupContext, failureMechanismContextMock);

            // Assert
            Assert.IsFalse(canDrag);
        }

        [Test]
        [Combinatorial]
        public void CanDropOrCanInsert_DraggingPipingCalculationItemContextOntoGroupNotContainingItem_ReturnTrue(
            [Values(DragDropTestMethod.CanDrop, DragDropTestMethod.CanInsert)] DragDropTestMethod methodToTest,
            [Values(CalculationType.Calculation, CalculationType.Group)] CalculationType draggedItemType)
        {
            // Setup
            ICalculationBase draggedItem;
            object draggedItemContext;

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();

            mocks.ReplayAll();

            CreateCalculationAndContext(draggedItemType, out draggedItem, out draggedItemContext, failureMechanism, assessmentSection);

            CalculationGroup targetGroup;
            GrassCoverErosionInwardsCalculationGroupContext targetGroupContext;
            CreateCalculationGroupAndContext(out targetGroup, out targetGroupContext, failureMechanism, assessmentSection);

            failureMechanism.CalculationsGroup.Children.Add(draggedItem);
            failureMechanism.CalculationsGroup.Children.Add(targetGroup);

            switch (methodToTest)
            {
                case DragDropTestMethod.CanDrop:
                    // Call
                    var canDrop = info.CanDrop(draggedItemContext, targetGroupContext);

                    // Assert
                    Assert.IsTrue(canDrop);
                    break;
                case DragDropTestMethod.CanInsert:
                    // Call
                    bool canInsert = info.CanInsert(draggedItemContext, targetGroupContext);

                    // Assert
                    Assert.IsTrue(canInsert);
                    break;
                default:
                    Assert.Fail(methodToTest + " not supported.");
                    break;
            }
            mocks.VerifyAll();
        }

        [Test]
        [Combinatorial]
        public void CanDropOrInsert_DraggingCalculationItemContextOntoGroupNotContainingItemOtherFailureMechanism_ReturnFalse(
            [Values(DragDropTestMethod.CanDrop, DragDropTestMethod.CanInsert)] DragDropTestMethod methodToTest,
            [Values(CalculationType.Calculation, CalculationType.Group)] CalculationType draggedItemType)
        {
            // Setup
            ICalculationBase draggedItem;
            object draggedItemContext;

            var targetFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();

            mocks.ReplayAll();

            CreateCalculationAndContext(draggedItemType, out draggedItem, out draggedItemContext, targetFailureMechanism, assessmentSection);

            var sourceFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            sourceFailureMechanism.CalculationsGroup.Children.Add(draggedItem);

            CalculationGroup targetGroup;
            GrassCoverErosionInwardsCalculationGroupContext targetGroupContext;
            CreateCalculationGroupAndContext(out targetGroup, out targetGroupContext, sourceFailureMechanism, assessmentSection);

            targetFailureMechanism.CalculationsGroup.Children.Add(targetGroup);

            switch (methodToTest)
            {
                case DragDropTestMethod.CanDrop:
                    // Call
                    var canDrop = info.CanDrop(draggedItemContext, targetGroupContext);

                    // Assert
                    Assert.IsFalse(canDrop);
                    break;
                case DragDropTestMethod.CanInsert:
                    // Call
                    bool canInsert = info.CanInsert(draggedItemContext, targetGroupContext);

                    // Assert
                    Assert.IsFalse(canInsert);
                    break;
                default:
                    Assert.Fail(methodToTest + " not supported.");
                    break;
            }
            mocks.VerifyAll();
        }

        [Test]
        [Combinatorial]
        public void OnDrop_DraggingPipingCalculationItemContextOntoGroupEnd_MoveCalculationItemInstanceToNewGroup(
            [Values(CalculationType.Calculation, CalculationType.Group)] CalculationType draggedItemType)
        {
            // Setup
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var pipingFailureMechanismMock = mocks.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();

            ICalculationBase draggedItem;
            object draggedItemContext;
            CreateCalculationAndContext(draggedItemType, out draggedItem, out draggedItemContext, pipingFailureMechanismMock, assessmentSection);

            CalculationGroup originalOwnerGroup;
            GrassCoverErosionInwardsCalculationGroupContext originalOwnerGroupContext;
            CreateCalculationGroupAndContext(out originalOwnerGroup, out originalOwnerGroupContext, pipingFailureMechanismMock, assessmentSection);
            originalOwnerGroup.Children.Add(draggedItem);

            CalculationGroup newOwnerGroup;
            GrassCoverErosionInwardsCalculationGroupContext newOwnerGroupContext;
            CreateCalculationGroupAndContext(out newOwnerGroup, out newOwnerGroupContext, pipingFailureMechanismMock, assessmentSection);

            var originalOwnerObserver = mocks.StrictMock<IObserver>();
            originalOwnerObserver.Expect(o => o.UpdateObserver());

            var newOwnerObserver = mocks.StrictMock<IObserver>();
            newOwnerObserver.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            originalOwnerGroup.Attach(originalOwnerObserver);
            newOwnerGroup.Attach(newOwnerObserver);

            // Precondition
            CollectionAssert.Contains(originalOwnerGroup.Children, draggedItem);
            CollectionAssert.DoesNotContain(newOwnerGroup.Children, draggedItem);

            // Call
            info.OnDrop(draggedItemContext, newOwnerGroupContext, originalOwnerGroupContext, 0, treeViewControlMock);

            // Assert
            CollectionAssert.DoesNotContain(originalOwnerGroup.Children, draggedItem);
            CollectionAssert.Contains(newOwnerGroup.Children, draggedItem);
            Assert.AreSame(draggedItem, newOwnerGroup.Children.Last(),
                           "Dragging node at the end of the target PipingCalculationGroup should put the dragged data at the end of 'newOwnerGroup'.");

            mocks.VerifyAll();
        }

        [Test]
        [Combinatorial]
        public void OnDrop_InsertingPipingCalculationItemContextAtDifferentLocationWithinSameGroup_ChangeItemIndexOfCalculationItem(
            [Values(CalculationType.Calculation, CalculationType.Group)] CalculationType draggedItemType,
            [Values(0, 2)] int newIndex)
        {
            // Setup
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var pipingFailureMechanismMock = mocks.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();

            const string name = "Very cool name";
            ICalculationBase draggedItem;
            object draggedItemContext;
            CreateCalculationAndContext(draggedItemType, out draggedItem, out draggedItemContext, pipingFailureMechanismMock, assessmentSection, name);

            var existingItemStub = mocks.Stub<ICalculationBase>();
            existingItemStub.Name = "";

            CalculationGroup originalOwnerGroup;
            GrassCoverErosionInwardsCalculationGroupContext originalOwnerGroupContext;
            CreateCalculationGroupAndContext(out originalOwnerGroup, out originalOwnerGroupContext, pipingFailureMechanismMock, assessmentSection);
            originalOwnerGroup.Children.Add(existingItemStub);
            originalOwnerGroup.Children.Add(draggedItem);
            originalOwnerGroup.Children.Add(existingItemStub);

            var originalOwnerObserver = mocks.StrictMock<IObserver>();
            originalOwnerObserver.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            originalOwnerGroup.Attach(originalOwnerObserver);

            // Precondition
            CollectionAssert.Contains(originalOwnerGroup.Children, draggedItem);

            // Call
            info.OnDrop(draggedItemContext, originalOwnerGroupContext, originalOwnerGroupContext, newIndex, treeViewControlMock);

            // Assert
            CollectionAssert.Contains(originalOwnerGroup.Children, draggedItem);
            Assert.AreNotSame(draggedItem, originalOwnerGroup.Children[1],
                              "Should have removed 'draggedItem' from its original location in the collection.");
            Assert.AreSame(draggedItem, originalOwnerGroup.Children[newIndex],
                           "Dragging node to specific location within owning PipingCalculationGroup should put the dragged data at that index.");
            Assert.AreEqual(name, draggedItem.Name,
                            "No renaming should occur when dragging within the same PipingCalculationGroup.");

            mocks.VerifyAll();
        }

        [Test]
        [Combinatorial]
        public void OnDrop_DraggingPipingCalculationItemContextOntoGroupWithSameNamedItem_MoveCalculationItemInstanceToNewGroupAndRename(
            [Values(CalculationType.Calculation, CalculationType.Group)] CalculationType draggedItemType)
        {
            // Setup
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var pipingFailureMechanismMock = mocks.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();

            ICalculationBase draggedItem;
            object draggedItemContext;
            CreateCalculationAndContext(draggedItemType, out draggedItem, out draggedItemContext, pipingFailureMechanismMock, assessmentSection);

            CalculationGroup originalOwnerGroup;
            GrassCoverErosionInwardsCalculationGroupContext originalOwnerGroupContext;
            CreateCalculationGroupAndContext(out originalOwnerGroup, out originalOwnerGroupContext, pipingFailureMechanismMock, assessmentSection);
            originalOwnerGroup.Children.Add(draggedItem);

            CalculationGroup newOwnerGroup;
            GrassCoverErosionInwardsCalculationGroupContext newOwnerGroupContext;
            CreateCalculationGroupAndContext(out newOwnerGroup, out newOwnerGroupContext, pipingFailureMechanismMock, assessmentSection);

            var sameNamedItem = mocks.Stub<ICalculationBase>();
            sameNamedItem.Name = draggedItem.Name;

            var originalOwnerObserver = mocks.StrictMock<IObserver>();
            originalOwnerObserver.Expect(o => o.UpdateObserver());

            var newOwnerObserver = mocks.StrictMock<IObserver>();
            newOwnerObserver.Expect(o => o.UpdateObserver());

            treeViewControlMock.Expect(tvc => tvc.TryRenameNodeForData(draggedItemContext));

            mocks.ReplayAll();

            newOwnerGroup.Children.Add(sameNamedItem);

            originalOwnerGroup.Attach(originalOwnerObserver);
            newOwnerGroup.Attach(newOwnerObserver);

            // Precondition
            CollectionAssert.Contains(originalOwnerGroup.Children, draggedItem);
            CollectionAssert.DoesNotContain(newOwnerGroup.Children, draggedItem);
            CollectionAssert.Contains(newOwnerGroup.Children.Select(c => c.Name), draggedItem.Name,
                                      "Name of the dragged item should already exist in new owner.");

            // Call
            info.OnDrop(draggedItemContext, newOwnerGroupContext, originalOwnerGroupContext, 0, treeViewControlMock);

            // Assert
            CollectionAssert.DoesNotContain(originalOwnerGroup.Children, draggedItem);
            CollectionAssert.Contains(newOwnerGroup.Children, draggedItem);
            Assert.AreSame(draggedItem, newOwnerGroup.Children.First(),
                           "Dragging to insert node at start of newOwnerGroup should place the node at the start of the list.");
            switch (draggedItemType)
            {
                case CalculationType.Calculation:
                    Assert.AreEqual("Nieuwe berekening", draggedItem.Name);
                    break;
                case CalculationType.Group:
                    Assert.AreEqual("Nieuwe map", draggedItem.Name);
                    break;
            }

            mocks.VerifyAll();
        }

        private const int contextMenuAddCalculationGroupIndex = 0;
        private const int contextMenuAddCalculationItemIndex = 1;

        /// <summary>
        /// Creates an instance of <see cref="CalculationGroup"/> and the corresponding
        /// <see cref="GrassCoverErosionInwardsCalculationGroupContext"/>.
        /// </summary>
        /// <param name="data">The created group without any children.</param>
        /// <param name="dataContext">The context object for <paramref name="data"/>, without any other data.</param>
        /// <param name="failureMechanism">The grass cover erosion inwards failure mechanism the item and context belong to.</param>
        /// <param name="assessmentSection">The assessment section the item and context belong to.</param>
        private void CreateCalculationGroupAndContext(out CalculationGroup data, out GrassCoverErosionInwardsCalculationGroupContext dataContext, GrassCoverErosionInwardsFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            data = new CalculationGroup();

            dataContext = new GrassCoverErosionInwardsCalculationGroupContext(data,
                                                            failureMechanism,
                                                            assessmentSection);
        }

        /// <summary>
        /// Creates an instance of <see cref="ICalculationBase"/> and the corresponding context.
        /// </summary>
        /// <param name="type">Defines the implementation of <see cref="ICalculationBase"/> to be constructed.</param>
        /// <param name="data">Output: The concrete create class based on <paramref name="type"/>.</param>
        /// <param name="dataContext">Output: The <see cref="GrassCoverErosionInwardsContext{T}"/> corresponding with <paramref name="data"/>.</param>
        /// <param name="failureMechanism">The grass cover erosion inwards  failure mechanism the item and context belong to.</param>
        /// <param name="assessmentSection">The assessment section the item and context belong to.</param>
        /// <param name="initialName">Optional: The name of <paramref name="data"/>.</param>
        /// <exception cref="System.NotSupportedException"></exception>
        private static void CreateCalculationAndContext(CalculationType type, out ICalculationBase data, out object dataContext, GrassCoverErosionInwardsFailureMechanism failureMechanism, IAssessmentSection assessmentSection, string initialName = null)
        {
            switch (type)
            {
                case CalculationType.Calculation:
                    var calculation = new GrassCoverErosionInwardsCalculation();
                    if (initialName != null)
                    {
                        calculation.Name = initialName;
                    }
                    data = calculation;
                    dataContext = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanism, assessmentSection);
                    break;
                case CalculationType.Group:
                    var group = new CalculationGroup();
                    if (initialName != null)
                    {
                        group.Name = initialName;
                    }
                    data = group;
                    dataContext = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                    failureMechanism,
                                                                    assessmentSection);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Type indicator for testing methods on <see cref="TreeNodeInfo"/>.
        /// </summary>
        public enum DragDropTestMethod
        {
            /// <summary>
            /// Indicates <see cref="TreeNodeInfo.CanDrop"/>.
            /// </summary>
            CanDrop,

            /// <summary>
            /// Indicates <see cref="TreeNodeInfo.CanInsert"/>.
            /// </summary>
            CanInsert
        }

        /// <summary>
        /// Type indicator for implementations of <see cref="ICalculationBase"/> to be created in a test.
        /// </summary>
        public enum CalculationType
        {
            /// <summary>
            /// Indicates <see cref="GrassCoverErosionInwardsCalculation"/>.
            /// </summary>
            Calculation,

            /// <summary>
            /// Indicates <see cref="CalculationGroup"/>.
            /// </summary>
            Group
        }
    }
}