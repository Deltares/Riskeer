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
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.Configurations.Helpers;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.IO.Properties;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.StabilityPointStructures.IO.Configurations
{
    /// <summary>
    /// Validates and assigns stochast configurations to a stability point structure calculation input.
    /// </summary>
    public class StabilityPointStructuresCalculationStochastAssigner
        : StructuresCalculationStochastAssigner<StabilityPointStructuresCalculationConfiguration,
            StabilityPointStructuresInput, StabilityPointStructure>
    {
        /// <summary>
        /// Creates a new instance of <see cref="StabilityPointStructuresCalculationStochastAssigner"/>
        /// </summary>
        /// <param name="configuration">The configuration that is used for stochast parameter source.</param>
        /// <param name="calculation">The target calculation.</param>
        /// <param name="setStandardDeviationStochast">The delegate for setting a stochast with standard deviation.</param>
        /// <param name="setVariationCoefficientStochast">The delegate for setting a stochast with variation coefficient.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameters is <c>null</c>.</exception>
        public StabilityPointStructuresCalculationStochastAssigner(StabilityPointStructuresCalculationConfiguration configuration,
                                                                   StructuresCalculation<StabilityPointStructuresInput> calculation,
                                                                   TrySetStandardDeviationStochast setStandardDeviationStochast,
                                                                   TrySetVariationCoefficientStochast setVariationCoefficientStochast)
            : base(configuration, calculation, setStandardDeviationStochast, setVariationCoefficientStochast) { }

        protected override IEnumerable<StandardDeviationDefinition> GetStandardDeviationStochasts(bool structureDependent = false)
        {
            yield return StandardDeviationDefinition.Create(
                ConfigurationSchemaIdentifiers.AllowedLevelIncreaseStorageStochastName,
                Configuration.AllowedLevelIncreaseStorage,
                i => i.AllowedLevelIncreaseStorage,
                (i, d) => i.AllowedLevelIncreaseStorage = (LogNormalDistribution) d);

            yield return StandardDeviationDefinition.Create(
                StabilityPointStructuresConfigurationSchemaIdentifiers.AreaFlowAperturesStochastName,
                Configuration.AreaFlowApertures,
                i => i.AreaFlowApertures,
                (i, d) => i.AreaFlowApertures = (LogNormalDistribution) d);

            yield return StandardDeviationDefinition.Create(
                StabilityPointStructuresConfigurationSchemaIdentifiers.BankWidthStochastName,
                Configuration.BankWidth,
                i => i.BankWidth,
                (i, d) => i.BankWidth = (NormalDistribution) d);

            yield return StandardDeviationDefinition.Create(
                ConfigurationSchemaIdentifiers.FlowWidthAtBottomProtectionStochastName,
                Configuration.FlowWidthAtBottomProtection,
                i => i.FlowWidthAtBottomProtection,
                (i, d) => i.FlowWidthAtBottomProtection = (LogNormalDistribution) d);

            yield return StandardDeviationDefinition.Create(
                StabilityPointStructuresConfigurationSchemaIdentifiers.InsideWaterLevelStochastName,
                Configuration.InsideWaterLevel,
                i => i.InsideWaterLevel,
                (i, d) => i.InsideWaterLevel = (NormalDistribution) d);

            yield return StandardDeviationDefinition.Create(
                StabilityPointStructuresConfigurationSchemaIdentifiers.InsideWaterLevelFailureConstructionStochastName,
                Configuration.InsideWaterLevelFailureConstruction,
                i => i.InsideWaterLevelFailureConstruction,
                (i, d) => i.InsideWaterLevelFailureConstruction = (NormalDistribution) d);

            yield return StandardDeviationDefinition.Create(
                StabilityPointStructuresConfigurationSchemaIdentifiers.LevelCrestStructureStochastName,
                Configuration.LevelCrestStructure,
                i => i.LevelCrestStructure,
                (i, d) => i.LevelCrestStructure = (NormalDistribution) d);

            yield return StandardDeviationDefinition.Create(
                ConfigurationSchemaIdentifiers.WidthFlowAperturesStochastName,
                Configuration.WidthFlowApertures,
                i => i.WidthFlowApertures,
                (i, d) => i.WidthFlowApertures = (NormalDistribution) d);

            yield return StandardDeviationDefinition.Create(
                StabilityPointStructuresConfigurationSchemaIdentifiers.ThresholdHeightOpenWeirStochastName,
                Configuration.ThresholdHeightOpenWeir,
                i => i.ThresholdHeightOpenWeir,
                (i, d) => i.ThresholdHeightOpenWeir = (NormalDistribution) d);

            if (!structureDependent)
            {
                yield return StandardDeviationDefinition.Create(
                    StabilityPointStructuresConfigurationSchemaIdentifiers.DrainCoefficientStochastName,
                    Configuration.DrainCoefficient,
                    i => i.DrainCoefficient,
                    (i, d) => i.DrainCoefficient = (NormalDistribution) d);

                yield return StandardDeviationDefinition.Create(
                    ConfigurationSchemaIdentifiers.ModelFactorSuperCriticalFlowStochastName,
                    Configuration.ModelFactorSuperCriticalFlow,
                    i => i.ModelFactorSuperCriticalFlow,
                    (i, d) => i.ModelFactorSuperCriticalFlow = (NormalDistribution) d);
            }
        }

        protected override IEnumerable<VariationCoefficientDefinition> GetVariationCoefficientStochasts(bool structureDependent = false)
        {
            yield return VariationCoefficientDefinition.Create(
                StabilityPointStructuresConfigurationSchemaIdentifiers.FlowVelocityStructureClosableStochastName,
                Configuration.FlowVelocityStructureClosable,
                i => i.FlowVelocityStructureClosable,
                (i, d) => i.FlowVelocityStructureClosable = (VariationCoefficientNormalDistribution) d);

            yield return VariationCoefficientDefinition.Create(
                ConfigurationSchemaIdentifiers.CriticalOvertoppingDischargeStochastName,
                Configuration.CriticalOvertoppingDischarge,
                i => i.CriticalOvertoppingDischarge,
                (i, d) => i.CriticalOvertoppingDischarge = (VariationCoefficientLogNormalDistribution) d);

            yield return VariationCoefficientDefinition.Create(
                StabilityPointStructuresConfigurationSchemaIdentifiers.ConstructiveStrengthLinearLoadModelStochastName,
                Configuration.ConstructiveStrengthLinearLoadModel,
                i => i.ConstructiveStrengthLinearLoadModel,
                (i, d) => i.ConstructiveStrengthLinearLoadModel = (VariationCoefficientLogNormalDistribution) d);

            yield return VariationCoefficientDefinition.Create(
                StabilityPointStructuresConfigurationSchemaIdentifiers.ConstructiveStrengthQuadraticLoadModelStochastName,
                Configuration.ConstructiveStrengthQuadraticLoadModel,
                i => i.ConstructiveStrengthQuadraticLoadModel,
                (i, d) => i.ConstructiveStrengthQuadraticLoadModel = (VariationCoefficientLogNormalDistribution) d);

            yield return VariationCoefficientDefinition.Create(
                StabilityPointStructuresConfigurationSchemaIdentifiers.FailureCollisionEnergyStochastName,
                Configuration.FailureCollisionEnergy,
                i => i.FailureCollisionEnergy,
                (i, d) => i.FailureCollisionEnergy = (VariationCoefficientLogNormalDistribution) d);

            yield return VariationCoefficientDefinition.Create(
                StabilityPointStructuresConfigurationSchemaIdentifiers.ShipMassStochastName,
                Configuration.ShipMass,
                i => i.ShipMass,
                (i, d) => i.ShipMass = (VariationCoefficientNormalDistribution) d);

            yield return VariationCoefficientDefinition.Create(
                StabilityPointStructuresConfigurationSchemaIdentifiers.ShipVelocityStochastName,
                Configuration.ShipVelocity,
                i => i.ShipVelocity,
                (i, d) => i.ShipVelocity = (VariationCoefficientNormalDistribution) d);

            yield return VariationCoefficientDefinition.Create(
                StabilityPointStructuresConfigurationSchemaIdentifiers.StabilityLinearLoadModelStochastName,
                Configuration.StabilityLinearLoadModel,
                i => i.StabilityLinearLoadModel,
                (i, d) => i.StabilityLinearLoadModel = (VariationCoefficientLogNormalDistribution) d);

            yield return VariationCoefficientDefinition.Create(
                StabilityPointStructuresConfigurationSchemaIdentifiers.StabilityQuadraticLoadModelStochastName,
                Configuration.StabilityQuadraticLoadModel,
                i => i.StabilityQuadraticLoadModel,
                (i, d) => i.StabilityQuadraticLoadModel = (VariationCoefficientLogNormalDistribution) d);

            yield return VariationCoefficientDefinition.Create(
                ConfigurationSchemaIdentifiers.StorageStructureAreaStochastName,
                Configuration.StorageStructureArea,
                i => i.StorageStructureArea,
                (i, d) => i.StorageStructureArea = (VariationCoefficientLogNormalDistribution) d);

            if (!structureDependent)
            {
                yield return VariationCoefficientDefinition.Create(
                    ConfigurationSchemaIdentifiers.StormDurationStochastName,
                    Configuration.StormDuration,
                    i => i.StormDuration,
                    (i, d) => i.StormDuration = (VariationCoefficientLogNormalDistribution) d);
            }
        }

        protected override bool ValidateSpecificStochasts()
        {
            if (Configuration.DrainCoefficient?.StandardDeviation != null
                || Configuration.DrainCoefficient?.VariationCoefficient != null)
            {
                Log.LogCalculationConversionError(RingtoetsCommonIOResources.StructuresCalculationStochastAssigner_ValidateStochasts_Cannot_define_spread_for_DrainCoefficient,
                                                  Configuration.Name);
                return false;
            }
            if (Configuration.FlowVelocityStructureClosable?.StandardDeviation != null
                || Configuration.FlowVelocityStructureClosable?.VariationCoefficient != null)
            {
                Log.LogCalculationConversionError(Resources.StabilityPointStructuresCalculationStochastAssigner_ValidateStochasts_Cannot_define_spread_for_FlowVelocityStructureClosable,
                                                  Configuration.Name);
                return false;
            }
            return base.ValidateSpecificStochasts();
        }
    }
}