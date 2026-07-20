// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Drawing;
using System.Windows.Forms;
using NSubstitute;
using NUnit.Framework;
using WinFormsTreeView = System.Windows.Forms.TreeView;

namespace Core.Common.Controls.TreeView.Test
{
    [TestFixture]
    public class DragDropHandlerTest
    {
        [Test]
        public void HandleItemDrag_WithItemInTree_SelectsItem()
        {
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
            var data = Substitute.For<IDataObject>();
            using (var treeView = CreateTreeViewWithTopLevelNodes(out TreeNode draggingNode, out TreeNode treeNode))
            {
                data.GetData(typeof(TreeNode)).Returns(draggingNode);

                var ddh = new DragDropHandler();
                Point originalCursorPosition = Cursor.Position;

                try
                {
                    Point nodePoint = GetNodeMiddlePoint(treeNode);
                    Point screenPoint = treeView.PointToScreen(nodePoint);
                    Cursor.Position = screenPoint;

                    var dragEvent = new DragEventArgs(data, 0, screenPoint.X, screenPoint.Y, DragDropEffects.All, DragDropEffects.None);
                    Func<object, TreeNodeInfo> action = o => new TreeNodeInfo
                    {
                        CanDrop = (oo, op) => canDrop
                    };

                    // Call
                    ddh.HandleDragOver(treeView, dragEvent, action);

                    // Assert
                    Assert.AreEqual(dropEffect, dragEvent.Effect);
                }
                finally
                {
                    ddh.HandleDragLeave(treeView);
                    Cursor.Position = originalCursorPosition;
                }
            }
        }

        [Test]
        public void HandleDragOver_NoTreeNodeDraggedToDroppableNode_DragDropEffectNoneSetForEvent()
        {
            // Setup
            var data = Substitute.For<IDataObject>();
            data.GetData(Arg.Any<Type>()).Returns(new object());

            using (var treeView = CreateTreeViewWithTopLevelNodes(out _, out TreeNode treeNode))
            {
                Point nodePoint = GetNodeMiddlePoint(treeNode);
                Point screenPoint = treeView.PointToScreen(nodePoint);

                var ddh = new DragDropHandler();
                var dragEvent = new DragEventArgs(data, 0, screenPoint.X, screenPoint.Y, DragDropEffects.All, DragDropEffects.None);
                Func<object, TreeNodeInfo> action = o => new TreeNodeInfo();

                // Call
                ddh.HandleDragOver(treeView, dragEvent, action);

                // Assert
                Assert.AreEqual(DragDropEffects.None, dragEvent.Effect);
            }
        }

        [Test]
        public void HandleDragOver_DataDraggedThrowsInvalidCastException_DragDropEffectNoneSetForEvent()
        {
            // Setup
            var data = Substitute.For<IDataObject>();
            data.GetData(Arg.Any<Type>()).Returns(_ => throw new InvalidCastException());

            using (var treeView = CreateTreeViewWithTopLevelNodes(out _, out TreeNode treeNode))
            {
                Point nodePoint = GetNodeMiddlePoint(treeNode);
                Point screenPoint = treeView.PointToScreen(nodePoint);

                var ddh = new DragDropHandler();
                var dragEvent = new DragEventArgs(data, 0, screenPoint.X, screenPoint.Y, DragDropEffects.All, DragDropEffects.None);
                Func<object, TreeNodeInfo> action = o => new TreeNodeInfo();

                // Call
                ddh.HandleDragOver(treeView, dragEvent, action);

                // Assert
                Assert.AreEqual(DragDropEffects.None, dragEvent.Effect);
            }
        }

        private static WinFormsTreeView CreateTreeViewWithTopLevelNodes(out TreeNode draggingNode, out TreeNode targetNode)
        {
            var treeView = new WinFormsTreeView
            {
                Size = new Size(200, 200)
            };

            draggingNode = new TreeNode("DraggingNode");
            targetNode = new TreeNode("TargetNode");

            treeView.Nodes.Add(draggingNode);
            treeView.Nodes.Add(targetNode);
            treeView.CreateControl();

            return treeView;
        }

        private static Point GetNodeMiddlePoint(TreeNode treeNode)
        {
            Rectangle bounds = treeNode.Bounds;
            return new Point(bounds.Left + 1, bounds.Top + bounds.Height / 2);
        }
    }
}