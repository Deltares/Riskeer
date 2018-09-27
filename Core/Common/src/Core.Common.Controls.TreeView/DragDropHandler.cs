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
using FormsTreeView = System.Windows.Forms.TreeView;

namespace Core.Common.Controls.TreeView
{
    /// <summary>
    /// This class handles the drag and drop related logic for a <see cref="TreeViewControl"/>.
    /// </summary>
    public class DragDropHandler
    {
        private enum PlaceholderLocation
        {
            Top,
            Bottom,
            Middle,
            None
        }

        private const int defaultImageWidth = 16;
        private const int spaceBetweenNodeParts = 2;

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
            RemovePlaceHolder(treeView);

            Point point = treeView.PointToClient(new Point(e.X, e.Y));
            TreeNode nodeOver = treeView.GetNodeAt(point);
            TreeNode draggedNode = GetDraggedNodeData(e);

            if (nodeOver == null)
            {
                return;
            }

            SetDropAtLocation(e, draggedNode, nodeOver);

            TreeNodeInfo treeNodeInfo = getTreeNodeInfoForData(nodeDropTarget.Tag);
            TreeNode formerParentNode = draggedNode.Parent;

            // Move the tree node
            formerParentNode.Nodes.Remove(draggedNode);
            nodeDropTarget.Nodes.Insert(dropAtLocation, draggedNode);

            // Ensure the dragged node is visible afterwards
            draggedNode.EnsureVisible();

            // Restore any lost selection
            treeView.SelectedNode = draggedNode;

            treeNodeInfo.OnDrop?.Invoke(draggedNode.Tag, draggedNode.Parent.Tag, formerParentNode.Tag, dropAtLocation, treeViewControl);
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

            Point point = treeView.PointToClient(lastDragOverPoint);
            TreeNode nodeOver = treeView.GetNodeAt(point);
            TreeNode draggedNode = GetDraggedNodeData(e);

            if (draggedNode == null || nodeOver == null || nodeOver == draggedNode || IsDropTargetChildOfDraggedNode(nodeOver, draggedNode))
            {
                RemovePlaceHolder(treeView);
                e.Effect = DragDropEffects.None;
                return;
            }

            ScrollIntoView(point, nodeOver, treeView);

            PlaceholderLocation placeholderLocation = GetPlaceHoldersLocation(treeView, draggedNode, nodeOver, getTreeNodeInfoForData);

            if (nodeDropTarget == null)
            {
                return;
            }

            TreeNodeInfo treeNodeInfo = getTreeNodeInfoForData(nodeDropTarget.Tag);
            bool canDrop = treeNodeInfo.CanDrop != null && treeNodeInfo.CanDrop(draggedNode.Tag, nodeDropTarget.Tag);

            e.Effect = canDrop ? DragDropEffects.Move : DragDropEffects.None;

            if (placeholderLocation == PlaceholderLocation.None)
            {
                RemovePlaceHolder(treeView);
                e.Effect = DragDropEffects.None;

                return;
            }

            if (canDrop)
            {
                CreatePlaceholder(treeView, nodeOver, placeholderLocation);
            }
            else
            {
                RemovePlaceHolder(treeView);

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
            treeView.SelectedNode = draggedNode;
            TreeNodeInfo treeNodeInfo = getTreeNodeInfoForData(draggedNode.Tag);
            object parentTag = draggedNode.Parent?.Tag;

            if (treeNodeInfo.CanDrag == null || !treeNodeInfo.CanDrag(draggedNode.Tag, parentTag))
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
            RemovePlaceHolder(treeView);
        }

        private static TreeNode GetDraggedNodeData(DragEventArgs e)
        {
            try
            {
                return (TreeNode) e.Data.GetData(typeof(TreeNode));
            }
            catch (InvalidCastException)
            {
                return null;
            }
        }

        private void SetDropAtLocation(DragEventArgs e, TreeNode draggedNode, TreeNode nodeOver)
        {
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
        }

        private void CreatePlaceholder(FormsTreeView treeView, TreeNode node, PlaceholderLocation placeHolderLocation)
        {
            if (lastPlaceholderNode == node && lastPlaceholderLocation == placeHolderLocation)
            {
                return;
            }

            RemovePlaceHolder(treeView);

            lastPlaceholderNode = node;
            lastPlaceholderLocation = placeHolderLocation;

            placeHolderGraphics = treeView.CreateGraphics();
            DrawPlaceHolder(node, placeHolderLocation, treeView.CreateGraphics());
        }

        private void RemovePlaceHolder(FormsTreeView treeView)
        {
            if (placeHolderGraphics != null)
            {
                lastPlaceholderNode = null;

                treeView.Refresh();

                placeHolderGraphics.Dispose();
                placeHolderGraphics = null;
            }
        }

        private static bool IsDropTargetChildOfDraggedNode(TreeNode dropTarget, TreeNode draggedNode)
        {
            while (dropTarget?.Parent != null)
            {
                if (dropTarget.Parent.Equals(draggedNode))
                {
                    return true;
                }

                dropTarget = dropTarget.Parent; // Walk up the tree
            }

            return false;
        }

        private static void ScrollIntoView(Point point, TreeNode nodeOver, FormsTreeView treeView)
        {
            int delta = treeView.Height - point.Y;
            double halfTreeViewHeight = treeView.Height / 2.0;

            if (delta < halfTreeViewHeight && delta > 0)
            {
                TreeNode nextVisibleNode = nodeOver.NextVisibleNode;
                nextVisibleNode?.EnsureVisible();
            }

            if (delta > halfTreeViewHeight && delta < treeView.Height)
            {
                TreeNode previousVisibleNode = nodeOver.PrevVisibleNode;
                previousVisibleNode?.EnsureVisible();
            }
        }

