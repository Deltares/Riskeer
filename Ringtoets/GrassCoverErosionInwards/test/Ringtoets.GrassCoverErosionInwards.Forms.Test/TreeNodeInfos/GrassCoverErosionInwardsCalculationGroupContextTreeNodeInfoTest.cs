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
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Plugin;
using Ringtoets.HydraRing.Data;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using GrassCoverErosionInwardsFormResources = Ringtoets.GrassCoverErosionInwards.Forms.Properties.Resources;
using GrassCoverErosionInwardsPluginResources = Ringtoets.GrassCoverErosionInwards.Plugin.Properties.Resources;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationGroupContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuGenerateCalculationsIndexRootGroup = 0;
        private const int contextMenuAddCalculationGroupIndexRootGroup = 2;
        private const int contextMenuAddCalculationIndexRootGroup = 3;
        private const int contextMenuValidateAllIndexRootGroup = 7;
        private const int contextMenuCalculateAllIndexRootGroup = 8;
        private const int contextMenuClearAllIndexRootGroup = 9;

        private const int contextMenuAddCalculationGroupIndexNestedGroup = 0;
        private const int contextMenuAddCalculationIndexNestedGroup = 1;
        private const int contextMenuValidateAllIndexNestedGroup = 3;
        private const int contextMenuCalculateAllIndexNestedGroup = 4;
        private const int contextMenuClearAllIndexNestedGroup = 5;
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.IO, "HydraulicBoundaryLocationReader");

        private IGui guiMock;
        private TreeNodeInfo info;
        private MockRepository mocks;
        private GrassCoverErosionInwardsPlugin plugin;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            guiMock = mocks.StrictMock<IGui>();
            plugin = new GrassCoverErosionInwardsPlugin
            {
                Gui = guiMock
            };

            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(GrassCoverErosionInwardsCalculationGroupContext));
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            mocks.ReplayAll();

            // Assert
            Assert.AreEqual(typeof(GrassCoverErosionInwardsCalculationGroupContext), info.TagType);
            Assert.IsNotNull(info.Text);
            Assert.IsNotNull(info.Image);
            Assert.IsNotNull(info.EnsureVisibleOnCreate);
            Assert.IsNotNull(info.ChildNodeObjects);
            Assert.IsNotNull(info.ContextMenuStrip);
            Assert.IsNotNull(info.CanRename);
            Assert.IsNotNull(info.OnNodeRenamed);
            Assert.IsNotNull(info.CanRemove);
            Assert.IsNotNull(info.OnNodeRemoved);
            Assert.IsNotNull(info.CanDrag);
            Assert.IsNotNull(info.CanInsert);
            Assert.IsNotNull(info.CanDrop);
            Assert.IsNotNull(info.OnDrop);
            Assert.IsNull(info.ForeColor);
            Assert.IsNull(info.CanCheck);
            Assert.IsNull(info.IsChecked);
            Assert.IsNull(info.OnNodeChecked);
        }

        [Test]
        public void ChildNodeObjects_EmptyGroup_ReturnEmpty()
        {
            // Setup
            var group = new CalculationGroup();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var groupContext = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                                   failureMechanism,
                                                                                   assessmentSectionMock);

            // Call
            var children = info.ChildNodeObjects(groupContext);

            // Assert
            CollectionAssert.IsEmpty(children);
        }

        [Test]
        public void ChildNodeObjects_GroupWithMixedContents_ReturnChildren()
        {
            // Setup
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
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
                                                                                   assessmentSectionMock);

            // Call
            var children = info.ChildNodeObjects(groupContext).ToArray();

            // Assert
            Assert.AreEqual(group.Children.Count, children.Length);
            var calculationGroupContext = (GrassCoverErosionInwardsCalculationGroupContext) children[0];
            Assert.AreSame(childGroup, calculationGroupContext.WrappedData);
            Assert.AreSame(failureMechanism, calculationGroupContext.FailureMechanism);
            Assert.AreSame(assessmentSectionMock, calculationGroupContext.AssessmentSection);
            Assert.AreSame(calculationItemMock, children[1]);
            var calculationContext = (GrassCoverErosionInwardsCalculationContext) children[2];
            Assert.AreSame(childCalculation, calculationContext.WrappedData);
            Assert.AreSame(assessmentSectionMock, calculationContext.AssessmentSection);
        }

        [Test]
        public void ContextMenuStrip_WithoutParentNodeDefaultBehavior_CallsContextMenuBuilderMethods()
        {
            // Setup
            var group = new CalculationGroup();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var groupContext = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                                   failureMechanism,
                                                                                   assessmentSectionMock);

            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();

            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddDeleteChildrenItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(cmp => cmp.Get(groupContext, treeViewControl)).Return(menuBuilderMock);
                guiMock.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());
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
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var groupContext = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                                   failureMechanism,
                                                                                   assessmentSectionMock);
            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                guiMock.Expect(g => g.Get(groupContext, treeViewControl)).Return(menuBuilder);
                guiMock.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(groupContext, null, treeViewControl))
                {
                    // Assert
                    Assert.AreEqual(14, menu.Items.Count);
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
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var groupContext = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                                   failureMechanism,
                                                                                   assessmentSectionMock);
            var parentGroupContext = new GrassCoverErosionInwardsCalculationGroupContext(parentGroup,
                                                                                         failureMechanism,
                                                                                         assessmentSectionMock);
            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();

                menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddRenameItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddDeleteItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.Build()).Return(null);
                guiMock.Expect(cmp => cmp.Get(groupContext, treeViewControl)).Return(menuBuilderMock);

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
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var groupContext = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                                   failureMechanism,
                                                                                   assessmentSectionMock);
            var parentGroupContext = new GrassCoverErosionInwardsCalculationGroupContext(parentGroup,
                                                                                         failureMechanism,
                                                                                         assessmentSectionMock);
            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                guiMock.Expect(g => g.Get(groupContext, treeViewControl)).Return(menuBuilder);

                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(groupContext, parentGroupContext, treeViewControl))
                {
                    // Assert
                    Assert.AreEqual(13, menu.Items.Count);

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
        public void ContextMenuStrip_NoFailureMechanismSections_ContextMenuItemCalculateAllDisabledAndTooltipSet()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var group = new CalculationGroup
            {
                Children =
                {
                    new GrassCoverErosionInwardsCalculation()
                }
            };

            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               failureMechanism,
                                                                               assessmentSectionMock);

            var applicationFeatureCommandHandlerStub = mocks.Stub<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsHandlerMock = mocks.StrictMock<IViewCommands>();

            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandlerStub,
                                                         importHandlerMock,
                                                         exportHandlerMock,
                                                         viewCommandsHandlerMock,
                                                         nodeData,
                                                         treeViewControl);
                guiMock.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                guiMock.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());
                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndexRootGroup,
                                                                  RingtoetsCommonFormsResources.Calculate_all,
                                                                  RingtoetsCommonFormsResources.Plugin_AllDataAvailable_No_failure_mechanism_sections_imported,
                                                                  RingtoetsCommonFormsResources.CalculateAllIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismSectionsSetNoHydraulicBoundaryDatabase_ContextMenuItemCalculateAllDisabledAndTooltipSet()
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
            failureMechanism.AddSection(new FailureMechanismSection("test", new[]
            {
                new Point2D(0, 0)
            }));
            failureMechanism.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation());

            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            assessmentSectionMock.Stub(asm => asm.HydraulicBoundaryDatabase).Return(null);

            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               failureMechanism,
                                                                               assessmentSectionMock);

            var applicationFeatureCommandHandlerStub = mocks.Stub<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsHandlerMock = mocks.StrictMock<IViewCommands>();

            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandlerStub,
                                                         importHandlerMock,
                                                         exportHandlerMock,
                                                         viewCommandsHandlerMock,
                                                         nodeData,
                                                         treeViewControl);
                guiMock.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                guiMock.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());
                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuCalculateAllIndexRootGroup,
                                                                  RingtoetsCommonFormsResources.Calculate_all,
                                                                  RingtoetsCommonFormsResources.Plugin_AllDataAvailable_No_hydraulic_boundary_database_imported,
                                                                  RingtoetsCommonFormsResources.CalculateAllIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismSectionsSetHydraulicBoundaryDatabaseNotValid_ContextMenuItemCalculateAllDisabledAndTooltipSet()
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
            failureMechanism.AddSection(new FailureMechanismSection("test", new[]
            {
                new Point2D(0, 0)
            }));
            failureMechanism.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation());

            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            assessmentSectionMock.Stub(asm => asm.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase());

            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               failureMechanism,
                                                                               assessmentSectionMock);

            var applicationFeatureCommandHandlerStub = mocks.Stub<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsHandlerMock = mocks.StrictMock<IViewCommands>();

            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandlerStub,
                                                         importHandlerMock,
                                                         exportHandlerMock,
                                                         viewCommandsHandlerMock,
                                                         nodeData,
                                                         treeViewControl);
                guiMock.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                guiMock.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());
                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    ToolStripItem contextMenuItem = contextMenu.Items[contextMenuCalculateAllIndexRootGroup];

                    Assert.AreEqual(RingtoetsCommonFormsResources.Calculate_all, contextMenuItem.Text);
                    StringAssert.Contains(string.Format(RingtoetsCommonServiceResources.Hydraulic_boundary_database_connection_failed_0_, ""), contextMenuItem.ToolTipText);
                    TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.CalculateAllIcon, contextMenuItem.Image);
                    Assert.IsFalse(contextMenuItem.Enabled);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismSectionsAndHydraulicDatabaseSet_ContextMenuItemCalculateAllEnabled()
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

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "1.0"
            };

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test", new[]
            {
                new Point2D(0, 0)
            }));
            failureMechanism.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation());

            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            assessmentSectionMock.Stub(asm => asm.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase);

            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               failureMechanism,
                                                                               assessmentSectionMock);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                guiMock.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndexRootGroup,
                                                                  RingtoetsCommonFormsResources.Calculate_all,
                                                                  RingtoetsCommonFormsResources.CalculationGroup_CalculateAll_ToolTip,
                                                                  RingtoetsCommonFormsResources.CalculateAllIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismWithoutDikeProfiles_ContextMenuItemGenerateCalculationsDisabledAndTooltipSet()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            assessmentSectionMock.Stub(asm => asm.HydraulicBoundaryDatabase).Return(null);

            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                               failureMechanism,
                                                                               assessmentSectionMock);

            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                guiMock.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                guiMock.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                // Call
                using (var contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuGenerateCalculationsIndexRootGroup,
                                                                  RingtoetsCommonFormsResources.CalculationGroup_Generate_Scenarios,
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
            failureMechanism.DikeProfiles.Add(CreateDikeProfile());

            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            assessmentSectionMock.Stub(asm => asm.HydraulicBoundaryDatabase).Return(null);

            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                               failureMechanism,
                                                                               assessmentSectionMock);

            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                guiMock.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                guiMock.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuGenerateCalculationsIndexRootGroup,
                                                                  RingtoetsCommonFormsResources.CalculationGroup_Generate_Scenarios,
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

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            failureMechanism.AddSection(new FailureMechanismSection("A", new[]
            {
                new Point2D(0, 0)
            }));

            failureMechanism.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation
            {
                Name = "A",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(-1, "nonExisting", 1, 2),
                    DikeProfile = CreateDikeProfile()
                }
            });

            failureMechanism.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation
            {
                Name = "B",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(-1, "nonExisting", 1, 2),
                    DikeProfile = CreateDikeProfile()
                }
            });

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var hydraulicBoundaryDatabaseStub = mocks.Stub<HydraulicBoundaryDatabase>();
            hydraulicBoundaryDatabaseStub.FilePath = validFilePath;

            var assessmentSectionMock = mocks.Stub<IAssessmentSection>();
            assessmentSectionMock.HydraulicBoundaryDatabase = hydraulicBoundaryDatabaseStub;

            var groupContext = new GrassCoverErosionInwardsCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                                   failureMechanism,
                                                                                   assessmentSectionMock);

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(g => g.Get(groupContext, treeViewControl)).Return(menuBuilder);
                guiMock.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                plugin.Gui = guiMock;

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

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            failureMechanism.AddSection(new FailureMechanismSection("A", new[]
            {
                new Point2D(0, 0),
                new Point2D(1.1, 1.1)
            }));

            failureMechanism.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation
            {
                Name = "A",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(-1, "nonExisting", 1, 2),
                    DikeProfile = CreateDikeProfile()
                }
            });

            failureMechanism.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation
            {
                Name = "B",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(-1, "nonExisting", 1, 2),
                    DikeProfile = CreateDikeProfile()
                }
            });

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var hydraulicBoundaryDatabaseStub = mocks.Stub<HydraulicBoundaryDatabase>();
            hydraulicBoundaryDatabaseStub.FilePath = validFilePath;

            var assessmentSectionMock = mocks.Stub<IAssessmentSection>();
            assessmentSectionMock.HydraulicBoundaryDatabase = hydraulicBoundaryDatabaseStub;
            assessmentSectionMock.Stub(a => a.Id).Return(string.Empty);
            assessmentSectionMock.Stub(a => a.FailureMechanismContribution)
                .Return(new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(),1,1));

            var groupContext = new GrassCoverErosionInwardsCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                                   failureMechanism,
                                                                                   assessmentSectionMock);

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(g => g.Get(groupContext, treeViewControl)).Return(menuBuilder);
                guiMock.Expect(g => g.MainWindow).Return(mainWindowStub);
                guiMock.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                plugin.Gui = guiMock;

                DialogBoxHandler = (name, wnd) =>
                {
                    // Expect an activity dialog which is automatically closed
                };

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(groupContext, null, treeViewControl))
                {
                    // Call
                    TestHelper.AssertLogMessages(() => contextMenu.Items[contextMenuCalculateAllIndexRootGroup].PerformClick(), messages =>
                    {
                        var messageList = messages.ToList();

                        // Assert
                        Assert.AreEqual(14, messageList.Count);
                        StringAssert.StartsWith("Validatie van 'A' gestart om: ", messageList[0]);
                        StringAssert.StartsWith("Validatie van 'A' beëindigd om: ", messageList[1]);
                        StringAssert.StartsWith("Berekening van 'A' gestart om: ", messageList[2]);
                        Assert.AreEqual("De berekening voor grasbekleding erosie kruin en binnentalud 'A' is niet gelukt.", messageList[3]);
                        StringAssert.StartsWith("Overloop berekeningsverslag. Klik op details voor meer informatie.", messageList[4]);
                        StringAssert.StartsWith("Berekening van 'A' beëindigd om: ", messageList[5]);
                        StringAssert.StartsWith("Validatie van 'B' gestart om: ", messageList[6]);
                        StringAssert.StartsWith("Validatie van 'B' beëindigd om: ", messageList[7]);
                        StringAssert.StartsWith("Berekening van 'B' gestart om: ", messageList[8]);
                        Assert.AreEqual("De berekening voor grasbekleding erosie kruin en binnentalud 'B' is niet gelukt.", messageList[9]);
                        StringAssert.StartsWith("Overloop berekeningsverslag. Klik op details voor meer informatie.", messageList[10]);
                        StringAssert.StartsWith("Berekening van 'B' beëindigd om: ", messageList[11]);
                        Assert.AreEqual("Uitvoeren van 'A' is mislukt.", messageList[12]);
                        Assert.AreEqual("Uitvoeren van 'B' is mislukt.", messageList[13]);
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
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               failureMechanism,
                                                                               assessmentSectionMock);
            var calculationGroup = new CalculationGroup
            {
                Name = "Nieuwe map"
            };
            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                guiMock.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

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
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               failureMechanism,
                                                                               assessmentSectionMock);
            var calculationItem = new GrassCoverErosionInwardsCalculation
            {
                Name = "Nieuwe berekening"
            };
            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                guiMock.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

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
                var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

                DikeProfile dikeProfile1 = CreateDikeProfile("Dike profile 1");
                DikeProfile dikeProfile2 = CreateDikeProfile("Dike profile 2");

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
                                                                                   assessmentSectionMock);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var mainWindow = mocks.Stub<IMainWindow>();

                guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                guiMock.Expect(g => g.MainWindow).Return(mainWindow);
                guiMock.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                plugin.Gui = guiMock;

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
                    var grassCoverErosionInwardsCalculations = failureMechanism.Calculations.OfType<GrassCoverErosionInwardsCalculation>().ToArray();
                    Assert.AreEqual(1, grassCoverErosionInwardsCalculations.Length);
                    Assert.AreSame(dikeProfile1, grassCoverErosionInwardsCalculations[0].InputParameters.DikeProfile);
                }
            }
        }

        [Test]
        public void GivenCalculationsViewGenerateScenariosButtonClicked_WhenCancelButtonClickedAndDialogClosed_ThenCalculationsNotUpdated()
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

                DikeProfile dikeProfile1 = CreateDikeProfile("Dike profile 1");
                DikeProfile dikeProfile2 = CreateDikeProfile("Dike profile 2");

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
                                                                                   assessmentSectionMock);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var mainWindow = mocks.Stub<IMainWindow>();

                guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                guiMock.Expect(g => g.MainWindow).Return(mainWindow);
                guiMock.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                plugin.Gui = guiMock;

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
                var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

                var existingCalculationName = "Dike profile";
                DikeProfile dikeProfile = CreateDikeProfile(existingCalculationName);

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
                                                                                   assessmentSectionMock);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var mainWindow = mocks.Stub<IMainWindow>();

                guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                guiMock.Expect(g => g.MainWindow).Return(mainWindow);
                guiMock.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                plugin.Gui = guiMock;

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
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var group = new CalculationGroup();
            var parentGroup = new CalculationGroup();
            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               failureMechanism,
                                                                               assessmentSectionMock);
            var parentNodeData = new GrassCoverErosionInwardsCalculationGroupContext(parentGroup,
                                                                                     failureMechanism,
                                                                                     assessmentSectionMock);

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
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var group = new CalculationGroup();
            var parentGroup = new CalculationGroup();
            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               failureMechanism,
                                                                               assessmentSectionMock);
            var parentNodeData = new GrassCoverErosionInwardsCalculationGroupContext(parentGroup,
                                                                                     failureMechanism,
                                                                                     assessmentSectionMock);
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
            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();
            var group = new CalculationGroup();
            var parentGroup = new CalculationGroup();
            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                       failureMechanism,
                                                                       assessmentSectionStub);
            var parentNodeData = new GrassCoverErosionInwardsCalculationGroupContext(parentGroup,
                                                                             failureMechanism,
                                                                             assessmentSectionStub);

            mocks.ReplayAll();

            parentGroup.Children.Add(group);

            failureMechanism.AddSection(new FailureMechanismSection("section", new[]
            {
                new Point2D(0, 0)
            }));

            var calculation = new GrassCoverErosionInwardsCalculation();
            group.Children.Add(calculation);
            failureMechanism.SectionResults.First().Calculation = calculation;

            // Call
            info.OnNodeRemoved(nodeData, parentNodeData);

            // Assert
            Assert.IsNull(failureMechanism.SectionResults.First().Calculation);
        }

        public override void TearDown()
        {
            plugin.Dispose();
            mocks.VerifyAll();

            base.TearDown();
        }

        private static DikeProfile CreateDikeProfile(string name = null)
        {
            return new DikeProfile(new Point2D(1, 2), new RoughnessPoint[0], new Point2D[0],
                                   null, new DikeProfile.ConstructionProperties
                                   {
                                       Name = name
                                   });
        }
    }
}