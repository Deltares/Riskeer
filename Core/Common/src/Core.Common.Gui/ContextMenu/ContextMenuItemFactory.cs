using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Gui.Properties;

namespace Core.Common.Gui.ContextMenu
{
    internal class ContextMenuItemFactory
    {
        private readonly IGui gui;

        public ContextMenuItemFactory(IGui gui)
        {
            this.gui = gui;
        }

        public ToolStripItem CreateExpandAllItem(ITreeNode treeNode)
        {
            if (!GuiSet())
            {
                return null;
            }
            var toolStripMenuItem = new ToolStripMenuItem(Resources.Expand_all)
            {
                ToolTipText = Resources.Expand_all_ToolTip,
                Image = Resources.ExpandAllIcon
            };
            toolStripMenuItem.Click += (s, e) => treeNode.TreeView.ExpandAll(treeNode);
            return toolStripMenuItem;
        }

        public ToolStripItem CreateCollapseAllItem(ITreeNode treeNode)
        {
            if (!GuiSet())
            {
                return null;
            }
            var toolStripMenuItem = new ToolStripMenuItem(Resources.Collapse_all)
            {
                ToolTipText = Resources.Collapse_all_ToolTip,
                Image = Resources.CollapseAllIcon
            };
            toolStripMenuItem.Click += (s, e) => treeNode.TreeView.CollapseAll(treeNode);
            return toolStripMenuItem;
        }

        public ToolStripItem CreateExportItem(ITreeNode treeNode)
        {
            if (!GuiSet())
            {
                return null;
            }
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

        public ToolStripItem CreateImportItem(ITreeNode treeNode)
        {
            if (!GuiSet())
            {
                return null;
            }
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

        public ToolStripItem CreatePropertiesItem(ITreeNode treeNode)
        {
            if (!GuiSet())
            {
                return null;
            }
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

        private bool GuiSet()
        {
            return gui != null;
        }
    }
}