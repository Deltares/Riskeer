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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Settings;

namespace Ringtoets.Common.IO.HydraRing
{
    /// <summary>
    /// This class is responsible for reading values from the Hydra-Ring settings database and creating
    /// settings from them.
    /// </summary>
    public class HydraRingSettingsDatabaseReader : SqLiteDatabaseReaderBase
    {
        private const string minColumn = "Min";
        private const string maxColumn = "Max";

        private const string subMechanismIdColumn = "SubMechanismID";
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

        private const string timeIntegrationSchemeIdColumn = "TimeIntegrationSchemeID";

        private string locationIdColumn = "LocationID";

        private const string locationIdParameterName = "@locationId";
        private const string calculationTypeIdParameterName = "@calculationTypeId";
        private const string mechanismIdParameterName = "@mechanismID";

        private readonly string designTableSettingsForLocationAndCalculationTypeQuery;
        private readonly string numericSettingsForLocationMechanismAndSubMechanismQuery;
        private readonly string excludedLocationsQuery;
        private readonly string timeIntegrationSettingsForLocationAndCalculationTypeQuery;

        /// <summary>
        /// Creates a new instance of <see cref="HydraRingSettingsDatabaseReader"/>.
        /// </summary>
        /// <param name="databaseFilePath">The full path to the database file to use when reading settings.</param>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>The <paramref name="databaseFilePath"/> contains invalid characters.</item>
        /// <item>No file could be found at <paramref name="databaseFilePath"/>.</item>
        /// <item>Unable to open database file.</item>
        /// </list>
        /// </exception>
        public HydraRingSettingsDatabaseReader(string databaseFilePath)
            : base(databaseFilePath)
        {
            designTableSettingsForLocationAndCalculationTypeQuery = string.Format(
                "SELECT {0}, {1} FROM DesignTablesSettings WHERE LocationID = {2} AND CalculationTypeID = {3}",
                minColumn,
                maxColumn,
                locationIdParameterName,
                calculationTypeIdParameterName);

            numericSettingsForLocationMechanismAndSubMechanismQuery = string.Format(
                "SELECT {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14} " +
                "FROM NumericsSettings WHERE LocationID = {15} AND MechanismID = {16}",
                subMechanismIdColumn,
                calculationTechniqueIdColumn,
                formStartMethodColumn,
                formNumberOfIterationsColumn,
                formRelaxationFactorColumn,
                formEpsBetaColumn,
                formEpsHohColumn,
                formEpsZFuncColumn,
                dsStartMethodColumn,
                dsMinNumberOfIterationsColumn,
                dsMaxNumberOfIterationsColumn,
                dsVarCoefficientColumn,
                niUMinColumn,
                niUMaxColumn,
                niNumberStepsColumn,
                locationIdParameterName,
                mechanismIdParameterName);

            timeIntegrationSettingsForLocationAndCalculationTypeQuery = string.Format(
                "SELECT {0} FROM TimeIntegrationSettings WHERE LocationID = {1} AND CalculationTypeID = {2}",
                timeIntegrationSchemeIdColumn,
                locationIdParameterName,
                calculationTypeIdParameterName);

            excludedLocationsQuery = string.Format(
                "SELECT {0} FROM ExcludedLocations",
                locationIdColumn);
        }

        /// <summary>
        /// Read a design tables setting for a given location and <see cref="HydraRingFailureMechanismType"/>.
        /// </summary>
        /// <param name="locationId">The id of a hydraulic boundary location.</param>
        /// <param name="calculationType">The type of the calculation to obtain the <see cref="DesignTablesSetting"/> for.</param>
        /// <returns>A new <see cref="DesignTablesSetting"/> containing values read from the database.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="calculationType"/> is not a valid
        /// <see cref="HydraRingFailureMechanismType"/> value.</exception>
        public DesignTablesSetting ReadDesignTableSetting(long locationId, HydraRingFailureMechanismType calculationType)
        {
            if (!Enum.IsDefined(calculationType.GetType(), calculationType))
            {
                throw new InvalidEnumArgumentException("calculationType", (int) calculationType, calculationType.GetType());
            }
            
            var reader = CreateDesignTablesDataReader(locationId, calculationType);
            if (MoveNext(reader))
            {
                return new DesignTablesSetting(
                    Convert.ToDouble(reader[minColumn]),
                    Convert.ToDouble(reader[maxColumn]));
            }
            return null;
        }

