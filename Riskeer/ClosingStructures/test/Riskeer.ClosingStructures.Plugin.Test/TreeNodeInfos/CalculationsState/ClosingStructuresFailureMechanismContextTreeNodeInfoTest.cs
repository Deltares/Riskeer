﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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

using System.Collections.Generic;
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
using Rhino.Mocks;
using Riskeer.ClosingStructures.Data;
using Riskeer.ClosingStructures.Data.TestUtil;
using Riskeer.ClosingStructures.Forms.PresentationObjects;
using Riskeer.ClosingStructures.Forms.PresentationObjects.CalculationsState;
using Riskeer.Common.Data;
using Riskeer.Common.Data.AssessmentSection;
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
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.ClosingStructures.Plugin.Test.TreeNodeInfos.CalculationsState
{
    [TestFixture]
    public class ClosingStructuresFailureMechanismContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuValidateAllIndex = 2;
        private const int contextMenuCalculateAllIndex = 3;
        private const int contextMenuClearAllIndex = 5;
        private const int contextMenuClearIllustrationPointsIndex = 6;

        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, nameof(HydraulicBoundaryData));
        private static readonly string validHlcdFilePath = Path.Combine(testDataPath, "hlcd.sqlite");
        private static readonly string validHrdFilePath = Path.Combine(testDataPath, "complete.sqlite");
        private static readonly string validHrdFileVersion = "Dutch coast South19-11-2015 12:0013";

        private MockRepository mocksRepository;
        private ClosingStructuresPlugin plugin;
        private TreeNodeInfo info;

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            mocksRepository.ReplayAll();

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
            Assert.IsNull(info.CheckedState);
            Assert.IsNull(info.OnNodeChecked);
            Assert.IsNull(info.CanDrag);
            Assert.IsNull(info.CanDrop);
            Assert.IsNull(info.CanInsert);
            Assert.IsNull(info.OnDrop);
        }

        [Test]
        public void Text_WithContext_ReturnsName()
        {
            // Setup
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            mocksRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var context = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            string text = info.Text(context);

            // Assert
            Assert.AreEqual(failureMechanism.Name, text);
        }

        [Test]
        public void Image_Always_ReturnsFailureMechanismIcon()
        {
            // Setup
            mocksRepository.ReplayAll();

            // Call
            Image image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.FailureMechanismIcon, image);
        }

        [Test]
        public void ChildNodeObjects_WithContext_ReturnChildDataNodes()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var context = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(context).ToArray();

            // Assert
            Assert.AreEqual(2, children.Length);

            var inputsFolder = (CategoryTreeFolder) children[0];
            Assert.AreEqual("Invoer", inputsFolder.Name);
            Assert.AreEqual(TreeFolderCategory.Input, inputsFolder.Category);

            Assert.AreEqual(3, inputsFolder.Contents.Count());
            var profilesContext = (ForeshoreProfilesContext) inputsFolder.Contents.ElementAt(0);
            Assert.AreSame(failureMechanism.ForeshoreProfiles, profilesContext.WrappedData);
            Assert.AreSame(failureMechanism, profilesContext.ParentFailureMechanism);
            Assert.AreSame(assessmentSection, profilesContext.ParentAssessmentSection);

            var closingStructuresContext = (ClosingStructuresContext) inputsFolder.Contents.ElementAt(1);
            Assert.AreSame(failureMechanism.ClosingStructures, closingStructuresContext.WrappedData);
            Assert.AreSame(failureMechanism, closingStructuresContext.FailureMechanism);
            Assert.AreSame(assessmentSection, closingStructuresContext.AssessmentSection);

            var calculationsInputComments = (Comment) inputsFolder.Contents.ElementAt(2);
            Assert.AreSame(failureMechanism.CalculationsInputComments, calculationsInputComments);

            var calculationsFolder = (ClosingStructuresCalculationGroupContext) children[1];
            Assert.AreEqual(failureMechanism.CalculationsGroup, calculationsFolder.WrappedData);
            Assert.IsNull(calculationsFolder.Parent);
            Assert.AreEqual(failureMechanism, calculationsFolder.FailureMechanism);
        }

        [Test]
        public void ContextMenuStrip_WithContext_CallsContextMenuBuilderMethods()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            var context = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSection);

            var menuBuilder = mocksRepository.StrictMock<IContextMenuBuilder>();
            using (mocksRepository.Ordered())
            {
                menuBuilder.Expect(mb => mb.AddOpenItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
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
                var gui = mocksRepository.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocksRepository.Stub<IMainWindow>());
                mocksRepository.ReplayAll();

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(context, null, treeViewControl);
            }

            // Assert
            // Assert is done in TearDown
        }

        [Test]
        public void ContextMenuStrip_WithContext_AddCustomItems()
        {
            // Setup
            using (var treeView = new TreeViewControl())
            {
                var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
                var failureMechanism = new ClosingStructuresFailureMechanism();
                var context = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSection);
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocksRepository.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeView)).Return(menuBuilder);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(g => g.MainWindow).Return(mocksRepository.Stub<IMainWindow>());
                mocksRepository.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(context, assessmentSection, treeView))
                {
                    // Assert
                    Assert.AreEqual(12, menu.Items.Count);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndex,
                                                                  "Alles &valideren",
                                                                  "Er zijn geen berekeningen om te valideren.",
                                                                  RiskeerCommonFormsResources.ValidateAllIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndex,
                                                                  "Alles be&rekenen",
                                                                  "Er zijn geen berekeningen om uit te voeren.",
                                                                  RiskeerCommonFormsResources.CalculateAllIcon,
                                                                  false);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearAllIndex,
                                                                  "&Wis alle uitvoer...",
                                                                  "Er zijn geen berekeningen met uitvoer om te wissen.",
                                                                  RiskeerCommonFormsResources.ClearIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearIllustrationPointsIndex,
                                                                  "Wis alle &illustratiepunten...",
                                                                  "Er zijn geen berekeningen met illustratiepunten om te wissen.",
                                                                  RiskeerCommonFormsResources.ClearIllustrationPointsIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_AllRequiredInputSet_ContextMenuItemCalculateAllAndValidateAllEnabled()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new StructuresCalculation<ClosingStructuresInput>());

            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            var nodeData = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocksRepository.Stub<IGui>();
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocksRepository.Stub<IMainWindow>());
                mocksRepository.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndex,
                                                                  "Alles be&rekenen",
                                                                  "Voer alle berekeningen binnen dit faalmechanisme uit.",
                                                                  RiskeerCommonFormsResources.CalculateAllIcon);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndex,
                                                                  "Alles &valideren",
                                                                  "Valideer alle berekeningen binnen dit faalmechanisme.",
                                                                  RiskeerCommonFormsResources.ValidateAllIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_ClickOnCalculateAllItem_ScheduleAllChildCalculations()
        {
            // Setup
            IMainWindow mainWindow = MainWindowTestHelper.CreateMainWindowStub(mocksRepository);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            var failureMechanism = new ClosingStructuresFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        new TestClosingStructuresCalculationScenario
                        {
                            Name = "A",
                            InputParameters =
                            {
                                HydraulicBoundaryLocation = hydraulicBoundaryLocation
                            }
                        },
                        new TestClosingStructuresCalculationScenario
                        {
                            Name = "B",
                            InputParameters =
                            {
                                HydraulicBoundaryLocation = hydraulicBoundaryLocation
                            }
                        }
                    }
                }
            };

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

            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.Id).Return(string.Empty);
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());
            assessmentSection.Stub(a => a.HydraulicBoundaryData).Return(hydraulicBoundaryData);

            var context = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocksRepository.Stub<IGui>();
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);

                int nrOfCalculators = failureMechanism.Calculations.Count();
                var calculatorFactory = mocksRepository.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresClosureCalculationInput>(
                                             Arg<HydraRingCalculationSettings>.Is.NotNull))
                                 .WhenCalled(invocation =>
                                 {
                                     HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                         HydraulicBoundaryCalculationSettingsFactory.CreateSettings(hydraulicBoundaryData,
                                                                                                    hydraulicBoundaryLocation),
                                         (HydraRingCalculationSettings) invocation.Arguments[0]);
                                 })
                                 .Return(new TestStructuresCalculator<StructuresClosureCalculationInput>())
                                 .Repeat
                                 .Times(nrOfCalculators);
                mocksRepository.ReplayAll();

                plugin.Gui = gui;

                DialogBoxHandler = (name, wnd) =>
                {
                    // Expect an activity dialog which is automatically closed
                };

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                {
                    // Call
                    TestHelper.AssertLogMessages(() => contextMenu.Items[contextMenuCalculateAllIndex].PerformClick(), messages =>
                    {
                        List<string> messageList = messages.ToList();

                        // Assert
                        Assert.AreEqual(14, messageList.Count);
                        Assert.AreEqual("Uitvoeren van berekening 'A' is gestart.", messageList[0]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(messageList[1]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(messageList[2]);
                        CalculationServiceTestHelper.AssertCalculationStartMessage(messageList[3]);
                        StringAssert.StartsWith("Betrouwbaarheid sluiting kunstwerk berekening is uitgevoerd op de tijdelijke locatie", messageList[4]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(messageList[5]);
                        Assert.AreEqual("Uitvoeren van berekening 'A' is gelukt.", messageList[6]);

                        Assert.AreEqual("Uitvoeren van berekening 'B' is gestart.", messageList[7]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(messageList[8]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(messageList[9]);
                        CalculationServiceTestHelper.AssertCalculationStartMessage(messageList[10]);
                        StringAssert.StartsWith("Betrouwbaarheid sluiting kunstwerk berekening is uitgevoerd op de tijdelijke locatie", messageList[11]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(messageList[12]);
                        Assert.AreEqual("Uitvoeren van berekening 'B' is gelukt.", messageList[13]);
                    });
                }
            }
        }

        [Test]
        public void ContextMenuStrip_ClickOnValidateAllItem_ValidateAllChildCalculations()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            var failureMechanism = new ClosingStructuresFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        new TestClosingStructuresCalculationScenario
                        {
                            Name = "A",
                            InputParameters =
                            {
                                HydraulicBoundaryLocation = hydraulicBoundaryLocation
                            }
                        },
                        new TestClosingStructuresCalculationScenario
                        {
                            Name = "B",
                            InputParameters =
                            {
                                HydraulicBoundaryLocation = hydraulicBoundaryLocation
                            }
                        }
                    }
                }
            };

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

            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryData).Return(hydraulicBoundaryData);

            var context = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var gui = mocksRepository.Stub<IGui>();
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocksRepository.Stub<IMainWindow>());
                mocksRepository.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Call
                    void Call() => contextMenu.Items[contextMenuValidateAllIndex].PerformClick();

                    // Assert
                    TestHelper.AssertLogMessages(Call, messages =>
                    {
                        string[] messageList = messages.ToArray();

                        Assert.AreEqual(4, messageList.Length);
                        CalculationServiceTestHelper.AssertValidationStartMessage(messageList[0]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(messageList[1]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(messageList[2]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(messageList[3]);
                    });
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismWithCalculationsContainingIllustrationPoints_ContextMenuItemClearIllustrationPointsEnabled()
        {
            // Setup
            var calculationWithIllustrationPoints = new TestClosingStructuresCalculationScenario
            {
                Output = new TestStructuresOutput(new TestGeneralResultFaultTreeIllustrationPoint())
            };

            var calculationWithOutput = new TestClosingStructuresCalculationScenario
            {
                Output = new TestStructuresOutput()
            };

            var failureMechanism = new ClosingStructuresFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        calculationWithIllustrationPoints,
                        calculationWithOutput,
                        new TestClosingStructuresCalculationScenario()
                    }
                }
            };

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(null, mocksRepository);

            var nodeData = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocksRepository.Stub<IGui>();
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocksRepository.Stub<IMainWindow>());
                mocksRepository.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Call
                    ToolStripItem toolStripItem = contextMenu.Items[contextMenuClearIllustrationPointsIndex];

                    // Assert
                    Assert.IsTrue(toolStripItem.Enabled);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismWithCalculationsWithoutIllustrationPoints_ContextMenuItemClearIllustrationPointsDisabled()
        {
            // Setup
            var calculationWithOutput = new TestClosingStructuresCalculationScenario
            {
                Output = new TestStructuresOutput()
            };

            var failureMechanism = new ClosingStructuresFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        calculationWithOutput,
                        new TestClosingStructuresCalculationScenario()
                    }
                }
            };

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(null, mocksRepository);

            var nodeData = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocksRepository.Stub<IGui>();
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocksRepository.Stub<IMainWindow>());
                mocksRepository.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Call
                    ToolStripItem toolStripItem = contextMenu.Items[contextMenuClearIllustrationPointsIndex];

                    // Assert
                    Assert.IsFalse(toolStripItem.Enabled);
                }
            }
        }

        [Test]
        public void GivenCalculationsWithIllustrationPoints_WhenClearIllustrationPointsClickedAndAborted_ThenInquiryAndIllustrationPointsNotCleared()
        {
            // Given
            var calculationWithIllustrationPoints = new TestClosingStructuresCalculationScenario
            {
                Output = new TestStructuresOutput(new TestGeneralResultFaultTreeIllustrationPoint())
            };

            var calculationWithOutput = new TestClosingStructuresCalculationScenario
            {
                Output = new TestStructuresOutput()
            };

            var failureMechanism = new ClosingStructuresFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        calculationWithIllustrationPoints,
                        calculationWithOutput,
                        new TestClosingStructuresCalculationScenario()
                    }
                }
            };

            var calculationObserver = mocksRepository.StrictMock<IObserver>();
            calculationWithIllustrationPoints.Attach(calculationObserver);

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(null, mocksRepository);

            var nodeData = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var messageBoxText = "";
            DialogBoxHandler = (name, wnd) =>
            {
                var helper = new MessageBoxTester(wnd);
                messageBoxText = helper.Text;

                helper.ClickCancel();
            };

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocksRepository.Stub<IGui>();
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocksRepository.Stub<IMainWindow>());
                mocksRepository.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    contextMenu.Items[contextMenuClearIllustrationPointsIndex].PerformClick();

                    // Then
                    Assert.AreEqual("Weet u zeker dat u alle illustratiepunten wilt wissen?", messageBoxText);

                    Assert.IsTrue(calculationWithOutput.HasOutput);
                    Assert.IsTrue(calculationWithIllustrationPoints.Output.HasGeneralResult);
                }
            }
        }

        [Test]
        public void GivenCalculationsWithIllustrationPoints_WhenClearIllustrationPointsClickedAndContinued_ThenInquiryAndIllustrationPointsCleared()
        {
            // Given
            var calculationWithIllustrationPoints = new TestClosingStructuresCalculationScenario
            {
                Output = new TestStructuresOutput(new TestGeneralResultFaultTreeIllustrationPoint())
            };

            var calculationWithOutput = new TestClosingStructuresCalculationScenario
            {
                Output = new TestStructuresOutput()
            };

            var failureMechanism = new ClosingStructuresFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        calculationWithIllustrationPoints,
                        calculationWithOutput,
                        new TestClosingStructuresCalculationScenario()
                    }
                }
            };

            var affectedCalculationObserver = mocksRepository.StrictMock<IObserver>();
            affectedCalculationObserver.Expect(o => o.UpdateObserver());
            calculationWithIllustrationPoints.Attach(affectedCalculationObserver);

            var unaffectedCalculationObserver = mocksRepository.StrictMock<IObserver>();
            calculationWithOutput.Attach(unaffectedCalculationObserver);

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(null, mocksRepository);

            var nodeData = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var messageBoxText = "";
            DialogBoxHandler = (name, wnd) =>
            {
                var helper = new MessageBoxTester(wnd);
                messageBoxText = helper.Text;

                helper.ClickOk();
            };

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocksRepository.Stub<IGui>();
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocksRepository.Stub<IMainWindow>());
                mocksRepository.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    contextMenu.Items[contextMenuClearIllustrationPointsIndex].PerformClick();

                    // Then
                    Assert.AreEqual("Weet u zeker dat u alle illustratiepunten wilt wissen?", messageBoxText);

                    Assert.IsTrue(calculationWithOutput.HasOutput);
                    Assert.IsFalse(calculationWithIllustrationPoints.Output.HasGeneralResult);
                }
            }
        }

        public override void Setup()
        {
            mocksRepository = new MockRepository();
            plugin = new ClosingStructuresPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(ClosingStructuresFailureMechanismContext));
        }

        public override void TearDown()
        {
            plugin.Dispose();

            mocksRepository.VerifyAll();
        }
    }
}