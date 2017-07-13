// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Service;
using Ringtoets.Common.Service.ValidationRules;
using Ringtoets.StabilityPointStructures.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsStabilityPointStructuresFormsResources = Ringtoets.StabilityPointStructures.Forms.Properties.Resources;

namespace Ringtoets.StabilityPointStructures.Service
{
    /// <summary>
    /// The stability point structures validation rules registry.
    /// </summary>
    public class StabilityPointStructuresValidationRulesRegistry : IStructuresValidationRulesRegistry<StabilityPointStructuresInput, StabilityPointStructure>
    {
        public IEnumerable<ValidationRule> GetValidationRules(StabilityPointStructuresInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            IEnumerable<ValidationRule> validationRules;
            switch (input.InflowModelType)
            {
                case StabilityPointStructureInflowModelType.LowSill:
                    switch (input.LoadSchematizationType)
                    {
                        case LoadSchematizationType.Linear:
                            validationRules = GetLowSillLinearValidationRules(input);
                            break;
                        case LoadSchematizationType.Quadratic:
                            validationRules = GetLowSillQuadraticValidationRules(input);
                            break;
                        default:
                            throw new InvalidEnumArgumentException(nameof(input),
                                                                   (int) input.LoadSchematizationType,
                                                                   typeof(LoadSchematizationType));
                    }
                    break;
                case StabilityPointStructureInflowModelType.FloodedCulvert:
                    switch (input.LoadSchematizationType)
                    {
                        case LoadSchematizationType.Linear:
                            validationRules = GetFloodedCulvertLinearValidationRules(input);
                            break;
                        case LoadSchematizationType.Quadratic:
                            validationRules = GetFloodedCulvertQuadraticValidationRules(input);
                            break;
                        default:
                            throw new InvalidEnumArgumentException(nameof(input),
                                                                   (int) input.LoadSchematizationType,
                                                                   typeof(LoadSchematizationType));
                    }
                    break;
                default:
                    throw new InvalidEnumArgumentException(nameof(input),
                                                           (int) input.InflowModelType,
                                                           typeof(StabilityPointStructureInflowModelType));
            }

            return validationRules;
        }

