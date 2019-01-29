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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Components.Chart.Data;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.GrassCoverErosionInwards.Data;

namespace Riskeer.GrassCoverErosionInwards.Forms.Factories
{
    /// <summary>
    /// Factory for creating collections of <see cref="Point2D"/> to use in <see cref="ChartData"/>
    /// (created via <see cref="GrassCoverErosionInwardsChartDataFactory"/>).
    /// </summary>
    internal static class GrassCoverErosionInwardsChartDataPointsFactory
    {
        /// <summary>
        /// Create dike geometry points in 2D space based on the provided <paramref name="dikeProfile"/>.
        /// </summary>
        /// <param name="dikeProfile">The <see cref="DikeProfile"/> to create the dike geometry points for.</param>
        /// <returns>A collection of points in 2D space or an empty collection when <paramref name="dikeProfile"/> is <c>null</c>.</returns>
        public static IEnumerable<Point2D> CreateDikeGeometryPoints(DikeProfile dikeProfile)
        {
            return dikeProfile?.DikeGeometry.Select(dg => dg.Point) ?? new Point2D[0];
        }

        /// <summary>
        /// Create foreshore geometry points in 2D space based on the provided <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The <see cref="GrassCoverErosionInwardsInput"/> to create the foreshore geometry points for.</param>
        /// <returns>A collection of points in 2D space or an empty collection when:
        /// <list type="bullet">
        /// <item><paramref name="input"/> is <c>null</c>;</item>
        /// <item>the <see cref="DikeProfile"/> in <paramref name="input"/> is <c>null</c>;</item>
        /// <item>the foreshore in <paramref name="input"/> is not in use.</item>
        /// </list>
        /// </returns>
        public static IEnumerable<Point2D> CreateForeshoreGeometryPoints(GrassCoverErosionInwardsInput input)
        {
            return input?.DikeProfile != null && input.UseForeshore
                       ? input.DikeProfile.ForeshoreGeometry.ToArray()
                       : new Point2D[0];
        }

        /// <summary>
        /// Create dike height points in 2D space based on the provided <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The <see cref="GrassCoverErosionInwardsInput"/> to create the dike height points for.</param>
        /// <returns>A collection of points in 2D space or an empty collection when:
        /// <list type="bullet">
        /// <item><paramref name="input"/> is <c>null</c>;</item>
        /// <item>the dike profile in <paramref name="input"/> is <c>null</c>;</item>
        /// <item>the dike height in <paramref name="input"/> is <c>double.NaN</c>;</item>
        /// <item>the dike geometry in <paramref name="input"/> contains less than two points.</item>
        /// </list>
        /// </returns>
        public static IEnumerable<Point2D> CreateDikeHeightPoints(GrassCoverErosionInwardsInput input)
        {
            if (input?.DikeProfile == null || double.IsNaN(input.DikeHeight) || input.DikeProfile.DikeGeometry.Count() < 2)
            {
                return new Point2D[0];
            }

            DikeProfile dikeProfile = input.DikeProfile;
            IEnumerable<RoughnessPoint> roughnessPoints = dikeProfile.DikeGeometry;

            return new[]
            {
                new Point2D(roughnessPoints.First().Point.X, input.DikeHeight),
                new Point2D(roughnessPoints.Last().Point.X, input.DikeHeight)
            };
        }
    }
}