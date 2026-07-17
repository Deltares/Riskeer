// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Gui;
using Core.Gui.Commands;
using Core.Gui.ContextMenu;
using Core.Gui.Forms.Main;
using Core.Gui.TestUtil;
using Core.Gui.TestUtil.ContextMenu;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using NSubstitute;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Forms;
using Riskeer.Common.Service.TestUtil;
using Riskeer.Common.Util.Helpers;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Data.Input.Structures;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityPointStructures.Data.TestUtil;
using Riskeer.StabilityPointStructures.Forms.PresentationObjects;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.StabilityPointStructures.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class StabilityPointStructuresCalculationGroupContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuGenerateCalculationsIndexRootGroup = 5;
        private const int contextMenuAddCalculationGroupIndexRootGroup = 7;
        private const int contextMenuAddCalculationIndexRootGroup = 8;
        private const int contextMenuUpdateForeshoreProfileIndexRootGroup = 10;
        private const int contextMenuUpdateStructureAllIndexRootGroup = 11;
        private const int contextMenuValidateAllIndexRootGroup = 13;
        private const int contextMenuCalculateAllIndexRootGroup = 14;
        private const int contextMenuClearAllIndexRootGroup = 16;
        private const int contextMenuClearIllustrationPointsIndexRootGroup = 17;

        private const int contextMenuDuplicateIndexNestedGroup = 3;
        private const int contextMenuAddCalculationGroupIndexNestedGroup = 5;
        private const int contextMenuAddCalculationIndexNestedGroup = 6;
        private const int contextMenuUpdateForeshoreProfileIndexNestedGroup = 9;
        private const int contextMenuUpdateStructureAllIndexNestedGroup = 10;
        private const int contextMenuValidateAllIndexNestedGroup = 12;
        private const int contextMenuCalculateAllIndexNestedGroup = 13;
        private const int contextMenuClearAllIndexNestedGroup = 15;
        private const int contextMenuClearIllustrationPointsIndexNestedGroup = 16;

        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, nameof(HydraulicBoundaryData));
        private static readonly string validHlcdFilePath = Path.Combine(testDataPath, "hlcd.sqlite");
        private static readonly string validHrdFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
        private static readonly string validHrdFileVersion = "Dutch coast South19-11-2015 12:0013";

        private IGui gui;
        private TreeNodeInfo info;
        private StabilityPointStructuresPlugin plugin;

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup

            // Assert
            Assert.IsNotNull(info.Text);
            Assert.IsNull(info.ForeColor);
            Assert.IsNotNull(info.Image);
            Assert.IsNotNull(info.ContextMenuStrip);
            Assert.IsNotNull(info.EnsureVisibleOnCreate);
            Assert.IsNull(info.ExpandOnCreate);
            Assert.IsNotNull(info.ChildNodeObjects);
            Assert.IsNotNull(info.CanRename);
            Assert.IsNotNull(info.OnNodeRenamed);
            Assert.IsNotNull(info.CanRemove);
            Assert.IsNotNull(info.OnNodeRemoved);
            Assert.IsNull(info.CanCheck);
            Assert.IsNull(info.CheckedState);
            Assert.IsNull(info.OnNodeChecked);
            Assert.IsNotNull(info.CanDrag);
            Assert.IsNotNull(info.CanDrop);
            Assert.IsNotNull(info.CanInsert);
            Assert.IsNotNull(info.OnDrop);
        }

        [Test]
        public void ChildNodeObjects_EmptyGroup_ReturnEmpty()
        {
            // Setup
            var group = new CalculationGroup();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var assessmentSection = Substitute.For<IAssessmentSection>();

            var groupContext = new StabilityPointStructuresCalculationGroupContext(group,
                                                                                   null,
                                                                                   failureMechanism,
                                                                                   assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(groupContext);

            // Assert
            CollectionAssert.IsEmpty(children);
        }

        [Test]
        public void ChildNodeObjects_GroupWithMixedContents_ReturnChildren()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var group = new CalculationGroup();
            var childGroup = new CalculationGroup();
            var childCalculation = new StructuresCalculationScenario<StabilityPointStructuresInput>();

            group.Children.Add(childGroup);
            group.Children.Add(childCalculation);

            var groupContext = new StabilityPointStructuresCalculationGroupContext(group,
                                                                                   null,
                                                                                   failureMechanism,
                                                                                   assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(groupContext).ToArray();

            // Assert
            Assert.AreEqual(group.Children.Count, children.Length);
            var calculationGroupContext = (StabilityPointStructuresCalculationGroupContext) children[0];
            Assert.AreSame(childGroup, calculationGroupContext.WrappedData);
            Assert.AreSame(group, calculationGroupContext.Parent);
            Assert.AreSame(failureMechanism, calculationGroupContext.FailureMechanism);
            Assert.AreSame(assessmentSection, calculationGroupContext.AssessmentSection);
            var calculationContext = (StabilityPointStructuresCalculationScenarioContext) children[1];
            Assert.AreSame(childCalculation, calculationContext.WrappedData);
            Assert.AreSame(group, calculationContext.Parent);
            Assert.AreSame(assessmentSection, calculationContext.AssessmentSection);
        }

        [Test]
        public void ContextMenuStrip_WithoutParentNodeDefaultBehavior_CallsContextMenuBuilderMethods()
        {
            // Setup
            var group = new CalculationGroup();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var groupContext = new StabilityPointStructuresCalculationGroupContext(group,
                                                                                   null,
                                                                                   failureMechanism,
                                                                                   assessmentSection);

            var menuBuilder = Substitute.For<IContextMenuBuilder>();
            menuBuilder.AddOpenItem().Returns(menuBuilder);
            menuBuilder.AddSeparator().Returns(menuBuilder);
            menuBuilder.AddImportItem().Returns(menuBuilder);
            menuBuilder.AddExportItem().Returns(menuBuilder);
            menuBuilder.AddSeparator().Returns(menuBuilder);
            menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>()).Returns(menuBuilder);
            menuBuilder.AddSeparator().Returns(menuBuilder);
            menuBuilder.AddDeleteChildrenItem().Returns(menuBuilder);
            menuBuilder.AddSeparator().Returns(menuBuilder);
            menuBuilder.AddCollapseAllItem().Returns(menuBuilder);
            menuBuilder.AddExpandAllItem().Returns(menuBuilder);
            menuBuilder.AddSeparator().Returns(menuBuilder);
            menuBuilder.AddPropertiesItem().Returns(menuBuilder);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Get(groupContext, treeViewControl).Returns(menuBuilder);
                gui.ViewCommands.Returns(Substitute.For<IViewCommands>());
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                // Call
                info.ContextMenuStrip(groupContext, null, treeViewControl);
            }

            // Assert
            Received.InOrder(() =>
            {
                menuBuilder.AddOpenItem();
                menuBuilder.AddSeparator();
                menuBuilder.AddImportItem();
                menuBuilder.AddExportItem();
                menuBuilder.AddSeparator();
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                menuBuilder.AddSeparator();
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                menuBuilder.AddSeparator();
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                menuBuilder.AddSeparator();
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                menuBuilder.AddSeparator();
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                menuBuilder.AddDeleteChildrenItem();
                menuBuilder.AddSeparator();
                menuBuilder.AddCollapseAllItem();
                menuBuilder.AddExpandAllItem();
                menuBuilder.AddSeparator();
                menuBuilder.AddPropertiesItem();
                menuBuilder.Build();
            });
        }

        [Test]
        public void ContextMenuStrip_WithoutParentNodeDefaultBehavior_AddCustomItems()
        {
            // Setup
            var group = new CalculationGroup();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var groupContext = new StabilityPointStructuresCalculationGroupContext(group,
                                                                                   null,
                                                                                   failureMechanism,
                                                                                   assessmentSection);
            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                gui.Get(groupContext, treeViewControl).Returns(menuBuilder);
                gui.ViewCommands.Returns(Substitute.For<IViewCommands>());
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(groupContext, null, treeViewControl))
                {
                    // Assert
                    Assert.AreEqual(24, menu.Items.Count);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuGenerateCalculationsIndexRootGroup,
                                                                  "Genereer &berekeningen...",
                                                                  "Er zijn geen kunstwerken beschikbaar om berekeningen voor te genereren.",
                                                                  RiskeerCommonFormsResources.GenerateScenariosIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationGroupIndexRootGroup,
                                                                  "&Map toevoegen",
                                                                  "Voeg een nieuwe map toe aan deze map met berekeningen.",
                                                                  RiskeerCommonFormsResources.AddFolderIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationIndexRootGroup,
                                                                  "Berekening &toevoegen",
                                                                  "Voeg een nieuwe berekening toe aan deze map met berekeningen.",
                                                                  RiskeerCommonFormsResources.ProbabilisticCalculationIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuUpdateForeshoreProfileIndexRootGroup,
                                                                  "&Bijwerken voorlandprofielen...",
                                                                  "Er zijn geen berekeningen om bij te werken.",
                                                                  RiskeerCommonFormsResources.UpdateItemIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuUpdateStructureAllIndexRootGroup,
                                                                  "Bijwerken &kunstwerken...",
                                                                  "Er zijn geen berekeningen om bij te werken.",
                                                                  RiskeerCommonFormsResources.UpdateItemIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndexRootGroup,
                                                                  "Alles &valideren",
                                                                  "Er zijn geen berekeningen om te valideren.",
                                                                  RiskeerCommonFormsResources.ValidateAllIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndexRootGroup,
                                                                  "Alles be&rekenen",
                                                                  "Er zijn geen berekeningen om uit te voeren.",
                                                                  RiskeerCommonFormsResources.CalculateAllIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearAllIndexRootGroup,
                                                                  "&Wis alle uitvoer...",
                                                                  "Er zijn geen berekeningen met uitvoer om te wissen.",
                                                                  RiskeerCommonFormsResources.ClearIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearIllustrationPointsIndexRootGroup,
                                                                  "Wis alle &illustratiepunten...",
                                                                  "Er zijn geen berekeningen met illustratiepunten om te wissen.",
                                                                  RiskeerCommonFormsResources.ClearIllustrationPointsIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_WithoutParentNodeWithStructuresImported_GenerateItemEnabledWithTooltip()
        {
            // Setup
            var group = new CalculationGroup();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            failureMechanism.StabilityPointStructures.AddRange(new[]
            {
                new TestStabilityPointStructure()
            }, "path");
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();
            var groupContext = new StabilityPointStructuresCalculationGroupContext(group,
                                                                                   null,
                                                                                   failureMechanism,
                                                                                   assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Get(groupContext, treeViewControl).Returns(menuBuilder);
                gui.ViewCommands.Returns(Substitute.For<IViewCommands>());
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(groupContext, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuGenerateCalculationsIndexRootGroup,
                                                                  "Genereer &berekeningen...",
                                                                  "Genereer berekeningen op basis van geselecteerde kunstwerken.",
                                                                  RiskeerCommonFormsResources.GenerateScenariosIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_AllRequiredInputSet_ContextMenuItemCalculateAllAndValidateAllEnabled()
        {
            // Setup
            var group = new CalculationGroup
            {
                Children =
                {
                    new StructuresCalculationScenario<StabilityPointStructuresInput>()
                }
            };

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new StructuresCalculationScenario<StabilityPointStructuresInput>());

            var assessmentSection = Substitute.For<IAssessmentSection>();

            var nodeData = new StabilityPointStructuresCalculationGroupContext(group,
                                                                               null,
                                                                               failureMechanism,
                                                                               assessmentSection);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
                gui.ViewCommands.Returns(Substitute.For<IViewCommands>());
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndexRootGroup,
                                                                  "Alles be&rekenen",
                                                                  "Voer alle berekeningen binnen deze map met berekeningen uit.",
                                                                  RiskeerCommonFormsResources.CalculateAllIcon);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndexRootGroup,
                                                                  "Alles &valideren",
                                                                  "Valideer alle berekeningen binnen deze map met berekeningen.",
                                                                  RiskeerCommonFormsResources.ValidateAllIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_NestedCalculationGroup_CallsContextMenuBuilderMethods()
        {
            // Setup
            var group = new CalculationGroup();
            var parentGroup = new CalculationGroup();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var groupContext = new StabilityPointStructuresCalculationGroupContext(group,
                                                                                   parentGroup,
                                                                                   failureMechanism,
                                                                                   assessmentSection);
            var parentGroupContext = new StabilityPointStructuresCalculationGroupContext(parentGroup,
                                                                                         null,
                                                                                         failureMechanism,
                                                                                         assessmentSection);

            var menuBuilder = Substitute.For<IContextMenuBuilder>();
            menuBuilder.AddImportItem().Returns(menuBuilder);
            menuBuilder.AddExportItem().Returns(menuBuilder);
            menuBuilder.AddSeparator().Returns(menuBuilder);
            menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>()).Returns(menuBuilder);
            menuBuilder.AddRenameItem().Returns(menuBuilder);
            menuBuilder.AddDeleteItem().Returns(menuBuilder);
            menuBuilder.AddCollapseAllItem().Returns(menuBuilder);
            menuBuilder.AddExpandAllItem().Returns(menuBuilder);
            menuBuilder.AddPropertiesItem().Returns(menuBuilder);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Get(groupContext, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                // Call
                info.ContextMenuStrip(groupContext, parentGroupContext, treeViewControl);
            }

            // Assert
            Received.InOrder(() =>
            {
                menuBuilder.AddImportItem();
                menuBuilder.AddExportItem();
                menuBuilder.AddSeparator();
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                menuBuilder.AddSeparator();
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                menuBuilder.AddSeparator();
                menuBuilder.AddRenameItem();
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                menuBuilder.AddSeparator();
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                menuBuilder.AddSeparator();
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                menuBuilder.AddDeleteItem();
                menuBuilder.AddSeparator();
                menuBuilder.AddCollapseAllItem();
                menuBuilder.AddExpandAllItem();
                menuBuilder.AddSeparator();
                menuBuilder.AddPropertiesItem();
                menuBuilder.Build();
            });
        }

        [Test]
        public void ContextMenuStrip_NestedCalculationGroup_AddCustomItems()
        {
            // Setup
            var group = new CalculationGroup();
            var parentGroup = new CalculationGroup();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var groupContext = new StabilityPointStructuresCalculationGroupContext(group,
                                                                                   parentGroup,
                                                                                   failureMechanism,
                                                                                   assessmentSection);
            var parentGroupContext = new StabilityPointStructuresCalculationGroupContext(parentGroup,
                                                                                         null,
                                                                                         failureMechanism,
                                                                                         assessmentSection);
            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                gui.Get(groupContext, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(groupContext, parentGroupContext, treeViewControl))
                {
                    // Assert
                    Assert.AreEqual(23, menu.Items.Count);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuDuplicateIndexNestedGroup,
                                                                  "D&upliceren",
                                                                  "Dupliceer dit element.",
                                                                  RiskeerCommonFormsResources.CopyHS);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationGroupIndexNestedGroup,
                                                                  "&Map toevoegen",
                                                                  "Voeg een nieuwe map toe aan deze map met berekeningen.",
                                                                  RiskeerCommonFormsResources.AddFolderIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationIndexNestedGroup,
                                                                  "Berekening &toevoegen",
                                                                  "Voeg een nieuwe berekening toe aan deze map met berekeningen.",
                                                                  RiskeerCommonFormsResources.ProbabilisticCalculationIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuUpdateForeshoreProfileIndexNestedGroup,
                                                                  "&Bijwerken voorlandprofielen...",
                                                                  "Er zijn geen berekeningen om bij te werken.",
                                                                  RiskeerCommonFormsResources.UpdateItemIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuUpdateStructureAllIndexNestedGroup,
                                                                  "Bijwerken &kunstwerken...",
                                                                  "Er zijn geen berekeningen om bij te werken.",
                                                                  RiskeerCommonFormsResources.UpdateItemIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndexNestedGroup,
                                                                  "Alles &valideren",
                                                                  "Er zijn geen berekeningen om te valideren.",
                                                                  RiskeerCommonFormsResources.ValidateAllIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndexNestedGroup,
                                                                  "Alles be&rekenen",
                                                                  "Er zijn geen berekeningen om uit te voeren.",
                                                                  RiskeerCommonFormsResources.CalculateAllIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearAllIndexNestedGroup,
                                                                  "&Wis alle uitvoer...",
                                                                  "Er zijn geen berekeningen met uitvoer om te wissen.",
                                                                  RiskeerCommonFormsResources.ClearIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearIllustrationPointsIndexNestedGroup,
                                                                  "Wis alle &illustratiepunten...",
                                                                  "Er zijn geen berekeningen met illustratiepunten om te wissen.",
                                                                  RiskeerCommonFormsResources.ClearIllustrationPointsIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationGroupWithoutCalculations_ContextMenuItemUpdateStructuresDisabledAndToolTipSet()
        {
            // Setup
            var group = new CalculationGroup();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var assessmentSection = Substitute.For<IAssessmentSection>();
            var nodeData = new StabilityPointStructuresCalculationGroupContext(group,
                                                                               null,
                                                                               failureMechanism,
                                                                               assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Get(nodeData, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuUpdateStructureAllIndexRootGroup,
                                                                  "Bijwerken &kunstwerken...",
                                                                  "Er zijn geen berekeningen om bij te werken.",
                                                                  RiskeerCommonFormsResources.UpdateItemIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationGroupWithCalculationsWithoutStructure_ContextMenuItemUpdateStructuresDisabledAndToolTipSet()
        {
            // Setup
            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>();
            var group = new CalculationGroup
            {
                Children =
                {
                    calculation
                }
            };

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var nodeData = new StabilityPointStructuresCalculationGroupContext(group,
                                                                               null,
                                                                               failureMechanism,
                                                                               assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Get(nodeData, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuUpdateStructureAllIndexRootGroup,
                                                                  "Bijwerken &kunstwerken...",
                                                                  "Er zijn geen berekeningen om bij te werken.",
                                                                  RiskeerCommonFormsResources.UpdateItemIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationGroupWithCalculationWithStructureAndInputInSync_ContextMenuItemUpdateStructuresDisabledAndToolTipSet()
        {
            // Setup
            var calculation = new TestStabilityPointStructuresCalculationScenario();
            var group = new CalculationGroup
            {
                Children =
                {
                    calculation
                }
            };

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var nodeData = new StabilityPointStructuresCalculationGroupContext(group,
                                                                               null,
                                                                               failureMechanism,
                                                                               assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Get(nodeData, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuUpdateStructureAllIndexRootGroup,
                                                                  "Bijwerken &kunstwerken...",
                                                                  "Er zijn geen berekeningen om bij te werken.",
                                                                  RiskeerCommonFormsResources.UpdateItemIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationGroupWithCalculationWithStructureAndInputOutOfSync_ContextMenuItemUpdateStructuresEnabledAndToolTipSet()
        {
            // Setup
            var calculation = new TestStabilityPointStructuresCalculationScenario();
            ChangeStructure(calculation.InputParameters.Structure);
            var group = new CalculationGroup
            {
                Children =
                {
                    calculation
                }
            };

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var nodeData = new StabilityPointStructuresCalculationGroupContext(group,
                                                                               null,
                                                                               failureMechanism,
                                                                               assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Get(nodeData, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuUpdateStructureAllIndexRootGroup,
                                                                  "Bijwerken &kunstwerken...",
                                                                  "Alle berekeningen met een kunstwerk bijwerken.",
                                                                  RiskeerCommonFormsResources.UpdateItemIcon);
                }
            }
        }

        [Test]
        public void GivenCalculationWithoutOutputAndWithInputOutOfSync_WhenUpdateStructuresClicked_ThenNoInquiryAndCalculationUpdatedAndInputObserverNotified()
        {
            // Given
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();
            var structure = new TestStabilityPointStructure();

            var calculation1Observer = Substitute.For<IObserver>();
            var calculation1InputObserver = Substitute.For<IObserver>();
            var calculation2Observer = Substitute.For<IObserver>();
            var calculation2InputObserver = Substitute.For<IObserver>();

            var calculation1 = new StructuresCalculationScenario<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    Structure = structure
                }
            };
            calculation1.Attach(calculation1Observer);
            calculation1.InputParameters.Attach(calculation1InputObserver);

            var calculation2 = new StructuresCalculationScenario<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    Structure = structure
                }
            };
            calculation2.Attach(calculation2Observer);
            calculation2.InputParameters.Attach(calculation2InputObserver);

            var childGroup = new CalculationGroup();
            childGroup.Children.Add(calculation1);
            var emptyChildGroup = new CalculationGroup();
            var group = new CalculationGroup
            {
                Children =
                {
                    childGroup,
                    emptyChildGroup,
                    calculation2
                }
            };

            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var nodeData = new StabilityPointStructuresCalculationGroupContext(group,
                                                                               null,
                                                                               failureMechanism,
                                                                               assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var mainWindow = Substitute.For<IMainWindow>();
                gui.Get(nodeData, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());
                gui.MainWindow.Returns(mainWindow);

                ChangeStructure(structure);

                // Precondition
                Assert.IsFalse(calculation1.InputParameters.IsStructureInputSynchronized);
                Assert.IsFalse(calculation2.InputParameters.IsStructureInputSynchronized);

                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, assessmentSection, treeViewControl))
                {
                    // When
                    menu.Items[contextMenuUpdateStructureAllIndexRootGroup].PerformClick();

                    // Then
                    Assert.IsTrue(calculation1.InputParameters.IsStructureInputSynchronized);
                    Assert.IsTrue(calculation2.InputParameters.IsStructureInputSynchronized);

                    // Note: observer assertions are verified below
                }
            }

            calculation1InputObserver.Received().UpdateObserver();
            calculation2InputObserver.Received().UpdateObserver();
        }

        [Test]
        public void GivenCalculationWithOutputAndInputOutOfSync_WhenUpdateStructuresClickedAndCancelled_ThenInquiryAndCalculationNotUpdatedAndObserversNotNotified()
        {
            // Given
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();
            var structure = new TestStabilityPointStructure();

            var calculation1Observer = Substitute.For<IObserver>();
            var calculation1InputObserver = Substitute.For<IObserver>();
            var calculation2Observer = Substitute.For<IObserver>();
            var calculation2InputObserver = Substitute.For<IObserver>();

            var calculation1 = new StructuresCalculationScenario<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    Structure = structure
                },
                Output = new TestStructuresOutput()
            };
            calculation1.Attach(calculation1Observer);
            calculation1.InputParameters.Attach(calculation1InputObserver);

            var calculation2 = new StructuresCalculationScenario<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    Structure = structure
                },
                Output = new TestStructuresOutput()
            };
            calculation2.Attach(calculation2Observer);
            calculation2.InputParameters.Attach(calculation2InputObserver);

            var childGroup = new CalculationGroup();
            childGroup.Children.Add(calculation1);
            var emptyChildGroup = new CalculationGroup();
            var group = new CalculationGroup
            {
                Children =
                {
                    childGroup,
                    emptyChildGroup,
                    calculation2
                }
            };

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var nodeData = new StabilityPointStructuresCalculationGroupContext(group, null, failureMechanism, assessmentSection);

            string textBoxMessage = null;
            DialogBoxHandler = (name, wnd) =>
            {
                var helper = new MessageBoxTester(wnd);
                textBoxMessage = helper.Text;
                helper.ClickCancel();
            };

            using (var treeViewControl = new TreeViewControl())
            {
                var mainWindow = Substitute.For<IMainWindow>();
                gui.Get(nodeData, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());
                gui.MainWindow.Returns(mainWindow);

                ChangeStructure(structure);

                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, assessmentSection, treeViewControl))
                {
                    // When
                    menu.Items[contextMenuUpdateStructureAllIndexRootGroup].PerformClick();

                    // Then
                    Assert.IsFalse(calculation1.InputParameters.IsStructureInputSynchronized);
                    Assert.IsTrue(calculation1.HasOutput);

                    Assert.IsFalse(calculation2.InputParameters.IsStructureInputSynchronized);
                    Assert.IsTrue(calculation2.HasOutput);

                    string expectedMessage = "Als u kiest voor bijwerken, dan wordt het resultaat van alle bij te werken berekeningen " +
                                             $"verwijderd.{Environment.NewLine}{Environment.NewLine}Weet u zeker dat u wilt doorgaan?";
                    Assert.AreEqual(expectedMessage, textBoxMessage);

                    // Note: observers are not notified
                }
            }
        }

        [Test]
        public void GivenCalculationWithOutputAndInputOutOfSync_WhenUpdateStructuresClickedAndContinued_ThenInquiryAndCalculationUpdatedAndObserversNotified()
        {
            // Given
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();
            var structure = new TestStabilityPointStructure();

            var calculation1Observer = Substitute.For<IObserver>();
            var calculation1InputObserver = Substitute.For<IObserver>();
            var calculation2Observer = Substitute.For<IObserver>();
            var calculation2InputObserver = Substitute.For<IObserver>();

            var calculation1 = new StructuresCalculationScenario<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    Structure = structure
                },
                Output = new TestStructuresOutput()
            };
            calculation1.Attach(calculation1Observer);
            calculation1.InputParameters.Attach(calculation1InputObserver);

            var calculation2 = new StructuresCalculationScenario<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    Structure = structure
                },
                Output = new TestStructuresOutput()
            };
            calculation2.Attach(calculation2Observer);
            calculation2.InputParameters.Attach(calculation2InputObserver);

            var childGroup = new CalculationGroup();
            childGroup.Children.Add(calculation1);
            var emptyChildGroup = new CalculationGroup();
            var group = new CalculationGroup
            {
                Children =
                {
                    childGroup,
                    emptyChildGroup,
                    calculation2
                }
            };

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var nodeData = new StabilityPointStructuresCalculationGroupContext(group, null, failureMechanism, assessmentSection);

            string textBoxMessage = null;
            DialogBoxHandler = (name, wnd) =>
            {
                var helper = new MessageBoxTester(wnd);
                textBoxMessage = helper.Text;
                helper.ClickOk();
            };

            using (var treeViewControl = new TreeViewControl())
            {
                var mainWindow = Substitute.For<IMainWindow>();
                gui.Get(nodeData, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());
                gui.MainWindow.Returns(mainWindow);

                ChangeStructure(structure);

                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, assessmentSection, treeViewControl))
                {
                    // When
                    menu.Items[contextMenuUpdateStructureAllIndexRootGroup].PerformClick();

                    // Then
                    Assert.IsTrue(calculation1.InputParameters.IsStructureInputSynchronized);
                    Assert.IsFalse(calculation1.HasOutput);

                    Assert.IsTrue(calculation2.InputParameters.IsStructureInputSynchronized);
                    Assert.IsFalse(calculation2.HasOutput);

                    string expectedMessage = "Als u kiest voor bijwerken, dan wordt het resultaat van alle bij te werken berekeningen " +
                                             $"verwijderd.{Environment.NewLine}{Environment.NewLine}Weet u zeker dat u wilt doorgaan?";
                    Assert.AreEqual(expectedMessage, textBoxMessage);

                    // Note: observer assertions are verified below
                }
            }

            calculation1Observer.Received().UpdateObserver();
            calculation1InputObserver.Received().UpdateObserver();
            calculation2Observer.Received().UpdateObserver();
            calculation2InputObserver.Received().UpdateObserver();
        }

        [Test]
        public void ContextMenuStrip_CalculationGroupWithCalculationWithForeshoreProfileAndInputOutOfSync_ContextMenuItemUpdateForeshoreProfilesEnabledAndToolTipSet()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile()
                }
            };

            var nodeData = new StabilityPointStructuresCalculationGroupContext(
                new CalculationGroup
                {
                    Children =
                    {
                        calculation
                    }
                },
                null,
                failureMechanism,
                assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Get(nodeData, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                plugin.Gui = gui;

                calculation.InputParameters.UseBreakWater = true;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                  contextMenuUpdateForeshoreProfileIndexRootGroup,
                                                                  "&Bijwerken voorlandprofielen...",
                                                                  "Alle berekeningen met een voorlandprofiel bijwerken.",
                                                                  RiskeerCommonFormsResources.UpdateItemIcon);
                }
            }
        }

        [Test]
        public void GivenCalculationWithoutOutputAndWithInputOutOfSync_WhenUpdateForeshoreProfilesClicked_ThenNoInquiryAndCalculationUpdatedAndInputObserverNotified()
        {
            // Given
            var calculationObserver = Substitute.For<IObserver>();
            var calculationInputObserver = Substitute.For<IObserver>();

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile(true)
                }
            };

            var nodeData = new StabilityPointStructuresCalculationGroupContext(
                new CalculationGroup
                {
                    Children =
                    {
                        calculation
                    }
                },
                null,
                failureMechanism,
                assessmentSection);

            calculation.Attach(calculationObserver);
            calculation.InputParameters.Attach(calculationInputObserver);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Get(nodeData, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                plugin.Gui = gui;

                calculation.InputParameters.UseBreakWater = false;

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    contextMenuStrip.Items[contextMenuUpdateForeshoreProfileIndexRootGroup].PerformClick();

                    // Then
                    Assert.IsTrue(calculation.InputParameters.IsForeshoreProfileInputSynchronized);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationGroupWithCalculationsContainingIllustrationPoints_ContextMenuItemClearIllustrationPointsEnabled()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var calculationWithIllustrationPoints = new TestStabilityPointStructuresCalculationScenario
            {
                Output = new TestStructuresOutput(new TestGeneralResultFaultTreeIllustrationPoint())
            };

            var calculationWithOutput = new TestStabilityPointStructuresCalculationScenario
            {
                Output = new TestStructuresOutput()
            };

            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculationWithIllustrationPoints,
                    calculationWithOutput,
                    new TestStabilityPointStructuresCalculationScenario()
                }
            };

            var nodeData = new StabilityPointStructuresCalculationGroupContext(calculationGroup, null, failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Call
                    ToolStripItem toolStripItem = contextMenu.Items[contextMenuClearIllustrationPointsIndexRootGroup];

                    // Assert
                    Assert.IsTrue(toolStripItem.Enabled);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationGroupWithoutIllustrationPoints_ContextMenuItemClearIllustrationPointsDisabled()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var calculationWithOutput = new TestStabilityPointStructuresCalculationScenario
            {
                Output = new TestStructuresOutput()
            };

            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculationWithOutput,
                    new TestStabilityPointStructuresCalculationScenario()
                }
            };

            var nodeData = new StabilityPointStructuresCalculationGroupContext(calculationGroup, null, failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Call
                    ToolStripItem toolStripItem = contextMenu.Items[contextMenuClearIllustrationPointsIndexRootGroup];

                    // Assert
                    Assert.IsFalse(toolStripItem.Enabled);
                }
            }
        }

        [Test]
        public void GivenCalculationsWithIllustrationPoints_WhenClearIllustrationPointsClickedAndAborted_ThenInquiryAndIllustrationPointsNotCleared()
        {
            // Given
            var calculationWithIllustrationPoints = new TestStabilityPointStructuresCalculationScenario
            {
                Output = new TestStructuresOutput(new TestGeneralResultFaultTreeIllustrationPoint())
            };

            var calculationWithOutput = new TestStabilityPointStructuresCalculationScenario
            {
                Output = new TestStructuresOutput()
            };

            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculationWithIllustrationPoints,
                    calculationWithOutput,
                    new TestStabilityPointStructuresCalculationScenario()
                }
            };

            var calculationObserver = Substitute.For<IObserver>();
            calculationWithIllustrationPoints.Attach(calculationObserver);

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var nodeData = new StabilityPointStructuresCalculationGroupContext(calculationGroup, null, failureMechanism, assessmentSection);

            var messageBoxText = "";
            DialogBoxHandler = (name, wnd) =>
            {
                var helper = new MessageBoxTester(wnd);
                messageBoxText = helper.Text;

                helper.ClickCancel();
            };

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Get(nodeData, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    contextMenuStrip.Items[contextMenuClearIllustrationPointsIndexRootGroup].PerformClick();

                    // Then
                    Assert.AreEqual("Weet u zeker dat u alle illustratiepunten wilt wissen?", messageBoxText);

                    Assert.IsTrue(calculationWithOutput.HasOutput);
                    Assert.IsTrue(calculationWithIllustrationPoints.Output.HasGeneralResult);
                }
            }
        }

        [Test]
        public void GivenCalculationsWithIllustrationPoints_WhenClearIllustrationPointsClickedAndContinued_ThenInquiryAndIllustrationPointsCleared()
        {
            // Given
            var calculationWithIllustrationPoints = new TestStabilityPointStructuresCalculationScenario
            {
                Output = new TestStructuresOutput(new TestGeneralResultFaultTreeIllustrationPoint())
            };

            var calculationWithOutput = new TestStabilityPointStructuresCalculationScenario
            {
                Output = new TestStructuresOutput()
            };

            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculationWithIllustrationPoints,
                    calculationWithOutput,
                    new TestStabilityPointStructuresCalculationScenario()
                }
            };

            var affectedCalculationObserver = Substitute.For<IObserver>();
            calculationWithIllustrationPoints.Attach(affectedCalculationObserver);

            var unaffectedCalculationObserver = Substitute.For<IObserver>();
            calculationWithOutput.Attach(unaffectedCalculationObserver);

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var nodeData = new StabilityPointStructuresCalculationGroupContext(calculationGroup, null, failureMechanism, assessmentSection);

            var messageBoxText = "";
            DialogBoxHandler = (name, wnd) =>
            {
                var helper = new MessageBoxTester(wnd);
                messageBoxText = helper.Text;

                helper.ClickOk();
            };

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Get(nodeData, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    contextMenuStrip.Items[contextMenuClearIllustrationPointsIndexRootGroup].PerformClick();

                    // Then
                    Assert.AreEqual("Weet u zeker dat u alle illustratiepunten wilt wissen?", messageBoxText);

                    Assert.IsTrue(calculationWithOutput.HasOutput);
                    Assert.IsFalse(calculationWithIllustrationPoints.Output.HasGeneralResult);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_ClickOnAddGroupItem_AddGroupToCalculationGroupAndNotifyObservers()
        {
            // Setup
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            var group = new CalculationGroup();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();
            var nodeData = new StabilityPointStructuresCalculationGroupContext(group,
                                                                               null,
                                                                               failureMechanism,
                                                                               assessmentSection);
            var calculationGroup = new CalculationGroup
            {
                Name = "Nieuwe map"
            };
            var observer = Substitute.For<IObserver>();

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
                gui.ViewCommands.Returns(Substitute.For<IViewCommands>());
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                group.Children.Add(calculationGroup);
                nodeData.Attach(observer);

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Precondition
                    Assert.AreEqual(1, group.Children.Count);

                    // Call
                    contextMenu.Items[contextMenuAddCalculationGroupIndexRootGroup].PerformClick();

                    // Assert
                    Assert.AreEqual(2, group.Children.Count);
                    ICalculationBase newlyAddedItem = group.Children.Last();
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
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            var group = new CalculationGroup();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();
            var nodeData = new StabilityPointStructuresCalculationGroupContext(group,
                                                                               null,
                                                                               failureMechanism,
                                                                               assessmentSection);
            var calculationItem = new StructuresCalculationScenario<StabilityPointStructuresInput>
            {
                Name = "Nieuwe berekening"
            };
            var observer = Substitute.For<IObserver>();

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
                gui.ViewCommands.Returns(Substitute.For<IViewCommands>());
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                group.Children.Add(calculationItem);
                nodeData.Attach(observer);

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Precondition
                    Assert.AreEqual(1, group.Children.Count);

                    // Call
                    contextMenu.Items[contextMenuAddCalculationIndexRootGroup].PerformClick();

                    // Assert
                    Assert.AreEqual(2, group.Children.Count);
                    ICalculationBase newlyAddedItem = group.Children.Last();
                    Assert.IsInstanceOf<StructuresCalculationScenario<StabilityPointStructuresInput>>(newlyAddedItem);
                    Assert.AreEqual("Nieuwe berekening (1)", newlyAddedItem.Name,
                                    "An item with the same name default name already exists, therefore '(1)' needs to be appended.");
                }
            }
        }

        [Test]
        public void ContextMenuStrip_ClickOnCalculateAllItem_ScheduleAllChildCalculations()
        {
            // Setup
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            var failureMechanism = new StabilityPointStructuresFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        new TestStabilityPointStructuresCalculationScenario
                        {
                            Name = "A",
                            InputParameters =
                            {
                                HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                                InflowModelType = StabilityPointStructureInflowModelType.LowSill,
                                LoadSchematizationType = LoadSchematizationType.Linear
                            }
                        },
                        new TestStabilityPointStructuresCalculationScenario
                        {
                            Name = "B",
                            InputParameters =
                            {
                                HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                                InflowModelType = StabilityPointStructureInflowModelType.LowSill,
                                LoadSchematizationType = LoadSchematizationType.Linear
                            }
                        }
                    }
                }
            };

            var hydraulicBoundaryData = new HydraulicBoundaryData
            {
                HydraulicLocationConfigurationDatabase =
                {
                    FilePath = validHlcdFilePath
                },
                HydraulicBoundaryDatabases =
                {
                    new HydraulicBoundaryDatabase
                    {
                        FilePath = validHrdFilePath,
                        Version = validHrdFileVersion,
                        Locations =
                        {
                            hydraulicBoundaryLocation
                        }
                    }
                }
            };

            var assessmentSection = Substitute.For<IAssessmentSection>();
            assessmentSection.Id.Returns(string.Empty);
            assessmentSection.FailureMechanismContribution.Returns(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());
            assessmentSection.HydraulicBoundaryData.Returns(hydraulicBoundaryData);

            var groupContext = new StabilityPointStructuresCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                                   null,
                                                                                   failureMechanism,
                                                                                   assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                IMainWindow mainWindow = MainWindowTestHelper.CreateMainWindowStub();

                gui.Get(groupContext, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(mainWindow);
                gui.ViewCommands.Returns(Substitute.For<IViewCommands>());

                int nrOfCalculators = failureMechanism.Calculations.Count();
                var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();
                calculatorFactory.CreateStructuresCalculator<StructuresStabilityPointCalculationInput>(
                    Arg.Any<HydraRingCalculationSettings>()).Returns(new TestStructuresCalculator<StructuresStabilityPointCalculationInput>());
                calculatorFactory.When(x => x.CreateStructuresCalculator<StructuresStabilityPointCalculationInput>(
                                           Arg.Any<HydraRingCalculationSettings>())).Do(invocation =>
                {
                    HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                        HydraulicBoundaryCalculationSettingsFactory.CreateSettings(hydraulicBoundaryData,
                                                                                   hydraulicBoundaryLocation),
                        (HydraRingCalculationSettings) invocation[0]);
                });

                plugin.Gui = gui;

                DialogBoxHandler = (name, wnd) =>
                {
                    // Expect an activity dialog which is automatically closed
                };

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(groupContext, null, treeViewControl))
                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                {
                    // Call
                    TestHelper.AssertLogMessages(() => contextMenu.Items[contextMenuCalculateAllIndexRootGroup].PerformClick(), messages =>
                    {
                        List<string> messageList = messages.ToList();

                        // Assert
                        Assert.AreEqual(14, messageList.Count);
                        Assert.AreEqual("Uitvoeren van berekening 'A' is gestart.", messageList[0]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(messageList[1]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(messageList[2]);
                        CalculationServiceTestHelper.AssertCalculationStartMessage(messageList[3]);
                        StringAssert.StartsWith("Puntconstructies berekening is uitgevoerd op de tijdelijke locatie", messageList[4]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(messageList[5]);
                        Assert.AreEqual("Uitvoeren van berekening 'A' is gelukt.", messageList[6]);

                        Assert.AreEqual("Uitvoeren van berekening 'B' is gestart.", messageList[7]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(messageList[8]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(messageList[9]);
                        CalculationServiceTestHelper.AssertCalculationStartMessage(messageList[10]);
                        StringAssert.StartsWith("Puntconstructies berekening is uitgevoerd op de tijdelijke locatie", messageList[11]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(messageList[12]);
                        Assert.AreEqual("Uitvoeren van berekening 'B' is gelukt.", messageList[13]);
                    });

                    calculatorFactory.Received(nrOfCalculators).CreateStructuresCalculator<StructuresStabilityPointCalculationInput>(
                        Arg.Any<HydraRingCalculationSettings>());
                }
            }
        }

        [Test]
        public void ContextMenuStrip_ClickOnValidateAllItem_ScheduleAllChildCalculations()
        {
            // Setup
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            var failureMechanism = new StabilityPointStructuresFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        new TestStabilityPointStructuresCalculationScenario
                        {
                            Name = "A",
                            InputParameters =
                            {
                                HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                                InflowModelType = StabilityPointStructureInflowModelType.LowSill,
                                LoadSchematizationType = LoadSchematizationType.Linear
                            }
                        },
                        new TestStabilityPointStructuresCalculationScenario
                        {
                            Name = "B",
                            InputParameters =
                            {
                                HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                                InflowModelType = StabilityPointStructureInflowModelType.LowSill,
                                LoadSchematizationType = LoadSchematizationType.Linear
                            }
                        }
                    }
                }
            };

            var hydraulicBoundaryData = new HydraulicBoundaryData
            {
                HydraulicLocationConfigurationDatabase =
                {
                    FilePath = validHlcdFilePath
                },
                HydraulicBoundaryDatabases =
                {
                    new HydraulicBoundaryDatabase
                    {
                        FilePath = validHrdFilePath,
                        Version = validHrdFileVersion,
                        Locations =
                        {
                            hydraulicBoundaryLocation
                        }
                    }
                }
            };

            var assessmentSection = Substitute.For<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryData.Returns(hydraulicBoundaryData);

            var groupContext = new StabilityPointStructuresCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                                   null,
                                                                                   failureMechanism,
                                                                                   assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Get(groupContext, treeViewControl).Returns(menuBuilder);
                gui.ViewCommands.Returns(Substitute.For<IViewCommands>());
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(groupContext, null, treeViewControl))
                {
                    // Call
                    TestHelper.AssertLogMessages(() => contextMenu.Items[contextMenuValidateAllIndexRootGroup].PerformClick(), messages =>
                    {
                        List<string> messageList = messages.ToList();

                        // Assert
                        Assert.AreEqual(4, messageList.Count);
                        CalculationServiceTestHelper.AssertValidationStartMessage(messageList[0]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(messageList[1]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(messageList[2]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(messageList[3]);
                    });
                }
            }
        }

        [Test]
        public void GivenCalculationsViewGenerateScenariosButtonClicked_WhenStabilityPointStructureSelectedAndDialogClosed_ThenCalculationsAddedWithStabilityPointStructureAssigned()
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();

                StabilityPointStructure structure1 = new TestStabilityPointStructure("id structure1");
                StabilityPointStructure structure2 = new TestStabilityPointStructure("id structure2");

                var existingCalculationGroup = new CalculationGroup();
                var existingCalculation = new StructuresCalculationScenario<StabilityPointStructuresInput>();
                var failureMechanism = new StabilityPointStructuresFailureMechanism
                {
                    CalculationsGroup =
                    {
                        Children =
                        {
                            existingCalculationGroup,
                            existingCalculation
                        }
                    }
                };
                failureMechanism.StabilityPointStructures.AddRange(new[]
                {
                    structure1,
                    structure2
                }, "path");

                var nodeData = new StabilityPointStructuresCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                                   null,
                                                                                   failureMechanism,
                                                                                   assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var mainWindow = Substitute.For<IMainWindow>();

                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(mainWindow);
                gui.ViewCommands.Returns(Substitute.For<IViewCommands>());
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                plugin.Gui = gui;

                DialogBoxHandler = (name, wnd) =>
                {
                    var selectionDialog = (StructureSelectionDialog) new FormTester(name).TheObject;
                    var grid = (DataGridViewControl) new ControlTester("DataGridViewControl", selectionDialog).TheObject;

                    grid.Rows[0].Cells[0].Value = true;

                    new ButtonTester("DoForSelectedButton", selectionDialog).Click();
                };

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    contextMenu.Items[contextMenuGenerateCalculationsIndexRootGroup].PerformClick();

                    // Then
                    Assert.AreEqual(3, failureMechanism.CalculationsGroup.Children.Count);
                    Assert.AreSame(existingCalculationGroup, failureMechanism.CalculationsGroup.Children[0]);
                    Assert.AreSame(existingCalculation, failureMechanism.CalculationsGroup.Children[1]);
                    var generatedCalculation = failureMechanism.CalculationsGroup.Children[2] as StructuresCalculationScenario<StabilityPointStructuresInput>;
                    Assert.IsNotNull(generatedCalculation);
                    Assert.AreSame(structure1, generatedCalculation.InputParameters.Structure);
                }
            }
        }

        [Test]
        public void GivenCalculationsViewGenerateScenariosButtonClicked_WhenCancelButtonClickedAndDialogClosed_ThenCalculationsNotUpdated()
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();

                StabilityPointStructure structure1 = new TestStabilityPointStructure("id structure1");
                StabilityPointStructure structure2 = new TestStabilityPointStructure("id structure2");

                var failureMechanism = new StabilityPointStructuresFailureMechanism();
                failureMechanism.StabilityPointStructures.AddRange(new[]
                {
                    structure1,
                    structure2
                }, "path");

                var nodeData = new StabilityPointStructuresCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                                   null,
                                                                                   failureMechanism,
                                                                                   assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var mainWindow = Substitute.For<IMainWindow>();

                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(mainWindow);
                gui.ViewCommands.Returns(Substitute.For<IViewCommands>());
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                plugin.Gui = gui;

                DialogBoxHandler = (name, wnd) =>
                {
                    var selectionDialog = (StructureSelectionDialog) new FormTester(name).TheObject;
                    var grid = (DataGridViewControl) new ControlTester("DataGridViewControl", selectionDialog).TheObject;

                    grid.Rows[0].Cells[0].Value = true;

                    new ButtonTester("CustomCancelButton", selectionDialog).Click();
                };

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    contextMenu.Items[contextMenuGenerateCalculationsIndexRootGroup].PerformClick();

                    // Then
                    Assert.AreEqual(0, failureMechanism.Calculations.OfType<StructuresCalculationScenario<StabilityPointStructuresInput>>().Count());
                }
            }
        }

        [Test]
        public void GivenScenariosWithExistingCalculationWithSameName_WhenOkButtonClickedAndDialogClosed_ThenCalculationWithUniqueNameAdded()
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();

                const string existingCalculationName = "StabilityPoint structure";
                StabilityPointStructure stabilityPointStructure = new TestStabilityPointStructure("id", existingCalculationName);

                var failureMechanism = new StabilityPointStructuresFailureMechanism
                {
                    CalculationsGroup =
                    {
                        Children =
                        {
                            new StructuresCalculationScenario<StabilityPointStructuresInput>
                            {
                                Name = existingCalculationName
                            }
                        }
                    }
                };
                failureMechanism.StabilityPointStructures.AddRange(new[]
                {
                    stabilityPointStructure
                }, "path");

                var nodeData = new StabilityPointStructuresCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                                   null,
                                                                                   failureMechanism,
                                                                                   assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var mainWindow = Substitute.For<IMainWindow>();

                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(mainWindow);
                gui.ViewCommands.Returns(Substitute.For<IViewCommands>());
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                plugin.Gui = gui;

                DialogBoxHandler = (name, wnd) =>
                {
                    var selectionDialog = (StructureSelectionDialog) new FormTester(name).TheObject;
                    var grid = (DataGridViewControl) new ControlTester("DataGridViewControl", selectionDialog).TheObject;

                    grid.Rows[0].Cells[0].Value = true;

                    new ButtonTester("DoForSelectedButton", selectionDialog).Click();
                };

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    string expectedNewName = NamingHelper.GetUniqueName(failureMechanism.CalculationsGroup.Children, existingCalculationName, c => c.Name);

                    // When
                    contextMenu.Items[contextMenuGenerateCalculationsIndexRootGroup].PerformClick();

                    // Then
                    StructuresCalculationScenario<StabilityPointStructuresInput>[] stabilityPointStructuresCalculations = failureMechanism.Calculations.OfType<StructuresCalculationScenario<StabilityPointStructuresInput>>().ToArray();
                    Assert.AreEqual(2, stabilityPointStructuresCalculations.Length);
                    Assert.AreEqual(expectedNewName, stabilityPointStructuresCalculations[1].Name);
                }
            }
        }

        [Test]
        public void OnNodeRemoved_NestedCalculationGroup_RemoveGroupAndNotifyObservers()
        {
            // Setup
            var observer = Substitute.For<IObserver>();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var group = new CalculationGroup();
            var parentGroup = new CalculationGroup();
            var nodeData = new StabilityPointStructuresCalculationGroupContext(group,
                                                                               parentGroup,
                                                                               failureMechanism,
                                                                               assessmentSection);
            var parentNodeData = new StabilityPointStructuresCalculationGroupContext(parentGroup,
                                                                                     null,
                                                                                     failureMechanism,
                                                                                     assessmentSection);

            parentGroup.Children.Add(group);
            parentNodeData.Attach(observer);

            // Precondition
            Assert.IsTrue(info.CanRemove(nodeData, parentNodeData));

            // Call
            info.OnNodeRemoved(nodeData, parentNodeData);

            // Assert
            CollectionAssert.DoesNotContain(parentGroup.Children, group);
        }

        [Test]
        public void OnNodeRemoved_NestedCalculationGroupContainingCalculations_RemoveGroupAndCalculationsAndNotifyObservers()
        {
            // Setup
            var observer = Substitute.For<IObserver>();
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var group = new CalculationGroup();
            var parentGroup = new CalculationGroup();
            var nodeData = new StabilityPointStructuresCalculationGroupContext(group,
                                                                               parentGroup,
                                                                               failureMechanism,
                                                                               assessmentSection);
            var parentNodeData = new StabilityPointStructuresCalculationGroupContext(parentGroup,
                                                                                     null,
                                                                                     failureMechanism,
                                                                                     assessmentSection);
            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>();

            group.Children.Add(calculation);
            parentGroup.Children.Add(group);
            parentNodeData.Attach(observer);

            // Precondition
            Assert.IsTrue(info.CanRemove(nodeData, parentNodeData));

            // Call
            info.OnNodeRemoved(nodeData, parentNodeData);

            // Assert
            CollectionAssert.DoesNotContain(parentGroup.Children, group);
        }

        public override void Setup()
        {
            gui = Substitute.For<IGui>();
            plugin = new StabilityPointStructuresPlugin
            {
                Gui = gui
            };

            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(StabilityPointStructuresCalculationGroupContext));
        }

        public override void TearDown()
        {
            plugin.Dispose();

            base.TearDown();
        }

        private static void ChangeStructure(StabilityPointStructure structure)
        {
            structure.CopyProperties(new StabilityPointStructure(
                                         new StabilityPointStructure.ConstructionProperties
                                         {
                                             Id = structure.Id,
                                             Name = structure.Name,
                                             Location = structure.Location
                                         }));
        }
    }
}