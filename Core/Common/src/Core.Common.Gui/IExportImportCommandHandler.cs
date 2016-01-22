using Core.Common.Base.IO;

namespace Core.Common.Gui
{
    /// <summary>
    /// Interface declaring commands/methods related to importing and exporting data.
    /// </summary>
    public interface IExportImportCommandHandler
    {
        /// <summary>
        /// Indicates if there are importers for the current Gui.Selection
        /// </summary>
        /// <param name="obj"></param>
        bool CanImportOn(object obj);

        /// <summary>
        /// Indicates if there are exporters for the current Gui.Selection
        /// </summary>
        /// <param name="obj"></param>
        bool CanExportFrom(object obj);

        void ExportFrom(object data, IFileExporter exporter = null);

        void ImportOn(object target, IFileImporter importer = null);
    }
}