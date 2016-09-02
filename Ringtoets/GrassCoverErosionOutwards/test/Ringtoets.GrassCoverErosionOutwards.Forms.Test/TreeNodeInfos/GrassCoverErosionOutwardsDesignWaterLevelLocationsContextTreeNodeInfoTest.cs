﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Forms.Properties;
using Ringtoets.GrassCoverErosionOutwards.Plugin;
using Ringtoets.HydraRing.Calculation.TestUtil;
using Ringtoets.HydraRing.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class GrassCoverErosionOutwardsDesignWaterLevelLocationsContextTreeNodeInfoTest
    {
        private const int contextMenuRunDesignWaterLevelCalculationsIndex = 0;
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
                Assert.AreEqual(typeof(GrassCoverErosionOutwardsDesignWaterLevelLocationsContext), info.TagType);

                Assert.IsNull(info.EnsureVisibleOnCreate);
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
                new ObservableList<GrassCoverErosionOutwardsHydraulicBoundaryLocation>(),
                assessmentSectionMock);

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
                new ObservableList<GrassCoverErosionOutwardsHydraulicBoundaryLocation>(),
                assessmentSectionMock);
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
            {
                using (var plugin = new GrassCoverErosionOutwardsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    var context = new GrassCoverErosionOutwardsDesignWaterLevelLocationsContext(
                        new ObservableList<GrassCoverErosionOutwardsHydraulicBoundaryLocation>(),
                        assessmentSectionMock);

                    var menuBuilder = mockRepository.StrictMock<IContextMenuBuilder>();
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

            using (var treeViewControl = new TreeViewControl())
            {
                using (var plugin = new GrassCoverErosionOutwardsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    var context = new GrassCoverErosionOutwardsDesignWaterLevelLocationsContext(
                        new ObservableList<GrassCoverErosionOutwardsHydraulicBoundaryLocation>(),
                        assessmentSectionMock);

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
                        Assert.AreEqual(3, menu.Items.Count);

                        TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                      0,
                                                                      Resources.GrassCoverErosionOutwardsWaterLevelLocation_Calculate_All,
                                                                      Resources.GrassCoverErosionOutwardsWaterLevelLocation_No_HRD_To_Calculate,
                                                                      RingtoetsCommonFormsResources.CalculateAllIcon,
                                                                      false);
                        TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                      2,
                                                                      CoreCommonGuiResources.Properties,
                                                                      CoreCommonGuiResources.Properties_ToolTip,
                                                                      CoreCommonGuiResources.PropertiesHS,
                                                                      false);

                        CollectionAssert.AllItemsAreInstancesOfType(new[]
                        {
                            menu.Items[1],
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

            using (var treeViewControl = new TreeViewControl())
            {
                using (var plugin = new GrassCoverErosionOutwardsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    var context = new GrassCoverErosionOutwardsDesignWaterLevelLocationsContext(
                        new ObservableList<GrassCoverErosionOutwardsHydraulicBoundaryLocation>(),
                        assessmentSectionMock);

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
                        Assert.AreEqual(3, menu.Items.Count);

                        TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                      0,
                                                                      Resources.GrassCoverErosionOutwardsWaterLevelLocation_Calculate_All,
                                                                      Resources.GrassCoverErosionOutwardsWaterLevelLocation_Calculate_All_ToolTip,
                                                                      RingtoetsCommonFormsResources.CalculateAllIcon);
                        TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                      2,
                                                                      CoreCommonGuiResources.Properties,
                                                                      CoreCommonGuiResources.Properties_ToolTip,
                                                                      CoreCommonGuiResources.PropertiesHS,
                                                                      false);

                        CollectionAssert.AllItemsAreInstancesOfType(new[]
                        {
                            menu.Items[1],
                        }, typeof(ToolStripSeparator));
                    }
                }
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void ForeColor_ContextHasLocationsData_ReturnControlText()
        {
            // Setup
            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "", 0, 0);
            HydraulicBoundaryDatabase database = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    hydraulicBoundaryLocation
                }
            };
            assessmentSectionMock.HydraulicBoundaryDatabase = database;
            mockRepository.ReplayAll();
            var locations = new ObservableList<GrassCoverErosionOutwardsHydraulicBoundaryLocation>
            {
                new GrassCoverErosionOutwardsHydraulicBoundaryLocation(hydraulicBoundaryLocation)
            };
            var context = new GrassCoverErosionOutwardsDesignWaterLevelLocationsContext(locations, assessmentSectionMock);
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Color color = info.ForeColor(context);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), color);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void ForeColor_ContextHasNoLocationsData_ReturnGrayText()
        {
            // Setup
            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();
            var context = new GrassCoverErosionOutwardsDesignWaterLevelLocationsContext(
                new ObservableList<GrassCoverErosionOutwardsHydraulicBoundaryLocation>(),
                assessmentSectionMock);
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Color color = info.ForeColor(context);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), color);
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

            var hydraulicBoundaryLocation1 = new HydraulicBoundaryLocation(100001, "", 1.1, 2.2);
            var hydraulicBoundaryLocation2 = new HydraulicBoundaryLocation(100002, "", 3.3, 4.4);
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = "D:/nonExistingDirectory/nonExistingFile"
            };

            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();
            assessmentSectionMock.Expect(a => a.Id).Return("Id");
            assessmentSectionMock.Expect(a => a.GetFailureMechanisms()).Return(new[]
            {
                new GrassCoverErosionOutwardsFailureMechanism
                {
                    Contribution = 1
                }
            });
            assessmentSectionMock
                .Expect(a => a.FailureMechanismContribution)
                .Return(new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 1, 5));

            assessmentSectionMock.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;
            var grassCoverErosionOutwardsHydraulicBoundaryLocation1 = new GrassCoverErosionOutwardsHydraulicBoundaryLocation(hydraulicBoundaryLocation1);
            var grassCoverErosionOutwardsHydraulicBoundaryLocation2 = new GrassCoverErosionOutwardsHydraulicBoundaryLocation(hydraulicBoundaryLocation2)
            {
                DesignWaterLevel = designWaterLevel
            };

            var grassCoverErosionOutwardsHydraulicBoundaryLocations = new ObservableList<GrassCoverErosionOutwardsHydraulicBoundaryLocation>
            {
                grassCoverErosionOutwardsHydraulicBoundaryLocation1,
                grassCoverErosionOutwardsHydraulicBoundaryLocation2
            };
            var context = new GrassCoverErosionOutwardsDesignWaterLevelLocationsContext(grassCoverErosionOutwardsHydraulicBoundaryLocations, assessmentSectionMock);

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
                        Action action = () => { contextMenuAdapter.Items[contextMenuRunDesignWaterLevelCalculationsIndex].PerformClick(); };

                        // Then
                        string message = string.Format("Berekeningen konden niet worden gestart. Fout bij het lezen van bestand '{0}': Het bestand bestaat niet.",
                                                       hydraulicBoundaryDatabase.FilePath);
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
        public void GivenHydraulicBoundaryLocationThatFails_CalculatingDesignWaterLevelFromContextMenu_ThenLogMessagesAddedPreviousOutputNotAffected()
        {
            // Given
            var guiMock = mockRepository.DynamicMock<IGui>();

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite")
            };

            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();
            assessmentSectionMock.Expect(a => a.Id).Return("Id");
            assessmentSectionMock.Expect(a => a.GetFailureMechanisms()).Return(new[]
            {
                new GrassCoverErosionOutwardsFailureMechanism
                {
                    Contribution = 1
                }
            });
            assessmentSectionMock.Expect(a => a.FailureMechanismContribution).Return(new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 1, 300));
            assessmentSectionMock.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;

            const string locationName = "HRbasis_ijsslm_1000";
            var location = new GrassCoverErosionOutwardsHydraulicBoundaryLocation(
                new HydraulicBoundaryLocation(700002, locationName, 1.1, 2.2));
            var grassCoverErosionOutwardsHydraulicBoundaryLocations = new ObservableList<GrassCoverErosionOutwardsHydraulicBoundaryLocation>
            {
                location
            };
            var context = new GrassCoverErosionOutwardsDesignWaterLevelLocationsContext(grassCoverErosionOutwardsHydraulicBoundaryLocations, assessmentSectionMock);

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
                    using (new HydraRingCalculationServiceConfig())
                    {
                        // When
                        Action action = () => { contextMenuAdapter.Items[contextMenuRunDesignWaterLevelCalculationsIndex].PerformClick(); };

                        // Then
                        TestHelper.AssertLogMessages(action, messages =>
                        {
                            var msgs = messages.ToArray();
                            Assert.AreEqual(6, msgs.Length);
                            StringAssert.StartsWith(string.Format("Validatie van 'Waterstand bij doorsnede-eis voor locatie {0}' gestart om:", locationName), msgs[0]);
                            StringAssert.StartsWith(string.Format("Validatie van 'Waterstand bij doorsnede-eis voor locatie {0}' beëindigd om:", locationName), msgs[1]);
                            StringAssert.StartsWith(string.Format("Berekening van 'Waterstand bij doorsnede-eis voor locatie {0}' gestart om:", locationName), msgs[2]);
                            StringAssert.StartsWith(string.Format("Er is een fout opgetreden tijdens de Waterstand bij doorsnede-eis berekening '{0}': inspecteer het logbestand.", locationName), msgs[3]);
                            StringAssert.StartsWith(string.Format("Berekening van 'Waterstand bij doorsnede-eis voor locatie {0}' beëindigd om:", locationName), msgs[4]);
                            StringAssert.StartsWith(string.Format("Uitvoeren van 'Waterstand bij doorsnede-eis berekenen voor locatie '{0}'' is mislukt.", locationName), msgs[5]);
                        });
                        Assert.AreEqual(CalculationConvergence.NotCalculated, location.DesignWaterLevelCalculationConvergence);
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