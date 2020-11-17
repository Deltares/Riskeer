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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.Properties;

namespace Core.Common.Gui.ContextMenu
{
    /// <summary>
    /// This class represents a factory for creating <see cref="ToolStripItem"/>.
    /// </summary>
    internal class GuiContextMenuItemFactory
    {
        private readonly IApplicationFeatureCommands applicationFeatureCommandHandler;
        private readonly IImportCommandHandler importCommandHandler;
        private readonly IExportCommandHandler exportCommandHandler;
        private readonly IUpdateCommandHandler updateCommandHandler;
        private readonly IViewCommands viewCommands;
        private readonly object dataObject;

        /// <summary>
        /// Creates a new instance of <see cref="GuiContextMenuItemFactory"/>.
        /// </summary>
        /// <param name="applicationFeatureCommandHandler">The <see cref="IApplicationFeatureCommands"/>
        /// which contains information for creating the <see cref="ToolStripItem"/>.</param>
        /// <param name="importCommandHandler">The <see cref="IImportCommandHandler"/> which contains
        /// information for creating the <see cref="ToolStripItem"/>.</param>
        /// <param name="exportCommandHandler">The <see cref="IExportCommandHandler"/> which contains
        /// information for creating the <see cref="ToolStripItem"/>.</param>
        /// <param name="updateCommandHandler">The <see cref="IUpdateCommandHandler"/> which contains
        /// information for creating the <see cref="ToolStripItem"/>.</param>
        /// <param name="viewCommandsHandler">The <see cref="IViewCommands"/> which contains information for
        /// creating the <see cref="ToolStripItem"/>.</param>
        /// <param name="dataObject">The data object for which to create <see cref="ToolStripItem"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        public GuiContextMenuItemFactory(IApplicationFeatureCommands applicationFeatureCommandHandler,
                                         IImportCommandHandler importCommandHandler,
                                         IExportCommandHandler exportCommandHandler,
                                         IUpdateCommandHandler updateCommandHandler,
                                         IViewCommands viewCommandsHandler,
                                         object dataObject)
        {
            if (applicationFeatureCommandHandler == null)
            {
                throw new ArgumentNullException(nameof(applicationFeatureCommandHandler),
                                                Resources.GuiContextMenuItemFactory_Can_not_create_gui_context_menu_items_without_gui);
            }

            if (importCommandHandler == null)
            {
                throw new ArgumentNullException(nameof(importCommandHandler),
                                                Resources.GuiContextMenuItemFactory_Can_not_create_gui_context_menu_items_without_import_handler);
            }

            if (exportCommandHandler == null)
            {
                throw new ArgumentNullException(nameof(exportCommandHandler),
                                                Resources.GuiContextMenuItemFactory_Can_not_create_gui_context_menu_items_without_export_handler);
            }

            if (updateCommandHandler == null)
            {
                throw new ArgumentNullException(nameof(updateCommandHandler),
                                                Resources.GuiContextMenuItemFactory_Can_not_create_gui_context_menu_items_without_update_handler);
            }

            if (viewCommandsHandler == null)
            {
                throw new ArgumentNullException(nameof(viewCommandsHandler),
                                                Resources.GuiContextMenuItemFactory_Can_not_create_gui_context_menu_items_without_view_commands);
            }

            if (dataObject == null)
            {
                throw new ArgumentNullException(nameof(dataObject),
                                                Resources.ContextMenuItemFactory_Can_not_create_context_menu_items_without_data);
            }

            this.applicationFeatureCommandHandler = applicationFeatureCommandHandler;
            this.importCommandHandler = importCommandHandler;
            this.exportCommandHandler = exportCommandHandler;
            this.updateCommandHandler = updateCommandHandler;
            viewCommands = viewCommandsHandler;
            this.dataObject = dataObject;
        }

        /// <summary>
        /// Creates a <see cref="ToolStripItem"/> which is bound to the action of opening a view 
        /// for the data of the <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The created <see cref="ToolStripItem"/>.</returns>
        public ToolStripItem CreateOpenItem()
        {
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
            bool canExport = exportCommandHandler.CanExportFrom(dataObject);
            var newItem = new ToolStripMenuItem(Resources.Export)
            {
                ToolTipText = Resources.Export_ToolTip,
                Image = Resources.ExportIcon,
                Enabled = canExport
            };
            newItem.Click += (s, e) => exportCommandHandler.ExportFrom(dataObject);

            return newItem;
        }

