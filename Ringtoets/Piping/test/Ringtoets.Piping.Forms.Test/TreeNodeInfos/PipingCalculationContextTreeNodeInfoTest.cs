﻿using System;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
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
using Ringtoets.HydraRing.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Plugin;
using Ringtoets.Piping.Primitives;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Piping.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class PipingCalculationContextTreeNodeInfoTest : NUnitFormTest
    {
        private MockRepository mocks;
        private PipingGuiPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new PipingGuiPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(PipingCalculationContext));
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(PipingCalculationContext), info.TagType);
            Assert.IsNull(info.ForeColor);
            Assert.IsNull(info.CanCheck);
            Assert.IsNull(info.IsChecked);
            Assert.IsNull(info.OnNodeChecked);
            Assert.IsNull(info.CanDrop);
            Assert.IsNull(info.CanInsert);
            Assert.IsNull(info.OnDrop);
        }

        [Test]
        public void Text_Always_ReturnsWrappedDataName()
        {
            // Setup
            var testname = "testName";
            var calculation = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput())
            {
                Name = testname
            };

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionBaseMock = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            var pipingCalculationContext = new PipingCalculationContext(calculation,
                                                                        new[]
                                                                        {
                                                                            new RingtoetsPipingSurfaceLine()
                                                                        },
                                                                        new[]
                                                                        {
                                                                            new TestPipingSoilProfile()
                                                                        },
                                                                        pipingFailureMechanismMock,
                                                                        assessmentSectionBaseMock);
            // Call
            var text = info.Text(pipingCalculationContext);

            // Assert
            Assert.AreEqual(testname, text);

            mocks.VerifyAll();
        }

        [Test]
        public void Image_Always_ReturnsPlaceHolderIcon()
        {
            // Call
            var image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(PipingFormsResources.PipingIcon, image);
        }

        [Test]
        public void EnsureVisibleOnCreate_Always_ReturnsTrue()
        {
            // Setup
            var calculation = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput())
            {
                Output = new PipingOutput(0.0, 0.0, 0.0, 0.0, 0.0, 0.0)
            };

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionBaseMock = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            var pipingCalculationContext = new PipingCalculationContext(calculation,
                                                                        new[]
                                                                        {
                                                                            new RingtoetsPipingSurfaceLine()
                                                                        },
                                                                        new[]
                                                                        {
                                                                            new TestPipingSoilProfile()
                                                                        },
                                                                        pipingFailureMechanismMock,
                                                                        assessmentSectionBaseMock);

            mocks.ReplayAll();

            // Call
            var result = info.EnsureVisibleOnCreate(pipingCalculationContext);

            // Assert
            Assert.IsTrue(result);

            mocks.VerifyAll();
        }

        [Test]
        public void ChildNodeObjects_WithOutputData_ReturnOutputChildNode()
        {
            // Setup
            var calculation = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput())
            {
                Output = new PipingOutput(0.0, 0.0, 0.0, 0.0, 0.0, 0.0)
            };

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionBaseMock = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            var pipingCalculationContext = new PipingCalculationContext(calculation,
                                                                        new[]
                                                                        {
                                                                            new RingtoetsPipingSurfaceLine()
                                                                        },
                                                                        new[]
                                                                        {
                                                                            new TestPipingSoilProfile()
                                                                        },
                                                                        pipingFailureMechanismMock,
                                                                        assessmentSectionBaseMock);

            // Call
            var children = info.ChildNodeObjects(pipingCalculationContext).ToArray();

            // Assert
            Assert.AreEqual(4, children.Length);
            Assert.AreSame(pipingCalculationContext.WrappedData.Comments, children[0]);
            var pipingInputContext = (PipingInputContext) children[1];
            Assert.AreSame(pipingCalculationContext.WrappedData.InputParameters, pipingInputContext.WrappedData);
            CollectionAssert.AreEqual(pipingCalculationContext.AvailablePipingSurfaceLines, pipingInputContext.AvailablePipingSurfaceLines);
            CollectionAssert.AreEqual(pipingCalculationContext.AvailablePipingSoilProfiles, pipingInputContext.AvailablePipingSoilProfiles);
            Assert.AreSame(pipingCalculationContext.WrappedData.SemiProbabilisticOutput, children[2]);
            Assert.IsInstanceOf<EmptyPipingCalculationReport>(children[3]);
        }

        [Test]
        public void ChildNodeObjects_WithoutOutput_ReturnNoChildNodes()
        {
            // Setup
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionBaseMock = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            var pipingCalculationContext = new PipingCalculationContext(new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput()),
                                                                        Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                        Enumerable.Empty<PipingSoilProfile>(),
                                                                        pipingFailureMechanismMock,
                                                                        assessmentSectionBaseMock);

            // Precondition
            Assert.IsFalse(pipingCalculationContext.WrappedData.HasOutput);

            // Call
            var children = info.ChildNodeObjects(pipingCalculationContext).ToArray();

            // Assert
            Assert.AreEqual(4, children.Length);
            Assert.AreSame(pipingCalculationContext.WrappedData.Comments, children[0]);
            var pipingInputContext = (PipingInputContext) children[1];
            Assert.AreSame(pipingCalculationContext.WrappedData.InputParameters, pipingInputContext.WrappedData);
            CollectionAssert.AreEqual(pipingCalculationContext.AvailablePipingSurfaceLines, pipingInputContext.AvailablePipingSurfaceLines);
            CollectionAssert.AreEqual(pipingCalculationContext.AvailablePipingSoilProfiles, pipingInputContext.AvailablePipingSoilProfiles);

            Assert.IsInstanceOf<EmptyPipingOutput>(children[2]);
            Assert.IsInstanceOf<EmptyPipingCalculationReport>(children[3]);
        }

        [Test]
        public void CanRenameNode_Always_ReturnTrue()
        {
            // Call
            var renameAllowed = info.CanRename(null, null);

            // Assert
            Assert.IsTrue(renameAllowed);
        }

        [Test]
        public void OnNodeRenamed_Always_SetNewNameToPipingCalculation()
        {
            // Setup
            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());

            var calculation = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput())
            {
                Name = "<Original name>"
            };

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionBaseMock = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            var pipingCalculationsInputs = new PipingCalculationContext(calculation,
                                                                        Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                        Enumerable.Empty<PipingSoilProfile>(),
                                                                        pipingFailureMechanismMock,
                                                                        assessmentSectionBaseMock);
            pipingCalculationsInputs.Attach(observerMock);

            // Call
            const string newName = "<Insert New Name Here>";
            info.OnNodeRenamed(pipingCalculationsInputs, newName);

            // Assert
            Assert.AreEqual(newName, calculation.Name);
            mocks.VerifyAll();
        }

        [Test]
        public void CanDrag_Always_ReturnTrue()
        {
            // Call
            var canDrag = info.CanDrag(null, null);

            // Assert
            Assert.IsTrue(canDrag);
        }

        [Test]
        public void ContextMenuStrip_PipingCalculationWithoutOutput_ContextMenuItemClearOutputDisabled()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var calculation = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput());
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionBaseMock = mocks.StrictMock<AssessmentSectionBase>();
            var nodeData = new PipingCalculationContext(calculation,
                                                        Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                        Enumerable.Empty<PipingSoilProfile>(),
                                                        pipingFailureMechanismMock,
                                                        assessmentSectionBaseMock);

            gui.Expect(cmp => cmp.Get(nodeData, treeViewControlMock)).Return(new CustomItemsOnlyContextMenuBuilder());

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            var contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControlMock);

            // Assert
            mocks.VerifyAll(); // Expect no calls on arguments

            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 0, RingtoetsCommonFormsResources.Validate, RingtoetsCommonFormsResources.Validate_ToolTip, RingtoetsCommonFormsResources.ValidateIcon);
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 1, RingtoetsCommonFormsResources.Calculate, RingtoetsCommonFormsResources.Calculate_ToolTip, RingtoetsCommonFormsResources.CalculateIcon);
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 2, PipingFormsResources.Clear_output, PipingFormsResources.ClearOutput_No_output_to_clear, RingtoetsCommonFormsResources.ClearIcon, false);
        }

        [Test]
        public void ContextMenuStrip_PipingCalculationWithOutput_ContextMenuItemClearOutputEnabled()
        {
            var gui = mocks.StrictMock<IGui>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var calculation = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput())
            {
                Output = new TestPipingOutput()
            };
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionBaseMock = mocks.StrictMock<AssessmentSectionBase>();
            var nodeData = new PipingCalculationContext(calculation,
                                                        Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                        Enumerable.Empty<PipingSoilProfile>(),
                                                        pipingFailureMechanismMock,
                                                        assessmentSectionBaseMock);

            gui.Expect(cmp => cmp.Get(nodeData, treeViewControlMock)).Return(new CustomItemsOnlyContextMenuBuilder());

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            var contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControlMock);

            // Assert
            mocks.VerifyAll(); // Expect no calls on arguments

            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 0, RingtoetsCommonFormsResources.Validate, RingtoetsCommonFormsResources.Validate_ToolTip, RingtoetsCommonFormsResources.ValidateIcon);
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 1, RingtoetsCommonFormsResources.Calculate, RingtoetsCommonFormsResources.Calculate_ToolTip, RingtoetsCommonFormsResources.CalculateIcon);
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 2, PipingFormsResources.Clear_output, PipingFormsResources.Clear_output_ToolTip, RingtoetsCommonFormsResources.ClearIcon);
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var menuBuilderMock = mocks.Stub<IContextMenuBuilder>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionBaseMock = mocks.StrictMock<AssessmentSectionBase>();
            var nodeData = new PipingCalculationContext(new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput()),
                                                        Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                        Enumerable.Empty<PipingSoilProfile>(),
                                                        pipingFailureMechanismMock,
                                                        assessmentSectionBaseMock);

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
        public void CanRemove_ParentIsPipingCalculationGroupWithCalculation_ReturnTrue(bool groupNameEditable)
        {
            // Setup
            var calculationToBeRemoved = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput());
            var group = new PipingCalculationGroup("", groupNameEditable);
            group.Children.Add(calculationToBeRemoved);

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            var calculationContext = new PipingCalculationContext(calculationToBeRemoved,
                                                                  Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                  Enumerable.Empty<PipingSoilProfile>(),
                                                                  pipingFailureMechanismMock,
                                                                  assessmentSectionMock);
            var groupContext = new PipingCalculationGroupContext(group,
                                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                 Enumerable.Empty<PipingSoilProfile>(),
                                                                 pipingFailureMechanismMock,
                                                                 assessmentSectionMock);

            // Call
            bool removalAllowed = info.CanRemove(calculationContext, groupContext);

            // Assert
            Assert.IsTrue(removalAllowed);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CanRemove_ParentIsPipingCalculationGroupWithoutCalculation_ReturnFalse(bool groupNameEditable)
        {
            // Setup
            var calculationToBeRemoved = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput());
            var group = new PipingCalculationGroup("", groupNameEditable);

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            var calculationContext = new PipingCalculationContext(calculationToBeRemoved,
                                                                  Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                  Enumerable.Empty<PipingSoilProfile>(),
                                                                  pipingFailureMechanismMock,
                                                                  assessmentSectionMock);
            var groupContext = new PipingCalculationGroupContext(group,
                                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                 Enumerable.Empty<PipingSoilProfile>(),
                                                                 pipingFailureMechanismMock,
                                                                 assessmentSectionMock);

            // Call
            bool removalAllowed = info.CanRemove(calculationContext, groupContext);

            // Assert
            Assert.IsFalse(removalAllowed);
        }

        [Test]
        public void CanRemove_EverythingElse_ReturnFalse()
        {
            // Setup
            var dataMock = mocks.StrictMock<object>();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionBaseMock = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            var nodeMock = new PipingCalculationContext(new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput()),
                                                        Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                        Enumerable.Empty<PipingSoilProfile>(),
                                                        pipingFailureMechanismMock,
                                                        assessmentSectionBaseMock);

            // Call
            bool removalAllowed = info.CanRemove(nodeMock, dataMock);

            // Assert
            Assert.IsFalse(removalAllowed);
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

            var elementToBeRemoved = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput());

            var group = new PipingCalculationGroup();
            group.Children.Add(elementToBeRemoved);
            group.Children.Add(new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput()));
            group.Attach(observer);

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            var calculationContext = new PipingCalculationContext(elementToBeRemoved,
                                                                  Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                  Enumerable.Empty<PipingSoilProfile>(),
                                                                  pipingFailureMechanismMock,
                                                                  assessmentSectionMock);
            var groupContext = new PipingCalculationGroupContext(group,
                                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                 Enumerable.Empty<PipingSoilProfile>(),
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
        public void GivenInvalidPipingCalculation_WhenCalculatingFromContextMenu_ThenPipingCalculationNotifiesObserversAndLogMessageAdded()
        {
            // Given
            var gui = mocks.DynamicMock<IGui>();
            var mainWindow = mocks.DynamicMock<IMainWindow>();
            var observer = mocks.StrictMock<IObserver>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var calculation = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput());
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionBaseMock = mocks.StrictMock<AssessmentSectionBase>();
            var pipingCalculationContext = new PipingCalculationContext(calculation,
                                                                        Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                        Enumerable.Empty<PipingSoilProfile>(),
                                                                        pipingFailureMechanismMock,
                                                                        assessmentSectionBaseMock);

            observer.Expect(o => o.UpdateObserver());

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
                // Don't care about dialogs in this test.
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
            var calculation = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput());
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionBaseMock = mocks.StrictMock<AssessmentSectionBase>();
            var pipingCalculationContext = new PipingCalculationContext(calculation,
                                                                        Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                        Enumerable.Empty<PipingSoilProfile>(),
                                                                        pipingFailureMechanismMock,
                                                                        assessmentSectionBaseMock);

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
            var calculation = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput());
            var validPipingInput = new TestPipingInput();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionBaseMock = mocks.StrictMock<AssessmentSectionBase>();
            var pipingCalculationContext = new PipingCalculationContext(calculation,
                                                                        Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                        Enumerable.Empty<PipingSoilProfile>(),
                                                                        pipingFailureMechanismMock,
                                                                        assessmentSectionBaseMock);

            gui.Expect(g => g.Get(pipingCalculationContext, treeViewControlMock)).Return(new CustomItemsOnlyContextMenuBuilder());
            gui.Expect(g => g.MainWindow).Return(mainWindow);

            observer.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            plugin.Gui = gui;

            calculation.InputParameters.DampingFactorExit.Mean = (RoundedDouble)validPipingInput.DampingFactorExit;
            calculation.InputParameters.DarcyPermeability.Mean = (RoundedDouble)validPipingInput.DarcyPermeability;
            calculation.InputParameters.Diameter70.Mean = (RoundedDouble)validPipingInput.Diameter70;
            calculation.InputParameters.ExitPointL = validPipingInput.ExitPointXCoordinate;
            calculation.InputParameters.PhreaticLevelExit.Mean = (RoundedDouble)validPipingInput.PhreaticLevelExit;
            calculation.InputParameters.SeepageLength.Mean = (RoundedDouble)validPipingInput.SeepageLength;
            calculation.InputParameters.ThicknessAquiferLayer.Mean = (RoundedDouble)validPipingInput.ThicknessAquiferLayer;
            calculation.InputParameters.ThicknessCoverageLayer.Mean = (RoundedDouble)validPipingInput.ThicknessCoverageLayer;
            calculation.InputParameters.SurfaceLine = validPipingInput.SurfaceLine;
            calculation.InputParameters.SoilProfile = validPipingInput.SoilProfile;
            calculation.InputParameters.HydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, string.Empty,0.0,0.0) {
                DesignWaterLevel = validPipingInput.AssessmentLevel
            };

            calculation.Attach(observer);

            var contextMenuAdapter = info.ContextMenuStrip(pipingCalculationContext, null, treeViewControlMock);

            DialogBoxHandler = (name, wnd) =>
            {
                // Don't care about dialogs in this test.
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
            var calculation = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput());
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionBaseMock = mocks.StrictMock<AssessmentSectionBase>();
            var pipingCalculationContext = new PipingCalculationContext(calculation,
                                                                        Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                        Enumerable.Empty<PipingSoilProfile>(),
                                                                        pipingFailureMechanismMock,
                                                                        assessmentSectionBaseMock);

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
    }
}