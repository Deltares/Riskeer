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
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.UpliftVan;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.Waternet;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.UpliftVan;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.Waternet;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels
{
    /// <summary>
    /// Factory which creates macro stability inwards kernel stubs for testing purposes.
    /// </summary>
    public class TestMacroStabilityInwardsKernelFactory : IMacroStabilityInwardsKernelFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestMacroStabilityInwardsKernelFactory"/>.
        /// </summary>
        public TestMacroStabilityInwardsKernelFactory()
        {
            LastCreatedUpliftVanKernel = new UpliftVanKernelStub();
            LastCreatedWaternetExtremeKernel = new WaternetKernelStub();
            LastCreatedWaternetDailyKernel = new WaternetKernelStub();
        }

        /// <summary>
        /// The last created Uplift Van kernel.
        /// </summary>
        public UpliftVanKernelStub LastCreatedUpliftVanKernel { get; }

        /// <summary>
        /// The last created Waternet extreme kernel.
        /// </summary>
        public WaternetKernelStub LastCreatedWaternetExtremeKernel { get; }

        /// <summary>
        /// The last created Waternet daily kernel.
        /// </summary>
        public WaternetKernelStub LastCreatedWaternetDailyKernel { get; }

        public IUpliftVanKernel CreateUpliftVanKernel(MacroStabilityInput kernelInput)
        {
            LastCreatedUpliftVanKernel.SetInput(kernelInput);
            return LastCreatedUpliftVanKernel;
        }

        public IWaternetKernel CreateWaternetExtremeKernel(MacroStabilityInput kernelInput)
        {
            return CreateWaternetKernel(kernelInput, LastCreatedWaternetExtremeKernel);
        }

        public IWaternetKernel CreateWaternetDailyKernel(MacroStabilityInput kernelInput)
        {
            return CreateWaternetKernel(kernelInput, LastCreatedWaternetDailyKernel);
        }

        private static IWaternetKernel CreateWaternetKernel(MacroStabilityInput kernelInput, WaternetKernelStub waternetKernel)
        {
            waternetKernel.SetInput(kernelInput);
            return waternetKernel;
        }
    }
}