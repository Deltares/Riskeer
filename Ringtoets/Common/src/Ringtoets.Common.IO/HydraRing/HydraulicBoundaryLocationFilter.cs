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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.IO;
using Ringtoets.Common.Data.Hydraulics;

namespace Ringtoets.Common.IO.HydraRing
{
    /// <summary>
    /// This class allows for filtering out <see cref="HydraulicBoundaryLocation"/> based
    /// on their name.
    /// </summary>
    public class HydraulicBoundaryLocationFilter
    {
        private readonly List<long> locationsToFilterOut;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryLocationFilter"/>, which uses the settings
        /// database at <paramref name="databaseFilePath"/> as its source.
        /// </summary>
        /// <param name="databaseFilePath">The path to the settings database containing a table
        /// with filtered locations.</param>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>The <paramref name="databaseFilePath"/> contains invalid characters.</item>
        /// <item>No file could be found at <paramref name="databaseFilePath"/>.</item>
        /// <item>Unable to open database file.</item>
        /// <item>Unable to read required data from database file.</item>
        /// </list>
        /// </exception>
        public HydraulicBoundaryLocationFilter(string databaseFilePath)
        {
            using (var reader = new HydraRingSettingsDatabaseReader(databaseFilePath))
            {
                List<long> filterList = reader.ReadExcludedLocations().ToList();
                filterList.Sort();
                locationsToFilterOut = filterList;
            }
        }

        /// <summary>
        /// Indicates if the <see cref="HydraulicBoundaryLocation"/> with <see cref="HydraulicBoundaryLocation.Name"/>
        /// should be imported.
        /// </summary>
        /// <param name="locationId">The name of the location.</param>
        /// <returns><c>true</c> if the location should be imported, <c>false</c> otherwise.</returns>
        public bool ShouldInclude(long locationId)
        {
            int matchingIndex = locationsToFilterOut.BinarySearch(locationId);
            return matchingIndex < 0;
        }
    }
}