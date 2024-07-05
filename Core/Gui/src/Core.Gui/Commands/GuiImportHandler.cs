﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.IO;
using Core.Common.Base.Service;
using Core.Common.Util.Reflection;
using Core.Gui.Forms;
using Core.Gui.Forms.ProgressDialog;
using Core.Gui.Helpers;
using Core.Gui.Plugin;
using Core.Gui.Properties;
using log4net;

namespace Core.Gui.Commands
{
    /// <summary>
    /// Class responsible for handling import workflow with user interaction.
    /// </summary>
    public class GuiImportHandler : IImportCommandHandler
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GuiImportHandler));

        private readonly IViewParent dialogParent;
        private readonly IEnumerable<ImportInfo> importInfos;
        private readonly IInquiryHelper inquiryHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuiImportHandler"/> class.
        /// </summary>
        /// <param name="dialogParent">The parent window to show dialogs on top.</param>
        /// <param name="importInfos">An enumeration of <see cref="ImportInfo"/>.</param>
        /// <param name="inquiryHelper">Helper responsible for performing information inquiries.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public GuiImportHandler(IViewParent dialogParent, IEnumerable<ImportInfo> importInfos, IInquiryHelper inquiryHelper)
        {
            if (dialogParent == null)
            {
                throw new ArgumentNullException(nameof(dialogParent));
            }

            if (importInfos == null)
            {
                throw new ArgumentNullException(nameof(importInfos));
            }

            if (inquiryHelper == null)
            {
                throw new ArgumentNullException(nameof(inquiryHelper));
            }

            this.dialogParent = dialogParent;
            this.importInfos = importInfos;
            this.inquiryHelper = inquiryHelper;
        }

        public IEnumerable<ImportInfo> GetSupportedImportInfos(object target)
        {
            if (target == null)
            {
                return Enumerable.Empty<ImportInfo>();
            }

            Type targetType = target.GetType();

            return importInfos.Where(info => (info.DataType == targetType || targetType.Implements(info.DataType))
                                             && (info.IsEnabled == null || info.IsEnabled(target)));
        }

        public void ImportOn(object target, IEnumerable<ImportInfo> supportedImportInfos)
        {
            ImportInfo importInfo = SelectImportInfo(target, supportedImportInfos.ToArray());

            if (importInfo == null)
            {
                return;
            }

            ImportItemsUsingDialog(importInfo, target);
        }

        private ImportInfo SelectImportInfo(object target, IReadOnlyList<ImportInfo> supportedImportInfos)
        {
            if (supportedImportInfos.Count == 0)
            {
                MessageBox.Show(Resources.GuiImportHandler_SelectImportInfo_No_importer_available_for_this_item,
                                Resources.GuiImportHandler_SelectImportInfo_Error);
                log.ErrorFormat(Resources.GuiImportHandler_SelectImportInfo_No_importer_available_for_this_item_0_,
                                target);
                return null;
            }

            if (supportedImportInfos.Count == 1)
            {
                return supportedImportInfos[0];
            }

            using (var selectImporterDialog = new SelectItemDialog(dialogParent, Resources.GuiImportHandler_GetSupportedImporterUsingDialog_Select_importer))
            {
                foreach (ImportInfo importInfo in supportedImportInfos)
                {
                    selectImporterDialog.AddItemType(GetItemName(importInfo),
                                                     importInfo.Category,
                                                     importInfo.Image,
                                                     importInfo);
                }

                if (selectImporterDialog.ShowDialog() == DialogResult.OK)
                {
                    return (ImportInfo) selectImporterDialog.SelectedItemTag;
                }
            }

            return null;
        }

        private static string GetItemName(ImportInfo importInfo)
        {
            return importInfo.FileFilterGenerator != null
                       ? string.Format(Resources.GetItemName_Name_0_FileExtension_1, importInfo.Name, importInfo.FileFilterGenerator.Extension)
                       : importInfo.Name;
        }

        private void ImportItemsUsingDialog(ImportInfo importInfo, object target)
        {
            string fileDialogResult = inquiryHelper.GetSourceFileLocation(importInfo.FileFilterGenerator.Filter);

            if (fileDialogResult != null && importInfo.VerifyUpdates(target))
            {
                RunImportActivity(importInfo.CreateFileImporter(target, fileDialogResult), importInfo.Name);
            }
            else
            {
                log.InfoFormat(Resources.GuiImportHandler_ImportItemsUsingDialog_Importing_cancelled);
            }
        }

        private void RunImportActivity(IFileImporter importer, string importName)
        {
            var activity = new FileImportActivity(importer,
                                                  !string.IsNullOrEmpty(importName)
                                                      ? string.Format(Resources.GuiImportHandler_RunImportActivity_Importing_0_, importName)
                                                      : string.Empty);

            ActivityProgressDialogRunner.Run(dialogParent, activity);
        }
    }
}