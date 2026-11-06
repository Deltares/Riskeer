// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Gui.Properties;

namespace Core.Gui.ContextMenu
{
    /// <summary>
    /// This class represents a factory for creating <see cref="ToolStripItem"/>.
    /// </summary>
    internal class TreeViewContextMenuItemFactory
    {
        private readonly object dataObject;
        private readonly ITreeViewCommands treeViewCommands;

        /// <summary>
        /// Creates a new instance of <see cref="TreeViewContextMenuItemFactory"/> for the given <paramref name="dataObject"/>.
        /// </summary>
        /// <param name="dataObject">The data object for which to create the <see cref="ToolStripItem"/> objects.</param>
        /// <param name="treeViewCommands">The <see cref="ITreeViewCommands"/> to use while executing the <see cref="ToolStripItem"/> actions.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dataObject"/> or <paramref name="treeViewCommands"/> is <c>null</c>.</exception>
        public TreeViewContextMenuItemFactory(object dataObject, ITreeViewCommands treeViewCommands)
        {
            if (dataObject == null)
            {
                throw new ArgumentNullException(nameof(dataObject), Resources.ContextMenuItemFactory_Can_not_create_context_menu_items_without_data);
            }

            if (treeViewCommands == null)
            {
                throw new ArgumentNullException(nameof(treeViewCommands), Resources.ContextMenuItemFactory_Can_not_create_context_menu_items_without_tree_view_commands);
            }

            this.dataObject = dataObject;
            this.treeViewCommands = treeViewCommands;
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
                Enabled = treeViewCommands.CanRenameNodeForData(dataObject)
            };
            toolStripMenuItem.Click += (s, e) => treeViewCommands.TryRenameNodeForData(dataObject);
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
                Enabled = treeViewCommands.CanRemoveNodeForData(dataObject)
            };
            toolStripMenuItem.Click += (s, e) => treeViewCommands.TryRemoveNodeForData(dataObject);
            return toolStripMenuItem;
        }

        /// <summary>
        /// Creates a <see cref="ToolStripItem"/> which is bound to the action of deleting
        /// the children of <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The created <see cref="ToolStripItem"/>.</returns>
        public ToolStripItem CreateDeleteChildrenItem()
        {
            var toolStripMenuItem = new ToolStripMenuItem(Resources.DeleteChildren)
            {
                ToolTipText = treeViewCommands.CanRemoveChildNodesOfData(dataObject)
                                  ? Resources.DeleteChildren_WithChildren_ToolTip
                                  : Resources.DeleteChildren_WithoutChildren_ToolTip,
                Image = Resources.DeleteChildrenIcon,
                Enabled = treeViewCommands.CanRemoveChildNodesOfData(dataObject)
            };
            toolStripMenuItem.Click += (s, e) => treeViewCommands.TryRemoveChildNodesOfData(dataObject);
            return toolStripMenuItem;
        }

        /// <summary>
        /// Creates a <see cref="ToolStripItem"/> which is bound to the action of expanding
        /// the <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The created <see cref="ToolStripItem"/>.</returns>
        public ToolStripItem CreateExpandAllItem()
        {
            var toolStripMenuItem = new ToolStripMenuItem(Resources.Expand_all)
            {
                ToolTipText = Resources.Expand_all_ToolTip,
                Image = Resources.ExpandAllIcon,
                Enabled = treeViewCommands.CanExpandOrCollapseForData(dataObject)
            };
            toolStripMenuItem.Click += (s, e) => treeViewCommands.TryExpandAllNodesForData(dataObject);
            return toolStripMenuItem;
        }

        /// <summary>
        /// Creates a <see cref="ToolStripItem"/> which is bound to the action of collapsing
        /// the <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The created <see cref="ToolStripItem"/>.</returns>
        public ToolStripItem CreateCollapseAllItem()
        {
            var toolStripMenuItem = new ToolStripMenuItem(Resources.Collapse_all)
            {
                ToolTipText = Resources.Collapse_all_ToolTip,
                Image = Resources.CollapseAllIcon,
                Enabled = treeViewCommands.CanExpandOrCollapseForData(dataObject)
            };
            toolStripMenuItem.Click += (s, e) => treeViewCommands.TryCollapseAllNodesForData(dataObject);
            return toolStripMenuItem;
        }
    }
}