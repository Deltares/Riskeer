using System;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Gui.Properties;

namespace Core.Common.Gui.ContextMenu
{
    /// <summary>
    /// This class represents a factory for creating <see cref="ToolStripItem"/>. The
    /// items the factory creates are dependent on a <see cref="ITreeView"/> set for
    /// the <see cref="ITreeNode"/>.
    /// </summary>
    internal class TreeViewContextMenuItemFactory
    {
        private readonly ITreeNode treeNode;

        /// <summary>
        /// Creates a new instance of <see cref="TreeViewContextMenuItemFactory"/> for the given <paramref name="treeNode"/>.
        /// </summary>
        /// <param name="treeNode">The <see cref="ITreeNode"/> for which to create <see cref="ToolStripItem"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="treeNode"/> is <c>null</c>.</exception>
        public TreeViewContextMenuItemFactory(ITreeNode treeNode)
        {
            if (treeNode == null)
            {
                throw new ArgumentNullException("treeNode", Resources.ContextMenuItemFactory_Can_not_create_context_menu_items_without_tree_node);
            }
            this.treeNode = treeNode;
        }

        /// <summary>
        /// Creates a <see cref="ToolStripItem"/> which is bound to the action of expanding
        /// the <see name="ITreeNode"/>.
        /// </summary>
        /// <returns>The created <see cref="ToolStripItem"/>.</returns>
        public ToolStripItem CreateExpandAllItem()
        {
            var toolStripMenuItem = new ToolStripMenuItem(Resources.Expand_all)
            {
                ToolTipText = Resources.Expand_all_ToolTip,
                Image = Resources.ExpandAllIcon
            };
            toolStripMenuItem.Click += (s, e) => treeNode.TreeView.ExpandAll(treeNode);
            return toolStripMenuItem;
        }

        /// <summary>
        /// Creates a <see cref="ToolStripItem"/> which is bound to the action of collapsing
        /// the <see name="ITreeNode"/>.
        /// </summary>
        /// <returns>The created <see cref="ToolStripItem"/>.</returns>
        public ToolStripItem CreateCollapseAllItem()
        {
            var toolStripMenuItem = new ToolStripMenuItem(Resources.Collapse_all)
            {
                ToolTipText = Resources.Collapse_all_ToolTip,
                Image = Resources.CollapseAllIcon
            };
            toolStripMenuItem.Click += (s, e) => treeNode.TreeView.CollapseAll(treeNode);
            return toolStripMenuItem;
        }

        /// <summary>
        /// Creates a <see cref="ToolStripItem"/> which is bound to the action of deleting
        /// the current <see name="ITreeNode"/>.
        /// </summary>
        /// <returns>The created <see cref="ToolStripItem"/>.</returns>
        public ToolStripItem CreateDeleteItem()
        {
            var toolStripMenuItem = new ToolStripMenuItem(Resources.Delete)
            {
                ToolTipText = Resources.Delete_ToolTip,
                Image = Resources.DeleteIcon
            };
            toolStripMenuItem.Click += (s, e) => treeNode.TreeView.TryDeleteSelectedNodeData();
            return toolStripMenuItem;
        }
    }
}