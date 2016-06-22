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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Components.Charting.Data;
using Core.Components.Charting.Styles;
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
        /// Create a <see cref="ChartLineData"/> instance with a name, but without data.
        /// </summary>
        /// <param name="name">The name of the <see cref="ChartLineData"/>.</param>
        /// <returns>An empty <see cref="ChartLineData"/> object.</returns>
        public static ChartLineData CreateEmptyLineData(string name)
        {
            return new ChartLineData(Enumerable.Empty<Point2D>(), name);
        }

        /// <summary>
        /// Create a <see cref="ChartPointData"/> instance with a name, but without data.
        /// </summary>
        /// <param name="name">The name of the <see cref="ChartPointData"/>.</param>
        /// <returns>An empty <see cref="ChartPointData"/> object.</returns>
        public static ChartPointData CreateEmptyPointData(string name)
        {
            return new ChartPointData(Enumerable.Empty<Point2D>(), name);
        }

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
                Style = new ChartLineStyle(Color.SaddleBrown, 2, DashStyle.Solid)
            };
        }

        /// <summary>
        /// Create a <see cref="ChartData"/> with default styling based on the <paramref name="entryPoint"/>.
        /// </summary>
        /// <param name="entryPoint">The entry point for which to create <see cref="ChartData"/>.</param>
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

            return new ChartPointData(new[]
            {
                new Point2D(entryPoint, surfaceLine.GetZAtL(entryPoint)),
            }, Resources.PipingInput_EntryPointL_DisplayName)
            {
                Style = new ChartPointStyle(Color.Blue, 8, Color.Gray, 2, ChartPointSymbol.Triangle)
            };
        }

        /// <summary>
        /// Create a <see cref="ChartData"/> with default styling based on the <paramref name="exitPoint"/>.
        /// </summary>
        /// <param name="exitPoint">The entry point for which to create <see cref="ChartData"/>.</param>
        /// <param name="surfaceLine">The <see cref="RingtoetsPipingSurfaceLine"/> to place the entry point on.</param>
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

            return new ChartPointData(new[]
            {
                new Point2D(exitPoint, surfaceLine.GetZAtL(exitPoint)),
            }, Resources.PipingInput_ExitPointL_DisplayName)
            {
                Style = new ChartPointStyle(Color.Brown, 8, Color.Gray, 2, ChartPointSymbol.Triangle)
            };
        }

        /// <summary>
        /// Create a <see cref="ChartData"/> with default styling based on the <paramref name="surfaceLine.DitchPolderSide"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsPipingSurfaceLine"/> to place the ditch polder side on.</param>
        /// <returns><see cref="ChartData"/> based on the <paramref name="surfaceLine.DitchPolderSide"/>.</returns>
        /// /// <exception cref="ArgumentNullException">Thrown when <paramref name="surfaceLine"/> is <c>null</c>.</exception>
        public static ChartData CreateDitchPolderSide(RingtoetsPipingSurfaceLine surfaceLine)
        {
            if (surfaceLine == null)
            {
                throw new ArgumentNullException("surfaceLine");
            }

            return CreateLocalPoint(surfaceLine.DitchPolderSide, surfaceLine, PipingDataResources.CharacteristicPoint_DitchPolderSide, new ChartPointStyle(Color.Red, 8, Color.Transparent, 0, ChartPointSymbol.Circle));
        }

        /// <summary>
        /// Create a <see cref="ChartData"/> with default styling based on the <paramref name="surfaceLine.BottomDitchPolderSide"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsPipingSurfaceLine"/> to place the bottom ditch polder side on.</param>
        /// <returns><see cref="ChartData"/> based on the <paramref name="surfaceLine.BottomDitchPolderSide"/>.</returns>
        /// /// <exception cref="ArgumentNullException">Thrown when <paramref name="surfaceLine"/> is <c>null</c>.</exception>
        public static ChartData CreateBottomDitchPolderSide(RingtoetsPipingSurfaceLine surfaceLine)
        {
            if (surfaceLine == null)
            {
                throw new ArgumentNullException("surfaceLine");
            }

            return CreateLocalPoint(surfaceLine.BottomDitchPolderSide, surfaceLine, PipingDataResources.CharacteristicPoint_BottomDitchPolderSide, new ChartPointStyle(Color.Blue, 8, Color.Transparent, 0, ChartPointSymbol.Circle));
        }

        /// <summary>
        /// Create a <see cref="ChartData"/> with default styling based on the <paramref name="surfaceLine.BottomDitchDikeSide"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsPipingSurfaceLine"/> to place the bottom ditch dike side on.</param>
        /// <returns><see cref="ChartData"/> based on the <paramref name="surfaceLine.BottomDitchDikeSide"/>.</returns>
        /// /// <exception cref="ArgumentNullException">Thrown when <paramref name="surfaceLine"/> is <c>null</c>.</exception>
        public static ChartData CreateBottomDitchDikeSide(RingtoetsPipingSurfaceLine surfaceLine)
        {
            if (surfaceLine == null)
            {
                throw new ArgumentNullException("surfaceLine");
            }

            return CreateLocalPoint(surfaceLine.BottomDitchDikeSide, surfaceLine, PipingDataResources.CharacteristicPoint_BottomDitchDikeSide, new ChartPointStyle(Color.Green, 8, Color.Transparent, 0, ChartPointSymbol.Circle));
        }

        /// <summary>
        /// Create a <see cref="ChartData"/> with default styling based on the <paramref name="surfaceLine.DitchDikeSide"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsPipingSurfaceLine"/> to place the ditch dike side on.</param>
        /// <returns><see cref="ChartData"/> based on the <paramref name="surfaceLine.DitchDikeSide"/>.</returns>
        /// /// <exception cref="ArgumentNullException">Thrown when <paramref name="surfaceLine"/> is <c>null</c>.</exception>
        public static ChartData CreateDitchDikeSide(RingtoetsPipingSurfaceLine surfaceLine)
        {
            if (surfaceLine == null)
            {
                throw new ArgumentNullException("surfaceLine");
            }

            return CreateLocalPoint(surfaceLine.DitchDikeSide, surfaceLine, PipingDataResources.CharacteristicPoint_DitchDikeSide, new ChartPointStyle(Color.Purple, 8, Color.Transparent, 0, ChartPointSymbol.Circle));
        }

        /// <summary>
        /// Create a <see cref="ChartData"/> with default styling based on the <paramref name="surfaceLine.DikeToeAtRiver"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsPipingSurfaceLine"/> to place the dike toe at river on.</param>
        /// <returns><see cref="ChartData"/> based on the <paramref name="surfaceLine.DikeToeAtRiver"/>.</returns>
        /// /// <exception cref="ArgumentNullException">Thrown when <paramref name="surfaceLine"/> is <c>null</c>.</exception>
        public static ChartData CreateDikeToeAtRiver(RingtoetsPipingSurfaceLine surfaceLine)
        {
            if (surfaceLine == null)
            {
                throw new ArgumentNullException("surfaceLine");
            }

            return CreateLocalPoint(surfaceLine.DikeToeAtRiver, surfaceLine, PipingDataResources.CharacteristicPoint_DikeToeAtRiver, new ChartPointStyle(Color.Orange, 8, Color.Transparent, 0, ChartPointSymbol.Circle));
        }

        /// <summary>
        /// Create a <see cref="ChartData"/> with default styling based on the <paramref name="surfaceLine.DikeToeAtPolder"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsPipingSurfaceLine"/> to place the dike toe at polder on.</param>
        /// <returns><see cref="ChartData"/> based on the <paramref name="surfaceLine.DikeToeAtPolder"/>.</returns>
        /// /// <exception cref="ArgumentNullException">Thrown when <paramref name="surfaceLine"/> is <c>null</c>.</exception>
        public static ChartData CreateDikeToeAtPolder(RingtoetsPipingSurfaceLine surfaceLine)
        {
            if (surfaceLine == null)
            {
                throw new ArgumentNullException("surfaceLine");
            }

            return CreateLocalPoint(surfaceLine.DikeToeAtPolder, surfaceLine, PipingDataResources.CharacteristicPoint_DikeToeAtPolder, new ChartPointStyle(Color.Silver, 8, Color.Transparent, 0, ChartPointSymbol.Circle));
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