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
using Riskeer.Common.Service;
using Riskeer.Common.Service.ValidationRules;
using Riskeer.HeightStructures.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Riskeer.HeightStructures.Service
{
    /// <summary>
    /// The height structures validation rules registry.
    /// </summary>
    public class HeightStructuresValidationRulesRegistry : IStructuresValidationRulesRegistry<HeightStructuresInput, HeightStructure>
    {
        public IEnumerable<ValidationRule> GetValidationRules(HeightStructuresInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return new ValidationRule[]
            {
                new UseBreakWaterRule(input),
                new VariationCoefficientLogNormalDistributionRule(
                    input.StormDuration,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_StormDuration_DisplayName)),
                new NormalDistributionRule(
                    input.ModelFactorSuperCriticalFlow,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_ModelFactorSuperCriticalFlow_DisplayName)),
                new NumericInputRule(
                    input.StructureNormalOrientation,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Orientation_DisplayName)),
                new LogNormalDistributionRule(
                    input.FlowWidthAtBottomProtection,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_FlowWidthAtBottomProtection_DisplayName)),
                new NormalDistributionRule(
                    input.WidthFlowApertures,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_WidthFlowApertures_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(
                    input.StorageStructureArea,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_StorageStructureArea_DisplayName)),
                new LogNormalDistributionRule(
                    input.AllowedLevelIncreaseStorage,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_AllowedLevelIncreaseStorage_DisplayName)),
                new NormalDistributionRule(
                    input.LevelCrestStructure,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_LevelCrestStructure_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(
                    input.CriticalOvertoppingDischarge,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_CriticalOvertoppingDischarge_DisplayName))
            };
        }
    }
}