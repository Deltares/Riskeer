// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using FormsTreeView = System.Windows.Forms.TreeView;

namespace Core.Common.Controls.TreeView
{
    /// <summary>
    /// This class handles the drag and drop related logic for a <see cref="TreeViewControl"/>.
    /// </summary>
    public class DragDropHandler
    {
        private int dropAtLocation;
        private Point lastDragOverPoint;
        private PlaceholderLocation lastPlaceholderLocation;
        private TreeNode nodeDropTarget;
        private TreeNode lastPlaceholderNode;
        private Graphics placeHolderGraphics;

        /// <summary>
        /// This method handles the <see cref="TreeView.DragDrop"/> event for a <see cref="TreeViewControl"/>.
        /// </summary>
        /// <param name="treeViewControl">The <see cref="TreeViewControl"/> to handle the <see cref="TreeView.DragDrop"/> event for.</param>
        /// <param name="treeView">The <see cref="TreeView"/> of the <see cref="TreeViewControl"/>.</param>
        /// <param name="e">The arguments of the <see cref="TreeView.DragDrop"/> event.</param>
        /// <param name="getTreeNodeInfoForData">A function for obtaining the <see cref="TreeNodeInfo"/> object corresponding to a provided data object.</param>
        public void HandleDragDrop(TreeViewControl treeViewControl, FormsTreeView treeView, DragEventArgs e, Func<object, TreeNodeInfo> getTreeNodeInfoForData)
        {
            ClearPlaceHolders(treeView);

            var point = treeView.PointToClient(new Point(e.X, e.Y));
            var nodeOver = treeView.GetNodeAt(point);
            var draggedNode = (TreeNode) e.Data.GetData(typeof(TreeNode));

            if (nodeOver == null)
            {
                return;
            }

            // Handle dragged items which were originally higher up in the tree under the same parent (all indices shift by one)
            if (e.Effect.Equals(DragDropEffects.Move) && draggedNode.Parent == nodeDropTarget && nodeOver.Index > draggedNode.Index && dropAtLocation > 0)
            {
                dropAtLocation--;
            }

            // Ensure the drop location is never < 0
            if (dropAtLocation < 0)
            {
                dropAtLocation = 0;
            }

            var treeNodeInfo = getTreeNodeInfoForData(nodeDropTarget.Tag);
            var formerParentNode = draggedNode.Parent;

            // Move the tree node
            formerParentNode.Nodes.Remove(draggedNode);
            nodeDropTarget.Nodes.Insert(dropAtLocation, draggedNode);

            // Ensure the dragged node is visible afterwards
            draggedNode.EnsureVisible();

            // Restore any lost selection
            treeView.SelectedNode = draggedNode;

            if (treeNodeInfo.OnDrop != null)
            {
                treeNodeInfo.OnDrop(draggedNode.Tag, draggedNode.Parent.Tag, formerParentNode.Tag, dropAtLocation, treeViewControl);
            }
        }

