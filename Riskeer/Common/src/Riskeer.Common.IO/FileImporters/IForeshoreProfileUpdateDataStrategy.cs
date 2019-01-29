// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using Core.Common.Base;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Exceptions;

namespace Riskeer.Common.IO.FileImporters
{
    /// <summary>
    /// Interface describing the method of updating the data model after new 
    /// foreshore profiles have been imported.
    /// </summary>
    public interface IForeshoreProfileUpdateDataStrategy
    {
        /// <summary>
        /// Updates the foreshore profiles and its dependent data with data from <paramref name="importedDataCollection"/>.
        /// </summary>
        /// <param name="importedDataCollection">The imported foreshore profiles.</param>
        /// <param name="sourceFilePath">The source path from where the foreshore profiles were imported.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of updated instances.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        /// <exception cref="UpdateDataException">Thrown when updating the data model has failed.</exception>
        IEnumerable<IObservable> UpdateForeshoreProfilesWithImportedData(IEnumerable<ForeshoreProfile> importedDataCollection, string sourceFilePath);
    }
}