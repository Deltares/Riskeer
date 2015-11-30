using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Workflow;
using Core.Common.Controls;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Properties;
using Core.Common.Utils.Aop;
using Core.Common.Utils.IO;
using Core.Common.Utils.Reflection;
using log4net;
using MessageBox = Core.Common.Controls.Swf.MessageBox;

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
            ConfigureImporterAndRun(importer, target);
        }

        public void ImportDataTo(object target)
        {
            ImportToItem(target);
        }

        public IFileImporter GetSupportedImporterForTargetType(object target)
        {
            var selectImporterDialog = new SelectItemDialog();

            var importers = gui.ApplicationCore.GetSupportedFileImporters(target);
            //if there is only one available exporter use that.
            if (!importers.Any())
            {
                MessageBox.Show(Resources.GuiImportHandler_GetSupportedImporterForTargetType_No_importer_available_for_this_item);
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

        /// <summary>
        /// Typically used after drop action of files from outsite the application
        /// </summary>
        /// <param name="target"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public IFileImporter GetSupportedImporterForTargetTypeAndSelectedFiles(object target, IEnumerable<string> files)
        {
            var selectImporterDialog = new SelectItemDialog();

            Image itemImage = Resources.brick;

            var importers = gui.ApplicationCore.GetSupportedFileImporters(target);

            importers =
                importers.Where(
                    importer => FileUtils.FileMatchesFileFilterByExtension(importer.FileFilter, files.First())).ToList();

            //if there is only one available importer use that.))
            if (!importers.Any())
            {
                return null;
            }
            if (importers.Count() == 1)
            {
                return importers.ElementAt(0);
            }

            foreach (IFileImporter importer in importers)
            {
                selectImporterDialog.AddItemType(importer.Name, Resources.GuiImportHandler_GetSupportedImporterForTargetType_Data_Import, itemImage, null);
            }

            if (selectImporterDialog.ShowDialog() == DialogResult.OK)
            {
                var importerName = selectImporterDialog.SelectedItemTypeName;
                return importers.First(i => i.Name == importerName);
            }

            return null;
        }

        private IActivityRunner ActivityRunner
        {
            get
            {
                return gui.ActivityRunner;
            }
        }

        private void ImportToItem(object item)
        {
            var importer = GetSupportedImporterForTargetType(item);
            if (importer == null)
            {
                return;
            }

            ConfigureImporterAndRun(importer, item);
        }

        private void ConfigureImporterAndRun(IFileImporter importer, object target)
        {
            using (var view = gui.DocumentViewsResolver.CreateViewForData(importer))
            {
                if (view == null)
                {
                    GetImportedItemsUsingFileOpenDialog(importer, target);
                    return;
                }

                var importerDialog = view as IDialog;
                if (importerDialog == null)
                {
                    return;
                }

                if (importerDialog.ShowModal() != DialogResult.OK)
                {
                    return;
                }

                //TODO : move to view provider..when the view is create the importer is there
                if (importerDialog is IConfigureDialog)
                {
                    ((IConfigureDialog) (importerDialog)).Configure(importer);
                }
            }

            var importActivity = new FileImportActivity(importer, target);

            importActivity.OnImportFinished += ImportActivityOnImportFinished;

            ActivityRunner.Enqueue(importActivity);
        }

        private void ImportActivityOnImportFinished(FileImportActivity fileImportActivity, object importedObject, IFileImporter importer)
        {
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            if (importer.OpenViewAfterImport)
            {
                OpenViewForImportedObject(importedObject);
            }
        }

        [InvokeRequired]
        private void OpenViewForImportedObject(object importedObject)
        {
            gui.Selection = importedObject;
            gui.CommandHandler.OpenViewForSelection();
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

            var importActivity = new FileImportActivity(importer, target)
            {
                Files = dialog.FileNames.ToArray()
            };

            importActivity.OnImportFinished += ImportActivityOnImportFinished;
            ActivityRunner.Enqueue(importActivity);
        }
    }
}