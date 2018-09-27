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
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Kernels;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Kernels.Waternet;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Waternet
{
    /// <summary>
    /// Class representing a Waternet calculator for extreme circumstances.
    /// </summary>
    public class WaternetExtremeCalculator : WaternetCalculator
    {
        /// <summary>
        /// Creates a new instance of <see cref="WaternetExtremeCalculator"/>.
        /// </summary>
        /// <param name="input">The <see cref="WaternetCalculatorInput"/> containing all the values
        /// required for performing the Waternet calculation.</param>
        /// <param name="factory">The factory responsible for creating the Waternet kernel.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter
        /// is <c>null</c>.</exception>
        public WaternetExtremeCalculator(WaternetCalculatorInput input, IMacroStabilityInwardsKernelFactory factory)
            : base(input, factory) {}

        protected override IWaternetKernel CreateWaternetKernel()
        {
            return Factory.CreateWaternetExtremeKernel();
        }
    }
}