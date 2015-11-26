using System.Linq;
using System.Windows.Forms;
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
            var exporters = gui.ApplicationCore.FileExporters.Where(fe => fe.CanExportFor(item));
            var newItem = new ToolStripMenuItem(Resources.Export)
            {
                ToolTipText = Resources.Export_ToolTip,
                Image = Resources.ExportIcon
            };

            newItem.Enabled = exporters.Any();

            return newItem;
        }

        public ToolStripItem CreateImportItem(object item)
        {
            var importers = gui.ApplicationCore.FileImporters.Where(fe => fe.CanImportOn(item));
            var newItem = new ToolStripMenuItem(Resources.Import)
            {
                ToolTipText = Resources.Import_ToolTip,
                Image = Resources.ImportIcon
            };

            newItem.Enabled = importers.Any();

            return newItem;
        }
    }
}