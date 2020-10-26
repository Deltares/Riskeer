// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using log4net;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Common.IO.HydraRing;
using Riskeer.Common.Service;
using Riskeer.Common.Service.IllustrationPoints;
using Riskeer.HydraRing.Calculation.Calculator;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input.Piping;
using Riskeer.HydraRing.Calculation.Exceptions;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Service.Properties;
using RiskeerCommonServiceResources = Riskeer.Common.Service.Properties.Resources;
using HydraRingGeneralResult = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.GeneralResult;

namespace Riskeer.Piping.Service.Probabilistic
{
    /// <summary>
    /// Service that provides methods for performing Hydra-ring calculations for piping probabilistic.
    /// </summary>
    public class ProbabilisticPipingCalculationService
    {
        private const int numberOfCalculators = 2;
        private static readonly ILog log = LogManager.GetLogger(typeof(ProbabilisticPipingCalculationService));

        private IPipingCalculator profileSpecificCalculator;
        private IPipingCalculator sectionSpecificCalculator;
        private bool canceled;
        
        /// <summary>
        /// Fired when the calculation progress changed.
        /// </summary>
        public event OnProgressChanged OnProgressChanged;

        /// <summary>
        /// Performs validation over the values on the given <paramref name="calculation"/>. Error and status information is logged during
        /// the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="ProbabilisticPipingCalculation"/> for which to validate the values.</param>
        /// <param name="generalInput">The <see cref="GeneralPipingInput"/> to derive values from used during the validation.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> for which to validate the values.</param>
        /// <returns><c>true</c> if <paramref name="calculation"/> has no validation errors; <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static bool Validate(ProbabilisticPipingCalculation calculation, GeneralPipingInput generalInput, IAssessmentSection assessmentSection)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            if (generalInput == null)
            {
                throw new ArgumentNullException(nameof(generalInput));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            CalculationServiceHelper.LogValidationBegin();

            CalculationServiceHelper.LogMessagesAsWarning(PipingCalculationValidationHelper.GetValidationWarnings(calculation.InputParameters).ToArray());

            string[] hydraulicBoundaryDatabaseMessages = ValidateHydraulicBoundaryDatabase(assessmentSection).ToArray();
            CalculationServiceHelper.LogMessagesAsError(hydraulicBoundaryDatabaseMessages);
            if (hydraulicBoundaryDatabaseMessages.Any())
            {
                CalculationServiceHelper.LogValidationEnd();
                return false;
            }

            string[] messages = ValidateInput(calculation.InputParameters, generalInput).ToArray();
            CalculationServiceHelper.LogMessagesAsError(messages);

            CalculationServiceHelper.LogValidationEnd();
            return !messages.Any();
        }

        /// <summary>
        /// Performs a structures calculation based on the supplied <see cref="ProbabilisticPipingCalculation"/> and sets <see cref="ProbabilisticPipingCalculation.Output"/>
        /// if the calculation was successful. Error and status information is logged during the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="ProbabilisticPipingCalculation"/> that holds all the information required to perform the calculation.</param>
        /// <param name="failureMechanism">The <see cref="PipingFailureMechanism"/> which the <paramref name="calculation"/> belongs to.</param>
        /// <param name="calculationSettings">The <see cref="HydraulicBoundaryCalculationSettings"/> with the
        /// hydraulic boundary calculation settings.</param>
        /// <remarks>Preprocessing is disabled when the preprocessor directory equals <see cref="string.Empty"/>.</remarks>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the hydraulic boundary database file path
        /// contains invalid characters.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when an unexpected
        /// enum value is encountered.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>No settings database file could be found at the location of the hydraulic boundary database file path
        /// with the same name.</item>
        /// <item>Unable to open settings database file.</item>
        /// <item>Unable to read required data from database file.</item>
        /// </list></exception>
        /// <exception cref="HydraRingCalculationException">Thrown when an error occurs while performing the calculation.</exception>
        public void Calculate(ProbabilisticPipingCalculation calculation, PipingFailureMechanism failureMechanism,
                              HydraulicBoundaryCalculationSettings calculationSettings)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (calculationSettings == null)
            {
                throw new ArgumentNullException(nameof(calculationSettings));
            }

            string hydraulicBoundaryDatabaseFilePath = calculationSettings.HydraulicBoundaryDatabaseFilePath;
            bool usePreprocessor = !string.IsNullOrEmpty(calculationSettings.PreprocessorDirectory);

