using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DelftTools.Utils;
using DelftTools.Utils.Collections;

namespace DelftTools.Controls.Swf.TreeViewControls
{
    public abstract class TreeViewNodePresenterBase<T> : ITreeNodePresenter
    {
        public ITreeView TreeView { get; set; }

        public virtual Type NodeTagType
        {
            get
            {
                return typeof(T);
            }
        }

        public abstract void UpdateNode(ITreeNode parentNode, ITreeNode node, T nodeData);

        public virtual IEnumerable GetChildNodeObjects(T parentNodeData, ITreeNode node)
        {
            return new object[0];
        }

        /// <seealso cref="ITreeNodePresenter.OnNodeRenamed"/>
        /// <exception cref="InvalidOperationException">This method should be overridden for <paramref name="data"/> that do not inherit from <see cref="INameable"/>.</exception>
        public virtual void OnNodeRenamed(T data, string newName)
        {
            if (data is INameable)
            {
                var nameable = (INameable) data;
                if (nameable.Name != newName)
                {
                    nameable.Name = newName;
                }
            }
            else
            {
                throw new InvalidOperationException("OnNodeRenamed must be implemented in derived class");
            }
        }

        ///<summary>
        /// Returns the default drag operation based on pressed control keys and allowed operations
        /// It can be used as replacement for the implementation for TreeViewNodePresenterBase&lt;T&gt;::<see cref="TreeViewNodePresenterBase{T}.CanDrop"/>.
        ///</summary>
        public static DragOperations GetDefaultDropOperation(ITreeView treeView, object item, ITreeNode sourceNode, ITreeNode targetNode, DragOperations validOperations)
        {
            if (null != sourceNode)
            {
                //TreeNode nodeDragging = (TreeNode)sourceNode;
                if (sourceNode.TreeView == treeView)
                {
                    // if the user is not forcing an operation using the keyboard default to move within treeview
                    if ((0 == (Control.ModifierKeys & Keys.Modifiers)) && (DragOperations.Move == (validOperations & DragOperations.Move)))
                    {
                        return DragOperations.Move;
                    }
                }
            }

            if (((Control.ModifierKeys & Keys.Shift) == Keys.Shift) && (DragOperations.Move == (validOperations & DragOperations.Move)))
            {
                return DragOperations.Move;
            }

            // do not return bitwise operation, preference here is disputable
            if (DragOperations.Move == (DragOperations.Move & validOperations))
            {
                return DragOperations.Move;
            }

            return DragOperations.None;
        }

        public virtual void OnDragDrop(object item, object sourceParentNodeData, T target,
                                       DragOperations operation, int position)
        {
            throw new NotImplementedException();
        }

        public virtual DragOperations CanDrag(T nodeData)
        {
            return DragOperations.None;
        }

        public void UpdateNode(ITreeNode parentNode, ITreeNode node, object nodeData)
        {
            T data = (T) nodeData;
            UpdateNode(parentNode, node, data);
        }

        IEnumerable ITreeNodePresenter.GetChildNodeObjects(object parentNodeData, ITreeNode node)
        {
            return GetChildNodeObjects((T) parentNodeData, node);
        }

        /// <returns>Will return false.</returns>
        public virtual bool CanRenameNode(ITreeNode node)
        {
            return false;
        }

        public virtual bool CanRenameNodeTo(ITreeNode node, string newName)
        {
            // default behavior is not to allow empty strings
            return CanRenameNode(node) && (newName.Length > 0);
        }

        /// <exception cref="InvalidOperationException">This method should be overridden for <paramref name="nodeData"/> that do not inherit from <see cref="INameable"/>.</exception>
        public void OnNodeRenamed(object nodeData, string newName)
        {
            var data = (T) nodeData;
            OnNodeRenamed(data, newName);
        }

        public virtual void OnNodeChecked(ITreeNode node)
        {
            if (!node.IsLoaded)
            {
                return;
            }

            foreach (var childNode in node.Nodes)
            {
                childNode.Checked = node.Checked;
            }
        }

        public virtual bool CanInsert(object item, ITreeNode sourceNode, ITreeNode targetNode)
        {
            return (null == TreeView.TreeViewNodeSorter);
        }

        ///<summary>
        /// Let the node decide what dragoperation should be carried out
        ///</summary>
        public virtual DragOperations CanDrop(object item, ITreeNode sourceNode, ITreeNode targetNode, DragOperations validOperations)
        {
            return DragOperations.None;
        }

        void ITreeNodePresenter.OnDragDrop(object item, object sourceParentNodeData, object targetParentNodeData,
                                           DragOperations operation, int position)
        {
            OnDragDrop(item, sourceParentNodeData, (T) targetParentNodeData, operation, position);
        }

        public bool CanRemove(object parentNodeData, object nodeData)
        {
            T data = (T) nodeData;
            return CanRemove(data);
        }

        public void OnNodeSelected(object nodeData)
        {
            throw new NotImplementedException();
        }

