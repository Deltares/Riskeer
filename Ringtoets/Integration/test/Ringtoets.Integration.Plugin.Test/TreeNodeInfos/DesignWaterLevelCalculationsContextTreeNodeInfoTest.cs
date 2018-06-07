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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Forms.ViewHost;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.Integration.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class DesignWaterLevelCalculationsContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuRunAssessmentLevelCalculationsIndex = 2;
        private MockRepository mockRepository;

        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new RingtoetsPlugin())
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
                Assert.IsNull(info.IsChecked);
                Assert.IsNull(info.OnNodeChecked);
                Assert.IsNull(info.CanDrag);
                Assert.IsNull(info.CanDrop);
                Assert.IsNull(info.CanInsert);
                Assert.IsNull(info.OnDrop);
            }
        }

        [Test]
        public void Text_Always_ReturnsCategoryBoundaryName()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mockRepository);
            mockRepository.ReplayAll();

            const string categoryBoundaryName = "A";
            var context = new DesignWaterLevelCalculationsContext(new ObservableList<HydraulicBoundaryLocationCalculation>(),
                                                                  assessmentSection,
                                                                  () => 0.01,
                                                                  categoryBoundaryName);

            // Setup
            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                string text = info.Text(context);

                // Assert
                Assert.AreEqual("Categorie " + categoryBoundaryName, text);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Image_Always_ReturnsGenericInputOutputIcon()
        {
            // Setup
            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Image image = info.Image(null);

                // Assert
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GenericInputOutputIcon, image);
            }
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mockRepository);

            var menuBuilder = mockRepository.StrictMock<IContextMenuBuilder>();
            using (mockRepository.Ordered())
            {
                menuBuilder.Expect(mb => mb.AddOpenItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.Build()).Return(null);
            }

            var nodeData = new DesignWaterLevelCalculationsContext(new ObservableList<HydraulicBoundaryLocationCalculation>(),
                                                                   assessmentSection,
                                                                   () => 0.01,
                                                                   "A");

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                mockRepository.ReplayAll();

                using (var plugin = new RingtoetsPlugin())
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
        public void ContextMenuStrip_HydraulicBoundaryDatabaseNotLinked_ContextMenuItemCalculateAllDisabledAndTooltipSet()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mockRepository);

            var nodeData = new DesignWaterLevelCalculationsContext(new ObservableList<HydraulicBoundaryLocationCalculation>(),
                                                                   assessmentSection,
                                                                   () => 0.01,
                                                                   "A");

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mockRepository.ReplayAll();

                using (var plugin = new RingtoetsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    plugin.Gui = gui;

                    // Call
                    using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                    {
                        // Assert
                        ToolStripItem contextMenuItem = contextMenu.Items[contextMenuRunAssessmentLevelCalculationsIndex];

                        Assert.AreEqual("Alles be&rekenen", contextMenuItem.Text);
                        StringAssert.Contains("Er is geen hydraulische randvoorwaardendatabase geïmporteerd.", contextMenuItem.ToolTipText);
                        TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.CalculateAllIcon, contextMenuItem.Image);
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
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(null, mockRepository, "invalidFilePath");

            var nodeData = new DesignWaterLevelCalculationsContext(new ObservableList<HydraulicBoundaryLocationCalculation>(),
                                                                   assessmentSection,
                                                                   () => 0.01,
                                                                   "A");

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mockRepository.ReplayAll();

                using (var plugin = new RingtoetsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    plugin.Gui = gui;

                    // Call
                    using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                    {
                        // Assert
                        ToolStripItem contextMenuItem = contextMenu.Items[contextMenuRunAssessmentLevelCalculationsIndex];

                        Assert.AreEqual("Alles be&rekenen", contextMenuItem.Text);
                        StringAssert.Contains("Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt.", contextMenuItem.ToolTipText);
                        TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.CalculateAllIcon, contextMenuItem.Image);
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
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "HydraulicBoundaryDatabaseImporter"), "complete.sqlite")
            };

            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase);

            var nodeData = new DesignWaterLevelCalculationsContext(new ObservableList<HydraulicBoundaryLocationCalculation>(),
                                                                   assessmentSection,
                                                                   () => 0.01,
                                                                   "A");

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mockRepository.ReplayAll();

                using (var plugin = new RingtoetsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    plugin.Gui = gui;

                    // Call
                    using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                    {
                        // Assert
                        const string expectedItemText = @"Alles be&rekenen";
                        const string expectedItemTooltip = @"Alle waterstanden berekenen.";

                        TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuRunAssessmentLevelCalculationsIndex,
                                                                      expectedItemText, expectedItemTooltip, RingtoetsCommonFormsResources.CalculateAllIcon);
                    }
                }
            }

            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void CalculateDesignWaterLevelsFromContextMenu_HydraulicBoundaryDatabaseWithCanUsePreprocessorFalse_SendsRightInputToCalculationService()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite")
                }
            };

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("locationName");
            var hydraulicBoundaryLocationCalculations = new ObservableList<HydraulicBoundaryLocationCalculation>
            {
                new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation)
            };

            Func<double> getNormFunc = () => 0.01;
            var context = new DesignWaterLevelCalculationsContext(hydraulicBoundaryLocationCalculations,
                                                                  assessmentSection,
                                                                  getNormFunc,
                                                                  "A");

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                gui.Stub(g => g.DocumentViewController).Return(mockRepository.Stub<IDocumentViewController>());

                var designWaterLevelCalculator = new TestDesignWaterLevelCalculator();
                var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(testDataPath, string.Empty)).Return(designWaterLevelCalculator);
                mockRepository.ReplayAll();

                DialogBoxHandler = (name, wnd) =>
                {
                    // Expect an activity dialog which is automatically closed
                };

                using (var plugin = new RingtoetsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = gui;
                    plugin.Activate();

                    using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(context, null, treeViewControl))
                    using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                    {
                        // Call
                        contextMenuAdapter.Items[contextMenuRunAssessmentLevelCalculationsIndex].PerformClick();

                        // Assert
                        AssessmentLevelCalculationInput designWaterLevelCalculationInput = designWaterLevelCalculator.ReceivedInputs.First();

                        Assert.AreEqual(hydraulicBoundaryLocation.Id, designWaterLevelCalculationInput.HydraulicBoundaryLocationId);
                        Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(getNormFunc()), designWaterLevelCalculationInput.Beta);
                    }
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void CalculateDesignWaterLevelsFromContextMenu_HydraulicBoundaryDatabaseWithUsePreprocessorTrue_SendsRightInputToCalculationService()
        {
            // Setup
            string preprocessorDirectory = TestHelper.GetScratchPadPath();
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite"),
                    CanUsePreprocessor = true,
                    UsePreprocessor = true,
                    PreprocessorDirectory = preprocessorDirectory
                }
            };

            Func<double> getNormFunc = () => 0.01;

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("locationName");
            var hydraulicBoundaryLocationCalculations = new ObservableList<HydraulicBoundaryLocationCalculation>
            {
                new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation)
            };

            var context = new DesignWaterLevelCalculationsContext(hydraulicBoundaryLocationCalculations,
                                                                  assessmentSection,
                                                                  getNormFunc,
                                                                  "A");

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                gui.Stub(g => g.DocumentViewController).Return(mockRepository.Stub<IDocumentViewController>());

                var designWaterLevelCalculator = new TestDesignWaterLevelCalculator();
                var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(testDataPath, preprocessorDirectory)).Return(designWaterLevelCalculator);
                mockRepository.ReplayAll();

                DialogBoxHandler = (name, wnd) =>
                {
                    // Expect an activity dialog which is automatically closed
                };

                using (var plugin = new RingtoetsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = gui;
                    plugin.Activate();

                    using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(context, null, treeViewControl))
                    using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                    {
                        // Call
                        contextMenuAdapter.Items[contextMenuRunAssessmentLevelCalculationsIndex].PerformClick();

                        // Assert
                        AssessmentLevelCalculationInput designWaterLevelCalculationInput = designWaterLevelCalculator.ReceivedInputs.First();

                        Assert.AreEqual(hydraulicBoundaryLocation.Id, designWaterLevelCalculationInput.HydraulicBoundaryLocationId);
                        Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(getNormFunc()), designWaterLevelCalculationInput.Beta);
                    }
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void CalculateDesignWaterLevelsFromContextMenu_HydraulicBoundaryDatabaseWithUsePreprocessorFalse_SendsRightInputToCalculationService()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite"),
                    CanUsePreprocessor = true,
                    UsePreprocessor = false,
                    PreprocessorDirectory = "InvalidPreprocessorDirectory"
                }
            };

            Func<double> getNormFunc = () => 0.01;

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("locationName");
            var hydraulicBoundaryLocationCalculations = new ObservableList<HydraulicBoundaryLocationCalculation>
            {
                new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation)
            };

            var context = new DesignWaterLevelCalculationsContext(hydraulicBoundaryLocationCalculations,
                                                                  assessmentSection,
                                                                  getNormFunc,
                                                                  "A");

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                gui.Stub(g => g.DocumentViewController).Return(mockRepository.Stub<IDocumentViewController>());

                var designWaterLevelCalculator = new TestDesignWaterLevelCalculator();
                var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(testDataPath, string.Empty)).Return(designWaterLevelCalculator);
                mockRepository.ReplayAll();

                DialogBoxHandler = (name, wnd) =>
                {
                    // Expect an activity dialog which is automatically closed
                };

                using (var plugin = new RingtoetsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = gui;
                    plugin.Activate();

                    using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(context, null, treeViewControl))
                    using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                    {
                        // Call
                        contextMenuAdapter.Items[contextMenuRunAssessmentLevelCalculationsIndex].PerformClick();

                        // Assert
                        AssessmentLevelCalculationInput designWaterLevelCalculationInput = designWaterLevelCalculator.ReceivedInputs.First();

                        Assert.AreEqual(hydraulicBoundaryLocation.Id, designWaterLevelCalculationInput.HydraulicBoundaryLocationId);
                        Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(getNormFunc()), designWaterLevelCalculationInput.Beta);
                    }
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenHydraulicBoundaryLocationThatSucceeds_CalculatingAssessmentLevelFromContextMenu_ThenLogMessagesAddedOutputSet()
        {
            // Given
            const string locationName = "locationName";
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite")
                }
            };

            const string categoryBoundaryName = "A";
            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation("locationName"));
            var hydraulicBoundaryLocationCalculations = new ObservableList<HydraulicBoundaryLocationCalculation>
            {
                hydraulicBoundaryLocationCalculation
            };

            var context = new DesignWaterLevelCalculationsContext(hydraulicBoundaryLocationCalculations,
                                                                  assessmentSection,
                                                                  () => 0.01,
                                                                  categoryBoundaryName);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                gui.Stub(g => g.DocumentViewController).Return(mockRepository.Stub<IDocumentViewController>());

                var calculator = new TestDesignWaterLevelCalculator
                {
                    Converged = false
                };
                var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(testDataPath, string.Empty)).Return(calculator);
                mockRepository.ReplayAll();

                DialogBoxHandler = (name, wnd) =>
                {
                    // Expect an activity dialog which is automatically closed
                };

                using (var plugin = new RingtoetsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = gui;
                    plugin.Activate();

                    using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(context, null, treeViewControl))
                    using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                    {
                        // When
                        Action call = () => contextMenuAdapter.Items[contextMenuRunAssessmentLevelCalculationsIndex].PerformClick();

                        // Then
                        TestHelper.AssertLogMessages(call, messages =>
                        {
                            string[] msgs = messages.ToArray();
                            Assert.AreEqual(8, msgs.Length);
                            Assert.AreEqual($"Waterstand berekenen voor locatie '{locationName}' (Categorie {categoryBoundaryName}) is gestart.", msgs[0]);
                            CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                            CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                            CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[3]);
                            Assert.AreEqual($"Waterstand berekening voor locatie '{locationName}' (Categorie {categoryBoundaryName}) is niet geconvergeerd.", msgs[4]);
                            StringAssert.StartsWith("Waterstand berekening is uitgevoerd op de tijdelijke locatie", msgs[5]);
                            CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[6]);
                            Assert.AreEqual($"Waterstand berekenen voor locatie '{locationName}' (Categorie {categoryBoundaryName}) is gelukt.", msgs[7]);
                        });

                        HydraulicBoundaryLocationCalculationOutput output = hydraulicBoundaryLocationCalculation.Output;
                        Assert.AreEqual(calculator.DesignWaterLevel, output.Result, output.Result.GetAccuracy());
                        Assert.AreEqual(CalculationConvergence.CalculatedNotConverged, output.CalculationConvergence);
                    }
                }
            }

            mockRepository.VerifyAll();
        }

        public override void Setup()
        {
            mockRepository = new MockRepository();
        }

        private static TreeNodeInfo GetInfo(RingtoetsPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(DesignWaterLevelCalculationsContext));
        }
    }
}