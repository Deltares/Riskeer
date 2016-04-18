using System;
using System.Windows.Forms;
using NUnit.Framework;

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
    }
}