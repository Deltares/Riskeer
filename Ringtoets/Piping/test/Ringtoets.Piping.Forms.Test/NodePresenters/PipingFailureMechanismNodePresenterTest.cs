using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using Core.Common.Utils.Events;

using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;

using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Piping.Calculation.TestUtil;
using Ringtoets.Piping.Data;

using Ringtoets.Piping.Forms.NodePresenters;
using Ringtoets.Piping.Forms.PresentationObjects;

using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;
using TreeNode = Core.Common.Controls.TreeView.TreeNode;
using TreeView = Core.Common.Controls.TreeView.TreeView;

namespace Ringtoets.Piping.Forms.Test.NodePresenters
{
    public class PipingFailureMechanismNodePresenterTest : NUnitFormTest
    {
        private MockRepository mockRepository;

        private const int contextMenuAddFolderIndex = 0; 
        private const int contextMenuAddCalculationIndex = 1;
        private const int contextMenuValidateAllIndex = 3;
        private const int contextMenuCalculateAllIndex = 4;
        private const int contextMenuClearIndex = 5;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void Constructor_NoMenuBuilderProvider_ArgumentNullException()
        {
            // Call
            TestDelegate test = () => new PipingFailureMechanismNodePresenter(null);

            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(CoreCommonGuiResources.NodePresenter_ContextMenuBuilderProvider_required, message);
            StringAssert.EndsWith("contextMenuBuilderProvider", message);
        }

