// Copyright (C) Stichting Deltares 2022. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using log4net;
using Riskeer.HydraRing.IO.Properties;

namespace Riskeer.HydraRing.IO.HydraulicLocationConfigurationDatabase
{
    /// <summary>
    /// Class for reading information from a hydraulic location configuration database (HLCD).
    /// </summary>
    public class EnhancedHydraulicLocationConfigurationDatabaseReader : SqLiteDatabaseReaderBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EnhancedHydraulicLocationConfigurationDatabaseReader));

        /// <summary>
        /// Creates a new instance of <see cref="EnhancedHydraulicLocationConfigurationDatabaseReader"/>, 
        /// which will use the <paramref name="databaseFilePath"/> as its source.
        /// </summary>
        /// <param name="databaseFilePath">The path of the database file to open.</param>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>the <paramref name="databaseFilePath"/> contains invalid characters;</item>
        /// <item>no file could be found at <paramref name="databaseFilePath"/>.</item>
        /// </list>
        /// </exception>
        public EnhancedHydraulicLocationConfigurationDatabaseReader(string databaseFilePath) : base(databaseFilePath) {}

        /// <summary>
        /// Reads the hydraulic location configuration database.
        /// </summary>
        /// <returns>A read hydraulic location configuration database.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when the hydraulic location configuration database
        /// could not be read.</exception>
        /// <exception cref="LineParseException">Thrown when the database returned incorrect values for 
        /// required properties.</exception>
        public EnhancedReadHydraulicLocationConfigurationDatabase Read()
        {
            return new EnhancedReadHydraulicLocationConfigurationDatabase(GetLocations(),
                                                                          IsScenarioInformationTablePresent()
                                                                              ? GetConfigurationSettings()
                                                                              : null,
                                                                          GetUsePreprocessorClosureByTrackId(1));
        }

        /// <summary>
        /// Gets the locations from the database.
        /// </summary>
        /// <returns>A collection of <see cref="ReadHydraulicLocation"/> as found in the database.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when the database query failed.</exception>
        /// <exception cref="LineParseException">Thrown when the database returned incorrect values for 
        /// required properties.</exception>
        private IEnumerable<ReadHydraulicLocation> GetLocations()
        {
            try
            {
                return GetLocationsFromDatabase();
            }
            catch (SQLiteException exception)
            {
                string message = new FileReaderErrorMessageBuilder(Path).Build(Resources.HydraulicLocationConfigurationDatabaseReader_Critical_Unexpected_Exception);
                throw new CriticalFileReadException(message, exception);
            }
            catch (InvalidCastException exception)
            {
                string message = new FileReaderErrorMessageBuilder(Path).Build(Resources.HydraulicBoundaryDatabaseReader_Critical_Unexpected_value_on_column);
                throw new LineParseException(message, exception);
            }
        }

        /// <summary>
        /// Gets the locations from the database.
        /// </summary>
        /// <returns>A collection of <see cref="ReadHydraulicLocation"/> as found in the database.</returns>
        /// <exception cref="SQLiteException">Thrown when the database query failed.</exception>
        /// <exception cref="InvalidCastException">Thrown when the database returned incorrect values for 
        /// required properties.</exception>
        private IEnumerable<ReadHydraulicLocation> GetLocationsFromDatabase()
        {
            using (IDataReader dataReader = CreateDataReader(HydraulicLocationConfigurationDatabaseQueryBuilder.GetLocationsQuery()))
            {
                while (MoveNext(dataReader))
                {
                    yield return ReadHydraulicLocation(dataReader);
                }
            }
        }

        /// <summary>
        /// Reads a location from the database.
        /// </summary>
        /// <param name="reader">The <see cref="IDataReader"/> which is used to read the data.</param>
        /// <returns>The read <see cref="ReadHydraulicLocation"/>.</returns>
        /// <exception cref="LineParseException">Thrown when the database returned incorrect values for 
        /// required properties.</exception>
        private ReadHydraulicLocation ReadHydraulicLocation(IDataReader reader)
        {
            try
            {
                return new ReadHydraulicLocation(Convert.ToInt64(reader[LocationsTableDefinitions.LocationId]),
                                                 Convert.ToInt64(reader[LocationsTableDefinitions.HrdLocationId]),
                                                 Convert.ToInt64(reader[LocationsTableDefinitions.TrackId]));
            }
            catch (ConversionException e)
            {
                string message = new FileReaderErrorMessageBuilder(Path).Build(Resources.HydraulicBoundaryDatabaseReader_Critical_Unexpected_value_on_column);
                throw new LineParseException(message, e);
            }
        }

        /// <summary>
        /// Determines whether the table related to the scenario information is present in the database.
        /// </summary>
        /// <returns><c>true</c> if the table is present; <c>false</c> otherwise.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when the information could not be read from the database file.</exception>
        private bool IsScenarioInformationTablePresent()
        {
            try
            {
                using (IDataReader dataReader = CreateDataReader(HydraulicLocationConfigurationDatabaseQueryBuilder.GetIsScenarioInformationPresentQuery()))
                {
                    if (dataReader.Read())
                    {
                        return Convert.ToBoolean(dataReader[ScenarioInformationTableDefinitions.IsScenarioInformationPresent]);
                    }

                    string message = new FileReaderErrorMessageBuilder(Path).Build(Resources.HydraulicBoundaryDatabaseReader_Critical_Unexpected_value_on_column);
                    throw new CriticalFileReadException(message);
                }
            }
            catch (SQLiteException exception)
            {
                string message = new FileReaderErrorMessageBuilder(Path).Build(Resources.HydraulicLocationConfigurationDatabaseReader_Critical_Unexpected_Exception);
                throw new CriticalFileReadException(message, exception);
            }
        }

        /// <summary>
        /// Gets the hydraulic location configuration settings from the database.
        /// </summary>
        /// <returns>A collection of the read hydraulic configuration database settings.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when the database query failed.</exception>
        /// <exception cref="LineParseException">Thrown when the database returned incorrect values for 
        /// required properties.</exception>
        private IEnumerable<ReadHydraulicLocationConfigurationDatabaseSettings> GetConfigurationSettings()
        {
            try
            {
                return GetConfigurationSettingsFromDatabase();
            }
            catch (SQLiteException exception)
            {
                string message = new FileReaderErrorMessageBuilder(Path).Build(Resources.HydraulicLocationConfigurationDatabaseReader_Critical_Unexpected_Exception);
                throw new CriticalFileReadException(message, exception);
            }
        }

        /// <summary>
        /// Gets the hydraulic location configuration settings from the database.
        /// </summary>
        /// <returns>A collection of the read hydraulic configuration database settings.</returns>
        /// <exception cref="SQLiteException">Thrown when the database query failed.</exception>
        /// <exception cref="LineParseException">Thrown when the database returned incorrect values for 
        /// required properties.</exception>
        private IEnumerable<ReadHydraulicLocationConfigurationDatabaseSettings> GetConfigurationSettingsFromDatabase()
        {
            using (IDataReader dataReader = CreateDataReader(HydraulicLocationConfigurationDatabaseQueryBuilder.GetScenarioInformationQuery()))
            {
                while (MoveNext(dataReader))
                {
                    yield return ReadSettings(dataReader);
                }
            }
        }

        /// <summary>
        /// Reads a hydraulic location configuration settings instance from the database.
        /// </summary>
        /// <param name="reader">The <see cref="IDataReader"/> which is used to read the data.</param>
        /// <returns>The read <see cref="ReadHydraulicLocationConfigurationDatabaseSettings"/>.</returns>
        /// <exception cref="LineParseException">Thrown when the database returned incorrect values for 
        /// required properties.</exception>
        private ReadHydraulicLocationConfigurationDatabaseSettings ReadSettings(IDataReader reader)
        {
            try
            {
                return new ReadHydraulicLocationConfigurationDatabaseSettings(reader.Read<string>(ScenarioInformationTableDefinitions.ScenarioName),
                                                                              reader.Read<int>(ScenarioInformationTableDefinitions.Year),
                                                                              reader.Read<string>(ScenarioInformationTableDefinitions.Scope),
                                                                              reader.Read<string>(ScenarioInformationTableDefinitions.SeaLevel),
                                                                              reader.Read<string>(ScenarioInformationTableDefinitions.RiverDischarge),
                                                                              reader.Read<string>(ScenarioInformationTableDefinitions.LakeLevel),
                                                                              reader.Read<string>(ScenarioInformationTableDefinitions.WindDirection),
                                                                              reader.Read<string>(ScenarioInformationTableDefinitions.WindSpeed),
                                                                              reader.Read<string>(ScenarioInformationTableDefinitions.Comment));
            }
            catch (ConversionException e)
            {
                string message = new FileReaderErrorMessageBuilder(Path).Build(Resources.HydraulicBoundaryDatabaseReader_Critical_Unexpected_value_on_column);
                throw new LineParseException(message, e);
            }
        }

        /// <summary>
        /// Gets the preprocessor closure indicator from the database.
        /// </summary>
        /// <param name="trackId">The track id to get the preprocessor closure for.</param>
        /// <returns>The read indicator whether to use the preprocessor closure.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when the information could not be read
        /// from the database file.</exception>
        /// <exception cref="LineParseException">Thrown when the database returned incorrect values for 
        /// required properties.</exception>
        private bool GetUsePreprocessorClosureByTrackId(long trackId)
        {
            var trackParameter = new SQLiteParameter
            {
                DbType = DbType.String,
                ParameterName = TracksTableDefinitions.TrackId,
                Value = trackId
            };

            try
            {
                return ReadUsePreprocessorClosure(trackParameter);
            }
            catch (SQLiteException e)
            {
                string message = new FileReaderErrorMessageBuilder(Path).Build(Resources.HydraulicLocationConfigurationDatabaseReader_Critical_Unexpected_Exception);
                throw new CriticalFileReadException(message, e);
            }
            catch (InvalidCastException e)
            {
                string message = new FileReaderErrorMessageBuilder(Path).Build(Resources.HydraulicBoundaryDatabaseReader_Critical_Unexpected_value_on_column);
                throw new LineParseException(message, e);
            }
        }

        /// <summary>
        /// Reads the use preprocessor closure from the database.
        /// </summary>
        /// <param name="trackParameter">A parameter containing the hydraulic boundary track id.</param>
        /// <returns>The read indicator whether to use the preprocessor closure.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when the information could not be read
        /// from the database file.</exception>
        /// <exception cref="SQLiteException">Thrown when the database query failed.</exception>
        /// <exception cref="InvalidCastException">Thrown when the database returned incorrect values for 
        /// required properties.</exception>
        private bool ReadUsePreprocessorClosure(SQLiteParameter trackParameter)
        {
            string query = HydraulicLocationConfigurationDatabaseQueryBuilder.GetRegionByTrackIdQuery();

            using (IDataReader dataReader = CreateDataReader(query, trackParameter))
            {
                DataTable schemaTable = dataReader.GetSchemaTable();
                DataColumn columnName = schemaTable.Columns[schemaTable.Columns.IndexOf("ColumnName")];

                if (schemaTable.Rows.Cast<DataRow>().All(row => row[columnName].ToString() != RegionsTableDefinitions.UsePreprocessorClosure))
                {
                    return false;
                }

                if (MoveNext(dataReader))
                {
                    return Convert.ToBoolean(dataReader[RegionsTableDefinitions.UsePreprocessorClosure]);
                }

                string message = new FileReaderErrorMessageBuilder(Path).Build(Resources.HydraulicLocationConfigurationDatabaseReader_Critical_Unexpected_Exception);
                throw new CriticalFileReadException(message);
            }
        }
    }
}