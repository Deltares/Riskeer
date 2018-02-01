// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Gui.Commands;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class HydraulicBoundaryDatabaseContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuImportHydraulicBoundaryDatabaseIndex = 0;

        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Forms, "HydraulicBoundaryDatabase");
        private readonly string testDataPathNoHlcd = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Forms, "HydraulicBoundaryDatabaseNoHLCD");

        private MockRepository mocks;

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

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
            const string name = "Hydraulische randvoorwaarden";
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            var context = new HydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase,
                                                               assessmentSection);

            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                string text = info.Text(context);

                // Assert
                Assert.AreEqual(name, text);
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
                Image image = info.Image(null);

                // Assert
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GenericInputOutputIcon, image);
            }
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var nodeData = new HydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase,
                                                                assessmentSection);

            var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();
            using (mocks.Ordered())
            {
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddExportItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.Build()).Return(null);
            }

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                mocks.ReplayAll();

                using (var plugin = new RingtoetsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    plugin.Gui = gui;

                    // Call
                    info.ContextMenuStrip(nodeData, null, treeViewControl);
                }
            }

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void ForeColor_HydraulicBoundaryDatabaseNotLinked_ReturnDisabledColor()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var hydraulicBoundaryDatabaseContext = new HydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase,
                                                                                        assessmentSection);

            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Color color = info.ForeColor(hydraulicBoundaryDatabaseContext);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), color);
            }
        }

        [Test]
        public void ForeColor_HydraulicBoundaryDatabaseLinked_ReturnEnabledColor()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = "databaseFile"
                }
            };
            var hydraulicBoundaryDatabaseContext = new HydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase,
                                                                                        assessmentSection);

            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Color color = info.ForeColor(hydraulicBoundaryDatabaseContext);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), color);
            }
        }

        [Test]
        public void ChildNodeObjects_HydraulicBoundaryDatabaseNotLinked_ReturnsEmpty()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var hydraulicBoundaryDatabaseContext = new HydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase,
                                                                                        assessmentSection);

            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                object[] objects = info.ChildNodeObjects(hydraulicBoundaryDatabaseContext).ToArray();

                // Assert
                Assert.AreEqual(0, objects.Length);
            }
        }

        [Test]
        public void ChildNodeObjects_HydraulicBoundaryDatabaseLinked_ReturnsChildrenOfData()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = "databaseFile"
                }
            };
            var hydraulicBoundaryDatabaseContext = new HydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase,
                                                                                        assessmentSection);

            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                object[] objects = info.ChildNodeObjects(hydraulicBoundaryDatabaseContext).ToArray();

                // Assert
                Assert.AreEqual(2, objects.Length);
                var designWaterLevelLocationsGroupContext = (DesignWaterLevelLocationsGroupContext) objects[0];
                Assert.AreSame(assessmentSection.HydraulicBoundaryDatabase.Locations, designWaterLevelLocationsGroupContext.WrappedData);

                var waveHeightLocationsGroupContext = (WaveHeightLocationsGroupContext) objects[1];
                Assert.AreSame(assessmentSection.HydraulicBoundaryDatabase.Locations, waveHeightLocationsGroupContext.WrappedData);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenNoDatabaseLinked_WhenOpeningValidFileFromContextMenu_ThenDatabaseLinkedObserversNotifiedAndLogMessagesAdded()
        {
            // Given
            var hydraulicBoundaryDatabaseObserver = mocks.StrictMock<IObserver>();
            hydraulicBoundaryDatabaseObserver.Expect(o => o.UpdateObserver());

            var hydraulicBoundaryLocationsObserver = mocks.StrictMock<IObserver>();
            hydraulicBoundaryLocationsObserver.Expect(o => o.UpdateObserver());

            var grassCoverErosionOutwardsLocationsObserver = mocks.StrictMock<IObserver>();
            grassCoverErosionOutwardsLocationsObserver.Expect(o => o.UpdateObserver());

            string testFile = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var hydraulicBoundaryDatabaseContext = new HydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase,
                                                                                        assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            using (var plugin = new RingtoetsPlugin())
            {
                var viewCommands = mocks.Stub<IViewCommands>();
                var mainWindow = mocks.Stub<IMainWindow>();

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(g => g.ViewCommands).Return(viewCommands);
                gui.Stub(cmp => cmp.Get(hydraulicBoundaryDatabaseContext, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mocks.ReplayAll();

                assessmentSection.HydraulicBoundaryDatabase.Attach(hydraulicBoundaryDatabaseObserver);
                assessmentSection.HydraulicBoundaryDatabase.Locations.Attach(hydraulicBoundaryLocationsObserver);
                assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations.Attach(grassCoverErosionOutwardsLocationsObserver);

                DialogBoxHandler = (name, wnd) =>
                {
                    var tester = new OpenFileDialogTester(wnd);
                    tester.OpenFile(testFile);
                };

                TreeNodeInfo info = GetInfo(plugin);
                plugin.Gui = gui;

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(hydraulicBoundaryDatabaseContext, null, treeViewControl))
                {
                    // When
                    Action action = () => contextMenuStrip.Items[contextMenuImportHydraulicBoundaryDatabaseIndex].PerformClick();

                    // Then
                    TestHelper.AssertLogMessages(action, messages =>
                    {
                        string[] msgs = messages.ToArray();
                        Assert.AreEqual(2, msgs.Length);
                        Assert.AreEqual("De hydraulische randvoorwaardenlocaties zijn ingelezen.", msgs[0]);
                        Assert.AreEqual($"Database op pad '{testFile}' gekoppeld.", msgs[1]);
                    });

                    Assert.IsTrue(assessmentSection.HydraulicBoundaryDatabase.IsLinked());
                    Assert.AreEqual(testFile, assessmentSection.HydraulicBoundaryDatabase.FilePath);
                    Assert.AreEqual("Dutch coast South19-11-2015 12:0013", assessmentSection.HydraulicBoundaryDatabase.Version);
                    CollectionAssert.IsNotEmpty(assessmentSection.HydraulicBoundaryDatabase.Locations);
                    CollectionAssert.IsNotEmpty(assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations);
                }
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenNoDatabaseLinked_WhenOpeningInvalidFileFromContextMenu_ThenNoDatabaseLinkedNoObserversNotifiedAndLogMessagesAdded()
        {
            // Given
            var assessmentSectionObserver = mocks.StrictMock<IObserver>();
            var grassCoverErosionOutwardsLocationsObserver = mocks.StrictMock<IObserver>();

            string testFile = Path.Combine(testDataPath, "empty.sqlite");

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var hydraulicBoundaryDatabaseContext = new HydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase,
                                                                                        assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            using (var plugin = new RingtoetsPlugin())
            {
                var mainWindow = mocks.Stub<IMainWindow>();

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(hydraulicBoundaryDatabaseContext, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mocks.ReplayAll();

                assessmentSection.Attach(assessmentSectionObserver);
                assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations.Attach(grassCoverErosionOutwardsLocationsObserver);

                DialogBoxHandler = (name, wnd) =>
                {
                    var tester = new OpenFileDialogTester(wnd);
                    tester.OpenFile(testFile);
                };

                TreeNodeInfo info = GetInfo(plugin);
                plugin.Gui = gui;

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(hydraulicBoundaryDatabaseContext, null, treeViewControl))
                {
                    // When
                    Action action = () => contextMenuStrip.Items[contextMenuImportHydraulicBoundaryDatabaseIndex].PerformClick();

                    // Then
                    string expectedMessage = $"Fout bij het lezen van bestand '{testFile}': kon geen locaties verkrijgen van de database.";
                    TestHelper.AssertLogMessageIsGenerated(action, expectedMessage);

                    Assert.IsFalse(assessmentSection.HydraulicBoundaryDatabase.IsLinked());
                }
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenNoDatabaseLinked_WhenOpeningValidFileWithoutHLCDFromContextMenu_ThenNoDatabaseLinkedNoObserversNotifiedAndLogMessagesAdded()
        {
            // Given
            var assessmentSectionObserver = mocks.StrictMock<IObserver>();
            var grassCoverErosionOutwardsLocationsObserver = mocks.StrictMock<IObserver>();

            string testFile = Path.Combine(testDataPathNoHlcd, "HRD dutch coast south.sqlite");

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var hydraulicBoundaryDatabaseContext = new HydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase,
                                                                                        assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            using (var plugin = new RingtoetsPlugin())
            {
                var mainWindow = mocks.Stub<IMainWindow>();

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(hydraulicBoundaryDatabaseContext, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mocks.ReplayAll();

                assessmentSection.Attach(assessmentSectionObserver);
                assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations.Attach(grassCoverErosionOutwardsLocationsObserver);

                DialogBoxHandler = (name, wnd) =>
                {
                    var tester = new OpenFileDialogTester(wnd);
                    tester.OpenFile(testFile);
                };

                TreeNodeInfo info = GetInfo(plugin);
                plugin.Gui = gui;

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(hydraulicBoundaryDatabaseContext, null, treeViewControl))
                {
                    // When
                    Action action = () => contextMenuStrip.Items[contextMenuImportHydraulicBoundaryDatabaseIndex].PerformClick();

                    // Then
                    string expectedMessage =
                        $"Fout bij het lezen van bestand '{testFile}': het bijbehorende HLCD bestand is niet gevonden in dezelfde map als het HRD bestand.";
                    TestHelper.AssertLogMessageIsGenerated(action, expectedMessage);

                    Assert.IsFalse(assessmentSection.HydraulicBoundaryDatabase.IsLinked());
                }
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenDatabaseLinked_WhenOpeningSameFileFromContextMenu_ThenCalculationsWillNotBeClearedAndNoObserversNotified()
        {
            // Given
            string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
            var assessmentObserver = mocks.StrictMock<IObserver>();
            var grassCoverErosionOutwardsLocationsObserver = mocks.StrictMock<IObserver>();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            using (var importer = new HydraulicBoundaryDatabaseImporter())
            {
                importer.Import(assessmentSection, validFilePath);
            }

            assessmentSection.Attach(assessmentObserver);

            var hydraulicBoundaryDatabaseContext = new HydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase,
                                                                                        assessmentSection);

            PipingOutput pipingOutput = PipingOutputTestFactory.Create();
            var pipingCalculation = new PipingCalculation(new GeneralPipingInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First()
                },
                Output = pipingOutput
            };

            assessmentSection.Piping.CalculationsGroup.Children.Add(pipingCalculation);
            assessmentSection.GrassCoverErosionOutwards.SetGrassCoverErosionOutwardsHydraulicBoundaryLocations(assessmentSection.HydraulicBoundaryDatabase.Locations);

            // Precondition
            Assert.IsTrue(assessmentSection.HydraulicBoundaryDatabase.IsLinked());
            CollectionAssert.IsNotEmpty(assessmentSection.HydraulicBoundaryDatabase.Locations);

            string currentFilePath = assessmentSection.HydraulicBoundaryDatabase.FilePath;
            string currentVersion = assessmentSection.HydraulicBoundaryDatabase.Version;
            List<HydraulicBoundaryLocation> currentLocations = assessmentSection.HydraulicBoundaryDatabase.Locations;
            HydraulicBoundaryLocation currentFirstGrassCoverErosionOutwardsLocation = assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations.First();
            assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations.Attach(grassCoverErosionOutwardsLocationsObserver);

            using (var treeViewControl = new TreeViewControl())
            using (var plugin = new RingtoetsPlugin())
            {
                var mainWindow = mocks.Stub<IMainWindow>();

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(hydraulicBoundaryDatabaseContext, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mocks.ReplayAll();

                DialogBoxHandler = (name, wnd) =>
                {
                    var tester = new OpenFileDialogTester(wnd);
                    tester.OpenFile(validFilePath);
                };

                TreeNodeInfo info = GetInfo(plugin);
                plugin.Gui = gui;

                using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(hydraulicBoundaryDatabaseContext, null, treeViewControl))
                {
                    // When
                    Action action = () => contextMenuAdapter.Items[contextMenuImportHydraulicBoundaryDatabaseIndex].PerformClick();

                    // Then
                    string expectedMessage = $"Database op pad '{validFilePath}' gekoppeld.";
                    TestHelper.AssertLogMessageIsGenerated(action, expectedMessage);

                    Assert.IsTrue(assessmentSection.HydraulicBoundaryDatabase.IsLinked());
                    Assert.AreEqual(currentFilePath, assessmentSection.HydraulicBoundaryDatabase.FilePath);
                    Assert.AreEqual(currentVersion, assessmentSection.HydraulicBoundaryDatabase.Version);
                    CollectionAssert.AreEqual(currentLocations, assessmentSection.HydraulicBoundaryDatabase.Locations);
                    Assert.AreSame(assessmentSection.HydraulicBoundaryDatabase.Locations.First(), pipingCalculation.InputParameters.HydraulicBoundaryLocation);
                    Assert.AreSame(pipingOutput, pipingCalculation.Output);
                    Assert.AreSame(currentFirstGrassCoverErosionOutwardsLocation, assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations.First());
                }
            }

            mocks.VerifyAll();
        }

        public override void Setup()
        {
            mocks = new MockRepository();
        }

        private static TreeNodeInfo GetInfo(RingtoetsPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(HydraulicBoundaryDatabaseContext));
        }
    }
}