﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Utils.Builders;
using log4net;
using Ringtoets.HydraRing.IO.Properties;

namespace Ringtoets.HydraRing.IO.HydraulicLocationConfigurationDatabaseContext
{
    /// <summary>
    /// Class for reading information from a hydraulic location configuration database (HLCD).
    /// </summary>
    public class HydraulicLocationConfigurationSqLiteDatabaseReader : SqLiteDatabaseReaderBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(HydraulicLocationConfigurationSqLiteDatabaseReader));

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicLocationConfigurationSqLiteDatabaseReader"/>, 
        /// which will use the <paramref name="databaseFilePath"/> as its source.
        /// </summary>
        /// <param name="databaseFilePath">The path of the database file to open.</param>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>The <paramref name="databaseFilePath"/> contains invalid characters.</item>
        /// <item>No file could be found at <paramref name="databaseFilePath"/>.</item>
        /// </list>
        /// </exception>
        public HydraulicLocationConfigurationSqLiteDatabaseReader(string databaseFilePath) : base(databaseFilePath) {}

        /// <summary>
        /// Gets the location ids from the database, based upon <paramref name="trackId"/>.
        /// </summary>
        /// <param name="trackId">The hydraulic boundary track id.</param>
        /// <returns>A dictionary with pairs of Hrd location id (key) and location id (value) as found in the database.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when the database query failed.</exception>
        /// <exception cref="LineParseException">Thrown when the database returned incorrect values for 
        /// required properties.</exception>
        public Dictionary<long, long> GetLocationIdsByTrackId(long trackId)
        {
            var trackParameter = new SQLiteParameter
            {
                DbType = DbType.String,
                ParameterName = LocationsTableDefinitions.TrackId,
                Value = trackId
            };

            try
            {
                return GetLocationIdsFromDatabase(trackParameter);
            }
            catch (SQLiteException exception)
            {
                string message = new FileReaderErrorMessageBuilder(Path).Build(Resources.HydraulicLocationConfigurationSqLiteDatabaseReader_Critical_Unexpected_Exception);
                throw new CriticalFileReadException(message, exception);
            }
            catch (InvalidCastException exception)
            {
                string message = new FileReaderErrorMessageBuilder(Path).Build(Resources.HydraulicBoundaryDatabaseReader_Critical_Unexpected_value_on_column);
                throw new LineParseException(message, exception);
            }
        }

        /// <summary>
        /// Gets whether the preprocessor can be used for the given <paramref name="trackId"/>.
        /// </summary>
        /// <param name="trackId">The hydraulic boundary track id.</param>
        /// <returns>The value found in the database; or <c>false</c> when the database
        /// is valid but outdated (no UsePreprocessor column present).</returns>
        /// <exception cref="CriticalFileReadException">Thrown when the database query failed or when no results could be found.</exception>
        /// <exception cref="LineParseException">Thrown when the database returned incorrect values for 
        /// required properties.</exception>
        public bool GetCanUsePreprocessorByTrackId(long trackId)
        {
            var trackParameter = new SQLiteParameter
            {
                DbType = DbType.String,
                ParameterName = TracksTableDefinitions.TrackId,
                Value = trackId
            };

            try
            {
                return GetCanUsePreprocessorFromDatabase(trackParameter);
            }
            catch (SQLiteException exception)
            {
                string message = new FileReaderErrorMessageBuilder(Path).Build(Resources.HydraulicLocationConfigurationSqLiteDatabaseReader_Critical_Unexpected_Exception);
                throw new CriticalFileReadException(message, exception);
            }
            catch (FormatException exception)
            {
                string message = new FileReaderErrorMessageBuilder(Path).Build(Resources.HydraulicBoundaryDatabaseReader_Critical_Unexpected_value_on_column);
                throw new LineParseException(message, exception);
            }
        }

        /// <summary>
        /// Gets the location ids from the database, based upon <paramref name="trackParameter"/>.
        /// </summary>
        /// <param name="trackParameter">A parameter containing the hydraulic boundary track id.</param>
        /// <returns>A dictionary with pairs of Hrd location id (key) and location id (value) as found in the database.</returns>
        /// <exception cref="SQLiteException">Thrown when the database query failed.</exception>
        /// <exception cref="InvalidCastException">Thrown when the database returned incorrect values for 
        /// required properties.</exception>
        private Dictionary<long, long> GetLocationIdsFromDatabase(SQLiteParameter trackParameter)
        {
            var dictionary = new Dictionary<long, long>();
            string query = HydraulicLocationConfigurationDatabaseQueryBuilder.GetLocationIdsByTrackIdQuery();
            using (IDataReader dataReader = CreateDataReader(query, trackParameter))
            {
                while (MoveNext(dataReader))
                {
                    long key = Convert.ToInt64(dataReader[LocationsTableDefinitions.HrdLocationId]);
                    long value = Convert.ToInt64(dataReader[LocationsTableDefinitions.LocationId]);

                    // Must be unique
                    if (dictionary.ContainsKey(key))
                    {
                        log.Warn(Resources.HydraulicLocationConfigurationSqLiteDatabaseReader_GetLocationIdFromDatabase_Ambiguous_Row_Found_Take_First);
                    }
                    else
                    {
                        dictionary.Add(key, value);
                    }
                }
            }
            return dictionary;
        }

        /// <summary>
        /// Gets whether the preprocessor can be used for the given <paramref name="trackParameter"/>.
        /// </summary>
        /// <param name="trackParameter">A parameter containing the hydraulic boundary track id.</param>
        /// <returns>The value found in the database; or <c>false</c> when the database
        /// is valid but outdated (no UsePreprocessor column present).</returns>
        /// <exception cref="SQLiteException">Thrown when the database query failed.</exception>
        /// <exception cref="FormatException">Thrown when the database returned incorrect values for 
        /// required properties.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when no results could be found.</exception>
        private bool GetCanUsePreprocessorFromDatabase(SQLiteParameter trackParameter)
        {
            string query = HydraulicLocationConfigurationDatabaseQueryBuilder.GetRegionByTrackIdQuery();
            using (IDataReader dataReader = CreateDataReader(query, trackParameter))
            {
                DataTable schemaTable = dataReader.GetSchemaTable();
                DataColumn columnName = schemaTable.Columns[schemaTable.Columns.IndexOf("ColumnName")];

                if (schemaTable.Rows.Cast<DataRow>().All(row => row[columnName].ToString() != RegionsTableDefinitions.UsePreprocessor))
                {
                    return false;
                }

                if (MoveNext(dataReader))
                {
                    return Convert.ToBoolean(dataReader[RegionsTableDefinitions.UsePreprocessor]);
                }

                string message = new FileReaderErrorMessageBuilder(Path).Build(Resources.HydraulicLocationConfigurationSqLiteDatabaseReader_Critical_Unexpected_Exception);
                throw new CriticalFileReadException(message);
            }
        }
    }
}