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
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionOutwards.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveHeightLocationsContextTreeNodeInfoTest
    {
        private const int contextMenuRunWaveHeightCalculationsIndex = 2;
        private MockRepository mockRepository;

        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

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
                Assert.IsNull(info.IsChecked);
                Assert.IsNull(info.OnNodeChecked);
                Assert.IsNull(info.CanDrag);
                Assert.IsNull(info.CanDrop);
                Assert.IsNull(info.CanInsert);
                Assert.IsNull(info.OnDrop);
            }
        }

        [Test]
        public void Text_Always_ReturnName()
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();
            var context = new GrassCoverErosionOutwardsWaveHeightLocationsContext(
                new ObservableList<HydraulicBoundaryLocation>(),
                assessmentSection,
                new GrassCoverErosionOutwardsFailureMechanism());

            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);
                // Call
                string nodeText = info.Text(context);

                // Assert
                Assert.AreEqual("Golfhoogtes bij doorsnede-eis", nodeText);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Image_Always_ReturnGenericInputOutputIcon()
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();
            var context = new GrassCoverErosionOutwardsWaveHeightLocationsContext(
                new ObservableList<HydraulicBoundaryLocation>(),
                assessmentSection,
                new GrassCoverErosionOutwardsFailureMechanism());
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Image icon = info.Image(context);

                // Assert
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GenericInputOutputIcon, icon);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();

            using (var treeViewControl = new TreeViewControl())
            {
                using (var plugin = new GrassCoverErosionOutwardsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    var context = new GrassCoverErosionOutwardsWaveHeightLocationsContext(
                        new ObservableList<HydraulicBoundaryLocation>(),
                        assessmentSection,
                        new GrassCoverErosionOutwardsFailureMechanism());

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

                    var gui = mockRepository.Stub<IGui>();
                    gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(menuBuilder);

                    mockRepository.ReplayAll();

                    plugin.Gui = gui;

                    // Call
                    using (info.ContextMenuStrip(context, null, treeViewControl)) {}
                }
            }

            // Assert
            mockRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_HydraulicBoundaryDatabaseLinkedToInvalidFile_ContextMenuItemCalculateAllDisabledAndTooltipSet()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(null, mockRepository, "invalidFilePath");

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

                    var context = new GrassCoverErosionOutwardsWaveHeightLocationsContext(
                        new ObservableList<HydraulicBoundaryLocation>(),
                        assessmentSection,
                        new GrassCoverErosionOutwardsFailureMechanism
                        {
                            Contribution = 5
                        });

                    var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler,
                                                             importCommandHandler,
                                                             exportCommandHandler,
                                                             updateCommandHandler,
                                                             viewCommandsHandler,
                                                             context,
                                                             treeViewControl);

                    var gui = mockRepository.Stub<IGui>();
                    gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(menuBuilder);

                    mockRepository.ReplayAll();

                    plugin.Gui = gui;

                    // Call
                    using (ContextMenuStrip menu = info.ContextMenuStrip(context, null, treeViewControl))
                    {
                        // Assert
                        ToolStripItem contextMenuItem = menu.Items[contextMenuRunWaveHeightCalculationsIndex];

                        Assert.AreEqual("Alles be&rekenen", contextMenuItem.Text);
                        StringAssert.Contains("Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt.", contextMenuItem.ToolTipText);
                        TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.CalculateAllIcon, contextMenuItem.Image);
                        Assert.IsFalse(contextMenuItem.Enabled);
                    }
                }
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismContributionZero_ContextMenuItemCalculateAllDisabledAndTooltipSet()
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
            {
                FilePath = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "HydraulicBoundaryDatabaseImporter"), "complete.sqlite"),
                Version = "1.0"
            });

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

                    var context = new GrassCoverErosionOutwardsWaveHeightLocationsContext(
                        new ObservableList<HydraulicBoundaryLocation>(),
                        assessmentSection,
                        new GrassCoverErosionOutwardsFailureMechanism());

                    var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler,
                                                             importCommandHandler,
                                                             exportCommandHandler,
                                                             updateCommandHandler,
                                                             viewCommandsHandler,
                                                             context,
                                                             treeViewControl);

                    var gui = mockRepository.Stub<IGui>();
                    gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(menuBuilder);

                    mockRepository.ReplayAll();

                    plugin.Gui = gui;

                    // Call
                    using (ContextMenuStrip menu = info.ContextMenuStrip(context, null, treeViewControl))
                    {
                        // Assert
                        TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                      contextMenuRunWaveHeightCalculationsIndex,
                                                                      "Alles be&rekenen",
                                                                      "De bijdrage van dit toetsspoor is nul.",
                                                                      RingtoetsCommonFormsResources.CalculateAllIcon,
                                                                      false);
                    }
                }
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_AllRequiredInputSet_ContextMenuItemCalculateAllEnabled()
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
            {
                FilePath = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "HydraulicBoundaryDatabaseImporter"), "complete.sqlite"),
                Version = "1.0"
            });

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

                    var context = new GrassCoverErosionOutwardsWaveHeightLocationsContext(
                        new ObservableList<HydraulicBoundaryLocation>(),
                        assessmentSection,
                        new GrassCoverErosionOutwardsFailureMechanism
                        {
                            Contribution = 5
                        });

                    var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler,
                                                             importCommandHandler,
                                                             exportCommandHandler,
                                                             updateCommandHandler,
                                                             viewCommandsHandler,
                                                             context,
                                                             treeViewControl);

                    var gui = mockRepository.Stub<IGui>();
                    gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(menuBuilder);

                    mockRepository.ReplayAll();

                    plugin.Gui = gui;

                    // Call
                    using (ContextMenuStrip menu = info.ContextMenuStrip(context, null, treeViewControl))
                    {
                        // Assert
                        TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                      contextMenuRunWaveHeightCalculationsIndex,
                                                                      "Alles be&rekenen",
                                                                      "Alle golfhoogtes bij doorsnede-eis berekenen.",
                                                                      RingtoetsCommonFormsResources.CalculateAllIcon);
                    }
                }
            }
            mockRepository.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenHydraulicBoundaryLocationThatFails_CalculatingWaveHeightFromContextMenu_ThenLogMessagesAddedPreviousOutputNotAffected()
        {
            // Given
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 5
            };
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(
                failureMechanism, mockRepository, Path.Combine(testDataPath, "HRD ijsselmeer.sqlite"));

            HydraulicBoundaryLocation grassCoverErosionOutwardsHydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations[0];
            var context = new GrassCoverErosionOutwardsWaveHeightLocationsContext(new ObservableList<HydraulicBoundaryLocation>
            {
                grassCoverErosionOutwardsHydraulicBoundaryLocation
            }, assessmentSection, new GrassCoverErosionOutwardsFailureMechanism());

            using (var treeViewControl = new TreeViewControl())
            {
                using (var plugin = new GrassCoverErosionOutwardsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    var gui = mockRepository.Stub<IGui>();
                    gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                    gui.Stub(g => g.MainWindow).Return(mockRepository.Stub<IMainWindow>());

                    var waveHeightCalculator = new TestWaveHeightCalculator
                    {
                        EndInFailure = true
                    };
                    var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
                    calculatorFactory.Stub(cf => cf.CreateWaveHeightCalculator(testDataPath, string.Empty)).Return(waveHeightCalculator);
                    mockRepository.ReplayAll();

                    plugin.Gui = gui;
                    plugin.Activate();

                    using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(context, null, treeViewControl))
                    using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                    {
                        // When
                        contextMenuAdapter.Items[contextMenuRunWaveHeightCalculationsIndex].PerformClick();

                        // Then
                        Assert.IsNaN(grassCoverErosionOutwardsHydraulicBoundaryLocation.WaveHeight);
                        Assert.AreEqual(CalculationConvergence.NotCalculated, grassCoverErosionOutwardsHydraulicBoundaryLocation.WaveHeightCalculation.Output.CalculationConvergence);
                    }
                }
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateWaveHeightsFromContextMenu_HydraulicBoundaryDatabaseWithCanUsePreprocessorFalse_SendsRightInputToCalculationService()
        {
            // Setup
            string filePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 5
            };

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1300001, string.Empty, 0, 0);
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(
                failureMechanism, mockRepository, filePath);

            var context = new GrassCoverErosionOutwardsWaveHeightLocationsContext(new ObservableList<HydraulicBoundaryLocation>
            {
                hydraulicBoundaryLocation
            }, assessmentSection, failureMechanism);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mockRepository.Stub<IMainWindow>());

                var waveHeightCalculator = new TestWaveHeightCalculator();
                var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(testDataPath, string.Empty)).Return(waveHeightCalculator);
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
                        contextMenuAdapter.Items[contextMenuRunWaveHeightCalculationsIndex].PerformClick();

                        // Assert
                        WaveHeightCalculationInput waveHeightCalculationInput = waveHeightCalculator.ReceivedInputs.First();

                        Assert.AreEqual(hydraulicBoundaryLocation.Id, waveHeightCalculationInput.HydraulicBoundaryLocationId);
                        double expectedProbability = RingtoetsCommonDataCalculationService.ProfileSpecificRequiredProbability(
                            assessmentSection.FailureMechanismContribution.Norm,
                            failureMechanism.Contribution,
                            failureMechanism.GeneralInput.N);
                        Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(expectedProbability), waveHeightCalculationInput.Beta);
                    }
                }
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateWaveHeightsFromContextMenu_HydraulicBoundaryDatabaseWithUsePreprocessorTrue_SendsRightInputToCalculationService()
        {
            // Setup
            string filePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");
            string preprocessorDirectory = TestHelper.GetScratchPadPath();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 5
            };

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1300001, string.Empty, 0, 0);
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(
                failureMechanism, mockRepository, filePath);

            assessmentSection.HydraulicBoundaryDatabase.CanUsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.UsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.PreprocessorDirectory = preprocessorDirectory;

            var context = new GrassCoverErosionOutwardsWaveHeightLocationsContext(new ObservableList<HydraulicBoundaryLocation>
            {
                hydraulicBoundaryLocation
            }, assessmentSection, failureMechanism);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mockRepository.Stub<IMainWindow>());

                var waveHeightCalculator = new TestWaveHeightCalculator();
                var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(testDataPath, preprocessorDirectory)).Return(waveHeightCalculator);
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
                        contextMenuAdapter.Items[contextMenuRunWaveHeightCalculationsIndex].PerformClick();

                        // Assert
                        WaveHeightCalculationInput waveHeightCalculationInput = waveHeightCalculator.ReceivedInputs.First();

                        Assert.AreEqual(hydraulicBoundaryLocation.Id, waveHeightCalculationInput.HydraulicBoundaryLocationId);
                        double expectedProbability = RingtoetsCommonDataCalculationService.ProfileSpecificRequiredProbability(
                            assessmentSection.FailureMechanismContribution.Norm,
                            failureMechanism.Contribution,
                            failureMechanism.GeneralInput.N);
                        Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(expectedProbability), waveHeightCalculationInput.Beta);
                    }
                }
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateWaveHeightsFromContextMenu_HydraulicBoundaryDatabaseWithUsePreprocessorFalse_SendsRightInputToCalculationService()
        {
            // Setup
            string filePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 5
            };

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1300001, string.Empty, 0, 0);
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(
                failureMechanism, mockRepository, filePath);

            assessmentSection.HydraulicBoundaryDatabase.CanUsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.UsePreprocessor = false;
            assessmentSection.HydraulicBoundaryDatabase.PreprocessorDirectory = "InvalidPreprocessorDirectory";

            var context = new GrassCoverErosionOutwardsWaveHeightLocationsContext(new ObservableList<HydraulicBoundaryLocation>
            {
                hydraulicBoundaryLocation
            }, assessmentSection, failureMechanism);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mockRepository.Stub<IMainWindow>());

                var waveHeightCalculator = new TestWaveHeightCalculator();
                var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(testDataPath, string.Empty)).Return(waveHeightCalculator);
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
                        contextMenuAdapter.Items[contextMenuRunWaveHeightCalculationsIndex].PerformClick();

                        // Assert
                        WaveHeightCalculationInput waveHeightCalculationInput = waveHeightCalculator.ReceivedInputs.First();

                        Assert.AreEqual(hydraulicBoundaryLocation.Id, waveHeightCalculationInput.HydraulicBoundaryLocationId);
                        double expectedProbability = RingtoetsCommonDataCalculationService.ProfileSpecificRequiredProbability(
                            assessmentSection.FailureMechanismContribution.Norm,
                            failureMechanism.Contribution,
                            failureMechanism.GeneralInput.N);
                        Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(expectedProbability), waveHeightCalculationInput.Beta);
                    }
                }
            }
            mockRepository.VerifyAll();
        }

        private static TreeNodeInfo GetInfo(GrassCoverErosionOutwardsPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(GrassCoverErosionOutwardsWaveHeightLocationsContext));
        }
    }
}