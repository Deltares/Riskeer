// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using log4net.Core;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Forms.PresentationObjects;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators;
using Ringtoets.MacroStabilityInwards.Primitives;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.MacroStabilityInwards.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationScenarioContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuDuplicateIndex = 2;
        private const int contextMenuValidateIndex = 6;
        private const int contextMenuCalculateIndex = 7;
        private const int contextMenuClearIndex = 9;

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
            mocks.ReplayAll();

            // Call
            Image image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.CalculationIcon, image);
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnOutputChildNode()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

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
                var failureMechanism = new TestMacroStabilityInwardsFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var nodeData = new MacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                                   new CalculationGroup(),
                                                                                   Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                   Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                                   failureMechanism,
                                                                                   assessmentSection);

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  contextMenuClearIndex,
                                                                  "&Wis uitvoer...",
                                                                  "Deze berekening heeft geen uitvoer om te wissen.",
                                                                  RingtoetsCommonFormsResources.ClearIcon,
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
                var failureMechanism = new TestMacroStabilityInwardsFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var nodeData = new MacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                                   new CalculationGroup(),
                                                                                   Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                   Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                                   failureMechanism,
                                                                                   assessmentSection);

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  contextMenuClearIndex,
                                                                  "&Wis uitvoer...",
                                                                  "Wis de uitvoer van deze berekening.",
                                                                  RingtoetsCommonFormsResources.ClearIcon);
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
                var failureMechanism = new TestMacroStabilityInwardsFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var nodeData = new MacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                                   new CalculationGroup(),
                                                                                   Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                   Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                                   failureMechanism,
                                                                                   assessmentSection);

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  contextMenuDuplicateIndex,
                                                                  "D&upliceren",
                                                                  "Dupliceer dit element.",
                                                                  RingtoetsCommonFormsResources.CopyHS);

                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  contextMenuValidateIndex,
                                                                  "&Valideren",
                                                                  "Valideer de invoer voor deze berekening.",
                                                                  RingtoetsCommonFormsResources.ValidateIcon);

                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  contextMenuCalculateIndex,
                                                                  "Be&rekenen",
                                                                  "Voer deze berekening uit.",
                                                                  RingtoetsCommonFormsResources.CalculateIcon);
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
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var nodeData = new MacroStabilityInwardsCalculationScenarioContext(new MacroStabilityInwardsCalculationScenario(),
                                                                                   new CalculationGroup(),
                                                                                   Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                   Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                                   failureMechanism,
                                                                                   assessmentSection);

                var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();
                using (mocks.Ordered())
                {
                    menuBuilder.Expect(mb => mb.AddExportItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddRenameItem()).Return(menuBuilder);
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

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(nodeData, null, treeViewControl);
            }

            // Assert
            // Assert expectancies are called in TearDown()
        }

        [Test]
        public void OnNodeRemoved_ParentIsCalculationGroupContext_RemoveCalculationFromGroup()
        {
            // Setup
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var elementToBeRemoved = new MacroStabilityInwardsCalculationScenario();

            var group = new CalculationGroup();
            group.Children.Add(elementToBeRemoved);
            group.Children.Add(new MacroStabilityInwardsCalculationScenario());
            group.Attach(observer);

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

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
        }

        [Test]
        public void OnNodeRemoved_ParentIsCalculationGroupContext_RemoveCalculationFromSectionResult()
        {
            // Setup
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            TestMacroStabilityInwardsFailureMechanism macroStabilityInwardsFailureMechanism = TestMacroStabilityInwardsFailureMechanism.GetFailureMechanismWithSurfaceLinesAndStochasticSoilModels();
            MacroStabilityInwardsSurfaceLine[] surfaceLines = macroStabilityInwardsFailureMechanism.SurfaceLines.ToArray();

            var elementToBeRemoved = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLines[0]
                }
            };

            var group = new CalculationGroup();
            group.Children.Add(elementToBeRemoved);
            group.Children.Add(new MacroStabilityInwardsCalculationScenario());
            group.Attach(observer);
            macroStabilityInwardsFailureMechanism.CalculationsGroup.Children.Add(group);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationContext = new MacroStabilityInwardsCalculationScenarioContext(elementToBeRemoved,
                                                                                         group,
                                                                                         Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                         Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                                         macroStabilityInwardsFailureMechanism,
                                                                                         assessmentSection);
            var groupContext = new MacroStabilityInwardsCalculationGroupContext(group,
                                                                                null,
                                                                                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                                macroStabilityInwardsFailureMechanism,
                                                                                assessmentSection);

            // Precondition
            Assert.IsTrue(info.CanRemove(calculationContext, groupContext));
            Assert.AreEqual(2, group.Children.Count);
            MacroStabilityInwardsFailureMechanismSectionResult[] sectionResults = macroStabilityInwardsFailureMechanism.SectionResults.ToArray();
            CollectionAssert.Contains(sectionResults[0].GetCalculationScenarios(macroStabilityInwardsFailureMechanism.Calculations.OfType<MacroStabilityInwardsCalculationScenario>()), elementToBeRemoved);

            // Call
            info.OnNodeRemoved(calculationContext, groupContext);

            // Assert
            Assert.AreEqual(1, group.Children.Count);
            CollectionAssert.DoesNotContain(group.Children, elementToBeRemoved);
            CollectionAssert.DoesNotContain(sectionResults[0].GetCalculationScenarios(macroStabilityInwardsFailureMechanism.Calculations.OfType<MacroStabilityInwardsCalculationScenario>()), elementToBeRemoved);
        }

        [Test]
        public void GivenInvalidCalculation_WhenCalculatingFromContextMenu_ThenCalculationNotifiesObserversAndLogMessageAdded()
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                var calculation = new MacroStabilityInwardsCalculationScenario();
                var failureMechanism = new TestMacroStabilityInwardsFailureMechanism();
                var assessmentSection = new AssessmentSectionStub();
                var calculationContext = new MacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                                             new CalculationGroup(),
                                                                                             Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                             Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                                             failureMechanism,
                                                                                             assessmentSection);

                var mainWindow = mocks.DynamicMock<IMainWindow>();

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(calculationContext, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mainWindow);

                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());

                mocks.ReplayAll();

                plugin.Gui = gui;

                calculation.Attach(observer);

                DialogBoxHandler = (name, wnd) =>
                {
                    // Expect an activity dialog which is automatically closed
                };

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(calculationContext, null, treeViewControl))
                {
                    // When
                    Action action = () => contextMenuStrip.Items[contextMenuCalculateIndex].PerformClick();

                    // Then
                    const int expectedValidationMessageCount = 3;
                    TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(action, messages =>
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
                var failureMechanism = new TestMacroStabilityInwardsFailureMechanism();
                var assessmentSection = new AssessmentSectionStub();

                var calculationContext = new MacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                                             new CalculationGroup(),
                                                                                             Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                             Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                                             failureMechanism,
                                                                                             assessmentSection);

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(calculationContext, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());

                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver()).Repeat.Never();

                mocks.ReplayAll();

                plugin.Gui = gui;

                calculation.Attach(observer);

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(calculationContext, null, treeViewControl))
                using (new MacroStabilityInwardsCalculatorFactoryConfig())
                {
                    // When
                    Action action = () => contextMenuStrip.Items[contextMenuValidateIndex].PerformClick();

                    // Then
                    const int expectedValidationMessageCount = 3;
                    const int expectedStatusMessageCount = 2;
                    const int expectedLogMessageCount = expectedValidationMessageCount + expectedStatusMessageCount;
                    TestHelper.AssertLogMessagesCount(action, expectedLogMessageCount);
                }
            }
        }

        [Test]
        public void GivenValidCalculation_WhenCalculatingFromContextMenu_ThenCalculationNotifiesObservers()
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                var failureMechanism = new TestMacroStabilityInwardsFailureMechanism();
                var assessmentSection = new AssessmentSectionStub();
                var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

                assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
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

                var mainWindow = mocks.DynamicMock<IMainWindow>();

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(calculationContext, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mainWindow);

                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());
                mocks.ReplayAll();

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
                    Action action = () => contextMenuAdapter.Items[contextMenuCalculateIndex].PerformClick();

                    // Then
                    TestHelper.AssertLogMessages(action, messages =>
                    {
                        using (IEnumerator<string> msgs = messages.GetEnumerator())
                        {
                            Assert.IsTrue(msgs.MoveNext());
                            Assert.AreEqual($"Uitvoeren van berekening '{calculation.Name}' is gestart.", msgs.Current);
                            Assert.IsTrue(msgs.MoveNext());
                            CalculationServiceTestHelper.AssertValidationStartMessage(msgs.Current);
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
                var assessmentSection = mocks.Stub<IAssessmentSection>();

                var calculationContext = new MacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                                             new CalculationGroup(),
                                                                                             Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                                                                                             Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                                                                                             failureMechanism,
                                                                                             assessmentSection);

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(calculationContext, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());

                var observer = mocks.StrictMock<IObserver>();
                if (confirm)
                {
                    observer.Expect(o => o.UpdateObserver());
                }

                mocks.ReplayAll();

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
            }
        }

        public override void Setup()
        {
            mocks = new MockRepository();
            plugin = new MacroStabilityInwardsPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(MacroStabilityInwardsCalculationScenarioContext));
        }

        public override void TearDown()
        {
            plugin.Dispose();
            mocks.VerifyAll();

            base.TearDown();
        }
    }
}