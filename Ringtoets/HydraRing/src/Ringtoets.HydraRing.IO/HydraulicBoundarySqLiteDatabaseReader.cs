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
using System.Data;
using System.Data.SQLite;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Core.Common.Utils.Builders;
using Ringtoets.HydraRing.Data;
using Ringtoets.HydraRing.IO.Properties;

namespace Ringtoets.HydraRing.IO
{
    /// <summary>
    /// This class reads a SqLite database file and constructs <see cref="HydraulicBoundaryLocation"/> instances from this database.
    /// </summary>
    public class HydraulicBoundarySqLiteDatabaseReader : SqLiteDatabaseReaderBase, IRowBasedDatabaseReader
    {
        private SQLiteDataReader dataReader;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundarySqLiteDatabaseReader"/>, which will use the <paramref name="databaseFilePath"/>
        /// as its source.
        /// </summary>
        /// <param name="databaseFilePath">The path of the database file to open.</param>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>The <paramref name="databaseFilePath"/> contains invalid characters.</item>
        /// <item>No file could be found at <paramref name="databaseFilePath"/>.</item>
        /// <item>Preparing the queries to read from the database failed.</item>
        /// </list>
        /// </exception>
        public HydraulicBoundarySqLiteDatabaseReader(string databaseFilePath)
            : base(databaseFilePath)
        {
            InitializeReader();
        }

        /// <summary>
        /// Gets the total number of locations that can be read from the database.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Gets the version from the database.
        /// </summary>
        public string Version { get; private set; }

        /// <summary>
        /// Gets the value <c>true</c> if locations can be read using the <see cref="HydraulicBoundarySqLiteDatabaseReader"/>.
        /// <c>false</c> otherwise.
        /// </summary>
        public bool HasNext { get; private set; }

        /// <summary>
        /// Reads the next location from the database.
        /// </summary>
        /// <returns>New instance of <see cref="HydraulicBoundaryLocation"/>, based on the data read from the database or <c>null</c> if no data is available.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when the database returned incorrect values for required properties.</exception>
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
                var message = new FileReaderErrorMessageBuilder(Path).Build(Resources.HydraulicBoundaryDatabaseReader_Critical_Unexpected_value_on_column);
                throw new CriticalFileReadException(message, e);
            }
        }

        /// <summary>
        /// Disposes the reader.
        /// </summary>
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

        /// <summary>
        /// Reads a value at column <paramref name="columnName"/> from the database.
        /// </summary>
        /// <typeparam name="T">The expected type of value in the column with name <paramref name="columnName"/>.</typeparam>
        /// <param name="columnName">The name of the column to read from.</param>
        /// <returns>The read value from the column with name <paramref name="columnName"/>.</returns>
        /// <exception cref="InvalidCastException">Thrown when the value in the column was not of type <typeparamref name="T"/>.</exception>
        public T Read<T>(string columnName)
        {
            return (T) dataReader[columnName];
        }

        /// <summary>
        /// Reads the value in the column with name <paramref name="columnName"/> from the currently pointed row.
        /// </summary>
        /// <typeparam name="T">The type of object to read.</typeparam>
        /// <param name="columnName">The name of the column to read from.</param>
        /// <returns>The value in the column, or <c>null</c> if the value was <see cref="DBNull.Value"/>.</returns>
        /// <exception cref="InvalidCastException">Thrown when the value in the column could not be casted to type <typeparamref name="T"/>.</exception>
        public T? ReadOrNull<T>(string columnName) where T : struct
        {
            var valueObject = dataReader[columnName];
            if (valueObject.Equals(DBNull.Value))
            {
                return null;
            }
            return (T) valueObject;
        }

        /// <summary>
        /// Reads the current row into a new instance of <see cref="HydraulicBoundaryLocation"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="HydraulicBoundaryLocation"/>, based upon the current row.</returns>
        /// <exception cref="InvalidCastException">Thrown when the database returned incorrect values for required properties.</exception>
        private HydraulicBoundaryLocation ReadHydraulicBoundaryLocation()
        {
            try
            {
                var id = Read<long>(HydraulicBoundaryDatabaseColumns.LocationId);
                var name = Read<string>(HydraulicBoundaryDatabaseColumns.LocationName);
                var x = Read<double>(HydraulicBoundaryDatabaseColumns.LocationX);
                var y = Read<double>(HydraulicBoundaryDatabaseColumns.LocationY);
                var designWaterLevel = "";
                MoveNext();
                return new HydraulicBoundaryLocation(id, name, x, y, designWaterLevel);
            }
            catch (InvalidCastException exception)
            {
                MoveNext();
                throw;
            }
        }

        /// <summary>
        /// Prepares a new data reader with queries for obtaining the locations and updates the reader
        /// so that it points to the first row of the result set.
        /// </summary>
        private void InitializeReader()
        {
            PrepareReader();
            MoveNext();
        }

        /// <summary>
        /// Prepares the queries required for obtaining locations from the database.
        /// </summary>
        private void PrepareReader()
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

        /// <summary>
        /// Creates a new data reader to use in this class.
        /// </summary>
        /// <exception cref="CriticalFileReadException">Thrown when
        /// <list type="bullet">
        ///     <item>Amount of locations in database could not be read.</item>
        ///     <item>A query could not be executed on the database schema.</item>
        /// </list>
        /// </exception>
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

        /// <summary>
        /// Gets the database version from the metadata table.
        /// </summary>
        private void GetVersion()
        {
            if (dataReader.Read())
            {
                Version = Read<string>(HydraulicBoundaryDatabaseColumns.Version);
            }
            dataReader.NextResult();
        }

        /// <summary>
        /// Gets the amount of locations that can be read from the database.
        /// </summary>
        private void GetCount()
        {
            dataReader.Read();
            Count = (int) Read<long>(HydraulicBoundaryDatabaseColumns.LocationCount);
            dataReader.NextResult();
        }
    }
}