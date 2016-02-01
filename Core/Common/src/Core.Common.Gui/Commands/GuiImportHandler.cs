// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights reserved.

using System.Linq;
using System.Windows.Forms;

using Core.Common.Base.IO;
using Core.Common.Base.Plugin;
using Core.Common.Base.Service;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Forms.ProgressDialog;
using Core.Common.Gui.Properties;

using log4net;

namespace Core.Common.Gui.Commands
{
    /// <summary>
    /// Class responsible for import handling.
    /// </summary>
    public class GuiImportHandler
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GuiImportHandler));

        private readonly IWin32Window dialogParent;
        private readonly ApplicationCore applicationCore;

        public GuiImportHandler(IWin32Window dialogParent, ApplicationCore applicationCore)
        {
            this.dialogParent = dialogParent;
            this.applicationCore = applicationCore;
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
            var selectImporterDialog = new SelectItemDialog(dialogParent);

            var importers = applicationCore.GetSupportedFileImporters(target);
            //if there is only one available exporter use that.
            if (!importers.Any())
            {
                MessageBox.Show(Resources.GuiImportHandler_GetSupportedImporterForTargetType_No_importer_available_for_this_item, Resources.GuiImportHandler_GetSupportedImporterForTargetType_Error);
                log.ErrorFormat(Resources.GuiImportHandler_GetSupportedImporterForTargetType_No_importer_available_for_this_item_0_, target);
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

            if (dialog.ShowDialog(dialogParent) != DialogResult.OK)
            {
                return;
            }

            log.Info(Resources.GuiImportHandler_GetImportedItemsUsingFileOpenDialog_Start_importing_data);

            ActivityProgressDialogRunner.Run(dialogParent, dialog.FileNames.Select(f => new FileImportActivity(importer, target, f)));
        }
    }
}