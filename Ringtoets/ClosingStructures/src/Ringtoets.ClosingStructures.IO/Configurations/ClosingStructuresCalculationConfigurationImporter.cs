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
using Core.Common.Base.Data;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.IO.Configurations.Helpers;
using Ringtoets.ClosingStructures.IO.Properties;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.Configurations.Helpers;
using Ringtoets.Common.IO.Configurations.Import;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.ClosingStructures.IO.Configurations
{
    /// <summary>
    /// Class for importing a configuration of <see cref="ClosingStructuresCalculationConfiguration"/> from an XML file and storing
    /// it on a <see cref="CalculationGroup"/>.
    /// </summary>
    public class ClosingStructuresCalculationConfigurationImporter
        : CalculationConfigurationImporter<ClosingStructuresCalculationConfigurationReader, ClosingStructuresCalculationConfiguration>
    {
        private readonly IEnumerable<HydraulicBoundaryLocation> availableHydraulicBoundaryLocations;
        private readonly IEnumerable<ForeshoreProfile> availableForeshoreProfiles;
        private readonly IEnumerable<ClosingStructure> availableStructures;

        /// <summary>
        /// Create new instance of <see cref="ClosingStructuresCalculationConfigurationImporter"/>
        /// </summary>
        /// <param name="xmlFilePath">The path to the XML file to import from.</param>
        /// <param name="importTarget">The calculation group to update.</param>
        /// <param name="hydraulicBoundaryLocations">The hydraulic boundary locations
        /// used to check if the imported objects contain the right location.</param>
        /// <param name="foreshoreProfiles">The foreshore profiles used to check if 
        /// the imported objects contain the right foreshore profile.</param>
        /// <param name="structures">The structures used to check if
        /// the imported objects contain the right structure.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public ClosingStructuresCalculationConfigurationImporter(
            string xmlFilePath,
            CalculationGroup importTarget,
            IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations,
            IEnumerable<ForeshoreProfile> foreshoreProfiles,
            IEnumerable<ClosingStructure> structures)
            : base(xmlFilePath, importTarget)
        {
            if (hydraulicBoundaryLocations == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocations));
            }
            if (foreshoreProfiles == null)
            {
                throw new ArgumentNullException(nameof(foreshoreProfiles));
            }
            if (structures == null)
            {
                throw new ArgumentNullException(nameof(structures));
            }
            availableHydraulicBoundaryLocations = hydraulicBoundaryLocations;
            availableForeshoreProfiles = foreshoreProfiles;
            availableStructures = structures;
        }

        protected override ClosingStructuresCalculationConfigurationReader CreateCalculationConfigurationReader(string xmlFilePath)
        {
            return new ClosingStructuresCalculationConfigurationReader(xmlFilePath);
        }

        protected override ICalculation ParseReadCalculation(ClosingStructuresCalculationConfiguration readCalculation)
        {
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                Name = readCalculation.Name
            };

            if (TryReadStructure(readCalculation.StructureName, calculation)
                && TryReadHydraulicBoundaryLocation(readCalculation.HydraulicBoundaryLocationName, calculation)
                && TryReadForeshoreProfile(readCalculation.ForeshoreProfileName, calculation)
                && TryReadStochasts(readCalculation, calculation)
                && TryReadOrientation(readCalculation, calculation)
                && TryReadFailureProbabilityStructureWithErosion(readCalculation, calculation)
                && TryReadFailureProbabilityOpenStructure(readCalculation, calculation)
                && TryReadFailureProbabilityReparation(readCalculation, calculation)
                && TryReadProbabilityOrFrequencyOpenStructureBeforeFlooding(readCalculation, calculation)
                && TryReadInflowModelType(readCalculation, calculation)
                && TryReadIdenticalApertures(readCalculation, calculation)
                && readCalculation.WaveReduction.ValidateWaveReduction(calculation.InputParameters.ForeshoreProfile, calculation.Name, Log))
            {
                ReadFactorStormDurationOpenStructure(readCalculation, calculation);
                ReadWaveReductionParameters(readCalculation.WaveReduction, calculation.InputParameters);
                return calculation;
            }
            return null;
        }

        private bool TryReadStochasts(ClosingStructuresCalculationConfiguration readCalculation, StructuresCalculation<ClosingStructuresInput> calculation)
        {
            if (!readCalculation.ValidateStructureBaseStochasts(Log))
            {
                return false;
            }
            if (!ValidateStochasts(readCalculation))
            {
                return false;
            }

            return TryReadStandardDeviationStochast(
                       ClosingStructuresConfigurationSchemaIdentifiers.LevelCrestStructureNotClosingStochastName,
                       calculation.Name,
                       calculation.InputParameters,
                       readCalculation.LevelCrestStructureNotClosing,
                       i => i.LevelCrestStructureNotClosing, (i, d) => i.LevelCrestStructureNotClosing = d)
                   && TryReadStandardDeviationStochast(
                       ClosingStructuresConfigurationSchemaIdentifiers.AreaFlowAperturesStochastName,
                       calculation.Name,
                       calculation.InputParameters,
                       readCalculation.AreaFlowApertures,
                       i => i.AreaFlowApertures, (i, d) => i.AreaFlowApertures = d)
                   && TryReadStandardDeviationStochast(
                       ClosingStructuresConfigurationSchemaIdentifiers.DrainCoefficientStochastName,
                       calculation.Name,
                       calculation.InputParameters,
                       readCalculation.DrainCoefficient,
                       i => i.DrainCoefficient, (i, d) => i.DrainCoefficient = d)
                   && TryReadStandardDeviationStochast(
                       ClosingStructuresConfigurationSchemaIdentifiers.InsideWaterLevelStochastName,
                       calculation.Name,
                       calculation.InputParameters,
                       readCalculation.InsideWaterLevel,
                       i => i.InsideWaterLevel, (i, d) => i.InsideWaterLevel = d)
                   && TryReadStandardDeviationStochast(
                       ClosingStructuresConfigurationSchemaIdentifiers.ThresholdHeightOpenWeirStochastName,
                       calculation.Name,
                       calculation.InputParameters,
                       readCalculation.ThresholdHeightOpenWeir,
                       i => i.ThresholdHeightOpenWeir, (i, d) => i.ThresholdHeightOpenWeir = d)
                   && TryReadStandardDeviationStochast(
                       ConfigurationSchemaIdentifiers.AllowedLevelIncreaseStorageStochastName,
                       calculation.Name,
                       calculation.InputParameters,
                       readCalculation.AllowedLevelIncreaseStorage,
                       i => i.AllowedLevelIncreaseStorage, (i, d) => i.AllowedLevelIncreaseStorage = d)
                   && TryReadStandardDeviationStochast(
                       ConfigurationSchemaIdentifiers.FlowWidthAtBottomProtectionStochastName,
                       calculation.Name,
                       calculation.InputParameters,
                       readCalculation.FlowWidthAtBottomProtection,
                       i => i.FlowWidthAtBottomProtection, (i, d) => i.FlowWidthAtBottomProtection = d)
                   && TryReadStandardDeviationStochast(
                       ConfigurationSchemaIdentifiers.ModelFactorSuperCriticalFlowStochastName,
                       calculation.Name,
                       calculation.InputParameters,
                       readCalculation.ModelFactorSuperCriticalFlow,
                       i => i.ModelFactorSuperCriticalFlow, (i, d) => i.ModelFactorSuperCriticalFlow = d)
                   && TryReadStandardDeviationStochast(
                       ConfigurationSchemaIdentifiers.WidthFlowAperturesStochastName,
                       calculation.Name,
                       calculation.InputParameters,
                       readCalculation.WidthFlowApertures, i => i.WidthFlowApertures, (i, d) => i.WidthFlowApertures = d)
                   && TryReadVariationCoefficientStochast(
                       ConfigurationSchemaIdentifiers.CriticalOvertoppingDischargeStochastName,
                       calculation.Name,
                       calculation.InputParameters,
                       readCalculation.CriticalOvertoppingDischarge,
                       i => i.CriticalOvertoppingDischarge, (i, d) => i.CriticalOvertoppingDischarge = d)
                   && TryReadVariationCoefficientStochast(
                       ConfigurationSchemaIdentifiers.StorageStructureAreaStochastName,
                       calculation.Name,
                       calculation.InputParameters,
                       readCalculation.StorageStructureArea,
                       i => i.StorageStructureArea, (i, d) => i.StorageStructureArea = d)
                   && TryReadVariationCoefficientStochast(
                       ConfigurationSchemaIdentifiers.StormDurationStochastName,
                       calculation.Name,
                       calculation.InputParameters,
                       readCalculation.StormDuration,
                       i => i.StormDuration, (i, d) => i.StormDuration = d);
        }

        private bool ValidateStochasts(ClosingStructuresCalculationConfiguration configuration)
        {
            if (configuration.DrainCoefficient?.StandardDeviation != null
                || configuration.DrainCoefficient?.VariationCoefficient != null)
            {
                Log.LogCalculationConversionError(Resources.CalculationConfigurationImporter_ValidateStochasts_Cannot_define_spread_for_DrainCoefficient,
                                                  configuration.Name);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Reads the orientation.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the orientation is invalid or when there is an orientation but
        /// no structure defined, <c>true</c> otherwise.</returns>
        private bool TryReadOrientation(StructuresCalculationConfiguration readCalculation, StructuresCalculation<ClosingStructuresInput> calculation)
        {
            if (readCalculation.StructureNormalOrientation.HasValue)
            {
                if (calculation.InputParameters.Structure == null)
                {
                    Log.LogCalculationConversionError(string.Format(RingtoetsCommonIOResources.CalculationConfigurationImporter_TryParameter_No_Structure_to_assign_Parameter_0_,
                                                                    RingtoetsCommonIOResources.CalculationConfigurationImporter_Orientation_DisplayName),
                                                      calculation.Name);

                    return false;
                }

                double orientation = readCalculation.StructureNormalOrientation.Value;

                try
                {
                    calculation.InputParameters.StructureNormalOrientation = (RoundedDouble) orientation;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Log.LogOutOfRangeException(string.Format(RingtoetsCommonIOResources.TryReadParameter_Value_0_ParameterName_1_is_invalid,
                                                             orientation,
                                                             RingtoetsCommonIOResources.CalculationConfigurationImporter_Orientation_DisplayName),
                                               calculation.Name,
                                               e);

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Reads the failure probability structure with erosion.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the orientation is invalid or when there is a failure probability 
        /// structure with erosion but no structure defined, <c>true</c> otherwise.</returns>
        private bool TryReadFailureProbabilityStructureWithErosion(StructuresCalculationConfiguration readCalculation, StructuresCalculation<ClosingStructuresInput> calculation)
        {
            if (readCalculation.FailureProbabilityStructureWithErosion.HasValue)
            {
                double failureProbability = readCalculation.FailureProbabilityStructureWithErosion.Value;

                try
                {
                    calculation.InputParameters.FailureProbabilityStructureWithErosion = (RoundedDouble) failureProbability;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Log.LogOutOfRangeException(string.Format(
                                                   RingtoetsCommonIOResources.TryReadParameter_Value_0_ParameterName_1_is_invalid,
                                                   failureProbability,
                                                   RingtoetsCommonIOResources.CalculationConfigurationImporter_FailureProbabilityStructureWithErosion_DisplayName),
                                               calculation.Name,
                                               e);

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Reads the factor storm duration.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        private void ReadFactorStormDurationOpenStructure(ClosingStructuresCalculationConfiguration readCalculation, StructuresCalculation<ClosingStructuresInput> calculation)
        {
            if (readCalculation.FactorStormDurationOpenStructure.HasValue)
            {
                calculation.InputParameters.FactorStormDurationOpenStructure = (RoundedDouble) readCalculation.FactorStormDurationOpenStructure.Value;
            }
        }

        /// <summary>
        /// Reads the failure probability open structure.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the failure probability open structure is invalid or when there is a failure probability
        /// open structure but no structure defined, <c>true</c> otherwise.</returns>
        private bool TryReadFailureProbabilityOpenStructure(ClosingStructuresCalculationConfiguration readCalculation, StructuresCalculation<ClosingStructuresInput> calculation)
        {
            if (readCalculation.FailureProbabilityOpenStructure.HasValue)
            {
                if (calculation.InputParameters.Structure == null)
                {
                    Log.LogCalculationConversionError(string.Format(RingtoetsCommonIOResources.CalculationConfigurationImporter_TryParameter_No_Structure_to_assign_Parameter_0_,
                                                                    RingtoetsCommonIOResources.CalculationConfigurationImporter_FailureProbabilityOpenStructure_DisplayName),
                                                      calculation.Name);

                    return false;
                }

                double failureProbability = readCalculation.FailureProbabilityOpenStructure.Value;

                try
                {
                    calculation.InputParameters.FailureProbabilityOpenStructure = (RoundedDouble) failureProbability;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Log.LogOutOfRangeException(string.Format(
                                                   RingtoetsCommonIOResources.TryReadParameter_Value_0_ParameterName_1_is_invalid,
                                                   failureProbability,
                                                   RingtoetsCommonIOResources.CalculationConfigurationImporter_FailureProbabilityOpenStructure_DisplayName),
                                               calculation.Name,
                                               e);

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Reads the failure probability reparation.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the failure probability reparation is invalid or when there is a failure probability 
        /// reparation but no structure defined, <c>true</c> otherwise.</returns>
        private bool TryReadFailureProbabilityReparation(ClosingStructuresCalculationConfiguration readCalculation, StructuresCalculation<ClosingStructuresInput> calculation)
        {
            if (readCalculation.FailureProbabilityReparation.HasValue)
            {
                if (calculation.InputParameters.Structure == null)
                {
                    Log.LogCalculationConversionError(string.Format(RingtoetsCommonIOResources.CalculationConfigurationImporter_TryParameter_No_Structure_to_assign_Parameter_0_,
                                                                    RingtoetsCommonIOResources.CalculationConfigurationImporter_FailureProbabilityReparation_DisplayName),
                                                      calculation.Name);

                    return false;
                }

                double failureProbability = readCalculation.FailureProbabilityReparation.Value;

                try
                {
                    calculation.InputParameters.FailureProbabilityReparation = (RoundedDouble) failureProbability;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Log.LogOutOfRangeException(string.Format(
                                                   RingtoetsCommonIOResources.TryReadParameter_Value_0_ParameterName_1_is_invalid,
                                                   failureProbability,
                                                   RingtoetsCommonIOResources.CalculationConfigurationImporter_FailureProbabilityReparation_DisplayName),
                                               calculation.Name,
                                               e);

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Reads the probability or frequency open structure before flooding.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the probability or frequency open structure before flooding is invalid or when there is a 
        /// probability or frequency open structure before flooding but no structure defined, <c>true</c> otherwise.</returns>
        private bool TryReadProbabilityOrFrequencyOpenStructureBeforeFlooding(ClosingStructuresCalculationConfiguration readCalculation, StructuresCalculation<ClosingStructuresInput> calculation)
        {
            if (readCalculation.ProbabilityOrFrequencyOpenStructureBeforeFlooding.HasValue)
            {
                if (calculation.InputParameters.Structure == null)
                {
                    Log.LogCalculationConversionError(string.Format(RingtoetsCommonIOResources.CalculationConfigurationImporter_TryParameter_No_Structure_to_assign_Parameter_0_,
                                                                    RingtoetsCommonIOResources.CalculationConfigurationImporter_ProbabilityOrFrequencyOpenStructureBeforeFlooding_DisplayName),
                                                      calculation.Name);

                    return false;
                }

                double failureProbability = readCalculation.ProbabilityOrFrequencyOpenStructureBeforeFlooding.Value;

                try
                {
                    calculation.InputParameters.ProbabilityOrFrequencyOpenStructureBeforeFlooding = (RoundedDouble) failureProbability;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Log.LogOutOfRangeException(string.Format(
                                                   RingtoetsCommonIOResources.TryReadParameter_Value_0_ParameterName_1_is_invalid,
                                                   failureProbability,
                                                   RingtoetsCommonIOResources.CalculationConfigurationImporter_ProbabilityOrFrequencyOpenStructureBeforeFlooding_DisplayName),
                                               calculation.Name,
                                               e);

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Reads the inflow model type.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the inflow model type is invalid or when there is a 
        /// inflow model type but no structure defined, <c>true</c> otherwise.</returns>
        private bool TryReadInflowModelType(ClosingStructuresCalculationConfiguration readCalculation, StructuresCalculation<ClosingStructuresInput> calculation)
        {
            if (readCalculation.InflowModelType.HasValue)
            {
                if (calculation.InputParameters.Structure == null)
                {
                    Log.LogCalculationConversionError(string.Format(RingtoetsCommonIOResources.CalculationConfigurationImporter_TryParameter_No_Structure_to_assign_Parameter_0_,
                                                                    RingtoetsCommonIOResources.CalculationConfigurationImporter_InflowModelType_DisplayName),
                                                      calculation.Name);

                    return false;
                }

                calculation.InputParameters.InflowModelType =
                    (ClosingStructureInflowModelType) new ConfigurationClosingStructureInflowModelTypeConverter()
                        .ConvertTo(readCalculation.InflowModelType.Value, typeof(ClosingStructureInflowModelType));
            }

            return true;
        }

        /// <summary>
        /// Reads the number of identical apertures.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the number of identical apertures is invalid or when there is a 
        /// number of identical apertures but no structure defined, <c>true</c> otherwise.</returns>
        private bool TryReadIdenticalApertures(ClosingStructuresCalculationConfiguration readCalculation, StructuresCalculation<ClosingStructuresInput> calculation)
        {
            if (readCalculation.IdenticalApertures.HasValue)
            {
                if (calculation.InputParameters.Structure == null)
                {
                    Log.LogCalculationConversionError(string.Format(RingtoetsCommonIOResources.CalculationConfigurationImporter_TryParameter_No_Structure_to_assign_Parameter_0_,
                                                                    RingtoetsCommonIOResources.CalculationConfigurationImporter_IdenticalApertures_DisplayName),
                                                      calculation.Name);

                    return false;
                }

                calculation.InputParameters.IdenticalApertures = readCalculation.IdenticalApertures.Value;
            }

            return true;
        }

        private bool TryReadHydraulicBoundaryLocation(string locationName, StructuresCalculation<ClosingStructuresInput> calculation)
        {
            HydraulicBoundaryLocation location;

            if (TryReadHydraulicBoundaryLocation(locationName, calculation.Name, availableHydraulicBoundaryLocations, out location))
            {
                calculation.InputParameters.HydraulicBoundaryLocation = location;
                return true;
            }

            return false;
        }

        private bool TryReadStructure(string structureName, StructuresCalculation<ClosingStructuresInput> calculation)
        {
            ClosingStructure structure;

            if (TryReadStructure(structureName, calculation.Name, availableStructures, out structure))
            {
                calculation.InputParameters.Structure = structure;
                return true;
            }

            return false;
        }

        private bool TryReadForeshoreProfile(string foreshoreProfileName, StructuresCalculation<ClosingStructuresInput> calculation)
        {
            ForeshoreProfile foreshoreProfile;

            if (TryReadForeshoreProfile(foreshoreProfileName, calculation.Name, availableForeshoreProfiles, out foreshoreProfile))
            {
                calculation.InputParameters.ForeshoreProfile = foreshoreProfile;
                return true;
            }

            return false;
        }
    }
}