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
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.Properties;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Views
{
    /// <summary>
    /// Factory for creating <see cref="ChartData"/> based on information used as input
    /// in the grass cover erosion inwards failure mechanism.
    /// </summary>
    public static class GrassCoverErosionInwardsChartDataFactory
    {
        /// <summary>
        /// Create <see cref="ChartData"/> with default styling based on the <paramref name="dikeGeometry"/>.
        /// </summary>
        /// <param name="dikeGeometry">The geometry of the <see cref="DikeProfile"/> for which to create 
        /// <see cref="ChartData"/>.</param>
        /// <param name="name">The name of the <see cref="DikeProfile"/>.</param>
        /// <returns><see cref="ChartData"/> based on the <paramref name="dikeGeometry"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dikeGeometry"/> 
        /// is <c>null</c>.</exception>
        public static ChartData Create(RoughnessPoint[] dikeGeometry, string name)
        {
            if (dikeGeometry == null)
            {
                throw new ArgumentNullException("dikeGeometry");
            }

            return new ChartLineData(dikeGeometry.Select(dg => dg.Point), string.Format(Resources.GrassCoverErosionInwardsChartDataFactory_Create_Name_format, Resources.DikeProfile_DisplayName, name))
            {
                Style = new ChartLineStyle(Color.SaddleBrown, 2, DashStyle.Solid)
            };
        }

        /// <summary>
        /// Create <see cref="ChartData"/> with default styling based on the <paramref name="foreshoreGeometry"/>.
        /// </summary>
        /// <param name="foreshoreGeometry">The forshore geometry of the <see cref="DikeProfile"/> 
        /// for which to create <see cref="ChartData"/>.</param>
        /// <param name="name">The name of the <see cref="DikeProfile"/>.</param>
        /// <returns><see cref="ChartData"/> based on the <paramref name="foreshoreGeometry"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="foreshoreGeometry"/> 
        /// is <c>null</c>.</exception>
        public static ChartData Create(Point2D[] foreshoreGeometry, string name)
        {
            if (foreshoreGeometry == null)
            {
                throw new ArgumentNullException("foreshoreGeometry");
            }

            return new ChartLineData(foreshoreGeometry, string.Format(Resources.GrassCoverErosionInwardsChartDataFactory_Create_Name_format, Resources.Foreshore_DisplayName, name))
            {
                Style = new ChartLineStyle(Color.DarkOrange, 2, DashStyle.Solid)
            };
        }

        /// <summary>
        /// Create <see cref="ChartData"/> with default styling based on the <paramref name="dikeHeight"/>.
        /// </summary>
        /// <param name="dikeHeight">The dike height of the <see cref="DikeProfile"/> for which
        /// to create <see cref="ChartData"/>.</param>
        /// <param name="dikeGeometry">The geometry of the <see cref="DikeProfile"/> to place the
        /// <paramref name="dikeHeight"/> on.</param>
        /// <param name="name">The name of the <see cref="DikeProfile"/>.</param>
        /// <returns><see cref="ChartData"/> based on the <paramref name="dikeHeight"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="dikeHeight"/> is <c>NaN</c>.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dikeGeometry"/> 
        /// is <c>null</c>.</exception>
        public static ChartData Create(RoundedDouble dikeHeight, RoughnessPoint[] dikeGeometry, string name)
        {
            if (double.IsNaN(dikeHeight))
            {
                throw new ArgumentException("Dike height should have a value.", "dikeHeight");
            }

            if (dikeGeometry == null)
            {
                throw new ArgumentNullException("dikeGeometry");
            }

            return new ChartLineData(CreateDikeHeightData(dikeHeight, dikeGeometry), string.Format(Resources.GrassCoverErosionInwardsChartDataFactory_Create_Name_format, Resources.DikeHeight_ChartName, name))
            {
                Style = new ChartLineStyle(Color.MediumSeaGreen, 2, DashStyle.Dash)
            };
        }

        private static IEnumerable<Point2D> CreateDikeHeightData(RoundedDouble dikeHeight, RoughnessPoint[] dikeGeometry)
        {
            return new[]
            {
                new Point2D(dikeGeometry.First().Point.X, dikeHeight),
                new Point2D(dikeGeometry.Last().Point.X, dikeHeight)
            };
        }
    }
}