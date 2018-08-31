// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Globalization;
using System.IO;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.IO;
using log4net;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.IO.HydraRing;
using Ringtoets.Common.Service;
using Ringtoets.Common.Service.ValidationRules;
using Ringtoets.HydraRing.Calculation.Calculator;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input.WaveConditions;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Service.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;

namespace Ringtoets.Revetment.Service
{
    /// <summary>
    /// Base class for calculating wave conditions for failure mechanisms.
    /// </summary>
    public abstract class WaveConditionsCalculationServiceBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WaveConditionsCalculationServiceBase));

        public OnProgressChanged OnProgress;
        protected int TotalWaterLevelCalculations;
        private int currentStep = 1;
        private IWaveConditionsCosineCalculator calculator;

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
        /// <param name="hydraulicBoundaryDatabaseFilePath">The file path of the hydraulic boundary database.</param>
        /// <param name="preprocessorDirectory">The preprocessor directory.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="WaveConditionsOutput"/>.</returns>
        /// <remarks>Preprocessing is disabled when <paramref name="preprocessorDirectory"/> equals <see cref="string.Empty"/>.</remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="waveConditionsInput"/>,
        /// <paramref name="hydraulicBoundaryDatabaseFilePath"/> or <paramref name="preprocessorDirectory"/>
        /// is <c>null</c>.</exception>
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
        /// <exception cref="HydraRingCalculationException">Thrown when an error occurs during
        /// the calculations.</exception>
        protected IEnumerable<WaveConditionsOutput> CalculateWaveConditions(WaveConditionsInput waveConditionsInput,
                                                                            RoundedDouble assessmentLevel,
                                                                            RoundedDouble a,
                                                                            RoundedDouble b,
                                                                            RoundedDouble c,
                                                                            double norm,
                                                                            string hydraulicBoundaryDatabaseFilePath,
                                                                            string preprocessorDirectory)
        {
            if (waveConditionsInput == null)
            {
                throw new ArgumentNullException(nameof(waveConditionsInput));
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
                                                                      a,
                                                                      b,
                                                                      c,
                                                                      norm,
                                                                      waveConditionsInput,
                                                                      hydraulicBoundaryDatabaseFilePath,
                                                                      preprocessorDirectory);

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
            string message = string.Format(Resources.WaveConditionsCalculationService_OnRun_Calculate_for_waterlevel_0_, waterLevel);
            OnProgress?.Invoke(message, currentStepNumber, totalStepsNumber);
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
        /// <param name="hydraulicBoundaryDatabaseFilePath">The path which points to the hydraulic boundary database file.</param>
        /// <param name="preprocessorDirectory">The preprocessor directory.</param>
        /// <returns>A <see cref="WaveConditionsOutput"/> if the calculation was successful; or <c>null</c> if it was canceled.</returns>
        /// <remarks>Preprocessing is disabled when <paramref name="preprocessorDirectory"/> equals <see cref="string.Empty"/>.</remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryDatabaseFilePath"/> or
        /// <paramref name="preprocessorDirectory"/> is <c>null</c>.</exception>
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
        private WaveConditionsOutput CalculateWaterLevel(RoundedDouble waterLevel,
                                                         RoundedDouble a,
                                                         RoundedDouble b,
                                                         RoundedDouble c,
                                                         double norm,
                                                         WaveConditionsInput input,
                                                         string hydraulicBoundaryDatabaseFilePath,
                                                         string preprocessorDirectory)
        {
            string hlcdDirectory = Path.GetDirectoryName(hydraulicBoundaryDatabaseFilePath);
            calculator = HydraRingCalculatorFactory.Instance.CreateWaveConditionsCosineCalculator(hlcdDirectory, preprocessorDirectory);
            WaveConditionsCosineCalculationInput calculationInput = CreateInput(waterLevel, a, b, c, norm, input, hydraulicBoundaryDatabaseFilePath,
                                                                                !string.IsNullOrEmpty(preprocessorDirectory));

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
        /// <param name="hydraulicBoundaryDatabaseFilePath">The path to the hydraulic boundary database file.</param>
        /// <param name="usePreprocessor">Indicator whether to use the preprocessor in the calculation.</param>
        /// <returns>A <see cref="WaveConditionsCalculationInput"/>.</returns>
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
        private static WaveConditionsCosineCalculationInput CreateInput(RoundedDouble waterLevel,
                                                                        RoundedDouble a,
                                                                        RoundedDouble b,
                                                                        RoundedDouble c,
                                                                        double norm,
                                                                        WaveConditionsInput input,
                                                                        string hydraulicBoundaryDatabaseFilePath,
                                                                        bool usePreprocessor)
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

            HydraRingSettingsDatabaseHelper.AssignSettingsFromDatabase(waveConditionsCosineCalculationInput, hydraulicBoundaryDatabaseFilePath, usePreprocessor);

            return waveConditionsCosineCalculationInput;
        }

        private static IEnumerable<string> ValidateWaveConditionsInput(WaveConditionsInput input,
                                                                       RoundedDouble assessmentLevel)
        {
            var messages = new List<string>();

            if (input.HydraulicBoundaryLocation == null)
            {
                messages.Add(RingtoetsCommonServiceResources.CalculationService_ValidateInput_No_hydraulic_boundary_location_selected);
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
                messages.AddRange(new NumericInputRule(input.Orientation, ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Orientation_DisplayName)).Validate());
            }

            return messages;
        }
    }
}