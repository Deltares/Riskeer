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
using System.Drawing;
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
using Riskeer.Common.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Service.TestUtil;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Forms.PresentationObjects;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Riskeer.MacroStabilityInwards.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class MacroStabilityInwardsFailureMechanismContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuRelevancyIndexWhenRelevant = 2;
        private const int contextMenuRelevancyIndexWhenNotRelevant = 0;

        private const int contextMenuValidateAllIndex = 4;
        private const int contextMenuCalculateAllIndex = 5;
        private const int contextMenuClearAllIndex = 7;

        private MockRepository mocks;
        private MacroStabilityInwardsPlugin plugin;
        private TreeNodeInfo info;

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            mocks.ReplayAll();

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
        public void Text_Always_ReturnsName()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var mechanism = new MacroStabilityInwardsFailureMechanism();
            var mechanismContext = new MacroStabilityInwardsFailureMechanismContext(mechanism, assessmentSection);

            // Call
            string text = info.Text(mechanismContext);

            // Assert
            Assert.AreEqual("Dijken en dammen - Macrostabiliteit binnenwaarts", text);
        }

        [Test]
        public void Image_Always_ReturnsMacroStabilityInwardsIcon()
        {
            // Setup
            mocks.ReplayAll();

            // Call
            Image image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.HydraulicCalculationIcon, image);
        }

        [Test]
        public void ChildNodeObjects_FailureMechanismIsRelevant_ReturnChildDataNodes()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new MacroStabilityInwardsCalculationScenario());
            failureMechanism.CalculationsGroup.Children.Add(new MacroStabilityInwardsCalculationScenario());

            var failureMechanismContext = new MacroStabilityInwardsFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(failureMechanismContext).ToArray();

            // Assert
            Assert.AreEqual(3, children.Length);
            var inputsFolder = (CategoryTreeFolder) children[0];
            Assert.AreEqual("Invoer", inputsFolder.Name);
            Assert.AreEqual(TreeFolderCategory.Input, inputsFolder.Category);

            Assert.AreEqual(4, inputsFolder.Contents.Count());
            var failureMechanismSectionsContext = (FailureMechanismSectionsContext) inputsFolder.Contents.ElementAt(0);
            Assert.AreSame(failureMechanism, failureMechanismSectionsContext.WrappedData);
            Assert.AreSame(assessmentSection, failureMechanismSectionsContext.AssessmentSection);

            var surfaceLinesContext = (MacroStabilityInwardsSurfaceLinesContext) inputsFolder.Contents.ElementAt(1);
            Assert.AreSame(failureMechanism.SurfaceLines, surfaceLinesContext.WrappedData);
            Assert.AreSame(failureMechanism, surfaceLinesContext.FailureMechanism);
            Assert.AreSame(assessmentSection, surfaceLinesContext.AssessmentSection);

            var stochasticSoilModelContext = (MacroStabilityInwardsStochasticSoilModelCollectionContext) inputsFolder.Contents.ElementAt(2);
            Assert.AreSame(failureMechanism, stochasticSoilModelContext.FailureMechanism);
            Assert.AreSame(failureMechanism, stochasticSoilModelContext.FailureMechanism);
            Assert.AreSame(assessmentSection, stochasticSoilModelContext.AssessmentSection);

            var comment = (Comment) inputsFolder.Contents.ElementAt(3);
            Assert.AreSame(failureMechanism.InputComments, comment);

            var calculationsFolder = (MacroStabilityInwardsCalculationGroupContext) children[1];
            Assert.AreSame(failureMechanism.CalculationsGroup, calculationsFolder.WrappedData);
            Assert.IsNull(calculationsFolder.Parent);
            Assert.AreSame(failureMechanism.SurfaceLines, calculationsFolder.AvailableMacroStabilityInwardsSurfaceLines);
            Assert.AreSame(failureMechanism.StochasticSoilModels, calculationsFolder.AvailableStochasticSoilModels);
            Assert.AreSame(failureMechanism, calculationsFolder.FailureMechanism);

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
                MacroStabilityInwardsProbabilityAssessmentInput probabilityAssessmentInput = failureMechanism.MacroStabilityInwardsProbabilityAssessmentInput;
                Assert.AreEqual(probabilityAssessmentInput.GetN(assessmentSection.ReferenceLine.Length), calculator.AssemblyCategoriesInput.N);
            }

            var failureMechanismScenariosContext = (MacroStabilityInwardsScenariosContext) outputsFolder.Contents.ElementAt(1);
            Assert.AreSame(failureMechanism, failureMechanismScenariosContext.FailureMechanism);
            Assert.AreSame(failureMechanism.CalculationsGroup, failureMechanismScenariosContext.WrappedData);
            Assert.AreSame(assessmentSection, failureMechanismScenariosContext.AssessmentSection);

            var failureMechanismResultsContext = (ProbabilityFailureMechanismSectionResultContext<MacroStabilityInwardsFailureMechanismSectionResult>) outputsFolder.Contents.ElementAt(2);
            Assert.AreSame(failureMechanism, failureMechanismResultsContext.FailureMechanism);
            Assert.AreSame(failureMechanism.SectionResults, failureMechanismResultsContext.WrappedData);
            Assert.AreSame(assessmentSection, failureMechanismResultsContext.AssessmentSection);

            var commentContext = (Comment) outputsFolder.Contents.ElementAt(3);
            Assert.AreSame(failureMechanism.OutputComments, commentContext);
        }

        [Test]
        public void ChildNodeObjects_FailureMechanismIsNotRelevant_ReturnOnlyFailureMechanismNotRelevantComments()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism
            {
                IsRelevant = false
            };

            var failureMechanismContext = new MacroStabilityInwardsFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(failureMechanismContext).ToArray();

            // Assert
            Assert.AreEqual(1, children.Length);
            var comment = (Comment) children[0];
            Assert.AreSame(failureMechanism.NotRelevantComments, comment);
        }

        [Test]
        public void ContextMenuStrip_HasCalculationWithOutput_ReturnsContextMenuWithCommonItems()
        {
            // Setup
            var failureMechanism = new TestMacroStabilityInwardsFailureMechanism();
            var calculation = new MacroStabilityInwardsCalculationScenario
            {
                Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
            };
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanismContext = new MacroStabilityInwardsFailureMechanismContext(failureMechanism, assessmentSection);

            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.Stub<IImportCommandHandler>();
            var exportCommandHandler = mocks.Stub<IExportCommandHandler>();
            var updateCommandHandler = mocks.Stub<IUpdateCommandHandler>();
            var viewCommandsHandler = mocks.Stub<IViewCommands>();

            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler,
                                                         importCommandHandler,
                                                         exportCommandHandler,
                                                         updateCommandHandler,
                                                         viewCommandsHandler,
                                                         failureMechanismContext,
                                                         treeViewControl);

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                gui.Stub(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl))
                {
                    // Assert
                    Assert.AreEqual(13, menu.Items.Count);

                    TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                  0,
                                                                  "&Openen",
                                                                  "Open de gegevens in een nieuw documentvenster.",
                                                                  CoreCommonGuiResources.OpenIcon,
                                                                  false);

                    TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                  2,
                                                                  "I&s relevant",
                                                                  "Geeft aan of dit toetsspoor relevant is of niet.",
                                                                  RiskeerCommonFormsResources.Checkbox_ticked);

                    TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                  4,
                                                                  "Alles &valideren",
                                                                  "Valideer alle berekeningen binnen dit toetsspoor.",
                                                                  RiskeerCommonFormsResources.ValidateAllIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                  5,
                                                                  "Alles be&rekenen",
                                                                  "Voer alle berekeningen binnen dit toetsspoor uit.",
                                                                  RiskeerCommonFormsResources.CalculateAllIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                  7,
                                                                  "&Wis alle uitvoer...",
                                                                  "Wis de uitvoer van alle berekeningen binnen dit toetsspoor.",
                                                                  RiskeerCommonFormsResources.ClearIcon);

                    TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                  9,
                                                                  "Alles i&nklappen",
                                                                  "Klap dit element en alle onderliggende elementen in.",
                                                                  CoreCommonGuiResources.CollapseAllIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                  10,
                                                                  "Alles ui&tklappen",
                                                                  "Klap dit element en alle onderliggende elementen uit.",
                                                                  CoreCommonGuiResources.ExpandAllIcon,
                                                                  false);

                    TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                  12,
                                                                  "Ei&genschappen",
                                                                  "Toon de eigenschappen in het Eigenschappenpaneel.",
                                                                  CoreCommonGuiResources.PropertiesHS,
                                                                  false);

                    CollectionAssert.AllItemsAreInstancesOfType(new[]
                    {
                        menu.Items[1],
                        menu.Items[3],
                        menu.Items[8],
                        menu.Items[11]
                    }, typeof(ToolStripSeparator));
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismWithCalculationsWithoutOutput_ContextMenuItemClearCalculationsOutputEnabled()
        {
            // Setup
            var calculationWithOutput = new MacroStabilityInwardsCalculationScenario
            {
                Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
            };

            var failureMechanism = new MacroStabilityInwardsFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        calculationWithOutput,
                        new MacroStabilityInwardsCalculationScenario()
                    }
                }
            };

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(null, mocks, "invalidFilePath");

            var nodeData = new MacroStabilityInwardsFailureMechanismContext(failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                gui.Stub(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Call
                    ToolStripItem toolStripItem = contextMenu.Items[contextMenuClearAllIndex];

                    // Assert
                    Assert.IsTrue(toolStripItem.Enabled);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismWithCalculationsWithoutOutput_ContextMenuItemClearCalculationsOutputDisabled()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        new MacroStabilityInwardsCalculationScenario()
                    }
                }
            };

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(null, mocks, "invalidFilePath");

            var nodeData = new MacroStabilityInwardsFailureMechanismContext(failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                gui.Stub(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Call
                    ToolStripItem toolStripItem = contextMenu.Items[contextMenuClearAllIndex];

                    // Assert
                    Assert.IsFalse(toolStripItem.Enabled);
                }
            }
        }

        [Test]
        public void GivenCalculationsWithOutput_WhenClearAllCalculationsOutputClickedAndAborted_ThenInquiryAndCalculationsOutputNotCleared()
        {
            // Given
            var calculationWithOutput = new MacroStabilityInwardsCalculationScenario
            {
                Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
            };

            var failureMechanism = new MacroStabilityInwardsFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        calculationWithOutput,
                        new MacroStabilityInwardsCalculationScenario()
                    }
                }
            };

            var calculationObserver = mocks.StrictMock<IObserver>();
            calculationWithOutput.Attach(calculationObserver);

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(null, mocks, "invalidFilePath");

            var nodeData = new MacroStabilityInwardsFailureMechanismContext(failureMechanism, assessmentSection);
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
                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                gui.Stub(g => g.ViewCommands).Return(mocks.StrictMock<IViewCommands>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    contextMenu.Items[contextMenuClearAllIndex].PerformClick();

                    // Then
                    Assert.AreEqual("Weet u zeker dat u alle uitvoer wilt wissen?", messageBoxText);

                    Assert.IsTrue(calculationWithOutput.HasOutput);
                }
            }
        }

        [Test]
        public void GivenCalculationsWithOutput_WhenClearAllCalculationsOutputClickedAndContinued_ThenInquiryAndOutputViewsClosedAndCalculationsOutputCleared()
        {
            // Given
            var calculationWithOutput = new MacroStabilityInwardsCalculationScenario
            {
                Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
            };

            var calculationWithoutOutput = new MacroStabilityInwardsCalculationScenario();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        calculationWithOutput,
                        calculationWithoutOutput
                    }
                }
            };

            var affectedCalculationObserver = mocks.StrictMock<IObserver>();
            affectedCalculationObserver.Expect(o => o.UpdateObserver());
            calculationWithOutput.Attach(affectedCalculationObserver);

            var unaffectedCalculationObserver = mocks.StrictMock<IObserver>();
            calculationWithoutOutput.Attach(unaffectedCalculationObserver);

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(null, mocks, "invalidFilePath");

            var nodeData = new MacroStabilityInwardsFailureMechanismContext(failureMechanism, assessmentSection);
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
                var viewCommands = mocks.StrictMock<IViewCommands>();
                viewCommands.Expect(vc => vc.RemoveAllViewsForItem(calculationWithOutput.Output));

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                gui.Stub(g => g.ViewCommands).Return(viewCommands);
                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    contextMenu.Items[contextMenuClearAllIndex].PerformClick();

                    // Then
                    Assert.AreEqual("Weet u zeker dat u alle uitvoer wilt wissen?", messageBoxText);

                    Assert.IsFalse(calculationWithOutput.HasOutput);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_AllRequiredInputSet_ContextMenuItemCalculateAllAndValidateAllEnabled()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSection = new AssessmentSectionStub();
                var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

                assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
                {
                    hydraulicBoundaryLocation
                }, true);

                var failureMechanism = new TestMacroStabilityInwardsFailureMechanism
                {
                    CalculationsGroup =
                    {
                        Children =
                        {
                            MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(hydraulicBoundaryLocation)
                        }
                    }
                };

                var failureMechanismContext = new MacroStabilityInwardsFailureMechanismContext(failureMechanism, assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                gui.Stub(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuCalculateAllIndex,
                                                                  "Alles be&rekenen",
                                                                  "Voer alle berekeningen binnen dit toetsspoor uit.",
                                                                  RiskeerCommonFormsResources.CalculateAllIcon);

                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuValidateAllIndex,
                                                                  "Alles &valideren",
                                                                  "Valideer alle berekeningen binnen dit toetsspoor.",
                                                                  RiskeerCommonFormsResources.ValidateAllIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsRelevant_CallsContextMenuBuilderMethods()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var failureMechanism = new MacroStabilityInwardsFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var failureMechanismContext = new MacroStabilityInwardsFailureMechanismContext(failureMechanism, assessmentSection);

                var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();
                using (mocks.Ordered())
                {
                    menuBuilder.Expect(mb => mb.AddOpenItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.Build()).Return(null);
                }

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                gui.Stub(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(failureMechanismContext, null, treeViewControl);
            }

            // Assert
            // Assert expectancies are called in TearDown()
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsNotRelevant_CallsContextMenuBuilderMethods()
        {
            // Setup
            var treeViewControl = new TreeViewControl();
            {
                var failureMechanism = new MacroStabilityInwardsFailureMechanism
                {
                    IsRelevant = false
                };
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var failureMechanismContext = new MacroStabilityInwardsFailureMechanismContext(failureMechanism, assessmentSection);

                var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();
                using (mocks.Ordered())
                {
                    menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.Build()).Return(null);
                }

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(failureMechanismContext, null, treeViewControl);
            }

            // Assert
            // Assert expectancies are called in TearDown()
        }

        [Test]
        public void ContextMenuStrip_ClickOnValidateAllItem_ValidateAllChildCalculations()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSection = new AssessmentSectionStub();
                var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

                assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
                {
                    hydraulicBoundaryLocation
                }, true);

                var failureMechanism = new TestMacroStabilityInwardsFailureMechanism();
                MacroStabilityInwardsCalculationScenario validCalculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(hydraulicBoundaryLocation);
                validCalculation.Name = "A";
                MacroStabilityInwardsCalculationScenario invalidCalculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithInvalidInput();
                invalidCalculation.Name = "B";

                failureMechanism.CalculationsGroup.Children.Add(validCalculation);
                failureMechanism.CalculationsGroup.Children.Add(invalidCalculation);

                var failureMechanismContext = new MacroStabilityInwardsFailureMechanismContext(failureMechanism, assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                gui.Stub(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl))
                using (new MacroStabilityInwardsCalculatorFactoryConfig())
                {
                    // Call
                    Action call = () => contextMenu.Items[contextMenuValidateAllIndex].PerformClick();

                    // Assert
                    TestHelper.AssertLogMessages(call, messages =>
                    {
                        string[] msgs = messages.ToArray();
                        Assert.AreEqual(9, msgs.Length);
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                        Assert.AreEqual("Validatie van waterspanningen in extreme omstandigheden is gestart.", msgs[1]);
                        Assert.AreEqual("Validatie van waterspanningen in dagelijkse omstandigheden is gestart.", msgs[2]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs[3]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs[4]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs[8]);
                    });
                }
            }
        }

        [Test]
        public void ContextMenuStrip_ClickOnCalculateAllItem_ScheduleAllChildCalculations()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var failureMechanism = new TestMacroStabilityInwardsFailureMechanism();
                var assessmentSection = new AssessmentSectionStub();
                var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

                assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
                {
                    hydraulicBoundaryLocation
                }, true);

                MacroStabilityInwardsCalculationScenario calculationA = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(hydraulicBoundaryLocation);
                calculationA.Name = "A";
                MacroStabilityInwardsCalculationScenario calculationB = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(hydraulicBoundaryLocation);
                calculationB.Name = "B";

                failureMechanism.CalculationsGroup.Children.Add(calculationA);
                failureMechanism.CalculationsGroup.Children.Add(calculationB);

                var failureMechanismContext = new MacroStabilityInwardsFailureMechanismContext(failureMechanism, assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                gui.Stub(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                DialogBoxHandler = (name, wnd) =>
                {
                    // Expect an activity dialog which is automatically closed
                };

                using (new MacroStabilityInwardsCalculatorFactoryConfig())
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl))
                {
                    // Call
                    Action call = () => contextMenu.Items[contextMenuCalculateAllIndex].PerformClick();

                    // Assert
                    TestHelper.AssertLogMessages(call, messages =>
                    {
                        string[] msgs = messages.ToArray();
                        Assert.AreEqual(16, msgs.Length);
                        Assert.AreEqual("Uitvoeren van berekening 'A' is gestart.", msgs[0]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                        Assert.AreEqual("Validatie van waterspanningen in extreme omstandigheden is gestart.", msgs[2]);
                        Assert.AreEqual("Validatie van waterspanningen in dagelijkse omstandigheden is gestart.", msgs[3]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs[4]);
                        CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[5]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[6]);
                        Assert.AreEqual("Uitvoeren van berekening 'A' is gelukt.", msgs[7]);

                        Assert.AreEqual("Uitvoeren van berekening 'B' is gestart.", msgs[8]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs[9]);
                        Assert.AreEqual("Validatie van waterspanningen in extreme omstandigheden is gestart.", msgs[10]);
                        Assert.AreEqual("Validatie van waterspanningen in dagelijkse omstandigheden is gestart.", msgs[11]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs[12]);
                        CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[13]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[14]);
                        Assert.AreEqual("Uitvoeren van berekening 'B' is gelukt.", msgs[15]);
                    });
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsRelevantAndClickOnIsRelevantItem_MakeFailureMechanismNotRelevant()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var failureMechanismObserver = mocks.Stub<IObserver>();
                failureMechanismObserver.Expect(o => o.UpdateObserver());

                var failureMechanism = new MacroStabilityInwardsFailureMechanism();
                failureMechanism.Attach(failureMechanismObserver);

                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var failureMechanismContext = new MacroStabilityInwardsFailureMechanismContext(failureMechanism, assessmentSection);

                var viewCommands = mocks.StrictMock<IViewCommands>();
                viewCommands.Expect(vs => vs.RemoveAllViewsForItem(failureMechanismContext));

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                gui.Stub(g => g.ViewCommands).Return(viewCommands);
                mocks.ReplayAll();

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
        public void ContextMenuStrip_FailureMechanismIsNotRelevantAndClickOnIsRelevantItem_MakeFailureMechanismRelevant()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var failureMechanismObserver = mocks.Stub<IObserver>();
                failureMechanismObserver.Expect(o => o.UpdateObserver());

                var failureMechanism = new MacroStabilityInwardsFailureMechanism
                {
                    IsRelevant = false
                };
                failureMechanism.Attach(failureMechanismObserver);

                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var failureMechanismContext = new MacroStabilityInwardsFailureMechanismContext(failureMechanism, assessmentSection);

                var viewCommands = mocks.StrictMock<IViewCommands>();
                viewCommands.Expect(vs => vs.RemoveAllViewsForItem(failureMechanismContext));

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.ViewCommands).Return(viewCommands);
                gui.Stub(g => g.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);
                mocks.ReplayAll();

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

        public override void Setup()
        {
            mocks = new MockRepository();
            plugin = new MacroStabilityInwardsPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(MacroStabilityInwardsFailureMechanismContext));
        }

        public override void TearDown()
        {
            plugin.Dispose();
            mocks.VerifyAll();

            base.TearDown();
        }
    }
}