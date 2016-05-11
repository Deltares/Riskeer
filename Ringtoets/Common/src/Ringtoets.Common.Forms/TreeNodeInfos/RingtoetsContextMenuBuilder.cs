// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Windows.Forms;
using Core.Common.Gui.ContextMenu;

namespace Ringtoets.Common.Forms.TreeNodeInfos
{
    /// <summary>
    /// Decorator for <see cref="ContextMenuBuilder"/>.
    /// </summary>
    public class RingtoetsContextMenuBuilder : IContextMenuBuilder
    {
        private readonly IContextMenuBuilder contextMenuBuilder;

        /// <summary>
        /// Creates a new instance of the <see cref="RingtoetsContextMenuBuilder"/>.
        /// </summary>
        /// <param name="contextMenuBuilder">The context menu builder to decorate.</param>
        public RingtoetsContextMenuBuilder(IContextMenuBuilder contextMenuBuilder)
        {
            this.contextMenuBuilder = contextMenuBuilder;
        }

        public IContextMenuBuilder AddRenameItem()
        {
            return contextMenuBuilder.AddRenameItem();
        }

        public IContextMenuBuilder AddDeleteItem()
        {
            return contextMenuBuilder.AddDeleteItem();
        }

        public IContextMenuBuilder AddExpandAllItem()
        {
            return contextMenuBuilder.AddExpandAllItem();
        }

        public IContextMenuBuilder AddCollapseAllItem()
        {
            return contextMenuBuilder.AddCollapseAllItem();
        }

        public IContextMenuBuilder AddOpenItem()
        {
            return contextMenuBuilder.AddOpenItem();
        }

        public IContextMenuBuilder AddExportItem()
        {
            return contextMenuBuilder.AddExportItem();
        }

        public IContextMenuBuilder AddImportItem()
        {
            return contextMenuBuilder.AddImportItem();
        }

        public IContextMenuBuilder AddPropertiesItem()
        {
            return contextMenuBuilder.AddPropertiesItem();
        }

        public IContextMenuBuilder AddSeparator()
        {
            return contextMenuBuilder.AddSeparator();
        }

        public IContextMenuBuilder AddCustomItem(StrictContextMenuItem item)
        {
            return contextMenuBuilder.AddCustomItem(item);
        }

        public ContextMenuStrip Build()
        {
            return contextMenuBuilder.Build();
        }
    }
}
