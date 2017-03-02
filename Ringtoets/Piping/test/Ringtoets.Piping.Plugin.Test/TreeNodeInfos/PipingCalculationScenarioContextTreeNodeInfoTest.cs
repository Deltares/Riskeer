﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
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
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Piping.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class PipingCalculationScenarioContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuUpdateEntryAndExitPointIndex = 1;

        private const int contextMenuValidateIndex = 2;
        private const int contextMenuCalculateIndex = 3;
        private const int contextMenuClearIndex = 5;

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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
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
                                                                                assessmentSection);

            // Call
            var children = info.ChildNodeObjects(pipingCalculationContext).ToArray();

            // Assert
            Assert.AreEqual(3, children.Length);
            var comment = (Comment) children[0];
            Assert.AreSame(pipingCalculationContext.WrappedData.Comments, comment);

            var pipingInputContext = (PipingInputContext) children[1];
            Assert.AreSame(pipingCalculationContext.WrappedData.InputParameters, pipingInputContext.WrappedData);
            CollectionAssert.AreEqual(pipingCalculationContext.AvailablePipingSurfaceLines, pipingInputContext.AvailablePipingSurfaceLines);
            CollectionAssert.AreEqual(pipingCalculationContext.AvailableStochasticSoilModels, pipingInputContext.AvailableStochasticSoilModels);

            var pipingOutputContext = (PipingOutputContext) children[2];
            Assert.AreSame(pipingCalculationContext.WrappedData.SemiProbabilisticOutput, pipingOutputContext.SemiProbabilisticOutput);
            Assert.AreSame(pipingCalculationContext.WrappedData.Output, pipingOutputContext.WrappedData);
        }

        [Test]
        public void ChildNodeObjects_WithoutOutput_ReturnNoChildNodes()
        {
            // Setup
            var pipingFailureMechanism = new PipingFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var pipingCalculationContext = new PipingCalculationScenarioContext(new PipingCalculationScenario(new GeneralPipingInput()),
                                                                                Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                Enumerable.Empty<StochasticSoilModel>(),
                                                                                pipingFailureMechanism,
                                                                                assessmentSection);

            // Precondition
            Assert.IsFalse(pipingCalculationContext.WrappedData.HasOutput);

            // Call
            var children = info.ChildNodeObjects(pipingCalculationContext).ToArray();

            // Assert
            Assert.AreEqual(3, children.Length);
            var comment = (Comment) children[0];
            Assert.AreSame(pipingCalculationContext.WrappedData.Comments, comment);

            var pipingInputContext = (PipingInputContext) children[1];
            Assert.AreSame(pipingCalculationContext.WrappedData.InputParameters, pipingInputContext.WrappedData);
            CollectionAssert.AreEqual(pipingCalculationContext.AvailablePipingSurfaceLines, pipingInputContext.AvailablePipingSurfaceLines);
            CollectionAssert.AreEqual(pipingCalculationContext.AvailableStochasticSoilModels, pipingInputContext.AvailableStochasticSoilModels);

            Assert.IsInstanceOf<EmptyPipingOutput>(children[2]);
        }

        [Test]
        public void ContextMenuStrip_PipingCalculationWithoutOutput_ContextMenuItemClearOutputDisabledAndTooltipSet()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var calculation = new PipingCalculationScenario(new GeneralPipingInput());
                var pipingFailureMechanism = new TestPipingFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var nodeData = new PipingCalculationScenarioContext(calculation,
                                                                    Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                    Enumerable.Empty<StochasticSoilModel>(),
                                                                    pipingFailureMechanism,
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
                                                                  RingtoetsCommonFormsResources.Clear_output,
                                                                  RingtoetsCommonFormsResources.ClearOutput_No_output_to_clear,
                                                                  RingtoetsCommonFormsResources.ClearIcon,
                                                                  false);
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
                var pipingFailureMechanism = new TestPipingFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var nodeData = new PipingCalculationScenarioContext(calculation,
                                                                    Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                    Enumerable.Empty<StochasticSoilModel>(),
                                                                    pipingFailureMechanism,
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
                                                                  RingtoetsCommonFormsResources.Clear_output,
                                                                  RingtoetsCommonFormsResources.Clear_output_ToolTip,
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
                var calculation = new PipingCalculationScenario(new GeneralPipingInput());
                var pipingFailureMechanism = new PipingFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var nodeData = new PipingCalculationScenarioContext(calculation,
                                                                    Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                    Enumerable.Empty<StochasticSoilModel>(),
                                                                    pipingFailureMechanism,
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
                                                                  contextMenuCalculateIndex,
                                                                  "Be&rekenen",
                                                                  "De bijdrage van dit toetsspoor is nul.",
                                                                  RingtoetsCommonFormsResources.CalculateIcon,
                                                                  false);

                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  contextMenuValidateIndex,
                                                                  "&Valideren",
                                                                  "De bijdrage van dit toetsspoor is nul.",
                                                                  RingtoetsCommonFormsResources.ValidateIcon,
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
                var calculation = new PipingCalculationScenario(new GeneralPipingInput());
                var pipingFailureMechanism = new TestPipingFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var nodeData = new PipingCalculationScenarioContext(calculation,
                                                                    Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                    Enumerable.Empty<StochasticSoilModel>(),
                                                                    pipingFailureMechanism,
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
                                                                  contextMenuCalculateIndex,
                                                                  "Be&rekenen",
                                                                  "Voer deze berekening uit.",
                                                                  RingtoetsCommonFormsResources.CalculateIcon);

                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  contextMenuValidateIndex,
                                                                  "&Valideren",
                                                                  "Valideer de invoer voor deze berekening.",
                                                                  RingtoetsCommonFormsResources.ValidateIcon);
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
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var nodeData = new PipingCalculationScenarioContext(new PipingCalculationScenario(new GeneralPipingInput()),
                                                                    Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                    Enumerable.Empty<StochasticSoilModel>(),
                                                                    pipingFailureMechanism,
                                                                    assessmentSection);

                var menuBuilderMock = mocks.Stub<IContextMenuBuilder>();
                using (mocks.Ordered())
                {
                    menuBuilderMock.Expect(mb => mb.AddRenameItem()).Return(menuBuilderMock);
                    menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
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
        public void ContextMenuStrip_CalculationWithoutSurfaceLine_ContextMenuItemUpdateEntryAndExitPointDisabled()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var calculation = new PipingCalculationScenario(new GeneralPipingInput());
                var pipingFailureMechanism = new TestPipingFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var nodeData = new PipingCalculationScenarioContext(calculation,
                                                                    Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                    Enumerable.Empty<StochasticSoilModel>(),
                                                                    pipingFailureMechanism,
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
                                                                  contextMenuUpdateEntryAndExitPointIndex,
                                                                  "&Bijwerken intrede- en uittredepunt",
                                                                  "Er moet een profielschematisatie geselecteerd zijn.",
                                                                  RingtoetsCommonFormsResources.UpdateItemIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationWithSurfaceLine_ContextMenuItemUpdateEntryAndExitPointEnabled()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var surfaceLine = new RingtoetsPipingSurfaceLine();
                surfaceLine.SetGeometry(new[]
                {
                    new Point3D(1, 2, 3),
                    new Point3D(4, 5, 6)
                });
                var calculation = new PipingCalculationScenario(new GeneralPipingInput())
                {
                    InputParameters =
                    {
                        SurfaceLine = surfaceLine
                    }
                };
                var pipingFailureMechanism = new TestPipingFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var nodeData = new PipingCalculationScenarioContext(calculation,
                                                                    Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                    Enumerable.Empty<StochasticSoilModel>(),
                                                                    pipingFailureMechanism,
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
                                                                  contextMenuUpdateEntryAndExitPointIndex,
                                                                  "&Bijwerken intrede- en uittredepunt",
                                                                  "Berekening bijwerken met de karakteristieke punten.",
                                                                  RingtoetsCommonFormsResources.UpdateItemIcon);
                }
            }
        }

        [Test]
        public void GivenCalculationWithSurfaceLineWithoutOutput_WhenEntryAndExitPointUpdatedAndUpdateEntryAndExitPointClicked_ThenNoInquiryAndPointsUpdatedAndInputObserverNotified()
        {
            using (var treeViewControl = new TreeViewControl())
            {
                // Given
                var surfaceLine = new RingtoetsPipingSurfaceLine();
                surfaceLine.SetGeometry(new[]
                {
                    new Point3D(1, 2, 3),
                    new Point3D(4, 5, 6)
                });
                var calculation = new PipingCalculationScenario(new GeneralPipingInput())
                {
                    InputParameters =
                    {
                        SurfaceLine = surfaceLine,
                        EntryPointL = (RoundedDouble) 0,
                        ExitPointL = (RoundedDouble) 1
                    }
                };

                var pipingFailureMechanism = new TestPipingFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var nodeData = new PipingCalculationScenarioContext(calculation,
                                                                    Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                    Enumerable.Empty<StochasticSoilModel>(),
                                                                    pipingFailureMechanism,
                                                                    assessmentSection);

                var inputObserver = mocks.StrictMock<IObserver>();
                inputObserver.Expect(obs => obs.UpdateObserver());
                calculation.InputParameters.Attach(inputObserver);

                var calculationObserver = mocks.StrictMock<IObserver>();
                calculation.Attach(calculationObserver);

                var mainWindow = mocks.Stub<IMainWindow>();
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    surfaceLine.SetGeometry(new[]
                    {
                        new Point3D(0, 0, 0),
                        new Point3D(1, 0, 2),
                        new Point3D(2, 0, 3),
                        new Point3D(3, 0, 0),
                        new Point3D(4, 0, 2),
                        new Point3D(5, 0, 3)
                    });
                    surfaceLine.SetDikeToeAtRiverAt(new Point3D(2, 0, 3));
                    surfaceLine.SetDikeToeAtPolderAt(new Point3D(3, 0, 0));

                    contextMenuStrip.Items[contextMenuUpdateEntryAndExitPointIndex].PerformClick();

                    // Then
                    PipingInput inputParameters = calculation.InputParameters;
                    Assert.AreSame(surfaceLine, inputParameters.SurfaceLine);
                    Assert.AreEqual(new RoundedDouble(2, 2), inputParameters.EntryPointL);
                    Assert.AreEqual(new RoundedDouble(3, 3), inputParameters.ExitPointL);
                    Assert.IsFalse(calculation.HasOutput);

                    // Note: observer assertions are verified in Teardown
                }
            }
        }

        [Test]
        public void GivenCalculationWithSurfaceLineAndOutput_WhenEntryAndExitPointsUpdatedAndUpdateEntryAndExitPointClickedAndContinued_ThenPointsUpdatedOutputsRemovedAndObserversNotified()
        {
            using (var treeViewControl = new TreeViewControl())
            {
                // Given
                var surfaceLine = new RingtoetsPipingSurfaceLine();
                surfaceLine.SetGeometry(new[]
                {
                    new Point3D(1, 2, 3),
                    new Point3D(4, 5, 6)
                });
                var calculation = new PipingCalculationScenario(new GeneralPipingInput())
                {
                    InputParameters =
                    {
                        SurfaceLine = surfaceLine,
                        EntryPointL = (RoundedDouble) 0,
                        ExitPointL = (RoundedDouble) 1
                    },
                    Output = new TestPipingOutput()
                };

                var pipingFailureMechanism = new TestPipingFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var nodeData = new PipingCalculationScenarioContext(calculation,
                                                                    Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                    Enumerable.Empty<StochasticSoilModel>(),
                                                                    pipingFailureMechanism,
                                                                    assessmentSection);

                var inputObserver = mocks.StrictMock<IObserver>();
                inputObserver.Expect(obs => obs.UpdateObserver());
                calculation.InputParameters.Attach(inputObserver);

                var calculationObserver = mocks.StrictMock<IObserver>();
                calculationObserver.Expect(obs => obs.UpdateObserver());
                calculation.Attach(calculationObserver);

                var mainWindow = mocks.Stub<IMainWindow>();
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                string textBoxMessage = null;
                DialogBoxHandler = (name, wnd) =>
                {
                    var helper = new MessageBoxTester(wnd);
                    textBoxMessage = helper.Text;
                    helper.ClickOk();
                };

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    surfaceLine.SetGeometry(new[]
                    {
                        new Point3D(0, 0, 0),
                        new Point3D(1, 0, 2),
                        new Point3D(2, 0, 3),
                        new Point3D(3, 0, 0),
                        new Point3D(4, 0, 2),
                        new Point3D(5, 0, 3)
                    });
                    surfaceLine.SetDikeToeAtRiverAt(new Point3D(2, 0, 3));
                    surfaceLine.SetDikeToeAtPolderAt(new Point3D(3, 0, 0));

                    contextMenuStrip.Items[contextMenuUpdateEntryAndExitPointIndex].PerformClick();

                    // Then
                    PipingInput inputParameters = calculation.InputParameters;
                    Assert.AreSame(surfaceLine, inputParameters.SurfaceLine);
                    Assert.AreEqual(new RoundedDouble(2, 2), inputParameters.EntryPointL);
                    Assert.AreEqual(new RoundedDouble(3, 3), inputParameters.ExitPointL);
                    Assert.IsFalse(calculation.HasOutput);

                    string expectedMessage = "Wanneer de intrede- en/of uittredepunten wijzigen als gevolg van het bijwerken, " +
                                             "zal het resultaat van de berekening die deze profielschematisaties gebruikt, worden " +
                                             $"verwijderd.{Environment.NewLine}{Environment.NewLine}Weet u zeker dat u wilt doorgaan?";
                    Assert.AreEqual(expectedMessage, textBoxMessage);

                    // Note: observer assertions are verified in Teardown
                }
            }
        }

        [Test]
        public void GivenCalculationWithSurfaceLineAndOutput_WhenUpdatedEntryAndExitPointsHasNoChangeAndUpdateEntryAndExitPointClickedAndContinued_ThenOutputNotRemovedAndObserversNotNotified()
        {
            using (var treeViewControl = new TreeViewControl())
            {
                // Given
                var surfaceLine = new RingtoetsPipingSurfaceLine();
                surfaceLine.SetGeometry(new[]
                {
                    new Point3D(1, 2, 3),
                    new Point3D(4, 5, 6)
                });
                var calculation = new PipingCalculationScenario(new GeneralPipingInput())
                {
                    InputParameters =
                    {
                        SurfaceLine = surfaceLine,
                        EntryPointL = (RoundedDouble) 2,
                        ExitPointL = (RoundedDouble) 3
                    },
                    Output = new TestPipingOutput()
                };

                var pipingFailureMechanism = new TestPipingFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var nodeData = new PipingCalculationScenarioContext(calculation,
                                                                    Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                    Enumerable.Empty<StochasticSoilModel>(),
                                                                    pipingFailureMechanism,
                                                                    assessmentSection);

                var inputObserver = mocks.StrictMock<IObserver>();
                calculation.InputParameters.Attach(inputObserver);

                var calculationObserver = mocks.StrictMock<IObserver>();
                calculation.Attach(calculationObserver);

                var mainWindow = mocks.Stub<IMainWindow>();
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                string textBoxMessage = null;
                DialogBoxHandler = (name, wnd) =>
                {
                    var helper = new MessageBoxTester(wnd);
                    textBoxMessage = helper.Text;
                    helper.ClickOk();
                };

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    surfaceLine.SetGeometry(new[]
                    {
                        new Point3D(0, 0, 0),
                        new Point3D(1, 0, 2),
                        new Point3D(2, 0, 3),
                        new Point3D(3, 0, 0),
                        new Point3D(4, 0, 2),
                        new Point3D(5, 0, 3)
                    });
                    surfaceLine.SetDikeToeAtRiverAt(new Point3D(2, 0, 3));
                    surfaceLine.SetDikeToeAtPolderAt(new Point3D(3, 0, 0));

                    contextMenuStrip.Items[contextMenuUpdateEntryAndExitPointIndex].PerformClick();

                    // Then
                    PipingInput inputParameters = calculation.InputParameters;
                    Assert.AreSame(surfaceLine, inputParameters.SurfaceLine);
                    Assert.AreEqual(new RoundedDouble(2, 2), inputParameters.EntryPointL);
                    Assert.AreEqual(new RoundedDouble(3, 3), inputParameters.ExitPointL);
                    Assert.IsTrue(calculation.HasOutput);

                    string expectedMessage = "Wanneer de intrede- en/of uittredepunten wijzigen als gevolg van het bijwerken, " +
                                             "zal het resultaat van de berekening die deze profielschematisaties gebruikt, worden " +
                                             $"verwijderd.{Environment.NewLine}{Environment.NewLine}Weet u zeker dat u wilt doorgaan?";
                    Assert.AreEqual(expectedMessage, textBoxMessage);

                    // Note: observer assertions are verified in Teardown
                }
            }
        }

        [Test]
        public void GivenCalculationWithSurfaceLineAndOutput_WhenEntryAndExitPointsUpdatedAndUpdateEntryAndExitPointClickedAndDiscontinued_ThenCalculationNotUpdatedAndObserversNotUpdated()
        {
            using (var treeViewControl = new TreeViewControl())
            {
                // Given
                var surfaceLine = new RingtoetsPipingSurfaceLine();
                surfaceLine.SetGeometry(new[]
                {
                    new Point3D(1, 2, 3),
                    new Point3D(4, 5, 6)
                });
                var calculation = new PipingCalculationScenario(new GeneralPipingInput())
                {
                    InputParameters =
                    {
                        SurfaceLine = surfaceLine,
                        EntryPointL = (RoundedDouble) 0,
                        ExitPointL = (RoundedDouble) 1
                    },
                    Output = new TestPipingOutput()
                };

                var pipingFailureMechanism = new TestPipingFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var nodeData = new PipingCalculationScenarioContext(calculation,
                                                                    Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                    Enumerable.Empty<StochasticSoilModel>(),
                                                                    pipingFailureMechanism,
                                                                    assessmentSection);

                var inputObserver = mocks.StrictMock<IObserver>();
                calculation.InputParameters.Attach(inputObserver);

                var calculationObserver = mocks.StrictMock<IObserver>();
                calculation.Attach(calculationObserver);

                var mainWindow = mocks.Stub<IMainWindow>();
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                string textBoxMessage = null;
                DialogBoxHandler = (name, wnd) =>
                {
                    var helper = new MessageBoxTester(wnd);
                    textBoxMessage = helper.Text;
                    helper.ClickCancel();
                };

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    surfaceLine.SetGeometry(new[]
                    {
                        new Point3D(0, 0, 0),
                        new Point3D(1, 0, 2),
                        new Point3D(2, 0, 3),
                        new Point3D(3, 0, 0),
                        new Point3D(4, 0, 2),
                        new Point3D(5, 0, 3)
                    });
                    surfaceLine.SetDikeToeAtRiverAt(new Point3D(2, 0, 3));
                    surfaceLine.SetDikeToeAtPolderAt(new Point3D(3, 0, 0));

                    contextMenuStrip.Items[contextMenuUpdateEntryAndExitPointIndex].PerformClick();

                    // Then
                    PipingInput inputParameters = calculation.InputParameters;
                    Assert.AreSame(surfaceLine, inputParameters.SurfaceLine);
                    Assert.AreEqual(new RoundedDouble(2, 0), inputParameters.EntryPointL);
                    Assert.AreEqual(new RoundedDouble(3, 1), inputParameters.ExitPointL);
                    Assert.IsTrue(calculation.HasOutput);

                    string expectedMessage = "Wanneer de intrede- en/of uittredepunten wijzigen als gevolg van het bijwerken, " +
                                             "zal het resultaat van de berekening die deze profielschematisaties gebruikt, worden " +
                                             $"verwijderd.{Environment.NewLine}{Environment.NewLine}Weet u zeker dat u wilt doorgaan?";
                    Assert.AreEqual(expectedMessage, textBoxMessage);

                    // Note: observer assertions are verified in Teardown
                }
            }
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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationContext = new PipingCalculationScenarioContext(elementToBeRemoved,
                                                                          Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                          Enumerable.Empty<StochasticSoilModel>(),
                                                                          pipingFailureMechanism,
                                                                          assessmentSection);
            var groupContext = new PipingCalculationGroupContext(group,
                                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                 Enumerable.Empty<StochasticSoilModel>(),
                                                                 pipingFailureMechanism,
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
        public void OnNodeRemoved_ParentIsPipingCalculationGroupContext_RemoveCalculationFromSectionResult(bool groupNameEditable)
        {
            // Setup
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var pipingFailureMechanism = TestPipingFailureMechanism.GetFailureMechanismWithSurfaceLinesAndStochasticSoilModels();
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

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationContext = new PipingCalculationScenarioContext(elementToBeRemoved,
                                                                          Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                          Enumerable.Empty<StochasticSoilModel>(),
                                                                          pipingFailureMechanism,
                                                                          assessmentSection);
            var groupContext = new PipingCalculationGroupContext(group,
                                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                 Enumerable.Empty<StochasticSoilModel>(),
                                                                 pipingFailureMechanism,
                                                                 assessmentSection);

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
                var pipingFailureMechanism = new TestPipingFailureMechanism();
                pipingFailureMechanism.AddSection(new FailureMechanismSection("A", new[]
                {
                    new Point2D(0, 0)
                }));
                IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(
                    pipingFailureMechanism, mocks);
                var pipingCalculationContext = new PipingCalculationScenarioContext(calculation,
                                                                                    Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                    Enumerable.Empty<StochasticSoilModel>(),
                                                                                    pipingFailureMechanism,
                                                                                    assessmentSectionStub);

                var mainWindow = mocks.DynamicMock<IMainWindow>();

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(pipingCalculationContext, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
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

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(pipingCalculationContext, null, treeViewControl))
                {
                    // When
                    Action action = () => contextMenuStrip.Items[contextMenuCalculateIndex].PerformClick();

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
                var pipingFailureMechanism = new TestPipingFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();

                var pipingCalculationContext = new PipingCalculationScenarioContext(calculation,
                                                                                    Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                    Enumerable.Empty<StochasticSoilModel>(),
                                                                                    pipingFailureMechanism,
                                                                                    assessmentSection);

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(pipingCalculationContext, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());

                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver()).Repeat.Never();

                mocks.ReplayAll();

                plugin.Gui = gui;

                calculation.Attach(observer);

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(pipingCalculationContext, null, treeViewControl))
                {
                    // When
                    Action action = () => contextMenuStrip.Items[contextMenuValidateIndex].PerformClick();

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
                var pipingFailureMechanism = new TestPipingFailureMechanism();
                pipingFailureMechanism.AddSection(new FailureMechanismSection("A", new[]
                {
                    new Point2D(0, 0)
                }));
                IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabase(
                    pipingFailureMechanism, mocks);

                var pipingCalculationContext = new PipingCalculationScenarioContext(calculation,
                                                                                    Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                    Enumerable.Empty<StochasticSoilModel>(),
                                                                                    pipingFailureMechanism,
                                                                                    assessmentSectionStub);

                var mainWindow = mocks.DynamicMock<IMainWindow>();

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(pipingCalculationContext, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
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

                using (var contextMenuAdapter = info.ContextMenuStrip(pipingCalculationContext, null, treeViewControl))
                {
                    // When
                    Action action = () => contextMenuAdapter.Items[contextMenuCalculateIndex].PerformClick();

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
                var assessmentSection = mocks.Stub<IAssessmentSection>();

                var pipingCalculationContext = new PipingCalculationScenarioContext(calculation,
                                                                                    Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                    Enumerable.Empty<StochasticSoilModel>(),
                                                                                    pipingFailureMechanism,
                                                                                    assessmentSection);

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(pipingCalculationContext, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());

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
                    contextMenuStrip.Items[contextMenuClearIndex].PerformClick();

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
    }
}