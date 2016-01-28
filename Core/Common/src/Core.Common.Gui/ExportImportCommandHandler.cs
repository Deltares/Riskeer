using System;
using System.Linq;
using System.Windows.Forms;

using Core.Common.Base.IO;
using Core.Common.Base.Plugin;
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

        private readonly ApplicationCore applicationCore;

        private readonly IDocumentViewController documentViewController;

        private readonly GuiImportHandler importHandler;
        private readonly GuiExportHandler exportHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportImportCommandHandler"/> class.
        /// </summary>
        /// <param name="dialogParent"></param>
        /// <param name="applicationCore"></param>
        /// <param name="documentViewController"></param>
        public ExportImportCommandHandler(IWin32Window dialogParent, ApplicationCore applicationCore, IDocumentViewController documentViewController)
        {
            this.applicationCore = applicationCore;
            this.documentViewController = documentViewController;
            importHandler = new GuiImportHandler(dialogParent, this.applicationCore);
            exportHandler = new GuiExportHandler(dialogParent,
                                                 o => this.applicationCore.GetSupportedFileExporters(o),
                                                 o => this.documentViewController.DocumentViewsResolver.CreateViewForData(o));
        }

        public bool CanImportOn(object obj)
        {
            return applicationCore.GetSupportedFileImporters(obj).Any();
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
            return applicationCore.GetSupportedFileExporters(obj).Any();
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
    }
}