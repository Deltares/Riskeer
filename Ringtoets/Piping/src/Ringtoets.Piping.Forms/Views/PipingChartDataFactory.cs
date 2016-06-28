// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Components.Charting.Data;
using Core.Components.Charting.Styles;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Piping.Forms.Properties;
using Ringtoets.Piping.Primitives;
using PipingDataResources = Ringtoets.Piping.Data.Properties.Resources;

namespace Ringtoets.Piping.Forms.Views
{
    /// <summary>
    /// Factory for creating <see cref="ChartData"/> based on information used as input in the piping failure mechanism.
    /// </summary>
    public static class PipingChartDataFactory
    {
        /// <summary>
        /// Create <see cref="ChartData"/> with default styling based on the <paramref name="surfaceLine"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsPipingSurfaceLine"/> for which to create <see cref="ChartData"/>.</param>
        /// <returns><see cref="ChartData"/> based on the <paramref name="surfaceLine"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="surfaceLine"/> is <c>null</c>.</exception>
        public static ChartData Create(RingtoetsPipingSurfaceLine surfaceLine)
        {
            if (surfaceLine == null)
            {
                throw new ArgumentNullException("surfaceLine");
            }

            return new ChartLineData(surfaceLine.ProjectGeometryToLZ(), surfaceLine.Name)
            {
                Style = new ChartLineStyle(Color.Sienna, 2, DashStyle.Solid)
            };
        }

        /// <summary>
        /// Create a <see cref="ChartData"/> with default styling based on the <paramref name="entryPoint"/>.
        /// </summary>
        /// <param name="entryPoint">The horizontal distance from the origin at which to place the entry point
        /// on the  <paramref name="surfaceLine"/>.</param>
        /// <param name="surfaceLine">The <see cref="RingtoetsPipingSurfaceLine"/> to place the entry point on.</param>
        /// <returns><see cref="ChartData"/> based on the <paramref name="entryPoint"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="entryPoint"/> is <c>NaN</c>.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="surfaceLine"/> is <c>null</c>.</exception>
        public static ChartData CreateEntryPoint(RoundedDouble entryPoint, RingtoetsPipingSurfaceLine surfaceLine)
        {
            if (double.IsNaN(entryPoint))
            {
                throw new ArgumentException("Entry point should have a value.", "entryPoint");
            }

            if (surfaceLine == null)
            {
                throw new ArgumentNullException("surfaceLine");
            }

            return CreatePointWithZAtL(entryPoint, surfaceLine, Resources.PipingInput_EntryPointL_DisplayName, Color.Gold);
        }

        /// <summary>
        /// Create a <see cref="ChartData"/> with default styling based on the <paramref name="exitPoint"/>.
        /// </summary>
        /// <param name="exitPoint">The horizontal distance from the origin at which to place the exit point
        /// on the  <paramref name="surfaceLine"/>.</param>
        /// <param name="surfaceLine">The <see cref="RingtoetsPipingSurfaceLine"/> to place the exit point on.</param>
        /// <returns><see cref="ChartData"/> based on the <paramref name="exitPoint"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="exitPoint"/> is <c>NaN</c>.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="surfaceLine"/> is <c>null</c>.</exception>
        public static ChartData CreateExitPoint(RoundedDouble exitPoint, RingtoetsPipingSurfaceLine surfaceLine)
        {
            if (double.IsNaN(exitPoint))
            {
                throw new ArgumentException("Exit point should have a value.", "exitPoint");
            }

            if (surfaceLine == null)
            {
                throw new ArgumentNullException("surfaceLine");
            }

            return CreatePointWithZAtL(exitPoint, surfaceLine, Resources.PipingInput_ExitPointL_DisplayName, Color.Tomato);
        }

