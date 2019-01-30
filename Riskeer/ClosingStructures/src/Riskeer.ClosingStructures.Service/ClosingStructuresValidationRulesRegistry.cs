// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.ClosingStructures.Data;
using Riskeer.Common.Service;
using Riskeer.Common.Service.ValidationRules;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;
using ClosingStructuresFormsResources = Riskeer.ClosingStructures.Forms.Properties.Resources;

namespace Riskeer.ClosingStructures.Service
{
    /// <summary>
    /// The closing structures validation rules registry.
    /// </summary>
    public class ClosingStructuresValidationRulesRegistry : IStructuresValidationRulesRegistry<ClosingStructuresInput, ClosingStructure>
    {
        public IEnumerable<ValidationRule> GetValidationRules(ClosingStructuresInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            ClosingStructureInflowModelType inflowModelType = input.InflowModelType;
            if (!Enum.IsDefined(typeof(ClosingStructureInflowModelType), inflowModelType))
            {
                throw new InvalidEnumArgumentException(nameof(input),
                                                       (int) inflowModelType,
                                                       typeof(ClosingStructureInflowModelType));
            }

            IEnumerable<ValidationRule> validationRules;
            switch (inflowModelType)
            {
                case ClosingStructureInflowModelType.VerticalWall:
                    validationRules = GetVerticalWallValidationRules(input);
                    break;
                case ClosingStructureInflowModelType.LowSill:
                    validationRules = GetLowSillValidationRules(input);
                    break;
                case ClosingStructureInflowModelType.FloodedCulvert:
                    validationRules = GetFloodedCulvertValidationRules(input);
                    break;
                default:
                    throw new NotSupportedException();
            }

            return validationRules;
        }

