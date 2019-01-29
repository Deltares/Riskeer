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
    /// Interface declaring commands/methods related to importing data.
    /// </summary>
    public interface IImportCommandHandler
    {
        /// <summary>
        /// Indicates if there are importers for the given target object.
        /// </summary>
        /// <param name="target">The target object to check importer availability for.</param>
        /// <returns><c>true</c> if there are importers available, <c>false</c> otherwise.</returns>
        bool CanImportOn(object target);

        /// <summary>
        /// Perform the import workflow by the following steps:
        /// <list type="number">
        /// <item>If multiple importers are available for the target object, determine
        /// which importer to use;</item>
        /// <item>Create the importer;</item>
        /// <item>Obtain data from the importer;</item>
        /// <item>Set the imported data on the target object.</item>
        /// </list>
        /// </summary>
        /// <param name="target">The data object to import to.</param>
        void ImportOn(object target);
    }
}