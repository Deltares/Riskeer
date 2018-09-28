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
using Core.Common.Util.Extensions;
using Core.Components.Chart.Data;
using Ringtoets.MacroStabilityInwards.Data;

namespace Ringtoets.MacroStabilityInwards.Forms.Factories
{
    /// <summary>
    /// Factory for creating collections of points in 2D space for a collection of <see cref="MacroStabilityInwardsSlice"/> 
    /// to use in <see cref="ChartData"/> (created via <see cref="MacroStabilityInwardsSliceChartDataFactory"/>).
    /// </summary>
    internal static class MacroStabilityInwardsSliceChartDataPointsFactory
    {
        /// <summary>
        /// Create lines of the slices based on the provided <paramref name="slidingCurve"/>.
        /// </summary>
        /// <param name="slidingCurve">The <see cref="MacroStabilityInwardsSlidingCurve"/> containing the slices
        /// to create the areas for.</param>
        /// <returns>A collection of collections of points in 2D space or an empty collection when <paramref name="slidingCurve"/>
        /// is <c>null</c>.</returns>
        public static IEnumerable<IEnumerable<Point2D>> CreateSliceAreas(MacroStabilityInwardsSlidingCurve slidingCurve)
        {
            if (slidingCurve == null)
            {
                return Enumerable.Empty<Point2D[]>();
            }

            return slidingCurve.Slices.Select(slice => new[]
            {
                slice.TopLeftPoint,
                slice.TopRightPoint,
                slice.BottomRightPoint,
                slice.BottomLeftPoint
            }).ToArray();
        }

        /// <summary>
        /// Creates the areas for <see cref="MacroStabilityInwardsSlice.Cohesion"/>
        /// values in <paramref name="slidingCurve"/>.
        /// </summary>
        /// <param name="slidingCurve">The <see cref="MacroStabilityInwardsSlidingCurve"/> to 
        /// get the slices from.</param>
        /// <returns>A collection of collections of points in 2D space containing areas representing the
        /// slice output values, or an empty collection when <paramref name="slidingCurve"/> is <c>null</c>.
        /// </returns>
        public static IEnumerable<IEnumerable<Point2D>> CreateCohesionAreas(MacroStabilityInwardsSlidingCurve slidingCurve)
        {
            return CreateParameterAreas(slidingCurve?.Slices, slice => slice.Cohesion, 0.125);
        }

        /// <summary>
        /// Creates the areas for <see cref="MacroStabilityInwardsSlice.EffectiveStress"/>
        /// values in <paramref name="slidingCurve"/>.
        /// </summary>
        /// <param name="slidingCurve">The <see cref="MacroStabilityInwardsSlidingCurve"/> to 
        /// get the slices from.</param>
        /// <returns>A collection of collections of points in 2D space containing areas representing the
        /// slice output values, or an empty collection when <paramref name="slidingCurve"/> is <c>null</c>.
        /// </returns>
        public static IEnumerable<IEnumerable<Point2D>> CreateEffectiveStressAreas(MacroStabilityInwardsSlidingCurve slidingCurve)
        {
            return CreateParameterAreas(slidingCurve?.Slices, slice => slice.EffectiveStress, 0.125);
        }

        /// <summary>
        /// Creates the areas for <see cref="MacroStabilityInwardsSlice.EffectiveStressDaily"/>
        /// values in <paramref name="slidingCurve"/>.
        /// </summary>
        /// <param name="slidingCurve">The <see cref="MacroStabilityInwardsSlidingCurve"/> to 
        /// get the slices from.</param>
        /// <returns>A collection of collections of points in 2D space containing areas representing the
        /// slice output values, or an empty collection when <paramref name="slidingCurve"/> is <c>null</c>.
        /// </returns>
        public static IEnumerable<IEnumerable<Point2D>> CreateEffectiveStressDailyAreas(MacroStabilityInwardsSlidingCurve slidingCurve)
        {
            return CreateParameterAreas(slidingCurve?.Slices, slice => slice.EffectiveStressDaily, 0.125);
        }

        /// <summary>
        /// Creates the areas for <see cref="MacroStabilityInwardsSlice.TotalPorePressure"/>
        /// values in <paramref name="slidingCurve"/>.
        /// </summary>
        /// <param name="slidingCurve">The <see cref="MacroStabilityInwardsSlidingCurve"/> to 
        /// get the slices from.</param>
        /// <returns>A collection of collections of points in 2D space containing areas representing the
        /// slice output values, or an empty collection when <paramref name="slidingCurve"/> is <c>null</c>.
        /// </returns>
        public static IEnumerable<IEnumerable<Point2D>> CreateTotalPorePressureAreas(MacroStabilityInwardsSlidingCurve slidingCurve)
        {
            return CreateParameterAreas(slidingCurve?.Slices, slice => slice.TotalPorePressure, 0.125);
        }

