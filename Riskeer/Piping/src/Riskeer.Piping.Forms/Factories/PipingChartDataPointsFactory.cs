// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Geometry;
using Core.Components.Chart.Data;
using Riskeer.Piping.Data;
using Riskeer.Piping.Primitives;

namespace Riskeer.Piping.Forms.Factories
{
    /// <summary>
    /// Factory for creating arrays of <see cref="Point2D"/> to use in <see cref="ChartData"/>
    /// (created via <see cref="PipingChartDataFactory"/>).
    /// </summary>
    internal static class PipingChartDataPointsFactory
    {
        /// <summary>
        /// Create surface line points in 2D space based on the provided <paramref name="surfaceLine"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="PipingSurfaceLine"/> to create the surface line points for.</param>
        /// <returns>An array of points in 2D space or an empty array when <paramref name="surfaceLine"/> is <c>null</c>.</returns>
        public static Point2D[] CreateSurfaceLinePoints(PipingSurfaceLine surfaceLine)
        {
            return surfaceLine?.LocalGeometry.ToArray() ?? new Point2D[0];
        }

        /// <summary>
        /// Create an entry point in 2D space based on the provided <paramref name="pipingInput"/>.
        /// </summary>
        /// <param name="pipingInput">The <see cref="PipingInput"/> to create the entry point for.</param>
        /// <returns>An array with an entry point in 2D space or an empty array when:
        /// <list type="bullet">
        /// <item><paramref name="pipingInput"/> is <c>null</c>;</item>
        /// <item>the <see cref="PipingSurfaceLine"/> in <paramref name="pipingInput"/> is <c>null</c>;</item>
        /// <item>the entry point in <paramref name="pipingInput"/> equals <c>double.NaN</c>.</item>
        /// </list>
        /// </returns>
        public static Point2D[] CreateEntryPointPoint(PipingInput pipingInput)
        {
            return pipingInput?.SurfaceLine != null && !double.IsNaN(pipingInput.EntryPointL)
                       ? new[]
                       {
                           new Point2D(pipingInput.EntryPointL, pipingInput.SurfaceLine.GetZAtL(pipingInput.EntryPointL))
                       }
                       : new Point2D[0];
        }

        /// <summary>
        /// Create an exit point in 2D space based on the provided <paramref name="pipingInput"/>.
        /// </summary>
        /// <param name="pipingInput">The <see cref="PipingInput"/> to create the exit point for.</param>
        /// <returns>An array with an exit point in 2D space or an empty array when:
        /// <list type="bullet">
        /// <item><paramref name="pipingInput"/> is <c>null</c>;</item>
        /// <item>the <see cref="PipingSurfaceLine"/> in <paramref name="pipingInput"/> is <c>null</c>;</item>
        /// <item>the exit point in <paramref name="pipingInput"/> equals <c>double.NaN</c>.</item>
        /// </list>
        /// </returns>
        public static Point2D[] CreateExitPointPoint(PipingInput pipingInput)
        {
            return pipingInput?.SurfaceLine != null && !double.IsNaN(pipingInput.ExitPointL)
                       ? new[]
                       {
                           new Point2D(pipingInput.ExitPointL, pipingInput.SurfaceLine.GetZAtL(pipingInput.ExitPointL))
                       }
                       : new Point2D[0];
        }

        /// <summary>
        /// Create a ditch polder side point in 2D space based on the provided <paramref name="surfaceLine"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="PipingSurfaceLine"/> to create the ditch polder side point for.</param>
        /// <returns>An array with a ditch polder side point in 2D space or an empty array when:
        /// <list type="bullet">
        /// <item><paramref name="surfaceLine"/> is <c>null</c>;</item>
        /// <item>the ditch polder side point in <paramref name="surfaceLine"/> is <c>null</c>.</item>
        /// </list>
        /// </returns>
        public static Point2D[] CreateDitchPolderSidePoint(PipingSurfaceLine surfaceLine)
        {
            return surfaceLine?.DitchPolderSide != null
                       ? new[]
                       {
                           surfaceLine.GetLocalPointFromGeometry(surfaceLine.DitchPolderSide)
                       }
                       : new Point2D[0];
        }

