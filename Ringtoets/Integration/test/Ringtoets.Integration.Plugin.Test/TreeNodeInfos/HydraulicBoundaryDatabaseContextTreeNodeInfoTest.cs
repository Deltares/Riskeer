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
using Core.Common.Gui.Forms.ViewHost;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Common.Plugin.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HydraRing.Calculation.Calculator;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
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
        private const int contextMenuCalculateAllIndex = 3;

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
        public void Image_Always_ReturnsGeneralFolderIcon()
        {
            // Setup
            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Image image = info.Image(null);

                // Assert
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GeneralFolderIcon, image);
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
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
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

                var designWaterLevelCalculationsGroupContext = (DesignWaterLevelCalculationsGroupContext) objects[0];
                Assert.AreSame(assessmentSection.HydraulicBoundaryDatabase.Locations, designWaterLevelCalculationsGroupContext.WrappedData);
                Assert.AreSame(assessmentSection, designWaterLevelCalculationsGroupContext.AssessmentSection);

                var waveHeightCalculationsGroupContext = (WaveHeightCalculationsGroupContext) objects[1];
                Assert.AreSame(assessmentSection.HydraulicBoundaryDatabase.Locations, waveHeightCalculationsGroupContext.WrappedData);
                Assert.AreSame(assessmentSection, waveHeightCalculationsGroupContext.AssessmentSection);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenNoDatabaseLinked_WhenOpeningValidFileFromContextMenu_ThenDatabaseLinkedObserversNotifiedAndLogMessagesAdded()
        {
            // Given
            string testFile = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            AttachHydraulicBoundaryDatabaseObserver(assessmentSection, true);
            AttachLocationAndCalculationObservers(assessmentSection, true);

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

                    ObservableList<HydraulicBoundaryLocation> hydraulicBoundaryLocations = assessmentSection.HydraulicBoundaryDatabase.Locations;
                    GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwardsFailureMechanism = assessmentSection.GrassCoverErosionOutwards;

                    CollectionAssert.IsNotEmpty(hydraulicBoundaryLocations);
                    CollectionAssert.AreEqual(hydraulicBoundaryLocations, assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
                    CollectionAssert.AreEqual(hydraulicBoundaryLocations, assessmentSection.WaterLevelCalculationsForSignalingNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
                    CollectionAssert.AreEqual(hydraulicBoundaryLocations, assessmentSection.WaterLevelCalculationsForLowerLimitNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
                    CollectionAssert.AreEqual(hydraulicBoundaryLocations, assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
                    CollectionAssert.AreEqual(hydraulicBoundaryLocations, assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
                    CollectionAssert.AreEqual(hydraulicBoundaryLocations, assessmentSection.WaveHeightCalculationsForSignalingNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
                    CollectionAssert.AreEqual(hydraulicBoundaryLocations, assessmentSection.WaveHeightCalculationsForLowerLimitNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
                    CollectionAssert.AreEqual(hydraulicBoundaryLocations, assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
                    CollectionAssert.AreEqual(hydraulicBoundaryLocations, grassCoverErosionOutwardsFailureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
                    CollectionAssert.AreEqual(hydraulicBoundaryLocations, grassCoverErosionOutwardsFailureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
                    CollectionAssert.AreEqual(hydraulicBoundaryLocations, grassCoverErosionOutwardsFailureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
                    CollectionAssert.AreEqual(hydraulicBoundaryLocations, grassCoverErosionOutwardsFailureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
                    CollectionAssert.AreEqual(hydraulicBoundaryLocations, grassCoverErosionOutwardsFailureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
                    CollectionAssert.AreEqual(hydraulicBoundaryLocations, grassCoverErosionOutwardsFailureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm.Select(hblc => hblc.HydraulicBoundaryLocation));
                }
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenNoDatabaseLinked_WhenOpeningInvalidFileFromContextMenu_ThenNoDatabaseLinkedNoObserversNotifiedAndLogMessagesAdded()
        {
            // Given
            string testFile = Path.Combine(testDataPath, "empty.sqlite");

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            AttachHydraulicBoundaryDatabaseObserver(assessmentSection);
            AttachLocationAndCalculationObservers(assessmentSection);

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
            string testFile = Path.Combine(testDataPathNoHlcd, "HRD dutch coast south.sqlite");

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            AttachHydraulicBoundaryDatabaseObserver(assessmentSection);
            AttachLocationAndCalculationObservers(assessmentSection);

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

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            using (var importer = new HydraulicBoundaryDatabaseImporter())
            {
                importer.Import(assessmentSection, validFilePath);
            }

            AttachHydraulicBoundaryDatabaseObserver(assessmentSection);
            AttachLocationAndCalculationObservers(assessmentSection);

            ObservableList<HydraulicBoundaryLocation> hydraulicBoundaryLocations = assessmentSection.HydraulicBoundaryDatabase.Locations;
            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwardsFailureMechanism = assessmentSection.GrassCoverErosionOutwards;

            assessmentSection.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryLocations);
            grassCoverErosionOutwardsFailureMechanism.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryLocations);

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

            var hydraulicBoundaryDatabaseContext = new HydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase,
                                                                                        assessmentSection);

            // Precondition
            Assert.IsTrue(assessmentSection.HydraulicBoundaryDatabase.IsLinked());
            CollectionAssert.IsNotEmpty(assessmentSection.HydraulicBoundaryDatabase.Locations);

            string currentFilePath = assessmentSection.HydraulicBoundaryDatabase.FilePath;
            string currentVersion = assessmentSection.HydraulicBoundaryDatabase.Version;
            IEnumerable<HydraulicBoundaryLocation> currentHydraulicBoundaryLocations = assessmentSection.HydraulicBoundaryDatabase.Locations.ToArray();
            IEnumerable<HydraulicBoundaryLocationCalculation> currentWaterLevelCalculationsForFactorizedSignalingNorm = assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.ToArray();
            IEnumerable<HydraulicBoundaryLocationCalculation> currentWaterLevelCalculationsForSignalingNorm = assessmentSection.WaterLevelCalculationsForSignalingNorm.ToArray();
            IEnumerable<HydraulicBoundaryLocationCalculation> currentWaterLevelCalculationsForLowerLimitNorm = assessmentSection.WaterLevelCalculationsForLowerLimitNorm.ToArray();
            IEnumerable<HydraulicBoundaryLocationCalculation> currentWaterLevelCalculationsForFactorizedLowerLimitNorm = assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm.ToArray();
            IEnumerable<HydraulicBoundaryLocationCalculation> currentWaveHeightCalculationsForFactorizedSignalingNorm = assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm.ToArray();
            IEnumerable<HydraulicBoundaryLocationCalculation> currentWaveHeightCalculationsForSignalingNorm = assessmentSection.WaveHeightCalculationsForSignalingNorm.ToArray();
            IEnumerable<HydraulicBoundaryLocationCalculation> currentWaveHeightCalculationsForLowerLimitNorm = assessmentSection.WaveHeightCalculationsForLowerLimitNorm.ToArray();
            IEnumerable<HydraulicBoundaryLocationCalculation> currentWaveHeightCalculationsForFactorizedLowerLimitNorm = assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm.ToArray();
            IEnumerable<HydraulicBoundaryLocationCalculation> currentWaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm = grassCoverErosionOutwardsFailureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm.ToArray();
            IEnumerable<HydraulicBoundaryLocationCalculation> currentWaterLevelCalculationsForMechanismSpecificSignalingNorm = grassCoverErosionOutwardsFailureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm.ToArray();
            IEnumerable<HydraulicBoundaryLocationCalculation> currentWaterLevelCalculationsForMechanismSpecificLowerLimitNorm = grassCoverErosionOutwardsFailureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm.ToArray();
            IEnumerable<HydraulicBoundaryLocationCalculation> currentWaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm = grassCoverErosionOutwardsFailureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm.ToArray();
            IEnumerable<HydraulicBoundaryLocationCalculation> currentWaveHeightCalculationsForMechanismSpecificSignalingNorm = grassCoverErosionOutwardsFailureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm.ToArray();
            IEnumerable<HydraulicBoundaryLocationCalculation> currentWaveHeightCalculationsForMechanismSpecificLowerLimitNorm = grassCoverErosionOutwardsFailureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm.ToArray();

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
                    CollectionAssert.AreEqual(currentHydraulicBoundaryLocations, assessmentSection.HydraulicBoundaryDatabase.Locations);
                    CollectionAssert.AreEqual(currentWaterLevelCalculationsForFactorizedSignalingNorm, assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm);
                    CollectionAssert.AreEqual(currentWaterLevelCalculationsForSignalingNorm, assessmentSection.WaterLevelCalculationsForSignalingNorm);
                    CollectionAssert.AreEqual(currentWaterLevelCalculationsForLowerLimitNorm, assessmentSection.WaterLevelCalculationsForLowerLimitNorm);
                    CollectionAssert.AreEqual(currentWaterLevelCalculationsForFactorizedLowerLimitNorm, assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm);
                    CollectionAssert.AreEqual(currentWaveHeightCalculationsForFactorizedSignalingNorm, assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm);
                    CollectionAssert.AreEqual(currentWaveHeightCalculationsForSignalingNorm, assessmentSection.WaveHeightCalculationsForSignalingNorm);
                    CollectionAssert.AreEqual(currentWaveHeightCalculationsForLowerLimitNorm, assessmentSection.WaveHeightCalculationsForLowerLimitNorm);
                    CollectionAssert.AreEqual(currentWaveHeightCalculationsForFactorizedLowerLimitNorm, assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm);
                    CollectionAssert.AreEqual(currentWaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm, grassCoverErosionOutwardsFailureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm);
                    CollectionAssert.AreEqual(currentWaterLevelCalculationsForMechanismSpecificSignalingNorm, grassCoverErosionOutwardsFailureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm);
                    CollectionAssert.AreEqual(currentWaterLevelCalculationsForMechanismSpecificLowerLimitNorm, grassCoverErosionOutwardsFailureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm);
                    CollectionAssert.AreEqual(currentWaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm, grassCoverErosionOutwardsFailureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm);
                    CollectionAssert.AreEqual(currentWaveHeightCalculationsForMechanismSpecificSignalingNorm, grassCoverErosionOutwardsFailureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm);
                    CollectionAssert.AreEqual(currentWaveHeightCalculationsForMechanismSpecificLowerLimitNorm, grassCoverErosionOutwardsFailureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm);
                    Assert.AreSame(assessmentSection.HydraulicBoundaryDatabase.Locations.First(), pipingCalculation.InputParameters.HydraulicBoundaryLocation);
                    Assert.AreSame(pipingOutput, pipingCalculation.Output);
                }
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenDatabaseLinked_WhenOpeningSameFileOnDifferentPathFromContextMenu_ThenFilePathUpdatedAndSpecificObserversNotified()
        {
            // Given
            string validFilePath1 = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
            string validFilePath2 = Path.Combine(testDataPath, "copy of HRD dutch coast south.sqlite");

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            using (var importer = new HydraulicBoundaryDatabaseImporter())
            {
                importer.Import(assessmentSection, validFilePath1);
            }

            AttachHydraulicBoundaryDatabaseObserver(assessmentSection, true);
            AttachLocationAndCalculationObservers(assessmentSection);

            ObservableList<HydraulicBoundaryLocation> hydraulicBoundaryLocations = assessmentSection.HydraulicBoundaryDatabase.Locations;
            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwardsFailureMechanism = assessmentSection.GrassCoverErosionOutwards;

            assessmentSection.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryLocations);
            grassCoverErosionOutwardsFailureMechanism.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryLocations);

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

            var hydraulicBoundaryDatabaseContext = new HydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase,
                                                                                        assessmentSection);

            // Precondition
            Assert.IsTrue(assessmentSection.HydraulicBoundaryDatabase.IsLinked());
            CollectionAssert.IsNotEmpty(assessmentSection.HydraulicBoundaryDatabase.Locations);

            string currentVersion = assessmentSection.HydraulicBoundaryDatabase.Version;
            IEnumerable<HydraulicBoundaryLocation> currentHydraulicBoundaryLocations = assessmentSection.HydraulicBoundaryDatabase.Locations.ToArray();
            IEnumerable<HydraulicBoundaryLocationCalculation> currentWaterLevelCalculationsForFactorizedSignalingNorm = assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.ToArray();
            IEnumerable<HydraulicBoundaryLocationCalculation> currentWaterLevelCalculationsForSignalingNorm = assessmentSection.WaterLevelCalculationsForSignalingNorm.ToArray();
            IEnumerable<HydraulicBoundaryLocationCalculation> currentWaterLevelCalculationsForLowerLimitNorm = assessmentSection.WaterLevelCalculationsForLowerLimitNorm.ToArray();
            IEnumerable<HydraulicBoundaryLocationCalculation> currentWaterLevelCalculationsForFactorizedLowerLimitNorm = assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm.ToArray();
            IEnumerable<HydraulicBoundaryLocationCalculation> currentWaveHeightCalculationsForFactorizedSignalingNorm = assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm.ToArray();
            IEnumerable<HydraulicBoundaryLocationCalculation> currentWaveHeightCalculationsForSignalingNorm = assessmentSection.WaveHeightCalculationsForSignalingNorm.ToArray();
            IEnumerable<HydraulicBoundaryLocationCalculation> currentWaveHeightCalculationsForLowerLimitNorm = assessmentSection.WaveHeightCalculationsForLowerLimitNorm.ToArray();
            IEnumerable<HydraulicBoundaryLocationCalculation> currentWaveHeightCalculationsForFactorizedLowerLimitNorm = assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm.ToArray();
            IEnumerable<HydraulicBoundaryLocationCalculation> currentWaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm = grassCoverErosionOutwardsFailureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm.ToArray();
            IEnumerable<HydraulicBoundaryLocationCalculation> currentWaterLevelCalculationsForMechanismSpecificSignalingNorm = grassCoverErosionOutwardsFailureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm.ToArray();
            IEnumerable<HydraulicBoundaryLocationCalculation> currentWaterLevelCalculationsForMechanismSpecificLowerLimitNorm = grassCoverErosionOutwardsFailureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm.ToArray();
            IEnumerable<HydraulicBoundaryLocationCalculation> currentWaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm = grassCoverErosionOutwardsFailureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm.ToArray();
            IEnumerable<HydraulicBoundaryLocationCalculation> currentWaveHeightCalculationsForMechanismSpecificSignalingNorm = grassCoverErosionOutwardsFailureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm.ToArray();
            IEnumerable<HydraulicBoundaryLocationCalculation> currentWaveHeightCalculationsForMechanismSpecificLowerLimitNorm = grassCoverErosionOutwardsFailureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm.ToArray();

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
                    tester.OpenFile(validFilePath2);
                };

                TreeNodeInfo info = GetInfo(plugin);
                plugin.Gui = gui;

                using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(hydraulicBoundaryDatabaseContext, null, treeViewControl))
                {
                    // When
                    Action action = () => contextMenuAdapter.Items[contextMenuImportHydraulicBoundaryDatabaseIndex].PerformClick();

                    // Then
                    string expectedMessage = $"Database op pad '{validFilePath2}' gekoppeld.";
                    TestHelper.AssertLogMessageIsGenerated(action, expectedMessage);

                    Assert.IsTrue(assessmentSection.HydraulicBoundaryDatabase.IsLinked());
                    Assert.AreEqual(validFilePath2, assessmentSection.HydraulicBoundaryDatabase.FilePath);
                    Assert.AreEqual(currentVersion, assessmentSection.HydraulicBoundaryDatabase.Version);
                    CollectionAssert.AreEqual(currentHydraulicBoundaryLocations, assessmentSection.HydraulicBoundaryDatabase.Locations);
                    CollectionAssert.AreEqual(currentWaterLevelCalculationsForFactorizedSignalingNorm, assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm);
                    CollectionAssert.AreEqual(currentWaterLevelCalculationsForSignalingNorm, assessmentSection.WaterLevelCalculationsForSignalingNorm);
                    CollectionAssert.AreEqual(currentWaterLevelCalculationsForLowerLimitNorm, assessmentSection.WaterLevelCalculationsForLowerLimitNorm);
                    CollectionAssert.AreEqual(currentWaterLevelCalculationsForFactorizedLowerLimitNorm, assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm);
                    CollectionAssert.AreEqual(currentWaveHeightCalculationsForFactorizedSignalingNorm, assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm);
                    CollectionAssert.AreEqual(currentWaveHeightCalculationsForSignalingNorm, assessmentSection.WaveHeightCalculationsForSignalingNorm);
                    CollectionAssert.AreEqual(currentWaveHeightCalculationsForLowerLimitNorm, assessmentSection.WaveHeightCalculationsForLowerLimitNorm);
                    CollectionAssert.AreEqual(currentWaveHeightCalculationsForFactorizedLowerLimitNorm, assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm);
                    CollectionAssert.AreEqual(currentWaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm, grassCoverErosionOutwardsFailureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm);
                    CollectionAssert.AreEqual(currentWaterLevelCalculationsForMechanismSpecificSignalingNorm, grassCoverErosionOutwardsFailureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm);
                    CollectionAssert.AreEqual(currentWaterLevelCalculationsForMechanismSpecificLowerLimitNorm, grassCoverErosionOutwardsFailureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm);
                    CollectionAssert.AreEqual(currentWaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm, grassCoverErosionOutwardsFailureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm);
                    CollectionAssert.AreEqual(currentWaveHeightCalculationsForMechanismSpecificSignalingNorm, grassCoverErosionOutwardsFailureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm);
                    CollectionAssert.AreEqual(currentWaveHeightCalculationsForMechanismSpecificLowerLimitNorm, grassCoverErosionOutwardsFailureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm);
                    Assert.AreSame(assessmentSection.HydraulicBoundaryDatabase.Locations.First(), pipingCalculation.InputParameters.HydraulicBoundaryLocation);
                    Assert.AreSame(pipingOutput, pipingCalculation.Output);
                }
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_HydraulicBoundaryDatabaseNotLinked_ContextMenuItemCalculateAllDisabledAndTooltipSet()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("locationName");
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            var nodeData = new HydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mocks.ReplayAll();

                using (var plugin = new RingtoetsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    plugin.Gui = gui;

                    // Call
                    using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                    {
                        // Assert
                        ToolStripItem contextMenuItem = contextMenu.Items[contextMenuCalculateAllIndex];

                        Assert.AreEqual("Alles be&rekenen", contextMenuItem.Text);
                        StringAssert.Contains("Er is geen hydraulische randvoorwaardendatabase geïmporteerd.", contextMenuItem.ToolTipText);
                        TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.CalculateAllIcon, contextMenuItem.Image);
                        Assert.IsFalse(contextMenuItem.Enabled);
                    }
                }
            }

            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void ContextMenuStrip_HydraulicBoundaryDatabaseLinkedToInvalidFile_ContextMenuItemCalculateAllDisabledAndTooltipSet()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = "invalidFilePath"
                }
            };

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("locationName");
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            var nodeData = new HydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mocks.ReplayAll();

                using (var plugin = new RingtoetsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    plugin.Gui = gui;

                    // Call
                    using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                    {
                        // Assert
                        ToolStripItem contextMenuItem = contextMenu.Items[contextMenuCalculateAllIndex];

                        Assert.AreEqual("Alles be&rekenen", contextMenuItem.Text);
                        StringAssert.Contains("Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt.", contextMenuItem.ToolTipText);
                        TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.CalculateAllIcon, contextMenuItem.Image);
                        Assert.IsFalse(contextMenuItem.Enabled);
                    }
                }
            }

            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenValidCalculations_WhenCalculatingAllFromContextMenu_ThenAllCalculationsScheduled()
        {
            // Given
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite")
                }
            };

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("locationName");
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            var context = new HydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            using (var plugin = new RingtoetsPlugin())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.DocumentViewController).Return(mocks.Stub<IDocumentViewController>());

                var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
                var designWaterLevelCalculator = new TestDesignWaterLevelCalculator
                {
                    Converged = false
                };
                var waveHeightCalculator = new TestWaveHeightCalculator
                {
                    Converged = false
                };

                calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(testDataPath, string.Empty)).Return(designWaterLevelCalculator).Repeat.Times(4);
                calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(testDataPath, string.Empty)).Return(waveHeightCalculator).Repeat.Times(4);
                mocks.ReplayAll();

                TreeNodeInfo info = GetInfo(plugin);
                plugin.Gui = gui;
                plugin.Activate();

                DialogBoxHandler = (name, wnd) =>
                {
                    // Expect an activity dialog which is automatically closed
                };

                using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(context, null, treeViewControl))
                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                {
                    // When
                    Action call = () => contextMenuAdapter.Items[contextMenuCalculateAllIndex].PerformClick();

                    // Then
                    TestHelper.AssertLogMessages(call, messages =>
                    {
                        string[] msgs = messages.ToArray();
                        Assert.AreEqual(64, msgs.Length);

                        const string designWaterLevelName = "Waterstand";
                        HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                            hydraulicBoundaryLocation.Name, designWaterLevelName, "A+->A", msgs, 0);
                        HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                            hydraulicBoundaryLocation.Name, designWaterLevelName, "A->B", msgs, 8);
                        HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                            hydraulicBoundaryLocation.Name, designWaterLevelName, "B->C", msgs, 16);
                        HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                            hydraulicBoundaryLocation.Name, designWaterLevelName, "C->D", msgs, 24);

                        const string waveHeightName = "Golfhoogte";
                        HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                            hydraulicBoundaryLocation.Name, waveHeightName, "A+->A", msgs, 32);
                        HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                            hydraulicBoundaryLocation.Name, waveHeightName, "A->B", msgs, 40);
                        HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                            hydraulicBoundaryLocation.Name, waveHeightName, "B->C", msgs, 48);
                        HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                            hydraulicBoundaryLocation.Name, waveHeightName, "C->D", msgs, 56);
                    });

                    AssertDesignWaterLevelCalculationOutput(designWaterLevelCalculator, assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.Single().Output);
                    AssertDesignWaterLevelCalculationOutput(designWaterLevelCalculator, assessmentSection.WaterLevelCalculationsForSignalingNorm.Single().Output);
                    AssertDesignWaterLevelCalculationOutput(designWaterLevelCalculator, assessmentSection.WaterLevelCalculationsForLowerLimitNorm.Single().Output);
                    AssertDesignWaterLevelCalculationOutput(designWaterLevelCalculator, assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm.Single().Output);

                    AssertWaveHeightCalculationOutput(waveHeightCalculator, assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm.Single().Output);
                    AssertWaveHeightCalculationOutput(waveHeightCalculator, assessmentSection.WaveHeightCalculationsForSignalingNorm.Single().Output);
                    AssertWaveHeightCalculationOutput(waveHeightCalculator, assessmentSection.WaveHeightCalculationsForLowerLimitNorm.Single().Output);
                    AssertWaveHeightCalculationOutput(waveHeightCalculator, assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm.Single().Output);
                }
            }

            mocks.VerifyAll();
        }

        public override void Setup()
        {
            mocks = new MockRepository();
        }

        private void AttachHydraulicBoundaryDatabaseObserver(AssessmentSection assessmentSection, bool expectUpdateObserver = false)
        {
            var hydraulicBoundaryDatabaseObserver = mocks.StrictMock<IObserver>();

            assessmentSection.HydraulicBoundaryDatabase.Attach(hydraulicBoundaryDatabaseObserver);

            if (expectUpdateObserver)
            {
                hydraulicBoundaryDatabaseObserver.Expect(o => o.UpdateObserver());
            }
        }

        private void AttachLocationAndCalculationObservers(AssessmentSection assessmentSection, bool expectUpdateObserver = false)
        {
            var hydraulicBoundaryLocationsObserver = mocks.StrictMock<IObserver>();

            var waterLevelCalculationsForFactorizedSignalingNormObserver = mocks.StrictMock<IObserver>();
            var waterLevelCalculationsForSignalingNormObserver = mocks.StrictMock<IObserver>();
            var waterLevelCalculationsForLowerLimitNormObserver = mocks.StrictMock<IObserver>();
            var waterLevelCalculationsForFactorizedLowerLimitNormObserver = mocks.StrictMock<IObserver>();
            var waveHeightCalculationsForFactorizedSignalingNormObserver = mocks.StrictMock<IObserver>();
            var waveHeightCalculationsForSignalingNormObserver = mocks.StrictMock<IObserver>();
            var waveHeightCalculationsForLowerLimitNormObserver = mocks.StrictMock<IObserver>();
            var waveHeightCalculationsForFactorizedLowerLimitNormObserver = mocks.StrictMock<IObserver>();

            var waterLevelCalculationsForMechanismSpecificFactorizedSignalingNormObserver = mocks.StrictMock<IObserver>();
            var waterLevelCalculationsForMechanismSpecificSignalingNormObserver = mocks.StrictMock<IObserver>();
            var waterLevelCalculationsForMechanismSpecificLowerLimitNormObserver = mocks.StrictMock<IObserver>();
            var waveHeightCalculationsForMechanismSpecificFactorizedSignalingNormObserver = mocks.StrictMock<IObserver>();
            var waveHeightCalculationsForMechanismSpecificSignalingNormObserver = mocks.StrictMock<IObserver>();
            var waveHeightCalculationsForMechanismSpecificLowerLimitNormObserver = mocks.StrictMock<IObserver>();

            var duneLocationsObserver = mocks.StrictMock<IObserver>();
            var duneLocationCalculationsForMechanismSpecificFactorizedSignalingNormObserver = mocks.StrictMock<IObserver>();
            var duneLocationCalculationsForMechanismSpecificSignalingNormObserver = mocks.StrictMock<IObserver>();
            var duneLocationCalculationsForMechanismSpecificLowerLimitNormObserver = mocks.StrictMock<IObserver>();
            var duneLocationCalculationsForLowerLimitNormObserver = mocks.StrictMock<IObserver>();
            var duneLocationCalculationsForFactorizedLowerLimitNormObserver = mocks.StrictMock<IObserver>();

            assessmentSection.HydraulicBoundaryDatabase.Locations.Attach(hydraulicBoundaryLocationsObserver);

            assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.Attach(waterLevelCalculationsForFactorizedSignalingNormObserver);
            assessmentSection.WaterLevelCalculationsForSignalingNorm.Attach(waterLevelCalculationsForSignalingNormObserver);
            assessmentSection.WaterLevelCalculationsForLowerLimitNorm.Attach(waterLevelCalculationsForLowerLimitNormObserver);
            assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm.Attach(waterLevelCalculationsForFactorizedLowerLimitNormObserver);
            assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm.Attach(waveHeightCalculationsForFactorizedSignalingNormObserver);
            assessmentSection.WaveHeightCalculationsForSignalingNorm.Attach(waveHeightCalculationsForSignalingNormObserver);
            assessmentSection.WaveHeightCalculationsForLowerLimitNorm.Attach(waveHeightCalculationsForLowerLimitNormObserver);
            assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm.Attach(waveHeightCalculationsForFactorizedLowerLimitNormObserver);

            assessmentSection.GrassCoverErosionOutwards.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm.Attach(waterLevelCalculationsForMechanismSpecificFactorizedSignalingNormObserver);
            assessmentSection.GrassCoverErosionOutwards.WaterLevelCalculationsForMechanismSpecificSignalingNorm.Attach(waterLevelCalculationsForMechanismSpecificSignalingNormObserver);
            assessmentSection.GrassCoverErosionOutwards.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm.Attach(waterLevelCalculationsForMechanismSpecificLowerLimitNormObserver);
            assessmentSection.GrassCoverErosionOutwards.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm.Attach(waveHeightCalculationsForMechanismSpecificFactorizedSignalingNormObserver);
            assessmentSection.GrassCoverErosionOutwards.WaveHeightCalculationsForMechanismSpecificSignalingNorm.Attach(waveHeightCalculationsForMechanismSpecificSignalingNormObserver);
            assessmentSection.GrassCoverErosionOutwards.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm.Attach(waveHeightCalculationsForMechanismSpecificLowerLimitNormObserver);

            assessmentSection.DuneErosion.DuneLocations.Attach(duneLocationsObserver);
            assessmentSection.DuneErosion.CalculationsForMechanismSpecificFactorizedSignalingNorm.Attach(duneLocationCalculationsForMechanismSpecificFactorizedSignalingNormObserver);
            assessmentSection.DuneErosion.CalculationsForMechanismSpecificSignalingNorm.Attach(duneLocationCalculationsForMechanismSpecificSignalingNormObserver);
            assessmentSection.DuneErosion.CalculationsForMechanismSpecificLowerLimitNorm.Attach(duneLocationCalculationsForMechanismSpecificLowerLimitNormObserver);
            assessmentSection.DuneErosion.CalculationsForLowerLimitNorm.Attach(duneLocationCalculationsForLowerLimitNormObserver);
            assessmentSection.DuneErosion.CalculationsForFactorizedLowerLimitNorm.Attach(duneLocationCalculationsForFactorizedLowerLimitNormObserver);

            if (expectUpdateObserver)
            {
                hydraulicBoundaryLocationsObserver.Expect(o => o.UpdateObserver());

                waterLevelCalculationsForFactorizedSignalingNormObserver.Expect(o => o.UpdateObserver());
                waterLevelCalculationsForSignalingNormObserver.Expect(o => o.UpdateObserver());
                waterLevelCalculationsForLowerLimitNormObserver.Expect(o => o.UpdateObserver());
                waterLevelCalculationsForFactorizedLowerLimitNormObserver.Expect(o => o.UpdateObserver());
                waveHeightCalculationsForFactorizedSignalingNormObserver.Expect(o => o.UpdateObserver());
                waveHeightCalculationsForSignalingNormObserver.Expect(o => o.UpdateObserver());
                waveHeightCalculationsForLowerLimitNormObserver.Expect(o => o.UpdateObserver());
                waveHeightCalculationsForFactorizedLowerLimitNormObserver.Expect(o => o.UpdateObserver());

                waterLevelCalculationsForMechanismSpecificFactorizedSignalingNormObserver.Expect(o => o.UpdateObserver());
                waterLevelCalculationsForMechanismSpecificSignalingNormObserver.Expect(o => o.UpdateObserver());
                waterLevelCalculationsForMechanismSpecificLowerLimitNormObserver.Expect(o => o.UpdateObserver());
                waveHeightCalculationsForMechanismSpecificFactorizedSignalingNormObserver.Expect(o => o.UpdateObserver());
                waveHeightCalculationsForMechanismSpecificSignalingNormObserver.Expect(o => o.UpdateObserver());
                waveHeightCalculationsForMechanismSpecificLowerLimitNormObserver.Expect(o => o.UpdateObserver());

                duneLocationsObserver.Expect(o => o.UpdateObserver());

                duneLocationCalculationsForMechanismSpecificFactorizedSignalingNormObserver.Expect(o => o.UpdateObserver());
                duneLocationCalculationsForMechanismSpecificSignalingNormObserver.Expect(o => o.UpdateObserver());
                duneLocationCalculationsForMechanismSpecificLowerLimitNormObserver.Expect(o => o.UpdateObserver());
                duneLocationCalculationsForLowerLimitNormObserver.Expect(o => o.UpdateObserver());
                duneLocationCalculationsForFactorizedLowerLimitNormObserver.Expect(o => o.UpdateObserver());
            }
        }

        private static void AssertDesignWaterLevelCalculationOutput(IDesignWaterLevelCalculator designWaterLevelCalculator,
                                                                    HydraulicBoundaryLocationCalculationOutput actualOutput)
        {
            Assert.AreEqual(designWaterLevelCalculator.DesignWaterLevel, actualOutput.Result, actualOutput.Result.GetAccuracy());
            Assert.AreEqual(CalculationConvergence.CalculatedNotConverged, actualOutput.CalculationConvergence);
        }

        private static void AssertWaveHeightCalculationOutput(IWaveHeightCalculator waveHeightCalculator,
                                                              HydraulicBoundaryLocationCalculationOutput actualOutput)
        {
            Assert.AreEqual(waveHeightCalculator.WaveHeight, actualOutput.Result, actualOutput.Result.GetAccuracy());
            Assert.AreEqual(CalculationConvergence.CalculatedNotConverged, actualOutput.CalculationConvergence);
        }

        private static TreeNodeInfo GetInfo(RingtoetsPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(HydraulicBoundaryDatabaseContext));
        }
    }
}