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

namespace Core.Common.Gui.Commands
{
    /// <summary>
    /// Interface declaring commands/methods related to exporting data.
    /// </summary>
    public interface IExportCommandHandler
    {
        /// <summary>
        /// Indicates if there are exporters for the given source object.
        /// </summary>
        /// <param name="source">The source object to check exporter availability for.</param>
        /// <returns><c>true</c> if there are exporters available, <c>false</c> otherwise.</returns>
        bool CanExportFrom(object source);

        /// <summary>
        /// Perform the export workflow by the following steps:
        /// <list type="number">
        /// <item>If multiple exporters are available for the source object, determine
        /// which exporter to use;</item>
        /// <item>Create the exporter;</item>
        /// <item>Use the exporter to export from the source object.</item>
        /// </list>
        /// </summary>
        /// <param name="source">The data object to export from.</param>
        void ExportFrom(object source);
    }
}