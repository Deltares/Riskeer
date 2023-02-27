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
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Core.Common.Util.Builders;
using Riskeer.HydraRing.IO.Properties;

namespace Riskeer.HydraRing.IO.HydraulicLocationConfigurationDatabase
{
    /// <summary>
    /// Class for reading information from a hydraulic location configuration database.
    /// </summary>
    public class HydraulicLocationConfigurationDatabaseReader : SqLiteDatabaseReaderBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="HydraulicLocationConfigurationDatabaseReader"/>, which will use the
        /// <paramref name="databaseFilePath"/> as its source.
        /// </summary>
        /// <param name="databaseFilePath">The path of the database file to open.</param>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>the <paramref name="databaseFilePath"/> contains invalid characters;</item>
        /// <item>no file could be found at <paramref name="databaseFilePath"/>.</item>
        /// </list>
        /// </exception>
        public HydraulicLocationConfigurationDatabaseReader(string databaseFilePath) : base(databaseFilePath) {}

        /// <summary>
        /// Reads the hydraulic location configuration database.
        /// </summary>
        /// <param name="trackId">The track id to read the location configurations for.</param>
        /// <returns>A read hydraulic location configuration database.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when the hydraulic location configuration database could not
        /// be read.</exception>
        /// <exception cref="LineParseException">Thrown when the database returned incorrect values for required properties.</exception>
        public ReadHydraulicLocationConfigurationDatabase Read(long trackId)
        {
            return new ReadHydraulicLocationConfigurationDatabase(GetFromDatabase(ReadLocations).Where(rhl => rhl.TrackId == trackId),
                                                                  GetFromDatabase(IsScenarioInformationTablePresent)
                                                                      ? GetFromDatabase(ReadConfigurationSettings)
                                                                      : null,
                                                                  GetFromDatabase(ReadTracks),
                                                                  GetFromDatabase(ReadTracks).First(rt => rt.TrackId == trackId).UsePreprocessorClosure);
        }

        /// <summary>
        /// Reads the hydraulic locations from the database.
        /// </summary>
        /// <returns>A collection of <see cref="ReadHydraulicLocation"/> as found in the database.</returns>
        /// <exception cref="SQLiteException">Thrown when the database query failed.</exception>
        /// <exception cref="ConversionException">Thrown when the database returned incorrect values for required properties.</exception>
        private IEnumerable<ReadHydraulicLocation> ReadLocations()
        {
            var readHydraulicLocations = new Collection<ReadHydraulicLocation>();

            using (IDataReader dataReader = CreateDataReader(HydraulicLocationConfigurationDatabaseQueryBuilder.GetLocationsQuery()))
            {
                while (MoveNext(dataReader))
                {
                    readHydraulicLocations.Add(new ReadHydraulicLocation(dataReader.Read<long>(LocationsTableDefinitions.LocationId),
                                                                         dataReader.Read<long>(LocationsTableDefinitions.HrdLocationId),
                                                                         dataReader.Read<long>(LocationsTableDefinitions.TrackId)));
                }
            }

            return readHydraulicLocations;
        }

        /// <summary>
        /// Determines whether the table related to the scenario information is present in the database.
        /// </summary>
        /// <returns><c>true</c> if the table is present; <c>false</c> otherwise.</returns>
        /// <exception cref="SQLiteException">Thrown when the database query failed.</exception>
        /// <exception cref="ConversionException">Thrown when the database returned incorrect values for required properties.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when the information could not be read from the database.</exception>
        private bool IsScenarioInformationTablePresent()
        {
            using (IDataReader dataReader = CreateDataReader(HydraulicLocationConfigurationDatabaseQueryBuilder.GetIsScenarioInformationPresentQuery()))
            {
                if (dataReader.Read())
                {
                    return dataReader.Read<bool>(ScenarioInformationTableDefinitions.IsScenarioInformationPresent);
                }

                string message = new FileReaderErrorMessageBuilder(Path).Build(Resources.HydraulicBoundaryDatabaseReader_Critical_Unexpected_value_on_column);
                throw new CriticalFileReadException(message);
            }
        }

