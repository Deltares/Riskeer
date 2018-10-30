// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Forms;
using Ringtoets.MacroStabilityInwards.Forms.PresentationObjects;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators;
using Ringtoets.MacroStabilityInwards.Primitives;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.MacroStabilityInwards.Plugin.Test.TreeNodeInfos
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

        private MockRepository mocks;
        private MacroStabilityInwardsPlugin plugin;
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
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

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
            var calculationItem = mocks.StrictMock<ICalculationBase>();

            var childCalculation = new MacroStabilityInwardsCalculationScenario();

            var childGroup = new CalculationGroup();

            var group = new CalculationGroup();
            group.Children.Add(calculationItem);
            group.Children.Add(childCalculation);
            group.Children.Add(childGroup);

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            mocks.ReplayAll();

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

            var failureMechanism = new TestMacroStabilityInwardsFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
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
                Assert.AreEqual(20, menu.Items.Count);
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
                                                              RingtoetsCommonFormsResources.CopyHS);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationGroupIndexNestedGroup,
                                                              "&Map toevoegen",
                                                              "Voeg een nieuwe map toe aan deze map met berekeningen.",
                                                              RingtoetsCommonFormsResources.AddFolderIcon);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationIndexNestedGroup,
                                                              "Berekening &toevoegen",
                                                              "Voeg een nieuwe berekening toe aan deze map met berekeningen.",
                                                              RingtoetsCommonFormsResources.CalculationIcon);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuRenameCalculationGroupIndexNestedGroup,
                                                              "&Hernoemen",
                                                              "Wijzig de naam van dit element.",
                                                              CoreCommonGuiResources.RenameIcon);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndexNestedGroup,
                                                              "Alles &valideren",
                                                              "Valideer alle berekeningen binnen deze map met berekeningen.",
                                                              RingtoetsCommonFormsResources.ValidateAllIcon);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndexNestedGroup,
                                                              "Alles be&rekenen",
                                                              "Voer alle berekeningen binnen deze map met berekeningen uit.",
                                                              RingtoetsCommonFormsResources.CalculateAllIcon);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearOutputIndexNestedGroup,
                                                              "&Wis alle uitvoer...",
                                                              "Wis de uitvoer van alle berekeningen binnen deze map met berekeningen.",
                                                              RingtoetsCommonFormsResources.ClearIcon);
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

            var failureMechanism = new TestMacroStabilityInwardsFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var nodeData = new MacroStabilityInwardsCalculationGroupContext(group,
                                                                            null,
                                                                            Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                            Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                            failureMechanism,
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
                Assert.AreEqual(20, menu.Items.Count);
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
                                                              RingtoetsCommonFormsResources.AddFolderIcon);

                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationIndexRootGroup,
                                                              "Berekening &toevoegen",
                                                              "Voeg een nieuwe berekening toe aan deze map met berekeningen.",
                                                              RingtoetsCommonFormsResources.CalculationIcon);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndexRootGroup,
                                                              "Alles &valideren",
                                                              "Valideer alle berekeningen binnen deze map met berekeningen.",
                                                              RingtoetsCommonFormsResources.ValidateAllIcon);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndexRootGroup,
                                                              "Alles be&rekenen",
                                                              "Voer alle berekeningen binnen deze map met berekeningen uit.",
                                                              RingtoetsCommonFormsResources.CalculateAllIcon);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearOutputIndexRootGroup,
                                                              "&Wis alle uitvoer...",
                                                              "Wis de uitvoer van alle berekeningen binnen deze map met berekeningen.",
                                                              RingtoetsCommonFormsResources.ClearIcon);

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
                var assessmentSection = mocks.Stub<IAssessmentSection>();

                var nodeData = new MacroStabilityInwardsCalculationGroupContext(group,
                                                                                null,
                                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                new[]
                                                                                {
                                                                                    MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel()
                                                                                },
                                                                                failureMechanism,
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
                                                                  "Genereer &scenario's...",
                                                                  "Er zijn geen profielschematisaties of stochastische ondergrondmodellen beschikbaar om berekeningen voor te genereren.",
                                                                  RingtoetsCommonFormsResources.GenerateScenariosIcon,
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
                var assessmentSection = mocks.Stub<IAssessmentSection>();

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
                                                                  "Genereer &scenario's...",
                                                                  "Er zijn geen profielschematisaties of stochastische ondergrondmodellen beschikbaar om berekeningen voor te genereren.",
                                                                  RingtoetsCommonFormsResources.GenerateScenariosIcon,
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
                var assessmentSection = mocks.Stub<IAssessmentSection>();

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
                                                                  "Genereer &scenario's...",
                                                                  "Genereer scenario's op basis van geselecteerde profielschematisaties.",
                                                                  RingtoetsCommonFormsResources.GenerateScenariosIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_AllRequiredInputSet_ContextMenuItemCalculateAllAndValidateAllEnabled()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var failureMechanism = new TestMacroStabilityInwardsFailureMechanism();
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

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, failureMechanism, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuCalculateAllIndexRootGroup,
                                                                  "Alles be&rekenen",
                                                                  "Voer alle berekeningen binnen deze map met berekeningen uit.",
                                                                  RingtoetsCommonFormsResources.CalculateAllIcon);

                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuValidateAllIndexRootGroup,
                                                                  "Alles &valideren",
                                                                  "Valideer alle berekeningen binnen deze map met berekeningen.",
                                                                  RingtoetsCommonFormsResources.ValidateAllIcon);
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
                var failureMechanism = new MacroStabilityInwardsFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
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
                var failureMechanism = new MacroStabilityInwardsFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
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

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);

                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());
                mocks.ReplayAll();

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

                assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
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

                var failureMechanism = new TestMacroStabilityInwardsFailureMechanism();

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

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                using (new MacroStabilityInwardsCalculatorFactoryConfig())
                {
                    // Call
                    Action call = () => contextMenu.Items[contextMenuValidateAllIndexNestedGroup].PerformClick();

                    // Assert
                    TestHelper.AssertLogMessages(call, messages =>
                    {
                        string[] msgs = messages.ToArray();
                        Assert.AreEqual(7, msgs.Length);
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs[1]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs[2]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs[6]);
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

                var assessmentSection = new AssessmentSectionStub();
                var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

                assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
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

                var failureMechanism = new TestMacroStabilityInwardsFailureMechanism();
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

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                mocks.ReplayAll();

                plugin.Gui = gui;

                using (new MacroStabilityInwardsCalculatorFactoryConfig())
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
        [TestCase(false, TestName = "ContextMenuStrip_ClearOutput_ClearOutputCalculationsAndNotifyObservers(false)")]
        [TestCase(true, TestName = "ContextMenuStrip_ClearOutput_ClearOutputCalculationsAndNotifyObservers(true)")]
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

                var assessmentSection = new AssessmentSectionStub();
                var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

                assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
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

                var failureMechanism = new MacroStabilityInwardsFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();

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
                var mainWindow = mocks.Stub<IMainWindow>();

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());
                mocks.ReplayAll();

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
        public void GivenCalculationsViewGenerateScenariosButtonClicked_WhenSurfaceLineSelectedAndDialogClosed_ThenUpdateSectionResultScenarios()
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                var failureMechanism = new MacroStabilityInwardsFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();

                var surfaceLine1 = new MacroStabilityInwardsSurfaceLine("Surface line 1")
                {
                    ReferenceLineIntersectionWorldPoint = new Point2D(0.0, 0.0)
                };

                surfaceLine1.SetGeometry(new[]
                {
                    new Point3D(0.0, 5.0, 0.0),
                    new Point3D(0.0, 0.0, 1.0),
                    new Point3D(0.0, -5.0, 0.0)
                });

                var surfaceLine2 = new MacroStabilityInwardsSurfaceLine("Surface line 2")
                {
                    ReferenceLineIntersectionWorldPoint = new Point2D(5.0, 0.0)
                };

                surfaceLine2.SetGeometry(new[]
                {
                    new Point3D(5.0, 5.0, 0.0),
                    new Point3D(5.0, 0.0, 1.0),
                    new Point3D(5.0, -5.0, 0.0)
                });

                MacroStabilityInwardsSurfaceLine[] surfaceLines =
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

                var nodeData = new MacroStabilityInwardsCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                                null,
                                                                                surfaceLines,
                                                                                new[]
                                                                                {
                                                                                    MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel("name", new[]
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

                DialogBoxHandler = (name, wnd) =>
                {
                    var selectionDialog = (MacroStabilityInwardsSurfaceLineSelectionDialog) new FormTester(name).TheObject;
                    var grid = (DataGridViewControl) new ControlTester("DataGridViewControl", selectionDialog).TheObject;

                    grid.Rows[0].Cells[0].Value = true;

                    new ButtonTester("DoForSelectedButton", selectionDialog).Click();
                };

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    contextMenu.Items[customOnlyContextMenuAddGenerateCalculationsIndex].PerformClick();

                    // Then
                    MacroStabilityInwardsFailureMechanismSectionResult failureMechanismSectionResult1 = failureMechanism.SectionResults.First();
                    MacroStabilityInwardsFailureMechanismSectionResult failureMechanismSectionResult2 = failureMechanism.SectionResults.ElementAt(1);

                    MacroStabilityInwardsCalculationScenario[] macroStabilityInwardsCalculationScenarios = failureMechanism.Calculations.OfType<MacroStabilityInwardsCalculationScenario>().ToArray();
                    Assert.AreEqual(2, failureMechanismSectionResult1.GetCalculationScenarios(macroStabilityInwardsCalculationScenarios).Count());

                    foreach (MacroStabilityInwardsCalculationScenario calculationScenario in failureMechanismSectionResult1.GetCalculationScenarios(macroStabilityInwardsCalculationScenarios))
                    {
                        Assert.IsInstanceOf<ICalculationScenario>(calculationScenario);
                    }

                    CollectionAssert.IsEmpty(failureMechanismSectionResult2.GetCalculationScenarios(macroStabilityInwardsCalculationScenarios));
                }
            }
        }

        [Test]
        public void GivenCalculationsViewGenerateScenariosButtonClicked_WhenCancelButtonClickedAndDialogClosed_ThenSectionResultScenariosNotUpdated()
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                var group = new CalculationGroup();

                var failureMechanism = new MacroStabilityInwardsFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();

                var surfaceLine1 = new MacroStabilityInwardsSurfaceLine("Surface line 1")
                {
                    ReferenceLineIntersectionWorldPoint = new Point2D(0.0, 0.0)
                };

                surfaceLine1.SetGeometry(new[]
                {
                    new Point3D(0.0, 5.0, 0.0),
                    new Point3D(0.0, 0.0, 1.0),
                    new Point3D(0.0, -5.0, 0.0)
                });

                var surfaceLine2 = new MacroStabilityInwardsSurfaceLine("Surface line 2")
                {
                    ReferenceLineIntersectionWorldPoint = new Point2D(5.0, 0.0)
                };

                surfaceLine2.SetGeometry(new[]
                {
                    new Point3D(5.0, 5.0, 0.0),
                    new Point3D(5.0, 0.0, 1.0),
                    new Point3D(5.0, -5.0, 0.0)
                });

                MacroStabilityInwardsSurfaceLine[] surfaceLines =
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

                var nodeData = new MacroStabilityInwardsCalculationGroupContext(group,
                                                                                null,
                                                                                surfaceLines,
                                                                                new[]
                                                                                {
                                                                                    MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel("name", new[]
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
                foreach (MacroStabilityInwardsFailureMechanismSectionResult failureMechanismSectionResult in failureMechanism.SectionResults)
                {
                    CollectionAssert.IsEmpty(failureMechanismSectionResult.GetCalculationScenarios(failureMechanism.Calculations.OfType<MacroStabilityInwardsCalculationScenario>()));
                }

                DialogBoxHandler = (name, wnd) =>
                {
                    var selectionDialog = (MacroStabilityInwardsSurfaceLineSelectionDialog) new FormTester(name).TheObject;
                    var grid = (DataGridViewControl) new ControlTester("DataGridViewControl", selectionDialog).TheObject;

                    grid.Rows[0].Cells[0].Value = true;

                    new ButtonTester("CustomCancelButton", selectionDialog).Click();
                };

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    contextMenu.Items[customOnlyContextMenuAddGenerateCalculationsIndex].PerformClick();

                    // Then
                    foreach (MacroStabilityInwardsFailureMechanismSectionResult failureMechanismSectionResult in failureMechanism.SectionResults)
                    {
                        CollectionAssert.IsEmpty(failureMechanismSectionResult.GetCalculationScenarios(failureMechanism.Calculations.OfType<MacroStabilityInwardsCalculationScenario>()));
                    }
                }
            }
        }

        [Test]
        public void OnNodeRemoved_ParentIsMacroStabilityInwardsCalculationGroupContainingGroup_RemoveGroupAndNotifyObservers()
        {
            // Setup
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            var group = new CalculationGroup();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

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
        }

        [Test]
        public void OnNodeRemoved_ParentIsMacroStabilityInwardsCalculationGroupContainingGroupContainingCalculations_RemoveGroupAndCalculationsAndNotifyObservers()
        {
            // Setup
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            var group = new CalculationGroup();
            TestMacroStabilityInwardsFailureMechanism macroStabilityInwardsFailureMechanism = TestMacroStabilityInwardsFailureMechanism.GetFailureMechanismWithSurfaceLinesAndStochasticSoilModels();
            MacroStabilityInwardsSurfaceLine[] surfaceLines = macroStabilityInwardsFailureMechanism.SurfaceLines.ToArray();

            var calculation = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLines[0]
                }
            };

            group.Children.Add(calculation);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            CalculationGroup parentGroup = macroStabilityInwardsFailureMechanism.CalculationsGroup;
            parentGroup.Children.Add(group);

            var nodeData = new MacroStabilityInwardsCalculationGroupContext(group,
                                                                            parentGroup,
                                                                            Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                            Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                            macroStabilityInwardsFailureMechanism,
                                                                            assessmentSection);
            var parentNodeData = new MacroStabilityInwardsCalculationGroupContext(parentGroup,
                                                                                  null,
                                                                                  Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                  Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                                  macroStabilityInwardsFailureMechanism,
                                                                                  assessmentSection);
            parentNodeData.Attach(observer);

            // Precondition
            Assert.IsTrue(info.CanRemove(nodeData, parentNodeData));
            MacroStabilityInwardsFailureMechanismSectionResult[] sectionResults = macroStabilityInwardsFailureMechanism.SectionResults.ToArray();
            CollectionAssert.Contains(sectionResults[0].GetCalculationScenarios(macroStabilityInwardsFailureMechanism.Calculations.OfType<MacroStabilityInwardsCalculationScenario>()), calculation);

            // Call
            info.OnNodeRemoved(nodeData, parentNodeData);

            // Assert
            CollectionAssert.DoesNotContain(parentGroup.Children, group);
            CollectionAssert.DoesNotContain(sectionResults[0].GetCalculationScenarios(macroStabilityInwardsFailureMechanism.Calculations.OfType<MacroStabilityInwardsCalculationScenario>()), calculation);
        }

        public override void Setup()
        {
            mocks = new MockRepository();
            plugin = new MacroStabilityInwardsPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(MacroStabilityInwardsCalculationGroupContext));
        }

        public override void TearDown()
        {
            plugin.Dispose();
            mocks.VerifyAll();

            base.TearDown();
        }
    }
}