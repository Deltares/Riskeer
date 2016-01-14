using System;
using System.Collections;
using System.Windows.Forms;
using Core.Common.Controls.TreeView.Properties;

namespace Core.Common.Controls.TreeView
{
    public abstract class TreeViewNodePresenterBase<T> : ITreeNodePresenter
    {
        public TreeView TreeView { get; set; }

        public virtual Type NodeTagType
        {
            get
            {
                return typeof(T);
            }
        }

        public abstract void UpdateNode(TreeNode parentNode, TreeNode node, T nodeData);

        public virtual IEnumerable GetChildNodeObjects(T parentNodeData)
        {
            return new object[0];
        }

        /// <seealso cref="ITreeNodePresenter.OnNodeRenamed"/>
        /// <exception cref="InvalidOperationException">This method should be overridden for <paramref name="data"/>.</exception>
        public virtual void OnNodeRenamed(T data, string newName)
        {
            throw new InvalidOperationException(Resources.TreeViewNodePresenterBase_OnNodeRenamed_OnNodeRenamed_must_be_implemented_in_derived_class);
        }

        protected static DragOperations GetDefaultDropOperation(DragOperations validOperation)
        {
            return DragOperations.Move == validOperation ? DragOperations.Move : DragOperations.None;
        }

        public virtual void OnDragDrop(object item, object itemParent, T target,
                                       DragOperations operation, int position)
        {
            throw new NotImplementedException();
        }

        public virtual DragOperations CanDrag(T nodeData)
        {
            return DragOperations.None;
        }

        public void UpdateNode(TreeNode parentNode, TreeNode node, object nodeData)
        {
            T data = (T) nodeData;
            UpdateNode(parentNode, node, data);
        }

        IEnumerable ITreeNodePresenter.GetChildNodeObjects(object parentNodeData)
        {
            return GetChildNodeObjects((T) parentNodeData);
        }

        /// <returns>Will return false.</returns>
        public virtual bool CanRenameNode(TreeNode node)
        {
            return false;
        }

        public virtual bool CanRenameNodeTo(TreeNode node, string newName)
        {
            // default behavior is not to allow empty strings
            return CanRenameNode(node) && (newName.Length > 0);
        }

        /// <exception cref="InvalidOperationException">This method should be overridden for <paramref name="nodeData"/>.</exception>
        public void OnNodeRenamed(object nodeData, string newName)
        {
            var data = (T) nodeData;
            OnNodeRenamed(data, newName);
        }

        public virtual void OnNodeChecked(TreeNode node)
        {
            foreach (var childNode in node.Nodes)
            {
                childNode.Checked = node.Checked;
            }
        }

        public virtual bool CanInsert(object item, TreeNode sourceNode, TreeNode targetNode)
        {
            return false;
        }

        /// <summary>
        /// Let the node decide what dragoperation should be carried out
        /// </summary>
        public virtual DragOperations CanDrop(object item, TreeNode sourceNode, TreeNode targetNode, DragOperations validOperations)
        {
            return DragOperations.None;
        }

        void ITreeNodePresenter.OnDragDrop(object item, object itemParent, object target,
                                           DragOperations operation, int position)
        {
            OnDragDrop(item, itemParent, (T) target, operation, position);
        }

        public bool CanRemove(object parentNodeData, object nodeData)
        {
            T data = (T) nodeData;
            return CanRemove(data);
        }

        public virtual ContextMenuStrip GetContextMenu(TreeNode node, object nodeData)
        {
            return null;
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

        protected virtual bool RemoveNodeData(object parentNodeData, T nodeData)
        {
            return false;
        }
    }
}