using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DelftTools.Controls;
using DelftTools.Utils.Collections;

namespace Wti.Forms.NodePresenters
{
    /// <summary>
    /// Implements <see cref="ITreeNodePresenter"/> in a featureless way as possible,
    /// to serve as a base class for all node presenters.
    /// </summary>
    /// <typeparam name="T">The data object class corresponding with the node.</typeparam>
    public abstract class PipingNodePresenterBase<T> : ITreeNodePresenter
    {
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

        public IEnumerable GetChildNodeObjects(object parentNodeData, ITreeNode node)
        {
            return GetChildNodeObjects((T)parentNodeData, node).Cast<object>();
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

        public DragOperations CanDrop(object item, ITreeNode sourceNode, ITreeNode targetNode, DragOperations validOperations)
        {
            return CanDrop((T)item, sourceNode, targetNode, validOperations);
        }

        public bool CanInsert(object item, ITreeNode sourceNode, ITreeNode targetNode)
        {
            return CanInsert((T)item, sourceNode, targetNode);
        }

        public void OnDragDrop(object item, object sourceParentNodeData, object targetParentNodeData, DragOperations operation, int position)
        {
            OnDragDrop((T)item, sourceParentNodeData, targetParentNodeData, operation, position);
        }

        public void OnNodeSelected(object nodeData)
        {
            OnNodeSelected((T)nodeData);
        }

        public ContextMenuStrip GetContextMenu(ITreeNode sender, object nodeData)
        {
            return GetContextMenu(sender, (T)nodeData);
        }

        public void OnPropertyChanged(object sender, ITreeNode node, PropertyChangedEventArgs e)
        {
            OnPropertyChanged((T)sender, node, e);
        }

        public void OnCollectionChanged(object sender, NotifyCollectionChangingEventArgs e)
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
        /// Typed implementation method of <see cref="ITreeNodePresenter.UpdateNode"/>.
        /// </summary>
        /// <seealso cref="ITreeNodePresenter.UpdateNode"/>
        protected abstract void UpdateNode(ITreeNode parentNode, ITreeNode node, T nodeData);

        /// <summary>
        /// Typed implementation method of <see cref="ITreeNodePresenter.GetChildNodeObjects"/>.
        /// </summary>
        /// <seealso cref="ITreeNodePresenter.GetChildNodeObjects"/>
        protected virtual IEnumerable GetChildNodeObjects(T nodeData, ITreeNode node)
        {
            return Enumerable.Empty<object>();
        }

        /// <summary>
        /// Typed implementation method of <see cref="ITreeNodePresenter.OnNodeRenamed"/>.
        /// </summary>
        /// <seealso cref="ITreeNodePresenter.OnNodeRenamed"/>
        protected virtual void OnNodeRenamed(T nodeData, string newName)
        {
            throw new InvalidOperationException(string.Format("Cannot rename tree node of type {0}.", GetType().Name));
        }

        /// <summary>
        /// Typed implementation method of <see cref="ITreeNodePresenter.CanDrag"/>.
        /// </summary>
        /// <seealso cref="ITreeNodePresenter.CanDrag"/>
        protected virtual DragOperations CanDrag(T nodeData)
        {
            return DragOperations.None;
        }

        /// <summary>
        /// Typed implementation method of <see cref="ITreeNodePresenter.CanDrop"/>.
        /// </summary>
        /// <seealso cref="ITreeNodePresenter.CanDrop"/>
        protected virtual DragOperations CanDrop(T item, ITreeNode sourceNode, ITreeNode targetNode, DragOperations validOperations)
        {
            return DragOperations.None;
        }

        /// <summary>
        /// Typed implementation method of <see cref="ITreeNodePresenter.CanInsert"/>.
        /// </summary>
        /// <seealso cref="ITreeNodePresenter.CanInsert"/>
        protected virtual bool CanInsert(T item, ITreeNode sourceNode, ITreeNode targetNode)
        {
            return false;
        }

        /// <summary>
        /// Typed implementation method of <see cref="ITreeNodePresenter.OnDragDrop"/>.
        /// </summary>
        /// <seealso cref="ITreeNodePresenter.OnDragDrop"/>
        protected virtual void OnDragDrop(T item, object sourceParentNodeData, object targetParentNodeData, DragOperations operation, int position)
        {
            // Do nothing
        }

        /// <summary>
        /// Typed implementation method of <see cref="ITreeNodePresenter.OnNodeSelected"/>.
        /// </summary>
        /// <seealso cref="ITreeNodePresenter.OnNodeSelected"/>
        protected virtual void OnNodeSelected(T nodeData)
        {
            // Do nothing
        }

        /// <summary>
        /// Typed implementation method of <see cref="ITreeNodePresenter.GetContextMenu"/>.
        /// </summary>
        /// <seealso cref="ITreeNodePresenter.GetContextMenu"/>
        protected virtual ContextMenuStrip GetContextMenu(ITreeNode sender, T nodeData)
        {
            return null;
        }

        /// <summary>
        /// Typed implementation method of <see cref="ITreeNodePresenter.OnPropertyChanged"/>.
        /// </summary>
        /// <seealso cref="ITreeNodePresenter.OnPropertyChanged"/>
        protected virtual void OnPropertyChanged(T sender, ITreeNode node, PropertyChangedEventArgs e)
        {
            // Do nothing
        }

        /// <summary>
        /// Typed implementation method of <see cref="ITreeNodePresenter.OnCollectionChanged"/>.
        /// </summary>
        /// <seealso cref="ITreeNodePresenter.OnCollectionChanged"/>
        protected virtual void OnCollectionChanged(T sender, NotifyCollectionChangingEventArgs e)
        {
            // Do nothing
        }

        /// <summary>
        /// Typed implementation method of <see cref="ITreeNodePresenter.CanRemove"/>.
        /// </summary>
        /// <seealso cref="ITreeNodePresenter.CanRemove"/>
        protected virtual bool CanRemove(object parentNodeData, T nodeData)
        {
            return false;
        }

        /// <summary>
        /// Typed implementation method of <see cref="ITreeNodePresenter.RemoveNodeData"/>.
        /// </summary>
        /// <seealso cref="ITreeNodePresenter.RemoveNodeData"/>
        protected virtual bool RemoveNodeData(object parentNodeData, T nodeData)
        {
            throw new InvalidOperationException(String.Format("Cannot delete node of type {0}.", GetType().Name));
        }
    }
}