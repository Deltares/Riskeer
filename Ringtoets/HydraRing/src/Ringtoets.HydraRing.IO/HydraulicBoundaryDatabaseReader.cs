using System;
using System.Data;
using System.Data.SQLite;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Core.Common.Utils.Builders;
using Ringtoets.HydraRing.Data;
using Ringtoets.HydraRing.IO.Properties;

namespace Ringtoets.HydraRing.IO
{
    public class HydraulicBoundaryDatabaseReader : DatabaseReaderBase, IRowBasedDatabaseReader
    {
        private SQLiteDataReader dataReader;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryDatabaseReader"/> which will use the <paramref name="databaseFilePath"/>
        /// as its source.
        /// </summary>
        /// <param name="databaseFilePath">The path of the database file to open.</param>
        /// <exception cref="Core.Common.IO.Exceptions.CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>The <paramref name="databaseFilePath"/> contains invalid characters.</item>
        /// <item>No file could be found at <paramref name="databaseFilePath"/>.</item>
        /// <item>Preparing the queries to read from the database failed.</item>
        /// </list>
        /// </exception>
        public HydraulicBoundaryDatabaseReader(string databaseFilePath)
            : base(databaseFilePath)
        {
            InitializeReader();
        }

        /// <summary>
        /// Gets the total number of profiles that can be read from the database.
        /// </summary>
        public int Count { get; private set; }


        /// <summary>
        /// Gets the version from the database.
        /// </summary>
        public string Version { get; private set; }

        /// <summary>
        /// Gets the value <c>true</c> if profiles can be read using the <see cref="HydraulicBoundaryDatabaseReader"/>.
        /// <c>false</c> otherwise.
        /// </summary>
        public bool HasNext { get; private set; }

        public HydraulicBoundaryLocation ReadLocation()
        {
            if (!HasNext)
            {
                return null;
            }

            try
            {
                return ReadHydraulicBoundaryLocation();
            }
            catch (InvalidCastException e)
            {
                var message = new FileReaderErrorMessageBuilder(Path).Build("Kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.");
                throw new CriticalFileReadException(message, e);
            }
        }

        public override void Dispose()
        {
            if (dataReader != null)
            {
                dataReader.Dispose();
            }
            base.Dispose();
        }

        /// <summary>
        /// Moves the reader to the next record in the database.
        /// </summary>
        public void MoveNext()
        {
            HasNext = dataReader.Read() || (dataReader.NextResult() && dataReader.Read());
        }

        public T Read<T>(string columnName)
        {
            return (T) dataReader[columnName];
        }

        public T? ReadOrNull<T>(string columnName) where T : struct
        {
            var valueObject = dataReader[columnName];
            if (valueObject.Equals(DBNull.Value))
            {
                return null;
            }
            return (T) valueObject;
        }

        private HydraulicBoundaryLocation ReadHydraulicBoundaryLocation()
        {
            try
            {
                var id = Read<long>(HydraulicBoundaryDatabaseColumns.LocationId);
                var name = Read<string>(HydraulicBoundaryDatabaseColumns.LocationName);
                var x = Read<double>(HydraulicBoundaryDatabaseColumns.LocationX);
                var y = Read<double>(HydraulicBoundaryDatabaseColumns.LocationY);
                MoveNext();
                return new HydraulicBoundaryLocation(id, name, x, y);
            }
            catch (InvalidCastException)
            {
                MoveNext();
                throw;
            }
        }

        /// <summary>
        /// </summary>
        private void InitializeReader()
        {
            ReadLocations();
            MoveNext();
        }

        /// <summary>
        /// </summary>
        private void ReadLocations()
        {
            var versionQuery = string.Format("SELECT (NameRegion || CreationDate) as {0} FROM General LIMIT 0,1;", HydraulicBoundaryDatabaseColumns.Version);
            var countQuery = string.Format("SELECT count(*) as {0} FROM HRDLocations WHERE LocationTypeId > 1 ;", HydraulicBoundaryDatabaseColumns.LocationCount);

            var locationsQuery = string.Format(
                "SELECT HRDLocationId as {0}, Name as {1}, XCoordinate as {2}, YCoordinate as {3} FROM HRDLocations WHERE LocationTypeId > 1;",
                HydraulicBoundaryDatabaseColumns.LocationId,
                HydraulicBoundaryDatabaseColumns.LocationName,
                HydraulicBoundaryDatabaseColumns.LocationX,
                HydraulicBoundaryDatabaseColumns.LocationY);

            CreateDataReader(string.Join(" ", versionQuery, countQuery, locationsQuery), new SQLiteParameter
            {
                DbType = DbType.String
            });
        }

        private void CreateDataReader(string queryString, params SQLiteParameter[] parameters)
        {
            using (var query = new SQLiteCommand(Connection)
            {
                CommandText = queryString
            })
            {
                query.Parameters.AddRange(parameters);

                try
                {
                    dataReader = query.ExecuteReader();
                    GetVersion();
                    GetCount();
                }
                catch (SQLiteException exception)
                {
                    Dispose();
                    var message = new FileReaderErrorMessageBuilder(Path).Build(Resources.Error_HydraulicBoundaryLocation_read_from_database);
                    throw new CriticalFileReadException(message, exception);
                }
            }
        }

        private void GetVersion()
        {
            if (dataReader.Read())
            {
                Version = Read<string>(HydraulicBoundaryDatabaseColumns.Version);
            }
            dataReader.NextResult();
        }

        private void GetCount()
        {
            dataReader.Read();
            Count = (int) Read<long>(HydraulicBoundaryDatabaseColumns.LocationCount);
            dataReader.NextResult();
        }
    }
}