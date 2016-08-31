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
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Forms.PresentationObjects;
using Ringtoets.StabilityStoneCover.Plugin;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.StabilityStoneCover.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class StabilityStoneCoverWaveConditionsCalculationGroupContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuAddCalculationGroupIndexRootGroup = 2;
        private const int contextMenuAddCalculationIndexRootGroup = 3;
        private const int contextMenuRemoveAllChildrenIndexRootGroup = 5;
        private const int contextMenuValidateAllIndexRootGroup = 7;
        private const int contextMenuCalculateAllIndexRootGroup = 8;
        private const int contextMenuClearOutputIndexRootGroup = 9;
        private const int contextMenuExpandAllIndexRootGroup = 11;
        private const int contextMenuCollapseAllIndexRootGroup = 12;
        private const int contextMenuPropertiesIndexRootGroup = 14;

        private const int contextMenuAddCalculationGroupIndexNestedGroup = 0;
        private const int contextMenuAddCalculationIndexNestedGroup = 1;
        private const int contextMenuValidateAllIndexNestedGroup = 3;
        private const int contextMenuCalculateAllIndexNestedGroup = 4;
        private const int contextMenuClearOutputNestedGroupIndex = 5;

        private const int customOnlyContextMenuRemoveAllChildrenIndex = 5;
        private MockRepository mocks;
        private StabilityStoneCoverPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new StabilityStoneCoverPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(StabilityStoneCoverWaveConditionsCalculationGroupContext));
        }

        [TearDown]
        public override void TearDown()
        {
            plugin.Dispose();
            mocks.VerifyAll();

            base.TearDown();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            mocks.ReplayAll();

            // Assert
            Assert.AreEqual(typeof(StabilityStoneCoverWaveConditionsCalculationGroupContext), info.TagType);

            Assert.IsNull(info.ForeColor);
            Assert.IsNull(info.CanCheck);
            Assert.IsNull(info.IsChecked);
            Assert.IsNull(info.OnNodeChecked);
        }

        [Test]
        public void Text_Always_ReturnGroupName()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var context = new StabilityStoneCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                       failureMechanism,
                                                                                       assessmentSection);

            // Call
            string text = info.Text(context);

            // Assert
            Assert.AreEqual("Hydraulische randvoorwaarden", text);
        }

        [Test]
        public void Image_Always_ReturnCalculationGroupIcon()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var context = new StabilityStoneCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                       failureMechanism,
                                                                                       assessmentSection);

            // Call
            Image icon = info.Image(context);

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GeneralFolderIcon, icon);
        }

        [Test]
        public void ChildNodeObjects_EmptyGroup_ReturnEmpty()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            var groupContext = new StabilityStoneCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                            failureMechanism,
                                                                                            assessmentSection);

            // Call
            var children = info.ChildNodeObjects(groupContext);

            // Assert
            CollectionAssert.IsEmpty(children);
        }

        [Test]
        public void ChildNodeObjects_GroupWithChildren_ReturnChildren()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var calculationItem = mocks.StrictMock<ICalculationBase>();
            mocks.ReplayAll();

            var childGroup = new CalculationGroup();

            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationItem);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(childGroup);

            var nodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                        failureMechanism,
                                                                                        assessmentSection);

            // Call
            var children = info.ChildNodeObjects(nodeData).ToArray();

            // Assert
            Assert.AreEqual(failureMechanism.WaveConditionsCalculationGroup.Children.Count, children.Length);
            Assert.AreSame(calculationItem, children[0]);
            var returnedCalculationGroupContext = (StabilityStoneCoverWaveConditionsCalculationGroupContext) children[1];
            Assert.AreSame(childGroup, returnedCalculationGroupContext.WrappedData);
            Assert.AreSame(failureMechanism, returnedCalculationGroupContext.FailureMechanism);
        }

        [Test]
        public void ContextMenuStrip_NestedCalculationGroup_ReturnContextMenuWithItems()
        {
            // Setup
            var group = new CalculationGroup();

            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);

            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var nodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(group,
                                                                                        failureMechanism,
                                                                                        assessmentSection);
            var parentNodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                              failureMechanism,
                                                                                              assessmentSection);

            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsHandler = mocks.StrictMock<IViewCommands>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();

            var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler,
                                                     importHandlerMock,
                                                     exportHandlerMock,
                                                     viewCommandsHandler,
                                                     nodeData,
                                                     treeViewControl);

            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);

            treeViewControl.Expect(tvc => tvc.CanRemoveNodeForData(nodeData)).Return(true);
            treeViewControl.Expect(tvc => tvc.CanRenameNodeForData(nodeData)).Return(true);
            treeViewControl.Expect(tvc => tvc.CanExpandOrCollapseForData(nodeData)).Repeat.Twice().Return(false);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
            {
                // Assert
                Assert.AreEqual(14, menu.Items.Count);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationGroupIndexNestedGroup,
                                                              RingtoetsCommonFormsResources.CalculationGroup_Add_CalculationGroup,
                                                              "Voeg een nieuwe berekeningsmap toe aan deze berekeningsmap.",
                                                              RingtoetsCommonFormsResources.AddFolderIcon);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationIndexNestedGroup,
                                                              RingtoetsCommonFormsResources.CalculationGroup_Add_Calculation,
                                                              "Voeg een nieuwe berekening toe aan deze berekeningsmap.",
                                                              RingtoetsCommonFormsResources.FailureMechanismIcon,
                                                              false);

                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndexNestedGroup,
                                                              RingtoetsCommonFormsResources.Validate_all,
                                                              "Er zijn geen berekeningen om te valideren.",
                                                              RingtoetsCommonFormsResources.ValidateAllIcon,
                                                              false);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndexNestedGroup,
                                                              RingtoetsCommonFormsResources.Calculate_all,
                                                              "Er zijn geen berekeningen om uit te voeren.",
                                                              RingtoetsCommonFormsResources.CalculateAllIcon,
                                                              false);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearOutputNestedGroupIndex,
                                                              "&Wis alle uitvoer...",
                                                              "Er zijn geen berekeningen met uitvoer om te wissen.",
                                                              RingtoetsCommonFormsResources.ClearIcon,
                                                              false);

                TestHelper.AssertContextMenuStripContainsItem(menu, 7,
                                                              CoreCommonGuiResources.Rename,
                                                              CoreCommonGuiResources.Rename_ToolTip,
                                                              CoreCommonGuiResources.RenameIcon);
                TestHelper.AssertContextMenuStripContainsItem(menu, 8,
                                                              CoreCommonGuiResources.Delete,
                                                              CoreCommonGuiResources.Delete_ToolTip,
                                                              CoreCommonGuiResources.DeleteIcon);

                TestHelper.AssertContextMenuStripContainsItem(menu, 10,
                                                              CoreCommonGuiResources.Expand_all,
                                                              CoreCommonGuiResources.Expand_all_ToolTip,
                                                              CoreCommonGuiResources.ExpandAllIcon,
                                                              false);
                TestHelper.AssertContextMenuStripContainsItem(menu, 11,
                                                              CoreCommonGuiResources.Collapse_all,
                                                              CoreCommonGuiResources.Collapse_all_ToolTip,
                                                              CoreCommonGuiResources.CollapseAllIcon,
                                                              false);

                TestHelper.AssertContextMenuStripContainsItem(menu, 13,
                                                              CoreCommonGuiResources.Properties,
                                                              CoreCommonGuiResources.Properties_ToolTip,
                                                              CoreCommonGuiResources.PropertiesHS,
                                                              false);

                CollectionAssert.AllItemsAreInstancesOfType(new[]
                {
                    menu.Items[2],
                    menu.Items[6],
                    menu.Items[9],
                    menu.Items[12]
                }, typeof(ToolStripSeparator));
            }
        }

        [Test]
        public void ContextMenuStrip_WithoutParentNodeDefaultBehavior_ReturnContextMenuWithoutRenameRemove()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            var nodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                        failureMechanism,
                                                                                        assessmentSection);

            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsHandler = mocks.StrictMock<IViewCommands>();
            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler,
                                                         importHandlerMock,
                                                         exportHandlerMock,
                                                         viewCommandsHandler,
                                                         nodeData,
                                                         treeViewControl);

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    Assert.AreEqual(15, menu.Items.Count);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationGroupIndexRootGroup,
                                                                  RingtoetsCommonFormsResources.CalculationGroup_Add_CalculationGroup,
                                                                  "Voeg een nieuwe berekeningsmap toe aan deze berekeningsmap.",
                                                                  RingtoetsCommonFormsResources.AddFolderIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationIndexRootGroup,
                                                                  RingtoetsCommonFormsResources.CalculationGroup_Add_Calculation,
                                                                  "Voeg een nieuwe berekening toe aan deze berekeningsmap.",
                                                                  RingtoetsCommonFormsResources.FailureMechanismIcon,
                                                                  false);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuRemoveAllChildrenIndexRootGroup,
                                                                  RingtoetsCommonFormsResources.CalculationGroup_RemoveAllChildrenFromGroup_Remove_all,
                                                                  "Er zijn geen berekeningen of mappen om te verwijderen.",
                                                                  RingtoetsCommonFormsResources.RemoveAllIcon,
                                                                  false);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndexRootGroup,
                                                                  RingtoetsCommonFormsResources.Validate_all,
                                                                  "Er zijn geen berekeningen om te valideren.",
                                                                  RingtoetsCommonFormsResources.ValidateAllIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndexRootGroup,
                                                                  RingtoetsCommonFormsResources.Calculate_all,
                                                                  "Er zijn geen berekeningen om uit te voeren.",
                                                                  RingtoetsCommonFormsResources.CalculateAllIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearOutputIndexRootGroup,
                                                                  "&Wis alle uitvoer...",
                                                                  "Er zijn geen berekeningen met uitvoer om te wissen.",
                                                                  RingtoetsCommonFormsResources.ClearIcon,
                                                                  false);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuExpandAllIndexRootGroup,
                                                                  CoreCommonGuiResources.Expand_all,
                                                                  CoreCommonGuiResources.Expand_all_ToolTip,
                                                                  CoreCommonGuiResources.ExpandAllIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCollapseAllIndexRootGroup,
                                                                  CoreCommonGuiResources.Collapse_all,
                                                                  CoreCommonGuiResources.Collapse_all_ToolTip,
                                                                  CoreCommonGuiResources.CollapseAllIcon,
                                                                  false);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuPropertiesIndexRootGroup,
                                                                  CoreCommonGuiResources.Properties,
                                                                  CoreCommonGuiResources.Properties_ToolTip,
                                                                  CoreCommonGuiResources.PropertiesHS,
                                                                  false);
                    CollectionAssert.AllItemsAreInstancesOfType(new[]
                    {
                        menu.Items[1],
                        menu.Items[4],
                        menu.Items[6],
                        menu.Items[10],
                        menu.Items[13]
                    }, typeof(ToolStripSeparator));
                }
            }
        }

        [Test]
        public void ContextMenuStrip_NestedCalculationGroupWithNoCalculations_ValidateAndCalculateAllDisabled()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSection = mocks.Stub<IAssessmentSection>();

                var group = new CalculationGroup();

                var failureMechanism = new StabilityStoneCoverFailureMechanism();
                failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);
                var nodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(group,
                                                                                            failureMechanism,
                                                                                            assessmentSection);
                var parentNodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                                  failureMechanism,
                                                                                                  assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);

                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Assert
                    ToolStripItem validateItem = contextMenu.Items[contextMenuValidateAllIndexNestedGroup];
                    ToolStripItem calculateItem = contextMenu.Items[contextMenuCalculateAllIndexNestedGroup];
                    Assert.IsFalse(validateItem.Enabled);
                    Assert.IsFalse(calculateItem.Enabled);
                    Assert.AreEqual(RingtoetsCommonFormsResources.FailureMechanism_CreateCalculateAllItem_No_calculations_to_run, calculateItem.ToolTipText);
                    Assert.AreEqual(RingtoetsCommonFormsResources.FailureMechanism_CreateValidateAllItem_No_calculations_to_validate, validateItem.ToolTipText);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_WithoutParentNodeWithNoChildren_RemoveAllChildrenDisabled()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSection = mocks.Stub<IAssessmentSection>();

                var failureMechanism = new StabilityStoneCoverFailureMechanism();
                var nodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                            failureMechanism,
                                                                                            assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    ToolStripItem removeAllItemDisabled = contextMenu.Items[customOnlyContextMenuRemoveAllChildrenIndex];
                    Assert.IsFalse(removeAllItemDisabled.Enabled);
                    Assert.AreEqual(RingtoetsCommonFormsResources.CalculationGroup_RemoveAllChildrenFromGroup_No_Calculation_or_Group_to_remove, removeAllItemDisabled.ToolTipText);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_WithoutParentNodeWithChildren_RemoveAllChildrenEnabled()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var failureMechanism = new StabilityStoneCoverFailureMechanism();
                failureMechanism.WaveConditionsCalculationGroup.Children.Add(mocks.Stub<ICalculation>());
                var nodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                            failureMechanism,
                                                                                            assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    ToolStripItem removeAllItemEnabled = contextMenu.Items[customOnlyContextMenuRemoveAllChildrenIndex];
                    Assert.IsTrue(removeAllItemEnabled.Enabled);
                    Assert.AreEqual(RingtoetsCommonFormsResources.CalculationGroup_RemoveAllChildrenFromGroup_Remove_all_Tooltip, removeAllItemEnabled.ToolTipText);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_ClickOnAddGroupItem_AddGroupToCalculationGroupAndNotifyObservers()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSection = mocks.Stub<IAssessmentSection>();

                var group = new CalculationGroup();
                var failureMechanism = new StabilityStoneCoverFailureMechanism();
                failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);

                var nodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(group,
                                                                                            failureMechanism,
                                                                                            assessmentSection);
                var parentNodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                                  failureMechanism,
                                                                                                  assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);

                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());
                mocks.ReplayAll();

                plugin.Gui = gui;

                var calculationItem = new CalculationGroup
                {
                    Name = "Nieuwe map"
                };
                group.Children.Add(calculationItem);

                nodeData.Attach(observer);

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Precondition
                    Assert.AreEqual(1, group.Children.Count);

                    // Call
                    contextMenu.Items[contextMenuAddCalculationGroupIndexNestedGroup].PerformClick();

                    // Assert
                    Assert.AreEqual(2, group.Children.Count);
                    var newlyAddedItem = group.Children.Last();
                    Assert.IsInstanceOf<CalculationGroup>(newlyAddedItem);
                    Assert.AreEqual("Nieuwe map (1)", newlyAddedItem.Name,
                                    "An item with the same name default name already exists, therefore '(1)' needs to be appended.");
                }
            }
        }

        [Test]
        public void ContextMenuStrip_ClickOnRemoveAllInGroup_RemovesAllChildren()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var calculation = mocks.Stub<ICalculation>();
                var failureMechanism = new StabilityStoneCoverFailureMechanism();
                failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation);
                var nodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                            failureMechanism,
                                                                                            assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var viewCommandsMock = mocks.StrictMock<IViewCommands>();
                viewCommandsMock.Expect(vc => vc.RemoveAllViewsForItem(calculation));

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(viewCommandsMock);

                mocks.ReplayAll();

                plugin.Gui = gui;

                DialogBoxHandler = (name, wnd) =>
                {
                    var dialog = new MessageBoxTester(wnd);
                    dialog.ClickOk();
                };

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Call
                    contextMenu.Items[contextMenuRemoveAllChildrenIndexRootGroup].PerformClick();

                    // Assert
                    Assert.IsEmpty(failureMechanism.WaveConditionsCalculationGroup.Children);
                }
            }
        }

        [Test]
        public void OnNodeRemoved_ParentIsPipingCalculationGroupContainingGroup_RemoveGroupAndNotifyObservers()
        {
            // Setup
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var group = new CalculationGroup();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            var nodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(group,
                                                                                        failureMechanism,
                                                                                        assessmentSection);

            failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);
            var parentNodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                              failureMechanism,
                                                                                              assessmentSection);
            parentNodeData.Attach(observer);

            // Precondition
            Assert.IsTrue(info.CanRemove(nodeData, parentNodeData));

            // Call
            info.OnNodeRemoved(nodeData, parentNodeData);

            // Assert
            CollectionAssert.DoesNotContain(failureMechanism.WaveConditionsCalculationGroup.Children, group);
        }
    }
}