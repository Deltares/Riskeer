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
using System.Linq;
using Core.Common.Base.Data;
using Ringtoets.Common.Service;
using Ringtoets.MacroStabilityInwards.CalculatedInput.Converters;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Output;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Kernels;
using Ringtoets.MacroStabilityInwards.Service.Converters;
using Ringtoets.MacroStabilityInwards.Service.Properties;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;

namespace Ringtoets.MacroStabilityInwards.Service
{
    /// <summary>
    /// This class is responsible for invoking operations on the <see cref="UpliftVanCalculator"/>. Error and status information is 
    /// logged during the execution of the operation.
    /// </summary>
    public static class MacroStabilityInwardsCalculationService
    {
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

            UpliftVanCalculatorInput upliftVanCalculatorInput = CreateInputFromData(calculation.InputParameters, normativeAssessmentLevel);
            IUpliftVanCalculator calculator = MacroStabilityInwardsCalculatorFactory.Instance.CreateUpliftVanCalculator(upliftVanCalculatorInput, MacroStabilityInwardsKernelWrapperFactory.Instance);

            UpliftVanKernelMessage[] kernelMessages;
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

            CalculationServiceHelper.LogMessagesAsError(kernelMessages.Where(msg => msg.ResultType == UpliftVanKernelMessageType.Error)
                                                                      .Select(msg => msg.Message).ToArray());
            CalculationServiceHelper.LogMessagesAsWarning(kernelMessages.Where(msg => msg.ResultType == UpliftVanKernelMessageType.Warning)
                                                                        .Select(msg => msg.Message).ToArray());
            CalculationServiceHelper.LogValidationEnd();

            return kernelMessages.All(r => r.ResultType != UpliftVanKernelMessageType.Error);
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
                CalculationServiceHelper.LogExceptionAsError(RingtoetsCommonServiceResources.CalculationService_Calculate_unexpected_error, e);
                CalculationServiceHelper.LogCalculationEnd();

                throw;
            }

            if (macroStabilityInwardsResult.CalculationMessages.Any(cm => cm.ResultType == UpliftVanKernelMessageType.Error))
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
                    ZValue = macroStabilityInwardsResult.ZValue,
                    ForbiddenZonesXEntryMin = macroStabilityInwardsResult.ForbiddenZonesXEntryMin,
                    ForbiddenZonesXEntryMax = macroStabilityInwardsResult.ForbiddenZonesXEntryMax
                });

            CalculationServiceHelper.LogCalculationEnd();
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
                    LandwardDirection = LandwardDirection.PositiveX,
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
                    LeakageLengthOutwardsPhreaticLine4 = inputParameters.LeakageLengthOutwardsPhreaticLine4,
                    LeakageLengthInwardsPhreaticLine4 = inputParameters.LeakageLengthInwardsPhreaticLine4,
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