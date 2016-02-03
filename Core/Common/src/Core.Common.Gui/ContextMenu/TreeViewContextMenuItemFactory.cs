// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights reserved.

using System;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.Properties;

namespace Core.Common.Gui.ContextMenu
{
    /// <summary>
    /// This class represents a factory for creating <see cref="ToolStripItem"/>.
    /// </summary>
    internal class TreeViewContextMenuItemFactory
    {
        private readonly TreeNode treeNode;
        private readonly TreeNodeInfo treeNodeInfo;
        private readonly TreeViewControl treeViewControl;

        /// <summary>
        /// Creates a new instance of <see cref="TreeViewContextMenuItemFactory"/> for the given <paramref name="treeNode"/>.
        /// </summary>
        /// <param name="treeNode">The <see cref="TreeNode"/> for which to create the <see cref="ToolStripItem"/> objects.</param>
        /// <param name="treeNodeInfo">The <see cref="TreeNodeInfo"/> to use while creating the <see cref="ToolStripItem"/> objects.</param>
        /// <param name="treeViewControl">The <see cref="TreeViewControl"/> to use while executing the <see cref="ToolStripItem"/> actions.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="treeNode"/> is <c>null</c>.</exception>
        public TreeViewContextMenuItemFactory(TreeNode treeNode, TreeNodeInfo treeNodeInfo, TreeViewControl treeViewControl)
        {
            if (treeNode == null)
            {
                throw new ArgumentNullException("treeNode", Resources.ContextMenuItemFactory_Can_not_create_context_menu_items_without_tree_node);
            }

            if (treeNodeInfo == null)
            {
                throw new ArgumentNullException("treeNodeInfo", Resources.ContextMenuItemFactory_Can_not_create_context_menu_items_without_tree_node_info);
            }

            if (treeViewControl == null)
            {
                throw new ArgumentNullException("treeViewControl", Resources.ContextMenuItemFactory_Can_not_create_context_menu_items_without_tree_view_control);
            }

            this.treeNode = treeNode;
            this.treeNodeInfo = treeNodeInfo;
            this.treeViewControl = treeViewControl;
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
                Enabled = treeNodeInfo.CanRename != null && treeNodeInfo.CanRename(treeNode)
            };
            toolStripMenuItem.Click += (s, e) => treeNode.BeginEdit();
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
                Enabled = treeNodeInfo.CanRemove != null && treeNodeInfo.CanRemove(treeNode.Tag, treeNode.Parent != null ? treeNode.Parent.Tag : null)
            };
            toolStripMenuItem.Click += (s, e) => treeViewControl.DeleteNode(treeNode);
            return toolStripMenuItem;
        }

        /// <summary>
        /// Creates a <see cref="ToolStripItem"/> which is bound to the action of expanding
        /// the <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The created <see cref="ToolStripItem"/>.</returns>
        public ToolStripItem CreateExpandAllItem()
        {
            var children = treeNode.Nodes.OfType<TreeNode>();
            var toolStripMenuItem = new ToolStripMenuItem(Resources.Expand_all)
            {
                ToolTipText = Resources.Expand_all_ToolTip,
                Image = Resources.ExpandAllIcon,
                Enabled = children.Any()
            };
            toolStripMenuItem.Click += (s, e) => treeViewControl.ExpandAll(treeNode);
            return toolStripMenuItem;
        }

        /// <summary>
        /// Creates a <see cref="ToolStripItem"/> which is bound to the action of collapsing
        /// the <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The created <see cref="ToolStripItem"/>.</returns>
        public ToolStripItem CreateCollapseAllItem()
        {
            var children = treeNode.Nodes.OfType<TreeNode>();
            var toolStripMenuItem = new ToolStripMenuItem(Resources.Collapse_all)
            {
                ToolTipText = Resources.Collapse_all_ToolTip,
                Image = Resources.CollapseAllIcon,
                Enabled = children.Any()
            };
            toolStripMenuItem.Click += (s, e) => treeViewControl.CollapseAll(treeNode);
            return toolStripMenuItem;
        }
    }
}