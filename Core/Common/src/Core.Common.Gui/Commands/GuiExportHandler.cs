// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.IO;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.Properties;
using Core.Common.Util.Reflection;
using log4net;

namespace Core.Common.Gui.Commands
{
    /// <summary>
    /// Class responsible for handling export workflow with user interaction.
    /// </summary>
    public class GuiExportHandler : IExportCommandHandler
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GuiExportHandler));

        private readonly IWin32Window dialogParent;
        private readonly IEnumerable<ExportInfo> exportInfos;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuiExportHandler"/> class.
        /// </summary>
        /// <param name="dialogParent">The parent window to show dialogs on top.</param>
        /// <param name="exportInfos">An enumeration of <see cref="ExportInfo"/>.</param>
        public GuiExportHandler(IWin32Window dialogParent, IEnumerable<ExportInfo> exportInfos)
        {
            this.dialogParent = dialogParent;
            this.exportInfos = exportInfos;
        }

        public bool CanExportFrom(object source)
        {
            return GetSupportedExportInfos(source).Any();
        }

        public void ExportFrom(object source)
        {
            ExportInfo exportInfo = GetSupportedExportInfoUsingDialog(source);
            if (exportInfo == null)
            {
                return;
            }

            ExportItem(exportInfo, source);
        }

        private IEnumerable<ExportInfo> GetSupportedExportInfos(object source)
        {
            if (source == null)
            {
                return Enumerable.Empty<ExportInfo>();
            }

            Type sourceType = source.GetType();

            return exportInfos.Where(info => (info.DataType == sourceType || sourceType.Implements(info.DataType)) && info.IsEnabled(source));
        }

        private ExportInfo GetSupportedExportInfoUsingDialog(object source)
        {
            ExportInfo[] supportedExportInfos = GetSupportedExportInfos(source).ToArray();

            if (supportedExportInfos.Length == 0)
            {
                MessageBox.Show(Resources.GuiExportHandler_GetSupportedExporterForItemUsingDialog_No_exporter_for_this_item_available,
                                Resources.GuiExportHandler_GetSupportedExporterForItemUsingDialog_Error);
                string itemToExportType = source == null ? "null" : source.GetType().FullName;
                log.Warn(string.Format(CultureInfo.CurrentCulture,
                                       Resources.GuiExportHandler_GetSupportedExporterForItemUsingDialog_No_exporter_for_this_item_0_available,
                                       itemToExportType));
                return null;
            }

            if (supportedExportInfos.Length == 1)
            {
                return supportedExportInfos[0];
            }

            using (var selectExportInfoDialog = new SelectItemDialog(dialogParent, Resources.GuiExportHandler_GetSupportedExportInfoUsingDialog_Select_exporter))
            {
                foreach (ExportInfo exportInfo in supportedExportInfos)
                {
                    selectExportInfoDialog.AddItemType(GetItemName(exportInfo),
                                                       exportInfo.Category,
                                                       exportInfo.Image,
                                                       exportInfo);
                }

                if (selectExportInfoDialog.ShowDialog() == DialogResult.OK)
                {
                    return (ExportInfo) selectExportInfoDialog.SelectedItemTag;
                }
            }

            return null;
        }

        private static string GetItemName(ExportInfo exportInfo)
        {
            return exportInfo.Extension != null
                       ? string.Format(Resources.GetItemName_Name_0_FileExtension_1, exportInfo.Name, exportInfo.Extension)
                       : exportInfo.Name;
        }

        private static void ExportItem(ExportInfo exportInfo, object source)
        {
            string exportFilePath = exportInfo.GetExportPath();

            if (exportFilePath != null)
            {
                log.InfoFormat(Resources.GuiExportHandler_ExportItemUsingDialog_Start_exporting_DataType_0_,
                               exportInfo.Name);

                IFileExporter exporter = exportInfo.CreateFileExporter(source, exportFilePath);

                if (exporter.Export())
                {
                    log.InfoFormat(Resources.GuiExportHandler_ExportItemUsingDialog_Data_exported_to_File_0, exportFilePath);
                    log.InfoFormat(Resources.GuiExportHandler_ExportItemUsingDialog_Export_of_DataType_0_successful,
                                   exportInfo.Name);
                }
                else
                {
                    log.ErrorFormat(Resources.GuiExportHandler_ExportItemUsingDialog_Export_of_DataType_0_failed,
                                    exportInfo.Name);
                }
            }
        }
    }
}