using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DelftTools.Controls;
using DelftTools.Utils.Collections;
using NUnit.Framework;
using Rhino.Mocks;
using Wti.Data;
using Wti.Forms.NodePresenters;

using WtiFormsResources = Wti.Forms.Properties.Resources;

namespace Wti.Forms.Test.NodePresenters
{
    [TestFixture]
    public class PipingSurfaceLineCollectionNodePresenterTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter();

            // Assert
            Assert.IsInstanceOf<ITreeNodePresenter>(nodePresenter);
            Assert.IsNull(nodePresenter.TreeView);
            Assert.AreEqual(typeof(IEnumerable<RingtoetsPipingSurfaceLine>), nodePresenter.NodeTagType);
        }

        [Test]
        public void UpdateNode_WithData_InitializeNode()
        {
            // Setup
            var mocks = new MockRepository();
            var surfaceLinesCollectionNodeMock = mocks.Stub<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter();

            IEnumerable<RingtoetsPipingSurfaceLine> surfaceLinesCollection = new[]{ new RingtoetsPipingSurfaceLine() };

            // Call
            nodePresenter.UpdateNode(null, surfaceLinesCollectionNodeMock, surfaceLinesCollection);

            // Assert
            Assert.AreEqual(WtiFormsResources.PipingSurfaceLinesCollectionName, surfaceLinesCollectionNodeMock.Text);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), surfaceLinesCollectionNodeMock.ForegroundColor);
            Assert.AreEqual(16, surfaceLinesCollectionNodeMock.Image.Height);
            Assert.AreEqual(16, surfaceLinesCollectionNodeMock.Image.Width);
        }

        [Test]
        public void UpdateNode_CollectionIsEmpty_InitializeNodeWithGreyedOutText()
        {
            // Setup
            var mocks = new MockRepository();
            var surfaceLinesCollectionNodeMock = mocks.Stub<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter();

            IEnumerable<RingtoetsPipingSurfaceLine> surfaceLinesCollection = Enumerable.Empty<RingtoetsPipingSurfaceLine>();

            // Call
            nodePresenter.UpdateNode(null, surfaceLinesCollectionNodeMock, surfaceLinesCollection);

            // Assert
            Assert.AreEqual(WtiFormsResources.PipingSurfaceLinesCollectionName, surfaceLinesCollectionNodeMock.Text);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), surfaceLinesCollectionNodeMock.ForegroundColor);
            Assert.AreEqual(16, surfaceLinesCollectionNodeMock.Image.Height);
            Assert.AreEqual(16, surfaceLinesCollectionNodeMock.Image.Width);
        }

        [Test]
        public void GetChildNodeObjects_WithData_ReturnAllItemsInCollection()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter();

            IEnumerable<RingtoetsPipingSurfaceLine> surfaceLinesCollection = new[]
            {
                new RingtoetsPipingSurfaceLine(),
                new RingtoetsPipingSurfaceLine()
            };

            // Call
            var children = nodePresenter.GetChildNodeObjects(surfaceLinesCollection, nodeMock);

            // Assert
            CollectionAssert.AreEqual(surfaceLinesCollection, children);
            mocks.VerifyAll(); // Expect no calls on tree node
        }

        [Test]
        public void CanRenameNode_Always_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter();

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

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter();

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
            var nodeMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter();

            // Call
            TestDelegate call = () => { nodePresenter.OnNodeRenamed(nodeMock, "<Insert New Name Here>"); };

            // Assert
            var exception = Assert.Throws<InvalidOperationException>(call);
            var expectedMessage = string.Format("Cannot rename tree node of type {0}.", nodePresenter.GetType().Name);
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

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter();

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
            var dataMock = mocks.StrictMock<object>();
            mocks.ReplayAll();

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter();

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
            var dataMock = mocks.StrictMock<object>();
            var sourceMock = mocks.StrictMock<ITreeNode>();
            var targetMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter();

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
            var dataMock = mocks.StrictMock<object>();
            var sourceMock = mocks.StrictMock<ITreeNode>();
            var targetMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter();

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
            var dataMock = mocks.StrictMock<object>();
            var sourceParentNodeMock = mocks.StrictMock<ITreeNode>();
            var targetParentNodeDataMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter();

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
            var dataMock = mocks.StrictMock<object>();
            mocks.ReplayAll();

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter();

            // Call
            nodePresenter.OnNodeSelected(dataMock);

            // Assert
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void GetContextMenu_DefaultScenario_ReturnNull()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var dataMock = mocks.StrictMock<object>();
            mocks.ReplayAll();

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter
            {
                ImportSurfaceLinesAction = null
            };

            // Call
            var contextMenu = nodePresenter.GetContextMenu(nodeMock, dataMock);

            // Assert
            Assert.IsNull(contextMenu);
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void GetContextMenu_SurfaceLinesImportActionSet_HaveImportSurfaceLinesItemInContextMenu()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var dataMock = mocks.StrictMock<object>();
            var actionStub = mocks.Stub<Action>();
            mocks.ReplayAll();

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter
            {
                ImportSurfaceLinesAction = actionStub
            };

            // Call
            var returnedContextMenu = nodePresenter.GetContextMenu(nodeMock, dataMock);

            // Assert
            Assert.AreEqual(1, returnedContextMenu.Items.Count);
            var importItem = returnedContextMenu.Items[0];
            Assert.AreEqual("Importeer dwarsdoorsnedes", importItem.Text);
            Assert.AreEqual("Importeer nieuwe dwarsdoorsnedes van een *.csv bestand.", importItem.ToolTipText);
            Assert.AreEqual(16, importItem.Image.Width);
            Assert.AreEqual(16, importItem.Image.Height);
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void OnPropertyChange_Always_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            var dataMock = mocks.StrictMock<object>();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var eventArgsMock = mocks.StrictMock<PropertyChangedEventArgs>("");
            mocks.ReplayAll();

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter();

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
            var dataMock = mocks.StrictMock<object>();
            var eventArgsMock = mocks.StrictMock<NotifyCollectionChangingEventArgs>();
            mocks.ReplayAll();

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter();

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
            var dataMock = mocks.StrictMock<object>();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter();

            // Call
            bool removalAllowed = nodePresenter.CanRemove(dataMock, nodeMock);

            // Assert
            Assert.IsFalse(removalAllowed);
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void RemoveNodeData_Always_ThrowInvalidOperationException()
        {
            // setup
            var mocks = new MockRepository();
            var parentNodeDataMock = mocks.StrictMock<object>();
            var nodeDataMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter();

            // call
            TestDelegate call = () => nodePresenter.RemoveNodeData(parentNodeDataMock, nodeDataMock);

            // assert
            var exception = Assert.Throws<InvalidOperationException>(call);
            var expectedMessage = string.Format("Cannot delete node of type {0}.", nodePresenter.GetType().Name);
            Assert.AreEqual(expectedMessage, exception.Message);
            mocks.VerifyAll(); // Expect no calls on arguments
        }
    }
}