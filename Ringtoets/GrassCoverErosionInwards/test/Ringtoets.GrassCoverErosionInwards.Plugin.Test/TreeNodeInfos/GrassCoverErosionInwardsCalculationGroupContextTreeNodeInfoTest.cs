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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Forms;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using GrassCoverErosionInwardsPluginResources = Ringtoets.GrassCoverErosionInwards.Plugin.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationGroupContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuGenerateCalculationsIndexRootGroup = 3;
        private const int contextMenuAddCalculationGroupIndexRootGroup = 5;
        private const int contextMenuAddCalculationIndexRootGroup = 6;
        private const int contextMenuUpdateDikeProfileAllIndexRootGroup = 8;
        private const int contextMenuValidateAllIndexRootGroup = 10;
        private const int contextMenuCalculateAllIndexRootGroup = 11;
        private const int contextMenuClearAllIndexRootGroup = 13;

        private const int contextMenuDuplicateIndexNestedGroup = 3;
        private const int contextMenuAddCalculationGroupIndexNestedGroup = 5;
        private const int contextMenuAddCalculationIndexNestedGroup = 6;
        private const int contextMenuUpdateDikeProfileAllIndexNestedGroup = 9;
        private const int contextMenuValidateAllIndexNestedGroup = 11;
        private const int contextMenuCalculateAllIndexNestedGroup = 12;
        private const int contextMenuClearAllIndexNestedGroup = 14;

        private const string dikeProfileCollectionPath = "some/arbitrary/path";
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "HydraulicBoundaryDatabaseImporter");

        private IGui gui;
        private TreeNodeInfo info;
        private MockRepository mocks;
        private GrassCoverErosionInwardsPlugin plugin;

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
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var groupContext = new GrassCoverErosionInwardsCalculationGroupContext(group,
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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var calculationItem = mocks.StrictMock<ICalculationBase>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var group = new CalculationGroup();
            var childGroup = new CalculationGroup();
            var childCalculation = new GrassCoverErosionInwardsCalculation();

            group.Children.Add(childGroup);
            group.Children.Add(calculationItem);
            group.Children.Add(childCalculation);

            var groupContext = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                                   null,
                                                                                   failureMechanism,
                                                                                   assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(groupContext).ToArray();

            // Assert
            Assert.AreEqual(group.Children.Count, children.Length);
            var calculationGroupContext = (GrassCoverErosionInwardsCalculationGroupContext) children[0];
            Assert.AreSame(childGroup, calculationGroupContext.WrappedData);
            Assert.AreSame(group, calculationGroupContext.Parent);
            Assert.AreSame(failureMechanism, calculationGroupContext.FailureMechanism);
            Assert.AreSame(assessmentSection, calculationGroupContext.AssessmentSection);
            Assert.AreSame(calculationItem, children[1]);
            var calculationContext = (GrassCoverErosionInwardsCalculationContext) children[2];
            Assert.AreSame(childCalculation, calculationContext.WrappedData);
            Assert.AreSame(group, calculationContext.Parent);
            Assert.AreSame(assessmentSection, calculationContext.AssessmentSection);
        }

        [Test]
        public void ContextMenuStrip_WithoutParentNodeDefaultBehavior_CallsContextMenuBuilderMethods()
        {
            // Setup
            var group = new CalculationGroup();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var groupContext = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                                   null,
                                                                                   failureMechanism,
                                                                                   assessmentSection);

            var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();
            using (mocks.Ordered())
            {
                menuBuilder.Expect(mb => mb.AddImportItem()).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddExportItem()).IgnoreArguments().Return(menuBuilder);
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
                gui.Stub(cmp => cmp.Get(groupContext, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());
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
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var groupContext = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                                   null,
                                                                                   failureMechanism,
                                                                                   assessmentSection);
            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                gui.Stub(g => g.Get(groupContext, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(groupContext, null, treeViewControl))
                {
                    // Assert
                    Assert.AreEqual(20, menu.Items.Count);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationGroupIndexRootGroup,
                                                                  "&Map toevoegen",
                                                                  "Voeg een nieuwe map toe aan deze map met berekeningen.",
                                                                  RingtoetsCommonFormsResources.AddFolderIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationIndexRootGroup,
                                                                  "Berekening &toevoegen",
                                                                  "Voeg een nieuwe berekening toe aan deze map met berekeningen.",
                                                                  RingtoetsCommonFormsResources.FailureMechanismIcon);

                    TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                  contextMenuUpdateDikeProfileAllIndexRootGroup,
                                                                  "&Bijwerken dijkprofielen...",
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
        public void ContextMenuStrip_NestedCalculationGroup_CallsContextMenuBuilderMethods()
        {
            // Setup
            var group = new CalculationGroup();
            var parentGroup = new CalculationGroup();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var groupContext = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                                   parentGroup,
                                                                                   failureMechanism,
                                                                                   assessmentSection);
            var parentGroupContext = new GrassCoverErosionInwardsCalculationGroupContext(parentGroup,
                                                                                         null,
                                                                                         failureMechanism,
                                                                                         assessmentSection);
            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();
                using (mocks.Ordered())
                {
                    menuBuilder.Expect(mb => mb.AddImportItem()).IgnoreArguments().Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddExportItem()).IgnoreArguments().Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
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

                gui.Stub(cmp => cmp.Get(groupContext, treeViewControl)).Return(menuBuilder);

                mocks.ReplayAll();

                // Call
                info.ContextMenuStrip(groupContext, parentGroupContext, treeViewControl);
            }

            // Assert
            // Assert expectancies are called in TearDown()
        }

        [Test]
        public void ContextMenuStrip_NestedCalculationGroup_AddCustomItems()
        {
            // Setup
            var group = new CalculationGroup();
            var parentGroup = new CalculationGroup();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var groupContext = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                                   parentGroup,
                                                                                   failureMechanism,
                                                                                   assessmentSection);
            var parentGroupContext = new GrassCoverErosionInwardsCalculationGroupContext(parentGroup,
                                                                                         null,
                                                                                         failureMechanism,
                                                                                         assessmentSection);
            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                gui.Stub(g => g.Get(groupContext, treeViewControl)).Return(menuBuilder);

                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(groupContext, parentGroupContext, treeViewControl))
                {
                    // Assert
                    Assert.AreEqual(21, menu.Items.Count);

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
                                                                  RingtoetsCommonFormsResources.FailureMechanismIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                  contextMenuUpdateDikeProfileAllIndexNestedGroup,
                                                                  "&Bijwerken dijkprofielen...",
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
        public void ContextMenuStrip_HydraulicBoundaryDatabaseNotLinked_ContextMenuItemCalculateAllDisabledAndTooltipSet()
        {
            // Setup
            var group = new CalculationGroup
            {
                Children =
                {
                    new GrassCoverErosionInwardsCalculation()
                }
            };

            var failureMechanism = new TestGrassCoverErosionInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation());

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);
            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               null,
                                                                               failureMechanism,
                                                                               assessmentSection);

            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var importHandler = mocks.Stub<IImportCommandHandler>();
            var exportHandler = mocks.Stub<IExportCommandHandler>();
            var updateHandler = mocks.Stub<IUpdateCommandHandler>();
            var viewCommandsHandler = mocks.Stub<IViewCommands>();

            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler,
                                                         importHandler,
                                                         exportHandler,
                                                         updateHandler,
                                                         viewCommandsHandler,
                                                         nodeData,
                                                         treeViewControl);
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());
                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuCalculateAllIndexRootGroup,
                                                                  "Alles be&rekenen",
                                                                  "Er is geen hydraulische belastingendatabase geïmporteerd.",
                                                                  RingtoetsCommonFormsResources.CalculateAllIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismWithoutCalculations_ContextMenuItemUpdateDikeProfileAllDisabledAndToolTipSet()
        {
            // Setup
            var group = new CalculationGroup();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               null,
                                                                               failureMechanism,
                                                                               assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuUpdateDikeProfileAllIndexRootGroup,
                                                                  "&Bijwerken dijkprofielen...",
                                                                  "Er zijn geen berekeningen om bij te werken.",
                                                                  RingtoetsCommonFormsResources.UpdateItemIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationGroupWithoutCalculations_ContextMenuItemUpdateDikeProfilesDisabledAndToolTipSet()
        {
            // Setup
            var group = new CalculationGroup();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               null,
                                                                               failureMechanism,
                                                                               assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuUpdateDikeProfileAllIndexRootGroup,
                                                                  "&Bijwerken dijkprofielen...",
                                                                  "Er zijn geen berekeningen om bij te werken.",
                                                                  RingtoetsCommonFormsResources.UpdateItemIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationGroupWithCalculationWithoutDikeProfile_ContextMenuItemUpdateDikeProfilesDisabledAndToolTipSet()
        {
            // Setup
            var group = new CalculationGroup
            {
                Children =
                {
                    new GrassCoverErosionInwardsCalculation()
                }
            };

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);
            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               null,
                                                                               failureMechanism,
                                                                               assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuUpdateDikeProfileAllIndexRootGroup,
                                                                  "&Bijwerken dijkprofielen...",
                                                                  "Er zijn geen berekeningen om bij te werken.",
                                                                  RingtoetsCommonFormsResources.UpdateItemIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationGroupWithCalculationWithDikeProfileAndInputInSync_ContextMenuItemUpdateDikeProfilesDisabledAndToolTipSet()
        {
            // Setup
            var group = new CalculationGroup
            {
                Children =
                {
                    new GrassCoverErosionInwardsCalculation
                    {
                        InputParameters =
                        {
                            DikeProfile = DikeProfileTestFactory.CreateDikeProfile()
                        }
                    }
                }
            };

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);
            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               null,
                                                                               failureMechanism,
                                                                               assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuUpdateDikeProfileAllIndexRootGroup,
                                                                  "&Bijwerken dijkprofielen...",
                                                                  "Er zijn geen berekeningen om bij te werken.",
                                                                  RingtoetsCommonFormsResources.UpdateItemIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationGroupWithCalculationWithDikeProfileAndInputOutOfSync_ContextMenuItemUpdateDikeProfilesEnabledAndToolTipSet()
        {
            // Setup
            DikeProfile dikeProfile = DikeProfileTestFactory.CreateDikeProfile();
            var group = new CalculationGroup
            {
                Children =
                {
                    new GrassCoverErosionInwardsCalculation
                    {
                        InputParameters =
                        {
                            DikeProfile = dikeProfile
                        }
                    }
                }
            };

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);
            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               null,
                                                                               failureMechanism,
                                                                               assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mocks.ReplayAll();

                plugin.Gui = gui;

                ChangeDikeProfile(dikeProfile);

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuUpdateDikeProfileAllIndexRootGroup,
                                                                  "&Bijwerken dijkprofielen...",
                                                                  "Alle berekeningen met een dijkprofiel bijwerken.",
                                                                  RingtoetsCommonFormsResources.UpdateItemIcon);
                }
            }
        }

        [Test]
        public void GivenCalculationWithoutOutputAndWithInputOutOfSync_WhenUpdateDikeProfilesClicked_ThenNoInquiryAndCalculationUpdatedAndInputObserverNotified()
        {
            // Given
            var calculation1InputObserver = mocks.StrictMock<IObserver>();
            calculation1InputObserver.Expect(obs => obs.UpdateObserver());
            var calculation2InputObserver = mocks.StrictMock<IObserver>();
            calculation2InputObserver.Expect(obs => obs.UpdateObserver());

            var calculation1Observer = mocks.StrictMock<IObserver>();
            var calculation2Observer = mocks.StrictMock<IObserver>();

            DikeProfile dikeProfile = DikeProfileTestFactory.CreateDikeProfile();
            var calculation1 = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = dikeProfile
                }
            };
            calculation1.Attach(calculation1Observer);
            calculation1.InputParameters.Attach(calculation1InputObserver);

            var calculation2 = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = dikeProfile
                }
            };
            calculation2.Attach(calculation2Observer);
            calculation2.InputParameters.Attach(calculation2InputObserver);

            var childGroup = new CalculationGroup();
            childGroup.Children.Add(calculation1);

            var emptyChildGroup = new CalculationGroup();
            var group = new CalculationGroup();
            group.Children.Add(childGroup);
            group.Children.Add(emptyChildGroup);
            group.Children.Add(calculation2);

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               null,
                                                                               failureMechanism,
                                                                               assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var mainWindow = mocks.Stub<IMainWindow>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                mocks.ReplayAll();

                plugin.Gui = gui;

                ChangeDikeProfile(dikeProfile);

                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    menu.Items[contextMenuUpdateDikeProfileAllIndexRootGroup].PerformClick();

                    // Then
                    Assert.IsTrue(calculation1.InputParameters.IsDikeProfileInputSynchronized);
                    Assert.IsTrue(calculation2.InputParameters.IsDikeProfileInputSynchronized);

                    // Note: observer assertions are verified in the TearDown()
                }
            }
        }

        [Test]
        public void GivenCalculationWithOutputAndInputOutOfSync_WhenUpdateDikeProfilesClickedAndCancelled_ThenInquiryAndCalculationNotUpdatedAndObserversNotNotified()
        {
            // Given
            var calculation1InputObserver = mocks.StrictMock<IObserver>();
            var calculation2InputObserver = mocks.StrictMock<IObserver>();

            var calculation1Observer = mocks.StrictMock<IObserver>();
            var calculation2Observer = mocks.StrictMock<IObserver>();

            DikeProfile dikeProfile = DikeProfileTestFactory.CreateDikeProfile();
            var calculation1 = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = dikeProfile
                },
                Output = new TestGrassCoverErosionInwardsOutput()
            };
            calculation1.Attach(calculation1Observer);
            calculation1.InputParameters.Attach(calculation1InputObserver);

            var calculation2 = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = dikeProfile
                },
                Output = new TestGrassCoverErosionInwardsOutput()
            };
            calculation2.Attach(calculation2Observer);
            calculation2.InputParameters.Attach(calculation2InputObserver);

            var childGroup = new CalculationGroup();
            childGroup.Children.Add(calculation1);

            var emptyChildGroup = new CalculationGroup();
            var group = new CalculationGroup();
            group.Children.Add(childGroup);
            group.Children.Add(emptyChildGroup);
            group.Children.Add(calculation2);

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               null,
                                                                               failureMechanism,
                                                                               assessmentSection);

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

                plugin.Gui = gui;

                ChangeDikeProfile(dikeProfile);

                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    menu.Items[contextMenuUpdateDikeProfileAllIndexRootGroup].PerformClick();

                    // Then
                    Assert.IsTrue(calculation1.HasOutput);
                    Assert.IsFalse(calculation1.InputParameters.IsDikeProfileInputSynchronized);
                    Assert.IsTrue(calculation2.HasOutput);
                    Assert.IsFalse(calculation2.InputParameters.IsDikeProfileInputSynchronized);

                    string expectedMessage = "Als u kiest voor bijwerken, dan wordt het resultaat van alle bij te werken berekeningen " +
                                             $"verwijderd.{Environment.NewLine}{Environment.NewLine}Weet u zeker dat u wilt doorgaan?";
                    Assert.AreEqual(expectedMessage, textBoxMessage);

                    // Note: observer assertions are verified in the TearDown()
                }
            }
        }

        [Test]
        public void GivenCalculationWithOutputAndInputOutOfSync_WhenUpdateDikeProfilesClickedAndContinued_ThenInquiryAndCalculationUpdatedAndObserversNotified()
        {
            // Given
            var calculation1InputObserver = mocks.StrictMock<IObserver>();
            calculation1InputObserver.Expect(obs => obs.UpdateObserver());
            var calculation2InputObserver = mocks.StrictMock<IObserver>();
            calculation2InputObserver.Expect(obs => obs.UpdateObserver());

            var calculation1Observer = mocks.StrictMock<IObserver>();
            calculation1Observer.Expect(obs => obs.UpdateObserver());
            var calculation2Observer = mocks.StrictMock<IObserver>();
            calculation2Observer.Expect(obs => obs.UpdateObserver());

            DikeProfile dikeProfile = DikeProfileTestFactory.CreateDikeProfile();
            var calculation1 = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = dikeProfile
                },
                Output = new TestGrassCoverErosionInwardsOutput()
            };
            calculation1.Attach(calculation1Observer);
            calculation1.InputParameters.Attach(calculation1InputObserver);

            var calculation2 = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = dikeProfile
                },
                Output = new TestGrassCoverErosionInwardsOutput()
            };
            calculation2.Attach(calculation2Observer);
            calculation2.InputParameters.Attach(calculation2InputObserver);

            var childGroup = new CalculationGroup();
            childGroup.Children.Add(calculation1);

            var emptyChildGroup = new CalculationGroup();
            var group = new CalculationGroup();
            group.Children.Add(childGroup);
            group.Children.Add(emptyChildGroup);
            group.Children.Add(calculation2);

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               null,
                                                                               failureMechanism,
                                                                               assessmentSection);
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

                plugin.Gui = gui;

                ChangeDikeProfile(dikeProfile);

                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    menu.Items[contextMenuUpdateDikeProfileAllIndexRootGroup].PerformClick();

                    // Then
                    Assert.IsFalse(calculation1.HasOutput);
                    Assert.IsTrue(calculation1.InputParameters.IsDikeProfileInputSynchronized);
                    Assert.IsFalse(calculation2.HasOutput);
                    Assert.IsTrue(calculation2.InputParameters.IsDikeProfileInputSynchronized);

                    string expectedMessage = "Als u kiest voor bijwerken, dan wordt het resultaat van alle bij te werken berekeningen " +
                                             $"verwijderd.{Environment.NewLine}{Environment.NewLine}Weet u zeker dat u wilt doorgaan?";
                    Assert.AreEqual(expectedMessage, textBoxMessage);

                    // Note: observer assertions are verified in the TearDown()
                }
            }
        }

        [Test]
        public void ContextMenuStrip_HydraulicBoundaryDatabaseLinkedToInvalidFile_ContextMenuItemCalculateAllDisabledAndTooltipSet()
        {
            // Setup
            var group = new CalculationGroup
            {
                Children =
                {
                    new GrassCoverErosionInwardsCalculation()
                }
            };

            var failureMechanism = new TestGrassCoverErosionInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation());

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(null, mocks, "invalidFilePath");

            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               null,
                                                                               failureMechanism,
                                                                               assessmentSection);

            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var importHandler = mocks.Stub<IImportCommandHandler>();
            var exportHandler = mocks.Stub<IExportCommandHandler>();
            var updateHandler = mocks.Stub<IUpdateCommandHandler>();
            var viewCommandsHandler = mocks.Stub<IViewCommands>();

            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler,
                                                         importHandler,
                                                         exportHandler,
                                                         updateHandler,
                                                         viewCommandsHandler,
                                                         nodeData,
                                                         treeViewControl);
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());
                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    ToolStripItem calculateContextMenuItem = contextMenu.Items[contextMenuCalculateAllIndexRootGroup];
                    Assert.AreEqual("Alles be&rekenen", calculateContextMenuItem.Text);
                    StringAssert.Contains("Herstellen van de verbinding met de hydraulische belastingendatabase is mislukt", calculateContextMenuItem.ToolTipText);
                    TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.CalculateAllIcon, calculateContextMenuItem.Image);
                    Assert.IsFalse(calculateContextMenuItem.Enabled);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_AllRequiredInputSet_ContextMenuItemCalculateAllEnabled()
        {
            // Setup
            var group = new CalculationGroup
            {
                Children =
                {
                    new GrassCoverErosionInwardsCalculation()
                }
            };

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var failureMechanism = new TestGrassCoverErosionInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation());

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "1.0"
            });

            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               null,
                                                                               failureMechanism,
                                                                               assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndexRootGroup,
                                                                  "Alles be&rekenen",
                                                                  "Voer alle berekeningen binnen deze map met berekeningen uit.",
                                                                  RingtoetsCommonFormsResources.CalculateAllIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_HydraulicBoundaryDatabaseNotLinked_ContextMenuItemValidateAllDisabledAndTooltipSet()
        {
            // Setup
            var group = new CalculationGroup
            {
                Children =
                {
                    new GrassCoverErosionInwardsCalculation()
                }
            };

            var failureMechanism = new TestGrassCoverErosionInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation());

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);
            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               null,
                                                                               failureMechanism,
                                                                               assessmentSection);

            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var importHandler = mocks.Stub<IImportCommandHandler>();
            var exportHandler = mocks.Stub<IExportCommandHandler>();
            var updateHandler = mocks.Stub<IUpdateCommandHandler>();
            var viewCommandsHandler = mocks.Stub<IViewCommands>();

            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler,
                                                         importHandler,
                                                         exportHandler,
                                                         updateHandler,
                                                         viewCommandsHandler,
                                                         nodeData,
                                                         treeViewControl);
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());
                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuValidateAllIndexRootGroup,
                                                                  "Alles &valideren",
                                                                  "Er is geen hydraulische belastingendatabase geïmporteerd.",
                                                                  RingtoetsCommonFormsResources.ValidateAllIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_HydraulicBoundaryDatabaseLinkedToInvalidFile_ContextMenuItemValidateAllDisabledAndTooltipSet()
        {
            // Setup
            var group = new CalculationGroup
            {
                Children =
                {
                    new GrassCoverErosionInwardsCalculation()
                }
            };

            var failureMechanism = new TestGrassCoverErosionInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation());

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(null, mocks, "invalidFilePath");

            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               null,
                                                                               failureMechanism,
                                                                               assessmentSection);

            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var importHandler = mocks.Stub<IImportCommandHandler>();
            var exportHandler = mocks.Stub<IExportCommandHandler>();
            var updateHandler = mocks.Stub<IUpdateCommandHandler>();
            var viewCommandsHandler = mocks.Stub<IViewCommands>();

            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler,
                                                         importHandler,
                                                         exportHandler,
                                                         updateHandler,
                                                         viewCommandsHandler,
                                                         nodeData,
                                                         treeViewControl);
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());
                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    ToolStripItem contextMenuItem = contextMenu.Items[contextMenuValidateAllIndexRootGroup];

                    Assert.AreEqual("Alles &valideren", contextMenuItem.Text);
                    StringAssert.Contains("Herstellen van de verbinding met de hydraulische belastingendatabase is mislukt.", contextMenuItem.ToolTipText);
                    TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.ValidateAllIcon, contextMenuItem.Image);
                    Assert.IsFalse(contextMenuItem.Enabled);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_AllRequiredInputSet_ContextMenuItemValidateAllEnabled()
        {
            // Setup
            var group = new CalculationGroup
            {
                Children =
                {
                    new GrassCoverErosionInwardsCalculation()
                }
            };

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var failureMechanism = new TestGrassCoverErosionInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation());

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "1.0"
            });

            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               null,
                                                                               failureMechanism,
                                                                               assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndexRootGroup,
                                                                  "Alles &valideren",
                                                                  "Valideer alle berekeningen binnen deze map met berekeningen.",
                                                                  RingtoetsCommonFormsResources.ValidateAllIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismWithoutDikeProfiles_ContextMenuItemGenerateCalculationsDisabledAndTooltipSet()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                               null,
                                                                               failureMechanism,
                                                                               assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuGenerateCalculationsIndexRootGroup,
                                                                  RingtoetsCommonFormsResources.CalculationGroup_Generate_calculations,
                                                                  GrassCoverErosionInwardsPluginResources.GrassCoverErosionInwardsPlugin_CreateGenerateCalculationsItem_NoDikeLocations_ToolTip,
                                                                  RingtoetsCommonFormsResources.GenerateScenariosIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismWithDikeProfiles_ContextMenuItemGenerateCalculationsDisabledAndTooltipSet()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.DikeProfiles.AddRange(new[]
            {
                DikeProfileTestFactory.CreateDikeProfile()
            }, dikeProfileCollectionPath);

            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                               null,
                                                                               failureMechanism,
                                                                               assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuGenerateCalculationsIndexRootGroup,
                                                                  RingtoetsCommonFormsResources.CalculationGroup_Generate_calculations,
                                                                  GrassCoverErosionInwardsPluginResources.GrassCoverErosionInwardsPlugin_CreateGenerateCalculationsItem_ToolTip,
                                                                  RingtoetsCommonFormsResources.GenerateScenariosIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_ClickOnCalculateAllItem_ValidateAllChildCalculations()
        {
            // Setup
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var failureMechanism = new TestGrassCoverErosionInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation
            {
                Name = "A",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(),
                    DikeProfile = DikeProfileTestFactory.CreateDikeProfile()
                }
            });
            failureMechanism.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation
            {
                Name = "B",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(),
                    DikeProfile = DikeProfileTestFactory.CreateDikeProfile()
                }
            });

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath
            });

            var groupContext = new GrassCoverErosionInwardsCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                                   null,
                                                                                   failureMechanism,
                                                                                   assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(groupContext, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

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
        public void ContextMenuStrip_ClickOnCalculateAllItem_ScheduleAllChildCalculations()
        {
            // Setup
            var mainWindow = mocks.Stub<IMainWindow>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var failureMechanism = new TestGrassCoverErosionInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation
            {
                Name = "A",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(),
                    DikeProfile = DikeProfileTestFactory.CreateDikeProfile()
                }
            });
            failureMechanism.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation
            {
                Name = "B",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(),
                    DikeProfile = DikeProfileTestFactory.CreateDikeProfile()
                }
            });

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.Id).Return(string.Empty);
            assessmentSection.Stub(a => a.FailureMechanismContribution)
                             .Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath
            });

            var groupContext = new GrassCoverErosionInwardsCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                                   null,
                                                                                   failureMechanism,
                                                                                   assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(groupContext, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                int nrOfCalculators = failureMechanism.Calculations.Count();
                var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                                 .WhenCalled(invocation =>
                                 {
                                     var settings = (HydraRingCalculationSettings) invocation.Arguments[0];
                                     Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                     Assert.IsEmpty(settings.PreprocessorDirectory);
                                 })
                                 .Return(new TestOvertoppingCalculator())
                                 .Repeat
                                 .Times(nrOfCalculators);
                mocks.ReplayAll();

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
                        StringAssert.StartsWith("De overloop en overslag berekening is uitgevoerd op de tijdelijke locatie", messageList[4]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(messageList[5]);
                        Assert.AreEqual("Uitvoeren van berekening 'A' is gelukt.", messageList[6]);

                        Assert.AreEqual("Uitvoeren van berekening 'B' is gestart.", messageList[7]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(messageList[8]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(messageList[9]);
                        CalculationServiceTestHelper.AssertCalculationStartMessage(messageList[10]);
                        StringAssert.StartsWith("De overloop en overslag berekening is uitgevoerd op de tijdelijke locatie", messageList[11]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(messageList[12]);
                        Assert.AreEqual("Uitvoeren van berekening 'B' is gelukt.", messageList[13]);
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
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               null,
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
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);
            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               null,
                                                                               failureMechanism,
                                                                               assessmentSection);
            var calculationItem = new GrassCoverErosionInwardsCalculation
            {
                Name = "Nieuwe berekening"
            };
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

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
                    Assert.IsInstanceOf<GrassCoverErosionInwardsCalculation>(newlyAddedItem);
                    Assert.AreEqual("Nieuwe berekening (1)", newlyAddedItem.Name,
                                    "An item with the same name default name already exists, therefore '(1)' needs to be appended.");
                }
            }
        }

        [Test]
        public void GivenCalculationsViewGenerateScenariosButtonClicked_WhenDikeProfileSelectedAndDialogClosed_ThenCalculationsAddedWithProfileAssigned()
        {
            // Given
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);

            DikeProfile dikeProfile1 = DikeProfileTestFactory.CreateDikeProfile("Dike profile 1", "id1");
            DikeProfile dikeProfile2 = DikeProfileTestFactory.CreateDikeProfile("Dike profile 2", "id2");

            var existingCalculationGroup = new CalculationGroup();
            var existingCalculation = new GrassCoverErosionInwardsCalculation();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
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
            failureMechanism.DikeProfiles.AddRange(new[]
            {
                dikeProfile1,
                dikeProfile2
            }, dikeProfileCollectionPath);

            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                               null,
                                                                               failureMechanism,
                                                                               assessmentSection);

            DialogBoxHandler = (name, wnd) =>
            {
                var selectionDialog = (GrassCoverErosionInwardsDikeProfileSelectionDialog) new FormTester(name).TheObject;
                var grid = (DataGridViewControl) new ControlTester("DataGridViewControl", selectionDialog).TheObject;

                grid.Rows[0].Cells[0].Value = true;

                new ButtonTester("DoForSelectedButton", selectionDialog).Click();
            };

            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var mainWindow = mocks.Stub<IMainWindow>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    contextMenu.Items[contextMenuGenerateCalculationsIndexRootGroup].PerformClick();

                    // Then
                    Assert.AreEqual(3, failureMechanism.CalculationsGroup.Children.Count);
                    Assert.AreSame(existingCalculationGroup, failureMechanism.CalculationsGroup.Children[0]);
                    Assert.AreSame(existingCalculation, failureMechanism.CalculationsGroup.Children[1]);
                    var generatedCalculation = failureMechanism.CalculationsGroup.Children[2] as GrassCoverErosionInwardsCalculation;
                    Assert.IsNotNull(generatedCalculation);
                    Assert.AreSame(dikeProfile1, generatedCalculation.InputParameters.DikeProfile);
                }
            }
        }

        [Test]
        public void GivenCalculationsViewGenerateScenariosButtonClicked_WhenCancelButtonClickedAndDialogClosed_ThenCalculationsNotUpdated()
        {
            // Given
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            DikeProfile dikeProfile1 = DikeProfileTestFactory.CreateDikeProfile("Dike profile 1", "id1");
            DikeProfile dikeProfile2 = DikeProfileTestFactory.CreateDikeProfile("Dike profile 2", "id2");

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.DikeProfiles.AddRange(new[]
            {
                dikeProfile1,
                dikeProfile2
            }, dikeProfileCollectionPath);

            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                               null,
                                                                               failureMechanism,
                                                                               assessmentSection);

            DialogBoxHandler = (name, wnd) =>

            {
                var selectionDialog = (GrassCoverErosionInwardsDikeProfileSelectionDialog) new FormTester(name).TheObject;
                var grid = (DataGridViewControl) new ControlTester("DataGridViewControl", selectionDialog).TheObject;

                grid.Rows[0].Cells[0].Value = true;

                new ButtonTester("CustomCancelButton", selectionDialog).Click();
            };

            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var mainWindow = mocks.Stub<IMainWindow>();

                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    contextMenu.Items[contextMenuGenerateCalculationsIndexRootGroup].PerformClick();

                    // Then
                    Assert.AreEqual(0, failureMechanism.Calculations.OfType<GrassCoverErosionInwardsCalculation>().Count());
                }
            }
        }

        [Test]
        public void GivenScenariosWithExistingCalculationWithSameName_WhenOkButtonClickedAndDialogClosed_ThenCalculationWithUniqueNameAdded()
        {
            // Given
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);

            const string existingCalculationName = "Dike profile";
            DikeProfile dikeProfile = DikeProfileTestFactory.CreateDikeProfile(existingCalculationName);

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        new GrassCoverErosionInwardsCalculation
                        {
                            Name = existingCalculationName
                        }
                    }
                }
            };
            failureMechanism.DikeProfiles.AddRange(new[]
            {
                dikeProfile
            }, dikeProfileCollectionPath);

            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                               null,
                                                                               failureMechanism,
                                                                               assessmentSection);

            DialogBoxHandler = (name, wnd) =>
            {
                var selectionDialog = (GrassCoverErosionInwardsDikeProfileSelectionDialog) new FormTester(name).TheObject;
                var grid = (DataGridViewControl) new ControlTester("DataGridViewControl", selectionDialog).TheObject;

                grid.Rows[0].Cells[0].Value = true;

                new ButtonTester("DoForSelectedButton", selectionDialog).Click();
            };

            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var mainWindow = mocks.Stub<IMainWindow>();

                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    string expectedNewName = NamingHelper.GetUniqueName(failureMechanism.CalculationsGroup.Children, existingCalculationName, c => c.Name);

                    // When
                    contextMenu.Items[contextMenuGenerateCalculationsIndexRootGroup].PerformClick();

                    // Then
                    GrassCoverErosionInwardsCalculation[] grassCoverErosionInwardsCalculations = failureMechanism.Calculations.OfType<GrassCoverErosionInwardsCalculation>().ToArray();
                    Assert.AreEqual(2, grassCoverErosionInwardsCalculations.Length);
                    Assert.AreEqual(expectedNewName, grassCoverErosionInwardsCalculations[1].Name);
                }
            }
        }

        [Test]
        public void OnNodeRemoved_NestedCalculationGroup_RemoveGroupAndNotifyObservers()
        {
            // Setup
            var observer = mocks.StrictMock<IObserver>();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var group = new CalculationGroup();
            var parentGroup = new CalculationGroup();
            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               parentGroup,
                                                                               failureMechanism,
                                                                               assessmentSection);
            var parentNodeData = new GrassCoverErosionInwardsCalculationGroupContext(parentGroup,
                                                                                     null,
                                                                                     failureMechanism,
                                                                                     assessmentSection);

            observer.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

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
            var observer = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var group = new CalculationGroup();
            var parentGroup = new CalculationGroup();
            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               parentGroup,
                                                                               failureMechanism,
                                                                               assessmentSection);
            var parentNodeData = new GrassCoverErosionInwardsCalculationGroupContext(parentGroup,
                                                                                     null,
                                                                                     failureMechanism,
                                                                                     assessmentSection);
            var calculation = new GrassCoverErosionInwardsCalculation();

            observer.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

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

        [Test]
        public void OnNodeRemoved_CalculationInGroupAssignedToSection_CalculationDetachedFromSection()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var group = new CalculationGroup();
            var parentGroup = new CalculationGroup();
            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               parentGroup,
                                                                               failureMechanism,
                                                                               assessmentSection);
            var parentNodeData = new GrassCoverErosionInwardsCalculationGroupContext(parentGroup,
                                                                                     null,
                                                                                     failureMechanism,
                                                                                     assessmentSection);

            mocks.ReplayAll();

            parentGroup.Children.Add(group);

            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                new FailureMechanismSection("section", new[]
                {
                    new Point2D(0, 0)
                })
            });

            var calculation = new GrassCoverErosionInwardsCalculation();
            group.Children.Add(calculation);
            GrassCoverErosionInwardsFailureMechanismSectionResult result = failureMechanism.SectionResults.First();
            result.Calculation = calculation;

            // Call
            info.OnNodeRemoved(nodeData, parentNodeData);

            // Assert
            Assert.IsNull(result.Calculation);
        }

        public override void Setup()
        {
            mocks = new MockRepository();
            gui = mocks.Stub<IGui>();
            plugin = new GrassCoverErosionInwardsPlugin
            {
                Gui = gui
            };

            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(GrassCoverErosionInwardsCalculationGroupContext));
        }

        public override void TearDown()
        {
            plugin.Dispose();
            mocks.VerifyAll();

            base.TearDown();
        }

        private static void ChangeDikeProfile(DikeProfile dikeProfile)
        {
            dikeProfile.CopyProperties(new DikeProfile(dikeProfile.WorldReferencePoint,
                                                       dikeProfile.DikeGeometry,
                                                       new[]
                                                       {
                                                           new Point2D(1.1, 2.2),
                                                           new Point2D(3.3, 4.4)
                                                       },
                                                       new BreakWater(BreakWaterType.Caisson, 10),
                                                       new DikeProfile.ConstructionProperties
                                                       {
                                                           Id = dikeProfile.Id,
                                                           DikeHeight = 10,
                                                           Orientation = 10
                                                       }));
        }
    }
}