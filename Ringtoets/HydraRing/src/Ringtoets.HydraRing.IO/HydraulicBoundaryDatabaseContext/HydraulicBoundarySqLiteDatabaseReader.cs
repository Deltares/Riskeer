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

namespace Ringtoets.HydraRing.IO.HydraulicBoundaryDatabaseContext
{
    /// <summary>
    /// This class reads a SqLite database file and constructs <see cref="HydraulicBoundaryLocation"/> 
    /// instances from this database.
    /// </summary>
    public class HydraulicBoundarySqLiteDatabaseReader : SqLiteDatabaseReaderBase
    {
        private IDataReader sqliteDataReader;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundarySqLiteDatabaseReader"/>, 
        /// which will use the <paramref name="databaseFilePath"/> as its source.
        /// </summary>
        /// <param name="databaseFilePath">The path of the database file to open.</param>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>The <paramref name="databaseFilePath"/> contains invalid characters.</item>
        /// <item>No file could be found at <paramref name="databaseFilePath"/>.</item>
        /// </list>
        /// </exception>
        public HydraulicBoundarySqLiteDatabaseReader(string databaseFilePath) : base(databaseFilePath) {}

        /// <summary>
        /// Gets a value indicating whether or not more hydraulic boundary locations can
        /// be read using the <see cref="HydraulicBoundarySqLiteDatabaseReader"/>.
        /// </summary>
        public bool HasNext { get; private set; }

        /// <summary>
        /// Reads the next location from the database.
        /// </summary>
        /// <returns>New instance of <see cref="HydraulicBoundaryLocation"/>, based on the 
        /// data read from the database or <c>null</c> if no data is available.</returns>
        /// <exception cref="LineParseException">Thrown when the database returned incorrect 
        /// values for required properties.</exception>
        public void PrepareReadLocation()
        {
            CloseDataReader();
            HasNext = false;

            string locationsQuery = HydraulicBoundaryDatabaseQueryBuilder.GetRelevantLocationsQuery();
            sqliteDataReader = CreateDataReader(locationsQuery);
            MoveNext();
        }

        public HrdLocation ReadLocation()
        {
            if (!HasNext)
            {
                return null;
            }

            try
            {
                return ReadHrdLocation();
            }
            catch (InvalidCastException e)
            {
                var message = new FileReaderErrorMessageBuilder(Path).
                    Build(Resources.HydraulicBoundaryDatabaseReader_Critical_Unexpected_value_on_column);
                throw new LineParseException(message, e);
            }
        }

        /// <summary>
        /// Gets the database version from the metadata table.
        /// </summary>
        /// <returns>The version found in the database, or <see cref="string.Empty"/> if the version
        /// cannot be found.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when a query could not be executed on the database schema.</exception>
        public string GetVersion()
        {
            string versionQuery = HydraulicBoundaryDatabaseQueryBuilder.GetVersionQuery();
            try
            {
                using (IDataReader dataReader = CreateDataReader(versionQuery, null))
                {
                    return !dataReader.Read() ? string.Empty : Convert.ToString(dataReader[GeneralTableDefinitions.GeneratedVersion]);
                }
            }
            catch (SQLiteException exception)
            {
                var message = new FileReaderErrorMessageBuilder(Path).Build(Resources.Error_HydraulicBoundaryLocation_read_from_database);
                throw new CriticalFileReadException(message, exception);
            }
        }

        /// <summary>
        /// Gets the track id from the metadata table.
        /// </summary>
        /// <returns>The track id found in the database, or 0 if the track id
        /// cannot be found.</returns>
        /// <exception cref="InvalidCastException">Thrown when the database returned incorrect 
        /// values for required properties.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when a query could not be executed on the database schema.</exception>
        public long GetTrackId()
        {
            string trackQuery = HydraulicBoundaryDatabaseQueryBuilder.GetTrackIdQuery();
            var sqliteParameter = new SQLiteParameter
            {
                DbType = DbType.String
            };
            try
            {
                using (IDataReader dataReader = CreateDataReader(trackQuery, sqliteParameter))
                {
                    return !dataReader.Read() ? 0 : Convert.ToInt64(dataReader[GeneralTableDefinitions.TrackId]);
                }
            }
            catch (InvalidCastException exception)
            {
                var message = new FileReaderErrorMessageBuilder(Path).
                    Build(Resources.HydraulicBoundaryDatabaseReader_Critical_Unexpected_value_on_column);
                throw new LineParseException(message, exception);
            }
            catch (SQLiteException exception)
            {
                var message = new FileReaderErrorMessageBuilder(Path).Build(Resources.Error_HydraulicBoundaryLocation_read_from_database);
                throw new CriticalFileReadException(message, exception);
            }
        }

        /// <summary>
        /// Gets the amount of locations that can be read from the database.
        /// </summary>
        /// <returns>The amount of locations that can be read.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when a query could not be executed on the database schema.</exception>
        public int GetLocationCount()
        {
            string locationCountQuery = HydraulicBoundaryDatabaseQueryBuilder.GetRelevantLocationsCountQuery();
            var sqliteParameter = new SQLiteParameter
            {
                DbType = DbType.String
            };

            try
            {
                using (IDataReader dataReader = CreateDataReader(locationCountQuery, sqliteParameter))
                {
                    return !dataReader.Read() ? 0 : Convert.ToInt32(dataReader[HrdLocationsTableDefinitions.Count]);
                }
            }
            catch (InvalidCastException)
            {
                return 0;
            }
            catch (SQLiteException exception)
            {
                var message = new FileReaderErrorMessageBuilder(Path).Build(Resources.Error_HydraulicBoundaryLocation_read_from_database);
                throw new CriticalFileReadException(message, exception);
            }
        }

        public override void Dispose()
        {
            CloseDataReader();
            base.Dispose();
        }

        /// <summary>
        /// Moves the reader to the next record in the database.
        /// </summary>
        private void MoveNext()
        {
            HasNext = MoveNext(sqliteDataReader);
        }

        /// <summary>
        /// Reads a value at column <paramref name="columnName"/> from the database.
        /// </summary>
        /// <typeparam name="T">The expected type of value in the column with name <paramref name="columnName"/>.</typeparam>
        /// <param name="columnName">The name of the column to read from.</param>
        /// <returns>The read value from the column with name <paramref name="columnName"/>.</returns>
        /// <exception cref="InvalidCastException">Thrown when the value in the column was not of type 
        /// <typeparamref name="T"/>.</exception>
        private T Read<T>(string columnName)
        {
            return (T) sqliteDataReader[columnName];
        }

        /// <summary>
        /// Reads the current row into a new instance of <see cref="HrdLocation"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="HrdLocation"/>, based upon the current row.</returns>
        /// <exception cref="InvalidCastException">Thrown when the database returned incorrect values for 
        /// required properties.</exception>
        private HrdLocation ReadHrdLocation()
        {
            try
            {
                var id = Read<long>(HrdLocationsTableDefinitions.HrdLocationId);
                var name = Read<string>(HrdLocationsTableDefinitions.Name);
                var x = Read<double>(HrdLocationsTableDefinitions.XCoordinate);
                var y = Read<double>(HrdLocationsTableDefinitions.YCoordinate);
                MoveNext();
                return new HrdLocation(id, name, x, y);
            }
            catch (InvalidCastException)
            {
                MoveNext();
                throw;
            }
        }

        private void CloseDataReader()
        {
            if (sqliteDataReader != null)
            {
                sqliteDataReader.Dispose();
            }
        }
    }
}