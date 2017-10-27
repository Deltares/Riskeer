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
using Core.Common.Base.Geometry;
using Ringtoets.Common.Service;
using Ringtoets.Common.Service.ValidationRules;
using Ringtoets.MacroStabilityInwards.CalculatedInput.Converters;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Output;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Kernels;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.MacroStabilityInwards.Service.Converters;
using Ringtoets.MacroStabilityInwards.Service.Properties;
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
                CalculationServiceHelper.LogMessagesAsError(inputValidationResults);
                CalculationServiceHelper.LogValidationEnd();
                return false;
            }

            UpliftVanCalculatorInput upliftVanCalculatorInput = CreateInputFromData(calculation.InputParameters);
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
        /// Performs a macro stability inwards calculation based on the supplied <see cref="MacroStabilityInwardsCalculation"/> and sets <see cref="MacroStabilityInwardsCalculation.Output"/>
        /// based on the result if the calculation was successful. Error and status information is logged during
        /// the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="MacroStabilityInwardsCalculation"/> to base the input for the calculation upon.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/> is <c>null</c>.</exception>
        /// <remarks>Consider calling <see cref="Validate"/> first to see if calculation is possible.</remarks>
        /// <exception cref="UpliftVanCalculatorException">Thrown when an error occurred during the calculation.</exception>
        public static void Calculate(MacroStabilityInwardsCalculation calculation)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            CalculationServiceHelper.LogCalculationBegin();

            try
            {
                IUpliftVanCalculator calculator = MacroStabilityInwardsCalculatorFactory.Instance.CreateUpliftVanCalculator(
                    CreateInputFromData(calculation.InputParameters),
                    MacroStabilityInwardsKernelWrapperFactory.Instance);
                UpliftVanCalculatorResult macroStabilityInwardsResult = calculator.Calculate();

                if (macroStabilityInwardsResult.CalculationMessages.Any(cm => cm.ResultType == UpliftVanKernelMessageType.Error))
                {
                    CalculationServiceHelper.LogMessagesAsError(macroStabilityInwardsResult.CalculationMessages
                                                                                           .Where(cm => cm.ResultType == UpliftVanKernelMessageType.Error)
                                                                                           .Select(cm => cm.Message).ToArray());
                }
                else
                {
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

                if (macroStabilityInwardsResult.CalculationMessages.Any(cm => cm.ResultType == UpliftVanKernelMessageType.Warning))
                {
                    CalculationServiceHelper.LogMessagesAsWarning(new[]
                    {
                        Resources.MacroStabilityInwardsCalculationService_Calculate_Warnings_in_MacroStabilityInwards_calculation + Environment.NewLine +
                        macroStabilityInwardsResult.CalculationMessages
                                                   .Where(cm => cm.ResultType == UpliftVanKernelMessageType.Warning)
                                                   .Aggregate(string.Empty, (current, logMessage) => current + $"* {logMessage.Message}{Environment.NewLine}").Trim()
                    });
                }
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

            IEnumerable<string> coreValidationError = ValidateCoreSurfaceLineAndSoilProfileProperties(inputParameters).ToArray();
            validationResults.AddRange(coreValidationError);

            if (!coreValidationError.Any())
            {
                validationResults.AddRange(ValidateSoilLayers(inputParameters));
            }

            return validationResults;
        }

        private static IEnumerable<string> ValidateSoilLayers(MacroStabilityInwardsInput inputParameters)
        {
            var soilProfile1D = inputParameters.StochasticSoilProfile.SoilProfile as MacroStabilityInwardsSoilProfile1D;
            if (soilProfile1D != null)
            {
                if (!ValidateTopOfProfileExceedsSurfaceLineTop(inputParameters, soilProfile1D))
                {
                    yield return Resources.MacroStabilityInwardsCalculationService_ValidateInput_SoilLayerTop_must_be_larger_than_SurfaceLineTop;
                }
                yield break;
            }

            var soilProfile2D = inputParameters.StochasticSoilProfile.SoilProfile as MacroStabilityInwardsSoilProfile2D;
            if (soilProfile2D != null)
            {
                if (!ValidateSurfaceLineIsNearSoilProfile(inputParameters, soilProfile2D))
                {
                    yield return Resources.MacroStabilityInwardsCalculationService_ValidateInput_SurfaceLine_must_be_on_SoilLayer;
                }
            }
        }

        private static bool ValidateTopOfProfileExceedsSurfaceLineTop(IMacroStabilityInwardsWaternetInput inputParameters,
                                                                      MacroStabilityInwardsSoilProfile1D soilProfile1D)
        {
            double layerTop = soilProfile1D.Layers.Max(l => l.Top);
            double surfaceLineTop = inputParameters.SurfaceLine.LocalGeometry.Max(p => p.Y);

            return layerTop + 0.05 >= surfaceLineTop;
        }

        private static bool ValidateSurfaceLineIsNearSoilProfile(MacroStabilityInwardsInput inputParameters,
                                                                 MacroStabilityInwardsSoilProfile2D soilProfile2D)
        {
            IEnumerable<double> uniqueXs = GetDistinctXFromCoordinates(inputParameters.SurfaceLine.LocalGeometry,
                                                                       soilProfile2D);

            IEnumerable<Point2D> surfaceLineWithInterpolations = GetSurfaceLineWithInterpolations(inputParameters, uniqueXs);

            foreach (Point2D surfaceLinePoint in surfaceLineWithInterpolations)
            {
                bool isNear = soilProfile2D.Layers.Any(l => IsPointNearSoilSegments(
                                                           surfaceLinePoint,
                                                           Math2D.ConvertLinePointsToLineSegments(l.OuterRing.Points)));
                if (!isNear)
                {
                    return false;
                }
            }
            return true;
        }

        private static IEnumerable<double> GetDistinctXFromCoordinates(IEnumerable<Point2D> surfaceLinePoints,
                                                                       MacroStabilityInwardsSoilProfile2D soilProfile2D)
        {
            return surfaceLinePoints.Select(p => p.X)
                                    .Concat(soilProfile2D.Layers
                                                         .SelectMany(l => l.OuterRing
                                                                           .Points
                                                                           .Select(outerRingPoint => outerRingPoint.X))
                                    ).OrderBy(d => d)
                                    .Distinct();
        }

        private static IEnumerable<Point2D> GetSurfaceLineWithInterpolations(MacroStabilityInwardsInput inputParameters,
                                                                             IEnumerable<double> uniqueXs)
        {
            Segment2D[] surfaceLineSegments = Math2D.ConvertLinePointsToLineSegments(inputParameters.SurfaceLine.LocalGeometry).ToArray();

            foreach (double x in uniqueXs)
            {
                double y = surfaceLineSegments.Interpolate(x);
                yield return new Point2D(x, y);
            }
        }

        private static bool IsPointNearSoilSegments(Point2D surfaceLinePoint, IEnumerable<Segment2D> soilSegments)
        {
            foreach (Segment2D soilSegment in soilSegments.Where(s => !s.IsVertical()))
            {
                double distance = soilSegment.GetEuclideanDistanceToPoint(surfaceLinePoint);

                if ((distance - 0.05).CompareTo(1e-6) < 1)
                {
                    return true;
                }
            }
            return false;
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
                    WaternetCreationMode = WaternetCreationMode.CreateWaternet,
                    PlLineCreationMethod = PlLineCreationMethod.RingtoetsWti2017,
                    AssessmentLevel = inputParameters.AssessmentLevel,
                    LandwardDirection = LandwardDirection.PositiveX,
                    SurfaceLine = inputParameters.SurfaceLine,
                    SoilProfile = SoilProfileConverter.Convert(inputParameters.SoilProfileUnderSurfaceLine),
                    DrainageConstruction = DrainageConstructionConverter.Convert(inputParameters),
                    PhreaticLineOffsetsExtreme = PhreaticLineOffsetsConverter.Convert(inputParameters.LocationInputExtreme),
                    PhreaticLineOffsetsDaily = PhreaticLineOffsetsConverter.Convert(inputParameters.LocationInputDaily),
                    SlipPlane = UpliftVanSlipPlaneConverter.Convert(inputParameters),
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
                    MaximumSliceWidth = inputParameters.MaximumSliceWidth,
                    CreateZones = inputParameters.CreateZones,
                    AutomaticForbiddenZones = inputParameters.ZoningBoundariesDeterminationType == MacroStabilityInwardsZoningBoundariesDeterminationType.Automatic,
                    SlipPlaneMinimumDepth = inputParameters.SlipPlaneMinimumDepth,
                    SlipPlaneMinimumLength = inputParameters.SlipPlaneMinimumLength
                });
        }
    }
}