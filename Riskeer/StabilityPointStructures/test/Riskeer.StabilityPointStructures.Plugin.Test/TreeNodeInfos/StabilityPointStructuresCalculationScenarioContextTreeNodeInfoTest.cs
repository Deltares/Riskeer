// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
using Core.Common.TestUtil;
using Core.Gui;
using Core.Gui.ContextMenu;
using Core.Gui.Forms.Main;
using Core.Gui.TestUtil;
using Core.Gui.TestUtil.ContextMenu;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using NSubstitute;
using Riskeer.Common.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Service.TestUtil;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Data.Input.Structures;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityPointStructures.Data.TestUtil;
using Riskeer.StabilityPointStructures.Forms.PresentationObjects;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.StabilityPointStructures.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class StabilityPointStructuresCalculationScenarioContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuDuplicateIndex = 2;
        private const int contextMenuUpdateForeshoreProfileIndex = 5;
        private const int contextMenuUpdateStructureIndex = 6;
        private const int contextMenuValidateIndex = 8;
        private const int contextMenuCalculateIndex = 9;
        private const int contextMenuClearIndex = 11;
        private const int contextMenuClearIllustrationPointsIndex = 12;

        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, nameof(HydraulicBoundaryData));
        private static readonly string validHlcdFilePath = Path.Combine(testDataPath, "hlcd.sqlite");
        private static readonly string validHrdFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
        private static readonly string validHrdFileVersion = "Dutch coast South19-11-2015 12:0013";

        private StabilityPointStructuresPlugin plugin;
        private TreeNodeInfo info;

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup

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

            // Call
            Image image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.ProbabilisticCalculationIcon, image);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ChildNodeObjects_Always_ReturnsChildrenOfData(bool hasOutput)
        {
            var assessmentSection = Substitute.For<IAssessmentSection>();

            var parent = new CalculationGroup();
            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>
            {
                Output = hasOutput ? new TestStructuresOutput() : null
            };
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculationContext = new StabilityPointStructuresCalculationScenarioContext(calculation, parent, failureMechanism, assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(calculationContext).ToArray();

            // Assert
            Assert.AreEqual(3, children.Length);

            var comment = children[0] as Comment;
            Assert.AreSame(calculationContext.WrappedData.Comments, comment);

            var stabilityPointStructuresInputContext = children[1] as StabilityPointStructuresInputContext;
            Assert.IsNotNull(stabilityPointStructuresInputContext);
            Assert.AreSame(calculationContext.WrappedData.InputParameters, stabilityPointStructuresInputContext.WrappedData);

            var structuresOutputContext = children[2] as StructuresOutputContext;
            Assert.IsNotNull(structuresOutputContext);
            Assert.AreSame(calculationContext.WrappedData, structuresOutputContext.WrappedData);
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var assessmentSection = new AssessmentSectionStub();
            var parent = new CalculationGroup();
            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>();
            var nodeData = new StabilityPointStructuresCalculationScenarioContext(calculation, parent, failureMechanism, assessmentSection);

            var menuBuilder = Substitute.For<IContextMenuBuilder>();
            {
                menuBuilder.AddExportItem().Returns(menuBuilder);
                menuBuilder.AddSeparator().Returns(menuBuilder);
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>()).Returns(menuBuilder);
                menuBuilder.AddSeparator().Returns(menuBuilder);
                menuBuilder.AddRenameItem().Returns(menuBuilder);
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>()).Returns(menuBuilder);
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>()).Returns(menuBuilder);
                menuBuilder.AddSeparator().Returns(menuBuilder);
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>()).Returns(menuBuilder);
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>()).Returns(menuBuilder);
                menuBuilder.AddSeparator().Returns(menuBuilder);
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>()).Returns(menuBuilder);
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>()).Returns(menuBuilder);
                menuBuilder.AddDeleteItem().Returns(menuBuilder);
                menuBuilder.AddSeparator().Returns(menuBuilder);
                menuBuilder.AddCollapseAllItem().Returns(menuBuilder);
                menuBuilder.AddExpandAllItem().Returns(menuBuilder);
                menuBuilder.AddSeparator().Returns(menuBuilder);
                menuBuilder.AddPropertiesItem().Returns(menuBuilder);
                menuBuilder.Build().Returns((ContextMenuStrip) null);
            }

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = Substitute.For<IGui>();
                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

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
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var parent = new CalculationGroup();
            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>();
            var nodeData = new StabilityPointStructuresCalculationScenarioContext(calculation, parent, failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = Substitute.For<IGui>();
                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                plugin.Gui = gui;

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
                                                                  "Bijwerken &kunstwerk...",
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
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();

            var parent = new CalculationGroup();
            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var nodeData = new StabilityPointStructuresCalculationScenarioContext(calculation, parent, failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = Substitute.For<IGui>();
                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, assessmentSection, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                  contextMenuUpdateStructureIndex,
                                                                  "Bijwerken &kunstwerk...",
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
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();

            var parent = new CalculationGroup();
            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestStabilityPointStructure()
                }
            };
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var nodeData = new StabilityPointStructuresCalculationScenarioContext(calculation, parent, failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = Substitute.For<IGui>();
                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, assessmentSection, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                  contextMenuUpdateStructureIndex,
                                                                  "Bijwerken &kunstwerk...",
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
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();

            var parent = new CalculationGroup();
            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestStabilityPointStructure()
                }
            };
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var nodeData = new StabilityPointStructuresCalculationScenarioContext(calculation, parent, failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            ChangeStructure(calculation.InputParameters.Structure);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = Substitute.For<IGui>();
                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, assessmentSection, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                  contextMenuUpdateStructureIndex,
                                                                  "Bijwerken &kunstwerk...",
                                                                  "Berekening bijwerken met het kunstwerk.",
                                                                  RiskeerCommonFormsResources.UpdateItemIcon);
                }
            }
        }

        [Test]
        public void GivenCalculationWithoutOutputAndWithInputOutOfSync_WhenUpdateStructureClicked_ThenNoInquiryAndCalculationUpdatedAndInputObserverNotified()
        {
            // Given
            var calculationObserver = Substitute.For<IObserver>();
            var calculationInputObserver = Substitute.For<IObserver>();

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var parent = new CalculationGroup();
            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestStabilityPointStructure()
                }
            };
            var nodeData = new StabilityPointStructuresCalculationScenarioContext(calculation,
                                                                                  parent,
                                                                                  failureMechanism,
                                                                                  assessmentSection);

            calculation.Attach(calculationObserver);
            calculation.InputParameters.Attach(calculationInputObserver);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = Substitute.For<IGui>();
                gui.Get(nodeData, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                plugin.Gui = gui;

                ChangeStructure(calculation.InputParameters.Structure);

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    contextMenuStrip.Items[contextMenuUpdateStructureIndex].PerformClick();

                    // Then
                    Assert.IsTrue(calculation.InputParameters.IsStructureInputSynchronized);
                }
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenCalculationWithOutputAndWithInputOutOfSync_WhenUpdateStructureClicked_ThenInquiryAndExpectedOutputAndNotifications(bool continuation)
        {
            // Given
            var calculationObserver = Substitute.For<IObserver>();
            var calculationInputObserver = Substitute.For<IObserver>();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var parent = new CalculationGroup();
            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestStabilityPointStructure()
                },
                Output = new TestStructuresOutput()
            };

            var nodeData = new StabilityPointStructuresCalculationScenarioContext(calculation,
                                                                                  parent,
                                                                                  failureMechanism,
                                                                                  assessmentSection);

            calculation.Attach(calculationObserver);
            calculation.InputParameters.Attach(calculationInputObserver);

            if (continuation) {}

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
                var gui = Substitute.For<IGui>();
                gui.Get(nodeData, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                plugin.Gui = gui;

                ChangeStructure(calculation.InputParameters.Structure);

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    contextMenuStrip.Items[contextMenuUpdateStructureIndex].PerformClick();

                    // Then
                    Assert.AreEqual(continuation, calculation.InputParameters.IsStructureInputSynchronized);
                    Assert.AreEqual(!continuation, calculation.HasOutput);
                }
            }

            string expectedMessageBoxText = "Als u kiest voor bijwerken, dan wordt het resultaat van deze berekening " +
                                            $"verwijderd.{Environment.NewLine}{Environment.NewLine}Weet u zeker dat u wilt doorgaan?";

            Assert.AreEqual(expectedMessageBoxText, messageBoxText);
        }

        [Test]
        public void ContextMenuStrip_AllRequiredInputSet_ContextMenuItemPerformCalculationAndValidationEnabled()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var parent = new CalculationGroup();
            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>();
            var nodeData = new StabilityPointStructuresCalculationScenarioContext(calculation, parent, failureMechanism, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = Substitute.For<IGui>();
                gui.Get(nodeData, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                plugin.Gui = gui;

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
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var parent = new CalculationGroup();
            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>();

            var nodeData = new StabilityPointStructuresCalculationScenarioContext(calculation,
                                                                                  parent,
                                                                                  failureMechanism,
                                                                                  assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = Substitute.For<IGui>();
                gui.Get(nodeData, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

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
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var parent = new CalculationGroup();
            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile()
                }
            };

            var nodeData = new StabilityPointStructuresCalculationScenarioContext(calculation,
                                                                                  parent,
                                                                                  failureMechanism,
                                                                                  assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = Substitute.For<IGui>();
                gui.Get(nodeData, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

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
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var foreshoreProfileInput = new TestForeshoreProfile();
            var parent = new CalculationGroup();
            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    ForeshoreProfile = foreshoreProfileInput
                }
            };
            TestForeshoreProfile.ChangeBreakWaterProperties(foreshoreProfileInput);

            var nodeData = new StabilityPointStructuresCalculationScenarioContext(calculation,
                                                                                  parent,
                                                                                  failureMechanism,
                                                                                  assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = Substitute.For<IGui>();
                gui.Get(nodeData, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

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
            var calculationObserver = Substitute.For<IObserver>();
            var calculationInputObserver = Substitute.For<IObserver>();

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var foreshoreProfileInput = new TestForeshoreProfile(true);
            var parent = new CalculationGroup();
            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    ForeshoreProfile = foreshoreProfileInput
                }
            };
            var nodeData = new StabilityPointStructuresCalculationScenarioContext(calculation,
                                                                                  parent,
                                                                                  failureMechanism,
                                                                                  assessmentSection);

            calculation.Attach(calculationObserver);
            calculation.InputParameters.Attach(calculationInputObserver);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = Substitute.For<IGui>();
                gui.Get(nodeData, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

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
        public void GivenCalculationWithOutputAndWithInputOutOfSync_WhenUpdateForeshoreProfileClicked_ThenInquiryAndExpectedOutputAndNotifications(bool continuation)
        {
            // Given
            var calculationObserver = Substitute.For<IObserver>();
            var calculationInputObserver = Substitute.For<IObserver>();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var foreshoreProfileInput = new TestForeshoreProfile(true);
            var parent = new CalculationGroup();
            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    ForeshoreProfile = foreshoreProfileInput
                },
                Output = new TestStructuresOutput()
            };

            var nodeData = new StabilityPointStructuresCalculationScenarioContext(calculation,
                                                                                  parent,
                                                                                  failureMechanism,
                                                                                  assessmentSection);

            calculation.Attach(calculationObserver);
            calculation.InputParameters.Attach(calculationInputObserver);

            if (continuation) {}

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
                var gui = Substitute.For<IGui>();
                gui.Get(nodeData, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

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
            var observer = Substitute.For<IObserver>();

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            var hydraulicBoundaryData = new HydraulicBoundaryData
            {
                HydraulicLocationConfigurationDatabase =
                {
                    FilePath = validHlcdFilePath
                },
                HydraulicBoundaryDatabases =
                {
                    new HydraulicBoundaryDatabase
                    {
                        FilePath = validHrdFilePath,
                        Version = validHrdFileVersion,
                        Locations =
                        {
                            hydraulicBoundaryLocation
                        }
                    }
                }
            };

            var assessmentSection = Substitute.For<IAssessmentSection>();
            assessmentSection.Id.Returns(string.Empty);
            assessmentSection.FailureMechanismContribution.Returns(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());
            assessmentSection.HydraulicBoundaryData.Returns(hydraulicBoundaryData);

            var parent = new CalculationGroup();
            var calculation = new TestStabilityPointStructuresCalculationScenario
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    InflowModelType = StabilityPointStructureInflowModelType.LowSill,
                    LoadSchematizationType = LoadSchematizationType.Linear
                }
            };
            calculation.Attach(observer);

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculationContext = new StabilityPointStructuresCalculationScenarioContext(calculation, parent, failureMechanism, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                IMainWindow mainWindow = MainWindowTestHelper.CreateMainWindowStub();

                var gui = Substitute.For<IGui>();
                gui.Get(calculationContext, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());
                gui.MainWindow.Returns(mainWindow);

                var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();
                calculatorFactory.CreateStructuresCalculator<StructuresStabilityPointCalculationInput>(
                    Arg.Any<HydraRingCalculationSettings>()).Returns(new TestStructuresCalculator<StructuresStabilityPointCalculationInput>());
                calculatorFactory.When(x => x.CreateStructuresCalculator<StructuresStabilityPointCalculationInput>(
                                           Arg.Any<HydraRingCalculationSettings>())).Do(invocation =>
                {
                    HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                        HydraulicBoundaryCalculationSettingsFactory.CreateSettings(hydraulicBoundaryData,
                                                                                   hydraulicBoundaryLocation),
                        (HydraRingCalculationSettings) invocation[0]);
                });

                plugin.Gui = gui;

                DialogBoxHandler = (name, wnd) =>
                {
                    // Expect an activity dialog which is automatically closed
                };

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(calculationContext, null, treeViewControl))
                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                {
                    // When
                    void Action() => contextMenuStrip.Items[contextMenuCalculateIndex].PerformClick();

                    // Then
                    TestHelper.AssertLogMessages(Action, messages =>
                    {
                        string[] msgs = messages.ToArray();
                        Assert.AreEqual(7, msgs.Length);
                        Assert.AreEqual($"Uitvoeren van berekening '{calculation.Name}' is gestart.", msgs[0]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                        CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[3]);
                        StringAssert.StartsWith("Puntconstructies berekening is uitgevoerd op de tijdelijke locatie", msgs[4]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[5]);
                        Assert.AreEqual($"Uitvoeren van berekening '{calculation.Name}' is gelukt.", msgs[6]);
                    });

                    Assert.IsNotNull(calculation.Output);
                }

                observer.Received(1).UpdateObserver();
                calculatorFactory.Received(1).CreateStructuresCalculator<StructuresStabilityPointCalculationInput>(
                    Arg.Any<HydraRingCalculationSettings>());
            }
        }

        [Test]
        public void GivenCalculationWithNonExistingFilePath_WhenValidatingFromContextMenu_ThenLogMessagesAdded()
        {
            // Given
            var observer = Substitute.For<IObserver>();

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            var hydraulicBoundaryData = new HydraulicBoundaryData
            {
                HydraulicLocationConfigurationDatabase =
                {
                    FilePath = validHlcdFilePath
                },
                HydraulicBoundaryDatabases =
                {
                    new HydraulicBoundaryDatabase
                    {
                        FilePath = validHrdFilePath,
                        Version = validHrdFileVersion,
                        Locations =
                        {
                            hydraulicBoundaryLocation
                        }
                    }
                }
            };

            var assessmentSection = Substitute.For<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryData.Returns(hydraulicBoundaryData);

            var parent = new CalculationGroup();
            var calculation = new TestStabilityPointStructuresCalculationScenario
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    InflowModelType = StabilityPointStructureInflowModelType.LowSill,
                    LoadSchematizationType = LoadSchematizationType.Linear
                }
            };
            calculation.Attach(observer);

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var calculationContext = new StabilityPointStructuresCalculationScenarioContext(calculation, parent, failureMechanism, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = Substitute.For<IGui>();
                gui.Get(calculationContext, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(calculationContext, null, treeViewControl))
                {
                    // When
                    void Action() => contextMenuStrip.Items[contextMenuValidateIndex].PerformClick();

                    // Then
                    TestHelper.AssertLogMessages(Action, messages =>
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
        public void ContextMenuStrip_CalculationWithIllustrationPoints_ContextMenuItemClearIllustrationPointsEnabled()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();

            var parent = new CalculationGroup();
            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>
            {
                Output = new TestStructuresOutput(new TestGeneralResultFaultTreeIllustrationPoint())
            };
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var nodeData = new StabilityPointStructuresCalculationScenarioContext(calculation, parent, failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = Substitute.For<IGui>();
                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                plugin.Gui = gui;

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
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();

            var parent = new CalculationGroup();
            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>
            {
                Output = new TestStructuresOutput()
            };
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var nodeData = new StabilityPointStructuresCalculationScenarioContext(calculation, parent, failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = Substitute.For<IGui>();
                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                plugin.Gui = gui;

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
        public void GivenCalculationWithIllustrationPoints_WhenClearIllustrationPointsClickedAndAborted_ThenInquiryAndIllustrationPointsNotCleared()
        {
            // Given
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();

            var parent = new CalculationGroup();
            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>
            {
                Output = new TestStructuresOutput(new TestGeneralResultFaultTreeIllustrationPoint())
            };
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var nodeData = new StabilityPointStructuresCalculationScenarioContext(calculation, parent, failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var calculationObserver = Substitute.For<IObserver>();
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
                var gui = Substitute.For<IGui>();
                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                plugin.Gui = gui;

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
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();

            var parent = new CalculationGroup();
            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>
            {
                Output = new TestStructuresOutput(new TestGeneralResultFaultTreeIllustrationPoint())
            };
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var nodeData = new StabilityPointStructuresCalculationScenarioContext(calculation, parent, failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var calculationObserver = Substitute.For<IObserver>();
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
                var gui = Substitute.For<IGui>();
                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                plugin.Gui = gui;

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

        [Test]
        public void OnNodeRemoved_ParentIsCalculationGroupContext_RemoveCalculationFromGroup()
        {
            // Setup
            var group = new CalculationGroup();
            var elementToBeRemoved = new StructuresCalculationScenario<StabilityPointStructuresInput>();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var observer = Substitute.For<IObserver>();
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var calculationContext = new StabilityPointStructuresCalculationScenarioContext(elementToBeRemoved,
                                                                                            group,
                                                                                            failureMechanism,
                                                                                            assessmentSection);
            var groupContext = new StabilityPointStructuresCalculationGroupContext(group,
                                                                                   null,
                                                                                   failureMechanism,
                                                                                   assessmentSection);

            group.Children.Add(elementToBeRemoved);
            group.Children.Add(new StructuresCalculationScenario<StabilityPointStructuresInput>());
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

        public override void Setup()
        {
            plugin = new StabilityPointStructuresPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(StabilityPointStructuresCalculationScenarioContext));
        }

        public override void TearDown()
        {
            plugin.Dispose();

            base.TearDown();
        }

        private static void ChangeStructure(StabilityPointStructure structure)
        {
            structure.CopyProperties(new StabilityPointStructure(
                                         new StabilityPointStructure.ConstructionProperties
                                         {
                                             Id = structure.Id,
                                             Name = structure.Name,
                                             Location = structure.Location
                                         }));
        }
    }
}