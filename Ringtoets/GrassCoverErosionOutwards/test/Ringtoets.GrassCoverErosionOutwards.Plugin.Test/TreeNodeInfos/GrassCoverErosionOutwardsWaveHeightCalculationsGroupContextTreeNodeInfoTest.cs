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
using System.Collections.Generic;
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
using Ringtoets.Common.Service;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.HydraRing.Calculation.Calculator;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionOutwards.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveHeightCalculationsGroupContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuRunWaveHeightCalculationsIndex = 0;
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");

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
                Assert.IsNotNull(info.ChildNodeObjects);
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
        public void Text_Always_ReturnsSetName()
        {
            // Setup
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                string text = info.Text(null);

                // Assert
                Assert.AreEqual("Golfhoogtes", text);
            }
        }

        [Test]
        public void Image_Always_ReturnsGeneralFolderIcon()
        {
            // Setup
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Image image = info.Image(null);

                // Assert
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GeneralFolderIcon, image);
            }
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mockRepository);
            var menuBuilder = mockRepository.StrictMock<IContextMenuBuilder>();

            using (mockRepository.Ordered())
            {
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.Build()).Return(null);
            }

            var nodeData = new GrassCoverErosionOutwardsWaveHeightCalculationsGroupContext(new ObservableList<HydraulicBoundaryLocation>(),
                                                                                           new GrassCoverErosionOutwardsFailureMechanism(),
                                                                                           assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                mockRepository.ReplayAll();

                using (var plugin = new GrassCoverErosionOutwardsPlugin())
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
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mockRepository);

            var nodeData = new GrassCoverErosionOutwardsWaveHeightCalculationsGroupContext(new ObservableList<HydraulicBoundaryLocation>(),
                                                                                           new GrassCoverErosionOutwardsFailureMechanism
                                                                                           {
                                                                                               Contribution = 5
                                                                                           },
                                                                                           assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mockRepository.ReplayAll();

                using (var plugin = new GrassCoverErosionOutwardsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    plugin.Gui = gui;

                    // Call
                    using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                    {
                        // Assert
                        ToolStripItem contextMenuItem = contextMenu.Items[contextMenuRunWaveHeightCalculationsIndex];

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
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(null, mockRepository, "invalidFilePath");

            var nodeData = new GrassCoverErosionOutwardsWaveHeightCalculationsGroupContext(new ObservableList<HydraulicBoundaryLocation>(),
                                                                                           new GrassCoverErosionOutwardsFailureMechanism
                                                                                           {
                                                                                               Contribution = 5
                                                                                           },
                                                                                           assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mockRepository.ReplayAll();

                using (var plugin = new GrassCoverErosionOutwardsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    plugin.Gui = gui;

                    // Call
                    using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                    {
                        // Assert
                        ToolStripItem contextMenuItem = contextMenu.Items[contextMenuRunWaveHeightCalculationsIndex];

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
        public void ContextMenuStrip_FailureMechanismContributionZero_ContextMenuItemCalculateAllDisabledAndTooltipSet()
        {
            // Setup
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mockRepository);

            var context = new GrassCoverErosionOutwardsWaveHeightCalculationsGroupContext(new ObservableList<HydraulicBoundaryLocation>(),
                                                                                          new GrassCoverErosionOutwardsFailureMechanism(),
                                                                                          assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                mockRepository.ReplayAll();

                using (var plugin = new GrassCoverErosionOutwardsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = gui;
                    plugin.Activate();

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
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
            {
                FilePath = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "HydraulicBoundaryDatabaseImporter"), "complete.sqlite")
            });

            var nodeData = new GrassCoverErosionOutwardsWaveHeightCalculationsGroupContext(new ObservableList<HydraulicBoundaryLocation>(),
                                                                                           new GrassCoverErosionOutwardsFailureMechanism
                                                                                           {
                                                                                               Contribution = 5
                                                                                           },
                                                                                           assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mockRepository.ReplayAll();

                using (var plugin = new GrassCoverErosionOutwardsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    plugin.Gui = gui;

                    // Call
                    using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                    {
                        // Assert
                        const string expectedItemText = @"Alles be&rekenen";
                        const string expectedItemTooltip = @"Alle golfhoogten berekenen.";

                        TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuRunWaveHeightCalculationsIndex,
                                                                      expectedItemText, expectedItemTooltip, RingtoetsCommonFormsResources.CalculateAllIcon);
                    }
                }
            }

            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void CalculateWaveHeightsFromContextMenu_AllRequiredInputSet_SendsRightInputToCalculationService()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = new AssessmentSectionStub
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite")
                }
            };

            var hydraulicBoundaryLocation1 = new TestHydraulicBoundaryLocation("locationName 1");
            var hydraulicBoundaryLocation2 = new TestHydraulicBoundaryLocation("locationName 2");
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation1,
                hydraulicBoundaryLocation2
            });

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 5
            };
            failureMechanism.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation1,
                hydraulicBoundaryLocation2
            });

            var context = new GrassCoverErosionOutwardsWaveHeightCalculationsGroupContext(new ObservableList<HydraulicBoundaryLocation>(),
                                                                                          failureMechanism,
                                                                                          assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                gui.Stub(g => g.DocumentViewController).Return(mockRepository.Stub<IDocumentViewController>());

                var waveHeightCalculator = new TestWaveHeightCalculator();
                var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(testDataPath, string.Empty)).Return(waveHeightCalculator).Repeat.Times(10);
                mockRepository.ReplayAll();

                DialogBoxHandler = (name, wnd) =>
                {
                    // Expect an activity dialog which is automatically closed
                };

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
                        double mechanismSpecificFactorizedSignalingNorm = GetExpectedNorm(failureMechanism,
                                                                                          () => assessmentSection.FailureMechanismContribution.SignalingNorm / 30);
                        AssertHydraRingCalculationInput(hydraulicBoundaryLocation1,
                                                        mechanismSpecificFactorizedSignalingNorm,
                                                        waveHeightCalculator.ReceivedInputs.ElementAt(0));
                        AssertHydraRingCalculationInput(hydraulicBoundaryLocation2,
                                                        mechanismSpecificFactorizedSignalingNorm,
                                                        waveHeightCalculator.ReceivedInputs.ElementAt(1));

                        double mechanismSpecificSignalingNorm = GetExpectedNorm(failureMechanism, () => assessmentSection.FailureMechanismContribution.SignalingNorm);
                        AssertHydraRingCalculationInput(hydraulicBoundaryLocation1,
                                                        mechanismSpecificSignalingNorm,
                                                        waveHeightCalculator.ReceivedInputs.ElementAt(2));
                        AssertHydraRingCalculationInput(hydraulicBoundaryLocation2,
                                                        mechanismSpecificSignalingNorm,
                                                        waveHeightCalculator.ReceivedInputs.ElementAt(3));

                        double mechanismSpecificLowerLimitNorm = GetExpectedNorm(failureMechanism, () => assessmentSection.FailureMechanismContribution.LowerLimitNorm);
                        AssertHydraRingCalculationInput(hydraulicBoundaryLocation1,
                                                        mechanismSpecificLowerLimitNorm,
                                                        waveHeightCalculator.ReceivedInputs.ElementAt(4));
                        AssertHydraRingCalculationInput(hydraulicBoundaryLocation2,
                                                        mechanismSpecificLowerLimitNorm,
                                                        waveHeightCalculator.ReceivedInputs.ElementAt(5));

                        double lowerLimitNorm = assessmentSection.FailureMechanismContribution.LowerLimitNorm;
                        AssertHydraRingCalculationInput(hydraulicBoundaryLocation1,
                                                        lowerLimitNorm,
                                                        waveHeightCalculator.ReceivedInputs.ElementAt(6));
                        AssertHydraRingCalculationInput(hydraulicBoundaryLocation2,
                                                        lowerLimitNorm,
                                                        waveHeightCalculator.ReceivedInputs.ElementAt(7));

                        double factorizedLowerLimitNorm = lowerLimitNorm * 30;
                        AssertHydraRingCalculationInput(hydraulicBoundaryLocation1,
                                                        factorizedLowerLimitNorm,
                                                        waveHeightCalculator.ReceivedInputs.ElementAt(8));
                        AssertHydraRingCalculationInput(hydraulicBoundaryLocation2,
                                                        factorizedLowerLimitNorm,
                                                        waveHeightCalculator.ReceivedInputs.ElementAt(9));
                    }
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenHydraulicBoundaryLocationThatSucceeds_CalculatingWaveHeightFromContextMenu_ThenLogMessagesAddedOutputSet()
        {
            // Given
            var mockRepository = new MockRepository();
            var assessmentSection = new AssessmentSectionStub
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite")
                }
            };

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("locationName");
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 5
            };
            failureMechanism.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            var context = new GrassCoverErosionOutwardsWaveHeightCalculationsGroupContext(new ObservableList<HydraulicBoundaryLocation>(),
                                                                                          failureMechanism,
                                                                                          assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                gui.Stub(g => g.DocumentViewController).Return(mockRepository.Stub<IDocumentViewController>());

                var waveHeightCalculator = new TestWaveHeightCalculator
                {
                    Converged = false
                };
                var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(testDataPath, string.Empty)).Return(waveHeightCalculator).Repeat.Times(5);
                mockRepository.ReplayAll();

                DialogBoxHandler = (name, wnd) =>
                {
                    // Expect an activity dialog which is automatically closed
                };

                using (var plugin = new GrassCoverErosionOutwardsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = gui;
                    plugin.Activate();

                    using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(context, null, treeViewControl))
                    using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                    {
                        // When
                        Action call = () => contextMenuAdapter.Items[contextMenuRunWaveHeightCalculationsIndex].PerformClick();

                        // Then
                        TestHelper.AssertLogMessages(call, messages =>
                        {
                            string[] msgs = messages.ToArray();
                            Assert.AreEqual(40, msgs.Length);
                            AssertHydraulicBoundaryLocationCalculationMessages(hydraulicBoundaryLocation, msgs, 0, "Iv->IIv");
                            AssertHydraulicBoundaryLocationCalculationMessages(hydraulicBoundaryLocation, msgs, 8, "IIv->IIIv");
                            AssertHydraulicBoundaryLocationCalculationMessages(hydraulicBoundaryLocation, msgs, 16, "IIIv->IVv");
                            AssertHydraulicBoundaryLocationCalculationMessages(hydraulicBoundaryLocation, msgs, 24, "IVv->Vv");
                            AssertHydraulicBoundaryLocationCalculationMessages(hydraulicBoundaryLocation, msgs, 32, "Vv->VIv");
                        });

                        AssertHydraulicBoundaryLocationCalculationOutput(waveHeightCalculator, failureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm.Single().Output);
                        AssertHydraulicBoundaryLocationCalculationOutput(waveHeightCalculator, failureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm.Single().Output);
                        AssertHydraulicBoundaryLocationCalculationOutput(waveHeightCalculator, failureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm.Single().Output);
                        AssertHydraulicBoundaryLocationCalculationOutput(waveHeightCalculator, assessmentSection.WaveHeightCalculationsForLowerLimitNorm.Single().Output);
                        AssertHydraulicBoundaryLocationCalculationOutput(waveHeightCalculator, assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm.Single().Output);
                    }
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnsExpectedChildData()
        {
            // Setup
            const double signalingNorm = 0.002;
            const double lowerLimitNorm = 0.005;

            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.FailureMechanismContribution.LowerLimitNorm = lowerLimitNorm;
            assessmentSection.FailureMechanismContribution.SignalingNorm = signalingNorm;

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 5
            };

            var locations = new ObservableList<HydraulicBoundaryLocation>();
            var calculationsGroupContext = new GrassCoverErosionOutwardsWaveHeightCalculationsGroupContext(locations, failureMechanism, assessmentSection);

            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                object[] childNodeObjects = info.ChildNodeObjects(calculationsGroupContext);

                // Assert
                Assert.AreEqual(5, childNodeObjects.Length);

                GrassCoverErosionOutwardsWaveHeightCalculationsContext[] calculationsContexts = childNodeObjects.OfType<GrassCoverErosionOutwardsWaveHeightCalculationsContext>().ToArray();
                Assert.AreEqual(5, calculationsContexts.Length);

                Assert.IsTrue(calculationsContexts.All(c => ReferenceEquals(assessmentSection, c.AssessmentSection)));
                Assert.IsTrue(calculationsContexts.All(c => ReferenceEquals(failureMechanism, c.FailureMechanism)));

                Assert.AreEqual("Iv->IIv", calculationsContexts[0].CategoryBoundaryName);
                Assert.AreSame(failureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm, calculationsContexts[0].WrappedData);
                Assert.AreEqual(GetExpectedNorm(failureMechanism, () => signalingNorm / 30), calculationsContexts[0].GetNormFunc(), 1e-6);

                Assert.AreEqual("IIv->IIIv", calculationsContexts[1].CategoryBoundaryName);
                Assert.AreSame(failureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm, calculationsContexts[1].WrappedData);
                Assert.AreEqual(GetExpectedNorm(failureMechanism, () => signalingNorm), calculationsContexts[1].GetNormFunc());

                Assert.AreEqual("IIIv->IVv", calculationsContexts[2].CategoryBoundaryName);
                Assert.AreSame(failureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm, calculationsContexts[2].WrappedData);
                Assert.AreEqual(GetExpectedNorm(failureMechanism, () => lowerLimitNorm), calculationsContexts[2].GetNormFunc());

                Assert.AreEqual("IVv->Vv", calculationsContexts[3].CategoryBoundaryName);
                Assert.AreSame(assessmentSection.WaveHeightCalculationsForLowerLimitNorm, calculationsContexts[3].WrappedData);
                Assert.AreEqual(lowerLimitNorm, calculationsContexts[3].GetNormFunc());

                Assert.AreEqual("Vv->VIv", calculationsContexts[4].CategoryBoundaryName);
                Assert.AreSame(assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm, calculationsContexts[4].WrappedData);
                Assert.AreEqual(lowerLimitNorm * 30, calculationsContexts[4].GetNormFunc());
            }
        }

        private static void AssertHydraulicBoundaryLocationCalculationOutput(IWaveHeightCalculator waveHeightCalculator,
                                                                             HydraulicBoundaryLocationCalculationOutput actualOutput)
        {
            Assert.AreEqual(waveHeightCalculator.WaveHeight, actualOutput.Result, actualOutput.Result.GetAccuracy());
            Assert.AreEqual(CalculationConvergence.CalculatedNotConverged, actualOutput.CalculationConvergence);
        }

        private static double GetExpectedNorm(GrassCoverErosionOutwardsFailureMechanism failureMechanism, Func<double> getNormFunc)
        {
            return RingtoetsCommonDataCalculationService.ProfileSpecificRequiredProbability(
                getNormFunc(),
                failureMechanism.Contribution,
                failureMechanism.GeneralInput.N);
        }

        private static TreeNodeInfo GetInfo(GrassCoverErosionOutwardsPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(GrassCoverErosionOutwardsWaveHeightCalculationsGroupContext));
        }

        private static void AssertHydraulicBoundaryLocationCalculationMessages(HydraulicBoundaryLocation hydraulicBoundaryLocation,
                                                                               IEnumerable<string> messages,
                                                                               int startIndex,
                                                                               string categoryName)
        {
            Assert.AreEqual($"Golfhoogte berekenen voor locatie '{hydraulicBoundaryLocation.Name}' (Categorie {categoryName}) is gestart.", messages.ElementAt(startIndex));
            CalculationServiceTestHelper.AssertValidationStartMessage(messages.ElementAt(startIndex + 1));
            CalculationServiceTestHelper.AssertValidationEndMessage(messages.ElementAt(startIndex + 2));
            CalculationServiceTestHelper.AssertCalculationStartMessage(messages.ElementAt(startIndex + 3));
            Assert.AreEqual($"Golfhoogte berekening voor locatie '{hydraulicBoundaryLocation.Name}' (Categorie {categoryName}) is niet geconvergeerd.", messages.ElementAt(startIndex + 4));
            StringAssert.StartsWith("Golfhoogte berekening is uitgevoerd op de tijdelijke locatie", messages.ElementAt(startIndex + 5));
            CalculationServiceTestHelper.AssertCalculationEndMessage(messages.ElementAt(startIndex + 6));
            Assert.AreEqual($"Golfhoogte berekenen voor locatie '{hydraulicBoundaryLocation.Name}' (Categorie {categoryName}) is gelukt.", messages.ElementAt(startIndex + 7));
        }

        private static void AssertHydraRingCalculationInput(HydraulicBoundaryLocation hydraulicBoundaryLocation,
                                                            double norm,
                                                            HydraRingCalculationInput actualCalculationInput)
        {
            Assert.AreEqual(hydraulicBoundaryLocation.Id, actualCalculationInput.HydraulicBoundaryLocationId);
            Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(norm), actualCalculationInput.Beta);
        }
    }
}