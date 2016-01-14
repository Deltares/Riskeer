using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;

namespace Core.Common.Controls.TreeView
{
    public class TreeNode : System.Windows.Forms.TreeNode, IObserver
    {
        private readonly TreeNodeList nodes;
        private readonly TreeView treeView;
        private bool isLoaded;
        private object tag;
        private IObservable observable;
        private readonly int maximumTextLength = 259; // Having very big strings causes problems by tree-view

        public TreeNode(TreeView treeView)
        {
            this.treeView = treeView;
            nodes = new TreeNodeList(base.Nodes);
            IsVisible = true;
        }

        /// <summary>
        /// Called when a user right clicks in the network tree
        /// </summary>
        public override ContextMenuStrip ContextMenuStrip
        {
            get
            {
                return Presenter != null ? Presenter.GetContextMenu(this, Tag) : null;
            }
        }

        /// <summary>
        /// Used in rendering (has children indicates if a plus or minus must be drawn)
        /// </summary>
        public bool HasChildren { get; set; }

        public new TreeView TreeView
        {
            get
            {
                return treeView;
            }
        }

        public ITreeNodePresenter Presenter { get; set; }

        public new string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                if (base.Text == value)
                {
                    return;
                }

                if (string.IsNullOrEmpty(value))
                {
                    base.Text = "";

                    return;
                }

                base.Text = value.Length > maximumTextLength ? value.Substring(0, maximumTextLength) : value;
            }
        }

        public new object Tag
        {
            get
            {
                return tag;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                if (tag == value)
                {
                    return;
                }

                if (observable != null)
                {
                    observable.Detach(this);
                }

                tag = value;

                observable = tag as IObservable;
                if (observable != null)
                {
                    observable.Attach(this);
                }
            }
        }

        public new bool IsVisible { get; set; }

        public new TreeNode Parent
        {
            get
            {
                return (TreeNode) base.Parent;
            }
        }

        public new string FullPath
        {
            get
            {
                return base.TreeView == null ? string.Empty : base.FullPath;
            }
        }

        public new TreeNode NextNode
        {
            get
            {
                return (TreeNode) base.NextNode;
            }
        }

        public new TreeNode NextVisibleNode
        {
            get
            {
                return (TreeNode) base.NextVisibleNode;
            }
        }

        public new IList<TreeNode> Nodes
        {
            get
            {
                if (!isLoaded)
                {
                    RefreshChildNodes();
                }

                return nodes;
            }
        }

        public TreeNode PreviousNode
        {
            get
            {
                return (TreeNode) PrevNode;
            }
        }

        public TreeNode PreviousVisibleNode
        {
            get
            {
                return (TreeNode) PrevVisibleNode;
            }
        }

        public bool IsLoaded
        {
            get
            {
                return isLoaded;
            }
        }

        public bool ShowCheckBox { get; set; }

        public Color ForegroundColor
        {
            get
            {
                return ForeColor;
            }
            set
            {
                ForeColor = value;
            }
        }

        public Image Image { get; set; }
        public bool IsUpdating { get; private set; }

        private void RefreshChildNodes(bool forcedRefresh = false)
        {
            if (IsUpdating && !forcedRefresh)
            {
                return; //prevent 're-entrancy' issues
            }

            IsUpdating = true;
            TreeView.RefreshChildNodes(this);
            isLoaded = true;
            IsUpdating = false;
        }

        /// <summary>
        /// Check if the node is a descendent of another node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool IsChildOf(TreeNode node)
        {
            TreeNode parentNode = this;
            while (parentNode != null && parentNode.Parent != null)
            {
                if (parentNode.Parent.Equals(node))
                {
                    return true;
                }

                parentNode = parentNode.Parent;
            }

            return false;
        }

        public void UpdateObserver()
        {
            if (IsUpdating)
            {
                return; //prevent 're-entrancy' issues
            }

            IsUpdating = true;
            TreeView.UpdateNode(this);
            IsUpdating = false;
        }

        public new void Expand()
        {
            if (!isLoaded)
            {
                TreeView.RefreshChildNodes(this);
            }

            base.Expand();
        }

        public TreeNode GetParentOfLevel(int level)
        {
            TreeNode node = this;

            for (var i = Level; i != level; i--)
            {
                node = node.Parent;
            }

            return node;
        }

        public void ScrollTo()
        {
            EnsureVisible();
        }

        public TreeNode GetNodeByTag(object item)
        {
            return nodes.FirstOrDefault(node => node.Tag == item);
        }

        public void ShowContextMenu(Point location)
        {
            if (base.ContextMenuStrip != null)
            {
                base.ContextMenuStrip.Show(location);
            }
        }

        public void Dispose()
        {
            if (observable != null)
            {
                observable.Detach(this);
            }
        }
    }
}