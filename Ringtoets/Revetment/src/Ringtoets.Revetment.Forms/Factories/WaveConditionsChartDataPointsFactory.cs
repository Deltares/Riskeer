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

using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Components.Charting.Data;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Forms.Factories;
using Ringtoets.Revetment.Data;

namespace Ringtoets.Revetment.Forms.Factories
{
    /// <summary>
    /// Factory for creating arrays of <see cref="Point2D"/> to use in <see cref="ChartData"/>
    /// (created via <see cref="RingtoetsChartDataFactory"/>).
    /// </summary>
    internal static class WaveConditionsChartDataPointsFactory
    {
        /// <summary>
        /// Create foreshore geometry points in 2D space based on the provided <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The <see cref="WaveConditionsInput"/> to create the foreshore geometry points for.</param>
        /// <returns>An array of points in 2D space or an empty array when:
        /// <list type="bullet">
        /// <item><paramref name="input"/> is <c>null</c>;</item>
        /// <item>the <see cref="ForeshoreProfile"/> in <paramref name="input"/> is <c>null</c>;</item>
        /// <item>the foreshore in <paramref name="input"/> is not in use.</item>
        /// </list>
        /// </returns>
        public static Point2D[] CreateForeshoreGeometryPoints(WaveConditionsInput input)
        {
            return input?.ForeshoreProfile != null && input.UseForeshore
                       ? input.ForeshoreGeometry.ToArray()
                       : new Point2D[0];
        }

        /// <summary>
        /// Create revetment geometry points in 2D space based on the provided <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The <see cref="WaveConditionsInput"/> to create the revetment geometry points for.</param>
        /// <returns>An array of points in 2D space or an empty array when:
        /// <list type="bullet">
        /// <item><paramref name="input"/> is <c>null</c>;</item>
        /// <item>the <see cref="WaveConditionsInput.LowerBoundaryRevetment"/> is not set;</item>
        /// <item>the <see cref="WaveConditionsInput.UpperBoundaryRevetment"/> is not set;</item>
        /// </list>
        /// </returns>
        public static Point2D[] CreateRevetmentGeometryPoints(WaveConditionsInput input)
        {
            if (input == null
                || double.IsNaN(input.LowerBoundaryRevetment)
                || double.IsNaN(input.UpperBoundaryRevetment))
            {
                return new Point2D[0];
            }

            Point2D baseStartPoint = GetBaseStartPoint(input);

            double startPointX = (input.LowerBoundaryRevetment - baseStartPoint.Y) / 3;
            RoundedDouble heightDiff = input.UpperBoundaryRevetment - input.LowerBoundaryRevetment;

            return new[]
            {
                new Point2D(startPointX + baseStartPoint.X, input.LowerBoundaryRevetment),
                new Point2D(heightDiff / 3 + startPointX, input.UpperBoundaryRevetment)
            };
        }

        /// <summary>
        /// Create revetment base geometry points in 2D space based on the provided <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The <see cref="WaveConditionsInput"/> to create the revetment base geometry points for.</param>
        /// <returns>An array of points in 2D space or an empty array when:
        /// <list type="bullet">
        /// <item><paramref name="input"/> is <c>null</c>;</item>
        /// <item>the <see cref="WaveConditionsInput.LowerBoundaryRevetment"/> is not set;</item>
        /// <item>the <see cref="WaveConditionsInput.UpperBoundaryRevetment"/> is not set;</item>
        /// </list>
        /// </returns>
        public static Point2D[] CreateRevetmentBaseGeometryPoints(WaveConditionsInput input)
        {
            if (input == null
                || double.IsNaN(input.LowerBoundaryRevetment)
                || double.IsNaN(input.UpperBoundaryRevetment))
            {
                return new Point2D[0];
            }

            Point2D startPoint = GetBaseStartPoint(input);

            double heightDiff = input.LowerBoundaryRevetment - startPoint.Y;

            return new[]
            {
                new Point2D(startPoint.X, startPoint.Y),
                new Point2D(heightDiff / 3 + startPoint.X, input.LowerBoundaryRevetment)
            };
        }

        public static Point2D[] CreateLowerBoundaryRevetmentGeometryPoints(WaveConditionsInput input)
        {
            if (input == null || double.IsNaN(input.LowerBoundaryRevetment))
            {
                return new Point2D[0];
            }

            double startPointX = input.ForeshoreProfile != null && input.UseForeshore
                                  ? input.ForeshoreGeometry.First().X
                                  : -10;

            Point2D baseStartPoint = GetBaseStartPoint(input);
            double endPointX = (input.LowerBoundaryRevetment - baseStartPoint.Y) / 3;

            return new[]
           {
                new Point2D(startPointX, input.LowerBoundaryRevetment),
                new Point2D(endPointX + baseStartPoint.X, input.LowerBoundaryRevetment)
            };
        }

        private static Point2D GetBaseStartPoint(WaveConditionsInput input)
        {
            double startPointX = input.ForeshoreProfile != null && input.UseForeshore
                                     ? input.ForeshoreGeometry.Last().X
                                     : 0;

            double startPointY = input.ForeshoreProfile != null && input.UseForeshore
                                     ? input.ForeshoreGeometry.Last().Y
                                     : 0;

            return new Point2D(startPointX, startPointY);
        }
    }
}