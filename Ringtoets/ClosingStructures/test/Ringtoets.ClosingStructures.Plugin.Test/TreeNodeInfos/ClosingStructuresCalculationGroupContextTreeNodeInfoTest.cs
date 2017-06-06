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
using System.Collections.Generic;
using System.IO;
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
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Data.TestUtil;
using Ringtoets.ClosingStructures.Forms.PresentationObjects;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.ClosingStructures.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class ClosingStructuresCalculationGroupContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuGenerateCalculationsIndexRootGroup = 3;
        private const int contextMenuAddCalculationGroupIndexRootGroup = 5;
        private const int contextMenuAddCalculationIndexRootGroup = 6;
        private const int contextMenuUpdateForeshoreProfileIndexRootGroup = 8;
        private const int contextMenuUpdateStructureAllIndexRootGroup = 9;
        private const int contextMenuValidateAllIndexRootGroup = 11;
        private const int contextMenuCalculateAllIndexRootGroup = 12;
        private const int contextMenuClearAllIndexRootGroup = 14;

        private const int contextMenuAddCalculationGroupIndexNestedGroup = 3;
        private const int contextMenuAddCalculationIndexNestedGroup = 4;
        private const int contextMenuUpdateForeshoreProfileIndexNestedGroup = 7;
        private const int contextMenuUpdateStructureAllIndexNestedGroup = 8;
        private const int contextMenuValidateAllIndexNestedGroup = 10;
        private const int contextMenuCalculateAllIndexNestedGroup = 11;
        private const int contextMenuClearAllIndexNestedGroup = 13;

        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "HydraulicBoundaryDatabaseImporter");

        private IGui gui;
        private TreeNodeInfo info;
        private MockRepository mocks;
        private ClosingStructuresPlugin plugin;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            gui = mocks.Stub<IGui>();
            plugin = new ClosingStructuresPlugin
            {
                Gui = gui
            };

            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(ClosingStructuresCalculationGroupContext));
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
        public void ChildNodeObjects_EmptyGroup_ReturnEmpty()
        {
            // Setup
            var group = new CalculationGroup();
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var groupContext = new ClosingStructuresCalculationGroupContext(group,
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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var calculationItem = mocks.Stub<ICalculationBase>();
            mocks.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var group = new CalculationGroup();
            var childGroup = new CalculationGroup();
            var childCalculation = new StructuresCalculation<ClosingStructuresInput>();

            group.Children.Add(childGroup);
            group.Children.Add(calculationItem);
            group.Children.Add(childCalculation);

            var groupContext = new ClosingStructuresCalculationGroupContext(group,
                                                                            failureMechanism,
                                                                            assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(groupContext).ToArray();

            // Assert
            Assert.AreEqual(group.Children.Count, children.Length);
            var calculationGroupContext = (ClosingStructuresCalculationGroupContext) children[0];
            Assert.AreSame(childGroup, calculationGroupContext.WrappedData);
            Assert.AreSame(failureMechanism, calculationGroupContext.FailureMechanism);
            Assert.AreSame(assessmentSection, calculationGroupContext.AssessmentSection);
            Assert.AreSame(calculationItem, children[1]);
            var calculationContext = (ClosingStructuresCalculationContext) children[2];
            Assert.AreSame(childCalculation, calculationContext.WrappedData);
            Assert.AreSame(assessmentSection, calculationContext.AssessmentSection);
        }

        [Test]
        public void ContextMenuStrip_WithoutParentNodeDefaultBehavior_CallsContextMenuBuilderMethods()
        {
            // Setup
            var group = new CalculationGroup();
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var groupContext = new ClosingStructuresCalculationGroupContext(group,
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
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var groupContext = new ClosingStructuresCalculationGroupContext(group,
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
                    Assert.AreEqual(21, menu.Items.Count);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuGenerateCalculationsIndexRootGroup,
                                                                  "Genereer &berekeningen...",
                                                                  "Er zijn geen kunstwerken beschikbaar om berekeningen voor te genereren.",
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
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuUpdateStructureAllIndexRootGroup,
                                                                  "&Bijwerken kunstwerken...",
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
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearAllIndexRootGroup,
                                                                  "&Wis alle uitvoer...",
                                                                  "Er zijn geen berekeningen met uitvoer om te wissen.",
                                                                  RingtoetsCommonFormsResources.ClearIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_WithoutParentNodeWithStructuresImported_GenerateItemEnabledWithTooltip()
        {
            // Setup
            var group = new CalculationGroup();
            var failureMechanism = new ClosingStructuresFailureMechanism();
            failureMechanism.ClosingStructures.AddRange(new[]
            {
                new TestClosingStructure()
            }, "some path");
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            var groupContext = new ClosingStructuresCalculationGroupContext(group,
                                                                            failureMechanism,
                                                                            assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(groupContext, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(groupContext, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuGenerateCalculationsIndexRootGroup,
                                                                  "Genereer &berekeningen...",
                                                                  "Genereer berekeningen op basis van geselecteerde kunstwerken.",
                                                                  RingtoetsCommonFormsResources.GenerateScenariosIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_NestedCalculationGroup_CallsContextMenuBuilderMethods()
        {
            // Setup
            var group = new CalculationGroup();
            var parentGroup = new CalculationGroup();
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var groupContext = new ClosingStructuresCalculationGroupContext(group,
                                                                            failureMechanism,
                                                                            assessmentSection);
            var parentGroupContext = new ClosingStructuresCalculationGroupContext(parentGroup,
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
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var groupContext = new ClosingStructuresCalculationGroupContext(group,
                                                                            failureMechanism,
                                                                            assessmentSection);
            var parentGroupContext = new ClosingStructuresCalculationGroupContext(parentGroup,
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
                    Assert.AreEqual(20, menu.Items.Count);

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
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuUpdateStructureAllIndexNestedGroup,
                                                                  "&Bijwerken kunstwerken...",
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
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearAllIndexNestedGroup,
                                                                  "&Wis alle uitvoer...",
                                                                  "Er zijn geen berekeningen met uitvoer om te wissen.",
                                                                  RingtoetsCommonFormsResources.ClearIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_NoHydraulicBoundaryDatabase_ContextMenuItemCalculateAllAndValidateAllDisabledAndTooltipSet()
        {
            // Setup
            var group = new CalculationGroup
            {
                Children =
                {
                    new StructuresCalculation<ClosingStructuresInput>()
                }
            };

            var failureMechanism = new TestClosingStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new StructuresCalculation<ClosingStructuresInput>());

            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var nodeData = new ClosingStructuresCalculationGroupContext(group,
                                                                        failureMechanism,
                                                                        assessmentSection);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuCalculateAllIndexRootGroup,
                                                                  "Alles be&rekenen",
                                                                  "Er is geen hydraulische randvoorwaardendatabase geïmporteerd.",
                                                                  RingtoetsCommonFormsResources.CalculateAllIcon,
                                                                  false);

                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuValidateAllIndexRootGroup,
                                                                  "Alles &valideren",
                                                                  "Er is geen hydraulische randvoorwaardendatabase geïmporteerd.",
                                                                  RingtoetsCommonFormsResources.ValidateAllIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_HydraulicBoundaryDatabaseNotValid_ContextMenuItemCalculateAllAndValidateAllDisabledAndTooltipSet()
        {
            // Setup
            var group = new CalculationGroup
            {
                Children =
                {
                    new StructuresCalculation<ClosingStructuresInput>()
                }
            };

            var failureMechanism = new TestClosingStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new StructuresCalculation<ClosingStructuresInput>());

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            var nodeData = new ClosingStructuresCalculationGroupContext(group,
                                                                        failureMechanism,
                                                                        assessmentSection);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    ToolStripItem calculatAllContextMenuItem = contextMenu.Items[contextMenuCalculateAllIndexRootGroup];

                    Assert.AreEqual("Alles be&rekenen", calculatAllContextMenuItem.Text);
                    StringAssert.Contains("Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt.", calculatAllContextMenuItem.ToolTipText);
                    TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.CalculateAllIcon, calculatAllContextMenuItem.Image);
                    Assert.IsFalse(calculatAllContextMenuItem.Enabled);

                    ToolStripItem validateAllContextMenuItem = contextMenu.Items[contextMenuValidateAllIndexRootGroup];

                    Assert.AreEqual("Alles &valideren", validateAllContextMenuItem.Text);
                    StringAssert.Contains("Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt.", validateAllContextMenuItem.ToolTipText);
                    TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.ValidateAllIcon, validateAllContextMenuItem.Image);
                    Assert.IsFalse(validateAllContextMenuItem.Enabled);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismContributionZero_ContextMenuItemCalculateAllAndValidateAllDisabledAndTooltipSet()
        {
            // Setup
            var group = new CalculationGroup
            {
                Children =
                {
                    new StructuresCalculation<ClosingStructuresInput>()
                }
            };

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var failureMechanism = new ClosingStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new StructuresCalculation<ClosingStructuresInput>());

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "1.0"
            };

            var nodeData = new ClosingStructuresCalculationGroupContext(group,
                                                                        failureMechanism,
                                                                        assessmentSection);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndexRootGroup,
                                                                  "Alles be&rekenen",
                                                                  "De bijdrage van dit toetsspoor is nul.",
                                                                  RingtoetsCommonFormsResources.CalculateAllIcon,
                                                                  false);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndexRootGroup,
                                                                  "Alles &valideren",
                                                                  "De bijdrage van dit toetsspoor is nul.",
                                                                  RingtoetsCommonFormsResources.ValidateAllIcon,
                                                                  false);
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
                    new StructuresCalculation<ClosingStructuresInput>()
                }
            };

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var failureMechanism = new TestClosingStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new StructuresCalculation<ClosingStructuresInput>());

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "1.0"
            };

            var nodeData = new ClosingStructuresCalculationGroupContext(group,
                                                                        failureMechanism,
                                                                        assessmentSection);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndexRootGroup,
                                                                  "Alles be&rekenen",
                                                                  "Voer alle berekeningen binnen deze map met berekeningen uit.",
                                                                  RingtoetsCommonFormsResources.CalculateAllIcon);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndexRootGroup,
                                                                  "Alles &valideren",
                                                                  "Valideer alle berekeningen binnen deze map met berekeningen.",
                                                                  RingtoetsCommonFormsResources.ValidateAllIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationGroupWithCalculationWithForeshoreProfileAndInputOutOfSync_ContextMenuItemUpdateForeshoreProfilesEnabledAndToolTipSet()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanism = new TestClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile()
                }
            };

            var nodeData = new ClosingStructuresCalculationGroupContext(
                new CalculationGroup
                {
                    Children =
                    {
                        calculation
                    }
                },
                failureMechanism,
                assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

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
                                                                  RingtoetsCommonFormsResources.UpdateItemIcon);
                }
            }
        }

        [Test]
        public void GivenCalculationWithoutOutputAndWithInputOutOfSync_WhenUpdateForeshoreProfilesClicked_ThenNoInquiryAndCalculationUpdatedAndInputObserverNotified()
        {
            // Given
            var calculationObserver = mocks.StrictMock<IObserver>();
            var calculationInputObserver = mocks.StrictMock<IObserver>();
            calculationInputObserver.Expect(o => o.UpdateObserver());

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanism = new TestClosingStructuresFailureMechanism();

            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile(true)
                }
            };

            var nodeData = new ClosingStructuresCalculationGroupContext(
                new CalculationGroup
                {
                    Children =
                    {
                        calculation
                    }
                },
                failureMechanism,
                assessmentSection);

            calculation.Attach(calculationObserver);
            calculation.InputParameters.Attach(calculationInputObserver);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

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
        public void ContextMenuStrip_ClickOnCalculateAllItem_ScheduleAllChildCalculations()
        {
            // Setup
            var mainWindow = mocks.Stub<IMainWindow>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var failureMechanism = new TestClosingStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new TestClosingStructuresCalculation
            {
                Name = "A",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                }
            });
            failureMechanism.CalculationsGroup.Children.Add(new TestClosingStructuresCalculation
            {
                Name = "B",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                }
            });

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var hydraulicBoundaryDatabase = mocks.Stub<HydraulicBoundaryDatabase>();
            hydraulicBoundaryDatabase.FilePath = validFilePath;

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;
            assessmentSection.Stub(a => a.Id).Return(string.Empty);
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 1, 1));

            var groupContext = new ClosingStructuresCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                            failureMechanism,
                                                                            assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(groupContext, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());

                int nrOfCalculators = failureMechanism.Calculations.Count();
                var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Expect(cf => cf.CreateStructuresClosureCalculator(testDataPath))
                                 .Return(new TestStructuresClosureCalculator())
                                 .Repeat
                                 .Times(nrOfCalculators);
                mocks.ReplayAll();

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
                        Assert.AreEqual(12, messageList.Count);
                        Assert.AreEqual("Validatie van 'A' gestart.", messageList[0]);
                        Assert.AreEqual("Validatie van 'A' beëindigd.", messageList[1]);
                        Assert.AreEqual("Berekening van 'A' gestart.", messageList[2]);
                        StringAssert.StartsWith("Betrouwbaarheid sluiting kunstwerk berekening is uitgevoerd op de tijdelijke locatie", messageList[3]);
                        Assert.AreEqual("Berekening van 'A' beëindigd.", messageList[4]);
                        Assert.AreEqual("Validatie van 'B' gestart.", messageList[5]);
                        Assert.AreEqual("Validatie van 'B' beëindigd.", messageList[6]);
                        Assert.AreEqual("Berekening van 'B' gestart.", messageList[7]);
                        StringAssert.StartsWith("Betrouwbaarheid sluiting kunstwerk berekening is uitgevoerd op de tijdelijke locatie", messageList[8]);
                        Assert.AreEqual("Berekening van 'B' beëindigd.", messageList[9]);
                        Assert.AreEqual("Uitvoeren van berekening 'A' is gelukt.", messageList[10]);
                        Assert.AreEqual("Uitvoeren van berekening 'B' is gelukt.", messageList[11]);
                    });
                }
            }
        }

        [Test]
        public void ContextMenuStrip_ClickOnValidateAllItem_ScheduleAllChildCalculations()
        {
            // Setup
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var failureMechanism = new TestClosingStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new TestClosingStructuresCalculation
            {
                Name = "A",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                }
            });
            failureMechanism.CalculationsGroup.Children.Add(new TestClosingStructuresCalculation
            {
                Name = "B",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                }
            });

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var hydraulicBoundaryDatabase = mocks.Stub<HydraulicBoundaryDatabase>();
            hydraulicBoundaryDatabase.FilePath = validFilePath;

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;

            var groupContext = new ClosingStructuresCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                            failureMechanism,
                                                                            assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(groupContext, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(groupContext, null, treeViewControl))
                {
                    // Call
                    TestHelper.AssertLogMessages(() => contextMenu.Items[contextMenuValidateAllIndexRootGroup].PerformClick(), messages =>
                    {
                        List<string> messageList = messages.ToList();

                        // Assert
                        Assert.AreEqual(4, messageList.Count);
                        Assert.AreEqual("Validatie van 'A' gestart.", messageList[0]);
                        Assert.AreEqual("Validatie van 'A' beëindigd.", messageList[1]);
                        Assert.AreEqual("Validatie van 'B' gestart.", messageList[2]);
                        Assert.AreEqual("Validatie van 'B' beëindigd.", messageList[3]);
                    });
                }
            }
        }

        [Test]
        public void ContextMenuStrip_ClickOnAddGroupItem_AddGroupToCalculationGroupAndNotifyObservers()
        {
            // Setup
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            var group = new CalculationGroup();
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var nodeData = new ClosingStructuresCalculationGroupContext(group,
                                                                        failureMechanism,
                                                                        assessmentSection);
            var calculationGroup = new CalculationGroup
            {
                Name = "Nieuwe map"
            };

            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

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
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var nodeData = new ClosingStructuresCalculationGroupContext(group,
                                                                        failureMechanism,
                                                                        assessmentSection);
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                Name = "Nieuwe berekening"
            };
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                group.Children.Add(calculation);
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
                    Assert.IsInstanceOf<StructuresCalculation<ClosingStructuresInput>>(newlyAddedItem);
                    Assert.AreEqual("Nieuwe berekening (1)", newlyAddedItem.Name,
                                    "An item with the same name default name already exists, therefore '(1)' needs to be appended.");
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationGroupWithoutCalculations_ContextMenuItemUpdateStructuresDisabledAndToolTipSet()
        {
            // Setup
            var group = new CalculationGroup();
            var failureMechanism = new ClosingStructuresFailureMechanism();

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var nodeData = new ClosingStructuresCalculationGroupContext(group,
                                                                        failureMechanism,
                                                                        assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuUpdateStructureAllIndexRootGroup,
                                                                  "&Bijwerken kunstwerken...",
                                                                  "Er zijn geen berekeningen om bij te werken.",
                                                                  RingtoetsCommonFormsResources.UpdateItemIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationGroupWithCalculationsWithoutStructure_ContextMenuItemUpdateStructuresDisabledAndToolTipSet()
        {
            // Setup
            var group = new CalculationGroup
            {
                Children =
                {
                    new StructuresCalculation<ClosingStructuresInput>()
                }
            };

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var nodeData = new ClosingStructuresCalculationGroupContext(group,
                                                                        failureMechanism,
                                                                        assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuUpdateStructureAllIndexRootGroup,
                                                                  "&Bijwerken kunstwerken...",
                                                                  "Er zijn geen berekeningen om bij te werken.",
                                                                  RingtoetsCommonFormsResources.UpdateItemIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationGroupWithCalculationWithStructureAndInputInSync_ContextMenuItemUpdateStructuresDisabledAndToolTipSet()
        {
            // Setup
            var group = new CalculationGroup
            {
                Children =
                {
                    new StructuresCalculation<ClosingStructuresInput>
                    {
                        InputParameters =
                        {
                            Structure = new TestClosingStructure()
                        }
                    }
                }
            };

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var nodeData = new ClosingStructuresCalculationGroupContext(group,
                                                                        failureMechanism,
                                                                        assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuUpdateStructureAllIndexRootGroup,
                                                                  "&Bijwerken kunstwerken...",
                                                                  "Er zijn geen berekeningen om bij te werken.",
                                                                  RingtoetsCommonFormsResources.UpdateItemIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationGroupWithCalculationWithStructureAndInputOutOfSync_ContextMenuItemUpdateStructuresEnabledAndToolTipSet()
        {
            // Setup
            var testClosingStructure = new TestClosingStructure();
            var group = new CalculationGroup
            {
                Children =
                {
                    new StructuresCalculation<ClosingStructuresInput>
                    {
                        InputParameters =
                        {
                            Structure = testClosingStructure
                        }
                    }
                }
            };

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var nodeData = new ClosingStructuresCalculationGroupContext(group,
                                                                        failureMechanism,
                                                                        assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                ChangeStructure(testClosingStructure);

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuUpdateStructureAllIndexRootGroup,
                                                                  "&Bijwerken kunstwerken...",
                                                                  "Alle berekeningen met een kunstwerk bijwerken.",
                                                                  RingtoetsCommonFormsResources.UpdateItemIcon);
                }
            }
        }

        [Test]
        public void GivenCalculationWithoutOutputAndWithInputOutOfSync_WhenUpdateStructuresClicked_ThenNoInquiryAndCalculationUpdatedAndInputObserverNotified()
        {
            // Given
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var structure = new TestClosingStructure();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = structure
                }
            };
            var failureMechanism = new ClosingStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            var nodeData = new ClosingStructuresCalculationGroupContext(failureMechanism.CalculationsGroup, failureMechanism, assessmentSection);

            var inputObserver = mocks.StrictMock<IObserver>();
            inputObserver.Expect(obs => obs.UpdateObserver());
            calculation.InputParameters.Attach(inputObserver);

            var calculationObserver = mocks.StrictMock<IObserver>();
            calculation.Attach(calculationObserver);

            using (var treeViewControl = new TreeViewControl())
            {
                var mainWindow = mocks.Stub<IMainWindow>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                mocks.ReplayAll();

                ChangeStructure(structure);

                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, assessmentSection, treeViewControl))
                {
                    // When
                    menu.Items[contextMenuUpdateStructureAllIndexRootGroup].PerformClick();

                    // Then
                    Assert.IsTrue(calculation.InputParameters.IsStructureInputSynchronized);

                    // Note: observer assertions are verified in the TearDown()
                }
            }
        }

        [Test]
        public void GivenCalculationWithOutputAndInputOutOfSync_WhenUpdateStructuresClickedAndCancelled_ThenInquiryAndCalculationNotUpdatedAndObserversNotNotified()
        {
            // Given
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var structure = new TestClosingStructure();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = structure
                },
                Output = new TestStructuresOutput()
            };

            var failureMechanism = new ClosingStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            var nodeData = new ClosingStructuresCalculationGroupContext(failureMechanism.CalculationsGroup, failureMechanism, assessmentSection);

            var inputObserver = mocks.StrictMock<IObserver>();
            calculation.InputParameters.Attach(inputObserver);

            var calculationObserver = mocks.StrictMock<IObserver>();
            calculation.Attach(calculationObserver);

            string textBoxMessage = null;
            DialogBoxHandler = (name, wnd) =>
            {
                var helper = new MessageBoxTester(wnd);
                textBoxMessage = helper.Text;
                helper.ClickCancel();
            };

            using (var treeViewControl = new TreeViewControl())
            {
                var mainWindow = mocks.Stub<IMainWindow>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                mocks.ReplayAll();

                ChangeStructure(structure);

                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, assessmentSection, treeViewControl))
                {
                    // When
                    menu.Items[contextMenuUpdateStructureAllIndexRootGroup].PerformClick();

                    // Then
                    Assert.IsTrue(calculation.HasOutput);
                    Assert.IsFalse(calculation.InputParameters.IsStructureInputSynchronized);

                    string expectedMessage = "Als u kiest voor bijwerken, dan wordt het resultaat van alle bij te werken berekeningen " +
                                             $"verwijderd.{Environment.NewLine}{Environment.NewLine}Weet u zeker dat u wilt doorgaan?";
                    Assert.AreEqual(expectedMessage, textBoxMessage);

                    // Note: observer assertions are verified in the TearDown()
                }
            }
        }

        [Test]
        public void GivenCalculationWithOutputAndInputOutOfSync_WhenUpdateStructuresClickedAndContinued_ThenInquiryAndCalculationUpdatedAndObserversNotified()
        {
            // Given
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var structure = new TestClosingStructure();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = structure
                },
                Output = new TestStructuresOutput()
            };
            var failureMechanism = new ClosingStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            var nodeData = new ClosingStructuresCalculationGroupContext(failureMechanism.CalculationsGroup, failureMechanism, assessmentSection);

            var inputObserver = mocks.StrictMock<IObserver>();
            inputObserver.Expect(obs => obs.UpdateObserver());
            calculation.InputParameters.Attach(inputObserver);

            var calculationObserver = mocks.StrictMock<IObserver>();
            calculationObserver.Expect(obs => obs.UpdateObserver());
            calculation.Attach(calculationObserver);

            string textBoxMessage = null;
            DialogBoxHandler = (name, wnd) =>
            {
                var helper = new MessageBoxTester(wnd);
                textBoxMessage = helper.Text;
                helper.ClickOk();
            };

            using (var treeViewControl = new TreeViewControl())
            {
                var mainWindow = mocks.Stub<IMainWindow>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                mocks.ReplayAll();

                ChangeStructure(structure);

                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, assessmentSection, treeViewControl))
                {
                    // When
                    menu.Items[contextMenuUpdateStructureAllIndexRootGroup].PerformClick();

                    // Then
                    Assert.IsFalse(calculation.HasOutput);
                    Assert.IsTrue(calculation.InputParameters.IsStructureInputSynchronized);

                    string expectedMessage = "Als u kiest voor bijwerken, dan wordt het resultaat van alle bij te werken berekeningen " +
                                             $"verwijderd.{Environment.NewLine}{Environment.NewLine}Weet u zeker dat u wilt doorgaan?";
                    Assert.AreEqual(expectedMessage, textBoxMessage);

                    // Note: observer assertions are verified in the TearDown()
                }
            }
        }

        [Test]
        public void GivenCalculationsViewGenerateScenariosButtonClicked_WhenClosingStructureSelectedAndDialogClosed_ThenCalculationsAddedWithClosingStructureAssigned()
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSection = mocks.Stub<IAssessmentSection>();

                ClosingStructure structure1 = new TestClosingStructure("Structure 1");
                ClosingStructure structure2 = new TestClosingStructure("Structure 2");

                var existingCalculationGroup = new CalculationGroup();
                var existingCalculation = new StructuresCalculation<ClosingStructuresInput>();
                var failureMechanism = new ClosingStructuresFailureMechanism
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
                failureMechanism.ClosingStructures.AddRange(new[]
                {
                    structure1,
                    structure2
                }, "some path");

                var nodeData = new ClosingStructuresCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                            failureMechanism,
                                                                            assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var mainWindow = mocks.Stub<IMainWindow>();

                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

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
                    var generatedCalculation = failureMechanism.CalculationsGroup.Children[2] as StructuresCalculation<ClosingStructuresInput>;
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
                var assessmentSection = mocks.Stub<IAssessmentSection>();

                ClosingStructure structure1 = new TestClosingStructure("Structure 1");
                ClosingStructure structure2 = new TestClosingStructure("Structure 2");

                var failureMechanism = new ClosingStructuresFailureMechanism();
                failureMechanism.ClosingStructures.AddRange(new[]
                {
                    structure1,
                    structure2
                }, "some path");

                var nodeData = new ClosingStructuresCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                            failureMechanism,
                                                                            assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var mainWindow = mocks.Stub<IMainWindow>();

                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

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
                    Assert.AreEqual(0, failureMechanism.Calculations.OfType<StructuresCalculation<ClosingStructuresInput>>().Count());
                }
            }
        }

        [Test]
        public void GivenScenariosWithExistingCalculationWithSameName_WhenOkButtonClickedAndDialogClosed_ThenCalculationWithUniqueNameAdded()
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSection = mocks.Stub<IAssessmentSection>();

                const string existingCalculationName = "Closing structure";
                ClosingStructure closingStructure = new TestClosingStructure("id", existingCalculationName);

                var failureMechanism = new ClosingStructuresFailureMechanism
                {
                    CalculationsGroup =
                    {
                        Children =
                        {
                            new StructuresCalculation<ClosingStructuresInput>
                            {
                                Name = existingCalculationName
                            }
                        }
                    }
                };
                failureMechanism.ClosingStructures.AddRange(new[]
                {
                    closingStructure
                }, "some path");

                var nodeData = new ClosingStructuresCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                            failureMechanism,
                                                                            assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var mainWindow = mocks.Stub<IMainWindow>();

                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

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
                    StructuresCalculation<ClosingStructuresInput>[] closingStructuresCalculations = failureMechanism.Calculations.OfType<StructuresCalculation<ClosingStructuresInput>>().ToArray();
                    Assert.AreEqual(2, closingStructuresCalculations.Length);
                    Assert.AreEqual(expectedNewName, closingStructuresCalculations[1].Name);
                }
            }
        }

        [Test]
        public void OnNodeRemoved_NestedCalculationGroup_RemoveGroupAndNotifyObservers()
        {
            // Setup
            var observerMock = mocks.StrictMock<IObserver>();
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var group = new CalculationGroup();
            var parentGroup = new CalculationGroup();
            var nodeData = new ClosingStructuresCalculationGroupContext(group,
                                                                        failureMechanism,
                                                                        assessmentSection);
            var parentNodeData = new ClosingStructuresCalculationGroupContext(parentGroup,
                                                                              failureMechanism,
                                                                              assessmentSection);

            observerMock.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            parentGroup.Children.Add(group);
            parentNodeData.Attach(observerMock);

            // Precondition
            Assert.IsTrue(info.CanRemove(nodeData, parentNodeData));

            // Call
            info.OnNodeRemoved(nodeData, parentNodeData);

            // Assert
            CollectionAssert.DoesNotContain(parentGroup.Children, group);
        }

        [Test]
        public void OnNodeRemoved_CalculationInGroupAssignedToSection_CalculationDetachedFromSection()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var group = new CalculationGroup();
            var parentGroup = new CalculationGroup();
            var nodeData = new ClosingStructuresCalculationGroupContext(group,
                                                                        failureMechanism,
                                                                        assessmentSection);
            var parentNodeData = new ClosingStructuresCalculationGroupContext(parentGroup,
                                                                              failureMechanism,
                                                                              assessmentSection);

            mocks.ReplayAll();

            parentGroup.Children.Add(group);

            failureMechanism.AddSection(new FailureMechanismSection("section", new[]
            {
                new Point2D(0, 0)
            }));

            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            group.Children.Add(calculation);

            ClosingStructuresFailureMechanismSectionResult result = failureMechanism.SectionResults.First();
            result.Calculation = calculation;

            // Call
            info.OnNodeRemoved(nodeData, parentNodeData);

            // Assert
            Assert.IsNull(result.Calculation);
        }

        [Test]
        public void OnNodeRemoved_NestedCalculationGroupContainingCalculations_RemoveGroupAndCalculationsAndNotifyObservers()
        {
            // Setup
            var observerMock = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var group = new CalculationGroup();
            var parentGroup = new CalculationGroup();
            var nodeData = new ClosingStructuresCalculationGroupContext(group,
                                                                        failureMechanism,
                                                                        assessmentSection);
            var parentNodeData = new ClosingStructuresCalculationGroupContext(parentGroup,
                                                                              failureMechanism,
                                                                              assessmentSection);
            var calculation = new StructuresCalculation<ClosingStructuresInput>();

            observerMock.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            group.Children.Add(calculation);
            parentGroup.Children.Add(group);
            parentNodeData.Attach(observerMock);

            // Precondition
            Assert.IsTrue(info.CanRemove(nodeData, parentNodeData));

            // Call
            info.OnNodeRemoved(nodeData, parentNodeData);

            // Assert
            CollectionAssert.DoesNotContain(parentGroup.Children, group);
        }

        private static void ChangeStructure(ClosingStructure structure)
        {
            structure.CopyProperties(new ClosingStructure(
                                         new ClosingStructure.ConstructionProperties
                                         {
                                             Id = structure.Id,
                                             Name = structure.Name,
                                             Location = structure.Location
                                         }));
        }
    }
}