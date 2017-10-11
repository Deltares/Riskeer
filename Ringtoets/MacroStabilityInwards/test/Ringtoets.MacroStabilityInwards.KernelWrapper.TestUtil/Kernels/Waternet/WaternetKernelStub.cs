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

using System;
using Deltares.WTIStability;
using Deltares.WTIStability.Data.Geo;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Kernels.Waternet;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.Waternet
{
    /// <summary>
    /// Waternet kernel stub for testing purposes.
    /// </summary>
    public class WaternetKernelStub : IWaternetKernel
    {
        /// <summary>
        /// Gets a value indicating whether <see cref="Calculate"/> was called or not.
        /// </summary>
        public bool Calculated { get; private set; }

        /// <summary>
        /// Indicator whether an exception must be thrown when performing the calculation.
        /// </summary>
        public bool ThrowExceptionOnCalculate { get; set; }

        public StabilityLocation Location { get; set; }

        public SoilModel SoilModel { get; set; }

        public SoilProfile2D SoilProfile { get; set; }

        public SurfaceLine2 SurfaceLine { get; set; }

        public void Calculate()
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new WaternetKernelWrapperException($"Message 1{Environment.NewLine}Message 2", new Exception());
            }

            Calculated = true;
        }
    }
}