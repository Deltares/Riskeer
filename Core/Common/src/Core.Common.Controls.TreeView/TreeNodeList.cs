using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using Core.Common.Controls.TreeView.Properties;

namespace Core.Common.Controls.TreeView
{
    /// <summary>
    /// Wraps TreeNodeCollection from Windows.Forms.TreeView so it appears as a list of ITreeNode.
    /// </summary>
    public class TreeNodeList : IList<ITreeNode>
    {
        private readonly TreeNodeCollection nodes;

        public TreeNodeList(TreeNodeCollection nodes)
        {
            this.nodes = nodes;
        }

        #region IList<ITreeNode> Members

        public int IndexOf(ITreeNode item)
        {
            return nodes.IndexOf((System.Windows.Forms.TreeNode) item);
        }

        public void Insert(int index, ITreeNode item)
        {
            var treeNodeItem = item as TreeNode;

            if (treeNodeItem != null)
            {
                nodes.Insert(index, treeNodeItem);
            }
        }

        public void RemoveAt(int index)
        {
            var nodeToRemove = (ITreeNode) nodes[index];

            nodes.RemoveAt(index);

            nodeToRemove.Dispose();
        }

        public ITreeNode this[int index]
        {
            get
            {
                return (ITreeNode) nodes[index];
            }
            set
            {
                nodes[index] = (System.Windows.Forms.TreeNode) value;
            }
        }

        public void Add(ITreeNode item)
        {
            var treeNodeItem = item as TreeNode;

            if (treeNodeItem != null)
            {
                nodes.Add(treeNodeItem);
            }
        }

        /// <summary>
        /// Removes all nodes from the collection
        /// </summary>
        public void Clear()
        {
            var nodesToRemove = new List<ITreeNode>();

            foreach (ITreeNode node in nodes)
            {
                nodesToRemove.AddRange(GetAllNodes(node));
            }

            nodes.Clear();

            foreach (var removedNode in nodesToRemove)
            {
                removedNode.Dispose();
            }
        }

        private IList<ITreeNode> GetAllNodes(ITreeNode treeNode)
        {
            var thisAndChildren = new List<ITreeNode>();
            thisAndChildren.Add(treeNode);
            foreach (var node in treeNode.Nodes)
            {
                thisAndChildren.AddRange(GetAllNodes(node));
            }
            return thisAndChildren;
        }

        public bool Contains(ITreeNode item)
        {
            return nodes.Contains((System.Windows.Forms.TreeNode) item);
        }

        public void CopyTo(ITreeNode[] array, int arrayIndex)
        {
            nodes.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                return nodes.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return nodes.IsReadOnly;
            }
        }

        /// <summary>
        /// Remove the treenode and update event subscriptions.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(ITreeNode item)
        {
            nodes.Remove((System.Windows.Forms.TreeNode) item);

            item.Dispose();

            return true;
        }

        public IEnumerator<ITreeNode> GetEnumerator()
        {
            //wrap TreeNodeCollection so it returns  the right enumerator variable
            foreach (ITreeNode node in nodes)
            {
                yield return node;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException(Resources.TreeNodeList_GetEnumerator_The_method_or_operation_is_not_implemented);
        }

        #endregion
    }
}