        /// <summary>
        /// Create a bottom ditch polder side point in 2D space based on the provided <paramref name="surfaceLine"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="PipingSurfaceLine"/> to create the bottom ditch polder side point for.</param>
        /// <returns>An array with a bottom ditch polder side point in 2D space or an empty array when:
        /// <list type="bullet">
        /// <item><paramref name="surfaceLine"/> is <c>null</c>;</item>
        /// <item>the bottom ditch polder side point in <paramref name="surfaceLine"/> is <c>null</c>.</item>
        /// </list>
        /// </returns>
        public static Point2D[] CreateBottomDitchPolderSidePoint(PipingSurfaceLine surfaceLine)
        {
            return surfaceLine?.BottomDitchPolderSide != null
                       ? new[]
                       {
                           surfaceLine.GetLocalPointFromGeometry(surfaceLine.BottomDitchPolderSide)
                       }
                       : new Point2D[0];
        }

        /// <summary>
        /// Create a bottom ditch dike side point in 2D space based on the provided <paramref name="surfaceLine"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="PipingSurfaceLine"/> to create the bottom ditch dike side point for.</param>
        /// <returns>An array with a bottom ditch dike side point in 2D space or an empty array when:
        /// <list type="bullet">
        /// <item><paramref name="surfaceLine"/> is <c>null</c>;</item>
        /// <item>the bottom ditch dike side point in <paramref name="surfaceLine"/> is <c>null</c>.</item>
        /// </list>
        /// </returns>
        public static Point2D[] CreateBottomDitchDikeSidePoint(PipingSurfaceLine surfaceLine)
        {
            return surfaceLine?.BottomDitchDikeSide != null
                       ? new[]
                       {
                           surfaceLine.GetLocalPointFromGeometry(surfaceLine.BottomDitchDikeSide)
                       }
                       : new Point2D[0];
        }

        /// <summary>
        /// Create a ditch dike side point in 2D space based on the provided <paramref name="surfaceLine"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="PipingSurfaceLine"/> to create the ditch dike side point for.</param>
        /// <returns>An array with a ditch dike side point in 2D space or an empty array when:
        /// <list type="bullet">
        /// <item><paramref name="surfaceLine"/> is <c>null</c>;</item>
        /// <item>the ditch dike side point in <paramref name="surfaceLine"/> is <c>null</c>.</item>
        /// </list>
        /// </returns>
        public static Point2D[] CreateDitchDikeSidePoint(PipingSurfaceLine surfaceLine)
        {
            return surfaceLine?.DitchDikeSide != null
                       ? new[]
                       {
                           surfaceLine.GetLocalPointFromGeometry(surfaceLine.DitchDikeSide)
                       }
                       : new Point2D[0];
        }

        /// <summary>
        /// Create a dike toe at river point in 2D space based on the provided <paramref name="surfaceLine"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="PipingSurfaceLine"/> to create the dike toe at river point for.</param>
        /// <returns>An array with a dike toe at river point in 2D space or an empty array when:
        /// <list type="bullet">
        /// <item><paramref name="surfaceLine"/> is <c>null</c>;</item>
        /// <item>the dike toe at river point in <paramref name="surfaceLine"/> is <c>null</c>.</item>
        /// </list>
        /// </returns>
        public static Point2D[] CreateDikeToeAtRiverPoint(PipingSurfaceLine surfaceLine)
        {
            return surfaceLine?.DikeToeAtRiver != null
                       ? new[]
                       {
                           surfaceLine.GetLocalPointFromGeometry(surfaceLine.DikeToeAtRiver)
                       }
                       : new Point2D[0];
        }

