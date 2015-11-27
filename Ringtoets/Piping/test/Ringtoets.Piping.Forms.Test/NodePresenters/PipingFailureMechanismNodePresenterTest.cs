﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Core.Common.Base;
using Core.Common.Controls;
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

namespace Ringtoets.Piping.Forms.Test.NodePresenters
{
    public class PipingFailureMechanismNodePresenterTest
    {

        private const int contextMenuAddCalculationIndex = 0;
        private const int contextMenuAddFolderIndex = 1;
        private const int contextMenuCalculateIndex = 2;
        private const int contextMenuClearIndex = 3;

        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var nodePresenter = new PipingFailureMechanismNodePresenter();

            // Assert
            Assert.IsInstanceOf<ITreeNodePresenter>(nodePresenter);
            Assert.IsNull(nodePresenter.TreeView);
            Assert.AreEqual(typeof(PipingFailureMechanism), nodePresenter.NodeTagType);
        }

        [Test]
        public void UpdateNode_WithData_InitializeNode()
        {
            // Setup
            var mocks = new MockRepository();
            var pipingNode = mocks.Stub<ITreeNode>();
            pipingNode.ForegroundColor = Color.AliceBlue;
            mocks.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter();

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
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter();

            var pipingFailureMechanism = new PipingFailureMechanism();
            pipingFailureMechanism.Calculations.Add(new PipingCalculation());
            pipingFailureMechanism.Calculations.Add(new PipingCalculation());

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

            var calculationsFolder = (CategoryTreeFolder)children[1];
            Assert.AreEqual("Berekeningen", calculationsFolder.Name);
            Assert.AreEqual(TreeFolderCategory.General, calculationsFolder.Category);
            var pipingCalculationChildObjects = calculationsFolder.Contents.Cast<PipingCalculationContext>()
                                                                        .Take(pipingFailureMechanism.Calculations.Count)
                                                                        .ToArray();
            CollectionAssert.AreEqual(pipingFailureMechanism.Calculations, pipingCalculationChildObjects.Select(pci => pci.WrappedData).ToArray());
            foreach (var pipingCalculationContext in pipingCalculationChildObjects)
            {
                Assert.AreSame(pipingFailureMechanism.SurfaceLines, pipingCalculationContext.AvailablePipingSurfaceLines);
                Assert.AreSame(pipingFailureMechanism.SoilProfiles, pipingCalculationContext.AvailablePipingSoilProfiles);
            }

            var outputsFolder = (CategoryTreeFolder)children[2];
            Assert.AreEqual("Uitvoer", outputsFolder.Name);
            Assert.AreEqual(TreeFolderCategory.Output, outputsFolder.Category);
            CollectionAssert.AreEqual(new object[]
            {
                pipingFailureMechanism.AssessmentResult
            }, outputsFolder.Contents);
            mocks.VerifyAll(); // Expect no calls on tree node
        }

        [Test]
        public void CanRenameNode_Always_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter();

            // Call
            var renameAllowed = nodePresenter.CanRenameNode(nodeMock);

