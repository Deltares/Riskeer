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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Plugin;
using Ringtoets.Piping.Primitives;
using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.Piping.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class PipingCalculationGroupContextTreeNodeInfoTest : NUnitFormTest
    {
        private MockRepository mocks;
        private PipingGuiPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new PipingGuiPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(PipingCalculationGroupContext));
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(PipingCalculationGroupContext), info.TagType);
            Assert.IsNull(info.ForeColor);
            Assert.IsNull(info.CanCheck);
            Assert.IsNull(info.IsChecked);
            Assert.IsNull(info.OnNodeChecked);
        }

        [Test]
        public void ChildNodeObjects_EmptyGroup_ReturnEmpty()
        {
            // Setup
            var group = new CalculationGroup();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var groupContext = new PipingCalculationGroupContext(group,
                                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                 Enumerable.Empty<StochasticSoilModel>(),
                                                                 pipingFailureMechanismMock,
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
            var calculationItem = mocks.StrictMock<ICalculationBase>();

            var childCalculation = new PipingCalculationScenario(new GeneralPipingInput(), new NormProbabilityPipingInput());

            var childGroup = new CalculationGroup();

            var group = new CalculationGroup();
            group.Children.Add(calculationItem);
            group.Children.Add(childCalculation);
            group.Children.Add(childGroup);

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            mocks.ReplayAll();

            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<StochasticSoilModel>(),
                                                             pipingFailureMechanismMock,
                                                             assessmentSectionMock);

            // Call
            var children = info.ChildNodeObjects(nodeData).ToArray();

            // Assert
            Assert.AreEqual(group.Children.Count, children.Length);
            Assert.AreSame(calculationItem, children[0]);
            var returnedCalculationContext = (PipingCalculationScenarioContext) children[1];
            Assert.AreSame(childCalculation, returnedCalculationContext.WrappedData);
            Assert.AreSame(pipingFailureMechanismMock, returnedCalculationContext.FailureMechanism);
            var returnedCalculationGroupContext = (PipingCalculationGroupContext) children[2];
            Assert.AreSame(childGroup, returnedCalculationGroupContext.WrappedData);
            Assert.AreSame(pipingFailureMechanismMock, returnedCalculationGroupContext.FailureMechanism);
            Assert.AreSame(assessmentSectionMock, returnedCalculationGroupContext.AssessmentSection);
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_ChildOfGroupValidDataWithCalculationOutput_ReturnContextMenuWithAllItems()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();

            var parentGroup = new CalculationGroup();
            var group = new CalculationGroup();

            parentGroup.Children.Add(group);
            group.Children.Add(new PipingCalculationScenario(new GeneralPipingInput(), new NormProbabilityPipingInput())
            {
                Output = new TestPipingOutput()
            });

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var parentData = new PipingCalculationGroupContext(parentGroup,
                                                               Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                               Enumerable.Empty<StochasticSoilModel>(),
                                                               pipingFailureMechanismMock,
                                                               assessmentSectionMock);

            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<StochasticSoilModel>(),
                                                             pipingFailureMechanismMock,
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
            Assert.AreEqual(17, menu.Items.Count);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationGroupIndex,
                                                          RingtoetsFormsResources.CalculationGroup_Add_CalculationGroup,
                                                          "Voeg een nieuwe berekeningsmap toe aan deze berekeningsmap.",
                                                          RingtoetsFormsResources.AddFolderIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationIndex,
                                                          RingtoetsFormsResources.CalculationGroup_Add_Calculation,
                                                          "Voeg een nieuwe berekening toe aan deze berekeningsmap.",
                                                          PipingFormsResources.PipingIcon);

            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndex,
                                                          RingtoetsFormsResources.Validate_all,
                                                          "Valideer alle berekeningen binnen deze berekeningsmap.",
                                                          RingtoetsFormsResources.ValidateAllIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndex,
                                                          RingtoetsFormsResources.Calculate_all,
                                                          "Valideer en voer alle berekeningen binnen deze berekeningsmap uit.",
                                                          RingtoetsFormsResources.CalculateAllIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearOutputIndex,
                                                          "&Wis alle uitvoer...",
                                                          "Wis de uitvoer van alle berekeningen binnen deze berekeningsmap.",
                                                          RingtoetsFormsResources.ClearIcon);

            TestHelper.AssertContextMenuStripContainsItem(menu, 7,
                                                          CoreCommonGuiResources.Rename,
                                                          CoreCommonGuiResources.Rename_ToolTip,
                                                          CoreCommonGuiResources.RenameIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, 8,
                                                          CoreCommonGuiResources.Delete,
                                                          CoreCommonGuiResources.Delete_ToolTip,
                                                          CoreCommonGuiResources.DeleteIcon);

            TestHelper.AssertContextMenuStripContainsItem(menu, 10,
                                                          CoreCommonGuiResources.Import,
                                                          CoreCommonGuiResources.Import_ToolTip,
                                                          CoreCommonGuiResources.ImportIcon,
                                                          false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 11,
                                                          CoreCommonGuiResources.Export,
                                                          CoreCommonGuiResources.Export_ToolTip,
                                                          CoreCommonGuiResources.ExportIcon,
                                                          false);

            TestHelper.AssertContextMenuStripContainsItem(menu, 13,
                                                          CoreCommonGuiResources.Expand_all,
                                                          CoreCommonGuiResources.Expand_all_ToolTip,
                                                          CoreCommonGuiResources.ExpandAllIcon,
                                                          false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 14,
                                                          CoreCommonGuiResources.Collapse_all,
                                                          CoreCommonGuiResources.Collapse_all_ToolTip,
                                                          CoreCommonGuiResources.CollapseAllIcon,
                                                          false);

            TestHelper.AssertContextMenuStripContainsItem(menu, 16,
                                                          CoreCommonGuiResources.Properties,
                                                          CoreCommonGuiResources.Properties_ToolTip,
                                                          CoreCommonGuiResources.PropertiesHS,
                                                          false);
            CollectionAssert.AllItemsAreInstancesOfType(new[]
            {
                menu.Items[2],
                menu.Items[6],
                menu.Items[9],
                menu.Items[12],
                menu.Items[15]
            }, typeof(ToolStripSeparator));

            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_NestedCalculationGroupWithCalculationOutput_ReturnContextWithItems()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var group = new CalculationGroup();
            var parentGroup = new CalculationGroup();

            group.Children.Add(new PipingCalculationScenario(new GeneralPipingInput(), new NormProbabilityPipingInput())
            {
                Output = new TestPipingOutput()
            });

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<StochasticSoilModel>(),
                                                             pipingFailureMechanismMock,
                                                             assessmentSectionMock);
            var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                                   Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                   Enumerable.Empty<StochasticSoilModel>(),
                                                                   pipingFailureMechanismMock,
                                                                   assessmentSectionMock);

            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var exportImportHandler = mocks.Stub<IExportImportCommandHandler>();
            var viewCommandsHandler = mocks.StrictMock<IViewCommands>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();

            var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler, exportImportHandler, viewCommandsHandler, nodeData, treeViewControl);
            gui.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);

            treeViewControl.Expect(tvc => tvc.CanRemoveNodeForData(nodeData)).Return(true);
            treeViewControl.Expect(tvc => tvc.CanRenameNodeForData(nodeData)).Return(true);
            treeViewControl.Expect(tvc => tvc.CanExpandOrCollapseForData(nodeData)).Repeat.Twice().Return(false);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            ContextMenuStrip menu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl);

            // Assert
            Assert.AreEqual(17, menu.Items.Count);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationGroupIndex,
                                                          RingtoetsFormsResources.CalculationGroup_Add_CalculationGroup,
                                                          "Voeg een nieuwe berekeningsmap toe aan deze berekeningsmap.",
                                                          RingtoetsFormsResources.AddFolderIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationIndex,
                                                          RingtoetsFormsResources.CalculationGroup_Add_Calculation,
                                                          "Voeg een nieuwe berekening toe aan deze berekeningsmap.",
                                                          PipingFormsResources.PipingIcon);

            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndex,
                                                          RingtoetsFormsResources.Validate_all,
                                                          "Valideer alle berekeningen binnen deze berekeningsmap.",
                                                          RingtoetsFormsResources.ValidateAllIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndex,
                                                          RingtoetsFormsResources.Calculate_all,
                                                          "Valideer en voer alle berekeningen binnen deze berekeningsmap uit.",
                                                          RingtoetsFormsResources.CalculateAllIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearOutputIndex,
                                                          "&Wis alle uitvoer...",
                                                          "Wis de uitvoer van alle berekeningen binnen deze berekeningsmap.",
                                                          RingtoetsFormsResources.ClearIcon);

            TestHelper.AssertContextMenuStripContainsItem(menu, 7,
                                                          CoreCommonGuiResources.Rename,
                                                          CoreCommonGuiResources.Rename_ToolTip,
                                                          CoreCommonGuiResources.RenameIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, 8,
                                                          CoreCommonGuiResources.Delete,
                                                          CoreCommonGuiResources.Delete_ToolTip,
                                                          CoreCommonGuiResources.DeleteIcon);

            TestHelper.AssertContextMenuStripContainsItem(menu, 10,
                                                          CoreCommonGuiResources.Import,
                                                          CoreCommonGuiResources.Import_ToolTip,
                                                          CoreCommonGuiResources.ImportIcon,
                                                          false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 11,
                                                          CoreCommonGuiResources.Export,
                                                          CoreCommonGuiResources.Export_ToolTip,
                                                          CoreCommonGuiResources.ExportIcon,
                                                          false);

            TestHelper.AssertContextMenuStripContainsItem(menu, 13,
                                                          CoreCommonGuiResources.Expand_all,
                                                          CoreCommonGuiResources.Expand_all_ToolTip,
                                                          CoreCommonGuiResources.ExpandAllIcon,
                                                          false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 14,
                                                          CoreCommonGuiResources.Collapse_all,
                                                          CoreCommonGuiResources.Collapse_all_ToolTip,
                                                          CoreCommonGuiResources.CollapseAllIcon,
                                                          false);

            TestHelper.AssertContextMenuStripContainsItem(menu, 16,
                                                          CoreCommonGuiResources.Properties,
                                                          CoreCommonGuiResources.Properties_ToolTip,
                                                          CoreCommonGuiResources.PropertiesHS,
                                                          false);

            CollectionAssert.AllItemsAreInstancesOfType(new[]
            {
                menu.Items[2],
                menu.Items[6],
                menu.Items[9],
                menu.Items[12],
                menu.Items[15]
            }, typeof(ToolStripSeparator));

            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_NoParent_ReturnContextMenuWithoutRenameRemove()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var group = new CalculationGroup();

            group.Children.Add(new PipingCalculationScenario(new GeneralPipingInput(), new NormProbabilityPipingInput())
            {
                Output = new TestPipingOutput()
            });

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<StochasticSoilModel>(),
                                                             pipingFailureMechanismMock,
                                                             assessmentSectionMock);

            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var exportImportHandler = mocks.Stub<IExportImportCommandHandler>();
            var viewCommandsHandler = mocks.StrictMock<IViewCommands>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();

            var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler, exportImportHandler, viewCommandsHandler, nodeData, treeViewControl);
            gui.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
            viewCommandsHandler.Expect(vc => vc.CanOpenViewFor(nodeData)).Return(true);
            treeViewControl.Expect(tvc => tvc.CanExpandOrCollapseForData(nodeData)).Repeat.Twice().Return(false);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl);

            // Assert
            var mainCalculationGroupContextMenuItemOffset = 4;
            Assert.AreEqual(18, menu.Items.Count);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationGroupIndex + mainCalculationGroupContextMenuItemOffset,
                                                          RingtoetsFormsResources.CalculationGroup_Add_CalculationGroup,
                                                          "Voeg een nieuwe berekeningsmap toe aan deze berekeningsmap.",
                                                          RingtoetsFormsResources.AddFolderIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationIndex + mainCalculationGroupContextMenuItemOffset,
                                                          RingtoetsFormsResources.CalculationGroup_Add_Calculation,
                                                          "Voeg een nieuwe berekening toe aan deze berekeningsmap.",
                                                          PipingFormsResources.PipingIcon);

            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndex + mainCalculationGroupContextMenuItemOffset,
                                                          RingtoetsFormsResources.Validate_all,
                                                          "Valideer alle berekeningen binnen deze berekeningsmap.",
                                                          RingtoetsFormsResources.ValidateAllIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndex + mainCalculationGroupContextMenuItemOffset,
                                                          RingtoetsFormsResources.Calculate_all,
                                                          "Valideer en voer alle berekeningen binnen deze berekeningsmap uit.",
                                                          RingtoetsFormsResources.CalculateAllIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearOutputIndex + mainCalculationGroupContextMenuItemOffset,
                                                          "&Wis alle uitvoer...",
                                                          "Wis de uitvoer van alle berekeningen binnen deze berekeningsmap.",
                                                          RingtoetsFormsResources.ClearIcon);

            TestHelper.AssertContextMenuStripContainsItem(menu, 11,
                                                          CoreCommonGuiResources.Import,
                                                          CoreCommonGuiResources.Import_ToolTip,
                                                          CoreCommonGuiResources.ImportIcon,
                                                          false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 12,
                                                          CoreCommonGuiResources.Export,
                                                          CoreCommonGuiResources.Export_ToolTip,
                                                          CoreCommonGuiResources.ExportIcon,
                                                          false);

            TestHelper.AssertContextMenuStripContainsItem(menu, 14,
                                                          CoreCommonGuiResources.Expand_all,
                                                          CoreCommonGuiResources.Expand_all_ToolTip,
                                                          CoreCommonGuiResources.ExpandAllIcon,
                                                          false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 15,
                                                          CoreCommonGuiResources.Collapse_all,
                                                          CoreCommonGuiResources.Collapse_all_ToolTip,
                                                          CoreCommonGuiResources.CollapseAllIcon,
                                                          false);

            TestHelper.AssertContextMenuStripContainsItem(menu, 17,
                                                          CoreCommonGuiResources.Properties,
                                                          CoreCommonGuiResources.Properties_ToolTip,
                                                          CoreCommonGuiResources.PropertiesHS,
                                                          false);
            CollectionAssert.AllItemsAreInstancesOfType(new[]
            {
                menu.Items[1],
                menu.Items[3],
                menu.Items[6],
                menu.Items[10],
                menu.Items[13],
                menu.Items[16]
            }, typeof(ToolStripSeparator));

            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_NoParentAndWithoutAvailableSurfaceLines_GenerateCalculationsDisabled()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var group = new CalculationGroup();

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             new[]
                                                             {
                                                                 new TestStochasticSoilModel()
                                                             },
                                                             pipingFailureMechanismMock,
                                                             assessmentSectionMock);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            gui.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl);

            // Assert
            TestHelper.AssertContextMenuStripContainsItem(menu, 1,
                                                          PipingFormsResources.PipingCalculationGroup_Generate_PipingCalculations,
                                                          PipingFormsResources.PipingCalculationGroup_Generate_PipingCalculations_NoSurfaceLinesOrSoilModels_ToolTip,
                                                          PipingFormsResources.GeneratePipingCalculationsIcon,
                                                          false);
        }

        [Test]
        public void ContextMenuStrip_NoParentAndWithoutAvailableSoilModels_GenerateCalculationsDisabled()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var group = new CalculationGroup();

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var nodeData = new PipingCalculationGroupContext(group,
                                                             new[]
                                                             {
                                                                 new RingtoetsPipingSurfaceLine()
                                                             },
                                                             Enumerable.Empty<StochasticSoilModel>(),
                                                             pipingFailureMechanismMock,
                                                             assessmentSectionMock);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            gui.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl);

            // Assert
            TestHelper.AssertContextMenuStripContainsItem(menu, 1,
                                                          PipingFormsResources.PipingCalculationGroup_Generate_PipingCalculations,
                                                          PipingFormsResources.PipingCalculationGroup_Generate_PipingCalculations_NoSurfaceLinesOrSoilModels_ToolTip,
                                                          PipingFormsResources.GeneratePipingCalculationsIcon,
                                                          false);
        }

        [Test]
        public void ContextMenuStrip_NoParentAndWithAvailableSurfaceLinesAndSoilModels_GenerateCalculationsEnabled()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var group = new CalculationGroup();

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var nodeData = new PipingCalculationGroupContext(group,
                                                             new[]
                                                             {
                                                                 new RingtoetsPipingSurfaceLine()
                                                             },
                                                             new[]
                                                             {
                                                                 new TestStochasticSoilModel()
                                                             },
                                                             pipingFailureMechanismMock,
                                                             assessmentSectionMock);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            gui.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl);

            // Assert
            TestHelper.AssertContextMenuStripContainsItem(menu, 1,
                                                          PipingFormsResources.PipingCalculationGroup_Generate_PipingCalculations,
                                                          PipingFormsResources.PipingCalculationGroup_Generate_PipingCalculations_ToolTip,
                                                          PipingFormsResources.GeneratePipingCalculationsIcon);
        }

        [Test]
        public void ContextMenuStrip_NestedCalculationGroupWithNoCalculations_ValidateAndCalculateAllDisabled()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var group = new CalculationGroup();
            var parentGroup = new CalculationGroup();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<StochasticSoilModel>(),
                                                             pipingFailureMechanismMock,
                                                             assessmentSectionMock);
            var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                                   Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                   Enumerable.Empty<StochasticSoilModel>(),
                                                                   pipingFailureMechanismMock,
                                                                   assessmentSectionMock);

            gui.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl);

            // Assert
            ToolStripItem validateItem = contextMenu.Items[contextMenuValidateAllIndex];
            ToolStripItem calculateItem = contextMenu.Items[contextMenuCalculateAllIndex];
            Assert.IsFalse(validateItem.Enabled);
            Assert.IsFalse(calculateItem.Enabled);
            Assert.AreEqual(PipingFormsResources.PipingFailureMechanism_CreateCalculateAllItem_No_calculations_to_run, calculateItem.ToolTipText);
            Assert.AreEqual(PipingFormsResources.PipingFailureMechanism_CreateValidateAllItem_No_calculations_to_validate, validateItem.ToolTipText);

            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void ContextMenuStrip_ClickOnAddGroupItem_AddGroupToCalculationGroupAndNotifyObservers()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            var group = new CalculationGroup();
            var parentGroup = new CalculationGroup();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<StochasticSoilModel>(),
                                                             pipingFailureMechanismMock,
                                                             assessmentSectionMock);
            var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                       Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                       Enumerable.Empty<StochasticSoilModel>(),
                                                       pipingFailureMechanismMock,
                                                       assessmentSectionMock);

            var calculationItem = mocks.Stub<ICalculationBase>();
            calculationItem.Stub(ci => ci.Name).Return("Nieuwe map");

            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var treeViewControl = mocks.StrictMock<TreeViewControl>();

            gui.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);

            mocks.ReplayAll();

            plugin.Gui = gui;

            group.Children.Add(calculationItem);

            nodeData.Attach(observer);

            ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl);

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
            var parentGroup = new CalculationGroup();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<StochasticSoilModel>(),
                                                             pipingFailureMechanismMock,
                                                             assessmentSectionMock);
            var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                                   Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                   Enumerable.Empty<StochasticSoilModel>(),
                                                                   pipingFailureMechanismMock,
                                                                   assessmentSectionMock);

            var calculationItem = mocks.Stub<ICalculationBase>();
            calculationItem.Stub(ci => ci.Name).Return("Nieuwe berekening");

            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var treeViewControl = mocks.StrictMock<TreeViewControl>();

            gui.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);

            mocks.ReplayAll();

            plugin.Gui = gui;

            group.Children.Add(calculationItem);

            nodeData.Attach(observer);

            var contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl);

            // Precondition
            Assert.AreEqual(1, group.Children.Count);

            // Call
            contextMenu.Items[contextMenuAddCalculationIndex].PerformClick();

            // Assert
            Assert.AreEqual(2, group.Children.Count);
            var newlyAddedItem = group.Children.Last();
            Assert.IsInstanceOf<PipingCalculation>(newlyAddedItem);
            Assert.AreEqual("Nieuwe berekening (1)", newlyAddedItem.Name,
                            "An item with the same name default name already exists, therefore '(1)' needs to be appended.");

            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_ClickOnValidateAllItem_ValidateAllChildCalculations()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var validCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            validCalculation.Name = "A";
            var invalidCalculation = PipingCalculationFactory.CreateCalculationWithInvalidData();
            invalidCalculation.Name = "B";

            var childGroup = new CalculationGroup();
            childGroup.Children.Add(validCalculation);

            var emptyChildGroup = new CalculationGroup();
            var group = new CalculationGroup();
            var parentGroup = new CalculationGroup();

            group.Children.Add(childGroup);
            group.Children.Add(emptyChildGroup);
            group.Children.Add(invalidCalculation);

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<StochasticSoilModel>(),
                                                             pipingFailureMechanismMock,
                                                             assessmentSectionMock);
            var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                                   Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                   Enumerable.Empty<StochasticSoilModel>(),
                                                                   pipingFailureMechanismMock,
                                                                   assessmentSectionMock);

            gui.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);

            mocks.ReplayAll();

            plugin.Gui = gui;

            var contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl);

            // Call
            Action call = () => contextMenu.Items[contextMenuValidateAllIndex].PerformClick();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(6, msgs.Length);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", validCalculation.Name), msgs[0]);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", validCalculation.Name), msgs[1]);

                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", invalidCalculation.Name), msgs[2]);
                // Some validation error from validation service
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", invalidCalculation.Name), msgs[5]);
            });
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_ClickOnCalculateAllItem_ScheduleAllChildCalculations()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var mainWindow = mocks.Stub<IMainWindow>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var validCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            validCalculation.Name = "A";
            var invalidCalculation = PipingCalculationFactory.CreateCalculationWithInvalidData();
            invalidCalculation.Name = "B";

            var childGroup = new CalculationGroup();
            childGroup.Children.Add(validCalculation);

            var emptyChildGroup = new CalculationGroup();

            var group = new CalculationGroup();
            var parentGroup = new CalculationGroup();

            group.Children.Add(childGroup);
            group.Children.Add(emptyChildGroup);
            group.Children.Add(invalidCalculation);

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<StochasticSoilModel>(),
                                                             pipingFailureMechanismMock,
                                                             assessmentSectionMock);
            var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                                   Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                   Enumerable.Empty<StochasticSoilModel>(),
                                                                   pipingFailureMechanismMock,
                                                                   assessmentSectionMock);

            gui.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
            gui.Expect(g => g.MainWindow).Return(mainWindow);

            mocks.ReplayAll();

            plugin.Gui = gui;

            var contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl);

            DialogBoxHandler = (name, wnd) =>
            {
                // Don't care about dialogs in this test.
            };

            // Call
            contextMenu.Items[contextMenuCalculateAllIndex].PerformClick();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void ContextMenuStrip_ClickOnClearOutputItem_ClearOutputAllChildCalculationsAndNotifyCalculationObservers(bool confirm)
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var calculation1Observer = mocks.StrictMock<IObserver>();
            var calculation2Observer = mocks.StrictMock<IObserver>();

            if (confirm)
            {
                calculation1Observer.Expect(o => o.UpdateObserver());
                calculation2Observer.Expect(o => o.UpdateObserver());
            }

            var calculation1 = PipingCalculationFactory.CreateCalculationWithValidInput();
            calculation1.Name = "A";
            calculation1.Output = new TestPipingOutput();
            calculation1.Attach(calculation1Observer);
            var calculation2 = PipingCalculationFactory.CreateCalculationWithValidInput();
            calculation2.Name = "B";
            calculation2.Output = new TestPipingOutput();
            calculation1.Attach(calculation2Observer);

            var childGroup = new CalculationGroup();
            childGroup.Children.Add(calculation1);

            var emptyChildGroup = new CalculationGroup();
            var group = new CalculationGroup();
            var parentGroup = new CalculationGroup();

            group.Children.Add(childGroup);
            group.Children.Add(emptyChildGroup);
            group.Children.Add(calculation2);

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<StochasticSoilModel>(),
                                                             pipingFailureMechanismMock,
                                                             assessmentSectionMock);
            var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                                   Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                   Enumerable.Empty<StochasticSoilModel>(),
                                                                   pipingFailureMechanismMock,
                                                                   assessmentSectionMock);

            gui.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);

            mocks.ReplayAll();

            plugin.Gui = gui;

            string messageBoxTitle = null, messageBoxText = null;
            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);

                messageBoxText = messageBox.Text;
                messageBoxTitle = messageBox.Title;

                if (confirm)
                {
                    messageBox.ClickOk();
                }
                else
                {
                    messageBox.ClickCancel();
                }
            };
            var contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl);

            // Call
            contextMenu.Items[contextMenuClearOutputIndex].PerformClick();

            // Assert
            Assert.AreNotEqual(confirm, calculation1.HasOutput);
            Assert.AreNotEqual(confirm, calculation2.HasOutput);

            Assert.AreEqual("Bevestigen", messageBoxTitle);
            Assert.AreEqual("Weet u zeker dat u alle uitvoer wilt wissen?", messageBoxText);
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_ClickOnGenerateCalculationsItemWithSurfaceLinesAndSoilModels_ShowSurfaceLineSelectionView()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var group = new CalculationGroup();

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var mainWindow = mocks.Stub<IMainWindow>();

            var surfaceLines = new[]
            {
                new RingtoetsPipingSurfaceLine
                {
                    Name = "surfaceLine1"
                },
                new RingtoetsPipingSurfaceLine
                {
                    Name = "surfaceLine2"
                }
            };
            var nodeData = new PipingCalculationGroupContext(group,
                                                             surfaceLines,
                                                             new[]
                                                             {
                                                                 new TestStochasticSoilModel()
                                                             },
                                                             pipingFailureMechanismMock,
                                                             assessmentSectionMock);

            gui.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
            gui.Expect(g => g.MainWindow).Return(mainWindow);

            mocks.ReplayAll();

            plugin.Gui = gui;

            PipingSurfaceLineSelectionDialog selectionDialog = null;
            DataGridView grid = null;
            DialogBoxHandler = (name, wnd) =>
            {
                selectionDialog = new FormTester(name).TheObject as PipingSurfaceLineSelectionDialog;
                grid = new ControlTester("SurfaceLineDataGrid", selectionDialog).TheObject as DataGridView;

                new ButtonTester("CustomCancelButton", selectionDialog).Click();
            };

            var contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl);

            // Call
            contextMenu.Items[contextMenuAddGenerateCalculationsIndex].PerformClick();

            // Assert
            Assert.NotNull(selectionDialog);
            Assert.NotNull(grid);
            Assert.AreEqual(2, grid.RowCount);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenPipingCalculationsViewGenerateScenariosButtonClicked_WhenSurfaceLineSelectedAndDialogClosed_ThenUpdateSectionResultScenarios()
        {
            // Given
            var gui = mocks.StrictMock<IGui>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var group = new CalculationGroup();

            var pipingFailureMechanism = new PipingFailureMechanism();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var mainWindow = mocks.Stub<IMainWindow>();

            var surfaceLine1 = new RingtoetsPipingSurfaceLine
            {
                Name = "Surface line 1",
                ReferenceLineIntersectionWorldPoint = new Point2D(0.0, 0.0)
            };

            surfaceLine1.SetGeometry(new[]
            {
                new Point3D(0.0, 5.0, 0.0),
                new Point3D(0.0, 0.0, 1.0),
                new Point3D(0.0, -5.0, 0.0)
            });

            var surfaceLine2 = new RingtoetsPipingSurfaceLine
            {
                Name = "Surface line 2",
                ReferenceLineIntersectionWorldPoint = new Point2D(5.0, 0.0)
            };

            surfaceLine2.SetGeometry(new[]
            {
                new Point3D(5.0, 5.0, 0.0),
                new Point3D(5.0, 0.0, 1.0),
                new Point3D(5.0, -5.0, 0.0)
            });

            var surfaceLines = new[]
            {
                surfaceLine1,
                surfaceLine2
            };
            pipingFailureMechanism.AddSection(new FailureMechanismSection("Section 1", new List<Point2D>
            {
                new Point2D(0.0, 0.0),
                new Point2D(5.0, 0.0)
            }));

            pipingFailureMechanism.AddSection(new FailureMechanismSection("Section 2", new List<Point2D>
            {
                new Point2D(5.0, 0.0),
                new Point2D(10.0, 0.0)
            }));

            var nodeData = new PipingCalculationGroupContext(group,
                                                             surfaceLines,
                                                             new[]
                                                             {
                                                                 new TestStochasticSoilModel
                                                                 {
                                                                     Geometry =
                                                                     {
                                                                         new Point2D(0.0, 0.0), new Point2D(5.0, 0.0)
                                                                     },
                                                                 }
                                                             },
                                                             pipingFailureMechanism,
                                                             assessmentSectionMock);

            gui.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
            gui.Expect(g => g.MainWindow).Return(mainWindow);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Precondition
            foreach (var failureMechanismSectionResult in pipingFailureMechanism.SectionResults)
            {
                CollectionAssert.IsEmpty(failureMechanismSectionResult.CalculationScenarios);
            }

            DialogBoxHandler = (name, wnd) =>
            {
                var selectionDialog = new FormTester(name).TheObject as PipingSurfaceLineSelectionDialog;
                var grid = new ControlTester("SurfaceLineDataGrid", selectionDialog).TheObject as DataGridView;

                grid.Rows[0].Cells[0].Value = true;

                new ButtonTester("OkButton", selectionDialog).Click();
            };

            var contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl);

            // When
            contextMenu.Items[contextMenuAddGenerateCalculationsIndex].PerformClick();

            // Then
            var failureMechanismSectionResult1 = pipingFailureMechanism.SectionResults.First();
            var failureMechanismSectionResult2 = pipingFailureMechanism.SectionResults.ElementAt(1);

            Assert.AreEqual(2, failureMechanismSectionResult1.CalculationScenarios.Count);

            foreach (var calculationScenario in failureMechanismSectionResult1.CalculationScenarios)
            {
                Assert.IsInstanceOf<ICalculationScenario>(calculationScenario);
            }

            CollectionAssert.IsEmpty(failureMechanismSectionResult2.CalculationScenarios);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenPipingCalculationsViewGenerateScenariosButtonClicked_WhenCancelButtonClickedAndDialogClosed_ThenSectionResultScenariosNotUpdated()
        {
            // Given
            var gui = mocks.StrictMock<IGui>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var group = new CalculationGroup();

            var pipingFailureMechanism = new PipingFailureMechanism();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var mainWindow = mocks.Stub<IMainWindow>();

            var surfaceLine1 = new RingtoetsPipingSurfaceLine
            {
                Name = "Surface line 1",
                ReferenceLineIntersectionWorldPoint = new Point2D(0.0, 0.0)
            };

            surfaceLine1.SetGeometry(new[]
            {
                new Point3D(0.0, 5.0, 0.0),
                new Point3D(0.0, 0.0, 1.0),
                new Point3D(0.0, -5.0, 0.0)
            });

            var surfaceLine2 = new RingtoetsPipingSurfaceLine
            {
                Name = "Surface line 2",
                ReferenceLineIntersectionWorldPoint = new Point2D(5.0, 0.0)
            };

            surfaceLine2.SetGeometry(new[]
            {
                new Point3D(5.0, 5.0, 0.0),
                new Point3D(5.0, 0.0, 1.0),
                new Point3D(5.0, -5.0, 0.0)
            });

            var surfaceLines = new[]
            {
                surfaceLine1,
                surfaceLine2
            };
            pipingFailureMechanism.AddSection(new FailureMechanismSection("Section 1", new List<Point2D>
            {
                new Point2D(0.0, 0.0),
                new Point2D(5.0, 0.0)
            }));

            pipingFailureMechanism.AddSection(new FailureMechanismSection("Section 2", new List<Point2D>
            {
                new Point2D(5.0, 0.0),
                new Point2D(10.0, 0.0)
            }));

            var nodeData = new PipingCalculationGroupContext(group,
                                                             surfaceLines,
                                                             new[]
                                                             {
                                                                 new TestStochasticSoilModel
                                                                 {
                                                                     Geometry =
                                                                     {
                                                                         new Point2D(0.0, 0.0), new Point2D(5.0, 0.0)
                                                                     },
                                                                 }
                                                             },
                                                             pipingFailureMechanism,
                                                             assessmentSectionMock);

            gui.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
            gui.Expect(g => g.MainWindow).Return(mainWindow);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Precondition
            foreach (var failureMechanismSectionResult in pipingFailureMechanism.SectionResults)
            {
                CollectionAssert.IsEmpty(failureMechanismSectionResult.CalculationScenarios);
            }

            DialogBoxHandler = (name, wnd) =>
            {
                var selectionDialog = new FormTester(name).TheObject as PipingSurfaceLineSelectionDialog;
                var grid = new ControlTester("SurfaceLineDataGrid", selectionDialog).TheObject as DataGridView;

                grid.Rows[0].Cells[0].Value = true;

                new ButtonTester("CustomCancelButton", selectionDialog).Click();
            };

            var contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl);

            // When
            contextMenu.Items[contextMenuAddGenerateCalculationsIndex].PerformClick();

            // Then
            foreach (var failureMechanismSectionResult in pipingFailureMechanism.SectionResults)
            {
                CollectionAssert.IsEmpty(failureMechanismSectionResult.CalculationScenarios);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void OnNodeRemoved_ParentIsPipingCalculationGroupContainingGroup_RemoveGroupAndNotifyObservers()
        {
            // Setup
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            var group = new CalculationGroup();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<StochasticSoilModel>(),
                                                             pipingFailureMechanismMock,
                                                             assessmentSectionMock);

            var parentGroup = new CalculationGroup();
            parentGroup.Children.Add(group);
            var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                                   Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                   Enumerable.Empty<StochasticSoilModel>(),
                                                                   pipingFailureMechanismMock,
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
        public void OnNodeRemoved_ParentIsPipingCalculationGroupContainingGroupContainingCalculations_RemoveGroupAndCalculationsAndNotifyObservers()
        {
            // Setup
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            var group = new CalculationGroup();
            var pipingFailureMechanism = GetFailureMechanism();
            var surfaceLines = pipingFailureMechanism.SurfaceLines.ToArray();

            var calculation = new PipingCalculationScenario(new GeneralPipingInput(), new NormProbabilityPipingInput())
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLines[0]
                }
            };

            group.Children.Add(calculation);

            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<StochasticSoilModel>(),
                                                             pipingFailureMechanism,
                                                             assessmentSectionMock);

            var parentGroup = new CalculationGroup();
            parentGroup.Children.Add(group);
            var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                                   Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                   Enumerable.Empty<StochasticSoilModel>(),
                                                                   pipingFailureMechanism,
                                                                   assessmentSectionMock);
            parentNodeData.Attach(observer);

            parentGroup.AddCalculationScenariosToFailureMechanismSectionResult(pipingFailureMechanism);

            // Precondition
            Assert.IsTrue(info.CanRemove(nodeData, parentNodeData));
            var sectionResults = pipingFailureMechanism.SectionResults.ToArray();
            CollectionAssert.Contains(sectionResults[0].CalculationScenarios, calculation);

            // Call
            info.OnNodeRemoved(nodeData, parentNodeData);

            // Assert
            CollectionAssert.DoesNotContain(parentGroup.Children, group);
            CollectionAssert.DoesNotContain(sectionResults[0].CalculationScenarios, calculation);
            mocks.VerifyAll();
        }

        [Test]
        [Combinatorial]
        public void OnDrop_DraggingPipingCalculationItemContextOntoGroupEnd_MoveCalculationItemInstanceToNewGroup(
            [Values(PipingCalculationType.Calculation, PipingCalculationType.Group)] PipingCalculationType draggedItemType)
        {
            // Setup
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();

            ICalculationBase draggedItem;
            object draggedItemContext;
            CreatePipingCalculationAndContext(draggedItemType, out draggedItem, out draggedItemContext, pipingFailureMechanismMock, assessmentSection);

            CalculationGroup originalOwnerGroup;
            PipingCalculationGroupContext originalOwnerGroupContext;
            CreatePipingCalculationGroupAndContext(out originalOwnerGroup, out originalOwnerGroupContext, pipingFailureMechanismMock, assessmentSection);
            originalOwnerGroup.Children.Add(draggedItem);

            CalculationGroup newOwnerGroup;
            PipingCalculationGroupContext newOwnerGroupContext;
            CreatePipingCalculationGroupAndContext(out newOwnerGroup, out newOwnerGroupContext, pipingFailureMechanismMock, assessmentSection);

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
            [Values(PipingCalculationType.Calculation, PipingCalculationType.Group)] PipingCalculationType draggedItemType,
            [Values(0, 2)] int newIndex)
        {
            // Setup
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();

            const string name = "Very cool name";
            ICalculationBase draggedItem;
            object draggedItemContext;
            CreatePipingCalculationAndContext(draggedItemType, out draggedItem, out draggedItemContext, pipingFailureMechanismMock, assessmentSection, name);

            var existingItemStub = mocks.Stub<ICalculationBase>();
            existingItemStub.Stub(ci => ci.Name).Return("");

            CalculationGroup originalOwnerGroup;
            PipingCalculationGroupContext originalOwnerGroupContext;
            CreatePipingCalculationGroupAndContext(out originalOwnerGroup, out originalOwnerGroupContext, pipingFailureMechanismMock, assessmentSection);
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
            [Values(PipingCalculationType.Calculation, PipingCalculationType.Group)] PipingCalculationType draggedItemType)
        {
            // Setup
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();

            ICalculationBase draggedItem;
            object draggedItemContext;
            CreatePipingCalculationAndContext(draggedItemType, out draggedItem, out draggedItemContext, pipingFailureMechanismMock, assessmentSection);

            CalculationGroup originalOwnerGroup;
            PipingCalculationGroupContext originalOwnerGroupContext;
            CreatePipingCalculationGroupAndContext(out originalOwnerGroup, out originalOwnerGroupContext, pipingFailureMechanismMock, assessmentSection);
            originalOwnerGroup.Children.Add(draggedItem);

            CalculationGroup newOwnerGroup;
            PipingCalculationGroupContext newOwnerGroupContext;
            CreatePipingCalculationGroupAndContext(out newOwnerGroup, out newOwnerGroupContext, pipingFailureMechanismMock, assessmentSection);

            var sameNamedItem = mocks.Stub<ICalculationBase>();
            sameNamedItem.Stub(sni => sni.Name).Return(draggedItem.Name);

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
                case PipingCalculationType.Calculation:
                    Assert.AreEqual("Nieuwe berekening", draggedItem.Name);
                    break;
                case PipingCalculationType.Group:
                    Assert.AreEqual("Nieuwe map", draggedItem.Name);
                    break;
            }

            mocks.VerifyAll();
        }

        private const int contextMenuAddGenerateCalculationsIndex = 1;
        private const int contextMenuAddCalculationGroupIndex = 0;
        private const int contextMenuAddCalculationIndex = 1;
        private const int contextMenuValidateAllIndex = 3;
        private const int contextMenuCalculateAllIndex = 4;
        private const int contextMenuClearOutputIndex = 5;

        /// <summary>
        /// Creates an instance of <see cref="CalculationGroup"/> and the corresponding
        /// <see cref="PipingCalculationGroupContext"/>.
        /// </summary>
        /// <param name="data">The created group without any children.</param>
        /// <param name="dataContext">The context object for <paramref name="data"/>, without any other data.</param>
        /// <param name="pipingFailureMechanism">The piping failure mechanism the item and context belong to.</param>
        /// <param name="assessmentSection">The assessment section the item and context belong to.</param>
        private void CreatePipingCalculationGroupAndContext(out CalculationGroup data, out PipingCalculationGroupContext dataContext, PipingFailureMechanism pipingFailureMechanism, IAssessmentSection assessmentSection)
        {
            data = new CalculationGroup();

            dataContext = new PipingCalculationGroupContext(data,
                                                            Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                            Enumerable.Empty<StochasticSoilModel>(),
                                                            pipingFailureMechanism,
                                                            assessmentSection);
        }

        /// <summary>
        /// Creates an instance of <see cref="ICalculationBase"/> and the corresponding context.
        /// </summary>
        /// <param name="type">Defines the implementation of <see cref="ICalculationBase"/> to be constructed.</param>
        /// <param name="data">Output: The concrete create class based on <paramref name="type"/>.</param>
        /// <param name="dataContext">Output: The <see cref="PipingContext{T}"/> corresponding with <paramref name="data"/>.</param>
        /// <param name="pipingFailureMechanism">The piping failure mechanism the item and context belong to.</param>
        /// <param name="assessmentSection">The assessment section the item and context belong to.</param>
        /// <param name="initialName">Optional: The name of <paramref name="data"/>.</param>
        /// <exception cref="System.NotSupportedException"></exception>
        private static void CreatePipingCalculationAndContext(PipingCalculationType type, out ICalculationBase data, out object dataContext, PipingFailureMechanism pipingFailureMechanism, IAssessmentSection assessmentSection, string initialName = null)
        {
            switch (type)
            {
                case PipingCalculationType.Calculation:
                    var calculation = new PipingCalculationScenario(new GeneralPipingInput(), new NormProbabilityPipingInput());
                    if (initialName != null)
                    {
                        calculation.Name = initialName;
                    }
                    data = calculation;
                    dataContext = new PipingCalculationScenarioContext(calculation,
                                                               Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                               Enumerable.Empty<StochasticSoilModel>(),
                                                               pipingFailureMechanism,
                                                               assessmentSection);
                    break;
                case PipingCalculationType.Group:
                    var group = new CalculationGroup();
                    if (initialName != null)
                    {
                        group.Name = initialName;
                    }
                    data = group;
                    dataContext = new PipingCalculationGroupContext(group,
                                                                    Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                    Enumerable.Empty<StochasticSoilModel>(),
                                                                    pipingFailureMechanism,
                                                                    assessmentSection);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="PipingFailureMechanism"/> with sections and a surface line.
        /// </summary>
        /// <returns>A new instance of <see cref="PipingFailureMechanism"/>.</returns>
        private static PipingFailureMechanism GetFailureMechanism()
        {
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "Surface line",
                ReferenceLineIntersectionWorldPoint = new Point2D(0.0, 0.0)
            };

            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0.0, 5.0, 0.0),
                new Point3D(0.0, 0.0, 1.0),
                new Point3D(0.0, -5.0, 0.0)
            });

            var failureMechanism = new PipingFailureMechanism
            {
                SurfaceLines =
                {
                    surfaceLine
                },
                StochasticSoilModels =
                {
                    new TestStochasticSoilModel
                    {
                        Geometry =
                        {
                            new Point2D(0.0, 0.0), new Point2D(5.0, 0.0)
                        },
                    }
                }
            };

            failureMechanism.AddSection(new FailureMechanismSection("Section", new List<Point2D>
            {
                new Point2D(0.0, 0.0),
                new Point2D(5.0, 0.0)
            }));

            return failureMechanism;
        }

        /// <summary>
        /// Type indicator for implementations of <see cref="ICalculationBase"/> to be created in a test.
        /// </summary>
        public enum PipingCalculationType
        {
            /// <summary>
            /// Indicates <see cref="PipingCalculation"/>.
            /// </summary>
            Calculation,

            /// <summary>
            /// Indicates <see cref="CalculationGroup"/>.
            /// </summary>
            Group
        }
    }
}