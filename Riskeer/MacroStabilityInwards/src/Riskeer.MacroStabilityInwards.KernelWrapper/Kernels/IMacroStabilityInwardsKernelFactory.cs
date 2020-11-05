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

using Deltares.MacroStability.CSharpWrapper.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.UpliftVan;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.Waternet;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Kernels
{
    /// <summary>
    /// Factory responsible for creating kernels that can be used for performing a macro stability inwards calculation.
    /// </summary>
    public interface IMacroStabilityInwardsKernelFactory
    {
        /// <summary>
        /// Creates an Uplift Van kernel.
        /// </summary>
        /// <param name="kernelInput">The input of the kernel.</param>
        /// <returns>A new <see cref="IUpliftVanKernel"/>.</returns>
        IUpliftVanKernel CreateUpliftVanKernel(MacroStabilityInput kernelInput);

        /// <summary>
        /// Creates a Waternet kernel for extreme circumstances.
        /// </summary>
        /// <param name="kernelInput">The input of the kernel.</param>
        /// <returns>A new <see cref="IWaternetKernel"/>.</returns>
        IWaternetKernel CreateWaternetExtremeKernel(MacroStabilityInput kernelInput);

        /// <summary>
        /// Creates Waternet kernel for daily circumstances.
        /// </summary>
        /// <param name="kernelInput">The input of the kernel.</param>
        /// <returns>A new <see cref="IWaternetKernel"/>.</returns>
        IWaternetKernel CreateWaternetDailyKernel(MacroStabilityInput kernelInput);
    }
}