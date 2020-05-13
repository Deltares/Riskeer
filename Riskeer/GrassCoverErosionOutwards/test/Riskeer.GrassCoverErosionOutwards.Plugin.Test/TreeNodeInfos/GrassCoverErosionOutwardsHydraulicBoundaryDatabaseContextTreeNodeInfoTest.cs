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
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Forms.ViewHost;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Plugin.TestUtil;
using Riskeer.Common.Service.TestUtil;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.GrassCoverErosionOutwards.Data.TestUtil;
using Riskeer.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;
using Riskeer.Revetment.Data;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.GrassCoverErosionOutwards.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class GrassCoverErosionOutwardsHydraulicBoundaryDatabaseContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuCalculateAllIndex = 2;
        private const int contextMenuClearIllustrationPointsIndex = 4;

        private MockRepository mockRepository;
        private GrassCoverErosionOutwardsPlugin plugin;
        private TreeNodeInfo info;

        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Forms, "HydraulicBoundaryDatabase");
        private static readonly string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            mockRepository.ReplayAll();

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
        public void Text_Always_ReturnName()
        {
            // Call
            string nodeText = info.Text(null);

            // Assert
            Assert.AreEqual("Hydraulische belastingen", nodeText);
        }

        [Test]
        public void Image_Always_ReturnFailureMechanismIcon()
        {
            // Call
            Image icon = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.GeneralFolderIcon, icon);
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSection = new AssessmentSectionStub();

                var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
                var context = new GrassCoverErosionOutwardsHydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase, failureMechanism, assessmentSection);

                var menuBuilder = mockRepository.StrictMock<IContextMenuBuilder>();
                using (mockRepository.Ordered())
                {
                    menuBuilder.Expect(mb => mb.AddExportItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.Build()).Return(null);
                }

                var gui = mockRepository.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                mockRepository.ReplayAll();

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(context, null, treeViewControl);
            }

            // Assert
            // Done in tearDown
        }

        [Test]
        public void ContextMenuStrip_AllRequiredInputSet_ContextMenuItemCalculateAllEnabled()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            var assessmentSection = new AssessmentSectionStub
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = validFilePath
                }
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);

            var nodeData = new GrassCoverErosionOutwardsHydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase,
                                                                                         failureMechanism,
                                                                                         assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                mockRepository.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndex,
                                                                  "Alles be&rekenen",
                                                                  "Voer alle berekeningen binnen deze map met berekeningen uit.",
                                                                  RiskeerCommonFormsResources.CalculateAllIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_HydraulicBoundaryDatabaseNotLinked_ContextMenuItemCalculateAllDisabledAndTooltipSet()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("Test");
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var assessmentSection = new AssessmentSectionStub();

            failureMechanism.AddHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            var context = new GrassCoverErosionOutwardsHydraulicBoundaryDatabaseContext(new HydraulicBoundaryDatabase(),
                                                                                        failureMechanism,
                                                                                        assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                mockRepository.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Assert
                    ToolStripItem contextMenuItem = contextMenu.Items[contextMenuCalculateAllIndex];

                    Assert.AreEqual("Alles be&rekenen", contextMenuItem.Text);
                    StringAssert.Contains("Er is geen hydraulische belastingendatabase geïmporteerd.", contextMenuItem.ToolTipText);
                    TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.CalculateAllIcon, contextMenuItem.Image);
                    Assert.IsFalse(contextMenuItem.Enabled);
                }
            }
        }

        [Test]
        [TestCaseSource(nameof(GetHydraulicBoundaryLocationCalculations))]
        public void ContextMenuStrip_HydraulicBoundaryLocationCalculationsWithIllustrationPoints_ContextMenuItemClearAllIllustrationPointsEnabledAndTooltipSet(
            Func<IAssessmentSection, GrassCoverErosionOutwardsFailureMechanism, HydraulicBoundaryLocationCalculation> getHydraulicBoundaryLocationCalculationFunc)
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(failureMechanism, assessmentSection, new[]
            {
                new TestHydraulicBoundaryLocation()
            });

            HydraulicBoundaryLocationCalculation calculation = getHydraulicBoundaryLocationCalculationFunc(assessmentSection, failureMechanism);
            calculation.Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble(), new TestGeneralResultSubMechanismIllustrationPoint());

            var nodeData = new GrassCoverErosionOutwardsHydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase,
                                                                                         failureMechanism,
                                                                                         assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                mockRepository.ReplayAll();

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

        [Test]
        public void ContextMenuStrip_HydraulicBoundaryLocationCalculationsWithoutIllustrationPoints_ContextMenuItemClearAllIllustrationPointsDisabled()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(failureMechanism, assessmentSection, new[]
            {
                new TestHydraulicBoundaryLocation()
            });

            var nodeData = new GrassCoverErosionOutwardsHydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase,
                                                                                         failureMechanism,
                                                                                         assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                mockRepository.ReplayAll();

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

        [Test]
        public void GivenValidCalculations_WhenCalculatingAllFromContextMenu_ThenAllCalculationsScheduled()
        {
            // Given
            var assessmentSection = new AssessmentSectionStub
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = validFilePath
                }
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 5
            };

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("Test");
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism, assessmentSection, new[]
                {
                    hydraulicBoundaryLocation
                });

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = CreateValidCalculation(hydraulicBoundaryLocation);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation);

            var context = new GrassCoverErosionOutwardsHydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase,
                                                                                        failureMechanism,
                                                                                        assessmentSection);

            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.DocumentViewController).Return(mockRepository.Stub<IDocumentViewController>());
                gui.Stub(cmp => cmp.MainWindow).Return(mockRepository.Stub<IMainWindow>());

                var calculatorFactory = mockRepository.Stub<IHydraRingCalculatorFactory>();
                var designWaterLevelCalculator = new TestDesignWaterLevelCalculator
                {
                    Converged = false,
                    DesignWaterLevel = 2.0
                };
                var waveHeightCalculator = new TestWaveHeightCalculator
                {
                    Converged = false
                };
                var waveConditionsCalculator = new TestWaveConditionsCosineCalculator
                {
                    Converged = false
                };

                HydraulicBoundaryCalculationSettings expectedCalculationSettings = HydraulicBoundaryCalculationSettingsFactory.CreateSettings(assessmentSection, hydraulicBoundaryLocation);
                calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                                 .WhenCalled(invocation =>
                                 {
                                     HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                         expectedCalculationSettings,
                                         (HydraRingCalculationSettings) invocation.Arguments[0]);
                                 })
                                 .Return(designWaterLevelCalculator)
                                 .Repeat
                                 .Times(5);
                calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                                 .WhenCalled(invocation =>
                                 {
                                     HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                         expectedCalculationSettings,
                                         (HydraRingCalculationSettings) invocation.Arguments[0]);
                                 })
                                 .Return(waveHeightCalculator)
                                 .Repeat
                                 .Times(5);
                calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                                 .WhenCalled(invocation =>
                                 {
                                     HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                         expectedCalculationSettings,
                                         (HydraRingCalculationSettings) invocation.Arguments[0]);
                                 })
                                 .Return(waveConditionsCalculator)
                                 .Repeat
                                 .Times(6);
                mockRepository.ReplayAll();

                plugin.Gui = gui;
                plugin.Activate();

                using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(context, null, treeViewControl))
                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                {
                    // When
                    Action call = () => contextMenuAdapter.Items[contextMenuCalculateAllIndex].PerformClick();

                    // Then
                    TestHelper.AssertLogMessages(call, messages =>
                    {
                        string[] msgs = messages.ToArray();
                        Assert.AreEqual(108, msgs.Length);

                        const string designWaterLevelCalculationTypeDisplayName = "Waterstand";
                        const string designWaterLevelCalculationDisplayName = "Waterstand berekening";

                        HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                            hydraulicBoundaryLocation.Name, designWaterLevelCalculationTypeDisplayName, designWaterLevelCalculationDisplayName, "Iv", msgs, 0);
                        HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                            hydraulicBoundaryLocation.Name, designWaterLevelCalculationTypeDisplayName, designWaterLevelCalculationDisplayName, "IIv", msgs, 8);
                        HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                            hydraulicBoundaryLocation.Name, designWaterLevelCalculationTypeDisplayName, designWaterLevelCalculationDisplayName, "IIIv", msgs, 16);
                        HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                            hydraulicBoundaryLocation.Name, designWaterLevelCalculationTypeDisplayName, designWaterLevelCalculationDisplayName, "IVv", msgs, 24);
                        HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                            hydraulicBoundaryLocation.Name, designWaterLevelCalculationTypeDisplayName, designWaterLevelCalculationDisplayName, "Vv", msgs, 32);

                        const string waveHeightCalculationTypeDisplayName = "Golfhoogte";
                        const string waveHeightCalculationDisplayName = "Golfhoogte berekening";

                        HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                            hydraulicBoundaryLocation.Name, waveHeightCalculationTypeDisplayName, waveHeightCalculationDisplayName, "Iv", msgs, 40);
                        HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                            hydraulicBoundaryLocation.Name, waveHeightCalculationTypeDisplayName, waveHeightCalculationDisplayName, "IIv", msgs, 48);
                        HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                            hydraulicBoundaryLocation.Name, waveHeightCalculationTypeDisplayName, waveHeightCalculationDisplayName, "IIIv", msgs, 56);
                        HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                            hydraulicBoundaryLocation.Name, waveHeightCalculationTypeDisplayName, waveHeightCalculationDisplayName, "IVv", msgs, 64);
                        HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                            hydraulicBoundaryLocation.Name, waveHeightCalculationTypeDisplayName, waveHeightCalculationDisplayName, "Vv", msgs, 72);

                        Assert.AreEqual($"Golfcondities berekenen voor '{calculation.Name}' is gestart.", msgs.ElementAt(80));
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs.ElementAt(81));
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs.ElementAt(82));
                        CalculationServiceTestHelper.AssertCalculationStartMessage(msgs.ElementAt(83));

                        IEnumerable<RoundedDouble> waterLevels = calculation.InputParameters.GetWaterLevels(
                            failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm.Single().Output.Result);
                        Assert.AreEqual(3, waterLevels.Count());
                        AssertWaveConditionsCalculationMessages(msgs, waterLevels, "golfoploop", 84);
                        AssertWaveConditionsCalculationMessages(msgs, waterLevels, "golfklap", 95);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(msgs.ElementAt(106));
                        Assert.AreEqual($"Golfcondities berekenen voor '{calculation.Name}' is gelukt.", msgs.ElementAt(107));
                    });
                }
            }
        }

        [Test]
        public void ChildNodeObjects_HydraulicBoundaryDatabaseNotLinked_ReturnNoChildNodes()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mockRepository);
            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var context = new GrassCoverErosionOutwardsHydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase, failureMechanism, assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(context).ToArray();

            // Assert
            Assert.AreEqual(0, children.Length);
        }

        [Test]
        public void ChildNodeObjects_HydraulicBoundaryDatabaseLinked_ReturnChildDataNodes()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = "databaseFile"
                }
            };
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                new TestHydraulicBoundaryLocation()
            });

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var context = new GrassCoverErosionOutwardsHydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase, failureMechanism, assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(context).ToArray();

            // Assert
            Assert.AreEqual(3, children.Length);
            var designWaterLevelCalculationsGroupContext = (GrassCoverErosionOutwardsDesignWaterLevelCalculationsGroupContext) children[0];
            CollectionAssert.AreEqual(assessmentSection.HydraulicBoundaryDatabase.Locations, designWaterLevelCalculationsGroupContext.WrappedData);
            Assert.AreSame(failureMechanism, designWaterLevelCalculationsGroupContext.FailureMechanism);
            Assert.AreSame(assessmentSection, designWaterLevelCalculationsGroupContext.AssessmentSection);

            var waveHeightHydraulicBoundaryCalculationsGroupContext = (GrassCoverErosionOutwardsWaveHeightCalculationsGroupContext) children[1];
            CollectionAssert.AreEqual(assessmentSection.HydraulicBoundaryDatabase.Locations, waveHeightHydraulicBoundaryCalculationsGroupContext.WrappedData);
            Assert.AreSame(failureMechanism, waveHeightHydraulicBoundaryCalculationsGroupContext.FailureMechanism);
            Assert.AreSame(assessmentSection, waveHeightHydraulicBoundaryCalculationsGroupContext.AssessmentSection);

            var waveConditionsCalculationGroupContext = (GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext) children[2];
            Assert.AreSame(failureMechanism.WaveConditionsCalculationGroup, waveConditionsCalculationGroupContext.WrappedData);
            Assert.IsNull(waveConditionsCalculationGroupContext.Parent);
            Assert.AreSame(failureMechanism, waveConditionsCalculationGroupContext.FailureMechanism);
            Assert.AreSame(assessmentSection, waveConditionsCalculationGroupContext.AssessmentSection);
        }

        [Test]
        public void ForeColor_HydraulicBoundaryDatabaseNotLinked_ReturnDisabledColor()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mockRepository);
            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var context = new GrassCoverErosionOutwardsHydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase, failureMechanism, assessmentSection);

            // Call
            Color color = info.ForeColor(context);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), color);
        }

        [Test]
        public void ForeColor_HydraulicBoundaryDatabaseLinked_ReturnEnabledColor()
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
            {
                FilePath = "databaseFile"
            });
            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var context = new GrassCoverErosionOutwardsHydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase, failureMechanism, assessmentSection);

            // Call
            Color color = info.ForeColor(context);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), color);
        }

        [Test]
        [TestCaseSource(nameof(GetHydraulicBoundaryLocationCalculations))]
        [Apartment(ApartmentState.STA)]
        public void GivenCalculationsWithIllustrationPoints_WhenClearIllustrationPointsClickedAndDoNotContinue_ThenInquiryAndIllustrationPointsNotCleared(
            Func<IAssessmentSection, GrassCoverErosionOutwardsFailureMechanism, HydraulicBoundaryLocationCalculation> getHydraulicLocationCalculationFunc)
        {
            // Given
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(failureMechanism, assessmentSection, new[]
            {
                new TestHydraulicBoundaryLocation()
            });

            var random = new Random(21);
            HydraulicBoundaryLocationCalculation calculation = getHydraulicLocationCalculationFunc(assessmentSection, failureMechanism);
            calculation.Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble(), new TestGeneralResultSubMechanismIllustrationPoint());

            HydraulicBoundaryLocationCalculation[] calculationsWithOutput = GetAllHydraulicLocationCalculationsWithOutput(assessmentSection, failureMechanism).ToArray();

            var messageBoxText = "";
            DialogBoxHandler = (name, wnd) =>
            {
                var helper = new MessageBoxTester(wnd);
                messageBoxText = helper.Text;

                helper.ClickCancel();
            };

            var context = new GrassCoverErosionOutwardsHydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase,
                                                                                        failureMechanism,
                                                                                        assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var affectedCalculationObserver = mockRepository.StrictMock<IObserver>();

                var gui = mockRepository.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mockRepository.ReplayAll();

                calculation.Attach(affectedCalculationObserver);

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // When
                    contextMenuAdapter.Items[contextMenuClearIllustrationPointsIndex].PerformClick();

                    // Then
                    const string expectedMessage = "Weet u zeker dat u alle berekende illustratiepunten bij 'Waterstanden en Golfhoogten' wilt wissen?";
                    Assert.AreEqual(expectedMessage, messageBoxText);

                    Assert.IsTrue(calculationsWithOutput.All(calc => calc.HasOutput));
                    Assert.IsTrue(calculation.Output.HasGeneralResult);
                }
            }
        }

        [Test]
        [TestCaseSource(nameof(GetHydraulicBoundaryLocationCalculations))]
        [Apartment(ApartmentState.STA)]
        public void GivenCalculationsWithIllustrationPoints_WhenClearIllustrationPointsClickedAndContinue_ThenInquiryAndIllustrationPointsCleared(
            Func<IAssessmentSection, GrassCoverErosionOutwardsFailureMechanism, HydraulicBoundaryLocationCalculation> getHydraulicLocationCalculationFunc)
        {
            // Given
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(failureMechanism, assessmentSection, new[]
            {
                new TestHydraulicBoundaryLocation(),
                new TestHydraulicBoundaryLocation()
            }, true);

            var random = new Random(21);
            HydraulicBoundaryLocationCalculation calculation = getHydraulicLocationCalculationFunc(assessmentSection, failureMechanism);
            calculation.Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble(), new TestGeneralResultSubMechanismIllustrationPoint());

            HydraulicBoundaryLocationCalculation unaffectedCalculation = assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm.ElementAt(1);

            HydraulicBoundaryLocationCalculation[] calculationsWithOutput = GetAllHydraulicLocationCalculationsWithOutput(assessmentSection, failureMechanism).ToArray();

            var messageBoxText = "";
            DialogBoxHandler = (name, wnd) =>
            {
                var helper = new MessageBoxTester(wnd);
                messageBoxText = helper.Text;

                helper.ClickOk();
            };

            var context = new GrassCoverErosionOutwardsHydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase,
                                                                                        failureMechanism,
                                                                                        assessmentSection);

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

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // When
                    contextMenuAdapter.Items[contextMenuClearIllustrationPointsIndex].PerformClick();

                    // Then
                    const string expectedMessage = "Weet u zeker dat u alle berekende illustratiepunten bij 'Waterstanden en Golfhoogten' wilt wissen?";
                    Assert.AreEqual(expectedMessage, messageBoxText);

                    Assert.IsTrue(calculationsWithOutput.All(calc => calc.HasOutput));
                    Assert.IsFalse(calculation.Output.HasGeneralResult);
                }
            }
        }

        public override void Setup()
        {
            mockRepository = new MockRepository();
            plugin = new GrassCoverErosionOutwardsPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(GrassCoverErosionOutwardsHydraulicBoundaryDatabaseContext));
        }

        public override void TearDown()
        {
            plugin.Dispose();
            mockRepository.VerifyAll();
        }

        private static void AssertWaveConditionsCalculationMessages(string[] logMessages, IEnumerable<RoundedDouble> waterLevels, string calculationType, int index)
        {
            Assert.AreEqual($"Berekening voor {calculationType} is gestart.", logMessages[index++]);

            foreach (RoundedDouble waterLevel in waterLevels)
            {
                Assert.AreEqual($"Berekening voor waterstand '{waterLevel}' is gestart.", logMessages[index++]);
                StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", logMessages[index++]);
                Assert.AreEqual($"Berekening voor waterstand '{waterLevel}' is beëindigd.", logMessages[index++]);
            }

            Assert.AreEqual($"Berekening voor {calculationType} is beëindigd.", logMessages[index]);
        }

        private static GrassCoverErosionOutwardsWaveConditionsCalculation CreateValidCalculation(HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            return new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    CategoryType = FailureMechanismCategoryType.MechanismSpecificFactorizedSignalingNorm,
                    ForeshoreProfile = new TestForeshoreProfile(true)
                    {
                        BreakWater =
                        {
                            Height = new Random(39).NextRoundedDouble()
                        }
                    },
                    UseForeshore = true,
                    UseBreakWater = true,
                    LowerBoundaryRevetment = (RoundedDouble) 1,
                    UpperBoundaryRevetment = (RoundedDouble) 2,
                    LowerBoundaryWaterLevels = (RoundedDouble) 1,
                    UpperBoundaryWaterLevels = (RoundedDouble) 2
                }
            };
        }

        private static IEnumerable<HydraulicBoundaryLocationCalculation> GetAllHydraulicLocationCalculationsWithOutput(IAssessmentSection assessmentSection,
                                                                                                                       GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            return failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm
                                   .Concat(failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm)
                                   .Concat(failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm)
                                   .Concat(assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm)
                                   .Concat(assessmentSection.WaterLevelCalculationsForLowerLimitNorm)
                                   .Concat(failureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm)
                                   .Concat(failureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm)
                                   .Concat(failureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm)
                                   .Concat(assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm)
                                   .Concat(assessmentSection.WaveHeightCalculationsForLowerLimitNorm)
                                   .Where(calc => calc.HasOutput);
        }

        private static IEnumerable<TestCaseData> GetHydraulicBoundaryLocationCalculations()
        {
            yield return new TestCaseData(new Func<IAssessmentSection, GrassCoverErosionOutwardsFailureMechanism, HydraulicBoundaryLocationCalculation>(
                                              (s, fm) => fm.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm.First()));
            yield return new TestCaseData(new Func<IAssessmentSection, GrassCoverErosionOutwardsFailureMechanism, HydraulicBoundaryLocationCalculation>(
                                              (s, fm) => fm.WaterLevelCalculationsForMechanismSpecificSignalingNorm.First()));
            yield return new TestCaseData(new Func<IAssessmentSection, GrassCoverErosionOutwardsFailureMechanism, HydraulicBoundaryLocationCalculation>(
                                              (s, fm) => fm.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm.First()));
            yield return new TestCaseData(new Func<IAssessmentSection, GrassCoverErosionOutwardsFailureMechanism, HydraulicBoundaryLocationCalculation>(
                                              (s, fm) => s.WaterLevelCalculationsForLowerLimitNorm.First()));
            yield return new TestCaseData(new Func<IAssessmentSection, GrassCoverErosionOutwardsFailureMechanism, HydraulicBoundaryLocationCalculation>(
                                              (s, fm) => s.WaterLevelCalculationsForFactorizedLowerLimitNorm.First()));
            yield return new TestCaseData(new Func<IAssessmentSection, GrassCoverErosionOutwardsFailureMechanism, HydraulicBoundaryLocationCalculation>(
                                              (s, fm) => fm.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm.First()));
            yield return new TestCaseData(new Func<IAssessmentSection, GrassCoverErosionOutwardsFailureMechanism, HydraulicBoundaryLocationCalculation>(
                                              (s, fm) => fm.WaveHeightCalculationsForMechanismSpecificSignalingNorm.First()));
            yield return new TestCaseData(new Func<IAssessmentSection, GrassCoverErosionOutwardsFailureMechanism, HydraulicBoundaryLocationCalculation>(
                                              (s, fm) => fm.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm.First()));
            yield return new TestCaseData(new Func<IAssessmentSection, GrassCoverErosionOutwardsFailureMechanism, HydraulicBoundaryLocationCalculation>(
                                              (s, fm) => s.WaveHeightCalculationsForLowerLimitNorm.First()));
            yield return new TestCaseData(new Func<IAssessmentSection, GrassCoverErosionOutwardsFailureMechanism, HydraulicBoundaryLocationCalculation>(
                                              (s, fm) => s.WaveHeightCalculationsForFactorizedLowerLimitNorm.First()));
        }
    }
}