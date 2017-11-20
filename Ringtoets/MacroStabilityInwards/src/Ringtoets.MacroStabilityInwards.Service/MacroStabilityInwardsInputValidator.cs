﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Service;
using Ringtoets.Common.Service.ValidationRules;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.MacroStabilityInwards.Service.Properties;

namespace Ringtoets.MacroStabilityInwards.Service
{
    /// <summary>
    /// Validator for <see cref="MacroStabilityInwardsInput"/> for use in <see cref="MacroStabilityInwardsCalculationService"/>.
    /// </summary>
    public static class MacroStabilityInwardsInputValidator
    {
        /// <summary>
        /// Performs validation over the values on the given <paramref name="inputParameters"/>.
        /// </summary>
        /// <param name="inputParameters">The <see cref="MacroStabilityInwardsInput"/> for which to validate the values.</param>
        /// <returns>Validation errors, if any.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="inputParameters"/> is <c>null</c>.</exception>
        public static IEnumerable<string> Validate(MacroStabilityInwardsInput inputParameters)
        {
            if (inputParameters == null)
            {
                throw new ArgumentNullException(nameof(inputParameters));
            }

            var validationResults = new List<string>();

            validationResults.AddRange(ValidateHydraulics(inputParameters));

            IEnumerable<string> coreValidationError = ValidateCoreSurfaceLineAndSoilProfileProperties(inputParameters).ToArray();
            validationResults.AddRange(coreValidationError);

            if (!coreValidationError.Any())
            {
                validationResults.AddRange(ValidateSoilLayers(inputParameters));
                validationResults.AddRange(ValidateZoneBoundaries(inputParameters));
                validationResults.AddRange(ValidateTangentLines(inputParameters));
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

        private static IEnumerable<string> ValidateZoneBoundaries(MacroStabilityInwardsInput inputParameters)
        {
            if (!inputParameters.CreateZones
                || inputParameters.ZoningBoundariesDeterminationType == MacroStabilityInwardsZoningBoundariesDeterminationType.Automatic)
            {
                yield break;
            }

            RoundedDouble zoneBoundaryLeft = inputParameters.ZoneBoundaryLeft;
            RoundedDouble zoneBoundaryRight = inputParameters.ZoneBoundaryRight;

            if (zoneBoundaryLeft > zoneBoundaryRight)
            {
                yield return Resources.MacroStabilityInwardsInputValidator_ValidateZoneBoundaries_ZoneBoundaries_BoundaryLeft_should_be_smaller_than_or_equal_to_BoundaryRight;
            }

            MacroStabilityInwardsSurfaceLine surfaceLine = inputParameters.SurfaceLine;
            if (!surfaceLine.ValidateInRange(zoneBoundaryLeft) || !surfaceLine.ValidateInRange(zoneBoundaryRight))
            {
                var validityRange = new Range<double>(surfaceLine.LocalGeometry.First().X, surfaceLine.LocalGeometry.Last().X);
                yield return string.Format(Resources.MacroStabilityInwardsInputValidator_ValidateZoneBoundaries_ZoneBoundaries_must_be_in_Range_0,
                                           validityRange.ToString(FormattableConstants.ShowAtLeastOneDecimal, CultureInfo.CurrentCulture));
            }
        }

        private static IEnumerable<string> ValidateTangentLines(MacroStabilityInwardsInput inputParameters)
        {
            if (inputParameters.TangentLineZTop == inputParameters.TangentLineZBottom
                && inputParameters.TangentLineNumber != 1)
            {
                yield return Resources.MacroStabilityInwardsInputValidator_ValidateTangentLines_TangentLineNumber_must_be_one_when_TangentLineTop_equals_TangentLineBottom;
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
                validationResult.AddRange(new NumericInputRule(inputParameters.AssessmentLevel, ParameterNameExtractor.GetFromDisplayName(Common.Forms.Properties.Resources.AssessmentLevel_DisplayName)).Validate());
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
    }
}