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

using System.Collections.Generic;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.Configurations.Helpers;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.ClosingStructures.IO.Configurations
{
    /// <summary>
    /// Validates and assigns stochast configurations to a Closing structure calculation input.
    /// </summary>
    public class ClosingStructuresCalculationStochastAssigner
        : StructuresCalculationStochastAssigner<ClosingStructuresCalculationConfiguration, ClosingStructuresInput, ClosingStructure>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ClosingStructuresCalculationStochastAssigner"/>
        /// </summary>
        /// <param name="configuration">The configuration that is used for stochast parameter source.</param>
        /// <param name="calculation">The target calculation.</param>
        public ClosingStructuresCalculationStochastAssigner(
            ClosingStructuresCalculationConfiguration configuration,
            StructuresCalculation<ClosingStructuresInput> calculation)
            : base(configuration, calculation) {}

        protected override IEnumerable<StandardDeviationDefinition> GetStandardDeviationStochasts(bool structureDependent = false)
        {
            yield return new StandardDeviationDefinition(
                ClosingStructuresConfigurationSchemaIdentifiers.LevelCrestStructureNotClosingStochastName,
                Configuration.LevelCrestStructureNotClosing,
                i => i.LevelCrestStructureNotClosing,
                (i, d) => i.LevelCrestStructureNotClosing = (NormalDistribution) d);

            yield return new StandardDeviationDefinition(
                ClosingStructuresConfigurationSchemaIdentifiers.AreaFlowAperturesStochastName,
                Configuration.AreaFlowApertures,
                i => i.AreaFlowApertures,
                (i, d) => i.AreaFlowApertures = (LogNormalDistribution) d);

            yield return new StandardDeviationDefinition(
                ClosingStructuresConfigurationSchemaIdentifiers.InsideWaterLevelStochastName,
                Configuration.InsideWaterLevel,
                i => i.InsideWaterLevel,
                (i, d) => i.InsideWaterLevel = (NormalDistribution) d);

            yield return new StandardDeviationDefinition(
                ClosingStructuresConfigurationSchemaIdentifiers.ThresholdHeightOpenWeirStochastName,
                Configuration.ThresholdHeightOpenWeir,
                i => i.ThresholdHeightOpenWeir,
                (i, d) => i.ThresholdHeightOpenWeir = (NormalDistribution) d);

            yield return new StandardDeviationDefinition(
                ConfigurationSchemaIdentifiers.AllowedLevelIncreaseStorageStochastName,
                Configuration.AllowedLevelIncreaseStorage,
                i => i.AllowedLevelIncreaseStorage,
                (i, d) => i.AllowedLevelIncreaseStorage = (LogNormalDistribution) d);

            yield return new StandardDeviationDefinition(
                ConfigurationSchemaIdentifiers.FlowWidthAtBottomProtectionStochastName,
                Configuration.FlowWidthAtBottomProtection,
                i => i.FlowWidthAtBottomProtection,
                (i, d) => i.FlowWidthAtBottomProtection = (LogNormalDistribution) d);

            yield return new StandardDeviationDefinition(
                ConfigurationSchemaIdentifiers.WidthFlowAperturesStochastName,
                Configuration.WidthFlowApertures,
                i => i.WidthFlowApertures,
                (i, d) => i.WidthFlowApertures = (NormalDistribution) d);

            if (!structureDependent)
            {
                yield return new StandardDeviationDefinition(
                    ClosingStructuresConfigurationSchemaIdentifiers.DrainCoefficientStochastName,
                    Configuration.DrainCoefficient,
                    i => i.DrainCoefficient,
                    (i, d) => i.DrainCoefficient = (NormalDistribution) d);

                yield return new StandardDeviationDefinition(
                    ConfigurationSchemaIdentifiers.ModelFactorSuperCriticalFlowStochastName,
                    Configuration.ModelFactorSuperCriticalFlow,
                    i => i.ModelFactorSuperCriticalFlow,
                    (i, d) => i.ModelFactorSuperCriticalFlow = (NormalDistribution) d);
            }
        }

        protected override IEnumerable<VariationCoefficientDefinition> GetVariationCoefficientStochasts(bool structureDependent = false)
        {
            yield return new VariationCoefficientDefinition(
                ConfigurationSchemaIdentifiers.CriticalOvertoppingDischargeStochastName,
                Configuration.CriticalOvertoppingDischarge,
                i => i.CriticalOvertoppingDischarge,
                (i, d) => i.CriticalOvertoppingDischarge = (VariationCoefficientLogNormalDistribution) d);

            yield return new VariationCoefficientDefinition(
                ConfigurationSchemaIdentifiers.StorageStructureAreaStochastName,
                Configuration.StorageStructureArea,
                i => i.StorageStructureArea,
                (i, d) => i.StorageStructureArea = (VariationCoefficientLogNormalDistribution) d);

            if (!structureDependent)
            {
                yield return new VariationCoefficientDefinition(
                    ConfigurationSchemaIdentifiers.StormDurationStochastName,
                    Configuration.StormDuration,
                    i => i.StormDuration,
                    (i, d) => i.StormDuration = (VariationCoefficientLogNormalDistribution) d);
            }
        }

        protected override bool ValidateSpecificStochasts()
        {
            if (Configuration.ModelFactorSuperCriticalFlow?.StandardDeviation != null
                || Configuration.ModelFactorSuperCriticalFlow?.VariationCoefficient != null)
            {
                Log.LogCalculationConversionError(
                    RingtoetsCommonIOResources.CalculationConfigurationImporter_ValidateStochasts_Cannot_define_spread_for_ModelFactorSuperCriticalFlow,
                    Configuration.Name);
                return false;
            }

            if (Configuration.DrainCoefficient?.StandardDeviation != null
                || Configuration.DrainCoefficient?.VariationCoefficient != null)
            {
                Log.LogCalculationConversionError(
                    RingtoetsCommonIOResources.StructuresCalculationStochastAssigner_ValidateStochasts_Cannot_define_spread_for_DrainCoefficient,
                    Configuration.Name);
                return false;
            }

            return true;
        }
    }
}