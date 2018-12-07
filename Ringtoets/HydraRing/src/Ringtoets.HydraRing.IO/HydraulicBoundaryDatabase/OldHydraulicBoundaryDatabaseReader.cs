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

using System;
using System.Data;
using System.Data.SQLite;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Core.Common.Util.Builders;
using Ringtoets.HydraRing.IO.Properties;

namespace Ringtoets.HydraRing.IO.HydraulicBoundaryDatabase
{
    /// <summary>
    /// This class reads a SqLite database file and constructs <see cref="ReadHydraulicBoundaryLocation"/>
    /// instances from this database.
    /// </summary>
    public class OldHydraulicBoundaryDatabaseReader : SqLiteDatabaseReaderBase
    {
        private IDataReader sqliteDataReader;

        /// <summary>
        /// Creates a new instance of <see cref="OldHydraulicBoundaryDatabaseReader"/>,
        /// which will use the <paramref name="databaseFilePath"/> as its source.
        /// </summary>
        /// <param name="databaseFilePath">The path of the database file to open.</param>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>The <paramref name="databaseFilePath"/> contains invalid characters.</item>
        /// <item>No file could be found at <paramref name="databaseFilePath"/>.</item>
        /// </list>
        /// </exception>
        public OldHydraulicBoundaryDatabaseReader(string databaseFilePath) : base(databaseFilePath) {}

        /// <summary>
        /// Gets a value indicating whether or not more hydraulic boundary locations can
        /// be read using the <see cref="HydraulicBoundaryDatabaseReader"/>.
        /// </summary>
        public bool HasNext { get; private set; }

        public void PrepareReadLocation()
        {
            CloseDataReader();
            HasNext = false;

            sqliteDataReader = CreateDataReader(HydraulicBoundaryDatabaseQueryBuilder.GetRelevantLocationsQuery());
            MoveNext();
        }

        /// <summary>
        /// Reads the next location from the database.
        /// </summary>
        /// <returns>A new instance of <see cref="ReadHydraulicBoundaryLocation"/> based on the data read from
        /// the database or <c>null</c> if no data is available.</returns>
        /// <exception cref="LineParseException">Thrown when the database contains incorrect values for required properties.</exception>
        public ReadHydraulicBoundaryLocation ReadLocation()
        {
            if (!HasNext)
            {
                return null;
            }

            try
            {
                var id = sqliteDataReader.Read<long>(HrdLocationsTableDefinitions.HrdLocationId);
                var name = sqliteDataReader.Read<string>(HrdLocationsTableDefinitions.Name);
                var x = sqliteDataReader.Read<double>(HrdLocationsTableDefinitions.XCoordinate);
                var y = sqliteDataReader.Read<double>(HrdLocationsTableDefinitions.YCoordinate);

                return new ReadHydraulicBoundaryLocation(id, name, x, y);
            }
            catch (ConversionException e)
            {
                string message = new FileReaderErrorMessageBuilder(Path).Build(Resources.HydraulicBoundaryDatabaseReader_Critical_Unexpected_value_on_column);
                throw new LineParseException(message, e);
            }
            finally
            {
                MoveNext();
            }
        }

        /// <summary>
        /// Gets the version of the hydraulic boundary database.
        /// </summary>
        /// <returns>The version found in the database, or <see cref="string.Empty"/> if the version cannot be found.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when a query could not be executed on the database schema.</exception>
        public string GetVersion()
        {
            try
            {
                using (IDataReader dataReader = CreateDataReader(HydraulicBoundaryDatabaseQueryBuilder.GetVersionQuery(), null))
                {
                    return !dataReader.Read() ? string.Empty : Convert.ToString(dataReader[GeneralTableDefinitions.GeneratedVersion]);
                }
            }
            catch (SQLiteException exception)
            {
                string message = new FileReaderErrorMessageBuilder(Path).Build(Resources.Error_HydraulicBoundaryLocation_read_from_database);
                throw new CriticalFileReadException(message, exception);
            }
        }

        /// <summary>
        /// Gets the track id from the hydraulic boundary database.
        /// </summary>
        /// <returns>The track id found in the database, or 0 if the track id cannot be found.</returns>
        /// <exception cref="LineParseException">Thrown when the database contains incorrect values for required properties.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when a query could not be executed on the database schema.</exception>
        public long GetTrackId()
        {
            try
            {
                using (IDataReader dataReader = CreateDataReader(HydraulicBoundaryDatabaseQueryBuilder.GetTrackIdQuery(),
                                                                 new SQLiteParameter
                                                                 {
                                                                     DbType = DbType.String
                                                                 }))
                {
                    return !dataReader.Read() ? 0 : Convert.ToInt64(dataReader[GeneralTableDefinitions.TrackId]);
                }
            }
            catch (InvalidCastException exception)
            {
                string message = new FileReaderErrorMessageBuilder(Path).Build(Resources.HydraulicBoundaryDatabaseReader_Critical_Unexpected_value_on_column);
                throw new LineParseException(message, exception);
            }
            catch (SQLiteException exception)
            {
                string message = new FileReaderErrorMessageBuilder(Path).Build(Resources.Error_HydraulicBoundaryLocation_read_from_database);
                throw new CriticalFileReadException(message, exception);
            }
        }

        protected override void Dispose(bool disposing)
        {
            CloseDataReader();
            base.Dispose(disposing);
        }

        /// <summary>
        /// Moves the reader to the next record in the database.
        /// </summary>
        private void MoveNext()
        {
            HasNext = MoveNext(sqliteDataReader);
        }

        private void CloseDataReader()
        {
            sqliteDataReader?.Dispose();
        }
    }
}