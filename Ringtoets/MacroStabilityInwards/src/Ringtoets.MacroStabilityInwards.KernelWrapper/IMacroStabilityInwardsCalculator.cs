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

using System.Collections.Generic;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper
{
    /// <summary>
    /// Interface that represents a combination of macro stability inwards sub calculations, which together can be used
    /// to assess based on macro stability inwards.
    /// </summary>
    public interface IMacroStabilityInwardsCalculator
    {
        /// <summary>
        /// Performs the actual sub calculations and returns a <see cref="MacroStabilityInwardsCalculatorResult"/>, which
        /// contains the results of all sub calculations.
        /// </summary>
        /// <returns>A <see cref="MacroStabilityInwardsCalculatorResult"/> containing the results of the sub calculations.</returns>
        MacroStabilityInwardsCalculatorResult Calculate();

        /// <summary>
        /// Returns a list of validation messages. The validation messages are based on the values of the <see cref="MacroStabilityInwardsCalculatorInput"/>
        /// which was provided to this <see cref="MacroStabilityInwardsCalculator"/> and are determined by the kernel.
        /// </summary>
        List<string> Validate();
    }
}