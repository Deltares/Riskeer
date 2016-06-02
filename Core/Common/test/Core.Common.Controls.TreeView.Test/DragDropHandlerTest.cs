using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Controls.TreeView.Test
{
    [TestFixture]
    public class DragDropHandlerTest
    {
        [Test]
        public void HandleItemDrag_WithItemInTree_SelectsItem()
        {
            // Setup
            DragDropHandler ddh = new DragDropHandler();
            Func<object, TreeNodeInfo> action = o => new TreeNodeInfo();
            var treeNode = new TreeNode();
            ItemDragEventArgs dragEvent = new ItemDragEventArgs(MouseButtons.Left, treeNode);
            System.Windows.Forms.TreeView treeView = new System.Windows.Forms.TreeView();

            treeView.Nodes.Add(treeNode);
            treeView.SelectedNode = null;

            // Call
            ddh.HandleItemDrag(treeView, dragEvent, action);

            // Assert
            Assert.AreSame(treeNode, treeView.SelectedNode);
        }

        [Test]
        public void HandleItemDrag_WithItemNotInTree_SelectsNull()
        {
            // Setup
            DragDropHandler ddh = new DragDropHandler();
            Func<object, TreeNodeInfo> action = o => new TreeNodeInfo();
            var draggingNode = new TreeNode();
            var treeNode = new TreeNode();
            ItemDragEventArgs dragEvent = new ItemDragEventArgs(MouseButtons.Left, draggingNode);
            System.Windows.Forms.TreeView treeView = new System.Windows.Forms.TreeView();

            treeView.Nodes.Add(treeNode);
            treeView.SelectedNode = treeNode;

            // Call
            ddh.HandleItemDrag(treeView, dragEvent, action);

            // Assert
            Assert.IsNull(treeView.SelectedNode);
        }

        [Test]
        [TestCase(false, DragDropEffects.None)]
        [TestCase(true, DragDropEffects.Move)]
        public void HandleDragOver_TreeNodeDraggedToDroppableNode_DragDropEffectSetForEvent(bool canDrop, DragDropEffects dropEffect)
        {
            // Setup
            int targetHeight = 30;

            var mocks = new MockRepository();
            var data = mocks.Stub<IDataObject>();
            var treeView = mocks.Stub<System.Windows.Forms.TreeView>();
            var treeNode = mocks.Stub<TreeNode>();
            var graphicsMock = mocks.Stub<Graphics>();
            treeNode.Stub(tn => tn.Parent).Return(null);
            treeNode.Stub(tn => tn.Bounds).Return(new Rectangle(0, 0, 50, targetHeight));

            var draggingNode = new TreeNode("DraggingNode");
            var nodePoint = new Point(0, 10);

            data.Expect(d => d.GetData(d.GetType())).IgnoreArguments().Return(draggingNode);
            treeView.Stub(tv => tv.PointToClient(Point.Empty)).IgnoreArguments().Return(nodePoint);
            treeView.Stub(tv => tv.GetNodeAt(nodePoint)).Return(treeNode);
            treeView.Stub(tv => tv.CreateGraphics()).Return(graphicsMock);
            mocks.ReplayAll();

            Func<object, TreeNodeInfo> action = o => new TreeNodeInfo
            {
                CanDrop = (oo,op) => canDrop
            };
            DragDropHandler ddh = new DragDropHandler();
            DragEventArgs dragEvent = new DragEventArgs(data, 0, 10, 15, DragDropEffects.All, DragDropEffects.None);

            // Call
            ddh.HandleDragOver(treeView, dragEvent, action);
            
            // Assert
            Assert.AreEqual(dragEvent.Effect, dropEffect);
            mocks.VerifyAll();
        }

        [Test]
        public void HandleDragOver_NoTreeNodeDraggedToDroppableNode_DragDropEffectNoneSetForEvent()
        {
            // Setup
            int targetHeight = 30;

            var mocks = new MockRepository();
            var data = mocks.Stub<IDataObject>();
            var treeView = mocks.Stub<System.Windows.Forms.TreeView>();
            var treeNode = mocks.Stub<TreeNode>();
            var graphicsMock = mocks.Stub<Graphics>();
            treeNode.Stub(tn => tn.Parent).Return(null);
            treeNode.Stub(tn => tn.Bounds).Return(new Rectangle(0, 0, 50, targetHeight));

            var nodePoint = new Point(0, 10);

            data.Expect(d => d.GetData(d.GetType())).IgnoreArguments().Return(new object());
            treeView.Stub(tv => tv.PointToClient(Point.Empty)).IgnoreArguments().Return(nodePoint);
            treeView.Stub(tv => tv.GetNodeAt(nodePoint)).Return(treeNode);
            treeView.Stub(tv => tv.CreateGraphics()).Return(graphicsMock);
            mocks.ReplayAll();

            Func<object, TreeNodeInfo> action = o => new TreeNodeInfo();
            DragDropHandler ddh = new DragDropHandler();
            DragEventArgs dragEvent = new DragEventArgs(data, 0, 10, 15, DragDropEffects.All, DragDropEffects.None);

            // Call
            ddh.HandleDragOver(treeView, dragEvent, action);
            
            // Assert
            Assert.AreEqual(dragEvent.Effect, DragDropEffects.None);
            mocks.VerifyAll();
        }

        [Test]
        public void HandleDragOver_DataDraggedThrowsInvalidCastException_DragDropEffectNoneSetForEvent()
        {
            // Setup
            int targetHeight = 30;

            var mocks = new MockRepository();
            var data = mocks.Stub<IDataObject>();
            var treeView = mocks.Stub<System.Windows.Forms.TreeView>();
            var treeNode = mocks.Stub<TreeNode>();
            var graphicsMock = mocks.Stub<Graphics>();
            treeNode.Stub(tn => tn.Parent).Return(null);
            treeNode.Stub(tn => tn.Bounds).Return(new Rectangle(0, 0, 50, targetHeight));

            var nodePoint = new Point(0, 10);

            data.Expect(d => d.GetData(d.GetType())).IgnoreArguments().Throw(new InvalidCastException());
            treeView.Stub(tv => tv.PointToClient(Point.Empty)).IgnoreArguments().Return(nodePoint);
            treeView.Stub(tv => tv.GetNodeAt(nodePoint)).Return(treeNode);
            treeView.Stub(tv => tv.CreateGraphics()).Return(graphicsMock);
            mocks.ReplayAll();

            Func<object, TreeNodeInfo> action = o => new TreeNodeInfo();
            DragDropHandler ddh = new DragDropHandler();
            DragEventArgs dragEvent = new DragEventArgs(data, 0, 10, 15, DragDropEffects.All, DragDropEffects.None);

            // Call
            ddh.HandleDragOver(treeView, dragEvent, action);
            
            // Assert
            Assert.AreEqual(dragEvent.Effect, DragDropEffects.None);
            mocks.VerifyAll();
        }
    }
}