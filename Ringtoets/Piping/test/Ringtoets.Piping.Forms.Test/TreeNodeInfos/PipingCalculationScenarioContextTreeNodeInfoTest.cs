// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Plugin;
using Ringtoets.Piping.Primitives;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Piping.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class PipingCalculationScenarioContextTreeNodeInfoTest : NUnitFormTest
    {
        private MockRepository mocks;
        private PipingPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new PipingPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(PipingCalculationScenarioContext));
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            mocks.ReplayAll();

            // Assert
            Assert.AreEqual(typeof(PipingCalculationScenarioContext), info.TagType);
            Assert.IsNull(info.ForeColor);
            Assert.IsNull(info.CanCheck);
            Assert.IsNull(info.IsChecked);
            Assert.IsNull(info.OnNodeChecked);
            Assert.IsNull(info.CanDrop);
            Assert.IsNull(info.CanInsert);
            Assert.IsNull(info.OnDrop);
        }

        [Test]
        public void Image_Always_ReturnsPipingIcon()
        {
            // Setup
            mocks.ReplayAll();

            // Call
            var image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.CalculationIcon, image);
        }

        [Test]
        public void ChildNodeObjects_WithOutputData_ReturnOutputChildNode()
        {
            // Setup
            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                Output = new TestPipingOutput(),
                SemiProbabilisticOutput = new TestPipingSemiProbabilisticOutput()
            };

            var pipingFailureMechanism = new PipingFailureMechanism();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var pipingCalculationContext = new PipingCalculationScenarioContext(calculation,
                                                                                new[]
                                                                                {
                                                                                    new RingtoetsPipingSurfaceLine()
                                                                                },
                                                                                new[]
                                                                                {
                                                                                    new TestStochasticSoilModel()
                                                                                },
                                                                                pipingFailureMechanism,
                                                                                assessmentSectionMock);

            // Call
            var children = info.ChildNodeObjects(pipingCalculationContext).ToArray();

            // Assert
            Assert.AreEqual(3, children.Length);
            var commentContext = children[0] as CommentContext<ICommentable>;
            Assert.IsNotNull(commentContext);
            Assert.AreSame(pipingCalculationContext.WrappedData, commentContext.WrappedData);

            var pipingInputContext = (PipingInputContext) children[1];
            Assert.AreSame(pipingCalculationContext.WrappedData.InputParameters, pipingInputContext.WrappedData);
            CollectionAssert.AreEqual(pipingCalculationContext.AvailablePipingSurfaceLines, pipingInputContext.AvailablePipingSurfaceLines);
            CollectionAssert.AreEqual(pipingCalculationContext.AvailableStochasticSoilModels, pipingInputContext.AvailableStochasticSoilModels);
            Assert.AreSame(pipingCalculationContext.WrappedData.SemiProbabilisticOutput, children[2]);
        }

        [Test]
        public void ChildNodeObjects_WithoutOutput_ReturnNoChildNodes()
        {
            // Setup
            var pipingFailureMechanism = new PipingFailureMechanism();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var pipingCalculationContext = new PipingCalculationScenarioContext(new PipingCalculationScenario(new GeneralPipingInput()),
                                                                                Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                Enumerable.Empty<StochasticSoilModel>(),
                                                                                pipingFailureMechanism,
                                                                                assessmentSectionMock);

            // Precondition
            Assert.IsFalse(pipingCalculationContext.WrappedData.HasOutput);

            // Call
            var children = info.ChildNodeObjects(pipingCalculationContext).ToArray();

            // Assert
            Assert.AreEqual(3, children.Length);
            var commentContext = children[0] as CommentContext<ICommentable>;
            Assert.IsNotNull(commentContext);
            Assert.AreSame(pipingCalculationContext.WrappedData, commentContext.WrappedData);

            var pipingInputContext = (PipingInputContext) children[1];
            Assert.AreSame(pipingCalculationContext.WrappedData.InputParameters, pipingInputContext.WrappedData);
            CollectionAssert.AreEqual(pipingCalculationContext.AvailablePipingSurfaceLines, pipingInputContext.AvailablePipingSurfaceLines);
            CollectionAssert.AreEqual(pipingCalculationContext.AvailableStochasticSoilModels, pipingInputContext.AvailableStochasticSoilModels);

            Assert.IsInstanceOf<EmptyPipingOutput>(children[2]);
        }

        [Test]
        public void ContextMenuStrip_PipingCalculationWithoutOutput_ContextMenuItemClearOutputDisabled()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var calculation = new PipingCalculationScenario(new GeneralPipingInput());
                var pipingFailureMechanism = new PipingFailureMechanism();
                pipingFailureMechanism.AddSection(new FailureMechanismSection("A", new[]
                {
                    new Point2D(0, 0)
                }));
                var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

                var nodeData = new PipingCalculationScenarioContext(calculation,
                                                                    Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                    Enumerable.Empty<StochasticSoilModel>(),
                                                                    pipingFailureMechanism,
                                                                    assessmentSectionMock);

                var gui = mocks.StrictMock<IGui>();
                gui.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());

                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, 0, RingtoetsCommonFormsResources.Validate, RingtoetsCommonFormsResources.Validate_ToolTip, RingtoetsCommonFormsResources.ValidateIcon);
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, 1, RingtoetsCommonFormsResources.Calculate, RingtoetsCommonFormsResources.Calculate_ToolTip, RingtoetsCommonFormsResources.CalculateIcon);
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, 2, RingtoetsCommonFormsResources.Clear_output, RingtoetsCommonFormsResources.ClearOutput_No_output_to_clear, RingtoetsCommonFormsResources.ClearIcon, false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_PipingCalculationWithOutput_ContextMenuItemClearOutputEnabled()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var calculation = new PipingCalculationScenario(new GeneralPipingInput())
                {
                    Output = new TestPipingOutput(),
                    SemiProbabilisticOutput = new TestPipingSemiProbabilisticOutput()
                };
                var pipingFailureMechanism = new PipingFailureMechanism();
                pipingFailureMechanism.AddSection(new FailureMechanismSection("A", new[]
                {
                    new Point2D(0, 0)
                }));
                var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

                var nodeData = new PipingCalculationScenarioContext(calculation,
                                                                    Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                    Enumerable.Empty<StochasticSoilModel>(),
                                                                    pipingFailureMechanism,
                                                                    assessmentSectionMock);

                var gui = mocks.StrictMock<IGui>();
                gui.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());

                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, 0, RingtoetsCommonFormsResources.Validate, RingtoetsCommonFormsResources.Validate_ToolTip, RingtoetsCommonFormsResources.ValidateIcon);
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, 1, RingtoetsCommonFormsResources.Calculate, RingtoetsCommonFormsResources.Calculate_ToolTip, RingtoetsCommonFormsResources.CalculateIcon);
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, 2, RingtoetsCommonFormsResources.Clear_output, RingtoetsCommonFormsResources.Clear_output_ToolTip, RingtoetsCommonFormsResources.ClearIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var pipingFailureMechanism = new PipingFailureMechanism();
                var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
                var nodeData = new PipingCalculationScenarioContext(new PipingCalculationScenario(new GeneralPipingInput()),
                                                                    Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                    Enumerable.Empty<StochasticSoilModel>(),
                                                                    pipingFailureMechanism,
                                                                    assessmentSectionMock);

                var menuBuilderMock = mocks.Stub<IContextMenuBuilder>();
                menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddRenameItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddDeleteItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.Build()).Return(null);

                var gui = mocks.StrictMock<IGui>();
                gui.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilderMock);

                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(nodeData, null, treeViewControl);
            }

            // Assert
            // Assert expectancies are called in TearDown()
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void OnNodeRemoved_ParentIsPipingCalculationGroupContext_RemoveCalculationFromGroup(bool groupNameEditable)
        {
            // Setup
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var elementToBeRemoved = new PipingCalculationScenario(new GeneralPipingInput());

            var group = new CalculationGroup("", groupNameEditable);
            group.Children.Add(elementToBeRemoved);
            group.Children.Add(new PipingCalculationScenario(new GeneralPipingInput()));
            group.Attach(observer);

            var pipingFailureMechanism = new PipingFailureMechanism();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationContext = new PipingCalculationScenarioContext(elementToBeRemoved,
                                                                          Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                          Enumerable.Empty<StochasticSoilModel>(),
                                                                          pipingFailureMechanism,
                                                                          assessmentSectionMock);
            var groupContext = new PipingCalculationGroupContext(group,
                                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                 Enumerable.Empty<StochasticSoilModel>(),
                                                                 pipingFailureMechanism,
                                                                 assessmentSectionMock);

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
        [TestCase(true)]
        [TestCase(false)]
        public void OnNodeRemoved_ParentIsPipingCalculationGroupContext_RemoveCalculationFromSectionResult(bool groupNameEditable)
        {
            // Setup
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var pipingFailureMechanism = GetFailureMechanism();
            var surfaceLines = pipingFailureMechanism.SurfaceLines.ToArray();

            var elementToBeRemoved = new PipingCalculationScenario(new GeneralPipingInput())
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLines[0]
                }
            };

            var group = new CalculationGroup("", groupNameEditable);
            group.Children.Add(elementToBeRemoved);
            group.Children.Add(new PipingCalculationScenario(new GeneralPipingInput()));
            group.Attach(observer);
            pipingFailureMechanism.CalculationsGroup.Children.Add(group);

            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationContext = new PipingCalculationScenarioContext(elementToBeRemoved,
                                                                          Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                          Enumerable.Empty<StochasticSoilModel>(),
                                                                          pipingFailureMechanism,
                                                                          assessmentSectionMock);
            var groupContext = new PipingCalculationGroupContext(group,
                                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                 Enumerable.Empty<StochasticSoilModel>(),
                                                                 pipingFailureMechanism,
                                                                 assessmentSectionMock);

            // Precondition
            Assert.IsTrue(info.CanRemove(calculationContext, groupContext));
            Assert.AreEqual(2, group.Children.Count);
            var sectionResults = pipingFailureMechanism.SectionResults.ToArray();
            CollectionAssert.Contains(sectionResults[0].GetCalculationScenarios(pipingFailureMechanism.Calculations.OfType<PipingCalculationScenario>()), elementToBeRemoved);

            // Call
            info.OnNodeRemoved(calculationContext, groupContext);

            // Assert
            Assert.AreEqual(1, group.Children.Count);
            CollectionAssert.DoesNotContain(group.Children, elementToBeRemoved);
            CollectionAssert.DoesNotContain(sectionResults[0].GetCalculationScenarios(pipingFailureMechanism.Calculations.OfType<PipingCalculationScenario>()), elementToBeRemoved);
        }

        [Test]
        public void GivenInvalidPipingCalculation_WhenCalculatingFromContextMenu_ThenPipingCalculationNotifiesObserversAndLogMessageAdded()
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                var calculation = new PipingCalculationScenario(new GeneralPipingInput());
                var pipingFailureMechanism = new PipingFailureMechanism();
                pipingFailureMechanism.AddSection(new FailureMechanismSection("A", new[]
                {
                    new Point2D(0, 0)
                }));
                var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
                assessmentSectionMock.Stub(s => s.FailureMechanismContribution).Return(new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 30, 20000));

                var pipingCalculationContext = new PipingCalculationScenarioContext(calculation,
                                                                                    Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                    Enumerable.Empty<StochasticSoilModel>(),
                                                                                    pipingFailureMechanism,
                                                                                    assessmentSectionMock);

                var mainWindow = mocks.DynamicMock<IMainWindow>();

                var gui = mocks.DynamicMock<IGui>();
                gui.Expect(cmp => cmp.Get(pipingCalculationContext, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Expect(g => g.MainWindow).Return(mainWindow);

                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());

                mocks.ReplayAll();

                plugin.Gui = gui;

                calculation.Attach(observer);

                DialogBoxHandler = (name, wnd) =>
                {
                    // Expect an activity dialog which is automatically closed
                };

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(pipingCalculationContext, null, treeViewControl))
                {
                    // When
                    var calculateContextMenuItemIndex = 1;
                    Action action = () => contextMenuStrip.Items[calculateContextMenuItemIndex].PerformClick(); 

                    // Then
                    var expectedValidationMessageCount = 5;
                    TestHelper.AssertLogMessages(action, messages =>
                    {
                        var msgs = messages.GetEnumerator();
                        Assert.IsTrue(msgs.MoveNext());
                        StringAssert.StartsWith("Validatie van 'Nieuwe berekening' gestart om: ", msgs.Current);
                        for (int i = 0; i < expectedValidationMessageCount; i++)
                        {
                            Assert.IsTrue(msgs.MoveNext());
                            StringAssert.StartsWith("Validatie mislukt: ", msgs.Current);
                        }
                        Assert.IsTrue(msgs.MoveNext());
                        StringAssert.StartsWith("Validatie van 'Nieuwe berekening' beëindigd om: ", msgs.Current);
                    });
                    Assert.IsNull(calculation.Output);
                    Assert.IsNull(calculation.SemiProbabilisticOutput);
                }
            }
        }

        [Test]
        public void GivenInvalidPipingCalculation_WhenValidatingFromContextMenu_ThenLogMessageAddedAndNoNotifyObserver()
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                var calculation = new PipingCalculationScenario(new GeneralPipingInput());
                var pipingFailureMechanism = new PipingFailureMechanism();
                var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

                var pipingCalculationContext = new PipingCalculationScenarioContext(calculation,
                                                                                    Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                    Enumerable.Empty<StochasticSoilModel>(),
                                                                                    pipingFailureMechanism,
                                                                                    assessmentSectionMock);

                var gui = mocks.DynamicMock<IGui>();
                gui.Expect(cmp => cmp.Get(pipingCalculationContext, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());

                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver()).Repeat.Never();

                mocks.ReplayAll();

                plugin.Gui = gui;

                calculation.Attach(observer);

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(pipingCalculationContext, null, treeViewControl))
                {
                    // When
                    var validateContextMenuItemIndex = 0;
                    Action action = () => contextMenuStrip.Items[validateContextMenuItemIndex].PerformClick();

                    // Then
                    var expectedValidationMessageCount = 5;
                    var expectedStatusMessageCount = 2;
                    var expectedLogMessageCount = expectedValidationMessageCount + expectedStatusMessageCount;
                    TestHelper.AssertLogMessagesCount(action, expectedLogMessageCount);
                }
            }
        }

        [Test]
        public void GivenValidPipingCalculation_WhenCalculatingFromContextMenu_ThenPipingCalculationNotifiesObservers()
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                PipingCalculationScenario calculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
                var pipingFailureMechanism = new PipingFailureMechanism();
                pipingFailureMechanism.AddSection(new FailureMechanismSection("A", new[]
                {
                    new Point2D(0, 0)
                }));
                var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
                assessmentSectionMock.Stub(s => s.FailureMechanismContribution).Return(new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 30, 20000));

                var pipingCalculationContext = new PipingCalculationScenarioContext(calculation,
                                                                                    Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                    Enumerable.Empty<StochasticSoilModel>(),
                                                                                    pipingFailureMechanism,
                                                                                    assessmentSectionMock);

                var mainWindow = mocks.DynamicMock<IMainWindow>();

                var gui = mocks.DynamicMock<IGui>();
                gui.Expect(g => g.Get(pipingCalculationContext, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Expect(g => g.MainWindow).Return(mainWindow);

                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());

                mocks.ReplayAll();

                plugin.Gui = gui;

                calculation.Attach(observer);

                DialogBoxHandler = (name, wnd) =>
                {
                    // Expect an activity dialog which is automatically closed
                };

                using (var contextMenuAdapter = info.ContextMenuStrip(pipingCalculationContext, null, treeViewControl))
                {
                    // When
                    var calculateContextMenuItemIndex = 1;
                    Action action = () => contextMenuAdapter.Items[calculateContextMenuItemIndex].PerformClick();

                    // Then
                    TestHelper.AssertLogMessages(action, messages =>
                    {
                        var msgs = messages.GetEnumerator();
                        Assert.IsTrue(msgs.MoveNext());
                        StringAssert.StartsWith("Validatie van 'Nieuwe berekening' gestart om: ", msgs.Current);
                        Assert.IsTrue(msgs.MoveNext());
                        StringAssert.StartsWith("Validatie van 'Nieuwe berekening' beëindigd om: ", msgs.Current);

                        Assert.IsTrue(msgs.MoveNext());
                        StringAssert.StartsWith("Berekening van 'Nieuwe berekening' gestart om: ", msgs.Current);
                        Assert.IsTrue(msgs.MoveNext());
                        StringAssert.StartsWith("Berekening van 'Nieuwe berekening' beëindigd om: ", msgs.Current);
                    });
                    Assert.IsNotNull(calculation.Output);
                    Assert.IsNotNull(calculation.SemiProbabilisticOutput);
                }
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenPipingCalculationWithOutput_WhenClearingOutputFromContextMenu_ThenPipingCalculationOutputClearedAndNotified(bool confirm)
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                var calculation = new PipingCalculationScenario(new GeneralPipingInput());
                var pipingFailureMechanism = new PipingFailureMechanism();
                var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

                var pipingCalculationContext = new PipingCalculationScenarioContext(calculation,
                                                                                    Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                    Enumerable.Empty<StochasticSoilModel>(),
                                                                                    pipingFailureMechanism,
                                                                                    assessmentSectionMock);

                var gui = mocks.DynamicMock<IGui>();
                gui.Expect(cmp => cmp.Get(pipingCalculationContext, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());

                var observer = mocks.StrictMock<IObserver>();
                if (confirm)
                {
                    observer.Expect(o => o.UpdateObserver());
                }

                mocks.ReplayAll();

                plugin.Gui = gui;

                calculation.Output = new TestPipingOutput();
                calculation.SemiProbabilisticOutput = new TestPipingSemiProbabilisticOutput();
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

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(pipingCalculationContext, null, treeViewControl))
                {
                    // When
                    int clearOutputItemPosition = 2;
                    contextMenuStrip.Items[clearOutputItemPosition].PerformClick();

                    // Then
                    Assert.AreNotEqual(confirm, calculation.HasOutput);
                    Assert.AreEqual("Bevestigen", messageBoxTitle);
                    Assert.AreEqual("Weet u zeker dat u de uitvoer van deze berekening wilt wissen?", messageBoxText);
                }
            }
        }

        public override void TearDown()
        {
            plugin.Dispose();
            mocks.VerifyAll();

            base.TearDown();
        }

        /// <summary>
        /// Creates a new instance of <see cref="PipingFailureMechanism"/> with sections and a surface line.
        /// </summary>
        /// <returns>A new instance of <see cref="PipingFailureMechanism"/>.</returns>
        private static PipingFailureMechanism GetFailureMechanism()
        {
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "Surface line",
                ReferenceLineIntersectionWorldPoint = new Point2D(0.0, 0.0)
            };

            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0.0, 5.0, 0.0),
                new Point3D(0.0, 0.0, 1.0),
                new Point3D(0.0, -5.0, 0.0)
            });

            var failureMechanism = new PipingFailureMechanism
            {
                SurfaceLines =
                {
                    surfaceLine
                },
                StochasticSoilModels =
                {
                    new TestStochasticSoilModel
                    {
                        Geometry =
                        {
                            new Point2D(0.0, 0.0), new Point2D(5.0, 0.0)
                        }
                    }
                }
            };

            failureMechanism.AddSection(new FailureMechanismSection("Section", new List<Point2D>
            {
                new Point2D(0.0, 0.0),
                new Point2D(5.0, 0.0)
            }));

            return failureMechanism;
        }
    }
}