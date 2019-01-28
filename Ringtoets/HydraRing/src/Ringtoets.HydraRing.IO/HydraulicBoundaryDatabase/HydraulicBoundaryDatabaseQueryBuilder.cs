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

namespace Riskeer.HydraRing.IO.HydraulicBoundaryDatabase
{
    /// <summary>
    /// Defines queries to execute on a hydraulic boundary database.
    /// </summary>
    public static class HydraulicBoundaryDatabaseQueryBuilder
    {
        /// <summary>
        /// Returns the query to get the version from the database.
        /// </summary>
        /// <returns>The query to get the version from the database.</returns>
        public static string GetVersionQuery()
        {
            return $"SELECT ({GeneralTableDefinitions.RegionName} || " +
                   $"{GeneralTableDefinitions.CreationDate} || " +
                   $"{GeneralTableDefinitions.TrackId}) as {GeneralTableDefinitions.GeneratedVersion} " +
                   $"FROM {GeneralTableDefinitions.TableName} LIMIT 0,1;";
        }

        /// <summary>
        /// Returns the query to get the track id from the database.
        /// </summary>
        /// <returns>The query to get the track id from the database.</returns>
        public static string GetTrackIdQuery()
        {
            return $"SELECT {GeneralTableDefinitions.TrackId} FROM {GeneralTableDefinitions.TableName} LIMIT 0,1;";
        }

        /// <summary>
        /// Returns the query to get all relevant locations from the database.
        /// </summary>
        /// <returns>The query to get all relevant locations from the database.</returns>
        public static string GetRelevantLocationsQuery()
        {
            return $"SELECT {HrdLocationsTableDefinitions.HrdLocationId}, {HrdLocationsTableDefinitions.Name}, " +
                   $"{HrdLocationsTableDefinitions.XCoordinate}, {HrdLocationsTableDefinitions.YCoordinate} " +
                   $"FROM {HrdLocationsTableDefinitions.TableName} " +
                   $"WHERE {HrdLocationsTableDefinitions.LocationTypeId} > 1;"; // Value > 1 makes it relevant
        }
    }
}