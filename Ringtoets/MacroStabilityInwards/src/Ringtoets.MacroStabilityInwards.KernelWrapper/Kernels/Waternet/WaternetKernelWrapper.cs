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
using Deltares.WTIStability.Calculation.Wrapper;
using Deltares.WTIStability.Data.Geo;
using Deltares.WTIStability.IO;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Kernels.Waternet
{
    /// <summary>
    /// Class that wraps <see cref="WTIStabilityCalculation"/> for performing a Waternet calculation.
    /// </summary>
    internal class WaternetKernelWrapper : IWaternetKernel
    {
        private readonly StabilityModel stabilityModel;

        public WaternetKernelWrapper()
        {
            stabilityModel = new StabilityModel();
        }

        public StabilityLocation Location
        {
            set
            {
                stabilityModel.Location = value;
            }
        }

        public SoilModel SoilModel
        {
            set
            {
                stabilityModel.SoilModel = value;
            }
        }

        public SoilProfile2D SoilProfile
        {
            set
            {
                stabilityModel.SoilProfile = value;
            }
        }

        public SurfaceLine2 SurfaceLine
        {
            set
            {
                stabilityModel.SurfaceLine2 = value;
            }
        }

        public void Calculate()
        {
            var waternetCalculation = new WTIStabilityCalculation();
            waternetCalculation.InitializeForDeterministic(WTISerializer.Serialize(stabilityModel));

            string s = waternetCalculation.CreateWaternet(false);

            Console.WriteLine(s);
        }
    }
}