            profileSpecificCalculator = HydraRingCalculatorFactory.Instance.CreatePipingCalculator(
                HydraRingCalculationSettingsFactory.CreateSettings(calculationSettings));

            sectionSpecificCalculator = HydraRingCalculatorFactory.Instance.CreatePipingCalculator(
                HydraRingCalculationSettingsFactory.CreateSettings(calculationSettings));

            CalculationServiceHelper.LogCalculationBegin();

            try
            {
                PartialProbabilisticPipingOutput profileSpecificOutput = CalculateProfileSpecific(
                    calculation, failureMechanism, hydraulicBoundaryDatabaseFilePath, usePreprocessor);

                if (canceled)
                {
                    return;
                }

                PartialProbabilisticPipingOutput sectionSpecificOutput = CalculateSectionSpecific(
                    calculation, failureMechanism, hydraulicBoundaryDatabaseFilePath, usePreprocessor);

                if (canceled)
                {
                    return;
                }

                calculation.Output = new ProbabilisticPipingOutput(sectionSpecificOutput, profileSpecificOutput);
            }
            finally
            {
                CalculationServiceHelper.LogCalculationEnd();

                profileSpecificCalculator = null;
                sectionSpecificCalculator = null;
            }
        }

        /// <summary>
        /// Cancels any currently running grass cover erosion inwards calculation.
        /// </summary>
        public void Cancel()
        {
            profileSpecificCalculator?.Cancel();
            sectionSpecificCalculator?.Cancel();
            canceled = true;
        }

        private PartialProbabilisticPipingOutput CalculateProfileSpecific(ProbabilisticPipingCalculation calculation, PipingFailureMechanism failureMechanism, string hydraulicBoundaryDatabaseFilePath, bool usePreprocessor)
        {
            NotifyProgress(string.Format(Resources.ProbabilisticPipingCalculationService_Calculate_Executing_calculation_of_type_0,
                                         Resources.ProbabilisticPipingCalculationService_ProfileSpecific),
                           1, numberOfCalculators);

            PipingCalculationInput profileSpecificCalculationInput = CreateInput(
                calculation, failureMechanism.GeneralInput, 0,
                hydraulicBoundaryDatabaseFilePath, usePreprocessor);

            PerformCalculation(() => profileSpecificCalculator.Calculate(profileSpecificCalculationInput),
                               () => profileSpecificCalculator.LastErrorFileContent,
                               () => profileSpecificCalculator.OutputDirectory,
                               calculation.Name,
                               Resources.ProbabilisticPipingCalculationService_ProfileSpecific);

            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult = null;
            try
            {
                generalResult = calculation.InputParameters.ShouldIllustrationPointsBeCalculated
                                    ? ConvertIllustrationPointsResult(profileSpecificCalculator.IllustrationPointsResult,
                                                                      profileSpecificCalculator.IllustrationPointsParserErrorMessage)
                                    : null;
            }
            catch (ArgumentException e)
            {
                log.WarnFormat(Resources.PipingProbabailisticCalculationService_Calculate_Error_in_reading_illustrationPoints_for_CalculationName_0_CalculationType_1_overtopping_with_ErrorMessage_2,
                               calculation.Name, Resources.ProbabilisticPipingCalculationService_ProfileSpecific, e.Message);
            }

            return new PartialProbabilisticPipingOutput(profileSpecificCalculator.ExceedanceProbabilityBeta,
                                                        generalResult);
        }

