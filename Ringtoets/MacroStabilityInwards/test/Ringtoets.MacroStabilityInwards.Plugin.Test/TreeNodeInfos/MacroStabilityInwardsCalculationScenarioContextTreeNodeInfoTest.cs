// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Forms.PresentationObjects;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil;
using Ringtoets.MacroStabilityInwards.Primitives;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.MacroStabilityInwards.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationScenarioContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuValidateIndex = 4;
        private const int contextMenuCalculateIndex = 5;
        private const int contextMenuClearIndex = 7;

        private MockRepository mocks;
        private MacroStabilityInwardsPlugin plugin;
        private TreeNodeInfo info;

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
        public void ChildNodeObjects_WithOutputData_ReturnOutputChildNode()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput())
            {
                Output = new TestMacroStabilityInwardsOutput(),
                SemiProbabilisticOutput = new TestMacroStabilityInwardsSemiProbabilisticOutput()
            };

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationContext = new MacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                                         new[]
                                                                                         {
                                                                                             new RingtoetsMacroStabilityInwardsSurfaceLine()
                                                                                         },
                                                                                         new[]
                                                                                         {
                                                                                             new TestStochasticSoilModel()
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
            Assert.AreSame(calculationContext.WrappedData.SemiProbabilisticOutput, outputContext.SemiProbabilisticOutput);
            Assert.AreSame(calculationContext.WrappedData.Output, outputContext.WrappedData);
        }

        [Test]
        public void ChildNodeObjects_WithoutOutput_ReturnNoChildNodes()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationContext = new MacroStabilityInwardsCalculationScenarioContext(new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput()),
                                                                                         Enumerable.Empty<RingtoetsMacroStabilityInwardsSurfaceLine>(),
                                                                                         Enumerable.Empty<StochasticSoilModel>(),
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

            Assert.IsInstanceOf<EmptyMacroStabilityInwardsOutput>(children[2]);
        }

        [Test]
        public void ContextMenuStrip_MacroStabilityInwardsCalculationWithoutOutput_ContextMenuItemClearOutputDisabledAndTooltipSet()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
                var failureMechanism = new TestMacroStabilityInwardsFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var nodeData = new MacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                                   Enumerable.Empty<RingtoetsMacroStabilityInwardsSurfaceLine>(),
                                                                                   Enumerable.Empty<StochasticSoilModel>(),
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
                var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput())
                {
                    Output = new TestMacroStabilityInwardsOutput(),
                    SemiProbabilisticOutput = new TestMacroStabilityInwardsSemiProbabilisticOutput()
                };
                var failureMechanism = new TestMacroStabilityInwardsFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var nodeData = new MacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                                   Enumerable.Empty<RingtoetsMacroStabilityInwardsSurfaceLine>(),
                                                                                   Enumerable.Empty<StochasticSoilModel>(),
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
        public void ContextMenuStrip_FailureMechanismContributionZero_ContextMenuItemCalculateAndValidateDisabledAndTooltipSet()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
                var failureMechanism = new MacroStabilityInwardsFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var nodeData = new MacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                                   Enumerable.Empty<RingtoetsMacroStabilityInwardsSurfaceLine>(),
                                                                                   Enumerable.Empty<StochasticSoilModel>(),
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
                                                                  contextMenuValidateIndex,
                                                                  "&Valideren",
                                                                  "De bijdrage van dit toetsspoor is nul.",
                                                                  RingtoetsCommonFormsResources.ValidateIcon,
                                                                  false);

                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  contextMenuCalculateIndex,
                                                                  "Be&rekenen",
                                                                  "De bijdrage van dit toetsspoor is nul.",
                                                                  RingtoetsCommonFormsResources.CalculateIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_AllRequiredInputSet_ContextMenuItemCalculateAndValidateEnabled()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
                var failureMechanism = new TestMacroStabilityInwardsFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var nodeData = new MacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                                   Enumerable.Empty<RingtoetsMacroStabilityInwardsSurfaceLine>(),
                                                                                   Enumerable.Empty<StochasticSoilModel>(),
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
                var nodeData = new MacroStabilityInwardsCalculationScenarioContext(new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput()),
                                                                                   Enumerable.Empty<RingtoetsMacroStabilityInwardsSurfaceLine>(),
                                                                                   Enumerable.Empty<StochasticSoilModel>(),
                                                                                   failureMechanism,
                                                                                   assessmentSection);

                var menuBuilderMock = mocks.Stub<IContextMenuBuilder>();
                using (mocks.Ordered())
                {
                    menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
                    menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                    menuBuilderMock.Expect(mb => mb.AddRenameItem()).Return(menuBuilderMock);
                    menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                    menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                    menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                    menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                    menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                    menuBuilderMock.Expect(mb => mb.AddDeleteItem()).Return(menuBuilderMock);
                    menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                    menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
                    menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
                    menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                    menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
                    menuBuilderMock.Expect(mb => mb.Build()).Return(null);
                }

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilderMock);
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
        public void OnNodeRemoved_ParentIsCalculationGroupContext_RemoveCalculationFromGroup(bool groupNameEditable)
        {
            // Setup
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var elementToBeRemoved = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());

            var group = new CalculationGroup("", groupNameEditable);
            group.Children.Add(elementToBeRemoved);
            group.Children.Add(new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput()));
            group.Attach(observer);

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationContext = new MacroStabilityInwardsCalculationScenarioContext(elementToBeRemoved,
                                                                                         Enumerable.Empty<RingtoetsMacroStabilityInwardsSurfaceLine>(),
                                                                                         Enumerable.Empty<StochasticSoilModel>(),
                                                                                         failureMechanism,
                                                                                         assessmentSection);
            var groupContext = new MacroStabilityInwardsCalculationGroupContext(group,
                                                                                Enumerable.Empty<RingtoetsMacroStabilityInwardsSurfaceLine>(),
                                                                                Enumerable.Empty<StochasticSoilModel>(),
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
        [TestCase(true)]
        [TestCase(false)]
        public void OnNodeRemoved_ParentIsCalculationGroupContext_RemoveCalculationFromSectionResult(bool groupNameEditable)
        {
            // Setup
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            TestMacroStabilityInwardsFailureMechanism macroStabilityInwardsFailureMechanism = TestMacroStabilityInwardsFailureMechanism.GetFailureMechanismWithSurfaceLinesAndStochasticSoilModels();
            RingtoetsMacroStabilityInwardsSurfaceLine[] surfaceLines = macroStabilityInwardsFailureMechanism.SurfaceLines.ToArray();

            var elementToBeRemoved = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput())
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLines[0]
                }
            };

            var group = new CalculationGroup("", groupNameEditable);
            group.Children.Add(elementToBeRemoved);
            group.Children.Add(new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput()));
            group.Attach(observer);
            macroStabilityInwardsFailureMechanism.CalculationsGroup.Children.Add(group);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationContext = new MacroStabilityInwardsCalculationScenarioContext(elementToBeRemoved,
                                                                                         Enumerable.Empty<RingtoetsMacroStabilityInwardsSurfaceLine>(),
                                                                                         Enumerable.Empty<StochasticSoilModel>(),
                                                                                         macroStabilityInwardsFailureMechanism,
                                                                                         assessmentSection);
            var groupContext = new MacroStabilityInwardsCalculationGroupContext(group,
                                                                                Enumerable.Empty<RingtoetsMacroStabilityInwardsSurfaceLine>(),
                                                                                Enumerable.Empty<StochasticSoilModel>(),
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
                var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
                var failureMechanism = new TestMacroStabilityInwardsFailureMechanism();
                failureMechanism.AddSection(new FailureMechanismSection("A", new[]
                {
                    new Point2D(0, 0)
                }));
                IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(
                    failureMechanism, mocks);
                var calculationContext = new MacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                                             Enumerable.Empty<RingtoetsMacroStabilityInwardsSurfaceLine>(),
                                                                                             Enumerable.Empty<StochasticSoilModel>(),
                                                                                             failureMechanism,
                                                                                             assessmentSectionStub);

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
                    TestHelper.AssertLogMessages(action, messages =>
                    {
                        IEnumerator<string> msgs = messages.GetEnumerator();
                        Assert.IsTrue(msgs.MoveNext());
                        Assert.AreEqual("Validatie van 'Nieuwe berekening' is gestart.", msgs.Current);
                        for (var i = 0; i < expectedValidationMessageCount; i++)
                        {
                            Assert.IsTrue(msgs.MoveNext());
                            StringAssert.StartsWith("Validatie mislukt: ", msgs.Current);
                        }
                        Assert.IsTrue(msgs.MoveNext());
                        Assert.AreEqual("Validatie van 'Nieuwe berekening' is beëindigd.", msgs.Current);
                    });
                    Assert.IsNull(calculation.Output);
                    Assert.IsNull(calculation.SemiProbabilisticOutput);
                }
            }
        }

        [Test]
        public void GivenInvalidCalculation_WhenValidatingFromContextMenu_ThenLogMessageAddedAndNoNotifyObserver()
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
                var failureMechanism = new TestMacroStabilityInwardsFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();

                var calculationContext = new MacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                                             Enumerable.Empty<RingtoetsMacroStabilityInwardsSurfaceLine>(),
                                                                                             Enumerable.Empty<StochasticSoilModel>(),
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
                MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput();
                var failureMechanism = new TestMacroStabilityInwardsFailureMechanism();
                failureMechanism.AddSection(new FailureMechanismSection("A", new[]
                {
                    new Point2D(0, 0)
                }));
                IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(
                    failureMechanism, mocks);

                var calculationContext = new MacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                                             Enumerable.Empty<RingtoetsMacroStabilityInwardsSurfaceLine>(),
                                                                                             Enumerable.Empty<StochasticSoilModel>(),
                                                                                             failureMechanism,
                                                                                             assessmentSectionStub);

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

                using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(calculationContext, null, treeViewControl))
                {
                    // When
                    Action action = () => contextMenuAdapter.Items[contextMenuCalculateIndex].PerformClick();

                    // Then
                    TestHelper.AssertLogMessages(action, messages =>
                    {
                        IEnumerator<string> msgs = messages.GetEnumerator();
                        Assert.IsTrue(msgs.MoveNext());
                        Assert.AreEqual("Validatie van 'Nieuwe berekening' is gestart.", msgs.Current);
                        Assert.IsTrue(msgs.MoveNext());
                        Assert.AreEqual("Validatie van 'Nieuwe berekening' is beëindigd.", msgs.Current);

                        Assert.IsTrue(msgs.MoveNext());
                        Assert.AreEqual("Berekening van 'Nieuwe berekening' is gestart.", msgs.Current);
                        Assert.IsTrue(msgs.MoveNext());
                        Assert.AreEqual("Berekening van 'Nieuwe berekening' is beëindigd.", msgs.Current);
                    });
                    Assert.IsNotNull(calculation.Output);
                    Assert.IsNotNull(calculation.SemiProbabilisticOutput);
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
                var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
                var failureMechanism = new MacroStabilityInwardsFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();

                var calculationContext = new MacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                                             Enumerable.Empty<RingtoetsMacroStabilityInwardsSurfaceLine>(),
                                                                                             Enumerable.Empty<StochasticSoilModel>(),
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

                calculation.Output = new TestMacroStabilityInwardsOutput();
                calculation.SemiProbabilisticOutput = new TestMacroStabilityInwardsSemiProbabilisticOutput();
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
    }
}