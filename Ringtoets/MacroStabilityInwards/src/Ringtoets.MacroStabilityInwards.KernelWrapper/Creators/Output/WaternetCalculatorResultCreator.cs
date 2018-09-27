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
using Deltares.WTIStability.Data.Geo;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Output;
using Point2D = Core.Common.Base.Geometry.Point2D;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Creators.Output
{
    /// <summary>
    /// Creates <see cref="WaternetCalculatorResult"/> instances.
    /// </summary>
    internal static class WaternetCalculatorResultCreator
    {
        /// <summary>
        /// Creates a <see cref="WaternetCalculatorResult"/> based on the information
        /// given in the <paramref name="waternet"/>.
        /// </summary>
        /// <param name="waternet">The output to create the result for.</param>
        /// <returns>A new <see cref="WaternetCalculatorResult"/> with information
        /// taken from the <paramref name="waternet"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="waternet"/>
        /// is <c>null</c>.</exception>
        public static WaternetCalculatorResult Create(Waternet waternet)
        {
            if (waternet == null)
            {
                throw new ArgumentNullException(nameof(waternet));
            }

            var phreaticLineLookup = new Dictionary<GeometryPointString, WaternetPhreaticLineResult>
            {
                {
                    waternet.PhreaticLine, CreatePhreaticLine(waternet.PhreaticLine)
                }
            };
            foreach (HeadLine headLine in waternet.HeadLineList)
            {
                phreaticLineLookup.Add(headLine, CreatePhreaticLine(headLine));
            }

            return new WaternetCalculatorResult(phreaticLineLookup.Values,
                                                waternet.WaternetLineList.Select(wl => CreateWaternetLine(wl, phreaticLineLookup)).ToArray());
        }

        private static WaternetLineResult CreateWaternetLine(WaternetLine waternetLine,
                                                             IDictionary<GeometryPointString, WaternetPhreaticLineResult> phreaticLines)
        {
            return new WaternetLineResult(waternetLine.Name,
                                          waternetLine.Points.Select(p => new Point2D(p.X, p.Z)).ToArray(),
                                          phreaticLines[waternetLine.HeadLine]);
        }

        private static WaternetPhreaticLineResult CreatePhreaticLine(GeometryPointString phreaticLine)
        {
            return new WaternetPhreaticLineResult(phreaticLine.Name,
                                                  phreaticLine.Points.Select(p => new Point2D(p.X, p.Z)).ToArray());
        }
    }
}