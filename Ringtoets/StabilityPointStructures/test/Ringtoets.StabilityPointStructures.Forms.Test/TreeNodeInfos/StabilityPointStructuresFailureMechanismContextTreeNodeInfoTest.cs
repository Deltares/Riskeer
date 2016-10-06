﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Geometry;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.HydraRing.Data;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Forms.PresentationObjects;
using Ringtoets.StabilityPointStructures.Plugin;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsCommonServicesResources = Ringtoets.Common.Service.Properties.Resources;

namespace Ringtoets.StabilityPointStructures.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class StabilityPointStructuresFailureMechanismContextTreeNodeInfoTest
    {
        private const int contextMenuRelevancyIndexWhenRelevant = 0;
        private const int contextMenuRelevancyIndexWhenNotRelevant = 0;
        private const int contextMenuValidateAllIndex = 2;
        private const int contextMenuCalculateAllIndex = 3;
        private const int contextMenuClearAllIndex = 4;
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.IO, "HydraulicBoundaryLocationReader");

        private MockRepository mocksRepository;

        [SetUp]
        public void SetUp()
        {
            mocksRepository = new MockRepository();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new StabilityPointStructuresPlugin())
            {
                var info = GetInfo(plugin);

                // Assert
                Assert.AreEqual(typeof(StabilityPointStructuresFailureMechanismContext), info.TagType);
                Assert.IsNotNull(info.Text);
                Assert.IsNotNull(info.ForeColor);
                Assert.IsNotNull(info.Image);
                Assert.IsNotNull(info.ChildNodeObjects);
                Assert.IsNotNull(info.ContextMenuStrip);
                Assert.IsNull(info.EnsureVisibleOnCreate);
                Assert.IsNull(info.CanRename);
                Assert.IsNull(info.OnNodeRenamed);
                Assert.IsNull(info.CanRemove);
                Assert.IsNull(info.OnNodeRemoved);
                Assert.IsNull(info.CanCheck);
                Assert.IsNull(info.IsChecked);
                Assert.IsNull(info.OnNodeChecked);
                Assert.IsNull(info.CanDrag);
                Assert.IsNull(info.CanDrop);
                Assert.IsNull(info.CanInsert);
                Assert.IsNull(info.OnDrop);
            }
        }

        [Test]
        public void ChildNodeObjects_FailureMechanismIsRelevant_ReturnChildDataNodes()
        {
            // Setup
            var assessmentSectionMock = mocksRepository.Stub<IAssessmentSection>();
            mocksRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var failureMechanismContext = new StabilityPointStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);

            using (var plugin = new StabilityPointStructuresPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var children = info.ChildNodeObjects(failureMechanismContext).ToArray();

                // Assert
                Assert.AreEqual(3, children.Length);

                var inputsFolder = (CategoryTreeFolder) children[0];
                Assert.AreEqual("Invoer", inputsFolder.Name);
                Assert.AreEqual(TreeFolderCategory.Input, inputsFolder.Category);
                Assert.AreEqual(3, inputsFolder.Contents.Count);
                var failureMechanismSectionsContext = (FailureMechanismSectionsContext) inputsFolder.Contents[0];
                Assert.AreSame(failureMechanism, failureMechanismSectionsContext.WrappedData);
                Assert.AreSame(assessmentSectionMock, failureMechanismSectionsContext.ParentAssessmentSection);

                var stabilityPointStructuresContext = (StabilityPointStructuresContext) inputsFolder.Contents[1];
                Assert.AreSame(failureMechanism.StabilityPointStructures, stabilityPointStructuresContext.WrappedData);
                Assert.AreSame(assessmentSectionMock, stabilityPointStructuresContext.AssessmentSection);

                var commentContext = (CommentContext<ICommentable>) inputsFolder.Contents[2];
                Assert.AreSame(failureMechanism, commentContext.WrappedData);

                var calculationsFolder = (StabilityPointStructuresCalculationGroupContext) children[1];
                Assert.AreEqual("Berekeningen", calculationsFolder.WrappedData.Name);
                Assert.AreSame(failureMechanism.CalculationsGroup, calculationsFolder.WrappedData);
                Assert.AreSame(failureMechanism, calculationsFolder.FailureMechanism);

                var outputsFolder = (CategoryTreeFolder) children[2];
                Assert.AreEqual("Oordeel", outputsFolder.Name);
                Assert.AreEqual(TreeFolderCategory.Output, outputsFolder.Category);
                Assert.AreEqual(1, outputsFolder.Contents.Count);

                var failureMechanismResultsContext = (FailureMechanismSectionResultContext<StabilityPointStructuresFailureMechanismSectionResult>) outputsFolder.Contents[0];
                Assert.AreSame(failureMechanism, failureMechanismResultsContext.FailureMechanism);
                Assert.AreSame(failureMechanism.SectionResults, failureMechanismResultsContext.WrappedData);
            }
            mocksRepository.VerifyAll();
        }

        [Test]
        public void ChildNodeObjects_FailureMechanismIsNotRelevant_ReturnOnlyFailureMechanismComments()
        {
            // Setup
            var assessmentSectionMock = mocksRepository.Stub<IAssessmentSection>();
            mocksRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism
            {
                IsRelevant = false
            };
            var failureMechanismContext = new StabilityPointStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);

            using (var plugin = new StabilityPointStructuresPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var children = info.ChildNodeObjects(failureMechanismContext).ToArray();

                // Assert
                Assert.AreEqual(1, children.Length);
                var commentContext = (CommentContext<ICommentable>) children[0];
                Assert.AreSame(failureMechanism, commentContext.WrappedData);
            }
            mocksRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsRelevant_CallsContextMenuBuilderMethods()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var assessmentSectionMock = mocksRepository.Stub<IAssessmentSection>();
            var failureMechanismContext = new StabilityPointStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);
            var guiMock = mocksRepository.StrictMock<IGui>();
            var menuBuilderMock = mocksRepository.StrictMock<IContextMenuBuilder>();

            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
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

            using (var plugin = new StabilityPointStructuresPlugin())
            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(cmp => cmp.Get(failureMechanismContext, treeViewControl)).Return(menuBuilderMock);
                mocksRepository.ReplayAll();

                plugin.Gui = guiMock;
                var info = GetInfo(plugin);

                // Call
                info.ContextMenuStrip(failureMechanismContext, null, treeViewControl);
            }

            // Assert
            mocksRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsNotRelevant_CallsContextMenuBuilderMethods()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism
            {
                IsRelevant = false
            };
            var assessmentSectionMock = mocksRepository.Stub<IAssessmentSection>();
            var failureMechanismContext = new StabilityPointStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);
            var guiMock = mocksRepository.StrictMock<IGui>();
            var menuBuilderMock = mocksRepository.StrictMock<IContextMenuBuilder>();

            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            using (var plugin = new StabilityPointStructuresPlugin())
            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(cmp => cmp.Get(failureMechanismContext, treeViewControl)).Return(menuBuilderMock);
                mocksRepository.ReplayAll();

                plugin.Gui = guiMock;
                var info = GetInfo(plugin);

                // Call
                info.ContextMenuStrip(failureMechanismContext, null, treeViewControl);
            }

            // Assert
            mocksRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsRelevant_AddCustomItems()
        {
            // Setup
            using (var plugin = new StabilityPointStructuresPlugin())
            using (var treeView = new TreeViewControl())
            {
                var assessmentSectionMock = mocksRepository.Stub<IAssessmentSection>();
                var failureMechanism = new StabilityPointStructuresFailureMechanism();
                var failureMechanismContext = new StabilityPointStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var guiMock = mocksRepository.StrictMock<IGui>();

                guiMock.Expect(cmp => cmp.Get(failureMechanismContext, treeView)).Return(menuBuilder);
                guiMock.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                guiMock.Stub(g => g.ProjectOpened -= null).IgnoreArguments();

                mocksRepository.ReplayAll();
                plugin.Gui = guiMock;

                var info = GetInfo(plugin);

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(failureMechanismContext, assessmentSectionMock, treeView))
                {
                    // Assert
                    Assert.AreEqual(7, menu.Items.Count);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuRelevancyIndexWhenRelevant,
                                                                  RingtoetsCommonFormsResources.FailureMechanismContextMenuStrip_Is_relevant,
                                                                  RingtoetsCommonFormsResources.FailureMechanismContextMenuStrip_Is_relevant_Tooltip,
                                                                  RingtoetsCommonFormsResources.Checkbox_ticked);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndex,
                                                                  RingtoetsCommonFormsResources.Validate_all,
                                                                  RingtoetsCommonFormsResources.ValidateAll_No_calculations_to_validate,
                                                                  RingtoetsCommonFormsResources.ValidateAllIcon,
                                                                  false);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndex,
                                                                  RingtoetsCommonFormsResources.Calculate_all,
                                                                  RingtoetsCommonFormsResources.FailureMechanism_CreateCalculateAllItem_No_calculations_to_run,
                                                                  RingtoetsCommonFormsResources.CalculateAllIcon,
                                                                  false);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearAllIndex,
                                                                  RingtoetsCommonFormsResources.Clear_all_output,
                                                                  RingtoetsCommonFormsResources.CalculationGroup_ClearOutput_No_calculation_with_output_to_clear,
                                                                  RingtoetsCommonFormsResources.ClearIcon,
                                                                  false);
                }
            }

            mocksRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsNotRelevant_AddCustomItems()
        {
            // Setup
            using (var plugin = new StabilityPointStructuresPlugin())
            using (var treeView = new TreeViewControl())
            {
                var assessmentSectionMock = mocksRepository.Stub<IAssessmentSection>();
                var failureMechanism = new StabilityPointStructuresFailureMechanism
                {
                    IsRelevant = false
                };
                var failureMechanismContext = new StabilityPointStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var guiMock = mocksRepository.StrictMock<IGui>();

                guiMock.Expect(cmp => cmp.Get(failureMechanismContext, treeView)).Return(menuBuilder);
                guiMock.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                guiMock.Stub(g => g.ProjectOpened -= null).IgnoreArguments();

                mocksRepository.ReplayAll();
                plugin.Gui = guiMock;

                var info = GetInfo(plugin);

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(failureMechanismContext, assessmentSectionMock, treeView))
                {
                    // Assert
                    Assert.AreEqual(2, menu.Items.Count);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuRelevancyIndexWhenNotRelevant,
                                                                  RingtoetsCommonFormsResources.FailureMechanismContextMenuStrip_Is_relevant,
                                                                  RingtoetsCommonFormsResources.FailureMechanismContextMenuStrip_Is_relevant_Tooltip,
                                                                  RingtoetsCommonFormsResources.Checkbox_empty);
                }
            }
            mocksRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsRelevantAndClickOnIsRelevantItem_OnChangeActionRemovesAllViewsForItem()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var assessmentSectionMock = mocksRepository.Stub<IAssessmentSection>();
            var failureMechanismContext = new StabilityPointStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);
            var viewCommandsMock = mocksRepository.StrictMock<IViewCommands>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            var guiMock = mocksRepository.StrictMock<IGui>();

            viewCommandsMock.Expect(vs => vs.RemoveAllViewsForItem(failureMechanismContext));
            guiMock.Stub(g => g.ViewCommands).Return(viewCommandsMock);

            using (var plugin = new StabilityPointStructuresPlugin())
            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(g => g.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);

                mocksRepository.ReplayAll();
                plugin.Gui = guiMock;

                var info = GetInfo(plugin);

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl))
                {
                    // Call
                    contextMenu.Items[contextMenuRelevancyIndexWhenRelevant].PerformClick();
                }
            }

            // Assert
            mocksRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsNotRelevantAndClickOnIsRelevantItem_MakeFailureMechanismRelevant()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism
            {
                IsRelevant = false
            };
            var assessmentSectionMock = mocksRepository.Stub<IAssessmentSection>();
            var failureMechanismContext = new StabilityPointStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            var guiMock = mocksRepository.StrictMock<IGui>();

            using (var plugin = new StabilityPointStructuresPlugin())
            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(g => g.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);

                mocksRepository.ReplayAll();
                plugin.Gui = guiMock;

                var info = GetInfo(plugin);

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl))
                {
                    // Call
                    contextMenu.Items[contextMenuRelevancyIndexWhenNotRelevant].PerformClick();
                }
            }

            // Assert
            mocksRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_NoFailureMechanismSections_ContextMenuItemCalculateAllDisabledAndTooltipSet()
        {
            // Setup
            var guiMock = mocksRepository.StrictMock<IGui>();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new StabilityPointStructuresCalculation());
            var assessmentSectionMock = mocksRepository.Stub<IAssessmentSection>();

            var nodeData = new StabilityPointStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var plugin = new StabilityPointStructuresPlugin())
            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                mocksRepository.ReplayAll();

                var info = GetInfo(plugin);
                plugin.Gui = guiMock;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndex,
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
            var guiMock = mocksRepository.StrictMock<IGui>();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new StabilityPointStructuresCalculation());
            failureMechanism.AddSection(new FailureMechanismSection("test", new[]
            {
                new Point2D(0, 0)
            }));

            var assessmentSectionMock = mocksRepository.Stub<IAssessmentSection>();

            var nodeData = new StabilityPointStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var plugin = new StabilityPointStructuresPlugin())
            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                mocksRepository.ReplayAll();

                var info = GetInfo(plugin);
                plugin.Gui = guiMock;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuCalculateAllIndex,
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
            var guiMock = mocksRepository.StrictMock<IGui>();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new StabilityPointStructuresCalculation());
            failureMechanism.AddSection(new FailureMechanismSection("test", new[]
            {
                new Point2D(0, 0)
            }));

            var assessmentSectionMock = mocksRepository.Stub<IAssessmentSection>();
            assessmentSectionMock.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            var nodeData = new StabilityPointStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var plugin = new StabilityPointStructuresPlugin())
            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                mocksRepository.ReplayAll();

                var info = GetInfo(plugin);
                plugin.Gui = guiMock;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    ToolStripItem contextMenuItem = contextMenu.Items[contextMenuCalculateAllIndex];

                    Assert.AreEqual(RingtoetsCommonFormsResources.Calculate_all, contextMenuItem.Text);
                    StringAssert.Contains(string.Format(RingtoetsCommonServicesResources.Hydraulic_boundary_database_connection_failed_0_, ""), contextMenuItem.ToolTipText);
                    TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.CalculateAllIcon, contextMenuItem.Image);
                    Assert.IsFalse(contextMenuItem.Enabled);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismSectionsAndHydraulicDatabaseSet_ContextMenuItemCalculateAllEnabled()
        {
            // Setup
            var guiMock = mocksRepository.StrictMock<IGui>();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new StabilityPointStructuresCalculation());
            failureMechanism.AddSection(new FailureMechanismSection("test", new[]
            {
                new Point2D(0, 0)
            }));

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "1.0"
            };

            var assessmentSectionMock = mocksRepository.Stub<IAssessmentSection>();
            assessmentSectionMock.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;

            var nodeData = new StabilityPointStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var plugin = new StabilityPointStructuresPlugin())
            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                mocksRepository.ReplayAll();

                var info = GetInfo(plugin);
                plugin.Gui = guiMock;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndex,
                                                                  RingtoetsCommonFormsResources.Calculate_all,
                                                                  RingtoetsCommonFormsResources.Calculate_all_ToolTip,
                                                                  RingtoetsCommonFormsResources.CalculateAllIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_NoFailureMechanismSections_ContextMenuItemValidateAllDisabledAndTooltipSet()
        {
            // Setup
            var guiMock = mocksRepository.StrictMock<IGui>();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new StabilityPointStructuresCalculation());
            var assessmentSectionMock = mocksRepository.Stub<IAssessmentSection>();

            var nodeData = new StabilityPointStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var plugin = new StabilityPointStructuresPlugin())
            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                mocksRepository.ReplayAll();

                var info = GetInfo(plugin);
                plugin.Gui = guiMock;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndex,
                                                                  RingtoetsCommonFormsResources.Validate_all,
                                                                  RingtoetsCommonFormsResources.Plugin_AllDataAvailable_No_failure_mechanism_sections_imported,
                                                                  RingtoetsCommonFormsResources.ValidateAllIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismSectionsSetNoHydraulicBoundaryDatabase_ContextMenuItemValidateAllDisabledAndTooltipSet()
        {
            // Setup
            var guiMock = mocksRepository.StrictMock<IGui>();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new StabilityPointStructuresCalculation());
            failureMechanism.AddSection(new FailureMechanismSection("test", new[]
            {
                new Point2D(0, 0)
            }));

            var assessmentSectionMock = mocksRepository.Stub<IAssessmentSection>();

            var nodeData = new StabilityPointStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var plugin = new StabilityPointStructuresPlugin())
            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                mocksRepository.ReplayAll();

                var info = GetInfo(plugin);
                plugin.Gui = guiMock;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuValidateAllIndex,
                                                                  RingtoetsCommonFormsResources.Validate_all,
                                                                  RingtoetsCommonFormsResources.Plugin_AllDataAvailable_No_hydraulic_boundary_database_imported,
                                                                  RingtoetsCommonFormsResources.ValidateAllIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismSectionsSetHydraulicBoundaryDatabaseNotValid_ContextMenuItemValidateAllDisabledAndTooltipSet()
        {
            // Setup
            var guiMock = mocksRepository.StrictMock<IGui>();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new StabilityPointStructuresCalculation());
            failureMechanism.AddSection(new FailureMechanismSection("test", new[]
            {
                new Point2D(0, 0)
            }));

            var assessmentSectionMock = mocksRepository.Stub<IAssessmentSection>();
            assessmentSectionMock.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            var nodeData = new StabilityPointStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var plugin = new StabilityPointStructuresPlugin())
            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                mocksRepository.ReplayAll();

                var info = GetInfo(plugin);
                plugin.Gui = guiMock;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    ToolStripItem contextMenuItem = contextMenu.Items[contextMenuValidateAllIndex];

                    Assert.AreEqual(RingtoetsCommonFormsResources.Validate_all, contextMenuItem.Text);
                    StringAssert.Contains(string.Format(RingtoetsCommonServicesResources.Hydraulic_boundary_database_connection_failed_0_, ""), contextMenuItem.ToolTipText);
                    TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.ValidateAllIcon, contextMenuItem.Image);
                    Assert.IsFalse(contextMenuItem.Enabled);
                }
            }
        }

        private static TreeNodeInfo GetInfo(StabilityPointStructuresPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(StabilityPointStructuresFailureMechanismContext));
        }
    }
}