﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using Ringtoets.MacroStabilityInwards.KernelWrapper.Kernels.UpliftVan;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Kernels.Waternet;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Kernels
{
    /// <summary>
    /// Factory that creates kernels that wrap the WTI macro stability inwards kernel.
    /// </summary>
    public class MacroStabilityInwardsKernelWrapperFactory : IMacroStabilityInwardsKernelFactory
    {
        private static IMacroStabilityInwardsKernelFactory instance;

        private MacroStabilityInwardsKernelWrapperFactory() {}

        /// <summary>
        /// Gets or sets an instance of <see cref="IMacroStabilityInwardsKernelFactory"/>.
        /// </summary>
        public static IMacroStabilityInwardsKernelFactory Instance
        {
            get
            {
                return instance ?? (instance = new MacroStabilityInwardsKernelWrapperFactory());
            }
            set
            {
                instance = value;
            }
        }

        public IUpliftVanKernel CreateUpliftVanKernel()
        {
            return new UpliftVanKernelWrapper();
        }

        public IWaternetKernel CreateWaternetKernel()
        {
            return new WaternetKernelWrapper();
        }
    }
}