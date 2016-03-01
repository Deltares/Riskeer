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

namespace Ringtoets.HydraRing.IO.HydraulicLocationConfigurationDatabaseContext
{
    /// <summary>
    /// Defines queries to execute on a hydraulic location configuration database.
    /// </summary>
    public static class HydraulicLocationConfigurationDatabaseQueryBuilder
    {
        /// <summary>
        /// Returns the query to get the LocationId from the database.
        /// </summary>
        /// <returns>The query to get the locationId from the database.</returns>
        public static string GetLocationIdQuery()
        {
            var countQuery = String.Format("Select COUNT({0}) FROM {1} WHERE {2} = @{2} AND {3} = @{3}",
                                           LocationsTableDefinitions.LocationId,
                                           LocationsTableDefinitions.TableName,
                                           LocationsTableDefinitions.RegionId,
                                           LocationsTableDefinitions.HrdLocationId);

            return String.Format("SELECT {0}, ({1}) as {2} FROM {3} WHERE {4} = @{4} AND {5} = @{5};",
                                 LocationsTableDefinitions.LocationId,
                                 countQuery,
                                 LocationsTableDefinitions.Count,
                                 LocationsTableDefinitions.TableName,
                                 LocationsTableDefinitions.RegionId,
                                 LocationsTableDefinitions.HrdLocationId
                );
        }
    }
}