        /// <summary>
        /// Create a <see cref="ChartData"/> with default styling based on the <paramref name="surfaceLine.DitchPolderSide"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsPipingSurfaceLine"/> which contains a point which 
        /// characterizes the ditch at polder side, to create <see cref="ChartData"/> for.</param>
        /// <returns><see cref="ChartData"/> based on the <paramref name="surfaceLine.DitchPolderSide"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="surfaceLine"/> is <c>null</c> or 
        /// the <see cref="RingtoetsPipingSurfaceLine"/> contains no <see cref="RingtoetsPipingSurfaceLine.DitchPolderSide"/>.</exception>
        public static ChartData CreateDitchPolderSide(RingtoetsPipingSurfaceLine surfaceLine)
        {
            if (surfaceLine == null)
            {
                throw new ArgumentNullException("surfaceLine");
            }

            return CreateCharacteristicPoint(surfaceLine.DitchPolderSide, surfaceLine, PipingDataResources.CharacteristicPoint_DitchPolderSide, Color.IndianRed);
        }

        /// <summary>
        /// Create a <see cref="ChartData"/> with default styling based on the <paramref name="surfaceLine.BottomDitchPolderSide"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsPipingSurfaceLine"/> which contains a point which 
        /// characterizes the bottom ditch at polder side, to create <see cref="ChartData"/> for.</param>
        /// <returns><see cref="ChartData"/> based on the <paramref name="surfaceLine.BottomDitchPolderSide"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="surfaceLine"/> is <c>null</c> or 
        /// the <see cref="RingtoetsPipingSurfaceLine"/> contains no <see cref="RingtoetsPipingSurfaceLine.BottomDitchPolderSide"/>.</exception>
        public static ChartData CreateBottomDitchPolderSide(RingtoetsPipingSurfaceLine surfaceLine)
        {
            if (surfaceLine == null)
            {
                throw new ArgumentNullException("surfaceLine");
            }

            return CreateCharacteristicPoint(surfaceLine.BottomDitchPolderSide, surfaceLine, PipingDataResources.CharacteristicPoint_BottomDitchPolderSide, Color.Teal);
        }

        /// <summary>
        /// Create a <see cref="ChartData"/> with default styling based on the <paramref name="surfaceLine.BottomDitchDikeSide"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsPipingSurfaceLine"/> which contains a point which 
        /// characterizes the bottom ditch at dike side, to create <see cref="ChartData"/> for.</param>
        /// <returns><see cref="ChartData"/> based on the <paramref name="surfaceLine.BottomDitchDikeSide"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="surfaceLine"/> is <c>null</c> or 
        /// the <see cref="RingtoetsPipingSurfaceLine"/> contains no <see cref="RingtoetsPipingSurfaceLine.BottomDitchDikeSide"/>.</exception>
        public static ChartData CreateBottomDitchDikeSide(RingtoetsPipingSurfaceLine surfaceLine)
        {
            if (surfaceLine == null)
            {
                throw new ArgumentNullException("surfaceLine");
            }

            return CreateCharacteristicPoint(surfaceLine.BottomDitchDikeSide, surfaceLine, PipingDataResources.CharacteristicPoint_BottomDitchDikeSide, Color.DarkSeaGreen);
        }

        /// <summary>
        /// Create a <see cref="ChartData"/> with default styling based on the <paramref name="surfaceLine.DitchDikeSide"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsPipingSurfaceLine"/> which contains a point which 
        /// characterizes the ditch at dike side, to create <see cref="ChartData"/> for.</param>
        /// <returns><see cref="ChartData"/> based on the <paramref name="surfaceLine.DitchDikeSide"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="surfaceLine"/> is <c>null</c> or 
        /// the <see cref="RingtoetsPipingSurfaceLine"/> contains no <see cref="RingtoetsPipingSurfaceLine.DitchDikeSide"/>.</exception>
        public static ChartData CreateDitchDikeSide(RingtoetsPipingSurfaceLine surfaceLine)
        {
            if (surfaceLine == null)
            {
                throw new ArgumentNullException("surfaceLine");
            }

            return CreateCharacteristicPoint(surfaceLine.DitchDikeSide, surfaceLine, PipingDataResources.CharacteristicPoint_DitchDikeSide, Color.MediumPurple);
        }

