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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights reserved.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Core.Common.Controls.TreeView
{
    /// <summary>
    /// Class that represents information for updating tree nodes.
    /// </summary>
    public class TreeNodeInfo
    {
        /// <summary>
        /// Constructs a new <see cref="TreeNodeInfo"/>.
        /// </summary>
        public TreeNodeInfo()
        {
            Text = tag => "";
        }

        /// <summary>
        /// Gets or sets the <see cref="Type"/> of the data wrapped by the tree node.
        /// </summary>
        public Type TagType { get; set; }

        /// <summary>
        /// Gets or sets a function for obtaining the tree node text.
        /// The <c>object</c> parameter represents the data of the tree node.
        /// </summary>
        public Func<object, string> Text { get; set; }

        /// <summary>
        /// Gets or sets a function for obtaining the tree node fore color.
        /// The <c>object</c> parameter represents the data of the tree node.
        /// </summary>
        public Func<object, Color> ForeColor { get; set; }

        /// <summary>
        /// Gets or sets a function for obtaining the tree node image.
        /// The <c>object</c> parameter represents the data of the tree node.
        /// </summary>
        public Func<object, Image> Image { get; set; }

        /// <summary>
        /// Gets or sets a function for obtaining the tree node context menu.
        /// The first <c>object</c> parameter represents the data of the tree node.
        /// The second <c>object</c> parameter represents the data of the parent tree node.
        /// The <see cref="TreeViewControl"/> parameter represents the current tree view control.
        /// </summary>
        public Func<object, object, TreeViewControl, ContextMenuStrip> ContextMenuStrip { get; set; }

        /// <summary>
        /// Gets or sets a function for determining whether or not the tree node should become visible and selected on creation.
        /// The <c>object</c> parameter represents the data of the tree node.
        /// </summary>
        public Func<object, bool> EnsureVisibleOnCreate { get; set; }

        /// <summary>
        /// Gets or sets a function for obtaining child node objects.
        /// The <c>object</c> parameter represents the data of the tree node.
        /// </summary>
        public Func<object, object[]> ChildNodeObjects { get; set; }

        /// <summary>
        /// Gets or sets a function for checking whether or not the tree node can be renamed.
        /// The first <c>object</c> parameter represents the data of the tree node.
        /// The second <c>object</c> parameter represents the data of the parent tree node.
        /// </summary>
        public Func<object, object, bool> CanRename { get; set; }

        /// <summary>
        /// Gets or sets an action for obtaining the logic to perform after renaming the tree node.
        /// The <c>object</c> parameter represents the data of the tree node.
        /// The <c>string</c> parameter represents the new name of the tree node.
        /// </summary>
        public Action<object, string> OnNodeRenamed { get; set; }

        /// <summary>
        /// Gets or sets a function for checking whether or not the tree node can be removed.
        /// The first <c>object</c> parameter represents the data of the tree node.
        /// The second <c>object</c> parameter represents the data of the parent tree node.
        /// </summary>
        public Func<object, object, bool> CanRemove { get; set; }

        /// <summary>
        /// Gets or sets an action for obtaining the logic to perform after removing the tree node.
        /// The <c>object</c> parameter represents the data of the tree node.
        /// The <c>object</c> parameter represents the data of the parent tree node.
        /// </summary>
        public Action<object, object> OnNodeRemoved { get; set; }

        /// <summary>
        /// Gets or sets a function for checking whether or not the tree node can be checked.
        /// The <c>object</c> parameter represents the data of the tree node.
        /// </summary>
        public Func<object, bool> CanCheck { get; set; }

        /// <summary>
        /// Gets or sets a function for checking whether or not the tree node should be checked.
        /// The <c>object</c> parameter represents the data of the tree node.
        /// </summary>
        public Func<object, bool> IsChecked { get; set; }

        /// <summary>
        /// Gets or sets an action for obtaining the logic to perform after checking or unchecking the tree node.
        /// The first <c>object</c> parameter represents the data of the tree node.
        /// The second <c>object</c> parameter represents the data of the parent tree node.
        /// </summary>
        public Action<object, object> OnNodeChecked { get; set; }

        /// <summary>
        /// Gets or sets a function for checking whether or not the tree node can be dragged to another location.
        /// The <c>object</c> parameter represents the data of the tree node.
        /// The <see cref="TreeNode"/> parameter represents the tree node which is dragged.
        /// </summary>
        public Func<object, TreeNode, DragOperations> CanDrag { get; set; }

        /// <summary>
        /// Gets or sets a function for checking whether or not the tree node can be dropped to another location.
        /// The first <see cref="TreeNode"/> parameter represents the tree node which is dragged.
        /// The second <see cref="TreeNode"/> parameter represents the tree node being considered as drop target.
        /// The <see cref="DragOperations"/> parameter represents the supported drop operations for the tree node which is dragged.
        /// The <see cref="DragOperations"/> return value indicates what operation is valid when the tree node is dropped onto the drop target.
        /// </summary>
        /// <remarks>When dragging a node, the <see cref="CanDrop"/> function of the <see cref="TreeNodeInfo"/> of the drop target should be called.</remarks>
        public Func<TreeNode, TreeNode, DragOperations, DragOperations> CanDrop { get; set; }

        /// <summary>
        /// Gets or sets a function for checking whether or not the tree node can be inserted into the drop target at a specific index.
        /// The first <see cref="TreeNode"/> parameter represents the tree node which is dragged.
        /// The second <see cref="TreeNode"/> parameter represents the tree node being considered as drop target.
        /// </summary>
        public Func<TreeNode, TreeNode, bool> CanInsert { get; set; }

        /// <summary>
        /// Gets or sets an action for obtaining the logic to perform after dropping a tree node.
        /// The first <see cref="TreeNode"/> parameter represents the tree node which was dropped.
        /// The second <see cref="TreeNode"/> parameter represents the former parent tree node.
        /// The <see cref="DragOperations"/> parameter represents the type of drag operation that was performed.
        /// The <see cref="int"/> parameter represents the drop target index which the tree node was inserted at.
        /// </summary>
        /// <remarks>After dropping a node, the <see cref="OnDrop"/> function of the <see cref="TreeNodeInfo"/> of the drop target should be called.</remarks>
        public Action<TreeNode, TreeNode, DragOperations, int> OnDrop { get; set; }
    }

    /// <summary>
    /// Class that represents information for updating tree nodes.
    /// </summary>
    /// <typeparam name="TData">The type of data wrapped by the tree node.</typeparam>
    public class TreeNodeInfo<TData>
    {
        /// <summary>
        /// Constructs a new <see cref="TreeNodeInfo{TData}"/>.
        /// </summary>
        public TreeNodeInfo()
        {
            Text = tag => "";
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of the data wrapped by the tree node.
        /// </summary>
        public Type TagType
        {
            get
            {
                return typeof(TData);
            }
        }

        /// <summary>
        /// Gets or sets a function for obtaining the tree node text.
        /// The <typeparamref name="TData"/> parameter represents the data of the tree node.
        /// </summary>
        public Func<TData, string> Text { get; set; }

        /// <summary>
        /// Gets or sets a function for obtaining the tree node color.
        /// The <typeparamref name="TData"/> parameter represents the data of the tree node.
        /// </summary>
        public Func<TData, Color> ForeColor { get; set; }

        /// <summary>
        /// Gets or sets a function for obtaining the tree node image.
        /// The <typeparamref name="TData"/> parameter represents the data of the tree node.
        /// </summary>
        public Func<TData, Image> Image { get; set; }

        /// <summary>
        /// Gets or sets a function for obtaining the tree node context menu.
        /// The <typeparamref name="TData"/> parameter represents the data of the tree node.
        /// The <c>object</c> parameter represents the data of the parent tree node.
        /// The <see cref="TreeViewControl"/> parameter represents the current tree view control.
        /// </summary>
        public Func<TData, object, TreeViewControl, ContextMenuStrip> ContextMenuStrip { get; set; }

        /// <summary>
        /// Gets or sets a function for determining whether or not the tree node should become visible and selected on creation.
        /// The <typeparamref name="TData"/> parameter represents the data of the tree node.
        /// </summary>
        public Func<TData, bool> EnsureVisibleOnCreate { get; set; }

        /// <summary>
        /// Gets or sets a function for obtaining child node objects.
        /// The <typeparamref name="TData"/> parameter represents the data of the tree node.
        /// </summary>
        public Func<TData, object[]> ChildNodeObjects { get; set; }

        /// <summary>
        /// Gets or sets a function for checking whether or not the tree node can be renamed.
        /// The <see cref="TData"/> parameter represents the data of the tree node.
        /// The <c>object</c> parameter represents the data of the parent tree node.
        /// </summary>
        public Func<TData, object, bool> CanRename { get; set; }

        /// <summary>
        /// Gets or sets an action for obtaining the logic to perform after renaming the tree node.
        /// The <typeparamref name="TData"/> parameter represents the data of the tree node.
        /// The <c>string</c> parameter represents the new name of the tree node.
        /// </summary>
        public Action<TData, string> OnNodeRenamed { get; set; }

        /// <summary>
        /// Gets or sets a function for checking whether or not the tree node can be removed.
        /// The <typeparamref name="TData"/> parameter represents the data of the tree node.
        /// The <c>object</c> parameter represents the data of the parent tree node.
        /// </summary>
        public Func<TData, object, bool> CanRemove { get; set; }

        /// <summary>
        /// Gets or sets an action for obtaining the logic to perform after removing the tree node.
        /// The <typeparamref name="TData"/> parameter represents the data of the tree node.
        /// The <c>object</c> parameter represents the data of the parent tree node.
        /// </summary>
        public Action<TData, object> OnNodeRemoved { get; set; }

        /// <summary>
        /// Gets or sets a function for checking whether or not the tree node can be checked.
        /// The <typeparamref name="TData"/> parameter represents the data of the tree node.
        /// </summary>
        public Func<TData, bool> CanCheck { get; set; }

        /// <summary>
        /// Gets or sets a function for checking whether or not the tree node should be checked.
        /// The <typeparamref name="TData"/> parameter represents the data of the tree node.
        /// </summary>
        public Func<TData, bool> IsChecked { get; set; }

        /// <summary>
        /// Gets or sets an action for obtaining the logic to perform after checking or unchecking the tree node.
        /// The <see cref="TData"/> parameter represents the data of the tree node.
        /// The <c>object</c> parameter represents the data of the parent tree node.
        /// </summary>
        public Action<TData, object> OnNodeChecked { get; set; }

        /// <summary>
        /// Gets or sets a function for checking whether or not the tree node can be dragged to another location.
        /// The <typeparamref name="TData"/> parameter represents the data of the tree node.
        /// The <see cref="TreeNode"/> parameter represents the tree node which is dragged.
        /// </summary>
        public Func<TData, TreeNode, DragOperations> CanDrag { get; set; }

        /// <summary>
        /// Gets or sets a function for checking whether or not the tree node can be dropped to another location.
        /// The first <see cref="TreeNode"/> parameter represents the tree node which is dragged.
        /// The second <see cref="TreeNode"/> parameter represents the tree node being considered as drop target.
        /// The <see cref="DragOperations"/> parameter represents the supported drop operations for the tree node which is dragged.
        /// The <see cref="DragOperations"/> return value indicates what operation is valid when the tree node is dropped onto the drop target.
        /// </summary>
        /// <remarks>When dragging a node, the <see cref="CanDrop"/> function of the <see cref="TreeNodeInfo"/> of the drop target should be called.</remarks>
        public Func<TreeNode, TreeNode, DragOperations, DragOperations> CanDrop { get; set; }

        /// <summary>
        /// Gets or sets a function for checking whether or not the tree node can be inserted into the drop target at a specific index.
        /// The first <see cref="TreeNode"/> parameter represents the tree node which is dragged.
        /// The second <see cref="TreeNode"/> parameter represents the tree node being considered as drop target.
        /// </summary>
        public Func<TreeNode, TreeNode, bool> CanInsert { get; set; }

        /// <summary>
        /// Gets or sets an action for obtaining the logic to perform after dropping a tree node.
        /// The first <see cref="TreeNode"/> parameter represents the tree node which was dropped.
        /// The second <see cref="TreeNode"/> parameter represents the former parent tree node.
        /// The <see cref="DragOperations"/> parameter represents the type of drag operation that was performed.
        /// The <see cref="int"/> parameter represents the drop target index which the tree node was inserted at.
        /// </summary>
        /// <remarks>After dropping a node, the <see cref="OnDrop"/> function of the <see cref="TreeNodeInfo"/> of the drop target should be called.</remarks>
        public Action<TreeNode, TreeNode, DragOperations, int> OnDrop { get; set; }

        /// <summary>
        /// This operator converts a <see cref="TreeNodeInfo{TData}"/> into a <see cref="TreeNodeInfo"/>.
        /// </summary>
        /// <param name="treeNodeInfo">The <see cref="TreeNodeInfo{TData}"/> to convert.</param>
        /// <returns>The converted <see cref="TreeNodeInfo"/>.</returns>
        public static implicit operator TreeNodeInfo(TreeNodeInfo<TData> treeNodeInfo)
        {
            return new TreeNodeInfo
            {
                TagType = treeNodeInfo.TagType,
                Text = treeNodeInfo.Text != null
                           ? tag => treeNodeInfo.Text((TData) tag)
                           : (Func<object, string>) null,
                ForeColor = treeNodeInfo.ForeColor != null
                                ? tag => treeNodeInfo.ForeColor((TData) tag)
                                : (Func<object, Color>) null,
                Image = treeNodeInfo.Image != null
                            ? tag => treeNodeInfo.Image((TData) tag)
                            : (Func<object, Image>) null,
                ContextMenuStrip = treeNodeInfo.ContextMenuStrip != null
                                       ? (tag, parentTag, treeViewControl) => treeNodeInfo.ContextMenuStrip((TData) tag, parentTag, treeViewControl)
                                       : (Func<object, object, TreeViewControl, ContextMenuStrip>) null,
                EnsureVisibleOnCreate = treeNodeInfo.EnsureVisibleOnCreate != null
                                            ? tag => treeNodeInfo.EnsureVisibleOnCreate((TData) tag)
                                            : (Func<object, bool>) null,
                ChildNodeObjects = treeNodeInfo.ChildNodeObjects != null
                                       ? tag => treeNodeInfo.ChildNodeObjects((TData) tag)
                                       : (Func<object, object[]>) null,
                CanRename = treeNodeInfo.CanRename != null
                                ? (tag, parentTag) => treeNodeInfo.CanRename((TData) tag, parentTag)
                                : (Func<object, object, bool>) null,
                OnNodeRenamed = treeNodeInfo.OnNodeRenamed != null
                                    ? (tag, name) => treeNodeInfo.OnNodeRenamed((TData) tag, name)
                                    : (Action<object, string>) null,
                CanRemove = treeNodeInfo.CanRemove != null
                                ? (tag, parentTag) => treeNodeInfo.CanRemove((TData) tag, parentTag)
                                : (Func<object, object, bool>) null,
                OnNodeRemoved = treeNodeInfo.OnNodeRemoved != null
                                    ? (tag, parentTag) => treeNodeInfo.OnNodeRemoved((TData) tag, parentTag)
                                    : (Action<object, object>) null,
                CanCheck = treeNodeInfo.CanCheck != null
                               ? tag => treeNodeInfo.CanCheck((TData) tag)
                               : (Func<object, bool>) null,
                IsChecked = treeNodeInfo.IsChecked != null
                                ? tag => treeNodeInfo.IsChecked((TData) tag)
                                : (Func<object, bool>) null,
                OnNodeChecked = treeNodeInfo.OnNodeChecked != null
                                    ? (tag, parentTag) => treeNodeInfo.OnNodeChecked((TData) tag, parentTag)
                                    : (Action<object, object>) null,
                CanDrag = treeNodeInfo.CanDrag != null
                              ? (tag, sourceNode) => treeNodeInfo.CanDrag((TData) tag, sourceNode)
                              : (Func<object, TreeNode, DragOperations>) null,
                CanDrop = treeNodeInfo.CanDrop != null
                              ? (sourceNode, targetNode, dragOperations) => treeNodeInfo.CanDrop(sourceNode, targetNode, dragOperations)
                              : (Func<TreeNode, TreeNode, DragOperations, DragOperations>) null,
                CanInsert = treeNodeInfo.CanInsert != null
                                ? (sourceNode, targetNode) => treeNodeInfo.CanInsert(sourceNode, targetNode)
                                : (Func<TreeNode, TreeNode, bool>) null,
                OnDrop = treeNodeInfo.OnDrop != null
                             ? (sourceNode, targetNode, dragOperations, index) => treeNodeInfo.OnDrop(sourceNode, targetNode, dragOperations, index)
                             : (Action<TreeNode, TreeNode, DragOperations, int>) null,
            };
        }
    }
}