        /// <summary>
        /// Reads the hydraulic location configuration settings from the database.
        /// </summary>
        /// <returns>A collection of the read hydraulic location configuration database settings.</returns>
        /// <exception cref="SQLiteException">Thrown when the database query failed.</exception>
        /// <exception cref="ConversionException">Thrown when the database returned incorrect values for required properties.</exception>
        private IEnumerable<ReadHydraulicLocationConfigurationDatabaseSettings> ReadConfigurationSettings()
        {
            var readSettings = new Collection<ReadHydraulicLocationConfigurationDatabaseSettings>();

            using (IDataReader dataReader = CreateDataReader(HydraulicLocationConfigurationDatabaseQueryBuilder.GetScenarioInformationQuery()))
            {
                while (MoveNext(dataReader))
                {
                    readSettings.Add(new ReadHydraulicLocationConfigurationDatabaseSettings(dataReader.Read<string>(ScenarioInformationTableDefinitions.ScenarioName),
                                                                                            dataReader.Read<int>(ScenarioInformationTableDefinitions.Year),
                                                                                            dataReader.Read<string>(ScenarioInformationTableDefinitions.Scope),
                                                                                            dataReader.Read<string>(ScenarioInformationTableDefinitions.SeaLevel),
                                                                                            dataReader.Read<string>(ScenarioInformationTableDefinitions.RiverDischarge),
                                                                                            dataReader.Read<string>(ScenarioInformationTableDefinitions.LakeLevel),
                                                                                            dataReader.Read<string>(ScenarioInformationTableDefinitions.WindDirection),
                                                                                            dataReader.Read<string>(ScenarioInformationTableDefinitions.WindSpeed),
                                                                                            dataReader.Read<string>(ScenarioInformationTableDefinitions.Comment)));
                }
            }

            return readSettings;
        }

        /// <summary>
        /// Reads the tracks from the database.
        /// </summary>
        /// <returns>A collection of <see cref="ReadTrack"/> as found in the database.</returns>
        /// <exception cref="SQLiteException">Thrown when the database query failed.</exception>
        /// <exception cref="ConversionException">Thrown when the database returned incorrect values for required properties.</exception>
        private IEnumerable<ReadTrack> ReadTracks()
        {
            var readTracks = new Collection<ReadTrack>();

            using (IDataReader dataReader = CreateDataReader(HydraulicLocationConfigurationDatabaseQueryBuilder.GetTracksQuery()))
            {
                bool isUsePreprocessorClosureColumnPresent = IsUsePreprocessorClosureColumnPresent(dataReader);

                while (MoveNext(dataReader))
                {
                    readTracks.Add(new ReadTrack(dataReader.Read<long>(TracksTableDefinitions.TrackId),
                                                 isUsePreprocessorClosureColumnPresent && dataReader.Read<bool>(RegionsTableDefinitions.UsePreprocessorClosure)));
                }
            }

            return readTracks;
        }

        private static bool IsUsePreprocessorClosureColumnPresent(IDataReader dataReader)
        {
            DataTable schemaTable = dataReader.GetSchemaTable();
            DataColumn columnName = schemaTable.Columns[schemaTable.Columns.IndexOf("ColumnName")];

            return schemaTable.Rows.Cast<DataRow>().Any(row => row[columnName].ToString() == RegionsTableDefinitions.UsePreprocessorClosure);
        }

        /// <summary>
        /// Gets data of type <typeparamref name="T"/> from the database.
        /// </summary>
        /// <param name="readFromDatabaseFunc">The <see cref="Func{T}"/> for reading data from the database.</param>
        /// <typeparam name="T">The type of data to read from the database.</typeparam>
        /// <returns>The read data of type <typeparamref name="T"/>.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when the database query failed or the data could not be read.</exception>
        /// <exception cref="LineParseException">Thrown when the database returned incorrect values for required properties.</exception>
        private T GetFromDatabase<T>(Func<T> readFromDatabaseFunc)
        {
            try
            {
                return readFromDatabaseFunc();
            }
            catch (SQLiteException exception)
            {
                throw new CriticalFileReadException(
                    new FileReaderErrorMessageBuilder(Path).Build(Resources.HydraulicLocationConfigurationDatabaseReader_Critical_Unexpected_Exception),
                    exception);
            }
            catch (ConversionException exception)
            {
                throw new LineParseException(
                    new FileReaderErrorMessageBuilder(Path).Build(Resources.HydraulicBoundaryDatabaseReader_Critical_Unexpected_value_on_column),
                    exception);
            }
        }
    }
}