        /// <summary>
        /// Create a <see cref="ChartData"/> with default styling based on the <paramref name="surfaceLine.DikeToeAtRiver"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsPipingSurfaceLine"/> which contains a point which 
        /// characterizes the dike toe at river side, to create <see cref="ChartData"/> for.</param>
        /// <returns><see cref="ChartData"/> based on the <paramref name="surfaceLine.DikeToeAtRiver"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="surfaceLine"/> is <c>null</c> or 
        /// the <see cref="RingtoetsPipingSurfaceLine"/> contains no <see cref="RingtoetsPipingSurfaceLine.DikeToeAtRiver"/>.</exception>
        public static ChartData CreateDikeToeAtRiver(RingtoetsPipingSurfaceLine surfaceLine)
        {
            if (surfaceLine == null)
            {
                throw new ArgumentNullException("surfaceLine");
            }

            return CreateCharacteristicPoint(surfaceLine.DikeToeAtRiver, surfaceLine, PipingDataResources.CharacteristicPoint_DikeToeAtRiver, Color.DarkBlue);
        }

        /// <summary>
        /// Create a <see cref="ChartData"/> with default styling based on the <paramref name="surfaceLine.DikeToeAtPolder"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsPipingSurfaceLine"/> which contains a point which 
        /// characterizes the dike toe at polder side, to create <see cref="ChartData"/> for.</param>
        /// <returns><see cref="ChartData"/> based on the <paramref name="surfaceLine.DikeToeAtPolder"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="surfaceLine"/> is <c>null</c> or 
        /// the <see cref="RingtoetsPipingSurfaceLine"/> contains no <see cref="RingtoetsPipingSurfaceLine.DikeToeAtPolder"/>.</exception>
        public static ChartData CreateDikeToeAtPolder(RingtoetsPipingSurfaceLine surfaceLine)
        {
            if (surfaceLine == null)
            {
                throw new ArgumentNullException("surfaceLine");
            }

            return CreateCharacteristicPoint(surfaceLine.DikeToeAtPolder, surfaceLine, PipingDataResources.CharacteristicPoint_DikeToeAtPolder, Color.SlateGray);
        }

