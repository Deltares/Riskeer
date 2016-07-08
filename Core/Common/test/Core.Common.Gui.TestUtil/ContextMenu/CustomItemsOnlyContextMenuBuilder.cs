﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Windows.Forms;
using Core.Common.Gui.ContextMenu;

namespace Core.Common.Gui.TestUtil.ContextMenu
{
    /// <summary>
    /// This class can be used for easily testing the custom items which are added to a context menu.
    /// </summary>
    public class CustomItemsOnlyContextMenuBuilder : IContextMenuBuilder
    {
        /// <summary>
        /// The context menu which is build.
        /// </summary>
        private readonly ContextMenuStrip contextMenu = new ContextMenuStrip();

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <returns>The <see cref="CustomItemsOnlyContextMenuBuilder"/>.</returns>
        public IContextMenuBuilder AddRenameItem()
        {
            return this;
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <returns>The <see cref="CustomItemsOnlyContextMenuBuilder"/>.</returns>
        public IContextMenuBuilder AddDeleteItem()
        {
            return this;
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <returns>The <see cref="CustomItemsOnlyContextMenuBuilder"/>.</returns>
        public IContextMenuBuilder AddExpandAllItem()
        {
            return this;
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <returns>The <see cref="CustomItemsOnlyContextMenuBuilder"/>.</returns>
        public IContextMenuBuilder AddCollapseAllItem()
        {
            return this;
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <returns>The <see cref="CustomItemsOnlyContextMenuBuilder"/>.</returns>
        public IContextMenuBuilder AddOpenItem()
        {
            return this;
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <returns>The <see cref="CustomItemsOnlyContextMenuBuilder"/>.</returns>
        public IContextMenuBuilder AddExportItem()
        {
            return this;
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <returns>The <see cref="CustomItemsOnlyContextMenuBuilder"/>.</returns>
        public IContextMenuBuilder AddImportItem()
        {
            return this;
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <returns>The <see cref="CustomItemsOnlyContextMenuBuilder"/>.</returns>
        public IContextMenuBuilder AddPropertiesItem()
        {
            return this;
        }

        /// <summary>
        /// Adds a toolstrip separator.
        /// </summary>
        /// <returns>The <see cref="CustomItemsOnlyContextMenuBuilder"/>.</returns>
        public IContextMenuBuilder AddSeparator()
        {
            contextMenu.Items.Add(new ToolStripSeparator());
            return this;
        }

        /// <summary>
        /// Adds a custom item to the <see cref="ContextMenuStrip"/>.
        /// </summary>
        /// <param name="item">The custom <see cref="StrictContextMenuItem"/> to add to the <see cref="ContextMenuStrip"/>.</param>
        /// <returns>The <see cref="CustomItemsOnlyContextMenuBuilder"/>.</returns>
        public IContextMenuBuilder AddCustomItem(StrictContextMenuItem item)
        {
            contextMenu.Items.Add(item);
            return this;
        }

        /// <summary>
        /// Obtain the <see cref="ContextMenuStrip"/>, which has been constructed by using <see cref="AddCustomItem"/>.
        /// </summary>
        /// <returns>The constructed <see cref="ContextMenuStrip"/>.</returns>
        public ContextMenuStrip Build()
        {
            return contextMenu;
        }
    }
}