        private PlaceholderLocation GetPlaceHoldersLocation(FormsTreeView treeView, TreeNode nodeDragging, TreeNode nodeOver, Func<object, TreeNodeInfo> getTreeNodeInfoForData)
        {
            var placeholderLocation = PlaceholderLocation.None;
            int offsetY = treeView.PointToClient(Cursor.Position).Y - nodeOver.Bounds.Top;

            if (offsetY < nodeOver.Bounds.Height / 3 && nodeDragging.NextNode != nodeOver)
            {
                if (nodeOver.Parent != null)
                {
                    TreeNodeInfo treeNodeInfo = getTreeNodeInfoForData(nodeOver.Parent.Tag);
                    if (treeNodeInfo.CanInsert != null && treeNodeInfo.CanInsert(nodeDragging.Tag, nodeOver.Parent.Tag))
                    {
                        nodeDropTarget = nodeOver.Parent;
                        dropAtLocation = nodeOver.Parent.Nodes.IndexOf(nodeOver);
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
            else if (nodeOver.Parent != null &&
                     offsetY > nodeOver.Bounds.Height - nodeOver.Bounds.Height / 3 &&
                     nodeDragging.PrevNode != nodeOver)
            {
                TreeNodeInfo treeNodeInfo = getTreeNodeInfoForData(nodeOver.Parent.Tag);
                if (treeNodeInfo.CanInsert != null && treeNodeInfo.CanInsert(nodeDragging.Tag, nodeOver.Parent.Tag))
                {
                    nodeDropTarget = nodeOver.Parent;
                    dropAtLocation = nodeOver.Parent.Nodes.IndexOf(nodeOver) + 1;
                    placeholderLocation = PlaceholderLocation.Bottom;
                }
                else
                {
                    nodeDropTarget = nodeOver;
                    dropAtLocation = 0;
                    placeholderLocation = PlaceholderLocation.Middle;
                }
            }
            else if (nodeDragging != nodeOver &&
                     offsetY < nodeOver.Bounds.Height - nodeOver.Bounds.Height / 4 &&
                     offsetY > nodeOver.Bounds.Height / 4)
            {
                nodeDropTarget = nodeOver;
                dropAtLocation = 0;
                placeholderLocation = PlaceholderLocation.Middle;
            }

            return placeholderLocation;
        }

        private static void DrawPlaceHolder(TreeNode node, PlaceholderLocation location, Graphics graphics)
        {
            Point[] rightTriangle = MakePlaceHoldeTriangle(node, AnchorStyles.Right, location);

            graphics.FillPolygon(Brushes.Black, rightTriangle);

            if (location == PlaceholderLocation.Middle)
            {
                return;
            }

            Point[] leftTriangle = MakePlaceHoldeTriangle(node, AnchorStyles.Left, location);
            graphics.FillPolygon(Brushes.Black, leftTriangle);

            int yLine = location == PlaceholderLocation.Top
                            ? node.Bounds.Top
                            : node.Bounds.Bottom;

            graphics.DrawLine(new Pen(Color.Black, 1), new Point(GetImageLeft(node), yLine), new Point(node.Bounds.Right, yLine));
        }

        private static Point[] MakePlaceHoldeTriangle(TreeNode node, AnchorStyles anchor, PlaceholderLocation location)
        {
            const int placeHolderWidth = 4;
            const int placeHolderHeight = 8;

            int xPos, yPos;
            Rectangle bounds = node.Bounds;

            switch (anchor)
            {
                case AnchorStyles.Left:
                    xPos = GetImageLeft(node) - placeHolderWidth;
                    break;
                case AnchorStyles.Right:
                    xPos = bounds.Right;
                    break;
                default:
                    throw new NotSupportedException();
            }

            switch (location)
            {
                case PlaceholderLocation.Top:
                    yPos = bounds.Top;
                    break;
                case PlaceholderLocation.Bottom:
                    yPos = bounds.Bottom;
                    break;
                case PlaceholderLocation.Middle:
                    yPos = bounds.Top + bounds.Height / 2;
                    break;
                default:
                    throw new NotSupportedException();
            }

            return CreateTrianglePoints(new Rectangle(xPos, yPos - placeHolderWidth, placeHolderWidth, placeHolderHeight), anchor);
        }

        private static int GetImageLeft(TreeNode node)
        {
            return node.Bounds.Left - (defaultImageWidth + spaceBetweenNodeParts);
        }

        private static Point[] CreateTrianglePoints(Rectangle bounds, AnchorStyles anchor)
        {
            switch (anchor)
            {
                case AnchorStyles.Left:
                    return new[]
                    {
                        new Point(bounds.Left, bounds.Top),
                        new Point(bounds.Right, bounds.Top + bounds.Height / 2),
                        new Point(bounds.Left, bounds.Top + bounds.Height),
                        new Point(bounds.Left, bounds.Top)
                    };
                case AnchorStyles.Right:
                    return new[]
                    {
                        new Point(bounds.Right, bounds.Top),
                        new Point(bounds.Left, bounds.Top + bounds.Height / 2),
                        new Point(bounds.Right, bounds.Top + bounds.Height),
                        new Point(bounds.Right, bounds.Top)
                    };
                default:
                    return new Point[0];
            }
        }
    }
}