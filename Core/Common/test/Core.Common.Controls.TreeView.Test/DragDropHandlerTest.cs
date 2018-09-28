// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Drawing;
using System.Windows.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using WinFormsTreeView = System.Windows.Forms.TreeView;

namespace Core.Common.Controls.TreeView.Test
{
    [TestFixture]
    public class DragDropHandlerTest
    {
        [Test]
        public void HandleItemDrag_WithItemInTree_SelectsItem()
        {
            // Setup
            using (var treeView = new WinFormsTreeView())
            {
                var treeNode = new TreeNode();

                treeView.Nodes.Add(treeNode);
                treeView.SelectedNode = null;

                var ddh = new DragDropHandler();

                var dragEvent = new ItemDragEventArgs(MouseButtons.Left, treeNode);
                Func<object, TreeNodeInfo> action = o => new TreeNodeInfo();

                // Call
                ddh.HandleItemDrag(treeView, dragEvent, action);

                // Assert
                Assert.AreSame(treeNode, treeView.SelectedNode);
            }
        }

        [Test]
        public void HandleItemDrag_WithItemNotInTree_SelectsNull()
        {
            // Setup
            using (var treeView = new WinFormsTreeView())
            {
                var treeNode = new TreeNode();

                treeView.Nodes.Add(treeNode);
                treeView.SelectedNode = treeNode;

                var ddh = new DragDropHandler();

                var draggingNode = new TreeNode();
                var dragEvent = new ItemDragEventArgs(MouseButtons.Left, draggingNode);
                Func<object, TreeNodeInfo> action = o => new TreeNodeInfo();

                // Call
                ddh.HandleItemDrag(treeView, dragEvent, action);

                // Assert
                Assert.IsNull(treeView.SelectedNode);
            }
        }

        [Test]
        [TestCase(false, DragDropEffects.None)]
        [TestCase(true, DragDropEffects.Move)]
        public void HandleDragOver_TreeNodeDraggedToDroppableNode_DragDropEffectSetForEvent(bool canDrop, DragDropEffects dropEffect)
        {
            // Setup
            const int targetHeight = 30;

            var draggingNode = new TreeNode("DraggingNode");

            var mocks = new MockRepository();

            var data = mocks.Stub<IDataObject>();
            data.Expect(d => d.GetData(d.GetType())).IgnoreArguments().Return(draggingNode);

            var treeNode = mocks.Stub<TreeNode>();
            treeNode.Stub(tn => tn.Parent).Return(null);
            treeNode.Stub(tn => tn.Bounds).Return(new Rectangle(0, 0, 50, targetHeight));

            var nodePoint = new Point(0, 10);
            var graphics = mocks.Stub<Graphics>();

            var treeView = mocks.Stub<WinFormsTreeView>();
            treeView.Stub(tv => tv.PointToClient(Point.Empty)).IgnoreArguments().Return(nodePoint);
            treeView.Stub(tv => tv.GetNodeAt(nodePoint)).Return(treeNode);
            treeView.Stub(tv => tv.CreateGraphics()).Return(graphics);
            mocks.ReplayAll();

            var ddh = new DragDropHandler();

            var dragEvent = new DragEventArgs(data, 0, 10, 15, DragDropEffects.All, DragDropEffects.None);
            Func<object, TreeNodeInfo> action = o => new TreeNodeInfo
            {
                CanDrop = (oo, op) => canDrop
            };

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
            const int targetHeight = 30;

            var mocks = new MockRepository();

            var data = mocks.Stub<IDataObject>();
            data.Expect(d => d.GetData(d.GetType())).IgnoreArguments().Return(new object());

            var treeNode = mocks.Stub<TreeNode>();
            treeNode.Stub(tn => tn.Parent).Return(null);
            treeNode.Stub(tn => tn.Bounds).Return(new Rectangle(0, 0, 50, targetHeight));

            var nodePoint = new Point(0, 10);

            var graphics = mocks.Stub<Graphics>();

            var treeView = mocks.Stub<WinFormsTreeView>();
            treeView.Stub(tv => tv.PointToClient(Point.Empty)).IgnoreArguments().Return(nodePoint);
            treeView.Stub(tv => tv.GetNodeAt(nodePoint)).Return(treeNode);
            treeView.Stub(tv => tv.CreateGraphics()).Return(graphics);
            mocks.ReplayAll();

            var ddh = new DragDropHandler();

            var dragEvent = new DragEventArgs(data, 0, 10, 15, DragDropEffects.All, DragDropEffects.None);
            Func<object, TreeNodeInfo> action = o => new TreeNodeInfo();

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
            const int targetHeight = 30;

            var mocks = new MockRepository();
            var data = mocks.Stub<IDataObject>();
            data.Expect(d => d.GetData(d.GetType())).IgnoreArguments().Throw(new InvalidCastException());

            var treeNode = mocks.Stub<TreeNode>();
            treeNode.Stub(tn => tn.Parent).Return(null);
            treeNode.Stub(tn => tn.Bounds).Return(new Rectangle(0, 0, 50, targetHeight));

            var nodePoint = new Point(0, 10);

            var graphics = mocks.Stub<Graphics>();

            var treeView = mocks.Stub<WinFormsTreeView>();
            treeView.Stub(tv => tv.PointToClient(Point.Empty)).IgnoreArguments().Return(nodePoint);
            treeView.Stub(tv => tv.GetNodeAt(nodePoint)).Return(treeNode);
            treeView.Stub(tv => tv.CreateGraphics()).Return(graphics);
            mocks.ReplayAll();

            var ddh = new DragDropHandler();

            var dragEvent = new DragEventArgs(data, 0, 10, 15, DragDropEffects.All, DragDropEffects.None);
            Func<object, TreeNodeInfo> action = o => new TreeNodeInfo();

            // Call
            ddh.HandleDragOver(treeView, dragEvent, action);

            // Assert
            Assert.AreEqual(dragEvent.Effect, DragDropEffects.None);
            mocks.VerifyAll();
        }
    }
}