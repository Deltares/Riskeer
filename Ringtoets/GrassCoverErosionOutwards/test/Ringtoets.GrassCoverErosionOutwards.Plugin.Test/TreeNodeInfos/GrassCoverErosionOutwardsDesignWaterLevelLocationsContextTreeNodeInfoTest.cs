// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using Core.Common.Utils;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Forms.Properties;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.GrassCoverErosionOutwards.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class GrassCoverErosionOutwardsDesignWaterLevelLocationsContextTreeNodeInfoTest
    {
        private const int contextMenuRunDesignWaterLevelCalculationsIndex = 2;
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
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            var context = new GrassCoverErosionOutwardsDesignWaterLevelLocationsContext(
                new ObservableList<HydraulicBoundaryLocation>(),
                assessmentSectionMock,
                new GrassCoverErosionOutwardsFailureMechanism());

            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                string nodeText = info.Text(context);

                // Assert
                Assert.AreEqual("Waterstanden bij doorsnede-eis", nodeText);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Image_Always_ReturnGenericInputOutputIcon()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();
            var context = new GrassCoverErosionOutwardsDesignWaterLevelLocationsContext(
                new ObservableList<HydraulicBoundaryLocation>(),
                assessmentSectionMock,
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
            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();

            using (var treeViewControl = new TreeViewControl())
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                var context = new GrassCoverErosionOutwardsDesignWaterLevelLocationsContext(
                    new ObservableList<HydraulicBoundaryLocation>(),
                    assessmentSectionMock,
                    new GrassCoverErosionOutwardsFailureMechanism());

                var menuBuilder = mockRepository.StrictMock<IContextMenuBuilder>();
                menuBuilder.Expect(mb => mb.AddOpenItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.Build()).Return(null);

                var gui = mockRepository.StrictMock<IGui>();
                gui.Expect(cmp => cmp.Get(context, treeViewControl)).Return(menuBuilder);

                mockRepository.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (info.ContextMenuStrip(context, null, treeViewControl)) {}
            }

            // Assert
            mockRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_WithoutHydraulicDatabase_ReturnsContextMenuWithCommonItems()
        {
            // Setup
            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();

            var applicationFeatureCommandHandler = mockRepository.Stub<IApplicationFeatureCommands>();
            var importCommandHandler = mockRepository.Stub<IImportCommandHandler>();
            var exportCommandHandler = mockRepository.Stub<IExportCommandHandler>();
            var viewCommandsHandler = mockRepository.Stub<IViewCommands>();
            viewCommandsHandler.Stub(vch => vch.CanOpenViewFor(null)).IgnoreArguments().Return(true);

            using (var treeViewControl = new TreeViewControl())
            {
                using (var plugin = new GrassCoverErosionOutwardsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    var context = new GrassCoverErosionOutwardsDesignWaterLevelLocationsContext(
                        new ObservableList<HydraulicBoundaryLocation>(),
                        assessmentSectionMock,
                        new GrassCoverErosionOutwardsFailureMechanism());

                    var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler,
                                                             importCommandHandler,
                                                             exportCommandHandler,
                                                             viewCommandsHandler,
                                                             context,
                                                             treeViewControl);

                    var gui = mockRepository.StrictMock<IGui>();
                    gui.Expect(cmp => cmp.Get(context, treeViewControl)).Return(menuBuilder);

                    mockRepository.ReplayAll();

                    plugin.Gui = gui;

                    // Call
                    using (ContextMenuStrip menu = info.ContextMenuStrip(context, null, treeViewControl))
                    {
                        // Assert
                        Assert.AreEqual(5, menu.Items.Count);

                        TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                      0,
                                                                      CoreCommonGuiResources.Open,
                                                                      CoreCommonGuiResources.Open_ToolTip,
                                                                      CoreCommonGuiResources.OpenIcon);
                        TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                      2,
                                                                      Resources.GrassCoverErosionOutwardsWaterLevelLocation_Calculate_All,
                                                                      Resources.GrassCoverErosionOutwardsWaterLevelLocation_No_HRD_To_Calculate,
                                                                      RingtoetsCommonFormsResources.CalculateAllIcon,
                                                                      false);
                        TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                      4,
                                                                      CoreCommonGuiResources.Properties,
                                                                      CoreCommonGuiResources.Properties_ToolTip,
                                                                      CoreCommonGuiResources.PropertiesHS,
                                                                      false);

                        CollectionAssert.AllItemsAreInstancesOfType(new[]
                        {
                            menu.Items[1],
                            menu.Items[3],
                        }, typeof(ToolStripSeparator));
                    }
                }
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_WithHydraulicDatabase_EnabledCalculateAllAndTooltip()
        {
            // Setup
            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();
            assessmentSectionMock.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            var applicationFeatureCommandHandler = mockRepository.Stub<IApplicationFeatureCommands>();
            var importCommandHandler = mockRepository.Stub<IImportCommandHandler>();
            var exportCommandHandler = mockRepository.Stub<IExportCommandHandler>();
            var viewCommandsHandler = mockRepository.Stub<IViewCommands>();
            viewCommandsHandler.Stub(vch => vch.CanOpenViewFor(null)).IgnoreArguments().Return(true);

            using (var treeViewControl = new TreeViewControl())
            {
                using (var plugin = new GrassCoverErosionOutwardsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    var context = new GrassCoverErosionOutwardsDesignWaterLevelLocationsContext(
                        new ObservableList<HydraulicBoundaryLocation>(),
                        assessmentSectionMock,
                        new GrassCoverErosionOutwardsFailureMechanism());

                    var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler,
                                                             importCommandHandler,
                                                             exportCommandHandler,
                                                             viewCommandsHandler,
                                                             context,
                                                             treeViewControl);

                    var gui = mockRepository.StrictMock<IGui>();
                    gui.Expect(cmp => cmp.Get(context, treeViewControl)).Return(menuBuilder);

                    mockRepository.ReplayAll();

                    plugin.Gui = gui;

                    // Call
                    using (ContextMenuStrip menu = info.ContextMenuStrip(context, null, treeViewControl))
                    {
                        // Assert
                        Assert.AreEqual(5, menu.Items.Count);

                        TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                      0,
                                                                      CoreCommonGuiResources.Open,
                                                                      CoreCommonGuiResources.Open_ToolTip,
                                                                      CoreCommonGuiResources.OpenIcon);
                        TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                      2,
                                                                      Resources.GrassCoverErosionOutwardsWaterLevelLocation_Calculate_All,
                                                                      Resources.GrassCoverErosionOutwardsWaterLevelLocation_Calculate_All_ToolTip,
                                                                      RingtoetsCommonFormsResources.CalculateAllIcon);
                        TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                      4,
                                                                      CoreCommonGuiResources.Properties,
                                                                      CoreCommonGuiResources.Properties_ToolTip,
                                                                      CoreCommonGuiResources.PropertiesHS,
                                                                      false);

                        CollectionAssert.AllItemsAreInstancesOfType(new[]
                        {
                            menu.Items[1],
                            menu.Items[3],
                        }, typeof(ToolStripSeparator));
                    }
                }
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateDesignWaterLevelsFromContextMenu_Always_SendsRightInputToCalculationService()
        {
            // Setup
            string filePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite");

            var guiMock = mockRepository.DynamicMock<IGui>();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 1
            };

            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mockRepository, filePath);
            HydraulicBoundaryLocation grassCoverErosionOutwardsHydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First();
            var context = new GrassCoverErosionOutwardsDesignWaterLevelLocationsContext(new ObservableList<HydraulicBoundaryLocation>
            {
                grassCoverErosionOutwardsHydraulicBoundaryLocation
            }, assessmentSectionStub, failureMechanism);

            var observer = mockRepository.StrictMock<IObserver>();
            context.Attach(observer);
            observer.Expect(o => o.UpdateObserver());

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(g => g.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                guiMock.Expect(g => g.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                mockRepository.ReplayAll();

                using (var plugin = new GrassCoverErosionOutwardsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = guiMock;
                    plugin.Activate();

                    using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(context, null, treeViewControl))
                    using (new HydraRingCalculatorFactoryConfig())
                    {
                        var testFactory = (TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance;

                        // Call
                        contextMenuAdapter.Items[contextMenuRunDesignWaterLevelCalculationsIndex].PerformClick();

                        // Assert
                        var testDesignWaterLevelCalculator = testFactory.DesignWaterLevelCalculator;
                        var designWaterLevelCalculationInput = testDesignWaterLevelCalculator.ReceivedInputs.First();

                        Assert.AreEqual(testDataPath, testDesignWaterLevelCalculator.HydraulicBoundaryDatabaseDirectory);
                        Assert.AreEqual(assessmentSectionStub.Id, testDesignWaterLevelCalculator.RingId);

                        Assert.AreEqual(grassCoverErosionOutwardsHydraulicBoundaryLocation.Id, designWaterLevelCalculationInput.HydraulicBoundaryLocationId);
                        var expectedProbability = assessmentSectionStub.FailureMechanismContribution.Norm
                                                  *(failureMechanism.Contribution/100)/
                                                  failureMechanism.GeneralInput.N;
                        Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(expectedProbability), designWaterLevelCalculationInput.Beta);
                    }
                }
            }
            mockRepository.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        public void GivenHydraulicBoundaryDatabaseWithNonExistingFilePath_WhenCalculatingDesignWaterLevelFromContextMenu_ThenLogMessagesAddedPreviousOutputNotAffected()
        {
            // Given
            var guiMock = mockRepository.DynamicMock<IGui>();
            RoundedDouble designWaterLevel = (RoundedDouble) 4.2;
            string filePath = "D:/nonExistingDirectory/nonExistingFile";

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 1
            };
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(
                failureMechanism, mockRepository, filePath);

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSectionStub.HydraulicBoundaryDatabase;
            hydraulicBoundaryDatabase.Locations.Add(TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(
                designWaterLevel));

            var grassCoverErosionOutwardsHydraulicBoundaryLocation1 = hydraulicBoundaryDatabase.Locations[0];
            var grassCoverErosionOutwardsHydraulicBoundaryLocation2 = hydraulicBoundaryDatabase.Locations[1];
            var grassCoverErosionOutwardsHydraulicBoundaryLocations = new ObservableList<HydraulicBoundaryLocation>
            {
                grassCoverErosionOutwardsHydraulicBoundaryLocation1,
                grassCoverErosionOutwardsHydraulicBoundaryLocation2
            };
            var context = new GrassCoverErosionOutwardsDesignWaterLevelLocationsContext(
                grassCoverErosionOutwardsHydraulicBoundaryLocations,
                assessmentSectionStub,
                failureMechanism);

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(g => g.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                guiMock.Expect(g => g.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                mockRepository.ReplayAll();

                using (var plugin = new GrassCoverErosionOutwardsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = guiMock;
                    plugin.Activate();

                    using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(context, null, treeViewControl))
                    {
                        // When
                        Action action = () => contextMenuAdapter.Items[contextMenuRunDesignWaterLevelCalculationsIndex].PerformClick();

                        // Then
                        string message = string.Format("Berekeningen konden niet worden gestart. Fout bij het lezen van bestand '{0}': het bestand bestaat niet.",
                                                       filePath);
                        TestHelper.AssertLogMessageWithLevelIsGenerated(action, new Tuple<string, LogLevelConstant>(message, LogLevelConstant.Error));

                        Assert.IsNaN(grassCoverErosionOutwardsHydraulicBoundaryLocation1.DesignWaterLevel); // No result set

                        // Previous result not cleared
                        Assert.AreEqual(designWaterLevel, grassCoverErosionOutwardsHydraulicBoundaryLocation2.DesignWaterLevel,
                                        grassCoverErosionOutwardsHydraulicBoundaryLocation2.DesignWaterLevel.GetAccuracy());
                    }
                }
            }
            mockRepository.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        public void GivenHydraulicBoundaryLocationSucceeds_CalculatingDesignWaterLevelFromContextMenu_ThenLogMessagesAddedOutputSet()
        {
            // Given
            var guiMock = mockRepository.DynamicMock<IGui>();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 1
            };
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(
                failureMechanism, mockRepository, Path.Combine(testDataPath, "HRD ijsselmeer.sqlite"));

            var hydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations[0];
            var context = new GrassCoverErosionOutwardsDesignWaterLevelLocationsContext(new ObservableList<HydraulicBoundaryLocation>
            {
                hydraulicBoundaryLocation
            }, assessmentSectionStub, failureMechanism);

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(g => g.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                guiMock.Expect(g => g.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                mockRepository.ReplayAll();

                using (var plugin = new GrassCoverErosionOutwardsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = guiMock;
                    plugin.Activate();

                    using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(context, null, treeViewControl))
                    using (new HydraRingCalculatorFactoryConfig())
                    {
                        // When
                        Action call = () => contextMenuAdapter.Items[contextMenuRunDesignWaterLevelCalculationsIndex].PerformClick();

                        // Then
                        TestHelper.AssertLogMessages(call, messages =>
                        {
                            var msgs = messages.ToArray();
                            Assert.AreEqual(7, msgs.Length);
                            StringAssert.StartsWith(string.Format("Validatie van 'Waterstand bij doorsnede-eis voor locatie '{0}'' gestart om: ",
                                                                  hydraulicBoundaryLocation.Name), msgs[0]);
                            StringAssert.StartsWith(string.Format("Validatie van 'Waterstand bij doorsnede-eis voor locatie '{0}'' beëindigd om: ",
                                                                  hydraulicBoundaryLocation.Name), msgs[1]);
                            StringAssert.StartsWith(string.Format("Berekening van 'Waterstand bij doorsnede-eis voor locatie '{0}'' gestart om: ",
                                                                  hydraulicBoundaryLocation.Name), msgs[2]);
                            Assert.AreEqual(string.Format("Waterstand bij doorsnede-eis berekening voor locatie '{0}' is niet geconvergeerd.",
                                                          hydraulicBoundaryLocation.Name), msgs[3]);
                            StringAssert.StartsWith("Toetspeil berekening is uitgevoerd op de tijdelijke locatie",
                                                    msgs[4]);
                            StringAssert.StartsWith(string.Format("Berekening van 'Waterstand bij doorsnede-eis voor locatie '{0}'' beëindigd om: ",
                                                                  hydraulicBoundaryLocation.Name), msgs[5]);
                            StringAssert.AreNotEqualIgnoringCase(string.Format("Uitvoeren van '{0}' is gelukt.",
                                                                               hydraulicBoundaryLocation.Name), msgs[6]);
                        });
                        Assert.AreEqual(0, hydraulicBoundaryLocation.DesignWaterLevel,
                                        hydraulicBoundaryLocation.DesignWaterLevel.GetAccuracy());
                        Assert.AreEqual(CalculationConvergence.CalculatedNotConverged, hydraulicBoundaryLocation.DesignWaterLevelCalculationConvergence);
                    }
                }
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateDesignWaterLevelsFromContextMenu_ContributionZero_DoesNotCalculateAndLog()
        {
            // Setup
            var guiMock = mockRepository.DynamicMock<IGui>();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 0
            };
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(
                failureMechanism, mockRepository, Path.Combine(testDataPath, "HRD ijsselmeer.sqlite"));

            var grassCoverErosionOutwardsHydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations[0];
            var context = new GrassCoverErosionOutwardsDesignWaterLevelLocationsContext(new ObservableList<HydraulicBoundaryLocation>
            {
                grassCoverErosionOutwardsHydraulicBoundaryLocation
            }, assessmentSectionStub, failureMechanism);

            var contextObserverMock = mockRepository.StrictMock<IObserver>();
            context.Attach(contextObserverMock);

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(g => g.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                guiMock.Expect(g => g.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                mockRepository.ReplayAll();

                using (var plugin = new GrassCoverErosionOutwardsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = guiMock;
                    plugin.Activate();

                    using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(context, null, treeViewControl))
                    using (new HydraRingCalculatorFactoryConfig())
                    {
                        var testFactory = (TestHydraRingCalculatorFactory) HydraRingCalculatorFactory.Instance;

                        // Call
                        Action action = () => contextMenuAdapter.Items[contextMenuRunDesignWaterLevelCalculationsIndex].PerformClick();

                        // Assert
                        TestHelper.AssertLogMessageIsGenerated(action, "De bijdrage van dit toetsspoor is nul. Daardoor is de doorsnede-eis onbepaald en kunnen de berekeningen niet worden uitgevoerd.");

                        var testDesignWaterLevelCalculator = testFactory.DesignWaterLevelCalculator;
                        Assert.IsEmpty(testDesignWaterLevelCalculator.ReceivedInputs);

                        Assert.IsNullOrEmpty(testDesignWaterLevelCalculator.HydraulicBoundaryDatabaseDirectory);
                        Assert.IsNullOrEmpty(testDesignWaterLevelCalculator.RingId);
                    }
                }
            }
            mockRepository.VerifyAll();
        }

        private static TreeNodeInfo GetInfo(GrassCoverErosionOutwardsPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(GrassCoverErosionOutwardsDesignWaterLevelLocationsContext));
        }
    }
}