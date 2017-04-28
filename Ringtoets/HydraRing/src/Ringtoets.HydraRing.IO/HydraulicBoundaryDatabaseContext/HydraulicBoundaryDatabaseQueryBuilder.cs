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

namespace Ringtoets.HydraRing.IO.HydraulicBoundaryDatabaseContext
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
            return string.Format(
                "SELECT ({0} || {1} || {2}) as {3} FROM {4} LIMIT 0,1;",
                GeneralTableDefinitions.RegionName,
                GeneralTableDefinitions.CreationDate,
                GeneralTableDefinitions.TrackId,
                GeneralTableDefinitions.GeneratedVersion,
                GeneralTableDefinitions.TableName
            );
        }

        /// <summary>
        /// Returns the query to get the track id from the database.
        /// </summary>
        /// <returns>The query to get the track id from the database.</returns>
        public static string GetTrackIdQuery()
        {
            return string.Format(
                "SELECT {0} FROM {1} LIMIT 0,1;",
                GeneralTableDefinitions.TrackId,
                GeneralTableDefinitions.TableName
            );
        }

        /// <summary>
        /// Returns the query to get the amount of relevant locations from the database.
        /// </summary>
        /// <returns>The query to get the amount of relevant locations from the database.</returns>
        public static string GetRelevantLocationsCountQuery()
        {
            return string.Format(
                "SELECT count({0}) as {1} FROM {2} WHERE {3} > 1 ;",
                HrdLocationsTableDefinitions.HrdLocationId,
                HrdLocationsTableDefinitions.Count,
                HrdLocationsTableDefinitions.TableName,
                HrdLocationsTableDefinitions.LocationTypeId // Value > 1 makes it relevant
            );
        }

        /// <summary>
        /// Returns the query to get the all relevant locations from the database.
        /// </summary>
        /// <returns>The query to get the all relevant locations from the database.</returns>
        public static string GetRelevantLocationsQuery()
        {
            return string.Format(
                "SELECT {0}, {1}, {2}, {3} FROM {4} WHERE {5} > 1;",
                HrdLocationsTableDefinitions.HrdLocationId,
                HrdLocationsTableDefinitions.Name,
                HrdLocationsTableDefinitions.XCoordinate,
                HrdLocationsTableDefinitions.YCoordinate,
                HrdLocationsTableDefinitions.TableName,
                HrdLocationsTableDefinitions.LocationTypeId // Value > 1 makes it relevant
            );
        }
    }
}