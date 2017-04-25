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
using Ringtoets.Common.IO.HydraRing;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class HydraulicBoundaryDatabaseContextTreeNodeInfoTest : NUnitFormTest
    {
        private MockRepository mocks;

        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Forms, "HydraulicBoundaryDatabase");
        private readonly string testDataPathNoHlcd = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Forms, "HydraulicBoundaryDatabaseNoHLCD");
        private readonly string testDataPathNoSettings = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Forms, "HydraulicBoundaryDatabaseNoSettings");
        private readonly string testDataPathInvalidSettings = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Forms, "HydraulicBoundaryDatabaseSettingsInvalid");

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
        }

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

            var context = new HydraulicBoundaryDatabaseContext(assessmentSection);

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
            var nodeData = new HydraulicBoundaryDatabaseContext(assessmentSection);

            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();
            using (mocks.Ordered())
            {
                menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.Build()).Return(null);
            }

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilderMock);
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
        public void ForeColor_ContextHasNoReferenceLine_ReturnDisabledColor()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var hydraulicBoundaryDatabaseContext = new HydraulicBoundaryDatabaseContext(assessmentSection);

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
        public void ForeColor_ContextHasReferenceLineData_ReturnControlText()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
            };

            var hydraulicBoundaryDatabaseContext = new HydraulicBoundaryDatabaseContext(assessmentSection);

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
        public void ChildNodeObjects_NoHydraulicBoundaryDatabaseSet_ReturnsEmpty()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var hydraulicBoundaryDatabaseContext = new HydraulicBoundaryDatabaseContext(assessmentSection);

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
        public void ChildNodeObjects_HydraulicBoundaryDatabaseSet_ReturnsChildrenOfData()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
            };

            var hydraulicBoundaryDatabaseContext = new HydraulicBoundaryDatabaseContext(assessmentSection);

            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                object[] objects = info.ChildNodeObjects(hydraulicBoundaryDatabaseContext).ToArray();

                // Assert
                Assert.AreEqual(2, objects.Length);
                var designWaterLevelLocationsContext = (DesignWaterLevelLocationsContext) objects[0];
                Assert.AreSame(assessmentSection, designWaterLevelLocationsContext.WrappedData);

                var waveHeightLocationsContext = (WaveHeightLocationsContext) objects[1];
                Assert.AreSame(assessmentSection, waveHeightLocationsContext.WrappedData);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenNoFilePathIsSet_WhenOpeningValidFileFromContextMenu_ThenPathWillBeSetAndNotifiesObserverAndLogMessageAdded()
        {
            // Given
            var assessmentSectionObserver = mocks.StrictMock<IObserver>();
            assessmentSectionObserver.Expect(o => o.UpdateObserver());

            var grassCoverErosionOutwardsLocationsObserver = mocks.StrictMock<IObserver>();
            grassCoverErosionOutwardsLocationsObserver.Expect(o => o.UpdateObserver());

            string testFile = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            const int contextMenuImportHydraulicBoundaryDatabaseIndex = 0;

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var hydraulicBoundaryDatabaseContext = new HydraulicBoundaryDatabaseContext(assessmentSection);

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
                    TestHelper.AssertLogMessages(action, messages =>
                    {
                        string[] msgs = messages.ToArray();
                        Assert.AreEqual(2, msgs.Length);
                        Assert.AreEqual("De hydraulische randvoorwaardenlocaties zijn ingelezen.", msgs[0]);
                        Assert.AreEqual($"Database op pad '{testFile}' gekoppeld.", msgs[1]);
                    });

                    Assert.IsNotNull(assessmentSection.HydraulicBoundaryDatabase);
                    Assert.IsNotEmpty(assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations);
                }
            }
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenNoFilePathIsSet_WhenOpeneningInvalidFileFromContextMenu_ThenPathWillNotBeSetAndLogMessageAdded()
        {
            // Given
            string testFile = Path.Combine(testDataPath, "empty.sqlite");

            const int contextMenuImportHydraulicBoundaryDatabaseIndex = 0;

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var hydraulicBoundaryDatabaseContext = new HydraulicBoundaryDatabaseContext(assessmentSection);

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
                    tester.OpenFile(testFile);
                };

                TreeNodeInfo info = GetInfo(plugin);
                plugin.Gui = gui;

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(hydraulicBoundaryDatabaseContext, null, treeViewControl))
                {
                    // When
                    Action action = () => contextMenuStrip.Items[contextMenuImportHydraulicBoundaryDatabaseIndex].PerformClick();

                    // Then
                    string expectedMessage = $"Fout bij het lezen van bestand '{testFile}': kon geen locaties verkrijgen van de database. Het bestand wordt overgeslagen.";
                    TestHelper.AssertLogMessageIsGenerated(action, expectedMessage);

                    Assert.IsNull(assessmentSection.HydraulicBoundaryDatabase);
                }
            }
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenNoFilePathIsSet_WhenOpeneningValidFileWithoutHLCDFromContextMenu_ThenPathWillNotBeSetAndLogMessageAdded()
        {
            // Given
            string testFile = Path.Combine(testDataPathNoHlcd, "HRD dutch coast south.sqlite");

            const int contextMenuImportHydraulicBoundaryDatabaseIndex = 0;

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var hydraulicBoundaryDatabaseContext = new HydraulicBoundaryDatabaseContext(assessmentSection);

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

                    Assert.IsNull(assessmentSection.HydraulicBoundaryDatabase);
                }
            }
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenNoFilePathIsSet_WhenOpeneningValidFileWithoutSettingsDatabaseFromContextMenu_ThenPathWillNotBeSetAndLogMessageAdded()
        {
            // Given
            string testFile = Path.Combine(testDataPathNoSettings, "HRD dutch coast south.sqlite");

            const int contextMenuImportHydraulicBoundaryDatabaseIndex = 0;

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var hydraulicBoundaryDatabaseContext = new HydraulicBoundaryDatabaseContext(assessmentSection);

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
                        $"Fout bij het lezen van bestand '{testFile}': kon het rekeninstellingen bestand niet openen. Fout bij het lezen van bestand '{HydraulicDatabaseHelper.GetHydraulicBoundarySettingsDatabase(testFile)}': het bestand bestaat niet.";
                    TestHelper.AssertLogMessageIsGenerated(action, expectedMessage);

                    Assert.IsNull(assessmentSection.HydraulicBoundaryDatabase);
                }
            }
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenNoFilePathIsSet_WhenOpeneningValidFileWithInvalidSettingsDatabaseFromContextMenu_ThenPathWillNotBeSetAndLogMessageAdded()
        {
            // Given
            string testFile = Path.Combine(testDataPathInvalidSettings, "HRD dutch coast south.sqlite");

            const int contextMenuImportHydraulicBoundaryDatabaseIndex = 0;

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var hydraulicBoundaryDatabaseContext = new HydraulicBoundaryDatabaseContext(assessmentSection);

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
                        $"Fout bij het lezen van bestand '{testFile}': kon het rekeninstellingen bestand niet openen. De rekeninstellingen database heeft niet het juiste schema.";
                    TestHelper.AssertLogMessageIsGenerated(action, expectedMessage);

                    Assert.IsNull(assessmentSection.HydraulicBoundaryDatabase);
                }
            }
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenFilePathIsSet_WhenOpeningSameFileFromContextMenu_ThenCalculationsWillNotBeClearedAndNoNotifyObservers()
        {
            // Given
            string validFile = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
            var assessmentObserver = mocks.StrictMock<IObserver>();
            var grassCoverErosionOutwardsLocationsObserver = mocks.StrictMock<IObserver>();
            const int contextMenuImportHydraulicBoundaryDatabaseIndex = 0;

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            using (var importer = new HydraulicBoundaryDatabaseImporter())
            {
                importer.Import(assessmentSection, validFile);
            }

            assessmentSection.Attach(assessmentObserver);

            var hydraulicBoundaryDatabaseContext = new HydraulicBoundaryDatabaseContext(assessmentSection);

            var pipingOutput = new TestPipingOutput();
            var pipingSemiProbabilisticOutput = new TestPipingSemiProbabilisticOutput();
            var pipingCalculation = new PipingCalculation(new GeneralPipingInput())
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First()
                },
                Output = pipingOutput,
                SemiProbabilisticOutput = pipingSemiProbabilisticOutput
            };

            assessmentSection.PipingFailureMechanism.CalculationsGroup.Children.Add(pipingCalculation);
            assessmentSection.GrassCoverErosionOutwards.SetGrassCoverErosionOutwardsHydraulicBoundaryLocations(assessmentSection.HydraulicBoundaryDatabase.Locations);

            // Precondition
            Assert.IsNotNull(assessmentSection.HydraulicBoundaryDatabase);
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
                    tester.OpenFile(validFile);
                };

                TreeNodeInfo info = GetInfo(plugin);
                plugin.Gui = gui;

                using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(hydraulicBoundaryDatabaseContext, null, treeViewControl))
                {
                    // When
                    Action action = () => contextMenuAdapter.Items[contextMenuImportHydraulicBoundaryDatabaseIndex].PerformClick();

                    // Then
                    string expectedMessage = $"Database op pad '{validFile}' gekoppeld.";
                    TestHelper.AssertLogMessageIsGenerated(action, expectedMessage);

                    Assert.IsNotNull(assessmentSection.HydraulicBoundaryDatabase);
                    Assert.AreEqual(currentFilePath, assessmentSection.HydraulicBoundaryDatabase.FilePath);
                    Assert.AreEqual(currentVersion, assessmentSection.HydraulicBoundaryDatabase.Version);
                    CollectionAssert.AreEqual(currentLocations, assessmentSection.HydraulicBoundaryDatabase.Locations);
                    Assert.AreSame(assessmentSection.HydraulicBoundaryDatabase.Locations.First(), pipingCalculation.InputParameters.HydraulicBoundaryLocation);
                    Assert.AreSame(pipingOutput, pipingCalculation.Output);
                    Assert.AreSame(pipingSemiProbabilisticOutput, pipingCalculation.SemiProbabilisticOutput);
                    Assert.AreSame(currentFirstGrassCoverErosionOutwardsLocation, assessmentSection.GrassCoverErosionOutwards.HydraulicBoundaryLocations.First());
                }
            }
            mocks.VerifyAll();
        }

        private static TreeNodeInfo GetInfo(RingtoetsPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(HydraulicBoundaryDatabaseContext));
        }
    }
}