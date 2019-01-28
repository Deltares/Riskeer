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
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Waternet;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Kernels;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators
{
    /// <summary>
    /// Interface for a factory which creates calculators for performing macro stability inwards calculations.
    /// </summary>
    public interface IMacroStabilityInwardsCalculatorFactory
    {
        /// <summary>
        /// Creates an Uplift Van calculator.
        /// </summary>
        /// <param name="input">The <see cref="UpliftVanCalculatorInput"/> containing all the values required
        /// for performing an Uplift Van calculation.</param>
        /// <param name="factory">The factory responsible for creating the Uplift Van kernel.</param>
        /// <returns>The Uplift Van calculator.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter
        /// is <c>null</c>.</exception>
        IUpliftVanCalculator CreateUpliftVanCalculator(UpliftVanCalculatorInput input, IMacroStabilityInwardsKernelFactory factory);

        /// <summary>
        /// Creates a Waternet calculator for the extreme circumstances.
        /// </summary>
        /// <param name="input">The <see cref="WaternetCalculatorInput"/> containing all the values required
        /// for performing a Waternet calculation.</param>
        /// <param name="factory">The factory responsible for creating the Waternet kernel.</param>
        /// <returns>The Waternet calculator.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter
        /// is <c>null</c>.</exception>
        IWaternetCalculator CreateWaternetExtremeCalculator(WaternetCalculatorInput input, IMacroStabilityInwardsKernelFactory factory);

        /// <summary>
        /// Creates a Waternet calculator for the daily circumstances.
        /// </summary>
        /// <param name="input">The <see cref="WaternetCalculatorInput"/> containing all the values required
        /// for performing a Waternet calculation.</param>
        /// <param name="factory">The factory responsible for creating the Waternet kernel.</param>
        /// <returns>The Waternet calculator.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter
        /// is <c>null</c>.</exception>
        IWaternetCalculator CreateWaternetDailyCalculator(WaternetCalculatorInput input, IMacroStabilityInwardsKernelFactory factory);
    }
}