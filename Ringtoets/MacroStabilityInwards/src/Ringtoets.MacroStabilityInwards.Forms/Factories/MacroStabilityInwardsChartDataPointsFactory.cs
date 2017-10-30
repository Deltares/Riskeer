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
        /// Create an area of the outer ring in 2D space based on the provided <paramref name="soilLayer"/>.
        /// </summary>
        /// <param name="soilLayer">The soil layer to create the outer ring for.</param>
        /// <returns>A collection containing a single array of points in 2D space 
        /// or an empty collection when <paramref name="soilLayer"/> is <c>null</c>.</returns>
        public static IEnumerable<Point2D[]> CreateOuterRingArea(MacroStabilityInwardsSoilLayer2D soilLayer)
        {
            return soilLayer != null
                       ? new List<Point2D[]>
                       {
                           soilLayer.OuterRing.Points.ToArray()
                       }
                       : Enumerable.Empty<Point2D[]>();
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
        /// <returns>An array of points in 2D space or an empty array when:
        /// <list type="bullet">
        /// <item><paramref name="waternetLine"/> is <c>null</c>;</item>
        /// <item><paramref name="surfaceLine"/> is <c>null</c>;</item>
        /// <item>The geometry of the <see cref="MacroStabilityInwardsWaternetLine"/> is <c>empty</c>;</item>
        /// <item>The geometry of the <see cref="MacroStabilityInwardsWaternetLine.PhreaticLine"/> is <c>empty</c>.</item>
        /// </list>
        /// </returns>
        public static IEnumerable<Point2D[]> CreateWaternetZonePoints(MacroStabilityInwardsWaternetLine waternetLine,
                                                                      MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            if (waternetLine == null || surfaceLine == null
                || !waternetLine.Geometry.Any() || !waternetLine.PhreaticLine.Geometry.Any())
            {
                return new Point2D[0][];
            }

            Point2D[] surfaceLineLocalGeometry = surfaceLine.LocalGeometry.ToArray();
            Point2D[] phreaticLineGeometry = new RoundedPoint2DCollection(2, waternetLine.PhreaticLine.Geometry).ToArray();
            Point2D[] waternetLineGeometry = new RoundedPoint2DCollection(2, waternetLine.Geometry).ToArray();

            return IsSurfaceLineAboveWaternetZone(surfaceLineLocalGeometry, waternetLineGeometry, phreaticLineGeometry)
                       ? CreateZoneAreas(waternetLineGeometry, phreaticLineGeometry).ToArray()
                       : GetWaternetZoneWithSurfaceLineIntersection(surfaceLineLocalGeometry, waternetLineGeometry, phreaticLineGeometry).ToArray();
        }

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
            return gridDeterminationType == MacroStabilityInwardsGridDeterminationType.Automatic
                       ? new Point2D[0]
                       : CreateGridPoints(grid);
        }

        /// <summary>
        /// Creates grid points in 2D space based on the provided <paramref name="grid"/>.
        /// </summary>
        /// <param name="grid">The grid to create the grid points for.</param>
        /// <returns>An array of interpolated points in 2D space based on the provided 
        /// <paramref name="grid"/> or an empty array when:
        /// <list type="bullet">
        /// <item><paramref name="grid"/> is <c>null</c>;</item>
        /// <item>The grid boundaries are <see cref="double.NaN"/>.</item>
        /// </list>
        /// </returns>
        public static Point2D[] CreateGridPoints(MacroStabilityInwardsGrid grid)
        {
            if (grid == null || !AreGridSettingsValid(grid))
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

        private static IEnumerable<Point2D[]> GetWaternetZoneWithSurfaceLineIntersection(Point2D[] surfaceLineLocalGeometry,
                                                                                         Point2D[] waternetLineGeometry,
                                                                                         Point2D[] phreaticLineGeometry)
        {
            IEnumerable<Point2D[]> waternetZoneAsPolygons = CreateZoneAreas(waternetLineGeometry, phreaticLineGeometry);

            foreach (Point2D[] waternetZoneAsPolygon in waternetZoneAsPolygons)
            {
                Point2D[] areaPoints = ClipWaternetZoneToSurfaceLine(surfaceLineLocalGeometry, waternetZoneAsPolygon);
                if (areaPoints.Any())
                {
                    yield return areaPoints;
                }
            }
        }

        private static Point2D[] ClipWaternetZoneToSurfaceLine(Point2D[] surfaceLineLocalGeometry, Point2D[] waternetZoneAsPolygon)
        {
            double leftX = waternetZoneAsPolygon.Min(p => p.X);
            double rightX = waternetZoneAsPolygon.Max(p => p.X);

            Segment2D[] surfaceLineSegments = Math2D.ConvertLinePointsToLineSegments(surfaceLineLocalGeometry).ToArray();
            Segment2D[] waternetZoneSegments = Math2D.ConvertLinePointsToLineSegments(waternetZoneAsPolygon).ToArray();

            var intersectionPoints = new List<Point2D>();

            foreach (Segment2D surfaceLineSegment in surfaceLineSegments)
            {
                foreach (Segment2D waternetZoneSegment in waternetZoneSegments)
                {
                    Segment2DIntersectSegment2DResult intersectionPointResult = Math2D.GetIntersectionBetweenSegments(surfaceLineSegment, waternetZoneSegment);

                    if (intersectionPointResult.IntersectionType == Intersection2DType.Intersects)
                    {
                        intersectionPoints.Add(intersectionPointResult.IntersectionPoints.First());
                    }
                }
            }

            IEnumerable<double> allXCoordinates = waternetZoneAsPolygon.Select(p => p.X)
                                                                       .Concat(surfaceLineLocalGeometry.Select(p => p.X))
                                                                       .Concat(intersectionPoints.Select(p => p.X))
                                                                       .Where(x => x >= leftX && x <= rightX)
                                                                       .OrderBy(d => d)
                                                                       .Distinct();

            var topLine = new List<Point2D>();
            var bottomLine = new List<Point2D>();

            foreach (double xCoordinate in allXCoordinates)
            {
                Point2D surfaceLineIntersection = Math2D.SegmentsIntersectionWithVerticalLine(surfaceLineSegments, xCoordinate).First();
                Point2D[] waternetZoneIntersection = Math2D.SegmentsIntersectionWithVerticalLine(waternetZoneSegments, xCoordinate).Distinct().ToArray();

                if (waternetZoneIntersection.Any())
                {
                    double waternetZoneTop = waternetZoneIntersection.Max(p => p.Y);
                    double waternetZoneBottom = waternetZoneIntersection.Min(p => p.Y);

                    double waternetBottomDelta = waternetZoneBottom - surfaceLineIntersection.Y;
                    double waternetTopDelta = waternetZoneTop - surfaceLineIntersection.Y;

                    if ((Math.Abs(waternetBottomDelta) < 1e-6 || waternetBottomDelta > 0) && !intersectionPoints.Any(c => c.X.Equals(xCoordinate)))
                    {
                        continue;
                    }
                    if (Math.Abs(waternetTopDelta) < 1e-6 || waternetTopDelta < 0)
                    {
                        Point2D[] waternetZonePoints = waternetZoneIntersection.OrderBy(p => p.Y).ToArray();

                        bottomLine.Add(waternetZonePoints.First());
                        topLine.Add(waternetZonePoints.Last());
                    }
                    else if (waternetTopDelta > 0 && waternetBottomDelta < 0)
                    {
                        Point2D[] waternetZonePoints = waternetZoneIntersection.OrderBy(p => p.Y).ToArray();
                        bottomLine.Add(waternetZonePoints.First());
                        topLine.Add(surfaceLineIntersection);
                    }
                }
            }

            var area = new List<Point2D>();
            if (AreaIsNotFlatLine(topLine, bottomLine))
            {
                area.AddRange(topLine);
                area.AddRange(bottomLine.OrderByDescending(p => p.X));
                if (topLine.Any())
                {
                    area.Add(topLine.First());
                }
            }
            return area.ToArray();
        }

        private static bool AreaIsNotFlatLine(List<Point2D> topLine, List<Point2D> bottomLine)
        {
            Point2D[] topLineCoordinates = topLine.ToArray();
            Point2D[] bottomLineCoordinates = bottomLine.ToArray();

            return topLineCoordinates.Length != bottomLineCoordinates.Length
                   || topLineCoordinates.Where((t, i) => !(Math.Abs(t.X - bottomLineCoordinates[i].X) < 1e-6)
                                                         || !(Math.Abs(t.Y - bottomLineCoordinates[i].Y) < 1e-6)).Any();
        }

        private static bool IsSurfaceLineAboveWaternetZone(Point2D[] surfaceLineLocalGeometry, Point2D[] waternetLineGeometry, Point2D[] phreaticLineGeometry)
        {
            double surfaceLineLowestPointY = surfaceLineLocalGeometry.Min(p => p.Y);
            double waternetZoneHeighestPointY = Math.Max(waternetLineGeometry.Max(p => p.Y), phreaticLineGeometry.Max(p => p.Y));

            return surfaceLineLowestPointY >= waternetZoneHeighestPointY;
        }

        private static IEnumerable<Point2D[]> CreateZoneAreas(Point2D[] waternetLineGeometry, Point2D[] phreaticLineGeometry)
        {
            var areas = new List<Point2D[]>();

            Segment2D[] phreaticLineSegments = Math2D.ConvertLinePointsToLineSegments(phreaticLineGeometry).ToArray();
            Segment2D[] waternetLineSegments = Math2D.ConvertLinePointsToLineSegments(waternetLineGeometry).ToArray();

            var intersectionPoints = new List<Point2D>();

            foreach (Segment2D phreaticLineSegment in phreaticLineSegments)
            {
                foreach (Segment2D waternetLineSegment in waternetLineSegments)
                {
                    Segment2DIntersectSegment2DResult intersectionPointResult = Math2D.GetIntersectionBetweenSegments(phreaticLineSegment, waternetLineSegment);

                    if (intersectionPointResult.IntersectionType == Intersection2DType.Intersects)
                    {
                        intersectionPoints.Add(intersectionPointResult.IntersectionPoints.First());
                    }
                }
            }

            IEnumerable<double> allXCoordinates = phreaticLineGeometry.Select(p => p.X)
                                                                      .Concat(waternetLineGeometry.Select(p => p.X))
                                                                      .Concat(intersectionPoints.Select(p => p.X))
                                                                      .OrderBy(d => d)
                                                                      .Distinct();

            var waternetLineArea = new List<Point2D>();
            var phreaticLineArea = new List<Point2D>();
            var areaPoints = new List<Point2D>();

            foreach (double xCoordinate in allXCoordinates)
            {
                IEnumerable<Point2D> phreaticLineIntersections = Math2D.SegmentsIntersectionWithVerticalLine(phreaticLineSegments, xCoordinate);
                IEnumerable<Point2D> waternetLineIntersections = Math2D.SegmentsIntersectionWithVerticalLine(waternetLineSegments, xCoordinate);

                if (!phreaticLineIntersections.Any() || !waternetLineIntersections.Any())
                {
                    continue;
                }

                Point2D phreaticLineIntersection = phreaticLineIntersections.First();
                Point2D waternetLineIntersection = waternetLineIntersections.First();

                waternetLineArea.Add(new Point2D(xCoordinate, waternetLineIntersection.Y));
                phreaticLineArea.Add(new Point2D(xCoordinate, phreaticLineIntersection.Y));

                if (intersectionPoints.Any(p => Math.Abs(p.X - xCoordinate) < 1e-6))
                {
                    areaPoints.AddRange(phreaticLineArea);
                    areaPoints.AddRange(waternetLineArea.OrderByDescending(p => p.X));
                    areaPoints.Add(phreaticLineArea.First());

                    areas.Add(areaPoints.ToArray());

                    waternetLineArea.Clear();
                    phreaticLineArea.Clear();
                    areaPoints.Clear();

                    waternetLineArea.Add(new Point2D(xCoordinate, waternetLineIntersection.Y));
                    phreaticLineArea.Add(new Point2D(xCoordinate, phreaticLineIntersection.Y));
                }
            }

            areaPoints.AddRange(phreaticLineArea);
            areaPoints.AddRange(waternetLineArea.OrderByDescending(p => p.X));
            areaPoints.Add(phreaticLineArea.First());

            areas.Add(areaPoints.ToArray());

            return areas;
        }

        #endregion
    }
}