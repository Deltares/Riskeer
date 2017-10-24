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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Components.Chart.Data;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Forms.Factories
{
    /// <summary>
    /// Factory for creating arrays of <see cref="Point2D"/> to use in <see cref="ChartData"/>
    /// (created via <see cref="MacroStabilityInwardsChartDataFactory"/>).
    /// </summary>
    internal static class MacroStabilityInwardsChartDataPointsFactory
    {
        /// <summary>
        /// Create surface line points in 2D space based on the provided <paramref name="surfaceLine"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="MacroStabilityInwardsSurfaceLine"/> to create the surface line points for.</param>
        /// <returns>An array of points in 2D space or an empty array when <paramref name="surfaceLine"/> is <c>null</c>.</returns>
        public static Point2D[] CreateSurfaceLinePoints(MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            return surfaceLine?.LocalGeometry.ToArray() ?? new Point2D[0];
        }

        /// <summary>
        /// Create a surface level outside point in 2D space based on the provided <paramref name="surfaceLine"/>.
        /// </summary>
        /// <param name="surfaceLine">The surface line to create the surface level outside point for.</param>
        /// <returns>An array with a surface level outside point in 2D space or an empty array when:
        /// <list type="bullet">
        /// <item><paramref name="surfaceLine"/> is <c>null</c>;</item>
        /// <item>the surface level outside point in <paramref name="surfaceLine"/> is <c>null</c>.</item>
        /// </list>
        /// </returns>
        public static Point2D[] CreateSurfaceLevelOutsidePoint(MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            return GetLocalPointsFromGeometry(surfaceLine, surfaceLine?.SurfaceLevelOutside);
        }

        /// <summary>
        /// Create a dike top at river point in 2D space based on the provided <paramref name="surfaceLine"/>.
        /// </summary>
        /// <param name="surfaceLine">The surface line to create the dike top at river point for.</param>
        /// <returns>An array with a dike top at river point in 2D space or an empty array when:
        /// <list type="bullet">
        /// <item><paramref name="surfaceLine"/> is <c>null</c>;</item>
        /// <item>the dike top at river point in <paramref name="surfaceLine"/> is <c>null</c>.</item>
        /// </list>
        /// </returns>
        public static Point2D[] CreateDikeTopAtRiverPoint(MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            return GetLocalPointsFromGeometry(surfaceLine, surfaceLine?.DikeTopAtRiver);
        }

        /// <summary>
        /// Create a dike toe at river point in 2D space based on the provided <paramref name="surfaceLine"/>.
        /// </summary>
        /// <param name="surfaceLine">The surface line to create the dike toe at river point for.</param>
        /// <returns>An array with a dike toe at river point in 2D space or an empty array when:
        /// <list type="bullet">
        /// <item><paramref name="surfaceLine"/> is <c>null</c>;</item>
        /// <item>the dike toe at river point in <paramref name="surfaceLine"/> is <c>null</c>.</item>
        /// </list>
        /// </returns>
        public static Point2D[] CreateDikeToeAtRiverPoint(MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            return GetLocalPointsFromGeometry(surfaceLine, surfaceLine?.DikeToeAtRiver);
        }

        /// <summary>
        /// Create a dike top at polder point in 2D space based on the provided <paramref name="surfaceLine"/>.
        /// </summary>
        /// <param name="surfaceLine">The surface line to create the dike top at polder point for.</param>
        /// <returns>An array with a dike top at polder point in 2D space or an empty array when:
        /// <list type="bullet">
        /// <item><paramref name="surfaceLine"/> is <c>null</c>;</item>
        /// <item>the dike top at polder point in <paramref name="surfaceLine"/> is <c>null</c>.</item>
        /// </list>
        /// </returns>
        public static Point2D[] CreateDikeTopAtPolderPoint(MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            return GetLocalPointsFromGeometry(surfaceLine, surfaceLine?.DikeTopAtPolder);
        }

        /// <summary>
        /// Create a shoulder base inside point in 2D space based on the provided <paramref name="surfaceLine"/>.
        /// </summary>
        /// <param name="surfaceLine">The surface line to create the shoulder base inside point for.</param>
        /// <returns>An array with a shoulder base inside point in 2D space or an empty array when:
        /// <list type="bullet">
        /// <item><paramref name="surfaceLine"/> is <c>null</c>;</item>
        /// <item>the shoulder base inside point in <paramref name="surfaceLine"/> is <c>null</c>.</item>
        /// </list>
        /// </returns>
        public static Point2D[] CreateShoulderBaseInsidePoint(MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            return GetLocalPointsFromGeometry(surfaceLine, surfaceLine?.ShoulderBaseInside);
        }

        /// <summary>
        /// Create a shoulder top inside point in 2D space based on the provided <paramref name="surfaceLine"/>.
        /// </summary>
        /// <param name="surfaceLine">The surface line to create the shoulder top inside point for.</param>
        /// <returns>An array with a shoulder top inside point in 2D space or an empty array when:
        /// <list type="bullet">
        /// <item><paramref name="surfaceLine"/> is <c>null</c>;</item>
        /// <item>the shoulder top inside point in <paramref name="surfaceLine"/> is <c>null</c>.</item>
        /// </list>
        /// </returns>
        public static Point2D[] CreateShoulderTopInsidePoint(MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            return GetLocalPointsFromGeometry(surfaceLine, surfaceLine?.ShoulderTopInside);
        }

        /// <summary>
        /// Create a dike toe at polder point in 2D space based on the provided <paramref name="surfaceLine"/>.
        /// </summary>
        /// <param name="surfaceLine">The surface line to create the dike toe at polder point for.</param>
        /// <returns>An array with a dike toe at polder point in 2D space or an empty array when:
        /// <list type="bullet">
        /// <item><paramref name="surfaceLine"/> is <c>null</c>;</item>
        /// <item>the dike toe at polder point in <paramref name="surfaceLine"/> is <c>null</c>.</item>
        /// </list>
        /// </returns>
        public static Point2D[] CreateDikeToeAtPolderPoint(MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            return GetLocalPointsFromGeometry(surfaceLine, surfaceLine?.DikeToeAtPolder);
        }

        /// <summary>
        /// Create a ditch dike side point in 2D space based on the provided <paramref name="surfaceLine"/>.
        /// </summary>
        /// <param name="surfaceLine">The surface line to create the ditch dike side point for.</param>
        /// <returns>An array with a ditch dike side point in 2D space or an empty array when:
        /// <list type="bullet">
        /// <item><paramref name="surfaceLine"/> is <c>null</c>;</item>
        /// <item>the ditch dike side point in <paramref name="surfaceLine"/> is <c>null</c>.</item>
        /// </list>
        /// </returns>
        public static Point2D[] CreateDitchDikeSidePoint(MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            return GetLocalPointsFromGeometry(surfaceLine, surfaceLine?.DitchDikeSide);
        }

        /// <summary>
        /// Create a bottom ditch dike side point in 2D space based on the provided <paramref name="surfaceLine"/>.
        /// </summary>
        /// <param name="surfaceLine">The surface line to create the bottom ditch dike side point for.</param>
        /// <returns>An array with a bottom ditch dike side point in 2D space or an empty array when:
        /// <list type="bullet">
        /// <item><paramref name="surfaceLine"/> is <c>null</c>;</item>
        /// <item>the bottom ditch dike side point in <paramref name="surfaceLine"/> is <c>null</c>.</item>
        /// </list>
        /// </returns>
        public static Point2D[] CreateBottomDitchDikeSidePoint(MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            return GetLocalPointsFromGeometry(surfaceLine, surfaceLine?.BottomDitchDikeSide);
        }

        /// <summary>
        /// Create a bottom ditch polder side point in 2D space based on the provided <paramref name="surfaceLine"/>.
        /// </summary>
        /// <param name="surfaceLine">The surface line to create the bottom ditch polder side point for.</param>
        /// <returns>An array with a bottom ditch polder side point in 2D space or an empty array when:
        /// <list type="bullet">
        /// <item><paramref name="surfaceLine"/> is <c>null</c>;</item>
        /// <item>the bottom ditch polder side point in <paramref name="surfaceLine"/> is <c>null</c>.</item>
        /// </list>
        /// </returns>
        public static Point2D[] CreateBottomDitchPolderSidePoint(MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            return GetLocalPointsFromGeometry(surfaceLine, surfaceLine?.BottomDitchPolderSide);
        }

        /// <summary>
        /// Create a surface level inside point in 2D space based on the provided <paramref name="surfaceLine"/>.
        /// </summary>
        /// <param name="surfaceLine">The surface line to create the surface level inside point for.</param>
        /// <returns>An array with a surface level inside point in 2D space or an empty array when:
        /// <list type="bullet">
        /// <item><paramref name="surfaceLine"/> is <c>null</c>;</item>
        /// <item>the surface level inside point in <paramref name="surfaceLine"/> is <c>null</c>.</item>
        /// </list>
        /// </returns>
        public static Point2D[] CreateSurfaceLevelInsidePoint(MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            return GetLocalPointsFromGeometry(surfaceLine, surfaceLine?.SurfaceLevelInside);
        }

        /// <summary>
        /// Create a ditch polder side point in 2D space based on the provided <paramref name="surfaceLine"/>.
        /// </summary>
        /// <param name="surfaceLine">The surface line to create the ditch polder side point for.</param>
        /// <returns>An array with a ditch polder side point in 2D space or an empty array when:
        /// <list type="bullet">
        /// <item><paramref name="surfaceLine"/> is <c>null</c>;</item>
        /// <item>the ditch polder side point in <paramref name="surfaceLine"/> is <c>null</c>.</item>
        /// </list>
        /// </returns>
        public static Point2D[] CreateDitchPolderSidePoint(MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            return GetLocalPointsFromGeometry(surfaceLine, surfaceLine?.DitchPolderSide);
        }

        /// <summary>
        /// Create areas of holes in 2D space based on the provided <paramref name="soilProfile"/>.
        /// </summary>
        /// <param name="soilProfile">The soil profile to create the holes for.</param>
        /// <returns>An array with an array of points in 2D space or an empty array when
        /// <paramref name="soilProfile"/> is <c>null</c>.</returns>
        public static IEnumerable<Point2D[]> CreateHolesAreas(IMacroStabilityInwardsSoilProfileUnderSurfaceLine soilProfile)
        {
            return soilProfile?.Layers.SelectMany(l => l.Holes).ToArray() ?? new Point2D[0][];
        }

        /// <summary>
        /// Create an area of the outer ring in 2D space based on the provided <paramref name="soilLayer"/>.
        /// </summary>
        /// <param name="soilLayer">The soil layer to create the outer ring for.</param>
        /// <returns>A collection containing a single array of points in 2D space 
        /// or an empty collection when <paramref name="soilLayer"/> is <c>null</c>.</returns>
        public static IEnumerable<Point2D[]> CreateOuterRingArea(IMacroStabilityInwardsSoilLayerUnderSurfaceLine soilLayer)
        {
            return soilLayer != null
                       ? new[]
                       {
                           soilLayer.OuterRing
                       }
                       : new Point2D[0][];
        }

        /// <summary>
        /// Create points of the phreatic line in 2D space based on the provided <paramref name="phreaticLine"/>.
        /// </summary>
        /// <param name="phreaticLine">The phreatic line to create the points for.</param>
        /// <returns>An array of points in 2D space or an empty array when <paramref name="phreaticLine"/>
        /// is <c>null</c>.</returns>
        public static Point2D[] CreatePhreaticLinePoints(MacroStabilityInwardsPhreaticLine phreaticLine)
        {
            return phreaticLine?.Geometry.ToArray() ?? new Point2D[0];
        }

        /// <summary>
        /// Create points of the waternet zone in 2D space based on the provide <paramref name="waternetLine"/>.
        /// </summary>
        /// <param name="waternetLine">The waternet line to create the zone for.</param>
        /// <param name="surfaceLine">The <see cref="MacroStabilityInwardsSurfaceLine"/> that may intersect with 
        /// the <see cref="MacroStabilityInwardsWaternetLine.PhreaticLine"/> and by doing that restricts the
        /// drawn height of it.</param>
        /// <returns>An array of points in 2D space or an empty array when <paramref name="waternetLine"/> or
        /// <paramref name="surfaceLine"/> is <c>null</c>.</returns>
        public static Point2D[] CreateWaternetZonePoints(MacroStabilityInwardsWaternetLine waternetLine,
                                                         MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            if (waternetLine == null || surfaceLine == null)
            {
                return new Point2D[0];
            }

            Point2D[] phreaticLineGeometry = waternetLine.PhreaticLine.Geometry.ToArray();
            var points = new List<Point2D>(waternetLine.Geometry.Reverse());
            var phreaticLinePoints = new List<Point2D>();

            phreaticLinePoints.AddRange(ClipPhreaticLineGeometryToSurfaceLine(phreaticLineGeometry, surfaceLine));
            phreaticLinePoints.AddRange(GetPhreaticLineIntersectionPointsWithSurfaceLine(phreaticLineGeometry, surfaceLine));
            phreaticLinePoints.AddRange(ClipPhreaticLineSegmentsToSurfaceLine(surfaceLine, phreaticLinePoints));

            points.AddRange(phreaticLinePoints.OrderBy(p => p.X));

            return points.ToArray();
        }

        /// <summary>
        /// Sets an empty area to every element in <paramref name="chartMultipleAreaData"/>.
        /// </summary>
        /// <param name="chartMultipleAreaData">A collection of <see cref="ChartMultipleAreaData"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="chartMultipleAreaData"/>
        /// is <c>null</c>.</exception>
        public static void SetEmptyAreas(IEnumerable<ChartMultipleAreaData> chartMultipleAreaData)
        {
            if (chartMultipleAreaData == null)
            {
                throw new ArgumentNullException(nameof(chartMultipleAreaData));
            }

            foreach (ChartMultipleAreaData t in chartMultipleAreaData)
            {
                t.Areas = Enumerable.Empty<Point2D[]>();
            }
        }

        #region SoilLayers and Surface Line Helpers

        private static Point2D[] GetLocalPointsFromGeometry(MacroStabilityInwardsSurfaceLine surfaceLine,
                                                            Point3D worldCoordinate)
        {
            if (surfaceLine == null || worldCoordinate == null)
            {
                return new Point2D[0];
            }

            return new[]
            {
                surfaceLine.GetLocalPointFromGeometry(worldCoordinate)
            };
        }

        #endregion

        #region Grid Helpers

        /// <summary>
        /// Creates grid points in 2D space based on the provided <paramref name="grid"/>.
        /// </summary>
        /// <param name="grid">The grid to create the grid points for.</param>
        /// <param name="gridDeterminationType">The grid determination type.</param>
        /// <returns>An array of interpolated points in 2D space based on the provided 
        /// <paramref name="grid"/> or an empty array when:
        /// <list type="bullet">
        /// <item><paramref name="grid"/> is <c>null</c>;</item>
        /// <item><paramref name="gridDeterminationType"/> is <see cref="MacroStabilityInwardsGridDeterminationType.Automatic"/>;</item>
        /// <item>The grid boundaries are <see cref="double.NaN"/>.</item>
        /// </list>
        /// </returns>
        public static Point2D[] CreateGridPoints(MacroStabilityInwardsGrid grid, MacroStabilityInwardsGridDeterminationType gridDeterminationType)
        {
            if (grid == null
                || gridDeterminationType == MacroStabilityInwardsGridDeterminationType.Automatic
                || !AreGridSettingsValid(grid))
            {
                return new Point2D[0];
            }

            var points = new List<Point2D>();
            IEnumerable<RoundedDouble> interPolatedVerticalPositions = GetInterPolatedVerticalPositions(grid.ZBottom,
                                                                                                        grid.ZTop,
                                                                                                        grid.NumberOfVerticalPoints);
            foreach (RoundedDouble interPolatedVerticalPosition in interPolatedVerticalPositions)
            {
                points.AddRange(GetInterPolatedHorizontalPoints(grid.XLeft,
                                                                grid.XRight,
                                                                interPolatedVerticalPosition,
                                                                grid.NumberOfHorizontalPoints));
            }

            return points.ToArray();
        }

        private static bool AreGridSettingsValid(MacroStabilityInwardsGrid grid)
        {
            return !double.IsNaN(grid.XLeft)
                   && !double.IsNaN(grid.XRight)
                   && !double.IsNaN(grid.ZTop)
                   && !double.IsNaN(grid.ZBottom);
        }

        private static IEnumerable<RoundedDouble> GetInterPolatedVerticalPositions(RoundedDouble startPoint,
                                                                                   RoundedDouble endPoint,
                                                                                   int nrOfPoints)
        {
            if (nrOfPoints <= 1)
            {
                yield return startPoint;
                yield break;
            }

            int nrofInterPolatedPoints = nrOfPoints - 1;
            RoundedDouble deltaZ = endPoint - startPoint;
            RoundedDouble deltaZBetweenPoints = nrOfPoints < 2
                                                    ? (RoundedDouble) 0.0
                                                    : (RoundedDouble) (deltaZ / nrofInterPolatedPoints);

            RoundedDouble z = startPoint;
            int nrOfRepetitions = nrofInterPolatedPoints < 0
                                      ? 0
                                      : nrofInterPolatedPoints;
            for (var i = 0; i < nrOfRepetitions + 1; i++)
            {
                yield return z;
                z += deltaZBetweenPoints;
            }
        }

        private static IEnumerable<Point2D> GetInterPolatedHorizontalPoints(RoundedDouble startPoint,
                                                                            RoundedDouble endPoint,
                                                                            RoundedDouble zPoint,
                                                                            int nrOfPoints)
        {
            if (nrOfPoints <= 1)
            {
                yield return new Point2D(startPoint, zPoint);
                yield break;
            }

            int nrofInterPolatedPoints = nrOfPoints - 1;
            RoundedDouble deltaX = endPoint - startPoint;
            RoundedDouble deltaXBetweenPoints = nrOfPoints < 2
                                                    ? (RoundedDouble) 0
                                                    : (RoundedDouble) (deltaX / nrofInterPolatedPoints);

            RoundedDouble x = startPoint;
            int nrOfRepetitions = nrofInterPolatedPoints < 0
                                      ? 0
                                      : nrofInterPolatedPoints;
            for (var i = 0; i < nrOfRepetitions + 1; i++)
            {
                yield return new Point2D(x, zPoint);
                x += deltaXBetweenPoints;
            }
        }

        #endregion

        #region Waternet Helpers

        private static IEnumerable<Point2D> ClipPhreaticLineGeometryToSurfaceLine(IEnumerable<Point2D> phreaticLineGeometry,
                                                                                  MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            foreach (Point2D phreaticLinePoint in phreaticLineGeometry)
            {
                double surfaceLineHeight = surfaceLine.GetZAtL((RoundedDouble)phreaticLinePoint.X);
                double height = surfaceLineHeight < phreaticLinePoint.Y ? surfaceLineHeight : phreaticLinePoint.Y;

                yield return new Point2D(phreaticLinePoint.X, height);
            }
        }


        private static IEnumerable<Point2D> GetPhreaticLineIntersectionPointsWithSurfaceLine(IEnumerable<Point2D> phreaticLineGeometry,
                                                                                             MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            IEnumerable<Segment2D> phreaticLineSegments = Math2D.ConvertLinePointsToLineSegments(phreaticLineGeometry);
            IEnumerable<Segment2D> surfacelineSegments = Math2D.ConvertLinePointsToLineSegments(surfaceLine.LocalGeometry);

            foreach (Segment2D phreaticLineSegment in phreaticLineSegments)
            {
                foreach (Segment2D surfaceLineSegment in surfacelineSegments)
                {
                    Segment2DIntersectSegment2DResult intersectionPointResult = Math2D.GetIntersectionBetweenSegments(
                        phreaticLineSegment, surfaceLineSegment);

                    if (intersectionPointResult.IntersectionType == Intersection2DType.Intersects)
                    {
                        yield return intersectionPointResult.IntersectionPoints.First();
                    }
                }
            }
        }

        private static IEnumerable<Point2D> ClipPhreaticLineSegmentsToSurfaceLine(MacroStabilityInwardsSurfaceLine surfaceLine,
                                                                                  List<Point2D> phreaticLinePoints)
        {
            IEnumerable<Segment2D> adaptedPhreaticLineSegments = Math2D.ConvertLinePointsToLineSegments(phreaticLinePoints);

            foreach (Point2D surfaceLinePoint in surfaceLine.LocalGeometry)
            {
                Point2D[] intersectionPoint = Math2D.SegmentsIntersectionWithVerticalLine(adaptedPhreaticLineSegments, surfaceLinePoint.X).ToArray();

                if (intersectionPoint.Any() && intersectionPoint.First().Y >= surfaceLinePoint.Y && !phreaticLinePoints.Contains(surfaceLinePoint))
                {
                    yield return surfaceLinePoint;
                }
            }
        }

        #endregion
    }
}