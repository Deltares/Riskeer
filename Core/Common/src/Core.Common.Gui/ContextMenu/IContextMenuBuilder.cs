// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Gui.Plugin;

namespace Core.Common.Gui.ContextMenu
{
    /// <summary>
    /// Specifies the interface for objects that build context menus.
    /// </summary>
    public interface IContextMenuBuilder
    {
        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which starts edit mode for the name of <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="ContextMenuBuilder"/> itself.</returns>
        IContextMenuBuilder AddRenameItem();

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which deletes the <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="ContextMenuBuilder"/> itself.</returns>
        IContextMenuBuilder AddDeleteItem();

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which deletes the children <see cref="TreeNode"/>
        /// of <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="ContextMenuBuilder"/> itself.</returns>
        IContextMenuBuilder AddDeleteChildrenItem();

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which expands the <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="IContextMenuBuilder"/> itself.</returns>
        IContextMenuBuilder AddExpandAllItem();

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which collapses the <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="IContextMenuBuilder"/> itself.</returns>
        IContextMenuBuilder AddCollapseAllItem();

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which opens a view for the data of the <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="IContextMenuBuilder"/> itself.</returns>
        IContextMenuBuilder AddOpenItem();

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which exports the data of the <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="IContextMenuBuilder"/> itself.</returns>
        IContextMenuBuilder AddExportItem();

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which imports to the data of the <see cref="TreeNode"/>.
        /// </summary>
        /// <param name="importInfos">An enumeration of <see cref="ImportInfo"/> instances, representing one or more
        /// suitable import actions.</param>
        /// <returns>The <see cref="IContextMenuBuilder"/> itself.</returns>
        /// <remarks>When no <paramref name="importInfos"/> parameter is provided, the suitable <see cref="ImportInfo"/>
        /// instances - as registered by the plugins - will be resolved dynamically.</remarks>
        IContextMenuBuilder AddImportItem(IEnumerable<ImportInfo> importInfos = null);

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which imports to the data of the <see cref="TreeNode"/>.
        /// </summary>
        /// <param name="text">The text of the import item.</param>
        /// <param name="toolTip">The tooltip of the import item.</param>
        /// <param name="image">The image of the import item.</param>
        /// <param name="importInfos">An enumeration of <see cref="ImportInfo"/> instances, representing one or more
        /// suitable import actions.</param>
        /// <returns>The <see cref="IContextMenuBuilder"/> itself.</returns>
        /// <remarks>When no <paramref name="importInfos"/> parameter is provided, the suitable <see cref="ImportInfo"/>
        /// instances - as registered by the plugins - will be resolved dynamically.</remarks>
        IContextMenuBuilder AddImportItem(string text, string toolTip, Image image, IEnumerable<ImportInfo> importInfos = null);

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which updates the data of the <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="IContextMenuBuilder"/> itself.</returns>
        IContextMenuBuilder AddUpdateItem();

        /// <summary>
        /// Adds an item to the <see cref="ContextMenuStrip"/>, which shows properties of the data of the <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The <see cref="IContextMenuBuilder"/> itself.</returns>
        IContextMenuBuilder AddPropertiesItem();

        /// <summary>
        /// Adds a <see cref="ToolStripSeparator"/> to the <see cref="ContextMenuStrip"/>. A <see cref="ToolStripSeparator"/>
        /// is only added if the last item that was added to the <see cref="ContextMenuStrip"/> exists and is not a 
        /// <see cref="ToolStripSeparator"/>.
        /// </summary>
        /// <returns>The <see cref="IContextMenuBuilder"/> itself.</returns>
        IContextMenuBuilder AddSeparator();

        /// <summary>
        /// Adds a custom item to the <see cref="ContextMenuStrip"/>.
        /// </summary>
        /// <param name="item">The custom <see cref="StrictContextMenuItem"/> to add to the <see cref="ContextMenuStrip"/>.</param>
        /// <returns>The <see cref="IContextMenuBuilder"/> itself.</returns>
        IContextMenuBuilder AddCustomItem(StrictContextMenuItem item);

        /// <summary>
        /// Obtain the <see cref="ContextMenuStrip"/>, which has been constructed by using the other methods of
        /// <see cref="IContextMenuBuilder"/>.
        /// </summary>
        /// <returns>The constructed <see cref="ContextMenuStrip"/>.</returns>
        ContextMenuStrip Build();
    }
}