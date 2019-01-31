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

using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Output;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Waternet
{
    /// <summary>
    /// Interface representing a Waternet calculator.
    /// </summary>
    /// <remarks>
    /// This interface is introduced for being able to test the conversion of:
    /// <list type="bullet">
    /// <item>Riskeer macro stability inwards input into calculator input;</item>
    /// <item>calculator output into Riskeer macro stability inwards output.</item>
    /// </list>
    /// </remarks>
    public interface IWaternetCalculator
    {
        /// <summary>
        /// Performs the calculation.
        /// </summary>
        /// <returns>A <see cref="WaternetCalculatorResult"/>.</returns>
        /// <exception cref="WaternetCalculatorException">Thrown when an error
        /// occurs during the calculation.</exception>
        WaternetCalculatorResult Calculate();
    }
}