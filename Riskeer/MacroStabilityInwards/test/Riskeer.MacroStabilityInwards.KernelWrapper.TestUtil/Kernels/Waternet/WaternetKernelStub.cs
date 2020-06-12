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

using System;
using System.Collections.Generic;
using Deltares.MacroStability.Geometry;
using Deltares.MacroStability.WaternetCreator;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.Waternet;
using WtiStabilityWaternet = Deltares.MacroStability.Geometry.Waternet;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.Waternet
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

        public Location Location { get; private set; }

        public SoilProfile2D SoilProfile { get; private set; }

        public SurfaceLine2 SurfaceLine { get; private set; }

        public WtiStabilityWaternet Waternet { get; set; }

        public void SetLocation(Location location)
        {
            Location = location;
        }

        public void SetSoilProfile(SoilProfile2D soilProfile)
        {
            SoilProfile = soilProfile;
        }

        public void SetSurfaceLine(SurfaceLine2 surfaceLine)
        {
            SurfaceLine = surfaceLine;
        }

        public void SetSoilModel(IList<Soil> soilModel)
        {
            
        }

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