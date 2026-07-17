// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
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
using NSubstitute;
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

        private DuneErosionPlugin plugin;
        private TreeNodeInfo info;

        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, nameof(HydraulicBoundaryData));
        private static readonly string validHlcdFilePath = Path.Combine(testDataPath, "hlcd.sqlite");
        private static readonly string validHrdFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
        private static readonly string validHrdFileVersion = "Dutch coast South19-11-2015 12:0013";

        [SetUp]
        public void SetUp()
        {
            plugin = new DuneErosionPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(DuneLocationCalculationsForUserDefinedTargetProbabilityContext));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
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
        [TestCase(0.025, 0.0025, 0.00025, "1/400")]
        [TestCase(0.0025, 0.0025, 0.025, "1/400 (1)")]
        [TestCase(0.0025, 0.0025, 0.0025, "1/400 (2)")]
        public void Text_WithContext_ReturnsUniquelyFormattedTargetProbability(double userDefinedTargetProbability1,
                                                                               double userDefinedTargetProbability2,
                                                                               double userDefinedTargetProbability3,
                                                                               string expectedText)
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();
            var calculationsForTargetProbability = new DuneLocationCalculationsForTargetProbability(userDefinedTargetProbability2);
            failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities.AddRange(new[]
            {
                new DuneLocationCalculationsForTargetProbability(userDefinedTargetProbability1),
                new DuneLocationCalculationsForTargetProbability(userDefinedTargetProbability3),
                calculationsForTargetProbability
            });

            var context = new DuneLocationCalculationsForUserDefinedTargetProbabilityContext(calculationsForTargetProbability,
                                                                                             failureMechanism,
                                                                                             new AssessmentSectionStub(new[]
                                                                                             {
                                                                                                 failureMechanism
                                                                                             }));

            // Call
            string text = info.Text(context);

            // Assert
            Assert.AreEqual(expectedText, text);
        }

        [Test]
        public void Image_Always_ReturnsGenericInputOutputIcon()
        {
            // Setup
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
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();

            var failureMechanism = new DuneErosionFailureMechanism();

            var calculationObserver = Substitute.For<IObserver>();
            var calculationForFirstTargetProbability = new DuneLocationCalculationsForTargetProbability(0.1);
            var calculationForSecondTargetProbability = new DuneLocationCalculationsForTargetProbability(0.01);
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
            calculationObserver.Received().UpdateObserver();
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();
                var context = new DuneLocationCalculationsForUserDefinedTargetProbabilityContext(new DuneLocationCalculationsForTargetProbability(0.1),
                                                                                                 new DuneErosionFailureMechanism(),
                                                                                                 assessmentSection);

                var menuBuilder = Substitute.For<IContextMenuBuilder>();
                menuBuilder.AddOpenItem().Returns(menuBuilder);
                menuBuilder.AddSeparator().Returns(menuBuilder);
                menuBuilder.AddExportItem().Returns(menuBuilder);
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>()).Returns(menuBuilder);
                menuBuilder.AddDeleteItem().Returns(menuBuilder);
                menuBuilder.AddPropertiesItem().Returns(menuBuilder);

                var gui = Substitute.For<IGui>();
                gui.Get(context, treeViewControl).Returns(menuBuilder);
                gui.ViewHost.Returns(Substitute.For<IViewHost>());
                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(context, null, treeViewControl);

                // Assert
                Received.InOrder(() =>
                {
                    menuBuilder.AddOpenItem();
                    menuBuilder.AddSeparator();
                    menuBuilder.AddExportItem();
                    menuBuilder.AddSeparator();
                    menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                    menuBuilder.AddSeparator();
                    menuBuilder.AddDeleteItem();
                    menuBuilder.AddSeparator();
                    menuBuilder.AddPropertiesItem();
                    menuBuilder.Build();
                });
            }
        }

        [Test]
        public void ContextMenuStrip_Always_AddCustomItems()
        {
            // Setup
            IAssessmentSection assessmentSection = new AssessmentSectionStub();

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var nodeData = new DuneLocationCalculationsForUserDefinedTargetProbabilityContext(
                new DuneLocationCalculationsForTargetProbability(0.1),
                new DuneErosionFailureMechanism(),
                assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                IGui gui = StubFactory.CreateGuiStub();
                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());
                gui.ViewHost.Returns(Substitute.For<IViewHost>());
                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, assessmentSection, treeViewControl))
                {
                    // Assert
                    Assert.AreEqual(9, menu.Items.Count);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndex,
                                                                  "Alles be&rekenen",
                                                                  "Alle hydraulische belastingen berekenen.",
                                                                  RiskeerCommonFormsResources.CalculateAllIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_AllRequiredInputSet_ContextMenuItemCalculateAllEnabled()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();

            var context = new DuneLocationCalculationsForUserDefinedTargetProbabilityContext(new DuneLocationCalculationsForTargetProbability(0.1),
                                                                                             new DuneErosionFailureMechanism(),
                                                                                             assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var builder = new CustomItemsOnlyContextMenuBuilder();
                var gui = Substitute.For<IGui>();
                gui.Get(context, treeViewControl).Returns(builder);
                gui.ViewHost.Returns(Substitute.For<IViewHost>());
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
                var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1300001, string.Empty, 0, 0);

                var duneLocationCalculationsForTargetProbability = new DuneLocationCalculationsForTargetProbability(0.01)
                {
                    DuneLocationCalculations =
                    {
                        new DuneLocationCalculation(new DuneLocation(locationName1, hydraulicBoundaryLocation, new DuneLocation.ConstructionProperties
                        {
                            CoastalAreaId = 0,
                            Offset = 0
                        })),
                        new DuneLocationCalculation(new DuneLocation(locationName2, hydraulicBoundaryLocation, new DuneLocation.ConstructionProperties
                        {
                            CoastalAreaId = 0,
                            Offset = 0
                        }))
                    }
                };

                var failureMechanism = new DuneErosionFailureMechanism();
                failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities.Add(duneLocationCalculationsForTargetProbability);

                var hydraulicBoundaryData = new HydraulicBoundaryData
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
                };

                var assessmentSection = Substitute.For<IAssessmentSection>();
                assessmentSection.HydraulicBoundaryData.Returns(hydraulicBoundaryData);
                assessmentSection.Id.Returns("13-1");
                assessmentSection.GetFailureMechanisms().Returns(new[]
                {
                    failureMechanism
                });
                assessmentSection.FailureMechanismContribution
                                 .Returns(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());

                var context = new DuneLocationCalculationsForUserDefinedTargetProbabilityContext(duneLocationCalculationsForTargetProbability,
                                                                                                 failureMechanism,
                                                                                                 assessmentSection);

                var builder = new CustomItemsOnlyContextMenuBuilder();

                IMainWindow mainWindow = MainWindowTestHelper.CreateMainWindowStub();

                var gui = Substitute.For<IGui>();
                gui.Get(context, treeViewControl).Returns(builder);
                gui.MainWindow.Returns(mainWindow);
                gui.ViewHost.Returns(Substitute.For<IViewHost>());
                var calculationObserver = Substitute.For<IObserver>();
                var calculationsObserver = Substitute.For<IObserver>();

                var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();
                calculatorFactory.CreateDunesBoundaryConditionsCalculator(Arg.Any<HydraRingCalculationSettings>())
                                 .Returns(new TestDunesBoundaryConditionsCalculator());
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

                calculationObserver.Received(2).UpdateObserver();
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void PerformDuneLocationCalculationsFromContextMenu_HydraulicBoundaryDataSet_SendsRightInputToCalculationService(bool usePreprocessorClosure)
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1300001, string.Empty, 0, 0);

            var duneLocationCalculationsForTargetProbability = new DuneLocationCalculationsForTargetProbability(0.01)
            {
                DuneLocationCalculations =
                {
                    new DuneLocationCalculation(new DuneLocation("A", hydraulicBoundaryLocation, new DuneLocation.ConstructionProperties
                    {
                        CoastalAreaId = 0,
                        Offset = 0
                    }))
                }
            };

            var failureMechanism = new DuneErosionFailureMechanism();
            failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities.Add(duneLocationCalculationsForTargetProbability);

            var hydraulicBoundaryData = new HydraulicBoundaryData
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
                        UsePreprocessorClosure = usePreprocessorClosure,
                        Locations =
                        {
                            hydraulicBoundaryLocation
                        }
                    }
                }
            };

            var assessmentSection = Substitute.For<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryData.Returns(hydraulicBoundaryData);
            assessmentSection.Id.Returns("13-1");
            assessmentSection.GetFailureMechanisms().Returns(new[]
            {
                failureMechanism
            });
            assessmentSection.FailureMechanismContribution
                             .Returns(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());

            var context = new DuneLocationCalculationsForUserDefinedTargetProbabilityContext(duneLocationCalculationsForTargetProbability,
                                                                                             failureMechanism,
                                                                                             assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                IMainWindow mainWindow = MainWindowTestHelper.CreateMainWindowStub();

                var gui = Substitute.For<IGui>();
                gui.Get(context, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());
                gui.MainWindow.Returns(mainWindow);
                gui.ViewHost.Returns(Substitute.For<IViewHost>());

                var dunesBoundaryConditionsCalculator = new TestDunesBoundaryConditionsCalculator();
                var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();

                calculatorFactory
                    .CreateDunesBoundaryConditionsCalculator(Arg.Is<HydraRingCalculationSettings>(x => x != null))
                    .Returns(callInfo =>
                    {
                        HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                            HydraulicBoundaryCalculationSettingsFactory.CreateSettings(hydraulicBoundaryData,
                                                                                       hydraulicBoundaryLocation),
                            callInfo.Arg<HydraRingCalculationSettings>());
                        return dunesBoundaryConditionsCalculator;
                    });

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
            // Call
            bool canDrag = info.CanDrag(null, null);

            // Assert
            Assert.IsTrue(canDrag);
        }
    }
}