        /// <summary>
        /// Read a numerics setting for a given location and <see cref="HydraRingFailureMechanismType"/>.
        /// </summary>
        /// <param name="locationId">The id of a hydraulic boundary location.</param>
        /// <param name="mechanimsId">The mechanism id to obtain the <see cref="NumericsSetting"/> for.</param>
        /// <returns>A new <see cref="Dictionary{T, T}"/> where the key is the submechanism id, and the value is
        /// the <see cref="NumericsSetting"/> containing values read from the database.</returns>
        public Dictionary<int, NumericsSetting> ReadNumericsSetting(long locationId, int mechanimsId)
        {
            var settings = new Dictionary<int, NumericsSetting>();
            var reader = CreateNumericsSettingsDataReader(locationId, mechanimsId);
            while (MoveNext(reader))
            {
                settings.Add(
                    Convert.ToInt32(reader[subMechanismIdColumn]),
                    new NumericsSetting(
                        Convert.ToInt32(reader[calculationTechniqueIdColumn]),
                        Convert.ToInt32(reader[formStartMethodColumn]),
                        Convert.ToInt32(reader[formNumberOfIterationsColumn]),
                        Convert.ToDouble(reader[formRelaxationFactorColumn]),
                        Convert.ToDouble(reader[formEpsBetaColumn]),
                        Convert.ToDouble(reader[formEpsHohColumn]),
                        Convert.ToDouble(reader[formEpsZFuncColumn]),
                        Convert.ToInt32(reader[dsStartMethodColumn]),
                        Convert.ToInt32(reader[dsMinNumberOfIterationsColumn]),
                        Convert.ToInt32(reader[dsMaxNumberOfIterationsColumn]),
                        Convert.ToDouble(reader[dsVarCoefficientColumn]),
                        Convert.ToDouble(reader[niUMinColumn]),
                        Convert.ToDouble(reader[niUMaxColumn]),
                        Convert.ToInt32(reader[niNumberStepsColumn])));
            }
            return settings;
        }

        /// <summary>
        /// Read a time integration setting for a given location and <see cref="HydraRingFailureMechanismType"/>.
        /// </summary>
        /// <param name="locationId">The id of a hydraulic boundary location.</param>
        /// <param name="calculationType">The type of the calculation to obtain the <see cref="DesignTablesSetting"/> for.</param>
        /// <returns>A new <see cref="DesignTablesSetting"/> containing values read from the database.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="calculationType"/> is not a valid
        /// <see cref="HydraRingFailureMechanismType"/> value.</exception>
        public HydraulicModelsSetting ReadTimeIntegrationSetting(long locationId, HydraRingFailureMechanismType calculationType)
        {
            if (!Enum.IsDefined(calculationType.GetType(), calculationType))
            {
                throw new InvalidEnumArgumentException("calculationType", (int)calculationType, calculationType.GetType());
            }

            var reader = CreateTimeIntegrationDataReader(locationId, calculationType);
            if (MoveNext(reader))
            {
                return new HydraulicModelsSetting(
                    Convert.ToInt32(reader[timeIntegrationSchemeIdColumn]));
            }
            return null;
        }

        /// <summary>
        /// Reads the excluded locations (those for which no calculation is possible) from the database.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of id's for all the excluded locations.</returns>
        public IEnumerable<long> ReadExcludedLocations()
        {
            var reader = CreateExcludedLocationsDataReader();
            while (MoveNext(reader))
            {
                yield return Convert.ToInt64(reader[locationIdColumn]);
            }
        }

        private SQLiteDataReader CreateDesignTablesDataReader(long locationId, HydraRingFailureMechanismType calculationType)
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
                Value = (int)calculationType
            };

            return CreateDataReader(
                designTableSettingsForLocationAndCalculationTypeQuery,
                locationParameter, 
                typeParameter);
        }

        private SQLiteDataReader CreateNumericsSettingsDataReader(long locationId, int mechanimsId)
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
                Value = mechanimsId
            };

            return CreateDataReader(
                numericSettingsForLocationMechanismAndSubMechanismQuery,
                locationParameter,
                mechanismIdParameter);
        }

        private SQLiteDataReader CreateTimeIntegrationDataReader(long locationId, HydraRingFailureMechanismType calculationType)
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
                Value = (int)calculationType
            };

            return CreateDataReader(
                timeIntegrationSettingsForLocationAndCalculationTypeQuery,
                locationParameter,
                typeParameter);
        }

        private SQLiteDataReader CreateExcludedLocationsDataReader()
        {
            return CreateDataReader(excludedLocationsQuery);
        }
    }
}