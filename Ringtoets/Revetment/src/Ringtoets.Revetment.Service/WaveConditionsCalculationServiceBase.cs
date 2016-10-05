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
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.WaveConditions;
using Ringtoets.HydraRing.Calculation.Parsers;
using Ringtoets.HydraRing.IO;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Service.Properties;
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

        public void Cancel()
        {
            if (calculator != null)
            {
                calculator.Cancel();
            }
            Canceled = true;
        }

        protected bool Canceled { get; set; }

        protected static bool ValidateWaveConditionsInput(WaveConditionsInput waveConditionsInput, string name, string hydraulicBoundaryDatabaseFilePath, string calculatedValueName)
        {
            if (calculatedValueName == null)
            {
                throw new ArgumentNullException("calculatedValueName");
            }

            CalculationServiceHelper.LogValidationBeginTime(name);

            string[] messages = ValidateInput(hydraulicBoundaryDatabaseFilePath, waveConditionsInput, calculatedValueName);

            CalculationServiceHelper.LogMessagesAsError(RingtoetsCommonServiceResources.Error_in_validation_0, messages);

            CalculationServiceHelper.LogValidationEndTime(name);

            return !messages.Any();
        }

        protected IEnumerable<WaveConditionsOutput> CalculateWaveConditions(string calculationName, WaveConditionsInput waveConditionsInput, RoundedDouble a, RoundedDouble b, RoundedDouble c, double norm, string ringId, string hlcdFilePath)
        {
            var outputs = new List<WaveConditionsOutput>();

            var waterLevels = waveConditionsInput.WaterLevels.ToArray();
            foreach (var waterLevel in waterLevels)
            {
                if (Canceled)
                {
                    break;
                }

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

        protected static string[] ValidateInput(string hydraulicBoundaryDatabaseFilePath,
                                              WaveConditionsInput input,
                                              string calculatedValueName)
        {
            List<string> validationResult = new List<string>();

            string validationProblem = HydraulicDatabaseHelper.ValidatePathForCalculation(hydraulicBoundaryDatabaseFilePath);
            if (!string.IsNullOrEmpty(validationProblem))
            {
                validationResult.Add(validationProblem);
            }
            else
            {
                string message = ValidateWaveConditionsInput(input, calculatedValueName);
                if (!string.IsNullOrEmpty(message))
                {
                    validationResult.Add(message);
                }
            }

            return validationResult.ToArray();
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
        /// <returns>A <see cref="WaveConditionsOutput"/> if the calcultion was succesful; or <c>null</c> if it was canceled.</returns>
        private WaveConditionsOutput CalculateWaterLevel(RoundedDouble waterLevel, double a, double b, double c, double norm, WaveConditionsInput input, string hlcdDirectory, string ringId, string name)
        {
            calculator = HydraRingCalculatorFactory.Instance.CreateWaveConditionsCosineCalculator(hlcdDirectory, ringId);
            WaveConditionsCosineCalculationInput calculationInput = CreateInput(waterLevel, a, b, c, norm, input);

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
                    log.ErrorFormat(CultureInfo.CurrentCulture, Resources.WaveConditionsCalculationService_VerifyWaveConditionsCalculationOutput_Error_in_wave_conditions_calculation_0_for_waterlevel_1, name, waterLevel);
                    throw;
                }
                return null;
            }
        }

        private static WaveConditionsCosineCalculationInput CreateInput(double waterLevel,
                                                                        double a,
                                                                        double b,
                                                                        double c,
                                                                        double norm,
                                                                        WaveConditionsInput input)
        {
            return new WaveConditionsCosineCalculationInput(1,
                                                            input.Orientation,
                                                            input.HydraulicBoundaryLocation.Id,
                                                            norm,
                                                            GetForeshore(input),
                                                            GetBreakWater(input),
                                                            waterLevel,
                                                            a,
                                                            b,
                                                            c);
        }

        private static HydraRingBreakWater GetBreakWater(WaveConditionsInput input)
        {
            return input.UseBreakWater ? new HydraRingBreakWater((int) input.BreakWater.Type, input.BreakWater.Height) : null;
        }

        private static IEnumerable<HydraRingForelandPoint> GetForeshore(WaveConditionsInput input)
        {
            return input.UseForeshore ? input.ForeshoreGeometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)) : new HydraRingForelandPoint[0];
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

            return null;
        }
    }
}