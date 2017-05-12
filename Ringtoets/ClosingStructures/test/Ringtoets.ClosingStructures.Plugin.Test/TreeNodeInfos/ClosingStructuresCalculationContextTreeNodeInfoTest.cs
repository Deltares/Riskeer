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
using Core.Common.Base.Data;
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
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Data.TestUtil;
using Ringtoets.ClosingStructures.Forms.PresentationObjects;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.ClosingStructures.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class ClosingStructuresCalculationContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuUpdateForeshoreProfileIndex = 3;
        private const int contextMenuUpdateStructureIndex = 4;
        private const int contextMenuValidateIndex = 6;
        private const int contextMenuCalculateIndex = 7;
        private const int contextMenuClearIndex = 9;
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "HydraulicBoundaryDatabaseImporter");

        private IGui gui;
        private TreeNodeInfo info;
        private MockRepository mocks;
        private ClosingStructuresPlugin plugin;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            gui = mocks.Stub<IGui>();
            plugin = new ClosingStructuresPlugin
            {
                Gui = gui
            };

            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(ClosingStructuresCalculationContext));
        }

        [TearDown]
        public override void TearDown()
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

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var calculationContext = new ClosingStructuresCalculationContext(calculation, failureMechanism, assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(calculationContext).ToArray();

            // Assert
            Assert.AreEqual(3, children.Length);

            var comment = children[0] as Comment;
            Assert.AreSame(calculationContext.WrappedData.Comments, comment);

            var closingStructuresInputContext = children[1] as ClosingStructuresInputContext;
            Assert.IsNotNull(closingStructuresInputContext);
            Assert.AreSame(calculationContext.WrappedData.InputParameters, closingStructuresInputContext.WrappedData);

            Assert.IsInstanceOf<EmptyProbabilityAssessmentOutput>(children[2]);
        }

        [Test]
        public void ChildNodeObjects_CalculationWithOutput_ReturnCollectionWithOutputObject()
        {
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                Output = new TestStructuresOutput()
            };

            var calculationContext = new ClosingStructuresCalculationContext(calculation, failureMechanism, assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(calculationContext).ToArray();

            // Assert
            Assert.AreEqual(3, children.Length);

            var comment = children[0] as Comment;
            Assert.AreSame(calculationContext.WrappedData.Comments, comment);

            var closingStructuresInputContext = children[1] as ClosingStructuresInputContext;
            Assert.IsNotNull(closingStructuresInputContext);
            Assert.AreSame(calculationContext.WrappedData.InputParameters, closingStructuresInputContext.WrappedData);

            Assert.IsInstanceOf<ProbabilityAssessmentOutput>(children[2]);
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var nodeData = new ClosingStructuresCalculationContext(calculation, failureMechanism, assessmentSection);

            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();
            using (mocks.Ordered())
            {
                menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddRenameItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
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
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilderMock);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                // Call
                info.ContextMenuStrip(nodeData, null, treeViewControl);
            }
            // Assert
            // Assert expectancies are called in TearDown()
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

            var failureMechanism = new TestClosingStructuresFailureMechanism();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var nodeData = new ClosingStructuresCalculationContext(calculation, failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, assessmentSection, treeViewControl))
                {
                    // Assert
                    Assert.AreEqual(16, menu.Items.Count);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuUpdateForeshoreProfileIndex,
                                                                  "&Bijwerken voorlandprofiel...",
                                                                  "Er moet een voorlandprofiel geselecteerd zijn.",
                                                                  RingtoetsCommonFormsResources.UpdateItemIcon,
                                                                  false);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuUpdateStructureIndex,
                                                                  "&Bijwerken kunstwerk",
                                                                  "Er moet een kunstwerk geselecteerd zijn.",
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
        public void ContextMenuStrip_CalculationWithoutStructure_ContextMenuItemUpdateStructureDisabled()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var failureMechanism = new TestClosingStructuresFailureMechanism();
            var nodeData = new ClosingStructuresCalculationContext(calculation, failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, assessmentSection, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                  contextMenuUpdateStructureIndex,
                                                                  "&Bijwerken kunstwerk",
                                                                  "Er moet een kunstwerk geselecteerd zijn.",
                                                                  RingtoetsCommonFormsResources.UpdateItemIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationWithStructure_ContextMenuItemUpdateStructureEnabled()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestClosingStructure()
                }
            };
            var failureMechanism = new TestClosingStructuresFailureMechanism();
            var nodeData = new ClosingStructuresCalculationContext(calculation, failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, assessmentSection, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                  contextMenuUpdateStructureIndex,
                                                                  "&Bijwerken kunstwerk",
                                                                  "Berekening bijwerken met het kunstwerk.",
                                                                  RingtoetsCommonFormsResources.UpdateItemIcon);
                }
            }
        }

        [Test]
        public void GivenCalculationWithStructureWithoutOutput_WhenStructureAndInputOutOfSyncAndUpdateClicked_ThenNoInquiryAndCalculationUpdatedAndInputObserverNotified()
        {
            // Given
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var structure = new TestClosingStructure();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = structure
                }
            };
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var nodeData = new ClosingStructuresCalculationContext(calculation, failureMechanism, assessmentSection);

            var inputObserver = mocks.StrictMock<IObserver>();
            inputObserver.Expect(obs => obs.UpdateObserver());
            calculation.InputParameters.Attach(inputObserver);

            var calculationObserver = mocks.StrictMock<IObserver>();
            calculation.Attach(calculationObserver);

            using (var treeViewControl = new TreeViewControl())
            {
                var mainWindow = mocks.Stub<IMainWindow>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                mocks.ReplayAll();

                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, assessmentSection, treeViewControl))
                {
                    // When
                    UpdateStructure(structure);
                    menu.Items[contextMenuUpdateStructureIndex].PerformClick();

                    // Then
                    Assert.IsFalse(calculation.HasOutput);
                    AssertClosingStructuresInput(structure, calculation.InputParameters);

                    // Note: observer assertions are verified in the TearDown()
                }
            }
        }

        [Test]
        public void GivenCalculationWithStructureWithOutput_WhenStructureAndInputOutOfSyncAndUpdateClickedAndCancelled_ThenInquiryAndCalculationNotUpdatedAndObserversNotNotified()
        {
            // Given
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var structure = new TestClosingStructure();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = structure
                },
                Output = new TestStructuresOutput()
            };

            ClosingStructuresInput calculationInput = calculation.InputParameters;
            RoundedDouble expectedStructureNormalOrientation = calculationInput.StructureNormalOrientation;
            NormalDistribution expectedLevelCrestStructureNotClosing = calculationInput.LevelCrestStructureNotClosing;
            LogNormalDistribution expectedFlowWidthAtBottomProtection = calculationInput.FlowWidthAtBottomProtection;
            VariationCoefficientLogNormalDistribution expectedCriticalOvertoppingDischarge = calculationInput.CriticalOvertoppingDischarge;
            NormalDistribution expectedWidthFlowApertures = calculationInput.WidthFlowApertures;
            VariationCoefficientLogNormalDistribution expectedStorageStructureArea = calculationInput.StorageStructureArea;
            LogNormalDistribution expectedAllowedLevelIncreaseStorage = calculationInput.AllowedLevelIncreaseStorage;
            ClosingStructureInflowModelType expectedInflowModelType = calculationInput.InflowModelType;
            LogNormalDistribution expectedAreaFlowApertures = calculationInput.AreaFlowApertures;
            double expectedFailureProbabilityOpenStructure = calculationInput.FailureProbabilityOpenStructure;
            double expectedFailureProbabilityReparation = calculationInput.FailureProbabilityReparation;
            int expectedIdenticalApertures = calculationInput.IdenticalApertures;
            NormalDistribution expectedInsideWaterLevel = calculationInput.InsideWaterLevel;
            double expectedProbabilityOrFrequencyOpenStructureBeforeFlooding = calculationInput.ProbabilityOrFrequencyOpenStructureBeforeFlooding;
            NormalDistribution expectedThresholdHeightOpenWeir = calculationInput.ThresholdHeightOpenWeir;

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var nodeData = new ClosingStructuresCalculationContext(calculation, failureMechanism, assessmentSection);

            var inputObserver = mocks.StrictMock<IObserver>();
            calculationInput.Attach(inputObserver);

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
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                mocks.ReplayAll();

                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, assessmentSection, treeViewControl))
                {
                    // When
                    UpdateStructure(structure);
                    menu.Items[contextMenuUpdateStructureIndex].PerformClick();

                    // Then
                    Assert.IsTrue(calculation.HasOutput);
                    Assert.AreSame(structure, calculationInput.Structure);
                    Assert.AreEqual(expectedStructureNormalOrientation, calculationInput.StructureNormalOrientation);
                    Assert.AreEqual(expectedLevelCrestStructureNotClosing, calculationInput.LevelCrestStructureNotClosing);
                    Assert.AreEqual(expectedFlowWidthAtBottomProtection, calculationInput.FlowWidthAtBottomProtection);
                    Assert.AreEqual(expectedCriticalOvertoppingDischarge, calculationInput.CriticalOvertoppingDischarge);
                    Assert.AreEqual(expectedWidthFlowApertures, calculationInput.WidthFlowApertures);
                    Assert.AreEqual(expectedStorageStructureArea, calculationInput.StorageStructureArea);
                    Assert.AreEqual(expectedAllowedLevelIncreaseStorage, calculationInput.AllowedLevelIncreaseStorage);
                    Assert.AreEqual(expectedInflowModelType, calculationInput.InflowModelType);
                    Assert.AreEqual(expectedAreaFlowApertures, calculationInput.AreaFlowApertures);
                    Assert.AreEqual(expectedFailureProbabilityOpenStructure, calculationInput.FailureProbabilityOpenStructure);
                    Assert.AreEqual(expectedFailureProbabilityReparation, calculationInput.FailureProbabilityReparation);
                    Assert.AreEqual(expectedIdenticalApertures, calculationInput.IdenticalApertures);
                    Assert.AreEqual(expectedInsideWaterLevel, calculationInput.InsideWaterLevel);
                    Assert.AreEqual(expectedProbabilityOrFrequencyOpenStructureBeforeFlooding, calculationInput.ProbabilityOrFrequencyOpenStructureBeforeFlooding);
                    Assert.AreEqual(expectedThresholdHeightOpenWeir, calculationInput.ThresholdHeightOpenWeir);

                    string expectedMessage = "Wanneer het kunstwerk wijzigt als gevolg van het bijwerken, " +
                                             "zal het resultaat van deze berekening worden " +
                                             $"verwijderd.{Environment.NewLine}{Environment.NewLine}Weet u zeker dat u wilt doorgaan?";
                    Assert.AreEqual(expectedMessage, textBoxMessage);

                    // Note: observer assertions are verified in the TearDown()
                }
            }
        }

        [Test]
        public void GivenCalculationWithStructureWithOutput_WhenStructureAndInputOutOfSyncAndUpdateClickedAndContinued_ThenInquiryAndUpdatesCalculationAndObserversNotified()
        {
            // Given
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var structure = new TestClosingStructure();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = structure
                },
                Output = new TestStructuresOutput()
            };
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var nodeData = new ClosingStructuresCalculationContext(calculation, failureMechanism, assessmentSection);

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
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                mocks.ReplayAll();

                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, assessmentSection, treeViewControl))
                {
                    // When
                    UpdateStructure(structure);
                    menu.Items[contextMenuUpdateStructureIndex].PerformClick();

                    // Then
                    Assert.IsFalse(calculation.HasOutput);
                    AssertClosingStructuresInput(structure, calculation.InputParameters);

                    string expectedMessage = "Wanneer het kunstwerk wijzigt als gevolg van het bijwerken, " +
                                             "zal het resultaat van deze berekening worden " +
                                             $"verwijderd.{Environment.NewLine}{Environment.NewLine}Weet u zeker dat u wilt doorgaan?";
                    Assert.AreEqual(expectedMessage, textBoxMessage);

                    // Note: observer assertions are verified in the TearDown()
                }
            }
        }

        [Test]
        public void GivenCalculationWithStructureWithOutput_WhenStructureAndInputInSyncAndUpdateClickedAndContinued_ThenInquiryAndCalculationNotUpdatedAndObserversNotNotified()
        {
            // Given
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var structure = new TestClosingStructure();
            UpdateStructure(structure);

            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = structure
                },
                Output = new TestStructuresOutput()
            };

            var inputObserver = mocks.StrictMock<IObserver>();
            calculation.InputParameters.Attach(inputObserver);

            var calculationObserver = mocks.StrictMock<IObserver>();
            calculation.Attach(calculationObserver);

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var nodeData = new ClosingStructuresCalculationContext(calculation, failureMechanism, assessmentSection);

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
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                mocks.ReplayAll();

                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, assessmentSection, treeViewControl))
                {
                    // When
                    menu.Items[contextMenuUpdateStructureIndex].PerformClick();

                    // Then
                    Assert.IsTrue(calculation.HasOutput);
                    AssertClosingStructuresInput(structure, calculation.InputParameters);

                    string expectedMessage = "Wanneer het kunstwerk wijzigt als gevolg van het bijwerken, " +
                                             "zal het resultaat van deze berekening worden " +
                                             $"verwijderd.{Environment.NewLine}{Environment.NewLine}Weet u zeker dat u wilt doorgaan?";
                    Assert.AreEqual(expectedMessage, textBoxMessage);

                    // Note: observer assertions are verified in the TearDown()
                }
            }
        }

        [Test]
        public void GivenCalculationWithStructureWithOutput_WhenStructureAndInputPartiallyOutOfSyncAndUpdateClicked_ThenInquiryAndUpdatesCalculationAndNotifiesObserver()
        {
            // Given
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var structure = new TestClosingStructure();
            UpdateStructure(structure);

            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = structure
                },
                Output = new TestStructuresOutput()
            };

            var inputObserver = mocks.StrictMock<IObserver>();
            inputObserver.Expect(obs => obs.UpdateObserver());
            calculation.InputParameters.Attach(inputObserver);

            var calculationObserver = mocks.StrictMock<IObserver>();
            calculationObserver.Expect(obs => obs.UpdateObserver());
            calculation.Attach(calculationObserver);

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var nodeData = new ClosingStructuresCalculationContext(calculation, failureMechanism, assessmentSection);

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
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                mocks.ReplayAll();

                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, assessmentSection, treeViewControl))
                {
                    // When
                    ClosingStructuresInput inputParameters = calculation.InputParameters;
                    inputParameters.StructureNormalOrientation = (RoundedDouble) 1.1;
                    menu.Items[contextMenuUpdateStructureIndex].PerformClick();

                    // Then
                    Assert.IsFalse(calculation.HasOutput);
                    AssertClosingStructuresInput(structure, calculation.InputParameters);

                    string expectedMessage = "Wanneer het kunstwerk wijzigt als gevolg van het bijwerken, " +
                                             "zal het resultaat van deze berekening worden " +
                                             $"verwijderd.{Environment.NewLine}{Environment.NewLine}Weet u zeker dat u wilt doorgaan?";
                    Assert.AreEqual(expectedMessage, textBoxMessage);

                    // Note: observer assertions are verified in the TearDown()
                }
            }
        }

        [Test]
        public void ContextMenuStrip_NoHydraulicBoundaryDatabase_ContextMenuItemPerformCalculationAndValidationDisabledAndTooltipSet()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var failureMechanism = new TestClosingStructuresFailureMechanism();
            var nodeData = new ClosingStructuresCalculationContext(calculation, failureMechanism, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuCalculateIndex,
                                                                  "Be&rekenen",
                                                                  "Er is geen hydraulische randvoorwaardendatabase geïmporteerd.",
                                                                  RingtoetsCommonFormsResources.CalculateIcon,
                                                                  false);

                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuValidateIndex,
                                                                  "&Valideren",
                                                                  "Er is geen hydraulische randvoorwaardendatabase geïmporteerd.",
                                                                  RingtoetsCommonFormsResources.ValidateIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_HydraulicBoundaryDatabaseNotValid_ContextMenuItemPerformCalculationAndValidationDisabledAndTooltipSet()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var failureMechanism = new TestClosingStructuresFailureMechanism();
            var nodeData = new ClosingStructuresCalculationContext(calculation, failureMechanism, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    ToolStripItem calculateContextMenuItem = contextMenu.Items[contextMenuCalculateIndex];

                    Assert.AreEqual("Be&rekenen", calculateContextMenuItem.Text);
                    StringAssert.Contains("Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt.", calculateContextMenuItem.ToolTipText);
                    TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.CalculateIcon, calculateContextMenuItem.Image);
                    Assert.IsFalse(calculateContextMenuItem.Enabled);

                    ToolStripItem validateContextMenuItem = contextMenu.Items[contextMenuValidateIndex];

                    Assert.AreEqual("&Valideren", validateContextMenuItem.Text);
                    StringAssert.Contains("Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt.", validateContextMenuItem.ToolTipText);
                    TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.ValidateIcon, validateContextMenuItem.Image);
                    Assert.IsFalse(validateContextMenuItem.Enabled);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismContributionZero_ContextMenuItemPerformCalculationAndValidationDisabledAndTooltipSet()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "1.0"
            };

            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var nodeData = new ClosingStructuresCalculationContext(calculation, failureMechanism, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuCalculateIndex,
                                                                  "Be&rekenen",
                                                                  "De bijdrage van dit toetsspoor is nul.",
                                                                  RingtoetsCommonFormsResources.CalculateIcon,
                                                                  false);

                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuValidateIndex,
                                                                  "&Valideren",
                                                                  "De bijdrage van dit toetsspoor is nul.",
                                                                  RingtoetsCommonFormsResources.ValidateIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_AllRequiredInputSet_ContextMenuItemPerformCalculationAndValidationEnabled()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "1.0"
            };

            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var failureMechanism = new TestClosingStructuresFailureMechanism();
            var nodeData = new ClosingStructuresCalculationContext(calculation, failureMechanism, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuCalculateIndex,
                                                                  "Be&rekenen",
                                                                  "Voer deze berekening uit.",
                                                                  RingtoetsCommonFormsResources.CalculateIcon);

                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuValidateIndex,
                                                                  "&Valideren",
                                                                  "Valideer de invoer voor deze berekening.",
                                                                  RingtoetsCommonFormsResources.ValidateIcon);
                }
            }
        }

        [Test]
        public void GivenSuccessfulCalculation_WhenCalculatingFromContextMenu_ThenOutputSetLogMessagesAddedAndUpdateObserver()
        {
            // Given
            var mainWindow = mocks.Stub<IMainWindow>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var failureMechanism = new TestClosingStructuresFailureMechanism();

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
            assessmentSection.Stub(a => a.Id).Return(string.Empty);
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 1, 1));

            var initialOutput = new ProbabilityAssessmentOutput(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN);
            var calculation = new TestClosingStructuresCalculation
            {
                Output = initialOutput,
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var calculationContext = new ClosingStructuresCalculationContext(calculation, failureMechanism, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(calculationContext, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mainWindow);

                mocks.ReplayAll();

                calculation.Attach(observer);

                DialogBoxHandler = (name, wnd) =>
                {
                    // Expect an activity dialog which is automatically closed
                };

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(calculationContext, null, treeViewControl))
                using (new HydraRingCalculatorFactoryConfig())
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
                        StringAssert.StartsWith("Betrouwbaarheid sluiting kunstwerk berekening is uitgevoerd op de tijdelijke locatie", msgs[3]);
                        StringAssert.StartsWith($"Berekening van '{calculation.Name}' beëindigd om: ", msgs[4]);
                        StringAssert.StartsWith($"Uitvoeren van '{calculation.Name}' is gelukt.", msgs[5]);
                    });

                    Assert.AreNotSame(initialOutput, calculation.Output);
                }
            }
        }

        [Test]
        public void GivenCalculationWithNonExistingFilePath_WhenValidatingFromContextMenu_ThenLogMessagesAdded()
        {
            // Given
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

            var calculation = new TestClosingStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var observer = mocks.StrictMock<IObserver>();
            calculation.Attach(observer);

            var failureMechanism = new TestClosingStructuresFailureMechanism();
            var calculationContext = new ClosingStructuresCalculationContext(calculation, failureMechanism, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(calculationContext, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(calculationContext, null, treeViewControl))
                {
                    // When
                    Action action = () => contextMenuStrip.Items[contextMenuValidateIndex].PerformClick();

                    // Then
                    TestHelper.AssertLogMessages(action, messages =>
                    {
                        string[] msgs = messages.ToArray();
                        Assert.AreEqual(2, msgs.Length);
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
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var elementToBeRemoved = new StructuresCalculation<ClosingStructuresInput>();
            var observer = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var calculationContext = new ClosingStructuresCalculationContext(elementToBeRemoved,
                                                                             failureMechanism,
                                                                             assessmentSection);
            var groupContext = new ClosingStructuresCalculationGroupContext(group,
                                                                            failureMechanism,
                                                                            assessmentSection);

            observer.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            group.Children.Add(elementToBeRemoved);
            group.Children.Add(new StructuresCalculation<ClosingStructuresInput>());
            group.Attach(observer);

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
        public void OnNodeRemoved_CalculationInGroupAssignedToSection_CalculationDetachedFromSection()
        {
            // Setup
            var group = new CalculationGroup();
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var elementToBeRemoved = new StructuresCalculation<ClosingStructuresInput>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var calculationContext = new ClosingStructuresCalculationContext(elementToBeRemoved,
                                                                             failureMechanism,
                                                                             assessmentSection);
            var groupContext = new ClosingStructuresCalculationGroupContext(group,
                                                                            failureMechanism,
                                                                            assessmentSection);

            mocks.ReplayAll();

            group.Children.Add(elementToBeRemoved);

            failureMechanism.AddSection(new FailureMechanismSection("section", new[]
            {
                new Point2D(0, 0)
            }));

            ClosingStructuresFailureMechanismSectionResult result = failureMechanism.SectionResults.First();
            result.Calculation = elementToBeRemoved;

            // Call
            info.OnNodeRemoved(calculationContext, groupContext);

            // Assert
            Assert.IsNull(result.Calculation);
        }

        private static void UpdateStructure(ClosingStructure structure)
        {
            var structureToUpdateFrom = new ClosingStructure(
                new ClosingStructure.ConstructionProperties
                {
                    Id = structure.Id,
                    Name = structure.Name,
                    Location = structure.Location,
                    StructureNormalOrientation = (RoundedDouble) 1.0,
                    LevelCrestStructureNotClosing =
                    {
                        Mean = (RoundedDouble) 2.0,
                        StandardDeviation = (RoundedDouble) 3.0
                    },
                    FlowWidthAtBottomProtection =
                    {
                        Mean = (RoundedDouble) 4.0,
                        StandardDeviation = (RoundedDouble) 5.0
                    },
                    CriticalOvertoppingDischarge =
                    {
                        Mean = (RoundedDouble) 6.0,
                        CoefficientOfVariation = (RoundedDouble) 7.0
                    },
                    WidthFlowApertures =
                    {
                        Mean = (RoundedDouble) 8.0,
                        StandardDeviation = (RoundedDouble) 9.0
                    },
                    StorageStructureArea =
                    {
                        Mean = (RoundedDouble) 10.0,
                        CoefficientOfVariation = (RoundedDouble) 11.0
                    },
                    AllowedLevelIncreaseStorage =
                    {
                        Mean = (RoundedDouble) 12.0,
                        StandardDeviation = (RoundedDouble) 13.0
                    },
                    InflowModelType = ClosingStructureInflowModelType.FloodedCulvert,
                    AreaFlowApertures =
                    {
                        Mean = (RoundedDouble) 14.0,
                        StandardDeviation = (RoundedDouble) 15.0
                    },
                    FailureProbabilityOpenStructure = 0.16,
                    FailureProbabilityReparation = 0.17,
                    IdenticalApertures = 18,
                    InsideWaterLevel =
                    {
                        Mean = (RoundedDouble) 19.0,
                        StandardDeviation = (RoundedDouble) 20.0
                    },
                    ProbabilityOrFrequencyOpenStructureBeforeFlooding = 0.21,
                    ThresholdHeightOpenWeir =
                    {
                        Mean = (RoundedDouble) 22.0,
                        StandardDeviation = (RoundedDouble) 23.0
                    }
                });

            structure.CopyProperties(structureToUpdateFrom);
        }

        private static void AssertClosingStructuresInput(TestClosingStructure structure, ClosingStructuresInput inputParameters)
        {
            Assert.AreSame(structure, inputParameters.Structure);
            Assert.AreEqual(structure.StructureNormalOrientation, inputParameters.StructureNormalOrientation);
            Assert.AreEqual(structure.LevelCrestStructureNotClosing, inputParameters.LevelCrestStructureNotClosing);
            Assert.AreEqual(structure.FlowWidthAtBottomProtection, inputParameters.FlowWidthAtBottomProtection);
            Assert.AreEqual(structure.CriticalOvertoppingDischarge, inputParameters.CriticalOvertoppingDischarge);
            Assert.AreEqual(structure.WidthFlowApertures, inputParameters.WidthFlowApertures);
            Assert.AreEqual(structure.StorageStructureArea, inputParameters.StorageStructureArea);
            Assert.AreEqual(structure.AllowedLevelIncreaseStorage, inputParameters.AllowedLevelIncreaseStorage);
            Assert.AreEqual(structure.InflowModelType, inputParameters.InflowModelType);
            Assert.AreEqual(structure.AreaFlowApertures, inputParameters.AreaFlowApertures);
            Assert.AreEqual(structure.FailureProbabilityOpenStructure, inputParameters.FailureProbabilityOpenStructure);
            Assert.AreEqual(structure.FailureProbabilityReparation, inputParameters.FailureProbabilityReparation);
            Assert.AreEqual(structure.IdenticalApertures, inputParameters.IdenticalApertures);
            Assert.AreEqual(structure.InsideWaterLevel, inputParameters.InsideWaterLevel);
            Assert.AreEqual(structure.ProbabilityOrFrequencyOpenStructureBeforeFlooding, inputParameters.ProbabilityOrFrequencyOpenStructureBeforeFlooding);
            Assert.AreEqual(structure.ThresholdHeightOpenWeir, inputParameters.ThresholdHeightOpenWeir);
        }
    }
}