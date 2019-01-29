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

using System;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Properties;

namespace Core.Common.Gui.ContextMenu
{
    /// <summary>
    /// This class represents a builder which can be used to create a context menu, which's items require information
    /// on the <see cref="IGui"/> in order to render or perform actions.
    /// </summary>
    public class ContextMenuBuilder : IContextMenuBuilder
    {
        private readonly GuiContextMenuItemFactory guiItemsFactory;
        private readonly TreeViewContextMenuItemFactory treeViewItemsFactory;
        private readonly ContextMenuStrip contextMenu;

        /// <summary>
        /// Creates a new instance of <see cref="ContextMenuBuilder"/>.
        /// </summary>
        /// <param name="featureCommandHandler">The <see cref="IApplicationFeatureCommands"/> from which to obtain
        /// information to render and bind actions to the items of the <see cref="ContextMenu"/>.</param>
        /// <param name="importCommandHandler">The <see cref="IImportCommandHandler"/> from which to obtain
        /// information to render and bind actions to the items of the <see cref="ContextMenu"/>.</param>
        /// <param name="exportCommandHandler">The <see cref="IExportCommandHandler"/> from which to obtain
        /// information to render and bind actions to the items of the <see cref="ContextMenu"/>.</param>
        /// <param name="updateCommandHandler">The <see cref="IUpdateCommandHandler"/> from which to obtain
        /// information to render and bind actions to the items of the <see cref="ContextMenu"/>.</param>
        /// <param name="viewCommands">The <see cref="IViewCommands"/> from which to obtain information to render
        /// and bind actions to the items of the <see cref="ContextMenu"/>.</param>
        /// <param name="dataValue">The data object for which to create a <see cref="ContextMenuStrip"/>.</param>
        /// <param name="treeViewControl">The <see cref="TreeViewControl"/> to use while executing the <see cref="ContextMenuStrip"/> actions.</param>
        /// <exception cref="ContextMenuBuilderException">Thrown when any input argument is <c>null</c>.</exception>
        public ContextMenuBuilder(IApplicationFeatureCommands featureCommandHandler,
                                  IImportCommandHandler importCommandHandler,
                                  IExportCommandHandler exportCommandHandler,
                                  IUpdateCommandHandler updateCommandHandler,
                                  IViewCommands viewCommands, object dataValue,
                                  TreeViewControl treeViewControl)
        {
            try
            {
                guiItemsFactory = new GuiContextMenuItemFactory(featureCommandHandler,
                                                                importCommandHandler,
                                                                exportCommandHandler,
                                                                updateCommandHandler,
                                                                viewCommands,
                                                                dataValue);

                treeViewItemsFactory = new TreeViewContextMenuItemFactory(dataValue, treeViewControl);
            }
            catch (ArgumentNullException e)
            {
                throw new ContextMenuBuilderException(Resources.ContextMenuBuilder_ContextMenuBuilder_Cannot_create_instances_of_factories, e);
            }

            contextMenu = new ContextMenuStrip();
        }

        public IContextMenuBuilder AddRenameItem()
        {
            AddItem(treeViewItemsFactory.CreateRenameItem());
            return this;
        }

        public IContextMenuBuilder AddDeleteItem()
        {
            AddItem(treeViewItemsFactory.CreateDeleteItem());
            return this;
        }

        public IContextMenuBuilder AddDeleteChildrenItem()
        {
            AddItem(treeViewItemsFactory.CreateDeleteChildrenItem());
            return this;
        }

        public IContextMenuBuilder AddExpandAllItem()
        {
            AddItem(treeViewItemsFactory.CreateExpandAllItem());
            return this;
        }

        public IContextMenuBuilder AddCollapseAllItem()
        {
            AddItem(treeViewItemsFactory.CreateCollapseAllItem());
            return this;
        }

        public IContextMenuBuilder AddOpenItem()
        {
            AddItem(guiItemsFactory.CreateOpenItem());
            return this;
        }

        public IContextMenuBuilder AddExportItem()
        {
            AddItem(guiItemsFactory.CreateExportItem());
            return this;
        }

        public IContextMenuBuilder AddImportItem()
        {
            AddItem(guiItemsFactory.CreateImportItem());
            return this;
        }

        public IContextMenuBuilder AddUpdateItem()
        {
            AddItem(guiItemsFactory.CreateUpdateItem());
            return this;
        }

        public IContextMenuBuilder AddCustomImportItem(string text, string toolTip, Image image)
        {
            AddItem(guiItemsFactory.CreateCustomImportItem(text, toolTip, image));
            return this;
        }

        public IContextMenuBuilder AddPropertiesItem()
        {
            AddItem(guiItemsFactory.CreatePropertiesItem());
            return this;
        }

        public IContextMenuBuilder AddSeparator()
        {
            if (MayAddSeparator())
            {
                AddItem(new ToolStripSeparator());
            }

            return this;
        }

        public IContextMenuBuilder AddCustomItem(StrictContextMenuItem item)
        {
            AddItem(item);
            return this;
        }

        public ContextMenuStrip Build()
        {
            if (contextMenu.Items.Count > 0)
            {
                int lastIndex = contextMenu.Items.Count - 1;
                if (contextMenu.Items[lastIndex] is ToolStripSeparator)
                {
                    contextMenu.Items.RemoveAt(lastIndex);
                }
            }

            return contextMenu;
        }

        private bool MayAddSeparator()
        {
            if (contextMenu.Items.Count == 0)
            {
                return false;
            }

            return !(contextMenu.Items[contextMenu.Items.Count - 1] is ToolStripSeparator);
        }

        private void AddItem(ToolStripItem item)
        {
            if (item != null)
            {
                contextMenu.Items.Add(item);
            }
        }
    }
}