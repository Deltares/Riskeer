using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using Core.Common.Controls.TreeView.Properties;

namespace Core.Common.Controls.TreeView
{
    /// <summary>
    /// Wraps TreeNodeCollection from Windows.Forms.TreeView so it appears as a list of TreeNode.
    /// </summary>
    public class TreeNodeList : IList<TreeNode>
    {
        private readonly TreeNodeCollection nodes;

        public TreeNodeList(TreeNodeCollection nodes)
        {
            this.nodes = nodes;
        }

        #region IList<TreeNode> Members

        public int IndexOf(TreeNode item)
        {
            return nodes.IndexOf(item);
        }

        public void Insert(int index, TreeNode item)
        {
            if (item != null)
            {
                nodes.Insert(index, item);
            }
        }

        public void RemoveAt(int index)
        {
            var nodeToRemove = (TreeNode) nodes[index];

            nodes.RemoveAt(index);

            nodeToRemove.Dispose();
        }

        public TreeNode this[int index]
        {
            get
            {
                return (TreeNode) nodes[index];
            }
            set
            {
                nodes[index] = value;
            }
        }

        public void Add(TreeNode item)
        {
            if (item != null)
            {
                nodes.Add(item);
            }
        }

        /// <summary>
        /// Removes all nodes from the collection
        /// </summary>
        public void Clear()
        {
            var nodesToRemove = new List<TreeNode>();

            foreach (TreeNode node in nodes)
            {
                nodesToRemove.AddRange(GetAllNodes(node));
            }

            nodes.Clear();

            foreach (var removedNode in nodesToRemove)
            {
                removedNode.Dispose();
            }
        }

        private IList<TreeNode> GetAllNodes(TreeNode treeNode)
        {
            var thisAndChildren = new List<TreeNode>();
            thisAndChildren.Add(treeNode);
            foreach (var node in treeNode.Nodes)
            {
                thisAndChildren.AddRange(GetAllNodes(node));
            }
            return thisAndChildren;
        }

        public bool Contains(TreeNode item)
        {
            return nodes.Contains(item);
        }

        public void CopyTo(TreeNode[] array, int arrayIndex)
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
        public bool Remove(TreeNode item)
        {
            nodes.Remove(item);

            item.Dispose();

            return true;
        }

        public IEnumerator<TreeNode> GetEnumerator()
        {
            //wrap TreeNodeCollection so it returns  the right enumerator variable
            foreach (TreeNode node in nodes)
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