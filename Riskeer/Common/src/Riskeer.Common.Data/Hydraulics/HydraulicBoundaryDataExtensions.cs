// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.IO;
using System.Linq;
using Core.Common.Util.Extensions;

namespace Riskeer.Common.Data.Hydraulics
{
    /// <summary>
    /// Extension methods for <see cref="HydraulicBoundaryData"/>.
    /// </summary>
    public static class HydraulicBoundaryDataExtensions
    {
        /// <summary>
        /// Checks whether the hydraulic boundary data is linked to a hydraulic location configuration database.
        /// </summary>
        /// <param name="hydraulicBoundaryData">The hydraulic boundary data to check for being linked.</param>
        /// <returns><c>true</c> if the hydraulic boundary data is linked; <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryData"/> is <c>null</c>.</exception>
        public static bool IsLinked(this HydraulicBoundaryData hydraulicBoundaryData)
        {
            if (hydraulicBoundaryData == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryData));
            }

            return !string.IsNullOrEmpty(hydraulicBoundaryData.HydraulicLocationConfigurationDatabase.FilePath);
        }

        /// <summary>
        /// Set a new folder path for the provided <paramref name="hydraulicBoundaryData"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryData">The hydraulic boundary data to set the new folder path for.</param>
        /// <param name="newFolderPath">The new folder path to set.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void SetNewFolderPath(this HydraulicBoundaryData hydraulicBoundaryData, string newFolderPath)
        {
            if (hydraulicBoundaryData == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryData));
            }

            if (newFolderPath == null)
            {
                throw new ArgumentNullException(nameof(newFolderPath));
            }

            hydraulicBoundaryData.HydraulicLocationConfigurationDatabase.FilePath
                = GetNewFilePath(hydraulicBoundaryData.HydraulicLocationConfigurationDatabase.FilePath, newFolderPath);
            hydraulicBoundaryData.HydraulicBoundaryDatabases
                                 .ForEachElementDo(hbd => hbd.FilePath = GetNewFilePath(hbd.FilePath, newFolderPath));

            hydraulicBoundaryData.NotifyObservers();
        }

        /// <summary>
        /// Gets the locations from all <see cref="HydraulicBoundaryDatabase"/> instances. 
        /// </summary>
        /// <param name="hydraulicBoundaryData">The <see cref="HydraulicBoundaryData"/> to get the locations from.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="HydraulicBoundaryLocation"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryData"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<HydraulicBoundaryLocation> GetLocations(this HydraulicBoundaryData hydraulicBoundaryData)
        {
            if (hydraulicBoundaryData == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryData));
            }

            return hydraulicBoundaryData.HydraulicBoundaryDatabases.SelectMany(hbd => hbd.Locations);
        }

        /// <summary>
        /// Gets the hydraulic boundary database that contains the provided <paramref name="hydraulicBoundaryLocation"/>. 
        /// </summary>
        /// <param name="hydraulicBoundaryData">The <see cref="HydraulicBoundaryData"/> to get the hydraulic boundary
        /// database from.</param>
        /// <param name="hydraulicBoundaryLocation">The <see cref="HydraulicBoundaryLocation"/> to get the hydraulic boundary
        /// database for.</param>
        /// <returns>The <see cref="HydraulicBoundaryDatabase"/> that contains the provided
        /// <paramref name="hydraulicBoundaryLocation"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="hydraulicBoundaryLocation"/> is not part of
        /// <paramref name="hydraulicBoundaryData"/>.</exception>
        public static HydraulicBoundaryDatabase GetHydraulicBoundaryDatabaseForLocation(this HydraulicBoundaryData hydraulicBoundaryData,
                                                                                        HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            if (hydraulicBoundaryData == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryData));
            }

            if (hydraulicBoundaryLocation == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocation));
            }

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = hydraulicBoundaryData.HydraulicBoundaryDatabases.FirstOrDefault(hbd => hbd.Locations.Contains(hydraulicBoundaryLocation));

            if (hydraulicBoundaryDatabase == null)
            {
                throw new ArgumentException($"'{nameof(hydraulicBoundaryLocation)}' is not part of '{nameof(hydraulicBoundaryData)}'.");
            }

            return hydraulicBoundaryDatabase;
        }

        private static string GetNewFilePath(string filePath, string newFolderPath)
        {
            return Path.Combine(newFolderPath, Path.GetFileName(filePath));
        }
    }
}