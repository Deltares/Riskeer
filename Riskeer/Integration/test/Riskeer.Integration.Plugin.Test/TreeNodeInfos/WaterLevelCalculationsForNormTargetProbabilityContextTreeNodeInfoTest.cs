﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Core.Gui.Forms.ViewHost;
using Core.Gui.TestUtil;
using Core.Gui.TestUtil.ContextMenu;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Plugin.TestUtil;
using Riskeer.Common.Service.TestUtil;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Data.Input.Hydraulics;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;
using Riskeer.Integration.Data;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class WaterLevelCalculationsForNormTargetProbabilityContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuRunWaterLevelCalculationsIndex = 4;
        private const int contextMenuClearIllustrationPointsIndex = 6;

        private MockRepository mockRepository;

        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Service, "HydraRingCalculation");
        private static readonly string validHlcdFilePath = Path.Combine(testDataPath, "hlcd.sqlite");
        private static readonly string validHrdFilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");
        private static readonly string validHrdFileVersion = "IJssel lake2016-07-04 16:187";

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
        [TestCase(0.1, 0.01, "1/10")]
        [TestCase(0.1, 0.1, "1/10")]
        public void Text_WithContext_ReturnsUniquelyFormattedTargetProbabilityForMaximumAllowableFloodingProbability(
            double maximumAllowableFloodingProbability, double signalFloodingProbability, string expectedText)
        {
            // Setup
            var maximumAllowableFloodingProbabilityCalculations = new ObservableList<HydraulicBoundaryLocationCalculation>();

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mockRepository);

            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(new FailureMechanismContribution(maximumAllowableFloodingProbability, signalFloodingProbability));
            assessmentSection.Stub(a => a.WaterLevelCalculationsForMaximumAllowableFloodingProbability).Return(maximumAllowableFloodingProbabilityCalculations);
            assessmentSection.Stub(a => a.WaterLevelCalculationsForSignalFloodingProbability).Return(new ObservableList<HydraulicBoundaryLocationCalculation>());
            assessmentSection.Stub(a => a.WaterLevelCalculationsForUserDefinedTargetProbabilities).Return(new ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>());

            mockRepository.ReplayAll();

            var context = new WaterLevelCalculationsForNormTargetProbabilityContext(maximumAllowableFloodingProbabilityCalculations, assessmentSection, () => maximumAllowableFloodingProbability);

            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                string text = info.Text(context);

                // Assert
                Assert.AreEqual(expectedText, text);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(0.1, 0.01, "1/100")]
        [TestCase(0.1, 0.1, "1/10 (1)")]
        public void Text_WithContext_ReturnsUniquelyFormattedTargetProbabilityForSignalFloodingProbability(
            double maximumAllowableFloodingProbability, double signalFloodingProbability, string expectedText)
        {
            // Setup
            var signalFloodingProbabilityCalculations = new ObservableList<HydraulicBoundaryLocationCalculation>();

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mockRepository);

            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(new FailureMechanismContribution(maximumAllowableFloodingProbability, signalFloodingProbability));
            assessmentSection.Stub(a => a.WaterLevelCalculationsForMaximumAllowableFloodingProbability).Return(new ObservableList<HydraulicBoundaryLocationCalculation>());
            assessmentSection.Stub(a => a.WaterLevelCalculationsForSignalFloodingProbability).Return(signalFloodingProbabilityCalculations);
            assessmentSection.Stub(a => a.WaterLevelCalculationsForUserDefinedTargetProbabilities).Return(new ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>());

            mockRepository.ReplayAll();

            var context = new WaterLevelCalculationsForNormTargetProbabilityContext(signalFloodingProbabilityCalculations, assessmentSection, () => maximumAllowableFloodingProbability);

            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                string text = info.Text(context);

                // Assert
                Assert.AreEqual(expectedText, text);
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
        [TestCaseSource(typeof(WaterLevelCalculationsForNormTargetProbabilityContextTreeNodeInfoTest), nameof(GetWaterLevelForNormTargetProbabilityCalculations), new object[]
        {
            "ContextMenuStrip_Always_CallsContextMenuBuilderMethods_{0}"
        })]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods(
            Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>> getCalculationsFunc)
        {
            // Setup
            IAssessmentSection assessmentSection = new AssessmentSectionStub();

            var menuBuilder = mockRepository.StrictMock<IContextMenuBuilder>();
            using (mockRepository.Ordered())
            {
                menuBuilder.Expect(mb => mb.AddOpenItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddExportItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.Build()).Return(null);
            }

            var nodeData = new WaterLevelCalculationsForNormTargetProbabilityContext(getCalculationsFunc(assessmentSection),
                                                                                     assessmentSection,
                                                                                     () => 0.01);

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
        [TestCaseSource(typeof(WaterLevelCalculationsForNormTargetProbabilityContextTreeNodeInfoTest), nameof(GetWaterLevelForNormTargetProbabilityCalculations), new object[]
        {
            "ContextMenuStrip_Always_AddCustomItems_{0}"
        })]
        public void ContextMenuStrip_Always_AddCustomItems(
            Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>> getCalculationsFunc)
        {
            // Setup
            IAssessmentSection assessmentSection = new AssessmentSectionStub();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            var nodeData = new WaterLevelCalculationsForNormTargetProbabilityContext(getCalculationsFunc(assessmentSection),
                                                                                     assessmentSection,
                                                                                     () => 0.01);

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
                        Assert.AreEqual(9, menu.Items.Count);

                        TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuRunWaterLevelCalculationsIndex,
                                                                      "Alles be&rekenen",
                                                                      "Alle waterstanden berekenen.",
                                                                      RiskeerCommonFormsResources.CalculateAllIcon);

                        TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearIllustrationPointsIndex,
                                                                      "Wis alle &illustratiepunten...",
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
        [TestCaseSource(typeof(WaterLevelCalculationsForNormTargetProbabilityContextTreeNodeInfoTest), nameof(GetWaterLevelForNormTargetProbabilityCalculations), new object[]
        {
            "ContextMenuStrip_AllRequiredInputSet_ContextMenuItemCalculateAllEnabled_{0}"
        })]
        public void ContextMenuStrip_AllRequiredInputSet_ContextMenuItemCalculateAllEnabled(
            Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>> getCalculationsFunc)
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();

            var nodeData = new WaterLevelCalculationsForNormTargetProbabilityContext(getCalculationsFunc(assessmentSection),
                                                                                     assessmentSection,
                                                                                     () => 0.01);

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
                        const string expectedItemText = "Alles be&rekenen";
                        const string expectedItemTooltip = "Alle waterstanden berekenen.";

                        TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuRunWaterLevelCalculationsIndex,
                                                                      expectedItemText, expectedItemTooltip, RiskeerCommonFormsResources.CalculateAllIcon);
                    }
                }
            }

            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        [TestCaseSource(typeof(WaterLevelCalculationsForNormTargetProbabilityContextTreeNodeInfoTest), nameof(GetWaterLevelForNormTargetProbabilityCalculations), new object[]
        {
            "ContextMenuStrip_HydraulicBoundaryLocationCalculationsWithIllustrationPoints_ContextMenuItemClearAllIllustrationPointsEnabledAndTooltipSet_{0}"
        })]
        public void ContextMenuStrip_HydraulicBoundaryLocationCalculationsWithIllustrationPoints_ContextMenuItemClearAllIllustrationPointsEnabledAndTooltipSet(
            Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>> getCalculationsFunc)
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.AddHydraulicBoundaryLocationCalculations(new[]
            {
                new TestHydraulicBoundaryLocation(),
                new TestHydraulicBoundaryLocation(),
                new TestHydraulicBoundaryLocation()
            });

            IObservableEnumerable<HydraulicBoundaryLocationCalculation> calculations = getCalculationsFunc(assessmentSection);
            HydraulicBoundaryLocationCalculation calculationWithOutput = calculations.ElementAt(0);
            calculationWithOutput.Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble());

            HydraulicBoundaryLocationCalculation calculationWithIllustrationPoints = calculations.ElementAt(1);
            calculationWithIllustrationPoints.Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble(), new TestGeneralResultSubMechanismIllustrationPoint());

            var nodeData = new WaterLevelCalculationsForNormTargetProbabilityContext(calculations,
                                                                                     assessmentSection,
                                                                                     () => 0.01);

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

                        Assert.AreEqual("Wis alle &illustratiepunten...", contextMenuItem.Text);
                        Assert.AreEqual("Wis alle berekende illustratiepunten.", contextMenuItem.ToolTipText);
                        TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.ClearIllustrationPointsIcon, contextMenuItem.Image);
                        Assert.IsTrue(contextMenuItem.Enabled);
                    }
                }
            }

            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        [TestCaseSource(typeof(WaterLevelCalculationsForNormTargetProbabilityContextTreeNodeInfoTest), nameof(GetWaterLevelForNormTargetProbabilityCalculations), new object[]
        {
            "ContextMenuStrip_HydraulicBoundaryLocationCalculationsWithoutIllustrationPoints_ContextMenuItemClearAllIllustrationPointsDisabled_{0}"
        })]
        public void ContextMenuStrip_HydraulicBoundaryLocationCalculationsWithoutIllustrationPoints_ContextMenuItemClearAllIllustrationPointsDisabled(
            Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>> getCalculationsFunc)
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.AddHydraulicBoundaryLocationCalculations(new[]
            {
                new TestHydraulicBoundaryLocation(),
                new TestHydraulicBoundaryLocation()
            });

            IObservableEnumerable<HydraulicBoundaryLocationCalculation> calculations = assessmentSection.WaterLevelCalculationsForMaximumAllowableFloodingProbability;
            HydraulicBoundaryLocationCalculation calculation = calculations.First();
            calculation.Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble());

            var nodeData = new WaterLevelCalculationsForNormTargetProbabilityContext(getCalculationsFunc(assessmentSection),
                                                                                     assessmentSection,
                                                                                     () => 0.01);

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
        [TestCaseSource(typeof(WaterLevelCalculationsForNormTargetProbabilityContextTreeNodeInfoTest), nameof(GetWaterLevelForNormTargetProbabilityCalculations), new object[]
        {
            "CalculateWaterLevelsFromContextMenu_HydraulicBoundaryDatabaseWithCanUsePreprocessorFalse_SendsRightInputToCalculationService_{0}"
        })]
        [Apartment(ApartmentState.STA)]
        public void CalculateWaterLevelsFromContextMenu_HydraulicBoundaryDatabaseWithUsePreprocessorClosureFalse_SendsRightInputToCalculationService(
            Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>> getCalculationsFunc)
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("locationName");
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryData =
                {
                    HydraulicLocationConfigurationDatabase =
                    {
                        FilePath = validHlcdFilePath
                    },
                    HydraulicBoundaryDatabases =
                    {
                        new HydraulicBoundaryDatabase
                        {
                            FilePath = validHrdFilePath,
                            Version = validHrdFileVersion,
                            Locations =
                            {
                                hydraulicBoundaryLocation
                            }
                        }
                    }
                }
            };
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            double GetNormFunc() => 0.01;
            var context = new WaterLevelCalculationsForNormTargetProbabilityContext(getCalculationsFunc(assessmentSection),
                                                                                    assessmentSection,
                                                                                    GetNormFunc);

            using (var treeViewControl = new TreeViewControl())
            {
                IMainWindow mainWindow = MainWindowTestHelper.CreateMainWindowStub(mockRepository);

                IGui gui = StubFactory.CreateGuiStub(mockRepository);
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.ProjectStore).Return(mockRepository.Stub<IStoreProject>());
                gui.Stub(g => g.DocumentViewController).Return(mockRepository.Stub<IDocumentViewController>());

                var designWaterLevelCalculator = new TestDesignWaterLevelCalculator();
                var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                                 .WhenCalled(invocation =>
                                 {
                                     HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                         HydraulicBoundaryCalculationSettingsFactory.CreateSettings(
                                             assessmentSection.HydraulicBoundaryData,
                                             hydraulicBoundaryLocation),
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
                        Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(GetNormFunc()), waterLevelCalculationInput.Beta);
                    }
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(WaterLevelCalculationsForNormTargetProbabilityContextTreeNodeInfoTest), nameof(GetWaterLevelForNormTargetProbabilityCalculations), new object[]
        {
            "CalculateWaterLevelsFromContextMenu_HydraulicBoundaryDatabaseWithUsePreprocessorTrue_SendsRightInputToCalculationService_{0}"
        })]
        [Apartment(ApartmentState.STA)]
        public void CalculateWaterLevelsFromContextMenu_HydraulicBoundaryDatabaseWithUsePreprocessorClosureTrue_SendsRightInputToCalculationService(
            Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>> getCalculationsFunc)
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("locationName");
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryData =
                {
                    HydraulicLocationConfigurationDatabase =
                    {
                        FilePath = validHlcdFilePath
                    },
                    HydraulicBoundaryDatabases =
                    {
                        new HydraulicBoundaryDatabase
                        {
                            FilePath = validHrdFilePath,
                            Version = validHrdFileVersion,
                            UsePreprocessorClosure = true,
                            Locations =
                            {
                                hydraulicBoundaryLocation
                            }
                        }
                    }
                }
            };

            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            double GetNormFunc() => 0.01;
            var context = new WaterLevelCalculationsForNormTargetProbabilityContext(getCalculationsFunc(assessmentSection),
                                                                                    assessmentSection,
                                                                                    GetNormFunc);

            using (var treeViewControl = new TreeViewControl())
            {
                IMainWindow mainWindow = MainWindowTestHelper.CreateMainWindowStub(mockRepository);

                IGui gui = StubFactory.CreateGuiStub(mockRepository);
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.ProjectStore).Return(mockRepository.Stub<IStoreProject>());
                gui.Stub(g => g.DocumentViewController).Return(mockRepository.Stub<IDocumentViewController>());

                var designWaterLevelCalculator = new TestDesignWaterLevelCalculator();
                var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                                 .WhenCalled(invocation =>
                                 {
                                     HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                         HydraulicBoundaryCalculationSettingsFactory.CreateSettings(
                                             assessmentSection.HydraulicBoundaryData,
                                             hydraulicBoundaryLocation),
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
                        Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(GetNormFunc()), waterLevelCalculationInput.Beta);
                    }
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetWaterLevelForNormTargetProbabilityCalculationConfigurations))]
        [Apartment(ApartmentState.STA)]
        public void GivenValidCalculations_WhenCalculatingAllFromContextMenu_ThenLogMessagesAddedOutputSet(
            double maximumAllowableFloodingProbability, double signalingFloodingProbability,
            Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>> getCalculationsFunc)
        {
            // Given
            const string locationName = "locationName";

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("locationName");

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                FailureMechanismContribution =
                {
                    MaximumAllowableFloodingProbability = maximumAllowableFloodingProbability,
                    SignalFloodingProbability = signalingFloodingProbability
                },
                HydraulicBoundaryData =
                {
                    HydraulicLocationConfigurationDatabase =
                    {
                        FilePath = validHlcdFilePath
                    },
                    HydraulicBoundaryDatabases =
                    {
                        new HydraulicBoundaryDatabase
                        {
                            FilePath = validHrdFilePath,
                            Version = validHrdFileVersion,
                            Locations =
                            {
                                hydraulicBoundaryLocation
                            }
                        }
                    }
                }
            };

            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            IObservableEnumerable<HydraulicBoundaryLocationCalculation> calculations = getCalculationsFunc(assessmentSection);
            var context = new WaterLevelCalculationsForNormTargetProbabilityContext(calculations,
                                                                                    assessmentSection,
                                                                                    () => 0.01);

            using (var treeViewControl = new TreeViewControl())
            {
                IMainWindow mainWindow = MainWindowTestHelper.CreateMainWindowStub(mockRepository);

                IGui gui = StubFactory.CreateGuiStub(mockRepository);
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.ProjectStore).Return(mockRepository.Stub<IStoreProject>());
                gui.Stub(g => g.DocumentViewController).Return(mockRepository.Stub<IDocumentViewController>());

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

                        HydraulicBoundaryLocationCalculationOutput output = calculations.First().Output;
                        Assert.AreEqual(calculator.DesignWaterLevel, output.Result, output.Result.GetAccuracy());
                        Assert.AreEqual(CalculationConvergence.CalculatedNotConverged, output.CalculationConvergence);
                    }
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetWaterLevelForNormTargetProbabilityCalculationContinuationCases))]
        [Apartment(ApartmentState.STA)]
        public void GivenCalculationWithIllustrationPoints_WhenClearIllustrationPointsClicked_ThenExpectedOutputAndCalculationObserversNotified(
            Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>> getCalculationsFunc, bool continuation)
        {
            // Given
            var random = new Random(21);

            var calculationObserver = mockRepository.StrictMock<IObserver>();
            if (continuation)
            {
                calculationObserver.Expect(o => o.UpdateObserver());
            }

            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.AddHydraulicBoundaryLocationCalculations(new[]
            {
                new TestHydraulicBoundaryLocation()
            });

            IObservableEnumerable<HydraulicBoundaryLocationCalculation> calculations = getCalculationsFunc(assessmentSection);
            HydraulicBoundaryLocationCalculation calculation = calculations.First();
            calculation.Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble(), new TestGeneralResultSubMechanismIllustrationPoint());
            calculation.Attach(calculationObserver);

            var nodeData = new WaterLevelCalculationsForNormTargetProbabilityContext(calculations,
                                                                                     assessmentSection,
                                                                                     () => 0.01);

            DialogBoxHandler = (name, wnd) =>
            {
                var helper = new MessageBoxTester(wnd);

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
                        Assert.IsTrue(calculation.HasOutput);
                        Assert.AreEqual(!continuation, calculation.Output.HasGeneralResult);
                    }
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetWaterLevelForNormTargetProbabilityCalculationDisplayTextCases))]
        [Apartment(ApartmentState.STA)]
        public void GivenNormCalculationWithIllustrationPoints_WhenClearIllustrationPointsClicked_ThenExpectedInquiryGiven(
            Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>> getCalculationsFunc,
            double maximumAllowableFloodingProbability, double signalFloodingProbability, string expectedText)
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSectionStub
            {
                FailureMechanismContribution =
                {
                    MaximumAllowableFloodingProbability = maximumAllowableFloodingProbability,
                    SignalFloodingProbability = signalFloodingProbability
                }
            };
            assessmentSection.AddHydraulicBoundaryLocationCalculations(new HydraulicBoundaryLocation[]
            {
                new TestHydraulicBoundaryLocation()
            });

            IObservableEnumerable<HydraulicBoundaryLocationCalculation> calculations = getCalculationsFunc(assessmentSection);
            HydraulicBoundaryLocationCalculation calculation = calculations.First();
            calculation.Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble(), new TestGeneralResultSubMechanismIllustrationPoint());

            var nodeData = new WaterLevelCalculationsForNormTargetProbabilityContext(calculations,
                                                                                     assessmentSection,
                                                                                     () => 0.1);
            var messageBoxText = "";
            DialogBoxHandler = (name, wnd) =>
            {
                var helper = new MessageBoxTester(wnd);
                messageBoxText = helper.Text;
                helper.ClickCancel();
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
                        var expectedMessage = $"Weet u zeker dat u alle berekende illustratiepunten bij '{expectedText}' wilt wissen?";
                        Assert.AreEqual(expectedMessage, messageBoxText);
                    }
                }
            }

            mockRepository.VerifyAll();
        }

        private static IEnumerable<TestCaseData> GetWaterLevelForNormTargetProbabilityCalculations(string testNameFormat)
        {
            yield return new TestCaseData(new Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>>(a => a.WaterLevelCalculationsForSignalFloodingProbability))
                .SetName(string.Format(testNameFormat, "SignalFloodingProbabilityCalculations"));
            yield return new TestCaseData(new Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>>(a => a.WaterLevelCalculationsForMaximumAllowableFloodingProbability))
                .SetName(string.Format(testNameFormat, "MaximumAllowableFloodingProbabilityCalculations"));
        }

        private static IEnumerable<TestCaseData> GetWaterLevelForNormTargetProbabilityCalculationConfigurations()
        {
            yield return new TestCaseData(1 / 10.0, 1 / 100.0, new Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>>(a => a.WaterLevelCalculationsForSignalFloodingProbability))
                .SetName("SignalFloodingProbabilityCalculations");
            yield return new TestCaseData(1 / 100.0, 1 / 200.0, new Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>>(a => a.WaterLevelCalculationsForMaximumAllowableFloodingProbability))
                .SetName("MaximumAllowableFloodingProbabilityCalculations");
        }

        private static IEnumerable<TestCaseData> GetWaterLevelForNormTargetProbabilityCalculationContinuationCases()
        {
            yield return new TestCaseData(new Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>>(a => a.WaterLevelCalculationsForSignalFloodingProbability), true)
                .SetName("SignalFloodingProbabilityCalculationsContinued");
            yield return new TestCaseData(new Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>>(a => a.WaterLevelCalculationsForSignalFloodingProbability), false)
                .SetName("SignalFloodingProbabilityCalculationsCancelled");
            yield return new TestCaseData(new Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>>(a => a.WaterLevelCalculationsForMaximumAllowableFloodingProbability), true)
                .SetName("MaximumAllowableFloodingProbabilityCalculationsContinued");
            yield return new TestCaseData(new Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>>(a => a.WaterLevelCalculationsForMaximumAllowableFloodingProbability), false)
                .SetName("MaximumAllowableFloodingProbabilityCalculationsCancelled");
        }

        private static IEnumerable<TestCaseData> GetWaterLevelForNormTargetProbabilityCalculationDisplayTextCases()
        {
            const double maximumAllowableFloodingProbability = 0.1;
            const double signalFloodingProbability = 0.01;

            yield return new TestCaseData(new Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>>(a => a.WaterLevelCalculationsForSignalFloodingProbability),
                                          signalFloodingProbability, signalFloodingProbability, "1/100 (1)")
                .SetName("SignalFloodingProbabilityCalculationsProbabilitiesSame");
            yield return new TestCaseData(new Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>>(a => a.WaterLevelCalculationsForSignalFloodingProbability),
                                          maximumAllowableFloodingProbability, signalFloodingProbability, "1/100")
                .SetName("SignalFloodingProbabilityCalculationsProbabilitiesDifferent");
            yield return new TestCaseData(new Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>>(a => a.WaterLevelCalculationsForMaximumAllowableFloodingProbability),
                                          maximumAllowableFloodingProbability, maximumAllowableFloodingProbability, "1/10")
                .SetName("MaximumAllowableFloodingProbabilityCalculationsProbabilitiesSame");
            yield return new TestCaseData(new Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>>(a => a.WaterLevelCalculationsForMaximumAllowableFloodingProbability),
                                          maximumAllowableFloodingProbability, signalFloodingProbability, "1/10")
                .SetName("MaximumAllowableFloodingProbabilityCalculationsProbabilitiesDifferent");
        }

        public override void Setup()
        {
            mockRepository = new MockRepository();
        }

        private static TreeNodeInfo GetInfo(RiskeerPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(WaterLevelCalculationsForNormTargetProbabilityContext));
        }
    }
}