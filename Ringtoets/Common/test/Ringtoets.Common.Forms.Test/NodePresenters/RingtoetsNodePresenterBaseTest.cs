//using System;
//using System.Linq;
//
//using Core.Common.Controls.TreeView;
//using Core.Common.Gui;
//using Core.Common.Gui.Properties;
//using NUnit.Framework;
//
//using Rhino.Mocks;
//
//using Ringtoets.Common.Forms.NodePresenters;
//
//namespace Ringtoets.Common.Forms.Test.NodePresenters
//{
//    [TestFixture]
//    public class RingtoetsNodePresenterBaseTest
//    {
//        private MockRepository mockRepository;
//
//        [SetUp]
//        public void SetUp()
//        {
//            mockRepository = new MockRepository();
//        }
//
//        [Test]
//        public void Constructor_NoMenuBuilderProvider_ArgumentNullException()
//        {
//            // Call
//            TestDelegate test = () => new SimpleRingtoetsNodePresenterBase<double>(null);
//
//            // Assert
//            var message = Assert.Throws<ArgumentNullException>(test).Message;
//            StringAssert.StartsWith(Resources.NodePresenter_ContextMenuBuilderProvider_required, message);
//            StringAssert.EndsWith("contextMenuBuilderProvider", message);
//        }
//
//        [Test]
//        public void Constructor_ParamsSet_ExpectedValues()
//        {
//            // Setup
//            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
//
//            mockRepository.ReplayAll();
//
//            // Call
//            var nodePresenter = new SimpleRingtoetsNodePresenterBase<double>(contextMenuBuilderProviderMock);
//
//            // Assert
//            Assert.AreEqual(typeof(double), nodePresenter.NodeTagType);
//            Assert.IsNull(nodePresenter.TreeView);
//
//            mockRepository.VerifyAll();
//        }
//
//        [Test]
//        public void UpdateNode_WithData_DoNothing()
//        {
//            // Setup
//            var parentNode = mockRepository.StrictMock<TreeNode>();
//            var pipingNode = mockRepository.StrictMock<TreeNode>();
//            var nodeData = mockRepository.StrictMock<object>();
//            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
//            mockRepository.ReplayAll();
//
//            var nodePresenter = new SimpleRingtoetsNodePresenterBase<object>(contextMenuBuilderProviderMock);
//
//            // Call
//            nodePresenter.UpdateNode(parentNode, pipingNode, nodeData);
//
//            // Assert
//            mockRepository.VerifyAll(); // Expect no calls on mocks
//        }
//
//        [Test]
//        public void GetChildNodeObjects_Always_ReturnNoChildData()
//        {
//            // Setup
//            var dataMock = mockRepository.StrictMock<object>();
//            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
//            mockRepository.ReplayAll();
//
//            var nodePresenter = new SimpleRingtoetsNodePresenterBase<object>(contextMenuBuilderProviderMock);
//
//            // Call
//            var children = nodePresenter.GetChildNodeObjects(dataMock).OfType<object>().ToArray();
//
//            // Assert
//            CollectionAssert.IsEmpty(children);
//            mockRepository.VerifyAll(); // Expect no calls on tree node
//        }
//
//        [Test]
//        public void CanRenameNode_Always_ReturnFalse()
//        {
//            // Setup
//            var nodeMock = mockRepository.StrictMock<TreeNode>();
//            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
//            mockRepository.ReplayAll();
//
//            var nodePresenter = new SimpleRingtoetsNodePresenterBase<object>(contextMenuBuilderProviderMock);
//
//            // Call
//            var renameAllowed = nodePresenter.CanRenameNode(nodeMock);
//
//            // Assert
//            Assert.IsFalse(renameAllowed);
//            mockRepository.VerifyAll(); // Expect no calls on tree node
//        }
//
//        [Test]
//        public void CanRenameNodeTo_Always_ReturnFalse()
//        {
//            // Setup
//            var nodeMock = mockRepository.StrictMock<TreeNode>();
//            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
//            mockRepository.ReplayAll();
//
//            var nodePresenter = new SimpleRingtoetsNodePresenterBase<object>(contextMenuBuilderProviderMock);
//
//            // Call
//            var renameAllowed = nodePresenter.CanRenameNodeTo(nodeMock, "<Insert New Name Here>");
//
//            // Assert
//            Assert.IsFalse(renameAllowed);
//            mockRepository.ReplayAll(); // Expect no calls on tree node
//        }
//
//        [Test]
//        public void OnNodeRenamed_Always_ThrowInvalidOperationException()
//        {
//            // Setup
//            var dataMock = mockRepository.StrictMock<object>();
//            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
//            mockRepository.ReplayAll();
//
//            var nodePresenter = new SimpleRingtoetsNodePresenterBase<object>(contextMenuBuilderProviderMock);
//
//            // Call
//            TestDelegate call = () => { nodePresenter.OnNodeRenamed(dataMock, "<Insert New Name Here>"); };
//
//            // Assert
//            var exception = Assert.Throws<InvalidOperationException>(call);
//            var expectedMessage = string.Format("Kan knoop uit boom van type {0} niet hernoemen.", nodePresenter.GetType().Name);
//            Assert.AreEqual(expectedMessage, exception.Message);
//            mockRepository.ReplayAll(); // Expect no calls on tree node
//        }
//
//        [Test]
//        public void OnNodeChecked_Always_DoNothing()
//        {
//            // Setup
//            var nodeMock = mockRepository.StrictMock<TreeNode>();
//            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
//            mockRepository.ReplayAll();
//
//            var nodePresenter = new SimpleRingtoetsNodePresenterBase<object>(contextMenuBuilderProviderMock);
//
//            // Call
//            nodePresenter.OnNodeChecked(nodeMock);
//
//            // Assert
//            mockRepository.VerifyAll(); // Expect no calls on tree node
//        }
//
//        [Test]
//        public void CanDrag_Always_ReturnNone()
//        {
//            // Setup
//            var dataMock = mockRepository.StrictMock<object>();
//            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
//            mockRepository.ReplayAll();
//
//            var nodePresenter = new SimpleRingtoetsNodePresenterBase<object>(contextMenuBuilderProviderMock);
//
//            // Call
//            DragOperations dragAllowed = nodePresenter.CanDrag(dataMock);
//
//            // Assert
//            Assert.AreEqual(DragOperations.None, dragAllowed);
//            mockRepository.VerifyAll();
//        }
//
//        [Test]
//        public void CanDrop_Always_ReturnNone()
//        {
//            // Setup
//            var dataMock = mockRepository.StrictMock<object>();
//            var sourceMock = mockRepository.StrictMock<TreeNode>();
//            var targetMock = mockRepository.StrictMock<TreeNode>();
//            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
//            mockRepository.ReplayAll();
//
//            var nodePresenter = new SimpleRingtoetsNodePresenterBase<object>(contextMenuBuilderProviderMock);
//
//            // Call
//            DragOperations dropAllowed = nodePresenter.CanDrop(dataMock, sourceMock, targetMock, DragOperations.Move);
//
//            // Assert
//            Assert.AreEqual(DragOperations.None, dropAllowed);
//            mockRepository.VerifyAll(); // Expect no calls on mockRepository.
//        }
//
//        [Test]
//        public void CanInsert_Always_ReturnFalse()
//        {
//            // Setup
//            var dataMock = mockRepository.StrictMock<object>();
//            var sourceMock = mockRepository.StrictMock<TreeNode>();
//            var targetMock = mockRepository.StrictMock<TreeNode>();
//            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
//            mockRepository.ReplayAll();
//
//            var nodePresenter = new SimpleRingtoetsNodePresenterBase<object>(contextMenuBuilderProviderMock);
//
//            // Call
//            bool insertionAllowed = nodePresenter.CanInsert(dataMock, sourceMock, targetMock);
//
//            // Assert
//            Assert.IsFalse(insertionAllowed);
//            mockRepository.VerifyAll(); // Expect no calls on arguments
//        }
//
//        [Test]
//        public void OnDragDrop_Always_DoNothing()
//        {
//            // Setup
//            var dataMock = mockRepository.StrictMock<object>();
//            var sourceParentNodeMock = mockRepository.StrictMock<TreeNode>();
//            var targetParentNodeDataMock = mockRepository.StrictMock<TreeNode>();
//            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
//            mockRepository.ReplayAll();
//
//            var nodePresenter = new SimpleRingtoetsNodePresenterBase<object>(contextMenuBuilderProviderMock);
//
//            // Call
//            nodePresenter.OnDragDrop(dataMock, sourceParentNodeMock, targetParentNodeDataMock, DragOperations.Move, 2);
//
//            // Assert
//            mockRepository.VerifyAll(); // Expect no calls on arguments
//        }
//
//        [Test]
//        public void GetContextMenu_Always_ReturnsNull()
//        {
//            // Setup
//            var nodeMock = mockRepository.StrictMock<TreeNode>();
//            var dataMock = mockRepository.StrictMock<object>();
//            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
//
//            mockRepository.ReplayAll();
//
//            var nodePresenter = new SimpleRingtoetsNodePresenterBase<object>(contextMenuBuilderProviderMock);
//
//            // Call
//            var contextMenu = nodePresenter.GetContextMenu(nodeMock, dataMock);
//
//            // Assert
//            Assert.IsNull(contextMenu);
//            mockRepository.VerifyAll(); // Expect no calls on arguments
//        }
//
//        [Test]
//        public void CanRemove_Always_ReturnFalse()
//        {
//            // Setup
//            var dataMock = mockRepository.StrictMock<object>();
//            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
//
//            mockRepository.ReplayAll();
//
//            var nodePresenter = new SimpleRingtoetsNodePresenterBase<object>(contextMenuBuilderProviderMock);
//
//            // Call
//            bool removalAllowed = nodePresenter.CanRemove(null, dataMock);
//
//            // Assert
//            Assert.IsFalse(removalAllowed);
//            mockRepository.VerifyAll(); // Expect no calls on arguments
//        }
//
//        [Test]
//        public void RemoveNodeData_Always_ThrowInvalidOperationException()
//        {
//            // Setup
//            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
//
//            mockRepository.ReplayAll();
//
//            var nodePresenter = new SimpleRingtoetsNodePresenterBase<object>(contextMenuBuilderProviderMock);
//
//            // Call
//            TestDelegate call = () => nodePresenter.RemoveNodeData(null, new object());
//
//            // Assert
//            var exception = Assert.Throws<InvalidOperationException>(call);
//            Assert.AreEqual(String.Format("Kan knoop uit boom van type {0} niet verwijderen.", nodePresenter.GetType().Name), exception.Message);
//
//            mockRepository.VerifyAll();
//        }
//
//        private class SimpleRingtoetsNodePresenterBase<T> : RingtoetsNodePresenterBase<T>
//        {
//            protected override void UpdateNode(TreeNode parentNode, TreeNode node, T nodeData) {}
//            public SimpleRingtoetsNodePresenterBase(IContextMenuBuilderProvider contextMenuBuilderProvider) : base(contextMenuBuilderProvider) {}
//        }
//    }
//}