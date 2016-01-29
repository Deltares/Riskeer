﻿// Copyright (C) Stichting Deltares 2016. All rights preserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights preserved.

//using System;
//using System.Collections;
//using System.Linq;
//using System.Windows.Forms;
//
//using Core.Common.Controls.TreeView;
//using Core.Common.Gui;
//using Ringtoets.Common.Forms.Properties;
//using TreeNode = Core.Common.Controls.TreeView.TreeNode;
//using TreeView = Core.Common.Controls.TreeView.TreeView;
//
//namespace Ringtoets.Common.Forms.NodePresenters
//{
//    /// <summary>
//    /// Implements <see cref="ITreeNodePresenter"/> in a featureless way as possible,
//    /// to serve as a base class for all node presenters.
//    /// </summary>
//    /// <typeparam name="T">The data object class corresponding with the node.</typeparam>
//    public abstract class RingtoetsNodePresenterBase<T> : ITreeNodePresenter
//    {
//        protected readonly IContextMenuBuilderProvider contextMenuBuilderProvider;
//
//        /// <summary>
//        /// Creates a new instance of <see cref="RingtoetsNodePresenterBase{T}"/>, which uses the 
//        /// <paramref name="contextMenuBuilderProvider"/> to create and bind its <see cref="ContextMenuStrip"/>.
//        /// </summary>
//        /// <param name="contextMenuBuilderProvider">The <see cref="IContextMenuBuilderProvider"/> 
//        /// to use for  building a <see cref="ContextMenuStrip"/>.</param>
//        /// <exception cref="ArgumentNullException">Thrown when no <paramref name="contextMenuBuilderProvider"/> was provided.</exception>
//        public RingtoetsNodePresenterBase(IContextMenuBuilderProvider contextMenuBuilderProvider)
//        {
//            if (contextMenuBuilderProvider == null)
//            {
//                throw new ArgumentNullException("contextMenuBuilderProvider", Core.Common.Gui.Properties.Resources.NodePresenter_ContextMenuBuilderProvider_required);
//            }
//            this.contextMenuBuilderProvider = contextMenuBuilderProvider;
//        }
//
//        public TreeView TreeView { get; set; }
//
//        public Type NodeTagType
//        {
//            get
//            {
//                return typeof(T);
//            }
//        }
//
//        public void UpdateNode(TreeNode parentNode, TreeNode node, object nodeData)
//        {
//            UpdateNode(parentNode, node, (T)nodeData);
//        }
//
//        public IEnumerable GetChildNodeObjects(object parentNodeData)
//        {
//            return GetChildNodeObjects((T)parentNodeData).Cast<object>();
//        }
//
//        public virtual bool CanRenameNode(TreeNode node)
//        {
//            return false;
//        }
//
//        public virtual bool CanRenameNodeTo(TreeNode node, string newName)
//        {
//            return false;
//        }
//
//        public void OnNodeRenamed(object nodeData, string newName)
//        {
//            var data = (T)nodeData;
//            OnNodeRenamed(data, newName);
//        }
//
//        public virtual void OnNodeChecked(TreeNode node)
//        {
//            // Not a checked node
//        }
//
//        public DragOperations CanDrag(object nodeData)
//        {
//            return CanDrag((T)nodeData);
//        }
//
//        public virtual DragOperations CanDrop(object item, TreeNode sourceNode, TreeNode targetNode, DragOperations validOperations)
//        {
//            return DragOperations.None;
//        }
//
//        public virtual bool CanInsert(object item, TreeNode sourceNode, TreeNode targetNode)
//        {
//            return false;
//        }
//
//        public void OnDragDrop(object item, object itemParent, object target, DragOperations operation, int position)
//        {
//            OnDragDrop(item, itemParent, (T)target, operation, position);
//        }
//
//        public ContextMenuStrip GetContextMenu(TreeNode node, object nodeData)
//        {
//            return GetContextMenu(node, (T)nodeData);
//        }
//
//        public bool CanRemove(object parentNodeData, object nodeData)
//        {
//            return CanRemove(parentNodeData, (T)nodeData);
//        }
//
//        public bool RemoveNodeData(object parentNodeData, object nodeData)
//        {
//            return RemoveNodeData(parentNodeData, (T)nodeData);
//        }
//
//        /// <summary>
//        /// Typed implementation of method <see cref="ITreeNodePresenter.UpdateNode"/>.
//        /// </summary>
//        /// <seealso cref="UpdateNode(Core.Common.Controls.TreeView.TreeNode, Core.Common.Controls.TreeView.TreeNode, object)"/>
//        protected abstract void UpdateNode(TreeNode parentNode, TreeNode node, T nodeData);
//
//        /// <summary>
//        /// Typed implementation of method <see cref="ITreeNodePresenter.GetChildNodeObjects"/>.
//        /// </summary>
//        /// <seealso cref="GetChildNodeObjects"/>
//        protected virtual IEnumerable GetChildNodeObjects(T nodeData)
//        {
//            return Enumerable.Empty<object>();
//        }
//
//        /// <summary>
//        /// Typed implementation of method <see cref="ITreeNodePresenter.OnNodeRenamed"/>.
//        /// </summary>
//        /// <seealso cref="OnNodeRenamed(object, string)"/>
//        protected virtual void OnNodeRenamed(T nodeData, string newName)
//        {
//            throw new InvalidOperationException(string.Format(Resources.RingtoetsNodePresenterBase_OnNodeRenamed_Cannot_rename_tree_node_of_type_0_, GetType().Name));
//        }
//
//        /// <summary>
//        /// Typed implementation of method <see cref="ITreeNodePresenter.CanDrag"/>.
//        /// </summary>
//        /// <seealso cref="CanDrag(object)"/>
//        protected virtual DragOperations CanDrag(T nodeData)
//        {
//            return DragOperations.None;
//        }
//
//        /// <summary>
//        /// Typed implementation of method <see cref="ITreeNodePresenter.OnDragDrop"/>.
//        /// </summary>
//        /// <seealso cref="OnDragDrop(object, object, object, DragOperations, System.Int32)"/>
//        protected virtual void OnDragDrop(object item, object itemParent, T target, DragOperations operation, int position)
//        {
//            // Do nothing
//        }
//
//        /// <summary>
//        /// Typed implementation of method <see cref="ITreeNodePresenter.GetContextMenu"/>.
//        /// </summary>
//        /// <seealso cref="GetContextMenu(TreeNode, object)"/>
//        protected virtual ContextMenuStrip GetContextMenu(TreeNode node, T nodeData)
//        {
//            return null;
//        }
//
//        /// <summary>
//        /// Typed implementation of method <see cref="ITreeNodePresenter.CanRemove"/>.
//        /// </summary>
//        /// <seealso cref="CanRemove(object, object)"/>
//        protected virtual bool CanRemove(object parentNodeData, T nodeData)
//        {
//            return false;
//        }
//
//        /// <summary>
//        /// Typed implementation of method <see cref="ITreeNodePresenter.RemoveNodeData"/>.
//        /// </summary>
//        /// <seealso cref="RemoveNodeData(object, object)"/>
//        protected virtual bool RemoveNodeData(object parentNodeData, T nodeData)
//        {
//            throw new InvalidOperationException(String.Format(Resources.RingtoetsNodePresenterBase_RemoveNodeData_Cannot_delete_node_of_type_0_, GetType().Name));
//        }
//    }
//}