        private static IEnumerable<ValidationRule> GetVerticalWallValidationRules(ClosingStructuresInput input)
        {
            return new ValidationRule[]
            {
                new UseBreakWaterRule(input),
                new VariationCoefficientLogNormalDistributionRule(
                    input.StormDuration,
                    ParameterNameExtractor.GetFromDisplayName(RiskeerCommonFormsResources.Structure_StormDuration_DisplayName)),
                new NormalDistributionRule(
                    input.ModelFactorSuperCriticalFlow,
                    ParameterNameExtractor.GetFromDisplayName(RiskeerCommonFormsResources.Structure_ModelFactorSuperCriticalFlow_DisplayName)),
                new NumericInputRule(
                    input.FactorStormDurationOpenStructure,
                    ParameterNameExtractor.GetFromDisplayName(RiskeerCommonFormsResources.Structure_FactorStormDurationOpenStructure_DisplayName)),
                new NormalDistributionRule(
                    input.WidthFlowApertures,
                    ParameterNameExtractor.GetFromDisplayName(RiskeerCommonFormsResources.Structure_WidthFlowApertures_DisplayName)),
                new NumericInputRule(
                    input.StructureNormalOrientation,
                    ParameterNameExtractor.GetFromDisplayName(RiskeerCommonFormsResources.Structure_StructureNormalOrientation_DisplayName)),
                new LogNormalDistributionRule(
                    input.FlowWidthAtBottomProtection,
                    ParameterNameExtractor.GetFromDisplayName(RiskeerCommonFormsResources.Structure_FlowWidthAtBottomProtection_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(
                    input.StorageStructureArea,
                    ParameterNameExtractor.GetFromDisplayName(RiskeerCommonFormsResources.Structure_StorageStructureArea_DisplayName)),
                new LogNormalDistributionRule(
                    input.AllowedLevelIncreaseStorage,
                    ParameterNameExtractor.GetFromDisplayName(RiskeerCommonFormsResources.Structure_AllowedLevelIncreaseStorage_DisplayName)),
                new NormalDistributionRule(
                    input.LevelCrestStructureNotClosing,
                    ParameterNameExtractor.GetFromDisplayName(ClosingStructuresFormsResources.LevelCrestStructureNotClosing_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(
                    input.CriticalOvertoppingDischarge,
                    ParameterNameExtractor.GetFromDisplayName(RiskeerCommonFormsResources.Structure_CriticalOvertoppingDischarge_DisplayName))
            };
        }

        private static IEnumerable<ValidationRule> GetLowSillValidationRules(ClosingStructuresInput input)
        {
            return new ValidationRule[]
            {
                new UseBreakWaterRule(input),
                new VariationCoefficientLogNormalDistributionRule(
                    input.StormDuration,
                    ParameterNameExtractor.GetFromDisplayName(RiskeerCommonFormsResources.Structure_StormDuration_DisplayName)),
                new NormalDistributionRule(
                    input.InsideWaterLevel,
                    ParameterNameExtractor.GetFromDisplayName(RiskeerCommonFormsResources.Structure_InsideWaterLevel_DisplayName)),
                new NormalDistributionRule(
                    input.ModelFactorSuperCriticalFlow,
                    ParameterNameExtractor.GetFromDisplayName(RiskeerCommonFormsResources.Structure_ModelFactorSuperCriticalFlow_DisplayName)),
                new NumericInputRule(
                    input.FactorStormDurationOpenStructure,
                    ParameterNameExtractor.GetFromDisplayName(RiskeerCommonFormsResources.Structure_FactorStormDurationOpenStructure_DisplayName)),
                new NormalDistributionRule(
                    input.WidthFlowApertures,
                    ParameterNameExtractor.GetFromDisplayName(RiskeerCommonFormsResources.Structure_WidthFlowApertures_DisplayName)),
                new LogNormalDistributionRule(
                    input.FlowWidthAtBottomProtection,
                    ParameterNameExtractor.GetFromDisplayName(RiskeerCommonFormsResources.Structure_FlowWidthAtBottomProtection_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(
                    input.StorageStructureArea,
                    ParameterNameExtractor.GetFromDisplayName(RiskeerCommonFormsResources.Structure_StorageStructureArea_DisplayName)),
                new LogNormalDistributionRule(
                    input.AllowedLevelIncreaseStorage,
                    ParameterNameExtractor.GetFromDisplayName(RiskeerCommonFormsResources.Structure_AllowedLevelIncreaseStorage_DisplayName)),
                new NormalDistributionRule(
                    input.ThresholdHeightOpenWeir,
                    ParameterNameExtractor.GetFromDisplayName(RiskeerCommonFormsResources.Structure_ThresholdHeightOpenWeir_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(
                    input.CriticalOvertoppingDischarge,
                    ParameterNameExtractor.GetFromDisplayName(RiskeerCommonFormsResources.Structure_CriticalOvertoppingDischarge_DisplayName))
            };
        }

        private static IEnumerable<ValidationRule> GetFloodedCulvertValidationRules(ClosingStructuresInput input)
        {
            return new ValidationRule[]
            {
                new UseBreakWaterRule(input),
                new VariationCoefficientLogNormalDistributionRule(
                    input.StormDuration,
                    ParameterNameExtractor.GetFromDisplayName(RiskeerCommonFormsResources.Structure_StormDuration_DisplayName)),
                new NormalDistributionRule(
                    input.InsideWaterLevel,
                    ParameterNameExtractor.GetFromDisplayName(RiskeerCommonFormsResources.Structure_InsideWaterLevel_DisplayName)),
                new NormalDistributionRule(
                    input.DrainCoefficient,
                    ParameterNameExtractor.GetFromDisplayName(RiskeerCommonFormsResources.Structure_DrainCoefficient_DisplayName)),
                new NumericInputRule(
                    input.FactorStormDurationOpenStructure,
                    ParameterNameExtractor.GetFromDisplayName(RiskeerCommonFormsResources.Structure_FactorStormDurationOpenStructure_DisplayName)),
                new LogNormalDistributionRule(
                    input.AreaFlowApertures,
                    ParameterNameExtractor.GetFromDisplayName(RiskeerCommonFormsResources.Structure_AreaFlowApertures_DisplayName)),
                new LogNormalDistributionRule(
                    input.FlowWidthAtBottomProtection,
                    ParameterNameExtractor.GetFromDisplayName(RiskeerCommonFormsResources.Structure_FlowWidthAtBottomProtection_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(
                    input.StorageStructureArea,
                    ParameterNameExtractor.GetFromDisplayName(RiskeerCommonFormsResources.Structure_StorageStructureArea_DisplayName)),
                new LogNormalDistributionRule(
                    input.AllowedLevelIncreaseStorage,
                    ParameterNameExtractor.GetFromDisplayName(RiskeerCommonFormsResources.Structure_AllowedLevelIncreaseStorage_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(
                    input.CriticalOvertoppingDischarge,
                    ParameterNameExtractor.GetFromDisplayName(RiskeerCommonFormsResources.Structure_CriticalOvertoppingDischarge_DisplayName))
            };
        }
    }
}