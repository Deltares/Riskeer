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

using System.ComponentModel;
using Ringtoets.Common.Service;
using Ringtoets.Common.Service.Structures;
using Ringtoets.HydraRing.Calculation.Data.Input.Structures;
using Ringtoets.StabilityPointStructures.Data;

namespace Ringtoets.StabilityPointStructures.Service
{
    /// <summary>
    /// Service that provides methods for performing Hydra-ring calculations for stability point structures.
    /// </summary>
    public class StabilityPointStructuresCalculationService : StructuresCalculationServiceBase<
        StabilityPointStructuresValidationRulesRegistry,
        StabilityPointStructuresInput,
        StabilityPointStructure,
        GeneralStabilityPointStructuresInput,
        StructuresStabilityPointCalculationInput>
    {
        /// <summary>
        /// Creates a new instance of <see cref="StabilityPointStructuresCalculationService"/>.
        /// </summary>
        public StabilityPointStructuresCalculationService() : base(new StabilityPointStructuresCalculationMessageProvider()) {}

        protected override StructuresStabilityPointCalculationInput CreateInput(StabilityPointStructuresInput structureInput,
                                                                                GeneralStabilityPointStructuresInput generalInput,
                                                                                string hydraulicBoundaryDatabaseFilePath)
        {
            StructuresStabilityPointCalculationInput input;
            switch (structureInput.InflowModelType)
            {
                case StabilityPointStructureInflowModelType.LowSill:
                    switch (structureInput.LoadSchematizationType)
                    {
                        case LoadSchematizationType.Linear:
                            input = CreateLowSillLinearCalculationInput(
                                structureInput,
                                generalInput);
                            break;
                        case LoadSchematizationType.Quadratic:
                            input = CreateLowSillQuadraticCalculationInput(
                                structureInput,
                                generalInput);
                            break;
                        default:
                            throw new InvalidEnumArgumentException(nameof(structureInput),
                                                                   (int) structureInput.LoadSchematizationType,
                                                                   typeof(LoadSchematizationType));
                    }
                    break;
                case StabilityPointStructureInflowModelType.FloodedCulvert:
                    switch (structureInput.LoadSchematizationType)
                    {
                        case LoadSchematizationType.Linear:
                            input = CreateFloodedCulvertLinearCalculationInput(
                                structureInput,
                                generalInput);
                            break;
                        case LoadSchematizationType.Quadratic:
                            input = CreateFloodedCulvertQuadraticCalculationInput(
                                structureInput,
                                generalInput);
                            break;
                        default:
                            throw new InvalidEnumArgumentException(nameof(structureInput),
                                                                   (int) structureInput.LoadSchematizationType,
                                                                   typeof(LoadSchematizationType));
                    }
                    break;
                default:
                    throw new InvalidEnumArgumentException(nameof(structureInput),
                                                           (int) structureInput.InflowModelType,
                                                           typeof(StabilityPointStructureInflowModelType));
            }

            HydraRingSettingsDatabaseHelper.AssignSettingsFromDatabase(input, hydraulicBoundaryDatabaseFilePath);
            return input;
        }

        private StructuresStabilityPointLowSillLinearCalculationInput CreateLowSillLinearCalculationInput(
            StabilityPointStructuresInput structureInput,
            GeneralStabilityPointStructuresInput generalInput)
        {
            var structuresStabilityPointLowSillLinearCalculationInput = new StructuresStabilityPointLowSillLinearCalculationInput(
                structureInput.HydraulicBoundaryLocation.Id,
                structureInput.StructureNormalOrientation,
                HydraRingInputParser.ParseForeshore(structureInput),
                HydraRingInputParser.ParseBreakWater(structureInput),
                structureInput.VolumicWeightWater,
                generalInput.GravitationalAcceleration,
                structureInput.LevelCrestStructure.Mean,
                structureInput.LevelCrestStructure.StandardDeviation,
                structureInput.StructureNormalOrientation,
                structureInput.FactorStormDurationOpenStructure,
                generalInput.ModelFactorSubCriticalFlow.Mean,
                generalInput.ModelFactorSubCriticalFlow.CoefficientOfVariation,
                structureInput.ThresholdHeightOpenWeir.Mean,
                structureInput.ThresholdHeightOpenWeir.StandardDeviation,
                structureInput.InsideWaterLevelFailureConstruction.Mean,
                structureInput.InsideWaterLevelFailureConstruction.StandardDeviation,
                structureInput.FailureProbabilityRepairClosure,
                structureInput.FailureCollisionEnergy.Mean,
                structureInput.FailureCollisionEnergy.CoefficientOfVariation,
                generalInput.ModelFactorCollisionLoad.Mean,
                generalInput.ModelFactorCollisionLoad.CoefficientOfVariation,
                structureInput.ShipMass.Mean,
                structureInput.ShipMass.CoefficientOfVariation,
                structureInput.ShipVelocity.Mean,
                structureInput.ShipVelocity.CoefficientOfVariation,
                structureInput.LevellingCount,
                structureInput.ProbabilityCollisionSecondaryStructure,
                structureInput.FlowVelocityStructureClosable.Mean,
                structureInput.FlowVelocityStructureClosable.CoefficientOfVariation,
                structureInput.InsideWaterLevel.Mean,
                structureInput.InsideWaterLevel.StandardDeviation,
                structureInput.AllowedLevelIncreaseStorage.Mean,
                structureInput.AllowedLevelIncreaseStorage.StandardDeviation,
                generalInput.ModelFactorStorageVolume.Mean,
                generalInput.ModelFactorStorageVolume.StandardDeviation,
                structureInput.StorageStructureArea.Mean,
                structureInput.StorageStructureArea.CoefficientOfVariation,
                generalInput.ModelFactorInflowVolume,
                structureInput.FlowWidthAtBottomProtection.Mean,
                structureInput.FlowWidthAtBottomProtection.StandardDeviation,
                structureInput.CriticalOvertoppingDischarge.Mean,
                structureInput.CriticalOvertoppingDischarge.CoefficientOfVariation,
                structureInput.FailureProbabilityStructureWithErosion,
                structureInput.StormDuration.Mean,
                structureInput.StormDuration.CoefficientOfVariation,
                structureInput.BankWidth.Mean,
                structureInput.BankWidth.StandardDeviation,
                structureInput.EvaluationLevel,
                generalInput.ModelFactorLoadEffect.Mean,
                generalInput.ModelFactorLoadEffect.StandardDeviation,
                generalInput.WaveRatioMaxHN,
                generalInput.WaveRatioMaxHStandardDeviation,
                structureInput.VerticalDistance,
                generalInput.ModificationFactorWavesSlowlyVaryingPressureComponent,
                generalInput.ModificationFactorDynamicOrImpulsivePressureComponent,
                structureInput.ModelFactorSuperCriticalFlow.Mean,
                structureInput.ModelFactorSuperCriticalFlow.StandardDeviation,
                structureInput.ConstructiveStrengthLinearLoadModel.Mean,
                structureInput.ConstructiveStrengthLinearLoadModel.CoefficientOfVariation,
                structureInput.StabilityLinearLoadModel.Mean,
                structureInput.StabilityLinearLoadModel.CoefficientOfVariation,
                structureInput.WidthFlowApertures.Mean,
                structureInput.WidthFlowApertures.StandardDeviation);

            return structuresStabilityPointLowSillLinearCalculationInput;
        }

        private StructuresStabilityPointLowSillQuadraticCalculationInput CreateLowSillQuadraticCalculationInput(
            StabilityPointStructuresInput structureInput,
            GeneralStabilityPointStructuresInput generalInput)
        {
            return new StructuresStabilityPointLowSillQuadraticCalculationInput(
                structureInput.HydraulicBoundaryLocation.Id,
                structureInput.StructureNormalOrientation,
                HydraRingInputParser.ParseForeshore(structureInput),
                HydraRingInputParser.ParseBreakWater(structureInput),
                structureInput.VolumicWeightWater,
                generalInput.GravitationalAcceleration,
                structureInput.LevelCrestStructure.Mean,
                structureInput.LevelCrestStructure.StandardDeviation,
                structureInput.StructureNormalOrientation,
                structureInput.FactorStormDurationOpenStructure,
                generalInput.ModelFactorSubCriticalFlow.Mean,
                generalInput.ModelFactorSubCriticalFlow.CoefficientOfVariation,
                structureInput.ThresholdHeightOpenWeir.Mean,
                structureInput.ThresholdHeightOpenWeir.StandardDeviation,
                structureInput.InsideWaterLevelFailureConstruction.Mean,
                structureInput.InsideWaterLevelFailureConstruction.StandardDeviation,
                structureInput.FailureProbabilityRepairClosure,
                structureInput.FailureCollisionEnergy.Mean,
                structureInput.FailureCollisionEnergy.CoefficientOfVariation,
                generalInput.ModelFactorCollisionLoad.Mean,
                generalInput.ModelFactorCollisionLoad.CoefficientOfVariation,
                structureInput.ShipMass.Mean,
                structureInput.ShipMass.CoefficientOfVariation,
                structureInput.ShipVelocity.Mean,
                structureInput.ShipVelocity.CoefficientOfVariation,
                structureInput.LevellingCount,
                structureInput.ProbabilityCollisionSecondaryStructure,
                structureInput.FlowVelocityStructureClosable.Mean,
                structureInput.FlowVelocityStructureClosable.CoefficientOfVariation,
                structureInput.InsideWaterLevel.Mean,
                structureInput.InsideWaterLevel.StandardDeviation,
                structureInput.AllowedLevelIncreaseStorage.Mean,
                structureInput.AllowedLevelIncreaseStorage.StandardDeviation,
                generalInput.ModelFactorStorageVolume.Mean,
                generalInput.ModelFactorStorageVolume.StandardDeviation,
                structureInput.StorageStructureArea.Mean,
                structureInput.StorageStructureArea.CoefficientOfVariation,
                generalInput.ModelFactorInflowVolume,
                structureInput.FlowWidthAtBottomProtection.Mean,
                structureInput.FlowWidthAtBottomProtection.StandardDeviation,
                structureInput.CriticalOvertoppingDischarge.Mean,
                structureInput.CriticalOvertoppingDischarge.CoefficientOfVariation,
                structureInput.FailureProbabilityStructureWithErosion,
                structureInput.StormDuration.Mean,
                structureInput.StormDuration.CoefficientOfVariation,
                structureInput.BankWidth.Mean,
                structureInput.BankWidth.StandardDeviation,
                structureInput.EvaluationLevel,
                generalInput.ModelFactorLoadEffect.Mean,
                generalInput.ModelFactorLoadEffect.StandardDeviation,
                generalInput.WaveRatioMaxHN,
                generalInput.WaveRatioMaxHStandardDeviation,
                structureInput.VerticalDistance,
                generalInput.ModificationFactorWavesSlowlyVaryingPressureComponent,
                generalInput.ModificationFactorDynamicOrImpulsivePressureComponent,
                structureInput.ModelFactorSuperCriticalFlow.Mean,
                structureInput.ModelFactorSuperCriticalFlow.StandardDeviation,
                structureInput.ConstructiveStrengthQuadraticLoadModel.Mean,
                structureInput.ConstructiveStrengthQuadraticLoadModel.CoefficientOfVariation,
                structureInput.StabilityQuadraticLoadModel.Mean,
                structureInput.StabilityQuadraticLoadModel.CoefficientOfVariation,
                structureInput.WidthFlowApertures.Mean,
                structureInput.WidthFlowApertures.StandardDeviation);
        }

        private StructuresStabilityPointFloodedCulvertLinearCalculationInput CreateFloodedCulvertLinearCalculationInput(
            StabilityPointStructuresInput structureInput,
            GeneralStabilityPointStructuresInput generalInput)
        {
            return new StructuresStabilityPointFloodedCulvertLinearCalculationInput(
                structureInput.HydraulicBoundaryLocation.Id,
                structureInput.StructureNormalOrientation,
                HydraRingInputParser.ParseForeshore(structureInput),
                HydraRingInputParser.ParseBreakWater(structureInput),
                structureInput.VolumicWeightWater,
                generalInput.GravitationalAcceleration,
                structureInput.LevelCrestStructure.Mean,
                structureInput.LevelCrestStructure.StandardDeviation,
                structureInput.StructureNormalOrientation,
                structureInput.FactorStormDurationOpenStructure,
                generalInput.ModelFactorSubCriticalFlow.Mean,
                generalInput.ModelFactorSubCriticalFlow.CoefficientOfVariation,
                structureInput.ThresholdHeightOpenWeir.Mean,
                structureInput.ThresholdHeightOpenWeir.StandardDeviation,
                structureInput.InsideWaterLevelFailureConstruction.Mean,
                structureInput.InsideWaterLevelFailureConstruction.StandardDeviation,
                structureInput.FailureProbabilityRepairClosure,
                structureInput.FailureCollisionEnergy.Mean,
                structureInput.FailureCollisionEnergy.CoefficientOfVariation,
                generalInput.ModelFactorCollisionLoad.Mean,
                generalInput.ModelFactorCollisionLoad.CoefficientOfVariation,
                structureInput.ShipMass.Mean,
                structureInput.ShipMass.CoefficientOfVariation,
                structureInput.ShipVelocity.Mean,
                structureInput.ShipVelocity.CoefficientOfVariation,
                structureInput.LevellingCount,
                structureInput.ProbabilityCollisionSecondaryStructure,
                structureInput.FlowVelocityStructureClosable.Mean,
                structureInput.FlowVelocityStructureClosable.CoefficientOfVariation,
                structureInput.InsideWaterLevel.Mean,
                structureInput.InsideWaterLevel.StandardDeviation,
                structureInput.AllowedLevelIncreaseStorage.Mean,
                structureInput.AllowedLevelIncreaseStorage.StandardDeviation,
                generalInput.ModelFactorStorageVolume.Mean,
                generalInput.ModelFactorStorageVolume.StandardDeviation,
                structureInput.StorageStructureArea.Mean,
                structureInput.StorageStructureArea.CoefficientOfVariation,
                generalInput.ModelFactorInflowVolume,
                structureInput.FlowWidthAtBottomProtection.Mean,
                structureInput.FlowWidthAtBottomProtection.StandardDeviation,
                structureInput.CriticalOvertoppingDischarge.Mean,
                structureInput.CriticalOvertoppingDischarge.CoefficientOfVariation,
                structureInput.FailureProbabilityStructureWithErosion,
                structureInput.StormDuration.Mean,
                structureInput.StormDuration.CoefficientOfVariation,
                structureInput.BankWidth.Mean,
                structureInput.BankWidth.StandardDeviation,
                structureInput.EvaluationLevel,
                generalInput.ModelFactorLoadEffect.Mean,
                generalInput.ModelFactorLoadEffect.StandardDeviation,
                generalInput.WaveRatioMaxHN,
                generalInput.WaveRatioMaxHStandardDeviation,
                structureInput.VerticalDistance,
                generalInput.ModificationFactorWavesSlowlyVaryingPressureComponent,
                generalInput.ModificationFactorDynamicOrImpulsivePressureComponent,
                structureInput.DrainCoefficient.Mean,
                structureInput.DrainCoefficient.StandardDeviation,
                structureInput.AreaFlowApertures.Mean,
                structureInput.AreaFlowApertures.StandardDeviation,
                structureInput.ConstructiveStrengthLinearLoadModel.Mean,
                structureInput.ConstructiveStrengthLinearLoadModel.CoefficientOfVariation,
                structureInput.StabilityLinearLoadModel.Mean,
                structureInput.StabilityLinearLoadModel.CoefficientOfVariation);
        }

        private StructuresStabilityPointFloodedCulvertQuadraticCalculationInput CreateFloodedCulvertQuadraticCalculationInput(
            StabilityPointStructuresInput structureInput,
            GeneralStabilityPointStructuresInput generalInput)
        {
            return new StructuresStabilityPointFloodedCulvertQuadraticCalculationInput(
                structureInput.HydraulicBoundaryLocation.Id,
                structureInput.StructureNormalOrientation,
                HydraRingInputParser.ParseForeshore(structureInput),
                HydraRingInputParser.ParseBreakWater(structureInput),
                structureInput.VolumicWeightWater,
                generalInput.GravitationalAcceleration,
                structureInput.LevelCrestStructure.Mean,
                structureInput.LevelCrestStructure.StandardDeviation,
                structureInput.StructureNormalOrientation,
                structureInput.FactorStormDurationOpenStructure,
                generalInput.ModelFactorSubCriticalFlow.Mean,
                generalInput.ModelFactorSubCriticalFlow.CoefficientOfVariation,
                structureInput.ThresholdHeightOpenWeir.Mean,
                structureInput.ThresholdHeightOpenWeir.StandardDeviation,
                structureInput.InsideWaterLevelFailureConstruction.Mean,
                structureInput.InsideWaterLevelFailureConstruction.StandardDeviation,
                structureInput.FailureProbabilityRepairClosure,
                structureInput.FailureCollisionEnergy.Mean,
                structureInput.FailureCollisionEnergy.CoefficientOfVariation,
                generalInput.ModelFactorCollisionLoad.Mean,
                generalInput.ModelFactorCollisionLoad.CoefficientOfVariation,
                structureInput.ShipMass.Mean,
                structureInput.ShipMass.CoefficientOfVariation,
                structureInput.ShipVelocity.Mean,
                structureInput.ShipVelocity.CoefficientOfVariation,
                structureInput.LevellingCount,
                structureInput.ProbabilityCollisionSecondaryStructure,
                structureInput.FlowVelocityStructureClosable.Mean,
                structureInput.FlowVelocityStructureClosable.CoefficientOfVariation,
                structureInput.InsideWaterLevel.Mean,
                structureInput.InsideWaterLevel.StandardDeviation,
                structureInput.AllowedLevelIncreaseStorage.Mean,
                structureInput.AllowedLevelIncreaseStorage.StandardDeviation,
                generalInput.ModelFactorStorageVolume.Mean,
                generalInput.ModelFactorStorageVolume.StandardDeviation,
                structureInput.StorageStructureArea.Mean,
                structureInput.StorageStructureArea.CoefficientOfVariation,
                generalInput.ModelFactorInflowVolume,
                structureInput.FlowWidthAtBottomProtection.Mean,
                structureInput.FlowWidthAtBottomProtection.StandardDeviation,
                structureInput.CriticalOvertoppingDischarge.Mean,
                structureInput.CriticalOvertoppingDischarge.CoefficientOfVariation,
                structureInput.FailureProbabilityStructureWithErosion,
                structureInput.StormDuration.Mean,
                structureInput.StormDuration.CoefficientOfVariation,
                structureInput.BankWidth.Mean,
                structureInput.BankWidth.StandardDeviation,
                structureInput.EvaluationLevel,
                generalInput.ModelFactorLoadEffect.Mean,
                generalInput.ModelFactorLoadEffect.StandardDeviation,
                generalInput.WaveRatioMaxHN,
                generalInput.WaveRatioMaxHStandardDeviation,
                structureInput.VerticalDistance,
                generalInput.ModificationFactorWavesSlowlyVaryingPressureComponent,
                generalInput.ModificationFactorDynamicOrImpulsivePressureComponent,
                structureInput.DrainCoefficient.Mean,
                structureInput.DrainCoefficient.StandardDeviation,
                structureInput.AreaFlowApertures.Mean,
                structureInput.AreaFlowApertures.StandardDeviation,
                structureInput.ConstructiveStrengthQuadraticLoadModel.Mean,
                structureInput.ConstructiveStrengthQuadraticLoadModel.CoefficientOfVariation,
                structureInput.StabilityQuadraticLoadModel.Mean,
                structureInput.StabilityQuadraticLoadModel.CoefficientOfVariation);
        }
    }
}