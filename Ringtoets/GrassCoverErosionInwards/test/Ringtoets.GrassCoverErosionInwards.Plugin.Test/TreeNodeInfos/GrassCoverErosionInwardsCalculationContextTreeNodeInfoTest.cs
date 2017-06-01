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
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuUpdateDikeProfileIndex = 3;
        private const int contextMenuValidateIndex = 5;
        private const int contextMenuCalculateIndex = 6;
        private const int contextMenuClearIndex = 8;
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "HydraulicBoundaryDatabaseImporter");

        private MockRepository mocks;
        private GrassCoverErosionInwardsPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new GrassCoverErosionInwardsPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(GrassCoverErosionInwardsCalculationContext));
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
            Assert.IsNotNull(info.ChildNodeObjects);
            Assert.IsNotNull(info.CanRename);
            Assert.IsNotNull(info.OnNodeRenamed);
            Assert.IsNotNull(info.CanRemove);
            Assert.IsNotNull(info.OnNodeRemoved);
            Assert.IsNull(info.CanCheck);
            Assert.IsNull(info.IsChecked);
            Assert.IsNull(info.OnNodeChecked);
            Assert.IsNotNull(info.CanDrag);
            Assert.IsNull(info.CanDrop);
            Assert.IsNull(info.CanInsert);
            Assert.IsNull(info.OnDrop);
        }

        [Test]
        public void Image_Always_ReturnsCalculationIcon()
        {
            // Setup
            mocks.ReplayAll();

            // Call
            Image image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.CalculationIcon, image);
        }

        [Test]
        public void ChildNodeObjects_CalculationWithoutOutput_ReturnCollectionWithEmptyOutputObject()
        {
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var calculationContext = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanism, assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(calculationContext).ToArray();

            // Assert
            Assert.AreEqual(3, children.Length);

            var comment = children[0] as Comment;
            Assert.AreSame(calculationContext.WrappedData.Comments, comment);

            var grassCoverErosionInwardsInputContext = children[1] as GrassCoverErosionInwardsInputContext;
            Assert.IsNotNull(grassCoverErosionInwardsInputContext);
            Assert.AreSame(calculationContext.WrappedData.InputParameters, grassCoverErosionInwardsInputContext.WrappedData);

            var emptyOutput = children[2] as EmptyProbabilityAssessmentOutput;
            Assert.IsNotNull(emptyOutput);
        }

        [Test]
        public void ChildNodeObjects_CalculationWithOutput_ReturnCollectionWithOutputObject()
        {
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(0, true, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0),
                                                            new TestDikeHeightOutput(0.0),
                                                            new TestOvertoppingRateOutput(0))
            };
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var calculationContext = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanism, assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(calculationContext).ToArray();

            // Assert
            Assert.AreEqual(3, children.Length);

            var comment = children[0] as Comment;
            Assert.AreSame(calculationContext.WrappedData.Comments, comment);

            var grassCoverErosionInwardsInputContext = children[1] as GrassCoverErosionInwardsInputContext;
            Assert.IsNotNull(grassCoverErosionInwardsInputContext);
            Assert.AreSame(calculationContext.WrappedData.InputParameters, grassCoverErosionInwardsInputContext.WrappedData);

            var output = children[2] as GrassCoverErosionInwardsOutput;
            Assert.IsNotNull(output);
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var calculation = new GrassCoverErosionInwardsCalculation();
            var nodeData = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanism, assessmentSection);

            var menuBuilderMock = mocks.Stub<IContextMenuBuilder>();
            using (mocks.Ordered())
            {
                menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddRenameItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddDeleteItem()).Return(menuBuilderMock);
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
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilderMock);
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(nodeData, null, treeViewControl);
            }
            // Assert
            // Assert expectancies called in TearDown()
        }

        [Test]
        public void ContextMenuStrip_Always_AddCustomItems()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "random"
            };

            var calculation = new GrassCoverErosionInwardsCalculation();
            var failureMechanism = new TestGrassCoverErosionInwardsFailureMechanism();
            var nodeData = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, assessmentSection, treeViewControl))
                {
                    // Assert
                    Assert.AreEqual(15, menu.Items.Count);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuUpdateDikeProfileIndex,
                                                                  "&Bijwerken dijkprofiel...",
                                                                  "Er moet een dijkprofiel geselecteerd zijn.",
                                                                  RingtoetsCommonFormsResources.UpdateItemIcon,
                                                                  false);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateIndex,
                                                                  "&Valideren",
                                                                  "Valideer de invoer voor deze berekening.",
                                                                  RingtoetsCommonFormsResources.ValidateIcon);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateIndex,
                                                                  "Be&rekenen",
                                                                  "Voer deze berekening uit.",
                                                                  RingtoetsCommonFormsResources.CalculateIcon);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearIndex,
                                                                  "&Wis uitvoer...",
                                                                  "Deze berekening heeft geen uitvoer om te wissen.",
                                                                  RingtoetsCommonFormsResources.ClearIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationWithoutDikeProfile_ContextMenuItemUpdateDikeProfileDisabledAndToolTipSet()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var calculation = new GrassCoverErosionInwardsCalculation();
            var failureMechanism = new TestGrassCoverErosionInwardsFailureMechanism();
            var nodeData = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, assessmentSection, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                  contextMenuUpdateDikeProfileIndex,
                                                                  "&Bijwerken dijkprofiel...",
                                                                  "Er moet een dijkprofiel geselecteerd zijn.",
                                                                  RingtoetsCommonFormsResources.UpdateItemIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationWithDikeProfileAndInputInSync_ContextMenuItemUpdateDikeProfileDisabledAndToolTipSet()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = new TestDikeProfile()
                }
            };
            var failureMechanism = new TestGrassCoverErosionInwardsFailureMechanism();
            var nodeData = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, assessmentSection, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                  contextMenuUpdateDikeProfileIndex,
                                                                  "&Bijwerken dijkprofiel...",
                                                                  "Er zijn geen wijzigingen om bij te werken.",
                                                                  RingtoetsCommonFormsResources.UpdateItemIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationWithDikeProfileAndInputOutOfSync_ContextMenuItemUpdateDikeProfileEnabledAndToolTipSet()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var testDikeProfile = new TestDikeProfile();
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = testDikeProfile
                }
            };
            var failureMechanism = new TestGrassCoverErosionInwardsFailureMechanism();
            var nodeData = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                mocks.ReplayAll();

                plugin.Gui = gui;

                ChangeDikeProfile(testDikeProfile);

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, assessmentSection, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                  contextMenuUpdateDikeProfileIndex,
                                                                  "&Bijwerken dijkprofiel...",
                                                                  "Berekening bijwerken met het dijkprofiel.",
                                                                  RingtoetsCommonFormsResources.UpdateItemIcon);
                }
            }
        }

        [Test]
        public void GivenCalculationWithoutOutputAndWithInputOutOfSync_WhenUpdateDikeProfileClicked_ThenNoInquiryAndCalculationUpdatedAndInputObserverNotified()
        {
            // Given
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var dikeProfile = new TestDikeProfile();
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = dikeProfile
                }
            };
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var nodeData = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanism, assessmentSection);

            var inputObserver = mocks.StrictMock<IObserver>();
            inputObserver.Expect(obs => obs.UpdateObserver());
            calculation.InputParameters.Attach(inputObserver);

            var calculationObserver = mocks.StrictMock<IObserver>();
            calculation.Attach(calculationObserver);

            using (var treeViewControl = new TreeViewControl())
            {
                var mainWindow = mocks.Stub<IMainWindow>();
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                mocks.ReplayAll();

                plugin.Gui = gui;

                ChangeDikeProfile(dikeProfile);

                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, assessmentSection, treeViewControl))
                {
                    // When
                    menu.Items[contextMenuUpdateDikeProfileIndex].PerformClick();

                    // Then
                    Assert.IsTrue(calculation.InputParameters.IsDikeProfileInputSynchronized);

                    // Note: observer assertions are verified in the TearDown()
                }
            }
        }

        [Test]
        public void GivenCalculationWithOutputAndInputOutOfSync_WhenUpdateDikeProfileClickedAndCancelled_ThenInquiryAndCalculationNotUpdatedAndObserversNotNotified()
        {
            // Given
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var dikeProfile = new TestDikeProfile();
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = dikeProfile
                },
                Output = new TestGrassCoverErosionInwardsOutput()
            };

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var nodeData = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanism, assessmentSection);

            var inputObserver = mocks.StrictMock<IObserver>();
            calculation.InputParameters.Attach(inputObserver);

            var calculationObserver = mocks.StrictMock<IObserver>();
            calculation.Attach(calculationObserver);

            string textBoxMessage = null;
            DialogBoxHandler = (name, wnd) =>
            {
                var helper = new MessageBoxTester(wnd);
                textBoxMessage = helper.Text;
                helper.ClickCancel();
            };

            using (var treeViewControl = new TreeViewControl())
            {
                var mainWindow = mocks.Stub<IMainWindow>();
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                mocks.ReplayAll();

                plugin.Gui = gui;

                ChangeDikeProfile(dikeProfile);

                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, assessmentSection, treeViewControl))
                {
                    // When
                    menu.Items[contextMenuUpdateDikeProfileIndex].PerformClick();

                    // Then
                    Assert.IsTrue(calculation.HasOutput);
                    Assert.IsFalse(calculation.InputParameters.IsDikeProfileInputSynchronized);

                    string expectedMessage = "Als u kiest voor bijwerken, dan wordt het resultaat van deze berekening " +
                                             $"verwijderd.{Environment.NewLine}{Environment.NewLine}Weet u zeker dat u wilt doorgaan?";
                    Assert.AreEqual(expectedMessage, textBoxMessage);

                    // Note: observer assertions are verified in the TearDown()
                }
            }
        }

        [Test]
        public void GivenCalculationWithOutputAndInputOutOfSync_WhenUpdateDikeProfileClickedAndContinued_ThenInquiryAndCalculationUpdatedAndObserversNotified()
        {
            // Given
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var dikeProfile = new TestDikeProfile();
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = dikeProfile
                },
                Output = new TestGrassCoverErosionInwardsOutput()
            };
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var nodeData = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanism, assessmentSection);

            var inputObserver = mocks.StrictMock<IObserver>();
            inputObserver.Expect(obs => obs.UpdateObserver());
            calculation.InputParameters.Attach(inputObserver);

            var calculationObserver = mocks.StrictMock<IObserver>();
            calculationObserver.Expect(obs => obs.UpdateObserver());
            calculation.Attach(calculationObserver);

            string textBoxMessage = null;
            DialogBoxHandler = (name, wnd) =>
            {
                var helper = new MessageBoxTester(wnd);
                textBoxMessage = helper.Text;
                helper.ClickOk();
            };

            using (var treeViewControl = new TreeViewControl())
            {
                var mainWindow = mocks.Stub<IMainWindow>();
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                mocks.ReplayAll();

                plugin.Gui = gui;

                ChangeDikeProfile(dikeProfile);

                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, assessmentSection, treeViewControl))
                {
                    // When
                    menu.Items[contextMenuUpdateDikeProfileIndex].PerformClick();

                    // Then
                    Assert.IsFalse(calculation.HasOutput);
                    Assert.IsTrue(calculation.InputParameters.IsDikeProfileInputSynchronized);

                    string expectedMessage = "Als u kiest voor bijwerken, dan wordt het resultaat van deze berekening " +
                                             $"verwijderd.{Environment.NewLine}{Environment.NewLine}Weet u zeker dat u wilt doorgaan?";
                    Assert.AreEqual(expectedMessage, textBoxMessage);

                    // Note: observer assertions are verified in the TearDown()
                }
            }
        }

        [Test]
        public void ContextMenuStrip_NoHydraulicBoundaryDatabase_ContextMenuItemPerformCalculationDisabledAndTooltipSet()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var calculation = new GrassCoverErosionInwardsCalculation();
            var failureMechanism = new TestGrassCoverErosionInwardsFailureMechanism();
            var nodeData = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanism, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuCalculateIndex,
                                                                  "Be&rekenen",
                                                                  "Er is geen hydraulische randvoorwaardendatabase geïmporteerd.",
                                                                  RingtoetsCommonFormsResources.CalculateIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_HydraulicBoundaryDatabaseNotValid_ContextMenuItemPerformCalculationDisabledAndTooltipSet()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            var calculation = new GrassCoverErosionInwardsCalculation();
            var failureMechanism = new TestGrassCoverErosionInwardsFailureMechanism();
            var nodeData = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanism, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    ToolStripItem contextMenuItem = contextMenu.Items[contextMenuCalculateIndex];

                    Assert.AreEqual("Be&rekenen", contextMenuItem.Text);
                    StringAssert.Contains("Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt.", contextMenuItem.ToolTipText);
                    TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.CalculateIcon, contextMenuItem.Image);
                    Assert.IsFalse(contextMenuItem.Enabled);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismContributionZero_ContextMenuItemPerformCalculationDisabledAndTooltipSet()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "1.0"
            };

            var calculation = new GrassCoverErosionInwardsCalculation();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var nodeData = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanism, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuCalculateIndex,
                                                                  "Be&rekenen",
                                                                  "De bijdrage van dit toetsspoor is nul.",
                                                                  RingtoetsCommonFormsResources.CalculateIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_AllRequiredInputSet_ContextMenuItemPerformCalculationEnabled()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "1.0"
            };

            var calculation = new GrassCoverErosionInwardsCalculation();
            var failureMechanism = new TestGrassCoverErosionInwardsFailureMechanism();
            var nodeData = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanism, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuCalculateIndex,
                                                                  "Be&rekenen",
                                                                  "Voer deze berekening uit.",
                                                                  RingtoetsCommonFormsResources.CalculateIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_NoHydraulicBoundaryDatabase_ContextMenuItemValidateCalculationDisabledAndTooltipSet()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var calculation = new GrassCoverErosionInwardsCalculation();
            var failureMechanism = new TestGrassCoverErosionInwardsFailureMechanism();
            var nodeData = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanism, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuValidateIndex,
                                                                  "&Valideren",
                                                                  "Er is geen hydraulische randvoorwaardendatabase geïmporteerd.",
                                                                  RingtoetsCommonFormsResources.ValidateIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_HydraulicBoundaryDatabaseNotValid_ContextMenuItemValidateCalculationDisabledAndTooltipSet()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            var calculation = new GrassCoverErosionInwardsCalculation();
            var failureMechanism = new TestGrassCoverErosionInwardsFailureMechanism();
            var nodeData = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanism, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    ToolStripItem contextMenuItem = contextMenu.Items[contextMenuValidateIndex];

                    Assert.AreEqual("&Valideren", contextMenuItem.Text);
                    StringAssert.Contains("Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt.", contextMenuItem.ToolTipText);
                    TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.ValidateIcon, contextMenuItem.Image);
                    Assert.IsFalse(contextMenuItem.Enabled);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismContributionZero_ContextMenuItemValidateCalculationDisabledAndTooltipSet()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "1.0"
            };

            var calculation = new GrassCoverErosionInwardsCalculation();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var nodeData = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanism, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuValidateIndex,
                                                                  "&Valideren",
                                                                  "De bijdrage van dit toetsspoor is nul.",
                                                                  RingtoetsCommonFormsResources.ValidateIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_AllRequiredInputSet_ContextMenuItemValidateCalculationEnabled()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "1.0"
            };

            var calculation = new GrassCoverErosionInwardsCalculation();
            var failureMechanism = new TestGrassCoverErosionInwardsFailureMechanism();
            var nodeData = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanism, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuValidateIndex,
                                                                  "&Valideren",
                                                                  "Valideer de invoer voor deze berekening.",
                                                                  RingtoetsCommonFormsResources.ValidateIcon);
                }
            }
        }

        [Test]
        public void GivenCalculationThatSucceeds_WhenCalculatingFromContextMenu_ThenOutputSetLogMessagesAddedAndUpdateObserver()
        {
            // Given
            var mainWindow = mocks.DynamicMock<IMainWindow>();
            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());

            var failureMechanism = new TestGrassCoverErosionInwardsFailureMechanism();

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.Id).Return(string.Empty);
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "random",
                Locations =
                {
                    hydraulicBoundaryLocation
                }
            };
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 1, 1));

            var initialOutput = new GrassCoverErosionInwardsOutput(0, true,
                                                                   new ProbabilityAssessmentOutput(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN),
                                                                   new TestDikeHeightOutput(double.NaN),
                                                                   new TestOvertoppingRateOutput(double.NaN));
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Output = initialOutput,
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    DikeProfile = new TestDikeProfile()
                }
            };

            var calculationContext = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanism, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(calculationContext, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mainWindow);

                var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(testDataPath)).Return(new TestOvertoppingCalculator());
                mocks.ReplayAll();

                plugin.Gui = gui;

                calculation.Attach(observerMock);

                DialogBoxHandler = (name, wnd) =>
                {
                    // Expect an activity dialog which is automatically closed
                };

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(calculationContext, null, treeViewControl))
                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                {
                    // When
                    Action action = () => contextMenuStrip.Items[contextMenuCalculateIndex].PerformClick();

                    // Then
                    TestHelper.AssertLogMessages(action, messages =>
                    {
                        string[] msgs = messages.ToArray();
                        Assert.AreEqual(6, msgs.Length);
                        StringAssert.StartsWith($"Validatie van '{calculation.Name}' gestart om: ", msgs[0]);
                        StringAssert.StartsWith($"Validatie van '{calculation.Name}' beëindigd om: ", msgs[1]);
                        StringAssert.StartsWith($"Berekening van '{calculation.Name}' gestart om: ", msgs[2]);
                        StringAssert.StartsWith("De overloop en overslag berekening is uitgevoerd op de tijdelijke locatie", msgs[3]);
                        StringAssert.StartsWith($"Berekening van '{calculation.Name}' beëindigd om: ", msgs[4]);
                        StringAssert.StartsWith($"Uitvoeren van '{calculation.Name}' is gelukt.", msgs[5]);
                    });

                    Assert.AreNotSame(initialOutput, calculation.Output);
                }
            }
        }

        [Test]
        public void GivenCalculation_WhenValidatingFromContextMenu_ThenLogMessagesAdded()
        {
            // Given
            var observerMock = mocks.StrictMock<IObserver>();

            var failureMechanism = new TestGrassCoverErosionInwardsFailureMechanism();

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "random",
                Locations =
                {
                    hydraulicBoundaryLocation
                }
            };

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    DikeProfile = new TestDikeProfile()
                }
            };

            var calculationContext = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanism, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(calculationContext, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mocks.ReplayAll();

                plugin.Gui = gui;

                calculation.Attach(observerMock);

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(calculationContext, null, treeViewControl))
                {
                    // When
                    Action action = () => contextMenuStrip.Items[contextMenuValidateIndex].PerformClick();

                    // Then
                    TestHelper.AssertLogMessages(action, messages =>
                    {
                        string[] msgs = messages.ToArray();
                        StringAssert.StartsWith($"Validatie van '{calculation.Name}' gestart om: ", msgs[0]);
                        StringAssert.StartsWith($"Validatie van '{calculation.Name}' beëindigd om: ", msgs[1]);
                    });
                }
            }
        }

        [Test]
        public void OnNodeRemoved_ParentIsCalculationGroupContext_RemoveCalculationFromGroup()
        {
            // Setup
            var group = new CalculationGroup();
            var elementToBeRemoved = new GrassCoverErosionInwardsCalculation();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var observerMock = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var calculationContext = new GrassCoverErosionInwardsCalculationContext(elementToBeRemoved,
                                                                                    failureMechanism,
                                                                                    assessmentSection);
            var groupContext = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                                   failureMechanism,
                                                                                   assessmentSection);

            observerMock.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            group.Children.Add(elementToBeRemoved);
            group.Children.Add(new GrassCoverErosionInwardsCalculation());
            group.Attach(observerMock);

            // Precondition
            Assert.IsTrue(info.CanRemove(calculationContext, groupContext));
            Assert.AreEqual(2, group.Children.Count);

            // Call
            info.OnNodeRemoved(calculationContext, groupContext);

            // Assert
            Assert.AreEqual(1, group.Children.Count);
            CollectionAssert.DoesNotContain(group.Children, elementToBeRemoved);
        }

        [Test]
        public void OnNodeRemoved_NodeRemoved_RemoveCalculationFromFailureMechanismSectionResults()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var group = new CalculationGroup();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test", new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(1.0, 1.0)
            }));
            DikeProfile dikeProfile = new TestDikeProfile(new Point2D(0.5, 0.5));
            failureMechanism.DikeProfiles.AddRange(new[]
            {
                dikeProfile
            }, "path");

            var elementToBeRemoved = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = dikeProfile
                }
            };

            GrassCoverErosionInwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.First();
            sectionResult.Calculation = elementToBeRemoved;

            var calculationContext = new GrassCoverErosionInwardsCalculationContext(elementToBeRemoved,
                                                                                    failureMechanism,
                                                                                    assessmentSection);
            var groupContext = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                                   failureMechanism,
                                                                                   assessmentSection);

            group.Children.Add(elementToBeRemoved);
            group.Children.Add(new GrassCoverErosionInwardsCalculation());

            // Precondition
            Assert.IsTrue(info.CanRemove(calculationContext, groupContext));
            Assert.AreEqual(2, group.Children.Count);
            Assert.AreEqual(elementToBeRemoved, sectionResult.Calculation);

            // Call
            info.OnNodeRemoved(calculationContext, groupContext);

            // Assert
            Assert.AreEqual(1, group.Children.Count);
            CollectionAssert.DoesNotContain(group.Children, elementToBeRemoved);
            Assert.IsNull(sectionResult.Calculation);
        }

        [Test]
        public void OnNodeRemoved_CalculationInGroupAssignedToSection_CalculationDetachedFromSection()
        {
            // Setup
            var group = new CalculationGroup();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var elementToBeRemoved = new GrassCoverErosionInwardsCalculation();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var calculationContext = new GrassCoverErosionInwardsCalculationContext(elementToBeRemoved,
                                                                                    failureMechanism,
                                                                                    assessmentSection);
            var groupContext = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                                   failureMechanism,
                                                                                   assessmentSection);

            mocks.ReplayAll();

            group.Children.Add(elementToBeRemoved);

            failureMechanism.AddSection(new FailureMechanismSection("section", new[]
            {
                new Point2D(0, 0)
            }));

            GrassCoverErosionInwardsFailureMechanismSectionResult result = failureMechanism.SectionResults.First();
            result.Calculation = elementToBeRemoved;

            // Call
            info.OnNodeRemoved(calculationContext, groupContext);

            // Assert
            Assert.IsNull(result.Calculation);
        }

        private static void ChangeDikeProfile(DikeProfile dikeProfile)
        {
            dikeProfile.CopyProperties(new DikeProfile(dikeProfile.WorldReferencePoint,
                                                       dikeProfile.DikeGeometry,
                                                       new[]
                                                       {
                                                           new Point2D(1.1, 2.2),
                                                           new Point2D(3.3, 4.4)
                                                       },
                                                       new BreakWater(BreakWaterType.Caisson, 10),
                                                       new DikeProfile.ConstructionProperties
                                                       {
                                                           Id = dikeProfile.Id,
                                                           DikeHeight = 10,
                                                           Orientation = 10
                                                       }));
        }

        public override void TearDown()
        {
            plugin.Dispose();
            mocks.VerifyAll();

            base.TearDown();
        }
    }
}