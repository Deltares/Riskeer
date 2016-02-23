namespace Ringtoets.HydraRing.IO.HydraulicBoundaryDatabase
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