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
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Service.TestUtil;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Data.Input.Hydraulics;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.GrassCoverErosionOutwards.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class GrassCoverErosionOutwardsDesignWaterLevelCalculationsContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuRunDesignWaterLevelCalculationsIndex = 2;
        private const int contextMenuClearIllustrationPointsIndex = 4;
        private MockRepository mockRepository;

        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Service, "HydraRingCalculation");
        private static readonly string validFilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Assert
                Assert.IsNotNull(info.Text);
                Assert.IsNull(info.ForeColor);
                Assert.IsNotNull(info.Image);
                Assert.IsNotNull(info.ContextMenuStrip);
                Assert.IsNull(info.EnsureVisibleOnCreate);
                Assert.IsNull(info.ExpandOnCreate);
                Assert.IsNull(info.ChildNodeObjects);
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
        }

        [Test]
        public void Text_Always_ReturnCategoryBoundaryName()
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            const string categoryBoundaryName = "A";
            var context = new GrassCoverErosionOutwardsDesignWaterLevelCalculationsContext(new ObservableList<HydraulicBoundaryLocationCalculation>(),
                                                                                           new GrassCoverErosionOutwardsFailureMechanism(),
                                                                                           assessmentSection,
                                                                                           () => 0.01,
                                                                                           categoryBoundaryName);

            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                string nodeText = info.Text(context);

                // Assert
                Assert.AreEqual($"Categoriegrens {categoryBoundaryName}", nodeText);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Image_Always_ReturnGenericInputOutputIcon()
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();
            var context = new GrassCoverErosionOutwardsDesignWaterLevelCalculationsContext(new ObservableList<HydraulicBoundaryLocationCalculation>(),
                                                                                           new GrassCoverErosionOutwardsFailureMechanism(),
                                                                                           assessmentSection,
                                                                                           () => 0.01,
                                                                                           "A");
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Image icon = info.Image(context);

                // Assert
                TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.GenericInputOutputIcon, icon);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mockRepository);

            using (var treeViewControl = new TreeViewControl())
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                var context = new GrassCoverErosionOutwardsDesignWaterLevelCalculationsContext(new ObservableList<HydraulicBoundaryLocationCalculation>(),
                                                                                               new GrassCoverErosionOutwardsFailureMechanism(),
                                                                                               assessmentSection,
                                                                                               () => 0.01,
                                                                                               "A");

                var menuBuilder = mockRepository.StrictMock<IContextMenuBuilder>();
                using (mockRepository.Ordered())
                {
                    menuBuilder.Expect(mb => mb.AddOpenItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.Build()).Return(null);
                }

                var gui = mockRepository.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                mockRepository.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (info.ContextMenuStrip(context, null, treeViewControl)) {}
            }

            // Assert
            mockRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_HydraulicBoundaryDatabaseLinkedToInvalidFile_ContextMenuItemCalculateAllDisabledAndTooltipSet()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(null, mockRepository, "invalidFilePath");

            var applicationFeatureCommandHandler = mockRepository.Stub<IApplicationFeatureCommands>();
            var importCommandHandler = mockRepository.Stub<IImportCommandHandler>();
            var exportCommandHandler = mockRepository.Stub<IExportCommandHandler>();
            var updateCommandHandler = mockRepository.Stub<IUpdateCommandHandler>();
            var viewCommandsHandler = mockRepository.Stub<IViewCommands>();

            viewCommandsHandler.Stub(vch => vch.CanOpenViewFor(null)).IgnoreArguments().Return(true);

            using (var treeViewControl = new TreeViewControl())
            {
                using (var plugin = new GrassCoverErosionOutwardsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    var context = new GrassCoverErosionOutwardsDesignWaterLevelCalculationsContext(new ObservableList<HydraulicBoundaryLocationCalculation>(),
                                                                                                   new GrassCoverErosionOutwardsFailureMechanism
                                                                                                   {
                                                                                                       Contribution = 5
                                                                                                   }, assessmentSection,
                                                                                                   () => 0.01,
                                                                                                   "A");

                    var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler,
                                                             importCommandHandler,
                                                             exportCommandHandler,
                                                             updateCommandHandler,
                                                             viewCommandsHandler,
                                                             context,
                                                             treeViewControl);

                    var gui = mockRepository.Stub<IGui>();
                    gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(menuBuilder);
                    gui.Stub(g => g.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                    mockRepository.ReplayAll();

                    plugin.Gui = gui;

                    // Call
                    using (ContextMenuStrip menu = info.ContextMenuStrip(context, null, treeViewControl))
                    {
                        // Assert
                        ToolStripItem contextMenuItem = menu.Items[contextMenuRunDesignWaterLevelCalculationsIndex];

                        Assert.AreEqual("Alles be&rekenen", contextMenuItem.Text);
                        StringAssert.Contains("Herstellen van de verbinding met de hydraulische belastingendatabase is mislukt.", contextMenuItem.ToolTipText);
                        TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.CalculateAllIcon, contextMenuItem.Image);
                        Assert.IsFalse(contextMenuItem.Enabled);
                    }
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_InvalidNorm_ContextMenuItemCalculateAllDisabledAndTooltipSet()
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            var applicationFeatureCommandHandler = mockRepository.Stub<IApplicationFeatureCommands>();
            var importCommandHandler = mockRepository.Stub<IImportCommandHandler>();
            var exportCommandHandler = mockRepository.Stub<IExportCommandHandler>();
            var updateCommandHandler = mockRepository.Stub<IUpdateCommandHandler>();
            var viewCommandsHandler = mockRepository.Stub<IViewCommands>();

            viewCommandsHandler.Stub(vch => vch.CanOpenViewFor(null)).IgnoreArguments().Return(true);
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
            {
                FilePath = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, nameof(HydraulicBoundaryDatabase)), "complete.sqlite"),
                Version = "1.0"
            });

            using (var treeViewControl = new TreeViewControl())
            {
                using (var plugin = new GrassCoverErosionOutwardsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    var context = new GrassCoverErosionOutwardsDesignWaterLevelCalculationsContext(new ObservableList<HydraulicBoundaryLocationCalculation>(),
                                                                                                   new GrassCoverErosionOutwardsFailureMechanism
                                                                                                   {
                                                                                                       Contribution = 5
                                                                                                   }, assessmentSection,
                                                                                                   () => 1.0,
                                                                                                   "A");

                    var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler,
                                                             importCommandHandler,
                                                             exportCommandHandler,
                                                             updateCommandHandler,
                                                             viewCommandsHandler,
                                                             context,
                                                             treeViewControl);

                    var gui = mockRepository.Stub<IGui>();
                    gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(menuBuilder);
                    gui.Stub(g => g.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                    mockRepository.ReplayAll();

                    plugin.Gui = gui;

                    // Call
                    using (ContextMenuStrip menu = info.ContextMenuStrip(context, null, treeViewControl))
                    {
                        // Assert
                        TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                      contextMenuRunDesignWaterLevelCalculationsIndex,
                                                                      "Alles be&rekenen",
                                                                      "Doelkans is te groot om een berekening uit te kunnen voeren.",
                                                                      RiskeerCommonFormsResources.CalculateAllIcon,
                                                                      false);
                    }
                }
            }

            mockRepository.VerifyAll();
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

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(null, mockRepository);

            var nodeData = new GrassCoverErosionOutwardsDesignWaterLevelCalculationsContext(new ObservableList<HydraulicBoundaryLocationCalculation>
                                                                                            {
                                                                                                calculationWithOutput,
                                                                                                calculationWithIllustrationPoints,
                                                                                                calculationWithoutOutput
                                                                                            },
                                                                                            new GrassCoverErosionOutwardsFailureMechanism
                                                                                            {
                                                                                                Contribution = 5
                                                                                            }, assessmentSection,
                                                                                            () => 0.01,
                                                                                            "A");

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                mockRepository.ReplayAll();

                using (var plugin = new GrassCoverErosionOutwardsPlugin())
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

            var nodeData = new GrassCoverErosionOutwardsDesignWaterLevelCalculationsContext(new ObservableList<HydraulicBoundaryLocationCalculation>
                                                                                            {
                                                                                                calculationWithOutput,
                                                                                                calculationWithoutOutput
                                                                                            },
                                                                                            new GrassCoverErosionOutwardsFailureMechanism
                                                                                            {
                                                                                                Contribution = 5
                                                                                            }, assessmentSection,
                                                                                            () => 0.01,
                                                                                            "A");

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                mockRepository.ReplayAll();

                using (var plugin = new GrassCoverErosionOutwardsPlugin())
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
        public void ContextMenuStrip_AllRequiredInputSet_ContextMenuItemCalculateAllEnabled()
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            var applicationFeatureCommandHandler = mockRepository.Stub<IApplicationFeatureCommands>();
            var importCommandHandler = mockRepository.Stub<IImportCommandHandler>();
            var exportCommandHandler = mockRepository.Stub<IExportCommandHandler>();
            var updateCommandHandler = mockRepository.Stub<IUpdateCommandHandler>();
            var viewCommandsHandler = mockRepository.Stub<IViewCommands>();

            viewCommandsHandler.Stub(vch => vch.CanOpenViewFor(null)).IgnoreArguments().Return(true);
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
            {
                FilePath = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, nameof(HydraulicBoundaryDatabase)), "complete.sqlite"),
                Version = "1.0"
            });

            using (var treeViewControl = new TreeViewControl())
            {
                using (var plugin = new GrassCoverErosionOutwardsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    var context = new GrassCoverErosionOutwardsDesignWaterLevelCalculationsContext(new ObservableList<HydraulicBoundaryLocationCalculation>(),
                                                                                                   new GrassCoverErosionOutwardsFailureMechanism
                                                                                                   {
                                                                                                       Contribution = 5
                                                                                                   }, assessmentSection,
                                                                                                   () => 0.01,
                                                                                                   "A");

                    var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler,
                                                             importCommandHandler,
                                                             exportCommandHandler,
                                                             updateCommandHandler,
                                                             viewCommandsHandler,
                                                             context,
                                                             treeViewControl);

                    var gui = mockRepository.Stub<IGui>();
                    gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(menuBuilder);
                    gui.Stub(g => g.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                    mockRepository.ReplayAll();

                    plugin.Gui = gui;

                    // Call
                    using (ContextMenuStrip menu = info.ContextMenuStrip(context, null, treeViewControl))
                    {
                        // Assert
                        TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                      contextMenuRunDesignWaterLevelCalculationsIndex,
                                                                      "Alles be&rekenen",
                                                                      "Alle waterstanden berekenen.",
                                                                      RiskeerCommonFormsResources.CalculateAllIcon);
                    }
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateDesignWaterLevelsFromContextMenu_HydraulicBoundaryDatabaseWithCanUsePreprocessorFalse_SendsRightInputToCalculationService()
        {
            // Setup
            const double norm = 0.01;
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 5
            };

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mockRepository, validFilePath);

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1300001, string.Empty, 0, 0);

            var context = new GrassCoverErosionOutwardsDesignWaterLevelCalculationsContext(
                new ObservableList<HydraulicBoundaryLocationCalculation>
                {
                    new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation)
                }, failureMechanism, assessmentSection,
                () => norm,
                "A");

            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mockRepository.Stub<IMainWindow>());

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

                using (var plugin = new GrassCoverErosionOutwardsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = gui;
                    plugin.Activate();

                    using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(context, null, treeViewControl))
                    using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                    {
                        // Call
                        contextMenuAdapter.Items[contextMenuRunDesignWaterLevelCalculationsIndex].PerformClick();

                        // Assert
                        AssessmentLevelCalculationInput designWaterLevelCalculationInput = designWaterLevelCalculator.ReceivedInputs.First();

                        Assert.AreEqual(hydraulicBoundaryLocation.Id, designWaterLevelCalculationInput.HydraulicBoundaryLocationId);
                        Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(norm), designWaterLevelCalculationInput.Beta);
                    }
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateDesignWaterLevelsFromContextMenu_HydraulicBoundaryDatabaseWithUsePreprocessorTrue_SendsRightInputToCalculationService()
        {
            // Setup
            const double norm = 0.01;
            string preprocessorDirectory = TestHelper.GetScratchPadPath();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 5
            };

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mockRepository, validFilePath);

            assessmentSection.HydraulicBoundaryDatabase.CanUsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.UsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.PreprocessorDirectory = preprocessorDirectory;

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1300001, string.Empty, 0, 0);

            var context = new GrassCoverErosionOutwardsDesignWaterLevelCalculationsContext(
                new ObservableList<HydraulicBoundaryLocationCalculation>
                {
                    new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation)
                }, failureMechanism, assessmentSection,
                () => norm,
                "A");

            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mockRepository.Stub<IMainWindow>());

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

                using (var plugin = new GrassCoverErosionOutwardsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = gui;
                    plugin.Activate();

                    using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(context, null, treeViewControl))
                    using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                    {
                        // Call
                        contextMenuAdapter.Items[contextMenuRunDesignWaterLevelCalculationsIndex].PerformClick();

                        // Assert
                        AssessmentLevelCalculationInput designWaterLevelCalculationInput = designWaterLevelCalculator.ReceivedInputs.First();

                        Assert.AreEqual(hydraulicBoundaryLocation.Id, designWaterLevelCalculationInput.HydraulicBoundaryLocationId);
                        Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(norm), designWaterLevelCalculationInput.Beta);
                    }
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateDesignWaterLevelsFromContextMenu_HydraulicBoundaryDatabaseWithUsePreprocessorFalse_SendsRightInputToCalculationService()
        {
            // Setup
            const double norm = 0.01;
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 5
            };

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mockRepository, validFilePath);

            assessmentSection.HydraulicBoundaryDatabase.CanUsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.UsePreprocessor = false;
            assessmentSection.HydraulicBoundaryDatabase.PreprocessorDirectory = "InvalidPreprocessorDirectory";

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1300001, string.Empty, 0, 0);

            var context = new GrassCoverErosionOutwardsDesignWaterLevelCalculationsContext(
                new ObservableList<HydraulicBoundaryLocationCalculation>
                {
                    new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation)
                }, failureMechanism, assessmentSection,
                () => norm,
                "A");

            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mockRepository.Stub<IMainWindow>());

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

                using (var plugin = new GrassCoverErosionOutwardsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = gui;
                    plugin.Activate();

                    using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(context, null, treeViewControl))
                    using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                    {
                        // Call
                        contextMenuAdapter.Items[contextMenuRunDesignWaterLevelCalculationsIndex].PerformClick();

                        // Assert
                        AssessmentLevelCalculationInput designWaterLevelCalculationInput = designWaterLevelCalculator.ReceivedInputs.First();

                        Assert.AreEqual(hydraulicBoundaryLocation.Id, designWaterLevelCalculationInput.HydraulicBoundaryLocationId);
                        Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(norm), designWaterLevelCalculationInput.Beta);
                    }
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenHydraulicBoundaryLocationSucceeds_CalculatingDesignWaterLevelFromContextMenu_ThenLogMessagesAddedOutputSet()
        {
            // Given
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 5
            };
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mockRepository, validFilePath);

            HydraulicBoundaryLocation hydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations[0];
            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation);

            const string categoryBoundaryName = "A";
            var context = new GrassCoverErosionOutwardsDesignWaterLevelCalculationsContext(
                new ObservableList<HydraulicBoundaryLocationCalculation>
                {
                    hydraulicBoundaryLocationCalculation
                }, failureMechanism, assessmentSection,
                () => 0.01,
                categoryBoundaryName);

            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mockRepository.Stub<IMainWindow>());

                var designWaterLevelCalculator = new TestDesignWaterLevelCalculator
                {
                    Converged = false
                };
                var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(null))
                                 .IgnoreArguments()
                                 .Return(designWaterLevelCalculator);
                mockRepository.ReplayAll();

                using (var plugin = new GrassCoverErosionOutwardsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = gui;
                    plugin.Activate();

                    using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(context, null, treeViewControl))
                    using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                    {
                        // When
                        Action call = () => contextMenuAdapter.Items[contextMenuRunDesignWaterLevelCalculationsIndex].PerformClick();

                        // Then
                        TestHelper.AssertLogMessages(call, messages =>
                        {
                            string[] msgs = messages.ToArray();
                            Assert.AreEqual(8, msgs.Length);
                            Assert.AreEqual($"Waterstand berekenen voor locatie '{hydraulicBoundaryLocation.Name}' (Categoriegrens {categoryBoundaryName}) is gestart.", msgs[0]);
                            CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                            CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                            CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[3]);
                            Assert.AreEqual($"Waterstand berekening voor locatie '{hydraulicBoundaryLocation.Name}' (Categoriegrens {categoryBoundaryName}) is niet geconvergeerd.", msgs[4]);
                            StringAssert.StartsWith("Waterstand berekening is uitgevoerd op de tijdelijke locatie", msgs[5]);
                            CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[6]);
                            Assert.AreEqual($"Waterstand berekenen voor locatie '{hydraulicBoundaryLocation.Name}' (Categoriegrens {categoryBoundaryName}) is gelukt.", msgs[7]);
                        });

                        HydraulicBoundaryLocationCalculationOutput output = hydraulicBoundaryLocationCalculation.Output;
                        Assert.AreEqual(0, output.Result, output.Result.GetAccuracy());
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

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(null, mockRepository);
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 5
            };

            const string categoryBoundaryName = "A";
            var nodeData = new GrassCoverErosionOutwardsDesignWaterLevelCalculationsContext(new ObservableList<HydraulicBoundaryLocationCalculation>
                                                                                            {
                                                                                                calculation
                                                                                            }, failureMechanism, assessmentSection,
                                                                                            () => 0.01,
                                                                                            categoryBoundaryName);

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
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mockRepository.ReplayAll();

                using (var plugin = new GrassCoverErosionOutwardsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = gui;

                    using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                    {
                        // When
                        contextMenu.Items[contextMenuClearIllustrationPointsIndex].PerformClick();

                        // Then
                        string expectedMessage = $"Weet u zeker dat u alle berekende illustratiepunten bij 'Categoriegrens {categoryBoundaryName}' wilt wissen?";
                        Assert.AreEqual(expectedMessage, messageBoxText);

                        Assert.IsTrue(calculation.HasOutput);
                        Assert.AreEqual(!continuation, calculation.Output.HasGeneralResult);
                    }
                }
            }

            mockRepository.VerifyAll();
        }

        public override void Setup()
        {
            mockRepository = new MockRepository();
        }

        private static TreeNodeInfo GetInfo(GrassCoverErosionOutwardsPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(GrassCoverErosionOutwardsDesignWaterLevelCalculationsContext));
        }
    }
}