        /// <summary>
        /// This method handles the <see cref="TreeView.DragOver"/> event for a <see cref="TreeViewControl"/>.
        /// </summary>
        /// <param name="treeView">The <see cref="TreeView"/> of the <see cref="TreeViewControl"/>.</param>
        /// <param name="e">The arguments of the <see cref="TreeView.DragOver"/> event.</param>
        /// <param name="getTreeNodeInfoForData">A function for obtaining the <see cref="TreeNodeInfo"/> object corresponding to a provided data object.</param>
        public void HandleDragOver(FormsTreeView treeView, DragEventArgs e, Func<object, TreeNodeInfo> getTreeNodeInfoForData)
        {
            if (lastDragOverPoint.X == e.X && lastDragOverPoint.Y == e.Y)
            {
                return;
            }

            lastDragOverPoint = new Point(e.X, e.Y);

            var point = treeView.PointToClient(lastDragOverPoint);
            var nodeOver = treeView.GetNodeAt(point);
            var draggedNode = (TreeNode) e.Data.GetData(typeof(TreeNode));

            if (nodeOver == null || nodeOver == draggedNode || IsChildOf(nodeOver, draggedNode))
            {
                ClearPlaceHolders(treeView);

                return;
            }

            ScrollIntoView(point, nodeOver, treeView);

            var placeholderLocation = GetPlaceHoldersLocation(treeView, draggedNode, nodeOver, e, getTreeNodeInfoForData);

            if (nodeDropTarget == null)
            {
                return;
            }

            var treeNodeInfo = getTreeNodeInfoForData(nodeDropTarget.Tag);
            var canDrop = treeNodeInfo.CanDrop != null && treeNodeInfo.CanDrop(draggedNode.Tag, nodeDropTarget.Tag);

            e.Effect = canDrop ? DragDropEffects.Move : DragDropEffects.None;

            if (placeholderLocation == PlaceholderLocation.None)
            {
                return;
            }

            if (canDrop)
            {
                DrawPlaceholder(treeView, nodeOver, placeholderLocation);
            }
            else
            {
                ClearPlaceHolders(treeView);

                e.Effect = DragDropEffects.None;
            }
        }

        /// <summary>
        /// This method handles the <see cref="TreeView.ItemDrag"/> event for a <see cref="TreeViewControl"/>.
        /// </summary>
        /// <param name="treeView">The <see cref="TreeView"/> of the <see cref="TreeViewControl"/>.</param>
        /// <param name="e">The arguments of the <see cref="TreeView.ItemDrag"/> event.</param>
        /// <param name="getTreeNodeInfoForData">A function for obtaining the <see cref="TreeNodeInfo"/> object corresponding to a provided data object.</param>
        public void HandleItemDrag(FormsTreeView treeView, ItemDragEventArgs e, Func<object, TreeNodeInfo> getTreeNodeInfoForData)
        {
            var draggedNode = (TreeNode) e.Item;
            var treeNodeInfo = getTreeNodeInfoForData(draggedNode.Tag);
            var parentTag = draggedNode.Parent != null ? draggedNode.Parent.Tag : null;

            var canDrag = treeNodeInfo.CanDrag != null && treeNodeInfo.CanDrag(draggedNode.Tag, parentTag);
            if (!canDrag)
            {
                return;
            }

            // Provide the drag drop operation with a data object containing the dragged tree node
            var dataObject = new DataObject();

            dataObject.SetData(typeof(TreeNode), draggedNode);

            treeView.DoDragDrop(dataObject, DragDropEffects.Move);
        }

        /// <summary>
        /// This method handles the <see cref="TreeView.DragLeave"/> event for a <see cref="TreeViewControl"/>.
        /// </summary>
        /// <param name="treeView">The <see cref="TreeView"/> of the <see cref="TreeViewControl"/>.</param>
        public void HandleDragLeave(FormsTreeView treeView)
        {
            ClearPlaceHolders(treeView);
        }

        private void DrawPlaceholder(FormsTreeView treeView, TreeNode node, PlaceholderLocation placeHolderLocation)
        {
            if (lastPlaceholderNode == node && lastPlaceholderLocation == placeHolderLocation)
            {
                return;
            }

            ClearPlaceHolders(treeView);

            lastPlaceholderNode = node;
            lastPlaceholderLocation = placeHolderLocation;

            placeHolderGraphics = treeView.CreateGraphics();
            node.DrawPlaceHolder(placeHolderLocation, treeView.CreateGraphics());
        }

        private void ClearPlaceHolders(FormsTreeView treeView)
        {
            if (placeHolderGraphics != null)
            {
                lastPlaceholderNode = null;

                treeView.Refresh();

                placeHolderGraphics.Dispose();
                placeHolderGraphics = null;
            }
        }

