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
        /// <summary>
        /// Creates a <see cref="ToolStripItem"/> which is bound to the action of expanding
        /// the <paramref name="treeNode"/>.
        /// </summary>
        /// <param name="treeNode">The <see cref="ITreeNode"/> for which to create the <see cref="ToolStripItem"/>
        /// and which to expand if clicked.</param>
        /// <returns>The created <see cref="ToolStripItem"/>.</returns>
        public ToolStripItem CreateExpandAllItem(ITreeNode treeNode)
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
        /// the <paramref name="treeNode"/>.
        /// </summary>
        /// <param name="treeNode">The <see cref="ITreeNode"/> for which to create the <see cref="ToolStripItem"/>
        /// and which to collapse if clicked.</param>
        /// <returns>The created <see cref="ToolStripItem"/>.</returns>
        public ToolStripItem CreateCollapseAllItem(ITreeNode treeNode)
        {
            var toolStripMenuItem = new ToolStripMenuItem(Resources.Collapse_all)
            {
                ToolTipText = Resources.Collapse_all_ToolTip,
                Image = Resources.CollapseAllIcon
            };
            toolStripMenuItem.Click += (s, e) => treeNode.TreeView.CollapseAll(treeNode);
            return toolStripMenuItem;
        }
    }
}