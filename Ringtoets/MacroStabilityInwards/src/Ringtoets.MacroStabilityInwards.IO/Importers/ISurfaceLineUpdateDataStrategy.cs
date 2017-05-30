// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.IO.Importers
{
    /// <summary>
    /// Interface describing the method of updating the data model after new surface lines 
    /// have been imported.
    /// </summary>
    public interface ISurfaceLineUpdateDataStrategy
    {
        /// <summary>
        /// Adds the imported data to the <paramref name="targetDataCollection"/>.
        /// </summary>
        /// <param name="targetDataCollection">The target collection which needs to be updated.</param>
        /// <param name="readSurfaceLines">The imported surface lines.</param>
        /// <param name="sourceFilePath">The source path from where the surface lines were imported from.</param>
        /// <returns>An <see cref="IEnumerable{IObservable}"/> of updated instances.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        /// <exception cref="UpdateDataException">Thrown when applying the strategy has failed. The 
        /// <see cref="UpdateDataException.InnerException"/> is set with a more detailed explanation
        /// of why the exception occurs.</exception>
        IEnumerable<IObservable> UpdateSurfaceLinesWithImportedData(
            RingtoetsMacroStabilityInwardsSurfaceLineCollection targetDataCollection,
            IEnumerable<RingtoetsMacroStabilityInwardsSurfaceLine> readSurfaceLines,
            string sourceFilePath);
    }
}