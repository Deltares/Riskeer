﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Riskeer.Common.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Service.TestUtil;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Data.TestUtil.Probabilistic;
using Riskeer.Piping.Data.TestUtil.SemiProbabilistic;
using Riskeer.Piping.Forms.PresentationObjects;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Riskeer.Piping.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class PipingFailureMechanismContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuRelevancyIndexWhenRelevant = 2;
        private const int contextMenuRelevancyIndexWhenNotRelevant = 0;

        private const int contextMenuValidateAllIndex = 4;
        private const int contextMenuCalculateAllIndex = 5;
        private const int contextMenuClearIndex = 7;

        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Service, "HydraRingCalculation");
        private static readonly string validHydraulicBoundaryDatabaseFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

        private MockRepository mocks;
        private PipingPlugin plugin;
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

            var mechanism = new PipingFailureMechanism();
            var mechanismContext = new PipingFailureMechanismContext(mechanism, assessmentSection);

            // Call
            string text = info.Text(mechanismContext);

            // Assert
            Assert.AreEqual("Dijken en dammen - Piping", text);
        }

        [Test]
        public void Image_Always_ReturnsPipingIcon()
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
            var pipingFailureMechanism = new PipingFailureMechanism();
            var pipingFailureMechanismContext = new PipingFailureMechanismContext(pipingFailureMechanism, assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(pipingFailureMechanismContext).ToArray();

            // Assert
            Assert.AreEqual(3, children.Length);
            var inputsFolder = (CategoryTreeFolder) children[0];
            Assert.AreEqual("Invoer", inputsFolder.Name);
            Assert.AreEqual(TreeFolderCategory.Input, inputsFolder.Category);

            Assert.AreEqual(4, inputsFolder.Contents.Count());
            var failureMechanismSectionsContext = (FailureMechanismSectionsContext) inputsFolder.Contents.ElementAt(0);
            Assert.AreSame(pipingFailureMechanism, failureMechanismSectionsContext.WrappedData);
            Assert.AreSame(assessmentSection, failureMechanismSectionsContext.AssessmentSection);

            var surfaceLinesContext = (PipingSurfaceLinesContext) inputsFolder.Contents.ElementAt(1);
            Assert.AreSame(pipingFailureMechanism.SurfaceLines, surfaceLinesContext.WrappedData);
            Assert.AreSame(pipingFailureMechanism, surfaceLinesContext.FailureMechanism);
            Assert.AreSame(assessmentSection, surfaceLinesContext.AssessmentSection);

            var stochasticSoilModelContext = (PipingStochasticSoilModelCollectionContext) inputsFolder.Contents.ElementAt(2);
            Assert.AreSame(pipingFailureMechanism, stochasticSoilModelContext.FailureMechanism);
            Assert.AreSame(pipingFailureMechanism, stochasticSoilModelContext.FailureMechanism);
            Assert.AreSame(assessmentSection, stochasticSoilModelContext.AssessmentSection);

            var comment = (Comment) inputsFolder.Contents.ElementAt(3);
            Assert.AreSame(pipingFailureMechanism.InputComments, comment);

            var calculationsFolder = (PipingCalculationGroupContext) children[1];
            Assert.AreSame(pipingFailureMechanism.CalculationsGroup, calculationsFolder.WrappedData);
            Assert.IsNull(calculationsFolder.Parent);
            Assert.AreSame(pipingFailureMechanism.SurfaceLines, calculationsFolder.AvailablePipingSurfaceLines);
            Assert.AreSame(pipingFailureMechanism.StochasticSoilModels, calculationsFolder.AvailableStochasticSoilModels);
            Assert.AreSame(pipingFailureMechanism, calculationsFolder.FailureMechanism);

            var outputsFolder = (CategoryTreeFolder) children[2];
            Assert.AreEqual("Oordeel", outputsFolder.Name);
            Assert.AreEqual(TreeFolderCategory.Output, outputsFolder.Category);

            Assert.AreEqual(4, outputsFolder.Contents.Count());
            var failureMechanismAssemblyCategoriesContext = (FailureMechanismAssemblyCategoriesContext) outputsFolder.Contents.ElementAt(0);
            Assert.AreSame(pipingFailureMechanism, failureMechanismAssemblyCategoriesContext.WrappedData);
            Assert.AreSame(assessmentSection, failureMechanismAssemblyCategoriesContext.AssessmentSection);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssemblyCategoriesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyCategoriesCalculator;

                failureMechanismAssemblyCategoriesContext.GetFailureMechanismSectionAssemblyCategoriesFunc();
                PipingProbabilityAssessmentInput probabilityAssessmentInput = pipingFailureMechanism.PipingProbabilityAssessmentInput;
                Assert.AreEqual(probabilityAssessmentInput.GetN(assessmentSection.ReferenceLine.Length), calculator.AssemblyCategoriesInput.N);
            }

            var failureMechanismScenariosContext = (PipingScenariosContext) outputsFolder.Contents.ElementAt(1);
            Assert.AreSame(pipingFailureMechanism, failureMechanismScenariosContext.FailureMechanism);
            Assert.AreSame(pipingFailureMechanism.CalculationsGroup, failureMechanismScenariosContext.WrappedData);
            Assert.AreSame(assessmentSection, failureMechanismScenariosContext.AssessmentSection);

            var failureMechanismResultsContext = (ProbabilityFailureMechanismSectionResultContext<PipingFailureMechanismSectionResult>) outputsFolder.Contents.ElementAt(2);
            Assert.AreSame(pipingFailureMechanism, failureMechanismResultsContext.FailureMechanism);
            Assert.AreSame(pipingFailureMechanism.SectionResults, failureMechanismResultsContext.WrappedData);
            Assert.AreSame(assessmentSection, failureMechanismResultsContext.AssessmentSection);

            var commentContext = (Comment) outputsFolder.Contents.ElementAt(3);
            Assert.AreSame(pipingFailureMechanism.OutputComments, commentContext);
        }

        [Test]
        public void ChildNodeObjects_FailureMechanismIsNotRelevant_ReturnOnlyFailureMechanismNotRelevantComments()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var pipingFailureMechanism = new PipingFailureMechanism
            {
                IsRelevant = false
            };

            var pipingFailureMechanismContext = new PipingFailureMechanismContext(pipingFailureMechanism, assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(pipingFailureMechanismContext).ToArray();

            // Assert
            Assert.AreEqual(1, children.Length);
            var comment = (Comment) children[0];
            Assert.AreSame(pipingFailureMechanism.NotRelevantComments, comment);
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void GivenMultiplePipingCalculationsWithOutput_WhenClearingOutputFromContextMenu_ThenPipingOutputCleared(bool confirm)
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                var pipingCalculation1 = new TestPipingCalculationScenario(true);
                var pipingCalculation2 = new TestPipingCalculationScenario(true);

                var observer = mocks.StrictMock<IObserver>();
                if (confirm)
                {
                    observer.Expect(o => o.UpdateObserver()).Repeat.Twice();
                }

                var failureMechanism = new PipingFailureMechanism();
                failureMechanism.CalculationsGroup.Children.Add(pipingCalculation1);
                failureMechanism.CalculationsGroup.Children.Add(pipingCalculation2);

                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var failureMechanismContext = new PipingFailureMechanismContext(failureMechanism, assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);
                mocks.ReplayAll();

                plugin.Gui = gui;

                failureMechanism.CalculationsGroup.Children.Add(pipingCalculation1);
                failureMechanism.CalculationsGroup.Children.Add(pipingCalculation2);
                failureMechanism.CalculationsGroup.Children.ElementAt(0).Attach(observer);
                failureMechanism.CalculationsGroup.Children.ElementAt(1).Attach(observer);

                string messageBoxTitle = null, messageBoxText = null;
                DialogBoxHandler = (name, wnd) =>
                {
                    var messageBox = new MessageBoxTester(wnd);

                    messageBoxText = messageBox.Text;
                    messageBoxTitle = messageBox.Title;

                    if (confirm)
                    {
                        messageBox.ClickOk();
                    }
                    else
                    {
                        messageBox.ClickCancel();
                    }
                };

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl))
                {
                    // When
                    contextMenuStrip.Items[contextMenuClearIndex].PerformClick();

                    // Then
                    foreach (ICalculation calc in failureMechanism.CalculationsGroup.Children.OfType<ICalculation>())
                    {
                        Assert.AreNotEqual(confirm, calc.HasOutput);
                    }

                    Assert.AreEqual("Bevestigen", messageBoxTitle);
                    Assert.AreEqual("Weet u zeker dat u alle uitvoer wilt wissen?", messageBoxText);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismHasCalculationWithOutput_ReturnsContextMenuWithCommonItems()
        {
            // Setup
            var failureMechanism = new TestPipingFailureMechanism();
            var pipingCalculation = new TestPipingCalculationScenario(true);
            failureMechanism.CalculationsGroup.Children.Add(pipingCalculation);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanismContext = new PipingFailureMechanismContext(failureMechanism, assessmentSection);

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
        public void ContextMenuStrip_FailureMechanismHasNoCalculationsWithOutput_ContextMenuItemClearAllOutputDisabled()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var data = mocks.StrictMock<PipingFailureMechanism>();
                data.Stub(dm => dm.Calculations).Return(new[]
                {
                    new TestPipingCalculationScenario()
                });

                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var failureMechanismContext = new PipingFailureMechanismContext(data, assessmentSection);

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl))
                {
                    // Assert
                    ToolStripItem clearOutputItem = contextMenu.Items[contextMenuClearIndex];
                    Assert.IsFalse(clearOutputItem.Enabled);
                    Assert.AreEqual("Er zijn geen berekeningen met uitvoer om te wissen.", clearOutputItem.ToolTipText);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismHasCalculationsWithOutput_ContextMenuItemClearAllOutputEnabled()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var pipingCalculation = new TestPipingCalculationScenario(true);

                var failureMechanism = new PipingFailureMechanism();
                failureMechanism.CalculationsGroup.Children.Add(pipingCalculation);

                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var failureMechanismContext = new PipingFailureMechanismContext(failureMechanism, assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);
                mocks.ReplayAll();

                plugin.Gui = gui;

                failureMechanism.CalculationsGroup.Children.Add(pipingCalculation);

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl))
                {
                    // Assert
                    ToolStripItem clearOutputItem = contextMenu.Items[contextMenuClearIndex];
                    Assert.IsTrue(clearOutputItem.Enabled);
                    Assert.AreEqual("Wis de uitvoer van alle berekeningen binnen dit toetsspoor.", clearOutputItem.ToolTipText);
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
                var failureMechanism = new TestPipingFailureMechanism
                {
                    CalculationsGroup =
                    {
                        Children =
                        {
                            new TestPipingCalculationScenario()
                        }
                    }
                };

                var failureMechanismContext = new PipingFailureMechanismContext(failureMechanism, assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);
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
                var pipingFailureMechanism = new PipingFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var pipingFailureMechanismContext = new PipingFailureMechanismContext(pipingFailureMechanism, assessmentSection);

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
                gui.Stub(cmp => cmp.Get(pipingFailureMechanismContext, treeViewControl)).Return(menuBuilder);
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(pipingFailureMechanismContext, null, treeViewControl);
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
                var pipingFailureMechanism = new PipingFailureMechanism
                {
                    IsRelevant = false
                };
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var pipingFailureMechanismContext = new PipingFailureMechanismContext(pipingFailureMechanism, assessmentSection);

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
                gui.Stub(cmp => cmp.Get(pipingFailureMechanismContext, treeViewControl)).Return(menuBuilder);
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(pipingFailureMechanismContext, null, treeViewControl);
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
                TestPipingFailureMechanism failureMechanism = TestPipingFailureMechanism.GetFailureMechanismWithSurfaceLinesAndStochasticSoilModels();

                assessmentSection.HydraulicBoundaryDatabase.FilePath = validHydraulicBoundaryDatabaseFilePath;
                HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);
                assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
                {
                    hydraulicBoundaryLocation
                }, true);

                SemiProbabilisticPipingCalculationScenario validSemiProbabilisticCalculation =
                    SemiProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<SemiProbabilisticPipingCalculationScenario>(hydraulicBoundaryLocation);
                SemiProbabilisticPipingCalculationScenario invalidSemiProbabilisticCalculation =
                    SemiProbabilisticPipingCalculationTestFactory.CreateCalculationWithInvalidInput<SemiProbabilisticPipingCalculationScenario>();
                ProbabilisticPipingCalculationScenario validProbabilisticCalculation =
                    ProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<ProbabilisticPipingCalculationScenario>(hydraulicBoundaryLocation);
                ProbabilisticPipingCalculationScenario invalidProbabilisticCalculation =
                    ProbabilisticPipingCalculationTestFactory.CreateCalculationWithInvalidInput<ProbabilisticPipingCalculationScenario>();

                failureMechanism.CalculationsGroup.Children.Add(validSemiProbabilisticCalculation);
                failureMechanism.CalculationsGroup.Children.Add(invalidProbabilisticCalculation);
                failureMechanism.CalculationsGroup.Children.Add(validProbabilisticCalculation);
                failureMechanism.CalculationsGroup.Children.Add(invalidSemiProbabilisticCalculation);

                var failureMechanismContext = new PipingFailureMechanismContext(failureMechanism, assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);
                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl))
                {
                    // Call
                    void Call() => contextMenu.Items[contextMenuValidateAllIndex].PerformClick();

                    // Assert
                    TestHelper.AssertLogMessages(Call, messages =>
                    {
                        string[] msgs = messages.ToArray();
                        Assert.AreEqual(18, msgs.Length);
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs[1]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs[2]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs[8]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs[9]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs[10]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs[11]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs[17]);
                    });
                }
            }
        }

        [Test]
        public void GivenFailureMechanismWithCalculationOfUnsupportedType_WhenClickOnValidateAllItemOfContextMenuStrip_ThenThrowsNotSupportedException()
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSection = new AssessmentSectionStub();
                var failureMechanism = new TestPipingFailureMechanism();

                failureMechanism.CalculationsGroup.Children.Add(new TestPipingCalculationScenario());

                var failureMechanismContext = new PipingFailureMechanismContext(failureMechanism, assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);

                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl))
                {
                    // When
                    void When() => contextMenu.Items[contextMenuValidateAllIndex].PerformClick();

                    // Then
                    Assert.Throws<NotSupportedException>(When);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_ClickOnCalculateAllItem_ScheduleAllChildCalculations()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSection = new AssessmentSectionStub();
                var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
                TestPipingFailureMechanism failureMechanism = TestPipingFailureMechanism.GetFailureMechanismWithSurfaceLinesAndStochasticSoilModels();

                assessmentSection.HydraulicBoundaryDatabase.FilePath = validHydraulicBoundaryDatabaseFilePath;
                HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);
                assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
                {
                    hydraulicBoundaryLocation
                }, true);

                SemiProbabilisticPipingCalculationScenario calculationA =
                    SemiProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<SemiProbabilisticPipingCalculationScenario>(hydraulicBoundaryLocation);
                calculationA.Name = "A";
                SemiProbabilisticPipingCalculationScenario calculationB =
                    SemiProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<SemiProbabilisticPipingCalculationScenario>(hydraulicBoundaryLocation);
                calculationB.Name = "B";
                ProbabilisticPipingCalculationScenario calculationC =
                    ProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<ProbabilisticPipingCalculationScenario>(hydraulicBoundaryLocation);
                calculationC.Name = "C";
                ProbabilisticPipingCalculationScenario calculationD =
                    ProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<ProbabilisticPipingCalculationScenario>(hydraulicBoundaryLocation);
                calculationD.Name = "D";

                failureMechanism.CalculationsGroup.Children.Add(calculationA);
                failureMechanism.CalculationsGroup.Children.Add(calculationC);
                failureMechanism.CalculationsGroup.Children.Add(calculationB);
                failureMechanism.CalculationsGroup.Children.Add(calculationD);

                var failureMechanismContext = new PipingFailureMechanismContext(failureMechanism, assessmentSection);

                var mainWindow = mocks.Stub<IMainWindow>();
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);

                plugin.Gui = gui;

                DialogBoxHandler = (name, wnd) =>
                {
                    // Expect an activity dialog which is automatically closed
                };

                var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
                calculatorFactory.Expect(cf => cf.CreatePipingCalculator(null))
                                 .IgnoreArguments()
                                 .Return(new TestPipingCalculator())
                                 .Repeat.Any();

                mocks.ReplayAll();

                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl))
                {
                    // Call
                    void Call() => contextMenu.Items[contextMenuCalculateAllIndex].PerformClick();

                    // Assert
                    TestHelper.AssertLogMessages(Call, messages =>
                    {
                        string[] msgs = messages.ToArray();
                        Assert.AreEqual(28, msgs.Length);

                        Assert.AreEqual("Uitvoeren van berekening 'A' is gestart.", msgs[0]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                        CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[3]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[4]);
                        Assert.AreEqual("Uitvoeren van berekening 'A' is gelukt.", msgs[5]);

                        Assert.AreEqual("Uitvoeren van berekening 'C' is gestart.", msgs[6]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs[7]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs[8]);
                        CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[9]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[12]);
                        Assert.AreEqual("Uitvoeren van berekening 'C' is gelukt.", msgs[13]);

                        Assert.AreEqual("Uitvoeren van berekening 'B' is gestart.", msgs[14]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs[15]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs[16]);
                        CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[17]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[18]);
                        Assert.AreEqual("Uitvoeren van berekening 'B' is gelukt.", msgs[19]);

                        Assert.AreEqual("Uitvoeren van berekening 'D' is gestart.", msgs[20]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs[21]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs[22]);
                        CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[23]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[26]);
                        Assert.AreEqual("Uitvoeren van berekening 'D' is gelukt.", msgs[27]);
                    });
                }
            }
        }

        [Test]
        public void GivenFailureMechanismWithCalculationOfUnsupportedType_WhenClickOnCalculateAllItemOfContextMenuStrip_ThenThrowsNotSupportedException()
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSection = new AssessmentSectionStub();
                var failureMechanism = new TestPipingFailureMechanism();

                failureMechanism.CalculationsGroup.Children.Add(new TestPipingCalculationScenario());

                var failureMechanismContext = new PipingFailureMechanismContext(failureMechanism, assessmentSection);

                var mainWindow = mocks.Stub<IMainWindow>();
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);

                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl))
                {
                    // When
                    void When() => contextMenu.Items[contextMenuCalculateAllIndex].PerformClick();

                    // Then
                    Assert.Throws<NotSupportedException>(When);
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

                var failureMechanism = new PipingFailureMechanism();
                failureMechanism.Attach(failureMechanismObserver);

                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var failureMechanismContext = new PipingFailureMechanismContext(failureMechanism, assessmentSection);

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

                var failureMechanism = new PipingFailureMechanism
                {
                    IsRelevant = false
                };
                failureMechanism.Attach(failureMechanismObserver);

                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var failureMechanismContext = new PipingFailureMechanismContext(failureMechanism, assessmentSection);

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
            plugin = new PipingPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(PipingFailureMechanismContext));
        }

        public override void TearDown()
        {
            plugin.Dispose();
            mocks.VerifyAll();

            base.TearDown();
        }
    }
}