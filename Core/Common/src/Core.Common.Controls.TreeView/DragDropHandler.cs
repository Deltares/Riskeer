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
    public class DragDropHandler
    {
        private int dropAtLocation;
        private Point lastDragOverPoint;
        private PlaceholderLocation lastPlaceholderLocation;
        private TreeNode nodeDropTarget;
        private TreeNode lastPlaceholderNode;
        private Graphics placeHolderGraphics;

        public void HandleDragDrop(TreeViewControl treeViewControl, FormsTreeView treeView, DragEventArgs e, Func<object, TreeNodeInfo> getTreeNodeInfoForData)
        {
            ClearPlaceHolders(treeView);

            Point point = treeView.PointToClient(new Point(e.X, e.Y));
            var nodeOver = treeView.GetNodeAt(point);

            var nodeDragging = e.Data.GetData(typeof(TreeNode)) as TreeNode;

            if (nodeOver == null || nodeDragging == null)
            {
                if (nodeOver != null)
                {
                    e.Effect = DragDropEffects.All;
                }

                return;
            }

            // Handle dragged items which were originally higher up in the tree under the same parent (all indices shift by one)
            if (e.Effect.Equals(DragDropEffects.Move) && nodeDragging.Parent == nodeDropTarget && nodeOver.Index > nodeDragging.Index && dropAtLocation > 0)
            {
                dropAtLocation--;
            }

            // Ensure the drop location is never < 0
            if (dropAtLocation < 0)
            {
                dropAtLocation = 0;
            }

            var treeNodeInfo = getTreeNodeInfoForData(nodeDropTarget.Tag);
            var previousParentNode = nodeDragging.Parent;

            previousParentNode.Nodes.Remove(nodeDragging);
            nodeDropTarget.Nodes.Insert(dropAtLocation, nodeDragging);

            // Ensure the dragged node is visible afterwards
            nodeDragging.EnsureVisible();

            // Restore any lost selection
            treeView.SelectedNode = nodeDragging;

            if (treeNodeInfo.OnDrop != null)
            {
                treeNodeInfo.OnDrop(nodeDragging.Tag, nodeDragging.Parent.Tag, previousParentNode.Tag, dropAtLocation, treeViewControl);
            }
        }

        public void HandleDragOver(FormsTreeView treeView, object sender, DragEventArgs e, Func<object, TreeNodeInfo> getTreeNodeInfoForData)
        {
            if (lastDragOverPoint.X == e.X && lastDragOverPoint.Y == e.Y)
            {
                return;
            }

            lastDragOverPoint = new Point(e.X, e.Y);

            var point = treeView.PointToClient(lastDragOverPoint);
            var nodeOver = treeView.GetNodeAt(point);
            var nodeDragging = e.Data.GetData(typeof(TreeNode)) as TreeNode;

            if (nodeOver == null || nodeDragging == null || nodeOver == nodeDragging || IsChildOf(nodeOver, nodeDragging))
            {
                ClearPlaceHolders(treeView);

                return;
            }

            ScrollIntoView(point, nodeOver, sender);

            PlaceholderLocation placeholderLocation = GetPlaceHoldersLocation(treeView, nodeDragging, nodeOver, e, getTreeNodeInfoForData);

            if (null == nodeDropTarget)
            {
                return;
            }

            var treeNodeInfo = getTreeNodeInfoForData(nodeDropTarget.Tag);

            DragOperations allowedOperations = treeNodeInfo.CanDrop != null
                                                   ? treeNodeInfo.CanDrop(nodeDragging.Tag, nodeDropTarget.Tag, ToDragOperation(e.AllowedEffect))
                                                   : DragOperations.None;

            e.Effect = ToDragDropEffects(allowedOperations);

            if (PlaceholderLocation.None == placeholderLocation)
            {
                return;
            }

            // Determine whether or not the node can be dropped based on the allowed operations.
            // A node can also be a valid drop target if it is the root item (nodeDragging.Parent == null).
            var dragOperations = treeNodeInfo.CanDrop != null
                                     ? treeNodeInfo.CanDrop(nodeDragging.Tag, nodeDropTarget.Tag, allowedOperations)
                                     : DragOperations.None;

            if (DragOperations.None != dragOperations)
            {
                DrawPlaceholder(treeView, nodeOver, placeholderLocation);
            }
            else
            {
                ClearPlaceHolders(treeView);

                e.Effect = DragDropEffects.None;
            }
        }

        public void HandleItemDrag(FormsTreeView treeView, ItemDragEventArgs e, Func<object, TreeNodeInfo> getTreeNodeInfoForData)
        {
            // gather allowed effects for the current item.
            var sourceNode = (TreeNode) e.Item;
            var treeNodeInfo = getTreeNodeInfoForData(sourceNode.Tag);
            var parentTag = sourceNode.Parent != null ? sourceNode.Parent.Tag : null;

            var canDrag = treeNodeInfo.CanDrag != null && treeNodeInfo.CanDrag(sourceNode.Tag, parentTag);
            if (!canDrag)
            {
                return;
            }

            // store both treenode and tag of treenode in dataobject
            // to be dragged.
            IDataObject dataObject = new DataObject();
            dataObject.SetData(typeof(TreeNode), sourceNode);

            if (sourceNode.Tag != null)
            {
                dataObject.SetData(sourceNode.Tag.GetType(), sourceNode.Tag);
            }

            treeView.DoDragDrop(dataObject, DragDropEffects.Move);
        }

        public void HandleDragLeave(FormsTreeView treeView)
        {
            ClearPlaceHolders(treeView);
        }

        private void DrawPlaceholder(FormsTreeView treeView, TreeNode node, PlaceholderLocation location)
        {
            if (lastPlaceholderNode == node && lastPlaceholderLocation == location)
            {
                return;
            }

            ClearPlaceHolders(treeView);

            lastPlaceholderNode = node;
            lastPlaceholderLocation = location;

            placeHolderGraphics = treeView.CreateGraphics();
            node.DrawPlaceHolder(location, treeView.CreateGraphics());
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

        private DragOperations ToDragOperation(DragDropEffects dragDropEffects)
        {
            return (DragOperations) Enum.Parse(typeof(DragOperations), dragDropEffects.ToString());
        }

        private bool IsChildOf(TreeNode childNode, TreeNode node)
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

        private static void ScrollIntoView(Point point, TreeNode nodeOver, object sender)
        {
            var treeView = sender as FormsTreeView;
            if (treeView == null)
            {
                return;
            }
            int delta = treeView.Height - point.Y;
            if ((delta < treeView.Height/2) && (delta > 0))
            {
                var nextVisibleNode = nodeOver.NextVisibleNode;
                if (nextVisibleNode != null)
                {
                    nextVisibleNode.EnsureVisible();
                }
            }
            if ((delta > treeView.Height/2) && (delta < treeView.Height))
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
            var loc = PlaceholderLocation.None;
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
                        loc = PlaceholderLocation.Top;
                    }
                    else
                    {
                        nodeDropTarget = nodeOver;
                        dropAtLocation = 0;
                        loc = PlaceholderLocation.Middle;
                    }
                }
                else
                {
                    nodeDropTarget = nodeOver;
                    dropAtLocation = 0;
                    loc = PlaceholderLocation.Middle;
                }
            }
            else if ((nodeOver.Parent != null) && (offsetY > nodeOver.Bounds.Height - nodeOver.Bounds.Height/3) &&
                     nodeDragging.PrevNode != nodeOver)
            {
                var treeNodeInfo = getTreeNodeInfoForData(nodeOver.Parent.Tag);
                if (treeNodeInfo.CanInsert != null && treeNodeInfo.CanInsert(nodeDragging.Tag, nodeOver.Tag))
                {
                    nodeDropTarget = nodeOver.Parent;
                    dropAtLocation = nodeOver.Parent == null
                                         ? 0
                                         : nodeOver.Parent.Nodes.IndexOf(nodeOver) + 1;
                    loc = PlaceholderLocation.Bottom;
                }
                else
                {
                    nodeDropTarget = nodeOver;
                    dropAtLocation = 0;
                    loc = PlaceholderLocation.Middle;
                }
            }
            else if (nodeDragging != nodeOver && offsetY < nodeOver.Bounds.Height - nodeOver.Bounds.Height/4
                     && offsetY > nodeOver.Bounds.Height/4)
            {
                nodeDropTarget = nodeOver;
                dropAtLocation = 0;
                loc = PlaceholderLocation.Middle;
            }

            if (loc == PlaceholderLocation.None ||
                (loc == PlaceholderLocation.Middle && nodeDropTarget == nodeDragging.Parent))
            {
                ClearPlaceHolders(treeView);
                e.Effect = DragDropEffects.None;
            }
            return loc;
        }

        private DragDropEffects ToDragDropEffects(DragOperations operation)
        {
            return (DragDropEffects) Enum.Parse(typeof(DragDropEffects), operation.ToString());
        }
    }
}