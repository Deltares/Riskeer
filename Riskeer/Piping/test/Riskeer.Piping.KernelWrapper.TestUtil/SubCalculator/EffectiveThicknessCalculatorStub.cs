﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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

using System.Collections.Generic;
using Deltares.WTIPiping;
using Riskeer.Piping.KernelWrapper.SubCalculator;

namespace Riskeer.Piping.KernelWrapper.TestUtil.SubCalculator
{
    /// <summary>
    /// Stub for the real effective thickness sub calculator of piping.
    /// </summary>
    public class EffectiveThicknessCalculatorStub : IEffectiveThicknessCalculator
    {
        /// <summary>
        /// Gets a value indicating whether <see cref="Calculate"/> was called or not.
        /// </summary>
        public bool Calculated { get; private set; }

        /// <summary>
        /// Gets a value indicating whether <see cref="Validate"/> was called or not.
        /// </summary>
        public bool Validated { get; private set; }

        public double ExitPointXCoordinate { get; private set; }
        public double PhreaticLevel { get; private set; }
        public double VolumicWeightOfWater { get; private set; }
        public PipingProfile SoilProfile { get; private set; }
        public PipingSurfaceLine SurfaceLine { get; private set; }

        public double EffectiveHeight
        {
            get
            {
                return 0.1;
            }
        }

        public void SetExitPointXCoordinate(double exitPointXCoordinate)
        {
            ExitPointXCoordinate = exitPointXCoordinate;
        }

        public void SetPhreaticLevel(double phreaticLevel)
        {
            PhreaticLevel = phreaticLevel;
        }

        public void SetVolumicWeightOfWater(double volumicWeightOfWater)
        {
            VolumicWeightOfWater = volumicWeightOfWater;
        }

        public void SetSoilProfile(PipingProfile soilProfile)
        {
            SoilProfile = soilProfile;
        }

        public void SetSurfaceLine(PipingSurfaceLine surfaceLine)
        {
            SurfaceLine = surfaceLine;
        }

        public List<string> Validate()
        {
            Validated = true;
            return new List<string>();
        }

        public void Calculate()
        {
            Calculated = true;
        }
    }
}