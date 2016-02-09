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

using Core.Common.Base.IO;

namespace Core.Common.Gui.Commands
{
    /// <summary>
    /// Interface declaring commands/methods related to importing and exporting data.
    /// </summary>
    public interface IExportImportCommandHandler
    {
        /// <summary>
        /// Indicates if there are importers for the given object.
        /// </summary>
        /// <param name="obj">The object to check importer availability for.</param>
        bool CanImportOn(object obj);

        /// <summary>
        /// Perform the import workflow by:
        /// <list type="number">
        /// <item>If multiple importers are available for target data object, ask the user
        /// which importer to use;</item>
        /// <item>Ask the user which file to use to import from;</item>
        /// <item>Import from the user specified file to the target data object.</item>
        /// </list>
        /// </summary>
        /// <param name="target">The import target.</param>
        /// <param name="importer">Optional: The specific importer to use.</param>
        void ImportOn(object target, IFileImporter importer = null);

        /// <summary>
        /// Indicates if there are exporters for the given object.
        /// </summary>
        /// <param name="obj">The object to check exporter availability for.</param>
        bool CanExportFrom(object obj);

        /// <summary>
        /// Perform the export workflow by:
        /// <list type="number">
        /// <item>If multiple exporters are available for target data object, ask the user
        /// which exporter to use;</item>
        /// <item>Ask the user which file or file-destination to export to;</item>
        /// <item>Export from the target data object to the specified location.</item>
        /// </list>
        /// </summary>
        /// <param name="data">The data to export.</param>
        /// <param name="exporter">Optional: The specific exporter to use.</param>
        void ExportFrom(object data, IFileExporter exporter = null);
    }
}