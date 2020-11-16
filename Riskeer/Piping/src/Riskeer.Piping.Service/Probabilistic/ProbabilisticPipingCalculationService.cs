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
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using log4net;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Common.IO.HydraRing;
using Riskeer.Common.Service;
using Riskeer.Common.Service.IllustrationPoints;
using Riskeer.HydraRing.Calculation.Calculator;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input;
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
    /// Service that provides methods for performing Hydra-Ring calculations for piping probabilistic.
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
        /// <param name="failureMechanism">The <see cref="PipingFailureMechanism"/> for which to validate the values.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> for which to validate the values.</param>
        /// <returns><c>true</c> if <paramref name="calculation"/> has no validation errors; <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static bool Validate(ProbabilisticPipingCalculation calculation,
                                    PipingFailureMechanism failureMechanism,
                                    IAssessmentSection assessmentSection)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            CalculationServiceHelper.LogValidationBegin();

            LogAnyWarnings(calculation);

            bool hasErrors = LogAnyErrors(calculation, failureMechanism, assessmentSection);

            CalculationServiceHelper.LogValidationEnd();

            return !hasErrors;
        }

        /// <summary>
        /// Cancels any currently running piping calculation.
        /// </summary>
        public void Cancel()
        {
            profileSpecificCalculator?.Cancel();
            sectionSpecificCalculator?.Cancel();
            canceled = true;
        }

        /// <summary>
        /// Performs a piping calculation based on the supplied <see cref="ProbabilisticPipingCalculation"/> and sets <see cref="ProbabilisticPipingCalculation.Output"/>
        /// if the calculation was successful. Error and status information is logged during the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="ProbabilisticPipingCalculation"/> that holds all the information required to perform the calculation.</param>
        /// <param name="generalInput">The <see cref="GeneralPipingInput"/> to derive values from during the calculation.</param>
        /// <param name="calculationSettings">The <see cref="HydraulicBoundaryCalculationSettings"/> with the
        /// hydraulic boundary calculation settings.</param>
        /// <param name="sectionLength">The length of the section the calculation belongs to.</param>
        /// <remarks>Preprocessing is disabled when the preprocessor directory equals <see cref="string.Empty"/>.</remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/>, <paramref name="generalInput"/>
        /// or <paramref name="calculationSettings"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the hydraulic boundary database file path
        /// contains invalid characters.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>No settings database file could be found at the location of the hydraulic boundary database file path
        /// with the same name.</item>
        /// <item>Unable to open settings database file.</item>
        /// <item>Unable to read required data from database file.</item>
        /// </list></exception>
        /// <exception cref="HydraRingFileParserException">Thrown when an error occurs during parsing of the Hydra-Ring output.</exception>
        /// <exception cref="HydraRingCalculationException">Thrown when an error occurs during the calculation.</exception>
        internal void Calculate(ProbabilisticPipingCalculation calculation, GeneralPipingInput generalInput,
                                HydraulicBoundaryCalculationSettings calculationSettings, double sectionLength)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            if (generalInput == null)
            {
                throw new ArgumentNullException(nameof(generalInput));
            }

            if (calculationSettings == null)
            {
                throw new ArgumentNullException(nameof(calculationSettings));
            }

            string hydraulicBoundaryDatabaseFilePath = calculationSettings.HydraulicBoundaryDatabaseFilePath;
            bool usePreprocessor = !string.IsNullOrEmpty(calculationSettings.PreprocessorDirectory);

            HydraRingCalculationSettings hydraRingCalculationSettings = HydraRingCalculationSettingsFactory.CreateSettings(calculationSettings);

            profileSpecificCalculator = HydraRingCalculatorFactory.Instance.CreatePipingCalculator(
                hydraRingCalculationSettings);
            sectionSpecificCalculator = HydraRingCalculatorFactory.Instance.CreatePipingCalculator(
                hydraRingCalculationSettings);

            CalculationServiceHelper.LogCalculationBegin();

            try
            {
                PartialProbabilisticPipingOutput profileSpecificOutput = CalculateProfileSpecific(
                    calculation, generalInput, hydraulicBoundaryDatabaseFilePath, usePreprocessor);

                if (canceled)
                {
                    return;
                }

                PartialProbabilisticPipingOutput sectionSpecificOutput = CalculateSectionSpecific(
                    calculation, generalInput, sectionLength, hydraulicBoundaryDatabaseFilePath, usePreprocessor);

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
        /// Performs a profile specific calculation.
        /// </summary>
        /// <param name="calculation">The calculation containing the input for the profile specific calculation.</param>
        /// <param name="generalInput">The general piping calculation input parameters.</param>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The path which points to the hydraulic boundary database file.</param>
        /// <param name="usePreprocessor">Indicator whether to use the preprocessor in the calculation.</param>
        /// <returns>A <see cref="PartialProbabilisticPipingOutput"/>.</returns>
        /// <exception cref="HydraRingCalculationException">Thrown when an error occurs while performing the calculation.</exception>
        private PartialProbabilisticPipingOutput CalculateProfileSpecific(ProbabilisticPipingCalculation calculation, GeneralPipingInput generalInput,
                                                                          string hydraulicBoundaryDatabaseFilePath, bool usePreprocessor)
        {
            NotifyProgress(string.Format(Resources.ProbabilisticPipingCalculationService_Calculate_Executing_calculation_of_type_0,
                                         Resources.ProbabilisticPipingCalculationService_ProfileSpecific),
                           1, numberOfCalculators);

            PipingCalculationInput profileSpecificCalculationInput = CreateInput(
                calculation, generalInput, 0, hydraulicBoundaryDatabaseFilePath, usePreprocessor);

            PerformCalculation(() => profileSpecificCalculator.Calculate(profileSpecificCalculationInput),
                               () => profileSpecificCalculator.LastErrorFileContent,
                               () => profileSpecificCalculator.OutputDirectory,
                               calculation.Name,
                               Resources.ProbabilisticPipingCalculationService_ProfileSpecific);

            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult = null;
            try
            {
                generalResult = calculation.InputParameters.ShouldProfileSpecificIllustrationPointsBeCalculated
                                    ? ConvertIllustrationPointsResult(profileSpecificCalculator.IllustrationPointsResult,
                                                                      profileSpecificCalculator.IllustrationPointsParserErrorMessage)
                                    : null;
            }
            catch (ArgumentException e)
            {
                log.WarnFormat(Resources.ProbabilisticPipingCalculationService_Calculate_Error_in_reading_illustrationPoints_for_CalculationName_0_CalculationType_1_with_ErrorMessage_2,
                               calculation.Name, Resources.ProbabilisticPipingCalculationService_ProfileSpecific, e.Message);
            }

            return new PartialProbabilisticPipingOutput(profileSpecificCalculator.ExceedanceProbabilityBeta,
                                                        generalResult);
        }

        /// <summary>
        /// Performs a section specific calculation.
        /// </summary>
        /// <param name="calculation">The calculation containing the input for the section specific calculation.</param>
        /// <param name="generalInput">The general piping calculation input parameters.</param>
        /// <param name="sectionLength">The length of the section.</param>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The path which points to the hydraulic boundary database file.</param>
        /// <param name="usePreprocessor">Indicator whether to use the preprocessor in the calculation.</param>
        /// <returns>A <see cref="PartialProbabilisticPipingOutput"/>.</returns>
        /// <exception cref="HydraRingCalculationException">Thrown when an error occurs while performing the calculation.</exception>
        private PartialProbabilisticPipingOutput CalculateSectionSpecific(ProbabilisticPipingCalculation calculation, GeneralPipingInput generalInput,
                                                                          double sectionLength, string hydraulicBoundaryDatabaseFilePath, bool usePreprocessor)
        {
            NotifyProgress(string.Format(Resources.ProbabilisticPipingCalculationService_Calculate_Executing_calculation_of_type_0,
                                         Resources.ProbabilisticPipingCalculationService_SectionSpecific),
                           2, numberOfCalculators);

            PipingCalculationInput sectionSpecificCalculationInput = CreateInput(
                calculation, generalInput, sectionLength,
                hydraulicBoundaryDatabaseFilePath, usePreprocessor);

            PerformCalculation(() => sectionSpecificCalculator.Calculate(sectionSpecificCalculationInput),
                               () => sectionSpecificCalculator.LastErrorFileContent,
                               () => sectionSpecificCalculator.OutputDirectory,
                               calculation.Name,
                               Resources.ProbabilisticPipingCalculationService_SectionSpecific);

            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult = null;
            try
            {
                generalResult = calculation.InputParameters.ShouldSectionSpecificIllustrationPointsBeCalculated
                                    ? ConvertIllustrationPointsResult(sectionSpecificCalculator.IllustrationPointsResult,
                                                                      sectionSpecificCalculator.IllustrationPointsParserErrorMessage)
                                    : null;
            }
            catch (ArgumentException e)
            {
                log.WarnFormat(Resources.ProbabilisticPipingCalculationService_Calculate_Error_in_reading_illustrationPoints_for_CalculationName_0_CalculationType_1_with_ErrorMessage_2,
                               calculation.Name, Resources.ProbabilisticPipingCalculationService_SectionSpecific, e.Message);
            }

            return new PartialProbabilisticPipingOutput(sectionSpecificCalculator.ExceedanceProbabilityBeta,
                                                        generalResult);
        }

        /// <summary>
        /// Performs a probabilistic piping calculation.
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

        private static PipingCalculationInput CreateInput(ProbabilisticPipingCalculation calculation, GeneralPipingInput generalInput,
                                                          double sectionLength, string hydraulicBoundaryDatabaseFilePath, bool usePreprocessor)
        {
            ProbabilisticPipingInput pipingInput = calculation.InputParameters;
            LogNormalDistribution thicknessCoverageLayer = DerivedPipingInput.GetThicknessCoverageLayer(pipingInput);
            VariationCoefficientLogNormalDistribution seepageLength = DerivedPipingInput.GetSeepageLength(pipingInput);
            LogNormalDistribution thicknessAquiferLayer = DerivedPipingInput.GetThicknessAquiferLayer(pipingInput);
            VariationCoefficientLogNormalDistribution darcyPermeability = DerivedPipingInput.GetDarcyPermeability(pipingInput);
            VariationCoefficientLogNormalDistribution diameterD70 = DerivedPipingInput.GetDiameterD70(pipingInput);

            PipingCalculationInput input;

            if (double.IsNaN(thicknessCoverageLayer.Mean))
            {
                input = new PipingCalculationInput(
                    pipingInput.HydraulicBoundaryLocation.Id,
                    sectionLength,
                    pipingInput.PhreaticLevelExit.Mean, pipingInput.PhreaticLevelExit.StandardDeviation,
                    generalInput.WaterVolumetricWeight,
                    generalInput.UpliftModelFactor.Mean, generalInput.UpliftModelFactor.StandardDeviation,
                    pipingInput.DampingFactorExit.Mean, pipingInput.DampingFactorExit.StandardDeviation,
                    seepageLength.Mean, seepageLength.CoefficientOfVariation,
                    thicknessAquiferLayer.Mean, thicknessAquiferLayer.StandardDeviation,
                    generalInput.SandParticlesVolumicWeight,
                    generalInput.SellmeijerModelFactor.Mean, generalInput.SellmeijerModelFactor.StandardDeviation,
                    generalInput.BeddingAngle,
                    generalInput.WhitesDragCoefficient,
                    generalInput.WaterKinematicViscosity,
                    darcyPermeability.Mean, darcyPermeability.CoefficientOfVariation,
                    diameterD70.Mean, diameterD70.CoefficientOfVariation,
                    generalInput.Gravity,
                    generalInput.CriticalHeaveGradient.Mean, generalInput.CriticalHeaveGradient.StandardDeviation);
            }
            else
            {
                LogNormalDistribution effectiveThicknessCoverageLayer = DerivedPipingInput.GetEffectiveThicknessCoverageLayer(pipingInput, generalInput);
                LogNormalDistribution saturatedVolumicWeightOfCoverageLayer = DerivedPipingInput.GetSaturatedVolumicWeightOfCoverageLayer(pipingInput);

                input = new PipingCalculationInput(
                    pipingInput.HydraulicBoundaryLocation.Id,
                    sectionLength,
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
                    generalInput.WaterKinematicViscosity,
                    darcyPermeability.Mean, darcyPermeability.CoefficientOfVariation,
                    diameterD70.Mean, diameterD70.CoefficientOfVariation,
                    generalInput.Gravity,
                    generalInput.CriticalHeaveGradient.Mean, generalInput.CriticalHeaveGradient.StandardDeviation);
            }

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

        private static void LogAnyWarnings(ProbabilisticPipingCalculation calculation)
        {
            CalculationServiceHelper.LogMessagesAsWarning(PipingCalculationValidationHelper.GetValidationWarnings(calculation.InputParameters).ToArray());
        }

        private static bool LogAnyErrors(ProbabilisticPipingCalculation calculation,
                                         PipingFailureMechanism failureMechanism,
                                         IAssessmentSection assessmentSection)
        {
            string[] messages = ValidateHydraulicBoundaryDatabase(assessmentSection).ToArray();

            if (messages.Length == 0)
            {
                messages = ValidateFailureMechanismHasSections(failureMechanism).ToArray();
            }

            if (messages.Length == 0)
            {
                messages = ValidateInput(calculation.InputParameters, failureMechanism.GeneralInput).ToArray();
            }

            if (messages.Length == 0)
            {
                messages = ValidateCalculationInMultipleSections(calculation, failureMechanism).ToArray();
            }

            if (messages.Length > 0)
            {
                CalculationServiceHelper.LogMessagesAsError(messages);
                return true;
            }

            return false;
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

        private static IEnumerable<string> ValidateFailureMechanismHasSections(PipingFailureMechanism failureMechanism)
        {
            if (!failureMechanism.Sections.Any())
            {
                yield return Resources.ProbabilisticPipingCalculationService_ValidateFailureMechanismHasSections_No_sections_imported;
            }
        }

        private static IEnumerable<string> ValidateInput(ProbabilisticPipingInput input, GeneralPipingInput generalInput)
        {
            var validationResults = new List<string>();

            if (input.HydraulicBoundaryLocation == null)
            {
                validationResults.Add(RiskeerCommonServiceResources.CalculationService_ValidateInput_No_hydraulic_boundary_location_selected);
            }

            validationResults.AddRange(PipingCalculationValidationHelper.GetValidationErrors(input));

            if (!validationResults.Any())
            {
                validationResults.AddRange(ValidateCoverageLayers(input, generalInput));
            }

            return validationResults;
        }

        private static IEnumerable<string> ValidateCoverageLayers(PipingInput input, GeneralPipingInput generalInput)
        {
            if (!double.IsNaN(DerivedPipingInput.GetThicknessCoverageLayer(input).Mean))
            {
                LogNormalDistribution saturatedVolumicWeightOfCoverageLayer = DerivedPipingInput.GetSaturatedVolumicWeightOfCoverageLayer(input);
                if (saturatedVolumicWeightOfCoverageLayer.Shift < generalInput.WaterVolumetricWeight)
                {
                    yield return Resources.ProbabilisticPipingCalculationService_ValidateInput_SaturatedVolumicWeightCoverageLayer_shift_must_be_larger_than_WaterVolumetricWeight;
                }
            }
        }

        private static IEnumerable<string> ValidateCalculationInMultipleSections(ProbabilisticPipingCalculation calculation, PipingFailureMechanism failureMechanism)
        {
            int numberOfSections = failureMechanism
                                   .Sections
                                   .Count(section => calculation.IsSurfaceLineIntersectionWithReferenceLineInSection(Math2D.ConvertPointsToLineSegments(section.Points)));

            if (numberOfSections > 1)
            {
                yield return Resources.ProbabilisticPipingCalculationService_ValidateCalculationInMultipleSections_Cannot_determine_section_for_calculation;
            }
        }

        private void NotifyProgress(string stepName, int currentStepNumber, int totalStepNumber)
        {
            OnProgressChanged?.Invoke(stepName, currentStepNumber, totalStepNumber);
        }
    }
}