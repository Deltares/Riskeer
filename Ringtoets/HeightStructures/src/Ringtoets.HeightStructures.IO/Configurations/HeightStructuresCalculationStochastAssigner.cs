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

using System.Collections.Generic;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.HeightStructures.Data;

namespace Ringtoets.HeightStructures.IO.Configurations
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
        /// <param name="setStandardDeviationStochast">The delegate for setting a stochast with standard deviation.</param>
        /// <param name="setVariationCoefficientStochast">The delegate for setting a stochast with variation coefficient.</param>
        public HeightStructuresCalculationStochastAssigner(
            HeightStructuresCalculationConfiguration configuration,
            StructuresCalculation<HeightStructuresInput> calculation,
            TrySetStandardDeviationStochast setStandardDeviationStochast, 
            TrySetVariationCoefficientStochast setVariationCoefficientStochast)
            : base(configuration, calculation, setStandardDeviationStochast, setVariationCoefficientStochast) {}

        protected override IEnumerable<StandardDeviationDefinition> GetStandardDeviationStochasts(bool onlyStructureDependent = false)
        {
            yield return StandardDeviationDefinition.Create(Configuration.LevelCrestStructure,
                HeightStructuresConfigurationSchemaIdentifiers.LevelCrestStructureStochastName,
                i => i.LevelCrestStructure, (i, d) => i.LevelCrestStructure = d as NormalDistribution);

            yield return StandardDeviationDefinition.Create(Configuration.AllowedLevelIncreaseStorage,
                ConfigurationSchemaIdentifiers.AllowedLevelIncreaseStorageStochastName,
                i => i.AllowedLevelIncreaseStorage, (i, d) => i.AllowedLevelIncreaseStorage = d as LogNormalDistribution);

            yield return StandardDeviationDefinition.Create(Configuration.FlowWidthAtBottomProtection,
                ConfigurationSchemaIdentifiers.FlowWidthAtBottomProtectionStochastName,
                i => i.FlowWidthAtBottomProtection, (i, d) => i.FlowWidthAtBottomProtection = d as LogNormalDistribution);

            yield return StandardDeviationDefinition.Create(Configuration.WidthFlowApertures,
                ConfigurationSchemaIdentifiers.WidthFlowAperturesStochastName,
                i => i.WidthFlowApertures, (i, d) => i.WidthFlowApertures = d as NormalDistribution);

            if (!onlyStructureDependent)
            {
                yield return StandardDeviationDefinition.Create(Configuration.ModelFactorSuperCriticalFlow,
                    ConfigurationSchemaIdentifiers.ModelFactorSuperCriticalFlowStochastName,
                    i => i.ModelFactorSuperCriticalFlow, (i, d) => i.ModelFactorSuperCriticalFlow = d as NormalDistribution);
            }
        }

        protected override IEnumerable<VariationCoefficientDefinition> GetVariationCoefficientStochasts(bool onlyStructureDependent = false)
        {
            yield return VariationCoefficientDefinition.Create(Configuration.CriticalOvertoppingDischarge,
                ConfigurationSchemaIdentifiers.CriticalOvertoppingDischargeStochastName,
                i => i.CriticalOvertoppingDischarge, (i, d) => i.CriticalOvertoppingDischarge = d as VariationCoefficientLogNormalDistribution);

            yield return VariationCoefficientDefinition.Create(Configuration.StorageStructureArea,
                ConfigurationSchemaIdentifiers.StorageStructureAreaStochastName,
                i => i.StorageStructureArea, (i, d) => i.StorageStructureArea = d as VariationCoefficientLogNormalDistribution);

            if (!onlyStructureDependent)
            {
                yield return VariationCoefficientDefinition.Create(Configuration.StormDuration,
                    ConfigurationSchemaIdentifiers.StormDurationStochastName,
                    i => i.StormDuration, (i, d) => i.StormDuration = d as VariationCoefficientLogNormalDistribution);
            }
        }
    }
}