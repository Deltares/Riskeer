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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
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
using NSubstitute;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms;
using Riskeer.Common.Service.TestUtil;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.GrassCoverErosionOutwards.Data.TestUtil;
using Riskeer.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;
using Riskeer.Revetment.Data;
using CoreGuiResources = Core.Gui.Properties.Resources;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.GrassCoverErosionOutwards.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class GrassCoverErosionOutwardsCalculationGroupContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuAddGenerateCalculationsIndex = 3;
        private const int contextMenuAddCalculationGroupIndexRootGroup = 5;
        private const int contextMenuAddCalculationIndexRootGroup = 6;
        private const int contextMenuUpdateForeshoreProfileIndexRootGroup = 8;
        private const int contextMenuValidateAllIndexRootGroup = 10;
        private const int contextMenuCalculateAllIndexRootGroup = 11;
        private const int contextMenuClearOutputIndexRootGroup = 13;
        private const int contextMenuRemoveAllChildrenIndexRootGroup = 14;
        private const int contextMenuCollapseAllIndexRootGroup = 16;
        private const int contextMenuExpandAllIndexRootGroup = 17;
        private const int contextMenuPropertiesIndexRootGroup = 19;

        private const int contextMenuDuplicateIndexNestedGroup = 3;
        private const int contextMenuAddCalculationGroupIndexNestedGroup = 5;
        private const int contextMenuAddCalculationIndexNestedGroup = 6;
        private const int contextMenuUpdateForeshoreProfileIndexNestedGroup = 9;
        private const int contextMenuValidateAllIndexNestedGroup = 11;
        private const int contextMenuCalculateAllIndexNestedGroup = 12;
        private const int contextMenuClearOutputIndexNestedGroup = 14;

        private const string expectedTextExpandAll = "Alles ui&tklappen";
        private const string expectedTextExpandAllToolTip = "Klap dit element en alle onderliggende elementen uit.";
        private const string expectedTextCollapseAll = "Alles i&nklappen";
        private const string expectedTextCollapseAllToolTip = "Klap dit element en alle onderliggende elementen in.";
        private const string expectedTextProperties = "Ei&genschappen";
        private const string expectedTextPropertiesToolTip = "Toon de eigenschappen in het Eigenschappenpaneel.";

        private IGui gui;
        private GrassCoverErosionOutwardsPlugin plugin;
        private TreeNodeInfo info;

        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Service, "HydraRingCalculation");
        private static readonly string validHlcdFilePath = Path.Combine(testDataPath, "hlcd.sqlite");
        private static readonly string validHrdFilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");
        private static readonly string validHrdFileVersion = "IJssel lake2016-07-04 16:187";

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
        public void Text_Always_ReturnGroupName()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var context = new GrassCoverErosionOutwardsCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                               null,
                                                                               failureMechanism,
                                                                               assessmentSection);

            // Call
            string text = info.Text(context);

            // Assert
            Assert.AreEqual("Hydraulische belastingen", text);
        }

        [Test]
        public void Image_Always_ReturnCalculationGroupIcon()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var context = new GrassCoverErosionOutwardsCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                               null,
                                                                               failureMechanism,
                                                                               assessmentSection);

            // Call
            Image icon = info.Image(context);

            // Assert
            TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.GeneralFolderIcon, icon);
        }

        [Test]
        public void ChildNodeObjects_EmptyGroup_ReturnEmpty()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var groupContext = new GrassCoverErosionOutwardsCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                                    null,
                                                                                    failureMechanism,
                                                                                    assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(groupContext);

            // Assert
            CollectionAssert.IsEmpty(children);
        }

        [Test]
        public void ChildNodeObjects_GroupWithChildren_ReturnChildren()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();

            var childGroup = new CalculationGroup();
            var calculationItem = new GrassCoverErosionOutwardsWaveConditionsCalculation();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationItem);
            failureMechanism.CalculationsGroup.Children.Add(childGroup);

            var nodeData = new GrassCoverErosionOutwardsCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                                null,
                                                                                failureMechanism,
                                                                                assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(nodeData).ToArray();

            // Assert
            Assert.AreEqual(failureMechanism.CalculationsGroup.Children.Count, children.Length);

            var returnedCalculationContext = (GrassCoverErosionOutwardsWaveConditionsCalculationContext) children[0];
            Assert.AreSame(calculationItem, returnedCalculationContext.WrappedData);
            Assert.AreSame(failureMechanism.CalculationsGroup, returnedCalculationContext.Parent);
            Assert.AreSame(failureMechanism, returnedCalculationContext.FailureMechanism);
            Assert.AreSame(assessmentSection, returnedCalculationContext.AssessmentSection);

            var returnedCalculationGroupContext = (GrassCoverErosionOutwardsCalculationGroupContext) children[1];
            Assert.AreSame(childGroup, returnedCalculationGroupContext.WrappedData);
            Assert.AreSame(failureMechanism.CalculationsGroup, returnedCalculationGroupContext.Parent);
            Assert.AreSame(failureMechanism, returnedCalculationGroupContext.FailureMechanism);
            Assert.AreSame(assessmentSection, returnedCalculationGroupContext.AssessmentSection);
        }

        [Test]
        public void ContextMenuStrip_WithoutParentNodeDefaultBehavior_CallsContextMenuBuilderMethods()
        {
            // Setup
            var group = new CalculationGroup();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();
            var groupContext = new GrassCoverErosionOutwardsCalculationGroupContext(group,
                                                                                    null,
                                                                                    failureMechanism,
                                                                                    assessmentSection);

            var menuBuilder = Substitute.For<IContextMenuBuilder>();
            menuBuilder.AddImportItem().Returns(menuBuilder);
            menuBuilder.AddExportItem().Returns(menuBuilder);
            menuBuilder.AddSeparator().Returns(menuBuilder);
            menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>()).Returns(menuBuilder);
            menuBuilder.AddDeleteChildrenItem().Returns(menuBuilder);
            menuBuilder.AddCollapseAllItem().Returns(menuBuilder);
            menuBuilder.AddExpandAllItem().Returns(menuBuilder);
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
                menuBuilder.AddImportItem();
                menuBuilder.AddExportItem();
                menuBuilder.AddSeparator();
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                menuBuilder.AddSeparator();
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                menuBuilder.AddSeparator();
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                menuBuilder.AddSeparator();
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                menuBuilder.AddSeparator();
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
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();
            var groupContext = new GrassCoverErosionOutwardsCalculationGroupContext(group,
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
                    Assert.AreEqual(20, menu.Items.Count);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddGenerateCalculationsIndex,
                                                                  "Genereer &berekeningen...",
                                                                  "Er is geen hydraulische belastingendatabase beschikbaar om de belastingenberekeningen te genereren.",
                                                                  RiskeerCommonFormsResources.GenerateScenariosIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationGroupIndexRootGroup,
                                                                  "&Map toevoegen",
                                                                  "Voeg een nieuwe map toe aan deze map met berekeningen.",
                                                                  RiskeerCommonFormsResources.AddFolderIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationIndexRootGroup,
                                                                  "Berekening &toevoegen",
                                                                  "Voeg een nieuwe berekening toe aan deze map met berekeningen.",
                                                                  RiskeerCommonFormsResources.HydraulicCalculationIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuUpdateForeshoreProfileIndexRootGroup,
                                                                  "&Bijwerken voorlandprofielen...",
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
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearOutputIndexRootGroup,
                                                                  "&Wis alle uitvoer...",
                                                                  "Er zijn geen berekeningen met uitvoer om te wissen.",
                                                                  RiskeerCommonFormsResources.ClearIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_NestedCalculationGroup_CallsContextMenuBuilderMethods()
        {
            // Setup
            var group = new CalculationGroup();
            var parentGroup = new CalculationGroup();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();
            var groupContext = new GrassCoverErosionOutwardsCalculationGroupContext(group,
                                                                                    parentGroup,
                                                                                    failureMechanism,
                                                                                    assessmentSection);
            var parentGroupContext = new GrassCoverErosionOutwardsCalculationGroupContext(parentGroup,
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
                menuBuilder.AddSeparator();
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                menuBuilder.AddSeparator();
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
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();
            var groupContext = new GrassCoverErosionOutwardsCalculationGroupContext(group,
                                                                                    parentGroup,
                                                                                    failureMechanism,
                                                                                    assessmentSection);
            var parentGroupContext = new GrassCoverErosionOutwardsCalculationGroupContext(parentGroup,
                                                                                          null,
                                                                                          failureMechanism,
                                                                                          assessmentSection);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            using (var treeViewControl = new TreeViewControl())
            {
                gui.Get(groupContext, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(groupContext, parentGroupContext, treeViewControl))
                {
                    // Assert
                    Assert.AreEqual(21, menu.Items.Count);

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
                                                                  RiskeerCommonFormsResources.HydraulicCalculationIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuUpdateForeshoreProfileIndexNestedGroup,
                                                                  "&Bijwerken voorlandprofielen...",
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
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearOutputIndexNestedGroup,
                                                                  "&Wis alle uitvoer...",
                                                                  "Er zijn geen berekeningen met uitvoer om te wissen.",
                                                                  RiskeerCommonFormsResources.ClearIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_WithoutParentNodeWithoutHydraulicLocationsDefaultBehavior_ReturnContextMenuWithoutRenameRemove()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            var nodeData = new GrassCoverErosionOutwardsCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                                null,
                                                                                failureMechanism,
                                                                                assessmentSection);

            var applicationFeatureCommandHandler = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            importCommandHandler.GetSupportedImportInfos(nodeData).Returns(new[]
            {
                new ImportInfo()
            });
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            exportCommandHandler.CanExportFrom(nodeData).Returns(true);
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommandsHandler = Substitute.For<IViewCommands>();
            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler,
                                                         importCommandHandler,
                                                         exportCommandHandler,
                                                         updateCommandHandler,
                                                         viewCommandsHandler,
                                                         nodeData,
                                                         treeViewControl);

                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
                gui.ViewCommands.Returns(Substitute.For<IViewCommands>());
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddGenerateCalculationsIndex,
                                                                  "Genereer &berekeningen...",
                                                                  "Er is geen hydraulische belastingendatabase beschikbaar om de belastingenberekeningen te genereren.",
                                                                  RiskeerCommonFormsResources.GenerateScenariosIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationGroupIndexRootGroup,
                                                                  "&Map toevoegen",
                                                                  "Voeg een nieuwe map toe aan deze map met berekeningen.",
                                                                  RiskeerCommonFormsResources.AddFolderIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationIndexRootGroup,
                                                                  "Berekening &toevoegen",
                                                                  "Voeg een nieuwe berekening toe aan deze map met berekeningen.",
                                                                  RiskeerCommonFormsResources.HydraulicCalculationIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuUpdateForeshoreProfileIndexRootGroup,
                                                                  "&Bijwerken voorlandprofielen...",
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
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearOutputIndexRootGroup,
                                                                  "&Wis alle uitvoer...",
                                                                  "Er zijn geen berekeningen met uitvoer om te wissen.",
                                                                  RiskeerCommonFormsResources.ClearIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuRemoveAllChildrenIndexRootGroup,
                                                                  "Ma&p leegmaken...",
                                                                  "Er zijn geen onderliggende elementen om te verwijderen.",
                                                                  CoreGuiResources.DeleteChildrenIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuExpandAllIndexRootGroup,
                                                                  expectedTextExpandAll,
                                                                  expectedTextExpandAllToolTip,
                                                                  CoreGuiResources.ExpandAllIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCollapseAllIndexRootGroup,
                                                                  expectedTextCollapseAll,
                                                                  expectedTextCollapseAllToolTip,
                                                                  CoreGuiResources.CollapseAllIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuPropertiesIndexRootGroup,
                                                                  expectedTextProperties,
                                                                  expectedTextPropertiesToolTip,
                                                                  CoreGuiResources.PropertiesHS,
                                                                  false);
                }

                importCommandHandler.Received().GetSupportedImportInfos(nodeData);
                exportCommandHandler.Received().CanExportFrom(nodeData);
            }
        }

        [Test]
        public void ContextMenuStrip_WithoutParentNodeWithHydraulicLocationsDefaultBehavior_ReturnContextMenuWithoutRenameRemove()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryData.Returns(new HydraulicBoundaryData
            {
                HydraulicBoundaryDatabases =
                {
                    new HydraulicBoundaryDatabase
                    {
                        Locations =
                        {
                            new TestHydraulicBoundaryLocation()
                        }
                    }
                }
            });

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            var nodeData = new GrassCoverErosionOutwardsCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                                null,
                                                                                failureMechanism,
                                                                                assessmentSection);

            var applicationFeatureCommandHandler = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            importCommandHandler.GetSupportedImportInfos(nodeData).Returns(new[]
            {
                new ImportInfo()
            });
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            exportCommandHandler.CanExportFrom(nodeData).Returns(true);
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommandsHandler = Substitute.For<IViewCommands>();
            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler,
                                                         importCommandHandler,
                                                         exportCommandHandler,
                                                         updateCommandHandler,
                                                         viewCommandsHandler,
                                                         nodeData,
                                                         treeViewControl);

                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
                gui.ViewCommands.Returns(Substitute.For<IViewCommands>());
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddGenerateCalculationsIndex,
                                                                  "Genereer &berekeningen...",
                                                                  "Genereer belastingenberekeningen.",
                                                                  RiskeerCommonFormsResources.GenerateScenariosIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationGroupIndexRootGroup,
                                                                  "&Map toevoegen",
                                                                  "Voeg een nieuwe map toe aan deze map met berekeningen.",
                                                                  RiskeerCommonFormsResources.AddFolderIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationIndexRootGroup,
                                                                  "Berekening &toevoegen",
                                                                  "Voeg een nieuwe berekening toe aan deze map met berekeningen.",
                                                                  RiskeerCommonFormsResources.HydraulicCalculationIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuRemoveAllChildrenIndexRootGroup,
                                                                  "Ma&p leegmaken...",
                                                                  "Er zijn geen onderliggende elementen om te verwijderen.",
                                                                  CoreGuiResources.DeleteChildrenIcon,
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
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearOutputIndexRootGroup,
                                                                  "&Wis alle uitvoer...",
                                                                  "Er zijn geen berekeningen met uitvoer om te wissen.",
                                                                  RiskeerCommonFormsResources.ClearIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuExpandAllIndexRootGroup,
                                                                  expectedTextExpandAll,
                                                                  expectedTextExpandAllToolTip,
                                                                  CoreGuiResources.ExpandAllIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCollapseAllIndexRootGroup,
                                                                  expectedTextCollapseAll,
                                                                  expectedTextCollapseAllToolTip,
                                                                  CoreGuiResources.CollapseAllIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuPropertiesIndexRootGroup,
                                                                  expectedTextProperties,
                                                                  expectedTextPropertiesToolTip,
                                                                  CoreGuiResources.PropertiesHS,
                                                                  false);
                }

                importCommandHandler.Received().GetSupportedImportInfos(nodeData);
                exportCommandHandler.Received().CanExportFrom(nodeData);
            }
        }

        [Test]
        public void ContextMenuStrip_NestedCalculationGroupWithoutCalculations_CalculateAllAndValidateAllDisabled()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var group = new CalculationGroup();

                var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
                failureMechanism.CalculationsGroup.Children.Add(group);

                IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();
                var nodeData = new GrassCoverErosionOutwardsCalculationGroupContext(group,
                                                                                    failureMechanism.CalculationsGroup,
                                                                                    failureMechanism,
                                                                                    assessmentSection);
                var parentNodeData = new GrassCoverErosionOutwardsCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                                          null,
                                                                                          failureMechanism,
                                                                                          assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Assert
                    ToolStripItem validateItem = contextMenu.Items[contextMenuValidateAllIndexNestedGroup];
                    ToolStripItem calculateItem = contextMenu.Items[contextMenuCalculateAllIndexNestedGroup];
                    Assert.IsFalse(validateItem.Enabled);
                    Assert.IsFalse(calculateItem.Enabled);
                    Assert.AreEqual("Er zijn geen berekeningen om uit te voeren.", calculateItem.ToolTipText);
                    Assert.AreEqual("Er zijn geen berekeningen om te valideren.", validateItem.ToolTipText);
                }
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ContextMenuStrip_AllRequiredInputSet_CalculateAllAndValidateAllEnabled(bool usePreprocessorClosure)
        {
            // Setup
            var group = new CalculationGroup();
            group.Children.Add(new GrassCoverErosionOutwardsWaveConditionsCalculation());

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(group);

            var assessmentSection = Substitute.For<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryData.Returns(new HydraulicBoundaryData());

            var nodeData = new GrassCoverErosionOutwardsCalculationGroupContext(group,
                                                                                failureMechanism.CalculationsGroup,
                                                                                failureMechanism,
                                                                                assessmentSection);
            var parentNodeData = new GrassCoverErosionOutwardsCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                                      null,
                                                                                      failureMechanism,
                                                                                      assessmentSection);
            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Assert
                    ToolStripItem validateItem = contextMenu.Items[contextMenuValidateAllIndexNestedGroup];
                    ToolStripItem calculateItem = contextMenu.Items[contextMenuCalculateAllIndexNestedGroup];
                    Assert.IsTrue(validateItem.Enabled);
                    Assert.IsTrue(calculateItem.Enabled);
                    Assert.AreEqual("Voer alle berekeningen binnen deze map met berekeningen uit.", calculateItem.ToolTipText);
                    Assert.AreEqual("Valideer alle berekeningen binnen deze map met berekeningen.", validateItem.ToolTipText);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_TwoCalculationsClickOnValidateAllInGroup_ValidationMessagesLogged()
        {
            // Setup
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();

            HydraulicBoundaryLocation hydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.GetLocations().First();

            assessmentSection.AddHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });
            ConfigureAssessmentSectionWithHydraulicBoundaryOutput(assessmentSection);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculationA = GetValidCalculation(hydraulicBoundaryLocation);
            GrassCoverErosionOutwardsWaveConditionsCalculation calculationB = GetValidCalculation(hydraulicBoundaryLocation);

            var group = new CalculationGroup();
            group.Children.Add(calculationA);
            group.Children.Add(calculationB);

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(group);

            var nodeData = new GrassCoverErosionOutwardsCalculationGroupContext(group,
                                                                                failureMechanism.CalculationsGroup,
                                                                                failureMechanism,
                                                                                assessmentSection);
            var parentNodeData = new GrassCoverErosionOutwardsCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                                      null,
                                                                                      failureMechanism,
                                                                                      assessmentSection);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Call
                    void Call() => contextMenu.Items[contextMenuValidateAllIndexNestedGroup].PerformClick();

                    // Assert
                    TestHelper.AssertLogMessages(Call, m =>
                    {
                        string[] messages = m.ToArray();
                        Assert.AreEqual(4, messages.Length);
                        CalculationServiceTestHelper.AssertValidationStartMessage(messages[0]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(messages[1]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(messages[2]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(messages[3]);
                    });
                }
            }
        }

        [Test]
        public void ContextMenuStrip_TwoCalculationsClickOnCalculateAllInGroup_MessagesLogged()
        {
            // Setup
            AssessmentSectionStub assessmentSection = CreateAssessmentSection();

            HydraulicBoundaryLocation hydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.GetLocations().First();

            assessmentSection.AddHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            ConfigureAssessmentSectionWithHydraulicBoundaryOutput(assessmentSection);

            var group = new CalculationGroup();
            GrassCoverErosionOutwardsWaveConditionsCalculation calculationA = GetValidCalculation(hydraulicBoundaryLocation);
            calculationA.Name = "A";
            GrassCoverErosionOutwardsWaveConditionsCalculation calculationB = GetValidCalculation(hydraulicBoundaryLocation);
            calculationB.Name = "B";
            group.Children.Add(calculationA);
            group.Children.Add(calculationB);

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(group);

            var nodeData = new GrassCoverErosionOutwardsCalculationGroupContext(group,
                                                                                failureMechanism.CalculationsGroup,
                                                                                failureMechanism,
                                                                                assessmentSection);
            var parentNodeData = new GrassCoverErosionOutwardsCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                                      null,
                                                                                      failureMechanism,
                                                                                      assessmentSection);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            using (var treeViewControl = new TreeViewControl())
            {
                IMainWindow mainWindow = MainWindowTestHelper.CreateMainWindowStub();

                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(mainWindow);

                var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();
                calculatorFactory.CreateWaveConditionsCosineCalculator(Arg.Any<HydraRingCalculationSettings>())
                                 .Returns(callInfo =>
                                 {
                                     HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                         HydraulicBoundaryCalculationSettingsFactory.CreateSettings(
                                             assessmentSection.HydraulicBoundaryData,
                                             hydraulicBoundaryLocation),
                                         (HydraRingCalculationSettings) callInfo[0]);
                                     return new TestWaveConditionsCosineCalculator();
                                 });

                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Call
                    void Call() => contextMenu.Items[contextMenuCalculateAllIndexNestedGroup].PerformClick();

                    // Assert
                    TestHelper.AssertLogMessages(Call, m =>
                    {
                        string[] messages = m.ToArray();
                        Assert.AreEqual(56, messages.Length);
                        Assert.AreEqual("Golfcondities berekenen voor 'A' is gestart.", messages[0]);
                        CalculationServiceTestHelper.AssertCalculationStartMessage(messages[3]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(messages[26]);
                        Assert.AreEqual("Golfcondities berekenen voor 'A' is gelukt.", messages[27]);

                        Assert.AreEqual("Golfcondities berekenen voor 'B' is gestart.", messages[28]);
                        CalculationServiceTestHelper.AssertCalculationStartMessage(messages[31]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(messages[54]);
                        Assert.AreEqual("Golfcondities berekenen voor 'B' is gelukt.", messages[55]);
                    });
                }

                Assert.AreEqual(3, calculationA.Output.WaveRunUpOutput.Count());
                Assert.AreEqual(3, calculationB.Output.WaveRunUpOutput.Count());
            }
        }

        [Test]
        public void ContextMenuStrip_NoCalculations_ClearAllOutputItemDisabled()
        {
            // Setup
            var group = new CalculationGroup();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(group);

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, validHrdFilePath);

            var nodeData = new GrassCoverErosionOutwardsCalculationGroupContext(group,
                                                                                failureMechanism.CalculationsGroup,
                                                                                failureMechanism,
                                                                                assessmentSection);
            var parentNodeData = new GrassCoverErosionOutwardsCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                                      null,
                                                                                      failureMechanism,
                                                                                      assessmentSection);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                var mainWindow = Substitute.For<IMainWindow>();

                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(mainWindow);

                var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();

                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Call
                    ToolStripItem clearAllOutputItem = contextMenu.Items[contextMenuClearOutputIndexNestedGroup];

                    // Assert
                    Assert.IsFalse(clearAllOutputItem.Enabled);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_TwoCalculationsWithoutOutput_ClearAllOutputItemDisabled()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, validHrdFilePath);

            HydraulicBoundaryData hydraulicBoundaryData = assessmentSection.HydraulicBoundaryData;
            HydraulicBoundaryLocation hydraulicBoundaryLocation = hydraulicBoundaryData.GetLocations().First();

            var group = new CalculationGroup();
            GrassCoverErosionOutwardsWaveConditionsCalculation calculationA = GetValidCalculation(hydraulicBoundaryLocation);
            GrassCoverErosionOutwardsWaveConditionsCalculation calculationB = GetValidCalculation(hydraulicBoundaryLocation);
            group.Children.Add(calculationA);
            group.Children.Add(calculationB);

            failureMechanism.CalculationsGroup.Children.Add(group);

            var nodeData = new GrassCoverErosionOutwardsCalculationGroupContext(group,
                                                                                failureMechanism.CalculationsGroup,
                                                                                failureMechanism,
                                                                                assessmentSection);
            var parentNodeData = new GrassCoverErosionOutwardsCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                                      null,
                                                                                      failureMechanism,
                                                                                      assessmentSection);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                var mainWindow = Substitute.For<IMainWindow>();

                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(mainWindow);

                var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();

                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Call
                    ToolStripItem clearAllOutputItem = contextMenu.Items[contextMenuClearOutputIndexNestedGroup];

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
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, validHrdFilePath);

            var observerA = Substitute.For<IObserver>();
            var observerB = Substitute.For<IObserver>();

            HydraulicBoundaryLocation hydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.GetLocations().First();

            var group = new CalculationGroup();
            GrassCoverErosionOutwardsWaveConditionsCalculation calculationA = GetValidCalculation(hydraulicBoundaryLocation);
            calculationA.Output = GrassCoverErosionOutwardsWaveConditionsOutputTestFactory.Create();
            GrassCoverErosionOutwardsWaveConditionsCalculation calculationB = GetValidCalculation(hydraulicBoundaryLocation);
            calculationB.Output = GrassCoverErosionOutwardsWaveConditionsOutputTestFactory.Create();
            group.Children.Add(calculationA);
            group.Children.Add(calculationB);
            calculationA.Attach(observerA);
            calculationB.Attach(observerB);

            failureMechanism.CalculationsGroup.Children.Add(group);

            var nodeData = new GrassCoverErosionOutwardsCalculationGroupContext(group,
                                                                                failureMechanism.CalculationsGroup,
                                                                                failureMechanism,
                                                                                assessmentSection);
            var parentNodeData = new GrassCoverErosionOutwardsCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                                      null,
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

            using (var treeViewControl = new TreeViewControl())
            {
                var mainWindow = Substitute.For<IMainWindow>();

                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(mainWindow);

                var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();

                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Call
                    contextMenu.Items[contextMenuClearOutputIndexNestedGroup].PerformClick();

                    // Assert
                    if (confirm)
                    {
                        Assert.IsNull(calculationA.Output);
                        Assert.IsNull(calculationB.Output);
                        observerA.Received().UpdateObserver();
                        observerB.Received().UpdateObserver();
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
                IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();

                var group = new CalculationGroup();

                var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
                failureMechanism.CalculationsGroup.Children.Add(group);

                var nodeData = new GrassCoverErosionOutwardsCalculationGroupContext(group,
                                                                                    failureMechanism.CalculationsGroup,
                                                                                    failureMechanism,
                                                                                    assessmentSection);
                var parentNodeData = new GrassCoverErosionOutwardsCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                                          null,
                                                                                          failureMechanism,
                                                                                          assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                var observer = Substitute.For<IObserver>();

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
                    observer.Received().UpdateObserver();
                }
            }
        }

        [Test]
        [TestCase(NormativeProbabilityType.SignalFloodingProbability)]
        [TestCase(NormativeProbabilityType.MaximumAllowableFloodingProbability)]
        public void ContextMenuStrip_ClickOnAddCalculationItem_AddCalculationToCalculationGroupAndNotifyObservers(
            NormativeProbabilityType normativeProbabilityType)
        {
            // Setup
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            var group = new CalculationGroup();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var assessmentSection = new AssessmentSectionStub
            {
                FailureMechanismContribution =
                {
                    NormativeProbabilityType = normativeProbabilityType
                }
            };
            var nodeData = new GrassCoverErosionOutwardsCalculationGroupContext(group,
                                                                                null,
                                                                                failureMechanism,
                                                                                assessmentSection);
            var calculationItem = new GrassCoverErosionOutwardsWaveConditionsCalculation
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

                    var newCalculationItem = newlyAddedItem as GrassCoverErosionOutwardsWaveConditionsCalculation;
                    Assert.IsNotNull(newCalculationItem);
                    Assert.AreEqual("Nieuwe berekening (1)", newlyAddedItem.Name,
                                    "An item with the same name default name already exists, therefore '(1)' needs to be appended.");
                    Assert.AreEqual(GetWaterLevelTypeFromNormativeProbabilityType(normativeProbabilityType), newCalculationItem.InputParameters.WaterLevelType);
                    observer.Received().UpdateObserver();
                }
            }
        }

        [Test]
        public void ContextMenuStrip_WithForeshoreProfileAndChanges_ContextMenuItemUpdateAllForeshoreProfilesEnabled()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile(),
                    UseBreakWater = true
                }
            };

            var nodeData = new GrassCoverErosionOutwardsCalculationGroupContext(
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
        public void GivenCalculationWithForeshoreProfileSet_WhenUpdatingForeshoreProfileFromContextMenu_ThenCalculationUpdatedAndUpdateObserver()
        {
            // Given
            var calculationObserver = Substitute.For<IObserver>();
            var calculationInputObserver = Substitute.For<IObserver>();

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile(true),
                    UseBreakWater = false
                }
            };

            var nodeData = new GrassCoverErosionOutwardsCalculationGroupContext(
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

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    contextMenuStrip.Items[contextMenuUpdateForeshoreProfileIndexRootGroup].PerformClick();

                    // Then
                    Assert.IsTrue(calculation.InputParameters.IsForeshoreProfileInputSynchronized);
                }
            }

            calculationInputObserver.Received().UpdateObserver();
        }

        [Test]
        public void GivenCalculationWithoutOutput_ThenClearOutputItemDisabled()
        {
            // Given
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();

            var context = new GrassCoverErosionOutwardsCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                               null,
                                                                               failureMechanism,
                                                                               assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var appFeatureCommandHandler = Substitute.For<IApplicationFeatureCommands>();
                var importHandler = Substitute.For<IImportCommandHandler>();
                importHandler.GetSupportedImportInfos(context).Returns(Array.Empty<ImportInfo>());
                var exportHandler = Substitute.For<IExportCommandHandler>();
                var updateHandler = Substitute.For<IUpdateCommandHandler>();
                var viewCommands = Substitute.For<IViewCommands>();
                var menuBuilder = new ContextMenuBuilder(appFeatureCommandHandler,
                                                         importHandler,
                                                         exportHandler,
                                                         updateHandler,
                                                         viewCommands,
                                                         context,
                                                         treeViewControl);

                gui.ViewCommands.Returns(viewCommands);
                gui.Get(context, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                // When
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Then
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  contextMenuAddCalculationIndexRootGroup,
                                                                  "Berekening &toevoegen",
                                                                  "Voeg een nieuwe berekening toe aan deze map met berekeningen.",
                                                                  RiskeerCommonFormsResources.HydraulicCalculationIcon);
                }
            }
        }

        [Test]
        public void GivenCalculationWithOutput_WhenClearingOutput_ThenClearOutput()
        {
            // Given
            var observer = Substitute.For<IObserver>();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Attach(observer);

            var assessmentSection = new AssessmentSectionStub();
            var context = new GrassCoverErosionOutwardsCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                               null,
                                                                               failureMechanism,
                                                                               assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var appFeatureCommandHandler = Substitute.For<IApplicationFeatureCommands>();
                var importHandler = Substitute.For<IImportCommandHandler>();
                importHandler.GetSupportedImportInfos(context).Returns(Array.Empty<ImportInfo>());
                var exportHandler = Substitute.For<IExportCommandHandler>();
                var updateHandler = Substitute.For<IUpdateCommandHandler>();
                var viewCommands = Substitute.For<IViewCommands>();
                var menuBuilder = new ContextMenuBuilder(appFeatureCommandHandler,
                                                         importHandler,
                                                         exportHandler,
                                                         updateHandler,
                                                         viewCommands,
                                                         context,
                                                         treeViewControl);

                gui.ViewCommands.Returns(viewCommands);
                gui.Get(context, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // When
                    ToolStripItem validateMenuItem = contextMenu.Items[contextMenuAddCalculationIndexRootGroup];
                    validateMenuItem.PerformClick();

                    // Then
                    Assert.AreEqual(1, failureMechanism.CalculationsGroup.Children.Count);
                    Assert.IsInstanceOf<GrassCoverErosionOutwardsWaveConditionsCalculation>(failureMechanism.CalculationsGroup.Children[0]);
                    observer.Received().UpdateObserver();
                }
            }
        }

        [Test]
        public void OnNodeRemoved_ParentIsCalculationGroupContainingGroup_RemoveGroupAndNotifyObservers()
        {
            // Setup
            var observer = Substitute.For<IObserver>();

            var assessmentSection = Substitute.For<IAssessmentSection>();

            var group = new CalculationGroup();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            var nodeData = new GrassCoverErosionOutwardsCalculationGroupContext(group,
                                                                                failureMechanism.CalculationsGroup,
                                                                                failureMechanism,
                                                                                assessmentSection);

            failureMechanism.CalculationsGroup.Children.Add(group);
            var parentNodeData = new GrassCoverErosionOutwardsCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                                      null,
                                                                                      failureMechanism,
                                                                                      assessmentSection);
            parentNodeData.Attach(observer);

            // Precondition
            Assert.IsTrue(info.CanRemove(nodeData, parentNodeData));

            // Call
            info.OnNodeRemoved(nodeData, parentNodeData);

            // Assert
            CollectionAssert.DoesNotContain(failureMechanism.CalculationsGroup.Children, group);
            observer.Received().UpdateObserver();
        }

        [Test]
        public void GivenDialogGenerateCalculationButtonClicked_WhenCalculationSelectedAndDialogClosed_ThenUpdateCalculationGroup()
        {
            // Given
            var random = new Random(21);
            var normativeProbabilityType = random.NextEnumValue<NormativeProbabilityType>();

            using (var treeViewControl = new TreeViewControl())
            {
                var existingGroup = new CalculationGroup();
                var existingCalculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();
                var group = new CalculationGroup
                {
                    Children =
                    {
                        existingGroup,
                        existingCalculation
                    }
                };
                var hydraulicBoundaryLocation1 = new TestHydraulicBoundaryLocation();
                var hydraulicBoundaryLocation2 = new TestHydraulicBoundaryLocation();
                var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
                var assessmentSection = new AssessmentSectionStub
                {
                    FailureMechanismContribution =
                    {
                        NormativeProbabilityType = normativeProbabilityType
                    },
                    HydraulicBoundaryData =
                    {
                        HydraulicBoundaryDatabases =
                        {
                            new HydraulicBoundaryDatabase
                            {
                                Locations =
                                {
                                    hydraulicBoundaryLocation1,
                                    hydraulicBoundaryLocation2
                                }
                            }
                        }
                    }
                };
                assessmentSection.AddHydraulicBoundaryLocationCalculations(new[]
                {
                    hydraulicBoundaryLocation1,
                    hydraulicBoundaryLocation2
                });

                var observer = Substitute.For<IObserver>();
                var nodeData = new GrassCoverErosionOutwardsCalculationGroupContext(group,
                                                                                    null,
                                                                                    failureMechanism,
                                                                                    assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var mainWindow = Substitute.For<IMainWindow>();
                var viewCommands = Substitute.For<IViewCommands>();

                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(mainWindow);
                gui.ViewCommands.Returns(viewCommands);

                nodeData.Attach(observer);

                HydraulicBoundaryLocationSelectionDialog dialog = null;
                DataGridViewControl grid = null;
                DialogBoxHandler = (name, wnd) =>
                {
                    dialog = (HydraulicBoundaryLocationSelectionDialog) new FormTester(name).TheObject;
                    grid = (DataGridViewControl) new ControlTester("DataGridViewControl", dialog).TheObject;
                    grid.Rows[0].Cells[0].Value = true;
                    grid.Rows[1].Cells[0].Value = true;
                    new ButtonTester("DoForSelectedButton", dialog).Click();
                };

                // When
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                    contextMenu.Items[contextMenuAddGenerateCalculationsIndex].PerformClick();

                // Then
                Assert.AreEqual(4, group.Children.Count);
                Assert.AreSame(existingGroup, group.Children[0]);
                Assert.AreSame(existingCalculation, group.Children[1]);
                Assert.NotNull(dialog);
                Assert.NotNull(grid);

                WaveConditionsInputWaterLevelType expectedWaveConditionsInputWaterLevelType = GetWaterLevelTypeFromNormativeProbabilityType(normativeProbabilityType);
                var firstCalculation = group.Children[2] as GrassCoverErosionOutwardsWaveConditionsCalculation;
                Assert.IsNotNull(firstCalculation);
                GrassCoverErosionOutwardsWaveConditionsInput firstCalculationInput = firstCalculation.InputParameters;
                Assert.AreSame(hydraulicBoundaryLocation1, firstCalculationInput.HydraulicBoundaryLocation);
                Assert.AreEqual(expectedWaveConditionsInputWaterLevelType, firstCalculationInput.WaterLevelType);

                var secondCalculation = group.Children[3] as GrassCoverErosionOutwardsWaveConditionsCalculation;
                Assert.IsNotNull(secondCalculation);
                GrassCoverErosionOutwardsWaveConditionsInput secondCalculationInput = secondCalculation.InputParameters;
                Assert.AreSame(hydraulicBoundaryLocation2, secondCalculationInput.HydraulicBoundaryLocation);
                Assert.AreEqual(expectedWaveConditionsInputWaterLevelType, secondCalculationInput.WaterLevelType);
                observer.Received().UpdateObserver();
            }
        }

        [Test]
        public void GivenDialogGenerateCalculationButtonClicked_WhenCancelButtonClickedAndDialogClosed_ThenCalculationGroupNotUpdated()
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                var group = new CalculationGroup();
                var hydraulicBoundaryLocation1 = new TestHydraulicBoundaryLocation();
                var hydraulicBoundaryLocation2 = new TestHydraulicBoundaryLocation();
                var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
                var assessmentSection = new AssessmentSectionStub
                {
                    HydraulicBoundaryData =
                    {
                        HydraulicBoundaryDatabases =
                        {
                            new HydraulicBoundaryDatabase
                            {
                                Locations =
                                {
                                    hydraulicBoundaryLocation1,
                                    hydraulicBoundaryLocation2
                                }
                            }
                        }
                    }
                };
                assessmentSection.AddHydraulicBoundaryLocationCalculations(new[]
                {
                    hydraulicBoundaryLocation1,
                    hydraulicBoundaryLocation2
                });

                var nodeData = new GrassCoverErosionOutwardsCalculationGroupContext(group,
                                                                                    null,
                                                                                    failureMechanism,
                                                                                    assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var mainWindow = Substitute.For<IMainWindow>();
                var viewCommands = Substitute.For<IViewCommands>();

                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(mainWindow);
                gui.ViewCommands.Returns(viewCommands);

                HydraulicBoundaryLocationSelectionDialog dialog = null;
                DataGridViewControl grid = null;
                DialogBoxHandler = (name, wnd) =>
                {
                    dialog = (HydraulicBoundaryLocationSelectionDialog) new FormTester(name).TheObject;
                    grid = (DataGridViewControl) new ControlTester("DataGridViewControl", dialog).TheObject;
                    grid.Rows[0].Cells[0].Value = true;
                    grid.Rows[1].Cells[0].Value = true;
                    new ButtonTester("CustomCancelButton", dialog).Click();
                };

                // When
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                    contextMenu.Items[contextMenuAddGenerateCalculationsIndex].PerformClick();

                // Then
                Assert.AreEqual(0, group.Children.Count);
                Assert.NotNull(dialog);
                Assert.NotNull(grid);
            }
        }

        public override void Setup()
        {
            gui = Substitute.For<IGui>();
            plugin = new GrassCoverErosionOutwardsPlugin
            {
                Gui = gui
            };
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(GrassCoverErosionOutwardsCalculationGroupContext));
        }

        public override void TearDown()
        {
            plugin.Dispose();

            base.TearDown();
        }

        private static AssessmentSectionStub CreateAssessmentSection(bool usePreprocessorClosure = false)
        {
            return new AssessmentSectionStub
            {
                HydraulicBoundaryData =
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
                            UsePreprocessorClosure = usePreprocessorClosure,
                            Locations =
                            {
                                new TestHydraulicBoundaryLocation()
                            }
                        }
                    }
                }
            };
        }

        private static void ConfigureAssessmentSectionWithHydraulicBoundaryOutput(IAssessmentSection assessmentSection)
        {
            assessmentSection.WaterLevelCalculationsForMaximumAllowableFloodingProbability.First().Output = new TestHydraulicBoundaryLocationCalculationOutput(9.3);
        }

        private static GrassCoverErosionOutwardsWaveConditionsCalculation GetValidCalculation(HydraulicBoundaryLocation location)
        {
            return new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = location,
                    WaterLevelType = WaveConditionsInputWaterLevelType.MaximumAllowableFloodingProbability,
                    ForeshoreProfile = new TestForeshoreProfile(true),
                    UseForeshore = true,
                    UseBreakWater = true,
                    LowerBoundaryRevetment = (RoundedDouble) 4,
                    UpperBoundaryRevetment = (RoundedDouble) 10,
                    UpperBoundaryWaterLevels = (RoundedDouble) 8,
                    LowerBoundaryWaterLevels = (RoundedDouble) 7.1
                }
            };
        }

        private static WaveConditionsInputWaterLevelType GetWaterLevelTypeFromNormativeProbabilityType(NormativeProbabilityType normativeProbabilityType)
        {
            switch (normativeProbabilityType)
            {
                case NormativeProbabilityType.MaximumAllowableFloodingProbability:
                    return WaveConditionsInputWaterLevelType.MaximumAllowableFloodingProbability;
                case NormativeProbabilityType.SignalFloodingProbability:
                    return WaveConditionsInputWaterLevelType.SignalFloodingProbability;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}