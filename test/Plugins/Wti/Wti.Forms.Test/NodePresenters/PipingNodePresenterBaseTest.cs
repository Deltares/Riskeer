using System;
using System.ComponentModel;
using System.Linq;

using DelftTools.Controls;
using DelftTools.Utils.Collections;

using NUnit.Framework;

using Rhino.Mocks;

using Wti.Forms.NodePresenters;

namespace Wti.Forms.Test.NodePresenters
{
    [TestFixture]
    public class PipingNodePresenterBaseTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var nodePresenter = new SimplePipingNodePresenterBase<double>();

            // Assert
            Assert.AreEqual(typeof(double), nodePresenter.NodeTagType);
            Assert.IsNull(nodePresenter.TreeView);
        }

        [Test]
        public void UpdateNode_WithData_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            var parentNode = mocks.StrictMock<ITreeNode>();
            var pipingNode = mocks.StrictMock<ITreeNode>();
            var pipingData = mocks.StrictMock<SomePipingClass>();
            mocks.ReplayAll();

            var nodePresenter = new SimplePipingNodePresenterBase<SomePipingClass>();

            // Call
            nodePresenter.UpdateNode(parentNode, pipingNode, pipingData);

            // Assert
            mocks.VerifyAll(); // Expect no calls on mocks
        }

        [Test]
        public void GetChildNodeObjects_Always_ReturnNoChildData()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var dataMock = mocks.StrictMock<SomePipingClass>();
            mocks.ReplayAll();

            var nodePresenter = new SimplePipingNodePresenterBase<SomePipingClass>();

            // Call
            var children = nodePresenter.GetChildNodeObjects(dataMock, nodeMock).OfType<object>().ToArray();

            // Assert
            CollectionAssert.IsEmpty(children);
            mocks.VerifyAll(); // Expect no calls on tree node
        }

        [Test]
        public void CanRenameNode_Always_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new SimplePipingNodePresenterBase<SomePipingClass>();

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

            var nodePresenter = new SimplePipingNodePresenterBase<SomePipingClass>();

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
            var dataMock = mocks.StrictMock<SomePipingClass>();
            mocks.ReplayAll();

            var nodePresenter = new SimplePipingNodePresenterBase<SomePipingClass>();

            // Call
            TestDelegate call = () => { nodePresenter.OnNodeRenamed(dataMock, "<Insert New Name Here>"); };

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

            var nodePresenter = new SimplePipingNodePresenterBase<SomePipingClass>();

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
            var dataMock = mocks.StrictMock<SomePipingClass>();
            mocks.ReplayAll();

            var nodePresenter = new SimplePipingNodePresenterBase<SomePipingClass>();

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
            var dataMock = mocks.StrictMock<SomePipingClass>();
            var sourceMock = mocks.StrictMock<ITreeNode>();
            var targetMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new SimplePipingNodePresenterBase<SomePipingClass>();

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
            var dataMock = mocks.StrictMock<SomePipingClass>();
            var sourceMock = mocks.StrictMock<ITreeNode>();
            var targetMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new SimplePipingNodePresenterBase<SomePipingClass>();

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
            var dataMock = mocks.StrictMock<SomePipingClass>();
            var sourceParentNodeMock = mocks.StrictMock<ITreeNode>();
            var targetParentNodeDataMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new SimplePipingNodePresenterBase<SomePipingClass>();

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
            var dataMock = mocks.StrictMock<SomePipingClass>();
            mocks.ReplayAll();

            var nodePresenter = new SimplePipingNodePresenterBase<SomePipingClass>();

            // Call
            nodePresenter.OnNodeSelected(dataMock);

            // Assert
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void GetContextMenu_Always_ReturnsNull()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var dataMock = mocks.StrictMock<SomePipingClass>();

            var nodePresenter = new SimplePipingNodePresenterBase<SomePipingClass>();

            mocks.ReplayAll();

            // Call
            var contextMenu = nodePresenter.GetContextMenu(nodeMock, dataMock);

            // Assert
            Assert.IsNull(contextMenu);
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void OnPropertyChange_Always_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            var dataMock = mocks.StrictMock<SomePipingClass>();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var eventArgsMock = mocks.StrictMock<PropertyChangedEventArgs>("");
            mocks.ReplayAll();

            var nodePresenter = new SimplePipingNodePresenterBase<SomePipingClass>();

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
            var dataMock = mocks.StrictMock<SomePipingClass>();
            var eventArgsMock = mocks.StrictMock<NotifyCollectionChangingEventArgs>();
            mocks.ReplayAll();

            var nodePresenter = new SimplePipingNodePresenterBase<SomePipingClass>();

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
            var dataMock = mocks.StrictMock<SomePipingClass>();
            mocks.ReplayAll();

            var nodePresenter = new SimplePipingNodePresenterBase<SomePipingClass>();

            // Call
            bool removalAllowed = nodePresenter.CanRemove(null, dataMock);

            // Assert
            Assert.IsFalse(removalAllowed);
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void RemoveNodeData_Always_ThrowInvalidOperationException()
        {
            // Setup
            var nodePresenter = new SimplePipingNodePresenterBase<SomePipingClass>();

            // Call
            TestDelegate call = () => nodePresenter.RemoveNodeData(null, new SomePipingClass());

            // Assert
            var exception = Assert.Throws<InvalidOperationException>(call);
            Assert.AreEqual(String.Format("Cannot delete node of type {0}.", nodePresenter.GetType().Name), exception.Message);
        }

        private class SimplePipingNodePresenterBase<T> : PipingNodePresenterBase<T>
        {
            protected override void UpdateNode(ITreeNode parentNode, ITreeNode node, T nodeData)
            {
                
            }
        }

        public class SomePipingClass{}
    }
}