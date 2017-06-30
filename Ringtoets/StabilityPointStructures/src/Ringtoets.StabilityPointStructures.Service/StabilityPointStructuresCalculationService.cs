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
using System.IO;
using System.Linq;
using Core.Common.Base.IO;
using log4net;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.IO.HydraRing;
using Ringtoets.Common.Service;
using Ringtoets.Common.Service.ValidationRules;
using Ringtoets.HydraRing.Calculation.Calculator;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.Data.Input.Structures;
using Ringtoets.HydraRing.Calculation.Exceptions;
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
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static bool Validate(StructuresCalculation<StabilityPointStructuresInput> calculation, IAssessmentSection assessmentSection)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            CalculationServiceHelper.LogValidationBegin();
            string[] messages = ValidateInput(calculation.InputParameters, assessmentSection);
            CalculationServiceHelper.LogMessagesAsError(RingtoetsCommonServiceResources.Error_in_validation_0, messages);
            CalculationServiceHelper.LogValidationEnd();

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
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/>, <paramref name="assessmentSection"/>
        /// or <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="hydraulicBoundaryDatabaseFilePath"/> 
        /// contains invalid characters or the given <see cref="HydraRingCalculationInput"/> is invalid.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>No settings database file could be found at the location of <paramref name="hydraulicBoundaryDatabaseFilePath"/>
        /// with the same name.</item>
        /// <item>Unable to open settings database file.</item>
        /// <item>Unable to read required data from database file.</item>
        /// </list></exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item><see cref="StabilityPointStructuresInput.InflowModelType"/> is an invalid <see cref="StabilityPointStructureInflowModelType"/>.</item>
        /// <item><see cref="StabilityPointStructuresInput.LoadSchematizationType"/> is an invalid <see cref="LoadSchematizationType"/>.</item>
        /// </list></exception>
        /// <exception cref="HydraRingCalculationException">Thrown when an error occurs while performing the calculation.</exception>
        public void Calculate(StructuresCalculation<StabilityPointStructuresInput> calculation,
                              IAssessmentSection assessmentSection,
                              StabilityPointStructuresFailureMechanism failureMechanism,
                              string hydraulicBoundaryDatabaseFilePath)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            string calculationName = calculation.Name;

            StructuresStabilityPointCalculationInput input = CreateStructuresStabilityPointCalculationInput(calculation,
                                                                                                            failureMechanism,
                                                                                                            hydraulicBoundaryDatabaseFilePath);

            string hlcdDirectory = Path.GetDirectoryName(hydraulicBoundaryDatabaseFilePath);
            calculator = HydraRingCalculatorFactory.Instance.CreateStructuresStabilityPointCalculator(hlcdDirectory);

            CalculationServiceHelper.LogCalculationBegin();

            var exceptionThrown = false;
            try
            {
                calculator.Calculate(input);

                if (!canceled && string.IsNullOrEmpty(calculator.LastErrorFileContent))
                {
                    calculation.Output = ProbabilityAssessmentService.Calculate(assessmentSection.FailureMechanismContribution.Norm,
                                                                                failureMechanism.Contribution,
                                                                                failureMechanism.GeneralInput.N,
                                                                                calculator.ExceedanceProbabilityBeta);
                }
            }
            catch (HydraRingCalculationException)
            {
                if (!canceled)
                {
                    string lastErrorContent = calculator.LastErrorFileContent;
                    if (string.IsNullOrEmpty(lastErrorContent))
                    {
                        log.ErrorFormat(Resources.StabilityPointStructuresCalculationService_Calculate_Error_in_StabilityPointStructuresCalculation_0_no_error_report,
                                        calculationName);
                    }
                    else
                    {
                        log.ErrorFormat(Resources.StabilityPointStructuresCalculationService_Calculate_Error_in_StabilityPointStructuresCalculation_0_click_details_for_last_error_report_1,
                                        calculationName, lastErrorContent);
                    }

                    exceptionThrown = true;
                    throw;
                }
            }
            finally
            {
                string lastErrorFileContent = calculator.LastErrorFileContent;
                bool errorOccurred = CalculationServiceHelper.HasErrorOccurred(canceled, exceptionThrown, lastErrorFileContent);
                if (errorOccurred)
                {
                    log.ErrorFormat(Resources.StabilityPointStructuresCalculationService_Calculate_Error_in_StabilityPointStructuresCalculation_0_click_details_for_last_error_report_1,
                                    calculationName, lastErrorFileContent);
                }

                log.InfoFormat(Resources.StabilityPointStructuresCalculationService_CalculateCalculation_temporary_directory_can_be_found_on_location_0, calculator.OutputDirectory);
                CalculationServiceHelper.LogCalculationEnd();

                if (errorOccurred)
                {
                    throw new HydraRingCalculationException(lastErrorFileContent);
                }
            }
        }

        /// <summary>
        /// Cancels any ongoing structures stability point calculation.
        /// </summary>
        public void Cancel()
        {
            calculator?.Cancel();
            canceled = true;
        }

        /// <summary>
        /// Creates the input for a structures stability point calculation.
        /// </summary>
        /// <param name="calculation">The calculation to create the input for.</param>
        /// <param name="failureMechanism">The failure mechanism that contains input to use in the calculation.</param>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The path to the hydraulic boundary database file.</param>
        /// <returns>A <see cref="StructuresStabilityPointCalculationInput"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="hydraulicBoundaryDatabaseFilePath"/> 
        /// contains invalid characters.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>No settings database file could be found at the location of <paramref name="hydraulicBoundaryDatabaseFilePath"/>
        /// with the same name.</item>
        /// <item>Unable to open settings database file.</item>
        /// <item>Unable to read required data from database file.</item>
        /// </list></exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item><see cref="StabilityPointStructuresInput.InflowModelType"/> is an invalid <see cref="StabilityPointStructureInflowModelType"/>.</item>
        /// <item><see cref="StabilityPointStructuresInput.LoadSchematizationType"/> is an invalid <see cref="LoadSchematizationType"/>.</item>
        /// </list></exception>
        private StructuresStabilityPointCalculationInput CreateStructuresStabilityPointCalculationInput(
            StructuresCalculation<StabilityPointStructuresInput> calculation,
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

        private static string[] ValidateInput(StabilityPointStructuresInput inputParameters, IAssessmentSection assessmentSection)
        {
            var validationResults = new List<string>();

            string validationProblem = HydraulicDatabaseHelper.ValidatePathForCalculation(assessmentSection.HydraulicBoundaryDatabase.FilePath);
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
                        throw new InvalidEnumArgumentException(nameof(inputParameters),
                                                               (int) inputParameters.InflowModelType,
                                                               typeof(StabilityPointStructureInflowModelType));
                }

                foreach (ValidationRule validationRule in validationRules)
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
                new VariationCoefficientNormalDistributionRule(input.FlowVelocityStructureClosable,
                                                               ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_FlowVelocityStructureClosable_DisplayName)),
                new NormalDistributionRule(input.ModelFactorSuperCriticalFlow,
                                           ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_ModelFactorSuperCriticalFlow_DisplayName)),
                new NumericInputRule(input.FactorStormDurationOpenStructure,
                                     ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_FactorStormDurationOpenStructure_DisplayName)),
                new NumericInputRule(input.StructureNormalOrientation,
                                     ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_StructureNormalOrientation_DisplayName)),
                new NormalDistributionRule(input.WidthFlowApertures,
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
                                                                  ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_StabilityLinearLoadModel_DisplayName))
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
                new VariationCoefficientNormalDistributionRule(input.FlowVelocityStructureClosable,
                                                               ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_FlowVelocityStructureClosable_DisplayName)),
                new NormalDistributionRule(input.ModelFactorSuperCriticalFlow,
                                           ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_ModelFactorSuperCriticalFlow_DisplayName)),
                new NumericInputRule(input.FactorStormDurationOpenStructure,
                                     ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_FactorStormDurationOpenStructure_DisplayName)),
                new NumericInputRule(input.StructureNormalOrientation,
                                     ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_StructureNormalOrientation_DisplayName)),
                new NormalDistributionRule(input.WidthFlowApertures,
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
                new VariationCoefficientNormalDistributionRule(input.FlowVelocityStructureClosable,
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
                                                                  ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_StabilityLinearLoadModel_DisplayName))
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
                new VariationCoefficientNormalDistributionRule(input.FlowVelocityStructureClosable,
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
                                                                  ParameterNameExtractor.GetFromDisplayName(RingtoetsStabilityPointStructuresFormsResources.Structure_StabilityQuadraticLoadModel_DisplayName))
            };
        }
    }
}