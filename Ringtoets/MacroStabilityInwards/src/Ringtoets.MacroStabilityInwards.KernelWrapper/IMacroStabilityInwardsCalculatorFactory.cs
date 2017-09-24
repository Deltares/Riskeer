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

using Ringtoets.MacroStabilityInwards.KernelWrapper.Kernel;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper
{
    /// <summary>
    /// Interface for a factory which creates the calculators from the macro stability inwards kernel.
    /// </summary>
    public interface IMacroStabilityInwardsCalculatorFactory
    {
        /// <summary>
        /// Creates the calculator.
        /// </summary>
        /// <param name="input">The <see cref="MacroStabilityInwardsCalculatorInput"/> containing all the values required
        /// for performing a macro stability inwards calculation.</param>
        /// <param name="factory">The factory responsible for creating the sub calculators.</param>
        /// <returns>The calculator.</returns>
        IMacroStabilityInwardsCalculator CreateCalculator(MacroStabilityInwardsCalculatorInput input, IMacroStabilityInwardsKernelFactory factory);
    }
}