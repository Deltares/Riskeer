﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.IO;
using Core.Common.Base.Service;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Forms.ProgressDialog;
using Core.Common.Gui.Properties;
using log4net;

namespace Core.Common.Gui.Commands
{
    /// <summary>
    /// Class responsible for handling import workflow with user interaction.
    /// </summary>
    public class GuiImportHandler
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GuiImportHandler));

        private readonly IWin32Window dialogParent;
        private readonly IEnumerable<IFileImporter> fileImporters;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuiImportHandler"/> class.
        /// </summary>
        /// <param name="dialogParent">The parent window to show dialogs on top.</param>
        /// <param name="fileImporters">An enumeration of <see cref="IFileImporter"/>.</param>
        public GuiImportHandler(IWin32Window dialogParent, IEnumerable<IFileImporter> fileImporters)
        {
            this.dialogParent = dialogParent;
            this.fileImporters = fileImporters;
        }

        /// <summary>
        /// Asks the user to select which importer to use if multiple are available. Then
        /// if an importer is found/selected, the user is asked for a source to import from.
        /// Finally the data is being imported to the target object.
        /// </summary>
        /// <param name="target">The import target.</param>
        public void ImportDataTo(object target)
        {
            ImportToItem(target);
        }

        private IFileImporter GetSupportedImporterForTargetType(object target)
        {
            var importers = fileImporters.Where(fileImporter => fileImporter.CanImportOn(target)).ToArray();
            if (!importers.Any())
            {
                MessageBox.Show(Resources.GuiImportHandler_GetSupportedImporterForTargetType_No_importer_available_for_this_item,
                                Resources.GuiImportHandler_GetSupportedImporterForTargetType_Error);
                log.ErrorFormat(Resources.GuiImportHandler_GetSupportedImporterForTargetType_No_importer_available_for_this_item_0_,
                                target);
                return null;
            }

            // If there is only one available importer use that:
            if (importers.Length == 1)
            {
                return importers[0];
            }

            using (var selectImporterDialog = new SelectItemDialog(dialogParent))
            {
                foreach (IFileImporter importer in importers)
                {
                    var category = string.IsNullOrEmpty(importer.Category) ? Resources.GuiImportHandler_GetSupportedImporterForTargetType_Data_Import : importer.Category;
                    var itemImage = importer.Image ?? Resources.brick;

                    selectImporterDialog.AddItemType(importer.Name, category, itemImage, null);
                }

                if (selectImporterDialog.ShowDialog() == DialogResult.OK)
                {
                    return importers.First(i => i.Name == selectImporterDialog.SelectedItemTypeName);
                }
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

        private void GetImportedItemsUsingFileOpenDialog(IFileImporter importer, object target)
        {
            var windowTitle = string.Format(Resources.GuiImportHandler_GetImportedItemsUsingFileOpenDialog_Select_a_DataType_0_file_to_import_from, importer.Name);
            using (var dialog = new OpenFileDialog
            {
                Filter = importer.FileFilter,
                Multiselect = true,
                Title = windowTitle,
                RestoreDirectory = true
            })
            {
                if (dialog.ShowDialog(dialogParent) == DialogResult.OK)
                {
                    log.Info(Resources.GuiImportHandler_GetImportedItemsUsingFileOpenDialog_Start_importing_data);

                    ActivityProgressDialogRunner.Run(dialogParent, dialog.FileNames.Select(f => new FileImportActivity(importer, target, f)).ToList());
                }
            }
        }
    }
}