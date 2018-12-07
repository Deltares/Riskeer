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
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Core.Common.Util.Builders;
using Ringtoets.HydraRing.IO.Properties;

namespace Ringtoets.HydraRing.IO.HydraulicBoundaryDatabase
{
    /// <summary>
    /// This class reads a SqLite database file and constructs a <see cref="ReadHydraulicBoundaryDatabase"/>
    /// instance from this database.
    /// </summary>
    public class HydraulicBoundaryDatabaseReader : SqLiteDatabaseReaderBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryDatabaseReader"/>,
        /// which will use the <paramref name="databaseFilePath"/> as its source.
        /// </summary>
        /// <param name="databaseFilePath">The path of the database file to open.</param>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>The <paramref name="databaseFilePath"/> contains invalid characters.</item>
        /// <item>No file could be found at <paramref name="databaseFilePath"/>.</item>
        /// </list>
        /// </exception>
        public HydraulicBoundaryDatabaseReader(string databaseFilePath) : base(databaseFilePath) {}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="LineParseException">Thrown when the database contains incorrect values for required properties.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when a query could not be executed on the database schema.</exception>
        public ReadHydraulicBoundaryDatabase Read()
        {
            return new ReadHydraulicBoundaryDatabase(ReadTrackId(), ReadVersion(), ReadLocations().ToArray());
        }

        /// <summary>
        /// Reads the track id from the hydraulic boundary database.
        /// </summary>
        /// <returns>The track id found in the database.</returns>
        /// <exception cref="LineParseException">Thrown when the database contains incorrect values for required properties.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when a query could not be executed on the database schema
        /// or the track id cannot be found.</exception>
        private long ReadTrackId()
        {
            try
            {
                using (IDataReader dataReader = CreateDataReader(HydraulicBoundaryDatabaseQueryBuilder.GetTrackIdQuery(),
                                                                 new SQLiteParameter
                                                                 {
                                                                     DbType = DbType.String
                                                                 }))
                {
                    if (dataReader.Read())
                    {
                        return Convert.ToInt64(dataReader[GeneralTableDefinitions.TrackId]);
                    }

                    throw new CriticalFileReadException(new FileReaderErrorMessageBuilder(Path)
                                                            .Build(Resources.HydraulicBoundaryDatabaseReader_Critical_Unexpected_value_on_column));
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

        /// <summary>
        /// Gets the version of the hydraulic boundary database.
        /// </summary>
        /// <returns>The version found in the database, or <see cref="string.Empty"/> if the version cannot be found.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when a query could not be executed on the database schema.</exception>
        private string ReadVersion()
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
        /// 
        /// </summary>
        /// <exception cref="LineParseException">Thrown when the database contains incorrect values for required properties.</exception>
        private IEnumerable<ReadHydraulicBoundaryLocation> ReadLocations()
        {
            using (IDataReader reader = CreateDataReader(HydraulicBoundaryDatabaseQueryBuilder.GetRelevantLocationsQuery()))
            {
                while (MoveNext(reader))
                {
                    yield return ReadLocation(reader);
                }
            }
        }

        /// <summary>
        /// Reads the next location from the database.
        /// </summary>
        /// <returns>A new instance of <see cref="ReadHydraulicBoundaryLocation"/> based on the data read from
        /// the database or <c>null</c> if no data is available.</returns>
        /// <exception cref="LineParseException">Thrown when the database contains incorrect values for required properties.</exception>
        private ReadHydraulicBoundaryLocation ReadLocation(IDataReader reader)
        {
            try
            {
                var id = reader.Read<long>(HrdLocationsTableDefinitions.HrdLocationId);
                var name = reader.Read<string>(HrdLocationsTableDefinitions.Name);
                var x = reader.Read<double>(HrdLocationsTableDefinitions.XCoordinate);
                var y = reader.Read<double>(HrdLocationsTableDefinitions.YCoordinate);

                return new ReadHydraulicBoundaryLocation(id, name, x, y);
            }
            catch (ConversionException e)
            {
                string message = new FileReaderErrorMessageBuilder(Path).Build(Resources.HydraulicBoundaryDatabaseReader_Critical_Unexpected_value_on_column);
                throw new LineParseException(message, e);
            }
        }
    }
}