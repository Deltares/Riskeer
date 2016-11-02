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
using System.Globalization;
using System.IO;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.IO;
using log4net;
using Ringtoets.Common.Service;
using Ringtoets.HydraRing.Calculation.Calculator;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input.WaveConditions;
using Ringtoets.HydraRing.Calculation.Parsers;
using Ringtoets.HydraRing.IO;
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

        public ProgressChangedDelegate OnProgress;
        protected int TotalWaterLevelCalculations;
        private int currentStep = 1;
        private IWaveConditionsCosineCalculator calculator;

        /// <summary>
        /// Cancels any currently running wave conditions calculation.
        /// </summary>
        public void Cancel()
        {
            if (calculator != null)
            {
                calculator.Cancel();
            }
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
        /// <returns><c>True</c>c> if <paramref name="waveConditionsInput"/> has no validation errors; <c>False</c>c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="designWaterLevelName"/> is <c>null</c>.</exception>
        protected static bool ValidateWaveConditionsInput(WaveConditionsInput waveConditionsInput,
                                                          string name,
                                                          string hydraulicBoundaryDatabaseFilePath,
                                                          string designWaterLevelName)
        {
            if (designWaterLevelName == null)
            {
                throw new ArgumentNullException("designWaterLevelName");
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
        /// <param name="hlcdFilePath">The filepath of the hydraulic boundary database.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="WaveConditionsOutput"/>.</returns>
        protected IEnumerable<WaveConditionsOutput> CalculateWaveConditions(string calculationName,
                                                                            WaveConditionsInput waveConditionsInput,
                                                                            RoundedDouble a,
                                                                            RoundedDouble b,
                                                                            RoundedDouble c,
                                                                            double norm,
                                                                            string ringId,
                                                                            string hlcdFilePath)
        {
            var outputs = new List<WaveConditionsOutput>();

            var waterLevels = waveConditionsInput.WaterLevels.ToArray();
            foreach (var waterLevel in waterLevels.TakeWhile(waterLevel => !Canceled))
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
                                                 Path.GetDirectoryName(hlcdFilePath),
                                                 ringId,
                                                 calculationName);

                if (output != null)
                {
                    outputs.Add(output);
                }

                log.Info(string.Format(Resources.WaveConditionsCalculationService_OnRun_Subject_0_for_waterlevel_1_ended,
                                       calculationName,
                                       waterLevel));
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
            if (OnProgress != null)
            {
                var message = string.Format(Resources.WaveConditionsCalculationService_OnRun_Calculate_waterlevel_0_, waterLevel);
                OnProgress(message, currentStepNumber, totalStepsNumber);
            }
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
        /// <param name="hlcdDirectory">The directory of the hydraulic boundary database.</param>
        /// <param name="ringId">The id of the assessment section for which calculations are performed.</param>
        /// <param name="name">The name used for logging.</param>
        /// <returns>A <see cref="WaveConditionsOutput"/> if the calculation was succesful; or <c>null</c> if it was canceled.</returns>
        private WaveConditionsOutput CalculateWaterLevel(RoundedDouble waterLevel,
                                                         RoundedDouble a,
                                                         RoundedDouble b,
                                                         RoundedDouble c,
                                                         double norm,
                                                         WaveConditionsInput input,
                                                         string hlcdDirectory,
                                                         string ringId,
                                                         string name)
        {
            calculator = HydraRingCalculatorFactory.Instance.CreateWaveConditionsCosineCalculator(hlcdDirectory, ringId);
            WaveConditionsCosineCalculationInput calculationInput = CreateInput(waterLevel, a, b, c, norm, input);

            var exceptionThrown = false;
            try
            {
                calculator.Calculate(calculationInput);

                return new WaveConditionsOutput(waterLevel,
                                                calculator.WaveHeight,
                                                calculator.WavePeakPeriod,
                                                calculator.WaveAngle);
            }
            catch (HydraRingFileParserException)
            {
                if (!Canceled)
                {
                    var lastErrorContent = calculator.LastErrorContent;
                    if (string.IsNullOrEmpty(lastErrorContent))
                    {
                        log.ErrorFormat(CultureInfo.CurrentCulture,
                                        Resources.WaveConditionsCalculationService_CalculateWaterLevel_Unexplained_error_in_wave_conditions_calculation_0_for_waterlevel_1,
                                        name,
                                        waterLevel);
                    }
                    else
                    {
                        log.ErrorFormat(CultureInfo.CurrentCulture,
                                        Resources.WaveConditionsCalculationService_CalculateWaterLevel_Error_in_wave_conditions_calculation_0_for_waterlevel_1_click_details_for_last_error_2,
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
                try
                {
                    var lastErrorContent = calculator.LastErrorContent;
                    if (!exceptionThrown && !string.IsNullOrEmpty(lastErrorContent))
                    {
                        log.ErrorFormat(CultureInfo.CurrentCulture,
                                        Resources.WaveConditionsCalculationService_CalculateWaterLevel_Error_in_wave_conditions_calculation_0_for_waterlevel_1_click_details_for_last_error_2,
                                        name,
                                        waterLevel,
                                        lastErrorContent);

                        throw new HydraRingFileParserException(lastErrorContent);
                    }
                }
                finally
                {
                    log.InfoFormat(Resources.WaveConditionsCalculationService_CalculateWaterLevel_Calculation_temporary_directory_can_be_found_on_location_0, calculator.OutputDirectory);
                }
            }
        }

        private static WaveConditionsCosineCalculationInput CreateInput(RoundedDouble waterLevel,
                                                                        RoundedDouble a,
                                                                        RoundedDouble b,
                                                                        RoundedDouble c,
                                                                        double norm,
                                                                        WaveConditionsInput input)
        {
            return new WaveConditionsCosineCalculationInput(1,
                                                            input.Orientation,
                                                            input.HydraulicBoundaryLocation.Id,
                                                            norm,
                                                            HydraRingInputParser.ParseForeshore(input),
                                                            HydraRingInputParser.ParseBreakWater(input),
                                                            waterLevel,
                                                            a,
                                                            b,
                                                            c);
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
                return string.Format(RingtoetsCommonServiceResources.Validation_ValidateInput_No_value_entered_for_ParameterName_0_,
                                     ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.Orientation_DisplayName));
            }

            return null;
        }
    }
}