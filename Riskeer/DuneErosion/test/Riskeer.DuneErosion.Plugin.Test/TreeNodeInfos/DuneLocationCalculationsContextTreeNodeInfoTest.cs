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

using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using Core.Common.Util;
using Core.Common.Util.Extensions;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Service.TestUtil;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Data.TestUtil;
using Riskeer.DuneErosion.Forms.PresentationObjects;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Data.Input.Hydraulics;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.DuneErosion.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class DuneLocationCalculationsContextTreeNodeInfoTest
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
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(DuneLocationCalculationsContext));
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

        [Test]
        public void Text_WithContext_ReturnCategoryBoundaryName()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            const string categoryBoundaryName = "A";
            var context = new DuneLocationCalculationsContext(new ObservableList<DuneLocationCalculation>(),
                                                              new DuneErosionFailureMechanism(),
                                                              assessmentSection,
                                                              () => 0.01,
                                                              categoryBoundaryName);

            // Call
            string text = info.Text(context);

            // Assert
            Assert.AreEqual($"Categoriegrens {categoryBoundaryName}", text);
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
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);
                var context = new DuneLocationCalculationsContext(new ObservableList<DuneLocationCalculation>(),
                                                                  new DuneErosionFailureMechanism(),
                                                                  assessmentSection,
                                                                  () => 0.01,
                                                                  "A");

                var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();
                using (mocks.Ordered())
                {
                    menuBuilder.Expect(mb => mb.AddOpenItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddExportItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
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
        public void ContextMenuStrip_HydraulicBoundaryDatabaseLinkedToInvalidFile_ContextMenuItemCalculateAllDisabledAndTooltipSet()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(null, mocks, "invalidFilePath");

                var failureMechanism = new DuneErosionFailureMechanism
                {
                    Contribution = 10
                };

                var duneLocationCalculations = new ObservableList<DuneLocationCalculation>
                {
                    new DuneLocationCalculation(new TestDuneLocation())
                };
                var context = new DuneLocationCalculationsContext(duneLocationCalculations,
                                                                  failureMechanism,
                                                                  assessmentSection,
                                                                  () => 0.01,
                                                                  "A");

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
                    StringAssert.Contains("Herstellen van de verbinding met één van de benodigde hydraulische belastingendatabases is mislukt.", contextMenuItem.ToolTipText);
                    TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.CalculateAllIcon, contextMenuItem.Image);
                    Assert.IsFalse(contextMenuItem.Enabled);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_InvalidNorm_ContextMenuItemCalculateAllDisabledAndTooltipSet()
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

            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = 10
            };

            var duneLocationCalculations = new ObservableList<DuneLocationCalculation>
            {
                new DuneLocationCalculation(new TestDuneLocation())
            };
            var context = new DuneLocationCalculationsContext(duneLocationCalculations,
                                                              failureMechanism,
                                                              assessmentSection,
                                                              () => 1.0,
                                                              "A");
            using (var treeViewControl = new TreeViewControl())
            {
                var builder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(builder);

                mocks.ReplayAll();

                plugin.Gui = gui;

                failureMechanism.AddDuneLocations(new[]
                {
                    new TestDuneLocation()
                });

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndex,
                                                                  "Alles be&rekenen",
                                                                  "Doelkans is te groot om een berekening uit te kunnen voeren.",
                                                                  RiskeerCommonFormsResources.CalculateAllIcon,
                                                                  false);
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

            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = 10
            };

            var duneLocationCalculations = new ObservableList<DuneLocationCalculation>
            {
                new DuneLocationCalculation(new TestDuneLocation())
            };
            var context = new DuneLocationCalculationsContext(duneLocationCalculations,
                                                              failureMechanism,
                                                              assessmentSection,
                                                              () => 0.01,
                                                              "A");

            using (var treeViewControl = new TreeViewControl())
            {
                var builder = new CustomItemsOnlyContextMenuBuilder();
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(builder);

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
            const string categoryBoundaryName = "A";
            const string locationName1 = "1";
            const string locationName2 = "2";

            using (var treeViewControl = new TreeViewControl())
            {
                var hydraulicBoundaryLocation1 = new HydraulicBoundaryLocation(1300001, "", 0, 0);
                var hydraulicBoundaryLocation2 = new HydraulicBoundaryLocation(1300002, "", 0, 0);

                var duneLocationCalculations = new ObservableList<DuneLocationCalculation>
                {
                    new DuneLocationCalculation(new DuneLocation(hydraulicBoundaryLocation1,
                                                                 locationName1,
                                                                 new Point2D(0, 0),
                                                                 new DuneLocation.ConstructionProperties
                                                                 {
                                                                     CoastalAreaId = 0,
                                                                     Offset = 0,
                                                                     Orientation = 0,
                                                                     D50 = 0.000007
                                                                 })),
                    new DuneLocationCalculation(new DuneLocation(hydraulicBoundaryLocation2,
                                                                 locationName2,
                                                                 new Point2D(0, 0),
                                                                 new DuneLocation.ConstructionProperties
                                                                 {
                                                                     CoastalAreaId = 0,
                                                                     Offset = 0,
                                                                     Orientation = 0,
                                                                     D50 = 0.000007
                                                                 }))
                };

                var failureMechanism = new DuneErosionFailureMechanism
                {
                    Contribution = 10
                };

                var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
                {
                    FilePath = validFilePath,
                    Locations =
                    {
                        hydraulicBoundaryLocation1,
                        hydraulicBoundaryLocation2
                    }
                };
                HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(hydraulicBoundaryDatabase);

                var assessmentSection = mocks.Stub<IAssessmentSection>();
                assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase);
                assessmentSection.Stub(a => a.HydraulicBoundaryDatabases).Return(new ObservableList<HydraulicBoundaryDatabase>
                {
                    hydraulicBoundaryDatabase
                });
                assessmentSection.Stub(a => a.Id).Return("13-1");
                assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
                {
                    failureMechanism
                });
                assessmentSection.Stub(a => a.FailureMechanismContribution)
                                 .Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());

                var context = new DuneLocationCalculationsContext(duneLocationCalculations,
                                                                  failureMechanism,
                                                                  assessmentSection,
                                                                  () => 0.01,
                                                                  categoryBoundaryName);

                var builder = new CustomItemsOnlyContextMenuBuilder();

                var mainWindow = mocks.Stub<IMainWindow>();
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(builder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                var calculationObserver = mocks.StrictMock<IObserver>();
                calculationObserver.Expect(o => o.UpdateObserver()).Repeat.Times(duneLocationCalculations.Count);
                var calculationsObserver = mocks.StrictMock<IObserver>();

                int nrOfCalculators = duneLocationCalculations.Count;
                var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(null))
                                 .IgnoreArguments()
                                 .Return(new TestDunesBoundaryConditionsCalculator())
                                 .Repeat
                                 .Times(nrOfCalculators);
                mocks.ReplayAll();

                duneLocationCalculations.Attach(calculationsObserver);
                duneLocationCalculations.ForEachElementDo(location => location.Attach(calculationObserver));

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
                        Assert.AreEqual($"Hydraulische belastingen berekenen voor locatie '{locationName1}' (Categoriegrens {categoryBoundaryName}) is gestart.", messageList[0]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(messageList[1]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(messageList[2]);
                        CalculationServiceTestHelper.AssertCalculationStartMessage(messageList[3]);
                        Assert.AreEqual($"Hydraulische belastingenberekening voor locatie '{locationName1}' (Categoriegrens {categoryBoundaryName}) is niet geconvergeerd.", messageList[4]);
                        StringAssert.StartsWith("Hydraulische belastingenberekening is uitgevoerd op de tijdelijke locatie", messageList[5]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(messageList[6]);
                        Assert.AreEqual($"Hydraulische belastingen berekenen voor locatie '{locationName1}' (Categoriegrens {categoryBoundaryName}) is gelukt.", messageList[7]);

                        Assert.AreEqual($"Hydraulische belastingen berekenen voor locatie '{locationName2}' (Categoriegrens {categoryBoundaryName}) is gestart.", messageList[8]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(messageList[9]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(messageList[10]);
                        CalculationServiceTestHelper.AssertCalculationStartMessage(messageList[11]);
                        Assert.AreEqual($"Hydraulische belastingenberekening voor locatie '{locationName2}' (Categoriegrens {categoryBoundaryName}) is niet geconvergeerd.", messageList[12]);
                        StringAssert.StartsWith("Hydraulische belastingenberekening is uitgevoerd op de tijdelijke locatie", messageList[13]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(messageList[14]);
                        Assert.AreEqual($"Hydraulische belastingen berekenen voor locatie '{locationName2}' (Categoriegrens {categoryBoundaryName}) is gelukt.", messageList[15]);
                    });
                }
            }
        }

        [Test]
        public void PerformDuneLocationCalculationsFromContextMenu_HydraulicBoundaryDatabaseWithCanUsePreprocessorFalse_SendsRightInputToCalculationService()
        {
            // Setup
            const double norm = 0.01;
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1300001, "", 0, 0);
            var duneLocationCalculations = new ObservableList<DuneLocationCalculation>
            {
                new DuneLocationCalculation(new DuneLocation(hydraulicBoundaryLocation,
                                                             "A",
                                                             new Point2D(0, 0),
                                                             new DuneLocation.ConstructionProperties
                                                             {
                                                                 CoastalAreaId = 0,
                                                                 Offset = 0,
                                                                 Orientation = 0,
                                                                 D50 = 0.000007
                                                             }))
            };

            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = 10
            };

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Locations =
                {
                    hydraulicBoundaryLocation
                }
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(hydraulicBoundaryDatabase);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase);
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabases).Return(new ObservableList<HydraulicBoundaryDatabase>
            {
                hydraulicBoundaryDatabase
            });
            assessmentSection.Stub(a => a.Id).Return("13-1");
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });
            assessmentSection.Stub(a => a.FailureMechanismContribution)
                             .Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());

            var context = new DuneLocationCalculationsContext(duneLocationCalculations,
                                                              failureMechanism,
                                                              assessmentSection,
                                                              () => norm,
                                                              "A");

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());

                var dunesBoundaryConditionsCalculator = new TestDunesBoundaryConditionsCalculator();
                var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                                 .WhenCalled(invocation =>
                                 {
                                     HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                         HydraulicBoundaryCalculationSettingsFactory.CreateSettings(assessmentSection, hydraulicBoundaryLocation),
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

                    Assert.AreEqual(hydraulicBoundaryLocation.Id, dunesBoundaryConditionsCalculationInput.HydraulicBoundaryLocationId);
                    Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(norm), dunesBoundaryConditionsCalculationInput.Beta);
                }
            }
        }

        [Test]
        public void PerformDuneLocationCalculationsFromContextMenu_HydraulicBoundaryDatabaseWithUsePreprocessorTrue_SendsRightInputToCalculationService()
        {
            // Setup
            const double norm = 0.01;
            string preprocessorDirectory = TestHelper.GetScratchPadPath();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1300001, "", 0, 0);
            var duneLocationCalculations = new ObservableList<DuneLocationCalculation>
            {
                new DuneLocationCalculation(new DuneLocation(hydraulicBoundaryLocation,
                                                             "A",
                                                             new Point2D(0, 0),
                                                             new DuneLocation.ConstructionProperties
                {
                    CoastalAreaId = 0,
                    Offset = 0,
                    Orientation = 0,
                    D50 = 0.000007
                }))
            };

            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = 10
            };

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Locations =
                {
                    hydraulicBoundaryLocation
                },
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
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabases).Return(new ObservableList<HydraulicBoundaryDatabase>
            {
                hydraulicBoundaryDatabase
            });
            assessmentSection.Stub(a => a.Id).Return("13-1");
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });
            assessmentSection.Stub(a => a.FailureMechanismContribution)
                             .Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());

            var context = new DuneLocationCalculationsContext(duneLocationCalculations,
                                                              failureMechanism,
                                                              assessmentSection,
                                                              () => norm,
                                                              "A");

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());

                var dunesBoundaryConditionsCalculator = new TestDunesBoundaryConditionsCalculator();
                var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                                 .WhenCalled(invocation =>
                                 {
                                     HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                         HydraulicBoundaryCalculationSettingsFactory.CreateSettings(assessmentSection, hydraulicBoundaryLocation),
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

                    Assert.AreEqual(hydraulicBoundaryLocation.Id, dunesBoundaryConditionsCalculationInput.HydraulicBoundaryLocationId);
                    Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(norm), dunesBoundaryConditionsCalculationInput.Beta);
                }
            }
        }

        [Test]
        public void PerformDuneLocationCalculationsFromContextMenu_HydraulicBoundaryDatabaseWithUsePreprocessorFalse_SendsRightInputToCalculationService()
        {
            // Setup
            const double norm = 0.01;

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1300001, "", 0, 0);
            var duneLocationCalculations = new ObservableList<DuneLocationCalculation>
            {
                new DuneLocationCalculation(new DuneLocation(hydraulicBoundaryLocation,
                                                             "A",
                                                             new Point2D(0, 0),
                                                             new DuneLocation.ConstructionProperties
                                                             {
                                                                 CoastalAreaId = 0,
                                                                 Offset = 0,
                                                                 Orientation = 0,
                                                                 D50 = 0.000007
                                                             }))
            };

            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = 10
            };

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Locations =
                {
                    hydraulicBoundaryLocation
                },
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
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabases).Return(new ObservableList<HydraulicBoundaryDatabase>
            {
                hydraulicBoundaryDatabase
            });
            assessmentSection.Stub(a => a.Id).Return("13-1");
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });
            assessmentSection.Stub(a => a.FailureMechanismContribution)
                             .Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());

            var context = new DuneLocationCalculationsContext(duneLocationCalculations,
                                                              failureMechanism,
                                                              assessmentSection,
                                                              () => norm,
                                                              "A");

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());

                var dunesBoundaryConditionsCalculator = new TestDunesBoundaryConditionsCalculator();
                var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                                 .WhenCalled(invocation =>
                                 {
                                     HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                         HydraulicBoundaryCalculationSettingsFactory.CreateSettings(assessmentSection, hydraulicBoundaryLocation),
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

                    Assert.AreEqual(hydraulicBoundaryLocation.Id, dunesBoundaryConditionsCalculationInput.HydraulicBoundaryLocationId);
                    Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(norm), dunesBoundaryConditionsCalculationInput.Beta);
                }
            }
        }
    }
}