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
using System.ComponentModel;
using System.IO;
using System.Linq;
using log4net;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Service;
using Ringtoets.Common.Utils;
using Ringtoets.HydraRing.Calculation.Calculator;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.Structures;
using Ringtoets.HydraRing.Calculation.Parsers;
using Ringtoets.HydraRing.IO;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Service.Properties;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;

namespace Ringtoets.StabilityPointStructures.Service
{
    /// <summary>
    /// Service that provides methods for performing Hydra-ring calculations for stability point structures calculations.
    /// </summary>
    public class StabilityPointStructuresCalculationService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(StabilityPointStructuresCalculationService));

        private bool canceled;
        private IStructuresStabilityPointCalculator calculator;

        /// <summary>
        /// Performs validation over the values on the given <paramref name="calculation"/>. Error and status information is logged during
        /// the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="StructuresCalculation{T}"/> for which to validate the values.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> for which to validate the values.</param>
        /// <returns><c>true</c>c> if <paramref name="calculation"/> has no validation errors; <c>false</c>c> otherwise.</returns>
        public static bool Validate(StructuresCalculation<StabilityPointStructuresInput> calculation, IAssessmentSection assessmentSection)
        {
            CalculationServiceHelper.LogValidationBeginTime(calculation.Name);
            var messages = ValidateInput(calculation.InputParameters, assessmentSection);
            CalculationServiceHelper.LogMessagesAsError(RingtoetsCommonServiceResources.Error_in_validation_0, messages);
            CalculationServiceHelper.LogValidationEndTime(calculation.Name);

            return !messages.Any();
        }

        /// <summary>
        /// Performs a stability point structures calculation based on the supplied <see cref="StructuresCalculation{T}"/> and sets 
        /// <see cref="StructuresCalculation{T}.Output"/> if the calculation was successful. Error and status information is 
        /// logged during the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="StructuresCalculation{T}"/> that holds all the information required to perform the calculation.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> that holds information about the norm used in the calculation.</param>
        /// <param name="failureMechanism">The <see cref="StabilityPointStructuresFailureMechanism"/> that holds the information about the contribution 
        /// and the general inputs used in the calculation.</param>
        /// <param name="hlcdFilePath">The filepath of the HLCD file that should be used for performing the calculation.</param>
        /// <exception cref="InvalidEnumArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item><see cref="StabilityPointStructuresInput.InflowModelType"/> is an invalid <see cref="StabilityPointStructureInflowModelType"/>.</item>
        /// <item><see cref="StabilityPointStructuresInput.LoadSchematizationType"/> is an invalid <see cref="LoadSchematizationType"/>.</item>
        /// </list>
        /// </exception>
        public void Calculate(StructuresCalculation<StabilityPointStructuresInput> calculation,
                              IAssessmentSection assessmentSection,
                              StabilityPointStructuresFailureMechanism failureMechanism,
                              string hlcdFilePath)
        {
            var calculationName = calculation.Name;

            FailureMechanismSection failureMechanismSection = StructuresHelper.FailureMechanismSectionForCalculation(failureMechanism.Sections,
                                                                                                                     calculation);

            StructuresStabilityPointCalculationInput input = CreateStructuresStabilityPointCalculationInput(calculation,
                                                                                                            failureMechanism,
                                                                                                            failureMechanismSection);

            string hlcdDirectory = Path.GetDirectoryName(hlcdFilePath);
            calculator = HydraRingCalculatorFactory.Instance.CreateStructuresStabilityPointCalculator(hlcdDirectory, assessmentSection.Id);

            CalculationServiceHelper.LogCalculationBeginTime(calculationName);

            try
            {
                calculator.Calculate(input);

                if (!canceled)
                {
                    calculation.Output = ProbabilityAssessmentService.Calculate(assessmentSection.FailureMechanismContribution.Norm,
                                                                                failureMechanism.Contribution,
                                                                                failureMechanism.GeneralInput.N,
                                                                                calculator.ExceedanceProbabilityBeta);
                }
            }
            catch (HydraRingFileParserException)
            {
                if (!canceled)
                {
                    log.ErrorFormat(Resources.StabilityPointStructuresCalculationService_Calculate_Error_in_stabilityPoint_structures_0_calculation, calculationName);
                    throw;
                }
            }
            finally
            {
                log.InfoFormat(Resources.StabilityPointStructuresCalculationService_Calculate_Calculation_report_Click_details_for_full_report_0, calculator.OutputFileContent);
                CalculationServiceHelper.LogCalculationEndTime(calculationName);
            }
        }

        /// <summary>
        /// Cancels any ongoing structures stability point calculation.
        /// </summary>
        public void Cancel()
        {
            if (calculator != null)
            {
                calculator.Cancel();
            }

            canceled = true;
        }

        private StructuresStabilityPointCalculationInput CreateStructuresStabilityPointCalculationInput(StructuresCalculation<StabilityPointStructuresInput> calculation,
                                                                                                        StabilityPointStructuresFailureMechanism failureMechanism,
                                                                                                        FailureMechanismSection failureMechanismSection)
        {
            StructuresStabilityPointCalculationInput input;
            switch (calculation.InputParameters.InflowModelType)
            {
                case StabilityPointStructureInflowModelType.LowSill:
                    switch (calculation.InputParameters.LoadSchematizationType)
                    {
                        case LoadSchematizationType.Linear:
                            input = CreateLowSillLinearCalculationInput(calculation, failureMechanismSection, failureMechanism.GeneralInput);
                            break;
                        case LoadSchematizationType.Quadratic:
                            input = CreateLowSillQuadraticCalculationInput(calculation, failureMechanismSection, failureMechanism.GeneralInput);
                            break;
                        default:
                            throw new InvalidEnumArgumentException("calculation",
                                                                   (int) calculation.InputParameters.LoadSchematizationType,
                                                                   typeof(LoadSchematizationType));
                    }
                    break;
                case StabilityPointStructureInflowModelType.FloodedCulvert:
                    switch (calculation.InputParameters.LoadSchematizationType)
                    {
                        case LoadSchematizationType.Linear:
                            input = CreateFloodedCulvertLinearCalculationInput(calculation, failureMechanismSection, failureMechanism.GeneralInput);
                            break;
                        case LoadSchematizationType.Quadratic:
                            input = CreateFloodedCulvertQuadraticCalculationInput(calculation, failureMechanismSection, failureMechanism.GeneralInput);
                            break;
                        default:
                            throw new InvalidEnumArgumentException("calculation",
                                                                   (int) calculation.InputParameters.LoadSchematizationType,
                                                                   typeof(LoadSchematizationType));
                    }
                    break;
                default:
                    throw new InvalidEnumArgumentException("calculation",
                                                           (int) calculation.InputParameters.InflowModelType,
                                                           typeof(StabilityPointStructureInflowModelType));
            }

            return input;
        }

        private StructuresStabilityPointLowSillLinearCalculationInput CreateLowSillLinearCalculationInput(StructuresCalculation<StabilityPointStructuresInput> calculation,
                                                                                                          FailureMechanismSection failureMechanismSection,
                                                                                                          GeneralStabilityPointStructuresInput generalInput)
        {
            return new StructuresStabilityPointLowSillLinearCalculationInput(calculation.InputParameters.HydraulicBoundaryLocation.Id,
                                                                             new HydraRingSection(1, failureMechanismSection.GetSectionLength(), calculation.InputParameters.StructureNormalOrientation),
                                                                             HydraRingInputParser.ParseForeshore(calculation.InputParameters),
                                                                             HydraRingInputParser.ParseBreakWater(calculation.InputParameters),
                                                                             calculation.InputParameters.VolumicWeightWater,
                                                                             generalInput.GravitationalAcceleration,
                                                                             calculation.InputParameters.LevelCrestStructure.Mean,
                                                                             calculation.InputParameters.LevelCrestStructure.StandardDeviation,
                                                                             calculation.InputParameters.StructureNormalOrientation,
                                                                             calculation.InputParameters.FactorStormDurationOpenStructure,
                                                                             calculation.InputParameters.ModelFactorSuperCriticalFlow.Mean,
                                                                             calculation.InputParameters.ModelFactorSuperCriticalFlow.StandardDeviation,
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
                                                                             calculation.InputParameters.FlowVelocityStructureClosable.StandardDeviation,
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
                                                                             calculation.InputParameters.WidthFlowApertures.CoefficientOfVariation);
        }

        private StructuresStabilityPointLowSillQuadraticCalculationInput CreateLowSillQuadraticCalculationInput(StructuresCalculation<StabilityPointStructuresInput> calculation,
                                                                                                                FailureMechanismSection failureMechanismSection,
                                                                                                                GeneralStabilityPointStructuresInput generalInput)
        {
            return new StructuresStabilityPointLowSillQuadraticCalculationInput(calculation.InputParameters.HydraulicBoundaryLocation.Id,
                                                                             new HydraRingSection(1, failureMechanismSection.GetSectionLength(), calculation.InputParameters.StructureNormalOrientation),
                                                                             HydraRingInputParser.ParseForeshore(calculation.InputParameters),
                                                                             HydraRingInputParser.ParseBreakWater(calculation.InputParameters),
                                                                             calculation.InputParameters.VolumicWeightWater,
                                                                             generalInput.GravitationalAcceleration,
                                                                             calculation.InputParameters.LevelCrestStructure.Mean,
                                                                             calculation.InputParameters.LevelCrestStructure.StandardDeviation,
                                                                             calculation.InputParameters.StructureNormalOrientation,
                                                                             calculation.InputParameters.FactorStormDurationOpenStructure,
                                                                             calculation.InputParameters.ModelFactorSuperCriticalFlow.Mean,
                                                                             calculation.InputParameters.ModelFactorSuperCriticalFlow.StandardDeviation,
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
                                                                             calculation.InputParameters.FlowVelocityStructureClosable.StandardDeviation,
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
                                                                             calculation.InputParameters.WidthFlowApertures.CoefficientOfVariation);
        }

        private StructuresStabilityPointFloodedCulvertLinearCalculationInput CreateFloodedCulvertLinearCalculationInput(StructuresCalculation<StabilityPointStructuresInput> calculation,
                                                                                                                        FailureMechanismSection failureMechanismSection,
                                                                                                                        GeneralStabilityPointStructuresInput generalInput)
        {
            return new StructuresStabilityPointFloodedCulvertLinearCalculationInput(calculation.InputParameters.HydraulicBoundaryLocation.Id,
                                                                             new HydraRingSection(1, failureMechanismSection.GetSectionLength(), calculation.InputParameters.StructureNormalOrientation),
                                                                             HydraRingInputParser.ParseForeshore(calculation.InputParameters),
                                                                             HydraRingInputParser.ParseBreakWater(calculation.InputParameters),
                                                                             calculation.InputParameters.VolumicWeightWater,
                                                                             generalInput.GravitationalAcceleration,
                                                                             calculation.InputParameters.LevelCrestStructure.Mean,
                                                                             calculation.InputParameters.LevelCrestStructure.StandardDeviation,
                                                                             calculation.InputParameters.StructureNormalOrientation,
                                                                             calculation.InputParameters.FactorStormDurationOpenStructure,
                                                                             calculation.InputParameters.ModelFactorSuperCriticalFlow.Mean,
                                                                             calculation.InputParameters.ModelFactorSuperCriticalFlow.StandardDeviation,
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
                                                                             calculation.InputParameters.FlowVelocityStructureClosable.StandardDeviation,
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

        private StructuresStabilityPointFloodedCulvertQuadraticCalculationInput CreateFloodedCulvertQuadraticCalculationInput(StructuresCalculation<StabilityPointStructuresInput> calculation,
                                                                                                                              FailureMechanismSection failureMechanismSection,
                                                                                                                              GeneralStabilityPointStructuresInput generalInput)
        {
            return new StructuresStabilityPointFloodedCulvertQuadraticCalculationInput(calculation.InputParameters.HydraulicBoundaryLocation.Id,
                                                                             new HydraRingSection(1, failureMechanismSection.GetSectionLength(), calculation.InputParameters.StructureNormalOrientation),
                                                                             HydraRingInputParser.ParseForeshore(calculation.InputParameters),
                                                                             HydraRingInputParser.ParseBreakWater(calculation.InputParameters),
                                                                             calculation.InputParameters.VolumicWeightWater,
                                                                             generalInput.GravitationalAcceleration,
                                                                             calculation.InputParameters.LevelCrestStructure.Mean,
                                                                             calculation.InputParameters.LevelCrestStructure.StandardDeviation,
                                                                             calculation.InputParameters.StructureNormalOrientation,
                                                                             calculation.InputParameters.FactorStormDurationOpenStructure,
                                                                             calculation.InputParameters.ModelFactorSuperCriticalFlow.Mean,
                                                                             calculation.InputParameters.ModelFactorSuperCriticalFlow.StandardDeviation,
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
                                                                             calculation.InputParameters.FlowVelocityStructureClosable.StandardDeviation,
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

        private static string[] ValidateInput(StabilityPointStructuresInput inputParameters, IAssessmentSection assessmentSection)
        {
            var validationResults = new List<string>();

            var validationProblem = HydraulicDatabaseHelper.ValidatePathForCalculation(assessmentSection.HydraulicBoundaryDatabase.FilePath);
            if (!string.IsNullOrEmpty(validationProblem))
            {
                validationResults.Add(validationProblem);
                return validationResults.ToArray();
            }

            if (inputParameters.HydraulicBoundaryLocation == null)
            {
                validationResults.Add(RingtoetsCommonServiceResources.CalculationService_ValidateInput_No_hydraulic_boundary_location_selected);
            }

            return validationResults.ToArray();
        }
    }
}