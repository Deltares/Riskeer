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
using Core.Gui.Plugin;
using Core.Gui.TestUtil;
using Core.Gui.TestUtil.ContextMenu;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using NSubstitute;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Service.TestUtil;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Forms;
using Riskeer.MacroStabilityInwards.Forms.PresentationObjects;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators;
using Riskeer.MacroStabilityInwards.Primitives;
using CoreGuiResources = Core.Gui.Properties.Resources;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.MacroStabilityInwards.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationGroupContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuImportCalculationGroupIndexRootGroup = 2;
        private const int contextMenuExportCalculationGroupIndexRootGroup = 3;
        private const int contextMenuAddCalculationGroupIndexRootGroup = 7;
        private const int contextMenuAddCalculationIndexRootGroup = 8;
        private const int contextMenuValidateAllIndexRootGroup = 10;
        private const int contextMenuCalculateAllIndexRootGroup = 11;
        private const int contextMenuClearOutputIndexRootGroup = 13;
        private const int contextMenuCollapseAllIndexRootGroup = 16;
        private const int contextMenuExpandAllIndexRootGroup = 17;
        private const int contextMenuPropertiesIndexRootGroup = 19;

        private const int contextMenuImportCalculationGroupIndexNestedGroup = 0;
        private const int contextMenuExportCalculationGroupIndexNestedGroup = 1;
        private const int contextMenuDuplicateIndexNestedGroup = 3;
        private const int contextMenuAddCalculationGroupIndexNestedGroup = 5;
        private const int contextMenuAddCalculationIndexNestedGroup = 6;
        private const int contextMenuRenameCalculationGroupIndexNestedGroup = 8;
        private const int contextMenuValidateAllIndexNestedGroup = 10;
        private const int contextMenuCalculateAllIndexNestedGroup = 11;
        private const int contextMenuClearOutputIndexNestedGroup = 13;
        private const int contextMenuDeleteCalculationGroupIndexNestedGroup = 14;
        private const int contextMenuCollapseAllIndexNestedGroup = 16;
        private const int contextMenuExpandAllIndexNestedGroup = 17;
        private const int contextMenuPropertiesIndexNestedGroup = 19;

        private const int customOnlyContextMenuAddGenerateCalculationsIndex = 5;

        
        private MacroStabilityInwardsPlugin plugin;
        private TreeNodeInfo info;

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
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var groupContext = new MacroStabilityInwardsCalculationGroupContext(group,
                                                                                null,
                                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
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
            var calculationItem = Substitute.For<ICalculationBase>();

            var childCalculation = new MacroStabilityInwardsCalculationScenario();

            var childGroup = new CalculationGroup();

            var group = new CalculationGroup();
            group.Children.Add(calculationItem);
            group.Children.Add(childCalculation);
            group.Children.Add(childGroup);

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var nodeData = new MacroStabilityInwardsCalculationGroupContext(group,
                                                                            null,
                                                                            Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                            Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                            failureMechanism,
                                                                            assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(nodeData).ToArray();

            // Assert
            Assert.AreEqual(group.Children.Count, children.Length);
            Assert.AreSame(calculationItem, children[0]);
            var returnedCalculationContext = (MacroStabilityInwardsCalculationScenarioContext) children[1];
            Assert.AreSame(childCalculation, returnedCalculationContext.WrappedData);
            Assert.AreSame(group, returnedCalculationContext.Parent);
            Assert.AreSame(failureMechanism, returnedCalculationContext.FailureMechanism);
            var returnedCalculationGroupContext = (MacroStabilityInwardsCalculationGroupContext) children[2];
            Assert.AreSame(childGroup, returnedCalculationGroupContext.WrappedData);
            Assert.AreSame(group, returnedCalculationGroupContext.Parent);
            Assert.AreSame(failureMechanism, returnedCalculationGroupContext.FailureMechanism);
            Assert.AreSame(assessmentSection, returnedCalculationGroupContext.AssessmentSection);
        }

        [Test]
        public void ContextMenuStrip_NestedCalculationGroupWithCalculationOutput_ReturnContextMenuWithItems()
        {
            // Setup
            var group = new CalculationGroup();
            var parentGroup = new CalculationGroup();

            group.Children.Add(new MacroStabilityInwardsCalculationScenario
            {
                Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
            });

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var nodeData = new MacroStabilityInwardsCalculationGroupContext(group,
                                                                            parentGroup,
                                                                            Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                            Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                            failureMechanism,
                                                                            assessmentSection);
            var parentNodeData = new MacroStabilityInwardsCalculationGroupContext(parentGroup,
                                                                                  null,
                                                                                  Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                  Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                                  failureMechanism,
                                                                                  assessmentSection);

            var applicationFeatureCommandHandler = Substitute.For<IApplicationFeatureCommands>();
            var importHandler = Substitute.For<IImportCommandHandler>();
            importHandler.GetSupportedImportInfos(nodeData).Returns(new[]
            {
                new ImportInfo()
            });
            var exportHandler = Substitute.For<IExportCommandHandler>();
            exportHandler.CanExportFrom(nodeData).Returns(true);
            var updateHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommandsHandler = Substitute.For<IViewCommands>();
            var treeViewControl = Substitute.For<ITreeViewControl>();

            var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler,
                                                     importHandler,
                                                     exportHandler,
                                                     updateHandler,
                                                     viewCommandsHandler,
                                                     nodeData,
                                                     treeViewControl);

            var gui = Substitute.For<IGui>();
            gui.Get(nodeData, treeViewControl).Returns(menuBuilder);

            treeViewControl.CanRemoveNodeForData(nodeData).Returns(true);
            treeViewControl.CanRenameNodeForData(nodeData).Returns(true);
            treeViewControl.CanExpandOrCollapseForData(nodeData).Returns(false);
            plugin.Gui = gui;

            // Call
            using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
            {
                // Assert
                Assert.AreEqual(20, menu.Items.Count);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuImportCalculationGroupIndexNestedGroup,
                                                              "&Importeren...",
                                                              "Importeer de gegevens vanuit een bestand.",
                                                              CoreGuiResources.ImportIcon);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuExportCalculationGroupIndexNestedGroup,
                                                              "&Exporteren...",
                                                              "Exporteer de gegevens naar een bestand.",
                                                              CoreGuiResources.ExportIcon);
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
                                                              RiskeerCommonFormsResources.SemiProbabilisticCalculationIcon);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuRenameCalculationGroupIndexNestedGroup,
                                                              "&Hernoemen",
                                                              "Wijzig de naam van dit element.",
                                                              CoreGuiResources.RenameIcon);
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
                                                              CoreGuiResources.DeleteIcon);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCollapseAllIndexNestedGroup,
                                                              "Alles i&nklappen",
                                                              "Klap dit element en alle onderliggende elementen in.",
                                                              CoreGuiResources.CollapseAllIcon,
                                                              false);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuExpandAllIndexNestedGroup,
                                                              "Alles ui&tklappen",
                                                              "Klap dit element en alle onderliggende elementen uit.",
                                                              CoreGuiResources.ExpandAllIcon,
                                                              false);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuPropertiesIndexNestedGroup,
                                                              "Ei&genschappen",
                                                              "Toon de eigenschappen in het Eigenschappenpaneel.",
                                                              CoreGuiResources.PropertiesHS,
                                                              false);

                CollectionAssert.AllItemsAreInstancesOfType(new[]
                {
                    menu.Items[2],
                    menu.Items[4],
                    menu.Items[7],
                    menu.Items[9],
                    menu.Items[12],
                    menu.Items[15],
                    menu.Items[18]
                }, typeof(ToolStripSeparator));
            }
        }

        [Test]
        public void ContextMenuStrip_WithoutParentNodeDefaultBehavior_ReturnContextMenuWithoutRenameRemove()
        {
            // Setup
            var group = new CalculationGroup();

            group.Children.Add(new MacroStabilityInwardsCalculationScenario
            {
                Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
            });

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var nodeData = new MacroStabilityInwardsCalculationGroupContext(group,
                                                                            null,
                                                                            Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                            Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                            failureMechanism,
                                                                            assessmentSection);

            var applicationFeatureCommandHandler = Substitute.For<IApplicationFeatureCommands>();
            var importHandler = Substitute.For<IImportCommandHandler>();
            importHandler.GetSupportedImportInfos(nodeData).Returns(new[]
            {
                new ImportInfo()
            });
            var exportHandler = Substitute.For<IExportCommandHandler>();
            exportHandler.CanExportFrom(nodeData).Returns(true);
            var updateHandler = Substitute.For<IUpdateCommandHandler>();

            var viewCommandsHandler = Substitute.For<IViewCommands>();
            viewCommandsHandler.CanOpenViewFor(nodeData).Returns(true);

            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler,
                                                         importHandler,
                                                         exportHandler,
                                                         updateHandler,
                                                         viewCommandsHandler,
                                                         nodeData,
                                                         treeViewControl);

                var gui = Substitute.For<IGui>();
                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
                 gui.ViewCommands.Returns(Substitute.For<IViewCommands>());
                plugin.Gui = gui;

                // Call
                ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl);

                // Assert
                Assert.AreEqual(20, menu.Items.Count);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuImportCalculationGroupIndexRootGroup,
                                                              "&Importeren...",
                                                              "Importeer de gegevens vanuit een bestand.",
                                                              CoreGuiResources.ImportIcon);

                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuExportCalculationGroupIndexRootGroup,
                                                              "&Exporteren...",
                                                              "Exporteer de gegevens naar een bestand.",
                                                              CoreGuiResources.ExportIcon);

                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationGroupIndexRootGroup,
                                                              "&Map toevoegen",
                                                              "Voeg een nieuwe map toe aan deze map met berekeningen.",
                                                              RiskeerCommonFormsResources.AddFolderIcon);

                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationIndexRootGroup,
                                                              "Berekening &toevoegen",
                                                              "Voeg een nieuwe berekening toe aan deze map met berekeningen.",
                                                              RiskeerCommonFormsResources.SemiProbabilisticCalculationIcon);
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

                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCollapseAllIndexRootGroup,
                                                              "Alles i&nklappen",
                                                              "Klap dit element en alle onderliggende elementen in.",
                                                              CoreGuiResources.CollapseAllIcon,
                                                              false);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuExpandAllIndexRootGroup,
                                                              "Alles ui&tklappen",
                                                              "Klap dit element en alle onderliggende elementen uit.",
                                                              CoreGuiResources.ExpandAllIcon,
                                                              false);

                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuPropertiesIndexRootGroup,
                                                              "Ei&genschappen",
                                                              "Toon de eigenschappen in het Eigenschappenpaneel.",
                                                              CoreGuiResources.PropertiesHS,
                                                              false);
                CollectionAssert.AllItemsAreInstancesOfType(new[]
                {
                    menu.Items[1],
                    menu.Items[4],
                    menu.Items[6],
                    menu.Items[9],
                    menu.Items[12],
                    menu.Items[15],
                    menu.Items[18]
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

                var failureMechanism = new MacroStabilityInwardsFailureMechanism();
                var assessmentSection = Substitute.For<IAssessmentSection>();

                var nodeData = new MacroStabilityInwardsCalculationGroupContext(group,
                                                                                null,
                                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                new[]
                                                                                {
                                                                                    MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel()
                                                                                },
                                                                                failureMechanism,
                                                                                assessmentSection);

                var gui = Substitute.For<IGui>();
                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
                 gui.ViewCommands.Returns(Substitute.For<IViewCommands>());
                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, customOnlyContextMenuAddGenerateCalculationsIndex,
                                                                  "Genereer &scenario's...",
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

                var failureMechanism = new MacroStabilityInwardsFailureMechanism();
                var assessmentSection = Substitute.For<IAssessmentSection>();

                var nodeData = new MacroStabilityInwardsCalculationGroupContext(group,
                                                                                null,
                                                                                new[]
                                                                                {
                                                                                    new MacroStabilityInwardsSurfaceLine(string.Empty)
                                                                                },
                                                                                Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                                failureMechanism,
                                                                                assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = Substitute.For<IGui>();
                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
                 gui.ViewCommands.Returns(Substitute.For<IViewCommands>());
                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, customOnlyContextMenuAddGenerateCalculationsIndex,
                                                                  "Genereer &scenario's...",
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

                var failureMechanism = new MacroStabilityInwardsFailureMechanism();
                var assessmentSection = Substitute.For<IAssessmentSection>();

                var nodeData = new MacroStabilityInwardsCalculationGroupContext(group,
                                                                                null,
                                                                                new[]
                                                                                {
                                                                                    new MacroStabilityInwardsSurfaceLine(string.Empty)
                                                                                },
                                                                                new[]
                                                                                {
                                                                                    MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel()
                                                                                },
                                                                                failureMechanism,
                                                                                assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = Substitute.For<IGui>();
                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
                 gui.ViewCommands.Returns(Substitute.For<IViewCommands>());
                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, customOnlyContextMenuAddGenerateCalculationsIndex,
                                                                  "Genereer &scenario's...",
                                                                  "Genereer scenario's op basis van geselecteerde profielschematisaties.",
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
                var failureMechanism = new MacroStabilityInwardsFailureMechanism();
                var assessmentSection = new AssessmentSectionStub();
                var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

                assessmentSection.AddHydraulicBoundaryLocationCalculations(new[]
                {
                    hydraulicBoundaryLocation
                }, true);

                var group = new CalculationGroup
                {
                    Children =
                    {
                        MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(hydraulicBoundaryLocation)
                    }
                };

                var nodeData = new MacroStabilityInwardsCalculationGroupContext(group,
                                                                                null,
                                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                                failureMechanism,
                                                                                assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = Substitute.For<IGui>();
                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, failureMechanism, treeViewControl))
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
        public void ContextMenuStrip_ClickOnAddGroupItem_AddGroupToCalculationGroupAndNotifyObservers()
        {
            using (var treeViewControl = new TreeViewControl())
            {
                var group = new CalculationGroup();
                var parentGroup = new CalculationGroup();
                var failureMechanism = new MacroStabilityInwardsFailureMechanism();
                var assessmentSection = Substitute.For<IAssessmentSection>();
                var nodeData = new MacroStabilityInwardsCalculationGroupContext(group,
                                                                                parentGroup,
                                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                                failureMechanism,
                                                                                assessmentSection);
                var parentNodeData = new MacroStabilityInwardsCalculationGroupContext(parentGroup,
                                                                                      null,
                                                                                      Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                      Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                                      failureMechanism,
                                                                                      assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = Substitute.For<IGui>();
                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);

                var observer = Substitute.For<IObserver>();
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
                observer.Received().UpdateObserver();
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
                var failureMechanism = new MacroStabilityInwardsFailureMechanism();
                var assessmentSection = Substitute.For<IAssessmentSection>();
                var nodeData = new MacroStabilityInwardsCalculationGroupContext(group,
                                                                                parentGroup,
                                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                                failureMechanism,
                                                                                assessmentSection);
                var parentNodeData = new MacroStabilityInwardsCalculationGroupContext(parentGroup,
                                                                                      null,
                                                                                      Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                      Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                                      failureMechanism,
                                                                                      assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = Substitute.For<IGui>();
                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);

                var observer = Substitute.For<IObserver>();
                plugin.Gui = gui;

                var calculationItem = new MacroStabilityInwardsCalculationScenario
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
                    Assert.IsInstanceOf<MacroStabilityInwardsCalculation>(newlyAddedItem);
                    Assert.AreEqual("Nieuwe berekening (1)", newlyAddedItem.Name,
                                    "An item with the same name default name already exists, therefore '(1)' needs to be appended.");
                }
                observer.Received().UpdateObserver();
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

                assessmentSection.AddHydraulicBoundaryLocationCalculations(new[]
                {
                    hydraulicBoundaryLocation
                }, true);

                MacroStabilityInwardsCalculationScenario validCalculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(hydraulicBoundaryLocation);
                validCalculation.Name = "A";
                MacroStabilityInwardsCalculationScenario invalidCalculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithInvalidInput();
                invalidCalculation.Name = "B";

                var childGroup = new CalculationGroup();
                childGroup.Children.Add(validCalculation);

                var emptyChildGroup = new CalculationGroup();
                var group = new CalculationGroup();
                var parentGroup = new CalculationGroup();

                group.Children.Add(childGroup);
                group.Children.Add(emptyChildGroup);
                group.Children.Add(invalidCalculation);

                var failureMechanism = new MacroStabilityInwardsFailureMechanism();

                var nodeData = new MacroStabilityInwardsCalculationGroupContext(group,
                                                                                parentGroup,
                                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                                failureMechanism,
                                                                                assessmentSection);
                var parentNodeData = new MacroStabilityInwardsCalculationGroupContext(parentGroup,
                                                                                      null,
                                                                                      Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                      Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                                      failureMechanism,
                                                                                      assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = Substitute.For<IGui>();
                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                using (new MacroStabilityInwardsCalculatorFactoryConfig())
                {
                    // Call
                    void Call() => contextMenu.Items[contextMenuValidateAllIndexNestedGroup].PerformClick();

                    // Assert
                    TestHelper.AssertLogMessages(Call, messages =>
                    {
                        string[] msgs = messages.ToArray();
                        Assert.AreEqual(9, msgs.Length);
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                        Assert.AreEqual("Validatie van waterspanningen in extreme omstandigheden is gestart.", msgs[1]);
                        Assert.AreEqual("Validatie van waterspanningen in dagelijkse omstandigheden is gestart.", msgs[2]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs[3]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs[4]);
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
                var assessmentSection = new AssessmentSectionStub();
                var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

                assessmentSection.AddHydraulicBoundaryLocationCalculations(new[]
                {
                    hydraulicBoundaryLocation
                }, true);

                MacroStabilityInwardsCalculationScenario calculationA = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(hydraulicBoundaryLocation);
                calculationA.Name = "A";
                MacroStabilityInwardsCalculationScenario calculationB = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(hydraulicBoundaryLocation);
                calculationB.Name = "B";

                var childGroup = new CalculationGroup();
                childGroup.Children.Add(calculationA);

                var emptyChildGroup = new CalculationGroup();

                var group = new CalculationGroup();
                var parentGroup = new CalculationGroup();

                group.Children.Add(childGroup);
                group.Children.Add(emptyChildGroup);
                group.Children.Add(calculationB);

                var failureMechanism = new MacroStabilityInwardsFailureMechanism();
                var nodeData = new MacroStabilityInwardsCalculationGroupContext(group,
                                                                                parentGroup,
                                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                                failureMechanism,
                                                                                assessmentSection);
                var parentNodeData = new MacroStabilityInwardsCalculationGroupContext(parentGroup,
                                                                                      null,
                                                                                      Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                      Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                                      failureMechanism,
                                                                                      assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                IMainWindow mainWindow = MainWindowTestHelper.CreateMainWindowStub();

                var gui = Substitute.For<IGui>();
                gui.MainWindow.Returns(mainWindow);
                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
                plugin.Gui = gui;

                using (new MacroStabilityInwardsCalculatorFactoryConfig())
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    DialogBoxHandler = (name, wnd) =>
                    {
                        // Expect an activity dialog which is automatically closed
                    };

                    // Call
                    void Call() => contextMenu.Items[contextMenuCalculateAllIndexNestedGroup].PerformClick();

                    // Assert
                    TestHelper.AssertLogMessages(Call, messages =>
                    {
                        string[] msgs = messages.ToArray();
                        Assert.AreEqual(16, msgs.Length);
                        Assert.AreEqual("Uitvoeren van berekening 'A' is gestart.", msgs[0]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                        Assert.AreEqual("Validatie van waterspanningen in extreme omstandigheden is gestart.", msgs[2]);
                        Assert.AreEqual("Validatie van waterspanningen in dagelijkse omstandigheden is gestart.", msgs[3]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs[4]);
                        CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[5]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[6]);
                        Assert.AreEqual("Uitvoeren van berekening 'A' is gelukt.", msgs[7]);

                        Assert.AreEqual("Uitvoeren van berekening 'B' is gestart.", msgs[8]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs[9]);
                        Assert.AreEqual("Validatie van waterspanningen in extreme omstandigheden is gestart.", msgs[10]);
                        Assert.AreEqual("Validatie van waterspanningen in dagelijkse omstandigheden is gestart.", msgs[11]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs[12]);
                        CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[13]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[14]);
                        Assert.AreEqual("Uitvoeren van berekening 'B' is gelukt.", msgs[15]);
                    });
                }
            }
        }

        [Test]
        [TestCase(false, TestName = "ContextMenuStrip_ClearOutput_ClearOutputCalculationsAndNotifyObservers(false)")]
        [TestCase(true, TestName = "ContextMenuStrip_ClearOutput_ClearOutputCalculationsAndNotifyObservers(true)")]
        public void ContextMenuStrip_ClickOnClearOutputItem_ClearOutputAllChildCalculationsAndNotifyCalculationObservers(bool confirm)
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var calculation1Observer = Substitute.For<IObserver>();
                var calculation2Observer = Substitute.For<IObserver>();
                
                var assessmentSection = new AssessmentSectionStub();
                var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

                assessmentSection.AddHydraulicBoundaryLocationCalculations(new[]
                {
                    hydraulicBoundaryLocation
                }, true);

                MacroStabilityInwardsCalculationScenario calculation1 = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(hydraulicBoundaryLocation);
                calculation1.Name = "A";
                calculation1.Output = MacroStabilityInwardsOutputTestFactory.CreateOutput();
                calculation1.Attach(calculation1Observer);
                MacroStabilityInwardsCalculationScenario calculation2 = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(hydraulicBoundaryLocation);
                calculation2.Name = "B";
                calculation2.Output = MacroStabilityInwardsOutputTestFactory.CreateOutput();
                calculation2.Attach(calculation2Observer);

                var childGroup = new CalculationGroup();
                childGroup.Children.Add(calculation1);

                var emptyChildGroup = new CalculationGroup();
                var group = new CalculationGroup();
                var parentGroup = new CalculationGroup();

                group.Children.Add(childGroup);
                group.Children.Add(emptyChildGroup);
                group.Children.Add(calculation2);

                var failureMechanism = new MacroStabilityInwardsFailureMechanism();

                var nodeData = new MacroStabilityInwardsCalculationGroupContext(group,
                                                                                parentGroup,
                                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                                failureMechanism,
                                                                                assessmentSection);
                var parentNodeData = new MacroStabilityInwardsCalculationGroupContext(parentGroup,
                                                                                      null,
                                                                                      Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                      Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                                      failureMechanism,
                                                                                      assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = Substitute.For<IGui>();
                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
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
                
                if (confirm)
                {
                    calculation1Observer.Received().UpdateObserver();
                    calculation2Observer.Received().UpdateObserver();
                }
                else
                {
                    calculation1Observer.DidNotReceive().UpdateObserver();
                    calculation2Observer.DidNotReceive().UpdateObserver();
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

                var failureMechanism = new MacroStabilityInwardsFailureMechanism();
                var assessmentSection = Substitute.For<IAssessmentSection>();

                var surfaceLines = new[]
                {
                    new MacroStabilityInwardsSurfaceLine("surfaceLine1"),
                    new MacroStabilityInwardsSurfaceLine("surfaceLine2")
                };
                var nodeData = new MacroStabilityInwardsCalculationGroupContext(group,
                                                                                null,
                                                                                surfaceLines,
                                                                                new[]
                                                                                {
                                                                                    MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel()
                                                                                },
                                                                                failureMechanism,
                                                                                assessmentSection);
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var mainWindow = Substitute.For<IMainWindow>();

                var gui = Substitute.For<IGui>();
                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(mainWindow);
                 gui.ViewCommands.Returns(Substitute.For<IViewCommands>());
                plugin.Gui = gui;

                MacroStabilityInwardsSurfaceLineSelectionDialog selectionDialog = null;
                DataGridViewControl grid = null;
                var rowCount = 0;
                DialogBoxHandler = (name, wnd) =>
                {
                    selectionDialog = (MacroStabilityInwardsSurfaceLineSelectionDialog) new FormTester(name).TheObject;
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
        public void OnNodeRemoved_ParentIsMacroStabilityInwardsCalculationGroupContainingGroup_RemoveGroupAndNotifyObservers()
        {
            // Setup
            var observer = Substitute.For<IObserver>();
            var group = new CalculationGroup();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var parentGroup = new CalculationGroup();
            parentGroup.Children.Add(group);

            var nodeData = new MacroStabilityInwardsCalculationGroupContext(group,
                                                                            parentGroup,
                                                                            Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                            Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                            failureMechanism,
                                                                            assessmentSection);
            var parentNodeData = new MacroStabilityInwardsCalculationGroupContext(parentGroup,
                                                                                  null,
                                                                                  Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                  Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                                  failureMechanism,
                                                                                  assessmentSection);
            parentNodeData.Attach(observer);

            // Precondition
            Assert.IsTrue(info.CanRemove(nodeData, parentNodeData));

            // Call
            info.OnNodeRemoved(nodeData, parentNodeData);

            // Assert
            CollectionAssert.DoesNotContain(parentGroup.Children, group);
            observer.Received().UpdateObserver();
        }

        public override void Setup()
        {
            
            plugin = new MacroStabilityInwardsPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(MacroStabilityInwardsCalculationGroupContext));
        }

        public override void TearDown()
        {
            plugin.Dispose();
            base.TearDown();
        }
    }
}