        public virtual IMenuItem GetContextMenu(ITreeNode sender, object nodeData)
        {
            return null;
        }

        public void OnPropertyChanged(object sender, ITreeNode node, PropertyChangedEventArgs e)
        {
            if (sender is T) // sometimes events are coming from child objects
            {
                OnPropertyChanged((T) sender, node, e);
            }
            else if (node != null)
            {
                node.Update(); // full refresh node
            }
        }

        public void OnCollectionChanged(object sender, NotifyCollectionChangingEventArgs e)
        {
            // TODO: should this code be moved into tree view implementaiton?

            switch (e.Action)
            {
                case NotifyCollectionChangeAction.Add:
                    // find first parent node where node must be added for e.Item (only 1 parent supported for now)
                    foreach (ITreeNode parentNode in TreeView.AllLoadedNodes)
                    {
                        ITreeNodePresenter presenter = parentNode.Presenter;
                        if (presenter != null)
                        {
                            var indexOf = IndexInParent(presenter, parentNode, e.Item);
                            if (indexOf >= 0)
                            {
                                ((TreeNode) parentNode).HasChildren = presenter.GetChildNodeObjects(parentNode.Tag, parentNode).GetEnumerator().MoveNext();
                                OnCollectionChanged((T) e.Item, parentNode, e, indexOf);
                                return; // only one Node per tag is supported for now
                            }
                        }
                    }
                    break;

                case NotifyCollectionChangeAction.Remove:
                {
                    ITreeNode node = TreeView.GetNodeByTag(e.Item);
                    if (node != null)
                    {
                        ITreeNode parentNode = node.Parent;
                        if (parentNode != null)
                        {
                            var parentPresenter = parentNode.Presenter;
                            if (parentPresenter != null)
                            {
                                //if it was removed from a collection our parent is not based on, we don't want
                                //to remove it from the nodes list
                                if (IndexInParent(parentPresenter, parentNode, e.Item) >= 0)
                                {
                                    return;
                                }
                                ((TreeNode) parentNode).HasChildren = parentPresenter.GetChildNodeObjects(parentNode.Tag, parentNode).GetEnumerator().MoveNext();
                                OnCollectionChanged((T) e.Item, parentNode, e, -1);
                            }
                        }
                    }
                }
                    break;

                case NotifyCollectionChangeAction.Replace:
                    break;
            }
        }

        public bool RemoveNodeData(object parentNodeData, object nodeData)
        {
            T data = (T) nodeData;
            return RemoveNodeData(parentNodeData, data);
        }

        public DragOperations CanDrag(object nodeData)
        {
            T data = (T) nodeData;
            return CanDrag(data);
        }

        /// <summary>
        /// Specifies if node can be deleted by user
        /// </summary>
        protected virtual bool CanRemove(T nodeData)
        {
            return false;
        }

        protected virtual void OnPropertyChanged(T item, ITreeNode node, PropertyChangedEventArgs e)
        {
            if (node == null)
            {
                return;
            }
            UpdateNode(node.Parent, node, item);
        }

        /// <summary>
        /// Implements generic way to deal with collection changes such as adding and removing.
        /// 
        /// Override if you want custom implementation
        /// </summary>
        /// <param name="childNodeData">Item which was changed (equals to e.Item)</param>
        /// <param name="parentNode"></param>
        /// <param name="e"></param>
        /// <param name="newNodeIndex"></param>
        protected virtual void OnCollectionChanged(T childNodeData, ITreeNode parentNode, NotifyCollectionChangingEventArgs e, int newNodeIndex)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangeAction.Replace:
                    throw new NotImplementedException();

                case NotifyCollectionChangeAction.Add:
                    if (parentNode.IsLoaded)
                    {
                        // do not add node twice to the same parent
                        ITreeNode existingNode = parentNode.GetNodeByTag(e.Item);
                        if (existingNode != null)
                        {
                            return;
                        }

                        // create and add a new tree node
                        var newNode = TreeView.AddNewNode(parentNode, e.Item, newNodeIndex);
                        newNode.ContextMenu = GetContextMenu(null, e.Item);
                    }
                    else
                    {
                        parentNode.Update(); // will load child nodes if needed
                    }

                    parentNode.Expand(); // makes sure node is expanded and all children nodes are loaded
                    break;

                case NotifyCollectionChangeAction.Remove:
                    ITreeNode node = TreeView.GetNodeByTag(e.Item);
                    if (node != null)
                    {
                        TreeView.Nodes.Remove(node);
                    }
                    break;
            }
        }

        protected virtual bool RemoveNodeData(object parentNodeData, T nodeData)
        {
            return false;
        }

        private static int IndexInParent(ITreeNodePresenter presenter, ITreeNode parentNode, object item)
        {
            var childItems = presenter.GetChildNodeObjects(parentNode.Tag, parentNode).OfType<object>().ToList();
            var i = 0;
            foreach (var childItem in childItems)
            {
                if (ReferenceEquals(childItem, item))
                {
                    return i;
                }
                i++;
            }
            return -1;
        }
    }
}