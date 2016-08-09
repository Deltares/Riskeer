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

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.Properties;
using Core.Common.Utils.Reflection;
using log4net;

namespace Core.Common.Gui.Commands
{
    /// <summary>
    /// Class responsible for data export commands.
    /// </summary>
    public class GuiExportHandler
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GuiExportHandler));
        private static readonly Bitmap brickImage = Resources.brick;

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

        /// <summary>
        /// Asks the user to select which exporter to use if multiple are available. Then
        /// if an exporter is found/selected, the user is asked for a location to export to.
        /// Finally the data is being exported from the source object.
        /// </summary>
        /// <param name="item">The export source.</param>
        public void ExportFrom(object item)
        {
            var exportInfo = GetSupportedExportInfoUsingDialog(item);
            if (exportInfo == null)
            {
                return;
            }

            GetExporterDialog(exportInfo, item);
        }

        /// <summary>
        /// Asks the user for the target file to export data to, then performs the export
        /// using the source object.
        /// </summary>
        /// <param name="exportInfo">The export information to use.</param>
        /// <param name="selectedItem">The export source.</param>
        public void GetExporterDialog(ExportInfo exportInfo, object selectedItem)
        {
            ExportItemUsingFileOpenDialog(exportInfo, selectedItem);
        }

        private ExportInfo GetSupportedExportInfoUsingDialog(object itemToExport)
        {
            var supportedExportInfos = GetSupportedExportInfos(itemToExport).ToArray();

            if (supportedExportInfos.Length == 0)
            {
                MessageBox.Show(Resources.GuiExportHandler_GetSupportedExporterForItemUsingDialog_No_exporter_for_this_item_available,
                                Resources.GuiExportHandler_GetSupportedExporterForItemUsingDialog_Error);
                var itemToExportType = itemToExport == null ? "null" : itemToExport.GetType().FullName;
                log.Warn(string.Format(Resources.GuiExportHandler_GetSupportedExporterForItemUsingDialog_No_exporter_for_this_item_0_available, itemToExportType));
                return null;
            }

            if (supportedExportInfos.Length == 1)
            {
                return supportedExportInfos[0];
            }

            using (var selectExportInfoDialog = new SelectItemDialog(dialogParent))
            {
                foreach (var exportInfo in supportedExportInfos)
                {
                    selectExportInfoDialog.AddItemType(exportInfo.Name, exportInfo.Category, exportInfo.Image ?? brickImage, null);
                }

                if (selectExportInfoDialog.ShowDialog() == DialogResult.OK)
                {
                    return supportedExportInfos.First(info => info.Name == selectExportInfoDialog.SelectedItemTypeName);
                }
            }
            return null;
        }

        /// <summary>
        /// Gets an enumeration of supported <see cref="ExportInfo"/> objects for the provided <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The data object to get the supported <see cref="ExportInfo"/> objects for.</param>
        /// <returns>An enumeration of supported <see cref="ExportInfo"/> objects.</returns>
        public IEnumerable<ExportInfo> GetSupportedExportInfos(object source)
        {
            if (source == null)
            {
                return Enumerable.Empty<ExportInfo>();
            }

            var sourceType = source.GetType();

            return exportInfos.Where(info => info.DataType == sourceType || sourceType.Implements(info.DataType));
        }

        private void ExportItemUsingFileOpenDialog(ExportInfo exportInfo, object item)
        {
            log.Info(Resources.GuiExportHandler_ExporterItemUsingFileOpenDialog_Start_exporting);

            var windowTitle = string.Format(Resources.GuiExportHandler_ExporterItemUsingFileOpenDialog_Select_a_DataType_0_file_to_export_to, exportInfo.Name);
            using (var saveFileDialog = new SaveFileDialog
            {
                Filter = exportInfo.FileFilter,
                Title = windowTitle,
                FilterIndex = 2
            })
            {
                if (saveFileDialog.ShowDialog(dialogParent) == DialogResult.OK)
                {
                    var exporter = exportInfo.CreateFileExporter(item, saveFileDialog.FileName);

                    if (exporter.Export())
                    {
                        log.Info(Resources.GuiExportHandler_ExporterItemUsingFileOpenDialog_Finished_exporting);
                    }
                    else
                    {
                        log.Warn(Resources.GuiExportHandler_ExporterItemUsingFileOpenDialog_Export_failed);
                    }
                }
            }
        }
    }
}