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
using Core.Common.Geometry;
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
        /// Create a collection of soil layer points (areas) in 2D space based on the provided <paramref name="soilLayer"/> and <paramref name="soilProfile"/>.
        /// </summary>
        /// <param name="soilLayer">The <see cref="MacroStabilityInwardsSoilLayer1D"/> to create the soil layer points for.</param>
        /// <param name="soilProfile">The <see cref="MacroStabilityInwardsSoilProfile1D"/> that contains <paramref name="soilLayer"/>.</param>
        /// <param name="surfaceLine">The <see cref="MacroStabilityInwardsSurfaceLine"/> that may intersect with 
        /// the <paramref name="soilLayer"/> and by doing that restricts the drawn height of it.</param>
        /// <returns>A collection which contains one or more (in the case of <paramref name="surfaceLine"/>
        /// splitting the layer in multiple parts) arrays of points in 2D space or an empty array when:
        /// <list type="bullet">
        /// <item><paramref name="soilLayer"/> is <c>null</c>;</item>
        /// <item><paramref name="soilProfile"/> is <c>null</c>;</item>
        /// <item><paramref name="surfaceLine"/> is <c>null</c>;</item>
        /// <item>the <paramref name="surfaceLine"/> is below the <paramref name="soilLayer"/>.</item>
        /// </list>
        /// </returns>
        public static IEnumerable<Point2D[]> CreateSoilLayerAreas(MacroStabilityInwardsSoilLayer1D soilLayer,
                                                                  MacroStabilityInwardsSoilProfile1D soilProfile,
                                                                  MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            if (soilLayer == null || soilProfile == null || surfaceLine == null)
            {
                return Enumerable.Empty<Point2D[]>();
            }

            Point2D[] surfaceLineLocalGeometry = surfaceLine.LocalGeometry.ToArray();

            if (IsSurfaceLineAboveSoilLayer(surfaceLineLocalGeometry, soilLayer))
            {
                return new List<Point2D[]>
                {
                    CreateSurfaceLineWideSoilLayer(surfaceLineLocalGeometry, soilLayer, soilProfile)
                };
            }

            if (IsSurfaceLineBelowSoilLayer(surfaceLineLocalGeometry, soilLayer, soilProfile))
            {
                return Enumerable.Empty<Point2D[]>();
            }

            return GetSoilLayerWithSurfaceLineIntersection(surfaceLineLocalGeometry, soilLayer, soilProfile);
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
        /// Create a traffic load outside point in 2D space based on the provided <paramref name="surfaceLine"/>.
        /// </summary>
        /// <param name="surfaceLine">The surface line to create the traffic load outside point for.</param>
        /// <returns>An array with a traffic load outside point in 2D space or an empty array when:
        /// <list type="bullet">
        /// <item><paramref name="surfaceLine"/> is <c>null</c>;</item>
        /// <item>the traffic load outside point in <paramref name="surfaceLine"/> is <c>null</c>.</item>
        /// </list>
        /// </returns>
        public static Point2D[] CreateTrafficLoadOutsidePoint(MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            return GetLocalPointsFromGeometry(surfaceLine, surfaceLine?.TrafficLoadOutside);
        }

        /// <summary>
        /// Create a traffic load inside point in 2D space based on the provided <paramref name="surfaceLine"/>.
        /// </summary>
        /// <param name="surfaceLine">The surface line to create the traffic load inside point for.</param>
        /// <returns>An array with a traffic load inside point in 2D space or an empty array when:
        /// <list type="bullet">
        /// <item><paramref name="surfaceLine"/> is <c>null</c>;</item>
        /// <item>the traffic load inside point in <paramref name="surfaceLine"/> is <c>null</c>.</item>
        /// </list>
        /// </returns>
        public static Point2D[] CreateTrafficLoadInsidePoint(MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            return GetLocalPointsFromGeometry(surfaceLine, surfaceLine?.TrafficLoadInside);
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

        #region Grid Helpers

        /// <summary>
        /// Creates grid points in 2D space based on the provided <paramref name="grid"/>.
        /// </summary>
        /// <param name="grid">The grid to create the grid points for.</param>
        /// <returns></returns>
        public static Point2D[] CreateGridPoints(MacroStabilityInwardsGrid grid)
        {
            if (grid == null || !AreGridSettingsValid(grid))
            {
                return new Point2D[0];
            }

            var points = new List<Point2D>();
            IEnumerable<RoundedDouble> interPolatedVerticalPositions = GetInterPolatedVerticalPositions(grid.ZBottom,
                                                                                                        grid.ZTop,
                                                                                                        grid.NumberOfVerticalPoints).ToArray();
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
            return grid.NumberOfHorizontalPoints > 0
                   && grid.NumberOfVerticalPoints > 0
                   && !IsGridCoordinateNaN(grid.XLeft)
                   && !IsGridCoordinateNaN(grid.XRight)
                   && !IsGridCoordinateNaN(grid.ZTop) 
                   && !IsGridCoordinateNaN(grid.ZBottom);
        }

        private static bool IsGridCoordinateNaN(RoundedDouble gridCoordinate)
        {
            return double.IsNaN(gridCoordinate);
        }

        private static IEnumerable<RoundedDouble> GetInterPolatedVerticalPositions(RoundedDouble startPoint,
                                                                                   RoundedDouble endPoint,
                                                                                   int nrofPoints)
        {
            if (nrofPoints <= 1)
            {
                yield return startPoint;
                yield break;
            }

            int nrofInterPolatedPoints = nrofPoints - 1;
            RoundedDouble deltaZ = endPoint - startPoint;
            RoundedDouble deltaZBetweenPoints = nrofPoints < 2
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
                                                                            int nrofPoints)
        {
            if (nrofPoints <= 1)
            {
                yield return new Point2D(startPoint, zPoint);
                yield break;
            }

            int nrofInterPolatedPoints = nrofPoints - 1;
            RoundedDouble deltaX = endPoint - startPoint;
            RoundedDouble deltaXBetweenPoints = nrofPoints < 2
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

        private static IEnumerable<Point2D[]> GetSoilLayerWithSurfaceLineIntersection(Point2D[] surfaceLineLocalGeometry,
                                                                                      MacroStabilityInwardsSoilLayer1D soilLayer,
                                                                                      MacroStabilityInwardsSoilProfile1D soilProfile)
        {
            Point2D[] surfaceLineAsPolygon = CreateSurfaceLinePolygonAroundSoilLayer(surfaceLineLocalGeometry, soilLayer, soilProfile);
            Point2D[] soilLayerAsPolygon = CreateSurfaceLineWideSoilLayer(surfaceLineLocalGeometry, soilLayer, soilProfile);

            return AdvancedMath2D.PolygonIntersectionWithPolygon(surfaceLineAsPolygon, soilLayerAsPolygon);
        }

        private static bool IsSurfaceLineAboveSoilLayer(IEnumerable<Point2D> surfaceLineLocalGeometry,
                                                        MacroStabilityInwardsSoilLayer1D soilLayer)
        {
            double surfaceLineLowestPointY = surfaceLineLocalGeometry.Select(p => p.Y).Min();
            double topLevel = soilLayer.Top;

            return surfaceLineLowestPointY >= topLevel;
        }

        private static bool IsSurfaceLineBelowSoilLayer(Point2D[] surfaceLineLocalGeometry,
                                                        MacroStabilityInwardsSoilLayer1D soilLayer,
                                                        MacroStabilityInwardsSoilProfile1D soilProfile)
        {
            double topLevel = soilLayer.Top;
            return surfaceLineLocalGeometry.Select(p => p.Y).Max() <= topLevel - soilProfile.GetLayerThickness(soilLayer);
        }

        private static Point2D[] CreateSurfaceLinePolygonAroundSoilLayer(Point2D[] surfaceLineLocalGeometry,
                                                                         MacroStabilityInwardsSoilLayer1D soilLayer,
                                                                         MacroStabilityInwardsSoilProfile1D soilProfile)
        {
            List<Point2D> surfaceLineAsPolygon = surfaceLineLocalGeometry.ToList();

            double topLevel = soilLayer.Top;
            double bottomLevel = topLevel - soilProfile.GetLayerThickness(soilLayer);
            double surfaceLineLowestPointY = surfaceLineAsPolygon.Select(p => p.Y).Min();

            double closingSurfaceLineToPolygonBottomLevel = Math.Min(surfaceLineLowestPointY, bottomLevel) - 1;

            surfaceLineAsPolygon.Add(new Point2D(surfaceLineAsPolygon.Last().X, closingSurfaceLineToPolygonBottomLevel));
            surfaceLineAsPolygon.Add(new Point2D(surfaceLineAsPolygon.First().X, closingSurfaceLineToPolygonBottomLevel));

            return surfaceLineAsPolygon.ToArray();
        }

        private static Point2D[] CreateSurfaceLineWideSoilLayer(Point2D[] surfaceLineLocalGeometry,
                                                                MacroStabilityInwardsSoilLayer1D soilLayer,
                                                                MacroStabilityInwardsSoilProfile1D soilProfile)
        {
            Point2D firstSurfaceLinePoint = surfaceLineLocalGeometry.First();
            Point2D lastSurfaceLinePoint = surfaceLineLocalGeometry.Last();

            double startX = firstSurfaceLinePoint.X;
            double endX = lastSurfaceLinePoint.X;

            double topLevel = soilLayer.Top;
            double bottomLevel = topLevel - soilProfile.GetLayerThickness(soilLayer);

            return new[]
            {
                new Point2D(startX, topLevel),
                new Point2D(endX, topLevel),
                new Point2D(endX, bottomLevel),
                new Point2D(startX, bottomLevel)
            };
        }

        #endregion
    }
}