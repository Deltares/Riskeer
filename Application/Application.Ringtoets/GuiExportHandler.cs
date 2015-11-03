using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Application.Ringtoets.Forms;
using Application.Ringtoets.Properties;
using Core.Common.Base;
using Core.Common.Controls;
using Core.Common.Gui;
using log4net;
using MessageBox = Core.Common.Controls.Swf.MessageBox;

namespace Application.Ringtoets
{
    public class GuiExportHandler
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GuiExportHandler));
        private static readonly Bitmap brickImage = Resources.brick;

        // TODO: refactor it, remove Funcs - too complicated design, initialize exporters in a different way
        public GuiExportHandler(Func<object, IEnumerable<IFileExporter>> fileExportersGetter, Func<object, IView> viewGetter)
        {
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

        public IList<IFileExporter> GetSupportedExportersForItem(object itemToExport)
        {
            var sourceType = itemToExport.GetType();

            return FileExportersGetter(itemToExport)
                .Where(e => e.SourceTypes().Any(type => type == sourceType || type.IsAssignableFrom(sourceType)) &&
                            e.CanExportFor(itemToExport))
                .ToList();
        }

        public void GetExporterDialog(IFileExporter exporter, object selectedItem)
        {
            var view = ViewGetter(exporter) as IDialog;

            if (view != null)
            {
                if (view.ShowModal() == DialogResult.OK)
                {
                    if (view is IConfigureDialog)
                    {
                        ((IConfigureDialog) (view)).Configure(exporter);
                    }

                    //Do Export (Not as activity yet)
                    exporter.Export(selectedItem, null); //path argument is 'bypassed' in Configure
                }

                return;
            }

            ExporterItemUsingFileOpenDialog(exporter, selectedItem);
        }

        private Func<object, IEnumerable<IFileExporter>> FileExportersGetter { get; set; }
        private Func<object, IView> ViewGetter { get; set; }

        private IFileExporter GetSupportedExporterForItemUsingDialog(object itemToExport)
        {
            var sourceType = itemToExport.GetType();
            var selectExporterDialog = new SelectItemDialog();

            var fileExporters = GetSupportedExportersForItem(itemToExport);

            //if there is only one available exporter use that.
            if (fileExporters.Count == 0)
            {
                MessageBox.Show(Resources.GuiExportHandler_GetSupportedExporterForItemUsingDialog_No_exporter_for_this_item_available_);
                log.Warn(String.Format(Resources.GuiExportHandler_GetSupportedExporterForItemUsingDialog_No_exporter_for_this_item___0___available_, sourceType));
                return null;
            }

            //if there is only one available exporter use that.
            if (fileExporters.Count == 1)
            {
                return fileExporters[0];
            }

            foreach (var fileExporter in fileExporters)
            {
                selectExporterDialog.AddItemType(fileExporter.Name, fileExporter.Category, fileExporter.Icon ?? brickImage, null);
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

            var dlg = new SaveFileDialog
            {
                Filter = exporter.FileFilter,
                Title = Resources.GuiExportHandler_ExporterItemUsingFileOpenDialog_Select_a_file_to_export_to,
                FilterIndex = 2
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (exporter.Export(item, dlg.FileName))
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