        private static IEnumerable<ValidationRule> GetLowSillLinearValidationRules(StabilityPointStructuresInput input)
        {
            return new ValidationRule[]
            {
                new UseBreakWaterRule(input),
                new NumericInputRule(
                    input.VolumicWeightWater,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_VolumicWeightWater_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(
                    input.StormDuration,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_StormDuration_DisplayName)),
                new NormalDistributionRule(
                    input.InsideWaterLevel,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_InsideWaterLevel_DisplayName)),
                new NormalDistributionRule(
                    input.InsideWaterLevelFailureConstruction,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_InsideWaterLevelFailureConstruction_DisplayName)),
                new VariationCoefficientNormalDistributionRule(
                    input.FlowVelocityStructureClosable,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_FlowVelocityStructureClosable_DisplayName)),
                new NormalDistributionRule(
                    input.ModelFactorSuperCriticalFlow,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_ModelFactorSuperCriticalFlow_DisplayName)),
                new NumericInputRule(
                    input.FactorStormDurationOpenStructure,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_FactorStormDurationOpenStructure_DisplayName)),
                new NumericInputRule(
                    input.StructureNormalOrientation,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_StructureNormalOrientation_DisplayName)),
                new NormalDistributionRule(
                    input.WidthFlowApertures,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_WidthFlowApertures_DisplayName)),
                new LogNormalDistributionRule(
                    input.FlowWidthAtBottomProtection,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_FlowWidthAtBottomProtection_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(
                    input.StorageStructureArea,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_StorageStructureArea_DisplayName)),
                new LogNormalDistributionRule(
                    input.AllowedLevelIncreaseStorage,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_AllowedLevelIncreaseStorage_DisplayName)),
                new NormalDistributionRule(
                    input.LevelCrestStructure,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_LevelCrestStructure_DisplayName)),
                new NormalDistributionRule(
                    input.ThresholdHeightOpenWeir,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_ThresholdHeightOpenWeir_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(
                    input.CriticalOvertoppingDischarge,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_CriticalOvertoppingDischarge_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(
                    input.ConstructiveStrengthLinearLoadModel,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_ConstructiveStrengthLinearLoadModel_DisplayName)),
                new NormalDistributionRule(
                    input.BankWidth,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_BankWidth_DisplayName)),
                new NumericInputRule(
                    input.EvaluationLevel,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_EvaluationLevel_DisplayName)),
                new NumericInputRule(
                    input.VerticalDistance,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_VerticalDistance_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(
                    input.FailureCollisionEnergy,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_FailureCollisionEnergy_DisplayName)),
                new VariationCoefficientNormalDistributionRule(
                    input.ShipMass,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_ShipMass_DisplayName)),
                new VariationCoefficientNormalDistributionRule(
                    input.ShipVelocity,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_ShipVelocity_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(
                    input.StabilityLinearLoadModel,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_StabilityLinearLoadModel_DisplayName))
            };
        }

        private static IEnumerable<ValidationRule> GetLowSillQuadraticValidationRules(StabilityPointStructuresInput input)
        {
            return new ValidationRule[]
            {
                new UseBreakWaterRule(input),
                new NumericInputRule(
                    input.VolumicWeightWater,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_VolumicWeightWater_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(
                    input.StormDuration,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_StormDuration_DisplayName)),
                new NormalDistributionRule(
                    input.InsideWaterLevel,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_InsideWaterLevel_DisplayName)),
                new NormalDistributionRule(
                    input.InsideWaterLevelFailureConstruction,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_InsideWaterLevelFailureConstruction_DisplayName)),
                new VariationCoefficientNormalDistributionRule(
                    input.FlowVelocityStructureClosable,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_FlowVelocityStructureClosable_DisplayName)),
                new NormalDistributionRule(
                    input.ModelFactorSuperCriticalFlow,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_ModelFactorSuperCriticalFlow_DisplayName)),
                new NumericInputRule(
                    input.FactorStormDurationOpenStructure,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_FactorStormDurationOpenStructure_DisplayName)),
                new NumericInputRule(
                    input.StructureNormalOrientation,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_StructureNormalOrientation_DisplayName)),
                new NormalDistributionRule(
                    input.WidthFlowApertures,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_WidthFlowApertures_DisplayName)),
                new LogNormalDistributionRule(
                    input.FlowWidthAtBottomProtection,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_FlowWidthAtBottomProtection_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(
                    input.StorageStructureArea,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_StorageStructureArea_DisplayName)),
                new LogNormalDistributionRule(
                    input.AllowedLevelIncreaseStorage,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_AllowedLevelIncreaseStorage_DisplayName)),
                new NormalDistributionRule(
                    input.LevelCrestStructure,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_LevelCrestStructure_DisplayName)),
                new NormalDistributionRule(
                    input.ThresholdHeightOpenWeir,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_ThresholdHeightOpenWeir_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(
                    input.CriticalOvertoppingDischarge,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_CriticalOvertoppingDischarge_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(
                    input.ConstructiveStrengthQuadraticLoadModel,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_ConstructiveStrengthQuadraticLoadModel_DisplayName)),
                new NormalDistributionRule(
                    input.BankWidth,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_BankWidth_DisplayName)),
                new NumericInputRule(
                    input.EvaluationLevel,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_EvaluationLevel_DisplayName)),
                new NumericInputRule(
                    input.VerticalDistance,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_VerticalDistance_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(
                    input.FailureCollisionEnergy,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_FailureCollisionEnergy_DisplayName)),
                new VariationCoefficientNormalDistributionRule(
                    input.ShipMass,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_ShipMass_DisplayName)),
                new VariationCoefficientNormalDistributionRule(
                    input.ShipVelocity,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_ShipVelocity_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(
                    input.StabilityQuadraticLoadModel,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_StabilityQuadraticLoadModel_DisplayName))
            };
        }

        private static IEnumerable<ValidationRule> GetFloodedCulvertLinearValidationRules(StabilityPointStructuresInput input)
        {
            return new ValidationRule[]
            {
                new UseBreakWaterRule(input),
                new NumericInputRule(
                    input.VolumicWeightWater,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_VolumicWeightWater_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(
                    input.StormDuration,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_StormDuration_DisplayName)),
                new NormalDistributionRule(
                    input.InsideWaterLevel,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_InsideWaterLevel_DisplayName)),
                new NormalDistributionRule(
                    input.InsideWaterLevelFailureConstruction,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_InsideWaterLevelFailureConstruction_DisplayName)),
                new VariationCoefficientNormalDistributionRule(
                    input.FlowVelocityStructureClosable,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_FlowVelocityStructureClosable_DisplayName)),
                new NormalDistributionRule(
                    input.DrainCoefficient,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_DrainCoefficient_DisplayName)),
                new NumericInputRule(
                    input.FactorStormDurationOpenStructure,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_FactorStormDurationOpenStructure_DisplayName)),
                new NumericInputRule(
                    input.StructureNormalOrientation,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_StructureNormalOrientation_DisplayName)),
                new LogNormalDistributionRule(
                    input.AreaFlowApertures,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_AreaFlowApertures_DisplayName)),
                new LogNormalDistributionRule(
                    input.FlowWidthAtBottomProtection,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_FlowWidthAtBottomProtection_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(
                    input.StorageStructureArea,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_StorageStructureArea_DisplayName)),
                new LogNormalDistributionRule(
                    input.AllowedLevelIncreaseStorage,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_AllowedLevelIncreaseStorage_DisplayName)),
                new NormalDistributionRule(
                    input.LevelCrestStructure,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_LevelCrestStructure_DisplayName)),
                new NormalDistributionRule(
                    input.ThresholdHeightOpenWeir,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_ThresholdHeightOpenWeir_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(
                    input.CriticalOvertoppingDischarge,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_CriticalOvertoppingDischarge_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(
                    input.ConstructiveStrengthLinearLoadModel,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_ConstructiveStrengthLinearLoadModel_DisplayName)),
                new NormalDistributionRule(
                    input.BankWidth,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_BankWidth_DisplayName)),
                new NumericInputRule(
                    input.EvaluationLevel,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_EvaluationLevel_DisplayName)),
                new NumericInputRule(
                    input.VerticalDistance,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_VerticalDistance_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(
                    input.FailureCollisionEnergy,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_FailureCollisionEnergy_DisplayName)),
                new VariationCoefficientNormalDistributionRule(
                    input.ShipMass,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_ShipMass_DisplayName)),
                new VariationCoefficientNormalDistributionRule(
                    input.ShipVelocity,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_ShipVelocity_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(
                    input.StabilityLinearLoadModel,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_StabilityLinearLoadModel_DisplayName))
            };
        }

        private static IEnumerable<ValidationRule> GetFloodedCulvertQuadraticValidationRules(StabilityPointStructuresInput input)
        {
            return new ValidationRule[]
            {
                new UseBreakWaterRule(input),
                new NumericInputRule(
                    input.VolumicWeightWater,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_VolumicWeightWater_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(
                    input.StormDuration,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_StormDuration_DisplayName)),
                new NormalDistributionRule(
                    input.InsideWaterLevel,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_InsideWaterLevel_DisplayName)),
                new NormalDistributionRule(
                    input.InsideWaterLevelFailureConstruction,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_InsideWaterLevelFailureConstruction_DisplayName)),
                new VariationCoefficientNormalDistributionRule(
                    input.FlowVelocityStructureClosable,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_FlowVelocityStructureClosable_DisplayName)),
                new NormalDistributionRule(
                    input.DrainCoefficient,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_DrainCoefficient_DisplayName)),
                new NumericInputRule(
                    input.FactorStormDurationOpenStructure,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_FactorStormDurationOpenStructure_DisplayName)),
                new NumericInputRule(
                    input.StructureNormalOrientation,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_StructureNormalOrientation_DisplayName)),
                new LogNormalDistributionRule(
                    input.AreaFlowApertures,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_AreaFlowApertures_DisplayName)),
                new LogNormalDistributionRule(
                    input.FlowWidthAtBottomProtection,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_FlowWidthAtBottomProtection_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(
                    input.StorageStructureArea,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_StorageStructureArea_DisplayName)),
                new LogNormalDistributionRule(
                    input.AllowedLevelIncreaseStorage,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_AllowedLevelIncreaseStorage_DisplayName)),
                new NormalDistributionRule(
                    input.LevelCrestStructure,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_LevelCrestStructure_DisplayName)),
                new NormalDistributionRule(
                    input.ThresholdHeightOpenWeir,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_ThresholdHeightOpenWeir_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(
                    input.CriticalOvertoppingDischarge,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_CriticalOvertoppingDischarge_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(
                    input.ConstructiveStrengthQuadraticLoadModel,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_ConstructiveStrengthQuadraticLoadModel_DisplayName)),
                new NormalDistributionRule(
                    input.BankWidth,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_BankWidth_DisplayName)),
                new NumericInputRule(
                    input.EvaluationLevel,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_EvaluationLevel_DisplayName)),
                new NumericInputRule(
                    input.VerticalDistance,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_VerticalDistance_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(
                    input.FailureCollisionEnergy,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_FailureCollisionEnergy_DisplayName)),
                new VariationCoefficientNormalDistributionRule(
                    input.ShipMass,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_ShipMass_DisplayName)),
                new VariationCoefficientNormalDistributionRule(
                    input.ShipVelocity,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_ShipVelocity_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(
                    input.StabilityQuadraticLoadModel,
                    ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_StabilityQuadraticLoadModel_DisplayName))
            };
        }
    }
}