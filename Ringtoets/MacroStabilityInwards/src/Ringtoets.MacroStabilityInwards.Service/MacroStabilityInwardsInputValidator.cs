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
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;

namespace Ringtoets.MacroStabilityInwards.Service
{
    /// <summary>
    /// Validator for <see cref="MacroStabilityInwardsInput"/> for use in <see cref="MacroStabilityInwardsCalculationService"/>.
    /// </summary>
    public static class MacroStabilityInwardsInputValidator
    {
        private const double withinSurfaceLineLevelLimit = 0.05;

        /// <summary>
        /// Performs validation over the values on the given <paramref name="inputParameters"/>.
        /// </summary>
        /// <param name="inputParameters">The <see cref="MacroStabilityInwardsInput"/> for which to validate the values.</param>
        /// <returns>Validation errors, if any.</returns>
        /// <param name="normativeAssessmentLevel">The normative assessment level to use in case the manual assessment level is not applicable.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="inputParameters"/> is <c>null</c>.</exception>
        public static IEnumerable<string> Validate(MacroStabilityInwardsInput inputParameters, RoundedDouble normativeAssessmentLevel)
        {
            if (inputParameters == null)
            {
                throw new ArgumentNullException(nameof(inputParameters));
            }

            var validationResults = new List<string>();

            validationResults.AddRange(ValidateHydraulics(inputParameters, normativeAssessmentLevel));

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
            if (soilProfile2D != null
                && !ValidateSurfaceLineIsNearSoilProfile(inputParameters, soilProfile2D))
            {
                yield return Resources.MacroStabilityInwardsCalculationService_ValidateInput_SurfaceLine_must_be_on_SoilLayer;
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

            return layerTop + withinSurfaceLineLevelLimit >= surfaceLineTop;
        }

        private static bool ValidateSurfaceLineIsNearSoilProfile(MacroStabilityInwardsInput inputParameters,
                                                                 MacroStabilityInwardsSoilProfile2D soilProfile2D)
        {
            IEnumerable<double> discretizedSurfaceLineXCoordinates = GetClippedDiscretizedXCoordinatesOfSurfaceLine(inputParameters.SurfaceLine.LocalGeometry,
                                                                                                                    soilProfile2D);
            IEnumerable<Point2D> surfaceLineWithInterpolations = GetSurfaceLineWithInterpolations(inputParameters, discretizedSurfaceLineXCoordinates);

            IEnumerable<IEnumerable<Segment2D>> layerPolygons = GetLayerPolygons(soilProfile2D);

            foreach (Point2D surfaceLinePoint in surfaceLineWithInterpolations)
            {
                IEnumerable<Point2D> intersectingCoordinates = layerPolygons.SelectMany(lp => Math2D.SegmentsIntersectionWithVerticalLine(lp, surfaceLinePoint.X));
                if (!intersectingCoordinates.Any())
                {
                    return false;
                }

                double maxYCoordinate = intersectingCoordinates.Select(p => p.Y).Max();
                if (Math.Abs(surfaceLinePoint.Y - maxYCoordinate) - withinSurfaceLineLevelLimit >= 1e-5)
                {
                    return false;
                }
            }

            return true;
        }

        private static IEnumerable<double> GetClippedDiscretizedXCoordinatesOfSurfaceLine(IEnumerable<Point2D> surfaceLinePoints,
                                                                                          MacroStabilityInwardsSoilProfile2D soilProfile2D)
        {
            IEnumerable<double> surfaceLineXCoordinates = surfaceLinePoints.Select(p => p.X).ToArray();
            IEnumerable<double> soilProfileXCoordinates = GetSoilProfile2DXCoordinates(soilProfile2D).ToArray();

            double maximumXCoordinateSurfaceLine = surfaceLineXCoordinates.Max();
            double maximumXCoordinateSoilProfile = soilProfileXCoordinates.Max();
            double maxXCoordinate = Math.Min(maximumXCoordinateSoilProfile, maximumXCoordinateSurfaceLine);

            double minimumXCoordinateSurfaceLine = surfaceLineXCoordinates.Min();
            double minimumXCoordinateSoilProfile = soilProfileXCoordinates.Min();
            double minXCoordinate = Math.Max(minimumXCoordinateSoilProfile, minimumXCoordinateSurfaceLine);

            IEnumerable<double> clippedSoilProfileXCoordinates = soilProfileXCoordinates.Where(xCoordinate => IsXCoordinateInRange(xCoordinate, minXCoordinate, maxXCoordinate));
            IEnumerable<double> clippedSurfaceLineXCoordinates = surfaceLineXCoordinates.Where(xCoordinate => IsXCoordinateInRange(xCoordinate, minXCoordinate, maxXCoordinate));

            double[] uniqueClippedXCoordinates = clippedSoilProfileXCoordinates.Concat(clippedSurfaceLineXCoordinates)
                                                                               .Distinct()
                                                                               .OrderBy(xCoordinate => xCoordinate)
                                                                               .ToArray();

            var xCoordinates = new List<double>();
            for (var i = 0; i < uniqueClippedXCoordinates.Length - 1; i++)
            {
                double firstXCoordinate = uniqueClippedXCoordinates[i];
                double secondXCoordinate = uniqueClippedXCoordinates[i + 1];

                xCoordinates.AddRange(GetDiscretizedXCoordinatesBetweenInterval(firstXCoordinate, secondXCoordinate));
            }

            xCoordinates.Add(uniqueClippedXCoordinates.Last());

            return xCoordinates;
        }

        private static IEnumerable<double> GetDiscretizedXCoordinatesBetweenInterval(double startXCoordinate, double endXCoordinate)
        {
            double xCoordinate = startXCoordinate;
            var discretizedXCoordinates = new List<double>();
            while (xCoordinate < endXCoordinate)
            {
                discretizedXCoordinates.Add(xCoordinate);
                xCoordinate += withinSurfaceLineLevelLimit;
            }

            return discretizedXCoordinates;
        }

        private static bool IsXCoordinateInRange(double xCoordinate, double minXCoordinate, double maxXCoordinate)
        {
            return xCoordinate <= maxXCoordinate
                   && xCoordinate >= minXCoordinate;
        }

        private static IEnumerable<IEnumerable<Segment2D>> GetLayerPolygons(MacroStabilityInwardsSoilProfile2D soilProfile2D)
        {
            return soilProfile2D.Layers
                                .Select(l => Math2D.ConvertPointsToPolygonSegments(l.OuterRing.Points))
                                .ToArray();
        }

        private static IEnumerable<double> GetSoilProfile2DXCoordinates(MacroStabilityInwardsSoilProfile2D soilProfile2D)
        {
            return soilProfile2D.Layers
                                .SelectMany(l => l.OuterRing
                                                  .Points
                                                  .Select(outerRingPoint => outerRingPoint.X));
        }

        private static IEnumerable<Point2D> GetSurfaceLineWithInterpolations(MacroStabilityInwardsInput inputParameters,
                                                                             IEnumerable<double> uniqueXs)
        {
            Segment2D[] surfaceLineSegments = Math2D.ConvertPointsToLineSegments(inputParameters.SurfaceLine.LocalGeometry).ToArray();

            foreach (double x in uniqueXs)
            {
                double y = surfaceLineSegments.Interpolate(x);
                yield return new Point2D(x, y);
            }
        }

        private static IEnumerable<string> ValidateHydraulics(MacroStabilityInwardsInput inputParameters, RoundedDouble normativeAssessmentLevel)
        {
            var validationResults = new List<string>();
            if (!inputParameters.UseAssessmentLevelManualInput && inputParameters.HydraulicBoundaryLocation == null)
            {
                validationResults.Add(RingtoetsCommonServiceResources.CalculationService_ValidateInput_No_hydraulic_boundary_location_selected);
            }
            else
            {
                validationResults.AddRange(ValidateAssessmentLevel(inputParameters, normativeAssessmentLevel));
            }

            return validationResults;
        }

        private static IEnumerable<string> ValidateAssessmentLevel(MacroStabilityInwardsInput inputParameters, RoundedDouble normativeAssessmentLevel)
        {
            var validationResult = new List<string>();

            if (inputParameters.UseAssessmentLevelManualInput)
            {
                validationResult.AddRange(new NumericInputRule(inputParameters.AssessmentLevel, ParameterNameExtractor.GetFromDisplayName(RingtoetsCommonFormsResources.WaterLevel_DisplayName)).Validate());
            }
            else
            {
                if (double.IsNaN(normativeAssessmentLevel))
                {
                    validationResult.Add(RingtoetsCommonServiceResources.CalculationService_ValidateInput_Cannot_determine_AssessmentLevel);
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