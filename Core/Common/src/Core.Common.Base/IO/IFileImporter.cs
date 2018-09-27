// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

namespace Core.Common.Base.IO
{
    /// <summary>
    /// Interface for data import from external formats.
    /// </summary>
    public interface IFileImporter
    {
        /// <summary>
        /// Sets the action to perform when progress has changed.
        /// </summary>
        void SetProgressChanged(OnProgressChanged action);

        /// <summary>
        /// This method imports the data to an item from a file at the given location.
        /// </summary>
        /// <returns><c>true</c> if the import was successful. <c>false</c> otherwise.</returns>
        /// <remarks>Implementations of this import method are allowed to throw exceptions of any kind.</remarks>
        bool Import();

        /// <summary>
        /// This method cancels an import.
        /// </summary>
        void Cancel();

        /// <summary>
        /// Notifies all observers of <see cref="IObservable"/> instances that have been
        /// changed during the import.
        /// </summary>
        /// <remarks>This method should be called by caller who calls <see cref="Cancel"/>
        /// on this importer.</remarks>
        void DoPostImport();
    }
}