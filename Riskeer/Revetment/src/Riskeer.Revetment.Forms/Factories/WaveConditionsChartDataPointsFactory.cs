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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Components.Chart.Data;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Forms.Factories;
using Riskeer.Revetment.Data;

namespace Riskeer.Revetment.Forms.Factories
{
    /// <summary>
    /// Factory for creating collections of <see cref="Point2D"/> to use in <see cref="ChartData"/>
    /// (created via <see cref="RingtoetsChartDataFactory"/> and <see cref="WaveConditionsChartDataFactory"/>) .
    /// </summary>
    internal static class WaveConditionsChartDataPointsFactory
    {
        /// <summary>
        /// Create foreshore geometry points in 2D space based on the provided <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The <see cref="WaveConditionsInput"/> to create the foreshore geometry points for.</param>
        /// <returns>A collection of points in 2D space or an empty collection when:
        /// <list type="bullet">
        /// <item><paramref name="input"/> is <c>null</c>;</item>
        /// <item>the <see cref="ForeshoreProfile"/> in <paramref name="input"/> is <c>null</c>;</item>
        /// <item>the foreshore in <paramref name="input"/> is not in use.</item>
        /// </list>
        /// </returns>
        public static IEnumerable<Point2D> CreateForeshoreGeometryPoints(WaveConditionsInput input)
        {
            return input?.ForeshoreProfile != null && input.UseForeshore
                       ? input.ForeshoreGeometry.ToArray()
                       : new Point2D[0];
        }

        /// <summary>
        /// Create revetment geometry points in 2D space based on the provided <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The <see cref="WaveConditionsInput"/> to create the revetment geometry points for.</param>
        /// <returns>A collection of points in 2D space or an empty collection when:
        /// <list type="bullet">
        /// <item><paramref name="input"/> is <c>null</c>;</item>
        /// <item>the <see cref="WaveConditionsInput.LowerBoundaryRevetment"/> is not set;</item>
        /// <item>the <see cref="WaveConditionsInput.UpperBoundaryRevetment"/> is not set.</item>
        /// </list>
        /// </returns>
        public static IEnumerable<Point2D> CreateRevetmentGeometryPoints(WaveConditionsInput input)
        {
            if (input == null
                || double.IsNaN(input.LowerBoundaryRevetment)
                || double.IsNaN(input.UpperBoundaryRevetment))
            {
                return new Point2D[0];
            }

            double startPointX = GetPointXOnRevetmentLine(input, input.LowerBoundaryRevetment);
            double endPointX = GetPointXOnRevetmentLine(input, input.UpperBoundaryRevetment);

            return new[]
            {
                new Point2D(startPointX, input.LowerBoundaryRevetment),
                new Point2D(endPointX, input.UpperBoundaryRevetment)
            };
        }

        /// <summary>
        /// Create revetment base geometry points in 2D space based on the provided <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The <see cref="WaveConditionsInput"/> to create the revetment base geometry points for.</param>
        /// <returns>A collection of points in 2D space or an empty collection when:
        /// <list type="bullet">
        /// <item><paramref name="input"/> is <c>null</c>;</item>
        /// <item>the <see cref="WaveConditionsInput.LowerBoundaryRevetment"/> is not set;</item>
        /// <item>the <see cref="WaveConditionsInput.UpperBoundaryRevetment"/> is not set.</item>
        /// </list>
        /// </returns>
        public static IEnumerable<Point2D> CreateRevetmentBaseGeometryPoints(WaveConditionsInput input)
        {
            if (input == null
                || double.IsNaN(input.LowerBoundaryRevetment)
                || double.IsNaN(input.UpperBoundaryRevetment))
            {
                return new Point2D[0];
            }

            var points = new List<Point2D>
            {
                GetBaseStartPoint(input),
                new Point2D(GetPointXOnRevetmentLine(input, input.LowerBoundaryRevetment), input.LowerBoundaryRevetment)
            };

            if (input.LowerBoundaryWaterLevels < points.First().Y)
            {
                points.Insert(0, new Point2D(GetPointXOnRevetmentLine(input, input.LowerBoundaryWaterLevels), input.LowerBoundaryWaterLevels));
            }

            return points.ToArray();
        }

        /// <summary>
        /// Create lower boundary revetment geometry points in 2D space based on the provided <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The <see cref="WaveConditionsInput"/> to create the lower boundary revetment geometry points for.</param>
        /// <returns>A collection of points in 2D space or an empty collection when:
        /// <list type="bullet">
        /// <item><paramref name="input"/> is <c>null</c>;</item>
        /// <item>the <see cref="WaveConditionsInput.LowerBoundaryRevetment"/> is not set.</item>
        /// </list>
        /// </returns>
        public static IEnumerable<Point2D> CreateLowerBoundaryRevetmentGeometryPoints(WaveConditionsInput input)
        {
            return input != null
                       ? CreateGeometryPoints(input, () => input.LowerBoundaryRevetment)
                       : new Point2D[0];
        }

