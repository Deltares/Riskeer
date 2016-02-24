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

namespace Ringtoets.HydraRing.IO.HydraulicBoundaryDatabaseContext
{
    public static class HydraulicBoundaryDatabaseQueryBuilder
    {
        public static string GetVersionQuery()
        {
            return string.Format(
                "SELECT ({0} || {1}) as {2} FROM {3} LIMIT 0,1;",
                GeneralEntity.NameRegion,
                GeneralEntity.CreationDate,
                HydraulicBoundaryDatabaseColumns.Version,
                GeneralEntity.TableName
                );
        }

        public static string GetLocationsCountQuery()
        {
            return string.Format(
                "SELECT count({0}) as {1} FROM {2} WHERE {3} > 1 ;",
                HrdLocationsEntity.HrdLocationId,
                HydraulicBoundaryDatabaseColumns.LocationCount,
                HrdLocationsEntity.TableName,
                HrdLocationsEntity.LocationTypeId
                );
        }

        public static string GetLocationsQuery()
        {
            return string.Format(
                "SELECT {0} as {1}, " +
                "{2} as {3}, " +
                "{4} as {5}, " +
                "{6} as {7} FROM " +
                "{8} WHERE {9} > 1;",
                HrdLocationsEntity.HrdLocationId, HydraulicBoundaryDatabaseColumns.LocationId,
                HrdLocationsEntity.Name, HydraulicBoundaryDatabaseColumns.LocationName,
                HrdLocationsEntity.XCoordinate, HydraulicBoundaryDatabaseColumns.LocationX,
                HrdLocationsEntity.YCoordinate, HydraulicBoundaryDatabaseColumns.LocationY,
                HrdLocationsEntity.TableName,
                HrdLocationsEntity.LocationTypeId);
        }
    }
}