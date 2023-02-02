// Copyright (C) Stichting Deltares 2022. All rights reserved.
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

namespace Riskeer.HydraRing.IO.HydraulicBoundaryDatabase
{
    /// <summary>
    /// Defines queries to execute on a hydraulic location configuration database.
    /// </summary>
    public static class HlcdQueryBuilder
    {
        /// <summary>
        /// Returns the query to get HRD file names from a hydraulic location configuration database.
        /// </summary>
        /// <param name="trackName">The name of the track to get the corresponding HRD file names for.</param>
        /// <returns>The query to get the HRD file names from a hydraulic location configuration database.</returns>
        public static string GetHrdFileNamesQuery(string trackName)
        {
            return $"SELECT {HlcdDefinitions.HrdFileNameColumn} " +
                   $"FROM {HlcdDefinitions.TracksTable} " +
                   $"WHERE ({HlcdDefinitions.TrackNameColumn} = '{trackName}'" +
                   $"OR {HlcdDefinitions.TrackNameColumn} LIKE '% {trackName} %'" +
                   $"OR {HlcdDefinitions.TrackNameColumn} LIKE '{trackName} %'" +
                   $"OR {HlcdDefinitions.TrackNameColumn} LIKE '% {trackName}');";
        }
    }
}