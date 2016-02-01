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
using System.Windows.Forms;
using Core.Common.Gui.Properties;

namespace Core.Common.Gui.ContextMenu
{
    /// <summary>
    /// This class represents a factory for creating <see cref="ToolStripItem"/>. The
    /// items the factory creates are dependent on a <see cref="IGui"/> set for this
    /// factory.
    /// </summary>
    internal class GuiContextMenuItemFactory
    {
        private readonly IApplicationFeatureCommands applicationFeatureCommandHandler;
        private readonly IExportImportCommandHandler exportImportCommandHandler;
        private readonly IViewCommands viewCommands;
        private readonly TreeNode treeNode;

        /// <summary>
        /// Creates a new instance of <see cref="GuiContextMenuItemFactory"/>, which uses the 
        /// <paramref name="applicationFeatureCommandHandler"/> to create <see cref="ToolStripItem"/>.
        /// </summary>
        /// <param name="applicationFeatureCommandHandler">The <see cref="IApplicationFeatureCommands"/> which contains information for creating the 
        /// <see cref="ToolStripItem"/>.</param>
        /// <param name="exportImportCommandHandler">The <see cref="IExportImportCommandHandler"/>
        /// which contains information for creating the <see cref="ToolStripItem"/>.</param>
        /// <param name="viewCommandsHandler">The <see cref="IViewCommands"/> which contains
        /// information for creating the <see cref="ToolStripItem"/>.</param>
        /// <param name="treeNode">The <see cref="TreeNode"/> for which to create <see cref="ToolStripItem"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when either <paramref name="applicationFeatureCommandHandler"/> 
        /// or <paramref name="exportImportCommandHandler"/> is <c>null</c>.</exception>
        public GuiContextMenuItemFactory(IApplicationFeatureCommands applicationFeatureCommandHandler, IExportImportCommandHandler exportImportCommandHandler, IViewCommands viewCommandsHandler, TreeNode treeNode)
        {
            if (applicationFeatureCommandHandler == null)
            {
                throw new ArgumentNullException("applicationFeatureCommandHandler", Resources.GuiContextMenuItemFactory_Can_not_create_gui_context_menu_items_without_gui);
            }
            if (exportImportCommandHandler == null)
            {
                throw new ArgumentNullException("exportImportCommandHandler", Resources.GuiContextMenuItemFactory_Can_not_create_gui_context_menu_items_without_exportImport_handler);
            }
            if (viewCommandsHandler == null)
            {
                throw new ArgumentNullException("viewCommandsHandler", Resources.GuiContextMenuItemFactory_Can_not_create_gui_context_menu_items_without_view_commands);
            }
            if (treeNode == null)
            {
                throw new ArgumentNullException("treeNode", Resources.ContextMenuItemFactory_Can_not_create_context_menu_items_without_tree_node);
            }
            this.applicationFeatureCommandHandler = applicationFeatureCommandHandler;
            this.exportImportCommandHandler = exportImportCommandHandler;
            viewCommands = viewCommandsHandler;
            this.treeNode = treeNode;
        }

        /// <summary>
        /// Creates a <see cref="ToolStripItem"/> which is bound to the action of opening a view 
        /// for the data of the <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The created <see cref="ToolStripItem"/>.</returns>
        public ToolStripItem CreateOpenItem()
        {
            object dataObject = treeNode.Tag;
            bool canOpenView = viewCommands.CanOpenViewFor(dataObject);
            var newItem = new ToolStripMenuItem(Resources.Open)
            {
                ToolTipText = Resources.Open_ToolTip,
                Image = Resources.OpenIcon,
                Enabled = canOpenView
            };
            newItem.Click += (s, e) => viewCommands.OpenView(dataObject);

            return newItem;
        }

        /// <summary>
        /// Creates a <see cref="ToolStripItem"/> which is bound to the action of exporting 
        /// the data of the <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The created <see cref="ToolStripItem"/>.</returns>
        public ToolStripItem CreateExportItem()
        {
            object dataObject = treeNode.Tag;
            bool canExport = exportImportCommandHandler.CanExportFrom(dataObject);
            var newItem = new ToolStripMenuItem(Resources.Export)
            {
                ToolTipText = Resources.Export_ToolTip,
                Image = Resources.ExportIcon,
                Enabled = canExport
            };
            newItem.Click += (s, e) => exportImportCommandHandler.ExportFrom(dataObject);

            return newItem;
        }

        /// <summary>
        /// Creates a <see cref="ToolStripItem"/> which is bound to the action of importing
        /// the data of the given <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The created <see cref="ToolStripItem"/>.</returns>
        public ToolStripItem CreateImportItem()
        {
            object dataObject = treeNode.Tag;
            bool canImport = exportImportCommandHandler.CanImportOn(dataObject);
            var newItem = new ToolStripMenuItem(Resources.Import)
            {
                ToolTipText = Resources.Import_ToolTip,
                Image = Resources.ImportIcon,
                Enabled = canImport
            };
            newItem.Click += (s, e) => exportImportCommandHandler.ImportOn(dataObject);

            return newItem;
        }

        /// <summary>
        /// Creates a <see cref="ToolStripItem"/> which is bound to the action of showing
        /// the properties of the data of the given <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The created <see cref="ToolStripItem"/>.</returns>
        public ToolStripItem CreatePropertiesItem()
        {
            object dataObject = treeNode.Tag;
            bool canShowProperties = applicationFeatureCommandHandler.CanShowPropertiesFor(dataObject);
            var newItem = new ToolStripMenuItem(Resources.Properties)
            {
                ToolTipText = Resources.Properties_ToolTip,
                Image = Resources.PropertiesIcon,
                Enabled = canShowProperties
            };
            newItem.Click += (s, e) => applicationFeatureCommandHandler.ShowPropertiesFor(dataObject);
            
            return newItem;
        }
    }
}