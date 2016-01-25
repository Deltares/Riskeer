using System;
using System.Windows.Forms;
using Core.Common.Gui.Properties;
using TreeNode = Core.Common.Controls.TreeView.TreeNode;

namespace Core.Common.Gui.ContextMenu
{
    /// <summary>
    /// This class represents a factory for creating <see cref="ToolStripItem"/>. The
    /// items the factory creates are dependent on a <see cref="IGui"/> set for this
    /// factory.
    /// </summary>
    internal class GuiContextMenuItemFactory
    {
        private readonly IGuiCommandHandler commandHandler;
        private readonly IExportImportCommandHandler exportImportCommandHandler;
        private readonly IViewCommands viewCommands;
        private readonly TreeNode treeNode;

        /// <summary>
        /// Creates a new instance of <see cref="GuiContextMenuItemFactory"/>, which uses the 
        /// <paramref name="commandHandler"/> to create <see cref="ToolStripItem"/>.
        /// </summary>
        /// <param name="commandHandler">The <see cref="IGuiCommandHandler"/> which contains information for creating the 
        /// <see cref="ToolStripItem"/>.</param>
        /// <param name="exportImportCommandHandler">The <see cref="IExportImportCommandHandler"/>
        /// which contains information for creating the <see cref="ToolStripItem"/>.</param>
        /// <param name="viewCommandsHandler">The <see cref="IViewCommands"/> which contains
        /// information for creating the <see cref="ToolStripItem"/>.</param>
        /// <param name="treeNode">The <see cref="TreeNode"/> for which to create <see cref="ToolStripItem"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when either <paramref name="commandHandler"/> 
        /// or <paramref name="exportImportCommandHandler"/> is <c>null</c>.</exception>
        public GuiContextMenuItemFactory(IGuiCommandHandler commandHandler, IExportImportCommandHandler exportImportCommandHandler, IViewCommands viewCommandsHandler, TreeNode treeNode)
        {
            if (commandHandler == null)
            {
                throw new ArgumentNullException("commandHandler", Resources.GuiContextMenuItemFactory_Can_not_create_gui_context_menu_items_without_gui);
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
            this.commandHandler = commandHandler;
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
            bool canShowProperties = commandHandler.CanShowPropertiesFor(dataObject);
            var newItem = new ToolStripMenuItem(Resources.Properties)
            {
                ToolTipText = Resources.Properties_ToolTip,
                Image = Resources.PropertiesIcon,
                Enabled = canShowProperties
            };
            newItem.Click += (s, e) => commandHandler.ShowPropertiesFor(dataObject);
            
            return newItem;
        }
    }
}