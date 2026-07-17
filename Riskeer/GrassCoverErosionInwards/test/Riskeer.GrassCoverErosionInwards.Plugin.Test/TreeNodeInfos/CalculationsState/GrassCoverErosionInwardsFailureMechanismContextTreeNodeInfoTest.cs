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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
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
using NSubstitute;
using Riskeer.Common.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Service.TestUtil;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Data.TestUtil;
using Riskeer.GrassCoverErosionInwards.Forms.PresentationObjects;
using Riskeer.GrassCoverErosionInwards.Forms.PresentationObjects.CalculationsState;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.GrassCoverErosionInwards.Plugin.Test.TreeNodeInfos.CalculationsState
{
    [TestFixture]
    public class GrassCoverErosionInwardsFailureMechanismContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuValidateAllIndex = 2;
        private const int contextMenuCalculateAllIndex = 3;
        private const int contextMenuClearAllIndex = 5;
        private const int contextMenuClearIllustrationPointsIndex = 6;

        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, nameof(HydraulicBoundaryData));
        private static readonly string validHlcdFilePath = Path.Combine(testDataPath, "hlcd.sqlite");
        private static readonly string validHrdFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
        private static readonly string validHrdFileVersion = "Dutch coast South19-11-2015 12:0013";

        private GrassCoverErosionInwardsPlugin plugin;
        private TreeNodeInfo info;

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
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
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var context = new GrassCoverErosionInwardsFailureMechanismContext(new GrassCoverErosionInwardsFailureMechanism(), assessmentSection);

            // Call
            string text = info.Text(context);

            // Assert
            Assert.AreEqual("Grasbekleding erosie kruin en binnentalud", text);
        }

        [Test]
        public void Image_Always_ReturnsFailureMechanismIcon()
        {
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
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var context = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(context).ToArray();

            // Assert
            Assert.AreEqual(2, children.Length);

            var inputsFolder = (CategoryTreeFolder) children[0];
            Assert.AreEqual("Invoer", inputsFolder.Name);
            Assert.AreEqual(TreeFolderCategory.Input, inputsFolder.Category);

            Assert.AreEqual(2, inputsFolder.Contents.Count());

            var dikeProfilesContext = (DikeProfilesContext) inputsFolder.Contents.ElementAt(0);
            Assert.AreSame(failureMechanism.DikeProfiles, dikeProfilesContext.WrappedData);
            Assert.AreSame(failureMechanism, dikeProfilesContext.ParentFailureMechanism);
            Assert.AreSame(assessmentSection, dikeProfilesContext.ParentAssessmentSection);

            var calculationsInputComments = (Comment) inputsFolder.Contents.ElementAt(1);
            Assert.AreSame(failureMechanism.CalculationsInputComments, calculationsInputComments);

            var calculationsFolder = (GrassCoverErosionInwardsCalculationGroupContext) children[1];
            Assert.AreSame(failureMechanism.CalculationsGroup, calculationsFolder.WrappedData);
            Assert.IsNull(calculationsFolder.Parent);
            Assert.AreSame(failureMechanism, calculationsFolder.FailureMechanism);
        }

        [Test]
        public void ContextMenuStrip_WithContext_CallsContextMenuBuilderMethods()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var context = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, assessmentSection);

            var menuBuilder = Substitute.For<IContextMenuBuilder>();
                menuBuilder.AddOpenItem().Returns(menuBuilder);
                menuBuilder.AddSeparator().Returns(menuBuilder);
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>()).Returns(menuBuilder);
                menuBuilder.AddCollapseAllItem().Returns(menuBuilder);
                menuBuilder.AddExpandAllItem().Returns(menuBuilder);
                menuBuilder.AddPropertiesItem().Returns(menuBuilder);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = Substitute.For<IGui>();
                gui.Get(context, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());
                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(context, null, treeViewControl);
            }

            // Assert
            Received.InOrder(() =>
            {
                menuBuilder.AddOpenItem();
                menuBuilder.AddSeparator();
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                menuBuilder.AddSeparator();
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                menuBuilder.AddSeparator();
                menuBuilder.AddCollapseAllItem();
                menuBuilder.AddExpandAllItem();
                menuBuilder.AddSeparator();
                menuBuilder.AddPropertiesItem();
                menuBuilder.Build();
            });
        }

        [Test]
        public void ContextMenuStrip_WithContext_AddCustomItems()
        {
            // Setup
            using (var treeView = new TreeViewControl())
            {
                var assessmentSection = Substitute.For<IAssessmentSection>();
                var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
                var context = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, assessmentSection);
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = Substitute.For<IGui>();
                gui.Get(context, treeView).Returns(menuBuilder);
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());
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
        public void ContextMenuStrip_AllRequiredInputSet_ContextMenuItemCalculateAllEnabled()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation());

            var assessmentSection = Substitute.For<IAssessmentSection>();

            var nodeData = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = Substitute.For<IGui>();
                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());
                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndex,
                                                                  "Alles be&rekenen",
                                                                  "Voer alle berekeningen binnen dit faalmechanisme uit.",
                                                                  RiskeerCommonFormsResources.CalculateAllIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_AllRequiredInputSet_ContextMenuItemValidateAllEnabled()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation());

            var assessmentSection = Substitute.For<IAssessmentSection>();

            var nodeData = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = Substitute.For<IGui>();
                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());
                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
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
            IMainWindow mainWindow = MainWindowTestHelper.CreateMainWindowStub();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            failureMechanism.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation
            {
                Name = "A",
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    DikeProfile = DikeProfileTestFactory.CreateDikeProfile()
                }
            });
            failureMechanism.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation
            {
                Name = "B",
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    DikeProfile = DikeProfileTestFactory.CreateDikeProfile()
                }
            });

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

            var context = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = Substitute.For<IGui>();
                gui.Get(context, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(mainWindow);

                int nrOfCalculators = failureMechanism.Calculations.Count();
                var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();
				
                calculatorFactory
                    .CreateOvertoppingCalculator(Arg.Is<HydraRingCalculationSettings>(x=>x!=null))
                    .Returns(callInfo =>
                    {
                        HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                            HydraulicBoundaryCalculationSettingsFactory.CreateSettings(hydraulicBoundaryData,
                                                                                       hydraulicBoundaryLocation),
                            callInfo.Arg<HydraRingCalculationSettings>());
                        return new TestOvertoppingCalculator();
                    });
              
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
                        StringAssert.StartsWith("De overloop en overslag berekening is uitgevoerd op de tijdelijke locatie", messageList[4]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(messageList[5]);
                        Assert.AreEqual("Uitvoeren van berekening 'A' is gelukt.", messageList[6]);

                        Assert.AreEqual("Uitvoeren van berekening 'B' is gestart.", messageList[7]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(messageList[8]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(messageList[9]);
                        CalculationServiceTestHelper.AssertCalculationStartMessage(messageList[10]);
                        StringAssert.StartsWith("De overloop en overslag berekening is uitgevoerd op de tijdelijke locatie", messageList[11]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(messageList[12]);
                        Assert.AreEqual("Uitvoeren van berekening 'B' is gelukt.", messageList[13]);
                    });
                }
                
                calculatorFactory.Received(nrOfCalculators).CreateOvertoppingCalculator(Arg.Is<HydraRingCalculationSettings>(x=>x!=null));
            }
        }

        [Test]
        public void ContextMenuStrip_ClickOnValidateAllItem_ValidateAllChildCalculations()
        {
            // Setup
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        new GrassCoverErosionInwardsCalculation
                        {
                            Name = "A",
                            InputParameters =
                            {
                                HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                                DikeProfile = DikeProfileTestFactory.CreateDikeProfile()
                            }
                        },
                        new GrassCoverErosionInwardsCalculation
                        {
                            Name = "B",
                            InputParameters =
                            {
                                HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                                DikeProfile = DikeProfileTestFactory.CreateDikeProfile()
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

            var assessmentSection = Substitute.For<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryData.Returns(hydraulicBoundaryData);

            var context = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = Substitute.For<IGui>();
                gui.Get(context, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());
                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Call
                    TestHelper.AssertLogMessages(() => contextMenu.Items[contextMenuValidateAllIndex].PerformClick(), messages =>
                    {
                        List<string> messageList = messages.ToList();

                        // Assert
                        Assert.AreEqual(4, messageList.Count);
                        CalculationServiceTestHelper.AssertValidationStartMessage(messageList[0]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(messageList[1]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(messageList[2]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(messageList[3]);
                    });
                }
            }
        }

        [Test]
        [TestCaseSource(nameof(GetCalculationConfigurationsWithIllustrationPoints))]
        public void ContextMenuStrip_FailureMechanismWithCalculationsContainingIllustrationPoints_ContextMenuItemClearIllustrationPointsEnabled(
            GrassCoverErosionInwardsCalculation calculation)
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();

            var calculationWithOutput = new GrassCoverErosionInwardsCalculation
            {
                Output = new TestGrassCoverErosionInwardsOutput()
            };

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        calculation,
                        calculationWithOutput,
                        new GrassCoverErosionInwardsCalculation()
                    }
                }
            };

            var context = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, assessmentSection);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            using (var treeViewControl = new TreeViewControl())
            {
                var gui = Substitute.For<IGui>();
                gui.Get(context, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());
                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
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
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();

            var calculationWithOutput = new GrassCoverErosionInwardsCalculation
            {
                Output = new TestGrassCoverErosionInwardsOutput()
            };

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        calculationWithOutput,
                        new GrassCoverErosionInwardsCalculation()
                    }
                }
            };

            var context = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, assessmentSection);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            using (var treeViewControl = new TreeViewControl())
            {
                var gui = Substitute.For<IGui>();
                gui.Get(context, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());
                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
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
            var calculationWithIllustrationPoints = new GrassCoverErosionInwardsCalculation
            {
                Output = new TestGrassCoverErosionInwardsOutput(new TestGeneralResultFaultTreeIllustrationPoint())
            };

            var calculationWithOutput = new GrassCoverErosionInwardsCalculation
            {
                Output = new TestGrassCoverErosionInwardsOutput()
            };

            var calculationObserver = Substitute.For<IObserver>();
            calculationWithIllustrationPoints.Attach(calculationObserver);

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        calculationWithIllustrationPoints,
                        calculationWithOutput,
                        new GrassCoverErosionInwardsCalculation()
                    }
                }
            };

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();

            var nodeData = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, assessmentSection);

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
                gui.Get(nodeData, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());
                plugin.Gui = gui;

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    contextMenuStrip.Items[contextMenuClearIllustrationPointsIndex].PerformClick();

                    // Then
                    Assert.AreEqual("Weet u zeker dat u alle illustratiepunten wilt wissen?", messageBoxText);

                    Assert.IsTrue(calculationWithOutput.HasOutput);

                    GrassCoverErosionInwardsOutput output = calculationWithIllustrationPoints.Output;
                    Assert.IsTrue(output.OvertoppingOutput.HasGeneralResult);
                    Assert.IsTrue(output.DikeHeightOutput.HasGeneralResult);
                    Assert.IsTrue(output.OvertoppingRateOutput.HasGeneralResult);
                }
            }
        }

        [Test]
        public void GivenCalculationsWithIllustrationPoints_WhenClearIllustrationPointsClickedAndContinued_ThenInquiryAndIllustrationPointsCleared()
        {
            // Given
            var calculationWithIllustrationPoints = new GrassCoverErosionInwardsCalculation
            {
                Output = new TestGrassCoverErosionInwardsOutput(new TestGeneralResultFaultTreeIllustrationPoint())
            };

            var calculationWithOutput = new GrassCoverErosionInwardsCalculation
            {
                Output = new TestGrassCoverErosionInwardsOutput()
            };

            var affectedCalculationObserver = Substitute.For<IObserver>();
            calculationWithIllustrationPoints.Attach(affectedCalculationObserver);

            var unaffectedCalculationObserver = Substitute.For<IObserver>();
            calculationWithOutput.Attach(unaffectedCalculationObserver);

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        calculationWithIllustrationPoints,
                        calculationWithOutput,
                        new GrassCoverErosionInwardsCalculation()
                    }
                }
            };

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();
            var nodeData = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, assessmentSection);

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
                gui.Get(nodeData, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());
                plugin.Gui = gui;

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    contextMenuStrip.Items[contextMenuClearIllustrationPointsIndex].PerformClick();

                    // Then
                    Assert.AreEqual("Weet u zeker dat u alle illustratiepunten wilt wissen?", messageBoxText);

                    Assert.IsTrue(calculationWithOutput.HasOutput);

                    GrassCoverErosionInwardsOutput output = calculationWithIllustrationPoints.Output;
                    Assert.IsFalse(output.OvertoppingOutput.HasGeneralResult);
                    Assert.IsFalse(output.DikeHeightOutput.HasGeneralResult);
                    Assert.IsFalse(output.OvertoppingRateOutput.HasGeneralResult);
                }
                affectedCalculationObserver.Received().UpdateObserver();
            }
        }

        public override void Setup()
        {
            plugin = new GrassCoverErosionInwardsPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(GrassCoverErosionInwardsFailureMechanismContext));
        }

        public override void TearDown()
        {
            plugin.Dispose();
            base.TearDown();
        }

        private static IEnumerable<TestCaseData> GetCalculationConfigurationsWithIllustrationPoints()
        {
            var random = new Random(21);
            var calculationWithOverToppingOutputWithIllustrationPoints = new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(new TestGeneralResultFaultTreeIllustrationPoint()),
                                                            null,
                                                            null)
            };

            var calculationWithDikeHeightRateWithIllustrationPoints = new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(random.NextDouble()),
                                                            new TestDikeHeightOutput(new TestGeneralResultFaultTreeIllustrationPoint()),
                                                            null)
            };

            var calculationWithOvertoppingRateWithIllustrationPoints = new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(random.NextDouble()),
                                                            null,
                                                            new TestOvertoppingRateOutput(new TestGeneralResultFaultTreeIllustrationPoint()))
            };

            yield return new TestCaseData(calculationWithOverToppingOutputWithIllustrationPoints);
            yield return new TestCaseData(calculationWithDikeHeightRateWithIllustrationPoints);
            yield return new TestCaseData(calculationWithOvertoppingRateWithIllustrationPoints);
        }
    }
}