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
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Ringtoets.Common.IO.Properties;
using Riskeer.HydraRing.Calculation.Data;
using Riskeer.HydraRing.Calculation.Data.Settings;

namespace Ringtoets.Common.IO.HydraRing
{
    /// <summary>
    /// This class is responsible for reading values from the Hydra-Ring settings database and creating
    /// settings from them.
    /// </summary>
    public class HydraRingSettingsDatabaseReader : SqLiteDatabaseReaderBase
    {
        private const string calculationTechniqueIdColumn = "CalculationMethod";
        private const string formStartMethodColumn = "FORM_StartMethod";
        private const string formNumberOfIterationsColumn = "FORM_NIterations";
        private const string formRelaxationFactorColumn = "FORM_RelaxationFactor";
        private const string formEpsBetaColumn = "FORM_EpsBeta";
        private const string formEpsHohColumn = "FORM_EpsHOH";
        private const string formEpsZFuncColumn = "FORM_EpsZFunc";
        private const string dsStartMethodColumn = "DS_StartMethod";
        private const string dsMinNumberOfIterationsColumn = "DS_Min";
        private const string dsMaxNumberOfIterationsColumn = "DS_Max";
        private const string dsVarCoefficientColumn = "DS_VarCoefficient";
        private const string niUMinColumn = "NI_UMin";
        private const string niUMaxColumn = "NI_UMax";
        private const string niNumberStepsColumn = "NI_NumberSteps";

        private const string minColumn = "Min";
        private const string maxColumn = "Max";

        private const string locationIdParameterName = "@locationId";
        private const string calculationTypeIdParameterName = "@calculationTypeId";
        private const string mechanismIdParameterName = "@mechanismID";
        private const string subMechanismIdParameterName = "@subMechanismID";
        private const string timeIntegrationSchemeIdColumn = "TimeIntegrationSchemeID";

        private const string locationIdColumn = "LocationID";

        private const string minValueRunPreprocessorColumn = "MinValueRunPreprocessor";
        private const string maxValueRunPreprocessorColumn = "MaxValueRunPreprocessor";

        private readonly string designTablesSettingsForLocationAndCalculationTypeQuery;
        private readonly string numericsSettingsForLocationMechanismAndSubMechanismQuery;
        private readonly string excludedLocationsQuery;
        private readonly string excludedPreprocessorLocationsQuery;
        private readonly string timeIntegrationSettingsForLocationAndCalculationTypeQuery;
        private readonly string preprocessorSettingsForLocationQuery;

        /// <summary>
        /// Creates a new instance of <see cref="HydraRingSettingsDatabaseReader"/>.
        /// </summary>
        /// <param name="databaseFilePath">The full path to the database file to use when reading settings.</param>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>The <paramref name="databaseFilePath"/> contains invalid characters.</item>
        /// <item>No file could be found at <paramref name="databaseFilePath"/>.</item>
        /// <item>Unable to open database file.</item>
        /// <item>The opened database doesn't have the expected schema.</item>
        /// </list>
        /// </exception>
        public HydraRingSettingsDatabaseReader(string databaseFilePath)
            : base(databaseFilePath)
        {
            designTablesSettingsForLocationAndCalculationTypeQuery = $"SELECT {minColumn}, {maxColumn} " +
                                                                     "FROM DesignTablesSettings " +
                                                                     $"WHERE LocationID = {locationIdParameterName} " +
                                                                     $"AND CalculationTypeID = {calculationTypeIdParameterName}";

            numericsSettingsForLocationMechanismAndSubMechanismQuery = $"SELECT {calculationTechniqueIdColumn}, {formStartMethodColumn}, " +
                                                                       $"{formNumberOfIterationsColumn}, {formRelaxationFactorColumn}, " +
                                                                       $"{formEpsBetaColumn}, {formEpsHohColumn}, " +
                                                                       $"{formEpsZFuncColumn}, {dsStartMethodColumn}, " +
                                                                       $"{dsMinNumberOfIterationsColumn}, {dsMaxNumberOfIterationsColumn}, " +
                                                                       $"{dsVarCoefficientColumn}, {niUMinColumn}, " +
                                                                       $"{niUMaxColumn}, {niNumberStepsColumn} " +
                                                                       "FROM NumericsSettings " +
                                                                       $"WHERE LocationID = {locationIdParameterName} " +
                                                                       $"AND MechanismID = {mechanismIdParameterName} " +
                                                                       $"AND SubMechanismID = {subMechanismIdParameterName}";

            timeIntegrationSettingsForLocationAndCalculationTypeQuery = $"SELECT {timeIntegrationSchemeIdColumn} " +
                                                                        "FROM TimeIntegrationSettings " +
                                                                        $"WHERE LocationID = {locationIdParameterName} " +
                                                                        $"AND CalculationTypeID = {calculationTypeIdParameterName}";

            excludedLocationsQuery = $"SELECT {locationIdColumn} FROM ExcludedLocations";

            preprocessorSettingsForLocationQuery = $"SELECT {minValueRunPreprocessorColumn}, {maxValueRunPreprocessorColumn} " +
                                                   "FROM PreprocessorSettings " +
                                                   $"WHERE LocationID = {locationIdParameterName}";

            excludedPreprocessorLocationsQuery = $"SELECT {locationIdColumn} FROM ExcludedLocationsPreprocessor";
        }

