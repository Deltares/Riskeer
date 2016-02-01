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

using Core.Common.Base.IO;

namespace Core.Common.Gui
{
    /// <summary>
    /// Interface declaring commands/methods related to importing and exporting data.
    /// </summary>
    public interface IExportImportCommandHandler
    {
        /// <summary>
        /// Indicates if there are importers for the current Gui.Selection
        /// </summary>
        /// <param name="obj"></param>
        bool CanImportOn(object obj);

        /// <summary>
        /// Indicates if there are exporters for the current Gui.Selection
        /// </summary>
        /// <param name="obj"></param>
        bool CanExportFrom(object obj);

        void ExportFrom(object data, IFileExporter exporter = null);

        void ImportOn(object target, IFileImporter importer = null);
    }
}