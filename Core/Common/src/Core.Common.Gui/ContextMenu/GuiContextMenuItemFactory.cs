using System;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls;
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
        private readonly IGui gui;
        private readonly ITreeNode treeNode;

        /// <summary>
        /// Creates a new instance of <see cref="GuiContextMenuItemFactory"/>, which uses the <paramref name="gui"/> to create
        /// <see cref="ToolStripItem"/>.
        /// </summary>
        /// <param name="gui">The <see cref="IGui"/> which contains information for creating the <see cref="ToolStripItem"/>.</param>
        /// <param name="treeNode">The <see cref="ITreeNode"/> for which to create <see cref="ToolStripItem"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when 
        /// <list type="bullet">
        /// <item><paramref name="gui"/> is <c>null</c></item>
        /// <item><paramref name="treeNode"/> is <c>null</c></item>
        /// </list></exception>
        public GuiContextMenuItemFactory(IGui gui, ITreeNode treeNode)
        {
            if (gui == null)
            {
                throw new ArgumentNullException("gui", Resources.GuiContextMenuItemFactory_Can_not_create_gui_context_menu_items_without_gui);
            }
            if (treeNode == null)
            {
                throw new ArgumentNullException("treeNode", Resources.ContextMenuItemFactory_Can_not_create_context_menu_items_without_tree_node);
            }
            this.treeNode = treeNode;
            this.gui = gui;
        }

        /// <summary>
        /// Creates a <see cref="ToolStripItem"/> which is bound to the action of exporting 
        /// the data of the <see cref="ITreeNode"/>.
        /// </summary>
        /// <returns>The created <see cref="ToolStripItem"/>.</returns>
        public ToolStripItem CreateExportItem()
        {
            var data = treeNode.Tag;
            var exporters = gui.ApplicationCore.GetSupportedFileExporters(data);
            var newItem = new ToolStripMenuItem(Resources.Export)
            {
                ToolTipText = Resources.Export_ToolTip,
                Image = Resources.ExportIcon,
                Enabled = exporters.Any()
            };

            return newItem;
        }

        /// <summary>
        /// Creates a <see cref="ToolStripItem"/> which is bound to the action of importing
        /// the data of the given <see cref="ITreeNode"/>.
        /// </summary>
        /// <returns>The created <see cref="ToolStripItem"/>.</returns>
        public ToolStripItem CreateImportItem()
        {
            var data = treeNode.Tag;
            var importers = gui.ApplicationCore.GetSupportedFileImporters(data);
            var newItem = new ToolStripMenuItem(Resources.Import)
            {
                ToolTipText = Resources.Import_ToolTip,
                Image = Resources.ImportIcon,
                Enabled = importers.Any()
            };

            return newItem;
        }

        /// <summary>
        /// Creates a <see cref="ToolStripItem"/> which is bound to the action of showing
        /// the properties of the data of the given <see cref="ITreeNode"/>.
        /// </summary>
        /// <returns>The created <see cref="ToolStripItem"/>.</returns>
        public ToolStripItem CreatePropertiesItem()
        {
            var data = treeNode.Tag;
            var propertyInfos = gui.Plugins.SelectMany(p => p.GetPropertyInfos()).Where(pi => pi.ObjectType == data.GetType());
            var newItem = new ToolStripMenuItem(Resources.Properties)
            {
                ToolTipText = Resources.Properties_ToolTip,
                Image = Resources.PropertiesIcon,
                Enabled = propertyInfos.Any()
            };

            var guiCommandHandler = gui.CommandHandler;
            if (guiCommandHandler != null)
            {
                newItem.Click += (s, e) => guiCommandHandler.ShowProperties();
            }

            return newItem;
        }
    }
}