        /// <summary>
        /// Read a design tables setting for a given location and <see cref="HydraRingFailureMechanismType"/>.
        /// </summary>
        /// <param name="locationId">The id of a hydraulic boundary location.</param>
        /// <param name="calculationType">The type of the calculation to obtain the <see cref="DesignTablesSetting"/> for.</param>
        /// <returns>A new <see cref="DesignTablesSetting"/> containing values read from the database.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="calculationType"/> is not a valid
        /// <see cref="HydraRingFailureMechanismType"/> value.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when a column that is being read doesn't
        /// contain expected type.</exception>
        public DesignTablesSetting ReadDesignTableSetting(long locationId, HydraRingFailureMechanismType calculationType)
        {
            if (!Enum.IsDefined(calculationType.GetType(), calculationType))
            {
                throw new InvalidEnumArgumentException(nameof(calculationType), (int) calculationType, calculationType.GetType());
            }

            using (IDataReader reader = CreateDesignTablesDataReader(locationId, calculationType))
            {
                if (MoveNext(reader))
                {
                    try
                    {
                        return new DesignTablesSetting(
                            reader.Read<double>(minColumn),
                            reader.Read<double>(maxColumn));
                    }
                    catch (ConversionException)
                    {
                        throw new CriticalFileReadException(Resources.HydraRingSettingsDatabase_Hydraulic_calculation_settings_database_has_invalid_schema);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Read a numerics setting for a given location and <see cref="HydraRingFailureMechanismType"/>.
        /// </summary>
        /// <param name="locationId">The id of a hydraulic boundary location.</param>
        /// <param name="mechanismId">The mechanism id to obtain the <see cref="NumericsSetting"/> for.</param>
        /// <param name="subMechanismId">The sub mechanism id to obtain the <see cref="NumericsSetting"/> for.</param>
        /// <returns>A new <see cref="NumericsSetting"/> containing values read from the database.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when a column that is being read doesn't
        /// contain expected type.</exception>
        public NumericsSetting ReadNumericsSetting(long locationId, int mechanismId, int subMechanismId)
        {
            using (IDataReader reader = CreateNumericsSettingsDataReader(locationId, mechanismId, subMechanismId))
            {
                if (MoveNext(reader))
                {
                    try
                    {
                        return new NumericsSetting(
                            reader.Read<int>(calculationTechniqueIdColumn),
                            reader.Read<int>(formStartMethodColumn),
                            reader.Read<int>(formNumberOfIterationsColumn),
                            reader.Read<double>(formRelaxationFactorColumn),
                            reader.Read<double>(formEpsBetaColumn),
                            reader.Read<double>(formEpsHohColumn),
                            reader.Read<double>(formEpsZFuncColumn),
                            reader.Read<int>(dsStartMethodColumn),
                            reader.Read<int>(dsMinNumberOfIterationsColumn),
                            reader.Read<int>(dsMaxNumberOfIterationsColumn),
                            reader.Read<double>(dsVarCoefficientColumn),
                            reader.Read<double>(niUMinColumn),
                            reader.Read<double>(niUMaxColumn),
                            reader.Read<int>(niNumberStepsColumn));
                    }
                    catch (ConversionException)
                    {
                        throw new CriticalFileReadException(Resources.HydraRingSettingsDatabase_Hydraulic_calculation_settings_database_has_invalid_schema);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Read a time integration setting for a given location and <see cref="HydraRingFailureMechanismType"/>.
        /// </summary>
        /// <param name="locationId">The id of a hydraulic boundary location.</param>
        /// <param name="calculationType">The type of the calculation to obtain the <see cref="TimeIntegrationSetting"/> for.</param>
        /// <returns>A new <see cref="TimeIntegrationSetting"/> containing values read from the database.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="calculationType"/> is not a valid
        /// <see cref="HydraRingFailureMechanismType"/> value.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when a column that is being read doesn't
        /// contain expected type.</exception>
        public TimeIntegrationSetting ReadTimeIntegrationSetting(long locationId, HydraRingFailureMechanismType calculationType)
        {
            if (!Enum.IsDefined(calculationType.GetType(), calculationType))
            {
                throw new InvalidEnumArgumentException(nameof(calculationType), (int) calculationType, calculationType.GetType());
            }

            using (IDataReader reader = CreateTimeIntegrationDataReader(locationId, calculationType))
            {
                if (MoveNext(reader))
                {
                    try
                    {
                        return new TimeIntegrationSetting(
                            reader.Read<int>(timeIntegrationSchemeIdColumn));
                    }
                    catch (ConversionException)
                    {
                        throw new CriticalFileReadException(Resources.HydraRingSettingsDatabase_Hydraulic_calculation_settings_database_has_invalid_schema);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Reads the excluded locations (those for which no calculation is possible) from the database.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of ids for all the excluded locations.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when a column that is being read doesn't
        /// contain expected type.</exception>
        public IEnumerable<long> ReadExcludedLocations()
        {
            using (IDataReader reader = CreateExcludedLocationsDataReader())
            {
                while (MoveNext(reader))
                {
                    yield return TryReadLocationIdColumn(reader);
                }
            }
        }

        /// <summary>
        /// Reads a preprocessor setting for a given location.
        /// </summary>
        /// <param name="locationId">The id of a hydraulic boundary location.</param>
        /// <returns>A new <see cref="ReadPreprocessorSetting"/> containing values read from the database.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when a column that is being read doesn't
        /// contain expected type.</exception>
        public ReadPreprocessorSetting ReadPreprocessorSetting(long locationId)
        {
            using (IDataReader reader = CreatePreprocessorSettingsDataReader(locationId))
            {
                if (MoveNext(reader))
                {
                    try
                    {
                        return new ReadPreprocessorSetting(
                            reader.Read<double>(minValueRunPreprocessorColumn),
                            reader.Read<double>(maxValueRunPreprocessorColumn));
                    }
                    catch (ConversionException)
                    {
                        throw new CriticalFileReadException(Resources.HydraRingSettingsDatabase_Hydraulic_calculation_settings_database_has_invalid_schema);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Reads the excluded preprocessor locations (those for which no preprocessor calculation is possible) from the database.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of ids for all the excluded locations.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when a column that is being read doesn't
        /// contain expected type.</exception>
        public IEnumerable<long> ReadExcludedPreprocessorLocations()
        {
            using (IDataReader reader = CreateExcludedPreprocessorLocationsDataReader())
            {
                while (MoveNext(reader))
                {
                    yield return TryReadLocationIdColumn(reader);
                }
            }
        }

        /// <summary>
        /// Tries to read the <see cref="locationIdColumn"/> from the <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">The reader to read the column's value from.</param>
        /// <returns>The id of the location that was read from the <paramref name="reader"/>.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when the <see cref="locationIdColumn"/>
        /// column contains a value of unexpected type.</exception>
        private long TryReadLocationIdColumn(IDataReader reader)
        {
            try
            {
                return reader.Read<long>(locationIdColumn);
            }
            catch (ConversionException)
            {
                throw new CriticalFileReadException(Resources.HydraRingSettingsDatabase_Hydraulic_calculation_settings_database_has_invalid_schema);
            }
        }

        private IDataReader CreateDesignTablesDataReader(long locationId, HydraRingFailureMechanismType calculationType)
        {
            var locationParameter = new SQLiteParameter
            {
                DbType = DbType.Int64,
                ParameterName = locationIdParameterName,
                Value = locationId
            };

            var typeParameter = new SQLiteParameter
            {
                DbType = DbType.Int32,
                ParameterName = calculationTypeIdParameterName,
                Value = (int) calculationType
            };

            return CreateDataReader(
                designTablesSettingsForLocationAndCalculationTypeQuery,
                locationParameter,
                typeParameter);
        }

        private IDataReader CreateNumericsSettingsDataReader(long locationId, int mechanismId, int subMechanismId)
        {
            var locationParameter = new SQLiteParameter
            {
                DbType = DbType.Int64,
                ParameterName = locationIdParameterName,
                Value = locationId
            };

            var mechanismIdParameter = new SQLiteParameter
            {
                DbType = DbType.Int32,
                ParameterName = mechanismIdParameterName,
                Value = mechanismId
            };

            var subMechanismIdParameter = new SQLiteParameter
            {
                DbType = DbType.Int32,
                ParameterName = subMechanismIdParameterName,
                Value = subMechanismId
            };

            return CreateDataReader(
                numericsSettingsForLocationMechanismAndSubMechanismQuery,
                locationParameter,
                mechanismIdParameter,
                subMechanismIdParameter);
        }

        private IDataReader CreateTimeIntegrationDataReader(long locationId, HydraRingFailureMechanismType calculationType)
        {
            var locationParameter = new SQLiteParameter
            {
                DbType = DbType.Int64,
                ParameterName = locationIdParameterName,
                Value = locationId
            };

            var typeParameter = new SQLiteParameter
            {
                DbType = DbType.Int32,
                ParameterName = calculationTypeIdParameterName,
                Value = (int) calculationType
            };

            return CreateDataReader(
                timeIntegrationSettingsForLocationAndCalculationTypeQuery,
                locationParameter,
                typeParameter);
        }

        private IDataReader CreateExcludedLocationsDataReader()
        {
            return CreateDataReader(excludedLocationsQuery);
        }

        private IDataReader CreatePreprocessorSettingsDataReader(long locationId)
        {
            var locationParameter = new SQLiteParameter
            {
                DbType = DbType.Int64,
                ParameterName = locationIdParameterName,
                Value = locationId
            };

            return CreateDataReader(
                preprocessorSettingsForLocationQuery,
                locationParameter);
        }

        private IDataReader CreateExcludedPreprocessorLocationsDataReader()
        {
            return CreateDataReader(excludedPreprocessorLocationsQuery);
        }
    }
}