        private PartialProbabilisticPipingOutput CalculateSectionSpecific(ProbabilisticPipingCalculation calculation, PipingFailureMechanism failureMechanism, string hydraulicBoundaryDatabaseFilePath, bool usePreprocessor)
        {
            NotifyProgress(string.Format(Resources.ProbabilisticPipingCalculationService_Calculate_Executing_calculation_of_type_0,
                                         Resources.ProbabilisticPipingCalculationService_SectionSpecific),
                           2, numberOfCalculators);

            FailureMechanismSection section = failureMechanism.Sections.First(s => calculation.IsSurfaceLineIntersectionWithReferenceLineInSection(Math2D.ConvertPointsToLineSegments(s.Points)));

            PipingCalculationInput sectionSpecificCalculationInput = CreateInput(
                calculation, failureMechanism.GeneralInput, section.Length,
                hydraulicBoundaryDatabaseFilePath, usePreprocessor);

            PerformCalculation(() => sectionSpecificCalculator.Calculate(sectionSpecificCalculationInput),
                               () => sectionSpecificCalculator.LastErrorFileContent,
                               () => sectionSpecificCalculator.OutputDirectory,
                               calculation.Name,
                               Resources.ProbabilisticPipingCalculationService_SectionSpecific);

            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult = null;
            try
            {
                generalResult = calculation.InputParameters.ShouldIllustrationPointsBeCalculated
                                    ? ConvertIllustrationPointsResult(sectionSpecificCalculator.IllustrationPointsResult,
                                                                      sectionSpecificCalculator.IllustrationPointsParserErrorMessage)
                                    : null;
            }
            catch (ArgumentException e)
            {
                log.WarnFormat(Resources.PipingProbabailisticCalculationService_Calculate_Error_in_reading_illustrationPoints_for_CalculationName_0_CalculationType_1_overtopping_with_ErrorMessage_2,
                               calculation.Name, Resources.ProbabilisticPipingCalculationService_SectionSpecific, e.Message);
            }

            return new PartialProbabilisticPipingOutput(sectionSpecificCalculator.ExceedanceProbabilityBeta,
                                                        generalResult);
        }

        /// <summary>
        /// Performs a grass cover erosion inwards calculation.
        /// </summary>
        /// <param name="performCalculation">The action that performs the calculation.</param>
        /// <param name="getLastErrorFileContent">The function for obtaining the last error file content.</param>
        /// <param name="getOutputDirectory">The function for obtaining the output directory.</param>
        /// <param name="calculationName">The name of the calculation to perform.</param>
        /// <param name="stepName">The name of the step to perform.</param>
        /// <exception cref="HydraRingCalculationException">Thrown when an error occurs while performing the calculation.</exception>
        private void PerformCalculation(Action performCalculation,
                                        Func<string> getLastErrorFileContent,
                                        Func<string> getOutputDirectory,
                                        string calculationName,
                                        string stepName)
        {
            var exceptionThrown = false;

            try
            {
                performCalculation();
            }
            catch (HydraRingCalculationException)
            {
                if (!canceled)
                {
                    string lastErrorFileContent = getLastErrorFileContent();
                    if (string.IsNullOrEmpty(lastErrorFileContent))
                    {
                        log.ErrorFormat(
                            Resources.ProbabilisticPipingCalculationService_Calculate_Error_in_calculation_of_type_0_for_calculation_with_name_1_no_error_report,
                            stepName,
                            calculationName);
                    }
                    else
                    {
                        log.ErrorFormat(
                            Resources.ProbabilisticPipingCalculationService_Calculate_Error_in_calculation_of_type_0_for_calculation_with_name_1_click_details_for_last_error_report_2,
                            stepName,
                            calculationName,
                            lastErrorFileContent);
                    }

                    exceptionThrown = true;
                    throw;
                }
            }
            finally
            {
                string lastErrorFileContent = getLastErrorFileContent();
                bool errorOccurred = CalculationServiceHelper.HasErrorOccurred(canceled, exceptionThrown, lastErrorFileContent);
                if (errorOccurred)
                {
                    log.ErrorFormat(
                        Resources.ProbabilisticPipingCalculationService_Calculate_Error_in_calculation_of_type_0_for_calculation_with_name_1_click_details_for_last_error_report_2,
                        stepName,
                        calculationName,
                        lastErrorFileContent);
                }

                log.InfoFormat(
                    Resources.ProbabilisticPipingCalculationService_Calculate_Calculation_of_type_0_performed_in_temporary_directory_1,
                    stepName,
                    getOutputDirectory());

                if (errorOccurred)
                {
                    throw new HydraRingCalculationException(lastErrorFileContent);
                }
            }
        }

