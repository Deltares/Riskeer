// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Threading;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Storage;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Common.Util;
using Core.Gui;
using Core.Gui.ContextMenu;
using Core.Gui.Forms.Main;
using Core.Gui.TestUtil;
using Core.Gui.TestUtil.ContextMenu;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Service.TestUtil;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Data.Input.Hydraulics;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;
using Riskeer.Integration.Data;
using Riskeer.Integration.TestUtil;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class WaterLevelCalculationsForUserDefinedTargetProbabilityContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuRunWaterLevelCalculationsIndex = 2;
        private const int contextMenuClearIllustrationPointsIndex = 4;

        private MockRepository mockRepository;

        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Service, "HydraRingCalculation");
        private static readonly string validFilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Assert
                Assert.IsNotNull(info.Text);
                Assert.IsNull(info.ForeColor);
                Assert.IsNotNull(info.Image);
                Assert.IsNotNull(info.ContextMenuStrip);
                Assert.IsNotNull(info.EnsureVisibleOnCreate);
                Assert.IsNull(info.ExpandOnCreate);
                Assert.IsNull(info.ChildNodeObjects);
                Assert.IsNull(info.CanRename);
                Assert.IsNull(info.OnNodeRenamed);
                Assert.IsNotNull(info.CanRemove);
                Assert.IsNotNull(info.OnNodeRemoved);
                Assert.IsNull(info.CanCheck);
                Assert.IsNull(info.CheckedState);
                Assert.IsNull(info.OnNodeChecked);
                Assert.IsNotNull(info.CanDrag);
                Assert.IsNull(info.CanDrop);
                Assert.IsNull(info.CanInsert);
                Assert.IsNull(info.OnDrop);
            }
        }

        [Test]
        public void Text_WithContext_ReturnsFormattedTargetProbability()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mockRepository);
            mockRepository.ReplayAll();

            var context = new WaterLevelCalculationsForUserDefinedTargetProbabilityContext(new HydraulicBoundaryLocationCalculationsForTargetProbability
                                                                                           {
                                                                                               TargetProbability = 0.01
                                                                                           },
                                                                                           assessmentSection);

            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                string text = info.Text(context);

                // Assert
                Assert.AreEqual("1/100", text);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Image_Always_ReturnsGenericInputOutputIcon()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Image image = info.Image(null);

                // Assert
                TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.GenericInputOutputIcon, image);
            }
        }

        [Test]
        public void EnsureVisibleOnCreate_Always_ReturnsTrue()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                bool ensureVisibleOnCreate = info.EnsureVisibleOnCreate(null, null);

                // Assert
                Assert.IsTrue(ensureVisibleOnCreate);
            }
        }

        [Test]
        public void CanRemove_Always_ReturnsTrue()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                bool canRemove = info.CanRemove(null, null);

                // Assert
                Assert.IsTrue(canRemove);
            }
        }

        [Test]
        public void OnNodeRemoved_WithContexts_RemovesItemAndNotifiesObservers()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mockRepository);

            var calculationObserver = mockRepository.StrictMock<IObserver>();
            calculationObserver.Expect(o => o.UpdateObserver());

            mockRepository.ReplayAll();

            var calculationForFirstTargetProbability = new HydraulicBoundaryLocationCalculationsForTargetProbability();
            var calculationForSecondTargetProbability = new HydraulicBoundaryLocationCalculationsForTargetProbability();
            var calculations = new ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>
            {
                calculationForFirstTargetProbability,
                calculationForSecondTargetProbability
            };

            calculations.Attach(calculationObserver);

            var parentContext = new WaterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContext(calculations,
                                                                                                        assessmentSection);

            var context = new WaterLevelCalculationsForUserDefinedTargetProbabilityContext(calculationForFirstTargetProbability,
                                                                                           assessmentSection);

            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                info.OnNodeRemoved(context, parentContext);

                // Assert
                Assert.AreEqual(1, calculations.Count);
                CollectionAssert.DoesNotContain(calculations, calculationForFirstTargetProbability);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mockRepository);

            var menuBuilder = mockRepository.StrictMock<IContextMenuBuilder>();
            using (mockRepository.Ordered())
            {
                menuBuilder.Expect(mb => mb.AddOpenItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddDeleteItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.Build()).Return(null);
            }

            var nodeData = new WaterLevelCalculationsForUserDefinedTargetProbabilityContext(new HydraulicBoundaryLocationCalculationsForTargetProbability(),
                                                                                            assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                IGui gui = StubFactory.CreateGuiStub(mockRepository);
                gui.Stub(cmp => cmp.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                mockRepository.ReplayAll();

                using (var plugin = new RiskeerPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    plugin.Gui = gui;

                    // Call
                    info.ContextMenuStrip(nodeData, null, treeViewControl);
                }
            }

            // Assert
            mockRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_Always_AddCustomItems()
        {
            // Setup
            IAssessmentSection assessmentSection = new AssessmentSectionStub();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            var nodeData = new WaterLevelCalculationsForUserDefinedTargetProbabilityContext(new HydraulicBoundaryLocationCalculationsForTargetProbability(),
                                                                                            assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                IGui gui = StubFactory.CreateGuiStub(mockRepository);
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                mockRepository.ReplayAll();

                using (var plugin = new RiskeerPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    plugin.Gui = gui;

                    // Call
                    using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, assessmentSection, treeViewControl))
                    {
                        // Assert
                        Assert.AreEqual(8, menu.Items.Count);

                        TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuRunWaterLevelCalculationsIndex,
                                                                      "Alles be&rekenen",
                                                                      "Er is geen hydraulische belastingendatabase geïmporteerd.",
                                                                      RiskeerCommonFormsResources.CalculateAllIcon,
                                                                      false);

                        TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearIllustrationPointsIndex,
                                                                      "Wis alle illustratiepunten...",
                                                                      "Er zijn geen berekeningen met illustratiepunten om te wissen.",
                                                                      RiskeerCommonFormsResources.ClearIllustrationPointsIcon,
                                                                      false);
                    }
                }
            }

            // Assert
            mockRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_HydraulicBoundaryDatabaseNotLinked_ContextMenuItemCalculateAllDisabledAndTooltipSet()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mockRepository);

            var nodeData = new WaterLevelCalculationsForUserDefinedTargetProbabilityContext(new HydraulicBoundaryLocationCalculationsForTargetProbability(),
                                                                                            assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                IGui gui = StubFactory.CreateGuiStub(mockRepository);
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                mockRepository.ReplayAll();

                using (var plugin = new RiskeerPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    plugin.Gui = gui;

                    // Call
                    using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                    {
                        // Assert
                        ToolStripItem contextMenuItem = contextMenu.Items[contextMenuRunWaterLevelCalculationsIndex];

                        Assert.AreEqual("Alles be&rekenen", contextMenuItem.Text);
                        StringAssert.Contains("Er is geen hydraulische belastingendatabase geïmporteerd.", contextMenuItem.ToolTipText);
                        TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.CalculateAllIcon, contextMenuItem.Image);
                        Assert.IsFalse(contextMenuItem.Enabled);
                    }
                }
            }

            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void ContextMenuStrip_HydraulicBoundaryDatabaseLinkedToInvalidFile_ContextMenuItemCalculateAllDisabledAndTooltipSet()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(null, mockRepository, "invalidFilePath");

            var nodeData = new WaterLevelCalculationsForUserDefinedTargetProbabilityContext(new HydraulicBoundaryLocationCalculationsForTargetProbability(),
                                                                                            assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                IGui gui = StubFactory.CreateGuiStub(mockRepository);
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                mockRepository.ReplayAll();

                using (var plugin = new RiskeerPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    plugin.Gui = gui;

                    // Call
                    using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                    {
                        // Assert
                        ToolStripItem contextMenuItem = contextMenu.Items[contextMenuRunWaterLevelCalculationsIndex];

                        Assert.AreEqual("Alles be&rekenen", contextMenuItem.Text);
                        StringAssert.Contains("Herstellen van de verbinding met de hydraulische belastingendatabase is mislukt.", contextMenuItem.ToolTipText);
                        TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.CalculateAllIcon, contextMenuItem.Image);
                        Assert.IsFalse(contextMenuItem.Enabled);
                    }
                }
            }

            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void ContextMenuStrip_AllRequiredInputSet_ContextMenuItemCalculateAllEnabled()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, nameof(HydraulicBoundaryDatabase)), "complete.sqlite")
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(hydraulicBoundaryDatabase);

            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase);

            var nodeData = new WaterLevelCalculationsForUserDefinedTargetProbabilityContext(new HydraulicBoundaryLocationCalculationsForTargetProbability(),
                                                                                            assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                IGui gui = StubFactory.CreateGuiStub(mockRepository);
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                mockRepository.ReplayAll();

                using (var plugin = new RiskeerPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    plugin.Gui = gui;

                    // Call
                    using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                    {
                        // Assert
                        const string expectedItemText = @"Alles be&rekenen";
                        const string expectedItemTooltip = @"Alle waterstanden berekenen.";

                        TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuRunWaterLevelCalculationsIndex,
                                                                      expectedItemText, expectedItemTooltip, RiskeerCommonFormsResources.CalculateAllIcon);
                    }
                }
            }

            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void ContextMenuStrip_HydraulicBoundaryLocationCalculationsWithIllustrationPoints_ContextMenuItemClearAllIllustrationPointsEnabledAndTooltipSet()
        {
            // Setup
            var random = new Random(21);
            var calculationWithOutput = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
            {
                Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble())
            };
            var calculationWithIllustrationPoints = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
            {
                Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble(), new TestGeneralResultSubMechanismIllustrationPoint())
            };
            var calculationWithoutOutput = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation());

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mockRepository);

            var nodeData = new WaterLevelCalculationsForUserDefinedTargetProbabilityContext(new HydraulicBoundaryLocationCalculationsForTargetProbability
                                                                                            {
                                                                                                HydraulicBoundaryLocationCalculations =
                                                                                                {
                                                                                                    calculationWithOutput,
                                                                                                    calculationWithIllustrationPoints,
                                                                                                    calculationWithoutOutput
                                                                                                }
                                                                                            },
                                                                                            assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                IGui gui = StubFactory.CreateGuiStub(mockRepository);
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                mockRepository.ReplayAll();

                using (var plugin = new RiskeerPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    plugin.Gui = gui;

                    // Call
                    using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                    {
                        // Assert
                        ToolStripItem contextMenuItem = contextMenu.Items[contextMenuClearIllustrationPointsIndex];

                        Assert.AreEqual("Wis alle illustratiepunten...", contextMenuItem.Text);
                        Assert.AreEqual("Wis alle berekende illustratiepunten.", contextMenuItem.ToolTipText);
                        TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.ClearIllustrationPointsIcon, contextMenuItem.Image);
                        Assert.IsTrue(contextMenuItem.Enabled);
                    }
                }
            }

            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void ContextMenuStrip_HydraulicBoundaryLocationCalculationsWithoutIllustrationPoints_ContextMenuItemClearAllIllustrationPointsDisabled()
        {
            // Setup
            var random = new Random(21);
            var calculationWithOutput = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
            {
                Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble())
            };
            var calculationWithoutOutput = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation());

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(null, mockRepository, "invalidFilePath");

            var nodeData = new WaterLevelCalculationsForUserDefinedTargetProbabilityContext(new HydraulicBoundaryLocationCalculationsForTargetProbability
                                                                                            {
                                                                                                HydraulicBoundaryLocationCalculations =
                                                                                                {
                                                                                                    calculationWithOutput,
                                                                                                    calculationWithoutOutput
                                                                                                }
                                                                                            },
                                                                                            assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                IGui gui = StubFactory.CreateGuiStub(mockRepository);
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                mockRepository.ReplayAll();

                using (var plugin = new RiskeerPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    plugin.Gui = gui;

                    // Call
                    using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                    {
                        // Assert
                        ToolStripItem contextMenuItem = contextMenu.Items[contextMenuClearIllustrationPointsIndex];
                        Assert.IsFalse(contextMenuItem.Enabled);
                    }
                }
            }

            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void CalculateWaterLevelsFromContextMenu_HydraulicBoundaryDatabaseWithCanUsePreprocessorFalse_SendsRightInputToCalculationService()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite")
                }
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("locationName");

            const double targetProbability = 0.01;

            var context = new WaterLevelCalculationsForUserDefinedTargetProbabilityContext(new HydraulicBoundaryLocationCalculationsForTargetProbability
                                                                                           {
                                                                                               TargetProbability = targetProbability,
                                                                                               HydraulicBoundaryLocationCalculations =
                                                                                               {
                                                                                                   new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation)
                                                                                               }
                                                                                           },
                                                                                           assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                IMainWindow mainWindow = MainWindowTestHelper.CreateMainWindowStub(mockRepository);

                IGui gui = StubFactory.CreateGuiStub(mockRepository);
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.ProjectStore).Return(mockRepository.Stub<IStoreProject>());

                var designWaterLevelCalculator = new TestDesignWaterLevelCalculator();
                var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                                 .WhenCalled(invocation =>
                                 {
                                     HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                         HydraulicBoundaryCalculationSettingsFactory.CreateSettings(assessmentSection.HydraulicBoundaryDatabase),
                                         (HydraRingCalculationSettings) invocation.Arguments[0]);
                                 })
                                 .Return(designWaterLevelCalculator);
                mockRepository.ReplayAll();

                DialogBoxHandler = (name, wnd) =>
                {
                    // Expect an activity dialog which is automatically closed
                };

                using (var plugin = new RiskeerPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = gui;
                    plugin.Activate();

                    using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(context, null, treeViewControl))
                    using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                    {
                        // Call
                        contextMenuAdapter.Items[contextMenuRunWaterLevelCalculationsIndex].PerformClick();

                        // Assert
                        AssessmentLevelCalculationInput waterLevelCalculationInput = designWaterLevelCalculator.ReceivedInputs.First();

                        Assert.AreEqual(hydraulicBoundaryLocation.Id, waterLevelCalculationInput.HydraulicBoundaryLocationId);
                        Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(targetProbability), waterLevelCalculationInput.Beta);
                    }
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void CalculateWaterLevelsFromContextMenu_HydraulicBoundaryDatabaseWithUsePreprocessorTrue_SendsRightInputToCalculationService()
        {
            // Setup
            string preprocessorDirectory = TestHelper.GetScratchPadPath();
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = validFilePath,
                    HydraulicLocationConfigurationSettings =
                    {
                        CanUsePreprocessor = true,
                        UsePreprocessor = true,
                        PreprocessorDirectory = preprocessorDirectory
                    }
                }
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);

            const double targetProbability = 0.01;

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("locationName");

            var context = new WaterLevelCalculationsForUserDefinedTargetProbabilityContext(new HydraulicBoundaryLocationCalculationsForTargetProbability
                                                                                           {
                                                                                               TargetProbability = targetProbability,
                                                                                               HydraulicBoundaryLocationCalculations =
                                                                                               {
                                                                                                   new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation)
                                                                                               }
                                                                                           },
                                                                                           assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                IMainWindow mainWindow = MainWindowTestHelper.CreateMainWindowStub(mockRepository);

                IGui gui = StubFactory.CreateGuiStub(mockRepository);
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.ProjectStore).Return(mockRepository.Stub<IStoreProject>());

                var designWaterLevelCalculator = new TestDesignWaterLevelCalculator();
                var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                                 .WhenCalled(invocation =>
                                 {
                                     HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                         HydraulicBoundaryCalculationSettingsFactory.CreateSettings(assessmentSection.HydraulicBoundaryDatabase),
                                         (HydraRingCalculationSettings) invocation.Arguments[0]);
                                 })
                                 .Return(designWaterLevelCalculator);
                mockRepository.ReplayAll();

                DialogBoxHandler = (name, wnd) =>
                {
                    // Expect an activity dialog which is automatically closed
                };

                using (var plugin = new RiskeerPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = gui;
                    plugin.Activate();

                    using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(context, null, treeViewControl))
                    using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                    {
                        // Call
                        contextMenuAdapter.Items[contextMenuRunWaterLevelCalculationsIndex].PerformClick();

                        // Assert
                        AssessmentLevelCalculationInput waterLevelCalculationInput = designWaterLevelCalculator.ReceivedInputs.First();

                        Assert.AreEqual(hydraulicBoundaryLocation.Id, waterLevelCalculationInput.HydraulicBoundaryLocationId);
                        Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(targetProbability), waterLevelCalculationInput.Beta);
                    }
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void CalculateWaterLevelsFromContextMenu_HydraulicBoundaryDatabaseWithUsePreprocessorFalse_SendsRightInputToCalculationService()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = validFilePath,
                    HydraulicLocationConfigurationSettings =
                    {
                        CanUsePreprocessor = true,
                        UsePreprocessor = false,
                        PreprocessorDirectory = "InvalidPreprocessorDirectory"
                    }
                }
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("locationName");

            const double targetProbability = 0.01;

            var context = new WaterLevelCalculationsForUserDefinedTargetProbabilityContext(new HydraulicBoundaryLocationCalculationsForTargetProbability
                                                                                           {
                                                                                               TargetProbability = targetProbability,
                                                                                               HydraulicBoundaryLocationCalculations =
                                                                                               {
                                                                                                   new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation)
                                                                                               }
                                                                                           },
                                                                                           assessmentSection);
            using (var treeViewControl = new TreeViewControl())
            {
                IMainWindow mainWindow = MainWindowTestHelper.CreateMainWindowStub(mockRepository);

                IGui gui = StubFactory.CreateGuiStub(mockRepository);
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.ProjectStore).Return(mockRepository.Stub<IStoreProject>());

                var designWaterLevelCalculator = new TestDesignWaterLevelCalculator();
                var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                                 .WhenCalled(invocation =>
                                 {
                                     HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                         HydraulicBoundaryCalculationSettingsFactory.CreateSettings(assessmentSection.HydraulicBoundaryDatabase),
                                         (HydraRingCalculationSettings) invocation.Arguments[0]);
                                 })
                                 .Return(designWaterLevelCalculator);
                mockRepository.ReplayAll();

                DialogBoxHandler = (name, wnd) =>
                {
                    // Expect an activity dialog which is automatically closed
                };

                using (var plugin = new RiskeerPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = gui;
                    plugin.Activate();

                    using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(context, null, treeViewControl))
                    using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                    {
                        // Call
                        contextMenuAdapter.Items[contextMenuRunWaterLevelCalculationsIndex].PerformClick();

                        // Assert
                        AssessmentLevelCalculationInput waterLevelCalculationInput = designWaterLevelCalculator.ReceivedInputs.First();

                        Assert.AreEqual(hydraulicBoundaryLocation.Id, waterLevelCalculationInput.HydraulicBoundaryLocationId);
                        Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(targetProbability), waterLevelCalculationInput.Beta);
                    }
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenValidCalculations_WhenCalculatingAllFromContextMenu_ThenLogMessagesAddedOutputSet()
        {
            // Given
            const string locationName = "locationName";
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = validFilePath
                }
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);

            var calculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation("locationName"));

            var context = new WaterLevelCalculationsForUserDefinedTargetProbabilityContext(new HydraulicBoundaryLocationCalculationsForTargetProbability
                                                                                           {
                                                                                               TargetProbability = 0.01,
                                                                                               HydraulicBoundaryLocationCalculations =
                                                                                               {
                                                                                                   calculation
                                                                                               }
                                                                                           },
                                                                                           assessmentSection);
            using (var treeViewControl = new TreeViewControl())
            {
                IMainWindow mainWindow = MainWindowTestHelper.CreateMainWindowStub(mockRepository);

                IGui gui = StubFactory.CreateGuiStub(mockRepository);
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.ProjectStore).Return(mockRepository.Stub<IStoreProject>());

                var calculator = new TestDesignWaterLevelCalculator
                {
                    Converged = false
                };
                var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(null))
                                 .IgnoreArguments()
                                 .Return(calculator);
                mockRepository.ReplayAll();

                DialogBoxHandler = (name, wnd) =>
                {
                    // Expect an activity dialog which is automatically closed
                };

                using (var plugin = new RiskeerPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = gui;
                    plugin.Activate();

                    using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(context, null, treeViewControl))
                    using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                    {
                        // When
                        void Call() => contextMenuAdapter.Items[contextMenuRunWaterLevelCalculationsIndex].PerformClick();

                        // Then
                        TestHelper.AssertLogMessages(Call, messages =>
                        {
                            string[] msgs = messages.ToArray();
                            Assert.AreEqual(8, msgs.Length);
                            Assert.AreEqual($"Waterstand berekenen voor locatie '{locationName}' (1/100) is gestart.", msgs[0]);
                            CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                            CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                            CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[3]);
                            Assert.AreEqual($"Waterstand berekening voor locatie '{locationName}' (1/100) is niet geconvergeerd.", msgs[4]);
                            StringAssert.StartsWith("Waterstand berekening is uitgevoerd op de tijdelijke locatie", msgs[5]);
                            CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[6]);
                            Assert.AreEqual($"Waterstand berekenen voor locatie '{locationName}' (1/100) is gelukt.", msgs[7]);
                        });

                        HydraulicBoundaryLocationCalculationOutput output = calculation.Output;
                        Assert.AreEqual(calculator.DesignWaterLevel, output.Result, output.Result.GetAccuracy());
                        Assert.AreEqual(CalculationConvergence.CalculatedNotConverged, output.CalculationConvergence);
                    }
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        [Apartment(ApartmentState.STA)]
        public void GivenCalculationWithIllustrationPoints_WhenClearIllustrationPointsClicked_ThenInquiryAndExpectedOutputAndCalculationObserversNotified(bool continuation)
        {
            // Given
            var random = new Random(21);
            var calculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
            {
                Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble(), new TestGeneralResultSubMechanismIllustrationPoint())
            };

            var calculationObserver = mockRepository.StrictMock<IObserver>();

            if (continuation)
            {
                calculationObserver.Expect(o => o.UpdateObserver());
            }

            calculation.Attach(calculationObserver);

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(null, mockRepository, "invalidFilePath");

            var nodeData = new WaterLevelCalculationsForUserDefinedTargetProbabilityContext(new HydraulicBoundaryLocationCalculationsForTargetProbability
                                                                                            {
                                                                                                TargetProbability = 0.01,
                                                                                                HydraulicBoundaryLocationCalculations =
                                                                                                {
                                                                                                    calculation
                                                                                                }
                                                                                            },
                                                                                            assessmentSection);
            var messageBoxText = "";
            DialogBoxHandler = (name, wnd) =>
            {
                var helper = new MessageBoxTester(wnd);
                messageBoxText = helper.Text;

                if (continuation)
                {
                    helper.ClickOk();
                }
                else
                {
                    helper.ClickCancel();
                }
            };

            using (var treeViewControl = new TreeViewControl())
            {
                IGui gui = StubFactory.CreateGuiStub(mockRepository);
                gui.Stub(cmp => cmp.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mockRepository.ReplayAll();

                using (var plugin = new RiskeerPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = gui;

                    using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                    {
                        // When
                        contextMenu.Items[contextMenuClearIllustrationPointsIndex].PerformClick();

                        // Then
                        string expectedMessage = "Weet u zeker dat u alle berekende illustratiepunten bij '1/100' wilt wissen?";
                        Assert.AreEqual(expectedMessage, messageBoxText);

                        Assert.IsTrue(calculation.HasOutput);
                        Assert.AreEqual(!continuation, calculation.Output.HasGeneralResult);
                    }
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void CanDrag_Always_ReturnsTrue()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                bool canDrag = info.CanDrag(null, null);

                // Assert
                Assert.IsTrue(canDrag);
            }
        }

        public override void Setup()
        {
            mockRepository = new MockRepository();
        }

        private static TreeNodeInfo GetInfo(RiskeerPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(WaterLevelCalculationsForUserDefinedTargetProbabilityContext));
        }
    }
}