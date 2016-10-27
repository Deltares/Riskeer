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
using Core.Common.Base.Data;
using log4net;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Service;
using Ringtoets.Common.Utils;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Service.Properties;
using Ringtoets.HydraRing.Calculation.Calculator;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.Structures;
using Ringtoets.HydraRing.Calculation.Parsers;
using Ringtoets.HydraRing.IO;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using HeightStructuresForms = Ringtoets.HeightStructures.Forms.Properties.Resources;

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
        /// <returns><c>True</c>c> if <paramref name="calculation"/> has no validation errors; <c>False</c>c> otherwise.</returns>
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
        /// <param name="hlcdFilePath">The filepath of the HLCD file that should be used for performing the calculation.</param>
        internal void Calculate(StructuresCalculation<HeightStructuresInput> calculation,
                                IAssessmentSection assessmentSection,
                                HeightStructuresFailureMechanism failureMechanism,
                                string hlcdFilePath)
        {
            var calculationName = calculation.Name;

            FailureMechanismSection failureMechanismSection = StructuresHelper.FailureMechanismSectionForCalculation(failureMechanism.Sections,
                                                                                                                           calculation);

            StructuresOvertoppingCalculationInput input = CreateInput(calculation, failureMechanismSection, failureMechanism.GeneralInput);

            string hlcdDirectory = Path.GetDirectoryName(hlcdFilePath);
            calculator = HydraRingCalculatorFactory.Instance.CreateStructuresOvertoppingCalculator(hlcdDirectory, assessmentSection.Id);

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
                    log.ErrorFormat(Resources.HeightStructuresCalculationService_Calculate_Error_in_height_structures_0_calculation, calculationName);
                    throw;
                }
            }
            finally
            {
                log.InfoFormat(Resources.HeightStructuresCalculationService_Calculate_Calculation_report_Click_details_for_full_report, calculator.OutputFileContent);
                CalculationServiceHelper.LogCalculationEndTime(calculationName);
            }
        }

        private static StructuresOvertoppingCalculationInput CreateInput(StructuresCalculation<HeightStructuresInput> calculation, FailureMechanismSection failureMechanismSection, GeneralHeightStructuresInput generalInput)
        {
            return new StructuresOvertoppingCalculationInput(
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
                validationResults.Add(RingtoetsCommonServiceResources.HeightStructuresCalculationService_ValidateInput_No_Structure_selected);
            }
            else
            {
                validationResults.AddRange(DistributionValidation.ValidateDistribution(inputParameters.StormDuration,
                                                                                       GenerateParameterNameWithoutUnits(RingtoetsCommonFormsResources.Structure_StormDuration_DisplayName)));

                if (IsInvalidNumber(inputParameters.DeviationWaveDirection))
                {
                    validationResults.Add(string.Format(RingtoetsCommonServiceResources.Validation_ValidateInput_No_value_entered_for_ParameterName_0_,
                                                        GenerateParameterNameWithoutUnits(RingtoetsCommonFormsResources.Structure_DeviationWaveDirection_DisplayName)));
                }

                validationResults.AddRange(DistributionValidation.ValidateDistribution(inputParameters.ModelFactorSuperCriticalFlow,
                                                                                       GenerateParameterNameWithoutUnits(RingtoetsCommonFormsResources.Structure_ModelFactorSuperCriticalFlow_DisplayName)));

                if (IsInvalidNumber(inputParameters.StructureNormalOrientation))
                {
                    validationResults.Add(string.Format(RingtoetsCommonServiceResources.Validation_ValidateInput_No_value_entered_for_ParameterName_0_,
                                                        GenerateParameterNameWithoutUnits(RingtoetsCommonFormsResources.Orientation_DisplayName)));
                }

                validationResults.AddRange(DistributionValidation.ValidateDistribution(inputParameters.FlowWidthAtBottomProtection,
                                                                                       GenerateParameterNameWithoutUnits(RingtoetsCommonFormsResources.Structure_FlowWidthAtBottomProtection_DisplayName)));
                validationResults.AddRange(DistributionValidation.ValidateDistribution(inputParameters.WidthFlowApertures,
                                                                                       GenerateParameterNameWithoutUnits(RingtoetsCommonFormsResources.Structure_WidthFlowApertures_DisplayName)));
                validationResults.AddRange(DistributionValidation.ValidateDistribution(inputParameters.StorageStructureArea,
                                                                                       GenerateParameterNameWithoutUnits(RingtoetsCommonFormsResources.Structure_StorageStructureArea_DisplayName)));
                validationResults.AddRange(DistributionValidation.ValidateDistribution(inputParameters.AllowedLevelIncreaseStorage,
                                                                                       GenerateParameterNameWithoutUnits(RingtoetsCommonFormsResources.Structure_AllowedLevelIncreaseStorage_DisplayName)));
                validationResults.AddRange(DistributionValidation.ValidateDistribution(inputParameters.LevelCrestStructure,
                                                                                       GenerateParameterNameWithoutUnits(RingtoetsCommonFormsResources.Structure_LevelCrestStructure_DisplayName)));
                validationResults.AddRange(DistributionValidation.ValidateDistribution(inputParameters.CriticalOvertoppingDischarge,
                                                                                       GenerateParameterNameWithoutUnits(RingtoetsCommonFormsResources.Structure_CriticalOvertoppingDischarge_DisplayName)));
            }

            return validationResults.ToArray();
        }

        private static string GenerateParameterNameWithoutUnits(string parameterDescription)
        {
            string[] splitString = parameterDescription.Split('[');
            return splitString.Length != 0 ? splitString[0].ToLower().Trim() : string.Empty;
        }

        private static bool IsInvalidNumber(RoundedDouble value)
        {
            return double.IsNaN(value) || double.IsInfinity(value);
        }
    }
}