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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
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
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Plugin;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.Revetment.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveConditionsCalculationGroupContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuAddCalculationGroupIndexRootGroup = 3;
        private const int contextMenuAddCalculationIndexRootGroup = 4;
        private const int contextMenuRemoveAllChildrenIndexRootGroup = 6;
        private const int contextMenuValidateAllIndexRootGroup = 8;
        private const int contextMenuCalculateAllIndexRootGroup = 9;
        private const int contextMenuClearOutputIndexRootGroup = 10;
        private const int contextMenuExpandAllIndexRootGroup = 12;
        private const int contextMenuCollapseAllIndexRootGroup = 13;
        private const int contextMenuPropertiesIndexRootGroup = 15;

        private const int contextMenuAddGenerateCalculationsIndex = 0;
        private const int contextMenuAddCalculationGroupIndexNestedGroup = 2;
        private const int contextMenuAddCalculationIndexNestedGroup = 3;
        private const int contextMenuValidateAllIndexNestedGroup = 5;
        private const int contextMenuCalculateAllIndexNestedGroup = 6;
        private const int contextMenuClearOutputNestedGroupIndex = 7;

        private const string expectedTextExpandAll = "Alles ui&tklappen";
        private const string expectedTextExpandAllToolTip = "Klap dit element en alle onderliggende elementen uit.";
        private const string expectedTextRename = "&Hernoemen";
        private const string expectedTextRenameToolTip = "Wijzig de naam van dit element.";
        private const string expectedTextDelete = "Verwij&deren...";
        private const string expectedTextDeleteToolTip = "Verwijder dit element uit de boom.";
        private const string expectedTextCollapseAll = "Alles i&nklappen";
        private const string expectedTextCollapseAllToolTip = "Klap dit element en alle onderliggende elementen in.";
        private const string expectedTextProperties = "Ei&genschappen";
        private const string expectedTextPropertiesToolTip = "Toon de eigenschappen in het Eigenschappenpaneel.";

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
                                                              "&Exporteren...",
                                                              "Exporteer de gegevens naar een bestand.",
                                                              CoreCommonGuiResources.ExportIcon);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationGroupIndexNestedGroup,
                                                              "&Map toevoegen",
                                                              "Voeg een nieuwe berekeningsmap toe aan deze berekeningsmap.",
                                                              RingtoetsCommonFormsResources.AddFolderIcon);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationIndexNestedGroup,
                                                              "Berekening &toevoegen",
                                                              "Voeg een nieuwe berekening toe aan deze berekeningsmap.",
                                                              RingtoetsCommonFormsResources.FailureMechanismIcon);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndexNestedGroup,
                                                              "Alles &valideren",
                                                              "Er zijn geen berekeningen om te valideren.",
                                                              RingtoetsCommonFormsResources.ValidateAllIcon,
                                                              false);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndexNestedGroup,
                                                              "Alles be&rekenen",
                                                              "Er zijn geen berekeningen om uit te voeren.",
                                                              RingtoetsCommonFormsResources.CalculateAllIcon,
                                                              false);
                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearOutputNestedGroupIndex,
                                                              "&Wis alle uitvoer...",
                                                              "Er zijn geen berekeningen met uitvoer om te wissen.",
                                                              RingtoetsCommonFormsResources.ClearIcon,
                                                              false);
                TestHelper.AssertContextMenuStripContainsItem(menu, 9,
                                                              expectedTextRename,
                                                              expectedTextRenameToolTip,
                                                              CoreCommonGuiResources.RenameIcon);
                TestHelper.AssertContextMenuStripContainsItem(menu, 10,
                                                              expectedTextDelete,
                                                              expectedTextDeleteToolTip,
                                                              CoreCommonGuiResources.DeleteIcon);
                TestHelper.AssertContextMenuStripContainsItem(menu, 12,
                                                              expectedTextExpandAll,
                                                              expectedTextExpandAllToolTip,
                                                              CoreCommonGuiResources.ExpandAllIcon,
                                                              false);
                TestHelper.AssertContextMenuStripContainsItem(menu, 13,
                                                              expectedTextCollapseAll,
                                                              expectedTextCollapseAllToolTip,
                                                              CoreCommonGuiResources.CollapseAllIcon,
                                                              false);
                TestHelper.AssertContextMenuStripContainsItem(menu, 15,
                                                              expectedTextProperties,
                                                              expectedTextPropertiesToolTip,
                                                              CoreCommonGuiResources.PropertiesHS,
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
                    Assert.AreEqual(16, menu.Items.Count);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddGenerateCalculationsIndex,
                                                                  "Genereer &berekeningen...",
                                                                  "Er is geen hydraulische randvoorwaardendatabase beschikbaar om de randvoorwaardenberekeningen te genereren.",
                                                                  RingtoetsCommonFormsResources.GenerateScenariosIcon, false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationGroupIndexRootGroup,
                                                                  "&Map toevoegen",
                                                                  "Voeg een nieuwe berekeningsmap toe aan deze berekeningsmap.",
                                                                  RingtoetsCommonFormsResources.AddFolderIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationIndexRootGroup,
                                                                  "Berekening &toevoegen",
                                                                  "Voeg een nieuwe berekening toe aan deze berekeningsmap.",
                                                                  RingtoetsCommonFormsResources.FailureMechanismIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuRemoveAllChildrenIndexRootGroup,
                                                                  "Ma&p leegmaken...",
                                                                  "Er zijn geen onderliggende elementen om te verwijderen.",
                                                                  CoreCommonGuiResources.DeleteChildrenIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndexRootGroup,
                                                                  "Alles &valideren",
                                                                  "Er zijn geen berekeningen om te valideren.",
                                                                  RingtoetsCommonFormsResources.ValidateAllIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndexRootGroup,
                                                                  "Alles be&rekenen",
                                                                  "Er zijn geen berekeningen om uit te voeren.",
                                                                  RingtoetsCommonFormsResources.CalculateAllIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearOutputIndexRootGroup,
                                                                  "&Wis alle uitvoer...",
                                                                  "Er zijn geen berekeningen met uitvoer om te wissen.",
                                                                  RingtoetsCommonFormsResources.ClearIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuExpandAllIndexRootGroup,
                                                                  expectedTextExpandAll,
                                                                  expectedTextExpandAllToolTip,
                                                                  CoreCommonGuiResources.ExpandAllIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCollapseAllIndexRootGroup,
                                                                  expectedTextCollapseAll,
                                                                  expectedTextCollapseAllToolTip,
                                                                  CoreCommonGuiResources.CollapseAllIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuPropertiesIndexRootGroup,
                                                                  expectedTextProperties,
                                                                  expectedTextPropertiesToolTip,
                                                                  CoreCommonGuiResources.PropertiesHS,
                                                                  false);
                    CollectionAssert.AllItemsAreInstancesOfType(new[]
                    {
                        menu.Items[2],
                        menu.Items[5],
                        menu.Items[7],
                        menu.Items[11],
                        menu.Items[14]
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
                    Assert.AreEqual(16, menu.Items.Count);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddGenerateCalculationsIndex,
                                                                  "Genereer &berekeningen...",
                                                                  "Genereer randvoorwaardenberekeningen.",
                                                                  RingtoetsCommonFormsResources.GenerateScenariosIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationGroupIndexRootGroup,
                                                                  "&Map toevoegen",
                                                                  "Voeg een nieuwe berekeningsmap toe aan deze berekeningsmap.",
                                                                  RingtoetsCommonFormsResources.AddFolderIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationIndexRootGroup,
                                                                  "Berekening &toevoegen",
                                                                  "Voeg een nieuwe berekening toe aan deze berekeningsmap.",
                                                                  RingtoetsCommonFormsResources.FailureMechanismIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuRemoveAllChildrenIndexRootGroup,
                                                                  "Ma&p leegmaken...",
                                                                  "Er zijn geen onderliggende elementen om te verwijderen.",
                                                                  CoreCommonGuiResources.DeleteChildrenIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndexRootGroup,
                                                                  "Alles &valideren",
                                                                  "Er zijn geen berekeningen om te valideren.",
                                                                  RingtoetsCommonFormsResources.ValidateAllIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndexRootGroup,
                                                                  "Alles be&rekenen",
                                                                  "Er zijn geen berekeningen om uit te voeren.",
                                                                  RingtoetsCommonFormsResources.CalculateAllIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearOutputIndexRootGroup,
                                                                  "&Wis alle uitvoer...",
                                                                  "Er zijn geen berekeningen met uitvoer om te wissen.",
                                                                  RingtoetsCommonFormsResources.ClearIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuExpandAllIndexRootGroup,
                                                                  expectedTextExpandAll,
                                                                  expectedTextExpandAllToolTip,
                                                                  CoreCommonGuiResources.ExpandAllIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCollapseAllIndexRootGroup,
                                                                  expectedTextCollapseAll,
                                                                  expectedTextCollapseAllToolTip,
                                                                  CoreCommonGuiResources.CollapseAllIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuPropertiesIndexRootGroup,
                                                                  expectedTextProperties,
                                                                  expectedTextPropertiesToolTip,
                                                                  CoreCommonGuiResources.PropertiesHS,
                                                                  false);
                    CollectionAssert.AllItemsAreInstancesOfType(new[]
                    {
                        menu.Items[2],
                        menu.Items[5],
                        menu.Items[7],
                        menu.Items[11],
                        menu.Items[14]
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
        public void ContextMenuStrip_AssessmentSectionWithoutHydraulicBoundaryDatabase_ValidateAndCalculateAllDisabled()
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
                    ToolStripItem validateItem = contextMenu.Items[contextMenuValidateAllIndexNestedGroup];
                    ToolStripItem calculateItem = contextMenu.Items[contextMenuCalculateAllIndexNestedGroup];
                    Assert.IsFalse(validateItem.Enabled);
                    Assert.IsFalse(calculateItem.Enabled);
                    Assert.AreEqual("Er is geen hydraulische randvoorwaardendatabase geïmporteerd.", calculateItem.ToolTipText);
                    Assert.AreEqual("Er is geen hydraulische randvoorwaardendatabase geïmporteerd.", validateItem.ToolTipText);
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
                    ToolStripItem validateItem = contextMenu.Items[contextMenuValidateAllIndexNestedGroup];
                    ToolStripItem calculateItem = contextMenu.Items[contextMenuCalculateAllIndexNestedGroup];
                    Assert.IsFalse(validateItem.Enabled);
                    Assert.IsFalse(calculateItem.Enabled);
                    var message = "Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt. Fout bij het lezen van bestand '': bestandspad mag niet leeg of ongedefinieerd zijn.";
                    Assert.AreEqual(message, calculateItem.ToolTipText);
                    Assert.AreEqual(message, validateItem.ToolTipText);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_TwoCalculationsClickOnValidateAllInGroup_ValidationMessagesLogged()
        {
            // Setup
            string hrdPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");

            var calculationA = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Name = "A",
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
                    UpperBoundaryWaterLevels = (RoundedDouble) 10.0,
                    Orientation = (RoundedDouble) 0
                }
            };
            var calculationB = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Name = "B",
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
                    UpperBoundaryWaterLevels = (RoundedDouble) 10.0,
                    Orientation = (RoundedDouble) 0
                }
            };

            var group = new CalculationGroup();
            group.Children.Add(calculationA);
            group.Children.Add(calculationB);

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);

            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, Path.Combine(hrdPath, "HRD ijsselmeer.sqlite"));

            var nodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(group,
                                                                                              failureMechanism,
                                                                                              assessmentSectionStub);
            var parentNodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                                    failureMechanism,
                                                                                                    assessmentSectionStub);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);

                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Call
                    Action test = () => contextMenu.Items[contextMenuValidateAllIndexNestedGroup].PerformClick();

                    // Assert
                    TestHelper.AssertLogMessages(test, m =>
                    {
                        var messages = m.ToArray();
                        Assert.AreEqual(4, messages.Length);
                        StringAssert.StartsWith("Validatie van 'A' gestart om: ", messages[0]);
                        StringAssert.StartsWith("Validatie van 'A' beëindigd om: ", messages[1]);
                        StringAssert.StartsWith("Validatie van 'B' gestart om: ", messages[2]);
                        StringAssert.StartsWith("Validatie van 'B' beëindigd om: ", messages[3]);
                    });
                }
            }
        }

        [Test]
        public void ContextMenuStrip_TwoCalculationsClickOnCalculateAllInGroup_MessagesLogged()
        {
            // Setup
            string hrdPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");

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

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 10
            };
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);

            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, Path.Combine(hrdPath, "HRD ijsselmeer.sqlite"));

            var nodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(group,
                                                                                              failureMechanism,
                                                                                              assessmentSectionStub);
            var parentNodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                                    failureMechanism,
                                                                                                    assessmentSectionStub);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            using (var treeViewControl = new TreeViewControl())
            {
                var mainWindow = mocks.Stub<IMainWindow>();

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);

                mocks.ReplayAll();

                plugin.Gui = gui;

                using (new HydraRingCalculatorFactoryConfig())
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Call
                    Action test = () => contextMenu.Items[contextMenuCalculateAllIndexNestedGroup].PerformClick();

                    // Assert
                    TestHelper.AssertLogMessages(test, m =>
                    {
                        var messages = m.ToArray();
                        Assert.AreEqual(28, messages.Length);
                        StringAssert.StartsWith("Berekening van 'Nieuwe berekening' gestart om: ", messages[2]);
                        StringAssert.StartsWith("Berekening van 'Nieuwe berekening' beëindigd om: ", messages[12]);
                        StringAssert.StartsWith("Berekening van 'Nieuwe berekening' gestart om: ", messages[15]);
                        StringAssert.StartsWith("Berekening van 'Nieuwe berekening' beëindigd om: ", messages[25]);
                        Assert.AreEqual("Uitvoeren van 'Nieuwe berekening' is gelukt.", messages[26]);
                        Assert.AreEqual("Uitvoeren van 'Nieuwe berekening' is gelukt.", messages[27]);
                    });
                }
                Assert.AreEqual(3, calculationA.Output.Items.Count());
                Assert.AreEqual(3, calculationB.Output.Items.Count());
            }
        }

        [Test]
        public void ContextMenuStrip_NoCalculations_ClearAllOutputItemDisabled()
        {
            // Setup
            string hrdPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var group = new CalculationGroup();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);

            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, Path.Combine(hrdPath, "HRD ijsselmeer.sqlite"));

            var nodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(group,
                                                                                              failureMechanism,
                                                                                              assessmentSectionStub);
            var parentNodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                                    failureMechanism,
                                                                                                    assessmentSectionStub);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                var mainWindow = mocks.Stub<IMainWindow>();

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);

                mocks.ReplayAll();

                plugin.Gui = gui;

                using (new HydraRingCalculatorFactoryConfig())
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
            // Setup
            string hrdPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");

            var group = new CalculationGroup();
            var calculationA = GetValidCalculation();
            var calculationB = GetValidCalculation();
            group.Children.Add(calculationA);
            group.Children.Add(calculationB);

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);

            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, Path.Combine(hrdPath, "HRD ijsselmeer.sqlite"));

            var nodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(group,
                                                                                              failureMechanism,
                                                                                              assessmentSectionStub);
            var parentNodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                                    failureMechanism,
                                                                                                    assessmentSectionStub);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                var mainWindow = mocks.Stub<IMainWindow>();

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);

                mocks.ReplayAll();

                plugin.Gui = gui;

                using (new HydraRingCalculatorFactoryConfig())
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Call
                    var clearAllOutputItem = contextMenu.Items[contextMenuClearOutputNestedGroupIndex];

                    // Assert
                    Assert.IsFalse(clearAllOutputItem.Enabled);
                }
            }
        }

        [Test]
        [TestCase(true, TestName = "Menu_ClickClearAllOutput_ClearAllOutputAfterConfirmation(true)")]
        [TestCase(false, TestName = "Menu_ClickClearAllOutput_ClearAllOutputAfterConfirmation(false)")]
        public void ContextMenuStrip_TwoCalculationsWithOutputClickOnClearAllOutput_OutputRemovedForCalculationsAfterConfirmation(bool confirm)
        {
            // Setup
            string hrdPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");

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
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);

            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, Path.Combine(hrdPath, "HRD ijsselmeer.sqlite"));

            var nodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(group,
                                                                                              failureMechanism,
                                                                                              assessmentSectionStub);
            var parentNodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                                    failureMechanism,
                                                                                                    assessmentSectionStub);

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

            using (var treeViewControl = new TreeViewControl())
            {
                var mainWindow = mocks.Stub<IMainWindow>();

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);

                mocks.ReplayAll();

                plugin.Gui = gui;

                using (new HydraRingCalculatorFactoryConfig())
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Call
                    contextMenu.Items[contextMenuClearOutputNestedGroupIndex].PerformClick();

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
                    ForeshoreProfile = new TestForeshoreProfile(true),
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
    }
}