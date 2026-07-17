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
using log4net.Core;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using NSubstitute;
using Riskeer.Common.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Service.TestUtil;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Forms.PresentationObjects;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators;
using Riskeer.MacroStabilityInwards.Primitives;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.MacroStabilityInwards.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationScenarioContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuDuplicateIndex = 2;
        private const int contextMenuValidateIndex = 6;
        private const int contextMenuCalculateIndex = 7;
        private const int contextMenuClearIndex = 9;

        
        private MacroStabilityInwardsPlugin plugin;
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
        public void Image_Always_ReturnCalculationIcon()
        {
            // Setup
            // Call
            Image image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.SemiProbabilisticCalculationIcon, image);
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnOutputChildNode()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var calculationContext = new MacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                                         new CalculationGroup(),
                                                                                         new[]
                                                                                         {
                                                                                             new MacroStabilityInwardsSurfaceLine(string.Empty)
                                                                                         },
                                                                                         new[]
                                                                                         {
                                                                                             MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel()
                                                                                         },
                                                                                         failureMechanism,
                                                                                         assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(calculationContext).ToArray();

            // Assert
            Assert.AreEqual(3, children.Length);
            var comment = (Comment) children[0];
            Assert.AreSame(calculationContext.WrappedData.Comments, comment);

            var inputContext = (MacroStabilityInwardsInputContext) children[1];
            Assert.AreSame(calculationContext.WrappedData.InputParameters, inputContext.WrappedData);
            CollectionAssert.AreEqual(calculationContext.AvailableMacroStabilityInwardsSurfaceLines, inputContext.AvailableMacroStabilityInwardsSurfaceLines);
            CollectionAssert.AreEqual(calculationContext.AvailableStochasticSoilModels, inputContext.AvailableStochasticSoilModels);

            var outputContext = (MacroStabilityInwardsOutputContext) children[2];
            Assert.AreSame(calculationContext.WrappedData, outputContext.WrappedData);
            Assert.AreSame(calculationContext.FailureMechanism, outputContext.FailureMechanism);
            Assert.AreSame(calculationContext.AssessmentSection, outputContext.AssessmentSection);
        }

        [Test]
        public void ContextMenuStrip_MacroStabilityInwardsCalculationWithoutOutput_ContextMenuItemClearOutputDisabledAndTooltipSet()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var calculation = new MacroStabilityInwardsCalculationScenario();
                var failureMechanism = new MacroStabilityInwardsFailureMechanism();
                var assessmentSection = Substitute.For<IAssessmentSection>();
                var nodeData = new MacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                                   new CalculationGroup(),
                                                                                   Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                   Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                                   failureMechanism,
                                                                                   assessmentSection);

                var gui = Substitute.For<IGui>();
                gui.Get(nodeData, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());
                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  contextMenuClearIndex,
                                                                  "&Wis uitvoer...",
                                                                  "Deze berekening heeft geen uitvoer om te wissen.",
                                                                  RiskeerCommonFormsResources.ClearIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_MacroStabilityInwardsCalculationWithOutput_ContextMenuItemClearOutputEnabled()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var calculation = new MacroStabilityInwardsCalculationScenario
                {
                    Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
                };
                var failureMechanism = new MacroStabilityInwardsFailureMechanism();
                var assessmentSection = Substitute.For<IAssessmentSection>();
                var nodeData = new MacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                                   new CalculationGroup(),
                                                                                   Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                   Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                                   failureMechanism,
                                                                                   assessmentSection);

                var gui = Substitute.For<IGui>();
                gui.Get(nodeData, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());
                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  contextMenuClearIndex,
                                                                  "&Wis uitvoer...",
                                                                  "Wis de uitvoer van deze berekening.",
                                                                  RiskeerCommonFormsResources.ClearIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_AllRequiredInputSet_ContextMenuItemCalculateAndValidateEnabled()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var calculation = new MacroStabilityInwardsCalculationScenario();
                var failureMechanism = new MacroStabilityInwardsFailureMechanism();
                var assessmentSection = Substitute.For<IAssessmentSection>();
                var nodeData = new MacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                                   new CalculationGroup(),
                                                                                   Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                   Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                                   failureMechanism,
                                                                                   assessmentSection);

                var gui = Substitute.For<IGui>();
                gui.Get(nodeData, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());
                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  contextMenuDuplicateIndex,
                                                                  "D&upliceren",
                                                                  "Dupliceer dit element.",
                                                                  RiskeerCommonFormsResources.CopyHS);

                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  contextMenuValidateIndex,
                                                                  "&Valideren",
                                                                  "Valideer de invoer voor deze berekening.",
                                                                  RiskeerCommonFormsResources.ValidateIcon);

                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  contextMenuCalculateIndex,
                                                                  "Be&rekenen",
                                                                  "Voer deze berekening uit.",
                                                                  RiskeerCommonFormsResources.CalculateIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var failureMechanism = new MacroStabilityInwardsFailureMechanism();
                var assessmentSection = Substitute.For<IAssessmentSection>();
                var nodeData = new MacroStabilityInwardsCalculationScenarioContext(new MacroStabilityInwardsCalculationScenario(),
                                                                                   new CalculationGroup(),
                                                                                   Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                   Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                                   failureMechanism,
                                                                                   assessmentSection);

                var menuBuilder = Substitute.For<IContextMenuBuilder>();
                    menuBuilder.AddExportItem().Returns(menuBuilder);
                    menuBuilder.AddSeparator().Returns(menuBuilder);
                    menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>()).Returns(menuBuilder);
                    menuBuilder.AddSeparator().Returns(menuBuilder);
                    menuBuilder.AddRenameItem().Returns(menuBuilder);
                    menuBuilder.AddSeparator().Returns(menuBuilder);
                    menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>()).Returns(menuBuilder);
                    menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>()).Returns(menuBuilder);
                    menuBuilder.AddSeparator().Returns(menuBuilder);
                    menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>()).Returns(menuBuilder);
                    menuBuilder.AddDeleteItem().Returns(menuBuilder);
                    menuBuilder.AddSeparator().Returns(menuBuilder);
                    menuBuilder.AddCollapseAllItem().Returns(menuBuilder);
                    menuBuilder.AddExpandAllItem().Returns(menuBuilder);
                    menuBuilder.AddSeparator().Returns(menuBuilder);
                    menuBuilder.AddPropertiesItem().Returns(menuBuilder);

                var gui = Substitute.For<IGui>();
                gui.Get(nodeData, treeViewControl).Returns(menuBuilder);
                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(nodeData, null, treeViewControl);
                
                // Assert
                Received.InOrder(() =>
                {
                    menuBuilder.AddExportItem();
                    menuBuilder.AddSeparator();
                    menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                    menuBuilder.AddSeparator();
                    menuBuilder.AddRenameItem();
                    menuBuilder.AddSeparator();
                    menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                    menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                    menuBuilder.AddSeparator();
                    menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                    menuBuilder.AddDeleteItem();
                    menuBuilder.AddSeparator();
                    menuBuilder.AddCollapseAllItem();
                    menuBuilder.AddExpandAllItem();
                    menuBuilder.AddSeparator();
                    menuBuilder.AddPropertiesItem();
                    menuBuilder.Build();
                });
            }
        }

        [Test]
        public void OnNodeRemoved_ParentIsCalculationGroupContext_RemoveCalculationFromGroup()
        {
            // Setup
            var observer = Substitute.For<IObserver>();

            var elementToBeRemoved = new MacroStabilityInwardsCalculationScenario();

            var group = new CalculationGroup();
            group.Children.Add(elementToBeRemoved);
            group.Children.Add(new MacroStabilityInwardsCalculationScenario());
            group.Attach(observer);

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var calculationContext = new MacroStabilityInwardsCalculationScenarioContext(elementToBeRemoved,
                                                                                         group,
                                                                                         Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                         Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                                         failureMechanism,
                                                                                         assessmentSection);
            var groupContext = new MacroStabilityInwardsCalculationGroupContext(group,
                                                                                null,
                                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                                failureMechanism,
                                                                                assessmentSection);

            // Precondition
            Assert.IsTrue(info.CanRemove(calculationContext, groupContext));
            Assert.AreEqual(2, group.Children.Count);

            // Call
            info.OnNodeRemoved(calculationContext, groupContext);

            // Assert
            Assert.AreEqual(1, group.Children.Count);
            CollectionAssert.DoesNotContain(group.Children, elementToBeRemoved);
            observer.Received().UpdateObserver();
        }

        [Test]
        public void GivenInvalidCalculation_WhenCalculatingFromContextMenu_ThenCalculationNotifiesObserversAndLogMessageAdded()
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                var calculation = new MacroStabilityInwardsCalculationScenario();
                var failureMechanism = new MacroStabilityInwardsFailureMechanism();
                var assessmentSection = new AssessmentSectionStub();
                var calculationContext = new MacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                                             new CalculationGroup(),
                                                                                             Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                             Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                                             failureMechanism,
                                                                                             assessmentSection);

                IMainWindow mainWindow = MainWindowTestHelper.CreateMainWindowStub();

                var gui = Substitute.For<IGui>();
                gui.MainWindow.Returns(mainWindow);
                gui.Get(calculationContext, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());

                var observer = Substitute.For<IObserver>();
                plugin.Gui = gui;

                calculation.Attach(observer);

                DialogBoxHandler = (name, wnd) =>
                {
                    // Expect an activity dialog which is automatically closed
                };

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(calculationContext, null, treeViewControl))
                {
                    // When
                    void Action() => contextMenuStrip.Items[contextMenuCalculateIndex].PerformClick();

                    // Then
                    const int expectedValidationMessageCount = 3;
                    TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(Action, messages =>
                    {
                        Tuple<string, Level, Exception>[] tupleArray = messages.ToArray();
                        string[] msgs = tupleArray.Select(tuple => tuple.Item1).ToArray();

                        Assert.AreEqual($"Uitvoeren van berekening '{calculation.Name}' is gestart.", msgs[0]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                        for (var i = 0; i < expectedValidationMessageCount; i++)
                        {
                            Assert.AreEqual(Level.Error, tupleArray[2 + i].Item2);
                        }

                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs[5]);
                        Assert.AreEqual($"Uitvoeren van berekening '{calculation.Name}' is mislukt.", msgs[6]);
                    });
                    Assert.IsNull(calculation.Output);
                    observer.Received().UpdateObserver();
                }
            }
        }

        [Test]
        public void GivenInvalidCalculation_WhenValidatingFromContextMenu_ThenLogMessageAddedAndNoNotifyObserver()
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                var calculation = new MacroStabilityInwardsCalculationScenario();
                var failureMechanism = new MacroStabilityInwardsFailureMechanism();
                var assessmentSection = new AssessmentSectionStub();

                var calculationContext = new MacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                                             new CalculationGroup(),
                                                                                             Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                             Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                                             failureMechanism,
                                                                                             assessmentSection);

                var gui = Substitute.For<IGui>();
                gui.Get(calculationContext, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());

                var observer = Substitute.For<IObserver>();
                plugin.Gui = gui;

                calculation.Attach(observer);

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(calculationContext, null, treeViewControl))
                using (new MacroStabilityInwardsCalculatorFactoryConfig())
                {
                    // When
                    void Action() => contextMenuStrip.Items[contextMenuValidateIndex].PerformClick();

                    // Then
                    const int expectedValidationMessageCount = 3;
                    const int expectedStatusMessageCount = 2;
                    const int expectedLogMessageCount = expectedValidationMessageCount + expectedStatusMessageCount;
                    TestHelper.AssertLogMessagesCount(Action, expectedLogMessageCount);
                }
                observer.DidNotReceive().UpdateObserver();
            }
        }

        [Test]
        public void GivenValidCalculation_WhenCalculatingFromContextMenu_ThenCalculationNotifiesObservers()
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                var failureMechanism = new MacroStabilityInwardsFailureMechanism();
                var assessmentSection = new AssessmentSectionStub();
                var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

                assessmentSection.AddHydraulicBoundaryLocationCalculations(new[]
                {
                    hydraulicBoundaryLocation
                }, true);

                MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(hydraulicBoundaryLocation);

                var calculationContext = new MacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                                             new CalculationGroup(),
                                                                                             Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                             Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                                             failureMechanism,
                                                                                             assessmentSection);

                IMainWindow mainWindow = MainWindowTestHelper.CreateMainWindowStub();

                var gui = Substitute.For<IGui>();
                gui.MainWindow.Returns(mainWindow);
                gui.Get(calculationContext, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());

                var observer = Substitute.For<IObserver>();
                plugin.Gui = gui;

                calculation.Attach(observer);

                DialogBoxHandler = (name, wnd) =>
                {
                    // Expect an activity dialog which is automatically closed
                };

                using (new MacroStabilityInwardsCalculatorFactoryConfig())
                using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(calculationContext, null, treeViewControl))
                {
                    // When
                    void Action() => contextMenuAdapter.Items[contextMenuCalculateIndex].PerformClick();

                    // Then
                    TestHelper.AssertLogMessages(Action, messages =>
                    {
                        using (IEnumerator<string> msgs = messages.GetEnumerator())
                        {
                            Assert.IsTrue(msgs.MoveNext());
                            Assert.AreEqual($"Uitvoeren van berekening '{calculation.Name}' is gestart.", msgs.Current);
                            Assert.IsTrue(msgs.MoveNext());
                            CalculationServiceTestHelper.AssertValidationStartMessage(msgs.Current);
                            Assert.IsTrue(msgs.MoveNext());
                            Assert.AreEqual("Validatie van waterspanningen in extreme omstandigheden is gestart.", msgs.Current);
                            Assert.IsTrue(msgs.MoveNext());
                            Assert.AreEqual("Validatie van waterspanningen in dagelijkse omstandigheden is gestart.", msgs.Current);
                            Assert.IsTrue(msgs.MoveNext());
                            CalculationServiceTestHelper.AssertValidationEndMessage(msgs.Current);
                            Assert.IsTrue(msgs.MoveNext());
                            CalculationServiceTestHelper.AssertCalculationStartMessage(msgs.Current);
                            Assert.IsTrue(msgs.MoveNext());
                            CalculationServiceTestHelper.AssertCalculationEndMessage(msgs.Current);
                            Assert.IsTrue(msgs.MoveNext());
                            Assert.AreEqual($"Uitvoeren van berekening '{calculation.Name}' is gelukt.", msgs.Current);
                        }
                    });
                    Assert.IsNotNull(calculation.Output);
                }
                observer.Received().UpdateObserver();
            }
        }

        [Test]
        [TestCase(true, TestName = "GivenCalculation_WhenClearingOutputFromContextMenu_ThenOutputClearedAndNotified(true)")]
        [TestCase(false, TestName = "GivenCalculation_WhenClearingOutputFromContextMenu_ThenOutputClearedAndNotified(false)")]
        public void GivenCalculationWithOutput_WhenClearingOutputFromContextMenu_ThenCalculationOutputClearedAndNotified(bool confirm)
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                var calculation = new MacroStabilityInwardsCalculationScenario();
                var failureMechanism = new MacroStabilityInwardsFailureMechanism();
                var assessmentSection = Substitute.For<IAssessmentSection>();

                var calculationContext = new MacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                                             new CalculationGroup(),
                                                                                             Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                             Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                                             failureMechanism,
                                                                                             assessmentSection);

                var gui = Substitute.For<IGui>();
                gui.Get(calculationContext, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());

                var observer = Substitute.For<IObserver>();

                plugin.Gui = gui;

                calculation.Output = MacroStabilityInwardsOutputTestFactory.CreateOutput();
                calculation.Attach(observer);

                string messageBoxText = null, messageBoxTitle = null;
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

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(calculationContext, null, treeViewControl))
                {
                    // When
                    contextMenuStrip.Items[contextMenuClearIndex].PerformClick();

                    // Then
                    Assert.AreNotEqual(confirm, calculation.HasOutput);
                    Assert.AreEqual("Bevestigen", messageBoxTitle);
                    Assert.AreEqual("Weet u zeker dat u de uitvoer van deze berekening wilt wissen?", messageBoxText);
                }
                if (confirm)
                {
                    observer.Received().UpdateObserver();
                }
                else
                {
                    observer.DidNotReceive().UpdateObserver();
                }
            }
        }

        public override void Setup()
        {
            
            plugin = new MacroStabilityInwardsPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(MacroStabilityInwardsCalculationScenarioContext));
        }

        public override void TearDown()
        {
            plugin.Dispose();
            base.TearDown();
        }
    }
}