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
using System.IO;
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
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Data.TestUtil.Probabilistic;
using Riskeer.Piping.Data.TestUtil.SemiProbabilistic;
using Riskeer.Piping.Forms;
using Riskeer.Piping.Forms.PresentationObjects;
using Riskeer.Piping.Forms.PresentationObjects.Probabilistic;
using Riskeer.Piping.Forms.PresentationObjects.SemiProbabilistic;
using Riskeer.Piping.KernelWrapper.TestUtil.SubCalculator;
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
        private const int contextMenuAddSemiProbabilisticCalculationIndexRootGroup = 8;
        private const int contextMenuAddProbabilisticCalculationIndexRootGroup = 9;
        private const int contextMenuUpdateEntryAndExitPointsAllIndexRootGroup = 11;
        private const int contextMenuValidateAllIndexRootGroup = 13;
        private const int contextMenuCalculateAllIndexRootGroup = 14;
        private const int contextMenuClearOutputIndexRootGroup = 16;
        private const int contextMenuClearIllustrationPointsIndexRootGroup = 17;
        private const int contextMenuCollapseAllIndexRootGroup = 20;
        private const int contextMenuExpandAllIndexRootGroup = 21;
        private const int contextMenuPropertiesIndexRootGroup = 23;

        private const int contextMenuImportCalculationGroupIndexNestedGroup = 0;
        private const int contextMenuExportCalculationGroupIndexNestedGroup = 1;
        private const int contextMenuDuplicateIndexNestedGroup = 3;
        private const int contextMenuAddCalculationGroupIndexNestedGroup = 5;
        private const int contextMenuAddSemiProbabilisticCalculationIndexNestedGroup = 6;
        private const int contextMenuAddProbabilisticCalculationIndexNestedGroup = 7;
        private const int contextMenuRenameCalculationGroupIndexNestedGroup = 9;
        private const int contextMenuUpdateEntryAndExitPointsAllIndexNestedGroup = 10;
        private const int contextMenuValidateAllIndexNestedGroup = 12;
        private const int contextMenuCalculateAllIndexNestedGroup = 13;
        private const int contextMenuClearOutputIndexNestedGroup = 15;
        private const int contextMenuClearIllustrationPointsIndexNestedGroup = 16;
        private const int contextMenuDeleteCalculationGroupIndexNestedGroup = 17;
        private const int contextMenuCollapseAllIndexNestedGroup = 19;
        private const int contextMenuExpandAllIndexNestedGroup = 20;
        private const int contextMenuPropertiesIndexNestedGroup = 22;

        private const int customOnlyContextMenuAddGenerateCalculationsIndex = 5;

        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Service, "HydraRingCalculation");
        private static readonly string validHydraulicBoundaryDatabaseFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

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

            var semiProbabilisticPipingCalculationScenario = new SemiProbabilisticPipingCalculationScenario();
            var probabilisticPipingCalculationScenario = new ProbabilisticPipingCalculationScenario();

            var childGroup = new CalculationGroup();

            var group = new CalculationGroup();
            group.Children.Add(calculationItem);
            group.Children.Add(semiProbabilisticPipingCalculationScenario);
            group.Children.Add(probabilisticPipingCalculationScenario);
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

            var returnedSemiProbabilisticPipingCalculationScenarioContext = (SemiProbabilisticPipingCalculationScenarioContext) children[1];
            Assert.AreSame(semiProbabilisticPipingCalculationScenario, returnedSemiProbabilisticPipingCalculationScenarioContext.WrappedData);
            Assert.AreSame(group, returnedSemiProbabilisticPipingCalculationScenarioContext.Parent);
            Assert.AreSame(pipingFailureMechanism, returnedSemiProbabilisticPipingCalculationScenarioContext.FailureMechanism);

            var returnedProbabilisticPipingCalculationScenarioContext = (ProbabilisticPipingCalculationScenarioContext) children[2];
            Assert.AreSame(probabilisticPipingCalculationScenario, returnedProbabilisticPipingCalculationScenarioContext.WrappedData);
            Assert.AreSame(group, returnedProbabilisticPipingCalculationScenarioContext.Parent);
            Assert.AreSame(pipingFailureMechanism, returnedProbabilisticPipingCalculationScenarioContext.FailureMechanism);

            var returnedCalculationGroupContext = (PipingCalculationGroupContext) children[3];
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

            group.Children.Add(new TestPipingCalculationScenario(true));

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
            gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());

            treeViewControl.Expect(tvc => tvc.CanRemoveNodeForData(nodeData)).Return(true);
            treeViewControl.Expect(tvc => tvc.CanRenameNodeForData(nodeData)).Return(true);
            treeViewControl.Expect(tvc => tvc.CanExpandOrCollapseForData(nodeData)).Repeat.Twice().Return(false);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
            {
                // Assert
                Assert.AreEqual(23, menu.Items.Count);
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
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddSemiProbabilisticCalculationIndexNestedGroup,
                                                              "Semi-probabilistische berekening &toevoegen",
                                                              "Voeg een nieuwe semi-probabilistische berekening toe aan deze map met berekeningen.",
                                                              RiskeerCommonFormsResources.SemiProbabilisticCalculationIcon);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddProbabilisticCalculationIndexNestedGroup,
                                                              "&Probabilistische berekening toevoegen",
                                                              "Voeg een nieuwe probabilistische berekening toe aan deze map met berekeningen.",
                                                              RiskeerCommonFormsResources.ProbabilisticCalculationIcon);
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
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearIllustrationPointsIndexNestedGroup,
                                                              "Wis alle illustratiepunten...",
                                                              "Er zijn geen berekeningen met illustratiepunten om te wissen.",
                                                              RiskeerCommonFormsResources.ClearIllustrationPointsIcon,
                                                              false);
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
                    menu.Items[8],
                    menu.Items[11],
                    menu.Items[14],
                    menu.Items[18],
                    menu.Items[21]
                }, typeof(ToolStripSeparator));
            }
        }

        [Test]
        public void ContextMenuStrip_WithoutParentNodeDefaultBehavior_ReturnContextMenuWithoutRenameRemove()
        {
            // Setup
            var group = new CalculationGroup();

            group.Children.Add(new TestPipingCalculationScenario(true));

            var pipingFailureMechanism = new PipingFailureMechanism();
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
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());

                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl);

                // Assert
                Assert.AreEqual(24, menu.Items.Count);
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

                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddSemiProbabilisticCalculationIndexRootGroup,
                                                              "Semi-probabilistische berekening &toevoegen",
                                                              "Voeg een nieuwe semi-probabilistische berekening toe aan deze map met berekeningen.",
                                                              RiskeerCommonFormsResources.SemiProbabilisticCalculationIcon);

                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddProbabilisticCalculationIndexRootGroup,
                                                              "&Probabilistische berekening toevoegen",
                                                              "Voeg een nieuwe probabilistische berekening toe aan deze map met berekeningen.",
                                                              RiskeerCommonFormsResources.ProbabilisticCalculationIcon);

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
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearIllustrationPointsIndexRootGroup,
                                                              "Wis alle illustratiepunten...",
                                                              "Er zijn geen berekeningen met illustratiepunten om te wissen.",
                                                              RiskeerCommonFormsResources.ClearIllustrationPointsIcon,
                                                              false);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCollapseAllIndexRootGroup,
                                                              "Alles i&nklappen",
                                                              "Klap dit element en alle onderliggende elementen in.",
                                                              CoreCommonGuiResources.CollapseAllIcon,
                                                              false);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuExpandAllIndexRootGroup,
                                                              "Alles ui&tklappen",
                                                              "Klap dit element en alle onderliggende elementen uit.",
                                                              CoreCommonGuiResources.ExpandAllIcon,
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
                    menu.Items[10],
                    menu.Items[12],
                    menu.Items[15],
                    menu.Items[19],
                    menu.Items[22]
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
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
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
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
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
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
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
                var assessmentSection = new AssessmentSectionStub();
                var pipingFailureMechanism = new PipingFailureMechanism();
                var group = new CalculationGroup
                {
                    Children =
                    {
                        new TestPipingCalculationScenario()
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
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
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
                var pipingFailureMechanism = new PipingFailureMechanism();

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
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
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
                var pipingFailureMechanism = new PipingFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var group = new CalculationGroup
                {
                    Children =
                    {
                        new TestPipingCalculationScenario()
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
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
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
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                TestPipingFailureMechanism pipingFailureMechanism = TestPipingFailureMechanism.GetFailureMechanismWithSurfaceLinesAndStochasticSoilModels();

                IPipingCalculationScenario<PipingInput> calculation = new TestPipingCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = pipingFailureMechanism.SurfaceLines.First()
                    }
                };

                var group = new CalculationGroup
                {
                    Children =
                    {
                        calculation
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
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
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
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                TestPipingFailureMechanism pipingFailureMechanism = TestPipingFailureMechanism.GetFailureMechanismWithSurfaceLinesAndStochasticSoilModels();

                IPipingCalculationScenario<PipingInput> calculation = new TestPipingCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = pipingFailureMechanism.SurfaceLines.First()
                    }
                };

                var group = new CalculationGroup
                {
                    Children =
                    {
                        calculation
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
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                ChangeSurfaceLine(calculation.InputParameters.SurfaceLine);

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
        public void ContextMenuStrip_CalculationGroupWithoutCalculations_ContextMenuItemClearIllustrationPointsDisabledAndToolTipSet()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var nodeData = new PipingCalculationGroupContext(new CalculationGroup(),
                                                                 null,
                                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                 new PipingFailureMechanism(),
                                                                 mocks.Stub<IAssessmentSection>());
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  contextMenuClearIllustrationPointsIndexRootGroup,
                                                                  "Wis alle illustratiepunten...",
                                                                  "Er zijn geen berekeningen met illustratiepunten om te wissen.",
                                                                  RiskeerCommonFormsResources.ClearIllustrationPointsIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationGroupCalculationsWithIllustrationPoints_ContextMenuClearIllustrationPointsEnabledAndToolTipSet()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                IPipingCalculationScenario<PipingInput> probabilisticCalculation = new ProbabilisticPipingCalculationScenario
                {
                    Output = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithIllustrationPoints()
                };

                IPipingCalculationScenario<PipingInput> semiProbabilisticCalculation = new SemiProbabilisticPipingCalculationScenario();

                var group = new CalculationGroup
                {
                    Children =
                    {
                        probabilisticCalculation,
                        semiProbabilisticCalculation
                    }
                };

                var nodeData = new PipingCalculationGroupContext(group,
                                                                 null,
                                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                 new TestPipingFailureMechanism(),
                                                                 mocks.Stub<IAssessmentSection>());

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  contextMenuClearIllustrationPointsIndexRootGroup,
                                                                  "Wis alle illustratiepunten...",
                                                                  "Wis alle berekende illustratiepunten binnen deze map met berekeningen.",
                                                                  RiskeerCommonFormsResources.ClearIllustrationPointsIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationGroupCalculationsWithoutIllustrationPoints_ContextMenuClearIllustrationPointsDisabledAndToolTipSet()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                IPipingCalculationScenario<PipingInput> probabilisticCalculation = new ProbabilisticPipingCalculationScenario
                {
                    Output = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithoutIllustrationPoints()
                };

                IPipingCalculationScenario<PipingInput> semiProbabilisticCalculation = new SemiProbabilisticPipingCalculationScenario();

                var group = new CalculationGroup
                {
                    Children =
                    {
                        probabilisticCalculation,
                        semiProbabilisticCalculation
                    }
                };

                var nodeData = new PipingCalculationGroupContext(group,
                                                                 null,
                                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                 new PipingFailureMechanism(),
                                                                 mocks.Stub<IAssessmentSection>());

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  contextMenuClearIllustrationPointsIndexRootGroup,
                                                                  "Wis alle illustratiepunten...",
                                                                  "Er zijn geen berekeningen met illustratiepunten om te wissen.",
                                                                  RiskeerCommonFormsResources.ClearIllustrationPointsIcon,
                                                                  false);
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
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());

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
        public void ContextMenuStrip_ClickOnAddSemiProbabilisticCalculationItem_AddSemiProbabilisticCalculationToCalculationGroupAndNotifyObservers()
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
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());

                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());
                mocks.ReplayAll();

                plugin.Gui = gui;

                var semiProbabilisticCalculationScenario = new SemiProbabilisticPipingCalculationScenario
                {
                    Name = "Nieuwe berekening"
                };
                group.Children.Add(semiProbabilisticCalculationScenario);

                nodeData.Attach(observer);

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Precondition
                    Assert.AreEqual(1, group.Children.Count);

                    // Call
                    contextMenu.Items[contextMenuAddSemiProbabilisticCalculationIndexNestedGroup].PerformClick();

                    // Assert
                    Assert.AreEqual(2, group.Children.Count);
                    ICalculationBase newlyAddedItem = group.Children.Last();
                    Assert.IsInstanceOf<SemiProbabilisticPipingCalculationScenario>(newlyAddedItem);
                    Assert.AreEqual("Nieuwe berekening (1)", newlyAddedItem.Name);
                }
            }
        }

        [Test]
        public void GivenCalculationGroupWithProbabilisticCalculation_WhenClickOnAddSemiProbabilisticCalculationItem_ThenCalculationAddedWithSameName()
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
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                var probabilisticCalculationScenario = new ProbabilisticPipingCalculationScenario
                {
                    Name = "Nieuwe berekening"
                };
                group.Children.Add(probabilisticCalculationScenario);

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Precondition
                    Assert.AreEqual(1, group.Children.Count);

                    // Call
                    contextMenu.Items[contextMenuAddSemiProbabilisticCalculationIndexNestedGroup].PerformClick();

                    // Assert
                    Assert.AreEqual(2, group.Children.Count);
                    ICalculationBase newlyAddedItem = group.Children.Last();
                    Assert.IsInstanceOf<SemiProbabilisticPipingCalculationScenario>(newlyAddedItem);
                    Assert.AreEqual("Nieuwe berekening", newlyAddedItem.Name);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_ClickOnAddProbabilisticCalculationItem_AddProbabilisticCalculationToCalculationGroupAndNotifyObservers()
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
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());

                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());
                mocks.ReplayAll();

                plugin.Gui = gui;

                var probabilisticCalculationScenario = new ProbabilisticPipingCalculationScenario
                {
                    Name = "Nieuwe berekening"
                };
                group.Children.Add(probabilisticCalculationScenario);

                nodeData.Attach(observer);

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Precondition
                    Assert.AreEqual(1, group.Children.Count);

                    // Call
                    contextMenu.Items[contextMenuAddProbabilisticCalculationIndexNestedGroup].PerformClick();

                    // Assert
                    Assert.AreEqual(2, group.Children.Count);
                    ICalculationBase newlyAddedItem = group.Children.Last();
                    Assert.IsInstanceOf<ProbabilisticPipingCalculationScenario>(newlyAddedItem);
                    Assert.AreEqual("Nieuwe berekening (1)", newlyAddedItem.Name);
                }
            }
        }

        [Test]
        public void GivenCalculationGroupWithSemiProbabilisticCalculation_WhenClickOnAddProbabilisticCalculationItem_ThenCalculationAddedWithSameName()
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
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());

                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());
                mocks.ReplayAll();

                plugin.Gui = gui;

                var semiProbabilisticCalculationScenario = new SemiProbabilisticPipingCalculationScenario
                {
                    Name = "Nieuwe berekening"
                };
                group.Children.Add(semiProbabilisticCalculationScenario);

                nodeData.Attach(observer);

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Precondition
                    Assert.AreEqual(1, group.Children.Count);

                    // Call
                    contextMenu.Items[contextMenuAddProbabilisticCalculationIndexNestedGroup].PerformClick();

                    // Assert
                    Assert.AreEqual(2, group.Children.Count);
                    ICalculationBase newlyAddedItem = group.Children.Last();
                    Assert.IsInstanceOf<ProbabilisticPipingCalculationScenario>(newlyAddedItem);
                    Assert.AreEqual("Nieuwe berekening", newlyAddedItem.Name);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_ClickOnValidateAllItem_ValidateAllChildCalculations()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSection = new AssessmentSectionStub();
                var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
                TestPipingFailureMechanism failureMechanism = TestPipingFailureMechanism.GetFailureMechanismWithSurfaceLinesAndStochasticSoilModels();

                assessmentSection.HydraulicBoundaryDatabase.FilePath = validHydraulicBoundaryDatabaseFilePath;
                HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);
                assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
                {
                    hydraulicBoundaryLocation
                }, true);

                var validSemiProbabilisticCalculation =
                    SemiProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<SemiProbabilisticPipingCalculationScenario>(
                        hydraulicBoundaryLocation);
                var invalidSemiProbabilisticCalculation =
                    SemiProbabilisticPipingCalculationTestFactory.CreateCalculationWithInvalidInput<SemiProbabilisticPipingCalculationScenario>();
                var validProbabilisticCalculation =
                    ProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<ProbabilisticPipingCalculationScenario>(
                        hydraulicBoundaryLocation);
                var invalidProbabilisticCalculation =
                    ProbabilisticPipingCalculationTestFactory.CreateCalculationWithInvalidInput<ProbabilisticPipingCalculationScenario>();

                var childGroup = new CalculationGroup();
                childGroup.Children.Add(validSemiProbabilisticCalculation);
                childGroup.Children.Add(invalidProbabilisticCalculation);

                var emptyChildGroup = new CalculationGroup();
                var group = new CalculationGroup();
                var parentGroup = new CalculationGroup();

                group.Children.Add(childGroup);
                group.Children.Add(emptyChildGroup);
                group.Children.Add(validProbabilisticCalculation);
                group.Children.Add(invalidSemiProbabilisticCalculation);

                var nodeData = new PipingCalculationGroupContext(group,
                                                                 parentGroup,
                                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                 failureMechanism,
                                                                 assessmentSection);
                var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                                       null,
                                                                       Enumerable.Empty<PipingSurfaceLine>(),
                                                                       Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                       failureMechanism,
                                                                       assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Call
                    void Call() => contextMenu.Items[contextMenuValidateAllIndexNestedGroup].PerformClick();

                    // Assert
                    TestHelper.AssertLogMessages(Call, messages =>
                    {
                        string[] msgs = messages.ToArray();
                        Assert.AreEqual(18, msgs.Length);
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs[1]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs[2]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs[8]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs[9]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs[10]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs[11]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs[17]);
                    });
                }
            }
        }

        [Test]
        public void GivenCalculationGroupWithCalculationOfUnsupportedType_WhenValidatingAllFromContextMenu_ThenThrowsNotSupportedException()
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSection = new AssessmentSectionStub();
                var failureMechanism = new PipingFailureMechanism();
                var calculationGroup = new CalculationGroup
                {
                    Children =
                    {
                        new TestPipingCalculationScenario()
                    }
                };

                var nodeData = new PipingCalculationGroupContext(calculationGroup,
                                                                 null,
                                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                 failureMechanism,
                                                                 assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());

                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    void Call() => contextMenu.Items[contextMenuValidateAllIndexRootGroup].PerformClick();

                    // Then
                    Assert.Throws<NotSupportedException>(Call);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_ClickOnCalculateAllItem_ScheduleAllChildCalculations()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSection = new AssessmentSectionStub();
                var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
                TestPipingFailureMechanism failureMechanism = TestPipingFailureMechanism.GetFailureMechanismWithSurfaceLinesAndStochasticSoilModels();

                assessmentSection.HydraulicBoundaryDatabase.FilePath = validHydraulicBoundaryDatabaseFilePath;
                HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);
                assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
                {
                    hydraulicBoundaryLocation
                }, true);

                var calculationA =
                    SemiProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<SemiProbabilisticPipingCalculationScenario>(
                        hydraulicBoundaryLocation);
                var calculationB =
                    SemiProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<SemiProbabilisticPipingCalculationScenario>(
                        hydraulicBoundaryLocation);
                var calculationC =
                    ProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<ProbabilisticPipingCalculationScenario>(
                        hydraulicBoundaryLocation);
                var calculationD =
                    ProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<ProbabilisticPipingCalculationScenario>(
                        hydraulicBoundaryLocation);

                calculationA.Name = "A";
                calculationB.Name = "B";
                calculationC.Name = "C";
                calculationD.Name = "D";

                var childGroup = new CalculationGroup();
                childGroup.Children.Add(calculationA);
                childGroup.Children.Add(calculationC);

                var emptyChildGroup = new CalculationGroup();

                var group = new CalculationGroup();
                var parentGroup = new CalculationGroup();

                group.Children.Add(childGroup);
                group.Children.Add(emptyChildGroup);
                group.Children.Add(calculationB);
                group.Children.Add(calculationD);

                var nodeData = new PipingCalculationGroupContext(group,
                                                                 parentGroup,
                                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                 failureMechanism,
                                                                 assessmentSection);
                var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                                       null,
                                                                       Enumerable.Empty<PipingSurfaceLine>(),
                                                                       Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                       failureMechanism,
                                                                       assessmentSection);

                var mainWindow = mocks.Stub<IMainWindow>();
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);

                var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Stub(cf => cf.CreatePipingCalculator(null))
                                 .IgnoreArguments()
                                 .Return(new TestPipingCalculator());

                mocks.ReplayAll();

                plugin.Gui = gui;

                DialogBoxHandler = (name, wnd) =>
                {
                    // Expect an activity dialog which is automatically closed
                };

                using (new PipingSubCalculatorFactoryConfig())
                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Call
                    void Call() => contextMenu.Items[contextMenuCalculateAllIndexNestedGroup].PerformClick();

                    // Assert
                    TestHelper.AssertLogMessages(Call, messages =>
                    {
                        string[] msgs = messages.ToArray();
                        Assert.AreEqual(28, msgs.Length);

                        Assert.AreEqual("Uitvoeren van berekening 'A' is gestart.", msgs[0]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                        CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[3]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[4]);
                        Assert.AreEqual("Uitvoeren van berekening 'A' is gelukt.", msgs[5]);

                        Assert.AreEqual("Uitvoeren van berekening 'C' is gestart.", msgs[6]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs[7]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs[8]);
                        CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[9]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[12]);
                        Assert.AreEqual("Uitvoeren van berekening 'C' is gelukt.", msgs[13]);

                        Assert.AreEqual("Uitvoeren van berekening 'B' is gestart.", msgs[14]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs[15]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs[16]);
                        CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[17]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[18]);
                        Assert.AreEqual("Uitvoeren van berekening 'B' is gelukt.", msgs[19]);

                        Assert.AreEqual("Uitvoeren van berekening 'D' is gestart.", msgs[20]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs[21]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs[22]);
                        CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[23]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[26]);
                        Assert.AreEqual("Uitvoeren van berekening 'D' is gelukt.", msgs[27]);
                    });
                }
            }
        }

        [Test]
        public void GivenCalculationGroupWithCalculationOfUnsupportedType_WhenCalculatingAllFromContextMenu_ThenThrowsNotSupportedException()
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSection = new AssessmentSectionStub();
                var failureMechanism = new PipingFailureMechanism();
                var calculationGroup = new CalculationGroup
                {
                    Children =
                    {
                        new TestPipingCalculationScenario()
                    }
                };

                var nodeData = new PipingCalculationGroupContext(calculationGroup,
                                                                 null,
                                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                 failureMechanism,
                                                                 assessmentSection);

                var mainWindow = mocks.Stub<IMainWindow>();
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);

                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    void Call() => contextMenu.Items[contextMenuValidateAllIndexRootGroup].PerformClick();

                    // Then
                    Assert.Throws<NotSupportedException>(Call);
                }
            }
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void ContextMenuStrip_ClickOnClearOutputItem_ClearOutputAllChildCalculationsAndNotifyCalculationObservers(bool confirm)
        {
            // Setup
            var calculation1Observer = mocks.StrictMock<IObserver>();
            var calculation2Observer = mocks.StrictMock<IObserver>();
            if (confirm)
            {
                calculation1Observer.Expect(o => o.UpdateObserver());
                calculation2Observer.Expect(o => o.UpdateObserver());
            }

            var pipingFailureMechanism = new PipingFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var calculation1 = new TestPipingCalculationScenario(true);
            calculation1.Attach(calculation1Observer);

            var calculation2 = new TestPipingCalculationScenario(true);
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

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
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
        [TestCase(false)]
        [TestCase(true)]
        public void ContextMenuStrip_ClickOnIllustrationPointsItem_ClearIllustrationPointsChildCalculationsAndNotifyCalculationObservers(bool confirm)
        {
            // Setup
            var calculation1Observer = mocks.StrictMock<IObserver>();
            var calculation2Observer = mocks.StrictMock<IObserver>();
            if (confirm)
            {
                calculation1Observer.Expect(o => o.UpdateObserver());
                calculation2Observer.Expect(o => o.UpdateObserver());
            }

            var pipingFailureMechanism = new PipingFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var calculation1 = new ProbabilisticPipingCalculationScenario
            {
                Output = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithIllustrationPoints()
            };
            calculation1.Attach(calculation1Observer);

            var calculation2 = new ProbabilisticPipingCalculationScenario
            {
                Output = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithIllustrationPoints()
            };
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

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
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
                    contextMenu.Items[contextMenuClearIllustrationPointsIndexNestedGroup].PerformClick();

                    // Assert
                    Assert.AreNotEqual(confirm, calculation1.Output.ProfileSpecificOutput.HasGeneralResult);
                    Assert.AreNotEqual(confirm, calculation1.Output.SectionSpecificOutput.HasGeneralResult);

                    Assert.AreNotEqual(confirm, calculation2.Output.ProfileSpecificOutput.HasGeneralResult);
                    Assert.AreNotEqual(confirm, calculation2.Output.SectionSpecificOutput.HasGeneralResult);

                    Assert.AreEqual("Bevestigen", messageBoxTitle);
                    Assert.AreEqual("Weet u zeker dat u alle illustratiepunten wilt wissen?", messageBoxText);
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
        [TestCase(true, true)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        public void GivenPipingCalculationsViewGenerateScenariosButtonClicked_WhenSurfaceLineSelectedAndDialogClosed_ThenCalculationScenariosGenerated(
            bool generateSemiProbabilistic, bool generateProbabilistic)
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

                    var semiProbabilisticCheckBox = (CheckBox) new CheckBoxTester("SemiProbabilisticCheckBox", selectionDialog).TheObject;
                    var probabilisticCheckBox = (CheckBox) new CheckBoxTester("ProbabilisticCheckBox", selectionDialog).TheObject;

                    semiProbabilisticCheckBox.Checked = generateSemiProbabilistic;
                    probabilisticCheckBox.Checked = generateProbabilistic;

                    grid.Rows[0].Cells[0].Value = true;

                    new ButtonTester("DoForSelectedButton", selectionDialog).Click();
                };

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    contextMenu.Items[customOnlyContextMenuAddGenerateCalculationsIndex].PerformClick();

                    // Then
                    CalculationGroup addedGroup = failureMechanism.CalculationsGroup.Children.OfType<CalculationGroup>().Single();
                    Assert.AreEqual("Surface line 1", addedGroup.Name);

                    if (generateSemiProbabilistic)
                    {
                        SemiProbabilisticPipingCalculationScenario[] semiProbabilisticPipingCalculationScenarios =
                            addedGroup.Children.OfType<SemiProbabilisticPipingCalculationScenario>().ToArray();

                        Assert.AreEqual(2, semiProbabilisticPipingCalculationScenarios.Length);
                        Assert.AreEqual("Surface line 1 A", semiProbabilisticPipingCalculationScenarios[0].Name);
                        Assert.AreEqual("Surface line 1 B", semiProbabilisticPipingCalculationScenarios[1].Name);
                    }

                    if (generateProbabilistic)
                    {
                        ProbabilisticPipingCalculationScenario[] probabilisticPipingCalculationScenarios =
                            addedGroup.Children.OfType<ProbabilisticPipingCalculationScenario>().ToArray();

                        Assert.AreEqual(2, probabilisticPipingCalculationScenarios.Length);
                        Assert.AreEqual("Surface line 1 A", probabilisticPipingCalculationScenarios[0].Name);
                        Assert.AreEqual("Surface line 1 B", probabilisticPipingCalculationScenarios[1].Name);
                    }
                }
            }
        }

        [Test]
        public void GivenPipingCalculationsViewGenerateScenariosButtonClicked_WhenCancelButtonClickedAndDialogClosed_ThenNoCalculationScenariosGenerated()
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
                CollectionAssert.IsEmpty(failureMechanism.CalculationsGroup.Children);

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
                    CollectionAssert.IsEmpty(failureMechanism.CalculationsGroup.Children);
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

            var calculation = new TestPipingCalculationScenario
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

            // Call
            info.OnNodeRemoved(nodeData, parentNodeData);

            // Assert
            CollectionAssert.DoesNotContain(parentGroup.Children, group);
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

                var calculation1 = new TestPipingCalculationScenario(new TestPipingInput
                {
                    SurfaceLine = surfaceLine,
                    EntryPointL = (RoundedDouble) 0,
                    ExitPointL = (RoundedDouble) 1
                });

                calculation1.Attach(calculation1Observer);
                calculation1.InputParameters.Attach(calculation1InputObserver);

                var calculation2 = new TestPipingCalculationScenario(new TestPipingInput
                {
                    SurfaceLine = surfaceLine,
                    EntryPointL = (RoundedDouble) 0,
                    ExitPointL = (RoundedDouble) 1
                });

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

                var calculation1 = new TestPipingCalculationScenario(new TestPipingInput
                {
                    SurfaceLine = surfaceLine,
                    EntryPointL = (RoundedDouble) 0,
                    ExitPointL = (RoundedDouble) 1
                }, true);

                calculation1.Attach(calculation1Observer);
                calculation1.InputParameters.Attach(calculation1InputObserver);

                var calculation2 = new TestPipingCalculationScenario(new TestPipingInput
                {
                    SurfaceLine = surfaceLine,
                    EntryPointL = (RoundedDouble) 0,
                    ExitPointL = (RoundedDouble) 1
                }, true);

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

                var calculation1 = new TestPipingCalculationScenario(new TestPipingInput
                {
                    SurfaceLine = surfaceLine,
                    EntryPointL = (RoundedDouble) 0,
                    ExitPointL = (RoundedDouble) 1
                }, true);

                calculation1.Attach(calculation1Observer);
                calculation1.InputParameters.Attach(calculation1InputObserver);

                var calculation2 = new TestPipingCalculationScenario(new TestPipingInput
                {
                    SurfaceLine = surfaceLine,
                    EntryPointL = (RoundedDouble) 0,
                    ExitPointL = (RoundedDouble) 1
                }, true);

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