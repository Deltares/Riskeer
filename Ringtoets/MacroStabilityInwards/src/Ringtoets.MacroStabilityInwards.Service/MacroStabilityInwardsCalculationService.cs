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
using System.Linq;
using Ringtoets.Common.Service;
using Ringtoets.Common.Service.ValidationRules;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Output;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Kernels;
using Ringtoets.MacroStabilityInwards.Service.Converters;
using Ringtoets.MacroStabilityInwards.Service.Properties;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

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
        /// <returns><c>False</c> if <paramref name="calculation"/> contains validation errors; <c>True</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/> is <c>null</c>.</exception>
        public static bool Validate(MacroStabilityInwardsCalculation calculation)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            CalculationServiceHelper.LogValidationBegin();

            string[] inputValidationResults = ValidateInput(calculation.InputParameters).ToArray();

            if (inputValidationResults.Length > 0)
            {
                CalculationServiceHelper.LogMessagesAsError(RingtoetsCommonServiceResources.Error_in_validation_0, inputValidationResults);
                CalculationServiceHelper.LogValidationEnd();
                return false;
            }

            UpliftVanCalculatorInput upliftVanCalculatorInput = CreateInputFromData(calculation.InputParameters);
            IUpliftVanCalculator calculator = MacroStabilityInwardsCalculatorFactory.Instance.CreateUpliftVanCalculator(upliftVanCalculatorInput, MacroStabilityInwardsKernelWrapperFactory.Instance);

            List<UpliftVanValidationResult> validationResults = calculator.Validate();

            CalculationServiceHelper.LogMessagesAsError(RingtoetsCommonServiceResources.Error_in_validation_0,
                                                        validationResults.Where(msg => msg.ResultType == UpliftVanValidationResultType.Error)
                                                                         .Select(msg => msg.Message).ToArray());
            CalculationServiceHelper.LogMessagesAsWarning(validationResults.Where(msg => msg.ResultType == UpliftVanValidationResultType.Warning)
                                                                           .Select(msg => string.Format(RingtoetsCommonServiceResources.Warning_in_validation_0,
                                                                                                        msg.Message)).ToArray());
            CalculationServiceHelper.LogValidationEnd();

            return !validationResults.Select(r => r.ResultType == UpliftVanValidationResultType.Error).Any();
        }

        /// <summary>
        /// Performs a macro stability inwards calculation based on the supplied <see cref="MacroStabilityInwardsCalculation"/> and sets <see cref="MacroStabilityInwardsCalculation.Output"/>
        /// based on the result if the calculation was successful. Error and status information is logged during
        /// the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="MacroStabilityInwardsCalculation"/> to base the input for the calculation upon.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/> is <c>null</c>.</exception>
        /// <remarks>Consider calling <see cref="Validate"/> first to see if calculation is possible.</remarks>
        public static void Calculate(MacroStabilityInwardsCalculation calculation)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            CalculationServiceHelper.LogCalculationBegin();

            try
            {
                IUpliftVanCalculator calculator = MacroStabilityInwardsCalculatorFactory.Instance.CreateUpliftVanCalculator(CreateInputFromData(calculation.InputParameters), MacroStabilityInwardsKernelWrapperFactory.Instance);
                UpliftVanCalculatorResult macroStabilityInwardsResult = calculator.Calculate();

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
            }
            catch (UpliftVanCalculatorException e)
            {
                CalculationServiceHelper.LogExceptionAsError(Resources.MacroStabilityInwardsCalculationService_Calculate_Error_in_MacroStabilityInwards_calculation, e);
                throw;
            }
            finally
            {
                CalculationServiceHelper.LogCalculationEnd();
            }
        }

        private static List<string> ValidateInput(MacroStabilityInwardsInput inputParameters)
        {
            var validationResults = new List<string>();

            validationResults.AddRange(ValidateHydraulics(inputParameters));

            IEnumerable<string> coreValidationError = ValidateCoreSurfaceLineAndSoilProfileProperties(inputParameters);
            validationResults.AddRange(coreValidationError);

            return validationResults;
        }

        private static IEnumerable<string> ValidateHydraulics(MacroStabilityInwardsInput inputParameters)
        {
            var validationResults = new List<string>();
            if (!inputParameters.UseAssessmentLevelManualInput && inputParameters.HydraulicBoundaryLocation == null)
            {
                validationResults.Add(Resources.MacroStabilityInwardsCalculationService_ValidateInput_No_HydraulicBoundaryLocation_selected);
            }
            else
            {
                validationResults.AddRange(ValidateAssessmentLevel(inputParameters));
            }

            return validationResults;
        }

        private static IEnumerable<string> ValidateAssessmentLevel(MacroStabilityInwardsInput inputParameters)
        {
            var validationResult = new List<string>();

            if (inputParameters.UseAssessmentLevelManualInput)
            {
                validationResult.AddRange(new NumericInputRule(inputParameters.AssessmentLevel, ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.AssessmentLevel_DisplayName)).Validate());
            }
            else
            {
                if (double.IsNaN(inputParameters.AssessmentLevel))
                {
                    validationResult.Add(Resources.MacroStabilityInwardsCalculationService_ValidateInput_Cannot_determine_AssessmentLevel);
                }
            }

            return validationResult;
        }

        private static IEnumerable<string> ValidateCoreSurfaceLineAndSoilProfileProperties(MacroStabilityInwardsInput inputParameters)
        {
            var validationResults = new List<string>();
            if (inputParameters.SurfaceLine == null)
            {
                validationResults.Add(Resources.MacroStabilityInwardsCalculationService_ValidateInput_No_SurfaceLine_selected);
            }
            if (inputParameters.StochasticSoilProfile == null)
            {
                validationResults.Add(Resources.MacroStabilityInwardsCalculationService_ValidateInput_No_StochasticSoilProfile_selected);
            }
            return validationResults;
        }

        private static UpliftVanCalculatorInput CreateInputFromData(MacroStabilityInwardsInput inputParameters)
        {
            return new UpliftVanCalculatorInput(
                new UpliftVanCalculatorInput.ConstructionProperties
                {
                    WaternetCreationMode = UpliftVanWaternetCreationMode.CreateWaternet,
                    PlLineCreationMethod = UpliftVanPlLineCreationMethod.RingtoetsWti2017,
                    AssessmentLevel = inputParameters.AssessmentLevel,
                    LandwardDirection = UpliftVanLandwardDirection.PositiveX,
                    SurfaceLine = inputParameters.SurfaceLine,
                    SoilProfile = UpliftVanSoilProfileConverter.Convert(inputParameters.SoilProfileUnderSurfaceLine),
                    DrainageConstruction = UpliftVanDrainageConstructionConverter.Convert(inputParameters),
                    PhreaticLineOffsets = UpliftVanPhreaticLineOffsetsConverter.Convert(inputParameters),
                    SlipPlane = UpliftVanSlipPlaneConverter.Convert(inputParameters),
                    DikeSoilScenario = inputParameters.DikeSoilScenario,
                    WaterLevelRiverAverage = inputParameters.WaterLevelRiverAverage,
                    WaterLevelPolder = inputParameters.WaterLevelPolder,
                    MinimumLevelPhreaticLineAtDikeTopRiver = inputParameters.MinimumLevelPhreaticLineAtDikeTopRiver,
                    MinimumLevelPhreaticLineAtDikeTopPolder = inputParameters.MinimumLevelPhreaticLineAtDikeTopPolder,
                    LeakageLengthOutwardsPhreaticLine3 = inputParameters.LeakageLengthOutwardsPhreaticLine3,
                    LeakageLengthInwardsPhreaticLine3 = inputParameters.LeakageLengthInwardsPhreaticLine3,
                    LeakageLengthOutwardsPhreaticLine4 = inputParameters.LeakageLengthOutwardsPhreaticLine4,
                    LeakageLengthInwardsPhreaticLine4 = inputParameters.LeakageLengthInwardsPhreaticLine4,
                    PiezometricHeadPhreaticLine2Outwards = inputParameters.PiezometricHeadPhreaticLine2Outwards,
                    PiezometricHeadPhreaticLine2Inwards = inputParameters.PiezometricHeadPhreaticLine2Inwards,
                    PenetrationLength = inputParameters.PenetrationLength,
                    AdjustPhreaticLine3And4ForUplift = inputParameters.AdjustPhreaticLine3And4ForUplift,
                    MoveGrid = inputParameters.MoveGrid,
                    MaximumSliceWidth = inputParameters.MaximumSliceWidth,
                    CreateZones = inputParameters.CreateZones,
                    AutomaticForbiddenZones = inputParameters.ZoningBoundariesDeterminationType == MacroStabilityInwardsZoningBoundariesDeterminationType.Automatic,
                    SlipPlaneMinimumDepth = inputParameters.SlipPlaneMinimumDepth,
                    SlipPlaneMinimumLength = inputParameters.SlipPlaneMinimumLength
                });
        }
    }
}