using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Gui.Properties;
using TreeNode = Core.Common.Controls.TreeView.TreeNode;

namespace Core.Common.Gui.ContextMenu
{
    /// <summary>
    /// This class represents a factory for creating <see cref="ToolStripItem"/>. The
    /// items the factory creates are dependent on a <see cref="Controls.TreeView.TreeView"/> set for
    /// the <see cref="TreeNode"/>.
    /// </summary>
    internal class TreeViewContextMenuItemFactory
    {
        private readonly TreeNode treeNode;

        /// <summary>
        /// Creates a new instance of <see cref="TreeViewContextMenuItemFactory"/> for the given <paramref name="treeNode"/>.
        /// </summary>
        /// <param name="treeNode">The <see cref="TreeNode"/> for which to create <see cref="ToolStripItem"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="treeNode"/> is <c>null</c>.</exception>
        public TreeViewContextMenuItemFactory(TreeNode treeNode)
        {
            if (treeNode == null)
            {
                throw new ArgumentNullException("treeNode", Resources.ContextMenuItemFactory_Can_not_create_context_menu_items_without_tree_node);
            }
            this.treeNode = treeNode;
        }

        /// <summary>
        /// Creates a <see cref="ToolStripItem"/> which is bound to the action of renaming
        /// the <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The created <see cref="ToolStripItem"/>.</returns>
        public ToolStripItem CreateRenameItem()
        {
            var toolStripMenuItem = new ToolStripMenuItem(Resources.Rename)
            {
                ToolTipText = Resources.Rename_ToolTip,
                Image = Resources.RenameIcon,
                Enabled = treeNode.Presenter.CanRenameNode(treeNode)
            };
            toolStripMenuItem.Click += (s, e) => treeNode.TreeView.StartLabelEdit(treeNode);
            return toolStripMenuItem;
        }

        /// <summary>
        /// Creates a <see cref="ToolStripItem"/> which is bound to the action of deleting
        /// the <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The created <see cref="ToolStripItem"/>.</returns>
        public ToolStripItem CreateDeleteItem()
        {
            var toolStripMenuItem = new ToolStripMenuItem(Resources.Delete)
            {
                ToolTipText = Resources.Delete_ToolTip,
                Image = Resources.DeleteIcon,
                Enabled = treeNode.Presenter.CanRemove(treeNode.Parent.Tag, treeNode.Tag)
            };
            toolStripMenuItem.Click += (s, e) => treeNode.TreeView.TryDeleteNodeData(treeNode);
            return toolStripMenuItem;
        }

        /// <summary>
        /// Creates a <see cref="ToolStripItem"/> which is bound to the action of expanding
        /// the <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The created <see cref="ToolStripItem"/>.</returns>
        public ToolStripItem CreateExpandAllItem()
        {
            IList<TreeNode> children = treeNode.Nodes;
            var toolStripMenuItem = new ToolStripMenuItem(Resources.Expand_all)
            {
                ToolTipText = Resources.Expand_all_ToolTip,
                Image = Resources.ExpandAllIcon,
                Enabled = children != null && children.Any()
            };
            toolStripMenuItem.Click += (s, e) => treeNode.TreeView.ExpandAll(treeNode);
            return toolStripMenuItem;
        }

        /// <summary>
        /// Creates a <see cref="ToolStripItem"/> which is bound to the action of collapsing
        /// the <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The created <see cref="ToolStripItem"/>.</returns>
        public ToolStripItem CreateCollapseAllItem()
        {
            IList<TreeNode> children = treeNode.Nodes;
            var toolStripMenuItem = new ToolStripMenuItem(Resources.Collapse_all)
            {
                ToolTipText = Resources.Collapse_all_ToolTip,
                Image = Resources.CollapseAllIcon,
                Enabled = children != null && children.Any()
            };
            toolStripMenuItem.Click += (s, e) => treeNode.TreeView.CollapseAll(treeNode);
            return toolStripMenuItem;
        }
    }
}