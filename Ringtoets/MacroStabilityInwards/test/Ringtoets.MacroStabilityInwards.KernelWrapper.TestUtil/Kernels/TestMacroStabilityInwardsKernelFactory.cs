﻿// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using Ringtoets.MacroStabilityInwards.KernelWrapper.Kernels;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Kernels.UpliftVan;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Kernels.Waternet;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.UpliftVan;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.Waternet;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels
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
            LastCreatedWaternetKernel = new WaternetKernelStub();
        }

        /// <summary>
        /// The last created Uplift Van kernel.
        /// </summary>
        public UpliftVanKernelStub LastCreatedUpliftVanKernel { get; }

        /// <summary>
        /// The last created Waternet kernel.
        /// </summary>
        public WaternetKernelStub LastCreatedWaternetKernel { get; }

        public IUpliftVanKernel CreateUpliftVanKernel()
        {
            return LastCreatedUpliftVanKernel;
        }

        public IWaternetKernel CreateWaternetExtremeKernel()
        {
            return LastCreatedWaternetKernel;
        }

        public IWaternetKernel CreateWaternetDailyKernel()
        {
            return LastCreatedWaternetKernel;
        }
    }
}