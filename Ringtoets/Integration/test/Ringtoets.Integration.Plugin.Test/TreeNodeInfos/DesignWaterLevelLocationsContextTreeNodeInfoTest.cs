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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.PresentationObjects;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class DesignWaterLevelLocationsContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuRunAssessmentLevelCalculationsIndex = 2;
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
            using (var plugin = new RingtoetsPlugin())
            {
                var info = GetInfo(plugin);

                // Assert
                Assert.AreEqual(typeof(DesignWaterLevelLocationsContext), info.TagType);
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
        }

        [Test]
        public void Text_Always_ReturnsSetName()
        {
            // Setup
            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                var text = info.Text(null);

                // Assert
                Assert.AreEqual("Toetspeilen", text);
            }
        }

        [Test]
        public void Image_Always_ReturnsGenericIcon()
        {
            // Setup
            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                var image = info.Image(null);

                // Assert
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GenericInputOutputIcon, image);
            }
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var guiMock = mockRepository.StrictMock<IGui>();
            var menuBuilderMock = mockRepository.StrictMock<IContextMenuBuilder>();
            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();

            var nodeData = new DesignWaterLevelLocationsContext(assessmentSectionMock);

            menuBuilderMock.Expect(mb => mb.AddOpenItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);
            guiMock.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            guiMock.Stub(g => g.ProjectOpened -= null).IgnoreArguments();

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilderMock);

                mockRepository.ReplayAll();

                using (var plugin = new RingtoetsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    plugin.Gui = guiMock;

                    // Call
                    info.ContextMenuStrip(nodeData, null, treeViewControl);
                }
            }
            // Assert
            mockRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_NoHydraulicBoundaryDatabaseSet_ContextMenuItemBerekenenDisabled()
        {
            // Setup
            var guiMock = mockRepository.StrictMock<IGui>();
            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();

            var nodeData = new DesignWaterLevelLocationsContext(assessmentSectionMock);

            guiMock.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            guiMock.Stub(g => g.ProjectOpened -= null).IgnoreArguments();

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mockRepository.ReplayAll();

                using (var plugin = new RingtoetsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    plugin.Gui = guiMock;

                    // Call
                    using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                    {
                        // Assert
                        const string expectedItemText = "Alles be&rekenen";
                        const string expectedItemTooltip = "Er is geen hydraulische randvoorwaardendatabase beschikbaar om de toetspeilen te berekenen.";

                        TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuRunAssessmentLevelCalculationsIndex,
                                                                      expectedItemText, expectedItemTooltip, RingtoetsCommonFormsResources.CalculateAllIcon, false);
                    }
                }
            }

            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void ContextMenuStrip_HydraulicBoundaryDatabaseSet_ContextMenuItemBerekenenEnabled()
        {
            // Setup
            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();

            var nodeData = new DesignWaterLevelLocationsContext(assessmentSectionMock)
            {
                WrappedData =
                {
                    HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
                }
            };

            using (var treeViewControl = new TreeViewControl())
            {
                var guiMock = mockRepository.StrictMock<IGui>();
                guiMock.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                guiMock.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mockRepository.ReplayAll();

                using (var plugin = new RingtoetsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    plugin.Gui = guiMock;

                    // Call
                    using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                    {
                        // Assert
                        const string expectedItemText = @"Alles be&rekenen";
                        const string expectedItemTooltip = @"Alle toetspeilen berekenen.";

                        TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuRunAssessmentLevelCalculationsIndex,
                                                                      expectedItemText, expectedItemTooltip, RingtoetsCommonFormsResources.CalculateAllIcon);
                    }
                }
            }

            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        [RequiresSTA]
        public void GivenHydraulicBoundaryDatabaseWithNonExistingFilePath_WhenCalculatingAssessmentLevelFromContextMenu_ThenLogMessagesAddedPreviousOutputNotAffected()
        {
            // Given
            var guiMock = mockRepository.DynamicMock<IGui>();
            RoundedDouble designWaterLevel = (RoundedDouble) 4.2;

            var hydraulicBoundaryLocation1 = new HydraulicBoundaryLocation(100001, "", 1.1, 2.2);
            var hydraulicBoundaryLocation2 = new HydraulicBoundaryLocation(100002, "", 3.3, 4.4)
            {
                DesignWaterLevelOutput = new TestHydraulicBoundaryLocationOutput(designWaterLevel)
            };

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    hydraulicBoundaryLocation1,
                    hydraulicBoundaryLocation2
                },
                FilePath = "D:/nonExistingDirectory/nonExistingFile",
                Version = "random"
            };

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase = hydraulicBoundaryDatabase
            };
            var context = new DesignWaterLevelLocationsContext(assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(g => g.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                guiMock.Expect(g => g.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                guiMock.Expect(g => g.DocumentViewController).Return(mockRepository.Stub<IDocumentViewController>());

                mockRepository.ReplayAll();

                using (var plugin = new RingtoetsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = guiMock;
                    plugin.Activate();

                    using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(context, null, treeViewControl))
                    {
                        // When
                        Action action = () => contextMenuAdapter.Items[contextMenuRunAssessmentLevelCalculationsIndex].PerformClick();

                        // Then
                        string message = string.Format("Berekeningen konden niet worden gestart. Fout bij het lezen van bestand '{0}': het bestand bestaat niet.",
                                                       hydraulicBoundaryDatabase.FilePath);
                        TestHelper.AssertLogMessageWithLevelIsGenerated(action, new Tuple<string, LogLevelConstant>(message, LogLevelConstant.Error));

                        Assert.IsNaN(hydraulicBoundaryLocation1.DesignWaterLevel); // No result set

                        // Previous result not cleared
                        Assert.AreEqual(designWaterLevel, hydraulicBoundaryLocation2.DesignWaterLevel, hydraulicBoundaryLocation2.DesignWaterLevel.GetAccuracy());
                    }
                }
            }
            mockRepository.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        public void GivenHydraulicBoundaryLocationThatSucceeds_CalculatingAssessmentLevelFromContextMenu_ThenLogMessagesAddedOutputSet()
        {
            // Given
            var guiMock = mockRepository.DynamicMock<IGui>();

            var location = new HydraulicBoundaryLocation(1, "locationName", 1.1, 2.2);
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
                {
                    Locations =
                    {
                        location
                    },
                    FilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite")
                }
            };

            var context = new DesignWaterLevelLocationsContext(assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(g => g.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                guiMock.Expect(g => g.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                guiMock.Expect(g => g.DocumentViewController).Return(mockRepository.Stub<IDocumentViewController>());
                mockRepository.ReplayAll();

                DialogBoxHandler = (name, wnd) =>
                {
                    // Expect an activity dialog which is automatically closed
                };

                using (var plugin = new RingtoetsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = guiMock;
                    plugin.Activate();

                    using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(context, null, treeViewControl))
                    using (new HydraRingCalculatorFactoryConfig())
                    {
                        // When
                        Action call = () => contextMenuAdapter.Items[contextMenuRunAssessmentLevelCalculationsIndex].PerformClick();

                        // Then
                        TestHelper.AssertLogMessages(call, messages =>
                        {
                            var msgs = messages.ToArray();
                            Assert.AreEqual(7, msgs.Length);
                            StringAssert.StartsWith(string.Format("Validatie van 'Toetspeil berekenen voor locatie '{0}'' gestart om:", location.Name), msgs[0]);
                            StringAssert.StartsWith(string.Format("Validatie van 'Toetspeil berekenen voor locatie '{0}'' beëindigd om:", location.Name), msgs[1]);
                            StringAssert.StartsWith(string.Format("Berekening van 'Toetspeil berekenen voor locatie '{0}'' gestart om:", location.Name), msgs[2]);
                            StringAssert.StartsWith(string.Format("Toetspeil berekening voor locatie {0} is niet geconvergeerd.", location.Name), msgs[3]);
                            StringAssert.StartsWith("Toetspeil berekening is uitgevoerd op de tijdelijke locatie", msgs[4]);
                            StringAssert.StartsWith(string.Format("Berekening van 'Toetspeil berekenen voor locatie '{0}'' beëindigd om:", location.Name), msgs[5]);
                            StringAssert.StartsWith(string.Format("Uitvoeren van 'Toetspeil berekenen voor locatie '{0}'' is gelukt.", location.Name), msgs[6]);
                        });
                        Assert.AreEqual(0, location.DesignWaterLevel, location.DesignWaterLevel.GetAccuracy());
                        Assert.AreEqual(CalculationConvergence.CalculatedNotConverged, location.DesignWaterLevelCalculationConvergence);
                    }
                }
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void ForeColor_ContextHasNoHydraulicBoundaryDatabase_ReturnDisabledColor()
        {
            // Setup
            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var context = new DesignWaterLevelLocationsContext(assessmentSectionMock);

            using (var plugin = new RingtoetsPlugin())
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
        public void ForeColor_ContextHasHydraulicBoundaryDatabase_ReturnControlColor()
        {
            // Setup
            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();
            assessmentSectionMock.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            mockRepository.ReplayAll();

            var context = new DesignWaterLevelLocationsContext(assessmentSectionMock);

            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Color color = info.ForeColor(context);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), color);
            }
            mockRepository.VerifyAll();
        }

        private static TreeNodeInfo GetInfo(RingtoetsPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(DesignWaterLevelLocationsContext));
        }
    }
}