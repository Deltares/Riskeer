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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using Core.Common.Base.Data;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using log4net;
using Ringtoets.Common.IO.HydraRing;
using Ringtoets.Common.Service;
using Ringtoets.HydraRing.Calculation.Calculator;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input;
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
        /// Gets whether the calculation is canceled or not.
        /// </summary>
        protected bool Canceled { get; private set; }

        /// <summary>
        /// Performs validation over the values on the given <paramref name="waveConditionsInput"/>.
        /// Error and status information is logged during the execution of the operation.
        /// </summary>
        /// <param name="waveConditionsInput">The input of the calculation.</param>
        /// <param name="name">The name of the calculation.</param>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The directory of the HLCD file that should be used for performing the calculation.</param>
        /// <param name="designWaterLevelName">The name of the design water level property.</param>
        /// <returns><c>True</c> if <paramref name="waveConditionsInput"/> has no validation errors; <c>False</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="waveConditionsInput"/>
        /// or <paramref name="designWaterLevelName"/> is <c>null</c>.</exception>
        protected static bool ValidateWaveConditionsInput(WaveConditionsInput waveConditionsInput,
                                                          string name,
                                                          string hydraulicBoundaryDatabaseFilePath,
                                                          string designWaterLevelName)
        {
            if (waveConditionsInput == null)
            {
                throw new ArgumentNullException(nameof(waveConditionsInput));
            }
            if (designWaterLevelName == null)
            {
                throw new ArgumentNullException(nameof(designWaterLevelName));
            }

            CalculationServiceHelper.LogValidationBeginTime(name);

            string[] messages = ValidateInput(hydraulicBoundaryDatabaseFilePath, waveConditionsInput, designWaterLevelName);

            CalculationServiceHelper.LogMessagesAsError(RingtoetsCommonServiceResources.Error_in_validation_0, messages);

            CalculationServiceHelper.LogValidationEndTime(name);

            return !messages.Any();
        }

        /// <summary>
        /// Performs a wave conditions calculation based on the supplied <see cref="WaveConditionsInput"/> and returns the output.
        /// Error and status information is logged during the execution of the operation.
        /// </summary>
        /// <param name="calculationName">The name of the calculation.</param>
        /// <param name="waveConditionsInput">The <see cref="WaveConditionsInput"/> that holds all the information required to perform the calculation.</param>
        /// <param name="a">The 'a' factor decided on failure mechanism level.</param>
        /// <param name="b">The 'b' factor decided on failure mechanism level.</param>
        /// <param name="c">The 'c' factor decided on failure mechanism level.</param>
        /// <param name="norm">The norm to use as the target.</param>
        /// <param name="ringId">The id of the assessment section for which calculations are performed.</param>
        /// <param name="hrdFilePath">The filepath of the hydraulic boundary database.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="WaveConditionsOutput"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="waveConditionsInput"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="hrdFilePath"/> 
        /// contains invalid characters.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>No settings database file could be found at the location of <paramref name="hrdFilePath"/>
        /// with the same name.</item>
        /// <item>Unable to open settings database file.</item>
        /// <item>Unable to read required data from database file.</item>
        /// </list>
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the target probability or 
        /// calculated probability falls outside the [0.0, 1.0] range and is not <see cref="double.NaN"/>.</exception>
        /// <exception cref="HydraRingFileParserException">Thrown when an error occurs during parsing of the Hydra-Ring output.</exception>
        /// <exception cref="HydraRingCalculationException">Thrown when an error occurs during the calculation.</exception>
        protected IEnumerable<WaveConditionsOutput> CalculateWaveConditions(string calculationName,
                                                                            WaveConditionsInput waveConditionsInput,
                                                                            RoundedDouble a,
                                                                            RoundedDouble b,
                                                                            RoundedDouble c,
                                                                            double norm,
                                                                            string ringId,
                                                                            string hrdFilePath)
        {
            if (waveConditionsInput == null)
            {
                throw new ArgumentNullException(nameof(waveConditionsInput));
            }

            var outputs = new List<WaveConditionsOutput>();

            var waterLevels = waveConditionsInput.WaterLevels.ToArray();
            foreach (var waterLevel in waterLevels.TakeWhile(waterLevel => !Canceled))
            {
                try
                {
                    log.Info(string.Format(Resources.WaveConditionsCalculationService_OnRun_Subject_0_for_waterlevel_1_started,
                                           calculationName,
                                           waterLevel));

                    NotifyProgress(waterLevel, currentStep++, TotalWaterLevelCalculations);

                    var output = CalculateWaterLevel(waterLevel,
                                                     a,
                                                     b,
                                                     c,
                                                     norm,
                                                     waveConditionsInput,
                                                     hrdFilePath,
                                                     ringId,
                                                     calculationName);

                    if (output != null)
                    {
                        outputs.Add(output);
                    }
                }
                finally
                {
                    log.Info(string.Format(Resources.WaveConditionsCalculationService_OnRun_Subject_0_for_waterlevel_1_ended,
                                           calculationName,
                                           waterLevel));
                }
            }
            return outputs;
        }

        private static string[] ValidateInput(string hydraulicBoundaryDatabaseFilePath,
                                              WaveConditionsInput input,
                                              string designWaterLevelName)
        {
            var validationResults = new List<string>();

            string validationProblem = HydraulicDatabaseHelper.ValidatePathForCalculation(hydraulicBoundaryDatabaseFilePath);
            if (!string.IsNullOrEmpty(validationProblem))
            {
                validationResults.Add(validationProblem);
            }
            else
            {
                string message = ValidateWaveConditionsInput(input, designWaterLevelName);
                if (!string.IsNullOrEmpty(message))
                {
                    validationResults.Add(message);
                }
            }

            return validationResults.ToArray();
        }

        private void NotifyProgress(RoundedDouble waterLevel, int currentStepNumber, int totalStepsNumber)
        {
            var message = string.Format(Resources.WaveConditionsCalculationService_OnRun_Calculate_waterlevel_0_, waterLevel);
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
        /// <param name="ringId">The id of the assessment section for which calculations are performed.</param>
        /// <param name="name">The name used for logging.</param>
        /// <returns>A <see cref="WaveConditionsOutput"/> if the calculation was successful; or <c>null</c> if it was canceled.</returns>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="hydraulicBoundaryDatabaseFilePath"/> 
        /// contains invalid characters or the given <see cref="HydraRingCalculationInput"/> is not unique.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>No settings database file could be found at the location of <paramref name="hydraulicBoundaryDatabaseFilePath"/>
        /// with the same name.</item>
        /// <item>Unable to open settings database file.</item>
        /// <item>Unable to read required data from database file.</item>
        /// </list></exception>
        /// <exception cref="SecurityException">Thrown when the temporary path can't be accessed due to missing permissions.</exception>
        /// <exception cref="IOException">Thrown when the specified path is not valid or the network name is not known 
        /// or an I/O error occurred while opening the file</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when the directory can't be created due to missing
        /// the required persmissions.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="HydraRingCalculationInput.FailureMechanismType"/>
        /// is not the same with already added input.</exception>
        /// <exception cref="Win32Exception">Thrown when there was an error in opening the associated file
        /// or the wait setting could not be accessed.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when the process object has already been disposed.</exception>
        /// <exception cref="HydraRingFileParserException">Thrown when the HydraRing file parser 
        /// encounters an error while parsing HydraRing output.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the target probability or 
        /// calculated probability falls outside the [0.0, 1.0] range and is not <see cref="double.NaN"/>.</exception>
        /// <exception cref="HydraRingFileParserException">Thrown when an error occurs during parsing of the Hydra-Ring output.</exception>
        /// <exception cref="HydraRingCalculationException">Thrown when an error occurs during the calculation.</exception>
        private WaveConditionsOutput CalculateWaterLevel(RoundedDouble waterLevel,
                                                         RoundedDouble a,
                                                         RoundedDouble b,
                                                         RoundedDouble c,
                                                         double norm,
                                                         WaveConditionsInput input,
                                                         string hydraulicBoundaryDatabaseFilePath,
                                                         string ringId,
                                                         string name)
        {
            string hlcdDirectory = Path.GetDirectoryName(hydraulicBoundaryDatabaseFilePath);
            calculator = HydraRingCalculatorFactory.Instance.CreateWaveConditionsCosineCalculator(hlcdDirectory, ringId);
            WaveConditionsCosineCalculationInput calculationInput = CreateInput(waterLevel, a, b, c, norm, input, hydraulicBoundaryDatabaseFilePath);

            var exceptionThrown = false;
            try
            {
                calculator.Calculate(calculationInput);

                WaveConditionsOutput output = WaveConditionsService.Calculate(waterLevel,
                                                                              calculator.WaveHeight,
                                                                              calculator.WavePeakPeriod,
                                                                              calculator.WaveAngle,
                                                                              calculator.WaveDirection,
                                                                              norm,
                                                                              calculator.ReliabilityIndex);
                return output;
            }
            catch (HydraRingFileParserException)
            {
                if (!Canceled)
                {
                    var lastErrorContent = calculator.LastErrorFileContent;
                    if (string.IsNullOrEmpty(lastErrorContent))
                    {
                        log.ErrorFormat(CultureInfo.CurrentCulture,
                                        Resources.WaveConditionsCalculationService_CalculateWaterLevel_Error_in_wave_conditions_calculation_0_for_waterlevel_1_no_error_report,
                                        name,
                                        waterLevel);
                    }
                    else
                    {
                        log.ErrorFormat(CultureInfo.CurrentCulture,
                                        Resources.WaveConditionsCalculationService_CalculateWaterLevel_Error_in_wave_conditions_calculation_0_for_waterlevel_1_click_details_for_last_error_report_2,
                                        name,
                                        waterLevel,
                                        lastErrorContent);
                    }

                    exceptionThrown = true;
                    throw;
                }
                return null;
            }
            finally
            {
                var lastErrorFileContent = calculator.LastErrorFileContent;
                bool errorOccurred = CalculationServiceHelper.HasErrorOccurred(Canceled, exceptionThrown, lastErrorFileContent);
                if (errorOccurred)
                {
                    log.ErrorFormat(CultureInfo.CurrentCulture,
                                    Resources.WaveConditionsCalculationService_CalculateWaterLevel_Error_in_wave_conditions_calculation_0_for_waterlevel_1_click_details_for_last_error_report_2,
                                    name,
                                    waterLevel,
                                    lastErrorFileContent);
                }

                log.InfoFormat(Resources.WaveConditionsCalculationService_CalculateWaterLevel_Calculation_temporary_directory_can_be_found_on_location_0, calculator.OutputDirectory);

                if (errorOccurred)
                {
                    throw new HydraRingCalculationException(lastErrorFileContent);
                }
            }
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
                                                                        string hydraulicBoundaryDatabaseFilePath)
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

            HydraRingSettingsDatabaseHelper.AssignSettingsFromDatabase(waveConditionsCosineCalculationInput, hydraulicBoundaryDatabaseFilePath);

            return waveConditionsCosineCalculationInput;
        }

        private static string ValidateWaveConditionsInput(WaveConditionsInput input, string designWaterLevelName)
        {
            if (input.HydraulicBoundaryLocation == null)
            {
                return Resources.WaveConditionsCalculationService_ValidateInput_No_HydraulicBoundaryLocation_selected;
            }

            if (double.IsNaN(input.HydraulicBoundaryLocation.DesignWaterLevel))
            {
                return string.Format(Resources.WaveConditionsCalculationService_ValidateInput_No_0_DesignWaterLevel_calculated, designWaterLevelName);
            }

            if (!input.WaterLevels.Any())
            {
                return Resources.WaveConditionsCalculationService_ValidateInput_No_derived_WaterLevels;
            }

            if (input.UseBreakWater)
            {
                if (double.IsInfinity(input.BreakWater.Height) || double.IsNaN(input.BreakWater.Height))
                {
                    return RingtoetsCommonServiceResources.Validation_Invalid_BreakWaterHeight_value;
                }
            }

            if (double.IsNaN(input.Orientation))
            {
                return string.Format(RingtoetsCommonServiceResources.Validation_ValidateInput_No_concrete_value_entered_for_ParameterName_0_,
                                     ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Orientation_DisplayName));
            }

            return null;
        }
    }
}