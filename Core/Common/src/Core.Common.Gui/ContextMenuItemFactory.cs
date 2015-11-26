using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Gui.Properties;

namespace Core.Common.Gui
{
    public class ContextMenuItemFactory
    {
        private readonly IGui gui;

        public ContextMenuItemFactory(IGui gui)
        {
            this.gui = gui;
        }

        public ToolStripItem CreateExportItem(object item)
        {
            var exporters = gui.ApplicationCore.GetSupportedExportersForItem(item);
            var newItem = new ToolStripMenuItem(Resources.Export)
            {
                ToolTipText = Resources.Export_ToolTip,
                Image = Resources.ExportIcon,
                Enabled = exporters.Any()
            };


            return newItem;
        }

        public ToolStripItem CreateImportItem(object item)
        {
            var importers = gui.ApplicationCore.GetImporters(item);
            var newItem = new ToolStripMenuItem(Resources.Import)
            {
                ToolTipText = Resources.Import_ToolTip,
                Image = Resources.ImportIcon,
                Enabled = importers.Any()
            };


            return newItem;
        }

        public ToolStripItem CreatePropertiesItem(object item)
        {
            var propertyInfos = gui.Plugins.SelectMany(p => p.GetPropertyInfos()).Where(pi => pi.ObjectType == item.GetType());
            var newItem = new ToolStripMenuItem(Resources.Properties)
            {
                ToolTipText = Resources.Properties_ToolTip,
                Image = Resources.PropertiesIcon,
                Enabled = propertyInfos.Any()
            };

            var guiCommandHandler = gui.CommandHandler;
            if (guiCommandHandler != null)
            {
                newItem.Click += (s,e) => guiCommandHandler.ShowProperties();
            }

            return newItem;
        }
    }
}