            // Assert
            Assert.IsFalse(renameAllowed);
            mocks.VerifyAll(); // Expect no calls on tree node
        }

        [Test]
        public void CanRenameNodeTo_Always_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter();

            // Call
            var renameAllowed = nodePresenter.CanRenameNodeTo(nodeMock, "<Insert New Name Here>");

            // Assert
            Assert.IsFalse(renameAllowed);
            mocks.ReplayAll(); // Expect no calls on tree node
        }

        [Test]
        public void OnNodeRenamed_Always_ThrowInvalidOperationException()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter();

            // Call
            TestDelegate call = () => { nodePresenter.OnNodeRenamed(nodeMock, "<Insert New Name Here>"); };

            // Assert
            var exception = Assert.Throws<InvalidOperationException>(call);
            var expectedMessage = string.Format("Kan knoop uit boom van type {0} niet hernoemen.", nodePresenter.GetType().Name);
            Assert.AreEqual(expectedMessage, exception.Message);
            mocks.ReplayAll(); // Expect no calls on tree node
        }

        [Test]
        public void OnNodeChecked_Always_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter();

            // Call
            nodePresenter.OnNodeChecked(nodeMock);

            // Assert
            mocks.VerifyAll(); // Expect no calls on tree node
        }

        [Test]
        public void CanDrag_Always_ReturnNone()
        {
            // Setup
            var mocks = new MockRepository();
            var dataMock = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter();

            // Call
            DragOperations dragAllowed = nodePresenter.CanDrag(dataMock);

            // Assert
            Assert.AreEqual(DragOperations.None, dragAllowed);
            mocks.VerifyAll();
        }

        [Test]
        public void CanDrop_Always_ReturnNone()
        {
            // Setup
            var mocks = new MockRepository();
            var dataMock = mocks.StrictMock<PipingFailureMechanism>();
            var sourceMock = mocks.StrictMock<ITreeNode>();
            var targetMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter();

            // Call
            DragOperations dropAllowed = nodePresenter.CanDrop(dataMock, sourceMock, targetMock, DragOperations.Move);

            // Assert
            Assert.AreEqual(DragOperations.None, dropAllowed);
            mocks.VerifyAll(); // Expect no calls on mocks.
        }

        [Test]
        public void CanInsert_Always_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var dataMock = mocks.StrictMock<PipingFailureMechanism>();
            var sourceMock = mocks.StrictMock<ITreeNode>();
            var targetMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter();

            // Call
            bool insertionAllowed = nodePresenter.CanInsert(dataMock, sourceMock, targetMock);

            // Assert
            Assert.IsFalse(insertionAllowed);
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void OnDragDrop_Always_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            var dataMock = mocks.StrictMock<PipingFailureMechanism>();
            var sourceParentNodeMock = mocks.StrictMock<ITreeNode>();
            var targetParentNodeDataMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter();

            // Call
            nodePresenter.OnDragDrop(dataMock, sourceParentNodeMock, targetParentNodeDataMock, DragOperations.Move, 2);

            // Assert
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void OnNodeSelected_Always_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            var dataMock = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter();

            // Call
            nodePresenter.OnNodeSelected(dataMock);

            // Assert
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void GetContextMenu_PipingFailureMechanism_ReturnsContextMenuWithFourItems()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodeData = new PipingFailureMechanism();
            ((PipingCalculation)nodeData.Calculations.First()).Output = new TestPipingOutput();

            var nodePresenter = new PipingFailureMechanismNodePresenter();

            // Call
            var contextMenu = nodePresenter.GetContextMenu(nodeMock, nodeData);

            // Assert
            Assert.AreEqual(4, contextMenu.Items.Count);
            var addCalculationItem = contextMenu.Items[contextMenuAddCalculationIndex];
            Assert.AreEqual("Berekening toevoegen", addCalculationItem.Text);
            Assert.AreEqual("Voeg een nieuwe piping berekening toe aan het faalmechanisme.", addCalculationItem.ToolTipText);
            Assert.AreEqual(16, addCalculationItem.Image.Width);
            Assert.AreEqual(16, addCalculationItem.Image.Height);

            var addFolderItem = contextMenu.Items[contextMenuAddFolderIndex];
            Assert.AreEqual("Map toevoegen", addFolderItem.Text);
            Assert.AreEqual("Voeg een nieuwe berekeningsmap toe aan het faalmechanisme.", addFolderItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(PipingFormsResources.AddFolderIcon, addFolderItem.Image);

            var runAllItem = contextMenu.Items[contextMenuCalculateIndex];
            Assert.AreEqual("Alles be&rekenen", runAllItem.Text);
            Assert.AreEqual("Valideer en voer alle berekeningen binnen het piping faalmechanisme uit.", runAllItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.CalculateAllIcon, runAllItem.Image);

            var clearOutputItem = contextMenu.Items[contextMenuClearIndex];
            Assert.AreEqual("&Wis alle uitvoer", clearOutputItem.Text);
            Assert.AreEqual("Wis de uitvoer van alle berekeningen binnen het faalmechanisme.", clearOutputItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.ClearIcon, clearOutputItem.Image);

            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void GetContextMenu_PipingFailureMechanismNoOutput_ClearAllOutputDisabled()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var dataMock = mocks.StrictMock<PipingFailureMechanism>();

            var nodePresenter = new PipingFailureMechanismNodePresenter();

            mocks.ReplayAll();

            // Call
            var contextMenu = nodePresenter.GetContextMenu(nodeMock, dataMock);

            // Assert
            Assert.AreEqual(4, contextMenu.Items.Count);

            var clearOutputItem = contextMenu.Items[contextMenuClearIndex];
            Assert.IsFalse(clearOutputItem.Enabled);
            Assert.AreEqual("Er zijn geen berekeningen met uitvoer om te wissen.", clearOutputItem.ToolTipText);

            mocks.VerifyAll(); // Expect no calls on arguments
        }
        
        [Test]
        public void GetContextMenu_PipingFailureMechanismWithOutput_ClearAllOutputEnabled()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var dataMock = mocks.StrictMock<PipingFailureMechanism>();
            dataMock.Calculations.Add(new PipingCalculation
            {
                Output = new TestPipingOutput()
            });

            var nodePresenter = new PipingFailureMechanismNodePresenter();

            mocks.ReplayAll();

            // Call
            var contextMenu = nodePresenter.GetContextMenu(nodeMock, dataMock);

            // Assert
            Assert.AreEqual(4, contextMenu.Items.Count);

            var clearOutputItem = contextMenu.Items[contextMenuClearIndex];
            Assert.IsTrue(clearOutputItem.Enabled);
            Assert.AreEqual(RingtoetsFormsResources.Clear_all_output_ToolTip, clearOutputItem.ToolTipText);

            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void GetContextMenu_ClickOnAddCalculationItem_NewPipingCalculationInstanceAddedToFailureMechanismAndNotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.Attach(observerMock);
            var nodePresenter = new PipingFailureMechanismNodePresenter();

            // Precondition
            Assert.AreEqual(1, failureMechanism.Calculations.Count);

            // Call
            var contextMenu = nodePresenter.GetContextMenu(nodeMock, failureMechanism);
            var addCalculationItem = contextMenu.Items[contextMenuAddCalculationIndex];
            addCalculationItem.PerformClick();

            // Assert
            Assert.AreEqual(2, failureMechanism.Calculations.Count);
            IPipingCalculationItem addedItem = failureMechanism.Calculations.ElementAt(1);
            Assert.AreEqual("Nieuwe berekening (1)", addedItem.Name);
            Assert.IsInstanceOf<PipingCalculation>(addedItem);
            mocks.VerifyAll();
        }

        [Test]
        public void GetContextMenu_ClickOnAddFolderItem_NewPipingCalculationGroupInstanceAddedToFailureMechanismAndNotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.Attach(observerMock);
            failureMechanism.Calculations.Clear();
            failureMechanism.Calculations.Add(new PipingCalculationGroup());
            var nodePresenter = new PipingFailureMechanismNodePresenter();

            // Precondition
            Assert.AreEqual(1, failureMechanism.Calculations.Count);

            // Call
            var contextMenu = nodePresenter.GetContextMenu(nodeMock, failureMechanism);
            var addCalculationItem = contextMenu.Items[contextMenuAddFolderIndex];
            addCalculationItem.PerformClick();

            // Assert
            Assert.AreEqual(2, failureMechanism.Calculations.Count);
            IPipingCalculationItem addedItem = failureMechanism.Calculations.ElementAt(1);
            Assert.AreEqual("Nieuwe map (1)", addedItem.Name);
            Assert.IsInstanceOf<PipingCalculationGroup>(addedItem);
            mocks.VerifyAll();
        }

        [Test]
        public void OnPropertyChange_Always_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            var dataMock = mocks.StrictMock<PipingFailureMechanism>();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var eventArgsMock = mocks.StrictMock<PropertyChangedEventArgs>("");
            mocks.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter();

            // Call
            nodePresenter.OnPropertyChanged(dataMock, nodeMock, eventArgsMock);

            // Assert
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void OnCollectionChange_Always_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            var dataMock = mocks.StrictMock<PipingFailureMechanism>();
            var eventArgsMock = mocks.StrictMock<NotifyCollectionChangingEventArgs>();
            mocks.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter();

            // Call
            nodePresenter.OnCollectionChanged(dataMock, eventArgsMock);

            // Assert
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void CanRemove_Always_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var parentNodeDataMock = mocks.StrictMock<object>();
            var nodeMock = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter();

            // Call
            bool removalAllowed = nodePresenter.CanRemove(parentNodeDataMock, nodeMock);

            // Assert
            Assert.IsFalse(removalAllowed);
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void RemoveNodeData_PipingFailureMechanism_PipingFailureMechanismRemovedFromAssessmentSection()
        {
            // Setup
            var nodePresenter = new PipingFailureMechanismNodePresenter();

            // Call
            TestDelegate call = () => nodePresenter.RemoveNodeData(new object(), new PipingFailureMechanism());

            // Assert
            Assert.Throws<InvalidOperationException>(call);
        }

        [Test]
        public void GivenMultiplePipingCalculationsWithOutput_WhenClearingOutputFromContextMenu_ThenPipingOutputCleared()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Twice();

            var dataMock = mocks.StrictMock<PipingFailureMechanism>();
            dataMock.Calculations.Add(new PipingCalculation
            {
                Output = new TestPipingOutput()
            });
            dataMock.Calculations.Add(new PipingCalculation
            {
                Output = new TestPipingOutput()
            });
            dataMock.Calculations.ElementAt(0).Attach(observer);
            dataMock.Calculations.ElementAt(1).Attach(observer);

            var nodePresenter = new PipingFailureMechanismNodePresenter();
            var contextMenuAdapter = nodePresenter.GetContextMenu(null, dataMock);

            mocks.ReplayAll();

            // When
            contextMenuAdapter.Items[contextMenuClearIndex].PerformClick();

            // Then
            CollectionAssert.IsEmpty(dataMock.Calculations.Where(c => c.HasOutput));

            mocks.VerifyAll();
        }
    }
}