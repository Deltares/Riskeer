using System;
using System.Linq;

using Core.Common.Base.IO;
using Core.Common.Gui.Properties;

using log4net;

namespace Core.Common.Gui
{
    /// <summary>
    /// Class responsible for exporting and importing of data.
    /// </summary>
    public class ExportImportCommandHandler : IExportImportCommandHandler
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ExportImportCommandHandler));

        private readonly IGui gui;
        private readonly GuiImportHandler importHandler;
        private readonly GuiExportHandler exportHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportImportCommandHandler"/> class.
        /// </summary>
        /// <param name="gui">The GUI.</param>
        public ExportImportCommandHandler(IGui gui)
        {
            this.gui = gui;
            importHandler = CreateGuiImportHandler(this.gui);
            exportHandler = CreateGuiExportHandler(this.gui);
        }

        public bool CanImportOn(object obj)
        {
            return gui.ApplicationCore.GetSupportedFileImporters(obj).Any();
        }

        public void ImportOn(object target, IFileImporter importer = null)
        {
            try
            {
                if (importer == null)
                {
                    importHandler.ImportDataTo(target);
                }
                else
                {
                    importHandler.ImportUsingImporter(importer, target);
                }
            }
            catch (Exception)
            {
                log.ErrorFormat(Resources.GuiCommandHandler_ImportOn_Unable_to_import_on_0_, target);
            }
        }

        public bool CanExportFrom(object obj)
        {
            return gui.ApplicationCore.GetSupportedFileExporters(obj).Any();
        }

        public void ExportFrom(object data, IFileExporter exporter = null)
        {
            if (exporter == null)
            {
                exportHandler.ExportFrom(data);
            }
            else
            {
                exportHandler.GetExporterDialog(exporter, data);
            }
        }

        private static GuiImportHandler CreateGuiImportHandler(IGui gui)
        {
            return new GuiImportHandler(gui);
        }

        private static GuiExportHandler CreateGuiExportHandler(IGui gui)
        {
            return new GuiExportHandler(gui.MainWindow, o => gui.ApplicationCore.GetSupportedFileExporters(o), o => gui.DocumentViewsResolver.CreateViewForData(o));
        }
    }
}