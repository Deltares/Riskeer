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
using System.Linq;
using System.Windows.Forms;

using Core.Common.Base.IO;
using Core.Common.Base.Plugin;
using Core.Common.Gui.Properties;

using log4net;

namespace Core.Common.Gui.Commands
{
    /// <summary>
    /// Class responsible for exporting and importing data.
    /// </summary>
    public class ExportImportCommandHandler : IExportImportCommandHandler
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ExportImportCommandHandler));

        private readonly ApplicationCore applicationCore;

        private readonly GuiImportHandler importHandler;
        private readonly GuiExportHandler exportHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportImportCommandHandler"/> class.
        /// </summary>
        /// <param name="dialogParent">The parent window onto which dialogs should be shown.</param>
        /// <param name="applicationCore">The host of all application plugins.</param>
        public ExportImportCommandHandler(IWin32Window dialogParent, ApplicationCore applicationCore)
        {
            this.applicationCore = applicationCore;
            importHandler = new GuiImportHandler(dialogParent, this.applicationCore);
            exportHandler = new GuiExportHandler(dialogParent, this.applicationCore);
        }

        public bool CanImportOn(object obj)
        {
            return applicationCore.GetSupportedFileImporters(obj).Any();
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
            return applicationCore.GetSupportedFileExporters(obj).Any();
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
    }
}