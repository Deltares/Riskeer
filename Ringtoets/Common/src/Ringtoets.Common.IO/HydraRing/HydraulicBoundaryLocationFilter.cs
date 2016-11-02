// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Linq;
using Ringtoets.Common.IO.Properties;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.Common.IO.HydraRing
{
    /// <summary>
    /// This class allows for filtering out <see cref="HydraulicBoundaryLocation"/> based
    /// on their name.
    /// </summary>
    public class HydraulicBoundaryLocationFilter
    {
        private readonly List<long> locationsToFilterOut;

        public HydraulicBoundaryLocationFilter()
        {
            string[] idsAsText = Resources.HydraulicBoundaryLocationsFilterList.Split(new[]
            {
                Environment.NewLine,
                "\n"
            }, StringSplitOptions.RemoveEmptyEntries);
            var filterList = new List<long>(idsAsText.Skip(1).Select(long.Parse)); // Skip the header, parse the remainder
            filterList.Sort();

            locationsToFilterOut = filterList;
        }

        /// <summary>
        /// Indicates if the <see cref="HydraulicBoundaryLocation"/> with <see cref="HydraulicBoundaryLocation.Name"/>
        /// should be imported.
        /// </summary>
        /// <param name="locationId">The name of the location.</param>
        /// <returns><c>True</c> if the location should be imported, <c>false</c> otherwise.</returns>
        public bool ShouldInclude(long locationId)
        {
            int matchingIndex = locationsToFilterOut.BinarySearch(locationId);
            return matchingIndex < 0;
        }
    }
}