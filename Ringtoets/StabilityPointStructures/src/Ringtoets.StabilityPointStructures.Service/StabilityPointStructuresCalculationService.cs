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
using Ringtoets.Common.IO.HydraRing;
using Ringtoets.Common.Service;
using Ringtoets.Common.Service.ValidationRules;
using Ringtoets.Common.Utils;
using Ringtoets.HydraRing.Calculation.Calculator;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.Structures;
using Ringtoets.HydraRing.Calculation.Parsers;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Service.Properties;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsStabilityPointStructuresFormsResources = Ringtoets.StabilityPointStructures.Forms.Properties.Resources;

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
        /// <returns><c>true</c> if <paramref name="calculation"/> has no validation errors; <c>false</c> otherwise.</returns>
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
        /// <param name="hydraulicBoundaryDatabaseFilePath">The path which points to the hydraulic boundary database file.</param>
        /// <exception cref="InvalidEnumArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item><see cref="StabilityPointStructuresInput.InflowModelType"/> is an invalid <see cref="StabilityPointStructureInflowModelType"/>.</item>
        /// <item><see cref="StabilityPointStructuresInput.LoadSchematizationType"/> is an invalid <see cref="LoadSchematizationType"/>.</item>
        /// </list>
        /// </exception>
        public void Calculate(StructuresCalculation<StabilityPointStructuresInput> calculation,
                              IAssessmentSection assessmentSection,
                              StabilityPointStructuresFailureMechanism failureMechanism,
                              string hydraulicBoundaryDatabaseFilePath)
        {
            var calculationName = calculation.Name;

            FailureMechanismSection failureMechanismSection = StructuresHelper.FailureMechanismSectionForCalculation(failureMechanism.Sections,
                                                                                                                     calculation);

            StructuresStabilityPointCalculationInput input = CreateStructuresStabilityPointCalculationInput(calculation,
                                                                                                            failureMechanism,
                                                                                                            failureMechanismSection,
                                                                                                            hydraulicBoundaryDatabaseFilePath);

            string hlcdDirectory = Path.GetDirectoryName(hydraulicBoundaryDatabaseFilePath);
            calculator = HydraRingCalculatorFactory.Instance.CreateStructuresStabilityPointCalculator(hlcdDirectory, assessmentSection.Id);

            CalculationServiceHelper.LogCalculationBeginTime(calculationName);

            var exceptionThrown = false;
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
                    var lastErrorContent = calculator.LastErrorContent;
                    if (string.IsNullOrEmpty(lastErrorContent))
                    {
                        log.ErrorFormat(Resources.StabilityPointStructuresCalculationService_Calculate_Unexplained_error_in_stabilityPoint_structures_0_calculation,
                                        calculationName);
                    }
                    else
                    {
                        log.ErrorFormat(Resources.StabilityPointStructuresCalculationService_Calculate_Error_in_stabilityPoint_structures_0_calculation_click_details_for_last_error_1,
                                        calculationName, lastErrorContent);
                    }

                    exceptionThrown = true;
                    throw;
                }
            }
            finally
            {
                try
                {
                    var lastErrorContent = calculator.LastErrorContent;
                    if (!exceptionThrown && !string.IsNullOrEmpty(lastErrorContent))
                    {
                        log.ErrorFormat(Resources.StabilityPointStructuresCalculationService_Calculate_Error_in_stabilityPoint_structures_0_calculation_click_details_for_last_error_1,
                                        calculationName, lastErrorContent);

                        throw new HydraRingFileParserException(lastErrorContent);
                    }
                }
                finally
                {
                    log.InfoFormat(Resources.StabilityPointStructuresCalculationService_CalculateCalculation_temporary_directory_can_be_found_on_location_0, calculator.OutputDirectory);
                    CalculationServiceHelper.LogCalculationEndTime(calculationName);
                }
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

        private StructuresStabilityPointCalculationInput CreateStructuresStabilityPointCalculationInput(
            StructuresCalculation<StabilityPointStructuresInput> calculation,
            StabilityPointStructuresFailureMechanism failureMechanism,
            FailureMechanismSection failureMechanismSection,
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
                                failureMechanismSection, 
                                failureMechanism.GeneralInput,
                                hydraulicBoundaryDatabaseFilePath);
                            break;
                        case LoadSchematizationType.Quadratic:
                            input = CreateLowSillQuadraticCalculationInput(
                                calculation, 
                                failureMechanismSection, 
                                failureMechanism.GeneralInput, 
                                hydraulicBoundaryDatabaseFilePath);
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
                            input = CreateFloodedCulvertLinearCalculationInput(
                                calculation, 
                                failureMechanismSection, 
                                failureMechanism.GeneralInput,
                                hydraulicBoundaryDatabaseFilePath);
                            break;
                        case LoadSchematizationType.Quadratic:
                            input = CreateFloodedCulvertQuadraticCalculationInput(
                                calculation, 
                                failureMechanismSection, 
                                failureMechanism.GeneralInput, 
                                hydraulicBoundaryDatabaseFilePath);
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
                                                                                                          GeneralStabilityPointStructuresInput generalInput,
            string hydraulicBoundaryDatabaseFilePath)
        {
            var structuresStabilityPointLowSillLinearCalculationInput = new StructuresStabilityPointLowSillLinearCalculationInput(
                calculation.InputParameters.HydraulicBoundaryLocation.Id,
                new HydraRingSection(1, failureMechanismSection.GetSectionLength(), calculation.InputParameters.StructureNormalOrientation),
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

            HydraRingSettingsDatabaseHelper.AssignSettingsFromDatabase(structuresStabilityPointLowSillLinearCalculationInput, hydraulicBoundaryDatabaseFilePath);

            return structuresStabilityPointLowSillLinearCalculationInput;
        }

        private StructuresStabilityPointLowSillQuadraticCalculationInput CreateLowSillQuadraticCalculationInput(
            StructuresCalculation<StabilityPointStructuresInput> calculation,
            FailureMechanismSection failureMechanismSection,
            GeneralStabilityPointStructuresInput generalInput,
            string hydraulicBoundaryDatabaseFilePath)
        {
            var structuresStabilityPointLowSillQuadraticCalculationInput = new StructuresStabilityPointLowSillQuadraticCalculationInput(
                calculation.InputParameters.HydraulicBoundaryLocation.Id,
                new HydraRingSection(1, failureMechanismSection.GetSectionLength(), calculation.InputParameters.StructureNormalOrientation),
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

            HydraRingSettingsDatabaseHelper.AssignSettingsFromDatabase(structuresStabilityPointLowSillQuadraticCalculationInput, hydraulicBoundaryDatabaseFilePath);

            return structuresStabilityPointLowSillQuadraticCalculationInput;
        }

        private StructuresStabilityPointFloodedCulvertLinearCalculationInput CreateFloodedCulvertLinearCalculationInput(
            StructuresCalculation<StabilityPointStructuresInput> calculation, 
            FailureMechanismSection failureMechanismSection, 
            GeneralStabilityPointStructuresInput generalInput, 
            string hydraulicBoundaryDatabaseFilePath)
        {
            var structuresStabilityPointFloodedCulvertLinearCalculationInput = new StructuresStabilityPointFloodedCulvertLinearCalculationInput(
                calculation.InputParameters.HydraulicBoundaryLocation.Id,
                new HydraRingSection(1, failureMechanismSection.GetSectionLength(), calculation.InputParameters.StructureNormalOrientation),
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

            HydraRingSettingsDatabaseHelper.AssignSettingsFromDatabase(structuresStabilityPointFloodedCulvertLinearCalculationInput, hydraulicBoundaryDatabaseFilePath);

            return structuresStabilityPointFloodedCulvertLinearCalculationInput;
        }

        private StructuresStabilityPointFloodedCulvertQuadraticCalculationInput CreateFloodedCulvertQuadraticCalculationInput(
            StructuresCalculation<StabilityPointStructuresInput> calculation,
            FailureMechanismSection failureMechanismSection,
            GeneralStabilityPointStructuresInput generalInput,
            string hydraulicBoundaryDatabaseFilePath)
        {
            var structuresStabilityPointFloodedCulvertQuadraticCalculationInput = new StructuresStabilityPointFloodedCulvertQuadraticCalculationInput(
                calculation.InputParameters.HydraulicBoundaryLocation.Id,
                new HydraRingSection(1, failureMechanismSection.GetSectionLength(), calculation.InputParameters.StructureNormalOrientation),
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

            HydraRingSettingsDatabaseHelper.AssignSettingsFromDatabase(structuresStabilityPointFloodedCulvertQuadraticCalculationInput, hydraulicBoundaryDatabaseFilePath); 

            return structuresStabilityPointFloodedCulvertQuadraticCalculationInput;
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

            if (inputParameters.Structure == null)
            {
                validationResults.Add(RingtoetsCommonServiceResources.StructuresCalculationService_ValidateInput_No_Structure_selected);
            }
            else
            {
                IEnumerable<ValidationRule> validationRules;
                switch (inputParameters.InflowModelType)
                {
                    case StabilityPointStructureInflowModelType.LowSill:
                        switch (inputParameters.LoadSchematizationType)
                        {
                            case LoadSchematizationType.Linear:
                                validationRules = GetLowSillLinearValidationRules(inputParameters);
                                break;
                            case LoadSchematizationType.Quadratic:
                                validationRules = GetLowSillQuadraticValidationRules(inputParameters);
                                break;
                            default:
                                validationResults.Add(Resources.StabilityPointStructuresCalculationService_ValidateInput_No_LoadSchematizationType_selected);
                                validationRules = Enumerable.Empty<ValidationRule>();
                                break;
                        }
                        break;
                    case StabilityPointStructureInflowModelType.FloodedCulvert:
                        switch (inputParameters.LoadSchematizationType)
                        {
                            case LoadSchematizationType.Linear:
                                validationRules = GetFloodedCulvertLinearValidationRules(inputParameters);
                                break;
                            case LoadSchematizationType.Quadratic:
                                validationRules = GetFloodedCulvertQuadraticValidationRules(inputParameters);
                                break;
                            default:
                                validationResults.Add(Resources.StabilityPointStructuresCalculationService_ValidateInput_No_LoadSchematizationType_selected);
                                validationRules = Enumerable.Empty<ValidationRule>();
                                break;
                        }
                        break;
                    default:
                        throw new InvalidEnumArgumentException("inputParameters",
                                                               (int) inputParameters.InflowModelType,
                                                               typeof(StabilityPointStructureInflowModelType));
                }

                foreach (var validationRule in validationRules)
                {
                    validationResults.AddRange(validationRule.Validate());
                }
            }
            return validationResults.ToArray();
        }

        private static ValidationRule[] GetLowSillLinearValidationRules(StabilityPointStructuresInput input)
        {
            return new ValidationRule[]
            {
                new UseBreakWaterRule(input),
                new NumericInputRule(input.VolumicWeightWater,
                                     ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_VolumicWeightWater_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(input.StormDuration,
                                                                  ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_StormDuration_DisplayName)),
                new NormalDistributionRule(input.InsideWaterLevel,
                                           ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_InsideWaterLevel_DisplayName)),
                new NormalDistributionRule(input.InsideWaterLevelFailureConstruction,
                                           ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_InsideWaterLevelFailureConstruction_DisplayName)),
                new NormalDistributionRule(input.FlowVelocityStructureClosable,
                                           ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_FlowVelocityStructureClosable_DisplayName)),
                new NormalDistributionRule(input.ModelFactorSuperCriticalFlow,
                                           ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_ModelFactorSuperCriticalFlow_DisplayName)),
                new NumericInputRule(input.FactorStormDurationOpenStructure,
                                     ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_FactorStormDurationOpenStructure_DisplayName)),
                new NumericInputRule(input.StructureNormalOrientation,
                                     ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_StructureNormalOrientation_DisplayName)),
                new VariationCoefficientNormalDistributionRule(input.WidthFlowApertures,
                                                               ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_WidthFlowApertures_DisplayName)),
                new LogNormalDistributionRule(input.FlowWidthAtBottomProtection,
                                              ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_FlowWidthAtBottomProtection_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(input.StorageStructureArea,
                                                                  ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_StorageStructureArea_DisplayName)),
                new LogNormalDistributionRule(input.AllowedLevelIncreaseStorage,
                                              ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_AllowedLevelIncreaseStorage_DisplayName)),
                new NormalDistributionRule(input.LevelCrestStructure,
                                           ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_LevelCrestStructure_DisplayName)),
                new NormalDistributionRule(input.ThresholdHeightOpenWeir,
                                           ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_ThresholdHeightOpenWeir_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(input.CriticalOvertoppingDischarge,
                                                                  ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_CriticalOvertoppingDischarge_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(input.ConstructiveStrengthLinearLoadModel,
                                                                  ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_ConstructiveStrengthLinearLoadModel_DisplayName)),
                new NormalDistributionRule(input.BankWidth,
                                           ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_BankWidth_DisplayName)),
                new NumericInputRule(input.EvaluationLevel,
                                     ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_EvaluationLevel_DisplayName)),
                new NumericInputRule(input.VerticalDistance,
                                     ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_VerticalDistance_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(input.FailureCollisionEnergy,
                                                                  ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_FailureCollisionEnergy_DisplayName)),
                new VariationCoefficientNormalDistributionRule(input.ShipMass,
                                                               ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_ShipMass_DisplayName)),
                new VariationCoefficientNormalDistributionRule(input.ShipVelocity,
                                                               ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_ShipVelocity_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(input.StabilityLinearLoadModel,
                                                                  ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_StabilityLinearLoadModel_DisplayName)),
            };
        }

        private static ValidationRule[] GetLowSillQuadraticValidationRules(StabilityPointStructuresInput input)
        {
            return new ValidationRule[]
            {
                new UseBreakWaterRule(input),
                new NumericInputRule(input.VolumicWeightWater,
                                     ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_VolumicWeightWater_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(input.StormDuration,
                                                                  ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_StormDuration_DisplayName)),
                new NormalDistributionRule(input.InsideWaterLevel,
                                           ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_InsideWaterLevel_DisplayName)),
                new NormalDistributionRule(input.InsideWaterLevelFailureConstruction,
                                           ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_InsideWaterLevelFailureConstruction_DisplayName)),
                new NormalDistributionRule(input.FlowVelocityStructureClosable,
                                           ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_FlowVelocityStructureClosable_DisplayName)),
                new NormalDistributionRule(input.ModelFactorSuperCriticalFlow,
                                           ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_ModelFactorSuperCriticalFlow_DisplayName)),
                new NumericInputRule(input.FactorStormDurationOpenStructure,
                                     ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_FactorStormDurationOpenStructure_DisplayName)),
                new NumericInputRule(input.StructureNormalOrientation,
                                     ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_StructureNormalOrientation_DisplayName)),
                new VariationCoefficientNormalDistributionRule(input.WidthFlowApertures,
                                                               ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_WidthFlowApertures_DisplayName)),
                new LogNormalDistributionRule(input.FlowWidthAtBottomProtection,
                                              ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_FlowWidthAtBottomProtection_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(input.StorageStructureArea,
                                                                  ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_StorageStructureArea_DisplayName)),
                new LogNormalDistributionRule(input.AllowedLevelIncreaseStorage,
                                              ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_AllowedLevelIncreaseStorage_DisplayName)),
                new NormalDistributionRule(input.LevelCrestStructure,
                                           ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_LevelCrestStructure_DisplayName)),
                new NormalDistributionRule(input.ThresholdHeightOpenWeir,
                                           ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_ThresholdHeightOpenWeir_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(input.CriticalOvertoppingDischarge,
                                                                  ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_CriticalOvertoppingDischarge_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(input.ConstructiveStrengthQuadraticLoadModel,
                                                                  ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_ConstructiveStrengthQuadraticLoadModel_DisplayName)),
                new NormalDistributionRule(input.BankWidth,
                                           ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_BankWidth_DisplayName)),
                new NumericInputRule(input.EvaluationLevel,
                                     ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_EvaluationLevel_DisplayName)),
                new NumericInputRule(input.VerticalDistance,
                                     ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_VerticalDistance_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(input.FailureCollisionEnergy,
                                                                  ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_FailureCollisionEnergy_DisplayName)),
                new VariationCoefficientNormalDistributionRule(input.ShipMass,
                                                               ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_ShipMass_DisplayName)),
                new VariationCoefficientNormalDistributionRule(input.ShipVelocity,
                                                               ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_ShipVelocity_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(input.StabilityQuadraticLoadModel,
                                                                  ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_StabilityQuadraticLoadModel_DisplayName))
            };
        }

        private static ValidationRule[] GetFloodedCulvertLinearValidationRules(StabilityPointStructuresInput input)
        {
            return new ValidationRule[]
            {
                new UseBreakWaterRule(input),
                new NumericInputRule(input.VolumicWeightWater,
                                     ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_VolumicWeightWater_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(input.StormDuration,
                                                                  ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_StormDuration_DisplayName)),
                new NormalDistributionRule(input.InsideWaterLevel,
                                           ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_InsideWaterLevel_DisplayName)),
                new NormalDistributionRule(input.InsideWaterLevelFailureConstruction,
                                           ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_InsideWaterLevelFailureConstruction_DisplayName)),
                new NormalDistributionRule(input.FlowVelocityStructureClosable,
                                           ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_FlowVelocityStructureClosable_DisplayName)),
                new NormalDistributionRule(input.DrainCoefficient,
                                           ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_DrainCoefficient_DisplayName)),
                new NumericInputRule(input.FactorStormDurationOpenStructure,
                                     ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_FactorStormDurationOpenStructure_DisplayName)),
                new NumericInputRule(input.StructureNormalOrientation,
                                     ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_StructureNormalOrientation_DisplayName)),
                new LogNormalDistributionRule(input.AreaFlowApertures,
                                              ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_AreaFlowApertures_DisplayName)),
                new LogNormalDistributionRule(input.FlowWidthAtBottomProtection,
                                              ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_FlowWidthAtBottomProtection_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(input.StorageStructureArea,
                                                                  ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_StorageStructureArea_DisplayName)),
                new LogNormalDistributionRule(input.AllowedLevelIncreaseStorage,
                                              ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_AllowedLevelIncreaseStorage_DisplayName)),
                new NormalDistributionRule(input.LevelCrestStructure,
                                           ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_LevelCrestStructure_DisplayName)),
                new NormalDistributionRule(input.ThresholdHeightOpenWeir,
                                           ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_ThresholdHeightOpenWeir_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(input.CriticalOvertoppingDischarge,
                                                                  ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_CriticalOvertoppingDischarge_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(input.ConstructiveStrengthLinearLoadModel,
                                                                  ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_ConstructiveStrengthLinearLoadModel_DisplayName)),
                new NormalDistributionRule(input.BankWidth,
                                           ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_BankWidth_DisplayName)),
                new NumericInputRule(input.EvaluationLevel,
                                     ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_EvaluationLevel_DisplayName)),
                new NumericInputRule(input.VerticalDistance,
                                     ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_VerticalDistance_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(input.FailureCollisionEnergy,
                                                                  ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_FailureCollisionEnergy_DisplayName)),
                new VariationCoefficientNormalDistributionRule(input.ShipMass,
                                                               ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_ShipMass_DisplayName)),
                new VariationCoefficientNormalDistributionRule(input.ShipVelocity,
                                                               ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_ShipVelocity_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(input.StabilityLinearLoadModel,
                                                                  ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_StabilityLinearLoadModel_DisplayName)),
            };
        }

        private static ValidationRule[] GetFloodedCulvertQuadraticValidationRules(StabilityPointStructuresInput input)
        {
            return new ValidationRule[]
            {
                new UseBreakWaterRule(input),
                new NumericInputRule(input.VolumicWeightWater,
                                     ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_VolumicWeightWater_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(input.StormDuration,
                                                                  ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_StormDuration_DisplayName)),
                new NormalDistributionRule(input.InsideWaterLevel,
                                           ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_InsideWaterLevel_DisplayName)),
                new NormalDistributionRule(input.InsideWaterLevelFailureConstruction,
                                           ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_InsideWaterLevelFailureConstruction_DisplayName)),
                new NormalDistributionRule(input.FlowVelocityStructureClosable,
                                           ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_FlowVelocityStructureClosable_DisplayName)),
                new NormalDistributionRule(input.DrainCoefficient,
                                           ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_DrainCoefficient_DisplayName)),
                new NumericInputRule(input.FactorStormDurationOpenStructure,
                                     ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_FactorStormDurationOpenStructure_DisplayName)),
                new NumericInputRule(input.StructureNormalOrientation,
                                     ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_StructureNormalOrientation_DisplayName)),
                new LogNormalDistributionRule(input.AreaFlowApertures,
                                              ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_AreaFlowApertures_DisplayName)),
                new LogNormalDistributionRule(input.FlowWidthAtBottomProtection,
                                              ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_FlowWidthAtBottomProtection_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(input.StorageStructureArea,
                                                                  ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_StorageStructureArea_DisplayName)),
                new LogNormalDistributionRule(input.AllowedLevelIncreaseStorage,
                                              ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_AllowedLevelIncreaseStorage_DisplayName)),
                new NormalDistributionRule(input.LevelCrestStructure,
                                           ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_LevelCrestStructure_DisplayName)),
                new NormalDistributionRule(input.ThresholdHeightOpenWeir,
                                           ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_ThresholdHeightOpenWeir_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(input.CriticalOvertoppingDischarge,
                                                                  ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_CriticalOvertoppingDischarge_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(input.ConstructiveStrengthQuadraticLoadModel,
                                                                  ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_ConstructiveStrengthQuadraticLoadModel_DisplayName)),
                new NormalDistributionRule(input.BankWidth,
                                           ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_BankWidth_DisplayName)),
                new NumericInputRule(input.EvaluationLevel,
                                     ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_EvaluationLevel_DisplayName)),
                new NumericInputRule(input.VerticalDistance,
                                     ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_VerticalDistance_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(input.FailureCollisionEnergy,
                                                                  ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_FailureCollisionEnergy_DisplayName)),
                new VariationCoefficientNormalDistributionRule(input.ShipMass,
                                                               ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_ShipMass_DisplayName)),
                new VariationCoefficientNormalDistributionRule(input.ShipVelocity,
                                                               ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_ShipVelocity_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(input.StabilityQuadraticLoadModel,
                                                                  ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_StabilityQuadraticLoadModel_DisplayName)),
            };
        }
    }
}