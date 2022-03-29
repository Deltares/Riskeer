// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Gui;
using Core.Gui.Commands;
using Core.Gui.ContextMenu;
using Core.Gui.Forms.Main;
using Core.Gui.TestUtil;
using Core.Gui.TestUtil.ContextMenu;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Service.TestUtil;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Forms.PresentationObjects;
using Riskeer.MacroStabilityInwards.Forms.PresentationObjects.CalculationsState;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators;
using CoreGuiResources = Core.Gui.Properties.Resources;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.MacroStabilityInwards.Plugin.Test.TreeNodeInfos.CalculationsState
{
    [TestFixture]
    public class MacroStabilityInwardsFailureMechanismContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuValidateAllIndex = 2;
        private const int contextMenuCalculateAllIndex = 3;
        private const int contextMenuClearIndex = 5;

        private MockRepository mocks;
        private MacroStabilityInwardsPlugin plugin;
        private TreeNodeInfo info;

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            mocks.ReplayAll();

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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var mechanism = new MacroStabilityInwardsFailureMechanism();
            var context = new MacroStabilityInwardsFailureMechanismContext(mechanism, assessmentSection);

            // Call
            string text = info.Text(context);

            // Assert
            Assert.AreEqual("Macrostabiliteit binnenwaarts", text);
        }

        [Test]
        public void Image_Always_ReturnsMacroStabilityInwardsIcon()
        {
            // Setup
            mocks.ReplayAll();

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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new MacroStabilityInwardsCalculationScenario());
            failureMechanism.CalculationsGroup.Children.Add(new MacroStabilityInwardsCalculationScenario());

            var context = new MacroStabilityInwardsFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(context).ToArray();

            // Assert
            Assert.AreEqual(2, children.Length);
            var inputsFolder = (CategoryTreeFolder) children[0];
            Assert.AreEqual("Invoer", inputsFolder.Name);
            Assert.AreEqual(TreeFolderCategory.Input, inputsFolder.Category);

            Assert.AreEqual(3, inputsFolder.Contents.Count());
            var surfaceLinesContext = (MacroStabilityInwardsSurfaceLinesContext) inputsFolder.Contents.ElementAt(0);
            Assert.AreSame(failureMechanism.SurfaceLines, surfaceLinesContext.WrappedData);
            Assert.AreSame(failureMechanism, surfaceLinesContext.FailureMechanism);
            Assert.AreSame(assessmentSection, surfaceLinesContext.AssessmentSection);

            var stochasticSoilModelContext = (MacroStabilityInwardsStochasticSoilModelCollectionContext) inputsFolder.Contents.ElementAt(1);
            Assert.AreSame(failureMechanism, stochasticSoilModelContext.FailureMechanism);
            Assert.AreSame(failureMechanism, stochasticSoilModelContext.FailureMechanism);
            Assert.AreSame(assessmentSection, stochasticSoilModelContext.AssessmentSection);

            var calculationsInputComments = (Comment) inputsFolder.Contents.ElementAt(2);
            Assert.AreSame(failureMechanism.CalculationsInputComments, calculationsInputComments);

            var calculationsFolder = (MacroStabilityInwardsCalculationGroupContext) children[1];
            Assert.AreSame(failureMechanism.CalculationsGroup, calculationsFolder.WrappedData);
            Assert.IsNull(calculationsFolder.Parent);
            Assert.AreSame(failureMechanism.SurfaceLines, calculationsFolder.AvailableMacroStabilityInwardsSurfaceLines);
            Assert.AreSame(failureMechanism.StochasticSoilModels, calculationsFolder.AvailableStochasticSoilModels);
            Assert.AreSame(failureMechanism, calculationsFolder.FailureMechanism);
        }

        [Test]
        [TestCase(false, TestName = "GivenMultipleCalculations_WhenClearingOutputFromContextMenu_ThenOutputCleared(false)")]
        [TestCase(true, TestName = "GivenMultipleCalculations_WhenClearingOutputFromContextMenu_ThenOutputCleared(true)")]
        public void GivenMultipleCalculationsWithOutput_WhenClearingOutputFromContextMenu_ThenOutputCleared(bool confirm)
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                var calculation1 = new MacroStabilityInwardsCalculationScenario
                {
                    Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
                };
                var calculation2 = new MacroStabilityInwardsCalculationScenario
                {
                    Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
                };

                var observer = mocks.StrictMock<IObserver>();
                if (confirm)
                {
                    observer.Expect(o => o.UpdateObserver()).Repeat.Twice();
                }

                var failureMechanism = new MacroStabilityInwardsFailureMechanism();
                failureMechanism.CalculationsGroup.Children.Add(calculation1);
                failureMechanism.CalculationsGroup.Children.Add(calculation2);

                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var context = new MacroStabilityInwardsFailureMechanismContext(failureMechanism, assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(menuBuilder);
                mocks.ReplayAll();

                plugin.Gui = gui;

                failureMechanism.CalculationsGroup.Children.Add(calculation1);
                failureMechanism.CalculationsGroup.Children.Add(calculation2);
                failureMechanism.CalculationsGroup.Children.ElementAt(0).Attach(observer);
                failureMechanism.CalculationsGroup.Children.ElementAt(1).Attach(observer);

                string messageBoxTitle = null, messageBoxText = null;
                DialogBoxHandler = (name, wnd) =>
                {
                    var messageBox = new MessageBoxTester(wnd);

                    messageBoxText = messageBox.Text;
                    messageBoxTitle = messageBox.Title;

                    if (confirm)
                    {
                        messageBox.ClickOk();
                    }
                    else
                    {
                        messageBox.ClickCancel();
                    }
                };

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // When
                    contextMenuStrip.Items[contextMenuClearIndex].PerformClick();

                    // Then
                    foreach (ICalculation calc in failureMechanism.CalculationsGroup.Children.OfType<ICalculation>())
                    {
                        Assert.AreNotEqual(confirm, calc.HasOutput);
                    }

                    Assert.AreEqual("Bevestigen", messageBoxTitle);
                    Assert.AreEqual("Weet u zeker dat u alle uitvoer wilt wissen?", messageBoxText);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_HasCalculationWithOutput_ReturnsContextMenuWithCommonItems()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var calculation = new MacroStabilityInwardsCalculationScenario
            {
                Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
            };
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var context = new MacroStabilityInwardsFailureMechanismContext(failureMechanism, assessmentSection);

            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.Stub<IImportCommandHandler>();
            var exportCommandHandler = mocks.Stub<IExportCommandHandler>();
            var updateCommandHandler = mocks.Stub<IUpdateCommandHandler>();
            var viewCommandsHandler = mocks.Stub<IViewCommands>();

            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler,
                                                         importCommandHandler,
                                                         exportCommandHandler,
                                                         updateCommandHandler,
                                                         viewCommandsHandler,
                                                         context,
                                                         treeViewControl);

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(menuBuilder);
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Assert
                    Assert.AreEqual(11, menu.Items.Count);

                    TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                  0,
                                                                  "&Openen",
                                                                  "Open de gegevens in een nieuw documentvenster.",
                                                                  CoreGuiResources.OpenIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                  2,
                                                                  "Alles &valideren",
                                                                  "Valideer alle berekeningen binnen dit faalmechanisme.",
                                                                  RiskeerCommonFormsResources.ValidateAllIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                  3,
                                                                  "Alles be&rekenen",
                                                                  "Voer alle berekeningen binnen dit faalmechanisme uit.",
                                                                  RiskeerCommonFormsResources.CalculateAllIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                  5,
                                                                  "&Wis alle uitvoer...",
                                                                  "Wis de uitvoer van alle berekeningen binnen dit faalmechanisme.",
                                                                  RiskeerCommonFormsResources.ClearIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                  7,
                                                                  "Alles i&nklappen",
                                                                  "Klap dit element en alle onderliggende elementen in.",
                                                                  CoreGuiResources.CollapseAllIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                  8,
                                                                  "Alles ui&tklappen",
                                                                  "Klap dit element en alle onderliggende elementen uit.",
                                                                  CoreGuiResources.ExpandAllIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                  10,
                                                                  "Ei&genschappen",
                                                                  "Toon de eigenschappen in het Eigenschappenpaneel.",
                                                                  CoreGuiResources.PropertiesHS,
                                                                  false);

                    CollectionAssert.AllItemsAreInstancesOfType(new[]
                    {
                        menu.Items[1],
                        menu.Items[4],
                        menu.Items[6],
                        menu.Items[9]
                    }, typeof(ToolStripSeparator));
                }
            }
        }

        [Test]
        public void ContextMenuStrip_MacroStabilityInwardsFailureMechanismNoOutput_ContextMenuItemClearAllOutputDisabled()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var data = new MacroStabilityInwardsFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var context = new MacroStabilityInwardsFailureMechanismContext(data, assessmentSection);

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(menuBuilder);
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Assert
                    ToolStripItem clearOutputItem = contextMenu.Items[contextMenuClearIndex];
                    Assert.IsFalse(clearOutputItem.Enabled);
                    Assert.AreEqual("Er zijn geen berekeningen met uitvoer om te wissen.", clearOutputItem.ToolTipText);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_MacroStabilityInwardsFailureMechanismWithOutput_ContextMenuItemClearAllOutputEnabled()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var calculation = new MacroStabilityInwardsCalculationScenario
                {
                    Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
                };

                var failureMechanism = new MacroStabilityInwardsFailureMechanism();
                failureMechanism.CalculationsGroup.Children.Add(calculation);

                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var context = new MacroStabilityInwardsFailureMechanismContext(failureMechanism, assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(menuBuilder);
                mocks.ReplayAll();

                plugin.Gui = gui;

                failureMechanism.CalculationsGroup.Children.Add(calculation);

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Assert
                    ToolStripItem clearOutputItem = contextMenu.Items[contextMenuClearIndex];
                    Assert.IsTrue(clearOutputItem.Enabled);
                    Assert.AreEqual("Wis de uitvoer van alle berekeningen binnen dit faalmechanisme.", clearOutputItem.ToolTipText);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_AllRequiredInputSet_ContextMenuItemCalculateAllAndValidateAllEnabled()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSection = new AssessmentSectionStub();
                var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

                assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
                {
                    hydraulicBoundaryLocation
                }, true);

                var failureMechanism = new MacroStabilityInwardsFailureMechanism
                {
                    CalculationsGroup =
                    {
                        Children =
                        {
                            MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(hydraulicBoundaryLocation)
                        }
                    }
                };

                var context = new MacroStabilityInwardsFailureMechanismContext(failureMechanism, assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(menuBuilder);
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuCalculateAllIndex,
                                                                  "Alles be&rekenen",
                                                                  "Voer alle berekeningen binnen dit faalmechanisme uit.",
                                                                  RiskeerCommonFormsResources.CalculateAllIcon);

                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuValidateAllIndex,
                                                                  "Alles &valideren",
                                                                  "Valideer alle berekeningen binnen dit faalmechanisme.",
                                                                  RiskeerCommonFormsResources.ValidateAllIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_WithContext_CallsContextMenuBuilderMethods()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var failureMechanism = new MacroStabilityInwardsFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var context = new MacroStabilityInwardsFailureMechanismContext(failureMechanism, assessmentSection);

                var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();
                using (mocks.Ordered())
                {
                    menuBuilder.Expect(mb => mb.AddOpenItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.Build()).Return(null);
                }

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(menuBuilder);
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(context, null, treeViewControl);
            }

            // Assert
            // Assert expectancies are called in TearDown()
        }

        [Test]
        public void ContextMenuStrip_ClickOnValidateAllItem_ValidateAllChildCalculations()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSection = new AssessmentSectionStub();
                var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

                assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
                {
                    hydraulicBoundaryLocation
                }, true);

                var failureMechanism = new MacroStabilityInwardsFailureMechanism();
                MacroStabilityInwardsCalculationScenario validCalculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(hydraulicBoundaryLocation);
                validCalculation.Name = "A";
                MacroStabilityInwardsCalculationScenario invalidCalculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithInvalidInput();
                invalidCalculation.Name = "B";

                failureMechanism.CalculationsGroup.Children.Add(validCalculation);
                failureMechanism.CalculationsGroup.Children.Add(invalidCalculation);

                var context = new MacroStabilityInwardsFailureMechanismContext(failureMechanism, assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilder);
                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                using (new MacroStabilityInwardsCalculatorFactoryConfig())
                {
                    // Call
                    void Call() => contextMenu.Items[contextMenuValidateAllIndex].PerformClick();

                    // Assert
                    TestHelper.AssertLogMessages(Call, messages =>
                    {
                        string[] msgs = messages.ToArray();
                        Assert.AreEqual(9, msgs.Length);
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                        Assert.AreEqual("Validatie van waterspanningen in extreme omstandigheden is gestart.", msgs[1]);
                        Assert.AreEqual("Validatie van waterspanningen in dagelijkse omstandigheden is gestart.", msgs[2]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs[3]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs[4]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs[8]);
                    });
                }
            }
        }

        [Test]
        public void ContextMenuStrip_ClickOnCalculateAllItem_ScheduleAllChildCalculations()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var failureMechanism = new MacroStabilityInwardsFailureMechanism();
                var assessmentSection = new AssessmentSectionStub();
                var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

                assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
                {
                    hydraulicBoundaryLocation
                }, true);

                MacroStabilityInwardsCalculationScenario calculationA = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(hydraulicBoundaryLocation);
                calculationA.Name = "A";
                MacroStabilityInwardsCalculationScenario calculationB = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(hydraulicBoundaryLocation);
                calculationB.Name = "B";

                failureMechanism.CalculationsGroup.Children.Add(calculationA);
                failureMechanism.CalculationsGroup.Children.Add(calculationB);

                var context = new MacroStabilityInwardsFailureMechanismContext(failureMechanism, assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                IMainWindow mainWindow = MainWindowTestHelper.CreateMainWindowStub(mocks);

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilder);
                mocks.ReplayAll();

                plugin.Gui = gui;

                DialogBoxHandler = (name, wnd) =>
                {
                    // Expect an activity dialog which is automatically closed
                };

                using (new MacroStabilityInwardsCalculatorFactoryConfig())
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Call
                    void Call() => contextMenu.Items[contextMenuCalculateAllIndex].PerformClick();

                    // Assert
                    TestHelper.AssertLogMessages(Call, messages =>
                    {
                        string[] msgs = messages.ToArray();
                        Assert.AreEqual(16, msgs.Length);
                        Assert.AreEqual("Uitvoeren van berekening 'A' is gestart.", msgs[0]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                        Assert.AreEqual("Validatie van waterspanningen in extreme omstandigheden is gestart.", msgs[2]);
                        Assert.AreEqual("Validatie van waterspanningen in dagelijkse omstandigheden is gestart.", msgs[3]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs[4]);
                        CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[5]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[6]);
                        Assert.AreEqual("Uitvoeren van berekening 'A' is gelukt.", msgs[7]);

                        Assert.AreEqual("Uitvoeren van berekening 'B' is gestart.", msgs[8]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs[9]);
                        Assert.AreEqual("Validatie van waterspanningen in extreme omstandigheden is gestart.", msgs[10]);
                        Assert.AreEqual("Validatie van waterspanningen in dagelijkse omstandigheden is gestart.", msgs[11]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs[12]);
                        CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[13]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[14]);
                        Assert.AreEqual("Uitvoeren van berekening 'B' is gelukt.", msgs[15]);
                    });
                }
            }
        }

        public override void Setup()
        {
            mocks = new MockRepository();
            plugin = new MacroStabilityInwardsPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(MacroStabilityInwardsFailureMechanismContext));
        }

        public override void TearDown()
        {
            plugin.Dispose();
            mocks.VerifyAll();

            base.TearDown();
        }
    }
}