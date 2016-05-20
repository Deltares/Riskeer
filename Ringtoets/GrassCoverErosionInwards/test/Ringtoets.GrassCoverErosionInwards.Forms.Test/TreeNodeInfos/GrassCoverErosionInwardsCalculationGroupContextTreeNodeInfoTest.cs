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
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Plugin;
using Ringtoets.GrassCoverErosionInwards.Plugin.Properties;
using Ringtoets.HydraRing.Data;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;
using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using GrassCoverErosionInwardsFormResources = Ringtoets.GrassCoverErosionInwards.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationGroupContextTreeNodeInfoTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.IO, "HydraulicBoundaryLocationReader");

        private IGui guiMock;
        private TreeNodeInfo info;
        private MockRepository mocks;
        private GrassCoverErosionInwardsGuiPlugin plugin;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            guiMock = mocks.StrictMock<IGui>();
            plugin = new GrassCoverErosionInwardsGuiPlugin
            {
                Gui = guiMock
            };

            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(GrassCoverErosionInwardsCalculationGroupContext));
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
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
            mocks.VerifyAll();
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
            var childCalculation = new GrassCoverErosionInwardsCalculation(failureMechanism.GeneralInput, failureMechanism.NormProbabilityInput);

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
            mocks.VerifyAll();
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
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();

            menuBuilderMock.Expect(mb => mb.AddOpenItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddImportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);
            guiMock.Expect(cmp => cmp.Get(groupContext, treeViewControlMock)).Return(menuBuilderMock);

            mocks.ReplayAll();

            // Call
            info.ContextMenuStrip(groupContext, null, treeViewControlMock);

            // Assert
            mocks.VerifyAll();
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
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            guiMock.Expect(g => g.Get(groupContext, treeViewControlMock)).Return(menuBuilder);

            mocks.ReplayAll();

            // Call
            ContextMenuStrip menu = info.ContextMenuStrip(groupContext, null, treeViewControlMock);

            // Assert
            Assert.AreEqual(9, menu.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationGroupIndexRootGroup,
                                                          RingtoetsFormsResources.CalculationGroup_Add_CalculationGroup,
                                                          RingtoetsFormsResources.CalculationGroup_Add_CalculationGroup_Tooltip,
                                                          RingtoetsFormsResources.AddFolderIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationIndexRootGroup,
                                                          RingtoetsFormsResources.CalculationGroup_Add_Calculation,
                                                          RingtoetsFormsResources.CalculationGroup_Add_Calculation_Tooltip,
                                                          RingtoetsFormsResources.FailureMechanismIcon);

            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndexRootGroup,
                                                          RingtoetsFormsResources.Calculate_all,
                                                          RingtoetsFormsResources.CalculationGroup_CalculateAll_No_calculations_to_run,
                                                          RingtoetsFormsResources.CalculateIcon,
                                                          false);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearAllIndexRootGroup,
                                                          RingtoetsFormsResources.Clear_all_output,
                                                          RingtoetsFormsResources.CalculationGroup_ClearOutput_No_calculation_with_output_to_clear,
                                                          RingtoetsFormsResources.ClearIcon,
                                                          false);

            mocks.VerifyAll();
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
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();

            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddRenameItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddDeleteItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddImportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);
            guiMock.Expect(cmp => cmp.Get(groupContext, treeViewControlMock)).Return(menuBuilderMock);

            mocks.ReplayAll();

            // Call
            info.ContextMenuStrip(groupContext, parentGroupContext, treeViewControlMock);

            // Assert
            mocks.VerifyAll();
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
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            guiMock.Expect(g => g.Get(groupContext, treeViewControlMock)).Return(menuBuilder);

            mocks.ReplayAll();

            // Call
            ContextMenuStrip menu = info.ContextMenuStrip(groupContext, parentGroupContext, treeViewControlMock);

            // Assert
            Assert.AreEqual(9, menu.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationGroupIndexNestedGroup,
                                                          RingtoetsFormsResources.CalculationGroup_Add_CalculationGroup,
                                                          RingtoetsFormsResources.CalculationGroup_Add_CalculationGroup_Tooltip,
                                                          RingtoetsFormsResources.AddFolderIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationIndexNestedGroup,
                                                          RingtoetsFormsResources.CalculationGroup_Add_Calculation,
                                                          RingtoetsFormsResources.CalculationGroup_Add_Calculation_Tooltip,
                                                          RingtoetsFormsResources.FailureMechanismIcon);

            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndexNestedGroup,
                                                          RingtoetsFormsResources.Calculate_all,
                                                          RingtoetsFormsResources.CalculationGroup_CalculateAll_No_calculations_to_run,
                                                          RingtoetsFormsResources.CalculateIcon,
                                                          false);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearAllIndexNestedGroup,
                                                          RingtoetsFormsResources.Clear_all_output,
                                                          RingtoetsFormsResources.CalculationGroup_ClearOutput_No_calculation_with_output_to_clear,
                                                          RingtoetsFormsResources.ClearIcon,
                                                          false);

            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_NoFailureMechanismSections_ReturnContextMenuDisabledAndTooltipSet()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var group = new CalculationGroup()
            {
                Children =
                {
                    new GrassCoverErosionInwardsCalculation(new GeneralGrassCoverErosionInwardsInput(), new GeneralNormProbabilityInput())
                }
            };

            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               failureMechanism,
                                                                               assessmentSectionMock);

            var applicationFeatureCommandHandlerStub = mocks.Stub<IApplicationFeatureCommands>();
            var exportImportHandlerStub = mocks.Stub<IExportImportCommandHandler>();
            var viewCommandsHandlerMock = mocks.StrictMock<IViewCommands>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();

            var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandlerStub, exportImportHandlerStub, viewCommandsHandlerMock, nodeData, treeViewControlMock);
            guiMock.Expect(g => g.Get(nodeData, treeViewControlMock)).Return(menuBuilder);

            treeViewControlMock.Expect(tvc => tvc.CanExpandOrCollapseForData(nodeData)).Repeat.Twice().Return(false);
            viewCommandsHandlerMock.Expect(vc => vc.CanOpenViewFor(nodeData)).Return(false);

            mocks.ReplayAll();

            // Call
            ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControlMock);

            // Assert
            TestHelper.AssertContextMenuStripContainsItem(menu, 5,
                                                          RingtoetsFormsResources.Calculate_all,
                                                          Resources.GrassCoverErosionInwardsGuiPlugin_AllDataAvailable_No_failure_mechanism_sections_imported,
                                                          RingtoetsFormsResources.CalculateAllIcon,
                                                          false);
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismSectionsSetNoHydraulicBoundaryDatabase_ContextMenuItemDisabledAndTooltipSet()
        {
            // Setup
            var group = new CalculationGroup()
            {
                Children =
                {
                    new GrassCoverErosionInwardsCalculation(new GeneralGrassCoverErosionInwardsInput(), new GeneralNormProbabilityInput())
                }
            };

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test", new[]
            {
                new Point2D(0, 0)
            }));
            failureMechanism.CalculationsGroup.Children.Add(
                new GrassCoverErosionInwardsCalculation(new GeneralGrassCoverErosionInwardsInput(), new GeneralNormProbabilityInput()));

            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            assessmentSectionMock.Stub(asm => asm.HydraulicBoundaryDatabase).Return(null);

            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               failureMechanism,
                                                                               assessmentSectionMock);

            var applicationFeatureCommandHandlerStub = mocks.Stub<IApplicationFeatureCommands>();
            var exportImportHandlerStub = mocks.Stub<IExportImportCommandHandler>();
            var viewCommandsHandlerMock = mocks.StrictMock<IViewCommands>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();

            var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandlerStub, exportImportHandlerStub, viewCommandsHandlerMock, nodeData, treeViewControlMock);
            guiMock.Expect(g => g.Get(nodeData, treeViewControlMock)).Return(menuBuilder);

            treeViewControlMock.Expect(tvc => tvc.CanExpandOrCollapseForData(nodeData)).Repeat.Twice().Return(false);
            viewCommandsHandlerMock.Expect(vc => vc.CanOpenViewFor(nodeData)).Return(false);

            mocks.ReplayAll();

            // Call
            var contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControlMock);

            // Assert
            mocks.VerifyAll(); // Expect no calls on arguments

            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 5,
                                                          RingtoetsFormsResources.Calculate_all,
                                                          Resources.GrassCoverErosionInwardsGuiPlugin_AllDataAvailable_No_hydraulic_boundary_database_imported,
                                                          RingtoetsFormsResources.CalculateAllIcon,
                                                          false);
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismSetHydraulicBoundaryDatabaseNotValid_ContextMenuItemDisabledAndTooltipSet()
        {
            // Setup
            var group = new CalculationGroup()
            {
                Children =
                {
                    new GrassCoverErosionInwardsCalculation(new GeneralGrassCoverErosionInwardsInput(), new GeneralNormProbabilityInput())
                }
            };

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test", new[]
            {
                new Point2D(0, 0)
            }));
            failureMechanism.CalculationsGroup.Children.Add(
                new GrassCoverErosionInwardsCalculation(new GeneralGrassCoverErosionInwardsInput(), new GeneralNormProbabilityInput()));

            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            assessmentSectionMock.Stub(asm => asm.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase());

            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               failureMechanism,
                                                                               assessmentSectionMock);

            var applicationFeatureCommandHandlerStub = mocks.Stub<IApplicationFeatureCommands>();
            var exportImportHandlerStub = mocks.Stub<IExportImportCommandHandler>();
            var viewCommandsHandlerMock = mocks.StrictMock<IViewCommands>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();

            var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandlerStub, exportImportHandlerStub, viewCommandsHandlerMock, nodeData, treeViewControlMock);
            guiMock.Expect(g => g.Get(nodeData, treeViewControlMock)).Return(menuBuilder);

            treeViewControlMock.Expect(tvc => tvc.CanExpandOrCollapseForData(nodeData)).Repeat.Twice().Return(false);
            viewCommandsHandlerMock.Expect(vc => vc.CanOpenViewFor(nodeData)).Return(false);

            mocks.ReplayAll();

            // Call
            ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControlMock);

            // Assert
            mocks.VerifyAll(); // Expect no calls on arguments

            ToolStripItem contextMenuItem = contextMenu.Items[5];

            Assert.AreEqual(RingtoetsFormsResources.Calculate_all, contextMenuItem.Text);
            StringAssert.Contains(string.Format(RingtoetsFormsResources.GuiPlugin_VerifyHydraulicBoundaryDatabasePath_Hydraulic_boundary_database_connection_failed_0_, ""), contextMenuItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.CalculateAllIcon, contextMenuItem.Image);
            Assert.IsFalse(contextMenuItem.Enabled);
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismSectionsAndHydraulicDatabaseSet_ReturnContextMenuWithCalculateAllEnabled()
        {
            // Setup
            var group = new CalculationGroup()
            {
                Children =
                {
                    new GrassCoverErosionInwardsCalculation(new GeneralGrassCoverErosionInwardsInput(), new GeneralNormProbabilityInput())
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
            failureMechanism.CalculationsGroup.Children.Add(
                new GrassCoverErosionInwardsCalculation(new GeneralGrassCoverErosionInwardsInput(), new GeneralNormProbabilityInput()));

            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            assessmentSectionMock.Stub(asm => asm.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase);

            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               failureMechanism,
                                                                               assessmentSectionMock);

            var applicationFeatureCommandHandlerStub = mocks.Stub<IApplicationFeatureCommands>();
            var exportImportHandlerStub = mocks.Stub<IExportImportCommandHandler>();
            var viewCommandsHandlerMock = mocks.StrictMock<IViewCommands>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();

            var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandlerStub, exportImportHandlerStub, viewCommandsHandlerMock, nodeData, treeViewControlMock);
            guiMock.Expect(g => g.Get(nodeData, treeViewControlMock)).Return(menuBuilder);

            treeViewControlMock.Expect(tvc => tvc.CanExpandOrCollapseForData(nodeData)).Repeat.Twice().Return(false);
            viewCommandsHandlerMock.Expect(vc => vc.CanOpenViewFor(nodeData)).Return(false);

            mocks.ReplayAll();

            // Call
            ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControlMock);

            // Assert
            TestHelper.AssertContextMenuStripContainsItem(menu, 5,
                                                          RingtoetsFormsResources.Calculate_all,
                                                          RingtoetsFormsResources.CalculationGroup_CalculateAll_ToolTip,
                                                          RingtoetsFormsResources.CalculateIcon);
            mocks.VerifyAll();
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
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();

            observerMock.Expect(o => o.UpdateObserver());
            guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControlMock)).Return(menuBuilder);

            mocks.ReplayAll();

            group.Children.Add(calculationGroup);
            nodeData.Attach(observerMock);

            ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControlMock);

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

            mocks.VerifyAll();
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

            var calculationItem = new GrassCoverErosionInwardsCalculation(new GeneralGrassCoverErosionInwardsInput(), new GeneralNormProbabilityInput())
            {
                Name = "Nieuwe berekening"
            };

            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());

            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();

            guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControlMock)).Return(menuBuilder);

            mocks.ReplayAll();

            group.Children.Add(calculationItem);

            nodeData.Attach(observerMock);

            var contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControlMock);

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

            mocks.VerifyAll();
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
            mocks.VerifyAll();
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
            var calculation = new GrassCoverErosionInwardsCalculation(
                new GeneralGrassCoverErosionInwardsInput(),
                new GeneralNormProbabilityInput());

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
            mocks.VerifyAll();
        }

        private const int contextMenuAddCalculationGroupIndexRootGroup = 1;
        private const int contextMenuAddCalculationIndexRootGroup = 2;
        private const int contextMenuCalculateAllIndexRootGroup = 4;
        private const int contextMenuClearAllIndexRootGroup = 5;
        private const int contextMenuAddCalculationGroupIndexNestedGroup = 0;
        private const int contextMenuAddCalculationIndexNestedGroup = 1;
        private const int contextMenuCalculateAllIndexNestedGroup = 3;
        private const int contextMenuClearAllIndexNestedGroup = 4;
    }
}