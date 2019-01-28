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

namespace Ringtoets.Common.Data.Hydraulics
{
    /// <summary>
    /// Extension methods for <see cref="HydraulicBoundaryDatabase"/>.
    /// </summary>
    public static class HydraulicBoundaryDatabaseExtensions
    {
        /// <summary>
        /// Checks whether the hydraulic boundary database is linked to a database file.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabase">The hydraulic boundary database to check
        /// for being linked.</param>
        /// <returns><c>true</c> if the hydraulic boundary database is linked;
        /// <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when
        /// <paramref name="hydraulicBoundaryDatabase"/> is <c>null</c>.</exception>
        public static bool IsLinked(this HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            if (hydraulicBoundaryDatabase == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryDatabase));
            }

            return !string.IsNullOrEmpty(hydraulicBoundaryDatabase.FilePath);
        }

        /// <summary>
        /// Gets the preprocessor directory to be used during Hydra-Ring calculations.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabase">The hydraulic boundary database to get the
        /// effective preprocessor directory from.</param>
        /// <returns>A preprocessor directory, which is <see cref="string.Empty"/> when
        /// <see cref="HydraulicBoundaryDatabase.CanUsePreprocessor"/> or
        /// <see cref="HydraulicBoundaryDatabase.UsePreprocessor"/> is <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when
        /// <paramref name="hydraulicBoundaryDatabase"/> is <c>null</c>.</exception>
        public static string EffectivePreprocessorDirectory(this HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            if (hydraulicBoundaryDatabase == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryDatabase));
            }

            return hydraulicBoundaryDatabase.CanUsePreprocessor && hydraulicBoundaryDatabase.UsePreprocessor
                       ? hydraulicBoundaryDatabase.PreprocessorDirectory
                       : string.Empty;
        }
    }
}