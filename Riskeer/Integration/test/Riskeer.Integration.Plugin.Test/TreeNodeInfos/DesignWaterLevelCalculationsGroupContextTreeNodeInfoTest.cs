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
using System.Collections.Generic;
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
using Core.Common.Gui.Forms.ViewHost;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
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
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class DesignWaterLevelCalculationsGroupContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuRunDesignWaterLevelCalculationsIndex = 0;
        private const int contextMenuClearIllustrationPointsIndex = 2;
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Service, "HydraRingCalculation");

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
        public void Text_Always_ReturnsSetName()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                string text = info.Text(null);

                // Assert
                Assert.AreEqual("Waterstanden", text);
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
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            IAssessmentSection assessmentSection = new AssessmentSectionStub();

            var mockRepository = new MockRepository();
            var menuBuilder = mockRepository.StrictMock<IContextMenuBuilder>();
            using (mockRepository.Ordered())
            {
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.Build()).Return(null);
            }

            var nodeData = new DesignWaterLevelCalculationsGroupContext(new ObservableList<HydraulicBoundaryLocation>(),
                                                                        assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mockRepository.Stub<IMainWindow>());
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
        public void ContextMenuStrip_HydraulicBoundaryDatabaseNotLinked_ContextMenuItemCalculateAllDisabledAndTooltipSet()
        {
            // Setup
            IAssessmentSection assessmentSection = new AssessmentSectionStub();

            var nodeData = new DesignWaterLevelCalculationsGroupContext(new ObservableList<HydraulicBoundaryLocation>(),
                                                                        assessmentSection);

            var mockRepository = new MockRepository();
            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                mockRepository.ReplayAll();

                using (var plugin = new RiskeerPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = gui;

                    // Precondition
                    Assert.IsFalse(assessmentSection.HydraulicBoundaryDatabase.IsLinked());

                    // Call
                    using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                    {
                        // Assert
                        ToolStripItem contextMenuItem = contextMenu.Items[contextMenuRunDesignWaterLevelCalculationsIndex];

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
            IAssessmentSection assessmentSection = new AssessmentSectionStub();
            assessmentSection.HydraulicBoundaryDatabase.FilePath = "invalidFilePath";

            var nodeData = new DesignWaterLevelCalculationsGroupContext(new ObservableList<HydraulicBoundaryLocation>(),
                                                                        assessmentSection);

            var mockRepository = new MockRepository();
            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
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
                        ToolStripItem contextMenuItem = contextMenu.Items[contextMenuRunDesignWaterLevelCalculationsIndex];

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
            IAssessmentSection assessmentSection = new AssessmentSectionStub();
            assessmentSection.HydraulicBoundaryDatabase.FilePath = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                                                                           nameof(HydraulicBoundaryDatabase)),
                                                                                "complete.sqlite");

            var nodeData = new DesignWaterLevelCalculationsGroupContext(new ObservableList<HydraulicBoundaryLocation>(),
                                                                        assessmentSection);

            var mockRepository = new MockRepository();
            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
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

                        TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuRunDesignWaterLevelCalculationsIndex,
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

            var nodeData = new DesignWaterLevelCalculationsGroupContext(new ObservableList<HydraulicBoundaryLocation>(), assessmentSection);

            var mockRepository = new MockRepository();
            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
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
        public void ContextMenuStrip_WaterLevelCalculationsWithoutIllustrationPoints_ContextMenuItemClearAllIllustrationPointsDisabled()
        {
            // Setup
            IAssessmentSection assessmentSection = GetConfiguredAssessmentSectionWithHydraulicBoundaryLocationCalculations();

            var nodeData = new DesignWaterLevelCalculationsGroupContext(new ObservableList<HydraulicBoundaryLocation>(), assessmentSection);

            var mockRepository = new MockRepository();
            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
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
        [TestCaseSource(nameof(GetWaveHeightCalculations))]
        public void ContextMenuStrip_WaveHeightCalculationsWithIllustrationPointsAndWaterLevelCalculationsWithoutIllustrationPoints_ContextMenuItemClearAllIllustrationPointsDisabled(
            Func<IAssessmentSection, HydraulicBoundaryLocationCalculation> getWaveHeightCalculationFunc)
        {
            // Setup
            var random = new Random(21);

            IAssessmentSection assessmentSection = GetConfiguredAssessmentSectionWithHydraulicBoundaryLocationCalculations();
            HydraulicBoundaryLocationCalculation calculation = getWaveHeightCalculationFunc(assessmentSection);
            calculation.Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble(), new TestGeneralResultSubMechanismIllustrationPoint());

            var nodeData = new DesignWaterLevelCalculationsGroupContext(new ObservableList<HydraulicBoundaryLocation>(), assessmentSection);

            var mockRepository = new MockRepository();
            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
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
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenValidCalculations_WhenCalculatingAllFromContextMenu_ThenLogMessagesAddedOutputSet()
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
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("locationName");
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            var context = new DesignWaterLevelCalculationsGroupContext(new ObservableList<HydraulicBoundaryLocation>(),
                                                                       assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                gui.Stub(g => g.DocumentViewController).Return(mockRepository.Stub<IDocumentViewController>());
                gui.Stub(g => g.ViewCommands).Return(mockRepository.Stub<IViewCommands>());

                var designWaterLevelCalculator = new TestDesignWaterLevelCalculator
                {
                    Converged = false
                };
                var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                                 .WhenCalled(invocation =>
                                 {
                                     HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                         HydraulicBoundaryCalculationSettingsFactory.CreateSettings(assessmentSection.HydraulicBoundaryDatabase),
                                         (HydraRingCalculationSettings) invocation.Arguments[0]);
                                 })
                                 .Return(designWaterLevelCalculator)
                                 .Repeat
                                 .Times(4);
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
                        Action call = () => contextMenuAdapter.Items[contextMenuRunDesignWaterLevelCalculationsIndex].PerformClick();

                        // Then
                        TestHelper.AssertLogMessages(call, messages =>
                        {
                            string[] msgs = messages.ToArray();
                            Assert.AreEqual(32, msgs.Length);

                            const string calculationTypeDisplayName = "Waterstand";
                            const string calculationDisplayName = "Waterstand berekening";

                            HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                                hydraulicBoundaryLocation.Name, calculationTypeDisplayName, calculationDisplayName, "A+", msgs, 0);
                            HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                                hydraulicBoundaryLocation.Name, calculationTypeDisplayName, calculationDisplayName, "A", msgs, 8);
                            HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                                hydraulicBoundaryLocation.Name, calculationTypeDisplayName, calculationDisplayName, "B", msgs, 16);
                            HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                                hydraulicBoundaryLocation.Name, calculationTypeDisplayName, calculationDisplayName, "C", msgs, 24);
                        });

                        AssertHydraulicBoundaryLocationCalculationOutput(designWaterLevelCalculator, assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.Single().Output);
                        AssertHydraulicBoundaryLocationCalculationOutput(designWaterLevelCalculator, assessmentSection.WaterLevelCalculationsForSignalingNorm.Single().Output);
                        AssertHydraulicBoundaryLocationCalculationOutput(designWaterLevelCalculator, assessmentSection.WaterLevelCalculationsForLowerLimitNorm.Single().Output);
                        AssertHydraulicBoundaryLocationCalculationOutput(designWaterLevelCalculator, assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm.Single().Output);
                    }
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnsChildrenOfData()
        {
            // Setup
            const double signalingNorm = 0.002;
            const double lowerLimitNorm = 0.005;

            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.FailureMechanismContribution.LowerLimitNorm = lowerLimitNorm;
            assessmentSection.FailureMechanismContribution.SignalingNorm = signalingNorm;

            var locations = new ObservableList<HydraulicBoundaryLocation>();
            var calculationsGroupContext = new DesignWaterLevelCalculationsGroupContext(locations, assessmentSection);

            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                object[] childNodeObjects = info.ChildNodeObjects(calculationsGroupContext);

                // Assert
                Assert.AreEqual(4, childNodeObjects.Length);

                DesignWaterLevelCalculationsContext[] calculationsContexts = childNodeObjects.OfType<DesignWaterLevelCalculationsContext>().ToArray();
                Assert.AreEqual(4, calculationsContexts.Length);

                Assert.IsTrue(calculationsContexts.All(c => ReferenceEquals(assessmentSection, c.AssessmentSection)));

                Assert.AreEqual("A+", calculationsContexts[0].CategoryBoundaryName);
                Assert.AreSame(assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm, calculationsContexts[0].WrappedData);
                Assert.AreEqual(signalingNorm / 30, calculationsContexts[0].GetNormFunc());

                Assert.AreEqual("A", calculationsContexts[1].CategoryBoundaryName);
                Assert.AreSame(assessmentSection.WaterLevelCalculationsForSignalingNorm, calculationsContexts[1].WrappedData);
                Assert.AreEqual(signalingNorm, calculationsContexts[1].GetNormFunc());

                Assert.AreEqual("B", calculationsContexts[2].CategoryBoundaryName);
                Assert.AreSame(assessmentSection.WaterLevelCalculationsForLowerLimitNorm, calculationsContexts[2].WrappedData);
                Assert.AreEqual(lowerLimitNorm, calculationsContexts[2].GetNormFunc());

                Assert.AreEqual("C", calculationsContexts[3].CategoryBoundaryName);
                Assert.AreSame(assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm, calculationsContexts[3].WrappedData);
                Assert.AreEqual(lowerLimitNorm * 30, calculationsContexts[3].GetNormFunc());
            }
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

            HydraulicBoundaryLocationCalculation unaffectedCalculation = assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm.First();
            unaffectedCalculation.Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble(), new TestGeneralResultSubMechanismIllustrationPoint());

            HydraulicBoundaryLocationCalculation[] calculationsWithOutput = GetAllWaterLevelCalculationsWithOutput(assessmentSection).ToArray();

            var messageBoxText = "";
            DialogBoxHandler = (name, wnd) =>
            {
                var helper = new MessageBoxTester(wnd);
                messageBoxText = helper.Text;

                helper.ClickCancel();
            };

            var context = new DesignWaterLevelCalculationsGroupContext(new ObservableList<HydraulicBoundaryLocation>(), assessmentSection);

            var mockRepository = new MockRepository();
            using (var treeViewControl = new TreeViewControl())
            {
                var affectedCalculationObserver = mockRepository.StrictMock<IObserver>();
                var unAffectedCalculationObserver = mockRepository.StrictMock<IObserver>();

                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mockRepository.ReplayAll();

                calculation.Attach(affectedCalculationObserver);
                unaffectedCalculation.Attach(unAffectedCalculationObserver);

                using (var plugin = new RiskeerPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = gui;

                    using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(context, null, treeViewControl))
                    {
                        // When
                        contextMenuAdapter.Items[contextMenuClearIllustrationPointsIndex].PerformClick();

                        // Then
                        const string expectedMessage = "Weet u zeker dat u alle berekende illustratiepunten bij 'Waterstanden' wilt wissen?";
                        Assert.AreEqual(expectedMessage, messageBoxText);

                        Assert.IsTrue(calculationsWithOutput.All(calc => calc.HasOutput));
                        Assert.IsTrue(calculation.Output.HasGeneralResult);

                        Assert.IsTrue(unaffectedCalculation.Output.HasGeneralResult);
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

            HydraulicBoundaryLocationCalculation unaffectedCalculation = assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm.First();
            unaffectedCalculation.Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble(), new TestGeneralResultSubMechanismIllustrationPoint());

            HydraulicBoundaryLocationCalculation[] calculationsWithOutput = GetAllWaterLevelCalculationsWithOutput(assessmentSection).ToArray();

            var messageBoxText = "";
            DialogBoxHandler = (name, wnd) =>
            {
                var helper = new MessageBoxTester(wnd);
                messageBoxText = helper.Text;

                helper.ClickOk();
            };

            var context = new DesignWaterLevelCalculationsGroupContext(new ObservableList<HydraulicBoundaryLocation>(), assessmentSection);

            var mockRepository = new MockRepository();
            using (var treeViewControl = new TreeViewControl())
            {
                var affectedCalculationObserver = mockRepository.StrictMock<IObserver>();
                affectedCalculationObserver.Expect(o => o.UpdateObserver());
                var unAffectedCalculationObserver = mockRepository.StrictMock<IObserver>();

                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mockRepository.ReplayAll();

                calculation.Attach(affectedCalculationObserver);
                unaffectedCalculation.Attach(unAffectedCalculationObserver);

                using (var plugin = new RiskeerPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = gui;

                    using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(context, null, treeViewControl))
                    {
                        // When
                        contextMenuAdapter.Items[contextMenuClearIllustrationPointsIndex].PerformClick();

                        // Then
                        const string expectedMessage = "Weet u zeker dat u alle berekende illustratiepunten bij 'Waterstanden' wilt wissen?";
                        Assert.AreEqual(expectedMessage, messageBoxText);

                        Assert.IsTrue(calculationsWithOutput.All(calc => calc.HasOutput));
                        Assert.IsFalse(calculation.Output.HasGeneralResult);

                        Assert.IsTrue(unaffectedCalculation.Output.HasGeneralResult);
                    }
                }
            }

            mockRepository.VerifyAll();
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

            assessmentSection.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryLocations, true);
            return assessmentSection;
        }

        private static IEnumerable<HydraulicBoundaryLocationCalculation> GetAllWaterLevelCalculationsWithOutput(IAssessmentSection assessmentSection)
        {
            return assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm
                                    .Concat(assessmentSection.WaterLevelCalculationsForSignalingNorm)
                                    .Concat(assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm)
                                    .Concat(assessmentSection.WaterLevelCalculationsForLowerLimitNorm)
                                    .Where(calc => calc.HasOutput);
        }

        private static IEnumerable<TestCaseData> GetWaterLevelCalculations()
        {
            yield return new TestCaseData(new Func<IAssessmentSection, HydraulicBoundaryLocationCalculation>(section => section.WaterLevelCalculationsForFactorizedSignalingNorm.First()));
            yield return new TestCaseData(new Func<IAssessmentSection, HydraulicBoundaryLocationCalculation>(section => section.WaterLevelCalculationsForSignalingNorm.First()));
            yield return new TestCaseData(new Func<IAssessmentSection, HydraulicBoundaryLocationCalculation>(section => section.WaterLevelCalculationsForFactorizedLowerLimitNorm.First()));
            yield return new TestCaseData(new Func<IAssessmentSection, HydraulicBoundaryLocationCalculation>(section => section.WaterLevelCalculationsForLowerLimitNorm.First()));
        }

        private static IEnumerable<TestCaseData> GetWaveHeightCalculations()
        {
            yield return new TestCaseData(new Func<IAssessmentSection, HydraulicBoundaryLocationCalculation>(section => section.WaveHeightCalculationsForFactorizedSignalingNorm.First()));
            yield return new TestCaseData(new Func<IAssessmentSection, HydraulicBoundaryLocationCalculation>(section => section.WaveHeightCalculationsForSignalingNorm.First()));
            yield return new TestCaseData(new Func<IAssessmentSection, HydraulicBoundaryLocationCalculation>(section => section.WaveHeightCalculationsForFactorizedLowerLimitNorm.First()));
            yield return new TestCaseData(new Func<IAssessmentSection, HydraulicBoundaryLocationCalculation>(section => section.WaveHeightCalculationsForLowerLimitNorm.First()));
        }

        private static TreeNodeInfo GetInfo(RiskeerPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(DesignWaterLevelCalculationsGroupContext));
        }
    }
}