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

using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Properties;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Plugin;
using Ringtoets.HydraRing.Calculation.TestUtil;
using Ringtoets.HydraRing.Data;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Service.TestUtil;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveConditionsCalculationGroupContextTreeNodeInfoTest : NUnitFormTest
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

        private const int contextMenuAddCalculationGroupIndexNestedGroup = 2;
        private const int contextMenuAddCalculationIndexNestedGroup = 3;
        private const int contextMenuValidateAllIndexNestedGroup = 5;
        private const int contextMenuCalculateAllIndexNestedGroup = 6;
        private const int contextMenuClearOutputNestedGroupIndex = 7;

        private const int contextMenuValidateAllIndexNestedGroupNoCalculations = 4;
        private const int contextMenuCalculateAllIndexNestedGroupNoCalculations = 5;

        private const int contextMenuRemoveAllInGroup = 4;

        private const int customOnlyContextMenuRemoveAllChildrenIndex = 4;

        private MockRepository mocks;
        private GrassCoverErosionOutwardsPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new GrassCoverErosionOutwardsPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext));
        }

        [TearDown]
        public override void TearDown()
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
            Assert.AreEqual(typeof(GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext), info.TagType);

            Assert.IsNull(info.ForeColor);
            Assert.IsNull(info.CanCheck);
            Assert.IsNull(info.IsChecked);
            Assert.IsNull(info.OnNodeChecked);
        }

        [Test]
        public void Text_Always_ReturnGroupName()
        {
            // Setup
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                             failureMechanism,
                                                                                             assessmentSection);

            // Call
            string text = info.Text(context);

            // Assert
            Assert.AreEqual("Berekeningen", text);
        }

        [Test]
        public void Image_Always_ReturnCalculationGroupIcon()
        {
            // Setup
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
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

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            var groupContext = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
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

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationItem);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(childGroup);

            var nodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                              failureMechanism,
                                                                                              assessmentSection);

            // Call
            var children = info.ChildNodeObjects(nodeData).ToArray();

            // Assert
            Assert.AreEqual(failureMechanism.WaveConditionsCalculationGroup.Children.Count, children.Length);
            Assert.AreSame(calculationItem, children[0]);
            var returnedCalculationGroupContext = (GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext) children[1];
            Assert.AreSame(childGroup, returnedCalculationGroupContext.WrappedData);
            Assert.AreSame(failureMechanism, returnedCalculationGroupContext.FailureMechanism);
        }

        [Test]
        public void ContextMenuStrip_NestedCalculationGroup_ReturnContextMenuWithItems()
        {
            // Setup
            var group = new CalculationGroup();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);

            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var nodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(group,
                                                                                              failureMechanism,
                                                                                              assessmentSection);
            var parentNodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                                    failureMechanism,
                                                                                                    assessmentSection);

            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            exportHandlerMock.Expect(ehm => ehm.CanExportFrom(nodeData)).Return(true);
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
                Assert.AreEqual(16, menu.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(menu, 0,
                                                              Resources.Export,
                                                              Resources.Export_ToolTip,
                                                              Resources.ExportIcon);

                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationGroupIndexNestedGroup,
                                                              RingtoetsCommonFormsResources.CalculationGroup_Add_CalculationGroup,
                                                              "Voeg een nieuwe berekeningsmap toe aan deze berekeningsmap.",
                                                              RingtoetsCommonFormsResources.AddFolderIcon);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationIndexNestedGroup,
                                                              RingtoetsCommonFormsResources.CalculationGroup_Add_Calculation,
                                                              "Voeg een nieuwe berekening toe aan deze berekeningsmap.",
                                                              RingtoetsCommonFormsResources.FailureMechanismIcon);
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

                TestHelper.AssertContextMenuStripContainsItem(menu, 9,
                                                              Resources.Rename,
                                                              Resources.Rename_ToolTip,
                                                              Resources.RenameIcon);
                TestHelper.AssertContextMenuStripContainsItem(menu, 10,
                                                              Resources.Delete,
                                                              Resources.Delete_ToolTip,
                                                              Resources.DeleteIcon);

                TestHelper.AssertContextMenuStripContainsItem(menu, 12,
                                                              Resources.Expand_all,
                                                              Resources.Expand_all_ToolTip,
                                                              Resources.ExpandAllIcon,
                                                              false);
                TestHelper.AssertContextMenuStripContainsItem(menu, 13,
                                                              Resources.Collapse_all,
                                                              Resources.Collapse_all_ToolTip,
                                                              Resources.CollapseAllIcon,
                                                              false);

                TestHelper.AssertContextMenuStripContainsItem(menu, 15,
                                                              Resources.Properties,
                                                              Resources.Properties_ToolTip,
                                                              Resources.PropertiesHS,
                                                              false);

                CollectionAssert.AllItemsAreInstancesOfType(new[]
                {
                    menu.Items[1],
                    menu.Items[4],
                    menu.Items[8],
                    menu.Items[11],
                    menu.Items[14]
                }, typeof(ToolStripSeparator));
            }
        }

        [Test]
        public void ContextMenuStrip_WithoutParentNodeWithoutHydraulicLocationsDefaultBehavior_ReturnContextMenuWithoutRenameRemove()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            var nodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                              failureMechanism,
                                                                                              assessmentSection);

            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            exportHandlerMock.Expect(ehm => ehm.CanExportFrom(nodeData)).Return(true);
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
                                                                  RingtoetsCommonFormsResources.FailureMechanismIcon);
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
                                                                  Resources.Expand_all,
                                                                  Resources.Expand_all_ToolTip,
                                                                  Resources.ExpandAllIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCollapseAllIndexRootGroup,
                                                                  Resources.Collapse_all,
                                                                  Resources.Collapse_all_ToolTip,
                                                                  Resources.CollapseAllIcon,
                                                                  false);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuPropertiesIndexRootGroup,
                                                                  Resources.Properties,
                                                                  Resources.Properties_ToolTip,
                                                                  Resources.PropertiesHS,
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
        public void ContextMenuStrip_WithoutParentNodeWithHydraulicLocationsDefaultBehavior_ReturnContextMenuWithoutRenameRemove()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    new HydraulicBoundaryLocation(1, "1", 1, 1)
                }
            };
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            var nodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                              failureMechanism,
                                                                                              assessmentSection);

            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            exportHandlerMock.Expect(ehm => ehm.CanExportFrom(nodeData)).Return(true);
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
                                                                  RingtoetsCommonFormsResources.FailureMechanismIcon);

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
                                                                  Resources.Expand_all,
                                                                  Resources.Expand_all_ToolTip,
                                                                  Resources.ExpandAllIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCollapseAllIndexRootGroup,
                                                                  Resources.Collapse_all,
                                                                  Resources.Collapse_all_ToolTip,
                                                                  Resources.CollapseAllIcon,
                                                                  false);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuPropertiesIndexRootGroup,
                                                                  Resources.Properties,
                                                                  Resources.Properties_ToolTip,
                                                                  Resources.PropertiesHS,
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
                var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
                var group = new CalculationGroup();
                failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var nodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(group,
                                                                                                  failureMechanism,
                                                                                                  assessmentSection);
                var parentNodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
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
                    ToolStripItem validateItem = contextMenu.Items[contextMenuValidateAllIndexNestedGroupNoCalculations];
                    ToolStripItem calculateItem = contextMenu.Items[contextMenuCalculateAllIndexNestedGroupNoCalculations];
                    Assert.IsFalse(validateItem.Enabled);
                    Assert.IsFalse(calculateItem.Enabled);
                    Assert.AreEqual(RingtoetsCommonFormsResources.FailureMechanism_CreateCalculateAllItem_No_calculations_to_run, calculateItem.ToolTipText);
                    Assert.AreEqual(RingtoetsCommonFormsResources.FailureMechanism_CreateValidateAllItem_No_calculations_to_validate, validateItem.ToolTipText);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismWithNoSections_ValidateAndCalculateAllDisabled()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
                var group = new CalculationGroup();
                group.Children.Add(new GrassCoverErosionOutwardsWaveConditionsCalculation());
                failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var nodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(group,
                                                                                                  failureMechanism,
                                                                                                  assessmentSection);
                var parentNodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
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
                    ToolStripItem validateItem = contextMenu.Items[contextMenuValidateAllIndexNestedGroupNoCalculations];
                    ToolStripItem calculateItem = contextMenu.Items[contextMenuCalculateAllIndexNestedGroupNoCalculations];
                    Assert.IsFalse(validateItem.Enabled);
                    Assert.IsFalse(calculateItem.Enabled);
                    Assert.AreEqual(RingtoetsCommonFormsResources.Plugin_AllDataAvailable_No_failure_mechanism_sections_imported, calculateItem.ToolTipText);
                    Assert.AreEqual(RingtoetsCommonFormsResources.Plugin_AllDataAvailable_No_failure_mechanism_sections_imported, validateItem.ToolTipText);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_AssessmentSectionWithoutHydraulicBoundaryDatabase_ValidateAndCalculateAllDisabled()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
                failureMechanism.AddSection(new FailureMechanismSection("", new[]
                {
                    new Point2D(0, 0)
                }));
                var group = new CalculationGroup();
                group.Children.Add(new GrassCoverErosionOutwardsWaveConditionsCalculation());
                failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var nodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(group,
                                                                                                  failureMechanism,
                                                                                                  assessmentSection);
                var parentNodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
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
                    ToolStripItem validateItem = contextMenu.Items[contextMenuValidateAllIndexNestedGroupNoCalculations];
                    ToolStripItem calculateItem = contextMenu.Items[contextMenuCalculateAllIndexNestedGroupNoCalculations];
                    Assert.IsFalse(validateItem.Enabled);
                    Assert.IsFalse(calculateItem.Enabled);
                    Assert.AreEqual(RingtoetsCommonFormsResources.Plugin_AllDataAvailable_No_hydraulic_boundary_database_imported, calculateItem.ToolTipText);
                    Assert.AreEqual(RingtoetsCommonFormsResources.Plugin_AllDataAvailable_No_hydraulic_boundary_database_imported, validateItem.ToolTipText);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_AssessmentSectionWithInvalidHydraulicBoundaryDatabasePath_ValidateAndCalculateAllDisabled()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
                failureMechanism.AddSection(new FailureMechanismSection("", new[]
                {
                    new Point2D(0, 0)
                }));
                var group = new CalculationGroup();
                group.Children.Add(new GrassCoverErosionOutwardsWaveConditionsCalculation());
                failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
                {
                    FilePath = ""
                };
                var nodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(group,
                                                                                                  failureMechanism,
                                                                                                  assessmentSection);
                var parentNodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
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
                    ToolStripItem validateItem = contextMenu.Items[contextMenuValidateAllIndexNestedGroupNoCalculations];
                    ToolStripItem calculateItem = contextMenu.Items[contextMenuCalculateAllIndexNestedGroupNoCalculations];
                    Assert.IsFalse(validateItem.Enabled);
                    Assert.IsFalse(calculateItem.Enabled);
                    var message = "Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt. Fout bij het lezen van bestand '': Bestandspad mag niet leeg of ongedefinieerd zijn.";
                    Assert.AreEqual(message, calculateItem.ToolTipText);
                    Assert.AreEqual(message, validateItem.ToolTipText);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_TwoCalculationsClickOnValidateAllInGroup_ValidationMessagesLogged()
        {
            string hrdPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = Path.Combine(hrdPath, "HRD ijsselmeer.sqlite")
            };
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(
                new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 30, 2));

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "", 1, 1)
                    {
                        DesignWaterLevel = (RoundedDouble) 12.0
                    },
                    LowerBoundaryRevetment = (RoundedDouble) 1.0,
                    UpperBoundaryRevetment = (RoundedDouble) 10.0,
                    StepSize = WaveConditionsInputStepSize.One,
                    LowerBoundaryWaterLevels = (RoundedDouble) 1.0,
                    UpperBoundaryWaterLevels = (RoundedDouble) 10.0
                }
            };

            var group = new CalculationGroup();
            group.Children.Add(calculation);
            group.Children.Add(calculation);

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("", new[]
            {
                new Point2D(0, 0)
            }));
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);
            var nodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(group,
                                                                                              failureMechanism,
                                                                                              assessmentSection);
            var parentNodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                                    failureMechanism,
                                                                                                    assessmentSection);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);

                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Call
                    Action test = () => contextMenu.Items[4].PerformClick();

                    // Assert
                    TestHelper.AssertLogMessages(test, m =>
                    {
                        var messages = m.ToArray();
                        Assert.AreEqual(4, messages.Length);
                        StringAssert.StartsWith("Validatie van 'Nieuwe berekening' gestart om: ", messages[0]);
                        StringAssert.StartsWith("Validatie van 'Nieuwe berekening' beëindigd om: ", messages[1]);
                        StringAssert.StartsWith("Validatie van 'Nieuwe berekening' gestart om: ", messages[2]);
                        StringAssert.StartsWith("Validatie van 'Nieuwe berekening' beëindigd om: ", messages[3]);
                    });
                }
            }
        }

        [Test]
        public void ContextMenuStrip_TwoCalculationsClickOnCalculateAllInGroup_MessagesLogged()
        {
            string hrdPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = Path.Combine(hrdPath, "HRD ijsselmeer.sqlite")
            };
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(
                new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 30, 2));
            assessmentSection.Stub(a => a.Id).Return("someId");

            var observerA = mocks.StrictMock<IObserver>();
            observerA.Expect(o => o.UpdateObserver());
            var observerB = mocks.StrictMock<IObserver>();
            observerB.Expect(o => o.UpdateObserver());

            var group = new CalculationGroup();
            var calculationA = GetValidCalculation();
            var calculationB = GetValidCalculation();
            calculationA.Attach(observerA);
            calculationB.Attach(observerB);
            group.Children.Add(calculationA);
            group.Children.Add(calculationB);

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("", new[]
            {
                new Point2D(0, 0)
            }));
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);
            var nodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(group,
                                                                                              failureMechanism,
                                                                                              assessmentSection);
            var parentNodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                                    failureMechanism,
                                                                                                    assessmentSection);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var mainWindow = mocks.Stub<IMainWindow>();

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);

                mocks.ReplayAll();

                plugin.Gui = gui;

                using (new HydraRingCalculationServiceConfig())
                using (new WaveConditionsCalculationServiceConfig())
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Call
                    Action test = () => contextMenu.Items[5].PerformClick();

                    // Assert
                    TestHelper.AssertLogMessages(test, m =>
                    {
                        var messages = m.ToArray();
                        Assert.AreEqual(18, messages.Length);
                        StringAssert.StartsWith("Berekening van 'Nieuwe berekening' gestart om: ", messages[0]);
                        StringAssert.StartsWith("Berekening van 'Nieuwe berekening' beëindigd om: ", messages[7]);
                        StringAssert.StartsWith("Berekening van 'Nieuwe berekening' gestart om: ", messages[8]);
                        StringAssert.StartsWith("Berekening van 'Nieuwe berekening' beëindigd om: ", messages[15]);
                        Assert.AreEqual("Uitvoeren van 'Nieuwe berekening' is gelukt.", messages[16]);
                        Assert.AreEqual("Uitvoeren van 'Nieuwe berekening' is gelukt.", messages[17]);
                    });
                }
            }
        }

        [Test]
        public void ContextMenuStrip_NoCalculations_ClearAllOutputItemDisabled()
        {
            string hrdPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = Path.Combine(hrdPath, "HRD ijsselmeer.sqlite")
            };
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(
                new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 30, 2));
            assessmentSection.Stub(a => a.Id).Return("someId");

            var group = new CalculationGroup();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("", new[]
            {
                new Point2D(0, 0)
            }));
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);
            var nodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(group,
                                                                                              failureMechanism,
                                                                                              assessmentSection);
            var parentNodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                                    failureMechanism,
                                                                                                    assessmentSection);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var mainWindow = mocks.Stub<IMainWindow>();

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);

                mocks.ReplayAll();

                plugin.Gui = gui;

                using (new HydraRingCalculationServiceConfig())
                using (new WaveConditionsCalculationServiceConfig())
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Call
                    var clearAllOutputItem = contextMenu.Items[5];

                    // Assert
                    Assert.IsFalse(clearAllOutputItem.Enabled);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_TwoCalculationsWithoutOutput_ClearAllOutputItemDisabled()
        {
            string hrdPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = Path.Combine(hrdPath, "HRD ijsselmeer.sqlite")
            };
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(
                new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 30, 2));
            assessmentSection.Stub(a => a.Id).Return("someId");

            var group = new CalculationGroup();
            var calculationA = GetValidCalculation();
            var calculationB = GetValidCalculation();
            group.Children.Add(calculationA);
            group.Children.Add(calculationB);

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("", new[]
            {
                new Point2D(0, 0)
            }));
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);
            var nodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(group,
                                                                                              failureMechanism,
                                                                                              assessmentSection);
            var parentNodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                                    failureMechanism,
                                                                                                    assessmentSection);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var mainWindow = mocks.Stub<IMainWindow>();

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);

                mocks.ReplayAll();

                plugin.Gui = gui;

                using (new HydraRingCalculationServiceConfig())
                using (new WaveConditionsCalculationServiceConfig())
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Call
                    var clearAllOutputItem = contextMenu.Items[6];

                    // Assert
                    Assert.IsFalse(clearAllOutputItem.Enabled);
                }
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ContextMenuStrip_TwoCalculationsWithOutputClickOnClearAllOutput_OutputRemovedForCalculationsAfterConfirmation(bool confirm)
        {
            string hrdPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = Path.Combine(hrdPath, "HRD ijsselmeer.sqlite")
            };
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(
                new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 30, 2));
            assessmentSection.Stub(a => a.Id).Return("someId");

            var observerA = mocks.StrictMock<IObserver>();
            var observerB = mocks.StrictMock<IObserver>();
            if (confirm)
            {
                observerA.Expect(o => o.UpdateObserver());
                observerB.Expect(o => o.UpdateObserver());
            }

            var group = new CalculationGroup();
            var calculationA = GetValidCalculation();
            calculationA.Output = new GrassCoverErosionOutwardsWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>());
            var calculationB = GetValidCalculation();
            calculationB.Output = new GrassCoverErosionOutwardsWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>());
            group.Children.Add(calculationA);
            group.Children.Add(calculationB);
            calculationA.Attach(observerA);
            calculationB.Attach(observerB);

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("", new[]
            {
                new Point2D(0, 0)
            }));
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);
            var nodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(group,
                                                                                              failureMechanism,
                                                                                              assessmentSection);
            var parentNodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                                    failureMechanism,
                                                                                                    assessmentSection);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            DialogBoxHandler = (name, wnd) =>
            {
                var dialog = new MessageBoxTester(wnd);
                if (confirm)
                {
                    dialog.ClickOk();
                }
                else
                {
                    dialog.ClickCancel();
                }
            };

            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var mainWindow = mocks.Stub<IMainWindow>();

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);

                mocks.ReplayAll();

                plugin.Gui = gui;

                using (new HydraRingCalculationServiceConfig())
                using (new WaveConditionsCalculationServiceConfig())
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Call
                    contextMenu.Items[6].PerformClick();

                    // Assert
                    if (confirm)
                    {
                        Assert.IsNull(calculationA.Output);
                        Assert.IsNull(calculationB.Output);
                    }
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

                var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
                var nodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
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
                var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
                failureMechanism.WaveConditionsCalculationGroup.Children.Add(mocks.Stub<ICalculation>());
                var nodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
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
                var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
                failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);

                var nodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(group,
                                                                                                  failureMechanism,
                                                                                                  assessmentSection);
                var parentNodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
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
                    contextMenu.Items[1].PerformClick();

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
        public void ContextMenuStrip_ClickOnRemoveAllInGroupAndConfirm_RemovesAllChildren()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var calculation = mocks.Stub<ICalculation>();
                var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
                failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation);
                var nodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                                  failureMechanism,
                                                                                                  assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var viewCommandsMock = mocks.StrictMock<IViewCommands>();
                viewCommandsMock.Expect(vc => vc.RemoveAllViewsForItem(calculation));

                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());
                failureMechanism.WaveConditionsCalculationGroup.Attach(observer);

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
                    contextMenu.Items[contextMenuRemoveAllInGroup].PerformClick();

                    // Assert
                    Assert.IsEmpty(failureMechanism.WaveConditionsCalculationGroup.Children);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_ClickOnRemoveAllInGroupAndCancel_ChildrenNotRemoved()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var calculation = mocks.Stub<ICalculation>();
                var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
                failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation);
                var nodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                                  failureMechanism,
                                                                                                  assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var viewCommandsMock = mocks.StrictMock<IViewCommands>();

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(viewCommandsMock);

                mocks.ReplayAll();

                plugin.Gui = gui;

                DialogBoxHandler = (name, wnd) =>
                {
                    var dialog = new MessageBoxTester(wnd);
                    dialog.ClickCancel();
                };

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Call
                    contextMenu.Items[contextMenuRemoveAllInGroup].PerformClick();

                    // Assert
                    Assert.AreEqual(new[]
                    {
                        calculation
                    }, failureMechanism.WaveConditionsCalculationGroup.Children);
                }
            }
        }

        [Test]
        public void GivenCalculationWithoutOutput_ThenClearOutputItemDisabled()
        {
            // Given
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                             failureMechanism,
                                                                                             assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var appFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
                var importHandler = mocks.Stub<IImportCommandHandler>();
                var exportHandler = mocks.Stub<IExportCommandHandler>();
                var viewCommands = mocks.Stub<IViewCommands>();
                var menuBuilderMock = new ContextMenuBuilder(appFeatureCommandHandler,
                                                             importHandler,
                                                             exportHandler,
                                                             viewCommands,
                                                             context,
                                                             treeViewControl);

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.ViewCommands).Return(viewCommands);
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilderMock);

                mocks.ReplayAll();

                plugin.Gui = gui;

                // When
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Then
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  contextMenuAddCalculationIndexRootGroup,
                                                                  "Berekening &toevoegen",
                                                                  "Voeg een nieuwe berekening toe aan deze berekeningsmap.",
                                                                  RingtoetsCommonFormsResources.FailureMechanismIcon);
                    // Check expectancies in TearDown()
                }
            }
        }

        [Test]
        public void GivenCalculationWithOutput_WhenClearingOutput_ThenClearOutput()
        {
            // Given
            var observer = mocks.Stub<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Attach(observer);
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                             failureMechanism,
                                                                                             assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var appFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
                var importHandler = mocks.Stub<IImportCommandHandler>();
                var exportHandler = mocks.Stub<IExportCommandHandler>();
                var viewCommands = mocks.Stub<IViewCommands>();
                var menuBuilderMock = new ContextMenuBuilder(appFeatureCommandHandler,
                                                             importHandler,
                                                             exportHandler,
                                                             viewCommands,
                                                             context,
                                                             treeViewControl);

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.ViewCommands).Return(viewCommands);
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilderMock);

                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Precondition
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  contextMenuAddCalculationIndexRootGroup,
                                                                  "Berekening &toevoegen",
                                                                  "Voeg een nieuwe berekening toe aan deze berekeningsmap.",
                                                                  RingtoetsCommonFormsResources.FailureMechanismIcon);

                    // When
                    ToolStripItem validateMenuItem = contextMenu.Items[contextMenuAddCalculationIndexRootGroup];
                    validateMenuItem.PerformClick();

                    // Then
                    Assert.AreEqual(1, failureMechanism.WaveConditionsCalculationGroup.Children.Count);
                    Assert.IsInstanceOf<GrassCoverErosionOutwardsWaveConditionsCalculation>(failureMechanism.WaveConditionsCalculationGroup.Children[0]);
                    // Check expectancies in TearDown()
                }
            }
        }

        [Test]
        public void OnNodeRemoved_ParentIsWaveConditionsCalculationGroupContainingGroup_RemoveGroupAndNotifyObservers()
        {
            // Setup
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var group = new CalculationGroup();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            var nodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(group,
                                                                                              failureMechanism,
                                                                                              assessmentSection);

            failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);
            var parentNodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
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

        private static GrassCoverErosionOutwardsWaveConditionsCalculation GetValidCalculation()
        {
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1300001, "", 0.0, 0.0),
                    ForeshoreProfile = CreateForeshoreProfile(),
                    UseForeshore = true,
                    UseBreakWater = true,
                    StepSize = WaveConditionsInputStepSize.Half,
                    LowerBoundaryRevetment = (RoundedDouble) 4,
                    UpperBoundaryRevetment = (RoundedDouble) 10,
                    UpperBoundaryWaterLevels = (RoundedDouble) 8,
                    LowerBoundaryWaterLevels = (RoundedDouble) 7.1
                }
            };
            calculation.InputParameters.HydraulicBoundaryLocation.DesignWaterLevel = (RoundedDouble) 9.3;
            return calculation;
        }

        private static ForeshoreProfile CreateForeshoreProfile()
        {
            return new ForeshoreProfile(new Point2D(0, 0),
                                        new[]
                                        {
                                            new Point2D(3.3, 4.4),
                                            new Point2D(5.5, 6.6)
                                        },
                                        new BreakWater(BreakWaterType.Dam, 10.0),
                                        new ForeshoreProfile.ConstructionProperties());
        }
    }
}