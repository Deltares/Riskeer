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
        private PipingGuiPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new PipingGuiPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(PipingCalculationScenarioContext));
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
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
            // Call
            var image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(PipingFormsResources.PipingIcon, image);
        }

        [Test]
        public void ChildNodeObjects_WithOutputData_ReturnOutputChildNode()
        {
            // Setup
            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                Output = new PipingOutput(0.0, 0.0, 0.0, 0.0, 0.0, 0.0)
            };

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
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
                                                                                pipingFailureMechanismMock,
                                                                                assessmentSectionMock);

            // Call
            var children = info.ChildNodeObjects(pipingCalculationContext).ToArray();

            // Assert
            Assert.AreEqual(4, children.Length);
            var commentContext = children[0] as CommentContext<ICommentable>;
            Assert.IsNotNull(commentContext);
            Assert.AreSame(pipingCalculationContext.WrappedData, commentContext.CommentContainer);

            var pipingInputContext = (PipingInputContext) children[1];
            Assert.AreSame(pipingCalculationContext.WrappedData.InputParameters, pipingInputContext.WrappedData);
            CollectionAssert.AreEqual(pipingCalculationContext.AvailablePipingSurfaceLines, pipingInputContext.AvailablePipingSurfaceLines);
            CollectionAssert.AreEqual(pipingCalculationContext.AvailableStochasticSoilModels, pipingInputContext.AvailableStochasticSoilModels);
            Assert.AreSame(pipingCalculationContext.WrappedData.SemiProbabilisticOutput, children[2]);
            Assert.IsInstanceOf<EmptyPipingCalculationReport>(children[3]);
        }

        [Test]
        public void ChildNodeObjects_WithoutOutput_ReturnNoChildNodes()
        {
            // Setup
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var pipingCalculationContext = new PipingCalculationScenarioContext(new PipingCalculationScenario(new GeneralPipingInput()),
                                                                                Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                Enumerable.Empty<StochasticSoilModel>(),
                                                                                pipingFailureMechanismMock,
                                                                                assessmentSectionMock);

            // Precondition
            Assert.IsFalse(pipingCalculationContext.WrappedData.HasOutput);

            // Call
            var children = info.ChildNodeObjects(pipingCalculationContext).ToArray();

            // Assert
            Assert.AreEqual(4, children.Length);
            var commentContext = children[0] as CommentContext<ICommentable>;
            Assert.IsNotNull(commentContext);
            Assert.AreSame(pipingCalculationContext.WrappedData, commentContext.CommentContainer);

            var pipingInputContext = (PipingInputContext) children[1];
            Assert.AreSame(pipingCalculationContext.WrappedData.InputParameters, pipingInputContext.WrappedData);
            CollectionAssert.AreEqual(pipingCalculationContext.AvailablePipingSurfaceLines, pipingInputContext.AvailablePipingSurfaceLines);
            CollectionAssert.AreEqual(pipingCalculationContext.AvailableStochasticSoilModels, pipingInputContext.AvailableStochasticSoilModels);

            Assert.IsInstanceOf<EmptyPipingOutput>(children[2]);
            Assert.IsInstanceOf<EmptyPipingCalculationReport>(children[3]);
        }

        [Test]
        public void ContextMenuStrip_PipingCalculationWithoutOutput_ContextMenuItemClearOutputDisabled()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var nodeData = new PipingCalculationScenarioContext(calculation,
                                                                Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                Enumerable.Empty<StochasticSoilModel>(),
                                                                pipingFailureMechanismMock,
                                                                assessmentSectionMock);

            gui.Expect(cmp => cmp.Get(nodeData, treeViewControlMock)).Return(new CustomItemsOnlyContextMenuBuilder());

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            var contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControlMock);

            // Assert
            mocks.VerifyAll(); // Expect no calls on arguments

            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 0, RingtoetsCommonFormsResources.Validate, RingtoetsCommonFormsResources.Validate_ToolTip, RingtoetsCommonFormsResources.ValidateIcon);
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 1, RingtoetsCommonFormsResources.Calculate, RingtoetsCommonFormsResources.Calculate_ToolTip, RingtoetsCommonFormsResources.CalculateIcon);
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 2, RingtoetsCommonFormsResources.Clear_output, RingtoetsCommonFormsResources.ClearOutput_No_output_to_clear, RingtoetsCommonFormsResources.ClearIcon, false);
        }

        [Test]
        public void ContextMenuStrip_PipingCalculationWithOutput_ContextMenuItemClearOutputEnabled()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                Output = new TestPipingOutput()
            };
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var nodeData = new PipingCalculationScenarioContext(calculation,
                                                                Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                Enumerable.Empty<StochasticSoilModel>(),
                                                                pipingFailureMechanismMock,
                                                                assessmentSectionMock);

            gui.Expect(cmp => cmp.Get(nodeData, treeViewControlMock)).Return(new CustomItemsOnlyContextMenuBuilder());

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            var contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControlMock);

            // Assert
            mocks.VerifyAll(); // Expect no calls on arguments

            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 0, RingtoetsCommonFormsResources.Validate, RingtoetsCommonFormsResources.Validate_ToolTip, RingtoetsCommonFormsResources.ValidateIcon);
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 1, RingtoetsCommonFormsResources.Calculate, RingtoetsCommonFormsResources.Calculate_ToolTip, RingtoetsCommonFormsResources.CalculateIcon);
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 2, RingtoetsCommonFormsResources.Clear_output, RingtoetsCommonFormsResources.Clear_output_ToolTip, RingtoetsCommonFormsResources.ClearIcon);
        }

        [Test]
        public void ContextMenuStrip_Always_ContextMenuItemPerformCalculationEnabled()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var nodeData = new PipingCalculationScenarioContext(calculation,
                                                                Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                Enumerable.Empty<StochasticSoilModel>(),
                                                                pipingFailureMechanismMock,
                                                                assessmentSectionMock);

            gui.Expect(cmp => cmp.Get(nodeData, treeViewControlMock)).Return(new CustomItemsOnlyContextMenuBuilder());

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            var contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControlMock);

            // Assert
            mocks.VerifyAll(); // Expect no calls on arguments

            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 1, RingtoetsCommonFormsResources.Calculate, RingtoetsCommonFormsResources.Calculate_ToolTip, RingtoetsCommonFormsResources.CalculateIcon);
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var menuBuilderMock = mocks.Stub<IContextMenuBuilder>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var nodeData = new PipingCalculationScenarioContext(new PipingCalculationScenario(new GeneralPipingInput()),
                                                                Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                Enumerable.Empty<StochasticSoilModel>(),
                                                                pipingFailureMechanismMock,
                                                                assessmentSectionMock);

            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddRenameItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddDeleteItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddImportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            gui.Expect(cmp => cmp.Get(nodeData, treeViewControlMock)).Return(menuBuilderMock);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            info.ContextMenuStrip(nodeData, null, treeViewControlMock);

            // Assert
            mocks.VerifyAll(); // Expect no calls on arguments
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

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationContext = new PipingCalculationScenarioContext(elementToBeRemoved,
                                                                          Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                          Enumerable.Empty<StochasticSoilModel>(),
                                                                          pipingFailureMechanismMock,
                                                                          assessmentSectionMock);
            var groupContext = new PipingCalculationGroupContext(group,
                                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                 Enumerable.Empty<StochasticSoilModel>(),
                                                                 pipingFailureMechanismMock,
                                                                 assessmentSectionMock);

            // Precondition
            Assert.IsTrue(info.CanRemove(calculationContext, groupContext));
            Assert.AreEqual(2, group.Children.Count);

            // Call
            info.OnNodeRemoved(calculationContext, groupContext);

            // Assert
            Assert.AreEqual(1, group.Children.Count);
            CollectionAssert.DoesNotContain(group.Children, elementToBeRemoved);

            mocks.VerifyAll();
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

            group.AddCalculationScenariosToFailureMechanismSectionResult(pipingFailureMechanism);

            // Precondition
            Assert.IsTrue(info.CanRemove(calculationContext, groupContext));
            Assert.AreEqual(2, group.Children.Count);
            var sectionResults = pipingFailureMechanism.SectionResults.ToArray();
            CollectionAssert.Contains(sectionResults[0].CalculationScenarios, elementToBeRemoved);

            // Call
            info.OnNodeRemoved(calculationContext, groupContext);

            // Assert
            Assert.AreEqual(1, group.Children.Count);
            CollectionAssert.DoesNotContain(group.Children, elementToBeRemoved);
            CollectionAssert.DoesNotContain(sectionResults[0].CalculationScenarios, elementToBeRemoved);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenInvalidPipingCalculation_WhenCalculatingFromContextMenu_ThenPipingCalculationNotifiesObserversAndLogMessageAdded()
        {
            // Given
            var gui = mocks.DynamicMock<IGui>();
            var mainWindow = mocks.DynamicMock<IMainWindow>();
            var observer = mocks.StrictMock<IObserver>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var pipingCalculationContext = new PipingCalculationScenarioContext(calculation,
                                                                                Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                Enumerable.Empty<StochasticSoilModel>(),
                                                                                pipingFailureMechanismMock,
                                                                                assessmentSectionMock);

            observer.Expect(o => o.UpdateObserver());

            assessmentSectionMock.Stub(s => s.FailureMechanismContribution).Return(new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 30, 20000));
            gui.Expect(cmp => cmp.Get(pipingCalculationContext, treeViewControlMock)).Return(new CustomItemsOnlyContextMenuBuilder());
            gui.Expect(g => g.MainWindow).Return(mainWindow);

            var expectedValidationMessageCount = 2; // No surfaceline or soil profile selected for calculation
            var calculateContextMenuItemIndex = 1;

            mocks.ReplayAll();

            plugin.Gui = gui;

            calculation.Attach(observer);

            var contextMenuAdapter = info.ContextMenuStrip(pipingCalculationContext, null, treeViewControlMock);

            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            // When
            Action action = () => { contextMenuAdapter.Items[calculateContextMenuItemIndex].PerformClick(); };

            // Then
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

            mocks.VerifyAll();
        }

        [Test]
        public void GivenInvalidPipingCalculation_WhenValidatingFromContextMenu_ThenLogMessageAddedAndNoNotifyObserver()
        {
            // Given
            var gui = mocks.DynamicMock<IGui>();
            var observer = mocks.StrictMock<IObserver>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var pipingCalculationContext = new PipingCalculationScenarioContext(calculation,
                                                                                Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                Enumerable.Empty<StochasticSoilModel>(),
                                                                                pipingFailureMechanismMock,
                                                                                assessmentSectionMock);

            gui.Expect(cmp => cmp.Get(pipingCalculationContext, treeViewControlMock)).Return(new CustomItemsOnlyContextMenuBuilder());

            var expectedValidationMessageCount = 2; // No surfaceline or soil profile selected for calculation
            var expectedStatusMessageCount = 2;
            var expectedLogMessageCount = expectedValidationMessageCount + expectedStatusMessageCount;

            var validateContextMenuItemIndex = 0;
            observer.Expect(o => o.UpdateObserver()).Repeat.Never();

            mocks.ReplayAll();

            plugin.Gui = gui;

            calculation.Attach(observer);

            var contextMenuAdapter = info.ContextMenuStrip(pipingCalculationContext, null, treeViewControlMock);

            // When
            Action action = () => contextMenuAdapter.Items[validateContextMenuItemIndex].PerformClick();

            // Then
            TestHelper.AssertLogMessagesCount(action, expectedLogMessageCount);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenValidPipingCalculation_WhenCalculatingFromContextMenu_ThenPipingCalculationNotifiesObservers()
        {
            // Given
            var gui = mocks.DynamicMock<IGui>();
            var mainWindow = mocks.DynamicMock<IMainWindow>();
            var observer = mocks.StrictMock<IObserver>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var calculateContextMenuItemIndex = 1;
            var calculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var pipingCalculationContext = new PipingCalculationScenarioContext(calculation,
                                                                                Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                Enumerable.Empty<StochasticSoilModel>(),
                                                                                pipingFailureMechanismMock,
                                                                                assessmentSectionMock);

            assessmentSectionMock.Stub(s => s.FailureMechanismContribution).Return(new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 30, 20000));
            gui.Expect(g => g.Get(pipingCalculationContext, treeViewControlMock)).Return(new CustomItemsOnlyContextMenuBuilder());
            gui.Expect(g => g.MainWindow).Return(mainWindow);

            observer.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            plugin.Gui = gui;

            calculation.Attach(observer);

            var contextMenuAdapter = info.ContextMenuStrip(pipingCalculationContext, null, treeViewControlMock);

            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            // When
            Action action = () => { contextMenuAdapter.Items[calculateContextMenuItemIndex].PerformClick(); };

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

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenPipingCalculationWithOutput_WhenClearingOutputFromContextMenu_ThenPipingCalculationOutputClearedAndNotified(bool confirm)
        {
            // Given
            var gui = mocks.DynamicMock<IGui>();
            var observer = mocks.StrictMock<IObserver>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var pipingCalculationContext = new PipingCalculationScenarioContext(calculation,
                                                                                Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                Enumerable.Empty<StochasticSoilModel>(),
                                                                                pipingFailureMechanismMock,
                                                                                assessmentSectionMock);

            gui.Expect(cmp => cmp.Get(pipingCalculationContext, treeViewControlMock)).Return(new CustomItemsOnlyContextMenuBuilder());

            int clearOutputItemPosition = 2;
            if (confirm)
            {
                observer.Expect(o => o.UpdateObserver());
            }

            mocks.ReplayAll();

            plugin.Gui = gui;

            calculation.Output = new TestPipingOutput();
            calculation.Attach(observer);

            var contextMenuAdapter = info.ContextMenuStrip(pipingCalculationContext, null, treeViewControlMock);

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

            // When
            contextMenuAdapter.Items[clearOutputItemPosition].PerformClick();

            // Then
            Assert.AreNotEqual(confirm, calculation.HasOutput);
            Assert.AreEqual("Bevestigen", messageBoxTitle);
            Assert.AreEqual("Weet u zeker dat u de uitvoer van deze berekening wilt wissen?", messageBoxText);
            mocks.VerifyAll();
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
                        },
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