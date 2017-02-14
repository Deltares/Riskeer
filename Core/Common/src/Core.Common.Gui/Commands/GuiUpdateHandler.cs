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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.IO;
using Core.Common.Base.Service;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Forms.ProgressDialog;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.Properties;
using Core.Common.Utils.Reflection;
using log4net;

namespace Core.Common.Gui.Commands
{
    /// <summary>
    /// Class responsible for handling import workflow with user interaction.
    /// </summary>
    public class GuiUpdateHandler : IUpdateCommandHandler
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GuiImportHandler));

        private readonly IWin32Window dialogParent;
        private readonly IEnumerable<UpdateInfo> updateInfos;
        private readonly IInquiryHelper inquiryHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuiImportHandler"/> class.
        /// </summary>
        /// <param name="dialogParent">The parent window to show dialogs on top.</param>
        /// <param name="updateInfos">An enumeration of <see cref="UpdateInfo"/>.</param>
        /// <param name="inquiryHelper">Helper responsible for performing information inquiries.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public GuiUpdateHandler(IWin32Window dialogParent, IEnumerable<UpdateInfo> updateInfos, IInquiryHelper inquiryHelper)
        {
            if (dialogParent == null)
            {
                throw new ArgumentNullException(nameof(dialogParent));
            }
            if (updateInfos == null)
            {
                throw new ArgumentNullException(nameof(updateInfos));
            }
            if (inquiryHelper == null)
            {
                throw new ArgumentNullException(nameof(inquiryHelper));
            }
            this.dialogParent = dialogParent;
            this.updateInfos = updateInfos;
            this.inquiryHelper = inquiryHelper;
        }

        public bool CanUpdateOn(object target)
        {
            return GetSupportedUpdateInfos(target).Any();
        }

        public void UpdateOn(object target)
        {
            UpdateInfo updateInfo = GetSupportedUpdaterUsingDialog(target);
            if (updateInfo != null)
            {
                UpdateItemsUsingDialog(updateInfo, target);
            }
        }

        private IEnumerable<UpdateInfo> GetSupportedUpdateInfos(object target)
        {
            if (target == null)
            {
                return Enumerable.Empty<UpdateInfo>();
            }

            var targetType = target.GetType();

            return updateInfos.Where(info => (info.DataType == targetType || targetType.Implements(info.DataType)) && info.IsEnabled(target));
        }

        private UpdateInfo GetSupportedUpdaterUsingDialog(object target)
        {
            UpdateInfo[] supportedUpdateInfo = GetSupportedUpdateInfos(target).ToArray();
            if (supportedUpdateInfo.Length == 0)
            {
                MessageBox.Show(dialogParent,
                                Resources.GuiUpdateHandler_GetSupportedUpdaterForTargetType_No_updater_available_for_this_item,
                                Resources.GuiUpdateHandler_GetSupportedUpdaterForTargetType_Error);
                log.ErrorFormat(Resources.GuiUpdateHandler_GetSupportedUpdaterForTargetType_No_updater_available_for_this_item_0_,
                                target);
                return null;
            }

            if (supportedUpdateInfo.Length == 1)
            {
                return supportedUpdateInfo[0];
            }

            using (var selectUpdaterDialog = new SelectItemDialog(dialogParent, Resources.GuiUpdateHandler_GetSupportedUpdaterUsingDialog_Select_updater))
            {
                foreach (UpdateInfo updateInfo in supportedUpdateInfo)
                {
                    string category = string.IsNullOrEmpty(updateInfo.Category) ?
                                          Resources.GuiUpdateHandler_GetSupportedUpdaterForTargetType_Data_Update :
                                          updateInfo.Category;
                    Image itemImage = updateInfo.Image ?? Resources.brick;

                    selectUpdaterDialog.AddItemType(updateInfo.Name, category, itemImage, null);
                }

                if (selectUpdaterDialog.ShowDialog() == DialogResult.OK)
                {
                    return supportedUpdateInfo.First(i => i.Name == selectUpdaterDialog.SelectedItemTypeName);
                }
            }

            return null;
        }

        private void UpdateItemsUsingDialog(UpdateInfo updateInfo, object target)
        {
            string filePath = updateInfo.CurrentPath(target);
            if (!File.Exists(filePath))
            {
                FileResult fileDialogResult = inquiryHelper.GetSourceFileLocation(updateInfo.FileFilter);
                filePath = fileDialogResult.FilePath;
            }
            if (filePath != null && updateInfo.VerifyUpdates(target))
            {
                RunUpdateActivity(updateInfo.CreateFileImporter(target, filePath), updateInfo.Name);
            }
        }

        private void RunUpdateActivity(IFileImporter importer, string importName)
        {
            log.Info(Resources.GuiImportHandler_ImportItemsUsingDialog_Start_importing_data);

            var activity = new FileImportActivity(importer, importName ?? string.Empty);
            ActivityProgressDialogRunner.Run(dialogParent, activity);
        }
    }
}