﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.Revetment.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.GrassCoverErosionOutwards.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveConditionsCalculationGroupContextTreeNodeInfoTest : NUnitFormTest
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

        private const int contextMenuAddCalculationGroupIndexNestedGroup = 3;
        private const int contextMenuAddCalculationIndexNestedGroup = 4;
        private const int contextMenuUpdateForeshoreProfileIndexNestedGroup = 7;
        private const int contextMenuValidateAllIndexNestedGroup = 9;
        private const int contextMenuCalculateAllIndexNestedGroup = 10;
        private const int contextMenuClearOutputIndexNestedGroup = 12;

        private const string expectedTextExpandAll = "Alles ui&tklappen";
        private const string expectedTextExpandAllToolTip = "Klap dit element en alle onderliggende elementen uit.";
        private const string expectedTextCollapseAll = "Alles i&nklappen";
        private const string expectedTextCollapseAllToolTip = "Klap dit element en alle onderliggende elementen in.";
        private const string expectedTextProperties = "Ei&genschappen";
        private const string expectedTextPropertiesToolTip = "Toon de eigenschappen in het Eigenschappenpaneel.";

        private IGui gui;
        private MockRepository mocks;
        private GrassCoverErosionOutwardsPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            gui = mocks.Stub<IGui>();
            plugin = new GrassCoverErosionOutwardsPlugin
            {
                Gui = gui
            };
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
            Assert.IsNull(info.IsChecked);
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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
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
            object[] children = info.ChildNodeObjects(groupContext);

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
            object[] children = info.ChildNodeObjects(nodeData).ToArray();

            // Assert
            Assert.AreEqual(failureMechanism.WaveConditionsCalculationGroup.Children.Count, children.Length);
            Assert.AreSame(calculationItem, children[0]);
            var returnedCalculationGroupContext = (GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext) children[1];
            Assert.AreSame(childGroup, returnedCalculationGroupContext.WrappedData);
            Assert.AreSame(failureMechanism, returnedCalculationGroupContext.FailureMechanism);
        }

        [Test]
        public void ContextMenuStrip_WithoutParentNodeDefaultBehavior_CallsContextMenuBuilderMethods()
        {
            // Setup
            var group = new CalculationGroup();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var groupContext = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(group,
                                                                                                  failureMechanism,
                                                                                                  assessmentSection);

            var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();
            using (mocks.Ordered())
            {
                menuBuilder.Expect(mb => mb.AddImportItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddExportItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddDeleteChildrenItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.Build()).Return(null);
            }

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(groupContext, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                // Call
                info.ContextMenuStrip(groupContext, null, treeViewControl);
            }
            // Assert
            // Assert expectancies called in TearDown()
        }

        [Test]
        public void ContextMenuStrip_WithoutParentNodeDefaultBehavior_AddCustomItems()
        {
            // Setup
            var group = new CalculationGroup();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var groupContext = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(group,
                                                                                                  failureMechanism,
                                                                                                  assessmentSection);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(groupContext, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(groupContext, null, treeViewControl))
                {
                    // Assert
                    Assert.AreEqual(20, menu.Items.Count);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddGenerateCalculationsIndex,
                                                                  "Genereer &berekeningen...",
                                                                  "Er is geen hydraulische randvoorwaardendatabase beschikbaar om de randvoorwaardenberekeningen te genereren.",
                                                                  RingtoetsCommonFormsResources.GenerateScenariosIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationGroupIndexRootGroup,
                                                                  "&Map toevoegen",
                                                                  "Voeg een nieuwe map toe aan deze map met berekeningen.",
                                                                  RingtoetsCommonFormsResources.AddFolderIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationIndexRootGroup,
                                                                  "Berekening &toevoegen",
                                                                  "Voeg een nieuwe berekening toe aan deze map met berekeningen.",
                                                                  RingtoetsCommonFormsResources.FailureMechanismIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuUpdateForeshoreProfileIndexRootGroup,
                                                                  "&Bijwerken voorlandprofielen...",
                                                                  "Er zijn geen berekeningen om bij te werken.",
                                                                  RingtoetsCommonFormsResources.UpdateItemIcon,
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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var groupContext = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(group,
                                                                                                  failureMechanism,
                                                                                                  assessmentSection);
            var parentGroupContext = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(parentGroup,
                                                                                                        failureMechanism,
                                                                                                        assessmentSection);

            var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();
            using (mocks.Ordered())
            {
                menuBuilder.Expect(mb => mb.AddImportItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddExportItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddRenameItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddDeleteItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.Build()).Return(null);
            }

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(cmp => cmp.Get(groupContext, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                // Call
                info.ContextMenuStrip(groupContext, parentGroupContext, treeViewControl);
            }
            // Assert
            // Assert expectancies called in TearDown()
        }

        [Test]
        public void ContextMenuStrip_NestedCalculationGroup_AddCustomItems()
        {
            // Setup
            var group = new CalculationGroup();
            var parentGroup = new CalculationGroup();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var groupContext = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(group,
                                                                                                  failureMechanism,
                                                                                                  assessmentSection);
            var parentGroupContext = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(parentGroup,
                                                                                                        failureMechanism,
                                                                                                        assessmentSection);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(groupContext, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());

                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(groupContext, parentGroupContext, treeViewControl))
                {
                    // Assert
                    Assert.AreEqual(19, menu.Items.Count);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationGroupIndexNestedGroup,
                                                                  "&Map toevoegen",
                                                                  "Voeg een nieuwe map toe aan deze map met berekeningen.",
                                                                  RingtoetsCommonFormsResources.AddFolderIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationIndexNestedGroup,
                                                                  "Berekening &toevoegen",
                                                                  "Voeg een nieuwe berekening toe aan deze map met berekeningen.",
                                                                  RingtoetsCommonFormsResources.FailureMechanismIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuUpdateForeshoreProfileIndexNestedGroup,
                                                                  "&Bijwerken voorlandprofielen...",
                                                                  "Er zijn geen berekeningen om bij te werken.",
                                                                  RingtoetsCommonFormsResources.UpdateItemIcon,
                                                                  false);
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
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearOutputIndexNestedGroup,
                                                                  "&Wis alle uitvoer...",
                                                                  "Er zijn geen berekeningen met uitvoer om te wissen.",
                                                                  RingtoetsCommonFormsResources.ClearIcon,
                                                                  false);
                }
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
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            importCommandHandler.Expect(ihm => ihm.CanImportOn(nodeData)).Return(true);
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            exportCommandHandler.Expect(ehm => ehm.CanExportFrom(nodeData)).Return(true);
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommandsHandler = mocks.StrictMock<IViewCommands>();
            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler,
                                                         importCommandHandler,
                                                         exportCommandHandler,
                                                         updateCommandHandler,
                                                         viewCommandsHandler,
                                                         nodeData,
                                                         treeViewControl);

                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddGenerateCalculationsIndex,
                                                                  "Genereer &berekeningen...",
                                                                  "Er is geen hydraulische randvoorwaardendatabase beschikbaar om de randvoorwaardenberekeningen te genereren.",
                                                                  RingtoetsCommonFormsResources.GenerateScenariosIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationGroupIndexRootGroup,
                                                                  "&Map toevoegen",
                                                                  "Voeg een nieuwe map toe aan deze map met berekeningen.",
                                                                  RingtoetsCommonFormsResources.AddFolderIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationIndexRootGroup,
                                                                  "Berekening &toevoegen",
                                                                  "Voeg een nieuwe berekening toe aan deze map met berekeningen.",
                                                                  RingtoetsCommonFormsResources.FailureMechanismIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuUpdateForeshoreProfileIndexRootGroup,
                                                                  "&Bijwerken voorlandprofielen...",
                                                                  "Er zijn geen berekeningen om bij te werken.",
                                                                  RingtoetsCommonFormsResources.UpdateItemIcon,
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
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuRemoveAllChildrenIndexRootGroup,
                                                                  "Ma&p leegmaken...",
                                                                  "Er zijn geen onderliggende elementen om te verwijderen.",
                                                                  CoreCommonGuiResources.DeleteChildrenIcon,
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
                    new TestHydraulicBoundaryLocation()
                }
            };

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            var nodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                              failureMechanism,
                                                                                              assessmentSection);

            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            importCommandHandler.Expect(ihm => ihm.CanImportOn(nodeData)).Return(true);
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            exportCommandHandler.Expect(ehm => ehm.CanExportFrom(nodeData)).Return(true);
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommandsHandler = mocks.StrictMock<IViewCommands>();
            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler,
                                                         importCommandHandler,
                                                         exportCommandHandler,
                                                         updateCommandHandler,
                                                         viewCommandsHandler,
                                                         nodeData,
                                                         treeViewControl);

                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddGenerateCalculationsIndex,
                                                                  "Genereer &berekeningen...",
                                                                  "Genereer randvoorwaardenberekeningen.",
                                                                  RingtoetsCommonFormsResources.GenerateScenariosIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationGroupIndexRootGroup,
                                                                  "&Map toevoegen",
                                                                  "Voeg een nieuwe map toe aan deze map met berekeningen.",
                                                                  RingtoetsCommonFormsResources.AddFolderIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationIndexRootGroup,
                                                                  "Berekening &toevoegen",
                                                                  "Voeg een nieuwe berekening toe aan deze map met berekeningen.",
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
                }
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
                failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);

                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var nodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(group,
                                                                                                  failureMechanism,
                                                                                                  assessmentSection);
                var parentNodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                                        failureMechanism,
                                                                                                        assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

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
        public void ContextMenuStrip_NoHydraulicBoundaryDatabase_CalculateAllAndValidateAllDisabled()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var group = new CalculationGroup();
                group.Children.Add(new GrassCoverErosionOutwardsWaveConditionsCalculation());

                var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
                {
                    Contribution = 5
                };
                failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);

                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var nodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(group,
                                                                                                  failureMechanism,
                                                                                                  assessmentSection);
                var parentNodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                                        failureMechanism,
                                                                                                        assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

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
        public void ContextMenuStrip_HydraulicBoundaryDatabaseNotValid_CalculateAllAndValidateAllDisabled()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var group = new CalculationGroup();
                group.Children.Add(new GrassCoverErosionOutwardsWaveConditionsCalculation());

                var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
                {
                    Contribution = 5
                };
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

                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Assert
                    ToolStripItem validateItem = contextMenu.Items[contextMenuValidateAllIndexNestedGroup];
                    ToolStripItem calculateItem = contextMenu.Items[contextMenuCalculateAllIndexNestedGroup];
                    Assert.IsFalse(validateItem.Enabled);
                    Assert.IsFalse(calculateItem.Enabled);
                    const string message = "Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt. Fout bij het lezen van bestand '': bestandspad mag niet leeg of ongedefinieerd zijn.";
                    Assert.AreEqual(message, calculateItem.ToolTipText);
                    Assert.AreEqual(message, validateItem.ToolTipText);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismContributionZero_CalculateAllAndValidateAllDisabled()
        {
            // Setup
            string validHydroDatabasePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                       Path.Combine("HydraulicBoundaryDatabaseImporter", "complete.sqlite"));

            using (var treeViewControl = new TreeViewControl())
            {
                var group = new CalculationGroup();
                group.Children.Add(new GrassCoverErosionOutwardsWaveConditionsCalculation());

                var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
                {
                    Contribution = 0
                };
                failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);

                var assessmentSection = mocks.Stub<IAssessmentSection>();
                assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
                {
                    FilePath = validHydroDatabasePath
                };
                var nodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(group,
                                                                                                  failureMechanism,
                                                                                                  assessmentSection);
                var parentNodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                                        failureMechanism,
                                                                                                        assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
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
                    const string message = "De bijdrage van dit toetsspoor is nul.";
                    Assert.AreEqual(message, calculateItem.ToolTipText);
                    Assert.AreEqual(message, validateItem.ToolTipText);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_AllRequiredInputSet_CalculateAllAndValidateAllEnabled()
        {
            // Setup
            string validHydroDatabasePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                       Path.Combine("HydraulicBoundaryDatabaseImporter", "complete.sqlite"));

            using (var treeViewControl = new TreeViewControl())
            {
                var group = new CalculationGroup();
                group.Children.Add(new GrassCoverErosionOutwardsWaveConditionsCalculation());

                var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
                {
                    Contribution = 5
                };
                failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);

                var assessmentSection = mocks.Stub<IAssessmentSection>();
                assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
                {
                    FilePath = validHydroDatabasePath
                };
                var nodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(group,
                                                                                                  failureMechanism,
                                                                                                  assessmentSection);
                var parentNodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                                        failureMechanism,
                                                                                                        assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

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
            string hrdPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");

            var calculationA = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Name = "A",
                InputParameters =
                {
                    HydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(12),
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
                    HydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(12),
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

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 5
            };
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);

            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, Path.Combine(hrdPath, "HRD ijsselmeer.sqlite"));

            var nodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(group,
                                                                                              failureMechanism,
                                                                                              assessmentSection);
            var parentNodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                                    failureMechanism,
                                                                                                    assessmentSection);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Call
                    Action test = () => contextMenu.Items[contextMenuValidateAllIndexNestedGroup].PerformClick();

                    // Assert
                    TestHelper.AssertLogMessages(test, m =>
                    {
                        string[] messages = m.ToArray();
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
            GrassCoverErosionOutwardsWaveConditionsCalculation calculationA = GetValidCalculation();
            GrassCoverErosionOutwardsWaveConditionsCalculation calculationB = GetValidCalculation();
            calculationA.Attach(observerA);
            calculationB.Attach(observerB);
            group.Children.Add(calculationA);
            group.Children.Add(calculationB);

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 10
            };
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);

            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, Path.Combine(hrdPath, "HRD ijsselmeer.sqlite"));

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

            using (var treeViewControl = new TreeViewControl())
            {
                var mainWindow = mocks.Stub<IMainWindow>();

                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);

                var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Stub(cf => cf.CreateWaveConditionsCosineCalculator(hrdPath)).Return(new TestWaveConditionsCosineCalculator());
                mocks.ReplayAll();

                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Call
                    Action test = () => contextMenu.Items[contextMenuCalculateAllIndexNestedGroup].PerformClick();

                    // Assert
                    TestHelper.AssertLogMessages(test, m =>
                    {
                        string[] messages = m.ToArray();
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

            var group = new CalculationGroup();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);

            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, Path.Combine(hrdPath, "HRD ijsselmeer.sqlite"));

            var nodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(group,
                                                                                              failureMechanism,
                                                                                              assessmentSection);
            var parentNodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                                    failureMechanism,
                                                                                                    assessmentSection);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                var mainWindow = mocks.Stub<IMainWindow>();

                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);

                var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Stub(cf => cf.CreateWaveConditionsCosineCalculator(hrdPath)).Return(new TestWaveConditionsCosineCalculator());
                mocks.ReplayAll();

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
            string hrdPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");

            var group = new CalculationGroup();
            GrassCoverErosionOutwardsWaveConditionsCalculation calculationA = GetValidCalculation();
            GrassCoverErosionOutwardsWaveConditionsCalculation calculationB = GetValidCalculation();
            group.Children.Add(calculationA);
            group.Children.Add(calculationB);

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);

            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, Path.Combine(hrdPath, "HRD ijsselmeer.sqlite"));

            var nodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(group,
                                                                                              failureMechanism,
                                                                                              assessmentSection);
            var parentNodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                                    failureMechanism,
                                                                                                    assessmentSection);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                var mainWindow = mocks.Stub<IMainWindow>();

                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);

                var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Stub(cf => cf.CreateWaveConditionsCosineCalculator(hrdPath)).Return(new TestWaveConditionsCosineCalculator());
                mocks.ReplayAll();

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
            string hrdPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");

            var observerA = mocks.StrictMock<IObserver>();
            var observerB = mocks.StrictMock<IObserver>();
            if (confirm)
            {
                observerA.Expect(o => o.UpdateObserver());
                observerB.Expect(o => o.UpdateObserver());
            }

            var group = new CalculationGroup();
            GrassCoverErosionOutwardsWaveConditionsCalculation calculationA = GetValidCalculation();
            calculationA.Output = new GrassCoverErosionOutwardsWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>());
            GrassCoverErosionOutwardsWaveConditionsCalculation calculationB = GetValidCalculation();
            calculationB.Output = new GrassCoverErosionOutwardsWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>());
            group.Children.Add(calculationA);
            group.Children.Add(calculationB);
            calculationA.Attach(observerA);
            calculationB.Attach(observerB);

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);

            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, Path.Combine(hrdPath, "HRD ijsselmeer.sqlite"));

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

            using (var treeViewControl = new TreeViewControl())
            {
                var mainWindow = mocks.Stub<IMainWindow>();

                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);

                var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Stub(cf => cf.CreateWaveConditionsCosineCalculator(hrdPath)).Return(new TestWaveConditionsCosineCalculator());
                mocks.ReplayAll();

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

                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());

                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());
                mocks.ReplayAll();

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
        public void ContextMenuStrip_WithForeshoreProfileAndChanges_ContextMenuItemUpdateAllForeshoreProfilesEnabled()
        {
            // Setup
            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile()
                }
            };
            calculation.InputParameters.UseBreakWater = true;

            var nodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(
                new CalculationGroup
                {
                    Children =
                    {
                        calculation
                    }
                },
                failureMechanism,
                assessmentSectionStub);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                  contextMenuUpdateForeshoreProfileIndexRootGroup,
                                                                  "&Bijwerken voorlandprofielen...",
                                                                  "Alle berekeningen met een voorlandprofiel bijwerken.",
                                                                  RingtoetsCommonFormsResources.UpdateItemIcon);
                }
            }
        }

        [Test]
        public void GivenCalculationWithForeshoreProfileSet_WhenUpdatingForeshoreProfileFromContextMenu_ThenCalculationUpdatedAndUpdateObserver()
        {
            // Given
            var calculationObserver = mocks.StrictMock<IObserver>();
            var calculationInputObserver = mocks.StrictMock<IObserver>();
            calculationInputObserver.Expect(o => o.UpdateObserver());

            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile(true)
                }
            };
            calculation.InputParameters.UseBreakWater = false;

            var nodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(
                new CalculationGroup
                {
                    Children =
                    {
                        calculation
                    }
                },
                failureMechanism,
                assessmentSectionStub);

            calculation.Attach(calculationObserver);
            calculation.InputParameters.Attach(calculationInputObserver);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

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
                var updateHandler = mocks.Stub<IUpdateCommandHandler>();
                var viewCommands = mocks.Stub<IViewCommands>();
                var menuBuilder = new ContextMenuBuilder(appFeatureCommandHandler,
                                                         importHandler,
                                                         exportHandler,
                                                         updateHandler,
                                                         viewCommands,
                                                         context,
                                                         treeViewControl);

                gui.Stub(g => g.ViewCommands).Return(viewCommands);
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                // When
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Then
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  contextMenuAddCalculationIndexRootGroup,
                                                                  "Berekening &toevoegen",
                                                                  "Voeg een nieuwe berekening toe aan deze map met berekeningen.",
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
                var updateHandler = mocks.Stub<IUpdateCommandHandler>();
                var viewCommands = mocks.Stub<IViewCommands>();
                var menuBuilder = new ContextMenuBuilder(appFeatureCommandHandler,
                                                         importHandler,
                                                         exportHandler,
                                                         updateHandler,
                                                         viewCommands,
                                                         context,
                                                         treeViewControl);

                gui.Stub(g => g.ViewCommands).Return(viewCommands);
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
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

        [Test]
        public void GivenDialogGenerateCalculationButtonClicked_WhenCalculationSelectedAndDialogClosed_ThenUpdateCalculationGroup()
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                var existingGroup = new CalculationGroup();
                var existingcalculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();
                var group = new CalculationGroup
                {
                    Children =
                    {
                        existingGroup,
                        existingcalculation
                    }
                };
                var hydraulicBoundaryLocation1 = new TestHydraulicBoundaryLocation();
                var hydraulicBoundaryLocation2 = new TestHydraulicBoundaryLocation();
                var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
                {
                    HydraulicBoundaryLocations =
                    {
                        hydraulicBoundaryLocation1,
                        hydraulicBoundaryLocation2
                    }
                };
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
                {
                    Locations =
                    {
                        hydraulicBoundaryLocation1,
                        hydraulicBoundaryLocation2
                    }
                };

                var observerMock = mocks.StrictMock<IObserver>();
                observerMock.Expect(o => o.UpdateObserver());
                var nodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(group,
                                                                                                  failureMechanism,
                                                                                                  assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var mainWindow = mocks.Stub<IMainWindow>();
                var viewCommands = mocks.StrictMock<IViewCommands>();

                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mainWindow);
                gui.Stub(cmp => cmp.ViewCommands).Return(viewCommands);

                mocks.ReplayAll();

                nodeData.Attach(observerMock);

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
                Assert.AreSame(existingcalculation, group.Children[1]);
                Assert.NotNull(dialog);
                Assert.NotNull(grid);
                var firstCalculation = group.Children[2] as GrassCoverErosionOutwardsWaveConditionsCalculation;
                Assert.IsNotNull(firstCalculation);
                Assert.AreSame(hydraulicBoundaryLocation1, firstCalculation.InputParameters.HydraulicBoundaryLocation);
                var secondCalculation = group.Children[3] as GrassCoverErosionOutwardsWaveConditionsCalculation;
                Assert.IsNotNull(secondCalculation);
                Assert.AreSame(hydraulicBoundaryLocation2, secondCalculation.InputParameters.HydraulicBoundaryLocation);
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
                var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
                {
                    HydraulicBoundaryLocations =
                    {
                        hydraulicBoundaryLocation1,
                        hydraulicBoundaryLocation2
                    }
                };
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
                {
                    Locations =
                    {
                        new TestHydraulicBoundaryLocation(),
                        hydraulicBoundaryLocation1,
                        hydraulicBoundaryLocation2
                    }
                };

                var nodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(group,
                                                                                                  failureMechanism,
                                                                                                  assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var mainWindow = mocks.Stub<IMainWindow>();
                var viewCommands = mocks.StrictMock<IViewCommands>();

                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mainWindow);
                gui.Stub(cmp => cmp.ViewCommands).Return(viewCommands);

                mocks.ReplayAll();

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

        private static GrassCoverErosionOutwardsWaveConditionsCalculation GetValidCalculation()
        {
            return new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(9.3),
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
        }
    }
}