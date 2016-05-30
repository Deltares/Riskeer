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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
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
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Plugin;
using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsDataResources = Ringtoets.Common.Data.Properties.Resources;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Piping.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class PipingFailureMechanismContextTreeNodeInfoTest : NUnitFormTest
    {
        private MockRepository mocks;
        private PipingGuiPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new PipingGuiPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(PipingFailureMechanismContext));
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(PipingFailureMechanismContext), info.TagType);
            Assert.IsNotNull(info.ForeColor);
            Assert.IsNull(info.EnsureVisibleOnCreate);
            Assert.IsNull(info.CanRename);
            Assert.IsNull(info.OnNodeRenamed);
            Assert.IsNull(info.CanRemove);
            Assert.IsNull(info.OnNodeRemoved);
            Assert.IsNull(info.CanCheck);
            Assert.IsNull(info.IsChecked);
            Assert.IsNull(info.OnNodeChecked);
            Assert.IsNull(info.CanDrag);
            Assert.IsNull(info.CanDrop);
            Assert.IsNull(info.CanInsert);
            Assert.IsNull(info.OnDrop);
        }

        [Test]
        public void Text_Always_ReturnsName()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var mechanism = new PipingFailureMechanism();
            var mechanismContext = new PipingFailureMechanismContext(mechanism, assessmentSection);

            // Call
            var text = info.Text(mechanismContext);

            // Assert
            Assert.AreEqual("Dijken en dammen - Piping", text);
            mocks.VerifyAll();
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
        public void ChildNodeObjects_FailureMechanismIsRelevant_ReturnChildDataNodes()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var pipingFailureMechanism = new PipingFailureMechanism();
            var generalInputParameters = new GeneralPipingInput();
            var pipingProbabilityAssessmentInput = new PipingProbabilityAssessmentInput();
            pipingFailureMechanism.CalculationsGroup.Children.Add(new PipingCalculationScenario(generalInputParameters, pipingProbabilityAssessmentInput));
            pipingFailureMechanism.CalculationsGroup.Children.Add(new PipingCalculationScenario(generalInputParameters, pipingProbabilityAssessmentInput));

            var pipingFailureMechanismContext = new PipingFailureMechanismContext(pipingFailureMechanism, assessmentSection);

            // Call
            var children = info.ChildNodeObjects(pipingFailureMechanismContext).ToArray();

            // Assert
            Assert.AreEqual(3, children.Length);
            var inputsFolder = (CategoryTreeFolder) children[0];
            Assert.AreEqual("Invoer", inputsFolder.Name);
            Assert.AreEqual(TreeFolderCategory.Input, inputsFolder.Category);

            Assert.AreEqual(4, inputsFolder.Contents.Count);
            var failureMechanismSectionsContext = (FailureMechanismSectionsContext) inputsFolder.Contents[0];
            CollectionAssert.AreEqual(pipingFailureMechanism.Sections, failureMechanismSectionsContext.WrappedData);
            Assert.AreSame(pipingFailureMechanism, failureMechanismSectionsContext.ParentFailureMechanism);
            Assert.AreSame(assessmentSection, failureMechanismSectionsContext.ParentAssessmentSection);

            var surfaceLinesContext = (RingtoetsPipingSurfaceLinesContext) inputsFolder.Contents[1];
            Assert.AreSame(pipingFailureMechanism, surfaceLinesContext.FailureMechanism);
            Assert.AreSame(assessmentSection, surfaceLinesContext.AssessmentSection);

            var stochasticSoilModelContext = (StochasticSoilModelContext) inputsFolder.Contents[2];
            Assert.AreSame(pipingFailureMechanism, stochasticSoilModelContext.FailureMechanism);
            Assert.AreSame(assessmentSection, stochasticSoilModelContext.AssessmentSection);

            var commentContext = (CommentContext<ICommentable>) inputsFolder.Contents[3];
            Assert.AreSame(pipingFailureMechanism, commentContext.CommentContainer);

            var calculationsFolder = (PipingCalculationGroupContext) children[1];
            Assert.AreEqual("Berekeningen", calculationsFolder.WrappedData.Name);
            CollectionAssert.AreEqual(pipingFailureMechanism.CalculationsGroup.Children, calculationsFolder.WrappedData.Children);
            Assert.AreSame(pipingFailureMechanism.SurfaceLines, calculationsFolder.AvailablePipingSurfaceLines);
            Assert.AreEqual(pipingFailureMechanism.StochasticSoilModels, calculationsFolder.AvailableStochasticSoilModels);
            Assert.AreSame(pipingFailureMechanism, calculationsFolder.FailureMechanism);

            var outputsFolder = (CategoryTreeFolder) children[2];
            Assert.AreEqual("Uitvoer", outputsFolder.Name);
            Assert.AreEqual(TreeFolderCategory.Output, outputsFolder.Category);

            var failureMechanismResultsContext = (FailureMechanismSectionResultContext<PipingFailureMechanismSectionResult>)outputsFolder.Contents[0];
            Assert.AreSame(pipingFailureMechanism, failureMechanismResultsContext.FailureMechanism);
            Assert.AreSame(pipingFailureMechanism.SectionResults, failureMechanismResultsContext.SectionResults);
            mocks.VerifyAll();
        }

        [Test]
        public void ChildNodeObjects_FailureMechanismIsNotRelevant_ReturnOnlyFailureMechanismComments()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var pipingFailureMechanism = new PipingFailureMechanism
            {
                IsRelevant = false
            };
            var generalInputParameters = new GeneralPipingInput();
            var pipingProbabilityAssessmentInput = new PipingProbabilityAssessmentInput();
            pipingFailureMechanism.CalculationsGroup.Children.Add(new PipingCalculationScenario(generalInputParameters, pipingProbabilityAssessmentInput));
            pipingFailureMechanism.CalculationsGroup.Children.Add(new PipingCalculationScenario(generalInputParameters, pipingProbabilityAssessmentInput));

            var pipingFailureMechanismContext = new PipingFailureMechanismContext(pipingFailureMechanism, assessmentSection);

            // Call
            var children = info.ChildNodeObjects(pipingFailureMechanismContext).ToArray();

            // Assert
            Assert.AreEqual(1, children.Length);
            var commentContext = (CommentContext<ICommentable>) children[0];
            Assert.AreSame(pipingFailureMechanism, commentContext.CommentContainer);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void GivenMultiplePipingCalculationsWithOutput_WhenClearingOutputFromContextMenu_ThenPipingOutputCleared(bool confirm)
        {
            // Given
            var gui = mocks.StrictMock<IGui>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            var generalInputParameters = new GeneralPipingInput();
            var pipingProbabilityAssessmentInput = new PipingProbabilityAssessmentInput();
            var pipingCalculation1 = new PipingCalculationScenario(generalInputParameters, pipingProbabilityAssessmentInput)
            {
                Output = new TestPipingOutput()
            };
            var pipingCalculation2 = new PipingCalculationScenario(generalInputParameters, pipingProbabilityAssessmentInput)
            {
                Output = new TestPipingOutput()
            };

            var observer = mocks.StrictMock<IObserver>();
            if (confirm)
            {
                observer.Expect(o => o.UpdateObserver()).Repeat.Twice();
            }

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(pipingCalculation1);
            failureMechanism.CalculationsGroup.Children.Add(pipingCalculation2);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanismContext = new PipingFailureMechanismContext(failureMechanism, assessmentSection);

            gui.Expect(cmp => cmp.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);

            mocks.ReplayAll();

            plugin.Gui = gui;

            failureMechanism.CalculationsGroup.Children.Clear();
            failureMechanism.CalculationsGroup.Children.Add(pipingCalculation1);
            failureMechanism.CalculationsGroup.Children.Add(pipingCalculation2);
            failureMechanism.CalculationsGroup.Children.ElementAt(0).Attach(observer);
            failureMechanism.CalculationsGroup.Children.ElementAt(1).Attach(observer);

            ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl);

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

            // When
            contextMenuAdapter.Items[contextMenuClearIndex].PerformClick();

            // Then
            foreach (var calc in failureMechanism.CalculationsGroup.Children.OfType<ICalculation>())
            {
                Assert.AreNotEqual(confirm, calc.HasOutput);
            }

            Assert.AreEqual("Bevestigen", messageBoxTitle);
            Assert.AreEqual("Weet u zeker dat u alle uitvoer wilt wissen?", messageBoxText);
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_HasCalculationWithOutput_ReturnsContextMenuWithCommonItems()
        {
            // Setup
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var failureMechanism = new PipingFailureMechanism();
            var pipingCalculation = new PipingCalculationScenario(failureMechanism.GeneralInput, failureMechanism.PipingProbabilityAssessmentInput)
            {
                Output = new TestPipingOutput()
            };
            failureMechanism.CalculationsGroup.Children.Add(pipingCalculation);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanismContext = new PipingFailureMechanismContext(failureMechanism, assessmentSection);

            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var exportImportHandler = mocks.Stub<IExportImportCommandHandler>();
            var viewCommandsHandler = mocks.Stub<IViewCommands>();

            var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler, exportImportHandler, viewCommandsHandler, failureMechanismContext, treeViewControl);

            var gui = mocks.StrictMock<IGui>();
            gui.Expect(cmp => cmp.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);
            treeViewControl.Expect(tvc => tvc.CanExpandOrCollapseForData(failureMechanismContext)).Repeat.Twice().Return(false);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            ContextMenuStrip menu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl);

            // Assert
            Assert.AreEqual(15, menu.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(menu, 0, CoreCommonGuiResources.Open, CoreCommonGuiResources.Open_ToolTip, CoreCommonGuiResources.OpenIcon, false);

            TestHelper.AssertContextMenuStripContainsItem(menu, 2, RingtoetsCommonFormsResources.FailureMechanismContextMenuStrip_Is_relevant, RingtoetsCommonFormsResources.FailureMechanismContextMenuStrip_Is_relevant_Tooltip, RingtoetsCommonFormsResources.Checkbox_ticked);

            TestHelper.AssertContextMenuStripContainsItem(menu, 4, RingtoetsFormsResources.Validate_all, RingtoetsFormsResources.Validate_all_ToolTip, RingtoetsFormsResources.ValidateAllIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, 5, RingtoetsFormsResources.Calculate_all, RingtoetsFormsResources.Calculate_all_ToolTip, RingtoetsFormsResources.CalculateAllIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, 6, RingtoetsFormsResources.Clear_all_output, RingtoetsFormsResources.Clear_all_output_ToolTip, RingtoetsFormsResources.ClearIcon);

            TestHelper.AssertContextMenuStripContainsItem(menu, 8, CoreCommonGuiResources.Import, CoreCommonGuiResources.Import_ToolTip, CoreCommonGuiResources.ImportIcon, false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 9, CoreCommonGuiResources.Export, CoreCommonGuiResources.Export_ToolTip, CoreCommonGuiResources.ExportIcon, false);

            TestHelper.AssertContextMenuStripContainsItem(menu, 11, CoreCommonGuiResources.Expand_all, CoreCommonGuiResources.Expand_all_ToolTip, CoreCommonGuiResources.ExpandAllIcon, false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 12, CoreCommonGuiResources.Collapse_all, CoreCommonGuiResources.Collapse_all_ToolTip, CoreCommonGuiResources.CollapseAllIcon, false);

            TestHelper.AssertContextMenuStripContainsItem(menu, 14, CoreCommonGuiResources.Properties, CoreCommonGuiResources.Properties_ToolTip, CoreCommonGuiResources.PropertiesHS, false);

            CollectionAssert.AllItemsAreInstancesOfType(new[]
            {
                menu.Items[1],
                menu.Items[3],
                menu.Items[7],
                menu.Items[10],
                menu.Items[13]
            }, typeof(ToolStripSeparator));

            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_PipingFailureMechanismNoOutput_ClearAllOutputDisabled()
        {
            // Setup
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var dataMock = mocks.StrictMock<PipingFailureMechanism>();
            dataMock.Stub(dm => dm.Calculations).Return(new ICalculation[0]);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanismContext = new PipingFailureMechanismContext(dataMock, assessmentSection);

            var gui = mocks.StrictMock<IGui>();
            gui.Expect(cmp => cmp.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            ContextMenuStrip contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl);

            // Assert
            ToolStripItem clearOutputItem = contextMenu.Items[contextMenuClearIndex];
            Assert.IsFalse(clearOutputItem.Enabled);
            Assert.AreEqual("Er zijn geen berekeningen met uitvoer om te wissen.", clearOutputItem.ToolTipText);

            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void ContextMenuStrip_PipingFailureMechanismWithOutput_ClearAllOutputEnabled()
        {
            // Setup
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            var pipingCalculation = new PipingCalculationScenario(new GeneralPipingInput(), new PipingProbabilityAssessmentInput())
            {
                Output = new TestPipingOutput()
            };

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(pipingCalculation);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanismContext = new PipingFailureMechanismContext(failureMechanism, assessmentSection);

            var gui = mocks.StrictMock<IGui>();
            gui.Expect(cmp => cmp.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);

            mocks.ReplayAll();

            plugin.Gui = gui;

            failureMechanism.CalculationsGroup.Children.Add(pipingCalculation);

            // Call
            ContextMenuStrip contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl);

            // Assert
            ToolStripItem clearOutputItem = contextMenu.Items[contextMenuClearIndex];
            Assert.IsTrue(clearOutputItem.Enabled);
            Assert.AreEqual(RingtoetsFormsResources.Clear_all_output_ToolTip, clearOutputItem.ToolTipText);

            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void ContextMenuStrip_PipingFailureMechanismWithNoCalculations_ValidateAndCalculateAllDisabled()
        {
            // Setup
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var dataMock = new PipingFailureMechanism();

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanismContext = new PipingFailureMechanismContext(dataMock, assessmentSection);

            var gui = mocks.StrictMock<IGui>();
            gui.Expect(cmp => cmp.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);

            mocks.ReplayAll();

            plugin.Gui = gui;

            dataMock.CalculationsGroup.Children.Clear();

            // Call
            ContextMenuStrip contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl);

            // Assert
            ToolStripItem validateItem = contextMenu.Items[contextMenuValidateAllIndex];
            ToolStripItem calculateItem = contextMenu.Items[contextMenuCalculateAllIndex];
            Assert.IsFalse(validateItem.Enabled);
            Assert.IsFalse(calculateItem.Enabled);
            Assert.AreEqual(RingtoetsCommonFormsResources.FailureMechanism_CreateCalculateAllItem_No_calculations_to_run, calculateItem.ToolTipText);
            Assert.AreEqual(RingtoetsCommonFormsResources.FailureMechanism_CreateValidateAllItem_No_calculations_to_validate, validateItem.ToolTipText);

            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsRelevant_CallsContextMenuBuilderMethods()
        {
            // Setup
            var pipingFailureMechanism = new PipingFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var pipingFailureMechanismContext = new PipingFailureMechanismContext(pipingFailureMechanism, assessmentSection);

            var gui = mocks.StrictMock<IGui>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();

            menuBuilder.Expect(mb => mb.AddOpenItem()).Return(menuBuilder);
            menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
            menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
            menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
            menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
            menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
            menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
            menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
            menuBuilder.Expect(mb => mb.AddImportItem()).Return(menuBuilder);
            menuBuilder.Expect(mb => mb.AddExportItem()).Return(menuBuilder);
            menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
            menuBuilder.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilder);
            menuBuilder.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilder);
            menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
            menuBuilder.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilder);
            menuBuilder.Expect(mb => mb.Build()).Return(null);

            gui.Expect(cmp => cmp.Get(pipingFailureMechanismContext, treeViewControl)).Return(menuBuilder);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            info.ContextMenuStrip(pipingFailureMechanismContext, null, treeViewControl);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsNotRelevant_CallsContextMenuBuilderMethods()
        {
            // Setup
            var pipingFailureMechanism = new PipingFailureMechanism
            {
                IsRelevant = false
            };
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var pipingFailureMechanismContext = new PipingFailureMechanismContext(pipingFailureMechanism, assessmentSection);

            var gui = mocks.StrictMock<IGui>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();

            menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
            menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
            menuBuilder.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilder);
            menuBuilder.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilder);
            menuBuilder.Expect(mb => mb.Build()).Return(null);

            gui.Expect(cmp => cmp.Get(pipingFailureMechanismContext, treeViewControl)).Return(menuBuilder);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            info.ContextMenuStrip(pipingFailureMechanismContext, null, treeViewControl);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_ClickOnValidateAllItem_ValidateAllChildCalculations()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Clear();

            var validCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            validCalculation.Name = "A";
            var invalidCalculation = PipingCalculationFactory.CreateCalculationWithInvalidData();
            invalidCalculation.Name = "B";

            failureMechanism.CalculationsGroup.Children.Add(validCalculation);
            failureMechanism.CalculationsGroup.Children.Add(invalidCalculation);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanismContext = new PipingFailureMechanismContext(failureMechanism, assessmentSection);

            gui.Expect(g => g.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);

            mocks.ReplayAll();

            plugin.Gui = gui;

            var contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl);

            // Call
            Action call = () => contextMenu.Items[contextMenuValidateAllIndex].PerformClick();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(6, msgs.Length);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", validCalculation.Name), msgs[0]);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", validCalculation.Name), msgs[1]);

                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", invalidCalculation.Name), msgs[2]);
                // Some validation error from validation service
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", invalidCalculation.Name), msgs[5]);
            });
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_ClickOnCalculateAllItem_ScheduleAllChildCalculations()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var mainWindow = mocks.Stub<IMainWindow>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Clear();

            var validCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            validCalculation.Name = "A";
            var invalidCalculation = PipingCalculationFactory.CreateCalculationWithInvalidData();
            invalidCalculation.Name = "B";

            failureMechanism.CalculationsGroup.Children.Add(validCalculation);
            failureMechanism.CalculationsGroup.Children.Add(invalidCalculation);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanismContext = new PipingFailureMechanismContext(failureMechanism, assessmentSection);

            gui.Expect(g => g.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);
            gui.Expect(g => g.MainWindow).Return(mainWindow);

            mocks.ReplayAll();

            plugin.Gui = gui;

            var contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl);

            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            // Call
            contextMenu.Items[contextMenuCalculateAllIndex].PerformClick();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsRelevantAndClickOnIsRelevantItem_MakeFailureMechanismNotRelevant()
        {
            // Setup
            var failureMechanismObserver = mocks.Stub<IObserver>();
            failureMechanismObserver.Expect(o => o.UpdateObserver());

            var failureMechanism = new PipingFailureMechanism
            {
                IsRelevant = true
            };
            failureMechanism.Attach(failureMechanismObserver);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanismContext = new PipingFailureMechanismContext(failureMechanism, assessmentSection);

            var viewCommands = mocks.StrictMock<IViewCommands>();
            viewCommands.Expect(vs => vs.RemoveAllViewsForItem(failureMechanismContext));

            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var gui = mocks.StrictMock<IGui>();
            gui.Stub(g => g.ViewCommands).Return(viewCommands);
            gui.Expect(g => g.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);

            mocks.ReplayAll();

            plugin.Gui = gui;

            var contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl);

            // Call
            contextMenu.Items[contextMenuRelevancyIndexWhenRelevant].PerformClick();

            // Assert
            Assert.IsFalse(failureMechanism.IsRelevant);
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsNotRelevantAndClickOnIsRelevantItem_MakeFailureMechanismRelevant()
        {
            // Setup
            var failureMechanismObserver = mocks.Stub<IObserver>();
            failureMechanismObserver.Expect(o => o.UpdateObserver());

            var failureMechanism = new PipingFailureMechanism
            {
                IsRelevant = false
            };
            failureMechanism.Attach(failureMechanismObserver);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanismContext = new PipingFailureMechanismContext(failureMechanism, assessmentSection);

            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var gui = mocks.StrictMock<IGui>();
            gui.Expect(g => g.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);

            mocks.ReplayAll();

            plugin.Gui = gui;

            var contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl);

            // Call
            contextMenu.Items[contextMenuRelevancyIndexWhenNotRelevant].PerformClick();

            // Assert
            Assert.IsTrue(failureMechanism.IsRelevant);
            mocks.VerifyAll();
        }

        private const int contextMenuRelevancyIndexWhenRelevant = 1;
        private const int contextMenuRelevancyIndexWhenNotRelevant = 0;
        private const int contextMenuValidateAllIndex = 3;
        private const int contextMenuCalculateAllIndex = 4;
        private const int contextMenuClearIndex = 5;
    }
}