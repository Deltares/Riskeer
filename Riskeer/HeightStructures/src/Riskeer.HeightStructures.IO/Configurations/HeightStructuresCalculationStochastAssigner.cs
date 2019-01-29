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
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.IO.Configurations;
using Riskeer.Common.IO.Configurations.Helpers;
using Riskeer.HeightStructures.Data;
using RiskeerCommonIOResources = Riskeer.Common.IO.Properties.Resources;

namespace Riskeer.HeightStructures.IO.Configurations
{
    /// <summary>
    /// Validates and assigns stochast configurations to a height structure calculation input.
    /// </summary>
    public class HeightStructuresCalculationStochastAssigner
        : StructuresCalculationStochastAssigner<HeightStructuresCalculationConfiguration, HeightStructuresInput, HeightStructure>
    {
        /// <summary>
        /// Creates a new instance of <see cref="HeightStructuresCalculationStochastAssigner"/>
        /// </summary>
        /// <param name="configuration">The configuration that is used for stochast parameter source.</param>
        /// <param name="calculation">The target calculation.</param>
        public HeightStructuresCalculationStochastAssigner(
            HeightStructuresCalculationConfiguration configuration,
            StructuresCalculation<HeightStructuresInput> calculation)
            : base(configuration, calculation) {}

        protected override IEnumerable<StandardDeviationDefinition> GetStandardDeviationStochasts(bool structureDependent = false)
        {
            yield return new StandardDeviationDefinition(
                HeightStructuresConfigurationSchemaIdentifiers.LevelCrestStructureStochastName,
                Configuration.LevelCrestStructure,
                i => i.LevelCrestStructure,
                (i, d) => i.LevelCrestStructure = (NormalDistribution) d);

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
                    RiskeerCommonIOResources.CalculationConfigurationImporter_ValidateStochasts_Cannot_define_spread_for_ModelFactorSuperCriticalFlow,
                    Configuration.Name);
                return false;
            }

            return true;
        }
    }
}