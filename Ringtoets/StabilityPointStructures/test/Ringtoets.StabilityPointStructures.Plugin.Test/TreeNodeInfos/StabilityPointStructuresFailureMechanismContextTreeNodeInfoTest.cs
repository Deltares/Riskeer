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
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Data.TestUtil;
using Ringtoets.StabilityPointStructures.Forms.PresentationObjects;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.StabilityPointStructures.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class StabilityPointStructuresFailureMechanismContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuRelevancyIndexWhenRelevant = 2;
        private const int contextMenuRelevancyIndexWhenNotRelevant = 0;

        private const int contextMenuValidateAllIndex = 4;
        private const int contextMenuCalculateAllIndex = 5;
        private const int contextMenuClearAllIndex = 7;

        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "HydraulicBoundaryDatabaseImporter");

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
                TreeNodeInfo info = GetInfo(plugin);

                // Assert
                Assert.IsNotNull(info.Text);
                Assert.IsNotNull(info.ForeColor);
                Assert.IsNotNull(info.Image);
                Assert.IsNotNull(info.ContextMenuStrip);
                Assert.IsNull(info.EnsureVisibleOnCreate);
                Assert.IsNull(info.ExpandOnCreate);
                Assert.IsNotNull(info.ChildNodeObjects);
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
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            mocksRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var failureMechanismContext = new StabilityPointStructuresFailureMechanismContext(failureMechanism, assessmentSection);

            using (var plugin = new StabilityPointStructuresPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                object[] children = info.ChildNodeObjects(failureMechanismContext).ToArray();

                // Assert
                Assert.AreEqual(3, children.Length);

                var inputsFolder = (CategoryTreeFolder) children[0];
                Assert.AreEqual("Invoer", inputsFolder.Name);
                Assert.AreEqual(TreeFolderCategory.Input, inputsFolder.Category);

                Assert.AreEqual(4, inputsFolder.Contents.Count);
                var failureMechanismSectionsContext = (FailureMechanismSectionsContext) inputsFolder.Contents[0];
                Assert.AreSame(failureMechanism, failureMechanismSectionsContext.WrappedData);
                Assert.AreSame(assessmentSection, failureMechanismSectionsContext.ParentAssessmentSection);

                var profilesContext = (ForeshoreProfilesContext) inputsFolder.Contents[1];
                Assert.AreSame(failureMechanism.ForeshoreProfiles, profilesContext.WrappedData);
                Assert.AreSame(failureMechanism, profilesContext.ParentFailureMechanism);
                Assert.AreSame(assessmentSection, profilesContext.ParentAssessmentSection);

                var stabilityPointStructuresContext = (StabilityPointStructuresContext) inputsFolder.Contents[2];
                Assert.AreSame(failureMechanism.StabilityPointStructures, stabilityPointStructuresContext.WrappedData);
                Assert.AreSame(failureMechanism, stabilityPointStructuresContext.FailureMechanism);
                Assert.AreSame(assessmentSection, stabilityPointStructuresContext.AssessmentSection);

                var inputComment = (Comment) inputsFolder.Contents[3];
                Assert.AreSame(failureMechanism.InputComments, inputComment);

                var calculationsFolder = (StabilityPointStructuresCalculationGroupContext) children[1];
                Assert.AreEqual("Berekeningen", calculationsFolder.WrappedData.Name);
                Assert.AreSame(failureMechanism.CalculationsGroup, calculationsFolder.WrappedData);
                Assert.AreSame(failureMechanism, calculationsFolder.FailureMechanism);

                var outputsFolder = (CategoryTreeFolder) children[2];
                Assert.AreEqual("Oordeel", outputsFolder.Name);
                Assert.AreEqual(TreeFolderCategory.Output, outputsFolder.Category);
                Assert.AreEqual(3, outputsFolder.Contents.Count);

                var scenariosContext = (StabilityPointStructuresScenariosContext) outputsFolder.Contents[0];
                Assert.AreSame(failureMechanism, scenariosContext.ParentFailureMechanism);
                Assert.AreSame(failureMechanism.CalculationsGroup, scenariosContext.WrappedData);

                var failureMechanismResultsContext = (FailureMechanismSectionResultContext<StabilityPointStructuresFailureMechanismSectionResult>) outputsFolder.Contents[1];
                Assert.AreSame(failureMechanism, failureMechanismResultsContext.FailureMechanism);
                Assert.AreSame(failureMechanism.SectionResults, failureMechanismResultsContext.WrappedData);

                var outputComment = (Comment) outputsFolder.Contents[2];
                Assert.AreSame(failureMechanism.OutputComments, outputComment);
            }
            mocksRepository.VerifyAll();
        }

        [Test]
        public void ChildNodeObjects_FailureMechanismIsNotRelevant_ReturnOnlyFailureMechanismNotRelevantComments()
        {
            // Setup
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            mocksRepository.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism
            {
                IsRelevant = false
            };
            var failureMechanismContext = new StabilityPointStructuresFailureMechanismContext(failureMechanism, assessmentSection);

            using (var plugin = new StabilityPointStructuresPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                object[] children = info.ChildNodeObjects(failureMechanismContext).ToArray();

                // Assert
                Assert.AreEqual(1, children.Length);
                var comment = (Comment) children[0];
                Assert.AreSame(failureMechanism.NotRelevantComments, comment);
            }
            mocksRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsRelevant_CallsContextMenuBuilderMethods()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            var failureMechanismContext = new StabilityPointStructuresFailureMechanismContext(failureMechanism, assessmentSection);

            var menuBuilderMock = mocksRepository.StrictMock<IContextMenuBuilder>();
            using (mocksRepository.Ordered())
            {
                menuBuilderMock.Expect(mb => mb.AddOpenItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.Build()).Return(null);
            }

            using (var plugin = new StabilityPointStructuresPlugin())
            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocksRepository.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(failureMechanismContext, treeViewControl)).Return(menuBuilderMock);
                mocksRepository.ReplayAll();

                plugin.Gui = gui;
                TreeNodeInfo info = GetInfo(plugin);

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
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            var failureMechanismContext = new StabilityPointStructuresFailureMechanismContext(failureMechanism, assessmentSection);

            var menuBuilderMock = mocksRepository.StrictMock<IContextMenuBuilder>();
            using (mocksRepository.Ordered())
            {
                menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.Build()).Return(null);
            }

            using (var plugin = new StabilityPointStructuresPlugin())
            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocksRepository.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(failureMechanismContext, treeViewControl)).Return(menuBuilderMock);
                mocksRepository.ReplayAll();

                plugin.Gui = gui;

                TreeNodeInfo info = GetInfo(plugin);

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
                var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
                var failureMechanism = new StabilityPointStructuresFailureMechanism();
                var failureMechanismContext = new StabilityPointStructuresFailureMechanismContext(failureMechanism, assessmentSection);
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocksRepository.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(failureMechanismContext, treeView)).Return(menuBuilder);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                mocksRepository.ReplayAll();

                plugin.Gui = gui;

                TreeNodeInfo info = GetInfo(plugin);

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(failureMechanismContext, assessmentSection, treeView))
                {
                    // Assert
                    Assert.AreEqual(13, menu.Items.Count);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuRelevancyIndexWhenRelevant,
                                                                  "I&s relevant",
                                                                  "Geeft aan of dit toetsspoor relevant is of niet.",
                                                                  RingtoetsCommonFormsResources.Checkbox_ticked);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndex,
                                                                  "Alles &valideren",
                                                                  "Er zijn geen berekeningen om te valideren.",
                                                                  RingtoetsCommonFormsResources.ValidateAllIcon,
                                                                  false);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndex,
                                                                  "Alles be&rekenen",
                                                                  "Er zijn geen berekeningen om uit te voeren.",
                                                                  RingtoetsCommonFormsResources.CalculateAllIcon,
                                                                  false);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearAllIndex,
                                                                  "&Wis alle uitvoer...",
                                                                  "Er zijn geen berekeningen met uitvoer om te wissen.",
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
                var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
                var failureMechanism = new StabilityPointStructuresFailureMechanism
                {
                    IsRelevant = false
                };
                var failureMechanismContext = new StabilityPointStructuresFailureMechanismContext(failureMechanism, assessmentSection);
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocksRepository.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(failureMechanismContext, treeView)).Return(menuBuilder);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                mocksRepository.ReplayAll();

                plugin.Gui = gui;

                TreeNodeInfo info = GetInfo(plugin);

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(failureMechanismContext, assessmentSection, treeView))
                {
                    // Assert
                    Assert.AreEqual(4, menu.Items.Count);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuRelevancyIndexWhenNotRelevant,
                                                                  "I&s relevant",
                                                                  "Geeft aan of dit toetsspoor relevant is of niet.",
                                                                  RingtoetsCommonFormsResources.Checkbox_empty);
                }
            }
            mocksRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsRelevantAndClickOnIsRelevantItem_MakeFailureMechanismNotRelevantAndRemovesAllViewsForItem()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            var failureMechanismContext = new StabilityPointStructuresFailureMechanismContext(failureMechanism, assessmentSection);
            var viewCommands = mocksRepository.StrictMock<IViewCommands>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            viewCommands.Expect(vs => vs.RemoveAllViewsForItem(failureMechanismContext));

            using (var plugin = new StabilityPointStructuresPlugin())
            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocksRepository.Stub<IGui>();
                gui.Stub(g => g.ViewCommands).Return(viewCommands);
                gui.Stub(g => g.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);
                mocksRepository.ReplayAll();

                plugin.Gui = gui;

                TreeNodeInfo info = GetInfo(plugin);

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl))
                {
                    // Call
                    contextMenu.Items[contextMenuRelevancyIndexWhenRelevant].PerformClick();

                    // Assert
                    Assert.IsFalse(failureMechanism.IsRelevant);
                }
            }

            mocksRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsNotRelevantAndClickOnIsRelevantItem_MakeFailureMechanismRelevantAndRemovesAllViewsForItem()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism
            {
                IsRelevant = false
            };
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            var failureMechanismContext = new StabilityPointStructuresFailureMechanismContext(failureMechanism, assessmentSection);
            var viewCommands = mocksRepository.StrictMock<IViewCommands>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            viewCommands.Expect(vs => vs.RemoveAllViewsForItem(failureMechanismContext));

            using (var plugin = new StabilityPointStructuresPlugin())
            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocksRepository.Stub<IGui>();
                gui.Stub(g => g.ViewCommands).Return(viewCommands);
                gui.Stub(g => g.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);
                mocksRepository.ReplayAll();

                plugin.Gui = gui;

                TreeNodeInfo info = GetInfo(plugin);

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl))
                {
                    // Call
                    contextMenu.Items[contextMenuRelevancyIndexWhenNotRelevant].PerformClick();

                    // Assert
                    Assert.IsTrue(failureMechanism.IsRelevant);
                }
            }

            mocksRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_NoHydraulicBoundaryDatabase_ContextMenuItemCalculateAllAndValidateAllDisabledAndTooltipSet()
        {
            // Setup
            var failureMechanism = new TestStabilityPointStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new StructuresCalculation<StabilityPointStructuresInput>());

            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();

            var nodeData = new StabilityPointStructuresFailureMechanismContext(failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var plugin = new StabilityPointStructuresPlugin())
            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocksRepository.Stub<IGui>();
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                mocksRepository.ReplayAll();

                TreeNodeInfo info = GetInfo(plugin);
                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuCalculateAllIndex,
                                                                  "Alles be&rekenen",
                                                                  "Er is geen hydraulische randvoorwaardendatabase geïmporteerd.",
                                                                  RingtoetsCommonFormsResources.CalculateAllIcon,
                                                                  false);

                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuValidateAllIndex,
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
            var failureMechanism = new TestStabilityPointStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new StructuresCalculation<StabilityPointStructuresInput>());

            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            var nodeData = new StabilityPointStructuresFailureMechanismContext(failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var plugin = new StabilityPointStructuresPlugin())
            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocksRepository.Stub<IGui>();
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                mocksRepository.ReplayAll();

                TreeNodeInfo info = GetInfo(plugin);
                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    ToolStripItem calculateAllContextMenuItem = contextMenu.Items[contextMenuCalculateAllIndex];

                    Assert.AreEqual("Alles be&rekenen", calculateAllContextMenuItem.Text);
                    StringAssert.Contains("Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt.", calculateAllContextMenuItem.ToolTipText);
                    TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.CalculateAllIcon, calculateAllContextMenuItem.Image);
                    Assert.IsFalse(calculateAllContextMenuItem.Enabled);

                    ToolStripItem validateAllContextMenuItem = contextMenu.Items[contextMenuValidateAllIndex];

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
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new StructuresCalculation<StabilityPointStructuresInput>());

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "1.0"
            };

            var nodeData = new StabilityPointStructuresFailureMechanismContext(failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var plugin = new StabilityPointStructuresPlugin())
            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocksRepository.Stub<IGui>();
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                mocksRepository.ReplayAll();

                TreeNodeInfo info = GetInfo(plugin);
                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndex,
                                                                  "Alles be&rekenen",
                                                                  "De bijdrage van dit toetsspoor is nul.",
                                                                  RingtoetsCommonFormsResources.CalculateAllIcon,
                                                                  false);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndex,
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
            var failureMechanism = new TestStabilityPointStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new StructuresCalculation<StabilityPointStructuresInput>());

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "1.0"
            };

            var nodeData = new StabilityPointStructuresFailureMechanismContext(failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var plugin = new StabilityPointStructuresPlugin())
            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocksRepository.Stub<IGui>();
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                mocksRepository.ReplayAll();

                TreeNodeInfo info = GetInfo(plugin);
                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndex,
                                                                  "Alles be&rekenen",
                                                                  "Voer alle berekeningen binnen dit toetsspoor uit.",
                                                                  RingtoetsCommonFormsResources.CalculateAllIcon);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndex,
                                                                  "Alles &valideren",
                                                                  "Valideer alle berekeningen binnen dit toetsspoor.",
                                                                  RingtoetsCommonFormsResources.ValidateAllIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_ClickOnCalculateAllItem_ScheduleAllChildCalculations()
        {
            // Setup
            var mainWindow = mocksRepository.Stub<IMainWindow>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var failureMechanism = new TestStabilityPointStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new TestStabilityPointStructuresCalculation
            {
                Name = "A",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(),
                    Structure = new TestStabilityPointStructure(),
                    InflowModelType = StabilityPointStructureInflowModelType.LowSill,
                    LoadSchematizationType = LoadSchematizationType.Linear
                }
            });
            failureMechanism.CalculationsGroup.Children.Add(new TestStabilityPointStructuresCalculation
            {
                Name = "B",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(),
                    Structure = new TestStabilityPointStructure(),
                    InflowModelType = StabilityPointStructureInflowModelType.LowSill,
                    LoadSchematizationType = LoadSchematizationType.Linear
                }
            });

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var hydraulicBoundaryDatabase = mocksRepository.Stub<HydraulicBoundaryDatabase>();
            hydraulicBoundaryDatabase.FilePath = validFilePath;

            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.Id).Return(string.Empty);
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 1, 1));
            assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;

            var failureMechanismContext = new StabilityPointStructuresFailureMechanismContext(failureMechanism, assessmentSection);

            using (var plugin = new StabilityPointStructuresPlugin())
            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocksRepository.Stub<IGui>();
                gui.Stub(g => g.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);

                int calculators = failureMechanism.Calculations.Count();
                var calculatorFactory = mocksRepository.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Expect(cf => cf.CreateStructuresStabilityPointCalculator(testDataPath))
                                 .Return(new TestStructuresStabilityPointCalculator())
                                 .Repeat
                                 .Times(calculators);
                mocksRepository.ReplayAll();

                TreeNodeInfo info = GetInfo(plugin);
                plugin.Gui = gui;

                DialogBoxHandler = (name, wnd) =>
                {
                    // Expect an activity dialog which is automatically closed
                };

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl))
                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                {
                    // Call
                    TestHelper.AssertLogMessages(() => contextMenu.Items[contextMenuCalculateAllIndex].PerformClick(), messages =>
                    {
                        List<string> messageList = messages.ToList();

                        // Assert
                        Assert.AreEqual(12, messageList.Count);
                        StringAssert.StartsWith("Validatie van 'A' gestart om: ", messageList[0]);
                        StringAssert.StartsWith("Validatie van 'A' beëindigd om: ", messageList[1]);
                        StringAssert.StartsWith("Berekening van 'A' gestart om: ", messageList[2]);
                        StringAssert.StartsWith("Puntconstructies berekening is uitgevoerd op de tijdelijke locatie", messageList[3]);
                        StringAssert.StartsWith("Berekening van 'A' beëindigd om: ", messageList[4]);
                        StringAssert.StartsWith("Validatie van 'B' gestart om: ", messageList[5]);
                        StringAssert.StartsWith("Validatie van 'B' beëindigd om: ", messageList[6]);
                        StringAssert.StartsWith("Berekening van 'B' gestart om: ", messageList[7]);
                        StringAssert.StartsWith("Puntconstructies berekening is uitgevoerd op de tijdelijke locatie", messageList[8]);
                        StringAssert.StartsWith("Berekening van 'B' beëindigd om: ", messageList[9]);
                        Assert.AreEqual("Uitvoeren van berekening 'A' is gelukt.", messageList[10]);
                        Assert.AreEqual("Uitvoeren van berekening 'B' is gelukt.", messageList[11]);
                    });
                }
            }
        }

        [Test]
        public void ContextMenuStrip_ClickOnValidateAllItem_ValidateAllChildCalculations()
        {
            // Setup
            var failureMechanism = new TestStabilityPointStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new TestStabilityPointStructuresCalculation
            {
                Name = "A",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(),
                    InflowModelType = StabilityPointStructureInflowModelType.LowSill,
                    LoadSchematizationType = LoadSchematizationType.Linear
                }
            });
            failureMechanism.CalculationsGroup.Children.Add(new TestStabilityPointStructuresCalculation
            {
                Name = "B",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(),
                    InflowModelType = StabilityPointStructureInflowModelType.LowSill,
                    LoadSchematizationType = LoadSchematizationType.Linear
                }
            });

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var hydraulicBoundaryDatabase = mocksRepository.Stub<HydraulicBoundaryDatabase>();
            hydraulicBoundaryDatabase.FilePath = validFilePath;

            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;

            var failureMechanismContext = new StabilityPointStructuresFailureMechanismContext(failureMechanism, assessmentSection);

            using (var plugin = new StabilityPointStructuresPlugin())
            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var gui = mocksRepository.Stub<IGui>();
                gui.Stub(g => g.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);

                mocksRepository.ReplayAll();

                plugin.Gui = gui;
                TreeNodeInfo info = GetInfo(plugin);

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl))
                {
                    // Call
                    Action call = () => contextMenu.Items[contextMenuValidateAllIndex].PerformClick();

                    // Assert
                    TestHelper.AssertLogMessages(call, messages =>
                    {
                        string[] messageList = messages.ToArray();

                        Assert.AreEqual(4, messageList.Length);
                        StringAssert.StartsWith("Validatie van 'A' gestart om: ", messageList[0]);
                        StringAssert.StartsWith("Validatie van 'A' beëindigd om: ", messageList[1]);
                        StringAssert.StartsWith("Validatie van 'B' gestart om: ", messageList[2]);
                        StringAssert.StartsWith("Validatie van 'B' beëindigd om: ", messageList[3]);
                    });
                }
            }
        }

        private static TreeNodeInfo GetInfo(StabilityPointStructuresPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(StabilityPointStructuresFailureMechanismContext));
        }
    }
}