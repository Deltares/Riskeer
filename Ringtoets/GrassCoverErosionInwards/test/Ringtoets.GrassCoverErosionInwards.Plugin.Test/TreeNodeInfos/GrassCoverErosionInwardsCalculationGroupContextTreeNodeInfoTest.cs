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
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Forms;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
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
        private const int contextMenuValidateAllIndexRootGroup = 8;
        private const int contextMenuCalculateAllIndexRootGroup = 9;
        private const int contextMenuClearAllIndexRootGroup = 11;

        private const int contextMenuAddCalculationGroupIndexNestedGroup = 3;
        private const int contextMenuAddCalculationIndexNestedGroup = 4;
        private const int contextMenuValidateAllIndexNestedGroup = 7;
        private const int contextMenuCalculateAllIndexNestedGroup = 8;
        private const int contextMenuClearAllIndexNestedGroup = 10;
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "HydraulicBoundaryDatabaseImporter");

        private IGui gui;
        private TreeNodeInfo info;
        private MockRepository mocks;
        private GrassCoverErosionInwardsPlugin plugin;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            gui = mocks.Stub<IGui>();
            plugin = new GrassCoverErosionInwardsPlugin
            {
                Gui = gui
            };

            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(GrassCoverErosionInwardsCalculationGroupContext));
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
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var groupContext = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                                   failureMechanism,
                                                                                   assessmentSection);

            // Call
            var children = info.ChildNodeObjects(groupContext);

            // Assert
            CollectionAssert.IsEmpty(children);
        }

        [Test]
        public void ChildNodeObjects_GroupWithMixedContents_ReturnChildren()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var calculationItemMock = mocks.StrictMock<ICalculationBase>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var group = new CalculationGroup();
            var childGroup = new CalculationGroup();
            var childCalculation = new GrassCoverErosionInwardsCalculation();

            group.Children.Add(childGroup);
            group.Children.Add(calculationItemMock);
            group.Children.Add(childCalculation);

            var groupContext = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                                   failureMechanism,
                                                                                   assessmentSection);

            // Call
            var children = info.ChildNodeObjects(groupContext).ToArray();

            // Assert
            Assert.AreEqual(group.Children.Count, children.Length);
            var calculationGroupContext = (GrassCoverErosionInwardsCalculationGroupContext) children[0];
            Assert.AreSame(childGroup, calculationGroupContext.WrappedData);
            Assert.AreSame(failureMechanism, calculationGroupContext.FailureMechanism);
            Assert.AreSame(assessmentSection, calculationGroupContext.AssessmentSection);
            Assert.AreSame(calculationItemMock, children[1]);
            var calculationContext = (GrassCoverErosionInwardsCalculationContext) children[2];
            Assert.AreSame(childCalculation, calculationContext.WrappedData);
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
                                                                                   failureMechanism,
                                                                                   assessmentSection);

            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();
            using (mocks.Ordered())
            {
                menuBuilderMock.Expect(mb => mb.AddImportItem()).IgnoreArguments().Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddExportItem()).IgnoreArguments().Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddDeleteChildrenItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.Build()).Return(null);
            }

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(cmp => cmp.Get(groupContext, treeViewControl)).Return(menuBuilderMock);
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
                    Assert.AreEqual(18, menu.Items.Count);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationGroupIndexRootGroup,
                                                                  RingtoetsCommonFormsResources.CalculationGroup_Add_CalculationGroup,
                                                                  RingtoetsCommonFormsResources.CalculationGroup_Add_CalculationGroup_Tooltip,
                                                                  RingtoetsCommonFormsResources.AddFolderIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationIndexRootGroup,
                                                                  RingtoetsCommonFormsResources.CalculationGroup_Add_Calculation,
                                                                  RingtoetsCommonFormsResources.CalculationGroup_Add_Calculation_Tooltip,
                                                                  RingtoetsCommonFormsResources.FailureMechanismIcon);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndexRootGroup,
                                                                  RingtoetsCommonFormsResources.Validate_all,
                                                                  RingtoetsCommonFormsResources.ValidateAll_No_calculations_to_validate,
                                                                  RingtoetsCommonFormsResources.ValidateAllIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndexRootGroup,
                                                                  RingtoetsCommonFormsResources.Calculate_all,
                                                                  RingtoetsCommonFormsResources.CalculationGroup_CalculateAll_No_calculations_to_run,
                                                                  RingtoetsCommonFormsResources.CalculateAllIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearAllIndexRootGroup,
                                                                  RingtoetsCommonFormsResources.Clear_all_output,
                                                                  RingtoetsCommonFormsResources.CalculationGroup_ClearOutput_No_calculation_with_output_to_clear,
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
                                                                                   failureMechanism,
                                                                                   assessmentSection);
            var parentGroupContext = new GrassCoverErosionInwardsCalculationGroupContext(parentGroup,
                                                                                         failureMechanism,
                                                                                         assessmentSection);
            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();
                using (mocks.Ordered())
                {
                    menuBuilderMock.Expect(mb => mb.AddImportItem()).IgnoreArguments().Return(menuBuilderMock);
                    menuBuilderMock.Expect(mb => mb.AddExportItem()).IgnoreArguments().Return(menuBuilderMock);
                    menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                    menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                    menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                    menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                    menuBuilderMock.Expect(mb => mb.AddRenameItem()).Return(menuBuilderMock);
                    menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                    menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                    menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                    menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                    menuBuilderMock.Expect(mb => mb.AddDeleteItem()).Return(menuBuilderMock);
                    menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                    menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
                    menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
                    menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                    menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
                    menuBuilderMock.Expect(mb => mb.Build()).Return(null);
                }

                gui.Stub(cmp => cmp.Get(groupContext, treeViewControl)).Return(menuBuilderMock);

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
                                                                                   failureMechanism,
                                                                                   assessmentSection);
            var parentGroupContext = new GrassCoverErosionInwardsCalculationGroupContext(parentGroup,
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
                    Assert.AreEqual(17, menu.Items.Count);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationGroupIndexNestedGroup,
                                                                  RingtoetsCommonFormsResources.CalculationGroup_Add_CalculationGroup,
                                                                  RingtoetsCommonFormsResources.CalculationGroup_Add_CalculationGroup_Tooltip,
                                                                  RingtoetsCommonFormsResources.AddFolderIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationIndexNestedGroup,
                                                                  RingtoetsCommonFormsResources.CalculationGroup_Add_Calculation,
                                                                  RingtoetsCommonFormsResources.CalculationGroup_Add_Calculation_Tooltip,
                                                                  RingtoetsCommonFormsResources.FailureMechanismIcon);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndexNestedGroup,
                                                                  RingtoetsCommonFormsResources.Validate_all,
                                                                  RingtoetsCommonFormsResources.ValidateAll_No_calculations_to_validate,
                                                                  RingtoetsCommonFormsResources.ValidateAllIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndexNestedGroup,
                                                                  RingtoetsCommonFormsResources.Calculate_all,
                                                                  RingtoetsCommonFormsResources.CalculationGroup_CalculateAll_No_calculations_to_run,
                                                                  RingtoetsCommonFormsResources.CalculateAllIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearAllIndexNestedGroup,
                                                                  RingtoetsCommonFormsResources.Clear_all_output,
                                                                  RingtoetsCommonFormsResources.CalculationGroup_ClearOutput_No_calculation_with_output_to_clear,
                                                                  RingtoetsCommonFormsResources.ClearIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_NoHydraulicBoundaryDatabase_ContextMenuItemCalculateAllDisabledAndTooltipSet()
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

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               failureMechanism,
                                                                               assessmentSection);

            var applicationFeatureCommandHandlerStub = mocks.Stub<IApplicationFeatureCommands>();
            var importHandler = mocks.Stub<IImportCommandHandler>();
            var exportHandler = mocks.Stub<IExportCommandHandler>();
            var updateHandler = mocks.Stub<IUpdateCommandHandler>();
            var viewCommandsHandler = mocks.Stub<IViewCommands>();

            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandlerStub,
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
                                                                  "Er is geen hydraulische randvoorwaardendatabase geïmporteerd.",
                                                                  RingtoetsCommonFormsResources.CalculateAllIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_HydraulicBoundaryDatabaseNotValid_ContextMenuItemCalculateAllDisabledAndTooltipSet()
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

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               failureMechanism,
                                                                               assessmentSection);

            var applicationFeatureCommandHandlerStub = mocks.Stub<IApplicationFeatureCommands>();
            var importHandler = mocks.Stub<IImportCommandHandler>();
            var exportHandler = mocks.Stub<IExportCommandHandler>();
            var updateHandler = mocks.Stub<IUpdateCommandHandler>();
            var viewCommandsHandler = mocks.Stub<IViewCommands>();

            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandlerStub,
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
                    StringAssert.Contains("Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt", calculateContextMenuItem.ToolTipText);
                    TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.CalculateAllIcon, calculateContextMenuItem.Image);
                    Assert.IsFalse(calculateContextMenuItem.Enabled);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismContributionZero_ContextMenuItemCalculateAllDisabledAndTooltipSet()
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

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation());

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "1.0"
            };

            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
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
                                                                  "De bijdrage van dit toetsspoor is nul.",
                                                                  RingtoetsCommonFormsResources.CalculateAllIcon,
                                                                  false);
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
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "1.0"
            };

            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
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
                                                                  "Voer alle berekeningen binnen deze berekeningsmap uit.",
                                                                  RingtoetsCommonFormsResources.CalculateAllIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_NoHydraulicBoundaryDatabase_ContextMenuItemValidateAllDisabledAndTooltipSet()
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

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               failureMechanism,
                                                                               assessmentSection);

            var applicationFeatureCommandHandlerStub = mocks.Stub<IApplicationFeatureCommands>();
            var importHandler = mocks.Stub<IImportCommandHandler>();
            var exportHandler = mocks.Stub<IExportCommandHandler>();
            var updateHandler = mocks.Stub<IUpdateCommandHandler>();
            var viewCommandsHandler = mocks.Stub<IViewCommands>();

            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandlerStub,
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
                                                                  "Er is geen hydraulische randvoorwaardendatabase geïmporteerd.",
                                                                  RingtoetsCommonFormsResources.ValidateAllIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_HydraulicBoundaryDatabaseNotValid_ContextMenuItemValidateAllDisabledAndTooltipSet()
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

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               failureMechanism,
                                                                               assessmentSection);

            var applicationFeatureCommandHandlerStub = mocks.Stub<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.Stub<IImportCommandHandler>();
            var exportHandlerMock = mocks.Stub<IExportCommandHandler>();
            var updateHandlerMock = mocks.Stub<IUpdateCommandHandler>();
            var viewCommandsHandlerMock = mocks.Stub<IViewCommands>();

            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandlerStub,
                                                         importHandlerMock,
                                                         exportHandlerMock,
                                                         updateHandlerMock,
                                                         viewCommandsHandlerMock,
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
                    StringAssert.Contains("Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt.", contextMenuItem.ToolTipText);
                    TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.ValidateAllIcon, contextMenuItem.Image);
                    Assert.IsFalse(contextMenuItem.Enabled);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismContributionZero_ContextMenuItemValidateAllDisabledAndTooltipSet()
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

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation());

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "1.0"
            };

            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
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
                                                                  "De bijdrage van dit toetsspoor is nul.",
                                                                  RingtoetsCommonFormsResources.ValidateAllIcon,
                                                                  false);
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
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "1.0"
            };

            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
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
                                                                  "Valideer alle berekeningen binnen deze berekeningsmap.",
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
                                                                               failureMechanism,
                                                                               assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                // Call
                using (var contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
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
            failureMechanism.DikeProfiles.Add(new TestDikeProfile());

            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(failureMechanism.CalculationsGroup,
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
                    DikeProfile = new TestDikeProfile()
                }
            });
            failureMechanism.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation
            {
                Name = "B",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(),
                    DikeProfile = new TestDikeProfile()
                }
            });

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var hydraulicBoundaryDatabaseStub = mocks.Stub<HydraulicBoundaryDatabase>();
            hydraulicBoundaryDatabaseStub.FilePath = validFilePath;

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabaseStub;

            var groupContext = new GrassCoverErosionInwardsCalculationGroupContext(failureMechanism.CalculationsGroup,
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
                        var messageList = messages.ToList();

                        // Assert
                        Assert.AreEqual(4, messageList.Count);
                        StringAssert.StartsWith("Validatie van 'A' gestart om: ", messageList[0]);
                        StringAssert.StartsWith("Validatie van 'A' beëindigd om: ", messageList[1]);
                        StringAssert.StartsWith("Validatie van 'B' gestart om: ", messageList[2]);
                        StringAssert.StartsWith("Validatie van 'B' beëindigd om: ", messageList[3]);
                    });
                }
            }
        }

        [Test]
        public void ContextMenuStrip_ClickOnCalculateAllItem_ScheduleAllChildCalculations()
        {
            // Setup
            var mainWindowStub = mocks.Stub<IMainWindow>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var failureMechanism = new TestGrassCoverErosionInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation
            {
                Name = "A",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(),
                    DikeProfile = new TestDikeProfile()
                }
            });
            failureMechanism.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation
            {
                Name = "B",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(),
                    DikeProfile = new TestDikeProfile()
                }
            });

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var hydraulicBoundaryDatabaseStub = mocks.Stub<HydraulicBoundaryDatabase>();
            hydraulicBoundaryDatabaseStub.FilePath = validFilePath;

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabaseStub;
            assessmentSection.Stub(a => a.Id).Return(string.Empty);
            assessmentSection.Stub(a => a.FailureMechanismContribution)
                             .Return(new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 1, 1));

            var groupContext = new GrassCoverErosionInwardsCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                                   failureMechanism,
                                                                                   assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(groupContext, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindowStub);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                plugin.Gui = gui;

                DialogBoxHandler = (name, wnd) =>
                {
                    // Expect an activity dialog which is automatically closed
                };

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(groupContext, null, treeViewControl))
                using (new HydraRingCalculatorFactoryConfig())
                {
                    // Call
                    TestHelper.AssertLogMessages(() => contextMenu.Items[contextMenuCalculateAllIndexRootGroup].PerformClick(), messages =>
                    {
                        var messageList = messages.ToList();

                        // Assert
                        Assert.AreEqual(12, messageList.Count);
                        StringAssert.StartsWith("Validatie van 'A' gestart om: ", messageList[0]);
                        StringAssert.StartsWith("Validatie van 'A' beëindigd om: ", messageList[1]);
                        StringAssert.StartsWith("Berekening van 'A' gestart om: ", messageList[2]);
                        StringAssert.StartsWith("Overloop berekening is uitgevoerd op de tijdelijke locatie", messageList[3]);
                        StringAssert.StartsWith("Berekening van 'A' beëindigd om: ", messageList[4]);
                        StringAssert.StartsWith("Validatie van 'B' gestart om: ", messageList[5]);
                        StringAssert.StartsWith("Validatie van 'B' beëindigd om: ", messageList[6]);
                        StringAssert.StartsWith("Berekening van 'B' gestart om: ", messageList[7]);
                        StringAssert.StartsWith("Overloop berekening is uitgevoerd op de tijdelijke locatie", messageList[8]);
                        StringAssert.StartsWith("Berekening van 'B' beëindigd om: ", messageList[9]);
                        Assert.AreEqual("Uitvoeren van 'A' is gelukt.", messageList[10]);
                        Assert.AreEqual("Uitvoeren van 'B' is gelukt.", messageList[11]);
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
                                                                               failureMechanism,
                                                                               assessmentSection);
            var calculationGroup = new CalculationGroup
            {
                Name = "Nieuwe map"
            };
            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                group.Children.Add(calculationGroup);
                nodeData.Attach(observerMock);

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Precondition
                    Assert.AreEqual(1, group.Children.Count);

                    // Call
                    contextMenu.Items[contextMenuAddCalculationGroupIndexRootGroup].PerformClick();

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
        public void ContextMenuStrip_ClickOnAddCalculationItem_AddCalculationToCalculationGroupAndNotifyObservers()
        {
            // Setup
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            var group = new CalculationGroup();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               failureMechanism,
                                                                               assessmentSection);
            var calculationItem = new GrassCoverErosionInwardsCalculation
            {
                Name = "Nieuwe berekening"
            };
            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                group.Children.Add(calculationItem);
                nodeData.Attach(observerMock);

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Precondition
                    Assert.AreEqual(1, group.Children.Count);

                    // Call
                    contextMenu.Items[contextMenuAddCalculationIndexRootGroup].PerformClick();

                    // Assert
                    Assert.AreEqual(2, group.Children.Count);
                    var newlyAddedItem = group.Children.Last();
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
            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSection = mocks.Stub<IAssessmentSection>();

                DikeProfile dikeProfile1 = new TestDikeProfile("Dike profile 1");
                DikeProfile dikeProfile2 = new TestDikeProfile("Dike profile 2");

                var existingCalculationGroup = new CalculationGroup();
                var existingCalculation = new GrassCoverErosionInwardsCalculation();
                var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
                {
                    DikeProfiles =
                    {
                        dikeProfile1,
                        dikeProfile2
                    },
                    CalculationsGroup =
                    {
                        Children =
                        {
                            existingCalculationGroup,
                            existingCalculation
                        }
                    }
                };

                var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                                   failureMechanism,
                                                                                   assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var mainWindow = mocks.Stub<IMainWindow>();

                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                plugin.Gui = gui;

                DialogBoxHandler = (name, wnd) =>
                {
                    var selectionDialog = (GrassCoverErosionInwardsDikeProfileSelectionDialog) new FormTester(name).TheObject;
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
            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSection = mocks.Stub<IAssessmentSection>();

                DikeProfile dikeProfile1 = new TestDikeProfile("Dike profile 1");
                DikeProfile dikeProfile2 = new TestDikeProfile("Dike profile 2");

                var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
                {
                    DikeProfiles =
                    {
                        dikeProfile1,
                        dikeProfile2
                    }
                };

                var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                                   failureMechanism,
                                                                                   assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var mainWindow = mocks.Stub<IMainWindow>();

                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                plugin.Gui = gui;

                DialogBoxHandler = (name, wnd) =>
                {
                    var selectionDialog = (GrassCoverErosionInwardsDikeProfileSelectionDialog) new FormTester(name).TheObject;
                    var grid = (DataGridViewControl) new ControlTester("DataGridViewControl", selectionDialog).TheObject;

                    grid.Rows[0].Cells[0].Value = true;

                    new ButtonTester("CustomCancelButton", selectionDialog).Click();
                };

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
            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSection = mocks.Stub<IAssessmentSection>();

                const string existingCalculationName = "Dike profile";
                DikeProfile dikeProfile = new TestDikeProfile(existingCalculationName);

                var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
                {
                    DikeProfiles =
                    {
                        dikeProfile
                    },
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

                var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                                   failureMechanism,
                                                                                   assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var mainWindow = mocks.Stub<IMainWindow>();

                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                plugin.Gui = gui;

                DialogBoxHandler = (name, wnd) =>
                {
                    var selectionDialog = (GrassCoverErosionInwardsDikeProfileSelectionDialog) new FormTester(name).TheObject;
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
                    var grassCoverErosionInwardsCalculations = failureMechanism.Calculations.OfType<GrassCoverErosionInwardsCalculation>().ToArray();
                    Assert.AreEqual(2, grassCoverErosionInwardsCalculations.Length);
                    Assert.AreEqual(expectedNewName, grassCoverErosionInwardsCalculations[1].Name);
                }
            }
        }

        [Test]
        public void OnNodeRemoved_NestedCalculationGroup_RemoveGroupAndNotifyObservers()
        {
            // Setup
            var observerMock = mocks.StrictMock<IObserver>();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var group = new CalculationGroup();
            var parentGroup = new CalculationGroup();
            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               failureMechanism,
                                                                               assessmentSection);
            var parentNodeData = new GrassCoverErosionInwardsCalculationGroupContext(parentGroup,
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
        public void OnNodeRemoved_NestedCalculationGroupContainingCalculations_RemoveGroupAndCalculationsAndNotifyObservers()
        {
            // Setup
            var observerMock = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var group = new CalculationGroup();
            var parentGroup = new CalculationGroup();
            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               failureMechanism,
                                                                               assessmentSection);
            var parentNodeData = new GrassCoverErosionInwardsCalculationGroupContext(parentGroup,
                                                                                     failureMechanism,
                                                                                     assessmentSection);
            var calculation = new GrassCoverErosionInwardsCalculation();

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

        [Test]
        public void OnNodeRemoved_CalculationInGroupAssignedToSection_CalculationDetachedFromSection()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var group = new CalculationGroup();
            var parentGroup = new CalculationGroup();
            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               failureMechanism,
                                                                               assessmentSection);
            var parentNodeData = new GrassCoverErosionInwardsCalculationGroupContext(parentGroup,
                                                                                     failureMechanism,
                                                                                     assessmentSection);

            mocks.ReplayAll();

            parentGroup.Children.Add(group);

            failureMechanism.AddSection(new FailureMechanismSection("section", new[]
            {
                new Point2D(0, 0)
            }));

            var calculation = new GrassCoverErosionInwardsCalculation();
            group.Children.Add(calculation);
            GrassCoverErosionInwardsFailureMechanismSectionResult result = failureMechanism.SectionResults.First();
            result.Calculation = calculation;

            // Call
            info.OnNodeRemoved(nodeData, parentNodeData);

            // Assert
            Assert.IsNull(result.Calculation);
        }

        public override void TearDown()
        {
            plugin.Dispose();
            mocks.VerifyAll();

            base.TearDown();
        }
    }
}