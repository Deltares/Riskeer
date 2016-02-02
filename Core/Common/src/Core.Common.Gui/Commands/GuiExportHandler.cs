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

using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Core.Common.Base.IO;
using Core.Common.Base.Plugin;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Properties;

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
        private readonly ApplicationCore applicationCore;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuiExportHandler"/> class.
        /// </summary>
        /// <param name="dialogParent">The parent window to show dialogs on top.</param>
        /// <param name="applicationCore">The application-plugins host.</param>
        public GuiExportHandler(IWin32Window dialogParent, ApplicationCore applicationCore)
        {
            this.dialogParent = dialogParent;
            this.applicationCore = applicationCore;
        }

        /// <summary>
        /// Asks the user to select which exporter to use if multiple are available. Then
        /// if an exporter is found/selected, the user is asked for a location to export to.
        /// Finally the data is being exported from the source object.
        /// </summary>
        /// <param name="item">The export source.</param>
        public void ExportFrom(object item)
        {
            var exporter = GetSupportedExporterForItemUsingDialog(item);
            if (exporter == null)
            {
                return;
            }
            GetExporterDialog(exporter, item);
        }

        /// <summary>
        /// Ask the user for the source file to import data from, then perform the import
        /// on the target object.
        /// </summary>
        /// <param name="exporter">The importer to use.</param>
        /// <param name="selectedItem">The import target.</param>
        public void GetExporterDialog(IFileExporter exporter, object selectedItem)
        {
            ExporterItemUsingFileOpenDialog(exporter, selectedItem);
        }

        private IFileExporter GetSupportedExporterForItemUsingDialog(object itemToExport)
        {
            var fileExporters = applicationCore.GetSupportedFileExporters(itemToExport).ToArray();

            if (fileExporters.Length == 0)
            {
                MessageBox.Show(Resources.GuiExportHandler_GetSupportedExporterForItemUsingDialog_No_exporter_for_this_item_available);
                log.Warn(string.Format(Resources.GuiExportHandler_GetSupportedExporterForItemUsingDialog_No_exporter_for_this_item_0_available, itemToExport.GetType()));
                return null;
            }

            // If there is only one available exporter use that:
            if (fileExporters.Length == 1)
            {
                return fileExporters[0];
            }

            using (var selectExporterDialog = new SelectItemDialog(dialogParent))
            {
                foreach (var fileExporter in fileExporters)
                {
                    selectExporterDialog.AddItemType(fileExporter.Name, fileExporter.Category, fileExporter.Image ?? brickImage, null);
                }

                if (selectExporterDialog.ShowDialog() == DialogResult.OK)
                {
                    return fileExporters.First(i => i.Name == selectExporterDialog.SelectedItemTypeName);
                }
            }
            return null;
        }

        private void ExporterItemUsingFileOpenDialog(IFileExporter exporter, object item)
        {
            log.Info(Resources.GuiExportHandler_ExporterItemUsingFileOpenDialog_Start_exporting);

            var windowTitle = string.Format(Resources.GuiExportHandler_ExporterItemUsingFileOpenDialog_Select_a_DataType_0_file_to_export_to, exporter.Name);
            using (var saveFileDialog = new SaveFileDialog
            {
                Filter = exporter.FileFilter,
                Title = windowTitle,
                FilterIndex = 2
            })
            {
                if (saveFileDialog.ShowDialog(dialogParent) == DialogResult.OK)
                {
                    if (exporter.Export(item, saveFileDialog.FileName))
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