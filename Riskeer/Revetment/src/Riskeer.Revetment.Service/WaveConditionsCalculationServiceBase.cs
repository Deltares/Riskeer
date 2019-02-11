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
using System.Globalization;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.IO;
using log4net;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.IO.HydraRing;
using Riskeer.Common.Service;
using Riskeer.Common.Service.ValidationRules;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.Service.Properties;
using Riskeer.HydraRing.Calculation.Calculator;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Data.Input.WaveConditions;
using Riskeer.HydraRing.Calculation.Exceptions;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;
using RiskeerCommonServiceResources = Riskeer.Common.Service.Properties.Resources;

namespace Riskeer.Revetment.Service
{
    /// <summary>
    /// Base class for calculating wave conditions for failure mechanisms.
    /// </summary>
    public abstract class WaveConditionsCalculationServiceBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WaveConditionsCalculationServiceBase));
        private int currentStep = 1;
        private IWaveConditionsCosineCalculator calculator;

        protected int TotalWaterLevelCalculations;
        protected string CurrentCalculationType;

        /// <summary>
        /// Fired when the calculation progress changed.
        /// </summary>
        public event OnProgressChanged OnProgressChanged;

        /// <summary>
        /// Cancels any currently running wave conditions calculation.
        /// </summary>
        public void Cancel()
        {
            calculator?.Cancel();
            Canceled = true;
        }

        /// <summary>
        /// Performs validation over the given input parameters. Error and status information is logged
        /// during the execution of the operation.
        /// </summary>
        /// <param name="waveConditionsInput">The input of the calculation.</param>
        /// <param name="assessmentLevel">The assessment level to use for determining water levels.</param>
        /// <param name="hydraulicBoundaryDatabase">The hydraulic boundary database to validate.</param>
        /// <param name="norm">The norm to validate.</param>
        /// <returns><c>true</c> if there were no validation errors; <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="waveConditionsInput"/> or
        /// <paramref name="hydraulicBoundaryDatabase"/> is <c>null</c>.</exception>
        public static bool Validate(WaveConditionsInput waveConditionsInput,
                                    RoundedDouble assessmentLevel,
                                    HydraulicBoundaryDatabase hydraulicBoundaryDatabase,
                                    double norm)
        {
            if (waveConditionsInput == null)
            {
                throw new ArgumentNullException(nameof(waveConditionsInput));
            }

            if (hydraulicBoundaryDatabase == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryDatabase));
            }

            CalculationServiceHelper.LogValidationBegin();

            string[] messages = ValidateInput(hydraulicBoundaryDatabase,
                                              waveConditionsInput,
                                              assessmentLevel,
                                              norm);

            CalculationServiceHelper.LogMessagesAsError(messages);

            CalculationServiceHelper.LogValidationEnd();

            return !messages.Any();
        }

        /// <summary>
        /// Gets whether the calculation is canceled or not.
        /// </summary>
        protected bool Canceled { get; private set; }

        /// <summary>
        /// Performs a wave conditions calculation based on the supplied <see cref="WaveConditionsInput"/>
        /// and returns the output. Error and status information is logged during the execution
        /// of the operation.
        /// </summary>
        /// <param name="waveConditionsInput">The <see cref="WaveConditionsInput"/> that holds all the information
        /// required to perform the calculation.</param>
        /// <param name="assessmentLevel">The assessment level to use for determining water levels.</param>
        /// <param name="a">The 'a' factor decided on failure mechanism level.</param>
        /// <param name="b">The 'b' factor decided on failure mechanism level.</param>
        /// <param name="c">The 'c' factor decided on failure mechanism level.</param>
        /// <param name="norm">The norm to use as the target.</param>
        /// <param name="hydraulicBoundaryDatabase">The hydraulic boundary database to perform the calculations with.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="WaveConditionsOutput"/>.</returns>
        /// <remarks>Preprocessing is disabled when the preprocessor directory equals <see cref="string.Empty"/>.</remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="waveConditionsInput"/> or
        /// <paramref name="hydraulicBoundaryDatabase"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the hydraulic boundary database file path
        /// contains invalid characters.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>No settings database file could be found at the location of the hydraulic boundary database file path
        /// with the same name.</item>
        /// <item>Unable to open settings database file.</item>
        /// <item>Unable to read required data from database file.</item>
        /// </list>
        /// </exception>
        /// <exception cref="HydraRingCalculationException">Thrown when an error occurs during
        /// the calculations.</exception>
        protected IEnumerable<WaveConditionsOutput> CalculateWaveConditions(WaveConditionsInput waveConditionsInput,
                                                                            RoundedDouble assessmentLevel,
                                                                            RoundedDouble a,
                                                                            RoundedDouble b,
                                                                            RoundedDouble c,
                                                                            double norm,
                                                                            HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            if (waveConditionsInput == null)
            {
                throw new ArgumentNullException(nameof(waveConditionsInput));
            }

            if (hydraulicBoundaryDatabase == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryDatabase));
            }

            var calculationsFailed = 0;
            var outputs = new List<WaveConditionsOutput>();

            RoundedDouble[] waterLevels = waveConditionsInput.GetWaterLevels(assessmentLevel).ToArray();
            foreach (RoundedDouble waterLevel in waterLevels.TakeWhile(waterLevel => !Canceled))
            {
                try
                {
                    log.Info(string.Format(CultureInfo.CurrentCulture,
                                           Resources.WaveConditionsCalculationService_OnRun_Calculation_for_waterlevel_0_started,
                                           waterLevel));

                    NotifyProgress(waterLevel, currentStep++, TotalWaterLevelCalculations);

                    WaveConditionsOutput output = CalculateWaterLevel(waterLevel,
                                                                      a, b, c, norm,
                                                                      waveConditionsInput,
                                                                      HydraulicBoundaryCalculationSettingsFactory.CreateSettings(hydraulicBoundaryDatabase));

                    if (output != null)
                    {
                        outputs.Add(output);
                    }
                    else
                    {
                        calculationsFailed++;
                        outputs.Add(WaveConditionsOutputFactory.CreateFailedOutput(waterLevel, norm));
                    }
                }
                finally
                {
                    log.Info(string.Format(CultureInfo.CurrentCulture,
                                           Resources.WaveConditionsCalculationService_OnRun_Calculation_for_waterlevel_0_ended,
                                           waterLevel));
                }
            }

            if (calculationsFailed == waterLevels.Length)
            {
                string message = string.Format(CultureInfo.CurrentCulture,
                                               Resources.WaveConditionsCalculationService_CalculateWaterLevel_Error_in_wave_conditions_calculation_for_all_waterlevels);
                log.Error(message);
                throw new HydraRingCalculationException(message);
            }

            return outputs;
        }

        private static string[] ValidateInput(HydraulicBoundaryDatabase hydraulicBoundaryDatabase,
                                              WaveConditionsInput input,
                                              RoundedDouble assessmentLevel,
                                              double norm)
        {
            var validationResults = new List<string>();

            string databaseFilePathValidationProblem = HydraulicBoundaryDatabaseConnectionValidator.Validate(hydraulicBoundaryDatabase);
            if (!string.IsNullOrEmpty(databaseFilePathValidationProblem))
            {
                validationResults.Add(databaseFilePathValidationProblem);
            }

            string preprocessorDirectoryValidationProblem = HydraulicBoundaryDatabaseHelper.ValidatePreprocessorDirectory(hydraulicBoundaryDatabase.EffectivePreprocessorDirectory());
            if (!string.IsNullOrEmpty(preprocessorDirectoryValidationProblem))
            {
                validationResults.Add(preprocessorDirectoryValidationProblem);
            }

            TargetProbabilityCalculationServiceHelper.ValidateTargetProbability(norm, message => validationResults.Add(message));

            if (validationResults.Any())
            {
                return validationResults.ToArray();
            }

            validationResults.AddRange(ValidateWaveConditionsInput(input, assessmentLevel));

            return validationResults.ToArray();
        }

        private void NotifyProgress(RoundedDouble waterLevel, int currentStepNumber, int totalStepsNumber)
        {
            string message = string.Format(Resources.WaveConditionsCalculationService_OnRun_Calculate_Waterlevel_0_for_Revetment_1, waterLevel, CurrentCalculationType);
            OnProgressChanged?.Invoke(message, currentStepNumber, totalStepsNumber);
        }

        /// <summary>
        /// Calculates values for a single water level.
        /// </summary>
        /// <param name="waterLevel">The level of the water.</param>
        /// <param name="a">The 'a' factor decided on failure mechanism level.</param>
        /// <param name="b">The 'b' factor decided on failure mechanism level.</param>
        /// <param name="c">The 'c' factor decided on failure mechanism level.</param>
        /// <param name="norm">The norm to use as the target.</param>
        /// <param name="input">The input that is different per calculation.</param>
        /// <param name="calculationSettings">The <see cref="HydraulicBoundaryCalculationSettings"/> containing all data
        /// to perform a hydraulic boundary calculation.</param>
        /// <returns>A <see cref="WaveConditionsOutput"/> if the calculation was successful; or <c>null</c> if it was canceled.</returns>
        /// <remarks>Preprocessing is disabled when the preprocessor directory equals <see cref="string.Empty"/>.</remarks>
        /// <exception cref="ArgumentException">Thrown when the hydraulic boundary database file path
        /// contains invalid characters.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>No settings database file could be found at the location of the hydraulic boundary database file path
        /// with the same name.</item>
        /// <item>Unable to open settings database file.</item>
        /// <item>Unable to read required data from database file.</item>
        /// </list>
        /// </exception>
        private WaveConditionsOutput CalculateWaterLevel(RoundedDouble waterLevel,
                                                         RoundedDouble a,
                                                         RoundedDouble b,
                                                         RoundedDouble c,
                                                         double norm,
                                                         WaveConditionsInput input,
                                                         HydraulicBoundaryCalculationSettings calculationSettings)
        {
            HydraRingCalculationSettings settings = HydraRingCalculationSettingsFactory.CreateSettings(calculationSettings);
            calculator = HydraRingCalculatorFactory.Instance.CreateWaveConditionsCosineCalculator(settings);
            WaveConditionsCosineCalculationInput calculationInput = CreateInput(waterLevel, a, b, c, norm, input, calculationSettings);

            WaveConditionsOutput output;
            var exceptionThrown = false;
            try
            {
                calculator.Calculate(calculationInput);

                output = WaveConditionsOutputFactory.CreateOutput(waterLevel,
                                                                  calculator.WaveHeight,
                                                                  calculator.WavePeakPeriod,
                                                                  calculator.WaveAngle,
                                                                  calculator.WaveDirection,
                                                                  norm,
                                                                  calculator.ReliabilityIndex,
                                                                  calculator.Converged);
            }
            catch (Exception e) when (e is HydraRingCalculationException || e is ArgumentOutOfRangeException)
            {
                if (!Canceled)
                {
                    string lastErrorContent = calculator.LastErrorFileContent;
                    if (string.IsNullOrEmpty(lastErrorContent))
                    {
                        log.ErrorFormat(CultureInfo.CurrentCulture,
                                        Resources.WaveConditionsCalculationService_CalculateWaterLevel_Error_in_wave_conditions_calculation_for_waterlevel_0_no_error_report,
                                        waterLevel);
                    }
                    else
                    {
                        log.ErrorFormat(CultureInfo.CurrentCulture,
                                        Resources.WaveConditionsCalculationService_CalculateWaterLevel_Error_in_wave_conditions_calculation_for_waterlevel_0_click_details_for_last_error_report_1,
                                        waterLevel,
                                        lastErrorContent);
                    }

                    exceptionThrown = true;
                }

                output = null;
            }
            finally
            {
                string lastErrorFileContent = calculator.LastErrorFileContent;
                bool errorOccurred = CalculationServiceHelper.HasErrorOccurred(Canceled, exceptionThrown, lastErrorFileContent);
                if (errorOccurred)
                {
                    log.ErrorFormat(CultureInfo.CurrentCulture,
                                    Resources.WaveConditionsCalculationService_CalculateWaterLevel_Error_in_wave_conditions_calculation_for_waterlevel_0_click_details_for_last_error_report_1,
                                    waterLevel,
                                    lastErrorFileContent);
                }

                log.InfoFormat(CultureInfo.CurrentCulture,
                               Resources.WaveConditionsCalculationService_CalculateWaterLevel_Calculation_temporary_directory_can_be_found_on_location_0,
                               calculator.OutputDirectory);

                if (errorOccurred)
                {
                    output = null;
                }
            }

            return output;
        }

        /// <summary>
        /// Creates the input for a calculation for the given <paramref name="waterLevel"/>.
        /// </summary>
        /// <param name="waterLevel">The level of the water.</param>
        /// <param name="a">The 'a' factor decided on failure mechanism level.</param>
        /// <param name="b">The 'b' factor decided on failure mechanism level.</param>
        /// <param name="c">The 'c' factor decided on failure mechanism level.</param>
        /// <param name="norm">The norm to use as the target.</param>
        /// <param name="input">The input that is different per calculation.</param>
        /// <param name="calculationSettings">The <see cref="HydraulicBoundaryCalculationSettings"/> containing all data
        /// to perform a hydraulic boundary calculation.</param>
        /// <returns>A <see cref="WaveConditionsCalculationInput"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the hydraulic boundary database file path.
        /// contains invalid characters.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>No settings database file could be found at the location of the hydraulic boundary database file path 
        /// with the same name.</item>
        /// <item>Unable to open settings database file.</item>
        /// <item>Unable to read required data from database file.</item>
        /// </list>
        /// </exception>
        private static WaveConditionsCosineCalculationInput CreateInput(RoundedDouble waterLevel,
                                                                        RoundedDouble a,
                                                                        RoundedDouble b,
                                                                        RoundedDouble c,
                                                                        double norm,
                                                                        WaveConditionsInput input,
                                                                        HydraulicBoundaryCalculationSettings calculationSettings)
        {
            var waveConditionsCosineCalculationInput = new WaveConditionsCosineCalculationInput(
                1,
                input.Orientation,
                input.HydraulicBoundaryLocation.Id,
                norm,
                HydraRingInputParser.ParseForeshore(input),
                HydraRingInputParser.ParseBreakWater(input),
                waterLevel,
                a,
                b,
                c);

            HydraRingSettingsDatabaseHelper.AssignSettingsFromDatabase(waveConditionsCosineCalculationInput,
                                                                       calculationSettings.HydraulicBoundaryDatabaseFilePath,
                                                                       !string.IsNullOrEmpty(calculationSettings.PreprocessorDirectory));

            return waveConditionsCosineCalculationInput;
        }

        private static IEnumerable<string> ValidateWaveConditionsInput(WaveConditionsInput input,
                                                                       RoundedDouble assessmentLevel)
        {
            var messages = new List<string>();

            if (input.HydraulicBoundaryLocation == null)
            {
                messages.Add(RiskeerCommonServiceResources.CalculationService_ValidateInput_No_hydraulic_boundary_location_selected);
            }
            else if (double.IsNaN(assessmentLevel))
            {
                messages.Add(Resources.WaveConditionsCalculationService_ValidateInput_No_AssessmentLevel_calculated);
            }
            else
            {
                if (!input.GetWaterLevels(assessmentLevel).Any())
                {
                    messages.Add(Resources.WaveConditionsCalculationService_ValidateInput_No_derived_WaterLevels);
                }

                messages.AddRange(new UseBreakWaterRule(input).Validate());
                messages.AddRange(new NumericInputRule(input.Orientation, ParameterNameExtractor.GetFromDisplayName(RiskeerCommonFormsResources.Orientation_DisplayName)).Validate());
            }

            return messages;
        }
    }
}