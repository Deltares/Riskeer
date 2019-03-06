// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Categories;
using Riskeer.ClosingStructures.Data;
using Riskeer.ClosingStructures.Data.TestUtil;
using Riskeer.ClosingStructures.Forms.PresentationObjects;
using Riskeer.Common.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Service.TestUtil;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Data.Input.Structures;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.ClosingStructures.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class ClosingStructuresFailureMechanismContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuRelevancyIndexWhenRelevant = 2;
        private const int contextMenuRelevancyIndexWhenNotRelevant = 0;

        private const int contextMenuValidateAllIndex = 4;
        private const int contextMenuCalculateAllIndex = 5;
        private const int contextMenuClearAllIndex = 7;
        private const int contextMenuClearIllustrationPointsIndex = 8;

        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, nameof(HydraulicBoundaryDatabase));

        private MockRepository mocksRepository;
        private ClosingStructuresPlugin plugin;
        private TreeNodeInfo info;

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            mocksRepository.ReplayAll();

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
            Assert.IsNull(info.CheckedState);
            Assert.IsNull(info.OnNodeChecked);
            Assert.IsNull(info.CanDrag);
            Assert.IsNull(info.CanDrop);
            Assert.IsNull(info.CanInsert);
            Assert.IsNull(info.OnDrop);
        }

        [Test]
        public void ChildNodeObjects_FailureMechanismIsRelevant_ReturnChildDataNodes()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var failureMechanismContext = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(failureMechanismContext).ToArray();

            // Assert
            Assert.AreEqual(3, children.Length);

            var inputsFolder = (CategoryTreeFolder) children[0];
            Assert.AreEqual("Invoer", inputsFolder.Name);
            Assert.AreEqual(TreeFolderCategory.Input, inputsFolder.Category);

            Assert.AreEqual(4, inputsFolder.Contents.Count());
            var failureMechanismSectionsContext = (ClosingStructuresFailureMechanismSectionsContext) inputsFolder.Contents.ElementAt(0);
            Assert.AreSame(failureMechanism, failureMechanismSectionsContext.WrappedData);
            Assert.AreSame(assessmentSection, failureMechanismSectionsContext.AssessmentSection);

            var profilesContext = (ForeshoreProfilesContext) inputsFolder.Contents.ElementAt(1);
            Assert.AreSame(failureMechanism.ForeshoreProfiles, profilesContext.WrappedData);
            Assert.AreSame(failureMechanism, profilesContext.ParentFailureMechanism);
            Assert.AreSame(assessmentSection, profilesContext.ParentAssessmentSection);

            var closingStructuresContext = (ClosingStructuresContext) inputsFolder.Contents.ElementAt(2);
            Assert.AreSame(failureMechanism.ClosingStructures, closingStructuresContext.WrappedData);
            Assert.AreSame(failureMechanism, closingStructuresContext.FailureMechanism);
            Assert.AreSame(assessmentSection, closingStructuresContext.AssessmentSection);

            var inputComment = (Comment) inputsFolder.Contents.ElementAt(3);
            Assert.AreSame(failureMechanism.InputComments, inputComment);

            var calculationsFolder = (ClosingStructuresCalculationGroupContext) children[1];
            Assert.AreEqual(failureMechanism.CalculationsGroup, calculationsFolder.WrappedData);
            Assert.IsNull(calculationsFolder.Parent);
            Assert.AreEqual(failureMechanism, calculationsFolder.FailureMechanism);

            var outputsFolder = (CategoryTreeFolder) children[2];
            Assert.AreEqual("Oordeel", outputsFolder.Name);
            Assert.AreEqual(TreeFolderCategory.Output, outputsFolder.Category);

            Assert.AreEqual(4, outputsFolder.Contents.Count());

            var failureMechanismAssemblyCategoriesContext = (FailureMechanismAssemblyCategoriesContext) outputsFolder.Contents.ElementAt(0);
            Assert.AreSame(failureMechanism, failureMechanismAssemblyCategoriesContext.WrappedData);
            Assert.AreSame(assessmentSection, failureMechanismAssemblyCategoriesContext.AssessmentSection);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssemblyCategoriesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyCategoriesCalculator;

                failureMechanismAssemblyCategoriesContext.GetFailureMechanismSectionAssemblyCategoriesFunc();
                Assert.AreEqual(failureMechanism.GeneralInput.N, calculator.AssemblyCategoriesInput.N);
            }

            var scenariosContext = (ClosingStructuresScenariosContext) outputsFolder.Contents.ElementAt(1);
            Assert.AreSame(failureMechanism.CalculationsGroup, scenariosContext.WrappedData);
            Assert.AreSame(failureMechanism, scenariosContext.ParentFailureMechanism);

            var failureMechanismResultsContext = (ProbabilityFailureMechanismSectionResultContext<ClosingStructuresFailureMechanismSectionResult>) outputsFolder.Contents.ElementAt(2);
            Assert.AreSame(failureMechanism, failureMechanismResultsContext.FailureMechanism);
            Assert.AreSame(failureMechanism.SectionResults, failureMechanismResultsContext.WrappedData);
            Assert.AreSame(assessmentSection, failureMechanismResultsContext.AssessmentSection);

            var outputComment = (Comment) outputsFolder.Contents.ElementAt(3);
            Assert.AreSame(failureMechanism.OutputComments, outputComment);
        }

        [Test]
        public void ChildNodeObjects_FailureMechanismIsNotRelevant_ReturnOnlyFailureMechanismNotRelevantComments()
        {
            // Setup
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            mocksRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism
            {
                IsRelevant = false
            };
            var failureMechanismContext = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(failureMechanismContext).ToArray();

            // Assert
            Assert.AreEqual(1, children.Length);
            var comment = (Comment) children[0];
            Assert.AreSame(failureMechanism.NotRelevantComments, comment);
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsRelevant_CallsContextMenuBuilderMethods()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            var failureMechanismContext = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSection);

            var menuBuilder = mocksRepository.StrictMock<IContextMenuBuilder>();
            using (mocksRepository.Ordered())
            {
                menuBuilder.Expect(mb => mb.AddOpenItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.Build()).Return(null);
            }

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocksRepository.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocksRepository.Stub<IMainWindow>());
                mocksRepository.ReplayAll();

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(failureMechanismContext, null, treeViewControl);
            }

            // Assert
            // Assert is done in TearDown
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsNotRelevant_CallsContextMenuBuilderMethods()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism
            {
                IsRelevant = false
            };
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            var failureMechanismContext = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSection);

            var menuBuilder = mocksRepository.StrictMock<IContextMenuBuilder>();
            using (mocksRepository.Ordered())
            {
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.Build()).Return(null);
            }

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocksRepository.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);
                mocksRepository.ReplayAll();

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(failureMechanismContext, null, treeViewControl);
            }

            // Assert
            // Assert is done in TearDown
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsRelevant_AddCustomItems()
        {
            // Setup
            using (var treeView = new TreeViewControl())
            {
                var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
                var failureMechanism = new ClosingStructuresFailureMechanism();
                var failureMechanismContext = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSection);
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocksRepository.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(failureMechanismContext, treeView)).Return(menuBuilder);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(g => g.MainWindow).Return(mocksRepository.Stub<IMainWindow>());
                mocksRepository.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(failureMechanismContext, assessmentSection, treeView))
                {
                    // Assert
                    Assert.AreEqual(14, menu.Items.Count);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuRelevancyIndexWhenRelevant,
                                                                  "I&s relevant",
                                                                  "Geeft aan of dit toetsspoor relevant is of niet.",
                                                                  RiskeerCommonFormsResources.Checkbox_ticked);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndex,
                                                                  "Alles &valideren",
                                                                  "Er zijn geen berekeningen om te valideren.",
                                                                  RiskeerCommonFormsResources.ValidateAllIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndex,
                                                                  "Alles be&rekenen",
                                                                  "Er zijn geen berekeningen om uit te voeren.",
                                                                  RiskeerCommonFormsResources.CalculateAllIcon,
                                                                  false);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearAllIndex,
                                                                  "&Wis alle uitvoer...",
                                                                  "Er zijn geen berekeningen met uitvoer om te wissen.",
                                                                  RiskeerCommonFormsResources.ClearIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearIllustrationPointsIndex,
                                                                  "Wis alle illustratiepunten...",
                                                                  "Er zijn geen berekeningen met illustratiepunten om te wissen.",
                                                                  RiskeerCommonFormsResources.ClearIllustrationPointsIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsNotRelevant_AddCustomItems()
        {
            // Setup
            using (var treeView = new TreeViewControl())
            {
                var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
                var failureMechanism = new ClosingStructuresFailureMechanism
                {
                    IsRelevant = false
                };
                var failureMechanismContext = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSection);
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocksRepository.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(failureMechanismContext, treeView)).Return(menuBuilder);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                mocksRepository.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(failureMechanismContext, assessmentSection, treeView))
                {
                    // Assert
                    Assert.AreEqual(6, menu.Items.Count);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuRelevancyIndexWhenNotRelevant,
                                                                  "I&s relevant",
                                                                  "Geeft aan of dit toetsspoor relevant is of niet.",
                                                                  RiskeerCommonFormsResources.Checkbox_empty);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsRelevantAndClickOnIsRelevantItem_MakeFailureMechanismNotRelevantAndRemovesAllViewsForItem()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            var failureMechanismContext = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSection);
            var viewCommands = mocksRepository.StrictMock<IViewCommands>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            viewCommands.Expect(vs => vs.RemoveAllViewsForItem(failureMechanismContext));

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocksRepository.Stub<IGui>();
                gui.Stub(g => g.ViewCommands).Return(viewCommands);
                gui.Stub(g => g.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocksRepository.Stub<IMainWindow>());
                mocksRepository.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl))
                {
                    // Call
                    contextMenu.Items[contextMenuRelevancyIndexWhenRelevant].PerformClick();

                    // Assert
                    Assert.IsFalse(failureMechanism.IsRelevant);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsNotRelevantAndClickOnIsRelevantItem_MakeFailureMechanismRelevantAndRemovesAllViewsForItem()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism
            {
                IsRelevant = false
            };
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            var failureMechanismContext = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSection);
            var viewCommands = mocksRepository.StrictMock<IViewCommands>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            viewCommands.Expect(vs => vs.RemoveAllViewsForItem(failureMechanismContext));

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocksRepository.Stub<IGui>();
                gui.Stub(g => g.ViewCommands).Return(viewCommands);
                gui.Stub(g => g.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);
                mocksRepository.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl))
                {
                    // Call
                    contextMenu.Items[contextMenuRelevancyIndexWhenNotRelevant].PerformClick();

                    // Assert
                    Assert.IsTrue(failureMechanism.IsRelevant);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_HydraulicBoundaryDatabaseNotLinked_ContextMenuItemCalculateAllAndValidateAllDisabledAndTooltipSet()
        {
            // Setup
            var failureMechanism = new TestClosingStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new StructuresCalculation<ClosingStructuresInput>());

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocksRepository);

            var nodeData = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocksRepository.Stub<IGui>();
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocksRepository.Stub<IMainWindow>());
                mocksRepository.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuCalculateAllIndex,
                                                                  "Alles be&rekenen",
                                                                  "Er is geen hydraulische belastingendatabase geïmporteerd.",
                                                                  RiskeerCommonFormsResources.CalculateAllIcon,
                                                                  false);

                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuValidateAllIndex,
                                                                  "Alles &valideren",
                                                                  "Er is geen hydraulische belastingendatabase geïmporteerd.",
                                                                  RiskeerCommonFormsResources.ValidateAllIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_HydraulicBoundaryDatabaseLinkedToInvalidFile_ContextMenuItemCalculateAllAndValidateAllDisabledAndTooltipSet()
        {
            // Setup
            var failureMechanism = new TestClosingStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new StructuresCalculation<ClosingStructuresInput>());

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(null, mocksRepository, "invalidFilePath");

            var nodeData = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocksRepository.Stub<IGui>();
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocksRepository.Stub<IMainWindow>());
                mocksRepository.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    ToolStripItem calculateAllContextMenuItem = contextMenu.Items[contextMenuCalculateAllIndex];

                    Assert.AreEqual("Alles be&rekenen", calculateAllContextMenuItem.Text);
                    StringAssert.Contains("Herstellen van de verbinding met de hydraulische belastingendatabase is mislukt. ", calculateAllContextMenuItem.ToolTipText);
                    TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.CalculateAllIcon, calculateAllContextMenuItem.Image);
                    Assert.IsFalse(calculateAllContextMenuItem.Enabled);

                    ToolStripItem validateAllContextMenuItem = contextMenu.Items[contextMenuValidateAllIndex];

                    Assert.AreEqual("Alles &valideren", validateAllContextMenuItem.Text);
                    StringAssert.Contains("Herstellen van de verbinding met de hydraulische belastingendatabase is mislukt. ", validateAllContextMenuItem.ToolTipText);
                    TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.ValidateAllIcon, validateAllContextMenuItem.Image);
                    Assert.IsFalse(validateAllContextMenuItem.Enabled);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismWithCalculationsContainingIllustrationPoints_ContextMenuItemClearIllustrationPointsEnabled()
        {
            // Setup
            var calculationWithIllustrationPoints = new TestClosingStructuresCalculation
            {
                Output = new TestStructuresOutput(new TestGeneralResultFaultTreeIllustrationPoint())
            };

            var calculationWithOutput = new TestClosingStructuresCalculation
            {
                Output = new TestStructuresOutput()
            };

            var failureMechanism = new TestClosingStructuresFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        calculationWithIllustrationPoints,
                        calculationWithOutput,
                        new TestClosingStructuresCalculation()
                    }
                }
            };

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(null, mocksRepository, "invalidFilePath");

            var nodeData = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocksRepository.Stub<IGui>();
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocksRepository.Stub<IMainWindow>());
                mocksRepository.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Call
                    ToolStripItem toolStripItem = contextMenu.Items[contextMenuClearIllustrationPointsIndex];

                    // Assert
                    Assert.IsTrue(toolStripItem.Enabled);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismWithCalculationsWithoutIllustrationPoints_ContextMenuItemClearIllustrationPointsDisabled()
        {
            // Setup
            var calculationWithOutput = new TestClosingStructuresCalculation
            {
                Output = new TestStructuresOutput()
            };

            var failureMechanism = new TestClosingStructuresFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        calculationWithOutput,
                        new TestClosingStructuresCalculation()
                    }
                }
            };

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(null, mocksRepository, "invalidFilePath");

            var nodeData = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocksRepository.Stub<IGui>();
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocksRepository.Stub<IMainWindow>());
                mocksRepository.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Call
                    ToolStripItem toolStripItem = contextMenu.Items[contextMenuClearIllustrationPointsIndex];

                    // Assert
                    Assert.IsFalse(toolStripItem.Enabled);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_AllRequiredInputSet_ContextMenuItemCalculateAllAndValidateAllEnabled()
        {
            // Setup
            var failureMechanism = new TestClosingStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new StructuresCalculation<ClosingStructuresInput>());

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "1.0"
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(hydraulicBoundaryDatabase);

            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase);

            var nodeData = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocksRepository.Stub<IGui>();
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocksRepository.Stub<IMainWindow>());
                mocksRepository.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndex,
                                                                  "Alles be&rekenen",
                                                                  "Voer alle berekeningen binnen dit toetsspoor uit.",
                                                                  RiskeerCommonFormsResources.CalculateAllIcon);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndex,
                                                                  "Alles &valideren",
                                                                  "Valideer alle berekeningen binnen dit toetsspoor.",
                                                                  RiskeerCommonFormsResources.ValidateAllIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_ClickOnCalculateAllItem_ScheduleAllChildCalculations()
        {
            // Setup
            var mainWindow = mocksRepository.Stub<IMainWindow>();
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

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = Path.Combine(testDataPath, "complete.sqlite")
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(hydraulicBoundaryDatabase);

            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.Id).Return(string.Empty);
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase);

            var failureMechanismContext = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocksRepository.Stub<IGui>();
                gui.Stub(g => g.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);

                int nrOfCalculators = failureMechanism.Calculations.Count();
                var calculatorFactory = mocksRepository.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresClosureCalculationInput>(
                                             Arg<HydraRingCalculationSettings>.Is.NotNull))
                                 .WhenCalled(invocation =>
                                 {
                                     HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                         HydraulicBoundaryCalculationSettingsFactory.CreateSettings(hydraulicBoundaryDatabase),
                                         (HydraRingCalculationSettings) invocation.Arguments[0]);
                                 })
                                 .Return(new TestStructuresCalculator<StructuresClosureCalculationInput>())
                                 .Repeat
                                 .Times(nrOfCalculators);
                mocksRepository.ReplayAll();

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
                        Assert.AreEqual(14, messageList.Count);
                        Assert.AreEqual("Uitvoeren van berekening 'A' is gestart.", messageList[0]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(messageList[1]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(messageList[2]);
                        CalculationServiceTestHelper.AssertCalculationStartMessage(messageList[3]);
                        StringAssert.StartsWith("Betrouwbaarheid sluiting kunstwerk berekening is uitgevoerd op de tijdelijke locatie", messageList[4]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(messageList[5]);
                        Assert.AreEqual("Uitvoeren van berekening 'A' is gelukt.", messageList[6]);

                        Assert.AreEqual("Uitvoeren van berekening 'B' is gestart.", messageList[7]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(messageList[8]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(messageList[9]);
                        CalculationServiceTestHelper.AssertCalculationStartMessage(messageList[10]);
                        StringAssert.StartsWith("Betrouwbaarheid sluiting kunstwerk berekening is uitgevoerd op de tijdelijke locatie", messageList[11]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(messageList[12]);
                        Assert.AreEqual("Uitvoeren van berekening 'B' is gelukt.", messageList[13]);
                    });
                }
            }
        }

        [Test]
        public void ContextMenuStrip_ClickOnValidateAllItem_ValidateAllChildCalculations()
        {
            // Setup
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

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(hydraulicBoundaryDatabase);

            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase);

            var failureMechanismContext = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var gui = mocksRepository.Stub<IGui>();
                gui.Stub(g => g.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocksRepository.Stub<IMainWindow>());
                mocksRepository.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl))
                {
                    // Call
                    Action call = () => contextMenu.Items[contextMenuValidateAllIndex].PerformClick();

                    // Assert
                    TestHelper.AssertLogMessages(call, messages =>
                    {
                        string[] messageList = messages.ToArray();

                        Assert.AreEqual(4, messageList.Length);
                        CalculationServiceTestHelper.AssertValidationStartMessage(messageList[0]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(messageList[1]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(messageList[2]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(messageList[3]);
                    });
                }
            }
        }

        [Test]
        public void GivenCalculationsWithIllustrationPoints_WhenClearIllustrationPointsClickedAndAborted_ThenInquiryAndIllustrationPointsNotCleared()
        {
            // Given
            var calculationWithIllustrationPoints = new TestClosingStructuresCalculation
            {
                Output = new TestStructuresOutput(new TestGeneralResultFaultTreeIllustrationPoint())
            };

            var calculationWithOutput = new TestClosingStructuresCalculation
            {
                Output = new TestStructuresOutput()
            };

            var failureMechanism = new TestClosingStructuresFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        calculationWithIllustrationPoints,
                        calculationWithOutput,
                        new TestClosingStructuresCalculation()
                    }
                }
            };

            var calculationObserver = mocksRepository.StrictMock<IObserver>();
            calculationWithIllustrationPoints.Attach(calculationObserver);

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(null, mocksRepository, "invalidFilePath");

            var nodeData = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var messageBoxText = "";
            DialogBoxHandler = (name, wnd) =>
            {
                var helper = new MessageBoxTester(wnd);
                messageBoxText = helper.Text;

                helper.ClickCancel();
            };

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocksRepository.Stub<IGui>();
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocksRepository.Stub<IMainWindow>());
                mocksRepository.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    contextMenu.Items[contextMenuClearIllustrationPointsIndex].PerformClick();

                    // Then
                    Assert.AreEqual("Weet u zeker dat u alle illustratiepunten wilt wissen?", messageBoxText);
                    Assert.IsTrue(calculationWithIllustrationPoints.Output.HasGeneralResult);
                }
            }
        }

        [Test]
        public void GivenCalculationsWithIllustrationPoints_WhenClearIllustrationPointsClickedAndContinued_ThenInquiryAndIllustrationPointsCleared()
        {
            // Given
            var calculationWithIllustrationPoints = new TestClosingStructuresCalculation
            {
                Output = new TestStructuresOutput(new TestGeneralResultFaultTreeIllustrationPoint())
            };

            var calculationWithOutput = new TestClosingStructuresCalculation
            {
                Output = new TestStructuresOutput()
            };

            var failureMechanism = new TestClosingStructuresFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        calculationWithIllustrationPoints,
                        calculationWithOutput,
                        new TestClosingStructuresCalculation()
                    }
                }
            };

            var affectedCalculationObserver = mocksRepository.StrictMock<IObserver>();
            affectedCalculationObserver.Expect(o => o.UpdateObserver());
            calculationWithIllustrationPoints.Attach(affectedCalculationObserver);

            var unaffectedCalculationObserver = mocksRepository.StrictMock<IObserver>();
            calculationWithOutput.Attach(unaffectedCalculationObserver);

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(null, mocksRepository, "invalidFilePath");

            var nodeData = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var messageBoxText = "";
            DialogBoxHandler = (name, wnd) =>
            {
                var helper = new MessageBoxTester(wnd);
                messageBoxText = helper.Text;

                helper.ClickOk();
            };

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocksRepository.Stub<IGui>();
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocksRepository.Stub<IMainWindow>());
                mocksRepository.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    contextMenu.Items[contextMenuClearIllustrationPointsIndex].PerformClick();

                    // Then
                    Assert.AreEqual("Weet u zeker dat u alle illustratiepunten wilt wissen?", messageBoxText);
                    Assert.IsFalse(calculationWithIllustrationPoints.Output.HasGeneralResult);
                }
            }
        }

        public override void Setup()
        {
            mocksRepository = new MockRepository();
            plugin = new ClosingStructuresPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(ClosingStructuresFailureMechanismContext));
        }

        public override void TearDown()
        {
            plugin.Dispose();

            mocksRepository.VerifyAll();
        }
    }
}