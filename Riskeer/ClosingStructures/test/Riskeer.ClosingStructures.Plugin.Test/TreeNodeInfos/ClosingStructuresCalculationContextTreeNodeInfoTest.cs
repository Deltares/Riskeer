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

using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.ClosingStructures.Data;
using Riskeer.ClosingStructures.Data.TestUtil;
using Riskeer.ClosingStructures.Forms.PresentationObjects;
using Riskeer.Common.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Service.TestUtil;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Data.Input.Structures;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.ClosingStructures.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class ClosingStructuresCalculationContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuDuplicateIndex = 2;
        private const int contextMenuUpdateForeshoreProfileIndex = 5;
        private const int contextMenuUpdateStructureIndex = 6;
        private const int contextMenuValidateIndex = 8;
        private const int contextMenuCalculateIndex = 9;
        private const int contextMenuClearIndex = 11;
        private const int contextMenuClearIllustrationPointsIndex = 12;
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, nameof(HydraulicBoundaryDatabase));

        private IGui gui;
        private TreeNodeInfo info;
        private MockRepository mocks;
        private ClosingStructuresPlugin plugin;

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
            Assert.IsNull(info.CheckedState);
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
            TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.CalculationIcon, image);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ChildNodeObjects_Always_ReturnsChildrenOfData(bool hasOutput)
        {
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                Output = hasOutput ? new TestStructuresOutput() : null
            };

            var calculationContext = new ClosingStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(calculationContext).ToArray();

            // Assert
            Assert.AreEqual(3, children.Length);

            var comment = children[0] as Comment;
            Assert.AreSame(calculationContext.WrappedData.Comments, comment);

            var closingStructuresInputContext = children[1] as ClosingStructuresInputContext;
            Assert.IsNotNull(closingStructuresInputContext);
            Assert.AreSame(calculationContext.WrappedData.InputParameters, closingStructuresInputContext.WrappedData);

            var structuresOutputContext = children[2] as ClosingStructuresOutputContext;
            Assert.IsNotNull(structuresOutputContext);
            Assert.AreSame(calculationContext.WrappedData, structuresOutputContext.WrappedData);
            Assert.AreSame(calculationContext.FailureMechanism, structuresOutputContext.FailureMechanism);
            Assert.AreSame(calculationContext.AssessmentSection, structuresOutputContext.AssessmentSection);
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var assessmentSection = new AssessmentSectionStub();
            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var nodeData = new ClosingStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);

            var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();
            using (mocks.Ordered())
            {
                menuBuilder.Expect(mb => mb.AddExportItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddRenameItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddDeleteItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.Build()).Return(null);
            }

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
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

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "random"
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(hydraulicBoundaryDatabase);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase);

            var failureMechanism = new TestClosingStructuresFailureMechanism();
            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var nodeData = new ClosingStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);
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
                    Assert.AreEqual(19, menu.Items.Count);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuDuplicateIndex,
                                                                  "D&upliceren",
                                                                  "Dupliceer dit element.",
                                                                  RiskeerCommonFormsResources.CopyHS);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuUpdateForeshoreProfileIndex,
                                                                  "&Bijwerken voorlandprofiel...",
                                                                  "Er moet een voorlandprofiel geselecteerd zijn.",
                                                                  RiskeerCommonFormsResources.UpdateItemIcon,
                                                                  false);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuUpdateStructureIndex,
                                                                  "&Bijwerken kunstwerk...",
                                                                  "Er moet een kunstwerk geselecteerd zijn.",
                                                                  RiskeerCommonFormsResources.UpdateItemIcon,
                                                                  false);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateIndex,
                                                                  "&Valideren",
                                                                  "Valideer de invoer voor deze berekening.",
                                                                  RiskeerCommonFormsResources.ValidateIcon);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateIndex,
                                                                  "Be&rekenen",
                                                                  "Voer deze berekening uit.",
                                                                  RiskeerCommonFormsResources.CalculateIcon);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearIndex,
                                                                  "&Wis uitvoer...",
                                                                  "Deze berekening heeft geen uitvoer om te wissen.",
                                                                  RiskeerCommonFormsResources.ClearIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearIllustrationPointsIndex,
                                                                  "Wis illustratiepunten...",
                                                                  "Deze berekening heeft geen illustratiepunten om te wissen.",
                                                                  RiskeerCommonFormsResources.ClearIllustrationPointsIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationWithoutStructure_ContextMenuItemUpdateStructureDisabledAndToolTipSet()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);

            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var failureMechanism = new TestClosingStructuresFailureMechanism();
            var nodeData = new ClosingStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);
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
                                                                  "&Bijwerken kunstwerk...",
                                                                  "Er moet een kunstwerk geselecteerd zijn.",
                                                                  RiskeerCommonFormsResources.UpdateItemIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationWithStructureAndInputInSync_ContextMenuItemUpdateStructureDisabledAndToolTipSet()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);

            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestClosingStructure()
                }
            };
            var failureMechanism = new TestClosingStructuresFailureMechanism();
            var nodeData = new ClosingStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);
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
                                                                  "&Bijwerken kunstwerk...",
                                                                  "Er zijn geen wijzigingen om bij te werken.",
                                                                  RiskeerCommonFormsResources.UpdateItemIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationWithStructureAndInputOutOfSync_ContextMenuItemUpdateStructureEnabledAndToolTipSet()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);

            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestClosingStructure()
                }
            };
            var failureMechanism = new TestClosingStructuresFailureMechanism();
            var nodeData = new ClosingStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            ChangeStructure(calculation.InputParameters.Structure);

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
                                                                  "&Bijwerken kunstwerk...",
                                                                  "Berekening bijwerken met het kunstwerk.",
                                                                  RiskeerCommonFormsResources.UpdateItemIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationWithIllustrationPoints_ContextMenuItemClearIllustrationPointsDisabled()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);

            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                Output = new TestStructuresOutput(new TestGeneralResultFaultTreeIllustrationPoint())
            };
            var failureMechanism = new TestClosingStructuresFailureMechanism();
            var nodeData = new ClosingStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, assessmentSection, treeViewControl))
                {
                    // Call
                    ToolStripItem contextMenuItem = menu.Items[contextMenuClearIllustrationPointsIndex];

                    // Assert
                    Assert.IsTrue(contextMenuItem.Enabled);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationWithOutputWithoutIllustrationPoints_ContextMenuItemClearIllustrationPointsDisabled()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);

            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                Output = new TestStructuresOutput()
            };
            var failureMechanism = new TestClosingStructuresFailureMechanism();
            var nodeData = new ClosingStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, assessmentSection, treeViewControl))
                {
                    // Call
                    ToolStripItem contextMenuItem = menu.Items[contextMenuClearIllustrationPointsIndex];

                    // Assert
                    Assert.IsFalse(contextMenuItem.Enabled);
                }
            }
        }

        [Test]
        public void GivenCalculationWithoutOutputAndWithInputOutOfSync_WhenUpdateStructureClicked_ThenNoInquiryAndCalculationUpdatedAndInputObserverNotified()
        {
            // Given
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);
            var structure = new TestClosingStructure();
            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = structure
                }
            };
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var nodeData = new ClosingStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);

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

                ChangeStructure(structure);

                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, assessmentSection, treeViewControl))
                {
                    // When
                    menu.Items[contextMenuUpdateStructureIndex].PerformClick();

                    // Then
                    Assert.IsTrue(calculation.InputParameters.IsStructureInputSynchronized);

                    // Note: observer assertions are verified in the TearDown()
                }
            }
        }

        [Test]
        public void GivenCalculationWithOutputAndInputOutOfSync_WhenUpdateStructureClickedAndCancelled_ThenInquiryAndCalculationNotUpdatedAndObserversNotNotified()
        {
            // Given
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);
            var structure = new TestClosingStructure();
            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = structure
                },
                Output = new TestStructuresOutput()
            };

            ClosingStructuresInput calculationInput = calculation.InputParameters;
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var nodeData = new ClosingStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);

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

                ChangeStructure(structure);

                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, assessmentSection, treeViewControl))
                {
                    // When
                    menu.Items[contextMenuUpdateStructureIndex].PerformClick();

                    // Then
                    Assert.IsTrue(calculation.HasOutput);
                    Assert.IsFalse(calculation.InputParameters.IsStructureInputSynchronized);

                    string expectedMessage = "Als u kiest voor bijwerken, dan wordt het resultaat van deze berekening " +
                                             $"verwijderd.{Environment.NewLine}{Environment.NewLine}Weet u zeker dat u wilt doorgaan?";
                    Assert.AreEqual(expectedMessage, textBoxMessage);

                    // Note: observer assertions are verified in the TearDown()
                }
            }
        }

        [Test]
        public void GivenCalculationWithOutputAndInputOutOfSync_WhenUpdateStructureClickedAndContinued_ThenInquiryAndCalculationUpdatedAndObserversNotified()
        {
            // Given
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);
            var structure = new TestClosingStructure();
            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = structure
                },
                Output = new TestStructuresOutput()
            };
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var nodeData = new ClosingStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);

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

                ChangeStructure(structure);

                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, assessmentSection, treeViewControl))
                {
                    // When
                    menu.Items[contextMenuUpdateStructureIndex].PerformClick();

                    // Then
                    Assert.IsFalse(calculation.HasOutput);
                    Assert.IsTrue(calculation.InputParameters.IsStructureInputSynchronized);

                    string expectedMessage = "Als u kiest voor bijwerken, dan wordt het resultaat van deze berekening " +
                                             $"verwijderd.{Environment.NewLine}{Environment.NewLine}Weet u zeker dat u wilt doorgaan?";
                    Assert.AreEqual(expectedMessage, textBoxMessage);

                    // Note: observer assertions are verified in the TearDown()
                }
            }
        }

        [Test]
        public void ContextMenuStrip_HydraulicBoundaryDatabaseNotLinked_ContextMenuItemPerformCalculationAndValidationDisabledAndTooltipSet()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);
            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var failureMechanism = new TestClosingStructuresFailureMechanism();
            var nodeData = new ClosingStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);

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
                                                                  "Er is geen hydraulische belastingendatabase geïmporteerd.",
                                                                  RiskeerCommonFormsResources.CalculateIcon,
                                                                  false);

                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuValidateIndex,
                                                                  "&Valideren",
                                                                  "Er is geen hydraulische belastingendatabase geïmporteerd.",
                                                                  RiskeerCommonFormsResources.ValidateIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_HydraulicBoundaryDatabaseLinkedToInvalidFile_ContextMenuItemPerformCalculationAndValidationDisabledAndTooltipSet()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(null, mocks, "invalidFilePath");
            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var failureMechanism = new TestClosingStructuresFailureMechanism();
            var nodeData = new ClosingStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);

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
                    StringAssert.Contains("Herstellen van de verbinding met de hydraulische belastingendatabase is mislukt.", calculateContextMenuItem.ToolTipText);
                    TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.CalculateIcon, calculateContextMenuItem.Image);
                    Assert.IsFalse(calculateContextMenuItem.Enabled);

                    ToolStripItem validateContextMenuItem = contextMenu.Items[contextMenuValidateIndex];

                    Assert.AreEqual("&Valideren", validateContextMenuItem.Text);
                    StringAssert.Contains("Herstellen van de verbinding met de hydraulische belastingendatabase is mislukt.", validateContextMenuItem.ToolTipText);
                    TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.ValidateIcon, validateContextMenuItem.Image);
                    Assert.IsFalse(validateContextMenuItem.Enabled);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_AllRequiredInputSet_ContextMenuItemPerformCalculationAndValidationEnabled()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "1.0"
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(hydraulicBoundaryDatabase);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase);

            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();
            var failureMechanism = new TestClosingStructuresFailureMechanism();
            var nodeData = new ClosingStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);

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
                                                                  RiskeerCommonFormsResources.CalculateIcon);

                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuValidateIndex,
                                                                  "&Valideren",
                                                                  "Valideer de invoer voor deze berekening.",
                                                                  RiskeerCommonFormsResources.ValidateIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationWithoutForeshoreProfile_ContextMenuItemUpdateForeshoreProfileDisabledAndToolTipSet()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);
            var failureMechanism = new TestClosingStructuresFailureMechanism();
            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<ClosingStructuresInput>();

            var nodeData = new ClosingStructuresCalculationContext(calculation,
                                                                   parent,
                                                                   failureMechanism,
                                                                   assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(
                        menu,
                        contextMenuUpdateForeshoreProfileIndex,
                        "&Bijwerken voorlandprofiel...",
                        "Er moet een voorlandprofiel geselecteerd zijn.",
                        RiskeerCommonFormsResources.UpdateItemIcon,
                        false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationWithForeshoreProfileAndInputInSync_ContextMenuItemUpdateForeshoreProfileDisabledAndToolTipSet()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);
            var failureMechanism = new TestClosingStructuresFailureMechanism();
            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile()
                }
            };

            var nodeData = new ClosingStructuresCalculationContext(calculation,
                                                                   parent,
                                                                   failureMechanism,
                                                                   assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(
                        menu,
                        contextMenuUpdateForeshoreProfileIndex,
                        "&Bijwerken voorlandprofiel...",
                        "Er zijn geen wijzigingen om bij te werken.",
                        RiskeerCommonFormsResources.UpdateItemIcon,
                        false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationWithForeshoreProfileAndInputOutSync_ContextMenuItemUpdateForeshoreProfileEnabledAndToolTipSet()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);
            var failureMechanism = new TestClosingStructuresFailureMechanism();

            var foreshoreProfileInput = new TestForeshoreProfile();
            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    ForeshoreProfile = foreshoreProfileInput
                }
            };
            TestForeshoreProfile.ChangeBreakWaterProperties(foreshoreProfileInput);

            var nodeData = new ClosingStructuresCalculationContext(calculation,
                                                                   parent,
                                                                   failureMechanism,
                                                                   assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(
                        menu,
                        contextMenuUpdateForeshoreProfileIndex,
                        "&Bijwerken voorlandprofiel...",
                        "Berekening bijwerken met het voorlandprofiel.",
                        RiskeerCommonFormsResources.UpdateItemIcon);
                }
            }
        }

        [Test]
        public void GivenCalculationWithoutOutputAndWithInputOutOfSync_WhenUpdateForeshoreProfileClicked_ThenNoInquiryAndCalculationUpdatedAndInputObserverNotified()
        {
            // Given
            var calculationObserver = mocks.StrictMock<IObserver>();
            var calculationInputObserver = mocks.StrictMock<IObserver>();
            calculationInputObserver.Expect(o => o.UpdateObserver());

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);
            var failureMechanism = new TestClosingStructuresFailureMechanism();

            var foreshoreProfileInput = new TestForeshoreProfile(true);
            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    ForeshoreProfile = foreshoreProfileInput
                }
            };
            var nodeData = new ClosingStructuresCalculationContext(calculation,
                                                                   parent,
                                                                   failureMechanism,
                                                                   assessmentSection);

            calculation.Attach(calculationObserver);
            calculation.InputParameters.Attach(calculationInputObserver);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                TestForeshoreProfile.ChangeBreakWaterProperties(foreshoreProfileInput);

                // Precondition
                Assert.IsFalse(calculation.InputParameters.IsForeshoreProfileInputSynchronized);

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    contextMenuStrip.Items[contextMenuUpdateForeshoreProfileIndex].PerformClick();

                    // Then
                    Assert.IsTrue(calculation.InputParameters.IsForeshoreProfileInputSynchronized);
                }
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenCalculationWithOutputAndWithInputOutOfSync_WhenPerformClick_ThenInquiryAndExpectedOutputAndNotifications(bool continuation)
        {
            // Given
            var calculationObserver = mocks.StrictMock<IObserver>();
            var calculationInputObserver = mocks.StrictMock<IObserver>();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);
            var failureMechanism = new TestClosingStructuresFailureMechanism();

            var foreshoreProfileInput = new TestForeshoreProfile(true);
            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    ForeshoreProfile = foreshoreProfileInput
                },
                Output = new TestStructuresOutput()
            };

            var nodeData = new ClosingStructuresCalculationContext(calculation,
                                                                   parent,
                                                                   failureMechanism,
                                                                   assessmentSection);

            calculation.Attach(calculationObserver);
            calculation.InputParameters.Attach(calculationInputObserver);

            if (continuation)
            {
                calculationObserver.Expect(o => o.UpdateObserver());
                calculationInputObserver.Expect(o => o.UpdateObserver());
            }

            var messageBoxText = "";
            DialogBoxHandler = (name, wnd) =>
            {
                var helper = new MessageBoxTester(wnd);
                messageBoxText = helper.Text;

                if (continuation)
                {
                    helper.ClickOk();
                }
                else
                {
                    helper.ClickCancel();
                }
            };

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                TestForeshoreProfile.ChangeBreakWaterProperties(foreshoreProfileInput);

                // Precondition
                Assert.IsFalse(calculation.InputParameters.IsForeshoreProfileInputSynchronized);

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    contextMenuStrip.Items[contextMenuUpdateForeshoreProfileIndex].PerformClick();

                    // Then
                    Assert.AreEqual(continuation, calculation.InputParameters.IsForeshoreProfileInputSynchronized);
                    Assert.AreEqual(!continuation, calculation.HasOutput);
                }
            }

            string expectedMessageBoxText = "Als u kiest voor bijwerken, dan wordt het resultaat van deze berekening " +
                                            $"verwijderd.{Environment.NewLine}{Environment.NewLine}Weet u zeker dat u wilt doorgaan?";

            Assert.AreEqual(expectedMessageBoxText, messageBoxText);
        }

        [Test]
        public void GivenValidCalculation_WhenCalculatingFromContextMenu_ThenOutputSetLogMessagesAddedAndUpdateObserver()
        {
            // Given
            var mainWindow = mocks.Stub<IMainWindow>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var failureMechanism = new TestClosingStructuresFailureMechanism();

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = Path.Combine(testDataPath, "complete.sqlite"),
                Version = "random",
                Locations =
                {
                    hydraulicBoundaryLocation
                }
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(hydraulicBoundaryDatabase);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.Id).Return(string.Empty);
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase);

            var parent = new CalculationGroup();
            var calculation = new TestClosingStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var calculationContext = new ClosingStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(calculationContext, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mainWindow);

                var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresClosureCalculationInput>(
                                             Arg<HydraRingCalculationSettings>.Is.NotNull))
                                 .WhenCalled(invocation =>
                                 {
                                     HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                         HydraulicBoundaryCalculationSettingsFactory.CreateSettings(hydraulicBoundaryDatabase),
                                         (HydraRingCalculationSettings) invocation.Arguments[0]);
                                 })
                                 .Return(new TestStructuresCalculator<StructuresClosureCalculationInput>());
                mocks.ReplayAll();

                calculation.Attach(observer);

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
                        Assert.AreEqual(7, msgs.Length);
                        Assert.AreEqual($"Uitvoeren van berekening '{calculation.Name}' is gestart.", msgs[0]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                        CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[3]);
                        StringAssert.StartsWith("Betrouwbaarheid sluiting kunstwerk berekening is uitgevoerd op de tijdelijke locatie", msgs[4]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[5]);
                        Assert.AreEqual($"Uitvoeren van berekening '{calculation.Name}' is gelukt.", msgs[6]);
                    });

                    Assert.IsNotNull(calculation.Output);
                }
            }
        }

        [Test]
        public void GivenCalculationWithNonExistingFilePath_WhenValidatingFromContextMenu_ThenLogMessagesAdded()
        {
            // Given
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "random",
                Locations =
                {
                    hydraulicBoundaryLocation
                }
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(hydraulicBoundaryDatabase);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase);

            var parent = new CalculationGroup();
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
            var calculationContext = new ClosingStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);

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
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs[1]);
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
                                                                             group,
                                                                             failureMechanism,
                                                                             assessmentSection);
            var groupContext = new ClosingStructuresCalculationGroupContext(group,
                                                                            null,
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
                                                                             group,
                                                                             failureMechanism,
                                                                             assessmentSection);
            var groupContext = new ClosingStructuresCalculationGroupContext(group,
                                                                            null,
                                                                            failureMechanism,
                                                                            assessmentSection);

            mocks.ReplayAll();

            group.Children.Add(elementToBeRemoved);

            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });

            ClosingStructuresFailureMechanismSectionResult result = failureMechanism.SectionResults.First();
            result.Calculation = elementToBeRemoved;

            // Call
            info.OnNodeRemoved(calculationContext, groupContext);

            // Assert
            Assert.IsNull(result.Calculation);
        }

        [Test]
        public void GivenCalculationWithIllustrationPoints_WhenClearIllustrationPointsClickedAndAborted_ThenInquiryAndIllustrationPointsNotCleared()
        {
            // Given
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);

            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                Output = new TestStructuresOutput(new TestGeneralResultFaultTreeIllustrationPoint())
            };
            var failureMechanism = new TestClosingStructuresFailureMechanism();
            var nodeData = new ClosingStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var calculationObserver = mocks.StrictMock<IObserver>();
            calculation.Attach(calculationObserver);

            var messageBoxText = "";
            DialogBoxHandler = (name, wnd) =>
            {
                var helper = new MessageBoxTester(wnd);
                messageBoxText = helper.Text;

                helper.ClickCancel();
            };

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, assessmentSection, treeViewControl))
                {
                    // When
                    menu.Items[contextMenuClearIllustrationPointsIndex].PerformClick();

                    // Then
                    Assert.AreEqual("Weet u zeker dat u de illustratiepunten van deze berekening wilt wissen?", messageBoxText);
                    Assert.IsTrue(calculation.Output.HasGeneralResult);
                }
            }
        }

        [Test]
        public void GivenCalculationWithIllustrationPoints_WhenClearIllustrationPointsClickedAndContinued_ThenInquiryAndIllustrationPointsCleared()
        {
            // Given
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);

            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                Output = new TestStructuresOutput(new TestGeneralResultFaultTreeIllustrationPoint())
            };
            var failureMechanism = new TestClosingStructuresFailureMechanism();
            var nodeData = new ClosingStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var calculationObserver = mocks.StrictMock<IObserver>();
            calculationObserver.Expect(o => o.UpdateObserver());
            calculation.Attach(calculationObserver);

            var messageBoxText = "";
            DialogBoxHandler = (name, wnd) =>
            {
                var helper = new MessageBoxTester(wnd);
                messageBoxText = helper.Text;

                helper.ClickOk();
            };

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, assessmentSection, treeViewControl))
                {
                    // When
                    menu.Items[contextMenuClearIllustrationPointsIndex].PerformClick();

                    // Then
                    Assert.AreEqual("Weet u zeker dat u de illustratiepunten van deze berekening wilt wissen?", messageBoxText);
                    Assert.IsFalse(calculation.Output.HasGeneralResult);
                }
            }
        }

        public override void Setup()
        {
            mocks = new MockRepository();
            gui = mocks.Stub<IGui>();
            plugin = new ClosingStructuresPlugin
            {
                Gui = gui
            };

            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(ClosingStructuresCalculationContext));
        }

        public override void TearDown()
        {
            plugin.Dispose();
            mocks.VerifyAll();
        }

        private static void ChangeStructure(ClosingStructure structure)
        {
            structure.CopyProperties(new ClosingStructure(
                                         new ClosingStructure.ConstructionProperties
                                         {
                                             Id = structure.Id,
                                             Name = structure.Name,
                                             Location = structure.Location
                                         }));
        }
    }
}