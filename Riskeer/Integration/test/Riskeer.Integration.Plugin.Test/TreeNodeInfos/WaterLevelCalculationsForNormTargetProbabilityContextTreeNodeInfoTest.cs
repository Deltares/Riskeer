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
using Core.Gui.Commands;
using Core.Gui.ContextMenu;
using Core.Gui.Forms.Main;
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
        public void Text_WithContext_ReturnsUniquelyFormattedTargetProbabilityForLowerLimitNorm(double lowerLimitNorm, double signalingNorm, string expectedText)
        {
            // Setup
            var lowerLimitNormCalculations = new ObservableList<HydraulicBoundaryLocationCalculation>();

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mockRepository);

            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(new FailureMechanismContribution(lowerLimitNorm, signalingNorm));
            assessmentSection.Stub(a => a.WaterLevelCalculationsForLowerLimitNorm).Return(lowerLimitNormCalculations);
            assessmentSection.Stub(a => a.WaterLevelCalculationsForSignalingNorm).Return(new ObservableList<HydraulicBoundaryLocationCalculation>());
            assessmentSection.Stub(a => a.WaterLevelCalculationsForUserDefinedTargetProbabilities).Return(new ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>());

            mockRepository.ReplayAll();

            var context = new WaterLevelCalculationsForNormTargetProbabilityContext(lowerLimitNormCalculations, assessmentSection, () => lowerLimitNorm);

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
        public void Text_WithContext_ReturnsUniquelyFormattedTargetProbabilityForSignalingNorm(double lowerLimitNorm, double signalingNorm, string expectedText)
        {
            // Setup
            var signalingNormCalculations = new ObservableList<HydraulicBoundaryLocationCalculation>();

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mockRepository);

            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(new FailureMechanismContribution(lowerLimitNorm, signalingNorm));
            assessmentSection.Stub(a => a.WaterLevelCalculationsForLowerLimitNorm).Return(new ObservableList<HydraulicBoundaryLocationCalculation>());
            assessmentSection.Stub(a => a.WaterLevelCalculationsForSignalingNorm).Return(signalingNormCalculations);
            assessmentSection.Stub(a => a.WaterLevelCalculationsForUserDefinedTargetProbabilities).Return(new ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>());

            mockRepository.ReplayAll();

            var context = new WaterLevelCalculationsForNormTargetProbabilityContext(signalingNormCalculations, assessmentSection, () => lowerLimitNorm);

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
        [TestCaseSource(nameof(GetWaterLevelForNormTargetProbabilityCalculations))]
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
        [TestCaseSource(nameof(GetWaterLevelForNormTargetProbabilityCalculations))]
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
        [TestCaseSource(nameof(GetWaterLevelForNormTargetProbabilityCalculations))]
        public void ContextMenuStrip_HydraulicBoundaryDatabaseNotLinked_ContextMenuItemCalculateAllDisabledAndTooltipSet(
            Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>> getCalculationsFunc)
        {
            // Setup
            IAssessmentSection assessmentSection = new AssessmentSectionStub();
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
        [TestCaseSource(nameof(GetWaterLevelForNormTargetProbabilityCalculations))]
        public void ContextMenuStrip_HydraulicBoundaryDatabaseLinkedToInvalidFile_ContextMenuItemCalculateAllDisabledAndTooltipSet(
            Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>> getCalculationsFunc)
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = "invalidFilePath"
                }
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);

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
        [TestCaseSource(nameof(GetWaterLevelForNormTargetProbabilityCalculations))]
        public void ContextMenuStrip_AllRequiredInputSet_ContextMenuItemCalculateAllEnabled(
            Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>> getCalculationsFunc)
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, nameof(HydraulicBoundaryDatabase)), "complete.sqlite")
                }
            };

            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);
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
        [TestCaseSource(nameof(GetWaterLevelForNormTargetProbabilityCalculations))]
        public void ContextMenuStrip_HydraulicBoundaryLocationCalculationsWithIllustrationPoints_ContextMenuItemClearAllIllustrationPointsEnabledAndTooltipSet(
            Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>> getCalculationsFunc)
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
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
        [TestCaseSource(nameof(GetWaterLevelForNormTargetProbabilityCalculations))]
        public void ContextMenuStrip_HydraulicBoundaryLocationCalculationsWithoutIllustrationPoints_ContextMenuItemClearAllIllustrationPointsDisabled(
            Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>> getCalculationsFunc)
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                new TestHydraulicBoundaryLocation(),
                new TestHydraulicBoundaryLocation()
            });

            IObservableEnumerable<HydraulicBoundaryLocationCalculation> calculations = assessmentSection.WaterLevelCalculationsForLowerLimitNorm;
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
        [TestCaseSource(nameof(GetWaterLevelForNormTargetProbabilityCalculations))]
        [Apartment(ApartmentState.STA)]
        public void CalculateWaterLevelsFromContextMenu_HydraulicBoundaryDatabaseWithCanUsePreprocessorFalse_SendsRightInputToCalculationService(
            Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>> getCalculationsFunc)
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("locationName");
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite")
                }
            };
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);

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
                gui.Stub(g => g.ViewCommands).Return(mockRepository.Stub<IViewCommands>());

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
                        Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(GetNormFunc()), waterLevelCalculationInput.Beta);
                    }
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetWaterLevelForNormTargetProbabilityCalculations))]
        [Apartment(ApartmentState.STA)]
        public void CalculateWaterLevelsFromContextMenu_HydraulicBoundaryDatabaseWithUsePreprocessorTrue_SendsRightInputToCalculationService(
            Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>> getCalculationsFunc)
        {
            // Setup
            string preprocessorDirectory = TestHelper.GetScratchPadPath();
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("locationName");
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
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);

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
                gui.Stub(g => g.ViewCommands).Return(mockRepository.Stub<IViewCommands>());

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
                        Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(GetNormFunc()), waterLevelCalculationInput.Beta);
                    }
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetWaterLevelForNormTargetProbabilityCalculations))]
        [Apartment(ApartmentState.STA)]
        public void CalculateWaterLevelsFromContextMenu_HydraulicBoundaryDatabaseWithUsePreprocessorFalse_SendsRightInputToCalculationService(
            Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>> getCalculationsFunc)
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("locationName");
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
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);

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
                gui.Stub(g => g.ViewCommands).Return(mockRepository.Stub<IViewCommands>());

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
            double lowerLimitNorm, double signallingNorm,
            Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>> getCalculationsFunc)
        {
            // Given
            const string locationName = "locationName";
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                FailureMechanismContribution =
                {
                    LowerLimitNorm = lowerLimitNorm,
                    SignalingNorm = signallingNorm
                },
                HydraulicBoundaryDatabase =
                {
                    FilePath = validFilePath
                }
            };
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                new TestHydraulicBoundaryLocation("locationName")
            });

            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);

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
                gui.Stub(g => g.ViewCommands).Return(mockRepository.Stub<IViewCommands>());

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
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
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
            double lowerLimitNorm, double signalingNorm, string expectedText)
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSectionStub
            {
                FailureMechanismContribution =
                {
                    LowerLimitNorm = lowerLimitNorm,
                    SignalingNorm = signalingNorm
                }
            };
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new HydraulicBoundaryLocation[]
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

        private static IEnumerable<TestCaseData> GetWaterLevelForNormTargetProbabilityCalculations()
        {
            yield return new TestCaseData(new Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>>(a => a.WaterLevelCalculationsForSignalingNorm))
                .SetName("SignalingNormCalculations");
            yield return new TestCaseData(new Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>>(a => a.WaterLevelCalculationsForLowerLimitNorm))
                .SetName("LowerLimitNormCalculations");
        }

        private static IEnumerable<TestCaseData> GetWaterLevelForNormTargetProbabilityCalculationConfigurations()
        {
            yield return new TestCaseData(1 / 10.0, 1 / 100.0, new Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>>(a => a.WaterLevelCalculationsForSignalingNorm))
                .SetName("SignalingNormCalculations");
            yield return new TestCaseData(1 / 100.0, 1 / 200.0, new Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>>(a => a.WaterLevelCalculationsForLowerLimitNorm))
                .SetName("LowerLimitNormCalculations");
        }

        private static IEnumerable<TestCaseData> GetWaterLevelForNormTargetProbabilityCalculationContinuationCases()
        {
            yield return new TestCaseData(new Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>>(a => a.WaterLevelCalculationsForSignalingNorm), true)
                .SetName("SignalingNormCalculationsContinued");
            yield return new TestCaseData(new Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>>(a => a.WaterLevelCalculationsForSignalingNorm), false)
                .SetName("SignalingNormCalculationsCancelled");
            yield return new TestCaseData(new Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>>(a => a.WaterLevelCalculationsForLowerLimitNorm), true)
                .SetName("LowerLimitNormCalculationsContinued");
            yield return new TestCaseData(new Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>>(a => a.WaterLevelCalculationsForLowerLimitNorm), false)
                .SetName("LowerLimitNormCalculationsCancelled");
        }

        private static IEnumerable<TestCaseData> GetWaterLevelForNormTargetProbabilityCalculationDisplayTextCases()
        {
            const double lowerLimitNorm = 0.1;
            const double signalingNorm = 0.01;

            yield return new TestCaseData(new Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>>(a => a.WaterLevelCalculationsForSignalingNorm),
                                          signalingNorm, signalingNorm, "1/100 (1)")
                .SetName("SignalingNormCalculationsNormsSame");
            yield return new TestCaseData(new Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>>(a => a.WaterLevelCalculationsForSignalingNorm),
                                          lowerLimitNorm, signalingNorm, "1/100")
                .SetName("SignalingNormCalculationsNormsDifferent");
            yield return new TestCaseData(new Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>>(a => a.WaterLevelCalculationsForLowerLimitNorm),
                                          lowerLimitNorm, lowerLimitNorm, "1/10")
                .SetName("LowerLimitNormCalculationsNormsSame");
            yield return new TestCaseData(new Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>>(a => a.WaterLevelCalculationsForLowerLimitNorm),
                                          lowerLimitNorm, signalingNorm, "1/10")
                .SetName("LowerLimitNormCalculationsNormsDifferent");
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