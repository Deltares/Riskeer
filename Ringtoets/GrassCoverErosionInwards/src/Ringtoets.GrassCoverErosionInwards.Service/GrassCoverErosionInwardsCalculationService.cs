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
using Core.Common.Base.IO;
using log4net;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Service;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Service.Properties;
using Ringtoets.HydraRing.Calculation.Calculator;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.Data.Input.Overtopping;
using Ringtoets.HydraRing.Calculation.Parsers;
using Ringtoets.HydraRing.IO;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;
using RingtoetsCommonForms = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Service
{
    /// <summary>
    /// Service that provides methods for performing Hydra-Ring calculations for grass cover erosion inwards calculations.
    /// </summary>
    public class GrassCoverErosionInwardsCalculationService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GrassCoverErosionInwardsCalculationService));

        public ProgressChangedDelegate OnProgress;
        private IOvertoppingCalculator overtoppingCalculator;
        private IDikeHeightCalculator dikeHeightCalculator;
        private bool canceled;

        /// <summary>
        /// Performs validation over the values on the given <paramref name="calculation"/>. Error and status information is logged during
        /// the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="GrassCoverErosionInwardsCalculation"/> for which to validate the values.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> for which to validate the values.</param>
        /// <returns><c>True</c>c> if <paramref name="calculation"/> has no validation errors; <c>False</c>c> otherwise.</returns>
        public bool Validate(GrassCoverErosionInwardsCalculation calculation, IAssessmentSection assessmentSection)
        {
            CalculationServiceHelper.LogValidationBeginTime(calculation.Name);

            var messages = ValidateInput(calculation.InputParameters, assessmentSection);
            CalculationServiceHelper.LogMessagesAsError(RingtoetsCommonServiceResources.Error_in_validation_0, messages);

            CalculationServiceHelper.LogValidationEndTime(calculation.Name);

            return !messages.Any();
        }

        /// <summary>
        /// Cancels any currently running grass cover erosion inwards calculation.
        /// </summary>
        public void Cancel()
        {
            if (overtoppingCalculator != null)
            {
                overtoppingCalculator.Cancel();
            }
            if (dikeHeightCalculator != null)
            {
                dikeHeightCalculator.Cancel();
            }
            canceled = true;
        }

        /// <summary>
        /// Performs a grass cover erosion inwards dike height calculation based on the supplied <see cref="GrassCoverErosionInwardsCalculation"/> 
        /// and sets <see cref="GrassCoverErosionInwardsCalculation.Output"/> if the calculation was successful. 
        /// Error and status information is logged during the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="GrassCoverErosionInwardsCalculation"/> that holds all the information required to perform the calculation.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> that holds information about the norm used in the calculation.</param>
        /// <param name="failureMechanismSection">The <see cref="FailureMechanismSection"/> to create input with.</param>
        /// <param name="generalInput">Calculation input parameters that apply to all <see cref="GrassCoverErosionInwardsCalculation"/> instances.</param>
        /// <param name="failureMechanismContribution">The amount of contribution for this failure mechanism in the assessment section.</param>
        /// <param name="hlcdDirectory">The directory of the HLCD file that should be used for performing the calculation.</param>
        internal void CalculateDikeHeight(GrassCoverErosionInwardsCalculation calculation,
                                          IAssessmentSection assessmentSection,
                                          FailureMechanismSection failureMechanismSection,
                                          GeneralGrassCoverErosionInwardsInput generalInput,
                                          double failureMechanismContribution,
                                          string hlcdDirectory)
        {
            var calculateDikeHeight = calculation.InputParameters.CalculateDikeHeight;
            var totalSteps = calculateDikeHeight ? 2 : 1;
            var calculationName = calculation.Name;

            NotifyProgress(Resources.GrassCoverErosionInwardsCalculationService_CalculateDikeHeight_Executing_overtopping_calculation, 1, totalSteps);

            CalculationServiceHelper.LogCalculationBeginTime(calculationName);

            overtoppingCalculator = HydraRingCalculatorFactory.Instance.CreateOvertoppingCalculator(hlcdDirectory, assessmentSection.Id);
            var overtoppingCalculationInput = CreateOvertoppingInput(calculation, failureMechanismSection, generalInput);
            double? dikeHeight = null;

            try
            {
                CalculateOvertopping(overtoppingCalculationInput, calculationName);

                if (calculateDikeHeight)
                {
                    NotifyProgress(Resources.GrassCoverErosionInwardsCalculationService_CalculateDikeHeight_Executing_dikeheight_calculation, 2, totalSteps);

                    dikeHeightCalculator = HydraRingCalculatorFactory.Instance.CreateDikeHeightCalculator(hlcdDirectory, assessmentSection.Id);
                    var dikeHeightCalculationInput = CreateDikeHeightInput(calculation, assessmentSection, failureMechanismSection, generalInput);
                    CalculateDikeHeight(dikeHeightCalculationInput, calculationName);
                    dikeHeight = dikeHeightCalculator.DikeHeight;
                }

                if (!canceled)
                {
                    calculation.Output = new GrassCoverErosionInwardsOutput(
                        overtoppingCalculator.WaveHeight,
                        overtoppingCalculator.IsOvertoppingDominant,
                        ProbabilityAssessmentService.Calculate(
                            assessmentSection.FailureMechanismContribution.Norm,
                            failureMechanismContribution,
                            generalInput.N,
                            overtoppingCalculator.ExceedanceProbabilityBeta),
                        dikeHeight);
                }
            }
            finally
            {
                CalculationServiceHelper.LogCalculationEndTime(calculationName);
            }
        }

        private void NotifyProgress(string stepName, int currentStepNumber, int totalStepNumber)
        {
            if (OnProgress != null)
            {
                OnProgress(stepName, currentStepNumber, totalStepNumber);
            }
        }

        private void CalculateOvertopping(OvertoppingCalculationInput overtoppingCalculationInput, string calculationName)
        {
            try
            {
                overtoppingCalculator.Calculate(overtoppingCalculationInput);
            }
            catch (HydraRingFileParserException)
            {
                if (!canceled)
                {
                    log.ErrorFormat(Resources.GrassCoverErosionInwardsCalculationService_Calculate_Error_in_grass_cover_erosion_inwards_0_calculation, calculationName);
                    throw;
                }
            }
            finally
            {
                log.InfoFormat(Resources.GrassCoverErosionInwardsCalculationService_CalculateOvertopping_calculation_report_message_text_0, overtoppingCalculator.OutputFileContent);
            }
        }

        private void CalculateDikeHeight(DikeHeightCalculationInput dikeHeightCalculationInput, string calculationName)
        {
            if (!canceled)
            {
                try
                {
                    dikeHeightCalculator.Calculate(dikeHeightCalculationInput);
                }
                catch (HydraRingFileParserException)
                {
                    if (!canceled)
                    {
                        log.ErrorFormat(Resources.GrassCoverErosionInwardsCalculationService_Calculate_Error_in_hbn_grass_cover_erosion_inwards_0_calculation, calculationName);
                    }
                }
                finally
                {
                    log.InfoFormat(Resources.GrassCoverErosionInwardsCalculationService_CalculateDikeHeight_calculation_report_message_text_0, dikeHeightCalculator.OutputFileContent);
                }
            }
        }

        private static OvertoppingCalculationInput CreateOvertoppingInput(GrassCoverErosionInwardsCalculation calculation,
                                                                          FailureMechanismSection failureMechanismSection,
                                                                          GeneralGrassCoverErosionInwardsInput generalInput)
        {
            return new OvertoppingCalculationInput(calculation.InputParameters.HydraulicBoundaryLocation.Id,
                                                   new HydraRingSection(1, failureMechanismSection.GetSectionLength(), calculation.InputParameters.Orientation),
                                                   ParseProfilePoints(calculation.InputParameters.DikeGeometry),
                                                   ParseForeshore(calculation.InputParameters),
                                                   ParseBreakWater(calculation.InputParameters),
                                                   calculation.InputParameters.DikeHeight,
                                                   generalInput.CriticalOvertoppingModelFactor,
                                                   generalInput.FbFactor.Mean,
                                                   generalInput.FbFactor.StandardDeviation,
                                                   generalInput.FnFactor.Mean,
                                                   generalInput.FnFactor.StandardDeviation,
                                                   generalInput.OvertoppingModelFactor,
                                                   calculation.InputParameters.CriticalFlowRate.Mean,
                                                   calculation.InputParameters.CriticalFlowRate.StandardDeviation,
                                                   generalInput.FrunupModelFactor.Mean,
                                                   generalInput.FrunupModelFactor.StandardDeviation,
                                                   generalInput.FshallowModelFactor.Mean,
                                                   generalInput.FshallowModelFactor.StandardDeviation);
        }

        private static DikeHeightCalculationInput CreateDikeHeightInput(GrassCoverErosionInwardsCalculation calculation, IAssessmentSection assessmentSection,
                                                                        FailureMechanismSection failureMechanismSection, GeneralGrassCoverErosionInwardsInput generalInput)
        {
            return new DikeHeightCalculationInput(calculation.InputParameters.HydraulicBoundaryLocation.Id,
                                                  assessmentSection.FailureMechanismContribution.Norm,
                                                  new HydraRingSection(1, failureMechanismSection.GetSectionLength(), calculation.InputParameters.Orientation),
                                                  ParseProfilePoints(calculation.InputParameters.DikeGeometry),
                                                  ParseForeshore(calculation.InputParameters),
                                                  ParseBreakWater(calculation.InputParameters),
                                                  generalInput.CriticalOvertoppingModelFactor,
                                                  generalInput.FbFactor.Mean,
                                                  generalInput.FbFactor.StandardDeviation,
                                                  generalInput.FnFactor.Mean,
                                                  generalInput.FnFactor.StandardDeviation,
                                                  generalInput.OvertoppingModelFactor,
                                                  calculation.InputParameters.CriticalFlowRate.Mean,
                                                  calculation.InputParameters.CriticalFlowRate.StandardDeviation,
                                                  generalInput.FrunupModelFactor.Mean,
                                                  generalInput.FrunupModelFactor.StandardDeviation,
                                                  generalInput.FshallowModelFactor.Mean,
                                                  generalInput.FshallowModelFactor.StandardDeviation);
        }

        private static HydraRingBreakWater ParseBreakWater(GrassCoverErosionInwardsInput input)
        {
            return input.UseBreakWater ? new HydraRingBreakWater((int) input.BreakWater.Type, input.BreakWater.Height) : null;
        }

        private static IEnumerable<HydraRingForelandPoint> ParseForeshore(GrassCoverErosionInwardsInput input)
        {
            return input.UseForeshore ? input.ForeshoreGeometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)) : new HydraRingForelandPoint[0];
        }

        private static IEnumerable<HydraRingRoughnessProfilePoint> ParseProfilePoints(RoughnessPoint[] roughnessProfilePoints)
        {
            for (var i = 0; i < roughnessProfilePoints.Length; i++)
            {
                var roughnessProfilePoint = roughnessProfilePoints[i];

                if (i == 0)
                {
                    yield return new HydraRingRoughnessProfilePoint(roughnessProfilePoint.Point.X, roughnessProfilePoint.Point.Y, 1.0);
                }
                else
                {
                    var precedingRoughnessProfilePoint = roughnessProfilePoints[i - 1];

                    yield return new HydraRingRoughnessProfilePoint(roughnessProfilePoint.Point.X, roughnessProfilePoint.Point.Y, precedingRoughnessProfilePoint.Roughness);
                }
            }
        }

        private static string[] ValidateInput(GrassCoverErosionInwardsInput inputParameters, IAssessmentSection assessmentSection)
        {
            List<string> validationResult = new List<string>();

            if (assessmentSection.HydraulicBoundaryDatabase == null)
            {
                validationResult.Add(RingtoetsCommonServiceResources.CalculationService_ValidateInput_No_hydraulic_boundary_database_selected);
            }
            else
            {
                if (inputParameters.HydraulicBoundaryLocation == null)
                {
                    validationResult.Add(RingtoetsCommonServiceResources.CalculationService_ValidateInput_No_hydraulic_boundary_location_selected);
                }

                var validationProblem = HydraulicDatabaseHelper.ValidatePathForCalculation(assessmentSection.HydraulicBoundaryDatabase.FilePath);

                if (!string.IsNullOrEmpty(validationProblem))
                {
                    validationResult.Add(validationProblem);
                }

                if (inputParameters.DikeProfile == null)
                {
                    validationResult.Add(RingtoetsCommonServiceResources.CalculationService_ValidateInput_No_dike_profile_selected);
                }
                else
                {
                    if (double.IsNaN(inputParameters.Orientation))
                    {
                        string message = string.Format(RingtoetsCommonServiceResources.Validation_ValidateInput_No_value_entered_for_0_,
                                                       GenerateParameterNameWithoutUnits(RingtoetsCommonForms.Orientation_DisplayName));
                        validationResult.Add(message);
                    }
                }

                if (inputParameters.UseBreakWater)
                {
                    if (double.IsNaN(inputParameters.BreakWater.Height) || double.IsInfinity(inputParameters.BreakWater.Height))
                    {
                        validationResult.Add(RingtoetsCommonServiceResources.Validation_Invalid_BreakWaterHeight_value);
                    }
                }
            }

            return validationResult.ToArray();
        }

        private static string GenerateParameterNameWithoutUnits(string parameterDescription)
        {
            string[] splitString = parameterDescription.Split('[');
            return splitString.Length != 0 ? splitString[0].ToLower().Trim() : string.Empty;
        }
    }
}