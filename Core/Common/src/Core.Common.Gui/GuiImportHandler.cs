using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.IO;
using Core.Common.Base.Service;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Forms.ProgressDialog;
using Core.Common.Gui.Properties;
using log4net;

namespace Core.Common.Gui
{
    /// <summary>
    /// Class responsible for import handling.
    /// </summary>
    public class GuiImportHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(GuiImportHandler));

        private readonly IGui gui;

        public GuiImportHandler(IGui gui)
        {
            this.gui = gui;
        }

        public void ImportUsingImporter(IFileImporter importer, object target)
        {
            GetImportedItemsUsingFileOpenDialog(importer, target);
        }

        public void ImportDataTo(object target)
        {
            ImportToItem(target);
        }

        public IFileImporter GetSupportedImporterForTargetType(object target)
        {
            var selectImporterDialog = new SelectItemDialog(gui.MainWindow);

            var importers = gui.ApplicationCore.GetSupportedFileImporters(target);
            //if there is only one available exporter use that.
            if (!importers.Any())
            {
                MessageBox.Show(Resources.GuiImportHandler_GetSupportedImporterForTargetType_No_importer_available_for_this_item, Resources.GuiImportHandler_GetSupportedImporterForTargetType_Error);
                Log.ErrorFormat(Resources.GuiImportHandler_GetSupportedImporterForTargetType_No_importer_available_for_this_item_0_, target);
                return null;
            }

            //if there is only one available importer use that.
            if (importers.Count() == 1)
            {
                return importers.ElementAt(0);
            }

            foreach (IFileImporter importer in importers)
            {
                var category = string.IsNullOrEmpty(importer.Category) ? Resources.GuiImportHandler_GetSupportedImporterForTargetType_Data_Import : importer.Category;
                var itemImage = importer.Image ?? Resources.brick;

                selectImporterDialog.AddItemType(importer.Name, category, itemImage, null);
            }

            if (selectImporterDialog.ShowDialog() == DialogResult.OK)
            {
                var importerName = selectImporterDialog.SelectedItemTypeName;
                return importers.First(i => i.Name == importerName);
            }

            return null;
        }

        private void ImportToItem(object item)
        {
            var importer = GetSupportedImporterForTargetType(item);
            if (importer == null)
            {
                return;
            }

            GetImportedItemsUsingFileOpenDialog(importer, item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importer">Item to import</param>
        /// <param name="target"></param>
        /// <returns></returns>
        private void GetImportedItemsUsingFileOpenDialog(IFileImporter importer, object target)
        {
            var windowTitle = string.Format(Resources.GuiImportHandler_GetImportedItemsUsingFileOpenDialog_Select_a_DataType_0_file_to_import_from, importer.Name);
            var dialog = new OpenFileDialog
            {
                Filter = importer.FileFilter,
                Multiselect = true,
                Title = windowTitle,
                RestoreDirectory = true
            };

            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            Log.Info(Resources.GuiImportHandler_GetImportedItemsUsingFileOpenDialog_Start_importing_data);

            ActivityProgressDialogRunner.Run(gui.MainWindow, dialog.FileNames.Select(f => new FileImportActivity(importer, target, f)));
        }
    }
}