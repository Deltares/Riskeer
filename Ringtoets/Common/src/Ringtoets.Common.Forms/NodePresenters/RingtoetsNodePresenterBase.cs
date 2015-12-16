using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

using Core.Common.Controls;
using Core.Common.Controls.Swf.TreeViewControls;
using Core.Common.Gui;
using Core.Common.Utils.Collections;

using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.NodePresenters
{
    /// <summary>
    /// Implements <see cref="ITreeNodePresenter"/> in a featureless way as possible,
    /// to serve as a base class for all node presenters.
    /// </summary>
    /// <typeparam name="T">The data object class corresponding with the node.</typeparam>
    public abstract class RingtoetsNodePresenterBase<T> : ITreeNodePresenter
    {
        protected readonly IContextMenuBuilderProvider contextMenuBuilderProvider;

        /// <summary>
        /// Creates a new instance of <see cref="RingtoetsNodePresenterBase{T}"/>, which uses the 
        /// <paramref name="contextMenuBuilderProvider"/> to create and bind its <see cref="ContextMenuStrip"/>.
        /// </summary>
        /// <param name="contextMenuBuilderProvider">The <see cref="IContextMenuBuilderProvider"/> 
        /// to use for  building a <see cref="ContextMenuStrip"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when no <paramref name="contextMenuBuilderProvider"/> was provided.</exception>
        public RingtoetsNodePresenterBase(IContextMenuBuilderProvider contextMenuBuilderProvider)
        {
            if (contextMenuBuilderProvider == null)
            {
                throw new ArgumentNullException("contextMenuBuilderProvider", Core.Common.Gui.Properties.Resources.NodePresenter_ContextMenuBuilderProvider_required);
            }
            this.contextMenuBuilderProvider = contextMenuBuilderProvider;
        }

        public ITreeView TreeView { get; set; }

        public Type NodeTagType
        {
            get
            {
                return typeof(T);
            }
        }

        public void UpdateNode(ITreeNode parentNode, ITreeNode node, object nodeData)
        {
            UpdateNode(parentNode, node, (T)nodeData);
        }

        public IEnumerable GetChildNodeObjects(object parentNodeData)
        {
            return GetChildNodeObjects((T)parentNodeData).Cast<object>();
        }

        public virtual bool CanRenameNode(ITreeNode node)
        {
            return false;
        }

        public virtual bool CanRenameNodeTo(ITreeNode node, string newName)
        {
            return false;
        }

        public void OnNodeRenamed(object nodeData, string newName)
        {
            var data = (T)nodeData;
            OnNodeRenamed(data, newName);
        }

        public virtual void OnNodeChecked(ITreeNode node)
        {
            // Not a checked node
        }

        public DragOperations CanDrag(object nodeData)
        {
            return CanDrag((T)nodeData);
        }

        public virtual DragOperations CanDrop(object item, ITreeNode sourceNode, ITreeNode targetNode, DragOperations validOperations)
        {
            return DragOperations.None;
        }

        public virtual bool CanInsert(object item, ITreeNode sourceNode, ITreeNode targetNode)
        {
            return false;
        }

        public void OnDragDrop(object item, object itemParent, object target, DragOperations operation, int position)
        {
            OnDragDrop(item, itemParent, (T)target, operation, position);
        }

        public void OnNodeSelected(object nodeData)
        {
            OnNodeSelected((T)nodeData);
        }

        public ContextMenuStrip GetContextMenu(ITreeNode node, object nodeData)
        {
            return GetContextMenu(node, (T)nodeData);
        }

        public void OnPropertyChanged(object sender, ITreeNode node, PropertyChangedEventArgs e)
        {
            OnPropertyChanged((T)sender, node, e);
        }

        public void OnCollectionChanged(object sender, NotifyCollectionChangeEventArgs e)
        {
            OnCollectionChanged((T)sender, e);
        }

        public bool CanRemove(object parentNodeData, object nodeData)
        {
            return CanRemove(parentNodeData, (T)nodeData);
        }

        public bool RemoveNodeData(object parentNodeData, object nodeData)
        {
            return RemoveNodeData(parentNodeData, (T)nodeData);
        }

        /// <summary>
        /// Typed implementation of method <see cref="ITreeNodePresenter.UpdateNode"/>.
        /// </summary>
        /// <seealso cref="UpdateNode(ITreeNode, ITreeNode, object)"/>
        protected abstract void UpdateNode(ITreeNode parentNode, ITreeNode node, T nodeData);

        /// <summary>
        /// Typed implementation of method <see cref="ITreeNodePresenter.GetChildNodeObjects"/>.
        /// </summary>
        /// <seealso cref="GetChildNodeObjects"/>
        protected virtual IEnumerable GetChildNodeObjects(T nodeData)
        {
            return Enumerable.Empty<object>();
        }

        /// <summary>
        /// Typed implementation of method <see cref="ITreeNodePresenter.OnNodeRenamed"/>.
        /// </summary>
        /// <seealso cref="OnNodeRenamed(object, string)"/>
        protected virtual void OnNodeRenamed(T nodeData, string newName)
        {
            throw new InvalidOperationException(string.Format(Resources.RingtoetsNodePresenterBase_OnNodeRenamed_Cannot_rename_tree_node_of_type_0_, GetType().Name));
        }

        /// <summary>
        /// Typed implementation of method <see cref="ITreeNodePresenter.CanDrag"/>.
        /// </summary>
        /// <seealso cref="CanDrag(object)"/>
        protected virtual DragOperations CanDrag(T nodeData)
        {
            return DragOperations.None;
        }

        /// <summary>
        /// Typed implementation of method <see cref="ITreeNodePresenter.OnDragDrop"/>.
        /// </summary>
        /// <seealso cref="OnDragDrop(object, object, object, DragOperations, System.Int32)"/>
        protected virtual void OnDragDrop(object item, object itemParent, T target, DragOperations operation, int position)
        {
            // Do nothing
        }

        /// <summary>
        /// Typed implementation of method <see cref="ITreeNodePresenter.OnNodeSelected"/>.
        /// </summary>
        /// <seealso cref="OnNodeSelected(object)"/>
        protected virtual void OnNodeSelected(T nodeData)
        {
            // Do nothing
        }

        /// <summary>
        /// Typed implementation of method <see cref="ITreeNodePresenter.GetContextMenu"/>.
        /// </summary>
        /// <seealso cref="GetContextMenu(ITreeNode, object)"/>
        protected virtual ContextMenuStrip GetContextMenu(ITreeNode node, T nodeData)
        {
            return null;
        }

        /// <summary>
        /// Typed implementation of method <see cref="ITreeNodePresenter.OnPropertyChanged"/>.
        /// </summary>
        /// <seealso cref="OnPropertyChanged(object, ITreeNode, PropertyChangedEventArgs)"/>
        protected virtual void OnPropertyChanged(T sender, ITreeNode node, PropertyChangedEventArgs e)
        {
            // Do nothing
        }

        /// <summary>
        /// Typed implementation of method <see cref="ITreeNodePresenter.OnCollectionChanged"/>.
        /// </summary>
        /// <seealso cref="OnCollectionChanged(object, NotifyCollectionChangeEventArgs)"/>
        protected virtual void OnCollectionChanged(T sender, NotifyCollectionChangeEventArgs e)
        {
            // Do nothing
        }

        /// <summary>
        /// Typed implementation of method <see cref="ITreeNodePresenter.CanRemove"/>.
        /// </summary>
        /// <seealso cref="CanRemove(object, object)"/>
        protected virtual bool CanRemove(object parentNodeData, T nodeData)
        {
            return false;
        }

        /// <summary>
        /// Typed implementation of method <see cref="ITreeNodePresenter.RemoveNodeData"/>.
        /// </summary>
        /// <seealso cref="RemoveNodeData(object, object)"/>
        protected virtual bool RemoveNodeData(object parentNodeData, T nodeData)
        {
            throw new InvalidOperationException(String.Format(Resources.RingtoetsNodePresenterBase_RemoveNodeData_Cannot_delete_node_of_type_0_, GetType().Name));
        }
    }
}