        /// <summary>
        /// Creates the areas for <see cref="MacroStabilityInwardsSlice.Weight"/>
        /// values in <paramref name="slidingCurve"/>.
        /// </summary>
        /// <param name="slidingCurve">The <see cref="MacroStabilityInwardsSlidingCurve"/> to 
        /// get the slices from.</param>
        /// <returns>A collection of collections of points in 2D space containing areas representing the
        /// slice output values, or an empty collection when <paramref name="slidingCurve"/> is <c>null</c>.
        /// </returns>
        public static IEnumerable<IEnumerable<Point2D>> CreateWeightAreas(MacroStabilityInwardsSlidingCurve slidingCurve)
        {
            return CreateParameterAreas(slidingCurve?.Slices, slice => slice.Weight, 0.125);
        }

        /// <summary>
        /// Creates the areas for <see cref="MacroStabilityInwardsSlice.PiezometricPorePressure"/>
        /// values in <paramref name="slidingCurve"/>.
        /// </summary>
        /// <param name="slidingCurve">The <see cref="MacroStabilityInwardsSlidingCurve"/> to 
        /// get the slices from.</param>
        /// <returns>A collection of collections of points in 2D space containing areas representing the
        /// slice output values, or an empty collection when <paramref name="slidingCurve"/> is <c>null</c>.
        /// </returns>
        public static IEnumerable<IEnumerable<Point2D>> CreatePiezometricPorePressureAreas(MacroStabilityInwardsSlidingCurve slidingCurve)
        {
            return CreateParameterAreas(slidingCurve?.Slices, slice => slice.PiezometricPorePressure, 0.125);
        }

        /// <summary>
        /// Creates the areas for <see cref="MacroStabilityInwardsSlice.PorePressure"/>
        /// values in <paramref name="slidingCurve"/>.
        /// </summary>
        /// <param name="slidingCurve">The <see cref="MacroStabilityInwardsSlidingCurve"/> to 
        /// get the slices from.</param>
        /// <returns>A collection of collections of points in 2D space containing areas representing the
        /// slice output values, or an empty collection when <paramref name="slidingCurve"/> is <c>null</c>.
        /// </returns>
        public static IEnumerable<IEnumerable<Point2D>> CreatePorePressureAreas(MacroStabilityInwardsSlidingCurve slidingCurve)
        {
            return CreateParameterAreas(slidingCurve?.Slices, slice => slice.PorePressure, 0.125);
        }

        /// <summary>
        /// Creates the areas for <see cref="MacroStabilityInwardsSlice.VerticalPorePressure"/>
        /// values in <paramref name="slidingCurve"/>.
        /// </summary>
        /// <param name="slidingCurve">The <see cref="MacroStabilityInwardsSlidingCurve"/> to 
        /// get the slices from.</param>
        /// <returns>A collection of collections of points in 2D space containing areas representing the
        /// slice output values, or an empty collection when <paramref name="slidingCurve"/> is <c>null</c>.
        /// </returns>
        public static IEnumerable<IEnumerable<Point2D>> CreateVerticalPorePressureAreas(MacroStabilityInwardsSlidingCurve slidingCurve)
        {
            return CreateParameterAreas(slidingCurve?.Slices, slice => slice.VerticalPorePressure, 0.125);
        }

        /// <summary>
        /// Creates the areas for <see cref="MacroStabilityInwardsSlice.HorizontalPorePressure"/>
        /// values in <paramref name="slidingCurve"/>.
        /// </summary>
        /// <param name="slidingCurve">The <see cref="MacroStabilityInwardsSlidingCurve"/> to 
        /// get the slices from.</param>
        /// <returns>A collection of collections of points in 2D space containing areas representing the
        /// slice output values, or an empty collection when <paramref name="slidingCurve"/> is <c>null</c>.
        /// </returns>
        public static IEnumerable<IEnumerable<Point2D>> CreateHorizontalPorePressureAreas(MacroStabilityInwardsSlidingCurve slidingCurve)
        {
            return CreateParameterAreas(slidingCurve?.Slices, slice => slice.HorizontalPorePressure, 0.125);
        }

        /// <summary>
        /// Creates the areas for <see cref="MacroStabilityInwardsSlice.OverConsolidationRatio"/>
        /// values in <paramref name="slidingCurve"/>.
        /// </summary>
        /// <param name="slidingCurve">The <see cref="MacroStabilityInwardsSlidingCurve"/> to 
        /// get the slices from.</param>
        /// <returns>A collection of collections of points in 2D space containing areas representing the
        /// slice output values, or an empty collection when <paramref name="slidingCurve"/> is <c>null</c>.
        /// </returns>
        public static IEnumerable<IEnumerable<Point2D>> CreateOverConsolidationRatioAreas(MacroStabilityInwardsSlidingCurve slidingCurve)
        {
            return CreateParameterAreas(slidingCurve?.Slices, slice => slice.OverConsolidationRatio, 0.05);
        }

