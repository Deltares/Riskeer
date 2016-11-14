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
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Service.Properties;
using Ringtoets.HydraRing.Calculation.Calculator;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.Structures;
using Ringtoets.HydraRing.Calculation.Parsers;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.HeightStructures.Service
{
    /// <summary>
    /// Service that provides methods for performing Hydra-Ring calculations for height structures calculations.
    /// </summary>
    public class HeightStructuresCalculationService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(HeightStructuresCalculationService));

        private IStructuresOvertoppingCalculator calculator;
        private bool canceled;

        /// <summary>
        /// Performs validation over the values on the given <paramref name="calculation"/>. Error and status information is logged during
        /// the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="StructuresCalculation{T}"/> for which to validate the values.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> for which to validate the values.</param>
        /// <returns><c>True</c> if <paramref name="calculation"/> has no validation errors; <c>False</c> otherwise.</returns>
        public static bool Validate(StructuresCalculation<HeightStructuresInput> calculation, IAssessmentSection assessmentSection)
        {
            CalculationServiceHelper.LogValidationBeginTime(calculation.Name);

            string[] messages = ValidateInput(calculation.InputParameters, assessmentSection);
            CalculationServiceHelper.LogMessagesAsError(RingtoetsCommonServiceResources.Error_in_validation_0, messages);

            CalculationServiceHelper.LogValidationEndTime(calculation.Name);

            return !messages.Any();
        }

        /// <summary>
        /// Cancels any currently running height structures calculation.
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
        /// Performs a height structures calculation based on the supplied <see cref="StructuresCalculation{T}"/> and sets <see cref="StructuresCalculation{T}.Output"/>
        /// if the calculation was successful. Error and status information is logged during the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="StructuresCalculation{T}"/> that holds all the information required to perform the calculation.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> that holds information about the norm used in the calculation.</param>
        /// <param name="failureMechanism"> The <see cref="HeightStructuresFailureMechanism"/> that holds the information about the contribution 
        /// and the general inputs used in the calculation.</param>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The path which points to the hydraulic boundary database file.</param>
        internal void Calculate(StructuresCalculation<HeightStructuresInput> calculation,
                                IAssessmentSection assessmentSection,
                                HeightStructuresFailureMechanism failureMechanism,
                                string hydraulicBoundaryDatabaseFilePath)
        {
            var calculationName = calculation.Name;

            FailureMechanismSection failureMechanismSection = StructuresHelper.FailureMechanismSectionForCalculation(failureMechanism.Sections,
                                                                                                                     calculation);

            StructuresOvertoppingCalculationInput input = CreateInput(calculation, failureMechanismSection, failureMechanism.GeneralInput, hydraulicBoundaryDatabaseFilePath);

            string hlcdDirectory = Path.GetDirectoryName(hydraulicBoundaryDatabaseFilePath);
            calculator = HydraRingCalculatorFactory.Instance.CreateStructuresOvertoppingCalculator(hlcdDirectory, assessmentSection.Id);

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
                    var lastErrorContent = calculator.LastErrorFileContent;
                    if (string.IsNullOrEmpty(lastErrorContent))
                    {
                        log.ErrorFormat(Resources.HeightStructuresCalculationService_Calculate_Unexplained_error_in_height_structures_0_calculation,
                                        calculationName);
                    }
                    else
                    {
                        log.ErrorFormat(Resources.HeightStructuresCalculationService_Calculate_Error_in_height_structures_0_calculation_click_details_for_last_error_1,
                                        calculationName, lastErrorContent);
                    }

                    exceptionThrown = true;
                    throw;
                }
            }
            finally
            {
                var lastErrorFileContent = calculator.LastErrorFileContent;
                bool errorOccurred = ErrorOccurred(exceptionThrown, lastErrorFileContent);
                if (errorOccurred)
                {
                    log.ErrorFormat(Resources.HeightStructuresCalculationService_Calculate_Error_in_height_structures_0_calculation_click_details_for_last_error_1,
                                    calculationName, lastErrorFileContent);
                }

                log.InfoFormat(Resources.HeightStructuresCalculationService_Calculate_Calculation_temporary_directory_can_be_found_on_location_0, calculator.OutputDirectory);
                CalculationServiceHelper.LogCalculationEndTime(calculationName);

                if (errorOccurred)
                {
                    throw new HydraRingFileParserException(lastErrorFileContent);
                }
            }
        }

        private bool ErrorOccurred(bool exceptionThrown, string lastErrorFileContent)
        {
            return !canceled && !exceptionThrown && !string.IsNullOrEmpty(lastErrorFileContent);
        }

        private static StructuresOvertoppingCalculationInput CreateInput(
            StructuresCalculation<HeightStructuresInput> calculation,
            FailureMechanismSection failureMechanismSection,
            GeneralHeightStructuresInput generalInput,
            string hydraulicBoundaryDatabaseFilePath)
        {
            var structuresOvertoppingCalculationInput = new StructuresOvertoppingCalculationInput(
                calculation.InputParameters.HydraulicBoundaryLocation.Id,
                new HydraRingSection(1, failureMechanismSection.GetSectionLength(), calculation.InputParameters.StructureNormalOrientation),
                HydraRingInputParser.ParseForeshore(calculation.InputParameters),
                HydraRingInputParser.ParseBreakWater(calculation.InputParameters),
                generalInput.GravitationalAcceleration,
                generalInput.ModelFactorOvertoppingFlow.Mean, generalInput.ModelFactorOvertoppingFlow.StandardDeviation,
                calculation.InputParameters.LevelCrestStructure.Mean, calculation.InputParameters.LevelCrestStructure.StandardDeviation,
                calculation.InputParameters.StructureNormalOrientation,
                calculation.InputParameters.ModelFactorSuperCriticalFlow.Mean, calculation.InputParameters.ModelFactorSuperCriticalFlow.StandardDeviation,
                calculation.InputParameters.AllowedLevelIncreaseStorage.Mean, calculation.InputParameters.AllowedLevelIncreaseStorage.StandardDeviation,
                generalInput.ModelFactorStorageVolume.Mean, generalInput.ModelFactorStorageVolume.StandardDeviation,
                calculation.InputParameters.StorageStructureArea.Mean, calculation.InputParameters.StorageStructureArea.CoefficientOfVariation,
                generalInput.ModelFactorInflowVolume,
                calculation.InputParameters.FlowWidthAtBottomProtection.Mean, calculation.InputParameters.FlowWidthAtBottomProtection.StandardDeviation,
                calculation.InputParameters.CriticalOvertoppingDischarge.Mean, calculation.InputParameters.CriticalOvertoppingDischarge.CoefficientOfVariation,
                calculation.InputParameters.FailureProbabilityStructureWithErosion,
                calculation.InputParameters.WidthFlowApertures.Mean, calculation.InputParameters.WidthFlowApertures.CoefficientOfVariation,
                calculation.InputParameters.DeviationWaveDirection,
                calculation.InputParameters.StormDuration.Mean, calculation.InputParameters.StormDuration.CoefficientOfVariation);

            HydraRingSettingsDatabaseHelper.AssignSettingsFromDatabase(structuresOvertoppingCalculationInput, hydraulicBoundaryDatabaseFilePath);

            return structuresOvertoppingCalculationInput;
        }

        private static string[] ValidateInput(HeightStructuresInput inputParameters, IAssessmentSection assessmentSection)
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
                IEnumerable<ValidationRule> inputValidationRules = GetInputValidationRules(inputParameters);
                foreach (var validationRule in inputValidationRules)
                {
                    validationResults.AddRange(validationRule.Validate());
                }
            }

            return validationResults.ToArray();
        }

        private static ValidationRule[] GetInputValidationRules(HeightStructuresInput input)
        {
            var validationRules = new ValidationRule[]
            {
                new UseBreakWaterRule(input),
                new VariationCoefficientLogNormalDistributionRule(input.StormDuration,
                                                                  ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_StormDuration_DisplayName)),
                new NormalDistributionRule(input.ModelFactorSuperCriticalFlow,
                                           ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_ModelFactorSuperCriticalFlow_DisplayName)),
                new NumericInputRule(input.StructureNormalOrientation,
                                     ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Orientation_DisplayName)),
                new LogNormalDistributionRule(input.FlowWidthAtBottomProtection,
                                              ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_FlowWidthAtBottomProtection_DisplayName)),
                new VariationCoefficientNormalDistributionRule(input.WidthFlowApertures,
                                                               ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_WidthFlowApertures_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(input.StorageStructureArea,
                                                                  ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_StorageStructureArea_DisplayName)),
                new LogNormalDistributionRule(input.AllowedLevelIncreaseStorage,
                                              ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_AllowedLevelIncreaseStorage_DisplayName)),
                new NormalDistributionRule(input.LevelCrestStructure,
                                           ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_LevelCrestStructure_DisplayName)),
                new VariationCoefficientLogNormalDistributionRule(input.CriticalOvertoppingDischarge,
                                                                  ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Structure_CriticalOvertoppingDischarge_DisplayName)),
            };

            return validationRules;
        }
    }
}