        private PipingCalculationInput CreateInput(ProbabilisticPipingCalculation calculation, GeneralPipingInput generalInput, double sectionLength, string hydraulicBoundaryDatabaseFilePath, bool usePreprocessor)
        {
            ProbabilisticPipingInput pipingInput = calculation.InputParameters;

            LogNormalDistribution effectiveThicknessCoverageLayer = DerivedPipingInput.GetEffectiveThicknessCoverageLayer(pipingInput, generalInput);
            LogNormalDistribution saturatedVolumicWeightOfCoverageLayer = DerivedPipingInput.GetSaturatedVolumicWeightOfCoverageLayer(pipingInput);
            VariationCoefficientLogNormalDistribution seepageLength = DerivedPipingInput.GetSeepageLength(pipingInput);
            LogNormalDistribution thicknessAquiferLayer = DerivedPipingInput.GetThicknessAquiferLayer(pipingInput);
            VariationCoefficientLogNormalDistribution darcyPermeability = DerivedPipingInput.GetDarcyPermeability(pipingInput);
            VariationCoefficientLogNormalDistribution diameterD70 = DerivedPipingInput.GetDiameterD70(pipingInput);

            var input = new PipingCalculationInput(
                pipingInput.HydraulicBoundaryLocation.Id,
                sectionLength,
                generalInput.WaterVolumetricWeight,
                pipingInput.PhreaticLevelExit.Mean, pipingInput.PhreaticLevelExit.StandardDeviation,
                generalInput.WaterVolumetricWeight,
                effectiveThicknessCoverageLayer.Mean, effectiveThicknessCoverageLayer.StandardDeviation,
                saturatedVolumicWeightOfCoverageLayer.Mean, saturatedVolumicWeightOfCoverageLayer.StandardDeviation,
                saturatedVolumicWeightOfCoverageLayer.Shift,
                generalInput.UpliftModelFactor.Mean, generalInput.UpliftModelFactor.StandardDeviation,
                pipingInput.DampingFactorExit.Mean, pipingInput.DampingFactorExit.StandardDeviation,
                seepageLength.Mean, seepageLength.CoefficientOfVariation,
                thicknessAquiferLayer.Mean, thicknessAquiferLayer.StandardDeviation,
                generalInput.SandParticlesVolumicWeight,
                generalInput.SellmeijerModelFactor.Mean, generalInput.SellmeijerModelFactor.StandardDeviation,
                generalInput.BeddingAngle,
                generalInput.WhitesDragCoefficient,
                darcyPermeability.Mean, darcyPermeability.CoefficientOfVariation,
                diameterD70.Mean, diameterD70.CoefficientOfVariation,
                generalInput.Gravity,
                generalInput.CriticalHeaveGradient.Mean, generalInput.CriticalHeaveGradient.StandardDeviation);

            HydraRingSettingsDatabaseHelper.AssignSettingsFromDatabase(input, hydraulicBoundaryDatabaseFilePath, usePreprocessor);

            return input;
        }

        private static GeneralResult<TopLevelFaultTreeIllustrationPoint> ConvertIllustrationPointsResult(HydraRingGeneralResult result, string errorMessage)
        {
            if (result == null)
            {
                log.Warn(errorMessage);
                return null;
            }

            try
            {
                GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult =
                    GeneralResultConverter.ConvertToGeneralResultTopLevelFaultTreeIllustrationPoint(result);
                return generalResult;
            }
            catch (IllustrationPointConversionException e)
            {
                log.Warn(RiskeerCommonServiceResources.SetGeneralResult_Converting_IllustrationPointResult_Failed, e);
            }

            return null;
        }

        private static IEnumerable<string> ValidateHydraulicBoundaryDatabase(IAssessmentSection assessmentSection)
        {
            string preprocessorDirectory = assessmentSection.HydraulicBoundaryDatabase.EffectivePreprocessorDirectory();
            string databaseValidationProblem = HydraulicBoundaryDatabaseConnectionValidator.Validate(assessmentSection.HydraulicBoundaryDatabase);
            if (!string.IsNullOrEmpty(databaseValidationProblem))
            {
                yield return databaseValidationProblem;
            }

            string preprocessorDirectoryValidationProblem = HydraulicBoundaryDatabaseHelper.ValidatePreprocessorDirectory(preprocessorDirectory);
            if (!string.IsNullOrEmpty(preprocessorDirectoryValidationProblem))
            {
                yield return preprocessorDirectoryValidationProblem;
            }
        }

        private static IEnumerable<string> ValidateInput(ProbabilisticPipingInput input, GeneralPipingInput generalInput)
        {
            var validationResults = new List<string>();

            if (input.HydraulicBoundaryLocation == null)
            {
                validationResults.Add(RiskeerCommonServiceResources.CalculationService_ValidateInput_No_hydraulic_boundary_location_selected);
            }

            validationResults.AddRange(PipingCalculationValidationHelper.GetValidationErrors(input, generalInput));

            return validationResults;
        }
        
        private void NotifyProgress(string stepName, int currentStepNumber, int totalStepNumber)
        {
            OnProgressChanged?.Invoke(stepName, currentStepNumber, totalStepNumber);
        }
    }
}