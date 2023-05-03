﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Plugin.TestUtil;
using Riskeer.Common.Service.TestUtil;
using Riskeer.HydraRing.Calculation.Calculator;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;
using Riskeer.Integration.Data;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class WaterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuAddTargetProbabilityIndex = 2;
        private const int contextMenuRunWaterLevelCalculationsIndex = 4;
        private const int contextMenuClearIllustrationPointsIndex = 6;

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
                Assert.IsNotNull(info.ChildNodeObjects);
                Assert.IsNull(info.CanRename);
                Assert.IsNull(info.OnNodeRenamed);
                Assert.IsNull(info.CanRemove);
                Assert.IsNull(info.OnNodeRemoved);
                Assert.IsNotNull(info.OnRemoveChildNodesConfirmationText);
                Assert.IsNull(info.CanCheck);
                Assert.IsNull(info.CheckedState);
                Assert.IsNull(info.OnNodeChecked);
                Assert.IsNull(info.CanDrag);
                Assert.IsNotNull(info.CanDrop);
                Assert.IsNotNull(info.CanInsert);
                Assert.IsNotNull(info.OnDrop);
            }
        }

        [Test]
        public void Text_Always_ReturnsSetName()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                string text = info.Text(null);

                // Assert
                Assert.AreEqual("Waterstanden bij vrije doelkans", text);
            }
        }

        [Test]
        public void Image_Always_ReturnsGeneralFolderIcon()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Image image = info.Image(null);

                // Assert
                TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.GeneralFolderIcon, image);
            }
        }

        [Test]
        public void OnRemoveConfirmationText_Always_ReturnsConfirmationMessage()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                string infoOnRemoveChildNodesConfirmationText = info.OnRemoveChildNodesConfirmationText(null);

                // Assert
                string expectedText = "Als u deze doelkansen verwijdert, dan wordt de uitvoer van alle ervan afhankelijke berekeningen verwijderd."
                                      + Environment.NewLine
                                      + Environment.NewLine
                                      + "Weet u zeker dat u wilt doorgaan?";
                Assert.AreEqual(expectedText, infoOnRemoveChildNodesConfirmationText);
            }
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnsChildrenOfData()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();

            var calculationsForTargetProbabilities = new ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>
            {
                new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1),
                new HydraulicBoundaryLocationCalculationsForTargetProbability(0.01)
            };
            var calculationsGroupContext = new WaterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContext(calculationsForTargetProbabilities, assessmentSection);

            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                object[] childNodeObjects = info.ChildNodeObjects(calculationsGroupContext);

                // Assert
                Assert.AreEqual(2, childNodeObjects.Length);

                WaterLevelCalculationsForUserDefinedTargetProbabilityContext[] calculationsContexts = childNodeObjects.OfType<WaterLevelCalculationsForUserDefinedTargetProbabilityContext>().ToArray();
                Assert.AreEqual(2, calculationsContexts.Length);

                Assert.AreSame(calculationsForTargetProbabilities[0], calculationsContexts[0].WrappedData);
                Assert.AreSame(assessmentSection, calculationsContexts[0].AssessmentSection);

                Assert.AreSame(calculationsForTargetProbabilities[1], calculationsContexts[1].WrappedData);
                Assert.AreSame(assessmentSection, calculationsContexts[1].AssessmentSection);
            }
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            IAssessmentSection assessmentSection = new AssessmentSectionStub();

            var mockRepository = new MockRepository();
            var menuBuilder = mockRepository.StrictMock<IContextMenuBuilder>();
            using (mockRepository.Ordered())
            {
                menuBuilder.Expect(mb => mb.AddExportItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddDeleteChildrenItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.Build()).Return(null);
            }

            var nodeData = new WaterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContext(new ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>(),
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
            var nodeData = new WaterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContext(new ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>(),
                                                                                                   assessmentSection);

            var mockRepository = new MockRepository();

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
                        Assert.AreEqual(11, menu.Items.Count);

                        TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddTargetProbabilityIndex,
                                                                      "Doelkans &toevoegen",
                                                                      "Voeg een nieuwe doelkans toe aan deze map.",
                                                                      RiskeerCommonFormsResources.GenericInputOutputIcon);

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
        public void ContextMenuStrip_ClickOnAddTargetProbabilityItem_CalculationsForTargetProbabilityAddedAndObserversNotified()
        {
            // Given
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryData =
                {
                    HydraulicBoundaryDatabases =
                    {
                        new HydraulicBoundaryDatabase
                        {
                            Locations =
                            {
                                new TestHydraulicBoundaryLocation("Location 1"),
                                new TestHydraulicBoundaryLocation("Location 2")
                            }
                        }
                    }
                }
            };

            var calculations = new ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>();
            var context = new WaterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContext(calculations, assessmentSection);

            var mockRepository = new MockRepository();
            var calculationsObserver = mockRepository.StrictMock<IObserver>();
            calculationsObserver.Expect(o => o.UpdateObserver());
            calculations.Attach(calculationsObserver);

            using (var treeViewControl = new TreeViewControl())
            {
                IMainWindow mainWindow = MainWindowTestHelper.CreateMainWindowStub(mockRepository);

                IGui gui = StubFactory.CreateGuiStub(mockRepository);
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.ProjectStore).Return(mockRepository.Stub<IStoreProject>());
                gui.Stub(g => g.DocumentViewController).Return(mockRepository.Stub<IDocumentViewController>());

                mockRepository.ReplayAll();

                using (var plugin = new RiskeerPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = gui;
                    plugin.Activate();

                    using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(context, null, treeViewControl))
                    {
                        // When
                        contextMenuAdapter.Items[contextMenuAddTargetProbabilityIndex].PerformClick();

                        // Then
                        Assert.AreEqual(1, calculations.Count);
                        Assert.AreEqual(0.01, calculations[0].TargetProbability);
                        Assert.AreEqual(2, calculations[0].HydraulicBoundaryLocationCalculations.Count);
                    }
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_AllRequiredInputSet_ContextMenuItemCalculateAllEnabled()
        {
            // Setup
            IAssessmentSection assessmentSection = new AssessmentSectionStub();

            var calculationsForTargetProbabilities = new ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>
            {
                new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1)
            };

            var nodeData = new WaterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContext(calculationsForTargetProbabilities, assessmentSection);

            var mockRepository = new MockRepository();
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
        [TestCaseSource(nameof(GetWaterLevelCalculations))]
        public void ContextMenuStrip_WaterLevelCalculationsWithIllustrationPoints_ContextMenuItemClearAllIllustrationPointsEnabledAndTooltipSet(
            Func<IAssessmentSection, HydraulicBoundaryLocationCalculation> getHydraulicLocationCalculationFunc)
        {
            // Setup
            var random = new Random(21);
            IAssessmentSection assessmentSection = GetConfiguredAssessmentSectionWithHydraulicBoundaryLocationCalculations();
            HydraulicBoundaryLocationCalculation calculation = getHydraulicLocationCalculationFunc(assessmentSection);
            calculation.Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble(), new TestGeneralResultSubMechanismIllustrationPoint());

            var nodeData = new WaterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContext(
                assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities,
                assessmentSection);

            var mockRepository = new MockRepository();
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
        public void ContextMenuStrip_WaterLevelCalculationsWithoutIllustrationPoints_ContextMenuItemClearAllIllustrationPointsDisabled()
        {
            // Setup
            IAssessmentSection assessmentSection = GetConfiguredAssessmentSectionWithHydraulicBoundaryLocationCalculations();

            var nodeData = new WaterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContext(
                assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities,
                assessmentSection);

            var mockRepository = new MockRepository();
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
        [SetCulture("nl-NL")]
        [Apartment(ApartmentState.STA)]
        public void GivenValidCalculations_WhenCalculatingAllFromContextMenu_ThenLogMessagesAddedOutputSet()
        {
            // Given
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("locationName");

            var mockRepository = new MockRepository();
            var assessmentSection = new AssessmentSectionStub
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

            assessmentSection.AddHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            var context = new WaterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContext(
                assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities,
                assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                IMainWindow mainWindow = MainWindowTestHelper.CreateMainWindowStub(mockRepository);

                IGui gui = StubFactory.CreateGuiStub(mockRepository);
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.ProjectStore).Return(mockRepository.Stub<IStoreProject>());
                gui.Stub(g => g.DocumentViewController).Return(mockRepository.Stub<IDocumentViewController>());

                var designWaterLevelCalculator = new TestDesignWaterLevelCalculator
                {
                    Converged = false
                };
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
                                 .Return(designWaterLevelCalculator)
                                 .Repeat
                                 .Times(2);
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
                            Assert.AreEqual(16, msgs.Length);

                            const string calculationTypeDisplayName = "Waterstand";
                            const string calculationDisplayName = "Waterstand berekening";

                            HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                                hydraulicBoundaryLocation.Name, calculationTypeDisplayName, calculationDisplayName, "1/10.000", msgs, 0);
                            HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                                hydraulicBoundaryLocation.Name, calculationTypeDisplayName, calculationDisplayName, "1/100.000", msgs, 8);
                        });

                        AssertHydraulicBoundaryLocationCalculationOutput(designWaterLevelCalculator, assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities[0].HydraulicBoundaryLocationCalculations[0].Output);
                        AssertHydraulicBoundaryLocationCalculationOutput(designWaterLevelCalculator, assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities[1].HydraulicBoundaryLocationCalculations[0].Output);
                    }
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetWaterLevelCalculations))]
        [Apartment(ApartmentState.STA)]
        public void GivenCalculationsWithIllustrationPoints_WhenClearIllustrationPointsClickedAndDoNotContinue_ThenInquiryAndIllustrationPointsNotCleared(
            Func<IAssessmentSection, HydraulicBoundaryLocationCalculation> getHydraulicLocationCalculationFunc)
        {
            // Given
            var random = new Random(21);
            IAssessmentSection assessmentSection = GetConfiguredAssessmentSectionWithHydraulicBoundaryLocationCalculations();
            HydraulicBoundaryLocationCalculation calculation = getHydraulicLocationCalculationFunc(assessmentSection);
            calculation.Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble(), new TestGeneralResultSubMechanismIllustrationPoint());

            HydraulicBoundaryLocationCalculation[] calculationsWithOutput = GetAllWaterLevelCalculationsWithOutput(assessmentSection).ToArray();

            var messageBoxText = "";
            DialogBoxHandler = (name, wnd) =>
            {
                var helper = new MessageBoxTester(wnd);
                messageBoxText = helper.Text;

                helper.ClickCancel();
            };

            var context = new WaterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContext(
                assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities,
                assessmentSection);

            var mockRepository = new MockRepository();
            using (var treeViewControl = new TreeViewControl())
            {
                var calculationObserver = mockRepository.StrictMock<IObserver>();

                IGui gui = StubFactory.CreateGuiStub(mockRepository);
                gui.Stub(cmp => cmp.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mockRepository.ReplayAll();

                calculation.Attach(calculationObserver);

                using (var plugin = new RiskeerPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = gui;

                    using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(context, null, treeViewControl))
                    {
                        // When
                        contextMenuAdapter.Items[contextMenuClearIllustrationPointsIndex].PerformClick();

                        // Then
                        const string expectedMessage = "Weet u zeker dat u alle berekende illustratiepunten bij 'Waterstanden bij vrije doelkans' wilt wissen?";
                        Assert.AreEqual(expectedMessage, messageBoxText);

                        Assert.IsTrue(calculationsWithOutput.All(calc => calc.HasOutput));
                        Assert.IsTrue(calculation.Output.HasGeneralResult);
                    }
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetWaterLevelCalculations))]
        [Apartment(ApartmentState.STA)]
        public void GivenCalculationsWithIllustrationPoints_WhenClearIllustrationPointsClickedAndContinue_ThenInquiryAndIllustrationPointsCleared(
            Func<IAssessmentSection, HydraulicBoundaryLocationCalculation> getHydraulicLocationCalculationFunc)
        {
            // Given
            var random = new Random(21);
            IAssessmentSection assessmentSection = GetConfiguredAssessmentSectionWithHydraulicBoundaryLocationCalculations();
            HydraulicBoundaryLocationCalculation calculation = getHydraulicLocationCalculationFunc(assessmentSection);
            calculation.Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble(), new TestGeneralResultSubMechanismIllustrationPoint());

            HydraulicBoundaryLocationCalculation[] calculationsWithOutput = GetAllWaterLevelCalculationsWithOutput(assessmentSection).ToArray();

            var messageBoxText = "";
            DialogBoxHandler = (name, wnd) =>
            {
                var helper = new MessageBoxTester(wnd);
                messageBoxText = helper.Text;

                helper.ClickOk();
            };

            var context = new WaterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContext(
                assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities,
                assessmentSection);

            var mockRepository = new MockRepository();
            using (var treeViewControl = new TreeViewControl())
            {
                var calculationObserver = mockRepository.StrictMock<IObserver>();
                calculationObserver.Expect(o => o.UpdateObserver());

                IGui gui = StubFactory.CreateGuiStub(mockRepository);
                gui.Stub(cmp => cmp.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mockRepository.ReplayAll();

                calculation.Attach(calculationObserver);

                using (var plugin = new RiskeerPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = gui;

                    using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(context, null, treeViewControl))
                    {
                        // When
                        contextMenuAdapter.Items[contextMenuClearIllustrationPointsIndex].PerformClick();

                        // Then
                        const string expectedMessage = "Weet u zeker dat u alle berekende illustratiepunten bij 'Waterstanden bij vrije doelkans' wilt wissen?";
                        Assert.AreEqual(expectedMessage, messageBoxText);

                        Assert.IsTrue(calculationsWithOutput.All(calc => calc.HasOutput));
                        Assert.IsFalse(calculation.Output.HasGeneralResult);
                    }
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void CanInsert_DraggedDataOfUnsupportedDataType_ReturnsFalse()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                var targetData = new WaterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContext(
                    new ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>(),
                    new AssessmentSectionStub());

                // Call
                bool canInsert = info.CanInsert(new object(), targetData);

                // Assert
                Assert.IsFalse(canInsert);
            }
        }

        [Test]
        public void CanInsert_DraggedDataNotPartOfGroupContext_ReturnsFalse()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                var assessmentSectionStub = new AssessmentSectionStub();

                var targetData = new WaterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContext(
                    new ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>(),
                    assessmentSectionStub);

                var draggedData = new WaterLevelCalculationsForUserDefinedTargetProbabilityContext(
                    new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1),
                    assessmentSectionStub);

                // Call
                bool canInsert = info.CanInsert(draggedData, targetData);

                // Assert
                Assert.IsFalse(canInsert);
            }
        }

        [Test]
        public void CanInsert_DraggedDataPartOfGroupContext_ReturnsTrue()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                var assessmentSectionStub = new AssessmentSectionStub();
                var calculationsForTargetProbability = new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1);

                var targetData = new WaterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContext(
                    new ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>
                    {
                        calculationsForTargetProbability
                    },
                    assessmentSectionStub);

                var draggedData = new WaterLevelCalculationsForUserDefinedTargetProbabilityContext(
                    calculationsForTargetProbability,
                    assessmentSectionStub);

                // Call
                bool canInsert = info.CanInsert(draggedData, targetData);

                // Assert
                Assert.IsTrue(canInsert);
            }
        }

        [Test]
        public void CanDrop_DraggedDataOfUnsupportedDataType_ReturnsFalse()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                var targetData = new WaterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContext(
                    new ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>(),
                    new AssessmentSectionStub());

                // Call
                bool canDrop = info.CanDrop(new object(), targetData);

                // Assert
                Assert.IsFalse(canDrop);
            }
        }

        [Test]
        public void CanDrop_DraggedDataNotPartOfGroupContext_ReturnsFalse()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                var assessmentSectionStub = new AssessmentSectionStub();

                var targetData = new WaterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContext(
                    new ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>(),
                    assessmentSectionStub);

                var draggedData = new WaterLevelCalculationsForUserDefinedTargetProbabilityContext(
                    new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1),
                    assessmentSectionStub);

                // Call
                bool canDrop = info.CanDrop(draggedData, targetData);

                // Assert
                Assert.IsFalse(canDrop);
            }
        }

        [Test]
        public void CanDrop_DraggedDataPartOfGroupContext_ReturnsTrue()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                var assessmentSectionStub = new AssessmentSectionStub();
                var calculationsForTargetProbability = new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1);

                var targetData = new WaterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContext(
                    new ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>
                    {
                        calculationsForTargetProbability
                    },
                    assessmentSectionStub);

                var draggedData = new WaterLevelCalculationsForUserDefinedTargetProbabilityContext(
                    calculationsForTargetProbability,
                    assessmentSectionStub);

                // Call
                bool canDrop = info.CanDrop(draggedData, targetData);

                // Assert
                Assert.IsTrue(canDrop);
            }
        }

        [Test]
        public void OnDrop_DataDroppedToDifferentIndex_DroppedDataCorrectlyMovedAndObserversNotified()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                var assessmentSectionStub = new AssessmentSectionStub();

                var calculationsForTargetProbability1 = new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1);
                var calculationsForTargetProbability2 = new HydraulicBoundaryLocationCalculationsForTargetProbability(0.01);
                var calculationsForTargetProbability3 = new HydraulicBoundaryLocationCalculationsForTargetProbability(0.001);

                var calculationsForTargetProbabilities = new ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>
                {
                    calculationsForTargetProbability1,
                    calculationsForTargetProbability2,
                    calculationsForTargetProbability3
                };

                var parentData = new WaterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContext(
                    calculationsForTargetProbabilities,
                    assessmentSectionStub);

                var droppedData = new WaterLevelCalculationsForUserDefinedTargetProbabilityContext(
                    calculationsForTargetProbability3,
                    assessmentSectionStub);

                var mockRepository = new MockRepository();
                var observer = mockRepository.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());
                mockRepository.ReplayAll();

                calculationsForTargetProbabilities.Attach(observer);

                // Call
                info.OnDrop(droppedData, parentData, parentData, 1, null);

                // Assert
                CollectionAssert.AreEqual(new[]
                {
                    calculationsForTargetProbability1,
                    calculationsForTargetProbability3,
                    calculationsForTargetProbability2
                }, calculationsForTargetProbabilities);

                mockRepository.VerifyAll();
            }
        }

        private static void AssertHydraulicBoundaryLocationCalculationOutput(IDesignWaterLevelCalculator designWaterLevelCalculator,
                                                                             HydraulicBoundaryLocationCalculationOutput actualOutput)
        {
            Assert.AreEqual(designWaterLevelCalculator.DesignWaterLevel, actualOutput.Result, actualOutput.Result.GetAccuracy());
            Assert.AreEqual(CalculationConvergence.CalculatedNotConverged, actualOutput.CalculationConvergence);
        }

        private static IAssessmentSection GetConfiguredAssessmentSectionWithHydraulicBoundaryLocationCalculations()
        {
            var assessmentSection = new AssessmentSectionStub();

            var hydraulicBoundaryLocations = new[]
            {
                new TestHydraulicBoundaryLocation(),
                new TestHydraulicBoundaryLocation(),
                new TestHydraulicBoundaryLocation()
            };

            assessmentSection.AddHydraulicBoundaryLocationCalculations(hydraulicBoundaryLocations, true);

            return assessmentSection;
        }

        private static IEnumerable<HydraulicBoundaryLocationCalculation> GetAllWaterLevelCalculationsWithOutput(IAssessmentSection assessmentSection)
        {
            return assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities
                                    .SelectMany(wlc => wlc.HydraulicBoundaryLocationCalculations)
                                    .Where(calc => calc.HasOutput);
        }

        private static IEnumerable<TestCaseData> GetWaterLevelCalculations()
        {
            yield return new TestCaseData(new Func<IAssessmentSection, HydraulicBoundaryLocationCalculation>(
                                              section => section.WaterLevelCalculationsForUserDefinedTargetProbabilities[0].HydraulicBoundaryLocationCalculations[0]));
            yield return new TestCaseData(new Func<IAssessmentSection, HydraulicBoundaryLocationCalculation>(
                                              section => section.WaterLevelCalculationsForUserDefinedTargetProbabilities[1].HydraulicBoundaryLocationCalculations[1]));
        }

        private static TreeNodeInfo GetInfo(RiskeerPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(WaterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContext));
        }
    }
}