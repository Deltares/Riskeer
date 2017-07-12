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
using log4net;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Service;
using Ringtoets.Common.Service.MessageProviders;
using Ringtoets.Common.Service.Structures;
using Ringtoets.HydraRing.Calculation.Calculator;
using Ringtoets.HydraRing.Calculation.Data.Input.Structures;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Service.Properties;

namespace Ringtoets.StabilityPointStructures.Service
{
    /// <summary>
    /// Service that provides methods for performing Hydra-ring calculations for stability point structures.
    /// </summary>
    public class StabilityPointStructuresCalculationService : StructuresCalculationServiceBase<StabilityPointStructuresValidationRulesRegistry,
        StabilityPointStructuresInput,
        StabilityPointStructure,
        StabilityPointStructuresFailureMechanism,
        StructuresStabilityPointCalculationInput>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(StabilityPointStructuresCalculationService));

        /// <summary>
        /// Creates a new instance of <see cref="StabilityPointStructuresCalculationService"/>.
        /// </summary>
        public StabilityPointStructuresCalculationService() : base(new StabilityPointStructuresCalculationMessageProvider()) {}

        protected override StructuresStabilityPointCalculationInput CreateInput(StructuresCalculation<StabilityPointStructuresInput> calculation,
                                                                                StabilityPointStructuresFailureMechanism failureMechanism,
                                                                                string hydraulicBoundaryDatabaseFilePath)
        {
            StructuresStabilityPointCalculationInput input;
            switch (calculation.InputParameters.InflowModelType)
            {
                case StabilityPointStructureInflowModelType.LowSill:
                    switch (calculation.InputParameters.LoadSchematizationType)
                    {
                        case LoadSchematizationType.Linear:
                            input = CreateLowSillLinearCalculationInput(
                                calculation,
                                failureMechanism.GeneralInput);
                            break;
                        case LoadSchematizationType.Quadratic:
                            input = CreateLowSillQuadraticCalculationInput(
                                calculation,
                                failureMechanism.GeneralInput);
                            break;
                        default:
                            throw new InvalidEnumArgumentException(nameof(calculation),
                                                                   (int) calculation.InputParameters.LoadSchematizationType,
                                                                   typeof(LoadSchematizationType));
                    }
                    break;
                case StabilityPointStructureInflowModelType.FloodedCulvert:
                    switch (calculation.InputParameters.LoadSchematizationType)
                    {
                        case LoadSchematizationType.Linear:
                            input = CreateFloodedCulvertLinearCalculationInput(
                                calculation,
                                failureMechanism.GeneralInput);
                            break;
                        case LoadSchematizationType.Quadratic:
                            input = CreateFloodedCulvertQuadraticCalculationInput(
                                calculation,
                                failureMechanism.GeneralInput);
                            break;
                        default:
                            throw new InvalidEnumArgumentException(nameof(calculation),
                                                                   (int) calculation.InputParameters.LoadSchematizationType,
                                                                   typeof(LoadSchematizationType));
                    }
                    break;
                default:
                    throw new InvalidEnumArgumentException(nameof(calculation),
                                                           (int) calculation.InputParameters.InflowModelType,
                                                           typeof(StabilityPointStructureInflowModelType));
            }

            HydraRingSettingsDatabaseHelper.AssignSettingsFromDatabase(input, hydraulicBoundaryDatabaseFilePath);
            return input;
        }

        protected override void PerformCalculation(IStructuresCalculator<StructuresStabilityPointCalculationInput> calculator,
                                                   StructuresStabilityPointCalculationInput input,
                                                   StructuresCalculation<StabilityPointStructuresInput> calculation,
                                                   IAssessmentSection assessmentSection,
                                                   StabilityPointStructuresFailureMechanism failureMechanism)
        {
            var exceptionThrown = false;
            try
            {
                calculator.Calculate(input);

                if (!Canceled && string.IsNullOrEmpty(calculator.LastErrorFileContent))
                {
                    ProbabilityAssessmentOutput probabilityAssessmentOutput =
                        ProbabilityAssessmentService.Calculate(assessmentSection.FailureMechanismContribution.Norm,
                                                               failureMechanism.Contribution,
                                                               failureMechanism.GeneralInput.N,
                                                               calculator.ExceedanceProbabilityBeta);
                    calculation.Output = new StructuresOutput(probabilityAssessmentOutput);
                }
            }
            catch (HydraRingCalculationException)
            {
                if (!Canceled)
                {
                    string lastErrorContent = calculator.LastErrorFileContent;
                    if (string.IsNullOrEmpty(lastErrorContent))
                    {
                        log.ErrorFormat(Resources.StabilityPointStructuresCalculationService_Calculate_Error_in_StabilityPointStructuresCalculation_0_no_error_report,
                                        calculation.Name);
                    }
                    else
                    {
                        log.ErrorFormat(Resources.StabilityPointStructuresCalculationService_Calculate_Error_in_StabilityPointStructuresCalculation_0_click_details_for_last_error_report_1,
                                        calculation.Name, lastErrorContent);
                    }

                    exceptionThrown = true;
                    throw;
                }
            }
            finally
            {
                string lastErrorFileContent = calculator.LastErrorFileContent;
                bool errorOccurred = CalculationServiceHelper.HasErrorOccurred(Canceled, exceptionThrown, lastErrorFileContent);
                if (errorOccurred)
                {
                    log.ErrorFormat(Resources.StabilityPointStructuresCalculationService_Calculate_Error_in_StabilityPointStructuresCalculation_0_click_details_for_last_error_report_1,
                                    calculation.Name, lastErrorFileContent);
                }

                log.InfoFormat(Resources.StabilityPointStructuresCalculationService_Calculate_Calculation_temporary_directory_can_be_found_on_location_0, calculator.OutputDirectory);
                CalculationServiceHelper.LogCalculationEnd();

                if (errorOccurred)
                {
                    throw new HydraRingCalculationException(lastErrorFileContent);
                }
            }
        }

        private StructuresStabilityPointLowSillLinearCalculationInput CreateLowSillLinearCalculationInput(StructuresCalculation<StabilityPointStructuresInput> calculation,
                                                                                                          GeneralStabilityPointStructuresInput generalInput)
        {
            var structuresStabilityPointLowSillLinearCalculationInput = new StructuresStabilityPointLowSillLinearCalculationInput(
                calculation.InputParameters.HydraulicBoundaryLocation.Id,
                calculation.InputParameters.StructureNormalOrientation,
                HydraRingInputParser.ParseForeshore(calculation.InputParameters),
                HydraRingInputParser.ParseBreakWater(calculation.InputParameters),
                calculation.InputParameters.VolumicWeightWater,
                generalInput.GravitationalAcceleration,
                calculation.InputParameters.LevelCrestStructure.Mean,
                calculation.InputParameters.LevelCrestStructure.StandardDeviation,
                calculation.InputParameters.StructureNormalOrientation,
                calculation.InputParameters.FactorStormDurationOpenStructure,
                generalInput.ModelFactorSubCriticalFlow.Mean,
                generalInput.ModelFactorSubCriticalFlow.CoefficientOfVariation,
                calculation.InputParameters.ThresholdHeightOpenWeir.Mean,
                calculation.InputParameters.ThresholdHeightOpenWeir.StandardDeviation,
                calculation.InputParameters.InsideWaterLevelFailureConstruction.Mean,
                calculation.InputParameters.InsideWaterLevelFailureConstruction.StandardDeviation,
                calculation.InputParameters.FailureProbabilityRepairClosure,
                calculation.InputParameters.FailureCollisionEnergy.Mean,
                calculation.InputParameters.FailureCollisionEnergy.CoefficientOfVariation,
                generalInput.ModelFactorCollisionLoad.Mean,
                generalInput.ModelFactorCollisionLoad.CoefficientOfVariation,
                calculation.InputParameters.ShipMass.Mean,
                calculation.InputParameters.ShipMass.CoefficientOfVariation,
                calculation.InputParameters.ShipVelocity.Mean,
                calculation.InputParameters.ShipVelocity.CoefficientOfVariation,
                calculation.InputParameters.LevellingCount,
                calculation.InputParameters.ProbabilityCollisionSecondaryStructure,
                calculation.InputParameters.FlowVelocityStructureClosable.Mean,
                calculation.InputParameters.FlowVelocityStructureClosable.CoefficientOfVariation,
                calculation.InputParameters.InsideWaterLevel.Mean,
                calculation.InputParameters.InsideWaterLevel.StandardDeviation,
                calculation.InputParameters.AllowedLevelIncreaseStorage.Mean,
                calculation.InputParameters.AllowedLevelIncreaseStorage.StandardDeviation,
                generalInput.ModelFactorStorageVolume.Mean,
                generalInput.ModelFactorStorageVolume.StandardDeviation,
                calculation.InputParameters.StorageStructureArea.Mean,
                calculation.InputParameters.StorageStructureArea.CoefficientOfVariation,
                generalInput.ModelFactorInflowVolume,
                calculation.InputParameters.FlowWidthAtBottomProtection.Mean,
                calculation.InputParameters.FlowWidthAtBottomProtection.StandardDeviation,
                calculation.InputParameters.CriticalOvertoppingDischarge.Mean,
                calculation.InputParameters.CriticalOvertoppingDischarge.CoefficientOfVariation,
                calculation.InputParameters.FailureProbabilityStructureWithErosion,
                calculation.InputParameters.StormDuration.Mean,
                calculation.InputParameters.StormDuration.CoefficientOfVariation,
                calculation.InputParameters.BankWidth.Mean,
                calculation.InputParameters.BankWidth.StandardDeviation,
                calculation.InputParameters.EvaluationLevel,
                generalInput.ModelFactorLoadEffect.Mean,
                generalInput.ModelFactorLoadEffect.StandardDeviation,
                generalInput.WaveRatioMaxHN,
                generalInput.WaveRatioMaxHStandardDeviation,
                calculation.InputParameters.VerticalDistance,
                generalInput.ModificationFactorWavesSlowlyVaryingPressureComponent,
                generalInput.ModificationFactorDynamicOrImpulsivePressureComponent,
                calculation.InputParameters.ModelFactorSuperCriticalFlow.Mean,
                calculation.InputParameters.ModelFactorSuperCriticalFlow.StandardDeviation,
                calculation.InputParameters.ConstructiveStrengthLinearLoadModel.Mean,
                calculation.InputParameters.ConstructiveStrengthLinearLoadModel.CoefficientOfVariation,
                calculation.InputParameters.StabilityLinearLoadModel.Mean,
                calculation.InputParameters.StabilityLinearLoadModel.CoefficientOfVariation,
                calculation.InputParameters.WidthFlowApertures.Mean,
                calculation.InputParameters.WidthFlowApertures.StandardDeviation);

            return structuresStabilityPointLowSillLinearCalculationInput;
        }

        private StructuresStabilityPointLowSillQuadraticCalculationInput CreateLowSillQuadraticCalculationInput(
            StructuresCalculation<StabilityPointStructuresInput> calculation,
            GeneralStabilityPointStructuresInput generalInput)
        {
            return new StructuresStabilityPointLowSillQuadraticCalculationInput(
                calculation.InputParameters.HydraulicBoundaryLocation.Id,
                calculation.InputParameters.StructureNormalOrientation,
                HydraRingInputParser.ParseForeshore(calculation.InputParameters),
                HydraRingInputParser.ParseBreakWater(calculation.InputParameters),
                calculation.InputParameters.VolumicWeightWater,
                generalInput.GravitationalAcceleration,
                calculation.InputParameters.LevelCrestStructure.Mean,
                calculation.InputParameters.LevelCrestStructure.StandardDeviation,
                calculation.InputParameters.StructureNormalOrientation,
                calculation.InputParameters.FactorStormDurationOpenStructure,
                generalInput.ModelFactorSubCriticalFlow.Mean,
                generalInput.ModelFactorSubCriticalFlow.CoefficientOfVariation,
                calculation.InputParameters.ThresholdHeightOpenWeir.Mean,
                calculation.InputParameters.ThresholdHeightOpenWeir.StandardDeviation,
                calculation.InputParameters.InsideWaterLevelFailureConstruction.Mean,
                calculation.InputParameters.InsideWaterLevelFailureConstruction.StandardDeviation,
                calculation.InputParameters.FailureProbabilityRepairClosure,
                calculation.InputParameters.FailureCollisionEnergy.Mean,
                calculation.InputParameters.FailureCollisionEnergy.CoefficientOfVariation,
                generalInput.ModelFactorCollisionLoad.Mean,
                generalInput.ModelFactorCollisionLoad.CoefficientOfVariation,
                calculation.InputParameters.ShipMass.Mean,
                calculation.InputParameters.ShipMass.CoefficientOfVariation,
                calculation.InputParameters.ShipVelocity.Mean,
                calculation.InputParameters.ShipVelocity.CoefficientOfVariation,
                calculation.InputParameters.LevellingCount,
                calculation.InputParameters.ProbabilityCollisionSecondaryStructure,
                calculation.InputParameters.FlowVelocityStructureClosable.Mean,
                calculation.InputParameters.FlowVelocityStructureClosable.CoefficientOfVariation,
                calculation.InputParameters.InsideWaterLevel.Mean,
                calculation.InputParameters.InsideWaterLevel.StandardDeviation,
                calculation.InputParameters.AllowedLevelIncreaseStorage.Mean,
                calculation.InputParameters.AllowedLevelIncreaseStorage.StandardDeviation,
                generalInput.ModelFactorStorageVolume.Mean,
                generalInput.ModelFactorStorageVolume.StandardDeviation,
                calculation.InputParameters.StorageStructureArea.Mean,
                calculation.InputParameters.StorageStructureArea.CoefficientOfVariation,
                generalInput.ModelFactorInflowVolume,
                calculation.InputParameters.FlowWidthAtBottomProtection.Mean,
                calculation.InputParameters.FlowWidthAtBottomProtection.StandardDeviation,
                calculation.InputParameters.CriticalOvertoppingDischarge.Mean,
                calculation.InputParameters.CriticalOvertoppingDischarge.CoefficientOfVariation,
                calculation.InputParameters.FailureProbabilityStructureWithErosion,
                calculation.InputParameters.StormDuration.Mean,
                calculation.InputParameters.StormDuration.CoefficientOfVariation,
                calculation.InputParameters.BankWidth.Mean,
                calculation.InputParameters.BankWidth.StandardDeviation,
                calculation.InputParameters.EvaluationLevel,
                generalInput.ModelFactorLoadEffect.Mean,
                generalInput.ModelFactorLoadEffect.StandardDeviation,
                generalInput.WaveRatioMaxHN,
                generalInput.WaveRatioMaxHStandardDeviation,
                calculation.InputParameters.VerticalDistance,
                generalInput.ModificationFactorWavesSlowlyVaryingPressureComponent,
                generalInput.ModificationFactorDynamicOrImpulsivePressureComponent,
                calculation.InputParameters.ModelFactorSuperCriticalFlow.Mean,
                calculation.InputParameters.ModelFactorSuperCriticalFlow.StandardDeviation,
                calculation.InputParameters.ConstructiveStrengthQuadraticLoadModel.Mean,
                calculation.InputParameters.ConstructiveStrengthQuadraticLoadModel.CoefficientOfVariation,
                calculation.InputParameters.StabilityQuadraticLoadModel.Mean,
                calculation.InputParameters.StabilityQuadraticLoadModel.CoefficientOfVariation,
                calculation.InputParameters.WidthFlowApertures.Mean,
                calculation.InputParameters.WidthFlowApertures.StandardDeviation);
        }

        private StructuresStabilityPointFloodedCulvertLinearCalculationInput CreateFloodedCulvertLinearCalculationInput(
            StructuresCalculation<StabilityPointStructuresInput> calculation,
            GeneralStabilityPointStructuresInput generalInput)
        {
            return new StructuresStabilityPointFloodedCulvertLinearCalculationInput(
                calculation.InputParameters.HydraulicBoundaryLocation.Id,
                calculation.InputParameters.StructureNormalOrientation,
                HydraRingInputParser.ParseForeshore(calculation.InputParameters),
                HydraRingInputParser.ParseBreakWater(calculation.InputParameters),
                calculation.InputParameters.VolumicWeightWater,
                generalInput.GravitationalAcceleration,
                calculation.InputParameters.LevelCrestStructure.Mean,
                calculation.InputParameters.LevelCrestStructure.StandardDeviation,
                calculation.InputParameters.StructureNormalOrientation,
                calculation.InputParameters.FactorStormDurationOpenStructure,
                generalInput.ModelFactorSubCriticalFlow.Mean,
                generalInput.ModelFactorSubCriticalFlow.CoefficientOfVariation,
                calculation.InputParameters.ThresholdHeightOpenWeir.Mean,
                calculation.InputParameters.ThresholdHeightOpenWeir.StandardDeviation,
                calculation.InputParameters.InsideWaterLevelFailureConstruction.Mean,
                calculation.InputParameters.InsideWaterLevelFailureConstruction.StandardDeviation,
                calculation.InputParameters.FailureProbabilityRepairClosure,
                calculation.InputParameters.FailureCollisionEnergy.Mean,
                calculation.InputParameters.FailureCollisionEnergy.CoefficientOfVariation,
                generalInput.ModelFactorCollisionLoad.Mean,
                generalInput.ModelFactorCollisionLoad.CoefficientOfVariation,
                calculation.InputParameters.ShipMass.Mean,
                calculation.InputParameters.ShipMass.CoefficientOfVariation,
                calculation.InputParameters.ShipVelocity.Mean,
                calculation.InputParameters.ShipVelocity.CoefficientOfVariation,
                calculation.InputParameters.LevellingCount,
                calculation.InputParameters.ProbabilityCollisionSecondaryStructure,
                calculation.InputParameters.FlowVelocityStructureClosable.Mean,
                calculation.InputParameters.FlowVelocityStructureClosable.CoefficientOfVariation,
                calculation.InputParameters.InsideWaterLevel.Mean,
                calculation.InputParameters.InsideWaterLevel.StandardDeviation,
                calculation.InputParameters.AllowedLevelIncreaseStorage.Mean,
                calculation.InputParameters.AllowedLevelIncreaseStorage.StandardDeviation,
                generalInput.ModelFactorStorageVolume.Mean,
                generalInput.ModelFactorStorageVolume.StandardDeviation,
                calculation.InputParameters.StorageStructureArea.Mean,
                calculation.InputParameters.StorageStructureArea.CoefficientOfVariation,
                generalInput.ModelFactorInflowVolume,
                calculation.InputParameters.FlowWidthAtBottomProtection.Mean,
                calculation.InputParameters.FlowWidthAtBottomProtection.StandardDeviation,
                calculation.InputParameters.CriticalOvertoppingDischarge.Mean,
                calculation.InputParameters.CriticalOvertoppingDischarge.CoefficientOfVariation,
                calculation.InputParameters.FailureProbabilityStructureWithErosion,
                calculation.InputParameters.StormDuration.Mean,
                calculation.InputParameters.StormDuration.CoefficientOfVariation,
                calculation.InputParameters.BankWidth.Mean,
                calculation.InputParameters.BankWidth.StandardDeviation,
                calculation.InputParameters.EvaluationLevel,
                generalInput.ModelFactorLoadEffect.Mean,
                generalInput.ModelFactorLoadEffect.StandardDeviation,
                generalInput.WaveRatioMaxHN,
                generalInput.WaveRatioMaxHStandardDeviation,
                calculation.InputParameters.VerticalDistance,
                generalInput.ModificationFactorWavesSlowlyVaryingPressureComponent,
                generalInput.ModificationFactorDynamicOrImpulsivePressureComponent,
                calculation.InputParameters.DrainCoefficient.Mean,
                calculation.InputParameters.DrainCoefficient.StandardDeviation,
                calculation.InputParameters.AreaFlowApertures.Mean,
                calculation.InputParameters.AreaFlowApertures.StandardDeviation,
                calculation.InputParameters.ConstructiveStrengthLinearLoadModel.Mean,
                calculation.InputParameters.ConstructiveStrengthLinearLoadModel.CoefficientOfVariation,
                calculation.InputParameters.StabilityLinearLoadModel.Mean,
                calculation.InputParameters.StabilityLinearLoadModel.CoefficientOfVariation);
        }

        private StructuresStabilityPointFloodedCulvertQuadraticCalculationInput CreateFloodedCulvertQuadraticCalculationInput(
            StructuresCalculation<StabilityPointStructuresInput> calculation,
            GeneralStabilityPointStructuresInput generalInput)
        {
            return new StructuresStabilityPointFloodedCulvertQuadraticCalculationInput(
                calculation.InputParameters.HydraulicBoundaryLocation.Id,
                calculation.InputParameters.StructureNormalOrientation,
                HydraRingInputParser.ParseForeshore(calculation.InputParameters),
                HydraRingInputParser.ParseBreakWater(calculation.InputParameters),
                calculation.InputParameters.VolumicWeightWater,
                generalInput.GravitationalAcceleration,
                calculation.InputParameters.LevelCrestStructure.Mean,
                calculation.InputParameters.LevelCrestStructure.StandardDeviation,
                calculation.InputParameters.StructureNormalOrientation,
                calculation.InputParameters.FactorStormDurationOpenStructure,
                generalInput.ModelFactorSubCriticalFlow.Mean,
                generalInput.ModelFactorSubCriticalFlow.CoefficientOfVariation,
                calculation.InputParameters.ThresholdHeightOpenWeir.Mean,
                calculation.InputParameters.ThresholdHeightOpenWeir.StandardDeviation,
                calculation.InputParameters.InsideWaterLevelFailureConstruction.Mean,
                calculation.InputParameters.InsideWaterLevelFailureConstruction.StandardDeviation,
                calculation.InputParameters.FailureProbabilityRepairClosure,
                calculation.InputParameters.FailureCollisionEnergy.Mean,
                calculation.InputParameters.FailureCollisionEnergy.CoefficientOfVariation,
                generalInput.ModelFactorCollisionLoad.Mean,
                generalInput.ModelFactorCollisionLoad.CoefficientOfVariation,
                calculation.InputParameters.ShipMass.Mean,
                calculation.InputParameters.ShipMass.CoefficientOfVariation,
                calculation.InputParameters.ShipVelocity.Mean,
                calculation.InputParameters.ShipVelocity.CoefficientOfVariation,
                calculation.InputParameters.LevellingCount,
                calculation.InputParameters.ProbabilityCollisionSecondaryStructure,
                calculation.InputParameters.FlowVelocityStructureClosable.Mean,
                calculation.InputParameters.FlowVelocityStructureClosable.CoefficientOfVariation,
                calculation.InputParameters.InsideWaterLevel.Mean,
                calculation.InputParameters.InsideWaterLevel.StandardDeviation,
                calculation.InputParameters.AllowedLevelIncreaseStorage.Mean,
                calculation.InputParameters.AllowedLevelIncreaseStorage.StandardDeviation,
                generalInput.ModelFactorStorageVolume.Mean,
                generalInput.ModelFactorStorageVolume.StandardDeviation,
                calculation.InputParameters.StorageStructureArea.Mean,
                calculation.InputParameters.StorageStructureArea.CoefficientOfVariation,
                generalInput.ModelFactorInflowVolume,
                calculation.InputParameters.FlowWidthAtBottomProtection.Mean,
                calculation.InputParameters.FlowWidthAtBottomProtection.StandardDeviation,
                calculation.InputParameters.CriticalOvertoppingDischarge.Mean,
                calculation.InputParameters.CriticalOvertoppingDischarge.CoefficientOfVariation,
                calculation.InputParameters.FailureProbabilityStructureWithErosion,
                calculation.InputParameters.StormDuration.Mean,
                calculation.InputParameters.StormDuration.CoefficientOfVariation,
                calculation.InputParameters.BankWidth.Mean,
                calculation.InputParameters.BankWidth.StandardDeviation,
                calculation.InputParameters.EvaluationLevel,
                generalInput.ModelFactorLoadEffect.Mean,
                generalInput.ModelFactorLoadEffect.StandardDeviation,
                generalInput.WaveRatioMaxHN,
                generalInput.WaveRatioMaxHStandardDeviation,
                calculation.InputParameters.VerticalDistance,
                generalInput.ModificationFactorWavesSlowlyVaryingPressureComponent,
                generalInput.ModificationFactorDynamicOrImpulsivePressureComponent,
                calculation.InputParameters.DrainCoefficient.Mean,
                calculation.InputParameters.DrainCoefficient.StandardDeviation,
                calculation.InputParameters.AreaFlowApertures.Mean,
                calculation.InputParameters.AreaFlowApertures.StandardDeviation,
                calculation.InputParameters.ConstructiveStrengthQuadraticLoadModel.Mean,
                calculation.InputParameters.ConstructiveStrengthQuadraticLoadModel.CoefficientOfVariation,
                calculation.InputParameters.StabilityQuadraticLoadModel.Mean,
                calculation.InputParameters.StabilityQuadraticLoadModel.CoefficientOfVariation);
        }
    }
}