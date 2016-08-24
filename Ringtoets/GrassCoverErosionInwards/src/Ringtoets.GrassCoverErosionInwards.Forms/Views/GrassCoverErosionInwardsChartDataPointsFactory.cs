﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Linq;
using Core.Common.Base.Geometry;
using Core.Components.Charting.Data;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Views
{
    /// <summary>
    /// Factory for creating arrays of <see cref="Point2D"/> to use in <see cref="ChartData"/>
    /// (created via <see cref="GrassCoverErosionInwardsChartDataFactory"/>).
    /// </summary>
    public static class GrassCoverErosionInwardsChartDataPointsFactory
    {
        /// <summary>
        /// Create dike geometry points in 2D space based on the provided <paramref name="dikeProfile"/>.
        /// </summary>
        /// <param name="dikeProfile">The <see cref="DikeProfile"/> to create the dike geometry points for.</param>
        /// <returns>An array of points in 2D space or an empty array when <paramref name="dikeProfile"/> is <c>null</c>.</returns>
        public static Point2D[] CreateDikeGeometryPoints(DikeProfile dikeProfile)
        {
            return dikeProfile != null
                       ? dikeProfile.DikeGeometry.Select(dg => dg.Point).ToArray()
                       : new Point2D[0];
        }

        /// <summary>
        /// Create foreshore geometry points in 2D space based on the provided <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The <see cref="GrassCoverErosionInwardsInput"/> to create the foreshore geometry points for.</param>
        /// <returns>An array of points in 2D space or an empty array when:
        /// <list type="bullet">
        /// <item><paramref name="input"/> is <c>null</c>;</item>
        /// <item>the <see cref="DikeProfile"/> in <paramref name="input"/> is <c>null</c>;</item>
        /// <item>the foreshore in <paramref name="input"/> is not in use.</item>
        /// </list>
        /// </returns>
        public static Point2D[] CreateForeshoreGeometryPoints(GrassCoverErosionInwardsInput input)
        {
            return input != null && input.DikeProfile != null && input.UseForeshore
                       ? input.DikeProfile.ForeshoreGeometry.ToArray()
                       : new Point2D[0];
        }

        /// <summary>
        /// Create dike height points in 2D space based on the provided <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The <see cref="GrassCoverErosionInwardsInput"/> to create the dike height points for.</param>
        /// <returns>An array of points in 2D space or an empty array when:
        /// <list type="bullet">
        /// <item><paramref name="input"/> is <c>null</c>;</item>
        /// <item>the dike profile in <paramref name="input"/> is <c>null</c>;</item>
        /// <item>the dike height in <paramref name="input"/> is <c>double.NaN</c>;</item>
        /// <item>the dike geometry in <paramref name="input"/> contains less than two points.</item>
        /// </list>
        /// </returns>
        public static Point2D[] CreateDikeHeightPoints(GrassCoverErosionInwardsInput input)
        {
            if (input == null || input.DikeProfile == null || double.IsNaN(input.DikeHeight) || input.DikeProfile.DikeGeometry.Length < 2)
            {
                return new Point2D[0];
            }

            var dikeProfile = input.DikeProfile;
            var roughnessPoints = dikeProfile.DikeGeometry;

            return new[]
            {
                new Point2D(roughnessPoints.First().Point.X, input.DikeHeight),
                new Point2D(roughnessPoints.Last().Point.X, input.DikeHeight)
            };
        }
    }
}