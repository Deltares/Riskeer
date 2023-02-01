// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.IO;
using System.Linq;
using System.Security;
using Core.Common.Base.IO;
using Core.Common.Util;
using log4net;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.IO.HydraRing;
using Riskeer.Common.Service;
using Riskeer.Common.Service.IllustrationPoints;
using Riskeer.Common.Service.ValidationRules;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Service.Properties;
using Riskeer.HydraRing.Calculation.Calculator;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Data.Input.Hydraulics;
using Riskeer.HydraRing.Calculation.Data.Input.Overtopping;
using Riskeer.HydraRing.Calculation.Exceptions;
using RiskeerCommonServiceResources = Riskeer.Common.Service.Properties.Resources;
using RiskeerCommonForms = Riskeer.Common.Forms.Properties.Resources;
using HydraRingGeneralResult = Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints.GeneralResult;

namespace Riskeer.GrassCoverErosionInwards.Service
{
    /// <summary>
    /// Service that provides methods for performing Hydra-Ring calculations for grass cover erosion inwards calculations.
    /// </summary>
    public class GrassCoverErosionInwardsCalculationService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GrassCoverErosionInwardsCalculationService));

        private bool canceled;
        private IOvertoppingCalculator overtoppingCalculator;
        private IHydraulicLoadsCalculator dikeHeightCalculator;
        private IHydraulicLoadsCalculator overtoppingRateCalculator;

        /// <summary>
        /// Fired when the calculation progress changed.
        /// </summary>
        public event OnProgressChanged OnProgressChanged;

        /// <summary>
        /// Performs validation over the values on the given <paramref name="calculation"/>. Error and status information is logged during
        /// the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="GrassCoverErosionInwardsCalculation"/> for which to validate the values.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> for which to validate the values.</param>
        /// <returns><c>true</c> if <paramref name="calculation"/> has no validation errors; <c>false</c> otherwise.</returns>
        public static bool Validate(GrassCoverErosionInwardsCalculation calculation, IAssessmentSection assessmentSection)
        {
            CalculationServiceHelper.LogValidationBegin();

            string[] hydraulicBoundaryDatabaseMessages = ValidateHydraulicBoundaryDatabase(assessmentSection).ToArray();
            CalculationServiceHelper.LogMessagesAsError(hydraulicBoundaryDatabaseMessages);
            if (hydraulicBoundaryDatabaseMessages.Any())
            {
                CalculationServiceHelper.LogValidationEnd();
                return false;
            }

            string[] messages = ValidateInput(calculation.InputParameters).ToArray();
            CalculationServiceHelper.LogMessagesAsError(messages);
            CalculationServiceHelper.LogValidationEnd();
            return !messages.Any();
        }

        /// <summary>
        /// Cancels any currently running grass cover erosion inwards calculation.
        /// </summary>
        public void Cancel()
        {
            overtoppingCalculator?.Cancel();
            dikeHeightCalculator?.Cancel();
            overtoppingRateCalculator?.Cancel();
            canceled = true;
        }

        /// <summary>
        /// Performs a grass cover erosion inwards calculation based on the supplied <see cref="GrassCoverErosionInwardsCalculation"/> 
        /// and sets <see cref="GrassCoverErosionInwardsCalculation.Output"/> if the calculation was successful. Error and status
        /// information is logged during the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="GrassCoverErosionInwardsCalculation"/> that holds all the information required to perform the calculation.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> that holds information about the target probability used in the calculation.</param>
        /// <param name="generalInput">Calculation input parameters that apply to all <see cref="GrassCoverErosionInwardsCalculation"/> instances.</param>
        /// <exception cref="ArgumentNullException">Thrown when one of the following parameters is <c>null</c>:
        /// <list type="bullet">
        /// <item><paramref name="calculation"/></item>
        /// <item><paramref name="assessmentSection"/></item>
        /// <item><paramref name="generalInput"/></item>
        /// </list></exception>
        /// <exception cref="ArgumentException">Thrown when the hydraulic boundary database file path contains invalid characters.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>No settings database file could be found at the location of the hydraulic boundary database file path
        /// with the same name.</item>
        /// <item>Unable to open settings database file.</item>
        /// <item>Unable to read required data from database file.</item>
        /// </list>
        /// </exception>
        /// <exception cref="SecurityException">Thrown when the temporary working directory can't be accessed due to missing permissions.</exception>
        /// <exception cref="IOException">Thrown when the specified path is not valid, the network name is not known 
        /// or an I/O error occurred while opening the file.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when the directory can't be created due to missing
        /// the required permissions.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="HydraRingCalculationInput.FailureMechanismType"/>
        /// is not the same with already added input.</exception>
        /// <exception cref="Win32Exception">Thrown when there was an error in opening the associated file
        /// or the wait setting could not be accessed.</exception>
        /// <exception cref="HydraRingFileParserException">Thrown when an error occurs during parsing of the Hydra-Ring output.</exception>
        /// <exception cref="HydraRingCalculationException">Thrown when an error occurs during the calculation.</exception>
        internal void Calculate(GrassCoverErosionInwardsCalculation calculation,
                                IAssessmentSection assessmentSection,
                                GeneralGrassCoverErosionInwardsInput generalInput)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (generalInput == null)
            {
                throw new ArgumentNullException(nameof(generalInput));
            }

            HydraulicBoundaryCalculationSettings calculationSettings =
                HydraulicBoundaryCalculationSettingsFactory.CreateSettings(assessmentSection.HydraulicBoundaryDatabases, calculation.InputParameters.HydraulicBoundaryLocation);
            int numberOfCalculators = CreateCalculators(calculation, calculationSettings);

            string hydraulicBoundaryDatabaseFilePath = calculationSettings.HydraulicBoundaryDatabaseFilePath;
            bool usePreprocessor = !string.IsNullOrEmpty(calculationSettings.PreprocessorDirectory);
            CalculationServiceHelper.LogCalculationBegin();

            try
            {
                OvertoppingOutput overtoppingOutput = CalculateOvertopping(calculation,
                                                                           generalInput,
                                                                           hydraulicBoundaryDatabaseFilePath,
                                                                           usePreprocessor,
                                                                           numberOfCalculators);

                if (canceled)
                {
                    return;
                }

                DikeHeightOutput dikeHeightOutput = CalculateDikeHeight(calculation,
                                                                        generalInput,
                                                                        hydraulicBoundaryDatabaseFilePath,
                                                                        usePreprocessor,
                                                                        numberOfCalculators);
                if (canceled)
                {
                    return;
                }

                OvertoppingRateOutput overtoppingRateOutput = CalculateOvertoppingRate(calculation,
                                                                                       generalInput,
                                                                                       hydraulicBoundaryDatabaseFilePath,
                                                                                       usePreprocessor,
                                                                                       numberOfCalculators);

                if (canceled)
                {
                    return;
                }

                calculation.Output = new GrassCoverErosionInwardsOutput(
                    overtoppingOutput,
                    dikeHeightOutput,
                    overtoppingRateOutput);
            }
            finally
            {
                CalculationServiceHelper.LogCalculationEnd();

                overtoppingCalculator = null;
                dikeHeightCalculator = null;
                overtoppingRateCalculator = null;
            }
        }

        private int CreateCalculators(GrassCoverErosionInwardsCalculation calculation,
                                      HydraulicBoundaryCalculationSettings calculationSettings)
        {
            var numberOfCalculators = 1;

            HydraRingCalculationSettings hydraRingCalculationSettings = HydraRingCalculationSettingsFactory.CreateSettings(calculationSettings);
            overtoppingCalculator = HydraRingCalculatorFactory.Instance.CreateOvertoppingCalculator(
                hydraRingCalculationSettings);

            if (calculation.InputParameters.ShouldDikeHeightBeCalculated)
            {
                dikeHeightCalculator = HydraRingCalculatorFactory.Instance.CreateDikeHeightCalculator(hydraRingCalculationSettings);
                numberOfCalculators++;
            }

            if (calculation.InputParameters.ShouldOvertoppingRateBeCalculated)
            {
                overtoppingRateCalculator = HydraRingCalculatorFactory.Instance.CreateOvertoppingRateCalculator(hydraRingCalculationSettings);
                numberOfCalculators++;
            }

            return numberOfCalculators;
        }

        /// <summary>
        /// Performs an overtopping calculation.
        /// </summary>
        /// <param name="calculation">The calculation containing the input for the overtopping calculation.</param>
        /// <param name="generalInput">The general grass cover erosion inwards calculation input parameters.</param>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The path which points to the hydraulic boundary database file.</param>
        /// <param name="usePreprocessor">Indicator whether to use the preprocessor in the calculation.</param>
        /// <param name="numberOfCalculators">The total number of calculations to perform.</param>
        /// <returns>A <see cref="OvertoppingOutput"/>.</returns>
        /// <exception cref="HydraRingCalculationException">Thrown when an error occurs while performing the calculation.</exception>
        private OvertoppingOutput CalculateOvertopping(GrassCoverErosionInwardsCalculation calculation,
                                                       GeneralGrassCoverErosionInwardsInput generalInput,
                                                       string hydraulicBoundaryDatabaseFilePath,
                                                       bool usePreprocessor,
                                                       int numberOfCalculators)
        {
            NotifyProgress(string.Format(Resources.GrassCoverErosionInwardsCalculationService_Calculate_Executing_calculation_of_type_0,
                                         Resources.GrassCoverErosionInwardsCalculationService_Overtopping),
                           1, numberOfCalculators);

            OvertoppingCalculationInput overtoppingCalculationInput = CreateOvertoppingInput(calculation,
                                                                                             generalInput,
                                                                                             hydraulicBoundaryDatabaseFilePath,
                                                                                             usePreprocessor);

            PerformCalculation(() => overtoppingCalculator.Calculate(overtoppingCalculationInput),
                               () => overtoppingCalculator.LastErrorFileContent,
                               () => overtoppingCalculator.OutputDirectory,
                               calculation.Name,
                               Resources.GrassCoverErosionInwardsCalculationService_Overtopping);

            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult = null;
            try
            {
                generalResult = calculation.InputParameters.ShouldOvertoppingOutputIllustrationPointsBeCalculated
                                    ? ConvertIllustrationPointsResult(overtoppingCalculator.IllustrationPointsResult,
                                                                      overtoppingCalculator.IllustrationPointsParserErrorMessage)
                                    : null;
            }
            catch (ArgumentException e)
            {
                log.Warn(string.Format(Resources.GrassCoverErosionInwardsCalculationService_CalculateOvertopping_Error_in_reading_illustrationPoints_for_CalculationName_0_overtopping_with_ErrorMessage_1,
                                       calculation.Name,
                                       e.Message));
            }

            var overtoppingOutput = new OvertoppingOutput(overtoppingCalculator.WaveHeight,
                                                          overtoppingCalculator.IsOvertoppingDominant,
                                                          overtoppingCalculator.ExceedanceProbabilityBeta,
                                                          generalResult);
            return overtoppingOutput;
        }

        private DikeHeightOutput CalculateDikeHeight(GrassCoverErosionInwardsCalculation calculation,
                                                     GeneralGrassCoverErosionInwardsInput generalInput,
                                                     string hydraulicBoundaryDatabaseFilePath,
                                                     bool usePreprocessor,
                                                     int numberOfCalculators)
        {
            if (dikeHeightCalculator == null)
            {
                return null;
            }

            NotifyProgress(string.Format(Resources.GrassCoverErosionInwardsCalculationService_Calculate_Executing_calculation_of_type_0,
                                         Resources.GrassCoverErosionInwardsCalculationService_DikeHeight),
                           2, numberOfCalculators);

            DikeHeightCalculationInput dikeHeightCalculationInput = CreateDikeHeightInput(
                calculation, generalInput, hydraulicBoundaryDatabaseFilePath, usePreprocessor);

            var dikeHeightCalculated = true;

            try
            {
                PerformCalculation(() => dikeHeightCalculator.Calculate(dikeHeightCalculationInput),
                                   () => dikeHeightCalculator.LastErrorFileContent,
                                   () => dikeHeightCalculator.OutputDirectory,
                                   calculation.Name,
                                   Resources.GrassCoverErosionInwardsCalculationService_DikeHeight);
            }
            catch (HydraRingCalculationException)
            {
                dikeHeightCalculated = false;
            }

            if (canceled || !dikeHeightCalculated)
            {
                return null;
            }

            DikeHeightOutput output = CreateDikeHeightOutput(dikeHeightCalculator,
                                                             calculation.Name,
                                                             dikeHeightCalculationInput.Beta,
                                                             calculation.InputParameters.DikeHeightTargetProbability,
                                                             calculation.InputParameters.ShouldDikeHeightIllustrationPointsBeCalculated);
            return output;
        }

        private OvertoppingRateOutput CalculateOvertoppingRate(GrassCoverErosionInwardsCalculation calculation,
                                                               GeneralGrassCoverErosionInwardsInput generalInput,
                                                               string hydraulicBoundaryDatabaseFilePath,
                                                               bool usePreprocessor,
                                                               int numberOfCalculators)
        {
            if (overtoppingRateCalculator == null)
            {
                return null;
            }

            NotifyProgress(string.Format(Resources.GrassCoverErosionInwardsCalculationService_Calculate_Executing_calculation_of_type_0,
                                         Resources.GrassCoverErosionInwardsCalculationService_OvertoppingRate),
                           numberOfCalculators, numberOfCalculators);

            OvertoppingRateCalculationInput overtoppingRateCalculationInput = CreateOvertoppingRateInput(
                calculation, generalInput, hydraulicBoundaryDatabaseFilePath, usePreprocessor);

            var overtoppingRateCalculated = true;

            try
            {
                PerformCalculation(() => overtoppingRateCalculator.Calculate(overtoppingRateCalculationInput),
                                   () => overtoppingRateCalculator.LastErrorFileContent,
                                   () => overtoppingRateCalculator.OutputDirectory,
                                   calculation.Name,
                                   Resources.GrassCoverErosionInwardsCalculationService_OvertoppingRate);
            }
            catch (HydraRingCalculationException)
            {
                overtoppingRateCalculated = false;
            }

            if (canceled || !overtoppingRateCalculated)
            {
                return null;
            }

            OvertoppingRateOutput output = CreateOvertoppingRateOutput(overtoppingRateCalculator,
                                                                       calculation.Name,
                                                                       overtoppingRateCalculationInput.Beta,
                                                                       calculation.InputParameters.OvertoppingRateTargetProbability,
                                                                       calculation.InputParameters.ShouldOvertoppingRateIllustrationPointsBeCalculated);
            return output;
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
                            Resources.GrassCoverErosionInwardsCalculationService_Calculate_Error_in_calculation_of_type_0_for_calculation_with_name_1_no_error_report,
                            stepName,
                            calculationName);
                    }
                    else
                    {
                        log.ErrorFormat(
                            Resources.GrassCoverErosionInwardsCalculationService_Calculate_Error_in_calculation_of_type_0_for_calculation_with_name_1_click_details_for_last_error_report_2,
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
                        Resources.GrassCoverErosionInwardsCalculationService_Calculate_Error_in_calculation_of_type_0_for_calculation_with_name_1_click_details_for_last_error_report_2,
                        stepName,
                        calculationName,
                        lastErrorFileContent);
                }

                log.InfoFormat(
                    Resources.GrassCoverErosionInwardsCalculationService_Calculate_Calculation_of_type_0_performed_in_temporary_directory_1,
                    stepName,
                    getOutputDirectory());

                if (errorOccurred)
                {
                    throw new HydraRingCalculationException(lastErrorFileContent);
                }
            }
        }

        /// <summary>
        /// Creates an instance of <see cref="OvertoppingCalculationInput"/> for calculation purposes.
        /// </summary>
        /// <param name="calculation">The calculation containing the input for the overtopping calculation.</param>
        /// <param name="generalInput">The general grass cover erosion inwards calculation input parameters.</param>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The path which points to the hydraulic boundary database file.</param>
        /// <param name="usePreprocessor">Indicator whether to use the preprocessor in the calculation.</param>
        /// <returns>A new <see cref="OvertoppingCalculationInput"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="hydraulicBoundaryDatabaseFilePath"/> 
        /// contains invalid characters.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>No settings database file could be found at the location of <paramref name="hydraulicBoundaryDatabaseFilePath"/>
        /// with the same name.</item>
        /// <item>Unable to open settings database file.</item>
        /// <item>Unable to read required data from database file.</item>
        /// </list>
        /// </exception>
        private static OvertoppingCalculationInput CreateOvertoppingInput(GrassCoverErosionInwardsCalculation calculation,
                                                                          GeneralGrassCoverErosionInwardsInput generalInput,
                                                                          string hydraulicBoundaryDatabaseFilePath,
                                                                          bool usePreprocessor)
        {
            var overtoppingCalculationInput = new OvertoppingCalculationInput(calculation.InputParameters.HydraulicBoundaryLocation.Id,
                                                                              calculation.InputParameters.Orientation,
                                                                              ParseProfilePoints(calculation.InputParameters.DikeGeometry),
                                                                              HydraRingInputParser.ParseForeshore(calculation.InputParameters),
                                                                              HydraRingInputParser.ParseBreakWater(calculation.InputParameters),
                                                                              calculation.InputParameters.DikeHeight,
                                                                              generalInput.CriticalOvertoppingModelFactor,
                                                                              generalInput.FbFactor.Mean,
                                                                              generalInput.FbFactor.StandardDeviation,
                                                                              generalInput.FbFactor.LowerBoundary,
                                                                              generalInput.FbFactor.UpperBoundary,
                                                                              generalInput.FnFactor.Mean,
                                                                              generalInput.FnFactor.StandardDeviation,
                                                                              generalInput.FnFactor.LowerBoundary,
                                                                              generalInput.FnFactor.UpperBoundary,
                                                                              generalInput.OvertoppingModelFactor,
                                                                              calculation.InputParameters.CriticalFlowRate.Mean,
                                                                              calculation.InputParameters.CriticalFlowRate.StandardDeviation,
                                                                              generalInput.FrunupModelFactor.Mean,
                                                                              generalInput.FrunupModelFactor.StandardDeviation,
                                                                              generalInput.FrunupModelFactor.LowerBoundary,
                                                                              generalInput.FrunupModelFactor.UpperBoundary,
                                                                              generalInput.FshallowModelFactor.Mean,
                                                                              generalInput.FshallowModelFactor.StandardDeviation,
                                                                              generalInput.FshallowModelFactor.LowerBoundary,
                                                                              generalInput.FshallowModelFactor.UpperBoundary);

            HydraRingSettingsDatabaseHelper.AssignSettingsFromDatabase(overtoppingCalculationInput, hydraulicBoundaryDatabaseFilePath, usePreprocessor);

            return overtoppingCalculationInput;
        }

        /// <summary>
        /// Creates an instance of <see cref="DikeHeightCalculationInput"/> for calculation purposes.
        /// </summary>
        /// <param name="calculation">The calculation containing the input for the dike height calculation.</param>
        /// <param name="generalInput">The general grass cover erosion inwards calculation input parameters.</param>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The path which points to the hydraulic boundary database file.</param>
        /// <param name="usePreprocessor">Indicator whether to use the preprocessor in the calculation.</param>
        /// <returns>A new <see cref="DikeHeightCalculationInput"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="hydraulicBoundaryDatabaseFilePath"/> 
        /// contains invalid characters.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>No settings database file could be found at the location of <paramref name="hydraulicBoundaryDatabaseFilePath"/>
        /// with the same name.</item>
        /// <item>Unable to open settings database file.</item>
        /// <item>Unable to read required data from database file.</item>
        /// </list>
        /// </exception>
        private static DikeHeightCalculationInput CreateDikeHeightInput(GrassCoverErosionInwardsCalculation calculation,
                                                                        GeneralGrassCoverErosionInwardsInput generalInput,
                                                                        string hydraulicBoundaryDatabaseFilePath,
                                                                        bool usePreprocessor)
        {
            var dikeHeightCalculationInput = new DikeHeightCalculationInput(calculation.InputParameters.HydraulicBoundaryLocation.Id,
                                                                            calculation.InputParameters.DikeHeightTargetProbability,
                                                                            calculation.InputParameters.Orientation,
                                                                            ParseProfilePoints(calculation.InputParameters.DikeGeometry),
                                                                            HydraRingInputParser.ParseForeshore(calculation.InputParameters),
                                                                            HydraRingInputParser.ParseBreakWater(calculation.InputParameters),
                                                                            generalInput.CriticalOvertoppingModelFactor,
                                                                            generalInput.FbFactor.Mean,
                                                                            generalInput.FbFactor.StandardDeviation,
                                                                            generalInput.FbFactor.LowerBoundary,
                                                                            generalInput.FbFactor.UpperBoundary,
                                                                            generalInput.FnFactor.Mean,
                                                                            generalInput.FnFactor.StandardDeviation,
                                                                            generalInput.FnFactor.LowerBoundary,
                                                                            generalInput.FnFactor.UpperBoundary,
                                                                            generalInput.OvertoppingModelFactor,
                                                                            calculation.InputParameters.CriticalFlowRate.Mean,
                                                                            calculation.InputParameters.CriticalFlowRate.StandardDeviation,
                                                                            generalInput.FrunupModelFactor.Mean,
                                                                            generalInput.FrunupModelFactor.StandardDeviation,
                                                                            generalInput.FrunupModelFactor.LowerBoundary,
                                                                            generalInput.FrunupModelFactor.UpperBoundary,
                                                                            generalInput.FshallowModelFactor.Mean,
                                                                            generalInput.FshallowModelFactor.StandardDeviation,
                                                                            generalInput.FshallowModelFactor.LowerBoundary,
                                                                            generalInput.FshallowModelFactor.UpperBoundary);

            HydraRingSettingsDatabaseHelper.AssignSettingsFromDatabase(dikeHeightCalculationInput, hydraulicBoundaryDatabaseFilePath, usePreprocessor);

            return dikeHeightCalculationInput;
        }

        /// <summary>
        /// Creates an instance of <see cref="OvertoppingRateCalculationInput"/> for calculation purposes.
        /// </summary>
        /// <param name="calculation">The calculation containing the input for the overtopping rate calculation.</param>
        /// <param name="generalInput">The general grass cover erosion inwards calculation input parameters.</param>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The path which points to the hydraulic boundary database file.</param>
        /// <param name="usePreprocessor">Indicator whether to use the preprocessor in the calculation.</param>
        /// <returns>A new <see cref="OvertoppingRateCalculationInput"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="hydraulicBoundaryDatabaseFilePath"/> 
        /// contains invalid characters.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>No settings database file could be found at the location of <paramref name="hydraulicBoundaryDatabaseFilePath"/>
        /// with the same name.</item>
        /// <item>Unable to open settings database file.</item>
        /// <item>Unable to read required data from database file.</item>
        /// </list>
        /// </exception>
        private static OvertoppingRateCalculationInput CreateOvertoppingRateInput(GrassCoverErosionInwardsCalculation calculation,
                                                                                  GeneralGrassCoverErosionInwardsInput generalInput,
                                                                                  string hydraulicBoundaryDatabaseFilePath,
                                                                                  bool usePreprocessor)
        {
            var overtoppingRateCalculationInput = new OvertoppingRateCalculationInput(calculation.InputParameters.HydraulicBoundaryLocation.Id,
                                                                                      calculation.InputParameters.OvertoppingRateTargetProbability,
                                                                                      calculation.InputParameters.Orientation,
                                                                                      ParseProfilePoints(calculation.InputParameters.DikeGeometry),
                                                                                      HydraRingInputParser.ParseForeshore(calculation.InputParameters),
                                                                                      HydraRingInputParser.ParseBreakWater(calculation.InputParameters),
                                                                                      calculation.InputParameters.DikeHeight,
                                                                                      generalInput.CriticalOvertoppingModelFactor,
                                                                                      generalInput.FbFactor.Mean,
                                                                                      generalInput.FbFactor.StandardDeviation,
                                                                                      generalInput.FbFactor.LowerBoundary,
                                                                                      generalInput.FbFactor.UpperBoundary,
                                                                                      generalInput.FnFactor.Mean,
                                                                                      generalInput.FnFactor.StandardDeviation,
                                                                                      generalInput.FnFactor.LowerBoundary,
                                                                                      generalInput.FnFactor.UpperBoundary,
                                                                                      generalInput.OvertoppingModelFactor,
                                                                                      generalInput.FrunupModelFactor.Mean,
                                                                                      generalInput.FrunupModelFactor.StandardDeviation,
                                                                                      generalInput.FrunupModelFactor.LowerBoundary,
                                                                                      generalInput.FrunupModelFactor.UpperBoundary,
                                                                                      generalInput.FshallowModelFactor.Mean,
                                                                                      generalInput.FshallowModelFactor.StandardDeviation,
                                                                                      generalInput.FshallowModelFactor.LowerBoundary,
                                                                                      generalInput.FshallowModelFactor.UpperBoundary);

            HydraRingSettingsDatabaseHelper.AssignSettingsFromDatabase(overtoppingRateCalculationInput, hydraulicBoundaryDatabaseFilePath, usePreprocessor);

            return overtoppingRateCalculationInput;
        }

        /// <summary>
        /// Creates the output of a dike height calculation.
        /// </summary>
        /// <param name="calculator">The calculator used for performing the calculation.</param>
        /// <param name="calculationName">The name of the calculation.</param>
        /// <param name="targetReliability">The target reliability for the calculation.</param>
        /// <param name="targetProbability">The target probability for the calculation.</param>
        /// <param name="shouldIllustrationPointsBeCalculated">Indicates whether the illustration points
        /// should be calculated for the calculation.</param>
        /// <returns>A <see cref="DikeHeightOutput"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="targetProbability"/> 
        /// or the calculated probability falls outside the [0.0, 1.0] range and is not <see cref="double.NaN"/>.</exception>
        private static DikeHeightOutput CreateDikeHeightOutput(IHydraulicLoadsCalculator calculator,
                                                               string calculationName,
                                                               double targetReliability,
                                                               double targetProbability,
                                                               bool shouldIllustrationPointsBeCalculated)
        {
            double dikeHeight = calculator.Value;
            double reliability = calculator.ReliabilityIndex;
            double probability = StatisticsConverter.ReliabilityToProbability(reliability);

            CalculationConvergence converged = RiskeerCommonDataCalculationService.GetCalculationConvergence(calculator.Converged);

            if (converged != CalculationConvergence.CalculatedConverged)
            {
                log.Warn(
                    string.Format(Resources.GrassCoverErosionInwardsCalculationService_Calculation_of_type_0_for_calculation_with_name_1_not_converged,
                                  Resources.GrassCoverErosionInwardsCalculationService_DikeHeight,
                                  calculationName));
            }

            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult = null;
            try
            {
                generalResult = shouldIllustrationPointsBeCalculated
                                    ? ConvertIllustrationPointsResult(calculator.IllustrationPointsResult,
                                                                      calculator.IllustrationPointsParserErrorMessage)
                                    : null;
            }
            catch (ArgumentException e)
            {
                log.Warn(string.Format(Resources.GrassCoverErosionInwardsCalculationService_CalculateOvertopping_Error_in_reading_illustrationPoints_for_CalculationName_0_dike_height_with_ErrorMessage_1,
                                       calculationName,
                                       e.Message));
            }

            return new DikeHeightOutput(dikeHeight, targetProbability,
                                        targetReliability, probability, reliability,
                                        converged, generalResult);
        }

        /// <summary>
        /// Creates the output of an overtopping rate calculation.
        /// </summary>
        /// <param name="calculator">The calculator used for performing the calculation.</param>
        /// <param name="calculationName">The name of the calculation.</param>
        /// <param name="targetReliability">The target reliability for the calculation.</param>
        /// <param name="targetProbability">The target probability for the calculation.</param>
        /// <param name="shouldIllustrationPointsBeCalculated">Indicates whether the illustration points
        /// should be calculated for the calculation.</param>
        /// <returns>A <see cref="OvertoppingRateOutput"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="targetProbability"/> 
        /// or the calculated probability falls outside the [0.0, 1.0] range and is not <see cref="double.NaN"/>.</exception>
        private static OvertoppingRateOutput CreateOvertoppingRateOutput(IHydraulicLoadsCalculator calculator,
                                                                         string calculationName,
                                                                         double targetReliability,
                                                                         double targetProbability,
                                                                         bool shouldIllustrationPointsBeCalculated)
        {
            double overtoppingRate = calculator.Value;
            double reliability = calculator.ReliabilityIndex;
            double probability = StatisticsConverter.ReliabilityToProbability(reliability);

            CalculationConvergence converged = RiskeerCommonDataCalculationService.GetCalculationConvergence(calculator.Converged);

            if (converged != CalculationConvergence.CalculatedConverged)
            {
                log.Warn(
                    string.Format(Resources.GrassCoverErosionInwardsCalculationService_Calculation_of_type_0_for_calculation_with_name_1_not_converged,
                                  Resources.GrassCoverErosionInwardsCalculationService_OvertoppingRate,
                                  calculationName));
            }

            GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult = null;
            try
            {
                generalResult = shouldIllustrationPointsBeCalculated
                                    ? ConvertIllustrationPointsResult(calculator.IllustrationPointsResult,
                                                                      calculator.IllustrationPointsParserErrorMessage)
                                    : null;
            }
            catch (ArgumentException e)
            {
                log.Warn(string.Format(Resources.GrassCoverErosionInwardsCalculationService_CalculateOvertopping_Error_in_reading_illustrationPoints_for_CalculationName_0_overtopping_rate_with_ErrorMessage_1,
                                       calculationName,
                                       e.Message));
            }

            return new OvertoppingRateOutput(overtoppingRate, targetProbability,
                                             targetReliability, probability, reliability,
                                             converged, generalResult);
        }

        private static IEnumerable<HydraRingRoughnessProfilePoint> ParseProfilePoints(IEnumerable<RoughnessPoint> roughnessProfilePoints)
        {
            return roughnessProfilePoints.Select(roughnessPoint => new HydraRingRoughnessProfilePoint(roughnessPoint.Point.X,
                                                                                                      roughnessPoint.Point.Y,
                                                                                                      roughnessPoint.Roughness)).ToArray();
        }

        private static IEnumerable<string> ValidateHydraulicBoundaryDatabase(IAssessmentSection assessmentSection)
        {
            string preprocessorDirectory = assessmentSection.HydraulicBoundaryDatabases.First().EffectivePreprocessorDirectory();
            string databaseValidationProblem = HydraulicBoundaryDatabaseConnectionValidator.Validate(assessmentSection.HydraulicBoundaryDatabases.First());
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

        private static IEnumerable<string> ValidateInput(GrassCoverErosionInwardsInput inputParameters)
        {
            var validationResults = new List<string>();

            if (inputParameters.HydraulicBoundaryLocation == null)
            {
                validationResults.Add(RiskeerCommonServiceResources.CalculationService_ValidateInput_No_hydraulic_boundary_location_selected);
            }

            if (inputParameters.DikeProfile == null)
            {
                validationResults.Add(RiskeerCommonServiceResources.CalculationService_ValidateInput_No_dike_profile_selected);
            }
            else
            {
                validationResults.AddRange(new NumericInputRule(inputParameters.Orientation, ParameterNameExtractor.GetFromDisplayName(RiskeerCommonForms.Orientation_DisplayName)).Validate());
                validationResults.AddRange(new NumericInputRule(inputParameters.DikeHeight, ParameterNameExtractor.GetFromDisplayName(RiskeerCommonForms.DikeHeight_DisplayName)).Validate());
            }

            validationResults.AddRange(new UseBreakWaterRule(inputParameters).Validate());

            return validationResults;
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

        private void NotifyProgress(string stepName, int currentStepNumber, int totalStepNumber)
        {
            OnProgressChanged?.Invoke(stepName, currentStepNumber, totalStepNumber);
        }
    }
}