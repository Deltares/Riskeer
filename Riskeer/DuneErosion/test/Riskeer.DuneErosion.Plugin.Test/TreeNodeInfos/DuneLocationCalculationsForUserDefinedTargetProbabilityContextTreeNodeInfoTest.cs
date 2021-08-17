﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Common.Util;
using Core.Common.Util.Extensions;
using Core.Gui;
using Core.Gui.ContextMenu;
using Core.Gui.Forms.Main;
using Core.Gui.Forms.ViewHost;
using Core.Gui.TestUtil;
using Core.Gui.TestUtil.ContextMenu;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Plugin.TestUtil;
using Riskeer.Common.Service.TestUtil;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Forms.PresentationObjects;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Data.Input.Hydraulics;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.DuneErosion.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class DuneLocationCalculationsForUserDefinedTargetProbabilityContextTreeNodeInfoTest
    {
        private const int contextMenuCalculateAllIndex = 4;

        private MockRepository mocks;
        private DuneErosionPlugin plugin;
        private TreeNodeInfo info;

        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, nameof(HydraulicBoundaryDatabase));
        private static readonly string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new DuneErosionPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(DuneLocationCalculationsForUserDefinedTargetProbabilityContext));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();

            mocks.VerifyAll();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            mocks.ReplayAll();

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

        [Test]
        public void Text_WithContext_ReturnsFormattedTargetProbability()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var context = new DuneLocationCalculationsForUserDefinedTargetProbabilityContext(new DuneLocationCalculationsForTargetProbability
                                                                                             {
                                                                                                 TargetProbability = 0.01
                                                                                             },
                                                                                             new DuneErosionFailureMechanism(),
                                                                                             assessmentSection);

            // Call
            string text = info.Text(context);

            // Assert
            Assert.AreEqual("1/100", text);
        }

        [Test]
        public void Image_Always_ReturnsGenericInputOutputIcon()
        {
            // Setup
            mocks.ReplayAll();

            // Call
            Image image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.GenericInputOutputIcon, image);
        }

        [Test]
        public void EnsureVisibleOnCreate_Always_ReturnsTrue()
        {
            // Call
            bool ensureVisibleOnCreate = info.EnsureVisibleOnCreate(null, null);

            // Assert
            Assert.IsTrue(ensureVisibleOnCreate);
        }

        [Test]
        public void CanRemove_Always_ReturnsTrue()
        {
            // Call
            bool canRemove = info.CanRemove(null, null);

            // Assert
            Assert.IsTrue(canRemove);
        }

        [Test]
        public void OnNodeRemoved_WithContexts_RemovesItemAndNotifiesObservers()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);

            var failureMechanism = new DuneErosionFailureMechanism();

            var calculationObserver = mocks.StrictMock<IObserver>();
            calculationObserver.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            var calculationForFirstTargetProbability = new DuneLocationCalculationsForTargetProbability();
            var calculationForSecondTargetProbability = new DuneLocationCalculationsForTargetProbability();
            var calculations = new ObservableList<DuneLocationCalculationsForTargetProbability>
            {
                calculationForFirstTargetProbability,
                calculationForSecondTargetProbability
            };

            calculations.Attach(calculationObserver);

            var parentContext = new DuneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext(calculations,
                                                                                                          failureMechanism,
                                                                                                          assessmentSection);

            var context = new DuneLocationCalculationsForUserDefinedTargetProbabilityContext(calculationForFirstTargetProbability,
                                                                                             failureMechanism,
                                                                                             assessmentSection);

            // Call
            info.OnNodeRemoved(context, parentContext);

            // Assert
            Assert.AreEqual(1, calculations.Count);
            CollectionAssert.DoesNotContain(calculations, calculationForFirstTargetProbability);

            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);
                var context = new DuneLocationCalculationsForUserDefinedTargetProbabilityContext(new DuneLocationCalculationsForTargetProbability(),
                                                                                                 new DuneErosionFailureMechanism(),
                                                                                                 assessmentSection);

                var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();
                using (mocks.Ordered())
                {
                    menuBuilder.Expect(mb => mb.AddOpenItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddExportItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddDeleteItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.Build()).Return(null);
                }

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(menuBuilder);

                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(context, null, treeViewControl);
            }

            // Assert
            // Done in tearDown
        }

        [Test]
        public void ContextMenuStrip_Always_AddCustomItems()
        {
            // Setup
            IAssessmentSection assessmentSection = new AssessmentSectionStub();

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var nodeData = new DuneLocationCalculationsForUserDefinedTargetProbabilityContext(
                new DuneLocationCalculationsForTargetProbability(),
                new DuneErosionFailureMechanism(),
                assessmentSection);

            var mockRepository = new MockRepository();

            using (var treeViewControl = new TreeViewControl())
            {
                IGui gui = StubFactory.CreateGuiStub(mockRepository);
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                mockRepository.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, assessmentSection, treeViewControl))
                {
                    // Assert
                    Assert.AreEqual(9, menu.Items.Count);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndex,
                                                                  "Alles be&rekenen",
                                                                  "Er is geen hydraulische belastingendatabase geïmporteerd.",
                                                                  RiskeerCommonFormsResources.CalculateAllIcon,
                                                                  false);
                }
            }

            // Assert
            mockRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_HydraulicBoundaryDatabaseLinkedToInvalidFile_ContextMenuItemCalculateAllDisabledAndTooltipSet()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(null, mocks, "invalidFilePath");

                var context = new DuneLocationCalculationsForUserDefinedTargetProbabilityContext(new DuneLocationCalculationsForTargetProbability(),
                                                                                                 new DuneErosionFailureMechanism(),
                                                                                                 assessmentSection);

                var builder = new CustomItemsOnlyContextMenuBuilder();
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(builder);

                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Assert
                    ToolStripItem contextMenuItem = menu.Items[contextMenuCalculateAllIndex];

                    Assert.AreEqual("Alles be&rekenen", contextMenuItem.Text);
                    StringAssert.Contains("Herstellen van de verbinding met de hydraulische belastingendatabase is mislukt.", contextMenuItem.ToolTipText);
                    TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.CalculateAllIcon, contextMenuItem.Image);
                    Assert.IsFalse(contextMenuItem.Enabled);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_AllRequiredInputSet_ContextMenuItemCalculateAllEnabled()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "1.0"
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(hydraulicBoundaryDatabase);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase);

            var context = new DuneLocationCalculationsForUserDefinedTargetProbabilityContext(new DuneLocationCalculationsForTargetProbability(),
                                                                                             new DuneErosionFailureMechanism(),
                                                                                             assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var builder = new CustomItemsOnlyContextMenuBuilder();
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(builder);
                gui.Stub(g => g.ViewHost).Return(mocks.Stub<IViewHost>());

                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndex,
                                                                  "Alles be&rekenen",
                                                                  "Alle hydraulische belastingen berekenen.",
                                                                  RiskeerCommonFormsResources.CalculateAllIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_ClickOnCalculateAllItem_ScheduleAllCalculationsAndNotifyObservers()
        {
            // Setup
            const string locationName1 = "1";
            const string locationName2 = "2";

            using (var treeViewControl = new TreeViewControl())
            {
                var duneLocationCalculationsForTargetProbability = new DuneLocationCalculationsForTargetProbability
                {
                    TargetProbability = 0.01,
                    DuneLocationCalculations =
                    {
                        new DuneLocationCalculation(new DuneLocation(1300001, locationName1, new Point2D(0, 0), new DuneLocation.ConstructionProperties
                        {
                            CoastalAreaId = 0,
                            Offset = 0,
                            Orientation = 0,
                            D50 = 0.000007
                        })),
                        new DuneLocationCalculation(new DuneLocation(1300002, locationName2, new Point2D(0, 0), new DuneLocation.ConstructionProperties
                        {
                            CoastalAreaId = 0,
                            Offset = 0,
                            Orientation = 0,
                            D50 = 0.000007
                        }))
                    }
                };

                var failureMechanism = new DuneErosionFailureMechanism();

                var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
                {
                    FilePath = validFilePath
                };
                HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(hydraulicBoundaryDatabase);

                var assessmentSection = mocks.Stub<IAssessmentSection>();
                assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase);
                assessmentSection.Stub(a => a.Id).Return("13-1");
                assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
                {
                    failureMechanism
                });
                assessmentSection.Stub(a => a.FailureMechanismContribution)
                                 .Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());

                var context = new DuneLocationCalculationsForUserDefinedTargetProbabilityContext(duneLocationCalculationsForTargetProbability,
                                                                                                 failureMechanism,
                                                                                                 assessmentSection);

                var builder = new CustomItemsOnlyContextMenuBuilder();

                IMainWindow mainWindow = MainWindowTestHelper.CreateMainWindowStub(mocks);

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(builder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                var calculationObserver = mocks.StrictMock<IObserver>();
                calculationObserver.Expect(o => o.UpdateObserver()).Repeat.Times(2);
                var calculationsObserver = mocks.StrictMock<IObserver>();

                var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(null))
                                 .IgnoreArguments()
                                 .Return(new TestDunesBoundaryConditionsCalculator())
                                 .Repeat
                                 .Times(2);
                mocks.ReplayAll();

                duneLocationCalculationsForTargetProbability.DuneLocationCalculations.Attach(calculationsObserver);
                duneLocationCalculationsForTargetProbability.DuneLocationCalculations.ForEachElementDo(location => location.Attach(calculationObserver));

                plugin.Gui = gui;
                plugin.Activate();

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                {
                    // Call
                    TestHelper.AssertLogMessages(() => contextMenu.Items[contextMenuCalculateAllIndex].PerformClick(), messages =>
                    {
                        List<string> messageList = messages.ToList();

                        // Assert
                        Assert.AreEqual(16, messageList.Count);
                        Assert.AreEqual($"Hydraulische belastingen berekenen voor locatie '{locationName1}' (1/100) is gestart.", messageList[0]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(messageList[1]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(messageList[2]);
                        CalculationServiceTestHelper.AssertCalculationStartMessage(messageList[3]);
                        Assert.AreEqual($"Hydraulische belastingenberekening voor locatie '{locationName1}' (1/100) is niet geconvergeerd.", messageList[4]);
                        StringAssert.StartsWith("Hydraulische belastingenberekening is uitgevoerd op de tijdelijke locatie", messageList[5]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(messageList[6]);
                        Assert.AreEqual($"Hydraulische belastingen berekenen voor locatie '{locationName1}' (1/100) is gelukt.", messageList[7]);

                        Assert.AreEqual($"Hydraulische belastingen berekenen voor locatie '{locationName2}' (1/100) is gestart.", messageList[8]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(messageList[9]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(messageList[10]);
                        CalculationServiceTestHelper.AssertCalculationStartMessage(messageList[11]);
                        Assert.AreEqual($"Hydraulische belastingenberekening voor locatie '{locationName2}' (1/100) is niet geconvergeerd.", messageList[12]);
                        StringAssert.StartsWith("Hydraulische belastingenberekening is uitgevoerd op de tijdelijke locatie", messageList[13]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(messageList[14]);
                        Assert.AreEqual($"Hydraulische belastingen berekenen voor locatie '{locationName2}' (1/100) is gelukt.", messageList[15]);
                    });
                }
            }
        }

        [Test]
        public void PerformDuneLocationCalculationsFromContextMenu_HydraulicBoundaryDatabaseWithCanUsePreprocessorFalse_SendsRightInputToCalculationService()
        {
            // Setup
            var duneLocationCalculationsForTargetProbability = new DuneLocationCalculationsForTargetProbability
            {
                TargetProbability = 0.01,
                DuneLocationCalculations =
                {
                    new DuneLocationCalculation(new DuneLocation(1300001, "A", new Point2D(0, 0), new DuneLocation.ConstructionProperties
                    {
                        CoastalAreaId = 0,
                        Offset = 0,
                        Orientation = 0,
                        D50 = 0.000007
                    }))
                }
            };

            var failureMechanism = new DuneErosionFailureMechanism();

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(hydraulicBoundaryDatabase);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase);
            assessmentSection.Stub(a => a.Id).Return("13-1");
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });
            assessmentSection.Stub(a => a.FailureMechanismContribution)
                             .Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());

            var context = new DuneLocationCalculationsForUserDefinedTargetProbabilityContext(duneLocationCalculationsForTargetProbability,
                                                                                             failureMechanism,
                                                                                             assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                IMainWindow mainWindow = MainWindowTestHelper.CreateMainWindowStub(mocks);

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mainWindow);

                var dunesBoundaryConditionsCalculator = new TestDunesBoundaryConditionsCalculator();
                var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                                 .WhenCalled(invocation =>
                                 {
                                     HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                         HydraulicBoundaryCalculationSettingsFactory.CreateSettings(hydraulicBoundaryDatabase),
                                         (HydraRingCalculationSettings) invocation.Arguments[0]);
                                 })
                                 .Return(dunesBoundaryConditionsCalculator);
                mocks.ReplayAll();

                plugin.Gui = gui;
                plugin.Activate();

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                {
                    // Call
                    contextMenu.Items[contextMenuCalculateAllIndex].PerformClick();

                    // Assert
                    DunesBoundaryConditionsCalculationInput dunesBoundaryConditionsCalculationInput = dunesBoundaryConditionsCalculator.ReceivedInputs.First();

                    Assert.AreEqual(duneLocationCalculationsForTargetProbability.DuneLocationCalculations[0].DuneLocation.Id,
                                    dunesBoundaryConditionsCalculationInput.HydraulicBoundaryLocationId);
                    Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(duneLocationCalculationsForTargetProbability.TargetProbability),
                                    dunesBoundaryConditionsCalculationInput.Beta);
                }
            }
        }

        [Test]
        public void PerformDuneLocationCalculationsFromContextMenu_HydraulicBoundaryDatabaseWithUsePreprocessorTrue_SendsRightInputToCalculationService()
        {
            // Setup
            string preprocessorDirectory = TestHelper.GetScratchPadPath();

            var duneLocationCalculationsForTargetProbability = new DuneLocationCalculationsForTargetProbability
            {
                TargetProbability = 0.01,
                DuneLocationCalculations =
                {
                    new DuneLocationCalculation(new DuneLocation(1300001, "A", new Point2D(0, 0), new DuneLocation.ConstructionProperties
                    {
                        CoastalAreaId = 0,
                        Offset = 0,
                        Orientation = 0,
                        D50 = 0.000007
                    }))
                }
            };

            var failureMechanism = new DuneErosionFailureMechanism();

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                HydraulicLocationConfigurationSettings =
                {
                    CanUsePreprocessor = true,
                    UsePreprocessor = true,
                    PreprocessorDirectory = preprocessorDirectory
                }
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(hydraulicBoundaryDatabase);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase);
            assessmentSection.Stub(a => a.Id).Return("13-1");
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });
            assessmentSection.Stub(a => a.FailureMechanismContribution)
                             .Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());

            var context = new DuneLocationCalculationsForUserDefinedTargetProbabilityContext(duneLocationCalculationsForTargetProbability,
                                                                                             failureMechanism,
                                                                                             assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                IMainWindow mainWindow = MainWindowTestHelper.CreateMainWindowStub(mocks);

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mainWindow);

                var dunesBoundaryConditionsCalculator = new TestDunesBoundaryConditionsCalculator();
                var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                                 .WhenCalled(invocation =>
                                 {
                                     HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                         HydraulicBoundaryCalculationSettingsFactory.CreateSettings(hydraulicBoundaryDatabase),
                                         (HydraRingCalculationSettings) invocation.Arguments[0]);
                                 })
                                 .Return(dunesBoundaryConditionsCalculator);

                mocks.ReplayAll();

                plugin.Gui = gui;
                plugin.Activate();

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                {
                    // Call
                    contextMenu.Items[contextMenuCalculateAllIndex].PerformClick();

                    // Assert
                    DunesBoundaryConditionsCalculationInput dunesBoundaryConditionsCalculationInput = dunesBoundaryConditionsCalculator.ReceivedInputs.First();

                    Assert.AreEqual(duneLocationCalculationsForTargetProbability.DuneLocationCalculations[0].DuneLocation.Id,
                                    dunesBoundaryConditionsCalculationInput.HydraulicBoundaryLocationId);
                    Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(duneLocationCalculationsForTargetProbability.TargetProbability),
                                    dunesBoundaryConditionsCalculationInput.Beta);
                }
            }
        }

        [Test]
        public void PerformDuneLocationCalculationsFromContextMenu_HydraulicBoundaryDatabaseWithUsePreprocessorFalse_SendsRightInputToCalculationService()
        {
            // Setup
            var duneLocationCalculationsForTargetProbability = new DuneLocationCalculationsForTargetProbability
            {
                TargetProbability = 0.01,
                DuneLocationCalculations =
                {
                    new DuneLocationCalculation(new DuneLocation(1300001, "A", new Point2D(0, 0), new DuneLocation.ConstructionProperties
                    {
                        CoastalAreaId = 0,
                        Offset = 0,
                        Orientation = 0,
                        D50 = 0.000007
                    }))
                }
            };

            var failureMechanism = new DuneErosionFailureMechanism();

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                HydraulicLocationConfigurationSettings =
                {
                    CanUsePreprocessor = true,
                    UsePreprocessor = false,
                    PreprocessorDirectory = "InvalidPreprocessorDirectory"
                }
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(hydraulicBoundaryDatabase);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase);
            assessmentSection.Stub(a => a.Id).Return("13-1");
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });
            assessmentSection.Stub(a => a.FailureMechanismContribution)
                             .Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());

            var context = new DuneLocationCalculationsForUserDefinedTargetProbabilityContext(duneLocationCalculationsForTargetProbability,
                                                                                             failureMechanism,
                                                                                             assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                IMainWindow mainWindow = MainWindowTestHelper.CreateMainWindowStub(mocks);

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mainWindow);

                var dunesBoundaryConditionsCalculator = new TestDunesBoundaryConditionsCalculator();
                var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                                 .WhenCalled(invocation =>
                                 {
                                     HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                         HydraulicBoundaryCalculationSettingsFactory.CreateSettings(hydraulicBoundaryDatabase),
                                         (HydraRingCalculationSettings) invocation.Arguments[0]);
                                 })
                                 .Return(dunesBoundaryConditionsCalculator);

                mocks.ReplayAll();

                plugin.Gui = gui;
                plugin.Activate();

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                {
                    // Call
                    contextMenu.Items[contextMenuCalculateAllIndex].PerformClick();

                    // Assert
                    DunesBoundaryConditionsCalculationInput dunesBoundaryConditionsCalculationInput = dunesBoundaryConditionsCalculator.ReceivedInputs.First();

                    Assert.AreEqual(duneLocationCalculationsForTargetProbability.DuneLocationCalculations[0].DuneLocation.Id,
                                    dunesBoundaryConditionsCalculationInput.HydraulicBoundaryLocationId);
                    Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(duneLocationCalculationsForTargetProbability.TargetProbability),
                                    dunesBoundaryConditionsCalculationInput.Beta);
                }
            }
        }

        [Test]
        public void CanDrag_Always_ReturnsTrue()
        {
            // Setup
            mocks.ReplayAll();

            // Call
            bool canDrag = info.CanDrag(null, null);

            // Assert
            Assert.IsTrue(canDrag);
        }
    }
}