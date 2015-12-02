﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.TestUtils;
using Core.Common.Gui.TestUtils.ContextMenu;
using Core.Common.TestUtils;
using Core.Common.Utils.Collections;
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

namespace Ringtoets.Piping.Forms.Test.NodePresenters
{
    public class PipingFailureMechanismNodePresenterTest
    {
        private MockRepository mockRepository;

        private const int contextMenuAddCalculationIndex = 0;
        private const int contextMenuAddFolderIndex = 1;
        private const int contextMenuCalculateAllIndex = 2;
        private const int contextMenuClearIndex = 3;

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
        public void DefaultConstructor_ExpectedValues()
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
            var pipingNode = mockRepository.Stub<ITreeNode>();
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
            var nodeMock = mockRepository.StrictMock<ITreeNode>();
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
            var nodeMock = mockRepository.StrictMock<ITreeNode>();
            mockRepository.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            // Call
            bool renameAllowed = nodePresenter.CanRenameNodeTo(nodeMock, "<Insert New Name Here>");

            // Assert
            Assert.IsFalse(renameAllowed);
            mockRepository.ReplayAll(); // Expect no calls on tree node
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
            mockRepository.ReplayAll(); // Expect no calls on tree node
        }

        [Test]
        public void OnNodeChecked_Always_DoNothing()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            var nodeMock = mockRepository.StrictMock<ITreeNode>();
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
            var sourceMock = mockRepository.StrictMock<ITreeNode>();
            var targetMock = mockRepository.StrictMock<ITreeNode>();
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
            var sourceMock = mockRepository.StrictMock<ITreeNode>();
            var targetMock = mockRepository.StrictMock<ITreeNode>();
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
            var dataMock = mockRepository.StrictMock<PipingFailureMechanism>();
            var sourceParentNodeMock = mockRepository.StrictMock<ITreeNode>();
            var targetParentNodeDataMock = mockRepository.StrictMock<ITreeNode>();
            mockRepository.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            // Call
            nodePresenter.OnDragDrop(dataMock, sourceParentNodeMock, targetParentNodeDataMock, DragOperations.Move, 2);

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
            var nodeMock = mockRepository.StrictMock<ITreeNode>();
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
            var eventArgsMock = mockRepository.StrictMock<NotifyCollectionChangingEventArgs>();
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
        public void GivenMultiplePipingCalculationsWithOutput_WhenClearingOutputFromContextMenu_ThenPipingOutputCleared()
        {
            // Given
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Twice();
            var nodeMock = mockRepository.Stub<ITreeNode>();

            var dataMock = mockRepository.StrictMock<PipingFailureMechanism>();
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

            contextMenuBuilderProviderMock.Expect(cmp => cmp.Get(nodeMock)).Return(menuBuilder);

            mockRepository.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            ContextMenuStrip contextMenuAdapter = nodePresenter.GetContextMenu(nodeMock, dataMock);

            // When
            contextMenuAdapter.Items[contextMenuClearIndex].PerformClick();

            // Then
            Assert.IsFalse(dataMock.CalculationsGroup.HasOutput);

            mockRepository.VerifyAll();
        }

        [Test]
        public void GetContextMenu_Always_AddFourCustomItems()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            var nodeMock = mockRepository.Stub<ITreeNode>();
            var failureMechanism = mockRepository.StrictMock<PipingFailureMechanism>();

            contextMenuBuilderProviderMock.Expect(cmp => cmp.Get(nodeMock)).Return(menuBuilder);

            mockRepository.ReplayAll();

            nodeMock.Tag = failureMechanism;

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            // Call
            ContextMenuStrip menu = nodePresenter.GetContextMenu(nodeMock, failureMechanism);

            // Assert
            Assert.AreEqual(4, menu.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationIndex, PipingFormsResources.PipingFailureMechanism_Add_PipingCalculation, PipingFormsResources.PipingFailureMechanism_Add_PipingCalculation_Tooltip, PipingFormsResources.PipingIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddFolderIndex, PipingFormsResources.PipingFailureMechanism_Add_PipingCalculationGroup, PipingFormsResources.PipingFailureMechanism_Add_PipingCalculationGroup_Tooltip, PipingFormsResources.AddFolderIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndex, RingtoetsFormsResources.Calculate_all, PipingFormsResources.PipingFailureMechanism_Calculate_Tooltip, RingtoetsFormsResources.CalculateAllIcon);

