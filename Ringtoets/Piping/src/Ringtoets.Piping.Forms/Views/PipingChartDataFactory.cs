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
        /// <param name="surfaceLine">The <see cref="RingtoetsPipingSurfaceLine"/> to get place the entry point on.</param>
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
        /// Create a <see cref="ChartData"/> with default styling based on the <paramref name="ditchPolderSide"/>.
        /// </summary>
        /// <param name="ditchPolderSide">The <see cref="Point3D"/> for which to create <see cref="ChartData"/>.</param>
        /// <returns><see cref="ChartData"/> based on the <paramref name="ditchPolderSide"/>.</returns>
        /// /// <exception cref="ArgumentNullException">Thrown when <paramref name="ditchPolderSide"/> is <c>null</c>.</exception>
        public static ChartData CreateDitchPolderSide(Point3D ditchPolderSide)
        {
            if (ditchPolderSide == null)
            {
                throw new ArgumentNullException("ditchPolderSide");
            }

            return CreatePoint(ditchPolderSide, PipingDataResources.CharacteristicPoint_DitchPolderSide, new ChartPointStyle(Color.Red, 8, Color.Transparent, 0, ChartPointSymbol.Circle));
        }

        /// <summary>
        /// Create a <see cref="ChartData"/> with default styling based on the <paramref name="bottomDitchPolderSide"/>.
        /// </summary>
        /// <param name="bottomDitchPolderSide">The <see cref="Point3D"/> for which to create <see cref="ChartData"/>.</param>
        /// <returns><see cref="ChartData"/> based on the <paramref name="bottomDitchPolderSide"/>.</returns>
        /// /// <exception cref="ArgumentNullException">Thrown when <paramref name="bottomDitchPolderSide"/> is <c>null</c>.</exception>
        public static ChartData CreateBottomDitchPolderSide(Point3D bottomDitchPolderSide)
        {
            if (bottomDitchPolderSide == null)
            {
                throw new ArgumentNullException("bottomDitchPolderSide");
            }

            return CreatePoint(bottomDitchPolderSide, PipingDataResources.CharacteristicPoint_BottomDitchPolderSide, new ChartPointStyle(Color.Blue, 8, Color.Transparent, 0, ChartPointSymbol.Circle));
        }

        /// <summary>
        /// Create a <see cref="ChartData"/> with default styling based on the <paramref name="bottomDitchDikeSide"/>.
        /// </summary>
        /// <param name="bottomDitchDikeSide">The <see cref="Point3D"/> for which to create <see cref="ChartData"/>.</param>
        /// <returns><see cref="ChartData"/> based on the <paramref name="bottomDitchDikeSide"/>.</returns>
        /// /// <exception cref="ArgumentNullException">Thrown when <paramref name="bottomDitchDikeSide"/> is <c>null</c>.</exception>
        public static ChartData CreateBottomDitchDikeSide(Point3D bottomDitchDikeSide)
        {
            if (bottomDitchDikeSide == null)
            {
                throw new ArgumentNullException("bottomDitchDikeSide");
            }

            return CreatePoint(bottomDitchDikeSide, PipingDataResources.CharacteristicPoint_BottomDitchDikeSide, new ChartPointStyle(Color.Green, 8, Color.Transparent, 0, ChartPointSymbol.Circle));
        }

        /// <summary>
        /// Create a <see cref="ChartData"/> with default styling based on the <paramref name="ditchDikeSide"/>.
        /// </summary>
        /// <param name="ditchDikeSide">The <see cref="Point3D"/> for which to create <see cref="ChartData"/>.</param>
        /// <returns><see cref="ChartData"/> based on the <paramref name="ditchDikeSide"/>.</returns>
        /// /// <exception cref="ArgumentNullException">Thrown when <paramref name="ditchDikeSide"/> is <c>null</c>.</exception>
        public static ChartData CreateDitchDikeSide(Point3D ditchDikeSide)
        {
            if (ditchDikeSide == null)
            {
                throw new ArgumentNullException("ditchDikeSide");
            }

            return CreatePoint(ditchDikeSide, PipingDataResources.CharacteristicPoint_DitchDikeSide, new ChartPointStyle(Color.Purple, 8, Color.Transparent, 0, ChartPointSymbol.Circle));
        }

        /// <summary>
        /// Create a <see cref="ChartData"/> with default styling based on the <paramref name="dikeToeAtRiver"/>.
        /// </summary>
        /// <param name="dikeToeAtRiver">The <see cref="Point3D"/> for which to create <see cref="ChartData"/>.</param>
        /// <returns><see cref="ChartData"/> based on the <paramref name="dikeToeAtRiver"/>.</returns>
        /// /// <exception cref="ArgumentNullException">Thrown when <paramref name="dikeToeAtRiver"/> is <c>null</c>.</exception>
        public static ChartData CreateDikeToeAtRiver(Point3D dikeToeAtRiver)
        {
            if (dikeToeAtRiver == null)
            {
                throw new ArgumentNullException("dikeToeAtRiver");
            }

            return CreatePoint(dikeToeAtRiver, PipingDataResources.CharacteristicPoint_DikeToeAtRiver, new ChartPointStyle(Color.Orange, 8, Color.Transparent, 0, ChartPointSymbol.Circle));
        }

        /// <summary>
        /// Create a <see cref="ChartData"/> with default styling based on the <paramref name="dikeToeAtPolder"/>.
        /// </summary>
        /// <param name="dikeToeAtPolder">The <see cref="Point3D"/> for which to create <see cref="ChartData"/>.</param>
        /// <returns><see cref="ChartData"/> based on the <paramref name="dikeToeAtPolder"/>.</returns>
        /// /// <exception cref="ArgumentNullException">Thrown when <paramref name="dikeToeAtPolder"/> is <c>null</c>.</exception>
        public static ChartData CreateDikeToeAtPolder(Point3D dikeToeAtPolder)
        {
            if (dikeToeAtPolder == null)
            {
                throw new ArgumentNullException("dikeToeAtPolder");
            }

            return CreatePoint(dikeToeAtPolder, PipingDataResources.CharacteristicPoint_DikeToeAtPolder, new ChartPointStyle(Color.Silver, 8, Color.Transparent, 0, ChartPointSymbol.Circle));
        }

        private static ChartData CreatePoint(Point3D dikeToeAtRiver, string name, ChartPointStyle style)
        {
            return new ChartPointData(new[]
            {
                Point3DToPoint2D(dikeToeAtRiver)
            }, name)
            {
                Style = style
            };
        }

        private static Point2D Point3DToPoint2D(Point3D point3D)
        {
            return new Point2D(point3D.X, point3D.Z);
        }
    }
}