        /// <summary>
        /// Creates a <see cref="ToolStripItem"/> which is bound to the action of importing
        /// to the data of the given <see cref="TreeNode"/>.
        /// </summary>
        /// <param name="importInfos">An enumeration of <see cref="ImportInfo"/> instances,
        /// representing one or more suitable import actions.</param>
        /// <returns>The created <see cref="ToolStripItem"/>.</returns>
        /// <remarks>When no <paramref name="importInfos"/> parameter is provided, the suitable
        /// <see cref="ImportInfo"/> instances - as registered by the plugins - will be resolved
        /// dynamically.</remarks>
        public ToolStripItem CreateImportItem(IEnumerable<ImportInfo> importInfos = null)
        {
            return CreateImportItem(Resources.Import, Resources.Import_ToolTip, Resources.ImportIcon, importInfos);
        }

        /// <summary>
        /// Creates a <see cref="ToolStripItem"/> which is bound to the action of importing
        /// to the data of the given <see cref="TreeNode"/>.
        /// </summary>
        /// <param name="text">The text of the import item.</param>
        /// <param name="toolTip">The toolTip of the import item.</param>
        /// <param name="image">The image of the import item.</param>
        /// <param name="importInfos">An enumeration of <see cref="ImportInfo"/> instances,
        /// representing one or more suitable import actions.</param>
        /// <returns>The created <see cref="ToolStripItem"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="text"/>
        /// is <c>null</c> or only whitespace.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="toolTip"/>
        /// or <paramref name="image"/> is <c>null</c>.</exception>
        /// <remarks>When no <paramref name="importInfos"/> parameter is provided, the suitable
        /// <see cref="ImportInfo"/> instances - as registered by the plugins - will be resolved
        /// dynamically.</remarks>
        public ToolStripItem CreateImportItem(string text, string toolTip, Image image, IEnumerable<ImportInfo> importInfos = null)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException(@"Text should be set.", nameof(text));
            }

            if (toolTip == null)
            {
                throw new ArgumentNullException(nameof(toolTip));
            }

            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            importInfos = importInfos == null
                              ? importCommandHandler.GetSupportedImportInfos(dataObject)
                              : importInfos.Where(info => info.IsEnabled(dataObject));

            var importItem = new ToolStripMenuItem(text)
            {
                ToolTipText = toolTip,
                Image = image,
                Enabled = importInfos.Any()
            };

            importItem.Click += (s, e) => importCommandHandler.ImportOn(dataObject);

            return importItem;
        }

        /// <summary>
        /// Creates a <see cref="ToolStripItem"/> which is bound to the action of updating
        /// the data of the given <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The created <see cref="ToolStripItem"/>.</returns>
        public ToolStripItem CreateUpdateItem()
        {
            bool canUpdate = updateCommandHandler.CanUpdateOn(dataObject);
            var newItem = new ToolStripMenuItem(Resources.Update)
            {
                ToolTipText = Resources.Update_ToolTip,
                Image = Resources.RefreshIcon,
                Enabled = canUpdate
            };
            newItem.Click += (s, e) => updateCommandHandler.UpdateOn(dataObject);

            return newItem;
        }

        /// <summary>
        /// Creates a <see cref="ToolStripItem"/> which is bound to the action of showing
        /// the properties of the data of the given <see cref="TreeNode"/>.
        /// </summary>
        /// <returns>The created <see cref="ToolStripItem"/>.</returns>
        public ToolStripItem CreatePropertiesItem()
        {
            bool canShowProperties = applicationFeatureCommandHandler.CanShowPropertiesFor(dataObject);
            var newItem = new ToolStripMenuItem(Resources.Properties)
            {
                ToolTipText = Resources.Properties_ToolTip,
                Image = Resources.PropertiesHS,
                Enabled = canShowProperties
            };
            newItem.Click += (s, e) => applicationFeatureCommandHandler.ShowPropertiesForSelection();

            return newItem;
        }
    }
}