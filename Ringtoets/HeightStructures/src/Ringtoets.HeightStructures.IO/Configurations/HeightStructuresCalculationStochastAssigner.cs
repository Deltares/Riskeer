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
    public class HeightStructuresCalculationStochastAssigner : StructuresCalculationStochastAssigner<HeightStructuresCalculationConfiguration>
    {
        public HeightStructuresCalculationStochastAssigner(
            HeightStructuresCalculationConfiguration configuration,
            StructuresCalculation<HeightStructuresInput> calculation,
            TrySetStandardDeviationStochast standardDeviationStochastSetter, 
            TrySetVariationCoefficientStochast variationCoefficientStochastSetter)
            : base(configuration, calculation, standardDeviationStochastSetter, variationCoefficientStochastSetter) {}

        protected override IEnumerable<StandardDeviationDefinition> GetStandardDeviationStochasts(bool onlyStructureDependent = false)
        {
            yield return StandardDeviationDefinition.Create(
                HeightStructuresConfigurationSchemaIdentifiers.LevelCrestStructureStochastName,
                configuration.LevelCrestStructure,
                i => i.LevelCrestStructure,
                (i, d) => i.LevelCrestStructure = d as NormalDistribution);

            yield return StandardDeviationDefinition.Create(
                ConfigurationSchemaIdentifiers.AllowedLevelIncreaseStorageStochastName,
                configuration.AllowedLevelIncreaseStorage,
                i => i.AllowedLevelIncreaseStorage,
                (i, d) => i.AllowedLevelIncreaseStorage = d as LogNormalDistribution);

            yield return StandardDeviationDefinition.Create(
                ConfigurationSchemaIdentifiers.FlowWidthAtBottomProtectionStochastName,
                configuration.FlowWidthAtBottomProtection,
                i => i.FlowWidthAtBottomProtection,
                (i, d) => i.FlowWidthAtBottomProtection = d as LogNormalDistribution);

            yield return StandardDeviationDefinition.Create(
                ConfigurationSchemaIdentifiers.WidthFlowAperturesStochastName,
                configuration.WidthFlowApertures,
                i => i.WidthFlowApertures,
                (i, d) => i.WidthFlowApertures = d as NormalDistribution);

            if (!onlyStructureDependent)
            {
                yield return StandardDeviationDefinition.Create(
                    ConfigurationSchemaIdentifiers.ModelFactorSuperCriticalFlowStochastName,
                    configuration.ModelFactorSuperCriticalFlow,
                    i => i.ModelFactorSuperCriticalFlow,
                    (i, d) => i.ModelFactorSuperCriticalFlow = d as NormalDistribution);
            }
        }

        protected override IEnumerable<VariationCoefficientDefinition> GetVariationCoefficientStochasts(bool onlyStructureDependent = false)
        {
            yield return VariationCoefficientDefinition.Create(
                ConfigurationSchemaIdentifiers.CriticalOvertoppingDischargeStochastName,
                configuration.CriticalOvertoppingDischarge,
                i => i.CriticalOvertoppingDischarge,
                (i, d) => i.CriticalOvertoppingDischarge = d as VariationCoefficientLogNormalDistribution);

            yield return VariationCoefficientDefinition.Create(
                ConfigurationSchemaIdentifiers.StorageStructureAreaStochastName,
                configuration.StorageStructureArea,
                i => i.StorageStructureArea,
                (i, d) => i.StorageStructureArea = d as VariationCoefficientLogNormalDistribution);

            if (!onlyStructureDependent)
            {
                yield return VariationCoefficientDefinition.Create(
                    ConfigurationSchemaIdentifiers.StormDurationStochastName,
                    configuration.StormDuration,
                    i => i.StormDuration,
                    (i, d) => i.StormDuration = d as VariationCoefficientLogNormalDistribution);
            }
        }
    }
}