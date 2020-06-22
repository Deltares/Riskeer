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
using System.Linq;
using Core.Common.Base.Data;
using log4net;
using Riskeer.Common.Service;
using Riskeer.MacroStabilityInwards.CalculatedInput;
using Riskeer.MacroStabilityInwards.CalculatedInput.Converters;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Output;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels;
using Riskeer.MacroStabilityInwards.Primitives;
using Riskeer.MacroStabilityInwards.Service.Converters;
using Riskeer.MacroStabilityInwards.Service.Properties;
using RiskeerCommonServiceResources = Riskeer.Common.Service.Properties.Resources;

namespace Riskeer.MacroStabilityInwards.Service
{
    /// <summary>
    /// This class is responsible for invoking operations on the <see cref="UpliftVanCalculator"/>. Error and status information is 
    /// logged during the execution of the operation.
    /// </summary>
    public static class MacroStabilityInwardsCalculationService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MacroStabilityInwardsCalculationService));

        /// <summary>
        /// Performs validation over the values on the given <paramref name="calculation"/>. Error and status information is logged during
        /// the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="MacroStabilityInwardsCalculation"/> for which to validate the values.</param>
        /// <param name="normativeAssessmentLevel">The normative assessment level to use in case the manual assessment level is not applicable.</param>
        /// <returns><c>false</c> if <paramref name="calculation"/> contains validation errors; <c>true</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/> is <c>null</c>.</exception>
        public static bool Validate(MacroStabilityInwardsCalculation calculation, RoundedDouble normativeAssessmentLevel)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            CalculationServiceHelper.LogValidationBegin();

            string[] inputValidationResults = MacroStabilityInwardsInputValidator.Validate(calculation.InputParameters, normativeAssessmentLevel).ToArray();

            if (inputValidationResults.Length > 0)
            {
                CalculationServiceHelper.LogMessagesAsError(inputValidationResults);
                CalculationServiceHelper.LogValidationEnd();

                return false;
            }

            MacroStabilityInwardsKernelMessage[] waternetExtremeKernelMessages;
            MacroStabilityInwardsKernelMessage[] waternetDailyKernelMessages;

            try
            {
                log.Info(Resources.MacroStabilityInwardsCalculationService_Validate_Waternet_extreme_validation_started);
                waternetExtremeKernelMessages = WaternetCalculationService.ValidateExtreme(calculation.InputParameters, normativeAssessmentLevel).ToArray();
                LogKernelMessages(waternetExtremeKernelMessages);
                
                log.Info(Resources.MacroStabilityInwardsCalculationService_Validate_Waternet_daily_validation_started);
                waternetDailyKernelMessages = WaternetCalculationService.ValidateDaily(calculation.InputParameters).ToArray();
                LogKernelMessages(waternetDailyKernelMessages);
            }
            catch (WaternetCalculationException e)
            {
                CalculationServiceHelper.LogExceptionAsError(Resources.MacroStabilityInwardsCalculationService_Validate_Error_in_MacroStabilityInwards_validation, e);
                CalculationServiceHelper.LogValidationEnd();

                return false;
            }

            MacroStabilityInwardsKernelMessage[] kernelMessages =
                {};

            if (!waternetExtremeKernelMessages.Any() && !waternetDailyKernelMessages.Any())
            {
                UpliftVanCalculatorInput upliftVanCalculatorInput = CreateInputFromData(calculation.InputParameters, normativeAssessmentLevel);
                IUpliftVanCalculator calculator = MacroStabilityInwardsCalculatorFactory.Instance.CreateUpliftVanCalculator(upliftVanCalculatorInput, MacroStabilityInwardsKernelWrapperFactory.Instance);

                try
                {
                    kernelMessages = calculator.Validate().ToArray();
                }
                catch (UpliftVanCalculatorException e)
                {
                    CalculationServiceHelper.LogExceptionAsError(Resources.MacroStabilityInwardsCalculationService_Validate_Error_in_MacroStabilityInwards_validation, e);
                    CalculationServiceHelper.LogValidationEnd();

                    return false;
                }
            }

            LogKernelMessages(kernelMessages);

            CalculationServiceHelper.LogValidationEnd();

            return kernelMessages.Concat(waternetDailyKernelMessages)
                                 .Concat(waternetExtremeKernelMessages)
                                 .All(r => r.Type != MacroStabilityInwardsKernelMessageType.Error);
        }

        /// <summary>
        /// Performs a macro stability inwards calculation based on the supplied <see cref="MacroStabilityInwardsCalculation"/>
        /// and sets <see cref="MacroStabilityInwardsCalculation.Output"/> based on the result if the calculation was successful.
        /// Error and status information is logged during the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="MacroStabilityInwardsCalculation"/> to base the input for the calculation upon.</param>
        /// <param name="normativeAssessmentLevel">The normative assessment level to use in case the manual assessment level is not applicable.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/> is <c>null</c>.</exception>
        /// <exception cref="UpliftVanCalculatorException">Thrown when an error (both expected or unexpected) occurred during the calculation.</exception>
        /// <remarks>Consider calling <see cref="Validate"/> first to see if calculation is possible.</remarks>
        public static void Calculate(MacroStabilityInwardsCalculation calculation, RoundedDouble normativeAssessmentLevel)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            UpliftVanCalculatorResult macroStabilityInwardsResult;

            CalculationServiceHelper.LogCalculationBegin();

            try
            {
                IUpliftVanCalculator calculator = MacroStabilityInwardsCalculatorFactory.Instance.CreateUpliftVanCalculator(
                    CreateInputFromData(calculation.InputParameters, normativeAssessmentLevel),
                    MacroStabilityInwardsKernelWrapperFactory.Instance);

                macroStabilityInwardsResult = calculator.Calculate();
            }
            catch (UpliftVanCalculatorException e)
            {
                CalculationServiceHelper.LogExceptionAsError(RiskeerCommonServiceResources.CalculationService_Calculate_unexpected_error, e);
                CalculationServiceHelper.LogCalculationEnd();

                throw;
            }

            if (macroStabilityInwardsResult.CalculationMessages.Any(cm => cm.Type == MacroStabilityInwardsKernelMessageType.Error))
            {
                CalculationServiceHelper.LogMessagesAsError(new[]
                {
                    CreateAggregatedLogMessage(Resources.MacroStabilityInwardsCalculationService_Calculate_Errors_in_MacroStabilityInwards_calculation, macroStabilityInwardsResult)
                });

                CalculationServiceHelper.LogCalculationEnd();

                throw new UpliftVanCalculatorException();
            }

            if (macroStabilityInwardsResult.CalculationMessages.Any())
            {
                CalculationServiceHelper.LogMessagesAsWarning(new[]
                {
                    CreateAggregatedLogMessage(Resources.MacroStabilityInwardsCalculationService_Calculate_Warnings_in_MacroStabilityInwards_calculation, macroStabilityInwardsResult)
                });
            }

            calculation.Output = new MacroStabilityInwardsOutput(
                MacroStabilityInwardsSlidingCurveConverter.Convert(macroStabilityInwardsResult.SlidingCurveResult),
                MacroStabilityInwardsSlipPlaneUpliftVanConverter.Convert(macroStabilityInwardsResult.CalculationGridResult),
                new MacroStabilityInwardsOutput.ConstructionProperties
                {
                    FactorOfStability = macroStabilityInwardsResult.FactorOfStability,
                    ForbiddenZonesXEntryMin = macroStabilityInwardsResult.ForbiddenZonesXEntryMin,
                    ForbiddenZonesXEntryMax = macroStabilityInwardsResult.ForbiddenZonesXEntryMax
                });

            CalculationServiceHelper.LogCalculationEnd();
        }

        private static void LogKernelMessages(MacroStabilityInwardsKernelMessage[] kernelMessages)
        {
            CalculationServiceHelper.LogMessagesAsError(kernelMessages.Where(msg => msg.Type == MacroStabilityInwardsKernelMessageType.Error)
                                                                      .Select(msg => msg.Message).ToArray());
            CalculationServiceHelper.LogMessagesAsWarning(kernelMessages.Where(msg => msg.Type == MacroStabilityInwardsKernelMessageType.Warning)
                                                                        .Select(msg => msg.Message).ToArray());
        }

        private static UpliftVanCalculatorInput CreateInputFromData(MacroStabilityInwardsInput inputParameters, RoundedDouble normativeAssessmentLevel)
        {
            RoundedDouble effectiveAssessmentLevel = inputParameters.UseAssessmentLevelManualInput
                                                         ? inputParameters.AssessmentLevel
                                                         : normativeAssessmentLevel;

            return new UpliftVanCalculatorInput(
                new UpliftVanCalculatorInput.ConstructionProperties
                {
                    WaternetCreationMode = WaternetCreationMode.CreateWaternet,
                    PlLineCreationMethod = PlLineCreationMethod.RingtoetsWti2017,
                    AssessmentLevel = effectiveAssessmentLevel,
                    SurfaceLine = inputParameters.SurfaceLine,
                    SoilProfile = SoilProfileConverter.Convert(inputParameters.SoilProfileUnderSurfaceLine),
                    DrainageConstruction = DrainageConstructionConverter.Convert(inputParameters),
                    PhreaticLineOffsetsExtreme = PhreaticLineOffsetsConverter.Convert(inputParameters.LocationInputExtreme),
                    PhreaticLineOffsetsDaily = PhreaticLineOffsetsConverter.Convert(inputParameters.LocationInputDaily),
                    SlipPlane = UpliftVanSlipPlaneConverter.Convert(inputParameters),
                    SlipPlaneConstraints = UpliftVanSlipPlaneConstraintsConverter.Convert(inputParameters),
                    DikeSoilScenario = inputParameters.DikeSoilScenario,
                    WaterLevelRiverAverage = inputParameters.WaterLevelRiverAverage,
                    WaterLevelPolderExtreme = inputParameters.LocationInputExtreme.WaterLevelPolder,
                    WaterLevelPolderDaily = inputParameters.LocationInputDaily.WaterLevelPolder,
                    MinimumLevelPhreaticLineAtDikeTopRiver = inputParameters.MinimumLevelPhreaticLineAtDikeTopRiver,
                    MinimumLevelPhreaticLineAtDikeTopPolder = inputParameters.MinimumLevelPhreaticLineAtDikeTopPolder,
                    LeakageLengthOutwardsPhreaticLine3 = inputParameters.LeakageLengthOutwardsPhreaticLine3,
                    LeakageLengthInwardsPhreaticLine3 = inputParameters.LeakageLengthInwardsPhreaticLine3,
                    LeakageLengthOutwardsPhreaticLine4 = inputParameters.DikeSoilScenario != MacroStabilityInwardsDikeSoilScenario.ClayDikeOnSand
                                                             ? inputParameters.LeakageLengthOutwardsPhreaticLine4
                                                             : 1.0,
                    LeakageLengthInwardsPhreaticLine4 = inputParameters.DikeSoilScenario != MacroStabilityInwardsDikeSoilScenario.ClayDikeOnSand
                                                            ? inputParameters.LeakageLengthInwardsPhreaticLine4
                                                            : 1.0,
                    PiezometricHeadPhreaticLine2Outwards = inputParameters.PiezometricHeadPhreaticLine2Outwards,
                    PiezometricHeadPhreaticLine2Inwards = inputParameters.PiezometricHeadPhreaticLine2Inwards,
                    PenetrationLengthExtreme = inputParameters.LocationInputExtreme.PenetrationLength,
                    PenetrationLengthDaily = inputParameters.LocationInputDaily.PenetrationLength,
                    AdjustPhreaticLine3And4ForUplift = inputParameters.AdjustPhreaticLine3And4ForUplift,
                    MoveGrid = inputParameters.MoveGrid,
                    MaximumSliceWidth = inputParameters.MaximumSliceWidth
                });
        }

        private static string CreateAggregatedLogMessage(string baseMessage, UpliftVanCalculatorResult macroStabilityInwardsResult)
        {
            return baseMessage
                   + Environment.NewLine
                   + macroStabilityInwardsResult.CalculationMessages
                                                .Aggregate(string.Empty, (current, logMessage) => current + $"* {logMessage.Message}{Environment.NewLine}").Trim();
        }
    }
}