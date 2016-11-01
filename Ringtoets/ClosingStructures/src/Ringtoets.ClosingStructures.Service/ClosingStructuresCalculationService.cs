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
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Service.Properties;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Service;
using Ringtoets.Common.Service.ValidationRules;
using Ringtoets.Common.Utils;
using Ringtoets.HydraRing.Calculation.Calculator;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.Structures;
using Ringtoets.HydraRing.Calculation.Parsers;
using Ringtoets.HydraRing.IO;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using ClosingStructuresFormsResources = Ringtoets.ClosingStructures.Forms.Properties.Resources;

namespace Ringtoets.ClosingStructures.Service
{
    /// <summary>
    /// Service that provides methods for performing Hydra-ring calculations for closing structures calculations.
    /// </summary>
    public class ClosingStructuresCalculationService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ClosingStructuresCalculationService));

        private IStructuresClosureCalculator calculator;
        private bool canceled;

        /// <summary>
        /// Performs a closing structures calculation based on the supplied <see cref="StructuresCalculation{T}"/> and sets 
        /// <see cref="StructuresCalculation{T}.Output"/> if the calculation was successful. Error and status information is 
        /// logged during the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="StructuresCalculation{T}"/> that holds all the information required to perform the calculation.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> that holds information about the norm used in the calculation.</param>
        /// <param name="failureMechanism">The <see cref="ClosingStructuresFailureMechanism"/> that holds the information about the contribution 
        /// and the general inputs used in the calculation.</param>
        /// <param name="hlcdFilePath">The filepath of the HLCD file that should be used for performing the calculation.</param>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="ClosingStructuresInput.InflowModelType"/> is an invalid
        /// <see cref="ClosingStructureInflowModelType"/>.</exception>
        public void Calculate(StructuresCalculation<ClosingStructuresInput> calculation,
                              IAssessmentSection assessmentSection,
                              ClosingStructuresFailureMechanism failureMechanism,
                              string hlcdFilePath)
        {
            var calculationName = calculation.Name;

            FailureMechanismSection failureMechanismSection = StructuresHelper.FailureMechanismSectionForCalculation(failureMechanism.Sections,
                                                                                                                     calculation);

            StructuresClosureCalculationInput input = CreateStructuresClosureCalculationInput(calculation,
                                                                                              failureMechanism,
                                                                                              failureMechanismSection);

            string hlcdDirectory = Path.GetDirectoryName(hlcdFilePath);
            calculator = HydraRingCalculatorFactory.Instance.CreateStructuresClosureCalculator(hlcdDirectory, assessmentSection.Id);

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
                    log.ErrorFormat(Resources.ClosingStructuresCalculationService_Calculate_Error_in_closing_structures_0_calculation, calculationName);
                    throw;
                }
            }
            finally
            {
                log.InfoFormat(Resources.ClosingStructuresCalculationService_Calculate_Calculation_temporary_directory_can_be_found_on_location_0, calculator.OutputDirectory);
                CalculationServiceHelper.LogCalculationEndTime(calculationName);
            }
        }

        /// <summary>
        /// Cancels any ongoing structures closure calculation.
        /// </summary>
        public void Cancel()
        {
            if (calculator != null)
            {
                calculator.Cancel();
            }

            canceled = true;
        }

        /// <summary>
        /// Performs validation over the values on the given <paramref name="calculation"/>. Error and status information is logged during
        /// the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="StructuresCalculation{T}"/> for which to validate the values.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> for which to validate the values.</param>
        /// <returns><c>true</c>c> if <paramref name="calculation"/> has no validation errors; <c>false</c>c> otherwise.</returns>
        public static bool Validate(StructuresCalculation<ClosingStructuresInput> calculation, IAssessmentSection assessmentSection)
        {
            CalculationServiceHelper.LogValidationBeginTime(calculation.Name);
            var messages = ValidateInput(calculation.InputParameters, assessmentSection);
            CalculationServiceHelper.LogMessagesAsError(RingtoetsCommonServiceResources.Error_in_validation_0, messages);
            CalculationServiceHelper.LogValidationEndTime(calculation.Name);

            return !messages.Any();
        }

        private static StructuresClosureCalculationInput CreateStructuresClosureCalculationInput(StructuresCalculation<ClosingStructuresInput> calculation,
                                                                                                 ClosingStructuresFailureMechanism failureMechanism,
                                                                                                 FailureMechanismSection failureMechanismSection)
        {
            StructuresClosureCalculationInput input;
            switch (calculation.InputParameters.InflowModelType)
            {
                case ClosingStructureInflowModelType.VerticalWall:
                    input = CreateClosureVerticalWallCalculationInput(calculation, failureMechanismSection, failureMechanism.GeneralInput);
                    break;
                case ClosingStructureInflowModelType.LowSill:
                    input = CreateLowSillCalculationInput(calculation, failureMechanismSection, failureMechanism.GeneralInput);
                    break;
                case ClosingStructureInflowModelType.FloodedCulvert:
                    input = CreateFloodedCulvertCalculationInput(calculation, failureMechanismSection, failureMechanism.GeneralInput);
                    break;
                default:
                    throw new InvalidEnumArgumentException("calculation",
                                                           (int) calculation.InputParameters.InflowModelType,
                                                           typeof(ClosingStructureInflowModelType));
            }
            return input;
        }

        private static StructuresClosureVerticalWallCalculationInput CreateClosureVerticalWallCalculationInput(StructuresCalculation<ClosingStructuresInput> calculation,
                                                                                                               FailureMechanismSection failureMechanismSection,
                                                                                                               GeneralClosingStructuresInput generalInput)
        {
            return new StructuresClosureVerticalWallCalculationInput(
                calculation.InputParameters.HydraulicBoundaryLocation.Id,
                new HydraRingSection(1, failureMechanismSection.GetSectionLength(), calculation.InputParameters.StructureNormalOrientation),
                HydraRingInputParser.ParseForeshore(calculation.InputParameters),
                HydraRingInputParser.ParseBreakWater(calculation.InputParameters),
                generalInput.GravitationalAcceleration,
                calculation.InputParameters.FactorStormDurationOpenStructure,
                calculation.InputParameters.FailureProbabilityOpenStructure,
                calculation.InputParameters.FailureProbabilityReparation,
                calculation.InputParameters.IdenticalApertures,
                calculation.InputParameters.AllowedLevelIncreaseStorage.Mean, calculation.InputParameters.AllowedLevelIncreaseStorage.StandardDeviation,
                generalInput.ModelFactorStorageVolume.Mean, generalInput.ModelFactorStorageVolume.StandardDeviation,
                calculation.InputParameters.StorageStructureArea.Mean, calculation.InputParameters.StorageStructureArea.CoefficientOfVariation,
                generalInput.ModelFactorInflowVolume,
                calculation.InputParameters.FlowWidthAtBottomProtection.Mean, calculation.InputParameters.FlowWidthAtBottomProtection.StandardDeviation,
                calculation.InputParameters.CriticalOvertoppingDischarge.Mean, calculation.InputParameters.CriticalOvertoppingDischarge.CoefficientOfVariation,
                calculation.InputParameters.FailureProbabilityStructureWithErosion,
                calculation.InputParameters.StormDuration.Mean, calculation.InputParameters.StormDuration.CoefficientOfVariation,
                calculation.InputParameters.ProbabilityOpenStructureBeforeFlooding,
                generalInput.ModelFactorOvertoppingFlow.Mean, generalInput.ModelFactorOvertoppingFlow.StandardDeviation,
                calculation.InputParameters.StructureNormalOrientation,
                calculation.InputParameters.ModelFactorSuperCriticalFlow.Mean, calculation.InputParameters.ModelFactorSuperCriticalFlow.StandardDeviation,
                calculation.InputParameters.LevelCrestStructureNotClosing.Mean, calculation.InputParameters.LevelCrestStructureNotClosing.StandardDeviation,
                calculation.InputParameters.WidthFlowApertures.Mean, calculation.InputParameters.WidthFlowApertures.CoefficientOfVariation,
                calculation.InputParameters.DeviationWaveDirection);
        }

        private static StructuresClosureLowSillCalculationInput CreateLowSillCalculationInput(StructuresCalculation<ClosingStructuresInput> calculation,
                                                                                              FailureMechanismSection failureMechanismSection,
                                                                                              GeneralClosingStructuresInput generalInput)
        {
            return new StructuresClosureLowSillCalculationInput(
                calculation.InputParameters.HydraulicBoundaryLocation.Id,
                new HydraRingSection(1, failureMechanismSection.GetSectionLength(), calculation.InputParameters.StructureNormalOrientation),
                HydraRingInputParser.ParseForeshore(calculation.InputParameters),
                HydraRingInputParser.ParseBreakWater(calculation.InputParameters),
                generalInput.GravitationalAcceleration,
                calculation.InputParameters.FactorStormDurationOpenStructure,
                calculation.InputParameters.FailureProbabilityOpenStructure,
                calculation.InputParameters.FailureProbabilityReparation,
                calculation.InputParameters.IdenticalApertures,
                calculation.InputParameters.AllowedLevelIncreaseStorage.Mean, calculation.InputParameters.AllowedLevelIncreaseStorage.StandardDeviation,
                generalInput.ModelFactorStorageVolume.Mean, generalInput.ModelFactorStorageVolume.StandardDeviation,
                calculation.InputParameters.StorageStructureArea.Mean, calculation.InputParameters.StorageStructureArea.CoefficientOfVariation,
                generalInput.ModelFactorInflowVolume,
                calculation.InputParameters.FlowWidthAtBottomProtection.Mean, calculation.InputParameters.FlowWidthAtBottomProtection.StandardDeviation,
                calculation.InputParameters.CriticalOvertoppingDischarge.Mean, calculation.InputParameters.CriticalOvertoppingDischarge.CoefficientOfVariation,
                calculation.InputParameters.FailureProbabilityStructureWithErosion,
                calculation.InputParameters.StormDuration.Mean, calculation.InputParameters.StormDuration.CoefficientOfVariation,
                calculation.InputParameters.ProbabilityOpenStructureBeforeFlooding,
                calculation.InputParameters.ModelFactorSuperCriticalFlow.Mean, calculation.InputParameters.ModelFactorSuperCriticalFlow.StandardDeviation,
                generalInput.ModelFactorSubCriticalFlow.Mean, generalInput.ModelFactorSubCriticalFlow.CoefficientOfVariation,
                calculation.InputParameters.ThresholdHeightOpenWeir.Mean, calculation.InputParameters.ThresholdHeightOpenWeir.StandardDeviation,
                calculation.InputParameters.InsideWaterLevel.Mean, calculation.InputParameters.InsideWaterLevel.StandardDeviation,
                calculation.InputParameters.WidthFlowApertures.Mean, calculation.InputParameters.WidthFlowApertures.CoefficientOfVariation);
        }

        private static StructuresClosureFloodedCulvertCalculationInput CreateFloodedCulvertCalculationInput(StructuresCalculation<ClosingStructuresInput> calculation,
                                                                                                            FailureMechanismSection failureMechanismSection,
                                                                                                            GeneralClosingStructuresInput generalInput)
        {
            return new StructuresClosureFloodedCulvertCalculationInput(
                calculation.InputParameters.HydraulicBoundaryLocation.Id,
                new HydraRingSection(1, failureMechanismSection.GetSectionLength(), calculation.InputParameters.StructureNormalOrientation),
                HydraRingInputParser.ParseForeshore(calculation.InputParameters),
                HydraRingInputParser.ParseBreakWater(calculation.InputParameters),
                generalInput.GravitationalAcceleration,
                calculation.InputParameters.FactorStormDurationOpenStructure,
                calculation.InputParameters.FailureProbabilityOpenStructure,
                calculation.InputParameters.FailureProbabilityReparation,
                calculation.InputParameters.IdenticalApertures,
                calculation.InputParameters.AllowedLevelIncreaseStorage.Mean, calculation.InputParameters.AllowedLevelIncreaseStorage.StandardDeviation,
                generalInput.ModelFactorStorageVolume.Mean, generalInput.ModelFactorStorageVolume.StandardDeviation,
                calculation.InputParameters.StorageStructureArea.Mean, calculation.InputParameters.StorageStructureArea.CoefficientOfVariation,
                generalInput.ModelFactorInflowVolume,
                calculation.InputParameters.FlowWidthAtBottomProtection.Mean, calculation.InputParameters.FlowWidthAtBottomProtection.StandardDeviation,
                calculation.InputParameters.CriticalOvertoppingDischarge.Mean, calculation.InputParameters.CriticalOvertoppingDischarge.CoefficientOfVariation,
                calculation.InputParameters.FailureProbabilityStructureWithErosion,
                calculation.InputParameters.StormDuration.Mean, calculation.InputParameters.StormDuration.CoefficientOfVariation,
                calculation.InputParameters.ProbabilityOpenStructureBeforeFlooding,
                calculation.InputParameters.DrainCoefficient.Mean, calculation.InputParameters.DrainCoefficient.StandardDeviation,
                calculation.InputParameters.AreaFlowApertures.Mean, calculation.InputParameters.AreaFlowApertures.StandardDeviation,
                calculation.InputParameters.InsideWaterLevel.Mean, calculation.InputParameters.InsideWaterLevel.StandardDeviation);
        }

        private static string[] ValidateInput(ClosingStructuresInput inputParameters, IAssessmentSection assessmentSection)
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
                    case ClosingStructureInflowModelType.VerticalWall:
                        validationRules = GetVerticalWallValidationRules(inputParameters);
                        break;
                    case ClosingStructureInflowModelType.LowSill:
                        validationRules = GetLowSillValidationRules(inputParameters);
                        break;
                    case ClosingStructureInflowModelType.FloodedCulvert:
                        validationRules = GetFloodedCulvertValidationRules(inputParameters);
                        break;
                    default:
                        throw new InvalidEnumArgumentException("inputParameters",
                                                               (int) inputParameters.InflowModelType,
                                                               typeof(ClosingStructureInflowModelType));
                }

                foreach (var validationRule in validationRules)
                {
                    validationResults.AddRange(validationRule.Validate());
                }
            }

            return validationResults.ToArray();
        }

        private static IEnumerable<ValidationRule> GetVerticalWallValidationRules(ClosingStructuresInput input)
        {
            var validationRules = new List<ValidationRule>
            {
                new UseBreakWaterRule(input),
                new VariationCoefficientLogNormalDistributionRule(input.StormDuration,
                                                                  ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_StormDuration_DisplayName)),
                new NumericInputRule(input.DeviationWaveDirection,
                                     ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_DeviationWaveDirection_DisplayName)),
                new NormalDistributionRule(input.ModelFactorSuperCriticalFlow,
                                           ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_ModelFactorSuperCriticalFlow_DisplayName)),
                new NumericInputRule(input.FactorStormDurationOpenStructure,
                                     ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_FactorStormDurationOpenStructure_DisplayName)),
                new VariationCoefficientNormalDistributionRule(input.WidthFlowApertures,
                                                               ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_WidthFlowApertures_DisplayName)),
                new NumericInputRule(input.StructureNormalOrientation,
                                     ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_StructureNormalOrientation_DisplayName)),
                new LogNormalDistributionRule(input.FlowWidthAtBottomProtection,
                                              ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_FlowWidthAtBottomProtection_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(input.StorageStructureArea,
                                                                  ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_StorageStructureArea_DisplayName)),
                new LogNormalDistributionRule(input.AllowedLevelIncreaseStorage,
                                              ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_AllowedLevelIncreaseStorage_DisplayName)),
                new NormalDistributionRule(input.LevelCrestStructureNotClosing,
                                           ParameterNameExtractor.GetFromDisplayName(ClosingStructuresFormsResources.LevelCrestStructureNotClosing_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(input.CriticalOvertoppingDischarge,
                                                                  ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_CriticalOvertoppingDischarge_DisplayName))
            };

            return validationRules;
        }

        private static IEnumerable<ValidationRule> GetLowSillValidationRules(ClosingStructuresInput input)
        {
            var validationRules = new List<ValidationRule>
            {
                new UseBreakWaterRule(input),
                new VariationCoefficientLogNormalDistributionRule(input.StormDuration,
                                                                  ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_StormDuration_DisplayName)),
                new NormalDistributionRule(input.InsideWaterLevel,
                                           ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_InsideWaterLevel_DisplayName)),
                new NormalDistributionRule(input.ModelFactorSuperCriticalFlow,
                                           ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_ModelFactorSuperCriticalFlow_DisplayName)),
                new NumericInputRule(input.FactorStormDurationOpenStructure,
                                     ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_FactorStormDurationOpenStructure_DisplayName)),
                new VariationCoefficientNormalDistributionRule(input.WidthFlowApertures,
                                                               ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_WidthFlowApertures_DisplayName)),
                new LogNormalDistributionRule(input.FlowWidthAtBottomProtection,
                                              ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_FlowWidthAtBottomProtection_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(input.StorageStructureArea,
                                                                  ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_StorageStructureArea_DisplayName)),
                new LogNormalDistributionRule(input.AllowedLevelIncreaseStorage,
                                              ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_AllowedLevelIncreaseStorage_DisplayName)),
                new NormalDistributionRule(input.ThresholdHeightOpenWeir,
                                           ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_ThresholdHeightOpenWeir_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(input.CriticalOvertoppingDischarge,
                                                                  ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_CriticalOvertoppingDischarge_DisplayName))
            };

            return validationRules;
        }

        private static IEnumerable<ValidationRule> GetFloodedCulvertValidationRules(ClosingStructuresInput input)
        {
            var validationRules = new List<ValidationRule>
            {
                new UseBreakWaterRule(input),
                new VariationCoefficientLogNormalDistributionRule(input.StormDuration,
                                                                  ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_StormDuration_DisplayName)),
                new NormalDistributionRule(input.InsideWaterLevel,
                                           ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_InsideWaterLevel_DisplayName)),
                new NormalDistributionRule(input.DrainCoefficient,
                                           ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_DrainCoefficient_DisplayName)),
                new NumericInputRule(input.FactorStormDurationOpenStructure,
                                     ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_FactorStormDurationOpenStructure_DisplayName)),
                new LogNormalDistributionRule(input.AreaFlowApertures,
                                              ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_AreaFlowApertures_DisplayName)),
                new LogNormalDistributionRule(input.FlowWidthAtBottomProtection,
                                              ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_FlowWidthAtBottomProtection_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(input.StorageStructureArea,
                                                                  ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_StorageStructureArea_DisplayName)),
                new LogNormalDistributionRule(input.AllowedLevelIncreaseStorage,
                                              ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_AllowedLevelIncreaseStorage_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(input.CriticalOvertoppingDischarge,
                                                                  ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_CriticalOvertoppingDischarge_DisplayName))
            };

            return validationRules;
        }
    }
}