using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.TestUtils;
using Core.Common.TestUtils;
using Core.Common.Utils.Collections;
using NUnit.Framework;
using Rhino.Mocks;

using Ringtoets.Piping.Data;

using Ringtoets.Piping.Forms.NodePresenters;

using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.Piping.Forms.Test.NodePresenters
{
    [TestFixture]
    public class PipingSurfaceLineCollectionNodePresenterTest
    {
        private MockRepository mockRepository;
        private IContextMenuBuilderProvider contextMenuBuilderProviderMock;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
            contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
        }

        [Test]
        public void Constructor_NoMenuBuilderProvider_ArgumentNullException()
        {
            // Call
            TestDelegate test = () => new PipingSurfaceLineCollectionNodePresenter(null);

            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(CoreCommonGuiResources.NodePresenter_ContextMenuBuilderProvider_required, message);
            StringAssert.EndsWith("contextMenuBuilderProvider", message);
        }

        [Test]
        public void Constructor_WithParamsSet_ExpectedValues()
        {
            // Call
            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter(contextMenuBuilderProviderMock);

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

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter(contextMenuBuilderProviderMock);

            IEnumerable<RingtoetsPipingSurfaceLine> surfaceLinesCollection = new[]{ new RingtoetsPipingSurfaceLine() };

            // Call
            nodePresenter.UpdateNode(null, surfaceLinesCollectionNodeMock, surfaceLinesCollection);

            // Assert
            Assert.AreEqual(PipingFormsResources.PipingSurfaceLinesCollection_DisplayName, surfaceLinesCollectionNodeMock.Text);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), surfaceLinesCollectionNodeMock.ForegroundColor);
            TestHelper.AssertImagesAreEqual(PipingFormsResources.FolderIcon, surfaceLinesCollectionNodeMock.Image);
        }

        [Test]
        public void UpdateNode_CollectionIsEmpty_InitializeNodeWithGreyedOutText()
        {
            // Setup
            var mocks = new MockRepository();
            var surfaceLinesCollectionNodeMock = mocks.Stub<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter(contextMenuBuilderProviderMock);

            IEnumerable<RingtoetsPipingSurfaceLine> surfaceLinesCollection = Enumerable.Empty<RingtoetsPipingSurfaceLine>();

            // Call
            nodePresenter.UpdateNode(null, surfaceLinesCollectionNodeMock, surfaceLinesCollection);

            // Assert
            Assert.AreEqual(PipingFormsResources.PipingSurfaceLinesCollection_DisplayName, surfaceLinesCollectionNodeMock.Text);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), surfaceLinesCollectionNodeMock.ForegroundColor);
            TestHelper.AssertImagesAreEqual(PipingFormsResources.FolderIcon, surfaceLinesCollectionNodeMock.Image);
        }

        [Test]
        public void GetChildNodeObjects_WithData_ReturnAllItemsInCollection()
        {
            // Setup
            var mocks = new MockRepository();
            mocks.ReplayAll();

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter(contextMenuBuilderProviderMock);

            IEnumerable<RingtoetsPipingSurfaceLine> surfaceLinesCollection = new[]
            {
                new RingtoetsPipingSurfaceLine(),
                new RingtoetsPipingSurfaceLine()
            };

            // Call
            var children = nodePresenter.GetChildNodeObjects(surfaceLinesCollection);

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

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter(contextMenuBuilderProviderMock);

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

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter(contextMenuBuilderProviderMock);

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
            var dataMock = mocks.StrictMock<IEnumerable<RingtoetsPipingSurfaceLine>>();
            mocks.ReplayAll();

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter(contextMenuBuilderProviderMock);

            // Call
            TestDelegate call = () => { nodePresenter.OnNodeRenamed(dataMock, "<Insert New Name Here>"); };

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

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter(contextMenuBuilderProviderMock);

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
            var dataMock = mocks.StrictMock<IEnumerable<RingtoetsPipingSurfaceLine>>();
            mocks.ReplayAll();

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter(contextMenuBuilderProviderMock);

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
            var dataMock = mocks.StrictMock<IEnumerable<RingtoetsPipingSurfaceLine>>();
            var sourceMock = mocks.StrictMock<ITreeNode>();
            var targetMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter(contextMenuBuilderProviderMock);

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
            var dataMock = mocks.StrictMock<IEnumerable<RingtoetsPipingSurfaceLine>>();
            var sourceMock = mocks.StrictMock<ITreeNode>();
            var targetMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter(contextMenuBuilderProviderMock);

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
            var dataMock = mocks.StrictMock<IEnumerable<RingtoetsPipingSurfaceLine>>();
            var sourceParentNodeMock = mocks.StrictMock<ITreeNode>();
            var targetParentNodeDataMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter(contextMenuBuilderProviderMock);

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
            var dataMock = mocks.StrictMock<IEnumerable<RingtoetsPipingSurfaceLine>>();
            mocks.ReplayAll();

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter(contextMenuBuilderProviderMock);

            // Call
            nodePresenter.OnNodeSelected(dataMock);

            // Assert
            mocks.VerifyAll(); // Expect no calls on arguments
        }


        [Test]
        public void GetContextMenu_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var nodeMock = mockRepository.StrictMock<ITreeNode>();
            var dataMock = mockRepository.StrictMock<IEnumerable<RingtoetsPipingSurfaceLine>>();
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            var menuBuilderMock = mockRepository.StrictMock<IContextMenuBuilder>();

            menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddImportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            contextMenuBuilderProviderMock.Expect(cmp => cmp.Get(nodeMock)).Return(menuBuilderMock);
            mockRepository.ReplayAll();

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter(contextMenuBuilderProviderMock);

            // Call
            nodePresenter.GetContextMenu(nodeMock, dataMock);

            // Assert
            mockRepository.VerifyAll();
        }

        [Test]
        public void OnPropertyChange_Always_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            var dataMock = mocks.StrictMock<IEnumerable<RingtoetsPipingSurfaceLine>>();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var eventArgsMock = mocks.StrictMock<PropertyChangedEventArgs>("");
            mocks.ReplayAll();

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter(contextMenuBuilderProviderMock);

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
            var dataMock = mocks.StrictMock<IEnumerable<RingtoetsPipingSurfaceLine>>();
            var eventArgsMock = mocks.StrictMock<NotifyCollectionChangingEventArgs>();
            mocks.ReplayAll();

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter(contextMenuBuilderProviderMock);

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
            var dataMock = mocks.StrictMock<IEnumerable<RingtoetsPipingSurfaceLine>>();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter(contextMenuBuilderProviderMock);

            // Call
            bool removalAllowed = nodePresenter.CanRemove(nodeMock, dataMock);

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
            var dataMock = mocks.StrictMock<IEnumerable<RingtoetsPipingSurfaceLine>>();
            mocks.ReplayAll();

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter(contextMenuBuilderProviderMock);

            // call
            TestDelegate call = () => nodePresenter.RemoveNodeData(parentNodeDataMock, dataMock);

            // assert
            var exception = Assert.Throws<InvalidOperationException>(call);
            var expectedMessage = string.Format("Kan knoop uit boom van type {0} niet verwijderen.", nodePresenter.GetType().Name);
            Assert.AreEqual(expectedMessage, exception.Message);
            mocks.VerifyAll(); // Expect no calls on arguments
        }
    }
}