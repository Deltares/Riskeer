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
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.Configurations.Helpers;
using Ringtoets.Common.IO.Configurations.Import;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.IO.Configurations.Helpers;
using Ringtoets.StabilityPointStructures.IO.Properties;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.StabilityPointStructures.IO.Configurations
{
    /// <summary>
    /// Class for importing a configuration of <see cref="StabilityPointStructuresCalculationConfiguration"/> from an XML file and storing
    /// it on a <see cref="CalculationGroup"/>.
    /// </summary>
    public class StabilityPointStructuresCalculationConfigurationImporter
        : CalculationConfigurationImporter<
            StabilityPointStructuresCalculationConfigurationReader,
            StabilityPointStructuresCalculationConfiguration>
    {
        private readonly IEnumerable<HydraulicBoundaryLocation> availableHydraulicBoundaryLocations;
        private readonly IEnumerable<ForeshoreProfile> availableForeshoreProfiles;
        private readonly IEnumerable<StabilityPointStructure> availableStructures;

        /// <summary>
        /// Create new instance of <see cref="StabilityPointStructuresCalculationConfigurationImporter"/>
        /// </summary>
        /// <param name="xmlFilePath">The path to the XML file to import from.</param>
        /// <param name="importTarget">The calculation group to update.</param>
        /// <param name="hydraulicBoundaryLocations">The hydraulic boundary locations
        /// used to check if the imported objects contain the right location.</param>
        /// <param name="foreshoreProfiles">The foreshore profiles used to check if 
        /// the imported objects contain the right foreshore profile.</param>
        /// <param name="structures">The structures used to check if
        /// the imported objects contain the right structure.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public StabilityPointStructuresCalculationConfigurationImporter(
            string xmlFilePath,
            CalculationGroup importTarget,
            IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations,
            IEnumerable<ForeshoreProfile> foreshoreProfiles,
            IEnumerable<StabilityPointStructure> structures)
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

        protected override StabilityPointStructuresCalculationConfigurationReader CreateCalculationConfigurationReader(
            string xmlFilePath)
        {
            return new StabilityPointStructuresCalculationConfigurationReader(xmlFilePath);
        }

        protected override ICalculation ParseReadCalculation(StabilityPointStructuresCalculationConfiguration readCalculation)
        {
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                Name = readCalculation.Name
            };

            if (TrySetStructure(readCalculation.StructureName, calculation)
                && TrySetHydraulicBoundaryLocation(readCalculation.HydraulicBoundaryLocationName, calculation)
                && TrySetForeshoreProfile(readCalculation.ForeshoreProfileName, calculation)
                && TryReadFailureProbabilityRepairClosure(readCalculation, calculation)
                && TryReadEvaluationLevel(readCalculation, calculation)
                && TryReadStochasts(readCalculation, calculation)
                && TryReadOrientation(readCalculation, calculation)
                && TryReadFailureProbabilityStructureWithErosion(readCalculation, calculation)
                && TryReadInflowModelType(readCalculation, calculation)
                && TryReadLoadSchematizationType(readCalculation, calculation)
                && TryReadLevellingCount(readCalculation, calculation)
                && TryReadProbabilityCollisionSecondaryStructure(readCalculation, calculation)
                && TryReadVerticalDistance(readCalculation, calculation)
                && readCalculation.WaveReduction.ValidateWaveReduction(calculation.InputParameters.ForeshoreProfile, calculation.Name, Log))
            {
                ReadFactorStormDurationOpenStructure(readCalculation, calculation);
                ReadVolumicWeightWater(readCalculation, calculation);
                ReadWaveReductionParameters(readCalculation.WaveReduction, calculation.InputParameters);
                return calculation;
            }
            return null;
        }

        /// <summary>
        /// Reads the evaluation level.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the evaluation level is invalid or when there is an
        /// evaluation level but no structure defined, <c>true</c> otherwise.</returns>
        private bool TryReadEvaluationLevel(StabilityPointStructuresCalculationConfiguration readCalculation,
                                            StructuresCalculation<StabilityPointStructuresInput> calculation)
        {
            if (readCalculation.EvaluationLevel.HasValue)
            {
                if (calculation.InputParameters.Structure == null)
                {
                    Log.LogCalculationConversionError(string.Format(RingtoetsCommonIOResources.CalculationConfigurationImporter_TryParameter_No_Structure_to_assign_Parameter_0_,
                                                                    Resources.CalculationConfigurationImporter_EvaluationLevel_DisplayName),
                                                      calculation.Name);

                    return false;
                }

                calculation.InputParameters.EvaluationLevel = (RoundedDouble) readCalculation.EvaluationLevel.Value;
            }

            return true;
        }

        private bool TryReadStochasts(StabilityPointStructuresCalculationConfiguration readCalculation,
                                      StructuresCalculation<StabilityPointStructuresInput> calculation)
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
                       ConfigurationSchemaIdentifiers.AllowedLevelIncreaseStorageStochastName,
                       calculation.Name,
                       calculation.InputParameters,
                       readCalculation.AllowedLevelIncreaseStorage,
                       i => i.AllowedLevelIncreaseStorage, (i, d) => i.AllowedLevelIncreaseStorage = d)
                   && TryReadStandardDeviationStochast(
                       StabilityPointStructuresConfigurationSchemaIdentifiers.AreaFlowAperturesStochastName,
                       calculation.Name,
                       calculation.InputParameters,
                       readCalculation.AreaFlowApertures,
                       i => i.AreaFlowApertures, (i, d) => i.AreaFlowApertures = d)
                   && TryReadStandardDeviationStochast(
                       StabilityPointStructuresConfigurationSchemaIdentifiers.BankWidthStochastName,
                       calculation.Name,
                       calculation.InputParameters,
                       readCalculation.BankWidth,
                       i => i.BankWidth, (i, d) => i.BankWidth = d)
                   && TryReadVariationCoefficientStochast(
                       ConfigurationSchemaIdentifiers.CriticalOvertoppingDischargeStochastName,
                       calculation.Name,
                       calculation.InputParameters,
                       readCalculation.CriticalOvertoppingDischarge,
                       i => i.CriticalOvertoppingDischarge, (i, d) => i.CriticalOvertoppingDischarge = d)
                   && TryReadStandardDeviationStochast(
                       StabilityPointStructuresConfigurationSchemaIdentifiers.DrainCoefficientStochastName,
                       calculation.Name,
                       calculation.InputParameters,
                       readCalculation.DrainCoefficient,
                       i => i.DrainCoefficient, (i, d) => i.DrainCoefficient = d)
                   && TryReadVariationCoefficientStochast(
                       StabilityPointStructuresConfigurationSchemaIdentifiers.FailureCollisionEnergyStochastName,
                       calculation.Name,
                       calculation.InputParameters,
                       readCalculation.FailureCollisionEnergy,
                       i => i.FailureCollisionEnergy, (i, d) => i.FailureCollisionEnergy = d)
                   && TryReadVariationCoefficientStochast(
                       StabilityPointStructuresConfigurationSchemaIdentifiers.FlowVelocityStructureClosableStochastName,
                       calculation.Name,
                       calculation.InputParameters,
                       readCalculation.FlowVelocityStructureClosable,
                       i => i.FlowVelocityStructureClosable, (i, d) => i.FlowVelocityStructureClosable = d)
                   && TryReadStandardDeviationStochast(
                       ConfigurationSchemaIdentifiers.FlowWidthAtBottomProtectionStochastName,
                       calculation.Name,
                       calculation.InputParameters,
                       readCalculation.FlowWidthAtBottomProtection,
                       i => i.FlowWidthAtBottomProtection, (i, d) => i.FlowWidthAtBottomProtection = d)
                   && TryReadStandardDeviationStochast(
                       StabilityPointStructuresConfigurationSchemaIdentifiers.InsideWaterLevelStochastName,
                       calculation.Name,
                       calculation.InputParameters,
                       readCalculation.InsideWaterLevel,
                       i => i.InsideWaterLevel, (i, d) => i.InsideWaterLevel = d)
                   && TryReadStandardDeviationStochast(
                       StabilityPointStructuresConfigurationSchemaIdentifiers.InsideWaterLevelFailureConstructionStochastName,
                       calculation.Name,
                       calculation.InputParameters,
                       readCalculation.InsideWaterLevelFailureConstruction,
                       i => i.InsideWaterLevelFailureConstruction, (i, d) => i.InsideWaterLevelFailureConstruction = d)
                   && TryReadStandardDeviationStochast(
                       ConfigurationSchemaIdentifiers.ModelFactorSuperCriticalFlowStochastName,
                       calculation.Name,
                       calculation.InputParameters,
                       readCalculation.ModelFactorSuperCriticalFlow,
                       i => i.ModelFactorSuperCriticalFlow, (i, d) => i.ModelFactorSuperCriticalFlow = d)
                   && TryReadStandardDeviationStochast(
                       StabilityPointStructuresConfigurationSchemaIdentifiers.LevelCrestStructureStochastName,
                       calculation.Name,
                       calculation.InputParameters,
                       readCalculation.LevelCrestStructure,
                       i => i.LevelCrestStructure, (i, d) => i.LevelCrestStructure = d)
                   && TryReadVariationCoefficientStochast(
                       StabilityPointStructuresConfigurationSchemaIdentifiers.ConstructiveStrengthLinearLoadModelStochastName,
                       calculation.Name,
                       calculation.InputParameters,
                       readCalculation.ConstructiveStrengthLinearLoadModel,
                       i => i.ConstructiveStrengthLinearLoadModel, (i, d) => i.ConstructiveStrengthLinearLoadModel = d)
                   && TryReadVariationCoefficientStochast(
                       StabilityPointStructuresConfigurationSchemaIdentifiers.ConstructiveStrengthQuadraticLoadModelStochastName,
                       calculation.Name,
                       calculation.InputParameters,
                       readCalculation.ConstructiveStrengthQuadraticLoadModel,
                       i => i.ConstructiveStrengthQuadraticLoadModel, (i, d) => i.ConstructiveStrengthQuadraticLoadModel = d)
                   && TryReadVariationCoefficientStochast(
                       StabilityPointStructuresConfigurationSchemaIdentifiers.ShipMassStochastName,
                       calculation.Name,
                       calculation.InputParameters,
                       readCalculation.ShipMass,
                       i => i.ShipMass, (i, d) => i.ShipMass = d)
                   && TryReadVariationCoefficientStochast(
                       StabilityPointStructuresConfigurationSchemaIdentifiers.ShipVelocityStochastName,
                       calculation.Name,
                       calculation.InputParameters,
                       readCalculation.ShipVelocity,
                       i => i.ShipVelocity, (i, d) => i.ShipVelocity = d)
                   && TryReadVariationCoefficientStochast(
                       StabilityPointStructuresConfigurationSchemaIdentifiers.StabilityLinearLoadModelStochastName,
                       calculation.Name,
                       calculation.InputParameters,
                       readCalculation.StabilityLinearLoadModel,
                       i => i.StabilityLinearLoadModel, (i, d) => i.StabilityLinearLoadModel = d)
                   && TryReadVariationCoefficientStochast(
                       StabilityPointStructuresConfigurationSchemaIdentifiers.StabilityQuadraticLoadModelStochastName,
                       calculation.Name,
                       calculation.InputParameters,
                       readCalculation.StabilityQuadraticLoadModel,
                       i => i.StabilityQuadraticLoadModel, (i, d) => i.StabilityQuadraticLoadModel = d)
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
                       i => i.StormDuration, (i, d) => i.StormDuration = d)
                   && TryReadStandardDeviationStochast(
                       ConfigurationSchemaIdentifiers.WidthFlowAperturesStochastName,
                       calculation.Name,
                       calculation.InputParameters,
                       readCalculation.WidthFlowApertures, i => i.WidthFlowApertures, (i, d) => i.WidthFlowApertures = d)
                   && TryReadStandardDeviationStochast(
                       StabilityPointStructuresConfigurationSchemaIdentifiers.ThresholdHeightOpenWeirStochastName,
                       calculation.Name,
                       calculation.InputParameters,
                       readCalculation.ThresholdHeightOpenWeir,
                       i => i.ThresholdHeightOpenWeir, (i, d) => i.ThresholdHeightOpenWeir = d);
        }

        private bool ValidateStochasts(StabilityPointStructuresCalculationConfiguration configuration)
        {
            if (configuration.DrainCoefficient?.StandardDeviation != null
                || configuration.DrainCoefficient?.VariationCoefficient != null)
            {
                Log.LogCalculationConversionError(RingtoetsCommonIOResources.CalculationConfigurationImporter_ValidateStochasts_Cannot_define_spread_for_DrainCoefficient,
                                                  configuration.Name);
                return false;
            }
            if (configuration.FlowVelocityStructureClosable?.StandardDeviation != null
                || configuration.FlowVelocityStructureClosable?.VariationCoefficient != null)
            {
                Log.LogCalculationConversionError(Resources.CalculationConfigurationImporter_ValidateStochasts_Cannot_define_spread_for_FlowVelocityStructureClosable,
                                                  configuration.Name);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Reads the structure normal orientation.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the orientation is invalid or when there is an orientation but
        /// no structure defined, <c>true</c> otherwise.</returns>
        private bool TryReadOrientation(StructuresCalculationConfiguration readCalculation, StructuresCalculation<StabilityPointStructuresInput> calculation)
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

        private bool TryReadFailureProbabilityRepairClosure(StabilityPointStructuresCalculationConfiguration readCalculation,
                                                            StructuresCalculation<StabilityPointStructuresInput> calculation)
        {
            if (!readCalculation.FailureProbabilityRepairClosure.HasValue)
            {
                return true;
            }

            if (calculation.InputParameters.Structure == null)
            {
                Log.LogCalculationConversionError(string.Format(RingtoetsCommonIOResources.CalculationConfigurationImporter_TryParameter_No_Structure_to_assign_Parameter_0_,
                                                                Resources.CalculationConfigurationImporter_FailureProbabilityRepairClosure_DisplayName),
                                                  calculation.Name);

                return false;
            }

            double failureProbabilityRepairClosure = readCalculation.FailureProbabilityRepairClosure.Value;

            try
            {
                calculation.InputParameters.FailureProbabilityRepairClosure = (RoundedDouble) failureProbabilityRepairClosure;
            }
            catch (ArgumentOutOfRangeException e)
            {
                Log.LogOutOfRangeException(string.Format(RingtoetsCommonIOResources.TryReadParameter_Value_0_ParameterName_1_is_invalid,
                                                         failureProbabilityRepairClosure,
                                                         Resources.CalculationConfigurationImporter_FailureProbabilityRepairClosure_DisplayName),
                                           calculation.Name,
                                           e);

                return false;
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
        private bool TryReadFailureProbabilityStructureWithErosion(StructuresCalculationConfiguration readCalculation,
                                                                   StructuresCalculation<StabilityPointStructuresInput> calculation)
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
        /// Reads the probability collision secondary structure.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the probability collision secondary structure is invalid or 
        /// when there is a probability collision secondary structure but no structure defined, 
        /// <c>true</c> otherwise.</returns>
        private bool TryReadProbabilityCollisionSecondaryStructure(StabilityPointStructuresCalculationConfiguration readCalculation,
                                                                   StructuresCalculation<StabilityPointStructuresInput> calculation)
        {
            if (readCalculation.ProbabilityCollisionSecondaryStructure.HasValue)
            {
                if (calculation.InputParameters.Structure == null)
                {
                    Log.LogCalculationConversionError(string.Format(RingtoetsCommonIOResources.CalculationConfigurationImporter_TryParameter_No_Structure_to_assign_Parameter_0_,
                                                                    Resources.CalculationConfigurationImporter_ProbabilityCollisionSecondaryStructure_DisplayName),
                                                      calculation.Name);

                    return false;
                }

                double failureProbability = readCalculation.ProbabilityCollisionSecondaryStructure.Value;

                try
                {
                    calculation.InputParameters.ProbabilityCollisionSecondaryStructure = failureProbability;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Log.LogOutOfRangeException(string.Format(
                                                   RingtoetsCommonIOResources.TryReadParameter_Value_0_ParameterName_1_is_invalid,
                                                   failureProbability,
                                                   Resources.CalculationConfigurationImporter_ProbabilityCollisionSecondaryStructure_DisplayName),
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
        private void ReadFactorStormDurationOpenStructure(StabilityPointStructuresCalculationConfiguration readCalculation,
                                                          StructuresCalculation<StabilityPointStructuresInput> calculation)
        {
            if (readCalculation.FactorStormDurationOpenStructure.HasValue)
            {
                calculation.InputParameters.FactorStormDurationOpenStructure = (RoundedDouble) readCalculation.FactorStormDurationOpenStructure.Value;
            }
        }
        /// <summary>
        /// Reads the volumic weight water.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        private void ReadVolumicWeightWater(StabilityPointStructuresCalculationConfiguration readCalculation,
                                                          StructuresCalculation<StabilityPointStructuresInput> calculation)
        {
            if (readCalculation.VolumicWeightWater.HasValue)
            {
                calculation.InputParameters.VolumicWeightWater = (RoundedDouble) readCalculation.VolumicWeightWater.Value;
            }
        }

        /// <summary>
        /// Reads the inflow model type.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the inflow model type is invalid or when there is a 
        /// inflow model type but no structure defined, <c>true</c> otherwise.</returns>
        private bool TryReadInflowModelType(StabilityPointStructuresCalculationConfiguration readCalculation,
                                            StructuresCalculation<StabilityPointStructuresInput> calculation)
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

                calculation.InputParameters.InflowModelType = (StabilityPointStructureInflowModelType)
                    new ConfigurationStabilityPointStructuresInflowModelTypeConverter()
                        .ConvertTo(readCalculation.InflowModelType.Value, typeof(StabilityPointStructureInflowModelType));
            }

            return true;
        }

        /// <summary>
        /// Reads the load schematization type.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the load schematization type is invalid or when there is a 
        /// load schematization type but no structure defined, <c>true</c> otherwise.</returns>
        private bool TryReadLoadSchematizationType(StabilityPointStructuresCalculationConfiguration readCalculation,
                                                   StructuresCalculation<StabilityPointStructuresInput> calculation)
        {
            if (readCalculation.LoadSchematizationType.HasValue)
            {
                if (calculation.InputParameters.Structure == null)
                {
                    Log.LogCalculationConversionError(string.Format(RingtoetsCommonIOResources.CalculationConfigurationImporter_TryParameter_No_Structure_to_assign_Parameter_0_,
                                                                    Resources.CalculationConfigurationImporter_LoadSchematizationType_DisplayName),
                                                      calculation.Name);

                    return false;
                }

                calculation.InputParameters.LoadSchematizationType = (LoadSchematizationType)
                    new ConfigurationStabilityPointStructuresLoadSchematizationTypeConverter()
                        .ConvertTo(readCalculation.LoadSchematizationType.Value, typeof(LoadSchematizationType));
            }

            return true;
        }

        /// <summary>
        /// Reads the leveling count.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the leveling count is invalid or when there is a 
        /// leveling count but no structure defined, <c>true</c> otherwise.</returns>
        private bool TryReadLevellingCount(StabilityPointStructuresCalculationConfiguration readCalculation,
                                           StructuresCalculation<StabilityPointStructuresInput> calculation)
        {
            if (readCalculation.LevellingCount.HasValue)
            {
                if (calculation.InputParameters.Structure == null)
                {
                    Log.LogCalculationConversionError(string.Format(RingtoetsCommonIOResources.CalculationConfigurationImporter_TryParameter_No_Structure_to_assign_Parameter_0_,
                                                                    Resources.CalculationConfigurationImporter_LevellingCount_DisplayName),
                                                      calculation.Name);

                    return false;
                }

                calculation.InputParameters.LevellingCount = readCalculation.LevellingCount.Value;
            }

            return true;
        }

        /// <summary>
        /// Reads the vertical distance.
        /// </summary>
        /// <param name="readCalculation">The calculation read from the imported file.</param>
        /// <param name="calculation">The calculation to configure.</param>
        /// <returns><c>false</c> when the vertical distance is invalid or when there is a 
        /// vertical distance but no structure defined, <c>true</c> otherwise.</returns>
        private bool TryReadVerticalDistance(StabilityPointStructuresCalculationConfiguration readCalculation,
                                             StructuresCalculation<StabilityPointStructuresInput> calculation)
        {
            if (readCalculation.VerticalDistance.HasValue)
            {
                if (calculation.InputParameters.Structure == null)
                {
                    Log.LogCalculationConversionError(string.Format(RingtoetsCommonIOResources.CalculationConfigurationImporter_TryParameter_No_Structure_to_assign_Parameter_0_,
                                                                    Resources.CalculationConfigurationImporter_VerticalDistance_DisplayName),
                                                      calculation.Name);

                    return false;
                }

                calculation.InputParameters.VerticalDistance = (RoundedDouble) readCalculation.VerticalDistance.Value;
            }

            return true;
        }

        private bool TrySetHydraulicBoundaryLocation(string locationName, StructuresCalculation<StabilityPointStructuresInput> calculation)
        {
            HydraulicBoundaryLocation location;

            if (TryReadHydraulicBoundaryLocation(locationName, calculation.Name, availableHydraulicBoundaryLocations, out location))
            {
                calculation.InputParameters.HydraulicBoundaryLocation = location;
                return true;
            }

            return false;
        }

        private bool TrySetStructure(string structureName, StructuresCalculation<StabilityPointStructuresInput> calculation)
        {
            StabilityPointStructure structure;

            if (TryReadStructure(structureName, calculation.Name, availableStructures, out structure))
            {
                calculation.InputParameters.Structure = structure;
                return true;
            }

            return false;
        }

        private bool TrySetForeshoreProfile(string foreshoreProfileName, StructuresCalculation<StabilityPointStructuresInput> calculation)
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