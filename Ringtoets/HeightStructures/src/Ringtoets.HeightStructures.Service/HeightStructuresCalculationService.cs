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
using System.Linq;
using Core.Common.Base.Data;
using log4net;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Service;
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
        /// <param name="calculation">The <see cref="HeightStructuresCalculation"/> for which to validate the values.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> for which to validate the values.</param>
        /// <returns><c>True</c>c> if <paramref name="calculation"/> has no validation errors; <c>False</c>c> otherwise.</returns>
        public bool Validate(HeightStructuresCalculation calculation, IAssessmentSection assessmentSection)
        {
            CalculationServiceHelper.LogValidationBeginTime(calculation.Name);

            var messages = ValidateInput(calculation.InputParameters, assessmentSection);
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
        /// Performs a height structures calculation based on the supplied <see cref="HeightStructuresCalculation"/> and sets <see cref="HeightStructuresCalculation.Output"/>
        /// if the calculation was successful. Error and status information is logged during the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="HeightStructuresCalculation"/> that holds all the information required to perform the calculation.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> that holds information about the norm used in the calculation.</param>
        /// <param name="failureMechanismSection">The <see cref="FailureMechanismSection"/> to create input with.</param>
        /// <param name="generalInput">The <see cref="GeneralHeightStructuresInput"/> to create the input with for the calculation.</param>
        /// <param name="failureMechanismContribution">The amount of contribution for this failure mechanism in the assessment section.</param>
        /// <param name="hlcdDirectory">The directory of the HLCD file that should be used for performing the calculation.</param>
        internal void Calculate(HeightStructuresCalculation calculation,
                                IAssessmentSection assessmentSection,
                                FailureMechanismSection failureMechanismSection,
                                GeneralHeightStructuresInput generalInput,
                                double failureMechanismContribution,
                                string hlcdDirectory)
        {
            var calculationName = calculation.Name;

            StructuresOvertoppingCalculationInput input = CreateInput(calculation, failureMechanismSection, generalInput);

            calculator = HydraRingCalculatorFactory.Instance.CreateStructuresOvertoppingCalculator(hlcdDirectory, assessmentSection.Id);

            CalculationServiceHelper.LogCalculationBeginTime(calculationName);

            try
            {
                calculator.Calculate(input);

                if (!canceled)
                {
                    calculation.Output = ProbabilityAssessmentService.Calculate(assessmentSection.FailureMechanismContribution.Norm,
                                                                                failureMechanismContribution,
                                                                                generalInput.N,
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

        private static StructuresOvertoppingCalculationInput CreateInput(HeightStructuresCalculation calculation, FailureMechanismSection failureMechanismSection, GeneralHeightStructuresInput generalInput)
        {
            return new StructuresOvertoppingCalculationInput(
                calculation.InputParameters.HydraulicBoundaryLocation.Id,
                new HydraRingSection(1, failureMechanismSection.GetSectionLength(), calculation.InputParameters.StructureNormalOrientation),
                ParseForeshore(calculation.InputParameters),
                ParseBreakWater(calculation.InputParameters),
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

        private static IEnumerable<HydraRingForelandPoint> ParseForeshore(HeightStructuresInput input)
        {
            return input.UseForeshore ? input.ForeshoreGeometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)) : new HydraRingForelandPoint[0];
        }

        private static HydraRingBreakWater ParseBreakWater(HeightStructuresInput input)
        {
            return input.UseBreakWater ? new HydraRingBreakWater((int) input.BreakWater.Type, input.BreakWater.Height) : null;
        }

        private static string[] ValidateInput(HeightStructuresInput inputParameters, IAssessmentSection assessmentSection)
        {
            var validationResult = new List<string>();

            var validationProblem = HydraulicDatabaseHelper.ValidatePathForCalculation(assessmentSection.HydraulicBoundaryDatabase.FilePath);
            if (!string.IsNullOrEmpty(validationProblem))
            {
                validationResult.Add(validationProblem);
                return validationResult.ToArray();
            }

            if (inputParameters.HydraulicBoundaryLocation == null)
            {
                validationResult.Add(RingtoetsCommonServiceResources.CalculationService_ValidateInput_No_hydraulic_boundary_location_selected);
            }

            if (inputParameters.Structure == null)
            {
                validationResult.Add(RingtoetsCommonServiceResources.HeightStructuresCalculationService_ValidateInput_No_Structure_selected);
            }
            else
            {
                validationResult.AddRange(DistributionValidationService.ValidateDistribution(inputParameters.StormDuration,
                                                                                             GenerateParameterNameWithoutUnits(RingtoetsCommonFormsResources.Structure_StormDuration_DisplayName)));

                if (IsInvalidNumber(inputParameters.DeviationWaveDirection))
                {
                    validationResult.Add(string.Format(Core.Common.Base.Properties.Resources.CalculationService_ValidateInput_Value_for_0_must_be_a_valid_number,
                                                       GenerateParameterNameWithoutUnits(HeightStructuresForms.DeviationWaveDirection_DisplayName)));
                }

                validationResult.AddRange(DistributionValidationService.ValidateDistribution(inputParameters.ModelFactorSuperCriticalFlow,
                                                                                             GenerateParameterNameWithoutUnits(RingtoetsCommonFormsResources.Structure_ModelFactorSuperCriticalFlow_DisplayName)));

                if (IsInvalidNumber(inputParameters.StructureNormalOrientation))
                {
                    validationResult.Add(RingtoetsCommonDataResources.Orientation_Value_needs_to_be_between_0_and_360);
                }

                validationResult.AddRange(DistributionValidationService.ValidateDistribution(inputParameters.FlowWidthAtBottomProtection,
                                                                                             GenerateParameterNameWithoutUnits(RingtoetsCommonFormsResources.Structure_FlowWidthAtBottomProtection_DisplayName)));
                validationResult.AddRange(DistributionValidationService.ValidateDistribution(inputParameters.WidthFlowApertures,
                                                                                             GenerateParameterNameWithoutUnits(RingtoetsCommonFormsResources.Structure_WidthFlowApertures_DisplayName)));
                validationResult.AddRange(DistributionValidationService.ValidateDistribution(inputParameters.StorageStructureArea,
                                                                                             GenerateParameterNameWithoutUnits(RingtoetsCommonFormsResources.Structure_StorageStructureArea_DisplayName)));
                validationResult.AddRange(DistributionValidationService.ValidateDistribution(inputParameters.AllowedLevelIncreaseStorage,
                                                                                             GenerateParameterNameWithoutUnits(RingtoetsCommonFormsResources.Structure_AllowedLevelIncreaseStorage_DisplayName)));
                validationResult.AddRange(DistributionValidationService.ValidateDistribution(inputParameters.LevelCrestStructure,
                                                                                             GenerateParameterNameWithoutUnits(RingtoetsCommonFormsResources.Structure_LevelCrestStructure_DisplayName)));
                validationResult.AddRange(DistributionValidationService.ValidateDistribution(inputParameters.CriticalOvertoppingDischarge,
                                                                                             GenerateParameterNameWithoutUnits(RingtoetsCommonFormsResources.Structure_CriticalOvertoppingDischarge_DisplayName)));
                // Probability structure given erosion
            }

            return validationResult.ToArray();
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