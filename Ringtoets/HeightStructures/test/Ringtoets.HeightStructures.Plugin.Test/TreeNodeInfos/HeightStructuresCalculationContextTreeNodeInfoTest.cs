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
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Data.TestUtil;
using Ringtoets.HeightStructures.Forms.PresentationObjects;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input.Structures;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.HeightStructures.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class HeightStructuresCalculationContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuDuplicateIndex = 2;
        private const int contextMenuUpdateForeshoreProfileIndex = 5;
        private const int contextMenuUpdateStructureIndex = 6;
        private const int contextMenuValidateIndex = 8;
        private const int contextMenuCalculateIndex = 9;
        private const int contextMenuClearIndex = 11;
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "HydraulicBoundaryDatabaseImporter");

        private MockRepository mocks;
        private HeightStructuresPlugin plugin;
        private TreeNodeInfo info;

        public override void Setup()
        {
            mocks = new MockRepository();
            plugin = new HeightStructuresPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(HeightStructuresCalculationContext));
        }

        public override void TearDown()
        {
            plugin.Dispose();
            mocks.VerifyAll();

            base.TearDown();
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
        [TestCase(true)]
        [TestCase(false)]
        public void ChildNodeObjects_Always_ReturnsChildrenOfData(bool hasOutput)
        {
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<HeightStructuresInput>
            {
                Output = hasOutput ? new TestStructuresOutput() : null
            };

            var calculationContext = new HeightStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(calculationContext).ToArray();

            // Assert
            Assert.AreEqual(3, children.Length);

            var comment = children[0] as Comment;
            Assert.AreSame(calculationContext.WrappedData.Comments, comment);

            var heightStructuresInputContext = children[1] as HeightStructuresInputContext;
            Assert.IsNotNull(heightStructuresInputContext);
            Assert.AreSame(calculationContext.WrappedData.InputParameters, heightStructuresInputContext.WrappedData);

            var structuresOutputContext = children[2] as StructuresOutputContext;
            Assert.IsNotNull(structuresOutputContext);
            Assert.AreSame(calculationContext.WrappedData, structuresOutputContext.WrappedData);
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<HeightStructuresInput>();
            var nodeData = new HeightStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);

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
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

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
            var failureMechanism = new TestHeightStructuresFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<HeightStructuresInput>();
            var nodeData = new HeightStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "random"
            });

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());

                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, assessmentSection, treeViewControl))
                {
                    // Assert
                    Assert.AreEqual(18, menu.Items.Count);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuDuplicateIndex,
                                                                  "D&upliceren",
                                                                  "Dupliceer dit element.",
                                                                  RingtoetsCommonFormsResources.CopyHS);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuUpdateForeshoreProfileIndex,
                                                                  "&Bijwerken voorlandprofiel...",
                                                                  "Er moet een voorlandprofiel geselecteerd zijn.",
                                                                  RingtoetsCommonFormsResources.UpdateItemIcon,
                                                                  false);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuUpdateStructureIndex,
                                                                  "&Bijwerken kunstwerk...",
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
        public void ContextMenuStrip_CalculationWithoutStructure_ContextMenuItemUpdateStructureDisabledAndToolTipSet()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mocks);

            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<HeightStructuresInput>();
            var failureMechanism = new TestHeightStructuresFailureMechanism();
            var nodeData = new HeightStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, assessmentSection, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                  contextMenuUpdateStructureIndex,
                                                                  "&Bijwerken kunstwerk...",
                                                                  "Er moet een kunstwerk geselecteerd zijn.",
                                                                  RingtoetsCommonFormsResources.UpdateItemIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationWithStructureAndInputInSync_ContextMenuItemUpdateStructureDisabledAndToolTipSet()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mocks);

            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestHeightStructure()
                }
            };
            var failureMechanism = new TestHeightStructuresFailureMechanism();
            var nodeData = new HeightStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, assessmentSection, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                  contextMenuUpdateStructureIndex,
                                                                  "&Bijwerken kunstwerk...",
                                                                  "Er zijn geen wijzigingen om bij te werken.",
                                                                  RingtoetsCommonFormsResources.UpdateItemIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationWithStructureAndInputOutOfSync_ContextMenuItemUpdateStructureEnabledAndToolTipSet()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mocks);

            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestHeightStructure()
                }
            };
            var failureMechanism = new TestHeightStructuresFailureMechanism();
            var nodeData = new HeightStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            ChangeStructure(calculation.InputParameters.Structure);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, assessmentSection, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                  contextMenuUpdateStructureIndex,
                                                                  "&Bijwerken kunstwerk...",
                                                                  "Berekening bijwerken met het kunstwerk.",
                                                                  RingtoetsCommonFormsResources.UpdateItemIcon);
                }
            }
        }

        [Test]
        public void GivenCalculationWithoutOutputAndWithInputOutOfSync_WhenUpdateStructureClicked_ThenNoInquiryAndCalculationUpdatedAndInputObserverNotified()
        {
            // Given
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var structure = new TestHeightStructure();
            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    Structure = structure
                }
            };
            var failureMechanism = new HeightStructuresFailureMechanism();
            var nodeData = new HeightStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);

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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var structure = new TestHeightStructure();
            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    Structure = structure
                },
                Output = new TestStructuresOutput()
            };

            var failureMechanism = new HeightStructuresFailureMechanism();
            var nodeData = new HeightStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);

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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var structure = new TestHeightStructure();
            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    Structure = structure
                },
                Output = new TestStructuresOutput()
            };
            var failureMechanism = new HeightStructuresFailureMechanism();
            var nodeData = new HeightStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);

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
        public void ContextMenuStrip_HydraulicBoundaryDatabaseNotCoupled_ContextMenuItemPerformCalculationDisabledAndTooltipSet()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mocks);
            var failureMechanism = new TestHeightStructuresFailureMechanism();
            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<HeightStructuresInput>();
            var nodeData = new HeightStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
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
        public void ContextMenuStrip_HydraulicBoundaryDatabaseCoupledToInvalidFile_ContextMenuItemPerformCalculationDisabledAndTooltipSet()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(null, mocks, "invalidFilePath");
            var failureMechanism = new TestHeightStructuresFailureMechanism();
            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<HeightStructuresInput>();
            var nodeData = new HeightStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
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
            var failureMechanism = new HeightStructuresFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<HeightStructuresInput>();
            var nodeData = new HeightStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);

            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "1.0"
            });

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
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
            var failureMechanism = new TestHeightStructuresFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<HeightStructuresInput>();
            var nodeData = new HeightStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);

            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "1.0"
            });

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
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
        public void ContextMenuStrip_HydraulicBoundaryDatabaseNotCoupled_ContextMenuItemValidateCalculationDisabledAndTooltipSet()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mocks);
            var failureMechanism = new TestHeightStructuresFailureMechanism();
            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<HeightStructuresInput>();
            var nodeData = new HeightStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
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
        public void ContextMenuStrip_HydraulicBoundaryDatabaseCoupledToInvalidFile_ContextMenuItemValidateCalculationDisabledAndTooltipSet()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(null, mocks, "invalidFilePath");
            var failureMechanism = new TestHeightStructuresFailureMechanism();
            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<HeightStructuresInput>();
            var nodeData = new HeightStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
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
            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<HeightStructuresInput>();
            var failureMechanism = new HeightStructuresFailureMechanism();
            var nodeData = new HeightStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);

            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "1.0"
            });

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
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
            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<HeightStructuresInput>();
            var failureMechanism = new TestHeightStructuresFailureMechanism();
            var nodeData = new HeightStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);

            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "1.0"
            });

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
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
        public void ContextMenuStrip_CalculationWithoutForeshoreProfile_ContextMenuItemUpdateForeshoreProfileDisabledAndToolTipSet()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mocks);
            var failureMechanism = new TestHeightStructuresFailureMechanism();
            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<HeightStructuresInput>();

            var nodeData = new HeightStructuresCalculationContext(calculation,
                                                                  parent,
                                                                  failureMechanism,
                                                                  assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
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
                        RingtoetsCommonFormsResources.UpdateItemIcon,
                        false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationWithForeshoreProfileAndInputInSync_ContextMenuItemUpdateForeshoreProfileDisabledAndToolTipSet()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mocks);
            var failureMechanism = new TestHeightStructuresFailureMechanism();
            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile()
                }
            };

            var nodeData = new HeightStructuresCalculationContext(calculation,
                                                                  parent,
                                                                  failureMechanism,
                                                                  assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
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
                        RingtoetsCommonFormsResources.UpdateItemIcon,
                        false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationWithForeshoreProfileAndInputOutSync_ContextMenuItemUpdateForeshoreProfileEnabledAndToolTipSet()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mocks);
            var failureMechanism = new TestHeightStructuresFailureMechanism();

            var foreshoreProfileInput = new TestForeshoreProfile();
            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    ForeshoreProfile = foreshoreProfileInput
                }
            };
            TestForeshoreProfile.ChangeBreakWaterProperties(foreshoreProfileInput);

            var nodeData = new HeightStructuresCalculationContext(calculation,
                                                                  parent,
                                                                  failureMechanism,
                                                                  assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
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
                        RingtoetsCommonFormsResources.UpdateItemIcon);
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

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanism = new HeightStructuresFailureMechanism();

            var foreshoreProfileInput = new TestForeshoreProfile(true);
            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    ForeshoreProfile = foreshoreProfileInput
                }
            };
            var nodeData = new HeightStructuresCalculationContext(calculation,
                                                                  parent,
                                                                  failureMechanism,
                                                                  assessmentSection);

            calculation.Attach(calculationObserver);
            calculation.InputParameters.Attach(calculationInputObserver);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
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
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mocks);
            var failureMechanism = new TestHeightStructuresFailureMechanism();

            var foreshoreProfileInput = new TestForeshoreProfile(true);
            var parent = new CalculationGroup();
            var calculation = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    ForeshoreProfile = foreshoreProfileInput
                },
                Output = new TestStructuresOutput()
            };

            var nodeData = new HeightStructuresCalculationContext(calculation,
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
                var gui = mocks.Stub<IGui>();
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
        public void GivenCalculationThatSucceeds_WhenCalculatingFromContextMenu_ThenOutputSetLogMessagesAddedAndUpdateObserver()
        {
            // Given
            var mainWindow = mocks.DynamicMock<IMainWindow>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var failureMechanism = new TestHeightStructuresFailureMechanism();
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.Id).Return(string.Empty);
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "random",
                Locations =
                {
                    hydraulicBoundaryLocation
                }
            });

            var initialOutput = new TestStructuresOutput();
            var parent = new CalculationGroup();
            var calculation = new TestHeightStructuresCalculation
            {
                Output = initialOutput,
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };
            calculation.Attach(observer);

            var calculationContext = new HeightStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(calculationContext, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mainWindow);

                var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresOvertoppingCalculationInput>(testDataPath, string.Empty))
                                 .Return(new TestStructuresCalculator<StructuresOvertoppingCalculationInput>());
                mocks.ReplayAll();

                plugin.Gui = gui;

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
                        StringAssert.StartsWith("Hoogte kunstwerk berekening is uitgevoerd op de tijdelijke locatie", msgs[4]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[5]);
                        Assert.AreEqual($"Uitvoeren van berekening '{calculation.Name}' is gelukt.", msgs[6]);
                    });

                    Assert.AreNotSame(initialOutput, calculation.Output);
                }
            }
        }

        [Test]
        public void GivenCalculationWithNonExistingFilePath_WhenValidatingFromContextMenu_ThenLogMessagesAdded()
        {
            // Given
            var observer = mocks.StrictMock<IObserver>();

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "random",
                Locations =
                {
                    hydraulicBoundaryLocation
                }
            });

            var parent = new CalculationGroup();
            var calculation = new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };
            calculation.Attach(observer);

            var failureMechanism = new TestHeightStructuresFailureMechanism();
            var calculationContext = new HeightStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(calculationContext, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

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
            var failureMechanism = new HeightStructuresFailureMechanism();
            var elementToBeRemoved = new StructuresCalculation<HeightStructuresInput>();
            var observer = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var calculationContext = new HeightStructuresCalculationContext(elementToBeRemoved,
                                                                            group,
                                                                            failureMechanism,
                                                                            assessmentSection);
            var groupContext = new HeightStructuresCalculationGroupContext(group,
                                                                           null,
                                                                           failureMechanism,
                                                                           assessmentSection);

            observer.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            group.Children.Add(elementToBeRemoved);
            group.Children.Add(new StructuresCalculation<HeightStructuresInput>());
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
            var failureMechanism = new HeightStructuresFailureMechanism();
            var elementToBeRemoved = new StructuresCalculation<HeightStructuresInput>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var calculationContext = new HeightStructuresCalculationContext(elementToBeRemoved,
                                                                            group,
                                                                            failureMechanism,
                                                                            assessmentSection);
            var groupContext = new HeightStructuresCalculationGroupContext(group,
                                                                           null,
                                                                           failureMechanism,
                                                                           assessmentSection);

            mocks.ReplayAll();

            group.Children.Add(elementToBeRemoved);

            failureMechanism.AddSection(new FailureMechanismSection("section", new[]
            {
                new Point2D(0, 0)
            }));

            HeightStructuresFailureMechanismSectionResult result = failureMechanism.SectionResults.First();
            result.Calculation = elementToBeRemoved;

            // Call
            info.OnNodeRemoved(calculationContext, groupContext);

            // Assert
            Assert.IsNull(result.Calculation);
        }

        private static void ChangeStructure(HeightStructure structure)
        {
            structure.CopyProperties(new HeightStructure(
                                         new HeightStructure.ConstructionProperties
                                         {
                                             Id = structure.Id,
                                             Name = structure.Name,
                                             Location = structure.Location
                                         }));
        }
    }
}