            mockRepository.VerifyAll();
        }

        [Test]
        public void GetContextMenu_PipingFailureMechanismNoOutput_ClearAllOutputDisabled()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            var nodeMock = mockRepository.StrictMock<ITreeNode>();
            var dataMock = mockRepository.StrictMock<PipingFailureMechanism>();

            contextMenuBuilderProviderMock.Expect(cmp => cmp.Get(nodeMock)).Return(menuBuilder);

            mockRepository.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);
          
            // Call
            ContextMenuStrip contextMenu = nodePresenter.GetContextMenu(nodeMock, dataMock);

            // Assert
            Assert.AreEqual(4, contextMenu.Items.Count);

            ToolStripItem clearOutputItem = contextMenu.Items[contextMenuClearIndex];
            Assert.IsFalse(clearOutputItem.Enabled);
            Assert.AreEqual("Er zijn geen berekeningen met uitvoer om te wissen.", clearOutputItem.ToolTipText);

            mockRepository.VerifyAll(); // Expect no calls on arguments
        }
        
        [Test]
        public void GetContextMenu_PipingFailureMechanismWithOutput_ClearAllOutputEnabled()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            var nodeMock = mockRepository.StrictMock<ITreeNode>();
            var dataMock = mockRepository.StrictMock<PipingFailureMechanism>();

            contextMenuBuilderProviderMock.Expect(cmp => cmp.Get(nodeMock)).Return(menuBuilder);

            dataMock.CalculationsGroup.Children.Add(new PipingCalculation
            {
                Output = new TestPipingOutput()
            });

            mockRepository.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            // Call
            ContextMenuStrip contextMenu = nodePresenter.GetContextMenu(nodeMock, dataMock);

            // Assert
            Assert.AreEqual(4, contextMenu.Items.Count);

            ToolStripItem clearOutputItem = contextMenu.Items[contextMenuClearIndex];
            Assert.IsTrue(clearOutputItem.Enabled);
            Assert.AreEqual(RingtoetsFormsResources.Clear_all_output_ToolTip, clearOutputItem.ToolTipText);

            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void GetContextMenu_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            var menuBuilderMock = mockRepository.StrictMock<IContextMenuBuilder>();
            var nodeMock = mockRepository.StrictMock<ITreeNode>();

            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddImportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
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
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            var nodeMock = mockRepository.StrictMock<ITreeNode>();
            var observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());

            contextMenuBuilderProviderMock.Expect(cmp => cmp.Get(nodeMock)).Return(menuBuilder);

            mockRepository.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.Attach(observerMock);

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            mockRepository.ReplayAll();

            // Precondition
            Assert.AreEqual(1, failureMechanism.CalculationsGroup.Children.Count);

            // Call
            ContextMenuStrip contextMenu = nodePresenter.GetContextMenu(nodeMock, failureMechanism);
            ToolStripItem addCalculationItem = contextMenu.Items[contextMenuAddCalculationIndex];
            addCalculationItem.PerformClick();

            // Assert
            Assert.AreEqual(2, failureMechanism.CalculationsGroup.Children.Count);
            IPipingCalculationItem addedItem = failureMechanism.CalculationsGroup.Children.ElementAt(1);
            Assert.AreEqual("Nieuwe berekening (1)", addedItem.Name);
            Assert.IsInstanceOf<PipingCalculation>(addedItem);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetContextMenu_ClickOnAddFolderItem_NewPipingCalculationGroupInstanceAddedToFailureMechanismAndNotifyObservers()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            var nodeMock = mockRepository.StrictMock<ITreeNode>();
            var observerMock = mockRepository.StrictMock<IObserver>();

            contextMenuBuilderProviderMock.Expect(cmp => cmp.Get(nodeMock)).Return(menuBuilder);

            observerMock.Expect(o => o.UpdateObserver());

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.Attach(observerMock);
            failureMechanism.CalculationsGroup.Children.Clear();
            failureMechanism.CalculationsGroup.Children.Add(new PipingCalculationGroup());

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            mockRepository.ReplayAll();

            // Precondition
            Assert.AreEqual(1, failureMechanism.CalculationsGroup.Children.Count);

            // Call
            ContextMenuStrip contextMenu = nodePresenter.GetContextMenu(nodeMock, failureMechanism);
            ToolStripItem addCalculationItem = contextMenu.Items[contextMenuAddFolderIndex];
            addCalculationItem.PerformClick();

            // Assert
            Assert.AreEqual(2, failureMechanism.CalculationsGroup.Children.Count);
            IPipingCalculationItem addedItem = failureMechanism.CalculationsGroup.Children.ElementAt(1);
            Assert.AreEqual("Nieuwe map (1)", addedItem.Name);
            Assert.IsInstanceOf<PipingCalculationGroup>(addedItem);
            mockRepository.VerifyAll();
        }
    }
}