        [Test]
        public void Constructor_ParamsSet_ExpectedValues()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            // Call
            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            // Assert
            Assert.IsInstanceOf<ITreeNodePresenter>(nodePresenter);
            Assert.IsNull(nodePresenter.TreeView);
            Assert.AreEqual(typeof(PipingFailureMechanism), nodePresenter.NodeTagType);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateNode_WithData_InitializeNode()
        {
            // Setup
            mockRepository.StrictMock<IContextMenuBuilderProvider>();
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            var pipingNode = mockRepository.Stub<TreeNode>();
            pipingNode.ForegroundColor = Color.AliceBlue;
            mockRepository.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            var failureMechanism = new PipingFailureMechanism();

            // Call
            nodePresenter.UpdateNode(null, pipingNode, failureMechanism);

            // Assert
            Assert.AreEqual("Dijken - Piping", pipingNode.Text);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), pipingNode.ForegroundColor);
            TestHelper.AssertImagesAreEqual(PipingFormsResources.PipingIcon, pipingNode.Image);
        }

        [Test]
        public void GetChildNodeObjects_Always_ReturnChildDataNodes()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            mockRepository.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            var pipingFailureMechanism = new PipingFailureMechanism();
            pipingFailureMechanism.CalculationsGroup.Children.Add(new PipingCalculation());
            pipingFailureMechanism.CalculationsGroup.Children.Add(new PipingCalculation());

            // Call
            var children = nodePresenter.GetChildNodeObjects(pipingFailureMechanism).OfType<object>().ToArray();

            // Assert
            Assert.AreEqual(3, children.Length);
            var inputsFolder = (CategoryTreeFolder)children[0];
            Assert.AreEqual("Invoer", inputsFolder.Name);
            Assert.AreEqual(TreeFolderCategory.Input, inputsFolder.Category);
            CollectionAssert.AreEqual(new object[]
            {
                pipingFailureMechanism.SectionDivisions,
                pipingFailureMechanism.SurfaceLines,
                pipingFailureMechanism.SoilProfiles,
                pipingFailureMechanism.BoundaryConditions
            }, inputsFolder.Contents);

            var calculationsFolder = (PipingCalculationGroupContext)children[1];
            Assert.AreEqual("Berekeningen", calculationsFolder.WrappedData.Name);
            CollectionAssert.AreEqual(pipingFailureMechanism.CalculationsGroup.Children, calculationsFolder.WrappedData.Children);
            Assert.AreSame(pipingFailureMechanism.SurfaceLines, calculationsFolder.AvailablePipingSurfaceLines);
            Assert.AreSame(pipingFailureMechanism.SoilProfiles, calculationsFolder.AvailablePipingSoilProfiles);

            var outputsFolder = (CategoryTreeFolder)children[2];
            Assert.AreEqual("Uitvoer", outputsFolder.Name);
            Assert.AreEqual(TreeFolderCategory.Output, outputsFolder.Category);
            CollectionAssert.AreEqual(new object[]
            {
                pipingFailureMechanism.AssessmentResult
            }, outputsFolder.Contents);
            mockRepository.VerifyAll(); // Expect no calls on tree node
        }

        [Test]
        public void CanRenameNode_Always_ReturnFalse()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            var nodeMock = mockRepository.StrictMock<TreeNode>();
            mockRepository.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            // Call
            bool renameAllowed = nodePresenter.CanRenameNode(nodeMock);

            // Assert
            Assert.IsFalse(renameAllowed);
            mockRepository.VerifyAll(); // Expect no calls on tree node
        }

        [Test]
        public void CanRenameNodeTo_Always_ReturnFalse()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            var nodeMock = mockRepository.StrictMock<TreeNode>();
            mockRepository.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            // Call
            bool renameAllowed = nodePresenter.CanRenameNodeTo(nodeMock, "<Insert New Name Here>");

            // Assert
            Assert.IsFalse(renameAllowed);
            mockRepository.VerifyAll(); // Expect no calls on tree node
        }

        [Test]
        public void OnNodeRenamed_Always_ThrowInvalidOperationException()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            var nodeMock = mockRepository.StrictMock<PipingFailureMechanism>();
            mockRepository.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            // Call
            TestDelegate call = () => { nodePresenter.OnNodeRenamed(nodeMock, "<Insert New Name Here>"); };

            // Assert
            var exception = Assert.Throws<InvalidOperationException>(call);
            var expectedMessage = string.Format("Kan knoop uit boom van type {0} niet hernoemen.", nodePresenter.GetType().Name);
            Assert.AreEqual(expectedMessage, exception.Message);
            mockRepository.VerifyAll(); // Expect no calls on tree node
        }

        [Test]
        public void OnNodeChecked_Always_DoNothing()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            var nodeMock = mockRepository.StrictMock<TreeNode>();
            mockRepository.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            // Call
            nodePresenter.OnNodeChecked(nodeMock);

            // Assert
            mockRepository.VerifyAll(); // Expect no calls on tree node
        }

        [Test]
        public void CanDrag_Always_ReturnNone()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            var dataMock = mockRepository.StrictMock<PipingFailureMechanism>();
            mockRepository.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            // Call
            DragOperations dragAllowed = nodePresenter.CanDrag(dataMock);

            // Assert
            Assert.AreEqual(DragOperations.None, dragAllowed);
            mockRepository.VerifyAll();
        }

        [Test]
        public void CanDrop_Always_ReturnNone()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            var dataMock = mockRepository.StrictMock<PipingFailureMechanism>();
            var sourceMock = mockRepository.StrictMock<TreeNode>();
            var targetMock = mockRepository.StrictMock<TreeNode>();
            mockRepository.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            // Call
            DragOperations dropAllowed = nodePresenter.CanDrop(dataMock, sourceMock, targetMock, DragOperations.Move);

            // Assert
            Assert.AreEqual(DragOperations.None, dropAllowed);
            mockRepository.VerifyAll(); // Expect no calls on mockRepository.
        }

        [Test]
        public void CanInsert_Always_ReturnFalse()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            var dataMock = mockRepository.StrictMock<PipingFailureMechanism>();
            var sourceMock = mockRepository.StrictMock<TreeNode>();
            var targetMock = mockRepository.StrictMock<TreeNode>();
            mockRepository.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            // Call
            bool insertionAllowed = nodePresenter.CanInsert(dataMock, sourceMock, targetMock);

            // Assert
            Assert.IsFalse(insertionAllowed);
            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void OnDragDrop_Always_DoNothing()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            var dataMock = mockRepository.StrictMock<object>();
            var dataMockOwner = mockRepository.StrictMock<object>();
            var target = mockRepository.StrictMock<PipingFailureMechanism>();
            mockRepository.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            // Call
            nodePresenter.OnDragDrop(dataMock, dataMockOwner, target, DragOperations.Move, 2);

            // Assert
            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void OnNodeSelected_Always_DoNothing()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            var dataMock = mockRepository.StrictMock<PipingFailureMechanism>();
            mockRepository.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            // Call
            nodePresenter.OnNodeSelected(dataMock);

            // Assert
            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void OnPropertyChange_Always_DoNothing()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            var dataMock = mockRepository.StrictMock<PipingFailureMechanism>();
            var nodeMock = mockRepository.StrictMock<TreeNode>();
            var eventArgsMock = mockRepository.StrictMock<PropertyChangedEventArgs>("");
            mockRepository.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            // Call
            nodePresenter.OnPropertyChanged(dataMock, nodeMock, eventArgsMock);

            // Assert
            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void OnCollectionChange_Always_DoNothing()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            var dataMock = mockRepository.StrictMock<PipingFailureMechanism>();
            var eventArgsMock = mockRepository.StrictMock<NotifyCollectionChangeEventArgs>();
            mockRepository.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            // Call
            nodePresenter.OnCollectionChanged(dataMock, eventArgsMock);

            // Assert
            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void CanRemove_Always_ReturnFalse()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            var parentNodeDataMock = mockRepository.StrictMock<object>();
            var nodeMock = mockRepository.StrictMock<PipingFailureMechanism>();
            mockRepository.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            // Call
            bool removalAllowed = nodePresenter.CanRemove(parentNodeDataMock, nodeMock);

            // Assert
            Assert.IsFalse(removalAllowed);
            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void RemoveNodeData_PipingFailureMechanism_PipingFailureMechanismRemovedFromAssessmentSection()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            // Call
            TestDelegate call = () => nodePresenter.RemoveNodeData(new object(), new PipingFailureMechanism());

            // Assert
            Assert.Throws<InvalidOperationException>(call);
            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void GivenMultiplePipingCalculationsWithOutput_WhenClearingOutputFromContextMenu_ThenPipingOutputCleared(bool confirm)
        {
            // Given
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var observer = mockRepository.StrictMock<IObserver>();
            var nodeMock = mockRepository.Stub<TreeNode>();
            var dataMock = mockRepository.StrictMock<PipingFailureMechanism>();

            if (confirm)
            {
                observer.Expect(o => o.UpdateObserver()).Repeat.Twice();
            }

            contextMenuBuilderProviderMock.Expect(cmp => cmp.Get(nodeMock)).Return(menuBuilder);

            mockRepository.ReplayAll();

            dataMock.CalculationsGroup.Children.Add(new PipingCalculation
            {
                Output = new TestPipingOutput()
            });
            dataMock.CalculationsGroup.Children.Add(new PipingCalculation
            {
                Output = new TestPipingOutput()
            });
            dataMock.CalculationsGroup.Children.ElementAt(0).Attach(observer);
            dataMock.CalculationsGroup.Children.ElementAt(1).Attach(observer);

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            ContextMenuStrip contextMenuAdapter = nodePresenter.GetContextMenu(nodeMock, dataMock);

            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);
                Assert.AreEqual("Weet u zeker dat u alle uitvoer wilt wissen?", messageBox.Text);
                Assert.AreEqual("Bevestigen", messageBox.Title);
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
            Assert.AreNotEqual(confirm, dataMock.CalculationsGroup.HasOutput);

            mockRepository.VerifyAll();
        }

        [Test]
        public void GetContextMenu_HasCalculationWithOutput_ReturnsContextMenuWithCommonItems()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.GetPipingCalculations().First().Output = new TestPipingOutput();

            var nodeMock = mockRepository.Stub<TreeNode>();
            nodeMock.Tag = failureMechanism;

            var commandHandler = mockRepository.Stub<IGuiCommandHandler>();

            mockRepository.ReplayAll();

            var contextMenuBuilder = new ContextMenuBuilder(commandHandler, nodeMock);
            var contextMenuBuilderProviderMock = new SimpleContextMenuBuilderProvider(contextMenuBuilder);

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            // Call
            ContextMenuStrip menu = nodePresenter.GetContextMenu(nodeMock, failureMechanism);

            // Assert
            Assert.AreEqual(12, menu.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddFolderIndex, PipingFormsResources.PipingCalculationGroup_Add_PipingCalculationGroup, PipingFormsResources.PipingFailureMechanism_Add_PipingCalculationGroup_Tooltip, PipingFormsResources.AddFolderIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationIndex, PipingFormsResources.PipingCalculationGroup_Add_PipingCalculation, PipingFormsResources.PipingFailureMechanism_Add_PipingCalculation_Tooltip, PipingFormsResources.PipingIcon);

            TestHelper.AssertContextMenuStripContainsItem(menu, 3, RingtoetsFormsResources.Validate_all, RingtoetsFormsResources.Validate_all_ToolTip, RingtoetsFormsResources.ValidateAllIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, 4, RingtoetsFormsResources.Calculate_all, RingtoetsFormsResources.Calculate_all_ToolTip, RingtoetsFormsResources.CalculateAllIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearIndex, RingtoetsFormsResources.Clear_all_output, RingtoetsFormsResources.Clear_all_output_ToolTip, RingtoetsFormsResources.ClearIcon);

            TestHelper.AssertContextMenuStripContainsItem(menu, 7, CoreCommonGuiResources.Import, CoreCommonGuiResources.Import_ToolTip, CoreCommonGuiResources.ImportIcon, false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 8, CoreCommonGuiResources.Export, CoreCommonGuiResources.Export_ToolTip, CoreCommonGuiResources.ExportIcon, false);

            TestHelper.AssertContextMenuStripContainsItem(menu, 10, CoreCommonGuiResources.Expand_all, CoreCommonGuiResources.Expand_all_ToolTip, CoreCommonGuiResources.ExpandAllIcon, false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 11, CoreCommonGuiResources.Collapse_all, CoreCommonGuiResources.Collapse_all_ToolTip, CoreCommonGuiResources.CollapseAllIcon, false);

            CollectionAssert.AllItemsAreInstancesOfType(new[] { menu.Items[2], menu.Items[6], menu.Items[9] }, typeof(ToolStripSeparator));

            mockRepository.VerifyAll();
        }

        [Test]
        public void GetContextMenu_PipingFailureMechanismNoOutput_ClearAllOutputDisabled()
        {
            // Setup
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var nodeMock = mockRepository.StrictMock<TreeNode>();
            var dataMock = mockRepository.StrictMock<PipingFailureMechanism>();

            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            contextMenuBuilderProviderMock.Expect(cmp => cmp.Get(nodeMock)).Return(menuBuilder);

            mockRepository.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);
          
            // Call
            ContextMenuStrip contextMenu = nodePresenter.GetContextMenu(nodeMock, dataMock);

            // Assert
            ToolStripItem clearOutputItem = contextMenu.Items[contextMenuClearIndex];
            Assert.IsFalse(clearOutputItem.Enabled);
            Assert.AreEqual("Er zijn geen berekeningen met uitvoer om te wissen.", clearOutputItem.ToolTipText);

            mockRepository.VerifyAll(); // Expect no calls on arguments
        }
        
        [Test]
        public void GetContextMenu_PipingFailureMechanismWithOutput_ClearAllOutputEnabled()
        {
            // Setup
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var nodeMock = mockRepository.StrictMock<TreeNode>();
            var dataMock = mockRepository.StrictMock<PipingFailureMechanism>();
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            contextMenuBuilderProviderMock.Expect(cmp => cmp.Get(nodeMock)).Return(menuBuilder);

            mockRepository.ReplayAll();

            dataMock.CalculationsGroup.Children.Add(new PipingCalculation
            {
                Output = new TestPipingOutput()
            });

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            // Call
            ContextMenuStrip contextMenu = nodePresenter.GetContextMenu(nodeMock, dataMock);

            // Assert
            ToolStripItem clearOutputItem = contextMenu.Items[contextMenuClearIndex];
            Assert.IsTrue(clearOutputItem.Enabled);
            Assert.AreEqual(RingtoetsFormsResources.Clear_all_output_ToolTip, clearOutputItem.ToolTipText);

            mockRepository.VerifyAll(); // Expect no calls on arguments
        }
        
        [Test]
        public void GetContextMenu_PipingFailureMechanismWithNoCalculations_ValidateAndCalculateAllDisabled()
        {
            // Setup
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var nodeMock = new TreeNode(null);
            var dataMock = new PipingFailureMechanism();
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            contextMenuBuilderProviderMock.Expect(cmp => cmp.Get(nodeMock)).Return(menuBuilder);

            mockRepository.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);
            dataMock.CalculationsGroup.Children.Clear();

            // Call
            ContextMenuStrip contextMenu = nodePresenter.GetContextMenu(nodeMock, dataMock);

            // Assert
            ToolStripItem validateItem = contextMenu.Items[contextMenuValidateAllIndex];
            ToolStripItem calculateItem = contextMenu.Items[contextMenuCalculateAllIndex];
            Assert.IsFalse(validateItem.Enabled);
            Assert.IsFalse(calculateItem.Enabled);
            Assert.AreEqual(PipingFormsResources.PipingFailureMechanismNodePresenter_CreateCalculateAllItem_No_calculations_to_run, calculateItem.ToolTipText);
            Assert.AreEqual(PipingFormsResources.PipingFailureMechanismNodePresenter_CreateValidateAllItem_No_calculations_to_validate, validateItem.ToolTipText);

            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void GetContextMenu_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            var menuBuilderMock = mockRepository.StrictMock<IContextMenuBuilder>();
            var nodeMock = mockRepository.StrictMock<TreeNode>();

            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddImportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            contextMenuBuilderProviderMock.Expect(cmp => cmp.Get(nodeMock)).Return(menuBuilderMock);

            mockRepository.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            // Call
            nodePresenter.GetContextMenu(nodeMock, new PipingFailureMechanism());

            // Assert
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetContextMenu_ClickOnAddCalculationItem_NewPipingCalculationInstanceAddedToFailureMechanismAndNotifyObservers()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Clear();
            failureMechanism.CalculationsGroup.Children.Add(new PipingCalculation());

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var newCalculationContextNode = mockRepository.StrictMock<TreeNode>();

            var failureMechanismCalculationsNode = mockRepository.StrictMock<TreeNode>();
            failureMechanismCalculationsNode.Expect(n => n.IsExpanded).Return(false);
            failureMechanismCalculationsNode.Expect(n => n.Expand());
            failureMechanismCalculationsNode.Expect(n => n.Nodes).WhenCalled(invocation =>
            {
                if (failureMechanism.CalculationsGroup.Children.Count == 2)
                {
                    invocation.ReturnValue = new List<TreeNode>
                    {
                        newCalculationContextNode
                    };
                }
            }).Return(null);

            var failureMechanismNode = mockRepository.StrictMock<TreeNode>();
            failureMechanismNode.Expect(n => n.IsExpanded).Return(false);
            failureMechanismNode.Expect(n => n.Expand());
            failureMechanismNode.Expect(n => n.Nodes).Return(new List<TreeNode>
            {
                null,
                failureMechanismCalculationsNode,
                null
            });

            var observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());

            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            contextMenuBuilderProviderMock.Expect(cmp => cmp.Get(failureMechanismNode)).Return(menuBuilder);

            var treeView = mockRepository.Stub<TreeView>();

            mockRepository.ReplayAll();

            failureMechanism.Attach(observerMock);

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock)
            {
                TreeView = treeView
            };

            // Precondition
            Assert.AreEqual(1, failureMechanism.CalculationsGroup.Children.Count);

            // Call
            ContextMenuStrip contextMenu = nodePresenter.GetContextMenu(failureMechanismNode, failureMechanism);
            ToolStripItem addCalculationItem = contextMenu.Items[contextMenuAddCalculationIndex];
            addCalculationItem.PerformClick();

            // Assert
            Assert.AreEqual(2, failureMechanism.CalculationsGroup.Children.Count);
            IPipingCalculationItem addedItem = failureMechanism.CalculationsGroup.Children.ElementAt(1);
            Assert.AreEqual("Nieuwe berekening (1)", addedItem.Name,
                "Because there is already an item with the same default name, '(1)' should be appended.");
            Assert.IsInstanceOf<PipingCalculation>(addedItem);

            Assert.AreSame(newCalculationContextNode, treeView.SelectedNode);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetContextMenu_ClickOnAddFolderItem_NewPipingCalculationGroupInstanceAddedToFailureMechanismAndNotifyObservers()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Clear();
            failureMechanism.CalculationsGroup.Children.Add(new PipingCalculationGroup());

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var newCalculationGroupContextNode = mockRepository.StrictMock<TreeNode>();

            var failureMechanismCalculationsNode = mockRepository.StrictMock<TreeNode>();
            failureMechanismCalculationsNode.Expect(n => n.IsExpanded).Return(false);
            failureMechanismCalculationsNode.Expect(n => n.Expand());
            failureMechanismCalculationsNode.Expect(n => n.Nodes).WhenCalled(invocation =>
            {
                if (failureMechanism.CalculationsGroup.Children.Count == 2)
                {
                    invocation.ReturnValue = new List<TreeNode>
                    {
                        newCalculationGroupContextNode
                    };
                }
            }).Return(null);

            var failureMechanismNode = mockRepository.StrictMock<TreeNode>();
            failureMechanismNode.Expect(n => n.IsExpanded).Return(false);
            failureMechanismNode.Expect(n => n.Expand());
            failureMechanismNode.Expect(n => n.Nodes).Return(new List<TreeNode>
            {
                null,
                failureMechanismCalculationsNode,
                null
            });

            var observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());

            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            contextMenuBuilderProviderMock.Expect(cmp => cmp.Get(failureMechanismNode)).Return(menuBuilder);

            var treeView = mockRepository.Stub<TreeView>();

            mockRepository.ReplayAll();

            failureMechanism.Attach(observerMock);

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock)
            {
                TreeView = treeView
            };

            // Precondition
            Assert.AreEqual(1, failureMechanism.CalculationsGroup.Children.Count);

            // Call
            ContextMenuStrip contextMenu = nodePresenter.GetContextMenu(failureMechanismNode, failureMechanism);
            ToolStripItem addCalculationItem = contextMenu.Items[contextMenuAddFolderIndex];
            addCalculationItem.PerformClick();

            // Assert
            Assert.AreEqual(2, failureMechanism.CalculationsGroup.Children.Count);
            IPipingCalculationItem addedItem = failureMechanism.CalculationsGroup.Children.ElementAt(1);
            Assert.AreEqual("Nieuwe map (1)", addedItem.Name,
                "Because there is already an item with the same default name, '(1)' should be appended.");
            Assert.IsInstanceOf<PipingCalculationGroup>(addedItem);

            Assert.AreSame(newCalculationGroupContextNode, treeView.SelectedNode);
            mockRepository.VerifyAll();
        }
    }
}