        /// <summary>
        /// Create an instance of <see cref="ChartData"/> with styling based on the color of <see name="SoilLayer.Color"/> of
        /// <paramref name="soilLayer"/>. The <paramref name="soilLayer"/> is drawn for the full width of the <paramref name="surfaceLine"/>
        /// (as far as the <paramref name="soilLayer"/> is visible below the <paramref name="surfaceLine"/>.
        /// </summary>
        /// <param name="soilLayer">The <see cref="PipingSoilLayer"/> to create <see cref="ChartData"/> for.</param>
        /// <param name="soilProfile">The <see cref="PipingSoilProfile"/> which contains the <see cref="PipingSoilLayer"/>.</param>
        /// <param name="surfaceLine">The <see cref="RingtoetsPipingSurfaceLine"/> that may intersect with the 
        /// <paramref name="soilLayer"/> and by doing that restricts the drawn height of the <paramref name="soilLayer"/>.</param>
        /// <returns>An <see cref="ICollection{T}"/> which contains one or more (in the case of the <paramref name="surfaceLine"/> 
        /// splitting the layer in multiple parts) instances of <see cref="ChartData"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="soilLayer"/> is <c>null</c>.</item>
        /// <item><paramref name="surfaceLine"/> is <c>null</c>.</item>
        /// </list></exception>
        public static ChartData CreatePipingSoilLayer(PipingSoilLayer soilLayer, PipingSoilProfile soilProfile, RingtoetsPipingSurfaceLine surfaceLine)
        {
            if (surfaceLine == null)
            {
                throw new ArgumentNullException("surfaceLine");
            }

            var index = 1;
            var name = string.Format("{0}_{1}", index, soilLayer.MaterialName);

            var surfaceLineLocalGeometry = surfaceLine.ProjectGeometryToLZ().ToArray();
            var surfaceLineLowestPointY = surfaceLineLocalGeometry.Select(p => p.Y).Min();
            var surfaceLineHeighestPointY = surfaceLineLocalGeometry.Select(p => p.Y).Max();
            var topLevel = soilLayer.Top;
            var bottomLevel = topLevel - soilProfile.GetLayerThickness(soilLayer);

            var firstSurfaceLinePoint = surfaceLineLocalGeometry.First();
            var lastSurfaceLinePoint = surfaceLineLocalGeometry.Last();

            var startX = firstSurfaceLinePoint.X;
            var endX = lastSurfaceLinePoint.X;

            if (surfaceLineLowestPointY >= topLevel)
            {
                return new ChartAreaData(new []
                {
                    new Point2D(startX, topLevel),
                    new Point2D(endX, topLevel),
                    new Point2D(endX, bottomLevel),
                    new Point2D(startX, bottomLevel)
                }, name);
            }
            if (surfaceLineLowestPointY > bottomLevel)
            {
                Point2D soilLayerTopPointA = new Point2D(
                    startX,
                    topLevel);
                Point2D soilLayerTopPointB = new Point2D(
                    endX,
                    topLevel);

                var points = new HashSet<Point2D>();
                var addingFromSurfaceLine = firstSurfaceLinePoint.Y < topLevel;
                if (addingFromSurfaceLine)
                {
                    points.Add(firstSurfaceLinePoint);
                }
                else
                {
                    points.Add(soilLayerTopPointA);
                }
                for (var i = 1; i < surfaceLineLocalGeometry.Length; i++)
                {
                    Point2D surfaceLinePointA = surfaceLineLocalGeometry[i-1];
                    Point2D surfaceLinePointB = surfaceLineLocalGeometry[i];

                    var intersection = Math2D.GetIntersectionBetweenSegments(
                        new Segment2D(soilLayerTopPointA, soilLayerTopPointB),
                        new Segment2D(surfaceLinePointA, surfaceLinePointB));

                    if (intersection.IntersectionType == Intersection2DType.Intersects)
                    {
                        addingFromSurfaceLine = surfaceLinePointA.Y > surfaceLinePointB.Y;
                        points.Add(intersection.IntersectionPoints[0]);
                    }
                    if (addingFromSurfaceLine)
                    {
                        points.Add(surfaceLinePointB);
                    }
                }
                if (!addingFromSurfaceLine)
                {
                    points.Add(new Point2D(endX, topLevel));
                }
                points.Add(new Point2D(endX, bottomLevel));
                points.Add(new Point2D(startX, bottomLevel));

                return new ChartAreaData(points, name);
            }
            if (surfaceLineHeighestPointY > bottomLevel)
            {
                 
            }
            return new ChartAreaData(Enumerable.Empty<Point2D>(), name);
        } 

        private static ChartData CreatePointWithZAtL(RoundedDouble pointL, RingtoetsPipingSurfaceLine surfaceLine, string name, Color color)
        {
            ChartPointData pointWithZatLData;

            try
            {
                var pointZ = surfaceLine.GetZAtL(pointL);

                pointWithZatLData = new ChartPointData(new[]
                {
                    new Point2D(pointL, pointZ),
                }, name)
                {
                    Style = new ChartPointStyle(color, 8, Color.Transparent, 0, ChartPointSymbol.Triangle)
                };
            }
            catch (ArgumentOutOfRangeException)
            {
                // TODO Should not have to handle when WTI-673 and WTI-396 are done.
                pointWithZatLData = ChartDataFactory.CreateEmptyPointData(name);
            }

            return pointWithZatLData;
        }

        private static ChartData CreateCharacteristicPoint(Point3D worldPoint, RingtoetsPipingSurfaceLine surfaceLine, string name, Color color)
        {
            return CreateLocalPoint(worldPoint, surfaceLine, name, new ChartPointStyle(color, 8, Color.Transparent, 0, ChartPointSymbol.Circle));
        }

        private static ChartData CreateLocalPoint(Point3D worldPoint, RingtoetsPipingSurfaceLine surfaceLine, string name, ChartPointStyle style)
        {
            Point2D firstPoint = Point3DToPoint2D(surfaceLine.Points.First());
            Point2D lastPoint = Point3DToPoint2D(surfaceLine.Points.Last());
            return new ChartPointData(new[]
            {
                worldPoint.ProjectIntoLocalCoordinates(firstPoint, lastPoint)
            }, name)
            {
                Style = style
            };
        }

        private static Point2D Point3DToPoint2D(Point3D point3D)
        {
            return new Point2D(point3D.X, point3D.Y);
        }
    }
}