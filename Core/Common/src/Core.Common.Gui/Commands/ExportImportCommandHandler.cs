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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.IO;
using Core.Common.Gui.Properties;
using Core.Common.Utils.Reflection;
using log4net;

namespace Core.Common.Gui.Commands
{
    /// <summary>
    /// Class responsible for exporting and importing data.
    /// </summary>
    public class ExportImportCommandHandler : IExportImportCommandHandler
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ExportImportCommandHandler));

        private readonly IEnumerable<IFileImporter> fileImporters;
        private readonly IEnumerable<IFileExporter> fileExporters;
        private readonly GuiImportHandler importHandler;
        private readonly GuiExportHandler exportHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportImportCommandHandler"/> class.
        /// </summary>
        /// <param name="dialogParent">The parent window onto which dialogs should be shown.</param>
        /// <param name="fileImporters">An enumeration of <see cref="IFileImporter"/>.</param>
        /// <param name="fileExporters">An enumeration of <see cref="IFileExporter"/>.</param>
        public ExportImportCommandHandler(IWin32Window dialogParent, IEnumerable<IFileImporter> fileImporters, IEnumerable<IFileExporter> fileExporters)
        {
            this.fileImporters = fileImporters;
            this.fileExporters = fileExporters;
            importHandler = new GuiImportHandler(dialogParent, this.fileImporters);
            exportHandler = new GuiExportHandler(dialogParent, this.fileExporters);
        }

        public bool CanImportOn(object target)
        {
            return fileImporters.Any(fileImporter => fileImporter.CanImportOn(target));
        }

        public void ImportOn(object target, IFileImporter importer = null)
        {
            try
            {
                if (importer == null)
                {
                    importHandler.ImportDataTo(target);
                }
                else
                {
                    importHandler.ImportUsingImporter(importer, target);
                }
            }
            catch (Exception)
            {
                log.ErrorFormat(Resources.ExportImportCommandHandler_ImportOn_Unable_to_import_on_0_, target);
            }
        }

        public bool CanExportFrom(object obj)
        {
            return GetSupportedFileExporters(obj).Any();
        }

        public void ExportFrom(object data, IFileExporter exporter = null)
        {
            if (exporter == null)
            {
                exportHandler.ExportFrom(data);
            }
            else
            {
                exportHandler.GetExporterDialog(exporter, data);
            }
        }

        private IEnumerable<IFileExporter> GetSupportedFileExporters(object source)
        {
            if (source == null)
            {
                return Enumerable.Empty<IFileExporter>();
            }

            var sourceType = source.GetType();

            return fileExporters.Where(fe => (fe.SupportedItemType == sourceType || sourceType.Implements(fe.SupportedItemType)));
        }
    }
}