        /// <summary>
        /// Creates the areas for <see cref="MacroStabilityInwardsSlice.Pop"/>
        /// values in <paramref name="slidingCurve"/>.
        /// </summary>
        /// <param name="slidingCurve">The <see cref="MacroStabilityInwardsSlidingCurve"/> to 
        /// get the slices from.</param>
        /// <returns>A collection of collections of points in 2D space containing areas representing the
        /// slice output values, or an empty collection when <paramref name="slidingCurve"/> is <c>null</c>.
        /// </returns>
        public static IEnumerable<IEnumerable<Point2D>> CreatePopAreas(MacroStabilityInwardsSlidingCurve slidingCurve)
        {
            return CreateParameterAreas(slidingCurve?.Slices, slice => slice.Pop, 0.125);
        }

        /// <summary>
        /// Creates the areas for <see cref="MacroStabilityInwardsSlice.NormalStress"/>
        /// values in <paramref name="slidingCurve"/>.
        /// </summary>
        /// <param name="slidingCurve">The <see cref="MacroStabilityInwardsSlidingCurve"/> to 
        /// get the slices from.</param>
        /// <returns>A collection of collections of points in 2D space containing areas representing the
        /// slice output values, or an empty collection when <paramref name="slidingCurve"/> is <c>null</c>.
        /// </returns>
        public static IEnumerable<IEnumerable<Point2D>> CreateNormalStressAreas(MacroStabilityInwardsSlidingCurve slidingCurve)
        {
            return CreateParameterAreas(slidingCurve?.Slices, slice => slice.NormalStress, 0.125);
        }

        /// <summary>
        /// Creates the areas for <see cref="MacroStabilityInwardsSlice.ShearStress"/>
        /// values in <paramref name="slidingCurve"/>.
        /// </summary>
        /// <param name="slidingCurve">The <see cref="MacroStabilityInwardsSlidingCurve"/> to 
        /// get the slices from.</param>
        /// <returns>A collection of collections of points in 2D space containing areas representing the
        /// slice output values, or an empty collection when <paramref name="slidingCurve"/> is <c>null</c>.
        /// </returns>
        public static IEnumerable<IEnumerable<Point2D>> CreateShearStressAreas(MacroStabilityInwardsSlidingCurve slidingCurve)
        {
            return CreateParameterAreas(slidingCurve?.Slices, slice => slice.ShearStress, 0.125);
        }

        /// <summary>
        /// Creates the areas for <see cref="MacroStabilityInwardsSlice.LoadStress"/>
        /// values in <paramref name="slidingCurve"/>.
        /// </summary>
        /// <param name="slidingCurve">The <see cref="MacroStabilityInwardsSlidingCurve"/> to 
        /// get the slices from.</param>
        /// <returns>A collection of collections of points in 2D space containing areas representing the
        /// slice output values, or an empty collection when <paramref name="slidingCurve"/> is <c>null</c>.
        /// </returns>
        public static IEnumerable<IEnumerable<Point2D>> CreateLoadStressAreas(MacroStabilityInwardsSlidingCurve slidingCurve)
        {
            return CreateParameterAreas(slidingCurve?.Slices, slice => slice.LoadStress, 0.125);
        }

        private static IEnumerable<IEnumerable<Point2D>> CreateParameterAreas(IEnumerable<MacroStabilityInwardsSlice> slices,
                                                                              Func<MacroStabilityInwardsSlice, RoundedDouble> getParameterFunc,
                                                                              double scaleFactor)
        {
            if (slices == null || getParameterFunc == null)
            {
                return Enumerable.Empty<Point2D[]>();
            }

            var areas = new List<Point2D[]>();
            foreach (MacroStabilityInwardsSlice slice in slices)
            {
                RoundedDouble value = getParameterFunc(slice);
                if (double.IsNaN(value))
                {
                    value = (RoundedDouble) 0.0;
                }

                value = value.ClipValue((RoundedDouble) (-2000.0), (RoundedDouble) 2000.0);

                double offset = value * scaleFactor;
                double deltaX = slice.BottomLeftPoint.X - slice.BottomRightPoint.X;
                double deltaY = slice.BottomLeftPoint.Y - slice.BottomRightPoint.Y;

                double length = Math.Sqrt(Math.Pow(deltaX, 2) +
                                          Math.Pow(deltaY, 2));

                areas.Add(new[]
                {
                    slice.BottomLeftPoint,
                    slice.BottomRightPoint,
                    new Point2D(slice.BottomRightPoint.X + offset * -deltaY / length,
                                slice.BottomRightPoint.Y + offset * deltaX / length),
                    new Point2D(slice.BottomLeftPoint.X + offset * -deltaY / length,
                                slice.BottomLeftPoint.Y + offset * deltaX / length)
                });
            }

            return areas;
        }
    }
}