        /// <summary>
        /// Create upper boundary revetment geometry points in 2D space based on the provided <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The <see cref="WaveConditionsInput"/> to create the upper boundary revetment geometry points for.</param>
        /// <returns>A collection of points in 2D space or an empty collection when:
        /// <list type="bullet">
        /// <item><paramref name="input"/> is <c>null</c>;</item>
        /// <item>the <see cref="WaveConditionsInput.UpperBoundaryRevetment"/> is not set.</item>
        /// </list>
        /// </returns>
        public static IEnumerable<Point2D> CreateUpperBoundaryRevetmentGeometryPoints(WaveConditionsInput input)
        {
            return input != null
                       ? CreateGeometryPoints(input, () => input.UpperBoundaryRevetment)
                       : new Point2D[0];
        }

        /// <summary>
        /// Create lower boundary water levels geometry points in 2D space based on the provided <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The <see cref="WaveConditionsInput"/> to create the lower boundary water levels geometry points for.</param>
        /// <returns>A collection of points in 2D space or an empty collection when:
        /// <list type="bullet">
        /// <item><paramref name="input"/> is <c>null</c>;</item>
        /// <item>the <see cref="WaveConditionsInput.LowerBoundaryWaterLevels"/> is not set.</item>
        /// </list>
        /// </returns>
        public static IEnumerable<Point2D> CreateLowerBoundaryWaterLevelsGeometryPoints(WaveConditionsInput input)
        {
            return input != null
                       ? CreateGeometryPoints(input, () => input.LowerBoundaryWaterLevels)
                       : new Point2D[0];
        }

        /// <summary>
        /// Create upper boundary water levels geometry points in 2D space based on the provided <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The <see cref="WaveConditionsInput"/> to create the upper boundary water levels geometry points for.</param>
        /// <returns>A collection of points in 2D space or an empty collection when:
        /// <list type="bullet">
        /// <item><paramref name="input"/> is <c>null</c>;</item>
        /// <item>the <see cref="WaveConditionsInput.UpperBoundaryWaterLevels"/> is not set.</item>
        /// </list>
        /// </returns>
        public static IEnumerable<Point2D> CreateUpperBoundaryWaterLevelsGeometryPoints(WaveConditionsInput input)
        {
            return input != null
                       ? CreateGeometryPoints(input, () => input.UpperBoundaryWaterLevels)
                       : new Point2D[0];
        }

        /// <summary>
        /// Create assessment level geometry points in 2D space based on the provided <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The <see cref="WaveConditionsInput"/> to create the assessment level geometry points for.</param>
        /// <param name="assessmentLevel">The assessment level to use while determining water levels.</param>
        /// <returns>A collection of points in 2D space or an empty collection when:
        /// <list type="bullet">
        /// <item><paramref name="input"/> is <c>null</c>;</item>
        /// <item><see cref="WaveConditionsInput.HydraulicBoundaryLocation"/> is <c>null</c>;</item>
        /// <item>the assessment level is <see cref="RoundedDouble.NaN"/>.</item>
        /// </list>
        /// </returns>
        public static IEnumerable<Point2D> CreateAssessmentLevelGeometryPoints(WaveConditionsInput input,
                                                                               RoundedDouble assessmentLevel)
        {
            return input != null
                       ? CreateGeometryPoints(input, () => assessmentLevel)
                       : new Point2D[0];
        }

        /// <summary>
        /// Create water levels geometry points in 2D space based on the provided <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The <see cref="WaveConditionsInput"/> to create the water levels geometry points for.</param>
        /// <param name="assessmentLevel">The assessment level to use while determining water levels.</param>
        /// <returns>A collection with collections of points in 2D space or an empty list when:
        /// <list type="bullet">
        /// <item><paramref name="input"/> is <c>null</c>;</item>
        /// <item>no water levels could be determined.</item>
        /// </list>
        /// </returns>
        public static IEnumerable<IEnumerable<Point2D>> CreateWaterLevelsGeometryPoints(WaveConditionsInput input, RoundedDouble assessmentLevel)
        {
            return input?.GetWaterLevels(assessmentLevel)
                        .Select(waterLevel => CreateGeometryPoints(input, () => waterLevel))
                        .ToArray() ?? new IEnumerable<Point2D>[0];
        }

        private static IEnumerable<Point2D> CreateGeometryPoints(WaveConditionsInput input, Func<double> getValueFunc)
        {
            double value = getValueFunc();

            return double.IsNaN(value)
                       ? new Point2D[0]
                       : new[]
                       {
                           new Point2D(GetForeshoreProfileStartX(input), value),
                           new Point2D(GetPointXOnRevetmentLine(input, value), value)
                       };
        }

        private static double GetForeshoreProfileStartX(WaveConditionsInput input)
        {
            return input.ForeshoreProfile != null && input.UseForeshore
                       ? input.ForeshoreGeometry.First().X
                       : -10;
        }

        private static double GetPointXOnRevetmentLine(WaveConditionsInput input, double valueY)
        {
            Point2D baseStartPoint = GetBaseStartPoint(input);
            double endPointX = ((valueY - baseStartPoint.Y) / 3) + baseStartPoint.X;
            return endPointX;
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