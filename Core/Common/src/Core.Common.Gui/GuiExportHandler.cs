using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.IO;
using Core.Common.Controls;
using Core.Common.Controls.Views;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Properties;
using log4net;

namespace Core.Common.Gui
{
    public class GuiExportHandler
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GuiExportHandler));
        private static readonly Bitmap brickImage = Resources.brick;

        private readonly IWin32Window owner;

        // TODO: refactor it, remove Funcs - too complicated design, initialize exporters in a different way
        public GuiExportHandler(IWin32Window owner, Func<object, IEnumerable<IFileExporter>> fileExportersGetter, Func<object, IView> viewGetter)
        {
            this.owner = owner;
            FileExportersGetter = fileExportersGetter;
            ViewGetter = viewGetter;
        }

        public void ExportFrom(object item)
        {
            var exporter = GetSupportedExporterForItemUsingDialog(item);
            if (exporter == null)
            {
                return;
            }
            GetExporterDialog(exporter, item);
        }

        public void GetExporterDialog(IFileExporter exporter, object selectedItem)
        {
            ExporterItemUsingFileOpenDialog(exporter, selectedItem);
        }

        private Func<object, IEnumerable<IFileExporter>> FileExportersGetter { get; set; }
        private Func<object, IView> ViewGetter { get; set; }

        private IFileExporter GetSupportedExporterForItemUsingDialog(object itemToExport)
        {
            var sourceType = itemToExport.GetType();
            var selectExporterDialog = new SelectItemDialog(owner);

            var fileExporters = FileExportersGetter(itemToExport);

            //if there is only one available exporter use that.
            if (!fileExporters.Any())
            {
                MessageBox.Show(Resources.GuiExportHandler_GetSupportedExporterForItemUsingDialog_No_exporter_for_this_item_available);
                log.Warn(String.Format(Resources.GuiExportHandler_GetSupportedExporterForItemUsingDialog_No_exporter_for_this_item_0_available, sourceType));
                return null;
            }

            //if there is only one available exporter use that.
            if (fileExporters.Count() == 1)
            {
                return fileExporters.ElementAt(0);
            }

            foreach (var fileExporter in fileExporters)
            {
                selectExporterDialog.AddItemType(fileExporter.Name, fileExporter.Category, fileExporter.Image ?? brickImage, null);
            }

            if (selectExporterDialog.ShowDialog() == DialogResult.OK)
            {
                return fileExporters.First(i => i.Name == selectExporterDialog.SelectedItemTypeName);
            }

            return null;
        }

        private void ExporterItemUsingFileOpenDialog(IFileExporter exporter, object item)
        {
            log.Info(Resources.GuiExportHandler_ExporterItemUsingFileOpenDialog_Start_exporting);

            var windowTitle = string.Format(Resources.GuiExportHandler_ExporterItemUsingFileOpenDialog_Select_a_DataType_0_file_to_export_to, exporter.Name);
            var saveFileDialog = new SaveFileDialog
            {
                Filter = exporter.FileFilter,
                Title = windowTitle,
                FilterIndex = 2
            };

            if (saveFileDialog.ShowDialog(owner) == DialogResult.OK)
            {
                if (exporter.Export(item, saveFileDialog.FileName))
                {
                    log.Info(Resources.GuiExportHandler_ExporterItemUsingFileOpenDialog_Finished_exporting);
                }
                else
                {
                    log.Warn(Resources.GuiExportHandler_ExporterItemUsingFileOpenDialog_Export_failed);
                }
            }
        }
    }
}