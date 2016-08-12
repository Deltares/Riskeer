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
    /// Class responsible for handling export workflow with user interaction.
    /// </summary>
    public class GuiExportHandler : IExportCommandHandler
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

        public bool CanExportFrom(object source)
        {
            return GetSupportedExportInfos(source).Any();
        }

        public void ExportFrom(object source)
        {
            var exportInfo = GetSupportedExportInfoUsingDialog(source);
            if (exportInfo == null)
            {
                return;
            }

            ExportItemUsingFileOpenDialog(exportInfo, source);
        }

        private IEnumerable<ExportInfo> GetSupportedExportInfos(object source)
        {
            if (source == null)
            {
                return Enumerable.Empty<ExportInfo>();
            }

            var sourceType = source.GetType();

            return exportInfos.Where(info => (info.DataType == sourceType || sourceType.Implements(info.DataType)) && info.IsEnabled(source));
        }

        private ExportInfo GetSupportedExportInfoUsingDialog(object source)
        {
            var supportedExportInfos = GetSupportedExportInfos(source).ToArray();

            if (supportedExportInfos.Length == 0)
            {
                MessageBox.Show(Resources.GuiExportHandler_GetSupportedExporterForItemUsingDialog_No_exporter_for_this_item_available,
                                Resources.GuiExportHandler_GetSupportedExporterForItemUsingDialog_Error);
                var itemToExportType = source == null ? "null" : source.GetType().FullName;
                log.Warn(string.Format(Resources.GuiExportHandler_GetSupportedExporterForItemUsingDialog_No_exporter_for_this_item_0_available, itemToExportType));
                return null;
            }

            if (supportedExportInfos.Length == 1)
            {
                return supportedExportInfos[0];
            }

            using (var selectExportInfoDialog = new SelectItemDialog(dialogParent, Resources.GuiExportHandler_GetSupportedExportInfoUsingDialog_Select_exporter))
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

        private void ExportItemUsingFileOpenDialog(ExportInfo exportInfo, object source)
        {
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
                    log.Info(Resources.GuiExportHandler_ExporterItemUsingFileOpenDialog_Start_exporting);

                    var exporter = exportInfo.CreateFileExporter(source, saveFileDialog.FileName);

                    if (exporter.Export())
                    {
                        log.Info(Resources.GuiExportHandler_ExporterItemUsingFileOpenDialog_Finished_exporting);
                    }
                    else
                    {
                        log.Error(Resources.GuiExportHandler_ExporterItemUsingFileOpenDialog_Export_failed);
                    }
                }
            }
        }
    }
}