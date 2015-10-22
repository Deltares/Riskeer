using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DelftTools.Controls;
using DelftTools.Shell.Core;
using DelftTools.Shell.Core.Workflow;
using DelftTools.Shell.Gui;
using DelftTools.Utils.Aop;
using DelftTools.Utils.IO;
using DelftTools.Utils.Reflection;
using DeltaShell.Gui.Forms;
using DeltaShell.Gui.Properties;
using log4net;
using MessageBox = DelftTools.Controls.Swf.MessageBox;

namespace DeltaShell.Gui
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
            ConfigureImporterAndRun(importer, null, target);
        }

        public void ImportDataTo(object target)
        {
            ImportToItem(target);
        }

        public IFileImporter GetSupportedImporterForTargetType(object target)
        {
            var selectImporterDialog = new SelectItemDialog();

            IList<IFileImporter> importers = GetImporters(target);
            //if there is only one available exporter use that.
            if (importers.Count == 0)
            {
                MessageBox.Show(Resources.GuiImportHandler_GetSupportedImporterForTargetType_No_importer_available_for_this_item_);
                Log.ErrorFormat(Resources.GuiImportHandler_GetSupportedImporterForTargetType_No_importer_available_for_this_item___0___, target);
                return null;
            }

            //if there is only one available importer use that.
            if (importers.Count == 1)
            {
                return importers[0];
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

        public IList<IFileImporter> GetImporters(object target)
        {
            var targetType = target == null ? null : target.GetType();

            IList<IFileImporter> importers = new List<IFileImporter>();
            foreach (IFileImporter importer in gui.Application.FileImporters)
            {
                importer.TargetDataDirectory = ProjectDataDirectory;

                if (targetType == null && !importer.CanImportOnRootLevel)
                {
                    //this importer requires something to import into, but we're importing globally (into project or folder), so skip it
                    continue;
                }

                // filter importers only to those which can import into targetType
                if ((targetType == null) || (importer.SupportedItemTypes.Any(t => (t == targetType) || targetType.Implements(t)) && importer.CanImportOn(target)))
                {
                    importers.Add(importer);
                }
            }
            return importers;
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

            IList<IFileImporter> importers = GetImporters(target);

            importers =
                importers.Where(
                    importer => FileUtils.FileMatchesFileFilterByExtension(importer.FileFilter, files.First())).ToList();

            //if there is only one available importer use that.))
            if (importers.Count == 0)
            {
                return null;
            }
            if (importers.Count == 1)
            {
                return importers[0];
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

        private string ProjectDataDirectory
        {
            get
            {
                return Path.GetDirectoryName(gui.Application.ProjectFilePath);
            }
        }

        private IActivityRunner ActivityRunner
        {
            get
            {
                return gui.Application.ActivityRunner;
            }
        }

        private void ImportToItem(object item)
        {
            var importer = GetSupportedImporterForTargetType(item);
            if (importer == null)
            {
                return;
            }

            ConfigureImporterAndRun(importer, null, item);
        }

        private void ConfigureImporterAndRun(IFileImporter importer, IProjectItem importedItemOwner, object target)
        {
            using (var view = gui.DocumentViewsResolver.CreateViewForData(importer))
            {
                if (view == null)
                {
                    GetImportedItemsUsingFileOpenDialog(importer, importedItemOwner, target);
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

            var importActivity = new FileImportActivity(importer, target)
            {
                ImportedItemOwner = importedItemOwner
            };

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
        private void GetImportedItemsUsingFileOpenDialog(IFileImporter importer, IProjectItem importedItemOwner, object target)
        {
            var dialog = new OpenFileDialog
            {
                Filter = importer.FileFilter,
                Multiselect = true,
                Title = Resources.GuiImportHandler_GetImportedItemsUsingFileOpenDialog_Select_a_file_to_import_from,
                RestoreDirectory = true
            };

            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            Log.Info(Resources.GuiImportHandler_GetImportedItemsUsingFileOpenDialog_Start_importing_data);

            var importActivity = new FileImportActivity(importer, target)
            {
                Files = dialog.FileNames.ToArray(),
                ImportedItemOwner = importedItemOwner
            };

            importActivity.OnImportFinished += ImportActivityOnImportFinished;
            ActivityRunner.Enqueue(importActivity);
        }
    }
}