        /// <summary>
        /// Create a dike toe at polder point in 2D space based on the provided <paramref name="surfaceLine"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="PipingSurfaceLine"/> to create the dike toe at polder point for.</param>
        /// <returns>An array with a dike toe at polder point in 2D space or an empty array when:
        /// <list type="bullet">
        /// <item><paramref name="surfaceLine"/> is <c>null</c>;</item>
        /// <item>the dike toe at polder point in <paramref name="surfaceLine"/> is <c>null</c>.</item>
        /// </list>
        /// </returns>
        public static Point2D[] CreateDikeToeAtPolderPoint(PipingSurfaceLine surfaceLine)
        {
            return surfaceLine?.DikeToeAtPolder != null
                       ? new[]
                       {
                           surfaceLine.GetLocalPointFromGeometry(surfaceLine.DikeToeAtPolder)
                       }
                       : new Point2D[0];
        }

        /// <summary>
        /// Create a collection of soil layer points (areas) in 2D space based on the provided <paramref name="soilLayer"/> and <paramref name="soilProfile"/>.
        /// </summary>
        /// <param name="soilLayer">The <see cref="PipingSoilLayer"/> to create the soil layer points for.</param>
        /// <param name="soilProfile">The <see cref="PipingSoilProfile"/> that contains <paramref name="soilLayer"/>.</param>
        /// <param name="surfaceLine">The <see cref="PipingSurfaceLine"/> that may intersect with 
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
        public static IEnumerable<Point2D[]> CreateSoilLayerAreas(PipingSoilLayer soilLayer, PipingSoilProfile soilProfile, PipingSurfaceLine surfaceLine)
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

        private static IEnumerable<Point2D[]> GetSoilLayerWithSurfaceLineIntersection(Point2D[] surfaceLineLocalGeometry, PipingSoilLayer soilLayer, PipingSoilProfile soilProfile)
        {
            IEnumerable<Point2D> surfaceLineAsPolygon = CreateSurfaceLinePolygonAroundSoilLayer(surfaceLineLocalGeometry, soilLayer, soilProfile);
            Point2D[] soilLayerAsPolygon = CreateSurfaceLineWideSoilLayer(surfaceLineLocalGeometry, soilLayer, soilProfile);

            return AdvancedMath2D.PolygonIntersectionWithPolygon(surfaceLineAsPolygon, soilLayerAsPolygon);
        }

        private static bool IsSurfaceLineAboveSoilLayer(IEnumerable<Point2D> surfaceLineLocalGeometry, PipingSoilLayer soilLayer)
        {
            double surfaceLineLowestPointY = surfaceLineLocalGeometry.Select(p => p.Y).Min();
            double topLevel = soilLayer.Top;

            return surfaceLineLowestPointY >= topLevel;
        }

        private static bool IsSurfaceLineBelowSoilLayer(IEnumerable<Point2D> surfaceLineLocalGeometry, PipingSoilLayer soilLayer, PipingSoilProfile soilProfile)
        {
            double topLevel = soilLayer.Top;
            return surfaceLineLocalGeometry.Select(p => p.Y).Max() <= topLevel - soilProfile.GetLayerThickness(soilLayer);
        }

        private static IEnumerable<Point2D> CreateSurfaceLinePolygonAroundSoilLayer(IEnumerable<Point2D> surfaceLineLocalGeometry, PipingSoilLayer soilLayer, PipingSoilProfile soilProfile)
        {
            double topLevel = soilLayer.Top;
            double bottomLevel = topLevel - soilProfile.GetLayerThickness(soilLayer);

            double closingSurfaceLineToPolygonBottomLevel = Math.Min(surfaceLineLocalGeometry.Select(p => p.Y).Min(), bottomLevel) - 1;

            return AdvancedMath2D.CompleteLineToPolygon(surfaceLineLocalGeometry, closingSurfaceLineToPolygonBottomLevel).ToArray();
        }

        private static Point2D[] CreateSurfaceLineWideSoilLayer(Point2D[] surfaceLineLocalGeometry, PipingSoilLayer soilLayer, PipingSoilProfile soilProfile)
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
    }
}