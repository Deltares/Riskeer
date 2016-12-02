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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Controls.DataGrid;
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
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Plugin;
using Ringtoets.Piping.Primitives;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.Piping.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class PipingCalculationGroupContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuAddCalculationGroupIndexRootGroup = 4;
        private const int contextMenuAddCalculationIndexRootGroup = 5;
        private const int contextMenuValidateAllIndexRootGroup = 9;
        private const int contextMenuCalculateAllIndexRootGroup = 10;
        private const int contextMenuClearOutputIndexRootGroup = 11;
        private const int contextMenuExpandAllIndexRootGroup = 13;
        private const int contextMenuCollapseAllIndexRootGroup = 14;
        private const int contextMenuPropertiesIndexRootGroup = 16;

        private const int contextMenuAddCalculationGroupIndexNestedGroup = 0;
        private const int contextMenuAddCalculationIndexNestedGroup = 1;
        private const int contextMenuValidateAllIndexNestedGroup = 3;
        private const int contextMenuCalculateAllIndexNestedGroup = 4;
        private const int contextMenuClearOutputNestedGroupIndex = 5;

        private const int customOnlyContextMenuAddGenerateCalculationsIndex = 2;

        private MockRepository mocks;
        private PipingPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new PipingPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(PipingCalculationGroupContext));
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            mocks.ReplayAll();

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
            var pipingFailureMechanism = new PipingFailureMechanism();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var groupContext = new PipingCalculationGroupContext(group,
                                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                 Enumerable.Empty<StochasticSoilModel>(),
                                                                 pipingFailureMechanism,
                                                                 assessmentSectionMock);

            // Call
            var children = info.ChildNodeObjects(groupContext);

            // Assert
            CollectionAssert.IsEmpty(children);
        }

        [Test]
        public void ChildNodeObjects_GroupWithMixedContents_ReturnChildren()
        {
            // Setup
            var calculationItem = mocks.StrictMock<ICalculationBase>();

            var childCalculation = new PipingCalculationScenario(new GeneralPipingInput());

            var childGroup = new CalculationGroup();

            var group = new CalculationGroup();
            group.Children.Add(calculationItem);
            group.Children.Add(childCalculation);
            group.Children.Add(childGroup);

            var pipingFailureMechanism = new PipingFailureMechanism();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            mocks.ReplayAll();

            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<StochasticSoilModel>(),
                                                             pipingFailureMechanism,
                                                             assessmentSectionMock);

            // Call
            var children = info.ChildNodeObjects(nodeData).ToArray();

            // Assert
            Assert.AreEqual(group.Children.Count, children.Length);
            Assert.AreSame(calculationItem, children[0]);
            var returnedCalculationContext = (PipingCalculationScenarioContext) children[1];
            Assert.AreSame(childCalculation, returnedCalculationContext.WrappedData);
            Assert.AreSame(pipingFailureMechanism, returnedCalculationContext.FailureMechanism);
            var returnedCalculationGroupContext = (PipingCalculationGroupContext) children[2];
            Assert.AreSame(childGroup, returnedCalculationGroupContext.WrappedData);
            Assert.AreSame(pipingFailureMechanism, returnedCalculationGroupContext.FailureMechanism);
            Assert.AreSame(assessmentSectionMock, returnedCalculationGroupContext.AssessmentSection);
        }

        [Test]
        public void ContextMenuStrip_NestedCalculationGroupWithCalculationOutput_ReturnContextWithItems()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var group = new CalculationGroup();
            var parentGroup = new CalculationGroup();

            group.Children.Add(new PipingCalculationScenario(new GeneralPipingInput())
            {
                Output = new TestPipingOutput(),
                SemiProbabilisticOutput = new TestPipingSemiProbabilisticOutput()
            });

            var pipingFailureMechanism = new PipingFailureMechanism();
            pipingFailureMechanism.AddSection(new FailureMechanismSection("A", new[]
            {
                new Point2D(0, 0)
            }));

            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<StochasticSoilModel>(),
                                                             pipingFailureMechanism,
                                                             assessmentSectionMock);
            var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                                   Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                   Enumerable.Empty<StochasticSoilModel>(),
                                                                   pipingFailureMechanism,
                                                                   assessmentSectionMock);

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
            gui.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);

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
                                                              RingtoetsCommonFormsResources.CalculationIcon);

                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndexNestedGroup,
                                                              RingtoetsCommonFormsResources.Validate_all,
                                                              "Valideer alle berekeningen binnen deze berekeningsmap.",
                                                              RingtoetsCommonFormsResources.ValidateAllIcon);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndexNestedGroup,
                                                              RingtoetsCommonFormsResources.Calculate_all,
                                                              "Voer alle berekeningen binnen deze berekeningsmap uit.",
                                                              RingtoetsCommonFormsResources.CalculateAllIcon);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearOutputNestedGroupIndex,
                                                              "&Wis alle uitvoer...",
                                                              "Wis de uitvoer van alle berekeningen binnen deze berekeningsmap.",
                                                              RingtoetsCommonFormsResources.ClearIcon);

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
            var gui = mocks.StrictMock<IGui>();
            var group = new CalculationGroup();

            group.Children.Add(new PipingCalculationScenario(new GeneralPipingInput())
            {
                Output = new TestPipingOutput(),
                SemiProbabilisticOutput = new TestPipingSemiProbabilisticOutput()
            });

            var pipingFailureMechanism = new PipingFailureMechanism();
            pipingFailureMechanism.AddSection(new FailureMechanismSection("A", new[]
            {
                new Point2D(0, 0)
            }));
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<StochasticSoilModel>(),
                                                             pipingFailureMechanism,
                                                             assessmentSectionMock);

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
                gui.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());
                viewCommandsHandler.Expect(vc => vc.CanOpenViewFor(nodeData)).Return(true);

                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl);

                // Assert
                Assert.AreEqual(17, menu.Items.Count);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationGroupIndexRootGroup,
                                                              RingtoetsCommonFormsResources.CalculationGroup_Add_CalculationGroup,
                                                              "Voeg een nieuwe berekeningsmap toe aan deze berekeningsmap.",
                                                              RingtoetsCommonFormsResources.AddFolderIcon);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationIndexRootGroup,
                                                              RingtoetsCommonFormsResources.CalculationGroup_Add_Calculation,
                                                              "Voeg een nieuwe berekening toe aan deze berekeningsmap.",
                                                              RingtoetsCommonFormsResources.CalculationIcon);

                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndexRootGroup,
                                                              RingtoetsCommonFormsResources.Validate_all,
                                                              "Valideer alle berekeningen binnen deze berekeningsmap.",
                                                              RingtoetsCommonFormsResources.ValidateAllIcon);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndexRootGroup,
                                                              RingtoetsCommonFormsResources.Calculate_all,
                                                              "Voer alle berekeningen binnen deze berekeningsmap uit.",
                                                              RingtoetsCommonFormsResources.CalculateAllIcon);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearOutputIndexRootGroup,
                                                              "&Wis alle uitvoer...",
                                                              "Wis de uitvoer van alle berekeningen binnen deze berekeningsmap.",
                                                              RingtoetsCommonFormsResources.ClearIcon);

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
                    menu.Items[3],
                    menu.Items[6],
                    menu.Items[8],
                    menu.Items[12],
                    menu.Items[15]
                }, typeof(ToolStripSeparator));
            }
        }

        [Test]
        public void ContextMenuStrip_WithoutParentNodeDefaultBehaviorAndWithoutAvailableSurfaceLines_GenerateCalculationsDisabled()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var group = new CalculationGroup();

                var pipingFailureMechanism = new PipingFailureMechanism();
                var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

                var nodeData = new PipingCalculationGroupContext(group,
                                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                 new[]
                                                                 {
                                                                     new TestStochasticSoilModel()
                                                                 },
                                                                 pipingFailureMechanism,
                                                                 assessmentSectionMock);

                var gui = mocks.StrictMock<IGui>();
                gui.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, 2,
                                                                  RingtoetsCommonFormsResources.CalculationGroup_Generate_Scenarios,
                                                                  PipingFormsResources.PipingCalculationGroup_Generate_PipingCalculations_NoSurfaceLinesOrSoilModels_ToolTip,
                                                                  RingtoetsCommonFormsResources.GenerateScenariosIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_WithoutParentNodeDefaultBehaviorAndWithoutAvailableSoilModels_GenerateCalculationsDisabled()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var group = new CalculationGroup();

                var pipingFailureMechanism = new PipingFailureMechanism();
                var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

                var nodeData = new PipingCalculationGroupContext(group,
                                                                 new[]
                                                                 {
                                                                     new RingtoetsPipingSurfaceLine()
                                                                 },
                                                                 Enumerable.Empty<StochasticSoilModel>(),
                                                                 pipingFailureMechanism,
                                                                 assessmentSectionMock);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.StrictMock<IGui>();
                gui.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, 2,
                                                                  RingtoetsCommonFormsResources.CalculationGroup_Generate_Scenarios,
                                                                  PipingFormsResources.PipingCalculationGroup_Generate_PipingCalculations_NoSurfaceLinesOrSoilModels_ToolTip,
                                                                  RingtoetsCommonFormsResources.GenerateScenariosIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_WithoutParentNodeDefaultBehaviorAndWithAvailableSurfaceLinesAndSoilModels_GenerateCalculationsEnabled()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var group = new CalculationGroup();

                var pipingFailureMechanism = new PipingFailureMechanism();
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
                                                                 pipingFailureMechanism,
                                                                 assessmentSectionMock);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.StrictMock<IGui>();
                gui.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, 2,
                                                                  RingtoetsCommonFormsResources.CalculationGroup_Generate_Scenarios,
                                                                  PipingFormsResources.PipingCalculationGroup_Generate_PipingCalculations_ToolTip,
                                                                  RingtoetsCommonFormsResources.GenerateScenariosIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_NestedCalculationGroupWithNoCalculations_ValidateAndCalculateAllDisabled()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var group = new CalculationGroup();
                var parentGroup = new CalculationGroup();

                var pipingFailureMechanism = new PipingFailureMechanism();
                var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
                var nodeData = new PipingCalculationGroupContext(group,
                                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                 Enumerable.Empty<StochasticSoilModel>(),
                                                                 pipingFailureMechanism,
                                                                 assessmentSectionMock);
                var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                                       Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                       Enumerable.Empty<StochasticSoilModel>(),
                                                                       pipingFailureMechanism,
                                                                       assessmentSectionMock);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.StrictMock<IGui>();
                gui.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);

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
        public void ContextMenuStrip_ClickOnAddGroupItem_AddGroupToCalculationGroupAndNotifyObservers()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var group = new CalculationGroup();
                var parentGroup = new CalculationGroup();
                var pipingFailureMechanism = new PipingFailureMechanism();
                var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
                var nodeData = new PipingCalculationGroupContext(group,
                                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                 Enumerable.Empty<StochasticSoilModel>(),
                                                                 pipingFailureMechanism,
                                                                 assessmentSectionMock);
                var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                                       Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                       Enumerable.Empty<StochasticSoilModel>(),
                                                                       pipingFailureMechanism,
                                                                       assessmentSectionMock);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.StrictMock<IGui>();
                gui.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);

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
        public void ContextMenuStrip_ClickOnAddCalculationItem_AddCalculationToCalculationGroupAndNotifyObservers()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var group = new CalculationGroup();
                var parentGroup = new CalculationGroup();
                var pipingFailureMechanism = new PipingFailureMechanism();
                var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
                var nodeData = new PipingCalculationGroupContext(group,
                                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                 Enumerable.Empty<StochasticSoilModel>(),
                                                                 pipingFailureMechanism,
                                                                 assessmentSectionMock);
                var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                                       Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                       Enumerable.Empty<StochasticSoilModel>(),
                                                                       pipingFailureMechanism,
                                                                       assessmentSectionMock);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.StrictMock<IGui>();
                gui.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);

                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());
                mocks.ReplayAll();

                plugin.Gui = gui;

                var calculationItem = new PipingCalculationScenario(new GeneralPipingInput())
                {
                    Name = "Nieuwe berekening"
                };
                group.Children.Add(calculationItem);

                nodeData.Attach(observer);

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Precondition
                    Assert.AreEqual(1, group.Children.Count);

                    // Call
                    contextMenu.Items[contextMenuAddCalculationIndexNestedGroup].PerformClick();

                    // Assert
                    Assert.AreEqual(2, group.Children.Count);
                    var newlyAddedItem = group.Children.Last();
                    Assert.IsInstanceOf<PipingCalculation>(newlyAddedItem);
                    Assert.AreEqual("Nieuwe berekening (1)", newlyAddedItem.Name,
                                    "An item with the same name default name already exists, therefore '(1)' needs to be appended.");
                }
            }
        }

        [Test]
        public void ContextMenuStrip_ClickOnValidateAllItem_ValidateAllChildCalculations()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                PipingCalculationScenario validCalculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
                validCalculation.Name = "A";
                PipingCalculationScenario invalidCalculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithInvalidInput();
                invalidCalculation.Name = "B";

                var childGroup = new CalculationGroup();
                childGroup.Children.Add(validCalculation);

                var emptyChildGroup = new CalculationGroup();
                var group = new CalculationGroup();
                var parentGroup = new CalculationGroup();

                group.Children.Add(childGroup);
                group.Children.Add(emptyChildGroup);
                group.Children.Add(invalidCalculation);

                var pipingFailureMechanism = new PipingFailureMechanism();
                var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

                var nodeData = new PipingCalculationGroupContext(group,
                                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                 Enumerable.Empty<StochasticSoilModel>(),
                                                                 pipingFailureMechanism,
                                                                 assessmentSectionMock);
                var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                                       Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                       Enumerable.Empty<StochasticSoilModel>(),
                                                                       pipingFailureMechanism,
                                                                       assessmentSectionMock);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.StrictMock<IGui>();
                gui.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);

                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Call
                    Action call = () => contextMenu.Items[contextMenuValidateAllIndexNestedGroup].PerformClick();

                    // Assert
                    TestHelper.AssertLogMessages(call, messages =>
                    {
                        var msgs = messages.ToArray();
                        Assert.AreEqual(9, msgs.Length);
                        StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", validCalculation.Name), msgs[0]);
                        StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", validCalculation.Name), msgs[1]);

                        StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", invalidCalculation.Name), msgs[2]);
                        // Some validation error from validation service
                        StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", invalidCalculation.Name), msgs[8]);
                    });
                }
            }
        }

        [Test]
        public void ContextMenuStrip_ClickOnCalculateAllItem_ScheduleAllChildCalculations()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var mainWindow = mocks.Stub<IMainWindow>();

                PipingCalculationScenario validCalculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
                validCalculation.Name = "A";
                PipingCalculationScenario invalidCalculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithInvalidInput();
                invalidCalculation.Name = "B";

                var childGroup = new CalculationGroup();
                childGroup.Children.Add(validCalculation);

                var emptyChildGroup = new CalculationGroup();

                var group = new CalculationGroup();
                var parentGroup = new CalculationGroup();

                group.Children.Add(childGroup);
                group.Children.Add(emptyChildGroup);
                group.Children.Add(invalidCalculation);

                var pipingFailureMechanism = new PipingFailureMechanism();
                pipingFailureMechanism.AddSection(new FailureMechanismSection("A", new[]
                {
                    new Point2D(0, 0)
                }));

                IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(
                    pipingFailureMechanism, mocks);

                var nodeData = new PipingCalculationGroupContext(group,
                                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                 Enumerable.Empty<StochasticSoilModel>(),
                                                                 pipingFailureMechanism,
                                                                 assessmentSectionStub);
                var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                                       Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                       Enumerable.Empty<StochasticSoilModel>(),
                                                                       pipingFailureMechanism,
                                                                       assessmentSectionStub);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.StrictMock<IGui>();
                gui.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Expect(g => g.MainWindow).Return(mainWindow);

                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    DialogBoxHandler = (name, wnd) =>
                    {
                        // Expect an activity dialog which is automatically closed
                    };

                    // Call
                    contextMenu.Items[contextMenuCalculateAllIndexNestedGroup].PerformClick();
                }
            }
            // Assert
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void ContextMenuStrip_ClickOnClearOutputItem_ClearOutputAllChildCalculationsAndNotifyCalculationObservers(bool confirm)
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var calculation1Observer = mocks.StrictMock<IObserver>();
                var calculation2Observer = mocks.StrictMock<IObserver>();
                if (confirm)
                {
                    calculation1Observer.Expect(o => o.UpdateObserver());
                    calculation2Observer.Expect(o => o.UpdateObserver());
                }

                PipingCalculationScenario calculation1 = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
                calculation1.Name = "A";
                calculation1.Output = new TestPipingOutput();
                calculation1.SemiProbabilisticOutput = new TestPipingSemiProbabilisticOutput();
                calculation1.Attach(calculation1Observer);
                PipingCalculationScenario calculation2 = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
                calculation2.Name = "B";
                calculation2.Output = new TestPipingOutput();
                calculation2.SemiProbabilisticOutput = new TestPipingSemiProbabilisticOutput();
                calculation1.Attach(calculation2Observer);

                var childGroup = new CalculationGroup();
                childGroup.Children.Add(calculation1);

                var emptyChildGroup = new CalculationGroup();
                var group = new CalculationGroup();
                var parentGroup = new CalculationGroup();

                group.Children.Add(childGroup);
                group.Children.Add(emptyChildGroup);
                group.Children.Add(calculation2);

                var pipingFailureMechanism = new PipingFailureMechanism();
                IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(
                    pipingFailureMechanism, mocks);

                var nodeData = new PipingCalculationGroupContext(group,
                                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                 Enumerable.Empty<StochasticSoilModel>(),
                                                                 pipingFailureMechanism,
                                                                 assessmentSectionStub);
                var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                                       Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                       Enumerable.Empty<StochasticSoilModel>(),
                                                                       pipingFailureMechanism,
                                                                       assessmentSectionStub);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.StrictMock<IGui>();
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

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Call
                    contextMenu.Items[contextMenuClearOutputNestedGroupIndex].PerformClick();

                    // Assert
                    Assert.AreNotEqual(confirm, calculation1.HasOutput);
                    Assert.AreNotEqual(confirm, calculation2.HasOutput);

                    Assert.AreEqual("Bevestigen", messageBoxTitle);
                    Assert.AreEqual("Weet u zeker dat u alle uitvoer wilt wissen?", messageBoxText);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_ClickOnGenerateCalculationsItemWithSurfaceLinesAndSoilModels_ShowSurfaceLineSelectionView()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var group = new CalculationGroup();

                var pipingFailureMechanism = new PipingFailureMechanism();
                var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

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
                                                                 pipingFailureMechanism,
                                                                 assessmentSectionMock);
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var mainWindow = mocks.Stub<IMainWindow>();

                var gui = mocks.StrictMock<IGui>();
                gui.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Expect(g => g.MainWindow).Return(mainWindow);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                plugin.Gui = gui;

                PipingSurfaceLineSelectionDialog selectionDialog = null;
                DataGridViewControl grid = null;
                int rowCount = 0;
                DialogBoxHandler = (name, wnd) =>
                {
                    selectionDialog = (PipingSurfaceLineSelectionDialog) new FormTester(name).TheObject;
                    grid = (DataGridViewControl) new ControlTester("DataGridViewControl", selectionDialog).TheObject;
                    rowCount = grid.Rows.Count;
                    new ButtonTester("CustomCancelButton", selectionDialog).Click();
                };

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Call
                    contextMenu.Items[customOnlyContextMenuAddGenerateCalculationsIndex].PerformClick();

                    // Assert
                    Assert.NotNull(selectionDialog);
                    Assert.NotNull(grid);
                    Assert.AreEqual(2, rowCount);
                }
            }
        }

        [Test]
        public void GivenPipingCalculationsViewGenerateScenariosButtonClicked_WhenSurfaceLineSelectedAndDialogClosed_ThenUpdateSectionResultScenarios()
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                var pipingFailureMechanism = new PipingFailureMechanism();
                var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

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

                var nodeData = new PipingCalculationGroupContext(pipingFailureMechanism.CalculationsGroup,
                                                                 surfaceLines,
                                                                 new[]
                                                                 {
                                                                     new TestStochasticSoilModel
                                                                     {
                                                                         Geometry =
                                                                         {
                                                                             new Point2D(0.0, 0.0), new Point2D(5.0, 0.0)
                                                                         }
                                                                     }
                                                                 },
                                                                 pipingFailureMechanism,
                                                                 assessmentSectionMock);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var mainWindow = mocks.Stub<IMainWindow>();

                var gui = mocks.StrictMock<IGui>();
                gui.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Expect(g => g.MainWindow).Return(mainWindow);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                plugin.Gui = gui;

                DialogBoxHandler = (name, wnd) =>
                {
                    var selectionDialog = (PipingSurfaceLineSelectionDialog) new FormTester(name).TheObject;
                    var grid = (DataGridViewControl) new ControlTester("DataGridViewControl", selectionDialog).TheObject;

                    grid.Rows[0].Cells[0].Value = true;

                    new ButtonTester("DoForSelectedButton", selectionDialog).Click();
                };

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    contextMenu.Items[customOnlyContextMenuAddGenerateCalculationsIndex].PerformClick();

                    // Then
                    var failureMechanismSectionResult1 = pipingFailureMechanism.SectionResults.First();
                    var failureMechanismSectionResult2 = pipingFailureMechanism.SectionResults.ElementAt(1);

                    var pipingCalculationScenarios = pipingFailureMechanism.Calculations.OfType<PipingCalculationScenario>().ToArray();
                    Assert.AreEqual(2, failureMechanismSectionResult1.GetCalculationScenarios(pipingCalculationScenarios).Count());

                    foreach (var calculationScenario in failureMechanismSectionResult1.GetCalculationScenarios(pipingCalculationScenarios))
                    {
                        Assert.IsInstanceOf<ICalculationScenario>(calculationScenario);
                    }

                    CollectionAssert.IsEmpty(failureMechanismSectionResult2.GetCalculationScenarios(pipingCalculationScenarios));
                }
            }
        }

        [Test]
        public void GivenPipingCalculationsViewGenerateScenariosButtonClicked_WhenCancelButtonClickedAndDialogClosed_ThenSectionResultScenariosNotUpdated()
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                var group = new CalculationGroup();

                var pipingFailureMechanism = new PipingFailureMechanism();
                var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

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
                                                                         }
                                                                     }
                                                                 },
                                                                 pipingFailureMechanism,
                                                                 assessmentSectionMock);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var mainWindow = mocks.Stub<IMainWindow>();

                var gui = mocks.StrictMock<IGui>();
                gui.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Expect(g => g.MainWindow).Return(mainWindow);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                plugin.Gui = gui;

                // Precondition
                foreach (var failureMechanismSectionResult in pipingFailureMechanism.SectionResults)
                {
                    CollectionAssert.IsEmpty(failureMechanismSectionResult.GetCalculationScenarios(pipingFailureMechanism.Calculations.OfType<PipingCalculationScenario>()));
                }

                DialogBoxHandler = (name, wnd) =>
                {
                    var selectionDialog = (PipingSurfaceLineSelectionDialog) new FormTester(name).TheObject;
                    var grid = (DataGridViewControl) new ControlTester("DataGridViewControl", selectionDialog).TheObject;

                    grid.Rows[0].Cells[0].Value = true;

                    new ButtonTester("CustomCancelButton", selectionDialog).Click();
                };

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    contextMenu.Items[customOnlyContextMenuAddGenerateCalculationsIndex].PerformClick();

                    // Then
                    foreach (var failureMechanismSectionResult in pipingFailureMechanism.SectionResults)
                    {
                        CollectionAssert.IsEmpty(failureMechanismSectionResult.GetCalculationScenarios(pipingFailureMechanism.Calculations.OfType<PipingCalculationScenario>()));
                    }
                }
            }
        }

        [Test]
        public void OnNodeRemoved_ParentIsPipingCalculationGroupContainingGroup_RemoveGroupAndNotifyObservers()
        {
            // Setup
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            var group = new CalculationGroup();
            var pipingFailureMechanism = new PipingFailureMechanism();
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

            // Precondition
            Assert.IsTrue(info.CanRemove(nodeData, parentNodeData));

            // Call
            info.OnNodeRemoved(nodeData, parentNodeData);

            // Assert
            CollectionAssert.DoesNotContain(parentGroup.Children, group);
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

            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
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

            var parentGroup = pipingFailureMechanism.CalculationsGroup;
            parentGroup.Children.Add(group);
            var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                                   Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                   Enumerable.Empty<StochasticSoilModel>(),
                                                                   pipingFailureMechanism,
                                                                   assessmentSectionMock);
            parentNodeData.Attach(observer);

            // Precondition
            Assert.IsTrue(info.CanRemove(nodeData, parentNodeData));
            var sectionResults = pipingFailureMechanism.SectionResults.ToArray();
            CollectionAssert.Contains(sectionResults[0].GetCalculationScenarios(pipingFailureMechanism.Calculations.OfType<PipingCalculationScenario>()), calculation);

            // Call
            info.OnNodeRemoved(nodeData, parentNodeData);

            // Assert
            CollectionAssert.DoesNotContain(parentGroup.Children, group);
            CollectionAssert.DoesNotContain(sectionResults[0].GetCalculationScenarios(pipingFailureMechanism.Calculations.OfType<PipingCalculationScenario>()), calculation);
        }

        public override void TearDown()
        {
            plugin.Dispose();
            mocks.VerifyAll();

            base.TearDown();
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
                        }
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
    }
}