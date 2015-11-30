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

namespace Ringtoets.Piping.Forms.Test.NodePresenters
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

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter();

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

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter();

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
            var dataMock = mocks.StrictMock<IEnumerable<RingtoetsPipingSurfaceLine>>();
            mocks.ReplayAll();

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter();

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
            var dataMock = mocks.StrictMock<IEnumerable<RingtoetsPipingSurfaceLine>>();
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
            var dataMock = mocks.StrictMock<IEnumerable<RingtoetsPipingSurfaceLine>>();
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
            var dataMock = mocks.StrictMock<IEnumerable<RingtoetsPipingSurfaceLine>>();
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
            var dataMock = mocks.StrictMock<IEnumerable<RingtoetsPipingSurfaceLine>>();
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
            var dataMock = mocks.StrictMock<IEnumerable<RingtoetsPipingSurfaceLine>>();
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
            var dataMock = mocks.StrictMock<IEnumerable<RingtoetsPipingSurfaceLine>>();
            mocks.ReplayAll();

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter();

            // Call
            var contextMenu = nodePresenter.GetContextMenu(nodeMock, dataMock);

            // Assert
            Assert.IsNull(contextMenu);
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void GetContextMenu_NoContextMenuBuilderProviderSet_ReturnsNull()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var dataMock = mocks.StrictMock<IEnumerable<RingtoetsPipingSurfaceLine>>();
            mocks.ReplayAll();

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter();

            // Call
            var returnedContextMenu = nodePresenter.GetContextMenu(nodeMock, dataMock);

            // Assert
            Assert.IsNull(returnedContextMenu);

            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GetContextMenu_ContextMenuBuilderProviderSet_HaveImportSurfaceLinesItemInContextMenu(bool importExportEnabled)
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var dataMock = mocks.StrictMock<IEnumerable<RingtoetsPipingSurfaceLine>>();

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter
            {
                ContextMenuBuilderProvider = TestContextMenuBuilderProvider.Create(mocks, nodeMock, importExportEnabled)
            };

            mocks.ReplayAll();

            // Call
            var returnedContextMenu = nodePresenter.GetContextMenu(nodeMock, dataMock);

            // Assert
            Assert.AreEqual(5, returnedContextMenu.Items.Count);
            var expandAllItem = returnedContextMenu.Items[0];
            Assert.AreEqual(Core.Common.Gui.Properties.Resources.Expand_all, expandAllItem.Text);
            Assert.AreEqual(Core.Common.Gui.Properties.Resources.Expand_all_ToolTip, expandAllItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(Core.Common.Gui.Properties.Resources.ExpandAllIcon, expandAllItem.Image);
            Assert.IsTrue(expandAllItem.Enabled);

            var collapseAllItem = returnedContextMenu.Items[1];
            Assert.AreEqual(Core.Common.Gui.Properties.Resources.Collapse_all, collapseAllItem.Text);
            Assert.AreEqual(Core.Common.Gui.Properties.Resources.Collapse_all_ToolTip, collapseAllItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(Core.Common.Gui.Properties.Resources.CollapseAllIcon, collapseAllItem.Image);
            Assert.IsTrue(collapseAllItem.Enabled);

            var importItem = returnedContextMenu.Items[3];
            Assert.AreEqual(Core.Common.Gui.Properties.Resources.Import, importItem.Text);
            Assert.AreEqual(Core.Common.Gui.Properties.Resources.Import_ToolTip, importItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(Core.Common.Gui.Properties.Resources.ImportIcon, importItem.Image);
            Assert.AreEqual(importExportEnabled, importItem.Enabled);

            var exportItem = returnedContextMenu.Items[4];
            Assert.AreEqual(Core.Common.Gui.Properties.Resources.Export, exportItem.Text);
            Assert.AreEqual(Core.Common.Gui.Properties.Resources.Export_ToolTip, exportItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(Core.Common.Gui.Properties.Resources.ExportIcon, exportItem.Image);
            Assert.AreEqual(importExportEnabled, exportItem.Enabled);

            Assert.IsInstanceOf<ToolStripSeparator>(returnedContextMenu.Items[2]);

            mocks.VerifyAll();
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
            var dataMock = mocks.StrictMock<IEnumerable<RingtoetsPipingSurfaceLine>>();
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
            var dataMock = mocks.StrictMock<IEnumerable<RingtoetsPipingSurfaceLine>>();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter();

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

            var nodePresenter = new PipingSurfaceLineCollectionNodePresenter();

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