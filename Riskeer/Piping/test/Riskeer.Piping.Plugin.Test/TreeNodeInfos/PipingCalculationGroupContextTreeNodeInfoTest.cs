// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Service.TestUtil;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Forms;
using Riskeer.Piping.Forms.PresentationObjects;
using Riskeer.Piping.Primitives;
using Riskeer.Piping.Primitives.TestUtil;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Riskeer.Piping.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class PipingCalculationGroupContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuImportCalculationGroupIndexRootGroup = 2;
        private const int contextMenuExportCalculationGroupIndexRootGroup = 3;
        private const int contextMenuAddCalculationGroupIndexRootGroup = 7;
        private const int contextMenuAddCalculationIndexRootGroup = 8;
        private const int contextMenuUpdateEntryAndExitPointsAllIndexRootGroup = 10;
        private const int contextMenuValidateAllIndexRootGroup = 12;
        private const int contextMenuCalculateAllIndexRootGroup = 13;
        private const int contextMenuClearOutputIndexRootGroup = 15;
        private const int contextMenuCollapseAllIndexRootGroup = 18;
        private const int contextMenuExpandAllIndexRootGroup = 19;
        private const int contextMenuPropertiesIndexRootGroup = 21;

        private const int contextMenuImportCalculationGroupIndexNestedGroup = 0;
        private const int contextMenuExportCalculationGroupIndexNestedGroup = 1;
        private const int contextMenuDuplicateIndexNestedGroup = 3;
        private const int contextMenuAddCalculationGroupIndexNestedGroup = 5;
        private const int contextMenuAddCalculationIndexNestedGroup = 6;
        private const int contextMenuRenameCalculationGroupIndexNestedGroup = 8;
        private const int contextMenuUpdateEntryAndExitPointsAllIndexNestedGroup = 9;
        private const int contextMenuValidateAllIndexNestedGroup = 11;
        private const int contextMenuCalculateAllIndexNestedGroup = 12;
        private const int contextMenuClearOutputIndexNestedGroup = 14;
        private const int contextMenuDeleteCalculationGroupIndexNestedGroup = 15;
        private const int contextMenuCollapseAllIndexNestedGroup = 17;
        private const int contextMenuExpandAllIndexNestedGroup = 18;
        private const int contextMenuPropertiesIndexNestedGroup = 20;

        private const int customOnlyContextMenuAddGenerateCalculationsIndex = 5;

        private MockRepository mocks;
        private PipingPlugin plugin;
        private TreeNodeInfo info;

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            mocks.ReplayAll();

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
            var pipingFailureMechanism = new PipingFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var groupContext = new PipingCalculationGroupContext(group,
                                                                 null,
                                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                 pipingFailureMechanism,
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
            var calculationItem = mocks.StrictMock<ICalculationBase>();

            var childCalculation = new PipingCalculationScenario(new GeneralPipingInput());

            var childGroup = new CalculationGroup();

            var group = new CalculationGroup();
            group.Children.Add(calculationItem);
            group.Children.Add(childCalculation);
            group.Children.Add(childGroup);

            var pipingFailureMechanism = new PipingFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            mocks.ReplayAll();

            var nodeData = new PipingCalculationGroupContext(group,
                                                             null,
                                                             Enumerable.Empty<PipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingStochasticSoilModel>(),
                                                             pipingFailureMechanism,
                                                             assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(nodeData).ToArray();

            // Assert
            Assert.AreEqual(group.Children.Count, children.Length);
            Assert.AreSame(calculationItem, children[0]);
            var returnedCalculationContext = (PipingCalculationScenarioContext) children[1];
            Assert.AreSame(childCalculation, returnedCalculationContext.WrappedData);
            Assert.AreSame(group, returnedCalculationContext.Parent);
            Assert.AreSame(pipingFailureMechanism, returnedCalculationContext.FailureMechanism);
            var returnedCalculationGroupContext = (PipingCalculationGroupContext) children[2];
            Assert.AreSame(childGroup, returnedCalculationGroupContext.WrappedData);
            Assert.AreSame(group, returnedCalculationGroupContext.Parent);
            Assert.AreSame(pipingFailureMechanism, returnedCalculationGroupContext.FailureMechanism);
            Assert.AreSame(assessmentSection, returnedCalculationGroupContext.AssessmentSection);
        }

        [Test]
        public void ContextMenuStrip_NestedCalculationGroupWithCalculationOutput_ReturnContextMenuWithItems()
        {
            // Setup
            var group = new CalculationGroup();
            var parentGroup = new CalculationGroup();

            group.Children.Add(new PipingCalculationScenario(new GeneralPipingInput())
            {
                Output = PipingOutputTestFactory.Create()
            });

            var pipingFailureMechanism = new TestPipingFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var nodeData = new PipingCalculationGroupContext(group,
                                                             parentGroup,
                                                             Enumerable.Empty<PipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingStochasticSoilModel>(),
                                                             pipingFailureMechanism,
                                                             assessmentSection);
            var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                                   null,
                                                                   Enumerable.Empty<PipingSurfaceLine>(),
                                                                   Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                   pipingFailureMechanism,
                                                                   assessmentSection);

            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var importHandler = mocks.StrictMock<IImportCommandHandler>();
            importHandler.Expect(ihm => ihm.CanImportOn(nodeData)).Return(true);
            var exportHandler = mocks.StrictMock<IExportCommandHandler>();
            exportHandler.Expect(ehm => ehm.CanExportFrom(nodeData)).Return(true);
            var updateHandler = mocks.Stub<IUpdateCommandHandler>();
            var viewCommandsHandler = mocks.Stub<IViewCommands>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();

            var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler,
                                                     importHandler,
                                                     exportHandler,
                                                     updateHandler,
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
                Assert.AreEqual(21, menu.Items.Count);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuImportCalculationGroupIndexNestedGroup,
                                                              "&Importeren...",
                                                              "Importeer de gegevens vanuit een bestand.",
                                                              CoreCommonGuiResources.ImportIcon);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuExportCalculationGroupIndexNestedGroup,
                                                              "&Exporteren...",
                                                              "Exporteer de gegevens naar een bestand.",
                                                              CoreCommonGuiResources.ExportIcon);
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
                                                              RiskeerCommonFormsResources.CalculationIcon);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuRenameCalculationGroupIndexNestedGroup,
                                                              "&Hernoemen",
                                                              "Wijzig de naam van dit element.",
                                                              CoreCommonGuiResources.RenameIcon);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuUpdateEntryAndExitPointsAllIndexNestedGroup,
                                                              "&Bijwerken intrede- en uittredepunten...",
                                                              "Er zijn geen berekeningen om bij te werken.",
                                                              RiskeerCommonFormsResources.UpdateItemIcon,
                                                              false);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndexNestedGroup,
                                                              "Alles &valideren",
                                                              "Valideer alle berekeningen binnen deze map met berekeningen.",
                                                              RiskeerCommonFormsResources.ValidateAllIcon);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndexNestedGroup,
                                                              "Alles be&rekenen",
                                                              "Voer alle berekeningen binnen deze map met berekeningen uit.",
                                                              RiskeerCommonFormsResources.CalculateAllIcon);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearOutputIndexNestedGroup,
                                                              "&Wis alle uitvoer...",
                                                              "Wis de uitvoer van alle berekeningen binnen deze map met berekeningen.",
                                                              RiskeerCommonFormsResources.ClearIcon);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuDeleteCalculationGroupIndexNestedGroup,
                                                              "Verwij&deren...",
                                                              "Verwijder dit element uit de boom.",
                                                              CoreCommonGuiResources.DeleteIcon);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCollapseAllIndexNestedGroup,
                                                              "Alles i&nklappen",
                                                              "Klap dit element en alle onderliggende elementen in.",
                                                              CoreCommonGuiResources.CollapseAllIcon,
                                                              false);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuExpandAllIndexNestedGroup,
                                                              "Alles ui&tklappen",
                                                              "Klap dit element en alle onderliggende elementen uit.",
                                                              CoreCommonGuiResources.ExpandAllIcon,
                                                              false);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuPropertiesIndexNestedGroup,
                                                              "Ei&genschappen",
                                                              "Toon de eigenschappen in het Eigenschappenpaneel.",
                                                              CoreCommonGuiResources.PropertiesHS,
                                                              false);

                CollectionAssert.AllItemsAreInstancesOfType(new[]
                {
                    menu.Items[2],
                    menu.Items[4],
                    menu.Items[7],
                    menu.Items[10],
                    menu.Items[13],
                    menu.Items[16],
                    menu.Items[19]
                }, typeof(ToolStripSeparator));
            }
        }

        [Test]
        public void ContextMenuStrip_WithoutParentNodeDefaultBehavior_ReturnContextMenuWithoutRenameRemove()
        {
            // Setup
            var group = new CalculationGroup();

            group.Children.Add(new PipingCalculationScenario(new GeneralPipingInput())
            {
                Output = PipingOutputTestFactory.Create()
            });

            var pipingFailureMechanism = new TestPipingFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var nodeData = new PipingCalculationGroupContext(group,
                                                             null,
                                                             Enumerable.Empty<PipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingStochasticSoilModel>(),
                                                             pipingFailureMechanism,
                                                             assessmentSection);

            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var importHandler = mocks.StrictMock<IImportCommandHandler>();
            importHandler.Expect(ihm => ihm.CanImportOn(nodeData)).Return(true);
            var exportHandler = mocks.StrictMock<IExportCommandHandler>();
            exportHandler.Expect(ehm => ehm.CanExportFrom(nodeData)).Return(true);
            var updateHandler = mocks.Stub<IUpdateCommandHandler>();

            var viewCommandsHandler = mocks.StrictMock<IViewCommands>();
            viewCommandsHandler.Expect(vc => vc.CanOpenViewFor(nodeData)).Return(true);

            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler,
                                                         importHandler,
                                                         exportHandler,
                                                         updateHandler,
                                                         viewCommandsHandler,
                                                         nodeData,
                                                         treeViewControl);

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl);

                // Assert
                Assert.AreEqual(22, menu.Items.Count);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuImportCalculationGroupIndexRootGroup,
                                                              "&Importeren...",
                                                              "Importeer de gegevens vanuit een bestand.",
                                                              CoreCommonGuiResources.ImportIcon);

                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuExportCalculationGroupIndexRootGroup,
                                                              "&Exporteren...",
                                                              "Exporteer de gegevens naar een bestand.",
                                                              CoreCommonGuiResources.ExportIcon);

                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationGroupIndexRootGroup,
                                                              "&Map toevoegen",
                                                              "Voeg een nieuwe map toe aan deze map met berekeningen.",
                                                              RiskeerCommonFormsResources.AddFolderIcon);

                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationIndexRootGroup,
                                                              "Berekening &toevoegen",
                                                              "Voeg een nieuwe berekening toe aan deze map met berekeningen.",
                                                              RiskeerCommonFormsResources.CalculationIcon);

                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuUpdateEntryAndExitPointsAllIndexRootGroup,
                                                              "&Bijwerken intrede- en uittredepunten...",
                                                              "Er zijn geen berekeningen om bij te werken.",
                                                              RiskeerCommonFormsResources.UpdateItemIcon,
                                                              false);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndexRootGroup,
                                                              "Alles &valideren",
                                                              "Valideer alle berekeningen binnen deze map met berekeningen.",
                                                              RiskeerCommonFormsResources.ValidateAllIcon);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndexRootGroup,
                                                              "Alles be&rekenen",
                                                              "Voer alle berekeningen binnen deze map met berekeningen uit.",
                                                              RiskeerCommonFormsResources.CalculateAllIcon);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearOutputIndexRootGroup,
                                                              "&Wis alle uitvoer...",
                                                              "Wis de uitvoer van alle berekeningen binnen deze map met berekeningen.",
                                                              RiskeerCommonFormsResources.ClearIcon);

                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuExpandAllIndexRootGroup,
                                                              "Alles ui&tklappen",
                                                              "Klap dit element en alle onderliggende elementen uit.",
                                                              CoreCommonGuiResources.ExpandAllIcon,
                                                              false);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCollapseAllIndexRootGroup,
                                                              "Alles i&nklappen",
                                                              "Klap dit element en alle onderliggende elementen in.",
                                                              CoreCommonGuiResources.CollapseAllIcon,
                                                              false);

                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuPropertiesIndexRootGroup,
                                                              "Ei&genschappen",
                                                              "Toon de eigenschappen in het Eigenschappenpaneel.",
                                                              CoreCommonGuiResources.PropertiesHS,
                                                              false);
                CollectionAssert.AllItemsAreInstancesOfType(new[]
                {
                    menu.Items[1],
                    menu.Items[6],
                    menu.Items[9],
                    menu.Items[11],
                    menu.Items[14],
                    menu.Items[17],
                    menu.Items[20]
                }, typeof(ToolStripSeparator));
            }
        }

        [Test]
        public void ContextMenuStrip_WithoutParentNodeDefaultBehaviorAndWithoutAvailableSurfaceLines_ContextMenuItemGenerateCalculationsDisabled()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var group = new CalculationGroup();

                var pipingFailureMechanism = new PipingFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();

                var nodeData = new PipingCalculationGroupContext(group,
                                                                 null,
                                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                                 new[]
                                                                 {
                                                                     PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel()
                                                                 },
                                                                 pipingFailureMechanism,
                                                                 assessmentSection);

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, customOnlyContextMenuAddGenerateCalculationsIndex,
                                                                  "Genereer &scenario\'s...",
                                                                  "Er zijn geen profielschematisaties of stochastische ondergrondmodellen beschikbaar om berekeningen voor te genereren.",
                                                                  RiskeerCommonFormsResources.GenerateScenariosIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_WithoutParentNodeDefaultBehaviorAndWithoutAvailableSoilModels_ContextMenuItemGenerateCalculationsDisabled()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var group = new CalculationGroup();

                var pipingFailureMechanism = new PipingFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();

                var nodeData = new PipingCalculationGroupContext(group,
                                                                 null,
                                                                 new[]
                                                                 {
                                                                     new PipingSurfaceLine(string.Empty)
                                                                 },
                                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                 pipingFailureMechanism,
                                                                 assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, customOnlyContextMenuAddGenerateCalculationsIndex,
                                                                  "Genereer &scenario\'s...",
                                                                  "Er zijn geen profielschematisaties of stochastische ondergrondmodellen beschikbaar om berekeningen voor te genereren.",
                                                                  RiskeerCommonFormsResources.GenerateScenariosIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_WithoutParentNodeDefaultBehaviorAndWithAvailableSurfaceLinesAndSoilModels_ContextMenuItemGenerateCalculationsEnabled()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var group = new CalculationGroup();

                var pipingFailureMechanism = new PipingFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();

                var nodeData = new PipingCalculationGroupContext(group,
                                                                 null,
                                                                 new[]
                                                                 {
                                                                     new PipingSurfaceLine(string.Empty)
                                                                 },
                                                                 new[]
                                                                 {
                                                                     PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel()
                                                                 },
                                                                 pipingFailureMechanism,
                                                                 assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, customOnlyContextMenuAddGenerateCalculationsIndex,
                                                                  "Genereer &scenario\'s...",
                                                                  "Genereer scenario\'s op basis van geselecteerde profielschematisaties.",
                                                                  RiskeerCommonFormsResources.GenerateScenariosIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_AllRequiredInputSet_ContextMenuItemCalculateAllAndValidateAllEnabled()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var pipingFailureMechanism = new TestPipingFailureMechanism();
                var assessmentSection = new AssessmentSectionStub();
                var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

                assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
                {
                    hydraulicBoundaryLocation
                }, true);

                var group = new CalculationGroup
                {
                    Children =
                    {
                        PipingCalculationScenarioTestFactory.CreatePipingCalculationScenarioWithValidInput(hydraulicBoundaryLocation)
                    }
                };

                var nodeData = new PipingCalculationGroupContext(group,
                                                                 null,
                                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                 pipingFailureMechanism,
                                                                 assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, pipingFailureMechanism, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuCalculateAllIndexRootGroup,
                                                                  "Alles be&rekenen",
                                                                  "Voer alle berekeningen binnen deze map met berekeningen uit.",
                                                                  RiskeerCommonFormsResources.CalculateAllIcon);

                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuValidateAllIndexRootGroup,
                                                                  "Alles &valideren",
                                                                  "Valideer alle berekeningen binnen deze map met berekeningen.",
                                                                  RiskeerCommonFormsResources.ValidateAllIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationGroupWithoutCalculations_ContextMenuItemUpdateEntryAndExitPointsDisabledAndToolTipSet()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var pipingFailureMechanism = new TestPipingFailureMechanism();

                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var group = new CalculationGroup();

                var nodeData = new PipingCalculationGroupContext(group,
                                                                 null,
                                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                 pipingFailureMechanism,
                                                                 assessmentSection);
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  contextMenuUpdateEntryAndExitPointsAllIndexRootGroup,
                                                                  "&Bijwerken intrede- en uittredepunten...",
                                                                  "Er zijn geen berekeningen om bij te werken.",
                                                                  RiskeerCommonFormsResources.UpdateItemIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationGroupWithCalculationsWithoutSurfaceLine_ContextMenuItemUpdateEntryAndExitPointsDisabledAndToolTipSet()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var pipingFailureMechanism = new TestPipingFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var group = new CalculationGroup
                {
                    Children =
                    {
                        new PipingCalculationScenario(new GeneralPipingInput())
                    }
                };

                var nodeData = new PipingCalculationGroupContext(group,
                                                                 null,
                                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                 pipingFailureMechanism,
                                                                 assessmentSection);

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  contextMenuUpdateEntryAndExitPointsAllIndexRootGroup,
                                                                  "&Bijwerken intrede- en uittredepunten...",
                                                                  "Er zijn geen berekeningen om bij te werken.",
                                                                  RiskeerCommonFormsResources.UpdateItemIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationGroupWithCalculationWithSurfaceLineAndInputInSync_ContextMenuItemUpdateEntryAndExitPointsDisabledAndToolTipSet()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var pipingFailureMechanism = new TestPipingFailureMechanism();
                var assessmentSection = new AssessmentSectionStub();
                var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

                assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
                {
                    hydraulicBoundaryLocation
                }, true);

                var group = new CalculationGroup
                {
                    Children =
                    {
                        PipingCalculationScenarioTestFactory.CreatePipingCalculationScenarioWithValidInput(hydraulicBoundaryLocation)
                    }
                };

                var nodeData = new PipingCalculationGroupContext(group,
                                                                 null,
                                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                 pipingFailureMechanism,
                                                                 assessmentSection);

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  contextMenuUpdateEntryAndExitPointsAllIndexRootGroup,
                                                                  "&Bijwerken intrede- en uittredepunten...",
                                                                  "Er zijn geen berekeningen om bij te werken.",
                                                                  RiskeerCommonFormsResources.UpdateItemIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationGroupWithCalculationWithSurfaceLineAndInputOutOfSync_ContextMenuItemUpdateEntryAndExitPointsEnabledAndToolTipSet()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var pipingFailureMechanism = new TestPipingFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                PipingCalculationScenario pipingCalculationScenario = PipingCalculationScenarioTestFactory.CreatePipingCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());
                var group = new CalculationGroup
                {
                    Children =
                    {
                        pipingCalculationScenario
                    }
                };

                var nodeData = new PipingCalculationGroupContext(group,
                                                                 null,
                                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                 pipingFailureMechanism,
                                                                 assessmentSection);

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mocks.ReplayAll();

                plugin.Gui = gui;

                ChangeSurfaceLine(pipingCalculationScenario.InputParameters.SurfaceLine);

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  contextMenuUpdateEntryAndExitPointsAllIndexRootGroup,
                                                                  "&Bijwerken intrede- en uittredepunten...",
                                                                  "Alle berekeningen met een profielschematisatie bijwerken.",
                                                                  RiskeerCommonFormsResources.UpdateItemIcon);
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
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var nodeData = new PipingCalculationGroupContext(group,
                                                                 parentGroup,
                                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                 pipingFailureMechanism,
                                                                 assessmentSection);
                var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                                       null,
                                                                       Enumerable.Empty<PipingSurfaceLine>(),
                                                                       Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                       pipingFailureMechanism,
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
            using (var treeViewControl = new TreeViewControl())
            {
                var group = new CalculationGroup();
                var parentGroup = new CalculationGroup();
                var pipingFailureMechanism = new PipingFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var nodeData = new PipingCalculationGroupContext(group,
                                                                 parentGroup,
                                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                 pipingFailureMechanism,
                                                                 assessmentSection);
                var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                                       null,
                                                                       Enumerable.Empty<PipingSurfaceLine>(),
                                                                       Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                       pipingFailureMechanism,
                                                                       assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);

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
                    ICalculationBase newlyAddedItem = group.Children.Last();
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
                var pipingFailureMechanism = new TestPipingFailureMechanism();
                var assessmentSection = new AssessmentSectionStub();
                var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

                assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
                {
                    hydraulicBoundaryLocation
                }, true);

                PipingCalculationScenario validCalculation = PipingCalculationScenarioTestFactory.CreatePipingCalculationScenarioWithValidInput(hydraulicBoundaryLocation);
                validCalculation.Name = "A";
                PipingCalculationScenario invalidCalculation = PipingCalculationScenarioTestFactory.CreatePipingCalculationScenarioWithInvalidInput();
                invalidCalculation.Name = "B";

                var childGroup = new CalculationGroup();
                childGroup.Children.Add(validCalculation);

                var emptyChildGroup = new CalculationGroup();
                var group = new CalculationGroup();
                var parentGroup = new CalculationGroup();

                group.Children.Add(childGroup);
                group.Children.Add(emptyChildGroup);
                group.Children.Add(invalidCalculation);

                var nodeData = new PipingCalculationGroupContext(group,
                                                                 parentGroup,
                                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                 pipingFailureMechanism,
                                                                 assessmentSection);
                var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                                       null,
                                                                       Enumerable.Empty<PipingSurfaceLine>(),
                                                                       Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                       pipingFailureMechanism,
                                                                       assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Call
                    Action call = () => contextMenu.Items[contextMenuValidateAllIndexNestedGroup].PerformClick();

                    // Assert
                    TestHelper.AssertLogMessages(call, messages =>
                    {
                        string[] msgs = messages.ToArray();
                        Assert.AreEqual(9, msgs.Length);
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs[1]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs[2]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs[8]);
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

                var pipingFailureMechanism = new TestPipingFailureMechanism();
                var assessmentSection = new AssessmentSectionStub();
                var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

                assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
                {
                    hydraulicBoundaryLocation
                }, true);

                PipingCalculationScenario calculationA = PipingCalculationScenarioTestFactory.CreatePipingCalculationScenarioWithValidInput(hydraulicBoundaryLocation);
                calculationA.Name = "A";
                PipingCalculationScenario calculationB = PipingCalculationScenarioTestFactory.CreatePipingCalculationScenarioWithValidInput(hydraulicBoundaryLocation);
                calculationB.Name = "B";

                var childGroup = new CalculationGroup();
                childGroup.Children.Add(calculationA);

                var emptyChildGroup = new CalculationGroup();

                var group = new CalculationGroup();
                var parentGroup = new CalculationGroup();

                group.Children.Add(childGroup);
                group.Children.Add(emptyChildGroup);
                group.Children.Add(calculationB);

                var nodeData = new PipingCalculationGroupContext(group,
                                                                 parentGroup,
                                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                 pipingFailureMechanism,
                                                                 assessmentSection);
                var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                                       null,
                                                                       Enumerable.Empty<PipingSurfaceLine>(),
                                                                       Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                       pipingFailureMechanism,
                                                                       assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    DialogBoxHandler = (name, wnd) =>
                    {
                        // Expect an activity dialog which is automatically closed
                    };

                    // Call
                    Action call = () => contextMenu.Items[contextMenuCalculateAllIndexNestedGroup].PerformClick();

                    // Assert
                    TestHelper.AssertLogMessages(call, messages =>
                    {
                        string[] msgs = messages.ToArray();
                        Assert.AreEqual(12, msgs.Length);
                        Assert.AreEqual("Uitvoeren van berekening 'A' is gestart.", msgs[0]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                        CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[3]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[4]);
                        Assert.AreEqual("Uitvoeren van berekening 'A' is gelukt.", msgs[5]);

                        Assert.AreEqual("Uitvoeren van berekening 'B' is gestart.", msgs[6]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs[7]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs[8]);
                        CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[9]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[10]);
                        Assert.AreEqual("Uitvoeren van berekening 'B' is gelukt.", msgs[11]);
                    });
                }
            }
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

                var pipingFailureMechanism = new TestPipingFailureMechanism();
                var assessmentSection = new AssessmentSectionStub();
                var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

                assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
                {
                    hydraulicBoundaryLocation
                }, true);

                PipingCalculationScenario calculation1 = PipingCalculationScenarioTestFactory.CreatePipingCalculationScenarioWithValidInput(hydraulicBoundaryLocation);
                calculation1.Name = "A";
                calculation1.Output = PipingOutputTestFactory.Create();
                calculation1.Attach(calculation1Observer);
                PipingCalculationScenario calculation2 = PipingCalculationScenarioTestFactory.CreatePipingCalculationScenarioWithValidInput(hydraulicBoundaryLocation);
                calculation2.Name = "B";
                calculation2.Output = PipingOutputTestFactory.Create();
                calculation2.Attach(calculation2Observer);

                var childGroup = new CalculationGroup();
                childGroup.Children.Add(calculation1);

                var emptyChildGroup = new CalculationGroup();
                var group = new CalculationGroup();
                var parentGroup = new CalculationGroup();

                group.Children.Add(childGroup);
                group.Children.Add(emptyChildGroup);
                group.Children.Add(calculation2);

                var nodeData = new PipingCalculationGroupContext(group,
                                                                 parentGroup,
                                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                 pipingFailureMechanism,
                                                                 assessmentSection);
                var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                                       null,
                                                                       Enumerable.Empty<PipingSurfaceLine>(),
                                                                       Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                       pipingFailureMechanism,
                                                                       assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
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
                    contextMenu.Items[contextMenuClearOutputIndexNestedGroup].PerformClick();

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
                var assessmentSection = mocks.Stub<IAssessmentSection>();

                var surfaceLines = new[]
                {
                    new PipingSurfaceLine("surfaceLine1"),
                    new PipingSurfaceLine("surfaceLine2")
                };
                var nodeData = new PipingCalculationGroupContext(group,
                                                                 null,
                                                                 surfaceLines,
                                                                 new[]
                                                                 {
                                                                     PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel()
                                                                 },
                                                                 pipingFailureMechanism,
                                                                 assessmentSection);
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var mainWindow = mocks.Stub<IMainWindow>();

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                PipingSurfaceLineSelectionDialog selectionDialog = null;
                DataGridViewControl grid = null;
                var rowCount = 0;
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
                var failureMechanism = new PipingFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();

                var surfaceLine1 = new PipingSurfaceLine("Surface line 1")
                {
                    ReferenceLineIntersectionWorldPoint = new Point2D(0.0, 0.0)
                };

                surfaceLine1.SetGeometry(new[]
                {
                    new Point3D(0.0, 5.0, 0.0),
                    new Point3D(0.0, 0.0, 1.0),
                    new Point3D(0.0, -5.0, 0.0)
                });

                var surfaceLine2 = new PipingSurfaceLine("Surface line 2")
                {
                    ReferenceLineIntersectionWorldPoint = new Point2D(5.0, 0.0)
                };

                surfaceLine2.SetGeometry(new[]
                {
                    new Point3D(5.0, 5.0, 0.0),
                    new Point3D(5.0, 0.0, 1.0),
                    new Point3D(5.0, -5.0, 0.0)
                });

                PipingSurfaceLine[] surfaceLines =
                {
                    surfaceLine1,
                    surfaceLine2
                };
                FailureMechanismTestHelper.SetSections(failureMechanism, new[]
                {
                    new FailureMechanismSection("Section 1", new[]
                    {
                        new Point2D(0.0, 0.0),
                        new Point2D(5.0, 0.0)
                    }),
                    new FailureMechanismSection("Section 2", new[]
                    {
                        new Point2D(5.0, 0.0),
                        new Point2D(10.0, 0.0)
                    })
                });

                var nodeData = new PipingCalculationGroupContext(
                    failureMechanism.CalculationsGroup,
                    null,
                    surfaceLines,
                    new[]
                    {
                        new PipingStochasticSoilModel("name", new[]
                        {
                            new Point2D(0.0, 0.0),
                            new Point2D(5.0, 0.0)
                        }, new[]
                        {
                            new PipingStochasticSoilProfile(0.5, PipingSoilProfileTestFactory.CreatePipingSoilProfile("A")),
                            new PipingStochasticSoilProfile(0.5, PipingSoilProfileTestFactory.CreatePipingSoilProfile("B"))
                        })
                    },
                    failureMechanism,
                    assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var mainWindow = mocks.Stub<IMainWindow>();

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);
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
                    PipingFailureMechanismSectionResult failureMechanismSectionResult1 = failureMechanism.SectionResults.First();
                    PipingFailureMechanismSectionResult failureMechanismSectionResult2 = failureMechanism.SectionResults.ElementAt(1);

                    PipingCalculationScenario[] pipingCalculationScenarios = failureMechanism.Calculations.OfType<PipingCalculationScenario>().ToArray();
                    Assert.AreEqual(2, failureMechanismSectionResult1.GetCalculationScenarios(pipingCalculationScenarios).Count());

                    foreach (PipingCalculationScenario calculationScenario in failureMechanismSectionResult1.GetCalculationScenarios(pipingCalculationScenarios))
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

                var failureMechanism = new PipingFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();

                var surfaceLine1 = new PipingSurfaceLine("Surface line 1")
                {
                    ReferenceLineIntersectionWorldPoint = new Point2D(0.0, 0.0)
                };

                surfaceLine1.SetGeometry(new[]
                {
                    new Point3D(0.0, 5.0, 0.0),
                    new Point3D(0.0, 0.0, 1.0),
                    new Point3D(0.0, -5.0, 0.0)
                });

                var surfaceLine2 = new PipingSurfaceLine("Surface line 2")
                {
                    ReferenceLineIntersectionWorldPoint = new Point2D(5.0, 0.0)
                };

                surfaceLine2.SetGeometry(new[]
                {
                    new Point3D(5.0, 5.0, 0.0),
                    new Point3D(5.0, 0.0, 1.0),
                    new Point3D(5.0, -5.0, 0.0)
                });

                PipingSurfaceLine[] surfaceLines =
                {
                    surfaceLine1,
                    surfaceLine2
                };
                FailureMechanismTestHelper.SetSections(failureMechanism, new[]
                {
                    new FailureMechanismSection("Section 1", new[]
                    {
                        new Point2D(0.0, 0.0),
                        new Point2D(5.0, 0.0)
                    }),
                    new FailureMechanismSection("Section 2", new[]
                    {
                        new Point2D(5.0, 0.0),
                        new Point2D(10.0, 0.0)
                    })
                });

                var nodeData = new PipingCalculationGroupContext(group,
                                                                 null,
                                                                 surfaceLines,
                                                                 new[]
                                                                 {
                                                                     PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel("name", new[]
                                                                     {
                                                                         new Point2D(0.0, 0.0),
                                                                         new Point2D(5.0, 0.0)
                                                                     })
                                                                 },
                                                                 failureMechanism,
                                                                 assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var mainWindow = mocks.Stub<IMainWindow>();

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Precondition
                foreach (PipingFailureMechanismSectionResult failureMechanismSectionResult in failureMechanism.SectionResults)
                {
                    CollectionAssert.IsEmpty(failureMechanismSectionResult.GetCalculationScenarios(failureMechanism.Calculations.OfType<PipingCalculationScenario>()));
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
                    foreach (PipingFailureMechanismSectionResult failureMechanismSectionResult in failureMechanism.SectionResults)
                    {
                        CollectionAssert.IsEmpty(failureMechanismSectionResult.GetCalculationScenarios(failureMechanism.Calculations.OfType<PipingCalculationScenario>()));
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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var nodeData = new PipingCalculationGroupContext(group,
                                                             null,
                                                             Enumerable.Empty<PipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingStochasticSoilModel>(),
                                                             pipingFailureMechanism,
                                                             assessmentSection);

            var parentGroup = new CalculationGroup();
            parentGroup.Children.Add(group);
            var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                                   null,
                                                                   Enumerable.Empty<PipingSurfaceLine>(),
                                                                   Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                   pipingFailureMechanism,
                                                                   assessmentSection);
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
            TestPipingFailureMechanism pipingFailureMechanism = TestPipingFailureMechanism.GetFailureMechanismWithSurfaceLinesAndStochasticSoilModels();
            PipingSurfaceLine[] surfaceLines = pipingFailureMechanism.SurfaceLines.ToArray();

            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLines[0]
                }
            };

            group.Children.Add(calculation);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            CalculationGroup parentGroup = pipingFailureMechanism.CalculationsGroup;
            parentGroup.Children.Add(group);

            var nodeData = new PipingCalculationGroupContext(group,
                                                             parentGroup,
                                                             Enumerable.Empty<PipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingStochasticSoilModel>(),
                                                             pipingFailureMechanism,
                                                             assessmentSection);
            var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                                   null,
                                                                   Enumerable.Empty<PipingSurfaceLine>(),
                                                                   Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                   pipingFailureMechanism,
                                                                   assessmentSection);
            parentNodeData.Attach(observer);

            // Precondition
            Assert.IsTrue(info.CanRemove(nodeData, parentNodeData));
            PipingFailureMechanismSectionResult[] sectionResults = pipingFailureMechanism.SectionResults.ToArray();
            CollectionAssert.Contains(sectionResults[0].GetCalculationScenarios(pipingFailureMechanism.Calculations.OfType<PipingCalculationScenario>()), calculation);

            // Call
            info.OnNodeRemoved(nodeData, parentNodeData);

            // Assert
            CollectionAssert.DoesNotContain(parentGroup.Children, group);
            CollectionAssert.DoesNotContain(sectionResults[0].GetCalculationScenarios(pipingFailureMechanism.Calculations.OfType<PipingCalculationScenario>()), calculation);
        }

        [Test]
        public void GivenCalculationWithoutOutputAndWithInputOutOfSync_WhenUpdateEntryAndExitPointsClicked_ThenNoInquiryAndCalculationUpdatedAndInputObserverNotified()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var calculation1Observer = mocks.StrictMock<IObserver>();
                var calculation1InputObserver = mocks.StrictMock<IObserver>();
                var calculation2Observer = mocks.StrictMock<IObserver>();
                var calculation2InputObserver = mocks.StrictMock<IObserver>();

                calculation1InputObserver.Expect(obs => obs.UpdateObserver());
                calculation2InputObserver.Expect(obs => obs.UpdateObserver());

                var surfaceLine = new PipingSurfaceLine(string.Empty);
                surfaceLine.SetGeometry(new[]
                {
                    new Point3D(1, 2, 3),
                    new Point3D(4, 5, 6)
                });

                var calculation1 = new PipingCalculationScenario(new GeneralPipingInput())
                {
                    InputParameters =
                    {
                        SurfaceLine = surfaceLine,
                        EntryPointL = (RoundedDouble) 0,
                        ExitPointL = (RoundedDouble) 1
                    }
                };
                calculation1.Attach(calculation1Observer);
                calculation1.InputParameters.Attach(calculation1InputObserver);

                var calculation2 = new PipingCalculationScenario(new GeneralPipingInput())
                {
                    InputParameters =
                    {
                        SurfaceLine = surfaceLine,
                        EntryPointL = (RoundedDouble) 0,
                        ExitPointL = (RoundedDouble) 1
                    }
                };
                calculation2.Attach(calculation2Observer);
                calculation2.InputParameters.Attach(calculation2InputObserver);

                var childGroup = new CalculationGroup();
                childGroup.Children.Add(calculation1);

                var emptyChildGroup = new CalculationGroup();
                var group = new CalculationGroup();
                var parentGroup = new CalculationGroup();

                group.Children.Add(childGroup);
                group.Children.Add(emptyChildGroup);
                group.Children.Add(calculation2);

                var pipingFailureMechanism = new PipingFailureMechanism();
                IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(pipingFailureMechanism, mocks);

                var nodeData = new PipingCalculationGroupContext(group,
                                                                 parentGroup,
                                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                 pipingFailureMechanism,
                                                                 assessmentSection);
                var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                                       null,
                                                                       Enumerable.Empty<PipingSurfaceLine>(),
                                                                       Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                       pipingFailureMechanism,
                                                                       assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var mainWindow = mocks.Stub<IMainWindow>();
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // When
                    ChangeSurfaceLine(surfaceLine);

                    contextMenu.Items[contextMenuUpdateEntryAndExitPointsAllIndexNestedGroup].PerformClick();

                    // Then
                    Assert.IsTrue(calculation1.InputParameters.IsEntryAndExitPointInputSynchronized);
                    Assert.IsTrue(calculation2.InputParameters.IsEntryAndExitPointInputSynchronized);
                }
            }
        }

        [Test]
        public void GivenCalculationWithOutputAndInputOutOfSync_WhenUpdateEntryAndExitPointsClickedAndCancelled_ThenInquiryAndCalculationNotUpdatedAndObserversNotNotified()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var calculation1Observer = mocks.StrictMock<IObserver>();
                var calculation1InputObserver = mocks.StrictMock<IObserver>();
                var calculation2Observer = mocks.StrictMock<IObserver>();
                var calculation2InputObserver = mocks.StrictMock<IObserver>();

                var surfaceLine = new PipingSurfaceLine(string.Empty);
                surfaceLine.SetGeometry(new[]
                {
                    new Point3D(1, 2, 3),
                    new Point3D(4, 5, 6)
                });

                var calculation1 = new PipingCalculationScenario(new GeneralPipingInput())
                {
                    InputParameters =
                    {
                        SurfaceLine = surfaceLine,
                        EntryPointL = (RoundedDouble) 0,
                        ExitPointL = (RoundedDouble) 1
                    },
                    Output = PipingOutputTestFactory.Create()
                };
                calculation1.Attach(calculation1Observer);
                calculation1.InputParameters.Attach(calculation1InputObserver);

                var calculation2 = new PipingCalculationScenario(new GeneralPipingInput())
                {
                    InputParameters =
                    {
                        SurfaceLine = surfaceLine,
                        EntryPointL = (RoundedDouble) 0,
                        ExitPointL = (RoundedDouble) 1
                    },
                    Output = PipingOutputTestFactory.Create()
                };
                calculation2.Attach(calculation2Observer);
                calculation2.InputParameters.Attach(calculation2InputObserver);

                var childGroup = new CalculationGroup();
                childGroup.Children.Add(calculation1);

                var emptyChildGroup = new CalculationGroup();
                var group = new CalculationGroup();
                var parentGroup = new CalculationGroup();

                group.Children.Add(childGroup);
                group.Children.Add(emptyChildGroup);
                group.Children.Add(calculation2);

                var pipingFailureMechanism = new PipingFailureMechanism();
                IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(pipingFailureMechanism, mocks);

                var nodeData = new PipingCalculationGroupContext(group,
                                                                 parentGroup,
                                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                 pipingFailureMechanism,
                                                                 assessmentSection);
                var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                                       null,
                                                                       Enumerable.Empty<PipingSurfaceLine>(),
                                                                       Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                       pipingFailureMechanism,
                                                                       assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var mainWindow = mocks.Stub<IMainWindow>();
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                string textBoxMessage = null;
                DialogBoxHandler = (name, wnd) =>
                {
                    var helper = new MessageBoxTester(wnd);
                    textBoxMessage = helper.Text;
                    helper.ClickCancel();
                };

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // When
                    ChangeSurfaceLine(surfaceLine);

                    contextMenu.Items[contextMenuUpdateEntryAndExitPointsAllIndexNestedGroup].PerformClick();

                    // Then
                    Assert.IsTrue(calculation1.HasOutput);
                    Assert.IsFalse(calculation1.InputParameters.IsEntryAndExitPointInputSynchronized);
                    Assert.IsTrue(calculation2.HasOutput);
                    Assert.IsFalse(calculation2.InputParameters.IsEntryAndExitPointInputSynchronized);

                    string expectedMessage = "Als u kiest voor bijwerken, dan wordt het resultaat van alle bij te werken berekeningen " +
                                             $"verwijderd.{Environment.NewLine}{Environment.NewLine}Weet u zeker dat u wilt doorgaan?";
                    Assert.AreEqual(expectedMessage, textBoxMessage);
                }
            }
        }

        [Test]
        public void GivenCalculationWithOutputAndInputOutOfSync_WhenUpdateEntryAndExitPointsClickedAndContinued_ThenInquiryAndCalculationUpdatedAndObserversNotified()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var calculation1Observer = mocks.StrictMock<IObserver>();
                var calculation1InputObserver = mocks.StrictMock<IObserver>();
                var calculation2Observer = mocks.StrictMock<IObserver>();
                var calculation2InputObserver = mocks.StrictMock<IObserver>();

                calculation1Observer.Expect(obs => obs.UpdateObserver());
                calculation1InputObserver.Expect(obs => obs.UpdateObserver());
                calculation2Observer.Expect(obs => obs.UpdateObserver());
                calculation2InputObserver.Expect(obs => obs.UpdateObserver());

                var surfaceLine = new PipingSurfaceLine(string.Empty);
                surfaceLine.SetGeometry(new[]
                {
                    new Point3D(1, 2, 3),
                    new Point3D(4, 5, 6)
                });

                var calculation1 = new PipingCalculationScenario(new GeneralPipingInput())
                {
                    InputParameters =
                    {
                        SurfaceLine = surfaceLine,
                        EntryPointL = (RoundedDouble) 0,
                        ExitPointL = (RoundedDouble) 1
                    },
                    Output = PipingOutputTestFactory.Create()
                };
                calculation1.Attach(calculation1Observer);
                calculation1.InputParameters.Attach(calculation1InputObserver);

                var calculation2 = new PipingCalculationScenario(new GeneralPipingInput())
                {
                    InputParameters =
                    {
                        SurfaceLine = surfaceLine,
                        EntryPointL = (RoundedDouble) 0,
                        ExitPointL = (RoundedDouble) 1
                    },
                    Output = PipingOutputTestFactory.Create()
                };
                calculation2.Attach(calculation2Observer);
                calculation2.InputParameters.Attach(calculation2InputObserver);

                var childGroup = new CalculationGroup();
                childGroup.Children.Add(calculation1);

                var emptyChildGroup = new CalculationGroup();
                var group = new CalculationGroup();
                var parentGroup = new CalculationGroup();

                group.Children.Add(childGroup);
                group.Children.Add(emptyChildGroup);
                group.Children.Add(calculation2);

                var pipingFailureMechanism = new PipingFailureMechanism();
                IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(pipingFailureMechanism, mocks);

                var nodeData = new PipingCalculationGroupContext(group,
                                                                 parentGroup,
                                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                 pipingFailureMechanism,
                                                                 assessmentSection);
                var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                                       null,
                                                                       Enumerable.Empty<PipingSurfaceLine>(),
                                                                       Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                       pipingFailureMechanism,
                                                                       assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var mainWindow = mocks.Stub<IMainWindow>();
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                string textBoxMessage = null;
                DialogBoxHandler = (name, wnd) =>
                {
                    var helper = new MessageBoxTester(wnd);
                    textBoxMessage = helper.Text;
                    helper.ClickOk();
                };

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // When
                    ChangeSurfaceLine(surfaceLine);

                    contextMenu.Items[contextMenuUpdateEntryAndExitPointsAllIndexNestedGroup].PerformClick();

                    // Then
                    Assert.IsFalse(calculation1.HasOutput);
                    Assert.IsTrue(calculation1.InputParameters.IsEntryAndExitPointInputSynchronized);
                    Assert.IsFalse(calculation2.HasOutput);
                    Assert.IsTrue(calculation2.InputParameters.IsEntryAndExitPointInputSynchronized);

                    string expectedMessage = "Als u kiest voor bijwerken, dan wordt het resultaat van alle bij te werken berekeningen " +
                                             $"verwijderd.{Environment.NewLine}{Environment.NewLine}Weet u zeker dat u wilt doorgaan?";
                    Assert.AreEqual(expectedMessage, textBoxMessage);
                }
            }
        }

        public override void Setup()
        {
            mocks = new MockRepository();
            plugin = new PipingPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(PipingCalculationGroupContext));
        }

        public override void TearDown()
        {
            plugin.Dispose();
            mocks.VerifyAll();

            base.TearDown();
        }

        private static void ChangeSurfaceLine(PipingSurfaceLine surfaceLine)
        {
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(1, 0, 2),
                new Point3D(2, 0, 3),
                new Point3D(3, 0, 0),
                new Point3D(4, 0, 2),
                new Point3D(5, 0, 3)
            });
            surfaceLine.SetDikeToeAtRiverAt(new Point3D(2, 0, 3));
            surfaceLine.SetDikeToeAtPolderAt(new Point3D(3, 0, 0));
        }
    }
}