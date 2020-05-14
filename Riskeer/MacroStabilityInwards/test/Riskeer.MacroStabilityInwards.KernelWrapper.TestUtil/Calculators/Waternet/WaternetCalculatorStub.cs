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

using System;
using Core.Common.Base.Geometry;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Waternet;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Output;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.Waternet
{
    /// <summary>
    /// Waternet calculator stub for testing purposes.
    /// </summary>
    public class WaternetCalculatorStub : IWaternetCalculator
    {
        /// <summary>
        /// Gets or sets the Waternet calculator input.
        /// </summary>
        public WaternetCalculatorInput Input { get; set; }

        /// <summary>
        /// Gets or sets the Waternet calculator output.
        /// </summary>
        public WaternetCalculatorResult Output { get; set; }

        /// <summary>
        /// Indicator whether an exception must be thrown when performing the calculation.
        /// </summary>
        public bool ThrowExceptionOnCalculate { get; set; }

        public WaternetCalculatorResult Calculate()
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new WaternetCalculatorException($"Message 1{Environment.NewLine}Message 2");
            }

            return Output ?? (Output = CreateWaternetCalculatorResult());
        }

        private static WaternetCalculatorResult CreateWaternetCalculatorResult()
        {
            var phreaticLineResult = new WaternetPhreaticLineResult("Line 1", new[]
            {
                new Point2D(0, 0),
                new Point2D(10, 0)
            });
            return new WaternetCalculatorResult(
                new[]
                {
                    phreaticLineResult
                }, new[]
                {
                    new WaternetLineResult("Line 2", new[]
                    {
                        new Point2D(0, 2),
                        new Point2D(10, 2)
                    }, phreaticLineResult)
                });
        }
    }
}