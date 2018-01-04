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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Data.TestUtil;
using Ringtoets.DuneErosion.Forms.PresentationObjects;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.DuneErosion.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class DuneLocationsContextTreeNodeInfoTest
    {
        private const int contextMenuCalculateAllIndex = 4;

        private MockRepository mocks;
        private DuneErosionPlugin plugin;
        private TreeNodeInfo info;

        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "HydraulicBoundaryDatabaseImporter");

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new DuneErosionPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(DuneLocationsContext));
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
            Assert.IsNotNull(info.ForeColor);
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

        [Test]
        public void Text_Always_ReturnsName()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var mechanism = new DuneErosionFailureMechanism();
            var context = new DuneLocationsContext(mechanism.DuneLocations, mechanism, assessmentSection);

            // Call
            string text = info.Text(context);

            // Assert
            Assert.AreEqual("Hydraulische randvoorwaarden", text);
        }

        [Test]
        public void Image_Always_ReturnsGenericInputOutputIcon()
        {
            // Setup
            mocks.ReplayAll();

            // Call
            Image image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GenericInputOutputIcon, image);
        }

        [Test]
        public void ForeColor_NoLocations_ReturnGrayText()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var mechanism = new DuneErosionFailureMechanism();
            var context = new DuneLocationsContext(mechanism.DuneLocations, mechanism, assessmentSection);

            // Call
            Color textColor = info.ForeColor(context);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), textColor);
        }

        [Test]
        public void ForeColor_WithLocations_ReturnControlText()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var mechanism = new DuneErosionFailureMechanism();
            mechanism.DuneLocations.Add(new TestDuneLocation());
            var context = new DuneLocationsContext(mechanism.DuneLocations, mechanism, assessmentSection);

            // Call
            Color textColor = info.ForeColor(context);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), textColor);
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var failureMechanism = new DuneErosionFailureMechanism();
                var context = new DuneLocationsContext(failureMechanism.DuneLocations, failureMechanism, assessmentSection);

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
                IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(null, mocks, "invalidFilePath");

                var failureMechanism = new DuneErosionFailureMechanism
                {
                    Contribution = 10
                };

                var builder = new CustomItemsOnlyContextMenuBuilder();
                var context = new DuneLocationsContext(failureMechanism.DuneLocations, failureMechanism, assessmentSection);

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(builder);

                mocks.ReplayAll();

                plugin.Gui = gui;

                failureMechanism.DuneLocations.Add(new TestDuneLocation());

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Assert
                    ToolStripItem contextMenuItem = menu.Items[contextMenuCalculateAllIndex];

                    Assert.AreEqual("Alles be&rekenen", contextMenuItem.Text);
                    StringAssert.Contains("Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt.", contextMenuItem.ToolTipText);
                    TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.CalculateAllIcon, contextMenuItem.Image);
                    Assert.IsFalse(contextMenuItem.Enabled);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_NoDuneLocations_ContextMenuItemCalculateAllDisabledAndTooltipSet()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSection = mocks.Stub<IAssessmentSection>();

                assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
                {
                    FilePath = validFilePath,
                    Version = "1.0"
                });

                var failureMechanism = new DuneErosionFailureMechanism
                {
                    Contribution = 10
                };

                var builder = new CustomItemsOnlyContextMenuBuilder();
                var context = new DuneLocationsContext(failureMechanism.DuneLocations, failureMechanism, assessmentSection);

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
                                                                  "Er zijn geen locaties om een berekening voor uit te voeren.",
                                                                  RingtoetsCommonFormsResources.CalculateAllIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismContributionZero_ContextMenuItemCalculateAllDisabledAndTooltipSet()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSection = mocks.Stub<IAssessmentSection>();

                assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
                {
                    FilePath = validFilePath,
                    Version = "1.0"
                });

                var failureMechanism = new DuneErosionFailureMechanism();

                var builder = new CustomItemsOnlyContextMenuBuilder();
                var context = new DuneLocationsContext(failureMechanism.DuneLocations, failureMechanism, assessmentSection);

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(builder);

                mocks.ReplayAll();

                plugin.Gui = gui;

                failureMechanism.DuneLocations.Add(new TestDuneLocation());

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndex,
                                                                  "Alles be&rekenen",
                                                                  "De bijdrage van dit toetsspoor is nul.",
                                                                  RingtoetsCommonFormsResources.CalculateAllIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_AllRequiredInputSet_ContextMenuItemCalculateAllEnabled()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSection = mocks.Stub<IAssessmentSection>();

                assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
                {
                    FilePath = validFilePath,
                    Version = "1.0"
                });

                var failureMechanism = new DuneErosionFailureMechanism
                {
                    Contribution = 10
                };

                var builder = new CustomItemsOnlyContextMenuBuilder();
                var context = new DuneLocationsContext(failureMechanism.DuneLocations, failureMechanism, assessmentSection);

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(builder);

                mocks.ReplayAll();

                plugin.Gui = gui;

                failureMechanism.DuneLocations.Add(new TestDuneLocation());

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndex,
                                                                  "Alles be&rekenen",
                                                                  "Alle hydraulische randvoorwaarden berekenen.",
                                                                  RingtoetsCommonFormsResources.CalculateAllIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_ClickOnCalculateAllItem_ScheduleAllLocationsAndNotifyObservers()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

                var failureMechanism = new DuneErosionFailureMechanism
                {
                    Contribution = 10
                };
                failureMechanism.DuneLocations.AddRange(new[]
                {
                    new DuneLocation(1300001, "A", new Point2D(0, 0), new DuneLocation.ConstructionProperties
                    {
                        CoastalAreaId = 0,
                        Offset = 0,
                        Orientation = 0,
                        D50 = 0.000007
                    }),
                    new DuneLocation(1300002, "B", new Point2D(0, 0), new DuneLocation.ConstructionProperties
                    {
                        CoastalAreaId = 0,
                        Offset = 0,
                        Orientation = 0,
                        D50 = 0.000007
                    })
                });

                var assessmentSection = mocks.Stub<IAssessmentSection>();

                assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
                {
                    FilePath = validFilePath
                });
                assessmentSection.Stub(a => a.Id).Return("13-1");
                assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
                {
                    failureMechanism
                });
                assessmentSection.Stub(a => a.FailureMechanismContribution)
                                 .Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution(new[]
                                 {
                                     failureMechanism
                                 }));

                var context = new DuneLocationsContext(failureMechanism.DuneLocations, failureMechanism, assessmentSection);

                var builder = new CustomItemsOnlyContextMenuBuilder();

                var mainWindow = mocks.Stub<IMainWindow>();
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(builder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                var locationObserver = mocks.StrictMock<IObserver>();
                locationObserver.Expect(o => o.UpdateObserver()).Repeat.Times(failureMechanism.DuneLocations.Count);
                var locationsObsever = mocks.StrictMock<IObserver>();

                int nrOfCalculators = failureMechanism.DuneLocations.Count;
                var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(testDataPath, string.Empty))
                                 .Return(new TestDunesBoundaryConditionsCalculator())
                                 .Repeat
                                 .Times(nrOfCalculators);
                mocks.ReplayAll();

                failureMechanism.DuneLocations.Attach(locationsObsever);
                failureMechanism.DuneLocations.ForEachElementDo(location => location.Attach(locationObserver));

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
                        Assert.AreEqual("Hydraulische randvoorwaarden berekenen voor locatie 'A' is gestart.", messageList[0]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(messageList[1]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(messageList[2]);
                        CalculationServiceTestHelper.AssertCalculationStartMessage(messageList[3]);
                        Assert.AreEqual("Hydraulische randvoorwaarden berekening voor locatie 'A' is niet geconvergeerd.", messageList[4]);
                        StringAssert.StartsWith("Hydraulische randvoorwaarden berekening is uitgevoerd op de tijdelijke locatie", messageList[5]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(messageList[6]);

                        Assert.AreEqual("Hydraulische randvoorwaarden berekenen voor locatie 'B' is gestart.", messageList[7]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(messageList[8]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(messageList[9]);
                        CalculationServiceTestHelper.AssertCalculationStartMessage(messageList[10]);
                        Assert.AreEqual("Hydraulische randvoorwaarden berekening voor locatie 'B' is niet geconvergeerd.", messageList[11]);
                        StringAssert.StartsWith("Hydraulische randvoorwaarden berekening is uitgevoerd op de tijdelijke locatie", messageList[12]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(messageList[13]);

                        Assert.AreEqual("Hydraulische randvoorwaarden berekenen voor locatie 'A' is gelukt.", messageList[14]);
                        Assert.AreEqual("Hydraulische randvoorwaarden berekenen voor locatie 'B' is gelukt.", messageList[15]);
                    });
                }
            }
        }

        [Test]
        public void CalculateDuneLocationsFromContextMenu_HydraulicBoundaryDatabaseWithCanUsePreprocessorFalse_SendsRightInputToCalculationService()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var duneLocation = new DuneLocation(1300001, "A", new Point2D(0, 0), new DuneLocation.ConstructionProperties
            {
                CoastalAreaId = 0,
                Offset = 0,
                Orientation = 0,
                D50 = 0.000007
            });

            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = 10,
                DuneLocations =
                {
                    duneLocation
                }
            };

            var assessmentSection = mocks.Stub<IAssessmentSection>();

            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath
            });
            assessmentSection.Stub(a => a.Id).Return("13-1");
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });
            assessmentSection.Stub(a => a.FailureMechanismContribution)
                             .Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution(new[]
                             {
                                 failureMechanism
                             }));

            var context = new DuneLocationsContext(failureMechanism.DuneLocations, failureMechanism, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());

                var dunesBoundaryConditionsCalculator = new TestDunesBoundaryConditionsCalculator();
                var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(testDataPath, string.Empty))
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

                    Assert.AreEqual(duneLocation.Id, dunesBoundaryConditionsCalculationInput.HydraulicBoundaryLocationId);
                    double expectedProbability = failureMechanism.GetMechanismSpecificNorm(assessmentSection.FailureMechanismContribution.Norm);
                    Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(expectedProbability), dunesBoundaryConditionsCalculationInput.Beta);
                }
            }
        }

        [Test]
        public void CalculateDuneLocationsFromContextMenu_HydraulicBoundaryDatabaseWithUsePreprocessorTrue_SendsRightInputToCalculationService()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");
            string preprocessorDirectory = TestHelper.GetScratchPadPath();

            var duneLocation = new DuneLocation(1300001, "A", new Point2D(0, 0), new DuneLocation.ConstructionProperties
            {
                CoastalAreaId = 0,
                Offset = 0,
                Orientation = 0,
                D50 = 0.000007
            });

            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = 10,
                DuneLocations =
                {
                    duneLocation
                }
            };

            var assessmentSection = mocks.Stub<IAssessmentSection>();

            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                CanUsePreprocessor = true,
                UsePreprocessor = true,
                PreprocessorDirectory = preprocessorDirectory
            });
            assessmentSection.Stub(a => a.Id).Return("13-1");
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });
            assessmentSection.Stub(a => a.FailureMechanismContribution)
                             .Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution(new[]
                             {
                                 failureMechanism
                             }));

            var context = new DuneLocationsContext(failureMechanism.DuneLocations, failureMechanism, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());

                var dunesBoundaryConditionsCalculator = new TestDunesBoundaryConditionsCalculator();
                var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(testDataPath, preprocessorDirectory))
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

                    Assert.AreEqual(duneLocation.Id, dunesBoundaryConditionsCalculationInput.HydraulicBoundaryLocationId);
                    double expectedProbability = failureMechanism.GetMechanismSpecificNorm(assessmentSection.FailureMechanismContribution.Norm);
                    Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(expectedProbability), dunesBoundaryConditionsCalculationInput.Beta);
                }
            }
        }

        [Test]
        public void CalculateDuneLocationsFromContextMenu_HydraulicBoundaryDatabaseWithUsePreprocessorFalse_SendsRightInputToCalculationService()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var duneLocation = new DuneLocation(1300001, "A", new Point2D(0, 0), new DuneLocation.ConstructionProperties
            {
                CoastalAreaId = 0,
                Offset = 0,
                Orientation = 0,
                D50 = 0.000007
            });

            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = 10,
                DuneLocations =
                {
                    duneLocation
                }
            };

            var assessmentSection = mocks.Stub<IAssessmentSection>();

            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                CanUsePreprocessor = true,
                UsePreprocessor = false,
                PreprocessorDirectory = "InvalidPreprocessorDirectory"
            });
            assessmentSection.Stub(a => a.Id).Return("13-1");
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });
            assessmentSection.Stub(a => a.FailureMechanismContribution)
                             .Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution(new[]
                             {
                                 failureMechanism
                             }));

            var context = new DuneLocationsContext(failureMechanism.DuneLocations, failureMechanism, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());

                var dunesBoundaryConditionsCalculator = new TestDunesBoundaryConditionsCalculator();
                var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(testDataPath, string.Empty))
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

                    Assert.AreEqual(duneLocation.Id, dunesBoundaryConditionsCalculationInput.HydraulicBoundaryLocationId);
                    double expectedProbability = failureMechanism.GetMechanismSpecificNorm(assessmentSection.FailureMechanismContribution.Norm);
                    Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(expectedProbability), dunesBoundaryConditionsCalculationInput.Beta);
                }
            }
        }
    }
}