        private static bool IsChildOf(TreeNode childNode, TreeNode node)
        {
            while (childNode != null && childNode.Parent != null)
            {
                if (childNode.Parent.Equals(node))
                {
                    return true;
                }

                childNode = childNode.Parent; // Walk up the tree
            }

            return false;
        }

        private static void ScrollIntoView(Point point, TreeNode nodeOver, FormsTreeView treeView)
        {
            int delta = treeView.Height - point.Y;

            if (delta < treeView.Height/2 && delta > 0)
            {
                var nextVisibleNode = nodeOver.NextVisibleNode;
                if (nextVisibleNode != null)
                {
                    nextVisibleNode.EnsureVisible();
                }
            }

            if (delta > treeView.Height/2 && delta < treeView.Height)
            {
                var previousVisibleNode = nodeOver.PrevVisibleNode;
                if (previousVisibleNode != null)
                {
                    previousVisibleNode.EnsureVisible();
                }
            }
        }

        private PlaceholderLocation GetPlaceHoldersLocation(FormsTreeView treeView, TreeNode nodeDragging, TreeNode nodeOver, DragEventArgs e, Func<object, TreeNodeInfo> getTreeNodeInfoForData)
        {
            var placeholderLocation = PlaceholderLocation.None;
            int offsetY = treeView.PointToClient(Cursor.Position).Y - nodeOver.Bounds.Top;

            if (offsetY < nodeOver.Bounds.Height/3 && nodeDragging.NextNode != nodeOver)
            {
                if (nodeOver.Parent != null)
                {
                    var treeNodeInfo = getTreeNodeInfoForData(nodeOver.Parent.Tag);
                    if (treeNodeInfo.CanInsert != null && treeNodeInfo.CanInsert(nodeDragging.Tag, nodeOver.Tag))
                    {
                        nodeDropTarget = nodeOver.Parent;
                        dropAtLocation = nodeOver.Parent == null ? 0 : nodeOver.Parent.Nodes.IndexOf(nodeOver);
                        placeholderLocation = PlaceholderLocation.Top;
                    }
                    else
                    {
                        nodeDropTarget = nodeOver;
                        dropAtLocation = 0;
                        placeholderLocation = PlaceholderLocation.Middle;
                    }
                }
                else
                {
                    nodeDropTarget = nodeOver;
                    dropAtLocation = 0;
                    placeholderLocation = PlaceholderLocation.Middle;
                }
            }
            else if (nodeOver.Parent != null
                     && offsetY > nodeOver.Bounds.Height - nodeOver.Bounds.Height/3
                     && nodeDragging.PrevNode != nodeOver)
            {
                var treeNodeInfo = getTreeNodeInfoForData(nodeOver.Parent.Tag);
                if (treeNodeInfo.CanInsert != null && treeNodeInfo.CanInsert(nodeDragging.Tag, nodeOver.Tag))
                {
                    nodeDropTarget = nodeOver.Parent;
                    dropAtLocation = nodeOver.Parent != null
                                         ? nodeOver.Parent.Nodes.IndexOf(nodeOver) + 1
                                         : 0;
                    placeholderLocation = PlaceholderLocation.Bottom;
                }
                else
                {
                    nodeDropTarget = nodeOver;
                    dropAtLocation = 0;
                    placeholderLocation = PlaceholderLocation.Middle;
                }
            }
            else if (nodeDragging != nodeOver
                     && offsetY < nodeOver.Bounds.Height - nodeOver.Bounds.Height/4
                     && offsetY > nodeOver.Bounds.Height/4)
            {
                nodeDropTarget = nodeOver;
                dropAtLocation = 0;
                placeholderLocation = PlaceholderLocation.Middle;
            }

            if (placeholderLocation == PlaceholderLocation.None
                || (placeholderLocation == PlaceholderLocation.Middle && nodeDropTarget == nodeDragging.Parent))
            {
                ClearPlaceHolders(treeView);
                e.Effect = DragDropEffects.None;
            }

            return placeholderLocation;
        }
    }
}