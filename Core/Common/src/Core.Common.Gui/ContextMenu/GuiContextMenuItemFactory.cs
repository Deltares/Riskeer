using System;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Controls.Swf.TreeViewControls;
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
        private readonly IGuiCommandHandler commandHandler;
        private readonly ITreeNode treeNode;

        /// <summary>
        /// Creates a new instance of <see cref="GuiContextMenuItemFactory"/>, which uses the 
        /// <paramref name="commandHandler"/> to create <see cref="ToolStripItem"/>.
        /// </summary>
        /// <param name="commandHandler">The <see cref="IGuiCommandHandler"/> which contains information for creating the 
        /// <see cref="ToolStripItem"/>.</param>
        /// <param name="treeNode">The <see cref="ITreeNode"/> for which to create <see cref="ToolStripItem"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="commandHandler"/> is <c>null</c>.</exception>
        public GuiContextMenuItemFactory(IGuiCommandHandler commandHandler, ITreeNode treeNode)
        {
            if (commandHandler == null)
            {
                throw new ArgumentNullException("commandHandler", Resources.GuiContextMenuItemFactory_Can_not_create_gui_context_menu_items_without_gui);
            }
            if (treeNode == null)
            {
                throw new ArgumentNullException("treeNode", Resources.ContextMenuItemFactory_Can_not_create_context_menu_items_without_tree_node);
            }
            this.commandHandler = commandHandler;
            this.treeNode = treeNode;
        }

        /// <summary>
        /// Creates a <see cref="ToolStripItem"/> which is bound to the action of opening a view 
        /// for the data of the <see cref="ITreeNode"/>.
        /// </summary>
        /// <returns>The created <see cref="ToolStripItem"/>.</returns>
        public ToolStripItem CreateOpenItem()
        {
            bool canOpenView = commandHandler.CanOpenDefaultViewFor(treeNode.Tag);
            var newItem = new ToolStripMenuItem(Resources.Open)
            {
                ToolTipText = Resources.Open_ToolTip,
                Image = Resources.OpenIcon,
                Enabled = canOpenView
            };
            newItem.Click += (s, e) => commandHandler.OpenView(treeNode.Tag);

            return newItem;
        }

        /// <summary>
        /// Creates a <see cref="ToolStripItem"/> which is bound to the action of exporting 
        /// the data of the <see cref="ITreeNode"/>.
        /// </summary>
        /// <returns>The created <see cref="ToolStripItem"/>.</returns>
        public ToolStripItem CreateExportItem()
        {
            bool canExport = commandHandler.CanExportFrom(treeNode.Tag);
            var newItem = new ToolStripMenuItem(Resources.Export)
            {
                ToolTipText = Resources.Export_ToolTip,
                Image = Resources.ExportIcon,
                Enabled = canExport
            };
            newItem.Click += (s, e) => commandHandler.ExportFrom(treeNode.Tag);

            return newItem;
        }

        /// <summary>
        /// Creates a <see cref="ToolStripItem"/> which is bound to the action of importing
        /// the data of the given <see cref="ITreeNode"/>.
        /// </summary>
        /// <returns>The created <see cref="ToolStripItem"/>.</returns>
        public ToolStripItem CreateImportItem()
        {
            bool canImport = commandHandler.CanImportOn(treeNode.Tag);
            var newItem = new ToolStripMenuItem(Resources.Import)
            {
                ToolTipText = Resources.Import_ToolTip,
                Image = Resources.ImportIcon,
                Enabled = canImport
            };
            newItem.Click += (s, e) => commandHandler.ImportOn(treeNode.Tag);

            return newItem;
        }

        /// <summary>
        /// Creates a <see cref="ToolStripItem"/> which is bound to the action of showing
        /// the properties of the data of the given <see cref="ITreeNode"/>.
        /// </summary>
        /// <returns>The created <see cref="ToolStripItem"/>.</returns>
        public ToolStripItem CreatePropertiesItem()
        {
            bool canShowProperties = commandHandler.CanShowPropertiesFor(treeNode.Tag);
            var newItem = new ToolStripMenuItem(Resources.Properties)
            {
                ToolTipText = Resources.Properties_ToolTip,
                Image = Resources.PropertiesIcon,
                Enabled = canShowProperties
            };
            newItem.Click += (s, e) => commandHandler.ShowPropertiesFor(treeNode.Tag);
            
            return newItem;
        }
    }
}