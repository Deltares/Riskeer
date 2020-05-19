// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Base.Geometry;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Output;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.Waternet.Output
{
    /// <summary>
    /// Factory to create simple <see cref="WaternetCalculatorResult"/> instances that can be used for testing.
    /// </summary>
    public static class WaternetCalculatorResultTestFactory
    {
        /// <summary>
        /// Creates a new <see cref="WaternetCalculatorResult"/>.
        /// </summary>
        /// <returns>The created <see cref="WaternetCalculatorResult"/>.</returns>
        public static WaternetCalculatorResult Create()
        {
            var phreaticLine = new WaternetPhreaticLineResult("Line 1", new[]
            {
                new Point2D(0, 0),
                new Point2D(10, 0)
            });

            return new WaternetCalculatorResult(new[]
            {
                phreaticLine
            }, new[]
            {
                new WaternetLineResult("Line 2", new[]
                {
                    new Point2D(2, 2),
                    new Point2D(3, 3)
                }, phreaticLine)
            });
        }

        /// <summary>
        /// Creates a new <see cref="WaternetCalculatorResult"/> with an empty phreatic lines and Waternet lines collections.
        /// </summary>
        /// <returns>The created <see cref="WaternetCalculatorResult"/>.</returns>
        public static WaternetCalculatorResult CreateEmptyResult()
        {
            return new WaternetCalculatorResult(new List<WaternetPhreaticLineResult>(), new List<WaternetLineResult>());
        }
    }
}