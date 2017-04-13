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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security;
using Core.Common.Base.IO;
using Core.Common.Utils;
using log4net;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.IO.HydraRing;
using Ringtoets.Common.Service;
using Ringtoets.Common.Service.ValidationRules;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Service.Properties;
using Ringtoets.HydraRing.Calculation.Calculator;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.Data.Input.Overtopping;
using Ringtoets.HydraRing.Calculation.Exceptions;
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
        public OnProgressChanged OnProgress;

        private bool canceled;
        private IOvertoppingCalculator overtoppingCalculator;
        private IHydraulicLoadsCalculator dikeHeightCalculator;
        private IHydraulicLoadsCalculator overtoppingRateCalculator;

        /// <summary>
        /// Performs validation over the values on the given <paramref name="calculation"/>. Error and status information is logged during
        /// the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="GrassCoverErosionInwardsCalculation"/> for which to validate the values.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> for which to validate the values.</param>
        /// <returns><c>True</c> if <paramref name="calculation"/> has no validation errors; <c>False</c> otherwise.</returns>
        public static bool Validate(GrassCoverErosionInwardsCalculation calculation, IAssessmentSection assessmentSection)
        {
            CalculationServiceHelper.LogValidationBeginTime(calculation.Name);

            string[] messages = ValidateInput(calculation.InputParameters, assessmentSection);
            CalculationServiceHelper.LogMessagesAsError(RingtoetsCommonServiceResources.Error_in_validation_0, messages);

            CalculationServiceHelper.LogValidationEndTime(calculation.Name);

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
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> that holds information about the norm used in the calculation.</param>
        /// <param name="generalInput">Calculation input parameters that apply to all <see cref="GrassCoverErosionInwardsCalculation"/> instances.</param>
        /// <param name="failureMechanismContribution">The amount of contribution for this failure mechanism in the assessment section.</param>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The path which points to the hydraulic boundary database file.</param>
        /// <exception cref="ArgumentNullException">Thrown when one of the following parameters is <c>null</c>:
        /// <list type="bullet">
        /// <item><paramref name="calculation"/></item>
        /// <item><paramref name="assessmentSection"/></item>
        /// <item><paramref name="generalInput"/></item>
        /// </list></exception>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item>The <paramref name="hydraulicBoundaryDatabaseFilePath"/> contains invalid characters.</item>
        /// <item>The contribution of the failure mechanism is zero.</item>
        /// <item>The target probability or the calculated probability of a dike height calculation falls outside 
        /// the [0.0, 1.0] range and is not <see cref="double.NaN"/>.</item>
        /// <item>The target probability or the calculated probability of an overtopping rate calculation falls outside 
        /// the [0.0, 1.0] range and is not <see cref="double.NaN"/>.</item>
        /// </list>
        /// </exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>No settings database file could be found at the location of <paramref name="hydraulicBoundaryDatabaseFilePath"/>
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
                                GeneralGrassCoverErosionInwardsInput generalInput,
                                double failureMechanismContribution,
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

            if (generalInput == null)
            {
                throw new ArgumentNullException(nameof(generalInput));
            }

            int numberOfCalculators = CreateCalculators(calculation, Path.GetDirectoryName(hydraulicBoundaryDatabaseFilePath));

            CalculationServiceHelper.LogCalculationBeginTime(calculation.Name);

            try
            {
                CalculateOvertopping(calculation, generalInput, hydraulicBoundaryDatabaseFilePath, numberOfCalculators);

                if (canceled)
                {
                    return;
                }

                HydraulicLoadsOutput dikeHeightOutput = CalculateDikeHeight(calculation,
                                                                            assessmentSection,
                                                                            generalInput,
                                                                            failureMechanismContribution,
                                                                            hydraulicBoundaryDatabaseFilePath,
                                                                            numberOfCalculators);

                if (canceled)
                {
                    return;
                }

                HydraulicLoadsOutput overtoppingRateOutput = CalculateOvertoppingRate(calculation,
                                                                                      assessmentSection,
                                                                                      generalInput,
                                                                                      failureMechanismContribution,
                                                                                      hydraulicBoundaryDatabaseFilePath,
                                                                                      numberOfCalculators);

                if (canceled)
                {
                    return;
                }

                calculation.Output = new GrassCoverErosionInwardsOutput(
                    overtoppingCalculator.WaveHeight,
                    overtoppingCalculator.IsOvertoppingDominant,
                    ProbabilityAssessmentService.Calculate(
                        assessmentSection.FailureMechanismContribution.Norm,
                        failureMechanismContribution,
                        generalInput.N,
                        overtoppingCalculator.ExceedanceProbabilityBeta),
                    dikeHeightOutput,
                    overtoppingRateOutput);
            }
            finally
            {
                CalculationServiceHelper.LogCalculationEndTime(calculation.Name);

                overtoppingCalculator = null;
                dikeHeightCalculator = null;
                overtoppingRateCalculator = null;
            }
        }

        private int CreateCalculators(GrassCoverErosionInwardsCalculation calculation, string hlcdDirectory)
        {
            var numberOfCalculators = 1;

            overtoppingCalculator = HydraRingCalculatorFactory.Instance.CreateOvertoppingCalculator(hlcdDirectory);

            if (calculation.InputParameters.DikeHeightCalculationType != DikeHeightCalculationType.NoCalculation)
            {
                dikeHeightCalculator = HydraRingCalculatorFactory.Instance.CreateDikeHeightCalculator(hlcdDirectory);
                numberOfCalculators++;
            }

            if (calculation.InputParameters.OvertoppingRateCalculationType != OvertoppingRateCalculationType.NoCalculation)
            {
                overtoppingRateCalculator = HydraRingCalculatorFactory.Instance.CreateOvertoppingRateCalculator(hlcdDirectory);
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
        /// <param name="numberOfCalculators">The total number of calculations to perform.</param>
        /// <exception cref="HydraRingCalculationException">Thrown when an error occurs while performing the calculation.</exception>
        private void CalculateOvertopping(GrassCoverErosionInwardsCalculation calculation,
                                          GeneralGrassCoverErosionInwardsInput generalInput,
                                          string hydraulicBoundaryDatabaseFilePath,
                                          int numberOfCalculators)
        {
            NotifyProgress(string.Format(Resources.GrassCoverErosionInwardsCalculationService_Calculate_Executing_calculation_of_type_0,
                                         Resources.GrassCoverErosionInwardsCalculationService_Overtopping),
                           1, numberOfCalculators);

            OvertoppingCalculationInput overtoppingCalculationInput = CreateOvertoppingInput(calculation, generalInput, hydraulicBoundaryDatabaseFilePath);

            PerformCalculation(() => overtoppingCalculator.Calculate(overtoppingCalculationInput),
                               () => overtoppingCalculator.LastErrorFileContent,
                               () => overtoppingCalculator.OutputDirectory,
                               calculation.Name,
                               Resources.GrassCoverErosionInwardsCalculationService_Overtopping);
        }

        private HydraulicLoadsOutput CalculateDikeHeight(GrassCoverErosionInwardsCalculation calculation,
                                                         IAssessmentSection assessmentSection,
                                                         GeneralGrassCoverErosionInwardsInput generalInput,
                                                         double failureMechanismContribution,
                                                         string hydraulicBoundaryDatabaseFilePath,
                                                         int numberOfCalculators)
        {
            if (dikeHeightCalculator == null)
            {
                return null;
            }

            NotifyProgress(string.Format(Resources.GrassCoverErosionInwardsCalculationService_Calculate_Executing_calculation_of_type_0,
                                         Resources.GrassCoverErosionInwardsCalculationService_DikeHeight),
                           2, numberOfCalculators);

            double norm = calculation.InputParameters.DikeHeightCalculationType == DikeHeightCalculationType.CalculateByAssessmentSectionNorm
                              ? assessmentSection.FailureMechanismContribution.Norm
                              : RingtoetsCommonDataCalculationService.ProfileSpecificRequiredProbability(
                                  assessmentSection.FailureMechanismContribution.Norm,
                                  failureMechanismContribution,
                                  generalInput.N);

            DikeHeightCalculationInput dikeHeightCalculationInput = CreateDikeHeightInput(calculation, norm,
                                                                                          generalInput,
                                                                                          hydraulicBoundaryDatabaseFilePath);

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

            return CreateHydraulicLoadsAssessmentOutput(dikeHeightCalculator,
                                                        calculation.Name,
                                                        Resources.GrassCoverErosionInwardsCalculationService_DikeHeight,
                                                        dikeHeightCalculationInput.Beta,
                                                        norm);
        }

        private HydraulicLoadsOutput CalculateOvertoppingRate(GrassCoverErosionInwardsCalculation calculation,
                                                              IAssessmentSection assessmentSection,
                                                              GeneralGrassCoverErosionInwardsInput generalInput,
                                                              double failureMechanismContribution,
                                                              string hydraulicBoundaryDatabaseFilePath,
                                                              int numberOfCalculators)
        {
            if (overtoppingRateCalculator == null)
            {
                return null;
            }

            NotifyProgress(string.Format(Resources.GrassCoverErosionInwardsCalculationService_Calculate_Executing_calculation_of_type_0,
                                         Resources.GrassCoverErosionInwardsCalculationService_OvertoppingRate),
                           numberOfCalculators, numberOfCalculators);

            double norm = calculation.InputParameters.OvertoppingRateCalculationType == OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm
                              ? assessmentSection.FailureMechanismContribution.Norm
                              : RingtoetsCommonDataCalculationService.ProfileSpecificRequiredProbability(
                                  assessmentSection.FailureMechanismContribution.Norm,
                                  failureMechanismContribution,
                                  generalInput.N);

            OvertoppingRateCalculationInput overtoppingRateCalculationInput = CreateOvertoppingRateInput(calculation, norm,
                                                                                                         generalInput,
                                                                                                         hydraulicBoundaryDatabaseFilePath);

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

            return CreateHydraulicLoadsAssessmentOutput(overtoppingRateCalculator,
                                                        calculation.Name,
                                                        Resources.GrassCoverErosionInwardsCalculationService_OvertoppingRate,
                                                        overtoppingRateCalculationInput.Beta,
                                                        norm);
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

        private static OvertoppingCalculationInput CreateOvertoppingInput(GrassCoverErosionInwardsCalculation calculation,
                                                                          GeneralGrassCoverErosionInwardsInput generalInput,
                                                                          string hydraulicBoundaryDatabaseFilePath)
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

            HydraRingSettingsDatabaseHelper.AssignSettingsFromDatabase(overtoppingCalculationInput, hydraulicBoundaryDatabaseFilePath);

            return overtoppingCalculationInput;
        }

        private static DikeHeightCalculationInput CreateDikeHeightInput(GrassCoverErosionInwardsCalculation calculation,
                                                                        double norm,
                                                                        GeneralGrassCoverErosionInwardsInput generalInput,
                                                                        string hydraulicBoundaryDatabaseFilePath)
        {
            var dikeHeightCalculationInput = new DikeHeightCalculationInput(calculation.InputParameters.HydraulicBoundaryLocation.Id,
                                                                            norm,
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

            HydraRingSettingsDatabaseHelper.AssignSettingsFromDatabase(dikeHeightCalculationInput, hydraulicBoundaryDatabaseFilePath);

            return dikeHeightCalculationInput;
        }

        private static OvertoppingRateCalculationInput CreateOvertoppingRateInput(GrassCoverErosionInwardsCalculation calculation,
                                                                                  double norm,
                                                                                  GeneralGrassCoverErosionInwardsInput generalInput,
                                                                                  string hydraulicBoundaryDatabaseFilePath)
        {
            var overtoppingRateCalculationInput = new OvertoppingRateCalculationInput(calculation.InputParameters.HydraulicBoundaryLocation.Id,
                                                                                      norm,
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

            HydraRingSettingsDatabaseHelper.AssignSettingsFromDatabase(overtoppingRateCalculationInput, hydraulicBoundaryDatabaseFilePath);

            return overtoppingRateCalculationInput;
        }

        /// <summary>
        /// Creates the output of a hydraulic loads calculation.
        /// </summary>
        /// <param name="calculator">The calculator to used for performing the calculation.</param>
        /// <param name="calculationName">The name of the calculation.</param>
        /// <param name="stepName">The name of the step that is performed.</param>
        /// <param name="targetReliability">The target reliability for the calculation.</param>
        /// <param name="targetProbability">The target probability for the calculation.</param>
        /// <returns>A <see cref="HydraulicLoadsOutput"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="targetProbability"/> 
        /// or the calculated probability falls outside the [0.0, 1.0] range and is not <see cref="double.NaN"/>.</exception>
        private static HydraulicLoadsOutput CreateHydraulicLoadsAssessmentOutput(IHydraulicLoadsCalculator calculator,
                                                                                 string calculationName,
                                                                                 string stepName,
                                                                                 double targetReliability,
                                                                                 double targetProbability)
        {
            double value = calculator.Value;
            double reliability = calculator.ReliabilityIndex;
            double probability = StatisticsConverter.ReliabilityToProbability(reliability);

            CalculationConvergence converged = RingtoetsCommonDataCalculationService.GetCalculationConvergence(calculator.Converged);

            if (converged != CalculationConvergence.CalculatedConverged)
            {
                log.Warn(
                    string.Format(Resources.GrassCoverErosionInwardsCalculationService_Calculation_of_type_0_for_calculation_with_name_1_not_converged,
                                  stepName,
                                  calculationName));
            }

            return new HydraulicLoadsOutput(value, targetProbability,
                                            targetReliability, probability, reliability,
                                            converged);
        }

        private static IEnumerable<HydraRingRoughnessProfilePoint> ParseProfilePoints(RoughnessPoint[] roughnessProfilePoints)
        {
            return roughnessProfilePoints.Select(roughnessPoint => new HydraRingRoughnessProfilePoint(roughnessPoint.Point.X, roughnessPoint.Point.Y, roughnessPoint.Roughness));
        }

        private static string[] ValidateInput(GrassCoverErosionInwardsInput inputParameters, IAssessmentSection assessmentSection)
        {
            var validationResult = new List<string>();

            string validationProblem = HydraulicDatabaseHelper.ValidatePathForCalculation(assessmentSection.HydraulicBoundaryDatabase.FilePath);
            if (!string.IsNullOrEmpty(validationProblem))
            {
                validationResult.Add(validationProblem);
                return validationResult.ToArray();
            }

            if (inputParameters.HydraulicBoundaryLocation == null)
            {
                validationResult.Add(RingtoetsCommonServiceResources.CalculationService_ValidateInput_No_hydraulic_boundary_location_selected);
            }

            if (inputParameters.DikeProfile == null)
            {
                validationResult.Add(RingtoetsCommonServiceResources.CalculationService_ValidateInput_No_dike_profile_selected);
            }
            else
            {
                validationResult.AddRange(new NumericInputRule(inputParameters.Orientation, ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonForms.Orientation_DisplayName)).Validate());
                validationResult.AddRange(new NumericInputRule(inputParameters.DikeHeight, ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonForms.DikeHeight_DisplayName)).Validate());
            }

            validationResult.AddRange(new UseBreakWaterRule(inputParameters).Validate());

            return validationResult.ToArray();
        }

        private void NotifyProgress(string stepName, int currentStepNumber, int totalStepNumber)
        {
            OnProgress?.Invoke(stepName, currentStepNumber, totalStepNumber);
        }
    }
}